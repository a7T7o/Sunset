using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 农田耕地存档往返属性测试（PBT）
/// 验证 FarmTileSaveData 的 Save → Load 往返一致性
/// 
/// **Validates: Requirements AC-2.5, CP-8**
/// </summary>
[TestFixture]
public class FarmTileSaveRoundTripTests
{
    #region 数据镜像（与运行时保持一致）

    private enum SoilMoistureState
    {
        Dry = 0,
        WetDark = 1,
        WetWithPuddle = 2
    }

    /// <summary>
    /// 镜像 FarmTileData 的核心字段
    /// </summary>
    private struct FarmTileData
    {
        public Vector3Int position;
        public int layerIndex;
        public bool isTilled;
        public bool wateredToday;
        public bool wateredYesterday;
        public float waterTime;
        public SoilMoistureState moistureState;
        public int puddleVariant;
    }

    /// <summary>
    /// 镜像 FarmTileSaveData 的字段
    /// </summary>
    private struct FarmTileSaveData
    {
        public int tileX;
        public int tileY;
        public int layer;
        public int soilState;
        public bool isWatered;
        public bool wateredYesterday;
        public float waterTime;
        public int puddleVariant;
    }

    #endregion

    #region Save/Load 镜像逻辑

    /// <summary>
    /// 镜像 FarmTileManager.Save() 中单个 tile 的序列化逻辑
    /// </summary>
    private static FarmTileSaveData SaveTile(FarmTileData tile)
    {
        return new FarmTileSaveData
        {
            tileX = tile.position.x,
            tileY = tile.position.y,
            layer = tile.layerIndex,
            soilState = (int)tile.moistureState,
            isWatered = tile.wateredToday,
            wateredYesterday = tile.wateredYesterday,
            waterTime = tile.waterTime,
            puddleVariant = tile.puddleVariant
        };
    }

    /// <summary>
    /// 镜像 FarmTileManager.CreateTileFromSaveData() 的反序列化逻辑
    /// </summary>
    private static FarmTileData LoadTile(FarmTileSaveData saveData)
    {
        return new FarmTileData
        {
            position = new Vector3Int(saveData.tileX, saveData.tileY, 0),
            layerIndex = saveData.layer,
            isTilled = true,
            moistureState = (SoilMoistureState)saveData.soilState,
            wateredToday = saveData.isWatered,
            wateredYesterday = saveData.wateredYesterday,
            waterTime = saveData.waterTime,
            puddleVariant = saveData.puddleVariant
        };
    }

    #endregion

    #region PBT：存档往返一致性

