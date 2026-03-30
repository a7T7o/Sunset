#if UNITY_EDITOR
using Sunset.Story;
using UnityEditor;
using UnityEngine;

public static class NPCInformalChatValidationMenu
{
    public const string ExclusiveValidationLockKey = "Sunset.NpcInformalChatValidation.Active";

    private const string TriggerNearestMenuPath = "Tools/Sunset/NPC/Validation/Trigger Nearest Informal Chat";
    private const string TriggerNpc002MenuPath = "Tools/Sunset/NPC/Validation/Trigger 002 Informal Chat";
    private const string TriggerNpc003MenuPath = "Tools/Sunset/NPC/Validation/Trigger 003 Informal Chat";
    private const string BreakActiveConversationMenuPath = "Tools/Sunset/NPC/Validation/Break Active Informal Chat";
    private const string TraceNpc002ClosureMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Closure";
    private const string TraceNpc003ClosureMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 Informal Chat Closure";
    private const string TraceNpc002InterruptMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Interrupt";
    private const string TraceNpc003InterruptMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 Informal Chat Interrupt";
    private const string TraceNpc002PlayerTypingInterruptMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 PlayerTyping Interrupt";
    private const string TraceNpc003PlayerTypingInterruptMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 PlayerTyping Interrupt";
    private const string TraceNpc002BlockingUiResumeMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 BlockingUi Resume";
    private const string TraceNpc003BlockingUiResumeMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 BlockingUi Resume";
    private const string TraceNpc002DialogueTakeoverResumeMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 DialogueTakeover Resume";
    private const string TraceNpc003DialogueTakeoverResumeMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 DialogueTakeover Resume";
    private const string TraceNpc002TargetInvalidAbortMenuPath = "Tools/Sunset/NPC/Validation/Trace 002 TargetInvalid Abort";
    private const string TraceNpc003TargetInvalidAbortMenuPath = "Tools/Sunset/NPC/Validation/Trace 003 TargetInvalid Abort";
    private const string ForceActiveBlockingUiAbortMenuPath = "Tools/Sunset/NPC/Validation/Force Active BlockingUi Abort";
    private const string ForceActiveDialogueTakeoverAbortMenuPath = "Tools/Sunset/NPC/Validation/Force Active DialogueTakeover Abort";
    private const string ForceActiveTargetInvalidAbortMenuPath = "Tools/Sunset/NPC/Validation/Force Active TargetInvalid Abort";
    private const string NavigationValidationPendingActionKey = "Sunset.NavigationLiveValidation.PendingAction";
    private const double FirstExchangeTimeoutSeconds = 6.0d;
    private const double SecondExchangeTimeoutSeconds = 6.0d;
    private const double InterruptTimeoutSeconds = 5.5d;
    private const double ResumeCompletionSettleTimeoutSeconds = 2.0d;

    private enum ValidationTracePhase
    {
        Idle = 0,
        WaitingFirstExchange = 1,
        WaitingSecondExchange = 2,
        WaitingInterrupt = 3,
        WaitingAbortSettle = 4,
        WaitingResumeIntro = 5,
        WaitingResumeContinuation = 6
    }

    private static ValidationTracePhase s_tracePhase;
    private static string s_traceTargetName = string.Empty;
    private static double s_tracePhaseStartedAt;
    private static string s_firstPlayerLine = string.Empty;
    private static string s_firstNpcLine = string.Empty;
    private static string s_secondPlayerLine = string.Empty;
    private static string s_secondNpcLine = string.Empty;
    private static bool s_interruptPlayerLineSeen;
    private static bool s_interruptNpcLineSeen;
    private static bool s_traceInterruptOnly;
    private static bool s_tracePlayerTypingInterrupt;
    private static bool s_traceResumeAfterAbort;
    private static NPCInformalChatLeaveCause s_traceForcedAbortCause;
    private static string s_resumePlayerLine = string.Empty;
    private static string s_resumeNpcLine = string.Empty;
    private static bool s_resumeTargetExchangeSeen;

    [MenuItem(TriggerNearestMenuPath)]
    private static void TriggerNearestInformalChat()
    {
        if (!TryGetPlayerContext(out PlayerMovement playerMovement, out PlayerNpcChatSessionService sessionService))
        {
            return;
        }

        InteractionContext context = BuildContext(playerMovement);
        NPCInformalChatInteractable candidate = FindBestInteractable(context, sessionService);
        if (candidate == null)
        {
            Debug.LogWarning("[NPCValidation] 当前没有可触发的 NPC 非正式聊天对象。");
            return;
        }

        TriggerConversation(candidate, playerMovement, "nearest");
    }

    [MenuItem(TriggerNpc002MenuPath)]
    private static void TriggerNpc002InformalChat()
    {
        TriggerConversationByName("002");
    }

    [MenuItem(TriggerNpc003MenuPath)]
    private static void TriggerNpc003InformalChat()
    {
        TriggerConversationByName("003");
    }

