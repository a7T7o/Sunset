#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sunset.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class HomePrimaryDoorContractMenu
    {
        private const string SetupMenuPath = "Tools/Sunset/Scene/Setup Home <-> Primary Door Contract";
        private const string ProbeMenuPath = "Tools/Sunset/Scene/Run Home <-> Primary Door Contract Probe";
        private const string HomeScenePath = "Assets/000_Scenes/Home.unity";
        private const string PrimaryScenePath = "Assets/000_Scenes/Primary.unity";
        private const string PrimaryWorldRootName = "2_World";
        private const string HomeContractsRootName = "Home_Contracts";
        private const string HomeDoorName = "HomeDoor";
        private const string HomeEntryAnchorName = "HomeEntryAnchor";
        private const string PrimaryContractsRootName = "Primary_HomeContracts";
        private const string PrimaryDoorName = "PrimaryHomeDoor";
        private const string PrimaryEntryAnchorName = "PrimaryHomeEntryAnchor";

        private static readonly Vector2 DefaultDoorColliderSize = new Vector2(1.2f, 2.4f);
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ProbeResultPath = Path.Combine(CommandRoot, "home-primary-door-contract-probe.json");

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string firstBlocker = string.Empty;
            public string message = string.Empty;
            public SceneSideRecord home = new SceneSideRecord();
            public SceneSideRecord primary = new SceneSideRecord();
            public RuntimeAttentionRecord runtimeAttention = new RuntimeAttentionRecord();
            public List<string> blockingFindings = new List<string>();
            public List<string> attentionFindings = new List<string>();
        }

        [Serializable]
        private sealed class SceneSideRecord
        {
            public string scenePath = string.Empty;
            public string contractsRootPath = string.Empty;
            public string doorPath = string.Empty;
            public string entryAnchorPath = string.Empty;
            public string doorPosition = string.Empty;
            public string entryAnchorPosition = string.Empty;
            public bool contractsRootExists;
            public bool doorExists;
            public bool entryAnchorExists;
            public bool doorNestedUnderContracts;
            public bool entryAnchorNestedUnderDoor;
            public bool hasTriggerCollider;
            public bool colliderIsTrigger;
            public bool hasSceneTransitionTrigger;
            public string targetSceneName = string.Empty;
            public string targetScenePath = string.Empty;
            public bool triggerOnPlayerEnter;
        }

        [Serializable]
        private sealed class RuntimeAttentionRecord
        {
            public bool homeHasPlayerMovement;
            public bool homeHasGameInputManager;
            public bool homeHasNavigationRoot;
            public bool homeHasCinemachineCamera;
        }

        [MenuItem(SetupMenuPath)]
        private static void SetupHomePrimaryDoorContract()
        {
            if (!TryOpenScenesForModification(out Scene primaryScene, out Scene homeScene, out bool openedHomeTemporarily, out string failureReason))
            {
                Debug.LogError($"[HomePrimaryDoorContract] {failureReason}");
                return;
            }

            Scene previousActiveScene = SceneManager.GetActiveScene();

            try
            {
                Transform primaryWorldRoot = FindFirstTransformByName(primaryScene, PrimaryWorldRootName);
                if (primaryWorldRoot == null)
                {
                    Debug.LogError("[HomePrimaryDoorContract] Primary 当前缺少 2_World，无法挂载回家门合同。");
                    return;
                }

                Transform homeContractsRoot = EnsureRoot(homeScene, HomeContractsRootName);
                Transform homeDoor = EnsureChild(homeContractsRoot, HomeDoorName);
                Transform homeEntryAnchor = EnsureChild(homeDoor, HomeEntryAnchorName);

                Transform primaryContractsRoot = EnsureChild(primaryWorldRoot, PrimaryContractsRootName);
                Transform primaryDoor = EnsureChild(primaryContractsRoot, PrimaryDoorName);
                Transform primaryEntryAnchor = EnsureChild(primaryDoor, PrimaryEntryAnchorName);

                if (primaryContractsRoot.localPosition == Vector3.zero)
                {
                    primaryContractsRoot.position = ResolveSuggestedPrimaryDoorPosition(primaryScene);
                }

                EnsureDoorContract(homeDoor.gameObject, "Primary", PrimaryScenePath);
                EnsureDoorContract(primaryDoor.gameObject, "Home", HomeScenePath);

                MarkSceneDirty(homeScene);
                MarkSceneDirty(primaryScene);

                EditorSceneManager.SaveScene(homeScene);
                EditorSceneManager.SaveScene(primaryScene);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log(
                    "[HomePrimaryDoorContract] 已补齐 Home <-> Primary 双向门合同：HomeDoor 现在回 Primary，Primary_HomeContracts/PrimaryHomeDoor 现在回 Home。你现在只需要手动移动门位与入口锚点。");
            }
            finally
            {
                if (openedHomeTemporarily && homeScene.IsValid() && homeScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(homeScene, true);
                }

                if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActiveScene);
                }
            }
        }

        [MenuItem(ProbeMenuPath)]
        private static void RunProbe()
        {
            Directory.CreateDirectory(CommandRoot);

            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ProbeResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();

            Debug.Log($"[HomePrimaryDoorContractProbe] {result.message}");
        }

        [MenuItem(SetupMenuPath, true)]
        [MenuItem(ProbeMenuPath, true)]
        private static bool ValidateMenu()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static ProbeResult BuildProbeResult()
        {
            ProbeResult result = new ProbeResult
            {
                timestamp = DateTime.Now.ToString("O")
            };

            Scene homeScene = default;
            bool openedHomeTemporarily = false;

            try
            {
                Scene primaryScene = SceneManager.GetSceneByPath(PrimaryScenePath);
                if (!primaryScene.IsValid() || !primaryScene.isLoaded)
                {
                    AddBlocker(result, "primary-scene-not-open", "Primary 当前没有处于已加载状态，无法继续判定双向门合同。");
                    FinalizeProbeResult(result);
                    return result;
                }

                homeScene = SceneManager.GetSceneByPath(HomeScenePath);
                if (!homeScene.IsValid() || !homeScene.isLoaded)
                {
                    homeScene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Additive);
                    openedHomeTemporarily = true;
                }

                EvaluateDoorSide(
                    result,
                    homeScene,
                    HomeContractsRootName,
                    HomeDoorName,
                    HomeEntryAnchorName,
                    expectedTargetSceneName: "Primary",
                    expectedTargetScenePath: PrimaryScenePath,
                    result.home);

                EvaluateDoorSide(
                    result,
                    primaryScene,
                    PrimaryContractsRootName,
                    PrimaryDoorName,
                    PrimaryEntryAnchorName,
                    expectedTargetSceneName: "Home",
                    expectedTargetScenePath: HomeScenePath,
                    result.primary);

                EvaluateRuntimeAttention(result, homeScene);
                FinalizeProbeResult(result);
                return result;
            }
            finally
            {
                if (openedHomeTemporarily && homeScene.IsValid() && homeScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(homeScene, true);
                }
            }
        }

        private static void EvaluateDoorSide(
            ProbeResult result,
            Scene scene,
            string rootName,
            string doorName,
            string entryAnchorName,
            string expectedTargetSceneName,
            string expectedTargetScenePath,
            SceneSideRecord record)
        {
            record.scenePath = scene.path;

            Transform contractsRoot = FindFirstTransformByName(scene, rootName);
            Transform door = FindFirstTransformByName(scene, doorName);
            Transform entryAnchor = FindFirstTransformByName(scene, entryAnchorName);

            record.contractsRootExists = contractsRoot != null;
            record.doorExists = door != null;
            record.entryAnchorExists = entryAnchor != null;

            if (contractsRoot == null)
            {
                AddBlocker(result, $"{rootName}-missing", $"{Path.GetFileNameWithoutExtension(scene.path)} 缺少 {rootName}。");
                return;
            }

            record.contractsRootPath = GetTransformPath(contractsRoot);

            if (door == null)
            {
                AddBlocker(result, $"{doorName}-missing", $"{Path.GetFileNameWithoutExtension(scene.path)} 缺少 {doorName}。");
                return;
            }

            record.doorPath = GetTransformPath(door);
            record.doorPosition = FormatVector(door.position);
            record.doorNestedUnderContracts = door.parent == contractsRoot;
            if (!record.doorNestedUnderContracts)
            {
                AddBlocker(result, $"{doorName}-parent-mismatch", $"{doorName} 当前不在 {rootName} 下。");
            }

            if (entryAnchor == null)
            {
                AddBlocker(result, $"{entryAnchorName}-missing", $"{Path.GetFileNameWithoutExtension(scene.path)} 缺少 {entryAnchorName}。");
                return;
            }

            record.entryAnchorPath = GetTransformPath(entryAnchor);
            record.entryAnchorPosition = FormatVector(entryAnchor.position);
            record.entryAnchorNestedUnderDoor = entryAnchor.parent == door;
            if (!record.entryAnchorNestedUnderDoor)
            {
                AddBlocker(result, $"{entryAnchorName}-parent-mismatch", $"{entryAnchorName} 当前不在 {doorName} 下。");
            }

            Collider2D collider = door.GetComponent<Collider2D>();
            record.hasTriggerCollider = collider != null;
            record.colliderIsTrigger = collider != null && collider.isTrigger;

            if (!record.hasTriggerCollider)
            {
                AddBlocker(result, $"{doorName}-collider-missing", $"{doorName} 当前缺少 Collider2D。");
            }
            else if (!record.colliderIsTrigger)
            {
                AddBlocker(result, $"{doorName}-collider-not-trigger", $"{doorName} 的 Collider2D 当前不是 trigger。");
            }

            SceneTransitionTrigger2D transition = door.GetComponent<SceneTransitionTrigger2D>();
            record.hasSceneTransitionTrigger = transition != null;
            if (transition == null)
            {
                AddBlocker(result, $"{doorName}-scene-transition-missing", $"{doorName} 当前还没有 SceneTransitionTrigger2D。");
                return;
            }

            record.targetSceneName = transition.TargetSceneName;
            record.targetScenePath = transition.TargetScenePath;
            record.triggerOnPlayerEnter = ReadSerializedBool(transition, "triggerOnPlayerEnter");

            if (!string.Equals(record.targetSceneName, expectedTargetSceneName, StringComparison.Ordinal))
            {
                AddBlocker(
                    result,
                    $"{doorName}-target-scene-name-mismatch",
                    $"{doorName} 当前目标场景名是 {record.targetSceneName}，不是期望的 {expectedTargetSceneName}。");
            }

            if (!string.Equals(record.targetScenePath, expectedTargetScenePath, StringComparison.Ordinal))
            {
                AddBlocker(
                    result,
                    $"{doorName}-target-scene-path-mismatch",
                    $"{doorName} 当前目标场景路径是 {record.targetScenePath}，不是期望的 {expectedTargetScenePath}。");
            }

            if (!record.triggerOnPlayerEnter)
            {
                result.attentionFindings.Add($"{doorName} 当前不是玩家进入即触发；如果你想做纯场景门，这里后续最好改回自动触发。");
            }
        }

        private static void EvaluateRuntimeAttention(ProbeResult result, Scene homeScene)
        {
            result.runtimeAttention.homeHasPlayerMovement = FindFirstComponentInScene<global::PlayerMovement>(homeScene) != null;
            result.runtimeAttention.homeHasGameInputManager = FindFirstComponentInScene<global::GameInputManager>(homeScene) != null;
            result.runtimeAttention.homeHasNavigationRoot = FindFirstTransformByName(homeScene, "NavigationRoot") != null;
            result.runtimeAttention.homeHasCinemachineCamera = FindFirstTransformByName(homeScene, "CinemachineCamera") != null;

            if (!result.runtimeAttention.homeHasPlayerMovement)
            {
                result.attentionFindings.Add("Home 当前还没有 scene-local PlayerMovement；如果不是走跨场景保留玩家，这里还不是完整自由进出的最终 runtime 闭环。");
            }

            if (!result.runtimeAttention.homeHasGameInputManager)
            {
                result.attentionFindings.Add("Home 当前还没有 scene-local GameInputManager；如果后续要像 Town 一样完全自由行走，这条输入链还需要补。");
            }

            if (!result.runtimeAttention.homeHasNavigationRoot)
            {
                result.attentionFindings.Add("Home 当前没有 NavigationRoot；小屋内或许能先靠无边界移动过渡，但它还不是 Town 等级的完整导航场景。");
            }

            if (!result.runtimeAttention.homeHasCinemachineCamera)
            {
                result.attentionFindings.Add("Home 当前没有 CinemachineCamera；如果屋内最终需要跟随镜头，这里还要继续补。");
            }
        }

        private static void EnsureDoorContract(GameObject doorObject, string targetSceneName, string targetScenePath)
        {
            BoxCollider2D collider = doorObject.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = Undo.AddComponent<BoxCollider2D>(doorObject);
            }

            collider.isTrigger = true;
            if (collider.size.sqrMagnitude < 0.0001f)
            {
                collider.size = DefaultDoorColliderSize;
            }

            SceneTransitionTrigger2D trigger = doorObject.GetComponent<SceneTransitionTrigger2D>();
            if (trigger == null)
            {
                trigger = Undo.AddComponent<SceneTransitionTrigger2D>(doorObject);
            }

            trigger.SetTargetScene(targetSceneName, targetScenePath);
            AssignSceneAsset(trigger, targetScenePath);
            EditorUtility.SetDirty(doorObject);
            EditorUtility.SetDirty(trigger);
            EditorUtility.SetDirty(collider);
        }

        private static void AssignSceneAsset(SceneTransitionTrigger2D trigger, string targetScenePath)
        {
            if (trigger == null || string.IsNullOrWhiteSpace(targetScenePath))
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(trigger);
            SerializedProperty sceneAssetProperty = serializedObject.FindProperty("targetSceneAsset");
            if (sceneAssetProperty == null)
            {
                return;
            }

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetScenePath);
            sceneAssetProperty.objectReferenceValue = sceneAsset;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Vector3 ResolveSuggestedPrimaryDoorPosition(Scene primaryScene)
        {
            Transform existingTransition = FindFirstTransformByName(primaryScene, "SceneTransitionTrigger");
            if (existingTransition != null)
            {
                return existingTransition.position + new Vector3(1.5f, 0f, 0f);
            }

            Transform player = FindFirstTransformByName(primaryScene, "Player");
            if (player != null)
            {
                return player.position;
            }

            return Vector3.zero;
        }

        private static bool TryOpenScenesForModification(
            out Scene primaryScene,
            out Scene homeScene,
            out bool openedHomeTemporarily,
            out string failureReason)
        {
            primaryScene = SceneManager.GetSceneByPath(PrimaryScenePath);
            homeScene = SceneManager.GetSceneByPath(HomeScenePath);
            openedHomeTemporarily = false;
            failureReason = string.Empty;

            if (!primaryScene.IsValid() || !primaryScene.isLoaded)
            {
                failureReason = "Primary 当前没有处于已加载状态；请先打开 Primary 再执行双向门合同。";
                return false;
            }

            if (!homeScene.IsValid() || !homeScene.isLoaded)
            {
                homeScene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Additive);
                openedHomeTemporarily = true;
            }

            if (!homeScene.IsValid() || !homeScene.isLoaded)
            {
                failureReason = "Home 当前无法以附加方式打开，不能继续补双向门合同。";
                return false;
            }

            return true;
        }

        private static Transform EnsureRoot(Scene scene, string objectName)
        {
            Transform existing = FindFirstTransformByName(scene, objectName);
            if (existing != null)
            {
                return existing;
            }

            GameObject created = new GameObject(objectName);
            SceneManager.MoveGameObjectToScene(created, scene);
            return created.transform;
        }

        private static Transform EnsureChild(Transform parent, string objectName)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Transform existing = parent.Find(objectName);
            if (existing != null)
            {
                return existing;
            }

            GameObject created = new GameObject(objectName);
            Transform createdTransform = created.transform;
            createdTransform.SetParent(parent, false);
            return createdTransform;
        }

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                T found = roots[index].GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static Transform FindFirstTransformByName(Scene scene, string objectName)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                Transform found = FindChildRecursive(roots[index].transform, objectName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static Transform FindChildRecursive(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (string.Equals(root.name, objectName, StringComparison.Ordinal))
            {
                return root;
            }

            for (int index = 0; index < root.childCount; index++)
            {
                Transform found = FindChildRecursive(root.GetChild(index), objectName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static void MarkSceneDirty(Scene scene)
        {
            if (scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        private static bool ReadSerializedBool(UnityEngine.Object target, string propertyName)
        {
            if (target == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            return property != null && property.propertyType == SerializedPropertyType.Boolean && property.boolValue;
        }

        private static string GetTransformPath(Transform target)
        {
            if (target == null)
            {
                return string.Empty;
            }

            Stack<string> segments = new Stack<string>();
            Transform current = target;
            while (current != null)
            {
                segments.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", segments.ToArray());
        }

        private static string FormatVector(Vector3 value)
        {
            return $"({value.x:F3}, {value.y:F3}, {value.z:F3})";
        }

        private static void AddBlocker(ProbeResult result, string blockerCode, string message)
        {
            if (result == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(result.firstBlocker))
            {
                result.firstBlocker = blockerCode;
            }

            result.blockingFindings.Add(message);
        }

        private static void FinalizeProbeResult(ProbeResult result)
        {
            if (result == null)
            {
                return;
            }

            if (result.blockingFindings.Count > 0)
            {
                result.status = "blocked";
                result.success = false;
                result.message = result.blockingFindings[0];
                return;
            }

            if (result.attentionFindings.Count > 0)
            {
                result.status = "attention";
                result.success = true;
                result.message = "Home <-> Primary 双向门合同已成立，但 Home 仍有 runtime attention 需要后续补齐。";
                return;
            }

            result.status = "ok";
            result.success = true;
            result.message = "Home <-> Primary 双向门合同已成立，当前结构侧没有发现新的 blocker。";
        }
    }
}
#endif
