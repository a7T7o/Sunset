namespace FarmGame.Data
{
    /// <summary>
    /// 放置类型枚举
    /// 定义不同物品的放置规则
    /// </summary>
    public enum PlacementType
    {
        None = 0,
        Sapling = 1,
        Decoration = 2,
        Building = 3,
        Furniture = 4,
        Workstation = 5,
        Storage = 6,
        InteractiveDisplay = 7,
        SimpleEvent = 8
    }

    /// <summary>
    /// 放置验证失败原因
    /// </summary>
    public enum PlacementInvalidReason
    {
        None = 0,
        OutOfRange = 1,
        ObstacleBlocking = 2,
        OnFarmland = 3,
        OnWater = 4,
        InsufficientSpace = 5,
        WrongSeason = 6,
        InvalidTerrain = 7,
        TreeTooClose = 8,
        BuildingOverlap = 9,
        IndoorOnly = 10,
        OutdoorOnly = 11
    }

    /// <summary>
    /// 工作台类型枚举
    /// </summary>
    public enum WorkstationType
    {
        Heating = 0,        // 取暖设施（壁炉、火盆等）
        Smelting = 1,       // 冶炼设施（熔炉、高炉等）
        Pharmacy = 2,       // 制药设施（炼药台、蒸馏器等）
        Sawmill = 3,        // 锯木设施（劈柴架、锯木台等）
        Cooking = 4,        // 烹饪设施（厨房、烤炉等）
        Crafting = 5        // 装备和工具制作设施（工作台、铁砧等）
    }

    /// <summary>
    /// 简单事件类型枚举
    /// </summary>
    public enum SimpleEventType
    {
        PlaySound = 0,      // 播放音效
        PlayAnimation = 1,  // 播放动画
        Teleport = 2,       // 传送
        Unlock = 3,         // 解锁
        GiveItem = 4,       // 给予物品
        TriggerQuest = 5,   // 触发任务
        ShowMessage = 6     // 显示消息
    }
}
