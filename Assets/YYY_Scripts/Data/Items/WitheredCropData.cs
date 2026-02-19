using UnityEngine;

namespace FarmGame.Data
{
    /// <summary>
    /// 枯萎作物数据 - 枯萎的农作物（继承FoodData，可食用但有负面buff）
    /// </summary>
    [CreateAssetMenu(fileName = "WCrop_New", menuName = "Farm/Items/Withered Crop", order = 4)]
    public class WitheredCropData : FoodData
    {
        [Header("=== 枯萎作物属性 ===")]
        [System.Obsolete("反向引用字段，已废弃。数据流改为单向：SeedData → cropPrefab → CropController。保留仅为存档兼容。")]
        [Tooltip("对应的种子ID（已废弃）")]
        public int seedID;

        protected override void OnValidate()
        {
            // 跳过 FoodData 的 ID 范围验证（5XXX），枯萎作物 ID 是 1150-1199
            ValidateItemDataBase();

            // 验证枯萎作物ID范围（1150-1199，与 CropData 共享 11XX 段）
            if (itemID < 1150 || itemID >= 1200)
            {
                Debug.LogWarning($"[{itemName}] 枯萎作物ID应在1150-1199范围内！当前:{itemID}");
            }
        }

        /// <summary>
        /// 调用 ItemData 基类的 OnValidate，跳过 FoodData 的 ID 范围验证
        /// </summary>
        private void ValidateItemDataBase()
        {
            if (itemID < 0 || itemID > 9999)
                Debug.LogWarning($"[{itemName}] ID超出范围！应在0-9999之间。");
            if (string.IsNullOrEmpty(itemName))
                Debug.LogWarning($"[ID:{itemID}] 物品名称为空！");
            if (icon == null)
                Debug.LogWarning($"[{itemName}] 缺少图标！");
            if (sellPrice > buyPrice && buyPrice > 0)
                Debug.LogWarning($"[{itemName}] 售价({sellPrice})高于买价({buyPrice})，不合理！");
        }
    }
}
