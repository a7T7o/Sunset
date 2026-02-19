using UnityEngine;

namespace FarmGame.Farm
{
    /// <summary>
    /// 作物阶段配置 — 定义每个生长阶段的视觉表现和生长天数
    /// 配置在 CropController 的 Prefab Inspector 上
    /// 参考 TreeController 的 StageConfig 模式（固定 4 阶段：种子→幼苗→生长→成熟）
    /// </summary>
    [System.Serializable]
    public struct CropStageConfig
    {
        [Tooltip("该阶段到下一阶段需要的天数（最后阶段设为 0，表示不再生长）")]
        [Range(0, 30)]
        public int daysToNextStage;

        [Tooltip("该阶段的正常 Sprite")]
        public Sprite normalSprite;

        [Tooltip("该阶段的枯萎 Sprite（可为空，为空时向前查找最近的非空枯萎 Sprite）")]
        public Sprite witheredSprite;
    }
}
