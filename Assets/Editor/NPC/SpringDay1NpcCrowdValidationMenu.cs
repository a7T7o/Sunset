#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sunset.Story;
using UnityEditor;
using UnityEngine;

public static class SpringDay1NpcCrowdValidationMenu
{
    private const string MenuPath = "Tools/NPC/Spring Day1/Validate New Cast";
    private const string RuntimeProbeMenuPath = "Tools/NPC/Spring Day1/Run Runtime Targeted Probe";
    // Keep the runtime probe isolated from Day1 integration and legacy scene drift.
    private const string SpriteRoot = "Assets/Sprites/NPC";
    private const string AnimationRoot = "Assets/100_Anim/NPC";
    private const string DataRoot = "Assets/111_Data/NPC/SpringDay1Crowd";
    private const string PrefabRoot = "Assets/222_Prefabs/NPC";
    private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";
    private const string RuntimeProbeRootName = "__SpringDay1NpcCrowdRuntimeProbeRoot";
    private const float RuntimeProbeBaseX = 120f;
    private const float RuntimeProbeBaseY = 120f;
    private const float RuntimeProbeSpacingX = 2.6f;
    private const float RuntimeProbeSpacingY = 2.3f;
    private const double RuntimeProbeSettleSeconds = 0.35d;
    private const double RuntimeProbeSelfTalkTimeoutSeconds = 4.2d;
    private const double RuntimeProbeChatTimeoutSeconds = 7.5d;
    private const double RuntimeProbePairTimeoutSeconds = 6.0d;
    private const double RuntimeProbeInterruptTimeoutSeconds = 12.0d;
    private const double RuntimeProbeInterruptFallbackTriggerDelaySeconds = 0.65d;
    private const int RuntimeProbeInterruptNaturalCompletionRetryLimit = 1;
    private const double RuntimeProbeCleanupTimeoutSeconds = 1.5d;
    private const float RuntimeProbePairDialogueDurationSeconds = 5.2f;
    private static readonly string[] ExpectedNpcIds = { "101", "102", "103", "104", "201", "202", "203" };
    private static readonly Dictionary<string, SpringDay1CrowdSceneDuty[]> ExpectedSceneDutiesByNpcId =
        new Dictionary<string, SpringDay1CrowdSceneDuty[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["101"] = new[]
            {
                SpringDay1CrowdSceneDuty.EnterVillagePostEntryCrowd,
                SpringDay1CrowdSceneDuty.DinnerBackground,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["102"] = new[]
            {
                SpringDay1CrowdSceneDuty.NightWitness,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["103"] = new[]
            {
                SpringDay1CrowdSceneDuty.EnterVillagePostEntryCrowd,
                SpringDay1CrowdSceneDuty.EnterVillageKidLook,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["104"] = new[]
            {
                SpringDay1CrowdSceneDuty.DinnerBackground,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["201"] = new[]
            {
                SpringDay1CrowdSceneDuty.DinnerBackground,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["202"] = new[]
            {
                SpringDay1CrowdSceneDuty.DinnerBackground,
                SpringDay1CrowdSceneDuty.DailyStand
            },
            ["203"] = new[]
            {
                SpringDay1CrowdSceneDuty.DinnerBackground,
                SpringDay1CrowdSceneDuty.DailyStand
            }
        };

    private static readonly Dictionary<string, string[]> ExpectedSemanticAnchorsByNpcId =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["101"] = new[] { "EnterVillageCrowdRoot", "DinnerBackgroundRoot", "DailyStand_01" },
            ["102"] = new[] { "NightWitness_01", "DailyStand_03" },
            ["103"] = new[] { "EnterVillageCrowdRoot", "KidLook_01", "DailyStand_02" },
            ["104"] = new[] { "DinnerBackgroundRoot", "DailyStand_01" },
            ["201"] = new[] { "DinnerBackgroundRoot", "DailyStand_02" },
            ["202"] = new[] { "DinnerBackgroundRoot", "DailyStand_03" },
            ["203"] = new[] { "DinnerBackgroundRoot", "DailyStand_01" }
        };

    private static readonly Dictionary<string, SpringDay1CrowdGrowthIntent> ExpectedGrowthIntentByNpcId =
        new Dictionary<string, SpringDay1CrowdGrowthIntent>(StringComparer.OrdinalIgnoreCase)
        {
            ["101"] = SpringDay1CrowdGrowthIntent.UpgradeCandidate,
            ["102"] = SpringDay1CrowdGrowthIntent.UpgradeCandidate,
            ["103"] = SpringDay1CrowdGrowthIntent.UpgradeCandidate,
            ["104"] = SpringDay1CrowdGrowthIntent.StableSupport,
            ["201"] = SpringDay1CrowdGrowthIntent.HoldAnonymous,
            ["202"] = SpringDay1CrowdGrowthIntent.HoldAnonymous,
            ["203"] = SpringDay1CrowdGrowthIntent.StableSupport
        };

    private static readonly Dictionary<string, SpringDay1CrowdResidentBaseline> ExpectedResidentBaselineByNpcId =
        new Dictionary<string, SpringDay1CrowdResidentBaseline>(StringComparer.OrdinalIgnoreCase)
        {
            ["101"] = SpringDay1CrowdResidentBaseline.DaytimeResident,
            ["102"] = SpringDay1CrowdResidentBaseline.PeripheralResident,
            ["103"] = SpringDay1CrowdResidentBaseline.DaytimeResident,
            ["104"] = SpringDay1CrowdResidentBaseline.PeripheralResident,
            ["201"] = SpringDay1CrowdResidentBaseline.DaytimeResident,
            ["202"] = SpringDay1CrowdResidentBaseline.PeripheralResident,
            ["203"] = SpringDay1CrowdResidentBaseline.DaytimeResident
        };

    private static readonly string[] ExpectedResidentBeatKeys =
    {
        "EnterVillage_PostEntry",
        "DinnerConflict_Table",
        "ReturnAndReminder_WalkBack",
        "FreeTime_NightWitness",
        "DayEnd_Settle",
        "DailyStand_Preview"
    };

    private static readonly Dictionary<string, string[]> ExpectedPriorityNpcIdsByBeat =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["EnterVillage_PostEntry"] = new[] { "101", "103" },
            ["DinnerConflict_Table"] = new[] { "101", "103", "104", "201", "202", "203" },
            ["FreeTime_NightWitness"] = new[] { "102" }
        };

    private static readonly Dictionary<string, string[]> ExpectedSupportNpcIdsByBeat =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["EnterVillage_PostEntry"] = new[] { "104", "201", "202", "203" },
            ["DinnerConflict_Table"] = Array.Empty<string>(),
            ["FreeTime_NightWitness"] = new[] { "101" }
        };

    private static readonly Dictionary<string, string[]> ExpectedBackstagePressureNpcIdsByBeat =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["EnterVillage_PostEntry"] = Array.Empty<string>(),
            ["DinnerConflict_Table"] = new[] { "102" },
            ["FreeTime_NightWitness"] = new[] { "103" }
        };

    private static readonly Dictionary<string, string[]> ExpectedTraceNpcIdsByBeat =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["EnterVillage_PostEntry"] = new[] { "102" },
            ["DinnerConflict_Table"] = Array.Empty<string>(),
            ["FreeTime_NightWitness"] = new[] { "104", "201", "202", "203" }
        };

    private static readonly Dictionary<string, StoryPhase[]> ExpectedPhaseFallbackPhasesByNpcId =
        new Dictionary<string, StoryPhase[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["101"] = new[] { StoryPhase.EnterVillage, StoryPhase.DinnerConflict, StoryPhase.DayEnd },
            ["102"] = new[] { StoryPhase.ReturnAndReminder, StoryPhase.FreeTime, StoryPhase.DayEnd },
            ["103"] = new[] { StoryPhase.EnterVillage, StoryPhase.DinnerConflict, StoryPhase.DayEnd },
            ["104"] = new[] { StoryPhase.WorkbenchFlashback, StoryPhase.DinnerConflict, StoryPhase.DayEnd },
            ["201"] = new[] { StoryPhase.HealingAndHP, StoryPhase.DinnerConflict, StoryPhase.DayEnd },
            ["202"] = new[] { StoryPhase.FarmingTutorial, StoryPhase.ReturnAndReminder, StoryPhase.DayEnd },
            ["203"] = new[] { StoryPhase.DinnerConflict, StoryPhase.ReturnAndReminder, StoryPhase.DayEnd }
        };

    private static readonly string[][] PairProbeNpcIds =
    {
        new[] { "101", "103" },
        new[] { "201", "202" }
    };

    private static readonly InterruptProbeSpec[] InterruptProbeSpecs =
    {
        new InterruptProbeSpec("101", interruptDuringPlayerTyping: false),
        new InterruptProbeSpec("201", interruptDuringPlayerTyping: true)
    };

    private static readonly SelfTalkProbeSpec[] SelfTalkProbeSpecs =
    {
        new SelfTalkProbeSpec("101", StoryPhase.EnterVillage),
        new SelfTalkProbeSpec("202", StoryPhase.ReturnAndReminder)
    };

    private enum RuntimeProbePhase
    {
        Idle = 0,
        SettlingInstances = 1,
        SelfTalkStart = 2,
        SelfTalkWait = 3,
        InformalChatStart = 4,
        InformalChatWait = 5,
        InformalChatCleanup = 6,
        PairStart = 7,
        PairWait = 8,
        InterruptStart = 9,
        InterruptWait = 10,
        InterruptCleanup = 11
    }

    private sealed class RuntimeProbeNpc
    {
        public string NpcId;
        public GameObject Instance;
        public GameObject HomeAnchor;
        public NPCAutoRoamController RoamController;
        public NPCInformalChatInteractable ChatInteractable;
        public NPCMotionController MotionController;
        public NPCAnimController AnimController;
        public NPCBubblePresenter BubblePresenter;
        public SpriteRenderer SpriteRenderer;
        public Animator Animator;

        public string ProbeName => Instance != null ? Instance.name : $"Probe_{NpcId}";
    }

    private sealed class InterruptProbeSpec
    {
        public readonly string NpcId;
        public readonly bool InterruptDuringPlayerTyping;

        public InterruptProbeSpec(string npcId, bool interruptDuringPlayerTyping)
        {
            NpcId = npcId;
            InterruptDuringPlayerTyping = interruptDuringPlayerTyping;
        }
    }

    private sealed class SelfTalkProbeSpec
    {
        public readonly string NpcId;
        public readonly StoryPhase StoryPhase;

        public SelfTalkProbeSpec(string npcId, StoryPhase storyPhase)
        {
            NpcId = npcId;
            StoryPhase = storyPhase;
        }
    }

