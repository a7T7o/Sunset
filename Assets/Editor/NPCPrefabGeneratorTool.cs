using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// NPC 预制体生成器
/// 输入四向三帧 PNG 后，一键生成 Sprite 切片、动画、控制器和预制体。
/// </summary>
public class NPCPrefabGeneratorTool : EditorWindow
{
    #region 内部类型

    private enum SourceDirection
    {
        Down,
        Left,
        Right,
        Up
    }

    private enum PivotMode
    {
        BottomCenter,
        Center
    }

    private enum GeneratedState
    {
        Idle = 0,
        Move = 1,
        Death = 2
    }

    private sealed class TextureTask
    {
        public string TexturePath;
        public string FileNameWithoutExtension;
        public GeneratedState State;
        public Dictionary<SourceDirection, List<Sprite>> SpritesByDirection = new Dictionary<SourceDirection, List<Sprite>>();
    }

    private sealed class GeneratedAssets
    {
        public string NpcName;
        public string ClipsFolder;
        public string ControllerPath;
        public string PrefabPath;
        public Dictionary<string, AnimationClip> Clips = new Dictionary<string, AnimationClip>();
    }

    #endregion

    #region 配置字段

    private DefaultAsset inputFolder;
    private string npcNameOverride = string.Empty;

    private string animationRootPath = "Assets/100_Anim/NPC";
    private string prefabRootPath = "Assets/Prefabs/NPC";

    private int columns = 3;
    private int rows = 4;
    private int pixelsPerUnit = 16;
    private float clipFrameRate = 6f;

    private SourceDirection row0 = SourceDirection.Down;
    private SourceDirection row1 = SourceDirection.Left;
    private SourceDirection row2 = SourceDirection.Right;
    private SourceDirection row3 = SourceDirection.Up;

    private PivotMode pivotMode = PivotMode.BottomCenter;
    private string sortingLayerName = "Layer 1";

    private float defaultMoveSpeed = 2.5f;
    private float defaultIdleAnimationSpeed = 1f;
    private float defaultMoveAnimationSpeed = 1f;
    private float defaultDeathAnimationSpeed = 1f;
    private bool enableDebugLogOnPrefab = false;

    private Vector2 scrollPos;
    private string lastSummary = "尚未执行生成。";

    #endregion

    #region 菜单

    [MenuItem("Tools/NPC/NPC预制体生成器")]
    private static void ShowWindow()
    {
        NPCPrefabGeneratorTool window = GetWindow<NPCPrefabGeneratorTool>("NPC预制体生成器");
        window.minSize = new Vector2(620f, 760f);
        window.Show();
    }

    #endregion

    #region Unity生命周期

