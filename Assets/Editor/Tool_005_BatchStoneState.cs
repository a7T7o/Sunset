using System.Collections.Generic;
using System.Linq;
using System;
using FarmGame.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Tool_005_BatchStoneState : EditorWindow
{
    private const string PrimaryMenuPath = "Tools/005批量 (Stone状态)";
    private const string LegacyMenuPath = "Tools/Sunset/Stone/批量石头状态工具";
    private const string WindowTitle = "005批量-Stone状态";
    private const float HeaderSpacing = 8f;
    private const float ButtonIndent = 22f;

    private readonly List<GameObject> selectedRoots = new List<GameObject>();
    private readonly List<StoneController> selectedStones = new List<StoneController>();

    private bool autoRefreshSelection = true;
    private bool includeInactiveChildren = true;
    private bool showSelectionPreview = true;

    private bool applyStage = true;
    private bool applyOreType = true;
    private bool applyOreIndex = true;
    private bool applyHealth;

    private StoneStage targetStage = StoneStage.M1;
    private OreType targetOreType = OreType.C1;
    private int targetOreIndex;
    private int targetHealth = 36;

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
        Tool_005_BatchStoneState window = GetWindow<Tool_005_BatchStoneState>(WindowTitle);
        window.minSize = new Vector2(500f, 660f);
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

            EditorGUILayout.LabelField("批量石头状态工具", EditorStyles.boldLabel);

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

        GUI.enabled = selectedStones.Count > 0;
        if (GUILayout.Button("从首块石头读入当前值", GUILayout.Height(26f)))
        {
            PullValuesFromFirstStone();
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
            EditorGUILayout.LabelField("命中的 StoneController", selectedStones.Count.ToString());

            if (selectedStones.Count == 0)
            {
                return;
            }

            StoneController[] previewStones = selectedStones.Where(stone => stone != null).Take(12).ToArray();

            showSelectionPreview = EditorGUILayout.Foldout(showSelectionPreview, "预览命中的石头", true);
            if (showSelectionPreview)
            {
                EditorGUI.indentLevel++;

                foreach (StoneController stone in previewStones)
                {
                    if (stone == null)
                    {
                        continue;
                    }

                    string label =
                        $"{GetHierarchyPath(stone.transform)}  |  {stone.GetCurrentStage()}  |  {stone.GetOreType()}  |  OreIndex {stone.GetOreIndex()}  |  HP {stone.GetCurrentHealth()}";
                    EditorGUILayout.ObjectField(label, stone, typeof(StoneController), true);
                }

                if (selectedStones.Count > 12)
                {
                    EditorGUILayout.LabelField($"... 还有 {selectedStones.Count - 12} 块");
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

        DrawToggleHeader(ref applyStage, "当前阶段", GetStoneStageLabel(targetStage));
        GUI.enabled = applyStage;
        DrawButtonRows(
            (StoneStage[])Enum.GetValues(typeof(StoneStage)),
            targetStage,
            GetStoneStageLabel,
            value => targetStage = value,
            4);
        GUI.enabled = true;

        DrawToggleHeader(ref applyOreType, "矿物类型", targetOreType.ToString());
        GUI.enabled = applyOreType;
        DrawButtonRows(
            (OreType[])Enum.GetValues(typeof(OreType)),
            targetOreType,
            oreType => oreType.ToString(),
            value => targetOreType = value,
            4);
        GUI.enabled = true;

        DrawToggleHeader(ref applyOreIndex, "含量指数", targetOreIndex.ToString());
        GUI.enabled = applyOreIndex;
        int oreIndexMax = GetDisplayOreIndexMax();
        DrawButtonRows(
            Enumerable.Range(0, oreIndexMax + 1).ToArray(),
            targetOreIndex,
            value => value.ToString(),
            value => targetOreIndex = value,
            6);
        GUI.enabled = true;

        DrawToggleHeader(ref applyHealth, "当前血量", targetHealth.ToString());
        GUI.enabled = applyHealth;
        DrawButtonRows(GetStoneHealthButtonValues(), targetHealth, value => value.ToString(), value => targetHealth = value, 5);
        DrawIntStepperRow(ref targetHealth, 1);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private void DrawApplySection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("应用", EditorStyles.boldLabel);

        bool hasAnyChangeToggle = applyStage || applyOreType || applyOreIndex || applyHealth;
        GUI.enabled = selectedStones.Count > 0 && hasAnyChangeToggle;

        if (GUILayout.Button("应用到当前选中的所有石头", GUILayout.Height(34f)))
        {
            ApplyBatchState();
        }

        GUI.enabled = true;
        EditorGUILayout.EndVertical();
    }

    private void RefreshSelection()
    {
        selectedRoots.Clear();
        selectedStones.Clear();

        GameObject[] roots = Selection.gameObjects;
        if (roots == null || roots.Length == 0)
        {
            return;
        }

        HashSet<StoneController> uniqueStones = new HashSet<StoneController>();

        foreach (GameObject root in roots)
        {
            if (root == null || EditorUtility.IsPersistent(root))
            {
                continue;
            }

            if (!selectedRoots.Contains(root))
            {
                selectedRoots.Add(root);
            }

            StoneController[] stones = root.GetComponentsInChildren<StoneController>(includeInactiveChildren);
            foreach (StoneController stone in stones)
            {
                if (stone != null && uniqueStones.Add(stone))
                {
                    selectedStones.Add(stone);
                }
            }
        }

        selectedStones.Sort((left, right) => string.CompareOrdinal(GetHierarchyPath(left.transform), GetHierarchyPath(right.transform)));
    }

    private void PullValuesFromFirstStone()
    {
        PruneMissingSelections();
        if (selectedStones.Count == 0)
        {
            return;
        }

        StoneController stone = selectedStones[0];
        targetStage = stone.GetCurrentStage();
        targetOreType = stone.GetOreType();
        targetOreIndex = stone.GetOreIndex();
        targetHealth = stone.GetCurrentHealth();
    }

    private void ApplyBatchState()
    {
        PruneMissingSelections();
        if (selectedStones.Count == 0)
        {
            return;
        }

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("批量修改石头状态");

        int changedCount = 0;
        foreach (StoneController stone in selectedStones)
        {
            if (stone == null)
            {
                continue;
            }

            Undo.RegisterFullObjectHierarchyUndo(stone.gameObject, "批量修改石头状态");
            stone.ApplyBatchEditorState(
                applyStage,
                targetStage,
                applyOreType,
                targetOreType,
                applyOreIndex,
                targetOreIndex,
                applyHealth,
                targetHealth);

            EditorUtility.SetDirty(stone);
            EditorSceneManager.MarkSceneDirty(stone.gameObject.scene);
            changedCount++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        SceneView.RepaintAll();
        RefreshSelection();
        Repaint();
    }

    private int GetDisplayOreIndexMax()
    {
        PruneMissingSelections();
        StoneStage referenceStage = applyStage
            ? targetStage
            : selectedStones.Count > 0 ? selectedStones[0].GetCurrentStage() : StoneStage.M1;

        return referenceStage switch
        {
            StoneStage.M1 => 4,
            StoneStage.M2 => 4,
            StoneStage.M3 => 3,
            StoneStage.M4 => 7,
            _ => 4
        };
    }

    private void DrawToggleHeader(ref bool toggle, string label, string currentValue)
    {
        EditorGUILayout.BeginHorizontal();
        toggle = EditorGUILayout.Toggle(toggle, GUILayout.Width(18f));
        EditorGUILayout.LabelField(label, GUILayout.Width(72f));
        EditorGUILayout.LabelField(currentValue, EditorStyles.miniBoldLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawIntStepperRow(ref int value, int minValue)
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

    private int[] GetStoneHealthButtonValues()
    {
        PruneMissingSelections();
        HashSet<int> values = new HashSet<int> { 1, 4, 9, 17, 36, Mathf.Max(1, targetHealth) };
        foreach (StoneController stone in selectedStones.Take(12))
        {
            if (stone != null)
            {
                values.Add(Mathf.Max(1, stone.GetCurrentHealth()));
            }
        }

        return values.OrderBy(value => value).ToArray();
    }

    private void PruneMissingSelections()
    {
        selectedRoots.RemoveAll(root => root == null);
        selectedStones.RemoveAll(stone => stone == null);
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

    private static string GetStoneStageLabel(StoneStage stage)
    {
        return stage switch
        {
            StoneStage.M1 => "M1 最大",
            StoneStage.M2 => "M2 中等",
            StoneStage.M3 => "M3 最小",
            StoneStage.M4 => "M4 装饰",
            _ => stage.ToString()
        };
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
