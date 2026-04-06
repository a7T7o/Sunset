#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class HomeSceneRestContractMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Run Home Rest Contract Probe";
        private const string HomeScenePath = "Assets/000_Scenes/Home.unity";
        private const float DefaultAnchorOriginTolerance = 0.05f;
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "home-rest-contract-probe.json");

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string firstBlocker = string.Empty;
            public string message = string.Empty;
            public string scenePath = HomeScenePath;
            public CameraRecord camera = new CameraRecord();
            public ManagerRecord managers = new ManagerRecord();
            public ContractRecord contract = new ContractRecord();
            public RestRecord rest = new RestRecord();
            public DoorRecord door = new DoorRecord();
            public List<string> blockingFindings = new List<string>();
            public List<string> attentionFindings = new List<string>();
        }

        [Serializable]
        private sealed class CameraRecord
        {
            public string mainCameraPath = string.Empty;
            public bool hasMainCameraTag;
            public bool hasAudioListener;
            public bool isOrthographic;
            public float orthographicSize;
            public string viewCenter = string.Empty;
            public string viewHalfExtents = string.Empty;
        }

        [Serializable]
        private sealed class ManagerRecord
        {
            public string persistentManagersPath = string.Empty;
            public bool hasPersistentManagers;
            public bool prefabDatabaseAssigned;
        }

        [Serializable]
        private sealed class ContractRecord
        {
            public string contractsPath = string.Empty;
            public string contractsPosition = string.Empty;
            public bool contractsMovedFromOrigin;
            public bool contractsInsideInitialView;
            public string doorPath = string.Empty;
            public string doorPosition = string.Empty;
            public bool doorInsideInitialView;
            public bool doorNestedUnderContracts;
            public string entryAnchorPath = string.Empty;
            public string entryAnchorPosition = string.Empty;
            public bool entryAnchorInsideInitialView;
            public bool entryAnchorNestedUnderDoor;
        }

        [Serializable]
        private sealed class RestRecord
        {
            public string bedPath = string.Empty;
            public string bedPosition = string.Empty;
            public bool bedInsideInitialView;
            public bool bedTaggedInteractable;
            public bool hasCollider2D;
            public bool colliderIsTrigger;
            public bool hasSpringDay1BedInteractable;
        }

        [Serializable]
        private sealed class DoorRecord
        {
            public string exitComponentType = string.Empty;
            public bool hasExitComponent;
            public bool hasTriggerCollider;
            public bool colliderIsTrigger;
            public string targetSceneName = string.Empty;
            public string targetScenePath = string.Empty;
            public bool triggerOnPlayerEnter;
            public bool requireKeyPress;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[HomeRestContractProbe] {result.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static ProbeResult BuildProbeResult()
        {
            Scene loadedHomeScene = default;
            bool loadedTemporarily = false;
            try
            {
                loadedHomeScene = SceneManager.GetSceneByPath(HomeScenePath);
                if (!loadedHomeScene.IsValid() || !loadedHomeScene.isLoaded)
                {
                    loadedHomeScene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Additive);
                    loadedTemporarily = true;
                }

                ProbeResult result = new ProbeResult
                {
                    timestamp = DateTime.Now.ToString("O")
                };

                if (!loadedHomeScene.IsValid() || !loadedHomeScene.isLoaded)
                {
                    AddBlocker(result, "home-scene-unavailable", "Home 场景当前无法只读加载，不能继续判定住处 contract。");
                    FinalizeResult(result);
                    return result;
                }

                EvaluateCamera(loadedHomeScene, result);
                EvaluateManagers(loadedHomeScene, result);
                EvaluateContracts(loadedHomeScene, result);
                EvaluateRestTarget(loadedHomeScene, result);
                EvaluateDoorExit(loadedHomeScene, result);
                FinalizeResult(result);
                return result;
            }
            finally
            {
                if (loadedTemporarily && loadedHomeScene.IsValid() && loadedHomeScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(loadedHomeScene, true);
                }
            }
        }

        private static void EvaluateCamera(Scene homeScene, ProbeResult result)
        {
            Camera mainCamera = FindMainCamera(homeScene);
            if (mainCamera == null)
            {
                AddBlocker(result, "main-camera-missing", "Home 缺少 Main Camera，当前无法继续判定屋内住处视野。");
                return;
            }

            result.camera.mainCameraPath = GetTransformPath(mainCamera.transform);
            result.camera.hasMainCameraTag = mainCamera.CompareTag("MainCamera");
            result.camera.hasAudioListener = mainCamera.GetComponent<AudioListener>() != null;
            result.camera.isOrthographic = mainCamera.orthographic;
            result.camera.orthographicSize = mainCamera.orthographic ? mainCamera.orthographicSize : 0f;
            result.camera.viewCenter = FormatVector(mainCamera.transform.position);
            Vector2 halfExtents = ResolveViewHalfExtents(mainCamera);
            result.camera.viewHalfExtents = $"{halfExtents.x:F3},{halfExtents.y:F3}";

            if (!result.camera.hasMainCameraTag)
            {
                AddBlocker(result, "main-camera-tag-mismatch", "Home 的 Main Camera 没有保持 MainCamera tag。");
            }

            if (!result.camera.hasAudioListener)
            {
                AddBlocker(result, "main-camera-audio-listener-missing", "Home 的 Main Camera 缺少 AudioListener。");
            }

            if (!result.camera.isOrthographic)
            {
                result.attentionFindings.Add("Home 当前不是正交相机；住处 probe 仍能运行，但视野判定会变得更脆弱。");
            }
        }

        private static void EvaluateManagers(Scene homeScene, ProbeResult result)
        {
            GameObject persistentManagers = FindFirstByName(homeScene, "PersistentManagers");
            if (persistentManagers == null)
            {
                AddBlocker(result, "persistent-managers-missing", "Home 缺少 PersistentManagers，运行时全局管理链没有住处落点。");
                return;
            }

            result.managers.hasPersistentManagers = true;
            result.managers.persistentManagersPath = GetTransformPath(persistentManagers.transform);

            MonoBehaviour managerComponent = FindNamedMonoBehaviour(persistentManagers, "PersistentManagers");
            if (managerComponent == null)
            {
                result.attentionFindings.Add("Home 找到了 PersistentManagers 节点，但当前没有读到 PersistentManagers 组件。");
                return;
            }

            SerializedProperty prefabDatabaseProperty = new SerializedObject(managerComponent).FindProperty("prefabDatabase");
            result.managers.prefabDatabaseAssigned =
                prefabDatabaseProperty != null &&
                prefabDatabaseProperty.propertyType == SerializedPropertyType.ObjectReference &&
                prefabDatabaseProperty.objectReferenceValue != null;

            if (!result.managers.prefabDatabaseAssigned)
            {
                result.attentionFindings.Add("Home 的 PersistentManagers 仍未显式配置 PrefabDatabase，运行时仍可能沿用那条已知的初始化 warning。");
            }
        }

        private static void EvaluateContracts(Scene homeScene, ProbeResult result)
        {
            Transform contracts = FindFirstTransformByName(homeScene, "Home_Contracts");
            Transform door = FindFirstTransformByName(homeScene, "HomeDoor");
            Transform entryAnchor = FindFirstTransformByName(homeScene, "HomeEntryAnchor");

            if (contracts == null)
            {
                AddBlocker(result, "home-contracts-missing", "Home 缺少 Home_Contracts，住处合同层没有根节点。");
                return;
            }

            result.contract.contractsPath = GetTransformPath(contracts);
            result.contract.contractsPosition = FormatVector(contracts.position);
            result.contract.contractsMovedFromOrigin = !IsNearOrigin(contracts.position);
            result.contract.contractsInsideInitialView = IsInsideInitialView(result.camera, contracts.position);

            if (!result.contract.contractsMovedFromOrigin)
            {
                result.attentionFindings.Add("Home_Contracts 仍停在默认原点；如果你本来想把门口摆到别的位置，这通常意味着现场还没真正摆开。");
            }

            if (door == null)
            {
                AddBlocker(result, "home-door-missing", "Home 缺少 HomeDoor，当前没有住处代理门位。");
                return;
            }

            result.contract.doorPath = GetTransformPath(door);
            result.contract.doorPosition = FormatVector(door.position);
            result.contract.doorInsideInitialView = IsInsideInitialView(result.camera, door.position);
            result.contract.doorNestedUnderContracts = door.parent == contracts;

            if (!result.contract.doorNestedUnderContracts)
            {
                AddBlocker(result, "home-door-parent-mismatch", "HomeDoor 当前不在 Home_Contracts 下，住处合同层层级已经漂了。");
            }

            if (entryAnchor == null)
            {
                AddBlocker(result, "home-entry-anchor-missing", "Home 缺少 HomeEntryAnchor，屋内入口落点还没有 scene-side 语义位。");
                return;
            }

            result.contract.entryAnchorPath = GetTransformPath(entryAnchor);
            result.contract.entryAnchorPosition = FormatVector(entryAnchor.position);
            result.contract.entryAnchorInsideInitialView = IsInsideInitialView(result.camera, entryAnchor.position);
            result.contract.entryAnchorNestedUnderDoor = entryAnchor.parent == door;

            if (!result.contract.entryAnchorNestedUnderDoor)
            {
                AddBlocker(result, "home-entry-anchor-parent-mismatch", "HomeEntryAnchor 当前不在 HomeDoor 下，入口合同层不再是单一闭环。");
            }

            if (!result.contract.contractsInsideInitialView)
            {
                result.attentionFindings.Add("Home_Contracts 当前不在主相机初始视野里，后续门位/入口位验收会比较别扭。");
            }

            if (!result.contract.doorInsideInitialView)
            {
                result.attentionFindings.Add("HomeDoor 当前不在主相机初始视野里，屋内门位的第一眼反馈会比较弱。");
            }

            if (!result.contract.entryAnchorInsideInitialView)
            {
                result.attentionFindings.Add("HomeEntryAnchor 当前不在主相机初始视野里，玩家进屋第一步的 scene-side 读感会偏弱。");
            }
        }

        private static void EvaluateRestTarget(Scene homeScene, ProbeResult result)
        {
            Transform homeBed = FindFirstTransformByName(homeScene, "HomeBed");
            if (homeBed == null)
            {
                AddBlocker(result, "home-bed-missing", "Home 缺少 HomeBed，住处的首选休息承载位还不存在。");
                return;
            }

            result.rest.bedPath = GetTransformPath(homeBed);
            result.rest.bedPosition = FormatVector(homeBed.position);
            result.rest.bedInsideInitialView = IsInsideInitialView(result.camera, homeBed.position);
            result.rest.bedTaggedInteractable = string.Equals(homeBed.tag, "Interactable", StringComparison.Ordinal);

            Collider2D collider = homeBed.GetComponent<Collider2D>();
            result.rest.hasCollider2D = collider != null;
            result.rest.colliderIsTrigger = collider != null && collider.isTrigger;
            result.rest.hasSpringDay1BedInteractable = FindNamedMonoBehaviour(homeBed.gameObject, "SpringDay1BedInteractable") != null;

            if (!result.rest.hasCollider2D)
            {
                AddBlocker(result, "home-bed-collider-missing", "HomeBed 缺少 Collider2D，导演链没法把它当成真正可交互住处位。");
                return;
            }

            if (!result.rest.colliderIsTrigger)
            {
                AddBlocker(result, "home-bed-collider-not-trigger", "HomeBed 的 Collider2D 当前不是 trigger，住处交互包络线不会按预期成立。");
            }

            if (!result.rest.bedTaggedInteractable)
            {
                result.attentionFindings.Add("HomeBed 当前没有保持 Interactable tag；虽然导演链仍可 runtime 补口，但 scene-side 自解释性会差一层。");
            }

            if (!result.rest.hasSpringDay1BedInteractable)
            {
                result.attentionFindings.Add("HomeBed 当前还没有显式挂 SpringDay1BedInteractable，依旧主要依赖 day1 运行时自动补口。");
            }

            if (!result.rest.bedInsideInitialView)
            {
                result.attentionFindings.Add("HomeBed 当前不在主相机初始视野里，进屋后第一眼看不到床位。");
            }
        }

        private static void EvaluateDoorExit(Scene homeScene, ProbeResult result)
        {
            Transform homeDoor = FindFirstTransformByName(homeScene, "HomeDoor");
            if (homeDoor == null)
            {
                return;
            }

            MonoBehaviour sceneTransition = FindNamedMonoBehaviour(homeDoor.gameObject, "SceneTransitionTrigger2D");
            MonoBehaviour doorTrigger = FindNamedMonoBehaviour(homeDoor.gameObject, "DoorTrigger");

            if (sceneTransition != null)
            {
                Collider2D collider = homeDoor.GetComponent<Collider2D>();
                result.door.exitComponentType = "SceneTransitionTrigger2D";
                result.door.hasExitComponent = true;
                result.door.hasTriggerCollider = collider != null;
                result.door.colliderIsTrigger = collider != null && collider.isTrigger;
                result.door.targetSceneName = ReadSerializedString(sceneTransition, "targetSceneName");
                result.door.targetScenePath = ReadSerializedString(sceneTransition, "targetScenePath");
                result.door.triggerOnPlayerEnter = ReadSerializedBool(sceneTransition, "triggerOnPlayerEnter");

                if (!result.door.hasTriggerCollider)
                {
                    result.attentionFindings.Add("HomeDoor 已挂 SceneTransitionTrigger2D，但当前还没有 Collider2D。");
                }
                else if (!result.door.colliderIsTrigger)
                {
                    result.attentionFindings.Add("HomeDoor 的切场触发器当前不是 trigger，运行时仍会依赖脚本自愈。");
                }

                if (string.IsNullOrWhiteSpace(result.door.targetSceneName) && string.IsNullOrWhiteSpace(result.door.targetScenePath))
                {
                    result.attentionFindings.Add("HomeDoor 已挂 SceneTransitionTrigger2D，但目标场景还没有填。");
                }

                if (!result.door.triggerOnPlayerEnter)
                {
                    result.attentionFindings.Add("HomeDoor 的 SceneTransitionTrigger2D 当前不是玩家进入即触发。");
                }

                return;
            }

            if (doorTrigger != null)
            {
                Collider2D collider = homeDoor.GetComponent<Collider2D>();
                result.door.exitComponentType = "DoorTrigger";
                result.door.hasExitComponent = true;
                result.door.hasTriggerCollider = collider != null;
                result.door.colliderIsTrigger = collider != null && collider.isTrigger;
                result.door.targetSceneName = ReadSerializedString(doorTrigger, "targetSceneName");
                result.door.requireKeyPress = ReadSerializedBool(doorTrigger, "requireKeyPress");

                if (!result.door.hasTriggerCollider)
                {
                    result.attentionFindings.Add("HomeDoor 已挂 DoorTrigger，但当前还没有 Collider2D。");
                }

                if (string.IsNullOrWhiteSpace(result.door.targetSceneName))
                {
                    result.attentionFindings.Add("HomeDoor 已挂 DoorTrigger，但目标场景仍未填写。");
                }

                return;
            }

            result.attentionFindings.Add("HomeDoor 当前还没有显式的 scene exit 组件；Home 已经是可消费住处，但还不是完整可自由进出的室内场景。");
        }

        private static void FinalizeResult(ProbeResult result)
        {
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
                result.firstBlocker = string.Empty;
                result.message = "Home 的住处 rest contract 已可用，但仍有 attention 需要后续决定。";
                return;
            }

            result.status = "completed";
            result.success = true;
            result.firstBlocker = string.Empty;
            result.message = "Home 的住处 camera/bed/door/entry scene-side contract 已站住，可继续作为 day1 的屋内住处承接层使用。";
        }

        private static void AddBlocker(ProbeResult result, string blockerCode, string message)
        {
            if (string.IsNullOrWhiteSpace(result.firstBlocker))
            {
                result.firstBlocker = blockerCode;
            }

            result.blockingFindings.Add(message);
        }

        private static Camera FindMainCamera(Scene scene)
        {
            List<Camera> cameras = FindComponentsInScene<Camera>(scene);
            Camera best = null;
            int bestScore = int.MinValue;
            for (int index = 0; index < cameras.Count; index++)
            {
                Camera candidate = cameras[index];
                if (candidate == null || candidate.targetTexture != null)
                {
                    continue;
                }

                int score = 0;
                if (candidate.CompareTag("MainCamera"))
                {
                    score += 1000;
                }

                if (string.Equals(candidate.name, "Main Camera", StringComparison.Ordinal))
                {
                    score += 200;
                }

                score += Mathf.RoundToInt(candidate.depth);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            return best;
        }

        private static GameObject FindFirstByName(Scene scene, string objectName)
        {
            Transform transform = FindFirstTransformByName(scene, objectName);
            return transform != null ? transform.gameObject : null;
        }

        private static Transform FindFirstTransformByName(Scene scene, string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return null;
            }

            Dictionary<string, Transform> lookup = BuildTransformLookup(scene);
            lookup.TryGetValue(objectName, out Transform transform);
            return transform;
        }

        private static Dictionary<string, Transform> BuildTransformLookup(Scene scene)
        {
            Dictionary<string, Transform> lookup = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return lookup;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
            {
                CollectTransforms(roots[rootIndex].transform, lookup);
            }

            return lookup;
        }

        private static void CollectTransforms(Transform current, Dictionary<string, Transform> lookup)
        {
            if (current == null)
            {
                return;
            }

            if (!lookup.ContainsKey(current.name))
            {
                lookup[current.name] = current;
            }

            for (int index = 0; index < current.childCount; index++)
            {
                CollectTransforms(current.GetChild(index), lookup);
            }
        }

        private static List<T> FindComponentsInScene<T>(Scene scene) where T : Component
        {
            List<T> components = new List<T>();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return components;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
            {
                components.AddRange(roots[rootIndex].GetComponentsInChildren<T>(true));
            }

            return components;
        }

        private static MonoBehaviour FindNamedMonoBehaviour(GameObject target, string typeName)
        {
            if (target == null || string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            MonoBehaviour[] behaviours = target.GetComponents<MonoBehaviour>();
            for (int index = 0; index < behaviours.Length; index++)
            {
                MonoBehaviour behaviour = behaviours[index];
                if (behaviour == null)
                {
                    continue;
                }

                Type behaviourType = behaviour.GetType();
                if (string.Equals(behaviourType.Name, typeName, StringComparison.Ordinal) ||
                    string.Equals(behaviourType.FullName, typeName, StringComparison.Ordinal))
                {
                    return behaviour;
                }
            }

            return null;
        }

        private static string ReadSerializedString(UnityEngine.Object target, string propertyName)
        {
            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            return property != null ? property.stringValue : string.Empty;
        }

        private static bool ReadSerializedBool(UnityEngine.Object target, string propertyName)
        {
            SerializedProperty property = new SerializedObject(target).FindProperty(propertyName);
            return property != null && property.boolValue;
        }

        private static Vector2 ResolveViewHalfExtents(Camera mainCamera)
        {
            float aspect = mainCamera != null && mainCamera.aspect > 0.01f ? mainCamera.aspect : 16f / 9f;
            float orthographicSize = mainCamera != null && mainCamera.orthographic ? mainCamera.orthographicSize : 5f;
            return new Vector2(orthographicSize * aspect, orthographicSize);
        }

        private static bool IsInsideInitialView(CameraRecord record, Vector3 worldPosition)
        {
            if (string.IsNullOrWhiteSpace(record.viewCenter) || string.IsNullOrWhiteSpace(record.viewHalfExtents))
            {
                return false;
            }

            Vector2 halfExtents = ParseVector2(record.viewHalfExtents);
            Vector3 center = ParseVector3(record.viewCenter);
            return Mathf.Abs(worldPosition.x - center.x) <= halfExtents.x &&
                   Mathf.Abs(worldPosition.y - center.y) <= halfExtents.y;
        }

        private static bool IsNearOrigin(Vector3 position)
        {
            return Mathf.Abs(position.x) <= DefaultAnchorOriginTolerance &&
                   Mathf.Abs(position.y) <= DefaultAnchorOriginTolerance &&
                   Mathf.Abs(position.z) <= DefaultAnchorOriginTolerance;
        }

        private static Vector2 ParseVector2(string value)
        {
            string[] parts = value.Split(',');
            if (parts.Length < 2)
            {
                return Vector2.zero;
            }

            return new Vector2(ParseFloat(parts[0]), ParseFloat(parts[1]));
        }

        private static Vector3 ParseVector3(string value)
        {
            string[] parts = value.Split(',');
            if (parts.Length < 3)
            {
                return Vector3.zero;
            }

            return new Vector3(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]));
        }

        private static float ParseFloat(string value)
        {
            return float.TryParse(value, out float parsed) ? parsed : 0f;
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

        private static string FormatVector(Vector3 value)
        {
            return $"{value.x:F3},{value.y:F3},{value.z:F3}";
        }
    }
}
#endif