    private static readonly List<RuntimeProbeNpc> RuntimeProbeNpcs = new List<RuntimeProbeNpc>();
    private static readonly List<string> RuntimeProbeIssues = new List<string>();
    private static readonly List<string> RuntimeProbeEvidence = new List<string>();
    private static readonly int[] RuntimeProbeInterruptRetryCounts = new int[InterruptProbeSpecs.Length];

    private static RuntimeProbePhase s_runtimeProbePhase;
    private static double s_runtimePhaseStartedAt;
    private static int s_runtimeSelfTalkIndex;
    private static int s_runtimeChatIndex;
    private static int s_runtimePairIndex;
    private static int s_runtimeInterruptIndex;
    private static RuntimeProbeNpc s_runtimeSelfTalkNpc;
    private static RuntimeProbeNpc s_runtimeActiveNpc;
    private static RuntimeProbeNpc s_runtimePairInitiator;
    private static RuntimeProbeNpc s_runtimePairResponder;
    private static bool s_runtimeInterruptTriggered;
    private static bool s_runtimeInterruptExpediteRequested;
    private static string s_runtimePairSeenInitiatorText;
    private static string s_runtimePairSeenResponderText;
    private static PlayerMovement s_runtimePlayerMovement;
    private static PlayerNpcChatSessionService s_runtimeSessionService;
    private static GameObject s_runtimeProbeRoot;
    private static Vector3 s_runtimeOriginalPlayerPosition;
    private static bool s_runtimePlayerPositionCaptured;
    private static bool s_runtimePreviousRunInBackground;
    private static bool s_runtimeRunInBackgroundOverridden;
    private static int s_instanceRuntimePassCount;
    private static int s_selfTalkPassCount;
    private static int s_informalChatPassCount;
    private static int s_pairDialoguePassCount;
    private static int s_walkAwayPassCount;

    [MenuItem(MenuPath)]
    private static void ValidateNewCast()
    {
        List<string> issues = new List<string>();
        SpringDay1NpcCrowdManifest manifest = AssetDatabase.LoadAssetAtPath<SpringDay1NpcCrowdManifest>(ManifestPath);
        if (manifest == null)
        {
            issues.Add($"missing manifest: {ManifestPath}");
        }

        Dictionary<string, SpringDay1NpcCrowdManifest.Entry> entryByNpcId = BuildEntryLookup(manifest, issues);
        Dictionary<string, NPCDialogueContentProfile> dialogueByNpcId = BuildDialogueLookup();
        if (dialogueByNpcId.Count != ExpectedNpcIds.Length)
        {
            issues.Add($"dialogue asset count mismatch: expected={ExpectedNpcIds.Length}, actual={dialogueByNpcId.Count}");
        }

        int roamProfileCount = CountNpcAssetsPresent<NPCRoamProfile>("RoamProfile.asset");
        if (roamProfileCount != ExpectedNpcIds.Length)
        {
            issues.Add($"roam profile count mismatch: expected={ExpectedNpcIds.Length}, actual={roamProfileCount}");
        }

        int totalPairLinks = 0;
        for (int index = 0; index < ExpectedNpcIds.Length; index++)
        {
            string npcId = ExpectedNpcIds[index];
            ValidateNpc(npcId, entryByNpcId, dialogueByNpcId, issues, ref totalPairLinks);
        }

        ValidatePairReciprocity(dialogueByNpcId, issues);
        ValidateDirectorConsumptionSlices(manifest, issues);

        if (issues.Count > 0)
        {
            Debug.LogError(
                $"[SpringDay1NpcCrowdValidation] FAIL | issues={issues.Count}\n- {string.Join("\n- ", issues)}");
            return;
        }

        Debug.Log(
            $"[SpringDay1NpcCrowdValidation] PASS | npcCount={ExpectedNpcIds.Length} | totalPairLinks={totalPairLinks} | roots={DataRoot} / {PrefabRoot} / {ManifestPath}");
    }

    [MenuItem(RuntimeProbeMenuPath)]
    private static void RunRuntimeTargetedProbe()
    {
        StopRuntimeTargetedProbe(stopPlayMode: false, logStop: false);

        if (!TryGetRuntimePlayerContext(out s_runtimePlayerMovement, out s_runtimeSessionService))
        {
            return;
        }

        EnsureRuntimeRunInBackground();
        ResetRuntimeProbeState();
        PrepareRuntimeProbeInstances();
        PositionNonPairNpcsFarAway();
        MovePlayerToProbePoint(new Vector2(RuntimeProbeBaseX - 1.2f, RuntimeProbeBaseY));

        s_runtimeProbePhase = RuntimeProbePhase.SettlingInstances;
        s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        EditorApplication.update -= TickRuntimeTargetedProbe;
        EditorApplication.update += TickRuntimeTargetedProbe;
        Debug.Log("[SpringDay1NpcRuntimeProbe] START | 目标=当前7人本体层 runtime targeted probe");
    }

    [MenuItem(RuntimeProbeMenuPath, true)]
    private static bool ValidateRuntimeProbeMenu()
    {
        return EditorApplication.isPlaying;
    }

