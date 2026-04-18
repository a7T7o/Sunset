using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

[TestFixture]
public class NpcCrowdResidentDirectorBridgeTests
{
    private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";

    [Test]
    public void StageBook_ShouldResolveCueForAllPriorityResidents_InEnterVillageAndDinner()
    {
        object manifest = LoadManifest();
        object book = LoadStageBook();

        AssertPriorityBeatFullyConsumable(manifest, book, "EnterVillage_PostEntry");
        AssertPriorityBeatFullyConsumable(manifest, book, "DinnerConflict_Table");
    }

    [Test]
    public void EnterVillage_ShouldResolveCueForExpandedOnstageResidentCrowd()
    {
        object manifest = LoadManifest();
        object book = LoadStageBook();

        string[] onstageNpcIds = GetSnapshotNpcIds(manifest, "EnterVillage_PostEntry", "priority")
            .Concat(GetSnapshotNpcIds(manifest, "EnterVillage_PostEntry", "support"))
            .OrderBy(npcId => npcId, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.That(onstageNpcIds, Is.EqualTo(new[] { "101", "103", "104", "201", "202", "203" }));

        for (int index = 0; index < onstageNpcIds.Length; index++)
        {
            string npcId = onstageNpcIds[index];
            object entry = FindManifestEntry(manifest, npcId);
            Assert.That(entry, Is.Not.Null, $"manifest 缺少 npcId={npcId}");

            object cue = InvokeInstance(book, "TryResolveCue", "EnterVillage_PostEntry", entry);
            Assert.That(cue, Is.Not.Null, $"EnterVillage_PostEntry 的 onstage resident={npcId} 还没有导演 cue。");
            AssertCueContract(cue, entry, "EnterVillage_PostEntry");
        }
    }

    [Test]
    public void DinnerConflict_CapturedCues_ShouldStayInsideResidentPriorityRoster()
    {
        object manifest = LoadManifest();
        object book = LoadStageBook();
        object beat = InvokeInstance(book, "FindBeat", "DinnerConflict_Table");

        Assert.That(beat, Is.Not.Null, "StageBook 缺少 DinnerConflict_Table。");

        string[] onstageNpcIds = GetOnstageResidentNpcIds(manifest, "DinnerConflict_Table");
        object[] cues = GetBeatCues(beat);
        string[] cueNpcIds = cues
            .Select(cue => GetFieldValue(cue, "npcId") as string)
            .Where(npcId => !string.IsNullOrWhiteSpace(npcId))
            .Select(npcId => npcId.Trim())
            .Where(npcId => onstageNpcIds.Contains(npcId, StringComparer.OrdinalIgnoreCase))
            .OrderBy(npcId => npcId, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.That(cueNpcIds.Length, Is.GreaterThanOrEqualTo(4), "晚饭冲突段当前至少应有一层可被导演直接消费的背景 cue。");
        for (int index = 0; index < cueNpcIds.Length; index++)
        {
            string cueNpcId = cueNpcIds[index];
            Assert.That(onstageNpcIds, Does.Contain(cueNpcId), $"DinnerConflict 的 cue 不应越界拉到 onstage resident roster 之外：{cueNpcId}");

            object entry = FindManifestEntry(manifest, cueNpcId);
            Assert.That(entry, Is.Not.Null, $"manifest 缺少 npcId={cueNpcId}");

            object cue = cues.First(item => string.Equals(GetFieldValue(item, "npcId") as string, cueNpcId, StringComparison.OrdinalIgnoreCase));
            AssertCueContract(cue, entry, "DinnerConflict_Table");
        }
    }

    [Test]
    public void DirectorCues_ShouldStayInsideManifestSemanticAnchorsAndDuties_ForActiveResidentBridgeBeats()
    {
        object manifest = LoadManifest();
        object book = LoadStageBook();

        AssertBeatCueContracts(manifest, book, "EnterVillage_PostEntry");
        AssertBeatCueContracts(manifest, book, "DinnerConflict_Table");
    }

    [Test]
    public void DirectorCueDatabase_ShouldPreferBeatSpecificCueBeforeEnterVillageFallbackAlias()
    {
        object manifest = LoadManifest();
        _ = LoadStageBook();
        object entry = FindManifestEntry(manifest, "203");
        Assert.That(entry, Is.Not.Null, "manifest 缺少 npcId=203");

        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        bool resolved = (bool)InvokeStatic(databaseType, "TryResolveCue", "DinnerConflict_Table", entry);
        object cue = GetOutArgumentFromLastInvoke();

        Assert.That(resolved, Is.True, "DinnerConflict_Table 应能解析到 runtime cue。");
        Assert.That(cue, Is.Not.Null, "DinnerConflict_Table 的 runtime cue 不应为空。");
        Assert.That(GetFieldValue(cue, "cueId") as string, Is.EqualTo("dinner-bg-203"),
            "如果 DinnerConflict 已经有自己的 cue，runtime 不应再偷偷回退成 EnterVillage 的旧 cue。");
    }

    private static void AssertPriorityBeatFullyConsumable(object manifest, object book, string beatKey)
    {
        string[] priorityNpcIds = GetSnapshotNpcIds(manifest, beatKey, "priority");
        Assert.That(priorityNpcIds.Length, Is.GreaterThan(0), $"{beatKey} 当前没有 priority resident。");

        for (int index = 0; index < priorityNpcIds.Length; index++)
        {
            string npcId = priorityNpcIds[index];
            object entry = FindManifestEntry(manifest, npcId);
            Assert.That(entry, Is.Not.Null, $"manifest 缺少 npcId={npcId}");

            object cue = InvokeInstance(book, "TryResolveCue", beatKey, entry);
            Assert.That(cue, Is.Not.Null, $"{beatKey} 的 priority resident={npcId} 还没有导演 cue，day1 不能直接消费。");
            AssertCueContract(cue, entry, beatKey);
        }
    }

    private static void AssertBeatCueContracts(object manifest, object book, string beatKey)
    {
        object beat = InvokeInstance(book, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat={beatKey}");

        object[] cues = GetBeatCues(beat);
        Assert.That(cues.Length, Is.GreaterThan(0), $"{beatKey} 当前没有任何 crowd cue。");
        string[] onstageNpcIds = GetOnstageResidentNpcIds(manifest, beatKey);
        int validatedCueCount = 0;

        for (int index = 0; index < cues.Length; index++)
        {
            object cue = cues[index];
            string npcId = GetFieldValue(cue, "npcId") as string;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                continue;
            }

            string normalizedNpcId = npcId.Trim();
            if (!onstageNpcIds.Contains(normalizedNpcId, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            object entry = FindManifestEntry(manifest, normalizedNpcId);
            Assert.That(entry, Is.Not.Null, $"{beatKey} 的 cue 指向了 manifest 不存在的 npcId={npcId}");
            AssertCueContract(cue, entry, beatKey);
            validatedCueCount++;
        }

        Assert.That(validatedCueCount, Is.GreaterThan(0), $"{beatKey} 当前没有任何 manifest-backed resident cue 被导演桥接消费。");
    }

    private static void AssertManifestBackedBeatCuesResolvable(object manifest, object book, string beatKey)
    {
        object beat = InvokeInstance(book, "FindBeat", beatKey);
        Assert.That(beat, Is.Not.Null, $"StageBook 缺少 beat={beatKey}");

        object[] cues = GetBeatCues(beat);
        int residentCueCount = 0;
        for (int index = 0; index < cues.Length; index++)
        {
            object cue = cues[index];
            string npcId = GetFieldValue(cue, "npcId") as string;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                continue;
            }

            object entry = FindManifestEntry(manifest, npcId.Trim());
            if (entry == null)
            {
                continue;
            }

            object resolvedCue = InvokeInstance(book, "TryResolveCue", beatKey, entry);
            Assert.That(resolvedCue, Is.Not.Null, $"{beatKey} 的 resident cue 应该能被导演数据库重新解析：npcId={npcId}");
            AssertCueContract(resolvedCue, entry, beatKey);
            residentCueCount++;
        }

        Assert.That(residentCueCount, Is.GreaterThan(0), $"{beatKey} 当前没有任何 resident bridge cue 可用于复核。");
    }

    private static void AssertCueContract(object cue, object entry, string beatKey)
    {
        string cueId = GetFieldValue(cue, "cueId") as string ?? string.Empty;
        string cueNpcId = GetFieldValue(cue, "npcId") as string ?? string.Empty;
        string semanticAnchorId = GetFieldValue(cue, "semanticAnchorId") as string ?? string.Empty;
        object duty = GetFieldValue(cue, "duty");
        string entryNpcId = GetFieldValue(entry, "npcId") as string ?? string.Empty;

        Assert.That(string.IsNullOrWhiteSpace(cueId), Is.False, $"{beatKey} 的 resident cue 不应丢失 cueId。");
        Assert.That(string.Equals(cueNpcId.Trim(), entryNpcId.Trim(), StringComparison.OrdinalIgnoreCase), Is.True,
            $"{beatKey}/{cueId} 的 cue npcId 应继续和 manifest entry 对齐。");

        if (string.IsNullOrWhiteSpace(semanticAnchorId))
        {
            Assert.That(duty != null && !string.Equals(duty.ToString(), "None", StringComparison.Ordinal), Is.True,
                $"{beatKey}/{cueId} 至少应保留 semanticAnchorId 或 duty 其一，不能退化成无语义 cue。");
        }
    }

    private static object LoadManifest()
    {
        Type manifestType = ResolveTypeOrFail("Sunset.Story.SpringDay1NpcCrowdManifest");
        object manifest = AssetDatabase.LoadAssetAtPath(ManifestPath, manifestType);
        Assert.That(manifest, Is.Not.Null, "未能加载 SpringDay1NpcCrowdManifest.asset");
        return manifest;
    }

    private static object LoadStageBook()
    {
        Type databaseType = ResolveTypeOrFail("Sunset.Story.SpringDay1DirectorStagingDatabase");
        object book = InvokeStatic(databaseType, "Load", true);
        Assert.That(book, Is.Not.Null, "未能加载 SpringDay1DirectorStageBook");
        return book;
    }

    private static object FindManifestEntry(object manifest, string npcId)
    {
        bool found = (bool)InvokeInstance(manifest, "TryGetEntry", npcId);
        if (!found)
        {
            return null;
        }

        return GetOutArgumentFromLastInvoke();
    }

    private static string[] GetSnapshotNpcIds(object manifest, string beatKey, string fieldName)
    {
        object snapshot = InvokeInstance(manifest, "BuildBeatConsumptionSnapshot", beatKey);
        Array entries = GetFieldValue(snapshot, fieldName) as Array;
        if (entries == null || entries.Length == 0)
        {
            return Array.Empty<string>();
        }

        return entries
            .Cast<object>()
            .Select(entry => GetFieldValue(entry, "npcId") as string)
            .Where(npcId => !string.IsNullOrWhiteSpace(npcId))
            .Select(npcId => npcId.Trim())
            .OrderBy(npcId => npcId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string[] GetOnstageResidentNpcIds(object manifest, string beatKey)
    {
        return GetSnapshotNpcIds(manifest, beatKey, "priority")
            .Concat(GetSnapshotNpcIds(manifest, beatKey, "support"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(npcId => npcId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static object[] GetBeatCues(object beat)
    {
        Array cues = GetFieldValue(beat, "actorCues") as Array;
        if (cues == null || cues.Length == 0)
        {
            return Array.Empty<object>();
        }

        return cues.Cast<object>().Where(cue => cue != null).ToArray();
    }

    private static object s_lastOutArgument;

    private static object InvokeStatic(Type targetType, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(targetType, methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, args);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        object[] effectiveArgs = BuildInvokeArgs(method, args);
        object result = method.Invoke(null, effectiveArgs);
        CaptureOutArgument(method, effectiveArgs);
        return result;
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(target.GetType(), methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, args);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        object[] effectiveArgs = BuildInvokeArgs(method, args);
        object result = method.Invoke(target, effectiveArgs);
        CaptureOutArgument(method, effectiveArgs);
        return result;
    }

    private static object GetOutArgumentFromLastInvoke()
    {
        return s_lastOutArgument;
    }

    private static void CaptureOutArgument(MethodInfo method, object[] effectiveArgs)
    {
        s_lastOutArgument = null;
        ParameterInfo[] parameters = method.GetParameters();
        for (int index = 0; index < parameters.Length; index++)
        {
            if (!parameters[index].IsOut)
            {
                continue;
            }

            s_lastOutArgument = effectiveArgs[index];
            return;
        }
    }

    private static object[] BuildInvokeArgs(MethodInfo method, object[] args)
    {
        object[] rawArgs = args ?? Array.Empty<object>();
        ParameterInfo[] parameters = method.GetParameters();
        object[] invokeArgs = new object[parameters.Length];
        for (int index = 0; index < parameters.Length; index++)
        {
            if (parameters[index].IsOut)
            {
                invokeArgs[index] = null;
                continue;
            }

            invokeArgs[index] = index < rawArgs.Length ? rawArgs[index] : null;
        }

        return invokeArgs;
    }

    private static MethodInfo FindMethod(Type targetType, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo[] candidates = targetType
            .GetMethods(bindingFlags)
            .Where(method => string.Equals(method.Name, methodName, StringComparison.Ordinal))
            .ToArray();

        for (int methodIndex = 0; methodIndex < candidates.Length; methodIndex++)
        {
            MethodInfo method = candidates[methodIndex];
            if (ParametersMatch(method.GetParameters(), args))
            {
                return method;
            }
        }

        return null;
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object[] args)
    {
        object[] rawArgs = args ?? Array.Empty<object>();
        int nonOutCount = parameters.Count(parameter => !parameter.IsOut);
        if (nonOutCount != rawArgs.Length)
        {
            return false;
        }

        int argIndex = 0;
        for (int index = 0; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];
            if (parameter.IsOut)
            {
                continue;
            }

            object argument = rawArgs[argIndex++];
            Type parameterType = parameter.ParameterType;
            if (argument == null)
            {
                if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                {
                    return false;
                }

                continue;
            }

            if (!parameterType.IsInstanceOfType(argument))
            {
                return false;
            }
        }

        return true;
    }

    private static object GetFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName, throwOnError: false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }
}
