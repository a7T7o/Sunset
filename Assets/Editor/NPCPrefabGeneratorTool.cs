using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// NPC 预制体生成器。
/// 固定适配 Sunset 当前 NPC 通用模板：
/// - 一张 PNG = 一个 NPC
/// - 固定 4 行 3 列
/// - 第 0 行 Down / 第 1 行 Left / 第 2 行 Right / 第 3 行 Up
/// - Idle 只使用每行第 1 列
/// - Move 使用每行 3 帧
/// </summary>
public class NPCPrefabGeneratorTool : EditorWindow
{
    private enum TemplateDirection
    {
        Down,
        Left,
        Right,
        Up
    }

    private enum GeneratedState
    {
        Idle = 0,
        Move = 1
    }

    private enum GeneratedNpcRole
    {
        Production = 0,
        BubbleReview = 1
    }

    private sealed class TextureTask
    {
        public string TexturePath;
        public string NpcName;
        public GeneratedNpcRole Role;
        public Dictionary<TemplateDirection, List<Sprite>> SpritesByDirection = new Dictionary<TemplateDirection, List<Sprite>>();
    }

    private sealed class GeneratedAssets
    {
        public string NpcName;
        public string ClipsFolder;
        public string ControllerPath;
        public string PrefabPath;
        public Dictionary<string, AnimationClip> Clips = new Dictionary<string, AnimationClip>();
    }

    private const int Columns = 3;
    private const int Rows = 4;
    private const int IdleFrameIndex = 1;
    private const int PixelsPerUnit = 16;
    private const float ClipFrameRate = 6f;
    private const string DefaultRoamProfilePath = "Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset";
    private const string BubbleReviewProfilePath = "Assets/111_Data/NPC/NPC_BubbleReviewProfile.asset";
    private const string VillageChiefProfilePath = "Assets/111_Data/NPC/NPC_001_VillageChiefRoamProfile.asset";
    private const string VillageDaughterProfilePath = "Assets/111_Data/NPC/NPC_002_VillageDaughterRoamProfile.asset";
    private const string ResearchReviewProfilePath = "Assets/111_Data/NPC/NPC_003_ResearchReviewProfile.asset";

    private static readonly string[] ResearcherStressTalkLines =
    {
        "先记一下。",
        "这个位置的光线比路口稳定。",
        "如果大家都从这边经过，我得留意间距。",
        "我想再看一遍气泡的停留时间和换行节奏。",
        "刚才那段路径有点挤，我得记下是谁先让开的。",
        "只要把这几次相遇顺序记清楚，我就能知道问题更像出在让行还是节奏。",
        "从这里观察会更方便，我能同时看到停顿、转向和说话时机有没有互相打架。",
        "要是下一轮大家的移动更自然了，我就可以把这份记录从测试笔记改成真正有用的结论。"
    };

    private static readonly TemplateDirection[] RowDirections =
    {
        TemplateDirection.Down,
        TemplateDirection.Left,
        TemplateDirection.Right,
        TemplateDirection.Up
    };

    private readonly List<Texture2D> selectedTextures = new List<Texture2D>();

    private string animationRootPath = "Assets/100_Anim/NPC";
    private string prefabRootPath = "Assets/222_Prefabs/NPC";
    private string sortingLayerName = "Layer 1";

    private float defaultMoveSpeed = 2.5f;
    private float defaultIdleAnimationSpeed = 1f;
    private float defaultMoveAnimationSpeed = 1f;
    private bool enableDebugLogOnPrefab = false;
    private bool autoAssignBubbleReviewRole = false;
    private bool addStressTalkerToBubbleReview = true;
    private string bubbleReviewNpcNames = string.Empty;

    private Vector2 scrollPos;
    private string lastSummary = "尚未获取选中项。";

    [MenuItem("Tools/NPC/NPC预制体生成器")]
    private static void ShowWindow()
    {
        NPCPrefabGeneratorTool window = GetWindow<NPCPrefabGeneratorTool>("NPC预制体生成器");
        window.minSize = new Vector2(620f, 660f);
        window.Show();
    }

