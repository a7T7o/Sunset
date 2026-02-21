using UnityEngine;
using FarmGame.Data;
using FarmGame.Data.Core;

namespace FarmGame.Farm
{
    /// <summary>
    /// 作物状态枚举
    /// </summary>
    public enum CropState
    {
        /// <summary>生长中（Stage 0 ~ N-2）</summary>
        Growing,

        /// <summary>成熟（最后阶段，可收获）</summary>
        Mature,

        /// <summary>未成熟枯萎（缺水/过季，锄头清除，无产出）</summary>
        WitheredImmature,

        /// <summary>成熟枯萎（过熟/过季，可收获枯萎作物）</summary>
        WitheredMature
    }

    /// <summary>
    /// 作物控制器（自治版 V2）
    /// 附加到单个作物 GameObject 上，负责作物的渲染和交互
    /// 自己订阅时间事件，自己检查脚下土地是否湿润，自己决定是否生长
    /// 
    /// 实现 IPersistentObject + IInteractable 接口
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CropController : MonoBehaviour, IPersistentObject, IInteractable
    {
        #region 阶段 Sprite 配置
        
        [Header("=== 阶段 Sprite 配置 ===")]
        [Tooltip("每个生长阶段的 Sprite 配置（在 Prefab Inspector 上配置）")]
        [SerializeField] private CropStageConfig[] stages;
        
        #endregion

        #region 掉落配置
        
        [Header("=== 掉落配置 ===")]
        [Tooltip("收获掉落的物品 SO")]
        [SerializeField] private ItemData dropItemData;
        
        [Tooltip("收获掉落数量")]
        [SerializeField] private int dropAmount = 1;
        
        [Tooltip("掉落散布半径")]
        [SerializeField] private float dropSpreadRadius = 0.4f;
        
        [Tooltip("掉落品质（0=Normal）")]
        [SerializeField] private int dropQuality = 0;
        
        [Tooltip("枯萎收获掉落的物品 SO（可为空=不掉落）")]
        [SerializeField] private ItemData witheredDropItemData;
        
        #endregion

        #region 组件引用
        
        [Header("组件引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D interactionTrigger;
        
        #endregion

        #region 成熟闪烁配置
        
        [Header("成熟特效")]
        [SerializeField] private bool enableMatureGlow = true;
        [SerializeField] private float glowSpeed = 2f;
        [SerializeField] private Color glowColor = new Color(1f, 1f, 0.8f, 1f);
        
        #endregion
        
        #region 生长规则配置
        
        [Header("生长规则")]
        [Tooltip("是否需要浇水才能生长（false = 无需浇水，每天自动生长）")]
        [SerializeField] private bool needsWatering = true;
        
        [Tooltip("连续多少天未浇水后作物枯萎（仅 needsWatering=true 时生效）")]
        [SerializeField] private int daysUntilWithered = 3;
        
        [Tooltip("成熟后多少天未收获变为过熟枯萎")]
        [SerializeField] private int daysUntilOverMature = 2;
        
        #endregion

        #region 位置信息
        
        private int layerIndex;
        private Vector3Int cellPosition;
        
        /// <summary>作物所在层级索引（供收获检测等外部逻辑使用）</summary>
        public int LayerIndex => layerIndex;
        /// <summary>作物所在格子坐标（供收获检测等外部逻辑使用）</summary>
        public Vector3Int CellPos => cellPosition;
        
        #endregion
        
        #region 持久化 ID
        
        [Header("持久化")]
        [SerializeField] private string persistentId;
        
        #endregion

        #region 运行时数据
        
        private SeedData seedData;
        private CropInstanceData instanceData;
        private CropState state = CropState.Growing;
        private int daysSinceMature = 0;
        private float glowTime = 0f;
        private bool isInitialized = false;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        #endregion

        #region 常量
        
        /// <summary>固定 4 阶段：种子(0)→幼苗(1)→生长(2)→成熟(3)</summary>
        public const int CROP_STAGE_COUNT = 4;
        public const int CROP_STAGE_SEED = 0;
        public const int CROP_STAGE_SPROUT = 1;
        public const int CROP_STAGE_GROWING = 2;
        public const int CROP_STAGE_MATURE = 3;
        
        private static readonly Color WitheredColor = new Color(0.8f, 0.7f, 0.4f, 1f);
        
        #endregion

        #region 兼容旧版
        
        [System.Obsolete("使用 Initialize(SeedData, CropInstanceData) 替代")]
        private CropInstance cropInstance;
        
        #endregion

        #region IInteractable 实现
        
        public int InteractionPriority => 40;
        public float InteractionDistance => 1.2f;

        public bool CanInteract(InteractionContext context)
        {
            return state == CropState.Mature || state == CropState.WitheredMature;
        }

        public void OnInteract(InteractionContext context)
        {
            Harvest(context);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            if (state == CropState.Mature) return "收获";
            if (state == CropState.WitheredMature) return "收获（枯萎）";
            return "";
        }
        
        #endregion

        #region 生命周期
        
        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (interactionTrigger == null)
                interactionTrigger = GetComponent<BoxCollider2D>();
            
            // 确保 BoxCollider2D 是 Trigger
            if (interactionTrigger != null)
                interactionTrigger.isTrigger = true;
        }
        
        private void OnEnable()
        {
            TimeManager.OnDayChanged += OnDayChanged;
            SeasonManager.OnSeasonChanged += OnSeasonChanged;
            
            if (!string.IsNullOrEmpty(persistentId) && PersistentObjectRegistry.Instance != null)
                PersistentObjectRegistry.Instance.Register(this);
        }
        
        private void OnDisable()
        {
            TimeManager.OnDayChanged -= OnDayChanged;
            SeasonManager.OnSeasonChanged -= OnSeasonChanged;
            
            if (!string.IsNullOrEmpty(persistentId) && PersistentObjectRegistry.Instance != null)
                PersistentObjectRegistry.Instance.Unregister(this);
        }
        
        private void Update()
        {
            if (state == CropState.Mature && enableMatureGlow)
            {
                glowTime += Time.deltaTime * glowSpeed;
                float glow = Mathf.PingPong(glowTime, 1f);
                spriteRenderer.color = Color.Lerp(Color.white, glowColor, glow * 0.3f);
            }
        }
        
        #endregion

        #region 状态转换
        
        /// <summary>
        /// 尝试转换状态（仅允许合法路径）
        /// </summary>
        /// <returns>是否转换成功</returns>
        public bool TryTransitionState(CropState newState)
        {
            if (!IsValidTransition(state, newState))
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[CropController] 非法状态转换: {state} → {newState}");
                return false;
            }
            
            state = newState;
            
            if (newState == CropState.Mature)
                daysSinceMature = 0;
            
            UpdateVisuals();
            return true;
        }
        
        /// <summary>
        /// 检查状态转换是否合法
        /// </summary>
        public static bool IsValidTransition(CropState from, CropState to)
        {
            return (from, to) switch
            {
                (CropState.Growing, CropState.Mature) => true,
                (CropState.Growing, CropState.WitheredImmature) => true,
                (CropState.Mature, CropState.WitheredMature) => true,
                (CropState.Mature, CropState.Growing) => true, // 可重复收获重置
                _ => false
            };
        }
        
        #endregion
        
        #region 时间事件处理
        
        private void OnDayChanged(int year, int day, int totalDays)
        {
            if (!isInitialized || instanceData == null) return;
            
            switch (state)
            {
                case CropState.Growing:
                    HandleGrowingDay();
                    break;
                case CropState.Mature:
                    HandleMatureDay();
                    break;
                // WitheredImmature / WitheredMature 不处理日变化
            }
        }
        
        private void HandleGrowingDay()
        {
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager == null) return;
            
            var tileData = farmTileManager.GetTileData(layerIndex, cellPosition);
            if (tileData == null) return;
            
            // 不需要浇水的作物：每天自动生长，不累计缺水天数
            if (!needsWatering)
            {
                instanceData.grownDays++;
                instanceData.daysWithoutWater = 0;
                UpdateGrowthStage();
                
                if (IsMatureStage())
                {
                    TryTransitionState(CropState.Mature);
                    return;
                }
                
                UpdateVisuals();
                return;
            }
            
            // 需要浇水的作物：检查昨天是否浇水
            if (tileData.wateredYesterday)
            {
                instanceData.grownDays++;
                instanceData.daysWithoutWater = 0;
                UpdateGrowthStage();
                
                // 检查是否达到成熟
                if (IsMatureStage())
                {
                    TryTransitionState(CropState.Mature);
                    return;
                }
            }
            else
            {
                instanceData.daysWithoutWater++;
                
                if (instanceData.daysWithoutWater >= daysUntilWithered)
                {
                    instanceData.isWithered = true;
                    TryTransitionState(CropState.WitheredImmature);
                    return;
                }
            }
            
            UpdateVisuals();
        }
        
