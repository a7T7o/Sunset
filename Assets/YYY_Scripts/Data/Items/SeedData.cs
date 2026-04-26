using UnityEngine;

namespace FarmGame.Data
{
    /// <summary>
    /// 种子数据 - 可种植的种子
    /// </summary>
    [CreateAssetMenu(fileName = "Seed_New", menuName = "Farm/Items/Seed", order = 1)]
    public class SeedData : ItemData
    {
        [Header("=== 种植专属属性 ===")]
        [Tooltip("适合种植的季节")]
        public Season season = Season.Spring;

        [Tooltip("是否可以重复收获（如草莓、蓝莓）")]
        public bool isReHarvestable = false;

        [Tooltip("重复收获间隔天数（仅当可重复收获时有效）")]
        [Range(1, 14)]
        public int reHarvestDays = 2;

        [Tooltip("总共可收获次数（0=无限次）")]
        public int maxHarvestCount = 0;

        [Header("=== 种子袋配置 ===")]
        [Tooltip("每袋种子数量")]
        [Range(1, 20)]
        public int seedsPerBag = 5;

        [Tooltip("未打开保质期（天）")]
        [Range(1, 28)]
        public int shelfLifeClosed = 7;

        [Tooltip("打开后保质期（天）")]
        [Range(1, 14)]
        public int shelfLifeOpened = 2;

        [Tooltip("已打开状态的图标")]
        public Sprite iconOpened;

        [Header("=== 作物预制体 ===")]
        [Tooltip("作物预制体（包含 CropController + 阶段 Sprite 配置）")]
        public GameObject cropPrefab;

        [Header("=== 种植需求 ===")]
        [Tooltip("是否需要支架/棚架")]
        public bool needsTrellis = false;

        [System.Obsolete("needsWatering 已移至 CropController 的生长规则配置，由 Prefab Inspector 统一管控。保留仅为存档兼容。")]
        [Tooltip("需要保持湿润（已废弃，移至 CropController）")]
        public bool needsWatering = true;

        [Tooltip("种植经验值（种植时获得）")]
        public int plantingExp = 5;

        [Tooltip("收获经验值")]
        public int harvestingExp = 10;

        private void OnEnable()
        {
            // 运行时兜底：历史种子资产磁盘上可能仍是 isPlaceable=0，
            // 不能再让放置系统依赖编辑器 OnValidate 是否触发过。
            isPlaceable = true;
        }

        /// <summary>
        /// 验证种子数据
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            // 🔴 补丁005 B.1.1：种子标记为可放置，走放置系统
            isPlaceable = true;

            // 验证种子ID范围（1000-1099）
            if (itemID < 1000 || itemID >= 1100)
            {
                Debug.LogWarning($"[{itemName}] 种子ID应在1000-1099范围内！当前:{itemID}");
            }

            // 🔥 10.X 纠正：harvestCropID 已废弃，移除范围验证

            // 验证作物预制体
            if (cropPrefab == null)
            {
                Debug.LogWarning($"[{itemName}] 缺少作物预制体（cropPrefab）！");
            }
            else if (cropPrefab.GetComponentInChildren<FarmGame.Farm.CropController>() == null)
            {
                Debug.LogWarning($"[{itemName}] 作物预制体上没有 CropController 组件！");
            }
        }

        public override string GetTooltipText()
        {
            string text = base.GetTooltipText();
            text += $"\n<color=green>季节: {GetSeasonName(season)}</color>";

            if (isReHarvestable)
                text += $"\n<color=cyan>可重复收获（每{reHarvestDays}天）</color>";

            return text;
        }

        private string GetSeasonName(Season s)
        {
            return s switch
            {
                Season.Spring => "春季",
                Season.Summer => "夏季",
                Season.Fall => "秋季",
                Season.Winter => "冬季",
                Season.AllSeason => "全季节",
                _ => "未知"
            };
        }
    }
}

