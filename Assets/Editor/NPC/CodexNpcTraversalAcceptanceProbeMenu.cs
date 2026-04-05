#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sunset.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

internal static class CodexNpcTraversalAcceptanceProbeMenu
{
    private const string MenuPath = "Tools/Codex/NPC/Run Traversal Acceptance Probe";
    private const string NaturalBridgeMenuPath = "Tools/Codex/NPC/Run Natural Roam Bridge Probe";
    private const string RequiredSceneName = "Primary";
    private const double BridgeTimeoutSeconds = 12.0d;
    private const double EdgeTimeoutSeconds = 8.0d;
    private const float BridgeReachTolerance = 0.45f;
    private const float BridgeSupportRadius = 0.18f;

    private static readonly Vector2 BridgeStart = new Vector2(-13.6f, 0.3f);
    private static readonly Vector2 BridgeTarget = new Vector2(-8.8f, 1.7f);
    private static readonly Vector2 EdgeStart = new Vector2(6.5f, 2.1f);
    private static readonly Vector2 EdgeOutsideTarget = new Vector2(9.6f, 2.1f);

    private enum ProbePhase
    {
        Idle = 0,
        Bridge = 1,
        Edge = 2
    }

    private sealed class ProbeNpcContext
    {
        public string Name;
        public NPCAutoRoamController Controller;
        public Rigidbody2D Rigidbody;
        public Collider2D Collider;
        public Transform HomeAnchor;
    }

    private static ProbePhase s_phase;
    private static double s_phaseStartedAt;
    private static bool s_bridgeSawWalkableOverride;
    private static bool s_naturalRoamBridgeMode;
    private static ProbeNpcContext s_bridgeNpc;
    private static ProbeNpcContext s_edgeNpc;
    private static NavGrid2D s_navGrid;
    private static TilemapCollider2D s_waterCollider;
    private static readonly List<Behaviour> s_temporarilyDisabledBehaviours = new List<Behaviour>();

    [MenuItem(MenuPath)]
    private static void RunProbe()
    {
        RunProbeInternal(naturalRoamBridgeMode: false);
    }

    [MenuItem(NaturalBridgeMenuPath)]
    private static void RunNaturalRoamBridgeProbe()
    {
        RunProbeInternal(naturalRoamBridgeMode: true);
    }

    private static void RunProbeInternal(bool naturalRoamBridgeMode)
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[CodexNpcTraversalAcceptance] 请先进入 Play Mode。");
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (!string.Equals(activeScene.name, RequiredSceneName, StringComparison.Ordinal))
        {
            Debug.LogWarning(
                $"[CodexNpcTraversalAcceptance] 当前场景={activeScene.name}；这条 probe 只在 {RequiredSceneName} 有效。");
            return;
        }

        Cleanup(logSummary: false);
        s_naturalRoamBridgeMode = naturalRoamBridgeMode;

        s_navGrid = UnityEngine.Object.FindFirstObjectByType<NavGrid2D>();
        s_waterCollider = FindWaterCollider();
        s_bridgeNpc = BuildContext("002");
        s_edgeNpc = naturalRoamBridgeMode ? null : BuildContext("003");
        if (!TryValidateDependencies(naturalRoamBridgeMode, out string missingReason))
        {
            Debug.LogError($"[CodexNpcTraversalAcceptance] 缺少运行时依赖：{missingReason}。");
            Cleanup(logSummary: false);
            return;
        }

        SuppressInterferingControllers();
        StopActiveDialogueIfNeeded();