        private void HandleMatureDay()
        {
            if (seedData == null) return;
            
            // 可重复收获作物不触发过熟枯萎
            if (seedData.isReHarvestable) return;
            
            daysSinceMature++;
            if (daysSinceMature >= daysUntilOverMature)
            {
                instanceData.isWithered = true;
                TryTransitionState(CropState.WitheredMature);
            }
        }
        
        private void OnSeasonChanged(SeasonManager.Season newSeason)
        {
            if (!isInitialized || seedData == null) return;
            
            // AllSeason 作物不受季节影响
            if (seedData.season == Season.AllSeason) return;
            
            // 比较 FarmGame.Data.Season 与 SeasonManager.Season
            bool seasonMatch = ((int)seedData.season == (int)newSeason);
            if (seasonMatch) return;
            
            // 过季枯萎
            switch (state)
            {
                case CropState.Mature:
                    instanceData.isWithered = true;
                    TryTransitionState(CropState.WitheredMature);
                    break;
                case CropState.Growing:
                    instanceData.isWithered = true;
                    TryTransitionState(CropState.WitheredImmature);
                    break;
            }
        }
        
        private void UpdateGrowthStage()
        {
            if (seedData == null || instanceData == null) return;
            if (stages == null || stages.Length == 0) return;
            
            // 学习 TreeController 模式：每阶段独立天数累加
            // 遍历 stages，累加 daysToNextStage，找到当前应处于的阶段
            int accumulatedDays = 0;
            int newStage = 0;
            
            for (int i = 0; i < stages.Length - 1; i++)
            {
                accumulatedDays += stages[i].daysToNextStage;
                if (instanceData.grownDays >= accumulatedDays)
                    newStage = i + 1;
                else
                    break;
            }
            
            // 最后阶段封顶
            instanceData.currentStage = Mathf.Clamp(newStage, 0, stages.Length - 1);
        }
        
