#if UNITY_INCLUDE_TESTS
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlacementManagerAdjacentIntentTests
{
    private static readonly MethodInfo BuildDirectionsMethod = typeof(PlacementManager).GetMethod(
        "BuildAdjacentIntentDirections",
        BindingFlags.NonPublic | BindingFlags.Static);

    [Test]
    public void InteriorPointer_DoesNotBiasToAdjacentCell()
    {
        Vector2Int[] directions = InvokeDirections(new Vector3(5.5f, 5.5f, 0f), new Vector3(5.89f, 5.5f, 0f));

        Assert.That(directions, Is.Empty);
    }

    [Test]
    public void EdgeBandPointer_BiasesToSingleAdjacentAxis()
    {
        Vector2Int[] directions = InvokeDirections(new Vector3(5.5f, 5.5f, 0f), new Vector3(5.91f, 5.5f, 0f));

        CollectionAssert.AreEqual(new[] { Vector2Int.right }, directions);
    }

    [Test]
    public void EdgeBandPointer_AtExactTenPercentBoundaryStillBiases()
    {
        Vector2Int[] directions = InvokeDirections(new Vector3(5.5f, 5.5f, 0f), new Vector3(5.9f, 5.5f, 0f));

        CollectionAssert.AreEqual(new[] { Vector2Int.right }, directions);
    }

    [Test]
    public void CornerPointer_PrefersDiagonalThenDeeperAxis()
    {
        Vector2Int[] directions = InvokeDirections(new Vector3(5.5f, 5.5f, 0f), new Vector3(5.95f, 5.91f, 0f));

        CollectionAssert.AreEqual(
            new[]
            {
                new Vector2Int(1, 1),
                Vector2Int.right,
                Vector2Int.up
            },
            directions);
    }

    private static Vector2Int[] InvokeDirections(Vector3 cellCenter, Vector3 mouseWorldPosition)
    {
        Assert.NotNull(BuildDirectionsMethod, "无法反射到 BuildAdjacentIntentDirections，说明连续放置边界语义入口被改名或移除。");
        return (Vector2Int[])BuildDirectionsMethod.Invoke(null, new object[] { cellCenter, mouseWorldPosition });
    }
}
#endif
