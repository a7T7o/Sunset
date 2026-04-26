using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;

public static class FarmAnimalPrefabBuilder
{
    private enum GeneratedState
    {
        Idle = 0,
        Move = 1
    }

    private enum FacingVisualPreset
    {
        ChickenHybrid = 0,
        CowLeftSourceSide = 1
    }

    private enum RoamProfilePreset
    {
        Chicken = 0,
        Cow = 1
    }

    private sealed class AnimalSpec
    {
        public string TexturePath;
        public string AnimalName;
        public int TotalRows;
        public int DownRowIndex;
        public int UpRowIndex;
        public int SideRowIndex;
        public FacingVisualPreset FacingPreset;
        public RoamProfilePreset RoamPreset;
    }

    private sealed class GeneratedAssets
    {
        public string AnimalName;
        public string ClipsFolder;
        public string ControllerPath;
        public string PrefabPath;
        public readonly Dictionary<string, AnimationClip> Clips = new Dictionary<string, AnimationClip>();
    }

    private const string MenuPath = "Tools/NPC/Farm Animals/生成可漫游动物预制体";
    private const string SourceRoot = "Assets/ZZZ_999_Package/001_OK/Await/Farm RPG FREE 16x16 - Tiny Asset Pack/Farm Animals";
    private const string AnimationRoot = "Assets/100_Anim/FarmAnimals";
    private const string PrefabRoot = "Assets/222_Prefabs/FarmAnimals";
    private const string DataRoot = "Assets/111_Data/NPC/FarmAnimals";
    private const string ChickenRoamProfilePath = DataRoot + "/FarmAnimal_ChickenRoamProfile.asset";
    private const string CowRoamProfilePath = DataRoot + "/FarmAnimal_CowRoamProfile.asset";
    private const int Columns = 4;
    private const int IdleFrameIndex = 1;
    private const int PixelsPerUnit = 16;
    private const float ClipFrameRate = 6f;

    private static readonly AnimalSpec[] Specs =
    {
        new AnimalSpec
        {
            TexturePath = SourceRoot + "/Baby Chicken Yellow.png",
            AnimalName = "Baby Chicken Yellow",
            TotalRows = 3,
            DownRowIndex = 0,
            UpRowIndex = 1,
            SideRowIndex = 1,
            FacingPreset = FacingVisualPreset.ChickenHybrid,
            RoamPreset = RoamProfilePreset.Chicken
        },
        new AnimalSpec
        {
            TexturePath = SourceRoot + "/Chicken Blonde  Green.png",
            AnimalName = "Chicken Blonde  Green",
            TotalRows = 2,
            DownRowIndex = 0,
            UpRowIndex = 1,
            SideRowIndex = 1,
            FacingPreset = FacingVisualPreset.ChickenHybrid,
            RoamPreset = RoamProfilePreset.Chicken
        },
        new AnimalSpec
        {
            TexturePath = SourceRoot + "/Chicken Red.png",
            AnimalName = "Chicken Red",
            TotalRows = 2,
            DownRowIndex = 0,
            UpRowIndex = 1,
            SideRowIndex = 1,
            FacingPreset = FacingVisualPreset.ChickenHybrid,
            RoamPreset = RoamProfilePreset.Chicken
        },
        new AnimalSpec
        {
            TexturePath = SourceRoot + "/Female Cow Brown.png",
            AnimalName = "Female Cow Brown",
            TotalRows = 3,
            DownRowIndex = 1,
            UpRowIndex = 2,
            SideRowIndex = 0,
            FacingPreset = FacingVisualPreset.CowLeftSourceSide,
            RoamPreset = RoamProfilePreset.Cow
        },
        new AnimalSpec
        {
            TexturePath = SourceRoot + "/Male Cow Brown.png",
            AnimalName = "Male Cow Brown",
            TotalRows = 3,
            DownRowIndex = 1,
            UpRowIndex = 2,
            SideRowIndex = 0,
            FacingPreset = FacingVisualPreset.CowLeftSourceSide,
            RoamPreset = RoamProfilePreset.Cow
        }
    };

