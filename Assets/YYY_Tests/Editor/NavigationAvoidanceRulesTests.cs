using System;
using System.Collections;
using System.Collections.Generic;
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
    public void Solver_ShouldRequestRepath_WhenMovingNpcPredictedConflictIsCloserThanCurrentForwardDistance()
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
            0.35f,
            100,
            true,
            false,
            true);

        object movingNpc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            new Vector2(0.9f, -0.05f),
            new Vector2(0f, 1.8f),
            0.3f,
            0.35f,
            50,
            true,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(movingNpc, 0);

        object result = InvokeStatic(solverType, "Solve", player, Vector2.right, 1.2f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");
        float speedScale = (float)GetFieldOrProperty(result, "SpeedScale");
        float blockingDistance = (float)GetFieldOrProperty(result, "BlockingDistance");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.True);
        Assert.That(speedScale, Is.LessThan(0.65f));
        Assert.That(blockingDistance, Is.LessThan(0.9f));
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
    public void Solver_ShouldKeepYieldingNpcPeerMovingSideways_WhenHeadOnConflictGetsClose()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object yieldingNpc = CreateSnapshot(
            snapshotType,
            unitType,
            2,
            "NPC",
            Vector2.zero,
            Vector2.right,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        object holdCourseNpc = CreateSnapshot(
            snapshotType,
            unitType,
            1,
            "NPC",
            new Vector2(0.74f, 0.02f),
            Vector2.left,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(holdCourseNpc, 0);

        object result = InvokeStatic(solverType, "Solve", yieldingNpc, Vector2.right, 1.4f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");
        float speedScale = (float)GetFieldOrProperty(result, "SpeedScale");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.True);
        Assert.That(speedScale, Is.GreaterThanOrEqualTo(0.22f));
        Assert.That(Mathf.Abs(adjustedDirection.y), Is.GreaterThan(0.08f));
    }

    [Test]
    public void Solver_ShouldKeepHoldCourseNpcPeerAdvancing_WhenHeadOnConflictGetsClose()
    {
        Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
        Type unitType = ResolveTypeOrFail("NavigationUnitType");
        Type solverType = ResolveTypeOrFail("NavigationLocalAvoidanceSolver");

        object holdCourseNpc = CreateSnapshot(
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
            new Vector2(0.74f, 0.02f),
            Vector2.left,
            0.35f,
            0.6f,
            50,
            true,
            false,
            true);

        Array nearbyAgents = Array.CreateInstance(snapshotType, 1);
        nearbyAgents.SetValue(yieldingNpc, 0);

        object result = InvokeStatic(solverType, "Solve", holdCourseNpc, Vector2.right, 1.4f, nearbyAgents);
        Assert.IsNotNull(result);

        bool hasBlockingAgent = (bool)GetFieldOrProperty(result, "HasBlockingAgent");
        bool shouldRepath = (bool)GetFieldOrProperty(result, "ShouldRepath");
        float speedScale = (float)GetFieldOrProperty(result, "SpeedScale");
        Vector2 adjustedDirection = (Vector2)GetFieldOrProperty(result, "AdjustedDirection");

        Assert.That(hasBlockingAgent, Is.True);
        Assert.That(shouldRepath, Is.False);
        Assert.That(speedScale, Is.GreaterThan(0.78f));
        Assert.That(adjustedDirection.x, Is.GreaterThan(0.78f));
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
    public void NPCAutoRoamController_ShouldTriggerStaticNearWallHardStop_WhenObstacleSitsDirectlyAhead()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject npcGo = new GameObject("NPCAutoRoamController_StaticSteeringTest");
        GameObject wallGo = new GameObject("StaticWallAhead");

        try
        {
            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);

            BoxCollider2D wallCollider = wallGo.AddComponent<BoxCollider2D>();
            wallGo.transform.position = new Vector2(0.48f, 0f);
            wallCollider.size = new Vector2(0.18f, 1.1f);
            Physics2D.SyncTransforms();

            Vector2 firstAdjustedDirection = (Vector2)InvokeInstance(
                controller,
                "AdjustDirectionByStaticColliders",
                Vector2.zero,
                Vector2.right,
                0.12f);
            bool firstHardStop = (bool)InvokeInstance(controller, "ShouldTriggerStaticNearWallHardStop");

            Vector2 secondAdjustedDirection = (Vector2)InvokeInstance(
                controller,
                "AdjustDirectionByStaticColliders",
                Vector2.zero,
                Vector2.right,
                0.12f);
            bool secondHardStop = (bool)InvokeInstance(controller, "ShouldTriggerStaticNearWallHardStop");

            Assert.That(firstAdjustedDirection.sqrMagnitude, Is.GreaterThan(0.0001f));
            Assert.That(firstHardStop, Is.False);
            Assert.That(secondAdjustedDirection.sqrMagnitude, Is.GreaterThan(0.0001f));
            Assert.That(secondHardStop, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(wallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
        }
    }

    public void NPCAutoRoamController_ShouldKeepSoftSteering_WhenStaticObstacleIsOffsetFromPath()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject npcGo = new GameObject("NPCAutoRoamController_StaticSoftSteeringTest");
        GameObject wallGo = new GameObject("StaticWallOffset");

        try
        {
            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);

            BoxCollider2D wallCollider = wallGo.AddComponent<BoxCollider2D>();
            wallGo.transform.position = new Vector2(0.48f, 0.22f);
            wallCollider.size = new Vector2(0.18f, 0.9f);
            Physics2D.SyncTransforms();

            Vector2 adjustedDirection = (Vector2)InvokeInstance(
                controller,
                "AdjustDirectionByStaticColliders",
                Vector2.zero,
                Vector2.right,
                0.12f);
            bool hardStop = (bool)InvokeInstance(controller, "ShouldTriggerStaticNearWallHardStop");

            Assert.That(adjustedDirection.x, Is.GreaterThan(0.7f));
            Assert.That(Mathf.Abs(adjustedDirection.y), Is.GreaterThan(0.01f));
            Assert.That(hardStop, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(wallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldReuseObstacleSteeringProbe_WhenFollowingPreviousAdjustedDirection()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject npcGo = new GameObject("NPCAutoRoamController_StaticProbeReuseTest");

        try
        {
            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);
            Vector2 cachedAdjustedDirection = new Vector2(0.88f, 0.42f).normalized;
            SetFieldOrProperty(controller, "lastStaticSteeringProbeDetectedObstacle", true);
            SetFieldOrProperty(controller, "lastStaticSteeringProbeTime", Time.time);
            SetFieldOrProperty(controller, "lastStaticSteeringProbePosition", Vector2.zero);
            SetFieldOrProperty(controller, "lastStaticSteeringProbeDirection", Vector2.right);
            SetFieldOrProperty(controller, "lastStaticSteeringAdjustedDirection", cachedAdjustedDirection);

            object[] reuseArgs =
            {
                new Vector2(0.03f, 0.01f),
                cachedAdjustedDirection,
                0.12f,
                Vector2.zero
            };
            MethodInfo reuseMethod = ResolveMethodByArguments(
                controllerType,
                "TryReuseObstacleStaticSteeringProbe",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                reuseArgs);

            Assert.That(reuseMethod, Is.Not.Null, "应能直接命中静态 obstacle steering probe 复用入口。");

            bool reused = (bool)reuseMethod.Invoke(controller, reuseArgs);
            Vector2 reusedDirection = (Vector2)reuseArgs[3];

            Assert.That(reused, Is.True);
            Assert.That(Vector2.Distance(reusedDirection, cachedAdjustedDirection), Is.LessThan(0.001f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(npcGo);
        }
    }

    [Test]
    public void NavigationTraversalCore_ShouldRejectNarrowGap_WhenClearanceMarginWouldClipNpcBody()
    {
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type traversalCoreType = ResolveTypeOrFail("NavigationTraversalCore");
        Type traversalContractType = ResolveTypeOrFail("NavigationTraversalCore+Contract");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_ClearanceMarginGapTest");
        GameObject npcGo = new GameObject("NPC_ClearanceMarginGapTest");
        GameObject leftWallGo = new GameObject("GapWall_Left");
        GameObject rightWallGo = new GameObject("GapWall_Right");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D leftWall = leftWallGo.AddComponent<BoxCollider2D>();
            leftWall.size = new Vector2(0.12f, 1.6f);
            leftWallGo.transform.position = new Vector2(-0.34f, 0f);

            BoxCollider2D rightWall = rightWallGo.AddComponent<BoxCollider2D>();
            rightWall.size = new Vector2(0.12f, 1.6f);
            rightWallGo.transform.position = new Vector2(0.34f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { leftWall, rightWall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            object traversalContract = Activator.CreateInstance(
                traversalContractType,
                navGrid,
                npcCollider,
                0.08f,
                0.05f,
                0.02f);

            bool footProbeOccupiable = InvokeStaticBool(
                traversalCoreType,
                "CanOccupyNavigationPoint",
                traversalContract,
                Vector2.zero);
            bool bodyClearanceOccupiable = InvokeStaticBool(
                traversalCoreType,
                "CanOccupyNavigationPointWithClearanceMargin",
                traversalContract,
                Vector2.zero,
                0.12f,
                0.18f,
                0.35f);

            Assert.That(footProbeOccupiable, Is.True);
            Assert.That(bodyClearanceOccupiable, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(rightWallGo);
            UnityEngine.Object.DestroyImmediate(leftWallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldTreatStoryOwnedFormalReturn_AsResidentScriptedMove()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        GameObject go = new GameObject("NPCAutoRoamController_StoryOwnedFormalReturnTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            IList scriptedOwners = (IList)GetFieldOrProperty(controller, "residentScriptedControlOwners");
            scriptedOwners.Add("day1");

            SetFieldOrProperty(controller, "debugMoveActive", true);
            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "FormalNavigation"));
            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));

            bool isResidentScriptedMoveActive = (bool)GetFieldOrProperty(controller, "IsResidentScriptedMoveActive");
            Assert.That(isResidentScriptedMoveActive, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldKeepPlainDebugBypass_ButEnableRecoverablePointToPointStaticSteering()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_DebugContractTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
            SetFieldOrProperty(controller, "debugMoveActive", true);
            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "PlainDebug"));

            bool plainDebugBypassesSharedAvoidance = (bool)InvokeInstance(controller, "ShouldBypassSharedAvoidanceForCurrentMove");
            bool plainDebugUsesStaticSteering = (bool)InvokeInstance(controller, "ShouldApplyAutonomousStaticObstacleSteering");

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "AutonomousDirected"));

            bool autonomousDirectedBypassesSharedAvoidance = (bool)InvokeInstance(controller, "ShouldBypassSharedAvoidanceForCurrentMove");
            bool autonomousDirectedUsesStaticSteering = (bool)InvokeInstance(controller, "ShouldApplyAutonomousStaticObstacleSteering");

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "FormalNavigation"));

            bool formalHomeReturnBypassesSharedAvoidance = (bool)InvokeInstance(controller, "ShouldBypassSharedAvoidanceForCurrentMove");
            bool formalHomeReturnUsesStaticSteering = (bool)InvokeInstance(controller, "ShouldApplyAutonomousStaticObstacleSteering");

            Assert.That(plainDebugBypassesSharedAvoidance, Is.True);
            Assert.That(plainDebugUsesStaticSteering, Is.False);
            Assert.That(autonomousDirectedBypassesSharedAvoidance, Is.False);
            Assert.That(autonomousDirectedUsesStaticSteering, Is.True);
            Assert.That(formalHomeReturnBypassesSharedAvoidance, Is.False);
            Assert.That(formalHomeReturnUsesStaticSteering, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldEnableReleasedBodyClearance_ForRecoverablePointToPointContracts()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        GameObject go = new GameObject("NPCAutoRoamController_ReleasedBodyClearanceContractTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "debugMoveActive", true);

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "PlainDebug"));
            bool plainDebugUsesReleasedBodyClearance = (bool)InvokeInstance(controller, "ShouldUseReleasedMovementBodyClearanceExecutionContract");

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "AutonomousDirected"));
            bool autonomousDirectedUsesReleasedBodyClearance = (bool)InvokeInstance(controller, "ShouldUseReleasedMovementBodyClearanceExecutionContract");

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "ResidentScripted"));
            bool residentScriptedUsesReleasedBodyClearance = (bool)InvokeInstance(controller, "ShouldUseReleasedMovementBodyClearanceExecutionContract");

            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "FormalNavigation"));
            bool formalNavigationUsesReleasedBodyClearance = (bool)InvokeInstance(controller, "ShouldUseReleasedMovementBodyClearanceExecutionContract");

            Assert.That(plainDebugUsesReleasedBodyClearance, Is.False);
            Assert.That(autonomousDirectedUsesReleasedBodyClearance, Is.True);
            Assert.That(residentScriptedUsesReleasedBodyClearance, Is.True);
            Assert.That(formalNavigationUsesReleasedBodyClearance, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldDelayImmediateRestart_AfterFormalNavigationArrival()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_FormalRestartSettleTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = Vector2.zero;

            SetFieldOrProperty(controller, "lastFormalNavigationArrivalTime", Time.time);
            SetFieldOrProperty(controller, "lastFormalNavigationArrivalPosition", Vector2.zero);

            bool shouldDelayRestart = (bool)InvokeInstance(controller, "ShouldDelayImmediateRoamRestartAfterFormalNavigation");
            Assert.That(shouldDelayRestart, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldReuseCrossFrameDecisionWindow_ForAutonomousRoam()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_AutonomousReuseWindowTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "lastHeavyMoveDecisionTime", Time.time);

            bool canReuse = (bool)InvokeInstance(controller, "CanReuseCrossFrameMoveDecisionWindow");
            Assert.That(canReuse, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldExtendAutonomousReuseWindow_AfterAdvanceConfirmed()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_AutonomousStableReuseWindowTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "lastHeavyMoveDecisionTime", Time.time - 0.08f);

            SetFieldOrProperty(controller, "lastMoveSkipReason", "IssuingVelocity");
            bool canReuseBeforeAdvanceConfirmed = (bool)InvokeInstance(controller, "CanReuseCrossFrameMoveDecisionWindow");

            SetFieldOrProperty(controller, "lastMoveSkipReason", "AdvanceConfirmed");
            bool canReuseAfterAdvanceConfirmed = (bool)InvokeInstance(controller, "CanReuseCrossFrameMoveDecisionWindow");

            Assert.That(canReuseBeforeAdvanceConfirmed, Is.False);
            Assert.That(canReuseAfterAdvanceConfirmed, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldReuseClearStaticSteeringProbe_WhenDirectionStaysStable()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_StaticSteeringProbeReuseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            InvokeInstance(controller, "RecordStaticSteeringProbe", Vector2.zero, Vector2.right, false);

            bool canReuse = (bool)InvokeInstance(
                controller,
                "CanReuseClearStaticSteeringProbe",
                new Vector2(0.08f, 0f),
                Vector2.right,
                0.1f);

            Assert.That(canReuse, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldNotReuseClearStaticSteeringProbe_WhenLastProbeDetectedObstacle()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_StaticSteeringObstacleProbeReuseRejectTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            InvokeInstance(controller, "RecordStaticSteeringProbe", Vector2.zero, Vector2.right, true);

            bool canReuse = (bool)InvokeInstance(
                controller,
                "CanReuseClearStaticSteeringProbe",
                new Vector2(0.08f, 0f),
                Vector2.right,
                0.1f);

            Assert.That(canReuse, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldReuseObstacleStaticSteeringProbe_WhenDirectionAndPositionStayStable()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_StaticSteeringObstacleProbeReuseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "lastStaticSteeringProbeTime", Time.time);
            SetFieldOrProperty(controller, "lastStaticSteeringProbePosition", Vector2.zero);
            SetFieldOrProperty(controller, "lastStaticSteeringProbeDirection", Vector2.right);
            SetFieldOrProperty(controller, "lastStaticSteeringProbeDetectedObstacle", true);
            SetFieldOrProperty(controller, "lastStaticSteeringAdjustedDirection", new Vector2(0.8f, 0.6f).normalized);

            MethodInfo reuseMethod = controllerType.GetMethod(
                "TryReuseObstacleStaticSteeringProbe",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(reuseMethod, Is.Not.Null);

            object[] args =
            {
                new Vector2(0.1f, 0.01f),
                Vector2.right,
                0.12f,
                null
            };

            bool reused = (bool)reuseMethod.Invoke(controller, args);

            Assert.That(reused, Is.True);
            Assert.That((Vector2)args[3], Is.EqualTo(new Vector2(0.8f, 0.6f).normalized));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldUseFastRetryShortPause_WhenAutonomousRetryWasDeferred()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_AutonomousFastRetryShortPauseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "shortPauseMin", 0.5f);
            SetFieldOrProperty(controller, "shortPauseMax", 3f);
            SetFieldOrProperty(controller, "preferFastAutonomousRetryShortPause", true);

            InvokeInstance(controller, "EnterShortPause", false);

            float timer = (float)GetFieldOrProperty(controller, "DebugStateTimer");
            Assert.That(timer, Is.GreaterThanOrEqualTo(0.03f));
            Assert.That(timer, Is.LessThan(0.2f));
            Assert.That((bool)GetFieldOrProperty(controller, "preferFastAutonomousRetryShortPause"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldDeferAutonomousResumeFromShortPause_WhenResumePermitBudgetIsExhausted()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_AutonomousResumePermitShortPauseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "ShortPause"));
            SetFieldOrProperty(controller, "stateTimer", 0f);
            SetFieldOrProperty(controller, "longPauseTriggerCount", 1);
            SetFieldOrProperty(controller, "autonomousRoamStartupWindowUntil", Time.time + 1f);
            SetFieldOrProperty(controller, "shortPauseMin", 0.1f);
            SetFieldOrProperty(controller, "shortPauseMax", 0.1f);

            for (int index = 0; index < 4; index++)
            {
                bool acquired = (bool)InvokeInstance(controller, "TryAcquireAutonomousResumePermit");
                Assert.That(acquired, Is.True);
            }

            bool deferred = (bool)InvokeInstance(controller, "TryDeferAutonomousResumeFromPauseState");

            Assert.That(deferred, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.GreaterThan(0f));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.LessThan(0.2f));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("AutonomousResumePermitDeferred"));
        }
        finally
        {
            SetStaticField(controllerType, "s_autonomousStartupResumePermitFrame", -1);
            SetStaticField(controllerType, "s_autonomousStartupResumePermitsThisFrame", 0);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldDeferAutonomousResumeFromLongPause_WhenResumePermitBudgetIsExhausted()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_AutonomousResumePermitLongPauseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "LongPause"));
            SetFieldOrProperty(controller, "stateTimer", 0f);
            SetFieldOrProperty(controller, "autonomousRoamStartupWindowUntil", Time.time + 1f);
            SetFieldOrProperty(controller, "shortPauseMin", 0.1f);
            SetFieldOrProperty(controller, "shortPauseMax", 0.1f);

            for (int index = 0; index < 4; index++)
            {
                bool acquired = (bool)InvokeInstance(controller, "TryAcquireAutonomousResumePermit");
                Assert.That(acquired, Is.True);
            }

            bool deferred = (bool)InvokeInstance(controller, "TryDeferAutonomousResumeFromPauseState");

            Assert.That(deferred, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.GreaterThan(0f));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.LessThan(0.2f));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("AdvanceConfirmed"));
        }
        finally
        {
            SetStaticField(controllerType, "s_autonomousStartupResumePermitFrame", -1);
            SetStaticField(controllerType, "s_autonomousStartupResumePermitsThisFrame", 0);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldThrottleTraversalSoftPassRefresh_WhenStateAndPositionAreStable()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_SoftPassThrottleStableTest");
        GameObject blockerGo = new GameObject("NPCAutoRoamController_SoftPassThrottleBlocker");

        try
        {
            Component controller = go.AddComponent(controllerType);
            BoxCollider2D blocker = blockerGo.AddComponent<BoxCollider2D>();
            object shortPause = Enum.Parse(roamStateType, "ShortPause");

            SetFieldOrProperty(controller, "allowTraversalOverridePhysicsSoftPass", true);
            SetFieldOrProperty(controller, "traversalSoftPassBlockers", new Collider2D[] { blocker });
            SetFieldOrProperty(controller, "hasTraversalSoftPassSample", true);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSampleTime", Time.time);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSamplePosition", Vector2.zero);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSampleState", shortPause);
            SetFieldOrProperty(controller, "state", shortPause);
            SetFieldOrProperty(controller, "lastTraversalSoftPassUpdateFrame", Time.frameCount - 1);

            bool shouldRefresh = (bool)InvokeInstance(
                controller,
                "ShouldRefreshTraversalSoftPassState",
                Vector2.zero);

            Assert.That(shouldRefresh, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(blockerGo);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRefreshTraversalSoftPass_WhenRoamStateChanges()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_SoftPassStateChangeTest");
        GameObject blockerGo = new GameObject("NPCAutoRoamController_SoftPassStateChangeBlocker");

        try
        {
            Component controller = go.AddComponent(controllerType);
            BoxCollider2D blocker = blockerGo.AddComponent<BoxCollider2D>();
            object shortPause = Enum.Parse(roamStateType, "ShortPause");
            object moving = Enum.Parse(roamStateType, "Moving");

            SetFieldOrProperty(controller, "allowTraversalOverridePhysicsSoftPass", true);
            SetFieldOrProperty(controller, "traversalSoftPassBlockers", new Collider2D[] { blocker });
            SetFieldOrProperty(controller, "hasTraversalSoftPassSample", true);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSampleTime", Time.time);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSamplePosition", Vector2.zero);
            SetFieldOrProperty(controller, "lastTraversalSoftPassSampleState", shortPause);
            SetFieldOrProperty(controller, "state", moving);
            SetFieldOrProperty(controller, "lastTraversalSoftPassUpdateFrame", Time.frameCount - 1);

            bool shouldRefresh = (bool)InvokeInstance(
                controller,
                "ShouldRefreshTraversalSoftPassState",
                Vector2.zero);

            Assert.That(shouldRefresh, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(blockerGo);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldSoftArrive_WhenAutonomousFailureHappensNearDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_SoftArrivalTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = new Vector2(0.16f, 0f);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool softArrived = (bool)InvokeInstance(
                controller,
                "TryFinishAutonomousRoamSoftArrival",
                new Vector2(0.16f, 0f),
                "UnitTest");

            Assert.That(softArrived, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((int)GetFieldOrProperty(controller, "CompletedShortPauseCount"), Is.EqualTo(1));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("AutonomousSoftArrival:UnitTest"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldUseBodyShellAsMainArrivalContract_ForAutonomousRoam()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_MainArrivalContractTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = new Vector2(0.16f, 0f);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "destinationTolerance", 0.12f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool arrived = (bool)InvokeInstance(
                controller,
                "CanTreatCurrentMoveAsBodyShellArrival",
                new Vector2(0.16f, 0f));
            float arrivalDistance = (float)InvokeInstance(controller, "GetCurrentMoveArrivalDistance");

            Assert.That(arrived, Is.True);
            Assert.That(arrivalDistance, Is.GreaterThan(0.45f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldUseBodyShellArrival_ForFormalNavigationMove()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        GameObject go = new GameObject("NPCAutoRoamController_FormalArrivalContractTest");

        try
        {
            Component controller = go.AddComponent(controllerType);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "debugMoveActive", true);
            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "FormalNavigation"));
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.36f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.36f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool arrived = (bool)InvokeInstance(
                controller,
                "CanTreatCurrentMoveAsBodyShellArrival",
                new Vector2(0.16f, 0f));

            Assert.That(arrived, Is.True);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldNotUseBodyShellArrival_ForResidentScriptedTravel()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        GameObject go = new GameObject("NPCAutoRoamController_ResidentScriptedArrivalGuardTest");

        try
        {
            Component controller = go.AddComponent(controllerType);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "debugMoveActive", true);
            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "ResidentScripted"));
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.36f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.36f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool arrived = (bool)InvokeInstance(
                controller,
                "CanTreatCurrentMoveAsBodyShellArrival",
                new Vector2(0.16f, 0f));

            Assert.That(arrived, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldNotSoftArrive_WhenAutonomousFailureIsStillFarFromDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_SoftArrivalRejectTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = Vector2.zero;

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(1.2f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(1.2f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool canSoftArrive = (bool)InvokeInstance(
                controller,
                "CanTreatAutonomousRoamFailureAsSoftArrival",
                Vector2.zero);

            Assert.That(canSoftArrive, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldExpandSoftArrivalDistance_WhenBlockedNearDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_BlockedSoftArrivalDistanceTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = Vector2.zero;

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.6f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.6f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            float baseArrivalDistance = (float)InvokeInstance(controller, "GetAutonomousRoamSoftArrivalDistance");
            bool baseCanArrive = (bool)InvokeInstance(
                controller,
                "CanTreatCurrentMoveAsBodyShellArrival",
                Vector2.zero);

            SetFieldOrProperty(controller, "blockedAdvanceFrames", 3);

            float blockedArrivalDistance = (float)InvokeInstance(controller, "GetAutonomousRoamSoftArrivalDistance");
            bool blockedCanArrive = (bool)InvokeInstance(
                controller,
                "CanTreatCurrentMoveAsBodyShellArrival",
                Vector2.zero);

            Assert.That(baseCanArrive, Is.False);
            Assert.That(blockedCanArrive, Is.True);
            Assert.That(blockedArrivalDistance, Is.GreaterThan(baseArrivalDistance));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldSoftArriveBlockedAdvance_WhenAutonomousFailureHappensNearDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_BlockedAdvanceSoftArrivalTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = new Vector2(0.16f, 0f);

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(0.34f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryHandleBlockedAdvance",
                new Vector2(0.16f, 0f),
                "MoveCommandNoProgress");

            Assert.That(handled, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((int)GetFieldOrProperty(controller, "CompletedShortPauseCount"), Is.EqualTo(1));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("AutonomousSoftArrival:BlockedAdvance:MoveCommandNoProgress"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldPreferMediumDistanceSampling_ForAutonomousRoam()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_MediumDistanceSamplingTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);

            float preferredDistance = (float)InvokeInstance(
                controller,
                "GetAutonomousRoamPreferredMinimumTravelDistance");
            float samplingDistance = (float)InvokeInstance(
                controller,
                "GetAutonomousRoamSamplingMinimumTravelDistance",
                false);

            Assert.That(preferredDistance, Is.GreaterThan(1.5f));
            Assert.That(samplingDistance, Is.GreaterThan(1.5f));
            Assert.That(samplingDistance, Is.LessThan(2.6f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldSampleOuterRing_WhenPreferredMinimumTravelDistanceNearlyMatchesActivityRadius()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_OuterRingSamplingTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);

            UnityEngine.Random.InitState(12345);
            Vector2 sampledOffset = (Vector2)InvokeInstance(
                controller,
                "SampleAutonomousRoamOffset",
                3f);

            Assert.That(sampledOffset.magnitude, Is.GreaterThanOrEqualTo(2.2f));
            Assert.That(sampledOffset.magnitude, Is.LessThanOrEqualTo(3.0001f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldEnterRecoveryHold_WhenStaticBlockedReasonRepeats()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_RecoveryHoldTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            go.transform.position = Vector2.zero;

            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(1.6f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(1.6f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryHandleBlockedAdvance",
                Vector2.zero,
                "StaticNearWallHardStop");

            Assert.That(handled, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.EqualTo(0.42f).Within(0.02f));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("AutonomousRecoveryHold:StaticNearWallHardStop"));
            Assert.That((bool)GetFieldOrProperty(controller, "autonomousRecoveryHoldActive"), Is.True);
            Assert.That((bool)GetFieldOrProperty(controller, "hasRequestedDestination"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldAbortRepeatedStaticNearWallHardStop_InsteadOfReenteringRecoveryHold()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_RepeatStaticHoldAbortTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            Vector2 currentPosition = Vector2.zero;
            Vector2 requestedDestination = new Vector2(1.6f, 0f);

            go.transform.position = currentPosition;
            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", requestedDestination);
            SetFieldOrProperty(controller, "requestedDestination", requestedDestination);
            SetFieldOrProperty(controller, "hasRequestedDestination", true);
            SetFieldOrProperty(controller, "blockedAdvanceFrames", 1);
            SetFieldOrProperty(controller, "lastBlockedAdvancePosition", currentPosition);
            SetFieldOrProperty(controller, "lastBlockedAdvanceRecoveryTime", float.NegativeInfinity);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryHandleBlockedAdvance",
                currentPosition,
                "StaticNearWallHardStop");

            Assert.That(handled, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((bool)GetFieldOrProperty(controller, "autonomousRecoveryHoldActive"), Is.False);
            Assert.That((bool)GetFieldOrProperty(controller, "hasRequestedDestination"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldPauseBeforeStaticRetargetRetry_AfterRepeatedStaticBlock()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_StaticRetargetPauseTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            Vector2 currentPosition = Vector2.zero;
            Vector2 requestedDestination = new Vector2(1.6f, 0f);

            go.transform.position = currentPosition;
            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", requestedDestination);
            SetFieldOrProperty(controller, "requestedDestination", requestedDestination);
            SetFieldOrProperty(controller, "hasRequestedDestination", true);
            SetFieldOrProperty(controller, "shortPauseMin", 0.1f);
            SetFieldOrProperty(controller, "shortPauseMax", 0.1f);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryRetargetAutonomousRoamAfterStaticBlock",
                currentPosition,
                "StaticNearWallHardStop");

            Assert.That(handled, Is.True);
            Assert.That(GetFieldOrProperty(controller, "DebugState"), Is.EqualTo("ShortPause"));
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("BlockedAdvanceStaticRetargetPause:StaticNearWallHardStop"));
            Assert.That((float)GetFieldOrProperty(controller, "DebugStateTimer"), Is.GreaterThanOrEqualTo(0.54f));
            Assert.That((bool)GetFieldOrProperty(controller, "hasRequestedDestination"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRememberStaticBlockedDestination_BeforeAutonomousRetarget()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        GameObject go = new GameObject("NPCAutoRoamController_StaticBlockMemoryTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            Vector2 currentPosition = Vector2.zero;
            Vector2 requestedDestination = new Vector2(2f, 0f);

            go.transform.position = currentPosition;
            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", requestedDestination);
            SetFieldOrProperty(controller, "requestedDestination", requestedDestination);
            SetFieldOrProperty(controller, "hasRequestedDestination", true);
            SetFieldOrProperty(controller, "blockedAdvanceFrames", 2);
            SetFieldOrProperty(controller, "lastBlockedAdvancePosition", currentPosition);
            SetFieldOrProperty(controller, "lastBlockedAdvanceRecoveryTime", float.NegativeInfinity);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryHandleBlockedAdvance",
                currentPosition,
                "MoveCommandNoProgress");

            Assert.That(handled, Is.True);
            Assert.That((Vector2)GetFieldOrProperty(controller, "lastStaticBlockedDestination"), Is.EqualTo(requestedDestination));
            Assert.That((Vector2)GetFieldOrProperty(controller, "lastStaticBlockedPosition"), Is.EqualTo(currentPosition));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRejectAutonomousBadZoneCandidate_AfterStaticBlockedDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_StaticBadZoneRejectTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);

            InvokeInstance(
                controller,
                "RememberStaticBlockedDestination",
                Vector2.zero,
                new Vector2(1.6f, 0f));

            bool rejected = (bool)InvokeInstance(
                controller,
                "IsRecentlyBlockedAutonomousDestination",
                new Vector2(0.72f, 0f));

            Assert.That(rejected, Is.True);
            Assert.That((float)GetFieldOrProperty(controller, "lastStaticBlockedZoneRadius"), Is.GreaterThan(0.8f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldPenalizeAutonomousBadZoneCandidate_AfterStaticBlockedDestination()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        GameObject go = new GameObject("NPCAutoRoamController_StaticBadZonePenaltyTest");

        try
        {
            Component controller = go.AddComponent(controllerType);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);

            InvokeInstance(
                controller,
                "RememberStaticBlockedDestination",
                Vector2.zero,
                new Vector2(1.6f, 0f));

            float nearPenalty = (float)InvokeInstance(
                controller,
                "GetRecentStaticBlockedBadZonePenalty",
                new Vector2(0.72f, 0f));
            float farPenalty = (float)InvokeInstance(
                controller,
                "GetRecentStaticBlockedBadZonePenalty",
                new Vector2(-2f, 0f));

            Assert.That(nearPenalty, Is.GreaterThan(0.4f));
            Assert.That(farPenalty, Is.EqualTo(0f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldPreferOpenSpaceOverCornerPocket_WhenAutonomousRoamScoresCandidates()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_AutonomousCornerScoreTest");
        GameObject npcGo = new GameObject("NPC_AutonomousCornerScoreTest");
        GameObject wallXGo = new GameObject("AutonomousCornerWall_X");
        GameObject wallYGo = new GameObject("AutonomousCornerWall_Y");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D wallX = wallXGo.AddComponent<BoxCollider2D>();
            wallX.size = new Vector2(0.12f, 1.2f);
            wallXGo.transform.position = new Vector2(0.68f, 0.58f);

            BoxCollider2D wallY = wallYGo.AddComponent<BoxCollider2D>();
            wallY.size = new Vector2(1.2f, 0.12f);
            wallYGo.transform.position = new Vector2(0.58f, 0.68f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { wallX, wallY },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "activityRadius", 3f);

            Vector2 openCandidate = new Vector2(0.9f, 0f);
            Vector2 cornerCandidate = new Vector2(0.55f, 0.55f);

            int openCornerCount = (int)InvokeInstance(controller, "CountAutonomousRoamOpenCornerProbes", openCandidate);
            int cornerCornerCount = (int)InvokeInstance(controller, "CountAutonomousRoamOpenCornerProbes", cornerCandidate);
            float openScore = (float)InvokeInstance(
                controller,
                "EvaluateAutonomousRoamDestinationScore",
                Vector2.zero,
                Vector2.zero,
                openCandidate,
                openCandidate);
            float cornerScore = (float)InvokeInstance(
                controller,
                "EvaluateAutonomousRoamDestinationScore",
                Vector2.zero,
                Vector2.zero,
                cornerCandidate,
                cornerCandidate);

            Assert.That(openCornerCount, Is.GreaterThan(cornerCornerCount));
            Assert.That(openScore, Is.GreaterThan(cornerScore));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(wallYGo);
            UnityEngine.Object.DestroyImmediate(wallXGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRejectAutonomousRoamBadPoint_WhenGapOnlyFitsBareFootprint()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_AutonomousBadPointGapTest");
        GameObject npcGo = new GameObject("NPC_AutonomousBadPointGapTest");
        GameObject leftWallGo = new GameObject("AutonomousGapWall_Left");
        GameObject rightWallGo = new GameObject("AutonomousGapWall_Right");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D leftWall = leftWallGo.AddComponent<BoxCollider2D>();
            leftWall.size = new Vector2(0.12f, 1.6f);
            leftWallGo.transform.position = new Vector2(-0.43f, 0f);

            BoxCollider2D rightWall = rightWallGo.AddComponent<BoxCollider2D>();
            rightWall.size = new Vector2(0.12f, 1.6f);
            rightWallGo.transform.position = new Vector2(0.43f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { leftWall, rightWall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);

            bool bodyClear = (bool)InvokeInstance(controller, "HasAutonomousRoamDestinationBodyClearance", Vector2.zero);
            bool comfortClear = (bool)InvokeInstance(controller, "HasAutonomousRoamDestinationComfortClearance", Vector2.zero);
            bool candidateClear = (bool)InvokeInstance(controller, "IsAutonomousRoamDestinationCandidateClear", Vector2.zero);

            Assert.That(bodyClear, Is.True);
            Assert.That(comfortClear, Is.False);
            Assert.That(candidateClear, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(rightWallGo);
            UnityEngine.Object.DestroyImmediate(leftWallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRejectFormalNavigationBadPoint_WhenDestinationGapIsNarrowerThanBody()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_FormalBadPointGapTest");
        GameObject npcGo = new GameObject("NPC_FormalBadPointGapTest");
        GameObject leftWallGo = new GameObject("FormalGapWall_Left");
        GameObject rightWallGo = new GameObject("FormalGapWall_Right");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D leftWall = leftWallGo.AddComponent<BoxCollider2D>();
            leftWall.size = new Vector2(0.12f, 1.6f);
            leftWallGo.transform.position = new Vector2(-0.32f, 0f);

            BoxCollider2D rightWall = rightWallGo.AddComponent<BoxCollider2D>();
            rightWall.size = new Vector2(0.12f, 1.6f);
            rightWallGo.transform.position = new Vector2(0.32f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { leftWall, rightWall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            npcGo.transform.position = new Vector2(0f, -1.2f);
            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);

            bool beganMove = (bool)InvokeInstance(
                controller,
                "BeginPathDirectedMove",
                Vector2.zero,
                Enum.Parse(travelContractType, "FormalNavigation"));

            Assert.That(beganMove, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(rightWallGo);
            UnityEngine.Object.DestroyImmediate(leftWallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldRejectRecoverablePointToPointBuiltPath_WhenSegmentClipsNarrowGap()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_ReleasedPathGapTest");
        GameObject npcGo = new GameObject("NPC_ReleasedPathGapTest");
        GameObject leftWallGo = new GameObject("ReleasedPathGapWall_Left");
        GameObject rightWallGo = new GameObject("ReleasedPathGapWall_Right");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D leftWall = leftWallGo.AddComponent<BoxCollider2D>();
            leftWall.size = new Vector2(0.12f, 1.6f);
            leftWallGo.transform.position = new Vector2(-0.32f, 0f);

            BoxCollider2D rightWall = rightWallGo.AddComponent<BoxCollider2D>();
            rightWall.size = new Vector2(0.12f, 1.6f);
            rightWallGo.transform.position = new Vector2(0.32f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { leftWall, rightWall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);

            object executionState = GetFieldOrProperty(controller, "navigationExecution");
            IList path = (IList)GetFieldOrProperty(executionState, "Path");
            path.Add(Vector2.zero);
            path.Add(new Vector2(0f, 1.1f));

            bool acceptable = (bool)InvokeInstance(
                controller,
                "IsRecoverablePointToPointBuiltPathAcceptable",
                new Vector2(0f, -1.2f),
                new Vector2(0f, 1.1f));

            Assert.That(acceptable, Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(rightWallGo);
            UnityEngine.Object.DestroyImmediate(leftWallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldCreateStaticRecoveryPath_WithOverrideWaypointAroundWall()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type travelContractType = ResolveTypeOrFail("NPCAutoRoamController+PointToPointTravelContract");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_StaticRecoveryPathTest");
        GameObject npcGo = new GameObject("NPC_StaticRecoveryPathTest");
        GameObject wallGo = new GameObject("StaticRecoveryWall");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D wall = wallGo.AddComponent<BoxCollider2D>();
            wall.size = new Vector2(0.18f, 0.9f);
            wallGo.transform.position = new Vector2(0.48f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { wall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);
            SetFieldOrProperty(controller, "debugMoveActive", true);
            SetFieldOrProperty(controller, "activePointToPointTravelContract", Enum.Parse(travelContractType, "FormalNavigation"));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(1.5f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(1.5f, 0f));

            bool createdRecovery = (bool)InvokeInstance(
                controller,
                "TryCreateStaticObstacleRecoveryPath",
                Vector2.zero,
                "ConstrainedZeroAdvance");

            object executionState = GetFieldOrProperty(controller, "navigationExecution");
            bool hasOverrideWaypoint = (bool)GetFieldOrProperty(executionState, "HasOverrideWaypoint");
            Vector2 overrideWaypoint = (Vector2)GetFieldOrProperty(executionState, "OverrideWaypoint");
            IList path = (IList)GetFieldOrProperty(executionState, "Path");
            Vector2 activeDestination = (Vector2)GetFieldOrProperty(executionState, "Destination");

            Assert.That(createdRecovery, Is.True);
            Assert.That(hasOverrideWaypoint, Is.True);
            Assert.That(Mathf.Abs(overrideWaypoint.y), Is.GreaterThan(0.05f));
            Assert.That(path.Count, Is.GreaterThan(0));
            Assert.That(activeDestination.x, Is.GreaterThan(1.2f));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(wallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
        }
    }

    [Test]
    public void NPCAutoRoamController_ShouldPreferStaticRecoveryPath_BeforeRecoveryHold_OnFirstStaticNearWallHardStop()
    {
        Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
        Type roamStateType = ResolveTypeOrFail("NPCAutoRoamController+RoamState");
        Type navGridType = ResolveTypeOrFail("NavGrid2D");
        Type tilemapType = ResolveTypeOrFail("UnityEngine.Tilemaps.Tilemap");
        GameObject navGridGo = new GameObject("NavGrid_StaticNearWallRecoveryPriorityTest");
        GameObject npcGo = new GameObject("NPC_StaticNearWallRecoveryPriorityTest");
        GameObject wallGo = new GameObject("StaticNearWallRecoveryPriorityWall");

        try
        {
            Component navGrid = navGridGo.AddComponent(navGridType);
            InvokeInstance(navGrid, "Awake");
            SetFieldOrProperty(navGrid, "autoDetectWorldBounds", false);
            SetFieldOrProperty(navGrid, "worldOrigin", new Vector2(-2f, -2f));
            SetFieldOrProperty(navGrid, "worldSize", new Vector2(4f, 4f));
            SetFieldOrProperty(navGrid, "cellSize", 0.25f);
            SetFieldOrProperty(navGrid, "probeRadius", 0.2f);

            BoxCollider2D wall = wallGo.AddComponent<BoxCollider2D>();
            wall.size = new Vector2(0.18f, 0.9f);
            wallGo.transform.position = new Vector2(0.48f, 0f);
            Physics2D.SyncTransforms();

            Array emptyTilemaps = Array.CreateInstance(tilemapType, 0);
            InvokeInstance(
                navGrid,
                "ConfigureExplicitObstacleSources",
                new Collider2D[] { wall },
                emptyTilemaps,
                false);
            InvokeInstance(navGrid, "RebuildGrid");

            Component controller = npcGo.AddComponent(controllerType);
            BoxCollider2D npcCollider = npcGo.AddComponent<BoxCollider2D>();
            npcCollider.size = new Vector2(0.4f, 0.4f);
            npcGo.transform.position = Vector2.zero;

            SetFieldOrProperty(controller, "navGrid", navGrid);
            SetFieldOrProperty(controller, "navigationCollider", npcCollider);
            SetFieldOrProperty(controller, "state", Enum.Parse(roamStateType, "Moving"));
            SetFieldOrProperty(controller, "homePosition", Vector2.zero);
            SetFieldOrProperty(controller, "activityRadius", 3f);
            SetFieldOrProperty(controller, "minimumMoveDistance", 0.6f);
            SetFieldOrProperty(controller, "currentDestination", new Vector2(1.5f, 0f));
            SetFieldOrProperty(controller, "requestedDestination", new Vector2(1.5f, 0f));
            SetFieldOrProperty(controller, "hasRequestedDestination", true);

            bool handled = (bool)InvokeInstance(
                controller,
                "TryHandleBlockedAdvance",
                Vector2.zero,
                "StaticNearWallHardStop");

            object executionState = GetFieldOrProperty(controller, "navigationExecution");
            bool hasOverrideWaypoint = (bool)GetFieldOrProperty(executionState, "HasOverrideWaypoint");

            Assert.That(handled, Is.True);
            Assert.That((bool)GetFieldOrProperty(controller, "autonomousRecoveryHoldActive"), Is.False);
            Assert.That(hasOverrideWaypoint, Is.True);
            Assert.That((string)GetFieldOrProperty(controller, "DebugLastMoveSkipReason"), Is.EqualTo("BlockedAdvanceStaticRecovery:StaticNearWallHardStop"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(wallGo);
            UnityEngine.Object.DestroyImmediate(npcGo);
            UnityEngine.Object.DestroyImmediate(navGridGo);
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
    public void NavigationPathExecutor_ShouldStampDetourMetadata_WhenOverrideWaypointIsSetDirectly()
    {
        Type executorType = ResolveTypeOrFail("NavigationPathExecutor2D");
        Type executionStateType = ResolveTypeOrFail("NavigationPathExecutor2D+ExecutionState");

        object state = Activator.CreateInstance(executionStateType);
        Vector2 detourPoint = new Vector2(1.2f, 0.4f);
        float beforeCreateTime = (float)GetFieldOrProperty(state, "LastDetourCreateTime");

        InvokeStatic(
            executorType,
            "SetOverrideWaypoint",
            state,
            detourPoint,
            9);

        Assert.That((bool)GetFieldOrProperty(state, "HasOverrideWaypoint"), Is.True);
        Assert.That((Vector2)GetFieldOrProperty(state, "OverrideWaypoint"), Is.EqualTo(detourPoint));
        Assert.That((int)GetFieldOrProperty(state, "OverrideWaypointOwnerId"), Is.EqualTo(9));
        Assert.That((int)GetFieldOrProperty(state, "LastDetourOwnerId"), Is.EqualTo(9));
        Assert.That((Vector2)GetFieldOrProperty(state, "LastDetourPoint"), Is.EqualTo(detourPoint));
        Assert.That((float)GetFieldOrProperty(state, "LastDetourCreateTime"), Is.GreaterThanOrEqualTo(beforeCreateTime));
        Assert.That((bool)GetFieldOrProperty(state, "LastDetourRecoverySucceeded"), Is.False);
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

    [Test]
    public void NavigationAgentRegistry_ShouldReturnOnlyActiveRegisteredRoamControllers()
    {
        GameObject activeA = new GameObject("RegistryNpc_A");
        GameObject activeB = new GameObject("RegistryNpc_B");
        GameObject inactive = new GameObject("RegistryNpc_Inactive");

        try
        {
            Type controllerType = ResolveTypeOrFail("PlayerAutoNavigator");
            Component controllerA = activeA.AddComponent(controllerType);
            Component controllerB = activeB.AddComponent(controllerType);
            Component inactiveController = inactive.AddComponent(controllerType);

            Type registryType = ResolveTypeOrFail("NavigationAgentRegistry");
            MethodInfo registerMethod = registryType.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(registerMethod, "未找到 NavigationAgentRegistry.Register");
            registerMethod.Invoke(null, new object[] { controllerA });
            registerMethod.Invoke(null, new object[] { controllerB });
            registerMethod.Invoke(null, new object[] { inactiveController });

            ((Behaviour)inactiveController).enabled = false;

            Type bufferType = typeof(List<>).MakeGenericType(controllerType);
            IList buffer = (IList)Activator.CreateInstance(bufferType);
            MethodInfo registryMethod = registryType
                .GetMethod("GetRegisteredUnits", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(controllerType);
            Assert.IsNotNull(registryMethod, "未找到 NavigationAgentRegistry.GetRegisteredUnits<T>");
            registryMethod.Invoke(null, new object[] { buffer });

            Assert.That(buffer.Contains(controllerA), Is.True);
            Assert.That(buffer.Contains(controllerB), Is.True);
            Assert.That(buffer.Contains(inactiveController), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(inactive);
            UnityEngine.Object.DestroyImmediate(activeB);
            UnityEngine.Object.DestroyImmediate(activeA);
        }
    }

    [Test]
    public void NavigationAgentRegistry_ShouldRefreshCachedUnits_WhenControllerDisablesWithinSameFrame()
    {
        GameObject active = new GameObject("RegistryNpc_CachedActive");

        try
        {
            Type controllerType = ResolveTypeOrFail("PlayerAutoNavigator");
            Component controller = active.AddComponent(controllerType);

            Type registryType = ResolveTypeOrFail("NavigationAgentRegistry");
            MethodInfo registerMethod = registryType.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(registerMethod, "未找到 NavigationAgentRegistry.Register");
            registerMethod.Invoke(null, new object[] { controller });

            Type bufferType = typeof(List<>).MakeGenericType(controllerType);
            IList buffer = (IList)Activator.CreateInstance(bufferType);
            MethodInfo registryMethod = registryType
                .GetMethod("GetRegisteredUnits", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(controllerType);
            Assert.IsNotNull(registryMethod, "未找到 NavigationAgentRegistry.GetRegisteredUnits<T>");

            registryMethod.Invoke(null, new object[] { buffer });
            Assert.That(buffer.Contains(controller), Is.True);

            ((Behaviour)controller).enabled = false;
            buffer.Clear();
            registryMethod.Invoke(null, new object[] { buffer });

            Assert.That(buffer.Contains(controller), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(active);
        }
    }

    [Test]
    public void NavigationAgentRegistry_ShouldReturnNearbySnapshots_WithoutIncludingSelf()
    {
        GameObject selfGo = new GameObject("RegistrySnapshot_Self");
        GameObject nearbyGo = new GameObject("RegistrySnapshot_Nearby");
        GameObject farGo = new GameObject("RegistrySnapshot_Far");

        try
        {
            Type controllerType = ResolveTypeOrFail("NPCAutoRoamController");
            Type registryType = ResolveTypeOrFail("NavigationAgentRegistry");
            Type snapshotType = ResolveTypeOrFail("NavigationAgentSnapshot");
            Component selfController = selfGo.AddComponent(controllerType);
            Component nearbyController = nearbyGo.AddComponent(controllerType);
            Component farController = farGo.AddComponent(controllerType);

            selfGo.transform.position = Vector2.zero;
            nearbyGo.transform.position = new Vector2(0.45f, 0f);
            farGo.transform.position = new Vector2(40f, 0f);

            MethodInfo registerMethod = registryType.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(registerMethod, "未找到 NavigationAgentRegistry.Register");
            registerMethod.Invoke(null, new object[] { selfController });
            registerMethod.Invoke(null, new object[] { nearbyController });
            registerMethod.Invoke(null, new object[] { farController });

            Type bufferType = typeof(List<>).MakeGenericType(snapshotType);
            IList buffer = (IList)Activator.CreateInstance(bufferType);
            MethodInfo nearbyMethod = registryType.GetMethod("GetNearbySnapshots", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(nearbyMethod, "未找到 NavigationAgentRegistry.GetNearbySnapshots");

            nearbyMethod.Invoke(null, new object[] { selfController, Vector2.zero, 1f, buffer });

            List<int> snapshotIds = new List<int>(buffer.Count);
            for (int index = 0; index < buffer.Count; index++)
            {
                snapshotIds.Add((int)GetFieldOrProperty(buffer[index], "InstanceId"));
            }

            Assert.That(snapshotIds.Count, Is.EqualTo(1));
            Assert.That(snapshotIds, Contains.Item(nearbyController.GetInstanceID()));
            Assert.That(snapshotIds, Has.No.Member(selfController.GetInstanceID()));
            Assert.That(snapshotIds, Has.No.Member(farController.GetInstanceID()));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(farGo);
            UnityEngine.Object.DestroyImmediate(nearbyGo);
            UnityEngine.Object.DestroyImmediate(selfGo);
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

    private static void SetStaticField(Type type, string name, object value)
    {
        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(field, $"未找到静态字段：{type.Name}.{name}");
        field.SetValue(null, value);
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
