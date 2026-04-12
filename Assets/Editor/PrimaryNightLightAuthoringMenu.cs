using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 为 Primary 场景批量生成可手动摆放的夜灯锚点。
/// 不自动绑死位置；先给出一组可移动对象，后续由用户自行挪到合适位置。
/// </summary>
public static class PrimaryNightLightAuthoringMenu
{
    private const string PrimaryScenePath = "Assets/000_Scenes/Primary.unity";
    private const string SceneRootPath = "SCENE/LAYER 1";
    private const string AnchorRootName = "NightLightAnchors";

    private static readonly string[] AnchorNames =
    {
        "NightLight_01",
        "NightLight_02",
        "NightLight_03",
        "NightLight_04",
        "NightLight_05",
        "NightLight_06"
    };

    private static readonly Vector3[] DefaultLocalPositions =
    {
        new Vector3(-2.5f, 1.5f, 0f),
        new Vector3(0f, 1.5f, 0f),
        new Vector3(2.5f, 1.5f, 0f),
        new Vector3(-2.5f, -1.5f, 0f),
        new Vector3(0f, -1.5f, 0f),
        new Vector3(2.5f, -1.5f, 0f)
    };

    [MenuItem("Tools/Lighting/Create Primary Night Light Anchors")]
    public static void CreatePrimaryNightLightAnchors()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path != PrimaryScenePath)
        {
            EditorUtility.DisplayDialog(
                "Primary 未打开",
                "请先把 Primary 场景设为当前活动场景，再执行这一键生成。",
                "知道了");
            return;
        }

        GameObject layerRoot = GameObject.Find(SceneRootPath);
        if (layerRoot == null)
        {
            EditorUtility.DisplayDialog(
                "缺少场景父节点",
                "未找到 SCENE/LAYER 1，暂时无法放置夜灯锚点。",
                "知道了");
            return;
        }

        GameObject anchorRoot = FindOrCreateChild(layerRoot.transform, AnchorRootName);
        Undo.RegisterFullObjectHierarchyUndo(anchorRoot, "Create Primary Night Light Anchors");

        for (int i = 0; i < AnchorNames.Length; i++)
        {
            GameObject anchor = FindOrCreateChild(anchorRoot.transform, AnchorNames[i]);
            anchor.transform.localPosition = DefaultLocalPositions[i];
            anchor.transform.localRotation = Quaternion.identity;
            anchor.transform.localScale = Vector3.one;

            NightLightMarker marker = anchor.GetComponent<NightLightMarker>();
            if (marker == null)
            {
                marker = Undo.AddComponent<NightLightMarker>(anchor);
            }
        }

        EditorSceneManager.MarkSceneDirty(activeScene);
        Selection.activeGameObject = anchorRoot;
    }

    private static GameObject FindOrCreateChild(Transform parent, string childName)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject created = new GameObject(childName);
        Undo.RegisterCreatedObjectUndo(created, "Create Night Light Anchor");
        created.transform.SetParent(parent, false);
        return created;
    }
}
