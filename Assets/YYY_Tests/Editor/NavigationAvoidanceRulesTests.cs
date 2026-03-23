using NUnit.Framework;
using UnityEngine;

public class NavigationAvoidanceRulesTests
{
    [Test]
    public void AutoNavigationPlayer_ShouldYield_ToMovingNpc()
    {
        NavigationAgentSnapshot player = new NavigationAgentSnapshot(
            1,
            NavigationUnitType.Player,
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        NavigationAgentSnapshot npc = new NavigationAgentSnapshot(
            2,
            NavigationUnitType.NPC,
            new Vector2(1f, 0f),
            Vector2.left,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        Assert.That(NavigationAvoidanceRules.ShouldYield(player, npc), Is.True);
        Assert.That(NavigationAvoidanceRules.ShouldYield(npc, player), Is.False);
    }

    [Test]
    public void Solver_ShouldProduceLateralOrBackwardBias_WhenPlayerFacesMovingNpc()
    {
        NavigationAgentSnapshot player = new NavigationAgentSnapshot(
            1,
            NavigationUnitType.Player,
            Vector2.zero,
            Vector2.right,
            0.3f,
            0.6f,
            100,
            true,
            false,
            true);

        NavigationAgentSnapshot npc = new NavigationAgentSnapshot(
            2,
            NavigationUnitType.NPC,
            new Vector2(0.7f, 0.05f),
            Vector2.zero,
            0.3f,
            0.6f,
            50,
            true,
            false,
            true);

        NavigationLocalAvoidanceSolver.AvoidanceResult result =
            NavigationLocalAvoidanceSolver.Solve(
                player,
                Vector2.right,
                1f,
                new[] { npc });

        Assert.That(result.HasBlockingAgent, Is.True);
        Assert.That(result.AdjustedDirection.x, Is.LessThan(0.99f));
        Assert.That(Mathf.Abs(result.AdjustedDirection.y), Is.GreaterThan(0.01f));
    }
}
