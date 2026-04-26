using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class ScenePrimaryBackupScratchDryRunMenu
    {
        private const string CurrentPrimaryPath = "Assets/000_Scenes/Primary.unity";
        private const string BackupPrimaryPath = "Assets/000_Scenes/primary_backup_2026-04-02_20-46-54.unity";
        private const string ScratchSceneDirectory = "Assets/__CodexSceneSyncScratch";
        private const string ScratchSourceScenePath = ScratchSceneDirectory + "/PrimaryBridgeVegetationDryRunSource.unity";
        private const string ScratchTargetScenePath = ScratchSceneDirectory + "/PrimaryBridgeVegetationDryRunTarget.unity";
        private const string ArtifactDirectory = ".codex/artifacts/scene-sync-dry-run";

        private static readonly string[] SyncPaths =
        {
            "SCENE/LAYER 1/Tilemap/桥",
            "SCENE/LAYER 1/Tilemap/植被"
        };

        private static readonly string[] GuardMissingPaths =
        {
            "SCENE/LAYER 1/Tilemap/农田",
            "SCENE/LAYER 1/Tilemap/基础地皮"
        };

        [Serializable]
        private sealed class DryRunReport
        {
            public string timestamp = string.Empty;
            public string sourceOriginalScenePath = BackupPrimaryPath;
            public string targetOriginalScenePath = CurrentPrimaryPath;
            public string actualSourceScenePath = ScratchSourceScenePath;
            public string actualTargetScenePath = ScratchTargetScenePath;
            public bool usedLoadedSourceCopy;
            public bool sourceWasDirty;
            public bool usedLoadedTargetCopy;
            public bool targetWasDirty;
            public string[] selectedPaths = Array.Empty<string>();
            public string[] guardMissingPaths = Array.Empty<string>();
            public string[] verifiedPresentPaths = Array.Empty<string>();
            public string[] verifiedStillMissingPaths = Array.Empty<string>();
            public ScenePartialSyncTool.SceneSyncExecutionReport syncReport;
            public bool success;
            public string message = string.Empty;
            public string artifactPath = string.Empty;
        }

        [Serializable]
        private sealed class ScratchCopyInfo
        {
            public string scenePath = string.Empty;
            public bool usedLoadedSceneCopy;
            public bool sceneWasDirty;
        }

        [MenuItem("Tools/Sunset/Scene/运行Primary backup候选scratch dry-run（桥+植被）")]
        private static void RunDryRun()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning("[ScenePrimaryBackupScratchDryRun] 当前 Unity 仍在 PlayMode 或忙碌中，已阻断 dry-run。");
                return;
            }

            DryRunReport report = new DryRunReport
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                selectedPaths = SyncPaths,
                guardMissingPaths = GuardMissingPaths
            };

            try
            {
                PrepareScratchDirectory();

                ScratchCopyInfo sourceCopy = CreateScratchSceneCopy(BackupPrimaryPath, ScratchSourceScenePath);
                ScratchCopyInfo targetCopy = CreateScratchSceneCopy(CurrentPrimaryPath, ScratchTargetScenePath);

                report.actualSourceScenePath = sourceCopy.scenePath;
                report.actualTargetScenePath = targetCopy.scenePath;
                report.usedLoadedSourceCopy = sourceCopy.usedLoadedSceneCopy;
                report.sourceWasDirty = sourceCopy.sceneWasDirty;
                report.usedLoadedTargetCopy = targetCopy.usedLoadedSceneCopy;
                report.targetWasDirty = targetCopy.sceneWasDirty;

                report.syncReport = ScenePartialSyncTool.ExecuteSceneSync(
                    report.actualSourceScenePath,
                    report.actualTargetScenePath,
                    SyncPaths,
                    ScenePartialSyncTool.SyncMode.CopyMissingOnly,
                    saveTargetScene: true,
                    requireConfirmation: false);

                report.verifiedPresentPaths = FilterExistingPaths(report.actualTargetScenePath, SyncPaths);
                report.verifiedStillMissingPaths = FilterMissingPaths(report.actualTargetScenePath, GuardMissingPaths);

                bool allSelectedPresent = report.verifiedPresentPaths.Length == SyncPaths.Length;
                bool allGuardsStillMissing = report.verifiedStillMissingPaths.Length == GuardMissingPaths.Length;

                report.success = report.syncReport.Success && allSelectedPresent && allGuardsStillMissing;
                report.message = report.success
                    ? "Primary backup scratch dry-run 已通过：桥与植被成功进入 scratch target，农田与基础地皮分组未被误带入。"
                    : "Primary backup scratch dry-run 未完全通过，请查看报告。";
            }
            catch (Exception exception)
            {
                report.message = $"Primary backup scratch dry-run 异常：{exception.GetType().Name}: {exception.Message}";
            }

            report.artifactPath = WriteArtifact(report);
            Debug.Log($"[ScenePrimaryBackupScratchDryRun] {report.message}\nArtifact: {report.artifactPath}");
        }

        private static ScratchCopyInfo CreateScratchSceneCopy(string originalScenePath, string scratchScenePath)
        {
            if (!File.Exists(ToAbsolutePath(originalScenePath)))
            {
                throw new FileNotFoundException($"场景文件不存在：{originalScenePath}", originalScenePath);
            }

            CloseScratchSceneIfLoaded(scratchScenePath);
            DeleteScratchAssetIfExists(scratchScenePath);

            Scene loadedScene = SceneManager.GetSceneByPath(originalScenePath);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                bool sceneWasDirty = loadedScene.isDirty;
                if (!EditorSceneManager.SaveScene(loadedScene, scratchScenePath, true))
                {
                    throw new InvalidOperationException($"无法保存 scratch 场景副本：{originalScenePath} -> {scratchScenePath}");
                }

                AssetDatabase.ImportAsset(scratchScenePath, ImportAssetOptions.ForceUpdate);
                return new ScratchCopyInfo
                {
                    scenePath = scratchScenePath,
                    usedLoadedSceneCopy = true,
                    sceneWasDirty = sceneWasDirty
                };
            }

            CopySceneAsset(originalScenePath, scratchScenePath);
            return new ScratchCopyInfo
            {
                scenePath = scratchScenePath,
                usedLoadedSceneCopy = false,
                sceneWasDirty = false
            };
        }

        private static string[] FilterExistingPaths(string scenePath, string[] paths)
        {
            return FilterPathsByExistence(scenePath, paths, shouldExist: true);
        }

        private static string[] FilterMissingPaths(string scenePath, string[] paths)
        {
            return FilterPathsByExistence(scenePath, paths, shouldExist: false);
        }

        private static string[] FilterPathsByExistence(string scenePath, string[] paths, bool shouldExist)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            try
            {
                System.Collections.Generic.List<string> result = new();
                foreach (string path in paths)
                {
                    bool exists = TryFindTransformByPath(scene, path, out _);
                    if (exists == shouldExist)
                    {
                        result.Add(path);
                    }
                }

                return result.ToArray();
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
            GameObject[] rootObjects = scene.GetRootGameObjects();
            GameObject rootObject = Array.Find(rootObjects, root => root.name == segments[0]);
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

        private static void PrepareScratchDirectory()
        {
            string fullDirectory = ToAbsolutePath(ScratchSceneDirectory);
            Directory.CreateDirectory(fullDirectory);
            AssetDatabase.Refresh();
        }

        private static void CloseScratchSceneIfLoaded(string scratchScenePath)
        {
            Scene scene = SceneManager.GetSceneByPath(scratchScenePath);
            if (scene.IsValid() && scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void DeleteScratchAssetIfExists(string scratchScenePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scratchScenePath) != null)
            {
                AssetDatabase.DeleteAsset(scratchScenePath);
            }

            string absoluteScenePath = ToAbsolutePath(scratchScenePath);
            string absoluteMetaPath = absoluteScenePath + ".meta";
            if (File.Exists(absoluteScenePath))
            {
                File.Delete(absoluteScenePath);
            }

            if (File.Exists(absoluteMetaPath))
            {
                File.Delete(absoluteMetaPath);
            }
        }

        private static void CopySceneAsset(string sourceProjectRelativePath, string targetProjectRelativePath)
        {
            string sourceFullPath = ToAbsolutePath(sourceProjectRelativePath);
            string targetFullPath = ToAbsolutePath(targetProjectRelativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath) ?? Directory.GetCurrentDirectory());
            File.Copy(sourceFullPath, targetFullPath, true);

            string sourceMetaPath = sourceFullPath + ".meta";
            string targetMetaPath = targetFullPath + ".meta";
            if (File.Exists(sourceMetaPath))
            {
                File.Copy(sourceMetaPath, targetMetaPath, true);
            }

            AssetDatabase.ImportAsset(targetProjectRelativePath, ImportAssetOptions.ForceUpdate);
        }

        private static string ToAbsolutePath(string projectRelativePath)
        {
            string projectRoot = Directory.GetCurrentDirectory();
            return Path.Combine(projectRoot, projectRelativePath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string WriteArtifact(DryRunReport report)
        {
            string directory = ToAbsolutePath(ArtifactDirectory);
            Directory.CreateDirectory(directory);

            string fullPath = Path.Combine(directory, $"{report.timestamp}_primary-backup-bridge-vegetation-dry-run.json");
            File.WriteAllText(fullPath, JsonUtility.ToJson(report, true));
            return fullPath;
        }
    }
}
