using UnityEngine;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.World;
using FarmGame.Farm;

/// <summary>
/// 放置验证器
/// 简化红色判定逻辑：只有两种情况显示红色
/// 1. Layer 不一致（玩家与放置位置不在同一楼层）
/// 2. 有障碍物（Tree、Rock、Building、Player 或水域）
/// 
/// 注意：距离不影响格子颜色！取消了"放置范围"概念
/// </summary>
public class PlacementValidator
{
    #region 配置参数
    
    /// <summary>障碍物检测标签（包含 Player）- 用于放置箱子/树苗等</summary>
    private string[] obstacleTags = new string[] { "Tree", "Rock", "Building", "Player" };

    private const float DefaultFarmingFootprintSize = 1.5f;
    private const float DefaultFarmingFootprintHalfExtent = DefaultFarmingFootprintSize * 0.5f;

    /// <summary>水域检测层</summary>
    private LayerMask waterLayer;
    
    /// <summary>是否启用 Layer 检测</summary>
    private bool enableLayerCheck = true;
    
    /// <summary>调试模式</summary>
    private bool showDebugInfo = false;
    
    #endregion
    
    #region 构造函数
    
    public PlacementValidator()
    {
        waterLayer = LayerMask.GetMask("Water");
    }
    
    #endregion
    
    #region 主验证方法
    
    /// <summary>
    /// 验证所有格子的状态
    /// 注意：此方法不检查距离，距离不影响格子颜色
    /// </summary>
    /// <param name="centerPosition">放置中心位置（方块中心）</param>
    /// <param name="gridSize">格子大小</param>
    /// <param name="playerTransform">玩家 Transform</param>
    /// <returns>每个格子的状态列表</returns>
    public List<CellState> ValidateCells(Vector3 centerPosition, Vector2Int gridSize, Transform playerTransform, ItemData placementItem = null)
    {
        var cellStates = new List<CellState>();
        var cellCenters = PlacementGridCalculator.GetOccupiedCellCenters(centerPosition, gridSize);
        var cellIndices = PlacementGridCalculator.GetOccupiedCellIndices(centerPosition, gridSize);
        bool forbidTilledFarmland = ShouldBlockTilledFarmland(placementItem);
        
        for (int i = 0; i < cellCenters.Count; i++)
        {
            Vector3 cellCenter = cellCenters[i];
            Vector2Int cellIndex = cellIndices[i];
            
            // 验证单个格子
            var state = ValidateSingleCell(cellCenter, cellIndex, playerTransform, forbidTilledFarmland);
            cellStates.Add(state);
            
            if (showDebugInfo && !state.isValid)
            {
                Debug.Log($"<color=red>[PlacementValidator] 格子 {cellIndex} 无效: {state.reason}</color>");
            }
        }
        
        return cellStates;
    }
    
    /// <summary>
    /// 验证单个格子
    /// </summary>
    public CellState ValidateSingleCell(Vector3 cellCenter, Vector2Int cellIndex, Transform playerTransform, bool forbidTilledFarmland = false)
    {
        // 检查 1：Layer 是否一致
        if (enableLayerCheck && IsLayerMismatch(cellCenter, playerTransform))
        {
            return new CellState(cellIndex, false, InvalidReason.LayerMismatch);
        }

        if (forbidTilledFarmland && IsOnFarmland(cellCenter))
        {
            return new CellState(cellIndex, false, InvalidReason.OnFarmland);
        }
        
        // 检查 2：是否有障碍物
        if (HasObstacle(cellCenter))
        {
            return new CellState(cellIndex, false, InvalidReason.HasObstacle);
        }
        
        // 检查 3：是否在水域
        if (IsOnWater(cellCenter))
        {
            return new CellState(cellIndex, false, InvalidReason.HasObstacle);
        }
        
        // 通过所有检查，格子有效
        return new CellState(cellIndex, true, InvalidReason.None);
    }
    
    /// <summary>
    /// 检查是否所有格子都有效
    /// </summary>
    public bool AreAllCellsValid(List<CellState> cellStates)
    {
        foreach (var state in cellStates)
        {
            if (!state.isValid)
                return false;
        }
        return true;
    }
    
    #endregion
    
    #region 红色判定（只有两种情况）
    
    /// <summary>
    /// 检查 Layer 是否不一致
    /// 红色情况 1：玩家与放置位置不在同一楼层
    /// </summary>
    public bool IsLayerMismatch(Vector3 position, Transform playerTransform)
    {
        if (playerTransform == null) return false;
        
        int positionLayer = PlacementLayerDetector.GetLayerAtPosition(position);
        int playerLayer = PlacementLayerDetector.GetPlayerLayer(playerTransform);
        
        return positionLayer != playerLayer;
    }
    