    [MenuItem(MenuPath)]
    private static void GenerateAll()
    {
        try
        {
            EnsureAssetFolder(AnimationRoot);
            EnsureAssetFolder(PrefabRoot);
            EnsureAssetFolder(DataRoot);

            NPCRoamProfile chickenProfile = EnsureRoamProfile(ChickenRoamProfilePath, 1.35f, 0.95f, 1.6f, 0.45f, 0.7f, 2.2f, 1.8f, 3.2f);
            NPCRoamProfile cowProfile = EnsureRoamProfile(CowRoamProfilePath, 1.1f, 0.9f, 2.4f, 0.8f, 1.1f, 2.8f, 2.4f, 4.2f);

            for (int i = 0; i < Specs.Length; i++)
            {
                AnimalSpec spec = Specs[i];
                EditorUtility.DisplayProgressBar("Farm Animals", $"正在生成 {spec.AnimalName} ({i + 1}/{Specs.Length})", (float)(i + 1) / Specs.Length);

                SliceTexture(spec);
                Dictionary<int, List<Sprite>> spritesByRow = LoadSpritesByRow(spec);
                ValidateRowSprites(spec, spritesByRow);

                GeneratedAssets assets = CreateAnimationAssets(spec, spritesByRow);
                RuntimeAnimatorController controller = CreateAnimatorController(assets);
                CreatePrefab(spec, spritesByRow, controller, spec.RoamPreset == RoamProfilePreset.Chicken ? chickenProfile : cowProfile, assets.PrefabPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[FarmAnimalPrefabBuilder] 已生成 {Specs.Length} 个农场动物可漫游 prefab。");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void SliceTexture(AnimalSpec spec)
    {
        TextureImporter importer = AssetImporter.GetAtPath(spec.TexturePath) as TextureImporter;
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(spec.TexturePath);
        if (importer == null || texture == null)
        {
            throw new InvalidOperationException($"无法读取动物素材：{spec.TexturePath}");
        }

        if (texture.width % Columns != 0 || texture.height % spec.TotalRows != 0)
        {
            throw new InvalidOperationException($"{spec.AnimalName} 不是 {Columns}x{spec.TotalRows} 的动画表。");
        }

        int cellWidth = texture.width / Columns;
        int cellHeight = texture.height / spec.TotalRows;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.SaveAndReimport();

        importer = AssetImporter.GetAtPath(spec.TexturePath) as TextureImporter;
        ApplySpriteSlices(spec, texture.height, cellWidth, cellHeight, importer);
        AssetDatabase.ImportAsset(spec.TexturePath, ImportAssetOptions.ForceUpdate);
    }

    private static void ApplySpriteSlices(AnimalSpec spec, int textureHeight, int cellWidth, int cellHeight, TextureImporter importer)
    {
        SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
        factory.Init();

        ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        Dictionary<string, GUID> existingIds = dataProvider
            .GetSpriteRects()
            .ToDictionary(rect => rect.name, rect => rect.spriteID, StringComparer.Ordinal);

        List<SpriteRect> spriteRects = new List<SpriteRect>(spec.TotalRows * Columns);
        for (int row = 0; row < spec.TotalRows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                string spriteName = $"{spec.AnimalName}_Row{row}_{col}";
                spriteRects.Add(new SpriteRect
                {
                    name = spriteName,
                    rect = new Rect(col * cellWidth, textureHeight - ((row + 1) * cellHeight), cellWidth, cellHeight),
                    alignment = SpriteAlignment.Custom,
                    pivot = new Vector2(0.5f, 0f),
                    spriteID = existingIds.TryGetValue(spriteName, out GUID id) ? id : GUID.Generate()
                });
            }
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        ISpriteNameFileIdDataProvider nameProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        nameProvider?.SetNameFileIdPairs(spriteRects.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)).ToList());
        dataProvider.Apply();
    }

    private static Dictionary<int, List<Sprite>> LoadSpritesByRow(AnimalSpec spec)
    {
        Dictionary<int, List<Sprite>> result = new Dictionary<int, List<Sprite>>();
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spec.TexturePath).OfType<Sprite>().ToArray();

        foreach (Sprite sprite in sprites)
        {
            if (!TryParseSpriteName(sprite.name, out int rowIndex, out int colIndex))
            {
                continue;
            }

            if (!result.TryGetValue(rowIndex, out List<Sprite> rowSprites))
            {
                rowSprites = new List<Sprite>();
                result[rowIndex] = rowSprites;
            }

            rowSprites.Add(sprite);
        }

        foreach (int rowIndex in result.Keys.ToList())
        {
            result[rowIndex] = result[rowIndex]
                .OrderBy(sprite =>
                {
                    TryParseSpriteName(sprite.name, out _, out int colIndex);
                    return colIndex;
                })
                .ToList();
        }

        return result;
    }

    private static bool TryParseSpriteName(string spriteName, out int rowIndex, out int colIndex)
    {
        rowIndex = -1;
        colIndex = -1;
        string[] parts = spriteName.Split('_');
        if (parts.Length < 2)
        {
            return false;
        }

        string rowToken = parts[parts.Length - 2];
        if (!rowToken.StartsWith("Row", StringComparison.Ordinal) ||
            !int.TryParse(rowToken.Substring(3), out rowIndex))
        {
            return false;
        }

        return int.TryParse(parts[parts.Length - 1], out colIndex);
    }

    private static void ValidateRowSprites(AnimalSpec spec, Dictionary<int, List<Sprite>> spritesByRow)
    {
        for (int row = 0; row < spec.TotalRows; row++)
        {
            if (!spritesByRow.TryGetValue(row, out List<Sprite> sprites) || sprites.Count != Columns)
            {
                throw new InvalidOperationException($"{spec.AnimalName} 第 {row} 行切片不完整。");
            }
        }
    }

    private static GeneratedAssets CreateAnimationAssets(AnimalSpec spec, Dictionary<int, List<Sprite>> spritesByRow)
    {
        string animalAnimRoot = $"{AnimationRoot}/{spec.AnimalName}";
        string clipsFolder = $"{animalAnimRoot}/Clips";
        string controllerFolder = $"{animalAnimRoot}/Controller";

        EnsureAssetFolder(animalAnimRoot);
        EnsureAssetFolder(clipsFolder);
        EnsureAssetFolder(controllerFolder);

        GeneratedAssets assets = new GeneratedAssets
        {
            AnimalName = spec.AnimalName,
            ClipsFolder = clipsFolder,
            ControllerPath = $"{controllerFolder}/{spec.AnimalName}_Controller.controller",
            PrefabPath = $"{PrefabRoot}/{spec.AnimalName}.prefab"
        };

        BuildDirectionalClip(spec, spritesByRow, spec.DownRowIndex, "Down", GeneratedState.Idle, assets);
        BuildDirectionalClip(spec, spritesByRow, spec.SideRowIndex, "Side", GeneratedState.Idle, assets);
        BuildDirectionalClip(spec, spritesByRow, spec.UpRowIndex, "Up", GeneratedState.Idle, assets);
        BuildDirectionalClip(spec, spritesByRow, spec.DownRowIndex, "Down", GeneratedState.Move, assets);
        BuildDirectionalClip(spec, spritesByRow, spec.SideRowIndex, "Side", GeneratedState.Move, assets);
        BuildDirectionalClip(spec, spritesByRow, spec.UpRowIndex, "Up", GeneratedState.Move, assets);

        return assets;
    }

    private static void BuildDirectionalClip(
        AnimalSpec spec,
        Dictionary<int, List<Sprite>> spritesByRow,
        int sourceRowIndex,
        string clipDirection,
        GeneratedState state,
        GeneratedAssets assets)
    {
        List<Sprite> clipSprites = state == GeneratedState.Idle
            ? new List<Sprite> { spritesByRow[sourceRowIndex][Mathf.Clamp(IdleFrameIndex, 0, Columns - 1)] }
            : new List<Sprite>(spritesByRow[sourceRowIndex]);

        string clipName = $"{spec.AnimalName}_{state}_{clipDirection}";
        string clipPath = $"{assets.ClipsFolder}/{clipName}.anim";
        assets.Clips[clipName] = CreateAnimationClip(clipName, clipPath, clipSprites);
    }

    private static AnimationClip CreateAnimationClip(string clipName, string clipPath, List<Sprite> sprites)
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
            time = Mathf.Max((sprites.Count / ClipFrameRate) - 0.0001f, 0f),
            value = sprites[sprites.Count - 1]
        });

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes.ToArray());
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        settings.stopTime = Mathf.Max(sprites.Count / ClipFrameRate, 1f / ClipFrameRate);
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);
        return AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
    }

    private static RuntimeAnimatorController CreateAnimatorController(GeneratedAssets assets)
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

        foreach ((string clipName, AnimationClip clip) in assets.Clips.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            AnimatorState state = stateMachine.AddState(clipName, new Vector3(320f + ((index % 3) * 240f), (index / 3) * 90f, 0f));
            state.motion = clip;
            states[clipName] = state;
            index++;
        }

        if (states.TryGetValue($"{assets.AnimalName}_{GeneratedState.Idle}_Down", out AnimatorState defaultState))
        {
            stateMachine.defaultState = defaultState;
        }

        foreach ((string stateName, AnimatorState state) in states)
        {
            if (!TryParseClipStateInfo(assets.AnimalName, stateName, out GeneratedState generatedState, out int direction))
            {
                continue;
            }

            AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
            transition.hasExitTime = false;
            transition.duration = 0f;
            transition.offset = 0f;
            transition.canTransitionToSelf = false;
            transition.AddCondition(AnimatorConditionMode.Equals, (float)generatedState, "State");
            transition.AddCondition(AnimatorConditionMode.Equals, direction, "Direction");
        }

        AssetDatabase.SaveAssets();
        return AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assets.ControllerPath);
    }

    private static bool TryParseClipStateInfo(string animalName, string clipName, out GeneratedState state, out int direction)
    {
        state = GeneratedState.Idle;
        direction = 0;

        string prefix = $"{animalName}_";
        if (!clipName.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        string[] parts = clipName.Substring(prefix.Length).Split('_');
        if (parts.Length < 2 || !Enum.TryParse(parts[0], out state))
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

    private static void CreatePrefab(
        AnimalSpec spec,
        Dictionary<int, List<Sprite>> spritesByRow,
        RuntimeAnimatorController controller,
        NPCRoamProfile roamProfile,
        string prefabPath)
    {
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        Sprite initialSprite = spritesByRow[spec.DownRowIndex][Mathf.Clamp(IdleFrameIndex, 0, Columns - 1)];
        GameObject root = new GameObject(spec.AnimalName);

        try
        {
            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = initialSprite;
            spriteRenderer.sortingLayerName = ResolveSortingLayerName();

            Animator animator = root.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;

            DynamicSortingOrder sortingOrder = root.AddComponent<DynamicSortingOrder>();
            sortingOrder.useSpriteBounds = true;
            sortingOrder.autoHandleShadow = true;

            BoxCollider2D boxCollider = root.AddComponent<BoxCollider2D>();
            ConfigureCollider(boxCollider, initialSprite);

            Rigidbody2D rigidbody2D = root.AddComponent<Rigidbody2D>();
            ConfigureRigidbody(rigidbody2D, initialSprite);

            NPCAnimController animController = root.AddComponent<NPCAnimController>();
            ApplyFacingVisualPreset(animController, spec.FacingPreset);

            root.AddComponent<NPCMotionController>();
            NPCAutoRoamController roamController = root.AddComponent<NPCAutoRoamController>();
            ApplyRoamProfile(roamController, roamProfile);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    private static void ConfigureCollider(BoxCollider2D boxCollider, Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        float colliderWidth = Mathf.Clamp(bounds.size.x * 0.44f, 0.28f, Mathf.Max(0.34f, bounds.size.x * 0.72f));
        float colliderHeight = Mathf.Clamp(bounds.size.y * 0.34f, 0.24f, Mathf.Max(0.3f, bounds.size.y * 0.5f));
        float bottomInset = Mathf.Clamp(bounds.size.y * 0.06f, 0.02f, 0.14f);

        boxCollider.size = new Vector2(colliderWidth, colliderHeight);
        boxCollider.offset = new Vector2(bounds.center.x, bounds.min.y + bottomInset + (colliderHeight * 0.5f));
        boxCollider.isTrigger = false;
    }

    private static void ConfigureRigidbody(Rigidbody2D rigidbody2D, Sprite sprite)
    {
        float spriteScale = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.mass = spriteScale >= 1.6f ? 8f : 2.2f;
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.linearDamping = 8f;
        rigidbody2D.angularDamping = 0.05f;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private static void ApplyFacingVisualPreset(NPCAnimController animController, FacingVisualPreset preset)
    {
        SerializedObject serializedObject = new SerializedObject(animController);

        switch (preset)
        {
            case FacingVisualPreset.ChickenHybrid:
                SetFacingMapping(serializedObject, useCustom: true, downDirection: 0, upDirection: 1, rightDirection: 1, leftDirection: 0, downFlip: false, upFlip: false, rightFlip: false, leftFlip: false);
                break;

            case FacingVisualPreset.CowLeftSourceSide:
                SetFacingMapping(serializedObject, useCustom: true, downDirection: 0, upDirection: 1, rightDirection: 2, leftDirection: 2, downFlip: false, upFlip: false, rightFlip: true, leftFlip: false);
                break;
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFacingMapping(
        SerializedObject serializedObject,
        bool useCustom,
        int downDirection,
        int upDirection,
        int rightDirection,
        int leftDirection,
        bool downFlip,
        bool upFlip,
        bool rightFlip,
        bool leftFlip)
    {
        serializedObject.FindProperty("useCustomFacingVisualMap").boolValue = useCustom;
        serializedObject.FindProperty("animatorDirectionWhenFacingDown").intValue = downDirection;
        serializedObject.FindProperty("animatorDirectionWhenFacingUp").intValue = upDirection;
        serializedObject.FindProperty("animatorDirectionWhenFacingRight").intValue = rightDirection;
        serializedObject.FindProperty("animatorDirectionWhenFacingLeft").intValue = leftDirection;
        serializedObject.FindProperty("flipXWhenFacingDown").boolValue = downFlip;
        serializedObject.FindProperty("flipXWhenFacingUp").boolValue = upFlip;
        serializedObject.FindProperty("flipXWhenFacingRight").boolValue = rightFlip;
        serializedObject.FindProperty("flipXWhenFacingLeft").boolValue = leftFlip;
    }

    private static void ApplyRoamProfile(NPCAutoRoamController roamController, NPCRoamProfile roamProfile)
    {
        SerializedObject serializedObject = new SerializedObject(roamController);
        serializedObject.FindProperty("roamProfile").objectReferenceValue = roamProfile;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static NPCRoamProfile EnsureRoamProfile(
        string assetPath,
        float moveSpeed,
        float moveAnimationSpeed,
        float activityRadius,
        float minimumMoveDistance,
        float shortPauseMin,
        float shortPauseMax,
        float longPauseMin,
        float longPauseMax)
    {
        NPCRoamProfile profile = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(assetPath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<NPCRoamProfile>();
            AssetDatabase.CreateAsset(profile, assetPath);
        }

        SerializedObject serializedObject = new SerializedObject(profile);
        serializedObject.FindProperty("moveSpeed").floatValue = moveSpeed;
        serializedObject.FindProperty("idleAnimationSpeed").floatValue = 1f;
        serializedObject.FindProperty("moveAnimationSpeed").floatValue = moveAnimationSpeed;
        serializedObject.FindProperty("activityRadius").floatValue = activityRadius;
        serializedObject.FindProperty("minimumMoveDistance").floatValue = minimumMoveDistance;
        serializedObject.FindProperty("pathSampleAttempts").intValue = 12;
        serializedObject.FindProperty("shortPauseMin").floatValue = shortPauseMin;
        serializedObject.FindProperty("shortPauseMax").floatValue = shortPauseMax;
        serializedObject.FindProperty("shortPauseCountMin").intValue = 2;
        serializedObject.FindProperty("shortPauseCountMax").intValue = 4;
        serializedObject.FindProperty("longPauseMin").floatValue = longPauseMin;
        serializedObject.FindProperty("longPauseMax").floatValue = longPauseMax;
        serializedObject.FindProperty("stuckCheckInterval").floatValue = 0.3f;
        serializedObject.FindProperty("stuckDistanceThreshold").floatValue = 0.08f;
        serializedObject.FindProperty("maxStuckRecoveries").intValue = 3;
        serializedObject.FindProperty("enableAmbientChat").boolValue = false;
        serializedObject.FindProperty("ambientChatRadius").floatValue = 0f;
        serializedObject.FindProperty("ambientChatChance").floatValue = 0f;
        serializedObject.FindProperty("ambientChatResponseDelay").floatValue = 0.85f;
        serializedObject.FindProperty("dialogueContentProfile").objectReferenceValue = null;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(profile);
        return profile;
    }

    private static string ResolveSortingLayerName()
    {
        string[] layerNames = SortingLayer.layers.Select(layer => layer.name).ToArray();
        if (layerNames.Contains("Layer 1"))
        {
            return "Layer 1";
        }

        return layerNames.Length > 0 ? layerNames[0] : "Default";
    }

    private static void EnsureAssetFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string parentFolder = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        string folderName = Path.GetFileName(folderPath);
        if (string.IsNullOrEmpty(parentFolder) || string.IsNullOrEmpty(folderName))
        {
            throw new InvalidOperationException($"无法创建资源目录：{folderPath}");
        }

        EnsureAssetFolder(parentFolder);
        AssetDatabase.CreateFolder(parentFolder, folderName);
    }
}
