using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class SpringDay1MiddayDialogueAssetGraphTests
{
    private const string HealingDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_Healing.asset";
    private const string WorkbenchDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_WorkbenchRecall.asset";
    private const string DinnerDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_DinnerConflict.asset";
    private const string ReminderDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_ReturnReminder.asset";
    private const string FreeTimeDialoguePath = "Assets/111_Data/Story/Dialogue/SpringDay1_FreeTimeOpening.asset";

    private static readonly Type DialogueSequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");

    [Test]
    public void MiddayDialogueAssets_ShouldExistWithExpectedSequenceIds()
    {
        UnityEngine.Object healing = LoadAsset(HealingDialoguePath);
        UnityEngine.Object workbench = LoadAsset(WorkbenchDialoguePath);
        UnityEngine.Object dinner = LoadAsset(DinnerDialoguePath);
        UnityEngine.Object reminder = LoadAsset(ReminderDialoguePath);
        UnityEngine.Object freeTime = LoadAsset(FreeTimeDialoguePath);

        Assert.That(GetMemberValue<string>(healing, "sequenceId"), Is.EqualTo("spring-day1-healing"));
        Assert.That(GetMemberValue<string>(workbench, "sequenceId"), Is.EqualTo("spring-day1-workbench"));
        Assert.That(GetMemberValue<string>(dinner, "sequenceId"), Is.EqualTo("spring-day1-dinner"));
        Assert.That(GetMemberValue<string>(reminder, "sequenceId"), Is.EqualTo("spring-day1-reminder"));
        Assert.That(GetMemberValue<string>(freeTime, "sequenceId"), Is.EqualTo("spring-day1-free-time-opening"));
    }

    [Test]
    public void MiddayDialogueAssets_ShouldPreserveLaterDaySemantics()
    {
        UnityEngine.Object healing = LoadAsset(HealingDialoguePath);
        UnityEngine.Object workbench = LoadAsset(WorkbenchDialoguePath);
        UnityEngine.Object dinner = LoadAsset(DinnerDialoguePath);
        UnityEngine.Object reminder = LoadAsset(ReminderDialoguePath);
        UnityEngine.Object freeTime = LoadAsset(FreeTimeDialoguePath);

        Assert.That(ContainsSpeaker(healing, "艾拉"), Is.True, "疗伤段必须保留艾拉作为正式承载。");
        Assert.That(ContainsText(healing, "信任"), Is.True, "疗伤段应保留“先救你，不等于完全信任你”的关系语义。");
        Assert.That(ContainsText(healing, "不像这附近的人"), Is.True, "疗伤段应保留艾拉对主角衣着和来历的谨慎观察。");
        Assert.That(ContainsText(healing, "先把人救下来"), Is.True, "疗伤段应保留村长先救人、后追问的稳场语义。");
        Assert.That(ContainsText(healing, "还剩下多少命"), Is.True, "疗伤段应把 HP 显现和主角自觉的生命底线绑定起来。");
        Assert.That(ContainsText(healing, "看清自己还能撑到哪一步"), Is.True, "疗伤段应明确把后续干活前的体力边界钉出来。");

        Assert.That(ContainsText(workbench, "村里能用的东西"), Is.True, "工作台回忆段应明确这是村里当前最基础的技术面。");
        Assert.That(ContainsText(workbench, "老乔治"), Is.True, "工作台回忆段应把老乔治作为技术参照被提及。");
        Assert.That(ContainsText(workbench, "最顶用"), Is.True, "工作台回忆段应强调这是村里当前最好但仍很基础的水平。");
        Assert.That(ContainsText(workbench, "手先醒了"), Is.True, "工作台回忆段应明确这是一次先于记忆的手艺回潮。");
        Assert.That(ContainsText(workbench, "挣一口饭"), Is.True, "工作台回忆段应把“会做活”明确连回 Day1 生存语义。");

        Assert.That(ContainsSpeaker(dinner, "卡尔"), Is.True, "晚餐冲突段必须保留卡尔。");
        Assert.That(ContainsText(dinner, "来历不明"), Is.True, "晚餐冲突段应保留卡尔对外来者的不信任。");
        Assert.That(ContainsText(dinner, "白天把他带进村已经够冒险了"), Is.True, "晚餐冲突段应把村里对外来者的现实成本说透。");
        Assert.That(ContainsText(dinner, "明天要是真能下地、能做活"), Is.True, "晚餐冲突段应把“明天用做活证明自己”抛到台面上。");

        Assert.That(ContainsText(reminder, "两点之前必须睡下"), Is.True, "归途提醒段必须钉住两点规则。");
        Assert.That(ContainsText(reminder, "不喜欢还醒着的人"), Is.True, "归途提醒段应保留这片土地夜里不对劲的语义。");
        Assert.That(ContainsText(reminder, "两点之后会怎样"), Is.True, "归途提醒段应允许主角追问两点规则的后果。");
        Assert.That(ContainsText(reminder, "别让自己留在外面"), Is.True, "归途提醒段应把夜里不能留在外面的硬警告说死。");
        Assert.That(ContainsText(reminder, "白吃白住"), Is.True, "归途提醒段应把主角对“明天证明自己”的念头显式接出来。");

        Assert.That(ContainsText(freeTime, "老乔治"), Is.True, "自由时段见闻包应让村庄生活面真正存在。");
        Assert.That(ContainsSpeaker(freeTime, "小孩"), Is.True, "自由时段见闻包应保留村中小孩的视线。");
        Assert.That(ContainsText(freeTime, "老汤姆"), Is.True, "自由时段应补上河边/码头的夜间见闻。");
        Assert.That(ContainsText(freeTime, "老杰克"), Is.True, "自由时段应补上农田尾声的夜间见闻。");
        Assert.That(ContainsSpeaker(freeTime, "小米"), Is.True, "自由时段应让原案里的群众目光真正出声。");
        Assert.That(ContainsText(freeTime, "他明天真的会去帮忙吗"), Is.True, "自由时段应把村里对主角明天表现的观察放出来。");
        Assert.That(ContainsText(freeTime, "还得看我能不能活过这一夜"), Is.True, "自由时段尾声应明确 Day1 的未被接纳感和过夜压力。");
    }

    private static UnityEngine.Object LoadAsset(string assetPath)
    {
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
