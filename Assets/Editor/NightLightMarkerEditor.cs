using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NightLightMarker))]
public class NightLightMarkerEditor : Editor
{
    private SerializedProperty maxIntensity;
    private SerializedProperty lightColor;
    private SerializedProperty radius;
    private SerializedProperty feather;
    private SerializedProperty overlayWeight;
    private SerializedProperty bindLight2D;
    private SerializedProperty pulseSpeed;
    private SerializedProperty pulseAmount;
    private SerializedProperty swayAmplitude;
    private SerializedProperty swaySpeed;
    private SerializedProperty animationSeed;

    private void OnEnable()
    {
        maxIntensity = serializedObject.FindProperty("maxIntensity");
        lightColor = serializedObject.FindProperty("lightColor");
        radius = serializedObject.FindProperty("radius");
        feather = serializedObject.FindProperty("feather");
        overlayWeight = serializedObject.FindProperty("overlayWeight");
        bindLight2D = serializedObject.FindProperty("bindLight2D");
        pulseSpeed = serializedObject.FindProperty("pulseSpeed");
        pulseAmount = serializedObject.FindProperty("pulseAmount");
        swayAmplitude = serializedObject.FindProperty("swayAmplitude");
        swaySpeed = serializedObject.FindProperty("swaySpeed");
        animationSeed = serializedObject.FindProperty("animationSeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInspectorHeader();
        DrawToolbar();
        DrawCoreControls();
        DrawAnimationControls();

        bool changed = serializedObject.ApplyModifiedProperties();
        if (changed)
        {
            DayNightManager.EditorRefreshAllManagers();
            SceneView.RepaintAll();
        }
    }

    private void DrawInspectorHeader()
    {
        EditorGUILayout.Space(4f);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13
        };
        titleStyle.normal.textColor = new Color(1f, 0.82f, 0.42f);
        EditorGUILayout.LabelField("夜灯控制器", titleStyle);
        EditorGUILayout.LabelField("调单盏灯的范围、羽化、呼吸和摇曳。", EditorStyles.miniLabel);
        EditorGUILayout.Space(4f);
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("刷新光影预览", GUILayout.Height(24f)))
            {
                DayNightManager.EditorRefreshAllManagers();
            }

            if (GUILayout.Button("随机相位", GUILayout.Height(24f)))
            {
                animationSeed.floatValue = Random.Range(0f, 1000f);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawCoreControls()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("光源主体", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lightColor, new GUIContent("颜色"));
            EditorGUILayout.Slider(maxIntensity, 0.1f, 2f, new GUIContent("最大强度"));
            EditorGUILayout.Slider(radius, 0.5f, 12f, new GUIContent("照射半径"));
            EditorGUILayout.Slider(feather, 0.05f, 0.95f, new GUIContent("边缘羽化"));
            EditorGUILayout.Slider(overlayWeight, 0.1f, 2f, new GUIContent("Overlay 权重"));
            EditorGUILayout.PropertyField(bindLight2D, new GUIContent("绑定 Light2D"));
        }
    }

    private void DrawAnimationControls()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("动态表现", EditorStyles.boldLabel);
            EditorGUILayout.Slider(pulseSpeed, 0f, 5f, new GUIContent("呼吸速度"));
            EditorGUILayout.Slider(pulseAmount, 0f, 1f, new GUIContent("呼吸幅度"));
            EditorGUILayout.Slider(swayAmplitude, 0f, 1f, new GUIContent("摇曳幅度"));
            EditorGUILayout.Slider(swaySpeed, 0f, 5f, new GUIContent("摇曳速度"));
            EditorGUILayout.PropertyField(animationSeed, new GUIContent("动画相位"));
        }
    }
}
