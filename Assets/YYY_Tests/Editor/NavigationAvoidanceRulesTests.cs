using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class NavigationAvoidanceRulesTests
{
    [Test]
    public void AutoNavigationPlayer_ShouldYield_ToMovingNpc()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type rulesType = ResolveTypeOrFail("NavigationAvoidanceRules");

        object player = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "Player",
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(1f, 0f),
            Vector2.left,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        bool playerShouldYield = InvokeStaticBool(rulesType, "ShouldYield", player, npc);
        bool npcShouldYield = InvokeStaticBool(rulesType, "ShouldYield", npc, player);

        Assert.That(playerShouldYield, Is.True);
        Assert.That(npcShouldYield, Is.False);
    }

    [Test]
    public void Solver_ShouldProduceLateralOrBackwardBias_WhenPlayerFacesMovingNpc()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object player = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "Player",
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(0.7f, 0.05f),
            Vector2.zero,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(npc, 0);

        object result = InvokeStatic(solverType, "Solve", player, Vector2.right, 1f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(adjustedDirection.x, Is.LessThan(0.99f));
        Assert.That(Mathf.Abs(adjustedDirection.y), Is.GreaterThan(0.01f));
    }

    [Test]
    public void CloseRangeConstraint_ShouldClampForwardPush_WhenBlockerIsTooClose()
    {
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");
        Type avoidanceResultType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver+AvoidanceResult");

        object avoidance = Activator.CreateInstance(
            avoidanceResultType,
            Vector2.right,
            1f,
            true,
            false,
            2,
            0.2f,
            new Vector2(0.62f, 0f),
            0.35f,
            Vector2.up);

        object constraint = InvokeStatic(
            solverType,
            "ApplyCloseRangeConstraint",
            Vector2.zero,
            Vector2.right,
            1f,
            0.35f,
            0.05f,
            avoidance);

        Assert.IsNotNull(constraint);

        bool applied = (bool)GetFieldOrProperty(constraint, "Applied");
        bool hardBlocked = (bool)GetFieldOrProperty(constraint, "HardBlocked");
        Vector2 constrainedDirection = (Vector2)GetFieldOrProperty(constraint, "ConstrainedDirection");
        float constrainedSpeedScale = (float)GetFieldOrProperty(constraint, "SpeedScale");

        Assert.That(applied, Is.True);
        Assert.That(hardBlocked, Is.True);
        Assert.That(constrainedDirection.x, Is.LessThan(0.95f));
        Assert.That(Mathf.Abs(constrainedDirection.y), Is.GreaterThan(0.01f));
        Assert.That(constrainedSpeedScale, Is.LessThan(0.2f));
    }

    [Test]
    public void CloseRangeConstraint_ShouldForceSeparation_WhenAlreadyOverlappingButNotFacingBlocker()
    {
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");
        Type avoidanceResultType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver+AvoidanceResult");

        Vector2 blockerPosition = new Vector2(0.52f, 0.1f);
        object avoidance = Activator.CreateInstance(
            avoidanceResultType,
            Vector2.down,
            0.05f,
            true,
            true,
            2,
            0.07f,
            blockerPosition,
            0.35f,
            Vector2.down);

        object constraint = InvokeStatic(
            solverType,
            "ApplyCloseRangeConstraint",
            Vector2.zero,
            Vector2.down,
            0.05f,
            0.35f,
            0.05f,
            avoidance);

        Assert.IsNotNull(constraint);

        bool applied = (bool)GetFieldOrProperty(constraint, "Applied");
        bool hardBlocked = (bool)GetFieldOrProperty(constraint, "HardBlocked");
        Vector2 constrainedDirection = (Vector2)GetFieldOrProperty(constraint, "ConstrainedDirection");
        float constrainedSpeedScale = (float)GetFieldOrProperty(constraint, "SpeedScale");
        float clearance = (float)GetFieldOrProperty(constraint, "Clearance");

        Vector2 separationDirection = (-blockerPosition).normalized;

        Assert.That(clearance, Is.LessThan(0f));
        Assert.That(applied, Is.True);
        Assert.That(hardBlocked, Is.False);
        Assert.That(Vector2.Dot(constrainedDirection.normalized, separationDirection), Is.GreaterThan(0.25f));
        Assert.That(constrainedSpeedScale, Is.InRange(0.03f, 0.08f));
    }

    [Test]
    public void CloseRangeConstraint_ShouldPreferMeasuredFootprintClearance_WhenAvailable()
    {
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");
        Type avoidanceResultType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver+AvoidanceResult");

        object avoidance = Activator.CreateInstance(
            avoidanceResultType,
            Vector2.right,
            1f,
            true,
            false,
            2,
            0.45f,
            new Vector2(1.0f, 0f),
            0.6f,
            Vector2.up);

        object constraint = InvokeStatic(
            solverType,
            "ApplyCloseRangeConstraint",
            Vector2.zero,
            Vector2.right,
            1f,
            0.35f,
            0.05f,
            avoidance,
            0.22f);

        Assert.IsNotNull(constraint);

        bool applied = (bool)GetFieldOrProperty(constraint, "Applied");
        float clearance = (float)GetFieldOrProperty(constraint, "Clearance");

        Assert.That(applied, Is.False);
        Assert.That(clearance, Is.EqualTo(0.22f).Within(0.001f));
    }

    [Test]
    public void Solver_ShouldStillTrackBlockingAgent_WhenNpcKeepsPriorityButPlayerIsAlreadyNearContact()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "NPC",
            Vector2.zero,
            Vector2.right,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        object player = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "Player",
            new Vector2(0.62f, 0f),
            Vector2.zero,
            0.35f,
            0.6f,
            100,
            false,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(player, 0);

        object result = InvokeStatic(solverType, "Solve", npc, Vector2.right, 1f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.True);
    }

    [Test]
    public void Solver_ShouldEscalateSleepingBlocker_EarlierForMovingNpc()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object npc = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "NPC",
            Vector2.zero,
            Vector2.right,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        object sleepingPlayer = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "Player",
            new Vector2(1.65f, 0.14f),
            Vector2.zero,
            0.35f,
            0.6f,
            100,
            false,
            true,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(sleepingPlayer, 0);

        object result = InvokeStatic(solverType, "Solve", npc, Vector2.right, 2f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");
        float speedScale = (float)GetFieldOrProperty(result, "SpeedScale");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.True);
        Assert.That(speedScale, Is.LessThan(0.8f));
        Assert.That(Mathf.Abs(adjustedDirection.y), Is.GreaterThan(0.01f));
    }

    [Test]
    public void Solver_ShouldKeepHoldCourse_ForNonYieldingNpcPeer_WhenOtherNpcTemporarilyStops()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object leadingNpc = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "NPC",
            Vector2.zero,
            Vector2.right,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        object yieldingNpc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(0.98f, 0.02f),
            Vector2.zero,
            0.35f,
            0.6f,
            50,
            false,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(yieldingNpc, 0);

        object result = InvokeStatic(solverType, "Solve", leadingNpc, Vector2.right, 1.5f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");
        float speedScale = (float)GetFieldOrProperty(result, "SpeedScale");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.False);
        Assert.That(speedScale, Is.GreaterThan(0.7f));
        Assert.That(adjustedDirection.x, Is.GreaterThan(0.9f));
    }

    [Test]
    public void NPCAutoRoamController_ShouldPreferRequestedDestination_WhenRecoveringFromDetour()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_DetourRecoveryTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            Vector2 requestedDestination = new Vector2(-7.30f, 5.35f);
            Vector2 driftedActualDestination = new Vector2(-7.25f, 5.75f);

            SetFieldOrProperty(controller, "requestedDestination", requestedDestination);
            SetFieldOrProperty(controller, "hasRequestedDestination", true);
            SetFieldOrProperty(controller, "currentDestination", driftedActualDestination);

            Vector2 rebuildDestination = (Vector2)InvokeInstance(controller, "GetRebuildRequestedDestination");
            Assert.That(rebuildDestination, Is.EqualTo(requestedDestination));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldKeepRequestedDestination_WhenIgnoringNpcSelfCollider()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");

        GameObject navGridGo = new GameObject("NavGrid_IgnoreSelfColliderTest");
        GameObject npcGo = new GameObject("NPC_SelfBlocker");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            SetFieldOrProperty(navGrid, "obstacleTags", new[] { "NPC" });
            LayerMask noObstacleMask = 0;
            SetFieldOrProperty(navGrid, "obstacleMask", noObstacleMask);

            npcGo.tag = "NPC";
            npcGo.transform.position = Vector2.zero;
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.8f, 0.6f);

            InvokeInstance(navGrid, "RebuildGrid");

            Vector2 requestedStart = new Vector2(-2f, 0f);
            Vector2 requestedDestination = Vector2.zero;

            object driftedState = Activator.CreateInstance(executionStateType);
            object driftedResult = InvokeStatic(
                executorType,
                "TryRefreshPath",
                driftedState,
                navGrid,
                requestedStart,
                requestedDestination,
                null,
                null,
                0f,
                null,
                null);

            object recoveredState = Activator.CreateInstance(executionStateType);
            object recoveredResult = InvokeStatic(
                executorType,
                "TryRefreshPath",
                recoveredState,
                navGrid,
                requestedStart,
                requestedDestination,
                null,
                null,
                0f,
                null,
                npcCollider);

            bool driftedSuccess = (bool)GetFieldOrProperty(driftedResult, "Success");
            Vector2 driftedActualDestination = (Vector2)GetFieldOrProperty(driftedResult, "ActualDestination");
            bool recoveredSuccess = (bool)GetFieldOrProperty(recoveredResult, "Success");
            Vector2 recoveredActualDestination = (Vector2)GetFieldOrProperty(recoveredResult, "ActualDestination");
            bool recoveredHasDestination = (bool)GetFieldOrProperty(recoveredState, "HasDestination");
            Vector2 recoveredStateDestination = (Vector2)GetFieldOrProperty(recoveredState, "Destination");

            Assert.That(driftedSuccess, Is.True);
            Assert.That(recoveredSuccess, Is.True);
            Assert.That(Vector2.Distance(driftedActualDestination, requestedDestination), Is.GreaterThan(0.01f));
            Assert.That(recoveredActualDestination, Is.EqualTo(requestedDestination));
            Assert.That(recoveredHasDestination, Is.True);
            Assert.That(recoveredStateDestination, Is.EqualTo(requestedDestination));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldOwnStuckRecoveryState_WhenSharedRepathSucceeds()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");

        GameObject navGridGo = new GameObject("NavGrid_SharedStuckRecoveryTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Vector2 currentPosition = new Vector2(-2f, 0f);
            Vector2 requestedDestination = new Vector2(2f, 0f);
            SetFieldOrProperty(state, "LastProgressCheckPosition", currentPosition);
            SetFieldOrProperty(state, "LastProgressCheckTime", 0f);

            object recovery = InvokeStatic(
                executorType,
                "TryHandleStuckRecovery",
                state,
                navGrid,
                currentPosition,
                requestedDestination,
                1f,
                0.3f,
                0.1f,
                3,
                0.25f,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(recovery, "Checked"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "IsStuck"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "RepathAttempted"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "RepathSucceeded"), Is.True);
            Assert.That((bool)GetFieldOrProperty(state, "HasDestination"), Is.True);
            Assert.That((Vector2)GetFieldOrProperty(state, "Destination"), Is.EqualTo(requestedDestination));
            Assert.That((float)GetFieldOrProperty(state, "LastRepathTime"), Is.EqualTo(1f).Within(0.001f));
            Assert.That((float)GetFieldOrProperty(state, "LastRecoveryTime"), Is.EqualTo(1f).Within(0.001f));
            Assert.That((bool)GetFieldOrProperty(state, "LastRecoverySucceeded"), Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldBlockStuckRepath_WhenSharedCooldownIsActive()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");

        GameObject navGridGo = new GameObject("NavGrid_StuckRecoveryCooldownTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Vector2 currentPosition = new Vector2(-2f, 0f);
            Vector2 requestedDestination = new Vector2(2f, 0f);
            SetFieldOrProperty(state, "LastProgressCheckPosition", currentPosition);
            SetFieldOrProperty(state, "LastProgressCheckTime", 0f);
            SetFieldOrProperty(state, "LastRepathTime", 0.8f);

            object recovery = InvokeStatic(
                executorType,
                "TryHandleStuckRecovery",
                state,
                navGrid,
                currentPosition,
                requestedDestination,
                1f,
                0.3f,
                0.1f,
                3,
                0.5f,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(recovery, "Checked"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "IsStuck"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "RepathOnCooldown"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovery, "RepathAttempted"), Is.False);
            Assert.That((bool)GetFieldOrProperty(state, "HasDestination"), Is.False);
            Assert.That((float)GetFieldOrProperty(state, "LastRepathTime"), Is.EqualTo(0.8f).Within(0.001f));
            Assert.That((float)GetFieldOrProperty(state, "LastRecoveryTime"), Is.EqualTo(1f).Within(0.001f));
            Assert.That((bool)GetFieldOrProperty(state, "LastRecoverySucceeded"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldCreateDetour_AndOwnDetourState()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");
        Type detourSpecType = ResolveTypeOrFail("NavigationPathExecutor2D+DetourCandidateSpec");

        GameObject navGridGo = new GameObject("NavGrid_DetourCreateOwnerTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Array candidateSpecs = Array.CreateInstance(detourSpecType, 2);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, false, 1.45f, 0.8f), 0);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, true, 1.55f, 0.95f), 1);

            object lifecycle = InvokeStatic(
                executorType,
                "TryCreateDetour",
                state,
                navGrid,
                Vector2.zero,
                7,
                new Vector2(0.9f, 0f),
                Vector2.right,
                Vector2.up,
                1.1f,
                1.1f,
                candidateSpecs,
                2f);

            Assert.That((bool)GetFieldOrProperty(lifecycle, "Created"), Is.True);
            Assert.That((bool)GetFieldOrProperty(lifecycle, "HasActiveDetour"), Is.True);
            Assert.That((int)GetFieldOrProperty(state, "OverrideWaypointOwnerId"), Is.EqualTo(7));
            Assert.That((int)GetFieldOrProperty(state, "LastDetourOwnerId"), Is.EqualTo(7));
            Assert.That((Vector2)GetFieldOrProperty(state, "OverrideWaypoint"), Is.EqualTo((Vector2)GetFieldOrProperty(lifecycle, "DetourPoint")));
            Assert.That((Vector2)GetFieldOrProperty(state, "LastDetourPoint"), Is.EqualTo((Vector2)GetFieldOrProperty(lifecycle, "DetourPoint")));
            Assert.That((float)GetFieldOrProperty(state, "LastDetourCreateTime"), Is.EqualTo(2f).Within(0.001f));
            Assert.That((bool)GetFieldOrProperty(state, "LastDetourRecoverySucceeded"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldClearDetour_AndRecoverPath()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");
        Type detourSpecType = ResolveTypeOrFail("NavigationPathExecutor2D+DetourCandidateSpec");

        GameObject navGridGo = new GameObject("NavGrid_DetourRecoverOwnerTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Vector2 requestedStart = new Vector2(-2f, 0f);
            Vector2 requestedDestination = new Vector2(2f, 0f);
            object buildResult = InvokeStatic(
                executorType,
                "TryRefreshPath",
                state,
                navGrid,
                requestedStart,
                requestedDestination,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(buildResult, "Success"), Is.True);

            Array candidateSpecs = Array.CreateInstance(detourSpecType, 2);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, false, 1.45f, 0.8f), 0);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, true, 1.55f, 0.95f), 1);

            object created = InvokeStatic(
                executorType,
                "TryCreateDetour",
                state,
                navGrid,
                new Vector2(-0.5f, 0f),
                9,
                new Vector2(0.1f, 0f),
                Vector2.right,
                Vector2.up,
                1.1f,
                1.1f,
                candidateSpecs,
                3f);

            Assert.That((bool)GetFieldOrProperty(created, "Created"), Is.True);

            object recovered = InvokeStatic(
                executorType,
                "TryClearDetourAndRecover",
                state,
                navGrid,
                new Vector2(-0.5f, 0f),
                requestedDestination,
                0,
                4f,
                true,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(recovered, "Cleared"), Is.True);
            Assert.That((bool)GetFieldOrProperty(recovered, "Recovered"), Is.True);
            Assert.That((bool)GetFieldOrProperty(state, "HasOverrideWaypoint"), Is.False);
            Assert.That((bool)GetFieldOrProperty(state, "HasDestination"), Is.True);
            Assert.That((Vector2)GetFieldOrProperty(state, "Destination"), Is.EqualTo(requestedDestination));
            Assert.That((float)GetFieldOrProperty(state, "LastDetourClearTime"), Is.EqualTo(4f).Within(0.001f));
            Assert.That((float)GetFieldOrProperty(state, "LastDetourRecoveryTime"), Is.EqualTo(4f).Within(0.001f));
            Assert.That((bool)GetFieldOrProperty(state, "LastDetourRecoverySucceeded"), Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldKeepActiveDetour_WhenClearHysteresisIsActive()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");
        Type detourSpecType = ResolveTypeOrFail("NavigationPathExecutor2D+DetourCandidateSpec");

        GameObject navGridGo = new GameObject("NavGrid_DetourClearHysteresisTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Array candidateSpecs = Array.CreateInstance(detourSpecType, 1);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, false, 1.45f, 0.8f), 0);

            object created = InvokeStatic(
                executorType,
                "TryCreateDetour",
                state,
                navGrid,
                Vector2.zero,
                12,
                new Vector2(0.9f, 0f),
                Vector2.right,
                Vector2.up,
                1.1f,
                1.1f,
                candidateSpecs,
                2f);

            Assert.That((bool)GetFieldOrProperty(created, "Created"), Is.True);

            object throttled = InvokeStatic(
                executorType,
                "TryClearDetourAndRecover",
                state,
                navGrid,
                Vector2.zero,
                new Vector2(2f, 0f),
                0,
                2.1f,
                true,
                0.35f,
                0.2f,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(throttled, "Cleared"), Is.False);
            Assert.That((bool)GetFieldOrProperty(throttled, "Recovered"), Is.False);
            Assert.That((bool)GetFieldOrProperty(throttled, "HasActiveDetour"), Is.True);
            Assert.That((bool)GetFieldOrProperty(throttled, "ShouldKeepCurrentDetour"), Is.True);
            Assert.That(GetFieldOrProperty(throttled, "FailureReason"), Is.EqualTo("detour_clear_hysteresis"));
            Assert.That((bool)GetFieldOrProperty(state, "HasOverrideWaypoint"), Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NavigationPathExecutor_ShouldThrottleRepeatedRecovery_WhenCooldownIsActive()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");
        Type detourSpecType = ResolveTypeOrFail("NavigationPathExecutor2D+DetourCandidateSpec");

        GameObject navGridGo = new GameObject("NavGrid_DetourRecoveryCooldownTest");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");

            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-4f, -4f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(8f, 8f));
            SetFieldOrProperty(navGrid, "cellSize", 0.5f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.5f);
            InvokeInstance(navGrid, "RebuildGrid");

            object state = Activator.CreateInstance(executionStateType);
            Array candidateSpecs = Array.CreateInstance(detourSpecType, 1);
            candidateSpecs.SetValue(Activator.CreateInstance(detourSpecType, false, 1.45f, 0.8f), 0);

            object created = InvokeStatic(
                executorType,
                "TryCreateDetour",
                state,
                navGrid,
                Vector2.zero,
                21,
                new Vector2(0.9f, 0f),
                Vector2.right,
                Vector2.up,
                1.1f,
                1.1f,
                candidateSpecs,
                2f);

            Assert.That((bool)GetFieldOrProperty(created, "Created"), Is.True);

            object recovered = InvokeStatic(
                executorType,
                "TryClearDetourAndRecover",
                state,
                navGrid,
                Vector2.zero,
                new Vector2(2f, 0f),
                0,
                2.5f,
                true,
                0f,
                0f,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(recovered, "Recovered"), Is.True);

            object throttled = InvokeStatic(
                executorType,
                "TryClearDetourAndRecover",
                state,
                navGrid,
                Vector2.zero,
                new Vector2(2f, 0f),
                0,
                2.62f,
                true,
                0f,
                0.2f,
                null,
                null,
                0f,
                null,
                null);

            Assert.That((bool)GetFieldOrProperty(throttled, "Cleared"), Is.False);
            Assert.That((bool)GetFieldOrProperty(throttled, "Recovered"), Is.False);
            Assert.That((bool)GetFieldOrProperty(throttled, "HasActiveDetour"), Is.False);
            Assert.That((bool)GetFieldOrProperty(throttled, "ShouldKeepCurrentDetour"), Is.False);
            Assert.That(GetFieldOrProperty(throttled, "FailureReason"), Is.EqualTo("detour_recovery_cooldown"));
            Assert.That((float)GetFieldOrProperty(state, "LastDetourRecoveryTime"), Is.EqualTo(2.5f).Within(0.001f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    private static object CreateSnapshot(
        Type snapshotType,
        Type unitType,
        int instanceId,
        string unitTypeName,
        Vector2 position,
        Vector2 velocity,
        float colliderRadius,
        float avoidanceRadius,
        int avoidancePriority,
        bool isCurrentlyMoving,
        bool isNavigationSleeping,
        bool participatesInLocalAvoidance)
    {
        object enumValue = Enum.Parse(unitType, unitTypeName);
        return Activator.CreateInstance(
            snapshotType,
            instanceId,
            enumValue,
            position,
            velocity,
            colliderRadius,
            avoidanceRadius,
            avoidancePriority,
            isCurrentlyMoving,
            isNavigationSleeping,
            participatesInLocalAvoidance);
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            Type[] candidates;
            try
            {
                candidates = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                candidates = ex.Types;
            }

            foreach (Type candidate in candidates)
            {
                if (candidate != null && (candidate.FullName == typeName || candidate.Name == typeName))
                {
                    return candidate;
                }
            }
        }

        Assert.Fail($"未找到类型：{typeName}");
        return null;
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, args);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(target.GetType(), methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, args);
        Assert.IsNotNull(method, $"未找到实例方法：{target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static bool InvokeStaticBool(Type type, string methodName, params object[] args)
    {
        return (bool)InvokeStatic(type, methodName, args);
    }

    private static object GetFieldOrProperty(object target, string name)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        Assert.Fail($"未找到字段或属性：{type.Name}.{name}");
        return null;
    }

    private static void SetFieldOrProperty(object target, string name, object value)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
            return;
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            property.SetValue(target, value);
            return;
        }

        Assert.Fail($"未找到可写字段或属性：{type.Name}.{name}");
    }

    private static MethodInfo ResolveMethodByArguments(Type type, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo fallback = null;
        MethodInfo[] methods = type.GetMethods(bindingFlags);
        for (int index = 0; index < methods.Length; index++)
        {
            MethodInfo method = methods[index];
            if (method.Name != methodName)
            {
                continue;
            }

            if (fallback == null)
            {
                fallback = method;
            }

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool match = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }

                object arg = args[parameterIndex];
                if (arg == null)
                {
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                    {
                        match = false;
                        break;
                    }

                    continue;
                }

                Type argType = arg.GetType();
                if (!parameterType.IsAssignableFrom(argType))
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                return method;
            }
        }

        return fallback;
    }
}
