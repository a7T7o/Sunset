using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Tool_004_BatchTreeState : EditorWindow
{
    private const string PrimaryMenuPath = "Tools/004批量 (Tree状态)";
    private const string LegacyMenuPath = "Tools/Sunset/Tree/批量树状态工具";
    private const string WindowTitle = "004批量-Tree状态";
    private const float HeaderSpacing = 8f;
    private const float ButtonIndent = 22f;

    private readonly List<GameObject> selectedRoots = new List<GameObject>();
    private readonly List<TreeController> selectedTrees = new List<TreeController>();

    private bool autoRefreshSelection = true;
    private bool includeInactiveChildren = true;
    private bool showSelectionPreview = true;

    private bool applyTreeId;
    private bool applyStage = true;
    private bool applyState = true;
    private bool applySeason = true;
    private bool applyAutoGrow;

    private int targetTreeId = -1;
    private int targetStage;
    private TreeState targetState = TreeState.Normal;
    private SeasonManager.Season targetSeason = SeasonManager.Season.Spring;
    private bool targetAutoGrow = true;

    private Vector2 scrollPosition;
    private GUIStyle selectedButtonStyle;

    [MenuItem(PrimaryMenuPath)]
    [MenuItem(LegacyMenuPath)]
    public static void ShowWindow()
    {
        OpenWindow();
    }

    public static void OpenWindow()
    {
        Tool_004_BatchTreeState window = GetWindow<Tool_004_BatchTreeState>(WindowTitle);
        window.minSize = new Vector2(460f, 700f);
        window.RefreshSelection();
        window.Show();
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
        RefreshSelection();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        if (!autoRefreshSelection)
        {
            return;
        }

        RefreshSelection();
        Repaint();
    }

    private void OnGUI()
    {
        PruneMissingSelections();
        EnsureStyles();

        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;

            EditorGUILayout.LabelField("批量树状态工具", EditorStyles.boldLabel);

            DrawSelectionControls();
            EditorGUILayout.Space(HeaderSpacing);
            DrawSelectionSummary();
            EditorGUILayout.Space(HeaderSpacing);
            DrawBatchStateSection();
            EditorGUILayout.Space(HeaderSpacing);
            DrawApplySection();
        }
    }

    private void DrawSelectionControls()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("选择源", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        autoRefreshSelection = EditorGUILayout.Toggle("自动跟随当前选择", autoRefreshSelection);
        includeInactiveChildren = EditorGUILayout.Toggle("包含未激活子物体", includeInactiveChildren);
        if (EditorGUI.EndChangeCheck() && autoRefreshSelection)
        {
            RefreshSelection();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("刷新当前选择", GUILayout.Height(26f)))
        {
            RefreshSelection();
            Repaint();
        }

        GUI.enabled = selectedTrees.Count > 0;
        if (GUILayout.Button("从首棵树读入当前值", GUILayout.Height(26f)))
        {
            PullValuesFromFirstTree();
            Repaint();
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawSelectionSummary()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("当前命中", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("选中父物体", selectedRoots.Count.ToString());
            EditorGUILayout.LabelField("命中的 TreeController", selectedTrees.Count.ToString());

            if (selectedTrees.Count == 0)
            {
                return;
            }

            TreeController[] previewTrees = selectedTrees.Where(tree => tree != null).Take(12).ToArray();

            showSelectionPreview = EditorGUILayout.Foldout(showSelectionPreview, "预览命中的树", true);
            if (showSelectionPreview)
            {
                EditorGUI.indentLevel++;

                foreach (TreeController tree in previewTrees)
                {
                    if (tree == null)
                    {
                        continue;
                    }

                    string label = $"{GetHierarchyPath(tree.transform)}  |  Stage {tree.GetCurrentStageIndex()}  |  {tree.GetCurrentState()}  |  {tree.GetCurrentSeason()}  |  {GetAutoGrowLabel(GetAutoGrowValue(tree))}";
                    EditorGUILayout.ObjectField(label, tree, typeof(TreeController), true);
                }

                if (selectedTrees.Count > 12)
                {
                    EditorGUILayout.LabelField($"... 还有 {selectedTrees.Count - 12} 棵");
                }

                EditorGUI.indentLevel--;
            }
        }
    }

    private void DrawBatchStateSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("—— 当前状态 ——", EditorStyles.boldLabel);
        EditorGUILayout.Space(4f);

        DrawToggleHeader(ref applyTreeId, "树木ID", targetTreeId.ToString());
        GUI.enabled = applyTreeId;
        DrawButtonRows(GetTreeIdButtonValues(), targetTreeId, value => value.ToString(), value => targetTreeId = value, 6);
        DrawIntStepperRow(ref targetTreeId, -1, allowResetToMinusOne: true);
        GUI.enabled = true;

        DrawToggleHeader(ref applyStage, "当前阶段", targetStage.ToString());
        GUI.enabled = applyStage;
        if (applyStage)
        {
            DrawButtonRows(Enumerable.Range(0, 6).ToArray(), targetStage, value => value.ToString(), value => targetStage = value, 6);
        }
        GUI.enabled = true;

        DrawToggleHeader(ref applyState, "当前状态", GetTreeStateLabel(targetState));
        GUI.enabled = applyState;
        DrawButtonRows(
            (TreeState[])Enum.GetValues(typeof(TreeState)),
            targetState,
            GetTreeStateLabel,
            value => targetState = value,
            3);
        GUI.enabled = true;

        DrawToggleHeader(ref applySeason, "当前季节", GetSeasonLabel(targetSeason));
        GUI.enabled = applySeason;
        DrawButtonRows(
            (SeasonManager.Season[])Enum.GetValues(typeof(SeasonManager.Season)),
            targetSeason,
            GetSeasonLabel,
            value => targetSeason = value,
            4);
        GUI.enabled = true;

        DrawToggleHeader(ref applyAutoGrow, "是否生长", GetAutoGrowLabel(targetAutoGrow));
        GUI.enabled = applyAutoGrow;
        DrawButtonRows(
            new[] { true, false },
            targetAutoGrow,
            GetAutoGrowLabel,
            value => targetAutoGrow = value,
            2);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private void DrawApplySection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("应用", EditorStyles.boldLabel);

        bool hasAnyChangeToggle = applyTreeId || applyStage || applyState || applySeason || applyAutoGrow;
        GUI.enabled = selectedTrees.Count > 0 && hasAnyChangeToggle;

        if (GUILayout.Button("应用到当前选中的所有树", GUILayout.Height(34f)))
        {
            ApplyBatchState();
        }

        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private void DrawToggleHeader(ref bool toggle, string label, string currentValue)
    {
        EditorGUILayout.BeginHorizontal();
        toggle = EditorGUILayout.Toggle(toggle, GUILayout.Width(18f));
        EditorGUILayout.LabelField(label, GUILayout.Width(72f));
        EditorGUILayout.LabelField(currentValue, EditorStyles.miniBoldLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawIntStepperRow(ref int value, int minValue, bool allowResetToMinusOne)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(ButtonIndent);

        if (GUILayout.Button("-10"))
        {
            value = Mathf.Max(minValue, value - 10);
        }

        if (GUILayout.Button("-1"))
        {
            value = Mathf.Max(minValue, value - 1);
        }

        if (GUILayout.Button("+1"))
        {
            value += 1;
        }

        if (GUILayout.Button("+10"))
        {
            value += 10;
        }

        if (allowResetToMinusOne && GUILayout.Button("重置-1"))
        {
            value = -1;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawButtonRows<T>(IReadOnlyList<T> values, T currentValue, Func<T, string> getLabel, Action<T> onSelected, int buttonsPerRow)
    {
        if (values == null || values.Count == 0)
        {
            return;
        }

        int safeButtonsPerRow = Mathf.Max(1, buttonsPerRow);
        for (int start = 0; start < values.Count; start += safeButtonsPerRow)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(ButtonIndent);

            int end = Mathf.Min(values.Count, start + safeButtonsPerRow);
            for (int i = start; i < end; i++)
            {
                T candidate = values[i];
                bool isSelected = EqualityComparer<T>.Default.Equals(currentValue, candidate);
                GUIStyle style = isSelected ? selectedButtonStyle : GUI.skin.button;
                Color previousBackground = GUI.backgroundColor;
                Color previousContent = GUI.contentColor;
                if (isSelected)
                {
                    GUI.backgroundColor = new Color(0.24f, 0.43f, 0.72f, 1f);
                    GUI.contentColor = Color.white;
                }

                if (GUILayout.Button(getLabel(candidate), style, GUILayout.Height(24f)))
                {
                    if (!isSelected)
                    {
                        onSelected(candidate);
                        GUI.changed = true;
                        Repaint();
                    }
                }

                GUI.backgroundColor = previousBackground;
                GUI.contentColor = previousContent;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private int[] GetTreeIdButtonValues()
    {
        PruneMissingSelections();
        HashSet<int> values = new HashSet<int> { -1, 0, 1, 2, 3, 4, targetTreeId };
        foreach (TreeController tree in selectedTrees.Take(12))
        {
            if (tree == null)
            {
                continue;
            }

            SerializedObject serializedObject = new SerializedObject(tree);
            SerializedProperty treeIdProperty = serializedObject.FindProperty("treeID");
            if (treeIdProperty != null)
            {
                values.Add(treeIdProperty.intValue);
            }
        }

        return values.OrderBy(value => value).ToArray();
    }

    private static string GetTreeStateLabel(TreeState state)
    {
        return state switch
        {
            TreeState.Normal => "标准",
            TreeState.Withered => "枯萎",
            TreeState.Frozen => "冰封",
            TreeState.Melted => "融冰",
            TreeState.Stump => "树桩",
            _ => state.ToString()
        };
    }

    private static string GetSeasonLabel(SeasonManager.Season season)
    {
        return season switch
        {
            SeasonManager.Season.Spring => "春",
            SeasonManager.Season.Summer => "夏",
            SeasonManager.Season.Autumn => "秋",
            SeasonManager.Season.Winter => "冬",
            _ => season.ToString()
        };
    }

    private static string GetAutoGrowLabel(bool autoGrow)
    {
        return autoGrow ? "可生长" : "不生长";
    }

    private void RefreshSelection()
    {
        selectedRoots.Clear();
        selectedTrees.Clear();

        GameObject[] roots = Selection.gameObjects;
        if (roots == null || roots.Length == 0)
        {
            return;
        }

        HashSet<TreeController> uniqueTrees = new HashSet<TreeController>();

        foreach (GameObject root in roots)
        {
            if (root == null)
            {
                continue;
            }

            if (EditorUtility.IsPersistent(root))
            {
                continue;
            }

            if (!selectedRoots.Contains(root))
            {
                selectedRoots.Add(root);
            }

            TreeController[] trees = root.GetComponentsInChildren<TreeController>(includeInactiveChildren);
            foreach (TreeController tree in trees)
            {
                if (tree != null && uniqueTrees.Add(tree))
                {
                    selectedTrees.Add(tree);
                }
            }
        }

        selectedTrees.Sort((left, right) => string.CompareOrdinal(GetHierarchyPath(left.transform), GetHierarchyPath(right.transform)));
    }

    private void PullValuesFromFirstTree()
    {
        PruneMissingSelections();
        if (selectedTrees.Count == 0)
        {
            return;
        }

        TreeController tree = selectedTrees[0];
        SerializedObject serializedObject = new SerializedObject(tree);

        SerializedProperty treeIdProperty = serializedObject.FindProperty("treeID");
        if (treeIdProperty != null)
        {
            targetTreeId = treeIdProperty.intValue;
        }

        targetStage = tree.GetCurrentStageIndex();
        targetState = tree.GetCurrentState();
        targetSeason = tree.GetCurrentSeason();
        targetAutoGrow = GetAutoGrowValue(tree);
    }

    private void ApplyBatchState()
    {
        PruneMissingSelections();
        if (selectedTrees.Count == 0)
        {
            return;
        }

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("批量修改树状态");

        int changedCount = 0;
        foreach (TreeController tree in selectedTrees)
        {
            if (tree == null)
            {
                continue;
            }

            Undo.RegisterFullObjectHierarchyUndo(tree.gameObject, "批量修改树状态");
            tree.ApplyBatchEditorState(
                applyTreeId,
                targetTreeId,
                applyStage,
                targetStage,
                applyState,
                targetState,
                applySeason,
                targetSeason,
                applyAutoGrow,
                targetAutoGrow);

            EditorUtility.SetDirty(tree);
            EditorSceneManager.MarkSceneDirty(tree.gameObject.scene);
            changedCount++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        SceneView.RepaintAll();
        RefreshSelection();
        Repaint();
    }

    private void PruneMissingSelections()
    {
        selectedRoots.RemoveAll(root => root == null);
        selectedTrees.RemoveAll(tree => tree == null);
    }

    private void EnsureStyles()
    {
        if (selectedButtonStyle != null)
        {
            return;
        }

        selectedButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold
        };
        selectedButtonStyle.normal.textColor = Color.white;
        selectedButtonStyle.hover.textColor = Color.white;
        selectedButtonStyle.active.textColor = Color.white;
        selectedButtonStyle.focused.textColor = Color.white;
    }

    private static bool GetAutoGrowValue(TreeController tree)
    {
        if (tree == null)
        {
            return true;
        }

        SerializedObject serializedObject = new SerializedObject(tree);
        SerializedProperty autoGrowProperty = serializedObject.FindProperty("autoGrow");
        return autoGrowProperty == null || autoGrowProperty.boolValue;
    }

    private static string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return "<null>";
        }

        Stack<string> path = new Stack<string>();
        Transform current = target;
        while (current != null)
        {
            path.Push(current.name);
            current = current.parent;
        }

        return string.Join("/", path);
    }
}
