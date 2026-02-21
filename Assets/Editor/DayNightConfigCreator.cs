using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 编辑器工具：创建 DayNightConfig 默认配置资产
/// 设置四季 Gradient 关键帧、天气修正色、过渡参数、光照曲线
/// 使用方法：菜单栏 Tools > Create DayNight Config
/// </summary>
public static class DayNightConfigCreator
{
    private const string AssetPath = "Assets/111_Data/DayNightConfig.asset";

    [MenuItem("Tools/Create DayNight Config")]
    public static void CreateDefaultConfig()
    {
        // 检查是否已存在
        var existing = AssetDatabase.LoadAssetAtPath<DayNightConfig>(AssetPath);
        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog(
                "DayNight Config 已存在",
                $"路径 {AssetPath} 已存在配置资产。\n是否覆盖？",
                "覆盖", "取消"))
            {
                return;
            }
        }

        var config = ScriptableObject.CreateInstance<DayNightConfig>();

        // ═══ 设置四季 Gradient ═══
        config.springGradient = CreateSpringGradient();
        config.summerGradient = CreateSummerGradient();
        config.autumnGradient = CreateAutumnGradient();
        config.winterGradient = CreateWinterGradient();

        // ═══ 天气修正色（使用类中的默认值，此处显式设置以确保一致） ═══
        config.rainyTint = new Color(0.55f, 0.55f, 0.6f, 1f);
        config.witheringTint = new Color(0.85f, 0.75f, 0.45f, 1f);
        config.weatherTintStrength = 0.5f;

        // ═══ 过渡参数 ═══
        config.weatherTransitionDuration = 1.5f;
        config.timeJumpTransitionDuration = 0.8f;
        config.seasonTransitionDuration = 2.0f;

        // ═══ 路线 A：全局光照曲线 ═══
        config.globalLightIntensityCurve = CreateGlobalLightIntensityCurve();
        config.globalLightColorGradient = CreateGlobalLightColorGradient();

        // ═══ 路线 A：局部光源参数 ═══
        config.nightLightActivateHour = 18f;
        config.nightLightDeactivateHour = 6f;
        config.pointLightFadeDuration = 1.0f;

        // ═══ 路线融合参数 ═══
        config.overlayStrengthWithURP = 0.4f;
        config.overlayStrengthWithoutURP = 1.0f;

        // 确保目标目录存在
        string directory = System.IO.Path.GetDirectoryName(AssetPath);
        if (!AssetDatabase.IsValidFolder(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // 保存资产
        AssetDatabase.CreateAsset(config, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 选中并高亮新创建的资产
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;

        Debug.Log($"[DayNightConfigCreator] 配置资产已创建：{AssetPath}");
    }

    // ═══════════════════════════════════════════════════════════
    //  四季 Gradient 创建方法
    //  Multiply 混合模式：白色(1,1,1)=无变化，越暗=越强效果
    //  dayProgress 映射：0.0=06:00, 0.3=12:00, 0.6=18:00, 0.8=22:00, 1.0=02:00
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// 春季：淡绿金 → 近白微绿 → 绿橙 → 深蓝绿 → 深蓝
    /// </summary>
    private static Gradient CreateSpringGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.95f, 0.90f, 0.75f), 0.00f),  // 06:00 淡绿金
                new GradientColorKey(new Color(0.98f, 0.98f, 0.95f), 0.30f),  // 12:00 近白微绿
                new GradientColorKey(new Color(0.90f, 0.80f, 0.60f), 0.60f),  // 18:00 绿橙
                new GradientColorKey(new Color(0.40f, 0.45f, 0.60f), 0.80f),  // 22:00 深蓝绿
                new GradientColorKey(new Color(0.30f, 0.30f, 0.50f), 1.00f),  // 02:00 深蓝
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }

    /// <summary>
    /// 夏季：暖金 → 近白微暖 → 暖橙红 → 深蓝暖 → 深蓝紫
    /// </summary>
    private static Gradient CreateSummerGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.95f, 0.88f, 0.70f), 0.00f),  // 06:00 暖金
                new GradientColorKey(new Color(1.00f, 0.98f, 0.95f), 0.30f),  // 12:00 近白微暖
                new GradientColorKey(new Color(0.92f, 0.75f, 0.55f), 0.60f),  // 18:00 暖橙红
                new GradientColorKey(new Color(0.38f, 0.40f, 0.58f), 0.80f),  // 22:00 深蓝暖
                new GradientColorKey(new Color(0.32f, 0.28f, 0.48f), 1.00f),  // 02:00 深蓝紫
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }

    /// <summary>
    /// 秋季：橙金 → 近白微橙 → 深橙 → 深蓝橙 → 深蓝
    /// </summary>
    private static Gradient CreateAutumnGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.95f, 0.82f, 0.60f), 0.00f),  // 06:00 橙金
                new GradientColorKey(new Color(0.98f, 0.95f, 0.90f), 0.30f),  // 12:00 近白微橙
                new GradientColorKey(new Color(0.88f, 0.70f, 0.45f), 0.60f),  // 18:00 深橙
                new GradientColorKey(new Color(0.40f, 0.38f, 0.55f), 0.80f),  // 22:00 深蓝橙
                new GradientColorKey(new Color(0.30f, 0.30f, 0.48f), 1.00f),  // 02:00 深蓝
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }

    /// <summary>
    /// 冬季：冷蓝白 → 近白微蓝 → 冷紫蓝 → 深蓝紫 → 深紫蓝
    /// </summary>
    private static Gradient CreateWinterGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.80f, 0.85f, 0.95f), 0.00f),  // 06:00 冷蓝白
                new GradientColorKey(new Color(0.95f, 0.95f, 0.98f), 0.30f),  // 12:00 近白微蓝
                new GradientColorKey(new Color(0.70f, 0.65f, 0.85f), 0.60f),  // 18:00 冷紫蓝
                new GradientColorKey(new Color(0.35f, 0.30f, 0.55f), 0.80f),  // 22:00 深蓝紫
                new GradientColorKey(new Color(0.28f, 0.25f, 0.50f), 1.00f),  // 02:00 深紫蓝
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }

    // ═══════════════════════════════════════════════════════════
    //  路线 A 光照曲线
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// 全局光照强度曲线（dayProgress 0-1）
    /// 06:00=0.7 → 09:00=0.9 → 12:00=1.0 → 15:00=0.9 → 18:00=0.6 → 22:00=0.3 → 02:00=0.2
    /// </summary>
    private static AnimationCurve CreateGlobalLightIntensityCurve()
    {
        var curve = new AnimationCurve(
            new Keyframe(0.00f, 0.7f),   // 06:00
            new Keyframe(0.15f, 0.9f),   // 09:00
            new Keyframe(0.30f, 1.0f),   // 12:00
            new Keyframe(0.45f, 0.9f),   // 15:00
            new Keyframe(0.60f, 0.6f),   // 18:00
            new Keyframe(0.80f, 0.3f),   // 22:00
            new Keyframe(1.00f, 0.2f)    // 02:00
        );

        // 设置所有关键帧为平滑切线，确保曲线连续
        for (int i = 0; i < curve.keys.Length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
        }

        return curve;
    }

    /// <summary>
    /// 全局光照颜色曲线（路线 A 用，与季节无关的通用光照色温）
    /// 日出暖色 → 正午白色 → 日落暖色 → 夜晚冷蓝
    /// </summary>
    private static Gradient CreateGlobalLightColorGradient()
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(1.0f, 0.85f, 0.65f), 0.00f),   // 06:00 暖色日出
                new GradientColorKey(new Color(1.0f, 0.98f, 0.95f), 0.30f),   // 12:00 近白正午
                new GradientColorKey(new Color(1.0f, 0.80f, 0.55f), 0.60f),   // 18:00 暖色日落
                new GradientColorKey(new Color(0.50f, 0.55f, 0.80f), 0.80f),  // 22:00 冷蓝夜晚
                new GradientColorKey(new Color(0.40f, 0.42f, 0.70f), 1.00f),  // 02:00 深蓝凌晨
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f),
            }
        );
        return gradient;
    }

    // ═══════════════════════════════════════════════════════════
    //  Multiply 混合模式材质创建
    // ═══════════════════════════════════════════════════════════

    private const string MaterialPath = "Assets/444_Shaders/Material/DayNightMultiply.mat";

    // ═══════════════════════════════════════════════════════════
    //  场景层级结构搭建
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// 在场景中创建 DayNightManager 层级结构并连接所有组件引用
    /// 层级：PersistentManagers / DayNightManager
    ///         ├── DayNightOverlay（路线 B，SpriteRenderer + Multiply 材质）
    ///         ├── GlobalLightController（路线 A，默认禁用）
    ///         └── PointLightManager（路线 A，默认禁用）
    /// </summary>
    [MenuItem("Tools/Setup DayNight Scene")]
    public static void SetupDayNightScene()
    {
        // 查找或创建 DayNightManager 根对象
        var existingMgr = Object.FindFirstObjectByType<DayNightManager>();
        if (existingMgr != null)
        {
            if (!EditorUtility.DisplayDialog(
                "DayNightManager 已存在",
                "场景中已存在 DayNightManager。\n是否重新配置引用？",
                "重新配置", "取消"))
            {
                return;
            }
            SetupHierarchy(existingMgr.gameObject);
            return;
        }

        // 创建新的 DayNightManager 对象
        var mgrGO = new GameObject("DayNightManager");
        mgrGO.AddComponent<DayNightManager>();

        // 尝试挂到 PersistentManagers 下
        var persistentManagers = GameObject.Find("PersistentManagers");
        if (persistentManagers != null)
        {
            mgrGO.transform.SetParent(persistentManagers.transform);
            Debug.Log("[SetupDayNightScene] 已挂载到 PersistentManagers 下");
        }
        else
        {
            Debug.LogWarning("[SetupDayNightScene] 未找到 PersistentManagers，DayNightManager 创建在根层级");
        }

        SetupHierarchy(mgrGO);
    }

    /// <summary>
    /// 在指定 GameObject 下创建子对象并连接所有引用
    /// </summary>
    private static void SetupHierarchy(GameObject mgrGO)
    {
        var mgrComp = mgrGO.GetComponent<DayNightManager>();
        if (mgrComp == null)
        {
            mgrComp = mgrGO.AddComponent<DayNightManager>();
        }

        // ═══ 创建 DayNightOverlay 子对象 ═══
        var overlayGO = FindOrCreateChild(mgrGO, "DayNightOverlay");
        var overlayComp = overlayGO.GetComponent<DayNightOverlay>();
        if (overlayComp == null)
        {
            overlayComp = overlayGO.AddComponent<DayNightOverlay>();
        }
        // 确保有 SpriteRenderer
        var sr = overlayGO.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = overlayGO.AddComponent<SpriteRenderer>();
        }
        // 分配 Multiply 材质到 DayNightOverlay
        var multiplyMat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (multiplyMat != null)
        {
            var overlaySO = new SerializedObject(overlayComp);
            var matProp = overlaySO.FindProperty("multiplyMaterial");
            if (matProp != null) matProp.objectReferenceValue = multiplyMat;
            var srProp = overlaySO.FindProperty("spriteRenderer");
            if (srProp != null) srProp.objectReferenceValue = sr;
            overlaySO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning($"[SetupDayNightScene] 未找到 Multiply 材质：{MaterialPath}，请先运行 Tools > Create DayNight Multiply Material");
        }

        // ═══ 创建 GlobalLightController 子对象（路线 A，默认禁用） ═══
        var globalLightGO = FindOrCreateChild(mgrGO, "GlobalLightController");
        var globalLightComp = globalLightGO.GetComponent<GlobalLightController>();
        if (globalLightComp == null)
        {
            globalLightComp = globalLightGO.AddComponent<GlobalLightController>();
        }
        globalLightGO.SetActive(false);

        // ═══ 创建 PointLightManager 子对象（路线 A，默认禁用） ═══
        var pointLightGO = FindOrCreateChild(mgrGO, "PointLightManager");
        var pointLightComp = pointLightGO.GetComponent<PointLightManager>();
        if (pointLightComp == null)
        {
            pointLightComp = pointLightGO.AddComponent<PointLightManager>();
        }
        pointLightGO.SetActive(false);

        // ═══ 通过 SerializedObject 连接 DayNightManager 的所有私有引用 ═══
        var mgrSO = new SerializedObject(mgrComp);

        // config → DayNightConfig 资产
        var configProp = mgrSO.FindProperty("config");
        if (configProp != null)
        {
            var configAsset = AssetDatabase.LoadAssetAtPath<DayNightConfig>(AssetPath);
            if (configAsset != null)
            {
                configProp.objectReferenceValue = configAsset;
            }
            else
            {
                Debug.LogWarning($"[SetupDayNightScene] 未找到 DayNightConfig：{AssetPath}，请先运行 Tools > Create DayNight Config");
            }
        }

        // overlay → DayNightOverlay
        var overlayProp = mgrSO.FindProperty("overlay");
        if (overlayProp != null) overlayProp.objectReferenceValue = overlayComp;

        // globalLight → GlobalLightController
        var globalLightProp = mgrSO.FindProperty("globalLight");
        if (globalLightProp != null) globalLightProp.objectReferenceValue = globalLightComp;

        // pointLightMgr → PointLightManager
        var pointLightProp = mgrSO.FindProperty("pointLightMgr");
        if (pointLightProp != null) pointLightProp.objectReferenceValue = pointLightComp;

        // enableRouteA → false
        var routeAProp = mgrSO.FindProperty("enableRouteA");
        if (routeAProp != null) routeAProp.boolValue = false;

        mgrSO.ApplyModifiedProperties();

        // ═══ 标记场景为脏，以便保存 ═══
        EditorSceneManager.MarkSceneDirty(
            SceneManager.GetActiveScene());

        Selection.activeGameObject = mgrGO;

        Debug.Log("<color=green>[SetupDayNightScene] ✅ DayNightManager 层级结构创建完成！</color>\n" +
                  "  ├── DayNightOverlay（路线 B）\n" +
                  "  ├── GlobalLightController（路线 A，已禁用）\n" +
                  "  └── PointLightManager（路线 A，已禁用）");
    }

    /// <summary>
    /// 查找或创建子对象
    /// </summary>
    private static GameObject FindOrCreateChild(GameObject parent, string childName)
    {
        var existing = parent.transform.Find(childName);
        if (existing != null) return existing.gameObject;

        var child = new GameObject(childName);
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        return child;
    }

    /// <summary>
    /// 创建 Multiply 混合模式材质，用于 DayNightOverlay 全屏色调叠加
    /// 使用自定义 Custom/SpriteMultiply shader，确保 Blend DstColor Zero 正片叠底生效
    /// </summary>
    [MenuItem("Tools/Create DayNight Multiply Material")]
    public static void CreateMultiplyMaterial()
    {
        // 检查是否已存在
        var existing = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog(
                "DayNight Multiply 材质已存在",
                $"路径 {MaterialPath} 已存在材质。\n是否覆盖？",
                "覆盖", "取消"))
            {
                return;
            }
        }

        // 查找自定义 SpriteMultiply shader（Blend DstColor Zero 正片叠底）
        Shader multiplyShader = Shader.Find("Custom/SpriteMultiply");
        if (multiplyShader == null)
        {
            Debug.LogError("[DayNightConfigCreator] 找不到 Custom/SpriteMultiply shader！请确认 SpriteMultiply.shader 已导入项目（Assets/444_Shaders/SpriteMultiply.shader）");
            return;
        }

        var mat = new Material(multiplyShader);
        mat.name = "DayNightMultiply";

        // 设置默认颜色为白色（Multiply 下白色 = 无变化）
        mat.color = Color.white;

        // 确保目标目录存在
        string directory = System.IO.Path.GetDirectoryName(MaterialPath);
        if (!AssetDatabase.IsValidFolder(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // 保存材质资产
        AssetDatabase.CreateAsset(mat, MaterialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 选中并高亮
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mat;

        Debug.Log($"[DayNightConfigCreator] Multiply 材质已创建（Custom/SpriteMultiply shader）：{MaterialPath}");
    }

}