    [MenuItem(BreakActiveConversationMenuPath)]
    private static void BreakActiveInformalChat()
    {
        if (!TryGetPlayerContext(out PlayerMovement playerMovement, out PlayerNpcChatSessionService sessionService))
        {
            return;
        }

        NPCInformalChatInteractable candidate = FindActiveInteractable(sessionService);
        if (candidate == null)
        {
            Debug.LogWarning("[NPCValidation] 当前没有正在进行中的 NPC 非正式聊天。");
            return;
        }

        ClearNavigationValidationInterference(playerMovement);
        MovePlayerSamplePoint(playerMovement, candidate, candidate.SessionBreakDistance + 0.42f);
        float boundaryDistance = candidate.GetBoundaryDistance(ResolvePlayerSamplePoint(playerMovement));
        Debug.Log($"[NPCValidation] 已将玩家移出 {candidate.name} 的闲聊范围，boundaryDistance={boundaryDistance:F3}");
    }

    [MenuItem(TraceNpc002ClosureMenuPath)]
    private static void TraceNpc002Closure()
    {
        StartValidationTrace("002", interruptOnly: false);
    }

    [MenuItem(TraceNpc003ClosureMenuPath)]
    private static void TraceNpc003Closure()
    {
        StartValidationTrace("003", interruptOnly: false);
    }

    [MenuItem(TraceNpc002InterruptMenuPath)]
    private static void TraceNpc002Interrupt()
    {
        StartValidationTrace("002", interruptOnly: true);
    }

    [MenuItem(TraceNpc003InterruptMenuPath)]
    private static void TraceNpc003Interrupt()
    {
        StartValidationTrace("003", interruptOnly: true);
    }

    [MenuItem(TraceNpc002PlayerTypingInterruptMenuPath)]
    private static void TraceNpc002PlayerTypingInterrupt()
    {
        StartValidationTrace("002", interruptOnly: true, playerTypingInterrupt: true);
    }

    [MenuItem(TraceNpc003PlayerTypingInterruptMenuPath)]
    private static void TraceNpc003PlayerTypingInterrupt()
    {
        StartValidationTrace("003", interruptOnly: true, playerTypingInterrupt: true);
    }

    [MenuItem(TraceNpc002BlockingUiResumeMenuPath)]
    private static void TraceNpc002BlockingUiResume()
    {
        StartValidationTrace("002", forcedAbortCause: NPCInformalChatLeaveCause.BlockingUi, resumeAfterAbort: true);
    }

    [MenuItem(TraceNpc003BlockingUiResumeMenuPath)]
    private static void TraceNpc003BlockingUiResume()
    {
        StartValidationTrace("003", forcedAbortCause: NPCInformalChatLeaveCause.BlockingUi, resumeAfterAbort: true);
    }

    [MenuItem(TraceNpc002DialogueTakeoverResumeMenuPath)]
    private static void TraceNpc002DialogueTakeoverResume()
    {
        StartValidationTrace("002", forcedAbortCause: NPCInformalChatLeaveCause.DialogueTakeover, resumeAfterAbort: true);
    }

    [MenuItem(TraceNpc003DialogueTakeoverResumeMenuPath)]
    private static void TraceNpc003DialogueTakeoverResume()
    {
        StartValidationTrace("003", forcedAbortCause: NPCInformalChatLeaveCause.DialogueTakeover, resumeAfterAbort: true);
    }

    [MenuItem(TraceNpc002TargetInvalidAbortMenuPath)]
    private static void TraceNpc002TargetInvalidAbort()
    {
        StartValidationTrace("002", forcedAbortCause: NPCInformalChatLeaveCause.TargetInvalid);
    }

    [MenuItem(TraceNpc003TargetInvalidAbortMenuPath)]
    private static void TraceNpc003TargetInvalidAbort()
    {
        StartValidationTrace("003", forcedAbortCause: NPCInformalChatLeaveCause.TargetInvalid);
    }

    [MenuItem(ForceActiveBlockingUiAbortMenuPath)]
    private static void ForceActiveBlockingUiAbort()
    {
        ForceActiveAbort(NPCInformalChatLeaveCause.BlockingUi);
    }

    [MenuItem(ForceActiveDialogueTakeoverAbortMenuPath)]
    private static void ForceActiveDialogueTakeoverAbort()
    {
        ForceActiveAbort(NPCInformalChatLeaveCause.DialogueTakeover);
    }

    [MenuItem(ForceActiveTargetInvalidAbortMenuPath)]
    private static void ForceActiveTargetInvalidAbort()
    {
        ForceActiveAbort(NPCInformalChatLeaveCause.TargetInvalid);
    }

    [MenuItem(TriggerNearestMenuPath, true)]
    [MenuItem(TriggerNpc002MenuPath, true)]
    [MenuItem(TriggerNpc003MenuPath, true)]
    [MenuItem(BreakActiveConversationMenuPath, true)]
    [MenuItem(TraceNpc002ClosureMenuPath, true)]
    [MenuItem(TraceNpc003ClosureMenuPath, true)]
    [MenuItem(TraceNpc002InterruptMenuPath, true)]
    [MenuItem(TraceNpc003InterruptMenuPath, true)]
    [MenuItem(TraceNpc002PlayerTypingInterruptMenuPath, true)]
    [MenuItem(TraceNpc003PlayerTypingInterruptMenuPath, true)]
    [MenuItem(TraceNpc002BlockingUiResumeMenuPath, true)]
    [MenuItem(TraceNpc003BlockingUiResumeMenuPath, true)]
    [MenuItem(TraceNpc002DialogueTakeoverResumeMenuPath, true)]
    [MenuItem(TraceNpc003DialogueTakeoverResumeMenuPath, true)]
    [MenuItem(TraceNpc002TargetInvalidAbortMenuPath, true)]
    [MenuItem(TraceNpc003TargetInvalidAbortMenuPath, true)]
    [MenuItem(ForceActiveBlockingUiAbortMenuPath, true)]
    [MenuItem(ForceActiveDialogueTakeoverAbortMenuPath, true)]
    [MenuItem(ForceActiveTargetInvalidAbortMenuPath, true)]
    private static bool ValidatePlayOnlyMenu()
    {
        return EditorApplication.isPlaying;
    }

