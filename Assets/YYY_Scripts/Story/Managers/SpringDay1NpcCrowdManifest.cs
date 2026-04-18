using System;
using UnityEngine;

namespace Sunset.Story
{
    public static class SpringDay1CrowdResidentBeatKeys
    {
        public const string EnterVillagePostEntry = "EnterVillage_PostEntry";
        public const string DinnerConflictTable = "DinnerConflict_Table";
        public const string ReturnAndReminderWalkBack = "ReturnAndReminder_WalkBack";
        public const string FreeTimeNightWitness = "FreeTime_NightWitness";
        public const string DayEndSettle = "DayEnd_Settle";
        public const string DailyStandPreview = "DailyStand_Preview";
    }

    public enum SpringDay1CrowdSceneDuty
    {
        None = 0,
        EnterVillagePostEntryCrowd = 1,
        EnterVillageKidLook = 2,
        DinnerBackground = 3,
        NightWitness = 4,
        DailyStand = 5
    }

    public enum SpringDay1CrowdGrowthIntent
    {
        HoldAnonymous = 0,
        StableSupport = 1,
        UpgradeCandidate = 2
    }

    public enum SpringDay1CrowdResidentBaseline
    {
        DaytimeResident = 1,
        PeripheralResident = 2,
        NightResident = 3
    }

    public enum SpringDay1CrowdResidentPresenceLevel
    {
        None = 0,
        Trace = 1,
        Background = 2,
        Visible = 3,
        Pressure = 4
    }

    public enum SpringDay1CrowdDirectorConsumptionRole
    {
        None = 0,
        Trace = 1,
        BackstagePressure = 2,
        Support = 3,
        Priority = 4
    }

    [Flags]
    public enum SpringDay1CrowdResidentBeatFlags
    {
        None = 0,
        AlreadyAround = 1 << 0,
        FirstNotice = 1 << 1,
        SilentWatch = 1 << 2,
        YieldSpace = 1 << 3,
        PretendBusy = 1 << 4,
        AmbientPressure = 1 << 5,
        KeepRoutine = 1 << 6,
        ReturnHome = 1 << 7,
        OffscreenRoutine = 1 << 8
    }

    [CreateAssetMenu(fileName = "SpringDay1NpcCrowdManifest", menuName = "Sunset/Story/Spring Day1 NPC Crowd Manifest", order = 260)]
    public sealed class SpringDay1NpcCrowdManifest : ScriptableObject
    {
        public sealed class BeatConsumptionEntry
        {
            public string npcId = string.Empty;
            public string displayName = string.Empty;
            public string[] semanticAnchorIds = Array.Empty<string>();
            public SpringDay1CrowdResidentPresenceLevel presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
            public SpringDay1CrowdResidentBeatFlags flags = SpringDay1CrowdResidentBeatFlags.None;
            public string note = string.Empty;
        }

        public sealed class BeatConsumptionSnapshot
        {
            public string beatKey = string.Empty;
            public BeatConsumptionEntry[] priority = Array.Empty<BeatConsumptionEntry>();
            public BeatConsumptionEntry[] support = Array.Empty<BeatConsumptionEntry>();
            public BeatConsumptionEntry[] trace = Array.Empty<BeatConsumptionEntry>();
            public BeatConsumptionEntry[] backstagePressure = Array.Empty<BeatConsumptionEntry>();
        }

        [Serializable]
        public sealed class ResidentBeatSemantic
        {
            public string beatKey = string.Empty;
            public SpringDay1CrowdResidentPresenceLevel presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
            public SpringDay1CrowdResidentBeatFlags flags = SpringDay1CrowdResidentBeatFlags.None;
            [TextArea] public string note = string.Empty;
        }

