using UnityEditor;
using UnityEngine;

/// <summary>
/// CloudShadowManager 自定义编辑器
/// 把高频调参与高频操作放到第一屏，减少滚动。
/// </summary>
[CustomEditor(typeof(CloudShadowManager))]
public class CloudShadowManagerEditor : Editor
{
    private const float MinScaleLimit = 0.1f;
    private const float MaxScaleLimit = 5f;

    private SerializedProperty enableCloudShadows;
    private SerializedProperty intensity;
    private SerializedProperty density;
    private SerializedProperty scaleRange;
    private SerializedProperty cloudSprites;
    private SerializedProperty cloudMaterial;
    private SerializedProperty direction;
    private SerializedProperty speed;
    private SerializedProperty areaSizeMode;
    private SerializedProperty areaSize;
    private SerializedProperty worldLayerNames;
    private SerializedProperty boundsPadding;
    private SerializedProperty sortingLayerName;
    private SerializedProperty sortingOrder;
    private SerializedProperty useWeatherGate;
    private SerializedProperty currentWeather;
    private SerializedProperty enableInSunny;
    private SerializedProperty enableInPartlyCloudy;
    private SerializedProperty enableInOvercast;
    private SerializedProperty enableInRain;
    private SerializedProperty enableInSnow;
    private SerializedProperty seed;
    private SerializedProperty randomizeOnStart;
    private SerializedProperty previewInEditor;
    private SerializedProperty maxClouds;
    private SerializedProperty minCloudSpacing;
    private SerializedProperty maxSpawnAttempts;
    private SerializedProperty spawnCooldown;
    private SerializedProperty enableDebug;

    private bool quickFoldout = true;
    private bool areaFoldout = true;
    private bool assetsFoldout;
    private bool advancedFoldout;

