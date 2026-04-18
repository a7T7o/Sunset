using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

[TestFixture]
public class NpcCrowdDialogueNormalizationTests
{
    private static readonly string ProjectRoot = Directory.GetCurrentDirectory();
    private static readonly string CrowdRoot = Path.Combine(ProjectRoot, "Assets/111_Data/NPC/SpringDay1Crowd");
    private static readonly string BootstrapSourcePath = Path.Combine(ProjectRoot, "Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs");
    private static readonly string[] DialogueAssetPaths =
    {
        Path.Combine(CrowdRoot, "NPC_101_LedgerScribeDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_102_HunterDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_103_ErrandBoyDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_104_CarpenterDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_201_SeamstressDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_202_FloristDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_203_CanteenKeeperDialogueContent.asset"),
    };

    private static readonly SemanticCoverageExpectation[] LateSceneSemanticCoverage =
    {
        Coverage(
            "NPC_101_LedgerScribeDialogueContent.asset",
            new[] { "停手" },
            new[] { "目光", "眼睛" },
            new[] { "照常" }),
        Coverage(
            "NPC_102_HunterDialogueContent.asset",
            new[] { "后坡" },
            new[] { "脚步" },
            new[] { "照常" }),
        Coverage(
            "NPC_103_ErrandBoyDialogueContent.asset",
            new[] { "偷看", "踮脚" },
            new[] { "压低嗓子", "慢了半拍" },
            new[] { "明早", "照样站成一排" }),
        Coverage(
            "NPC_104_CarpenterDialogueContent.asset",
            new[] { "回屋" },
            new[] { "门栓" },
            new[] { "照旧" }),
        Coverage(
            "NPC_201_SeamstressDialogueContent.asset",
            new[] { "回屋" },
            new[] { "压得太低", "压低" },
            new[] { "照常" }),
        Coverage(
            "NPC_202_FloristDialogueContent.asset",
            new[] { "安神草" },
            new[] { "回屋" },
            new[] { "照常" }),
        Coverage(
            "NPC_203_CanteenKeeperDialogueContent.asset",
            new[] { "热汤", "开火" },
            new[] { "低声议论", "桌子先静" },
            new[] { "回屋" }),
    };

    private static readonly SemanticCoverageExpectation[] PhaseFallbackBundleCoverage =
    {
        Coverage("NPC_101_LedgerScribeDialogueContent.asset", new[] { "101-enter-village-resident" }, new[] { "101-dinner-resident" }, new[] { "101-day-end-resident" }),
        Coverage("NPC_102_HunterDialogueContent.asset", new[] { "102-return-reminder-resident" }, new[] { "102-free-time-resident" }, new[] { "102-day-end-resident" }),
        Coverage("NPC_103_ErrandBoyDialogueContent.asset", new[] { "103-enter-village-resident" }, new[] { "103-dinner-resident" }, new[] { "103-day-end-resident" }),
        Coverage("NPC_104_CarpenterDialogueContent.asset", new[] { "104-workbench-resident" }, new[] { "104-dinner-resident" }, new[] { "104-day-end-resident" }),
        Coverage("NPC_201_SeamstressDialogueContent.asset", new[] { "201-healing-resident" }, new[] { "201-dinner-resident" }, new[] { "201-day-end-resident" }),
        Coverage("NPC_202_FloristDialogueContent.asset", new[] { "202-farming-resident" }, new[] { "202-return-reminder-resident" }, new[] { "202-day-end-resident" }),
        Coverage("NPC_203_CanteenKeeperDialogueContent.asset", new[] { "203-dinner-resident" }, new[] { "203-return-reminder-resident" }, new[] { "203-day-end-resident" }),
    };