        [Serializable]
        public sealed class Entry
        {
            public string npcId = string.Empty;
            public string displayName = string.Empty;
            [TextArea] public string roleSummary = string.Empty;
            public GameObject prefab;
            [Tooltip("支持用 | 分隔多个候选锚点名，运行时会按顺序查找。")]
            public string anchorObjectName = string.Empty;
            public Vector2 spawnOffset = Vector2.zero;
            public Vector2 fallbackWorldPosition = Vector2.zero;
            public Vector2 initialFacing = Vector2.down;
            public StoryPhase minPhase = StoryPhase.EnterVillage;
            public StoryPhase maxPhase = StoryPhase.DayEnd;
            [Tooltip("导演层 / 群像层语义，不直接等同 runtime phase owner。")]
            public SpringDay1CrowdSceneDuty[] sceneDuties = Array.Empty<SpringDay1CrowdSceneDuty>();
            [Tooltip("稳定语义锚点名，不要求和当前 runtime anchorObjectName 完全一致。")]
            public string[] semanticAnchorIds = Array.Empty<string>();
            public SpringDay1CrowdGrowthIntent growthIntent = SpringDay1CrowdGrowthIntent.HoldAnonymous;
            [Tooltip("这位村民在驻村常驻语义里的默认存在方式。")]
            public SpringDay1CrowdResidentBaseline residentBaseline = SpringDay1CrowdResidentBaseline.PeripheralResident;
            [Tooltip("驻村常驻居民语义矩阵，只描述玩家感受到的在场方式，不直接等同 runtime 部署。")]
            public ResidentBeatSemantic[] residentBeatSemantics = Array.Empty<ResidentBeatSemantic>();

            public bool CanAppear(StoryPhase phase)
            {
                if (phase == StoryPhase.None)
                {
                    return false;
                }

                return phase >= minPhase && phase <= maxPhase;
            }

