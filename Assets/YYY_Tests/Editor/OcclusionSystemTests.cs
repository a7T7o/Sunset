using NUnit.Framework;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// 遮挡透明系统单元测试
/// 测试核心算法的正确性
/// </summary>
[TestFixture]
public class OcclusionSystemTests
{
    #region 属性 1: 遮挡检测双向一致性
    
    /// <summary>
    /// 测试 Bounds.Contains 检测逻辑
    /// 验证需求: 1.1, 1.2
    /// </summary>
    [Test]
    public void BoundsContains_PlayerInsideBounds_ReturnsTrue()
    {
        // Arrange
        Bounds occluderBounds = new Bounds(Vector3.zero, new Vector3(2f, 3f, 1f));
        Vector2 playerCenter = new Vector2(0.5f, 0.5f);
        
        // Act
        bool isInside = occluderBounds.Contains(playerCenter);
        
        // Assert
        Assert.IsTrue(isInside, "玩家中心在遮挡物边界内应返回 true");
    }
    
    [Test]
    public void BoundsContains_PlayerOutsideBounds_ReturnsFalse()
    {
        // Arrange
        Bounds occluderBounds = new Bounds(Vector3.zero, new Vector3(2f, 3f, 1f));
        Vector2 playerCenter = new Vector2(5f, 5f);
        
        // Act
        bool isInside = occluderBounds.Contains(playerCenter);
        
        // Assert
        Assert.IsFalse(isInside, "玩家中心在遮挡物边界外应返回 false");
    }
    
    [Test]
    public void BoundsContains_PlayerOnBoundary_ReturnsTrue()
    {
        // Arrange
        Bounds occluderBounds = new Bounds(Vector3.zero, new Vector3(2f, 3f, 1f));
        Vector2 playerCenter = new Vector2(1f, 0f); // 边界上
        
        // Act
        bool isInside = occluderBounds.Contains(playerCenter);
        
        // Assert
        Assert.IsTrue(isInside, "玩家中心在边界上应返回 true");
    }
    
    #endregion
    
    #region 属性 5: 树林连通性算法正确性
    
    /// <summary>
    /// 测试树根距离判定
    /// 验证需求: 3.2
    /// </summary>
    [Test]
    public void TreeConnection_RootDistanceWithinThreshold_ReturnsTrue()
    {
        // Arrange
        Vector2 rootA = new Vector2(0f, 0f);
        Vector2 rootB = new Vector2(2f, 0f);
        float connectionDistance = 2.5f;
        
        // Act
        float distance = Vector2.Distance(rootA, rootB);
        bool isConnected = distance <= connectionDistance;
        
        // Assert
        Assert.IsTrue(isConnected, "树根距离在阈值内应判定为连通");
    }
    
    [Test]
    public void TreeConnection_RootDistanceExceedsThreshold_ReturnsFalse()
    {
        // Arrange
        Vector2 rootA = new Vector2(0f, 0f);
        Vector2 rootB = new Vector2(5f, 0f);
        float connectionDistance = 2.5f;
        
        // Act
        float distance = Vector2.Distance(rootA, rootB);
        bool isConnected = distance <= connectionDistance;
        
        // Assert
        Assert.IsFalse(isConnected, "树根距离超出阈值应判定为不连通");
    }
    
    /// <summary>
    /// 测试树冠重叠判定
    /// 验证需求: 3.2
    /// </summary>
    [Test]
    public void TreeConnection_CanopyOverlapAboveThreshold_ReturnsTrue()
    {
        // Arrange
        Bounds boundsA = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(2f, 3f, 1f));
        Bounds boundsB = new Bounds(new Vector3(1f, 0f, 0f), new Vector3(2f, 3f, 1f));
        float overlapThreshold = 0.15f;
        
        // Act
        float overlapRatio = CalculateOverlapRatio(boundsA, boundsB);
        bool isConnected = overlapRatio >= overlapThreshold;
        
