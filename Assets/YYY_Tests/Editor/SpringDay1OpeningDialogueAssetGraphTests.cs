using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class SpringDay1OpeningDialogueAssetGraphTests
{
    private const string FirstDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset";
    private const string FollowupDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset";
    private const string VillageGateDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset";
    private const string HouseArrivalDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset";

    private static readonly Type DialogueSequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");

    [Test]
    public void OpeningDialogueAssets_FormExpectedFollowupGraph()
    {
        UnityEngine.Object first = LoadAsset(FirstDialoguePath);
        UnityEngine.Object followup = LoadAsset(FollowupDialoguePath);
        UnityEngine.Object villageGate = LoadAsset(VillageGateDialoguePath);
        UnityEngine.Object houseArrival = LoadAsset(HouseArrivalDialoguePath);

        Assert.That(GetMemberValue<UnityEngine.Object>(first, "followupSequence"), Is.SameAs(followup), "矿洞口首段应接到撤离段。");
        Assert.That(GetMemberValue<UnityEngine.Object>(followup, "followupSequence"), Is.SameAs(villageGate), "撤离段仍应能桥到进村围观段。");
        Assert.That(GetMemberValue<UnityEngine.Object>(villageGate, "followupSequence"), Is.Null, "进村围观段不应再在同场对白里自动续到 Primary 承接；承接应交回 Director 在切场后接管。");
        Assert.That(GetMemberValue<UnityEngine.Object>(houseArrival, "followupSequence"), Is.Null, "Primary 承接段应作为进入疗伤前的最后一拍。");
    }

    [Test]
    public void OpeningDialogueAssets_PreserveOpeningSemantics()
    {
        UnityEngine.Object first = LoadAsset(FirstDialoguePath);
        UnityEngine.Object followup = LoadAsset(FollowupDialoguePath);
        UnityEngine.Object villageGate = LoadAsset(VillageGateDialoguePath);
        UnityEngine.Object houseArrival = LoadAsset(HouseArrivalDialoguePath);

        Assert.That(GetMemberValue<bool>(first, "markLanguageDecodedOnComplete"), Is.True, "矿洞口首段完成后应正式解码语言。");
        Assert.That(GetMemberValue<bool>(first, "advanceStoryPhaseOnComplete"), Is.False, "矿洞口首段不应直接推进到 EnterVillage。");
        Assert.That(GetMemberValue<bool>(followup, "advanceStoryPhaseOnComplete"), Is.True, "撤离段收束后应推进到 EnterVillage。");
        Assert.That(GetMemberValue<object>(followup, "nextStoryPhase")?.ToString(), Is.EqualTo("EnterVillage"), "撤离段应把外层阶段推进到 EnterVillage。");
        Assert.That(ContainsText(followup, "矿洞口不安全"), Is.True, "撤离段应保留矿洞危险提醒。");
        Assert.That(ContainsText(followup, "跟紧我"), Is.True, "撤离段应明确由村长带路离开。");
        Assert.That(ContainsText(followup, "骷髅"), Is.True, "撤离段应明确矿洞口逼近的怪物威胁。");
        Assert.That(ContainsText(followup, "别回头"), Is.True, "撤离段应明确这是一次被迫撤离，不是站着聊天。");
        Assert.That(ContainsSpeaker(villageGate, "村民"), Is.True, "进村围观段应保留村民视线。");
        Assert.That(ContainsSpeaker(villageGate, "小孩"), Is.True, "进村围观段应保留小孩视线。");
        Assert.That(ContainsText(villageGate, "谁有话"), Is.True, "进村围观段应保留村长压住围观场面的语义。");
        Assert.That(ContainsText(houseArrival, "先在这边站稳"), Is.True, "Primary 承接段应先把玩家重新站稳。");
        Assert.That(ContainsText(houseArrival, "艾拉"), Is.True, "Primary 承接段应明确艾拉会来接手疗伤。");
        Assert.That(ContainsText(houseArrival, "还没把我接成自己人"), Is.True, "Primary 承接段应保留“暂时接住但未完全接纳”的语义。");
    }

    private static UnityEngine.Object LoadAsset(string assetPath)
    {
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, DialogueSequenceType);
        Assert.That(asset, Is.Not.Null, $"缺少对白资产：{assetPath}");
        return asset;
    }

    private static bool ContainsText(UnityEngine.Object sequence, string fragment)
    {
        IList nodes = GetMemberValue<IList>(sequence, "nodes");
        if (nodes == null)
        {
            return false;
        }

        for (int index = 0; index < nodes.Count; index++)
        {
            string text = GetMemberValue<string>(nodes[index], "text");
            if (!string.IsNullOrWhiteSpace(text) && text.Contains(fragment))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsSpeaker(UnityEngine.Object sequence, string speakerName)
    {
        IList nodes = GetMemberValue<IList>(sequence, "nodes");
        if (nodes == null)
        {
            return false;
        }

        for (int index = 0; index < nodes.Count; index++)
        {
            string currentSpeaker = GetMemberValue<string>(nodes[index], "speakerName");
            if (!string.IsNullOrWhiteSpace(currentSpeaker) && currentSpeaker.Contains(speakerName))
            {
                return true;
            }
        }

        return false;
    }

    private static T GetMemberValue<T>(object target, string memberName)
    {
        Assert.That(target, Is.Not.Null, $"读取成员 {memberName} 时目标为空。");

        Type targetType = target.GetType();
        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        FieldInfo field = targetType.GetField(memberName, Flags);
        if (field != null)
        {
            return (T)field.GetValue(target);
        }

        PropertyInfo property = targetType.GetProperty(memberName, Flags);
        if (property != null)
        {
            return (T)property.GetValue(target);
        }

        Assert.Fail($"类型 {targetType.FullName} 缺少成员：{memberName}");
        return default;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int index = 0; index < assemblies.Length; index++)
        {
            Type resolved = assemblies[index].GetType(fullName, throwOnError: false);
            if (resolved != null)
            {
                return resolved;
            }
        }

        Assert.Fail($"缺少类型：{fullName}");
        return null;
    }
}
