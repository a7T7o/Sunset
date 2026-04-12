using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 002批量工具 - Hierarchy窗口专用
/// 整合：Order排序、Transform、碰撞器
/// V2.0: 智能Pivot换算 - 统一底部基点计算Order
/// </summary>
public class Tool_002_BatchHierarchy : EditorWindow
{
    internal const string WindowTitle = "002批量-Hierarchy";
    private const string PersistedSelectionIdsKey = "Batch002_SelectedGlobalObjectIds";
    private enum ToolMode { Order, Transform, 碰撞器 }
    private ToolMode currentMode = ToolMode.Order;
    private Vector2 scrollPos;
    private bool showLockedObjectList;
    private bool showOrderAdvanced = true;
    private bool showOrderNotes;
    
    private List<GameObject> selectedObjs = new List<GameObject>();

    [MenuItem("Tools/002批量 (Hierarchy窗口)")]
    public static void ShowWindow()
    {
        Tool_002_BatchHierarchy window = GetWindow<Tool_002_BatchHierarchy>(false, WindowTitle, true);
        window.titleContent = new GUIContent(WindowTitle);
        window.minSize = new Vector2(480, 650);
        window.Show();
        window.Focus();
    }

    private void OnEnable()
    {
        titleContent = new GUIContent(WindowTitle);
        currentMode = (ToolMode)EditorPrefs.GetInt("Batch002_Mode", 0);
        LoadSettings();
        
        // 只监听重绘，不再自动接管 Hierarchy 当前选择
        Selection.selectionChanged += OnSelectionChanged;
        
        // 初始加载上次确认并持久化的选择
        LoadPersistedSelection();
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt("Batch002_Mode", (int)currentMode);
        SaveSettings();
        SavePersistedSelection();
        
        // 取消监听
        Selection.selectionChanged -= OnSelectionChanged;
    }
    
