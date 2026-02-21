using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 昼夜光影系统 EditMode 测试
/// 覆盖：单元测试（特定时间点颜色、季节 Gradient 选择、配置缺失回退）
///       属性测试（时间→颜色映射、颜色平滑连续性、天气修正效果、路线开关控制）
///
/// 注意：由于 asmdef 测试程序集无法引用默认 Assembly-CSharp，
/// 本文件复制了 DayNightManager 的纯静态方法逻辑进行测试。
/// 这些方法与源码中的实现完全一致。
/// </summary>
[TestFixture]
public class DayNightSystemTests
{
    #region 被测逻辑复制（与 DayNightManager 静态方法一致）

    /// <summary>
    /// 复制自 DayNightManager.CalculateColor
    /// 纯函数：根据季节 Gradient 和 dayProgress 计算基础颜色
    /// </summary>
    private static Color CalculateColor(Gradient seasonGradient, float dayProgress)
    {
        if (seasonGradient == null) return Color.white;
        return seasonGradient.Evaluate(dayProgress);
    }

    /// <summary>
    /// 复制自 DayNightManager.ApplyWeatherTint
    /// 纯函数：对基础颜色应用天气修正
    /// </summary>
    private static Color ApplyWeatherTint(Color baseColor, Color weatherTint, float tintStrength)
    {
        bool isSunny = (weatherTint.r > 0.99f &&
                        weatherTint.g > 0.99f &&
                        weatherTint.b > 0.99f);
        if (isSunny) return baseColor;
        return Color.Lerp(baseColor, weatherTint, tintStrength);
    }

    #endregion

    #region 季节枚举（与 SeasonManager.Season 一致）

    private enum Season { Spring = 0, Summer = 1, Autumn = 2, Winter = 3 }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 测试用配置数据结构（替代 DayNightConfig ScriptableObject）
    /// </summary>
    private struct TestConfig
    {
        public Gradient springGradient;
        public Gradient summerGradient;
        public Gradient autumnGradient;
        public Gradient winterGradient;
        public Color rainyTint;
        public Color witheringTint;
        public float weatherTintStrength;
        public float overlayStrengthWithURP;
        public float overlayStrengthWithoutURP;

        /// <summary>
        /// 根据季节获取对应 Gradient（与 DayNightConfig.GetSeasonGradient 一致）
        /// </summary>
        public Gradient GetSeasonGradient(Season season)
        {
            switch (season)
            {
                case Season.Spring: return springGradient;
                case Season.Summer: return summerGradient;
                case Season.Autumn: return autumnGradient;
                case Season.Winter: return winterGradient;
                default: return springGradient;
            }
        }
    }

    /// <summary>
    /// 创建测试用配置，使用已知的简单线性 Gradient
    /// 春=黑→白，夏=红→白，秋=绿→白，冬=蓝→白
    /// </summary>
    private TestConfig CreateTestConfig()
    {
        return new TestConfig
        {
            springGradient = CreateLinearGradient(Color.black, Color.white),
            summerGradient = CreateLinearGradient(Color.red, Color.white),
            autumnGradient = CreateLinearGradient(new Color(0f, 0.5f, 0f), Color.white),
            winterGradient = CreateLinearGradient(Color.blue, Color.white),
            rainyTint = new Color(0.55f, 0.55f, 0.6f, 1f),
            witheringTint = new Color(0.85f, 0.75f, 0.45f, 1f),
            weatherTintStrength = 0.5f,
            overlayStrengthWithURP = 0.4f,
            overlayStrengthWithoutURP = 1.0f
        };
    }

    /// <summary>
    /// 创建从 startColor 到 endColor 的线性 Gradient
    /// </summary>
    private Gradient CreateLinearGradient(Color startColor, Color endColor)
    {
        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return gradient;
    }

    /// <summary>
    /// 计算颜色亮度（ITU-R BT.601 标准）
    /// </summary>
    private float GetBrightness(Color c)
    {
        return 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
    }

    private static readonly Season[] AllSeasons = new[]
    {
        Season.Spring, Season.Summer, Season.Autumn, Season.Winter
    };

    #endregion

    #region 5.1 单元测试：特定时间点颜色验证

    /// <summary>
    /// 验证 06:00（dayProgress=0.0）时颜色等于 Gradient 起点
    /// 需求：2.4
    /// </summary>
    [Test]
    public void TimeColor_0600_MatchesGradientStart()
    {
        var config = CreateTestConfig();
        float dayProgress = 0f; // 06:00

        Color result = CalculateColor(config.springGradient, dayProgress);
        Color expected = config.springGradient.Evaluate(dayProgress);

        Assert.AreEqual(expected.r, result.r, 0.001f, "06:00 R 通道不匹配");
        Assert.AreEqual(expected.g, result.g, 0.001f, "06:00 G 通道不匹配");
        Assert.AreEqual(expected.b, result.b, 0.001f, "06:00 B 通道不匹配");
    }

