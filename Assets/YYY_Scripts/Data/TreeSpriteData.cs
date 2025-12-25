using UnityEngine;

/// <summary>
/// 影子配置 - 包含 Sprite 和缩放
/// 用于配置每个阶段的影子显示
/// </summary>
[System.Serializable]
public class ShadowConfig
{
    [Tooltip("影子 Sprite（可选，未配置则使用原始 Sprite）")]
    public Sprite sprite;
    
    [Tooltip("影子缩放")]
    [Range(0f, 2f)]
    public float scale = 1f;
    
    /// <summary>
    /// 创建默认的影子配置数组（5个元素，对应阶段1-5）
    /// 阶段0没有影子
    /// </summary>
    public static ShadowConfig[] CreateDefaultConfigs()
    {
        return new ShadowConfig[]
        {
            new ShadowConfig { sprite = null, scale = 0f },    // 阶段1（无影子）
            new ShadowConfig { sprite = null, scale = 0.6f },  // 阶段2
            new ShadowConfig { sprite = null, scale = 0.8f },  // 阶段3
            new ShadowConfig { sprite = null, scale = 0.9f },  // 阶段4
            new ShadowConfig { sprite = null, scale = 1.0f }   // 阶段5
        };
    }
}

/// <summary>
/// 季节 Sprite 集合
/// 包含5种植被季节的 Sprite
/// </summary>
[System.Serializable]
public class SeasonSpriteSet
{
    [Tooltip("早春 Sprite")]
    public Sprite earlySpring;
    
    [Tooltip("晚春早夏 Sprite")]
    public Sprite lateSpringEarlySummer;
    
    [Tooltip("晚夏早秋 Sprite")]
    public Sprite lateSummerEarlyFall;
    
    [Tooltip("晚秋 Sprite")]
    public Sprite lateFall;
    
    [Tooltip("冬季 Sprite（挂冰状态）")]
    public Sprite winter;
    
    /// <summary>
    /// 根据植被季节获取对应的 Sprite
    /// </summary>
    public Sprite GetSprite(SeasonManager.VegetationSeason season)
    {
        return season switch
        {
            SeasonManager.VegetationSeason.EarlySpring => earlySpring,
            SeasonManager.VegetationSeason.LateSpringEarlySummer => lateSpringEarlySummer,
            SeasonManager.VegetationSeason.LateSummerEarlyFall => lateSummerEarlyFall,
            SeasonManager.VegetationSeason.LateFall => lateFall,
            SeasonManager.VegetationSeason.Winter => winter,
            _ => earlySpring
        };
    }
}

/// <summary>
/// 单个阶段的 Sprite 数据
/// 包含正常状态、枯萎状态、冬季状态的 Sprite
/// </summary>
[System.Serializable]
public class StageSpriteData
{
    [Header("━━━━ 正常状态 ━━━━")]
    [Tooltip("正常状态的季节 Sprite 集合")]
    public SeasonSpriteSet normal;
    
    [Header("━━━━ 枯萎状态（可选）━━━━")]
    [Tooltip("是否有枯萎状态（阶段0通常没有）")]
    public bool hasWitheredState = false;
    
    [Tooltip("枯萎状态的 Sprite（夏季枯萎）")]
    public Sprite witheredSummer;
    
    [Tooltip("枯萎状态的 Sprite（秋季枯萎）")]
    public Sprite witheredFall;
    
    [Header("━━━━ 树桩状态（可选）━━━━")]
    [Tooltip("是否有树桩（阶段3-5有）")]
    public bool hasStump = false;
    
    [Tooltip("春夏树桩 Sprite")]
    public Sprite stumpSpringSummer;
    
    [Tooltip("秋季树桩 Sprite")]
    public Sprite stumpFall;
    
    [Tooltip("冬季树桩 Sprite")]
    public Sprite stumpWinter;
    
    /// <summary>
    /// 获取树桩 Sprite
    /// </summary>
    public Sprite GetStumpSprite(SeasonManager.VegetationSeason season)
    {
        if (!hasStump) return null;
        
        return season switch
        {
            SeasonManager.VegetationSeason.EarlySpring => stumpSpringSummer,
            SeasonManager.VegetationSeason.LateSpringEarlySummer => stumpSpringSummer,
            SeasonManager.VegetationSeason.LateSummerEarlyFall => stumpFall,
            SeasonManager.VegetationSeason.LateFall => stumpFall,
            SeasonManager.VegetationSeason.Winter => stumpWinter,
            _ => stumpSpringSummer
        };
    }
    
    /// <summary>
    /// 获取枯萎 Sprite
    /// </summary>
    public Sprite GetWitheredSprite(SeasonManager.VegetationSeason season)
    {
        if (!hasWitheredState) return null;
        
        return season switch
        {
            SeasonManager.VegetationSeason.EarlySpring => witheredSummer, // 早春不应有枯萎，降级
            SeasonManager.VegetationSeason.LateSpringEarlySummer => witheredSummer,
            SeasonManager.VegetationSeason.LateSummerEarlyFall => witheredFall,
            SeasonManager.VegetationSeason.LateFall => witheredFall,
            SeasonManager.VegetationSeason.Winter => witheredFall,
            _ => witheredSummer
        };
    }
}

/// <summary>
/// 完整的树木 Sprite 数据
/// 包含6个阶段的所有 Sprite 配置
/// </summary>
[System.Serializable]
public class TreeSpriteConfig
{
    [Header("6阶段 Sprite 数据")]
    [Tooltip("阶段0：树苗")]
    public StageSpriteData stage0;
    
    [Tooltip("阶段1：小树苗")]
    public StageSpriteData stage1;
    
    [Tooltip("阶段2：中等树")]
    public StageSpriteData stage2;
    
    [Tooltip("阶段3：大树")]
    public StageSpriteData stage3;
    
    [Tooltip("阶段4：成熟树")]
    public StageSpriteData stage4;
    
    [Tooltip("阶段5：完全成熟")]
    public StageSpriteData stage5;
    
    /// <summary>
    /// 根据阶段索引获取 Sprite 数据
    /// </summary>
    public StageSpriteData GetStageData(int stageIndex)
    {
        return stageIndex switch
        {
            0 => stage0,
            1 => stage1,
            2 => stage2,
            3 => stage3,
            4 => stage4,
            5 => stage5,
            _ => stage0
        };
    }
}