    private void OnSelectionChanged()
    {
        Repaint();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawModeSwitch();
        DrawLine();
        DrawSelectionSummary();
        DrawLine();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        switch (currentMode)
        {
            case ToolMode.Order: DrawOrderMode(); break;
            case ToolMode.Transform: DrawTransformMode(); break;
            case ToolMode.碰撞器: DrawColliderMode(); break;
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void OnInspectorUpdate()
    {
        // 定期刷新，确保UI更新
        Repaint();
    }

    private void DrawHeader()
    {
        EditorGUILayout.LabelField("002批量-Hierarchy", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("先锁定对象，再按当前模式批量处理。", EditorStyles.miniLabel);
    }

    private void DrawModeSwitch()
    {
        EditorGUILayout.BeginHorizontal();

        ToolMode nextMode = (ToolMode)GUILayout.Toolbar(
            (int)currentMode,
            new[] { "Order", "Transform", "碰撞器" },
            GUILayout.Height(28));

        if (nextMode != currentMode)
        {
            currentMode = nextMode;
            EditorPrefs.SetInt("Batch002_Mode", (int)currentMode);
        }

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("恢复默认", GUILayout.Width(88), GUILayout.Height(28)))
        {
            if (EditorUtility.DisplayDialog("确认", $"恢复【{currentMode}】的默认设置？", "确定", "取消"))
            {
                ResetCurrentMode();
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSelectionSummary()
    {
        int hierarchySelectionCount = Selection.gameObjects != null ? Selection.gameObjects.Length : 0;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        string lockedSummary = selectedObjs.Count > 0
            ? $"已锁定 {selectedObjs.Count} 个"
            : "未锁定对象";
        EditorGUILayout.LabelField(
            $"{lockedSummary}  |  Hierarchy 当前选中 {hierarchySelectionCount} 个",
            EditorStyles.miniBoldLabel);

        if (GUILayout.Button("确认选取", GUILayout.Width(90)))
        {
            ConfirmCurrentHierarchySelection();
        }

        if (GUILayout.Button("清空", GUILayout.Width(56)))
        {
            ClearLockedSelection();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(
            "在 Hierarchy 选好对象后，点击“确认选取”才会替换当前锁定列表。",
            EditorStyles.wordWrappedMiniLabel);

        if (selectedObjs.Count > 0)
        {
            showLockedObjectList = EditorGUILayout.Foldout(showLockedObjectList, "查看锁定对象", true);
            if (showLockedObjectList)
            {
                int show = Mathf.Min(selectedObjs.Count, 6);
                for (int i = 0; i < show; i++)
                {
                    if (selectedObjs[i] != null)
                    {
                        EditorGUILayout.LabelField($"• {selectedObjs[i].name}", EditorStyles.miniLabel);
                    }
                }

                if (selectedObjs.Count > 6)
                {
                    EditorGUILayout.LabelField($"... 还有 {selectedObjs.Count - 6} 个", EditorStyles.miniLabel);
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
    }

    private void ConfirmCurrentHierarchySelection()
    {
        selectedObjs.Clear();
        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
        {
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject != null && !selectedObjs.Contains(selectedObject))
                {
                    selectedObjs.Add(selectedObject);
                }
            }
        }

        SavePersistedSelection();
        Repaint();
    }

    private void ClearLockedSelection()
    {
        selectedObjs.Clear();
        SavePersistedSelection();
        Repaint();
    }

    private void LoadPersistedSelection()
    {
        selectedObjs.Clear();

        string serializedIds = EditorPrefs.GetString(PersistedSelectionIdsKey, string.Empty);
        if (string.IsNullOrWhiteSpace(serializedIds))
        {
            return;
        }

        string[] lines = serializedIds.Split('\n');
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (!GlobalObjectId.TryParse(line, out GlobalObjectId globalObjectId))
            {
                continue;
            }

            GameObject restoredObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId) as GameObject;
            if (restoredObject != null && !selectedObjs.Contains(restoredObject))
            {
                selectedObjs.Add(restoredObject);
            }
        }

    }

    private void SavePersistedSelection()
    {
        if (selectedObjs.Count == 0)
        {
            EditorPrefs.DeleteKey(PersistedSelectionIdsKey);
            return;
        }

        StringBuilder builder = new StringBuilder();
        foreach (GameObject selectedObject in selectedObjs)
        {
            if (selectedObject == null)
            {
                continue;
            }

            GlobalObjectId globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(selectedObject);
            if (builder.Length > 0)
            {
                builder.Append('\n');
            }

            builder.Append(globalObjectId.ToString());
        }

        if (builder.Length == 0)
        {
            EditorPrefs.DeleteKey(PersistedSelectionIdsKey);
            return;
        }

        EditorPrefs.SetString(PersistedSelectionIdsKey, builder.ToString());
    }

    #region ========== Order排序模式 ==========

    // Sorting Layer 设置
    private bool sort_chk_layer = false;
    private string sort_layer = "Default";
    
    // 快速偏移
    private int sort_quickOffset = 1;
    
    // 按Y坐标计算Order参数
    private int sort_multiplier = 100;
    private int sort_orderOffset = 0;
    private bool sort_useSpriteBounds = true;
    private float sort_bottomOffset = 0f;
    private int sort_shadowOffset = -1;
    private int sort_glowOffset = 0;
    private bool sort_useBuildingMode = true;
    private int sort_buildingFrontOrderOffset = 12;
    private float sort_buildingFrontageLocalYOffset = 1f;

    private class BuildingSortContext
    {
        public bool IsActive;
        public GameObject RootObject;
        public SpriteRenderer BaseRenderer;
        public float BaseSortingY;
        public int BaseOrder;
        public float BaseLocalY;
        public string BaseSource;
    }

    private void DrawOrderMode()
    {
        EditorGUILayout.LabelField("核心规则", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(
            "优先按 Collider2D 底边算 Order；没有 Collider 时回退到 Sprite 底边。",
            EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("快速操作", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Order偏移:", GUILayout.Width(80));
        sort_quickOffset = EditorGUILayout.IntField(sort_quickOffset, GUILayout.Width(50));
        
        GUI.enabled = selectedObjs.Count > 0;
        if (GUILayout.Button("↑ +", GUILayout.Width(50)))
            QuickOffsetOrder(sort_quickOffset);
        if (GUILayout.Button("↓ -", GUILayout.Width(50)))
            QuickOffsetOrder(-sort_quickOffset);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        DrawLine();

        EditorGUILayout.LabelField("核心参数", EditorStyles.boldLabel);
        sort_multiplier = EditorGUILayout.IntField("Y坐标缩放倍数", sort_multiplier);
        sort_orderOffset = EditorGUILayout.IntField("Order偏移值", sort_orderOffset);
        sort_useSpriteBounds = EditorGUILayout.Toggle("优先用底边计算", sort_useSpriteBounds);
        sort_bottomOffset = EditorGUILayout.FloatField("底部偏移（世界单位）", sort_bottomOffset);
        EditorGUILayout.LabelField("推荐：倍数 100，偏移默认 0，需要整体上提时再调底部偏移。", EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space(2);
        showOrderAdvanced = EditorGUILayout.Foldout(showOrderAdvanced, "高级设置", true);
        if (showOrderAdvanced)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("子物体偏移", EditorStyles.boldLabel);
            sort_shadowOffset = EditorGUILayout.IntField("Shadow偏移值", sort_shadowOffset);
            sort_glowOffset = EditorGUILayout.IntField("Glow/特效偏移值", sort_glowOffset);

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("建筑模式", EditorStyles.boldLabel);
            sort_useBuildingMode = EditorGUILayout.Toggle("房子多片统一基线", sort_useBuildingMode);
            if (sort_useBuildingMode)
            {
                sort_buildingFrontOrderOffset = EditorGUILayout.IntField("前檐/前片偏移", sort_buildingFrontOrderOffset);
                sort_buildingFrontageLocalYOffset = EditorGUILayout.FloatField("前片局部Y阈值", sort_buildingFrontageLocalYOffset);
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Sorting Layer", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            sort_chk_layer = EditorGUILayout.Toggle(sort_chk_layer, GUILayout.Width(18));
            EditorGUI.BeginDisabledGroup(!sort_chk_layer);
            sort_layer = EditorGUILayout.TextField(sort_layer);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("勾选后执行时会同时设置 Sorting Layer。", EditorStyles.wordWrappedMiniLabel);

            EditorGUI.indentLevel--;
        }

        DrawLine();
        GUI.enabled = selectedObjs.Count > 0;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 1f);
        if (GUILayout.Button("🚀 设置选中物体的Order in Layer", GUILayout.Height(40)))
            SetOrderByY();
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
        
        EditorGUILayout.Space(5);
        
        GUI.enabled = selectedObjs.Count > 0;
        if (GUILayout.Button("📊 显示选中物体的当前Order", GUILayout.Height(30)))
            ShowCurrentOrders();
        GUI.enabled = true;

        EditorGUILayout.Space(6);
        showOrderNotes = EditorGUILayout.Foldout(showOrderNotes, "补充说明", true);
        if (showOrderNotes)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("• 公式：-Round(底部Y × 倍数) + Order偏移值", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.LabelField("• 只选父物体也可以，工具会递归处理子物体的 SpriteRenderer。", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.LabelField("• 正底部偏移 = 逻辑底边上移；树等高物体可从 0.2~0.5 开始试。", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();
        }
    }

    private void QuickOffsetOrder(int offset)
    {
        // 🔥 修复：包含所有子物体的SpriteRenderer
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        foreach (var obj in selectedObjs)
        {
            SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>(true);
            renderers.AddRange(srs);
        }
        
        if (renderers.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "选中对象及其子物体中没有SpriteRenderer", "确定");
            return;
        }
        
        Undo.RecordObjects(renderers.ToArray(), "Quick Offset Order");
        
        int skipped = 0;
        foreach (var sr in renderers)
        {
            // ✅ 跳过特殊标记的物体（Order < -9990）
            if (sr.sortingOrder < -9990)
            {
                skipped++;
                continue;
            }
            
            sr.sortingOrder += offset;
            EditorUtility.SetDirty(sr);
        }
        
        if (skipped > 0)
            Debug.Log($"<color=grey>[002批量] 跳过了 {skipped} 个特殊标记物体（Order < -9990）</color>");
        
        Debug.Log($"<color=green>[002批量] Order偏移 {offset:+0;-0}，共{renderers.Count}个对象（含子物体）</color>");
    }

    private void SetOrderByY()
    {
        if (selectedObjs.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先选择对象！", "确定");
            return;
        }
        
        int count = 0;
        List<SpriteRenderer> allRenderers = new List<SpriteRenderer>();

        foreach (GameObject obj in selectedObjs)
        {
            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
            allRenderers.AddRange(renderers);
        }

        if (allRenderers.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "选中对象及其子物体中没有SpriteRenderer！", "确定");
            return;
        }

        
        foreach (GameObject selectedRoot in selectedObjs)
        {
            SpriteRenderer[] renderers = selectedRoot.GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                continue;
            }

            BuildingSortContext buildingContext = CreateBuildingSortContext(selectedRoot, renderers);

            foreach (SpriteRenderer sr in renderers)
            {
                Undo.RecordObject(sr, "Set Order in Layer");

                if (sr.sortingOrder < -9990)
                {
                    Debug.Log($"<color=grey>[{GetGameObjectPath(sr.gameObject)}] Order={sr.sortingOrder} < -9990，跳过处理</color>");
                    continue;
                }

                float sortingY;
                string source;
                int calculatedOrder = CalculateOrderForRenderer(sr, buildingContext, out sortingY, out source);

                if (sr.gameObject.name.ToLower().Contains("shadow"))
                {
                    Transform effectParent = sr.transform.parent;
                    if (effectParent != null)
                    {
                        SpriteRenderer parentSr = effectParent.GetComponent<SpriteRenderer>();
                        if (parentSr != null)
                        {
                            float parentSortY;
                            string parentSource;
                            int parentOrder = CalculateOrderForRenderer(parentSr, buildingContext, out parentSortY, out parentSource);
                            calculatedOrder = parentOrder + sort_shadowOffset;
                            source = $"Parent({parentSource})+ShadowOffset";
                            sortingY = parentSortY;

                            Debug.Log($"  ↳ [Shadow: {sr.gameObject.name}] 父Order={parentOrder} → Shadow Order={calculatedOrder}");
                        }
                    }
                }
                else if (sr.gameObject.name.ToLower().Contains("glow") ||
                         sr.gameObject.name.ToLower().Contains("light") ||
                         sr.gameObject.name.ToLower().Contains("effect"))
                {
                    Transform effectParent = sr.transform.parent;
                    if (effectParent != null)
                    {
                        SpriteRenderer parentSr = effectParent.GetComponent<SpriteRenderer>();
                        if (parentSr != null)
                        {
                            float parentSortY;
                            string parentSource;
                            int parentOrder = CalculateOrderForRenderer(parentSr, buildingContext, out parentSortY, out parentSource);
                            calculatedOrder = parentOrder + sort_glowOffset;
                            source = $"Parent({parentSource})+GlowOffset";
                            sortingY = parentSortY;

                            Debug.Log($"  ↳ [Glow: {sr.gameObject.name}] 父Order={parentOrder} → Glow Order={calculatedOrder}");
                        }
                    }
                }

                if (sort_chk_layer)
                {
                    sr.sortingLayerName = sort_layer;
                }

                sr.sortingOrder = calculatedOrder;
                EditorUtility.SetDirty(sr);
                count++;

                string path = GetGameObjectPath(sr.gameObject);
                Collider2D col = sr.GetComponent<Collider2D>();
                Transform parent = sr.transform.parent;

                string debugInfo = $"[{path}]\n" +
                                  $"  Transform.Y = {sr.transform.position.y:F3}\n";

                if (col != null)
                    debugInfo += $"  Collider.min.y = {col.bounds.min.y:F3}\n";
                if (sr.sprite != null)
                    debugInfo += $"  Sprite.min.y = {sr.bounds.min.y:F3}\n";
                if (parent != null && parent.GetComponent<SpriteRenderer>() == null)
                    debugInfo += $"  Parent.Y = {parent.position.y:F3}\n";

                if (buildingContext.IsActive && buildingContext.BaseRenderer != null)
                {
                    debugInfo += $"  BuildingBase = {buildingContext.BaseRenderer.gameObject.name}, BaseOrder = {buildingContext.BaseOrder}\n";
                }

                debugInfo += $"  → 用{source}底部Y = {sortingY:F3}\n" +
                            $"  → 计算 = -Round({sortingY:F3} × {sort_multiplier}) + {sort_orderOffset}\n" +
                            $"  → Order = {calculatedOrder}";

                Debug.Log(debugInfo);
            }
        }
        
        string msg = $"已设置 {count} 个SpriteRenderer";
        if (sort_chk_layer)
            msg += $"\n• Sorting Layer: {sort_layer}";
        msg += "\n• Order: 自动计算（基于Collider底部）";
        
        EditorUtility.DisplayDialog("完成", msg, "确定");
        Debug.Log($"<color=green>[002批量] 设置完成！共处理 {count} 个对象{(sort_chk_layer ? $"，Layer={sort_layer}" : "")}</color>");
    }
    
    /// <summary>
    /// 计算排序用的Y坐标
    /// 核心：优先使用当前对象自己的Collider底部；只有无Collider时才回退双层结构父节点或Sprite底部
    /// </summary>
    private float CalculateSortingY(SpriteRenderer sr, Transform trans)
    {
        return CalculateDefaultSortingY(sr, trans, out _);
    }

    private float CalculateDefaultSortingY(SpriteRenderer sr, Transform trans, out string source)
    {
        Collider2D collider = sr.GetComponent<Collider2D>();

        if (collider != null && sort_useSpriteBounds)
        {
            source = "Collider";
            return collider.bounds.min.y + sort_bottomOffset;
        }

        Transform parent = trans.parent;
        if (parent != null)
        {
            SpriteRenderer parentSr = parent.GetComponent<SpriteRenderer>();
            if (parentSr == null)
            {
                source = "Parent";
                return parent.position.y + sort_bottomOffset;
            }
        }

        if (sort_useSpriteBounds && sr.sprite != null)
        {
            source = "Sprite";
            return sr.bounds.min.y + sort_bottomOffset;
        }

        source = "Transform";
        return trans.position.y + sort_bottomOffset;
    }

    private float CalculateDirectVisualSortingY(SpriteRenderer sr, Transform trans, out string source)
    {
        Collider2D collider = sr.GetComponent<Collider2D>();
        if (collider != null && sort_useSpriteBounds)
        {
            source = "Collider";
            return collider.bounds.min.y + sort_bottomOffset;
        }

        if (sort_useSpriteBounds && sr.sprite != null)
        {
            source = "Sprite";
            return sr.bounds.min.y + sort_bottomOffset;
        }

        source = "Transform";
        return trans.position.y + sort_bottomOffset;
    }

    private BuildingSortContext CreateBuildingSortContext(GameObject selectedRoot, SpriteRenderer[] renderers)
    {
        BuildingSortContext context = new BuildingSortContext { IsActive = false };

        if (!sort_useBuildingMode || !IsBuildingSelection(selectedRoot, renderers))
        {
            return context;
        }

        SpriteRenderer baseRenderer = FindPrimaryBuildingRenderer(renderers);
        if (baseRenderer == null)
        {
            return context;
        }

        string baseSource;
        float baseSortingY = CalculateDefaultSortingY(baseRenderer, baseRenderer.transform, out baseSource);

        context.IsActive = true;
        context.RootObject = selectedRoot;
        context.BaseRenderer = baseRenderer;
        context.BaseSortingY = baseSortingY;
        context.BaseOrder = -Mathf.RoundToInt(baseSortingY * sort_multiplier) + sort_orderOffset;
        context.BaseLocalY = baseRenderer.transform.localPosition.y;
        context.BaseSource = baseSource;
        return context;
    }

    private bool IsBuildingSelection(GameObject selectedRoot, SpriteRenderer[] renderers)
    {
        if (selectedRoot == null || renderers == null || renderers.Length <= 1)
        {
            return false;
        }

        string nameLower = selectedRoot.name.ToLowerInvariant();
        string tagValue = selectedRoot.tag;
        return nameLower.Contains("house") ||
               tagValue == "Building" ||
               tagValue == "Buildings";
    }

    private SpriteRenderer FindPrimaryBuildingRenderer(SpriteRenderer[] renderers)
    {
        SpriteRenderer bestRenderer = null;
        float bestScore = float.MinValue;

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null || sr.sortingOrder < -9990)
            {
                continue;
            }

            float score = GetRendererScore(sr);
            if (score > bestScore)
            {
                bestScore = score;
                bestRenderer = sr;
            }
        }

        return bestRenderer;
    }

    private float GetRendererScore(SpriteRenderer sr)
    {
        Collider2D collider = sr.GetComponent<Collider2D>();
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            return bounds.size.x * bounds.size.y;
        }

        if (sr.sprite != null)
        {
            Rect rect = sr.sprite.rect;
            return rect.width * rect.height;
        }

        Bounds rendererBounds = sr.bounds;
        return rendererBounds.size.x * rendererBounds.size.y;
    }

    private bool IsBuildingFrontageRenderer(SpriteRenderer sr, BuildingSortContext context)
    {
        if (!context.IsActive || sr == null || context.BaseRenderer == null || sr == context.BaseRenderer)
        {
            return false;
        }

        float localDelta = context.BaseLocalY - sr.transform.localPosition.y;
        return localDelta >= Mathf.Max(0.1f, sort_buildingFrontageLocalYOffset);
    }

    private int CalculateOrderForRenderer(SpriteRenderer sr, BuildingSortContext context, out float sortingY, out string source)
    {
        if (!context.IsActive)
        {
            sortingY = CalculateDefaultSortingY(sr, sr.transform, out source);
            return -Mathf.RoundToInt(sortingY * sort_multiplier) + sort_orderOffset;
        }

        if (sr == context.BaseRenderer)
        {
            sortingY = context.BaseSortingY;
            source = $"{context.BaseSource}[BuildingBase]";
            return context.BaseOrder;
        }

        if (IsBuildingFrontageRenderer(sr, context))
        {
            sortingY = CalculateDirectVisualSortingY(sr, sr.transform, out source);
            source = $"{source}[BuildingFrontage]";
            int ownOrder = -Mathf.RoundToInt(sortingY * sort_multiplier) + sort_orderOffset;
            return Mathf.Max(context.BaseOrder + sort_buildingFrontOrderOffset, ownOrder);
        }

        sortingY = context.BaseSortingY;
        source = "BuildingBaseInherited";
        return context.BaseOrder;
    }
    
    
    private void ShowCurrentOrders()
    {
        if (selectedObjs.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先选择对象！", "确定");
            return;
        }
        
        Debug.Log("========== 当前选中物体的Order信息 ==========");
        
        foreach (GameObject obj in selectedObjs)
        {
            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
            
            Debug.Log($"[{obj.name}] 包含 {renderers.Length} 个SpriteRenderer:");
            
            foreach (SpriteRenderer sr in renderers)
            {
                string path = GetGameObjectPath(sr.gameObject);
                Debug.Log($"  • {path}\n    Layer: {sr.sortingLayerName}, Order: {sr.sortingOrder}");
            }
        }
        
        Debug.Log("==========================================");
    }
    
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;
        
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }


    #endregion

    #region ========== Transform模式 ==========

    private bool tf_chk_pos = false;
    private bool tf_chk_rot = false;
    private bool tf_chk_scale = false;
    private bool tf_offset = false;
    
    private Vector3 tf_pos = Vector3.zero;
    private Vector3 tf_rot = Vector3.zero;
    private Vector3 tf_scale = Vector3.one;
    private float tf_quickY = 0.5f;

    private void DrawTransformMode()
    {
        EditorGUILayout.LabelField("⚡ 快速Y轴偏移", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("偏移值:", GUILayout.Width(60));
        tf_quickY = EditorGUILayout.FloatField(tf_quickY, GUILayout.Width(60));
        
        GUI.enabled = selectedObjs.Count > 0;
        if (GUILayout.Button("↑ 上移", GUILayout.Width(70)))
            QuickOffsetY(tf_quickY);
        if (GUILayout.Button("↓ 下移", GUILayout.Width(70)))
            QuickOffsetY(-tf_quickY);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        
        DrawLine();
        
        EditorGUILayout.LabelField("⚙️ 详细设置", EditorStyles.boldLabel);
        
        tf_offset = EditorGUILayout.ToggleLeft("偏移模式（非设置模式）", tf_offset);
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        tf_chk_pos = EditorGUILayout.Toggle(tf_chk_pos, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tf_chk_pos);
        tf_pos = EditorGUILayout.Vector3Field("Position", tf_pos);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        tf_chk_rot = EditorGUILayout.Toggle(tf_chk_rot, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tf_chk_rot);
        tf_rot = EditorGUILayout.Vector3Field("Rotation", tf_rot);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        tf_chk_scale = EditorGUILayout.Toggle(tf_chk_scale, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tf_chk_scale);
        tf_scale = EditorGUILayout.Vector3Field("Scale", tf_scale);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        DrawLine();
        
        GUI.enabled = selectedObjs.Count > 0;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("🚀 应用Transform设置", GUILayout.Height(40)))
            ApplyTransformSettings();
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
    }

    private void QuickOffsetY(float offset)
    {
        Undo.RecordObjects(selectedObjs.ToArray(), "Quick Offset Y");
        
        foreach (var obj in selectedObjs)
        {
            Vector3 pos = obj.transform.position;
            pos.y += offset;
            obj.transform.position = pos;
            EditorUtility.SetDirty(obj.transform);
        }
        
        Debug.Log($"<color=green>[002批量] Y轴偏移 {offset:+0.00;-0.00}，共{selectedObjs.Count}个对象</color>");
    }

    private void ApplyTransformSettings()
    {
        if (!tf_chk_pos && !tf_chk_rot && !tf_chk_scale)
        {
            EditorUtility.DisplayDialog("提示", "请至少勾选一个选项！", "确定");
            return;
        }
        
        Undo.RecordObjects(selectedObjs.ToArray(), "Apply Transform Settings");
        
        foreach (var obj in selectedObjs)
        {
            if (tf_chk_pos)
            {
                if (tf_offset)
                    obj.transform.position += tf_pos;
                else
                    obj.transform.position = tf_pos;
            }
            
            if (tf_chk_rot)
            {
                if (tf_offset)
                    obj.transform.eulerAngles += tf_rot;
                else
                    obj.transform.eulerAngles = tf_rot;
            }
            
            if (tf_chk_scale)
            {
                if (tf_offset)
                    obj.transform.localScale = Vector3.Scale(obj.transform.localScale, tf_scale);
                else
                    obj.transform.localScale = tf_scale;
            }
            
            EditorUtility.SetDirty(obj.transform);
        }
        
        Debug.Log($"<color=green>[002批量] Transform设置完成！{selectedObjs.Count}个对象</color>");
    }

    #endregion

    #region ========== 碰撞器模式 ==========

    private enum ColliderType { BoxCollider2D, CircleCollider2D, PolygonCollider2D }
    private ColliderType col_type = ColliderType.BoxCollider2D;
    private bool col_trigger = false;
    private bool col_addRb = false;
    
    private Vector2 col_boxSize = Vector2.one;
    private float col_circleRadius = 0.5f;

    private void DrawColliderMode()
    {
        EditorGUILayout.LabelField("⚡ 快速预设", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("角色碰撞器"))
        {
            col_type = ColliderType.BoxCollider2D;
            col_trigger = false;
            col_addRb = true;
            col_boxSize = new Vector2(0.8f, 1f);
        }
        if (GUILayout.Button("墙体碰撞器"))
        {
            col_type = ColliderType.BoxCollider2D;
            col_trigger = false;
            col_addRb = false;
            col_boxSize = Vector2.one;
        }
        if (GUILayout.Button("触发器"))
        {
            col_type = ColliderType.BoxCollider2D;
            col_trigger = true;
            col_addRb = false;
            col_boxSize = Vector2.one;
        }
        EditorGUILayout.EndHorizontal();
        
        DrawLine();
        
        EditorGUILayout.LabelField("⚙️ 详细设置", EditorStyles.boldLabel);
        
        col_type = (ColliderType)EditorGUILayout.EnumPopup("碰撞器类型", col_type);
        col_trigger = EditorGUILayout.Toggle("Is Trigger", col_trigger);
        col_addRb = EditorGUILayout.Toggle("添加Rigidbody2D", col_addRb);
        
        EditorGUILayout.Space();
        
        if (col_type == ColliderType.BoxCollider2D)
        {
            col_boxSize = EditorGUILayout.Vector2Field("Box Size", col_boxSize);
        }
        else if (col_type == ColliderType.CircleCollider2D)
        {
            col_circleRadius = EditorGUILayout.FloatField("Circle Radius", col_circleRadius);
        }
        
        DrawLine();
        
        GUI.enabled = selectedObjs.Count > 0;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("🚀 添加碰撞器", GUILayout.Height(40)))
            ApplyCollider();
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
    }

    private void ApplyCollider()
    {
        Undo.RecordObjects(selectedObjs.ToArray(), "Add Colliders");
        
        int count = 0;
        
        foreach (var obj in selectedObjs)
        {
            Collider2D collider = null;
            
            switch (col_type)
            {
                case ColliderType.BoxCollider2D:
                    var box = obj.GetComponent<BoxCollider2D>();
                    if (box == null) box = obj.AddComponent<BoxCollider2D>();
                    box.size = col_boxSize;
                    box.isTrigger = col_trigger;
                    collider = box;
                    break;
                    
                case ColliderType.CircleCollider2D:
                    var circle = obj.GetComponent<CircleCollider2D>();
                    if (circle == null) circle = obj.AddComponent<CircleCollider2D>();
                    circle.radius = col_circleRadius;
                    circle.isTrigger = col_trigger;
                    collider = circle;
                    break;
                    
                case ColliderType.PolygonCollider2D:
                    var poly = obj.GetComponent<PolygonCollider2D>();
                    if (poly == null) poly = obj.AddComponent<PolygonCollider2D>();
                    poly.isTrigger = col_trigger;
                    collider = poly;
                    break;
            }
            
            if (col_addRb)
            {
                var rb = obj.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = obj.AddComponent<Rigidbody2D>();
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
            
            if (collider != null)
            {
                EditorUtility.SetDirty(obj);
                count++;
            }
        }
        
        EditorUtility.DisplayDialog("完成", $"成功添加碰撞器：{count}个对象", "确定");
        Debug.Log($"<color=green>[002批量] 添加碰撞器完成！{count}个对象</color>");
    }

    #endregion

    #region ========== 设置保存/加载 ==========

    private void LoadSettings()
    {
        // 排序层设置
        sort_chk_layer = EditorPrefs.GetBool("Batch002_Sort_ChkLayer", false);
        sort_layer = EditorPrefs.GetString("Batch002_Sort_Layer", "Default");
        sort_quickOffset = EditorPrefs.GetInt("Batch002_Sort_QuickOffset", 1);
        
        // 排序层 - Y坐标计算
        sort_multiplier = EditorPrefs.GetInt("Batch002_Sort_Multiplier", 100);
        sort_orderOffset = EditorPrefs.GetInt("Batch002_Sort_OrderOffset", 0);
        sort_useSpriteBounds = EditorPrefs.GetBool("Batch002_Sort_UseSpriteBounds", true);
        sort_bottomOffset = EditorPrefs.GetFloat("Batch002_Sort_BottomOffset", 0f);
        sort_shadowOffset = EditorPrefs.GetInt("Batch002_Sort_ShadowOffset", -1);
        sort_glowOffset = EditorPrefs.GetInt("Batch002_Sort_GlowOffset", 0);
        
        // Transform
        tf_chk_pos = EditorPrefs.GetBool("Batch002_TF_ChkPos", false);
        tf_chk_rot = EditorPrefs.GetBool("Batch002_TF_ChkRot", false);
        tf_chk_scale = EditorPrefs.GetBool("Batch002_TF_ChkScale", false);
        tf_offset = EditorPrefs.GetBool("Batch002_TF_Offset", false);
        tf_quickY = EditorPrefs.GetFloat("Batch002_TF_QuickY", 0.5f);
        
        // 碰撞器
        col_type = (ColliderType)EditorPrefs.GetInt("Batch002_Col_Type", 0);
        col_trigger = EditorPrefs.GetBool("Batch002_Col_Trigger", false);
        col_addRb = EditorPrefs.GetBool("Batch002_Col_AddRb", false);
    }

    private void SaveSettings()
    {
        // 排序层设置
        EditorPrefs.SetBool("Batch002_Sort_ChkLayer", sort_chk_layer);
        EditorPrefs.SetString("Batch002_Sort_Layer", sort_layer);
        EditorPrefs.SetInt("Batch002_Sort_QuickOffset", sort_quickOffset);
        
        // 排序层 - Y坐标计算
        EditorPrefs.SetInt("Batch002_Sort_Multiplier", sort_multiplier);
        EditorPrefs.SetInt("Batch002_Sort_OrderOffset", sort_orderOffset);
        EditorPrefs.SetBool("Batch002_Sort_UseSpriteBounds", sort_useSpriteBounds);
        EditorPrefs.SetFloat("Batch002_Sort_BottomOffset", sort_bottomOffset);
        EditorPrefs.SetInt("Batch002_Sort_ShadowOffset", sort_shadowOffset);
        EditorPrefs.SetInt("Batch002_Sort_GlowOffset", sort_glowOffset);
        
        // Transform
        EditorPrefs.SetBool("Batch002_TF_ChkPos", tf_chk_pos);
        EditorPrefs.SetBool("Batch002_TF_ChkRot", tf_chk_rot);
        EditorPrefs.SetBool("Batch002_TF_ChkScale", tf_chk_scale);
        EditorPrefs.SetBool("Batch002_TF_Offset", tf_offset);
        EditorPrefs.SetFloat("Batch002_TF_QuickY", tf_quickY);
        
        // 碰撞器
        EditorPrefs.SetInt("Batch002_Col_Type", (int)col_type);
        EditorPrefs.SetBool("Batch002_Col_Trigger", col_trigger);
        EditorPrefs.SetBool("Batch002_Col_AddRb", col_addRb);
    }

    private void ResetCurrentMode()
    {
        switch (currentMode)
        {
            case ToolMode.Order:
                sort_chk_layer = false;
                sort_layer = "Default";
                sort_quickOffset = 1;
                sort_multiplier = 100;
                sort_orderOffset = 0;
                sort_useSpriteBounds = true;
                sort_bottomOffset = 0f;
                sort_shadowOffset = -1;
                sort_glowOffset = 0;
                break;
                
            case ToolMode.Transform:
                tf_chk_pos = false;
                tf_chk_rot = false;
                tf_chk_scale = false;
                tf_offset = false;
                tf_pos = Vector3.zero;
                tf_rot = Vector3.zero;
                tf_scale = Vector3.one;
                tf_quickY = 0.5f;
                break;
                
            case ToolMode.碰撞器:
                col_type = ColliderType.BoxCollider2D;
                col_trigger = false;
                col_addRb = false;
                col_boxSize = Vector2.one;
                col_circleRadius = 0.5f;
                break;
        }
        
        SaveSettings();
        Repaint();
    }

    #endregion
}

[InitializeOnLoad]
internal static class Tool002BatchHierarchyPlayModeGuard
{
    static Tool002BatchHierarchyPlayModeGuard()
    {
        // 当前已确认报错根因在 Unity 的布局缓存残留，而不是稳定可控的 EditorWindow 生命周期；
        // 这里先不做自动清理，避免在坏页签现场上再次引入新的 Editor 空引用。
    }
}
