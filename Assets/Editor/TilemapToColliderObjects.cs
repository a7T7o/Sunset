using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// Tilemap 转碰撞物体工具
/// 将 Tilemap 中的每个 Tile 转成同位置的 GameObject，并可附带 SpriteRenderer / Collider2D。
/// </summary>
public class TilemapToColliderObjects : EditorWindow
{
    public enum GenerationMode
    {
        PerTileObjects,
        VegetationClusters
    }

    private class ConversionRequest
    {
        public Tilemap Tilemap;
        public BoundsInt Bounds;
        public string SourceLabel;
    }

    private struct OccupiedTileData
    {
        public Vector3Int CellPosition;
        public Sprite Sprite;
    }

    private struct ClusterFrontierNode
    {
        public Vector3Int CellPosition;
        public int ClusterId;
        public int Distance;
    }

    private class VegetationCluster
    {
        public int ClusterId;
        public readonly List<OccupiedTileData> Tiles = new List<OccupiedTileData>();
        public readonly List<Vector3Int> AnchorCells = new List<Vector3Int>();
        public int MinX = int.MaxValue;
        public int MaxX = int.MinValue;
        public int MinY = int.MaxValue;
        public int MaxY = int.MinValue;

        public void AddTile(OccupiedTileData tileData)
        {
            Tiles.Add(tileData);
            MinX = Mathf.Min(MinX, tileData.CellPosition.x);
            MaxX = Mathf.Max(MaxX, tileData.CellPosition.x);
            MinY = Mathf.Min(MinY, tileData.CellPosition.y);
            MaxY = Mathf.Max(MaxY, tileData.CellPosition.y);
        }

        public void AddAnchor(Vector3Int cellPosition)
        {
            AnchorCells.Add(cellPosition);
            MinX = Mathf.Min(MinX, cellPosition.x);
            MaxX = Mathf.Max(MaxX, cellPosition.x);
            MinY = Mathf.Min(MinY, cellPosition.y);
            MaxY = Mathf.Max(MaxY, cellPosition.y);
        }
    }

    public enum ColliderMode
    {
        BoxCollider2D,
        PolygonCollider2D
    }

