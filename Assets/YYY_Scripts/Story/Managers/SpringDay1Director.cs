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
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Sunset.Story
{
    /// <summary>
    /// spring-day1 运行时导演。
    /// 当前优先把 0.0.3 ~ 0.0.6 串成可推进骨架，并尽量复用现有系统。
    /// </summary>
    public class SpringDay1Director : MonoBehaviour
    {
        public const string DebugWorkbenchSkipEditorPrefKey = "Sunset.SpringDay1.DebugSkipWorkbenchPhase05";

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

        public readonly struct TaskListBridgePromptDisplayState
        {
            public TaskListBridgePromptDisplayState(
                bool visible,
                string text,
                string semanticKey)
            {
                Visible = visible;
                Text = text ?? string.Empty;
                SemanticKey = semanticKey ?? string.Empty;
            }

            public bool Visible { get; }
            public string Text { get; }
            public string SemanticKey { get; }
        }

        public readonly struct TaskListVisibilitySemanticState
        {
            public TaskListVisibilitySemanticState(
                bool managedByDay1,
                bool forceHidden,
                bool allowRestore,
                string semanticKey,
                string activeFormalSequenceId,
                string consumedFormalSequenceId)
            {
                ManagedByDay1 = managedByDay1;
                ForceHidden = forceHidden;
                AllowRestore = allowRestore;
                SemanticKey = semanticKey ?? string.Empty;
                ActiveFormalSequenceId = activeFormalSequenceId ?? string.Empty;
                ConsumedFormalSequenceId = consumedFormalSequenceId ?? string.Empty;
            }

            public bool ManagedByDay1 { get; }
            public bool ForceHidden { get; }
            public bool AllowRestore { get; }
            public string SemanticKey { get; }
            public string ActiveFormalSequenceId { get; }
            public string ConsumedFormalSequenceId { get; }
        }

        public readonly struct PlacementSemanticState
        {
            public PlacementSemanticState(
                bool managedByDay1,
                bool allowPlacement,
                bool allowFarmingPlacement,
                bool blockedByFormalDialogue,
                string semanticKey,
                string reason)
            {
                ManagedByDay1 = managedByDay1;
                AllowPlacement = allowPlacement;
                AllowFarmingPlacement = allowFarmingPlacement;
                BlockedByFormalDialogue = blockedByFormalDialogue;
                SemanticKey = semanticKey ?? string.Empty;
                Reason = reason ?? string.Empty;
            }

            public bool ManagedByDay1 { get; }
            public bool AllowPlacement { get; }
            public bool AllowFarmingPlacement { get; }
            public bool BlockedByFormalDialogue { get; }
            public string SemanticKey { get; }
            public string Reason { get; }
        }

        private const string PrimarySceneName = "Primary";
        private const string TownSceneName = "Town";
        private const string HomeSceneName = "Home";
        private const string HomeSceneAssetPath = "Assets/000_Scenes/Home.unity";
        private const string StoryTimePauseSource = "SpringDay1Director";
        private const string FirstSequenceId = "spring-day1-first";
        private const string FirstFollowupSequenceId = "spring-day1-first-followup";
        private const string VillageGateSequenceId = "spring-day1-village-gate";
        private const string HouseArrivalSequenceId = "spring-day1-house-arrival";
        private const string HealingSequenceId = "spring-day1-healing";
        private const string WorkbenchBriefingSequenceId = "spring-day1-workbench-briefing";
        private const string WorkbenchSequenceId = "spring-day1-workbench";
        private const string PostTutorialWrapSequenceId = "spring-day1-post-tutorial-wrap";
        private const string DinnerSequenceId = "spring-day1-dinner";
        private const string ReminderSequenceId = "spring-day1-reminder";
        private const string FreeTimeIntroSequenceId = "spring-day1-free-time-opening";
        private const string CrowdManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";
        private const string HealingDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_Healing.asset";
        private const string VillageGateDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset";
        private const string HouseArrivalDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset";
        private const string WorkbenchDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_WorkbenchRecall.asset";
        private const string PostTutorialWrapDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_PostTutorialWrap.asset";
        private const string DinnerDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_DinnerConflict.asset";
        private const string ReminderDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_ReturnReminder.asset";
        private const string FreeTimeIntroDialogueAssetPath = "Assets/111_Data/Story/Dialogue/SpringDay1_FreeTimeOpening.asset";
        private static readonly string[] PreferredWorkbenchObjectNames = { "Anvil_0", "Workbench", "Anvil" };
        private static readonly string[] PreferredBedObjectNames = { "Bed", "PlayerBed", "HomeBed" };
        private static readonly string[] PreferredRestProxyObjectNames = { "House 1_2", "HomeDoor", "HouseDoor", "Door" };
        private static readonly string[] PreferredRestProxyKeywords = { "door", "entry", "entrance", "house", "home" };
        private const string ChiefNpcId = "001";
        private const string CompanionNpcId = "002";
        private const string ThirdResidentNpcId = "003";
        private const string DirectorResidentControlOwnerKey = "spring-day1-director";
        private static readonly string[] PreferredTownChiefObjectNames = { "001", "NPC001" };
        private static readonly string[] PreferredTownCompanionObjectNames = { "002", "NPC002" };
        private static readonly string[] PreferredTownThirdResidentObjectNames = { "003", "NPC003" };
        private static readonly string[] PreferredTownLeadTargetObjectNames = { "EnterVillage_HouseLead", "进屋安置点", "进村安置点", "SceneTransitionTrigger" };
        private static readonly string[] PreferredTownTransitionTriggerObjectNames = { "SceneTransitionTrigger" };
        private static readonly string[] PreferredTownOpeningLayoutGroupObjectNames = { "进村围观", "EnterVillageCrowdRoot", "VillageCrowd" };
        private static readonly string[] PreferredPrimaryArrivalAnchorObjectNames = { "PrimaryHomeEntryAnchor", "PrimaryEntryAnchor" };
        private static readonly string[] PreferredPrimaryEntryGroupObjectNames = { "刚进primary" };
        private static readonly string[] PreferredPrimaryWorkbenchEscortGroupObjectNames = { "到工作台的npc终点" };
        private static readonly string[] PreferredPrimaryWorkbenchIdleGroupObjectNames = { "工作台结束，在旁等待站位" };
        private static readonly string[] PreferredVillageCrowdMarkerRootObjectNames = { "EnterVillageCrowdRoot", "进村围观", "VillageCrowd" };
        private static readonly string[] PreferredDinnerGatheringAnchorObjectNames = { "DirectorReady_DinnerBackgroundRoot", "DinnerBackgroundRoot" };
        private const string PrimaryEntryChiefPointName = "村长初始站位";
        private const string PrimaryEntryCompanionPointName = "艾拉初始站位";
        private const string PrimaryEntryPlayerPointName = "玩家初始站位";
        private const string TownOpeningEndGroupName = "终点";
        private const string TownOpeningChiefPointName = "001终点";
        private const string TownOpeningCompanionPointName = "002终点";
        private const string TownOpeningThirdResidentPointName = "003终点";
        private const string StoryNpcChiefLabel = "001";
        private const string StoryNpcCompanionLabel = "002";
        private const string BedInteractionHint = "睡觉";
        private const string RestProxyInteractionHint = "回屋休息";
        private const string TownLeadBridgePromptText = "跟住村长和艾拉往住处走，别在村口掉队。";
        private const string TownLeadWaitPromptText = "小伙子，先跟我走";
        private const string TownLeadTransitionPromptText = "先跟紧，到了里面先站稳，艾拉马上接手。";
        private const string HealingBridgePromptText = "走到艾拉身边，她会先替你把伤势稳住。";
        private const string HealingBridgeArrivalPromptText = "先别动，让艾拉把你的伤稳住。";
        private const string WorkbenchEscortPromptText = "跟着村长和艾拉去工作台，把今晚该学的手艺先接回来。";
        private const string WorkbenchBriefingPromptText = "先听村长把工作台和边上的补给箱交代清楚。";
        private const string WorkbenchEscortWaitPromptText = "小伙子，先跟我走";
        private const string WorkbenchEscortReadyPromptText = "村长已经把工作台和补给箱交代清楚了，按 E 打开制作菜单。";
        private const string PostTutorialWrapPromptText = "农田这边先收住，去和村长说一声。";
        private const string PostTutorialWrapProgressText = "农田与工作台目标已完成，等待和村长收口";
        private const string PostTutorialExplorePromptText = "今晚可以四处看看；如果不想等到晚上，回村找村长聊聊就能直接开饭。";
        private const string PostTutorialExploreProgressText = "傍晚自由活动中，可继续探索；想直接开饭就回村找村长";
        private const string FreeTimeIntroBridgePromptText = "先听一圈村里夜里的动静，再决定什么时候回屋。";
        private const string FreeTimePromptText = "今晚可以自由活动，记得回住处休息。";
        private const string FreeTimeProgressText = "自由活动中，回住处休息即可结束";
        private const string FreeTimeIntroPendingProgressText = "等待自由时段见闻接管";
        private const string FreeTimeIntroActiveProgressText = "自由时段见闻进行中";
        private const string DinnerBridgePromptText = "先跟着村长和艾拉回村里，晚饭那一桌马上就会接管。";
        private const string DinnerBridgeChiefOnlyPromptText = "先跟着村长回村里，晚饭那一桌马上就会接管。";
        private const string DinnerBridgeCompanionOnlyPromptText = "先跟着艾拉回村里，晚饭那一桌马上就会接管。";
        private const string DinnerEscortWaitPromptText = "小伙子，先跟我走";
        private const string DinnerEscortTransitionPromptText = "先一起回村，晚饭那一桌马上接管。";
        private const string ReturnReminderBridgePromptText = "夜色越来越深了，别在外面逗留太久。";
        private const string FreeTimeNightPromptText = "夜深了，别在外面逗留太久，记得回住处休息。";
        private const string FreeTimeMidnightPromptText = "已经过了午夜，尽快回住处。再拖下去今晚会更难熬。";
        private const string FreeTimeFinalPromptText = "快到凌晨两点了，再不睡就会直接昏睡过去。";
        private const string DayEndPromptText = "第一夜先熬过去了。明天开始，再让村里看清你。";
        private const string DayEndProgressText = "第一夜已经熬过去，明天还得继续用做活证明自己";
        private const int TownOpeningHour = 9;
        private const int TutorialTimeCapHour = 16;
        private const int DinnerReturnHour = 18;
        private const int DinnerReturnMinute = 0;
        private const int ReminderStartHour = 19;
        private const int ReminderStartMinute = 0;
        private const int FreeTimeStartHour = 19;
        private const int FreeTimeStartMinute = 30;
        private const int StoryActorReturnHomeHour = 20;
        private const int StoryActorForcedRestHour = 21;
        private const int StoryActorMorningReleaseHour = 9;
        private const int FreeTimeNightWarningHour = 22;
        private const int FreeTimeMidnightWarningHour = 24;
        private const int FreeTimeFinalWarningHour = 25;
        private const int Day1RuntimeManagedYear = 1;
        private const int Day1RuntimeManagedDay = 1;
        private static readonly Vector2 HomeDoorSleepFallbackOffset = new Vector2(-2.6f, 2.45f);
        private static readonly Vector3 DinnerGatheringTownPlayerPosition = new Vector3(-15.64f, 10.19f, 0.25189802f);
        private static readonly Vector2 DinnerGatheringChiefAnchorOffset = new Vector2(0.70f, 1.35f);
        private static readonly Vector2 DinnerGatheringCompanionAnchorOffset = new Vector2(2.20f, 1.25f);
        private static readonly Vector2 DinnerGatheringChiefFallbackOffset = new Vector2(-0.86f, -0.24f);
        private static readonly Vector2 DinnerGatheringCompanionFallbackOffset = new Vector2(0.64f, -0.34f);

        private static SpringDay1Director _instance;
        private static FieldInfo _farmTilesField;
        private static SpringDay1NpcCrowdManifest _cachedCrowdManifest;

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
        [SerializeField] private bool debugSkipDirectToWorkbenchPhase05 = false;

        [Header("Town Lead")]
        [SerializeField, Min(0.1f)] private float townHouseLeadChiefArrivalDistance = 0.65f;
        [SerializeField, Min(0.1f)] private float townHouseLeadPlayerTransitionDistance = 0.55f;
        [SerializeField, Min(0.1f)] private float townHouseLeadMoveRetryInterval = 0.85f;
        [SerializeField, Min(0.1f)] private float escortMaxLeadDistance = 5f;
        [SerializeField, Min(0.1f)] private float escortResumeLeadDistanceBuffer = 1.25f;
        [SerializeField, Min(0f)] private float escortResumeConfirmationDuration = 0.18f;
        [SerializeField, Min(0.1f)] private float escortBubbleCooldown = 2.2f;
        [SerializeField, Min(0.05f)] private float escortRetargetTolerance = 0.45f;
        [SerializeField, Min(0.05f)] private float escortCompanionSideOffset = 0.45f;
        [SerializeField, Min(0.05f)] private float escortCompanionTrailingOffset = 0.35f;
        [SerializeField, Min(0.1f)] private float healingSupportApproachDistance = 0.9f;
        [SerializeField, Min(0f)] private float healingSupportPauseDuration = 0.18f;
        [SerializeField, Min(0.1f)] private float primaryArrivalReframeDistance = 1.35f;
        [SerializeField, Min(0f)] private float primaryArrivalDialogueDelay = 0.18f;
        [SerializeField] private Vector2 primaryArrivalChiefOffset = new Vector2(-0.7f, 0.15f);
        [SerializeField] private Vector2 primaryArrivalCompanionOffset = new Vector2(0.75f, -0.05f);
        [SerializeField, Min(0f)] private float storyBlinkFadeOutDuration = 0.08f;
        [SerializeField, Min(0f)] private float storyBlinkHoldDuration = 0.04f;
        [SerializeField, Min(0f)] private float storyBlinkFadeInDuration = 0.08f;
        [SerializeField, Min(0f)] private float dayEndTaskCardVisibleSeconds = 2f;
        [SerializeField] private float forcedSleepTownBedOffsetY = -0.9f;
        [SerializeField, Min(0.1f)] private float workbenchEscortReadyDistance = 3f;
        [SerializeField, Min(0f)] private float workbenchEscortDialogueDelay = 0.15f;
        [SerializeField, Min(0f)] private float townVillageGateCueSettleTimeout = 5f;
        [SerializeField, Min(0f)] private float dinnerCueSettleTimeout = 5f;

        private bool _healingStarted;
        private bool _villageGateSequencePlayed;
        private bool _houseArrivalSequencePlayed;
        private bool _healingSequencePlayed;
        private bool _workbenchBriefingSequenceQueued;
        private bool _workbenchSequencePlayed;
        private bool _postTutorialWrapSequenceQueued;
        private bool _postTutorialExploreWindowPending;
        private float _postTutorialExploreWindowEarliestEnterAt = -1f;
        private bool _postTutorialExploreWindowEntered;
        private bool _dinnerGatheringRequested;
        private bool _pendingDinnerTownLoad;
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
        private bool _freeTimeIntroQueued;
        private bool _freeTimeIntroCompleted;
        private bool _debugWorkbenchSkipApplied;
        private bool _townHouseLeadStarted;
        private bool _townHouseLeadTransitionQueued;
        private bool _townHouseLeadWaitingForPlayer;
        private float _townHouseLeadNextMoveRetryAt;
        private float _townHouseLeadNextBubbleAt;
        private float _townHouseLeadCaughtUpSince;
        private string _townHouseLeadChiefName = string.Empty;
        private string _townHouseLeadCompanionName = string.Empty;
        private string _townHouseLeadTargetName = string.Empty;
        private float _townHouseLeadChiefDistance = float.PositiveInfinity;
        private float _townHouseLeadCompanionDistance = float.PositiveInfinity;
        private float _townHouseLeadPlayerDistance = float.PositiveInfinity;
        private float _townVillageGateCueWaitStartedAt = -1f;
        private float _dinnerCueWaitStartedAt = -1f;
#if UNITY_EDITOR
        private bool? _editorDinnerCueSettledOverride;
#endif
        private bool _primaryArrivalActorsAligned;
        private float _primaryArrivalDialogueReadyAt;
        private bool _healingBridgePending;
        private bool _healingBridgeSequenceQueued;
        private float _healingBridgeHoldUntil;
        private string _healingBridgeNpcName = string.Empty;
        private float _healingBridgeNpcDistance = float.PositiveInfinity;
        private bool _workbenchEscortStarted;
        private bool _workbenchEscortWaitingForPlayer;
        private bool _workbenchEscortIdlePlaced;
        private float _workbenchEscortNextMoveRetryAt;
        private float _workbenchEscortNextBubbleAt;
        private float _workbenchEscortCaughtUpSince;
        private float _workbenchEscortDialogueReadyAt;
        private string _workbenchEscortChiefName = string.Empty;
        private string _workbenchEscortCompanionName = string.Empty;
        private string _workbenchEscortTargetName = string.Empty;
        private float _workbenchEscortChiefDistance = float.PositiveInfinity;
        private float _workbenchEscortCompanionDistance = float.PositiveInfinity;
        private float _workbenchEscortPlayerDistance = float.PositiveInfinity;
        private bool _returnEscortStarted;
        private bool _returnEscortTransitionQueued;
        private bool _returnEscortWaitingForPlayer;
        private float _returnEscortNextMoveRetryAt;
        private float _returnEscortNextBubbleAt;
        private float _returnEscortCaughtUpSince;
        private string _returnEscortChiefName = string.Empty;
        private string _returnEscortCompanionName = string.Empty;
        private string _returnEscortTargetName = string.Empty;
        private float _returnEscortChiefDistance = float.PositiveInfinity;
        private float _returnEscortCompanionDistance = float.PositiveInfinity;
        private float _returnEscortPlayerDistance = float.PositiveInfinity;
        private int _cachedSceneHandle = int.MinValue;
        private Transform _cachedChiefActor;
        private Transform _cachedCompanionActor;
        private Transform _cachedThirdResidentActor;
        private Transform _cachedWorkbenchTarget;
        private Transform _cachedTownLeadTarget;
        private SceneTransitionTrigger2D _cachedTownTransitionTrigger;
        private SceneTransitionTrigger2D _cachedPrimaryTownTransitionTrigger;
        private bool _dayEndTaskCardAutoHideArmed;
        private float _dayEndTaskCardShownAt = -1f;
        private bool _suppressDay1TimeGuardrail;
        private bool _storyActorNightRestActive;
        private int _pendingForcedSleepRestPlacementFrames;
        private string _taskListBridgePromptText = string.Empty;
        private string _taskListBridgePromptPhaseKey = string.Empty;

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
            SpringDay1NpcCrowdDirector.EnsureRuntime();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueSequenceCompletedEvent>(HandleDialogueSequenceCompleted, owner: this);
            EventBus.Subscribe<DialogueEndEvent>(HandleDialogueEnded, owner: this);
            EventBus.Subscribe<StoryPhaseChangedEvent>(HandleStoryPhaseChanged, owner: this);
            SceneManager.sceneLoaded += HandleSceneLoaded;
            EnergySystem.OnEnergyChanged += HandleEnergyChanged;
            TimeManager.OnHourChanged += HandleHourChanged;
            TimeManager.OnSleep += HandleSleep;
            RefreshCraftingServiceSubscription();
            RefreshInventoryTrackingSubscription();
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            SceneManager.sceneLoaded -= HandleSceneLoaded;
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
            if (!IsStoryRuntimeSceneActive())
            {
                ReleaseStoryTimePause();
                return;
            }

            EnsureNpcCrowdRuntimeForActiveDay1Scene();
            EnforceDay1TimeGuardrails();

            if (Time.unscaledTime < _nextPollAt)
            {
                TryFinalizePendingForcedSleepRestPlacement();
                return;
            }

            _nextPollAt = Time.unscaledTime + 0.2f;
            if (IsTownSceneActive())
            {
                TickTownSceneFlow();
            }
            else if (IsHomeSceneActive())
            {
                TickHomeSceneFlow();
            }
            else
            {
                TickPrimarySceneFlow();
            }

            TryFinalizePendingForcedSleepRestPlacement();
        }

        private static void EnsureNpcCrowdRuntimeForActiveDay1Scene()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            if (currentPhase < StoryPhase.EnterVillage || currentPhase > StoryPhase.DayEnd)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
            {
                return;
            }

            bool shouldEnsureForScene =
                string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal)
                || string.Equals(activeScene.name, PrimarySceneName, StringComparison.Ordinal);
            if (!shouldEnsureForScene)
            {
                return;
            }

            SpringDay1NpcCrowdDirector.EnsureRuntime();
        }

        public string GetDebugSummary()
        {
            StoryManager storyManager = StoryManager.Instance;
            string phaseText = storyManager != null ? storyManager.CurrentPhase.ToString() : "n/a";
            return $"Phase={phaseText}, Tilled={GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}, Planted={GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}, Watered={GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}, Wood={GetCollectedWoodProgress()}/{requiredWoodCollectedCount}, Crafted={_craftedCount}, Wrap={_postTutorialWrapSequenceQueued}, Explore={_postTutorialExploreWindowEntered}, DinnerReq={_dinnerGatheringRequested}, FreeTime={_freeTimeEntered}, DayEnd={_dayEnded}";
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
                StoryPhase.FarmingTutorial when IsPostTutorialExploreWindowActive() => "0.0.6 傍晚自由活动",
                StoryPhase.FarmingTutorial when IsAwaitingPostTutorialChiefWrap() => "0.0.5 农田教学收口",
                StoryPhase.FarmingTutorial => "0.0.5 农田/砍树教学",
                StoryPhase.DinnerConflict => "0.0.6 晚餐冲突",
                StoryPhase.ReturnAndReminder => "0.0.6 归途提醒",
                StoryPhase.FreeTime => "0.0.6 自由时段",
                StoryPhase.DayEnd => "0.0.6 第一夜收束",
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
                    return "村里承接对白进行中";
                }

                if (IsTownHouseLeadPending())
                {
                    if (_townHouseLeadWaitingForPlayer)
                    {
                        return "你掉队了，村长和艾拉正在前面等你";
                    }

                    return _townHouseLeadTransitionQueued
                        ? "进村承接即将切到 Primary"
                        : "村口围观已过，村长正带着你往住处走";
                }

                if (HasVillageGateProgressed() && !HasHouseArrivalProgressed())
                {
                    return "村口围观已过，等待 Primary 承接对白接上";
                }

                return IsFirstFollowupPending()
                    ? "等待进村围观与安置链接管"
                    : "进村安置已收束，等待疗伤段启动";
            }

            if (phase == StoryPhase.HealingAndHP)
            {
                if (_healingBridgePending && !_healingBridgeSequenceQueued)
                {
                    return "先走到艾拉身边，让她接手疗伤";
                }

                return "等待疗伤对话结束";
            }

            if (phase == StoryPhase.WorkbenchFlashback)
            {
                if (!_workbenchOpened)
                {
                    if (_workbenchEscortWaitingForPlayer)
                    {
                        return "你掉队了，村长和艾拉正在等你去工作台";
                    }

                    if (_workbenchEscortStarted)
                    {
                        return "村长和艾拉正带着你去工作台";
                    }

                    return "等待工作台带路接管";
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
                if (IsPostTutorialExploreWindowActive())
                {
                    return PostTutorialExploreProgressText;
                }

                if (IsAwaitingPostTutorialChiefWrap())
                {
                    return PostTutorialWrapProgressText;
                }

                if (_workbenchCraftingActive)
                {
                    return BuildWorkbenchCraftProgressText();
                }

                return $"教学进度 {GetCompletedTutorialObjectiveCount()}/5";
            }

            if (phase == StoryPhase.DinnerConflict)
            {
                if (IsReturnToTownEscortPending())
                {
                    if (_returnEscortWaitingForPlayer)
                    {
                        return "你掉队了，村长和艾拉正在等你回村";
                    }

                    return _returnEscortTransitionQueued
                        ? "回村切场即将接管"
                        : "农田已收束，村长和艾拉正带你回村";
                }

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
                if (IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId))
                {
                    return FreeTimeIntroActiveProgressText;
                }

                if (!_freeTimeIntroCompleted)
                {
                    return FreeTimeIntroPendingProgressText;
                }

                return BuildFreeTimeProgressText();
            }

            if (phase == StoryPhase.DayEnd)
            {
                return DayEndProgressText;
            }

            return "等待推进";
        }

        public TaskListVisibilitySemanticState GetTaskListVisibilitySemanticState()
        {
            if (!IsStoryRuntimeSceneActive())
            {
                return new TaskListVisibilitySemanticState(
                    managedByDay1: false,
                    forceHidden: false,
                    allowRestore: false,
                    semanticKey: "day1:inactive",
                    activeFormalSequenceId: string.Empty,
                    consumedFormalSequenceId: string.Empty);
            }

            string activeFormalSequenceId = ResolveActiveFormalTaskListSequenceId();
            if (!string.IsNullOrEmpty(activeFormalSequenceId))
            {
                return new TaskListVisibilitySemanticState(
                    managedByDay1: true,
                    forceHidden: true,
                    allowRestore: false,
                    semanticKey: $"day1:task-list:hidden:{activeFormalSequenceId}",
                    activeFormalSequenceId: activeFormalSequenceId,
                    consumedFormalSequenceId: activeFormalSequenceId);
            }

            string consumedFormalSequenceId = ResolveLatestConsumedFormalTaskListSequenceId();
            return new TaskListVisibilitySemanticState(
                managedByDay1: true,
                forceHidden: false,
                allowRestore: true,
                semanticKey: string.IsNullOrEmpty(consumedFormalSequenceId)
                    ? "day1:task-list:allow"
                    : $"day1:task-list:restore:{consumedFormalSequenceId}",
                activeFormalSequenceId: string.Empty,
                consumedFormalSequenceId: consumedFormalSequenceId);
        }

        public bool ShouldForceHideTaskListForCurrentStory()
        {
            return GetTaskListVisibilitySemanticState().ForceHidden;
        }

        public string GetTaskListVisibilitySemanticKey()
        {
            return GetTaskListVisibilitySemanticState().SemanticKey;
        }

        public PlacementSemanticState GetPlacementSemanticState()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (!IsStoryRuntimeSceneActive() || storyManager == null)
            {
                return new PlacementSemanticState(
                    managedByDay1: false,
                    allowPlacement: true,
                    allowFarmingPlacement: true,
                    blockedByFormalDialogue: false,
                    semanticKey: "day1:placement:unmanaged",
                    reason: "当前不在 spring-day1 有效运行态，day1 不接管放置语义。");
            }

            string activeFormalSequenceId = ResolveActiveFormalTaskListSequenceId();
            if (!string.IsNullOrEmpty(activeFormalSequenceId))
            {
                return new PlacementSemanticState(
                    managedByDay1: true,
                    allowPlacement: false,
                    allowFarmingPlacement: false,
                    blockedByFormalDialogue: true,
                    semanticKey: $"day1:placement:blocked:{activeFormalSequenceId}",
                    reason: $"正式对白进行中（{activeFormalSequenceId}），应暂时收起放置/播种输入。");
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            return currentPhase switch
            {
                StoryPhase.FarmingTutorial => new PlacementSemanticState(
                    managedByDay1: true,
                    allowPlacement: true,
                    allowFarmingPlacement: true,
                    blockedByFormalDialogue: false,
                    semanticKey: "day1:placement:farming-tutorial-allow",
                    reason: "0.0.5 农田教学阶段必须允许锄地、播种、浇水连续成立。"),
                StoryPhase.FreeTime => new PlacementSemanticState(
                    managedByDay1: true,
                    allowPlacement: true,
                    allowFarmingPlacement: true,
                    blockedByFormalDialogue: false,
                    semanticKey: "day1:placement:free-time-allow",
                    reason: "自由活动阶段允许继续农田/放置链。"),
                _ => new PlacementSemanticState(
                    managedByDay1: true,
                    allowPlacement: true,
                    allowFarmingPlacement: true,
                    blockedByFormalDialogue: false,
                    semanticKey: $"day1:placement:allow:{currentPhase}",
                    reason: $"{currentPhase} 阶段当前不是“剧情禁止播种”的语义来源；若播种失败，应优先排查 placement/preview 运行时链。")
            };
        }

        public bool ShouldAllowFarmingPlacementForCurrentStory()
        {
            return GetPlacementSemanticState().AllowFarmingPlacement;
        }

        public string GetPlacementSemanticKey()
        {
            return GetPlacementSemanticState().SemanticKey;
        }

        public string GetPlacementSemanticReason()
        {
            return GetPlacementSemanticState().Reason;
        }

        public bool ShouldShowPlacementModeGuidance()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (!IsStoryRuntimeSceneActive() || storyManager == null || storyManager.CurrentPhase != StoryPhase.FarmingTutorial)
            {
                return false;
            }

            InitializeFarmingTutorialTracking();
            return !_tillObjectiveCompleted || !_plantObjectiveCompleted || !_waterObjectiveCompleted;
        }

        public string GetPlacementModeGuidanceText()
        {
            if (!ShouldShowPlacementModeGuidance())
            {
                return string.Empty;
            }

            if (!_tillObjectiveCompleted)
            {
                return "按 V 开启放置模式，再用锄头开垦一格土地。";
            }

            if (!_plantObjectiveCompleted)
            {
                return "保持放置模式开启，把花椰菜种子种进刚开垦的土地。";
            }

            if (!_waterObjectiveCompleted)
            {
                return "保持放置模式开启，用浇水壶给作物浇水。";
            }

            return string.Empty;
        }

        public bool CanConsumeStoryNpcInteraction(Transform actor, InteractionContext context)
        {
            if (actor == null || context == null || StoryManager.Instance == null)
            {
                return false;
            }

            if (!ReferenceEquals(actor, ResolveStoryChiefTransform()))
            {
                return false;
            }

            if (StoryManager.Instance.CurrentPhase != StoryPhase.FarmingTutorial)
            {
                return false;
            }

            return (IsAwaitingPostTutorialChiefWrap() && IsPrimarySceneActive())
                || (IsPostTutorialExploreWindowActive() && IsTownSceneActive());
        }

        public bool TryConsumeStoryNpcInteraction(Transform actor, InteractionContext context)
        {
            if (!CanConsumeStoryNpcInteraction(actor, context))
            {
                return false;
            }

            if (IsAwaitingPostTutorialChiefWrap())
            {
                return TryStartPostTutorialWrapSequence();
            }

            if (IsPostTutorialExploreWindowActive())
            {
                return TryRequestDinnerGatheringStart(autoTriggered: false);
            }

            return false;
        }

        public string GetCurrentFocusTextForTests()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null ? GetPromptFocusText(storyManager.CurrentPhase) : string.Empty;
        }

        public void ShowTaskListBridgePrompt(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                HideTaskListBridgePrompt();
                return;
            }

            _taskListBridgePromptText = text.Trim();
            _taskListBridgePromptPhaseKey = GetCurrentPromptPhaseKey();
        }

        public void HideTaskListBridgePrompt()
        {
            _taskListBridgePromptText = string.Empty;
            _taskListBridgePromptPhaseKey = string.Empty;
        }

        public TaskListBridgePromptDisplayState GetTaskListBridgePromptDisplayState()
        {
            string currentPhaseKey = GetCurrentPromptPhaseKey();
            if (!string.IsNullOrWhiteSpace(_taskListBridgePromptPhaseKey)
                && !string.Equals(_taskListBridgePromptPhaseKey, currentPhaseKey, StringComparison.Ordinal))
            {
                HideTaskListBridgePrompt();
            }

            if (string.IsNullOrWhiteSpace(_taskListBridgePromptText))
            {
                return new TaskListBridgePromptDisplayState(false, string.Empty, "day1:bridge-prompt:hidden");
            }

            TaskListVisibilitySemanticState visibilityState = GetTaskListVisibilitySemanticState();
            if (visibilityState.ForceHidden)
            {
                return new TaskListBridgePromptDisplayState(
                    false,
                    string.Empty,
                    $"{visibilityState.SemanticKey}:bridge-hidden");
            }

            PromptCardModel model = BuildPromptCardModel();
            if (!visibilityState.ManagedByDay1 && model == null)
            {
                return new TaskListBridgePromptDisplayState(
                    false,
                    string.Empty,
                    $"{visibilityState.SemanticKey}:bridge-unmanaged");
            }

            if (model != null && IsBridgePromptRedundant(model, _taskListBridgePromptText))
            {
                return new TaskListBridgePromptDisplayState(
                    false,
                    string.Empty,
                    $"day1:bridge-prompt:redundant:{currentPhaseKey}");
            }

            return new TaskListBridgePromptDisplayState(
                true,
                _taskListBridgePromptText,
                $"day1:bridge-prompt:{currentPhaseKey}:{_taskListBridgePromptText}");
        }

        private string GetCurrentPromptPhaseKey()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null ? storyManager.CurrentPhase.ToString() : string.Empty;
        }

        private static bool IsBridgePromptRedundant(PromptCardModel model, string bridgePromptText)
        {
            if (model == null || string.IsNullOrWhiteSpace(bridgePromptText))
            {
                return false;
            }

            string normalizedBridgePrompt = NormalizePromptComparisonText(bridgePromptText);
            if (string.IsNullOrEmpty(normalizedBridgePrompt))
            {
                return true;
            }

            if (ContainsPromptOverlap(model.FocusText, normalizedBridgePrompt)
                || ContainsPromptOverlap(model.Subtitle, normalizedBridgePrompt)
                || ContainsPromptOverlap(model.FooterText, normalizedBridgePrompt))
            {
                return true;
            }

            if (model.Items == null)
            {
                return false;
            }

            for (int index = 0; index < model.Items.Length; index++)
            {
                PromptTaskItem item = model.Items[index];
                if (ContainsPromptOverlap(item.Label, normalizedBridgePrompt)
                    || ContainsPromptOverlap(item.Detail, normalizedBridgePrompt))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsPromptOverlap(string source, string normalizedNeedle)
        {
            if (string.IsNullOrEmpty(normalizedNeedle))
            {
                return true;
            }

            string normalizedSource = NormalizePromptComparisonText(source);
            if (string.IsNullOrEmpty(normalizedSource))
            {
                return false;
            }

            if (normalizedSource.IndexOf(normalizedNeedle, StringComparison.Ordinal) >= 0
                || normalizedNeedle.IndexOf(normalizedSource, StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return CalculatePromptOverlapScore(normalizedSource, normalizedNeedle) >= 0.58f;
        }

        private static float CalculatePromptOverlapScore(string normalizedSource, string normalizedNeedle)
        {
            HashSet<string> sourceNgrams = BuildPromptNgrams(normalizedSource);
            HashSet<string> needleNgrams = BuildPromptNgrams(normalizedNeedle);
            if (sourceNgrams.Count == 0 || needleNgrams.Count == 0)
            {
                return 0f;
            }

            HashSet<string> smaller = sourceNgrams.Count <= needleNgrams.Count ? sourceNgrams : needleNgrams;
            HashSet<string> larger = ReferenceEquals(smaller, sourceNgrams) ? needleNgrams : sourceNgrams;
            int overlapCount = 0;
            foreach (string gram in smaller)
            {
                if (larger.Contains(gram))
                {
                    overlapCount++;
                }
            }

            return overlapCount / (float)Mathf.Max(1, smaller.Count);
        }

        private static HashSet<string> BuildPromptNgrams(string normalizedText)
        {
            HashSet<string> grams = new HashSet<string>(StringComparer.Ordinal);
            if (string.IsNullOrEmpty(normalizedText))
            {
                return grams;
            }

            if (normalizedText.Length == 1)
            {
                grams.Add(normalizedText);
                return grams;
            }

            for (int index = 0; index < normalizedText.Length - 1; index++)
            {
                grams.Add(normalizedText.Substring(index, 2));
            }

            return grams;
        }

        private static string NormalizePromptComparisonText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(text.Length);
            for (int index = 0; index < text.Length; index++)
            {
                char character = text[index];
                if (char.IsWhiteSpace(character) || char.IsPunctuation(character))
                {
                    continue;
                }

                builder.Append(char.ToLowerInvariant(character));
            }

            return builder.ToString();
        }

        public string GetCurrentBeatKey()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return string.Empty;
            }

            StoryPhase phase = storyManager.CurrentPhase;
            return phase switch
            {
                StoryPhase.CrashAndMeet => IsDialogueSequenceCurrentlyActive(FirstFollowupSequenceId) || HasCompletedDialogueSequence(FirstSequenceId)
                    ? SpringDay1DirectorBeatKeys.CrashAndMeetEscape
                    : SpringDay1DirectorBeatKeys.CrashAndMeetAwake,
                StoryPhase.EnterVillage => ShouldUseEnterVillageHouseArrivalBeat()
                    ? SpringDay1DirectorBeatKeys.EnterVillageHouseArrival
                    : SpringDay1DirectorBeatKeys.EnterVillagePostEntry,
                StoryPhase.HealingAndHP => SpringDay1DirectorBeatKeys.HealingAndHpTreatment,
                StoryPhase.WorkbenchFlashback => SpringDay1DirectorBeatKeys.WorkbenchFlashbackRecall,
                StoryPhase.FarmingTutorial => SpringDay1DirectorBeatKeys.FarmingTutorialFieldwork,
                StoryPhase.DinnerConflict => SpringDay1DirectorBeatKeys.DinnerConflictTable,
                StoryPhase.ReturnAndReminder => SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack,
                StoryPhase.FreeTime => SpringDay1DirectorBeatKeys.FreeTimeNightWitness,
                StoryPhase.DayEnd => _dayEnded
                    ? SpringDay1DirectorBeatKeys.DailyStandPreview
                    : SpringDay1DirectorBeatKeys.DayEndSettle,
                _ => string.Empty
            };
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
            if (phase != StoryPhase.DayEnd)
            {
                _dayEndTaskCardAutoHideArmed = false;
                _dayEndTaskCardShownAt = -1f;
            }

            if (phase == StoryPhase.DayEnd && ShouldAutoHideDayEndTaskCard())
            {
                return null;
            }

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
                StoryPhase.EnterVillage => "进村后先撑过围观，再切到 Primary 站稳，随后才进入疗伤。",
                StoryPhase.HealingAndHP => "艾拉会先把伤势和 HP 一起稳住，村长也会在这里把“先救人、后追问”钉死。",
                StoryPhase.WorkbenchFlashback => "工作台不是单纯教学入口，而是你第一次把“活下去的手艺”重新抓回手里。",
                StoryPhase.FarmingTutorial when IsPostTutorialExploreWindowActive() => "白天该学的活已经收住了，接下来可以在村里和住处之间自由活动；真不想等到天黑，就回村找村长把晚饭直接开起来。",
                StoryPhase.FarmingTutorial when IsAwaitingPostTutorialChiefWrap() => "白天这套农田和工作台已经过线，但剧情还没收口；你得先去和村长说一声，今晚的节奏才会从教学切进傍晚。",
                StoryPhase.FarmingTutorial => "今天不是在学系统，而是在把开垦、播种、浇水、收木、制作全都变成留下来的本事。",
                StoryPhase.DinnerConflict => "晚餐桌边的敌意会第一次正面落到你身上，卡尔也会把村里的怀疑说破。",
                StoryPhase.ReturnAndReminder => "归途这段会把回屋睡觉、两点规则和夜里这片地的不对劲一起压下来。",
                StoryPhase.FreeTime => "自由活动不是空窗，它是 Day1 尾声：村里先把你留在了这里，却还没把你算成自己人。",
                StoryPhase.DayEnd => "今晚只是先活下来；明天还得靠下地和做活继续把自己留在村里。",
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
                StoryPhase.EnterVillage when IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId) => "先在村里这边站稳，等艾拉正式接手疗伤。",
                StoryPhase.EnterVillage when IsTownHouseLeadPending() && _townHouseLeadWaitingForPlayer => "别掉队，村长和艾拉会停下来等你一起进村里。",
                StoryPhase.EnterVillage when IsTownHouseLeadPending() && _townHouseLeadTransitionQueued => "Primary 承接马上接上，先跟住村长。",
                StoryPhase.EnterVillage when IsTownHouseLeadPending() => "跟住村长和艾拉往住处走，靠近入口就会切到 Primary。",
                StoryPhase.EnterVillage when HasCompletedDialogueSequence(VillageGateSequenceId) => "村口已经让开，接下来先在村里这边站稳。",
                StoryPhase.EnterVillage when IsDialogueSequenceCurrentlyActive(VillageGateSequenceId) => "别在围观里停住，跟着村长穿过村口。",
                StoryPhase.EnterVillage => "跟着村长和艾拉进村，先撑过村口的围观和注视。",
                StoryPhase.HealingAndHP when _healingBridgePending && !_healingBridgeSequenceQueued => "先走到艾拉身边，再让疗伤对白和 HP 卡片接管。",
                StoryPhase.HealingAndHP => "等待疗伤对白与 HP 卡片完整播完。",
                StoryPhase.WorkbenchFlashback when _workbenchOpened => "等待工作台回忆完整播完。",
                StoryPhase.WorkbenchFlashback => "靠近 Anvil_0，按 E 打开工作台。",
                StoryPhase.FarmingTutorial when IsPostTutorialExploreWindowActive() => "今晚可以继续探索；如果不想等到晚上，回 Town 和村长聊聊就能直接开饭。",
                StoryPhase.FarmingTutorial when IsAwaitingPostTutorialChiefWrap() => "先去和村长说一声，把白天这套活收住，傍晚的自由活动才会正式打开。",
                StoryPhase.FarmingTutorial when !_tillObjectiveCompleted => GetFarmingTutorialTillPromptText(),
                StoryPhase.FarmingTutorial when !_plantObjectiveCompleted => GetFarmingTutorialPlantPromptText(),
                StoryPhase.FarmingTutorial when !_waterObjectiveCompleted => "保持放置模式开启，完成一次浇水，让作物稳稳进入下一步。",
                StoryPhase.FarmingTutorial when !_woodObjectiveCompleted => $"继续收木材，还差 {Mathf.Max(0, requiredWoodCollectedCount - GetCollectedWoodProgress())} 份。",
                StoryPhase.FarmingTutorial when _workbenchCraftingActive => "守在工作台旁，等当前制作完成。",
                StoryPhase.FarmingTutorial when _craftedCount < requiredCraftedCount => "回到工作台，完成一次真正的基础制作。",
                StoryPhase.FarmingTutorial => "农田与制作目标都已完成，先去和村长说一声。",
                StoryPhase.DinnerConflict when IsReturnToTownEscortPending() && _returnEscortWaitingForPlayer => "别掉队，村长和艾拉会停下来等你回村。",
                StoryPhase.DinnerConflict when IsReturnToTownEscortPending() && _returnEscortTransitionQueued => "快跟着一起进村，晚饭那一桌马上接管。",
                StoryPhase.DinnerConflict when IsReturnToTownEscortPending() => "跟住村长和艾拉回村，晚饭那一桌很快就会接管。",
                StoryPhase.DinnerConflict => IsDialogueSequenceCurrentlyActive(DinnerSequenceId)
                    ? "先把这顿饭吃完，也把卡尔那股敌意听到底。"
                    : "晚餐冲突会先接管，别在这时被别的交互带跑。",
                StoryPhase.ReturnAndReminder => IsDialogueSequenceCurrentlyActive(ReminderSequenceId)
                    ? "先听村长把回屋和两点规矩说死。"
                    : "先接住归途提醒，别在夜里规矩落下前乱走。",
                StoryPhase.FreeTime when IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId) => "先听一圈村里夜里的动静，再决定现在回不回屋。",
                StoryPhase.FreeTime when !_freeTimeIntroCompleted => "自由时段见闻会先接管，先别急着立刻睡。",
                StoryPhase.FreeTime => GetFreeTimeFocusText(),
                StoryPhase.DayEnd => "这一夜总算先熬过去了。天亮以后，再让他们看清你值不值得留下。",
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
                        "跟着村长和艾拉进入村子",
                        HasVillageGateCompleted()
                            ? "已撑过村口围观。"
                            : "先让村长和艾拉带路，别在陌生人的注视里乱跑。",
                        HasVillageGateCompleted()),
                    new PromptTaskItem(
                        "在村里这边先站稳",
                        HasHouseArrivalProgressed()
                            ? "已完成，等待艾拉接手疗伤。"
                            : IsTownHouseLeadPending()
                                ? _townHouseLeadWaitingForPlayer
                                    ? "别掉队，村长和艾拉会停下来等你一起进村里。"
                                    : "跟住村长和艾拉走到入口，靠近后就会切到 Primary 承接。"
                                : "先让 Primary 这边把你和村长、艾拉重新站稳。",
                        HasHouseArrivalProgressed())
                };
            }

            if (phase == StoryPhase.HealingAndHP)
            {
                bool hpVisible = HealthSystem.Instance != null && HealthSystem.Instance.IsVisible;
                if (_healingBridgePending && !_healingBridgeSequenceQueued)
                {
                    return new[]
                    {
                        new PromptTaskItem("走到艾拉身边", "你靠近她之后，疗伤对白和 HP 卡片才会正式接起来。", false),
                        new PromptTaskItem("显示 HP 卡片", "艾拉会先替你稳住伤势，随后 HP 才会真正显形。", false)
                    };
                }

                return new[]
                {
                    !_healingStarted
                        ? new PromptTaskItem("进入疗伤流程", "等待进村安置链收束，再由艾拉正式接手。", false)
                        : !hpVisible
                            ? new PromptTaskItem("显示 HP 卡片", "艾拉会先替你稳住伤势，随后 HP 才会真正显形。", false)
                            : new PromptTaskItem("播完疗伤对白", _healingSequencePlayed ? "疗伤对白已完整结束。" : "疗伤对白会把“先救人、后追问”和你还能撑到哪一步一起说清。", _healingSequencePlayed)
                };
            }

            if (phase == StoryPhase.WorkbenchFlashback)
            {
                return BuildWorkbenchFlashbackPromptItems();
            }

            if (phase == StoryPhase.FarmingTutorial)
            {
                if (IsPostTutorialExploreWindowActive())
                {
                    return new[]
                    {
                        new PromptTaskItem(
                            "自由活动直到晚饭",
                            "Primary、Town、Home 现在都可以去；七点一到会自动切进晚饭。",
                            false),
                        new PromptTaskItem(
                            "想直接开饭就回村找村长",
                            "如果不想继续等到天黑，回 Town 和村长聊一声就会直接开饭。",
                            false)
                    };
                }

                if (IsAwaitingPostTutorialChiefWrap())
                {
                    return new[]
                    {
                        new PromptTaskItem(
                            "去和村长说一声",
                            "白天的农田和工作台已经完成，先把村长这句收口对白接起来。",
                            false),
                        new PromptTaskItem(
                            "收口后进入傍晚自由活动",
                            "这句对白结束后，Primary 的 001/002 会退出，只留 Town 那边的原生 resident 承接后续。",
                            false)
                    };
                }

                return BuildFarmingTutorialPromptItems();
            }

            if (phase == StoryPhase.DinnerConflict)
            {
                return new[]
                {
                    new PromptTaskItem(
                        IsReturnToTownEscortPending() ? "跟着村长和艾拉回村" : "观看晚餐事件",
                        IsReturnToTownEscortPending()
                            ? _returnEscortWaitingForPlayer
                                ? "别掉队，村长和艾拉会停下来等你回村。"
                                : _returnEscortTransitionQueued
                                    ? "回村切场已开始，晚餐桌边马上就会接管。"
                                    : "农田刚收束，先跟着他们回村里坐上晚饭那一桌。"
                            : IsDialogueSequenceCurrentlyActive(DinnerSequenceId)
                                ? "晚餐桌边的敌意正在摊开。"
                                : _dinnerSequencePlayed
                                    ? "晚餐冲突已排队，准备由正式对白接管。"
                                    : "等待卡尔把村里对外来者的怀疑当面说破。",
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
                            ? "回屋与两点规则提醒正在落下。"
                            : _returnSequencePlayed
                                ? "归途提醒已排队，回屋和两点规矩马上就会压下来。"
                                : "等待村长把今晚最后的规矩交代清楚，再决定还能不能继续留在外面。",
                        false)
                };
            }

            if (phase == StoryPhase.FreeTime)
            {
                return new[]
                {
                    !_freeTimeIntroCompleted
                        ? new PromptTaskItem(
                            "听完村里夜间见闻",
                            IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId)
                                ? "老乔治、老汤姆、老杰克和村里的余光，会先把今晚最后一点余波说完。"
                                : "自由时段见闻已排队，先让这段夜里的动静完整接管。",
                            false)
                        : new PromptTaskItem(
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
                    new PromptTaskItem("熬过第一夜", "今晚总算先熬过去了，但明天开始还得继续用下地和做活证明自己。", true)
                };
            }

            return new[]
            {
                new PromptTaskItem("等待剧情推进", "当前还没有激活新的 Day1 任务。", false)
            };
        }

        private bool AreFarmingTutorialObjectivesComplete()
        {
            return _tillObjectiveCompleted
                && _plantObjectiveCompleted
                && _waterObjectiveCompleted
                && _woodObjectiveCompleted
                && _craftedCount >= requiredCraftedCount;
        }

        private bool IsAwaitingPostTutorialChiefWrap()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.FarmingTutorial
                && AreFarmingTutorialObjectivesComplete()
                && !_postTutorialExploreWindowEntered;
        }

        private bool IsPostTutorialExploreWindowActive()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.FarmingTutorial
                && _postTutorialExploreWindowEntered
                && !_dayEnded;
        }

        private void TryHandlePostTutorialExploreWindow()
        {
            if (!IsStoryRuntimeSceneActive() || StoryManager.Instance == null || StoryManager.Instance.CurrentPhase != StoryPhase.FarmingTutorial)
            {
                return;
            }

            if (_postTutorialExploreWindowPending && CanEnterPostTutorialExploreWindowNow())
            {
                EnterPostTutorialExploreWindow();
                return;
            }

            if (IsPostTutorialExploreWindowActive())
            {
                TimeManager timeManager = TimeManager.Instance;
                if (timeManager != null && timeManager.GetHour() >= DinnerReturnHour)
                {
                    TryRequestDinnerGatheringStart(autoTriggered: true);
                    return;
                }

                SpringDay1PromptOverlay.Instance.Show(PostTutorialExplorePromptText);
                return;
            }

            if (IsAwaitingPostTutorialChiefWrap())
            {
                SpringDay1PromptOverlay.Instance.Show(PostTutorialWrapPromptText);
            }
        }

        private bool TryStartPostTutorialWrapSequence()
        {
            if (HasCompletedDialogueSequence(PostTutorialWrapSequenceId))
            {
                RequestPostTutorialExploreWindowEntry();
                return true;
            }

            if (!ShouldQueueDialogueSequence(ref _postTutorialWrapSequenceQueued, PostTutorialWrapSequenceId))
            {
                return IsDialogueSequenceCurrentlyActive(PostTutorialWrapSequenceId);
            }

            SpringDay1PromptOverlay.Instance.Hide();
            PlayDialogueNowOrQueue(BuildPostTutorialWrapSequence());
            return true;
        }

        private void EnterPostTutorialExploreWindow()
        {
            if (_postTutorialExploreWindowEntered)
            {
                return;
            }

            _postTutorialExploreWindowPending = false;
            _postTutorialExploreWindowEarliestEnterAt = -1f;

            void ApplyExploreWindowState()
            {
                _postTutorialExploreWindowEntered = true;
                _dinnerGatheringRequested = false;
                _pendingDinnerTownLoad = false;
                EnsureStoryHourAtLeast(TutorialTimeCapHour);
                UpdateSceneStoryNpcVisibility();
                SpringDay1PromptOverlay.Instance.Show(PostTutorialExplorePromptText);
            }

            if (IsPrimarySceneActive() && !SceneTransitionRunner.IsBusy)
            {
                if (SceneTransitionRunner.TryBlink(
                        ApplyExploreWindowState,
                        storyBlinkFadeOutDuration,
                        storyBlinkHoldDuration,
                        storyBlinkFadeInDuration))
                {
                    return;
                }
            }

            ApplyExploreWindowState();
        }

        private void RequestPostTutorialExploreWindowEntry()
        {
            if (_postTutorialExploreWindowEntered)
            {
                return;
            }

            if (!CanEnterPostTutorialExploreWindowNow())
            {
                _postTutorialExploreWindowPending = true;
                return;
            }

            EnterPostTutorialExploreWindow();
        }

        private bool CanEnterPostTutorialExploreWindowNow()
        {
            if (_postTutorialExploreWindowEntered)
            {
                return false;
            }

            if (_postTutorialExploreWindowEarliestEnterAt > 0f && Time.unscaledTime < _postTutorialExploreWindowEarliestEnterAt)
            {
                return false;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return false;
            }

            GameInputManager inputManager = GameInputManager.Instance;
            if (inputManager != null && !inputManager.IsInputEnabledForDebug)
            {
                return false;
            }

            return true;
        }

        private bool TryRequestDinnerGatheringStart(bool autoTriggered)
        {
            if (_dinnerGatheringRequested || !IsPostTutorialExploreWindowActive())
            {
                return false;
            }

            if (SceneTransitionRunner.IsBusy)
            {
                return false;
            }

            _dinnerGatheringRequested = true;
            _pendingDinnerTownLoad = !IsTownSceneActive();

            bool blinkStarted = SceneTransitionRunner.TryBlink(
                () =>
                {
                    if (_pendingDinnerTownLoad)
                    {
                        SceneManager.LoadScene(TownSceneName, LoadSceneMode.Single);
                        return;
                    }

                    ActivateDinnerGatheringOnTownScene();
                },
                storyBlinkFadeOutDuration,
                storyBlinkHoldDuration,
                storyBlinkFadeInDuration);

            if (!blinkStarted)
            {
                _dinnerGatheringRequested = false;
                _pendingDinnerTownLoad = false;
                return false;
            }

            if (!autoTriggered)
            {
                SpringDay1PromptOverlay.Instance.Hide();
            }

            return true;
        }

        private void ActivateDinnerGatheringOnTownScene()
        {
            _pendingDinnerTownLoad = false;
            ResetDinnerCueSettlementState();
            if (!IsTownSceneActive())
            {
                _dinnerGatheringRequested = false;
                return;
            }

            AlignTownDinnerGatheringActorsAndPlayer();
            EnsureStoryTimeAtLeast(DinnerReturnHour, DinnerReturnMinute);
            StoryManager.Instance.SetPhase(StoryPhase.DinnerConflict);
            UpdateSceneStoryNpcVisibility();
            SpringDay1PromptOverlay.Instance.Hide();
            BeginDinnerConflict();
        }

        private void AlignTownDinnerGatheringActorsAndPlayer()
        {
            MovePlayerToExactPosition(DinnerGatheringTownPlayerPosition);
            AlignTownDinnerGatheringStoryActors();
        }

        private void AlignTownDinnerGatheringStoryActors()
        {
            Transform chief = ResolveStoryChiefTransform();
            Transform companion = ResolveStoryCompanionTransform();
            Vector2 lookTarget = new Vector2(DinnerGatheringTownPlayerPosition.x, DinnerGatheringTownPlayerPosition.y);

            if (TryResolveDinnerGatheringActorTarget(isChief: true, chief, out Vector3 chiefTarget))
            {
                ReframeStoryActor(chief, chiefTarget, lookTarget);
            }

            if (TryResolveDinnerGatheringActorTarget(isChief: false, companion, out Vector3 companionTarget))
            {
                ReframeStoryActor(companion, companionTarget, lookTarget);
            }
        }

        private static bool TryResolveDinnerGatheringActorTarget(bool isChief, Transform actor, out Vector3 targetPosition)
        {
            Vector2 fallbackPoint = new Vector2(DinnerGatheringTownPlayerPosition.x, DinnerGatheringTownPlayerPosition.y)
                + (isChief ? DinnerGatheringChiefFallbackOffset : DinnerGatheringCompanionFallbackOffset);
            targetPosition = BuildStoryActorReframeTarget(fallbackPoint, actor);

            Transform dinnerAnchor = FindPreferredObjectTransform(PreferredDinnerGatheringAnchorObjectNames);
            if (dinnerAnchor == null)
            {
                return actor != null;
            }

            Vector2 anchorPoint = dinnerAnchor.position;
            Vector2 anchorOffset = isChief ? DinnerGatheringChiefAnchorOffset : DinnerGatheringCompanionAnchorOffset;
            targetPosition = BuildStoryActorReframeTarget(anchorPoint + anchorOffset, actor);
            return actor != null;
        }

        private static bool TryResolveVillageCrowdMarker(string npcId, bool preferStart, out Transform marker)
        {
            string preferredSuffixCn = preferStart ? "起点" : "终点";
            string preferredSuffixEn = preferStart ? "Start" : "End";
            string fallbackSuffixCn = preferStart ? "终点" : "起点";
            string fallbackSuffixEn = preferStart ? "End" : "Start";

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}{preferredSuffixCn}");
            if (marker != null)
            {
                return true;
            }

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}{preferredSuffixEn}");
            if (marker != null)
            {
                return true;
            }

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}{fallbackSuffixCn}");
            if (marker != null)
            {
                return true;
            }

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}{fallbackSuffixEn}");
            return marker != null;
        }

        private void MovePlayerToExactPosition(Vector3 targetPosition)
        {
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                return;
            }

            _playerMovement.StopMovement();
            Vector3 resolvedPosition = targetPosition;
            resolvedPosition.z = targetPosition.z;

            Rigidbody2D playerBody = _playerMovement.GetComponent<Rigidbody2D>();
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector2.zero;
                playerBody.position = new Vector2(resolvedPosition.x, resolvedPosition.y);
            }
            else
            {
                playerTransform.position = resolvedPosition;
            }

            Physics2D.SyncTransforms();
        }

        private PromptTaskItem[] BuildWorkbenchFlashbackPromptItems()
        {
            if (!HasWorkbenchBriefingCompleted())
            {
                return new[]
                {
                    new PromptTaskItem(
                        "先在工作台边听村长交代",
                        "站到工作台边上，先把台子和旁边补给箱的交代听完。",
                        false),
                    new PromptTaskItem(
                        "等交代完再按 E",
                        "这段交代说完后，工作台交互才会正式打开。",
                        false)
                };
            }

            return new[]
            {
                new PromptTaskItem(
                    "靠近工作台并按 E",
                    _workbenchOpened ? "工作台已经打开。" : "从交互包络线进入后按 E，让这张旧台子把记忆拽出来。",
                    _workbenchOpened),
                new PromptTaskItem(
                    "看完工作台回忆",
                    _workbenchSequencePlayed ? "工作台回忆已播完。" : "打开后会自动推进你对手艺的第一段正式回忆。",
                    _workbenchSequencePlayed)
            };
        }

        private string GetFarmingTutorialTillPromptText()
        {
            string placementGuidance = GetPlacementModeGuidanceText();
            return string.IsNullOrWhiteSpace(placementGuidance)
                ? "先用锄头开垦一格土地。"
                : placementGuidance;
        }

        private string GetFarmingTutorialPlantPromptText()
        {
            if (!_plantObjectiveCompleted)
            {
                return "保持放置模式开启，把花椰菜种子种进刚开垦的土地。";
            }

            return "把花椰菜种子种进刚开垦的土地。";
        }

        private string GetFarmingTutorialWaterPromptText()
        {
            if (!_waterObjectiveCompleted)
            {
                return "保持放置模式开启，用浇水壶给作物浇水。";
            }

            return "用浇水壶给作物浇水。";
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
                        : $"先按 V 开启放置模式，再开垦出第一格教学土地 {GetLatchedProgress(_tillObjectiveCompleted, requiredTilledCount)}/{requiredTilledCount}",
                    _tillObjectiveCompleted),
                new PromptTaskItem(
                    "完成播种",
                    _plantObjectiveCompleted
                        ? $"已完成 {requiredPlantedCount}/{requiredPlantedCount}"
                        : !_tillObjectiveCompleted
                            ? "先按 V 开启放置模式完成开垦，再把花椰菜种子种下。"
                            : $"保持放置模式开启，再把花椰菜种子种下 {GetLatchedProgress(_plantObjectiveCompleted, requiredPlantedCount)}/{requiredPlantedCount}",
                    _plantObjectiveCompleted),
                new PromptTaskItem(
                    "完成浇水",
                    _waterObjectiveCompleted
                        ? $"已完成 {requiredWateredCount}/{requiredWateredCount}"
                        : !_plantObjectiveCompleted
                            ? "播种后保持放置模式开启，再用浇水壶推进下一步。"
                            : $"保持放置模式开启，再完成浇水 {GetLatchedProgress(_waterObjectiveCompleted, requiredWateredCount)}/{requiredWateredCount}",
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
            if (!IsStoryRuntimeSceneActive())
            {
                return;
            }

            SyncStoryTimePauseState();
            ResyncLowEnergyState(false);
            EnforceDay1TimeGuardrails();

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

            ResetTownHouseLeadState();

            if (evt.CurrentPhase == StoryPhase.FarmingTutorial)
            {
                _postTutorialWrapSequenceQueued = false;
                _postTutorialExploreWindowEntered = false;
                _dinnerGatheringRequested = false;
                _pendingDinnerTownLoad = false;
                ResetDinnerCueSettlementState();
                InitializeFarmingTutorialTracking(true);
            }
        }

        private void HandleDialogueSequenceCompleted(DialogueSequenceCompletedEvent evt)
        {
            if (!IsStoryRuntimeSceneActive() || evt == null)
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

            if (evt.SequenceId == FirstFollowupSequenceId)
            {
                if (HasPlayableNodes(evt.FollowupSequence))
                {
                    return;
                }

                BeginHealingAndHp();
                return;
            }

            if (evt.SequenceId == VillageGateSequenceId)
            {
                _villageGateSequencePlayed = true;
                SpringDay1NpcCrowdDirector.ForceImmediateSync();
                if (IsPrimarySceneActive())
                {
                    ResetTownHouseLeadState();
                    TryQueuePrimaryHouseArrival();
                }
                else
                {
                    TryBeginTownHouseLead();
                }

                return;
            }

            if (evt.SequenceId == HouseArrivalSequenceId)
            {
                _houseArrivalSequencePlayed = true;
                ResetTownHouseLeadState();
                BeginHealingAndHp();
                return;
            }

            if (evt.SequenceId == HealingSequenceId)
            {
                StoryManager.Instance.SetPhase(StoryPhase.WorkbenchFlashback);
                SpringDay1PromptOverlay.Instance.Show(WorkbenchEscortPromptText);
                return;
            }

            if (evt.SequenceId == WorkbenchBriefingSequenceId)
            {
                SpringDay1PromptOverlay.Instance.Show(WorkbenchEscortReadyPromptText);
                return;
            }

            if (evt.SequenceId == WorkbenchSequenceId)
            {
                StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);
                InitializeFarmingTutorialTracking(true);
                _postTutorialWrapSequenceQueued = false;
                _postTutorialExploreWindowEntered = false;
                _dinnerGatheringRequested = false;
                _pendingDinnerTownLoad = false;
                ResetDinnerCueSettlementState();
                SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialTillPromptText());
                return;
            }

            if (evt.SequenceId == PostTutorialWrapSequenceId)
            {
                RequestPostTutorialExploreWindowEntry();
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
                return;
            }

            if (evt.SequenceId == FreeTimeIntroSequenceId)
            {
                CompleteFreeTimeIntro();
            }
        }

        private void HandleDialogueEnded(DialogueEndEvent evt)
        {
            if (!_postTutorialExploreWindowPending
                || _postTutorialExploreWindowEntered
                || evt == null
                || !string.Equals(evt.SequenceId, PostTutorialWrapSequenceId, StringComparison.Ordinal))
            {
                return;
            }

            _postTutorialExploreWindowEarliestEnterAt = Time.unscaledTime + 0.05f;
            RequestPostTutorialExploreWindowEntry();
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

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (string.Equals(scene.name, PrimarySceneName, StringComparison.Ordinal)
                || string.Equals(scene.name, TownSceneName, StringComparison.Ordinal)
                || string.Equals(scene.name, HomeSceneName, StringComparison.Ordinal))
            {
                EnforceDay1TimeGuardrails();
                if (_pendingForcedSleepRestPlacementFrames > 0)
                {
                    TryPlacePlayerNearCurrentSceneRestTarget();
                }
            }

            if (!_pendingDinnerTownLoad || !string.Equals(scene.name, TownSceneName, StringComparison.Ordinal))
            {
                return;
            }

            ActivateDinnerGatheringOnTownScene();
        }

        private void HandleHourChanged(int hour)
        {
            if (_suppressDay1TimeGuardrail)
            {
                return;
            }

            EnforceDay1TimeGuardrails();

            if (_dayEnded || StoryManager.Instance == null)
            {
                return;
            }

            SyncStoryActorNightRestSchedule();

            if (IsPostTutorialExploreWindowActive() && hour >= DinnerReturnHour)
            {
                TryRequestDinnerGatheringStart(autoTriggered: true);
                return;
            }

            if (StoryManager.Instance.CurrentPhase == StoryPhase.FreeTime
                && _freeTimeEntered
                && _freeTimeIntroCompleted
                && hour >= 26)
            {
                TimeManager.Instance?.Sleep();
                if (!_dayEnded)
                {
                    HandleSleep();
                }
                return;
            }

            if (StoryManager.Instance.CurrentPhase != StoryPhase.FreeTime || !_freeTimeIntroCompleted)
            {
                return;
            }

            ShowFreeTimePressurePromptForHour(hour);
        }

        private void HandleSleep()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (!ShouldHandleSpringDay1SleepResolution(storyManager))
            {
                HandleGenericForcedSleepFallback();
                return;
            }

            if (_dayEnded)
            {
                HandleGenericForcedSleepFallback();
                return;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            if (!CanFinalizeDayEndFromCurrentState(currentPhase))
            {
                RecoverFromInvalidEarlySleep(currentPhase);
                return;
            }

            SpringDay1PromptOverlay.Instance.Hide();
            bool canUseSceneBlink = Application.isPlaying;

            void FinalizeSleepSceneState()
            {
                if (!IsHomeSceneActive())
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.OpenScene(HomeSceneAssetPath, OpenSceneMode.Single);
                    }
                    else
#endif
                    {
                        SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
                    }
                }

                TryPlacePlayerNearCurrentSceneRestTarget();
                TrySnapResidentsToHomeAnchorsForDayEnd();
                TrySnapStoryActorsToHomeAnchorsForDayEnd();
                ArmForcedSleepRestPlacementRetries();
            }

            if (canUseSceneBlink)
            {
                bool blinkStarted = SceneTransitionRunner.TryBlink(
                    FinalizeSleepSceneState,
                    storyBlinkFadeOutDuration,
                    storyBlinkHoldDuration,
                    storyBlinkFadeInDuration);
                if (!blinkStarted)
                {
                    FinalizeSleepSceneState();
                }
            }
            else
            {
                FinalizeSleepSceneState();
            }

            _dayEnded = true;
            _freeTimeEntered = true;
            _freeTimeIntroCompleted = true;
            storyManager.SetPhase(StoryPhase.DayEnd);
            if (EnergySystem.Instance != null)
            {
                EnergySystem.Instance.FullRestore();
                EnergySystem.Instance.SetLowEnergyWarningVisual(false);
            }

            _workbenchCraftingActive = false;
            _workbenchCraftProgress = 0f;
            _workbenchCraftQueueTotal = 0;
            _workbenchCraftQueueCompleted = 0;
            _workbenchCraftRecipeName = string.Empty;
            _freeTimeNightWarningShown = false;
            _freeTimeMidnightWarningShown = false;
            _freeTimeFinalWarningShown = false;
            _dayEndTaskCardAutoHideArmed = true;
            _dayEndTaskCardShownAt = Time.unscaledTime;
            ResyncLowEnergyState(false);
            SyncStoryTimePauseState();
        }

        private bool ShouldHandleSpringDay1SleepResolution(StoryManager storyManager)
        {
            if (storyManager == null || _dayEnded)
            {
                return false;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return false;
            }

            return timeManager.GetYear() == Day1RuntimeManagedYear
                && timeManager.GetSeason() == SeasonManager.Season.Spring
                && timeManager.GetDay() >= Day1RuntimeManagedDay
                && timeManager.GetDay() <= Day1RuntimeManagedDay + 1
                && storyManager.CurrentPhase >= StoryPhase.CrashAndMeet
                && storyManager.CurrentPhase < StoryPhase.DayEnd;
        }

        private void HandleGenericForcedSleepFallback()
        {
            SpringDay1PromptOverlay existingPromptOverlay =
                FindFirstObjectByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include);
            existingPromptOverlay?.Hide();
            ReleaseStoryTimePause();

            void FinalizeSleepSceneState()
            {
                if (!IsHomeSceneActive())
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.OpenScene(HomeSceneAssetPath, OpenSceneMode.Single);
                    }
                    else
