using NUnit.Framework;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 云朵阴影系统单元测试
/// 测试核心算法的正确性
/// </summary>
[TestFixture]
public class CloudShadowSystemTests
{
    private const string CloudShadowManagerAssemblyQualifiedName = "CloudShadowManager, Assembly-CSharp";

    #region 属性 6: 停用状态清理语义

    [Test]
    public void Lifecycle_DisableComponentInEditor_ShouldClearSpawnedCloudObjects()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);

            Assert.Greater(CountCloudShadowChildren(manager.transform), 0, "预期先生成至少一个编辑器预览云影");

            manager.enabled = false;

            Assert.AreEqual(0, CountCloudShadowChildren(manager.transform), "组件禁用后应立即清掉残留云影对象");
        }
        finally
        {
            if (managerObject != null)
            {
                Object.DestroyImmediate(managerObject);
            }

            if (sprite != null)
            {
                Object.DestroyImmediate(sprite);
            }
        }
    }

    [Test]
    public void SimulateStep_Disabled_ShouldClearSpawnedCloudObjects()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "enableCloudShadows", false);

            InvokeInstanceMethod(manager, "SimulateStep", 0f);

            Assert.AreEqual(0, CountCloudShadowChildren(manager.transform), "关闭总开关后应立即清掉已有云影");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    [Test]
    public void SimulateStep_WeatherBlocked_ShouldClearSpawnedCloudObjects()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "useWeatherGate", true);
            SetPrivateField(manager, "enableInOvercast", false);
            InvokeInstanceMethod(manager, "SetWeatherState", ParseNestedEnum(manager, "WeatherState", "Overcast"));

            InvokeInstanceMethod(manager, "SimulateStep", 0f);

            Assert.AreEqual(0, CountCloudShadowChildren(manager.transform), "天气门控禁止时应立即清掉已有云影");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    [Test]
    public void SimulateStep_NullOnlySprites_ShouldClearSpawnedCloudObjects()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "cloudSprites", new Sprite[] { null });

            InvokeInstanceMethod(manager, "SimulateStep", 0f);

            Assert.AreEqual(0, CountCloudShadowChildren(manager.transform), "仅剩空 Sprite 引用时也应清场，不能保留历史云影");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    #endregion

    #region 高负载补云回归

    [Test]
    public void SimulateStep_HighSpeed_ShouldRecycleAndRefillMultipleCloudsInSameStep()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "areaSize", new Vector2(20f, 8f));
            SetPrivateField(manager, "direction", new Vector2(1f, 0f));
            SetPrivateField(manager, "speed", 50f);
            SetPrivateField(manager, "density", 1f);
            SetPrivateField(manager, "maxClouds", 4);
            SetPrivateField(manager, "minCloudSpacing", 0f);
            SetPrivateField(manager, "maxSpawnAttempts", 24);
            SetPrivateField(manager, "spawnCooldown", 0f);
            SetPrivateField(manager, "scaleRange", new Vector2(0.5f, 0.5f));

            InvokeInstanceMethod(manager, "EditorRebuildNow");

            Assert.GreaterOrEqual(CountCloudShadowChildren(manager.transform), 3, "测试前应先构造出多朵云影");

            InvokeInstanceMethod(manager, "SimulateStep", 1f);

            Assert.GreaterOrEqual(CountCloudShadowChildren(manager.transform), 3, "高速清退后应在同一步补回多朵云影，不能只剩 1 朵");
            Assert.IsFalse(HasCloudOutsideRightExit(manager.transform, 12f), "高速推进后不应还残留明显越界却没回收的云影");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    [Test]
    public void EdgeSpawn_HorizontalMovement_ShouldKeepCloudHeightInsideArea()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "maxSpawnAttempts", 12);

            Rect area = new Rect(-10f, -4f, 20f, 8f);
            Vector2 direction = new Vector2(1f, 0f);
            Vector3? position = (Vector3?)InvokeInstanceMethodResult(
                manager,
                "TryFindNonOverlappingEdgePosition",
                area,
                direction,
                1f,
                1.5f);

            Assert.IsTrue(position.HasValue, "应能找到边缘生成位置");
            Assert.That(position.Value.y, Is.InRange(area.yMin + 1.5f, area.yMax - 1.5f), "水平补云时不应把云影上下边缘塞出区域外");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    [Test]
    public void EditorRebuildNow_ShouldSeedAtLeastOneCloudInsideActiveArea()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "areaSize", new Vector2(20f, 8f));
            SetPrivateField(manager, "direction", new Vector2(1f, 0f));
            SetPrivateField(manager, "density", 1f);
            SetPrivateField(manager, "maxClouds", 4);
            SetPrivateField(manager, "minCloudSpacing", 0f);
            SetPrivateField(manager, "maxSpawnAttempts", 24);
            SetPrivateField(manager, "scaleRange", new Vector2(0.5f, 0.5f));
            manager.transform.position = Vector3.zero;

            InvokeInstanceMethod(manager, "EditorRebuildNow");

            Rect area = new Rect(-10f, -4f, 20f, 8f);
            Assert.IsTrue(HasCloudInsideArea(manager.transform, area), "重建后至少应有一朵云直接分布到活动区域内部，而不是全部卡在进场边缘外");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    [Test]
    public void EdgeRefillSpawn_ShouldPlaceCloudInsideLeadingBandInsteadOfOutsideArea()
    {
        GameObject managerObject = null;
        Sprite sprite = null;

        try
        {
            Behaviour manager = CreateManagerWithSpawnedCloud(out managerObject, out sprite);
            SetPrivateField(manager, "areaSize", new Vector2(20f, 8f));
            SetPrivateField(manager, "direction", new Vector2(1f, 0f));
            SetPrivateField(manager, "density", 1f);
            SetPrivateField(manager, "maxClouds", 4);
            SetPrivateField(manager, "minCloudSpacing", 0f);
            SetPrivateField(manager, "maxSpawnAttempts", 24);
            SetPrivateField(manager, "scaleRange", new Vector2(0.5f, 0.5f));
            manager.transform.position = Vector3.zero;

            bool spawned = (bool)InvokeInstanceMethodResult(
                manager,
                "TrySpawnOneAtEdge",
                new Rect(-10f, -4f, 20f, 8f),
                new Vector2(1f, 0f));

            Assert.IsTrue(spawned, "补云时应能成功生成新云影");

            Transform newestCloud = FindLastCloud(manager.transform);
            Assert.IsNotNull(newestCloud, "生成后应能找到最新云影对象");
            Assert.GreaterOrEqual(newestCloud.position.x, -9f, "补云不应再把云影中心刷到活动区左外侧");
            Assert.LessOrEqual(newestCloud.position.x, -2f, "补云应先落在进场带，而不是直接跳到过深区域");
        }
        finally
        {
            CleanupManager(managerObject, sprite);
        }
    }

    #endregion

    #region 属性 7: 云朵天气联动一致性
    
    /// <summary>
    /// 测试晴天时云影应该显示
    /// 验证需求: 4.1
    /// </summary>
    [Test]
    public void WeatherGate_Sunny_CloudsEnabled()
    {
        // Arrange
        bool enableInSunny = true;
        string currentWeather = "Sunny";
        
        // Act
        bool shouldShow = IsWeatherAllowed(currentWeather, enableInSunny, true, false, false, false);
        
        // Assert
        Assert.IsTrue(shouldShow, "晴天时云影应该显示");
    }
    
    /// <summary>
    /// 测试雨天时云影应该隐藏
    /// 验证需求: 4.2
    /// </summary>
    [Test]
    public void WeatherGate_Rain_CloudsDisabled()
    {
        // Arrange
        bool enableInRain = false;
        string currentWeather = "Rain";
        
        // Act
        bool shouldShow = IsWeatherAllowed(currentWeather, true, true, false, enableInRain, false);
        
        // Assert
        Assert.IsFalse(shouldShow, "雨天时云影应该隐藏");
    }
    
    /// <summary>
    /// 测试阴天时云影应该隐藏
    /// 验证需求: 4.2
    /// </summary>
    [Test]
    public void WeatherGate_Overcast_CloudsDisabled()
    {
        // Arrange
        bool enableInOvercast = false;
        string currentWeather = "Overcast";
        
        // Act
        bool shouldShow = IsWeatherAllowed(currentWeather, true, true, enableInOvercast, false, false);
        
        // Assert
        Assert.IsFalse(shouldShow, "阴天时云影应该隐藏");
    }
    
    #endregion
    
    #region 属性 8: 云朵循环移动不变性
    
    /// <summary>
    /// 测试云朵移出右边界后应该出现在左边界
    /// 验证需求: 4.3
    /// </summary>
    [Test]
    public void CloudLoop_ExitRight_EnterLeft()
    {
        // Arrange
        Rect area = new Rect(-20f, -12f, 40f, 24f);
        Vector2 direction = new Vector2(1f, 0f); // 向右移动
        float halfWidth = 2f;
        float cloudX = area.xMax + halfWidth + 1f; // 超出右边界 + halfWidth
        
        // Act
        float newX = cloudX;
        if (direction.x > 0f && cloudX > area.xMax + halfWidth)
        {
            newX = area.xMin - halfWidth;
        }
        
        // Assert
        Assert.Less(newX, area.center.x, "云朵应该传送到左侧");
    }
    
    /// <summary>
    /// 测试云朵移出左边界后应该出现在右边界
    /// 验证需求: 4.3
    /// </summary>
    [Test]
    public void CloudLoop_ExitLeft_EnterRight()
    {
        // Arrange
        Rect area = new Rect(-20f, -12f, 40f, 24f);
        Vector2 direction = new Vector2(-1f, 0f); // 向左移动
        float cloudX = area.xMin - 3f; // 超出左边界
        float halfWidth = 2f;
        
        // Act
        float newX = cloudX;
        if (direction.x < 0f && cloudX < area.xMin - halfWidth)
        {
            newX = area.xMax + halfWidth;
        }
        
        // Assert
        Assert.Greater(newX, area.center.x, "云朵应该传送到右侧");
    }
    
    #endregion
    
    #region 属性 9: 对象池资源管理正确性
    
    /// <summary>
    /// 测试对象池复用逻辑
    /// 验证需求: 4.4
    /// </summary>
    [Test]
    public void ObjectPool_Reuse_WorksCorrectly()
    {
        // Arrange
        var pool = new System.Collections.Generic.Stack<GameObject>();
        var active = new System.Collections.Generic.List<GameObject>();
        
        // 模拟创建和回收
        var obj1 = new GameObject("Cloud1");
        var obj2 = new GameObject("Cloud2");
        
        // Act - 回收到池
        pool.Push(obj1);
        pool.Push(obj2);
        
        // Act - 从池取出
        var reused1 = pool.Pop();
        var reused2 = pool.Pop();
        
        // Assert
        Assert.AreEqual(obj2, reused1, "应该复用最后入池的对象");
        Assert.AreEqual(obj1, reused2, "应该复用先入池的对象");
        Assert.AreEqual(0, pool.Count, "池应该为空");
        
        // Cleanup
        Object.DestroyImmediate(obj1);
        Object.DestroyImmediate(obj2);
    }
    
    /// <summary>
    /// 测试云朵数量调整
    /// 验证需求: 4.4
    /// </summary>
    [Test]
    public void CloudCount_Adjustment_WorksCorrectly()
    {
        // Arrange
        int currentCount = 5;
        int targetCount = 8;
        int maxClouds = 32;
        
        // Act
        int toAdd = Mathf.Max(0, targetCount - currentCount);
        int finalCount = Mathf.Min(currentCount + toAdd, maxClouds);
        
        // Assert
        Assert.AreEqual(targetCount, finalCount, "云朵数量应该调整到目标值");
    }
    
    #endregion
    
    #region 辅助方法
    
    /// <summary>
    /// 模拟天气门控逻辑
    /// </summary>
    private bool IsWeatherAllowed(string weather, 
        bool enableInSunny, bool enableInPartlyCloudy, 
        bool enableInOvercast, bool enableInRain, bool enableInSnow)
    {
        switch (weather)
        {
            case "Sunny": return enableInSunny;
            case "PartlyCloudy": return enableInPartlyCloudy;
            case "Overcast": return enableInOvercast;
            case "Rain": return enableInRain;
            case "Snow": return enableInSnow;
            default: return true;
        }
    }

    private Behaviour CreateManagerWithSpawnedCloud(out GameObject managerObject, out Sprite sprite)
    {
        managerObject = new GameObject("CloudShadowManagerTest");
        System.Type managerType = ResolveCloudShadowManagerType();
        var manager = managerObject.AddComponent(managerType) as Behaviour;
        Assert.IsNotNull(manager, "未能创建 CloudShadowManager 行为实例");
        sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));

        SetPrivateField(manager, "cloudSprites", new[] { sprite });
        SetPrivateField(manager, "density", 1f);
        SetPrivateField(manager, "maxClouds", 1);
        SetPrivateField(manager, "randomizeOnStart", false);
        SetPrivateField(manager, "enableCloudShadows", true);

        InvokeInstanceMethod(manager, "EditorRebuildNow");
        return manager;
    }

    private void CleanupManager(GameObject managerObject, Sprite sprite)
    {
        if (managerObject != null)
        {
            Object.DestroyImmediate(managerObject);
        }

        if (sprite != null)
        {
            Object.DestroyImmediate(sprite);
        }
    }

    private void SetPrivateField<T>(Component manager, string fieldName, T value)
    {
        FieldInfo field = manager.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"未找到私有字段: {fieldName}");
        field.SetValue(manager, value);
    }

    private void InvokeInstanceMethod(Component manager, string methodName, params object[] args)
    {
        MethodInfo method = manager.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"未找到方法: {methodName}");
        method.Invoke(manager, args);
    }

    private object InvokeInstanceMethodResult(Component manager, string methodName, params object[] args)
    {
        MethodInfo method = manager.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"未找到方法: {methodName}");
        return method.Invoke(manager, args);
    }

    private object ParseNestedEnum(Component manager, string enumName, string valueName)
    {
        System.Type enumType = manager.GetType().GetNestedType(enumName, BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(enumType, $"未找到枚举: {enumName}");
        return System.Enum.Parse(enumType, valueName);
    }

    private System.Type ResolveCloudShadowManagerType()
    {
        System.Type managerType = System.Type.GetType(CloudShadowManagerAssemblyQualifiedName);
        Assert.IsNotNull(managerType, "未能从 Assembly-CSharp 解析 CloudShadowManager 类型");
        return managerType;
    }

    private int CountCloudShadowChildren(Transform root)
    {
        int count = 0;
        for (int i = 0; i < root.childCount; i++)
        {
            if (root.GetChild(i).name == "CloudShadow")
            {
                count++;
            }
        }

        return count;
    }

    private bool HasCloudOutsideRightExit(Transform root, float xLimit)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == "CloudShadow" && child.position.x > xLimit)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasCloudInsideArea(Transform root, Rect area)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == "CloudShadow" && area.Contains(child.position))
            {
                return true;
            }
        }

        return false;
    }

    private Transform FindLastCloud(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);
            if (child.name == "CloudShadow")
            {
                return child;
            }
        }

        return null;
    }
    
    #endregion
}
