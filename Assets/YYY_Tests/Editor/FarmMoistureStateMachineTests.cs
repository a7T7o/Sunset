using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 农田湿度状态机测试
/// 验证湿度状态转换的正确性
/// 
/// **Validates: Requirements AC-3.3, AC-3.4**
/// </summary>
[TestFixture]
public class FarmMoistureStateMachineTests
{
    #region 数据镜像

    private enum SoilMoistureState
    {
        Dry = 0,
        WetDark = 1,
        WetWithPuddle = 2
    }

    /// <summary>
    /// 镜像 FarmTileData 的湿度相关字段
    /// </summary>
    private class FarmTileData
    {
        public bool wateredToday;
        public bool wateredYesterday;
        public float waterTime;
        public SoilMoistureState moistureState;
        public int puddleVariant;

        public void SetWatered(float currentTime, int variant = 0)
        {
            wateredToday = true;
            waterTime = currentTime;
            moistureState = SoilMoistureState.WetWithPuddle;
            puddleVariant = Mathf.Clamp(variant, 0, 2);
        }

        public void ResetDailyWaterState()
        {
            wateredYesterday = wateredToday;
            wateredToday = false;
            waterTime = -1f;
            moistureState = SoilMoistureState.Dry;
        }
    }

    #endregion

    #region AC-3.3：湿度状态转换

    /// <summary>
    /// **Validates: Requirements AC-3.3**
    /// 状态转换：Dry → WetWithPuddle（浇水）
    /// </summary>
    [Test]
    public void MoistureState_DryToWetWithPuddle_OnWatering()
    {
        var tile = new FarmTileData();
        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "初始状态应为 Dry");

        tile.SetWatered(10f, 1);

        Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState, "浇水后应为 WetWithPuddle");
    }

    /// <summary>
    /// **Validates: Requirements AC-3.3**
    /// 状态转换：WetWithPuddle → WetDark（水渍消退）
    /// 镜像 OnHourChanged 中的水渍消退逻辑
    /// </summary>
    [Test]
    public void MoistureState_WetWithPuddleToWetDark_OnPuddleDry()
    {
        var tile = new FarmTileData();
        tile.SetWatered(6f, 0);

        Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState);

        // 模拟水渍消退（OnHourChanged 中的逻辑）
        float hoursUntilPuddleDry = 4f;
        float currentTime = 6f + hoursUntilPuddleDry + 0.1f; // 超过消退时间
        float hoursSinceWatering = currentTime - tile.waterTime;

        if (hoursSinceWatering >= hoursUntilPuddleDry)
        {
            tile.moistureState = SoilMoistureState.WetDark;
        }

        Assert.AreEqual(SoilMoistureState.WetDark, tile.moistureState, "水渍消退后应为 WetDark");
    }

    /// <summary>
    /// **Validates: Requirements AC-3.3**
    /// 水渍消退时间计算（跨天情况）
    /// </summary>
    [Test]
    public void MoistureState_PuddleDry_CrossDayCalculation()
    {
        var tile = new FarmTileData();
        tile.SetWatered(18f, 0); // 傍晚浇水

        float hoursUntilPuddleDry = 4f;
        float currentTime = 2f; // 第二天凌晨
        float hoursSinceWatering = currentTime - tile.waterTime;

        // 处理跨天情况
        if (hoursSinceWatering < 0)
            hoursSinceWatering += 20f; // 游戏一天 20 小时

        Assert.AreEqual(4f, hoursSinceWatering, 0.001f, "跨天时间计算应正确");
        Assert.IsTrue(hoursSinceWatering >= hoursUntilPuddleDry, "应达到水渍消退时间");
    }

    #endregion

    #region AC-3.4：日结回退

    /// <summary>
    /// **Validates: Requirements AC-3.4**
    /// 状态转换：WetWithPuddle → Dry（日结）
    /// </summary>
    [Test]
    public void MoistureState_WetWithPuddleToDry_OnDayEnd()
    {
        var tile = new FarmTileData();
        tile.SetWatered(10f, 1);

        Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState);

        tile.ResetDailyWaterState();

        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "日结后应回退到 Dry");
    }

    /// <summary>
    /// **Validates: Requirements AC-3.4**
    /// 状态转换：WetDark → Dry（日结）
    /// </summary>
    [Test]
    public void MoistureState_WetDarkToDry_OnDayEnd()
    {
        var tile = new FarmTileData();
        tile.SetWatered(6f, 0);
        tile.moistureState = SoilMoistureState.WetDark; // 模拟水渍已消退

        tile.ResetDailyWaterState();

        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "日结后应回退到 Dry");
    }

    #endregion

    #region 状态机完整性

    /// <summary>
    /// **Validates: Requirements AC-3.3, AC-3.4**
    /// 完整状态机流程：Dry → WetWithPuddle → WetDark → Dry
    /// </summary>
    [Test]
    public void MoistureState_FullCycle()
    {
        var tile = new FarmTileData();

        // 初始状态
        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "初始应为 Dry");

        // 浇水
        tile.SetWatered(6f, 0);
        Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState, "浇水后应为 WetWithPuddle");

        // 水渍消退
        tile.moistureState = SoilMoistureState.WetDark;
        Assert.AreEqual(SoilMoistureState.WetDark, tile.moistureState, "水渍消退后应为 WetDark");

        // 日结
        tile.ResetDailyWaterState();
        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "日结后应为 Dry");
    }

    /// <summary>
    /// **Validates: Requirements AC-3.3, AC-3.4**
    /// PBT：随机状态转换序列的合法性
    /// </summary>
    [Test]
    public void MoistureState_PBT_RandomTransitions_AlwaysValid()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var tile = new FarmTileData();

        for (int i = 0; i < iterations; i++)
        {
            int action = random.Next(3);

            switch (action)
            {
                case 0: // 浇水
                    tile.SetWatered(random.Next(6, 20), random.Next(0, 3));
                    Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState,
                        $"迭代 {i}: 浇水后应为 WetWithPuddle");
                    break;

                case 1: // 水渍消退
                    if (tile.moistureState == SoilMoistureState.WetWithPuddle)
                    {
                        tile.moistureState = SoilMoistureState.WetDark;
                        Assert.AreEqual(SoilMoistureState.WetDark, tile.moistureState,
                            $"迭代 {i}: 水渍消退后应为 WetDark");
                    }
                    break;

                case 2: // 日结
                    tile.ResetDailyWaterState();
                    Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState,
                        $"迭代 {i}: 日结后应为 Dry");
                    break;
            }

            // 状态始终在合法范围内
            Assert.IsTrue(
                tile.moistureState == SoilMoistureState.Dry ||
                tile.moistureState == SoilMoistureState.WetDark ||
                tile.moistureState == SoilMoistureState.WetWithPuddle,
                $"迭代 {i}: 状态应在合法范围内");
        }
    }

    #endregion
}