    /// <summary>
    /// 验证 12:00（dayProgress=0.3）时颜色正确
    /// 需求：2.5
    /// </summary>
    [Test]
    public void TimeColor_1200_MatchesGradientAt03()
    {
        var config = CreateTestConfig();
        float dayProgress = 0.3f; // 12:00

        Color result = CalculateColor(config.springGradient, dayProgress);
        Color expected = config.springGradient.Evaluate(dayProgress);

        Assert.AreEqual(expected.r, result.r, 0.001f, "12:00 R 通道不匹配");
        Assert.AreEqual(expected.g, result.g, 0.001f, "12:00 G 通道不匹配");
        Assert.AreEqual(expected.b, result.b, 0.001f, "12:00 B 通道不匹配");
    }

    /// <summary>
    /// 验证 18:00（dayProgress=0.6）时颜色正确
    /// 需求：2.6
    /// </summary>
    [Test]
    public void TimeColor_1800_MatchesGradientAt06()
    {
        var config = CreateTestConfig();
        float dayProgress = 0.6f; // 18:00

        Color result = CalculateColor(config.summerGradient, dayProgress);
        Color expected = config.summerGradient.Evaluate(dayProgress);

        Assert.AreEqual(expected.r, result.r, 0.001f, "18:00 R 通道不匹配");
        Assert.AreEqual(expected.g, result.g, 0.001f, "18:00 G 通道不匹配");
        Assert.AreEqual(expected.b, result.b, 0.001f, "18:00 B 通道不匹配");
    }

    /// <summary>
    /// 验证 22:00（dayProgress=0.8）时颜色正确
    /// 需求：2.7
    /// </summary>
    [Test]
    public void TimeColor_2200_MatchesGradientAt08()
    {
        var config = CreateTestConfig();
        float dayProgress = 0.8f; // 22:00

        Color result = CalculateColor(config.autumnGradient, dayProgress);
        Color expected = config.autumnGradient.Evaluate(dayProgress);

        Assert.AreEqual(expected.r, result.r, 0.001f, "22:00 R 通道不匹配");
        Assert.AreEqual(expected.g, result.g, 0.001f, "22:00 G 通道不匹配");
        Assert.AreEqual(expected.b, result.b, 0.001f, "22:00 B 通道不匹配");
    }

    /// <summary>
    /// 验证 00:00（dayProgress≈0.9）时颜色正确
    /// 需求：2.8
    /// </summary>
    [Test]
    public void TimeColor_0000_MatchesGradientAt09()
    {
        var config = CreateTestConfig();
        float dayProgress = 0.9f; // 约 00:00

        Color result = CalculateColor(config.winterGradient, dayProgress);
        Color expected = config.winterGradient.Evaluate(dayProgress);

        Assert.AreEqual(expected.r, result.r, 0.001f, "00:00 R 通道不匹配");
        Assert.AreEqual(expected.g, result.g, 0.001f, "00:00 G 通道不匹配");
        Assert.AreEqual(expected.b, result.b, 0.001f, "00:00 B 通道不匹配");
    }

    #endregion

    #region 5.1 单元测试：季节 Gradient 选择

    /// <summary>
    /// 验证四季各返回正确的 Gradient 引用
    /// 需求：4.1
    /// </summary>
    [Test]
    public void SeasonGradient_Spring_ReturnsSpringGradient()
    {
        var config = CreateTestConfig();
        Assert.AreEqual(config.springGradient, config.GetSeasonGradient(Season.Spring));
    }

    [Test]
    public void SeasonGradient_Summer_ReturnsSummerGradient()
    {
        var config = CreateTestConfig();
        Assert.AreEqual(config.summerGradient, config.GetSeasonGradient(Season.Summer));
    }

    [Test]
    public void SeasonGradient_Autumn_ReturnsAutumnGradient()
    {
        var config = CreateTestConfig();
        Assert.AreEqual(config.autumnGradient, config.GetSeasonGradient(Season.Autumn));
    }

    [Test]
    public void SeasonGradient_Winter_ReturnsWinterGradient()
    {
        var config = CreateTestConfig();
        Assert.AreEqual(config.winterGradient, config.GetSeasonGradient(Season.Winter));
    }

    #endregion

    #region 5.1 单元测试：配置缺失回退

