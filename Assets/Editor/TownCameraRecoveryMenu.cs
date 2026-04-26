#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class TownCameraRecoveryMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Town恢复Main Camera（从事故前备份）";
        private const string TargetScenePath = "Assets/000_Scenes/Town.unity";
        private const string BackupScenePath = ".codex/artifacts/town-foundation/backups/Town_before_bootstrap_2026-04-03_02-37-16.unity";
        private const string ScratchDirectory = "Assets/__CodexSceneSyncScratch";
        private const string ScratchScenePath = ScratchDirectory + "/TownCameraRecoverySource.unity";
        private const string ArtifactDirectory = ".codex/artifacts/town-foundation";

        [Serializable]
        private sealed class TownCameraRecoveryReport
        {
            public string timestamp = string.Empty;
            public string targetScenePath = TargetScenePath;
            public string sourceBackupPath = BackupScenePath;
            public string scratchScenePath = ScratchScenePath;
            public string sceneBackupPath = string.Empty;
            public bool success;
            public string message = string.Empty;
            public string targetCameraPathBefore = string.Empty;
            public string targetCameraPathAfter = string.Empty;
            public string sourceCameraPath = string.Empty;
            public List<string> sourceRootObjects = new List<string>();
            public List<string> targetRootObjects = new List<string>();
            public List<string> sourceCameraCandidates = new List<string>();
            public List<string> targetCameraCandidates = new List<string>();
        }

        [MenuItem(MenuPath)]
        private static void RecoverTownMainCamera()
        {
            TownCameraRecoveryReport report = new TownCameraRecoveryReport
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
            };

            try
            {
                EnsureEditorIsReady();
                report.sceneBackupPath = CreateTownSceneBackup(report.timestamp);
                PrepareScratchSceneFromBackup();

                bool townSceneWasAlreadyLoaded = false;
                Scene targetScene = GetOrOpenTargetScene(ref townSceneWasAlreadyLoaded);
                Scene scratchScene = EditorSceneManager.OpenScene(ScratchScenePath, OpenSceneMode.Additive);

                try
                {
                    report.sourceRootObjects = GetRootObjectPaths(scratchScene);
                    report.targetRootObjects = GetRootObjectPaths(targetScene);
                    report.sourceCameraCandidates = FindCameraCandidatePaths(scratchScene);
                    report.targetCameraCandidates = FindCameraCandidatePaths(targetScene);

                    GameObject sourceCamera = FindPreferredCamera(scratchScene, preferRootMainCamera: true);
                    if (sourceCamera == null)
                    {
                        throw new InvalidOperationException("事故前备份里找不到可用 Main Camera。");
                    }

                    GameObject targetCamera = FindPreferredCamera(targetScene, preferRootMainCamera: false);
                    if (targetCamera == null)
                    {
                        throw new InvalidOperationException("Town 当前场景里找不到 Main Camera。");
                    }

                    report.sourceCameraPath = GetTransformPath(sourceCamera.transform);
                    report.targetCameraPathBefore = GetTransformPath(targetCamera.transform);

                    SyncGameObjectFromSource(sourceCamera, targetCamera);

                    report.targetCameraPathAfter = GetTransformPath(targetCamera.transform);
                    report.success = EditorSceneManager.SaveScene(targetScene);
                    report.message = report.success
                        ? "Town Main Camera 已按事故前备份恢复。"
                        : "Town Main Camera 恢复已执行，但保存 Town 失败。";
                }
                finally
                {
                    EditorSceneManager.CloseScene(scratchScene, true);
                    if (!townSceneWasAlreadyLoaded)
                    {
                        EditorSceneManager.CloseScene(targetScene, true);
                    }
                }
            }
            catch (Exception exception)
            {
                report.success = false;
                report.message = $"{exception.GetType().Name}: {exception.Message}";
            }

            string artifactPath = WriteArtifact(report);
            Debug.Log($"[TownCameraRecovery] {report.message}\nArtifact: {artifactPath}");
        }

        private static void EnsureEditorIsReady()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                throw new InvalidOperationException("Unity 当前仍在 PlayMode、编译或刷新中，已阻断 Town 相机恢复。");
            }
        }

        private static Scene GetOrOpenTargetScene(ref bool townSceneWasAlreadyLoaded)
        {
            Scene targetScene = SceneManager.GetSceneByPath(TargetScenePath);
            if (targetScene.IsValid() && targetScene.isLoaded)
            {
                townSceneWasAlreadyLoaded = true;
                return targetScene;
            }

            townSceneWasAlreadyLoaded = false;
            return EditorSceneManager.OpenScene(TargetScenePath, OpenSceneMode.Additive);
        }

        private static string CreateTownSceneBackup(string timestamp)
        {
            string backupPath = $".codex/artifacts/town-foundation/backups/Town_before_camera_recovery_{timestamp}.unity";
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath) ?? ArtifactDirectory);
            File.Copy(TargetScenePath, backupPath, overwrite: true);

            string metaSource = TargetScenePath + ".meta";
            string metaBackup = backupPath + ".meta";
            if (File.Exists(metaSource))
            {
                File.Copy(metaSource, metaBackup, overwrite: true);
            }

            return backupPath;
        }

        private static void PrepareScratchSceneFromBackup()
        {
            ValidateBackupSceneLooksSafe();
            Directory.CreateDirectory(ScratchDirectory);
            File.Copy(BackupScenePath, ScratchScenePath, overwrite: true);

            string metaSource = BackupScenePath + ".meta";
            string metaTarget = ScratchScenePath + ".meta";
            if (File.Exists(metaSource))
            {
                File.Copy(metaSource, metaTarget, overwrite: true);
            }

            AssetDatabase.ImportAsset(ScratchScenePath, ImportAssetOptions.ForceUpdate);
        }

        private static void ValidateBackupSceneLooksSafe()
        {
            if (!File.Exists(BackupScenePath))
            {
                throw new FileNotFoundException("Town 相机恢复备份不存在。", BackupScenePath);
            }

            string backupText = File.ReadAllText(BackupScenePath);
            if (backupText.Contains("&9223372036854775808", StringComparison.Ordinal) ||
                backupText.Contains("fileID: 9223372036854775808", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Town 相机恢复备份仍包含超界 fileID，已阻断执行：{BackupScenePath}");
            }
        }

        private static void SyncGameObjectFromSource(GameObject source, GameObject target)
        {
            Transform sourceParent = source.transform.parent;
            Transform targetParent = sourceParent == null ? null : FindTransformByPath(target.scene, GetTransformPath(sourceParent));
            target.transform.SetParent(targetParent, false);

            target.name = source.name;
            target.tag = source.tag;
            target.layer = source.layer;
            target.SetActive(source.activeSelf);
            target.transform.localPosition = source.transform.localPosition;
            target.transform.localRotation = source.transform.localRotation;
            target.transform.localScale = source.transform.localScale;

            List<Component> sourceComponents = source.GetComponents<Component>()
                .Where(component => component != null && component is not Transform)
                .ToList();
            List<Component> targetComponents = target.GetComponents<Component>()
                .Where(component => component != null && component is not Transform)
                .ToList();

            HashSet<Type> sourceTypes = new HashSet<Type>(sourceComponents.Select(component => component.GetType()));
            List<Component> extraComponents = targetComponents
                .Where(component => !sourceTypes.Contains(component.GetType()))
                .ToList();

            foreach (Component component in extraComponents)
            {
                UnityEngine.Object.DestroyImmediate(component, true);
            }

            foreach (IGrouping<Type, Component> sourceGroup in sourceComponents.GroupBy(component => component.GetType()))
            {
                List<Component> sourceList = sourceGroup.ToList();
                List<Component> targetList = target.GetComponents(sourceGroup.Key).Cast<Component>().ToList();

                while (targetList.Count < sourceList.Count)
                {
                    targetList.Add(target.AddComponent(sourceGroup.Key));
                }

                while (targetList.Count > sourceList.Count)
                {
                    Component extra = targetList[targetList.Count - 1];
                    targetList.RemoveAt(targetList.Count - 1);
                    UnityEngine.Object.DestroyImmediate(extra, true);
                }

                for (int index = 0; index < sourceList.Count; index++)
                {
                    EditorUtility.CopySerialized(sourceList[index], targetList[index]);
                }
            }
        }

        private static GameObject FindPreferredCamera(Scene scene, bool preferRootMainCamera)
        {
            List<Transform> candidates = EnumerateSceneTransforms(scene)
                .Where(transform =>
                    string.Equals(transform.name, "Main Camera", StringComparison.Ordinal) ||
                    transform.GetComponent<Camera>() != null)
                .ToList();

            if (preferRootMainCamera)
            {
                Transform rootMainCamera = candidates.FirstOrDefault(transform =>
                    string.Equals(transform.name, "Main Camera", StringComparison.Ordinal) &&
                    transform.parent == null);
                if (rootMainCamera != null)
                {
                    return rootMainCamera.gameObject;
                }
            }

            Transform nestedMainCamera = candidates.FirstOrDefault(transform =>
                string.Equals(GetTransformPath(transform), "Camera/Main Camera", StringComparison.Ordinal));
            if (nestedMainCamera != null)
            {
                return nestedMainCamera.gameObject;
            }

            Transform anyMainCamera = candidates.FirstOrDefault(transform =>
                string.Equals(transform.name, "Main Camera", StringComparison.Ordinal));
            if (anyMainCamera != null)
            {
                return anyMainCamera.gameObject;
            }

            Transform anyCameraWithComponent = candidates.FirstOrDefault(transform => transform.GetComponent<Camera>() != null);
            return anyCameraWithComponent != null ? anyCameraWithComponent.gameObject : null;
        }

        private static List<string> FindCameraCandidatePaths(Scene scene)
        {
            return EnumerateSceneTransforms(scene)
                .Where(transform =>
                    string.Equals(transform.name, "Main Camera", StringComparison.Ordinal) ||
                    transform.GetComponent<Camera>() != null)
                .Select(GetTransformPath)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToList();
        }

        private static List<string> GetRootObjectPaths(Scene scene)
        {
            return scene.GetRootGameObjects()
                .Select(rootObject => rootObject.name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();
        }

        private static IEnumerable<Transform> EnumerateSceneTransforms(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                foreach (Transform transform in rootObject.GetComponentsInChildren<Transform>(true))
                {
                    if (transform != null && transform.gameObject.scene == scene)
                    {
                        yield return transform;
                    }
                }
            }
        }

        private static GameObject FindGameObjectByPath(Scene scene, string path)
        {
            Transform transform = FindTransformByPath(scene, path);
            return transform != null ? transform.gameObject : null;
        }

        private static Transform FindTransformByPath(Scene scene, string path)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == path)
                {
                    return rootObject.transform;
                }

                string[] segments = path.Split('/');
                if (segments.Length == 0 || rootObject.name != segments[0])
                {
                    continue;
                }

                Transform current = rootObject.transform;
                bool matched = true;
                for (int index = 1; index < segments.Length; index++)
                {
                    current = current.Find(segments[index]);
                    if (current == null)
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    return current;
                }
            }

            return null;
        }

        private static string GetTransformPath(Transform transform)
        {
            Stack<string> names = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names);
        }

        private static string WriteArtifact(TownCameraRecoveryReport report)
        {
            Directory.CreateDirectory(ArtifactDirectory);
            string artifactPath = Path.Combine(
                ArtifactDirectory,
                $"town-camera-recovery_{report.timestamp}.json");

            string payload = JsonUtility.ToJson(report, true);
            File.WriteAllText(artifactPath, payload);
            return artifactPath.Replace("\\", "/");
        }
    }
}
#endif
