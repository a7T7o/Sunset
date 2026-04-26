using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class ScenePartialSyncValidationMenu
    {
        private const string ScratchSceneDirectory = "Assets/__CodexSceneSyncScratch";
        private const string ArtifactDirectory = ".codex/artifacts/scene-sync-validation";
        private const string CurrentPrimaryPath = "Assets/000_Scenes/Primary.unity";
        private const string BackupPrimaryPath = "Assets/000_Scenes/primary_backup_2026-04-02_20-46-54.unity";

        [Serializable]
        private sealed class ValidationReport
        {
            public string timestamp = string.Empty;
            public bool minimalCopySuccess;
            public bool minimalOverwriteSuccess;
            public bool realSceneCopySuccess;
            public string artifactPath = string.Empty;
            public string scratchSceneDirectory = string.Empty;
            public ScenePartialSyncTool.SceneSyncExecutionReport minimalCopyReport;
            public ScenePartialSyncTool.SceneSyncExecutionReport minimalOverwriteReport;
            public ScenePartialSyncTool.SceneSyncExecutionReport realSceneCopyReport;
            public string[] verifiedPaths = Array.Empty<string>();
            public string message = string.Empty;
        }

        [MenuItem("Tools/Sunset/Scene/运行局部同步工具副本自测")]
        private static void RunValidation()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning("[ScenePartialSyncValidation] 当前 Unity 仍在 PlayMode 或忙碌中，已阻断自测。");
                return;
            }

            ValidationReport report = new ValidationReport
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                scratchSceneDirectory = ScratchSceneDirectory
            };

            try
            {
                PrepareScratchDirectory();

                report.minimalCopyReport = RunMinimalCopyValidation();
                report.minimalCopySuccess = report.minimalCopyReport.Success
                    && ScenePathExists(GetScratchPath("MinimalTarget.unity"), "RootA")
                    && ScenePathExists(GetScratchPath("MinimalTarget.unity"), "RootB/OldBranch");

                report.minimalOverwriteReport = RunMinimalOverwriteValidation();
                report.minimalOverwriteSuccess = report.minimalOverwriteReport.Success
                    && ScenePathExists(GetScratchPath("MinimalTarget.unity"), "RootB/NewBranch")
                    && !ScenePathExists(GetScratchPath("MinimalTarget.unity"), "RootB/OldBranch");

                report.realSceneCopyReport = RunRealSceneValidation();
                report.realSceneCopySuccess = report.realSceneCopyReport.Success
                    && ScenePathExists(GetScratchPath("PrimaryValidationTarget.unity"), "PersistentManagers")
                    && ScenePathExists(GetScratchPath("PrimaryValidationTarget.unity"), "StoryManager");

                report.verifiedPaths = new[]
                {
                    "MinimalTarget.unity :: RootA",
                    "MinimalTarget.unity :: RootB/NewBranch",
                    "PrimaryValidationTarget.unity :: PersistentManagers",
                    "PrimaryValidationTarget.unity :: StoryManager"
                };

                report.message = report.minimalCopySuccess && report.minimalOverwriteSuccess && report.realSceneCopySuccess
                    ? "ScenePartialSyncTool 副本自测已通过。"
                    : "ScenePartialSyncTool 副本自测未全部通过，请查看报告。";
            }
            catch (Exception exception)
            {
                report.message = $"副本自测异常：{exception.GetType().Name}: {exception.Message}";
            }

            report.artifactPath = WriteArtifact(report);
            Debug.Log($"[ScenePartialSyncValidation] {report.message}\nArtifact: {report.artifactPath}");
        }

        private static ScenePartialSyncTool.SceneSyncExecutionReport RunMinimalCopyValidation()
        {
            string sourcePath = GetScratchPath("MinimalSource.unity");
            string targetPath = GetScratchPath("MinimalTarget.unity");

            CreateMinimalSourceScene(sourcePath);
            CreateMinimalTargetScene(targetPath);

            return ScenePartialSyncTool.ExecuteSceneSync(
                sourcePath,
                targetPath,
                new[] { "RootA" },
                ScenePartialSyncTool.SyncMode.CopyMissingOnly,
                saveTargetScene: true,
                requireConfirmation: false);
        }

        private static ScenePartialSyncTool.SceneSyncExecutionReport RunMinimalOverwriteValidation()
        {
            string sourcePath = GetScratchPath("MinimalSource.unity");
            string targetPath = GetScratchPath("MinimalTarget.unity");

            return ScenePartialSyncTool.ExecuteSceneSync(
                sourcePath,
                targetPath,
                new[] { "RootB" },
                ScenePartialSyncTool.SyncMode.OverwriteByPath,
                saveTargetScene: true,
                requireConfirmation: false);
        }

        private static ScenePartialSyncTool.SceneSyncExecutionReport RunRealSceneValidation()
        {
            string sourcePath = GetScratchPath("PrimaryValidationSource.unity");
            string targetPath = GetScratchPath("PrimaryValidationTarget.unity");

            CopySceneAsset(CurrentPrimaryPath, targetPath);
            CopySceneAsset(BackupPrimaryPath, sourcePath);

            return ScenePartialSyncTool.ExecuteSceneSync(
                sourcePath,
                targetPath,
                new[] { "PersistentManagers", "StoryManager" },
                ScenePartialSyncTool.SyncMode.CopyMissingOnly,
                saveTargetScene: true,
                requireConfirmation: false);
        }

        private static void CreateMinimalSourceScene(string scenePath)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                GameObject rootA = new GameObject("RootA");
                SceneManager.MoveGameObjectToScene(rootA, scene);
                new GameObject("ChildA").transform.SetParent(rootA.transform, false);

                GameObject rootB = new GameObject("RootB");
                SceneManager.MoveGameObjectToScene(rootB, scene);
                new GameObject("NewBranch").transform.SetParent(rootB.transform, false);

                EditorSceneManager.SaveScene(scene, scenePath);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void CreateMinimalTargetScene(string scenePath)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                GameObject rootB = new GameObject("RootB");
                SceneManager.MoveGameObjectToScene(rootB, scene);
                new GameObject("OldBranch").transform.SetParent(rootB.transform, false);

                EditorSceneManager.SaveScene(scene, scenePath);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void PrepareScratchDirectory()
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), ScratchSceneDirectory.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        private static string GetScratchPath(string fileName)
        {
            return $"{ScratchSceneDirectory}/{fileName}";
        }

        private static void CopySceneAsset(string sourceProjectRelativePath, string targetProjectRelativePath)
        {
            string projectRoot = Directory.GetCurrentDirectory();
            string sourceFullPath = Path.Combine(projectRoot, sourceProjectRelativePath.Replace('/', Path.DirectorySeparatorChar));
            string targetFullPath = Path.Combine(projectRoot, targetProjectRelativePath.Replace('/', Path.DirectorySeparatorChar));

            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath) ?? projectRoot);
            File.Copy(sourceFullPath, targetFullPath, overwrite: true);

            string sourceMeta = sourceFullPath + ".meta";
            string targetMeta = targetFullPath + ".meta";
            if (File.Exists(sourceMeta))
            {
                File.Copy(sourceMeta, targetMeta, overwrite: true);
            }

            AssetDatabase.ImportAsset(targetProjectRelativePath, ImportAssetOptions.ForceUpdate);
        }

        private static bool ScenePathExists(string scenePath, string transformPath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            try
            {
                return TryFindTransformByPath(scene, transformPath, out _);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static bool TryFindTransformByPath(Scene scene, string path, out Transform result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string[] segments = path.Split('/');
            GameObject rootObject = Array.Find(scene.GetRootGameObjects(), root => root.name == segments[0]);
            if (rootObject == null)
            {
                return false;
            }

            Transform current = rootObject.transform;
            for (int index = 1; index < segments.Length; index++)
            {
                current = current.Find(segments[index]);
                if (current == null)
                {
                    return false;
                }
            }

            result = current;
            return true;
        }

        private static string WriteArtifact(ValidationReport report)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), ArtifactDirectory.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);

            string fullPath = Path.Combine(directory, $"{report.timestamp}_scene-partial-sync-validation.json");
            File.WriteAllText(fullPath, JsonUtility.ToJson(report, true));
            return fullPath;
        }
    }
}