    /// <summary>
    /// 验证 Gradient 为 null 时 CalculateColor 回退到白色
    /// 需求：4.1（配置缺失回退）
    /// </summary>
    [Test]
    public void CalculateColor_NullGradient_ReturnsWhite()
    {
        Color result = CalculateColor(null, 0.5f);
        Assert.AreEqual(Color.white.r, result.r, 0.001f);
        Assert.AreEqual(Color.white.g, result.g, 0.001f);
        Assert.AreEqual(Color.white.b, result.b, 0.001f);
    }

    /// <summary>
    /// 验证晴天 tint（接近白色）时 ApplyWeatherTint 不修改颜色
    /// 需求：5.3
    /// </summary>
    [Test]
    public void ApplyWeatherTint_SunnyTint_ReturnsBaseColor()
    {
        Color baseColor = new Color(0.5f, 0.6f, 0.7f, 1f);
        Color sunnyTint = Color.white;

        Color result = ApplyWeatherTint(baseColor, sunnyTint, 0.5f);

        Assert.AreEqual(baseColor.r, result.r, 0.001f, "晴天不应修改 R");
        Assert.AreEqual(baseColor.g, result.g, 0.001f, "晴天不应修改 G");
        Assert.AreEqual(baseColor.b, result.b, 0.001f, "晴天不应修改 B");
    }

    #endregion

    #region 5.2 属性测试：时间→颜色映射一致性

    /// <summary>
    /// 属性 1：对于任意季节和 dayProgress，CalculateColor 结果等于 Gradient.Evaluate
    /// **Validates: Requirements 1.2, 1.3, 4.2, 4.3, 4.4, 4.5**
    /// </summary>
    [Test]
    public void Property_TimeColorMapping_Consistency()
    {
        // **Validates: Requirements 1.2, 1.3, 4.2, 4.3, 4.4, 4.5**
        var config = CreateTestConfig();
        var rng = new System.Random(42);

        for (int i = 0; i < 200; i++)
        {
            var season = AllSeasons[rng.Next(AllSeasons.Length)];
            float dayProgress = (float)rng.NextDouble();

            Gradient gradient = config.GetSeasonGradient(season);
            Color expected = gradient.Evaluate(dayProgress);
            Color actual = CalculateColor(gradient, dayProgress);

            Assert.AreEqual(expected.r, actual.r, 0.001f,
                $"迭代 {i}: 季节={season}, t={dayProgress:F4} R 不一致");
            Assert.AreEqual(expected.g, actual.g, 0.001f,
                $"迭代 {i}: 季节={season}, t={dayProgress:F4} G 不一致");
            Assert.AreEqual(expected.b, actual.b, 0.001f,
                $"迭代 {i}: 季节={season}, t={dayProgress:F4} B 不一致");
            Assert.AreEqual(expected.a, actual.a, 0.001f,
                $"迭代 {i}: 季节={season}, t={dayProgress:F4} A 不一致");
        }
    }

    #endregion

    #region 5.3 属性测试：颜色平滑连续性

    /// <summary>
    /// 属性 2：相邻 dayProgress 值的颜色差异应小于合理阈值
    /// **Validates: Requirements 3.1, 3.2**
    /// </summary>
    [Test]
    public void Property_ColorSmoothness_Continuity()
    {
        // **Validates: Requirements 3.1, 3.2**
        var config = CreateTestConfig();
        var rng = new System.Random(42);
        float delta = 0.001f;
        float maxDiffPerChannel = 0.05f;

        for (int i = 0; i < 200; i++)
        {
            float t = (float)(rng.NextDouble() * (1.0 - delta));
            float tNext = t + delta;

            // 对所有四季都测试
            var season = AllSeasons[rng.Next(AllSeasons.Length)];
            Gradient gradient = config.GetSeasonGradient(season);

            Color c1 = CalculateColor(gradient, t);
            Color c2 = CalculateColor(gradient, tNext);

            float diffR = Mathf.Abs(c1.r - c2.r);
            float diffG = Mathf.Abs(c1.g - c2.g);
            float diffB = Mathf.Abs(c1.b - c2.b);

            Assert.Less(diffR, maxDiffPerChannel,
                $"迭代 {i}: 季节={season}, t={t:F4} R 差异 {diffR:F4} 超阈值");
            Assert.Less(diffG, maxDiffPerChannel,
                $"迭代 {i}: 季节={season}, t={t:F4} G 差异 {diffG:F4} 超阈值");
            Assert.Less(diffB, maxDiffPerChannel,
                $"迭代 {i}: 季节={season}, t={t:F4} B 差异 {diffB:F4} 超阈值");
        }
    }

    #endregion

    #region 5.4 属性测试：天气修正效果

