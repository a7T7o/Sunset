using System.Collections.Generic;
using UnityEngine;

public static class PlayerNpcRelationshipService
{
    private const string PlayerPrefsKeyPrefix = "Sunset.Npc.RelationshipStage.";
    private const string KnownNpcIdsKey = "Sunset.Npc.RelationshipStage.KnownIds";
    private const char KnownNpcIdsSeparator = '|';
    private static readonly Dictionary<string, NPCRelationshipStage> CachedStages = new Dictionary<string, NPCRelationshipStage>();
    private static readonly HashSet<string> KnownNpcIds = new HashSet<string>();
    private static bool knownNpcIdsLoaded;

    public static NPCRelationshipStage GetStage(string npcId)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        if (string.IsNullOrEmpty(normalizedNpcId))
        {
            return NPCRelationshipStage.Stranger;
        }

        if (CachedStages.TryGetValue(normalizedNpcId, out NPCRelationshipStage cachedStage))
        {
            return cachedStage;
        }

        return NPCRelationshipStage.Stranger;
    }

    public static void SetStage(string npcId, NPCRelationshipStage relationshipStage, bool persist = true)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        if (string.IsNullOrEmpty(normalizedNpcId))
        {
            return;
        }

        NPCRelationshipStage sanitizedStage = NPCRelationshipStageUtility.Sanitize(relationshipStage);
        CachedStages[normalizedNpcId] = sanitizedStage;
        RememberKnownNpcId(normalizedNpcId, persistRegistry: false);
    }

    public static bool PromoteToAtLeast(string npcId, NPCRelationshipStage minimumStage, bool persist = true)
    {
        NPCRelationshipStage sanitizedStage = NPCRelationshipStageUtility.Sanitize(minimumStage);
        if (GetStage(npcId) >= sanitizedStage)
        {
            return false;
        }

        SetStage(npcId, sanitizedStage, persist);
        return true;
    }

    public static NPCRelationshipStage AdjustStage(string npcId, int delta, bool persist = true)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        if (string.IsNullOrEmpty(normalizedNpcId) || delta == 0)
        {
            return GetStage(normalizedNpcId);
        }

        NPCRelationshipStage shiftedStage = ShiftStageWithinBounds(GetStage(normalizedNpcId), delta);
        SetStage(normalizedNpcId, shiftedStage, persist);
        return shiftedStage;
    }

    public static void ResetStage(string npcId, bool deletePersisted = true)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        if (string.IsNullOrEmpty(normalizedNpcId))
        {
            return;
        }

        CachedStages.Remove(normalizedNpcId);
        ForgetKnownNpcId(normalizedNpcId, persistRegistry: false);
    }

    public static IReadOnlyDictionary<string, NPCRelationshipStage> GetSnapshot()
    {
        EnsureKnownNpcIdsLoaded();

        Dictionary<string, NPCRelationshipStage> snapshot = new Dictionary<string, NPCRelationshipStage>();
        foreach (string npcId in KnownNpcIds)
        {
            snapshot[npcId] = GetStage(npcId);
        }

        foreach (KeyValuePair<string, NPCRelationshipStage> pair in CachedStages)
        {
            snapshot[pair.Key] = NPCRelationshipStageUtility.Sanitize(pair.Value);
        }

        return snapshot;
    }

    public static void ReplaceAllStages(IReadOnlyDictionary<string, NPCRelationshipStage> snapshot, bool persist = true)
    {
        EnsureKnownNpcIdsLoaded();

        Dictionary<string, NPCRelationshipStage> normalizedSnapshot = new Dictionary<string, NPCRelationshipStage>();
        if (snapshot != null)
        {
            foreach (KeyValuePair<string, NPCRelationshipStage> pair in snapshot)
            {
                string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(pair.Key);
                if (string.IsNullOrEmpty(normalizedNpcId))
                {
                    continue;
                }

                normalizedSnapshot[normalizedNpcId] = NPCRelationshipStageUtility.Sanitize(pair.Value);
            }
        }

        HashSet<string> staleNpcIds = new HashSet<string>(KnownNpcIds);
        staleNpcIds.ExceptWith(normalizedSnapshot.Keys);

        CachedStages.Clear();
        foreach (KeyValuePair<string, NPCRelationshipStage> pair in normalizedSnapshot)
        {
            CachedStages[pair.Key] = pair.Value;
        }

        KnownNpcIds.Clear();
        foreach (string npcId in normalizedSnapshot.Keys)
        {
            KnownNpcIds.Add(npcId);
        }

    }

    public static string BuildPlayerPrefsKey(string npcId)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        return string.IsNullOrEmpty(normalizedNpcId)
            ? string.Empty
            : PlayerPrefsKeyPrefix + normalizedNpcId;
    }

    private static void EnsureKnownNpcIdsLoaded()
    {
        if (knownNpcIdsLoaded)
        {
            return;
        }

        knownNpcIdsLoaded = true;
    }

    private static void RememberKnownNpcId(string normalizedNpcId, bool persistRegistry)
    {
        if (string.IsNullOrEmpty(normalizedNpcId))
        {
            return;
        }

        EnsureKnownNpcIdsLoaded();
        if (!KnownNpcIds.Add(normalizedNpcId) || !persistRegistry)
        {
            return;
        }

        PersistKnownNpcIds();
    }

    private static void ForgetKnownNpcId(string normalizedNpcId, bool persistRegistry)
    {
        if (string.IsNullOrEmpty(normalizedNpcId))
        {
            return;
        }

        EnsureKnownNpcIdsLoaded();
        if (!KnownNpcIds.Remove(normalizedNpcId) || !persistRegistry)
        {
            return;
        }

        PersistKnownNpcIds();
    }

    private static void PersistKnownNpcIds()
    {
        EnsureKnownNpcIdsLoaded();
    }

    private static NPCRelationshipStage ShiftStageWithinBounds(NPCRelationshipStage stage, int delta)
    {
        int clampedValue = Mathf.Clamp(
            (int)NPCRelationshipStageUtility.Sanitize(stage) + delta,
            (int)NPCRelationshipStage.Stranger,
            (int)NPCRelationshipStage.Close);
        return NPCRelationshipStageUtility.FromStoredValue(clampedValue);
    }
}
