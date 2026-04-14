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
        private const string ResidentRootName = "Town_Day1Residents";
        private const string ResidentDefaultPresentRootName = "Resident_DefaultPresent";
        private const string ResidentTakeoverReadyRootName = "Resident_DirectorTakeoverReady";
        private const string ResidentBackstageRootName = "Resident_BackstagePresent";
        private const string CarrierRootName = "Town_Day1Carriers";
        private const string StoryEscortChiefNpcId = "001";
        private const string StoryEscortCompanionNpcId = "002";
        private const string VillageCrowdMarkerStartSuffixCn = "起点";
        private const string VillageCrowdMarkerEndSuffixCn = "终点";
        private const string ManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";
        private const float ResidentReturnHomeThreshold = 0.05f;
        private const int ResidentReturnHomeHour = 20;
        private const int ResidentForcedRestHour = 21;
        private const int ResidentMorningReleaseHour = 9;
        private const string RestoredDirectorCuePlaceholderKey = "__restored-director-cue__";
        private const string ResidentScriptedControlOwnerKey = nameof(SpringDay1NpcCrowdDirector);
        private static readonly string[] VillageCrowdMarkerRootNames = { "进村围观", "VillageCrowd", "EnterVillageCrowdRoot" };
        private static readonly string[] VillageCrowdMarkerStartGroupNames = { "起点", "Start" };
        private static readonly string[] VillageCrowdMarkerEndGroupNames = { "终点", "End" };

        private static SpringDay1NpcCrowdDirector _instance;

        private readonly Dictionary<string, SpawnState> _spawnStates = new Dictionary<string, SpawnState>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ResidentRuntimeSnapshotEntry> _residentRuntimeSnapshotCache = new Dictionary<string, ResidentRuntimeSnapshotEntry>(StringComparer.OrdinalIgnoreCase);

        private SpringDay1NpcCrowdManifest _manifest;
        private GameObject _sceneRoot;
        private GameObject _residentRoot;
        private GameObject _residentDefaultRoot;
        private GameObject _residentTakeoverReadyRoot;
        private GameObject _residentBackstageRoot;
        private GameObject _carrierRoot;
        private float _nextSyncAt;
        private bool _syncRequested = true;

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

            Scene activeScene = SceneManager.GetActiveScene();
            if (ShouldRecoverMissingTownResidents(activeScene))
            {
                _nextSyncAt = Time.unscaledTime + 0.1f;
                _syncRequested = false;
                SyncCrowd();
                return;
            }

            SyncResidentNightRestSchedule();
            TickResidentReturns();

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
                || !string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal)
                || _spawnStates.Count > 0)
            {
                return false;
            }

            if (StoryManager.Instance == null || SpringDay1Director.Instance == null || LoadManifest() == null)
            {
                return false;
            }

            EnsureSceneRoot(activeScene);
            if (_residentRoot != null || _residentDefaultRoot != null || _residentTakeoverReadyRoot != null || _residentBackstageRoot != null)
            {
                return true;
            }

            SpringDay1NpcCrowdManifest.Entry[] entries = _manifest != null ? _manifest.Entries : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry != null && FindSceneResident(entry, activeScene) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
        {
            CaptureSceneResidentRuntimeSnapshotCache(previousScene);
            _syncRequested = true;
            if (!IsSupportedRuntimeScene(nextScene))
            {
                TeardownAll();
            }
        }

        private void HandleSceneLoaded(Scene _, LoadSceneMode __)
        {
            _syncRequested = true;
        }

        private void HandleStoryPhaseChanged(StoryPhaseChangedEvent _)
        {
            _syncRequested = true;
        }

        private void SyncCrowd()
        {
            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            if (manifest == null)
            {
                TeardownAll();
                return;
            }

            EnsureSceneRoot(SceneManager.GetActiveScene());
            PruneDestroyedStates();

            StoryPhase currentPhase = ResolveCurrentPhase();
            string currentBeatKey = ResolveCurrentBeatKey();
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption = manifest.BuildBeatConsumptionSnapshot(currentBeatKey);
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries;
            HashSet<string> liveIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || string.IsNullOrWhiteSpace(entry.npcId) || entry.prefab == null)
                {
                    continue;
                }

                liveIds.Add(entry.npcId);
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

            List<string> staleIds = new List<string>(_spawnStates.Keys);
            for (int index = 0; index < staleIds.Count; index++)
            {
                string npcId = staleIds[index];
                if (!liveIds.Contains(npcId))
                {
                    DestroyState(npcId);
                }
            }
        }

        private string BuildRuntimeSummary()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SpringDay1NpcCrowdManifest manifest = LoadManifest();
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            string currentBeatKey = ResolveCurrentBeatKey();
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption = manifest != null
                ? manifest.BuildBeatConsumptionSnapshot(currentBeatKey)
                : default;

            StringBuilder builder = new StringBuilder();
            builder.Append("scene=");
            builder.Append(activeScene.IsValid() ? activeScene.name : "invalid");
            builder.Append("|phase=");
            builder.Append(ResolveCurrentPhase());
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

            EnsureSceneRoot(activeScene);
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

            Transform residentParent = ResolveResidentParentByGroupKey(snapshot.residentGroupKey);
            if (residentParent != null)
            {
                ReparentState(state, residentParent, residentParent.name);
            }
            else if (!string.IsNullOrWhiteSpace(snapshot.residentGroupKey))
            {
                state.ResidentGroupKey = snapshot.residentGroupKey.Trim();
            }

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
            state.IsReturningHome = restoreReturnHome;
            state.ResumeRoamAfterReturn = restoreReturnHome;
            if (restoreReturnHome || snapshot.underDirectorCue)
            {
                AcquireResidentDirectorControl(state, resumeRoamWhenReleased: restoreReturnHome);
            }
            else
            {
                ReleaseResidentDirectorControl(state, resumeResidentLogic: snapshot.isActive);
            }

            if (snapshot.underDirectorCue)
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
            if (state == null || !state.IsReturningHome)
            {
                return false;
            }

            if (state.IsNightResting)
            {
                return true;
            }

            if (ShouldResidentsRestByClock() || ShouldResidentsReturnHomeByClock())
            {
                return true;
            }

            return ResolveCurrentPhase() >= StoryPhase.FreeTime;
        }

        private bool ShouldRestoreResidentReturnHomeSnapshot(ResidentRuntimeSnapshotEntry snapshot)
        {
            if (snapshot == null || !snapshot.isReturningHome)
            {
                return false;
            }

            if (ShouldResidentsRestByClock() || ShouldResidentsReturnHomeByClock())
            {
                return true;
            }

            return ResolveCurrentPhase() >= StoryPhase.FreeTime;
        }

        private bool TryGetResidentRuntimeSnapshot(string sceneName, string npcId, out ResidentRuntimeSnapshotEntry snapshot)
        {
            return _residentRuntimeSnapshotCache.TryGetValue(
                BuildResidentRuntimeSnapshotCacheKey(sceneName, npcId),
                out snapshot);
        }

        private Transform ResolveResidentParentByGroupKey(string residentGroupKey)
        {
            string normalizedGroupKey = NormalizeSnapshotValue(residentGroupKey);
            return normalizedGroupKey switch
            {
                ResidentRootName => _residentRoot != null ? _residentRoot.transform : null,
                ResidentDefaultPresentRootName => _residentDefaultRoot != null ? _residentDefaultRoot.transform : null,
                ResidentTakeoverReadyRootName => _residentTakeoverReadyRoot != null ? _residentTakeoverReadyRoot.transform : null,
                ResidentBackstageRootName => _residentBackstageRoot != null ? _residentBackstageRoot.transform : null,
                CarrierRootName => _carrierRoot != null ? _carrierRoot.transform : null,
                RuntimeRootName => _sceneRoot != null ? _sceneRoot.transform : null,
                _ => null
            };
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
            Transform initialResidentParent = ResolveResidentParent(entry, ResolveCurrentPhase(), beatKey, beatConsumption);
            ReparentState(state, initialResidentParent, initialResidentParent != null ? initialResidentParent.name : string.Empty);
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
                    roamController.SetHomeAnchor(homeAnchor);
                }

                roamController.ApplyProfile();
            }

            if (instance.GetComponent<NPCDialogueInteractable>() == null
                && instance.GetComponent<NPCInformalChatInteractable>() == null
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

        private static Transform FindSemanticAnchor(SpringDay1NpcCrowdManifest.Entry entry, string beatKey)
        {
            if (entry == null)
            {
                return null;
            }

            string preferredSemanticAnchor = ResolvePreferredSemanticAnchor(entry, beatKey);
            if (!string.IsNullOrWhiteSpace(preferredSemanticAnchor))
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
                Transform resolved = FindAnchor(entry.semanticAnchorIds[index]);
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

                if (SpringDay1TownAnchorContractDatabase.TryResolveAnchor(semanticAnchorId, out worldPosition))
                {
                    anchorName = semanticAnchorId.Trim();
                    return true;
                }
            }

            return false;
        }

        private static Transform FindAnchor(string anchorObjectName)
        {
            if (string.IsNullOrWhiteSpace(anchorObjectName))
            {
                return null;
            }

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
                    GameObject found = GameObject.Find(lookupName);
                    if (found != null)
                    {
                        return found.transform;
                    }
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

            foreach (string aliasName in EnumerateSemanticAnchorAliasNames(trimmed))
            {
                yield return aliasName;
            }

            yield return trimmed;
        }

        private static IEnumerable<string> EnumerateSemanticAnchorAliasNames(string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                yield break;
            }

            string trimmed = candidateName.Trim();
            yield return $"DirectorReady_{trimmed}_HomeAnchor";
            yield return $"Resident_{trimmed}_HomeAnchor";
            yield return $"Backstage_{trimmed}_HomeAnchor";
            yield return $"ResidentSlot_{trimmed}_HomeAnchor";
            yield return $"BackstageSlot_{trimmed}_HomeAnchor";
            yield return $"DirectorReady_{trimmed}";
            yield return $"ResidentSlot_{trimmed}";
            yield return $"BackstageSlot_{trimmed}";
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
                SpringDay1DirectorActorCue runtimeCue = ResolveRuntimeCueOverride(beatKey, entry, cue);
                CancelResidentReturnHome(state, resumeRoam: false);
                ReparentState(state, _carrierRoot != null ? _carrierRoot.transform : _sceneRoot.transform, CarrierRootName);
                SetEntryActive(entry.npcId, true);
                SpringDay1DirectorStagingPlayback playback = GetOrAddComponent<SpringDay1DirectorStagingPlayback>(state.Instance);
                string cueKey = runtimeCue != null ? runtimeCue.StableKey : cue.StableKey;
                bool needsApply = !string.Equals(state.AppliedBeatKey, beatKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(state.AppliedCueKey, cueKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(playback.CurrentBeatKey, beatKey, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(playback.CurrentCueKey, cueKey, StringComparison.OrdinalIgnoreCase);
                if (needsApply)
                {
                    if (ShouldRestartCueFromSceneBaseline(beatKey, state))
                    {
                        ResetStateToBasePose(state);
                    }

                    playback.ApplyCue(beatKey, runtimeCue ?? cue, state.HomeAnchor != null ? state.HomeAnchor.transform : null);
                    state.AppliedBeatKey = beatKey;
                    state.AppliedCueKey = cueKey;
                    state.ResolvedAnchorName = $"staging:{cueKey}";
                    state.UsedFallback = false;
                    state.ReleasedFromDirectorCue = false;
                    state.NeedsResidentReset = false;
                }

                return true;
            }

            if (string.IsNullOrWhiteSpace(state.AppliedCueKey))
            {
                return false;
            }

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
            if (state.HomeAnchor != null)
            {
                roamController.SetHomeAnchor(state.HomeAnchor.transform);
            }

            roamController.ApplyProfile();
            if (!state.IsNightResting && !state.IsReturningHome)
            {
                roamController.RefreshRoamCenterFromCurrentContext();
            }

            return roamController;
        }

        private static NPCAutoRoamController AcquireResidentDirectorControl(SpawnState state, bool resumeRoamWhenReleased)
        {
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            roamController?.AcquireResidentScriptedControl(ResidentScriptedControlOwnerKey, resumeRoamWhenReleased);
            return roamController;
        }

        private static NPCAutoRoamController ReleaseResidentDirectorControl(SpawnState state, bool resumeResidentLogic)
        {
            NPCAutoRoamController roamController = PrepareResidentRoamController(state);
            roamController?.ReleaseResidentScriptedControl(ResidentScriptedControlOwnerKey, resumeResidentLogic);
            return roamController;
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

            motionController.SetFacingDirection(facing);
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

            Scene activeScene = SceneManager.GetActiveScene();
            bool isTownScene = activeScene.IsValid() && string.Equals(activeScene.name, TownSceneName, StringComparison.Ordinal);
            bool shouldMirrorVillageCrowd = isTownScene
                && (string.Equals(beatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(beatKey, SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase));

            SpringDay1DirectorStageBook book = SpringDay1DirectorStagingDatabase.Load();
            if (shouldMirrorVillageCrowd)
            {
                cue = book != null ? book.TryResolveCue(SpringDay1DirectorBeatKeys.EnterVillagePostEntry, entry) : null;
                if (cue != null)
                {
                    return true;
                }
            }

            cue = book != null ? book.TryResolveCue(beatKey, entry) : null;
            return cue != null;
        }

        private static bool ShouldSuppressEnterVillageCrowdCueForTownHouseLead(string beatKey)
        {
            if (!string.Equals(beatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance;
            return director != null && director.ShouldReleaseEnterVillageCrowd();
        }

        private static SpringDay1DirectorActorCue ResolveRuntimeCueOverride(
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

            resolvedCue.EnsureDefaults();
            return resolvedCue;
        }

        private static bool ShouldUseVillageCrowdSceneResidentStart(string beatKey)
        {
            return string.Equals(beatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase)
                || string.Equals(beatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                || string.Equals(beatKey, SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ShouldUseVillageCrowdMarkerCueOverride(string beatKey)
        {
            return string.Equals(beatKey, SpringDay1DirectorBeatKeys.EnterVillagePostEntry, StringComparison.OrdinalIgnoreCase)
                || string.Equals(beatKey, SpringDay1DirectorBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                || string.Equals(beatKey, SpringDay1DirectorBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryResolveVillageCrowdMarkers(string npcId, out Transform startMarker, out Transform endMarker)
        {
            startMarker = null;
            endMarker = null;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            Transform root = FindFirstExistingAnchor(VillageCrowdMarkerRootNames);
            if (root == null)
            {
                return false;
            }

            Transform startGroup = FindFirstExistingChild(root, VillageCrowdMarkerStartGroupNames);
            Transform endGroup = FindFirstExistingChild(root, VillageCrowdMarkerEndGroupNames);
            startMarker = FindVillageCrowdMarker(startGroup, npcId, start: true);
            endMarker = FindVillageCrowdMarker(endGroup, npcId, start: false);
            return startMarker != null || endMarker != null;
        }

        private static Transform FindFirstExistingAnchor(string[] names)
        {
            if (names == null)
            {
                return null;
            }

            for (int index = 0; index < names.Length; index++)
            {
                Transform candidate = FindAnchor(names[index]);
                if (candidate != null)
                {
                    return candidate;
                }
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
            Transform residentParent = ResolveResidentParent(entry, currentPhase, beatKey, beatConsumption);
            if (state.IsNightResting && _residentDefaultRoot != null)
            {
                residentParent = _residentDefaultRoot.transform;
            }

            ReparentState(state, residentParent, residentParent != null ? residentParent.name : string.Empty);
            if (state.IsNightResting)
            {
                shouldRemainActive = true;
            }

            SetEntryActive(entry.npcId, shouldRemainActive);

            if (!shouldRemainActive)
            {
                CancelResidentReturnHome(state, resumeRoam: false);
                state.ReleasedFromDirectorCue = false;
                state.NeedsResidentReset = false;
                state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: true);
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.IsNightResting)
            {
                ApplyResidentNightRestState(state);
                state.ResolvedAnchorName = $"night-rest:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.IsReturningHome)
            {
                if (!ShouldAllowResidentReturnHome(currentPhase))
                {
                    CancelResidentReturnHome(state, resumeRoam: false);
                    if (TryReleaseResidentToDaytimeBaseline(entry, state, currentPhase, beatKey))
                    {
                        state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: false);
                        state.UsedFallback = state.BaseUsedFallback;
                        return;
                    }
                }

                state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: false) + ":return-home";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (state.ReleasedFromDirectorCue && state.NeedsResidentReset)
            {
                if (ShouldAllowResidentReturnHome(currentPhase))
                {
                    if (TryBeginResidentReturnHome(state))
                    {
                        state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: false) + ":return-home";
                        state.UsedFallback = state.BaseUsedFallback;
                        return;
                    }
                }
                else if (TryReleaseResidentToDaytimeBaseline(entry, state, currentPhase, beatKey))
                {
                    state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: false);
                    state.UsedFallback = state.BaseUsedFallback;
                    return;
                }
            }

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                roamController = ReleaseResidentDirectorControl(state, resumeResidentLogic: true);
                if (roamController != null && !roamController.IsRoaming)
                {
                    roamController.StartRoam();
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

            state.ResolvedAnchorName = BuildResidentSummaryAnchor(entry, currentPhase, beatKey, residentParent, hidden: false);
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

            return currentPhase == StoryPhase.EnterVillage
                || currentPhase == StoryPhase.HealingAndHP
                || currentPhase == StoryPhase.WorkbenchFlashback
                || currentPhase == StoryPhase.FarmingTutorial
                || currentPhase == StoryPhase.DinnerConflict
                || currentPhase == StoryPhase.ReturnAndReminder;
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

                NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
                if (roamController != null)
                {
                    AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false);
                }

                Vector3 targetPosition = state.HomeAnchor.transform.position;
                targetPosition.z = state.Instance.transform.position.z;
                state.Instance.transform.position = targetPosition;

                NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
                if (motionController != null)
                {
                    motionController.StopMotion();
                    ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
                }

                state.IsNightResting = true;
                state.IsReturningHome = false;
                state.ResumeRoamAfterReturn = false;
                state.NeedsResidentReset = false;
                state.ReleasedFromDirectorCue = false;
                state.ResolvedAnchorName = state.BaseResolvedAnchorName;
                state.UsedFallback = state.BaseUsedFallback;
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
            Vector3 delta = targetPosition - currentPosition;
            delta.z = 0f;
            float maxStep = GetResidentReturnHomeStepDistance(state, deltaTime);
            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();

            if (delta.sqrMagnitude <= ResidentReturnHomeThreshold * ResidentReturnHomeThreshold
                || delta.sqrMagnitude <= maxStep * maxStep)
            {
                state.Instance.transform.position = targetPosition;
                FinishResidentReturnHome(state);
                return;
            }

            if (TryDriveResidentReturnHome(state, roamController))
            {
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return;
            }

            if (roamController != null)
            {
                roamController.HaltResidentScriptedMovement();
            }

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            Vector3 step = delta.normalized * maxStep;
            state.Instance.transform.position = currentPosition + step;
            if (motionController != null)
            {
                motionController.SetFacingDirection(new Vector2(step.x, step.y));
            }

            state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
        }

        private bool TryBeginResidentReturnHome(SpawnState state)
        {
            if (state == null || state.Instance == null || state.HomeAnchor == null || state.IsReturningHome)
            {
                return false;
            }

            Vector3 currentPosition = state.Instance.transform.position;
            Vector3 homePosition = state.HomeAnchor.transform.position;
            if ((homePosition - currentPosition).sqrMagnitude <= ResidentReturnHomeThreshold * ResidentReturnHomeThreshold)
            {
                return false;
            }

            if (TryDriveResidentReturnHome(state))
            {
                state.IsReturningHome = true;
                state.ResumeRoamAfterReturn = !state.IsNightResting;
                state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
                state.UsedFallback = state.BaseUsedFallback;
                return true;
            }

            state.IsReturningHome = true;
            state.ResumeRoamAfterReturn = !state.IsNightResting;
            state.ResolvedAnchorName = $"return-home:{state.BaseResolvedAnchorName}";
            state.UsedFallback = state.BaseUsedFallback;
            return true;
        }

        private static float GetResidentReturnHomeStepDistance(SpawnState state, float deltaTime)
        {
            NPCMotionController motionController = state != null && state.Instance != null
                ? state.Instance.GetComponent<NPCMotionController>()
                : null;
            float moveSpeed = motionController != null ? Mathf.Max(0.5f, motionController.MoveSpeed) : 1.5f;
            return moveSpeed * Mathf.Max(0.0001f, deltaTime);
        }

        private bool ShouldAllowResidentReturnHome(StoryPhase currentPhase)
        {
            return ShouldResidentsRestByClock()
                   || ShouldResidentsReturnHomeByClock();
        }

        private static bool TryReleaseResidentToDaytimeBaseline(
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

            bool hasBasePosition = state.BasePosition.sqrMagnitude > 0.0001f;
            Vector3 releasePosition = hasBasePosition ? state.BasePosition : state.Instance.transform.position;
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
                else if (state.HomeAnchor != null
                         && roamController.TryResolveOccupiablePosition((Vector2)state.HomeAnchor.transform.position, out Vector2 occupiableHomePosition))
                {
                    releasePosition = new Vector3(occupiableHomePosition.x, occupiableHomePosition.y, state.Instance.transform.position.z);
                    shouldPromoteBasePose = false;
                    promotedAnchorName = state.BaseResolvedAnchorName;
                }
                else if (hasBasePosition
                         && roamController.TryResolveOccupiablePosition((Vector2)state.BasePosition, out Vector2 occupiableBasePosition))
                {
                    releasePosition = new Vector3(occupiableBasePosition.x, occupiableBasePosition.y, state.Instance.transform.position.z);
                    shouldPromoteBasePose = false;
                    promotedAnchorName = state.BaseResolvedAnchorName;
                }
                else if (roamController.TryResolveOccupiablePosition((Vector2)state.Instance.transform.position, out Vector2 occupiableCurrentPosition))
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
            roamController = ReleaseResidentDirectorControl(state, resumeResidentLogic: true);
            if (roamController != null)
            {
                roamController.RestartRoamFromCurrentContext(tryImmediateMove: true);
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
            return true;
        }

        private static bool ShouldUseResidentDaytimeSemanticBaseline(StoryPhase currentPhase, string beatKey)
        {
            return currentPhase < StoryPhase.FreeTime
                && !string.IsNullOrWhiteSpace(beatKey)
                && beatKey.IndexOf("DailyStand", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryResolveResidentDaytimeBaselinePosition(
            SpringDay1NpcCrowdManifest.Entry entry,
            out Vector3 worldPosition,
            out string anchorName)
        {
            worldPosition = Vector3.zero;
            anchorName = string.Empty;
            if (entry?.semanticAnchorIds == null)
            {
                return false;
            }

            for (int index = 0; index < entry.semanticAnchorIds.Length; index++)
            {
                string candidate = entry.semanticAnchorIds[index];
                if (!IsResidentDaytimeBaselineSemanticAnchor(candidate))
                {
                    continue;
                }

                if (!SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(candidate, out worldPosition))
                {
                    continue;
                }

                anchorName = candidate.Trim();
                return true;
            }

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

            return roamController.DriveResidentScriptedMoveTo(
                ResidentScriptedControlOwnerKey,
                state.HomeAnchor.transform.position,
                resumeRoamWhenReleased: !state.IsNightResting,
                retargetTolerance: Mathf.Max(ResidentReturnHomeThreshold * 4f, 0.35f));
        }

        private void FinishResidentReturnHome(SpawnState state)
        {
            if (state == null || state.Instance == null)
            {
                return;
            }

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
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
                NPCAutoRoamController roamController = ReleaseResidentDirectorControl(state, resumeResidentLogic: true);
                if (roamController != null)
                {
                    if (!roamController.IsRoaming)
                    {
                        roamController.StartRoam();
                    }
                }
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = state.BaseResolvedAnchorName;
            state.UsedFallback = state.BaseUsedFallback;
        }

        private static void CancelResidentReturnHome(SpawnState state, bool resumeRoam)
        {
            if (state == null)
            {
                return;
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;

            if (!resumeRoam || state.Instance == null || state.IsNightResting)
            {
                return;
            }

            NPCAutoRoamController roamController = ReleaseResidentDirectorControl(state, resumeResidentLogic: true);
            if (roamController != null)
            {
                if (!roamController.IsRoaming)
                {
                    roamController.StartRoam();
                }
            }
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
                if (state == null || state.Instance == null || state.HomeAnchor == null)
                {
                    continue;
                }

                if (shouldRest)
                {
                    if (state.IsNightResting)
                    {
                        continue;
                    }

                    state.IsNightResting = true;
                    ApplyResidentNightRestState(state);
                    _syncRequested = true;
                    continue;
                }

                if (state.IsNightResting)
                {
                    state.IsNightResting = false;
                    _syncRequested = true;
                }

                if (shouldReturnHome)
                {
                    if (state.IsReturningHome)
                    {
                        continue;
                    }

                    if (TryBeginResidentReturnHome(state))
                    {
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

        private static void ApplyResidentNightRestState(SpawnState state)
        {
            if (state == null || state.Instance == null || state.HomeAnchor == null)
            {
                return;
            }

            state.Instance.SetActive(true);

            NPCAutoRoamController roamController = state.Instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                AcquireResidentDirectorControl(state, resumeRoamWhenReleased: false);
            }

            Vector3 targetPosition = state.HomeAnchor.transform.position;
            targetPosition.z = state.Instance.transform.position.z;
            state.Instance.transform.position = targetPosition;

            NPCMotionController motionController = state.Instance.GetComponent<NPCMotionController>();
            if (motionController != null)
            {
                motionController.StopMotion();
                ApplyFacingIfIdle(state.Instance, motionController, state.BaseFacing);
            }

            state.IsReturningHome = false;
            state.ResumeRoamAfterReturn = false;
            state.NeedsResidentReset = false;
            state.ReleasedFromDirectorCue = false;
            state.ResolvedAnchorName = $"night-rest:{state.BaseResolvedAnchorName}";
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

        private Transform ResolveResidentParent(
            SpringDay1NpcCrowdManifest.Entry entry,
            StoryPhase currentPhase,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (ShouldReleaseResidentsToFreeTimeBaseline(currentPhase)
                && ResolveBeatConsumptionRole(beatConsumption, entry.npcId) != SpringDay1CrowdDirectorConsumptionRole.Priority)
            {
                return _residentDefaultRoot != null
                    ? _residentDefaultRoot.transform
                    : _sceneRoot != null ? _sceneRoot.transform : null;
            }

            if (ShouldUseBackstageParent(entry, currentPhase, beatKey, beatConsumption))
            {
                return _residentBackstageRoot != null
                    ? _residentBackstageRoot.transform
                    : _sceneRoot != null ? _sceneRoot.transform : null;
            }

            if (ShouldUseTakeoverReadyParent(entry, beatKey, beatConsumption))
            {
                return _residentTakeoverReadyRoot != null
                    ? _residentTakeoverReadyRoot.transform
                    : _sceneRoot != null ? _sceneRoot.transform : null;
            }

            return _residentDefaultRoot != null
                ? _residentDefaultRoot.transform
                : _sceneRoot != null ? _sceneRoot.transform : null;
        }

        private bool ShouldReleaseResidentsToFreeTimeBaseline(StoryPhase currentPhase)
        {
            return currentPhase == StoryPhase.FreeTime
                && !ShouldResidentsReturnHomeByClock()
                && !ShouldResidentsRestByClock();
        }

        private static bool ShouldUseTakeoverReadyParent(
            SpringDay1NpcCrowdManifest.Entry entry,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (entry == null)
            {
                return false;
            }

            SpringDay1CrowdDirectorConsumptionRole role = ResolveBeatConsumptionRole(beatConsumption, entry.npcId);
            return role == SpringDay1CrowdDirectorConsumptionRole.Priority
                || role == SpringDay1CrowdDirectorConsumptionRole.Support
                || entry.residentBaseline == SpringDay1CrowdResidentBaseline.PeripheralResident;
        }

        private static bool ShouldUseBackstageParent(
            SpringDay1NpcCrowdManifest.Entry entry,
            StoryPhase currentPhase,
            string beatKey,
            SpringDay1NpcCrowdManifest.BeatConsumptionSnapshot beatConsumption)
        {
            if (entry == null)
            {
                return false;
            }

            SpringDay1CrowdDirectorConsumptionRole role = ResolveBeatConsumptionRole(beatConsumption, entry.npcId);
            if (role == SpringDay1CrowdDirectorConsumptionRole.Trace
                || role == SpringDay1CrowdDirectorConsumptionRole.BackstagePressure)
            {
                return true;
            }

            if (entry.residentBaseline == SpringDay1CrowdResidentBaseline.NightResident
                && currentPhase < StoryPhase.FreeTime)
            {
                return true;
            }

            return currentPhase < StoryPhase.EnterVillage
                && entry.residentBaseline != SpringDay1CrowdResidentBaseline.DaytimeResident;
        }

        private static string BuildResidentSummaryAnchor(
            SpringDay1NpcCrowdManifest.Entry entry,
            StoryPhase currentPhase,
            string beatKey,
            Transform residentParent,
            bool hidden)
        {
            string beatLabel = string.IsNullOrWhiteSpace(beatKey) ? currentPhase.ToString() : beatKey;
            string rootLabel = residentParent != null ? residentParent.name : "runtime-root";
            return hidden
                ? $"resident-hidden:{rootLabel}:{beatLabel}"
                : $"resident:{rootLabel}:{beatLabel}";
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
            if (_sceneRoot != null && _sceneRoot.scene.IsValid() && _sceneRoot.scene == scene)
            {
                return;
            }

            _sceneRoot = FindSceneRoot(RuntimeRootName, scene);
            _carrierRoot = FindSceneRoot(CarrierRootName, scene);
            _residentRoot = FindSceneRoot(ResidentRootName, scene);
            _residentDefaultRoot = FindSceneRoot(ResidentDefaultPresentRootName, scene);
            _residentTakeoverReadyRoot = FindSceneRoot(ResidentTakeoverReadyRootName, scene);
            _residentBackstageRoot = FindSceneRoot(ResidentBackstageRootName, scene);
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

        private static GameObject FindSceneRoot(string rootName, Scene scene)
        {
            if (!scene.IsValid() || string.IsNullOrWhiteSpace(rootName))
            {
                return null;
            }

            GameObject[] sceneRoots = scene.GetRootGameObjects();
            for (int index = 0; index < sceneRoots.Length; index++)
            {
                GameObject sceneRoot = sceneRoots[index];
                if (sceneRoot == null)
                {
                    continue;
                }

                if (string.Equals(sceneRoot.name, rootName, StringComparison.OrdinalIgnoreCase))
                {
                    return sceneRoot;
                }

                Transform exact = sceneRoot.transform.Find(rootName);
                if (exact != null)
                {
                    return exact.gameObject;
                }

                Transform recursive = FindChildRecursive(sceneRoot.transform, rootName);
                if (recursive != null)
                {
                    return recursive.gameObject;
                }
            }

            return null;
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
            _sceneRoot = null;

            _residentRoot = null;
            _residentDefaultRoot = null;
            _residentTakeoverReadyRoot = null;
            _residentBackstageRoot = null;
            _carrierRoot = null;
        }

        private void DestroyState(string npcId)
        {
            if (!_spawnStates.TryGetValue(npcId, out SpawnState state))
            {
                return;
            }

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

        private static GameObject FindSceneResident(SpringDay1NpcCrowdManifest.Entry entry, Scene scene)
        {
            if (entry == null || !scene.IsValid())
            {
                return null;
            }

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

        private static Transform FindSceneResidentHomeAnchor(SpringDay1NpcCrowdManifest.Entry entry, Scene scene, Transform residentTransform)
        {
            if (entry == null || !scene.IsValid())
            {
                return null;
            }

            foreach (string candidateName in EnumerateResidentLookupNames(entry.npcId))
            {
                foreach (string homeAnchorName in EnumerateAnchorNames(candidateName))
                {
                    Transform anchor = FindAnchor(homeAnchorName);
                    if (anchor != null && anchor.gameObject.scene == scene)
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
                Transform anchor = FindAnchor(candidateName);
                if (anchor != null && anchor.gameObject.scene == scene)
                {
                    return anchor;
                }
            }

            return null;
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