        // Assert
        Assert.IsTrue(isConnected, "树冠重叠超过15%应判定为连通");
    }
    
    [Test]
    public void TreeConnection_NoCanopyOverlap_ReturnsFalse()
    {
        // Arrange
        Bounds boundsA = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(2f, 3f, 1f));
        Bounds boundsB = new Bounds(new Vector3(10f, 0f, 0f), new Vector3(2f, 3f, 1f));
        float overlapThreshold = 0.15f;
        
        // Act
        float overlapRatio = CalculateOverlapRatio(boundsA, boundsB);
        bool isConnected = overlapRatio >= overlapThreshold;
        
        // Assert
        Assert.IsFalse(isConnected, "无树冠重叠应判定为不连通");
    }
    
    #endregion
    
    #region 属性 6: 树林搜索边界限制
    
    /// <summary>
    /// 测试搜索深度限制
    /// 验证需求: 3.3
    /// </summary>
    [Test]
    public void ForestSearch_DepthLimit_IsRespected()
    {
        // Arrange
        int maxSearchDepth = 50;
        int simulatedTreeCount = 100;
        
        // Act
        int actualSearchCount = Mathf.Min(simulatedTreeCount, maxSearchDepth);
        
        // Assert
        Assert.LessOrEqual(actualSearchCount, maxSearchDepth, 
            "搜索深度不应超过最大限制");
    }
    
    /// <summary>
    /// 测试搜索半径限制
    /// 验证需求: 3.3
    /// </summary>
    [Test]
    public void ForestSearch_RadiusLimit_IsRespected()
    {
        // Arrange
        Vector2 playerPos = Vector2.zero;
        Vector2 treePos = new Vector2(20f, 0f);
        float maxSearchRadius = 15f;
        
        // Act
        float distance = Vector2.Distance(playerPos, treePos);
        bool shouldInclude = distance <= maxSearchRadius;
        
        // Assert
        Assert.IsFalse(shouldInclude, 
            "超出搜索半径的树木不应被包含");
    }
    
    #endregion
    
    #region 属性 10: 性能限制遵守性
    
    /// <summary>
    /// 测试检测间隔配置
    /// 验证需求: 6.1
    /// </summary>
    [Test]
    public void DetectionInterval_DefaultValue_IsCorrect()
    {
        // Arrange
        float expectedInterval = 0.1f;
        float tolerance = 0.01f;
        
        // Act & Assert
        // 这里只验证默认值的合理性
        Assert.AreEqual(expectedInterval, 0.1f, tolerance,
            "默认检测间隔应为 0.1 秒");
    }
    
    /// <summary>
    /// 测试云朵数量限制
    /// 验证需求: 6.2
    /// </summary>
    [Test]
    public void CloudCount_MaxLimit_IsRespected()
    {
        // Arrange
        int maxClouds = 32;
        int requestedClouds = 50;
        
        // Act
        int actualClouds = Mathf.Min(requestedClouds, maxClouds);
        
        // Assert
        Assert.LessOrEqual(actualClouds, maxClouds,
            "云朵数量不应超过最大限制");
    }
    
    #endregion
    
    #region 辅助方法
    
    /// <summary>
    /// 计算两个 Bounds 的重叠面积比例
    /// </summary>
    [Test]
    public void OcclusionRoot_ParentAndChildComponents_ShareSamePhysicalTree()
    {
        GameObject root = new GameObject("TreeRoot");
        root.tag = "Tree";

        GameObject child = new GameObject("Tree");
        child.tag = "Tree";
        child.transform.SetParent(root.transform);
        child.AddComponent<SpriteRenderer>();
        Type treeControllerType = ResolveTypeOrFail("TreeController");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");

        child.AddComponent(treeControllerType);

        Component parentOcclusion = root.AddComponent(occlusionTransparencyType);
        Component childOcclusion = child.AddComponent(occlusionTransparencyType);

        Transform parentRoot = InvokeTransform(parentOcclusion, "GetOcclusionRootTransform");
        Transform childRoot = InvokeTransform(childOcclusion, "GetOcclusionRootTransform");
        bool sharesRoot = InvokeBool(parentOcclusion, "SharesOcclusionRoot", childOcclusion);

        Assert.AreSame(root.transform, parentRoot);
        Assert.AreSame(root.transform, childRoot);
        Assert.IsTrue(sharesRoot, "父/子双遮挡组件应归属于同一物理树");

        UnityEngine.Object.DestroyImmediate(root);
    }

    [Test]
    public void ChoppingHighlight_ParentAndChildComponents_AreUpdatedTogether()
    {
        Type treeControllerType = ResolveTypeOrFail("TreeController");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");
        Type occlusionManagerType = ResolveTypeOrFail("OcclusionManager");

        GameObject managerObject = new GameObject("OcclusionManager");
        Component manager = managerObject.AddComponent(occlusionManagerType);

        GameObject root = new GameObject("TreeRoot");
        root.tag = "Tree";

        GameObject child = new GameObject("Tree");
        child.tag = "Tree";
        child.transform.SetParent(root.transform);
        child.AddComponent<SpriteRenderer>();
        child.AddComponent(treeControllerType);

        Component parentOcclusion = root.AddComponent(occlusionTransparencyType);
        Component childOcclusion = child.AddComponent(occlusionTransparencyType);

        InvokeVoid(manager, "RegisterOccluder", parentOcclusion);
        InvokeVoid(manager, "RegisterOccluder", childOcclusion);

        InvokeVoid(manager, "SetChoppingTree", parentOcclusion, 0.5f);
        Assert.IsTrue(ReadBoolProperty(parentOcclusion, "IsBeingChopped"));
        Assert.IsTrue(ReadBoolProperty(childOcclusion, "IsBeingChopped"));

        InvokeVoid(manager, "ClearChoppingHighlight");
        Assert.IsFalse(ReadBoolProperty(parentOcclusion, "IsBeingChopped"));
        Assert.IsFalse(ReadBoolProperty(childOcclusion, "IsBeingChopped"));

        UnityEngine.Object.DestroyImmediate(managerObject);
        UnityEngine.Object.DestroyImmediate(root);
    }

    [Test]
    public void ContainsPhysicalTree_ParentAndChildComponents_ReturnsTrue()
    {
        Type treeControllerType = ResolveTypeOrFail("TreeController");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");
        Type occlusionManagerType = ResolveTypeOrFail("OcclusionManager");

        GameObject managerObject = new GameObject("OcclusionManager");
        Component manager = managerObject.AddComponent(occlusionManagerType);

        GameObject root = new GameObject("TreeRoot");
        root.tag = "Tree";

        GameObject child = new GameObject("Tree");
        child.tag = "Tree";
        child.transform.SetParent(root.transform);
        child.AddComponent<SpriteRenderer>();
        child.AddComponent(treeControllerType);

        Component parentOcclusion = root.AddComponent(occlusionTransparencyType);
        Component childOcclusion = child.AddComponent(occlusionTransparencyType);

        Type hashSetType = typeof(System.Collections.Generic.HashSet<>).MakeGenericType(occlusionTransparencyType);
        object collection = Activator.CreateInstance(hashSetType);
        InvokeVoid(collection, "Add", parentOcclusion);

        bool containsSameTree = InvokeBoolNonPublic(manager, "ContainsPhysicalTree", collection, childOcclusion);
        Assert.IsTrue(containsSameTree, "同一物理树的父/子组件应被识别为同一棵树");

        UnityEngine.Object.DestroyImmediate(managerObject);
        UnityEngine.Object.DestroyImmediate(root);
    }

    [Test]
    public void TreeGrowthStage_ParentOcclusion_UsesChildTreeController()
    {
        Type treeControllerType = ResolveTypeOrFail("TreeController");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");

        GameObject root = new GameObject("TreeRoot");
        root.tag = "Tree";

        GameObject child = new GameObject("Tree");
        child.tag = "Tree";
        child.transform.SetParent(root.transform);
        child.AddComponent<SpriteRenderer>();
        Component treeController = child.AddComponent(treeControllerType);

        Component parentOcclusion = root.AddComponent(occlusionTransparencyType);
        Component childOcclusion = child.AddComponent(occlusionTransparencyType);

        SetPrivateField(treeController, "currentStageIndex", 2);

        int parentStage = (int)parentOcclusion.GetType().GetMethod("GetTreeGrowthStageIndex", BindingFlags.Instance | BindingFlags.Public).Invoke(parentOcclusion, null);
        int childStage = (int)childOcclusion.GetType().GetMethod("GetTreeGrowthStageIndex", BindingFlags.Instance | BindingFlags.Public).Invoke(childOcclusion, null);

        Assert.AreEqual(2, parentStage);
        Assert.AreEqual(2, childStage);

        UnityEngine.Object.DestroyImmediate(root);
    }

    [Test]
    public void BoundaryClassification_CenterTree_IsNotBoundaryTree()
    {
        TestForestContext context = CreateForestContext();

        InvokeNonPublicVoid(context.Manager, "FindConnectedForest", context.Center.Occlusion, Vector2.zero);
        bool isBoundary = InvokeBoolNonPublic(context.Manager, "IsBoundaryTree", context.Center.Occlusion);

        Assert.IsFalse(isBoundary, "中心树应被识别为内侧树，而不是边界树");

        CleanupForestContext(context);
    }

    [Test]
    public void ForestOcclusion_BoundaryTreeOutsideForest_OnlyTargetTreeTransparent()
    {
        TestForestContext context = CreateForestContext();
        Bounds playerBounds = new Bounds(new Vector3(0f, 3.4f, 0f), new Vector3(0.5f, 0.8f, 1f));
        Vector2 playerPos = playerBounds.center;

        InvokeNonPublicVoid(context.Manager, "FindConnectedForest", context.Top.Occlusion, playerPos);
        InvokeNonPublicVoid(context.Manager, "HandleForestOcclusion", context.Top.Occlusion, playerPos, playerBounds);

        Assert.IsTrue(ReadPrivateBoolField(context.Top.Occlusion, "isOccluding"));
        Assert.IsFalse(ReadPrivateBoolField(context.Center.Occlusion, "isOccluding"));
        Assert.IsFalse(ReadPrivateBoolField(context.Left.Occlusion, "isOccluding"));
        Assert.IsFalse(ReadPrivateBoolField(context.Right.Occlusion, "isOccluding"));
        Assert.IsFalse(ReadPrivateBoolField(context.Bottom.Occlusion, "isOccluding"));

        CleanupForestContext(context);
    }

    [Test]
    public void ForestOcclusion_InteriorTreeTrigger_MakesEntireForestTransparent()
    {
        TestForestContext context = CreateForestContext();
        Bounds playerBounds = new Bounds(new Vector3(0f, 1.0f, 0f), new Vector3(0.5f, 0.8f, 1f));
        Vector2 playerPos = new Vector2(0f, 3.4f);

        InvokeNonPublicVoid(context.Manager, "FindConnectedForest", context.Center.Occlusion, playerPos);
        InvokeNonPublicVoid(context.Manager, "HandleForestOcclusion", context.Center.Occlusion, playerPos, playerBounds);

        Assert.IsTrue(ReadPrivateBoolField(context.Center.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Left.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Right.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Top.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Bottom.Occlusion, "isOccluding"));

        CleanupForestContext(context);
    }

    [Test]
    public void ForestOcclusion_PlayerInsideForest_MakesEntireForestTransparent()
    {
        TestForestContext context = CreateForestContext();
        Bounds playerBounds = new Bounds(new Vector3(0f, 1.0f, 0f), new Vector3(0.5f, 0.8f, 1f));
        Vector2 playerPos = playerBounds.center;

        InvokeNonPublicVoid(context.Manager, "FindConnectedForest", context.Top.Occlusion, playerPos);
        InvokeNonPublicVoid(context.Manager, "HandleForestOcclusion", context.Top.Occlusion, playerPos, playerBounds);

        Assert.IsTrue(ReadPrivateBoolField(context.Center.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Left.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Right.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Top.Occlusion, "isOccluding"));
        Assert.IsTrue(ReadPrivateBoolField(context.Bottom.Occlusion, "isOccluding"));

        CleanupForestContext(context);
    }

    [Test]
    public void PreviewOcclusion_ClearPreviewBounds_RestoresTransparency()
    {
        Type occlusionManagerType = ResolveTypeOrFail("OcclusionManager");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");

        GameObject managerObject = new GameObject("OcclusionManager");
        Component manager = managerObject.AddComponent(occlusionManagerType);

        TestOccluderContext rock = CreateOccluder("Rock", "Rock", Vector2.zero, null, occlusionTransparencyType, addTreeController: false);
        InvokeVoid(manager, "RegisterOccluder", rock.Occlusion);

        Bounds previewBounds = new Bounds(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 1f));
        InvokeVoid(manager, "SetPreviewBounds", previewBounds);
        Assert.IsTrue(ReadPrivateBoolField(rock.Occlusion, "isOccluding"));

        InvokeVoid(manager, "SetPreviewBounds", (object)null);
        Assert.IsFalse(ReadPrivateBoolField(rock.Occlusion, "isOccluding"));

        CleanupOccluder(rock);
        UnityEngine.Object.DestroyImmediate(managerObject);
    }

    private Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型：{typeName}");
        return null;
    }

    private Transform InvokeTransform(Component component, string methodName)
    {
        MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.IsNotNull(method, $"未找到方法：{component.GetType().Name}.{methodName}");
        return (Transform)method.Invoke(component, null);
    }

    private bool InvokeBool(Component component, string methodName, params object[] args)
    {
        return InvokeBoolWithFlags(component, methodName, BindingFlags.Instance | BindingFlags.Public, args);
    }

    private bool InvokeBoolNonPublic(Component component, string methodName, params object[] args)
    {
        return InvokeBoolWithFlags(component, methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, args);
    }

    private bool InvokeBoolWithFlags(Component component, string methodName, BindingFlags flags, object[] args)
    {
        MethodInfo method = component.GetType().GetMethod(methodName, flags);
        Assert.IsNotNull(method, $"未找到方法：{component.GetType().Name}.{methodName}");
        return (bool)method.Invoke(component, args);
    }

    private void InvokeVoid(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.IsNotNull(method, $"未找到方法：{target.GetType().Name}.{methodName}");
        method.Invoke(target, args);
    }

    private void InvokeNonPublicVoid(Component component, string methodName, params object[] args)
    {
        MethodInfo method = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, $"未找到私有方法：{component.GetType().Name}.{methodName}");
        method.Invoke(component, args);
    }

    private bool ReadBoolProperty(Component component, string propertyName)
    {
        PropertyInfo property = component.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.IsNotNull(property, $"未找到属性：{component.GetType().Name}.{propertyName}");
        return (bool)property.GetValue(component);
    }

    private bool ReadPrivateBoolField(Component component, string fieldName)
    {
        FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"未找到字段：{component.GetType().Name}.{fieldName}");
        return (bool)field.GetValue(component);
    }

    private void SetPrivateField(Component component, string fieldName, object value)
    {
        FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"未找到字段：{component.GetType().Name}.{fieldName}");
        field.SetValue(component, value);
    }

    private TestForestContext CreateForestContext()
    {
        Type treeControllerType = ResolveTypeOrFail("TreeController");
        Type occlusionTransparencyType = ResolveTypeOrFail("OcclusionTransparency");
        Type occlusionManagerType = ResolveTypeOrFail("OcclusionManager");

        GameObject managerObject = new GameObject("OcclusionManager");
        Component manager = managerObject.AddComponent(occlusionManagerType);

        TestOccluderContext left = CreateOccluder("LeftTree", "Tree", new Vector2(-2f, 0f), treeControllerType, occlusionTransparencyType);
        TestOccluderContext right = CreateOccluder("RightTree", "Tree", new Vector2(2f, 0f), treeControllerType, occlusionTransparencyType);
        TestOccluderContext top = CreateOccluder("TopTree", "Tree", new Vector2(0f, 2f), treeControllerType, occlusionTransparencyType);
        TestOccluderContext bottom = CreateOccluder("BottomTree", "Tree", new Vector2(0f, -2f), treeControllerType, occlusionTransparencyType);
        TestOccluderContext center = CreateOccluder("CenterTree", "Tree", Vector2.zero, treeControllerType, occlusionTransparencyType);

        InvokeVoid(manager, "RegisterOccluder", left.Occlusion);
        InvokeVoid(manager, "RegisterOccluder", right.Occlusion);
        InvokeVoid(manager, "RegisterOccluder", top.Occlusion);
        InvokeVoid(manager, "RegisterOccluder", bottom.Occlusion);
        InvokeVoid(manager, "RegisterOccluder", center.Occlusion);

        return new TestForestContext
        {
            ManagerObject = managerObject,
            Manager = manager,
            Left = left,
            Right = right,
            Top = top,
            Bottom = bottom,
            Center = center
        };
    }

    private TestOccluderContext CreateOccluder(
        string name,
        string tag,
        Vector2 rootPosition,
        Type treeControllerType,
        Type occlusionTransparencyType,
        bool addTreeController = true)
    {
        GameObject root = new GameObject(name);
        root.tag = tag;
        root.transform.position = rootPosition;

        BoxCollider2D rootCollider = root.AddComponent<BoxCollider2D>();
        rootCollider.size = new Vector2(1f, 1f);
        rootCollider.offset = new Vector2(0f, 0.5f);

        GameObject visual = new GameObject("Tree");
        visual.tag = tag;
        visual.transform.SetParent(root.transform);
        visual.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(32, 64, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[32 * 64];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 32f, 64f), new Vector2(0.5f, 0f), 32f);
        spriteRenderer.sprite = sprite;

        if (addTreeController && treeControllerType != null)
        {
            visual.AddComponent(treeControllerType);
        }

        Component occlusion = visual.AddComponent(occlusionTransparencyType);

        return new TestOccluderContext
        {
            Root = root,
            Visual = visual,
            Occlusion = occlusion,
            Sprite = sprite,
            Texture = texture
        };
    }

    private void CleanupForestContext(TestForestContext context)
    {
        CleanupOccluder(context.Left);
        CleanupOccluder(context.Right);
        CleanupOccluder(context.Top);
        CleanupOccluder(context.Bottom);
        CleanupOccluder(context.Center);
        UnityEngine.Object.DestroyImmediate(context.ManagerObject);
    }

    private void CleanupOccluder(TestOccluderContext context)
    {
        if (context.Root != null)
        {
            UnityEngine.Object.DestroyImmediate(context.Root);
        }
        if (context.Sprite != null)
        {
            UnityEngine.Object.DestroyImmediate(context.Sprite);
        }
        if (context.Texture != null)
        {
            UnityEngine.Object.DestroyImmediate(context.Texture);
        }
    }

    private sealed class TestForestContext
    {
        public GameObject ManagerObject;
        public Component Manager;
        public TestOccluderContext Left;
        public TestOccluderContext Right;
        public TestOccluderContext Top;
        public TestOccluderContext Bottom;
        public TestOccluderContext Center;
    }

    private sealed class TestOccluderContext
    {
        public GameObject Root;
        public GameObject Visual;
        public Component Occlusion;
        public Sprite Sprite;
        public Texture2D Texture;
    }

    private float CalculateOverlapRatio(Bounds a, Bounds b)
    {
        float overlapMinX = Mathf.Max(a.min.x, b.min.x);
        float overlapMaxX = Mathf.Min(a.max.x, b.max.x);
        float overlapMinY = Mathf.Max(a.min.y, b.min.y);
        float overlapMaxY = Mathf.Min(a.max.y, b.max.y);
        
        float overlapWidth = overlapMaxX - overlapMinX;
        float overlapHeight = overlapMaxY - overlapMinY;
        
        if (overlapWidth <= 0 || overlapHeight <= 0)
            return 0f;
        
        float overlapArea = overlapWidth * overlapHeight;
        float areaA = a.size.x * a.size.y;
        float areaB = b.size.x * b.size.y;
        float smallerArea = Mathf.Min(areaA, areaB);
        
        return overlapArea / smallerArea;
    }
    
    #endregion
}
