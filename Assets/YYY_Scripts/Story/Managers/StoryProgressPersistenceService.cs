using System;
using System.Collections.Generic;
using FarmGame.Data.Core;
using System.Reflection;
using UnityEngine;

namespace Sunset.Story
{
    [DefaultExecutionOrder(-850)]
    public sealed class StoryProgressPersistenceService : MonoBehaviour, IPersistentObject
    {
        private const string RuntimeObjectName = "[StoryProgressPersistenceService]";
        private const string PersistentObjectId = "story-progress-persistence-service";
        private const string PersistentObjectType = "StoryProgressState";
        private const string WorkbenchHintConsumedKey = "spring-day1.workbench-entry-hint-consumed";
        private const string WorkbenchCraftingActiveFieldName = "_workbenchCraftingActive";
        private const string SpringDay1VillageGateSequenceId = "spring-day1-village-gate";
        private const string SpringDay1HouseArrivalSequenceId = "spring-day1-house-arrival";
        private const string SpringDay1HealingSequenceId = "spring-day1-healing";
        private const string SpringDay1WorkbenchSequenceId = "spring-day1-workbench";
        private const string SpringDay1DinnerSequenceId = "spring-day1-dinner";
        private const string SpringDay1ReminderSequenceId = "spring-day1-reminder";
        private const string SpringDay1FreeTimeIntroSequenceId = "spring-day1-free-time-opening";
        private static readonly BindingFlags DirectorMemberFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly BindingFlags ChatSessionMemberFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly BindingFlags NearbyFeedbackMemberFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static StoryProgressPersistenceService _instance;
        private static readonly Dictionary<string, FieldInfo> DirectorFieldCache = new Dictionary<string, FieldInfo>(StringComparer.Ordinal);
        private static readonly Dictionary<string, MethodInfo> DirectorMethodCache = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
        private static readonly Dictionary<string, MethodInfo> ChatSessionMethodCache = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
        private static readonly Dictionary<string, MethodInfo> NearbyFeedbackMethodCache = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
        private static bool s_workbenchHintConsumed;

        private bool _isRegistered;

        public string PersistentId => PersistentObjectId;
        public string ObjectType => PersistentObjectType;
        public bool ShouldSave => true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            EnsureRuntime();
        }

        public static StoryProgressPersistenceService EnsureRuntime()
        {
            StoryProgressPersistenceService service = _instance;
            if (service == null)
            {
                service = PersistentManagers.EnsureManagedComponent<StoryProgressPersistenceService>(RuntimeObjectName);
            }

            if (service != null)
            {
                if (!service.gameObject.activeSelf)
                {
                    service.gameObject.SetActive(true);
                }

                service.RegisterIfNeeded();
            }

            return service;
        }

        public static void ResetToOpeningRuntimeState()
        {
            StoryProgressPersistenceService service = EnsureRuntime();
            if (service == null)
            {
                return;
            }

            service.ApplySnapshot(CreateDefaultSnapshot());
        }

        public static void ResetToTownOpeningRuntimeState()
        {
            StoryProgressPersistenceService service = EnsureRuntime();
            if (service == null)
            {
                return;
            }

            service.ApplySnapshot(CreateTownOpeningSnapshot());
        }

        public static bool CanSaveNow(out string blockerReason)
        {
            blockerReason = string.Empty;

            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                blockerReason = "当前正式对白仍在进行；本版存档不支持把对白停在半句中间，请等对白收束后再保存。";
                return false;
            }

            PlayerNpcChatSessionService chatSessionService = FindFirstObjectByType<PlayerNpcChatSessionService>(FindObjectsInactive.Include);
            if (chatSessionService != null && chatSessionService.HasActiveConversation)
            {
                blockerReason = "当前 NPC 闲聊仍在进行；本版存档不会续存聊天过程态，请等这段交流结束后再保存。";
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director != null && director.TryGetStorySaveLoadBlockReason(out string storyBlockReason))
            {
                blockerReason = storyBlockReason;
                return false;
            }

