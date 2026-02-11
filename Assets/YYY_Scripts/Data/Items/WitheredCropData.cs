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
        [Tooltip("对应的正常作物ID")]
        public int normalCropID;

        [Tooltip("对应的种子ID")]
        public int seedID;

        protected override void OnValidate()
        {
            // 跳过 FoodData 的 ID 范围验证（5XXX），枯萎作物 ID 是 12XX
            ValidateItemDataBase();

            // 验证枯萎作物ID范围（12XX）
            if (itemID < 1200 || itemID >= 1300)
            {
                Debug.LogWarning($"[{itemName}] 枯萎作物ID应在1200-1299范围内！当前:{itemID}");
            }

            // 验证正常作物ID（11XX）
            if (normalCropID != 0 && (normalCropID < 1100 || normalCropID >= 1200))
            {
                Debug.LogWarning($"[{itemName}] 对应正常作物ID应在1100-1199范围内！当前:{normalCropID}");
            }

            // 验证种子ID（10XX）
            if (seedID != 0 && (seedID < 1000 || seedID >= 1100))
            {
                Debug.LogWarning($"[{itemName}] 对应种子ID应在1000-1099范围内！当前:{seedID}");
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