    private static void StartValidationTrace(
        string targetName,
        bool interruptOnly = false,
        bool playerTypingInterrupt = false,
        NPCInformalChatLeaveCause forcedAbortCause = NPCInformalChatLeaveCause.Any,
        bool resumeAfterAbort = false)
    {
        StopValidationTrace();

        if (!TryGetPlayerContext(out PlayerMovement playerMovement, out _))
        {
            return;
        }

        NPCInformalChatInteractable candidate = FindInteractableByName(targetName);
        if (candidate == null)
        {
            Debug.LogWarning($"[NPCValidation] 未找到名为 {targetName} 的 NPCInformalChatInteractable，无法执行闭环 trace。");
            return;
        }

        s_traceTargetName = targetName;
        s_traceInterruptOnly = interruptOnly;
        EditorPrefs.SetBool(ExclusiveValidationLockKey, true);
        s_tracePhase = ValidationTracePhase.WaitingFirstExchange;
        s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
        s_firstPlayerLine = string.Empty;
        s_firstNpcLine = string.Empty;
        s_secondPlayerLine = string.Empty;
        s_secondNpcLine = string.Empty;
        s_interruptPlayerLineSeen = false;
        s_interruptNpcLineSeen = false;
        s_tracePlayerTypingInterrupt = playerTypingInterrupt;
        s_traceResumeAfterAbort = resumeAfterAbort;
        s_traceForcedAbortCause = forcedAbortCause;
        s_resumePlayerLine = string.Empty;
        s_resumeNpcLine = string.Empty;
        s_resumeTargetExchangeSeen = false;

        TriggerConversation(candidate, playerMovement, $"trace-{targetName}-start");
        TryCaptureImmediatePlayerTypingInterrupt(playerMovement);
        EditorApplication.update += TickValidationTrace;
        string traceLabel = playerTypingInterrupt
            ? "玩家首句打字中断"
            : (resumeAfterAbort
                ? $"{forcedAbortCause} 续聊"
                : (forcedAbortCause != NPCInformalChatLeaveCause.Any
                    ? $"{forcedAbortCause} 中断"
                    : (interruptOnly ? "中断" : "闭环")));
        Debug.Log($"[NPCValidation] 已开始 {targetName} 的{traceLabel} trace。");
    }

    private static void StopValidationTrace()
    {
        EditorApplication.update -= TickValidationTrace;
        if (EditorPrefs.HasKey(ExclusiveValidationLockKey))
        {
            EditorPrefs.DeleteKey(ExclusiveValidationLockKey);
        }

        s_tracePhase = ValidationTracePhase.Idle;
        s_traceTargetName = string.Empty;
        s_tracePhaseStartedAt = 0d;
        s_firstPlayerLine = string.Empty;
        s_firstNpcLine = string.Empty;
        s_secondPlayerLine = string.Empty;
        s_secondNpcLine = string.Empty;
        s_interruptPlayerLineSeen = false;
        s_interruptNpcLineSeen = false;
        s_traceInterruptOnly = false;
        s_tracePlayerTypingInterrupt = false;
        s_traceResumeAfterAbort = false;
        s_traceForcedAbortCause = NPCInformalChatLeaveCause.Any;
        s_resumePlayerLine = string.Empty;
        s_resumeNpcLine = string.Empty;
        s_resumeTargetExchangeSeen = false;
    }

    private static void TickValidationTrace()
    {
        if (s_tracePhase == ValidationTracePhase.Idle)
        {
            StopValidationTrace();
            return;
        }

        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning($"[NPCValidation] {s_traceTargetName} 闭环 trace 中断：Unity 已退出 Play。");
            StopValidationTrace();
            return;
        }

        ClearStoryDialogueInterference();

        if (!TryGetPlayerContext(out PlayerMovement playerMovement, out PlayerNpcChatSessionService sessionService))
        {
            Debug.LogWarning($"[NPCValidation] {s_traceTargetName} 闭环 trace 中断：未找到玩家上下文。");
            StopValidationTrace();
            return;
        }

        NPCInformalChatInteractable candidate = FindInteractableByName(s_traceTargetName);
        if (candidate == null)
        {
            Debug.LogWarning($"[NPCValidation] {s_traceTargetName} 闭环 trace 中断：未找到目标 NPC。");
            StopValidationTrace();
            return;
        }

        PlayerThoughtBubblePresenter playerBubble = playerMovement.GetComponent<PlayerThoughtBubblePresenter>();
        NPCBubblePresenter npcBubble = candidate.BubblePresenter;
        string playerText = playerBubble != null ? playerBubble.CurrentBubbleText : string.Empty;
        string npcText = npcBubble != null ? npcBubble.CurrentBubbleText : string.Empty;
        bool sessionActive = sessionService != null && sessionService.HasActiveConversation;
        string sessionState = sessionService != null ? sessionService.DebugStateName : "MissingService";

