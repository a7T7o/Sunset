using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using FarmGame.Data.Core;

namespace FarmGame.Farm
{
    /// <summary>
    /// 耕地状态管理器（自治版）
    /// 负责管理多楼层的耕地数据、浇水状态
    /// 直接订阅时间事件，自己处理每日重置和水渍干涸
    /// 不依赖 FarmingManagerNew
    ///
    /// 实现 IPersistentObject 接口，支持存档/读档
    /// </summary>
    public class FarmTileManager : MonoBehaviour, IPersistentObject
    {
        #region 单例

        public static FarmTileManager Instance { get; private set; }

        #endregion

        #region 配置

        [Header("多楼层配置")]
        [SerializeField] private LayerTilemaps[] layerTilemaps;

        [Header("水渍干涸设置")]
        [Tooltip("浇水多少小时后，水渍消失变为深色土壤")]
        [SerializeField] private float hoursUntilPuddleDry = 2f;

        [Header("视觉管理器引用")]
        [SerializeField] private FarmVisualManager visualManager;

        [Header("耕地障碍物检测配置")]
        [Tooltip("耕地障碍物检测盒边长（历史字段名保留为 Radius；默认 1.5 x 1.5，格心锚定）")]
        [SerializeField] private float farmingObstacleCheckRadius = 1.5f;

        [Tooltip("耕地障碍物标签（可多选）")]
        [SerializeField] private string[] farmingObstacleTags = new string[] { "Tree", "Rock", "Building" };

        [Tooltip("耕地白名单标签（这些物体不算障碍物，如稻草人、洒水器）")]
        [SerializeField] private string[] farmingWhitelistTags = new string[0];

        [Header("耕地地面检测配置")]
        [Tooltip("是否检测周围8格地面（防止悬空耕地）")]
        [SerializeField] private bool requireGroundAround = false;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        // 耕地检测配置访问器（供 FarmingManagerNew 使用）
        // 注意：FarmingObstacleCheckRadius 是历史命名，实际语义是检测盒边长。
        public static float FarmingObstacleCheckRadius => Instance != null ? Instance.farmingObstacleCheckRadius : 1.5f;
        public static string[] FarmingObstacleTags => Instance != null ? Instance.farmingObstacleTags : new string[] { "Tree", "Rock", "Building" };
        public static string[] FarmingWhitelistTags => Instance != null ? Instance.farmingWhitelistTags : new string[0];
        public static bool RequireGroundAround => Instance != null ? Instance.requireGroundAround : false;

        #endregion

        #region 数据存储

        /// <summary>
        /// 按楼层分组的耕地数据
        /// Key: layerIndex, Value: (cellPosition -> FarmTileData)
        /// </summary>
        private Dictionary<int, Dictionary<Vector3Int, FarmTileData>> farmTilesByLayer;

        /// <summary>
        /// 今天浇水的耕地（用于优化 OnHourChanged 遍历）
        /// </summary>
        private HashSet<(int layer, Vector3Int pos)> wateredTodayTiles;

        private const int EmptyTilledExpiryDays = 3;

        #endregion

        #region 生命周期

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDataStructures();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // 订阅时间事件（静态事件）
            TimeManager.OnDayChanged += OnDayChanged;
            TimeManager.OnHourChanged += OnHourChanged;

            if (showDebugInfo)
                Debug.Log("[FarmTileManager] 已订阅时间事件");
        }

        private void OnDisable()
        {
            // 取消订阅（防止内存泄漏）
            TimeManager.OnDayChanged -= OnDayChanged;
            TimeManager.OnHourChanged -= OnHourChanged;

            if (showDebugInfo)
                Debug.Log("[FarmTileManager] 已取消订阅时间事件");
        }

        private void Start()
        {
            // 自动获取 FarmVisualManager（如果未手动设置）
            if (visualManager == null)
            {
                visualManager = FindFirstObjectByType<FarmVisualManager>();
            }

            // 注册到持久化对象注册中心
            if (PersistentObjectRegistry.Instance != null)
            {
                PersistentObjectRegistry.Instance.PruneStaleRecords();
                PersistentObjectRegistry.Instance.Register(this);

                if (showDebugInfo)
                    Debug.Log("[FarmTileManager] 已注册到 PersistentObjectRegistry");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                if (PersistentObjectRegistry.Instance != null)
                {
                    PersistentObjectRegistry.Instance.Unregister(this);
                    PersistentObjectRegistry.Instance.PruneStaleRecords();
                }

                Instance = null;
            }
        }

