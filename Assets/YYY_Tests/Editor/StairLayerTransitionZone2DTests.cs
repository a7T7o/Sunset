using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class StairLayerTransitionZone2DTests
{
    private const string TopUnityLayerName = "Layer 2";
    private const string TopSortingLayerName = "Layer 2";

    private GameObject zoneObject;
    private MonoBehaviour zone;
    private Type zoneType;

    [SetUp]
    public void SetUp()
    {
        zoneObject = new GameObject("StairZone");
        BoxCollider2D zoneCollider = zoneObject.AddComponent<BoxCollider2D>();
        zoneCollider.isTrigger = true;
        zoneCollider.size = new Vector2(2f, 2f);
        zoneType = Type.GetType("StairLayerTransitionZone2D, Assembly-CSharp");
        Assert.IsNotNull(zoneType, "未找到 StairLayerTransitionZone2D 类型");
        zone = (MonoBehaviour)zoneObject.AddComponent(zoneType);
    }

    [TearDown]
    public void TearDown()
    {
        if (zoneObject != null)
        {
            UnityEngine.Object.DestroyImmediate(zoneObject);
        }

        GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i] != null && allObjects[i].name.StartsWith("PlayerRoot", StringComparison.Ordinal))
            {
                UnityEngine.Object.DestroyImmediate(allObjects[i]);
            }
        }
    }

    [Test]
    public void ApplyTransitionForExit_UsesPlayerRootColliderInsteadOfChildToolCollider()
    {
        GameObject playerRoot = CreatePlayerRoot();
        Collider2D bodyCollider = playerRoot.AddComponent<BoxCollider2D>();
        ((BoxCollider2D)bodyCollider).size = new Vector2(0.6f, 0.4f);
        playerRoot.transform.position = new Vector3(0f, 1.25f, 0f);

        GameObject toolChild = new GameObject("ToolChild");
        toolChild.transform.SetParent(playerRoot.transform, false);
        toolChild.transform.localPosition = new Vector3(-2f, 0.2f, 0f);
        BoxCollider2D toolCollider = toolChild.AddComponent<BoxCollider2D>();
        toolCollider.size = new Vector2(0.2f, 0.2f);

        SpriteRenderer bodyRenderer = playerRoot.AddComponent<SpriteRenderer>();
        bodyRenderer.sortingLayerName = "Layer 1";

        GameObject sortingChild = new GameObject("SortingChild");
        sortingChild.transform.SetParent(playerRoot.transform, false);
        SortingGroup sortingGroup = sortingChild.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Layer 1";

        SetExitTarget("topExitTarget", TopUnityLayerName, TopSortingLayerName);
        InvokeInstance("ApplyTransitionForExit", toolCollider, playerRoot);

        int expectedUnityLayer = LayerMask.NameToLayer(TopUnityLayerName);
        Assert.AreEqual(expectedUnityLayer, playerRoot.layer, "应按玩家本体 Collider 判定上下边界，而不是被左侧工具碰撞体带偏。");
        Assert.AreEqual(expectedUnityLayer, toolChild.layer, "切层应同步到子物体。");
        Assert.AreEqual(TopSortingLayerName, bodyRenderer.sortingLayerName, "SpriteRenderer 的 Sorting Layer 应同步更新。");
        Assert.AreEqual(TopSortingLayerName, sortingGroup.sortingLayerName, "SortingGroup 的 Sorting Layer 应同步更新。");
    }

    [Test]
    public void ApplyTransitionForExit_IgnoresSideExit()
    {
        GameObject playerRoot = CreatePlayerRoot();
        BoxCollider2D bodyCollider = playerRoot.AddComponent<BoxCollider2D>();
        ((BoxCollider2D)bodyCollider).size = new Vector2(0.4f, 0.4f);
        playerRoot.transform.position = new Vector3(-1.2f, 0f, 0f);

        SpriteRenderer bodyRenderer = playerRoot.AddComponent<SpriteRenderer>();
        bodyRenderer.sortingLayerName = "Layer 1";

        SetExitTarget("topExitTarget", TopUnityLayerName, TopSortingLayerName);
        InvokeInstance("ApplyTransitionForExit", bodyCollider, playerRoot);

        Assert.AreEqual(0, playerRoot.layer, "从楼梯左右离开时不应切换 Unity Layer。");
        Assert.AreEqual("Layer 1", bodyRenderer.sortingLayerName, "从楼梯左右离开时不应切换 Sorting Layer。");
    }

    private static GameObject CreatePlayerRoot()
    {
        return new GameObject("PlayerRoot");
    }

    private void SetExitTarget(string fieldName, string unityLayerName, string sortingLayerName)
    {
        FieldInfo targetField = zoneType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(targetField, $"未找到字段 {fieldName}");

        Type configType = zoneType.GetNestedType("ExitTargetConfig", BindingFlags.NonPublic);
        Assert.IsNotNull(configType, "未找到 ExitTargetConfig");

        object config = Activator.CreateInstance(configType);
        configType.GetField("unityLayerName", BindingFlags.Public | BindingFlags.Instance)?.SetValue(config, unityLayerName);
        configType.GetField("sortingLayerName", BindingFlags.Public | BindingFlags.Instance)?.SetValue(config, sortingLayerName);
        targetField.SetValue(zone, config);
    }

    private object InvokeInstance(string methodName, params object[] args)
    {
        MethodInfo method = zoneType.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"未找到方法 {methodName}");
        return method.Invoke(zone, args);
    }
}
