using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class StaticObjectOrderAutoCalibrator
{
    private const string ManualCalibrateMenuPath = "Tools/Static Order/Calibrate All Static Objects";
    private const string AutoCalibrateBeforePlayMenuPath = "Tools/Static Order/Auto Calibrate Before Play";
    private const string AutoCalibrateBeforePlayPrefKey = "Sunset.StaticObjectOrderAutoCalibrator.AutoCalibrateBeforePlay";

    private const int Multiplier = 100;
    private const int OrderOffset = 0;
    private const float BottomOffset = 0f;
    private const int ShadowOffset = -1;
    private const int GlowOffset = 0;

    static StaticObjectOrderAutoCalibrator()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
        {
            return;
        }

        if (!IsAutoCalibrateBeforePlayEnabled())
        {
            return;
        }

        CalibrateAllStaticObjects();
    }

    [MenuItem(ManualCalibrateMenuPath)]
    public static void ManualCalibrate()
    {
        CalibrateAllStaticObjects();
    }

    [MenuItem(AutoCalibrateBeforePlayMenuPath)]
    private static void ToggleAutoCalibrateBeforePlay()
    {
        bool enabled = !IsAutoCalibrateBeforePlayEnabled();
        EditorPrefs.SetBool(AutoCalibrateBeforePlayPrefKey, enabled);
        Menu.SetChecked(AutoCalibrateBeforePlayMenuPath, enabled);
        Debug.Log($"[AutoCalibrator] Auto calibrate before play: {(enabled ? "enabled" : "disabled")}");
    }

    [MenuItem(AutoCalibrateBeforePlayMenuPath, true)]
    private static bool ValidateAutoCalibrateBeforePlayMenu()
    {
        Menu.SetChecked(AutoCalibrateBeforePlayMenuPath, IsAutoCalibrateBeforePlayEnabled());
        return true;
    }

    private static void CalibrateAllStaticObjects()
    {
        Debug.Log("<color=cyan>========== Start static order calibration ==========</color>");

        CleanEmptySpriteRenderers();

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<SpriteRenderer> staticRenderers = new List<SpriteRenderer>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<DynamicSortingOrder>() != null)
            {
                continue;
            }

            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in renderers)
            {
                if (!HasDynamicSortingOrderInHierarchy(sr.gameObject))
                {
                    staticRenderers.Add(sr);
                }
            }
        }

        if (staticRenderers.Count == 0)
        {
            Debug.Log("<color=yellow>[AutoCalibrator] No static sprite renderers found.</color>");
            return;
        }

        int calibratedCount = 0;
        int skippedCount = 0;

        foreach (SpriteRenderer sr in staticRenderers)
        {
            if (sr.sortingOrder < -9990)
            {
                skippedCount++;
                continue;
            }

            float sortingY = CalculateSortingY(sr);
            int calculatedOrder = -Mathf.RoundToInt(sortingY * Multiplier) + OrderOffset;

            string loweredName = sr.gameObject.name.ToLowerInvariant();
            if (loweredName.Contains("shadow"))
            {
                Transform parent = sr.transform.parent;
                if (parent != null)
                {
                    SpriteRenderer parentSr = parent.GetComponent<SpriteRenderer>();
                    if (parentSr != null)
                    {
                        float parentSortY = CalculateSortingY(parentSr);
                        int parentOrder = -Mathf.RoundToInt(parentSortY * Multiplier) + OrderOffset;
                        calculatedOrder = parentOrder + ShadowOffset;
                    }
                }
            }
            else if (loweredName.Contains("glow") || loweredName.Contains("light") || loweredName.Contains("effect"))
            {
                Transform parent = sr.transform.parent;
                if (parent != null)
                {
                    SpriteRenderer parentSr = parent.GetComponent<SpriteRenderer>();
                    if (parentSr != null)
                    {
                        float parentSortY = CalculateSortingY(parentSr);
                        int parentOrder = -Mathf.RoundToInt(parentSortY * Multiplier) + OrderOffset;
                        calculatedOrder = parentOrder + GlowOffset;
                    }
                }
            }

            if (sr.sortingOrder == calculatedOrder)
            {
                continue;
            }

            Undo.RecordObject(sr, "Auto Calibrate Static Objects Order");
            sr.sortingOrder = calculatedOrder;
            EditorUtility.SetDirty(sr);
            calibratedCount++;
        }

        if (calibratedCount > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        Debug.Log(
            $"<color=green>[AutoCalibrator] Done.</color>\n" +
            $"  calibrated: {calibratedCount}\n" +
            $"  skipped: {skippedCount}\n" +
            $"  total: {staticRenderers.Count}");
        Debug.Log("<color=cyan>========================================</color>");
    }

    private static float CalculateSortingY(SpriteRenderer sr)
    {
        Transform parent = sr.transform.parent;
        if (parent != null)
        {
            SpriteRenderer parentSr = parent.GetComponent<SpriteRenderer>();
            if (parentSr == null)
            {
                return parent.position.y + BottomOffset;
            }
        }

        Collider2D collider = sr.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds.min.y + BottomOffset;
        }

        if (sr.sprite != null)
        {
            return sr.bounds.min.y + BottomOffset;
        }

        return sr.transform.position.y + BottomOffset;
    }

    private static void CleanEmptySpriteRenderers()
    {
        OcclusionTransparency[] occlusionObjects = Object.FindObjectsByType<OcclusionTransparency>(FindObjectsSortMode.None);
        int cleanedCount = 0;

        foreach (OcclusionTransparency occlusion in occlusionObjects)
        {
            SpriteRenderer sr = occlusion.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite != null)
            {
                continue;
            }

            Undo.DestroyObjectImmediate(sr);
            cleanedCount++;
            Debug.Log($"<color=yellow>[AutoCalibrator] Removed empty SpriteRenderer from {occlusion.gameObject.name}</color>");
        }

        if (cleanedCount > 0)
        {
            Debug.Log($"<color=green>[AutoCalibrator] Removed {cleanedCount} empty SpriteRenderer components.</color>");
        }
    }

    private static bool HasDynamicSortingOrderInHierarchy(GameObject obj)
    {
        Transform current = obj.transform;
        while (current != null)
        {
            if (current.GetComponent<DynamicSortingOrder>() != null)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private static bool IsAutoCalibrateBeforePlayEnabled()
    {
        return EditorPrefs.GetBool(AutoCalibrateBeforePlayPrefKey, false);
    }
}
