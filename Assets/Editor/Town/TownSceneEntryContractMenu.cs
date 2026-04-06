#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sunset.Story;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class TownSceneEntryContractMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Run Town Entry Contract Probe";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";
        private const string ExpectedReturnSceneName = "Primary";
        private const string ExpectedReturnScenePath = "Assets/000_Scenes/Primary.unity";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "town-entry-contract-probe.json");

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string firstBlocker = string.Empty;
            public string message = string.Empty;
            public string scenePath = TownScenePath;
            public CameraRecord camera = new CameraRecord();
            public PlayerRecord player = new PlayerRecord();
            public TransitionRecord transition = new TransitionRecord();
            public List<string> blockingFindings = new List<string>();
            public List<string> attentionFindings = new List<string>();
        }

        [Serializable]
        private sealed class CameraRecord
        {
            public string mainCameraPath = string.Empty;
            public bool hasMainCameraTag;
            public bool hasAudioListener;
            public bool hasCinemachineBrain;
            public string cinemachineCameraPath = string.Empty;
            public bool hasCameraDeadZoneSync;
            public bool syncReferencesMainCamera;
            public bool syncReferencesCinemachineCamera;
            public bool syncHasBoundingCollider;
        }

        [Serializable]
        private sealed class PlayerRecord
        {
            public string playerPath = string.Empty;
            public bool taggedPlayer;
            public bool hasPlayerMovement;
            public string playerPosition = string.Empty;
        }

        [Serializable]
        private sealed class TransitionRecord
        {
            public string triggerPath = string.Empty;
            public bool hasCollider2D;
            public bool colliderIsTrigger;
            public bool triggerOnPlayerEnter;
            public string playerTag = string.Empty;
            public bool hasValidTarget;
            public string targetSceneName = string.Empty;
            public string targetScenePath = string.Empty;
            public string triggerPosition = string.Empty;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[TownEntryContractProbe] {result.message}");
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
                    result.message = "Town 场景当前无法只读加载，不能继续判定相机/转场/玩家契约。";
                    result.blockingFindings.Add("Town 场景不可用。");
                    return result;
                }

                EvaluateCameraContract(loadedTownScene, result);
                EvaluatePlayerContract(loadedTownScene, result);
                EvaluateTransitionContract(loadedTownScene, result);
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

        private static void EvaluateCameraContract(Scene townScene, ProbeResult result)
        {
            Camera mainCamera = FindMainCamera(townScene);
            if (mainCamera == null)
            {
                result.blockingFindings.Add("Town 缺少 Main Camera。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "main-camera-missing");
                return;
            }

            result.camera.mainCameraPath = GetTransformPath(mainCamera.transform);
            result.camera.hasMainCameraTag = mainCamera.CompareTag("MainCamera");
            result.camera.hasAudioListener = mainCamera.GetComponent<AudioListener>() != null;
            result.camera.hasCinemachineBrain = mainCamera.GetComponent<CinemachineBrain>() != null;

            if (!result.camera.hasMainCameraTag)
            {
                result.blockingFindings.Add("Main Camera 未保持 MainCamera tag。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "main-camera-tag-mismatch");
            }

            if (!result.camera.hasAudioListener)
            {
                result.blockingFindings.Add("Main Camera 缺少 AudioListener。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "main-camera-audio-listener-missing");
            }

            if (!result.camera.hasCinemachineBrain)
            {
                result.attentionFindings.Add("Main Camera 缺少 CinemachineBrain，当前会依赖 runtime 自愈。");
            }

            CinemachineCamera cinemachineCamera = FindFirstComponentInScene<CinemachineCamera>(townScene);
            if (cinemachineCamera == null)
            {
                result.blockingFindings.Add("Town 缺少 CinemachineCamera。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "cinemachine-camera-missing");
                return;
            }

            result.camera.cinemachineCameraPath = GetTransformPath(cinemachineCamera.transform);

            MonoBehaviour cameraSync = FindNamedMonoBehaviour(cinemachineCamera.gameObject, "CameraDeadZoneSync");
            if (cameraSync == null)
            {
                result.attentionFindings.Add("CinemachineCamera 上没有 CameraDeadZoneSync，当前缺少 Town 相机 confiner/自愈挂点。");
                return;
            }

            result.camera.hasCameraDeadZoneSync = true;
            SerializedObject serializedSync = new SerializedObject(cameraSync);
            SerializedProperty mainCameraProperty = serializedSync.FindProperty("mainCamera");
            SerializedProperty cinemachineCameraProperty = serializedSync.FindProperty("cinemachineCamera");
            SerializedProperty boundingColliderProperty = serializedSync.FindProperty("boundingCollider");

            Camera referencedMainCamera = mainCameraProperty != null ? mainCameraProperty.objectReferenceValue as Camera : null;
            CinemachineCamera referencedCinemachine = cinemachineCameraProperty != null ? cinemachineCameraProperty.objectReferenceValue as CinemachineCamera : null;
            Collider2D referencedBoundingCollider = boundingColliderProperty != null ? boundingColliderProperty.objectReferenceValue as Collider2D : null;

            result.camera.syncReferencesMainCamera = referencedMainCamera == mainCamera;
            result.camera.syncReferencesCinemachineCamera = referencedCinemachine == cinemachineCamera;
            result.camera.syncHasBoundingCollider = referencedBoundingCollider != null;

            if (!result.camera.syncReferencesMainCamera)
            {
                result.attentionFindings.Add("CameraDeadZoneSync.mainCamera 未显式指向当前 Town Main Camera，当前仍依赖 runtime 自动解析。");
            }

            if (!result.camera.syncReferencesCinemachineCamera)
            {
                result.attentionFindings.Add("CameraDeadZoneSync.cinemachineCamera 未显式指向当前 Town CinemachineCamera，当前仍依赖 runtime 自动解析。");
            }

            if (!result.camera.syncHasBoundingCollider)
            {
                result.attentionFindings.Add("CameraDeadZoneSync.boundingCollider 未显式配置，当前主要依赖自动边界检测。");
            }
        }

        private static void EvaluatePlayerContract(Scene townScene, ProbeResult result)
        {
            PlayerMovement playerMovement = FindFirstComponentInScene<global::PlayerMovement>(townScene);
            if (playerMovement == null)
            {
                result.blockingFindings.Add("Town 缺少 PlayerMovement。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "player-movement-missing");
                return;
            }

            GameObject playerObject = playerMovement.gameObject;
            result.player.playerPath = GetTransformPath(playerObject.transform);
            result.player.taggedPlayer = playerObject.CompareTag("Player");
            result.player.hasPlayerMovement = true;
            result.player.playerPosition = FormatVector(playerObject.transform.position);

            if (!result.player.taggedPlayer)
            {
                result.blockingFindings.Add("Town 玩家对象未保持 Player tag。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "player-tag-mismatch");
            }
        }

        private static void EvaluateTransitionContract(Scene townScene, ProbeResult result)
        {
            SceneTransitionTrigger2D transitionTrigger = FindFirstComponentInScene<SceneTransitionTrigger2D>(townScene);
            if (transitionTrigger == null)
            {
                result.blockingFindings.Add("Town 缺少 SceneTransitionTrigger2D。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-trigger-missing");
                return;
            }

            GameObject triggerObject = transitionTrigger.gameObject;
            Collider2D triggerCollider = triggerObject.GetComponent<Collider2D>();
            result.transition.triggerPath = GetTransformPath(triggerObject.transform);
            result.transition.hasCollider2D = triggerCollider != null;
            result.transition.colliderIsTrigger = triggerCollider != null && triggerCollider.isTrigger;
            result.transition.triggerPosition = FormatVector(triggerObject.transform.position);
            result.transition.triggerOnPlayerEnter = ReadSerializedBool(transitionTrigger, "triggerOnPlayerEnter");
            result.transition.playerTag = ReadSerializedString(transitionTrigger, "playerTag");
            result.transition.hasValidTarget = transitionTrigger.HasValidTarget;
            result.transition.targetSceneName = transitionTrigger.TargetSceneName;
            result.transition.targetScenePath = transitionTrigger.TargetScenePath;

            if (!result.transition.hasCollider2D)
            {
                result.blockingFindings.Add("SceneTransitionTrigger 缺少 Collider2D。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-collider-missing");
            }
            else if (!result.transition.colliderIsTrigger)
            {
                result.attentionFindings.Add("SceneTransitionTrigger 的 Collider2D 当前不是 trigger，运行时会依赖脚本自愈。");
            }

            if (!result.transition.triggerOnPlayerEnter)
            {
                result.blockingFindings.Add("SceneTransitionTrigger 未启用玩家进入即切场。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-player-enter-disabled");
            }

            if (!string.Equals(result.transition.playerTag, "Player", StringComparison.Ordinal))
            {
                result.blockingFindings.Add("SceneTransitionTrigger.playerTag 不是 Player。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-player-tag-mismatch");
            }

            if (!result.transition.hasValidTarget)
            {
                result.blockingFindings.Add("SceneTransitionTrigger 当前没有可解析的目标场景。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-target-invalid");
                return;
            }

            if (!string.Equals(result.transition.targetSceneName, ExpectedReturnSceneName, StringComparison.Ordinal) ||
                !string.Equals(result.transition.targetScenePath, ExpectedReturnScenePath, StringComparison.Ordinal))
            {
                result.blockingFindings.Add(
                    $"SceneTransitionTrigger 当前目标是 Name='{result.transition.targetSceneName}', Path='{result.transition.targetScenePath}'，未对齐预期返回场景 Primary。");
                result.firstBlocker = SetFirstBlocker(result.firstBlocker, "scene-transition-target-mismatch");
            }
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
            result.message = "Town 的 Main Camera / Cinemachine / Player / SceneTransition 当前已形成可直接消费的 scene-side entry contract。";
        }

        private static string SetFirstBlocker(string current, string next)
        {
            return string.IsNullOrWhiteSpace(current) ? next : current;
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
                    score += 300;
                }

                if (candidate.GetComponent<CinemachineBrain>() != null)
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

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            List<T> components = FindComponentsInScene<T>(scene);
            return components.Count > 0 ? components[0] : null;
        }

        private static List<T> FindComponentsInScene<T>(Scene scene) where T : Component
        {
            List<T> results = new List<T>();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return results;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
            {
                T[] components = roots[rootIndex].GetComponentsInChildren<T>(true);
                for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    T component = components[componentIndex];
                    if (component != null && component.gameObject.scene == scene)
                    {
                        results.Add(component);
                    }
                }
            }

            return results;
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
    }
}
#endif
