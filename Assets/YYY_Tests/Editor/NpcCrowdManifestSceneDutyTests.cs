using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

[TestFixture]
public class NpcCrowdManifestSceneDutyTests
{
    private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";

    [Test]
    public void CrowdManifest_ShouldExposeDirectorSemanticDuties_ForNpcOwnHandoff()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type dutyType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdSceneDuty");
        Type growthIntentType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdGrowthIntent");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);

        Assert.That(manifest, Is.Not.Null, "缺少 SpringDay1 crowd manifest。");

        object[] entries = GetPublicProperty<IEnumerable>(manifest, "Entries").Cast<object>().ToArray();
        Assert.That(entries.Length, Is.EqualTo(8), "crowd manifest 仍未覆盖新 8 人。");

        Dictionary<string, object> entryByNpcId = entries.ToDictionary(
            entry => GetPublicField<string>(entry, "npcId"),
            entry => entry,
            StringComparer.OrdinalIgnoreCase);

        foreach (object entry in entries)
        {
            Array duties = GetPublicField<Array>(entry, "sceneDuties");
            string[] semanticAnchors = GetPublicField<string[]>(entry, "semanticAnchorIds");

            Assert.That(duties, Is.Not.Null.And.Length.GreaterThan(0), $"{GetPublicField<string>(entry, "npcId")} 缺少 scene duties。");
            Assert.That(semanticAnchors, Is.Not.Null.And.Length.GreaterThan(0), $"{GetPublicField<string>(entry, "npcId")} 缺少 semantic anchors。");
        }

        AssertEntry(entryByNpcId["103"], dutyType, growthIntentType, "EnterVillagePostEntryCrowd", "EnterVillageKidLook", "UpgradeCandidate", "KidLook_01");
        AssertEntry(entryByNpcId["301"], dutyType, growthIntentType, "NightWitness", null, "UpgradeCandidate", "NightWitness_01");
        AssertEntry(entryByNpcId["201"], dutyType, growthIntentType, "DinnerBackground", "DailyStand", "HoldAnonymous", "DinnerBackgroundRoot");

        int dinnerBackgroundCount = entries.Count(entry => HasDuty(entry, dutyType, "DinnerBackground"));
        int dailyStandCount = entries.Count(entry => HasDuty(entry, dutyType, "DailyStand"));
        int nightWitnessCount = entries.Count(entry => HasDuty(entry, dutyType, "NightWitness"));
        int anonymousCount = entries.Count(entry => HasGrowthIntent(entry, growthIntentType, "HoldAnonymous"));
        int upgradeCount = entries.Count(entry => HasGrowthIntent(entry, growthIntentType, "UpgradeCandidate"));

        Assert.That(dinnerBackgroundCount, Is.GreaterThanOrEqualTo(5), "DinnerBackground 背景层承接人数不够。");
        Assert.That(dailyStandCount, Is.GreaterThanOrEqualTo(6), "DailyStand 次日站位承接人数不够。");
        Assert.That(nightWitnessCount, Is.GreaterThanOrEqualTo(2), "NightWitness 夜间见闻层承接人数不够。");
        Assert.That(anonymousCount, Is.GreaterThanOrEqualTo(2), "至少应保留一批继续匿名的 crowd。");
        Assert.That(upgradeCount, Is.GreaterThanOrEqualTo(4), "至少应有一批升级候选 crowd。");
    }

    private static void AssertEntry(
        object entry,
        Type dutyType,
        Type growthIntentType,
        string requiredDutyA,
        string requiredDutyB,
        string expectedGrowthIntent,
        string requiredSemanticAnchor)
    {
        Assert.That(HasDuty(entry, dutyType, requiredDutyA), Is.True, $"{GetPublicField<string>(entry, "npcId")} 缺少 duty={requiredDutyA}。");

        if (!string.IsNullOrWhiteSpace(requiredDutyB))
        {
            Assert.That(HasDuty(entry, dutyType, requiredDutyB), Is.True, $"{GetPublicField<string>(entry, "npcId")} 缺少 duty={requiredDutyB}。");
        }

        Assert.That(HasGrowthIntent(entry, growthIntentType, expectedGrowthIntent), Is.True, $"{GetPublicField<string>(entry, "npcId")} growthIntent 不对。");
        Assert.That(GetPublicField<string[]>(entry, "semanticAnchorIds"), Does.Contain(requiredSemanticAnchor), $"{GetPublicField<string>(entry, "npcId")} 缺少 semantic anchor={requiredSemanticAnchor}。");
    }

    private static bool HasDuty(object entry, Type dutyType, string dutyName)
    {
        object expected = Enum.Parse(dutyType, dutyName);
        Array duties = GetPublicField<Array>(entry, "sceneDuties");
        foreach (object duty in duties)
        {
            if (Equals(duty, expected))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasGrowthIntent(object entry, Type growthIntentType, string intentName)
    {
        object expected = Enum.Parse(growthIntentType, intentName);
        object actual = GetPublicField<object>(entry, "growthIntent");
        return Equals(actual, expected);
    }

    private static T GetPublicField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static T GetPublicProperty<T>(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return (T)property.GetValue(target);
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolved = TryResolveType(assembly, typeName);
            if (resolved != null)
            {
                return resolved;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }
}