    private void OnEnable()
    {
        string[] sortingLayers = GetSortingLayerNames();
        if (sortingLayers.Length > 0 && !sortingLayers.Contains(sortingLayerName))
        {
            sortingLayerName = sortingLayers[0];
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawHeader();
        DrawSelectionSection();
        DrawOutputSection();
        DrawRuntimeSection();
        DrawRolePresetSection();
        DrawSummarySection();
        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("━━━━ NPC 预制体生成器 ━━━━", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "固定处理 Sunset 当前 NPC 通用模板：\n" +
            "1. 一张 PNG = 一个 NPC\n" +
            "2. 固定 4 行 3 列\n" +
            "3. 第 0 行 Down / 第 1 行 Left / 第 2 行 Right / 第 3 行 Up\n" +
            "4. Idle 只取中间帧，Move 使用三帧\n" +
            "5. 直接生成 Idle / Move / Controller / Prefab",
            MessageType.Info);
        EditorGUILayout.Space(10f);
    }

    private void DrawSelectionSection()
    {
        EditorGUILayout.LabelField("━━━━ 选中项 ━━━━", EditorStyles.boldLabel);

        if (GUILayout.Button("获取选中项", GUILayout.Height(32f)))
        {
            RefreshSelectedTextures();
        }

        if (selectedTextures.Count == 0)
        {
            EditorGUILayout.HelpBox("去 Project 窗口选中 PNG 或文件夹，然后点“获取选中项”。", MessageType.None);
        }
        else
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"已获取 {selectedTextures.Count} 个 PNG", EditorStyles.boldLabel);

            int showCount = Mathf.Min(selectedTextures.Count, 8);
            for (int i = 0; i < showCount; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedTextures[i]);
                EditorGUILayout.LabelField($"• {Path.GetFileName(assetPath)}", EditorStyles.miniLabel);
            }

