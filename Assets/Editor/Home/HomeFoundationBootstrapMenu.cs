using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FarmGame.Data;
using FarmGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sunset.EditorTools.SceneSync
{
    internal static class HomeFoundationBootstrapMenu
    {
        private const string SourceScenePath = "Assets/000_Scenes/Primary.unity";
        private const string TargetScenePath = "Assets/000_Scenes/Home.unity";
        private const string SourceSceneContainerName = "Primary";
        private const string TargetSceneContainerName = "Home";
        private const string ArtifactDirectory = ".codex/artifacts/home-foundation";
        private static readonly BindingFlags InstanceBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly string[] RequiredRootNames =
        {
            "PersistentManagers",
            "UI",
            "HealthSystem",
            "SprintStateManager"
        };

        private static readonly string[] RequiredSceneChildren =
        {
            "Systems",
            "InventorySystem",
            "HotbarSelection",
            "EquipmentSystem",
            "EventSystem",
            "NavigationRoot",
            "TraversalBlockManager2D",
            "OcclusionManager"
        };

        [Serializable]
        private sealed class HomeFoundationReport
        {
            public string timestamp = string.Empty;
            public string sourceScenePath = SourceScenePath;
            public string targetScenePath = TargetScenePath;
            public string[] requiredRootNames = Array.Empty<string>();
            public string[] copiedRootNames = Array.Empty<string>();
            public string[] alreadyPresentRootNames = Array.Empty<string>();
            public string[] missingInSourceRootNames = Array.Empty<string>();
            public string[] verifiedRootNames = Array.Empty<string>();
            public bool savedTargetScene;
            public bool success;
            public string message = string.Empty;
            public string artifactPath = string.Empty;
        }

        [MenuItem("Tools/Sunset/Scene/Home基础骨架增量补齐（只增不删）")]
        private static void BootstrapHomeFoundation()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning("[HomeFoundationBootstrap] 当前 Unity 正在 PlayMode 或忙碌中，已阻断 Home 基础骨架补齐。");
                return;
            }

            HomeFoundationReport report = new HomeFoundationReport
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                requiredRootNames = RequiredRootNames.Concat(RequiredSceneChildren).ToArray()
            };

            try
            {
                if (!LoadedSceneContext.TryOpen(SourceScenePath, TargetScenePath, out LoadedSceneContext context, out string errorMessage))
                {
                    throw new InvalidOperationException(errorMessage);
                }

                using (context)
                {
                    if (context.SourceWasAlreadyLoaded && context.SourceScene.isDirty)
                    {
                        throw new InvalidOperationException("Primary 当前已在编辑器中打开且存在未保存修改，Home bootstrap 已阻断。");
                    }

                    if (context.TargetWasAlreadyLoaded && context.TargetScene.isDirty)
                    {
                        throw new InvalidOperationException("Home 当前已在编辑器中打开且存在未保存修改，Home bootstrap 已阻断。");
                    }

                    List<string> copied = new List<string>();
                    List<string> alreadyPresent = new List<string>();
                    List<string> missingInSource = new List<string>();

                    Undo.IncrementCurrentGroup();
                    int undoGroup = Undo.GetCurrentGroup();
                    Undo.SetCurrentGroupName("Home Foundation Bootstrap");

                    for (int index = 0; index < report.requiredRootNames.Length; index++)
                    {
                        string objectName = report.requiredRootNames[index];
                        Transform sourceTransform = FindSourceTransform(context.SourceScene, objectName);
                        if (sourceTransform == null)
                        {
                            missingInSource.Add(objectName);
                            continue;
                        }

                        Transform targetTransform = FindTargetTransform(context.TargetScene, objectName);
                        if (targetTransform != null)
                        {
                            alreadyPresent.Add(objectName);
                            continue;
                        }

                        Transform targetParent = ResolveTargetParent(context.TargetScene, sourceTransform);
                        GameObject clone = UnityEngine.Object.Instantiate(sourceTransform.gameObject);
                        clone.name = sourceTransform.name;
                        SceneManager.MoveGameObjectToScene(clone, context.TargetScene);
                        clone.transform.SetParent(targetParent, false);
                        clone.transform.SetSiblingIndex(sourceTransform.GetSiblingIndex());
                        Undo.RegisterCreatedObjectUndo(clone, "Home Foundation Bootstrap Create");
                        copied.Add(objectName);
                    }

                    RebindHomeSceneReferences(context.TargetScene);
                    Undo.CollapseUndoOperations(undoGroup);

                    report.copiedRootNames = copied.ToArray();
                    report.alreadyPresentRootNames = alreadyPresent.ToArray();
                    report.missingInSourceRootNames = missingInSource.ToArray();
                    report.verifiedRootNames = report.requiredRootNames
                        .Where(rootName => FindTargetTransform(context.TargetScene, rootName) != null)
                        .ToArray();

                    if (copied.Count > 0)
                    {
                        EditorSceneManager.MarkSceneDirty(context.TargetScene);
                    }

                    report.savedTargetScene = EditorSceneManager.SaveScene(context.TargetScene);
                    report.success = report.savedTargetScene &&
                                     report.missingInSourceRootNames.Length == 0 &&
                                     report.verifiedRootNames.Length == report.requiredRootNames.Length;
                    report.message = report.success
                        ? "Home 基础骨架已按只增不删口径补齐，可作为三场景统一 runtime core 的轻场景承接面。"
                        : "Home 基础骨架补齐未完全通过，请查看 artifact。";
                }
            }
            catch (Exception exception)
            {
                report.success = false;
                report.message = $"Home 基础骨架补齐异常：{exception.GetType().Name}: {exception.Message}";
            }

            report.artifactPath = WriteArtifact(report);
            Debug.Log($"[HomeFoundationBootstrap] {report.message}\nArtifact: {report.artifactPath}");
        }

        private static string WriteArtifact(HomeFoundationReport report)
        {
            string artifactDirectory = Path.Combine(Directory.GetCurrentDirectory(), ArtifactDirectory.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(artifactDirectory);

            string artifactPath = Path.Combine(artifactDirectory, $"home-foundation-{report.timestamp}.json");
            File.WriteAllText(artifactPath, JsonUtility.ToJson(report, true));
            return artifactPath;
        }

        private static GameObject FindRootGameObjectInScene(Scene scene, string rootName)
        {
            if (!scene.IsValid() || !scene.isLoaded || string.IsNullOrWhiteSpace(rootName))
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                GameObject candidate = roots[index];
                if (candidate != null && string.Equals(candidate.name, rootName, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static Transform FindSourceTransform(Scene scene, string objectName)
        {
            GameObject root = FindRootGameObjectInScene(scene, objectName);
            if (root != null)
            {
                return root.transform;
            }

            Transform sceneContainer = FindSceneContainer(scene, SourceSceneContainerName);
            return sceneContainer != null ? FindTransformByNameRecursive(sceneContainer, objectName) : null;
        }

        private static Transform FindTargetTransform(Scene scene, string objectName)
        {
            GameObject root = FindRootGameObjectInScene(scene, objectName);
            if (root != null)
            {
                return root.transform;
            }

            Transform sceneContainer = FindSceneContainer(scene, TargetSceneContainerName);
            return sceneContainer != null ? FindTransformByNameRecursive(sceneContainer, objectName) : null;
        }

        private static Transform ResolveTargetParent(Scene targetScene, Transform sourceTransform)
        {
            if (sourceTransform == null || sourceTransform.parent == null)
            {
                return null;
            }

            Transform sourceSceneContainer = FindSceneContainer(sourceTransform.gameObject.scene, SourceSceneContainerName);
            Transform targetSceneContainer = FindSceneContainer(targetScene, TargetSceneContainerName);
            if (sourceSceneContainer == null || targetSceneContainer == null)
            {
                return null;
            }

            string sourceContainerPath = GetTransformPath(sourceSceneContainer);
            string sourceParentPath = GetTransformPath(sourceTransform.parent);
            if (string.Equals(sourceParentPath, sourceContainerPath, StringComparison.Ordinal))
            {
                return targetSceneContainer;
            }

            if (sourceParentPath.StartsWith(sourceContainerPath + "/", StringComparison.Ordinal))
            {
                string relativeParentPath = sourceParentPath.Substring(sourceContainerPath.Length + 1);
                return EnsureTargetRelativePath(targetScene, targetSceneContainer, relativeParentPath);
            }

            return null;
        }

        private static Transform FindSceneContainer(Scene scene, string containerName)
        {
            GameObject root = FindRootGameObjectInScene(scene, containerName);
            return root != null ? root.transform : null;
        }

        private static Transform FindDirectChildByName(Transform parent, string childName)
        {
            if (parent == null || string.IsNullOrWhiteSpace(childName))
            {
                return null;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (child != null && string.Equals(child.name, childName, StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }

        private static Transform EnsureTargetRelativePath(Scene targetScene, Transform targetSceneContainer, string relativePath)
        {
            if (targetSceneContainer == null || string.IsNullOrWhiteSpace(relativePath))
            {
                return targetSceneContainer;
            }

            Transform current = targetSceneContainer;
            string[] segments = relativePath.Split('/');
            for (int index = 0; index < segments.Length; index++)
            {
                string segment = segments[index];
                Transform child = FindDirectChildByName(current, segment);
                if (child == null)
                {
                    GameObject created = new GameObject(segment);
                    SceneManager.MoveGameObjectToScene(created, targetScene);
                    created.transform.SetParent(current, false);
                    child = created.transform;
                }

                current = child;
            }

            return current;
        }

        private static void RebindHomeSceneReferences(Scene targetScene)
        {
            InventoryService inventory = FindFirstComponentInScene<InventoryService>(targetScene);
            EquipmentService equipment = FindFirstComponentInScene<EquipmentService>(targetScene);
            HotbarSelectionService hotbarSelection = FindFirstComponentInScene<HotbarSelectionService>(targetScene);
            InventorySortService sortService = FindFirstComponentInScene<InventorySortService>(targetScene);
            PackagePanelTabsUI packageTabs = FindFirstComponentInScene<PackagePanelTabsUI>(targetScene);
            WorldSpawnService worldSpawnService = FindFirstComponentInScene<WorldSpawnService>(targetScene);
            NavGrid2D navGrid = FindFirstComponentInScene<NavGrid2D>(targetScene);
            Camera worldCamera = FindMainCameraInScene(targetScene);
            ItemDatabase database = inventory != null ? inventory.Database : null;

            for (int index = 0; index < FindComponentsInScene<ToolbarUI>(targetScene).Count; index++)
            {
                ToolbarUI toolbar = FindComponentsInScene<ToolbarUI>(targetScene)[index];
                TrySetField(toolbar, "inventory", inventory);
                TrySetField(toolbar, "database", database);
                TrySetField(toolbar, "selection", hotbarSelection);
            }

            for (int index = 0; index < FindComponentsInScene<InventoryPanelUI>(targetScene).Count; index++)
            {
                InventoryPanelUI panel = FindComponentsInScene<InventoryPanelUI>(targetScene)[index];
                TrySetField(panel, "inventory", inventory);
                TrySetField(panel, "equipment", equipment);
                TrySetField(panel, "database", database);
                TrySetField(panel, "selection", hotbarSelection);
            }

            for (int index = 0; index < FindComponentsInScene<InventoryInteractionManager>(targetScene).Count; index++)
            {
                InventoryInteractionManager manager = FindComponentsInScene<InventoryInteractionManager>(targetScene)[index];
                TrySetField(manager, "inventory", inventory);
                TrySetField(manager, "equipment", equipment);
                TrySetField(manager, "database", database);
                TrySetField(manager, "sortService", sortService);
            }

            for (int index = 0; index < FindComponentsInScene<ItemTooltip>(targetScene).Count; index++)
            {
                ItemTooltip tooltip = FindComponentsInScene<ItemTooltip>(targetScene)[index];
                tooltip.SetDatabase(database);
                EditorUtility.SetDirty(tooltip);
            }

            for (int index = 0; index < FindComponentsInScene<GameInputManager>(targetScene).Count; index++)
            {
                GameInputManager inputManager = FindComponentsInScene<GameInputManager>(targetScene)[index];
                TrySetField(inputManager, "playerMovement", null);
                TrySetField(inputManager, "playerInteraction", null);
                TrySetField(inputManager, "playerToolController", null);
                TrySetField(inputManager, "autoNavigator", null);
                TrySetField(inputManager, "inventory", inventory);
                TrySetField(inputManager, "hotbarSelection", hotbarSelection);
                TrySetField(inputManager, "packageTabs", packageTabs);
                TrySetField(inputManager, "worldCamera", worldCamera);
            }

            for (int index = 0; index < FindComponentsInScene<WorldSpawnDebug>(targetScene).Count; index++)
            {
                WorldSpawnDebug debug = FindComponentsInScene<WorldSpawnDebug>(targetScene)[index];
                TrySetField(debug, "spawnService", worldSpawnService);
                TrySetField(debug, "inventory", inventory);
                TrySetField(debug, "database", database);
                TrySetField(debug, "hotbar", hotbarSelection);
                TrySetField(debug, "worldCamera", worldCamera);
            }

            for (int index = 0; index < FindComponentsInScene<TraversalBlockManager2D>(targetScene).Count; index++)
            {
                TraversalBlockManager2D traversal = FindComponentsInScene<TraversalBlockManager2D>(targetScene)[index];
                TrySetField(traversal, "navGrid", navGrid);
                TrySetField(traversal, "playerMovement", null);
            }

            for (int index = 0; index < FindComponentsInScene<HotbarSelectionService>(targetScene).Count; index++)
            {
                HotbarSelectionService service = FindComponentsInScene<HotbarSelectionService>(targetScene)[index];
                TrySetField(service, "playerToolController", null);
                TrySetField(service, "inventory", inventory);
            }

            Slider healthSlider = FindSliderByName(targetScene, "HP");
            for (int index = 0; index < FindComponentsInScene<HealthSystem>(targetScene).Count; index++)
            {
                HealthSystem healthSystem = FindComponentsInScene<HealthSystem>(targetScene)[index];
                TrySetField(healthSystem, "healthSlider", healthSlider);
            }

            Toggle sprintToggle = FindToggleByName(targetScene, "Shift");
            for (int index = 0; index < FindComponentsInScene<SprintStateManager>(targetScene).Count; index++)
            {
                SprintStateManager sprintStateManager = FindComponentsInScene<SprintStateManager>(targetScene)[index];
                TrySetField(sprintStateManager, "sprintToggleUI", sprintToggle);
            }

            for (int index = 0; index < FindComponentsInScene<OcclusionManager>(targetScene).Count; index++)
            {
                OcclusionManager occlusionManager = FindComponentsInScene<OcclusionManager>(targetScene)[index];
                TrySetField(occlusionManager, "player", null);
                TrySetField(occlusionManager, "playerSprite", null);
                TrySetField(occlusionManager, "playerCollider", null);
                TrySetField(occlusionManager, "playerSorting", null);
            }
        }

        private static List<T> FindComponentsInScene<T>(Scene scene) where T : Component
        {
            T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            List<T> results = new List<T>();
            for (int index = 0; index < components.Length; index++)
            {
                T component = components[index];
                if (component != null && component.gameObject.scene == scene)
                {
                    results.Add(component);
                }
            }

            return results;
        }

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            List<T> components = FindComponentsInScene<T>(scene);
            return components.Count > 0 ? components[0] : null;
        }

        private static Camera FindMainCameraInScene(Scene scene)
        {
            List<Camera> cameras = FindComponentsInScene<Camera>(scene);
            for (int index = 0; index < cameras.Count; index++)
            {
                Camera camera = cameras[index];
                if (camera != null && string.Equals(camera.gameObject.name, "Main Camera", StringComparison.Ordinal))
                {
                    return camera;
                }
            }

            return cameras.FirstOrDefault(camera => camera != null && camera.CompareTag("MainCamera"));
        }

        private static Slider FindSliderByName(Scene scene, string objectName)
        {
            Transform transform = FindTransformByName(scene, objectName);
            return transform != null ? transform.GetComponent<Slider>() : null;
        }

        private static Toggle FindToggleByName(Scene scene, string objectName)
        {
            Transform transform = FindTransformByName(scene, objectName);
            return transform != null ? transform.GetComponent<Toggle>() : null;
        }

        private static Transform FindTransformByName(Scene scene, string objectName)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                Transform match = FindTransformByNameRecursive(roots[index].transform, objectName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static Transform FindTransformByNameRecursive(Transform current, string objectName)
        {
            if (current == null)
            {
                return null;
            }

            if (string.Equals(current.name, objectName, StringComparison.Ordinal))
            {
                return current;
            }

            for (int index = 0; index < current.childCount; index++)
            {
                Transform match = FindTransformByNameRecursive(current.GetChild(index), objectName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            Stack<string> names = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names);
        }

        private static void TrySetField(object target, string fieldName, object value)
        {
            if (target == null || string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }

            FieldInfo fieldInfo = target.GetType().GetField(fieldName, InstanceBindingFlags);
            if (fieldInfo == null || !fieldInfo.FieldType.IsInstanceOfType(value) && value != null)
            {
                return;
            }

            fieldInfo.SetValue(target, value);
            if (target is UnityEngine.Object unityObject)
            {
                EditorUtility.SetDirty(unityObject);
            }
        }

        private sealed class LoadedSceneContext : IDisposable
        {
            public Scene SourceScene { get; private set; }
            public Scene TargetScene { get; private set; }
            public bool SourceWasAlreadyLoaded { get; private set; }
            public bool TargetWasAlreadyLoaded { get; private set; }

            private readonly List<Scene> openedByTool = new List<Scene>();

            public static bool TryOpen(string sourceScenePath, string targetScenePath, out LoadedSceneContext context, out string errorMessage)
            {
                context = new LoadedSceneContext();
                errorMessage = string.Empty;

                if (!TryGetOrOpenScene(sourceScenePath, context.openedByTool, out Scene sourceScene, out bool sourceWasAlreadyLoaded, out errorMessage))
                {
                    context.Dispose();
                    context = null;
                    return false;
                }

                if (!TryGetOrOpenScene(targetScenePath, context.openedByTool, out Scene targetScene, out bool targetWasAlreadyLoaded, out errorMessage))
                {
                    context.Dispose();
                    context = null;
                    return false;
                }

                context.SourceScene = sourceScene;
                context.TargetScene = targetScene;
                context.SourceWasAlreadyLoaded = sourceWasAlreadyLoaded;
                context.TargetWasAlreadyLoaded = targetWasAlreadyLoaded;
                return true;
            }

            public void Dispose()
            {
                for (int index = openedByTool.Count - 1; index >= 0; index--)
                {
                    Scene scene = openedByTool[index];
                    if (scene.IsValid() && scene.isLoaded)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }

                openedByTool.Clear();
            }

            private static bool TryGetOrOpenScene(string scenePath, List<Scene> openedByTool, out Scene scene, out bool wasAlreadyLoaded, out string errorMessage)
            {
                scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.IsValid() && scene.isLoaded)
                {
                    wasAlreadyLoaded = true;
                    errorMessage = string.Empty;
                    return true;
                }

                try
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    openedByTool.Add(scene);
                    wasAlreadyLoaded = false;
                    errorMessage = string.Empty;
                    return true;
                }
                catch (Exception exception)
                {
                    wasAlreadyLoaded = false;
                    errorMessage = $"打开场景失败：{scenePath}\n{exception.Message}";
                    return false;
                }
            }
        }
    }
}