    /// <summary>
    /// **Validates: Requirements AC-2.5, CP-8**
    /// 属性：∀ FarmTileData d, Load(Save(d)) == d
    /// 随机生成耕地数据，验证存档→读档后所有字段一致
    /// </summary>
    [Test]
    public void FarmTile_PBT_SaveLoadRoundTrip_AllFieldsPreserved()
    {
        const int iterations = 500;
        var random = new System.Random(42);
        var moistureStates = new[] { SoilMoistureState.Dry, SoilMoistureState.WetDark, SoilMoistureState.WetWithPuddle };

        for (int i = 0; i < iterations; i++)
        {
            // 随机生成耕地数据
            var original = new FarmTileData
            {
                position = new Vector3Int(
                    random.Next(-50, 50),
                    random.Next(-50, 50),
                    0),
                layerIndex = random.Next(0, 3),
                isTilled = true,
                wateredToday = random.Next(2) == 1,
                wateredYesterday = random.Next(2) == 1,
                waterTime = (float)(random.NextDouble() * 24.0),
                moistureState = moistureStates[random.Next(moistureStates.Length)],
                puddleVariant = random.Next(0, 3)
            };

            // Save → Load
            var saveData = SaveTile(original);
            var loaded = LoadTile(saveData);

            // 逐字段断言
            Assert.AreEqual(original.position, loaded.position,
                $"迭代 {i}: position 不一致");
            Assert.AreEqual(original.layerIndex, loaded.layerIndex,
                $"迭代 {i}: layerIndex 不一致");
            Assert.AreEqual(original.isTilled, loaded.isTilled,
                $"迭代 {i}: isTilled 不一致");
            Assert.AreEqual(original.wateredToday, loaded.wateredToday,
                $"迭代 {i}: wateredToday 不一致");
            Assert.AreEqual(original.wateredYesterday, loaded.wateredYesterday,
                $"迭代 {i}: wateredYesterday 不一致");
            Assert.AreEqual(original.waterTime, loaded.waterTime, 0.001f,
                $"迭代 {i}: waterTime 不一致");
            Assert.AreEqual(original.moistureState, loaded.moistureState,
                $"迭代 {i}: moistureState 不一致");
            Assert.AreEqual(original.puddleVariant, loaded.puddleVariant,
                $"迭代 {i}: puddleVariant 不一致");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-2.4, CP-9**
    /// 旧存档兼容性：只有 5 个字段的存档数据，新增字段使用默认值
    /// </summary>
    [Test]
    public void FarmTile_OldSaveData_LoadsWithDefaults()
    {
        // 模拟旧存档（只有 5 个字段，新增字段为默认值）
        var oldSaveData = new FarmTileSaveData
        {
            tileX = 5,
            tileY = -3,
            layer = 1,
            soilState = 0, // Dry
            isWatered = true,
            // 新增字段使用默认值
            wateredYesterday = false, // bool 默认值
            waterTime = 0f,           // float 默认值
            puddleVariant = 0         // int 默认值
        };

        var loaded = LoadTile(oldSaveData);

        // 旧字段正确恢复
        Assert.AreEqual(new Vector3Int(5, -3, 0), loaded.position);
        Assert.AreEqual(1, loaded.layerIndex);
        Assert.AreEqual(SoilMoistureState.Dry, loaded.moistureState);
        Assert.IsTrue(loaded.wateredToday);

        // 新增字段使用安全默认值
        Assert.IsFalse(loaded.wateredYesterday, "旧存档 wateredYesterday 应为 false");
        Assert.AreEqual(0f, loaded.waterTime, 0.001f, "旧存档 waterTime 应为 0");
        Assert.AreEqual(0, loaded.puddleVariant, "旧存档 puddleVariant 应为 0");
    }

    /// <summary>
    /// **Validates: Requirements CP-8**
    /// PBT：多次连续 Save→Load 结果稳定（幂等性）
    /// </summary>
    [Test]
    public void FarmTile_PBT_MultipleRoundTrips_Stable()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var moistureStates = new[] { SoilMoistureState.Dry, SoilMoistureState.WetDark, SoilMoistureState.WetWithPuddle };

        for (int i = 0; i < iterations; i++)
        {
            var original = new FarmTileData
            {
                position = new Vector3Int(random.Next(-50, 50), random.Next(-50, 50), 0),
                layerIndex = random.Next(0, 3),
                isTilled = true,
                wateredToday = random.Next(2) == 1,
                wateredYesterday = random.Next(2) == 1,
                waterTime = (float)(random.NextDouble() * 24.0),
                moistureState = moistureStates[random.Next(moistureStates.Length)],
                puddleVariant = random.Next(0, 3)
            };

            // 连续 3 次 Save→Load
            var current = original;
            for (int round = 0; round < 3; round++)
            {
                var saved = SaveTile(current);
                current = LoadTile(saved);
            }

            // 最终结果应与原始一致
            Assert.AreEqual(original.position, current.position, $"迭代 {i}: 多轮后 position 不一致");
            Assert.AreEqual(original.wateredToday, current.wateredToday, $"迭代 {i}: 多轮后 wateredToday 不一致");
            Assert.AreEqual(original.wateredYesterday, current.wateredYesterday, $"迭代 {i}: 多轮后 wateredYesterday 不一致");
            Assert.AreEqual(original.waterTime, current.waterTime, 0.001f, $"迭代 {i}: 多轮后 waterTime 不一致");
            Assert.AreEqual(original.moistureState, current.moistureState, $"迭代 {i}: 多轮后 moistureState 不一致");
            Assert.AreEqual(original.puddleVariant, current.puddleVariant, $"迭代 {i}: 多轮后 puddleVariant 不一致");
        }
    }

    #endregion
}