        private void InitializeDataStructures()
        {
            farmTilesByLayer = new Dictionary<int, Dictionary<Vector3Int, FarmTileData>>();
            wateredTodayTiles = new HashSet<(int, Vector3Int)>();

            // 为每个楼层初始化字典
            if (layerTilemaps != null)
            {
                for (int i = 0; i < layerTilemaps.Length; i++)
                {
                    farmTilesByLayer[i] = new Dictionary<Vector3Int, FarmTileData>();
                }
            }
        }

        #endregion

        #region 时间事件处理（自治）

        /// <summary>
        /// 每天开始时触发：重置所有耕地的浇水状态
        /// </summary>
        private void OnDayChanged(int year, int day, int totalDays)
        {
            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] 新的一天开始 - 第{day}天，重置浇水状态");

            // 重置所有耕地的每日浇水状态
            ResetDailyWaterState();

            // 空置耕地按天结算：有作物清计时，无作物累计，满 3 天移除
            ProcessEmptyTilledTileExpiry(totalDays);

            // 刷新所有耕地视觉
            if (visualManager != null)
            {
                visualManager.RefreshAllTileVisuals();
            }
        }

        /// <summary>
        /// 对离场场景里的耕地补“过去了几天”的结算。
        /// 这里只处理农地自己的连续性：
        /// - 无作物耕地的空置寿命
        /// - 无作物耕地的浇水残留清理
        /// 作物本身的成长/枯萎由 CropController 在同一轮回场里单独补票。
        /// </summary>
        public void ApplyOffSceneElapsedDays(int elapsedDays, int currentTotalDays)
        {
            if (elapsedDays <= 0 || farmTilesByLayer == null)
            {
                return;
            }

            int firstMissedDayTotalDays = Mathf.Max(0, currentTotalDays - elapsedDays + 1);
            bool visualsDirty = false;
            var tilesToRemoveByLayer = new Dictionary<int, List<Vector3Int>>();

            foreach (var layerKvp in farmTilesByLayer)
            {
                int layerIndex = layerKvp.Key;
                Dictionary<Vector3Int, FarmTileData> layerTiles = layerKvp.Value;
                if (layerTiles == null)
                {
                    continue;
                }

                foreach (var tileKvp in layerTiles)
                {
                    FarmTileData tile = tileKvp.Value;
                    if (tile == null || !tile.isTilled)
                    {
                        continue;
                    }

                    if (tile.HasCrop())
                    {
                        tile.ClearEmptyCountdown();
                        continue;
                    }

                    if (tile.wateredToday || tile.wateredYesterday || tile.waterTime >= 0f || tile.moistureState != SoilMoistureState.Dry)
                    {
                        tile.wateredToday = false;
                        tile.wateredYesterday = false;
                        tile.waterTime = -1f;
                        tile.moistureState = SoilMoistureState.Dry;
                        visualsDirty = true;
                    }

                    if (!tile.HasEmptyCountdown())
                    {
                        tile.StartEmptyCountdown(firstMissedDayTotalDays);
                    }

                    if (tile.GetEmptyDayCount(currentTotalDays) < EmptyTilledExpiryDays)
                    {
                        continue;
                    }

                    if (!tilesToRemoveByLayer.TryGetValue(layerIndex, out List<Vector3Int> removals))
                    {
                        removals = new List<Vector3Int>();
                        tilesToRemoveByLayer[layerIndex] = removals;
                    }

                    removals.Add(tile.position);
                }
            }

            wateredTodayTiles.Clear();

            foreach (var pair in tilesToRemoveByLayer)
            {
                ApplyExpiredTileRemovalBatch(pair.Key, pair.Value);
            }

            if (visualsDirty && visualManager != null)
            {
                visualManager.RefreshAllTileVisuals();
            }

            if (showDebugInfo)
            {
                Debug.Log($"[FarmTileManager] ApplyOffSceneElapsedDays: elapsedDays={elapsedDays}, currentTotalDays={currentTotalDays}, removedBatches={tilesToRemoveByLayer.Count}");
            }
        }

        /// <summary>
        /// 每小时触发：更新土壤视觉状态（水渍→深色）
        /// </summary>
        private void OnHourChanged(int currentHour)
        {
            var timeManager = TimeManager.Instance;
            if (timeManager == null) return;

            float currentTime = timeManager.GetHour() + timeManager.GetMinute() / 60f;

            // 只遍历今天浇水的耕地（性能优化）
            foreach (var (layer, pos) in wateredTodayTiles)
            {
                var tileData = GetTileData(layer, pos);
                if (tileData == null) continue;

                // 只处理水渍状态的耕地
                if (tileData.moistureState != SoilMoistureState.WetWithPuddle) continue;

                float hoursSinceWatering = currentTime - tileData.waterTime;

                // 处理跨天情况
                if (hoursSinceWatering < 0)
                    hoursSinceWatering += 20f; // 游戏一天 20 小时

                // 超过设定时间：水渍消失，变为深色
                if (hoursSinceWatering >= hoursUntilPuddleDry)
                {
                    tileData.moistureState = SoilMoistureState.WetDark;

                    var tilemaps = GetLayerTilemaps(layer);
                    if (tilemaps != null && visualManager != null)
                    {
                        // 🔥 10.1.0：启动渐变协程（从干燥色渐变到湿润色）
                        visualManager.StartGradualMoistureTransition(tilemaps, pos, tileData);
                    }

                    if (showDebugInfo)
                        Debug.Log($"[FarmTileManager] 水渍干涸: Layer={layer}, Pos={pos}");
                }
            }
        }

        #endregion

        #region 空置耕地寿命

        private void ProcessEmptyTilledTileExpiry(int totalDays)
        {
            var tilesToRemoveByLayer = new Dictionary<int, List<Vector3Int>>();

            foreach (var layerKvp in farmTilesByLayer)
            {
                int layerIndex = layerKvp.Key;

                foreach (var tileKvp in layerKvp.Value)
                {
                    FarmTileData tile = tileKvp.Value;
                    if (tile == null || !tile.isTilled)
                    {
                        continue;
                    }

                    if (tile.HasCrop())
                    {
                        tile.ClearEmptyCountdown();
                        continue;
                    }

                    if (!tile.HasEmptyCountdown())
                    {
                        tile.StartEmptyCountdown(totalDays);
                        continue;
                    }

                    if (tile.GetEmptyDayCount(totalDays) < EmptyTilledExpiryDays)
                    {
                        continue;
                    }

                    if (!tilesToRemoveByLayer.TryGetValue(layerIndex, out var layerRemovals))
                    {
                        layerRemovals = new List<Vector3Int>();
                        tilesToRemoveByLayer[layerIndex] = layerRemovals;
                    }

                    layerRemovals.Add(tile.position);
                }
            }

            foreach (var layerRemoval in tilesToRemoveByLayer)
            {
                ApplyExpiredTileRemovalBatch(layerRemoval.Key, layerRemoval.Value);
            }
        }

        private void ApplyExpiredTileRemovalBatch(int layerIndex, List<Vector3Int> removalCandidates)
        {
            if (removalCandidates == null || removalCandidates.Count == 0)
            {
                return;
            }

            var uniqueRemovalPositions = new HashSet<Vector3Int>(removalCandidates);
            var affectedPositions = CollectAffectedPositions(uniqueRemovalPositions);

            foreach (var position in uniqueRemovalPositions)
            {
                RemoveTileDataOnly(layerIndex, position);
            }

            RefreshTileAreaAfterFinalRemoval(layerIndex, uniqueRemovalPositions, affectedPositions);

            if (showDebugInfo)
            {
                Debug.Log($"[FarmTileManager] 空置耕地到期移除: Layer={layerIndex}, Removed={uniqueRemovalPositions.Count}, Affected={affectedPositions.Count}");
            }
        }

        private static HashSet<Vector3Int> CollectAffectedPositions(IEnumerable<Vector3Int> centerPositions)
        {
            var affected = new HashSet<Vector3Int>();

            foreach (var center in centerPositions)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        affected.Add(center + new Vector3Int(dx, dy, 0));
                    }
                }
            }

            return affected;
        }

        private void RemoveTileDataOnly(int layerIndex, Vector3Int cellPosition)
        {
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                return;
            }

            if (!layerTiles.TryGetValue(cellPosition, out var existingTile) || !existingTile.isTilled)
            {
                return;
            }

            layerTiles.Remove(cellPosition);
            wateredTodayTiles.Remove((layerIndex, cellPosition));
        }

        private void RefreshTileAreaAfterFinalRemoval(int layerIndex, HashSet<Vector3Int> removedPositions, HashSet<Vector3Int> affectedPositions)
        {
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps == null)
            {
                return;
            }

            foreach (var removedPosition in removedPositions)
            {
                if (visualManager != null)
                {
                    visualManager.ClearTileVisual(tilemaps, removedPosition);
                }
                else
                {
                    if (tilemaps.farmlandCenterTilemap != null)
                    {
                        tilemaps.farmlandCenterTilemap.SetTile(removedPosition, null);
                    }

                    if (tilemaps.waterPuddleTilemapNew != null)
                    {
                        tilemaps.waterPuddleTilemapNew.SetTile(removedPosition, null);
                    }
                }
            }

            var borderManager = FarmlandBorderManager.Instance;

            foreach (var affectedPosition in affectedPositions)
            {
                var tileData = GetTileData(layerIndex, affectedPosition);
                if (tileData != null && tileData.isTilled && visualManager != null)
                {
                    visualManager.UpdateTileVisual(tilemaps, affectedPosition, tileData);
                }

                if (borderManager != null)
                {
                    var borderTile = borderManager.CalculateBorderTileAt(layerIndex, affectedPosition, null);
                    borderManager.ApplyBorderTile(tilemaps, affectedPosition, borderTile);
                }
                else if (tilemaps.farmlandBorderTilemap != null)
                {
                    tilemaps.farmlandBorderTilemap.SetTile(affectedPosition, null);
                }
            }
        }

        public void RefreshTileVisual(int layerIndex, Vector3Int cellPosition)
        {
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps == null || visualManager == null)
            {
                return;
            }

            FarmTileData tileData = GetTileData(layerIndex, cellPosition);
            if (tileData != null && tileData.isTilled)
            {
                visualManager.UpdateTileVisual(tilemaps, cellPosition, tileData);
            }
            else
            {
                visualManager.ClearTileVisual(tilemaps, cellPosition);
            }
        }

        #endregion

        #region 楼层检测

        /// <summary>
        /// 获取玩家当前所在楼层索引
        /// </summary>
        /// <param name="playerPosition">玩家世界坐标</param>
        /// <returns>楼层索引（0 = LAYER 1）</returns>
        public int GetCurrentLayerIndex(Vector3 playerPosition)
        {
            if (TryResolveLayerIndexFromDetectedLayer(playerPosition, out int detectedLayerIndex))
            {
                return detectedLayerIndex;
            }

            if (TryResolveLayerIndexFromConfiguredTilemaps(playerPosition, out int tilemapLayerIndex))
            {
                return tilemapLayerIndex;
            }
            // TODO: 实现基于玩家位置的楼层检测
            // 目前默认返回 0（LAYER 1）
            // 后续可以根据玩家所在的 Transform 父级或 Y 坐标判断
            return 0;
        }

        /// <summary>
        /// 获取指定楼层的 Tilemap 配置
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <returns>楼层 Tilemap 配置，无效时返回 null</returns>
        private bool TryResolveLayerIndexFromDetectedLayer(Vector3 worldPosition, out int layerIndex)
        {
            layerIndex = -1;

            if (layerTilemaps == null || layerTilemaps.Length == 0)
            {
                return false;
            }

            int detectedLayer = PlacementLayerDetector.GetLayerAtPosition(worldPosition);
            string detectedLayerName = LayerMask.LayerToName(detectedLayer);
            if (string.IsNullOrEmpty(detectedLayerName) ||
                string.Equals(detectedLayerName, "Default", StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = 0; i < layerTilemaps.Length; i++)
            {
                var tilemaps = layerTilemaps[i];
                if (tilemaps == null) continue;

                if (string.Equals(tilemaps.layerName, detectedLayerName, StringComparison.OrdinalIgnoreCase))
                {
                    layerIndex = i;
                    return true;
                }
            }

            if (TryParseLayerNameToIndex(detectedLayerName, out int parsedIndex) &&
                parsedIndex >= 0 &&
                parsedIndex < layerTilemaps.Length)
            {
                layerIndex = parsedIndex;
                return true;
            }

            return false;
        }

        private bool TryResolveLayerIndexFromConfiguredTilemaps(Vector3 worldPosition, out int layerIndex)
        {
            layerIndex = -1;

            if (layerTilemaps == null || layerTilemaps.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < layerTilemaps.Length; i++)
            {
                var tilemaps = layerTilemaps[i];
                if (tilemaps == null) continue;

                Vector3Int cellPos = tilemaps.WorldToCell(worldPosition);
                if (HasAnyConfiguredTileAtCell(tilemaps, cellPos))
                {
                    layerIndex = i;
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseLayerNameToIndex(string layerName, out int layerIndex)
        {
            layerIndex = -1;
            if (string.IsNullOrWhiteSpace(layerName)) return false;

            const string prefix = "LAYER ";
            if (!layerName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!int.TryParse(layerName.Substring(prefix.Length), out int parsedLayerNumber))
            {
                return false;
            }

            layerIndex = parsedLayerNumber - 1;
            return layerIndex >= 0;
        }

        private static bool HasAnyConfiguredTileAtCell(LayerTilemaps tilemaps, Vector3Int cellPos)
        {
            if (tilemaps == null)
            {
                return false;
            }

            if (tilemaps.groundTilemap != null && tilemaps.groundTilemap.GetTile(cellPos) != null)
            {
                return true;
            }

            if (tilemaps.farmlandCenterTilemap != null && tilemaps.farmlandCenterTilemap.GetTile(cellPos) != null)
            {
                return true;
            }

            if (tilemaps.farmlandBorderTilemap != null && tilemaps.farmlandBorderTilemap.GetTile(cellPos) != null)
            {
                return true;
            }

            #pragma warning disable 0618
            if (tilemaps.farmlandTilemap != null && tilemaps.farmlandTilemap.GetTile(cellPos) != null)
            {
                return true;
            }
            #pragma warning restore 0618

            return false;
        }

        public LayerTilemaps GetLayerTilemaps(int layerIndex)
        {
            if (layerTilemaps == null || layerIndex < 0 || layerIndex >= layerTilemaps.Length)
            {
                return null;
            }
            return layerTilemaps[layerIndex];
        }

        /// <summary>
        /// 获取楼层数量
        /// </summary>
        public int LayerCount => layerTilemaps?.Length ?? 0;

        #endregion

        #region 耕地 CRUD 操作

        /// <summary>
        /// 获取指定位置的耕地数据
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <returns>耕地数据，不存在时返回 null</returns>
        public FarmTileData GetTileData(int layerIndex, Vector3Int cellPosition)
        {
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                return null;
            }

            layerTiles.TryGetValue(cellPosition, out FarmTileData data);
            return data;
        }

        /// <summary>
        /// 从世界坐标解析楼层、格子和耕地数据。
        /// 返回 true 仅表示成功解析到楼层和格子，tileData 可能为空。
        /// </summary>
        public bool TryResolveTileAtWorld(
            Vector3 worldPosition,
            out int layerIndex,
            out Vector3Int cellPosition,
            out FarmTileData tileData)
        {
            layerIndex = GetCurrentLayerIndex(worldPosition);
            cellPosition = Vector3Int.zero;
            tileData = null;

            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps == null)
            {
                return false;
            }

            cellPosition = tilemaps.WorldToCell(worldPosition);
            tileData = GetTileData(layerIndex, cellPosition);
            return true;
        }

        /// <summary>
        /// 检查指定格子是否存在作物占位。
        /// </summary>
        public bool HasCropOccupant(int layerIndex, Vector3Int cellPosition)
        {
            var tileData = GetTileData(layerIndex, cellPosition);
            return tileData != null && tileData.HasCrop();
        }

        /// <summary>
        /// 检查世界坐标对应格子是否存在作物占位。
        /// </summary>
        public bool HasCropOccupantAtWorld(Vector3 worldPosition)
        {
            return TryResolveTileAtWorld(worldPosition, out int layerIndex, out Vector3Int cellPosition, out _) &&
                   HasCropOccupant(layerIndex, cellPosition);
        }

        /// <summary>
        /// 检查世界坐标对应格子是否已耕地。
        /// </summary>
        public bool IsTilledAtWorld(Vector3 worldPosition)
        {
            return TryResolveTileAtWorld(worldPosition, out _, out _, out FarmTileData tileData) &&
                   tileData != null &&
                   tileData.isTilled;
        }

        /// <summary>
        /// 获取指定格子的世界格心坐标。
        /// </summary>
        public Vector3 GetCellCenterWorld(int layerIndex, Vector3Int cellPosition)
        {
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps != null)
            {
                return tilemaps.GetCellCenterWorld(cellPosition);
            }

            return PlacementGridCalculator.CellIndexToWorldCenter(cellPosition);
        }

        /// <summary>
        /// 创建耕地
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <returns>是否创建成功</returns>
        public bool CreateTile(int layerIndex, Vector3Int cellPosition)
        {
            // ===== P0：强制配置检查（防止虚空锄地）=====
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps == null)
            {
                Debug.LogError($"[FarmTileManager] CreateTile 失败: 楼层 {layerIndex} 的 Tilemap 配置为空");
                return false;
            }

            if (tilemaps.groundTilemap == null)
            {
                Debug.LogError($"[FarmTileManager] CreateTile 失败: 楼层 {layerIndex} 的 groundTilemap 未配置");
                return false;
            }

            // 检查楼层是否有效
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                Debug.LogError($"[FarmTileManager] CreateTile 失败: 无效的楼层索引 {layerIndex}");
                return false;
            }

            // 检查是否已存在耕地
            if (layerTiles.ContainsKey(cellPosition))
            {
                var existingTile = layerTiles[cellPosition];
                if (existingTile.isTilled)
                {
                    if (showDebugInfo)
                        Debug.Log($"[FarmTileManager] 该位置已有耕地: Layer={layerIndex}, Pos={cellPosition}");
                    return false;
                }
            }

            // 检查是否可以耕作（有地面 Tile）
            if (!tilemaps.IsValid())
            {
                Debug.LogError($"[FarmTileManager] CreateTile 失败: 楼层 {layerIndex} 的 Tilemap 配置无效");
                return false;
            }

            // 检查地面 Tile 是否存在
            TileBase groundTile = tilemaps.groundTilemap.GetTile(cellPosition);
            if (groundTile == null)
            {
                if (showDebugInfo)
                    Debug.Log($"[FarmTileManager] 该位置没有地面 Tile: {cellPosition}");
                return false;
            }

            // 创建耕地数据
            FarmTileData newTile = new FarmTileData(cellPosition, layerIndex);
            newTile.isTilled = true;
            newTile.moistureState = SoilMoistureState.Dry;
            newTile.ClearEmptyCountdown();

            layerTiles[cellPosition] = newTile;

            // 通知边界管理器更新边界
            if (FarmlandBorderManager.Instance != null)
            {
                FarmlandBorderManager.Instance.OnCenterBlockPlaced(layerIndex, cellPosition);
            }

            // 🔥 10.1.0：创建时立即刷新视觉（确保干燥农田 Tile 正确显示）
            if (visualManager != null)
            {
                visualManager.UpdateTileVisual(tilemaps, cellPosition, newTile);
            }

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] 创建耕地: Layer={layerIndex}, Pos={cellPosition}");

            return true;
        }

        /// <summary>
        /// 检查指定位置是否可以耕作
        /// </summary>
        public bool CanTillAt(int layerIndex, Vector3Int cellPosition)
        {
            // ===== P0：强制配置检查（防止虚空锄地）=====
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps == null)
            {
                Debug.LogError($"[FarmTileManager] CanTillAt 失败: 楼层 {layerIndex} 的 Tilemap 配置为空");
                return false;
            }

            if (tilemaps.groundTilemap == null)
            {
                Debug.LogError($"[FarmTileManager] CanTillAt 失败: 楼层 {layerIndex} 的 groundTilemap 未配置");
                return false;
            }

            // 检查楼层是否有效
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                return false;
            }

            // 检查是否已存在耕地
            if (layerTiles.TryGetValue(cellPosition, out var existingTile))
            {
                if (existingTile.isTilled)
                {
                    return false;
                }
            }

            // 检查是否有地面 Tile
            if (!tilemaps.IsValid())
            {
                return false;
            }

            // 检查地面 Tile 是否存在
            TileBase groundTile = tilemaps.groundTilemap.GetTile(cellPosition);
            if (groundTile == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除耕地
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveTile(int layerIndex, Vector3Int cellPosition)
        {
            // 检查楼层是否有效
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                return false;
            }

            // 检查是否存在耕地
            if (!layerTiles.TryGetValue(cellPosition, out var existingTile) || !existingTile.isTilled)
            {
                return false;
            }

            // 移除耕地数据
            layerTiles.Remove(cellPosition);

            // 从今天浇水集合中移除
            wateredTodayTiles.Remove((layerIndex, cellPosition));

            // 通知边界管理器更新边界
            if (FarmlandBorderManager.Instance != null)
            {
                FarmlandBorderManager.Instance.OnCenterBlockRemoved(layerIndex, cellPosition);
            }

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] 移除耕地: Layer={layerIndex}, Pos={cellPosition}");

            return true;
        }

        #endregion

        #region 浇水状态管理

        /// <summary>
        /// 设置耕地浇水状态
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <param name="waterTime">浇水时间（游戏小时）</param>
        /// <param name="puddleVariant">水渍变体索引（0-2）</param>
        /// <returns>是否设置成功</returns>
        public bool SetWatered(int layerIndex, Vector3Int cellPosition, float waterTime, int puddleVariant = -1)
        {
            var tileData = GetTileData(layerIndex, cellPosition);
            if (tileData == null || !tileData.isTilled)
            {
                return false;
            }

            // 今天已经浇过水
            if (tileData.wateredToday)
            {
                return false;
            }

            // 随机水渍变体
            if (puddleVariant < 0)
            {
                puddleVariant = UnityEngine.Random.Range(0, 3);
            }

            tileData.SetWatered(waterTime, puddleVariant);

            // 添加到今天浇水的集合
            wateredTodayTiles.Add((layerIndex, cellPosition));

            // 🔥 Bug C 修复：浇水后立即更新视觉（显示水渍 Tile）
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps != null && visualManager != null)
            {
                visualManager.UpdateTileVisual(tilemaps, cellPosition, tileData);
            }

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] 浇水: Layer={layerIndex}, Pos={cellPosition}, Time={waterTime:F1}");

            return true;
        }

        /// <summary>
        /// 重置所有耕地的每日浇水状态（每天开始时调用）
        /// </summary>
        public void ResetDailyWaterState()
        {
            int resetCount = 0;

            foreach (var layerKvp in farmTilesByLayer)
            {
                foreach (var tileKvp in layerKvp.Value)
                {
                    FarmTileData tile = tileKvp.Value;
                    if (tile.isTilled)
                    {
                        tile.ResetDailyWaterState();
                        resetCount++;
                    }
                }
            }

            // 清空今天浇水的集合
            wateredTodayTiles.Clear();

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] 重置每日浇水状态: {resetCount} 块耕地");
        }

        /// <summary>
        /// 获取今天浇水的耕地集合（用于优化 OnHourChanged 遍历）
        /// </summary>
        public IEnumerable<(int layer, Vector3Int pos)> GetWateredTodayTiles()
        {
            return wateredTodayTiles;
        }

        #endregion

        #region 遍历方法

        /// <summary>
        /// 获取指定楼层的所有耕地数据
        /// </summary>
        public IEnumerable<FarmTileData> GetAllTilesInLayer(int layerIndex)
        {
            if (farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                return layerTiles.Values;
            }
            return System.Array.Empty<FarmTileData>();
        }

        /// <summary>
        /// 获取所有楼层的所有耕地数据
        /// </summary>
        public IEnumerable<FarmTileData> GetAllTiles()
        {
            foreach (var layerKvp in farmTilesByLayer)
            {
                foreach (var tileKvp in layerKvp.Value)
                {
                    yield return tileKvp.Value;
                }
            }
        }

        /// <summary>
        /// 获取所有有作物的耕地
        /// </summary>
        public IEnumerable<FarmTileData> GetAllTilesWithCrops()
        {
            foreach (var tile in GetAllTiles())
            {
                if (tile.HasCrop())
                {
                    yield return tile;
                }
            }
        }

        #endregion

        #region 调试

        private void OnDrawGizmos()
        {
            if (!showDebugInfo || farmTilesByLayer == null) return;

            foreach (var layerKvp in farmTilesByLayer)
            {
                int layerIndex = layerKvp.Key;
                var tilemaps = GetLayerTilemaps(layerIndex);
                #pragma warning disable 0618
                if (tilemaps == null || tilemaps.farmlandTilemap == null) continue;
                #pragma warning restore 0618

                foreach (var tileKvp in layerKvp.Value)
                {
                    FarmTileData data = tileKvp.Value;
                    if (!data.isTilled) continue;

                    Vector3 worldPos = tilemaps.GetCellCenterWorld(tileKvp.Key);

                    // 根据状态设置颜色
                    if (data.HasCrop())
                    {
                        Gizmos.color = data.cropData.isWithered ? Color.red : Color.green;
                    }
                    else if (data.moistureState == SoilMoistureState.WetWithPuddle)
                    {
                        Gizmos.color = Color.cyan;
                    }
                    else if (data.moistureState == SoilMoistureState.WetDark)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = new Color(0.6f, 0.4f, 0.2f);
                    }

                    Gizmos.DrawWireCube(worldPos, Vector3.one * 0.3f);
                }
            }
        }

        #endregion

        #region IPersistentObject 实现

        /// <summary>
        /// 持久化 ID（FarmTileManager 是单例，使用固定 ID）
        /// </summary>
        public string PersistentId => "FarmTileManager";

        /// <summary>
        /// 对象类型标识
        /// </summary>
        public string ObjectType => "FarmTileManager";

        /// <summary>
        /// 是否需要保存（始终为 true）
        /// </summary>
        public bool ShouldSave => true;

        /// <summary>
        /// 保存耕地数据
        /// </summary>
        public WorldObjectSaveData Save()
        {
            var saveData = new WorldObjectSaveData
            {
                guid = PersistentId,
                objectType = ObjectType
            };

            // 序列化所有耕地数据
            var farmTiles = new List<FarmTileSaveData>();

            foreach (var layerKvp in farmTilesByLayer)
            {
                int layerIndex = layerKvp.Key;

                foreach (var tileKvp in layerKvp.Value)
                {
                    var tile = tileKvp.Value;
                    if (!tile.isTilled) continue;

                    farmTiles.Add(new FarmTileSaveData
                    {
                        tileX = tile.position.x,
                        tileY = tile.position.y,
                        layer = tile.layerIndex,
                        soilState = (int)tile.moistureState,
                        isWatered = tile.wateredToday,
                        // 🔥 10.1.0：新增持久化字段
                        wateredYesterday = tile.wateredYesterday,
                        waterTime = tile.waterTime,
                        puddleVariant = tile.puddleVariant,
                        hasEmptySinceRecord = tile.hasEmptySinceRecord,
                        emptySinceTotalDays = tile.emptySinceTotalDays
                    });
                }
            }

            saveData.genericData = JsonUtility.ToJson(new FarmTileListWrapper { tiles = farmTiles });

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] Save: 保存了 {farmTiles.Count} 块耕地");

            return saveData;
        }

        /// <summary>
        /// 加载耕地数据
        /// </summary>
        public void Load(WorldObjectSaveData data)
        {
            if (data == null || string.IsNullOrEmpty(data.genericData))
            {
                Debug.LogWarning("[FarmTileManager] Load: 存档数据为空");
                return;
            }

            // 清空现有数据
            ClearAllTiles();

            // 反序列化耕地数据
            var wrapper = JsonUtility.FromJson<FarmTileListWrapper>(data.genericData);
            if (wrapper == null || wrapper.tiles == null)
            {
                Debug.LogWarning("[FarmTileManager] Load: 反序列化失败");
                return;
            }

            int loadedCount = 0;

            foreach (var tileData in wrapper.tiles)
            {
                var cellPos = new Vector3Int(tileData.tileX, tileData.tileY, 0);

                // 创建耕地数据
                if (CreateTileFromSaveData(tileData.layer, cellPos, tileData))
                {
                    loadedCount++;
                }
            }

            // 刷新边界视觉
            if (FarmlandBorderManager.Instance != null)
            {
                FarmlandBorderManager.Instance.RefreshAllLayersBorders();
            }

            // 刷新所有耕地视觉
            if (visualManager != null)
            {
                visualManager.RefreshAllTileVisuals();
            }

            if (showDebugInfo)
                Debug.Log($"[FarmTileManager] Load: 加载了 {loadedCount} 块耕地");
        }

        /// <summary>
        /// 清空所有耕地数据
        /// </summary>
        public void ClearAllTiles()
        {
            // 清空 Tilemap
            foreach (var layerKvp in farmTilesByLayer)
            {
                int layerIndex = layerKvp.Key;
                var tilemaps = GetLayerTilemaps(layerIndex);

                if (tilemaps != null)
                {
                    #pragma warning disable 0618
                    // 清空耕地 Tilemap（新版优先）
                    if (tilemaps.farmlandCenterTilemap != null)
                    {
                        tilemaps.farmlandCenterTilemap.ClearAllTiles();
                    }
                    else if (tilemaps.farmlandTilemap != null)
                    {
                        tilemaps.farmlandTilemap.ClearAllTiles();
                    }

                    // 清空水渍 Tilemap（新版优先）
                    if (tilemaps.waterPuddleTilemapNew != null)
                    {
                        tilemaps.waterPuddleTilemapNew.ClearAllTiles();
                    }
                    else if (tilemaps.waterPuddleTilemap != null)
                    {
                        tilemaps.waterPuddleTilemap.ClearAllTiles();
                    }
                    #pragma warning restore 0618
                }

                // 清空数据字典
                layerKvp.Value.Clear();
            }

            // 清空今天浇水的集合
            wateredTodayTiles.Clear();

            if (showDebugInfo)
                Debug.Log("[FarmTileManager] ClearAllTiles: 已清空所有耕地");
        }

        /// <summary>
        /// 从存档数据创建耕地
        /// </summary>
        /// <param name="layerIndex">楼层索引</param>
        /// <param name="cellPosition">格子坐标</param>
        /// <param name="saveData">存档数据</param>
        /// <returns>是否创建成功</returns>
        public bool CreateTileFromSaveData(int layerIndex, Vector3Int cellPosition, FarmTileSaveData saveData)
        {
            // 检查楼层是否有效
            if (!farmTilesByLayer.TryGetValue(layerIndex, out var layerTiles))
            {
                // 尝试初始化楼层
                if (layerIndex >= 0 && layerIndex < LayerCount)
                {
                    farmTilesByLayer[layerIndex] = new Dictionary<Vector3Int, FarmTileData>();
                    layerTiles = farmTilesByLayer[layerIndex];
                }
                else
                {
                    Debug.LogError($"[FarmTileManager] CreateTileFromSaveData 失败: 无效的楼层索引 {layerIndex}");
                    return false;
                }
            }

            // 创建耕地数据
            FarmTileData newTile = new FarmTileData(cellPosition, layerIndex);
            newTile.isTilled = true;
            newTile.moistureState = (SoilMoistureState)saveData.soilState;
            newTile.wateredToday = saveData.isWatered;
            // 🔥 10.1.0：恢复新增持久化字段
            newTile.wateredYesterday = saveData.wateredYesterday;
            newTile.waterTime = saveData.waterTime;
            newTile.puddleVariant = saveData.puddleVariant;
            newTile.hasEmptySinceRecord = saveData.hasEmptySinceRecord;
            newTile.emptySinceTotalDays = saveData.emptySinceTotalDays;

            layerTiles[cellPosition] = newTile;

            // 如果今天已浇水，添加到集合
            if (saveData.isWatered)
            {
                wateredTodayTiles.Add((layerIndex, cellPosition));
            }

            // 放置 Tilemap Tile
            var tilemaps = GetLayerTilemaps(layerIndex);
            if (tilemaps != null && visualManager != null)
            {
                visualManager.UpdateTileVisual(tilemaps, cellPosition, newTile);
            }

            return true;
        }

        #endregion
    }
}
