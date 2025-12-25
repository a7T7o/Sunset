using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// 001批量工具 - Project窗口专用
/// 整合：纹理设置、SR设置
/// </summary>
public class Tool_001_BatchProject : EditorWindow
{
    private enum ToolMode { 纹理设置, SR设置 }
    private ToolMode currentMode = ToolMode.纹理设置;
    private Vector2 scrollPos;

    [MenuItem("Tools/001批量 (Project窗口)")]
    public static void ShowWindow()
    {
        var window = GetWindow<Tool_001_BatchProject>("001批量-Project");
        window.minSize = new Vector2(480, 650);
        window.Show();
    }

    private void OnEnable()
    {
        currentMode = (ToolMode)EditorPrefs.GetInt("Batch001_Mode", 0);
        LoadSettings();
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt("Batch001_Mode", (int)currentMode);
        SaveSettings();
    }

    private void OnGUI()
    {
        DrawHeader();
        DrawModeSwitch();
        
        EditorGUILayout.Space(3);
        DrawLine();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        if (currentMode == ToolMode.纹理设置)
            DrawTextureMode();
        else
            DrawSRMode();
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField("📁 001批量工具 (Project)", style, GUILayout.Height(28));
    }

    private void DrawModeSwitch()
    {
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = currentMode == ToolMode.纹理设置 ? new Color(0.3f, 0.8f, 1f) : Color.white;
        if (GUILayout.Button("🎨 纹理设置", GUILayout.Height(40)))
        {
            currentMode = ToolMode.纹理设置;
            EditorPrefs.SetInt("Batch001_Mode", 0);
        }
        
        GUI.backgroundColor = currentMode == ToolMode.SR设置 ? new Color(0.3f, 0.8f, 1f) : Color.white;
        if (GUILayout.Button("🖼️ SR设置", GUILayout.Height(40)))
        {
            currentMode = ToolMode.SR设置;
            EditorPrefs.SetInt("Batch001_Mode", 1);
        }
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        
        // 恢复默认按钮
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("🔄 恢复默认", GUILayout.Width(100)))
        {
            if (EditorUtility.DisplayDialog("确认", $"恢复【{currentMode}】的默认设置？", "确定", "取消"))
            {
                ResetCurrentMode();
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
    }

    #region ========== 纹理设置模式 ==========

    private List<Object> tex_selected = new List<Object>();
    private bool tex_includeSub = true;
    
    // 勾选项
    private bool tex_chk_ppu = true;
    private bool tex_chk_filter = true;
    private bool tex_chk_pivot = true;
    private bool tex_chk_compress = false;
    private bool tex_chk_maxsize = false;
    private bool tex_chk_readwrite = false;  // ✅ 新增：Read/Write Enabled
    
    // 参数
    private float tex_ppu = 16;
    private FilterMode tex_filter = FilterMode.Point;
    private SpriteAlignment tex_pivot = SpriteAlignment.BottomCenter;
    private TextureImporterCompression tex_compress = TextureImporterCompression.Uncompressed;
    private int tex_maxsize = 2048;

    private void DrawTextureMode()
    {
        EditorGUILayout.HelpBox("📂 在Project窗口选择文件/文件夹，支持.aseprite", MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🔍 获取选中项", GUILayout.Height(32)))
        {
            GetSelectedAssets();
        }
        
        // 显示选中
        if (tex_selected.Count == 0)
        {
            EditorGUILayout.HelpBox("⚠️ 未选择任何项", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"✓ 已选择 {tex_selected.Count} 项", EditorStyles.boldLabel);
            int show = Mathf.Min(tex_selected.Count, 8);
            for (int i = 0; i < show; i++)
            {
                string path = AssetDatabase.GetAssetPath(tex_selected[i]);
                bool isDir = AssetDatabase.IsValidFolder(path);
                EditorGUILayout.LabelField($"{(isDir ? "📁" : "📄")} {System.IO.Path.GetFileName(path)}", EditorStyles.miniLabel);
            }
            if (tex_selected.Count > 8) EditorGUILayout.LabelField($"... 还有 {tex_selected.Count - 8} 项");
            EditorGUILayout.EndVertical();
        }
        
        tex_includeSub = EditorGUILayout.ToggleLeft("包含子文件夹", tex_includeSub);
        
        DrawLine();
        
        EditorGUILayout.LabelField("⚙️ 设置参数", EditorStyles.boldLabel);
        
        // PPU
        EditorGUILayout.BeginHorizontal();
        tex_chk_ppu = EditorGUILayout.Toggle(tex_chk_ppu, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tex_chk_ppu);
        tex_ppu = EditorGUILayout.FloatField("PPU", tex_ppu);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        // Filter
        EditorGUILayout.BeginHorizontal();
        tex_chk_filter = EditorGUILayout.Toggle(tex_chk_filter, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tex_chk_filter);
        tex_filter = (FilterMode)EditorGUILayout.EnumPopup("过滤模式", tex_filter);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        // Pivot
        EditorGUILayout.BeginHorizontal();
        tex_chk_pivot = EditorGUILayout.Toggle(tex_chk_pivot, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tex_chk_pivot);
        tex_pivot = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot对齐", tex_pivot);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // 高级选项（折叠）
        EditorGUILayout.BeginHorizontal();
        tex_chk_compress = EditorGUILayout.Toggle(tex_chk_compress, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tex_chk_compress);
        tex_compress = (TextureImporterCompression)EditorGUILayout.EnumPopup("压缩", tex_compress);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        tex_chk_maxsize = EditorGUILayout.Toggle(tex_chk_maxsize, GUILayout.Width(20));
        EditorGUI.BeginDisabledGroup(!tex_chk_maxsize);
        tex_maxsize = EditorGUILayout.IntPopup("最大尺寸", tex_maxsize,
            new[]{"512","1024","2048","4096","8192"},
            new[]{512,1024,2048,4096,8192});
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        // ✅ 新增：Read/Write Enabled（用于像素采样遮挡检测）
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        tex_chk_readwrite = EditorGUILayout.Toggle(tex_chk_readwrite, GUILayout.Width(20));
        EditorGUILayout.LabelField("Read/Write Enabled", EditorStyles.label);
        EditorGUILayout.EndHorizontal();
        if (tex_chk_readwrite)
        {
            EditorGUILayout.HelpBox("⚠️ 启用后纹理将占用更多内存，但支持像素采样遮挡检测", MessageType.Warning);
        }
        
        DrawLine();
        
        // 应用按钮
        GUI.enabled = tex_selected.Count > 0;
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("🚀 应用设置", GUILayout.Height(40)))
        {
            ApplyTextureSettings();
        }
        GUI.backgroundColor = Color.white;
        GUI.enabled = true;
    }

    private void GetSelectedAssets()
    {
        tex_selected.Clear();
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsValidFolder(path) || obj is Texture2D || 
                path.EndsWith(".aseprite", System.StringComparison.OrdinalIgnoreCase))
            {
                tex_selected.Add(obj);
            }
        }
        Repaint();
    }

    private void ApplyTextureSettings()
    {
        List<string> files = new List<string>();
        
        foreach (var obj in tex_selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            
            if (AssetDatabase.IsValidFolder(path))
            {
                // 文件夹：查找纹理
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[]{path});
                foreach (string guid in guids)
                {
                    string p = AssetDatabase.GUIDToAssetPath(guid);
                    if (!tex_includeSub)
                    {
                        string dir = System.IO.Path.GetDirectoryName(p).Replace('\\', '/');
                        if (dir != path) continue;
                    }
                    if (!files.Contains(p)) files.Add(p);
                }
                
                // 查找.aseprite
                string[] aseFiles = System.IO.Directory.GetFiles(path, "*.aseprite",
                    tex_includeSub ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
                foreach (string full in aseFiles)
                {
                    string p = full.Replace('\\', '/').Replace(Application.dataPath, "Assets");
                    if (!files.Contains(p)) files.Add(p);
                }
            }
            else
            {
                // 单文件
                if (!files.Contains(path)) files.Add(path);
            }
        }
        
        if (files.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到可处理的文件！", "确定");
            return;
        }
        
        string msg = $"将修改 {files.Count} 个文件\n\n";
        if (tex_chk_ppu) msg += $"• PPU → {tex_ppu}\n";
        if (tex_chk_filter) msg += $"• Filter → {tex_filter}\n";
        if (tex_chk_pivot) msg += $"• Pivot → {tex_pivot}\n";
        if (tex_chk_compress) msg += $"• 压缩 → {tex_compress}\n";
        if (tex_chk_maxsize) msg += $"• 最大尺寸 → {tex_maxsize}\n";
        if (tex_chk_readwrite) msg += $"• Read/Write → 启用\n";
        msg += "\n是否继续？";
        
        if (!EditorUtility.DisplayDialog("确认", msg, "确定", "取消")) return;
        
        int success = 0, fail = 0;
        
        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 0; i < files.Count; i++)
            {
                EditorUtility.DisplayProgressBar("应用设置", $"{i+1}/{files.Count}", (float)i/files.Count);
                
                try
                {
                    string path = files[i];
                    AssetImporter imp = AssetImporter.GetAtPath(path);
                    if (imp == null) { fail++; continue; }
                    
                    bool isAse = path.EndsWith(".aseprite", System.StringComparison.OrdinalIgnoreCase);
                    
                    if (!isAse)
                    {
                        // 普通纹理
                        TextureImporter ti = imp as TextureImporter;
                        if (ti == null) { fail++; continue; }
                        
                        if (ti.textureType != TextureImporterType.Sprite)
                            ti.textureType = TextureImporterType.Sprite;
                        
                        if (tex_chk_ppu) ti.spritePixelsPerUnit = tex_ppu;
                        if (tex_chk_filter) ti.filterMode = tex_filter;
                        if (tex_chk_compress) ti.textureCompression = tex_compress;
                        if (tex_chk_maxsize) ti.maxTextureSize = tex_maxsize;
                        if (tex_chk_readwrite) ti.isReadable = true;  // ✅ 启用 Read/Write
                        
                        if (tex_chk_pivot)
                        {
                            TextureImporterSettings s = new TextureImporterSettings();
                            ti.ReadTextureSettings(s);
                            s.spriteAlignment = (int)tex_pivot;
                            ti.SetTextureSettings(s);
                        }
                        
                        EditorUtility.SetDirty(ti);
                        ti.SaveAndReimport();
                        success++;
                    }
                    else
                    {
                        // Aseprite文件 - 用反射
                        var type = imp.GetType();
                        bool modified = false;
                        
                        if (tex_chk_ppu)
                        {
                            var prop = type.GetProperty("spritePixelsPerUnit");
                            if (prop != null && prop.CanWrite)
                            {
                                prop.SetValue(imp, tex_ppu);
                                modified = true;
                            }
                        }
                        
                        if (tex_chk_pivot)
                        {
                            var prop = type.GetProperty("pivotAlignment");
                            if (prop != null && prop.CanWrite)
                            {
                                prop.SetValue(imp, tex_pivot);
                                modified = true;
                            }
                        }
                        
                        if (tex_chk_filter)
                        {
                            var prop = type.GetProperty("filterMode");
                            if (prop != null && prop.CanWrite)
                            {
                                prop.SetValue(imp, tex_filter);
                                modified = true;
                            }
                        }
                        
                        if (modified)
                        {
                            EditorUtility.SetDirty(imp);
                            imp.SaveAndReimport();
                        }
                        success++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"处理失败: {files[i]}\n{ex.Message}");
                    fail++;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();
        }
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"成功: {success}\n失败: {fail}", "确定");
        Debug.Log($"<color=green>[001批量] 纹理设置完成！成功:{success} 失败:{fail}</color>");
    }

    #endregion

    #region ========== SR设置模式 ==========

    private void DrawSRMode()
    {
        EditorGUILayout.HelpBox("🚧 SR设置功能整合中...\n暂时请使用原有的【批量SpriteRenderer设置工具】", MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("打开原工具", GUILayout.Height(35)))
        {
            EditorApplication.ExecuteMenuItem("Tools/批量SpriteRenderer设置工具");
        }
    }

    #endregion

    #region ========== 设置保存/加载 ==========

    private void LoadSettings()
    {
        // 纹理设置
        tex_includeSub = EditorPrefs.GetBool("Batch001_Tex_IncludeSub", true);
        tex_chk_ppu = EditorPrefs.GetBool("Batch001_Tex_ChkPPU", true);
        tex_chk_filter = EditorPrefs.GetBool("Batch001_Tex_ChkFilter", true);
        tex_chk_pivot = EditorPrefs.GetBool("Batch001_Tex_ChkPivot", true);
        tex_chk_compress = EditorPrefs.GetBool("Batch001_Tex_ChkCompress", false);
        tex_chk_maxsize = EditorPrefs.GetBool("Batch001_Tex_ChkMaxSize", false);
        tex_chk_readwrite = EditorPrefs.GetBool("Batch001_Tex_ChkReadWrite", false);  // ✅ 新增
        
        tex_ppu = EditorPrefs.GetFloat("Batch001_Tex_PPU", 16);
        tex_filter = (FilterMode)EditorPrefs.GetInt("Batch001_Tex_Filter", (int)FilterMode.Point);
        tex_pivot = (SpriteAlignment)EditorPrefs.GetInt("Batch001_Tex_Pivot", (int)SpriteAlignment.BottomCenter);
        tex_compress = (TextureImporterCompression)EditorPrefs.GetInt("Batch001_Tex_Compress", 0);
        tex_maxsize = EditorPrefs.GetInt("Batch001_Tex_MaxSize", 2048);
    }

    private void SaveSettings()
    {
        // 纹理设置
        EditorPrefs.SetBool("Batch001_Tex_IncludeSub", tex_includeSub);
        EditorPrefs.SetBool("Batch001_Tex_ChkPPU", tex_chk_ppu);
        EditorPrefs.SetBool("Batch001_Tex_ChkFilter", tex_chk_filter);
        EditorPrefs.SetBool("Batch001_Tex_ChkPivot", tex_chk_pivot);
        EditorPrefs.SetBool("Batch001_Tex_ChkCompress", tex_chk_compress);
        EditorPrefs.SetBool("Batch001_Tex_ChkMaxSize", tex_chk_maxsize);
        EditorPrefs.SetBool("Batch001_Tex_ChkReadWrite", tex_chk_readwrite);  // ✅ 新增
        
        EditorPrefs.SetFloat("Batch001_Tex_PPU", tex_ppu);
        EditorPrefs.SetInt("Batch001_Tex_Filter", (int)tex_filter);
        EditorPrefs.SetInt("Batch001_Tex_Pivot", (int)tex_pivot);
        EditorPrefs.SetInt("Batch001_Tex_Compress", (int)tex_compress);
        EditorPrefs.SetInt("Batch001_Tex_MaxSize", tex_maxsize);
    }

    private void ResetCurrentMode()
    {
        if (currentMode == ToolMode.纹理设置)
        {
            tex_chk_ppu = true;
            tex_chk_filter = true;
            tex_chk_pivot = true;
            tex_chk_compress = false;
            tex_chk_maxsize = false;
            tex_chk_readwrite = false;  // ✅ 新增
            
            tex_ppu = 16;
            tex_filter = FilterMode.Point;
            tex_pivot = SpriteAlignment.BottomCenter;
            tex_compress = TextureImporterCompression.Uncompressed;
            tex_maxsize = 2048;
            tex_includeSub = true;
        }
        
        SaveSettings();
        Repaint();
    }

    #endregion
}