        private bool IsMatureStage()
        {
            if (stages == null || stages.Length == 0) return false;
            return instanceData.currentStage >= stages.Length - 1;
        }
        
        #endregion

        #region 收获逻辑
        
        /// <summary>
        /// 收获作物
        /// </summary>
        private void Harvest(InteractionContext context)
        {
            if (seedData == null || instanceData == null) return;
            
            var database = context?.Database;
            var inventory = context?.Inventory;
            
            if (state == CropState.Mature)
            {
                HarvestMature(database, inventory);
            }
            else if (state == CropState.WitheredMature)
            {
                HarvestWitheredMature(database, inventory);
            }
        }
        
        private void HarvestMature(ItemDatabase database, InventoryService inventory)
        {
            if (seedData == null) return;
            
            // 🔥 10.X 纠正：改用掉落到地面模式（学习 TreeController.SpawnDrops）
            if (dropItemData != null && dropAmount > 0)
            {
                var spawnService = WorldSpawnService.Instance;
                if (spawnService != null)
                {
                    spawnService.SpawnMultiple(dropItemData, dropQuality, dropAmount, transform.position, dropSpreadRadius);
                }
                else
                {
                    Debug.LogWarning($"[CropController] WorldSpawnService 不存在，无法掉落收获物: {dropItemData.itemName}");
                }
            }
            
            // 可重复收获：重置
            if (seedData.isReHarvestable)
            {
                int reGrowStage = Mathf.Max(0, (stages?.Length ?? 1) - 2);
                instanceData.currentStage = reGrowStage;
                instanceData.harvestCount++;
                instanceData.lastHarvestDay = TimeManager.Instance?.GetTotalDaysPassed() ?? 0;
                
                // 重新计算生长天数：累加到 reGrowStage 所需的总天数
                int accDays = 0;
                if (stages != null)
                {
                    for (int i = 0; i < reGrowStage && i < stages.Length; i++)
                        accDays += stages[i].daysToNextStage;
                }
                instanceData.grownDays = accDays;
                
                TryTransitionState(CropState.Growing);
            }
            else
            {
                DestroyCrop();
            }
        }
        
