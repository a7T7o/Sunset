public enum NPCRelationshipStage
{
    Stranger = 0,
    Acquainted = 1,
    Familiar = 2,
    Close = 3
}

public static class NPCRelationshipStageUtility
{
    public static int ToStoredValue(NPCRelationshipStage stage)
    {
        return (int)Sanitize(stage);
    }

    public static NPCRelationshipStage Sanitize(NPCRelationshipStage stage)
    {
        return stage switch
        {
            NPCRelationshipStage.Acquainted => NPCRelationshipStage.Acquainted,
            NPCRelationshipStage.Familiar => NPCRelationshipStage.Familiar,
            NPCRelationshipStage.Close => NPCRelationshipStage.Close,
            _ => NPCRelationshipStage.Stranger
        };
    }

    public static NPCRelationshipStage FromStoredValue(int rawValue)
    {
        return Sanitize((NPCRelationshipStage)rawValue);
    }

    public static NPCRelationshipStage Shift(NPCRelationshipStage stage, int delta)
    {
        int clamped = UnityEngine.Mathf.Clamp(ToStoredValue(stage) + delta, (int)NPCRelationshipStage.Stranger, (int)NPCRelationshipStage.Close);
        return FromStoredValue(clamped);
    }
}