    /// <summary>
    /// 检查是否有障碍物（用于放置箱子/树苗等）
    /// 红色情况 2：有 Tree、Rock、Building、Player 或水域
    /// 增强：同时检测无碰撞体的树苗和箱子
    /// </summary>
    public bool HasObstacle(Vector3 cellCenter)
    {
        // 1. 原有的碰撞体检测
        if (obstacleTags != null && obstacleTags.Length > 0)
        {
            // 使用 OverlapBox 检测整个格子区域
            Vector2 boxSize = new Vector2(0.9f, 0.9f); // 略小于 1x1，避免边缘误检
            Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, boxSize, 0f);
            
            foreach (var hit in hits)
            {
                if (HasAnyTag(hit.transform, obstacleTags))
                {
                    return true;
                }
            }
        }
        
        // 2. 新增：检测无碰撞体的树苗（Stage 0）
        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager != null && farmTileManager.HasCropOccupantAtWorld(cellCenter))
            return true;

        if (HasTreeAtPosition(cellCenter, 0.5f))
            return true;
        
        // 3. 新增：检测无碰撞体的箱子
        if (HasChestAtPosition(cellCenter, 0.5f))
            return true;
        
        return false;
    }
    
    /// <summary>
    /// 🔥 检查是否有农田障碍物（用于锄地/浇水/种植）
    /// 关键区别：不检测 Player 标签！
    ///
    /// 设计原理：
    /// - 放置箱子时，箱子不能压住玩家 → HasObstacle() 包含 Player
    /// - 锄地时，玩家必然站在地里 → HasFarmingObstacle() 不包含 Player
    ///
    /// 配置来源：从 FarmTileManager 读取配置（可在 Inspector 中调整）
    /// </summary>
    /// <param name="cellCenter">格子中心世界坐标</param>
    /// <returns>true=有障碍物，false=无障碍物</returns>
    public static bool HasFarmingObstacle(Vector3 cellCenter, int layerIndex = -1, bool includeCropOccupant = true)
    {
        // 从 FarmTileManager 读取配置
        float footprintSize = FarmTileManager.FarmingObstacleCheckRadius;
        string[] obstacleTags = FarmTileManager.FarmingObstacleTags;
        string[] whitelistTags = FarmTileManager.FarmingWhitelistTags;
        bool requireGroundAround = FarmTileManager.RequireGroundAround;

        // 1. 碰撞体检测（使用格心锚定的 1.5 x 1.5 检测盒）
        // Physics2D.OverlapBoxAll 的第二个参数是完整 box size，不是 half extents。
        Vector2 boxSize = new Vector2(footprintSize, footprintSize);
        Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, boxSize, 0f);

        foreach (var hit in hits)
        {
            // 白名单检查：如果在白名单中，跳过
            if (whitelistTags.Length > 0 && HasAnyTagStatic(hit.transform, whitelistTags))
                continue;

            // 障碍物检查
            if (HasAnyTagStatic(hit.transform, obstacleTags))
            {
                return true;
            }
        }

        // 2. 检测无碰撞体的树苗（Stage 0）
        if (includeCropOccupant)
        {
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                if (layerIndex >= 0)
                {
                    var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
                    if (tilemaps != null)
                    {
                        Vector3Int cellPos = tilemaps.WorldToCell(cellCenter);
                        if (farmTileManager.HasCropOccupant(layerIndex, cellPos))
                            return true;
                    }
                }
                else if (farmTileManager.HasCropOccupantAtWorld(cellCenter))
                {
                    return true;
                }
            }
        }

        if (HasTreeAtPositionStatic(cellCenter))
            return true;

        // 3. 检测无碰撞体的箱子
        if (HasChestAtPositionStatic(cellCenter))
            return true;

        // 4. 可选：检查周围是否有地面（用于防止在悬空位置锄地）
        if (requireGroundAround)
        {
            // TODO: 实现地面检测逻辑
            // 可以使用 Physics2D.Raycast 向下检测地面
        }

        return false;
    }
    
    /// <summary>
    /// 静态辅助方法：检查 Transform 或其父级是否有指定标签
    /// </summary>
    private static bool HasAnyTagStatic(Transform t, string[] tags)
    {
        Transform current = t;
        while (current != null)
        {
            foreach (var tag in tags)
            {
                if (current.CompareTag(tag))
                    return true;
            }
            current = current.parent;
        }
        return false;
    }
    
    /// <summary>
    /// 静态辅助方法：检查指定格子是否有树木
    /// 使用 AABB 正方形距离检测（与 OverlapBoxAll 一致）
    /// </summary>
    private static bool HasTreeAtPositionStatic(Vector3 cellCenter)
    {
        Vector2Int targetCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);

        // 遍历场景中所有 TreeController
        var allTrees = Object.FindObjectsByType<TreeController>(FindObjectsSortMode.None);
        foreach (var tree in allTrees)
        {
            if (IsTreeOccupyingCell(tree, targetCellIndex))
                return true;
        }

        return false;
    }

    
    /// <summary>
    /// 静态辅助方法：检查指定格子是否有箱子
    /// 使用 AABB 正方形距离检测（与 OverlapBoxAll 一致）
    /// </summary>
    private static bool HasChestAtPositionStatic(Vector3 cellCenter)
    {
        float checkRadius = GetFarmingFootprintHalfExtent();
        
        // 遍历场景中所有 ChestController
        var allChests = Object.FindObjectsByType<ChestController>(FindObjectsSortMode.None);
        foreach (var chest in allChests)
        {
            // 获取箱子的 Collider 来确定中心位置
            var collider = chest.GetComponentInChildren<Collider2D>();
            Vector3 chestCenter;
            
            if (collider != null)
            {
                chestCenter = collider.bounds.center;
            }
            else
            {
                chestCenter = chest.transform.position;
            }
            
            // 使用 AABB 正方形距离判断（与 OverlapBoxAll 一致）
            float dx = Mathf.Abs(cellCenter.x - chestCenter.x);
            float dy = Mathf.Abs(cellCenter.y - chestCenter.y);
            
            if (dx <= checkRadius && dy <= checkRadius)
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 检查是否在水域
    /// </summary>
    public bool IsOnWater(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, waterLayer);
        return hit != null;
    }
    
    #endregion
    
    #region 树苗特殊验证
    
    /// <summary>
    /// 验证树苗放置
    /// </summary>
    public CellState ValidateSaplingPlacement(SaplingData sapling, Vector3 position, Transform playerTransform)
    {
        // 基础格子验证
        if (enableLayerCheck && IsLayerMismatch(position, playerTransform))
        {
            return new CellState(Vector2Int.zero, false, InvalidReason.LayerMismatch);
        }
        
        // 检查冬季
        if (sapling != null && sapling.IsWinter())
        {
            return new CellState(Vector2Int.zero, false, InvalidReason.WrongSeason);
        }
        
        // 检查是否在耕地上
        if (IsOnFarmland(position))
        {
            return new CellState(Vector2Int.zero, false, InvalidReason.OnFarmland);
        }

        if (HasObstacle(position))
        {
            return new CellState(Vector2Int.zero, false, InvalidReason.HasObstacle);
        }

        if (IsOnWater(position))
        {
            return new CellState(Vector2Int.zero, false, InvalidReason.HasObstacle);
        }
        
        // 检查成长边距（使用距离检测，树苗之间需要保持一定距离）
        if (sapling != null)
        {
            float vMargin, hMargin;
            if (!sapling.GetStage0Margins(out vMargin, out hMargin))
            {
                vMargin = 0.2f;
                hMargin = 0.15f;
            }
            
            if (HasTreeWithinDistance(position, Mathf.Max(vMargin, hMargin)))
            {
                return new CellState(Vector2Int.zero, false, InvalidReason.TreeTooClose);
            }
        }
        
        return new CellState(Vector2Int.zero, true, InvalidReason.None);
    }
    
    /// <summary>
    /// 检查是否在耕地上
    /// </summary>
    public bool IsOnFarmland(Vector3 position)
    {
        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null)
        {
            return false;
        }

        return farmTileManager.IsTilledAtWorld(position);
    }
    
    /// <summary>
    /// 检查指定格子是否与已放置的树木重叠
    /// 使用 AABB 正方形距离检测（与 OverlapBoxAll 一致）
    /// </summary>
    public bool HasTreeAtPosition(Vector3 cellCenter, float checkRadius)
    {
        Vector2Int targetCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);
        Vector3 targetCellCenter = PlacementGridCalculator.GetCellCenter(cellCenter);

        Vector2 boxSize = Vector2.one * Mathf.Clamp(checkRadius * 2f, 0.1f, 0.98f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetCellCenter, boxSize, 0f);
        foreach (var hit in hits)
        {
            var treeController = hit.GetComponentInParent<TreeController>();
            if (IsTreeOccupyingCell(treeController, targetCellIndex))
                return true;
        }

        var allTrees = Object.FindObjectsByType<TreeController>(FindObjectsSortMode.None);
        foreach (var tree in allTrees)
        {
            if (IsTreeOccupyingCell(tree, targetCellIndex))
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 检查边距内是否有其他树木（旧方法，保留兼容）
    /// </summary>
    [System.Obsolete("使用 HasTreeAtPosition 代替")]
    public bool HasTreeInMargin(Vector3 center, float vMargin, float hMargin)
    {
        return HasTreeWithinDistance(center, Mathf.Max(vMargin, hMargin));
    }
    
    /// <summary>
    /// 检查指定距离内是否有其他树木（用于树苗边距检测）
    /// 这个方法使用距离检测，专门用于树苗种植时的边距验证
    /// </summary>
    public bool HasTreeWithinDistance(Vector3 center, float distance)
    {
        // 方法1：使用 Physics2D 检测有碰撞体的树木（Stage 1+）
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, distance);
        foreach (var hit in hits)
        {
            var treeController = hit.GetComponentInParent<TreeController>();
            if (treeController != null)
                return true;
            
            // 兼容旧版 TreeController
            var oldTreeController = hit.GetComponentInParent<TreeController>();
            if (oldTreeController != null)
                return true;
        }
        
        // 方法2：遍历场景中所有 TreeController，检测树苗（Stage 0，无碰撞体）
        var allTrees = Object.FindObjectsByType<TreeController>(FindObjectsSortMode.None);
        foreach (var tree in allTrees)
        {
            // 计算树根位置（父物体位置）
            Vector3 treeRootPos = tree.transform.parent != null 
                ? tree.transform.parent.position 
                : tree.transform.position;
            
            // 检查距离
            float dist = Vector2.Distance(
                new Vector2(center.x, center.y),
                new Vector2(treeRootPos.x, treeRootPos.y)
            );
            
            if (dist < distance)
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 检查指定格子是否与已放置的箱子重叠
    /// 使用 AABB 正方形距离检测（与 OverlapBoxAll 一致）
    /// </summary>
    public bool HasChestAtPosition(Vector3 cellCenter, float checkRadius)
    {
        // 方法1：使用 Physics2D 检测有碰撞体的箱子
        // 使用小范围检测，只检测格子内部
        Vector2 boxSize = new Vector2(0.4f, 0.4f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, boxSize, 0f);
        foreach (var hit in hits)
        {
            var chestController = hit.GetComponentInParent<ChestController>();
            if (chestController != null)
                return true;
        }

        // 方法2：遍历场景中所有 ChestController，使用 AABB 正方形检测
        float aabbRadius = GetFarmingFootprintHalfExtent();
        var allChests = Object.FindObjectsByType<ChestController>(FindObjectsSortMode.None);
        foreach (var chest in allChests)
        {
            // 获取箱子的 Collider 来确定中心位置
            var collider = chest.GetComponentInChildren<Collider2D>();
            Vector3 chestCenter;

            if (collider != null)
            {
                chestCenter = collider.bounds.center;
            }
            else
            {
                chestCenter = chest.transform.position;
            }

            // 使用 AABB 正方形距离判断（与 OverlapBoxAll 一致）
            float dx = Mathf.Abs(cellCenter.x - chestCenter.x);
            float dy = Mathf.Abs(cellCenter.y - chestCenter.y);

            if (dx <= aabbRadius && dy <= aabbRadius)
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 获取箱子占用的所有格子索引
    /// 修复：使用 Collider 中心计算格子索引，而不是 bounds 边界
    /// 这样可以避免因底部对齐导致的边界偏移问题
    /// </summary>
    private List<Vector2Int> GetChestOccupiedCellIndices(ChestController chest)
    {
        var indices = new List<Vector2Int>();
        
        // 获取箱子的 Collider 来确定占用的格子
        var collider = chest.GetComponentInChildren<Collider2D>();
        if (collider != null)
        {
            // ★ 修复：使用 Collider 中心计算格子索引
            // 这样可以避免因底部对齐导致的边界偏移问题
            Bounds bounds = collider.bounds;
            Vector3 colliderCenter = bounds.center;
            
            // 计算 Collider 中心所在的格子索引
            Vector2Int centerCellIndex = PlacementGridCalculator.GetCellIndex(colliderCenter);
            
            // 计算 Collider 大小（向上取整）
            int gridWidth = Mathf.Max(1, Mathf.CeilToInt(bounds.size.x - 0.01f));
            int gridHeight = Mathf.Max(1, Mathf.CeilToInt(bounds.size.y - 0.01f));
            
            // 计算起始格子索引（以中心格子为锚点）
            int startX = centerCellIndex.x - (gridWidth - 1) / 2;
            int startY = centerCellIndex.y - (gridHeight - 1) / 2;
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    indices.Add(new Vector2Int(startX + x, startY + y));
                }
            }
        }
        else
        {
            // 没有 Collider，使用 transform.position 所在的格子
            Vector2Int cellIndex = PlacementGridCalculator.GetCellIndex(chest.transform.position);
            indices.Add(cellIndex);
        }
        
        return indices;
    }
    
    #endregion
    
    #region 辅助方法
    
    /// <summary>
    /// 检查 Transform 或其父级是否有指定标签
    /// </summary>
    private bool HasAnyTag(Transform t, string[] tags)
    {
        Transform current = t;
        while (current != null)
        {
            foreach (var tag in tags)
            {
                if (current.CompareTag(tag))
                    return true;
            }
            current = current.parent;
        }
        return false;
    }

    private static float GetFarmingFootprintHalfExtent()
    {
        float footprintSize = FarmTileManager.FarmingObstacleCheckRadius;
        if (footprintSize <= 0f)
        {
            return DefaultFarmingFootprintHalfExtent;
        }

        return footprintSize * 0.5f;
    }

    private static bool IsTreeOccupyingCell(TreeController tree, Vector2Int targetCellIndex)
    {
        if (tree == null)
        {
            return false;
        }

        Vector3 treeRootPos = tree.transform.parent != null
            ? tree.transform.parent.position
            : tree.transform.position;

        return PlacementGridCalculator.GetCellIndex(treeRootPos) == targetCellIndex;
    }
    
    #endregion
    
    #region 配置方法
    
    public void SetObstacleTags(string[] tags)
    {
        obstacleTags = tags;
    }
    
    public void SetEnableLayerCheck(bool enable)
    {
        enableLayerCheck = enable;
    }
    
    public void SetDebugMode(bool debug)
    {
        showDebugInfo = debug;
    }
    
    #endregion

    #region 普通 Placeable 规则

    private bool ShouldBlockTilledFarmland(ItemData placementItem)
    {
        return placementItem is PlaceableItemData && placementItem is not SaplingData;
    }

    #endregion
    
    #region 种子放置验证（补丁005 B.2.1）
    
    /// <summary>
    /// 种子专用放置验证：检查目标位置是否可以种植种子。
    /// layerIndex 内部通过 FarmTileManager.GetCurrentLayerIndex 获取（与 FarmToolPreview.UpdateSeedPreview 同源）。
    /// </summary>
    public static bool ValidateSeedPlacement(SeedData seedData, Vector3 worldPos)
    {
        if (seedData == null) return false;
        
        var farmTileManager = FarmTileManager.Instance;
        if (farmTileManager == null) return false;
        
        // 获取楼层索引（与 FarmToolPreview.UpdateSeedPreview 同源）
        if (!farmTileManager.TryResolveTileAtWorld(worldPos, out int layerIndex, out Vector3Int cellPos, out FarmTileData tileData))
            return false;
        
        
        
        // 1. 检查有耕地数据
        if (tileData == null) return false;
        
        // 2. 检查可种植
        if (!tileData.CanPlant()) return false;
        if (farmTileManager.HasCropOccupant(layerIndex, cellPos)) return false;
        
        // 3. 季节匹配检查
        if (seedData.season != Season.AllSeason && TimeManager.Instance != null)
        {
            var currentSeason = TimeManager.Instance.GetSeason();
            if ((int)seedData.season != (int)currentSeason) return false;
        }
        
        return true;
    }
    
    #endregion
}

/// <summary>
/// 格子状态（包含无效原因）
/// </summary>
public struct CellState
{
    public Vector2Int gridPosition;
    public bool isValid;
    public InvalidReason reason;
    
    public CellState(Vector2Int position, bool valid, InvalidReason invalidReason)
    {
        gridPosition = position;
        isValid = valid;
        reason = invalidReason;
    }
}

/// <summary>
/// 无效原因枚举
/// </summary>
public enum InvalidReason
{
    None,           // 有效
    LayerMismatch,  // Layer 不一致（红色情况 1）
    HasObstacle,    // 有障碍物（红色情况 2）
    WrongSeason,    // 季节不对（树苗专用）
    OnFarmland,     // 在耕地上（树苗专用）
    TreeTooClose    // 距离其他树木太近（树苗专用）
}