            if (selectedTextures.Count > showCount)
            {
                EditorGUILayout.LabelField($"... 还有 {selectedTextures.Count - showCount} 个", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(10f);
    }

    private void DrawOutputSection()
    {
        EditorGUILayout.LabelField("━━━━ 输出路径 ━━━━", EditorStyles.boldLabel);
        animationRootPath = EditorGUILayout.TextField("动画根目录", animationRootPath);
        prefabRootPath = EditorGUILayout.TextField("预制体根目录", prefabRootPath);

        string[] sortingLayers = GetSortingLayerNames();
        if (sortingLayers.Length > 0)
        {
            int selectedIndex = Mathf.Max(0, Array.IndexOf(sortingLayers, sortingLayerName));
            selectedIndex = EditorGUILayout.Popup("Sorting Layer", selectedIndex, sortingLayers);
            sortingLayerName = sortingLayers[selectedIndex];
        }
        else
        {
            sortingLayerName = EditorGUILayout.TextField("Sorting Layer", sortingLayerName);
        }

        EditorGUILayout.Space(10f);
    }

    private void DrawRuntimeSection()
    {
        EditorGUILayout.LabelField("━━━━ 运行时默认值 ━━━━", EditorStyles.boldLabel);
        defaultMoveSpeed = EditorGUILayout.FloatField("默认移动速度", defaultMoveSpeed);
        defaultIdleAnimationSpeed = EditorGUILayout.FloatField("Idle 动画速度", defaultIdleAnimationSpeed);
        defaultMoveAnimationSpeed = EditorGUILayout.FloatField("Move 动画速度", defaultMoveAnimationSpeed);
        enableDebugLogOnPrefab = EditorGUILayout.Toggle("生成时启用调试日志", enableDebugLogOnPrefab);

        EditorGUILayout.HelpBox(
            "4 行 3 列、PPU=16、帧率=6、Bottom Center Pivot、方向顺序全都已经硬编码。\n" +
            "这里只保留输出路径、Sorting Layer 和运行时默认值。",
            MessageType.None);

        EditorGUILayout.Space(10f);
    }

    private void DrawRolePresetSection()
    {
        EditorGUILayout.LabelField("━━━━ 角色预设 ━━━━", EditorStyles.boldLabel);
        autoAssignBubbleReviewRole = EditorGUILayout.Toggle("自动分流验证样本", autoAssignBubbleReviewRole);

        using (new EditorGUI.DisabledScope(!autoAssignBubbleReviewRole))
        {
            bubbleReviewNpcNames = EditorGUILayout.TextField("验证样本名称", bubbleReviewNpcNames);
            addStressTalkerToBubbleReview = EditorGUILayout.Toggle("验证样本自动挂压测组件", addStressTalkerToBubbleReview);
        }

        EditorGUILayout.HelpBox(
            "正式 NPC 默认不自动进入压测模式。\n" +
            "只有显式填写到验证样本名称里的 NPC，才会进入 BubbleReview 模式并自动启用 NPCBubbleStressTalker。\n" +
            "验证样本名称支持逗号、空格、分号或换行分隔，例如：003, NPC_Test_A",
            MessageType.None);

        EditorGUILayout.Space(10f);
    }

    private void DrawSummarySection()
    {
        EditorGUILayout.LabelField("━━━━ 最近结果 ━━━━", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(lastSummary, MessageType.None);
        EditorGUILayout.Space(10f);
    }

    private void DrawActionButtons()
    {
        GUI.enabled = selectedTextures.Count > 0;
        if (GUILayout.Button("一键生成 NPC 资源", GUILayout.Height(50f)))
        {
            GenerateNpcAssets();
        }

        GUI.enabled = true;
    }

    private void RefreshSelectedTextures()
    {
        selectedTextures.Clear();
        HashSet<string> texturePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (UnityEngine.Object selectedObject in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                continue;
            }

            if (AssetDatabase.IsValidFolder(assetPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { assetPath });
                foreach (string guid in guids)
                {
                    string texturePath = AssetDatabase.GUIDToAssetPath(guid);
                    if (string.Equals(Path.GetExtension(texturePath), ".png", StringComparison.OrdinalIgnoreCase))
                    {
                        texturePaths.Add(texturePath);
                    }
                }

                continue;
            }

            if (string.Equals(Path.GetExtension(assetPath), ".png", StringComparison.OrdinalIgnoreCase))
            {
                texturePaths.Add(assetPath);
            }
        }

        foreach (string texturePath in texturePaths.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture != null)
            {
                selectedTextures.Add(texture);
            }
        }

        if (selectedTextures.Count == 0)
        {
            lastSummary = "未获取到有效 PNG。请去 Project 窗口重新选择。";
            return;
        }

        lastSummary = $"已获取 {selectedTextures.Count} 个 PNG，可以直接一键生成。";
    }

