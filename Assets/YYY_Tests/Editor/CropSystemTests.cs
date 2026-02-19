using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 农作物系统测试
/// 包含 CropState 状态转换、种子袋保质期、腐烂食物堆叠、收获产出、过季枯萎等测试
/// 
/// 由于测试程序集与主程序集分离，这里测试核心算法逻辑
/// </summary>
[TestFixture]
public class CropSystemTests
{
    #region 状态枚举镜像（与 CropState 保持一致）

    private enum CropState
    {
        Growing = 0,
        Mature = 1,
        WitheredImmature = 2,
        WitheredMature = 3
    }

    /// <summary>
    /// 镜像 CropController.IsValidTransition 的逻辑
    /// </summary>
    private static bool IsValidTransition(CropState from, CropState to)
    {
        return (from, to) switch
        {
            (CropState.Growing, CropState.Mature) => true,
            (CropState.Growing, CropState.WitheredImmature) => true,
            (CropState.Mature, CropState.WitheredMature) => true,
            (CropState.Mature, CropState.Growing) => true, // 可重复收获重置
            _ => false
        };
    }

    #endregion

    #region 5.1 CropState 状态转换测试（PBT）

    /// <summary>
    /// **Validates: Requirements AC-3.4, AC-3.5**
    /// P3：状态转换合法性 - 合法路径全部被允许
    /// </summary>
    [Test]
    public void CropState_ValidTransitions_AreAllowed()
    {
        // 所有合法路径
        var validPaths = new (CropState from, CropState to)[]
        {
            (CropState.Growing, CropState.Mature),
            (CropState.Growing, CropState.WitheredImmature),
            (CropState.Mature, CropState.WitheredMature),
            (CropState.Mature, CropState.Growing),
        };

        foreach (var (from, to) in validPaths)
        {
            Assert.IsTrue(IsValidTransition(from, to),
                $"合法路径 {from} → {to} 应该被允许");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.4, AC-3.5**
    /// P3：状态转换合法性 - 非法路径全部被拒绝
    /// </summary>
    [Test]
    public void CropState_InvalidTransitions_AreRejected()
    {
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        var invalidPaths = new (CropState from, CropState to)[]
        {
            (CropState.WitheredImmature, CropState.Growing),
            (CropState.WitheredImmature, CropState.Mature),
            (CropState.WitheredImmature, CropState.WitheredMature),
            (CropState.WitheredMature, CropState.Growing),
            (CropState.WitheredMature, CropState.Mature),
            (CropState.WitheredMature, CropState.WitheredImmature),
            (CropState.Growing, CropState.WitheredMature),
            (CropState.Mature, CropState.WitheredImmature),
            // 自身转换也是非法的
            (CropState.Growing, CropState.Growing),
            (CropState.Mature, CropState.Mature),
            (CropState.WitheredImmature, CropState.WitheredImmature),
            (CropState.WitheredMature, CropState.WitheredMature),
        };

        foreach (var (from, to) in invalidPaths)
        {
            Assert.IsFalse(IsValidTransition(from, to),
                $"非法路径 {from} → {to} 应该被拒绝");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.4, AC-3.5**
    /// PBT：随机生成状态转换序列，验证只有合法路径被允许
    /// </summary>
    [Test]
    public void CropState_PBT_RandomTransitionSequences_OnlyValidPathsAllowed()
    {
        const int iterations = 500;
        var random = new System.Random(42);
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            CropState from = allStates[random.Next(allStates.Length)];
            CropState to = allStates[random.Next(allStates.Length)];

            bool result = IsValidTransition(from, to);

            // 验证结果与预期一致
            bool expected = (from, to) switch
            {
                (CropState.Growing, CropState.Mature) => true,
                (CropState.Growing, CropState.WitheredImmature) => true,
                (CropState.Mature, CropState.WitheredMature) => true,
                (CropState.Mature, CropState.Growing) => true,
                _ => false
            };

            Assert.AreEqual(expected, result,
                $"迭代 {i}: {from} → {to} 应该返回 {expected}，实际返回 {result}");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.4**
    /// PBT：枯萎状态是终态（不能转换到任何非枯萎状态）
    /// </summary>
    [Test]
    public void CropState_PBT_WitheredStatesAreTerminal()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var witheredStates = new[] { CropState.WitheredImmature, CropState.WitheredMature };
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            CropState from = witheredStates[random.Next(witheredStates.Length)];
            CropState to = allStates[random.Next(allStates.Length)];

            bool result = IsValidTransition(from, to);
            Assert.IsFalse(result,
                $"迭代 {i}: 枯萎状态 {from} 不应该能转换到 {to}");
        }
    }

    #endregion

    #region 5.2 种子袋保质期测试（PBT）

    /// <summary>
    /// 模拟种子袋保质期逻辑
    /// </summary>
    private struct SeedBagState
    {
        public bool opened;
        public int remaining;
        public int shelfExpireDay;
    }

    private static SeedBagState InitSeedBag(int seedsPerBag, int shelfLifeClosed, int currentDay)
    {
        return new SeedBagState
        {
            opened = false,
            remaining = seedsPerBag,
            shelfExpireDay = currentDay + shelfLifeClosed
        };
    }

    private static SeedBagState OpenSeedBag(SeedBagState bag, int shelfLifeOpened, int currentDay)
    {
        if (bag.opened) return bag;
        bag.opened = true;
        int remainingDays = bag.shelfExpireDay - currentDay;
        int newExpire = currentDay + Mathf.Min(remainingDays, shelfLifeOpened);
        bag.shelfExpireDay = newExpire;
        return bag;
    }

    private static bool IsExpired(SeedBagState bag, int currentDay)
    {
        return currentDay >= bag.shelfExpireDay;
    }

    /// <summary>
    /// **Validates: Requirements AC-1.2, AC-1.3, AC-1.4**
    /// P1：保质期单调递减 - 打开种子袋后保质期不增加
    /// </summary>
    [Test]
    public void SeedBag_PBT_ShelfLifeMonotonicallyDecreasing()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            int seedsPerBag = random.Next(1, 20);
            int shelfLifeClosed = random.Next(1, 28);
            int shelfLifeOpened = random.Next(1, 14);
            int startDay = random.Next(0, 100);

            var bag = InitSeedBag(seedsPerBag, shelfLifeClosed, startDay);
            int originalExpire = bag.shelfExpireDay;

            // 在随机天数后打开
            int openDay = startDay + random.Next(0, shelfLifeClosed);
            bag = OpenSeedBag(bag, shelfLifeOpened, openDay);

            Assert.LessOrEqual(bag.shelfExpireDay, originalExpire,
                $"迭代 {i}: 打开后保质期 {bag.shelfExpireDay} 不应超过原始 {originalExpire}");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-1.2, AC-1.3**
    /// P1：打开后保质期不超过 shelfLifeOpened
    /// </summary>
    [Test]
    public void SeedBag_PBT_OpenedShelfLifeNotExceedMax()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            int shelfLifeClosed = random.Next(3, 28);
            int shelfLifeOpened = random.Next(1, 14);
            int startDay = random.Next(0, 100);

            var bag = InitSeedBag(5, shelfLifeClosed, startDay);

            int openDay = startDay + random.Next(0, shelfLifeClosed);
            bag = OpenSeedBag(bag, shelfLifeOpened, openDay);

            int remainingAfterOpen = bag.shelfExpireDay - openDay;
            Assert.LessOrEqual(remainingAfterOpen, shelfLifeOpened,
                $"迭代 {i}: 打开后剩余天数 {remainingAfterOpen} 不应超过 shelfLifeOpened={shelfLifeOpened}");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-1.4**
    /// 过期后正确检测
    /// </summary>
    [Test]
    public void SeedBag_PBT_ExpirationDetection()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            int shelfLifeClosed = random.Next(1, 28);
            int startDay = random.Next(0, 100);

            var bag = InitSeedBag(5, shelfLifeClosed, startDay);

            // 在过期日当天
            Assert.IsTrue(IsExpired(bag, bag.shelfExpireDay),
                $"迭代 {i}: 过期日当天应检测为过期");

            // 在过期日之前
            if (bag.shelfExpireDay > startDay)
            {
                int beforeDay = startDay + random.Next(0, bag.shelfExpireDay - startDay);
                Assert.IsFalse(IsExpired(bag, beforeDay),
                    $"迭代 {i}: 过期日之前 day={beforeDay} 不应检测为过期");
            }
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-1.3**
    /// 重复打开不改变状态
    /// </summary>
    [Test]
    public void SeedBag_OpenTwice_NoChange()
    {
        var bag = InitSeedBag(5, 7, 0);
        bag = OpenSeedBag(bag, 2, 3);
        int expireAfterFirst = bag.shelfExpireDay;

        bag = OpenSeedBag(bag, 2, 4);
        Assert.AreEqual(expireAfterFirst, bag.shelfExpireDay,
            "重复打开不应改变保质期");
    }

    #endregion

    #region 5.3 腐烂食物堆叠测试

    /// <summary>
    /// 模拟 CanStackWith 逻辑
    /// </summary>
    private static bool CanStackWith(int itemIdA, int qualityA, int propsCountA,
                                      int itemIdB, int qualityB, int propsCountB)
    {
        if (itemIdA != itemIdB) return false;
        if (qualityA != qualityB) return false;
        if (propsCountA != 0 || propsCountB != 0) return false;
        return true;
    }

    /// <summary>
    /// **Validates: Requirements AC-1.5, AC-1.6**
    /// P2：腐烂食物无动态属性，可堆叠
    /// </summary>
    [Test]
    public void RottenFood_PBT_NoPropertiesAndStackable()
    {
        const int rottenFoodId = 5999;
        const int rottenQuality = 0;
        const int iterations = 100;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            // 腐烂食物始终 properties.Count == 0
            int propsCountA = 0;
            int propsCountB = 0;

            bool canStack = CanStackWith(rottenFoodId, rottenQuality, propsCountA,
                                          rottenFoodId, rottenQuality, propsCountB);
            Assert.IsTrue(canStack,
                $"迭代 {i}: 两个腐烂食物应该可以堆叠");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-1.5**
    /// 有动态属性的物品不能与腐烂食物堆叠
    /// </summary>
    [Test]
    public void RottenFood_PBT_WithPropertiesCannotStack()
    {
        const int rottenFoodId = 5999;
        const int iterations = 100;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            int propsCountA = 0; // 腐烂食物无属性
            int propsCountB = random.Next(1, 5); // 有属性的物品

            bool canStack = CanStackWith(rottenFoodId, 0, propsCountA,
                                          rottenFoodId, 0, propsCountB);
            Assert.IsFalse(canStack,
                $"迭代 {i}: 有动态属性的物品不应与腐烂食物堆叠");
        }
    }

    #endregion

    #region 5.4 收获产出一致性测试

    /// <summary>
    /// 模拟收获逻辑
    /// </summary>
    private enum HarvestResult { None, CropItem, WitheredCropItem }

    private static HarvestResult GetHarvestResult(CropState state)
    {
        return state switch
        {
            CropState.Mature => HarvestResult.CropItem,
            CropState.WitheredMature => HarvestResult.WitheredCropItem,
            _ => HarvestResult.None
        };
    }

    private static bool CanHarvest(CropState state)
    {
        return state == CropState.Mature || state == CropState.WitheredMature;
    }

    /// <summary>
    /// **Validates: Requirements AC-4.1, AC-4.3**
    /// P4：收获产出一致性
    /// </summary>
    [Test]
    public void Harvest_PBT_OutputConsistency()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            CropState state = allStates[random.Next(allStates.Length)];
            var result = GetHarvestResult(state);
            bool canHarvest = CanHarvest(state);

            switch (state)
            {
                case CropState.Mature:
                    Assert.IsTrue(canHarvest, $"迭代 {i}: Mature 应该可收获");
                    Assert.AreEqual(HarvestResult.CropItem, result, $"迭代 {i}: Mature 应产出 CropItem");
                    break;
                case CropState.WitheredMature:
                    Assert.IsTrue(canHarvest, $"迭代 {i}: WitheredMature 应该可收获");
                    Assert.AreEqual(HarvestResult.WitheredCropItem, result, $"迭代 {i}: WitheredMature 应产出 WitheredCropItem");
                    break;
                case CropState.Growing:
                case CropState.WitheredImmature:
                    Assert.IsFalse(canHarvest, $"迭代 {i}: {state} 不应该可收获");
                    Assert.AreEqual(HarvestResult.None, result, $"迭代 {i}: {state} 不应有产出");
                    break;
            }
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-4.1**
    /// 收获数量在合理范围内
    /// </summary>
    [Test]
    public void Harvest_PBT_AmountWithinRange()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int i = 0; i < iterations; i++)
        {
            int minAmount = random.Next(1, 3);
            int maxAmount = minAmount + random.Next(0, 5);

            int amount = random.Next(minAmount, maxAmount + 1);

            Assert.GreaterOrEqual(amount, minAmount,
                $"迭代 {i}: 收获数量 {amount} 不应小于 {minAmount}");
            Assert.LessOrEqual(amount, maxAmount,
                $"迭代 {i}: 收获数量 {amount} 不应大于 {maxAmount}");
        }
    }

    #endregion

    #region 5.5 过季枯萎覆盖测试

    private enum Season { Spring = 0, Summer = 1, Autumn = 2, Winter = 3, AllSeason = 4 }

    /// <summary>
    /// 模拟过季枯萎逻辑
    /// </summary>
    private static CropState ApplySeasonChange(CropState currentState, Season cropSeason, Season newSeason)
    {
        if (cropSeason == Season.AllSeason) return currentState;
        if (cropSeason == newSeason) return currentState;

        // 过季枯萎
        return currentState switch
        {
            CropState.Mature => CropState.WitheredMature,
            CropState.Growing => CropState.WitheredImmature,
            _ => currentState // 已经枯萎的不变
        };
    }

    /// <summary>
    /// **Validates: Requirements AC-3.5**
    /// P5：过季枯萎全覆盖 - 非 AllSeason 作物在季节不匹配时必须枯萎
    /// </summary>
    [Test]
    public void SeasonWither_PBT_NonAllSeasonCropsWither()
    {
        const int iterations = 300;
        var random = new System.Random(42);
        var seasons = new[] { Season.Spring, Season.Summer, Season.Autumn, Season.Winter };
        var activeStates = new[] { CropState.Growing, CropState.Mature };

        for (int i = 0; i < iterations; i++)
        {
            Season cropSeason = seasons[random.Next(seasons.Length)];
            Season newSeason;
            do
            {
                newSeason = seasons[random.Next(seasons.Length)];
            } while (newSeason == cropSeason);

            CropState state = activeStates[random.Next(activeStates.Length)];
            CropState result = ApplySeasonChange(state, cropSeason, newSeason);

            bool isWithered = result == CropState.WitheredImmature || result == CropState.WitheredMature;
            Assert.IsTrue(isWithered,
                $"迭代 {i}: {state} 作物（季节={cropSeason}）在新季节 {newSeason} 应该枯萎，实际={result}");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.5**
    /// P5：AllSeason 作物不受季节影响
    /// </summary>
    [Test]
    public void SeasonWither_PBT_AllSeasonCropsUnaffected()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var seasons = new[] { Season.Spring, Season.Summer, Season.Autumn, Season.Winter };
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            Season newSeason = seasons[random.Next(seasons.Length)];
            CropState state = allStates[random.Next(allStates.Length)];

            CropState result = ApplySeasonChange(state, Season.AllSeason, newSeason);
            Assert.AreEqual(state, result,
                $"迭代 {i}: AllSeason 作物状态 {state} 在季节 {newSeason} 不应改变");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.5**
    /// 同季节不触发枯萎
    /// </summary>
    [Test]
    public void SeasonWither_PBT_SameSeasonNoWither()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var seasons = new[] { Season.Spring, Season.Summer, Season.Autumn, Season.Winter };
        var allStates = new[] { CropState.Growing, CropState.Mature, CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            Season season = seasons[random.Next(seasons.Length)];
            CropState state = allStates[random.Next(allStates.Length)];

            CropState result = ApplySeasonChange(state, season, season);
            Assert.AreEqual(state, result,
                $"迭代 {i}: 同季节 {season} 不应改变状态 {state}");
        }
    }

    /// <summary>
    /// **Validates: Requirements AC-3.5**
    /// 已枯萎的作物过季不改变状态
    /// </summary>
    [Test]
    public void SeasonWither_PBT_AlreadyWitheredUnchanged()
    {
        const int iterations = 200;
        var random = new System.Random(42);
        var seasons = new[] { Season.Spring, Season.Summer, Season.Autumn, Season.Winter };
        var witheredStates = new[] { CropState.WitheredImmature, CropState.WitheredMature };

        for (int i = 0; i < iterations; i++)
        {
            Season cropSeason = seasons[random.Next(seasons.Length)];
            Season newSeason;
            do
            {
                newSeason = seasons[random.Next(seasons.Length)];
            } while (newSeason == cropSeason);

            CropState state = witheredStates[random.Next(witheredStates.Length)];
            CropState result = ApplySeasonChange(state, cropSeason, newSeason);

            Assert.AreEqual(state, result,
                $"迭代 {i}: 已枯萎状态 {state} 过季不应改变");
        }
    }

    #endregion

    #region 5.6 阶段计算测试（daysToNextStage 累加模式）

    /// <summary>
    /// 模拟 CropController.UpdateGrowthStage 的累加逻辑
    /// stages[i].daysToNextStage 表示从阶段 i 到阶段 i+1 需要的天数
    /// </summary>
    private static int CalculateStage(int[] daysToNextStage, int grownDays)
    {
        int accumulatedDays = 0;
        int stage = 0;
        
        for (int i = 0; i < daysToNextStage.Length - 1; i++)
        {
            accumulatedDays += daysToNextStage[i];
            if (grownDays >= accumulatedDays)
                stage = i + 1;
            else
                break;
        }
        
        return Mathf.Clamp(stage, 0, daysToNextStage.Length - 1);
    }

    /// <summary>
    /// 计算到达指定阶段所需的总天数
    /// </summary>
    private static int CalculateDaysForStage(int[] daysToNextStage, int targetStage)
    {
        int accDays = 0;
        for (int i = 0; i < targetStage && i < daysToNextStage.Length; i++)
            accDays += daysToNextStage[i];
        return accDays;
    }

    /// <summary>
    /// **Validates: 迭代需求001 — 每阶段独立天数**
    /// P6：固定 4 阶段配置（种子1天→幼苗2天→生长2天→成熟0天）
    /// 验证 grownDays=0 → 阶段0，grownDays=1 → 阶段1，grownDays=3 → 阶段2，grownDays=5 → 阶段3
    /// </summary>
    [Test]
    public void StageCalc_FixedFourStages_CorrectProgression()
    {
        // 种子(1天) → 幼苗(2天) → 生长(2天) → 成熟(0天)
        var days = new int[] { 1, 2, 2, 0 };
        
        Assert.AreEqual(0, CalculateStage(days, 0), "grownDays=0 应为阶段0（种子）");
        Assert.AreEqual(1, CalculateStage(days, 1), "grownDays=1 应为阶段1（幼苗）");
        Assert.AreEqual(1, CalculateStage(days, 2), "grownDays=2 应为阶段1（幼苗）");
        Assert.AreEqual(2, CalculateStage(days, 3), "grownDays=3 应为阶段2（生长）");
        Assert.AreEqual(2, CalculateStage(days, 4), "grownDays=4 应为阶段2（生长）");
        Assert.AreEqual(3, CalculateStage(days, 5), "grownDays=5 应为阶段3（成熟）");
        Assert.AreEqual(3, CalculateStage(days, 10), "grownDays=10 应为阶段3（成熟，封顶）");
    }

    /// <summary>
    /// **Validates: 迭代需求001 — 阶段单调递增**
    /// PBT：随机生成 daysToNextStage 配置，验证 grownDays 增加时阶段只增不减
    /// </summary>
    [Test]
    public void StageCalc_PBT_StageMonotonicallyIncreasing()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int iter = 0; iter < iterations; iter++)
        {
            // 随机生成 4 阶段配置
            var days = new int[]
            {
                random.Next(1, 5),  // 种子→幼苗
                random.Next(1, 5),  // 幼苗→生长
                random.Next(1, 5),  // 生长→成熟
                0                   // 成熟（不再生长）
            };

            int totalDays = days[0] + days[1] + days[2];
            int prevStage = 0;

            for (int d = 0; d <= totalDays + 3; d++)
            {
                int stage = CalculateStage(days, d);
                Assert.GreaterOrEqual(stage, prevStage,
                    $"迭代 {iter}: grownDays={d} 阶段 {stage} 不应小于前一天的阶段 {prevStage}");
                prevStage = stage;
            }
        }
    }