    private static readonly SemanticCoverageExpectation[] PhaseNearbyCoverage =
    {
        Coverage("NPC_101_LedgerScribeDialogueContent.asset", new[] { "拿余光追着你", "摊边那阵停顿" }, new[] { "桌边的话更轻", "动作反而更容易慢半拍" }, new[] { "纸边还压着今晚", "今晚这口气还在村口打转" }),
        Coverage("NPC_102_HunterDialogueContent.asset", new[] { "回屋这一路的脚步最会露底", "碎石踩乱" }, new[] { "夜里后坡最诚实", "把自己的步子收轻" }, new[] { "谁没睡安稳其实瞒不过地面", "昨晚那点慌不会这么快全埋住" }),
        Coverage("NPC_103_ErrandBoyDialogueContent.asset", new[] { "假装低头偷偷看你", "大人反而更爱装作" }, new[] { "谁把嗓子压得太狠", "眼神还在拐着弯往这边跑" }, new[] { "偷听的劲儿还没真散掉", "今晚谁最爱偷看我记得清" }),
        Coverage("NPC_104_CarpenterDialogueContent.asset", new[] { "工作台这边一响", "人心更会响" }, new[] { "敲木声都压轻了", "从木头回声里就听得见" }, new[] { "先把门闩顶稳", "照旧开门干活" }),
        Coverage("NPC_201_SeamstressDialogueContent.asset", new[] { "坐下都先捏袖口", "别把气憋着带回屋" }, new[] { "连来找针线的人都把脚步放轻了", "先把人稳住" }, new[] { "谁都装作自己没事", "还有人来补口子" }),
        Coverage("NPC_202_FloristDialogueContent.asset", new[] { "带一束安神草", "留点颜色把人拉回来" }, new[] { "门边这点香", "眼里只剩灰色" }, new[] { "花还是照常开", "颜色全抽干" }),
        Coverage("NPC_203_CanteenKeeperDialogueContent.asset", new[] { "别空着肚子回屋", "这点热气把心撑住" }, new[] { "垫两口更稳", "锅边这点热气也跟着灭了" }, new[] { "照旧开火", "先把人喂热" }),
    };

    private static readonly SemanticCoverageExpectation[] PhaseSelfTalkCoverage =
    {
        Coverage("NPC_101_LedgerScribeDialogueContent.asset", new[] { "围观的眼神", "纸边先留" }, new[] { "桌边越装作照常", "压在页边" }, new[] { "今晚这页收好", "记到天亮" }),
        Coverage("NPC_102_HunterDialogueContent.asset", new[] { "脚印踩乱", "碎石声里记下来" }, new[] { "后坡这会儿最诚实", "呼吸压轻" }, new[] { "再看一圈地面", "昨晚那一下犹豫" }),
        Coverage("NPC_103_ErrandBoyDialogueContent.asset", new[] { "装作没看见他们偷看", "眼神反而更会拐弯" }, new[] { "静得太刻意了", "眼睛倒还在往这边跑" }, new[] { "谁偷听最久", "明天准有人装作没事" }),
        Coverage("NPC_104_CarpenterDialogueContent.asset", new[] { "门栓和木缝", "工作台这边一响" }, new[] { "木头敲轻一点", "跟着人心一起松" }, new[] { "门闩先顶住", "照旧开门干活" }),
        Coverage("NPC_201_SeamstressDialogueContent.asset", new[] { "针脚压稳", "憋着不肯先松气" }, new[] { "针线旁边多留一会儿", "来补口子的人反而更多了" }, new[] { "线头理顺", "针脚才有地方落稳" }),
        Coverage("NPC_202_FloristDialogueContent.asset", new[] { "留一点颜色", "安神草不吵" }, new[] { "回屋前先闻一闻", "眼里只剩灰" }, new[] { "花先别收", "明早照常摆出来" }),
        Coverage("NPC_203_CanteenKeeperDialogueContent.asset", new[] { "锅边这点热气不能断", "端稳碗" }, new[] { "回屋前再垫两口", "人气不能一下散光" }, new[] { "照旧开火", "最后回屋的人喂热" }),
    };

