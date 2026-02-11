using UnityEngine;

namespace FarmGame.Data
{
    /// <summary>
    /// 作物数据 - 收获的农作物（继承FoodData，作物可食用）
    /// </summary>
    [CreateAssetMenu(fileName = "Crop_New", menuName = "Farm/Items/Crop", order = 2)]
    public class CropData : FoodData
    {
        [Header("=== 作物专属属性 ===")]
        [Tooltip("对应的种子ID")]
        public int seedID;

        [Tooltip("收获经验值")]
        public int harvestExp = 10;

        [Tooltip("对应的枯萎作物ID（WitheredCropData）")]
        public int witheredCropID;

        /// <summary>
        /// 获取作物图标（品质不影响外观，始终返回icon）
        /// </summary>
        public Sprite GetCropIcon()
        {
            return icon;
        }

        protected override void OnValidate()
        {
            // 跳过 FoodData 的 ID 范围验证（5XXX），直接调用 ItemData.OnValidate
            // FoodData.OnValidate 会验证 5000-5999，但作物 ID 是 11XX
            ValidateItemDataBase();

            // 验证作物ID范围（11XX）
            if (itemID < 1100 || itemID >= 1200)
            {
                Debug.LogWarning($"[{itemName}] 作物ID应在1100-1199范围内！");
            }

            // 验证种子ID
            if (seedID < 1000 || seedID >= 1100)
            {
                Debug.LogWarning($"[{itemName}] 对应种子ID应在1000-1099范围内！");
            }

            // 验证枯萎作物ID（12XX）
            if (witheredCropID != 0 && (witheredCropID < 1200 || witheredCropID >= 1300))
            {
                Debug.LogWarning($"[{itemName}] 枯萎作物ID应在1200-1299范围内！");
            }
        }

        /// <summary>
        /// 调用 ItemData 基类的 OnValidate，跳过 FoodData 的 ID 范围验证
        /// </summary>
        private void ValidateItemDataBase()
        {
            // 手动执行 ItemData.OnValidate 中的通用验证逻辑
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

