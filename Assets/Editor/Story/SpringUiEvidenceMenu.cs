using System;
using System.Collections;
using System.Reflection;
using FarmGame.Data;
using Sunset.Story;
using UnityEditor;
using UnityEngine;

public static class SpringUiEvidenceMenu
{
    private const string CaptureMenuPath = "Sunset/Story/Debug/Capture Spring UI Evidence";
    private const string BootstrapAndCaptureMenuPath = "Sunset/Story/Debug/Bootstrap + Capture Spring UI Evidence";
    private const string OpenWorkbenchMenuPath = "Sunset/Story/Debug/Open Spring Day1 Workbench Overlay";
    private const string OpenWorkbenchAndCaptureMenuPath = "Sunset/Story/Debug/Open Workbench + Capture Spring UI Evidence";
    private const string OpenPickaxeWorkbenchAndCaptureMenuPath = "Sunset/Story/Debug/Open Pickaxe Workbench + Capture Spring UI Evidence";
    private const string PromoteMenuPath = "Sunset/Story/Debug/Promote Latest Spring UI Evidence";
    private const string PruneDryRunMenuPath = "Sunset/Story/Debug/Prune Spring UI Pending Evidence (Dry Run)";
    private const string PruneMenuPath = "Sunset/Story/Debug/Prune Spring UI Pending Evidence (14d)";
    private static readonly string[] PreferredWorkbenchObjectNames = { "Anvil_0", "Workbench", "Anvil" };

    [MenuItem(CaptureMenuPath)]
    private static void CaptureSpringUiEvidence()
    {
        Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("manual"));
    }

    [MenuItem(BootstrapAndCaptureMenuPath)]
    private static void BootstrapAndCaptureSpringUiEvidence()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再执行 Bootstrap + Capture。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[SpringUiEvidence] {runner.BootstrapRuntime()}");
        runner.LogSnapshot("ui-evidence-bootstrap");
        Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("bootstrap"));
    }

    [MenuItem(OpenWorkbenchMenuPath)]
    private static void OpenWorkbenchOverlayForEvidence()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再打开工作台 UI。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[SpringUiEvidence] {runner.BootstrapRuntime()}");

        SpringDay1PromptOverlay.EnsureRuntime();
        SpringDay1PromptOverlay.Instance?.Hide();

        Transform anchor = FindWorkbenchAnchor();
        if (anchor == null)
        {
            Debug.LogError("[SpringUiEvidence] 未找到工作台交互物（Anvil_0 / Workbench / Anvil）。");
            return;
        }

        SpringDay1WorkbenchCraftingOverlay.EnsureRuntime();
        SpringDay1WorkbenchCraftingOverlay overlay = SpringDay1WorkbenchCraftingOverlay.Instance;
        if (overlay == null)
        {
            Debug.LogError("[SpringUiEvidence] 未能创建 SpringDay1WorkbenchCraftingOverlay。");
            return;
        }

        CraftingService craftingService = ResolveCraftingServiceForEvidence();
        if (craftingService == null)
        {
            Debug.LogError("[SpringUiEvidence] 未找到可用的 CraftingService。");
            return;
        }

        Transform playerTransform = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include)?.transform;
        overlay.Open(anchor, playerTransform, craftingService, 999f);
        runner.LogSnapshot("ui-evidence-workbench-open");
        Debug.Log($"[SpringUiEvidence] 已直接打开工作台 UI：{anchor.name}");
    }

    [MenuItem(OpenWorkbenchAndCaptureMenuPath)]
    private static void OpenWorkbenchAndCaptureSpringUiEvidence()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再执行 Workbench + Capture。");
            return;
        }

        OpenWorkbenchOverlayForEvidence();
        Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("workbench"));
    }

    [MenuItem(OpenPickaxeWorkbenchAndCaptureMenuPath)]
    private static void OpenPickaxeWorkbenchAndCaptureSpringUiEvidence()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再执行 Pickaxe Workbench + Capture。");
            return;
        }

        OpenWorkbenchOverlayForEvidence();

        SpringDay1WorkbenchCraftingOverlay overlay = SpringDay1WorkbenchCraftingOverlay.Instance;
        if (overlay == null)
        {
            Debug.LogError("[SpringUiEvidence] 未能获取 SpringDay1WorkbenchCraftingOverlay。");
            return;
        }

        if (!TrySelectWorkbenchRecipeByKeyword(overlay, "pickaxe"))
        {
            Debug.LogWarning("[SpringUiEvidence] 未找到 Pickaxe 配方，将继续抓取当前工作台画面。");
        }

        Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("workbench-pickaxe"));
    }

    [MenuItem(PromoteMenuPath)]
    private static void PromoteLatestSpringUiEvidence()
    {
        Debug.Log(SpringUiEvidenceCaptureRuntime.PromoteLatestCaptureToAccepted("menu"));
    }

    [MenuItem(PruneDryRunMenuPath)]
    private static void PrunePendingSpringUiEvidenceDryRun()
    {
        Debug.Log(SpringUiEvidenceCaptureRuntime.PrunePendingCaptures(14, true, "menu"));
    }

    [MenuItem(PruneMenuPath)]
    private static void PrunePendingSpringUiEvidence()
    {
        Debug.Log(SpringUiEvidenceCaptureRuntime.PrunePendingCaptures(14, false, "menu"));
    }

    private static Transform FindWorkbenchAnchor()
    {
        for (int index = 0; index < PreferredWorkbenchObjectNames.Length; index++)
        {
            GameObject exactMatch = GameObject.Find(PreferredWorkbenchObjectNames[index]);
            if (exactMatch == null)
            {
                continue;
            }

            return exactMatch.transform;
        }

        CraftingStationInteractable interactable = UnityEngine.Object.FindFirstObjectByType<CraftingStationInteractable>(FindObjectsInactive.Include);
        return interactable != null ? interactable.transform : null;
    }

    private static CraftingService ResolveCraftingServiceForEvidence()
    {
        CraftingService service = UnityEngine.Object.FindFirstObjectByType<CraftingService>(FindObjectsInactive.Include);
        if (service != null)
        {
            return service;
        }

        GameObject runtimeObject = new GameObject(nameof(CraftingService));
        return runtimeObject.AddComponent<CraftingService>();
    }

    private static bool TrySelectWorkbenchRecipeByKeyword(SpringDay1WorkbenchCraftingOverlay overlay, string keyword)
    {
        if (overlay == null || string.IsNullOrWhiteSpace(keyword))
        {
            return false;
        }

        FieldInfo recipesField = typeof(SpringDay1WorkbenchCraftingOverlay).GetField("_recipes", BindingFlags.Instance | BindingFlags.NonPublic);
        MethodInfo selectRecipeMethod = typeof(SpringDay1WorkbenchCraftingOverlay).GetMethod("SelectRecipe", BindingFlags.Instance | BindingFlags.NonPublic);
        if (recipesField == null || selectRecipeMethod == null)
        {
            return false;
        }

        if (recipesField.GetValue(overlay) is not IList recipes)
        {
            return false;
        }

        for (int index = 0; index < recipes.Count; index++)
        {
            if (recipes[index] is not RecipeData recipe)
            {
                continue;
            }

            string recipeName = recipe.recipeName ?? string.Empty;
            if (recipeName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            selectRecipeMethod.Invoke(overlay, new object[] { index });
            return true;
        }

        return false;
    }
}