        Bounds worldBounds = s_navGrid.GetWorldBounds();
        Debug.Log($"[CodexNpcTraversalAcceptance] world_bounds center={worldBounds.center} size={worldBounds.size}");
        StartBridgePhase();
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying)
        {
            Cleanup(logSummary: false);
            return;
        }

        switch (s_phase)
        {
            case ProbePhase.Bridge:
                TickBridgePhase();
                break;

            case ProbePhase.Edge:
                TickEdgePhase();
                break;
        }
    }

    private static void StartBridgePhase()
    {
        s_phase = ProbePhase.Bridge;
        s_phaseStartedAt = EditorApplication.timeSinceStartup;
        s_bridgeSawWalkableOverride = false;
        ConfigureNpcForProbe(s_bridgeNpc, BridgeStart, BridgeTarget);
        if (s_naturalRoamBridgeMode)
        {
            if (!DispatchMoveTo(s_bridgeNpc, BridgeTarget, "bridge_natural"))
            {
                return;
            }
        }
        else if (!DispatchMoveTo(s_bridgeNpc, BridgeTarget, "bridge"))
        {
            return;
        }

        Debug.Log(s_naturalRoamBridgeMode
            ? $"[CodexNpcTraversalAcceptance] bridge_natural_probe_start npc={s_bridgeNpc.Name} requestedStart={BridgeStart} actualStart={GetNpcPosition(s_bridgeNpc)} roamCenter={BridgeTarget}"
            : $"[CodexNpcTraversalAcceptance] bridge_probe_start npc={s_bridgeNpc.Name} requestedStart={BridgeStart} actualStart={GetNpcPosition(s_bridgeNpc)} target={BridgeTarget}");
    }

    private static void TickBridgePhase()
    {
        Vector2 position = GetNpcPosition(s_bridgeNpc);
        s_bridgeNpc.Controller.GetNavigationProbePoints(
            position,
            out Vector2 centerProbe,
            out Vector2 leftProbe,
            out Vector2 rightProbe);
        bool inBounds = s_navGrid.IsWithinWorldBounds(position);
        bool bridgeSupported = HasBridgeSupport(centerProbe, leftProbe, rightProbe);
        bool inWater = s_waterCollider != null &&
                       (s_waterCollider.OverlapPoint(centerProbe) ||
                        s_waterCollider.OverlapPoint(leftProbe) ||
                        s_waterCollider.OverlapPoint(rightProbe));
        s_bridgeSawWalkableOverride |= bridgeSupported;

        if (!inBounds)
        {
            Fail($"bridge_probe_fail reason=out_of_bounds position={position}");
            return;
        }

        if (inWater && !bridgeSupported)
        {
            Fail($"bridge_probe_fail reason=entered_water_off_bridge position={position} center={centerProbe}");
            return;
        }

        if (Vector2.Distance(position, BridgeTarget) <= BridgeReachTolerance)
        {
            Debug.Log(s_naturalRoamBridgeMode
                ? $"[CodexNpcTraversalAcceptance] bridge_natural_probe_pass npc={s_bridgeNpc.Name} final={position} sawBridgeSupport={s_bridgeSawWalkableOverride} inWater={inWater} state={s_bridgeNpc.Controller.DebugState}"
                : $"[CodexNpcTraversalAcceptance] bridge_probe_pass npc={s_bridgeNpc.Name} final={position} sawBridgeSupport={s_bridgeSawWalkableOverride} inWater={inWater}");
            if (s_naturalRoamBridgeMode)
            {
                Pass();
            }
            else
            {
                StartEdgePhase();
            }
            return;
        }

        if (EditorApplication.timeSinceStartup - s_phaseStartedAt >= BridgeTimeoutSeconds)
        {
            Debug.Log($"[CodexNpcTraversalAcceptance] runtime {DescribeProbeRuntime(s_bridgeNpc)}");
            Fail(
                $"{(s_naturalRoamBridgeMode ? "bridge_natural_probe_fail" : "bridge_probe_fail")} reason=timeout position={position} pathCount={s_bridgeNpc.Controller.DebugPathPointCount} " +
                $"state={s_bridgeNpc.Controller.DebugState} sawBridgeSupport={s_bridgeSawWalkableOverride} progress={s_bridgeNpc.Controller.DebugLastProgressDistance:F3} " +
                $"{DescribeProbeRuntime(s_bridgeNpc)}");
        }
    }

    private static void StartEdgePhase()
    {
        s_phase = ProbePhase.Edge;
        s_phaseStartedAt = EditorApplication.timeSinceStartup;
        ConfigureNpcForProbe(s_edgeNpc, EdgeStart, EdgeStart);
        if (!DispatchMoveTo(s_edgeNpc, EdgeOutsideTarget, "edge"))
        {
            return;
        }

        Debug.Log(
            $"[CodexNpcTraversalAcceptance] edge_probe_start npc={s_edgeNpc.Name} requestedStart={EdgeStart} " +
            $"actualStart={GetNpcPosition(s_edgeNpc)} target={EdgeOutsideTarget}");
    }

    private static void TickEdgePhase()
    {
        Vector2 position = GetNpcPosition(s_edgeNpc);
        bool inBounds = s_navGrid.IsWithinWorldBounds(position);
        if (!inBounds)
        {
            Fail($"edge_probe_fail reason=left_world_bounds position={position}");
            return;
        }

        if (position.x > 8.0f)
        {
            Debug.Log(
                $"[CodexNpcTraversalAcceptance] edge_probe_pass npc={s_edgeNpc.Name} position={position} inBounds={inBounds}");
            Pass();
            return;
        }

        if (EditorApplication.timeSinceStartup - s_phaseStartedAt >= EdgeTimeoutSeconds)
        {
            Fail(
                $"edge_probe_fail reason=timeout position={position} pathCount={s_edgeNpc.Controller.DebugPathPointCount} " +
                $"state={s_edgeNpc.Controller.DebugState}");
        }
    }

    private static void Pass()
    {
        Debug.Log(s_naturalRoamBridgeMode
            ? "[CodexNpcTraversalAcceptance] PASS natural-roam-bridge"
            : "[CodexNpcTraversalAcceptance] PASS bridge+water+edge");
        Cleanup(logSummary: false);
    }

    private static void Fail(string message)
    {
        Debug.LogError($"[CodexNpcTraversalAcceptance] {message}");
        Cleanup(logSummary: false);
    }

    private static void Cleanup(bool logSummary)
    {
        EditorApplication.update -= Tick;
        s_phase = ProbePhase.Idle;
        s_phaseStartedAt = 0d;
        s_bridgeSawWalkableOverride = false;
        s_naturalRoamBridgeMode = false;
        s_bridgeNpc = null;
        s_edgeNpc = null;
        s_navGrid = null;
        s_waterCollider = null;
        RestoreSuppressedControllers();
        if (logSummary)
        {
            Debug.Log("[CodexNpcTraversalAcceptance] cleanup");
        }
    }

    private static ProbeNpcContext BuildContext(string npcName)
    {
        NPCAutoRoamController[] controllers = UnityEngine.Object.FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);
        for (int index = 0; index < controllers.Length; index++)
        {
            NPCAutoRoamController controller = controllers[index];
            if (controller == null || !string.Equals(controller.name, npcName, StringComparison.Ordinal))
            {
                continue;
            }

            return new ProbeNpcContext
            {
                Name = npcName,
                Controller = controller,
                Rigidbody = controller.GetComponent<Rigidbody2D>(),
                Collider = controller.GetComponent<Collider2D>(),
                HomeAnchor = FindNamedTransform($"{npcName}_HomeAnchor")
            };
        }

        return null;
    }

    private static void ConfigureNpcForProbe(ProbeNpcContext context, Vector2 startPosition, Vector2 homeAnchorPosition)
    {
        if (context == null)
        {
            return;
        }

        context.Controller.StopRoam();
        context.Controller.enabled = true;
        Vector2 resolvedStartPosition = ResolveStableStartPosition(context, startPosition);
        if (context.Rigidbody != null)
        {
            context.Rigidbody.position = resolvedStartPosition;
            context.Rigidbody.linearVelocity = Vector2.zero;
            context.Rigidbody.rotation = 0f;
        }

        context.Controller.transform.position = resolvedStartPosition;
        if (context.HomeAnchor != null)
        {
            context.HomeAnchor.position = homeAnchorPosition;
        }

        SetMember(context.Controller, "activityRadius", 0.5f);
        SetMember(context.Controller, "minimumMoveDistance", 0.05f);
        SetMember(context.Controller, "shortPauseMin", 0.05f);
        SetMember(context.Controller, "shortPauseMax", 0.05f);
        SetMember(context.Controller, "enableAmbientChat", false);
    }

    private static Vector2 ResolveStableStartPosition(ProbeNpcContext context, Vector2 requestedStartPosition)
    {
        if (context == null || s_navGrid == null)
        {
            return requestedStartPosition;
        }

        if (s_navGrid.TryFindNearestWalkable(requestedStartPosition, out Vector2 walkableStart, context.Collider))
        {
            return walkableStart;
        }

        return requestedStartPosition;
    }

    private static bool DispatchMoveTo(ProbeNpcContext context, Vector2 targetPosition, string probeLabel)
    {
        if (context == null || context.Controller == null)
        {
            Fail($"{probeLabel}_probe_fail reason=missing_context");
            return false;
        }

        if (context.Controller.DebugMoveTo(targetPosition))
        {
            return true;
        }

        Vector2 currentPosition = GetNpcPosition(context);
        Fail(
            $"{probeLabel}_probe_fail reason=dispatch_failed start={currentPosition} target={targetPosition} " +
            $"pathCount={context.Controller.DebugPathPointCount} state={context.Controller.DebugState}");
        return false;
    }

    private static Vector2 GetNpcPosition(ProbeNpcContext context)
    {
        if (context == null)
        {
            return Vector2.zero;
        }

        if (context.Rigidbody != null)
        {
            return context.Rigidbody.position;
        }

        return context.Controller != null
            ? (Vector2)context.Controller.transform.position
            : Vector2.zero;
    }

    private static bool HasBridgeSupport(Vector2 centerProbe, Vector2 leftProbe, Vector2 rightProbe)
    {
        if (s_navGrid == null)
        {
            return false;
        }

        return s_navGrid.HasWalkableOverrideAt(centerProbe, BridgeSupportRadius) ||
               s_navGrid.HasWalkableOverrideAt(leftProbe, BridgeSupportRadius) ||
               s_navGrid.HasWalkableOverrideAt(rightProbe, BridgeSupportRadius);
    }

    private static bool TryValidateDependencies(bool naturalRoamBridgeMode, out string missingReason)
    {
        System.Collections.Generic.List<string> missing = new System.Collections.Generic.List<string>();
        if (s_navGrid == null)
        {
            missing.Add("NavGrid");
        }

        if (s_waterCollider == null)
        {
            missing.Add("Water");
        }

        if (s_bridgeNpc == null)
        {
            missing.Add("NPC 002");
        }

        if (!naturalRoamBridgeMode && s_edgeNpc == null)
        {
            missing.Add("NPC 003");
        }

        missingReason = string.Join("/", missing);
        return missing.Count == 0;
    }

    private static TilemapCollider2D FindWaterCollider()
    {
        TilemapCollider2D exact = FindNamedComponent<TilemapCollider2D>("Layer 1 - Water");
        if (exact != null)
        {
            return exact;
        }

        TilemapCollider2D[] colliders = UnityEngine.Object.FindObjectsByType<TilemapCollider2D>(FindObjectsSortMode.None);
        for (int index = 0; index < colliders.Length; index++)
        {
            TilemapCollider2D collider = colliders[index];
            if (collider == null)
            {
                continue;
            }

            string objectName = collider.name ?? string.Empty;
            if (objectName.IndexOf("Water", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return collider;
            }
        }

        return null;
    }

    private static void StopActiveDialogueIfNeeded()
    {
        DialogueManager manager = DialogueManager.Instance;
        if (manager == null || !manager.IsDialogueActive)
        {
            return;
        }

        manager.StopDialogue();
        Debug.Log("[CodexNpcTraversalAcceptance] 已停止活动对话，避免 NPC probe 被全局 dialogue freeze 冻住。");
    }

    private static string DescribeProbeRuntime(ProbeNpcContext context)
    {
        if (context == null)
        {
            return "runtime=n/a";
        }

        Vector2 rbPosition = context.Rigidbody != null ? context.Rigidbody.position : Vector2.zero;
        Vector2 rbVelocity = context.Rigidbody != null ? context.Rigidbody.linearVelocity : Vector2.zero;
        bool rbSimulated = context.Rigidbody != null && context.Rigidbody.simulated;
        string bodyType = context.Rigidbody != null ? context.Rigidbody.bodyType.ToString() : "None";
        Vector2 motionVelocity = context.Controller != null && context.Controller.GetComponent<NPCMotionController>() != null
            ? context.Controller.GetComponent<NPCMotionController>().CurrentVelocity
            : Vector2.zero;

        return
            $"controllerEnabled={(context.Controller != null && context.Controller.enabled)} " +
            $"gameObjectActive={(context.Controller != null && context.Controller.gameObject.activeInHierarchy)} " +
            $"rbSimulated={rbSimulated} bodyType={bodyType} rbPos={rbPosition} rbVel={rbVelocity} " +
            $"motionVel={motionVelocity} timeScale={Time.timeScale:F2}";
    }

    private static void SuppressInterferingControllers()
    {
        RestoreSuppressedControllers();

        SpringDay1NpcCrowdDirector[] crowdDirectors = UnityEngine.Object.FindObjectsByType<SpringDay1NpcCrowdDirector>(FindObjectsSortMode.None);
        for (int index = 0; index < crowdDirectors.Length; index++)
        {
            DisableBehaviourForProbe(crowdDirectors[index]);
        }

        SuppressNpcInterferers(s_bridgeNpc);
        SuppressNpcInterferers(s_edgeNpc);
    }

    private static void SuppressNpcInterferers(ProbeNpcContext context)
    {
        if (context?.Controller == null)
        {
            return;
        }

        GameObject npcObject = context.Controller.gameObject;
        DisableBehaviourForProbe(npcObject.GetComponent<SpringDay1DirectorStagingPlayback>());
        DisableBehaviourForProbe(npcObject.GetComponent<SpringDay1DirectorNpcTakeover>());
        DisableBehaviourForProbe(npcObject.GetComponent<SpringDay1DirectorStagingRehearsalDriver>());

        Behaviour[] behaviours = npcObject.GetComponents<Behaviour>();
        for (int index = 0; index < behaviours.Length; index++)
        {
            Behaviour behaviour = behaviours[index];
            if (behaviour == null ||
                !behaviour.enabled ||
                behaviour == context.Controller ||
                behaviour is NPCMotionController ||
                behaviour is NPCAnimController ||
                behaviour is Animator)
            {
                continue;
            }

            DisableBehaviourForProbe(behaviour);
        }
    }

    private static void DisableBehaviourForProbe(Behaviour behaviour)
    {
        if (behaviour == null || !behaviour.enabled)
        {
            return;
        }

        behaviour.enabled = false;
        s_temporarilyDisabledBehaviours.Add(behaviour);
    }

    private static void RestoreSuppressedControllers()
    {
        for (int index = s_temporarilyDisabledBehaviours.Count - 1; index >= 0; index--)
        {
            Behaviour behaviour = s_temporarilyDisabledBehaviours[index];
            if (behaviour != null)
            {
                behaviour.enabled = true;
            }
        }

        s_temporarilyDisabledBehaviours.Clear();
    }

    private static T FindNamedComponent<T>(string objectName) where T : Component
    {
        T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
        for (int index = 0; index < components.Length; index++)
        {
            T component = components[index];
            if (component != null && string.Equals(component.name, objectName, StringComparison.Ordinal))
            {
                return component;
            }
        }

        return null;
    }

    private static Transform FindNamedTransform(string objectName)
    {
        Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        for (int index = 0; index < transforms.Length; index++)
        {
            Transform transform = transforms[index];
            if (transform != null && string.Equals(transform.name, objectName, StringComparison.Ordinal))
            {
                return transform;
            }
        }

        return null;
    }

    private static void SetMember(object target, string memberName, object value)
    {
        if (target == null || string.IsNullOrWhiteSpace(memberName))
        {
            return;
        }

        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Type type = target.GetType();
        FieldInfo field = type.GetField(memberName, Flags);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = type.GetProperty(memberName, Flags);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }
}
#endif
