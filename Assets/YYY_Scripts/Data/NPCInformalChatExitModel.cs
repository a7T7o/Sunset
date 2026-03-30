public enum NPCInformalChatLeaveCause
{
    Any = 0,
    DistanceGraceExceeded = 1,
    BlockingUi = 2,
    DialogueTakeover = 3,
    TargetInvalid = 4,
    PlayerUnavailable = 5,
    ServiceDisabled = 6
}

public enum NPCInformalChatLeavePhase
{
    Any = 0,
    PlayerSpeaking = 1,
    NpcThinking = 2,
    NpcSpeaking = 3,
    BetweenTurns = 4,
    InterruptSequence = 5,
    CompletionHold = 6
}
