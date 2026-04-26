using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class ChestPlacementGridTests
{
    private const string Box1PrefabPath = "Assets/222_Prefabs/Box/Box_1.prefab";
    private const string Box2PrefabPath = "Assets/222_Prefabs/Box/Box_2.prefab";

    private static readonly string ProjectRoot =
        Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", ".."));

    private static readonly string ChestControllerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/World/Placeable/ChestController.cs");

    [TestCase(Box1PrefabPath, 10.5f, 20.5f)]
    [TestCase(Box2PrefabPath, 10.5f, 20.5f)]
    public void PlacementPosition_ShouldKeepChestColliderCenteredOnTargetGrid(string prefabPath, float x, float y)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.That(prefab, Is.Not.Null, $"未找到箱子预制体：{prefabPath}");

        Vector3 mouseGridCenter = new Vector3(x, y, -3f);
        Vector2Int gridSize = CallGetRequiredGridSizeFromPrefab(prefab);
        Vector2 expectedGridCenter = GetExpectedGridGeometricCenter(mouseGridCenter, gridSize);
        Vector2 colliderCenter = GetColliderCenterRelativeToPrefabRoot(prefab);
        Vector3 placementPosition = CallGetPlacementPosition(mouseGridCenter, prefab);
        Vector2 actualColliderCenter = (Vector2)placementPosition + colliderCenter;

        Assert.That(actualColliderCenter.x, Is.EqualTo(expectedGridCenter.x).Within(0.0001f));
        Assert.That(actualColliderCenter.y, Is.EqualTo(expectedGridCenter.y).Within(0.0001f));
    }

    [TestCase(Box1PrefabPath, 6.5f, 8.5f)]
    [TestCase(Box2PrefabPath, 6.5f, 8.5f)]
    public void ReachEnvelope_ShouldStayAlignedWithChestColliderFootprint(string prefabPath, float x, float y)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.That(prefab, Is.Not.Null, $"未找到箱子预制体：{prefabPath}");

        Vector3 mouseGridCenter = new Vector3(x, y, -2f);
        Vector2Int gridSize = CallGetRequiredGridSizeFromPrefab(prefab);
        Vector2 expectedGridCenter = GetExpectedGridGeometricCenter(mouseGridCenter, gridSize);

        bool success = CallTryGetPlacementReachEnvelopeBounds(mouseGridCenter, prefab, out Bounds bounds);
        Assert.That(success, Is.True, $"未能为 {prefabPath} 计算 reach envelope");
        Assert.That(bounds.center.x, Is.EqualTo(expectedGridCenter.x).Within(0.0001f));
        Assert.That(bounds.center.y, Is.EqualTo(expectedGridCenter.y).Within(0.0001f));
    }

    [TestCase(Box1PrefabPath, 3.5f, 4.5f)]
    [TestCase(Box2PrefabPath, 3.5f, 4.5f)]
    public void PreviewLocalPosition_ShouldMatchActualPlacedVisualOrigin(string prefabPath, float x, float y)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Assert.That(prefab, Is.Not.Null, $"未找到箱子预制体：{prefabPath}");

        Vector3 mouseGridCenter = new Vector3(x, y, -1f);
        Vector3 previewLocalPosition = CallGetPreviewSpriteLocalPosition(prefab);
        Vector3 placementPosition = CallGetPlacementPosition(mouseGridCenter, prefab);

        Assert.That(mouseGridCenter.x + previewLocalPosition.x, Is.EqualTo(placementPosition.x).Within(0.0001f));
        Assert.That(mouseGridCenter.y + previewLocalPosition.y, Is.EqualTo(placementPosition.y).Within(0.0001f));
    }

    [Test]
    public void ChestPush_ShouldUseColliderCastInsteadOfOverlapCircleGuess()
    {
        string scriptText = File.ReadAllText(ChestControllerPath);

        StringAssert.Contains("activeCollider.Cast(pushDir, s_pushContactFilter, s_pushCastHits, distance)", scriptText);
        Assert.False(scriptText.Contains("Physics2D.OverlapCircleAll(targetPos, collisionCheckRadius)"));
    }

    private static Vector2Int CallGetRequiredGridSizeFromPrefab(GameObject prefab)
    {
        return (Vector2Int)InvokePlacementGridCalculator("GetRequiredGridSizeFromPrefab", prefab);
    }

    private static Vector3 CallGetPlacementPosition(Vector3 mouseGridCenter, GameObject prefab)
    {
        return (Vector3)InvokePlacementGridCalculator("GetPlacementPosition", mouseGridCenter, prefab);
    }

    private static Vector3 CallGetPreviewSpriteLocalPosition(GameObject prefab)
    {
        return (Vector3)InvokePlacementGridCalculator("GetPreviewSpriteLocalPosition", prefab);
    }

    private static bool CallTryGetPlacementReachEnvelopeBounds(Vector3 mouseGridCenter, GameObject prefab, out Bounds bounds)
    {
        Type calculatorType = ResolvePlacementGridCalculatorType();
        MethodInfo method = calculatorType.GetMethod("TryGetPlacementReachEnvelopeBounds", BindingFlags.Public | BindingFlags.Static);
        Assert.That(method, Is.Not.Null, "未找到 PlacementGridCalculator.TryGetPlacementReachEnvelopeBounds");

        object[] args = { mouseGridCenter, prefab, default(Bounds) };
        bool success = (bool)method.Invoke(null, args);
        bounds = (Bounds)args[2];
        return success;
    }

    private static object InvokePlacementGridCalculator(string methodName, params object[] args)
    {
        Type calculatorType = ResolvePlacementGridCalculatorType();
        MethodInfo method = calculatorType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        Assert.That(method, Is.Not.Null, $"未找到 PlacementGridCalculator.{methodName}");
        return method.Invoke(null, args);
    }

    private static Type ResolvePlacementGridCalculatorType()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType("PlacementGridCalculator");
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail("当前 AppDomain 中未找到 PlacementGridCalculator 类型");
        return null;
    }

    private static Vector2 GetExpectedGridGeometricCenter(Vector3 mouseGridCenter, Vector2Int gridSize)
    {
        float gridCenterOffsetX = (gridSize.x % 2 == 0) ? 0.5f : 0f;
        float gridCenterOffsetY = (gridSize.y % 2 == 0) ? 0.5f : 0f;
        return new Vector2(mouseGridCenter.x + gridCenterOffsetX, mouseGridCenter.y + gridCenterOffsetY);
    }

    private static Vector2 GetColliderCenterRelativeToPrefabRoot(GameObject prefab)
    {
        Collider2D collider = prefab.GetComponentInChildren<Collider2D>(true);
        Assert.That(collider, Is.Not.Null, $"预制体 {prefab.name} 缺少 Collider2D");

        Transform root = prefab.transform;
        Vector2 relativePosition = (Vector2)collider.transform.position - (Vector2)root.position;
        Vector2 scale = new Vector2(Mathf.Abs(collider.transform.lossyScale.x), Mathf.Abs(collider.transform.lossyScale.y));

        if (collider is PolygonCollider2D polygonCollider && polygonCollider.pathCount > 0)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] path = polygonCollider.GetPath(pathIndex);
                for (int pointIndex = 0; pointIndex < path.Length; pointIndex++)
                {
                    Vector2 localPoint = path[pointIndex] + polygonCollider.offset;
                    min = Vector2.Min(min, localPoint);
                    max = Vector2.Max(max, localPoint);
                }
            }

            Vector2 scaledMin = relativePosition + Vector2.Scale(min, scale);
            Vector2 scaledMax = relativePosition + Vector2.Scale(max, scale);
            return (scaledMin + scaledMax) * 0.5f;
        }

        if (collider is BoxCollider2D boxCollider)
        {
            return relativePosition + Vector2.Scale(boxCollider.offset, scale);
        }

        Assert.Fail($"测试暂未覆盖的 Collider 类型：{collider.GetType().Name}");
        return Vector2.zero;
    }
}