#endif
                    {
                    SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
                    }
                }

                TryPlacePlayerNearCurrentSceneRestTarget();
                TrySnapResidentsToHomeAnchorsForDayEnd();
                TrySnapStoryActorsToHomeAnchorsForDayEnd();
                ArmForcedSleepRestPlacementRetries();
            }

            if (Application.isPlaying)
            {
                bool blinkStarted = SceneTransitionRunner.TryBlink(
                    FinalizeSleepSceneState,
                    storyBlinkFadeOutDuration,
                    storyBlinkHoldDuration,
                    storyBlinkFadeInDuration);
                if (!blinkStarted)
                {
                    FinalizeSleepSceneState();
                }

                return;
            }

            FinalizeSleepSceneState();
        }

        private static void TrySnapResidentsToHomeAnchorsForDayEnd()
        {
            MethodInfo snapMethod = typeof(SpringDay1NpcCrowdDirector).GetMethod(
                "SnapResidentsToHomeAnchors",
                BindingFlags.Public | BindingFlags.Static);

            if (snapMethod != null)
            {
                snapMethod.Invoke(null, null);
                return;
            }

            SpringDay1NpcCrowdDirector.EnsureRuntime();
            Debug.LogWarning("[SpringDay1Director] SnapResidentsToHomeAnchors() 不可用，已回退为 EnsureRuntime。");
        }

        private bool CanFinalizeDayEndFromCurrentState(StoryPhase currentPhase)
        {
            return currentPhase == StoryPhase.FreeTime
                && _freeTimeEntered
                && _freeTimeIntroCompleted
                && !_dayEnded;
        }

        private void RecoverFromInvalidEarlySleep(StoryPhase currentPhase)
        {
            if (showDebugLog)
            {
                Debug.Log($"[SpringDay1Director] 已拦截非法跨天：phase={currentPhase}");
            }

            EnforceDay1TimeGuardrails();
            SyncStoryTimePauseState();
        }

        private void TickTownSceneFlow()
        {
            InitializeRuntimeUiIfNeeded();
            SyncStoryTimePauseState();
            UpdateSceneStoryNpcVisibility();
            SyncStoryActorNightRestSchedule();
            TryAdoptTownOpeningState();
            TryHandleTownEnterVillageFlow();
            MaintainTownVillageGateActorsWhileDialogueActive();

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return;
            }

            if (storyManager.CurrentPhase != StoryPhase.EnterVillage)
            {
                ResetTownHouseLeadState();
            }
            if (storyManager.CurrentPhase != StoryPhase.DinnerConflict)
            {
                ResetDinnerCueSettlementState();
            }

            ResetPrimaryArrivalState();
            ResetHealingBridgeState();
            ResetReturnEscortState();
            ResetWorkbenchEscortState();
            TryHandlePostTutorialExploreWindow();

            if (storyManager.CurrentPhase == StoryPhase.DinnerConflict)
            {
                BeginDinnerConflict();
                return;
            }

            if (storyManager.CurrentPhase == StoryPhase.ReturnAndReminder && !_returnSequencePlayed)
            {
                BeginReturnReminder();
            }
        }

        private void TickPrimarySceneFlow()
        {
            InitializeRuntimeUiIfNeeded();
            ResetTownHouseLeadState();
            RefreshCraftingServiceSubscription();
            RefreshInventoryTrackingSubscription();
            TryAutoBindWorkbenchInteractable();
            TryAutoBindBedInteractable();
            SyncStoryTimePauseState();
            UpdateSceneStoryNpcVisibility();
            SyncStoryActorNightRestSchedule();
            TryApplyDebugWorkbenchSkip();
            TryQueuePrimaryHouseArrival();

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            TryRecoverConsumedSequenceProgression(currentPhase);
            currentPhase = StoryManager.Instance.CurrentPhase;

            if (currentPhase != StoryPhase.EnterVillage)
            {
                ResetPrimaryArrivalState();
            }

            if (currentPhase != StoryPhase.HealingAndHP)
            {
                ResetHealingBridgeState();
            }

            if (currentPhase != StoryPhase.WorkbenchFlashback || _workbenchOpened)
            {
                ResetWorkbenchEscortState();
            }

            if (currentPhase != StoryPhase.DinnerConflict)
            {
                ResetReturnEscortState();
                ResetDinnerCueSettlementState();
            }

            TryHandlePostTutorialExploreWindow();

            if (currentPhase == StoryPhase.HealingAndHP || currentPhase == StoryPhase.WorkbenchFlashback)
            {
                if (currentPhase == StoryPhase.HealingAndHP)
                {
                    TryHandleHealingBridge();
                }
                else
                {
                    TryHandleWorkbenchEscort();
                }

                TryHandleWorkbenchFlashback();
            }

            if (currentPhase == StoryPhase.FarmingTutorial)
            {
                TickFarmingTutorial();
            }

            if (currentPhase == StoryPhase.DinnerConflict)
            {
                BeginDinnerConflict();
            }
        }

        private void TickHomeSceneFlow()
        {
            InitializeRuntimeUiIfNeeded();
            RefreshCraftingServiceSubscription(null);
            RefreshInventoryTrackingSubscription(null);
            TryAutoBindBedInteractable();
            SyncStoryTimePauseState();
            SyncStoryActorNightRestSchedule();
            ResetTownHouseLeadState();
            ResetPrimaryArrivalState();
            ResetHealingBridgeState();
            ResetReturnEscortState();
            ResetWorkbenchEscortState();
            TryHandlePostTutorialExploreWindow();
        }

        private void UpdateSceneStoryNpcVisibility()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            Transform chief = ResolveStoryChiefTransform();
            Transform companion = ResolveStoryCompanionTransform();
            if (IsTownSceneActive())
            {
                SetStoryNpcActiveIfPresent(chief, true);
                SetStoryNpcActiveIfPresent(companion, true);
                bool storyActorMode = ShouldUseTownStoryActorMode(currentPhase);
                bool shouldSuppressRoam = storyActorMode;
                ApplyStoryActorRuntimePolicy(chief, storyActorMode, shouldSuppressRoam, resumeRoamWhenReleased: true);
                ApplyStoryActorRuntimePolicy(companion, storyActorMode, shouldSuppressRoam, resumeRoamWhenReleased: true);
                return;
            }

            if (!IsPrimarySceneActive())
            {
                ApplyStoryActorRuntimePolicy(chief, false, false, resumeRoamWhenReleased: false);
                ApplyStoryActorRuntimePolicy(companion, false, false, resumeRoamWhenReleased: false);
                return;
            }

            bool chiefVisible = ShouldKeepPrimaryChiefVisible(currentPhase);
            bool companionVisible = ShouldKeepPrimaryCompanionVisible(currentPhase);
            SetStoryNpcActiveIfPresent(chief, chiefVisible);
            SetStoryNpcActiveIfPresent(companion, companionVisible);

            bool storyActorModeInPrimary = ShouldUsePrimaryStoryActorMode(currentPhase);
            bool shouldSuppressRoamInPrimary = storyActorModeInPrimary;
            bool resumeRoamWhenReleasedInPrimary = ShouldAllowPrimaryEscortRoam(currentPhase);
            ApplyStoryActorRuntimePolicy(chief, storyActorModeInPrimary && chiefVisible, shouldSuppressRoamInPrimary, resumeRoamWhenReleasedInPrimary);
            ApplyStoryActorRuntimePolicy(companion, storyActorModeInPrimary && companionVisible, shouldSuppressRoamInPrimary, resumeRoamWhenReleasedInPrimary);
        }

        private bool ShouldUseTownStoryActorMode(StoryPhase currentPhase)
        {
            return currentPhase switch
            {
                StoryPhase.EnterVillage => !HasHouseArrivalProgressed(),
                StoryPhase.DinnerConflict => true,
                StoryPhase.ReturnAndReminder => true,
                _ => false
            };
        }

        private bool ShouldUsePrimaryStoryActorMode(StoryPhase currentPhase)
        {
            return currentPhase switch
            {
                StoryPhase.EnterVillage => true,
                StoryPhase.HealingAndHP => true,
                StoryPhase.WorkbenchFlashback => true,
                StoryPhase.FarmingTutorial => !_postTutorialExploreWindowEntered && !AreFarmingTutorialObjectivesComplete(),
                _ => false
            };
        }

        private bool ShouldAllowPrimaryEscortRoam(StoryPhase currentPhase)
        {
            if (currentPhase == StoryPhase.WorkbenchFlashback)
            {
                return !_workbenchOpened;
            }

            return currentPhase == StoryPhase.DinnerConflict && IsReturnToTownEscortPending();
        }

        private void ApplyStoryActorRuntimePolicy(Transform actor, bool storyActorMode, bool suppressRoam, bool resumeRoamWhenReleased)
        {
            if (actor == null)
            {
                return;
            }

            NPCDialogueInteractable formalDialogue = actor.GetComponent<NPCDialogueInteractable>();
            NPCInformalChatInteractable informalDialogue = actor.GetComponent<NPCInformalChatInteractable>();
            NPCAutoRoamController roamController = actor.GetComponent<NPCAutoRoamController>();

            if (storyActorMode)
            {
                if (formalDialogue != null && formalDialogue.enabled)
                {
                    formalDialogue.enabled = false;
                }

                if (informalDialogue != null && informalDialogue.enabled)
                {
                    informalDialogue.enabled = false;
                }

                if (roamController != null)
                {
                    roamController.AcquireResidentScriptedControl(DirectorResidentControlOwnerKey, resumeRoamWhenReleased);
                }

                ResolveNpcBubblePresenter(actor)?.HideBubble();
                return;
            }

            if (formalDialogue != null && !formalDialogue.enabled)
            {
                formalDialogue.enabled = true;
            }

            if (informalDialogue != null && !informalDialogue.enabled)
            {
                informalDialogue.enabled = true;
            }

            if (roamController != null && ShouldKeepStoryActorNightRestControl(actor))
            {
                return;
            }

            if (roamController != null)
            {
                roamController.ReleaseResidentScriptedControl(DirectorResidentControlOwnerKey, resumeRoamWhenReleased);
            }

            if (roamController != null
                && roamController.isActiveAndEnabled
                && actor.gameObject.activeInHierarchy
                && IsTownSceneActive()
                && !roamController.IsRoaming
                && !roamController.IsResidentScriptedControlActive)
            {
                roamController.StartRoam();
            }
        }

        private bool ShouldKeepStoryActorNightRestControl(Transform actor)
        {
            if (!_storyActorNightRestActive || actor == null)
            {
                return false;
            }

            return ReferenceEquals(actor, ResolveStoryChiefTransform())
                || ReferenceEquals(actor, ResolveStoryCompanionTransform())
                || ReferenceEquals(actor, ResolveStoryThirdResidentTransform());
        }

        private void SyncStoryActorNightRestSchedule()
        {
            if (!ShouldManageStoryActorNightSchedule(out int currentHour))
            {
                ReleaseStoryActorNightRestControlIfNeeded(resumeRoam: true);
                return;
            }

            if (ShouldStoryActorsRestByClock(currentHour))
            {
                ApplyStoryActorNightRestSchedule(forceSnap: true);
                return;
            }

            if (ShouldStoryActorsReturnHomeByClock(currentHour))
            {
                ApplyStoryActorNightRestSchedule(forceSnap: false);
                return;
            }

            ReleaseStoryActorNightRestControlIfNeeded(resumeRoam: true);
        }

        private bool ShouldManageStoryActorNightSchedule(out int currentHour)
        {
            currentHour = 0;
            if (!IsTownSceneActive())
            {
                return false;
            }

            StoryManager storyManager = StoryManager.Instance;
            TimeManager timeManager = TimeManager.Instance;
            if (storyManager == null
                || timeManager == null
                || !ShouldHandleSpringDay1SleepResolution(storyManager))
            {
                return false;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            if (currentPhase != StoryPhase.FreeTime && currentPhase != StoryPhase.DayEnd)
            {
                return false;
            }

            currentHour = timeManager.GetHour();
            return true;
        }

        private static bool ShouldStoryActorsReturnHomeByClock(int currentHour)
        {
            return currentHour >= StoryActorReturnHomeHour && currentHour < StoryActorForcedRestHour;
        }

        private static bool ShouldStoryActorsRestByClock(int currentHour)
        {
            return currentHour >= StoryActorForcedRestHour || currentHour < StoryActorMorningReleaseHour;
        }

        private void ApplyStoryActorNightRestSchedule(bool forceSnap)
        {
            ApplyStoryActorNightRestToActor(ResolveStoryChiefTransform(), forceSnap);
            ApplyStoryActorNightRestToActor(ResolveStoryCompanionTransform(), forceSnap);
            ApplyStoryActorNightRestToActor(ResolveStoryThirdResidentTransform(), forceSnap);
            _storyActorNightRestActive = true;
        }

        private void ApplyStoryActorNightRestToActor(Transform actor, bool forceSnap)
        {
            if (actor == null || !actor.gameObject.activeInHierarchy)
            {
                return;
            }

            NPCAutoRoamController roamController = actor.GetComponent<NPCAutoRoamController>();
            Transform homeAnchor = ResolveStoryActorNightHomeAnchor(actor, roamController);
            if (homeAnchor == null)
            {
                return;
            }

            Vector3 targetPosition = homeAnchor.position;
            targetPosition.z = actor.position.z;

            if (!forceSnap
                && roamController != null
                && Vector2.Distance(actor.position, targetPosition) > 0.05f)
            {
                roamController.DriveResidentScriptedMoveTo(
                    DirectorResidentControlOwnerKey,
                    homeAnchor.position,
                    resumeRoamWhenReleased: true,
                    retargetTolerance: Mathf.Max(escortRetargetTolerance, 0.35f));
                return;
            }

            if (roamController != null)
            {
                roamController.AcquireResidentScriptedControl(DirectorResidentControlOwnerKey, resumeRoamWhenReleased: true);
                roamController.HaltResidentScriptedMovement();
            }

            actor.position = targetPosition;
            actor.GetComponent<NPCMotionController>()?.StopMotion();
            Physics2D.SyncTransforms();
        }

        private static Transform ResolveStoryActorNightHomeAnchor(Transform actor, NPCAutoRoamController roamController)
        {
            if (roamController != null && roamController.HomeAnchor != null)
            {
                return roamController.HomeAnchor;
            }

            if (actor == null)
            {
                return null;
            }

            string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(actor.name);
            string[] preferredAnchorNames = string.IsNullOrWhiteSpace(normalizedNpcId)
                ? new[] { $"{actor.name}_HomeAnchor" }
                : new[] { $"{actor.name}_HomeAnchor", $"{normalizedNpcId}_HomeAnchor", $"NPC{normalizedNpcId}_HomeAnchor" };

            Transform homeAnchor = FindPreferredObjectTransform(preferredAnchorNames);
            if (homeAnchor != null && roamController != null)
            {
                roamController.SetHomeAnchor(homeAnchor);
            }

            return homeAnchor;
        }

        private void ReleaseStoryActorNightRestControlIfNeeded(bool resumeRoam)
        {
            if (!_storyActorNightRestActive)
            {
                return;
            }

            ReleaseStoryActorNightRestControl(ResolveStoryChiefTransform(), resumeRoam);
            ReleaseStoryActorNightRestControl(ResolveStoryCompanionTransform(), resumeRoam);
            ReleaseStoryActorNightRestControl(ResolveStoryThirdResidentTransform(), resumeRoam);
            _storyActorNightRestActive = false;
        }

        private static void ReleaseStoryActorNightRestControl(Transform actor, bool resumeRoam)
        {
            NPCAutoRoamController roamController = actor != null ? actor.GetComponent<NPCAutoRoamController>() : null;
            if (roamController == null || !roamController.IsResidentScriptedControlActive)
            {
                return;
            }

            roamController.ReleaseResidentScriptedControl(DirectorResidentControlOwnerKey, resumeRoam);
        }

        private void TrySnapStoryActorsToHomeAnchorsForDayEnd()
        {
            ApplyStoryActorNightRestSchedule(forceSnap: true);
        }

        private bool ShouldKeepPrimaryChiefVisible(StoryPhase currentPhase)
        {
            if (!IsPrimarySceneActive())
            {
                return false;
            }

            return ShouldKeepPrimaryStoryEscortVisible(currentPhase);
        }

        private bool ShouldKeepPrimaryCompanionVisible(StoryPhase currentPhase)
        {
            return ShouldKeepPrimaryStoryEscortVisible(currentPhase);
        }

        private bool ShouldKeepPrimaryStoryEscortVisible(StoryPhase currentPhase)
        {
            return currentPhase == StoryPhase.EnterVillage
                || currentPhase == StoryPhase.HealingAndHP
                || currentPhase == StoryPhase.WorkbenchFlashback
                || (currentPhase == StoryPhase.FarmingTutorial && !_postTutorialExploreWindowEntered);
        }

        private void TryAdoptTownOpeningState()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.CrashAndMeet)
            {
                return;
            }

            if (!IsTownSceneActive())
            {
                return;
            }

            storyManager.SetLanguageDecoded(true);
            _villageGateSequencePlayed = false;
            _houseArrivalSequencePlayed = false;
            storyManager.SetPhase(StoryPhase.EnterVillage);
            EnsureStoryHourAtLeast(TownOpeningHour);
            SpringDay1PromptOverlay.Instance.Show("跟着村长和艾拉进村，先撑过村口的围观和注视。");
        }

        private void TryHandleTownEnterVillageFlow()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.EnterVillage)
            {
                _townVillageGateCueWaitStartedAt = -1f;
                return;
            }

            EnsureStoryHourAtLeast(TownOpeningHour);

            if (HasHouseArrivalProgressed())
            {
                _houseArrivalSequencePlayed = true;
                _townVillageGateCueWaitStartedAt = -1f;
                ResetTownHouseLeadState();
                return;
            }

            if (HasVillageGateProgressed())
            {
                _villageGateSequencePlayed = true;
                _townVillageGateCueWaitStartedAt = -1f;
                if (!IsDialogueChainStillActive())
                {
                    TryBeginTownHouseLead();
                }

                return;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            SpringDay1NpcCrowdDirector.ForceImmediateSync();
            bool beatSettled = SpringDay1NpcCrowdDirector.IsBeatCueSettled(SpringDay1DirectorBeatKeys.EnterVillagePostEntry);
            if (!beatSettled)
            {
                if (_townVillageGateCueWaitStartedAt < 0f)
                {
                    _townVillageGateCueWaitStartedAt = Time.unscaledTime;
                }

                bool beatReady = string.Equals(
                    GetCurrentBeatKey(),
                    SpringDay1DirectorBeatKeys.EnterVillagePostEntry,
                    StringComparison.OrdinalIgnoreCase);
                bool timedOut = townVillageGateCueSettleTimeout <= 0f
                    || Time.unscaledTime - _townVillageGateCueWaitStartedAt >= townVillageGateCueSettleTimeout;
                if (!beatReady || !timedOut)
                {
                    return;
                }

                if (!SpringDay1NpcCrowdDirector.ForceSettleBeatCue(SpringDay1DirectorBeatKeys.EnterVillagePostEntry))
                {
                    return;
                }

                _townVillageGateCueWaitStartedAt = -1f;
            }
            else
            {
                _townVillageGateCueWaitStartedAt = -1f;
            }

            if (!ShouldQueueDialogueSequence(ref _villageGateSequencePlayed, VillageGateSequenceId))
            {
                return;
            }

            if (!TryPrepareTownVillageGateActors())
            {
                _villageGateSequencePlayed = false;
                return;
            }

            SpringDay1NpcCrowdDirector.ForceImmediateSync();
            _townVillageGateCueWaitStartedAt = -1f;
            PlayDialogueNowOrQueue(BuildVillageGateSequence());
        }

        private void MaintainTownVillageGateActorsWhileDialogueActive()
        {
            if (!ShouldHoldEnterVillageCrowdCue())
            {
                return;
            }

            TryPrepareTownVillageGateActors();
        }

        private void TryBeginTownHouseLead()
        {
            if (!IsTownHouseLeadPending())
            {
                return;
            }

            bool startedNow = !_townHouseLeadStarted;
            if (startedNow)
            {
                _townHouseLeadStarted = true;
                _townHouseLeadTransitionQueued = false;
                _townHouseLeadWaitingForPlayer = false;
                _townHouseLeadNextMoveRetryAt = 0f;
                _townHouseLeadNextBubbleAt = 0f;
                _townHouseLeadCaughtUpSince = 0f;
            }

            UpdateTownHouseLeadSnapshot(
                out Transform chief,
                out NPCAutoRoamController chiefRoam,
                out NPCBubblePresenter chiefBubble,
                out Transform companion,
                out NPCAutoRoamController companionRoam,
                out Transform leadTarget,
                out SceneTransitionTrigger2D transitionTrigger);

            string bridgePromptText = ResolveEscortBridgePromptText(
                chief,
                companion,
                TownLeadBridgePromptText,
                "跟住村长往住处走，别在村口掉队。",
                "跟住艾拉往住处走，别在村口掉队。");

            if (startedNow)
            {
                SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            bool shouldWaitForPlayer = ResolveEscortWaitState(
                ShouldEscortWaitForPlayer(_townHouseLeadChiefDistance, _townHouseLeadPlayerDistance, _townHouseLeadWaitingForPlayer),
                _townHouseLeadWaitingForPlayer,
                ref _townHouseLeadCaughtUpSince);
            if (shouldWaitForPlayer)
            {
                if (!_townHouseLeadWaitingForPlayer)
                {
                    PauseStoryActorNavigation(chiefRoam);
                    PauseStoryActorNavigation(companionRoam);
                    SpringDay1PromptOverlay.Instance.Show(TownLeadWaitPromptText);
                }

                _townHouseLeadWaitingForPlayer = true;
                TryShowEscortWaitBubble(chiefBubble, TownLeadWaitPromptText, ref _townHouseLeadNextBubbleAt);
            }
            else
            {
                if (_townHouseLeadWaitingForPlayer)
                {
                    _townHouseLeadWaitingForPlayer = false;
                    _townHouseLeadCaughtUpSince = 0f;
                    _townHouseLeadNextMoveRetryAt = 0f;
                    ResumeStoryActorNavigation(chiefRoam);
                    ResumeStoryActorNavigation(companionRoam);
                    SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
                }

                if (leadTarget != null && Time.unscaledTime >= _townHouseLeadNextMoveRetryAt)
                {
                    bool issuedMove = false;
                    if (chief != null)
                    {
                        if (TryDriveEscortActor(chief, chiefRoam, leadTarget.position, townHouseLeadChiefArrivalDistance))
                        {
                            issuedMove = true;
                        }
                    }

                    if (companion != null)
                    {
                        Vector3 companionTarget = BuildEscortCompanionTarget(
                            chief != null ? chief.position : companion.position,
                            leadTarget.position);
                        if (TryDriveEscortActor(companion, companionRoam, companionTarget, townHouseLeadChiefArrivalDistance))
                        {
                            issuedMove = true;
                        }
                    }

                    if (issuedMove)
                    {
                        _townHouseLeadNextMoveRetryAt = Time.unscaledTime + townHouseLeadMoveRetryInterval;
                    }
                }
            }

            if (_townHouseLeadTransitionQueued || transitionTrigger == null)
            {
                return;
            }

            Vector3 transitionTarget = leadTarget != null
                ? leadTarget.position
                : transitionTrigger.transform.position;
            if (!AreEscortActorsReadyForTransition(chief, companion, transitionTarget, townHouseLeadChiefArrivalDistance))
            {
                return;
            }

            if (TryIsPlayerReadyForTownHouseTransition(transitionTrigger, leadTarget, out float playerDistance))
            {
                _townHouseLeadPlayerDistance = playerDistance;
                if (transitionTrigger.TryStartTransition())
                {
                    _townHouseLeadTransitionQueued = true;
                    SpringDay1PromptOverlay.Instance.Show(TownLeadTransitionPromptText);
                }
            }
        }

        private void TryQueuePrimaryHouseArrival()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null
                || storyManager.CurrentPhase != StoryPhase.EnterVillage
                || !HasVillageGateProgressed()
                || HasHouseArrivalProgressed()
                || IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId))
            {
                return;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            if (_houseArrivalSequencePlayed
                && !HasCompletedDialogueSequence(HouseArrivalSequenceId)
                && !IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId))
            {
                _houseArrivalSequencePlayed = false;
            }

            if (!TryPreparePrimaryArrivalActors())
            {
                return;
            }

            if (_primaryArrivalDialogueReadyAt > Time.unscaledTime)
            {
                return;
            }

            if (!ShouldQueueDialogueSequence(ref _houseArrivalSequencePlayed, HouseArrivalSequenceId))
            {
                return;
            }

            SpringDay1PromptOverlay.Instance.Show("先在村里这边站稳，艾拉马上接手你的伤。");
            PlayDialogueNowOrQueue(BuildHouseArrivalSequence());
        }

        private void TryApplyDebugWorkbenchSkip()
        {
            bool consumedEditorToggle = false;
            if (!IsDebugWorkbenchSkipEnabled(out consumedEditorToggle) || _debugWorkbenchSkipApplied || StoryManager.Instance == null)
            {
                return;
            }

            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            if (currentPhase == StoryPhase.FreeTime
                || currentPhase == StoryPhase.ReturnAndReminder
                || currentPhase == StoryPhase.DinnerConflict
                || currentPhase == StoryPhase.DayEnd)
            {
                _debugWorkbenchSkipApplied = true;
                ConsumeDebugWorkbenchSkipEditorPref(consumedEditorToggle);
                return;
            }

            _debugWorkbenchSkipApplied = true;
            _healingStarted = true;
            _healingSequencePlayed = true;
            _workbenchOpened = true;
            _workbenchSequencePlayed = true;
            _dinnerSequencePlayed = false;
            _returnSequencePlayed = false;
            _freeTimeEntered = false;
            _dayEnded = false;
            _dayEndTaskCardAutoHideArmed = false;
            _dayEndTaskCardShownAt = -1f;
            _freeTimeIntroQueued = false;
            _freeTimeIntroCompleted = false;
            _postTutorialWrapSequenceQueued = false;
            _postTutorialExploreWindowEntered = false;
            _dinnerGatheringRequested = false;
            _pendingDinnerTownLoad = false;
            ResetDinnerCueSettlementState();
            _workbenchCraftingActive = false;
            _workbenchCraftProgress = 0f;
            _workbenchCraftQueueTotal = 0;
            _workbenchCraftQueueCompleted = 0;
            _workbenchCraftRecipeName = string.Empty;
            _craftedCount = 0;
            StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);
            InitializeFarmingTutorialTracking(true);
            _tillObjectiveCompleted = true;
            _plantObjectiveCompleted = true;
            _waterObjectiveCompleted = true;
            _woodObjectiveCompleted = true;
            _woodTrackingArmed = true;
            _collectedWoodSinceWoodStepStart = requiredWoodCollectedCount;
            _baselineWoodCount = Mathf.Max(GetCurrentWoodCount(), requiredWoodCollectedCount);
            _trackedWoodCountSnapshot = _baselineWoodCount;
            ConsumeDebugWorkbenchSkipEditorPref(consumedEditorToggle);
            SpringDay1PromptOverlay.Instance.Show("调试直跳已开启：直接进入 0.0.5，返回工作台完成一次基础制作。");
        }

        private bool IsDebugWorkbenchSkipEnabled(out bool consumedEditorToggle)
        {
            consumedEditorToggle = false;
#if UNITY_EDITOR
            bool editorToggleEnabled = EditorPrefs.GetBool(DebugWorkbenchSkipEditorPrefKey, false);
            if (editorToggleEnabled)
            {
                consumedEditorToggle = true;
            }
#else
            const bool editorToggleEnabled = false;
#endif
            return debugSkipDirectToWorkbenchPhase05 || editorToggleEnabled;
        }

        private static void ConsumeDebugWorkbenchSkipEditorPref(bool consumedEditorToggle)
        {
#if UNITY_EDITOR
            if (consumedEditorToggle)
            {
                EditorPrefs.SetBool(DebugWorkbenchSkipEditorPrefKey, false);
            }
#endif
        }

        private void BeginHealingAndHp()
        {
            if (_healingStarted)
            {
                return;
            }

            _healingStarted = true;
            _postTutorialWrapSequenceQueued = false;
            _postTutorialExploreWindowEntered = false;
            _dinnerGatheringRequested = false;
            _pendingDinnerTownLoad = false;
            ResetDinnerCueSettlementState();
            ResetHealingBridgeState();
            _healingBridgePending = true;
            StoryManager.Instance.SetPhase(StoryPhase.HealingAndHP);
            SyncStoryTimePauseState();

            HealthSystem healthSystem = HealthSystem.Instance;
            healthSystem.SetHealthState(initialHealth, maxHealth);
            healthSystem.SetVisible(false);

            EnergySystem energySystem = EnergySystem.Instance != null
                ? EnergySystem.Instance
                : FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
            if (energySystem != null)
            {
                energySystem.SetVisible(false);
            }

            SpringDay1PromptOverlay.Instance.Show(HealingBridgePromptText);
        }

        private void TryHandleWorkbenchEscort()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null
                || storyManager.CurrentPhase != StoryPhase.WorkbenchFlashback
                || _workbenchOpened)
            {
                return;
            }

            bool startedNow = !_workbenchEscortStarted;
            if (startedNow)
            {
                _workbenchEscortStarted = true;
                _workbenchEscortWaitingForPlayer = false;
                _workbenchEscortNextMoveRetryAt = 0f;
                _workbenchEscortNextBubbleAt = 0f;
                _workbenchEscortCaughtUpSince = 0f;
                _workbenchEscortDialogueReadyAt = 0f;
            }

            UpdateWorkbenchEscortSnapshot(
                out Transform chief,
                out NPCAutoRoamController chiefRoam,
                out NPCBubblePresenter chiefBubble,
                out Transform companion,
                out NPCAutoRoamController companionRoam,
                out Transform workbenchTarget);
            ResolveWorkbenchEscortTargets(
                chief,
                companion,
                workbenchTarget,
                out Vector3 chiefEscortTarget,
                out Vector3 companionEscortTarget);

            string bridgePromptText = ResolveEscortBridgePromptText(
                chief,
                companion,
                WorkbenchEscortPromptText,
                "跟着村长去工作台，把今晚要学的活先接起来。",
                "跟着艾拉去工作台，把今晚要学的活先接起来。");

            if (startedNow)
            {
                SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            bool shouldWaitForPlayer = ResolveEscortWaitState(
                ShouldEscortWaitForPlayer(_workbenchEscortChiefDistance, _workbenchEscortPlayerDistance, _workbenchEscortWaitingForPlayer),
                _workbenchEscortWaitingForPlayer,
                ref _workbenchEscortCaughtUpSince);
            if (shouldWaitForPlayer)
            {
                if (!_workbenchEscortWaitingForPlayer)
                {
                    PauseStoryActorNavigation(chiefRoam);
                    PauseStoryActorNavigation(companionRoam);
                    SpringDay1PromptOverlay.Instance.Show(WorkbenchEscortWaitPromptText);
                }

                _workbenchEscortWaitingForPlayer = true;
                TryShowEscortWaitBubble(chiefBubble, WorkbenchEscortWaitPromptText, ref _workbenchEscortNextBubbleAt);
                return;
            }

            if (_workbenchEscortWaitingForPlayer)
            {
                _workbenchEscortWaitingForPlayer = false;
                _workbenchEscortCaughtUpSince = 0f;
                _workbenchEscortNextMoveRetryAt = 0f;
                ResumeStoryActorNavigation(chiefRoam);
                ResumeStoryActorNavigation(companionRoam);
                SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
            }

            if ((workbenchTarget != null || chief != null || companion != null) && Time.unscaledTime >= _workbenchEscortNextMoveRetryAt)
            {
                bool issuedMove = false;
                if (chief != null)
                {
                    if (TryDriveEscortActor(chief, chiefRoam, chiefEscortTarget, townHouseLeadChiefArrivalDistance))
                    {
                        issuedMove = true;
                    }
                }

                if (companion != null)
                {
                    if (TryDriveEscortActor(companion, companionRoam, companionEscortTarget, townHouseLeadChiefArrivalDistance))
                    {
                        issuedMove = true;
                    }
                }

                if (issuedMove)
                {
                    _workbenchEscortNextMoveRetryAt = Time.unscaledTime + townHouseLeadMoveRetryInterval;
                }
            }

            bool chiefReady = float.IsInfinity(_workbenchEscortChiefDistance)
                || _workbenchEscortChiefDistance <= townHouseLeadChiefArrivalDistance;
            bool companionReady = float.IsInfinity(_workbenchEscortCompanionDistance)
                || _workbenchEscortCompanionDistance <= townHouseLeadChiefArrivalDistance;
            bool playerReady = !float.IsInfinity(_workbenchEscortPlayerDistance)
                && _workbenchEscortPlayerDistance <= workbenchEscortReadyDistance;
            if (playerReady && chiefReady && companionReady)
            {
                if (!_workbenchEscortIdlePlaced)
                {
                    ApplyWorkbenchIdleLayout(chief, companion);
                    _workbenchEscortIdlePlaced = true;
                    _workbenchEscortDialogueReadyAt = Time.unscaledTime + workbenchEscortDialogueDelay;
                }

                HaltStoryActorNavigation(chiefRoam);
                HaltStoryActorNavigation(companionRoam);

                if (_workbenchEscortDialogueReadyAt > Time.unscaledTime)
                {
                    SpringDay1PromptOverlay.Instance.Show(WorkbenchEscortReadyPromptText);
                    return;
                }

                if (!HasWorkbenchBriefingCompleted())
                {
                    if (!IsDialogueSequenceCurrentlyActive(WorkbenchBriefingSequenceId)
                        && ShouldQueueDialogueSequence(ref _workbenchBriefingSequenceQueued, WorkbenchBriefingSequenceId))
                    {
                        PlayDialogueNowOrQueue(BuildWorkbenchBriefingSequence());
                    }

                    SpringDay1PromptOverlay.Instance.Show(WorkbenchBriefingPromptText);
                    return;
                }

                SpringDay1PromptOverlay.Instance.Show(WorkbenchEscortReadyPromptText);
            }
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

            if (!HasWorkbenchBriefingCompleted())
            {
                SpringDay1PromptOverlay.Instance.Show(WorkbenchBriefingPromptText);
                return;
            }

            if (HasCompletedDialogueSequence(WorkbenchSequenceId))
            {
                _workbenchSequencePlayed = true;
                StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);
                InitializeFarmingTutorialTracking(true);
                SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialTillPromptText());
                return;
            }

            if (ShouldQueueDialogueSequence(ref _workbenchSequencePlayed, WorkbenchSequenceId))
            {
                PlayDialogueNowOrQueue(BuildWorkbenchSequence());
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

            Transform workbenchCandidate = ResolveWorkbenchCandidateCached();
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

            RestTargetBinding binding = FindRestInteractionTargetCandidate();
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

        private static RestTargetBinding FindRestInteractionTargetCandidate()
        {
            Transform preferredBed = FindPreferredObjectTransform(PreferredBedObjectNames);
            if (IsBedCandidate(preferredBed))
            {
                return RestTargetBinding.ForBed(preferredBed);
            }

            Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int index = 0; index < allTransforms.Length; index++)
            {
                Transform candidate = allTransforms[index];
                if (IsBedCandidate(candidate))
                {
                    return RestTargetBinding.ForBed(candidate);
                }
            }

            return default;
        }

        private bool ShouldAutoHideDayEndTaskCard()
        {
            if (!_dayEndTaskCardAutoHideArmed)
            {
                return false;
            }

            if (dayEndTaskCardVisibleSeconds <= 0f)
            {
                return true;
            }

            if (_dayEndTaskCardShownAt < 0f)
            {
                return false;
            }

            return Time.unscaledTime - _dayEndTaskCardShownAt >= dayEndTaskCardVisibleSeconds;
        }

        public bool TryNormalizeDebugTimeTarget(
            int requestedYear,
            SeasonManager.Season requestedSeason,
            int requestedDay,
            int requestedHour,
            int requestedMinute,
            out int normalizedYear,
            out SeasonManager.Season normalizedSeason,
            out int normalizedDay,
            out int normalizedHour,
            out int normalizedMinute)
        {
            normalizedYear = requestedYear;
            normalizedSeason = requestedSeason;
            normalizedDay = requestedDay;
            normalizedHour = requestedHour;
            normalizedMinute = requestedMinute;

            StoryManager storyManager = StoryManager.Instance;
            if (!ShouldManageDay1TimeGuardrails(storyManager))
            {
                return false;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return false;
            }

            bool requestsNextManagedDay = requestedYear == timeManager.GetYear()
                && requestedSeason == timeManager.GetSeason()
                && requestedDay > timeManager.GetDay();
            if (requestsNextManagedDay && CanFinalizeDayEndFromCurrentState(storyManager.CurrentPhase))
            {
                return false;
            }

            NormalizeManagedDay1TimeTarget(
                requestedYear,
                requestedSeason,
                requestedDay,
                requestedHour,
                requestedMinute,
                storyManager.CurrentPhase,
                out normalizedYear,
                out normalizedSeason,
                out normalizedDay,
                out normalizedHour,
                out normalizedMinute);
            return true;
        }

        private void EnforceDay1TimeGuardrails()
        {
            if (_suppressDay1TimeGuardrail)
            {
                return;
            }

            StoryManager storyManager = StoryManager.Instance;
            if (!ShouldManageDay1TimeGuardrails(storyManager))
            {
                return;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return;
            }

            NormalizeManagedDay1TimeTarget(
                timeManager.GetYear(),
                timeManager.GetSeason(),
                timeManager.GetDay(),
                timeManager.GetHour(),
                timeManager.GetMinute(),
                storyManager.CurrentPhase,
                out int normalizedYear,
                out SeasonManager.Season normalizedSeason,
                out int normalizedDay,
                out int normalizedHour,
                out int normalizedMinute);

            if (normalizedYear == timeManager.GetYear()
                && normalizedSeason == timeManager.GetSeason()
                && normalizedDay == timeManager.GetDay()
                && normalizedHour == timeManager.GetHour()
                && normalizedMinute == timeManager.GetMinute())
            {
                return;
            }

            ApplyManagedDay1TimeTarget(normalizedYear, normalizedSeason, normalizedDay, normalizedHour, normalizedMinute);
        }

        private bool ShouldManageDay1TimeGuardrails(StoryManager storyManager)
        {
            return Application.isPlaying
                && storyManager != null
                && !_dayEnded
                && storyManager.CurrentPhase >= StoryPhase.CrashAndMeet
                && storyManager.CurrentPhase < StoryPhase.DayEnd;
        }

        private void NormalizeManagedDay1TimeTarget(
            int requestedYear,
            SeasonManager.Season requestedSeason,
            int requestedDay,
            int requestedHour,
            int requestedMinute,
            StoryPhase currentPhase,
            out int normalizedYear,
            out SeasonManager.Season normalizedSeason,
            out int normalizedDay,
            out int normalizedHour,
            out int normalizedMinute)
        {
            normalizedYear = Day1RuntimeManagedYear;
            normalizedSeason = SeasonManager.Season.Spring;
            normalizedDay = Day1RuntimeManagedDay;

            int clampedRequestedHour = Mathf.Max(0, requestedHour);
            int clampedRequestedMinute = Mathf.Clamp(requestedMinute, 0, 59);
            int requestedTotalMinutes = (clampedRequestedHour * 60) + clampedRequestedMinute;
            int minimumTotalMinutes = GetManagedMinimumTotalMinutesForPhase(currentPhase);
            int maximumTotalMinutes = GetManagedMaximumTotalMinutesForPhase(currentPhase);
            int normalizedTotalMinutes = Mathf.Clamp(requestedTotalMinutes, minimumTotalMinutes, maximumTotalMinutes);
            normalizedHour = normalizedTotalMinutes / 60;
            normalizedMinute = normalizedTotalMinutes % 60;
        }

        private int GetManagedMinimumTotalMinutesForPhase(StoryPhase currentPhase)
        {
            return currentPhase switch
            {
                StoryPhase.DinnerConflict => (DinnerReturnHour * 60) + DinnerReturnMinute,
                StoryPhase.ReturnAndReminder => (ReminderStartHour * 60) + ReminderStartMinute,
                StoryPhase.FreeTime => (FreeTimeStartHour * 60) + FreeTimeStartMinute,
                StoryPhase.FarmingTutorial when _postTutorialExploreWindowEntered || _dinnerGatheringRequested || _pendingDinnerTownLoad => TutorialTimeCapHour * 60,
                _ => TownOpeningHour * 60
            };
        }

        private int GetManagedMaximumTotalMinutesForPhase(StoryPhase currentPhase)
        {
            return currentPhase switch
            {
                StoryPhase.DinnerConflict => ((ReminderStartHour * 60) + ReminderStartMinute) - 1,
                StoryPhase.ReturnAndReminder => ((FreeTimeStartHour * 60) + FreeTimeStartMinute) - 1,
                StoryPhase.FreeTime => 26 * 60,
                StoryPhase.FarmingTutorial when _postTutorialExploreWindowEntered || _dinnerGatheringRequested || _pendingDinnerTownLoad => (DinnerReturnHour * 60) + DinnerReturnMinute,
                _ => (TutorialTimeCapHour * 60) + 59
            };
        }

        private void ApplyManagedDay1TimeTarget(
            int targetYear,
            SeasonManager.Season targetSeason,
            int targetDay,
            int targetHour,
            int targetMinute)
        {
            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return;
            }

            _suppressDay1TimeGuardrail = true;
            try
            {
                timeManager.SetTime(targetYear, targetSeason, targetDay, targetHour, targetMinute);
            }
            finally
            {
                _suppressDay1TimeGuardrail = false;
            }
        }

        private void TryPlacePlayerNearCurrentSceneRestTarget()
        {
            if (_playerMovement == null || _playerMovement.gameObject.scene != SceneManager.GetActiveScene())
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                return;
            }

            RestTargetBinding restTarget = FindRestTargetCandidate();
            Vector3 targetPosition = playerTransform.position;
            if (TryResolveForcedSleepHomePlacement(out Vector3 homePlacement))
            {
                targetPosition = homePlacement;
            }
            else if (restTarget.Transform != null)
            {
                targetPosition = restTarget.Transform.position;
                targetPosition.y += forcedSleepTownBedOffsetY;
            }

            targetPosition.z = playerTransform.position.z;
            _playerMovement.StopMovement();
            Rigidbody2D playerBody = _playerMovement.GetComponent<Rigidbody2D>();
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector2.zero;
                playerBody.position = new Vector2(targetPosition.x, targetPosition.y);
                playerTransform.position = targetPosition;
            }
            else
            {
                playerTransform.position = targetPosition;
            }

            Physics2D.SyncTransforms();
        }

        private void ArmForcedSleepRestPlacementRetries()
        {
            _pendingForcedSleepRestPlacementFrames = 3;
        }

        private void TryFinalizePendingForcedSleepRestPlacement()
        {
            if (_pendingForcedSleepRestPlacementFrames <= 0)
            {
                return;
            }

            _pendingForcedSleepRestPlacementFrames--;
            TryPlacePlayerNearCurrentSceneRestTarget();
        }

        private bool TryResolveForcedSleepHomePlacement(out Vector3 targetPosition)
        {
            targetPosition = Vector3.zero;
            if (!IsHomeSceneActive())
            {
                return false;
            }

            Transform preferredBed = FindPreferredObjectTransform(PreferredBedObjectNames);
            if (preferredBed != null && preferredBed.gameObject.activeInHierarchy)
            {
                targetPosition = preferredBed.position;
                targetPosition.y += forcedSleepTownBedOffsetY;
                return true;
            }

            Transform homeDoor = FindPreferredObjectTransform(PreferredRestProxyObjectNames);
            if (homeDoor == null || !homeDoor.gameObject.activeInHierarchy)
            {
                return false;
            }

            targetPosition = homeDoor.position;
            targetPosition.x += HomeDoorSleepFallbackOffset.x;
            targetPosition.y += HomeDoorSleepFallbackOffset.y;
            return true;
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

            if (ShouldQueueDialogueSequence(ref _healingSequencePlayed, HealingSequenceId))
            {
                PlayDialogueNowOrQueue(BuildHealingSequence());
            }
        }

        private void TryHandleHealingBridge()
        {
            if (!_healingStarted || _healingBridgeSequenceQueued || _healingSequencePlayed)
            {
                return;
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            UpdateHealingBridgeSnapshot(out Transform supportNpc, out NPCAutoRoamController supportRoam);
            if (supportNpc == null || !TryGetPlayerInteractionSamplePoint(out Vector2 playerSamplePoint))
            {
                StartHealingSequence();
                return;
            }

            Vector2 npcPosition = supportNpc.position;
            _healingBridgeNpcDistance = Vector2.Distance(npcPosition, playerSamplePoint);
            if (_healingBridgeNpcDistance > healingSupportApproachDistance)
            {
                HaltStoryActorNavigation(supportRoam);
                _healingBridgeHoldUntil = 0f;
                SpringDay1PromptOverlay.Instance.Show(HealingBridgePromptText);
                return;
            }

            HaltStoryActorNavigation(supportRoam);

            if (_healingBridgeHoldUntil <= 0f)
            {
                _healingBridgeHoldUntil = Time.unscaledTime + healingSupportPauseDuration;
                SpringDay1PromptOverlay.Instance.Show(HealingBridgeArrivalPromptText);
                return;
            }

            if (Time.unscaledTime < _healingBridgeHoldUntil)
            {
                return;
            }

            StartHealingSequence();
        }

        private void StartHealingSequence()
        {
            if (_healingBridgeSequenceQueued)
            {
                return;
            }

            _healingBridgeSequenceQueued = true;
            StartCoroutine(HealAndPromptCoroutine());
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
            if (manager == null)
            {
                return false;
            }

            return manager.HasCompletedSequence(sequenceId);
        }

        private static bool IsDialogueSequenceConsumed(string sequenceId)
        {
            return IsDialogueSequenceCurrentlyActive(sequenceId) || HasCompletedDialogueSequence(sequenceId);
        }

        private static string ResolveActiveFormalTaskListSequenceId()
        {
            string[] sequenceIds =
            {
                FirstSequenceId,
                FirstFollowupSequenceId,
                VillageGateSequenceId,
                HouseArrivalSequenceId,
                HealingSequenceId,
                WorkbenchBriefingSequenceId,
                WorkbenchSequenceId,
                PostTutorialWrapSequenceId,
                DinnerSequenceId,
                ReminderSequenceId,
                FreeTimeIntroSequenceId
            };

            for (int index = 0; index < sequenceIds.Length; index++)
            {
                string sequenceId = sequenceIds[index];
                if (IsDialogueSequenceCurrentlyActive(sequenceId))
                {
                    return sequenceId;
                }
            }

            return string.Empty;
        }

        private static string ResolveLatestConsumedFormalTaskListSequenceId()
        {
            string[] sequenceIds =
            {
                FreeTimeIntroSequenceId,
                ReminderSequenceId,
                DinnerSequenceId,
                PostTutorialWrapSequenceId,
                WorkbenchSequenceId,
                WorkbenchBriefingSequenceId,
                HealingSequenceId,
                HouseArrivalSequenceId,
                VillageGateSequenceId,
                FirstFollowupSequenceId,
                FirstSequenceId
            };

            for (int index = 0; index < sequenceIds.Length; index++)
            {
                string sequenceId = sequenceIds[index];
                if (HasCompletedDialogueSequence(sequenceId))
                {
                    return sequenceId;
                }
            }

            return string.Empty;
        }

        private static bool ShouldQueueDialogueSequence(ref bool localPlayedFlag, string sequenceId)
        {
            if (localPlayedFlag || IsDialogueSequenceConsumed(sequenceId))
            {
                return false;
            }

            localPlayedFlag = true;
            return true;
        }

        private void TryRecoverConsumedSequenceProgression(StoryPhase currentPhase)
        {
            if (StoryManager.Instance == null)
            {
                return;
            }

            switch (currentPhase)
            {
                case StoryPhase.EnterVillage:
                    if (HasCompletedDialogueSequence(HouseArrivalSequenceId))
                    {
                        _villageGateSequencePlayed = true;
                        _houseArrivalSequencePlayed = true;
                        if (IsPrimarySceneActive() && !IsDialogueChainStillActive())
                        {
                            BeginHealingAndHp();
                        }
                    }
                    else if (HasCompletedDialogueSequence(VillageGateSequenceId))
                    {
                        _villageGateSequencePlayed = true;
                        if (IsPrimarySceneActive())
                        {
                            TryQueuePrimaryHouseArrival();
                        }
                    }

                    break;
                case StoryPhase.HealingAndHP:
                    if (HasCompletedDialogueSequence(HealingSequenceId))
                    {
                        _healingSequencePlayed = true;
                        StoryManager.Instance.SetPhase(StoryPhase.WorkbenchFlashback);
                        SpringDay1PromptOverlay.Instance.Show("靠近工作台，尝试打开制作菜单。");
                    }
                    break;
                case StoryPhase.WorkbenchFlashback:
                    if (HasCompletedDialogueSequence(WorkbenchSequenceId))
                    {
                        _workbenchSequencePlayed = true;
                        StoryManager.Instance.SetPhase(StoryPhase.FarmingTutorial);
                        InitializeFarmingTutorialTracking(true);
                        SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialTillPromptText());
                    }
                    break;
                case StoryPhase.FarmingTutorial:
                    if (!_postTutorialExploreWindowEntered && HasCompletedDialogueSequence(PostTutorialWrapSequenceId))
                    {
                        _postTutorialWrapSequenceQueued = true;
                        RequestPostTutorialExploreWindowEntry();
                    }
                    else
                    {
                        TryHandlePostTutorialExploreWindow();
                    }
                    break;
                case StoryPhase.DinnerConflict:
                    if (HasCompletedDialogueSequence(DinnerSequenceId))
                    {
                        _dinnerSequencePlayed = true;
                        BeginReturnReminder();
                    }
                    break;
                case StoryPhase.ReturnAndReminder:
                    if (HasCompletedDialogueSequence(ReminderSequenceId))
                    {
                        _returnSequencePlayed = true;
                        EnterFreeTime();
                    }
                    break;
                case StoryPhase.FreeTime:
                    if (!_freeTimeIntroCompleted && HasCompletedDialogueSequence(FreeTimeIntroSequenceId))
                    {
                        _freeTimeIntroQueued = true;
                        CompleteFreeTimeIntro();
                    }
                    break;
            }
        }

        private void CompleteFreeTimeIntro()
        {
            _freeTimeIntroCompleted = true;

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager != null)
            {
                int currentHour = timeManager.GetHour();
                if (currentHour < FreeTimeNightWarningHour)
                {
                    SpringDay1PromptOverlay.Instance.Show(FreeTimePromptText);
                }
                else
                {
                    ShowFreeTimePressurePromptForHour(currentHour);
                }
            }
            else
            {
                SpringDay1PromptOverlay.Instance.Show(FreeTimePromptText);
            }
        }

        private void TickFarmingTutorial()
        {
            InitializeFarmingTutorialTracking();

            StoryManager storyManager = StoryManager.Instance;
            StoryPhase currentPhase = storyManager != null ? storyManager.CurrentPhase : StoryPhase.None;
            bool previousStoryActorMode = ShouldUsePrimaryStoryActorMode(currentPhase);

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

            bool storyActorModeChanged = previousStoryActorMode != ShouldUsePrimaryStoryActorMode(currentPhase);
            if (storyActorModeChanged || IsAwaitingPostTutorialChiefWrap())
            {
                UpdateSceneStoryNpcVisibility();
            }

            if (!_staminaRevealed && _tillObjectiveCompleted)
            {
                _staminaRevealed = true;
                EnergySystem.Instance.SetEnergyState(initialEnergy, maxEnergy);
                EnergySystem.Instance.PlayRevealAndAnimateTo(initialEnergy, initialEnergy, maxEnergy, energyRevealDuration, 0f);
            }

            if (!_tillObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialTillPromptText());
                return;
            }

            if (!_plantObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialPlantPromptText());
                return;
            }

            if (!_waterObjectiveCompleted)
            {
                SpringDay1PromptOverlay.Instance.Show(GetFarmingTutorialWaterPromptText());
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

            if (IsPostTutorialExploreWindowActive())
            {
                TryHandlePostTutorialExploreWindow();
                return;
            }

            SpringDay1PromptOverlay.Instance.Show(PostTutorialWrapPromptText);
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
                return "执行 Step，模拟一次基础制作并推进到和村长收口。";
            }

            return "农田教学目标已齐，先去和村长说一声，再进入傍晚自由活动。";
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
                return "验收入口：已模拟完成一次基础制作，当前应进入和村长收口的阶段。";
            }

            TickFarmingTutorial();
            return "农田教学验证目标已全部完成；接下来应和村长收口并进入傍晚自由活动。";
        }

        public string GetValidationFreeTimeNextAction()
        {
            if (IsFreeTimeIntroPending())
            {
                return IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId)
                    ? "先把夜里的见闻听完，再继续验证回屋收束。"
                    : "自由时段见闻尚未收束，先让这段夜里的动静完整接管。";
            }

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
            if (IsFreeTimeIntroPending())
            {
                return IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId)
                    ? "当前夜间见闻仍在进行，先把它听完。"
                    : "自由时段见闻尚未收束，先听完这段夜里的动静。";
            }

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
            _workbenchCraftRecipeName = GetPlayerFacingWorkbenchRecipeName(recipe);
        }

        public bool CanPerformWorkbenchCraft(out string blockerMessage)
        {
            blockerMessage = string.Empty;
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return true;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            if (currentPhase == StoryPhase.WorkbenchFlashback && !_workbenchOpened)
            {
                blockerMessage = HasWorkbenchBriefingCompleted()
                    ? "先完成这段工作台回忆。"
                    : "先听村长把工作台和边上的补给箱交代清楚。";
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
                StoryPhase.WorkbenchFlashback => HasWorkbenchBriefingCompleted(),
                _ => true
            };
        }

        public bool ShouldShowWorkbenchEntryHint()
        {
            StoryPhase currentPhase = StoryManager.Instance.CurrentPhase;
            return currentPhase == StoryPhase.WorkbenchFlashback
                && HasWorkbenchBriefingCompleted()
                && !_workbenchOpened;
        }

        private static string GetPlayerFacingWorkbenchRecipeName(RecipeData recipe)
        {
            if (recipe == null)
            {
                return string.Empty;
            }

            string recipeName = recipe.recipeName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                return $"配方 {recipe.recipeID}";
            }

            string lower = recipeName.ToLowerInvariant();
            if (lower.Contains("pickaxe"))
            {
                return "镐子";
            }

            if (lower.Contains("axe"))
            {
                return "斧头";
            }

            if (lower.Contains("hoe"))
            {
                return "锄头";
            }

            if (lower.Contains("storage"))
            {
                return "箱子";
            }

            if (lower.Contains("sword"))
            {
                return "短剑";
            }

            return recipeName.Replace('_', ' ');
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

        public bool IsTownHouseLeadPending()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.EnterVillage
                && IsTownSceneActive()
                && !IsDialogueSequenceCurrentlyActive(VillageGateSequenceId)
                && HasVillageGateProgressed()
                && !HasHouseArrivalProgressed();
        }

        public bool ShouldReleaseEnterVillageCrowd()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.EnterVillage
                && IsTownSceneActive()
                && (HasVillageGateCompleted()
                    || _townHouseLeadStarted
                    || _townHouseLeadTransitionQueued
                    || _townHouseLeadWaitingForPlayer);
        }

        public bool ShouldHoldEnterVillageCrowdCue()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.EnterVillage
                && IsTownSceneActive()
                && IsDialogueSequenceCurrentlyActive(VillageGateSequenceId);
        }

        public bool IsTownHouseLeadRuntimeActive()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.EnterVillage
                && (_townHouseLeadStarted || _townHouseLeadTransitionQueued || _townHouseLeadWaitingForPlayer);
        }

        public string GetTownHouseLeadSummary()
        {
            if (!IsTownHouseLeadPending())
            {
                return "inactive";
            }

            UpdateTownHouseLeadSnapshot(
                out _,
                out _,
                out _,
                out _,
                out _,
                out _,
                out _);

            return
                $"started={_townHouseLeadStarted}" +
                $"|queued={_townHouseLeadTransitionQueued}" +
                $"|waiting={_townHouseLeadWaitingForPlayer}" +
                $"|chief={_townHouseLeadChiefName}" +
                $"|companion={_townHouseLeadCompanionName}" +
                $"|target={_townHouseLeadTargetName}" +
                $"|chiefDist={FormatLeadDistance(_townHouseLeadChiefDistance)}" +
                $"|companionDist={FormatLeadDistance(_townHouseLeadCompanionDistance)}" +
                $"|playerDist={FormatLeadDistance(_townHouseLeadPlayerDistance)}";
        }

        private bool IsReturnToTownEscortPending()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.DinnerConflict
                && IsPrimarySceneActive()
                && !HasCompletedDialogueSequence(DinnerSequenceId);
        }

        private bool HasVillageGateProgressed()
        {
            return _villageGateSequencePlayed || HasCompletedDialogueSequence(VillageGateSequenceId);
        }

        private bool HasVillageGateCompleted()
        {
            return HasCompletedDialogueSequence(VillageGateSequenceId);
        }

        private bool ShouldUseEnterVillageHouseArrivalBeat()
        {
            if (IsDialogueSequenceCurrentlyActive(VillageGateSequenceId))
            {
                return false;
            }

            return IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId)
                || HasVillageGateCompleted()
                || _townHouseLeadStarted
                || _townHouseLeadTransitionQueued
                || HasHouseArrivalProgressed();
        }

        private bool HasHouseArrivalProgressed()
        {
            return _houseArrivalSequencePlayed || HasCompletedDialogueSequence(HouseArrivalSequenceId);
        }

        public bool IsWorkbenchFlashbackAwaitingInteraction()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.WorkbenchFlashback
                && !_workbenchOpened;
        }

        public bool IsHealingBridgePendingForValidation()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.HealingAndHP
                && _healingStarted
                && _healingBridgePending
                && !_healingBridgeSequenceQueued
                && !_healingSequencePlayed;
        }

        public string GetValidationHealingNextAction()
        {
            if (IsHealingBridgePendingForValidation())
            {
                return "执行 Step，把玩家验证位推进到艾拉身边，再等疗伤对白和 HP 卡片接管。";
            }

            if (_healingSequencePlayed || HasCompletedDialogueSequence(HealingSequenceId))
            {
                return "疗伤对白已收束，等待工作台闪回接管。";
            }

            return "疗伤阶段正在自动接管，先别被别的交互带跑。";
        }

        public string GetValidationWorkbenchNextAction()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.WorkbenchFlashback)
            {
                return "当前不在工作台闪回阶段。";
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return "工作台前的桥接对白正在进行，继续执行 Step 即可。";
            }

            if (!HasWorkbenchBriefingCompleted())
            {
                return "执行 Step，把玩家验证位推进到工作台边，再让村长把箱子和工作台交代清楚。";
            }

            if (_workbenchOpened)
            {
                return "工作台已打开，等待工作台回忆自动播出。";
            }

            return "执行 Step，把玩家验证位推进到工作台交互范围并触发工作台回忆。";
        }

        public string TryAdvancePrimaryArrivalValidationStep()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.EnterVillage)
            {
                return "当前不在进村承接阶段。";
            }

            if (!IsFirstFollowupPending())
            {
                return "进村安置已收束，等待疗伤段启动。";
            }

            if (!IsPrimarySceneActive())
            {
                return "Town 围观/Primary 安置现在由导演自动接管；当前无需再次手动触发 NPC001。";
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return "Primary 承接对白正在进行，继续推进对白即可。";
            }

            if (_houseArrivalSequencePlayed
                && !HasCompletedDialogueSequence(HouseArrivalSequenceId)
                && !IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId))
            {
                _houseArrivalSequencePlayed = false;
            }

            TryQueuePrimaryHouseArrival();

            if (IsDialogueSequenceCurrentlyActive(HouseArrivalSequenceId))
            {
                return "验收入口：已重新接起 Primary 承接对白。";
            }

            if (HasCompletedDialogueSequence(HouseArrivalSequenceId))
            {
                return "Primary 承接对白已收束，等待疗伤段启动。";
            }

            return "Primary 承接对白仍未接起；当前需要继续检查入场摆位或对白排队。";
        }

        public string TryAdvanceHealingValidationStep()
        {
            if (!IsHealingBridgePendingForValidation())
            {
                return GetValidationHealingNextAction();
            }

            UpdateHealingBridgeSnapshot(out Transform supportNpc, out NPCAutoRoamController supportRoam);
            if (!TrySnapValidationPlayerNearHealingSupport(supportNpc, out float resultingDistance, out string message))
            {
                return message;
            }

            HaltStoryActorNavigation(supportRoam);
            if (!_healingBridgeSequenceQueued)
            {
                StartHealingSequence();
            }

            return resultingDistance <= healingSupportApproachDistance
                ? "验收入口：已模拟玩家靠近艾拉，疗伤对白与 HP 卡片开始接管。"
                : $"验收入口：已模拟玩家靠近艾拉并强制疗伤接管（当前采样距离={resultingDistance:F2}）。";
        }

        public string TryAdvanceWorkbenchValidationStep()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null || storyManager.CurrentPhase != StoryPhase.WorkbenchFlashback)
            {
                return GetValidationWorkbenchNextAction();
            }

            if (_workbenchOpened)
            {
                return "工作台已打开，等待工作台回忆自动播出。";
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return "工作台前的桥接对白正在进行，继续推进对白即可。";
            }

            UpdateWorkbenchEscortSnapshot(
                out Transform chief,
                out NPCAutoRoamController chiefRoam,
                out NPCBubblePresenter _,
                out Transform companion,
                out NPCAutoRoamController companionRoam,
                out Transform workbenchTarget);

            TryAutoBindWorkbenchInteractable();
            CraftingStationInteractable interactable = _boundWorkbenchInteractable;
            bool awaitingBriefing = !HasWorkbenchBriefingCompleted();
            Transform snapTarget = awaitingBriefing
                ? chief != null ? chief : companion != null ? companion : workbenchTarget
                : interactable != null ? interactable.transform : workbenchTarget;
            if (!TrySnapValidationPlayerNearEscortTarget(
                    snapTarget,
                    awaitingBriefing
                        ? workbenchEscortReadyDistance
                        : interactable != null ? interactable.InteractionDistance : workbenchEscortReadyDistance,
                    out float resultingDistance,
                    out string message))
            {
                return message;
            }

            if (!_workbenchEscortIdlePlaced)
            {
                ApplyWorkbenchIdleLayout(chief, companion);
                _workbenchEscortIdlePlaced = true;
            }

            _workbenchEscortDialogueReadyAt = 0f;
            HaltStoryActorNavigation(chiefRoam);
            HaltStoryActorNavigation(companionRoam);

            if (!HasWorkbenchBriefingCompleted())
            {
                if (!IsDialogueSequenceCurrentlyActive(WorkbenchBriefingSequenceId)
                    && ShouldQueueDialogueSequence(ref _workbenchBriefingSequenceQueued, WorkbenchBriefingSequenceId))
                {
                    PlayDialogueNowOrQueue(BuildWorkbenchBriefingSequence());
                }

                return resultingDistance <= workbenchEscortReadyDistance
                    ? "验收入口：已把玩家推进到工作台边，村长正在交代箱子和工作台。"
                    : $"验收入口：已把玩家推进到工作台边并强制工作台桥接对白接管（当前采样距离={resultingDistance:F2}）。";
            }

            if (interactable == null)
            {
                return "验收入口：已完成工作台前交代，但当前未找到工作台交互脚本。";
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            InteractionContext context = playerTransform != null
                ? new InteractionContext
                {
                    PlayerTransform = playerTransform,
                    PlayerPosition = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform)
                }
                : null;
            if (context == null)
            {
                return "验收入口：玩家交互上下文缺失，当前无法自动触发工作台。";
            }

            if (!interactable.CanInteract(context))
            {
                return "验收入口：玩家已被推进到工作台边，但当前仍被 UI 或对白门槛拦住。";
            }

            interactable.OnInteract(context);
            return "验收入口：已把玩家推进到工作台边并触发工作台回忆。";
        }

        public string GetHealingBridgeSummary()
        {
            if (!_healingStarted)
            {
                return "inactive";
            }

            return
                $"pending={_healingBridgePending}" +
                $"|queued={_healingBridgeSequenceQueued}" +
                $"|hold={(Mathf.Max(0f, _healingBridgeHoldUntil - Time.unscaledTime)).ToString("F2")}" +
                $"|npc={_healingBridgeNpcName}" +
                $"|dist={FormatLeadDistance(_healingBridgeNpcDistance)}";
        }

        public bool IsDinnerDialoguePendingStart()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.DinnerConflict
                && _dinnerSequencePlayed
                && !HasCompletedDialogueSequence(DinnerSequenceId)
                && !IsDialogueSequenceCurrentlyActive(DinnerSequenceId);
        }

        public bool IsReminderDialoguePendingStart()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.ReturnAndReminder
                && _returnSequencePlayed
                && !HasCompletedDialogueSequence(ReminderSequenceId)
                && !IsDialogueSequenceCurrentlyActive(ReminderSequenceId);
        }

        public bool IsReturnEscortPendingForValidation()
        {
            return IsReturnToTownEscortPending();
        }

        public string TryRequestValidationEscortTransition()
        {
            if (IsTownHouseLeadPending())
            {
                return TryRequestTownHouseLeadValidationTransition();
            }

            if (IsReturnToTownEscortPending())
            {
                return TryRequestReturnEscortValidationTransition();
            }

            return "当前没有待请求的 escort 转场。";
        }

        public bool IsSleepInteractionAvailable()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.FreeTime
                && _freeTimeEntered
                && _freeTimeIntroCompleted
                && !_dayEnded;
        }

        private bool IsFreeTimeIntroPending()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null
                && storyManager.CurrentPhase == StoryPhase.FreeTime
                && _freeTimeEntered
                && !_freeTimeIntroCompleted
                && !_dayEnded;
        }

        private static string TryRequestValidationEscortTransition(SceneTransitionTrigger2D trigger, string tag, string targetSceneName)
        {
            if (trigger == null)
            {
                return $"{tag}:trigger-missing:{targetSceneName}";
            }

            return trigger.TryStartTransition()
                ? $"{tag}:transition-requested:{trigger.name}"
                : $"{tag}:transition-request-failed:{trigger.name}";
        }

        private string TryRequestTownHouseLeadValidationTransition()
        {
            UpdateTownHouseLeadSnapshot(
                out Transform chief,
                out NPCAutoRoamController _,
                out NPCBubblePresenter _,
                out Transform companion,
                out NPCAutoRoamController _,
                out Transform leadTarget,
                out SceneTransitionTrigger2D transitionTrigger);

            if (leadTarget == null)
            {
                return $"town-house-lead:lead-target-missing:{PrimarySceneName}";
            }

            if (!TryIsPlayerReadyForTownHouseTransition(transitionTrigger, leadTarget, out float playerDistance))
            {
                if (!TrySnapValidationPlayerNearEscortTarget(leadTarget, townHouseLeadPlayerTransitionDistance, out float snappedDistance, out string snapMessage))
                {
                    return snapMessage;
                }

                _townHouseLeadPlayerDistance = snappedDistance;
                return $"town-house-lead:player-snapped:{FormatLeadDistance(snappedDistance)}";
            }

            _townHouseLeadPlayerDistance = playerDistance;
            if (!AreEscortActorsReadyForTransition(chief, companion, leadTarget.position, townHouseLeadChiefArrivalDistance))
            {
                return $"town-house-lead:escort-not-ready:chief={FormatLeadDistance(_townHouseLeadChiefDistance)}|companion={FormatLeadDistance(_townHouseLeadCompanionDistance)}";
            }

            return TryRequestValidationEscortTransition(transitionTrigger, "town-house-lead", PrimarySceneName);
        }

        private string TryRequestReturnEscortValidationTransition()
        {
            UpdateReturnEscortSnapshot(
                out Transform chief,
                out NPCAutoRoamController _,
                out NPCBubblePresenter _,
                out Transform companion,
                out NPCAutoRoamController _,
                out SceneTransitionTrigger2D transitionTrigger);

            Transform leadTarget = transitionTrigger != null ? transitionTrigger.transform : null;
            if (!TryIsPlayerReadyForSceneTransition(transitionTrigger, leadTarget, townHouseLeadPlayerTransitionDistance, out float playerDistance))
            {
                if (!TrySnapValidationPlayerNearEscortTarget(leadTarget, townHouseLeadPlayerTransitionDistance, out float snappedDistance, out string snapMessage))
                {
                    return snapMessage;
                }

                _returnEscortPlayerDistance = snappedDistance;
                return $"primary-return-escort:player-snapped:{FormatLeadDistance(snappedDistance)}";
            }

            _returnEscortPlayerDistance = playerDistance;
            if (!AreEscortActorsReadyForTransition(chief, companion, leadTarget != null ? leadTarget.position : Vector3.zero, townHouseLeadChiefArrivalDistance))
            {
                return $"primary-return-escort:escort-not-ready:chief={FormatLeadDistance(_returnEscortChiefDistance)}|companion={FormatLeadDistance(_returnEscortCompanionDistance)}";
            }

            return TryRequestValidationEscortTransition(transitionTrigger, "primary-return-escort", TownSceneName);
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

        private void BeginDinnerConflict()
        {
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return;
            }

            if (storyManager.CurrentPhase != StoryPhase.DinnerConflict)
            {
                storyManager.SetPhase(StoryPhase.DinnerConflict);
            }

            EnsureStoryTimeAtLeast(DinnerReturnHour, DinnerReturnMinute);
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Hide();

            if (IsPrimarySceneActive())
            {
                ResetDinnerCueSettlementState();
                TryHandleReturnToTownEscort();
                return;
            }

            SpringDay1PromptOverlay.Instance.Show(DinnerBridgePromptText);

            if (HasCompletedDialogueSequence(DinnerSequenceId))
            {
                _dinnerSequencePlayed = true;
                BeginReturnReminder();
                return;
            }

            AlignTownDinnerGatheringStoryActors();

            if (!CanStartDinnerConflictDialogueNow())
            {
                return;
            }

            if (!ShouldQueueDialogueSequence(ref _dinnerSequencePlayed, DinnerSequenceId))
            {
                return;
            }

            ResetDinnerCueSettlementState();
            PlayDialogueNowOrQueue(BuildDinnerSequence());
        }

        private bool CanStartDinnerConflictDialogueNow()
        {
            return EvaluateDinnerCueStartPermission(ResolveDinnerCueSettledState());
        }

        private bool EvaluateDinnerCueStartPermission(bool beatSettled)
        {
            if (beatSettled)
            {
                ResetDinnerCueSettlementState();
                return true;
            }

            if (_dinnerCueWaitStartedAt < 0f)
            {
                _dinnerCueWaitStartedAt = Time.unscaledTime;
                return false;
            }

            bool timedOut = dinnerCueSettleTimeout <= 0f
                || Time.unscaledTime - _dinnerCueWaitStartedAt >= dinnerCueSettleTimeout;
            if (!timedOut)
            {
                return false;
            }

            if (!SpringDay1NpcCrowdDirector.ForceSettleBeatCue(SpringDay1DirectorBeatKeys.DinnerConflictTable))
            {
                return false;
            }

            ResetDinnerCueSettlementState();
            return true;
        }

        private bool ResolveDinnerCueSettledState()
        {
#if UNITY_EDITOR
            if (_editorDinnerCueSettledOverride.HasValue)
            {
                return _editorDinnerCueSettledOverride.Value;
            }
#endif
            return SpringDay1NpcCrowdDirector.IsBeatCueSettled(SpringDay1DirectorBeatKeys.DinnerConflictTable);
        }

        private void BeginReturnReminder()
        {
            StoryManager.Instance.SetPhase(StoryPhase.ReturnAndReminder);
            EnsureStoryTimeAtLeast(ReminderStartHour, ReminderStartMinute);
            EnergySystem.Instance.PlayRestoreAnimation(dinnerRestoreEnergy, dinnerRestoreDuration);
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Show(ReturnReminderBridgePromptText);

            if (HasCompletedDialogueSequence(ReminderSequenceId))
            {
                _returnSequencePlayed = true;
                EnterFreeTime();
                return;
            }

            if (ShouldQueueDialogueSequence(ref _returnSequencePlayed, ReminderSequenceId))
            {
                PlayDialogueNowOrQueue(BuildReminderSequence());
            }
        }

        private void EnterFreeTime()
        {
            if (_freeTimeEntered)
            {
                return;
            }

            _freeTimeEntered = true;
            _freeTimeIntroCompleted = false;
            _freeTimeIntroQueued = false;
            _freeTimeNightWarningShown = false;
            _freeTimeMidnightWarningShown = false;
            _freeTimeFinalWarningShown = false;
            StoryManager.Instance.SetPhase(StoryPhase.FreeTime);
            EnsureStoryTimeAtLeast(FreeTimeStartHour, FreeTimeStartMinute);
            SyncStoryTimePauseState();
            SpringDay1PromptOverlay.Instance.Show(FreeTimeIntroBridgePromptText);

            if (HasCompletedDialogueSequence(FreeTimeIntroSequenceId))
            {
                _freeTimeIntroQueued = true;
                CompleteFreeTimeIntro();
                return;
            }

            if (!_freeTimeIntroQueued && !IsDialogueSequenceCurrentlyActive(FreeTimeIntroSequenceId))
            {
                _freeTimeIntroQueued = true;
                PlayDialogueNowOrQueue(BuildFreeTimeIntroSequence());
            }
        }

        public bool TryTriggerSleepFromBed()
        {
            if (!IsSleepInteractionAvailable())
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
                _ => "回屋休息"
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
                _ => "按 E 回屋休息，也可以先在村里再看看。"
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

        public string GetCurrentResidentBeatConsumptionSummary()
        {
            SpringDay1NpcCrowdManifest manifest = LoadCrowdManifest();
            if (manifest == null)
            {
                return "manifest-missing";
            }

            string beatKey = GetCurrentBeatKey();
            if (string.IsNullOrWhiteSpace(beatKey))
            {
                return "beat=inactive";
            }

            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot snapshot = manifest.BuildBeatConsumptionSnapshot(beatKey);
            string displayBeatKey = string.IsNullOrWhiteSpace(snapshot.beatKey) ? beatKey : snapshot.beatKey;
            return $"beat={displayBeatKey}|priority={FormatBeatConsumptionEntries(snapshot.priority)}|support={FormatBeatConsumptionEntries(snapshot.support)}|trace={FormatBeatConsumptionEntries(snapshot.trace)}|backstage={FormatBeatConsumptionEntries(snapshot.backstagePressure)}";
        }

        public string GetOneShotProgressSummary()
        {
            return
                $"healing={HasCompletedDialogueSequence(HealingSequenceId)}" +
                $"|workbench={HasCompletedDialogueSequence(WorkbenchSequenceId)}" +
                $"|dinner={HasCompletedDialogueSequence(DinnerSequenceId)}" +
                $"|reminder={HasCompletedDialogueSequence(ReminderSequenceId)}" +
                $"|freeTimeIntro={HasCompletedDialogueSequence(FreeTimeIntroSequenceId)}";
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

        private void ResetTownHouseLeadState()
        {
            _townHouseLeadStarted = false;
            _townHouseLeadTransitionQueued = false;
            _townHouseLeadWaitingForPlayer = false;
            _townHouseLeadNextMoveRetryAt = 0f;
            _townHouseLeadNextBubbleAt = 0f;
            _townHouseLeadCaughtUpSince = 0f;
            _townHouseLeadChiefName = string.Empty;
            _townHouseLeadCompanionName = string.Empty;
            _townHouseLeadTargetName = string.Empty;
            _townHouseLeadChiefDistance = float.PositiveInfinity;
            _townHouseLeadCompanionDistance = float.PositiveInfinity;
            _townHouseLeadPlayerDistance = float.PositiveInfinity;
            _townVillageGateCueWaitStartedAt = -1f;
        }

        private void ResetDinnerCueSettlementState()
        {
            _dinnerCueWaitStartedAt = -1f;
        }

        private void ResetPrimaryArrivalState()
        {
            _primaryArrivalActorsAligned = false;
            _primaryArrivalDialogueReadyAt = 0f;
        }

        private void ResetHealingBridgeState()
        {
            _healingBridgePending = false;
            _healingBridgeSequenceQueued = false;
            _healingBridgeHoldUntil = 0f;
            _healingBridgeNpcName = string.Empty;
            _healingBridgeNpcDistance = float.PositiveInfinity;
        }

        private void ResetWorkbenchEscortState()
        {
            _workbenchEscortStarted = false;
            _workbenchEscortWaitingForPlayer = false;
            _workbenchEscortIdlePlaced = false;
            _workbenchBriefingSequenceQueued = HasWorkbenchBriefingCompleted();
            _workbenchEscortNextMoveRetryAt = 0f;
            _workbenchEscortNextBubbleAt = 0f;
            _workbenchEscortCaughtUpSince = 0f;
            _workbenchEscortDialogueReadyAt = 0f;
            _workbenchEscortChiefName = string.Empty;
            _workbenchEscortCompanionName = string.Empty;
            _workbenchEscortTargetName = string.Empty;
            _workbenchEscortChiefDistance = float.PositiveInfinity;
            _workbenchEscortCompanionDistance = float.PositiveInfinity;
            _workbenchEscortPlayerDistance = float.PositiveInfinity;
        }

        private static bool HasWorkbenchBriefingCompleted()
        {
            return HasCompletedDialogueSequence(WorkbenchBriefingSequenceId);
        }

        private void ResetReturnEscortState()
        {
            _returnEscortStarted = false;
            _returnEscortTransitionQueued = false;
            _returnEscortWaitingForPlayer = false;
            _returnEscortNextMoveRetryAt = 0f;
            _returnEscortNextBubbleAt = 0f;
            _returnEscortCaughtUpSince = 0f;
            _returnEscortChiefName = string.Empty;
            _returnEscortCompanionName = string.Empty;
            _returnEscortTargetName = string.Empty;
            _returnEscortChiefDistance = float.PositiveInfinity;
            _returnEscortCompanionDistance = float.PositiveInfinity;
            _returnEscortPlayerDistance = float.PositiveInfinity;
        }

        private void MarkPrimaryArrivalActorsAligned()
        {
            if (_primaryArrivalActorsAligned)
            {
                return;
            }

            _primaryArrivalActorsAligned = true;
            _primaryArrivalDialogueReadyAt = Time.unscaledTime + primaryArrivalDialogueDelay;
        }

        private bool ResolveEscortWaitState(bool thresholdWait, bool currentlyWaiting, ref float caughtUpSince)
        {
            if (!currentlyWaiting)
            {
                caughtUpSince = 0f;
                return thresholdWait;
            }

            if (thresholdWait)
            {
                caughtUpSince = 0f;
                return true;
            }

            if (escortResumeConfirmationDuration <= 0f)
            {
                caughtUpSince = 0f;
                return false;
            }

            if (caughtUpSince <= 0f)
            {
                caughtUpSince = Time.unscaledTime;
                return true;
            }

            if (Time.unscaledTime - caughtUpSince < escortResumeConfirmationDuration)
            {
                return true;
            }

            caughtUpSince = 0f;
            return false;
        }

        private void EnsureSceneObjectCaches()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
            {
                ClearSceneObjectCaches();
                return;
            }

            if (_cachedSceneHandle == activeScene.handle)
            {
                return;
            }

            ClearSceneObjectCaches();
            _cachedSceneHandle = activeScene.handle;
        }

        private void ClearSceneObjectCaches()
        {
            _cachedSceneHandle = int.MinValue;
            _cachedChiefActor = null;
            _cachedCompanionActor = null;
            _cachedThirdResidentActor = null;
            _cachedWorkbenchTarget = null;
            _cachedTownLeadTarget = null;
            _cachedTownTransitionTrigger = null;
            _cachedPrimaryTownTransitionTrigger = null;
        }

        private Transform ResolveStoryChiefTransform()
        {
            return ResolveCachedStoryNpcTransform(
                ref _cachedChiefActor,
                ChiefNpcId,
                PreferredTownChiefObjectNames);
        }

        private Transform ResolveStoryCompanionTransform()
        {
            return ResolveCachedStoryNpcTransform(
                ref _cachedCompanionActor,
                CompanionNpcId,
                PreferredTownCompanionObjectNames);
        }

        private Transform ResolveStoryThirdResidentTransform()
        {
            return ResolveCachedStoryNpcTransform(
                ref _cachedThirdResidentActor,
                ThirdResidentNpcId,
                PreferredTownThirdResidentObjectNames);
        }

        private Transform ResolveCachedStoryNpcTransform(
            ref Transform cachedTransform,
            string npcId,
            string[] preferredObjectNames)
        {
            EnsureSceneObjectCaches();
            Scene activeScene = SceneManager.GetActiveScene();
            if (IsTransformCachedForScene(cachedTransform, activeScene))
            {
                return cachedTransform;
            }

            cachedTransform = FindPreferredStoryNpcTransform(npcId, preferredObjectNames);
            return cachedTransform;
        }

        private Transform ResolveWorkbenchCandidateCached()
        {
            EnsureSceneObjectCaches();
            Scene activeScene = SceneManager.GetActiveScene();
            if (IsTransformCachedForScene(_cachedWorkbenchTarget, activeScene))
            {
                return _cachedWorkbenchTarget;
            }

            _cachedWorkbenchTarget = FindWorkbenchCandidate();
            return _cachedWorkbenchTarget;
        }

        private SceneTransitionTrigger2D ResolveCachedTownTransitionTrigger()
        {
            return ResolveCachedSceneTransitionTrigger(
                ref _cachedTownTransitionTrigger,
                PrimarySceneName,
                PreferredTownTransitionTriggerObjectNames);
        }

        private SceneTransitionTrigger2D ResolveCachedPrimaryTownTransitionTrigger()
        {
            return ResolveCachedSceneTransitionTrigger(
                ref _cachedPrimaryTownTransitionTrigger,
                TownSceneName);
        }

        private SceneTransitionTrigger2D ResolveCachedSceneTransitionTrigger(
            ref SceneTransitionTrigger2D cachedTrigger,
            string targetSceneName,
            string[] preferredObjectNames = null)
        {
            EnsureSceneObjectCaches();
            Scene activeScene = SceneManager.GetActiveScene();
            if (cachedTrigger != null
                && cachedTrigger.gameObject.scene == activeScene
                && string.Equals(cachedTrigger.TargetSceneName, targetSceneName, StringComparison.Ordinal))
            {
                return cachedTrigger;
            }

            cachedTrigger = ResolveSceneTransitionTriggerByTargetScene(targetSceneName, preferredObjectNames);
            return cachedTrigger;
        }

        private Transform ResolveCachedTownLeadTarget(SceneTransitionTrigger2D transitionTrigger)
        {
            EnsureSceneObjectCaches();
            Scene activeScene = SceneManager.GetActiveScene();
            if (IsTransformCachedForScene(_cachedTownLeadTarget, activeScene))
            {
                return _cachedTownLeadTarget;
            }

            _cachedTownLeadTarget = ResolveTownLeadTarget(transitionTrigger);
            return _cachedTownLeadTarget;
        }

        private bool TryPrepareTownVillageGateActors()
        {
            if (!IsTownSceneActive())
            {
                return true;
            }

            Transform chief = ResolveStoryChiefTransform();
            Transform companion = ResolveStoryCompanionTransform();
            Transform thirdResident = ResolveStoryThirdResidentTransform();
            bool hasSceneAuthoredTargets = HasAnyTownVillageGateSceneAuthoring();

            TryResolveTownOpeningLayoutPoints(out Transform chiefPoint, out Transform companionPoint, out Transform thirdResidentPoint);

            Vector2 lookTarget = _playerMovement != null
                ? (Vector2)_playerMovement.transform.position
                : TryGetPlayerInteractionSamplePoint(out Vector2 playerSamplePoint)
                    ? playerSamplePoint
                    : Vector2.zero;
            bool preparedAnyActor = false;

            if (chief != null)
            {
                if (!TryResolveTownVillageGateActorTarget(
                        StoryNpcChiefLabel,
                        chief,
                        chiefPoint,
                        hasSceneAuthoredTargets,
                        out Vector3 chiefTarget))
                {
                    return false;
                }

                ReframeStoryActor(chief, chiefTarget, lookTarget);
                preparedAnyActor = true;
            }

            if (companion != null)
            {
                if (!TryResolveTownVillageGateActorTarget(
                        StoryNpcCompanionLabel,
                        companion,
                        companionPoint,
                        hasSceneAuthoredTargets,
                        out Vector3 companionTarget))
                {
                    return false;
                }

                ReframeStoryActor(companion, companionTarget, lookTarget);
                preparedAnyActor = true;
            }

            if (thirdResident != null)
            {
                if (!TryResolveTownVillageGateActorTarget(
                        ThirdResidentNpcId,
                        thirdResident,
                        thirdResidentPoint,
                        hasSceneAuthoredTargets,
                        out Vector3 thirdResidentTarget))
                {
                    return false;
                }

                ReframeStoryActor(thirdResident, thirdResidentTarget, lookTarget);
                preparedAnyActor = true;
            }

            return preparedAnyActor || !hasSceneAuthoredTargets;
        }

        private static bool IsTransformCachedForScene(Transform candidate, Scene scene)
        {
            return candidate != null && scene.IsValid() && candidate.gameObject.scene == scene;
        }

        private bool TryPreparePrimaryArrivalActors()
        {
            if (_primaryArrivalActorsAligned)
            {
                return !SceneTransitionRunner.IsBusy;
            }

            if (SceneTransitionRunner.IsBusy)
            {
                return false;
            }

            Transform chief = ResolveStoryChiefTransform();
            Transform companion = ResolveStoryCompanionTransform();
            if (TryResolvePrimaryEntryLayoutPoints(out Transform chiefPoint, out Transform companionPoint, out Transform playerPoint))
            {
                ApplyStoryActorEntryLayout(chief, chiefPoint, companion, companionPoint, playerPoint);
                MarkPrimaryArrivalActorsAligned();
                return true;
            }

            Transform arrivalAnchor = FindPreferredObjectTransform(PreferredPrimaryArrivalAnchorObjectNames);
            Vector2 referencePoint;
            if (arrivalAnchor != null)
            {
                referencePoint = arrivalAnchor.position;
            }
            else if (!TryGetPlayerInteractionSamplePoint(out referencePoint))
            {
                MarkPrimaryArrivalActorsAligned();
                return true;
            }

            Vector3 chiefTarget = BuildStoryActorReframeTarget(referencePoint, primaryArrivalChiefOffset, chief);
            Vector3 companionTarget = BuildStoryActorReframeTarget(referencePoint, primaryArrivalCompanionOffset, companion);
            if (!NeedsStoryActorReframe(chief, chiefTarget) && !NeedsStoryActorReframe(companion, companionTarget))
            {
                MarkPrimaryArrivalActorsAligned();
                return true;
            }

            bool blinkStarted = SceneTransitionRunner.TryBlink(
                () =>
                {
                    ReframeStoryActor(chief, chiefTarget, referencePoint);
                    ReframeStoryActor(companion, companionTarget, referencePoint);
                },
                storyBlinkFadeOutDuration,
                storyBlinkHoldDuration,
                storyBlinkFadeInDuration);
            if (!blinkStarted)
            {
                return false;
            }

            MarkPrimaryArrivalActorsAligned();
            return false;
        }

        private void ApplyStoryActorEntryLayout(
            Transform chiefActor,
            Transform chiefPoint,
            Transform companionActor,
            Transform companionPoint,
            Transform playerPoint)
        {
            Vector2 lookTarget = playerPoint != null
                ? (Vector2)playerPoint.position
                : TryGetPlayerInteractionSamplePoint(out Vector2 playerSamplePoint)
                    ? playerSamplePoint
                    : Vector2.zero;

            if (chiefActor != null && chiefPoint != null)
            {
                ReframeStoryActor(chiefActor, BuildStoryPointPosition(chiefPoint, chiefActor), lookTarget);
            }

            if (companionActor != null && companionPoint != null)
            {
                ReframeStoryActor(companionActor, BuildStoryPointPosition(companionPoint, companionActor), lookTarget);
            }

            MovePlayerToStoryPoint(playerPoint);
        }

        private Vector3 BuildStoryActorReframeTarget(Vector2 referencePoint, Vector2 offset, Transform actor)
        {
            float z = actor != null ? actor.position.z : 0f;
            return new Vector3(referencePoint.x + offset.x, referencePoint.y + offset.y, z);
        }

        private static Vector3 BuildStoryActorReframeTarget(Vector2 referencePoint, Transform actor)
        {
            float z = actor != null ? actor.position.z : 0f;
            return new Vector3(referencePoint.x, referencePoint.y, z);
        }

        private bool NeedsStoryActorReframe(Transform actor, Vector3 targetPosition)
        {
            return actor != null
                && Vector2.Distance(actor.position, targetPosition) > primaryArrivalReframeDistance;
        }

        private static bool HasAnyTownVillageGateSceneAuthoring()
        {
            return FindPreferredObjectTransform(PreferredTownOpeningLayoutGroupObjectNames) != null
                || FindPreferredObjectTransform(PreferredVillageCrowdMarkerRootObjectNames) != null;
        }

        private static bool TryResolveTownVillageGateActorTarget(
            string npcId,
            Transform actor,
            Transform explicitPoint,
            out Vector3 targetPosition)
        {
            return TryResolveTownVillageGateActorTarget(
                npcId,
                actor,
                explicitPoint,
                requireSceneAuthoredTarget: false,
                out targetPosition);
        }

        private static bool TryResolveTownVillageGateActorTarget(
            string npcId,
            Transform actor,
            Transform explicitPoint,
            bool requireSceneAuthoredTarget,
            out Vector3 targetPosition)
        {
            targetPosition = default;
            if (actor == null)
            {
                return false;
            }

            if (explicitPoint != null)
            {
                targetPosition = BuildStoryPointPosition(explicitPoint, actor);
                return true;
            }

            if (TryResolveVillageCrowdEndMarker(npcId, out Transform crowdEndMarker))
            {
                targetPosition = BuildStoryPointPosition(crowdEndMarker, actor);
                return true;
            }

            if (requireSceneAuthoredTarget)
            {
                return false;
            }

            if (!TryResolveTownVillageGateCueTarget(npcId, actor.position.z, out targetPosition))
            {
                return TryResolveTownVillageGateHardFallbackTarget(npcId, actor.position.z, out targetPosition);
            }

            return true;
        }

        private static bool TryResolveVillageCrowdEndMarker(string npcId, out Transform marker)
        {
            marker = null;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}终点");
            if (marker != null)
            {
                return true;
            }

            marker = FindPreferredNamedChildTransform(PreferredVillageCrowdMarkerRootObjectNames, $"{npcId}End");
            return marker != null;
        }

        private static bool TryResolveTownVillageGateCueTarget(string npcId, float actorZ, out Vector3 targetPosition)
        {
            targetPosition = default;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            SpringDay1DirectorStageBook stageBook = SpringDay1DirectorStagingDatabase.Load();
            SpringDay1DirectorBeatEntry beatEntry = stageBook != null
                ? stageBook.FindBeat(SpringDay1DirectorBeatKeys.EnterVillagePostEntry)
                : null;
            if (beatEntry?.actorCues == null)
            {
                return false;
            }

            SpringDay1DirectorActorCue cue = null;
            for (int index = 0; index < beatEntry.actorCues.Length; index++)
            {
                SpringDay1DirectorActorCue candidate = beatEntry.actorCues[index];
                if (candidate == null
                    || !string.Equals(candidate.npcId?.Trim(), npcId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                cue = candidate;
                break;
            }

            if (cue == null)
            {
                return false;
            }

            if (!TryResolveCueArrivalWorldPosition(cue, actorZ, out targetPosition))
            {
                return false;
            }

            return true;
        }

        private static bool TryResolveCueArrivalWorldPosition(
            SpringDay1DirectorActorCue cue,
            float actorZ,
            out Vector3 targetPosition)
        {
            targetPosition = default;
            if (cue == null)
            {
                return false;
            }

            if (!TryResolveCueStartWorldPosition(cue, actorZ, out Vector3 cueStartPosition))
            {
                return false;
            }

            SpringDay1DirectorPathPoint finalPoint = ResolveLastCuePoint(cue);
            if (finalPoint == null)
            {
                targetPosition = cueStartPosition;
                return true;
            }

            Vector2 target2D = finalPoint.position;
            if (cue.pathPointsAreOffsets)
            {
                target2D += new Vector2(cueStartPosition.x, cueStartPosition.y);
            }
            else if (cue.useSemanticAnchorAsStart
                     && cue.startPositionIsSemanticAnchorOffset
                     && SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(cue.semanticAnchorId, out Vector3 anchorWorldPosition))
            {
                target2D = new Vector2(anchorWorldPosition.x, anchorWorldPosition.y) + (finalPoint.position - cue.startPosition);
            }

            targetPosition = new Vector3(target2D.x, target2D.y, actorZ);
            return true;
        }

        private static bool TryResolveCueStartWorldPosition(
            SpringDay1DirectorActorCue cue,
            float actorZ,
            out Vector3 targetPosition)
        {
            targetPosition = default;
            if (cue == null || cue.keepCurrentSpawnPosition)
            {
                return false;
            }

            if (cue.useSemanticAnchorAsStart
                && SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(cue.semanticAnchorId, out Vector3 anchorWorldPosition))
            {
                Vector2 start2D = new Vector2(anchorWorldPosition.x, anchorWorldPosition.y);
                if (cue.startPositionIsSemanticAnchorOffset)
                {
                    start2D += cue.startPosition;
                }
                else if (cue.startPosition != Vector2.zero)
                {
                    start2D = cue.startPosition;
                }

                targetPosition = new Vector3(start2D.x, start2D.y, actorZ);
                return true;
            }

            targetPosition = new Vector3(cue.startPosition.x, cue.startPosition.y, actorZ);
            return true;
        }

        private static SpringDay1DirectorPathPoint ResolveLastCuePoint(SpringDay1DirectorActorCue cue)
        {
            if (cue?.path == null || cue.path.Length == 0)
            {
                return null;
            }

            for (int index = cue.path.Length - 1; index >= 0; index--)
            {
                if (cue.path[index] != null)
                {
                    return cue.path[index];
                }
            }

            return null;
        }

        private static bool TryResolveTownVillageGateHardFallbackTarget(string npcId, float actorZ, out Vector3 targetPosition)
        {
            targetPosition = default;
            switch (npcId?.Trim())
            {
                case StoryNpcChiefLabel:
                    targetPosition = new Vector3(-12.55f, 14.52f, actorZ);
                    return true;
                case StoryNpcCompanionLabel:
                    targetPosition = new Vector3(-10.91f, 16.86f, actorZ);
                    return true;
                case ThirdResidentNpcId:
                    targetPosition = new Vector3(-22.02f, 10.29f, actorZ);
                    return true;
                default:
                    return false;
            }
        }

        private static void ReframeStoryActor(Transform actor, Vector3 targetPosition, Vector2 lookTargetPosition)
        {
            if (actor == null)
            {
                return;
            }

            NPCAutoRoamController roamController = actor.GetComponent<NPCAutoRoamController>();
            if (roamController != null && !roamController.IsResidentScriptedControlActive)
            {
                roamController.AcquireResidentScriptedControl(DirectorResidentControlOwnerKey, resumeRoamWhenReleased: true);
            }

            HaltStoryActorNavigation(roamController);

            NPCMotionController motionController = actor.GetComponent<NPCMotionController>();
            motionController?.StopMotion();
            actor.position = targetPosition;

            if (motionController != null)
            {
                Vector2 facing = lookTargetPosition - (Vector2)targetPosition;
                if (facing.sqrMagnitude <= 0.0001f)
                {
                    facing = Vector2.down;
                }

                motionController.SetFacingDirection(facing);
            }
        }

        private static Vector3 BuildStoryPointPosition(Transform point, Transform actor)
        {
            Vector3 position = point.position;
            if (actor != null)
            {
                position.z = actor.position.z;
            }

            return position;
        }

        private void MovePlayerToStoryPoint(Transform playerPoint)
        {
            if (playerPoint == null)
            {
                return;
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                return;
            }

            Vector3 target = playerPoint.position;
            target.z = playerTransform.position.z;
            playerTransform.position = target;
        }

        private void UpdateTownHouseLeadSnapshot(
            out Transform chief,
            out NPCAutoRoamController chiefRoam,
            out NPCBubblePresenter chiefBubble,
            out Transform companion,
            out NPCAutoRoamController companionRoam,
            out Transform leadTarget,
            out SceneTransitionTrigger2D transitionTrigger)
        {
            chief = ResolveStoryChiefTransform();
            chiefRoam = chief != null ? chief.GetComponent<NPCAutoRoamController>() : null;
            chiefBubble = ResolveNpcBubblePresenter(chief);
            companion = ResolveStoryCompanionTransform();
            companionRoam = companion != null ? companion.GetComponent<NPCAutoRoamController>() : null;
            PromoteCompanionToEscortLeaderIfNeeded(ref chief, ref chiefRoam, ref chiefBubble, ref companion, ref companionRoam);
            transitionTrigger = ResolveCachedTownTransitionTrigger();
            leadTarget = ResolveCachedTownLeadTarget(transitionTrigger);

            _townHouseLeadChiefName = chief != null ? chief.name : "missing";
            _townHouseLeadCompanionName = companion != null ? companion.name : "missing";
            _townHouseLeadTargetName = leadTarget != null
                ? leadTarget.name
                : transitionTrigger != null
                    ? transitionTrigger.name
                    : "missing";
            _townHouseLeadChiefDistance = chief != null && leadTarget != null
                ? Vector2.Distance(chief.position, leadTarget.position)
                : float.PositiveInfinity;
            _townHouseLeadCompanionDistance = companion != null && leadTarget != null
                ? Vector2.Distance(companion.position, BuildEscortCompanionTarget(chief != null ? chief.position : companion.position, leadTarget.position))
                : float.PositiveInfinity;
            _townHouseLeadPlayerDistance = TryGetPlayerDistanceToEscortTarget(transitionTrigger, leadTarget, out float playerDistance)
                ? playerDistance
                : float.PositiveInfinity;
        }

        private void UpdateWorkbenchEscortSnapshot(
            out Transform chief,
            out NPCAutoRoamController chiefRoam,
            out NPCBubblePresenter chiefBubble,
            out Transform companion,
            out NPCAutoRoamController companionRoam,
            out Transform workbenchTarget)
        {
            chief = ResolveStoryChiefTransform();
            chiefRoam = chief != null ? chief.GetComponent<NPCAutoRoamController>() : null;
            chiefBubble = ResolveNpcBubblePresenter(chief);
            companion = ResolveStoryCompanionTransform();
            companionRoam = companion != null ? companion.GetComponent<NPCAutoRoamController>() : null;
            PromoteCompanionToEscortLeaderIfNeeded(ref chief, ref chiefRoam, ref chiefBubble, ref companion, ref companionRoam);
            workbenchTarget = ResolveWorkbenchCandidateCached();

            _workbenchEscortChiefName = chief != null ? chief.name : "missing";
            _workbenchEscortCompanionName = companion != null ? companion.name : "missing";
            _workbenchEscortTargetName = workbenchTarget != null ? workbenchTarget.name : "missing";
            _workbenchEscortChiefDistance = chief != null && workbenchTarget != null
                ? Vector2.Distance(chief.position, workbenchTarget.position)
                : float.PositiveInfinity;
            _workbenchEscortCompanionDistance = companion != null && workbenchTarget != null
                ? Vector2.Distance(companion.position, BuildEscortCompanionTarget(chief != null ? chief.position : companion.position, workbenchTarget.position))
                : float.PositiveInfinity;
            Transform playerReadyReference = chief != null ? chief : workbenchTarget;
            _workbenchEscortPlayerDistance = TryGetPlayerDistanceToEscortTarget(null, playerReadyReference, out float playerDistance)
                ? playerDistance
                : float.PositiveInfinity;
        }

        private void ResolveWorkbenchEscortTargets(
            Transform chief,
            Transform companion,
            Transform workbenchTarget,
            out Vector3 chiefTarget,
            out Vector3 companionTarget)
        {
            chiefTarget = chief != null && workbenchTarget != null
                ? BuildStoryPointPosition(workbenchTarget, chief)
                : chief != null
                    ? chief.position
                    : Vector3.zero;
            companionTarget = companion != null && workbenchTarget != null
                ? BuildEscortCompanionTarget(chief != null ? chief.position : companion.position, workbenchTarget.position)
                : companion != null
                    ? companion.position
                    : Vector3.zero;

            if (TryResolvePrimaryWorkbenchEscortPoints(out Transform chiefPoint, out Transform companionPoint))
            {
                if (chief != null && chiefPoint != null)
                {
                    chiefTarget = BuildStoryPointPosition(chiefPoint, chief);
                }

                if (companion != null && companionPoint != null)
                {
                    companionTarget = BuildStoryPointPosition(companionPoint, companion);
                }
            }

            _workbenchEscortChiefDistance = chief != null
                ? Vector2.Distance(chief.position, chiefTarget)
                : float.PositiveInfinity;
            _workbenchEscortCompanionDistance = companion != null
                ? Vector2.Distance(companion.position, companionTarget)
                : float.PositiveInfinity;
        }

        private void ApplyWorkbenchIdleLayout(Transform chief, Transform companion)
        {
            if (!TryResolvePrimaryWorkbenchIdlePoints(out Transform chiefPoint, out Transform companionPoint))
            {
                return;
            }

            Vector2 lookTarget = TryGetPlayerInteractionSamplePoint(out Vector2 playerSamplePoint)
                ? playerSamplePoint
                : Vector2.zero;
            if (chief != null && chiefPoint != null)
            {
                ReframeStoryActor(chief, BuildStoryPointPosition(chiefPoint, chief), lookTarget);
            }

            if (companion != null && companionPoint != null)
            {
                ReframeStoryActor(companion, BuildStoryPointPosition(companionPoint, companion), lookTarget);
            }
        }

        private bool TryIsPlayerReadyForTownHouseTransition(
            SceneTransitionTrigger2D transitionTrigger,
            Transform leadTarget,
            out float playerDistance)
        {
            return TryIsPlayerReadyForSceneTransition(transitionTrigger, leadTarget, townHouseLeadPlayerTransitionDistance, out playerDistance);
        }

        private bool TryGetPlayerDistanceToTownLeadTarget(
            SceneTransitionTrigger2D transitionTrigger,
            Transform leadTarget,
            out float distance)
        {
            return TryGetPlayerDistanceToEscortTarget(transitionTrigger, leadTarget, out distance);
        }

        private bool TryIsPlayerReadyForSceneTransition(
            SceneTransitionTrigger2D transitionTrigger,
            Transform leadTarget,
            float readyDistance,
            out float playerDistance)
        {
            playerDistance = float.PositiveInfinity;
            if (!TryGetPlayerDistanceToEscortTarget(transitionTrigger, leadTarget, out playerDistance))
            {
                return false;
            }

            return playerDistance <= readyDistance;
        }

        private bool AreEscortActorsReadyForTransition(
            Transform chief,
            Transform companion,
            Vector3 transitionTarget,
            float arrivalDistance)
        {
            if (chief != null && Vector2.Distance(chief.position, transitionTarget) > arrivalDistance)
            {
                return false;
            }

            if (companion == null)
            {
                return true;
            }

            Vector3 companionTarget = BuildEscortCompanionTarget(
                chief != null ? chief.position : companion.position,
                transitionTarget);
            return Vector2.Distance(companion.position, companionTarget) <= arrivalDistance;
        }

        private bool TryGetPlayerDistanceToEscortTarget(
            SceneTransitionTrigger2D transitionTrigger,
            Transform leadTarget,
            out float distance)
        {
            distance = float.PositiveInfinity;
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                return false;
            }

            Vector2 samplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
            Collider2D triggerCollider = transitionTrigger != null ? transitionTrigger.GetComponent<Collider2D>() : null;
            if (triggerCollider != null)
            {
                Vector2 closestPoint = triggerCollider.ClosestPoint(samplePoint);
                distance = Vector2.Distance(samplePoint, closestPoint);
                return true;
            }

            if (leadTarget == null)
            {
                return false;
            }

            distance = Vector2.Distance(samplePoint, leadTarget.position);
            return true;
        }

        private bool TryGetPlayerInteractionSamplePoint(out Vector2 samplePoint)
        {
            samplePoint = Vector2.zero;
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                return false;
            }

            samplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
            return true;
        }

        private static Transform ResolveTownLeadTarget(SceneTransitionTrigger2D transitionTrigger)
        {
            Transform explicitTarget = FindPreferredObjectTransform(PreferredTownLeadTargetObjectNames);
            return explicitTarget != null ? explicitTarget : transitionTrigger != null ? transitionTrigger.transform : null;
        }

        private static SceneTransitionTrigger2D ResolveTownTransitionTrigger()
        {
            return ResolveSceneTransitionTriggerByTargetScene(PrimarySceneName, PreferredTownTransitionTriggerObjectNames);
        }

        private static SceneTransitionTrigger2D ResolvePrimaryTownTransitionTrigger()
        {
            return ResolveSceneTransitionTriggerByTargetScene(TownSceneName);
        }

        private static SceneTransitionTrigger2D ResolveSceneTransitionTriggerByTargetScene(
            string targetSceneName,
            string[] preferredObjectNames = null)
        {
            if (preferredObjectNames != null)
            {
                Transform triggerTransform = FindPreferredObjectTransform(preferredObjectNames);
                if (triggerTransform != null)
                {
                    SceneTransitionTrigger2D directTrigger = triggerTransform.GetComponent<SceneTransitionTrigger2D>();
                    if (directTrigger != null && string.Equals(directTrigger.TargetSceneName, targetSceneName, StringComparison.Ordinal))
                    {
                        return directTrigger;
                    }
                }
            }

            SceneTransitionTrigger2D[] triggers = FindObjectsByType<SceneTransitionTrigger2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < triggers.Length; index++)
            {
                SceneTransitionTrigger2D trigger = triggers[index];
                if (trigger != null && string.Equals(trigger.TargetSceneName, targetSceneName, StringComparison.Ordinal))
                {
                    return trigger;
                }
            }

            return FindFirstObjectByType<SceneTransitionTrigger2D>(FindObjectsInactive.Include);
        }

        private static Transform FindPreferredObjectTransform(string[] preferredObjectNames)
        {
            if (preferredObjectNames == null)
            {
                return null;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                for (int nameIndex = 0; nameIndex < preferredObjectNames.Length; nameIndex++)
                {
                    string preferredName = preferredObjectNames[nameIndex];
                    if (string.IsNullOrWhiteSpace(preferredName))
                    {
                        continue;
                    }

                    for (int transformIndex = 0; transformIndex < allTransforms.Length; transformIndex++)
                    {
                        Transform candidate = allTransforms[transformIndex];
                        if (candidate == null
                            || candidate.gameObject.scene != activeScene
                            || !string.Equals(candidate.name, preferredName, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        return candidate;
                    }
                }
            }

            for (int index = 0; index < preferredObjectNames.Length; index++)
            {
                if (string.IsNullOrWhiteSpace(preferredObjectNames[index]))
                {
                    continue;
                }

                GameObject exactMatch = GameObject.Find(preferredObjectNames[index]);
                if (exactMatch != null)
                {
                    return exactMatch.transform;
                }
            }

            return null;
        }

        private static Transform FindPreferredNamedChildTransform(string[] parentObjectNames, string childName)
        {
            if (string.IsNullOrWhiteSpace(childName))
            {
                return null;
            }

            Transform parent = FindPreferredObjectTransform(parentObjectNames);
            if (parent == null)
            {
                return null;
            }

            Transform[] nested = parent.GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < nested.Length; index++)
            {
                Transform candidate = nested[index];
                if (candidate == null || candidate == parent)
                {
                    continue;
                }

                if (string.Equals(candidate.name, childName, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool TryResolvePrimaryEntryLayoutPoints(out Transform chiefPoint, out Transform companionPoint, out Transform playerPoint)
        {
            chiefPoint = FindPreferredNamedChildTransform(PreferredPrimaryEntryGroupObjectNames, PrimaryEntryChiefPointName);
            companionPoint = FindPreferredNamedChildTransform(PreferredPrimaryEntryGroupObjectNames, PrimaryEntryCompanionPointName);
            playerPoint = FindPreferredNamedChildTransform(PreferredPrimaryEntryGroupObjectNames, PrimaryEntryPlayerPointName);
            return chiefPoint != null || companionPoint != null || playerPoint != null;
        }

        private static bool TryResolveTownOpeningLayoutPoints(out Transform chiefPoint, out Transform companionPoint, out Transform thirdResidentPoint)
        {
            chiefPoint = FindPreferredNamedGrandchildTransform(PreferredTownOpeningLayoutGroupObjectNames, TownOpeningEndGroupName, TownOpeningChiefPointName);
            companionPoint = FindPreferredNamedGrandchildTransform(PreferredTownOpeningLayoutGroupObjectNames, TownOpeningEndGroupName, TownOpeningCompanionPointName);
            thirdResidentPoint = FindPreferredNamedGrandchildTransform(PreferredTownOpeningLayoutGroupObjectNames, TownOpeningEndGroupName, TownOpeningThirdResidentPointName);
            return chiefPoint != null || companionPoint != null || thirdResidentPoint != null;
        }

        private static bool TryResolvePrimaryWorkbenchEscortPoints(out Transform chiefPoint, out Transform companionPoint)
        {
            chiefPoint = FindPreferredNamedChildTransform(PreferredPrimaryWorkbenchEscortGroupObjectNames, StoryNpcChiefLabel);
            companionPoint = FindPreferredNamedChildTransform(PreferredPrimaryWorkbenchEscortGroupObjectNames, StoryNpcCompanionLabel);
            return chiefPoint != null || companionPoint != null;
        }

        private static Transform FindPreferredNamedGrandchildTransform(string[] preferredParentNames, string childGroupName, string targetName)
        {
            if (preferredParentNames == null || string.IsNullOrWhiteSpace(childGroupName) || string.IsNullOrWhiteSpace(targetName))
            {
                return null;
            }

            for (int index = 0; index < preferredParentNames.Length; index++)
            {
                Transform parent = FindPreferredObjectTransform(new[] { preferredParentNames[index] });
                if (parent == null)
                {
                    continue;
                }

                Transform childGroup = FindDirectChildTransform(parent, childGroupName);
                if (childGroup == null)
                {
                    continue;
                }

                Transform target = FindDirectChildTransform(childGroup, targetName);
                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }

        private static Transform FindDirectChildTransform(Transform parent, string childName)
        {
            if (parent == null || string.IsNullOrWhiteSpace(childName))
            {
                return null;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (string.Equals(child.name, childName, StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }

        private static bool TryResolvePrimaryWorkbenchIdlePoints(out Transform chiefPoint, out Transform companionPoint)
        {
            chiefPoint = FindPreferredNamedChildTransform(PreferredPrimaryWorkbenchIdleGroupObjectNames, StoryNpcChiefLabel);
            companionPoint = FindPreferredNamedChildTransform(PreferredPrimaryWorkbenchIdleGroupObjectNames, StoryNpcCompanionLabel);
            return chiefPoint != null || companionPoint != null;
        }

        private static Transform FindPreferredStoryNpcTransform(string npcId, string[] preferredObjectNames)
        {
            Transform sceneActor = FindSceneNpcTransformById(npcId);
            if (sceneActor != null)
            {
                return sceneActor;
            }

            Transform target = FindPreferredObjectTransform(preferredObjectNames);
            return IsLikelyNpcActorTransform(target) ? target : null;
        }

        private static Transform FindSceneNpcTransformById(string npcId)
        {
            string normalizedTargetId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
            if (string.IsNullOrWhiteSpace(normalizedTargetId))
            {
                return null;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
            {
                return null;
            }

            NPCAutoRoamController[] roamControllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < roamControllers.Length; index++)
            {
                NPCAutoRoamController candidate = roamControllers[index];
                if (candidate == null || candidate.gameObject.scene != activeScene)
                {
                    continue;
                }

                string candidateNpcId = candidate.RoamProfile != null
                    ? candidate.RoamProfile.ResolveNpcId(candidate.name)
                    : NPCDialogueContentProfile.NormalizeNpcId(candidate.name);
                if (string.Equals(candidateNpcId, normalizedTargetId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return candidate.transform;
                }
            }

            Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < allTransforms.Length; index++)
            {
                Transform candidate = allTransforms[index];
                if (candidate == null
                    || candidate.gameObject.scene != activeScene
                    || !IsLikelyNpcActorTransform(candidate))
                {
                    continue;
                }

                if (string.Equals(NPCDialogueContentProfile.NormalizeNpcId(candidate.name), normalizedTargetId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static bool IsLikelyNpcActorTransform(Transform candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            return candidate.GetComponent<NPCAutoRoamController>() != null
                || candidate.GetComponent<NPCMotionController>() != null
                || candidate.GetComponent<NPCDialogueInteractable>() != null
                || candidate.GetComponent<NPCInformalChatInteractable>() != null;
        }

        private static void SetStoryNpcActiveIfPresent(Transform target, bool shouldBeActive)
        {
            if (target == null)
            {
                return;
            }

            GameObject targetObject = target.gameObject;
            if (targetObject.activeSelf != shouldBeActive)
            {
                targetObject.SetActive(shouldBeActive);
            }
        }

        private static string FormatLeadDistance(float distance)
        {
            return float.IsInfinity(distance) ? "n/a" : distance.ToString("F2");
        }

        private bool ShouldEscortWaitForPlayer(float leaderDistance, float playerDistance, bool currentlyWaiting)
        {
            if (float.IsInfinity(leaderDistance) || float.IsInfinity(playerDistance))
            {
                return false;
            }

            if (playerDistance <= leaderDistance)
            {
                return false;
            }

            float waitThreshold = leaderDistance + escortMaxLeadDistance;
            if (!currentlyWaiting)
            {
                return playerDistance > waitThreshold;
            }

            float resumeThreshold = Mathf.Max(leaderDistance, waitThreshold - escortResumeLeadDistanceBuffer);
            return playerDistance > resumeThreshold;
        }

        private Vector3 BuildEscortCompanionTarget(Vector3 leaderPosition, Vector3 leadTargetPosition)
        {
            Vector2 direction = (Vector2)(leadTargetPosition - leaderPosition);
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector2.right;
            }
            else
            {
                direction.Normalize();
            }

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 target = (Vector2)leaderPosition - direction * escortCompanionTrailingOffset + perpendicular * escortCompanionSideOffset;
            return new Vector3(target.x, target.y, leaderPosition.z);
        }

        private bool TryDriveEscortActor(
            Transform actor,
            NPCAutoRoamController roamController,
            Vector3 targetPosition,
            float arrivalDistance)
        {
            if (actor == null)
            {
                return false;
            }

            Vector3 desired = targetPosition;
            desired.z = actor.position.z;
            float distance = Vector2.Distance(actor.position, desired);
            if (distance <= arrivalDistance)
            {
                StopStoryActorMotion(actor);
                return false;
            }

            if (roamController != null)
            {
                bool resumeRoamWhenReleased = roamController.IsResidentScriptedControlActive
                    ? roamController.ResumeRoamWhenResidentControlReleases
                    : true;
                if (roamController.DriveResidentScriptedMoveTo(
                        DirectorResidentControlOwnerKey,
                        desired,
                        resumeRoamWhenReleased,
                        Mathf.Max(escortRetargetTolerance, arrivalDistance * 0.5f)))
                {
                    return true;
                }

                HaltStoryActorNavigation(roamController);
                return false;
            }

            return NudgeStoryActorTowards(actor, desired);
        }

        private static bool NudgeStoryActorTowards(Transform actor, Vector3 targetPosition)
        {
            if (actor == null)
            {
                return false;
            }

            Vector3 delta = targetPosition - actor.position;
            delta.z = 0f;
            if (delta.sqrMagnitude <= 0.0001f)
            {
                StopStoryActorMotion(actor);
                return false;
            }

            NPCMotionController motionController = actor.GetComponent<NPCMotionController>();
            float moveSpeed = motionController != null ? Mathf.Max(0.5f, motionController.MoveSpeed) : 1.5f;
            float tickSeconds = 0.22f;
            float maxStep = moveSpeed * tickSeconds;
            Vector3 step = delta.sqrMagnitude <= maxStep * maxStep
                ? delta
                : delta.normalized * maxStep;

            Vector3 nextPosition = actor.position + step;
            Rigidbody2D body = actor.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.position = new Vector2(nextPosition.x, nextPosition.y);
            }

            actor.position = nextPosition;
            if (motionController != null)
            {
                Vector2 velocity = new Vector2(step.x, step.y) / tickSeconds;
                motionController.SetExternalVelocity(velocity);
                motionController.SetFacingDirection(new Vector2(step.x, step.y));
            }

            return true;
        }

        private static void StopStoryActorMotion(Transform actor)
        {
            if (actor == null)
            {
                return;
            }

            HaltStoryActorNavigation(actor.GetComponent<NPCAutoRoamController>());
            actor.GetComponent<NPCMotionController>()?.StopMotion();
        }

        private static void PauseStoryActorNavigation(NPCAutoRoamController roamController)
        {
            if (roamController == null)
            {
                return;
            }

            if (roamController.IsResidentScriptedControlActive)
            {
                roamController.PauseResidentScriptedMovement();
                return;
            }

            roamController.StopRoam();
        }

        private static void ResumeStoryActorNavigation(NPCAutoRoamController roamController)
        {
            if (roamController == null)
            {
                return;
            }

            if (roamController.IsResidentScriptedControlActive)
            {
                roamController.ResumeResidentScriptedMovement();
            }
        }

        private static void HaltStoryActorNavigation(NPCAutoRoamController roamController)
        {
            if (roamController == null)
            {
                return;
            }

            if (roamController.IsResidentScriptedControlActive)
            {
                roamController.HaltResidentScriptedMovement();
                return;
            }

            roamController.StopRoam();
        }

        private static NPCBubblePresenter ResolveNpcBubblePresenter(Transform actor)
        {
            if (actor == null)
            {
                return null;
            }

            return actor.GetComponent<NPCBubblePresenter>()
                ?? actor.GetComponentInChildren<NPCBubblePresenter>(true)
                ?? actor.GetComponentInParent<NPCBubblePresenter>();
        }

        private static void PromoteCompanionToEscortLeaderIfNeeded(
            ref Transform chief,
            ref NPCAutoRoamController chiefRoam,
            ref NPCBubblePresenter chiefBubble,
            ref Transform companion,
            ref NPCAutoRoamController companionRoam)
        {
            if (chief != null || companion == null)
            {
                return;
            }

            chief = companion;
            chiefRoam = companionRoam;
            chiefBubble = ResolveNpcBubblePresenter(companion);
            companion = null;
            companionRoam = null;
        }

        private static string ResolveEscortBridgePromptText(
            Transform chief,
            Transform companion,
            string bothPrompt,
            string chiefOnlyPrompt,
            string companionOnlyPrompt)
        {
            if (chief != null && companion != null)
            {
                return bothPrompt;
            }

            if (chief != null)
            {
                return chiefOnlyPrompt;
            }

            if (companion != null)
            {
                return companionOnlyPrompt;
            }

            return bothPrompt;
        }

        private void TryShowEscortWaitBubble(NPCBubblePresenter bubblePresenter, string content, ref float nextBubbleAt)
        {
            if (bubblePresenter == null ||
                !bubblePresenter.isActiveAndEnabled ||
                !bubblePresenter.gameObject.activeInHierarchy)
            {
                return;
            }

            string normalizedContent = string.IsNullOrWhiteSpace(content) ? string.Empty : content.Trim();
            if (bubblePresenter.IsConversationPriorityVisible &&
                string.Equals(bubblePresenter.LastPresentedText, normalizedContent, StringComparison.Ordinal))
            {
                bubblePresenter.ShowConversationText(normalizedContent, escortBubbleCooldown * 0.6f, restartFadeIn: false);
                nextBubbleAt = Time.unscaledTime + escortBubbleCooldown;
                return;
            }

            if (Time.unscaledTime < nextBubbleAt)
            {
                return;
            }

            if (bubblePresenter.ShowConversationText(normalizedContent, escortBubbleCooldown * 0.6f))
            {
                nextBubbleAt = Time.unscaledTime + escortBubbleCooldown;
            }
        }

        private void UpdateHealingBridgeSnapshot(out Transform supportNpc, out NPCAutoRoamController supportRoam)
        {
            supportNpc = ResolveStoryCompanionTransform();
            supportRoam = supportNpc != null ? supportNpc.GetComponent<NPCAutoRoamController>() : null;
            _healingBridgeNpcName = supportNpc != null ? supportNpc.name : "missing";
            _healingBridgeNpcDistance = float.PositiveInfinity;
        }

        private bool TrySnapValidationPlayerNearHealingSupport(Transform supportNpc, out float resultingDistance, out string message)
        {
            resultingDistance = float.PositiveInfinity;
            message = string.Empty;

            if (supportNpc == null)
            {
                message = "验收入口：未找到艾拉，当前无法继续疗伤桥验证。";
                return false;
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                message = "验收入口：缺少 PlayerMovement，当前无法把玩家推到艾拉身边。";
                return false;
            }

            Vector2 currentSamplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
            Vector2 supportPosition = supportNpc.position;
            Vector2 playerOffsetFromSupport = currentSamplePoint - supportPosition;
            if (playerOffsetFromSupport.sqrMagnitude <= 0.0001f)
            {
                playerOffsetFromSupport = Vector2.right;
            }
            else
            {
                playerOffsetFromSupport.Normalize();
            }

            Vector2 desiredSamplePoint = supportPosition + playerOffsetFromSupport * Mathf.Max(0.18f, healingSupportApproachDistance * 0.55f);
            Vector3 targetPlayerPosition = playerTransform.position + (Vector3)(desiredSamplePoint - currentSamplePoint);
            targetPlayerPosition.z = playerTransform.position.z;

            _playerMovement.StopMovement();
            _playerMovement.SetFacingDirection(supportPosition - desiredSamplePoint);

            Rigidbody2D playerBody = _playerMovement.GetComponent<Rigidbody2D>();
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector2.zero;
                playerBody.position = targetPlayerPosition;
            }
            else
            {
                playerTransform.position = targetPlayerPosition;
            }

            Physics2D.SyncTransforms();
            resultingDistance = Vector2.Distance(SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform), supportPosition);
            return true;
        }

        private bool TrySnapValidationPlayerNearEscortTarget(
            Transform escortTarget,
            float readyDistance,
            out float resultingDistance,
            out string message)
        {
            resultingDistance = float.PositiveInfinity;
            message = string.Empty;

            if (escortTarget == null)
            {
                message = "验收入口：缺少 escort 目标点，当前无法把玩家推到引路链附近。";
                return false;
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            Transform playerTransform = _playerMovement != null ? _playerMovement.transform : null;
            if (playerTransform == null)
            {
                message = "验收入口：缺少 PlayerMovement，当前无法把玩家推到引路链附近。";
                return false;
            }

            Vector2 currentSamplePoint = SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform);
            Vector2 escortPosition = escortTarget.position;
            Vector2 playerOffsetFromTarget = currentSamplePoint - escortPosition;
            if (playerOffsetFromTarget.sqrMagnitude <= 0.0001f)
            {
                playerOffsetFromTarget = Vector2.left;
            }
            else
            {
                playerOffsetFromTarget.Normalize();
            }

            Vector2 desiredSamplePoint = escortPosition + playerOffsetFromTarget * Mathf.Max(0.18f, readyDistance * 0.72f);
            Vector3 targetPlayerPosition = playerTransform.position + (Vector3)(desiredSamplePoint - currentSamplePoint);
            targetPlayerPosition.z = playerTransform.position.z;

            _playerMovement.StopMovement();
            _playerMovement.SetFacingDirection(escortPosition - desiredSamplePoint);

            Rigidbody2D playerBody = _playerMovement.GetComponent<Rigidbody2D>();
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector2.zero;
                playerBody.position = targetPlayerPosition;
            }
            else
            {
                playerTransform.position = targetPlayerPosition;
            }

            Physics2D.SyncTransforms();
            resultingDistance = Vector2.Distance(SpringDay1UiLayerUtility.GetInteractionSamplePoint(playerTransform), escortPosition);
            return true;
        }

        private Vector2 BuildHealingSupportTarget(Vector2 playerSamplePoint, Vector2 npcPosition)
        {
            Vector2 offsetDirection = npcPosition - playerSamplePoint;
            if (offsetDirection.sqrMagnitude <= 0.0001f)
            {
                offsetDirection = Vector2.right;
            }
            else
            {
                offsetDirection.Normalize();
            }

            return playerSamplePoint + offsetDirection * healingSupportApproachDistance;
        }

        private void UpdateReturnEscortSnapshot(
            out Transform chief,
            out NPCAutoRoamController chiefRoam,
            out NPCBubblePresenter chiefBubble,
            out Transform companion,
            out NPCAutoRoamController companionRoam,
            out SceneTransitionTrigger2D transitionTrigger)
        {
            chief = ResolveStoryChiefTransform();
            chiefRoam = chief != null ? chief.GetComponent<NPCAutoRoamController>() : null;
            chiefBubble = ResolveNpcBubblePresenter(chief);
            companion = ResolveStoryCompanionTransform();
            companionRoam = companion != null ? companion.GetComponent<NPCAutoRoamController>() : null;
            PromoteCompanionToEscortLeaderIfNeeded(ref chief, ref chiefRoam, ref chiefBubble, ref companion, ref companionRoam);
            transitionTrigger = ResolveCachedPrimaryTownTransitionTrigger();

            _returnEscortChiefName = chief != null ? chief.name : "missing";
            _returnEscortCompanionName = companion != null ? companion.name : "missing";
            _returnEscortTargetName = transitionTrigger != null ? transitionTrigger.name : "missing";
            _returnEscortChiefDistance = chief != null && transitionTrigger != null
                ? Vector2.Distance(chief.position, transitionTrigger.transform.position)
                : float.PositiveInfinity;
            _returnEscortCompanionDistance = companion != null && transitionTrigger != null
                ? Vector2.Distance(companion.position, BuildEscortCompanionTarget(chief != null ? chief.position : companion.position, transitionTrigger.transform.position))
                : float.PositiveInfinity;
            _returnEscortPlayerDistance = TryGetPlayerDistanceToEscortTarget(
                transitionTrigger,
                transitionTrigger != null ? transitionTrigger.transform : null,
                out float playerDistance)
                ? playerDistance
                : float.PositiveInfinity;
        }

        private void TryHandleReturnToTownEscort()
        {
            if (!IsReturnToTownEscortPending())
            {
                return;
            }

            bool startedNow = !_returnEscortStarted;
            if (startedNow)
            {
                _returnEscortStarted = true;
                _returnEscortTransitionQueued = false;
                _returnEscortWaitingForPlayer = false;
                _returnEscortNextMoveRetryAt = 0f;
                _returnEscortNextBubbleAt = 0f;
                _returnEscortCaughtUpSince = 0f;
            }

            UpdateReturnEscortSnapshot(
                out Transform chief,
                out NPCAutoRoamController chiefRoam,
                out NPCBubblePresenter chiefBubble,
                out Transform companion,
                out NPCAutoRoamController companionRoam,
                out SceneTransitionTrigger2D transitionTrigger);

            string bridgePromptText = ResolveEscortBridgePromptText(
                chief,
                companion,
                DinnerBridgePromptText,
                DinnerBridgeChiefOnlyPromptText,
                DinnerBridgeCompanionOnlyPromptText);

            if (startedNow)
            {
                SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
            }

            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return;
            }

            bool shouldWaitForPlayer = ResolveEscortWaitState(
                ShouldEscortWaitForPlayer(_returnEscortChiefDistance, _returnEscortPlayerDistance, _returnEscortWaitingForPlayer),
                _returnEscortWaitingForPlayer,
                ref _returnEscortCaughtUpSince);
            if (shouldWaitForPlayer)
            {
                if (!_returnEscortWaitingForPlayer)
                {
                    PauseStoryActorNavigation(chiefRoam);
                    PauseStoryActorNavigation(companionRoam);
                    SpringDay1PromptOverlay.Instance.Show(DinnerEscortWaitPromptText);
                }

                _returnEscortWaitingForPlayer = true;
                TryShowEscortWaitBubble(chiefBubble, DinnerEscortWaitPromptText, ref _returnEscortNextBubbleAt);
            }
            else
            {
                if (_returnEscortWaitingForPlayer)
                {
                    _returnEscortWaitingForPlayer = false;
                    _returnEscortCaughtUpSince = 0f;
                    _returnEscortNextMoveRetryAt = 0f;
                    ResumeStoryActorNavigation(chiefRoam);
                    ResumeStoryActorNavigation(companionRoam);
                    SpringDay1PromptOverlay.Instance.Show(bridgePromptText);
                }

                if (transitionTrigger != null && Time.unscaledTime >= _returnEscortNextMoveRetryAt)
                {
                    bool issuedMove = false;
                    if (chief != null)
                    {
                        if (TryDriveEscortActor(chief, chiefRoam, transitionTrigger.transform.position, townHouseLeadChiefArrivalDistance))
                        {
                            issuedMove = true;
                        }
                    }

                    if (companion != null)
                    {
                        Vector3 companionTarget = BuildEscortCompanionTarget(
                            chief != null ? chief.position : companion.position,
                            transitionTrigger.transform.position);
                        if (TryDriveEscortActor(companion, companionRoam, companionTarget, townHouseLeadChiefArrivalDistance))
                        {
                            issuedMove = true;
                        }
                    }

                    if (issuedMove)
                    {
                        _returnEscortNextMoveRetryAt = Time.unscaledTime + townHouseLeadMoveRetryInterval;
                    }
                }
            }

            if (_returnEscortTransitionQueued || transitionTrigger == null)
            {
                return;
            }

            if (!AreEscortActorsReadyForTransition(chief, companion, transitionTrigger.transform.position, townHouseLeadChiefArrivalDistance))
            {
                return;
            }

            if (TryIsPlayerReadyForSceneTransition(transitionTrigger, transitionTrigger.transform, townHouseLeadPlayerTransitionDistance, out float playerDistance))
            {
                _returnEscortPlayerDistance = playerDistance;
                if (transitionTrigger.TryStartTransition())
                {
                    _returnEscortTransitionQueued = true;
                    SpringDay1PromptOverlay.Instance.Show(DinnerEscortTransitionPromptText);
                }
            }
        }

        private static SpringDay1NpcCrowdManifest LoadCrowdManifest()
        {
            if (_cachedCrowdManifest == null)
            {
                _cachedCrowdManifest = Resources.Load<SpringDay1NpcCrowdManifest>(CrowdManifestResourcePath);
            }

            return _cachedCrowdManifest;
        }

        private static string FormatBeatConsumptionEntries(SpringDay1NpcCrowdManifest.BeatConsumptionEntry[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return "none";
            }

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < entries.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(',');
                }

                SpringDay1NpcCrowdManifest.BeatConsumptionEntry entry = entries[index];
                builder.Append(string.IsNullOrWhiteSpace(entry.npcId) ? "unknown" : entry.npcId.Trim());
                builder.Append('(');
                builder.Append(entry.presenceLevel);
                builder.Append(')');
            }

            return builder.ToString();
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

        private void PlayDialogueNowOrQueue(DialogueSequenceSO sequence)
        {
            if (sequence == null)
            {
                return;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && !dialogueManager.IsDialogueActive)
            {
                dialogueManager.PlayDialogue(sequence);
                return;
            }

            QueueDialogue(sequence);
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

        private DialogueSequenceSO BuildVillageGateSequence()
        {
            return ResolveDialogueSequence(VillageGateDialogueAssetPath, BuildVillageGateSequenceFallback);
        }

        private DialogueSequenceSO BuildHouseArrivalSequence()
        {
            return ResolveDialogueSequence(HouseArrivalDialogueAssetPath, BuildHouseArrivalSequenceFallback);
        }

        private DialogueSequenceSO BuildHealingSequence()
        {
            return ResolveDialogueSequence(HealingDialogueAssetPath, BuildHealingSequenceFallback);
        }

        private DialogueSequenceSO BuildWorkbenchSequence()
        {
            return ResolveDialogueSequence(WorkbenchDialogueAssetPath, BuildWorkbenchSequenceFallback);
        }

        private DialogueSequenceSO BuildPostTutorialWrapSequence()
        {
            return ResolveDialogueSequence(PostTutorialWrapDialogueAssetPath, BuildPostTutorialWrapSequenceFallback);
        }

        private DialogueSequenceSO BuildWorkbenchBriefingSequence()
        {
            return CreateSequence(
                WorkbenchBriefingSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "台子边上的箱子里，我先给你备了几样能上手的东西。别赤着手硬猜，先拿现成的练。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "先照着村长给你备好的东西做。你伤还没稳，别一上来就逞强乱试。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "等你把箱子和台子都摸明白，再动手开第一件。先学会照规矩做，再谈你以前会不会。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildDinnerSequence()
        {
            return ResolveDialogueSequence(DinnerDialogueAssetPath, BuildDinnerSequenceFallback);
        }

        private DialogueSequenceSO BuildReminderSequence()
        {
            return ResolveDialogueSequence(ReminderDialogueAssetPath, BuildReminderSequenceFallback);
        }

        private DialogueSequenceSO BuildFreeTimeIntroSequence()
        {
            return ResolveDialogueSequence(FreeTimeIntroDialogueAssetPath, BuildFreeTimeIntroSequenceFallback);
        }

        private DialogueSequenceSO ResolveDialogueSequence(string assetPath, Func<DialogueSequenceSO> fallbackFactory)
        {
            DialogueSequenceSO authoredSequence = TryLoadDialogueSequenceAsset(assetPath);
            return HasPlayableNodes(authoredSequence) ? authoredSequence : fallbackFactory();
        }

        private DialogueSequenceSO BuildVillageGateSequenceFallback()
        {
            return CreateSequence(
                VillageGateSequenceId,
                new DialogueNode
                {
                    speakerName = "村民",
                    text = "村长，你怎么带了个外乡人回来？",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "小孩",
                    text = "他真的是从矿洞那边出来的吗？身上全是灰……",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "先让路。人是我带回来的，今晚的事由我担着。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（所有人的目光都压到了我身上。这里先给了我一条路，但还没有谁真正把我当成自己人。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                });
        }

        private DialogueSequenceSO BuildHouseArrivalSequenceFallback()
        {
            return CreateSequence(
                HouseArrivalSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "先在这边站稳。围观的人虽然散了，可今晚还没人真把你算成自己人。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "我明白。你们只是先给我一个能站住脚的位置，至于能不能留下，还得看后面我能不能证明自己。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（村里的声响就在不远处压着我。比起矿洞那边的风声，这里已经算是活人该站着的地方了。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                });
        }

        private DialogueSequenceSO BuildHealingSequenceFallback()
        {
            return CreateSequence(
                HealingSequenceId,
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "先别动。我得先看看伤口裂到哪儿了，不然一会儿施术也压不住血。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "你们为什么愿意救我？我连自己为什么会掉在这里都说不清。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "救你，是因为你还活着，不是因为大家已经信任你了。先把伤稳下来。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "还有，你这身衣料、缝法，还有你说话的调子，都不像这附近的人。等你能坐稳了，我还要再问。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "我脑子里只剩碎片。石头、风声、还有一路被什么东西追着跑……剩下的全是空白。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "艾拉，先把人救下来。来历的事，等他今晚不会死在我们村里，再慢慢问。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "伤口我能先替你压住，但这不代表你已经没事了。今天别逞强，先学着看清自己还能撑到哪一步。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（温热的光顺着伤口压下来，疼痛像被一层厚布慢慢按住。至少今晚，我不会马上死在这里。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（那股暖意压住伤口的同时，意识里像被硬生生刻出一道清楚的刻度。我第一次真切知道，自己还剩下多少命。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "我好像……能感觉到自己还剩多少力气。像是身体里忽然多了一道不会说谎的底线。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "能感觉到底线就记牢。后面无论下地还是干活，都别再把自己耗到躺回去。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "既然命先拽回来了，就跟我去看看村里还能让你活下去的东西。空着手的人，在这里熬不过太久。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildWorkbenchSequenceFallback()
        {
            return CreateSequence(
                WorkbenchSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "村里能用的东西都在这儿了。算不上多好，但这已经是村里现在最顶用、也最基础的台子。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "坏掉的农具、旧木箱、补过的门闩，大家平时都是在这张台子上将就修出来的。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "老乔治总说，手上这点家伙什不漂亮，可只要还能拼出能用的东西，人就不算被逼到绝路。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（木料、绑绳、刀口角度……这些步骤像从雾里一块一块浮出来。我明明不该熟悉，却偏偏知道下一步是什么。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（更深处还有一点模糊的残影：更整齐的台面、更亮的火光、有人在我身后纠正过握刀的角度。可名字和脸全都抓不住。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "木料要先分软硬，绑绳和箱钉也不能混着算……不，对，先把门闩和木箱拆成两套。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "你连这些碎话都会顺嘴说出来？刚才那一瞬，你像忽然想起了别的地方的活法。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "你刚才还半死不活，这会儿倒像手先醒了。你不是临时瞎猜，你是真的做过这些。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "别把自己吓住。会这些就先拿来活下去，村里现在只要最基础、最顶用的那几样。老乔治见了，大概也只会嫌你手生。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "如果我明天还要留在这里，就先从这些最基础的活开始。我至少能替自己挣一口饭。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildPostTutorialWrapSequenceFallback()
        {
            return CreateSequence(
                PostTutorialWrapSequenceId,
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "白天该教你的活，你已经接住了。今晚别再把自己绷得太紧，先去把村里和住处都认一认。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "也就是说，我现在可以先四处看看？",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "可以。你要是嫌等到天黑太久，就回村里找我，我直接把晚饭那桌开起来；真拖到入夜，我也会把你叫回来。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "伤口今晚别再硬撑。认路可以，别把自己又拖回半死不活的样子。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildDinnerSequenceFallback()
        {
            return CreateSequence(
                DinnerSequenceId,
                new DialogueNode
                {
                    speakerName = "饭馆村民",
                    text = "先坐吧，汤还热着。折腾了一整天，总得先把肚子垫起来。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（热气总算把手指上的凉意赶开了一点。直到这一刻，我才像是真的吃上了今天第一顿饭。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "卡尔",
                    text = "凭什么让一个来历不明的人先吃村里的粮？矿洞那边捡回来的人，谁知道会带来什么。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "卡尔",
                    text = "白天把他带进村已经够冒险了，现在还要把吃的、药和住处都往他身上搭？村里不是拿来赌运气的。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "人是我带回来的。今晚这一口算在我头上。你要有话，明天再冲我来。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "艾拉",
                    text = "他现在连伤都没好利索。卡尔，你非要挑今晚跟他过不去吗？",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "如果一碗汤也算欠债，那我记下。等我能站稳，我会把今天吃下去的都还上。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "卡尔",
                    text = "我不是要你嘴上还。我只提醒你们，别因为一时心软，把村子往更麻烦的地方推。明天要是真能下地、能做活，再来谈你是不是累赘。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（桌边短暂安静了下来，没人再接话。热汤还在冒气，可那股敌意没有跟着散掉。这个村子只是暂时容下了我，还没有谁真正把我当成自己人。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（这顿饭把我留在了桌边，却也把明天压到了眼前。要不要把我算进这个村子，看来不是今晚一句话能定下来的。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                });
        }

        private DialogueSequenceSO BuildReminderSequenceFallback()
        {
            return CreateSequence(
                ReminderSequenceId,
                new DialogueNode
                {
                    speakerName = "",
                    text = "（摊板一扇扇合上，屋檐下的灯也比刚才更暗了。白天还热闹的村路，开始一截一截收回影子里。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "今晚别在外面拖太晚。真要睡，就回住处去，把门关好，别硬撑。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "白天那些人……好像都还在看着我。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "村子不怕穷，怕的是来路不明。先活过今晚，明天再慢慢让大家看清你。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "卡尔说得也没错。换成我，大概也不会轻易信一个从矿洞里摔出来的人。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "你能这么想，不算坏事。可今晚别再琢磨怎么证明自己，先把命保住。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "两点之后会怎样？",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "别拿这事试。夜里这片地，不喜欢还醒着的人。我没兴趣半夜出去把你从路边拖回来。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "还有，记牢了。两点之前必须睡下。这不是吓唬你的规矩。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村长",
                    text = "真到了那时候，路会像认不得人，风声也不像风声。你只要记住一件事：别让自己留在外面。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "旅人",
                    text = "……我记住了。今晚先把状态稳住，明天再想办法让他们看到我不是白吃白住。",
                    fontStyleKey = "default"
                });
        }

        private DialogueSequenceSO BuildFreeTimeIntroSequenceFallback()
        {
            return CreateSequence(
                FreeTimeIntroSequenceId,
                new DialogueNode
                {
                    speakerName = "",
                    text = "（村里的灯一盏一盏暗下去，只剩几处还留着人声和火光。屋门合上之后，这地方反而更像它本来的样子。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "村民",
                    text = "铁匠铺那边还亮着灯，老乔治今天怕是又要收得很晚。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村民",
                    text = "听说他明天还得去看村口那几把裂开的锄头，今晚不把火收利索，早上又要骂人。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村民",
                    text = "河边那只小船还没拴好，老汤姆多半又要摸黑收最后一趟。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "村民",
                    text = "老杰克刚从田埂那边回来，说今晚的露水气味不对。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "小米",
                    text = "娘说别老盯着那个外乡人，可大家都在偷偷看他。",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "小孩",
                    text = "那个外乡人还没睡啊……",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "小孩",
                    text = "他明天真的会去帮忙吗？",
                    fontStyleKey = "default"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（他们仍在用余光看我。可和白天比起来，那股警惕已经没那么锋利，像是这个村子先把我放进了门里，又没有真正把我算进去。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
                },
                new DialogueNode
                {
                    speakerName = "",
                    text = "（他们给我的，只是一块能先站住脚的地方。明天要不要把我算进去，还得看我能不能活过这一夜，也能不能在天亮后做出点什么。）",
                    isInnerMonologue = true,
                    fontStyleKey = "narration"
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

        private static DialogueSequenceSO TryLoadDialogueSequenceAsset(string assetPath)
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<DialogueSequenceSO>(assetPath);
#else
            return null;
#endif
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

            int readyCount = Mathf.Clamp(_workbenchCraftQueueCompleted, 0, Mathf.Max(1, _workbenchCraftQueueTotal));
            int percent = Mathf.RoundToInt(_workbenchCraftProgress * 100f);
            return _workbenchCraftQueueTotal > 1
                ? $"工作台制作中 · {_workbenchCraftRecipeName} · 进度 {readyCount}/{_workbenchCraftQueueTotal} · {percent}%"
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
                _ => "今晚可以先看看夜里的村子，但最晚两点前得回住处。"
            };
        }

        private string BuildFreeTimeTaskDetail()
        {
            return GetFreeTimePressureTier() switch
            {
                FreeTimePressureTier.FinalCall => "快到凌晨两点了，再拖会直接昏睡过去。",
                FreeTimePressureTier.AfterMidnight => "已经过了午夜，今晚该尽快收尾回去睡觉。",
                FreeTimePressureTier.NightWarning => "天已经黑透了，别在外面逗留太久。",
                _ => "今晚可以再看看夜里的村子，但最晚两点前必须回住处。"
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

        private static bool IsTownSceneActive()
        {
            return SceneManager.GetActiveScene().name == TownSceneName;
        }

        private static bool IsHomeSceneActive()
        {
            return SceneManager.GetActiveScene().name == HomeSceneName;
        }

        private static bool IsStoryRuntimeSceneActive()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return activeScene.name == PrimarySceneName
                || activeScene.name == TownSceneName
                || activeScene.name == HomeSceneName;
        }

        private void EnsureStoryHourAtLeast(int targetHour)
        {
            EnsureStoryTimeAtLeast(targetHour, 0);
        }

        private void EnsureStoryTimeAtLeast(int targetHour, int targetMinute)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            TimeManager timeManager = TimeManager.Instance;
            if (timeManager == null)
            {
                return;
            }

            int currentTotalMinutes = (timeManager.GetHour() * 60) + timeManager.GetMinute();
            int targetTotalMinutes = (Mathf.Max(0, targetHour) * 60) + Mathf.Clamp(targetMinute, 0, 59);
            if (currentTotalMinutes >= targetTotalMinutes)
            {
                return;
            }

            timeManager.SetTime(
                timeManager.GetYear(),
                timeManager.GetSeason(),
                timeManager.GetDay(),
                targetHour,
                Mathf.Clamp(targetMinute, 0, 59));
        }

        private static Transform FindWorkbenchCandidate()
        {
            Transform preferredWorkbench = FindPreferredObjectTransform(PreferredWorkbenchObjectNames);
            if (preferredWorkbench != null && preferredWorkbench.gameObject.activeInHierarchy)
            {
                return preferredWorkbench;
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
            Transform preferredBed = FindPreferredObjectTransform(PreferredBedObjectNames);
            if (preferredBed != null && preferredBed.gameObject.activeInHierarchy)
            {
                return RestTargetBinding.ForBed(preferredBed);
            }

            Transform preferredRestProxy = FindPreferredObjectTransform(PreferredRestProxyObjectNames);
            if (preferredRestProxy != null
                && preferredRestProxy.gameObject.activeInHierarchy
                && IsRestProxyCandidate(preferredRestProxy))
            {
                return RestTargetBinding.ForRestProxy(preferredRestProxy);
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

        private static bool IsBedCandidate(Transform candidate)
        {
            if (candidate == null || !candidate.gameObject.activeInHierarchy)
            {
                return false;
            }

            string loweredName = candidate.name.ToLowerInvariant();
            return loweredName.Contains("bed");
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
            TimeManager timeManager = Application.isPlaying
                ? TimeManager.Instance
                : FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
            if (timeManager == null)
            {
                return;
            }

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return;
            }

            StoryPhase currentPhase = storyManager.CurrentPhase;
            bool shouldPause = ShouldPauseStoryTimeForCurrentPhase(currentPhase);

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

        private bool ShouldPauseStoryTimeForCurrentPhase(StoryPhase currentPhase)
        {
            if (currentPhase == StoryPhase.None
                || currentPhase == StoryPhase.FreeTime
                || currentPhase == StoryPhase.DayEnd)
            {
                return false;
            }

            if (ShouldKeepStoryTimeRunningForRuntimeBridge(currentPhase))
            {
                return false;
            }

            return true;
        }

        private bool ShouldKeepStoryTimeRunningForRuntimeBridge(StoryPhase currentPhase)
        {
            return currentPhase switch
            {
                StoryPhase.EnterVillage => IsTownHouseLeadPending(),
                StoryPhase.HealingAndHP => _healingBridgePending && !_healingBridgeSequenceQueued,
                StoryPhase.WorkbenchFlashback => !_workbenchOpened,
                StoryPhase.DinnerConflict => IsReturnToTownEscortPending(),
                _ => false
            };
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
            StoryManager storyManager = StoryManager.Instance;
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
            NPCInformalChatInteractable npcInformal = npcInteractable != null
                ? npcInteractable.GetComponent<NPCInformalChatInteractable>()
                : FindPreferredComponent<NPCInformalChatInteractable>(PreferredNpcObjectNames);
            PlayerNpcChatSessionService npcChatSession = FindFirstObjectByType<PlayerNpcChatSessionService>(FindObjectsInactive.Include);
            PlayerNpcNearbyFeedbackService nearbyFeedbackService = FindFirstObjectByType<PlayerNpcNearbyFeedbackService>(FindObjectsInactive.Include);
            CraftingStationInteractable workbenchInteractable = FindPreferredComponent<CraftingStationInteractable>(PreferredWorkbenchObjectNames);
            SpringDay1WorkbenchCraftingOverlay workbenchOverlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
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
            AppendPair("BeatConsumption", director != null ? director.GetCurrentResidentBeatConsumptionSummary() : "n/a");
            AppendPair("OneShot", director != null ? director.GetOneShotProgressSummary() : "n/a");
            AppendPair("HealingBridge", director != null ? director.GetHealingBridgeSummary() : "n/a");
            AppendPair("WorldHint", director != null ? director.GetCurrentWorldHintSummary() : "n/a");
            AppendPair("PlayerFacing", director != null ? director.BuildPlayerFacingStatusSummary() : "n/a");
            AppendPair("TownLead", director != null ? director.GetTownHouseLeadSummary() : "n/a");
            AppendPair(
                "Director",
                director != null
                    ? $"{director.GetCurrentTaskLabel()}|{director.GetCurrentProgressLabel()}|followupPending={director.IsFirstFollowupPending()}|workbenchAwaiting={director.IsWorkbenchFlashbackAwaitingInteraction()}|dinnerPending={director.IsDinnerDialoguePendingStart()}|reminderPending={director.IsReminderDialoguePendingStart()}|sleepReady={director.IsSleepInteractionAvailable()}|nightPressure={director.GetFreeTimePressureState()}"
                    : "n/a");
            AppendPair("NPC", BuildNpcSummary(npcInteractable, npcRoam));
            AppendPair("NpcPrompt", BuildNpcPromptSummary(npcInformal, npcChatSession));
            AppendPair("NpcNearby", BuildNpcNearbySummary(nearbyFeedbackService));
            AppendPair("Workbench", workbenchInteractable != null ? workbenchInteractable.name : "n/a");
            AppendPair("WorkbenchUi", BuildWorkbenchUiSummary(workbenchOverlay));
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

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return "缺少 StoryManager，先执行 Bootstrap。";
            }

            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => "触发 NPC001 首段对话，接住醒来、危险感与撤离链。",
                StoryPhase.EnterVillage when director != null && director.IsTownHouseLeadPending()
                    => "执行 Step，请求进村引导切到 Primary，再自动接承接对白。",
                StoryPhase.EnterVillage => director != null && director.IsFirstFollowupPending()
                    ? "Town 会自动接进围观；围观收束后切到 Primary，再等承接对白自动接上。"
                    : "进村承接已收束，等待疗伤段启动。",
                StoryPhase.HealingAndHP => director != null
                    ? director.GetValidationHealingNextAction()
                    : "先走到艾拉身边，让疗伤对白和 HP 卡片接管。",
                StoryPhase.WorkbenchFlashback => director != null
                    ? director.GetValidationWorkbenchNextAction()
                    : "交互 Anvil_0 / Workbench，触发工作台闪回。",
                StoryPhase.FarmingTutorial => director != null
                    ? director.GetValidationFarmingNextAction()
                    : "执行 Step，模拟农田教学的最小推进。",
                StoryPhase.DinnerConflict when director != null && director.IsReturnEscortPendingForValidation()
                    => "执行 Step，请求回村转场，再让晚饭那一桌接管。",
                StoryPhase.DinnerConflict => director != null && director.IsDinnerDialoguePendingStart()
                    ? "晚餐对白已排队，等待接管。"
                    : "晚餐对白进行中，继续推进即可。",
                StoryPhase.ReturnAndReminder => director != null && director.IsReminderDialoguePendingStart()
                    ? "归途提醒对白已排队，等待接管。"
                    : "归途提醒对白进行中，继续推进即可。",
                StoryPhase.FreeTime => director != null
                    ? director.GetValidationFreeTimeNextAction()
                    : "自由时段尚未开放睡觉收束。",
                StoryPhase.DayEnd => "春1日已结束；下一轮该从“明天继续证明自己”这一拍开始复测。",
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

            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return SetActionResult("未找到 StoryManager；请先执行 Bootstrap。");
            }

            SpringDay1Director director = FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);

            return storyManager.CurrentPhase switch
            {
                StoryPhase.CrashAndMeet => SetActionResult(TryTriggerNpcDialogue()),
                StoryPhase.EnterVillage when director != null && director.IsTownHouseLeadPending()
                    => SetActionResult(director.TryRequestValidationEscortTransition()),
                StoryPhase.EnterVillage => SetActionResult(director != null && director.IsFirstFollowupPending()
                    ? director.TryAdvancePrimaryArrivalValidationStep()
                    : "进村安置已收束，等待疗伤段自动启动。"),
                StoryPhase.HealingAndHP => SetActionResult(director != null
                    ? director.TryAdvanceHealingValidationStep()
                    : "先走到艾拉身边，让疗伤对白和 HP 卡片接管。"),
                StoryPhase.WorkbenchFlashback => SetActionResult(director != null
                    ? director.TryAdvanceWorkbenchValidationStep()
                    : TryTriggerWorkbenchInteraction()),
                StoryPhase.FarmingTutorial => SetActionResult(director != null
                    ? director.TryAdvanceFarmingTutorialValidationStep()
                    : "当前缺少 Day1 导演层，无法推进农田教学验收入口。"),
                StoryPhase.DinnerConflict when director != null && director.IsReturnEscortPendingForValidation()
                    => SetActionResult(director.TryRequestValidationEscortTransition()),
                StoryPhase.DinnerConflict => SetActionResult(director != null && director.IsDinnerDialoguePendingStart()
                    ? "晚餐对白已排队，等待自动接管。"
                    : "晚餐对白进行中；继续执行 Step 即可推进。"),
                StoryPhase.ReturnAndReminder => SetActionResult(director != null && director.IsReminderDialoguePendingStart()
                    ? "归途提醒对白已排队，等待自动接管。"
                    : "归途提醒对白进行中；继续执行 Step 即可推进。"),
                StoryPhase.FreeTime => SetActionResult(director != null
                    ? director.TryAdvanceFreeTimeValidationStep()
                    : "自由时段尚未开放睡觉收束。"),
                StoryPhase.DayEnd => SetActionResult("春1日已结束；无需继续推进，下一轮从明天的承接复测开始。"),
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
            StoryManager storyManager = StoryManager.Instance;
            if (storyManager == null)
            {
                return false;
            }

            return storyManager.CurrentPhase == StoryPhase.CrashAndMeet
                || storyManager.CurrentPhase == StoryPhase.EnterVillage;
        }

        private static bool ShouldAllowRestValidationFallback()
        {
            StoryManager storyManager = StoryManager.Instance;
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

        private string BuildNpcSummary(NPCDialogueInteractable npcInteractable, NPCAutoRoamController npcRoam)
        {
            if (npcInteractable == null)
            {
                return "n/a";
            }

            string roam = npcRoam != null ? npcRoam.IsRoaming.ToString() : "n/a";
            string roamState = npcRoam != null ? npcRoam.DebugState : "n/a";
            string formalState = npcInteractable.GetFormalDialogueStateForCurrentStory().ToString();
            string yieldResident = npcInteractable.WillYieldToInformalResident().ToString();
            return $"{npcInteractable.name}|roam={roam}|state={roamState}|formal={formalState}|yieldResident={yieldResident}";
        }

        private string BuildNpcPromptSummary(NPCInformalChatInteractable npcInformal, PlayerNpcChatSessionService sessionService)
        {
            if (npcInformal == null)
            {
                return "n/a";
            }

            string caption = sessionService != null ? sessionService.GetPromptCaption(npcInformal) : "n/a";
            string detail = sessionService != null ? sessionService.GetPromptDetail(npcInformal) : "n/a";
            string state = sessionService != null ? sessionService.DebugStateName : "n/a";
            string activeNpc = sessionService != null ? sessionService.ActiveNpcName : "n/a";
            string playerBubble = sessionService != null ? sessionService.CurrentPlayerBubbleText : string.Empty;
            string npcBubble = sessionService != null ? sessionService.CurrentNpcBubbleText : string.Empty;
            return
                $"{npcInformal.name}" +
                $"|residentTone={npcInformal.ShouldUseResidentPromptTone()}" +
                $"|caption={Sanitize(caption)}" +
                $"|detail={Sanitize(detail)}" +
                $"|state={state}" +
                $"|activeNpc={Sanitize(activeNpc)}" +
                $"|playerBubble={Sanitize(playerBubble)}" +
                $"|npcBubble={Sanitize(npcBubble)}";
        }

        private string BuildNpcNearbySummary(PlayerNpcNearbyFeedbackService nearbyFeedbackService)
        {
            return nearbyFeedbackService != null ? nearbyFeedbackService.DebugSummary : "n/a";
        }

        private string BuildWorkbenchUiSummary(SpringDay1WorkbenchCraftingOverlay overlay)
        {
            return overlay != null ? overlay.GetRuntimeRecipeShellSummary() : "n/a";
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
