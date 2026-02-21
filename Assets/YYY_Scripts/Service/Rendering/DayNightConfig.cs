using UnityEngine;

/// <summary>
/// 昼夜光影系统配置资产
/// 存储所有光影参数：季节颜色曲线、天气修正、过渡参数、路线 A/B 配置
/// </summary>
[CreateAssetMenu(fileName = "DayNightConfig", menuName = "Game/DayNight Config")]
public class DayNightConfig : ScriptableObject
{
    // ═══ 路线 B：颜色叠加配置 ═══

    [Header("━━━━ 季节颜色曲线（路线 B）━━━━")]
    [Tooltip("春季 24h 色调曲线")]
    public Gradient springGradient;

    [Tooltip("夏季 24h 色调曲线")]
    public Gradient summerGradient;

    [Tooltip("秋季 24h 色调曲线")]
    public Gradient autumnGradient;

    [Tooltip("冬季 24h 色调曲线")]
    public Gradient winterGradient;

    [Header("━━━━ 天气修正色 ━━━━")]
    [Tooltip("雨天修正色（偏灰偏暗）")]
    public Color rainyTint = new Color(0.55f, 0.55f, 0.6f, 1f);

    [Tooltip("枯萎天修正色（偏黄偏干燥）")]
    public Color witheringTint = new Color(0.85f, 0.75f, 0.45f, 1f);

    [Tooltip("天气修正强度 0-1")]
    [Range(0f, 1f)]
    public float weatherTintStrength = 0.5f;

    [Header("━━━━ 过渡参数 ━━━━")]
    [Tooltip("天气过渡时长（秒）")]
    public float weatherTransitionDuration = 1.5f;

    [Tooltip("时间跳跃过渡时长（秒）")]
    public float timeJumpTransitionDuration = 0.8f;

    [Tooltip("季节过渡时长（秒）")]
    public float seasonTransitionDuration = 2.0f;

    // ═══ 路线 A：URP 光照配置 ═══

    [Header("━━━━ 全局光照曲线（路线 A）━━━━")]
    [Tooltip("24h 光照强度曲线")]
    public AnimationCurve globalLightIntensityCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Tooltip("24h 光照颜色曲线")]
    public Gradient globalLightColorGradient;

    [Header("━━━━ 局部光源（路线 A）━━━━")]
    [Tooltip("夜间光源激活时间（小时）")]
    public float nightLightActivateHour = 18f;

    [Tooltip("夜间光源关闭时间（小时）")]
    public float nightLightDeactivateHour = 6f;

    [Tooltip("点光源淡入淡出时长（秒）")]
    public float pointLightFadeDuration = 1.0f;

    [Header("━━━━ 路线融合 ━━━━")]
    [Tooltip("路线 A 启用时路线 B 的叠加强度")]
    [Range(0f, 1f)]
    public float overlayStrengthWithURP = 0.4f;

    [Tooltip("仅路线 B 时的叠加强度")]
    [Range(0f, 1f)]
    public float overlayStrengthWithoutURP = 1.0f;

    /// <summary>
    /// 根据季节获取对应的 24h 颜色 Gradient
    /// </summary>
    /// <param name="season">日历季节</param>
    /// <returns>该季节的颜色曲线，若未设置则返回空 Gradient</returns>
    public Gradient GetSeasonGradient(SeasonManager.Season season)
    {
        switch (season)
        {
            case SeasonManager.Season.Spring:
                return springGradient;
            case SeasonManager.Season.Summer:
                return summerGradient;
            case SeasonManager.Season.Autumn:
                return autumnGradient;
            case SeasonManager.Season.Winter:
                return winterGradient;
            default:
                Debug.LogWarning($"[DayNightConfig] 未知季节: {season}，回退到春季曲线");
                return springGradient;
        }
    }
}
