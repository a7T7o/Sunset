using UnityEngine;

namespace FarmGame.Data
{
    /// <summary>
    /// 存储容器数据 - 可存储物品的容器
    /// 包括：箱子、柜子、冰箱等
    /// </summary>
    [CreateAssetMenu(fileName = "Storage_New", menuName = "Farm/Placeable/Storage", order = 3)]
    public class StorageData : PlaceableItemData
    {
        [Header("=== 存储专属属性 ===")]
        [Tooltip("存储容量（格子数）")]
        [Range(1, 100)]
        public int storageCapacity = 20;

        [Tooltip("显示行数")]
        [Range(1, 10)]
        public int storageRows = 4;

        [Tooltip("显示列数")]
        [Range(1, 10)]
        public int storageCols = 5;

        [Header("=== 存储限制 ===")]
        [Tooltip("允许存放的物品类型（空=全部）")]
        public ItemCategory[] allowedCategories;

        [Tooltip("是否可上锁")]
        public bool isLockable = false;

        [Tooltip("默认是否上锁")]
        public bool defaultLocked = false;

        [Header("=== 预制体 ===")]
        [Tooltip("存储容器预制体")]
        public GameObject storagePrefab;

        #region PlaceableItemData 实现

        public override PlacementType GetPlacementType() => PlacementType.Storage;

        public override GameObject GetPlacementPrefab() => storagePrefab;

        public override void OnPlaced(Vector3 position, GameObject instance)
        {
            base.OnPlaced(position, instance);
            
            // TODO: 初始化存储组件
            Debug.Log($"[StorageData] 存储容器放置成功: {itemName} (容量:{storageCapacity})");
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查物品类型是否允许存放
        /// </summary>
        public bool IsItemAllowed(ItemCategory category)
        {
            // 如果没有限制，允许所有类型
            if (allowedCategories == null || allowedCategories.Length == 0)
                return true;

            foreach (var allowed in allowedCategories)
            {
                if (allowed == category)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取存储容量描述
        /// </summary>
        public string GetCapacityDescription()
        {
            return $"{storageRows}x{storageCols} ({storageCapacity}格)";
        }

        #endregion

        #region 验证

        protected override void OnValidate()
        {
            base.OnValidate();

            // 验证存储容器ID范围（14XX - 存储容器类）
            if (itemID < 1400 || itemID >= 1500)
            {
                Debug.LogWarning($"[{itemName}] 存储容器ID建议在1400-1499范围内！当前:{itemID}");
            }

            // 验证预制体
            if (storagePrefab == null)
            {
                Debug.LogWarning($"[{itemName}] 缺少存储容器预制体！");
            }

            // 验证容量与行列匹配
            int calculatedCapacity = storageRows * storageCols;
            if (storageCapacity != calculatedCapacity)
            {
                Debug.LogWarning($"[{itemName}] 容量({storageCapacity})与行列({storageRows}x{storageCols}={calculatedCapacity})不匹配");
            }

            // 锁定配置验证
            if (defaultLocked && !isLockable)
            {
                defaultLocked = false;
                Debug.LogWarning($"[{itemName}] 不可上锁但默认上锁，已自动取消");
            }
        }

        public override string GetTooltipText()
        {
            string text = base.GetTooltipText();
            text += $"\n\n<color=cyan>容量: {GetCapacityDescription()}</color>";
            
            if (isLockable)
                text += $"\n<color=yellow>可上锁</color>";

            if (allowedCategories != null && allowedCategories.Length > 0)
                text += $"\n<color=orange>限定存放类型</color>";

            return text;
        }

        #endregion
    }
}
