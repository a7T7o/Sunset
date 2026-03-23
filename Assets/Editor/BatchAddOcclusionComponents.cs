using UnityEditor;
using UnityEngine;

/// <summary>
/// 批量给树/房屋等父物体补齐当前版本遮挡组件链路。
/// 该工具只写 OcclusionTransparency 仍然存在的序列化字段，
/// 避免继续写入已经删除的 affectChildren / occlusionTags。
/// </summary>
public class BatchAddOcclusionComponents : Editor
{
    [MenuItem("Tools/批量添加遮挡组件（CompositeCollider）")]
    private static void AddOcclusionComponents()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先选中需要处理的父物体。", "确定");
            return;
        }

        int successCount = 0;
        int skippedCount = 0;

        foreach (GameObject parentObj in selected)
        {
            Undo.RecordObject(parentObj, "Add Occlusion Components");

            if (ShouldSkip(parentObj))
            {
                skippedCount++;
                continue;
            }

            Transform treeChild = parentObj.transform.Find("Tree");
            if (treeChild == null)
            {
                Debug.LogWarning($"[{parentObj.name}] 未找到 Tree 子物体，跳过。");
                skippedCount++;
                continue;
            }

            PolygonCollider2D treePoly = treeChild.GetComponent<PolygonCollider2D>();
            if (treePoly == null)
            {
                Debug.LogWarning($"[{parentObj.name}] Tree 子物体缺少 PolygonCollider2D，跳过。");
                skippedCount++;
                continue;
            }

            EnsureStaticBody(parentObj);
            EnsureMergedPolygon(treeChild.gameObject, treePoly);
            EnsureCompositeCollider(parentObj);
            RemoveParentSpriteRenderer(parentObj);
            EnsureOcclusionTransparency(parentObj);

            EditorUtility.SetDirty(parentObj);
            successCount++;
        }

        string message = $"成功处理：{successCount} 个物体\n";
        if (skippedCount > 0)
        {
            message += $"跳过：{skippedCount} 个物体（缺少 Tree 子物体或 PolygonCollider2D）\n";
        }

        message += "\n已补齐：\n" +
                   "- Rigidbody2D (Static)\n" +
                   "- CompositeCollider2D (Trigger)\n" +
                   "- OcclusionTransparency\n" +
                   "- Tree 子物体 PolygonCollider2D -> Composite Operation: Merge";

        EditorUtility.DisplayDialog("完成", message, "确定");
        Debug.Log($"<color=green>[批量添加遮挡组件] 成功: {successCount}, 跳过: {skippedCount}</color>");
    }

    [MenuItem("Tools/批量添加遮挡组件（CompositeCollider）", true)]
    private static bool ValidateAddOcclusionComponents()
    {
        return Selection.gameObjects.Length > 0;
    }

    private static bool ShouldSkip(GameObject parentObj)
    {
        string objName = parentObj.name.ToLowerInvariant();
        bool isSystemObject =
            objName.Contains("system") ||
            objName.Contains("manager") ||
            objName.Contains("service") ||
            objName.Contains("controller");

        if (isSystemObject)
        {
            Debug.LogWarning($"[{parentObj.name}] 跳过系统/管理器物体。");
        }

        return isSystemObject;
    }

    private static void EnsureStaticBody(GameObject parentObj)
    {
        Rigidbody2D rb = parentObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = parentObj.AddComponent<Rigidbody2D>();
        }

        if (rb.bodyType != RigidbodyType2D.Static)
        {
            rb.bodyType = RigidbodyType2D.Static;
            EditorUtility.SetDirty(parentObj);
        }
    }

    private static void EnsureMergedPolygon(GameObject treeChild, PolygonCollider2D treePoly)
    {
        if (treePoly.compositeOperation == Collider2D.CompositeOperation.Merge)
        {
            return;
        }

        treePoly.compositeOperation = Collider2D.CompositeOperation.Merge;
        EditorUtility.SetDirty(treeChild);
    }

    private static void EnsureCompositeCollider(GameObject parentObj)
    {
        CompositeCollider2D composite = parentObj.GetComponent<CompositeCollider2D>();
        if (composite == null)
        {
            composite = parentObj.AddComponent<CompositeCollider2D>();
        }

        composite.isTrigger = true;
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
        composite.generationType = CompositeCollider2D.GenerationType.Synchronous;
        composite.GenerateGeometry();
        EditorUtility.SetDirty(parentObj);
    }

    private static void RemoveParentSpriteRenderer(GameObject parentObj)
    {
        SpriteRenderer parentRenderer = parentObj.GetComponent<SpriteRenderer>();
        if (parentRenderer == null)
        {
            return;
        }

        DestroyImmediate(parentRenderer);
    }

    private static void EnsureOcclusionTransparency(GameObject parentObj)
    {
        OcclusionTransparency occlusion = parentObj.GetComponent<OcclusionTransparency>();
        if (occlusion == null)
        {
            occlusion = parentObj.AddComponent<OcclusionTransparency>();
        }

        SerializedObject so = new SerializedObject(occlusion);
        TrySetFloat(so, "occludedAlpha", 0.3f);
        TrySetFloat(so, "fadeSpeed", 8f);
        TrySetBool(so, "canBeOccluded", true);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void TrySetFloat(SerializedObject serializedObject, string propertyName, float value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.floatValue = value;
        }
    }

    private static void TrySetBool(SerializedObject serializedObject, string propertyName, bool value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.boolValue = value;
        }
    }
}
