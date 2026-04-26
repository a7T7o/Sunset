using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 旧版存档调试入口已经退役。
/// 保留这个组件名只是为了兼容场景里已挂上的脚本引用，并在运行时主动清理遗留的 F5/F9 调试面板。
/// </summary>
[DisallowMultipleComponent]
public sealed class SaveLoadDebugUI : MonoBehaviour
{
    private const string LegacyCanvasName = "SaveLoadDebugCanvas";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void DisableLegacyPanelsAfterSceneLoad()
    {
        SaveLoadDebugUI[] components = FindObjectsByType<SaveLoadDebugUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < components.Length; index++)
        {
            if (components[index] != null)
            {
                components[index].DisableAndCleanup();
            }
        }

        DestroyLooseLegacyCanvases();
    }

    private void Awake()
    {
        DisableAndCleanup();
    }

    private void OnEnable()
    {
        DisableAndCleanup();
    }

    private void DisableAndCleanup()
    {
        DestroyLegacyCanvasChildren(transform);
        DestroyLooseLegacyCanvases();

        if (this != null)
        {
            Destroy(this);
        }
    }

    private static void DestroyLegacyCanvasChildren(Transform root)
    {
        if (root == null)
        {
            return;
        }

        Canvas[] canvases = root.GetComponentsInChildren<Canvas>(includeInactive: true);
        for (int index = 0; index < canvases.Length; index++)
        {
            Canvas canvas = canvases[index];
            if (canvas != null && canvas.name == LegacyCanvasName)
            {
                Destroy(canvas.gameObject);
            }
        }
    }

    private static void DestroyLooseLegacyCanvases()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < canvases.Length; index++)
        {
            Canvas canvas = canvases[index];
            if (canvas != null && canvas.name == LegacyCanvasName)
            {
                Destroy(canvas.gameObject);
            }
        }

        Text[] labels = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int index = 0; index < labels.Length; index++)
        {
            Text label = labels[index];
            if (label == null)
            {
                continue;
            }

            string content = label.text ?? string.Empty;
            if (content.Contains("F5=保存") || content.Contains("保存 (F5)") || content.Contains("加载 (F9)"))
            {
                Transform ancestorCanvas = label.transform;
                while (ancestorCanvas != null && ancestorCanvas.name != LegacyCanvasName)
                {
                    ancestorCanvas = ancestorCanvas.parent;
                }

                if (ancestorCanvas != null)
                {
                    Destroy(ancestorCanvas.gameObject);
                }
            }
        }
    }
}