    private static readonly SemanticCoverageExpectation[] PhaseWalkAwayReactionCoverage =
    {
        Coverage("NPC_101_LedgerScribeDialogueContent.asset", new[] { "101-enter-village-walkaway" }, new[] { "101-dinner-walkaway" }, new[] { "101-day-end-walkaway" }),
        Coverage("NPC_102_HunterDialogueContent.asset", new[] { "102-return-reminder-walkaway" }, new[] { "102-free-time-walkaway" }, new[] { "102-day-end-walkaway" }),
        Coverage("NPC_103_ErrandBoyDialogueContent.asset", new[] { "103-enter-village-walkaway" }, new[] { "103-dinner-walkaway" }, new[] { "103-day-end-walkaway" }),
        Coverage("NPC_104_CarpenterDialogueContent.asset", new[] { "104-workbench-walkaway" }, new[] { "104-dinner-walkaway" }, new[] { "104-day-end-walkaway" }),
        Coverage("NPC_201_SeamstressDialogueContent.asset", new[] { "201-healing-walkaway" }, new[] { "201-dinner-walkaway" }, new[] { "201-day-end-walkaway" }),
        Coverage("NPC_202_FloristDialogueContent.asset", new[] { "202-farming-walkaway" }, new[] { "202-return-reminder-walkaway" }, new[] { "202-day-end-walkaway" }),
        Coverage("NPC_203_CanteenKeeperDialogueContent.asset", new[] { "203-dinner-walkaway" }, new[] { "203-return-reminder-walkaway" }, new[] { "203-day-end-walkaway" }),
    };

    private static readonly string[] BannedAuthoredNameTokens =
    {
        "莱札",
        "炎栎",
        "阿澈",
        "沈斧",
        "白槿",
        "桃羽",
        "麦禾",
        "朽钟",
        "小骨头",
    };

    private static readonly string[] BannedCrowdDirectivePhrases =
    {
        "我这儿能给你一半答案",
        "帮忙送几页抄件",
        "回来拿你那一页答案",
        "第一时间告诉我",
        "不是一个人跑的",
        "替我留意有人藏船",
        "记得看那棵歪脖树旁边",
        "有空就替我搬两块板",
        "先看门栓卡没卡紧",
        "今晚早点歇",
        "先叫人吃饱，再骂那帮装死的",
        "像老早就排练过",
        "总得有人把第一眼看到的样子留住",
        "村里现在缺的是稳手，不是更大的嗓门",
        "先把针脚落稳，心才不会跟着散",
        "花不会替人解决事，但它能让人记得自己还活着",
        "我只是不想让大家一回头，看到的全是逃跑留下的空白",
        "我不替你们抓人，只替夜里记回声",
        "更像陷阱",
        "别让夜色替你做决定",
    };

