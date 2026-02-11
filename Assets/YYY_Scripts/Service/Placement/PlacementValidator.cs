using UnityEngine;
using System.Collections.Generic;
using FarmGame.Data;
using FarmGame.World;

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
    
    /// <summary>农田障碍物检测标签（不包含 Player）- 用于锄地/浇水/种植</summary>
    /// <remarks>
    /// 关键设计决策：
    /// - HasObstacle() 包含 Player 标签 → 放置箱子时不能压住玩家
    /// - HasFarmingObstacle() 不包含 Player 标签 → 玩家可以在脚下锄地
    /// </remarks>
    private static readonly string[] FarmingObstacleTags = new string[] { "Tree", "Rock", "Building" };
    
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
    public List<CellState> ValidateCells(Vector3 centerPosition, Vector2Int gridSize, Transform playerTransform)
    {
        var cellStates = new List<CellState>();
        var cellCenters = PlacementGridCalculator.GetOccupiedCellCenters(centerPosition, gridSize);
        var cellIndices = PlacementGridCalculator.GetOccupiedCellIndices(centerPosition, gridSize);
        
        for (int i = 0; i < cellCenters.Count; i++)
        {
            Vector3 cellCenter = cellCenters[i];
            Vector2Int cellIndex = cellIndices[i];
            
            // 验证单个格子
            var state = ValidateSingleCell(cellCenter, cellIndex, playerTransform);
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
    public CellState ValidateSingleCell(Vector3 cellCenter, Vector2Int cellIndex, Transform playerTransform)
    {
        // 检查 1：Layer 是否一致
        if (enableLayerCheck && IsLayerMismatch(cellCenter, playerTransform))
        {
            return new CellState(cellIndex, false, InvalidReason.LayerMismatch);
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
    /// </summary>
    /// <param name="cellCenter">格子中心世界坐标</param>
    /// <returns>true=有障碍物，false=无障碍物</returns>
    public static bool HasFarmingObstacle(Vector3 cellCenter)
    {
        // 1. 碰撞体检测（使用静态标签列表，不包含 Player）
        Vector2 boxSize = new Vector2(0.9f, 0.9f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, boxSize, 0f);
        
        foreach (var hit in hits)
        {
            if (HasAnyTagStatic(hit.transform, FarmingObstacleTags))
            {
                return true;
            }
        }
        
        // 2. 检测无碰撞体的树苗（Stage 0）
        if (HasTreeAtPositionStatic(cellCenter))
            return true;
        
        // 3. 检测无碰撞体的箱子
        if (HasChestAtPositionStatic(cellCenter))
            return true;
        
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
    /// </summary>
    private static bool HasTreeAtPositionStatic(Vector3 cellCenter)
    {
        Vector2Int checkCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);
        
        // 遍历场景中所有 TreeController
        var allTrees = Object.FindObjectsByType<TreeController>(FindObjectsSortMode.None);
        foreach (var tree in allTrees)
        {
            Vector3 treeRootPos = tree.transform.parent != null 
                ? tree.transform.parent.position 
                : tree.transform.position;
            
            Vector2Int treeCellIndex = PlacementGridCalculator.GetCellIndex(treeRootPos);
            
            if (checkCellIndex == treeCellIndex)
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 静态辅助方法：检查指定格子是否有箱子
    /// </summary>
    private static bool HasChestAtPositionStatic(Vector3 cellCenter)
    {
        Vector2Int checkCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);
        
        // 遍历场景中所有 ChestController
        var allChests = Object.FindObjectsByType<ChestController>(FindObjectsSortMode.None);
        foreach (var chest in allChests)
        {
            // 获取箱子的 Collider 来确定占用的格子
            var collider = chest.GetComponentInChildren<Collider2D>();
            if (collider != null)
            {
                Vector2Int chestCellIndex = PlacementGridCalculator.GetCellIndex(collider.bounds.center);
                if (checkCellIndex == chestCellIndex)
                    return true;
            }
            else
            {
                Vector2Int chestCellIndex = PlacementGridCalculator.GetCellIndex(chest.transform.position);
                if (checkCellIndex == chestCellIndex)
                    return true;
            }
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
        var baseState = ValidateSingleCell(position, Vector2Int.zero, playerTransform);
        if (!baseState.isValid)
            return baseState;
        
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
        // TODO: 与 FarmingSystem 集成
        return false;
    }
    
    /// <summary>
    /// 检查指定格子是否与已放置的树木重叠
    /// 使用格子索引比较，而不是距离检测
    /// </summary>
    public bool HasTreeAtPosition(Vector3 cellCenter, float checkRadius)
    {
        // 计算当前检测格子的索引
        Vector2Int checkCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);
        
        // 方法1：使用 Physics2D 检测有碰撞体的树木（Stage 1+）
        // 使用小范围检测，只检测格子内部
        Vector2 boxSize = new Vector2(0.4f, 0.4f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(cellCenter, boxSize, 0f);
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
        
        // 方法2：遍历场景中所有 TreeController，检查格子是否重叠
        var allTrees = Object.FindObjectsByType<TreeController>(FindObjectsSortMode.None);
        foreach (var tree in allTrees)
        {
            // 获取树木占用的格子索引（树苗是 1x1）
            Vector3 treeRootPos = tree.transform.parent != null 
                ? tree.transform.parent.position 
                : tree.transform.position;
            
            Vector2Int treeCellIndex = PlacementGridCalculator.GetCellIndex(treeRootPos);
            
            // 检查当前格子是否与树木占用的格子重叠
            if (checkCellIndex == treeCellIndex)
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
    /// 使用格子索引比较，而不是距离检测
    /// </summary>
    public bool HasChestAtPosition(Vector3 cellCenter, float checkRadius)
    {
        // 计算当前检测格子的索引
        Vector2Int checkCellIndex = PlacementGridCalculator.GetCellIndex(cellCenter);
        
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
        
        // 方法2：遍历场景中所有 ChestController，检查格子是否重叠
        var allChests = Object.FindObjectsByType<ChestController>(FindObjectsSortMode.None);
        foreach (var chest in allChests)
        {
            // 获取箱子占用的格子索引
            var chestCellIndices = GetChestOccupiedCellIndices(chest);
            
            // 检查当前格子是否与箱子占用的任何格子重叠
            foreach (var chestCellIndex in chestCellIndices)
            {
                if (checkCellIndex == chestCellIndex)
                    return true;
            }
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
