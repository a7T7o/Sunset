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
    [InitializeOnLoad]
    internal static class TownFoundationBootstrapMenu
    {
        private const string SourceScenePath = "Assets/000_Scenes/Primary.unity";
        private const string TargetScenePath = "Assets/000_Scenes/Town.unity";
        private const string ArtifactDirectory = ".codex/artifacts/town-foundation";
        private const string ScratchSceneDirectory = "Assets/__CodexSceneSyncScratch";
        private const string ScratchSourceScenePath = ScratchSceneDirectory + "/TownBootstrapSource.unity";
        private const string OneShotBootstrapSignalPath = "Library/CodexEditorCommands/town-bootstrap.once";

        private static readonly string[] ExcludedRootNames =
        {
            "SCENE",
            "NavGrid2DStressTest",
            "NPCs"
        };

        private static readonly string[] ExcludedPathPrefixes =
        {
            "Primary/3_Debug"
        };

        static TownFoundationBootstrapMenu()
        {
            EditorApplication.update -= TryRunOneShotBootstrap;
            EditorApplication.update += TryRunOneShotBootstrap;
        }

        [Serializable]
        private sealed class TownFoundationReport
        {
            public string timestamp = string.Empty;
            public string sourceScenePath = SourceScenePath;
            public string actualSourceScenePath = SourceScenePath;
            public string targetScenePath = TargetScenePath;
            public bool usedSourceSceneCopy;
            public bool sourceSceneWasDirty;
            public string[] excludedRootNames = Array.Empty<string>();
            public string[] excludedPathPrefixes = Array.Empty<string>();
            public string[] requestedPaths = Array.Empty<string>();
            public string[] verifiedPaths = Array.Empty<string>();
            public ScenePartialSyncTool.SceneSyncExecutionReport syncReport;
            public bool success;
            public string message = string.Empty;
            public string artifactPath = string.Empty;
        }

        [MenuItem("Tools/Sunset/Scene/Town基础骨架增量补齐（只增不删）")]
        private static void BootstrapTownFoundation()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                Debug.LogWarning("[TownFoundationBootstrap] 当前 Unity 仍在 PlayMode 或忙碌中，已阻断 Town 基础骨架补齐。");
                return;
            }

            TownFoundationReport report = new TownFoundationReport
            {
                timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                excludedRootNames = ExcludedRootNames,
                excludedPathPrefixes = ExcludedPathPrefixes
            };

            try
            {
                report.actualSourceScenePath = PrepareSourceSceneForBootstrap(out bool usedSourceSceneCopy, out bool sourceSceneWasDirty);
                report.usedSourceSceneCopy = usedSourceSceneCopy;
                report.sourceSceneWasDirty = sourceSceneWasDirty;
                report.requestedPaths = BuildRuntimeSyncPaths(report.actualSourceScenePath, TargetScenePath);

                if (report.requestedPaths.Length == 0)
                {
                    report.syncReport = new ScenePartialSyncTool.SceneSyncExecutionReport
                    {
                        SourceScenePath = report.actualSourceScenePath,
                        TargetScenePath = TargetScenePath,
                        Mode = "只复制目标缺失项",
                        RequestedPaths = Array.Empty<string>(),
                        EffectivePaths = Array.Empty<string>(),
                        SavedTarget = false,
                        Success = true,
                        Message = "Town 当前已经具备非 SCENE 运行骨架，不需要新增同步。",
                        StatusType = MessageType.Info
                    };
                    report.verifiedPaths = Array.Empty<string>();
                    report.success = true;
                    report.message = report.syncReport.Message;
                }
                else
                {
                    report.syncReport = ScenePartialSyncTool.ExecuteSceneSync(
                        report.actualSourceScenePath,
                        TargetScenePath,
                        report.requestedPaths,
                        ScenePartialSyncTool.SyncMode.CopyMissingOnly,
                        saveTargetScene: true,
                        requireConfirmation: false);

                    report.verifiedPaths = report.requestedPaths;
                    report.success = report.syncReport.Success && VerifyTownFoundation(report.verifiedPaths);
                    report.message = report.success
                        ? "Town 非 SCENE 运行骨架已按只增不删口径补齐。"
                        : "Town 非 SCENE 运行骨架补齐未完全通过，请查看报告。";
                }
            }
            catch (Exception exception)
            {
                report.message = $"Town 基础骨架补齐异常：{exception.GetType().Name}: {exception.Message}";
            }

            report.artifactPath = WriteArtifact(report);
            Debug.Log($"[TownFoundationBootstrap] {report.message}\nArtifact: {report.artifactPath}");
        }

        private static void TryRunOneShotBootstrap()
        {
            string signalPath = Path.Combine(Directory.GetCurrentDirectory(), OneShotBootstrapSignalPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(signalPath))
            {
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            File.Delete(signalPath);
            BootstrapTownFoundation();
        }

        private static string PrepareSourceSceneForBootstrap(out bool usedSourceSceneCopy, out bool sourceSceneWasDirty)
        {
            usedSourceSceneCopy = false;
            sourceSceneWasDirty = false;

            Scene sourceScene = SceneManager.GetSceneByPath(SourceScenePath);
            if (!(sourceScene.IsValid() && sourceScene.isLoaded))
            {
                return SourceScenePath;
            }

            PrepareScratchDirectory();
            DeleteScratchAssetIfExists(ScratchSourceScenePath);

            sourceSceneWasDirty = sourceScene.isDirty;
            if (!EditorSceneManager.SaveScene(sourceScene, ScratchSourceScenePath, true))
            {
                throw new InvalidOperationException($"无法保存 Town bootstrap 源场景副本：{SourceScenePath} -> {ScratchSourceScenePath}");
            }

            AssetDatabase.ImportAsset(ScratchSourceScenePath, ImportAssetOptions.ForceUpdate);
            usedSourceSceneCopy = true;
            return ScratchSourceScenePath;
        }

        private static string[] BuildRuntimeSyncPaths(string sourceScenePath, string targetScenePath)
        {
            if (!OpenSceneContext.TryOpen(sourceScenePath, targetScenePath, out OpenSceneContext context, out string errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            try
            {
                List<string> sourcePaths = CollectTransformPaths(context.SourceScene).ToList();
                HashSet<string> targetPathSet = new HashSet<string>(CollectTransformPaths(context.TargetScene), StringComparer.Ordinal);

                IEnumerable<string> missingRuntimePaths = sourcePaths.Where(path =>
                    !IsExcludedPath(path) &&
                    !targetPathSet.Contains(path));

                List<string> normalized = ScenePartialSyncTool.NormalizeSelectedPaths(missingRuntimePaths);
                List<string> blockers = ScenePartialSyncTool.FindMissingParentBlockers(normalized, targetPathSet);
                if (blockers.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Town 基础骨架自动补齐仍遇到父路径阻断：\n- " + string.Join("\n- ", blockers));
                }

                return normalized.ToArray();
            }
            finally
            {
                context.Dispose();
            }
        }

        private static bool IsExcludedPath(string path)
        {
            foreach (string rootName in ExcludedRootNames)
            {
                if (string.Equals(path, rootName, StringComparison.Ordinal) ||
                    path.StartsWith(rootName + "/", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            foreach (string pathPrefix in ExcludedPathPrefixes)
            {
                if (string.Equals(path, pathPrefix, StringComparison.Ordinal) ||
                    path.StartsWith(pathPrefix + "/", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> CollectTransformPaths(Scene scene)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                foreach (string path in EnumerateTransformPaths(rootObject.transform))
                {
                    yield return path;
                }
            }
        }

        private static IEnumerable<string> EnumerateTransformPaths(Transform transform)
        {
            yield return GetTransformPath(transform);

            for (int index = 0; index < transform.childCount; index++)
            {
                foreach (string childPath in EnumerateTransformPaths(transform.GetChild(index)))
                {
                    yield return childPath;
                }
            }
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

        private static void PrepareScratchDirectory()
        {
            string directory = Path.Combine(
                Directory.GetCurrentDirectory(),
                ScratchSceneDirectory.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        private static void DeleteScratchAssetIfExists(string projectRelativePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(projectRelativePath) != null)
            {
                AssetDatabase.DeleteAsset(projectRelativePath);
            }

            string absolutePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                projectRelativePath.Replace('/', Path.DirectorySeparatorChar));
            string absoluteMetaPath = absolutePath + ".meta";
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            if (File.Exists(absoluteMetaPath))
            {
                File.Delete(absoluteMetaPath);
            }
        }

        private static bool VerifyTownFoundation(IEnumerable<string> requiredPaths)
        {
            Scene townScene = EditorSceneManager.OpenScene(TargetScenePath, OpenSceneMode.Additive);
            try
            {
                foreach (string path in requiredPaths)
                {
                    if (!TryFindTransformByPath(townScene, path, out _))
                    {
                        return false;
                    }
                }

                return true;
            }
            finally
            {
                EditorSceneManager.CloseScene(townScene, true);
            }
        }

        private static Transform FindRootTransform(Scene scene, string rootName)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == rootName)
                {
                    return rootObject.transform;
                }
            }

            return null;
        }

        private static bool TryFindTransformByPath(Scene scene, string path, out Transform result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string[] segments = path.Split('/');
            Transform current = FindRootTransform(scene, segments[0]);
            if (current == null)
            {
                return false;
            }

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

        private static string WriteArtifact(TownFoundationReport report)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), ArtifactDirectory.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);

            string fullPath = Path.Combine(directory, $"{report.timestamp}_town-foundation.json");
            File.WriteAllText(fullPath, JsonUtility.ToJson(report, true));
            return fullPath;
        }

        private sealed class OpenSceneContext : IDisposable
        {
            public Scene SourceScene { get; private set; }
            public Scene TargetScene { get; private set; }

            private readonly List<Scene> openedByTool = new List<Scene>();

            public static bool TryOpen(string sourceScenePath, string targetScenePath, out OpenSceneContext context, out string errorMessage)
            {
                context = new OpenSceneContext();
                errorMessage = string.Empty;

                if (!TryGetOrOpenScene(sourceScenePath, context.openedByTool, out Scene sourceScene, out errorMessage) ||
                    !TryGetOrOpenScene(targetScenePath, context.openedByTool, out Scene targetScene, out errorMessage))
                {
                    context.Dispose();
                    context = null;
                    return false;
                }

                context.SourceScene = sourceScene;
                context.TargetScene = targetScene;
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

            private static bool TryGetOrOpenScene(
                string scenePath,
                List<Scene> openedByTool,
                out Scene scene,
                out string errorMessage)
            {
                scene = SceneManager.GetSceneByPath(scenePath);
                if (scene.IsValid() && scene.isLoaded)
                {
                    errorMessage = string.Empty;
                    return true;
                }

                try
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    openedByTool.Add(scene);
                    errorMessage = string.Empty;
                    return true;
                }
                catch (Exception exception)
                {
                    errorMessage = $"打开场景失败：{scenePath}\n{exception.Message}";
                    return false;
                }
            }
        }
    }
}