        private void HarvestWitheredMature(ItemDatabase database, InventoryService inventory)
        {
            if (seedData == null) return;
            
            // 🔥 10.X 纠正：枯萎收获改用掉落模式
            if (witheredDropItemData != null)
            {
                var spawnService = WorldSpawnService.Instance;
                if (spawnService != null)
                {
                    spawnService.SpawnMultiple(witheredDropItemData, 0, 1, transform.position, dropSpreadRadius);
                }
                else
                {
                    Debug.LogWarning($"[CropController] WorldSpawnService 不存在，无法掉落枯萎收获物");
                }
            }
            // witheredDropItemData 为空时不掉落，直接清除
            
            // 枯萎作物不可重复收获
            DestroyCrop();
        }
        
        /// <summary>
        /// 随机判定收获品质
        /// </summary>
        [System.Obsolete("10.X 纠正：品质改由 dropQuality 字段控制，不再随机")]
        private int DetermineHarvestQuality()
        {
            // 简单随机：80% Normal, 15% Rare, 4% Epic, 1% Legendary
            float roll = Random.value;
            if (roll < 0.01f) return (int)ItemQuality.Legendary;
            if (roll < 0.05f) return (int)ItemQuality.Epic;
            if (roll < 0.20f) return (int)ItemQuality.Rare;
            return (int)ItemQuality.Normal;
        }
        
        /// <summary>
        /// 掉落物品到地面
        /// </summary>
        [System.Obsolete("10.X 纠正：收获改用 dropItemData + SpawnMultiple 掉落模式")]
        private void DropItemToWorld(int itemId, int quality, int amount)
        {
            var spawnService = WorldSpawnService.Instance;
            if (spawnService == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[CropController] WorldSpawnService 不存在，无法掉落物品: ID={itemId}");
                return;
            }
            
            // 通过 InventoryService 获取 database
            var invService = FindFirstObjectByType<InventoryService>();
            ItemDatabase db = invService != null ? invService.Database : null;
            if (db == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[CropController] ItemDatabase 不存在，无法掉落物品: ID={itemId}");
                return;
            }
            
            var itemData = db.GetItemByID(itemId);
            if (itemData == null)
            {
                if (showDebugInfo)
                    Debug.LogWarning($"[CropController] 找不到物品数据: ID={itemId}");
                return;
            }
            
            spawnService.SpawnMultiple(itemData, quality, amount, transform.position);
        }
        
        /// <summary>
        /// 清除枯萎未成熟作物（锄头操作，无产出）
        /// </summary>
        public void ClearWitheredImmature()
        {
            if (state != CropState.WitheredImmature)
            {
                Debug.LogWarning($"[CropController] ClearWitheredImmature: 当前状态 {state} 不是 WitheredImmature");
                return;
            }
            
            // TODO: 添加枯萎清除 VFX/SFX（粒子效果 + 音效）
            if (showDebugInfo)
                Debug.Log($"[CropController] 清除枯萎作物: seed={seedData?.itemName}, stage={instanceData?.currentStage}");
            
            DestroyCrop();
        }
        
