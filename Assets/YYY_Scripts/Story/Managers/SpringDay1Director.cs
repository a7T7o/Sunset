using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FarmGame.Data;
using FarmGame.Farm;
using Sunset.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story
{
    /// <summary>
    /// spring-day1 运行时导演。
    /// 当前优先把 0.0.3 ~ 0.0.6 串成可推进骨架，并尽量复用现有系统。
    /// </summary>
    public class SpringDay1Director : MonoBehaviour
    {
        private const string PrimarySceneName = "Primary";
        private const string FirstSequenceId = "spring-day1-first";
        private const string HealingSequenceId = "spring-day1-healing";
        private const string WorkbenchSequenceId = "spring-day1-workbench";
        private const string DinnerSequenceId = "spring-day1-dinner";
        private const string ReminderSequenceId = "spring-day1-reminder";

        private static SpringDay1Director _instance;
        private static FieldInfo _farmTilesField;

        [Header("Healing")]
        [SerializeField] private int initialHealth = 60;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int healedHealth = 85;

        [Header("Energy")]
        [SerializeField] private int initialEnergy = 80;
        [SerializeField] private int maxEnergy = 200;
        [SerializeField] private int dinnerRestoreEnergy = 30;
        [SerializeField] private int lowEnergyWarningThreshold = 20;

        [Header("Minimal Goals")]
        [SerializeField] private int requiredTilledCount = 1;
        [SerializeField] private int requiredPlantedCount = 1;
        [SerializeField] private int requiredWateredCount = 1;
        [SerializeField] private int requiredTreeChoppedCount = 1;
        [SerializeField] private int requiredCraftedCount = 1;

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        private bool _healingStarted;
        private bool _healingSequencePlayed;
        private bool _workbenchSequencePlayed;
        private bool _dinnerSequencePlayed;
        private bool _returnSequencePlayed;
        private bool _freeTimeEntered;
        private bool _dayEnded;

        private bool _staminaRevealed;
        private bool _lowEnergyWarned;
        private bool _workbenchOpened;
        private bool _uiInitialized;

        private int _craftedCount;
        private float _nextPollAt;

        public static SpringDay1Director Instance => _instance;

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            SpringDay1Director existing = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            GameObject runtimeObject = new GameObject(nameof(SpringDay1Director));
            _instance = runtimeObject.AddComponent<SpringDay1Director>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            CacheReflection();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueSequenceCompletedEvent>(HandleDialogueSequenceCompleted, owner: this);
            EventBus.Subscribe<StoryPhaseChangedEvent>(HandleStoryPhaseChanged, owner: this);
            EnergySystem.OnEnergyChanged += HandleEnergyChanged;
            TimeManager.OnSleep += HandleSleep;

            CraftingService craftingService = FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            if (craftingService != null)
            {
                craftingService.OnCraftSuccess += HandleCraftSuccess;
            }
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            EnergySystem.OnEnergyChanged -= HandleEnergyChanged;
            TimeManager.OnSleep -= HandleSleep;

            CraftingService craftingService = FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            if (craftingService != null)
            {
                craftingService.OnCraftSuccess -= HandleCraftSuccess;
            }
        }

        private void Update()
        {
            if (!IsPrimarySceneActive())
            {
                return;
            }

            if (Time.unscaledTime < _nextPollAt)
            {
                return;
            }

            _nextPollAt = Time.unscaledTime + 0.2f;
            TickPrimarySceneFlow();
        }

        public string GetDebugSummary()
        {
            return $"Phase={StoryManager.Instance.CurrentPhase}, Tilled={GetTilledCount()}, Planted={GetPlantedCount()}, Watered={GetWateredCount()}, Trees={GetTreeStumpCount()}, Crafted={_craftedCount}, FreeTime={_freeTimeEntered}, DayEnd={_dayEnded}";
        }

        private void HandleStoryPhaseChanged(StoryPhaseChangedEvent evt)
        {
            if (!IsPrimarySceneActive())
            {
                return;
            }

            if (showDebugLog)
            {
                Debug.Log($"[SpringDay1Director] Phase: {evt.PreviousPhase} -> {evt.CurrentPhase}");
            }

            if (evt.CurrentPhase == StoryPhase.EnterVillage)
            {
                BeginHealingAndHp();
            }
        }

        private void HandleDialogueSequenceCompleted(DialogueSequenceCompletedEvent evt)
        {
            if (!IsPrimarySceneActive() || evt == null)
            {
                return;
            }

            if (evt.SequenceId == FirstSequenceId)
            {
                BeginHealingAndHp();
                return;
            }

            if (evt.SequenceId == HealingSequenceId)
            {
                StoryManager.Instance.SetPhase(StoryPhase.WorkbenchFlashback);
                SpringDay1PromptOverlay.Instance.Show("靠近工作台，尝试打开制作菜单。");
                return;
            }

            if (evt.SequenceId == WorkbenchSequenceId)
            {
                StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);
                SpringDay1PromptOverlay.Instance.Show("先用锄头开垦一格土地。");
                return;
            }

            if (evt.SequenceId == DinnerSequenceId)
            {
                BeginReturnReminder();
                return;
            }

            if (evt.SequenceId == ReminderSequenceId)
            {
                EnterFreeTime();
            }
        }

        private void HandleCraftSuccess(RecipeData recipe, CraftResult result)
        {
            _craftedCount++;

            if (showDebugLog)
            {
                Debug.Log($"[SpringDay1Director] Craft success: {recipe?.recipeName} x{result.resultAmount}");
            }
        }

        private void HandleEnergyChanged(int current, int max)
        {
            if (_staminaRevealed && !_lowEnergyWarned && current > 0 && current <= lowEnergyWarningThreshold)
            {
                _lowEnergyWarned = true;
                SpringDay1PromptOverlay.Instance.Show("精力过低，先休息或吃点东西。");
            }
            else if (current > lowEnergyWarningThreshold)
            {
                _lowEnergyWarned = false;
            }
        }

        private void HandleSleep()
        {
            if (_freeTimeEntered && !_dayEnded)
            {
                _dayEnded = true;
                StoryManager.Instance.SetPhase(StoryPhase.DayEnd);
                SpringDay1PromptOverlay.Instance.Show("春1日结束。明天继续。");
            }
        }

        private void TickPrimarySceneFlow()
        {
            InitializeRuntimeUiIfNeeded();

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;

            if (currentPhase == StoryPhase.HealingAndHP || currentPhase == StoryPhase.WorkbenchFlashback)
            {
                TryHandleWorkbenchFlashback();
            }

            if (currentPhase == StoryPhase.FarmingTutorial)
            {
                TickFarmingTutorial();
            }

            if (currentPhase == StoryPhase.DinnerConflict && !_dinnerSequencePlayed)
            {
                _dinnerSequencePlayed = true;
                QueueDialogue(BuildDinnerSequence());
            }
        }

        private void BeginHealingAndHp()
        {
            if (_healingStarted)
            {
                return;
            }

            _healingStarted = true;
            StoryManager.Instance.SetPhase(StoryPhase.HealingAndHP);

            HealthSystem healthSystem = HealthSystem.Instance;
            healthSystem.SetHealthState(initialHealth, maxHealth);
            healthSystem.SetVisible(true);

            EnergySystem.Instance.SetVisible(false);

            StartCoroutine(HealAndPromptCoroutine());
        }

        private void TryHandleWorkbenchFlashback()
        {
            CraftingService craftingService = FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            CraftingPanel craftingPanel = FindFirstObjectByType<CraftingPanel>(FindObjectsInactive.Include);

            if (craftingService == null || craftingPanel == null || !craftingPanel.gameObject.activeInHierarchy)
            {
                return;
            }

            if (craftingService.CurrentStation != CraftingStation.Workbench)
            {
                return;
            }

            if (_workbenchOpened)
            {
                return;
            }

            _workbenchOpened = true;

            if (!_workbenchSequencePlayed)
            {
                _workbenchSequencePlayed = true;
                QueueDialogue(BuildWorkbenchSequence());
            }
        }

        private void InitializeRuntimeUiIfNeeded()
        {
            if (_uiInitialized)
            {
                return;
            }

            _uiInitialized = true;
            SpringDay1PromptOverlay.EnsureRuntime();
            HealthSystem.Instance.SetVisible(false);
            EnergySystem.Instance.SetVisible(false);
        }

        private IEnumerator HealAndPromptCoroutine()
        {
            yield return null;

            HealthSystem.Instance.SetHealthState(healedHealth, maxHealth);

            if (!_healingSequencePlayed)
            {
                _healingSequencePlayed = true;
                QueueDialogue(BuildHealingSequence());
            }
        }

        private void TickFarmingTutorial()
        {
            int tilledCount = GetTilledCount();
            int plantedCount = GetPlantedCount();
            int wateredCount = GetWateredCount();
            int treeCount = GetTreeStumpCount();

            if (!_staminaRevealed && tilledCount > 0)
            {
                _staminaRevealed = true;
                EnergySystem.Instance.SetEnergyState(initialEnergy, maxEnergy);
                EnergySystem.Instance.SetVisible(true);
            }

            if (tilledCount < requiredTilledCount)
            {
                SpringDay1PromptOverlay.Instance.Show("先用锄头开垦一格土地。");
                return;
            }

            if (plantedCount < requiredPlantedCount)
            {
                SpringDay1PromptOverlay.Instance.Show("把花椰菜种子种进刚开垦的土地。");
                return;
            }

            if (wateredCount < requiredWateredCount)
            {
                SpringDay1PromptOverlay.Instance.Show("用浇水壶给作物浇水。");
                return;
            }

            if (treeCount < requiredTreeChoppedCount)
            {
                SpringDay1PromptOverlay.Instance.Show("再砍倒一棵树，收集一些木料。");
                return;
            }

            if (_craftedCount < requiredCraftedCount)
            {
                SpringDay1PromptOverlay.Instance.Show("回到工作台，完成一次基础制作。");
                return;
            }

            SpringDay1PromptOverlay.Instance.Hide();
            StoryManager.Instance.SetPhase(StoryPhase.DinnerConflict);
        }

        private void BeginReturnReminder()
        {
            StoryManager.Instance.SetPhase(StoryPhase.ReturnAndReminder);
            EnergySystem.Instance.RestoreEnergy(dinnerRestoreEnergy);

            if (!_returnSequencePlayed)
            {
                _returnSequencePlayed = true;
                QueueDialogue(BuildReminderSequence());
            }
        }

        private void EnterFreeTime()
        {
            if (_freeTimeEntered)
            {
                return;
            }

            _freeTimeEntered = true;
            StoryManager.Instance.SetPhase(StoryPhase.FreeTime);
            SpringDay1PromptOverlay.Instance.Show("今晚可以自由活动，记得回床边睡觉。");
        }

        private void QueueDialogue(DialogueSequenceSO sequence)
        {
            if (sequence == null)
            {
                return;
            }

            StartCoroutine(PlayDialogueWhenReady(sequence));
        }

        private IEnumerator PlayDialogueWhenReady(DialogueSequenceSO sequence)
        {
            while (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                yield return null;
            }

            DialogueManager.Instance.PlayDialogue(sequence);
        }

        private DialogueSequenceSO BuildHealingSequence()
        {
            return CreateSequence(
                HealingSequenceId,
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "伤口已经简单处理好了。至少今晚不会继续恶化。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "谢谢你……我感觉比刚醒来的时候好多了。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（先活下来，然后想办法弄明白这里的一切。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                });
        }

        private DialogueSequenceSO BuildWorkbenchSequence()
        {
            return CreateSequence(
                WorkbenchSequenceId,
                new DialogueNode
                {
                    speakerName = "",
                    text = "（这张工作台让我想起了某些很熟悉的流程：木料、工具、配方……）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "也许我能靠它先做些最基础的东西。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildDinnerSequence()
        {
            return CreateSequence(
                DinnerSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "今天先到这里吧。你已经撑过最难的一天了。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "卡尔",
                    text = "哼，希望你明天还能像现在这样老实。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "我会证明自己不是来惹麻烦的。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildReminderSequence()
        {
            return CreateSequence(
                ReminderSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "夜里别逗留太久。两点之前，记得回去睡觉。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "我记住了。今晚先把状态调整回来。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO CreateSequence(string sequenceId, params DialogueNode[] nodes)
        {
            DialogueSequenceSO sequence = ScriptableObject.CreateInstance<DialogueSequenceSO>();
            sequence.sequenceId = sequenceId;
            sequence.nodes = new List<DialogueNode>(nodes);
            sequence.canSkip = true;
            sequence.defaultTypingSpeed = 40f;
            return sequence;
        }

        private static void CacheReflection()
        {
            if (_farmTilesField == null)
            {
                _farmTilesField = typeof(FarmTileManager).GetField("farmTilesByLayer", BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }

        private int GetTilledCount()
        {
            int tilledCount = 0;
            foreach (FarmTileData tileData in EnumerateFarmTiles())
            {
                if (tileData != null && tileData.isTilled)
                {
                    tilledCount++;
                }
            }

            return tilledCount;
        }

        private int GetPlantedCount()
        {
            int plantedCount = 0;
            foreach (FarmTileData tileData in EnumerateFarmTiles())
            {
                if (tileData != null && tileData.cropData != null)
                {
                    plantedCount++;
                }
            }

            return plantedCount;
        }

        private int GetWateredCount()
        {
            int wateredCount = 0;
            foreach (FarmTileData tileData in EnumerateFarmTiles())
            {
                if (tileData != null && (tileData.wateredToday || tileData.wateredYesterday))
                {
                    wateredCount++;
                }
            }

            return wateredCount;
        }

        private int GetTreeStumpCount()
        {
            int stumpCount = 0;
            TreeController[] trees = FindObjectsByType<TreeController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TreeController tree in trees)
            {
                if (tree != null && tree.GetCurrentState().ToString() == "Stump")
                {
                    stumpCount++;
                }
            }

            return stumpCount;
        }

        private IEnumerable<FarmTileData> EnumerateFarmTiles()
        {
            CacheReflection();

            FarmTileManager farmTileManager = FarmTileManager.Instance;
            if (farmTileManager == null || _farmTilesField == null)
            {
                yield break;
            }

            if (_farmTilesField.GetValue(farmTileManager) is not IDictionary dictionary)
            {
                yield break;
            }

            foreach (DictionaryEntry layerEntry in dictionary)
            {
                if (layerEntry.Value is not IDictionary tileDictionary)
                {
                    continue;
                }

                foreach (DictionaryEntry tileEntry in tileDictionary)
                {
                    if (tileEntry.Value is FarmTileData tileData)
                    {
                        yield return tileData;
                    }
                }
            }
        }

        private static bool IsPrimarySceneActive()
        {
            return SceneManager.GetActiveScene().name == PrimarySceneName;
        }
    }
}