    private readonly List<Tilemap> selectedTilemaps = new List<Tilemap>();
    private static readonly string[] GenerationModeLabels = { "逐格物体", "植被整体对象" };
    private static readonly Vector3Int[] AnchorNeighborOffsets =
    {
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.up,
        Vector3Int.down
    };
    private static readonly Vector3Int[] VegetationNeighborOffsets =
    {
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.up,
        Vector3Int.down,
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, -1, 0)
    };

    private bool useCurrentGridSelection = true;
    private Tilemap gridSelectionTilemap;
    private BoundsInt gridSelectionBounds;
    private Transform customParent;
    private string containerSuffix = "_ConvertedObjects";
    private GenerationMode generationMode = GenerationMode.PerTileObjects;
    private bool createSpriteRenderer = true;
    private bool createColliders = true;
    private bool copyTileColor = true;
    private bool copySortingFromTilemap = true;
    private ColliderMode colliderMode = ColliderMode.PolygonCollider2D;
    private bool colliderIsTrigger = false;
    private bool addRigidbody2D = false;
    private RigidbodyType2D rigidbodyType = RigidbodyType2D.Static;
    private bool clearSourceTilesAfterConversion = false;
    private bool disableTilemapRendererAfterConversion = false;
    private bool reuseExistingContainer = true;
    private int vegetationSortingOrderMultiplier = 100;
    private int vegetationSortingOrderOffset = 0;
    private int vegetationInternalOrderStep = 10;
    private Vector2 scrollPos;

    [MenuItem("Tools/Tilemap转碰撞物体工具")]
    public static void ShowWindow()
    {
        var window = GetWindow<TilemapToColliderObjects>("Tilemap转碰撞物体");
        window.minSize = new Vector2(420f, 620f);
    }

    public static void OpenForCurrentGridSelection()
    {
        var window = GetWindow<TilemapToColliderObjects>("Tilemap转碰撞物体");
        window.minSize = new Vector2(420f, 620f);
        window.useCurrentGridSelection = true;
        window.CaptureCurrentGridSelection(false);
        window.Focus();
    }

    public static bool TryGetCurrentGridSelection(out Tilemap tilemap, out BoundsInt bounds)
    {
        tilemap = null;
        bounds = default;

        if (!GridSelection.active || GridSelection.target == null)
        {
            return false;
        }

        tilemap = GridSelection.target.GetComponent<Tilemap>();
        if (tilemap == null)
        {
            return false;
        }

        BoundsInt selectionBounds = GridSelection.position;
        Vector3Int size = selectionBounds.size;
        if (size.x <= 0 || size.y <= 0)
        {
            return false;
        }

        if (size.z <= 0)
        {
            size.z = 1;
        }

        bounds = new BoundsInt(selectionBounds.min, size);
        return true;
    }

    public static int CountOccupiedCells(Tilemap tilemap, BoundsInt bounds)
    {
        return CountOccupiedCellsInBounds(tilemap, bounds);
    }

    public static string DescribeBounds(BoundsInt bounds)
    {
        return FormatBounds(bounds);
    }

    public static void RunGridSelectionConversion(
        GenerationMode generationMode,
        bool createSpriteRenderer,
        bool createColliders,
        bool copyTileColor,
        bool copySortingFromTilemap,
        ColliderMode colliderMode,
        bool colliderIsTrigger,
        bool addRigidbody2D,
        RigidbodyType2D rigidbodyType,
        bool clearSourceTilesAfterConversion,
        bool disableTilemapRendererAfterConversion,
        bool reuseExistingContainer,
        int vegetationSortingOrderMultiplier,
        int vegetationSortingOrderOffset,
        int vegetationInternalOrderStep,
        Transform customParent,
        string containerSuffix)
    {
        var converter = CreateInstance<TilemapToColliderObjects>();
        try
        {
            converter.useCurrentGridSelection = true;
            converter.generationMode = generationMode;
            converter.createSpriteRenderer = createSpriteRenderer;
            converter.createColliders = createColliders;
            converter.copyTileColor = copyTileColor;
            converter.copySortingFromTilemap = copySortingFromTilemap;
            converter.colliderMode = colliderMode;
            converter.colliderIsTrigger = colliderIsTrigger;
            converter.addRigidbody2D = addRigidbody2D;
            converter.rigidbodyType = rigidbodyType;
            converter.clearSourceTilesAfterConversion = clearSourceTilesAfterConversion;
            converter.disableTilemapRendererAfterConversion = disableTilemapRendererAfterConversion;
            converter.reuseExistingContainer = reuseExistingContainer;
            converter.vegetationSortingOrderMultiplier = vegetationSortingOrderMultiplier;
            converter.vegetationSortingOrderOffset = vegetationSortingOrderOffset;
            converter.vegetationInternalOrderStep = vegetationInternalOrderStep;
            converter.customParent = customParent;
            converter.containerSuffix = string.IsNullOrWhiteSpace(containerSuffix) ? "_ConvertedObjects" : containerSuffix.Trim();
            converter.CaptureCurrentGridSelection(false);
            converter.ConvertSelectedTilemaps();
        }
        finally
        {
            DestroyImmediate(converter);
        }
    }

    private void OnGUI()
    {
        RefreshGridSelectionCacheIfNeeded();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Tilemap转碰撞物体工具", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "使用方法：\n" +
            "推荐工作流：\n" +
            "1. 在 Tile Palette / Tilemap 选择工具里框选目标区域\n" +
            "2. 保持“使用当前框选区域”开启\n" +
            "3. 选择生成模式（逐格 / 植被整体）\n" +
            "4. 直接点击“开始转换”\n\n" +
            "也兼容旧工作流：在 Hierarchy 选中 Tilemap 后手动抓取。\n" +
            "逐格模式会一格一个物体。\n" +
            "植被整体模式会把连在一起的 tile 切成一丛丛对象，并把整丛当一个排序单位。\n" +
            "碰撞体现在可以按需开关；你可以只生排序用对象，不带 Collider。\n" +
            "默认只新增物体，不会清空原 Tile；只有勾选“清空源 Tile”时才会改动当前场景内容。",
            MessageType.Info);

        EditorGUILayout.Space();

        DrawSourceSettings();
        DrawSeparator();
        DrawConversionSettings();
        DrawWarnings();
        DrawSeparator();
        DrawConvertButton();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSourceSettings()
    {
        GUILayout.Label("输入来源", EditorStyles.boldLabel);

        useCurrentGridSelection = EditorGUILayout.Toggle("使用当前框选区域", useCurrentGridSelection);

        if (useCurrentGridSelection)
        {
            if (GUILayout.Button("读取当前 GridSelection", GUILayout.Height(32f)))
            {
                CaptureCurrentGridSelection(true);
            }

            DrawGridSelectionInfo();
            return;
        }

        if (GUILayout.Button("获取选中的 Tilemap（从 Hierarchy）", GUILayout.Height(34f)))
        {
            GetSelectedTilemaps();
        }

        DrawTilemapSelection();
    }

    private void DrawGridSelectionInfo()
    {
        if (!HasCapturedGridSelection())
        {
            EditorGUILayout.HelpBox(
                "当前没有可用的 GridSelection。\n请先在 Tile Palette / Scene 中用框选工具选中 Tilemap 区域，再回来点“读取当前 GridSelection”。",
                MessageType.Warning);
            return;
        }

        EditorGUILayout.ObjectField("目标 Tilemap", gridSelectionTilemap, typeof(Tilemap), true);
        EditorGUILayout.LabelField("框选范围", FormatBounds(gridSelectionBounds));
        EditorGUILayout.LabelField("预计转换格子数", CountOccupiedCellsInBounds(gridSelectionTilemap, gridSelectionBounds).ToString());
        EditorGUILayout.HelpBox("当前将只处理框选范围内的非空 Tile，不会扫整张 Tilemap。", MessageType.Info);
    }

    private void DrawTilemapSelection()
    {
        selectedTilemaps.RemoveAll(tilemap => tilemap == null);

        if (selectedTilemaps.Count == 0)
        {
            EditorGUILayout.HelpBox("还没有获取到 Tilemap。", MessageType.Warning);
            return;
        }

        GUILayout.Label($"已选择 {selectedTilemaps.Count} 个 Tilemap：", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        foreach (Tilemap tilemap in selectedTilemaps)
        {
            EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), true);
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("预计转换格子数", CountOccupiedCells().ToString());

        if (GUILayout.Button("清空列表"))
        {
            selectedTilemaps.Clear();
        }
    }

    private void DrawConversionSettings()
    {
        GUILayout.Label("转换设置", EditorStyles.boldLabel);
        generationMode = (GenerationMode)EditorGUILayout.Popup("生成模式", (int)generationMode, GenerationModeLabels);

        customParent = (Transform)EditorGUILayout.ObjectField("自定义父物体（可选）", customParent, typeof(Transform), true);
        containerSuffix = EditorGUILayout.TextField("容器后缀", containerSuffix);
        reuseExistingContainer = EditorGUILayout.Toggle("复用同名容器", reuseExistingContainer);

        EditorGUILayout.Space();
        createSpriteRenderer = EditorGUILayout.Toggle("生成 SpriteRenderer", createSpriteRenderer);
        using (new EditorGUI.DisabledScope(!createSpriteRenderer))
        {
            copyTileColor = EditorGUILayout.Toggle("复制 Tile 颜色", copyTileColor);
            copySortingFromTilemap = EditorGUILayout.Toggle("复制 Tilemap 排序层", copySortingFromTilemap);
        }

        EditorGUILayout.Space();
        createColliders = EditorGUILayout.Toggle("生成碰撞体", createColliders);
        using (new EditorGUI.DisabledScope(!createColliders))
        {
            colliderMode = (ColliderMode)EditorGUILayout.EnumPopup("碰撞体类型", colliderMode);
            colliderIsTrigger = EditorGUILayout.Toggle("Collider Is Trigger", colliderIsTrigger);
        }

        using (new EditorGUI.DisabledScope(!createColliders))
        {
            addRigidbody2D = EditorGUILayout.Toggle("附加 Rigidbody2D", addRigidbody2D);
        }
        using (new EditorGUI.DisabledScope(!createColliders || !addRigidbody2D))
        {
            rigidbodyType = (RigidbodyType2D)EditorGUILayout.EnumPopup("Rigidbody 类型", rigidbodyType);
        }

        if (generationMode == GenerationMode.VegetationClusters)
        {
            EditorGUILayout.Space();
            vegetationSortingOrderMultiplier = EditorGUILayout.IntField("植被排序倍率", vegetationSortingOrderMultiplier);
            vegetationSortingOrderOffset = EditorGUILayout.IntField("植被排序偏移", vegetationSortingOrderOffset);
            vegetationInternalOrderStep = EditorGUILayout.IntField("组内行间距", vegetationInternalOrderStep);
            EditorGUILayout.HelpBox(
                "植被整体模式会先找底部锚点，再把相连的 tile 划成一丛丛对象。\n" +
                "每丛会生成一个根对象，并用 SortingGroup 作为整体排序单位；子物体仍保留逐 tile 的 Sprite / Collider。",
                MessageType.Info);
        }

        EditorGUILayout.Space();
        clearSourceTilesAfterConversion = EditorGUILayout.Toggle("清空源 Tile", clearSourceTilesAfterConversion);
        using (new EditorGUI.DisabledScope(!clearSourceTilesAfterConversion))
        {
            disableTilemapRendererAfterConversion = EditorGUILayout.Toggle("转换后关闭 TilemapRenderer", disableTilemapRendererAfterConversion);
        }
    }

    private void DrawWarnings()
    {
        EditorGUILayout.Space();

        if (!createSpriteRenderer)
        {
            EditorGUILayout.HelpBox("当前会生成“只有碰撞体”的物体，不会显示原 Tile 的画面。", MessageType.Info);
        }

        if (!createColliders)
        {
            EditorGUILayout.HelpBox("当前不会生成碰撞体，只会生成可见对象与排序结构。", MessageType.Info);
        }

        if (!createColliders && addRigidbody2D)
        {
            EditorGUILayout.HelpBox("碰撞体关闭时，Rigidbody2D 也会自动跳过，不会实际生成。", MessageType.Warning);
        }

        if (generationMode == GenerationMode.VegetationClusters)
        {
            EditorGUILayout.HelpBox(
                "植被整体模式更适合灌木、花丛、连续装饰这类“应该按一个整体排序”的内容。\n" +
                "如果你要的是每格都独立处理，请切回逐格模式。",
                MessageType.Info);
        }

        if (colliderMode == ColliderMode.PolygonCollider2D && !createSpriteRenderer)
        {
            EditorGUILayout.HelpBox("PolygonCollider2D 需要 SpriteRenderer.sprite 来生成轮廓；当前配置下会自动回退为 BoxCollider2D。", MessageType.Warning);
        }

        if (clearSourceTilesAfterConversion)
        {
            EditorGUILayout.HelpBox(
                "你已勾选“清空源 Tile”。这会直接修改当前场景中的 Tilemap 内容。\n" +
                "如果你只是想先试效果，建议先不要勾这个选项。",
                MessageType.Warning);
        }
    }

    private void DrawConvertButton()
    {
        GUI.enabled = HasAnyValidSource();
        if (GUILayout.Button("开始转换", GUILayout.Height(40f)))
        {
            ConvertSelectedTilemaps();
        }

        GUI.enabled = true;
    }

    private void DrawSeparator()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
    }

    private void GetSelectedTilemaps()
    {
        selectedTilemaps.Clear();

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请先在 Hierarchy 中选择 Tilemap 或其父物体。", "确定");
            return;
        }

        foreach (GameObject selectedObject in selectedObjects)
        {
            Tilemap selfTilemap = selectedObject.GetComponent<Tilemap>();
            if (selfTilemap != null && !selectedTilemaps.Contains(selfTilemap))
            {
                selectedTilemaps.Add(selfTilemap);
            }

            Tilemap[] childTilemaps = selectedObject.GetComponentsInChildren<Tilemap>(true);
            foreach (Tilemap childTilemap in childTilemaps)
            {
                if (!selectedTilemaps.Contains(childTilemap))
                {
                    selectedTilemaps.Add(childTilemap);
                }
            }
        }

        if (selectedTilemaps.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "选中的对象里没有 Tilemap。", "确定");
        }
        else
        {
            Debug.Log($"<color=green>[TilemapToColliderObjects] 已获取 {selectedTilemaps.Count} 个 Tilemap</color>");
        }
    }

    private int CountOccupiedCells()
    {
        int count = 0;

        foreach (ConversionRequest request in BuildConversionRequests())
        {
            count += CountOccupiedCellsInBounds(request.Tilemap, request.Bounds);
        }

        return count;
    }

    private void ConvertSelectedTilemaps()
    {
        List<ConversionRequest> requests = BuildConversionRequests();
        if (requests.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "当前没有可转换的 Tilemap 或框选区域。", "确定");
            return;
        }

        if (clearSourceTilesAfterConversion)
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "确认清空源 Tile",
                "当前配置会在生成物体后清空源 Tilemap 中对应的 Tile。\n\n确定继续吗？",
                "继续",
                "取消");

            if (!confirmed)
            {
                return;
            }
        }

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Tilemap 转碰撞物体");

        int createdObjectCount = 0;
        int createdVegetationGroupCount = 0;
        int createdContainerCount = 0;
        int skippedCount = 0;
        int clearedTileCount = 0;

        foreach (ConversionRequest request in requests)
        {
            Tilemap tilemap = request.Tilemap;
            if (tilemap == null)
            {
                continue;
            }

            TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
            bool createdNewContainer;
            Transform container = ResolveContainer(tilemap, ref createdContainerCount, out createdNewContainer);

            if (clearSourceTilesAfterConversion)
            {
                Undo.RecordObject(tilemap, "Clear Tilemap Tiles");
            }

            if (disableTilemapRendererAfterConversion && tilemapRenderer != null)
            {
                Undo.RecordObject(tilemapRenderer, "Disable Tilemap Renderer");
            }

            bool sceneChanged = createdNewContainer;

            if (generationMode == GenerationMode.VegetationClusters)
            {
                List<VegetationCluster> clusters = BuildVegetationClusters(tilemap, request.Bounds, out int skippedSpriteCount);
                skippedCount += skippedSpriteCount;

                foreach (VegetationCluster cluster in clusters)
                {
                    GameObject clusterRoot = CreateVegetationClusterRoot(tilemap, tilemapRenderer, container, cluster);
                    if (clusterRoot == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    createdVegetationGroupCount++;
                    sceneChanged = true;

                    foreach (OccupiedTileData tileData in cluster.Tiles)
                    {
                        ColliderMode actualColliderMode = GetActualColliderMode(tileData.Sprite);
                        int childSortingOrder = CalculateVegetationChildSortingOrder(cluster, tileData.CellPosition);
                        bool applySortingSettings = tilemapRenderer != null;
                        int sortingLayerId = tilemapRenderer != null ? tilemapRenderer.sortingLayerID : 0;

                        GameObject cellObject = CreateCellObject(
                            tilemap,
                            clusterRoot.transform,
                            tileData.CellPosition,
                            tileData.Sprite,
                            actualColliderMode,
                            applySortingSettings,
                            sortingLayerId,
                            childSortingOrder,
                            false);

                        if (cellObject == null)
                        {
                            skippedCount++;
                            continue;
                        }

                        createdObjectCount++;

                        if (clearSourceTilesAfterConversion)
                        {
                            tilemap.SetTile(tileData.CellPosition, null);
                            clearedTileCount++;
                        }
                    }

                    ApplyVegetationSorting(tilemapRenderer, clusterRoot);
                }
            }
            else
            {
                foreach (Vector3Int cellPosition in request.Bounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(cellPosition))
                    {
                        continue;
                    }

                    Sprite sprite = tilemap.GetSprite(cellPosition);
                    ColliderMode actualColliderMode = GetActualColliderMode(sprite);

                    if (sprite == null && createSpriteRenderer)
                    {
                        skippedCount++;
                        continue;
                    }

                    bool applySortingSettings = copySortingFromTilemap && tilemapRenderer != null;
                    int sortingLayerId = tilemapRenderer != null ? tilemapRenderer.sortingLayerID : 0;
                    int sortingOrder = tilemapRenderer != null ? tilemapRenderer.sortingOrder : 0;

                    GameObject cellObject = CreateCellObject(
                        tilemap,
                        container,
                        cellPosition,
                        sprite,
                        actualColliderMode,
                        applySortingSettings,
                        sortingLayerId,
                        sortingOrder,
                        addRigidbody2D);
                    if (cellObject == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    createdObjectCount++;
                    sceneChanged = true;

                    if (clearSourceTilesAfterConversion)
                    {
                        tilemap.SetTile(cellPosition, null);
                        clearedTileCount++;
                    }
                }
            }

            if (disableTilemapRendererAfterConversion && clearSourceTilesAfterConversion && tilemapRenderer != null)
            {
                tilemapRenderer.enabled = false;
                sceneChanged = true;
            }

            if (sceneChanged)
            {
                EditorSceneManager.MarkSceneDirty(tilemap.gameObject.scene);
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        string summary = generationMode == GenerationMode.VegetationClusters
            ?
            $"已生成 {createdVegetationGroupCount} 个植被整体对象\n" +
            $"已生成子物体 {createdObjectCount} 个\n" +
            $"新建容器 {createdContainerCount} 个\n" +
            $"跳过 {skippedCount} 格\n" +
            $"清空源 Tile {clearedTileCount} 格"
            :
            $"已生成 {createdObjectCount} 个物体\n" +
            $"新建容器 {createdContainerCount} 个\n" +
            $"跳过 {skippedCount} 格\n" +
            $"清空源 Tile {clearedTileCount} 格";

        EditorUtility.DisplayDialog("转换完成", summary, "确定");
        Debug.Log($"<color=green>[TilemapToColliderObjects]</color>\n{summary}");
    }

    private List<VegetationCluster> BuildVegetationClusters(Tilemap tilemap, BoundsInt bounds, out int skippedSpriteCount)
    {
        skippedSpriteCount = 0;

        var occupiedTiles = new Dictionary<Vector3Int, OccupiedTileData>();
        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(cellPosition))
            {
                continue;
            }

            Sprite sprite = tilemap.GetSprite(cellPosition);
            if (createSpriteRenderer && sprite == null)
            {
                skippedSpriteCount++;
                continue;
            }

            occupiedTiles[cellPosition] = new OccupiedTileData
            {
                CellPosition = cellPosition,
                Sprite = sprite
            };
        }

        if (occupiedTiles.Count == 0)
        {
            return new List<VegetationCluster>();
        }

        List<List<Vector3Int>> seedSets = BuildVegetationSeedSets(occupiedTiles);
        if (seedSets.Count == 0)
        {
            return BuildConnectedComponentClusters(occupiedTiles);
        }

        seedSets.Sort(CompareSeedSets);

        var assignments = new Dictionary<Vector3Int, int>();
        var distances = new Dictionary<Vector3Int, int>();
        var queue = new Queue<ClusterFrontierNode>();

        for (int seedIndex = 0; seedIndex < seedSets.Count; seedIndex++)
        {
            List<Vector3Int> seedSet = seedSets[seedIndex];
            foreach (Vector3Int seedCell in seedSet)
            {
                assignments[seedCell] = seedIndex;
                distances[seedCell] = 0;
                queue.Enqueue(new ClusterFrontierNode
                {
                    CellPosition = seedCell,
                    ClusterId = seedIndex,
                    Distance = 0
                });
            }
        }

        while (queue.Count > 0)
        {
            ClusterFrontierNode node = queue.Dequeue();
            foreach (Vector3Int offset in VegetationNeighborOffsets)
            {
                Vector3Int neighbor = node.CellPosition + offset;
                if (!occupiedTiles.ContainsKey(neighbor))
                {
                    continue;
                }

                int nextDistance = node.Distance + 1;
                if (!distances.TryGetValue(neighbor, out int currentDistance) || nextDistance < currentDistance)
                {
                    distances[neighbor] = nextDistance;
                    assignments[neighbor] = node.ClusterId;
                    queue.Enqueue(new ClusterFrontierNode
                    {
                        CellPosition = neighbor,
                        ClusterId = node.ClusterId,
                        Distance = nextDistance
                    });
                }
            }
        }

        if (assignments.Count < occupiedTiles.Count)
        {
            return BuildConnectedComponentClusters(occupiedTiles);
        }

        var clustersById = new Dictionary<int, VegetationCluster>();
        foreach (KeyValuePair<Vector3Int, OccupiedTileData> pair in occupiedTiles)
        {
            int clusterId = assignments[pair.Key];
            if (!clustersById.TryGetValue(clusterId, out VegetationCluster cluster))
            {
                cluster = new VegetationCluster { ClusterId = clusterId };
                clustersById.Add(clusterId, cluster);
            }

            cluster.AddTile(pair.Value);
        }

        for (int seedIndex = 0; seedIndex < seedSets.Count; seedIndex++)
        {
            if (!clustersById.TryGetValue(seedIndex, out VegetationCluster cluster))
            {
                continue;
            }

            foreach (Vector3Int anchorCell in seedSets[seedIndex])
            {
                cluster.AddAnchor(anchorCell);
            }

            cluster.Tiles.Sort(CompareOccupiedTilesForCluster);
            cluster.AnchorCells.Sort(CompareCellsLeftToRightThenBottomUp);
        }

        List<VegetationCluster> clusters = new List<VegetationCluster>(clustersById.Values);
        clusters.Sort(CompareVegetationClusters);
        return clusters;
    }

    private List<List<Vector3Int>> BuildVegetationSeedSets(Dictionary<Vector3Int, OccupiedTileData> occupiedTiles)
    {
        var anchorCells = new HashSet<Vector3Int>();
        foreach (Vector3Int cellPosition in occupiedTiles.Keys)
        {
            if (!occupiedTiles.ContainsKey(cellPosition + Vector3Int.down))
            {
                anchorCells.Add(cellPosition);
            }
        }

        if (anchorCells.Count == 0)
        {
            return new List<List<Vector3Int>>();
        }

        var orderedAnchors = new List<Vector3Int>(anchorCells);
        orderedAnchors.Sort(CompareCellsLeftToRightThenBottomUp);

        var visited = new HashSet<Vector3Int>();
        var seedSets = new List<List<Vector3Int>>();
        foreach (Vector3Int anchorCell in orderedAnchors)
        {
            if (visited.Contains(anchorCell))
            {
                continue;
            }

            var component = new List<Vector3Int>();
            var queue = new Queue<Vector3Int>();
            queue.Enqueue(anchorCell);
            visited.Add(anchorCell);

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();
                component.Add(current);

                foreach (Vector3Int offset in AnchorNeighborOffsets)
                {
                    Vector3Int neighbor = current + offset;
                    if (!anchorCells.Contains(neighbor) || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            component.Sort(CompareCellsLeftToRightThenBottomUp);
            seedSets.Add(component);
        }

        return seedSets;
    }

    private List<VegetationCluster> BuildConnectedComponentClusters(Dictionary<Vector3Int, OccupiedTileData> occupiedTiles)
    {
        var orderedCells = new List<Vector3Int>(occupiedTiles.Keys);
        orderedCells.Sort(CompareCellsLeftToRightThenBottomUp);

        var visited = new HashSet<Vector3Int>();
        var clusters = new List<VegetationCluster>();

        foreach (Vector3Int startCell in orderedCells)
        {
            if (visited.Contains(startCell))
            {
                continue;
            }

            var cluster = new VegetationCluster { ClusterId = clusters.Count };
            var queue = new Queue<Vector3Int>();
            queue.Enqueue(startCell);
            visited.Add(startCell);

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();
                cluster.AddTile(occupiedTiles[current]);

                if (!occupiedTiles.ContainsKey(current + Vector3Int.down))
                {
                    cluster.AddAnchor(current);
                }

                foreach (Vector3Int offset in VegetationNeighborOffsets)
                {
                    Vector3Int neighbor = current + offset;
                    if (!occupiedTiles.ContainsKey(neighbor) || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            cluster.Tiles.Sort(CompareOccupiedTilesForCluster);
            cluster.AnchorCells.Sort(CompareCellsLeftToRightThenBottomUp);
            clusters.Add(cluster);
        }

        clusters.Sort(CompareVegetationClusters);
        return clusters;
    }

    private GameObject CreateVegetationClusterRoot(Tilemap tilemap, TilemapRenderer tilemapRenderer, Transform container, VegetationCluster cluster)
    {
        GameObject root = new GameObject(BuildVegetationClusterName(tilemap, cluster));
        Undo.RegisterCreatedObjectUndo(root, "Create Vegetation Cluster Root");

        if (container != null)
        {
            Undo.SetTransformParent(root.transform, container, "Parent Vegetation Cluster Root");
        }

        root.layer = tilemap.gameObject.layer;
        root.tag = tilemap.gameObject.tag;
        root.transform.position = CalculateVegetationRootPosition(tilemap, cluster);
        root.transform.rotation = Quaternion.identity;
        root.transform.localScale = Vector3.one;

        if (createSpriteRenderer)
        {
            SortingGroup sortingGroup = Undo.AddComponent<SortingGroup>(root);
            if (tilemapRenderer != null)
            {
                sortingGroup.sortingLayerID = tilemapRenderer.sortingLayerID;
            }
        }

        if (addRigidbody2D && createColliders)
        {
            Rigidbody2D rigidbody2D = Undo.AddComponent<Rigidbody2D>(root);
            rigidbody2D.bodyType = rigidbodyType;
        }

        return root;
    }

    private void ApplyVegetationSorting(TilemapRenderer tilemapRenderer, GameObject clusterRoot)
    {
        if (!createSpriteRenderer)
        {
            return;
        }

        SortingGroup sortingGroup = clusterRoot.GetComponent<SortingGroup>();
        if (sortingGroup == null)
        {
            return;
        }

        if (tilemapRenderer != null)
        {
            sortingGroup.sortingLayerID = tilemapRenderer.sortingLayerID;
        }

        sortingGroup.sortingOrder = CalculateVegetationSortingOrder(clusterRoot.transform);
    }

    private int CalculateVegetationSortingOrder(Transform clusterRoot)
    {
        float sortingY = ResolveClusterSortingY(clusterRoot);
        return -Mathf.RoundToInt(sortingY * vegetationSortingOrderMultiplier) + vegetationSortingOrderOffset;
    }

    private static float ResolveClusterSortingY(Transform clusterRoot)
    {
        float minY = float.PositiveInfinity;
        bool foundRenderable = false;

        Collider2D[] colliders = clusterRoot.GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D collider in colliders)
        {
            minY = Mathf.Min(minY, collider.bounds.min.y);
            foundRenderable = true;
        }

        if (foundRenderable)
        {
            return minY;
        }

        SpriteRenderer[] renderers = clusterRoot.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer spriteRenderer in renderers)
        {
            if (spriteRenderer.sprite == null)
            {
                continue;
            }

            minY = Mathf.Min(minY, spriteRenderer.bounds.min.y);
            foundRenderable = true;
        }

        return foundRenderable ? minY : clusterRoot.position.y;
    }

    private int CalculateVegetationChildSortingOrder(VegetationCluster cluster, Vector3Int cellPosition)
    {
        return -(cellPosition.y - cluster.MinY) * vegetationInternalOrderStep;
    }

    private static string BuildVegetationClusterName(Tilemap tilemap, VegetationCluster cluster)
    {
        return $"{tilemap.gameObject.name}_Vegetation_{cluster.MinX}_{cluster.MinY}_{cluster.MaxX}_{cluster.MaxY}";
    }

    private static Vector3 CalculateVegetationRootPosition(Tilemap tilemap, VegetationCluster cluster)
    {
        List<Vector3Int> sourceCells = cluster.AnchorCells.Count > 0 ? cluster.AnchorCells : null;
        if (sourceCells == null)
        {
            sourceCells = new List<Vector3Int>();
            foreach (OccupiedTileData tileData in cluster.Tiles)
            {
                sourceCells.Add(tileData.CellPosition);
            }
        }

        Vector3 accumulated = Vector3.zero;
        for (int index = 0; index < sourceCells.Count; index++)
        {
            accumulated += tilemap.GetCellCenterWorld(sourceCells[index]);
        }

        return accumulated / Mathf.Max(1, sourceCells.Count);
    }

    private static int CompareOccupiedTilesForCluster(OccupiedTileData left, OccupiedTileData right)
    {
        int rowCompare = right.CellPosition.y.CompareTo(left.CellPosition.y);
        if (rowCompare != 0)
        {
            return rowCompare;
        }

        return left.CellPosition.x.CompareTo(right.CellPosition.x);
    }

    private static int CompareVegetationClusters(VegetationCluster left, VegetationCluster right)
    {
        int bottomCompare = left.MinY.CompareTo(right.MinY);
        if (bottomCompare != 0)
        {
            return bottomCompare;
        }

        return left.MinX.CompareTo(right.MinX);
    }

    private static int CompareCellsLeftToRightThenBottomUp(Vector3Int left, Vector3Int right)
    {
        int xCompare = left.x.CompareTo(right.x);
        if (xCompare != 0)
        {
            return xCompare;
        }

        return left.y.CompareTo(right.y);
    }

    private static int CompareSeedSets(List<Vector3Int> left, List<Vector3Int> right)
    {
        if (left.Count == 0 || right.Count == 0)
        {
            return left.Count.CompareTo(right.Count);
        }

        return CompareCellsLeftToRightThenBottomUp(left[0], right[0]);
    }

    private void RefreshGridSelectionCacheIfNeeded()
    {
        if (!useCurrentGridSelection)
        {
            return;
        }

        if (GridSelection.active && GridSelection.target != null)
        {
            CaptureCurrentGridSelection(false);
        }
    }

    private void CaptureCurrentGridSelection(bool showDialogOnFailure)
    {
        if (TryGetActiveGridSelection(out Tilemap tilemap, out BoundsInt bounds))
        {
            gridSelectionTilemap = tilemap;
            gridSelectionBounds = bounds;
            Repaint();
            return;
        }

        if (showDialogOnFailure)
        {
            EditorUtility.DisplayDialog(
                "提示",
                "没有读取到有效的 GridSelection。\n请先在 Tilemap 里框选区域，并确保目标对象就是 Tilemap。",
                "确定");
        }
    }

    private bool TryGetActiveGridSelection(out Tilemap tilemap, out BoundsInt bounds)
    {
        return TryGetCurrentGridSelection(out tilemap, out bounds);
    }

    private bool HasCapturedGridSelection()
    {
        return gridSelectionTilemap != null && gridSelectionBounds.size.x > 0 && gridSelectionBounds.size.y > 0;
    }

    private bool HasAnyValidSource()
    {
        return BuildConversionRequests().Count > 0;
    }

    private List<ConversionRequest> BuildConversionRequests()
    {
        var requests = new List<ConversionRequest>();

        if (useCurrentGridSelection)
        {
            if (HasCapturedGridSelection())
            {
                requests.Add(new ConversionRequest
                {
                    Tilemap = gridSelectionTilemap,
                    Bounds = gridSelectionBounds,
                    SourceLabel = "GridSelection"
                });
            }

            return requests;
        }

        selectedTilemaps.RemoveAll(tilemap => tilemap == null);
        foreach (Tilemap tilemap in selectedTilemaps)
        {
            tilemap.CompressBounds();
            requests.Add(new ConversionRequest
            {
                Tilemap = tilemap,
                Bounds = tilemap.cellBounds,
                SourceLabel = "Hierarchy"
            });
        }

        return requests;
    }

    private static int CountOccupiedCellsInBounds(Tilemap tilemap, BoundsInt bounds)
    {
        if (tilemap == null)
        {
            return 0;
        }

        int count = 0;
        foreach (Vector3Int cellPosition in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPosition))
            {
                count++;
            }
        }

        return count;
    }

    private static string FormatBounds(BoundsInt bounds)
    {
        return $"min=({bounds.xMin}, {bounds.yMin}, {bounds.zMin}) size=({bounds.size.x}, {bounds.size.y}, {bounds.size.z})";
    }

    private ColliderMode GetActualColliderMode(Sprite sprite)
    {
        if (colliderMode == ColliderMode.PolygonCollider2D && sprite == null)
        {
            return ColliderMode.BoxCollider2D;
        }

        return colliderMode;
    }

    private Transform ResolveContainer(Tilemap tilemap, ref int createdContainerCount, out bool createdNewContainer)
    {
        string containerName = tilemap.gameObject.name + containerSuffix;
        Scene scene = tilemap.gameObject.scene;
        createdNewContainer = false;

        if (customParent != null)
        {
            Transform existingChild = reuseExistingContainer ? customParent.Find(containerName) : null;
            if (existingChild != null)
            {
                return existingChild;
            }

            GameObject childContainer = new GameObject(containerName);
            Undo.RegisterCreatedObjectUndo(childContainer, "Create Tilemap Container");
            Undo.SetTransformParent(childContainer.transform, customParent, "Parent Tilemap Container");
            childContainer.transform.localPosition = Vector3.zero;
            childContainer.transform.localRotation = Quaternion.identity;
            childContainer.transform.localScale = Vector3.one;
            createdContainerCount++;
            createdNewContainer = true;
            return childContainer.transform;
        }

        if (reuseExistingContainer)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == containerName)
                {
                    return rootObject.transform;
                }
            }
        }

        GameObject container = new GameObject(containerName);
        Undo.RegisterCreatedObjectUndo(container, "Create Tilemap Container");
        SceneManager.MoveGameObjectToScene(container, scene);
        container.transform.position = Vector3.zero;
        container.transform.rotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;
        createdContainerCount++;
        createdNewContainer = true;
        return container.transform;
    }

    private GameObject CreateCellObject(
        Tilemap tilemap,
        Transform container,
        Vector3Int cellPosition,
        Sprite sprite,
        ColliderMode actualColliderMode,
        bool applySortingSettings,
        int sortingLayerId,
        int sortingOrder,
        bool createRigidbodyOnCell)
    {
        string objectName = BuildCellObjectName(tilemap, sprite, cellPosition);
        GameObject cellObject = new GameObject(objectName);

        Undo.RegisterCreatedObjectUndo(cellObject, "Create Tile Collider Object");
        if (container != null)
        {
            Undo.SetTransformParent(cellObject.transform, container, "Parent Tile Collider Object");
        }

        cellObject.layer = tilemap.gameObject.layer;
        cellObject.tag = tilemap.gameObject.tag;
        Matrix4x4 tileTransform = tilemap.GetTransformMatrix(cellPosition);
        cellObject.transform.position = tilemap.GetCellCenterWorld(cellPosition);
        cellObject.transform.rotation = tilemap.transform.rotation * ExtractRotation(tileTransform);
        cellObject.transform.localScale = GetScaleRelativeToParent(
            Vector3.Scale(tilemap.transform.lossyScale, ExtractScale(tileTransform)),
            container);

        if (createSpriteRenderer)
        {
            SpriteRenderer spriteRenderer = Undo.AddComponent<SpriteRenderer>(cellObject);
            spriteRenderer.sprite = sprite;

            if (copyTileColor)
            {
                spriteRenderer.color = tilemap.GetColor(cellPosition);
            }

            if (applySortingSettings)
            {
                spriteRenderer.sortingLayerID = sortingLayerId;
                spriteRenderer.sortingOrder = sortingOrder;
            }
        }

        if (createColliders)
        {
            switch (actualColliderMode)
            {
                case ColliderMode.PolygonCollider2D:
                {
                    PolygonCollider2D polygonCollider = Undo.AddComponent<PolygonCollider2D>(cellObject);
                    polygonCollider.isTrigger = colliderIsTrigger;
                    break;
                }
                default:
                {
                    BoxCollider2D boxCollider = Undo.AddComponent<BoxCollider2D>(cellObject);
                    boxCollider.isTrigger = colliderIsTrigger;

                    if (sprite != null)
                    {
                        boxCollider.size = sprite.bounds.size;
                        boxCollider.offset = sprite.bounds.center;
                    }
                    else
                    {
                        Vector3 cellSize = tilemap.layoutGrid.cellSize;
                        boxCollider.size = new Vector2(cellSize.x, cellSize.y);
                        boxCollider.offset = Vector2.zero;
                    }

                    break;
                }
            }
        }

        if (createRigidbodyOnCell && createColliders)
        {
            Rigidbody2D rigidbody2D = Undo.AddComponent<Rigidbody2D>(cellObject);
            rigidbody2D.bodyType = rigidbodyType;
        }

        return cellObject;
    }

    private static string BuildCellObjectName(Tilemap tilemap, Sprite sprite, Vector3Int cellPosition)
    {
        string spriteName = sprite != null ? sprite.name : "Tile";
        return $"{tilemap.gameObject.name}_{spriteName}_{cellPosition.x}_{cellPosition.y}";
    }

    private static Vector3 GetScaleRelativeToParent(Vector3 worldScale, Transform parent)
    {
        if (parent == null)
        {
            return worldScale;
        }

        Vector3 parentScale = parent.lossyScale;
        return new Vector3(
            SafeDivide(worldScale.x, parentScale.x),
            SafeDivide(worldScale.y, parentScale.y),
            SafeDivide(worldScale.z, parentScale.z));
    }

    private static float SafeDivide(float value, float divisor)
    {
        return Mathf.Approximately(divisor, 0f) ? value : value / divisor;
    }

    private static Vector3 ExtractScale(Matrix4x4 matrix)
    {
        Vector3 x = new Vector3(matrix.m00, matrix.m10, matrix.m20);
        Vector3 y = new Vector3(matrix.m01, matrix.m11, matrix.m21);
        Vector3 z = new Vector3(matrix.m02, matrix.m12, matrix.m22);
        return new Vector3(x.magnitude, y.magnitude, z.magnitude);
    }

    private static Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        Vector3 forward = new Vector3(matrix.m02, matrix.m12, matrix.m22).normalized;
        Vector3 upwards = new Vector3(matrix.m01, matrix.m11, matrix.m21).normalized;

        if (forward.sqrMagnitude < 0.0001f || upwards.sqrMagnitude < 0.0001f)
        {
            return Quaternion.identity;
        }

        return Quaternion.LookRotation(forward, upwards);
    }
}
