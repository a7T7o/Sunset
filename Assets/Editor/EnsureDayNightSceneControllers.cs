using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 确保 Primary / Town 场景在编辑模式下就有可见的 DayNight 控制器。
/// 只在缺失时补齐，不主动覆盖用户已经摆好的层级。
/// </summary>
[InitializeOnLoad]
public static class EnsureDayNightSceneControllers
{
    private const string PrimaryScenePath = "Assets/000_Scenes/Primary.unity";
    private const string TownScenePath = "Assets/000_Scenes/Town.unity";
    private const string PersistentManagersName = "PersistentManagers";
    private const string DayNightManagerName = "DayNightManager";
    private const string OverlayName = "DayNightOverlay";
    private const string GlobalLightName = "GlobalLightController";
    private const string PointLightManagerName = "PointLightManager";
    private const string ConfigAssetPath = "Assets/111_Data/DayNightConfig.asset";
    private static bool initialSceneSyncDone;
    private static readonly bool VerboseSyncLog = false;

    static EnsureDayNightSceneControllers()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.delayCall += EnsureOpenSupportedScenes;
        EditorApplication.delayCall += EnsureAndSaveSupportedScenesOnce;
    }

    [MenuItem("Tools/Lighting/Ensure DayNight Controllers In Open Scenes")]
    public static void EnsureOpenSupportedScenesMenu()
    {
        EnsureOpenSupportedScenes();
    }

    [MenuItem("Tools/Lighting/Ensure And Save DayNight Controllers")]
    public static void EnsureAndSaveSupportedScenesMenu()
    {
        EnsureAndSaveSupportedScenes();
    }

    public static void EnsureOpenSupportedScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return;
        }

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            EnsureSceneController(scene, markDirty: true);
        }
    }

    public static void EnsureAndSaveSupportedScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return;
        }

        Scene previousActiveScene = SceneManager.GetActiveScene();
        var openedTemporarily = new System.Collections.Generic.List<Scene>();

        try
        {
            string[] scenePaths = { PrimaryScenePath, TownScenePath };
            for (int i = 0; i < scenePaths.Length; i++)
            {
                string scenePath = scenePaths[i];
                Scene scene = SceneManager.GetSceneByPath(scenePath);
                bool openedByThisPass = false;
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    openedByThisPass = true;
                }

                bool changed = EnsureSceneController(scene, markDirty: true);
                Log($"scene={scene.path} changed={changed} dirty={scene.isDirty}");
                if (changed || scene.isDirty)
                {
                    Log($"saving scene={scene.path}");
                    EditorSceneManager.SaveScene(scene);
                }

                if (openedByThisPass)
                {
                    openedTemporarily.Add(scene);
                }
            }
        }
        finally
        {
            for (int i = openedTemporarily.Count - 1; i >= 0; i--)
            {
                if (openedTemporarily[i].IsValid() && openedTemporarily[i].isLoaded)
                {
                    EditorSceneManager.CloseScene(openedTemporarily[i], true);
                }
            }

            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
            {
                SceneManager.SetActiveScene(previousActiveScene);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static bool EnsureSceneController(Scene scene, bool markDirty)
    {
        if (!scene.IsValid() || !scene.isLoaded || !IsSupportedScene(scene.path))
        {
            return false;
        }

        GameObject persistentRoot = FindSceneObject(scene, PersistentManagersName);
        if (persistentRoot == null)
        {
            Log($"scene={scene.path} missing PersistentManagers");
            return false;
        }

        bool changed = false;
        GameObject managerObject = FindChildByName(persistentRoot.transform, DayNightManagerName)?.gameObject;
        if (managerObject == null)
        {
            managerObject = new GameObject(DayNightManagerName);
            managerObject.transform.SetParent(persistentRoot.transform, false);
            changed = true;
            Log($"scene={scene.path} created DayNightManager");
        }
        else if (managerObject.transform.parent != persistentRoot.transform)
        {
            managerObject.transform.SetParent(persistentRoot.transform, false);
            changed = true;
            Log($"scene={scene.path} reparented DayNightManager");
        }

        managerObject.transform.localPosition = Vector3.zero;
        managerObject.transform.localRotation = Quaternion.identity;
        managerObject.transform.localScale = Vector3.one;

        DayNightManager manager = EnsureComponent<DayNightManager>(managerObject, ref changed);
        GameObject overlayObject = FindOrCreateChild(managerObject.transform, OverlayName, ref changed);
        DayNightOverlay overlay = EnsureComponent<DayNightOverlay>(overlayObject, ref changed);
        EnsureComponent<SpriteRenderer>(overlayObject, ref changed);

        GameObject globalLightObject = FindOrCreateChild(managerObject.transform, GlobalLightName, ref changed);
        GlobalLightController globalLight = EnsureComponent<GlobalLightController>(globalLightObject, ref changed);

        GameObject pointLightManagerObject = FindOrCreateChild(managerObject.transform, PointLightManagerName, ref changed);
        PointLightManager pointLightManager = EnsureComponent<PointLightManager>(pointLightManagerObject, ref changed);

        changed |= ApplySerializedReferences(manager, overlay, globalLight, pointLightManager);

        if (changed && markDirty)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorUtility.SetDirty(manager);
            Log($"scene={scene.path} marked dirty");
        }

        return changed;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode _)
    {
        EditorApplication.delayCall += () =>
        {
            if (scene.IsValid())
            {
                EnsureSceneController(scene, markDirty: true);
                DayNightManager.EditorRefreshAllManagers();
            }
        };
    }

    private static void EnsureAndSaveSupportedScenesOnce()
    {
        if (initialSceneSyncDone)
        {
            return;
        }

        initialSceneSyncDone = true;
        EnsureAndSaveSupportedScenes();
    }

    private static bool IsSupportedScene(string scenePath)
    {
        return scenePath == PrimaryScenePath || scenePath == TownScenePath;
    }

    private static GameObject FindSceneObject(Scene scene, string objectName)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            Transform found = FindChildByName(roots[i].transform, objectName);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        return null;
    }

    private static Transform FindChildByName(Transform root, string childName)
    {
        if (root.name == childName)
        {
            return root;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform match = FindChildByName(root.GetChild(i), childName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static GameObject FindOrCreateChild(Transform parent, string childName, ref bool changed)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject created = new GameObject(childName);
        created.transform.SetParent(parent, false);
        changed = true;
        return created;
    }

    private static T EnsureComponent<T>(GameObject target, ref bool changed) where T : Component
    {
        T existing = target.GetComponent<T>();
        if (existing != null)
        {
            return existing;
        }

        changed = true;
        return target.AddComponent<T>();
    }

    private static bool ApplySerializedReferences(
        DayNightManager manager,
        DayNightOverlay overlay,
        GlobalLightController globalLight,
        PointLightManager pointLightManager)
    {
        bool changed = false;
        SerializedObject so = new SerializedObject(manager);

        SerializedProperty configProp = so.FindProperty("config");
        DayNightConfig configAsset = AssetDatabase.LoadAssetAtPath<DayNightConfig>(ConfigAssetPath);
        if (configProp != null && configAsset != null && configProp.objectReferenceValue != configAsset)
        {
            configProp.objectReferenceValue = configAsset;
            changed = true;
        }

        SerializedProperty overlayProp = so.FindProperty("overlay");
        if (overlayProp != null && overlayProp.objectReferenceValue != overlay)
        {
            overlayProp.objectReferenceValue = overlay;
            changed = true;
        }

        SerializedProperty globalLightProp = so.FindProperty("globalLight");
        if (globalLightProp != null && globalLightProp.objectReferenceValue != globalLight)
        {
            globalLightProp.objectReferenceValue = globalLight;
            changed = true;
        }

        SerializedProperty pointLightProp = so.FindProperty("pointLightMgr");
        if (pointLightProp != null && pointLightProp.objectReferenceValue != pointLightManager)
        {
            pointLightProp.objectReferenceValue = pointLightManager;
            changed = true;
        }

        if (changed)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        return changed;
    }

    private static void Log(string message)
    {
        if (!VerboseSyncLog)
        {
            return;
        }

        Debug.Log($"[EnsureDayNightSceneControllers] {message}");
    }
}
