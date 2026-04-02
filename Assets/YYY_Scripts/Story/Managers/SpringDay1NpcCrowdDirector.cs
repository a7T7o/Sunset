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
        }

        private const string PrimarySceneName = "Primary";
        private const string RuntimeRootName = "SpringDay1NpcCrowdRuntimeRoot";
        private const string ManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";

        private static SpringDay1NpcCrowdDirector _instance;

        private readonly Dictionary<string, SpawnState> _spawnStates = new Dictionary<string, SpawnState>(StringComparer.OrdinalIgnoreCase);

        private SpringDay1NpcCrowdManifest _manifest;
        private GameObject _sceneRoot;
        private float _nextSyncAt;
        private bool _syncRequested = true;

        public static string CurrentRuntimeSummary => _instance != null ? _instance.BuildRuntimeSummary() : "not-created";

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
            if (!IsPrimarySceneActive())
            {
                TeardownAll();
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

        private void HandleActiveSceneChanged(Scene _, Scene nextScene)
        {
            _syncRequested = true;
            if (nextScene.name != PrimarySceneName)
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
                if (!entry.CanAppear(currentPhase))
                {
                    SetEntryActive(entry.npcId, false);
                    continue;
                }

                SpawnState state = GetOrCreateState(entry);
                if (state?.Instance != null && !state.Instance.activeSelf)
                {
                    state.Instance.SetActive(true);
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

            StringBuilder builder = new StringBuilder();
            builder.Append("scene=");
            builder.Append(activeScene.IsValid() ? activeScene.name : "invalid");
            builder.Append("|phase=");
            builder.Append(ResolveCurrentPhase());
            builder.Append("|manifest=");
            builder.Append(entries.Length);
            builder.Append("|spawned=");
            builder.Append(_spawnStates.Count);

            if (_spawnStates.Count > 0)
            {
                builder.Append("|states=");
                bool appendedAny = false;
                foreach (KeyValuePair<string, SpawnState> pair in _spawnStates)
                {
                    if (appendedAny)
                    {
                        builder.Append(',');
                    }

                    appendedAny = true;
                    builder.Append(pair.Key);
                    builder.Append(':');
                    builder.Append(pair.Value.Instance != null && pair.Value.Instance.activeSelf ? "on" : "off");
                }
            }

            return builder.ToString();
        }

        private SpawnState GetOrCreateState(SpringDay1NpcCrowdManifest.Entry entry)
        {
            if (_spawnStates.TryGetValue(entry.npcId, out SpawnState existing) && existing.Instance != null)
            {
                return existing;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != PrimarySceneName)
            {
                return null;
            }

            Vector3 spawnPosition = ResolveSpawnPosition(entry);
            GameObject instance = Instantiate(entry.prefab, spawnPosition, Quaternion.identity);
            instance.name = entry.npcId;
            SceneManager.MoveGameObjectToScene(instance, activeScene);
            if (_sceneRoot != null)
            {
                instance.transform.SetParent(_sceneRoot.transform, true);
            }

            GameObject homeAnchor = new GameObject($"{entry.npcId}_RuntimeHomeAnchor");
            homeAnchor.transform.position = spawnPosition;
            SceneManager.MoveGameObjectToScene(homeAnchor, activeScene);
            if (_sceneRoot != null)
            {
                homeAnchor.transform.SetParent(_sceneRoot.transform, true);
            }

            ConfigureSpawnedNpc(entry, instance, homeAnchor.transform);

            SpawnState state = new SpawnState
            {
                Instance = instance,
                HomeAnchor = homeAnchor
            };
            _spawnStates[entry.npcId] = state;
            return state;
        }

        private void ConfigureSpawnedNpc(SpringDay1NpcCrowdManifest.Entry entry, GameObject instance, Transform homeAnchor)
        {
            NPCAutoRoamController roamController = instance.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                roamController.SetHomeAnchor(homeAnchor);
                roamController.SyncHomeAnchorToCurrentPosition();
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
                motionController.SetFacingDirection(entry.initialFacing);
            }
        }

        private Vector3 ResolveSpawnPosition(SpringDay1NpcCrowdManifest.Entry entry)
        {
            Transform anchor = FindAnchor(entry.anchorObjectName);
            Vector3 basePosition = anchor != null
                ? anchor.position
                : new Vector3(entry.fallbackWorldPosition.x, entry.fallbackWorldPosition.y, 0f);

            return basePosition + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
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

        private static StoryPhase ResolveCurrentPhase()
        {
            StoryManager storyManager = StoryManager.Instance;
            return storyManager != null ? storyManager.CurrentPhase : StoryPhase.CrashAndMeet;
        }

        private void EnsureSceneRoot(Scene scene)
        {
            if (_sceneRoot != null && _sceneRoot.scene.IsValid() && _sceneRoot.scene == scene)
            {
                return;
            }

            if (_sceneRoot != null)
            {
                Destroy(_sceneRoot);
            }

            _sceneRoot = new GameObject(RuntimeRootName);
            SceneManager.MoveGameObjectToScene(_sceneRoot, scene);
        }

        private void PruneDestroyedStates()
        {
            List<string> keys = new List<string>(_spawnStates.Keys);
            for (int index = 0; index < keys.Count; index++)
            {
                string npcId = keys[index];
                SpawnState state = _spawnStates[npcId];
                if (state.Instance == null || state.HomeAnchor == null)
                {
                    if (state.Instance != null)
                    {
                        Destroy(state.Instance);
                    }

                    if (state.HomeAnchor != null)
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

            if (_sceneRoot != null)
            {
                Destroy(_sceneRoot);
                _sceneRoot = null;
            }
        }

        private void DestroyState(string npcId)
        {
            if (!_spawnStates.TryGetValue(npcId, out SpawnState state))
            {
                return;
            }

            if (state.Instance != null)
            {
                Destroy(state.Instance);
            }

            if (state.HomeAnchor != null)
            {
                Destroy(state.HomeAnchor);
            }

            _spawnStates.Remove(npcId);
        }

        private static bool IsPrimarySceneActive()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return activeScene.IsValid() && activeScene.name == PrimarySceneName;
        }
    }
}
