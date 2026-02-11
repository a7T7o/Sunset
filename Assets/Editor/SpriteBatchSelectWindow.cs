using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Sprite 批量选择弹窗
/// 从 Texture 或文件夹中网格展示 Sprite 并支持勾选
/// 参考 ItemBatchSelectWindow 的交互模式
/// </summary>
public class SpriteBatchSelectWindow : EditorWindow
{
    #region 静态方法

    private static Action<List<Sprite>> onSpritesSelected;

    /// <summary>
    /// 打开批量选择弹窗
    /// </summary>
    /// <param name="callback">选中后的回调，传回选中的 Sprite 列表</param>
    public static void ShowWindow(Action<List<Sprite>> callback)
    {
        onSpritesSelected = callback;

        var window = GetWindow<SpriteBatchSelectWindow>(true, "批量选择 Sprite", true);
        window.minSize = new Vector2(520, 420);
        window.LoadSpritesFromSelection();
        window.Show();
    }

    #endregion

    #region 字段

    private List<Sprite> allSprites = new List<Sprite>();
    private HashSet<Sprite> selectedSprites = new HashSet<Sprite>();
    private string searchFilter = "";
    private Vector2 scrollPosition;

    private GUIStyle itemButtonStyle;
    private GUIStyle selectedButtonStyle;
    private bool stylesInitialized = false;

    #endregion

    #region 加载逻辑

