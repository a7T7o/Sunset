using System;
using System.Collections.Generic;
using System.Text;
using Sunset.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story
{
    [DefaultExecutionOrder(-300)]
    public sealed class SpringDay1NpcCrowdDirector : MonoBehaviour
    {
        private sealed class SpawnState
        {
            public GameObject Instance;
            public GameObject HomeAnchor;
            public bool OwnsInstance;
            public bool OwnsHomeAnchor;
            public string ResolvedAnchorName;
            public bool UsedFallback;
            public string BaseResolvedAnchorName;
            public bool BaseUsedFallback;
            public Vector3 BasePosition;
            public Vector2 BaseFacing;
            public string AppliedBeatKey;
            public string AppliedCueKey;
            public string ResidentGroupKey;
            public bool NeedsResidentReset;
            public bool ReleasedFromDirectorCue;
            public bool IsReturningHome;
            public bool ResumeRoamAfterReturn;
            public bool IsNightResting;
            public bool HideWhileNightResting;
            public float NextReturnHomeDriveRetryAt;
            public bool QueuedAutonomousResume;
            public bool QueuedAutonomousResumeTryImmediateMove;
            public float NextAutonomousResumeAt;
            public bool InteractionLockOwnedByCue;
            public bool FormalDialogueWasEnabledBeforeCue;
            public bool InformalDialogueWasEnabledBeforeCue;
        }

        [Serializable]
        public sealed class ResidentRuntimeSnapshotEntry
        {
            public string npcId = string.Empty;
            public string sceneName = string.Empty;
            public string residentGroupKey = string.Empty;
            public string resolvedAnchorName = string.Empty;
            public float positionX;
            public float positionY;
            public float facingX;
            public float facingY;
            public bool isActive = true;
            public bool isReturningHome;
            public bool underDirectorCue;
        }

        [Serializable]
        public sealed class ResidentControlProbeEntry
        {
            public string npcId = string.Empty;
            public bool isActive;
            public string residentGroupKey = string.Empty;
            public string resolvedAnchorName = string.Empty;
            public string baseResolvedAnchorName = string.Empty;
            public string appliedBeatKey = string.Empty;
            public string appliedCueKey = string.Empty;
            public bool needsResidentReset;
            public bool releasedFromDirectorCue;
            public bool isReturningHome;
            public bool resumeRoamAfterReturn;
            public bool isNightResting;
            public float positionX;
            public float positionY;
            public float basePositionX;
            public float basePositionY;
            public bool hasHomeAnchor;
            public float homeAnchorX;
            public float homeAnchorY;
            public bool roamControllerEnabled;
            public bool isRoaming;
            public bool isMoving;
            public bool scriptedControlActive;
            public string scriptedControlOwnerKey = string.Empty;
            public bool scriptedMoveActive;
            public bool scriptedMovePaused;
            public bool resumeRoamWhenResidentControlReleases;
            public string roamState = string.Empty;
            public string lastMoveSkipReason = string.Empty;
            public int blockedAdvanceFrames;
            public int consecutivePathBuildFailures;
            public int sharedAvoidanceBlockingFrames;
            public bool hasSharedAvoidanceDetour;
        }

        private readonly struct SpawnPointResolution
        {
            public SpawnPointResolution(Vector3 position, string anchorName, bool usedFallback)
            {
                Position = position;
                AnchorName = anchorName;
                UsedFallback = usedFallback;
            }

            public Vector3 Position { get; }
            public string AnchorName { get; }
            public bool UsedFallback { get; }
        }

        private const string PrimarySceneName = "Primary";
        private const string TownSceneName = "Town";
        private const string RuntimeRootName = "SpringDay1NpcCrowdRuntimeRoot";
        private const string StoryEscortChiefNpcId = "001";
        private const string StoryEscortCompanionNpcId = "002";
        private const string ThirdResidentNpcId = "003";
        private const string VillageCrowdMarkerStartSuffixCn = "起点";
        private const string VillageCrowdMarkerEndSuffixCn = "终点";
        private const string ManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";
        private const float ResidentReturnHomeThreshold = 0.05f;
        private const float ResidentReturnHomeArrivalRadius = 0.64f;
        private const float ResidentReturnHomeRetryInterval = 0.35f;
        private const float TownResidentRecoveryRetryInterval = 0.05f;
        private const float DaytimeAutonomyYieldRetryInterval = 0.05f;
        private const int MaxTownResidentRecoveriesPerTick = 3;
        private const int MaxDaytimeAutonomyReleasesPerTick = 3;
        private const int MaxQueuedAutonomousResumesPerTick = 2;
        private const float QueuedAutonomousResumeBaseDelay = 0.08f;
        private const float QueuedAutonomousResumeJitterSeconds = 0.28f;
        private const float QueuedAutonomousResumeDispatchInterval = 0.08f;
        private const float StorySceneMarkerWalkSpeed = 2.5f;
        private const int ResidentReturnHomeHour = 20;
        private const int ResidentForcedRestHour = 21;
        private const int ResidentMorningReleaseHour = 7;
        private const string RestoredDirectorCuePlaceholderKey = "__restored-director-cue__";
        private const string ResidentScriptedControlOwnerKey = nameof(SpringDay1NpcCrowdDirector);
        private const string DirectorResidentScriptedControlOwnerKey = "spring-day1-director";
        private static readonly string[] VillageCrowdMarkerRootNames = { "进村围观" };
        private static readonly string[] VillageCrowdMarkerStartGroupNames = { "起点", "Start" };
        private static readonly string[] VillageCrowdMarkerEndGroupNames = { "终点", "End" };

        private static SpringDay1NpcCrowdDirector _instance;

        private readonly Dictionary<string, SpawnState> _spawnStates = new Dictionary<string, SpawnState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ResidentRuntimeSnapshotEntry> _residentRuntimeSnapshotCache = new Dictionary<string, ResidentRuntimeSnapshotEntry>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, GameObject> _sceneGameObjectLookupCache = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Transform> _sceneTransformLookupCache = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _sceneMissingTransformLookupCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _liveIdsScratch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _staleIdsScratch = new List<string>();

        private SpringDay1NpcCrowdManifest _manifest;
        private int _sceneLookupCacheHandle = -1;
        private float _nextSyncAt;
        private float _nextQueuedAutonomousResumeDispatchAt;
        private bool _syncRequested = true;
        private bool _enterVillageCrowdReleaseLatched;

        public static string CurrentRuntimeSummary => _instance != null ? _instance.BuildRuntimeSummary() : "not-created";
        public static bool IsBeatCueSettled(string beatKey)
        {
            return _instance == null || _instance.AreBeatCuesSettled(beatKey);
        }

        public static List<ResidentRuntimeSnapshotEntry> CaptureResidentRuntimeSnapshots()
        {
            EnsureRuntime();
            return _instance != null
                ? _instance.CaptureResidentRuntimeSnapshotsInternal()
                : new List<ResidentRuntimeSnapshotEntry>();
        }

        public static void ApplyResidentRuntimeSnapshots(IReadOnlyList<ResidentRuntimeSnapshotEntry> snapshots)
        {
            EnsureRuntime();
            _instance?.ApplyResidentRuntimeSnapshotsInternal(snapshots);
        }

        public static void ClearResidentRuntimeSnapshots()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.ClearResidentRuntimeSnapshotsInternal();
        }

        public static void SnapResidentsToHomeAnchors()
        {
            EnsureRuntime();
            _instance?.SnapResidentsToHomeAnchorsInternal();
        }

        public static List<ResidentControlProbeEntry> CaptureResidentControlProbe()
        {
            EnsureRuntime();
            return _instance != null
                ? _instance.CaptureResidentControlProbeInternal()
                : new List<ResidentControlProbeEntry>();
        }

        public static void ForceImmediateSync()
        {
            EnsureRuntime();
            if (_instance == null || !IsRuntimeSceneActive())
            {
                return;
            }

            _instance._syncRequested = false;
            _instance._nextSyncAt = Time.unscaledTime + 0.35f;
            _instance.SyncCrowd();
        }

        public static bool ForceSettleBeatCue(string beatKey)
        {
            EnsureRuntime();
            if (_instance == null || !IsRuntimeSceneActive())
            {
                return false;
            }

            return _instance.ForceSettleBeatCueInternal(beatKey);
        }

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            SpringDay1NpcCrowdDirector existing = FindFirstObjectByType<SpringDay1NpcCrowdDirector>(FindObjectsInactive.Include);
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            GameObject runtimeObject = new GameObject(nameof(SpringDay1NpcCrowdDirector));
            _instance = runtimeObject.AddComponent<SpringDay1NpcCrowdDirector>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            EnsureRuntime();
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
            hideFlags = HideFlags.DontSave;
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += HandleActiveSceneChanged;
            SceneManager.sceneLoaded += HandleSceneLoaded;
            EventBus.Subscribe<StoryPhaseChangedEvent>(HandleStoryPhaseChanged, owner: this);
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            EventBus.UnsubscribeAll(this);
            TeardownAll();
        }

        private void Update()
        {
            if (!IsRuntimeSceneActive())
            {
                TeardownAll();
                return;
            }

            RefreshEnterVillageCrowdReleaseLatch();
            Scene activeScene = SceneManager.GetActiveScene();
            if (ShouldStandDownDuringPostTutorialTownAutonomy(activeScene))
            {
                CancelQueuedAutonomousResumes();
                return;
            }

            TickQueuedAutonomousResumes();
            if (ShouldStandDownAfterEnterVillageCrowdRelease(activeScene)
                && !HasPendingDaytimeAutonomyRelease()
                && !HasPendingQueuedAutonomousResume())
            {
                return;
            }

            if (ShouldRecoverMissingTownResidents(activeScene))
            {
                if (!_syncRequested && Time.unscaledTime < _nextSyncAt)
                {
                    return;
                }

                SyncCrowd();
                bool hasMorePendingTownRecovery = RecoverReleasedTownResidentsWithoutSpawnStates(activeScene);
                _syncRequested = hasMorePendingTownRecovery;
                _nextSyncAt = Time.unscaledTime + (hasMorePendingTownRecovery
                    ? TownResidentRecoveryRetryInterval
                    : 0.1f);
                return;
            }

            SyncResidentNightRestSchedule();
            TickResidentReturns();

            if (ShouldYieldDaytimeResidentsToAutonomy())
            {
                bool hasPendingDaytimeAutonomyRelease = HasPendingDaytimeAutonomyRelease();
                if (!hasPendingDaytimeAutonomyRelease && !ShouldResyncBeforeSkippingDaytimeAutonomyYield())
                {
                    return;
                }

                if (!_syncRequested && Time.unscaledTime < _nextSyncAt)
                {
                    return;
                }

                if (hasPendingDaytimeAutonomyRelease)
                {
                    bool hasMorePendingDaytimeAutonomyRelease = YieldDaytimeResidentsToAutonomy();
                    _syncRequested = hasMorePendingDaytimeAutonomyRelease;
                    _nextSyncAt = Time.unscaledTime + (hasMorePendingDaytimeAutonomyRelease
                        ? DaytimeAutonomyYieldRetryInterval
                        : 0.35f);
                }
                else
                {
                    _syncRequested = false;
                    _nextSyncAt = Time.unscaledTime + 0.35f;
                    SyncCrowd();
                }
                return;
            }

            if (!_syncRequested && Time.unscaledTime < _nextSyncAt)
            {
                return;
            }

            _nextSyncAt = Time.unscaledTime + 0.35f;
            _syncRequested = false;
            SyncCrowd();
        }

        private bool ShouldRecoverMissingTownResidents(Scene activeScene)
        {
            if (!activeScene.IsValid()
                || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal))
            {
                return false;
            }

            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            if (StoryManager.Instance == null || SpringDay1Director.Instance == null || manifest == null)
            {
                return false;
            }

            if (!ShouldResumeReleasedTownResidents())
            {
                return false;
            }

            SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries ?? Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            for (int index = 0; index < entries.Length; index++)
            {
                if (NeedsReleasedResidentRecovery(entries[index], activeScene))
                {
                    return true;
                }
            }

            return false;
        }

        private bool RecoverReleasedTownResidentsWithoutSpawnStates(Scene activeScene)
        {
            if (!activeScene.IsValid()
                || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal))
            {
                return false;
            }

            if (!ShouldResumeReleasedTownResidents())
            {
                return false;
            }

            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            bool recoveredAnyState = false;
            int recoveredCount = 0;
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (!NeedsReleasedResidentRecovery(entry, activeScene))
                {
                    continue;
                }

                if (recoveredCount >= MaxTownResidentRecoveriesPerTick)
                {
                    return true;
                }

                if (TryRecoverReleasedResidentState(entry, activeScene))
                {
                    recoveredAnyState = true;
                    recoveredCount++;
                }
            }

            if (recoveredAnyState)
            {
                _syncRequested = true;
            }

            return false;
        }

        private bool ShouldResumeReleasedTownResidents()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            StoryPhase currentPhase = ResolveCurrentPhase();
            string currentBeatKey = NormalizeResidentRuntimeBeatKey(currentPhase, ResolveCurrentBeatKey());
            return currentPhase > StoryPhase.EnterVillage
                || string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.EnterVillageHouseArrival, StringComparison.OrdinalIgnoreCase)
                || director != null && director.ShouldLatchEnterVillageCrowdRelease();
        }

        private bool NeedsReleasedResidentRecovery(SpringDay1NpcCrowdManifest.Entry entry, Scene activeScene)
        {
            if (entry == null
                || string.IsNullOrWhiteSpace(entry.npcId)
                || _spawnStates.ContainsKey(entry.npcId))
            {
                return false;
            }

            GameObject instance = FindSceneResident(entry, activeScene);
            if (instance == null || !instance.activeInHierarchy)
            {
                return false;
            }

            SpringDay1DirectorStagingPlayback playback = instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback != null && !string.IsNullOrWhiteSpace(playback.CurrentCueKey))
            {
                return false;
            }

            NPCAutoRoamController roamController = instance.GetComponent<NPCAutoRoamController>();
            return roamController == null || !roamController.IsResidentScriptedControlActive;
        }

        private bool TryRecoverReleasedResidentState(SpringDay1NpcCrowdManifest.Entry entry, Scene activeScene)
        {
            if (entry == null
                || string.IsNullOrWhiteSpace(entry.npcId)
                || _spawnStates.ContainsKey(entry.npcId))
            {
                return false;
            }

            GameObject instance = FindSceneResident(entry, activeScene);
            if (instance == null || !instance.activeInHierarchy)
            {
                return false;
            }

            SpringDay1DirectorStagingPlayback playback = instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback != null && !string.IsNullOrWhiteSpace(playback.CurrentCueKey))
            {
                return false;
            }

            NPCAutoRoamController roamController = instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null && roamController.IsResidentScriptedControlActive)
            {
                return false;
            }

            Transform homeAnchor = FindSceneResidentHomeAnchor(entry, activeScene, instance.transform);
            ConfigureBoundNpc(entry, instance, homeAnchor);

            string resolvedAnchorName = homeAnchor != null ? homeAnchor.name : instance.name;
            SpawnState state = new SpawnState
            {
                Instance = instance,
                HomeAnchor = homeAnchor != null ? homeAnchor.gameObject : null,
                OwnsInstance = false,
                OwnsHomeAnchor = false,
                ResolvedAnchorName = resolvedAnchorName,
                UsedFallback = false,
                BaseResolvedAnchorName = resolvedAnchorName,
                BaseUsedFallback = false,
                BasePosition = instance.transform.position,
                BaseFacing = entry.initialFacing,
                AppliedBeatKey = string.Empty,
                AppliedCueKey = string.Empty,
                ResidentGroupKey = string.Empty,
                NeedsResidentReset = false,
                ReleasedFromDirectorCue = false,
                IsReturningHome = false,
                ResumeRoamAfterReturn = false,
                IsNightResting = false,
                NextReturnHomeDriveRetryAt = 0f
            };
            _spawnStates[entry.npcId] = state;

            if (roamController != null)
            {
                // 返场补绑不能只把 NPC 纳回 crowd 视图；否则会把 controller 留在
                // “表面仍在 roam，但内部 path/recovery 已坏”的旧 runtime 上。
                QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);
            }

            return true;
        }

        private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
        {
            CaptureSceneResidentRuntimeSnapshotCache(previousScene);
            InvalidateSceneLookupCache();
            _syncRequested = true;
            if (!IsSupportedRuntimeScene(nextScene))
            {
                TeardownAll();
            }
        }

        private void HandleSceneLoaded(Scene _, LoadSceneMode __)
        {
            InvalidateSceneLookupCache();
            _syncRequested = true;
        }

        private void HandleStoryPhaseChanged(StoryPhaseChangedEvent _)
        {
            _syncRequested = true;
            if (ResolveCurrentPhase() != StoryPhase.EnterVillage)
            {
                _enterVillageCrowdReleaseLatched = false;
            }
        }

        private void RefreshEnterVillageCrowdReleaseLatch()
        {
            StoryPhase currentPhase = ResolveCurrentPhase();
            if (currentPhase != StoryPhase.EnterVillage)
            {
                _enterVillageCrowdReleaseLatched = false;
                return;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null && director.ShouldLatchEnterVillageCrowdRelease())
            {
                _enterVillageCrowdReleaseLatched = true;
            }
        }

        private bool ShouldStandDownAfterEnterVillageCrowdRelease(Scene activeScene)
        {
            if (!_enterVillageCrowdReleaseLatched
                || !activeScene.IsValid()
                || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal))
            {
                return false;
            }

            if (ShouldResidentsRestByClock() || ShouldResidentsReturnHomeByClock())
            {
                return false;
            }

            return ResolveCurrentPhase() == StoryPhase.EnterVillage;
        }

        private bool ShouldYieldDaytimeResidentsToAutonomy()
        {
            if (_spawnStates.Count <= 0)
            {
                return false;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal))
            {
                return false;
            }

            if (ShouldResidentsRestByClock() || ShouldResidentsReturnHomeByClock())
            {
                return false;
            }

            StoryPhase currentPhase = ResolveCurrentPhase();
            if (currentPhase < StoryPhase.EnterVillage || currentPhase >= StoryPhase.DayEnd)
            {
                return false;
            }

            // opening/house-lead 期间不能提前把 Town resident 放回自由漫游；
            // 只有 village gate 真正收束后，白天 resident 才恢复全图自治。
            if (currentPhase == StoryPhase.EnterVillage)
            {
                return false;
            }

            string currentBeatKey = ResolveCurrentBeatKey();
            if (string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.DayEndSettle, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.DailyStandPreview, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(currentBeatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase))
            {
                return ShouldSuppressEnterVillageCrowdCueForTownHouseLead(currentBeatKey);
            }

            return true;
        }

        private bool YieldDaytimeResidentsToAutonomy()
        {
            if (_spawnStates.Count <= 0)
            {
                return false;
            }

            int releasedCount = 0;
            foreach (KeyValuePair<string, SpawnState> pair in _spawnStates)
            {
                SpawnState state = pair.Value;
                if (state?.Instance == null)
                {
                    continue;
                }

                if (!NeedsDaytimeAutonomyRelease(state))
                {
                    continue;
                }

                if (releasedCount >= MaxDaytimeAutonomyReleasesPerTick)
                {
                    return true;
                }

                ReleaseResidentToAutonomousRoam(state);
                releasedCount++;
            }

            return false;
        }

        private bool HasPendingDaytimeAutonomyRelease()
        {
            foreach (SpawnState state in _spawnStates.Values)
            {
                if (NeedsDaytimeAutonomyRelease(state))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasPendingQueuedAutonomousResume()
        {
            foreach (SpawnState state in _spawnStates.Values)
            {
                if (state != null && state.QueuedAutonomousResume)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldResyncBeforeSkippingDaytimeAutonomyYield()
        {
            if (_syncRequested)
            {
                return true;
            }

            foreach (SpawnState state in _spawnStates.Values)
            {
                if (state == null || state.Instance == null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NeedsDaytimeAutonomyRelease(SpawnState state)
        {
            if (state?.Instance == null || state.IsNightResting || state.IsReturningHome)
            {
                return false;
            }

            SpringDay1DirectorStagingPlayback playback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback != null && playback.IsManualPreviewLocked)
            {
                return false;
            }

            if (state.Instance.GetComponent<SpringDay1DirectorStagingRehearsalDriver>() != null)
            {
                return false;
            }

            if (state.ReleasedFromDirectorCue || state.NeedsResidentReset)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(state.AppliedCueKey) || !string.IsNullOrWhiteSpace(state.AppliedBeatKey))
            {
                return true;
            }

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            return roamController != null && roamController.IsResidentScriptedControlActive;
        }

        private void ReleaseResidentToAutonomousRoam(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return;
            }

            ClearResidentStagingCue(state);

            state.Instance.SetActive(true);
            CancelResidentReturnHome(state, resumeRoam: true);
            state.IsNightResting = false;
            state.ResidentGroupKey = string.Empty;

            QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);

            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.ResolvedAnchorName = $"autonomous:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
        }

        private static void ClearResidentStagingCue(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return;
            }

            SetResidentCueInteractionLock(state, active: false);

            SpringDay1DirectorStagingPlayback existingPlayback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (existingPlayback != null)
            {
                existingPlayback.ClearCue();
                Destroy(existingPlayback);
            }

            state.AppliedBeatKey = string.Empty;
            state.AppliedCueKey = string.Empty;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
        }

        private void SyncCrowd()
        {
            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            if (manifest == null)
            {
                TeardownAll();
                return;
            }

            PruneDestroyedStates();

            StoryPhase currentPhase = ResolveCurrentPhase();
            string currentBeatKey = ResolveCurrentBeatKey();
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption = manifest.BuildBeatConsumptionSnapshot(currentBeatKey);
            SpringDay1NpcCrowdManifest.Entry[] entries = GetRuntimeEntries(manifest, currentPhase);
            _liveIdsScratch.Clear();

            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
                {
                    continue;
                }

                _liveIdsScratch.Add(entry.npcId);
                SpawnState state = GetOrCreateState(entry, currentBeatKey, beatConsumption);
                if (state == null)
                {
                    continue;
                }

                if (TryHoldForRehearsal(state) || TryHoldForManualPreview(state))
                {
                    continue;
                }

                bool appliedCue = ApplyStagingCue(entry, state, currentBeatKey);
                if (!appliedCue)
                {
                    ApplyResidentBaseline(entry, state, currentPhase, currentBeatKey, beatConsumption);
                }
            }

            _staleIdsScratch.Clear();
            foreach (string npcId in _spawnStates.Keys)
            {
                _staleIdsScratch.Add(npcId);
            }

            for (int index = 0; index < _staleIdsScratch.Count; index++)
            {
                string npcId = _staleIdsScratch[index];
                if (!_liveIdsScratch.Contains(npcId))
                {
                    DestroyState(npcId);
                }
            }
        }

        private string BuildRuntimeSummary()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            StoryPhase currentPhase = ResolveCurrentPhase();
            SpringDay1NpcCrowdManifest.Entry[] entries = GetRuntimeEntries(manifest, currentPhase);
            string currentBeatKey = NormalizeResidentRuntimeBeatKey(currentPhase, ResolveCurrentBeatKey());
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption = manifest != null
                ? manifest.BuildBeatConsumptionSnapshot(currentBeatKey)
                : default;

            StringBuilder builder = new StringBuilder();
            builder.Append("scene=");
            builder.Append(activeScene.IsValid() ? activeScene.name : "invalid");
            builder.Append("|phase=");
            builder.Append(currentPhase);
            builder.Append("|beat=");
            builder.Append(string.IsNullOrWhiteSpace(beatConsumption.beatKey)
                ? string.IsNullOrWhiteSpace(currentBeatKey) ? "inactive" : currentBeatKey
                : beatConsumption.beatKey);
            builder.Append("|manifest=");
            builder.Append(entries.Length);
            builder.Append("|spawned=");
            builder.Append(_spawnStates.Count);
            builder.Append("|consumption=");
            builder.Append(BuildBeatConsumptionSummary(beatConsumption));
            builder.Append("|missing=");
            bool appendedMissing = false;
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
                {
                    continue;
                }

                if (_spawnStates.ContainsKey(entry.npcId))
                {
                    continue;
                }

                if (appendedMissing)
                {
                    builder.Append(',');
                }

                appendedMissing = true;
                builder.Append(entry.npcId.Trim());
            }

            if (!appendedMissing)
            {
                builder.Append("none");
            }

            builder.Append("|active=");
            bool appendedActive = false;
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
                {
                    continue;
                }

                if (!_spawnStates.TryGetValue(entry.npcId, out SpawnState state) || state.Instance == null || !state.Instance.activeSelf)
                {
                    continue;
                }

                if (appendedActive)
                {
                    builder.Append(';');
                }

                appendedActive = true;
                AppendStateSummary(builder, entry.npcId, state, beatConsumption, true);
            }

            if (!appendedActive)
            {
                builder.Append("none");
            }

            if (_spawnStates.Count > 0)
            {
                builder.Append("|states=");
                List<string> keys = new List<string>(_spawnStates.Keys);
                keys.Sort(StringComparer.OrdinalIgnoreCase);

                bool appendedAny = false;
                for (int index = 0; index < keys.Count; index++)
                {
                    string npcId = keys[index];
                    SpawnState state = _spawnStates[npcId];
                    if (appendedAny)
                    {
                        builder.Append(';');
                    }

                    appendedAny = true;
                    AppendStateSummary(builder, npcId, state, beatConsumption, false);
                }
            }

            return builder.ToString();
        }

        private static string BuildBeatConsumptionSummary(SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            return $"p={CountEntries(beatConsumption.priority)}/s={CountEntries(beatConsumption.support)}/t={CountEntries(beatConsumption.trace)}/b={CountEntries(beatConsumption.backstagePressure)}";
        }

        private List<ResidentRuntimeSnapshotEntry> CaptureResidentRuntimeSnapshotsInternal()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            CaptureSceneResidentRuntimeSnapshotCache(activeScene);

            List<ResidentRuntimeSnapshotEntry> snapshots = new List<ResidentRuntimeSnapshotEntry>();
            foreach (ResidentRuntimeSnapshotEntry snapshot in _residentRuntimeSnapshotCache.Values)
            {
                if (!IsValidResidentRuntimeSnapshot(snapshot))
                {
                    continue;
                }

                snapshots.Add(CloneResidentRuntimeSnapshot(snapshot));
            }

            snapshots.Sort(CompareResidentRuntimeSnapshots);
            return snapshots;
        }

        private void ApplyResidentRuntimeSnapshotsInternal(IReadOnlyList<ResidentRuntimeSnapshotEntry> snapshots)
        {
            _residentRuntimeSnapshotCache.Clear();

            if (snapshots != null)
            {
                for (int index = 0; index < snapshots.Count; index++)
                {
                    ResidentRuntimeSnapshotEntry snapshot = snapshots[index];
                    if (!IsValidResidentRuntimeSnapshot(snapshot))
                    {
                        continue;
                    }

                    _residentRuntimeSnapshotCache[BuildResidentRuntimeSnapshotCacheKey(snapshot.sceneName, snapshot.npcId)] =
                        CloneResidentRuntimeSnapshot(snapshot);
                }
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!IsSupportedRuntimeScene(activeScene))
            {
                return;
            }

            List<string> keys = new List<string>(_spawnStates.Keys);
            for (int index = 0; index < keys.Count; index++)
            {
                string npcId = keys[index];
                if (!_spawnStates.TryGetValue(npcId, out SpawnState state))
                {
                    continue;
                }

                TryApplyCachedResidentRuntimeSnapshot(activeScene, npcId, state);
            }

            _syncRequested = true;
        }

        private void ClearResidentRuntimeSnapshotsInternal()
        {
            _residentRuntimeSnapshotCache.Clear();
        }

        private void CaptureSceneResidentRuntimeSnapshotCache(Scene scene)
        {
            if (!IsSupportedRuntimeScene(scene))
            {
                return;
            }

            foreach (KeyValuePair<string, SpawnState> pair in _spawnStates)
            {
                ResidentRuntimeSnapshotEntry snapshot = BuildResidentRuntimeSnapshot(pair.Key, pair.Value, scene);
                if (!IsValidResidentRuntimeSnapshot(snapshot))
                {
                    continue;
                }

                _residentRuntimeSnapshotCache[BuildResidentRuntimeSnapshotCacheKey(snapshot.sceneName, snapshot.npcId)] = snapshot;
            }
        }

        private ResidentRuntimeSnapshotEntry BuildResidentRuntimeSnapshot(string npcId, SpawnState state, Scene scene)
        {
            if (state?.Instance == null || !scene.IsValid() || state.Instance.scene != scene)
            {
                return null;
            }

            Vector2 facing = ResolveRuntimeFacing(state);
            bool persistReturnHome = ShouldPersistResidentReturnHomeSnapshot(state);
            return new ResidentRuntimeSnapshotEntry
            {
                npcId = NormalizeSnapshotValue(npcId),
                sceneName = NormalizeSnapshotValue(scene.name),
                residentGroupKey = NormalizeSnapshotValue(state.ResidentGroupKey),
                resolvedAnchorName = NormalizeSnapshotValue(state.ResolvedAnchorName),
                positionX = state.Instance.transform.position.x,
                positionY = state.Instance.transform.position.y,
                facingX = facing.x,
                facingY = facing.y,
                isActive = state.Instance.activeSelf,
                isReturningHome = persistReturnHome,
                underDirectorCue = !string.IsNullOrWhiteSpace(state.AppliedCueKey)
            };
        }

        private bool TryApplyCachedResidentRuntimeSnapshot(Scene scene, string npcId, SpawnState state)
        {
            if (!TryGetResidentRuntimeSnapshot(scene.name, npcId, out ResidentRuntimeSnapshotEntry snapshot))
            {
                return false;
            }

            ApplyResidentRuntimeSnapshotToState(state, snapshot);
            return true;
        }

        private void ApplyResidentRuntimeSnapshotToState(SpawnState state, ResidentRuntimeSnapshotEntry snapshot)
        {
            if (state?.Instance == null || snapshot == null)
            {
                return;
            }

            state.ResidentGroupKey = string.Empty;

            state.Instance.SetActive(snapshot.isActive);

            Vector3 currentPosition = state.Instance.transform.position;
            state.Instance.transform.position = new Vector3(snapshot.positionX, snapshot.positionY, currentPosition.z);

            if (TryResolveSnapshotFacing(snapshot, state.BaseFacing, out Vector2 restoredFacing))
            {
                state.BaseFacing = restoredFacing;

                NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
                if (motionController != null)
                {
                    motionController.StopMotion();
                    ApplyFacingIfIdle(state.Instance, motionController, restoredFacing);
                }
            }

            if (!string.IsNullOrWhiteSpace(snapshot.resolvedAnchorName))
            {
                state.ResolvedAnchorName = snapshot.resolvedAnchorName.Trim();
            }

            bool restoreReturnHome = ShouldRestoreResidentReturnHomeSnapshot(snapshot);
            bool shouldRestoreDirectorCue = snapshot.underDirectorCue && !ShouldYieldDaytimeResidentsToAutonomy();
            state.IsReturningHome = restoreReturnHome;
            state.ResumeRoamAfterReturn = restoreReturnHome;
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            if (restoreReturnHome || shouldRestoreDirectorCue)
            {
                AcquireResidentDirectorControl(state, resumeRoamWhenReleased: restoreReturnHome);
            }
            else
            {
                ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
                if (snapshot.isActive && roamController != null)
                {
                    // crowd continuity snapshot 只记了位置/朝向/owner 语义，没有完整保存
                    // NPCAutoRoamController 的内部 path/recovery 现场；返场后要补一次受控重启。
                    QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);
                }
            }

            if (shouldRestoreDirectorCue)
            {
                state.AppliedBeatKey = RestoredDirectorCuePlaceholderKey;
                state.AppliedCueKey = RestoredDirectorCuePlaceholderKey;
                state.NeedsResidentReset = false;
                state.ReleasedFromDirectorCue = false;
            }
            else if (restoreReturnHome)
            {
                state.AppliedBeatKey = string.Empty;
                state.AppliedCueKey = string.Empty;
                state.NeedsResidentReset = true;
                state.ReleasedFromDirectorCue = true;
            }
            else
            {
                state.AppliedBeatKey = string.Empty;
                state.AppliedCueKey = string.Empty;
                state.NeedsResidentReset = false;
                state.ReleasedFromDirectorCue = false;
            }
        }

        private bool ShouldPersistResidentReturnHomeSnapshot(SpawnState state)
        {
            return false;
        }

        private bool ShouldRestoreResidentReturnHomeSnapshot(ResidentRuntimeSnapshotEntry snapshot)
        {
            return false;
        }

        private bool TryGetResidentRuntimeSnapshot(string sceneName, string npcId, out ResidentRuntimeSnapshotEntry snapshot)
        {
            return _residentRuntimeSnapshotCache.TryGetValue(
                BuildResidentRuntimeSnapshotCacheKey(sceneName, npcId),
                out snapshot);
        }

        private static string BuildResidentRuntimeSnapshotCacheKey(string sceneName, string npcId)
        {
            return $"{NormalizeSnapshotValue(sceneName)}::{NormalizeSnapshotValue(npcId)}";
        }

        private static ResidentRuntimeSnapshotEntry CloneResidentRuntimeSnapshot(ResidentRuntimeSnapshotEntry snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            return new ResidentRuntimeSnapshotEntry
            {
                npcId = snapshot.npcId,
                sceneName = snapshot.sceneName,
                residentGroupKey = snapshot.residentGroupKey,
                resolvedAnchorName = snapshot.resolvedAnchorName,
                positionX = snapshot.positionX,
                positionY = snapshot.positionY,
                facingX = snapshot.facingX,
                facingY = snapshot.facingY,
                isActive = snapshot.isActive,
                isReturningHome = snapshot.isReturningHome,
                underDirectorCue = snapshot.underDirectorCue
            };
        }

        private static bool IsValidResidentRuntimeSnapshot(ResidentRuntimeSnapshotEntry snapshot)
        {
            return snapshot != null
                && !string.IsNullOrWhiteSpace(snapshot.npcId)
                && !string.IsNullOrWhiteSpace(snapshot.sceneName)
                && (snapshot.sceneName == PrimarySceneName || snapshot.sceneName == TownSceneName);
        }

        private static int CompareResidentRuntimeSnapshots(ResidentRuntimeSnapshotEntry left, ResidentRuntimeSnapshotEntry right)
        {
            int sceneCompare = string.CompareOrdinal(left?.sceneName ?? string.Empty, right?.sceneName ?? string.Empty);
            return sceneCompare != 0
                ? sceneCompare
                : string.CompareOrdinal(left?.npcId ?? string.Empty, right?.npcId ?? string.Empty);
        }

        private static string NormalizeSnapshotValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static Vector2 ResolveRuntimeFacing(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return Vector2.down;
            }

            NPCAnimController animController = state.Instance.GetComponent<NPCAnimController>();
            if (animController != null)
            {
                return ConvertFacingDirection(animController.CurrentDirection);
            }

            return state.BaseFacing.sqrMagnitude > 0.0001f
                ? state.BaseFacing.normalized
                : Vector2.down;
        }

        private static bool TryResolveSnapshotFacing(ResidentRuntimeSnapshotEntry snapshot, Vector2 fallbackFacing, out Vector2 facing)
        {
            Vector2 snapshotFacing = snapshot != null
                ? new Vector2(snapshot.facingX, snapshot.facingY)
                : Vector2.zero;
            if (snapshotFacing.sqrMagnitude > 0.0001f)
            {
                facing = snapshotFacing.normalized;
                return true;
            }

            if (fallbackFacing.sqrMagnitude > 0.0001f)
            {
                facing = fallbackFacing.normalized;
                return true;
            }

            facing = Vector2.zero;
            return false;
        }

        private static Vector2 ConvertFacingDirection(NPCAnimController.NPCAnimDirection direction)
        {
            return direction switch
            {
                NPCAnimController.NPCAnimDirection.Up => Vector2.up,
                NPCAnimController.NPCAnimDirection.Left => Vector2.left,
                NPCAnimController.NPCAnimDirection.Right => Vector2.right,
                _ => Vector2.down
            };
        }

        private static int CountEntries(SpringDay1NpcCrowdManifest.BeatConsumptionEntry[] entries)
        {
            return entries != null ? entries.Length : 0;
        }

        private SpawnState GetOrCreateState(
            SpringDay1NpcCrowdManifest.Entry entry,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (_spawnStates.TryGetValue(entry.npcId, out SpawnState existing) && existing.Instance != null)
            {
                return existing;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!IsSupportedRuntimeScene(activeScene))
            {
                return null;
            }

            if (!TryBindSceneResident(entry, activeScene, beatKey, out SpawnState state))
            {
                return null;
            }

            _spawnStates[entry.npcId] = state;
            state.ResidentGroupKey = string.Empty;
            TryApplyCachedResidentRuntimeSnapshot(activeScene, entry.npcId, state);
            return state;
        }

        private bool TryBindSceneResident(
            SpringDay1NpcCrowdManifest.Entry entry,
            Scene activeScene,
            string beatKey,
            out SpawnState state)
        {
            state = null;
            GameObject instance = FindSceneResident(entry, activeScene);
            if (instance == null)
            {
                return false;
            }

            Transform homeAnchor = FindSceneResidentHomeAnchor(entry, activeScene, instance.transform);
            Vector3 basePosition = instance.transform.position;
            string anchorName = instance.name;

            ConfigureBoundNpc(entry, instance, homeAnchor);

            state = new SpawnState
            {
                Instance = instance,
                HomeAnchor = homeAnchor != null ? homeAnchor.gameObject : null,
                OwnsInstance = false,
                OwnsHomeAnchor = false,
                ResolvedAnchorName = anchorName,
                UsedFallback = false,
                BaseResolvedAnchorName = anchorName,
                BaseUsedFallback = false,
                BasePosition = basePosition,
                BaseFacing = entry.initialFacing,
                ResidentGroupKey = string.Empty,
                NeedsResidentReset = true
            };
            return true;
        }

        private void ConfigureBoundNpc(SpringDay1NpcCrowdManifest.Entry entry, GameObject instance, Transform homeAnchor)
        {
            NPCAutoRoamController roamController = instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                if (homeAnchor != null)
                {
                    roamController.BindResidentHomeAnchor(homeAnchor);
                }

                roamController.SyncRuntimeProfileFromAsset();
            }

            if (instance.GetComponent<NPCInformalChatInteractable>() == null
                && roamController != null
                && roamController.RoamProfile != null
                && roamController.RoamProfile.HasInformalConversationContent)
            {
                instance.AddComponent<NPCInformalChatInteractable>();
            }

            NPCMotionController motionController = instance.GetComponent<NPCMotionController>();
            if (motionController != null && entry.initialFacing.sqrMagnitude > 0.01f)
            {
                ApplyFacingIfIdle(instance, motionController, entry.initialFacing);
            }
        }

        private SpawnPointResolution ResolveSpawnPoint(SpringDay1NpcCrowdManifest.Entry entry, string beatKey)
        {
            Transform anchor = FindSemanticAnchor(entry, beatKey);
            if (anchor != null)
            {
                Vector3 resolvedPosition = anchor.position + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
                return new SpawnPointResolution(resolvedPosition, anchor.name, usedFallback: false);
            }

            if (TryResolveTownContractAnchor(entry, beatKey, out Vector3 contractPosition, out string contractAnchorName))
            {
                Vector3 resolvedPosition = contractPosition + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
                return new SpawnPointResolution(resolvedPosition, contractAnchorName, usedFallback: false);
            }

            anchor = FindAnchor(entry.anchorObjectName);
            bool usedFallback = anchor == null;
            Vector3 basePosition = usedFallback
                ? new Vector3(entry.fallbackWorldPosition.x, entry.fallbackWorldPosition.y, 0f)
                : anchor.position;
            Vector3 fallbackResolvedPosition = basePosition + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
            string anchorName = usedFallback ? "fallback" : anchor.name;
            return new SpawnPointResolution(fallbackResolvedPosition, anchorName, usedFallback);
        }

        private Transform FindSemanticAnchor(SpringDay1NpcCrowdManifest.Entry entry, string beatKey)
        {
            if (entry == null)
            {
                return null;
            }

            string preferredSemanticAnchor = ResolvePreferredSemanticAnchor(entry, beatKey);
            if (!string.IsNullOrWhiteSpace(preferredSemanticAnchor)
                && !IsDeprecatedRuntimeSemanticAnchor(preferredSemanticAnchor))
            {
                Transform preferredAnchor = FindAnchor(preferredSemanticAnchor);
                if (preferredAnchor != null)
                {
                    return preferredAnchor;
                }
            }

            if (entry.semanticAnchorIds == null)
            {
                return null;
            }

            for (int index = 0; index < entry.semanticAnchorIds.Length; index++)
            {
                string semanticAnchorId = entry.semanticAnchorIds[index];
                if (IsDeprecatedRuntimeSemanticAnchor(semanticAnchorId))
                {
                    continue;
                }

                Transform resolved = FindAnchor(semanticAnchorId);
                if (resolved != null)
                {
                    return resolved;
                }
            }

            return null;
        }

        private static string ResolvePreferredSemanticAnchor(SpringDay1NpcCrowdManifest.Entry entry, string beatKey)
        {
            if (entry == null || string.IsNullOrWhiteSpace(beatKey) || entry.semanticAnchorIds == null)
            {
                return string.Empty;
            }

            if (SpringDay1DirectorStagingDatabase.TryResolveCue(beatKey, entry, out SpringDay1DirectorActorCue cue)
                && !string.IsNullOrWhiteSpace(cue.semanticAnchorId))
            {
                return cue.semanticAnchorId;
            }

            return string.Empty;
        }

        private static bool TryResolveTownContractAnchor(SpringDay1NpcCrowdManifest.Entry entry, string beatKey, out Vector3 worldPosition, out string anchorName)
        {
            worldPosition = Vector3.zero;
            anchorName = string.Empty;
            if (entry == null)
            {
                return false;
            }

            string preferredSemanticAnchor = ResolvePreferredSemanticAnchor(entry, beatKey);
            if (!string.IsNullOrWhiteSpace(preferredSemanticAnchor)
                && !IsDeprecatedRuntimeSemanticAnchor(preferredSemanticAnchor)
                && SpringDay1TownAnchorContractDatabase.TryResolveAnchor(preferredSemanticAnchor, out worldPosition))
            {
                anchorName = preferredSemanticAnchor.Trim();
                return true;
            }

            if (entry.semanticAnchorIds == null)
            {
                return false;
            }

            for (int index = 0; index < entry.semanticAnchorIds.Length; index++)
            {
                string semanticAnchorId = entry.semanticAnchorIds[index];
                if (string.IsNullOrWhiteSpace(semanticAnchorId))
                {
                    continue;
                }

                if (IsDeprecatedRuntimeSemanticAnchor(semanticAnchorId))
                {
                    continue;
                }

                if (SpringDay1TownAnchorContractDatabase.TryResolveAnchor(semanticAnchorId, out worldPosition))
                {
                    anchorName = semanticAnchorId.Trim();
                    return true;
                }
            }

            return false;
        }

        private Transform FindAnchor(string anchorObjectName)
        {
            if (string.IsNullOrWhiteSpace(anchorObjectName))
            {
                return null;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            EnsureSceneLookupCache(activeScene);
            string[] candidates = anchorObjectName.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < candidates.Length; index++)
            {
                string name = candidates[index].Trim();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                foreach (string lookupName in EnumerateAnchorLookupNames(name))
                {
                    if (_sceneTransformLookupCache.TryGetValue(lookupName, out Transform cachedAnchor)
                        && cachedAnchor != null
                        && cachedAnchor.gameObject.scene == activeScene)
                    {
                        return cachedAnchor;
                    }

                    if (_sceneMissingTransformLookupCache.Contains(lookupName))
                    {
                        continue;
                    }

                    GameObject found = GameObject.Find(lookupName);
                    if (found != null && found.scene == activeScene)
                    {
                        _sceneGameObjectLookupCache[lookupName] = found;
                        _sceneTransformLookupCache[lookupName] = found.transform;
                        return found.transform;
                    }

                    _sceneMissingTransformLookupCache.Add(lookupName);
                }
            }

            return null;
        }

        private static IEnumerable<string> EnumerateAnchorNames(string anchorObjectName)
        {
            if (string.IsNullOrWhiteSpace(anchorObjectName))
            {
                yield break;
            }

            string[] candidates = anchorObjectName.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < candidates.Length; index++)
            {
                string candidate = candidates[index].Trim();
                if (!string.IsNullOrEmpty(candidate))
                {
                    yield return candidate;
                }
            }
        }

        private static IEnumerable<string> EnumerateAnchorLookupNames(string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                yield break;
            }

            string trimmed = candidateName.Trim();
            string derivedHomeAnchor = TryBuildHomeAnchorAlias(trimmed);
            if (!string.IsNullOrEmpty(derivedHomeAnchor))
            {
                yield return derivedHomeAnchor;
            }

            if (!trimmed.EndsWith("_HomeAnchor", StringComparison.OrdinalIgnoreCase))
            {
                yield return $"{trimmed}_HomeAnchor";
            }

            yield return trimmed;
        }

        private static string TryBuildHomeAnchorAlias(string candidateName)
        {
            if (!candidateName.StartsWith("NPC", StringComparison.OrdinalIgnoreCase) || candidateName.Length <= 3)
            {
                return null;
            }

            string numericSuffix = candidateName.Substring(3);
            for (int index = 0; index < numericSuffix.Length; index++)
            {
                if (!char.IsDigit(numericSuffix[index]))
                {
                    return null;
                }
            }

            return $"{numericSuffix}_HomeAnchor";
        }

        private void SetEntryActive(string npcId, bool active)
        {
            if (!_spawnStates.TryGetValue(npcId, out SpawnState state) || state.Instance == null)
            {
                return;
            }

            if (state.Instance.activeSelf != active)
            {
                state.Instance.SetActive(active);
            }
        }

        private SpringDay1NpcCrowdManifest LoadManifest()
        {
            if (_manifest == null)
            {
                _manifest = Resources.Load<SpringDay1NpcCrowdManifest>(ManifestResourcePath);
            }

            return _manifest;
        }

        private SpringDay1NpcCrowdManifest.Entry[] GetRuntimeEntries(SpringDay1NpcCrowdManifest manifest, StoryPhase currentPhase)
        {
            SpringDay1NpcCrowdManifest.Entry[] manifestEntries = manifest != null
                ? manifest.Entries
                : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();

            List<SpringDay1NpcCrowdManifest.Entry> syntheticEntries = BuildSyntheticRuntimeEntries(manifestEntries, currentPhase);
            if (syntheticEntries.Count <= 0)
            {
                return manifestEntries;
            }

            SpringDay1NpcCrowdManifest.Entry[] runtimeEntries = new SpringDay1NpcCrowdManifest.Entry[manifestEntries.Length + syntheticEntries.Count];
            Array.Copy(manifestEntries, runtimeEntries, manifestEntries.Length);
            for (int index = 0; index < syntheticEntries.Count; index++)
            {
                runtimeEntries[manifestEntries.Length + index] = syntheticEntries[index];
            }

            return runtimeEntries;
        }

        private static List<SpringDay1NpcCrowdManifest.Entry> BuildSyntheticRuntimeEntries(
            SpringDay1NpcCrowdManifest.Entry[] manifestEntries,
            StoryPhase currentPhase)
        {
            List<SpringDay1NpcCrowdManifest.Entry> entries = new List<SpringDay1NpcCrowdManifest.Entry>();

            if (ShouldIncludeThirdResidentInResidentRuntime(currentPhase)
                && !RuntimeEntriesContainNpcId(manifestEntries, ThirdResidentNpcId))
            {
                entries.Add(BuildSyntheticThirdResidentResidentEntry());
            }

            if (ShouldIncludeStoryEscortInUnifiedNightRuntime(currentPhase))
            {
                if (!RuntimeEntriesContainNpcId(manifestEntries, StoryEscortChiefNpcId))
                {
                    entries.Add(BuildSyntheticStoryEscortResidentEntry(
                        StoryEscortChiefNpcId,
                        "晚段后并回普通村民 runtime，20:00/21:00 跟全体 resident 统一走夜间合同。",
                        "001_HomeAnchor|NPC001_HomeAnchor",
                        Vector2.left));
                }

                if (!RuntimeEntriesContainNpcId(manifestEntries, StoryEscortCompanionNpcId))
                {
                    entries.Add(BuildSyntheticStoryEscortResidentEntry(
                        StoryEscortCompanionNpcId,
                        "晚段后并回普通村民 runtime，20:00/21:00 跟全体 resident 统一走夜间合同。",
                        "002_HomeAnchor|NPC002_HomeAnchor",
                        Vector2.right));
                }
            }

            return entries;
        }

        private static bool ShouldIncludeThirdResidentInResidentRuntime(StoryPhase currentPhase)
        {
            return currentPhase >= StoryPhase.EnterVillage && currentPhase <= StoryPhase.DayEnd;
        }

        private static bool ShouldIncludeStoryEscortInUnifiedNightRuntime(StoryPhase currentPhase)
        {
            return currentPhase >= StoryPhase.DinnerConflict && currentPhase <= StoryPhase.DayEnd;
        }

        private static bool RuntimeEntriesContainNpcId(
            SpringDay1NpcCrowdManifest.Entry[] entries,
            string npcId)
        {
            if (entries == null || string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry != null
                    && string.Equals(entry.npcId?.Trim(), npcId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static SpringDay1NpcCrowdManifest.Entry BuildSyntheticThirdResidentResidentEntry()
        {
            return new SpringDay1NpcCrowdManifest.Entry
            {
                npcId = ThirdResidentNpcId,
                displayName = "坡边目击人",
                roleSummary = "opening 后并回普通村民 runtime，不再继续挂在导演私有合同里。",
                anchorObjectName = "003_HomeAnchor|NPC003_HomeAnchor",
                initialFacing = Vector2.left,
                semanticAnchorIds = Array.Empty<string>(),
                residentBaseline = SpringDay1CrowdResidentBaseline.DaytimeResident,
                residentBeatSemantics = new[]
                {
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.FreeTimeNightWitness,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.KeepRoutine | SpringDay1CrowdResidentBeatFlags.AmbientPressure,
                        note = "opening 之后按普通村民收口，白天和傍晚都不再挂在导演 special runtime。"
                    },
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.DayEndSettle,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.ReturnHome | SpringDay1CrowdResidentBeatFlags.KeepRoutine,
                        note = "20:00 后和普通 resident 一样回家，21:00 后进入 rest。"
                    },
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.DailyStandPreview,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.AlreadyAround | SpringDay1CrowdResidentBeatFlags.KeepRoutine,
                        note = "次日继续作为普通村民在场。"
                    }
                }
            };
        }

        private static SpringDay1NpcCrowdManifest.Entry BuildSyntheticStoryEscortResidentEntry(
            string npcId,
            string roleSummary,
            string anchorObjectName,
            Vector2 initialFacing)
        {
            return new SpringDay1NpcCrowdManifest.Entry
            {
                npcId = npcId,
                displayName = $"夜段并回普通村民_{npcId}",
                roleSummary = roleSummary,
                anchorObjectName = anchorObjectName,
                initialFacing = initialFacing,
                semanticAnchorIds = Array.Empty<string>(),
                residentBaseline = SpringDay1CrowdResidentBaseline.DaytimeResident,
                residentBeatSemantics = new[]
                {
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.FreeTimeNightWitness,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.KeepRoutine | SpringDay1CrowdResidentBeatFlags.AmbientPressure,
                        note = "19:30 后退出导演私链，先按普通村民规则自由活动，直到 20:00 夜间合同开始。"
                    },
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.DayEndSettle,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.ReturnHome | SpringDay1CrowdResidentBeatFlags.KeepRoutine,
                        note = "20:00 后和全体 resident 一样进入统一回家/休息合同。"
                    },
                    new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = SpringDay1CrowdResidentBeatKeys.DailyStandPreview,
                        presenceLevel = SpringDay1CrowdResidentPresenceLevel.Background,
                        flags = SpringDay1CrowdResidentBeatFlags.AlreadyAround | SpringDay1CrowdResidentBeatFlags.KeepRoutine,
                        note = "次日不再由 Day1 私链继续持有，直接还给普通 NPC 规则。"
                    }
                }
            };
        }

        private static StoryPhase ResolveCurrentPhase()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null ? storyManager.CurrentPhase : StoryPhase.CrashAndMeet;
        }

        private static string ResolveCurrentBeatKey()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            return director != null ? director.GetCurrentBeatKey() : string.Empty;
        }

        private static string NormalizeResidentRuntimeBeatKey(StoryPhase currentPhase, string beatKey)
        {
            if (currentPhase == StoryPhase.ReturnAndReminder
                && string.Equals(beatKey, SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase))
            {
                return SpringDay1DirectorBeatKeys.DinnerConflictTable;
            }

            return beatKey ?? string.Empty;
        }

        private static SpringDay1CrowdDirectorConsumptionRole ResolveBeatConsumptionRole(
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption,
            string npcId)
        {
            if (TryFindBeatConsumptionEntry(beatConsumption.priority, npcId, out _))
            {
                return SpringDay1CrowdDirectorConsumptionRole.Priority;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.support, npcId, out _))
            {
                return SpringDay1CrowdDirectorConsumptionRole.Support;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.trace, npcId, out _))
            {
                return SpringDay1CrowdDirectorConsumptionRole.Trace;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.backstagePressure, npcId, out _))
            {
                return SpringDay1CrowdDirectorConsumptionRole.BackstagePressure;
            }

            return SpringDay1CrowdDirectorConsumptionRole.None;
        }

        private static bool TryResolveBeatPresence(
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption,
            string npcId,
            out SpringDay1CrowdResidentPresenceLevel presenceLevel)
        {
            if (TryFindBeatConsumptionEntry(beatConsumption.priority, npcId, out SpringDay1NpcCrowdManifest.BeatConsumptionEntry priorityEntry))
            {
                presenceLevel = priorityEntry.presenceLevel;
                return true;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.support, npcId, out SpringDay1NpcCrowdManifest.BeatConsumptionEntry supportEntry))
            {
                presenceLevel = supportEntry.presenceLevel;
                return true;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.trace, npcId, out SpringDay1NpcCrowdManifest.BeatConsumptionEntry traceEntry))
            {
                presenceLevel = traceEntry.presenceLevel;
                return true;
            }

            if (TryFindBeatConsumptionEntry(beatConsumption.backstagePressure, npcId, out SpringDay1NpcCrowdManifest.BeatConsumptionEntry backstageEntry))
            {
                presenceLevel = backstageEntry.presenceLevel;
                return true;
            }

            presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
            return false;
        }

        private static bool TryFindBeatConsumptionEntry(
            SpringDay1NpcCrowdManifest.BeatConsumptionEntry[] entries,
            string npcId,
            out SpringDay1NpcCrowdManifest.BeatConsumptionEntry entry)
        {
            if (entries != null)
            {
                for (int index = 0; index < entries.Length; index++)
                {
                    if (string.Equals(entries[index].npcId, npcId, StringComparison.OrdinalIgnoreCase))
                    {
                        entry = entries[index];
                        return true;
                    }
                }
            }

            entry = default;
            return false;
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }

        private static bool TryHoldForRehearsal(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return false;
            }

            SpringDay1DirectorStagingRehearsalDriver rehearsalDriver = state.Instance.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
            if (rehearsalDriver == null || !rehearsalDriver.isActiveAndEnabled)
            {
                return false;
            }

            SpringDay1DirectorStagingPlayback playback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            string cueKey = playback != null && !string.IsNullOrWhiteSpace(playback.CurrentCueKey)
                ? playback.CurrentCueKey
                : state.AppliedCueKey;

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.ResolvedAnchorName = string.IsNullOrWhiteSpace(cueKey)
                ? $"rehearsal:{state.Instance.name}"
                : $"rehearsal:{cueKey}";
            state.UsedFallback = false;
            return true;
        }

        private static bool TryHoldForManualPreview(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return false;
            }

            SpringDay1DirectorStagingPlayback playback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback == null || !playback.IsManualPreviewLocked)
            {
                return false;
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.ResolvedAnchorName = string.IsNullOrWhiteSpace(playback.CurrentCueKey)
                ? "manual-preview"
                : $"manual-preview:{playback.CurrentCueKey}";
            state.UsedFallback = false;
            return true;
        }

        private bool ApplyStagingCue(SpringDay1NpcCrowdManifest.Entry entry, SpawnState state, string beatKey)
        {
            if (entry == null || state?.Instance == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(beatKey)
                && TryResolveStagingCueForCurrentScene(beatKey, entry, out SpringDay1DirectorActorCue cue))
            {
                CancelResidentReturnHome(state, resumeRoam: false);
                state.ResidentGroupKey = string.Empty;
                SetEntryActive(entry.npcId, true);
                SpringDay1DirectorStagingPlayback playback = GetOrAddComponent<SpringDay1DirectorStagingPlayback>(state.Instance);
                SpringDay1DirectorActorCue runtimeCue = ResolveRuntimeCueOverride(beatKey, entry, cue);
                string cueKey = runtimeCue != null ? runtimeCue.StableKey : cue.StableKey;
                bool needsApply = !string.Equals(state.AppliedBeatKey, beatKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(state.AppliedCueKey, cueKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(playback.CurrentBeatKey, beatKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(playback.CurrentCueKey, cueKey, StringComparison.OrdinalIgnoreCase);
                if (!needsApply)
                {
                    return true;
                }

                if (ShouldRestartCueFromSceneBaseline(beatKey, state))
                {
                    ResetStateToBasePose(state);
                }

                SetResidentCueInteractionLock(state, active: true);
                playback.ApplyCue(beatKey, runtimeCue ?? cue, state.HomeAnchor != null ? state.HomeAnchor.transform : null);
                state.AppliedBeatKey = beatKey;
                state.AppliedCueKey = cueKey;
                state.ResolvedAnchorName = $"staging:{cueKey}";
                state.UsedFallback = false;
                state.ReleasedFromDirectorCue = false;
                state.NeedsResidentReset = false;

                return true;
            }

            if (string.IsNullOrWhiteSpace(state.AppliedCueKey))
            {
                return false;
            }

            if (ShouldHoldDinnerCueThroughReturnReminderBlock(state))
            {
                SetResidentCueInteractionLock(state, active: true);
                state.ResidentGroupKey = string.Empty;
                state.ResolvedAnchorName = $"staging:{state.AppliedCueKey}";
                state.UsedFallback = false;
                state.ReleasedFromDirectorCue = false;
                state.NeedsResidentReset = false;
                return true;
            }

            SpringDay1DirectorStagingPlayback existingPlayback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (existingPlayback != null)
            {
                existingPlayback.ClearCue();
                Destroy(existingPlayback);
            }

            SetResidentCueInteractionLock(state, active: false);
            state.AppliedBeatKey = string.Empty;
            state.AppliedCueKey = string.Empty;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
            state.NeedsResidentReset = true;
            state.ReleasedFromDirectorCue = true;
            return false;
        }

        private static bool ShouldRestartCueFromSceneBaseline(string beatKey, SpawnState state)
        {
            return state?.Instance != null
                && state.Instance.scene.IsValid()
                && string.Equals(state.Instance.scene.name, TownSceneName, StringComparison.Ordinal)
                && ShouldUseVillageCrowdSceneResidentStart(beatKey);
        }

        private static bool ShouldHoldDinnerCueThroughReturnReminderBlock(SpawnState state)
        {
            return state != null
                && ResolveCurrentPhase() == StoryPhase.ReturnAndReminder
                && string.Equals(state.AppliedBeatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(state.AppliedCueKey);
        }

        private static void ResetStateToBasePose(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return;
            }

            AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false);

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            motionController?.StopMotion();

            Vector3 basePosition = state.BasePosition;
            basePosition.z = state.Instance.transform.position.z;
            state.Instance.transform.position = basePosition;
            if (motionController != null)
            {
                ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
        }

        private static NPCAutoRoamController PrepareResidentRoamController(SpawnState state)
        {
            if (state?.Instance == null)
            {
                return null;
            }

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            if (roamController == null)
            {
                return null;
            }

            roamController.enabled = true;
            if (state.HomeAnchor != null && roamController.HomeAnchor != state.HomeAnchor.transform)
            {
                roamController.BindResidentHomeAnchor(state.HomeAnchor.transform);
            }

            return roamController;
        }

        private static NPCAutoRoamController AcquireResidentDirectorControl(SpawnState state, bool resumeRoamWhenReleased)
        {
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            roamController?.AcquireStoryControl(ResidentScriptedControlOwnerKey, resumeRoamWhenReleased);
            return roamController;
        }

        private static NPCAutoRoamController ReleaseResidentDirectorControl(SpawnState state, bool resumeResidentLogic)
        {
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            roamController?.ReleaseStoryControl(ResidentScriptedControlOwnerKey, resumeResidentLogic);
            return roamController;
        }

        private static NPCAutoRoamController ReleaseResidentSharedRuntimeControl(SpawnState state, bool resumeResidentLogic)
        {
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            if (roamController == null)
            {
                return null;
            }

            roamController.ReleaseStoryControl(ResidentScriptedControlOwnerKey, resumeResidentLogic);
            roamController.ReleaseStoryControl(DirectorResidentScriptedControlOwnerKey, resumeResidentLogic);
            return roamController;
        }

        private static void QueueResidentAutonomousRoamResume(SpawnState state, bool tryImmediateMove)
        {
            NPCAutoRoamController roamController = ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
            if (roamController == null)
            {
                ClearQueuedAutonomousResume(state);
                return;
            }

            state.QueuedAutonomousResume = true;
            state.QueuedAutonomousResumeTryImmediateMove = tryImmediateMove;
            state.NextAutonomousResumeAt = Mathf.Max(
                state.NextAutonomousResumeAt,
                Time.unscaledTime + GetQueuedAutonomousResumeDelay(state, roamController));
        }

        private static float GetQueuedAutonomousResumeDelay(SpawnState state, NPCAutoRoamController roamController)
        {
            int stableId = roamController != null ? roamController.GetInstanceID() : state?.Instance?.GetInstanceID() ?? 0;
            float spread = Mathf.Repeat(Mathf.Abs(stableId) * 0.0173f, 1f);
            return QueuedAutonomousResumeBaseDelay + spread * QueuedAutonomousResumeJitterSeconds;
        }

        private static void SetResidentCueInteractionLock(SpawnState state, bool active)
        {
            if (state?.Instance == null)
            {
                return;
            }

            NPCDialogueInteractable formalDialogue = state.Instance.GetComponent<NPCDialogueInteractable>();
            NPCInformalChatInteractable informalDialogue = state.Instance.GetComponent<NPCInformalChatInteractable>();

            if (active)
            {
                if (!state.InteractionLockOwnedByCue)
                {
                    state.FormalDialogueWasEnabledBeforeCue = formalDialogue != null && formalDialogue.enabled;
                    state.InformalDialogueWasEnabledBeforeCue = informalDialogue != null && informalDialogue.enabled;
                    state.InteractionLockOwnedByCue = true;
                }

                return;
            }

            if (!state.InteractionLockOwnedByCue)
            {
                return;
            }

            if (formalDialogue != null)
            {
                formalDialogue.enabled = state.FormalDialogueWasEnabledBeforeCue;
            }

            if (informalDialogue != null)
            {
                informalDialogue.enabled = state.InformalDialogueWasEnabledBeforeCue;
            }

            state.InteractionLockOwnedByCue = false;
            state.FormalDialogueWasEnabledBeforeCue = false;
            state.InformalDialogueWasEnabledBeforeCue = false;
        }

        private void TickQueuedAutonomousResumes()
        {
            if (_spawnStates.Count <= 0)
            {
                return;
            }

            float now = Time.unscaledTime;
            if (now < _nextQueuedAutonomousResumeDispatchAt)
            {
                return;
            }

            int resumedCount = 0;
            foreach (SpawnState state in _spawnStates.Values)
            {
                if (!CanDispatchQueuedAutonomousResume(state, now))
                {
                    continue;
                }

                NPCAutoRoamController roamController = PrepareResidentRoamController(state);
                if (roamController == null || roamController.IsResidentScriptedControlActive)
                {
                    ClearQueuedAutonomousResume(state);
                    continue;
                }

                roamController.ResumeAutonomousRoam(state.QueuedAutonomousResumeTryImmediateMove);
                ClearQueuedAutonomousResume(state);
                resumedCount++;
                if (resumedCount >= MaxQueuedAutonomousResumesPerTick)
                {
                    break;
                }
            }

            if (resumedCount > 0)
            {
                _nextQueuedAutonomousResumeDispatchAt = now + QueuedAutonomousResumeDispatchInterval;
            }
        }

        private static bool CanDispatchQueuedAutonomousResume(SpawnState state, float now)
        {
            if (state == null || !state.QueuedAutonomousResume)
            {
                return false;
            }

            if (state.Instance == null ||
                !state.Instance.activeInHierarchy ||
                state.IsNightResting ||
                state.IsReturningHome ||
                state.NeedsResidentReset ||
                state.ReleasedFromDirectorCue ||
                !string.IsNullOrWhiteSpace(state.AppliedCueKey) ||
                !string.IsNullOrWhiteSpace(state.AppliedBeatKey))
            {
                ClearQueuedAutonomousResume(state);
                return false;
            }

            return now >= state.NextAutonomousResumeAt;
        }

        private static void ClearQueuedAutonomousResume(SpawnState state)
        {
            if (state == null)
            {
                return;
            }

            state.QueuedAutonomousResume = false;
            state.QueuedAutonomousResumeTryImmediateMove = false;
            state.NextAutonomousResumeAt = 0f;
        }

        private void CancelQueuedAutonomousResumes()
        {
            if (_spawnStates.Count <= 0)
            {
                return;
            }

            foreach (SpawnState state in _spawnStates.Values)
            {
                ClearQueuedAutonomousResume(state);
            }

            _nextQueuedAutonomousResumeDispatchAt = 0f;
        }

        private static void ApplyFacingIfIdle(GameObject instance, NPCMotionController motionController, Vector2 facing)
        {
            if (instance == null || motionController == null || facing.sqrMagnitude <= 0.01f)
            {
                return;
            }

            NPCAutoRoamController roamController = instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null && roamController.IsMoving)
            {
                return;
            }

            if (motionController.CurrentVelocity.sqrMagnitude > 0.0001f ||
                motionController.ReportedVelocity.sqrMagnitude > 0.0001f)
            {
                return;
            }

            if (roamController != null)
            {
                roamController.ApplyIdleFacing(facing);
                return;
            }

            motionController.ApplyIdleFacing(facing);
        }

        private static bool TryResolveStagingCueForCurrentScene(
            string beatKey,
            SpringDay1NpcCrowdManifest.Entry entry,
            out SpringDay1DirectorActorCue cue)
        {
            cue = null;
            if (string.IsNullOrWhiteSpace(beatKey) || entry == null)
            {
                return false;
            }

            if (ShouldSuppressEnterVillageCrowdCueForTownHouseLead(beatKey))
            {
                return false;
            }

            return TryBuildSceneMarkerCue(entry, out cue)
                && ShouldUseVillageCrowdMarkerCueOverride(beatKey);
        }

        private static bool ShouldSuppressEnterVillageCrowdCueForTownHouseLead(string beatKey)
        {
            if (!string.Equals(beatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (_instance != null && _instance._enterVillageCrowdReleaseLatched)
            {
                return true;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            return director != null && director.ShouldReleaseEnterVillageCrowd();
        }

        private bool ShouldStandDownDuringPostTutorialTownAutonomy(Scene activeScene)
        {
            if (!activeScene.IsValid()
                || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal))
            {
                return false;
            }

            if (ShouldResidentsRestByClock() || ShouldResidentsReturnHomeByClock())
            {
                return false;
            }

            if (ResolveCurrentPhase() != StoryPhase.FarmingTutorial)
            {
                return false;
            }

            return string.Equals(
                ResolveCurrentBeatKey(),
                SpringDay1DirectorBeatKeys.FreeTimeNightWitness,
                StringComparison.OrdinalIgnoreCase);
        }

        private SpringDay1DirectorActorCue ResolveRuntimeCueOverride(
            string beatKey,
            SpringDay1NpcCrowdManifest.Entry entry,
            SpringDay1DirectorActorCue cue)
        {
            bool shouldUseSceneResidentStart = ShouldUseVillageCrowdSceneResidentStart(beatKey);
            if (cue == null
                || entry == null
                || string.IsNullOrWhiteSpace(entry.npcId)
                || !ShouldUseVillageCrowdMarkerCueOverride(beatKey))
            {
                return cue;
            }

            bool hasLiveMarkerOverride = TryResolveVillageCrowdMarkers(entry.npcId, out Transform startMarker, out Transform endMarker)
                && (startMarker != null || endMarker != null);
            if (!hasLiveMarkerOverride && !shouldUseSceneResidentStart)
            {
                return cue;
            }

            SpringDay1DirectorActorCue resolvedCue = CloneCue(cue);
            if (shouldUseSceneResidentStart)
            {
                resolvedCue.keepCurrentSpawnPosition = true;
                resolvedCue.useSemanticAnchorAsStart = false;
                resolvedCue.startPositionIsSemanticAnchorOffset = false;
            }

            if (hasLiveMarkerOverride)
            {
                resolvedCue.keepCurrentSpawnPosition = false;
                resolvedCue.useSemanticAnchorAsStart = false;
                resolvedCue.startPositionIsSemanticAnchorOffset = false;
                resolvedCue.pathPointsAreOffsets = false;
                resolvedCue.loopPath = false;

                if (startMarker != null)
                {
                    resolvedCue.startPosition = new Vector2(startMarker.position.x, startMarker.position.y);
                }

                if (endMarker != null)
                {
                    Vector2 facing = resolvedCue.facing;
                    if (startMarker != null)
                    {
                        Vector2 direction = (Vector2)(endMarker.position - startMarker.position);
                        if (direction.sqrMagnitude > 0.0001f)
                        {
                            facing = direction.normalized;
                        }
                    }

                    resolvedCue.path = new[]
                    {
                        new SpringDay1DirectorPathPoint
                        {
                            position = new Vector2(endMarker.position.x, endMarker.position.y),
                            facing = facing,
                            holdSeconds = 0.25f,
                            lookAtTargetName = resolvedCue.lookAtTargetName
                        }
                    };
                }
                else
                {
                    resolvedCue.path = Array.Empty<SpringDay1DirectorPathPoint>();
                }
            }

            ApplyStorySceneCueRuntimeDefaults(resolvedCue);
            resolvedCue.EnsureDefaults();
            return resolvedCue;
        }

        private static bool TryBuildSceneMarkerCue(
            SpringDay1NpcCrowdManifest.Entry entry,
            out SpringDay1DirectorActorCue cue)
        {
            cue = null;
            if (entry == null
                || string.IsNullOrWhiteSpace(entry.npcId)
                || !TryResolveVillageCrowdMarkersStatic(entry.npcId, out Transform startMarker, out Transform endMarker))
            {
                return false;
            }

            Vector3 startPosition = startMarker.position;
            Vector3 endPosition = endMarker.position;
            Vector2 facing = entry.initialFacing.sqrMagnitude > 0.0001f
                ? entry.initialFacing.normalized
                : Vector2.down;
            Vector2 direction = new Vector2(endPosition.x - startPosition.x, endPosition.y - startPosition.y);
            if (direction.sqrMagnitude > 0.0001f)
            {
                facing = direction.normalized;
            }

            cue = new SpringDay1DirectorActorCue
            {
                cueId = $"scene-marker-{entry.npcId}",
                npcId = entry.npcId,
                semanticAnchorId = string.Empty,
                keepCurrentSpawnPosition = false,
                useSemanticAnchorAsStart = false,
                startPositionIsSemanticAnchorOffset = false,
                pathPointsAreOffsets = false,
                startPosition = new Vector2(startPosition.x, startPosition.y),
                facing = facing,
                lookAtTargetName = string.Empty,
                suspendRoam = true,
                loopPath = false,
                moveSpeed = StorySceneMarkerWalkSpeed,
                initialHoldSeconds = 0f,
                path = new[]
                {
                    new SpringDay1DirectorPathPoint
                    {
                        position = new Vector2(endPosition.x, endPosition.y),
                        facing = facing,
                        holdSeconds = 0.25f,
                        lookAtTargetName = string.Empty
                    }
                }
            };
            ApplyStorySceneCueRuntimeDefaults(cue);
            cue.EnsureDefaults();
            return true;
        }

        private static void ApplyStorySceneCueRuntimeDefaults(SpringDay1DirectorActorCue cue)
        {
            if (cue == null)
            {
                return;
            }

            cue.moveSpeed = Mathf.Max(cue.moveSpeed, StorySceneMarkerWalkSpeed);
            cue.lookAtTargetName = SpringDay1DirectorStagingPlayback.StoryPlayerLookTargetToken;
            if (cue.path == null)
            {
                return;
            }

            for (int index = 0; index < cue.path.Length; index++)
            {
                SpringDay1DirectorPathPoint point = cue.path[index];
                if (point == null)
                {
                    continue;
                }

                point.lookAtTargetName = SpringDay1DirectorStagingPlayback.StoryPlayerLookTargetToken;
            }
        }

        private static bool ShouldUseVillageCrowdSceneResidentStart(string beatKey)
        {
            return false;
        }

        private static bool ShouldUseVillageCrowdMarkerCueOverride(string beatKey)
        {
            return string.Equals(beatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase)
                || string.Equals(beatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase);
        }

        private bool TryResolveVillageCrowdMarkers(string npcId, out Transform startMarker, out Transform endMarker)
        {
            return TryResolveVillageCrowdMarkersStatic(npcId, out startMarker, out endMarker);
        }

        private static bool TryResolveVillageCrowdMarkersStatic(string npcId, out Transform startMarker, out Transform endMarker)
        {
            startMarker = null;
            endMarker = null;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            Transform root = FindFirstExistingAnchorStatic(VillageCrowdMarkerRootNames);
            if (root == null)
            {
                return false;
            }

            Transform startGroup = FindFirstExistingChild(root, VillageCrowdMarkerStartGroupNames);
            Transform endGroup = FindFirstExistingChild(root, VillageCrowdMarkerEndGroupNames);
            startMarker = FindVillageCrowdMarker(startGroup, npcId, start: true);
            endMarker = FindVillageCrowdMarker(endGroup, npcId, start: false);
            return startMarker != null && endMarker != null;
        }

        private static Transform FindFirstExistingAnchorStatic(string[] names)
        {
            if (names == null)
            {
                return null;
            }

            for (int index = 0; index < names.Length; index++)
            {
                Transform candidate = FindNamedTransformInActiveScene(names[index]);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static Transform FindNamedTransformInActiveScene(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return null;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < allTransforms.Length; index++)
            {
                Transform candidate = allTransforms[index];
                if (candidate == null
                    || candidate.gameObject.scene != activeScene
                    || !string.Equals(candidate.name, objectName, StringComparison.Ordinal))
                {
                    continue;
                }

                return candidate;
            }

            return null;
        }

        private static Transform FindFirstExistingChild(Transform root, string[] names)
        {
            if (root == null || names == null)
            {
                return null;
            }

            for (int index = 0; index < names.Length; index++)
            {
                string name = names[index];
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                Transform direct = root.Find(name.Trim());
                if (direct != null)
                {
                    return direct;
                }

                Transform recursive = FindChildRecursive(root, name.Trim());
                if (recursive != null)
                {
                    return recursive;
                }
            }

            return null;
        }

        private static Transform FindVillageCrowdMarker(Transform groupRoot, string npcId, bool start)
        {
            if (groupRoot == null || string.IsNullOrWhiteSpace(npcId))
            {
                return null;
            }

            foreach (string candidateName in EnumerateVillageCrowdMarkerNames(npcId, start))
            {
                if (string.IsNullOrWhiteSpace(candidateName))
                {
                    continue;
                }

                Transform marker = FindChildRecursive(groupRoot, candidateName);
                if (marker != null)
                {
                    return marker;
                }
            }

            return null;
        }

        private static IEnumerable<string> EnumerateVillageCrowdMarkerNames(string npcId, bool start)
        {
            string suffixCn = start ? VillageCrowdMarkerStartSuffixCn : VillageCrowdMarkerEndSuffixCn;
            string suffixEn = start ? "Start" : "End";
            foreach (string residentName in EnumerateResidentLookupNames(npcId))
            {
                if (string.IsNullOrWhiteSpace(residentName))
                {
                    continue;
                }

                string trimmed = residentName.Trim();
                yield return $"{trimmed}{suffixCn}";
                yield return $"{trimmed}_{suffixEn}";
                yield return $"{trimmed}{suffixEn}";
                yield return $"NPC{trimmed}{suffixCn}";
                yield return $"NPC{trimmed}_{suffixEn}";
            }
        }

        private static SpringDay1DirectorActorCue CloneCue(SpringDay1DirectorActorCue source)
        {
            if (source == null)
            {
                return null;
            }

            SpringDay1DirectorActorCue clone = new SpringDay1DirectorActorCue
            {
                cueId = source.cueId,
                npcId = source.npcId,
                semanticAnchorId = source.semanticAnchorId,
                duty = source.duty,
                keepCurrentSpawnPosition = source.keepCurrentSpawnPosition,
                useSemanticAnchorAsStart = source.useSemanticAnchorAsStart,
                startPositionIsSemanticAnchorOffset = source.startPositionIsSemanticAnchorOffset,
                pathPointsAreOffsets = source.pathPointsAreOffsets,
                startPosition = source.startPosition,
                facing = source.facing,
                lookAtTargetName = source.lookAtTargetName,
                suspendRoam = source.suspendRoam,
                loopPath = source.loopPath,
                moveSpeed = source.moveSpeed,
                initialHoldSeconds = source.initialHoldSeconds
            };

            SpringDay1DirectorPathPoint[] sourcePath = source.path ?? Array.Empty<SpringDay1DirectorPathPoint>();
            SpringDay1DirectorPathPoint[] clonedPath = new SpringDay1DirectorPathPoint[sourcePath.Length];
            for (int index = 0; index < sourcePath.Length; index++)
            {
                SpringDay1DirectorPathPoint point = sourcePath[index];
                if (point == null)
                {
                    clonedPath[index] = null;
                    continue;
                }

                clonedPath[index] = new SpringDay1DirectorPathPoint
                {
                    position = point.position,
                    facing = point.facing,
                    holdSeconds = point.holdSeconds,
                    lookAtTargetName = point.lookAtTargetName
                };
            }

            clone.path = clonedPath;
            clone.EnsureDefaults();
            return clone;
        }

        private bool AreBeatCuesSettled(string beatKey)
        {
            if (string.IsNullOrWhiteSpace(beatKey))
            {
                return true;
            }

            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : null;
            if (entries == null || entries.Length == 0)
            {
                return true;
            }

            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
                {
                    continue;
                }

                if (!TryResolveStagingCueForCurrentScene(beatKey, entry, out SpringDay1DirectorActorCue cue))
                {
                    continue;
                }

                SpringDay1DirectorActorCue runtimeCue = ResolveRuntimeCueOverride(beatKey, entry, cue);
                string expectedCueKey = runtimeCue != null ? runtimeCue.StableKey : cue.StableKey;

                if (!_spawnStates.TryGetValue(entry.npcId, out SpawnState state)
                    || state?.Instance == null
                    || TryHoldForRehearsal(state)
                    || TryHoldForManualPreview(state))
                {
                    return false;
                }

                SpringDay1DirectorStagingPlayback playback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
                if (playback == null
                    || !string.Equals(playback.CurrentCueKey, expectedCueKey, StringComparison.OrdinalIgnoreCase)
                    || !playback.HasSettledCurrentCue())
                {
                    return false;
                }
            }

            return true;
        }

        private bool ForceSettleBeatCueInternal(string beatKey)
        {
            if (string.IsNullOrWhiteSpace(beatKey))
            {
                return true;
            }

            SyncCrowd();

            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : null;
            if (entries == null || entries.Length == 0)
            {
                return true;
            }

            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
                {
                    continue;
                }

                if (!TryResolveStagingCueForCurrentScene(beatKey, entry, out SpringDay1DirectorActorCue cue))
                {
                    continue;
                }

                if (!_spawnStates.TryGetValue(entry.npcId, out SpawnState state) || state?.Instance == null)
                {
                    return false;
                }

                ApplyStagingCue(entry, state, beatKey);

                SpringDay1DirectorStagingPlayback playback = state.Instance.GetComponent<SpringDay1DirectorStagingPlayback>();
                if (playback == null || !playback.ForceSnapToCueEnd())
                {
                    return false;
                }
            }

            return AreBeatCuesSettled(beatKey);
        }

        private void ApplyResidentBaseline(
            SpringDay1NpcCrowdManifest.Entry entry,
            SpawnState state,
            StoryPhase currentPhase,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (entry == null || state?.Instance == null)
            {
                return;
            }

            SetResidentCueInteractionLock(state, active: false);

            if (ShouldDeferToStoryEscortDirector(entry, currentPhase))
            {
                CancelResidentReturnHome(state, resumeRoam: false);
                SetEntryActive(entry.npcId, true);
                AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false);

                state.ReleasedFromDirectorCue = false;
                state.NeedsResidentReset = false;
                state.ResolvedAnchorName = $"story-escort:{currentPhase}";
                state.UsedFallback = false;
                return;
            }

            bool shouldRemainActive = ShouldKeepResidentActive(entry, currentPhase, beatKey, beatConsumption);
            state.ResidentGroupKey = string.Empty;
            if (state.IsNightResting)
            {
                shouldRemainActive = !state.HideWhileNightResting;
            }
            else if (ShouldResidentsReturnHomeByClock() && state.HideWhileNightResting)
            {
                shouldRemainActive = false;
            }

            SetEntryActive(entry.npcId, shouldRemainActive);

            if (!shouldRemainActive)
            {
                CancelResidentReturnHome(state, resumeRoam: false);
                state.ReleasedFromDirectorCue = false;
                state.NeedsResidentReset = false;
                state.ResolvedAnchorName = BuildResidentSummaryAnchor(currentPhase, beatKey, hidden: true);
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.IsNightResting)
            {
                ApplyResidentNightRestState(state);
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.IsReturningHome)
            {
                if (!ShouldKeepResidentReturnHomeContract(state, currentPhase))
                {
                    CancelResidentReturnHome(state, resumeRoam: false);
                    if (TryReleaseResidentToDaytimeBaseline(entry, state, currentPhase, beatKey))
                    {
                        state.ResolvedAnchorName = $"baseline-release:{state.BaseResolvedAnchorName}";
                        state.UsedFallback = state.BaseUsedFallback;
                        return;
                    }

                    ReleaseResidentToAutonomousRoam(state);
                    state.ResolvedAnchorName = $"autonomous:{state.BaseResolvedAnchorName}";
                    state.UsedFallback = state.BaseUsedFallback;
                    return;
                }

                state.ResolvedAnchorName = BuildResidentSummaryAnchor(currentPhase, beatKey, hidden: false) + ":return-home";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.ReleasedFromDirectorCue && state.NeedsResidentReset)
            {
                bool shouldQueueClockOwnedReturnHome = ShouldAllowResidentReturnHome(currentPhase)
                    && ShouldQueueResidentReturnHomeAfterCueRelease(state, currentPhase);
                if (shouldQueueClockOwnedReturnHome
                    && TryBeginResidentReturnHome(state))
                {
                    state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                    state.UsedFallback = state.BaseUsedFallback;
                    return;
                }

                if (shouldQueueClockOwnedReturnHome
                    && QueueResidentReturnHomeRetry(state))
                {
                    state.ResolvedAnchorName = state.HideWhileNightResting && !state.Instance.activeSelf
                        ? $"night-return-hidden:{state.BaseResolvedAnchorName}"
                        : $"return-home-pending:{state.BaseResolvedAnchorName}";
                    state.UsedFallback = state.BaseUsedFallback;
                    return;
                }

                if (TryReleaseResidentToDaytimeBaseline(entry, state, currentPhase, beatKey))
                {
                    state.ResolvedAnchorName = $"baseline-release:{state.BaseResolvedAnchorName}";
                    state.UsedFallback = state.BaseUsedFallback;
                    return;
                }

                ReleaseResidentToAutonomousRoam(state);
                state.ResolvedAnchorName = $"autonomous:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                roamController = ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
                if (roamController != null && !roamController.IsRoaming)
                {
                    QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);
                }
            }

            if (roamController == null)
            {
                NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
                if (motionController != null)
                {
                    motionController.StopMotion();
                    ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
                }
            }

            state.ResolvedAnchorName = BuildResidentSummaryAnchor(currentPhase, beatKey, hidden: false);
            state.UsedFallback = state.BaseUsedFallback;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
        }

        private static bool ShouldDeferToStoryEscortDirector(SpringDay1NpcCrowdManifest.Entry entry, StoryPhase currentPhase)
        {
            if (entry == null)
            {
                return false;
            }

            string npcId = entry.npcId != null ? entry.npcId.Trim() : string.Empty;
            bool isStoryEscort = string.Equals(npcId, StoryEscortChiefNpcId, StringComparison.OrdinalIgnoreCase)
                || string.Equals(npcId, StoryEscortCompanionNpcId, StringComparison.OrdinalIgnoreCase);
            if (!isStoryEscort)
            {
                return false;
            }

            if (currentPhase == StoryPhase.FarmingTutorial
                && SpringDay1Director.Instance != null
                && SpringDay1Director.Instance.IsPostTutorialExploreWindowActive())
            {
                return false;
            }

            return currentPhase == StoryPhase.EnterVillage
                || currentPhase == StoryPhase.HealingAndHP
                || currentPhase == StoryPhase.WorkbenchFlashback
                || currentPhase == StoryPhase.FarmingTutorial;
        }

        private void TickResidentReturns()
        {
            if (_spawnStates.Count <= 0)
            {
                return;
            }

            List<SpawnState> states = new List<SpawnState>(_spawnStates.Values);
            for (int index = 0; index < states.Count; index++)
            {
                if (TryHoldForRehearsal(states[index]) || TryHoldForManualPreview(states[index]))
                {
                    continue;
                }

                TickResidentReturnHome(states[index], Time.deltaTime);
            }
        }

        private void SnapResidentsToHomeAnchorsInternal()
        {
            if (_spawnStates.Count <= 0)
            {
                return;
            }

            foreach (SpawnState state in _spawnStates.Values)
            {
                if (state == null
                    || state.Instance == null
                    || state.HomeAnchor == null
                    || TryHoldForRehearsal(state)
                    || TryHoldForManualPreview(state))
                {
                    continue;
                }

                state.IsNightResting = true;
                state.HideWhileNightResting = true;
                ApplyResidentNightRestState(state);
            }

            Physics2D.SyncTransforms();
            _syncRequested = true;
        }

        private void TickResidentReturnHome(SpawnState state, float deltaTime)
        {
            if (state == null || !state.IsReturningHome || state.Instance == null)
            {
                return;
            }

            if (state.HomeAnchor == null)
            {
                FinishResidentReturnHome(state);
                return;
            }

            Transform homeAnchor = state.HomeAnchor.transform;
            Vector3 currentPosition = state.Instance.transform.position;
            Vector3 targetPosition = homeAnchor.position;
            targetPosition.z = currentPosition.z;
            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();

            if (TryFinishResidentReturnHomeFromNavigationCompletion(state, roamController, targetPosition))
            {
                return;
            }

            // 这条只保留成 legacy/repair 兜底，不再作为 FormalNavigation 到达收尾的主路径。
            if (IsResidentAtHomeAnchor(state, roamController))
            {
                state.Instance.transform.position = targetPosition;
                FinishResidentReturnHome(state);
                return;
            }

            if (HasActiveResidentReturnHomeDrive(roamController))
            {
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            float now = Time.unscaledTime;
            if (state.NextReturnHomeDriveRetryAt > 0f && now < state.NextReturnHomeDriveRetryAt)
            {
                state.ResolvedAnchorName = $"return-home-pending:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (TryDriveResidentReturnHome(state, roamController))
            {
                state.NextReturnHomeDriveRetryAt = 0f;
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            KeepResidentPendingReturnHome(state, now + ResidentReturnHomeRetryInterval);
            state.ResolvedAnchorName = $"return-home-pending:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
        }

        private bool TryFinishResidentReturnHomeFromNavigationCompletion(
            SpawnState state,
            NPCAutoRoamController roamController,
            Vector3 targetPosition)
        {
            if (state == null || state.Instance == null || roamController == null)
            {
                return false;
            }

            if (!roamController.TryConsumeFormalNavigationArrival(out Vector2 arrivalPosition))
            {
                return false;
            }

            float acceptanceRadius = GetResidentReturnHomeArrivalRadius();
            Vector2 comparableArrivalTarget = ResolveResidentReturnHomeArrivalTarget(state, roamController, targetPosition);
            if (Vector2.Distance(arrivalPosition, targetPosition) > acceptanceRadius
                && Vector2.Distance(arrivalPosition, comparableArrivalTarget) > acceptanceRadius)
            {
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return false;
            }

            state.Instance.transform.position = targetPosition;
            FinishResidentReturnHome(state);
            return true;
        }

        private bool TryBeginResidentReturnHome(SpawnState state)
        {
            if (state == null || state.Instance == null || state.HomeAnchor == null || state.IsReturningHome)
            {
                return false;
            }

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            if (IsResidentAtHomeAnchor(state, roamController))
            {
                HideResidentAtHomeForNightReturn(state);
                return false;
            }

            float now = Time.unscaledTime;
            if (state.NextReturnHomeDriveRetryAt > 0f && now < state.NextReturnHomeDriveRetryAt)
            {
                return false;
            }

            if (TryDriveResidentReturnHome(state))
            {
                state.IsReturningHome = true;
                state.ResumeRoamAfterReturn = false;
                state.NextReturnHomeDriveRetryAt = 0f;
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return true;
            }

            QueueResidentReturnHomeRetry(state, now + ResidentReturnHomeRetryInterval);
            return false;
        }

        private bool ShouldAllowResidentReturnHome(StoryPhase currentPhase)
        {
            return ShouldResidentsReturnHomeByClock();
        }

        private bool ShouldAllowResidentReturnHome(SpawnState state, StoryPhase currentPhase)
        {
            if (state != null && state.ResumeRoamAfterReturn)
            {
                return true;
            }

            return ShouldAllowResidentReturnHome(currentPhase);
        }

        private static bool ShouldQueueResidentReturnHomeAfterCueRelease(SpawnState state, StoryPhase currentPhase)
        {
            if (state?.Instance == null || state.HomeAnchor == null)
            {
                return false;
            }

            return !IsResidentAtHomeAnchor(state);
        }

        private bool ShouldKeepResidentReturnHomeContract(SpawnState state, StoryPhase currentPhase)
        {
            if (state == null)
            {
                return false;
            }

            if (state.ReleasedFromDirectorCue && state.NeedsResidentReset)
            {
                return ShouldAllowResidentReturnHome(currentPhase);
            }

            return ShouldAllowResidentReturnHome(state, currentPhase);
        }

        private bool TryReleaseResidentToDaytimeBaseline(
            SpringDay1NpcCrowdManifest.Entry entry,
            SpawnState state,
            StoryPhase currentPhase,
            string beatKey)
        {
            if (state?.Instance == null)
            {
                return false;
            }

            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false);

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            motionController?.StopMotion();

            Vector3 currentPosition = state.Instance.transform.position;
            Vector3 releasePosition = currentPosition;
            releasePosition.z = state.Instance.transform.position.z;
            string promotedAnchorName = state.BaseResolvedAnchorName;
            bool shouldPromoteBasePose = false;

            if (ShouldUseResidentDaytimeSemanticBaseline(currentPhase, beatKey)
                && TryResolveResidentDaytimeBaselinePosition(entry, out Vector3 daytimeBaselinePosition, out string daytimeBaselineAnchorName))
            {
                releasePosition = new Vector3(daytimeBaselinePosition.x, daytimeBaselinePosition.y, state.Instance.transform.position.z);
                promotedAnchorName = daytimeBaselineAnchorName;
                shouldPromoteBasePose = true;
            }

            if (roamController != null)
            {
                if (roamController.TryResolveOccupiablePosition((Vector2)releasePosition, out Vector2 occupiableReleasePosition))
                {
                    releasePosition = new Vector3(occupiableReleasePosition.x, occupiableReleasePosition.y, state.Instance.transform.position.z);
                }
                else if (roamController.TryResolveOccupiablePosition((Vector2)currentPosition, out Vector2 occupiableCurrentPosition))
                {
                    releasePosition = new Vector3(occupiableCurrentPosition.x, occupiableCurrentPosition.y, state.Instance.transform.position.z);
                    shouldPromoteBasePose = false;
                    promotedAnchorName = state.BaseResolvedAnchorName;
                }
            }

            state.Instance.transform.position = releasePosition;
            if (motionController != null)
            {
                ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
            }

            if (shouldPromoteBasePose)
            {
                state.BasePosition = releasePosition;
                state.BaseResolvedAnchorName = promotedAnchorName;
                state.BaseUsedFallback = false;
            }

            Physics2D.SyncTransforms();
            QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
            return true;
        }

        private static bool ShouldUseResidentDaytimeSemanticBaseline(StoryPhase currentPhase, string beatKey)
        {
            return false;
        }

        private static bool TryResolveResidentDaytimeBaselinePosition(
            SpringDay1NpcCrowdManifest.Entry entry,
            out Vector3 worldPosition,
            out string anchorName)
        {
            worldPosition = Vector3.zero;
            anchorName = string.Empty;
            return false;
        }

        private static bool IsResidentDaytimeBaselineSemanticAnchor(string semanticAnchorId)
        {
            if (string.IsNullOrWhiteSpace(semanticAnchorId))
            {
                return false;
            }

            string trimmed = semanticAnchorId.Trim();
            return trimmed.IndexOf("DailyStand", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsDeprecatedRuntimeSemanticAnchor(string semanticAnchorId)
        {
            if (string.IsNullOrWhiteSpace(semanticAnchorId))
            {
                return false;
            }

            string trimmed = semanticAnchorId.Trim();
            return string.Equals(trimmed, "EnterVillageCrowdRoot", StringComparison.OrdinalIgnoreCase)
                || string.Equals(trimmed, "DinnerBackgroundRoot", StringComparison.OrdinalIgnoreCase)
                || string.Equals(trimmed, "NightWitness_01", StringComparison.OrdinalIgnoreCase)
                || trimmed.IndexOf("DailyStand_", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryDriveResidentReturnHome(SpawnState state)
        {
            NPCAutoRoamController roamController = state != null && state.Instance != null
                ? state.Instance.GetComponent<NPCAutoRoamController>()
                : null;
            return TryDriveResidentReturnHome(state, roamController);
        }

        private static bool TryDriveResidentReturnHome(SpawnState state, NPCAutoRoamController roamController)
        {
            if (state?.Instance == null || state.HomeAnchor == null)
            {
                return false;
            }

            if (roamController == null)
            {
                return false;
            }

            return roamController.RequestReturnToAnchor(
                ResidentScriptedControlOwnerKey,
                state.HomeAnchor.transform,
                resumeAutonomousRoamWhenReleased: !state.IsNightResting,
                retargetTolerance: GetResidentReturnHomeArrivalRadius());
        }

        private static bool HasActiveResidentReturnHomeDrive(NPCAutoRoamController roamController)
        {
            return roamController != null && roamController.IsFormalNavigationDriveActive();
        }

        private static bool QueueResidentReturnHomeRetry(SpawnState state, float nextRetryAt = -1f)
        {
            if (state?.Instance == null)
            {
                return false;
            }

            if (state.HideWhileNightResting && !state.Instance.activeSelf)
            {
                state.IsReturningHome = false;
                state.ResumeRoamAfterReturn = false;
                state.NextReturnHomeDriveRetryAt = 0f;
                return true;
            }

            if (state.HomeAnchor == null)
            {
                return false;
            }

            ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: true);
            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            if (nextRetryAt >= 0f)
            {
                state.NextReturnHomeDriveRetryAt = nextRetryAt;
            }
            else if (state.NextReturnHomeDriveRetryAt <= 0f)
            {
                state.NextReturnHomeDriveRetryAt = Time.unscaledTime + ResidentReturnHomeRetryInterval;
            }

            return true;
        }

        private static bool KeepResidentPendingReturnHome(SpawnState state, float nextRetryAt = -1f)
        {
            if (state?.Instance == null)
            {
                return false;
            }

            if (state.HideWhileNightResting && !state.Instance.activeSelf)
            {
                state.IsReturningHome = false;
                state.ResumeRoamAfterReturn = false;
                state.NextReturnHomeDriveRetryAt = 0f;
                return true;
            }

            if (state.HomeAnchor == null)
            {
                return false;
            }

            ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: true);

            state.IsReturningHome = true;
            state.ResumeRoamAfterReturn = false;
            if (nextRetryAt >= 0f)
            {
                state.NextReturnHomeDriveRetryAt = nextRetryAt;
            }
            else if (state.NextReturnHomeDriveRetryAt <= 0f)
            {
                state.NextReturnHomeDriveRetryAt = Time.unscaledTime + ResidentReturnHomeRetryInterval;
            }

            return true;
        }

        private void FinishResidentReturnHome(SpawnState state)
        {
            if (state == null || state.Instance == null)
            {
                return;
            }

            NPCAutoRoamController roamController = ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            if (ShouldHideResidentImmediatelyAfterNightReturn())
            {
                HideResidentAtHomeForNightReturn(state, roamController, motionController);
                return;
            }

            if (motionController != null)
            {
                motionController.StopMotion();
                if (!state.ResumeRoamAfterReturn || state.IsNightResting)
                {
                    ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
                }
            }

            if (state.ResumeRoamAfterReturn && !state.IsNightResting)
            {
                if (roamController != null)
                {
                    QueueResidentAutonomousRoamResume(state, tryImmediateMove: true);
                }
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
        }

        private static bool IsResidentAtHomeAnchor(SpawnState state, NPCAutoRoamController roamController = null)
        {
            if (state?.Instance == null || state.HomeAnchor == null)
            {
                return false;
            }

            Vector3 currentPosition = state.Instance.transform.position;
            Vector3 homePosition = state.HomeAnchor.transform.position;
            homePosition.z = currentPosition.z;
            float arrivalRadius = GetResidentReturnHomeArrivalRadius();
            Vector2 currentPosition2D = currentPosition;
            Vector2 homePosition2D = homePosition;
            if (Vector2.Distance(currentPosition2D, homePosition2D) <= arrivalRadius)
            {
                return true;
            }

            Vector2 comparableArrivalTarget = ResolveResidentReturnHomeArrivalTarget(state, roamController, homePosition);
            return Vector2.Distance(currentPosition2D, comparableArrivalTarget) <= arrivalRadius;
        }

        private static float GetResidentReturnHomeArrivalRadius()
        {
            return Mathf.Max(ResidentReturnHomeThreshold * 4f, ResidentReturnHomeArrivalRadius);
        }

        private static Vector2 ResolveResidentReturnHomeArrivalTarget(
            SpawnState state,
            NPCAutoRoamController roamController,
            Vector3 homePosition)
        {
            Vector2 comparableTarget = homePosition;
            if (state?.Instance == null)
            {
                return comparableTarget;
            }

            roamController ??= state.Instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null
                && roamController.TryResolveFormalNavigationDestination(homePosition, out Vector2 resolvedComparableTarget))
            {
                comparableTarget = resolvedComparableTarget;
            }

            return comparableTarget;
        }

        private bool ShouldHideResidentImmediatelyAfterNightReturn()
        {
            return ShouldResidentsReturnHomeByClock() && !ShouldResidentsRestByClock();
        }

        private void HideResidentAtHomeForNightReturn(
            SpawnState state,
            NPCAutoRoamController roamController = null,
            NPCMotionController motionController = null)
        {
            if (state == null || state.Instance == null)
            {
                return;
            }

            roamController ??= ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
            motionController ??= state.Instance.GetComponent<NPCMotionController>();

            Vector3 targetPosition = state.HomeAnchor != null
                ? state.HomeAnchor.transform.position
                : state.Instance.transform.position;
            targetPosition.z = state.Instance.transform.position.z;

            if (roamController != null)
            {
                roamController.SnapToTarget((Vector2)targetPosition, state.BaseFacing);
            }
            else
            {
                state.Instance.transform.position = targetPosition;
            }

            if (motionController != null)
            {
                motionController.StopMotion();
                ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
            }
            else
            {
                state.Instance.GetComponent<NPCMotionController>()?.StopMotion();
            }

            state.HideWhileNightResting = true;
            state.Instance.SetActive(false);
            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = $"night-return-hidden:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
            Physics2D.SyncTransforms();
        }

        private void CancelResidentReturnHome(SpawnState state, bool resumeRoam)
        {
            if (state == null)
            {
                return;
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;

            if (!resumeRoam || state.Instance == null || state.IsNightResting)
            {
                return;
            }

            QueueResidentAutonomousRoamResume(state, tryImmediateMove: false);
        }

        private bool ShouldResidentsRestByClock()
        {
            TimeManager timeManager = ResolveCrowdScheduleTimeManager();
            if (timeManager == null)
            {
                return false;
            }

            int currentHour = timeManager.GetHour();
            return currentHour >= ResidentForcedRestHour || currentHour < ResidentMorningReleaseHour;
        }

        private bool ShouldResidentsReturnHomeByClock()
        {
            TimeManager timeManager = ResolveCrowdScheduleTimeManager();
            if (timeManager == null)
            {
                return false;
            }

            int currentHour = timeManager.GetHour();
            return currentHour >= ResidentReturnHomeHour && currentHour < ResidentForcedRestHour;
        }

        private static TimeManager ResolveCrowdScheduleTimeManager()
        {
            if (!Application.isPlaying)
            {
                return FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
            }

            return TimeManager.Instance;
        }

        private void SyncResidentNightRestSchedule()
        {
            if (_spawnStates.Count <= 0)
            {
                return;
            }

            bool shouldRest = ShouldResidentsRestByClock();
            bool shouldReturnHome = ShouldResidentsReturnHomeByClock();
            foreach (SpawnState state in _spawnStates.Values)
            {
                if (state == null || state.Instance == null)
                {
                    continue;
                }

                if (shouldRest)
                {
                    if (state.IsNightResting && state.HideWhileNightResting)
                    {
                        continue;
                    }

                    state.IsNightResting = true;
                    state.HideWhileNightResting = true;
                    ApplyResidentNightRestState(state);
                    _syncRequested = true;
                    continue;
                }

                if (state.IsNightResting)
                {
                    state.IsNightResting = false;
                    state.HideWhileNightResting = false;
                    RestoreResidentFromNightRestState(state);
                    _syncRequested = true;
                }

                if (state.HomeAnchor == null)
                {
                    continue;
                }

                if (shouldReturnHome)
                {
                    if (!state.IsNightResting && state.HideWhileNightResting && !state.Instance.activeSelf)
                    {
                        continue;
                    }

                    if (state.IsReturningHome)
                    {
                        continue;
                    }

                    if (TryBeginResidentReturnHome(state))
                    {
                        state.HideWhileNightResting = false;
                        _syncRequested = true;
                    }

                    continue;
                }

                if (state.IsReturningHome)
                {
                    CancelResidentReturnHome(state, resumeRoam: true);
                    _syncRequested = true;
                }
            }
        }

        private static void RestoreResidentFromNightRestState(SpawnState state)
        {
            if (state == null || state.Instance == null)
            {
                return;
            }

            state.Instance.SetActive(true);

            Vector3 targetPosition = state.Instance.transform.position;
            if (state.HomeAnchor != null)
            {
                targetPosition = state.HomeAnchor.transform.position;
                targetPosition.z = state.Instance.transform.position.z;
            }

            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            if (roamController != null)
            {
                ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
                roamController.SnapToTarget((Vector2)targetPosition, state.BaseFacing);
            }
            else
            {
                state.Instance.transform.position = targetPosition;
            }

            state.Instance.GetComponent<NPCMotionController>()?.StopMotion();
            Physics2D.SyncTransforms();
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
        }

        private static void ApplyResidentNightRestState(SpawnState state)
        {
            if (state == null || state.Instance == null)
            {
                return;
            }

            state.Instance.SetActive(true);

            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            Vector3 targetPosition = state.Instance.transform.position;
            if (state.HomeAnchor != null)
            {
                targetPosition = state.HomeAnchor.transform.position;
                targetPosition.z = state.Instance.transform.position.z;
            }

            if (roamController != null)
            {
                ReleaseResidentSharedRuntimeControl(state, resumeResidentLogic: false);
                roamController.SnapToTarget((Vector2)targetPosition, state.BaseFacing);
            }
            else
            {
                state.Instance.transform.position = targetPosition;
            }

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            if (motionController != null)
            {
                motionController.StopMotion();
                ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
            }

            if (state.HideWhileNightResting)
            {
                state.Instance.SetActive(false);
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NextReturnHomeDriveRetryAt = 0f;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = state.HideWhileNightResting
                ? $"night-rest-hidden:{state.BaseResolvedAnchorName}"
                : $"night-rest:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
        }

        private bool ShouldKeepResidentActive(
            SpringDay1NpcCrowdManifest.Entry entry,
            StoryPhase currentPhase,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (entry == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(beatKey)
                && TryResolveBeatPresence(beatConsumption, entry.npcId, out SpringDay1CrowdResidentPresenceLevel presence))
            {
                if (presence == SpringDay1CrowdResidentPresenceLevel.Visible
                    || presence == SpringDay1CrowdResidentPresenceLevel.Pressure
                    || presence == SpringDay1CrowdResidentPresenceLevel.Background)
                {
                    return true;
                }

                if (presence == SpringDay1CrowdResidentPresenceLevel.Trace)
                {
                    return false;
                }
            }

            if (currentPhase < StoryPhase.EnterVillage)
            {
                return entry.residentBaseline == SpringDay1CrowdResidentBaseline.DaytimeResident;
            }

            if (currentPhase >= StoryPhase.FreeTime)
            {
                return true;
            }

            return entry.residentBaseline != SpringDay1CrowdResidentBaseline.NightResident;
        }

        private static string BuildResidentSummaryAnchor(
            StoryPhase currentPhase,
            string beatKey,
            bool hidden)
        {
            string beatLabel = string.IsNullOrWhiteSpace(beatKey) ? currentPhase.ToString() : beatKey;
            return hidden
                ? $"resident-hidden:{beatLabel}"
                : $"resident:{beatLabel}";
        }

        private void ReparentState(SpawnState state, Transform newParent, string residentGroupKey)
        {
            if (state == null)
            {
                return;
            }

            if (state.Instance != null && newParent != null && state.Instance.transform.parent != newParent)
            {
                state.Instance.transform.SetParent(newParent, true);
            }

            if (state.HomeAnchor != null
                && state.OwnsHomeAnchor
                && newParent != null
                && state.HomeAnchor.transform.parent != newParent)
            {
                state.HomeAnchor.transform.SetParent(newParent, true);
            }

            state.ResidentGroupKey = residentGroupKey ?? string.Empty;
        }

        private void EnsureSceneRoot(Scene scene)
        {
            EnsureSceneLookupCache(scene);
        }

        private void InvalidateSceneLookupCache()
        {
            _sceneLookupCacheHandle = -1;
            _sceneGameObjectLookupCache.Clear();
            _sceneTransformLookupCache.Clear();
            _sceneMissingTransformLookupCache.Clear();
        }

        private void EnsureSceneLookupCache(Scene scene)
        {
            if (!scene.IsValid())
            {
                InvalidateSceneLookupCache();
                return;
            }

            if (_sceneLookupCacheHandle == scene.handle)
            {
                return;
            }

            _sceneLookupCacheHandle = scene.handle;
            _sceneGameObjectLookupCache.Clear();
            _sceneTransformLookupCache.Clear();
            _sceneMissingTransformLookupCache.Clear();

            GameObject[] sceneRoots = scene.GetRootGameObjects();
            for (int index = 0; index < sceneRoots.Length; index++)
            {
                CacheSceneHierarchy(sceneRoots[index], scene);
            }
        }

        private void CacheSceneHierarchy(GameObject rootObject, Scene scene)
        {
            if (rootObject == null || rootObject.scene != scene)
            {
                return;
            }

            CacheSceneHierarchy(rootObject.transform, scene);
        }

        private void CacheSceneHierarchy(Transform root, Scene scene)
        {
            if (root == null || root.gameObject.scene != scene)
            {
                return;
            }

            string name = root.name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!_sceneGameObjectLookupCache.ContainsKey(name))
                {
                    _sceneGameObjectLookupCache[name] = root.gameObject;
                }

                if (!_sceneTransformLookupCache.ContainsKey(name))
                {
                    _sceneTransformLookupCache[name] = root;
                }
            }

            for (int index = 0; index < root.childCount; index++)
            {
                CacheSceneHierarchy(root.GetChild(index), scene);
            }
        }

        private GameObject FindSceneRoot(string rootName, Scene scene)
        {
            if (!scene.IsValid() || string.IsNullOrWhiteSpace(rootName))
            {
                return null;
            }

            EnsureSceneLookupCache(scene);
            if (_sceneGameObjectLookupCache.TryGetValue(rootName, out GameObject cachedRoot)
                && cachedRoot != null
                && cachedRoot.scene == scene)
            {
                return cachedRoot;
            }

            return null;
        }

        private List<ResidentControlProbeEntry> CaptureResidentControlProbeInternal()
        {
            List<ResidentControlProbeEntry> entries = new List<ResidentControlProbeEntry>();
            List<string> keys = new List<string>(_spawnStates.Keys);
            keys.Sort(StringComparer.OrdinalIgnoreCase);

            for (int index = 0; index < keys.Count; index++)
            {
                string npcId = keys[index];
                if (!_spawnStates.TryGetValue(npcId, out SpawnState state) || state == null)
                {
                    continue;
                }

                ResidentControlProbeEntry entry = new ResidentControlProbeEntry
                {
                    npcId = npcId,
                    isActive = state.Instance != null && state.Instance.activeSelf,
                    residentGroupKey = state.ResidentGroupKey ?? string.Empty,
                    resolvedAnchorName = state.ResolvedAnchorName ?? string.Empty,
                    baseResolvedAnchorName = state.BaseResolvedAnchorName ?? string.Empty,
                    appliedBeatKey = state.AppliedBeatKey ?? string.Empty,
                    appliedCueKey = state.AppliedCueKey ?? string.Empty,
                    needsResidentReset = state.NeedsResidentReset,
                    releasedFromDirectorCue = state.ReleasedFromDirectorCue,
                    isReturningHome = state.IsReturningHome,
                    resumeRoamAfterReturn = state.ResumeRoamAfterReturn,
                    isNightResting = state.IsNightResting,
                    basePositionX = state.BasePosition.x,
                    basePositionY = state.BasePosition.y,
                    hasHomeAnchor = state.HomeAnchor != null
                };

                if (state.Instance != null)
                {
                    entry.positionX = state.Instance.transform.position.x;
                    entry.positionY = state.Instance.transform.position.y;
                }

                if (state.HomeAnchor != null)
                {
                    entry.homeAnchorX = state.HomeAnchor.transform.position.x;
                    entry.homeAnchorY = state.HomeAnchor.transform.position.y;
                }

                NPCAutoRoamController roamController = state.Instance != null
                    ? state.Instance.GetComponent<NPCAutoRoamController>()
                    : null;
                if (roamController != null)
                {
                    entry.roamControllerEnabled = roamController.enabled;
                    entry.isRoaming = roamController.IsRoaming;
                    entry.isMoving = roamController.IsMoving;
                    entry.scriptedControlActive = roamController.IsResidentScriptedControlActive;
                    entry.scriptedControlOwnerKey = roamController.ResidentScriptedControlOwnerKey;
                    entry.scriptedMoveActive = roamController.IsResidentScriptedMoveActive;
                    entry.scriptedMovePaused = roamController.IsResidentScriptedMovePaused;
                    entry.resumeRoamWhenResidentControlReleases = roamController.ResumeRoamWhenResidentControlReleases;
                    entry.roamState = roamController.DebugState;
                    entry.lastMoveSkipReason = roamController.DebugLastMoveSkipReason;
                    entry.blockedAdvanceFrames = roamController.DebugBlockedAdvanceFrames;
                    entry.consecutivePathBuildFailures = roamController.DebugConsecutivePathBuildFailures;
                    entry.sharedAvoidanceBlockingFrames = roamController.DebugSharedAvoidanceBlockingFrames;
                    entry.hasSharedAvoidanceDetour = roamController.DebugHasSharedAvoidanceDetour;
                }

                entries.Add(entry);
            }

            return entries;
        }

        private static Transform FindChildRecursive(Transform root, string childName)
        {
            if (root == null)
            {
                return null;
            }

            for (int index = 0; index < root.childCount; index++)
            {
                Transform child = root.GetChild(index);
                if (string.Equals(child.name, childName, StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }

                Transform nested = FindChildRecursive(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private void PruneDestroyedStates()
        {
            List<string> keys = new List<string>(_spawnStates.Keys);
            for (int index = 0; index < keys.Count; index++)
            {
                string npcId = keys[index];
                SpawnState state = _spawnStates[npcId];
                if (state.Instance == null || (state.OwnsHomeAnchor && state.HomeAnchor == null))
                {
                    if (state.OwnsInstance && state.Instance != null)
                    {
                        Destroy(state.Instance);
                    }

                    if (state.OwnsHomeAnchor && state.HomeAnchor != null)
                    {
                        Destroy(state.HomeAnchor);
                    }

                    _spawnStates.Remove(npcId);
                }
            }
        }

        private void TeardownAll()
        {
            List<string> keys = new List<string>(_spawnStates.Keys);
            for (int index = 0; index < keys.Count; index++)
            {
                DestroyState(keys[index]);
            }

            _spawnStates.Clear();
            InvalidateSceneLookupCache();
            _enterVillageCrowdReleaseLatched = false;
        }

        private void DestroyState(string npcId)
        {
            if (!_spawnStates.TryGetValue(npcId, out SpawnState state))
            {
                return;
            }

            SetResidentCueInteractionLock(state, active: false);

            if (state.OwnsInstance && state.Instance != null)
            {
                Destroy(state.Instance);
            }

            if (state.OwnsHomeAnchor && state.HomeAnchor != null)
            {
                Destroy(state.HomeAnchor);
            }

            _spawnStates.Remove(npcId);
        }

        private static bool IsRuntimeSceneActive()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return IsSupportedRuntimeScene(activeScene);
        }

        private static bool IsSupportedRuntimeScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return false;
            }

            return scene.name == PrimarySceneName || scene.name == TownSceneName;
        }

        private GameObject FindSceneResident(SpringDay1NpcCrowdManifest.Entry entry, Scene scene)
        {
            if (entry == null || !scene.IsValid())
            {
                return null;
            }

            EnsureSceneLookupCache(scene);
            foreach (string candidateName in EnumerateResidentLookupNames(entry.npcId))
            {
                if (string.IsNullOrWhiteSpace(candidateName))
                {
                    continue;
                }

                GameObject resident = FindSceneRoot(candidateName, scene);
                if (resident != null)
                {
                    return resident;
                }
            }

            return null;
        }

        private Transform FindSceneResidentHomeAnchor(SpringDay1NpcCrowdManifest.Entry entry, Scene scene, Transform residentTransform)
        {
            if (entry == null || !scene.IsValid())
            {
                return null;
            }

            EnsureSceneLookupCache(scene);
            foreach (string candidateName in EnumerateResidentLookupNames(entry.npcId))
            {
                foreach (string homeAnchorName in EnumerateAnchorLookupNames(candidateName))
                {
                    if (_sceneTransformLookupCache.TryGetValue(homeAnchorName, out Transform cachedAnchor)
                        && IsValidResidentHomeAnchorCandidate(cachedAnchor, scene, residentTransform))
                    {
                        return cachedAnchor;
                    }

                    Transform anchor = FindAnchor(homeAnchorName);
                    if (IsValidResidentHomeAnchorCandidate(anchor, scene, residentTransform))
                    {
                        return anchor;
                    }
                }
            }

            if (residentTransform != null)
            {
                Transform nested = FindChildRecursive(residentTransform, "HomeAnchor");
                if (nested != null)
                {
                    return nested;
                }
            }

            foreach (string candidateName in EnumerateAnchorNames(entry.anchorObjectName))
            {
                if (!IsHomeAnchorSemanticName(candidateName))
                {
                    continue;
                }

                if (_sceneTransformLookupCache.TryGetValue(candidateName, out Transform cachedAnchor)
                    && IsValidResidentHomeAnchorCandidate(cachedAnchor, scene, residentTransform))
                {
                    return cachedAnchor;
                }

                Transform anchor = FindAnchor(candidateName);
                if (IsValidResidentHomeAnchorCandidate(anchor, scene, residentTransform))
                {
                    return anchor;
                }
            }

            return null;
        }

        private static bool IsValidResidentHomeAnchorCandidate(Transform candidate, Scene scene, Transform residentTransform)
        {
            if (candidate == null || candidate.gameObject.scene != scene)
            {
                return false;
            }

            return (residentTransform == null || !ReferenceEquals(candidate, residentTransform))
                && IsHomeAnchorSemanticName(candidate.name);
        }

        private static bool IsHomeAnchorSemanticName(string candidateName)
        {
            return !string.IsNullOrWhiteSpace(candidateName)
                && candidateName.IndexOf("HomeAnchor", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static IEnumerable<string> EnumerateResidentLookupNames(string npcId)
        {
            if (string.IsNullOrWhiteSpace(npcId))
            {
                yield break;
            }

            string trimmed = npcId.Trim();
            yield return trimmed;

            if (int.TryParse(trimmed, out int numericId))
            {
                yield return numericId.ToString("000");
            }
        }

        private static void AppendStateSummary(
            StringBuilder builder,
            string npcId,
            SpawnState state,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption,
            bool activeOnly)
        {
            builder.Append(npcId);
            builder.Append(':');
            builder.Append(state.Instance != null && state.Instance.activeSelf ? "on" : "off");
            builder.Append('@');
            builder.Append(string.IsNullOrWhiteSpace(state.ResolvedAnchorName) ? "unknown-anchor" : state.ResolvedAnchorName);
            builder.Append("@fallback=");
            builder.Append(state.UsedFallback ? '1' : '0');
            builder.Append("@group=");
            builder.Append(string.IsNullOrWhiteSpace(state.ResidentGroupKey) ? "none" : state.ResidentGroupKey);
            builder.Append("@owned=");
            builder.Append(state.OwnsInstance ? "runtime" : "scene");
            builder.Append("@home=");
            builder.Append(state.HomeAnchor == null ? "none" : state.OwnsHomeAnchor ? "runtime" : "scene");
            builder.Append("@cue=");
            builder.Append(string.IsNullOrWhiteSpace(state.AppliedCueKey) ? "none" : state.AppliedCueKey);
            builder.Append("@return=");
            builder.Append(state.IsReturningHome ? "home" : "none");
            builder.Append("@role=");
            builder.Append(ResolveBeatConsumptionRole(beatConsumption, npcId));
            builder.Append("@presence=");
            builder.Append(TryResolveBeatPresence(beatConsumption, npcId, out SpringDay1CrowdResidentPresenceLevel presenceLevel)
                ? presenceLevel.ToString()
                : SpringDay1CrowdResidentPresenceLevel.None.ToString());

            if (state.Instance != null)
            {
                Vector3 position = state.Instance.transform.position;
                builder.Append("@pos=");
                builder.Append(FormatPosition(position));
            }
            else if (!activeOnly)
            {
                builder.Append("@pos=destroyed");
            }
        }

        private static string FormatPosition(Vector3 position)
        {
            return $"{position.x:F2}/{position.y:F2}";
        }
    }
}
