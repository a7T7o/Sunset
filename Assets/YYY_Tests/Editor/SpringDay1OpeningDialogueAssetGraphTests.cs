using NUnit.Framework;
using UnityEditor;
using Sunset.Story;

[TestFixture]
public class SpringDay1OpeningDialogueAssetGraphTests
{
    private const string FirstDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset";
    private const string FollowupDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset";
    private const string VillageGateDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset";
    private const string HouseArrivalDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset";

    [Test]
    public void OpeningDialogueAssets_FormExpectedFollowupGraph()
    {
        DialogueSequenceSO first = LoadAsset(FirstDialoguePath);
        DialogueSequenceSO followup = LoadAsset(FollowupDialoguePath);
        DialogueSequenceSO villageGate = LoadAsset(VillageGateDialoguePath);
        DialogueSequenceSO houseArrival = LoadAsset(HouseArrivalDialoguePath);

        Assert.Multiple(() =>
        {
            Assert.That(first.followupSequence, Is.SameAs(followup), "矿洞口首段应接到撤离段。");
            Assert.That(followup.followupSequence, Is.SameAs(villageGate), "撤离段应自动续到进村围观段。");
            Assert.That(villageGate.followupSequence, Is.SameAs(houseArrival), "进村围观段应自动续到闲置小屋安置段。");
            Assert.That(houseArrival.followupSequence, Is.Null, "闲置小屋安置段应作为进入疗伤前的最后一拍。");
        });
    }

    [Test]
    public void OpeningDialogueAssets_PreserveOpeningSemantics()
    {
        DialogueSequenceSO first = LoadAsset(FirstDialoguePath);
        DialogueSequenceSO followup = LoadAsset(FollowupDialoguePath);
        DialogueSequenceSO villageGate = LoadAsset(VillageGateDialoguePath);
        DialogueSequenceSO houseArrival = LoadAsset(HouseArrivalDialoguePath);

        Assert.Multiple(() =>
        {
            Assert.That(first.markLanguageDecodedOnComplete, Is.True, "矿洞口首段完成后应正式解码语言。");
            Assert.That(first.advanceStoryPhaseOnComplete, Is.False, "矿洞口首段不应直接推进到 EnterVillage。");
            Assert.That(followup.advanceStoryPhaseOnComplete, Is.True, "撤离段收束后应推进到 EnterVillage。");
            Assert.That(followup.nextStoryPhase, Is.EqualTo(StoryPhase.EnterVillage), "撤离段应把外层阶段推进到 EnterVillage。");
            Assert.That(ContainsText(followup, "矿洞口不安全"), Is.True, "撤离段应保留矿洞危险提醒。");
            Assert.That(ContainsText(followup, "跟紧我"), Is.True, "撤离段应明确由村长带路离开。");
            Assert.That(ContainsSpeaker(villageGate, "村民"), Is.True, "进村围观段应保留村民视线。");
            Assert.That(ContainsSpeaker(villageGate, "小孩"), Is.True, "进村围观段应保留小孩视线。");
            Assert.That(ContainsText(houseArrival, "不是谁家的房间"), Is.True, "安置段应明确这不是谁家的房间。");
            Assert.That(ContainsText(houseArrival, "艾拉"), Is.True, "安置段应明确艾拉会来接手疗伤。");
        });
    }

    private static DialogueSequenceSO LoadAsset(string assetPath)
    {
        DialogueSequenceSO asset = AssetDatabase.LoadAssetAtPath<DialogueSequenceSO>(assetPath);
        Assert.That(asset, Is.Not.Null, $"缺少对白资产：{assetPath}");
        return asset;
    }

    private static bool ContainsText(DialogueSequenceSO sequence, string fragment)
    {
        return sequence.nodes != null
            && sequence.nodes.Exists(node => !string.IsNullOrWhiteSpace(node.text) && node.text.Contains(fragment));
    }

    private static bool ContainsSpeaker(DialogueSequenceSO sequence, string speakerName)
    {
        return sequence.nodes != null
            && sequence.nodes.Exists(node => !string.IsNullOrWhiteSpace(node.speakerName) && node.speakerName.Contains(speakerName));
    }
}