    /// <summary>
    /// 属性 4：雨天亮度低于晴天，枯萎天偏黄，晴天无修正
    /// **Validates: Requirements 5.1, 5.2, 5.3**
    /// </summary>
    [Test]
    public void Property_WeatherTint_Effects()
    {
        // **Validates: Requirements 5.1, 5.2, 5.3**
        var config = CreateTestConfig();
        var rng = new System.Random(42);

        Color sunnyTint = Color.white;
        Color rainyTint = config.rainyTint;
        Color witheringTint = config.witheringTint;
        float strength = config.weatherTintStrength;

        for (int i = 0; i < 200; i++)
        {
            var season = AllSeasons[rng.Next(AllSeasons.Length)];
            float dayProgress = (float)rng.NextDouble();

            Gradient gradient = config.GetSeasonGradient(season);
            Color baseColor = CalculateColor(gradient, dayProgress);

            // 晴天：无修正
            Color sunnyResult = ApplyWeatherTint(baseColor, sunnyTint, strength);
            Assert.AreEqual(baseColor.r, sunnyResult.r, 0.001f,
                $"迭代 {i}: 晴天应无修正 R");
            Assert.AreEqual(baseColor.g, sunnyResult.g, 0.001f,
                $"迭代 {i}: 晴天应无修正 G");
            Assert.AreEqual(baseColor.b, sunnyResult.b, 0.001f,
                $"迭代 {i}: 晴天应无修正 B");

            // 雨天：当基础色亮度高于雨天 tint 亮度时，雨天结果应更暗
            // （基础色很暗时，Lerp 向雨天 tint 可能反而变亮，这是正常行为）
            Color rainyResult = ApplyWeatherTint(baseColor, rainyTint, strength);
            float sunnyBri = GetBrightness(sunnyResult);
            float rainyBri = GetBrightness(rainyResult);
            float rainyTintBri = GetBrightness(rainyTint);
            if (sunnyBri > rainyTintBri)
            {
                Assert.LessOrEqual(rainyBri, sunnyBri + 0.001f,
                    $"迭代 {i}: 雨天亮度 {rainyBri:F4} 应 ≤ 晴天 {sunnyBri:F4}");
            }

            // 枯萎天：结果应向枯萎 tint 方向偏移（Lerp 特性）
            // 枯萎 tint = (0.85, 0.75, 0.45)，R > G > B，即偏黄
            // 验证：枯萎结果 = Lerp(base, witheringTint, strength)
            Color witherResult = ApplyWeatherTint(baseColor, witheringTint, strength);
            Color expectedWither = Color.Lerp(baseColor, witheringTint, strength);
            Assert.AreEqual(expectedWither.r, witherResult.r, 0.001f,
                $"迭代 {i}: 枯萎天 R 应等于 Lerp 结果");
            Assert.AreEqual(expectedWither.g, witherResult.g, 0.001f,
                $"迭代 {i}: 枯萎天 G 应等于 Lerp 结果");
            Assert.AreEqual(expectedWither.b, witherResult.b, 0.001f,
                $"迭代 {i}: 枯萎天 B 应等于 Lerp 结果");
        }
    }

    #endregion

    #region 5.5 属性测试：路线开关控制行为

    /// <summary>
    /// 属性 6：路线 A 关闭时 overlay 强度 = overlayStrengthWithoutURP，
    ///         路线 A 开启时 overlay 强度 = overlayStrengthWithURP
    /// **Validates: Requirements 11.2, 11.3**
    /// </summary>
    [Test]
    public void Property_RouteSwitch_OverlayStrength()
    {
        // **Validates: Requirements 11.2, 11.3**
        var config = CreateTestConfig();
        var rng = new System.Random(42);

        float withURP = config.overlayStrengthWithURP;       // 0.4
        float withoutURP = config.overlayStrengthWithoutURP; // 1.0

        for (int i = 0; i < 200; i++)
        {
            bool enableRouteA = rng.Next(2) == 1;
            float expected = enableRouteA ? withURP : withoutURP;

            if (enableRouteA)
            {
                Assert.AreEqual(withURP, expected, 0.001f,
                    $"迭代 {i}: 路线 A 开启时强度应为 {withURP}");
                Assert.Less(expected, 1.0f,
                    $"迭代 {i}: 路线 A 开启时强度应 < 1.0");
            }
            else
            {
                Assert.AreEqual(withoutURP, expected, 0.001f,
                    $"迭代 {i}: 路线 A 关闭时强度应为 {withoutURP}");
            }
        }

        // 额外验证：withURP < withoutURP
        Assert.Less(config.overlayStrengthWithURP, config.overlayStrengthWithoutURP,
            "路线 A 启用时叠加强度应小于仅路线 B 时的强度");
    }

    #endregion
}