        switch (s_tracePhase)
        {
            case ValidationTracePhase.WaitingFirstExchange:
                if (s_tracePlayerTypingInterrupt)
                {
                    if (!sessionActive)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 玩家首句打字中断 trace 失败：会话过早结束。 " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null && sessionService.CompletedExchangeCount >= 1)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 玩家首句打字中断 trace 失败：第一轮已结束，错过 PlayerTyping 窗口。 " +
                            $"player=\"{sessionService.LastCompletedPlayerLine}\" | npc=\"{sessionService.LastCompletedNpcLine}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null &&
                        sessionService.DebugStateName == "PlayerTyping" &&
                        !string.IsNullOrWhiteSpace(playerText))
                    {
                        if (sessionService.TryStartWalkAwayInterruptForValidation())
                        {
                            Debug.Log(
                                $"[NPCValidation] {s_traceTargetName} 已在玩家首句打字阶段直接触发跑开中断，playerText=\"{playerText}\"");
                            s_tracePhase = ValidationTracePhase.WaitingInterrupt;
                            s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                        }

                        return;
                    }

                    if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > FirstExchangeTimeoutSeconds)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 玩家首句打字中断 trace 超时：未在 {FirstExchangeTimeoutSeconds:F1} 秒内捕捉到 PlayerTyping 窗口。 " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                    }