        /// <summary>
        /// 销毁作物
        /// </summary>
        private void DestroyCrop()
        {
            // 清除耕地上的作物数据和控制器引用
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                var tileData = farmTileManager.GetTileData(layerIndex, cellPosition);
                if (tileData != null)
                {
                    tileData.ClearCropData();
                    tileData.cropController = null;
                }
            }
            
            // 取消注册持久化对象
            if (FarmGame.Data.Core.PersistentObjectRegistry.Instance != null)
            {
                FarmGame.Data.Core.PersistentObjectRegistry.Instance.Unregister(this);
            }
            
            Destroy(gameObject);
        }
        
        #endregion

        #region 初始化
        
        public void Initialize(SeedData seed, CropInstanceData data)
        {
            seedData = seed;
            instanceData = data;
            
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                layerIndex = farmTileManager.GetCurrentLayerIndex(transform.position);
                var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
                if (tilemaps != null)
                    cellPosition = tilemaps.WorldToCell(transform.position);
                
                // 🔥 10.X 纠正：设置 FarmTileData 的控制器引用
                var tileData = farmTileManager.GetTileData(layerIndex, cellPosition);
                if (tileData != null)
                    tileData.cropController = this;
            }
            
            // 根据数据恢复状态
            RestoreStateFromData();
            
            isInitialized = true;
            UpdateVisuals();
        }
        
        public void Initialize(SeedData seed, CropInstanceData data, int layer, Vector3Int cell)
        {
            seedData = seed;
            instanceData = data;
            layerIndex = layer;
            cellPosition = cell;
            
            // 🔥 10.X 纠正：设置 FarmTileData 的控制器引用
            var ftm = FarmTileManager.Instance;
            if (ftm != null)
            {
                var tileData = ftm.GetTileData(layerIndex, cellPosition);
                if (tileData != null)
                    tileData.cropController = this;
            }
            
            RestoreStateFromData();
            
            isInitialized = true;
            UpdateVisuals();
        }
        
        /// <summary>
        /// 根据实例数据恢复状态
        /// </summary>
        private void RestoreStateFromData()
        {
            if (instanceData == null || seedData == null) return;
            
            if (instanceData.isWithered)
            {
                state = IsMatureStage() ? CropState.WitheredMature : CropState.WitheredImmature;
            }
            else if (IsMatureStage())
            {
                state = CropState.Mature;
            }
            else
            {
                state = CropState.Growing;
            }
        }
        
        [System.Obsolete("使用 Initialize(SeedData, CropInstanceData) 替代")]
        public void Initialize(CropInstance instance)
        {
            #pragma warning disable 0618
            cropInstance = instance;
            #pragma warning restore 0618
            
            if (instance != null && instance.seedData != null)
            {
                seedData = instance.seedData;
                instanceData = new CropInstanceData(instance.seedData.itemID, instance.plantedDay);
                instanceData.currentStage = instance.currentStage;
                instanceData.grownDays = instance.grownDays;
                instanceData.daysWithoutWater = instance.daysWithoutWater;
                instanceData.isWithered = instance.isWithered;
                instanceData.harvestCount = instance.harvestCount;
                instanceData.lastHarvestDay = instance.lastHarvestDay;
                instanceData.quality = (int)instance.quality;
            }
            
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                layerIndex = farmTileManager.GetCurrentLayerIndex(transform.position);
                var tilemaps = farmTileManager.GetLayerTilemaps(layerIndex);
                if (tilemaps != null)
                    cellPosition = tilemaps.WorldToCell(transform.position);
            }
            
            RestoreStateFromData();
            isInitialized = true;
            UpdateVisuals();
        }
        
        #endregion

        #region 视觉更新
        
        public void UpdateVisuals()
        {
            if (spriteRenderer == null) return;
            
            Sprite sprite = GetCurrentSprite();
            if (sprite != null)
                spriteRenderer.sprite = sprite;
            
            switch (state)
            {
                case CropState.WitheredImmature:
                case CropState.WitheredMature:
                    spriteRenderer.color = WitheredColor;
                    break;
                case CropState.Mature:
                    // 成熟闪烁在 Update 中处理，这里设置基础白色
                    spriteRenderer.color = Color.white;
                    break;
                default:
                    spriteRenderer.color = Color.white;
                    break;
            }

            AlignSpriteBottom();
        }

        /// <summary>
        /// 将 Sprite 底部对齐到 GameObject 原点（格子中心），
        /// 确保不同生长阶段的作物底部始终对齐。
        /// 参考：TreeController.AlignSpriteBottom()
        /// </summary>
        private void AlignSpriteBottom()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null) return;

            Bounds spriteBounds = spriteRenderer.sprite.bounds;
            Vector3 localPos = spriteRenderer.transform.localPosition;
            localPos.y = -spriteBounds.min.y;
            spriteRenderer.transform.localPosition = localPos;
        }
        
        private Sprite GetCurrentSprite()
        {
            if (stages == null || stages.Length == 0) return null;
            
            int stage = instanceData?.currentStage ?? 0;
            int index = Mathf.Clamp(stage, 0, stages.Length - 1);
            
            // 枯萎状态使用枯萎 Sprite（向前回退查找非空）
            if (state == CropState.WitheredImmature || state == CropState.WitheredMature)
            {
                for (int i = index; i >= 0; i--)
                {
                    if (stages[i].witheredSprite != null)
                        return stages[i].witheredSprite;
                }
                // 所有枯萎 Sprite 都为空，回退到正常 Sprite
            }
            
            // 正常状态使用生长 Sprite
            return stages[index].normalSprite;
        }
        
        #endregion

        #region 状态查询
        
        public CropState GetState() => state;
        
        public bool IsMature() => state == CropState.Mature;
        
        public bool IsWithered() => state == CropState.WitheredImmature || state == CropState.WitheredMature;
        
        public int GetCurrentStage() => instanceData?.currentStage ?? 0;
        
        public int GetTotalStages() => stages?.Length ?? 0;
        
        public float GetGrowthProgress()
        {
            int totalStages = GetTotalStages();
            if (totalStages <= 1) return 1f;
            return (float)GetCurrentStage() / (totalStages - 1);
        }
        
        /// <summary>
        /// 获取第一阶段的普通 Sprite（种子预览用）。
        /// 补丁003 模块B：CP-B2
        /// </summary>
        public Sprite GetFirstStageSprite()
        {
            if (stages == null || stages.Length == 0) return null;
            return stages[0].normalSprite;
        }
        
        /// <summary>
        /// 获取作物的格子中心世界坐标（父物体位置）。
        /// 套父物体后，CropController 在子物体上，transform.position 是偏移后的位置。
        /// 外部代码应使用此方法获取格子中心。
        /// 补丁003 模块B：CP-B1
        /// </summary>
        public Vector3 GetCellCenterPosition()
        {
            return transform.parent != null ? transform.parent.position : transform.position;
        }
        
        public SeedData GetSeedData() => seedData;
        public CropInstanceData GetInstanceData() => instanceData;
        
        #endregion

        #region 生长操作（外部调用）
        
        public void Grow()
        {
            if (instanceData == null || seedData == null) return;
            if (state != CropState.Growing) return;
            
            instanceData.grownDays++;
            UpdateGrowthStage();
            
            if (IsMatureStage())
                TryTransitionState(CropState.Mature);
            else
                UpdateVisuals();
        }
        
        public void SetWithered()
        {
            if (instanceData == null) return;
            instanceData.isWithered = true;
            
            if (state == CropState.Mature)
                TryTransitionState(CropState.WitheredMature);
            else if (state == CropState.Growing)
                TryTransitionState(CropState.WitheredImmature);
        }
        
        public void ResetForReHarvest(int reGrowStage)
        {
            if (instanceData == null) return;
            
            instanceData.currentStage = Mathf.Max(0, reGrowStage);
            
            // 累加到 reGrowStage 所需的总天数
            int accDays = 0;
            if (stages != null)
            {
                for (int i = 0; i < reGrowStage && i < stages.Length; i++)
                    accDays += stages[i].daysToNextStage;
            }
            instanceData.grownDays = accDays;
            
            TryTransitionState(CropState.Growing);
        }
        
        #endregion
        
        #region IPersistentObject 实现
        
        public string PersistentId => persistentId;
        public string ObjectType => "Crop";
        public bool ShouldSave => isInitialized && instanceData != null && seedData != null;
        
        public WorldObjectSaveData Save()
        {
            var saveData = new WorldObjectSaveData
            {
                guid = persistentId,
                objectType = ObjectType,
                prefabId = seedData?.itemID.ToString() ?? "-1"
            };
            
            saveData.SetPosition(transform.position);
            
            if (spriteRenderer != null)
                saveData.SetSortingLayer(spriteRenderer);
            
            var cropSaveData = new CropSaveData
            {
                seedId = seedData?.itemID ?? -1,
                currentStage = instanceData?.currentStage ?? 0,
                grownDays = instanceData?.grownDays ?? 0,
                daysWithoutWater = instanceData?.daysWithoutWater ?? 0,
                isWithered = instanceData?.isWithered ?? false,
                quality = instanceData?.quality ?? 0,
                harvestCount = instanceData?.harvestCount ?? 0,
                lastHarvestDay = instanceData?.lastHarvestDay ?? 0,
                daysSinceMature = daysSinceMature,
                layerIndex = layerIndex,
                cellX = cellPosition.x,
                cellY = cellPosition.y
            };
            
            saveData.genericData = JsonUtility.ToJson(cropSaveData);
            return saveData;
        }
        
        public void Load(WorldObjectSaveData data)
        {
            if (data == null || string.IsNullOrEmpty(data.genericData))
            {
                Debug.LogWarning($"[CropController] Load: 存档数据为空, GUID: {persistentId}");
                return;
            }
            
            var cropSaveData = JsonUtility.FromJson<CropSaveData>(data.genericData);
            if (cropSaveData == null)
            {
                Debug.LogWarning($"[CropController] Load: 反序列化失败, GUID: {persistentId}");
                return;
            }
            
            transform.position = data.GetPosition();
            
            if (spriteRenderer != null)
                data.RestoreSortingLayer(spriteRenderer);
            
            layerIndex = cropSaveData.layerIndex;
            cellPosition = new Vector3Int(cropSaveData.cellX, cropSaveData.cellY, 0);
            daysSinceMature = cropSaveData.daysSinceMature;
            
            if (instanceData == null)
                instanceData = new CropInstanceData(cropSaveData.seedId, 0);
            
            instanceData.currentStage = cropSaveData.currentStage;
            instanceData.grownDays = cropSaveData.grownDays;
            instanceData.daysWithoutWater = cropSaveData.daysWithoutWater;
            instanceData.isWithered = cropSaveData.isWithered;
            instanceData.quality = cropSaveData.quality;
            instanceData.harvestCount = cropSaveData.harvestCount;
            instanceData.lastHarvestDay = cropSaveData.lastHarvestDay;
            
            var farmTileManager = FarmTileManager.Instance;
            if (farmTileManager != null)
            {
                var tileData = farmTileManager.GetTileData(layerIndex, cellPosition);
                if (tileData != null)
                {
                    tileData.SetCropData(instanceData);
                    tileData.cropController = this; // 🔥 10.X 纠正：恢复控制器引用
                }
            }
            
            RestoreStateFromData();
            isInitialized = true;
            UpdateVisuals();
        }
        
        public void GeneratePersistentId()
        {
            persistentId = System.Guid.NewGuid().ToString();
        }
        
        public void SetPersistentId(string id)
        {
            persistentId = id;
        }
        
        #endregion
    }
}
