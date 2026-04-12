using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DayNightManager))]
public class DayNightManagerEditor : Editor
{
    private SerializedProperty config;
    private SerializedProperty enableRouteA;
    private SerializedProperty overlay;
    private SerializedProperty globalLight;
    private SerializedProperty pointLightMgr;
    private SerializedProperty enableEditorPreview;
    private SerializedProperty editorPreviewMode;
    private SerializedProperty editorPreviewTime;
    private SerializedProperty editorPreviewSeason;
    private SerializedProperty editorPreviewWeather;
    private SerializedProperty editorPreviewFocus;
    private SerializedProperty animateLightsInEditor;
    private SerializedProperty showDebugInfo;
    private SerializedProperty enableNightVision;
    private SerializedProperty duskStartHour;
    private SerializedProperty fullNightHour;
    private SerializedProperty dawnRecoverHour;
    private SerializedProperty dawnClearHour;
    private SerializedProperty dayVisionRadiusNormalized;
    private SerializedProperty nightVisionRadiusNormalized;
    private SerializedProperty visionSoftness;
    private SerializedProperty visionOuterDarkness;
    private SerializedProperty visionAspect;
    private SerializedProperty dawnResidualVisionStrength;
    private SerializedProperty enableNightLightOverlay;
    private SerializedProperty nightLightWarmth;
    private SerializedProperty nightLightIntensityScale;
    private SerializedProperty nightLightSearchInterval;
    private SerializedProperty maxOverlayNightLights;
    private SerializedProperty debugFinalColor;
    private SerializedProperty debugBrightness;
    private SerializedProperty debugIsNight;

    private bool quickFoldout = true;
    private bool visionFoldout = true;
    private bool lightFoldout = true;
    private bool refsFoldout;
    private bool debugFoldout;

    private void OnEnable()
    {
        config = serializedObject.FindProperty("config");
        enableRouteA = serializedObject.FindProperty("enableRouteA");
        overlay = serializedObject.FindProperty("overlay");
        globalLight = serializedObject.FindProperty("globalLight");
        pointLightMgr = serializedObject.FindProperty("pointLightMgr");
        enableEditorPreview = serializedObject.FindProperty("enableEditorPreview");
        editorPreviewMode = serializedObject.FindProperty("editorPreviewMode");
        editorPreviewTime = serializedObject.FindProperty("editorPreviewTime");
        editorPreviewSeason = serializedObject.FindProperty("editorPreviewSeason");
        editorPreviewWeather = serializedObject.FindProperty("editorPreviewWeather");
        editorPreviewFocus = serializedObject.FindProperty("editorPreviewFocus");
        animateLightsInEditor = serializedObject.FindProperty("animateLightsInEditor");
        showDebugInfo = serializedObject.FindProperty("showDebugInfo");
        enableNightVision = serializedObject.FindProperty("enableNightVision");
        duskStartHour = serializedObject.FindProperty("duskStartHour");
        fullNightHour = serializedObject.FindProperty("fullNightHour");
        dawnRecoverHour = serializedObject.FindProperty("dawnRecoverHour");
        dawnClearHour = serializedObject.FindProperty("dawnClearHour");
        dayVisionRadiusNormalized = serializedObject.FindProperty("dayVisionRadiusNormalized");
        nightVisionRadiusNormalized = serializedObject.FindProperty("nightVisionRadiusNormalized");
        visionSoftness = serializedObject.FindProperty("visionSoftness");
        visionOuterDarkness = serializedObject.FindProperty("visionOuterDarkness");
        visionAspect = serializedObject.FindProperty("visionAspect");
        dawnResidualVisionStrength = serializedObject.FindProperty("dawnResidualVisionStrength");
        enableNightLightOverlay = serializedObject.FindProperty("enableNightLightOverlay");
        nightLightWarmth = serializedObject.FindProperty("nightLightWarmth");
        nightLightIntensityScale = serializedObject.FindProperty("nightLightIntensityScale");
        nightLightSearchInterval = serializedObject.FindProperty("nightLightSearchInterval");
        maxOverlayNightLights = serializedObject.FindProperty("maxOverlayNightLights");
        debugFinalColor = serializedObject.FindProperty("debugFinalColor");
        debugBrightness = serializedObject.FindProperty("debugBrightness");
        debugIsNight = serializedObject.FindProperty("debugIsNight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DayNightManager manager = (DayNightManager)target;

        DrawInspectorHeader();
        DrawTopToggles();
        DrawToolbar(manager);

        quickFoldout = DrawSection("快速调试", quickFoldout, DrawQuickControls);
        visionFoldout = DrawSection("夜晚视野", visionFoldout, DrawVisionControls);
        lightFoldout = DrawSection("夜灯与摇曳", lightFoldout, DrawLightControls);
        refsFoldout = DrawSection("引用与路线", refsFoldout, DrawReferenceControls);
        debugFoldout = DrawSection("调试读数", debugFoldout, DrawDebugControls);

        bool changed = serializedObject.ApplyModifiedProperties();
        if (changed)
        {
            manager.EditorRefreshNow();
        }
    }

    private void DrawInspectorHeader()
    {
        EditorGUILayout.Space(4f);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14
        };
        titleStyle.normal.textColor = new Color(1f, 0.76f, 0.30f);
        EditorGUILayout.LabelField("光影控制台", titleStyle);
        EditorGUILayout.LabelField("像云朵一样直接在 Inspector 调晨昏、夜视和夜灯。", EditorStyles.miniLabel);
        EditorGUILayout.Space(4f);
    }

