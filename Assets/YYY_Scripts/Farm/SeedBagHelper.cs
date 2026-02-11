using FarmGame.Data;
using FarmGame.Data.Core;

namespace FarmGame.Farm
{
    /// <summary>
    /// 种子袋辅助工具 - 管理种子袋的动态属性
    /// 
    /// 动态属性 Key:
    /// - bag_opened (bool): 是否已打开
    /// - bag_remaining (int): 袋内剩余种子数
    /// - shelf_expire_day (int): 过期的游戏总天数
    /// </summary>
    public static class SeedBagHelper
    {
        public const string KEY_OPENED = "bag_opened";
        public const string KEY_REMAINING = "bag_remaining";
        public const string KEY_EXPIRE_DAY = "shelf_expire_day";

        /// <summary>
        /// 初始化种子袋动态属性（购买/获得时调用）
        /// </summary>
        public static void InitializeSeedBag(InventoryItem item, SeedData seedData, int currentTotalDays)
        {
            if (item == null || seedData == null) return;

            item.SetProperty(KEY_OPENED, false);
            item.SetProperty(KEY_REMAINING, seedData.seedsPerBag);
            item.SetProperty(KEY_EXPIRE_DAY, currentTotalDays + seedData.shelfLifeClosed);
        }

        /// <summary>
        /// 打开种子袋
        /// </summary>
        /// <returns>是否成功打开（已打开的返回 false）</returns>
        public static bool OpenSeedBag(InventoryItem item, SeedData seedData, int currentTotalDays)
        {
            if (item == null || seedData == null) return false;
            if (item.GetPropertyBool(KEY_OPENED)) return false; // 已打开

            item.SetProperty(KEY_OPENED, true);

            // 保质期重算：Min(当前剩余天数, 打开后最大保质期)
            int currentExpire = item.GetPropertyInt(KEY_EXPIRE_DAY);
            int remainingDays = currentExpire - currentTotalDays;
            int newExpire = currentTotalDays + UnityEngine.Mathf.Min(remainingDays, seedData.shelfLifeOpened);
            item.SetProperty(KEY_EXPIRE_DAY, newExpire);

            return true;
        }

        /// <summary>
        /// 种植消耗一颗种子
        /// </summary>
        /// <returns>剩余种子数（0 表示用完）</returns>
        public static int ConsumeSeed(InventoryItem item, SeedData seedData, int currentTotalDays)
        {
            if (item == null || seedData == null) return -1;

            // 自动打开未打开的种子袋
            if (!item.GetPropertyBool(KEY_OPENED))
            {
                OpenSeedBag(item, seedData, currentTotalDays);
            }

            int remaining = item.GetPropertyInt(KEY_REMAINING);
            remaining--;
            item.SetProperty(KEY_REMAINING, remaining);

            return remaining;
        }

        /// <summary>
        /// 检查种子袋是否已过期
        /// </summary>
        public static bool IsExpired(InventoryItem item, int currentTotalDays)
        {
            if (item == null) return false;
            if (!item.HasProperty(KEY_EXPIRE_DAY)) return false;

            int expireDay = item.GetPropertyInt(KEY_EXPIRE_DAY);
            return currentTotalDays >= expireDay;
        }

        /// <summary>
        /// 检查物品是否是种子袋（有种子袋动态属性）
        /// </summary>
        public static bool IsSeedBag(InventoryItem item)
        {
            if (item == null || item.IsEmpty) return false;
            return item.HasProperty(KEY_EXPIRE_DAY);
        }

        /// <summary>
        /// 获取剩余种子数
        /// </summary>
        public static int GetRemaining(InventoryItem item)
        {
            if (item == null) return 0;
            return item.GetPropertyInt(KEY_REMAINING);
        }

        /// <summary>
        /// 是否已打开
        /// </summary>
        public static bool IsOpened(InventoryItem item)
        {
            if (item == null) return false;
            return item.GetPropertyBool(KEY_OPENED);
        }
    }
}
