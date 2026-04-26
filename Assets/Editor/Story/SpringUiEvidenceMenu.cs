using System;
using System.Collections;
using System.Reflection;
using FarmGame.Data;
using Sunset.Story;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SpringUiEvidenceMenu
{
    private const string CaptureMenuPath = "Sunset/Story/Debug/Capture Spring UI Evidence";
    private const string BootstrapAndCaptureMenuPath = "Sunset/Story/Debug/Bootstrap + Capture Spring UI Evidence";
    private const string PlayDialogueAndCaptureMenuPath = "Sunset/Story/Debug/Play Dialogue + Capture Spring UI Evidence";
    private const string OpenWorkbenchMenuPath = "Sunset/Story/Debug/Open Spring Day1 Workbench Overlay";
    private const string OpenWorkbenchAndCaptureMenuPath = "Sunset/Story/Debug/Open Workbench + Capture Spring UI Evidence";
    private const string OpenPickaxeWorkbenchAndCaptureMenuPath = "Sunset/Story/Debug/Open Pickaxe Workbench + Capture Spring UI Evidence";
    private const string OpenPackageMapMenuPath = "Sunset/Story/Debug/Open Package Map Page";
    private const string OpenPackageMapAndCaptureMenuPath = "Sunset/Story/Debug/Open Package Map + Capture Spring UI Evidence";
    private const string OpenPackageRelationsMenuPath = "Sunset/Story/Debug/Open Package Relations Page";
    private const string OpenPackageRelationsAndCaptureMenuPath = "Sunset/Story/Debug/Open Package Relations + Capture Spring UI Evidence";
    private const string RunWorkbenchCraftExitProbeMenuPath = "Sunset/Story/Debug/Run Spring Day1 Workbench Craft Exit Probe";
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

    [MenuItem(PlayDialogueAndCaptureMenuPath)]
    private static void PlayDialogueAndCaptureSpringUiEvidence()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再执行 Dialogue + Capture。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[SpringUiEvidence] {runner.BootstrapRuntime()}");
        runner.LogSnapshot("ui-evidence-dialogue");

        if (!EditorApplication.ExecuteMenuItem("Sunset/Story/Debug/Play Spring Day1 Dialogue"))
        {
            Debug.LogError("[SpringUiEvidence] 未能触发 Play Spring Day1 Dialogue 菜单。");
            return;
        }

        Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("dialogue"));
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

    [MenuItem(OpenPackageMapMenuPath)]
    private static void OpenPackageMapForEvidence()
    {
        OpenPackagePageForEvidence(openRelations: false, captureTag: null);
    }

    [MenuItem(OpenPackageMapAndCaptureMenuPath)]
    private static void OpenPackageMapAndCaptureForEvidence()
    {
        OpenPackagePageForEvidence(openRelations: false, captureTag: "package-map");
    }

    [MenuItem(OpenPackageRelationsMenuPath)]
    private static void OpenPackageRelationsForEvidence()
    {
        OpenPackagePageForEvidence(openRelations: true, captureTag: null);
    }

    [MenuItem(OpenPackageRelationsAndCaptureMenuPath)]
    private static void OpenPackageRelationsAndCaptureForEvidence()
    {
        OpenPackagePageForEvidence(openRelations: true, captureTag: "package-relations");
    }

    [MenuItem(RunWorkbenchCraftExitProbeMenuPath)]
    private static void RunWorkbenchCraftExitProbe()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再执行工作台离台制作 probe。");
            return;
        }

        OpenWorkbenchOverlayForEvidence();

        SpringDay1WorkbenchCraftingOverlay overlay = SpringDay1WorkbenchCraftingOverlay.Instance;
        if (overlay == null)
        {
            Debug.LogError("[SpringUiEvidence] 未能获取 SpringDay1WorkbenchCraftingOverlay。");
            return;
        }

        Transform playerTransform = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include)?.transform;
        Transform anchor = FindWorkbenchAnchor();
        if (playerTransform == null || anchor == null)
        {
            Debug.LogError("[SpringUiEvidence] 缺少 player 或 workbench，无法执行离台制作 probe。");
            return;
        }

        WorkbenchProbeRunner runner = overlay.gameObject.GetComponent<WorkbenchProbeRunner>();
        if (runner == null)
        {
            runner = overlay.gameObject.AddComponent<WorkbenchProbeRunner>();
        }

        runner.Run(overlay, anchor, playerTransform);
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

    private static void OpenPackagePageForEvidence(bool openRelations, string captureTag)
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[SpringUiEvidence] 请先进入 PlayMode，再打开 Package 页面。");
            return;
        }

        SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
        Debug.Log($"[SpringUiEvidence] {runner.BootstrapRuntime()}");

        SpringDay1PromptOverlay.EnsureRuntime();
        SpringDay1PromptOverlay.Instance?.Hide();

        PackagePanelTabsUI packageTabs = UnityEngine.Object.FindFirstObjectByType<PackagePanelTabsUI>(FindObjectsInactive.Include);
        if (packageTabs == null)
        {
            Debug.LogError("[SpringUiEvidence] 未找到 PackagePanelTabsUI。");
            return;
        }

        packageTabs.EnsureReady();
        if (openRelations)
        {
            packageTabs.OpenRelations();
            runner.LogSnapshot("ui-evidence-package-relations");
            Debug.Log("[SpringUiEvidence] 已直接打开 Package 关系页。");
        }
        else
        {
            packageTabs.OpenMap();
            runner.LogSnapshot("ui-evidence-package-map");
            Debug.Log("[SpringUiEvidence] 已直接打开 Package 地图页。");
        }

        if (!string.IsNullOrWhiteSpace(captureTag))
        {
            Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence(captureTag));
        }
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

    private static T FindDescendantComponent<T>(Transform root, string name) where T : Component
    {
        Transform target = FindDescendant(root, name);
        return target != null ? target.GetComponent<T>() : null;
    }

    private static RectTransform FindDescendantRect(Transform root, string name)
    {
        return FindDescendant(root, name) as RectTransform;
    }

    private static RectTransform FindDirectChildRect(Transform parent, string name)
    {
        if (parent == null)
        {
            return null;
        }

        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (string.Equals(child.name, name, StringComparison.Ordinal))
            {
                return child as RectTransform;
            }
        }

        return null;
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        if (root == null)
        {
            return null;
        }

        for (int index = 0; index < root.childCount; index++)
        {
            Transform child = root.GetChild(index);
            if (string.Equals(child.name, name, StringComparison.Ordinal))
            {
                return child;
            }

            Transform nested = FindDescendant(child, name);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

    private sealed class WorkbenchProbeRunner : MonoBehaviour
    {
        private static readonly FieldInfo DisplayBelowField =
            typeof(SpringDay1WorkbenchCraftingOverlay).GetField("_displayBelow", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo RecipesField =
            typeof(SpringDay1WorkbenchCraftingOverlay).GetField("_recipes", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo SelectRecipeMethod =
            typeof(SpringDay1WorkbenchCraftingOverlay).GetMethod("SelectRecipe", BindingFlags.Instance | BindingFlags.NonPublic);

        private Coroutine _probeCoroutine;

        public void Run(SpringDay1WorkbenchCraftingOverlay overlay, Transform anchor, Transform playerTransform)
        {
            if (_probeCoroutine != null)
            {
                StopCoroutine(_probeCoroutine);
            }

            _probeCoroutine = StartCoroutine(RunProbe(overlay, anchor, playerTransform));
        }

        private IEnumerator RunProbe(SpringDay1WorkbenchCraftingOverlay overlay, Transform anchor, Transform playerTransform)
        {
            Vector3 originalPlayerPosition = playerTransform.position;
            Quaternion originalPlayerRotation = playerTransform.rotation;

            try
            {
                if (!TrySelectWorkbenchRecipeByKeyword(overlay, "sword"))
                {
                    TrySelectFirstRecipe(overlay);
                }

                yield return null;
                LogRecipeSnapshot(overlay);

                Vector3 anchorPosition = anchor.position;
                playerTransform.position = anchorPosition + new Vector3(0f, -0.35f, 0f);
                yield return null;
                bool belowSouth = ReadDisplayBelow(overlay);

                playerTransform.position = anchorPosition + new Vector3(0f, 0.35f, 0f);
                yield return null;
                bool belowNorth = ReadDisplayBelow(overlay);

                Button craftButton = SpringUiEvidenceMenu.FindDescendantComponent<Button>(overlay.transform, "CraftButton");
                RectTransform panelRect = SpringUiEvidenceMenu.FindDescendantRect(overlay.transform, "PanelRoot");
                RectTransform floatingRoot = SpringUiEvidenceMenu.FindDirectChildRect(overlay.transform as RectTransform, "FloatingProgressRoot");
                if (craftButton == null)
                {
                    Debug.LogError("[SpringUiEvidence] Workbench probe 未找到 CraftButton。");
                    yield break;
                }

                craftButton.onClick.Invoke();
                yield return null;
                yield return null;

                bool switchedToOtherRecipe = TrySelectWorkbenchRecipeByKeyword(overlay, "hoe");
                if (!switchedToOtherRecipe)
                {
                    switchedToOtherRecipe = TrySelectWorkbenchRecipeByKeyword(overlay, "pickaxe");
                }

                yield return null;
                yield return null;

                TextMeshProUGUI selectedName = panelRect != null ? SpringUiEvidenceMenu.FindDescendantComponent<TextMeshProUGUI>(panelRect, "SelectedName") : null;
                TextMeshProUGUI progressLabel = panelRect != null ? SpringUiEvidenceMenu.FindDescendantComponent<TextMeshProUGUI>(panelRect, "ProgressLabel") : null;
                Debug.Log(
                    $"[SpringUiEvidence] WorkbenchSelectionIsolation => switched={switchedToOtherRecipe}, " +
                    $"selected='{(selectedName != null ? selectedName.text : "n/a")}', " +
                    $"progress='{(progressLabel != null ? progressLabel.text : "n/a")}'");

                playerTransform.position = anchorPosition + new Vector3(2.3f, 0f, 0f);
                yield return null;
                yield return null;

                bool floatingVisible = floatingRoot != null && floatingRoot.gameObject.activeInHierarchy;
                TextMeshProUGUI floatingLabel = floatingRoot != null ? SpringUiEvidenceMenu.FindDescendantComponent<TextMeshProUGUI>(floatingRoot, "Label") : null;
                Image floatingFill = floatingRoot != null ? SpringUiEvidenceMenu.FindDescendantComponent<Image>(floatingRoot, "ProgressFill") : null;
                float panelY = panelRect != null ? panelRect.anchoredPosition.y : float.NaN;

                Debug.Log(
                    $"[SpringUiEvidence] WorkbenchCraftExitProbe => belowSouth={belowSouth}, belowNorth={belowNorth}, " +
                    $"switchOk={belowSouth != belowNorth}, panelY={panelY:F2}, floatingVisible={floatingVisible}, " +
                    $"floatingLabel='{(floatingLabel != null ? floatingLabel.text : "n/a")}', " +
                    $"floatingFill={(floatingFill != null ? floatingFill.fillAmount.ToString("F2") : "n/a")}");

                Debug.Log(SpringUiEvidenceCaptureRuntime.CaptureCurrentSpringUiEvidence("workbench-craft-exit-probe"));
            }
            finally
            {
                playerTransform.position = originalPlayerPosition;
                playerTransform.rotation = originalPlayerRotation;
                _probeCoroutine = null;
            }
        }

        private static void TrySelectFirstRecipe(SpringDay1WorkbenchCraftingOverlay overlay)
        {
            if (SelectRecipeMethod == null || RecipesField == null)
            {
                return;
            }

            if (RecipesField.GetValue(overlay) is IList recipes && recipes.Count > 0)
            {
                SelectRecipeMethod.Invoke(overlay, new object[] { 0 });
            }
        }

        private static void LogRecipeSnapshot(SpringDay1WorkbenchCraftingOverlay overlay)
        {
            if (RecipesField == null || RecipesField.GetValue(overlay) is not IList recipes)
            {
                return;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int index = 0; index < recipes.Count; index++)
            {
                if (recipes[index] is not RecipeData recipe)
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append(" | ");
                }

                builder.Append(recipe.recipeName).Append("@").Append(recipe.craftingTime.ToString("F1"));
            }

            Debug.Log($"[SpringUiEvidence] Workbench recipes => {builder}");
        }

        private static bool ReadDisplayBelow(SpringDay1WorkbenchCraftingOverlay overlay)
        {
            return DisplayBelowField != null && (bool)DisplayBelowField.GetValue(overlay);
        }
    }
}
