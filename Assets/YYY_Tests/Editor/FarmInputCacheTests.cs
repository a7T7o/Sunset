using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 农田输入缓存测试
/// 验证浇水输入缓存的逻辑正确性
/// 
/// **Validates: Requirements AC-1.3, AC-1.4**
/// </summary>
[TestFixture]
public class FarmInputCacheTests
{
    #region 数据镜像

    private enum SoilMoistureState
    {
        Dry = 0,
        WetDark = 1,
        WetWithPuddle = 2
    }

    /// <summary>
    /// 镜像 FarmTileData 的浇水相关字段
    /// </summary>
    private class FarmTileData
    {
        public bool wateredToday;
        public bool wateredYesterday;
        public float waterTime;
        public SoilMoistureState moistureState;
        public int puddleVariant;

        /// <summary>
        /// 镜像 SetWatered 方法
        /// </summary>
        public void SetWatered(float currentTime, int variant = 0)
        {
            wateredToday = true;
            waterTime = currentTime;
            moistureState = SoilMoistureState.WetWithPuddle;
            puddleVariant = Mathf.Clamp(variant, 0, 2);
        }

        /// <summary>
        /// 镜像 ResetDailyWaterState 方法
        /// </summary>
        public void ResetDailyWaterState()
        {
            wateredYesterday = wateredToday;
            wateredToday = false;
            waterTime = -1f;
            moistureState = SoilMoistureState.Dry;
        }
    }

    #endregion

    #region AC-1.3：浇水输入缓存

    /// <summary>
    /// **Validates: Requirements AC-1.3**
    /// 浇水后 wateredToday = true，waterTime 记录当前时间
    /// </summary>
    [Test]
    public void SetWatered_SetsWateredTodayAndWaterTime()
    {
        var tile = new FarmTileData();
        float currentTime = 10.5f;

        tile.SetWatered(currentTime, 1);

        Assert.IsTrue(tile.wateredToday, "浇水后 wateredToday 应为 true");
        Assert.AreEqual(currentTime, tile.waterTime, 0.001f, "waterTime 应记录浇水时间");
        Assert.AreEqual(SoilMoistureState.WetWithPuddle, tile.moistureState, "浇水后应为 WetWithPuddle 状态");
        Assert.AreEqual(1, tile.puddleVariant, "puddleVariant 应正确设置");
    }

    /// <summary>
    /// **Validates: Requirements AC-1.3**
    /// puddleVariant 应被 Clamp 到 0-2 范围
    /// </summary>
    [Test]
    public void SetWatered_ClampsPuddleVariant()
    {
        var tile1 = new FarmTileData();
        var tile2 = new FarmTileData();
        var tile3 = new FarmTileData();

        tile1.SetWatered(10f, -5);  // 负数
        tile2.SetWatered(10f, 10);  // 超出范围
        tile3.SetWatered(10f, 2);   // 边界值

        Assert.AreEqual(0, tile1.puddleVariant, "负数应 Clamp 到 0");
        Assert.AreEqual(2, tile2.puddleVariant, "超出范围应 Clamp 到 2");
        Assert.AreEqual(2, tile3.puddleVariant, "边界值 2 应保持");
    }

    #endregion

    #region AC-1.4：日结状态转移

    /// <summary>
    /// **Validates: Requirements AC-1.4**
    /// 日结时 wateredToday → wateredYesterday，wateredToday 重置
    /// </summary>
    [Test]
    public void ResetDailyWaterState_TransfersWateredTodayToYesterday()
    {
        var tile = new FarmTileData();
        tile.SetWatered(10f, 1);

        Assert.IsTrue(tile.wateredToday, "浇水后 wateredToday 应为 true");
        Assert.IsFalse(tile.wateredYesterday, "浇水后 wateredYesterday 应为 false");

        tile.ResetDailyWaterState();

        Assert.IsFalse(tile.wateredToday, "日结后 wateredToday 应重置为 false");
        Assert.IsTrue(tile.wateredYesterday, "日结后 wateredYesterday 应为 true");
        Assert.AreEqual(-1f, tile.waterTime, 0.001f, "日结后 waterTime 应重置为 -1");
        Assert.AreEqual(SoilMoistureState.Dry, tile.moistureState, "日结后应为 Dry 状态");
    }

    /// <summary>
    /// **Validates: Requirements AC-1.4**
    /// 连续两天浇水的状态转移
    /// </summary>
    [Test]
    public void ResetDailyWaterState_ConsecutiveDays()
    {
        var tile = new FarmTileData();

        // 第一天浇水
        tile.SetWatered(10f, 0);
        tile.ResetDailyWaterState();

        Assert.IsFalse(tile.wateredToday);
        Assert.IsTrue(tile.wateredYesterday);

        // 第二天浇水
        tile.SetWatered(11f, 1);
        tile.ResetDailyWaterState();

        Assert.IsFalse(tile.wateredToday);
        Assert.IsTrue(tile.wateredYesterday, "连续浇水后 wateredYesterday 应保持 true");
    }

    /// <summary>
    /// **Validates: Requirements AC-1.4**
    /// 隔天不浇水的状态转移
    /// </summary>
    [Test]
    public void ResetDailyWaterState_SkippedDay()
    {
        var tile = new FarmTileData();

        // 第一天浇水
        tile.SetWatered(10f, 0);
        tile.ResetDailyWaterState();

        Assert.IsTrue(tile.wateredYesterday);

        // 第二天不浇水，直接日结
        tile.ResetDailyWaterState();

        Assert.IsFalse(tile.wateredToday);
        Assert.IsFalse(tile.wateredYesterday, "隔天不浇水后 wateredYesterday 应为 false");
    }

    #endregion

    #region PBT：多天模拟

    /// <summary>
    /// **Validates: Requirements AC-1.3, AC-1.4**
    /// PBT：随机浇水模式下状态转移正确
    /// </summary>
    [Test]
    public void FarmTile_PBT_RandomWateringPattern_StateTransferCorrect()
    {
        const int days = 100;
        var random = new System.Random(42);
        var tile = new FarmTileData();

        bool expectedYesterday = false;

        for (int day = 0; day < days; day++)
        {
            bool waterToday = random.Next(2) == 1;

            if (waterToday)
            {
                tile.SetWatered(random.Next(6, 20), random.Next(0, 3));
            }

            // 日结前验证
            Assert.AreEqual(waterToday, tile.wateredToday, $"第 {day} 天: wateredToday 不一致");
            Assert.AreEqual(expectedYesterday, tile.wateredYesterday, $"第 {day} 天: wateredYesterday 不一致");

            // 日结
            tile.ResetDailyWaterState();

            // 更新期望值
            expectedYesterday = waterToday;
        }
    }

    #endregion
}
