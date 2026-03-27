public enum NPCRelationshipStage
{
    Stranger = 0,
    Acquainted = 1,
    Familiar = 2,
    Close = 3
}

public static class NPCRelationshipStageUtility
{
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
}