    private void GenerateNpcAssets()
    {
        if (selectedTextures.Count == 0)
        {
            lastSummary = "没有可生成的 PNG。";
            EditorUtility.DisplayDialog("NPC 生成器", "没有可生成的 PNG。请先获取选中项。", "确定");
            return;
        }

        defaultMoveSpeed = Mathf.Max(0f, defaultMoveSpeed);
        defaultIdleAnimationSpeed = Mathf.Max(0.1f, defaultIdleAnimationSpeed);
        defaultMoveAnimationSpeed = Mathf.Max(0.1f, defaultMoveAnimationSpeed);

        List<TextureTask> tasks = selectedTextures
            .Select(texture => new TextureTask
            {
                TexturePath = AssetDatabase.GetAssetPath(texture),
                NpcName = texture.name,
                Role = ResolveGeneratedNpcRole(texture.name)
            })
            .ToList();

        try
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                TextureTask task = tasks[i];
                EditorUtility.DisplayProgressBar(
                    "NPC 生成器",
                    $"正在处理 {task.NpcName} ({i + 1}/{tasks.Count})",
                    (float)(i + 1) / tasks.Count);

                SliceTexture(task);
                task.SpritesByDirection = LoadDirectionalSprites(task);
                ValidateDirectionalSprites(task);

                GeneratedAssets assets = CreateAnimationAssets(task);
                RuntimeAnimatorController controller = CreateAnimatorController(assets);
                CreatePrefab(task, controller, assets.PrefabPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            int bubbleReviewCount = tasks.Count(task => task.Role == GeneratedNpcRole.BubbleReview);
            int productionCount = tasks.Count - bubbleReviewCount;
            lastSummary = $"生成完成：共处理 {tasks.Count} 个 NPC PNG，正式 {productionCount} / 验证样本 {bubbleReviewCount}。";
            EditorUtility.DisplayDialog(
                "NPC 生成器",
                $"生成完成，共处理 {tasks.Count} 个 NPC PNG。\n正式：{productionCount}\n验证样本：{bubbleReviewCount}",
                "确定");
        }
        catch (Exception ex)
        {
            lastSummary = $"生成失败：{ex.Message}";
            Debug.LogException(ex);
            EditorUtility.DisplayDialog("NPC 生成器", $"生成失败：\n{ex.Message}", "确定");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void SliceTexture(TextureTask task)
    {
        TextureImporter importer = AssetImporter.GetAtPath(task.TexturePath) as TextureImporter;
        if (importer == null)
        {
            throw new InvalidOperationException($"无法读取 TextureImporter：{task.TexturePath}");
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(task.TexturePath);
        if (texture == null)
        {
            throw new InvalidOperationException($"无法加载纹理：{task.TexturePath}");
        }

        if (texture.width % Columns != 0 || texture.height % Rows != 0)
        {
            throw new InvalidOperationException($"{task.NpcName} 不是标准 4x3 模板：{task.TexturePath}");
        }

        int cellWidth = texture.width / Columns;
        int cellHeight = texture.height / Rows;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        importer = AssetImporter.GetAtPath(task.TexturePath) as TextureImporter;
        if (importer == null)
        {
            throw new InvalidOperationException($"切片回写后无法重新读取 TextureImporter：{task.TexturePath}");
        }

        ApplySpriteSlices(task.NpcName, texture.height, cellWidth, cellHeight, importer);
        AssetDatabase.ImportAsset(task.TexturePath, ImportAssetOptions.ForceUpdate);
    }

    private void ApplySpriteSlices(string npcName, int textureHeight, int cellWidth, int cellHeight, TextureImporter importer)
    {
        SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
        factory.Init();

        ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        if (dataProvider == null)
        {
            throw new InvalidOperationException($"无法初始化 SpriteEditorDataProvider：{importer.assetPath}");
        }

        dataProvider.InitSpriteEditorDataProvider();

        Dictionary<string, GUID> existingSpriteIds = dataProvider
            .GetSpriteRects()
            .ToDictionary(rect => rect.name, rect => rect.spriteID, StringComparer.Ordinal);

        List<SpriteRect> spriteRects = BuildSpriteRects(npcName, textureHeight, cellWidth, cellHeight, existingSpriteIds);
        dataProvider.SetSpriteRects(spriteRects.ToArray());

        ISpriteNameFileIdDataProvider nameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        if (nameFileIdDataProvider != null)
        {
            List<SpriteNameFileIdPair> nameFileIdPairs = spriteRects
                .Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID))
                .ToList();
            nameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
        }

        dataProvider.Apply();
    }

    private List<SpriteRect> BuildSpriteRects(
        string npcName,
        int textureHeight,
        int cellWidth,
        int cellHeight,
        Dictionary<string, GUID> existingSpriteIds)
    {
        List<SpriteRect> spriteRects = new List<SpriteRect>(Rows * Columns);

        for (int row = 0; row < Rows; row++)
        {
            TemplateDirection direction = RowDirections[row];

            for (int col = 0; col < Columns; col++)
            {
                string spriteName = $"{npcName}_{direction}_{col}";
                spriteRects.Add(new SpriteRect
                {
                    name = spriteName,
                    rect = new Rect(col * cellWidth, textureHeight - ((row + 1) * cellHeight), cellWidth, cellHeight),
                    alignment = SpriteAlignment.Custom,
                    pivot = new Vector2(0.5f, 0f),
                    spriteID = existingSpriteIds.TryGetValue(spriteName, out GUID spriteId)
                        ? spriteId
                        : GUID.Generate()
                });
            }
        }

        return spriteRects;
    }

    private Dictionary<TemplateDirection, List<Sprite>> LoadDirectionalSprites(TextureTask task)
    {
        Dictionary<TemplateDirection, List<Sprite>> map = new Dictionary<TemplateDirection, List<Sprite>>();
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(task.TexturePath).OfType<Sprite>().ToArray();

        foreach (Sprite sprite in sprites)
        {
            string[] parts = sprite.name.Split('_');
            if (parts.Length < 3)
            {
                continue;
            }

            if (!Enum.TryParse(parts[parts.Length - 2], out TemplateDirection direction))
            {
                continue;
            }

            if (!map.TryGetValue(direction, out List<Sprite> list))
            {
                list = new List<Sprite>();
                map[direction] = list;
            }

            list.Add(sprite);
        }

        foreach (TemplateDirection direction in map.Keys.ToList())
        {
            map[direction] = map[direction]
                .OrderBy(sprite => ExtractTrailingNumber(sprite.name))
                .ToList();
        }

        return map;
    }

    private void ValidateDirectionalSprites(TextureTask task)
    {
        foreach (TemplateDirection direction in RowDirections)
        {
            if (!task.SpritesByDirection.TryGetValue(direction, out List<Sprite> sprites) || sprites.Count != Columns)
            {
                throw new InvalidOperationException($"{task.NpcName} 缺少 {direction} 方向的 3 帧切片。");
            }
        }
    }

    private int ExtractTrailingNumber(string value)
    {
        int lastUnderscore = value.LastIndexOf('_');
        if (lastUnderscore < 0 || lastUnderscore >= value.Length - 1)
        {
            return 0;
        }

        return int.TryParse(value.Substring(lastUnderscore + 1), out int number) ? number : 0;
    }

    private GeneratedAssets CreateAnimationAssets(TextureTask task)
    {
        string npcAnimRoot = $"{animationRootPath}/{task.NpcName}";
        string clipsFolder = $"{npcAnimRoot}/Clips";
        string controllerFolder = $"{npcAnimRoot}/Controller";

        EnsureAssetFolder(animationRootPath);
        EnsureAssetFolder(npcAnimRoot);
        EnsureAssetFolder(clipsFolder);
        EnsureAssetFolder(controllerFolder);
        EnsureAssetFolder(prefabRootPath);

        GeneratedAssets assets = new GeneratedAssets
        {
            NpcName = task.NpcName,
            ClipsFolder = clipsFolder,
            ControllerPath = $"{controllerFolder}/{task.NpcName}_Controller.controller",
            PrefabPath = $"{prefabRootPath}/{task.NpcName}.prefab"
        };

        BuildDirectionalClip(task, TemplateDirection.Down, "Down", GeneratedState.Idle, assets);
        BuildSideClip(task, GeneratedState.Idle, assets);
        BuildDirectionalClip(task, TemplateDirection.Up, "Up", GeneratedState.Idle, assets);

        BuildDirectionalClip(task, TemplateDirection.Down, "Down", GeneratedState.Move, assets);
        BuildSideClip(task, GeneratedState.Move, assets);
        BuildDirectionalClip(task, TemplateDirection.Up, "Up", GeneratedState.Move, assets);

        return assets;
    }

    private void BuildDirectionalClip(
        TextureTask task,
        TemplateDirection sourceDirection,
        string clipDirection,
        GeneratedState state,
        GeneratedAssets assets)
    {
        List<Sprite> clipSprites = FilterSpritesForState(task.SpritesByDirection[sourceDirection], state);
        string clipName = $"{task.NpcName}_{state}_{clipDirection}";
        string clipPath = $"{assets.ClipsFolder}/{clipName}.anim";
        AnimationClip clip = CreateAnimationClip(clipName, clipPath, clipSprites);
        assets.Clips[clipName] = clip;
    }

    private void BuildSideClip(TextureTask task, GeneratedState state, GeneratedAssets assets)
    {
        List<Sprite> sourceSprites = task.SpritesByDirection[TemplateDirection.Right];
        if (sourceSprites == null || sourceSprites.Count == 0)
        {
            sourceSprites = task.SpritesByDirection[TemplateDirection.Left];
        }

        List<Sprite> clipSprites = FilterSpritesForState(sourceSprites, state);
        string clipName = $"{task.NpcName}_{state}_Side";
        string clipPath = $"{assets.ClipsFolder}/{clipName}.anim";
        AnimationClip clip = CreateAnimationClip(clipName, clipPath, clipSprites);
        assets.Clips[clipName] = clip;
    }

    private List<Sprite> FilterSpritesForState(List<Sprite> sprites, GeneratedState state)
    {
        if (state == GeneratedState.Idle)
        {
            return new List<Sprite> { sprites[IdleFrameIndex] };
        }

        return new List<Sprite>(sprites);
    }

    private AnimationClip CreateAnimationClip(string clipName, string clipPath, List<Sprite> sprites)
    {
        AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(clipPath);
        }

        AnimationClip clip = new AnimationClip
        {
            name = clipName,
            frameRate = ClipFrameRate
        };

        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = string.Empty,
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, BuildKeyframes(sprites));

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        settings.stopTime = Mathf.Max(sprites.Count / ClipFrameRate, 1f / ClipFrameRate);
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
    }

    private ObjectReferenceKeyframe[] BuildKeyframes(List<Sprite> sprites)
    {
        float clipDuration = Mathf.Max(sprites.Count / ClipFrameRate, 1f / ClipFrameRate);
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();

        for (int i = 0; i < sprites.Count; i++)
        {
            keyframes.Add(new ObjectReferenceKeyframe
            {
                time = i / ClipFrameRate,
                value = sprites[i]
            });
        }

        keyframes.Add(new ObjectReferenceKeyframe
        {
            time = Mathf.Max(clipDuration - 0.0001f, 0f),
            value = sprites[sprites.Count - 1]
        });

        return keyframes.ToArray();
    }

    private RuntimeAnimatorController CreateAnimatorController(GeneratedAssets assets)
    {
        AnimatorController existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(assets.ControllerPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(assets.ControllerPath);
        }

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(assets.ControllerPath);
        controller.AddParameter("State", AnimatorControllerParameterType.Int);
        controller.AddParameter("Direction", AnimatorControllerParameterType.Int);

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        Dictionary<string, AnimatorState> states = new Dictionary<string, AnimatorState>();

        int index = 0;
        foreach (KeyValuePair<string, AnimationClip> pair in assets.Clips.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            AnimatorState state = stateMachine.AddState(
                pair.Key,
                new Vector3(320f + ((index % 3) * 240f), (index / 3) * 90f, 0f));
            state.motion = pair.Value;
            states[pair.Key] = state;
            index++;
        }

        string defaultStateName = $"{assets.NpcName}_{GeneratedState.Idle}_Down";
        if (states.TryGetValue(defaultStateName, out AnimatorState defaultState))
        {
            stateMachine.defaultState = defaultState;
        }
        else if (states.Count > 0)
        {
            stateMachine.defaultState = states.Values.First();
        }

        foreach (KeyValuePair<string, AnimatorState> pair in states)
        {
            if (!TryParseClipStateInfo(assets.NpcName, pair.Key, out GeneratedState state, out int direction))
            {
                continue;
            }

            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(pair.Value);
            transition.hasExitTime = false;
            transition.duration = 0f;
            transition.offset = 0f;
            transition.canTransitionToSelf = false;
            transition.AddCondition(AnimatorConditionMode.Equals, (float)state, "State");
            transition.AddCondition(AnimatorConditionMode.Equals, direction, "Direction");
        }

        AssetDatabase.SaveAssets();
        return AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assets.ControllerPath);
    }

    private bool TryParseClipStateInfo(string npcName, string clipName, out GeneratedState state, out int direction)
    {
        state = GeneratedState.Idle;
        direction = 0;

        string prefix = $"{npcName}_";
        if (!clipName.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        string[] parts = clipName.Substring(prefix.Length).Split('_');
        if (parts.Length < 2)
        {
            return false;
        }

        if (!Enum.TryParse(parts[0], out state))
        {
            return false;
        }

        direction = parts[1] switch
        {
            "Down" => 0,
            "Up" => 1,
            "Side" => 2,
            _ => 0
        };

        return true;
    }

    private void CreatePrefab(TextureTask task, RuntimeAnimatorController controller, string prefabPath)
    {
        Sprite initialSprite = ResolveInitialSprite(task);
        if (initialSprite == null)
        {
            throw new InvalidOperationException($"{task.NpcName} 无法确定初始 Sprite。");
        }

        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        GameObject root = new GameObject(task.NpcName);
        try
        {
            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = initialSprite;
            spriteRenderer.sortingLayerName = sortingLayerName;

            Animator animator = root.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;

            DynamicSortingOrder sortingOrder = root.AddComponent<DynamicSortingOrder>();
            sortingOrder.useSpriteBounds = true;
            sortingOrder.autoHandleShadow = true;

            BoxCollider2D boxCollider = root.AddComponent<BoxCollider2D>();
            ConfigureCollider(boxCollider, initialSprite);

            Rigidbody2D rigidbody2D = root.AddComponent<Rigidbody2D>();
            ConfigureRigidbody(rigidbody2D);

            NPCAnimController animController = root.AddComponent<NPCAnimController>();
            animController.SetAnimationSpeed(defaultIdleAnimationSpeed, defaultMoveAnimationSpeed);

            NPCMotionController motionController = root.AddComponent<NPCMotionController>();
            motionController.MoveSpeed = defaultMoveSpeed;

            root.AddComponent<NPCBubblePresenter>();
            NPCAutoRoamController roamController = root.AddComponent<NPCAutoRoamController>();
            ApplyGeneratedRole(task, root, roamController);

            if (!enableDebugLogOnPrefab)
            {
                SetPrivateBoolField(animController, "showDebugLog", false);
                SetPrivateBoolField(motionController, "showDebugLog", false);
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            DestroyImmediate(root);
        }
    }

    private Sprite ResolveInitialSprite(TextureTask task)
    {
        return task.SpritesByDirection[TemplateDirection.Down][IdleFrameIndex];
    }

    private void ConfigureCollider(BoxCollider2D boxCollider, Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        float colliderWidth = Mathf.Clamp(bounds.size.x * 0.44f, 0.32f, Mathf.Max(0.36f, bounds.size.x * 0.72f));
        float colliderHeight = Mathf.Clamp(bounds.size.y * 0.34f, 0.28f, Mathf.Max(0.34f, bounds.size.y * 0.46f));
        float bottomInset = Mathf.Clamp(bounds.size.y * 0.06f, 0.02f, 0.12f);

        boxCollider.size = new Vector2(colliderWidth, colliderHeight);
        boxCollider.offset = new Vector2(bounds.center.x, bounds.min.y + bottomInset + (colliderHeight * 0.5f));
        boxCollider.isTrigger = false;
    }

    private void ConfigureRigidbody(Rigidbody2D rigidbody2D)
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.mass = 6f;
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.linearDamping = 8f;
        rigidbody2D.angularDamping = 0.05f;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void SetPrivateBoolField(UnityEngine.Object target, string fieldName, bool value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);
        if (property != null)
        {
            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private GeneratedNpcRole ResolveGeneratedNpcRole(string npcName)
    {
        if (!autoAssignBubbleReviewRole)
        {
            return GeneratedNpcRole.Production;
        }

        HashSet<string> bubbleReviewNames = ParseNpcNameList(bubbleReviewNpcNames);
        return bubbleReviewNames.Contains(npcName)
            ? GeneratedNpcRole.BubbleReview
            : GeneratedNpcRole.Production;
    }

    private HashSet<string> ParseNpcNameList(string rawValue)
    {
        HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return names;
        }

        string[] tokens = rawValue.Split(new[] { ',', ';', '|', '\n', '\r', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tokens.Length; i++)
        {
            string normalized = tokens[i].Trim();
            if (!string.IsNullOrEmpty(normalized))
            {
                names.Add(normalized);
            }
        }

        return names;
    }

    private void ApplyGeneratedRole(TextureTask task, GameObject root, NPCAutoRoamController roamController)
    {
        string profilePath = ResolveGeneratedProfilePath(task.NpcName, task.Role);
        AssignRoamProfile(roamController, profilePath);

        if (task.Role == GeneratedNpcRole.BubbleReview && addStressTalkerToBubbleReview)
        {
            NPCBubbleStressTalker stressTalker = root.AddComponent<NPCBubbleStressTalker>();
            stressTalker.ConfigureMode(enableOnStart: true, disableRoamDuringTest: true);
            stressTalker.RebindReferences();
            ConfigureBubbleReviewContent(task.NpcName, stressTalker);
        }
    }

    private string ResolveGeneratedProfilePath(string npcName, GeneratedNpcRole role)
    {
        if (role == GeneratedNpcRole.BubbleReview && string.Equals(npcName, "003", StringComparison.OrdinalIgnoreCase))
        {
            return ResearchReviewProfilePath;
        }

        if (role == GeneratedNpcRole.Production)
        {
            if (string.Equals(npcName, "001", StringComparison.OrdinalIgnoreCase))
            {
                return VillageChiefProfilePath;
            }

            if (string.Equals(npcName, "002", StringComparison.OrdinalIgnoreCase))
            {
                return VillageDaughterProfilePath;
            }

            if (string.Equals(npcName, "003", StringComparison.OrdinalIgnoreCase))
            {
                return ResearchReviewProfilePath;
            }
        }

        return role == GeneratedNpcRole.BubbleReview
            ? BubbleReviewProfilePath
            : DefaultRoamProfilePath;
    }

    private void ConfigureBubbleReviewContent(string npcName, NPCBubbleStressTalker stressTalker)
    {
        if (stressTalker == null || !string.Equals(npcName, "003", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(stressTalker);
        SerializedProperty linesProperty = serializedObject.FindProperty("testLines");
        if (linesProperty == null || !linesProperty.isArray)
        {
            return;
        }

        linesProperty.arraySize = ResearcherStressTalkLines.Length;
        for (int i = 0; i < ResearcherStressTalkLines.Length; i++)
        {
            linesProperty.GetArrayElementAtIndex(i).stringValue = ResearcherStressTalkLines[i];
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private void AssignRoamProfile(NPCAutoRoamController roamController, string profilePath)
    {
        if (roamController == null)
        {
            return;
        }

        NPCRoamProfile profile = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(profilePath);
        if (profile == null)
        {
            return;
        }

        SerializedObject serializedObject = new SerializedObject(roamController);
        SerializedProperty profileProperty = serializedObject.FindProperty("roamProfile");
        SerializedProperty applyOnAwakeProperty = serializedObject.FindProperty("applyProfileOnAwake");

        if (profileProperty != null)
        {
            profileProperty.objectReferenceValue = profile;
        }

        if (applyOnAwakeProperty != null)
        {
            applyOnAwakeProperty.boolValue = true;
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        roamController.ApplyProfile();
    }

    private void EnsureAssetFolder(string assetFolderPath)
    {
        string normalizedPath = assetFolderPath.Replace("\\", "/");
        string[] parts = normalizedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts[0] != "Assets")
        {
            throw new InvalidOperationException($"非法资源路径：{assetFolderPath}");
        }

        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }

            current = next;
        }
    }

    private string[] GetSortingLayerNames()
    {
        return SortingLayer.layers.Select(layer => layer.name).ToArray();
    }
}
