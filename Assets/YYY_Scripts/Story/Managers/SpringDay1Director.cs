using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Serialization;
using FarmGame.Data;
using FarmGame.Farm;
using Sunset.Events;
using TMPro;
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
        private const string StoryTimePauseSource = "SpringDay1Director";
        private const string FirstSequenceId = "spring-day1-first";
        private const string HealingSequenceId = "spring-day1-healing";
        private const string WorkbenchSequenceId = "spring-day1-workbench";
        private const string DinnerSequenceId = "spring-day1-dinner";
        private const string ReminderSequenceId = "spring-day1-reminder";
        private static readonly string[] PreferredWorkbenchObjectNames = { "Anvil_0", "Workbench", "Anvil" };
        private static readonly string[] PreferredBedObjectNames = { "Bed", "PlayerBed", "HomeBed" };
        private static readonly string[] PreferredRestProxyObjectNames = { "House 1_2", "HomeDoor", "HouseDoor", "Door" };
        private static readonly string[] PreferredRestProxyKeywords = { "door", "entry", "entrance", "house", "home" };
        private const string BedInteractionHint = "睡觉";
        private const string RestProxyInteractionHint = "回屋休息";
        private const string FreeTimePromptText = "今晚可以自由活动，记得回住处休息。";
        private const string FreeTimeProgressText = "自由活动中，回住处休息即可结束";

        private static SpringDay1Director _instance;
        private static FieldInfo _farmTilesField;

        [Header("Healing")]
        [SerializeField] private int initialHealth = 60;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int healedHealth = 85;
        [SerializeField] private float healthRevealDuration = 0.35f;
        [SerializeField] private float healthRecoveryDuration = 0.95f;
        [SerializeField] private float postHealDialogueDelay = 0.12f;

        [Header("Energy")]
        [SerializeField] private int initialEnergy = 80;
        [SerializeField] private int maxEnergy = 200;
        [SerializeField] private int dinnerRestoreEnergy = 30;
        [SerializeField] private int lowEnergyWarningThreshold = 20;
        [SerializeField] private float energyRevealDuration = 0.25f;
        [SerializeField] private float dinnerRestoreDuration = 0.9f;
        [SerializeField, Range(0.1f, 1f)] private float lowEnergyMoveSpeedMultiplier = 0.8f;

        [Header("Minimal Goals")]
        [SerializeField] private int requiredTilledCount = 1;
        [SerializeField] private int requiredPlantedCount = 1;
        [SerializeField] private int requiredWateredCount = 1;
        [FormerlySerializedAs("requiredTreeChoppedCount")]
        [SerializeField] private int requiredWoodCollectedCount = 3;
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
        private CraftingStationInteractable _boundWorkbenchInteractable;
        private SpringDay1BedInteractable _boundBedInteractable;
        private PlayerMovement _playerMovement;
        private CraftingService _observedCraftingService;
        private InventoryService _observedInventoryService;
        private bool _uiInitialized;
        private bool _storyTimePauseApplied;

        private int _craftedCount;
        private float _nextPollAt;
        private bool _farmingTutorialTrackingInitialized;
        private bool _tillObjectiveCompleted;
        private bool _plantObjectiveCompleted;
        private bool _waterObjectiveCompleted;
        private bool _woodObjectiveCompleted;
        private int _baselineWoodCount = -1;
        private bool _woodTrackingArmed;
        private int _trackedWoodCountSnapshot = -1;
        private int _collectedWoodSinceWoodStepStart;

        public static SpringDay1Director Instance => _instance;
        private const int WoodItemId = 3200;

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
            RefreshCraftingServiceSubscription();
            RefreshInventoryTrackingSubscription();
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            EnergySystem.OnEnergyChanged -= HandleEnergyChanged;
            TimeManager.OnSleep -= HandleSleep;
            RefreshCraftingServiceSubscription(null);
            RefreshInventoryTrackingSubscription(null);

            ApplyLowEnergyMovementPenalty(false);
            ReleaseStoryTimePause();
        }

        private void Update()
        {
            if (!IsPrimarySceneActive())
            {
                ReleaseStoryTimePause();
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
            return $"Phase={StoryManager.Instance.CurrentPhase}, Tilled={GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}, Planted={GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}, Watered={GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}, Wood={GetCollectedWoodProgress()}/{requiredWoodCollectedCount}, Crafted={_craftedCount}, FreeTime={_freeTimeEntered}, DayEnd={_dayEnded}";
        }

        public string GetCurrentTaskLabel()
        {
            return StoryManager.Instance.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => "0.0.2 首段推进链",
                StoryPhase.EnterVillage => "0.0.2 首段推进链",
                StoryPhase.HealingAndHP => "0.0.3 疗伤/血条",
                StoryPhase.WorkbenchFlashback => "0.0.4 工作台闪回",
                StoryPhase.FarmingTutorial => "0.0.5 农田/砍树教学",
                StoryPhase.DinnerConflict => "0.0.6 晚餐冲突",
                StoryPhase.ReturnAndReminder => "0.0.6 归途提醒",
                StoryPhase.FreeTime => "0.0.6 自由时段",
                StoryPhase.DayEnd => "0.0.6 睡觉结束",
                _ => "未开始"
            };
        }

        public string GetCurrentProgressLabel()
        {
            StoryPhase phase = StoryManager.Instance.CurrentPhase;
            if (phase == StoryPhase.CrashAndMeet || phase == StoryPhase.EnterVillage)
            {
                return "首段对话进行中";
            }

            if (phase == StoryPhase.HealingAndHP)
            {
                return "等待疗伤对话结束";
            }

            if (phase == StoryPhase.WorkbenchFlashback)
            {
                return _workbenchOpened ? "已打开工作台" : "等待打开工作台";
            }

            if (phase == StoryPhase.FarmingTutorial)
            {
                return $"开垦 {GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}/{requiredTilledCount} | 播种 {GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}/{requiredPlantedCount} | 浇水 {GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}/{requiredWateredCount} | 木材 {GetCollectedWoodProgress()}/{requiredWoodCollectedCount} | 制作 {Mathf.Min(_craftedCount, requiredCraftedCount)}/{requiredCraftedCount}";
            }

            if (phase == StoryPhase.DinnerConflict)
            {
                return "晚餐事件进行中";
            }

            if (phase == StoryPhase.ReturnAndReminder)
            {
                return "归途提醒进行中";
            }

            if (phase == StoryPhase.FreeTime)
            {
                return FreeTimeProgressText;
            }

            if (phase == StoryPhase.DayEnd)
            {
                return "春1日已结束";
            }

            return "等待推进";
        }

        private void HandleStoryPhaseChanged(StoryPhaseChangedEvent evt)
        {
            if (!IsPrimarySceneActive())
            {
                return;
            }

            SyncStoryTimePauseState();

            if (showDebugLog)
            {
                Debug.Log($"[SpringDay1Director] Phase: {evt.PreviousPhase} -> {evt.CurrentPhase}");
            }

            if (evt.CurrentPhase == StoryPhase.EnterVillage)
            {
                BeginHealingAndHp();
                return;
            }

            if (evt.CurrentPhase == StoryPhase.FarmingTutorial)
            {
                InitializeFarmingTutorialTracking(true);
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
                InitializeFarmingTutorialTracking(true);
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

        private void RefreshCraftingServiceSubscription(CraftingService targetService = null)
        {
            CraftingService resolvedService = targetService ?? FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
            if (_observedCraftingService == resolvedService)
            {
                return;
            }

            if (_observedCraftingService != null)
            {
                _observedCraftingService.OnCraftSuccess -= HandleCraftSuccess;
            }

            _observedCraftingService = resolvedService;

            if (_observedCraftingService != null)
            {
                _observedCraftingService.OnCraftSuccess += HandleCraftSuccess;
            }
        }

        private void RefreshInventoryTrackingSubscription(InventoryService targetService = null)
        {
            InventoryService resolvedService = targetService ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
            if (_observedInventoryService == resolvedService)
            {
                return;
            }

            if (_observedInventoryService != null)
            {
                _observedInventoryService.OnInventoryChanged -= HandleInventoryChanged;
            }

            _observedInventoryService = resolvedService;

            if (_observedInventoryService != null)
            {
                _observedInventoryService.OnInventoryChanged += HandleInventoryChanged;
            }
        }

        private void HandleInventoryChanged()
        {
            if (!_woodTrackingArmed || _woodObjectiveCompleted)
            {
                return;
            }

            int currentWoodCount = GetCurrentWoodCount();
            if (_trackedWoodCountSnapshot < 0)
            {
                _trackedWoodCountSnapshot = currentWoodCount;
            }

            int delta = currentWoodCount - _trackedWoodCountSnapshot;
            if (delta > 0)
            {
                _collectedWoodSinceWoodStepStart += delta;
                if (_collectedWoodSinceWoodStepStart >= requiredWoodCollectedCount)
                {
                    _woodObjectiveCompleted = true;
                    _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
                }
            }

            _trackedWoodCountSnapshot = currentWoodCount;
        }

        private void HandleEnergyChanged(int current, int max)
        {
            bool shouldWarn = _staminaRevealed && current > 0 && current <= lowEnergyWarningThreshold;
            EnergySystem.Instance.SetLowEnergyWarningVisual(shouldWarn);
            ApplyLowEnergyMovementPenalty(shouldWarn);

            if (shouldWarn && !_lowEnergyWarned)
            {
                _lowEnergyWarned = true;
                SpringDay1PromptOverlay.Instance.Show("精力过低，先休息或吃点东西。");
            }
            else if (!shouldWarn)
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
                EnergySystem.Instance.FullRestore();
                ApplyLowEnergyMovementPenalty(false);
                SyncStoryTimePauseState();
                SpringDay1PromptOverlay.Instance.Show("春1日结束。明天继续。");
            }
        }

        private void TickPrimarySceneFlow()
        {
            InitializeRuntimeUiIfNeeded();
            RefreshCraftingServiceSubscription();
            RefreshInventoryTrackingSubscription();
            TryAutoBindWorkbenchInteractable();
            TryAutoBindBedInteractable();
            SyncStoryTimePauseState();

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
            SyncStoryTimePauseState();

            HealthSystem healthSystem = HealthSystem.Instance;
            healthSystem.SetHealthState(initialHealth, maxHealth);
            healthSystem.SetVisible(false);

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

            NotifyCraftingStationOpened(craftingService.CurrentStation);
        }

        public void NotifyCraftingStationOpened(CraftingStation station)
        {
            if (station != CraftingStation.Workbench)
            {
                return;
            }

            _workbenchOpened = true;

            if (StoryManager.Instance.CurrentPhase != StoryPhase.WorkbenchFlashback || _workbenchSequencePlayed)
            {
                return;
            }

            _workbenchSequencePlayed = true;
            QueueDialogue(BuildWorkbenchSequence());
        }

        private void InitializeRuntimeUiIfNeeded()
        {
            if (_uiInitialized)
            {
                return;
            }

            _uiInitialized = true;
            SpringDay1PromptOverlay.EnsureRuntime();
            HealthSystem healthSystem = HealthSystem.Instance;
            if (healthSystem != null)
            {
                healthSystem.SetVisible(false);
            }

            EnergySystem energySystem = EnergySystem.Instance;
            if (energySystem != null)
            {
                energySystem.SetVisible(false);
            }
            SyncStoryTimePauseState();
        }

        private void TryAutoBindWorkbenchInteractable()
        {
            if (_boundWorkbenchInteractable != null)
            {
                return;
            }

            Transform workbenchCandidate = FindWorkbenchCandidate();
            if (workbenchCandidate == null || workbenchCandidate.GetComponent<Collider2D>() == null)
            {
                return;
            }

            CraftingStationInteractable interactable = workbenchCandidate.GetComponent<CraftingStationInteractable>();
            if (interactable == null)
            {
                interactable = workbenchCandidate.gameObject.AddComponent<CraftingStationInteractable>();
            }

            interactable.ConfigureRuntimeDefaults(CraftingStation.Workbench, "使用工作台", 0.5f, 28);
            _boundWorkbenchInteractable = interactable;
        }

        private void TryAutoBindBedInteractable()
        {
            if (_boundBedInteractable != null)
            {
                return;
            }

            RestTargetBinding binding = FindRestTargetCandidate();
            if (binding.Transform == null)
            {
                return;
            }

            Collider2D interactableCollider = EnsureRestInteractableCollider(binding);
            if (interactableCollider == null)
            {
                return;
            }

            SpringDay1BedInteractable interactable = binding.Transform.GetComponent<SpringDay1BedInteractable>();
            if (interactable == null)
            {
                interactable = binding.Transform.gameObject.AddComponent<SpringDay1BedInteractable>();
            }

            interactable.ConfigureRuntimeDefaults(binding.InteractionHint, binding.InteractionDistance, binding.InteractionPriority);
            _boundBedInteractable = interactable;
        }

        private IEnumerator HealAndPromptCoroutine()
        {
            yield return null;

            yield return HealthSystem.Instance.PlayRevealAndAnimateTo(
                initialHealth,
                healedHealth,
                maxHealth,
                healthRevealDuration,
                healthRecoveryDuration);

            if (postHealDialogueDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(postHealDialogueDelay);
            }

            if (!_healingSequencePlayed)
            {
                _healingSequencePlayed = true;
                QueueDialogue(BuildHealingSequence());
            }
        }

        private void TickFarmingTutorial()
        {
            InitializeFarmingTutorialTracking();

            int tilledCount = GetTilledCount();
            int plantedCount = GetPlantedCount();
            int wateredCount = GetWateredCount();
            int currentWoodCount = GetCurrentWoodCount();

            if (!_tillObjectiveCompleted && tilledCount >= requiredTilledCount)
            {
                _tillObjectiveCompleted = true;
            }

            if (!_plantObjectiveCompleted && plantedCount >= requiredPlantedCount)
            {
                _plantObjectiveCompleted = true;
            }

            if (!_waterObjectiveCompleted && wateredCount >= requiredWateredCount)
            {
                _waterObjectiveCompleted = true;
            }

            if (_waterObjectiveCompleted && !_woodTrackingArmed)
            {
                ArmWoodTracking();
            }

            if (!_woodObjectiveCompleted && _woodTrackingArmed && _collectedWoodSinceWoodStepStart >= requiredWoodCollectedCount)
            {
                _woodObjectiveCompleted = true;
            }

            if (!_staminaRevealed && _tillObjectiveCompleted)
            {
                _staminaRevealed = true;
                EnergySystem.Instance.SetEnergyState(initialEnergy, maxEnergy);
                EnergySystem.Instance.PlayRevealAndAnimateTo(initialEnergy, initialEnergy, maxEnergy, energyRevealDuration, 0f);
            }

            if (!_tillObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show("先用锄头开垦一格土地。");
                return;
            }

            if (!_plantObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show("把花椰菜种子种进刚开垦的土地。");
                return;
            }

            if (!_waterObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show("用浇水壶给作物浇水。");
                return;
            }

            if (!_woodObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show($"再去收集一些木材，当前还差 {Mathf.Max(0, requiredWoodCollectedCount - GetCollectedWoodProgress())} 份。");
                return;
            }

            if (_craftedCount < requiredCraftedCount)
            {
                SpringDay1PromptOverlay.Instance.Show("回到工作台附近，按 E 完成一次基础制作。");
                return;
            }

            SpringDay1PromptOverlay.Instance.Hide();
            StoryManager.Instance.SetPhase(StoryPhase.DinnerConflict);
        }

        public string TryHandleWorkbenchTestInteraction(CraftingStation station)
        {
            if (station != CraftingStation.Workbench)
            {
                return string.Empty;
            }

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            if (currentPhase == StoryPhase.WorkbenchFlashback)
            {
                return "已触发工作台回忆。";
            }

            if (currentPhase != StoryPhase.FarmingTutorial)
            {
                return "工作台已接通，当前阶段暂不需要额外制作操作。";
            }

            InitializeFarmingTutorialTracking();

            if (!_tillObjectiveCompleted)
            {
                return "先完成开垦，再回工作台。";
            }

            if (!_plantObjectiveCompleted)
            {
                return "先把种子种下，再回工作台。";
            }

            if (!_waterObjectiveCompleted)
            {
                return "先完成一次浇水，再回工作台。";
            }

            if (!_woodObjectiveCompleted)
            {
                return $"先收集足够木材，当前还差 {Mathf.Max(0, requiredWoodCollectedCount - GetCollectedWoodProgress())} 份。";
            }

            if (_craftedCount >= requiredCraftedCount)
            {
                return "基础制作已经完成。";
            }

            _craftedCount = requiredCraftedCount;
            return "测试用工作台交互：已记作一次基础制作。";
        }

        public bool CanPerformWorkbenchCraft(out string blockerMessage)
        {
            blockerMessage = string.Empty;

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            if (currentPhase == StoryPhase.WorkbenchFlashback)
            {
                blockerMessage = "先完成这段工作台回忆。";
                return false;
            }

            if (currentPhase != StoryPhase.FarmingTutorial)
            {
                return true;
            }

            InitializeFarmingTutorialTracking();

            if (!_tillObjectiveCompleted)
            {
                blockerMessage = "先完成开垦，再回来制作。";
                return false;
            }

            if (!_plantObjectiveCompleted)
            {
                blockerMessage = "先把种子种下，再回来制作。";
                return false;
            }

            if (!_waterObjectiveCompleted)
            {
                blockerMessage = "先完成一次浇水，再回来制作。";
                return false;
            }

            if (!_woodObjectiveCompleted)
            {
                blockerMessage = $"先收集足够木材，当前还差 {Mathf.Max(0, requiredWoodCollectedCount - GetCollectedWoodProgress())} 份。";
                return false;
            }

            return true;
        }

        public bool ShouldShowWorkbenchEntryHint()
        {
            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            return currentPhase == StoryPhase.WorkbenchFlashback && !_workbenchOpened;
        }

        private void BeginReturnReminder()
        {
            StoryManager.Instance.SetPhase(StoryPhase.ReturnAndReminder);
            EnergySystem.Instance.PlayRestoreAnimation(dinnerRestoreEnergy, dinnerRestoreDuration);
            SyncStoryTimePauseState();

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
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Show(FreeTimePromptText);
        }

        public bool TryTriggerSleepFromBed()
        {
            if (_dayEnded || StoryManager.Instance.CurrentPhase != StoryPhase.FreeTime)
            {
                return false;
            }

            TimeManager.Instance.Sleep();
            return true;
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

        private void InitializeFarmingTutorialTracking(bool forceReset = false)
        {
            if (_farmingTutorialTrackingInitialized && !forceReset)
            {
                return;
            }

            _farmingTutorialTrackingInitialized = true;
            _tillObjectiveCompleted = false;
            _plantObjectiveCompleted = false;
            _waterObjectiveCompleted = false;
            _woodObjectiveCompleted = false;
            _baselineWoodCount = GetCurrentWoodCount();
            _woodTrackingArmed = false;
            _trackedWoodCountSnapshot = -1;
            _collectedWoodSinceWoodStepStart = 0;
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

        private int GetCurrentWoodCount()
        {
            InventoryService inventoryService = FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
            if (inventoryService == null)
            {
                return 0;
            }

            int total = 0;
            for (int slotIndex = 0; slotIndex < inventoryService.Size; slotIndex++)
            {
                ItemStack stack = inventoryService.GetSlot(slotIndex);
                if (!stack.IsEmpty && stack.itemId == WoodItemId)
                {
                    total += stack.amount;
                }
            }

            return total;
        }

        private int GetCollectedWoodProgress()
        {
            if (!_woodTrackingArmed)
            {
                return _woodObjectiveCompleted ? requiredWoodCollectedCount : 0;
            }

            return _woodObjectiveCompleted
                ? requiredWoodCollectedCount
                : Mathf.Clamp(_collectedWoodSinceWoodStepStart, 0, requiredWoodCollectedCount);
        }

        private void ArmWoodTracking()
        {
            _woodTrackingArmed = true;
            _trackedWoodCountSnapshot = GetCurrentWoodCount();
            _collectedWoodSinceWoodStepStart = 0;
        }

        private static int GetLatchedProgress(bool completed, int requiredCount)
        {
            if (requiredCount <= 0)
            {
                return 0;
            }

            return completed ? requiredCount : 0;
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

        private static Transform FindWorkbenchCandidate()
        {
            for (int index = 0; index < PreferredWorkbenchObjectNames.Length; index++)
            {
                GameObject exactMatch = GameObject.Find(PreferredWorkbenchObjectNames[index]);
                if (exactMatch != null)
                {
                    return exactMatch.transform;
                }
            }

            Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int index = 0; index < allTransforms.Length; index++)
            {
                Transform candidate = allTransforms[index];
                if (candidate == null)
                {
                    continue;
                }

                string loweredName = candidate.name.ToLowerInvariant();
                if (loweredName.Contains("anvil") || loweredName.Contains("workbench"))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static RestTargetBinding FindRestTargetCandidate()
        {
            for (int index = 0; index < PreferredBedObjectNames.Length; index++)
            {
                GameObject exactMatch = GameObject.Find(PreferredBedObjectNames[index]);
                if (exactMatch != null)
                {
                    return RestTargetBinding.ForBed(exactMatch.transform);
                }
            }

            for (int index = 0; index < PreferredRestProxyObjectNames.Length; index++)
            {
                GameObject exactMatch = GameObject.Find(PreferredRestProxyObjectNames[index]);
                if (exactMatch != null && IsRestProxyCandidate(exactMatch.transform))
                {
                    return RestTargetBinding.ForRestProxy(exactMatch.transform);
                }
            }

            Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int index = 0; index < allTransforms.Length; index++)
            {
                Transform candidate = allTransforms[index];
                if (candidate == null)
                {
                    continue;
                }

                string loweredName = candidate.name.ToLowerInvariant();
                if (loweredName.Contains("bed"))
                {
                    return RestTargetBinding.ForBed(candidate);
                }

                if (IsRestProxyCandidate(candidate))
                {
                    return RestTargetBinding.ForRestProxy(candidate);
                }
            }

            return default;
        }

        private static bool IsRestProxyCandidate(Transform candidate)
        {
            if (candidate == null || candidate.GetComponent<SpriteRenderer>() == null)
            {
                return false;
            }

            string loweredName = candidate.name.ToLowerInvariant();
            bool nameMatched = false;
            for (int index = 0; index < PreferredRestProxyKeywords.Length; index++)
            {
                if (loweredName.Contains(PreferredRestProxyKeywords[index]))
                {
                    nameMatched = true;
                    break;
                }
            }

            bool parentSuggestsRest = candidate.parent != null && candidate.parent.name.ToLowerInvariant().Contains("house");
            bool tagMatched = candidate.CompareTag("Interactable") || candidate.CompareTag("Building");
            return tagMatched && (nameMatched || parentSuggestsRest);
        }

        private static Collider2D EnsureRestInteractableCollider(RestTargetBinding binding)
        {
            Collider2D existingCollider = binding.Transform.GetComponent<Collider2D>();
            if (existingCollider != null)
            {
                return existingCollider;
            }

            SpriteRenderer spriteRenderer = binding.Transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                return null;
            }

            BoxCollider2D runtimeCollider = binding.Transform.gameObject.AddComponent<BoxCollider2D>();
            runtimeCollider.isTrigger = true;
            runtimeCollider.offset = spriteRenderer.sprite.bounds.center;
            runtimeCollider.size = spriteRenderer.sprite.bounds.size;
            return runtimeCollider;
        }

        private readonly struct RestTargetBinding
        {
            public Transform Transform { get; }
            public string InteractionHint { get; }
            public float InteractionDistance { get; }
            public int InteractionPriority { get; }

            private RestTargetBinding(Transform transform, string interactionHint, float interactionDistance, int interactionPriority)
            {
                Transform = transform;
                InteractionHint = interactionHint;
                InteractionDistance = interactionDistance;
                InteractionPriority = interactionPriority;
            }

            public static RestTargetBinding ForBed(Transform transform)
            {
                return new RestTargetBinding(transform, BedInteractionHint, 1.6f, 24);
            }

            public static RestTargetBinding ForRestProxy(Transform transform)
            {
                return new RestTargetBinding(transform, RestProxyInteractionHint, 1.8f, 24);
            }
        }

        private void ApplyLowEnergyMovementPenalty(bool shouldSlow)
        {
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            if (_playerMovement == null)
            {
                return;
            }

            if (shouldSlow)
            {
                _playerMovement.SetRuntimeSpeedMultiplier(lowEnergyMoveSpeedMultiplier);
                return;
            }

            _playerMovement.ResetRuntimeSpeedMultiplier();
        }

        private void SyncStoryTimePauseState()
        {
            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return;
            }

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            bool shouldPause = currentPhase != StoryPhase.None
                && currentPhase != StoryPhase.FreeTime
                && currentPhase != StoryPhase.DayEnd;

            if (shouldPause)
            {
                timeManager.PauseTime(StoryTimePauseSource);
                _storyTimePauseApplied = true;
                return;
            }

            ReleaseStoryTimePause();
        }

        private void ReleaseStoryTimePause()
        {
            if (!_storyTimePauseApplied)
            {
                return;
            }

            TimeManager.Instance.ResumeTime(StoryTimePauseSource);
            _storyTimePauseApplied = false;
        }
    }

    /// <summary>
    /// spring-day1 的轻量运行态验收入口。
    /// 不修改场景承载，只负责汇总状态与触发“下一步最小验证动作”。
    /// </summary>
    [DisallowMultipleComponent]
    public class SpringDay1LiveValidationRunner : MonoBehaviour
    {
        private static readonly string[] PreferredNpcObjectNames = { "NPC001", "001", "DialogueTestNPC" };
        private static readonly string[] PreferredWorkbenchObjectNames = { "Anvil_0", "Workbench", "Anvil" };
        private static readonly string[] PreferredRestObjectNames = { "Bed", "PlayerBed", "HomeBed", "House 1_2", "HomeDoor", "HouseDoor" };

        private static SpringDay1LiveValidationRunner _instance;

        private readonly StringBuilder _builder = new StringBuilder(512);

        public static SpringDay1LiveValidationRunner Instance => EnsureRuntime();

        public string LastSnapshot { get; private set; } = string.Empty;
        public string LastActionResult { get; private set; } = string.Empty;

        public static SpringDay1LiveValidationRunner EnsureRuntime()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = FindFirstObjectByType<SpringDay1LiveValidationRunner>(FindObjectsInactive.Include);
            if (_instance != null)
            {
                return _instance;
            }

            GameObject runtimeObject = new GameObject(nameof(SpringDay1LiveValidationRunner));
            _instance = runtimeObject.AddComponent<SpringDay1LiveValidationRunner>();
            return _instance;
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
        }

        public string BootstrapRuntime()
        {
            _ = StoryManager.Instance;
            SpringDay1Director.EnsureRuntime();
            SpringDay1PromptOverlay.EnsureRuntime();
            _ = HealthSystem.Instance;
            _ = EnergySystem.Instance;
            _ = TimeManager.Instance;

            return SetActionResult("已确保 StoryManager / Day1Director / PromptOverlay / HP / EP / Time 运行时对象就位。");
        }

        public string BuildSnapshot(string label = null)
        {
            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            DialogueUI dialogueUi = FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
            SpringDay1PromptOverlay promptOverlay = FindFirstObjectByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include);
            HealthSystem healthSystem = FindFirstObjectByType<HealthSystem>(FindObjectsInactive.Include);
            EnergySystem energySystem = FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
            TimeManager timeManager = FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
            GameInputManager inputManager = FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
            NPCDialogueInteractable npcInteractable = FindPreferredComponent<NPCDialogueInteractable>(PreferredNpcObjectNames);
            NPCAutoRoamController npcRoam = npcInteractable != null ? npcInteractable.GetComponent<NPCAutoRoamController>() : null;
            CraftingStationInteractable workbenchInteractable = FindPreferredComponent<CraftingStationInteractable>(PreferredWorkbenchObjectNames);
            SpringDay1BedInteractable restInteractable = FindPreferredComponent<SpringDay1BedInteractable>(PreferredRestObjectNames);

            _builder.Clear();
            AppendPair("Label", string.IsNullOrWhiteSpace(label) ? "manual" : label);
            AppendPair("Scene", SceneManager.GetActiveScene().name);
            AppendPair("Phase", storyManager != null ? storyManager.CurrentPhase.ToString() : "n/a");
            AppendPair("Decoded", storyManager != null ? storyManager.IsLanguageDecoded.ToString() : "n/a");
            AppendPair("Dialogue", BuildDialogueSummary(dialogueManager));
            AppendPair("DialogueUI", BuildDialogueUiSummary(dialogueUi));
            AppendPair("Prompt", BuildPromptSummary(promptOverlay));
            AppendPair("HP", healthSystem != null ? $"{healthSystem.CurrentHealth}/{healthSystem.MaxHealth}|visible={healthSystem.IsVisible}" : "n/a");
            AppendPair("EP", energySystem != null ? $"{energySystem.CurrentEnergy}/{energySystem.MaxEnergy}|visible={energySystem.IsVisible}" : "n/a");
            AppendPair("Time", timeManager != null ? $"paused={timeManager.IsTimePaused()}|depth={timeManager.GetPauseStackDepth()}|clock={timeManager.GetFormattedTime()}" : "n/a");
            AppendPair("Input", inputManager != null ? inputManager.IsInputEnabledForDebug.ToString() : "n/a");
            AppendPair("Director", director != null ? $"{director.GetCurrentTaskLabel()}|{director.GetCurrentProgressLabel()}" : "n/a");
            AppendPair("NPC", npcInteractable != null ? $"{npcInteractable.name}|roam={(npcRoam != null ? npcRoam.IsRoaming.ToString() : "n/a")}|state={(npcRoam != null ? npcRoam.DebugState : "n/a")}" : "n/a");
            AppendPair("Workbench", workbenchInteractable != null ? workbenchInteractable.name : "n/a");
            AppendPair("Rest", restInteractable != null ? restInteractable.name : "n/a");
            AppendPair("Next", GetRecommendedNextAction());

            LastSnapshot = _builder.ToString();
            return LastSnapshot;
        }

        public void LogSnapshot(string label = null)
        {
            Debug.Log($"[SpringDay1LiveValidation] {BuildSnapshot(label)}");
        }

        public string GetRecommendedNextAction()
        {
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return "当前有对话进行中，继续推进或强制补完当前句。";
            }

            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            if (storyManager == null)
            {
                return "缺少 StoryManager，先执行 Bootstrap。";
            }

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => "触发 NPC001 首段对话。",
                StoryPhase.EnterVillage => "等待疗伤段启动，若未启动可再次触发 NPC001。",
                StoryPhase.HealingAndHP => "等待血条渐显与疗伤对话自动播出。",
                StoryPhase.WorkbenchFlashback => "交互 Anvil_0 / Workbench，触发工作台闪回。",
                StoryPhase.FarmingTutorial => "人工完成开垦、播种、浇水、砍树与一次制作。",
                StoryPhase.DinnerConflict => "等待晚餐对话自动排队；若已播出则继续推进。",
                StoryPhase.ReturnAndReminder => "推进归途提醒对话，准备进入自由时段。",
                StoryPhase.FreeTime => "回住处休息，验证 DayEnd。",
                StoryPhase.DayEnd => "春1日已结束，可开始下一轮复测。",
                _ => "当前阶段暂无推荐动作。"
            };
        }

        public string TriggerRecommendedAction()
        {
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                dialogueManager.ForceCompleteOrAdvance();
                return SetActionResult("已推进当前对话一拍。");
            }

            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            if (storyManager == null)
            {
                return SetActionResult("未找到 StoryManager；请先执行 Bootstrap。");
            }

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => SetActionResult(TryTriggerNpcDialogue()),
                StoryPhase.EnterVillage => SetActionResult(TryTriggerNpcDialogue()),
                StoryPhase.HealingAndHP => SetActionResult("疗伤阶段主要由导演自动推进；当前无需额外手动触发。"),
                StoryPhase.WorkbenchFlashback => SetActionResult(TryTriggerWorkbenchInteraction()),
                StoryPhase.FarmingTutorial => SetActionResult("农田教学阶段需要人工完成真实操作，当前不做脚本代跑。"),
                StoryPhase.DinnerConflict => SetActionResult("晚餐阶段等待导演自动排队对话；若对话已开始，再次执行 Step 即可推进。"),
                StoryPhase.ReturnAndReminder => SetActionResult("归途提醒阶段等待对话播出；若对话已开始，再次执行 Step 即可推进。"),
                StoryPhase.FreeTime => SetActionResult(TryTriggerRestInteraction()),
                StoryPhase.DayEnd => SetActionResult("春1日已结束，无需继续推进。"),
                _ => SetActionResult("当前阶段暂无可脚本触发的推荐动作。")
            };
        }

        private string TryTriggerNpcDialogue()
        {
            NPCDialogueInteractable interactable = FindPreferredComponent<NPCDialogueInteractable>(PreferredNpcObjectNames);
            if (interactable == null)
            {
                return "未找到 NPCDialogueInteractable（优先查找 NPC001/001/DialogueTestNPC）。";
            }

            InteractionContext context = BuildInteractionContext();
            if (!interactable.CanInteract(context))
            {
                return $"NPC {interactable.name} 当前不可交互。";
            }

            interactable.OnInteract(context);
            return $"已触发 NPC 对话：{interactable.name}";
        }

        private string TryTriggerWorkbenchInteraction()
        {
            CraftingStationInteractable interactable = FindPreferredComponent<CraftingStationInteractable>(PreferredWorkbenchObjectNames);
            if (interactable == null)
            {
                return "未找到工作台交互脚本（优先查找 Anvil_0 / Workbench / Anvil）。";
            }

            InteractionContext context = BuildInteractionContext();
            if (!interactable.CanInteract(context))
            {
                return $"工作台 {interactable.name} 当前不可交互。";
            }

            interactable.OnInteract(context);
            return $"已触发工作台交互：{interactable.name}";
        }

        private string TryTriggerRestInteraction()
        {
            SpringDay1BedInteractable interactable = FindPreferredComponent<SpringDay1BedInteractable>(PreferredRestObjectNames);
            if (interactable == null)
            {
                return "未找到住处/床位交互脚本；请先确认 Day1 导演已自动补挂。";
            }

            InteractionContext context = BuildInteractionContext();
            if (!interactable.CanInteract(context))
            {
                return $"休息承载物 {interactable.name} 当前不可交互。";
            }

            interactable.OnInteract(context);
            return $"已触发休息交互：{interactable.name}";
        }

        private static InteractionContext BuildInteractionContext()
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            Transform playerTransform = playerMovement != null ? playerMovement.transform : null;

            return new InteractionContext
            {
                PlayerTransform = playerTransform,
                PlayerPosition = playerTransform != null ? (Vector2)playerTransform.position : Vector2.zero
            };
        }

        private string BuildDialogueSummary(DialogueManager dialogueManager)
        {
            if (dialogueManager == null)
            {
                return "n/a";
            }

            if (!dialogueManager.IsDialogueActive)
            {
                return "idle";
            }

            int displayIndex = Mathf.Max(1, dialogueManager.CurrentNodeIndex + 1);
            int totalCount = Mathf.Max(1, dialogueManager.CurrentNodeCount);
            return $"{Sanitize(dialogueManager.CurrentSequenceId)}[{displayIndex}/{totalCount}]|typing={dialogueManager.IsNodeTyping}";
        }

        private string BuildDialogueUiSummary(DialogueUI dialogueUi)
        {
            if (dialogueUi == null)
            {
                return "n/a";
            }

            return $"alpha={dialogueUi.CurrentCanvasAlpha:F2}|speaker={Sanitize(dialogueUi.CurrentSpeakerName)}|status={Sanitize(dialogueUi.CurrentTestStatus)}";
        }

        private string BuildPromptSummary(SpringDay1PromptOverlay promptOverlay)
        {
            if (promptOverlay == null)
            {
                return "n/a";
            }

            CanvasGroup group = promptOverlay.GetComponent<CanvasGroup>();
            TextMeshProUGUI text = promptOverlay.GetComponentInChildren<TextMeshProUGUI>(true);
            float alpha = group != null ? group.alpha : -1f;
            string prompt = text != null ? Sanitize(text.text) : string.Empty;
            return $"alpha={alpha:F2}|text={prompt}";
        }

        private void AppendPair(string key, string value)
        {
            if (_builder.Length > 0)
            {
                _builder.Append(", ");
            }

            _builder.Append(key);
            _builder.Append('=');
            _builder.Append(Sanitize(value));
        }

        private string SetActionResult(string result)
        {
            LastActionResult = result;
            return LastActionResult;
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "n/a";
            }

            return value.Replace('\n', ' ').Replace('\r', ' ').Trim();
        }

        private static T FindPreferredComponent<T>(string[] preferredObjectNames) where T : Component
        {
            if (preferredObjectNames != null)
            {
                for (int index = 0; index < preferredObjectNames.Length; index++)
                {
                    GameObject exactMatch = GameObject.Find(preferredObjectNames[index]);
                    if (exactMatch == null)
                    {
                        continue;
                    }

                    T directComponent = exactMatch.GetComponent<T>();
                    if (directComponent != null)
                    {
                        return directComponent;
                    }

                    T nestedComponent = exactMatch.GetComponentInChildren<T>(true);
                    if (nestedComponent != null)
                    {
                        return nestedComponent;
                    }
                }
            }

            return FindFirstObjectByType<T>(FindObjectsInactive.Include);
        }
    }
}