    private void OnEnable()
    {
        enableCloudShadows = serializedObject.FindProperty("enableCloudShadows");
        intensity = serializedObject.FindProperty("intensity");
        density = serializedObject.FindProperty("density");
        scaleRange = serializedObject.FindProperty("scaleRange");
        cloudSprites = serializedObject.FindProperty("cloudSprites");
        cloudMaterial = serializedObject.FindProperty("cloudMaterial");
        direction = serializedObject.FindProperty("direction");
        speed = serializedObject.FindProperty("speed");
        areaSizeMode = serializedObject.FindProperty("areaSizeMode");
        areaSize = serializedObject.FindProperty("areaSize");
        worldLayerNames = serializedObject.FindProperty("worldLayerNames");
        boundsPadding = serializedObject.FindProperty("boundsPadding");
        sortingLayerName = serializedObject.FindProperty("sortingLayerName");
        sortingOrder = serializedObject.FindProperty("sortingOrder");
        useWeatherGate = serializedObject.FindProperty("useWeatherGate");
        currentWeather = serializedObject.FindProperty("currentWeather");
        enableInSunny = serializedObject.FindProperty("enableInSunny");
        enableInPartlyCloudy = serializedObject.FindProperty("enableInPartlyCloudy");
        enableInOvercast = serializedObject.FindProperty("enableInOvercast");
        enableInRain = serializedObject.FindProperty("enableInRain");
        enableInSnow = serializedObject.FindProperty("enableInSnow");
        seed = serializedObject.FindProperty("seed");
        randomizeOnStart = serializedObject.FindProperty("randomizeOnStart");
        previewInEditor = serializedObject.FindProperty("previewInEditor");
        maxClouds = serializedObject.FindProperty("maxClouds");
        minCloudSpacing = serializedObject.FindProperty("minCloudSpacing");
        maxSpawnAttempts = serializedObject.FindProperty("maxSpawnAttempts");
        spawnCooldown = serializedObject.FindProperty("spawnCooldown");
        enableDebug = serializedObject.FindProperty("enableDebug");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CloudShadowManager manager = (CloudShadowManager)target;

        DrawInspectorHeader();
        DrawPrimaryToggles();
        DrawToolbar(manager);

        if (!enableCloudShadows.boolValue)
        {
            EditorGUILayout.HelpBox("云影系统当前关闭，已自动清掉预览对象。", MessageType.Info);
            manager.EditorDespawnAll();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawStatusWarnings();

        quickFoldout = DrawSectionFoldout("快速调试", quickFoldout, DrawQuickControls);
        areaFoldout = DrawSectionFoldout("区域", areaFoldout, () => DrawAreaControls(manager));
        assetsFoldout = DrawSectionFoldout("素材与渲染", assetsFoldout, DrawAssetControls);
        advancedFoldout = DrawSectionFoldout("联动与高级", advancedFoldout, DrawAdvancedControls);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInspectorHeader()
    {
        EditorGUILayout.Space(4f);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14
        };
        titleStyle.normal.textColor = new Color(0.30f, 0.78f, 1f);
        EditorGUILayout.LabelField("云影控制台", titleStyle);
        EditorGUILayout.LabelField("先调顶部工具条和快速参数，低频项折叠收起。", EditorStyles.miniLabel);
        EditorGUILayout.Space(4f);
    }

    private void DrawPrimaryToggles()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(enableCloudShadows, new GUIContent("启用云影"), GUILayout.MinWidth(120f));
        EditorGUILayout.PropertyField(previewInEditor, new GUIContent("编辑器预览"), GUILayout.MinWidth(120f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private bool DrawSectionFoldout(string title, bool expanded, System.Action drawContent)
    {
        EditorGUILayout.Space(2f);
        expanded = EditorGUILayout.Foldout(expanded, title, true, EditorStyles.foldoutHeader);
        if (!expanded)
        {
            return false;
        }

        using (new EditorGUILayout.VerticalScope("box"))
        {
            drawContent?.Invoke();
        }

        return true;
    }

    private void DrawToolbar(CloudShadowManager manager)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("快捷操作", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        using (new EditorGUI.DisabledScope((CloudShadowManager.AreaSizeMode)areaSizeMode.enumValueIndex == CloudShadowManager.AreaSizeMode.Manual))
        {
            if (GUILayout.Button("刷新区域", GUILayout.Height(24f)))
            {
                manager.UpdateAreaSizeFromMode();
                EditorUtility.SetDirty(target);
                SceneView.RepaintAll();
            }
        }

        if (GUILayout.Button("立即重建", GUILayout.Height(24f)))
        {
            manager.EditorRebuildNow();
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("随机种子", GUILayout.Height(24f)))
        {
            manager.RandomizeSeed();
            manager.EditorRebuildNow();
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("清空", GUILayout.Height(24f)))
        {
            manager.EditorDespawnAll();
            EditorUtility.SetDirty(target);
            SceneView.RepaintAll();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawStatusWarnings()
    {
        if (cloudSprites.arraySize == 0)
        {
            EditorGUILayout.HelpBox("还没配置云影 Sprite，当前不会生成任何云影。", MessageType.Warning);
        }

        int layerId = SortingLayer.NameToID(sortingLayerName.stringValue);
        if (layerId == 0 && sortingLayerName.stringValue != "Default")
        {
            EditorGUILayout.HelpBox($"Sorting Layer '{sortingLayerName.stringValue}' 不存在，当前渲染层可能不对。", MessageType.Warning);
        }
    }

    private void DrawQuickControls()
    {
        EditorGUILayout.Slider(intensity, 0f, 1f, new GUIContent("强度"));
        EditorGUILayout.Slider(density, 0f, 1f, new GUIContent("密度"));
        EditorGUILayout.Slider(speed, 0f, 5f, new GUIContent("速度"));
        EditorGUILayout.IntSlider(maxClouds, 1, 20, new GUIContent("最大数量"));

        EditorGUILayout.Space(2f);
        DrawScaleSlider();

        EditorGUILayout.PropertyField(direction, new GUIContent("移动方向"));
    }

    private void DrawScaleSlider()
    {
        Vector2 value = scaleRange.vector2Value;
        float minValue = Mathf.Clamp(Mathf.Min(value.x, value.y), MinScaleLimit, MaxScaleLimit);
        float maxValue = Mathf.Clamp(Mathf.Max(value.x, value.y), MinScaleLimit, MaxScaleLimit);

        EditorGUILayout.LabelField("大小范围");
        EditorGUILayout.BeginHorizontal();
        float minField = EditorGUILayout.FloatField(minValue, GUILayout.Width(52f));
        EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, MinScaleLimit, MaxScaleLimit);
        float maxField = EditorGUILayout.FloatField(maxValue, GUILayout.Width(52f));
        EditorGUILayout.EndHorizontal();

        minValue = Mathf.Clamp(Mathf.Min(minField, maxField), MinScaleLimit, MaxScaleLimit);
        maxValue = Mathf.Clamp(Mathf.Max(minField, maxField), MinScaleLimit, MaxScaleLimit);
        scaleRange.vector2Value = new Vector2(minValue, maxValue);
    }

    private void DrawAreaControls(CloudShadowManager manager)
    {
        EditorGUILayout.PropertyField(areaSizeMode, new GUIContent("区域模式"));

        CloudShadowManager.AreaSizeMode mode = (CloudShadowManager.AreaSizeMode)areaSizeMode.enumValueIndex;
        switch (mode)
        {
            case CloudShadowManager.AreaSizeMode.Manual:
                EditorGUILayout.PropertyField(areaSize, new GUIContent("区域大小"));
                break;

            case CloudShadowManager.AreaSizeMode.FromNavGrid:
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(areaSize, new GUIContent("区域大小"));
                }
                EditorGUILayout.PropertyField(boundsPadding, new GUIContent("边界扩展"));
                if (FindFirstObjectByType<NavGrid2D>() == null)
                {
                    EditorGUILayout.HelpBox("场景里没找到 NavGrid2D，刷新区域后也不会拿到自动边界。", MessageType.Warning);
                }
                break;

            case CloudShadowManager.AreaSizeMode.AutoDetect:
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(areaSize, new GUIContent("区域大小"));
                }
                EditorGUILayout.PropertyField(worldLayerNames, new GUIContent("检测层级"), true);
                EditorGUILayout.PropertyField(boundsPadding, new GUIContent("边界扩展"));
                break;
        }

        EditorGUILayout.LabelField($"当前中心锚点：{manager.transform.position}", EditorStyles.miniLabel);
    }

    private void DrawAssetControls()
    {
        EditorGUILayout.PropertyField(cloudSprites, new GUIContent("云影 Sprite"), true);
        EditorGUILayout.PropertyField(cloudMaterial, new GUIContent("云影材质"));
        EditorGUILayout.PropertyField(sortingLayerName, new GUIContent("Sorting Layer"));
        EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Sorting Order"));
    }

    private void DrawAdvancedControls()
    {
        EditorGUILayout.PropertyField(useWeatherGate, new GUIContent("启用天气门控"));
        if (useWeatherGate.boolValue)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(currentWeather, new GUIContent("当前天气（手调）"));
                EditorGUILayout.PropertyField(enableInSunny, new GUIContent("晴天启用"));
                EditorGUILayout.PropertyField(enableInPartlyCloudy, new GUIContent("多云启用"));
                EditorGUILayout.PropertyField(enableInOvercast, new GUIContent("阴天启用"));
                EditorGUILayout.PropertyField(enableInRain, new GUIContent("雨天启用"));
                EditorGUILayout.PropertyField(enableInSnow, new GUIContent("雪天启用"));
            }
        }

        EditorGUILayout.Space(4f);
        EditorGUILayout.PropertyField(seed, new GUIContent("随机种子"));
        EditorGUILayout.PropertyField(randomizeOnStart, new GUIContent("启动时随机化"));
        EditorGUILayout.Slider(minCloudSpacing, 0f, 10f, new GUIContent("最小间距"));
        EditorGUILayout.IntSlider(maxSpawnAttempts, 1, 40, new GUIContent("单次补云尝试次数"));
        EditorGUILayout.Slider(spawnCooldown, 0f, 2f, new GUIContent("补云冷却"));
        EditorGUILayout.PropertyField(enableDebug, new GUIContent("详细日志"));
    }
}