            public bool SupportsDuty(SpringDay1CrowdSceneDuty duty)
            {
                if (sceneDuties == null)
                {
                    return false;
                }

                for (int index = 0; index < sceneDuties.Length; index++)
                {
                    if (sceneDuties[index] == duty)
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool SupportsSemanticAnchor(string semanticAnchorId)
            {
                if (semanticAnchorIds == null || string.IsNullOrWhiteSpace(semanticAnchorId))
                {
                    return false;
                }

                string trimmed = semanticAnchorId.Trim();
                for (int index = 0; index < semanticAnchorIds.Length; index++)
                {
                    if (string.Equals(semanticAnchorIds[index]?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool TryGetResidentBeatSemantic(string beatKey, out ResidentBeatSemantic semantic)
            {
                semantic = null;
                if (residentBeatSemantics == null || string.IsNullOrWhiteSpace(beatKey))
                {
                    return false;
                }

                string trimmed = beatKey.Trim();
                for (int index = 0; index < residentBeatSemantics.Length; index++)
                {
                    ResidentBeatSemantic candidate = residentBeatSemantics[index];
                    if (candidate == null)
                    {
                        continue;
                    }

                    if (string.Equals(candidate.beatKey?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                    {
                        semantic = candidate;
                        return true;
                    }
                }

                return false;
            }

            public SpringDay1CrowdResidentPresenceLevel GetResidentPresenceLevel(string beatKey)
            {
                return TryGetResidentBeatSemantic(beatKey, out ResidentBeatSemantic semantic)
                    ? semantic.presenceLevel
                    : SpringDay1CrowdResidentPresenceLevel.None;
            }

            public SpringDay1CrowdResidentBeatFlags GetResidentBeatFlags(string beatKey)
            {
                return TryGetResidentBeatSemantic(beatKey, out ResidentBeatSemantic semantic)
                    ? semantic.flags
                    : SpringDay1CrowdResidentBeatFlags.None;
            }

            public bool HasResidentBeatFlags(string beatKey, SpringDay1CrowdResidentBeatFlags flags)
            {
                SpringDay1CrowdResidentBeatFlags actual = GetResidentBeatFlags(beatKey);
                if (actual == SpringDay1CrowdResidentBeatFlags.None || flags == SpringDay1CrowdResidentBeatFlags.None)
                {
                    return false;
                }

                return (actual & flags) == flags;
            }

            public bool IsDirectorPriorityBeat(string beatKey)
            {
                SpringDay1CrowdResidentPresenceLevel presenceLevel = GetResidentPresenceLevel(beatKey);
                return presenceLevel == SpringDay1CrowdResidentPresenceLevel.Visible ||
                       presenceLevel == SpringDay1CrowdResidentPresenceLevel.Pressure;
            }

            public bool IsDirectorSupportBeat(string beatKey)
            {
                return GetResidentPresenceLevel(beatKey) == SpringDay1CrowdResidentPresenceLevel.Background;
            }

            public bool IsDirectorTraceBeat(string beatKey)
            {
                return GetResidentPresenceLevel(beatKey) == SpringDay1CrowdResidentPresenceLevel.Trace;
            }

            public SpringDay1CrowdDirectorConsumptionRole GetDirectorConsumptionRole(string beatKey)
            {
                if (!TryGetResidentBeatSemantic(beatKey, out ResidentBeatSemantic semantic) || semantic == null)
                {
                    return SpringDay1CrowdDirectorConsumptionRole.None;
                }

                SpringDay1CrowdResidentBeatFlags flags = semantic.flags;
                bool hasOnstageSignal = (flags & OnstageResidentFlags) != 0;
                bool hasOffscreenSignal = (flags & OffscreenResidentFlags) != 0;
                bool hasAmbientPressure = (flags & SpringDay1CrowdResidentBeatFlags.AmbientPressure) != 0;

                if (hasAmbientPressure && hasOffscreenSignal && !hasOnstageSignal)
                {
                    return SpringDay1CrowdDirectorConsumptionRole.BackstagePressure;
                }

                if (hasOffscreenSignal && !hasOnstageSignal)
                {
                    return SpringDay1CrowdDirectorConsumptionRole.Trace;
                }

                return semantic.presenceLevel switch
                {
                    SpringDay1CrowdResidentPresenceLevel.Pressure => SpringDay1CrowdDirectorConsumptionRole.Priority,
                    SpringDay1CrowdResidentPresenceLevel.Visible => SpringDay1CrowdDirectorConsumptionRole.Priority,
                    SpringDay1CrowdResidentPresenceLevel.Background => SpringDay1CrowdDirectorConsumptionRole.Support,
                    SpringDay1CrowdResidentPresenceLevel.Trace => SpringDay1CrowdDirectorConsumptionRole.Trace,
                    _ => SpringDay1CrowdDirectorConsumptionRole.None
                };
            }

            public bool IsDirectorBackstagePressureBeat(string beatKey)
            {
                return GetDirectorConsumptionRole(beatKey) == SpringDay1CrowdDirectorConsumptionRole.BackstagePressure;
            }

            private const SpringDay1CrowdResidentBeatFlags OnstageResidentFlags =
                SpringDay1CrowdResidentBeatFlags.AlreadyAround |
                SpringDay1CrowdResidentBeatFlags.FirstNotice |
                SpringDay1CrowdResidentBeatFlags.SilentWatch |
                SpringDay1CrowdResidentBeatFlags.YieldSpace |
                SpringDay1CrowdResidentBeatFlags.PretendBusy;

            private const SpringDay1CrowdResidentBeatFlags OffscreenResidentFlags =
                SpringDay1CrowdResidentBeatFlags.ReturnHome |
                SpringDay1CrowdResidentBeatFlags.OffscreenRoutine;
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        public Entry[] Entries => entries ?? Array.Empty<Entry>();

        public bool TryGetEntry(string npcId, out Entry entry)
        {
            entry = null;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            string trimmed = npcId.Trim();
            Entry[] currentEntries = Entries;
            for (int index = 0; index < currentEntries.Length; index++)
            {
                Entry candidate = currentEntries[index];
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.npcId?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    entry = candidate;
                    return true;
                }
            }

            return false;
        }

        public Entry[] GetEntriesForDirectorConsumptionRole(
            string beatKey,
            SpringDay1CrowdDirectorConsumptionRole role)
        {
            string resolvedBeatKey = ResolveDirectorConsumptionBeatKey(beatKey);
            if (string.IsNullOrWhiteSpace(resolvedBeatKey) || role == SpringDay1CrowdDirectorConsumptionRole.None)
            {
                return Array.Empty<Entry>();
            }

            Entry[] currentEntries = Entries;
            Entry[] matches = new Entry[currentEntries.Length];
            int count = 0;
            for (int index = 0; index < currentEntries.Length; index++)
            {
                Entry candidate = currentEntries[index];
                if (candidate == null || candidate.GetDirectorConsumptionRole(resolvedBeatKey) != role)
                {
                    continue;
                }

                matches[count++] = candidate;
            }

            if (count == 0)
            {
                return Array.Empty<Entry>();
            }

            Entry[] result = new Entry[count];
            Array.Copy(matches, result, count);
            return result;
        }

        public BeatConsumptionSnapshot BuildBeatConsumptionSnapshot(string beatKey)
        {
            string resolvedBeatKey = ResolveDirectorConsumptionBeatKey(beatKey);
            BeatConsumptionSnapshot snapshot = new BeatConsumptionSnapshot
            {
                beatKey = resolvedBeatKey
            };

            if (string.IsNullOrWhiteSpace(resolvedBeatKey))
            {
                return snapshot;
            }

            Entry[] priorityEntries = GetEntriesForDirectorConsumptionRole(resolvedBeatKey, SpringDay1CrowdDirectorConsumptionRole.Priority);
            Entry[] supportEntries = GetEntriesForDirectorConsumptionRole(resolvedBeatKey, SpringDay1CrowdDirectorConsumptionRole.Support);
            Entry[] traceEntries = GetEntriesForDirectorConsumptionRole(resolvedBeatKey, SpringDay1CrowdDirectorConsumptionRole.Trace);
            Entry[] backstageEntries = GetEntriesForDirectorConsumptionRole(resolvedBeatKey, SpringDay1CrowdDirectorConsumptionRole.BackstagePressure);

            snapshot.priority = BuildBeatConsumptionEntries(priorityEntries, resolvedBeatKey);
            snapshot.support = BuildBeatConsumptionEntries(supportEntries, resolvedBeatKey);
            snapshot.trace = BuildBeatConsumptionEntries(traceEntries, resolvedBeatKey);
            snapshot.backstagePressure = BuildBeatConsumptionEntries(backstageEntries, resolvedBeatKey);
            return snapshot;
        }

        private static string ResolveDirectorConsumptionBeatKey(string beatKey)
        {
            string trimmedBeatKey = beatKey?.Trim() ?? string.Empty;
            if (string.Equals(trimmedBeatKey, SpringDay1CrowdResidentBeatKeys.DinnerConflictTable, StringComparison.OrdinalIgnoreCase)
                || string.Equals(trimmedBeatKey, SpringDay1CrowdResidentBeatKeys.ReturnAndReminderWalkBack, StringComparison.OrdinalIgnoreCase))
            {
                return SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry;
            }

            return trimmedBeatKey;
        }

        private static BeatConsumptionEntry[] BuildBeatConsumptionEntries(Entry[] sourceEntries, string beatKey)
        {
            if (sourceEntries == null || sourceEntries.Length == 0)
            {
                return Array.Empty<BeatConsumptionEntry>();
            }

            BeatConsumptionEntry[] entries = new BeatConsumptionEntry[sourceEntries.Length];
            int count = 0;
            for (int index = 0; index < sourceEntries.Length; index++)
            {
                Entry sourceEntry = sourceEntries[index];
                if (sourceEntry == null || !sourceEntry.TryGetResidentBeatSemantic(beatKey, out ResidentBeatSemantic semantic))
                {
                    continue;
                }

                entries[count++] = new BeatConsumptionEntry
                {
                    npcId = sourceEntry.npcId ?? string.Empty,
                    displayName = sourceEntry.displayName ?? string.Empty,
                    semanticAnchorIds = sourceEntry.semanticAnchorIds ?? Array.Empty<string>(),
                    presenceLevel = semantic.presenceLevel,
                    flags = semantic.flags,
                    note = semantic.note ?? string.Empty
                };
            }

            if (count == 0)
            {
                return Array.Empty<BeatConsumptionEntry>();
            }

            BeatConsumptionEntry[] result = new BeatConsumptionEntry[count];
            Array.Copy(entries, result, count);
            return result;
        }

#if UNITY_EDITOR
        public void SetEntries(Entry[] newEntries)
        {
            entries = newEntries ?? Array.Empty<Entry>();
        }
#endif
    }
}
