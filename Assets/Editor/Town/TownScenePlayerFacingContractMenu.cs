#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sunset.Service.Camera;
using Sunset.Story;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class TownScenePlayerFacingContractMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Run Town Player-Facing Contract Probe";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";
        private const float BlockedReturnTriggerEdgeDistance = 0.05f;
        private const float AttentionReturnTriggerEdgeDistance = 1.0f;
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "town-player-facing-contract-probe.json");
        private static readonly string[] CriticalEntryAnchors = Array.Empty<string>();
        private static readonly string[] RuntimeAnchors = Array.Empty<string>();

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string firstBlocker = string.Empty;
            public string message = string.Empty;
            public string scenePath = TownScenePath;
            public CameraFlowRecord camera = new CameraFlowRecord();
            public PlayerFlowRecord player = new PlayerFlowRecord();
            public TransitionFlowRecord transition = new TransitionFlowRecord();
            public BoundsFlowRecord bounds = new BoundsFlowRecord();
            public List<AnchorFlowRecord> anchors = new List<AnchorFlowRecord>();
            public List<string> blockingFindings = new List<string>();
            public List<string> attentionFindings = new List<string>();
        }

        [Serializable]
        private sealed class CameraFlowRecord
        {
            public string mainCameraPath = string.Empty;
            public string cinemachineCameraPath = string.Empty;
            public string trackingTargetPath = string.Empty;
            public bool trackingTargetMatchesPlayer;
            public string virtualCameraPosition = string.Empty;
            public bool virtualCameraXYMatchesPlayer;
            public string initialViewCenter = string.Empty;
            public string initialViewHalfExtents = string.Empty;
        }

        [Serializable]
        private sealed class PlayerFlowRecord
        {
            public string playerPath = string.Empty;
            public string playerPosition = string.Empty;
            public bool hasCollider2D;
            public bool insideCameraBounds;
            public bool insideInitialView;
            public float distanceToLeftBound;
            public float distanceToRightBound;
            public float distanceToBottomBound;
            public float distanceToTopBound;
        }

        [Serializable]
        private sealed class TransitionFlowRecord
        {
            public string triggerPath = string.Empty;
            public string triggerPosition = string.Empty;
            public bool insideCameraBounds;
            public bool insideInitialView;
            public bool playerOverlapsReturnTrigger;
            public float returnTriggerEdgeDistance;
        }

        [Serializable]
        private sealed class BoundsFlowRecord
        {
            public string colliderPath = string.Empty;
            public string worldMin = string.Empty;
            public string worldMax = string.Empty;
            public bool containsPlayer;
            public bool containsReturnTrigger;
        }

        [Serializable]
        private sealed class AnchorFlowRecord
        {
            public string anchorName = string.Empty;
            public string anchorPath = string.Empty;
            public string worldPosition = string.Empty;
            public bool exists;
            public bool insideCameraBounds;
            public bool insideInitialView;
            public float distanceToPlayer;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[TownPlayerFacingProbe] {result.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static ProbeResult BuildProbeResult()
        {
            Scene loadedTownScene = default;
            bool loadedTemporarily = false;
            try
            {
                loadedTownScene = SceneManager.GetSceneByPath(TownScenePath);
                if (!loadedTownScene.IsValid() || !loadedTownScene.isLoaded)
                {
                    loadedTownScene = EditorSceneManager.OpenScene(TownScenePath, OpenSceneMode.Additive);
                    loadedTemporarily = true;
                }

                ProbeResult result = new ProbeResult
                {
                    timestamp = DateTime.Now.ToString("O")
                };

                if (!loadedTownScene.IsValid() || !loadedTownScene.isLoaded)
                {
                    result.status = "blocked";
                    result.success = false;
                    result.firstBlocker = "town-scene-unavailable";
                    result.message = "Town 场景当前无法只读加载，不能继续判定更深的 player-facing contract。";
                    result.blockingFindings.Add("Town 场景不可用。");
                    return result;
                }

                EvaluatePlayerFacingContract(loadedTownScene, result);
                FinalizeResult(result);
                return result;
            }
            finally
            {
                if (loadedTemporarily && loadedTownScene.IsValid() && loadedTownScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(loadedTownScene, true);
                }
            }
        }

        private static void EvaluatePlayerFacingContract(Scene townScene, ProbeResult result)
        {
            Camera mainCamera = FindMainCamera(townScene);
            CinemachineCamera cinemachineCamera = FindFirstComponentInScene<CinemachineCamera>(townScene);
            CameraDeadZoneSync cameraSync = FindFirstComponentInScene<CameraDeadZoneSync>(townScene);
            global::PlayerMovement playerMovement = FindFirstComponentInScene<global::PlayerMovement>(townScene);
            SceneTransitionTrigger2D transitionTrigger = FindFirstComponentInScene<SceneTransitionTrigger2D>(townScene);

            if (mainCamera == null)
            {
                AddBlocker(result, "main-camera-missing", "Town 缺少 Main Camera，无法继续判定 player-facing 视野。");
                return;
            }

            if (cinemachineCamera == null)
            {
                AddBlocker(result, "cinemachine-camera-missing", "Town 缺少 CinemachineCamera，无法继续判定玩家起步镜头。");
                return;
            }

            if (playerMovement == null)
            {
                AddBlocker(result, "player-movement-missing", "Town 缺少 PlayerMovement，无法继续判定玩家起步安全距离。");
                return;
            }

            if (cameraSync == null)
            {
                AddBlocker(result, "camera-sync-missing", "Town 缺少 CameraDeadZoneSync，无法继续判定相机 bounds/player-facing contract。");
                return;
            }

            if (transitionTrigger == null)
            {
                AddBlocker(result, "scene-transition-trigger-missing", "Town 缺少 SceneTransitionTrigger2D，无法继续判定玩家返回 Primary 的起步安全。");
                return;
            }

            Transform playerTransform = playerMovement.transform;
            Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
            Collider2D returnTriggerCollider = transitionTrigger.GetComponent<Collider2D>();
            PolygonCollider2D boundsCollider = ResolveBoundsCollider(cameraSync, townScene);

            result.camera.mainCameraPath = GetTransformPath(mainCamera.transform);
            result.camera.cinemachineCameraPath = GetTransformPath(cinemachineCamera.transform);
            result.camera.trackingTargetPath = cinemachineCamera.Follow != null ? GetTransformPath(cinemachineCamera.Follow) : string.Empty;
            result.camera.trackingTargetMatchesPlayer = cinemachineCamera.Follow == playerTransform;
            result.camera.virtualCameraPosition = FormatVector(cinemachineCamera.transform.position);

            Vector2 initialViewHalfExtents = ResolveInitialViewHalfExtents(mainCamera, cinemachineCamera);
            Vector3 initialViewCenter = new Vector3(playerTransform.position.x, playerTransform.position.y, 0f);
            result.camera.initialViewCenter = FormatVector(initialViewCenter);
            result.camera.initialViewHalfExtents = $"{initialViewHalfExtents.x:F3},{initialViewHalfExtents.y:F3}";
            result.camera.virtualCameraXYMatchesPlayer =
                Vector2.Distance(
                    new Vector2(cinemachineCamera.transform.position.x, cinemachineCamera.transform.position.y),
                    new Vector2(playerTransform.position.x, playerTransform.position.y)) <= 0.25f;

            if (!result.camera.trackingTargetMatchesPlayer)
            {
                AddBlocker(result, "cinemachine-tracking-target-mismatch", "Town 的 CinemachineCamera 当前没有真实跟住 Player，这会直接打到玩家进 Town 后的镜头体验。");
            }

            if (!result.camera.virtualCameraXYMatchesPlayer)
            {
                result.attentionFindings.Add("Town 的 CinemachineCamera 当前没有站在玩家起步位附近，运行时首屏可能会先跳一下再回正。");
            }

            result.player.playerPath = GetTransformPath(playerTransform);
            result.player.playerPosition = FormatVector(playerTransform.position);
            result.player.hasCollider2D = playerCollider != null;
            result.player.insideInitialView = Contains(initialViewCenter, initialViewHalfExtents, playerTransform.position);

            if (boundsCollider == null)
            {
                AddBlocker(result, "camera-bounds-missing", "Town 的 _CameraBounds 当前缺失，无法继续判定玩家是否在 confiner 安全区内。");
                return;
            }

            Bounds bounds = boundsCollider.bounds;
            result.bounds.colliderPath = GetTransformPath(boundsCollider.transform);
            result.bounds.worldMin = FormatVector(bounds.min);
            result.bounds.worldMax = FormatVector(bounds.max);
            result.bounds.containsPlayer = Contains(bounds, playerTransform.position);
            result.player.insideCameraBounds = result.bounds.containsPlayer;
            result.player.distanceToLeftBound = playerTransform.position.x - bounds.min.x;
            result.player.distanceToRightBound = bounds.max.x - playerTransform.position.x;
            result.player.distanceToBottomBound = playerTransform.position.y - bounds.min.y;
            result.player.distanceToTopBound = bounds.max.y - playerTransform.position.y;

            if (!result.player.insideCameraBounds)
            {
                AddBlocker(result, "player-outside-camera-bounds", "Town 玩家当前不在 _CameraBounds 内，进场后镜头/玩家边界体验不可信。");
            }

            result.transition.triggerPath = GetTransformPath(transitionTrigger.transform);
            result.transition.triggerPosition = FormatVector(transitionTrigger.transform.position);
            result.transition.insideCameraBounds = Contains(bounds, transitionTrigger.transform.position);
            result.transition.insideInitialView = Contains(initialViewCenter, initialViewHalfExtents, transitionTrigger.transform.position);
            result.bounds.containsReturnTrigger = result.transition.insideCameraBounds;

            if (returnTriggerCollider == null)
            {
                AddBlocker(result, "return-trigger-collider-missing", "Town 的返回触发器缺少 Collider2D，无法继续判定玩家起步安全距离。");
            }
            else
            {
                result.transition.playerOverlapsReturnTrigger = returnTriggerCollider.OverlapPoint(playerTransform.position);
                result.transition.returnTriggerEdgeDistance = ResolveEdgeDistance(playerCollider, returnTriggerCollider, playerTransform.position);

                if (result.transition.playerOverlapsReturnTrigger || result.transition.returnTriggerEdgeDistance <= BlockedReturnTriggerEdgeDistance)
                {
                    AddBlocker(result, "player-overlaps-return-trigger", "Town 玩家起步位与返回 Primary 的触发区发生重叠，玩家一进场就可能被立刻反咬回去。");
                }
                else if (result.transition.returnTriggerEdgeDistance <= AttentionReturnTriggerEdgeDistance)
                {
                    result.attentionFindings.Add($"Town 玩家起步位距离返回触发区只有 {result.transition.returnTriggerEdgeDistance:F2}，当前存在误触回切的边缘风险。");
                }
            }

            if (!result.transition.insideCameraBounds)
            {
                result.attentionFindings.Add("Town 的返回触发器当前不在 _CameraBounds 内，镜头边界与返回链之间可能还留着窄口。");
            }

            Dictionary<string, Transform> transformLookup = BuildTransformLookup(townScene);
            for (int index = 0; index < RuntimeAnchors.Length; index++)
            {
                string anchorName = RuntimeAnchors[index];
                transformLookup.TryGetValue(anchorName, out Transform anchor);
                AnchorFlowRecord record = BuildAnchorRecord(anchorName, anchor, bounds, initialViewCenter, initialViewHalfExtents, playerTransform.position);
                result.anchors.Add(record);

                if (!record.exists)
                {
                    if (Array.IndexOf(CriticalEntryAnchors, anchorName) >= 0)
                    {
                        AddBlocker(result, "critical-entry-anchor-missing", $"Town 缺少 {anchorName}，当前入口第一屏的玩家-facing 演出锚点不完整。");
                    }
                    else
                    {
                        result.attentionFindings.Add($"Town 缺少 {anchorName}，后续更深 runtime 消费仍会撞到这处空洞。");
                    }

                    continue;
                }

                if (!record.insideCameraBounds)
                {
                    result.attentionFindings.Add($"{anchorName} 当前不在 _CameraBounds 内，后续 runtime 拉人过去时镜头边界可能先撞。");
                }

                if (Array.IndexOf(CriticalEntryAnchors, anchorName) >= 0 && !record.insideInitialView)
                {
                    result.attentionFindings.Add($"{anchorName} 当前不在玩家入 Town 的第一屏视野里，入口群像可能会显得空。");
                }
            }
        }

        private static AnchorFlowRecord BuildAnchorRecord(
            string anchorName,
            Transform anchor,
            Bounds bounds,
            Vector3 initialViewCenter,
            Vector2 initialViewHalfExtents,
            Vector3 playerPosition)
        {
            AnchorFlowRecord record = new AnchorFlowRecord
            {
                anchorName = anchorName,
                exists = anchor != null,
                anchorPath = anchor != null ? GetTransformPath(anchor) : string.Empty,
                worldPosition = anchor != null ? FormatVector(anchor.position) : string.Empty,
                insideCameraBounds = anchor != null && Contains(bounds, anchor.position),
                insideInitialView = anchor != null && Contains(initialViewCenter, initialViewHalfExtents, anchor.position),
                distanceToPlayer = anchor != null ? Vector2.Distance(anchor.position, playerPosition) : -1f
            };

            return record;
        }

        private static PolygonCollider2D ResolveBoundsCollider(CameraDeadZoneSync sync, Scene townScene)
        {
            SerializedProperty boundingColliderProperty = new SerializedObject(sync).FindProperty("boundingCollider");
            if (boundingColliderProperty != null && boundingColliderProperty.objectReferenceValue is PolygonCollider2D serializedCollider)
            {
                return serializedCollider;
            }

            return FindFirstComponentInScene<PolygonCollider2D>(townScene, candidate =>
                candidate != null && string.Equals(candidate.gameObject.name, "_CameraBounds", StringComparison.Ordinal));
        }

        private static float ResolveEdgeDistance(Collider2D playerCollider, Collider2D triggerCollider, Vector3 playerPosition)
        {
            if (playerCollider != null && triggerCollider != null)
            {
                ColliderDistance2D distance = triggerCollider.Distance(playerCollider);
                return distance.distance;
            }

            return DistanceFromPointToBounds(playerPosition, triggerCollider != null ? triggerCollider.bounds : default);
        }

        private static float DistanceFromPointToBounds(Vector3 point, Bounds bounds)
        {
            if (Contains(bounds, point))
            {
                return 0f;
            }

            float clampedX = Mathf.Clamp(point.x, bounds.min.x, bounds.max.x);
            float clampedY = Mathf.Clamp(point.y, bounds.min.y, bounds.max.y);
            return Vector2.Distance(new Vector2(point.x, point.y), new Vector2(clampedX, clampedY));
        }

        private static Vector2 ResolveInitialViewHalfExtents(Camera mainCamera, CinemachineCamera cinemachineCamera)
        {
            float aspect = mainCamera != null && mainCamera.aspect > 0.01f ? mainCamera.aspect : (16f / 9f);
            float orthographicSize = 10.5f;

            if (cinemachineCamera != null)
            {
                orthographicSize = cinemachineCamera.Lens.OrthographicSize;
            }
            else if (mainCamera != null && mainCamera.orthographic)
            {
                orthographicSize = mainCamera.orthographicSize;
            }

            return new Vector2(orthographicSize * aspect, orthographicSize);
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
                result.success = false;
                result.firstBlocker = string.Empty;
                result.message = result.attentionFindings[0];
                return;
            }

            result.status = "completed";
            result.success = true;
            result.firstBlocker = string.Empty;
            result.message = "Town 的 player-facing contract 已推进到玩家起步安全、相机跟随目标对齐和入口第一屏演出锚点成立这一层。";
        }

        private static void AddBlocker(ProbeResult result, string blockerCode, string message)
        {
            if (string.IsNullOrWhiteSpace(result.firstBlocker))
            {
                result.firstBlocker = blockerCode;
            }

            result.blockingFindings.Add(message);
        }

        private static bool Contains(Bounds bounds, Vector3 position)
        {
            return position.x >= bounds.min.x &&
                   position.x <= bounds.max.x &&
                   position.y >= bounds.min.y &&
                   position.y <= bounds.max.y;
        }

        private static bool Contains(Vector3 center, Vector2 halfExtents, Vector3 position)
        {
            return Mathf.Abs(position.x - center.x) <= halfExtents.x &&
                   Mathf.Abs(position.y - center.y) <= halfExtents.y;
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

                if (score > bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            return best;
        }

        private static Dictionary<string, Transform> BuildTransformLookup(Scene scene)
        {
            Dictionary<string, Transform> lookup = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                CollectTransforms(roots[index].transform, lookup);
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

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            return FindFirstComponentInScene<T>(scene, _ => true);
        }

        private static T FindFirstComponentInScene<T>(Scene scene, Predicate<T> predicate) where T : Component
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
            {
                T[] components = roots[rootIndex].GetComponentsInChildren<T>(true);
                for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    T candidate = components[componentIndex];
                    if (candidate == null || (predicate != null && !predicate(candidate)))
                    {
                        continue;
                    }

                    return candidate;
                }
            }

            return null;
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

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            Stack<string> stack = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                stack.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", stack.ToArray());
        }

        private static string FormatVector(Vector3 value)
        {
            return $"{value.x:F3},{value.y:F3},{value.z:F3}";
        }
    }
}
#endif
