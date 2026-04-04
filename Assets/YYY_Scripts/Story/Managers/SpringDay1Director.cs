using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine.Serialization;
using FarmGame.Data;
using FarmGame.Farm;
using FarmGame.UI;
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
        public readonly struct PromptTaskItem
        {
            public PromptTaskItem(string label, string detail, bool completed)
            {
                Label = label;
                Detail = detail;
                Completed = completed;
            }

            public string Label { get; }
            public string Detail { get; }
            public bool Completed { get; }
        }

        public sealed class PromptCardModel
        {
            public string PhaseKey;
            public string StageLabel;
            public string Subtitle;
            public string FocusText;
            public string FooterText;
            public PromptTaskItem[] Items;
        }

        private const string PrimarySceneName = "Primary";
        private const string StoryTimePauseSource = "SpringDay1Director";
        private const string FirstSequenceId = "spring-day1-first";
        private const string FirstFollowupSequenceId = "spring-day1-first-followup";
        private const string VillageGateSequenceId = "spring-day1-village-gate";
        private const string HouseArrivalSequenceId = "spring-day1-house-arrival";
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
        private const string DinnerBridgePromptText = "天已经晚了，先回去吃点东西。";
        private const string ReturnReminderBridgePromptText = "夜色越来越深了，别在外面逗留太久。";
        private const string FreeTimeNightPromptText = "夜深了，别在外面逗留太久，记得回住处休息。";
        private const string FreeTimeMidnightPromptText = "已经过了午夜，尽快回住处。再拖下去今晚会更难熬。";
        private const string FreeTimeFinalPromptText = "快到凌晨两点了，再不睡就会直接昏睡过去。";
        private const int FreeTimeNightWarningHour = 22;
        private const int FreeTimeMidnightWarningHour = 24;
        private const int FreeTimeFinalWarningHour = 25;

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
        private bool _workbenchCraftingActive;
        private float _workbenchCraftProgress;
        private int _workbenchCraftQueueTotal;
        private int _workbenchCraftQueueCompleted;
        private string _workbenchCraftRecipeName = string.Empty;
        private bool _freeTimeNightWarningShown;
        private bool _freeTimeMidnightWarningShown;
        private bool _freeTimeFinalWarningShown;

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
            TimeManager.OnHourChanged += HandleHourChanged;
            TimeManager.OnSleep += HandleSleep;
            RefreshCraftingServiceSubscription();
            RefreshInventoryTrackingSubscription();
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            EnergySystem.OnEnergyChanged -= HandleEnergyChanged;
            TimeManager.OnHourChanged -= HandleHourChanged;
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
            StoryManager storyManager = StoryManager.Instance;
            string phaseText = storyManager != null ? storyManager.CurrentPhase.ToString() : "n/a";
            return $"Phase={phaseText}, Tilled={GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}, Planted={GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}, Watered={GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}, Wood={GetCollectedWoodProgress()}/{requiredWoodCollectedCount}, Crafted={_craftedCount}, FreeTime={_freeTimeEntered}, DayEnd={_dayEnded}";
        }

        public string GetCurrentTaskLabel()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return "未初始化";
            }

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => "0.0.2 矿洞首遇/撤离",
                StoryPhase.EnterVillage => "0.0.2 进村/安置",
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
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return "等待 StoryManager 初始化";
            }

            StoryPhase phase = storyManager.CurrentPhase;
            if (phase == StoryPhase.CrashAndMeet)
            {
                if (IsDialogueSequenceCurrentlyActive(FirstSequenceId))
                {
                    return "矿洞口醒来与语言错位进行中";
                }

                if (IsDialogueSequenceCurrentlyActive(FirstFollowupSequenceId))
                {
                    return "怪物逼近与撤离进行中";
                }

                if (HasCompletedDialogueSequence(FirstSequenceId))
                {
                    return "已听懂村长的话，等待撤离矿洞口";
                }

                return "等待触发矿洞口首遇";
            }

            if (phase == StoryPhase.EnterVillage)
            {
                if (IsDialogueSequenceCurrentlyActive(VillageGateSequenceId))
                {
                    return "进村围观进行中";
                }

                if (IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId))
                {
                    return "闲置小屋安置进行中";
                }

                if (HasCompletedDialogueSequence(VillageGateSequenceId) && !HasCompletedDialogueSequence(HouseArrivalSequenceId))
                {
                    return "村口围观已过，等待进屋安置";
                }

                return IsFirstFollowupPending()
                    ? "等待进村围观与安置链接管"
                    : "进村安置已收束，等待疗伤段启动";
            }

            if (phase == StoryPhase.HealingAndHP)
            {
                return "等待疗伤对话结束";
            }

            if (phase == StoryPhase.WorkbenchFlashback)
            {
                if (!_workbenchOpened)
                {
                    return "等待打开工作台";
                }

                DialogueManager dialogueManager = DialogueManager.Instance;
                if (dialogueManager != null
                    && dialogueManager.IsDialogueActive
                    && dialogueManager.CurrentSequenceId == WorkbenchSequenceId)
                {
                    return "工作台回忆进行中";
                }

                return "工作台已打开，等待回忆收束";
            }

            if (phase == StoryPhase.FarmingTutorial)
            {
                if (_workbenchCraftingActive)
                {
                    return BuildWorkbenchCraftProgressText();
                }

                return $"教学进度 {GetCompletedTutorialObjectiveCount()}/5";
            }

            if (phase == StoryPhase.DinnerConflict)
            {
                if (IsDialogueSequenceCurrentlyActive(DinnerSequenceId))
                {
                    return "晚餐对白进行中";
                }

                if (_dinnerSequencePlayed)
                {
                    return "等待晚餐对白接管";
                }

                return "晚餐事件进行中";
            }

            if (phase == StoryPhase.ReturnAndReminder)
            {
                if (IsDialogueSequenceCurrentlyActive(ReminderSequenceId))
                {
                    return "归途提醒对白进行中";
                }

                if (_returnSequencePlayed)
                {
                    return "等待归途提醒对白接管";
                }

                return "归途提醒进行中";
            }

            if (phase == StoryPhase.FreeTime)
            {
                return BuildFreeTimeProgressText();
            }

            if (phase == StoryPhase.DayEnd)
            {
                return "春1日已结束";
            }

            return "等待推进";
        }

        public PromptCardModel BuildPromptCardModel()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return new PromptCardModel
                {
                    PhaseKey = "Uninitialized",
                    StageLabel = "未初始化",
                    Subtitle = "等待 Day1 运行时对象就位。",
                    FocusText = "等待 StoryManager 初始化后再刷新正式任务摘要。",
                    FooterText = "暂无正式阶段信息。",
                    Items = System.Array.Empty<PromptTaskItem>()
                };
            }

            StoryPhase phase = storyManager.CurrentPhase;
            RefreshPromptModelTrackingState(phase);
            return new PromptCardModel
            {
                PhaseKey = phase.ToString(),
                StageLabel = GetCurrentTaskLabel(),
                Subtitle = GetPromptSubtitle(phase),
                FocusText = GetPromptFocusText(phase),
                FooterText = GetCurrentProgressLabel(),
                Items = BuildPromptItems(phase)
            };
        }

        private void RefreshPromptModelTrackingState(StoryPhase phase)
        {
            if (phase != StoryPhase.FarmingTutorial)
            {
                return;
            }

            InitializeFarmingTutorialTracking();
            RefreshInventoryTrackingSubscription();
            HandleInventoryChanged();
        }

        private string GetPromptSubtitle(StoryPhase phase)
        {
            return phase switch
            {
                StoryPhase.CrashAndMeet => "先稳住意识，接住陌生语言，再跟村长离开矿洞口。",
                StoryPhase.EnterVillage => "进村后先撑过围观，再被安置到闲置小屋，随后才进入疗伤。",
                StoryPhase.HealingAndHP => "疗伤演出与血量显现会在这里完成。",
                StoryPhase.WorkbenchFlashback => "工作台需要先触发一次正式回忆，再进入教学链。",
                StoryPhase.FarmingTutorial => "今天的教学链会逐条锁定并逐条完成。",
                StoryPhase.DinnerConflict => "晚餐事件是 Day1 的情绪收束段。",
                StoryPhase.ReturnAndReminder => "收完提醒后，才会进入真正的自由时段。",
                StoryPhase.FreeTime => "现在可以自由走动，但最终还要回住处休息。",
                StoryPhase.DayEnd => "Day1 主流程已结束。",
                _ => "等待 Day1 剧情正式开始。"
            };
        }

        private string GetPromptFocusText(StoryPhase phase)
        {
            return phase switch
            {
                StoryPhase.CrashAndMeet when IsDialogueSequenceCurrentlyActive(FirstFollowupSequenceId) => "跟住村长往村里撤，别在矿洞口回头。",
                StoryPhase.CrashAndMeet when HasCompletedDialogueSequence(FirstSequenceId) => "跟住村长，别在矿洞口停留太久。",
                StoryPhase.CrashAndMeet => "靠近村长并按 E，先弄清眼前到底发生了什么。",
                StoryPhase.EnterVillage when IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId) => "先进闲置小屋落脚，等艾拉过来接手疗伤。",
                StoryPhase.EnterVillage when HasCompletedDialogueSequence(VillageGateSequenceId) => "跟着村长进屋，先在闲置小屋安顿下来。",
                StoryPhase.EnterVillage when IsDialogueSequenceCurrentlyActive(VillageGateSequenceId) => "别在围观里停住，跟着村长穿过村口。",
                StoryPhase.EnterVillage => "跟着村长进村，先撑过村口的围观和注视。",
                StoryPhase.HealingAndHP => "等待疗伤对白与 HP 卡片完整播完。",
                StoryPhase.WorkbenchFlashback when _workbenchOpened => "等待工作台回忆完整播完。",
                StoryPhase.WorkbenchFlashback => "靠近 Anvil_0，按 E 打开工作台。",
                StoryPhase.FarmingTutorial when !_tillObjectiveCompleted => "先用锄头开垦一格土地。",
                StoryPhase.FarmingTutorial when !_plantObjectiveCompleted => "把花椰菜种子种进刚开垦的土地。",
                StoryPhase.FarmingTutorial when !_waterObjectiveCompleted => "完成一次浇水，让作物稳稳进入下一步。",
                StoryPhase.FarmingTutorial when !_woodObjectiveCompleted => $"继续收木材，还差 {Mathf.Max(0, requiredWoodCollectedCount - GetCollectedWoodProgress())} 份。",
                StoryPhase.FarmingTutorial when _workbenchCraftingActive => "守在工作台旁，等当前制作完成。",
                StoryPhase.FarmingTutorial when _craftedCount < requiredCraftedCount => "回到工作台，完成一次真正的基础制作。",
                StoryPhase.FarmingTutorial => "农田与制作目标都已完成，等待晚餐事件推进。",
                StoryPhase.DinnerConflict => "看完晚餐事件，准备进入归途提醒。",
                StoryPhase.ReturnAndReminder => "听完提醒，接住今天最后一段过渡。",
                StoryPhase.FreeTime => GetFreeTimeFocusText(),
                StoryPhase.DayEnd => "春1日结束，可以进入下一阶段开发或验收。",
                _ => "等待推进。"
            };
        }

        private PromptTaskItem[] BuildPromptItems(StoryPhase phase)
        {
            if (phase == StoryPhase.CrashAndMeet)
            {
                return new[]
                {
                    new PromptTaskItem(
                        "先听懂村长的话",
                        HasCompletedDialogueSequence(FirstSequenceId)
                            ? "语言已经接上，先别在矿洞口继续停留。"
                            : "在矿洞口醒来后，先弄清眼前这个人想做什么。",
                        HasCompletedDialogueSequence(FirstSequenceId)),
                    new PromptTaskItem(
                        "跟村长离开矿洞口",
                        HasCompletedDialogueSequence(FirstFollowupSequenceId)
                            ? "已完成，正在进入村庄。"
                            : "怪物正在逼近，先跟着他离开危险区域。",
                        HasCompletedDialogueSequence(FirstFollowupSequenceId))
                };
            }

            if (phase == StoryPhase.EnterVillage)
            {
                return new[]
                {
                    new PromptTaskItem(
                        "跟着村长进入村子",
                        HasCompletedDialogueSequence(VillageGateSequenceId)
                            ? "已撑过村口围观。"
                            : "先让村长带路，别在陌生人的注视里乱跑。",
                        HasCompletedDialogueSequence(VillageGateSequenceId)),
                    new PromptTaskItem(
                        "在闲置小屋安顿下来",
                        HasCompletedDialogueSequence(HouseArrivalSequenceId)
                            ? "已完成，等待艾拉接手疗伤。"
                            : "村长会先把你安置进一间空置很久的旧屋。",
                        HasCompletedDialogueSequence(HouseArrivalSequenceId))
                };
            }

            if (phase == StoryPhase.HealingAndHP)
            {
                bool hpVisible = HealthSystem.Instance != null && HealthSystem.Instance.IsVisible;
                return new[]
                {
                    !_healingStarted
                        ? new PromptTaskItem("进入疗伤流程", "等待进村安置链收束后触发。", false)
                        : !hpVisible
                            ? new PromptTaskItem("显示 HP 卡片", "先让血量条缓慢显现出来。", false)
                            : new PromptTaskItem("播完疗伤对白", _healingSequencePlayed ? "疗伤对白已完整结束。" : "等待疗伤对白真正播完。", _healingSequencePlayed)
                };
            }

            if (phase == StoryPhase.WorkbenchFlashback)
            {
                return BuildWorkbenchFlashbackPromptItems();
            }

            if (phase == StoryPhase.FarmingTutorial)
            {
                return BuildFarmingTutorialPromptItems();
            }

            if (phase == StoryPhase.DinnerConflict)
            {
                return new[]
                {
                    new PromptTaskItem(
                        "观看晚餐事件",
                        IsDialogueSequenceCurrentlyActive(DinnerSequenceId)
                            ? "晚餐对白进行中。"
                            : _dinnerSequencePlayed
                                ? "晚餐对白已排队，等待接管。"
                                : "等待晚餐剧情开始。",
                        false)
                };
            }

            if (phase == StoryPhase.ReturnAndReminder)
            {
                return new[]
                {
                    new PromptTaskItem(
                        "接住归途提醒",
                        IsDialogueSequenceCurrentlyActive(ReminderSequenceId)
                            ? "归途提醒对白进行中。"
                            : _returnSequencePlayed
                                ? "归途提醒已排队，等待接管。"
                                : "等待提醒对白开始。",
                        false)
                };
            }

            if (phase == StoryPhase.FreeTime)
            {
                return new[]
                {
                    new PromptTaskItem(
                        "回住处睡觉",
                        _dayEnded
                            ? "已完成。"
                            : IsSleepInteractionAvailable()
                                ? BuildFreeTimeTaskDetail()
                                : "等待自由时段正式开放。",
                        _dayEnded)
                };
            }

            if (phase == StoryPhase.DayEnd)
            {
                return new[]
                {
                    new PromptTaskItem("完成 Day1 收尾", "今天的剧情与教学链已经全部跑完。", true)
                };
            }

            return new[]
            {
                new PromptTaskItem("等待剧情推进", "当前还没有激活新的 Day1 任务。", false)
            };
        }

        private PromptTaskItem[] BuildWorkbenchFlashbackPromptItems()
        {
            return new[]
            {
                new PromptTaskItem(
                    "靠近工作台并按 E",
                    _workbenchOpened ? "工作台已经打开。" : "从交互包络线进入后按 E 打开。",
                    _workbenchOpened),
                new PromptTaskItem(
                    "看完工作台回忆",
                    _workbenchSequencePlayed ? "工作台回忆已播完。" : "打开后会自动推进这段对白。",
                    _workbenchSequencePlayed)
            };
        }

        private PromptTaskItem[] BuildFarmingTutorialPromptItems()
        {
            InitializeFarmingTutorialTracking();

            string craftDetail = _workbenchCraftingActive
                ? BuildWorkbenchCraftProgressText()
                : $"{Mathf.Min(_craftedCount, requiredCraftedCount)}/{requiredCraftedCount}";

            return new[]
            {
                new PromptTaskItem(
                    "开垦土地",
                    _tillObjectiveCompleted
                        ? $"已完成 {requiredTilledCount}/{requiredTilledCount}"
                        : $"先开垦出第一格教学土地 {GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}/{requiredTilledCount}",
                    _tillObjectiveCompleted),
                new PromptTaskItem(
                    "完成播种",
                    _plantObjectiveCompleted
                        ? $"已完成 {requiredPlantedCount}/{requiredPlantedCount}"
                        : !_tillObjectiveCompleted
                            ? "先完成开垦，再把花椰菜种子种下。"
                            : $"{GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}/{requiredPlantedCount}",
                    _plantObjectiveCompleted),
                new PromptTaskItem(
                    "完成浇水",
                    _waterObjectiveCompleted
                        ? $"已完成 {requiredWateredCount}/{requiredWateredCount}"
                        : !_plantObjectiveCompleted
                            ? "播种后再用浇水壶推进下一步。"
                            : $"{GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}/{requiredWateredCount}",
                    _waterObjectiveCompleted),
                new PromptTaskItem(
                    "收集木材",
                    _woodObjectiveCompleted
                        ? $"已收齐 {requiredWoodCollectedCount}/{requiredWoodCollectedCount}"
                        : !_waterObjectiveCompleted
                            ? "先完成浇水，再开始收集木材。"
                            : $"{GetCollectedWoodProgress()}/{requiredWoodCollectedCount}",
                    _woodObjectiveCompleted),
                new PromptTaskItem(
                    "完成基础制作",
                    _craftedCount >= requiredCraftedCount
                        ? "基础制作已完成。"
                        : !_woodObjectiveCompleted
                            ? "先把木材收齐，再回工作台制作。"
                            : craftDetail,
                    _craftedCount >= requiredCraftedCount)
            };
        }

        private void HandleStoryPhaseChanged(StoryPhaseChangedEvent evt)
        {
            if (!IsPrimarySceneActive())
            {
                return;
            }

            SyncStoryTimePauseState();
            ResyncLowEnergyState(false);

            if (showDebugLog)
            {
                Debug.Log($"[SpringDay1Director] Phase: {evt.PreviousPhase} -> {evt.CurrentPhase}");
            }

            if (evt.CurrentPhase == StoryPhase.EnterVillage)
            {
                if (!IsDialogueChainStillActive() && !IsFirstFollowupPending())
                {
                    BeginHealingAndHp();
                }

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
                if (HasPlayableNodes(evt.FollowupSequence))
                {
                    return;
                }

                StoryManager.Instance.SetPhase(StoryPhase.EnterVillage);
                BeginHealingAndHp();
                return;
            }

            if (evt.SequenceId == FirstFollowupSequenceId || evt.SequenceId == VillageGateSequenceId)
            {
                if (HasPlayableNodes(evt.FollowupSequence))
                {
                    return;
                }

                BeginHealingAndHp();
                return;
            }

            if (evt.SequenceId == HouseArrivalSequenceId)
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
                HandleInventoryChanged();
            }
        }

        private void HandleInventoryChanged()
        {
            if (!_farmingTutorialTrackingInitialized || _woodObjectiveCompleted)
            {
                return;
            }

            int currentWoodCount = GetCurrentWoodCount();
            if (currentWoodCount >= requiredWoodCollectedCount)
            {
                _baselineWoodCount = Mathf.Max(_baselineWoodCount, currentWoodCount);
                _trackedWoodCountSnapshot = currentWoodCount;
                _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
                _woodTrackingArmed = true;
                _woodObjectiveCompleted = true;
                return;
            }

            if (_baselineWoodCount < 0)
            {
                _baselineWoodCount = currentWoodCount;
            }

            if (_trackedWoodCountSnapshot < 0)
            {
                _trackedWoodCountSnapshot = currentWoodCount;
            }

            int delta = currentWoodCount - _trackedWoodCountSnapshot;
            if (delta > 0)
            {
                _collectedWoodSinceWoodStepStart += delta;
                _collectedWoodSinceWoodStepStart = Mathf.Min(_collectedWoodSinceWoodStepStart, requiredWoodCollectedCount);
            }

            _trackedWoodCountSnapshot = currentWoodCount;
            if (_woodTrackingArmed && _collectedWoodSinceWoodStepStart >= requiredWoodCollectedCount)
            {
                _woodObjectiveCompleted = true;
                _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
            }
        }

        private void HandleEnergyChanged(int current, int max)
        {
            ResyncLowEnergyState(true);
        }

        private void HandleHourChanged(int hour)
        {
            if (_dayEnded || StoryManager.Instance == null || StoryManager.Instance.CurrentPhase != StoryPhase.FreeTime)
            {
                return;
            }

            ShowFreeTimePressurePromptForHour(hour);
        }

        private void HandleSleep()
        {
            if (_freeTimeEntered && !_dayEnded)
            {
                _dayEnded = true;
                StoryManager.Instance.SetPhase(StoryPhase.DayEnd);
                EnergySystem.Instance.FullRestore();
                EnergySystem.Instance.SetLowEnergyWarningVisual(false);
                ResyncLowEnergyState(false);
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

        private static bool IsDialogueChainStillActive()
        {
            DialogueManager manager = DialogueManager.Instance;
            return manager != null && manager.IsDialogueActive;
        }

        private static bool HasPlayableNodes(DialogueSequenceSO sequence)
        {
            return sequence != null && sequence.nodes != null && sequence.nodes.Count > 0;
        }

        private static bool IsDialogueSequenceCurrentlyActive(string sequenceId)
        {
            DialogueManager manager = DialogueManager.Instance;
            return manager != null
                && manager.IsDialogueActive
                && manager.CurrentSequenceId == sequenceId;
        }

        private static bool HasCompletedDialogueSequence(string sequenceId)
        {
            DialogueManager manager = DialogueManager.Instance;
            return manager != null && manager.HasCompletedSequence(sequenceId);
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
            SpringDay1PromptOverlay.Instance.Show(DinnerBridgePromptText);
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

            return "工作台界面当前未接通，本次不会记作基础制作。等工作台真正打开后再完成这一步。";
        }

        public string GetValidationFarmingNextAction()
        {
            InitializeFarmingTutorialTracking();

            if (!_tillObjectiveCompleted)
            {
                return "执行 Step，模拟第一格开垦并验证 EP 首次出现。";
            }

            if (!_plantObjectiveCompleted)
            {
                return "执行 Step，模拟完成播种并继续压测 EP 消耗。";
            }

            if (!_waterObjectiveCompleted)
            {
                return "执行 Step，模拟完成浇水并压到 low-energy warning 阈值。";
            }

            if (!_woodObjectiveCompleted)
            {
                return $"执行 Step，模拟收齐 {requiredWoodCollectedCount} 份木材。";
            }

            if (_craftedCount < requiredCraftedCount)
            {
                return "执行 Step，模拟一次基础制作并推进到晚餐。";
            }

            return "农田教学目标已齐，等待晚餐事件接管。";
        }

        public string TryAdvanceFarmingTutorialValidationStep()
        {
            InitializeFarmingTutorialTracking();

            if (!_tillObjectiveCompleted)
            {
                _tillObjectiveCompleted = true;
                TickFarmingTutorial();
                return "验收入口：已模拟完成第一格开垦，EP 正式出现。";
            }

            if (!_plantObjectiveCompleted)
            {
                _plantObjectiveCompleted = true;
                ApplyValidationEnergyState(Mathf.Max(lowEnergyWarningThreshold + 25, lowEnergyWarningThreshold + 1));
                TickFarmingTutorial();
                return "验收入口：已模拟完成播种，并压入一段正式 EP 消耗。";
            }

            if (!_waterObjectiveCompleted)
            {
                _waterObjectiveCompleted = true;
                ApplyValidationEnergyState(lowEnergyWarningThreshold);
                TickFarmingTutorial();
                return "验收入口：已模拟完成浇水，低精力 warning 与减速应已生效。";
            }

            if (!_woodObjectiveCompleted)
            {
                _woodObjectiveCompleted = true;
                _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
                TickFarmingTutorial();
                return $"验收入口：已模拟收齐 {requiredWoodCollectedCount} 份木材。";
            }

            if (_craftedCount < requiredCraftedCount)
            {
                _craftedCount = requiredCraftedCount;
                TickFarmingTutorial();
                return "验收入口：已模拟完成一次基础制作，等待晚餐事件接管。";
            }

            TickFarmingTutorial();
            return "农田教学验证目标已全部完成；继续执行 Step 推进晚餐链。";
        }

        public string GetValidationFreeTimeNextAction()
        {
            if (!IsSleepInteractionAvailable())
            {
                return "自由时段尚未开放睡觉收束。";
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return "缺少 TimeManager，无法推进夜间验收入口。";
            }

            int currentHour = timeManager.GetHour();
            if (currentHour < FreeTimeNightWarningHour)
            {
                return "执行 Step，模拟推进到夜里 10 点并验证回住处压力。";
            }

            if (currentHour < FreeTimeMidnightWarningHour)
            {
                return "执行 Step，模拟推进到午夜并验证夜深提醒。";
            }

            if (currentHour < FreeTimeFinalWarningHour)
            {
                return "执行 Step，模拟推进到凌晨一点并验证最终催促。";
            }

            return "回住处休息，或继续执行 Step 验证两点规则收束。";
        }

        public string TryAdvanceFreeTimeValidationStep()
        {
            if (!IsSleepInteractionAvailable())
            {
                return "自由时段尚未开放，不能推进夜间验收入口。";
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return "当前缺少 TimeManager，无法推进夜间验收入口。";
            }

            int currentHour = timeManager.GetHour();
            if (currentHour < FreeTimeNightWarningHour)
            {
                timeManager.SetTime(timeManager.GetYear(), timeManager.GetSeason(), timeManager.GetDay(), FreeTimeNightWarningHour, 0);
                return "验收入口：已模拟推进到夜里 10 点，回住处压力应已增强。";
            }

            if (currentHour < FreeTimeMidnightWarningHour)
            {
                timeManager.SetTime(timeManager.GetYear(), timeManager.GetSeason(), timeManager.GetDay(), FreeTimeMidnightWarningHour, 0);
                return "验收入口：已模拟推进到午夜，夜深提醒应已触发。";
            }

            if (currentHour < FreeTimeFinalWarningHour)
            {
                timeManager.SetTime(timeManager.GetYear(), timeManager.GetSeason(), timeManager.GetDay(), FreeTimeFinalWarningHour, 0);
                return "验收入口：已模拟推进到凌晨一点，最终催促应已触发。";
            }

            timeManager.Sleep();
            return "验收入口：已模拟两点规则触发，Day1 应进入结束态。";
        }

        private void ApplyValidationEnergyState(int targetCurrent)
        {
            EnergySystem energySystem = EnergySystem.Instance;
            if (energySystem == null)
            {
                return;
            }

            int clampedTarget = Mathf.Clamp(targetCurrent, 0, energySystem.MaxEnergy);
            if (energySystem.CurrentEnergy > clampedTarget)
            {
                energySystem.TryConsumeEnergy(energySystem.CurrentEnergy - clampedTarget);
                return;
            }

            energySystem.SetEnergyState(clampedTarget, energySystem.MaxEnergy);
        }

        public void NotifyWorkbenchCraftProgress(RecipeData recipe, int queueTotal, int queueCompleted, float progress, bool active)
        {
            if (!active || recipe == null)
            {
                _workbenchCraftingActive = false;
                _workbenchCraftProgress = 0f;
                _workbenchCraftQueueTotal = 0;
                _workbenchCraftQueueCompleted = 0;
                _workbenchCraftRecipeName = string.Empty;
                return;
            }

            _workbenchCraftingActive = true;
            _workbenchCraftProgress = Mathf.Clamp01(progress);
            _workbenchCraftQueueTotal = Mathf.Max(1, queueTotal);
            _workbenchCraftQueueCompleted = Mathf.Clamp(queueCompleted, 0, _workbenchCraftQueueTotal);
            _workbenchCraftRecipeName = string.IsNullOrWhiteSpace(recipe.recipeName)
                ? $"配方 {recipe.recipeID}"
                : recipe.recipeName;
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

        public bool ShouldExposeWorkbenchInteraction()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return true;
            }

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => false,
                StoryPhase.EnterVillage => false,
                StoryPhase.HealingAndHP => false,
                _ => true
            };
        }

        public bool ShouldShowWorkbenchEntryHint()
        {
            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            return currentPhase == StoryPhase.WorkbenchFlashback && !_workbenchOpened;
        }

        public bool IsFirstFollowupPending()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.EnterVillage)
            {
                return false;
            }

            DialogueManager manager = DialogueManager.Instance;
            return manager == null || !manager.HasCompletedSequence(HouseArrivalSequenceId);
        }

        public bool IsWorkbenchFlashbackAwaitingInteraction()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.WorkbenchFlashback
                && !_workbenchOpened;
        }

        public bool IsDinnerDialoguePendingStart()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.DinnerConflict
                && _dinnerSequencePlayed
                && !IsDialogueSequenceCurrentlyActive(DinnerSequenceId);
        }

        public bool IsReminderDialoguePendingStart()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.ReturnAndReminder
                && _returnSequencePlayed
                && !IsDialogueSequenceCurrentlyActive(ReminderSequenceId);
        }

        public bool IsSleepInteractionAvailable()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.FreeTime
                && !_dayEnded;
        }

        public string GetFreeTimePressureState()
        {
            if (!IsSleepInteractionAvailable())
            {
                return "inactive";
            }

            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.Relaxed => "settled",
                FreeTimePressureTier.NightWarning => "night",
                FreeTimePressureTier.AfterMidnight => "midnight",
                FreeTimePressureTier.FinalCall => "final-call",
                _ => "inactive"
            };
        }

        private void BeginReturnReminder()
        {
            StoryManager.Instance.SetPhase(StoryPhase.ReturnAndReminder);
            EnergySystem.Instance.PlayRestoreAnimation(dinnerRestoreEnergy, dinnerRestoreDuration);
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Show(ReturnReminderBridgePromptText);

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
            _freeTimeNightWarningShown = false;
            _freeTimeMidnightWarningShown = false;
            _freeTimeFinalWarningShown = false;
            StoryManager.Instance.SetPhase(StoryPhase.FreeTime);
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Show(GetFreeTimePromptTextForCurrentTime());

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager != null)
            {
                ShowFreeTimePressurePromptForHour(timeManager.GetHour());
            }
        }

        public bool TryTriggerSleepFromBed()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (_dayEnded || storyManager == null || storyManager.CurrentPhase != StoryPhase.FreeTime)
            {
                return false;
            }

            if (TimeManager.Instance == null)
            {
                return false;
            }

            TimeManager.Instance.Sleep();
            return true;
        }

        public string GetRestInteractionHint(string fallbackHint)
        {
            if (!IsSleepInteractionAvailable())
            {
                return fallbackHint;
            }

            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "赶紧睡觉",
                FreeTimePressureTier.AfterMidnight => "尽快回屋睡觉",
                FreeTimePressureTier.NightWarning => "回屋休息",
                _ => fallbackHint
            };
        }

        public string GetRestInteractionDetail(string fallbackDetail)
        {
            if (!IsSleepInteractionAvailable())
            {
                return fallbackDetail;
            }

            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "按 E 立刻睡觉收束今天",
                FreeTimePressureTier.AfterMidnight => "按 E 回屋睡觉，别再继续逗留",
                FreeTimePressureTier.NightWarning => "按 E 回屋休息，今天随时可以收束",
                _ => fallbackDetail
            };
        }

        public string GetCurrentWorldHintSummary()
        {
            string focusSummary = TryGetCurrentFocusSummary();
            if (!string.IsNullOrWhiteSpace(focusSummary) && focusSummary != "none")
            {
                return focusSummary;
            }

            SpringDay1WorldHintBubble worldHintBubble = FindFirstObjectByType<SpringDay1WorldHintBubble>(FindObjectsInactive.Include);
            bool isVisible = TryGetPublicPropertyValue(worldHintBubble, "IsVisible", false);
            if (worldHintBubble == null || !isVisible)
            {
                return "none";
            }

            Transform anchorTarget = TryGetPublicPropertyValue<Transform>(worldHintBubble, "CurrentAnchorTarget");
            string anchorName = anchorTarget != null ? anchorTarget.name : "unknown";
            string keyLabel = TryGetPublicPropertyValue(worldHintBubble, "CurrentKeyLabel", string.Empty);
            string captionText = TryGetPublicPropertyValue(worldHintBubble, "CurrentCaptionText", string.Empty);
            string detailText = TryGetPublicPropertyValue(worldHintBubble, "CurrentDetailText", string.Empty);
            return $"{anchorName}|{SanitizePlayerFacingText(keyLabel)}|{SanitizePlayerFacingText(captionText)}|{SanitizePlayerFacingText(detailText)}";
        }

        public string BuildPlayerFacingStatusSummary()
        {
            PromptCardModel card = StoryManager.Instance != null
                ? BuildPromptCardModel()
                : new PromptCardModel
                {
                    StageLabel = "未初始化",
                    FocusText = "等待 Day1 运行时对象就位。",
                    FooterText = "暂无正式阶段信息。"
                };
            return $"{SanitizePlayerFacingText(card.StageLabel)}|focus={SanitizePlayerFacingText(card.FocusText)}|progress={SanitizePlayerFacingText(card.FooterText)}|hint={SanitizePlayerFacingText(GetCurrentWorldHintSummary())}";
        }

        private static string SanitizePlayerFacingText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value
                .Replace('\n', ' ')
                .Replace('\r', ' ')
                .Replace('|', '/')
                .Trim();
        }

        private static string TryGetCurrentFocusSummary()
        {
            const string proximityTypeName = "Sunset.Story.SpringDay1ProximityInteractionService";

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int index = 0; index < assemblies.Length; index++)
            {
                Type type = assemblies[index].GetType(proximityTypeName);
                if (type == null)
                {
                    continue;
                }

                PropertyInfo property = type.GetProperty("CurrentFocusSummary", BindingFlags.Static | BindingFlags.Public);
                if (property == null)
                {
                    return string.Empty;
                }

                return property.GetValue(null) as string ?? string.Empty;
            }

            return string.Empty;
        }

        private static T TryGetPublicPropertyValue<T>(object target, string propertyName, T fallback = default)
        {
            if (target == null)
            {
                return fallback;
            }

            PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
            {
                return fallback;
            }

            object value = property.GetValue(target);
            return value is T typed ? typed : fallback;
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
            while (true)
            {
                DialogueManager dialogueManager = DialogueManager.Instance;
                if (dialogueManager == null)
                {
                    yield return null;
                    continue;
                }

                if (!dialogueManager.IsDialogueActive)
                {
                    dialogueManager.PlayDialogue(sequence);
                    yield break;
                }

                yield return null;
            }
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
            _trackedWoodCountSnapshot = _baselineWoodCount;
            _collectedWoodSinceWoodStepStart = 0;

            if (_baselineWoodCount >= requiredWoodCollectedCount)
            {
                _woodTrackingArmed = true;
                _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
                _woodObjectiveCompleted = true;
            }
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
            InventoryService inventoryService = ResolvePreferredWoodInventoryService();
            if (inventoryService == null)
            {
                return 0;
            }

            return CountWoodInInventory(inventoryService);
        }

        private InventoryService ResolvePreferredWoodInventoryService()
        {
            if (_observedInventoryService != null)
            {
                return _observedInventoryService;
            }

            InventoryService[] inventoryServices = FindObjectsByType<InventoryService>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (inventoryServices == null || inventoryServices.Length == 0)
            {
                return null;
            }

            InventoryService bestService = null;
            int bestScore = int.MinValue;
            for (int index = 0; index < inventoryServices.Length; index++)
            {
                InventoryService candidate = inventoryServices[index];
                if (candidate == null)
                {
                    continue;
                }

                int woodCount = CountWoodInInventory(candidate);
                int score = woodCount;
                if (woodCount >= requiredWoodCollectedCount)
                {
                    score += 1000;
                }

                if (candidate.isActiveAndEnabled)
                {
                    score += 100;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestService = candidate;
                }
            }

            return bestService;
        }

        private static int CountWoodInInventory(InventoryService inventoryService)
        {
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
            if (!_farmingTutorialTrackingInitialized)
            {
                return 0;
            }

            return _woodObjectiveCompleted
                ? requiredWoodCollectedCount
                : Mathf.Clamp(_collectedWoodSinceWoodStepStart, 0, requiredWoodCollectedCount);
        }

        private void ArmWoodTracking()
        {
            _woodTrackingArmed = true;
            if (_trackedWoodCountSnapshot < 0)
            {
                _trackedWoodCountSnapshot = GetCurrentWoodCount();
            }

            if (_collectedWoodSinceWoodStepStart >= requiredWoodCollectedCount)
            {
                _woodObjectiveCompleted = true;
            }
        }

        private int GetCompletedTutorialObjectiveCount()
        {
            int completedCount = 0;
            if (_tillObjectiveCompleted)
            {
                completedCount++;
            }

            if (_plantObjectiveCompleted)
            {
                completedCount++;
            }

            if (_waterObjectiveCompleted)
            {
                completedCount++;
            }

            if (_woodObjectiveCompleted)
            {
                completedCount++;
            }

            if (_craftedCount >= requiredCraftedCount)
            {
                completedCount++;
            }

            return completedCount;
        }

        private string BuildWorkbenchCraftProgressText()
        {
            if (!_workbenchCraftingActive || string.IsNullOrWhiteSpace(_workbenchCraftRecipeName))
            {
                return $"教学进度 {GetCompletedTutorialObjectiveCount()}/5";
            }

            int currentItemIndex = Mathf.Clamp(_workbenchCraftQueueCompleted + 1, 1, Mathf.Max(1, _workbenchCraftQueueTotal));
            int percent = Mathf.RoundToInt(_workbenchCraftProgress * 100f);
            return _workbenchCraftQueueTotal > 1
                ? $"工作台制作中 · {_workbenchCraftRecipeName} · {currentItemIndex}/{_workbenchCraftQueueTotal} · {percent}%"
                : $"工作台制作中 · {_workbenchCraftRecipeName} · {percent}%";
        }

        private string BuildFreeTimeProgressText()
        {
            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "快到凌晨两点了，必须立刻回去睡觉",
                FreeTimePressureTier.AfterMidnight => "已经过了午夜，尽快回住处结束今天",
                FreeTimePressureTier.NightWarning => "夜深了，最好尽快回住处休息",
                _ => FreeTimeProgressText
            };
        }

        private string GetFreeTimeFocusText()
        {
            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "现在不要再逗留，立刻回住处睡觉。",
                FreeTimePressureTier.AfterMidnight => "已经过了午夜，先回住处睡觉再说。",
                FreeTimePressureTier.NightWarning => "夜色已经深了，别走太远，记得回住处休息。",
                _ => FreeTimePromptText
            };
        }

        private string BuildFreeTimeTaskDetail()
        {
            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "快到凌晨两点了，再拖会直接昏睡过去。",
                FreeTimePressureTier.AfterMidnight => "已经过了午夜，今晚该尽快收尾回去睡觉。",
                FreeTimePressureTier.NightWarning => "天已经黑透了，别在外面逗留太久。",
                _ => "现在可以自由活动，也可以直接回住处睡觉。"
            };
        }

        private string GetFreeTimePromptTextForCurrentTime()
        {
            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => FreeTimeFinalPromptText,
                FreeTimePressureTier.AfterMidnight => FreeTimeMidnightPromptText,
                FreeTimePressureTier.NightWarning => FreeTimeNightPromptText,
                _ => FreeTimePromptText
            };
        }

        private void ShowFreeTimePressurePromptForHour(int hour)
        {
            if (hour >= FreeTimeFinalWarningHour && !_freeTimeFinalWarningShown)
            {
                _freeTimeFinalWarningShown = true;
                SpringDay1PromptOverlay.Instance.Show(FreeTimeFinalPromptText);
                return;
            }

            if (hour >= FreeTimeMidnightWarningHour && !_freeTimeMidnightWarningShown)
            {
                _freeTimeMidnightWarningShown = true;
                SpringDay1PromptOverlay.Instance.Show(FreeTimeMidnightPromptText);
                return;
            }

            if (hour >= FreeTimeNightWarningHour && !_freeTimeNightWarningShown)
            {
                _freeTimeNightWarningShown = true;
                SpringDay1PromptOverlay.Instance.Show(FreeTimeNightPromptText);
            }
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

        private void ResyncLowEnergyState(bool allowPrompt)
        {
            EnergySystem energySystem = EnergySystem.Instance;
            if (energySystem == null)
            {
                ApplyLowEnergyMovementPenalty(false);
                _lowEnergyWarned = false;
                return;
            }

            bool shouldWarn = _staminaRevealed
                && energySystem.CurrentEnergy > 0
                && energySystem.CurrentEnergy <= lowEnergyWarningThreshold;

            energySystem.SetLowEnergyWarningVisual(shouldWarn);
            ApplyLowEnergyMovementPenalty(shouldWarn);

            if (!allowPrompt)
            {
                _lowEnergyWarned = shouldWarn;
                return;
            }

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

        private FreeTimePressureTier GetFreeTimePressureTier()
        {
            if (!IsSleepInteractionAvailable())
            {
                return FreeTimePressureTier.Inactive;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return FreeTimePressureTier.Relaxed;
            }

            int currentHour = timeManager.GetHour();
            if (currentHour >= FreeTimeFinalWarningHour)
            {
                return FreeTimePressureTier.FinalCall;
            }

            if (currentHour >= FreeTimeMidnightWarningHour)
            {
                return FreeTimePressureTier.AfterMidnight;
            }

            if (currentHour >= FreeTimeNightWarningHour)
            {
                return FreeTimePressureTier.NightWarning;
            }

            return FreeTimePressureTier.Relaxed;
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

            TimeManager timeManager = FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
            if (timeManager != null)
            {
                timeManager.ResumeTime(StoryTimePauseSource);
            }

            _storyTimePauseApplied = false;
        }

        private enum FreeTimePressureTier
        {
            Inactive,
            Relaxed,
            NightWarning,
            AfterMidnight,
            FinalCall
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
        private bool _validationBorrowedRunInBackground;
        private bool _previousRunInBackground;

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

        private void OnDestroy()
        {
            RestoreValidationRunInBackground();

            if (_instance == this)
            {
                _instance = null;
            }
        }

        public string BootstrapRuntime()
        {
            EnsureValidationRunInBackground();
            _ = StoryManager.Instance;
            SpringDay1Director.EnsureRuntime();
            SpringDay1PromptOverlay.EnsureRuntime();
            _ = HealthSystem.Instance;
            _ = EnergySystem.Instance;
            _ = TimeManager.Instance;

            return SetActionResult("已确保 StoryManager / Day1Director / PromptOverlay / HP / EP / Time 运行时对象就位，并允许失焦时继续推进自动演出。");
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
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
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
            AppendPair("EP", energySystem != null ? $"{energySystem.CurrentEnergy}/{energySystem.MaxEnergy}|visible={energySystem.IsVisible}|warn={energySystem.IsLowEnergyWarningActive}" : "n/a");
            AppendPair("Move", BuildMovementSummary(playerMovement));
            AppendPair("Time", timeManager != null ? $"paused={timeManager.IsTimePaused()}|depth={timeManager.GetPauseStackDepth()}|clock={timeManager.GetFormattedTime()}" : "n/a");
            AppendPair("Input", inputManager != null ? inputManager.IsInputEnabledForDebug.ToString() : "n/a");
            AppendPair("Crowd", SpringDay1NpcCrowdDirector.CurrentRuntimeSummary);
            AppendPair("WorldHint", director != null ? director.GetCurrentWorldHintSummary() : "n/a");
            AppendPair("PlayerFacing", director != null ? director.BuildPlayerFacingStatusSummary() : "n/a");
            AppendPair(
                "Director",
                director != null
                    ? $"{director.GetCurrentTaskLabel()}|{director.GetCurrentProgressLabel()}|followupPending={director.IsFirstFollowupPending()}|workbenchAwaiting={director.IsWorkbenchFlashbackAwaitingInteraction()}|dinnerPending={director.IsDinnerDialoguePendingStart()}|reminderPending={director.IsReminderDialoguePendingStart()}|sleepReady={director.IsSleepInteractionAvailable()}|nightPressure={director.GetFreeTimePressureState()}"
                    : "n/a");
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

            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => "触发 NPC001 首段对话，接住醒来与撤离链。",
                StoryPhase.EnterVillage => director != null && director.IsFirstFollowupPending()
                    ? "等待进村围观与安置收束；若链路未续播可再次触发 NPC001。"
                    : "进村安置已收束，等待疗伤段启动。",
                StoryPhase.HealingAndHP => "等待血条渐显与疗伤对话自动播出。",
                StoryPhase.WorkbenchFlashback => director != null && director.IsWorkbenchFlashbackAwaitingInteraction()
                    ? "交互 Anvil_0 / Workbench，触发工作台闪回。"
                    : "工作台已打开，等待工作台回忆自动播出。",
                StoryPhase.FarmingTutorial => director != null
                    ? director.GetValidationFarmingNextAction()
                    : "执行 Step，模拟农田教学的最小推进。",
                StoryPhase.DinnerConflict => director != null && director.IsDinnerDialoguePendingStart()
                    ? "晚餐对白已排队，等待接管。"
                    : "晚餐对白进行中，继续推进即可。",
                StoryPhase.ReturnAndReminder => director != null && director.IsReminderDialoguePendingStart()
                    ? "归途提醒对白已排队，等待接管。"
                    : "归途提醒对白进行中，继续推进即可。",
                StoryPhase.FreeTime => director != null && director.IsSleepInteractionAvailable()
                    ? director.GetValidationFreeTimeNextAction()
                    : "自由时段尚未开放睡觉收束。",
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

            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => SetActionResult(TryTriggerNpcDialogue()),
                StoryPhase.EnterVillage => SetActionResult(director != null && director.IsFirstFollowupPending()
                    ? TryTriggerNpcDialogue()
                    : "进村安置已收束，等待疗伤段自动启动。"),
                StoryPhase.HealingAndHP => SetActionResult("疗伤阶段主要由导演自动推进；当前无需额外手动触发。"),
                StoryPhase.WorkbenchFlashback => SetActionResult(director != null && director.IsWorkbenchFlashbackAwaitingInteraction()
                    ? TryTriggerWorkbenchInteraction()
                    : "工作台已打开，等待工作台回忆自动播出。"),
                StoryPhase.FarmingTutorial => SetActionResult(director != null
                    ? director.TryAdvanceFarmingTutorialValidationStep()
                    : "当前缺少 Day1 导演层，无法推进农田教学验收入口。"),
                StoryPhase.DinnerConflict => SetActionResult(director != null && director.IsDinnerDialoguePendingStart()
                    ? "晚餐对白已排队，等待自动接管。"
                    : "晚餐对白进行中；继续执行 Step 即可推进。"),
                StoryPhase.ReturnAndReminder => SetActionResult(director != null && director.IsReminderDialoguePendingStart()
                    ? "归途提醒对白已排队，等待自动接管。"
                    : "归途提醒对白进行中；继续执行 Step 即可推进。"),
                StoryPhase.FreeTime => SetActionResult(director != null && director.IsSleepInteractionAvailable()
                    ? director.TryAdvanceFreeTimeValidationStep()
                    : "自由时段尚未开放睡觉收束。"),
                StoryPhase.DayEnd => SetActionResult("春1日已结束，无需继续推进。"),
                _ => SetActionResult("当前阶段暂无可脚本触发的推荐动作。")
            };
        }

        private string TryTriggerNpcDialogue()
        {
            CloseBlockingPageUiForValidation();
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

            if (context == null)
            {
                return $"NPC {interactable.name} 当前缺少玩家交互上下文。";
            }

            if (interactable.GetBoundaryDistance(context.PlayerPosition) > interactable.InteractionDistance)
            {
                if (!ShouldAllowNpcValidationFallback())
                {
                    return $"NPC {interactable.name} 当前不在有效交互距离内。";
                }

                interactable.OnInteract(context);
                return $"NPC {interactable.name} 当前不在有效交互距离内，已通过验收入口脚本触发 NPC 对话。";
            }

            interactable.OnInteract(context);
            return $"已触发 NPC 对话：{interactable.name}";
        }

        private string TryTriggerWorkbenchInteraction()
        {
            CloseBlockingPageUiForValidation();
            CraftingStationInteractable interactable = FindPreferredComponent<CraftingStationInteractable>(PreferredWorkbenchObjectNames);
            if (interactable == null)
            {
                return "未找到工作台交互脚本（优先查找 Anvil_0 / Workbench / Anvil）。";
            }

            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            bool allowValidationFallback = director != null && director.IsWorkbenchFlashbackAwaitingInteraction();
            InteractionContext context = BuildInteractionContext();
            if (!interactable.CanInteract(context))
            {
                return $"工作台 {interactable.name} 当前不可交互。";
            }

            if (context == null)
            {
                if (!allowValidationFallback)
                {
                    return $"工作台 {interactable.name} 当前缺少玩家交互上下文。";
                }

                interactable.OnInteract(null);
                return $"工作台 {interactable.name} 缺少玩家上下文，已通过验收入口脚本触发工作台回忆。";
            }

            if (interactable.GetBoundaryDistance(context.PlayerPosition) > interactable.InteractionDistance)
            {
                if (!allowValidationFallback)
                {
                    return $"工作台 {interactable.name} 当前不在交互包络线内。";
                }

                interactable.OnInteract(context);
                return $"工作台 {interactable.name} 当前不在交互包络线内，已通过验收入口脚本触发工作台回忆。";
            }

            interactable.OnInteract(context);
            return $"已触发工作台交互：{interactable.name}";
        }

        private string TryTriggerRestInteraction()
        {
            CloseBlockingPageUiForValidation();
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

            bool allowValidationFallback = ShouldAllowRestValidationFallback();
            if (context == null)
            {
                if (!allowValidationFallback)
                {
                    return $"休息承载物 {interactable.name} 当前缺少玩家交互上下文。";
                }

                interactable.OnInteract(null);
                return $"休息承载物 {interactable.name} 缺少玩家上下文，已通过验收入口脚本触发休息交互。";
            }

            if (GetRestBoundaryDistance(interactable, context.PlayerPosition) > interactable.InteractionDistance)
            {
                if (!allowValidationFallback)
                {
                    return $"休息承载物 {interactable.name} 当前不在交互包络线内。";
                }

                interactable.OnInteract(context);
                return $"休息承载物 {interactable.name} 当前不在交互包络线内，已通过验收入口脚本触发休息交互。";
            }

            interactable.OnInteract(context);
            return $"已触发休息交互：{interactable.name}";
        }

        private static bool ShouldAllowNpcValidationFallback()
        {
            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            if (storyManager == null)
            {
                return false;
            }

            return storyManager.CurrentPhase == StoryPhase.CrashAndMeet
                || storyManager.CurrentPhase == StoryPhase.EnterVillage;
        }

        private static bool ShouldAllowRestValidationFallback()
        {
            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            return storyManager != null && storyManager.CurrentPhase == StoryPhase.FreeTime;
        }

        private static float GetRestBoundaryDistance(SpringDay1BedInteractable interactable, Vector2 playerPosition)
        {
            if (interactable == null)
            {
                return float.PositiveInfinity;
            }

            MethodInfo method = typeof(SpringDay1BedInteractable).GetMethod("GetBoundaryDistance", BindingFlags.Instance | BindingFlags.Public);
            if (method != null)
            {
                object result = method.Invoke(interactable, new object[] { playerPosition });
                if (result is float distance)
                {
                    return distance;
                }
            }

            Bounds bounds = SpringDay1UiLayerUtility.TryGetPresentationBounds(interactable.transform, out Bounds presentationBounds)
                ? presentationBounds
                : new Bounds(interactable.transform.position, Vector3.one);
            return Vector2.Distance(playerPosition, bounds.ClosestPoint(playerPosition));
        }

        private static void CloseBlockingPageUiForValidation()
        {
            if (BoxPanelUI.ActiveInstance != null && BoxPanelUI.ActiveInstance.IsOpen)
            {
                BoxPanelUI.ActiveInstance.Close();
            }

            PackagePanelTabsUI packageTabs = FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
            if (packageTabs != null && packageTabs.IsPanelOpen())
            {
                packageTabs.ShowPanel(false);
            }
        }

        private static InteractionContext BuildInteractionContext()
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            Transform playerTransform = playerMovement != null ? playerMovement.transform : null;
            if (playerTransform == null)
            {
                return null;
            }

            return new InteractionContext
            {
                PlayerTransform = playerTransform,
                PlayerPosition = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform)
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

        private string BuildMovementSummary(PlayerMovement playerMovement)
        {
            if (playerMovement == null)
            {
                return "n/a";
            }

            return $"runtimeMultiplier={playerMovement.GetRuntimeSpeedMultiplier():F2}";
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

        private void EnsureValidationRunInBackground()
        {
            if (_validationBorrowedRunInBackground)
            {
                return;
            }

            _previousRunInBackground = Application.runInBackground;
            if (_previousRunInBackground)
            {
                return;
            }

            Application.runInBackground = true;
            _validationBorrowedRunInBackground = true;
        }

        private void RestoreValidationRunInBackground()
        {
            if (!_validationBorrowedRunInBackground)
            {
                return;
            }

            Application.runInBackground = _previousRunInBackground;
            _validationBorrowedRunInBackground = false;
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
