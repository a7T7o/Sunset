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
    private static readonly ExpectedEntry[] ExpectedEntries =
    {
        new ExpectedEntry("101", "EnterVillage", "DayEnd", "UpgradeCandidate", new[] { "EnterVillagePostEntryCrowd", "DinnerBackground", "DailyStand" }),
        new ExpectedEntry("102", "FarmingTutorial", "DayEnd", "UpgradeCandidate", new[] { "NightWitness", "DailyStand" }),
        new ExpectedEntry("103", "EnterVillage", "DayEnd", "UpgradeCandidate", new[] { "EnterVillagePostEntryCrowd", "EnterVillageKidLook", "DailyStand" }),
        new ExpectedEntry("104", "WorkbenchFlashback", "DayEnd", "StableSupport", new[] { "EnterVillagePostEntryCrowd", "DinnerBackground", "DailyStand" }),
        new ExpectedEntry("201", "HealingAndHP", "DayEnd", "HoldAnonymous", new[] { "EnterVillagePostEntryCrowd", "DinnerBackground", "DailyStand" }),
        new ExpectedEntry("202", "FarmingTutorial", "DayEnd", "HoldAnonymous", new[] { "EnterVillagePostEntryCrowd", "DinnerBackground", "DailyStand" }),
        new ExpectedEntry("203", "DinnerConflict", "DayEnd", "StableSupport", new[] { "EnterVillagePostEntryCrowd", "DinnerBackground", "DailyStand" })
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
        "朽钟"
    };

    private static readonly ExpectedResidentProfile[] ExpectedResidentProfiles =
    {
        new ExpectedResidentProfile(
            "101",
            "DaytimeResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "FirstNotice", "SilentWatch", "PretendBusy", "AmbientPressure"),
            Beat("DinnerConflict_Table", "AlreadyAround", "SilentWatch", "AmbientPressure", "KeepRoutine"),
            Beat("ReturnAndReminder_WalkBack", "OffscreenRoutine", "KeepRoutine"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "SilentWatch"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine", "AmbientPressure")),
        new ExpectedResidentProfile(
            "102",
            "PeripheralResident",
            Beat("EnterVillage_PostEntry", "OffscreenRoutine", "KeepRoutine"),
            Beat("DinnerConflict_Table", "OffscreenRoutine", "AmbientPressure"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "SilentWatch", "AmbientPressure"),
            Beat("FreeTime_NightWitness", "AlreadyAround", "FirstNotice", "SilentWatch", "AmbientPressure"),
            Beat("DayEnd_Settle", "ReturnHome", "OffscreenRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "KeepRoutine", "PretendBusy")),
        new ExpectedResidentProfile(
            "103",
            "DaytimeResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "FirstNotice", "SilentWatch", "YieldSpace", "PretendBusy"),
            Beat("DinnerConflict_Table", "AlreadyAround", "SilentWatch", "AmbientPressure"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "FirstNotice", "SilentWatch"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "AmbientPressure"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine", "AmbientPressure")),
        new ExpectedResidentProfile(
            "104",
            "PeripheralResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "YieldSpace", "PretendBusy", "KeepRoutine"),
            Beat("DinnerConflict_Table", "AlreadyAround", "FirstNotice", "SilentWatch", "AmbientPressure", "KeepRoutine"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "YieldSpace", "KeepRoutine"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "KeepRoutine"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine")),
        new ExpectedResidentProfile(
            "201",
            "DaytimeResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "FirstNotice", "YieldSpace", "KeepRoutine"),
            Beat("DinnerConflict_Table", "AlreadyAround", "FirstNotice", "SilentWatch", "AmbientPressure"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "YieldSpace", "KeepRoutine"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "KeepRoutine"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine")),
        new ExpectedResidentProfile(
            "202",
            "PeripheralResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "FirstNotice", "PretendBusy", "KeepRoutine"),
            Beat("DinnerConflict_Table", "AlreadyAround", "FirstNotice", "AmbientPressure", "KeepRoutine"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "YieldSpace", "KeepRoutine"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "KeepRoutine"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine")),
        new ExpectedResidentProfile(
            "203",
            "DaytimeResident",
            Beat("EnterVillage_PostEntry", "AlreadyAround", "SilentWatch", "AmbientPressure", "KeepRoutine"),
            Beat("DinnerConflict_Table", "AlreadyAround", "FirstNotice", "SilentWatch", "AmbientPressure", "KeepRoutine"),
            Beat("ReturnAndReminder_WalkBack", "AlreadyAround", "YieldSpace", "KeepRoutine"),
            Beat("FreeTime_NightWitness", "OffscreenRoutine", "KeepRoutine"),
            Beat("DayEnd_Settle", "ReturnHome", "KeepRoutine", "AmbientPressure"),
            Beat("DailyStand_Preview", "AlreadyAround", "PretendBusy", "KeepRoutine"))
    };

    private static readonly ExpectedDirectorConsumptionBeat[] ExpectedDirectorConsumptionBeats =
    {
        new ExpectedDirectorConsumptionBeat(
            "EnterVillage_PostEntry",
            priorityNpcIds: new[] { "101", "103" },
            supportNpcIds: new[] { "104", "201", "202", "203" },
            backstagePressureNpcIds: Array.Empty<string>(),
            traceNpcIds: new[] { "102" }),
        new ExpectedDirectorConsumptionBeat(
            "DinnerConflict_Table",
            priorityNpcIds: new[] { "101", "103", "104", "201", "202", "203" },
            supportNpcIds: Array.Empty<string>(),
            backstagePressureNpcIds: new[] { "102" },
            traceNpcIds: Array.Empty<string>()),
        new ExpectedDirectorConsumptionBeat(
            "FreeTime_NightWitness",
            priorityNpcIds: new[] { "102" },
            supportNpcIds: new[] { "101" },
            backstagePressureNpcIds: new[] { "103" },
            traceNpcIds: new[] { "104", "201", "202", "203" })
    };

    [Test]
    public void CrowdManifest_ShouldExposeDirectorSemanticDuties_ForNpcOwnHandoff()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type dutyType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdSceneDuty");
        Type growthIntentType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdGrowthIntent");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);

        Assert.That(manifest, Is.Not.Null, "缺少 SpringDay1 crowd manifest。");

        object[] entries = GetPublicProperty<IEnumerable>(manifest, "Entries").Cast<object>().ToArray();
        Assert.That(entries.Length, Is.EqualTo(7), "crowd manifest 当前应只覆盖 7 人正式阵容。");

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

        foreach (ExpectedEntry expected in ExpectedEntries)
        {
            Assert.That(entryByNpcId.ContainsKey(expected.NpcId), Is.True, $"manifest 缺少 npcId={expected.NpcId}。");
            AssertEntry(entryByNpcId[expected.NpcId], dutyType, growthIntentType, expected);
        }

        int dinnerBackgroundCount = entries.Count(entry => HasDuty(entry, dutyType, "DinnerBackground"));
        int dailyStandCount = entries.Count(entry => HasDuty(entry, dutyType, "DailyStand"));
        int nightWitnessCount = entries.Count(entry => HasDuty(entry, dutyType, "NightWitness"));
        int anonymousCount = entries.Count(entry => HasGrowthIntent(entry, growthIntentType, "HoldAnonymous"));
        int upgradeCount = entries.Count(entry => HasGrowthIntent(entry, growthIntentType, "UpgradeCandidate"));

        Assert.That(dinnerBackgroundCount, Is.GreaterThanOrEqualTo(5), "DinnerBackground 背景层承接人数不够。");
        Assert.That(dailyStandCount, Is.GreaterThanOrEqualTo(6), "DailyStand 次日站位承接人数不够。");
        Assert.That(nightWitnessCount, Is.GreaterThanOrEqualTo(1), "NightWitness 夜间见闻层承接人数不够。");
        Assert.That(anonymousCount, Is.GreaterThanOrEqualTo(2), "至少应保留一批继续匿名的 crowd。");
        Assert.That(upgradeCount, Is.GreaterThanOrEqualTo(3), "至少应有一批升级候选 crowd。");
    }

    [Test]
    public void CrowdManifest_ShouldExposeResidentSemanticMatrix_ForVillageResidentLayer()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type residentBaselineType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBaseline");
        Type residentFlagsType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdResidentBeatFlags");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);

        Assert.That(manifest, Is.Not.Null, "缺少 SpringDay1 crowd manifest。");

        object[] entries = GetPublicProperty<IEnumerable>(manifest, "Entries").Cast<object>().ToArray();
        Dictionary<string, object> entryByNpcId = entries.ToDictionary(
            entry => GetPublicField<string>(entry, "npcId"),
            entry => entry,
            StringComparer.OrdinalIgnoreCase);

        foreach (ExpectedResidentProfile expected in ExpectedResidentProfiles)
        {
            Assert.That(entryByNpcId.ContainsKey(expected.NpcId), Is.True, $"manifest 缺少 npcId={expected.NpcId}。");
            AssertResidentProfile(entryByNpcId[expected.NpcId], residentBaselineType, residentFlagsType, expected);
        }
    }

    [Test]
    public void CrowdManifest_ShouldExposeDirectorConsumptionRoles_ForDay1ResidentDeployment()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        Type consumptionRoleType = ResolveTypeOrFail("Sunset.Story.SpringDay1CrowdDirectorConsumptionRole");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);

        Assert.That(manifest, Is.Not.Null, "缺少 SpringDay1 crowd manifest。");

        Dictionary<string, object> entryByNpcId = GetPublicProperty<IEnumerable>(manifest, "Entries")
            .Cast<object>()
            .ToDictionary(entry => GetPublicField<string>(entry, "npcId"), entry => entry, StringComparer.OrdinalIgnoreCase);

        foreach (ExpectedDirectorConsumptionBeat expectedBeat in ExpectedDirectorConsumptionBeats)
        {
            AssertExactRoleSet(manifest, consumptionRoleType, expectedBeat.BeatKey, "Priority", expectedBeat.PriorityNpcIds);
            AssertExactRoleSet(manifest, consumptionRoleType, expectedBeat.BeatKey, "Support", expectedBeat.SupportNpcIds);
            AssertExactRoleSet(manifest, consumptionRoleType, expectedBeat.BeatKey, "BackstagePressure", expectedBeat.BackstagePressureNpcIds);
            AssertExactRoleSet(manifest, consumptionRoleType, expectedBeat.BeatKey, "Trace", expectedBeat.TraceNpcIds);
        }

        Assert.That(
            InvokeInstance<bool>(entryByNpcId["102"], "IsDirectorBackstagePressureBeat", "DinnerConflict_Table"),
            Is.True,
            "102 在晚饭冲突段应属于离屏压力，不该被 day1 当成前台露脸演员。");
        Assert.That(
            InvokeInstance<object>(entryByNpcId["101"], "GetDirectorConsumptionRole", "EnterVillage_PostEntry").ToString(),
            Is.EqualTo("Priority"),
            "101 在进村围观段应是前台 priority。");
        Assert.That(
            InvokeInstance<object>(entryByNpcId["104"], "GetDirectorConsumptionRole", "EnterVillage_PostEntry").ToString(),
            Is.EqualTo("Support"),
            "104 在进村围观段应作为背景 support 被 day1 接住，而不是继续留在离屏 trace。");
    }

    [Test]
    public void CrowdManifest_ShouldBuildBeatConsumptionSnapshot_ForDay1DirectConsumption()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);

        Assert.That(manifest, Is.Not.Null, "缺少 SpringDay1 crowd manifest。");

        object enterVillageSnapshot = InvokeInstance<object>(manifest, "BuildBeatConsumptionSnapshot", "EnterVillage_PostEntry");
        Assert.That(GetPublicField<string>(enterVillageSnapshot, "beatKey"), Is.EqualTo("EnterVillage_PostEntry"));
        Assert.That(GetSnapshotNpcIds(enterVillageSnapshot, "priority"), Is.EqualTo(new[] { "101", "103" }));
        Assert.That(GetSnapshotNpcIds(enterVillageSnapshot, "support"), Is.EqualTo(new[] { "104", "201", "202", "203" }));
        Assert.That(GetSnapshotNpcIds(enterVillageSnapshot, "trace"), Is.EqualTo(new[] { "102" }));
        Assert.That(GetSnapshotNpcIds(enterVillageSnapshot, "backstagePressure"), Is.Empty);

        object dinnerSnapshot = InvokeInstance<object>(manifest, "BuildBeatConsumptionSnapshot", "DinnerConflict_Table");
        Assert.That(GetSnapshotNpcIds(dinnerSnapshot, "priority"), Is.EqualTo(new[] { "101", "103", "104", "201", "202", "203" }));
        Assert.That(GetSnapshotNpcIds(dinnerSnapshot, "backstagePressure"), Is.EqualTo(new[] { "102" }));

        string[] backstageNotes = GetSnapshotNotes(dinnerSnapshot, "backstagePressure");
        Assert.That(backstageNotes.Length, Is.EqualTo(2), "晚饭冲突段的后台压力切片应把 note 一起带出来。");
        Assert.That(backstageNotes.All(note => !string.IsNullOrWhiteSpace(note)), Is.True, "snapshot note 不应为空。");

        string[] priorityAnchors = GetSnapshotAnchors(dinnerSnapshot, "priority");
        Assert.That(priorityAnchors.Length, Is.GreaterThan(0), "snapshot 应保留可回溯的 semantic anchor 信息。");
    }

    private static void AssertEntry(
        object entry,
        Type dutyType,
        Type growthIntentType,
        ExpectedEntry expected)
    {
        string npcId = GetPublicField<string>(entry, "npcId");
        Array duties = GetPublicField<Array>(entry, "sceneDuties");
        string[] semanticAnchors = GetPublicField<string[]>(entry, "semanticAnchorIds");
        string[] actualDutyNames = duties.Cast<object>().Select(duty => duty.ToString()).OrderBy(name => name).ToArray();
        string[] actualAnchors = semanticAnchors.OrderBy(name => name).ToArray();
        string actualMinPhase = GetPublicField<object>(entry, "minPhase")?.ToString();
        string actualMaxPhase = GetPublicField<object>(entry, "maxPhase")?.ToString();

        Assert.That(actualDutyNames, Is.EqualTo(expected.SceneDuties.OrderBy(name => name).ToArray()), $"{npcId} 的 scene duty matrix 漂了。");
        Assert.That(actualAnchors.Length, Is.GreaterThan(0), $"{npcId} 不应丢失全部 semantic anchors。");
        Assert.That(actualMinPhase, Is.EqualTo(expected.MinPhase), $"{npcId} 的 minPhase 漂了。");
        Assert.That(actualMaxPhase, Is.EqualTo(expected.MaxPhase), $"{npcId} 的 maxPhase 漂了。");
        Assert.That(HasGrowthIntent(entry, growthIntentType, expected.GrowthIntent), Is.True, $"{npcId} growthIntent 不对。");

        for (int index = 0; index < expected.SceneDuties.Length; index++)
        {
            string dutyName = expected.SceneDuties[index];
            Assert.That(HasDuty(entry, dutyType, dutyName), Is.True, $"{npcId} 缺少 duty={dutyName}。");
        }

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

    private static void AssertResidentProfile(
        object entry,
        Type residentBaselineType,
        Type residentFlagsType,
        ExpectedResidentProfile expected)
    {
        string displayName = GetPublicField<string>(entry, "displayName");
        string roleSummary = GetPublicField<string>(entry, "roleSummary");
        object actualBaseline = GetPublicField<object>(entry, "residentBaseline");
        Array beatSemantics = GetPublicField<Array>(entry, "residentBeatSemantics");

        for (int index = 0; index < BannedAuthoredNameTokens.Length; index++)
        {
            string token = BannedAuthoredNameTokens[index];
            Assert.That(displayName, Does.Not.Contain(token), $"{expected.NpcId} 的 displayName 仍带旧具名角色：{token}");
            Assert.That(roleSummary, Does.Not.Contain(token), $"{expected.NpcId} 的 roleSummary 仍带旧具名角色：{token}");
        }

        Assert.That(actualBaseline, Is.EqualTo(Enum.Parse(residentBaselineType, expected.ResidentBaseline)), $"{expected.NpcId} residentBaseline 漂了。");
        Assert.That(beatSemantics, Is.Not.Null.And.Length.EqualTo(expected.Beats.Length), $"{expected.NpcId} resident beat count 不对。");

        for (int index = 0; index < expected.Beats.Length; index++)
        {
            ResidentBeatExpectation expectedBeat = expected.Beats[index];
            object actualBeat = FindBeatSemantic(beatSemantics, expectedBeat.BeatKey);
            Assert.That(actualBeat, Is.Not.Null, $"{expected.NpcId} 缺少 beat={expectedBeat.BeatKey} 的 resident 语义。");

            string note = GetPublicField<string>(actualBeat, "note");
            Assert.That(string.IsNullOrWhiteSpace(note), Is.False, $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} 缺 note。");

            object presenceLevel = GetPublicField<object>(actualBeat, "presenceLevel");
            Assert.That(presenceLevel.ToString(), Is.Not.EqualTo("None"), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} presenceLevel 仍是 None。");
            Assert.That(InvokeInstance<object>(entry, "GetResidentPresenceLevel", expectedBeat.BeatKey), Is.EqualTo(presenceLevel), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} GetResidentPresenceLevel 漂了。");

            object actualFlags = GetPublicField<object>(actualBeat, "flags");
            object expectedFlags = CombineFlags(residentFlagsType, expectedBeat.Flags);
            Assert.That(actualFlags, Is.EqualTo(expectedFlags), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} flags 漂了。");
            Assert.That(InvokeInstance<object>(entry, "GetResidentBeatFlags", expectedBeat.BeatKey), Is.EqualTo(expectedFlags), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} GetResidentBeatFlags 漂了。");
            Assert.That(InvokeInstance<bool>(entry, "HasResidentBeatFlags", expectedBeat.BeatKey, expectedFlags), Is.True, $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} HasResidentBeatFlags 失败。");

            bool expectedPriority = string.Equals(presenceLevel.ToString(), "Visible", StringComparison.Ordinal) ||
                                    string.Equals(presenceLevel.ToString(), "Pressure", StringComparison.Ordinal);
            bool expectedSupport = string.Equals(presenceLevel.ToString(), "Background", StringComparison.Ordinal);
            bool expectedTrace = string.Equals(presenceLevel.ToString(), "Trace", StringComparison.Ordinal);

            Assert.That(InvokeInstance<bool>(entry, "IsDirectorPriorityBeat", expectedBeat.BeatKey), Is.EqualTo(expectedPriority), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} IsDirectorPriorityBeat 漂了。");
            Assert.That(InvokeInstance<bool>(entry, "IsDirectorSupportBeat", expectedBeat.BeatKey), Is.EqualTo(expectedSupport), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} IsDirectorSupportBeat 漂了。");
            Assert.That(InvokeInstance<bool>(entry, "IsDirectorTraceBeat", expectedBeat.BeatKey), Is.EqualTo(expectedTrace), $"{expected.NpcId} 的 beat={expectedBeat.BeatKey} IsDirectorTraceBeat 漂了。");
        }
    }

    private static void AssertExactRoleSet(
        object manifest,
        Type consumptionRoleType,
        string beatKey,
        string roleName,
        string[] expectedNpcIds)
    {
        object role = Enum.Parse(consumptionRoleType, roleName);
        IEnumerable entries = InvokeInstance<IEnumerable>(manifest, "GetEntriesForDirectorConsumptionRole", beatKey, role);
        string[] actualNpcIds = entries
            .Cast<object>()
            .Select(entry => GetPublicField<string>(entry, "npcId"))
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.That(
            actualNpcIds,
            Is.EqualTo((expectedNpcIds ?? Array.Empty<string>()).OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToArray()),
            $"{beatKey} 的 {roleName} 切片漂了。");
    }

    private static string[] GetSnapshotNpcIds(object snapshot, string fieldName)
    {
        Array entries = GetPublicField<Array>(snapshot, fieldName);
        return entries
            .Cast<object>()
            .Select(entry => GetPublicField<string>(entry, "npcId"))
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string[] GetSnapshotNotes(object snapshot, string fieldName)
    {
        Array entries = GetPublicField<Array>(snapshot, fieldName);
        return entries
            .Cast<object>()
            .Select(entry => GetPublicField<string>(entry, "note"))
            .ToArray();
    }

    private static string[] GetSnapshotAnchors(object snapshot, string fieldName)
    {
        Array entries = GetPublicField<Array>(snapshot, fieldName);
        return entries
            .Cast<object>()
            .SelectMany(entry => GetPublicField<string[]>(entry, "semanticAnchorIds") ?? Array.Empty<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(anchor => anchor, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static object FindBeatSemantic(Array beatSemantics, string beatKey)
    {
        foreach (object semantic in beatSemantics)
        {
            if (semantic == null)
            {
                continue;
            }

            string actualBeatKey = GetPublicField<string>(semantic, "beatKey");
            if (string.Equals(actualBeatKey, beatKey, StringComparison.OrdinalIgnoreCase))
            {
                return semantic;
            }
        }

        return null;
    }

    private static object CombineFlags(Type enumType, string[] flagNames)
    {
        int combined = 0;
        for (int index = 0; index < flagNames.Length; index++)
        {
            object value = Enum.Parse(enumType, flagNames[index]);
            combined |= Convert.ToInt32(value);
        }

        return Enum.ToObject(enumType, combined);
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

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
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

    private readonly struct ExpectedEntry
    {
        public ExpectedEntry(string npcId, string minPhase, string maxPhase, string growthIntent, string[] sceneDuties)
        {
            NpcId = npcId;
            MinPhase = minPhase;
            MaxPhase = maxPhase;
            GrowthIntent = growthIntent;
            SceneDuties = sceneDuties ?? Array.Empty<string>();
        }

        public string NpcId { get; }
        public string MinPhase { get; }
        public string MaxPhase { get; }
        public string GrowthIntent { get; }
        public string[] SceneDuties { get; }
    }

    private static ResidentBeatExpectation Beat(string beatKey, params string[] flags)
    {
        return new ResidentBeatExpectation(beatKey, flags);
    }

    private readonly struct ExpectedResidentProfile
    {
        public ExpectedResidentProfile(string npcId, string residentBaseline, params ResidentBeatExpectation[] beats)
        {
            NpcId = npcId;
            ResidentBaseline = residentBaseline;
            Beats = beats ?? Array.Empty<ResidentBeatExpectation>();
        }

        public string NpcId { get; }
        public string ResidentBaseline { get; }
        public ResidentBeatExpectation[] Beats { get; }
    }

    private readonly struct ResidentBeatExpectation
    {
        public ResidentBeatExpectation(string beatKey, string[] flags)
        {
            BeatKey = beatKey;
            Flags = flags ?? Array.Empty<string>();
        }

        public string BeatKey { get; }
        public string[] Flags { get; }
    }

    private readonly struct ExpectedDirectorConsumptionBeat
    {
        public ExpectedDirectorConsumptionBeat(
            string beatKey,
            string[] priorityNpcIds,
            string[] supportNpcIds,
            string[] backstagePressureNpcIds,
            string[] traceNpcIds)
        {
            BeatKey = beatKey;
            PriorityNpcIds = priorityNpcIds ?? Array.Empty<string>();
            SupportNpcIds = supportNpcIds ?? Array.Empty<string>();
            BackstagePressureNpcIds = backstagePressureNpcIds ?? Array.Empty<string>();
            TraceNpcIds = traceNpcIds ?? Array.Empty<string>();
        }

        public string BeatKey { get; }
        public string[] PriorityNpcIds { get; }
        public string[] SupportNpcIds { get; }
        public string[] BackstagePressureNpcIds { get; }
        public string[] TraceNpcIds { get; }
    }
}
