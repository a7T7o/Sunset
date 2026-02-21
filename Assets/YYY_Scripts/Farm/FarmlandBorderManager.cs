using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmGame.Farm
{
    /// <summary>
    /// 耕地边界管理器
    /// 负责计算和更新耕地边界 Tile
    /// 
    /// 核心概念：
    /// - 中心块 (C)：玩家锄地创建的"真正的耕地"
    /// - 边界 Tile：围绕中心块的视觉装饰，不是耕地
    /// - 阴影 Tile：对角线方向有中心块时显示
    /// 
    /// 命名规则：
    /// - U/D/L/R 表示该方向有中心块
    /// - 边界内容贴着有中心块的那一侧
    /// </summary>
    public class FarmlandBorderManager : MonoBehaviour
    {
        #region 单例
        
        public static FarmlandBorderManager Instance { get; private set; }
        
        #endregion

        #region Tile 资源配置
        
        [Header("中心块")]
        [Tooltip("耕地中心块 Tile - 未施肥 (C0)")]
        [SerializeField] private TileBase centerTileUnfertilized;
        
        [Tooltip("耕地中心块 Tile - 已施肥 (C1)")]
        [SerializeField] private TileBase centerTileFertilized;
        
        [Header("单方向边界")]
        [Tooltip("上方有中心块，边界贴着顶部 (U)")]
        [SerializeField] private TileBase borderU;
        [Tooltip("下方有中心块，边界贴着底部 (D)")]
        [SerializeField] private TileBase borderD;
        [Tooltip("左方有中心块，边界贴着左侧 (L)")]
        [SerializeField] private TileBase borderL;
        [Tooltip("右方有中心块，边界贴着右侧 (R)")]
        [SerializeField] private TileBase borderR;
        
        [Header("双方向边界 - 对边")]
        [Tooltip("上下都有中心块，左右贯通 (UD)")]
        [SerializeField] private TileBase borderUD;
        [Tooltip("左右都有中心块，上下贯通 (LR)")]
        [SerializeField] private TileBase borderLR;
        
        [Header("双方向边界 - 相邻")]
        [Tooltip("上方和左方有中心块 (UL)")]
        [SerializeField] private TileBase borderUL;
        [Tooltip("上方和右方有中心块 (UR)")]
        [SerializeField] private TileBase borderUR;
        [Tooltip("下方和左方有中心块 (DL)")]
        [SerializeField] private TileBase borderDL;
        [Tooltip("下方和右方有中心块 (DR)")]
        [SerializeField] private TileBase borderDR;
        
        [Header("三方向边界")]
        [Tooltip("上下左都有中心块，只有右侧有边界线 (UDL)")]
        [SerializeField] private TileBase borderUDL;
        [Tooltip("上下右都有中心块，只有左侧有边界线 (UDR)")]
        [SerializeField] private TileBase borderUDR;
        [Tooltip("上左右都有中心块，只有底部有边界线 (ULR)")]
        [SerializeField] private TileBase borderULR;
        [Tooltip("下左右都有中心块，只有顶部有边界线 (DLR)")]
        [SerializeField] private TileBase borderDLR;
        
        [Header("四方向边界")]
        [Tooltip("四周都有中心块，无边界线 (UDLR)")]
        [SerializeField] private TileBase borderUDLR;
        
        [Header("角落阴影")]
        [Tooltip("左上角阴影 (SLU)")]
        [SerializeField] private TileBase shadowLU;
        [Tooltip("右上角阴影 (SRU)")]
        [SerializeField] private TileBase shadowRU;
        [Tooltip("左下角阴影 (SLD)")]
        [SerializeField] private TileBase shadowLD;
        [Tooltip("右下角阴影 (SRD)")]
        [SerializeField] private TileBase shadowRD;
        
        #endregion

        #region Debug
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion

        #region 生命周期
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        #endregion


        #region 公开接口
        
        /// <summary>
        /// 当中心块被放置时调用
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <param name="isFertilized">是否已施肥</param>
        public void OnCenterBlockPlaced(int layerIndex, Vector3Int cellPosition, bool isFertilized = false)
        {
            var tilemaps = FarmTileManager.Instance?.GetLayerTilemaps(layerIndex);
            if (tilemaps == null || !tilemaps.IsNewFarmlandSystemValid())
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[FarmlandBorderManager] 楼层 {layerIndex} 的新版耕地 Tilemap 未配置");
                return;
            }
            
            // 1. 放置中心块（根据施肥状态选择）
            TileBase centerTile = isFertilized ? centerTileFertilized : centerTileUnfertilized;
            tilemaps.farmlandCenterTilemap.SetTile(cellPosition, centerTile);
            
            // 2. 清除该位置的边界（如果有）
            tilemaps.farmlandBorderTilemap.SetTile(cellPosition, null);
            
            // 3. 更新周围边界
            UpdateBordersAround(layerIndex, cellPosition);
            
            if (showDebugInfo)
                Debug.Log($"[FarmlandBorderManager] 放置中心块: Layer={layerIndex}, Pos={cellPosition}, Fertilized={isFertilized}");
        }
        
        /// <summary>
        /// 更新中心块的施肥状态
        /// </summary>
        public void UpdateCenterBlockFertilized(int layerIndex, Vector3Int cellPosition, bool isFertilized)
        {
            var tilemaps = FarmTileManager.Instance?.GetLayerTilemaps(layerIndex);
            if (tilemaps == null || !tilemaps.IsNewFarmlandSystemValid())
            {
                return;
            }
            
            // 只更新中心块 Tile，不影响边界
            TileBase centerTile = isFertilized ? centerTileFertilized : centerTileUnfertilized;
            tilemaps.farmlandCenterTilemap.SetTile(cellPosition, centerTile);
        }
        
        /// <summary>
        /// 当中心块被移除时调用
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        public void OnCenterBlockRemoved(int layerIndex, Vector3Int cellPosition)
        {
            var tilemaps = FarmTileManager.Instance?.GetLayerTilemaps(layerIndex);
            if (tilemaps == null || !tilemaps.IsNewFarmlandSystemValid())
            {
                return;
            }
            
            // 1. 移除中心块
            tilemaps.farmlandCenterTilemap.SetTile(cellPosition, null);
            
            // 2. 更新周围边界（包括该位置本身，可能变成边界）
            UpdateBordersAround(layerIndex, cellPosition);
            
            // 3. 更新该位置本身（可能变成边界或阴影）
            UpdateBorderAt(layerIndex, cellPosition);
            
            if (showDebugInfo)
                Debug.Log($"[FarmlandBorderManager] 移除中心块: Layer={layerIndex}, Pos={cellPosition}");
        }
        
        #endregion

        #region 边界更新
        
        /// <summary>
        /// 更新指定位置周围 3×3 范围的边界
        /// </summary>
        public void UpdateBordersAround(int layerIndex, Vector3Int centerPosition)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // 跳过中心
                    
                    Vector3Int neighborPos = centerPosition + new Vector3Int(dx, dy, 0);
                    UpdateBorderAt(layerIndex, neighborPos);
                }
            }
        }
        
        /// <summary>
        /// 更新指定位置的边界 Tile（应用到真实 Tilemap）
        /// </summary>
        private void UpdateBorderAt(int layerIndex, Vector3Int position)
        {
            var tilemaps = FarmTileManager.Instance?.GetLayerTilemaps(layerIndex);
            if (tilemaps == null || !tilemaps.IsNewFarmlandSystemValid())
            {
                return;
            }
            
            // 计算边界 Tile
            TileBase tile = CalculateBorderTileAt(layerIndex, position, null);
            
            // 应用到 Tilemap
            ApplyBorderTile(tilemaps, position, tile);
        }
        
        /// <summary>
        /// 纯计算：计算指定位置应该显示的边界 Tile
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="position">格子坐标</param>
        /// <param name="isTilledPredicate">断言函数，用于预览计算（null = 使用真实数据）</param>
        /// <returns>应该显示的 Tile（可能为 null）</returns>
        public TileBase CalculateBorderTileAt(int layerIndex, Vector3Int position, System.Func<Vector3Int, bool> isTilledPredicate)
        {
            // 如果该位置是中心块，不需要边界
            if (IsCenterBlock(layerIndex, position, isTilledPredicate))
            {
                return null;
            }
            
            // 检查周围中心块分布
            var neighbors = CheckNeighborCenters(layerIndex, position, isTilledPredicate);
            
            // 选择边界 Tile
            TileBase borderTile = SelectBorderTile(
                neighbors.hasU, neighbors.hasD, neighbors.hasL, neighbors.hasR);
            
            if (borderTile != null)
            {
                return borderTile;
            }
            
            // 检查是否需要放置阴影
            return SelectShadowTile(
                neighbors.hasU, neighbors.hasD, neighbors.hasL, neighbors.hasR,
                neighbors.hasLU, neighbors.hasRU, neighbors.hasLD, neighbors.hasRD);
        }
        
        /// <summary>
        /// 副作用：将 Tile 应用到 Tilemap
        /// </summary>
        /// <param name="tilemaps">楼层 Tilemap 配置</param>
        /// <param name="position">格子坐标</param>
        /// <param name="tile">要设置的 Tile（null = 清除）</param>
        public void ApplyBorderTile(LayerTilemaps tilemaps, Vector3Int position, TileBase tile)
        {
            if (tilemaps == null || tilemaps.farmlandBorderTilemap == null) return;
            tilemaps.farmlandBorderTilemap.SetTile(position, tile);
        }
        
        #endregion
        
        #region 预览接口
        
        /// <summary>
        /// 获取预览 Tiles（用于 FarmToolPreview）
        /// 计算如果在 centerPos 锄地，中心块和周围 8 格边界会变成什么样
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="centerPos">预计锄地的位置</param>
        /// <returns>位置 → Tile 的映射（包含中心块和边界变化）</returns>
        public System.Collections.Generic.Dictionary<Vector3Int, TileBase> GetPreviewTiles(int layerIndex, Vector3Int centerPos)
        {
            var result = new System.Collections.Generic.Dictionary<Vector3Int, TileBase>();
            
            // 构造断言：假装 centerPos 已经被耕作
            System.Func<Vector3Int, bool> predicate = (pos) =>
            {
                // 如果是预览位置，返回 true（假装已耕作）
                if (pos == centerPos) return true;
                // 否则使用真实数据
                return IsCenterBlock(layerIndex, pos);
            };
            
            // 1. 中心块：使用未施肥的中心 Tile
            result[centerPos] = centerTileUnfertilized;
            
            // 2. 计算周围 8 格的边界变化
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // 跳过中心
                    
                    Vector3Int neighborPos = centerPos + new Vector3Int(dx, dy, 0);
                    TileBase tile = CalculateBorderTileAt(layerIndex, neighborPos, predicate);
                    
                    // 只记录有变化的位置（tile 可能为 null，表示清除）
                    result[neighborPos] = tile;
                }
            }
            
            return result;
        }

        /// <summary>
        /// 🔴 补丁004V3：计算预览 tile，支持额外已耕位置（队列预览位置）。
        /// predicate 扩展为：centerPos + additionalTilledPositions + 实际耕地。
        /// </summary>
        public System.Collections.Generic.Dictionary<Vector3Int, TileBase> GetPreviewTiles(
            int layerIndex, Vector3Int centerPos, System.Collections.Generic.HashSet<Vector3Int> additionalTilledPositions)
        {
            var result = new System.Collections.Generic.Dictionary<Vector3Int, TileBase>();

            // 构造断言：假装 centerPos + additionalTilledPositions 都已被耕作
            System.Func<Vector3Int, bool> predicate = (pos) =>
            {
                if (pos == centerPos) return true;
                if (additionalTilledPositions != null && additionalTilledPositions.Contains(pos)) return true;
                return IsCenterBlock(layerIndex, pos);
            };

            // 1. 中心块
            result[centerPos] = centerTileUnfertilized;

            // 2. 计算周围 8 格的边界变化
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector3Int neighborPos = centerPos + new Vector3Int(dx, dy, 0);
                    TileBase tile = CalculateBorderTileAt(layerIndex, neighborPos, predicate);
                    result[neighborPos] = tile;
                }
            }

            return result;
        }

        
        /// <summary>
        /// 获取中心块 Tile（用于预览）
        /// </summary>
        /// <param name="isFertilized">是否已施肥</param>
        public TileBase GetCenterTile(bool isFertilized = false)
        {
            return isFertilized ? centerTileFertilized : centerTileUnfertilized;
        }
        
        #endregion

        #region 中心块检测
        
        /// <summary>
        /// 检查指定位置是否是中心块（使用真实数据）
        /// </summary>
        private bool IsCenterBlock(int layerIndex, Vector3Int position)
        {
            var tileData = FarmTileManager.Instance?.GetTileData(layerIndex, position);
            return tileData != null && tileData.isTilled;
        }
        
        /// <summary>
        /// 检查指定位置是否是中心块（支持断言/预测）
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="position">格子坐标</param>
        /// <param name="isTilledPredicate">断言函数，用于预览计算（假装某位置已耕作）</param>
        private bool IsCenterBlock(int layerIndex, Vector3Int position, System.Func<Vector3Int, bool> isTilledPredicate)
        {
            if (isTilledPredicate != null)
            {
                return isTilledPredicate(position);
            }
            return IsCenterBlock(layerIndex, position);
        }
        
        /// <summary>
        /// 检查指定位置周围 8 个方向的中心块分布（使用真实数据）
        /// </summary>
        private (bool hasU, bool hasD, bool hasL, bool hasR, 
                 bool hasLU, bool hasRU, bool hasLD, bool hasRD) 
            CheckNeighborCenters(int layerIndex, Vector3Int position)
        {
            return CheckNeighborCenters(layerIndex, position, null);
        }
        
        /// <summary>
        /// 检查指定位置周围 8 个方向的中心块分布（支持断言/预测）
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="position">格子坐标</param>
        /// <param name="isTilledPredicate">断言函数，用于预览计算</param>
        private (bool hasU, bool hasD, bool hasL, bool hasR, 
                 bool hasLU, bool hasRU, bool hasLD, bool hasRD) 
            CheckNeighborCenters(int layerIndex, Vector3Int position, System.Func<Vector3Int, bool> isTilledPredicate)
        {
            return (
                hasU:  IsCenterBlock(layerIndex, position + new Vector3Int(0, 1, 0), isTilledPredicate),
                hasD:  IsCenterBlock(layerIndex, position + new Vector3Int(0, -1, 0), isTilledPredicate),
                hasL:  IsCenterBlock(layerIndex, position + new Vector3Int(-1, 0, 0), isTilledPredicate),
                hasR:  IsCenterBlock(layerIndex, position + new Vector3Int(1, 0, 0), isTilledPredicate),
                hasLU: IsCenterBlock(layerIndex, position + new Vector3Int(-1, 1, 0), isTilledPredicate),
                hasRU: IsCenterBlock(layerIndex, position + new Vector3Int(1, 1, 0), isTilledPredicate),
                hasLD: IsCenterBlock(layerIndex, position + new Vector3Int(-1, -1, 0), isTilledPredicate),
                hasRD: IsCenterBlock(layerIndex, position + new Vector3Int(1, -1, 0), isTilledPredicate)
            );
        }
        
        #endregion


        #region 边界选择算法
        
        /// <summary>
        /// 根据周围中心块分布选择边界 Tile
        /// </summary>
        // 🔴 补丁004V2 模块L（CP-L2）：改为 public，增量差集计算需要从差集方向生成增量 tile
        public TileBase SelectBorderTile(bool hasU, bool hasD, bool hasL, bool hasR)
        {
            int count = (hasU ? 1 : 0) + (hasD ? 1 : 0) + (hasL ? 1 : 0) + (hasR ? 1 : 0);
            
            switch (count)
            {
                case 0:
                    return null; // 无边界，可能是阴影
                    
                case 1:
                    if (hasU) return borderU;
                    if (hasD) return borderD;
                    if (hasL) return borderL;
                    if (hasR) return borderR;
                    break;
                    
                case 2:
                    // 对边
                    if (hasU && hasD) return borderUD;
                    if (hasL && hasR) return borderLR;
                    // 相邻
                    if (hasU && hasL) return borderUL;
                    if (hasU && hasR) return borderUR;
                    if (hasD && hasL) return borderDL;
                    if (hasD && hasR) return borderDR;
                    break;
                    
                case 3:
                    if (!hasR) return borderUDL;
                    if (!hasL) return borderUDR;
                    if (!hasD) return borderULR;
                    if (!hasU) return borderDLR;
                    break;
                    
                case 4:
                    return borderUDLR;
            }
            
            return null;
        }
        
        /// <summary>
        /// 🔴 补丁004V2 模块L（CP-L5）：将 tile 引用解析为方向四元组
        /// </summary>
        public (bool hasU, bool hasD, bool hasL, bool hasR) ParseDirections(TileBase tile)
        {
            if (tile == null) return (false, false, false, false);
            // 单方向
            if (tile == borderU) return (true, false, false, false);
            if (tile == borderD) return (false, true, false, false);
            if (tile == borderL) return (false, false, true, false);
            if (tile == borderR) return (false, false, false, true);
            // 双方向对边
            if (tile == borderUD) return (true, true, false, false);
            if (tile == borderLR) return (false, false, true, true);
            // 双方向相邻
            if (tile == borderUL) return (true, false, true, false);
            if (tile == borderUR) return (true, false, false, true);
            if (tile == borderDL) return (false, true, true, false);
            if (tile == borderDR) return (false, true, false, true);
            // 三方向
            if (tile == borderUDL) return (true, true, true, false);
            if (tile == borderUDR) return (true, true, false, true);
            if (tile == borderULR) return (true, false, true, true);
            if (tile == borderDLR) return (false, true, true, true);
            // 四方向
            if (tile == borderUDLR) return (true, true, true, true);
            return (false, false, false, false);
        }
        
        /// <summary>
        /// 🔴 补丁004V2 模块L（CP-L1）：判断 tile 是否为边界 tile
        /// </summary>
        public bool IsBorderTile(TileBase tile)
        {
            if (tile == null) return false;
            var dirs = ParseDirections(tile);
            return dirs.hasU || dirs.hasD || dirs.hasL || dirs.hasR;
        }
        
        /// <summary>
        /// 🔴 补丁004V2 模块L（CP-L3）：判断 tile 是否为阴影 tile
        /// </summary>
        public bool IsShadowTile(TileBase tile)
        {
            return tile == shadowLU || tile == shadowRU || tile == shadowLD || tile == shadowRD;
        }
        
        /// <summary>
        /// 选择阴影 Tile（只有四个正方向都没有中心块时才检查）
        /// </summary>
        private TileBase SelectShadowTile(bool hasU, bool hasD, bool hasL, bool hasR,
                                          bool hasLU, bool hasRU, bool hasLD, bool hasRD)
        {
            // 只有四个正方向都没有中心块时，才放置阴影
            if (hasU || hasD || hasL || hasR)
            {
                return null;
            }
            
            // 检查对角线，优先级：左上 > 右上 > 左下 > 右下
            if (hasLU) return shadowLU;
            if (hasRU) return shadowRU;
            if (hasLD) return shadowLD;
            if (hasRD) return shadowRD;
            
            return null;
        }
        
        #endregion

        #region 批量操作
        
        /// <summary>
        /// 刷新指定楼层的所有边界
        /// </summary>
        public void RefreshAllBorders(int layerIndex)
        {
            var tilemaps = FarmTileManager.Instance?.GetLayerTilemaps(layerIndex);
            if (tilemaps == null || !tilemaps.IsNewFarmlandSystemValid())
            {
                return;
            }
            
            // 清除所有边界
            tilemaps.farmlandBorderTilemap.ClearAllTiles();
            
            // 收集所有中心块位置
            var centerPositions = new System.Collections.Generic.HashSet<Vector3Int>();
            foreach (var tileData in FarmTileManager.Instance.GetAllTilesInLayer(layerIndex))
            {
                if (tileData.isTilled)
                {
                    centerPositions.Add(tileData.position);
                    
                    // 确保中心块 Tile 存在（根据施肥状态选择）
                    // TODO: 需要从 tileData 获取施肥状态
                    TileBase centerTile = centerTileUnfertilized; // 默认未施肥
                    tilemaps.farmlandCenterTilemap.SetTile(tileData.position, centerTile);
                }
            }
            
            // 收集所有需要更新的边界位置
            var borderPositions = new System.Collections.Generic.HashSet<Vector3Int>();
            foreach (var centerPos in centerPositions)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        
                        Vector3Int neighborPos = centerPos + new Vector3Int(dx, dy, 0);
                        if (!centerPositions.Contains(neighborPos))
                        {
                            borderPositions.Add(neighborPos);
                        }
                    }
                }
            }
            
            // 更新所有边界
            foreach (var borderPos in borderPositions)
            {
                UpdateBorderAt(layerIndex, borderPos);
            }
            
            if (showDebugInfo)
                Debug.Log($"[FarmlandBorderManager] 刷新楼层 {layerIndex} 边界: {centerPositions.Count} 个中心块, {borderPositions.Count} 个边界位置");
        }
        
        /// <summary>
        /// 刷新所有楼层的边界
        /// </summary>
        public void RefreshAllLayersBorders()
        {
            if (FarmTileManager.Instance == null) return;
            
            for (int i = 0; i < FarmTileManager.Instance.LayerCount; i++)
            {
                RefreshAllBorders(i);
            }
        }
        
        #endregion

        #region 调试
        
        /// <summary>
        /// 验证 Tile 资源配置
        /// </summary>
        public bool ValidateTileResources()
        {
            bool isValid = true;
            
            if (centerTileUnfertilized == null) { Debug.LogError("[FarmlandBorderManager] centerTileUnfertilized (C0) 未配置"); isValid = false; }
            if (centerTileFertilized == null) { Debug.LogError("[FarmlandBorderManager] centerTileFertilized (C1) 未配置"); isValid = false; }
            
            if (borderU == null) { Debug.LogError("[FarmlandBorderManager] borderU 未配置"); isValid = false; }
            if (borderD == null) { Debug.LogError("[FarmlandBorderManager] borderD 未配置"); isValid = false; }
            if (borderL == null) { Debug.LogError("[FarmlandBorderManager] borderL 未配置"); isValid = false; }
            if (borderR == null) { Debug.LogError("[FarmlandBorderManager] borderR 未配置"); isValid = false; }
            
            if (borderUD == null) { Debug.LogError("[FarmlandBorderManager] borderUD 未配置"); isValid = false; }
            if (borderLR == null) { Debug.LogError("[FarmlandBorderManager] borderLR 未配置"); isValid = false; }
            
            if (borderUL == null) { Debug.LogError("[FarmlandBorderManager] borderUL 未配置"); isValid = false; }
            if (borderUR == null) { Debug.LogError("[FarmlandBorderManager] borderUR 未配置"); isValid = false; }
            if (borderDL == null) { Debug.LogError("[FarmlandBorderManager] borderDL 未配置"); isValid = false; }
            if (borderDR == null) { Debug.LogError("[FarmlandBorderManager] borderDR 未配置"); isValid = false; }
            
            if (borderUDL == null) { Debug.LogError("[FarmlandBorderManager] borderUDL 未配置"); isValid = false; }
            if (borderUDR == null) { Debug.LogError("[FarmlandBorderManager] borderUDR 未配置"); isValid = false; }
            if (borderULR == null) { Debug.LogError("[FarmlandBorderManager] borderULR 未配置"); isValid = false; }
            if (borderDLR == null) { Debug.LogError("[FarmlandBorderManager] borderDLR 未配置"); isValid = false; }
            
            if (borderUDLR == null) { Debug.LogError("[FarmlandBorderManager] borderUDLR 未配置"); isValid = false; }
            
            if (shadowLU == null) { Debug.LogWarning("[FarmlandBorderManager] shadowLU 未配置（可选）"); }
            if (shadowRU == null) { Debug.LogWarning("[FarmlandBorderManager] shadowRU 未配置（可选）"); }
            if (shadowLD == null) { Debug.LogWarning("[FarmlandBorderManager] shadowLD 未配置（可选）"); }
            if (shadowRD == null) { Debug.LogWarning("[FarmlandBorderManager] shadowRD 未配置（可选）"); }
            
            return isValid;
        }
        
        #endregion
    }
}
