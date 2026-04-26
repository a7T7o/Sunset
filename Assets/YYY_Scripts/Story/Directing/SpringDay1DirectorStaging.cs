using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sunset.Story
{
    public static class SpringDay1DirectorBeatKeys
    {
        public const string CrashAndMeetAwake = "CrashAndMeet_Awake";
        public const string CrashAndMeetEscape = "CrashAndMeet_Escape";
        public const string EnterVillagePostEntry = "EnterVillage_PostEntry";
        public const string EnterVillageHouseArrival = "EnterVillage_HouseArrival";
        public const string HealingAndHpTreatment = "HealingAndHP_Treatment";
        public const string WorkbenchFlashbackRecall = "WorkbenchFlashback_Recall";
        public const string FarmingTutorialFieldwork = "FarmingTutorial_Fieldwork";
        public const string DinnerConflictTable = "DinnerConflict_Table";
        public const string ReturnAndReminderWalkBack = "ReturnAndReminder_WalkBack";
        public const string FreeTimeNightWitness = "FreeTime_NightWitness";
        public const string DayEndSettle = "DayEnd_Settle";
        public const string DailyStandPreview = "DailyStand_Preview";

        private static readonly string[] KnownKeys =
        {
            CrashAndMeetAwake,
            CrashAndMeetEscape,
            EnterVillagePostEntry,
            EnterVillageHouseArrival,
            HealingAndHpTreatment,
            WorkbenchFlashbackRecall,
            FarmingTutorialFieldwork,
            DinnerConflictTable,
            ReturnAndReminderWalkBack,
            FreeTimeNightWitness,
            DayEndSettle,
            DailyStandPreview
        };

        public static IReadOnlyList<string> OrderedKeys => KnownKeys;

        public static bool IsKnown(string beatKey)
        {
            if (string.IsNullOrWhiteSpace(beatKey))
            {
                return false;
            }

            for (int index = 0; index < KnownKeys.Length; index++)
            {
                if (string.Equals(KnownKeys[index], beatKey, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class SpringDay1DirectorPathPoint
    {
        public Vector2 position = Vector2.zero;
        public Vector2 facing = Vector2.down;
        public float holdSeconds = 0.25f;
        public string lookAtTargetName = string.Empty;

        public void EnsureDefaults()
        {
            if (holdSeconds < 0f)
            {
                holdSeconds = 0f;
            }

            if (facing.sqrMagnitude <= 0.0001f)
            {
                facing = Vector2.down;
            }

            lookAtTargetName ??= string.Empty;
        }
    }

    [Serializable]
    public sealed class SpringDay1DirectorActorCue
    {
        public string cueId = string.Empty;
        public string npcId = string.Empty;
        public string semanticAnchorId = string.Empty;
        public SpringDay1CrowdSceneDuty duty = SpringDay1CrowdSceneDuty.None;
        public bool keepCurrentSpawnPosition = true;
        public bool useSemanticAnchorAsStart = false;
        public bool startPositionIsSemanticAnchorOffset = false;
        public bool pathPointsAreOffsets = false;
        public Vector2 startPosition = Vector2.zero;
        public Vector2 facing = Vector2.down;
        public string lookAtTargetName = string.Empty;
        public bool suspendRoam = true;
        public bool loopPath = false;
        public float moveSpeed = 1.4f;
        public float initialHoldSeconds = 0.2f;
        public SpringDay1DirectorPathPoint[] path = Array.Empty<SpringDay1DirectorPathPoint>();

        public string StableKey
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(cueId))
                {
                    return cueId.Trim();
                }

                if (!string.IsNullOrWhiteSpace(npcId))
                {
                    return npcId.Trim();
                }

                if (!string.IsNullOrWhiteSpace(semanticAnchorId))
                {
                    return semanticAnchorId.Trim();
                }

                return duty.ToString();
            }
        }

        public bool Matches(SpringDay1NpcCrowdManifest.Entry entry)
        {
            return MatchesNpcId(entry)
                || MatchesSemanticAnchor(entry)
                || MatchesDuty(entry);
        }

        public bool MatchesNpcId(SpringDay1NpcCrowdManifest.Entry entry)
        {
            return entry != null
                && !string.IsNullOrWhiteSpace(npcId)
                && string.Equals(npcId.Trim(), entry.npcId?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public bool MatchesSemanticAnchor(SpringDay1NpcCrowdManifest.Entry entry)
        {
            return entry != null
                && !string.IsNullOrWhiteSpace(semanticAnchorId)
                && entry.SupportsSemanticAnchor(semanticAnchorId);
        }

        public bool MatchesDuty(SpringDay1NpcCrowdManifest.Entry entry)
        {
            return entry != null
                && duty != SpringDay1CrowdSceneDuty.None
                && entry.SupportsDuty(duty);
        }

        public void EnsureDefaults()
        {
            cueId ??= string.Empty;
            npcId ??= string.Empty;
            semanticAnchorId ??= string.Empty;
            lookAtTargetName ??= string.Empty;
            moveSpeed = Mathf.Max(0.05f, moveSpeed);
            initialHoldSeconds = Mathf.Max(0f, initialHoldSeconds);

            if (facing.sqrMagnitude <= 0.0001f)
            {
                facing = Vector2.down;
            }

            path ??= Array.Empty<SpringDay1DirectorPathPoint>();
            for (int index = 0; index < path.Length; index++)
            {
                path[index] ??= new SpringDay1DirectorPathPoint();
                path[index].EnsureDefaults();
            }
        }
    }

    public static class SpringDay1DirectorSemanticAnchorResolver
    {
        public static bool TryResolveWorldPosition(string semanticAnchorId, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            if (string.IsNullOrWhiteSpace(semanticAnchorId))
            {
                return false;
            }

            string trimmed = semanticAnchorId.Trim();
            GameObject liveAnchor = GameObject.Find(trimmed);
            if (liveAnchor != null)
            {
                worldPosition = liveAnchor.transform.position;
                return true;
            }

            return SpringDay1TownAnchorContractDatabase.TryResolveAnchor(trimmed, out worldPosition);
        }
    }

    [Serializable]
    public sealed class SpringDay1DirectorBeatEntry
    {
        public string beatKey = string.Empty;
        public string phaseKey = string.Empty;
        public string sceneLabel = string.Empty;
        [TextArea] public string storySummary = string.Empty;
        [TextArea] public string assetizationHook = string.Empty;
        public SpringDay1DirectorActorCue[] actorCues = Array.Empty<SpringDay1DirectorActorCue>();

        public SpringDay1DirectorActorCue TryResolveCue(SpringDay1NpcCrowdManifest.Entry entry)
        {
            if (entry == null || actorCues == null)
            {
                return null;
            }

            SpringDay1DirectorActorCue semanticAnchorFallback = null;
            SpringDay1DirectorActorCue dutyFallback = null;
            for (int index = 0; index < actorCues.Length; index++)
            {
                SpringDay1DirectorActorCue cue = actorCues[index];
                if (cue == null)
                {
                    continue;
                }

                if (cue.MatchesNpcId(entry))
                {
                    return cue;
                }

                if (semanticAnchorFallback == null && cue.MatchesSemanticAnchor(entry))
                {
                    semanticAnchorFallback = cue;
                }

                if (dutyFallback == null && cue.MatchesDuty(entry))
                {
                    dutyFallback = cue;
                }
            }

            return semanticAnchorFallback ?? dutyFallback;
        }

        public void EnsureDefaults()
        {
            beatKey ??= string.Empty;
            phaseKey ??= string.Empty;
            sceneLabel ??= string.Empty;
            storySummary ??= string.Empty;
            assetizationHook ??= string.Empty;
            actorCues ??= Array.Empty<SpringDay1DirectorActorCue>();

            for (int index = 0; index < actorCues.Length; index++)
            {
                actorCues[index] ??= new SpringDay1DirectorActorCue();
                actorCues[index].EnsureDefaults();
            }
        }
    }

    [Serializable]
    public sealed class SpringDay1DirectorStageBook
    {
        public string version = "1.0";
        public SpringDay1DirectorBeatEntry[] beats = Array.Empty<SpringDay1DirectorBeatEntry>();

        public SpringDay1DirectorBeatEntry FindBeat(string beatKey)
        {
            if (string.IsNullOrWhiteSpace(beatKey) || beats == null)
            {
                return null;
            }

            for (int index = 0; index < beats.Length; index++)
            {
                SpringDay1DirectorBeatEntry beat = beats[index];
                if (beat != null
                    && string.Equals(beat.beatKey?.Trim(), beatKey.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return beat;
                }
            }

            return null;
        }

        public SpringDay1DirectorActorCue TryResolveCue(string beatKey, SpringDay1NpcCrowdManifest.Entry entry)
        {
            SpringDay1DirectorBeatEntry beat = FindBeat(beatKey);
            return beat?.TryResolveCue(entry);
        }

        public void EnsureDefaults()
        {
            version ??= "1.0";
            beats ??= Array.Empty<SpringDay1DirectorBeatEntry>();

            for (int index = 0; index < beats.Length; index++)
            {
                beats[index] ??= new SpringDay1DirectorBeatEntry();
                beats[index].EnsureDefaults();
            }
        }

        public static SpringDay1DirectorStageBook CreateEmpty()
        {
            SpringDay1DirectorStageBook book = new SpringDay1DirectorStageBook
            {
                beats = new SpringDay1DirectorBeatEntry[SpringDay1DirectorBeatKeys.OrderedKeys.Count]
            };

            for (int index = 0; index < SpringDay1DirectorBeatKeys.OrderedKeys.Count; index++)
            {
                string beatKey = SpringDay1DirectorBeatKeys.OrderedKeys[index];
                book.beats[index] = new SpringDay1DirectorBeatEntry
                {
                    beatKey = beatKey,
                    phaseKey = GuessPhaseKey(beatKey),
                    sceneLabel = beatKey,
                    storySummary = string.Empty,
                    assetizationHook = string.Empty,
                    actorCues = Array.Empty<SpringDay1DirectorActorCue>()
                };
            }

            return book;
        }

        private static string GuessPhaseKey(string beatKey)
        {
            if (string.IsNullOrWhiteSpace(beatKey))
            {
                return string.Empty;
            }

            if (beatKey.StartsWith("CrashAndMeet", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.CrashAndMeet.ToString();
            }

            if (beatKey.StartsWith("EnterVillage", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.EnterVillage.ToString();
            }

            if (beatKey.StartsWith("HealingAndHP", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.HealingAndHP.ToString();
            }

            if (beatKey.StartsWith("WorkbenchFlashback", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.WorkbenchFlashback.ToString();
            }

            if (beatKey.StartsWith("FarmingTutorial", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.FarmingTutorial.ToString();
            }

            if (beatKey.StartsWith("DinnerConflict", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.DinnerConflict.ToString();
            }

            if (beatKey.StartsWith("ReturnAndReminder", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.ReturnAndReminder.ToString();
            }

            if (beatKey.StartsWith("FreeTime", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.FreeTime.ToString();
            }

            if (beatKey.StartsWith("DayEnd", StringComparison.OrdinalIgnoreCase) || beatKey.StartsWith("DailyStand", StringComparison.OrdinalIgnoreCase))
            {
                return StoryPhase.DayEnd.ToString();
            }

            return string.Empty;
        }
    }

    public static class SpringDay1DirectorStagingDatabase
    {
        public const string ResourcePath = "Story/SpringDay1/Directing/SpringDay1DirectorStageBook";
#if UNITY_EDITOR
        public const string AssetPath = "Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json";
#endif

        private static SpringDay1DirectorStageBook _cachedBook;
        private static string _cachedRawText = string.Empty;

        public static SpringDay1DirectorStageBook Load(bool forceReload = false)
        {
            if (!forceReload && _cachedBook != null)
            {
                return _cachedBook;
            }

            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            string rawText = asset != null ? asset.text : string.Empty;

            if (!forceReload
                && _cachedBook != null
                && string.Equals(_cachedRawText, rawText, StringComparison.Ordinal))
            {
                return _cachedBook;
            }

            SpringDay1DirectorStageBook parsed = null;
            if (!string.IsNullOrWhiteSpace(rawText))
            {
                try
                {
                    parsed = JsonUtility.FromJson<SpringDay1DirectorStageBook>(rawText);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"[SpringDay1DirectorStaging] 读取 stage book 失败，将退回空表。{exception.Message}");
                }
            }

            _cachedBook = parsed ?? SpringDay1DirectorStageBook.CreateEmpty();
            _cachedBook.EnsureDefaults();
            _cachedRawText = rawText ?? string.Empty;
            return _cachedBook;
        }

        public static bool TryResolveCue(string beatKey, SpringDay1NpcCrowdManifest.Entry entry, out SpringDay1DirectorActorCue cue)
        {
            cue = null;

            if (string.IsNullOrWhiteSpace(beatKey) || entry == null)
            {
                return false;
            }

            SpringDay1DirectorStageBook book = Load();
            cue = book.TryResolveCue(beatKey, entry);
            return cue != null;
        }

#if UNITY_EDITOR
        public static void Save(SpringDay1DirectorStageBook book)
        {
            if (book == null)
            {
                return;
            }

            book.EnsureDefaults();
            Directory.CreateDirectory(Path.GetDirectoryName(AssetPath) ?? "Assets");
            string json = JsonUtility.ToJson(book, prettyPrint: true);
            File.WriteAllText(AssetPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            _cachedBook = null;
            _cachedRawText = string.Empty;
        }
#endif
    }

    [DisallowMultipleComponent]
    public sealed class SpringDay1DirectorNpcTakeover : MonoBehaviour
    {
        internal const string OwnerKey = "spring-day1-director";

        private NPCAutoRoamController _roamController;
        private NPCInformalChatInteractable _informalChatInteractable;
        private NPCDialogueInteractable _dialogueInteractable;
        private NPCBubblePresenter _bubblePresenter;
        private NPCBubbleStressTalker _stressTalker;
        private NPCMotionController _motionController;
        private int _lockDepth;
        private bool _roamWasEnabled;
        private bool _informalWasEnabled;
        private bool _dialogueWasEnabled;
        private bool _bubbleWasEnabled;
        private bool _stressWasEnabled;
        private bool _roamStoryControlOwned;
        private bool _keepRoamEnabled;

        public bool IsOwned => _lockDepth > 0;

        public void Acquire()
        {
            Acquire(keepRoamEnabled: false);
        }

        public void Acquire(bool keepRoamEnabled = false)
        {
            CacheComponents();
            if (_lockDepth == 0)
            {
                _keepRoamEnabled = keepRoamEnabled;
                _roamWasEnabled = _roamController != null && _roamController.enabled;
                _informalWasEnabled = _informalChatInteractable != null && _informalChatInteractable.enabled;
                _dialogueWasEnabled = _dialogueInteractable != null && _dialogueInteractable.enabled;
                _bubbleWasEnabled = _bubblePresenter != null && _bubblePresenter.enabled;
                _stressWasEnabled = _stressTalker != null && _stressTalker.enabled;

                if (_roamController != null)
                {
                    _roamController.AcquireStoryControl(OwnerKey, resumeAutonomousRoamWhenReleased: _roamWasEnabled);
                    _roamStoryControlOwned = true;
                    if (!keepRoamEnabled)
                    {
                        _roamController.enabled = false;
                    }
                }

                if (_informalChatInteractable != null)
                {
                    _informalChatInteractable.enabled = false;
                }

                if (_dialogueInteractable != null)
                {
                    _dialogueInteractable.enabled = false;
                }

                if (_bubblePresenter != null)
                {
                    _bubblePresenter.enabled = false;
                }

                if (_stressTalker != null)
                {
                    _stressTalker.enabled = false;
                }

                if (_motionController != null)
                {
                    _motionController.StopMotion();
                }
            }

            _lockDepth++;
        }

        public void Release()
        {
            if (_lockDepth <= 0)
            {
                return;
            }

            _lockDepth--;
            if (_lockDepth > 0)
            {
                return;
            }

            CacheComponents();
            if (_roamController != null)
            {
                if (_roamStoryControlOwned)
                {
                    _roamController.HaltStoryControlMotion(preserveResumeAutonomousRoamWhenReleased: _roamWasEnabled);
                    _roamController.ReleaseStoryControl(OwnerKey, resumeAutonomousRoam: _roamWasEnabled);
                    _roamStoryControlOwned = false;
                }

                _roamController.enabled = _roamWasEnabled;
            }

            if (_informalChatInteractable != null)
            {
                _informalChatInteractable.enabled = _informalWasEnabled;
            }

            if (_dialogueInteractable != null)
            {
                _dialogueInteractable.enabled = _dialogueWasEnabled;
            }

            if (_bubblePresenter != null)
            {
                _bubblePresenter.enabled = _bubbleWasEnabled;
            }

            if (_stressTalker != null)
            {
                _stressTalker.enabled = _stressWasEnabled;
            }

            if (_motionController != null)
            {
                _motionController.StopMotion();
            }

            _keepRoamEnabled = false;
        }

        private void OnDisable()
        {
            if (_lockDepth > 0)
            {
                _lockDepth = 1;
                Release();
            }
        }

        private void CacheComponents()
        {
            if (_roamController == null)
            {
                _roamController = GetComponent<NPCAutoRoamController>();
            }

            if (_informalChatInteractable == null)
            {
                _informalChatInteractable = GetComponent<NPCInformalChatInteractable>();
            }

            if (_dialogueInteractable == null)
            {
                _dialogueInteractable = GetComponent<NPCDialogueInteractable>();
            }

            if (_bubblePresenter == null)
            {
                _bubblePresenter = GetComponent<NPCBubblePresenter>();
            }

            if (_stressTalker == null)
            {
                _stressTalker = GetComponent<NPCBubbleStressTalker>();
            }

            if (_motionController == null)
            {
                _motionController = GetComponent<NPCMotionController>();
            }
        }
    }

    [DisallowMultipleComponent]
    public sealed class SpringDay1DirectorPlayerRehearsalLock : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerController _playerController;
        private PlayerAutoNavigator _playerAutoNavigator;
        private PlayerInteraction _playerInteraction;
        private int _lockDepth;
        private bool _movementWasEnabled;
        private bool _controllerWasEnabled;
        private bool _autoNavigatorWasEnabled;
        private bool _interactionWasEnabled;

        public bool IsOwned => _lockDepth > 0;

        public void Acquire()
        {
            CacheComponents();
            if (_lockDepth == 0)
            {
                _movementWasEnabled = _playerMovement != null && _playerMovement.enabled;
                _controllerWasEnabled = _playerController != null && _playerController.enabled;
                _autoNavigatorWasEnabled = _playerAutoNavigator != null && _playerAutoNavigator.enabled;
                _interactionWasEnabled = _playerInteraction != null && _playerInteraction.enabled;

                if (_playerAutoNavigator != null)
                {
                    _playerAutoNavigator.ForceCancel();
                    _playerAutoNavigator.enabled = false;
                }

                if (_playerInteraction != null)
                {
                    _playerInteraction.enabled = false;
                }

                if (_playerController != null)
                {
                    _playerController.enabled = false;
                }

                if (_playerMovement != null)
                {
                    _playerMovement.StopMovement();
                    _playerMovement.enabled = false;
                }
            }

            _lockDepth++;
        }

        public void Release()
        {
            if (_lockDepth <= 0)
            {
                return;
            }

            _lockDepth--;
            if (_lockDepth > 0)
            {
                return;
            }

            CacheComponents();
            if (_playerMovement != null)
            {
                _playerMovement.enabled = _movementWasEnabled;
                _playerMovement.StopMovement();
            }

            if (_playerController != null)
            {
                _playerController.enabled = _controllerWasEnabled;
            }

            if (_playerInteraction != null)
            {
                _playerInteraction.enabled = _interactionWasEnabled;
            }

            if (_playerAutoNavigator != null)
            {
                _playerAutoNavigator.enabled = _autoNavigatorWasEnabled;
                _playerAutoNavigator.ForceCancel();
            }
        }

        private void OnDisable()
        {
            if (_lockDepth > 0)
            {
                _lockDepth = 1;
                Release();
            }
        }

        private void CacheComponents()
        {
            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
            }

            GameObject playerRoot = _playerMovement != null ? _playerMovement.gameObject : null;
            if (playerRoot == null)
            {
                return;
            }

            if (_playerController == null)
            {
                _playerController = playerRoot.GetComponent<PlayerController>();
            }

            if (_playerAutoNavigator == null)
            {
                _playerAutoNavigator = playerRoot.GetComponent<PlayerAutoNavigator>();
            }

            if (_playerInteraction == null)
            {
                _playerInteraction = playerRoot.GetComponent<PlayerInteraction>();
            }
        }
    }

    [DisallowMultipleComponent]
    public sealed class SpringDay1DirectorStagingPlayback : MonoBehaviour
    {
        public const string StoryPlayerLookTargetToken = "__story_player__";
        private const float CueArrivalDistance = 0.05f;

        private NPCAutoRoamController _roamController;
        private NPCMotionController _motionController;
        private SpringDay1DirectorNpcTakeover _takeover;
        private Transform _homeAnchor;
        private SpringDay1DirectorActorCue _cue;
        private Vector3 _basePosition;
        private int _pathIndex;
        private float _holdTimer;
        private bool _hasCue;
        private bool _isManualPreviewLocked;
        private string _beatKey = string.Empty;
        private Vector2 _issuedTravelTarget;
        private bool _hasIssuedTravelTarget;

        public string CurrentCueKey => _cue != null ? _cue.StableKey : string.Empty;
        public string CurrentBeatKey => _beatKey;
        public bool IsManualPreviewLocked => _isManualPreviewLocked;

        public bool HasSettledCurrentCue()
        {
            if (!_hasCue || _cue == null)
            {
                return true;
            }

            if (_holdTimer > 0f)
            {
                return false;
            }

            if (_cue.loopPath)
            {
                return false;
            }

            return _cue.path == null
                || _cue.path.Length == 0
                || _pathIndex >= _cue.path.Length;
        }

        public bool ForceSnapToCueEnd()
        {
            if (!_hasCue || _cue == null)
            {
                return false;
            }

            SpringDay1DirectorPathPoint finalPoint = ResolveLastCuePoint();
            if (finalPoint != null)
            {
                Vector3 finalPosition = ResolveTargetPosition(finalPoint);
                transform.position = finalPosition;
                if (_roamController != null)
                {
                    _roamController.SnapToTarget(new Vector2(finalPosition.x, finalPosition.y), finalPoint.facing);
                }

                ApplyFacingOrLookAt(finalPoint.lookAtTargetName, finalPoint.facing);
            }
            else if (!_cue.keepCurrentSpawnPosition)
            {
                Vector3 startPosition = ResolveCueStartPosition();
                transform.position = startPosition;
                if (_roamController != null)
                {
                    _roamController.SnapToTarget(new Vector2(startPosition.x, startPosition.y), _cue.facing);
                }

                ApplyFacingOrLookAt(_cue.lookAtTargetName, _cue.facing);
            }

            if (_motionController != null)
            {
                _motionController.StopMotion();
            }

            _holdTimer = 0f;
            _pathIndex = _cue.path != null ? _cue.path.Length : 0;
            _hasIssuedTravelTarget = false;
            return true;
        }

        public void ApplyCue(string beatKey, SpringDay1DirectorActorCue cue, Transform homeAnchor)
        {
            ApplyCue(beatKey, cue, homeAnchor, manualPreviewLock: false, forceRestart: false);
        }

        public void ApplyCue(string beatKey, SpringDay1DirectorActorCue cue, Transform homeAnchor, bool manualPreviewLock = false)
        {
            ApplyCue(beatKey, cue, homeAnchor, manualPreviewLock, forceRestart: false);
        }

        public void ApplyCue(string beatKey, SpringDay1DirectorActorCue cue, Transform homeAnchor, bool manualPreviewLock, bool forceRestart)
        {
            if (cue == null)
            {
                ClearCue();
                return;
            }

            string nextBeatKey = beatKey ?? string.Empty;
            cue.EnsureDefaults();
            bool isSameCue = _hasCue
                && _cue != null
                && string.Equals(_beatKey, nextBeatKey, StringComparison.OrdinalIgnoreCase)
                && string.Equals(_cue.StableKey, cue.StableKey, StringComparison.OrdinalIgnoreCase);
            if (isSameCue && !forceRestart)
            {
                if (homeAnchor != null)
                {
                    _homeAnchor = homeAnchor;
                }

                _isManualPreviewLocked = manualPreviewLock;
                return;
            }

            CacheComponents();
            if (_cue != null && _cue.suspendRoam && _takeover != null)
            {
                _takeover.Release();
            }

            if (_motionController != null)
            {
                _motionController.StopMotion();
            }

            _beatKey = nextBeatKey;
            _cue = cue;
            _homeAnchor = homeAnchor;
            _pathIndex = 0;
            _holdTimer = _cue.initialHoldSeconds;
            _hasCue = true;
            _isManualPreviewLocked = manualPreviewLock;
            _hasIssuedTravelTarget = false;

            if (_cue.suspendRoam)
            {
                _takeover ??= GetOrAddComponent<SpringDay1DirectorNpcTakeover>(gameObject);
                _takeover.Acquire(keepRoamEnabled: _roamController != null);
            }

            Vector3 startPosition = ResolveCueStartPosition();
            _basePosition = startPosition;
            transform.position = startPosition;
            if (_roamController != null)
            {
                _roamController.SnapToTarget(new Vector2(startPosition.x, startPosition.y), _cue.facing);
            }

            ApplyFacingOrLookAt(_cue.lookAtTargetName, _cue.facing);
        }

        public void ClearCue()
        {
            if (_motionController != null)
            {
                _motionController.StopMotion();
            }

            if (_cue != null && _cue.suspendRoam)
            {
                _takeover ??= GetOrAddComponent<SpringDay1DirectorNpcTakeover>(gameObject);
                _takeover.Release();
            }

            _cue = null;
            _beatKey = string.Empty;
            _homeAnchor = null;
            _pathIndex = 0;
            _holdTimer = 0f;
            _hasCue = false;
            _isManualPreviewLocked = false;
            _hasIssuedTravelTarget = false;
        }

        private void OnDisable()
        {
            if (_motionController != null)
            {
                _motionController.StopMotion();
            }
        }

        private void Update()
        {
            if (!_hasCue || _cue == null)
            {
                return;
            }

            if (_cue.path == null || _cue.path.Length == 0)
            {
                StopCurrentCueMotion();
                return;
            }

            if (_holdTimer > 0f)
            {
                _holdTimer = Mathf.Max(0f, _holdTimer - Time.deltaTime);
                StopCurrentCueMotion();
                return;
            }

            if (_pathIndex < 0 || _pathIndex >= _cue.path.Length)
            {
                if (_cue.loopPath)
                {
                    _pathIndex = 0;
                }
                else
                {
                    StopCurrentCueMotion();
                    return;
                }
            }

            SpringDay1DirectorPathPoint point = _cue.path[_pathIndex];
            if (point == null)
            {
                _pathIndex++;
                _hasIssuedTravelTarget = false;
                return;
            }

            Vector3 targetPosition = ResolveTargetPosition(point);
            if (TryAdvanceCueWithRoamController(point, targetPosition))
            {
                return;
            }

            Vector3 delta = targetPosition - transform.position;
            delta.z = 0f;
            float maxDistance = Mathf.Max(0.05f, _cue.moveSpeed) * Time.deltaTime;

            if (delta.sqrMagnitude <= maxDistance * maxDistance)
            {
                transform.position = targetPosition;

                if (_motionController != null)
                {
                    _motionController.StopMotion();
                }

                ApplyFacingOrLookAt(point.lookAtTargetName, point.facing);
                _holdTimer = Mathf.Max(0f, point.holdSeconds);
                _pathIndex++;
                _hasIssuedTravelTarget = false;
                return;
            }

            Vector3 step = delta.normalized * maxDistance;
            transform.position += step;
            if (_motionController != null)
            {
                Vector2 velocity = new Vector2(step.x, step.y) / Mathf.Max(Time.deltaTime, 0.0001f);
                _motionController.ApplyDirectedMotion(velocity, new Vector2(step.x, step.y));
            }
        }

        private Vector3 ResolveTargetPosition(SpringDay1DirectorPathPoint point)
        {
            Vector2 target2D = point.position;
            if (_cue != null && _cue.pathPointsAreOffsets)
            {
                target2D += new Vector2(_basePosition.x, _basePosition.y);
            }
            else if (_cue != null && _cue.useSemanticAnchorAsStart)
            {
                target2D += new Vector2(_basePosition.x, _basePosition.y) - _cue.startPosition;
            }

            return new Vector3(target2D.x, target2D.y, transform.position.z);
        }

        private Vector3 ResolveCueStartPosition()
        {
            if (_cue == null || _cue.keepCurrentSpawnPosition)
            {
                return transform.position;
            }

            if (_cue.useSemanticAnchorAsStart
                && SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(_cue.semanticAnchorId, out Vector3 anchorPosition))
            {
                Vector2 resolvedStart = new Vector2(anchorPosition.x, anchorPosition.y);
                if (_cue.startPositionIsSemanticAnchorOffset)
                {
                    resolvedStart += _cue.startPosition;
                }

                return new Vector3(resolvedStart.x, resolvedStart.y, transform.position.z);
            }

            return new Vector3(_cue.startPosition.x, _cue.startPosition.y, transform.position.z);
        }

        private bool TryAdvanceCueWithRoamController(SpringDay1DirectorPathPoint point, Vector3 targetPosition)
        {
            if (_roamController == null || !_cue.suspendRoam)
            {
                return false;
            }

            Vector2 target2D = new Vector2(targetPosition.x, targetPosition.y);
            float distance = Vector2.Distance(transform.position, target2D);
            if (distance <= CueArrivalDistance)
            {
                _roamController.SnapToTarget(target2D, point.facing);
                _roamController.HaltStoryControlMotion(preserveResumeAutonomousRoamWhenReleased: false);
                ApplyFacingOrLookAt(point.lookAtTargetName, point.facing);
                _holdTimer = Mathf.Max(0f, point.holdSeconds);
                _pathIndex++;
                _hasIssuedTravelTarget = false;
                return true;
            }

            bool shouldIssueTravel = !_hasIssuedTravelTarget
                || Vector2.Distance(_issuedTravelTarget, target2D) > 0.01f
                || !_roamController.IsResidentScriptedMoveActive;
            if (!shouldIssueTravel)
            {
                if (_roamController.IsResidentScriptedMovePaused)
                {
                    _roamController.ResumeStoryControlMotion();
                }

                return true;
            }

            _roamController.ResumeStoryControlMotion();
            if (_roamController.RequestStageTravel(
                SpringDay1DirectorNpcTakeover.OwnerKey,
                transform.position,
                target2D,
                retargetTolerance: CueArrivalDistance,
                resumeAutonomousRoamWhenReleased: false))
            {
                _issuedTravelTarget = target2D;
                _hasIssuedTravelTarget = true;
                return true;
            }

            _hasIssuedTravelTarget = false;
            return false;
        }

        private void StopCurrentCueMotion()
        {
            if (_roamController != null && _cue != null && _cue.suspendRoam)
            {
                _roamController.HaltStoryControlMotion(preserveResumeAutonomousRoamWhenReleased: false);
                _hasIssuedTravelTarget = false;
            }

            if (_motionController != null)
            {
                _motionController.StopMotion();
            }
        }

        private SpringDay1DirectorPathPoint ResolveLastCuePoint()
        {
            if (_cue?.path == null || _cue.path.Length == 0)
            {
                return null;
            }

            for (int index = _cue.path.Length - 1; index >= 0; index--)
            {
                if (_cue.path[index] != null)
                {
                    return _cue.path[index];
                }
            }

            return null;
        }

        private void CacheComponents()
        {
            if (_roamController == null)
            {
                _roamController = GetComponent<NPCAutoRoamController>();
            }

            if (_motionController == null)
            {
                _motionController = GetComponent<NPCMotionController>();
            }
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

        private void ApplyFacingOrLookAt(string lookAtTargetName, Vector2 fallbackFacing)
        {
            if (_motionController == null)
            {
                return;
            }

            Transform lookTarget = ResolveLookTargetTransform(lookAtTargetName);
            if (lookTarget != null)
            {
                Vector3 delta = lookTarget.position - transform.position;
                Vector2 direction = new Vector2(delta.x, delta.y);
                if (direction.sqrMagnitude > 0.0001f)
                {
                    _motionController.ApplyIdleFacing(direction);
                    return;
                }
            }

            if (fallbackFacing.sqrMagnitude > 0.0001f)
            {
                _motionController.ApplyIdleFacing(fallbackFacing);
            }
        }

        private static Transform ResolveLookTargetTransform(string lookAtTargetName)
        {
            if (string.IsNullOrWhiteSpace(lookAtTargetName))
            {
                return null;
            }

            string trimmed = lookAtTargetName.Trim();
            if (string.Equals(trimmed, StoryPlayerLookTargetToken, StringComparison.Ordinal))
            {
                PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
                return playerMovement != null ? playerMovement.transform : null;
            }

            GameObject target = GameObject.Find(trimmed);
            return target != null ? target.transform : null;
        }
    }

    [DisallowMultipleComponent]
    public sealed class SpringDay1DirectorStagingRehearsalDriver : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.6f;
        [SerializeField] private float sprintMultiplier = 1.6f;

        private NPCMotionController _motionController;
        private SpringDay1DirectorNpcTakeover _takeover;
        private SpringDay1DirectorPlayerRehearsalLock _playerLock;
        private SpringDay1DirectorStagingPlayback _playback;
        private Vector2 _rehearsalInput;
        private bool _sprintHeld;
        private float _lastEditorInputAt = float.NegativeInfinity;
        private bool _playbackWasEnabled;

        public void Configure(float speed)
        {
            moveSpeed = Mathf.Max(0.2f, speed);
        }

        public void SetRehearsalInput(Vector2 input, bool sprintHeld)
        {
            _rehearsalInput = input;
            _sprintHeld = sprintHeld;
            _lastEditorInputAt = Time.unscaledTime;
        }

        private void OnEnable()
        {
            _motionController = GetComponent<NPCMotionController>();
            _takeover = GetComponent<SpringDay1DirectorNpcTakeover>();
            if (_takeover == null)
            {
                _takeover = gameObject.AddComponent<SpringDay1DirectorNpcTakeover>();
            }

            _playerLock = GetComponent<SpringDay1DirectorPlayerRehearsalLock>();
            if (_playerLock == null)
            {
                _playerLock = gameObject.AddComponent<SpringDay1DirectorPlayerRehearsalLock>();
            }

            _playback = GetComponent<SpringDay1DirectorStagingPlayback>();
            _playbackWasEnabled = _playback != null && _playback.enabled;
            if (_playbackWasEnabled)
            {
                _playback.enabled = false;
            }

            _takeover.Acquire();
            _playerLock.Acquire();
        }

        private void OnDisable()
        {
            if (_motionController != null)
            {
                _motionController.StopMotion();
            }

            if (_takeover != null)
            {
                _takeover.Release();
            }

            if (_playback != null)
            {
                _playback.enabled = _playbackWasEnabled;
            }

            if (_playerLock != null)
            {
                _playerLock.Release();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying || Time.unscaledTime - _lastEditorInputAt > 0.25f)
            {
                if (_motionController != null)
                {
                    _motionController.StopMotion();
                }

                return;
            }

            Vector2 input = _rehearsalInput;
            if (input.sqrMagnitude <= 0.0001f)
            {
                if (_motionController != null)
                {
                    _motionController.StopMotion();
                }

                return;
            }

            float speed = moveSpeed * (_sprintHeld ? sprintMultiplier : 1f);
            Vector3 delta = new Vector3(input.x, input.y, 0f).normalized * (speed * Time.deltaTime);
            transform.position += delta;

            if (_motionController != null)
            {
                Vector2 velocity = new Vector2(delta.x, delta.y) / Mathf.Max(Time.deltaTime, 0.0001f);
                _motionController.ApplyDirectedMotion(velocity, input);
            }
        }
    }
}