    private void OnEnable()
    {
        string[] sortingLayers = GetSortingLayerNames();
        if (sortingLayers != null && sortingLayers.Length > 0 && !sortingLayers.Contains(sortingLayerName))
        {
            sortingLayerName = sortingLayers[0];
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawHeader();
        DrawInputSection();
        DrawGridSection();
        DrawOutputSection();
        DrawRuntimeSection();
        DrawSummarySection();
        DrawActionButtons();

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region 界面绘制

    private void DrawHeader()
    {
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("━━━━ NPC 预制体生成器 ━━━━", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "输入四向三帧 NPC PNG 后，一键生成：\n" +
            "1. Sprite 切片与重命名\n" +
            "2. Idle / Move / Death 动画剪辑\n" +
            "3. NPC Animator Controller\n" +
            "4. 可直接拖入场景的 NPC Prefab\n\n" +
            "默认桥接规则：Left / Right → Side 参数 + flipX。",
            MessageType.Info);
        EditorGUILayout.Space(10f);
    }

    private void DrawInputSection()
    {
        EditorGUILayout.LabelField("━━━━ 输入 ━━━━", EditorStyles.boldLabel);
        inputFolder = EditorGUILayout.ObjectField("PNG 文件夹", inputFolder, typeof(DefaultAsset), false) as DefaultAsset;
        npcNameOverride = EditorGUILayout.TextField("NPC 名称（可选覆盖）", npcNameOverride);

        EditorGUILayout.HelpBox(
            "文件夹内至少需要包含 Idle 与 Run 的 PNG。\n" +
            "支持文件名关键词：Idle / Run / Walk / Move / Death。",
            MessageType.None);

        EditorGUILayout.Space(10f);
    }

    private void DrawGridSection()
    {
        EditorGUILayout.LabelField("━━━━ 切片与方向映射 ━━━━", EditorStyles.boldLabel);

        columns = EditorGUILayout.IntField("列数（每方向帧数）", columns);
        rows = EditorGUILayout.IntField("行数（方向数）", rows);
        pixelsPerUnit = EditorGUILayout.IntField("Pixels Per Unit", pixelsPerUnit);
        clipFrameRate = EditorGUILayout.FloatField("Clip 帧率", clipFrameRate);
        pivotMode = (PivotMode)EditorGUILayout.EnumPopup("Pivot 模式", pivotMode);

        EditorGUILayout.Space(5f);
        EditorGUILayout.LabelField("行方向顺序", EditorStyles.boldLabel);
        row0 = (SourceDirection)EditorGUILayout.EnumPopup("第 0 行", row0);
        row1 = (SourceDirection)EditorGUILayout.EnumPopup("第 1 行", row1);
        row2 = (SourceDirection)EditorGUILayout.EnumPopup("第 2 行", row2);
        row3 = (SourceDirection)EditorGUILayout.EnumPopup("第 3 行", row3);

        EditorGUILayout.HelpBox(
            "默认按用户当前 NPC 模板处理：Down / Left / Right / Up。\n" +
            "若后续换素材包，只需要在这里改行顺序，不需要改代码。",
            MessageType.None);

        EditorGUILayout.Space(10f);
    }

    private void DrawOutputSection()
    {
        EditorGUILayout.LabelField("━━━━ 输出路径 ━━━━", EditorStyles.boldLabel);
        animationRootPath = EditorGUILayout.TextField("动画根目录", animationRootPath);
        prefabRootPath = EditorGUILayout.TextField("预制体根目录", prefabRootPath);

        string[] sortingLayers = GetSortingLayerNames();
        if (sortingLayers != null && sortingLayers.Length > 0)
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
        defaultDeathAnimationSpeed = EditorGUILayout.FloatField("Death 动画速度", defaultDeathAnimationSpeed);
        enableDebugLogOnPrefab = EditorGUILayout.Toggle("生成时启用调试日志", enableDebugLogOnPrefab);

        EditorGUILayout.HelpBox(
            "默认值已参考当前玩家实配：WalkSpeed=4、RunSpeed=6、Animator.speed=1。\n" +
            "这里给 NPC 采用更保守的初值，后续可在生成出的预制体 Inspector 中继续调。",
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
        GUI.enabled = inputFolder != null;

        if (GUILayout.Button("扫描输入", GUILayout.Height(36f)))
        {
            ScanInputFolder();
        }

        if (GUILayout.Button("一键生成 NPC 资源", GUILayout.Height(48f)))
        {
            GenerateNpcAssets();
        }

        GUI.enabled = true;
    }

    #endregion

    #region 生成流程

    private void ScanInputFolder()
    {
        if (!TryValidateInput(out string inputFolderPath, out string npcName, out string error))
        {
            lastSummary = error;
            ShowNotification(new GUIContent("输入校验失败"));
            return;
        }

        List<TextureTask> tasks = CollectTextureTasks(inputFolderPath);
        if (tasks.Count == 0)
        {
            lastSummary = "未在输入文件夹内找到可识别的 PNG（Idle / Run / Death）。";
            return;
        }

        string stateSummary = string.Join("、", tasks.Select(t => $"{t.FileNameWithoutExtension}->{t.State}"));
        lastSummary = $"扫描成功：NPC={npcName}\n识别到 {tasks.Count} 张动作图：{stateSummary}";
    }

    private void GenerateNpcAssets()
    {
        if (!TryValidateInput(out string inputFolderPath, out string npcName, out string error))
        {
            lastSummary = error;
            EditorUtility.DisplayDialog("NPC 生成器", error, "确定");
            return;
        }

        try
        {
            List<TextureTask> tasks = CollectTextureTasks(inputFolderPath);
            if (tasks.Count == 0)
            {
                throw new InvalidOperationException("未找到可识别的 Idle / Run / Death PNG。");
            }

            if (!tasks.Any(t => t.State == GeneratedState.Idle) || !tasks.Any(t => t.State == GeneratedState.Move))
            {
                throw new InvalidOperationException("至少需要 Idle 和 Run（或 Walk / Move）两类 PNG。");
            }

            foreach (TextureTask task in tasks)
            {
                SliceTexture(task);
                task.SpritesByDirection = LoadDirectionalSprites(task);
            }

            GeneratedAssets assets = CreateAnimationAssets(npcName, tasks);
            RuntimeAnimatorController controller = CreateAnimatorController(assets);
            CreatePrefab(npcName, controller, tasks, assets.PrefabPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            lastSummary =
                $"生成完成：\n" +
                $"NPC 名称：{npcName}\n" +
                $"动作图数量：{tasks.Count}\n" +
                $"动画目录：{assets.ClipsFolder}\n" +
                $"控制器：{assets.ControllerPath}\n" +
                $"预制体：{assets.PrefabPath}";

            EditorUtility.DisplayDialog("NPC 生成器", "NPC 资源生成完成。", "确定");
        }
        catch (Exception ex)
        {
            lastSummary = $"生成失败：{ex.Message}";
            Debug.LogException(ex);
            EditorUtility.DisplayDialog("NPC 生成器", $"生成失败：\n{ex.Message}", "确定");
        }
    }

    #endregion

    #region 输入处理

    private bool TryValidateInput(out string inputFolderPath, out string npcName, out string error)
    {
        inputFolderPath = string.Empty;
        npcName = string.Empty;
        error = string.Empty;

        if (inputFolder == null)
        {
            error = "请先选择 NPC PNG 文件夹。";
            return false;
        }

        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);
        pixelsPerUnit = Mathf.Max(1, pixelsPerUnit);
        clipFrameRate = Mathf.Max(1f, clipFrameRate);

        inputFolderPath = AssetDatabase.GetAssetPath(inputFolder);
        if (string.IsNullOrEmpty(inputFolderPath) || !AssetDatabase.IsValidFolder(inputFolderPath))
        {
            error = "输入对象不是有效的 Unity 文件夹。";
            return false;
        }

        npcName = string.IsNullOrWhiteSpace(npcNameOverride)
            ? Path.GetFileName(inputFolderPath.TrimEnd('/', '\\'))
            : npcNameOverride.Trim();

        if (string.IsNullOrWhiteSpace(npcName))
        {
            error = "无法推断 NPC 名称，请手动填写名称。";
            return false;
        }

        return true;
    }

    private List<TextureTask> CollectTextureTasks(string inputFolderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { inputFolderPath });
        List<TextureTask> tasks = new List<TextureTask>();

        foreach (string guid in guids)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(guid);
            string extension = Path.GetExtension(texturePath);
            if (!string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(texturePath);
            if (!TryParseState(fileName, out GeneratedState state))
            {
                continue;
            }

            tasks.Add(new TextureTask
            {
                TexturePath = texturePath,
                FileNameWithoutExtension = fileName,
                State = state
            });
        }

        return tasks
            .OrderBy(t => t.State)
            .ThenBy(t => t.FileNameWithoutExtension, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private bool TryParseState(string fileName, out GeneratedState state)
    {
        string lowered = fileName.ToLowerInvariant();

        if (lowered.Contains("idle"))
        {
            state = GeneratedState.Idle;
            return true;
        }

        if (lowered.Contains("run") || lowered.Contains("walk") || lowered.Contains("move"))
        {
            state = GeneratedState.Move;
            return true;
        }

        if (lowered.Contains("death"))
        {
            state = GeneratedState.Death;
            return true;
        }

        state = GeneratedState.Idle;
        return false;
    }

    #endregion

    #region 纹理切片

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

        int cellWidth = texture.width / columns;
        int cellHeight = texture.height / rows;
        if (cellWidth <= 0 || cellHeight <= 0)
        {
            throw new InvalidOperationException($"切片尺寸非法：{task.TexturePath}");
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = pixelsPerUnit;

        SpriteMetaData[] metas = BuildSpriteMetas(task, texture.width, texture.height, cellWidth, cellHeight);
        importer.spritesheet = metas;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private SpriteMetaData[] BuildSpriteMetas(TextureTask task, int textureWidth, int textureHeight, int cellWidth, int cellHeight)
    {
        List<SpriteMetaData> metas = new List<SpriteMetaData>(rows * columns);
        SourceDirection[] rowDirections = GetRowDirections();

        for (int row = 0; row < rows; row++)
        {
            SourceDirection direction = row < rowDirections.Length ? rowDirections[row] : SourceDirection.Down;

            for (int col = 0; col < columns; col++)
            {
                Rect rect = new Rect(
                    col * cellWidth,
                    textureHeight - ((row + 1) * cellHeight),
                    cellWidth,
                    cellHeight);

                SpriteMetaData meta = new SpriteMetaData
                {
                    name = $"{task.State}_{direction}_{col}",
                    rect = rect,
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = pivotMode == PivotMode.BottomCenter ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f)
                };
                metas.Add(meta);
            }
        }

        return metas.ToArray();
    }

    private Dictionary<SourceDirection, List<Sprite>> LoadDirectionalSprites(TextureTask task)
    {
        Dictionary<SourceDirection, List<Sprite>> map = new Dictionary<SourceDirection, List<Sprite>>();
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(task.TexturePath).OfType<Sprite>().ToArray();

        foreach (Sprite sprite in sprites)
        {
            string[] parts = sprite.name.Split('_');
            if (parts.Length < 3)
            {
                continue;
            }

            if (!Enum.TryParse(parts[1], out SourceDirection direction))
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

        foreach ((SourceDirection direction, List<Sprite> list) in map.ToArray())
        {
            map[direction] = list.OrderBy(sprite => ExtractTrailingNumber(sprite.name)).ToList();
        }

        return map;
    }

    private SourceDirection[] GetRowDirections()
    {
        return new[]
        {
            row0,
            row1,
            row2,
            row3
        };
    }

    private int ExtractTrailingNumber(string value)
    {
        int lastUnderscore = value.LastIndexOf('_');
        if (lastUnderscore < 0 || lastUnderscore >= value.Length - 1)
        {
            return 0;
        }

        string suffix = value.Substring(lastUnderscore + 1);
        return int.TryParse(suffix, out int number) ? number : 0;
    }

    #endregion

    #region 动画资源生成

    private GeneratedAssets CreateAnimationAssets(string npcName, List<TextureTask> tasks)
    {
        string npcAnimRoot = $"{animationRootPath}/{npcName}";
        string clipsFolder = $"{npcAnimRoot}/Clips";
        string controllerFolder = $"{npcAnimRoot}/Controller";

        EnsureAssetFolder(animationRootPath);
        EnsureAssetFolder(npcAnimRoot);
        EnsureAssetFolder(clipsFolder);
        EnsureAssetFolder(controllerFolder);
        EnsureAssetFolder(prefabRootPath);

        GeneratedAssets assets = new GeneratedAssets
        {
            NpcName = npcName,
            ClipsFolder = clipsFolder,
            ControllerPath = $"{controllerFolder}/{npcName}_Controller.controller",
            PrefabPath = $"{prefabRootPath}/{npcName}.prefab"
        };

        foreach (TextureTask task in tasks)
        {
            BuildClipForDirection(task, SourceDirection.Down, "Down", task.State, assets);
            BuildSideClip(task, task.State, assets);
            BuildClipForDirection(task, SourceDirection.Up, "Up", task.State, assets);
        }

        return assets;
    }

    private void BuildClipForDirection(TextureTask task, SourceDirection sourceDirection, string clipDirection, GeneratedState state, GeneratedAssets assets)
    {
        if (!task.SpritesByDirection.TryGetValue(sourceDirection, out List<Sprite> sprites) || sprites.Count == 0)
        {
            return;
        }

        string clipName = $"{state}_{clipDirection}_Clip";
        string clipPath = $"{assets.ClipsFolder}/{clipName}.anim";
        bool shouldLoop = state != GeneratedState.Death;

        AnimationClip clip = CreateAnimationClip(clipName, clipPath, sprites, shouldLoop);
        assets.Clips[clipName] = clip;
    }

    private void BuildSideClip(TextureTask task, GeneratedState state, GeneratedAssets assets)
    {
        List<Sprite> sourceSprites = null;

        if (task.SpritesByDirection.TryGetValue(SourceDirection.Right, out List<Sprite> rightSprites) && rightSprites.Count > 0)
        {
            sourceSprites = rightSprites;
        }
        else if (task.SpritesByDirection.TryGetValue(SourceDirection.Left, out List<Sprite> leftSprites) && leftSprites.Count > 0)
        {
            sourceSprites = leftSprites;
        }

        if (sourceSprites == null || sourceSprites.Count == 0)
        {
            return;
        }

        string clipName = $"{state}_Side_Clip";
        string clipPath = $"{assets.ClipsFolder}/{clipName}.anim";
        bool shouldLoop = state != GeneratedState.Death;

        AnimationClip clip = CreateAnimationClip(clipName, clipPath, sourceSprites, shouldLoop);
        assets.Clips[clipName] = clip;
    }

    private AnimationClip CreateAnimationClip(string clipName, string clipPath, List<Sprite> sprites, bool shouldLoop)
    {
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath) != null)
        {
            AssetDatabase.DeleteAsset(clipPath);
        }

        AnimationClip clip = new AnimationClip
        {
            name = clipName,
            frameRate = clipFrameRate
        };

        ObjectReferenceKeyframe[] keyframes = BuildKeyframes(sprites);
        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = string.Empty,
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = shouldLoop;
        settings.stopTime = sprites.Count / clipFrameRate;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
    }

    private ObjectReferenceKeyframe[] BuildKeyframes(List<Sprite> sprites)
    {
        List<ObjectReferenceKeyframe> keyframes = new List<ObjectReferenceKeyframe>();

        for (int i = 0; i < sprites.Count; i++)
        {
            keyframes.Add(new ObjectReferenceKeyframe
            {
                time = i / clipFrameRate,
                value = sprites[i]
            });
        }

        if (sprites.Count > 0)
        {
            keyframes.Add(new ObjectReferenceKeyframe
            {
                time = Mathf.Max((sprites.Count / clipFrameRate) - 0.0001f, 0f),
                value = sprites[sprites.Count - 1]
            });
        }

        return keyframes.ToArray();
    }

    #endregion

    #region 控制器生成

    private RuntimeAnimatorController CreateAnimatorController(GeneratedAssets assets)
    {
        if (AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(assets.ControllerPath) != null)
        {
            AssetDatabase.DeleteAsset(assets.ControllerPath);
        }

        UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(assets.ControllerPath);
        controller.AddParameter("State", UnityEngine.AnimatorControllerParameterType.Int);
        controller.AddParameter("Direction", UnityEngine.AnimatorControllerParameterType.Int);

        UnityEditor.Animations.AnimatorControllerLayer baseLayer = controller.layers[0];
        AnimatorStateMachine stateMachine = baseLayer.stateMachine;

        Dictionary<string, AnimatorState> states = new Dictionary<string, AnimatorState>();
        int index = 0;

        foreach (KeyValuePair<string, AnimationClip> pair in assets.Clips.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            AnimatorState state = stateMachine.AddState(pair.Key, new Vector3(320f + ((index % 3) * 240f), (index / 3) * 90f, 0f));
            state.motion = pair.Value;
            states[pair.Key] = state;
            index++;
        }

        if (states.TryGetValue("Idle_Down_Clip", out AnimatorState defaultState))
        {
            stateMachine.defaultState = defaultState;
        }
        else if (states.Count > 0)
        {
            stateMachine.defaultState = states.Values.First();
        }

        foreach (KeyValuePair<string, AnimatorState> pair in states)
        {
            if (!TryParseClipStateInfo(pair.Key, out GeneratedState state, out int direction))
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

    private bool TryParseClipStateInfo(string clipName, out GeneratedState state, out int direction)
    {
        state = GeneratedState.Idle;
        direction = 0;

        string[] parts = clipName.Split('_');
        if (parts.Length < 3)
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

    #endregion

    #region 预制体生成

    private void CreatePrefab(string npcName, RuntimeAnimatorController controller, List<TextureTask> tasks, string prefabPath)
    {
        Sprite initialSprite = ResolveInitialSprite(tasks);
        if (initialSprite == null)
        {
            throw new InvalidOperationException("无法确定预制体初始 Sprite。");
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        GameObject root = new GameObject(npcName);
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

            CircleCollider2D circleCollider = root.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            ConfigureCollider(circleCollider, initialSprite);

            NPCAnimController animController = root.AddComponent<NPCAnimController>();
            animController.SetAnimationSpeed(defaultIdleAnimationSpeed, defaultMoveAnimationSpeed, defaultDeathAnimationSpeed);

            NPCMotionController motionController = root.AddComponent<NPCMotionController>();
            motionController.MoveSpeed = defaultMoveSpeed;

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

    private Sprite ResolveInitialSprite(List<TextureTask> tasks)
    {
        TextureTask idleTask = tasks.FirstOrDefault(task => task.State == GeneratedState.Idle);
        if (idleTask == null || idleTask.SpritesByDirection.Count == 0)
        {
            return null;
        }

        if (idleTask.SpritesByDirection.TryGetValue(SourceDirection.Down, out List<Sprite> downSprites) && downSprites.Count > 0)
        {
            return downSprites[0];
        }

        return idleTask.SpritesByDirection.Values.FirstOrDefault(list => list.Count > 0)?.FirstOrDefault();
    }

    private void ConfigureCollider(CircleCollider2D circleCollider, Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        float radius = Mathf.Clamp(bounds.size.x * 0.35f, 0.18f, 0.75f);
        Vector2 offset = new Vector2(bounds.center.x, bounds.min.y + radius);

        circleCollider.radius = radius;
        circleCollider.offset = offset;
    }

    private void SetPrivateBoolField(object target, string fieldName, bool value)
    {
        SerializedObject serializedObject = new SerializedObject((UnityEngine.Object)target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);
        if (property != null)
        {
            property.boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    #endregion

    #region 目录辅助

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

    #endregion
}