            bool isCraftingActive = ReadPrivateFieldValue(
                director,
                DirectorFieldCache,
                WorkbenchCraftingActiveFieldName,
                DirectorMemberFlags,
                false);
            if (isCraftingActive)
            {
                blockerReason = "当前工作台仍有活跃制作队列；本版存档不支持制作途中存档，请先等待完成、领取或取消后再保存。";
                return false;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = SpringDay1WorkbenchCraftingOverlay.Instance
                ?? FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            if (overlay != null)
            {
                if (overlay.HasReadyWorkbenchOutputs)
                {
                    blockerReason = "当前工作台已有完成但未领取的产物；本版存档不会替你保留这批待领取结果，请先领取后再保存。";
                    return false;
                }

                if (overlay.HasWorkbenchFloatingState)
                {
                    blockerReason = "当前工作台仍有未收束的制作队列状态；本版存档不会续存这段临时队列，请先收束后再保存。";
                    return false;
                }
            }

            return true;
        }

        public static bool CanLoadNow(out string blockerReason)
        {
            blockerReason = string.Empty;

            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                blockerReason = "当前正式对白仍在进行；请等对白收束后再读取存档。";
                return false;
            }

            PlayerNpcChatSessionService chatSessionService = FindFirstObjectByType<PlayerNpcChatSessionService>(FindObjectsInactive.Include);
            if (chatSessionService != null && chatSessionService.HasActiveConversation)
            {
                blockerReason = "当前 NPC 闲聊仍在进行；请等这段交流结束后再读取存档。";
                return false;
            }

            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director != null && director.TryGetStorySaveLoadBlockReason(out string storyBlockReason))
            {
                blockerReason = storyBlockReason;
                return false;
            }

            return true;
        }

        public static bool IsWorkbenchHintConsumed()
        {
            return HasConsumedWorkbenchHint();
        }

        public static void MarkWorkbenchHintConsumed()
        {
            ApplyWorkbenchHintConsumed(true);
        }

        public static void FinalizeLoadedSave(List<WorldObjectSaveData> worldObjects)
        {
            StoryProgressPersistenceService service = EnsureRuntime();
            if (service == null || ContainsSelfSaveData(worldObjects))
            {
                return;
            }

            service.ApplySnapshot(CreateDefaultSnapshot());
        }

        public WorldObjectSaveData Save()
        {
            StoryProgressSaveData snapshot = CaptureSnapshot();
            return new WorldObjectSaveData
            {
                guid = PersistentId,
                objectType = ObjectType,
                sceneName = gameObject.scene.name,
                isActive = true,
                genericData = JsonUtility.ToJson(snapshot)
            };
        }

        public void Load(WorldObjectSaveData data)
        {
            StoryProgressSaveData snapshot = CreateDefaultSnapshot();

            if (data != null && !string.IsNullOrWhiteSpace(data.genericData))
            {
                try
                {
                    snapshot = JsonUtility.FromJson<StoryProgressSaveData>(data.genericData) ?? snapshot;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"[StoryProgressPersistenceService] 解析剧情长期态存档失败，已回退默认值：{exception.Message}");
                }
            }

            ApplySnapshot(snapshot);
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            gameObject.name = RuntimeObjectName;
            RegisterIfNeeded();
        }

        private void OnEnable()
        {
            RegisterIfNeeded();
        }

        private void OnDestroy()
        {
            UnregisterIfNeeded();

            if (_instance == this)
            {
                _instance = null;
            }
        }

        private StoryProgressSaveData CaptureSnapshot()
        {
            StoryProgressSaveData snapshot = CreateDefaultSnapshot();

            StoryManager storyManager = FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
            if (storyManager != null)
            {
                snapshot.storyPhase = (int)storyManager.CurrentPhase;
                snapshot.isLanguageDecoded = storyManager.IsLanguageDecoded;
            }

            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null)
            {
                snapshot.completedDialogueSequenceIds = dialogueManager.GetCompletedSequenceIdsSnapshot();
            }

            snapshot.springDay1 = CaptureSpringDay1Progress();
            snapshot.health = CaptureHealthState();
            snapshot.energy = CaptureEnergyState();
            snapshot.npcRelationships = BuildNpcRelationshipEntries(PlayerNpcRelationshipService.GetSnapshot());
            snapshot.workbenchHintConsumed = HasConsumedWorkbenchHint();
            return snapshot;
        }

        private void ApplySnapshot(StoryProgressSaveData snapshot)
        {
            StoryProgressSaveData safeSnapshot = snapshot ?? CreateDefaultSnapshot();
            StoryManager.Instance.ResetState(SanitizeStoryPhase(safeSnapshot.storyPhase), safeSnapshot.isLanguageDecoded);

            DialogueManager dialogueManager = DialogueManager.EnsureRuntime();
            dialogueManager.StopDialogue();
            dialogueManager.ReplaceCompletedSequenceIds(safeSnapshot.completedDialogueSequenceIds);

            PlayerNpcRelationshipService.ReplaceAllStages(
                BuildNpcRelationshipMap(safeSnapshot.npcRelationships),
                persist: false);

            ApplyWorkbenchHintConsumed(safeSnapshot.workbenchHintConsumed);
            ApplySpringDay1Progress(
                safeSnapshot.springDay1,
                SanitizeStoryPhase(safeSnapshot.storyPhase),
                BuildCompletedSequenceSet(safeSnapshot.completedDialogueSequenceIds));
            ApplySpringDay1ResidentRuntime(safeSnapshot.springDay1);
            ApplyHealthState(safeSnapshot.health);
            ApplyEnergyState(safeSnapshot.energy);
            ResyncSpringDay1LowEnergyState();
            ResetNpcTransientState();
        }

        private void RegisterIfNeeded()
        {
            if (_isRegistered)
            {
                return;
            }

            PersistentObjectRegistry registry = PersistentObjectRegistry.Instance;
            if (registry == null)
            {
                return;
            }

            registry.Register(this);
            _isRegistered = true;
        }

        private void UnregisterIfNeeded()
        {
            if (!_isRegistered)
            {
                return;
            }

            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.Unregister(this);
            }

            _isRegistered = false;
        }

        private static bool ContainsSelfSaveData(List<WorldObjectSaveData> worldObjects)
        {
            if (worldObjects == null)
            {
                return false;
            }

            foreach (WorldObjectSaveData data in worldObjects)
            {
                if (data != null &&
                    string.Equals(data.guid, PersistentObjectId, StringComparison.Ordinal) &&
                    string.Equals(data.objectType, PersistentObjectType, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static StoryProgressSaveData CreateDefaultSnapshot()
        {
            return new StoryProgressSaveData
            {
                storyPhase = (int)StoryPhase.CrashAndMeet,
                isLanguageDecoded = false,
                completedDialogueSequenceIds = new List<string>(),
                npcRelationships = new List<NpcRelationshipEntry>(),
                workbenchHintConsumed = false,
                springDay1 = null,
                health = null,
                energy = null
            };
        }

        private static StoryProgressSaveData CreateTownOpeningSnapshot()
        {
            StoryProgressSaveData snapshot = CreateDefaultSnapshot();
            snapshot.storyPhase = (int)StoryPhase.EnterVillage;
            snapshot.isLanguageDecoded = true;
            return snapshot;
        }

        private static StoryPhase SanitizeStoryPhase(int rawPhase)
        {
            if (Enum.IsDefined(typeof(StoryPhase), rawPhase) && rawPhase != (int)StoryPhase.None)
            {
                return (StoryPhase)rawPhase;
            }

            return StoryPhase.CrashAndMeet;
        }

        private static List<NpcRelationshipEntry> BuildNpcRelationshipEntries(IReadOnlyDictionary<string, NPCRelationshipStage> snapshot)
        {
            List<NpcRelationshipEntry> entries = new List<NpcRelationshipEntry>();
            if (snapshot == null)
            {
                return entries;
            }

            foreach (KeyValuePair<string, NPCRelationshipStage> pair in snapshot)
            {
                if (string.IsNullOrWhiteSpace(pair.Key))
                {
                    continue;
                }

                entries.Add(new NpcRelationshipEntry
                {
                    npcId = pair.Key,
                    relationshipStage = (int)NPCRelationshipStageUtility.Sanitize(pair.Value)
                });
            }

            entries.Sort((left, right) => string.CompareOrdinal(left.npcId, right.npcId));
            return entries;
        }

        private static Dictionary<string, NPCRelationshipStage> BuildNpcRelationshipMap(List<NpcRelationshipEntry> entries)
        {
            Dictionary<string, NPCRelationshipStage> map = new Dictionary<string, NPCRelationshipStage>();
            if (entries == null)
            {
                return map;
            }

            foreach (NpcRelationshipEntry entry in entries)
            {
                string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(entry.npcId);
                if (string.IsNullOrEmpty(normalizedNpcId))
                {
                    continue;
                }

                map[normalizedNpcId] = NPCRelationshipStageUtility.FromStoredValue(entry.relationshipStage);
            }

            return map;
        }

        private static bool HasConsumedWorkbenchHint()
        {
            return s_workbenchHintConsumed;
        }

        private static void ApplyWorkbenchHintConsumed(bool isConsumed)
        {
            s_workbenchHintConsumed = isConsumed;
        }

        private static SpringDay1ProgressSaveData CaptureSpringDay1Progress()
        {
            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director == null)
            {
                return null;
            }

            return new SpringDay1ProgressSaveData
            {
                tillObjectiveCompleted = ReadPrivateFieldValue(director, DirectorFieldCache, "_tillObjectiveCompleted", DirectorMemberFlags, false),
                plantObjectiveCompleted = ReadPrivateFieldValue(director, DirectorFieldCache, "_plantObjectiveCompleted", DirectorMemberFlags, false),
                waterObjectiveCompleted = ReadPrivateFieldValue(director, DirectorFieldCache, "_waterObjectiveCompleted", DirectorMemberFlags, false),
                woodObjectiveCompleted = ReadPrivateFieldValue(director, DirectorFieldCache, "_woodObjectiveCompleted", DirectorMemberFlags, false),
                woodCollectedProgress = Mathf.Max(0, ReadPrivateFieldValue(director, DirectorFieldCache, "_collectedWoodSinceWoodStepStart", DirectorMemberFlags, 0)),
                craftedCount = Mathf.Max(0, ReadPrivateFieldValue(director, DirectorFieldCache, "_craftedCount", DirectorMemberFlags, 0)),
                freeTimeEntered = ReadPrivateFieldValue(director, DirectorFieldCache, "_freeTimeEntered", DirectorMemberFlags, false),
                freeTimeIntroCompleted = ReadPrivateFieldValue(director, DirectorFieldCache, "_freeTimeIntroCompleted", DirectorMemberFlags, false),
                dayEnded = ReadPrivateFieldValue(director, DirectorFieldCache, "_dayEnded", DirectorMemberFlags, false),
                staminaRevealed = ReadPrivateFieldValue(director, DirectorFieldCache, "_staminaRevealed", DirectorMemberFlags, false),
                residentRuntimeSnapshots = SpringDay1NpcCrowdDirector.CaptureResidentRuntimeSnapshots()
            };
        }

        private static HealthStateSaveData CaptureHealthState()
        {
            HealthSystem healthSystem = FindFirstObjectByType<HealthSystem>(FindObjectsInactive.Include);
            if (healthSystem == null)
            {
                return null;
            }

            return new HealthStateSaveData
            {
                current = healthSystem.CurrentHealth,
                max = healthSystem.MaxHealth,
                visible = healthSystem.IsVisible
            };
        }

        private static EnergyStateSaveData CaptureEnergyState()
        {
            EnergySystem energySystem = FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
            if (energySystem == null)
            {
                return null;
            }

            return new EnergyStateSaveData
            {
                current = energySystem.CurrentEnergy,
                max = energySystem.MaxEnergy,
                visible = energySystem.IsVisible,
                lowEnergyWarningActive = energySystem.IsLowEnergyWarningActive
            };
        }

        private static void ApplySpringDay1Progress(
            SpringDay1ProgressSaveData snapshot,
            StoryPhase currentPhase,
            HashSet<string> completedSequenceIds)
        {
            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director == null)
            {
                return;
            }

            SpringDay1ProgressSaveData safeSnapshot = snapshot ?? new SpringDay1ProgressSaveData();
            bool hasVillageGateSequence = completedSequenceIds.Contains(SpringDay1VillageGateSequenceId);
            bool hasHouseArrivalSequence = completedSequenceIds.Contains(SpringDay1HouseArrivalSequenceId);
            bool hasHealingSequence = completedSequenceIds.Contains(SpringDay1HealingSequenceId);
            bool hasWorkbenchSequence = completedSequenceIds.Contains(SpringDay1WorkbenchSequenceId);
            bool hasDinnerSequence = completedSequenceIds.Contains(SpringDay1DinnerSequenceId);
            bool hasReminderSequence = completedSequenceIds.Contains(SpringDay1ReminderSequenceId);
            bool hasFreeTimeIntroSequence = completedSequenceIds.Contains(SpringDay1FreeTimeIntroSequenceId);

            WritePrivateFieldValue(director, DirectorFieldCache, "_villageGateSequencePlayed", DirectorMemberFlags, hasVillageGateSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_houseArrivalSequencePlayed", DirectorMemberFlags, hasHouseArrivalSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_healingStarted", DirectorMemberFlags, currentPhase >= StoryPhase.HealingAndHP);
            WritePrivateFieldValue(director, DirectorFieldCache, "_healingSequencePlayed", DirectorMemberFlags, hasHealingSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchOpened", DirectorMemberFlags, hasWorkbenchSequence || currentPhase > StoryPhase.WorkbenchFlashback);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchSequencePlayed", DirectorMemberFlags, hasWorkbenchSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_dinnerSequencePlayed", DirectorMemberFlags, hasDinnerSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_returnSequencePlayed", DirectorMemberFlags, hasReminderSequence);
            WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeNightWarningShown", DirectorMemberFlags, false);
            WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeMidnightWarningShown", DirectorMemberFlags, false);
            WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeFinalWarningShown", DirectorMemberFlags, false);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchCraftingActive", DirectorMemberFlags, false);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchCraftProgress", DirectorMemberFlags, 0f);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchCraftQueueTotal", DirectorMemberFlags, 0);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchCraftQueueCompleted", DirectorMemberFlags, 0);
            WritePrivateFieldValue(director, DirectorFieldCache, "_workbenchCraftRecipeName", DirectorMemberFlags, string.Empty);

            if (currentPhase >= StoryPhase.FarmingTutorial)
            {
                InvokePrivateMethod(
                    director,
                    DirectorMethodCache,
                    "InitializeFarmingTutorialTracking",
                    DirectorMemberFlags,
                    true);

                int currentWoodCount = 0;
                MethodInfo getCurrentWoodCountMethod = GetCachedMethod(
                    DirectorMethodCache,
                    director.GetType(),
                    "GetCurrentWoodCount",
                    DirectorMemberFlags);
                if (getCurrentWoodCountMethod != null)
                {
                    object currentWoodCountValue = getCurrentWoodCountMethod.Invoke(director, null);
                    if (currentWoodCountValue is int typedCurrentWoodCount)
                    {
                        currentWoodCount = typedCurrentWoodCount;
                    }
                    else if (currentWoodCountValue != null)
                    {
                        try
                        {
                            currentWoodCount = Convert.ToInt32(currentWoodCountValue);
                        }
                        catch
                        {
                            currentWoodCount = 0;
                        }
                    }
                }
                int savedWoodProgress = Mathf.Max(0, safeSnapshot.woodCollectedProgress);
                int baselineWoodCount = Mathf.Max(0, currentWoodCount - savedWoodProgress);
                bool woodTrackingArmed = safeSnapshot.waterObjectiveCompleted || safeSnapshot.woodObjectiveCompleted || savedWoodProgress > 0;

                WritePrivateFieldValue(director, DirectorFieldCache, "_tillObjectiveCompleted", DirectorMemberFlags, safeSnapshot.tillObjectiveCompleted);
                WritePrivateFieldValue(director, DirectorFieldCache, "_plantObjectiveCompleted", DirectorMemberFlags, safeSnapshot.plantObjectiveCompleted);
                WritePrivateFieldValue(director, DirectorFieldCache, "_waterObjectiveCompleted", DirectorMemberFlags, safeSnapshot.waterObjectiveCompleted);
                WritePrivateFieldValue(director, DirectorFieldCache, "_woodObjectiveCompleted", DirectorMemberFlags, safeSnapshot.woodObjectiveCompleted);
                WritePrivateFieldValue(director, DirectorFieldCache, "_craftedCount", DirectorMemberFlags, Mathf.Max(0, safeSnapshot.craftedCount));
                WritePrivateFieldValue(director, DirectorFieldCache, "_baselineWoodCount", DirectorMemberFlags, baselineWoodCount);
                WritePrivateFieldValue(director, DirectorFieldCache, "_trackedWoodCountSnapshot", DirectorMemberFlags, currentWoodCount);
                WritePrivateFieldValue(director, DirectorFieldCache, "_collectedWoodSinceWoodStepStart", DirectorMemberFlags, savedWoodProgress);
                WritePrivateFieldValue(director, DirectorFieldCache, "_woodTrackingArmed", DirectorMemberFlags, woodTrackingArmed);
            }
            else
            {
                WritePrivateFieldValue(director, DirectorFieldCache, "_craftedCount", DirectorMemberFlags, Mathf.Max(0, safeSnapshot.craftedCount));
            }

            bool staminaRevealed = safeSnapshot.staminaRevealed || currentPhase > StoryPhase.FarmingTutorial;
            WritePrivateFieldValue(director, DirectorFieldCache, "_staminaRevealed", DirectorMemberFlags, staminaRevealed);
            WritePrivateFieldValue(director, DirectorFieldCache, "_lowEnergyWarned", DirectorMemberFlags, false);

            if (currentPhase == StoryPhase.FreeTime && safeSnapshot.freeTimeEntered && !safeSnapshot.dayEnded)
            {
                WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeEntered", DirectorMemberFlags, false);
                WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeIntroCompleted", DirectorMemberFlags, false);
                WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeIntroQueued", DirectorMemberFlags, false);
                WritePrivateFieldValue(director, DirectorFieldCache, "_dayEnded", DirectorMemberFlags, false);
                InvokePrivateMethod(director, DirectorMethodCache, "EnterFreeTime", DirectorMemberFlags);
            }
            else
            {
                WritePrivateFieldValue(director, DirectorFieldCache, "_freeTimeEntered", DirectorMemberFlags, safeSnapshot.freeTimeEntered && currentPhase >= StoryPhase.FreeTime);
                WritePrivateFieldValue(
                    director,
                    DirectorFieldCache,
                    "_freeTimeIntroCompleted",
                    DirectorMemberFlags,
                    safeSnapshot.freeTimeIntroCompleted || hasFreeTimeIntroSequence);
                WritePrivateFieldValue(
                    director,
                    DirectorFieldCache,
                    "_freeTimeIntroQueued",
                    DirectorMemberFlags,
                    safeSnapshot.freeTimeIntroCompleted || hasFreeTimeIntroSequence);
                WritePrivateFieldValue(director, DirectorFieldCache, "_dayEnded", DirectorMemberFlags, safeSnapshot.dayEnded && currentPhase >= StoryPhase.DayEnd);
            }

            director.HideTaskListBridgePrompt();
            InvokePrivateMethod(
                director,
                DirectorMethodCache,
                "RestoreLoadedDayEndTaskCardState",
                DirectorMemberFlags);
            InvokePrivateMethod(director, DirectorMethodCache, "SyncStoryTimePauseState", DirectorMemberFlags);
            if (currentPhase == StoryPhase.DayEnd)
            {
                EnergySystem energySystem = FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
                if (energySystem != null)
                {
                    energySystem.FullRestore();
                    energySystem.SetLowEnergyWarningVisual(false);
                }
            }
        }

        private static void ApplyHealthState(HealthStateSaveData snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            HealthSystem healthSystem = HealthSystem.Instance;
            if (healthSystem == null)
            {
                return;
            }

            healthSystem.SetHealthState(snapshot.current, snapshot.max);
            healthSystem.SetVisible(snapshot.visible);
        }

        private static void ApplyEnergyState(EnergyStateSaveData snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            EnergySystem energySystem = FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
            if (energySystem == null)
            {
                return;
            }

            energySystem.SetEnergyState(snapshot.current, snapshot.max);
            energySystem.SetVisible(snapshot.visible);
            energySystem.SetLowEnergyWarningVisual(snapshot.lowEnergyWarningActive);
        }

        private static void ApplySpringDay1ResidentRuntime(SpringDay1ProgressSaveData snapshot)
        {
            if (snapshot?.residentRuntimeSnapshots == null || snapshot.residentRuntimeSnapshots.Count <= 0)
            {
                SpringDay1NpcCrowdDirector.ClearResidentRuntimeSnapshots();
                return;
            }

            SpringDay1NpcCrowdDirector.ApplyResidentRuntimeSnapshots(snapshot.residentRuntimeSnapshots);
        }

        private static void ResyncSpringDay1LowEnergyState()
        {
            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director == null)
            {
                return;
            }

            InvokePrivateMethod(director, DirectorMethodCache, "ResyncLowEnergyState", DirectorMemberFlags, false);
        }

        private static void ResetNpcTransientState()
        {
            PlayerNpcChatSessionService chatSessionService = FindFirstObjectByType<PlayerNpcChatSessionService>(FindObjectsInactive.Include);
            if (chatSessionService != null)
            {
                InvokePrivateMethod(
                    chatSessionService,
                    ChatSessionMethodCache,
                    "CancelConversationImmediate",
                    ChatSessionMemberFlags,
                    PlayerNpcChatSessionService.ConversationEndReason.Cancelled,
                    NPCInformalChatLeaveCause.ServiceDisabled);
                InvokePrivateMethod(chatSessionService, ChatSessionMethodCache, "ClearPendingResumeSnapshot", ChatSessionMemberFlags);
                InvokePrivateMethod(chatSessionService, ChatSessionMethodCache, "ResetValidationSnapshot", ChatSessionMemberFlags);
                InvokePrivateMethod(chatSessionService, ChatSessionMethodCache, "ResetConversationBubbleVisualsImmediate", ChatSessionMemberFlags);
            }

            PlayerNpcNearbyFeedbackService nearbyFeedbackService = FindFirstObjectByType<PlayerNpcNearbyFeedbackService>(FindObjectsInactive.Include);
            if (nearbyFeedbackService != null)
            {
                InvokePrivateMethod(nearbyFeedbackService, NearbyFeedbackMethodCache, "HideActiveNearbyBubble", NearbyFeedbackMemberFlags);
                InvokePrivateMethod(nearbyFeedbackService, NearbyFeedbackMethodCache, "SyncDialogueSuppressionState", NearbyFeedbackMemberFlags);
            }
        }

        private static HashSet<string> BuildCompletedSequenceSet(List<string> sequenceIds)
        {
            HashSet<string> set = new HashSet<string>(StringComparer.Ordinal);
            if (sequenceIds == null)
            {
                return set;
            }

            foreach (string sequenceId in sequenceIds)
            {
                if (!string.IsNullOrWhiteSpace(sequenceId))
                {
                    set.Add(sequenceId.Trim());
                }
            }

            return set;
        }

        private static T ReadPrivateFieldValue<T>(
            object target,
            Dictionary<string, FieldInfo> cache,
            string fieldName,
            BindingFlags bindingFlags,
            T fallback)
        {
            if (target == null)
            {
                return fallback;
            }

            FieldInfo field = GetCachedField(cache, target.GetType(), fieldName, bindingFlags);
            if (field == null)
            {
                return fallback;
            }

            object value = field.GetValue(target);
            if (value is T typedValue)
            {
                return typedValue;
            }

            if (value == null)
            {
                return fallback;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return fallback;
            }
        }

        private static void WritePrivateFieldValue<T>(
            object target,
            Dictionary<string, FieldInfo> cache,
            string fieldName,
            BindingFlags bindingFlags,
            T value)
        {
            if (target == null)
            {
                return;
            }

            FieldInfo field = GetCachedField(cache, target.GetType(), fieldName, bindingFlags);
            if (field == null)
            {
                return;
            }

            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod(
            object target,
            Dictionary<string, MethodInfo> cache,
            string methodName,
            BindingFlags bindingFlags,
            params object[] args)
        {
            if (target == null)
            {
                return;
            }

            MethodInfo method = GetCachedMethod(cache, target.GetType(), methodName, bindingFlags);
            if (method == null)
            {
                return;
            }

            method.Invoke(target, args);
        }

        private static FieldInfo GetCachedField(
            Dictionary<string, FieldInfo> cache,
            Type targetType,
            string fieldName,
            BindingFlags bindingFlags)
        {
            if (targetType == null || string.IsNullOrWhiteSpace(fieldName))
            {
                return null;
            }

            if (!cache.TryGetValue(fieldName, out FieldInfo field))
            {
                field = targetType.GetField(fieldName, bindingFlags);
                cache[fieldName] = field;
            }

            return field;
        }

        private static MethodInfo GetCachedMethod(
            Dictionary<string, MethodInfo> cache,
            Type targetType,
            string methodName,
            BindingFlags bindingFlags)
        {
            if (targetType == null || string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            if (!cache.TryGetValue(methodName, out MethodInfo method))
            {
                method = targetType.GetMethod(methodName, bindingFlags);
                cache[methodName] = method;
            }

            return method;
        }

        [Serializable]
        private sealed class StoryProgressSaveData
        {
            public int storyPhase = (int)StoryPhase.CrashAndMeet;
            public bool isLanguageDecoded;
            public List<string> completedDialogueSequenceIds = new List<string>();
            public List<NpcRelationshipEntry> npcRelationships = new List<NpcRelationshipEntry>();
            public bool workbenchHintConsumed;
            public SpringDay1ProgressSaveData springDay1;
            public HealthStateSaveData health;
            public EnergyStateSaveData energy;
        }

        [Serializable]
        private sealed class SpringDay1ProgressSaveData
        {
            public bool tillObjectiveCompleted;
            public bool plantObjectiveCompleted;
            public bool waterObjectiveCompleted;
            public bool woodObjectiveCompleted;
            public int woodCollectedProgress;
            public int craftedCount;
            public bool freeTimeEntered;
            public bool freeTimeIntroCompleted;
            public bool dayEnded;
            public bool staminaRevealed;
            public List<SpringDay1NpcCrowdDirector.ResidentRuntimeSnapshotEntry> residentRuntimeSnapshots =
                new List<SpringDay1NpcCrowdDirector.ResidentRuntimeSnapshotEntry>();
        }

        [Serializable]
        private sealed class NpcRelationshipEntry
        {
            public string npcId;
            public int relationshipStage;
        }

        [Serializable]
        private sealed class HealthStateSaveData
        {
            public int current;
            public int max;
            public bool visible;
        }

        [Serializable]
        private sealed class EnergyStateSaveData
        {
            public int current;
            public int max;
            public bool visible;
            public bool lowEnergyWarningActive;
        }
    }
}