    private void DrawTopToggles()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(enableEditorPreview, new GUIContent("编辑器预览"), GUILayout.MinWidth(120f));
            EditorGUILayout.PropertyField(animateLightsInEditor, new GUIContent("编辑器摇曳"), GUILayout.MinWidth(120f));
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(!enableEditorPreview.boolValue))
            {
                EditorGUILayout.PropertyField(editorPreviewMode, new GUIContent("预览模式"));
            }
        }
    }

    private void DrawToolbar(DayNightManager manager)
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("快捷操作", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("刷新预览", GUILayout.Height(24f)))
            {
                manager.EditorRefreshNow();
            }

            if (GUILayout.Button("补控制器", GUILayout.Height(24f)))
            {
                EnsureDayNightSceneControllers.EnsureSceneController(manager.gameObject.scene, markDirty: true);
                manager.EditorRefreshNow();
            }

            if (GUILayout.Button("选中 Overlay", GUILayout.Height(24f)))
            {
                Object overlayTarget = overlay.objectReferenceValue;
                if (overlayTarget != null && (overlayTarget.hideFlags & HideFlags.DontSaveInEditor) == 0)
                {
                    Selection.activeObject = overlayTarget;
                }
                else if (overlayTarget != null)
                {
                    EditorGUIUtility.PingObject(overlayTarget);
                }
            }

            if (GUILayout.Button("刷新全部灯", GUILayout.Height(24f)))
            {
                DayNightManager.EditorRefreshAllManagers();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private bool DrawSection(string title, bool expanded, System.Action drawContent)
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

    private void DrawQuickControls()
    {
        using (new EditorGUI.DisabledScope(!enableEditorPreview.boolValue))
        {
            float timeValue = editorPreviewTime.floatValue;
            bool isGlobalScenePreview = editorPreviewMode.enumValueIndex == (int)DayNightManager.EditorPreviewMode.GlobalScene;
            EditorGUILayout.Slider(editorPreviewTime, 0f, 23.99f, new GUIContent("预览时间"));
            EditorGUILayout.LabelField($"当前时间：{FormatHour(editorPreviewTime.floatValue)}", EditorStyles.miniLabel);
            EditorGUILayout.PropertyField(editorPreviewSeason, new GUIContent("预览季节"));
            EditorGUILayout.PropertyField(editorPreviewWeather, new GUIContent("预览天气"));
            using (new EditorGUI.DisabledScope(isGlobalScenePreview))
            {
                EditorGUILayout.PropertyField(
                    editorPreviewFocus,
                    new GUIContent(isGlobalScenePreview ? "预览焦点（全局模式下无效）" : "预览焦点"));
            }

            EditorGUILayout.HelpBox(
                isGlobalScenePreview
                    ? "全局预览：只看整张场景的晨昏/夜色和夜灯，不显示玩家视野收缩。"
                    : "局部预览：保留玩家视野收缩，适合调正式运行时的夜晚可见范围。",
                MessageType.None);

            if (!Mathf.Approximately(timeValue, editorPreviewTime.floatValue))
            {
                Repaint();
            }
        }
    }

    private void DrawVisionControls()
    {
        EditorGUILayout.PropertyField(enableNightVision, new GUIContent("启用夜晚视野"));
        using (new EditorGUI.DisabledScope(!enableNightVision.boolValue))
        {
            EditorGUILayout.Slider(duskStartHour, 16f, 22f, new GUIContent("开始收缩"));
            EditorGUILayout.Slider(fullNightHour, 18f, 23.99f, new GUIContent("收缩完成"));
            EditorGUILayout.Slider(dawnRecoverHour, 0f, 10f, new GUIContent("开始回放"));
            EditorGUILayout.Slider(dawnClearHour, 4f, 10f, new GUIContent("完全恢复"));
            EditorGUILayout.Slider(dayVisionRadiusNormalized, 0.6f, 1.6f, new GUIContent("白天半径"));
            EditorGUILayout.Slider(nightVisionRadiusNormalized, 0.15f, 1.0f, new GUIContent("夜晚半径"));
            EditorGUILayout.Slider(visionSoftness, 0.05f, 1.2f, new GUIContent("边缘柔化"));
            EditorGUILayout.Slider(visionOuterDarkness, 0f, 0.9f, new GUIContent("外围压暗"));
            EditorGUILayout.Slider(visionAspect, 0.5f, 1.3f, new GUIContent("横向比例"));
            EditorGUILayout.Slider(dawnResidualVisionStrength, 0f, 0.6f, new GUIContent("清晨残留"));
        }
    }

    private void DrawLightControls()
    {
        EditorGUILayout.PropertyField(enableNightLightOverlay, new GUIContent("启用夜灯 Overlay"));
        using (new EditorGUI.DisabledScope(!enableNightLightOverlay.boolValue))
        {
            EditorGUILayout.Slider(nightLightWarmth, 0f, 1f, new GUIContent("暖色混合"));
            EditorGUILayout.Slider(nightLightIntensityScale, 0.2f, 1.8f, new GUIContent("强度系数"));
            EditorGUILayout.Slider(nightLightSearchInterval, 0.2f, 3f, new GUIContent("刷新间隔"));
            EditorGUILayout.IntSlider(maxOverlayNightLights, 1, 8, new GUIContent("最大灯数"));
        }
    }

    private void DrawReferenceControls()
    {
        Object configTarget = config.objectReferenceValue;
        if (configTarget != null && (configTarget.hideFlags & HideFlags.DontSave) != 0)
        {
            EditorGUILayout.LabelField("配置资产", $"{configTarget.name}（临时预览配置）");
            EditorGUILayout.HelpBox("当前显示的是临时预览配置对象，不会写回正式资产。切换场景或刷新预览后会自动回到正式配置引用。", MessageType.Info);
        }
        else
        {
            EditorGUILayout.PropertyField(config, new GUIContent("配置资产"));
        }

        EditorGUILayout.PropertyField(enableRouteA, new GUIContent("启用路线 A"));
        EditorGUILayout.PropertyField(overlay, new GUIContent("Overlay"));
        EditorGUILayout.PropertyField(globalLight, new GUIContent("Global Light"));
        EditorGUILayout.PropertyField(pointLightMgr, new GUIContent("Point Light Manager"));
        EditorGUILayout.PropertyField(showDebugInfo, new GUIContent("详细日志"));
    }

    private void DrawDebugControls()
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(debugFinalColor, new GUIContent("当前颜色"));
            EditorGUILayout.PropertyField(debugBrightness, new GUIContent("当前亮度"));
            EditorGUILayout.PropertyField(debugIsNight, new GUIContent("夜晚模式"));
        }
    }

    private static string FormatHour(float rawHour)
    {
        float hourValue = rawHour % 24f;
        if (hourValue < 0f)
        {
            hourValue += 24f;
        }

        int hour = Mathf.FloorToInt(hourValue);
        int minute = Mathf.FloorToInt((hourValue - hour) * 60f);
        return $"{hour:00}:{minute:00}";
    }
}