                    return;
                }

                if (sessionService != null && sessionService.CompletedExchangeCount >= 1)
                {
                    s_firstPlayerLine = sessionService.LastCompletedPlayerLine;
                    s_firstNpcLine = sessionService.LastCompletedNpcLine;
                    Debug.Log($"[NPCValidation] {s_traceTargetName} 首轮完成 | 玩家=\"{s_firstPlayerLine}\" | NPC=\"{s_firstNpcLine}\"");
                    Debug.Log($"[NPCValidation] {s_traceTargetName} 已进入自动续聊观察。");
                    s_tracePhase = ValidationTracePhase.WaitingSecondExchange;
                    s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > FirstExchangeTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 闭环 trace 超时：首轮未在 {FirstExchangeTimeoutSeconds:F1} 秒内完成。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;

            case ValidationTracePhase.WaitingSecondExchange:
                if (s_traceResumeAfterAbort)
                {
                    if (!sessionActive)
                    {
                        string endReason = sessionService != null ? sessionService.LastConversationEndReasonName : "MissingService";
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 续聊 trace 失败：强制中断前会话已结束。 " +
                            $"endReason={endReason} | state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null &&
                        sessionService.CompletedExchangeCount == 1 &&
                        sessionService.TryForceAbortForValidation(s_traceForcedAbortCause))
                    {
                        Debug.Log(
                            $"[NPCValidation] {s_traceTargetName} 已执行续聊前强制中断，abortCause={s_traceForcedAbortCause} | " +
                            $"leavePhase={sessionService.LastLeavePhaseName} | playerExit=\"{sessionService.LastInterruptedPlayerLine}\" | " +
                            $"npcReaction=\"{sessionService.LastInterruptedNpcLine}\"");
                        s_tracePhase = ValidationTracePhase.WaitingAbortSettle;
                        s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                        return;
                    }

                    if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > SecondExchangeTimeoutSeconds)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 续聊 trace 超时：未在第一轮完成后进入可强制中断窗口。 " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                    }

                    return;
                }

                if (s_traceForcedAbortCause != NPCInformalChatLeaveCause.Any)
                {
                    if (!sessionActive)
                    {
                        string endReason = sessionService != null ? sessionService.LastConversationEndReasonName : "MissingService";
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} {s_traceForcedAbortCause} trace 失败：强制中断前会话已结束。 " +
                            $"endReason={endReason} | state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null &&
                        sessionService.CompletedExchangeCount == 1 &&
                        sessionService.TryForceAbortForValidation(s_traceForcedAbortCause))
                    {
                        Debug.Log(
                            $"[NPCValidation] {s_traceTargetName} 已执行强制中断，abortCause={s_traceForcedAbortCause} | " +
                            $"leavePhase={sessionService.LastLeavePhaseName} | playerExit=\"{sessionService.LastInterruptedPlayerLine}\" | " +
                            $"npcReaction=\"{sessionService.LastInterruptedNpcLine}\"");
                        s_tracePhase = ValidationTracePhase.WaitingAbortSettle;
                        s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                        return;
                    }

                    if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > SecondExchangeTimeoutSeconds)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} {s_traceForcedAbortCause} trace 超时：未在第一轮完成后进入强制中断窗口。 " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                    }

                    return;
                }

                if (s_traceInterruptOnly)
                {
                    if (!sessionActive)
                    {
                        string endReason = sessionService != null ? sessionService.LastConversationEndReasonName : "MissingService";
                        string abortCause = sessionService != null ? sessionService.LastAbortCauseName : "MissingService";
                        string leavePhase = sessionService != null ? sessionService.LastLeavePhaseName : "MissingService";
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 中断 trace 失败：第二轮中途跑开前会话已结束。 " +
                            $"endReason={endReason} | abortCause={abortCause} | leavePhase={leavePhase} | " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null && sessionService.CompletedExchangeCount >= 2)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 中断 trace 失败：第二轮已完整结束，未能在中途切入。 " +
                            $"player=\"{sessionService.LastCompletedPlayerLine}\" | npc=\"{sessionService.LastCompletedNpcLine}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService != null &&
                        sessionService.CompletedExchangeCount == 1 &&
                        sessionService.IsWaitingNpcReply)
                    {
                        if (sessionService.TryStartWalkAwayInterruptForValidation())
                        {
                            Debug.Log(
                                $"[NPCValidation] {s_traceTargetName} 已在第二轮等待回复阶段直接触发跑开中断，state={sessionState}");
                            s_tracePhase = ValidationTracePhase.WaitingInterrupt;
                            s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                        }

                        return;
                    }

                    if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > SecondExchangeTimeoutSeconds)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 中断 trace 超时：未在第二轮中途捕捉到可跑开窗口。 " +
                            $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                        StopValidationTrace();
                    }

                    return;
                }

                if (sessionService != null && sessionService.CompletedExchangeCount >= 2)
                {
                    s_secondPlayerLine = sessionService.LastCompletedPlayerLine;
                    s_secondNpcLine = sessionService.LastCompletedNpcLine;
                    Debug.Log($"[NPCValidation] {s_traceTargetName} 第二轮完成 | 玩家=\"{s_secondPlayerLine}\" | NPC=\"{s_secondNpcLine}\"");
                    ClearNavigationValidationInterference(playerMovement);
                    MovePlayerSamplePoint(playerMovement, candidate, candidate.SessionBreakDistance + 0.42f);
                    float boundaryDistance = candidate.GetBoundaryDistance(ResolvePlayerSamplePoint(playerMovement));
                    Debug.Log($"[NPCValidation] {s_traceTargetName} 已发起跑开中断，boundaryDistance={boundaryDistance:F3}");
                    s_tracePhase = ValidationTracePhase.WaitingInterrupt;
                    s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                    return;
                }

                if (!sessionActive)
                {
                    string endReason = sessionService != null ? sessionService.LastConversationEndReasonName : "MissingService";
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 闭环 trace 失败：第二轮出现前会话已结束。 " +
                        $"endReason={endReason} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > SecondExchangeTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 闭环 trace 超时：第二轮未在 {SecondExchangeTimeoutSeconds:F1} 秒内完成。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;

            case ValidationTracePhase.WaitingInterrupt:
                if (!sessionActive)
                {
                    if (sessionService != null)
                    {
                        s_interruptPlayerLineSeen = !string.IsNullOrWhiteSpace(sessionService.LastInterruptedPlayerLine);
                        s_interruptNpcLineSeen = !string.IsNullOrWhiteSpace(sessionService.LastInterruptedNpcLine);
                    }

                    string endReason = sessionService?.LastConversationEndReasonName ?? "MissingService";
                    string summaryPrefix =
                        endReason == "WalkAwayInterrupt"
                            ? $"{s_traceTargetName} 跑开中断完成"
                            : $"{s_traceTargetName} 闭环收尾完成";
                    Debug.Log(
                        $"[NPCValidation] {summaryPrefix} | " +
                        $"playerExitSeen={s_interruptPlayerLineSeen} | npcReactionSeen={s_interruptNpcLineSeen} | " +
                        $"playerExit=\"{sessionService?.LastInterruptedPlayerLine ?? string.Empty}\" | " +
                        $"npcReaction=\"{sessionService?.LastInterruptedNpcLine ?? string.Empty}\" | " +
                        $"endReason={endReason} | " +
                        $"abortCause={sessionService?.LastAbortCauseName ?? "MissingService"} | " +
                        $"leavePhase={sessionService?.LastLeavePhaseName ?? "MissingService"}");
                    StopValidationTrace();
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > InterruptTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 闭环 trace 超时：跑开中断未在 {InterruptTimeoutSeconds:F1} 秒内收尾。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;

            case ValidationTracePhase.WaitingAbortSettle:
                if (!sessionActive)
                {
                    if (s_traceForcedAbortCause != NPCInformalChatLeaveCause.Any && !s_traceResumeAfterAbort)
                    {
                        Debug.Log(
                            $"[NPCValidation] {s_traceTargetName} 强制中断完成 | " +
                            $"abortCause={sessionService?.LastAbortCauseName ?? "MissingService"} | " +
                            $"endReason={sessionService?.LastConversationEndReasonName ?? "MissingService"} | " +
                            $"leavePhase={sessionService?.LastLeavePhaseName ?? "MissingService"} | " +
                            $"playerExit=\"{sessionService?.LastInterruptedPlayerLine ?? string.Empty}\" | " +
                            $"npcReaction=\"{sessionService?.LastInterruptedNpcLine ?? string.Empty}\"");
                        StopValidationTrace();
                        return;
                    }

                    if (sessionService == null ||
                        !sessionService.HasPendingResumeConversation ||
                        sessionService.PendingResumeNpcId != s_traceTargetName)
                    {
                        Debug.LogWarning(
                            $"[NPCValidation] {s_traceTargetName} 续聊 trace 失败：中断后未留下有效续接快照。 " +
                            $"pendingNpc=\"{sessionService?.PendingResumeNpcId ?? string.Empty}\" | " +
                            $"pendingAbortCause={sessionService?.PendingResumeAbortCauseName ?? "MissingService"} | " +
                            $"pendingLeavePhase={sessionService?.PendingResumeLeavePhaseName ?? "MissingService"}");
                        StopValidationTrace();
                        return;
                    }

                    Debug.Log(
                        $"[NPCValidation] {s_traceTargetName} 已留下续接快照 | " +
                        $"pendingNpc=\"{sessionService.PendingResumeNpcId}\" | " +
                        $"pendingExchangeIndex={sessionService.PendingResumeExchangeIndex} | " +
                        $"pendingAbortCause={sessionService.PendingResumeAbortCauseName} | " +
                        $"pendingLeavePhase={sessionService.PendingResumeLeavePhaseName} | " +
                        $"resumeOutcome={sessionService.LastResumeOutcomeName}");
                    TriggerConversation(candidate, playerMovement, $"resume-{s_traceTargetName}");
                    s_tracePhase = ValidationTracePhase.WaitingResumeIntro;
                    s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > InterruptTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 续聊 trace 超时：强制中断后会话未在 {InterruptTimeoutSeconds:F1} 秒内结束。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;

            case ValidationTracePhase.WaitingResumeIntro:
                if (!sessionActive)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 续聊 trace 失败：重新触发后会话过早结束。 " +
                        $"playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                    return;
                }

                if (sessionService != null &&
                    (!string.IsNullOrWhiteSpace(sessionService.LastResumePlayerLine) ||
                     !string.IsNullOrWhiteSpace(sessionService.LastResumeNpcLine)))
                {
                    s_resumePlayerLine = sessionService.LastResumePlayerLine;
                    s_resumeNpcLine = sessionService.LastResumeNpcLine;
                    Debug.Log(
                        $"[NPCValidation] {s_traceTargetName} 已进入续聊补口 | " +
                        $"resumePlayer=\"{s_resumePlayerLine}\" | resumeNpc=\"{s_resumeNpcLine}\" | " +
                        $"resumeOutcome={sessionService.LastResumeOutcomeName}");
                    s_tracePhase = ValidationTracePhase.WaitingResumeContinuation;
                    s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > FirstExchangeTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 续聊 trace 超时：未在 {FirstExchangeTimeoutSeconds:F1} 秒内捕捉到续聊补口。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;

            case ValidationTracePhase.WaitingResumeContinuation:
                if (sessionService != null && sessionService.CompletedExchangeCount >= 2)
                {
                    s_secondPlayerLine = sessionService.LastCompletedPlayerLine;
                    s_secondNpcLine = sessionService.LastCompletedNpcLine;

                    if (!s_resumeTargetExchangeSeen)
                    {
                        s_resumeTargetExchangeSeen = true;
                        s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
                    }

                    bool resumeCompletionSettled =
                        !sessionActive ||
                        sessionService.LastConversationEndReason != PlayerNpcChatSessionService.ConversationEndReason.None;

                    if (resumeCompletionSettled)
                    {
                        Debug.Log(
                            $"[NPCValidation] {s_traceTargetName} 续聊完成 | " +
                            $"resumePlayer=\"{s_resumePlayerLine}\" | resumeNpc=\"{s_resumeNpcLine}\" | " +
                            $"player=\"{s_secondPlayerLine}\" | npc=\"{s_secondNpcLine}\" | " +
                            $"endReason={sessionService.LastConversationEndReasonName} | " +
                            $"resumeOutcome={sessionService.LastResumeOutcomeName}");
                        StopValidationTrace();
                        return;
                    }
                }

                if (s_resumeTargetExchangeSeen &&
                    EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > ResumeCompletionSettleTimeoutSeconds)
                {
                    Debug.Log(
                        $"[NPCValidation] {s_traceTargetName} 续聊完成 | " +
                        $"resumePlayer=\"{s_resumePlayerLine}\" | resumeNpc=\"{s_resumeNpcLine}\" | " +
                        $"player=\"{s_secondPlayerLine}\" | npc=\"{s_secondNpcLine}\" | " +
                        $"endReason={sessionService?.LastConversationEndReasonName ?? "None"} | " +
                        $"resumeOutcome={sessionService?.LastResumeOutcomeName ?? "None"} | " +
                        $"completionSettle=Timeout");
                    StopValidationTrace();
                    return;
                }

                if (!sessionActive)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 续聊 trace 失败：续聊补口后未完成目标轮次就结束。 " +
                        $"endReason={sessionService?.LastConversationEndReasonName ?? "MissingService"} | " +
                        $"player=\"{sessionService?.LastCompletedPlayerLine ?? string.Empty}\" | " +
                        $"npc=\"{sessionService?.LastCompletedNpcLine ?? string.Empty}\"");
                    StopValidationTrace();
                    return;
                }

                if (EditorApplication.timeSinceStartup - s_tracePhaseStartedAt > SecondExchangeTimeoutSeconds)
                {
                    Debug.LogWarning(
                        $"[NPCValidation] {s_traceTargetName} 续聊 trace 超时：补口后目标轮次未在 {SecondExchangeTimeoutSeconds:F1} 秒内完成。 " +
                        $"state={sessionState} | playerText=\"{playerText}\" | npcText=\"{npcText}\"");
                    StopValidationTrace();
                }
                return;
        }
    }

    private static void TryCaptureImmediatePlayerTypingInterrupt(PlayerMovement playerMovement)
    {
        if (!s_tracePlayerTypingInterrupt || playerMovement == null)
        {
            return;
        }

        PlayerNpcChatSessionService sessionService =
            PlayerNpcChatSessionService.ResolveForPlayer(playerMovement.gameObject);
        if (sessionService == null ||
            sessionService.DebugStateName != "PlayerTyping" ||
            !sessionService.TryStartWalkAwayInterruptForValidation())
        {
            return;
        }

        Debug.Log(
            $"[NPCValidation] {s_traceTargetName} 已在触发后的即时 PlayerTyping 窗口切入跑开中断，" +
            $"playerText=\"{sessionService.CurrentPlayerBubbleText}\"");
        s_tracePhase = ValidationTracePhase.WaitingInterrupt;
        s_tracePhaseStartedAt = EditorApplication.timeSinceStartup;
    }

    private static void ForceActiveAbort(NPCInformalChatLeaveCause abortCause)
    {
        if (!TryGetPlayerContext(out _, out PlayerNpcChatSessionService sessionService) || sessionService == null)
        {
            return;
        }

        if (!sessionService.TryForceAbortForValidation(abortCause))
        {
            Debug.LogWarning($"[NPCValidation] 当前没有可强制收尾的 NPC 非正式聊天，abortCause={abortCause}");
            return;
        }

        Debug.Log(
            $"[NPCValidation] 已强制执行 active abort | abortCause={abortCause} | " +
            $"endReason={sessionService.LastConversationEndReasonName} | " +
            $"leavePhase={sessionService.LastLeavePhaseName} | " +
            $"playerExit=\"{sessionService.LastInterruptedPlayerLine}\" | " +
            $"npcReaction=\"{sessionService.LastInterruptedNpcLine}\"");
    }

    private static void TriggerConversationByName(string targetName)
    {
        if (!TryGetPlayerContext(out PlayerMovement playerMovement, out _))
        {
            return;
        }

        NPCInformalChatInteractable candidate = FindInteractableByName(targetName);
        if (candidate == null)
        {
            Debug.LogWarning($"[NPCValidation] 未找到名为 {targetName} 的 NPCInformalChatInteractable。");
            return;
        }

        TriggerConversation(candidate, playerMovement, targetName);
    }

    private static void TriggerConversation(
        NPCInformalChatInteractable candidate,
        PlayerMovement playerMovement,
        string sourceTag)
    {
        if (candidate == null || playerMovement == null)
        {
            return;
        }

        ClearNavigationValidationInterference(playerMovement);
        ClearStoryDialogueInterference();
        MovePlayerSamplePoint(playerMovement, candidate, Mathf.Min(candidate.InteractionDistance * 0.45f, 0.4f));
        InteractionContext context = BuildContext(playerMovement);
        bool conversationAlreadyActive =
            PlayerNpcChatSessionService.Instance != null &&
            PlayerNpcChatSessionService.Instance.IsConversationActiveWith(candidate);
        if (!conversationAlreadyActive && !candidate.CanInteract(context))
        {
            bool dialogueActive = DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive;
            Debug.LogWarning(
                $"[NPCValidation] {candidate.name} 当前无法开始非正式聊天。 " +
                $"dialogueActive={dialogueActive} | boundaryDistance={candidate.GetBoundaryDistance(context.PlayerPosition):F3}");
            return;
        }

        float distance = candidate.GetBoundaryDistance(context.PlayerPosition);
        bool handled = candidate.TryHandleInteract(context);
        if (!handled && !conversationAlreadyActive)
        {
            Debug.LogWarning(
                $"[NPCValidation] {candidate.name} 未接住非正式聊天触发。 " +
                $"source={sourceTag} | boundaryDistance={distance:F3}");
            return;
        }

        Debug.Log(
            $"[NPCValidation] 已触发 {candidate.name} 的非正式聊天，source={sourceTag}, boundaryDistance={distance:F3}, " +
            $"alreadyActive={conversationAlreadyActive}");
    }

    private static bool TryGetPlayerContext(
        out PlayerMovement playerMovement,
        out PlayerNpcChatSessionService sessionService)
    {
        playerMovement = null;
        sessionService = null;

        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[NPCValidation] 仅可在 Play Mode 中触发 NPC 非正式聊天。");
            return false;
        }

        playerMovement = Object.FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include);
        if (playerMovement == null)
        {
            Debug.LogWarning("[NPCValidation] 未找到 PlayerMovement，无法构造交互上下文。");
            return false;
        }

        sessionService = PlayerNpcChatSessionService.ResolveForPlayer(playerMovement.gameObject);
        return true;
    }

    private static NPCInformalChatInteractable FindBestInteractable(
        InteractionContext context,
        PlayerNpcChatSessionService sessionService)
    {
        NPCInformalChatInteractable[] interactables =
            Object.FindObjectsByType<NPCInformalChatInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        NPCInformalChatInteractable activeCandidate = null;
        NPCInformalChatInteractable nearestCandidate = null;
        float nearestDistance = float.PositiveInfinity;

        for (int index = 0; index < interactables.Length; index++)
        {
            NPCInformalChatInteractable interactable = interactables[index];
            if (interactable == null || !interactable.isActiveAndEnabled)
            {
                continue;
            }

            if (sessionService != null && sessionService.IsConversationActiveWith(interactable))
            {
                activeCandidate = interactable;
                break;
            }

            if (!interactable.CanInteract(context))
            {
                continue;
            }

            float boundaryDistance = interactable.GetBoundaryDistance(context.PlayerPosition);
            if (boundaryDistance > interactable.InteractionDistance || boundaryDistance >= nearestDistance)
            {
                continue;
            }

            nearestDistance = boundaryDistance;
            nearestCandidate = interactable;
        }

        return activeCandidate != null ? activeCandidate : nearestCandidate;
    }

    private static NPCInformalChatInteractable FindActiveInteractable(PlayerNpcChatSessionService sessionService)
    {
        if (sessionService == null)
        {
            return null;
        }

        NPCInformalChatInteractable[] interactables =
            Object.FindObjectsByType<NPCInformalChatInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int index = 0; index < interactables.Length; index++)
        {
            NPCInformalChatInteractable interactable = interactables[index];
            if (interactable != null && sessionService.IsConversationActiveWith(interactable))
            {
                return interactable;
            }
        }

        return null;
    }

    private static NPCInformalChatInteractable FindInteractableByName(string targetName)
    {
        NPCInformalChatInteractable[] interactables =
            Object.FindObjectsByType<NPCInformalChatInteractable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int index = 0; index < interactables.Length; index++)
        {
            NPCInformalChatInteractable interactable = interactables[index];
            if (interactable == null)
            {
                continue;
            }

            if (string.Equals(interactable.name, targetName, System.StringComparison.OrdinalIgnoreCase))
            {
                return interactable;
            }
        }

        return null;
    }

    private static InteractionContext BuildContext(PlayerMovement playerMovement)
    {
        return new InteractionContext
        {
            PlayerTransform = playerMovement.transform,
            PlayerPosition = ResolvePlayerSamplePoint(playerMovement)
        };
    }

    private static void ClearNavigationValidationInterference(PlayerMovement playerMovement)
    {
        if (EditorPrefs.HasKey(NavigationValidationPendingActionKey))
        {
            EditorPrefs.DeleteKey(NavigationValidationPendingActionKey);
        }

        NavigationLiveValidationRunner runner =
            Object.FindFirstObjectByType<NavigationLiveValidationRunner>(FindObjectsInactive.Include);
        if (runner != null)
        {
            Object.Destroy(runner.gameObject);
        }

        if (playerMovement == null)
        {
            return;
        }

        PlayerAutoNavigator navigator = playerMovement.GetComponent<PlayerAutoNavigator>();
        navigator?.ForceCancel();
    }

    private static void ClearStoryDialogueInterference()
    {
        DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
        if (dialogueManager == null)
        {
            return;
        }

        int guard = 0;
        while (dialogueManager.IsDialogueActive && guard < 8)
        {
            dialogueManager.ForceCompleteOrAdvance();
            guard++;
        }
    }

    private static void MovePlayerSamplePoint(
        PlayerMovement playerMovement,
        NPCInformalChatInteractable interactable,
        float gapFromBoundary)
    {
        if (playerMovement == null || interactable == null)
        {
            return;
        }

        PlayerAutoNavigator navigator = playerMovement.GetComponent<PlayerAutoNavigator>();
        navigator?.ForceCancel();

        Rigidbody2D playerBody = playerMovement.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.linearVelocity = Vector2.zero;
            playerBody.angularVelocity = 0f;
        }

        Vector2 sampleOffset = ResolvePlayerSamplePoint(playerMovement) - (Vector2)playerMovement.transform.position;
        Bounds targetBounds = ResolveInteractionBounds(interactable.transform);
        float sideDirection = playerMovement.transform.position.x <= interactable.transform.position.x ? -1f : 1f;
        Vector2 targetSamplePoint = new Vector2(
            targetBounds.center.x + sideDirection * (targetBounds.extents.x + Mathf.Max(0.02f, gapFromBoundary)),
            targetBounds.center.y);
        Vector3 targetWorldPosition = (Vector3)(targetSamplePoint - sampleOffset);
        targetWorldPosition.z = playerMovement.transform.position.z;

        if (playerBody != null)
        {
            playerBody.position = targetWorldPosition;
        }
        else
        {
            playerMovement.transform.position = targetWorldPosition;
        }

        Physics2D.SyncTransforms();
    }

    private static Bounds ResolveInteractionBounds(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            SpriteRenderer spriteRenderer = targetTransform.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.bounds;
            }

            Collider2D collider2D = targetTransform.GetComponentInChildren<Collider2D>();
            if (collider2D != null)
            {
                return collider2D.bounds;
            }
        }

        return new Bounds(targetTransform != null ? targetTransform.position : Vector3.zero, Vector3.one);
    }

    private static Vector2 ResolvePlayerSamplePoint(PlayerMovement playerMovement)
    {
        if (playerMovement == null)
        {
            return Vector2.zero;
        }

        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            return playerCollider.bounds.center;
        }

        return playerMovement.transform.position;
    }
}
#endif