    private static void TickRuntimeTargetedProbe()
    {
        if (!EditorApplication.isPlaying)
        {
            StopRuntimeTargetedProbe(stopPlayMode: false, logStop: false);
            return;
        }

        if (!TryGetRuntimePlayerContext(out s_runtimePlayerMovement, out s_runtimeSessionService))
        {
            RuntimeProbeIssues.Add("runtime probe failed: player context missing in play mode");
            FinishRuntimeTargetedProbe();
            return;
        }

        ClearDialogueInterference();

        switch (s_runtimeProbePhase)
        {
            case RuntimeProbePhase.SettlingInstances:
                if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt >= RuntimeProbeSettleSeconds)
                {
                    EvaluateInstanceRuntime();
                    s_runtimeProbePhase = RuntimeProbePhase.SelfTalkStart;
                    s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
                }
                break;

            case RuntimeProbePhase.SelfTalkStart:
                StartNextSelfTalkProbe();
                break;

            case RuntimeProbePhase.SelfTalkWait:
                TickSelfTalkProbe();
                break;

            case RuntimeProbePhase.InformalChatStart:
                StartNextInformalChatProbe();
                break;

            case RuntimeProbePhase.InformalChatWait:
                TickInformalChatProbe();
                break;

            case RuntimeProbePhase.InformalChatCleanup:
                TickInformalChatCleanup();
                break;

            case RuntimeProbePhase.PairStart:
                StartNextPairProbe();
                break;

            case RuntimeProbePhase.PairWait:
                TickPairProbe();
                break;

            case RuntimeProbePhase.InterruptStart:
                StartNextInterruptProbe();
                break;

            case RuntimeProbePhase.InterruptWait:
                TickInterruptProbe();
                break;

            case RuntimeProbePhase.InterruptCleanup:
                TickInterruptCleanup();
                break;
        }
    }

    private static void ResetRuntimeProbeState()
    {
        ClearRuntimePairPhaseOverride();
        RuntimeProbeNpcs.Clear();
        RuntimeProbeIssues.Clear();
        RuntimeProbeEvidence.Clear();
        for (int index = 0; index < RuntimeProbeInterruptRetryCounts.Length; index++)
        {
            RuntimeProbeInterruptRetryCounts[index] = 0;
        }
        s_runtimeProbePhase = RuntimeProbePhase.Idle;
        s_runtimePhaseStartedAt = 0d;
        s_runtimeSelfTalkIndex = 0;
        s_runtimeChatIndex = 0;
        s_runtimePairIndex = 0;
        s_runtimeInterruptIndex = 0;
        s_runtimeSelfTalkNpc = null;
        s_runtimeActiveNpc = null;
        s_runtimePairInitiator = null;
        s_runtimePairResponder = null;
        s_runtimeInterruptTriggered = false;
        s_runtimeInterruptExpediteRequested = false;
        s_runtimePairSeenInitiatorText = string.Empty;
        s_runtimePairSeenResponderText = string.Empty;
        s_instanceRuntimePassCount = 0;
        s_selfTalkPassCount = 0;
        s_informalChatPassCount = 0;
        s_pairDialoguePassCount = 0;
        s_walkAwayPassCount = 0;
    }

    private static void PrepareRuntimeProbeInstances()
    {
        if (s_runtimeProbeRoot != null)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(s_runtimeProbeRoot);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(s_runtimeProbeRoot);
            }
        }

        s_runtimeProbeRoot = new GameObject(RuntimeProbeRootName);

        SpringDay1NpcCrowdManifest manifest = AssetDatabase.LoadAssetAtPath<SpringDay1NpcCrowdManifest>(ManifestPath);
        Dictionary<string, SpringDay1NpcCrowdManifest.Entry> entryByNpcId = BuildEntryLookup(manifest, RuntimeProbeIssues);

        for (int index = 0; index < ExpectedNpcIds.Length; index++)
        {
            string npcId = ExpectedNpcIds[index];
            Vector3 spawnPosition = ResolveRuntimeProbeSpawnPosition(index);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/{npcId}.prefab");
            if (prefab == null)
            {
                RuntimeProbeIssues.Add($"{npcId}: runtime probe prefab missing");
                continue;
            }

            GameObject instance = UnityEngine.Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            instance.name = $"Probe_{npcId}";
            instance.transform.SetParent(s_runtimeProbeRoot.transform, true);

            GameObject homeAnchor = new GameObject($"{instance.name}_HomeAnchor");
            homeAnchor.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            homeAnchor.transform.SetParent(s_runtimeProbeRoot.transform, true);

            RuntimeProbeNpc probeNpc = new RuntimeProbeNpc
            {
                NpcId = npcId,
                Instance = instance,
                HomeAnchor = homeAnchor,
                RoamController = instance.GetComponent<NPCAutoRoamController>(),
                ChatInteractable = instance.GetComponent<NPCInformalChatInteractable>(),
                MotionController = instance.GetComponent<NPCMotionController>(),
                AnimController = instance.GetComponent<NPCAnimController>(),
                BubblePresenter = instance.GetComponent<NPCBubblePresenter>(),
                SpriteRenderer = instance.GetComponent<SpriteRenderer>(),
                Animator = instance.GetComponent<Animator>()
            };

            if (probeNpc.RoamController != null)
            {
                probeNpc.RoamController.SetHomeAnchor(homeAnchor.transform);
                probeNpc.RoamController.ApplyProfile();
                probeNpc.RoamController.StartRoam();
            }

            if (probeNpc.MotionController != null &&
                entryByNpcId.TryGetValue(npcId, out SpringDay1NpcCrowdManifest.Entry entry) &&
                entry.initialFacing.sqrMagnitude > 0.0001f)
            {
                probeNpc.MotionController.SetFacingDirection(entry.initialFacing);
            }

            RuntimeProbeNpcs.Add(probeNpc);
        }

        Physics2D.SyncTransforms();
    }

    private static void EvaluateInstanceRuntime()
    {
        for (int index = 0; index < RuntimeProbeNpcs.Count; index++)
        {
            RuntimeProbeNpc probeNpc = RuntimeProbeNpcs[index];
            if (probeNpc == null || probeNpc.Instance == null)
            {
                RuntimeProbeIssues.Add($"{ExpectedNpcIds[index]}: runtime instance missing");
                continue;
            }

            Transform boundHomeAnchor = probeNpc.RoamController != null
                ? GetBoundHomeAnchor(probeNpc.RoamController)
                : null;

            bool pass =
                probeNpc.SpriteRenderer != null &&
                probeNpc.SpriteRenderer.sprite != null &&
                probeNpc.Animator != null &&
                probeNpc.Animator.runtimeAnimatorController != null &&
                probeNpc.AnimController != null &&
                probeNpc.MotionController != null &&
                probeNpc.RoamController != null &&
                probeNpc.RoamController.RoamProfile != null &&
                probeNpc.ChatInteractable != null &&
                probeNpc.BubblePresenter != null &&
                boundHomeAnchor != null;

            if (!pass)
            {
                RuntimeProbeIssues.Add(
                    $"{probeNpc.NpcId}: runtime instance broken | " +
                    $"sprite={(probeNpc.SpriteRenderer != null && probeNpc.SpriteRenderer.sprite != null)} | " +
                    $"animator={(probeNpc.Animator != null && probeNpc.Animator.runtimeAnimatorController != null)} | " +
                    $"animController={(probeNpc.AnimController != null)} | " +
                    $"motion={(probeNpc.MotionController != null)} | " +
                    $"roam={(probeNpc.RoamController != null)} | " +
                    $"roamProfile={(probeNpc.RoamController != null && probeNpc.RoamController.RoamProfile != null)} | " +
                    $"chat={(probeNpc.ChatInteractable != null)} | " +
                    $"bubble={(probeNpc.BubblePresenter != null)} | " +
                    $"homeAnchor={(boundHomeAnchor != null)}");
                continue;
            }

            s_instanceRuntimePassCount++;
            RuntimeProbeEvidence.Add(
                $"{probeNpc.NpcId}: instance-ok | facing={probeNpc.AnimController.CurrentDirection} | " +
                $"isRoaming={probeNpc.RoamController.IsRoaming} | homeAnchor={boundHomeAnchor.name}");
        }
    }

    private static void StartNextSelfTalkProbe()
    {
        if (s_runtimeSelfTalkIndex >= SelfTalkProbeSpecs.Length)
        {
            ClearRuntimeValidationPhaseOverride();
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        SelfTalkProbeSpec spec = SelfTalkProbeSpecs[s_runtimeSelfTalkIndex];
        s_runtimeSelfTalkNpc = FindProbeNpc(spec.NpcId);
        if (s_runtimeSelfTalkNpc == null)
        {
            RuntimeProbeIssues.Add($"{spec.NpcId}: selfTalk probe target missing");
            s_runtimeSelfTalkIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.SelfTalkStart;
            return;
        }

        AbortActiveConversationIfNeeded();
        PositionNonPairNpcsFarAway();
        ResetProbeNpcPosition(s_runtimeSelfTalkNpc, new Vector2(RuntimeProbeBaseX, RuntimeProbeBaseY), restartRoam: false);
        MovePlayerToProbePoint(new Vector2(RuntimeProbeBaseX - 8f, RuntimeProbeBaseY - 6f));
        ApplyRuntimeValidationPhaseOverride(spec.StoryPhase);
        s_runtimeSelfTalkNpc.RoamController?.DebugEnterLongPause();
        s_runtimeProbePhase = RuntimeProbePhase.SelfTalkWait;
        s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void TickSelfTalkProbe()
    {
        if (s_runtimeSelfTalkNpc == null)
        {
            RuntimeProbeIssues.Add("selfTalk probe failed: npc state missing");
            ClearRuntimeValidationPhaseOverride();
            s_runtimeSelfTalkIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.SelfTalkStart;
            return;
        }

        SelfTalkProbeSpec spec = SelfTalkProbeSpecs[s_runtimeSelfTalkIndex];
        string currentText = s_runtimeSelfTalkNpc.BubblePresenter != null ? s_runtimeSelfTalkNpc.BubblePresenter.CurrentBubbleText : string.Empty;
        string lastText = s_runtimeSelfTalkNpc.BubblePresenter != null ? s_runtimeSelfTalkNpc.BubblePresenter.LastPresentedText : string.Empty;
        if (IsExpectedSelfTalkLine(s_runtimeSelfTalkNpc, spec.StoryPhase, currentText) ||
            IsExpectedSelfTalkLine(s_runtimeSelfTalkNpc, spec.StoryPhase, lastText))
        {
            s_selfTalkPassCount++;
            RuntimeProbeEvidence.Add(
                $"{s_runtimeSelfTalkNpc.NpcId}: selfTalk-ok | phase={spec.StoryPhase} | " +
                $"text=\"{(!string.IsNullOrWhiteSpace(currentText) ? currentText : lastText)}\"");
            ClearRuntimeValidationPhaseOverride();
            s_runtimeSelfTalkNpc.BubblePresenter?.HideImmediateBubble();
            s_runtimeSelfTalkIndex++;
            s_runtimeSelfTalkNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.SelfTalkStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbeSelfTalkTimeoutSeconds)
        {
            RuntimeProbeIssues.Add(
                $"{s_runtimeSelfTalkNpc.NpcId}: selfTalk timeout | phase={spec.StoryPhase} | " +
                $"current=\"{currentText}\" | last=\"{lastText}\" | " +
                $"expected={DescribeResolvedSelfTalkLines(s_runtimeSelfTalkNpc, spec.StoryPhase)} | " +
                $"bubble={DescribeBubblePresenterState(s_runtimeSelfTalkNpc.BubblePresenter)}");
            ClearRuntimeValidationPhaseOverride();
            s_runtimeSelfTalkNpc.BubblePresenter?.HideImmediateBubble();
            s_runtimeSelfTalkIndex++;
            s_runtimeSelfTalkNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.SelfTalkStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void StartNextInformalChatProbe()
    {
        if (s_runtimeChatIndex >= RuntimeProbeNpcs.Count)
        {
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        s_runtimeActiveNpc = RuntimeProbeNpcs[s_runtimeChatIndex];
        if (!PrepareProbeNpcForConversation(s_runtimeActiveNpc))
        {
            s_runtimeChatIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatStart;
            return;
        }

        s_runtimeProbePhase = RuntimeProbePhase.InformalChatWait;
        s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
    }

    private static bool PrepareProbeNpcForConversation(RuntimeProbeNpc probeNpc)
    {
        if (probeNpc == null || probeNpc.Instance == null || probeNpc.ChatInteractable == null)
        {
            RuntimeProbeIssues.Add($"{probeNpc?.NpcId ?? "unknown"}: informal chat probe target missing");
            return false;
        }

        AbortActiveConversationIfNeeded();
        PositionNonPairNpcsFarAway();
        ResetProbeNpcPosition(probeNpc, new Vector2(RuntimeProbeBaseX, RuntimeProbeBaseY), restartRoam: false);
        MovePlayerNearInteractable(probeNpc.ChatInteractable, 0.28f);
        InteractionContext context = BuildInteractionContext(s_runtimePlayerMovement);
        bool handled = probeNpc.ChatInteractable.TryHandleInteract(context);
        if (!handled)
        {
            RuntimeProbeIssues.Add($"{probeNpc.NpcId}: informal chat did not start");
            return false;
        }

        return true;
    }

    private static void TickInformalChatProbe()
    {
        if (s_runtimeActiveNpc == null || s_runtimeSessionService == null)
        {
            RuntimeProbeIssues.Add("informal chat probe failed: session service missing");
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (s_runtimeSessionService.IsConversationActiveWith(s_runtimeActiveNpc.ChatInteractable) &&
            s_runtimeSessionService.CompletedExchangeCount >= 1 &&
            !string.IsNullOrWhiteSpace(s_runtimeSessionService.LastCompletedPlayerLine) &&
            !string.IsNullOrWhiteSpace(s_runtimeSessionService.LastCompletedNpcLine))
        {
            s_informalChatPassCount++;
            RuntimeProbeEvidence.Add(
                $"{s_runtimeActiveNpc.NpcId}: informal-chat-ok | player=\"{s_runtimeSessionService.LastCompletedPlayerLine}\" | " +
                $"npc=\"{s_runtimeSessionService.LastCompletedNpcLine}\"");
            s_runtimeSessionService.TryForceAbortForValidation(NPCInformalChatLeaveCause.TargetInvalid);
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbeChatTimeoutSeconds)
        {
            RuntimeProbeIssues.Add(
                $"{s_runtimeActiveNpc.NpcId}: informal chat timeout | " +
                $"state={s_runtimeSessionService.DebugStateName} | " +
                $"playerText=\"{s_runtimeSessionService.CurrentPlayerBubbleText}\" | " +
                $"npcText=\"{s_runtimeSessionService.CurrentNpcBubbleText}\"");
            s_runtimeSessionService.TryForceAbortForValidation(NPCInformalChatLeaveCause.TargetInvalid);
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void TickInformalChatCleanup()
    {
        if (!HasActiveConversation())
        {
            s_runtimeChatIndex++;
            s_runtimeActiveNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbeCleanupTimeoutSeconds)
        {
            AbortActiveConversationIfNeeded();
            s_runtimeChatIndex++;
            s_runtimeActiveNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.InformalChatStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void StartNextPairProbe()
    {
        if (s_runtimePairIndex >= PairProbeNpcIds.Length)
        {
            ClearRuntimePairPhaseOverride();
            s_runtimeProbePhase = RuntimeProbePhase.InterruptStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        ApplyRuntimePairPhaseOverride();
        string[] pairIds = PairProbeNpcIds[s_runtimePairIndex];
        s_runtimePairInitiator = FindProbeNpc(pairIds[0]);
        s_runtimePairResponder = FindProbeNpc(pairIds[1]);
        s_runtimePairSeenInitiatorText = string.Empty;
        s_runtimePairSeenResponderText = string.Empty;
        if (s_runtimePairInitiator == null || s_runtimePairResponder == null)
        {
            RuntimeProbeIssues.Add($"{pairIds[0]}<{pairIds[1]}: pair probe target missing");
            s_runtimePairIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            return;
        }

        AbortActiveConversationIfNeeded();
        ClearPairProbeBubbleLocks();
        PreparePairProbePositions(s_runtimePairInitiator, s_runtimePairResponder);
        bool triggered = ForceAmbientPairDialogue(s_runtimePairInitiator, s_runtimePairResponder);
        if (!triggered)
        {
            RuntimeProbeIssues.Add($"{s_runtimePairInitiator.NpcId}<{s_runtimePairResponder.NpcId}: pair dialogue failed to trigger");
            s_runtimePairIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            return;
        }

        s_runtimeProbePhase = RuntimeProbePhase.PairWait;
        s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void TickPairProbe()
    {
        if (s_runtimePairInitiator == null || s_runtimePairResponder == null)
        {
            RuntimeProbeIssues.Add("pair probe failed: pair state missing");
            s_runtimePairIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            return;
        }

        string initiatorText = s_runtimePairInitiator.BubblePresenter != null ? s_runtimePairInitiator.BubblePresenter.CurrentBubbleText : string.Empty;
        string responderText = s_runtimePairResponder.BubblePresenter != null ? s_runtimePairResponder.BubblePresenter.CurrentBubbleText : string.Empty;
        string initiatorLastText = s_runtimePairInitiator.BubblePresenter != null ? s_runtimePairInitiator.BubblePresenter.LastPresentedText : string.Empty;
        string responderLastText = s_runtimePairResponder.BubblePresenter != null ? s_runtimePairResponder.BubblePresenter.LastPresentedText : string.Empty;
        bool initiatorDecisionMatched = s_runtimePairInitiator.RoamController != null &&
                                        s_runtimePairInitiator.RoamController.LastAmbientDecision.IndexOf(s_runtimePairResponder.ProbeName, StringComparison.OrdinalIgnoreCase) >= 0;
        bool responderDecisionMatched = s_runtimePairResponder.RoamController != null &&
                                        s_runtimePairResponder.RoamController.LastAmbientDecision.IndexOf(s_runtimePairInitiator.ProbeName, StringComparison.OrdinalIgnoreCase) >= 0;
        if (string.IsNullOrWhiteSpace(s_runtimePairSeenInitiatorText) &&
            (IsExpectedPairLine(s_runtimePairInitiator, s_runtimePairResponder, initiatorText, initiator: true) ||
             IsExpectedPairLine(s_runtimePairInitiator, s_runtimePairResponder, initiatorLastText, initiator: true)))
        {
            s_runtimePairSeenInitiatorText = !string.IsNullOrWhiteSpace(initiatorText) ? initiatorText : initiatorLastText;
        }

        if (string.IsNullOrWhiteSpace(s_runtimePairSeenResponderText) &&
            (IsExpectedPairLine(s_runtimePairResponder, s_runtimePairInitiator, responderText, initiator: false) ||
             IsExpectedPairLine(s_runtimePairResponder, s_runtimePairInitiator, responderLastText, initiator: false)))
        {
            s_runtimePairSeenResponderText = !string.IsNullOrWhiteSpace(responderText) ? responderText : responderLastText;
        }

        if (initiatorDecisionMatched &&
            responderDecisionMatched &&
            !string.IsNullOrWhiteSpace(s_runtimePairSeenInitiatorText) &&
            !string.IsNullOrWhiteSpace(s_runtimePairSeenResponderText))
        {
            s_pairDialoguePassCount++;
            RuntimeProbeEvidence.Add(
                $"{s_runtimePairInitiator.NpcId}<{s_runtimePairResponder.NpcId}: pair-ok | " +
                $"initiator=\"{s_runtimePairSeenInitiatorText}\" | responder=\"{s_runtimePairSeenResponderText}\"");
            BreakAmbientChatLink(s_runtimePairInitiator);
            BreakAmbientChatLink(s_runtimePairResponder);
            s_runtimePairIndex++;
            s_runtimePairInitiator = null;
            s_runtimePairResponder = null;
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbePairTimeoutSeconds)
        {
            RuntimeProbeIssues.Add(
                $"{s_runtimePairInitiator.NpcId}<{s_runtimePairResponder.NpcId}: pair dialogue timeout | " +
                $"initiatorDecision={s_runtimePairInitiator.RoamController?.LastAmbientDecision ?? "missing"} | " +
                $"responderDecision={s_runtimePairResponder.RoamController?.LastAmbientDecision ?? "missing"} | " +
                $"initiatorText=\"{initiatorText}\" | responderText=\"{responderText}\" | " +
                $"initiatorLastText=\"{initiatorLastText}\" | responderLastText=\"{responderLastText}\" | " +
                $"seenInitiatorText=\"{s_runtimePairSeenInitiatorText}\" | seenResponderText=\"{s_runtimePairSeenResponderText}\" | " +
                $"initiatorLines={DescribeResolvedAmbientLines(s_runtimePairInitiator, s_runtimePairResponder, initiator: true)} | " +
                $"responderLines={DescribeResolvedAmbientLines(s_runtimePairResponder, s_runtimePairInitiator, initiator: false)} | " +
                $"initiatorBubble={DescribeBubblePresenterState(s_runtimePairInitiator.BubblePresenter)} | " +
                $"responderBubble={DescribeBubblePresenterState(s_runtimePairResponder.BubblePresenter)} | " +
                $"conversationOwner={DescribeConversationBubbleOwner()}");
            BreakAmbientChatLink(s_runtimePairInitiator);
            BreakAmbientChatLink(s_runtimePairResponder);
            s_runtimePairIndex++;
            s_runtimePairInitiator = null;
            s_runtimePairResponder = null;
            s_runtimeProbePhase = RuntimeProbePhase.PairStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void StartNextInterruptProbe()
    {
        if (s_runtimeInterruptIndex >= InterruptProbeSpecs.Length)
        {
            FinishRuntimeTargetedProbe();
            return;
        }

        InterruptProbeSpec spec = InterruptProbeSpecs[s_runtimeInterruptIndex];
        s_runtimeActiveNpc = FindProbeNpc(spec.NpcId);
        s_runtimeInterruptTriggered = false;
        s_runtimeInterruptExpediteRequested = false;
        if (!PrepareProbeNpcForConversation(s_runtimeActiveNpc))
        {
            s_runtimeInterruptIndex++;
            s_runtimeProbePhase = RuntimeProbePhase.InterruptStart;
            return;
        }

        s_runtimeProbePhase = RuntimeProbePhase.InterruptWait;
        s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void TickInterruptProbe()
    {
        if (s_runtimeActiveNpc == null || s_runtimeSessionService == null)
        {
            RuntimeProbeIssues.Add("walk-away probe failed: active npc or session service missing");
            s_runtimeProbePhase = RuntimeProbePhase.InterruptCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        InterruptProbeSpec spec = InterruptProbeSpecs[s_runtimeInterruptIndex];
        bool hasActiveConversation = HasActiveConversation();
        string stateName = s_runtimeSessionService.DebugStateName;
        double probeElapsed = EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt;

        if (!s_runtimeInterruptTriggered)
        {
            bool shouldTrigger = spec.InterruptDuringPlayerTyping
                ? stateName == "PlayerTyping" &&
                  !string.IsNullOrWhiteSpace(s_runtimeSessionService.CurrentPlayerBubbleText)
                : stateName == "WaitingNpcReply" ||
                  (hasActiveConversation &&
                   s_runtimeSessionService.CompletedExchangeCount >= 1 &&
                   probeElapsed >= RuntimeProbeInterruptFallbackTriggerDelaySeconds);

            if (shouldTrigger && s_runtimeSessionService.TryStartWalkAwayInterruptForValidation())
            {
                s_runtimeInterruptTriggered = true;
                s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
                return;
            }
        }

        if (!s_runtimeInterruptTriggered &&
            !hasActiveConversation &&
            s_runtimeSessionService.LastConversationEndReason == PlayerNpcChatSessionService.ConversationEndReason.Completed)
        {
            int retryIndex = Mathf.Clamp(s_runtimeInterruptIndex, 0, RuntimeProbeInterruptRetryCounts.Length - 1);
            if (RuntimeProbeInterruptRetryCounts[retryIndex] < RuntimeProbeInterruptNaturalCompletionRetryLimit)
            {
                RuntimeProbeInterruptRetryCounts[retryIndex]++;
                if (PrepareProbeNpcForConversation(s_runtimeActiveNpc))
                {
                    s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
                    return;
                }
            }

            RuntimeProbeIssues.Add(
                $"{s_runtimeActiveNpc.NpcId}: walk-away precondition missed | mode={(spec.InterruptDuringPlayerTyping ? "PlayerTyping" : "WaitingNpcReply")} | " +
                $"retries={RuntimeProbeInterruptRetryCounts[retryIndex]} | state={stateName} | endReason={s_runtimeSessionService.LastConversationEndReasonName}");
            s_runtimeProbePhase = RuntimeProbePhase.InterruptCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (s_runtimeInterruptTriggered &&
            !HasActiveConversation() &&
            s_runtimeSessionService.LastConversationEndReason == PlayerNpcChatSessionService.ConversationEndReason.WalkAwayInterrupt &&
            !string.IsNullOrWhiteSpace(s_runtimeSessionService.LastInterruptedPlayerLine) &&
            !string.IsNullOrWhiteSpace(s_runtimeSessionService.LastInterruptedNpcLine))
        {
            s_walkAwayPassCount++;
            RuntimeProbeEvidence.Add(
                $"{s_runtimeActiveNpc.NpcId}: walk-away-ok | mode={(spec.InterruptDuringPlayerTyping ? "PlayerTyping" : "WaitingNpcReply")} | " +
                $"playerExit=\"{s_runtimeSessionService.LastInterruptedPlayerLine}\" | " +
                $"npcReaction=\"{s_runtimeSessionService.LastInterruptedNpcLine}\"");
            s_runtimeProbePhase = RuntimeProbePhase.InterruptCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (s_runtimeInterruptTriggered &&
            !s_runtimeInterruptExpediteRequested &&
            s_runtimeSessionService.DebugStateName == "Interrupting")
        {
            s_runtimeInterruptExpediteRequested = s_runtimeSessionService.RequestAdvanceOrSkipActiveConversation();
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbeInterruptTimeoutSeconds)
        {
            RuntimeProbeIssues.Add(
                $"{s_runtimeActiveNpc.NpcId}: walk-away timeout | mode={(spec.InterruptDuringPlayerTyping ? "PlayerTyping" : "WaitingNpcReply")} | " +
                $"triggered={s_runtimeInterruptTriggered} | state={s_runtimeSessionService.DebugStateName} | " +
                $"endReason={s_runtimeSessionService.LastConversationEndReasonName} | " +
                $"playerExit=\"{s_runtimeSessionService.LastInterruptedPlayerLine}\" | " +
                $"npcReaction=\"{s_runtimeSessionService.LastInterruptedNpcLine}\"");
            AbortActiveConversationIfNeeded();
            s_runtimeProbePhase = RuntimeProbePhase.InterruptCleanup;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void TickInterruptCleanup()
    {
        if (!HasActiveConversation())
        {
            s_runtimeInterruptIndex++;
            s_runtimeActiveNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.InterruptStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
            return;
        }

        if (EditorApplication.timeSinceStartup - s_runtimePhaseStartedAt > RuntimeProbeCleanupTimeoutSeconds)
        {
            AbortActiveConversationIfNeeded();
            s_runtimeInterruptIndex++;
            s_runtimeActiveNpc = null;
            s_runtimeProbePhase = RuntimeProbePhase.InterruptStart;
            s_runtimePhaseStartedAt = EditorApplication.timeSinceStartup;
        }
    }

    private static void FinishRuntimeTargetedProbe()
    {
        string summary =
            $"instance={s_instanceRuntimePassCount}/{ExpectedNpcIds.Length} | " +
            $"selfTalk={s_selfTalkPassCount}/{SelfTalkProbeSpecs.Length} | " +
            $"informal={s_informalChatPassCount}/{ExpectedNpcIds.Length} | " +
            $"pair={s_pairDialoguePassCount}/{PairProbeNpcIds.Length} | " +
            $"walkAway={s_walkAwayPassCount}/{InterruptProbeSpecs.Length}";

        if (RuntimeProbeIssues.Count > 0)
        {
            for (int index = 0; index < RuntimeProbeIssues.Count; index++)
            {
                Debug.LogError($"[SpringDay1NpcRuntimeProbe][Issue] {RuntimeProbeIssues[index]}");
            }

            Debug.LogError(
                $"[SpringDay1NpcRuntimeProbe] FAIL | {summary}\n- {string.Join("\n- ", RuntimeProbeIssues)}");
        }
        else
        {
            for (int index = 0; index < RuntimeProbeEvidence.Count; index++)
            {
                Debug.Log($"[SpringDay1NpcRuntimeProbe][Evidence] {RuntimeProbeEvidence[index]}");
            }

            Debug.Log($"[SpringDay1NpcRuntimeProbe] PASS | {summary}\n- {string.Join("\n- ", RuntimeProbeEvidence)}");
        }

        StopRuntimeTargetedProbe(stopPlayMode: true, logStop: false);
    }

    private static void StopRuntimeTargetedProbe(bool stopPlayMode, bool logStop)
    {
        EditorApplication.update -= TickRuntimeTargetedProbe;
        AbortActiveConversationIfNeeded();
        ClearRuntimePairPhaseOverride();
        RestoreRuntimeRunInBackground();

        if (s_runtimePlayerPositionCaptured && s_runtimePlayerMovement != null)
        {
            RestorePlayerPosition();
        }

        if (s_runtimeProbeRoot != null)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(s_runtimeProbeRoot);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(s_runtimeProbeRoot);
            }
            s_runtimeProbeRoot = null;
        }

        RuntimeProbeNpcs.Clear();
        s_runtimeProbePhase = RuntimeProbePhase.Idle;
        s_runtimeSelfTalkNpc = null;
        s_runtimeActiveNpc = null;
        s_runtimePairInitiator = null;
        s_runtimePairResponder = null;
        s_runtimePlayerMovement = null;
        s_runtimeSessionService = null;
        s_runtimePlayerPositionCaptured = false;

        if (logStop)
        {
            Debug.Log("[SpringDay1NpcRuntimeProbe] STOP");
        }

        if (stopPlayMode && EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
    }

    private static void ApplyRuntimePairPhaseOverride()
    {
        ApplyRuntimeValidationPhaseOverride(StoryPhase.FreeTime);
    }

    private static void ClearRuntimePairPhaseOverride()
    {
        ClearRuntimeValidationPhaseOverride();
    }

    private static void ApplyRuntimeValidationPhaseOverride(StoryPhase storyPhase)
    {
        NpcInteractionPriorityPolicy.SetEditorValidationPhaseOverride(storyPhase);
    }

    private static void ClearRuntimeValidationPhaseOverride()
    {
        NpcInteractionPriorityPolicy.SetEditorValidationPhaseOverride(null);
    }

    private static Dictionary<string, SpringDay1NpcCrowdManifest.Entry> BuildEntryLookup(
        SpringDay1NpcCrowdManifest manifest,
        List<string> issues)
    {
        Dictionary<string, SpringDay1NpcCrowdManifest.Entry> entryByNpcId = new Dictionary<string, SpringDay1NpcCrowdManifest.Entry>(StringComparer.OrdinalIgnoreCase);
        if (manifest == null)
        {
            return entryByNpcId;
        }

        SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries;
        if (entries.Length != ExpectedNpcIds.Length)
        {
            issues.Add($"manifest entry count mismatch: expected={ExpectedNpcIds.Length}, actual={entries.Length}");
        }

        for (int index = 0; index < entries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = entries[index];
            if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
            {
                issues.Add($"manifest entry[{index}] missing npcId");
                continue;
            }

            if (!entryByNpcId.TryAdd(entry.npcId, entry))
            {
                issues.Add($"manifest duplicate npcId: {entry.npcId}");
            }
        }

        return entryByNpcId;
    }

    private static void ValidateNpc(
        string npcId,
        IReadOnlyDictionary<string, SpringDay1NpcCrowdManifest.Entry> entryByNpcId,
        IReadOnlyDictionary<string, NPCDialogueContentProfile> dialogueByNpcId,
        List<string> issues,
        ref int totalPairLinks)
    {
        Texture2D spriteTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{SpriteRoot}/{npcId}.png");
        NPCDialogueContentProfile dialogue = dialogueByNpcId.TryGetValue(npcId, out NPCDialogueContentProfile cachedDialogue)
            ? cachedDialogue
            : LoadNpcAsset<NPCDialogueContentProfile>(npcId, "DialogueContent.asset");
        NPCRoamProfile roamProfile = LoadNpcAsset<NPCRoamProfile>(npcId, "RoamProfile.asset");
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/{npcId}.prefab");
        RuntimeAnimatorController controller = LoadSingleAsset<RuntimeAnimatorController>($"{AnimationRoot}/{npcId}/Controller");
        string[] clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { $"{AnimationRoot}/{npcId}/Clips" });

        if (spriteTexture == null)
        {
            issues.Add($"{npcId}: sprite texture missing");
        }

        if (dialogue == null)
        {
            issues.Add($"{npcId}: dialogue content missing");
        }

        if (roamProfile == null)
        {
            issues.Add($"{npcId}: roam profile missing");
        }

        if (prefab == null)
        {
            issues.Add($"{npcId}: prefab missing");
        }

        if (controller == null)
        {
            issues.Add($"{npcId}: animator controller missing");
        }

        if (clipGuids.Length != 6)
        {
            issues.Add($"{npcId}: animation clip count mismatch -> {clipGuids.Length}");
        }

        if (!entryByNpcId.ContainsKey(npcId))
        {
            issues.Add($"{npcId}: manifest entry missing");
        }
        else if (prefab != null && entryByNpcId[npcId].prefab != prefab)
        {
            issues.Add($"{npcId}: manifest prefab reference mismatch");
        }
        else
        {
            ValidateManifestEntrySemantics(npcId, entryByNpcId[npcId], issues);
        }

        if (dialogue != null)
        {
            if (!string.Equals(dialogue.ResolveNpcId(npcId), npcId, StringComparison.OrdinalIgnoreCase))
            {
                issues.Add($"{npcId}: dialogue npcId mismatch -> {dialogue.ResolveNpcId(npcId)}");
            }

            if (dialogue.DefaultInformalConversationBundles.Length == 0 || !dialogue.HasInformalConversationContent)
            {
                issues.Add($"{npcId}: informal conversation bundle missing");
            }

            ValidatePhaseInformalFallback(npcId, dialogue, issues);
            ValidatePhaseSelfTalkCoverage(npcId, dialogue, issues);
            ValidatePhaseNearbyCoverage(npcId, dialogue, issues);

            if (dialogue.PlayerNearbyLines.Length == 0)
            {
                issues.Add($"{npcId}: playerNearbyLines missing");
            }

            if (!dialogue.DefaultWalkAwayReaction.HasAnyContent())
            {
                issues.Add($"{npcId}: walkAwayReaction missing");
            }

            if (dialogue.PairDialogueSets.Length == 0)
            {
                issues.Add($"{npcId}: pairDialogueSets missing");
            }
            else
            {
                ValidatePairDialogueSets(npcId, dialogue, issues);
            }

            totalPairLinks += dialogue.PairDialogueSets.Length;
        }

        if (roamProfile != null && dialogue != null && roamProfile.DialogueContentProfile != dialogue)
        {
            issues.Add($"{npcId}: roam profile dialogue reference mismatch");
        }

        if (prefab == null)
        {
            return;
        }

        NPCAutoRoamController roamController = prefab.GetComponent<NPCAutoRoamController>();
        if (roamController == null)
        {
            issues.Add($"{npcId}: prefab missing NPCAutoRoamController");
        }
        else if (roamProfile != null && roamController.RoamProfile != roamProfile)
        {
            issues.Add($"{npcId}: prefab roamProfile reference mismatch");
        }

        if (prefab.GetComponent<NPCInformalChatInteractable>() == null)
        {
            issues.Add($"{npcId}: prefab missing NPCInformalChatInteractable");
        }

        if (prefab.GetComponent<NPCBubblePresenter>() == null)
        {
            issues.Add($"{npcId}: prefab missing NPCBubblePresenter");
        }

        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            issues.Add($"{npcId}: prefab missing SpriteRenderer");
        }
        else if (spriteRenderer.sprite == null)
        {
            issues.Add($"{npcId}: prefab sprite missing");
        }
        else if (spriteTexture != null)
        {
            string spritePath = AssetDatabase.GetAssetPath(spriteRenderer.sprite);
            string texturePath = AssetDatabase.GetAssetPath(spriteTexture);
            if (!string.Equals(spritePath, texturePath, StringComparison.OrdinalIgnoreCase))
            {
                issues.Add($"{npcId}: prefab sprite source mismatch -> {spritePath}");
            }
        }

        Animator animator = prefab.GetComponent<Animator>();
        if (animator == null)
        {
            issues.Add($"{npcId}: prefab missing Animator");
        }
        else if (animator.runtimeAnimatorController == null)
        {
            issues.Add($"{npcId}: prefab animator controller missing");
        }
        else if (controller != null && animator.runtimeAnimatorController != controller)
        {
            issues.Add($"{npcId}: prefab animator controller reference mismatch");
        }
        else
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips == null || clips.Length != 6)
            {
                issues.Add($"{npcId}: animator clip count mismatch -> {(clips == null ? 0 : clips.Length)}");
            }
        }

        if (prefab.GetComponent<NPCAnimController>() == null)
        {
            issues.Add($"{npcId}: prefab missing NPCAnimController");
        }

        if (prefab.GetComponent<NPCMotionController>() == null)
        {
            issues.Add($"{npcId}: prefab missing NPCMotionController");
        }
    }

    private static void ValidatePairDialogueSets(
        string npcId,
        NPCDialogueContentProfile dialogue,
        List<string> issues)
    {
        HashSet<string> partners = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        NPCDialogueContentProfile.PairDialogueSet[] pairSets = dialogue.PairDialogueSets;
        for (int index = 0; index < pairSets.Length; index++)
        {
            NPCDialogueContentProfile.PairDialogueSet pairSet = pairSets[index];
            if (pairSet == null || string.IsNullOrWhiteSpace(pairSet.PartnerNpcId))
            {
                issues.Add($"{npcId}: pairDialogueSet[{index}] missing partnerNpcId");
                continue;
            }

            if (!partners.Add(pairSet.PartnerNpcId))
            {
                issues.Add($"{npcId}: duplicate pairDialogue partner -> {pairSet.PartnerNpcId}");
            }

            if (string.Equals(pairSet.PartnerNpcId, npcId, StringComparison.OrdinalIgnoreCase))
            {
                issues.Add($"{npcId}: pairDialogue partner cannot be self");
            }

            if (Array.IndexOf(ExpectedNpcIds, pairSet.PartnerNpcId) < 0)
            {
                issues.Add($"{npcId}: pairDialogue partner unknown -> {pairSet.PartnerNpcId}");
            }

            if (pairSet.InitiatorLines == null || pairSet.InitiatorLines.Length == 0)
            {
                issues.Add($"{npcId}: pairDialogue {pairSet.PartnerNpcId} missing initiatorLines");
            }

            if (pairSet.ResponderLines == null || pairSet.ResponderLines.Length == 0)
            {
                issues.Add($"{npcId}: pairDialogue {pairSet.PartnerNpcId} missing responderLines");
            }
        }
    }

    private static void ValidatePairReciprocity(
        IReadOnlyDictionary<string, NPCDialogueContentProfile> dialogueByNpcId,
        List<string> issues)
    {
        foreach (KeyValuePair<string, NPCDialogueContentProfile> pair in dialogueByNpcId)
        {
            string npcId = pair.Key;
            NPCDialogueContentProfile dialogue = pair.Value;
            if (dialogue == null)
            {
                continue;
            }

            NPCDialogueContentProfile.PairDialogueSet[] pairSets = dialogue.PairDialogueSets;
            for (int index = 0; index < pairSets.Length; index++)
            {
                NPCDialogueContentProfile.PairDialogueSet pairSet = pairSets[index];
                if (pairSet == null || string.IsNullOrWhiteSpace(pairSet.PartnerNpcId))
                {
                    continue;
                }

                if (!dialogueByNpcId.TryGetValue(pairSet.PartnerNpcId, out NPCDialogueContentProfile partnerDialogue) || partnerDialogue == null)
                {
                    continue;
                }

                if (!HasReversePair(partnerDialogue, npcId))
                {
                    issues.Add($"{npcId}: pairDialogue {pairSet.PartnerNpcId} missing reverse pair");
                }
            }
        }
    }

    private static void ValidateDirectorConsumptionSlices(
        SpringDay1NpcCrowdManifest manifest,
        List<string> issues)
    {
        if (manifest == null)
        {
            return;
        }

        ValidateDirectorConsumptionRole(manifest, "EnterVillage_PostEntry", SpringDay1CrowdDirectorConsumptionRole.Priority, ExpectedPriorityNpcIdsByBeat["EnterVillage_PostEntry"], issues);
        ValidateDirectorConsumptionRole(manifest, "EnterVillage_PostEntry", SpringDay1CrowdDirectorConsumptionRole.Support, ExpectedSupportNpcIdsByBeat["EnterVillage_PostEntry"], issues);
        ValidateDirectorConsumptionRole(manifest, "EnterVillage_PostEntry", SpringDay1CrowdDirectorConsumptionRole.Trace, ExpectedTraceNpcIdsByBeat["EnterVillage_PostEntry"], issues);
        ValidateDirectorConsumptionRole(manifest, "DinnerConflict_Table", SpringDay1CrowdDirectorConsumptionRole.Priority, ExpectedPriorityNpcIdsByBeat["DinnerConflict_Table"], issues);
        ValidateDirectorConsumptionRole(manifest, "DinnerConflict_Table", SpringDay1CrowdDirectorConsumptionRole.Support, ExpectedSupportNpcIdsByBeat["DinnerConflict_Table"], issues);
        ValidateDirectorConsumptionRole(manifest, "DinnerConflict_Table", SpringDay1CrowdDirectorConsumptionRole.BackstagePressure, ExpectedBackstagePressureNpcIdsByBeat["DinnerConflict_Table"], issues);
        ValidateDirectorConsumptionRole(manifest, "FreeTime_NightWitness", SpringDay1CrowdDirectorConsumptionRole.Support, ExpectedSupportNpcIdsByBeat["FreeTime_NightWitness"], issues);
        ValidateDirectorConsumptionRole(manifest, "FreeTime_NightWitness", SpringDay1CrowdDirectorConsumptionRole.Priority, ExpectedPriorityNpcIdsByBeat["FreeTime_NightWitness"], issues);
        ValidateDirectorConsumptionRole(manifest, "FreeTime_NightWitness", SpringDay1CrowdDirectorConsumptionRole.BackstagePressure, ExpectedBackstagePressureNpcIdsByBeat["FreeTime_NightWitness"], issues);
        ValidateDirectorConsumptionRole(manifest, "FreeTime_NightWitness", SpringDay1CrowdDirectorConsumptionRole.Trace, ExpectedTraceNpcIdsByBeat["FreeTime_NightWitness"], issues);

        if (!manifest.TryGetEntry("102", out SpringDay1NpcCrowdManifest.Entry npc102) ||
            !npc102.IsDirectorBackstagePressureBeat("DinnerConflict_Table"))
        {
            issues.Add("102: dinner backstage pressure contract drifted");
        }
    }

    private static void ValidateDirectorConsumptionRole(
        SpringDay1NpcCrowdManifest manifest,
        string beatKey,
        SpringDay1CrowdDirectorConsumptionRole role,
        string[] expectedNpcIds,
        List<string> issues)
    {
        SpringDay1NpcCrowdManifest.Entry[] entries = manifest.GetEntriesForDirectorConsumptionRole(beatKey, role);
        string[] actualNpcIds = ExtractNpcIds(entries);
        if (!SetEquals(actualNpcIds, expectedNpcIds))
        {
            issues.Add($"{beatKey}: director consumption role drifted -> {role} | actual=[{string.Join(",", actualNpcIds)}]");
        }
    }

    private static string[] ExtractNpcIds(SpringDay1NpcCrowdManifest.Entry[] entries)
    {
        if (entries == null || entries.Length == 0)
        {
            return Array.Empty<string>();
        }

        List<string> npcIds = new List<string>();
        for (int index = 0; index < entries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = entries[index];
            if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
            {
                continue;
            }

            npcIds.Add(entry.npcId.Trim());
        }

        return npcIds.ToArray();
    }

    private static void ValidateManifestEntrySemantics(
        string npcId,
        SpringDay1NpcCrowdManifest.Entry entry,
        List<string> issues)
    {
        if (entry == null)
        {
            issues.Add($"{npcId}: manifest entry null");
            return;
        }

        if (ExpectedSceneDutiesByNpcId.TryGetValue(npcId, out SpringDay1CrowdSceneDuty[] expectedDuties) &&
            !SetEquals(entry.sceneDuties, expectedDuties))
        {
            issues.Add($"{npcId}: manifest sceneDuties drifted");
        }

        if (ExpectedSemanticAnchorsByNpcId.TryGetValue(npcId, out string[] expectedAnchors) &&
            !SetEquals(entry.semanticAnchorIds, expectedAnchors))
        {
            issues.Add($"{npcId}: manifest semanticAnchorIds drifted");
        }

        if (ExpectedGrowthIntentByNpcId.TryGetValue(npcId, out SpringDay1CrowdGrowthIntent expectedGrowthIntent) &&
            entry.growthIntent != expectedGrowthIntent)
        {
            issues.Add($"{npcId}: manifest growthIntent drifted -> {entry.growthIntent}");
        }

        if (ExpectedResidentBaselineByNpcId.TryGetValue(npcId, out SpringDay1CrowdResidentBaseline expectedResidentBaseline) &&
            entry.residentBaseline != expectedResidentBaseline)
        {
            issues.Add($"{npcId}: manifest residentBaseline drifted -> {entry.residentBaseline}");
        }

        SpringDay1NpcCrowdManifest.ResidentBeatSemantic[] residentBeatSemantics = entry.residentBeatSemantics;
        if (residentBeatSemantics == null || residentBeatSemantics.Length != ExpectedResidentBeatKeys.Length)
        {
            issues.Add($"{npcId}: manifest residentBeatSemantics count drifted");
            return;
        }

        for (int index = 0; index < ExpectedResidentBeatKeys.Length; index++)
        {
            string beatKey = ExpectedResidentBeatKeys[index];
            if (!TryFindResidentBeatSemantic(entry, beatKey, out SpringDay1NpcCrowdManifest.ResidentBeatSemantic semantic))
            {
                issues.Add($"{npcId}: missing resident beat semantic -> {beatKey}");
                continue;
            }

            if (semantic.flags == SpringDay1CrowdResidentBeatFlags.None)
            {
                issues.Add($"{npcId}: resident beat semantic has empty flags -> {beatKey}");
            }

            if (semantic.presenceLevel == SpringDay1CrowdResidentPresenceLevel.None)
            {
                issues.Add($"{npcId}: resident beat semantic has empty presenceLevel -> {beatKey}");
            }

            if (string.IsNullOrWhiteSpace(semantic.note))
            {
                issues.Add($"{npcId}: resident beat semantic note missing -> {beatKey}");
            }
        }
    }

    private static bool SetEquals<T>(T[] actual, T[] expected)
    {
        if (actual == null || expected == null)
        {
            return actual == expected;
        }

        if (actual.Length != expected.Length)
        {
            return false;
        }

        HashSet<T> values = new HashSet<T>(actual);
        for (int index = 0; index < expected.Length; index++)
        {
            if (!values.Contains(expected[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasReversePair(NPCDialogueContentProfile dialogue, string partnerNpcId)
    {
        NPCDialogueContentProfile.PairDialogueSet[] pairSets = dialogue.PairDialogueSets;
        for (int index = 0; index < pairSets.Length; index++)
        {
            NPCDialogueContentProfile.PairDialogueSet pairSet = pairSets[index];
            if (pairSet == null)
            {
                continue;
            }

            if (string.Equals(pairSet.PartnerNpcId, partnerNpcId, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryFindResidentBeatSemantic(
        SpringDay1NpcCrowdManifest.Entry entry,
        string beatKey,
        out SpringDay1NpcCrowdManifest.ResidentBeatSemantic semantic)
    {
        semantic = null;
        SpringDay1NpcCrowdManifest.ResidentBeatSemantic[] residentBeatSemantics = entry.residentBeatSemantics;
        if (residentBeatSemantics == null)
        {
            return false;
        }

        for (int index = 0; index < residentBeatSemantics.Length; index++)
        {
            SpringDay1NpcCrowdManifest.ResidentBeatSemantic candidate = residentBeatSemantics[index];
            if (candidate == null)
            {
                continue;
            }

            if (string.Equals(candidate.beatKey, beatKey, StringComparison.OrdinalIgnoreCase))
            {
                semantic = candidate;
                return true;
            }
        }

        return false;
    }

    private static void ValidatePhaseInformalFallback(
        string npcId,
        NPCDialogueContentProfile dialogue,
        List<string> issues)
    {
        if (dialogue == null)
        {
            return;
        }

        if (!ExpectedPhaseFallbackPhasesByNpcId.TryGetValue(npcId, out StoryPhase[] expectedPhases) ||
            expectedPhases == null ||
            expectedPhases.Length == 0)
        {
            return;
        }

        for (int index = 0; index < expectedPhases.Length; index++)
        {
            StoryPhase phase = expectedPhases[index];
            if (!dialogue.TryGetPhaseInformalChatSet(phase, out NPCDialogueContentProfile.PhaseInformalChatSet phaseSet) ||
                phaseSet == null)
            {
                issues.Add($"{npcId}: missing phase fallback set -> {phase}");
                continue;
            }

            if (!phaseSet.HasConversationContent())
            {
                issues.Add($"{npcId}: empty phase fallback set -> {phase}");
            }
        }
    }

    private static void ValidatePhaseNearbyCoverage(
        string npcId,
        NPCDialogueContentProfile dialogue,
        List<string> issues)
    {
        if (dialogue == null)
        {
            return;
        }

        if (!ExpectedPhaseFallbackPhasesByNpcId.TryGetValue(npcId, out StoryPhase[] expectedPhases) ||
            expectedPhases == null ||
            expectedPhases.Length == 0)
        {
            return;
        }

        for (int index = 0; index < expectedPhases.Length; index++)
        {
            StoryPhase phase = expectedPhases[index];
            if (!dialogue.TryGetPhaseNearbySet(phase, out NPCDialogueContentProfile.PhaseNearbySet phaseSet) ||
                phaseSet == null)
            {
                issues.Add($"{npcId}: missing phase nearby set -> {phase}");
                continue;
            }

            if (!HasAnyDialogueLines(phaseSet.PlayerNearbyLines))
            {
                issues.Add($"{npcId}: empty phase nearby set -> {phase}");
            }
        }
    }

    private static void ValidatePhaseSelfTalkCoverage(
        string npcId,
        NPCDialogueContentProfile dialogue,
        List<string> issues)
    {
        if (dialogue == null)
        {
            return;
        }

        if (!ExpectedPhaseFallbackPhasesByNpcId.TryGetValue(npcId, out StoryPhase[] expectedPhases) ||
            expectedPhases == null ||
            expectedPhases.Length == 0)
        {
            return;
        }

        for (int index = 0; index < expectedPhases.Length; index++)
        {
            StoryPhase phase = expectedPhases[index];
            if (!dialogue.TryGetPhaseSelfTalkSet(phase, out NPCDialogueContentProfile.PhaseSelfTalkSet phaseSet) ||
                phaseSet == null)
            {
                issues.Add($"{npcId}: missing phase selfTalk set -> {phase}");
                continue;
            }

            if (!HasAnyDialogueLines(phaseSet.SelfTalkLines))
            {
                issues.Add($"{npcId}: empty phase selfTalk set -> {phase}");
            }
        }
    }

    private static bool HasAnyDialogueLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetRuntimePlayerContext(
        out PlayerMovement playerMovement,
        out PlayerNpcChatSessionService sessionService)
    {
        playerMovement = null;
        sessionService = null;

        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[SpringDay1NpcRuntimeProbe] 仅可在 Play Mode 中执行 runtime probe。");
            return false;
        }

        playerMovement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
        if (playerMovement == null)
        {
            Debug.LogWarning("[SpringDay1NpcRuntimeProbe] 未找到 PlayerMovement。");
            return false;
        }

        sessionService = PlayerNpcChatSessionService.ResolveForPlayer(playerMovement.gameObject);
        if (!s_runtimePlayerPositionCaptured)
        {
            s_runtimeOriginalPlayerPosition = playerMovement.transform.position;
            s_runtimePlayerPositionCaptured = true;
        }

        return true;
    }

    private static RuntimeProbeNpc FindProbeNpc(string npcId)
    {
        for (int index = 0; index < RuntimeProbeNpcs.Count; index++)
        {
            RuntimeProbeNpc probeNpc = RuntimeProbeNpcs[index];
            if (probeNpc != null && string.Equals(probeNpc.NpcId, npcId, StringComparison.OrdinalIgnoreCase))
            {
                return probeNpc;
            }
        }

        return null;
    }

    private static Vector3 ResolveRuntimeProbeSpawnPosition(int index)
    {
        float x = RuntimeProbeBaseX + (index % 4) * RuntimeProbeSpacingX;
        float y = RuntimeProbeBaseY - (index / 4) * RuntimeProbeSpacingY;
        return new Vector3(x, y, 0f);
    }

    private static void PositionNonPairNpcsFarAway()
    {
        for (int index = 0; index < RuntimeProbeNpcs.Count; index++)
        {
            RuntimeProbeNpc probeNpc = RuntimeProbeNpcs[index];
            if (probeNpc?.Instance == null)
            {
                continue;
            }

            ResetProbeNpcPosition(probeNpc, new Vector2(RuntimeProbeBaseX + 30f + index * 3f, RuntimeProbeBaseY + 20f), restartRoam: false);
        }
    }

    private static void PreparePairProbePositions(RuntimeProbeNpc initiator, RuntimeProbeNpc responder)
    {
        PositionNonPairNpcsFarAway();
        ResetProbeNpcPosition(initiator, new Vector2(RuntimeProbeBaseX, RuntimeProbeBaseY), restartRoam: false);
        ResetProbeNpcPosition(responder, new Vector2(RuntimeProbeBaseX + 0.9f, RuntimeProbeBaseY), restartRoam: false);
        MovePlayerToProbePoint(new Vector2(RuntimeProbeBaseX - 8f, RuntimeProbeBaseY - 6f));
    }

    private static void ResetProbeNpcPosition(RuntimeProbeNpc probeNpc, Vector2 worldPosition, bool restartRoam)
    {
        if (probeNpc == null || probeNpc.Instance == null)
        {
            return;
        }

        probeNpc.Instance.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
        if (probeNpc.HomeAnchor != null)
        {
            probeNpc.HomeAnchor.transform.position = probeNpc.Instance.transform.position;
        }

        if (probeNpc.MotionController != null)
        {
            probeNpc.MotionController.StopMotion();
        }

        if (probeNpc.BubblePresenter != null)
        {
            probeNpc.BubblePresenter.SetInteractionPromptSuppressed(false);
            probeNpc.BubblePresenter.SetConversationChannelActive(false);
            probeNpc.BubblePresenter.HideImmediateBubble();
        }

        if (probeNpc.RoamController != null)
        {
            BreakAmbientChatLink(probeNpc);
            probeNpc.RoamController.SetHomeAnchor(probeNpc.HomeAnchor != null ? probeNpc.HomeAnchor.transform : null);
            if (restartRoam)
            {
                probeNpc.RoamController.StartRoam();
            }
            else
            {
                probeNpc.RoamController.StopRoam();
            }
        }

        Physics2D.SyncTransforms();
    }

    private static void MovePlayerNearInteractable(NPCInformalChatInteractable interactable, float gapFromBoundary)
    {
        if (interactable == null || s_runtimePlayerMovement == null)
        {
            return;
        }

        CancelPlayerNavigation();

        Rigidbody2D playerBody = s_runtimePlayerMovement.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }

        Vector2 sampleOffset = ResolvePlayerSamplePoint(s_runtimePlayerMovement) - (Vector2)s_runtimePlayerMovement.transform.position;
        Bounds targetBounds = ResolveInteractionBounds(interactable.transform);
        float sideDirection = s_runtimePlayerMovement.transform.position.x <= interactable.transform.position.x ? -1f : 1f;
        Vector2 targetSamplePoint = new Vector2(
            targetBounds.center.x + sideDirection * (targetBounds.extents.x + Mathf.Max(0.02f, gapFromBoundary)),
            targetBounds.center.y);
        MovePlayerToProbePoint(targetSamplePoint - sampleOffset);
    }

    private static void MovePlayerToProbePoint(Vector2 worldPosition)
    {
        if (s_runtimePlayerMovement == null)
        {
            return;
        }

        Rigidbody2D playerBody = s_runtimePlayerMovement.GetComponent<Rigidbody2D>();
        Vector3 target = new Vector3(worldPosition.x, worldPosition.y, s_runtimePlayerMovement.transform.position.z);
        if (playerBody != null)
        {
            playerBody.position = target;
        }
        else
        {
            s_runtimePlayerMovement.transform.position = target;
        }

        Physics2D.SyncTransforms();
    }

    private static void RestorePlayerPosition()
    {
        if (s_runtimePlayerMovement == null)
        {
            return;
        }

        CancelPlayerNavigation();
        Rigidbody2D playerBody = s_runtimePlayerMovement.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.position = s_runtimeOriginalPlayerPosition;
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }
        else
        {
            s_runtimePlayerMovement.transform.position = s_runtimeOriginalPlayerPosition;
        }

        Physics2D.SyncTransforms();
    }

    private static void CancelPlayerNavigation()
    {
        if (s_runtimePlayerMovement == null)
        {
            return;
        }

        PlayerAutoNavigator navigator = s_runtimePlayerMovement.GetComponent<PlayerAutoNavigator>();
        navigator?.ForceCancel();
    }

    private static InteractionContext BuildInteractionContext(PlayerMovement playerMovement)
    {
        return new InteractionContext
        {
            PlayerTransform = playerMovement.transform,
            PlayerPosition = ResolvePlayerSamplePoint(playerMovement)
        };
    }

    private static Vector2 ResolvePlayerSamplePoint(PlayerMovement playerMovement)
    {
        if (playerMovement == null)
        {
            return Vector2.zero;
        }

        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
        return playerCollider != null ? (Vector2)playerCollider.bounds.center : (Vector2)playerMovement.transform.position;
    }

    private static Bounds ResolveInteractionBounds(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            SpriteRenderer spriteRenderer = targetTransform.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.bounds;
            }

            Collider2D collider2D = targetTransform.GetComponentInChildren<Collider2D>();
            if (collider2D != null)
            {
                return collider2D.bounds;
            }
        }

        return new Bounds(targetTransform != null ? targetTransform.position : Vector3.zero, Vector3.one);
    }

    private static void ClearDialogueInterference()
    {
        DialogueManager dialogueManager = UnityEngine.Object.FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
        if (dialogueManager == null)
        {
            return;
        }

        int guard = 0;
        while (dialogueManager.IsDialogueActive && guard < 8)
        {
            dialogueManager.ForceCompleteOrAdvance();
            guard++;
        }
    }

    private static bool HasActiveConversation()
    {
        return s_runtimeSessionService != null && s_runtimeSessionService.HasActiveConversation;
    }

    private static void AbortActiveConversationIfNeeded()
    {
        if (s_runtimeSessionService == null || !s_runtimeSessionService.HasActiveConversation)
        {
            return;
        }

        s_runtimeSessionService.TryForceAbortForValidation(NPCInformalChatLeaveCause.TargetInvalid);
    }

    private static bool ForceAmbientPairDialogue(RuntimeProbeNpc initiator, RuntimeProbeNpc responder)
    {
        if (initiator?.RoamController == null || responder?.RoamController == null)
        {
            return false;
        }

        BreakAmbientChatLink(initiator);
        BreakAmbientChatLink(responder);
        SetAmbientChatState(initiator.RoamController, "LongPause");
        SetAmbientChatState(responder.RoamController, "LongPause");
        SetPrivateField(initiator.RoamController, "ambientChatChance", 1f);
        SetPrivateField(responder.RoamController, "ambientChatChance", 1f);
        SetPrivateField(initiator.RoamController, "enableAmbientChat", true);
        SetPrivateField(responder.RoamController, "enableAmbientChat", true);

        MethodInfo tryStartAmbientChat = typeof(NPCAutoRoamController).GetMethod(
            "TryStartAmbientChat",
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new[] { typeof(float) },
            null);
        if (tryStartAmbientChat == null)
        {
            RuntimeProbeIssues.Add("pair probe failed: TryStartAmbientChat reflection missing");
            return false;
        }

        object result = tryStartAmbientChat.Invoke(initiator.RoamController, new object[] { RuntimeProbePairDialogueDurationSeconds });
        return result is bool boolResult && boolResult;
    }

    private static void BreakAmbientChatLink(RuntimeProbeNpc probeNpc)
    {
        if (probeNpc?.RoamController == null)
        {
            return;
        }

        MethodInfo breakMethod = typeof(NPCAutoRoamController).GetMethod(
            "BreakAmbientChatLink",
            BindingFlags.Instance | BindingFlags.NonPublic);
        breakMethod?.Invoke(probeNpc.RoamController, new object[] { true });
    }

    private static void ClearPairProbeBubbleLocks()
    {
        FieldInfo ownerField = typeof(NPCBubblePresenter).GetField(
            "s_conversationChannelOwner",
            BindingFlags.Static | BindingFlags.NonPublic);
        NPCBubblePresenter owner = ownerField?.GetValue(null) as NPCBubblePresenter;
        if (owner == null)
        {
            return;
        }

        owner.SetConversationChannelActive(false);
        owner.HideImmediateBubble();
        ownerField?.SetValue(null, null);
    }

    private static void SetAmbientChatState(NPCAutoRoamController roamController, string stateName)
    {
        if (roamController == null)
        {
            return;
        }

        Type roamStateType = typeof(NPCAutoRoamController).GetNestedType("RoamState", BindingFlags.NonPublic);
        if (roamStateType == null)
        {
            return;
        }

        object stateValue = Enum.Parse(roamStateType, stateName);
        SetPrivateField(roamController, "state", stateValue);
        SetPrivateField(roamController, "stateTimer", 3.2f);
        SetPrivateField(roamController, "chatPartner", null);
    }

    private static string DescribeBubblePresenterState(NPCBubblePresenter presenter)
    {
        if (presenter == null)
        {
            return "missing";
        }

        bool suppressed = GetPrivateBoolField(presenter, "_suppressAmbientWhilePromptFocused");
        object channel = GetPrivateFieldValue(presenter, "_channelPriority");
        bool canCreate = InvokePrivateBool(presenter, "CanCreateBubbleUiInCurrentContext");
        bool hasCanvas = GetPrivateFieldValue(presenter, "_canvas") != null;
        bool hasText = GetPrivateFieldValue(presenter, "_bubbleText") != null;
        return $"visible={presenter.IsBubbleVisible},text=\"{presenter.CurrentBubbleText}\",last=\"{presenter.LastPresentedText}\",suppressed={suppressed},channel={channel ?? "null"},canCreate={canCreate},hasCanvas={hasCanvas},hasText={hasText}";
    }

    private static string DescribeConversationBubbleOwner()
    {
        FieldInfo ownerField = typeof(NPCBubblePresenter).GetField(
            "s_conversationChannelOwner",
            BindingFlags.Static | BindingFlags.NonPublic);
        NPCBubblePresenter owner = ownerField?.GetValue(null) as NPCBubblePresenter;
        return owner != null ? owner.name : "none";
    }

    private static string DescribeResolvedAmbientLines(RuntimeProbeNpc speaker, RuntimeProbeNpc partner, bool initiator)
    {
        if (speaker?.RoamController == null)
        {
            return "missing";
        }

        MethodInfo resolveMethod = typeof(NPCAutoRoamController).GetMethod(
            "ResolveAmbientChatLinesForPartner",
            BindingFlags.Instance | BindingFlags.NonPublic);
        if (resolveMethod == null)
        {
            return "reflection-missing";
        }

        object result = resolveMethod.Invoke(speaker.RoamController, new object[] { partner?.RoamController, initiator });
        if (result is not string[] lines)
        {
            return "null";
        }

        int nonEmptyCount = 0;
        string firstLine = string.Empty;
        for (int index = 0; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
            {
                continue;
            }

            nonEmptyCount++;
            if (string.IsNullOrEmpty(firstLine))
            {
                firstLine = lines[index].Trim();
            }
        }

        return $"count={nonEmptyCount},first=\"{firstLine}\"";
    }

    private static string DescribeResolvedSelfTalkLines(RuntimeProbeNpc speaker, StoryPhase storyPhase)
    {
        if (speaker?.RoamController?.RoamProfile == null)
        {
            return "missing";
        }

        string[] lines = speaker.RoamController.RoamProfile.GetSelfTalkLines(storyPhase);
        if (lines == null || lines.Length == 0)
        {
            return "count=0,first=\"\"";
        }

        int nonEmptyCount = 0;
        string firstLine = string.Empty;
        for (int index = 0; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
            {
                continue;
            }

            nonEmptyCount++;
            if (string.IsNullOrEmpty(firstLine))
            {
                firstLine = lines[index].Trim();
            }
        }

        return $"count={nonEmptyCount},first=\"{firstLine}\"";
    }

    private static bool IsExpectedPairLine(RuntimeProbeNpc speaker, RuntimeProbeNpc partner, string bubbleText, bool initiator)
    {
        if (speaker?.RoamController?.RoamProfile == null || string.IsNullOrWhiteSpace(bubbleText))
        {
            return false;
        }

        string normalizedBubbleText = NormalizeBubbleComparisonText(bubbleText);
        string[] expectedLines = speaker.RoamController.RoamProfile.GetAmbientChatLines(partner.NpcId, initiator);
        for (int index = 0; index < expectedLines.Length; index++)
        {
            if (string.Equals(
                    NormalizeBubbleComparisonText(expectedLines[index]),
                    normalizedBubbleText,
                    StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsExpectedSelfTalkLine(RuntimeProbeNpc speaker, StoryPhase storyPhase, string bubbleText)
    {
        if (speaker?.RoamController?.RoamProfile == null || string.IsNullOrWhiteSpace(bubbleText))
        {
            return false;
        }

        string normalizedBubbleText = NormalizeBubbleComparisonText(bubbleText);
        string[] expectedLines = speaker.RoamController.RoamProfile.GetSelfTalkLines(storyPhase);
        for (int index = 0; index < expectedLines.Length; index++)
        {
            if (string.Equals(
                    NormalizeBubbleComparisonText(expectedLines[index]),
                    normalizedBubbleText,
                    StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizeBubbleComparisonText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return text
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();
    }

    private static Transform GetBoundHomeAnchor(NPCAutoRoamController roamController)
    {
        return GetPrivateField<Transform>(roamController, "homeAnchor");
    }

    private static T GetPrivateField<T>(object target, string fieldName) where T : class
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field?.GetValue(target) as T;
    }

    private static bool GetPrivateBoolField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        object value = field?.GetValue(target);
        return value is bool boolValue && boolValue;
    }

    private static bool InvokePrivateBool(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            return false;
        }

        object result = method.Invoke(target, null);
        return result is bool boolValue && boolValue;
    }

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field?.GetValue(target);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(target, value);
    }

    private static void EnsureRuntimeRunInBackground()
    {
        if (s_runtimeRunInBackgroundOverridden)
        {
            return;
        }

        s_runtimePreviousRunInBackground = Application.runInBackground;
        Application.runInBackground = true;
        s_runtimeRunInBackgroundOverridden = true;
    }

    private static void RestoreRuntimeRunInBackground()
    {
        if (!s_runtimeRunInBackgroundOverridden)
        {
            return;
        }

        Application.runInBackground = s_runtimePreviousRunInBackground;
        s_runtimeRunInBackgroundOverridden = false;
    }

    private static T LoadNpcAsset<T>(string npcId, string fileNameSuffix) where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { DataRoot });
        for (int index = 0; index < guids.Length; index++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[index]);
            if (!FileNameMatchesNpc(path, npcId, fileNameSuffix))
            {
                continue;
            }

            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return null;
    }

    private static Dictionary<string, NPCDialogueContentProfile> BuildDialogueLookup()
    {
        Dictionary<string, NPCDialogueContentProfile> dialogueByNpcId = new Dictionary<string, NPCDialogueContentProfile>(StringComparer.OrdinalIgnoreCase);
        for (int index = 0; index < ExpectedNpcIds.Length; index++)
        {
            string npcId = ExpectedNpcIds[index];
            NPCDialogueContentProfile dialogue = LoadNpcAsset<NPCDialogueContentProfile>(npcId, "DialogueContent.asset");
            if (dialogue != null)
            {
                dialogueByNpcId[npcId] = dialogue;
            }
        }

        return dialogueByNpcId;
    }

    private static int CountNpcAssetsPresent<T>(string fileNameSuffix) where T : UnityEngine.Object
    {
        int count = 0;
        for (int index = 0; index < ExpectedNpcIds.Length; index++)
        {
            if (LoadNpcAsset<T>(ExpectedNpcIds[index], fileNameSuffix) != null)
            {
                count++;
            }
        }

        return count;
    }

    private static T LoadSingleAsset<T>(string rootPath) where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { rootPath });
        if (guids.Length != 1)
        {
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static bool FileNameMatchesNpc(string path, string npcId, string fileNameSuffix)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(npcId) || string.IsNullOrWhiteSpace(fileNameSuffix))
        {
            return false;
        }

        string fileName = System.IO.Path.GetFileName(path);
        string prefix = $"NPC_{npcId}_";
        return fileName != null &&
               fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
               fileName.EndsWith(fileNameSuffix, StringComparison.OrdinalIgnoreCase);
    }
}
#endif
