using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

[TestFixture]
public class NpcCrowdDialogueNormalizationTests
{
    private static readonly string ProjectRoot = Directory.GetCurrentDirectory();
    private static readonly string CrowdRoot = Path.Combine(ProjectRoot, "Assets/111_Data/NPC/SpringDay1Crowd");
    private static readonly string[] DialogueAssetPaths =
    {
        Path.Combine(CrowdRoot, "NPC_101_LedgerScribeDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_102_HunterDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_103_ErrandBoyDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_104_CarpenterDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_201_SeamstressDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_202_FloristDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_203_CanteenKeeperDialogueContent.asset"),
        Path.Combine(CrowdRoot, "NPC_301_GraveWardenBoneDialogueContent.asset"),
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
    public void GraveWardenDialogueAsset_ShouldStayOnGenericNightWatchTone()
    {
        string assetPath = Path.Combine(CrowdRoot, "NPC_301_GraveWardenBoneDialogueContent.asset");
        string assetText = ReadDialogueFacingText(assetPath);

        Assert.That(assetText, Does.Not.Contain("骨头不会说谎"));
        Assert.That(assetText, Does.Not.Contain("小骨头"));
        Assert.That(assetText, Does.Contain("夜路"));
        Assert.That(assetText, Does.Contain("后坡"));
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
}
