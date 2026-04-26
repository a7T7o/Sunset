using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    public sealed class ScenePartialSyncTool : EditorWindow
    {
        public enum SyncMode
        {
            CopyMissingOnly = 0,
            OverwriteByPath = 1
        }

        [Serializable]
        public sealed class SceneSyncExecutionReport
        {
            public string SourceScenePath = string.Empty;
            public string TargetScenePath = string.Empty;
            public string Mode = string.Empty;
            public string[] RequestedPaths = Array.Empty<string>();
            public string[] EffectivePaths = Array.Empty<string>();
            public string[] DuplicateSourcePaths = Array.Empty<string>();
            public string[] DuplicateTargetPaths = Array.Empty<string>();
            public string[] MissingParentBlockers = Array.Empty<string>();
            public int CreatedCount;
            public int OverwrittenCount;
            public int SkippedCount;
            public bool SavedTarget;
            public bool Success;
            public string Message = string.Empty;
            public MessageType StatusType = MessageType.Info;
        }

        [Serializable]
        private sealed class PathEntry
        {
            public string Path;
            public bool Selected;
            public bool ExistsInTarget;
            public int Depth;
        }

        private const string WindowTitle = "Scene Partial Sync";

        [SerializeField] private SceneAsset sourceSceneAsset;
        [SerializeField] private SceneAsset targetSceneAsset;
        [SerializeField] private SyncMode syncMode = SyncMode.CopyMissingOnly;
        [SerializeField] private bool showOnlyMissingInTarget;
        [SerializeField] private string searchText = string.Empty;

        private readonly List<PathEntry> pathEntries = new();
        private readonly List<string> duplicateSourcePaths = new();
        private readonly List<string> duplicateTargetPaths = new();
        private Vector2 scrollPosition;
        private string statusMessage = "请选择源场景和目标场景，然后先点“刷新对象路径”。";
        private MessageType statusType = MessageType.Info;

        [MenuItem("Tools/Sunset/Scene/局部同步工具")]
        private static void ShowWindow()
        {
            ScenePartialSyncTool window = GetWindow<ScenePartialSyncTool>(WindowTitle);
            window.minSize = new Vector2(760f, 520f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Scene Partial Sync Tool", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "安全版第一刀：按对象路径把源场景中的根对象或子树，同步到目标场景。支持“只复制缺失”和“按路径覆盖”。\n\n注意：这不是 YAML 文本合并器。第一版不会自动重映射外部场景引用，也不会做字段级局部 merge；最稳的用法是同步相对独立的对象子树。",
                MessageType.Info);

            sourceSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("源场景", sourceSceneAsset, typeof(SceneAsset), false);
            targetSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("目标场景", targetSceneAsset, typeof(SceneAsset), false);
            syncMode = (SyncMode)EditorGUILayout.EnumPopup("同步模式", syncMode);
            showOnlyMissingInTarget = EditorGUILayout.Toggle("只看目标缺失项", showOnlyMissingInTarget);
            searchText = EditorGUILayout.TextField("路径筛选", searchText ?? string.Empty);

            DrawToolbar();
            DrawStatus();
            DrawRiskSummary();
            DrawPathList();
            DrawExecutionArea();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("刷新对象路径", GUILayout.Height(28f)))
                {
                    RefreshPreview();
                }

                using (new EditorGUI.DisabledScope(pathEntries.Count == 0))
                {
                    if (GUILayout.Button("全选当前可见项", GUILayout.Height(28f)))
                    {
                        foreach (PathEntry entry in GetVisibleEntries())
                        {
                            entry.Selected = true;
                        }
                    }

                    if (GUILayout.Button("清空选择", GUILayout.Height(28f)))
                    {
                        foreach (PathEntry entry in pathEntries)
                        {
                            entry.Selected = false;
                        }
                    }
                }
            }
        }

        private void DrawStatus()
        {
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }

        private void DrawRiskSummary()
        {
            if (syncMode == SyncMode.OverwriteByPath)
            {
                EditorGUILayout.HelpBox(
                    "按路径覆盖 = 目标场景同路径对象会被整棵子树替换。其他对象如果原来引用了被替换对象，本工具第一版不会替你自动修补这些外部场景引用。",
                    MessageType.Warning);
            }

            if (duplicateSourcePaths.Count > 0 || duplicateTargetPaths.Count > 0)
            {
                string duplicateMessage =
                    "检测到路径重名，当前版本会直接阻断执行：\n"
                    + (duplicateSourcePaths.Count > 0
                        ? "源场景重名路径：\n- " + string.Join("\n- ", duplicateSourcePaths) + "\n"
                        : string.Empty)
                    + (duplicateTargetPaths.Count > 0
                        ? "目标场景重名路径：\n- " + string.Join("\n- ", duplicateTargetPaths)
                        : string.Empty);

                EditorGUILayout.HelpBox(duplicateMessage.TrimEnd(), MessageType.Error);
            }
        }

        private void DrawPathList()
        {
            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField($"源场景对象路径（共 {pathEntries.Count} 项）", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.MinHeight(260f)))
                {
                    scrollPosition = scroll.scrollPosition;

                    IReadOnlyList<PathEntry> visibleEntries = GetVisibleEntries();
                    if (visibleEntries.Count == 0)
                    {
                        EditorGUILayout.LabelField("当前没有可显示的对象路径。先刷新，或调整筛选条件。", EditorStyles.miniLabel);
                        return;
                    }

                    foreach (PathEntry entry in visibleEntries)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            entry.Selected = EditorGUILayout.Toggle(entry.Selected, GUILayout.Width(18f));

                            string prefix = entry.ExistsInTarget ? "[已存在]" : "[缺失]";
                            string label = $"{prefix} {entry.Path}";
                            GUIStyle style = entry.ExistsInTarget ? EditorStyles.label : EditorStyles.boldLabel;

                            GUILayout.Space(Mathf.Max(0, entry.Depth - 1) * 14f);
                            EditorGUILayout.LabelField(label, style);
                        }
                    }
                }
            }
        }

        private void DrawExecutionArea()
        {
            EditorGUILayout.Space(8f);

            List<string> selectedPaths = NormalizeSelectedPaths(pathEntries.Where(entry => entry.Selected).Select(entry => entry.Path));
            EditorGUILayout.LabelField($"实际将同步 {selectedPaths.Count} 条路径（已自动去掉祖先已选中的重复子项）", EditorStyles.miniBoldLabel);

            using (new EditorGUI.DisabledScope(!CanExecuteSync()))
            {
                if (GUILayout.Button("执行同步并保存目标场景", GUILayout.Height(34f)))
                {
                    ExecuteSync(selectedPaths);
                }
            }
        }

        private bool CanExecuteSync()
        {
            if (!TryResolveScenePaths(out string sourceScenePath, out string targetScenePath, out _))
            {
                return false;
            }

            return pathEntries.Any(entry => entry.Selected)
                   && !string.Equals(sourceScenePath, targetScenePath, StringComparison.OrdinalIgnoreCase);
        }

        private void RefreshPreview()
        {
            pathEntries.Clear();
            duplicateSourcePaths.Clear();
            duplicateTargetPaths.Clear();

            if (!TryResolveScenePaths(out string sourceScenePath, out string targetScenePath, out string errorMessage))
            {
                SetStatus(errorMessage, MessageType.Warning);
                return;
            }

            if (!LoadedSceneContext.TryOpen(sourceScenePath, targetScenePath, out LoadedSceneContext context, out errorMessage))
            {
                SetStatus(errorMessage, MessageType.Error);
                return;
            }

            try
            {
                List<(string path, int depth)> sourceEntries = CollectTransformPathEntries(context.SourceScene).ToList();
                List<string> targetPaths = CollectTransformPaths(context.TargetScene).ToList();
                duplicateSourcePaths.AddRange(FindDuplicatePaths(sourceEntries.Select(entry => entry.path)));
                duplicateTargetPaths.AddRange(FindDuplicatePaths(targetPaths));

                HashSet<string> targetPathSet = new HashSet<string>(targetPaths, StringComparer.Ordinal);
                foreach ((string path, int depth) in sourceEntries)
                {
                    pathEntries.Add(new PathEntry
                    {
                        Path = path,
                        Depth = depth,
                        ExistsInTarget = targetPathSet.Contains(path),
                        Selected = false
                    });
                }

                if (duplicateSourcePaths.Count > 0 || duplicateTargetPaths.Count > 0)
                {
                    SetStatus(
                        "已读取对象路径，但检测到路径重名。当前版本会先阻断执行，避免错同步。请先把重名层级整理开，或改成选择更高一层的唯一父节点。",
                        MessageType.Warning);
                }
                else
                {
                    SetStatus(
                        $"已读取源场景 {pathEntries.Count} 条对象路径。你可以先筛选，再勾选要同步的对象子树。",
                        MessageType.Info);
                }
            }
            finally
            {
                context.Dispose();
            }
        }

        private void ExecuteSync(List<string> selectedPaths)
        {
            if (!TryResolveScenePaths(out string sourceScenePath, out string targetScenePath, out string errorMessage))
            {
                SetStatus(errorMessage, MessageType.Warning);
                return;
            }

            if (selectedPaths.Count == 0)
            {
                SetStatus("还没有勾选任何同步路径。", MessageType.Warning);
                return;
            }

            SceneSyncExecutionReport report = ExecuteSceneSync(
                sourceScenePath,
                targetScenePath,
                selectedPaths,
                syncMode,
                saveTargetScene: true,
                requireConfirmation: true);

            SetStatus(report.Message, report.StatusType);

            RefreshPreview();
        }

        public static SceneSyncExecutionReport ExecuteSceneSync(
            string sourceScenePath,
            string targetScenePath,
            IEnumerable<string> selectedPaths,
            SyncMode mode,
            bool saveTargetScene = true,
            bool requireConfirmation = false)
        {
            SceneSyncExecutionReport report = new SceneSyncExecutionReport
            {
                SourceScenePath = sourceScenePath ?? string.Empty,
                TargetScenePath = targetScenePath ?? string.Empty,
                Mode = GetModeLabel(mode),
                RequestedPaths = selectedPaths?
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .ToArray() ?? Array.Empty<string>()
            };

            List<string> effectivePaths = NormalizeSelectedPaths(report.RequestedPaths);
            report.EffectivePaths = effectivePaths.ToArray();

            if (string.IsNullOrWhiteSpace(sourceScenePath) || string.IsNullOrWhiteSpace(targetScenePath))
            {
                report.Message = "源场景和目标场景都要先选好。";
                report.StatusType = MessageType.Warning;
                return report;
            }

            if (string.Equals(sourceScenePath, targetScenePath, StringComparison.OrdinalIgnoreCase))
            {
                report.Message = "源场景和目标场景不能是同一个文件。";
                report.StatusType = MessageType.Warning;
                return report;
            }

            if (effectivePaths.Count == 0)
            {
                report.Message = "还没有选中任何有效同步路径。";
                report.StatusType = MessageType.Warning;
                return report;
            }

            if (!LoadedSceneContext.TryOpen(sourceScenePath, targetScenePath, out LoadedSceneContext context, out string errorMessage))
            {
                report.Message = errorMessage;
                report.StatusType = MessageType.Error;
                return report;
            }

            try
            {
                if (context.SourceWasAlreadyLoaded && context.SourceScene.isDirty)
                {
                    report.Message = "源场景当前已在编辑器中打开且存在未保存修改。为避免把不稳定现场当成同步源，当前已阻断，请先保存或改用副本。";
                    report.StatusType = MessageType.Error;
                    return report;
                }

                if (context.TargetWasAlreadyLoaded && context.TargetScene.isDirty)
                {
                    report.Message = "目标场景当前已在编辑器中打开且存在未保存修改。为避免覆盖现场，当前已阻断，请先保存或改用副本。";
                    report.StatusType = MessageType.Error;
                    return report;
                }

                List<(string path, int depth)> sourceEntries = CollectTransformPathEntries(context.SourceScene).ToList();
                List<string> targetPaths = CollectTransformPaths(context.TargetScene).ToList();
                report.DuplicateSourcePaths = FilterDuplicatePathsForSelectedLookups(
                    FindDuplicatePaths(sourceEntries.Select(entry => entry.path)),
                    effectivePaths).ToArray();
                report.DuplicateTargetPaths = FilterDuplicatePathsForSelectedLookups(
                    FindDuplicatePaths(targetPaths),
                    effectivePaths).ToArray();

                if (report.DuplicateSourcePaths.Length > 0 || report.DuplicateTargetPaths.Length > 0)
                {
                    report.Message = "当前存在路径重名，已阻断同步。请先处理重名层级，再执行。";
                    report.StatusType = MessageType.Error;
                    return report;
                }

                report.MissingParentBlockers = FindMissingParentBlockers(effectivePaths, targetPaths).ToArray();
                if (report.MissingParentBlockers.Length > 0)
                {
                    report.Message =
                        "目标场景缺少这些父路径，当前版本无法把所选子树放到正确位置：\n- "
                        + string.Join("\n- ", report.MissingParentBlockers);
                    report.StatusType = MessageType.Error;
                    return report;
                }

                if (requireConfirmation)
                {
                    string confirmMessage =
                        $"即将把 {effectivePaths.Count} 条路径从\n{sourceScenePath}\n同步到\n{targetScenePath}\n\n模式：{GetModeLabel(mode)}\n\n目标场景会被保存。是否继续？";
                    if (!EditorUtility.DisplayDialog("确认 Scene Partial Sync", confirmMessage, "继续", "取消"))
                    {
                        report.Message = "已取消同步。";
                        report.StatusType = MessageType.Info;
                        return report;
                    }
                }

                Undo.IncrementCurrentGroup();
                int undoGroup = Undo.GetCurrentGroup();
                Undo.SetCurrentGroupName("Scene Partial Sync");

                foreach (string path in effectivePaths)
                {
                    if (!TryFindTransformByPath(context.SourceScene, path, out Transform sourceTransform))
                    {
                        report.SkippedCount++;
                        continue;
                    }

                    bool targetExists = TryFindTransformByPath(context.TargetScene, path, out Transform targetTransform);
                    if (targetExists && mode == SyncMode.CopyMissingOnly)
                    {
                        report.SkippedCount++;
                        continue;
                    }

                    Transform targetParent = null;
                    int siblingIndex = sourceTransform.GetSiblingIndex();

                    if (targetExists)
                    {
                        targetParent = targetTransform.parent;
                        siblingIndex = targetTransform.GetSiblingIndex();
                        Undo.DestroyObjectImmediate(targetTransform.gameObject);
                        report.OverwrittenCount++;
                    }
                    else
                    {
                        string parentPath = GetParentPath(path);
                        if (!string.IsNullOrEmpty(parentPath))
                        {
                            TryFindTransformByPath(context.TargetScene, parentPath, out targetParent);
                        }

                        report.CreatedCount++;
                    }

                    GameObject clone = InstantiateForTargetScene(sourceTransform, targetParent, context.TargetScene, siblingIndex);
                    Undo.RegisterCreatedObjectUndo(clone, "Scene Partial Sync Create");
                }

                Undo.CollapseUndoOperations(undoGroup);
                EditorSceneManager.MarkSceneDirty(context.TargetScene);

                if (saveTargetScene)
                {
                    if (!EditorSceneManager.SaveScene(context.TargetScene))
                    {
                        report.Message = "目标场景保存失败，已停止。";
                        report.StatusType = MessageType.Error;
                        return report;
                    }

                    report.SavedTarget = true;
                }

                report.Success = true;
                report.StatusType = MessageType.Info;
                report.Message =
                    $"同步完成：新增 {report.CreatedCount}，覆盖 {report.OverwrittenCount}，跳过 {report.SkippedCount}。"
                    + (saveTargetScene
                        ? " 目标场景已保存。"
                        : " 当前未自动保存目标场景。")
                    + "\n如果这轮同步的是带外部场景引用的对象，请务必再打开目标场景做一次引用核验。";
                return report;
            }
            catch (Exception exception)
            {
                report.Message = $"同步异常：{exception.GetType().Name}: {exception.Message}";
                report.StatusType = MessageType.Error;
                return report;
            }
            finally
            {
                context.Dispose();
            }
        }

        private IReadOnlyList<PathEntry> GetVisibleEntries()
        {
            IEnumerable<PathEntry> query = pathEntries;
            if (showOnlyMissingInTarget)
            {
                query = query.Where(entry => !entry.ExistsInTarget);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(entry => entry.Path.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return query.ToList();
        }

        private bool TryResolveScenePaths(out string sourceScenePath, out string targetScenePath, out string errorMessage)
        {
            sourceScenePath = sourceSceneAsset != null ? AssetDatabase.GetAssetPath(sourceSceneAsset) : string.Empty;
            targetScenePath = targetSceneAsset != null ? AssetDatabase.GetAssetPath(targetSceneAsset) : string.Empty;

            if (string.IsNullOrWhiteSpace(sourceScenePath) || string.IsNullOrWhiteSpace(targetScenePath))
            {
                errorMessage = "源场景和目标场景都要先选好。";
                return false;
            }

            if (string.Equals(sourceScenePath, targetScenePath, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "源场景和目标场景不能是同一个文件。";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void SetStatus(string message, MessageType type)
        {
            statusMessage = message;
            statusType = type;
            Repaint();
        }

        private static string GetModeLabel(SyncMode mode)
        {
            return mode switch
            {
                SyncMode.CopyMissingOnly => "只复制目标缺失项",
                SyncMode.OverwriteByPath => "按路径覆盖目标对象",
                _ => mode.ToString()
            };
        }

        private static GameObject InstantiateForTargetScene(Transform sourceTransform, Transform targetParent, Scene targetScene, int siblingIndex)
        {
            GameObject clone = Instantiate(sourceTransform.gameObject);
            clone.name = sourceTransform.name;

            SceneManager.MoveGameObjectToScene(clone, targetScene);
            Transform cloneTransform = clone.transform;
            if (targetParent != null)
            {
                cloneTransform.SetParent(targetParent, false);
            }

            CopyTransformState(sourceTransform, cloneTransform);
            cloneTransform.SetSiblingIndex(Mathf.Max(0, siblingIndex));
            return clone;
        }

        private static void CopyTransformState(Transform source, Transform target)
        {
            if (source is RectTransform sourceRect && target is RectTransform targetRect)
            {
                targetRect.anchorMin = sourceRect.anchorMin;
                targetRect.anchorMax = sourceRect.anchorMax;
                targetRect.anchoredPosition3D = sourceRect.anchoredPosition3D;
                targetRect.sizeDelta = sourceRect.sizeDelta;
                targetRect.pivot = sourceRect.pivot;
                targetRect.localRotation = sourceRect.localRotation;
                targetRect.localScale = sourceRect.localScale;
                return;
            }

            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private static IEnumerable<(string path, int depth)> CollectTransformPathEntries(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach ((string path, int depth) in EnumerateTransformPaths(root.transform))
                {
                    yield return (path, depth);
                }
            }
        }

        private static IEnumerable<string> CollectTransformPaths(Scene scene)
        {
            return CollectTransformPathEntries(scene).Select(entry => entry.path);
        }

        private static List<string> FindDuplicatePaths(IEnumerable<string> paths)
        {
            return paths
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .GroupBy(path => path, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToList();
        }

        private static List<string> FilterDuplicatePathsForSelectedLookups(
            IEnumerable<string> duplicatePaths,
            IEnumerable<string> selectedPaths)
        {
            List<string> selected = selectedPaths?
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .ToList() ?? new List<string>();

            return duplicatePaths
                .Where(candidate => selected.Any(selection =>
                    string.Equals(candidate, selection, StringComparison.Ordinal)
                    || selection.StartsWith(candidate + "/", StringComparison.Ordinal)))
                .ToList();
        }

        private static IEnumerable<(string path, int depth)> EnumerateTransformPaths(Transform transform)
        {
            string path = GetTransformPath(transform);
            int depth = path.Count(character => character == '/') + 1;
            yield return (path, depth);

            for (int index = 0; index < transform.childCount; index++)
            {
                foreach ((string childPath, int childDepth) in EnumerateTransformPaths(transform.GetChild(index)))
                {
                    yield return (childPath, childDepth);
                }
            }
        }

        public static List<string> NormalizeSelectedPaths(IEnumerable<string> selectedPaths)
        {
            List<string> ordered = selectedPaths
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(path => path.Count(character => character == '/'))
                .ThenBy(path => path, StringComparer.Ordinal)
                .ToList();

            List<string> result = new();
            foreach (string path in ordered)
            {
                bool coveredByAncestor = result.Any(existing =>
                    path.Length > existing.Length
                    && path.StartsWith(existing, StringComparison.Ordinal)
                    && path[existing.Length] == '/');

                if (!coveredByAncestor)
                {
                    result.Add(path);
                }
            }

            return result;
        }

        public static List<string> FindMissingParentBlockers(IEnumerable<string> selectedPaths, IEnumerable<string> existingTargetPaths)
        {
            HashSet<string> targetPathSet = new HashSet<string>(existingTargetPaths ?? Array.Empty<string>(), StringComparer.Ordinal);
            List<string> blockers = new();

            foreach (string path in selectedPaths.Where(path => !string.IsNullOrWhiteSpace(path)))
            {
                if (targetPathSet.Contains(path))
                {
                    continue;
                }

                string parentPath = GetParentPath(path);
                if (string.IsNullOrEmpty(parentPath))
                {
                    continue;
                }

                if (!targetPathSet.Contains(parentPath))
                {
                    blockers.Add(path);
                }
            }

            return blockers;
        }

        public static string GetParentPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            int separatorIndex = path.LastIndexOf('/');
            return separatorIndex <= 0 ? string.Empty : path.Substring(0, separatorIndex);
        }

        private static string GetTransformPath(Transform transform)
        {
            Stack<string> names = new();
            Transform current = transform;
            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names);
        }

        private static bool TryFindTransformByPath(Scene scene, string path, out Transform result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string[] segments = path.Split('/');
            GameObject rootObject = scene.GetRootGameObjects().FirstOrDefault(root => root.name == segments[0]);
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

        private sealed class LoadedSceneContext : IDisposable
        {
            public Scene SourceScene { get; private set; }
            public Scene TargetScene { get; private set; }
            public bool SourceWasAlreadyLoaded { get; private set; }
            public bool TargetWasAlreadyLoaded { get; private set; }

            private readonly List<Scene> openedByTool = new();

            private LoadedSceneContext()
            {
            }

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
