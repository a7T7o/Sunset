using System.Collections.Generic;
using UnityEngine;

public static class PlayerNpcRelationshipService
{
    private const string PlayerPrefsKeyPrefix = "Sunset.Npc.RelationshipStage.";
    private static readonly Dictionary<string, NPCRelationshipStage> CachedStages = new Dictionary<string, NPCRelationshipStage>();

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

        NPCRelationshipStage loadedStage = NPCRelationshipStageUtility.FromStoredValue(
            PlayerPrefs.GetInt(BuildPlayerPrefsKey(normalizedNpcId), (int)NPCRelationshipStage.Stranger));
        CachedStages[normalizedNpcId] = loadedStage;
        return loadedStage;
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

        if (!persist)
        {
            return;
        }

        PlayerPrefs.SetInt(BuildPlayerPrefsKey(normalizedNpcId), (int)sanitizedStage);
        PlayerPrefs.Save();
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
        if (!deletePersisted)
        {
            return;
        }

        PlayerPrefs.DeleteKey(BuildPlayerPrefsKey(normalizedNpcId));
        PlayerPrefs.Save();
    }

    public static string BuildPlayerPrefsKey(string npcId)
    {
        string normalizedNpcId = NPCDialogueContentProfile.NormalizeNpcId(npcId);
        return string.IsNullOrEmpty(normalizedNpcId)
            ? string.Empty
            : PlayerPrefsKeyPrefix + normalizedNpcId;
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