    private void LoadSpritesFromSelection()
    {
        allSprites.Clear();
        selectedSprites.Clear();
        searchFilter = "";

        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                AddSpritesFromTexture(texture);
            }
            else if (obj is Sprite sprite)
            {
                if (!allSprites.Contains(sprite))
                    allSprites.Add(sprite);
            }
            else if (obj is DefaultAsset)
            {
                string folderPath = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(folderPath))
                {
                    AddSpritesFromFolder(folderPath);
                }
            }
        }

        allSprites.Sort((a, b) => NaturalCompare(a.name, b.name));
    }

    /// <summary>
    /// 自然排序比较：数字部分按数值大小排序，非数字部分按字典序
    /// 例如：All_0, All_1, All_2, ... All_9, All_10, All_11
    /// </summary>
    private static int NaturalCompare(string a, string b)
    {
        var partsA = Regex.Split(a, @"(\d+)");
        var partsB = Regex.Split(b, @"(\d+)");

        int len = Mathf.Min(partsA.Length, partsB.Length);
        for (int i = 0; i < len; i++)
        {
            // 两段都是纯数字 → 按数值比较
            bool aIsNum = int.TryParse(partsA[i], out int numA);
            bool bIsNum = int.TryParse(partsB[i], out int numB);

            if (aIsNum && bIsNum)
            {
                if (numA != numB) return numA.CompareTo(numB);
            }
            else
            {
                int cmp = string.Compare(partsA[i], partsB[i], StringComparison.OrdinalIgnoreCase);
                if (cmp != 0) return cmp;
            }
        }

        return partsA.Length.CompareTo(partsB.Length);
    }

    private void AddSpritesFromTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();
        foreach (var s in sprites)
        {
            if (!allSprites.Contains(s))
                allSprites.Add(s);
        }
    }

    private void AddSpritesFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>();
            foreach (var s in sprites)
            {
                if (!allSprites.Contains(s))
                    allSprites.Add(s);
            }
        }
    }

    #endregion

    #region 样式初始化

    private void InitStyles()
    {
        if (stylesInitialized) return;

        itemButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(4, 4, 2, 2),
            fixedHeight = 30
        };

        selectedButtonStyle = new GUIStyle(itemButtonStyle);
        selectedButtonStyle.normal.background = MakeColorTexture(new Color(0.3f, 0.5f, 0.3f, 1f));
        selectedButtonStyle.normal.textColor = Color.white;

        stylesInitialized = true;
    }

    private Texture2D MakeColorTexture(Color color)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }

    #endregion

    #region GUI

    private void OnGUI()
    {
        InitStyles();

        // 无 Sprite 时显示提示
        if (allSprites.Count == 0)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox(
                "未找到任何 Sprite。\n\n请先在 Project 窗口中选中一个 Texture 或文件夹，然后再点击「批量选择」按钮。",
                MessageType.Warning);

            EditorGUILayout.Space(10);
            if (GUILayout.Button("重新加载（从当前选中项）", GUILayout.Height(28)))
            {
                LoadSpritesFromSelection();
            }

            EditorGUILayout.Space(5);
            if (GUILayout.Button("关闭", GUILayout.Height(25)))
            {
                Close();
            }
            return;
        }

        DrawToolbar();
        EditorGUILayout.Space(3);
        DrawSpriteGrid();
        EditorGUILayout.Space(3);
        DrawBottomButtons();
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            // 搜索框
            GUILayout.Label("搜索:", GUILayout.Width(35));
            searchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(150));

            if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                searchFilter = "";
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();

            // 已选数量
            GUILayout.Label($"已选: {selectedSprites.Count}/{allSprites.Count}", EditorStyles.miniLabel);

            GUILayout.Space(5);

            // 全选
            if (GUILayout.Button("全选", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                SelectAllFiltered();
            }

            // 取消全选
            if (GUILayout.Button("取消", EditorStyles.toolbarButton, GUILayout.Width(40)))
            {
                selectedSprites.Clear();
            }
        }
    }

    private void DrawSpriteGrid()
    {
        var filtered = GetFilteredSprites();

        if (filtered.Count == 0)
        {
            EditorGUILayout.HelpBox("没有匹配的 Sprite", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 网格布局：根据窗口宽度计算列数
        int columnWidth = 180;
        int columns = Mathf.Max(1, (int)(position.width - 20) / columnWidth);
        int itemIndex = 0;

        while (itemIndex < filtered.Count)
        {
            EditorGUILayout.BeginHorizontal();

            for (int col = 0; col < columns && itemIndex < filtered.Count; col++)
            {
                var sprite = filtered[itemIndex];
                DrawSpriteItem(sprite);
                itemIndex++;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSpriteItem(Sprite sprite)
    {
        bool isSelected = selectedSprites.Contains(sprite);
        GUIStyle style = isSelected ? selectedButtonStyle : itemButtonStyle;

        Rect itemRect = EditorGUILayout.BeginHorizontal(style, GUILayout.Width(170), GUILayout.Height(30));

        // 勾选框
        bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(18));
        if (newSelected != isSelected)
        {
            if (newSelected) selectedSprites.Add(sprite);
            else selectedSprites.Remove(sprite);
        }

        // Sprite 图标
        if (sprite != null && sprite.texture != null)
        {
            Rect iconRect = GUILayoutUtility.GetRect(24, 24, GUILayout.Width(24), GUILayout.Height(24));
            GUI.DrawTextureWithTexCoords(iconRect, sprite.texture,
                new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
                         sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height));
        }
        else
        {
            GUILayout.Space(24);
        }

        // 名称
        GUILayout.Label(sprite.name, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndHorizontal();

        // 点击整行切换选中
        if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
        {
            if (isSelected) selectedSprites.Remove(sprite);
            else selectedSprites.Add(sprite);
            Event.current.Use();
            Repaint();
        }
    }

    private void DrawBottomButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("取消", GUILayout.Width(80), GUILayout.Height(28)))
            {
                Close();
            }

            GUILayout.Space(10);

            using (new EditorGUI.DisabledScope(selectedSprites.Count == 0))
            {
                if (GUILayout.Button($"添加 ({selectedSprites.Count})", GUILayout.Width(110), GUILayout.Height(28)))
                {
                    AddSelectedSprites();
                }
            }
        }

        EditorGUILayout.Space(3);
    }

    #endregion

    #region 辅助方法

    private List<Sprite> GetFilteredSprites()
    {
        if (string.IsNullOrEmpty(searchFilter))
            return allSprites;

        string filter = searchFilter.ToLower();
        return allSprites.Where(s => s.name.ToLower().Contains(filter)).ToList();
    }

    private void SelectAllFiltered()
    {
        var filtered = GetFilteredSprites();
        foreach (var s in filtered)
        {
            selectedSprites.Add(s);
        }
    }

    private void AddSelectedSprites()
    {
        // 按 allSprites 中的原始顺序返回
        var ordered = allSprites.Where(s => selectedSprites.Contains(s)).ToList();
        onSpritesSelected?.Invoke(ordered);
        Close();
    }

    #endregion
}