    [Test]
    public void CrowdDialogueAssets_ShouldNotContainAuthoredNpcNamesInRuntimeLines()
    {
        for (int index = 0; index < DialogueAssetPaths.Length; index++)
        {
            string assetPath = DialogueAssetPaths[index];
            string assetText = ReadDialogueFacingText(assetPath);

            for (int tokenIndex = 0; tokenIndex < BannedAuthoredNameTokens.Length; tokenIndex++)
            {
                string token = BannedAuthoredNameTokens[tokenIndex];
                Assert.That(
                    assetText,
                    Does.Not.Contain(token),
                    $"{Path.GetFileName(assetPath)} 仍保留现编名字或过度具象化 token: {token}");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldNotContainStaleDirectiveHooks()
    {
        for (int index = 0; index < DialogueAssetPaths.Length; index++)
        {
            string assetPath = DialogueAssetPaths[index];
            string assetText = ReadDialogueFacingText(assetPath);

            for (int tokenIndex = 0; tokenIndex < BannedCrowdDirectivePhrases.Length; tokenIndex++)
            {
                string token = BannedCrowdDirectivePhrases[tokenIndex];
                Assert.That(
                    assetText,
                    Does.Not.Contain(token),
                    $"{Path.GetFileName(assetPath)} 仍保留过重的任务/指挥口气：{token}");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldProvideCrowdSpecificInterruptAndResumeRules()
    {
        for (int index = 0; index < DialogueAssetPaths.Length; index++)
        {
            string assetPath = DialogueAssetPaths[index];
            string assetText = ReadDialogueFacingText(assetPath);

            Assert.That(
                assetText,
                Does.Not.Contain("defaultInterruptRules: []"),
                $"{Path.GetFileName(assetPath)} 仍然没有默认 interrupt rules。");
            Assert.That(
                assetText,
                Does.Not.Contain("defaultResumeRules: []"),
                $"{Path.GetFileName(assetPath)} 仍然没有默认 resume rules。");
            Assert.That(
                assetText,
                Does.Contain("resumeIntro:"),
                $"{Path.GetFileName(assetPath)} 缺少 resumeIntro 序列化内容。");
        }
    }

    [Test]
    public void BootstrapSource_ShouldNotReintroduceNamedPairCallsOrStaleDirectiveHooks()
    {
        Assert.That(File.Exists(BootstrapSourcePath), Is.True, $"缺少 bootstrap 源文件：{BootstrapSourcePath}");
        string sourceText = File.ReadAllText(BootstrapSourcePath);

        for (int index = 0; index < BannedAuthoredNameTokens.Length; index++)
        {
            string token = BannedAuthoredNameTokens[index];
            Assert.That(
                sourceText,
                Does.Not.Contain(token),
                $"bootstrap 源头仍保留现编名字 token：{token}");
        }

        for (int index = 0; index < BannedCrowdDirectivePhrases.Length; index++)
        {
            string token = BannedCrowdDirectivePhrases[index];
            Assert.That(
                sourceText,
                Does.Not.Contain(token),
                $"bootstrap 源头仍保留过重 crowd 口气：{token}");
        }

        Assert.That(
            sourceText,
            Does.Not.Contain("defaultInterruptRules = Array.Empty<InterruptRulePayload>()"),
            "bootstrap 源头仍把 crowd interrupt rules 留空。");
        Assert.That(
            sourceText,
            Does.Not.Contain("defaultResumeRules = Array.Empty<ResumeRulePayload>()"),
            "bootstrap 源头仍把 crowd resume rules 留空。");
        Assert.That(
            sourceText,
            Does.Contain("Refresh Crowd Dialogue Assets"),
            "bootstrap 源头缺少只刷新 crowd dialogue 的菜单入口。");
    }

    [Test]
    public void CrowdDialogueAssets_ShouldCarryLateSceneSemanticCoverage()
    {
        for (int index = 0; index < LateSceneSemanticCoverage.Length; index++)
        {
            SemanticCoverageExpectation expectation = LateSceneSemanticCoverage[index];
            string assetPath = Path.Combine(CrowdRoot, expectation.FileName);
            string assetText = ReadDialogueFacingText(assetPath);

            for (int groupIndex = 0; groupIndex < expectation.RequiredTokenGroups.Length; groupIndex++)
            {
                AssertContainsAny(
                    assetText,
                    expectation.RequiredTokenGroups[groupIndex],
                    $"{expectation.FileName} 缺少 Day1 后半段群像语义支撑。");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldContainPhaseAwareResidentFallbackBundles()
    {
        for (int index = 0; index < PhaseFallbackBundleCoverage.Length; index++)
        {
            SemanticCoverageExpectation expectation = PhaseFallbackBundleCoverage[index];
            string assetPath = Path.Combine(CrowdRoot, expectation.FileName);
            string assetText = ReadDialogueFacingText(assetPath);

            Assert.That(
                assetText,
                Does.Contain("phaseInformalChatSets:"),
                $"{expectation.FileName} 缺少 phase-aware resident fallback 序列化段。");

            for (int groupIndex = 0; groupIndex < expectation.RequiredTokenGroups.Length; groupIndex++)
            {
                AssertContainsAny(
                    assetText,
                    expectation.RequiredTokenGroups[groupIndex],
                    $"{expectation.FileName} 缺少对应 phase 的 resident fallback bundle。");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldContainPhaseAwareResidentNearbyLines()
    {
        for (int index = 0; index < PhaseNearbyCoverage.Length; index++)
        {
            SemanticCoverageExpectation expectation = PhaseNearbyCoverage[index];
            string assetPath = Path.Combine(CrowdRoot, expectation.FileName);
            string assetText = ReadDialogueFacingText(assetPath);

            Assert.That(
                assetText,
                Does.Contain("phaseNearbyLines:"),
                $"{expectation.FileName} 缺少 phase-aware resident nearby 序列化段。");

            for (int groupIndex = 0; groupIndex < expectation.RequiredTokenGroups.Length; groupIndex++)
            {
                AssertContainsAny(
                    assetText,
                    expectation.RequiredTokenGroups[groupIndex],
                    $"{expectation.FileName} 缺少对应 phase 的 resident nearby lines。");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldContainPhaseAwareResidentSelfTalkLines()
    {
        for (int index = 0; index < PhaseSelfTalkCoverage.Length; index++)
        {
            SemanticCoverageExpectation expectation = PhaseSelfTalkCoverage[index];
            string assetPath = Path.Combine(CrowdRoot, expectation.FileName);
            string assetText = ReadDialogueFacingText(assetPath);

            Assert.That(
                assetText,
                Does.Contain("phaseSelfTalkLines:"),
                $"{expectation.FileName} 缺少 phase-aware resident selfTalk 序列化段。");

            for (int groupIndex = 0; groupIndex < expectation.RequiredTokenGroups.Length; groupIndex++)
            {
                AssertContainsAny(
                    assetText,
                    expectation.RequiredTokenGroups[groupIndex],
                    $"{expectation.FileName} 缺少对应 phase 的 resident selfTalk lines。");
            }
        }
    }

    [Test]
    public void CrowdDialogueAssets_ShouldContainPhaseAwareWalkAwayReactions()
    {
        for (int index = 0; index < PhaseWalkAwayReactionCoverage.Length; index++)
        {
            SemanticCoverageExpectation expectation = PhaseWalkAwayReactionCoverage[index];
            string assetPath = Path.Combine(CrowdRoot, expectation.FileName);
            string assetText = ReadDialogueFacingText(assetPath);

            Assert.That(
                assetText,
                Does.Contain("walkAwayReaction:"),
                $"{expectation.FileName} 缺少 phase-aware resident walkAwayReaction 序列化段。");

            for (int groupIndex = 0; groupIndex < expectation.RequiredTokenGroups.Length; groupIndex++)
            {
                AssertContainsAny(
                    assetText,
                    expectation.RequiredTokenGroups[groupIndex],
                    $"{expectation.FileName} 缺少对应 phase 的 resident walkAwayReaction cue。");
            }
        }
    }

    private static string ReadDialogueFacingText(string assetPath)
    {
        Assert.That(File.Exists(assetPath), Is.True, $"缺少 crowd 对话资产：{assetPath}");

        string[] lines = File.ReadAllLines(assetPath);
        using StringWriter writer = new StringWriter();
        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            if (line.StartsWith("  m_Name:"))
            {
                continue;
            }

            writer.WriteLine(Regex.Unescape(line));
        }

        return writer.ToString();
    }

    private static void AssertContainsAny(string assetText, string[] candidateTokens, string message)
    {
        for (int index = 0; index < candidateTokens.Length; index++)
        {
            if (assetText.Contains(candidateTokens[index]))
            {
                return;
            }
        }

        Assert.Fail($"{message} 候选 token: {string.Join(" / ", candidateTokens)}");
    }

    private static SemanticCoverageExpectation Coverage(string fileName, params string[][] requiredTokenGroups)
    {
        return new SemanticCoverageExpectation(fileName, requiredTokenGroups);
    }

    private sealed class SemanticCoverageExpectation
    {
        public SemanticCoverageExpectation(string fileName, string[][] requiredTokenGroups)
        {
            FileName = fileName;
            RequiredTokenGroups = requiredTokenGroups;
        }

        public string FileName { get; }
        public string[][] RequiredTokenGroups { get; }
    }
}