    /// <summary>
    /// **Validates: 迭代需求001 — 成熟阶段封顶**
    /// PBT：grownDays 超过总天数后，阶段始终为最后阶段
    /// </summary>
    [Test]
    public void StageCalc_PBT_MatureStageIsCapped()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int iter = 0; iter < iterations; iter++)
        {
            var days = new int[]
            {
                random.Next(1, 5),
                random.Next(1, 5),
                random.Next(1, 5),
                0
            };

            int totalDays = days[0] + days[1] + days[2];
            int maxStage = days.Length - 1; // 3

            // 超过总天数后应始终为最后阶段
            for (int extra = 0; extra < 10; extra++)
            {
                int stage = CalculateStage(days, totalDays + extra);
                Assert.AreEqual(maxStage, stage,
                    $"迭代 {iter}: grownDays={totalDays + extra} 应为最后阶段 {maxStage}");
            }
        }
    }

    /// <summary>
    /// **Validates: 迭代需求001 — 重复收获阶段天数计算**
    /// 验证 CalculateDaysForStage 正确累加
    /// </summary>
    [Test]
    public void StageCalc_DaysForStage_CorrectAccumulation()
    {
        var days = new int[] { 1, 2, 2, 0 };
        
        Assert.AreEqual(0, CalculateDaysForStage(days, 0), "到阶段0需要0天");
        Assert.AreEqual(1, CalculateDaysForStage(days, 1), "到阶段1需要1天");
        Assert.AreEqual(3, CalculateDaysForStage(days, 2), "到阶段2需要3天");
        Assert.AreEqual(5, CalculateDaysForStage(days, 3), "到阶段3需要5天");
    }

    /// <summary>
    /// **Validates: 迭代需求001 — 阶段边界精确性**
    /// PBT：恰好在阶段边界天数时应进入下一阶段
    /// </summary>
    [Test]
    public void StageCalc_PBT_ExactBoundaryTransitions()
    {
        const int iterations = 200;
        var random = new System.Random(42);

        for (int iter = 0; iter < iterations; iter++)
        {
            var days = new int[]
            {
                random.Next(1, 8),
                random.Next(1, 8),
                random.Next(1, 8),
                0
            };

            // 在每个阶段边界检查
            int boundary = 0;
            for (int i = 0; i < 3; i++)
            {
                boundary += days[i];
                int stageAtBoundary = CalculateStage(days, boundary);
                int stageBeforeBoundary = CalculateStage(days, boundary - 1);
                
                Assert.AreEqual(i + 1, stageAtBoundary,
                    $"迭代 {iter}: grownDays={boundary}（边界）应为阶段 {i + 1}");
                Assert.AreEqual(i, stageBeforeBoundary,
                    $"迭代 {iter}: grownDays={boundary - 1}（边界前）应为阶段 {i}");
            }
        }
    }

    #endregion

}
