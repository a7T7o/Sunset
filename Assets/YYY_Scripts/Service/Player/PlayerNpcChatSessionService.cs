using System.Collections;
using System.Collections.Generic;
using Sunset.Story;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(110)]
public class PlayerNpcChatSessionService : MonoBehaviour
{
    private enum SessionState
    {
        Idle = 0,
        PlayerTyping = 1,
        WaitingNpcReply = 2,
        NpcTyping = 3,
        AutoContinuing = 4,
        Interrupting = 5,
        Completing = 6
    }

    private sealed class ResumeIntroPlan
    {
        public string PlayerLine;
        public string NpcLine;
        public float NpcReplyDelay;

        public bool HasAnyContent =>
            !string.IsNullOrWhiteSpace(PlayerLine) ||
            !string.IsNullOrWhiteSpace(NpcLine);
    }

    public enum ConversationEndReason
    {
        None = 0,
        Completed = 1,
        WalkAwayInterrupt = 2,
        Cancelled = 3,
        SystemTakeover = 4,
        TargetInvalid = 5,
        PlayerUnavailable = 6,
        ServiceDisabled = 7
    }

    public enum ConversationResumeOutcome
    {
        None = 0,
        ResumedWithIntro = 1,
        ResumedSilently = 2,
        SuppressedByCooldown = 3,
        Expired = 4,
        DifferentNpc = 5,
        InvalidSnapshot = 6
    }

    [SerializeField] private float playerCharsPerSecond = 36f;
    [SerializeField] private float npcCharsPerSecond = 30f;
    [SerializeField] private float sessionBreakGraceSeconds = 0.18f;
    [SerializeField] private float resumeWindowSeconds = 16f;
    [SerializeField] private float resumeIntroCooldownSeconds = 5f;
    [SerializeField] private float autoContinueDelayMin = 0.18f;
    [SerializeField] private float autoContinueDelayMax = 0.34f;
    [SerializeField] private float completionHoldSeconds = 1.15f;
    [SerializeField] private float interruptionHoldSeconds = 1.35f;
    [SerializeField] private float conversationBubbleSideOffset = 0.34f;
    [SerializeField] private float playerConversationBubbleLift = 0.22f;
    [SerializeField] private float npcConversationBubbleLift = 0.06f;
    [SerializeField] private PlayerThoughtBubblePresenter playerBubblePresenter;

    private readonly Dictionary<string, int> _bundleCursorByNpcId = new Dictionary<string, int>();

    private Coroutine _sessionCoroutine;
    private PlayerMovement _playerMovement;
    private NPCInformalChatInteractable _activeInteractable;
    private NPCDialogueContentProfile.InformalConversationBundle _activeBundle;
    private string _activeNpcId = string.Empty;
    private NPCRelationshipStage _activeRelationshipStage;
    private SessionState _state;
    private bool _skipCurrentEffectRequested;
    private bool _advanceRequested;
    private bool _expediteNpcReplyRequested;
    private bool _completionDeltaApplied;
    private int _completedExchangeCount;
    private string _lastCompletedPlayerLine = string.Empty;
    private string _lastCompletedNpcLine = string.Empty;
    private string _lastInterruptedPlayerLine = string.Empty;
    private string _lastInterruptedNpcLine = string.Empty;
    private string _lastResumePlayerLine = string.Empty;
    private string _lastResumeNpcLine = string.Empty;
    private ConversationResumeOutcome _lastResumeOutcome = ConversationResumeOutcome.None;
    private string _lastResumeNpcId = string.Empty;
    private ConversationEndReason _lastConversationEndReason = ConversationEndReason.None;
    private NPCInformalChatLeaveCause _lastAbortCause = NPCInformalChatLeaveCause.Any;
    private NPCInformalChatLeavePhase _lastLeavePhase = NPCInformalChatLeavePhase.Any;
    private float _sessionBreakExceededAt = -1f;
    private NPCDialogueContentProfile.InformalConversationBundle _pendingResumeBundle;
    private string _pendingResumeNpcId = string.Empty;
    private int _pendingResumeExchangeIndex = -1;
    private string _pendingResumeLastCompletedPlayerLine = string.Empty;
    private string _pendingResumeLastCompletedNpcLine = string.Empty;
    private NPCInformalChatLeaveCause _pendingResumeAbortCause = NPCInformalChatLeaveCause.Any;
    private NPCInformalChatLeavePhase _pendingResumeLeavePhase = NPCInformalChatLeavePhase.Any;
    private float _pendingResumeExpiresAt = -1f;
    private string _resumeIntroCooldownNpcId = string.Empty;
    private float _resumeIntroCooldownEndsAt = -1f;

    public static PlayerNpcChatSessionService Instance { get; private set; }
    public static bool IsAnySessionActive => Instance != null && Instance.HasActiveConversation;

    public bool HasActiveConversation => _activeInteractable != null && _state != SessionState.Idle;
    public string DebugStateName => _state.ToString();
    public bool IsAwaitingAdvance => _state == SessionState.AutoContinuing;
    public bool IsWaitingNpcReply => _state == SessionState.WaitingNpcReply;
    public bool IsAutoContinuing => _state == SessionState.AutoContinuing;
    public bool IsTypingBubble => _state is SessionState.PlayerTyping or SessionState.NpcTyping;
    public int CompletedExchangeCount => _completedExchangeCount;
    public string LastCompletedPlayerLine => _lastCompletedPlayerLine ?? string.Empty;
    public string LastCompletedNpcLine => _lastCompletedNpcLine ?? string.Empty;
    public string LastInterruptedPlayerLine => _lastInterruptedPlayerLine ?? string.Empty;
    public string LastInterruptedNpcLine => _lastInterruptedNpcLine ?? string.Empty;
    public string LastResumePlayerLine => _lastResumePlayerLine ?? string.Empty;
    public string LastResumeNpcLine => _lastResumeNpcLine ?? string.Empty;
    public ConversationResumeOutcome LastResumeOutcome => _lastResumeOutcome;
    public string LastResumeOutcomeName => _lastResumeOutcome.ToString();
    public string LastResumeNpcId => _lastResumeNpcId ?? string.Empty;
    public ConversationEndReason LastConversationEndReason => _lastConversationEndReason;
    public string LastConversationEndReasonName => _lastConversationEndReason.ToString();
    public string LastAbortCauseName => _lastAbortCause.ToString();
    public string LastLeavePhaseName => _lastLeavePhase.ToString();
    public bool HasPendingResumeConversation => HasPendingResumeSnapshot(Time.unscaledTime);
    public string PendingResumeNpcId => _pendingResumeNpcId ?? string.Empty;
    public int PendingResumeExchangeIndex => _pendingResumeExchangeIndex;
    public string PendingResumeAbortCauseName => _pendingResumeAbortCause.ToString();
    public string PendingResumeLeavePhaseName => _pendingResumeLeavePhase.ToString();
    public string ActiveNpcName => _activeInteractable != null ? _activeInteractable.name : string.Empty;
    public string CurrentPlayerBubbleText => playerBubblePresenter != null ? playerBubblePresenter.CurrentBubbleText : string.Empty;
    public string CurrentNpcBubbleText =>
        _activeInteractable != null && _activeInteractable.BubblePresenter != null
            ? _activeInteractable.BubblePresenter.CurrentBubbleText
            : string.Empty;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        ResolveForPlayer(null);
    }

    public static PlayerNpcChatSessionService ResolveForPlayer(GameObject playerRoot)
    {
        GameObject root = playerRoot;
        if (root == null)
        {
            PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
            if (interaction != null)
            {
                root = interaction.gameObject;
            }
        }

        if (root == null)
        {
            PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
            if (movement != null)
            {
                root = movement.gameObject;
            }
        }

        if (root == null)
        {
            return Instance;
        }

        PlayerNpcChatSessionService service = root.GetComponent<PlayerNpcChatSessionService>();
        if (service == null)
        {
            service = root.AddComponent<PlayerNpcChatSessionService>();
        }

        return service;
    }

    public bool CanRouteInteractable(NPCInformalChatInteractable interactable)
    {
        return interactable != null && (_activeInteractable == null || _activeInteractable == interactable);
    }

    public bool IsConversationActiveWith(NPCInformalChatInteractable interactable)
    {
        return interactable != null && _activeInteractable == interactable && HasActiveConversation;
    }

    public string GetPromptCaption(NPCInformalChatInteractable interactable)
    {
        if (!IsConversationActiveWith(interactable))
        {
            return "闲聊";
        }

        return _state switch
        {
            SessionState.PlayerTyping => "你在说话",
            SessionState.WaitingNpcReply => "等对方回应",
            SessionState.NpcTyping => "对方在回你",
            SessionState.AutoContinuing => "对话还在继续",
            SessionState.Interrupting => "聊到一半",
            SessionState.Completing => "聊完了",
            _ => "闲聊"
        };
    }

    public string GetPromptDetail(NPCInformalChatInteractable interactable)
    {
        if (!IsConversationActiveWith(interactable))
        {
            return "按 E 开口";
        }

        return _state switch
        {
            SessionState.PlayerTyping => "按 E 跳过动效",
            SessionState.WaitingNpcReply => "按 E 让回应快一点",
            SessionState.NpcTyping => "按 E 跳过动效",
            SessionState.AutoContinuing => "这段会自己接上",
            SessionState.Interrupting => "正在收尾",
            SessionState.Completing => "按 E 立刻收尾",
            _ => string.Empty
        };
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        EnsurePlayerReferences();
    }

    private void OnEnable()
    {
        EnsurePlayerReferences();
    }

    private void OnDisable()
    {
        CancelConversationImmediate(ConversationEndReason.ServiceDisabled, NPCInformalChatLeaveCause.ServiceDisabled);

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!HasActiveConversation)
        {
            return;
        }

        if (_activeInteractable == null || !_activeInteractable.isActiveAndEnabled)
        {
            CancelConversationImmediate(ConversationEndReason.TargetInvalid, NPCInformalChatLeaveCause.TargetInvalid);
            return;
        }

        EnsurePlayerReferences();
        if (_playerMovement == null)
        {
            CancelConversationImmediate(ConversationEndReason.PlayerUnavailable, NPCInformalChatLeaveCause.PlayerUnavailable);
            return;
        }

        if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
        {
            CancelConversationImmediate(ConversationEndReason.SystemTakeover, NPCInformalChatLeaveCause.BlockingUi);
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            CancelConversationImmediate(ConversationEndReason.SystemTakeover, NPCInformalChatLeaveCause.DialogueTakeover);
            return;
        }

        if (_state is SessionState.Interrupting or SessionState.Completing)
        {
            ResetSessionBreakProbe();
            UpdateConversationBubbleLayout();
            return;
        }

        UpdateConversationBubbleLayout();
        HandleSessionBreakDistance();
    }

    public bool HandleInteract(NPCInformalChatInteractable interactable, InteractionContext context)
    {
        if (interactable == null)
        {
            return false;
        }

        if (_activeInteractable == null || _state == SessionState.Idle)
        {
            return StartConversation(interactable, context);
        }

        if (_activeInteractable != interactable)
        {
            return false;
        }

        switch (_state)
        {
            case SessionState.PlayerTyping:
            case SessionState.NpcTyping:
            case SessionState.Interrupting:
                _skipCurrentEffectRequested = true;
                return true;

            case SessionState.WaitingNpcReply:
                _expediteNpcReplyRequested = true;
                return true;

            case SessionState.Completing:
                _advanceRequested = true;
                return true;
        }

        return false;
    }

    public bool RequestAdvanceOrSkipActiveConversation()
    {
        return _activeInteractable != null && HandleInteract(_activeInteractable, null);
    }

    public bool TryStartWalkAwayInterruptForValidation()
    {
        if (!HasActiveConversation || _activeInteractable == null || _state is SessionState.Interrupting or SessionState.Completing)
        {
            return false;
        }

        StartWalkAwayInterrupt();
        return true;
    }

    public bool TryForceAbortForValidation(NPCInformalChatLeaveCause abortCause)
    {
        if (!HasActiveConversation)
        {
            return false;
        }

        CancelConversationImmediate(ResolveEndReasonForAbortCause(abortCause), abortCause);
        return true;
    }

    private bool StartConversation(NPCInformalChatInteractable interactable, InteractionContext context)
    {
        if (interactable == null)
        {
            return false;
        }

        EnsurePlayerReferences();
        if (_playerMovement == null)
        {
            return false;
        }

        string npcId = interactable.ResolveNpcId();
        NPCRelationshipStage relationshipStage = interactable.ResolveRelationshipStage();
        bool shouldClearPendingResumeAfterStart = false;
        int startExchangeIndex = 0;
        string resumedLastCompletedPlayerLine = string.Empty;
        string resumedLastCompletedNpcLine = string.Empty;
        ResumeIntroPlan resumeIntro = null;
        ConversationResumeOutcome resumeOutcome = ConversationResumeOutcome.None;

        NPCDialogueContentProfile.InformalConversationBundle bundle =
            TryResolvePendingResumePlan(
                npcId,
                out startExchangeIndex,
                out resumedLastCompletedPlayerLine,
                out resumedLastCompletedNpcLine,
                out resumeOutcome);
        if (bundle == null)
        {
            bundle = ResolveBundle(npcId, interactable.ResolveConversationBundles(relationshipStage));
            startExchangeIndex = 0;
            shouldClearPendingResumeAfterStart =
                !string.IsNullOrEmpty(_pendingResumeNpcId) &&
                !string.Equals(_pendingResumeNpcId, npcId, System.StringComparison.OrdinalIgnoreCase);
            if (shouldClearPendingResumeAfterStart)
            {
                resumeOutcome = ConversationResumeOutcome.DifferentNpc;
            }
        }

        if (bundle == null)
        {
            return false;
        }

        CancelConversationImmediate();

        _activeInteractable = interactable;
        _activeBundle = bundle;
        _activeNpcId = npcId;
        _activeRelationshipStage = relationshipStage;
        ResetValidationSnapshot();
        if (startExchangeIndex > 0)
        {
            _completedExchangeCount = startExchangeIndex;
            _lastCompletedPlayerLine = resumedLastCompletedPlayerLine ?? string.Empty;
            _lastCompletedNpcLine = resumedLastCompletedNpcLine ?? string.Empty;
        }

        _completionDeltaApplied = false;
        _skipCurrentEffectRequested = false;
        _advanceRequested = false;
        _expediteNpcReplyRequested = false;
        ResetSessionBreakProbe();
        if (bundle == _pendingResumeBundle &&
            string.Equals(_pendingResumeNpcId, npcId, System.StringComparison.OrdinalIgnoreCase))
        {
            resumeIntro = ResolveResumeIntroPlan(
                _pendingResumeAbortCause,
                _pendingResumeLeavePhase,
                startExchangeIndex);
        }

        if (resumeIntro != null &&
            resumeIntro.HasAnyContent &&
            ShouldSuppressResumeIntro(
                npcId,
                _resumeIntroCooldownNpcId,
                _resumeIntroCooldownEndsAt,
                Time.unscaledTime))
        {
            resumeIntro = null;
            if (bundle == _pendingResumeBundle &&
                string.Equals(_pendingResumeNpcId, npcId, System.StringComparison.OrdinalIgnoreCase))
            {
                resumeOutcome = ConversationResumeOutcome.SuppressedByCooldown;
            }
        }

        if (bundle == _pendingResumeBundle &&
            string.Equals(_pendingResumeNpcId, npcId, System.StringComparison.OrdinalIgnoreCase))
        {
            if (resumeOutcome == ConversationResumeOutcome.None)
            {
                resumeOutcome = ResolveMatchedResumeOutcome(
                    resumeIntro != null && resumeIntro.HasAnyContent,
                    suppressedByCooldown: false);
            }

            ClearPendingResumeSnapshot();
        }
        else if (shouldClearPendingResumeAfterStart)
        {
            ClearPendingResumeSnapshot();
        }

        RecordResumeOutcome(npcId, resumeOutcome);

        _activeInteractable.EnterConversationOccupation(context);
        FaceEachOther();
        UpdateConversationBubbleLayout();
        if (resumeIntro != null && resumeIntro.HasAnyContent)
        {
            MarkResumeIntroCooldown(npcId);
        }

        _sessionCoroutine = StartCoroutine(RunConversation(startExchangeIndex, resumeIntro));
        return true;
    }

    private IEnumerator RunConversation(int startExchangeIndex, ResumeIntroPlan resumeIntro)
    {
        List<NPCDialogueContentProfile.InformalChatExchange> exchanges = CollectPlayableExchanges(_activeBundle);
        if (exchanges.Count == 0)
        {
            EndConversation();
            yield break;
        }

        if (resumeIntro != null && resumeIntro.HasAnyContent)
        {
            _lastResumePlayerLine = resumeIntro.PlayerLine ?? string.Empty;
            _lastResumeNpcLine = resumeIntro.NpcLine ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(resumeIntro.PlayerLine))
            {
                yield return PlayPlayerLine(resumeIntro.PlayerLine, restartFadeIn: true);
            }

            if (!string.IsNullOrWhiteSpace(resumeIntro.NpcLine))
            {
                yield return WaitForSecondsOrAdvance(resumeIntro.NpcReplyDelay);

                if (_activeInteractable == null)
                {
                    yield break;
                }

                if (_playerMovement != null)
                {
                    _activeInteractable.FaceToward(_playerMovement.transform.position - _activeInteractable.transform.position);
                }

                yield return PlayNpcLine(resumeIntro.NpcLine, restartFadeIn: true);
            }
        }
        else
        {
            _lastResumePlayerLine = string.Empty;
            _lastResumeNpcLine = string.Empty;
        }

        int clampedStartExchangeIndex = Mathf.Clamp(startExchangeIndex, 0, Mathf.Max(0, exchanges.Count - 1));
        for (int exchangeIndex = clampedStartExchangeIndex; exchangeIndex < exchanges.Count; exchangeIndex++)
        {
            NPCDialogueContentProfile.InformalChatExchange exchange = exchanges[exchangeIndex];
            string playerLine = PickLine(exchange.PlayerLines, exchangeIndex);
            if (!string.IsNullOrWhiteSpace(playerLine))
            {
                yield return PlayPlayerLine(playerLine, restartFadeIn: exchangeIndex == 0);
            }

            yield return WaitForNpcReply(exchange);

            if (_activeInteractable == null)
            {
                yield break;
            }

            _activeInteractable.FaceToward(_playerMovement.transform.position - _activeInteractable.transform.position);

            string npcReplyLine = PickLine(exchange.NpcReplyLines, exchangeIndex);
            if (!string.IsNullOrWhiteSpace(npcReplyLine))
            {
                yield return PlayNpcLine(npcReplyLine, restartFadeIn: exchangeIndex == 0);
            }

            _completedExchangeCount = exchangeIndex + 1;
            _lastCompletedPlayerLine = playerLine ?? string.Empty;
            _lastCompletedNpcLine = npcReplyLine ?? string.Empty;

            if (exchangeIndex >= exchanges.Count - 1)
            {
                ApplyCompletionRelationshipDelta();
                _state = SessionState.Completing;
                yield return WaitForSecondsOrAdvance(completionHoldSeconds);
                EndConversation(ConversationEndReason.Completed);
                yield break;
            }

            _state = SessionState.AutoContinuing;
            yield return WaitForSecondsOrAdvance(Random.Range(autoContinueDelayMin, autoContinueDelayMax));
        }

        EndConversation();
    }

    private IEnumerator PlayPlayerLine(string line, bool restartFadeIn)
    {
        _state = SessionState.PlayerTyping;
        FaceEachOther();
        UpdateConversationBubbleLayout();
        yield return PlayTypedLine(
            line,
            playerCharsPerSecond,
            restartFadeIn,
            updateText: partial => playerBubblePresenter.ShowText(partial, -1f, restartFadeIn: false),
            openLine: firstChunk => playerBubblePresenter.ShowText(firstChunk, -1f, restartFadeIn));
    }

    private IEnumerator PlayNpcLine(string line, bool restartFadeIn)
    {
        if (_activeInteractable == null || _activeInteractable.BubblePresenter == null)
        {
            yield break;
        }

        _state = SessionState.NpcTyping;
        UpdateConversationBubbleLayout();
        yield return PlayTypedLine(
            line,
            npcCharsPerSecond,
            restartFadeIn,
            updateText: partial => _activeInteractable.BubblePresenter.ShowText(partial, -1f, restartFadeIn: false),
            openLine: firstChunk => _activeInteractable.BubblePresenter.ShowText(firstChunk, -1f, restartFadeIn));
    }

    private IEnumerator PlayTypedLine(string fullText, float charsPerSecond, bool restartFadeIn, System.Action<string> updateText, System.Action<string> openLine)
    {
        string sanitizedText = (fullText ?? string.Empty).Replace("\r", string.Empty);
        if (string.IsNullOrWhiteSpace(sanitizedText))
        {
            yield break;
        }

        _skipCurrentEffectRequested = false;
        int visibleCount = 0;
        float secondsPerChar = 1f / Mathf.Max(1f, charsPerSecond);
        float accumulatedTime = secondsPerChar;
        bool hasOpenedLine = false;

        while (visibleCount < sanitizedText.Length)
        {
            if (_skipCurrentEffectRequested)
            {
                visibleCount = sanitizedText.Length;
            }
            else
            {
                accumulatedTime += Time.unscaledDeltaTime;
                while (accumulatedTime >= secondsPerChar && visibleCount < sanitizedText.Length)
                {
                    visibleCount++;
                    accumulatedTime -= secondsPerChar;
                }
            }

            if (visibleCount <= 0)
            {
                visibleCount = 1;
            }

            string partial = sanitizedText.Substring(0, Mathf.Min(visibleCount, sanitizedText.Length));
            if (!hasOpenedLine)
            {
                openLine(partial);
                hasOpenedLine = true;
            }
            else
            {
                updateText(partial);
            }

            if (visibleCount >= sanitizedText.Length)
            {
                break;
            }

            yield return null;
        }

        updateText(sanitizedText);
        _skipCurrentEffectRequested = false;
    }

    private IEnumerator WaitForNpcReply(NPCDialogueContentProfile.InformalChatExchange exchange)
    {
        _state = SessionState.WaitingNpcReply;
        _expediteNpcReplyRequested = false;

        float waitSeconds = Random.Range(exchange.NpcReplyDelayMin, exchange.NpcReplyDelayMax);
        while (waitSeconds > 0f && !_expediteNpcReplyRequested)
        {
            waitSeconds -= Time.unscaledDeltaTime;
            yield return null;
        }

        _expediteNpcReplyRequested = false;
    }

    private IEnumerator WaitForSecondsOrAdvance(float seconds)
    {
        _advanceRequested = false;
        float remaining = Mathf.Max(0f, seconds);
        while (remaining > 0f && !_advanceRequested)
        {
            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        _advanceRequested = false;
    }

    private void StartWalkAwayInterrupt()
    {
        if (_activeInteractable == null || _state is SessionState.Interrupting or SessionState.Completing)
        {
            return;
        }

        NPCInformalChatLeavePhase leavePhase = ResolveCurrentLeavePhase();
        CapturePendingResumeSnapshot(NPCInformalChatLeaveCause.DistanceGraceExceeded, leavePhase);
        ResetSessionBreakProbe();
        _lastAbortCause = NPCInformalChatLeaveCause.DistanceGraceExceeded;
        _lastLeavePhase = leavePhase;
        _skipCurrentEffectRequested = false;
        _advanceRequested = false;
        _expediteNpcReplyRequested = false;

        if (playerBubblePresenter != null && playerBubblePresenter.IsVisible)
        {
            playerBubblePresenter.HideImmediate();
        }

        if (_sessionCoroutine != null)
        {
            StopCoroutine(_sessionCoroutine);
            _sessionCoroutine = null;
        }

        _state = SessionState.Interrupting;
        _sessionCoroutine = StartCoroutine(RunWalkAwayInterrupt(leavePhase));
    }

    private IEnumerator RunWalkAwayInterrupt(NPCInformalChatLeavePhase leavePhase)
    {
        _lastInterruptedPlayerLine = string.Empty;
        _lastInterruptedNpcLine = string.Empty;

        NPCDialogueContentProfile.InformalChatInterruptReaction reaction =
            ResolveInterruptReaction(NPCInformalChatLeaveCause.DistanceGraceExceeded, leavePhase);

        string playerExitLine = PickLine(reaction.PlayerExitLines, 0);
        if (string.IsNullOrWhiteSpace(playerExitLine))
        {
            playerExitLine = "我先走一步，等会儿再聊。";
        }

        _lastInterruptedPlayerLine = playerExitLine;
        yield return PlayPlayerLine(playerExitLine, restartFadeIn: true);
        yield return WaitForSecondsOrAdvance(0.18f);

        if (_activeInteractable != null && _activeInteractable.BubblePresenter != null)
        {
            string reactionCue = reaction.ReactionCue;
            if (!string.IsNullOrWhiteSpace(reactionCue))
            {
                _lastInterruptedNpcLine = reactionCue.Trim();
                _activeInteractable.BubblePresenter.ShowReactionCue(
                    reactionCue,
                    -1f,
                    restartFadeIn: leavePhase == NPCInformalChatLeavePhase.NpcSpeaking || !_activeInteractable.BubblePresenter.IsBubbleVisible);
                yield return WaitForSecondsOrAdvance(Mathf.Min(0.4f, Mathf.Max(0.15f, reaction.NpcReactionDelay)));
            }

            string npcReactionLine = PickLine(reaction.NpcReactionLines, 0);
            if (!string.IsNullOrWhiteSpace(npcReactionLine))
            {
                yield return WaitForSecondsOrAdvance(reaction.NpcReactionDelay);
                if (_playerMovement != null)
                {
                    _activeInteractable.FaceToward(_playerMovement.transform.position - _activeInteractable.transform.position);
                }

                _lastInterruptedNpcLine = npcReactionLine;
                yield return PlayNpcLine(
                    npcReactionLine,
                    restartFadeIn: leavePhase == NPCInformalChatLeavePhase.NpcSpeaking || !_activeInteractable.BubblePresenter.IsBubbleVisible);
            }
        }

        if (reaction.RelationshipDelta != 0)
        {
            PlayerNpcRelationshipService.AdjustStage(_activeNpcId, reaction.RelationshipDelta);
        }

        _state = SessionState.Completing;
        yield return WaitForSecondsOrAdvance(interruptionHoldSeconds);
        EndConversation(ConversationEndReason.WalkAwayInterrupt);
    }

    private NPCDialogueContentProfile.InformalChatInterruptReaction ResolveInterruptReaction(
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (_activeInteractable != null)
        {
            NPCDialogueContentProfile.InformalChatInterruptReaction configuredReaction =
                _activeInteractable.ResolveInterruptReaction(_activeRelationshipStage, leaveCause, leavePhase);
            if (configuredReaction != null && configuredReaction.HasAnyContent())
            {
                return configuredReaction;
            }
        }

        return CreateFallbackInterruptReaction(leaveCause, leavePhase);
    }

    private void ApplyCompletionRelationshipDelta()
    {
        if (_completionDeltaApplied || _activeBundle == null || string.IsNullOrEmpty(_activeNpcId))
        {
            return;
        }

        int delta = _activeBundle.CompletionRelationshipDelta;
        if (delta != 0)
        {
            PlayerNpcRelationshipService.AdjustStage(_activeNpcId, delta);
        }

        _completionDeltaApplied = true;
    }

    private void EndConversation(ConversationEndReason endReason = ConversationEndReason.None)
    {
        if (_sessionCoroutine != null)
        {
            StopCoroutine(_sessionCoroutine);
            _sessionCoroutine = null;
        }

        ResetSessionBreakProbe();

        if (_activeInteractable != null)
        {
            _activeInteractable.ExitConversationOccupation();
            if (_activeInteractable.BubblePresenter != null)
            {
                _activeInteractable.BubblePresenter.ClearConversationLayoutShift();
                _activeInteractable.BubblePresenter.HideBubble();
            }
        }

        if (playerBubblePresenter != null)
        {
            playerBubblePresenter.ClearConversationLayoutShift();
            playerBubblePresenter.HideImmediate();
        }

        _activeInteractable = null;
        _activeBundle = null;
        _activeNpcId = string.Empty;
        _state = SessionState.Idle;
        _skipCurrentEffectRequested = false;
        _advanceRequested = false;
        _expediteNpcReplyRequested = false;
        _completionDeltaApplied = false;

        if (endReason != ConversationEndReason.None)
        {
            _lastConversationEndReason = endReason;
        }
    }

    private void CancelConversationImmediate(
        ConversationEndReason endReason = ConversationEndReason.Cancelled,
        NPCInformalChatLeaveCause abortCause = NPCInformalChatLeaveCause.Any)
    {
        bool hadConversation =
            _sessionCoroutine != null ||
            _activeInteractable != null ||
            _state != SessionState.Idle;
        NPCInformalChatLeavePhase leavePhase = ResolveCurrentLeavePhase();

        if (hadConversation)
        {
            CaptureImmediateAbortSnapshot(abortCause, leavePhase);
            CapturePendingResumeSnapshot(abortCause, leavePhase);
        }

        if (_sessionCoroutine != null)
        {
            StopCoroutine(_sessionCoroutine);
            _sessionCoroutine = null;
        }

        ResetSessionBreakProbe();

        if (_activeInteractable != null)
        {
            _activeInteractable.ExitConversationOccupation();
            if (_activeInteractable.BubblePresenter != null)
            {
                _activeInteractable.BubblePresenter.ClearConversationLayoutShift();
                _activeInteractable.BubblePresenter.HideBubble();
            }
        }

        if (playerBubblePresenter != null && playerBubblePresenter.IsVisible)
        {
            playerBubblePresenter.HideImmediate();
        }
        else if (playerBubblePresenter != null)
        {
            playerBubblePresenter.ClearConversationLayoutShift();
        }

        _activeInteractable = null;
        _activeBundle = null;
        _activeNpcId = string.Empty;
        _state = SessionState.Idle;
        _skipCurrentEffectRequested = false;
        _advanceRequested = false;
        _expediteNpcReplyRequested = false;
        _completionDeltaApplied = false;

        if (hadConversation)
        {
            _lastConversationEndReason = endReason;
            _lastAbortCause = abortCause;
            _lastLeavePhase = leavePhase;
        }
    }

    private void ResetValidationSnapshot()
    {
        _completedExchangeCount = 0;
        _lastCompletedPlayerLine = string.Empty;
        _lastCompletedNpcLine = string.Empty;
        _lastInterruptedPlayerLine = string.Empty;
        _lastInterruptedNpcLine = string.Empty;
        _lastResumePlayerLine = string.Empty;
        _lastResumeNpcLine = string.Empty;
        _lastResumeOutcome = ConversationResumeOutcome.None;
        _lastResumeNpcId = string.Empty;
        _lastConversationEndReason = ConversationEndReason.None;
        _lastAbortCause = NPCInformalChatLeaveCause.Any;
        _lastLeavePhase = NPCInformalChatLeavePhase.Any;
        ResetSessionBreakProbe();
    }

    private NPCDialogueContentProfile.InformalConversationBundle TryResolvePendingResumePlan(
        string npcId,
        out int startExchangeIndex,
        out string resumedLastCompletedPlayerLine,
        out string resumedLastCompletedNpcLine,
        out ConversationResumeOutcome resumeOutcome)
    {
        startExchangeIndex = 0;
        resumedLastCompletedPlayerLine = string.Empty;
        resumedLastCompletedNpcLine = string.Empty;
        resumeOutcome = ConversationResumeOutcome.None;

        if (!HasPendingResumeSnapshot(Time.unscaledTime))
        {
            if (!string.IsNullOrEmpty(_pendingResumeNpcId))
            {
                resumeOutcome = ResolveUnavailablePendingResumeOutcome(_pendingResumeExpiresAt, Time.unscaledTime);
                ClearPendingResumeSnapshot();
            }

            return null;
        }

        if (!string.Equals(_pendingResumeNpcId, npcId, System.StringComparison.OrdinalIgnoreCase))
        {
            resumeOutcome = ConversationResumeOutcome.DifferentNpc;
            return null;
        }

        List<NPCDialogueContentProfile.InformalChatExchange> playableExchanges = CollectPlayableExchanges(_pendingResumeBundle);
        if (!CanConsumePendingResumeSnapshot(
                _pendingResumeNpcId,
                npcId,
                _pendingResumeExpiresAt,
                Time.unscaledTime,
                _pendingResumeExchangeIndex,
                playableExchanges.Count))
        {
            resumeOutcome = ResolveUnavailablePendingResumeOutcome(_pendingResumeExpiresAt, Time.unscaledTime);
            ClearPendingResumeSnapshot();
            return null;
        }

        startExchangeIndex = _pendingResumeExchangeIndex;
        resumedLastCompletedPlayerLine = _pendingResumeLastCompletedPlayerLine;
        resumedLastCompletedNpcLine = _pendingResumeLastCompletedNpcLine;
        return _pendingResumeBundle;
    }

    private void EnsurePlayerReferences()
    {
        if (_playerMovement == null)
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        if (_playerMovement == null)
        {
            _playerMovement = GetComponentInChildren<PlayerMovement>();
        }

        if (playerBubblePresenter == null)
        {
            playerBubblePresenter = GetComponent<PlayerThoughtBubblePresenter>();
        }

        if (playerBubblePresenter == null)
        {
            playerBubblePresenter = gameObject.AddComponent<PlayerThoughtBubblePresenter>();
        }
    }

    private void FaceEachOther()
    {
        if (_activeInteractable == null || _playerMovement == null)
        {
            return;
        }

        Vector2 playerToNpc = _activeInteractable.transform.position - _playerMovement.transform.position;
        Vector2 npcToPlayer = _playerMovement.transform.position - _activeInteractable.transform.position;
        _playerMovement.SetFacingDirection(playerToNpc);
        _activeInteractable.FaceToward(npcToPlayer);
    }

    private void UpdateConversationBubbleLayout()
    {
        if (playerBubblePresenter == null)
        {
            return;
        }

        if (_activeInteractable == null || _playerMovement == null)
        {
            playerBubblePresenter.ClearConversationLayoutShift();
            return;
        }

        NPCBubblePresenter npcBubblePresenter = _activeInteractable.BubblePresenter;
        if (npcBubblePresenter == null)
        {
            playerBubblePresenter.ClearConversationLayoutShift();
            return;
        }

        Vector3 delta = _activeInteractable.transform.position - _playerMovement.transform.position;
        float horizontalSign = Mathf.Abs(delta.x) > 0.08f ? Mathf.Sign(delta.x) : 1f;
        Vector3 playerShift = new Vector3(
            -horizontalSign * conversationBubbleSideOffset,
            playerConversationBubbleLift,
            0f);
        Vector3 npcShift = new Vector3(
            horizontalSign * conversationBubbleSideOffset,
            npcConversationBubbleLift,
            0f);

        playerBubblePresenter.SetConversationLayoutShift(playerShift);
        npcBubblePresenter.SetConversationLayoutShift(npcShift);
    }

    private void HandleSessionBreakDistance()
    {
        if (_activeInteractable == null || _playerMovement == null)
        {
            ResetSessionBreakProbe();
            return;
        }

        Vector2 playerPosition = SpringDay1UiLayerUtility.GetInteractionSamplePoint(_playerMovement.transform);
        float boundaryDistance = _activeInteractable.GetBoundaryDistance(playerPosition);
        if (boundaryDistance <= _activeInteractable.SessionBreakDistance)
        {
            ResetSessionBreakProbe();
            return;
        }

        if (_sessionBreakExceededAt < 0f)
        {
            _sessionBreakExceededAt = Time.unscaledTime;
            return;
        }

        if (Time.unscaledTime - _sessionBreakExceededAt >= sessionBreakGraceSeconds)
        {
            StartWalkAwayInterrupt();
        }
    }

    private void ResetSessionBreakProbe()
    {
        _sessionBreakExceededAt = -1f;
    }

    private bool HasPendingResumeSnapshot(float now)
    {
        return CanConsumePendingResumeSnapshot(
            _pendingResumeNpcId,
            _pendingResumeNpcId,
            _pendingResumeExpiresAt,
            now,
            _pendingResumeExchangeIndex,
            _pendingResumeBundle != null ? CollectPlayableExchanges(_pendingResumeBundle).Count : 0);
    }

    private NPCInformalChatLeavePhase ResolveCurrentLeavePhase()
    {
        return _state switch
        {
            SessionState.PlayerTyping => NPCInformalChatLeavePhase.PlayerSpeaking,
            SessionState.WaitingNpcReply => NPCInformalChatLeavePhase.NpcThinking,
            SessionState.NpcTyping => NPCInformalChatLeavePhase.NpcSpeaking,
            SessionState.AutoContinuing => NPCInformalChatLeavePhase.BetweenTurns,
            SessionState.Interrupting => NPCInformalChatLeavePhase.InterruptSequence,
            SessionState.Completing => NPCInformalChatLeavePhase.CompletionHold,
            _ => NPCInformalChatLeavePhase.Any
        };
    }

    private void CaptureImmediateAbortSnapshot(
        NPCInformalChatLeaveCause abortCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (abortCause == NPCInformalChatLeaveCause.Any)
        {
            return;
        }

        NPCDialogueContentProfile.InformalChatInterruptReaction reaction =
            ResolveInterruptReaction(abortCause, leavePhase);
        if (reaction == null || !reaction.HasAnyContent())
        {
            return;
        }

        string playerExitLine = PickLine(reaction.PlayerExitLines, 0);
        if (!string.IsNullOrWhiteSpace(playerExitLine))
        {
            _lastInterruptedPlayerLine = playerExitLine;
        }

        string npcReactionLine = PickLine(reaction.NpcReactionLines, 0);
        if (!string.IsNullOrWhiteSpace(npcReactionLine))
        {
            _lastInterruptedNpcLine = npcReactionLine;
            return;
        }

        if (!string.IsNullOrWhiteSpace(reaction.ReactionCue))
        {
            _lastInterruptedNpcLine = reaction.ReactionCue.Trim();
        }
    }

    private void CapturePendingResumeSnapshot(
        NPCInformalChatLeaveCause abortCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        if (_activeBundle == null || string.IsNullOrEmpty(_activeNpcId))
        {
            return;
        }

        List<NPCDialogueContentProfile.InformalChatExchange> playableExchanges = CollectPlayableExchanges(_activeBundle);
        if (!TryResolveResumeExchangeIndex(
                abortCause,
                leavePhase,
                _completedExchangeCount,
                playableExchanges.Count,
                out int startExchangeIndex))
        {
            if (abortCause != NPCInformalChatLeaveCause.Any)
            {
                ClearPendingResumeSnapshot();
            }

            return;
        }

        _pendingResumeBundle = _activeBundle;
        _pendingResumeNpcId = _activeNpcId;
        _pendingResumeExchangeIndex = startExchangeIndex;
        _pendingResumeLastCompletedPlayerLine = _lastCompletedPlayerLine ?? string.Empty;
        _pendingResumeLastCompletedNpcLine = _lastCompletedNpcLine ?? string.Empty;
        _pendingResumeAbortCause = abortCause;
        _pendingResumeLeavePhase = leavePhase;
        _pendingResumeExpiresAt = Time.unscaledTime + Mathf.Max(0.1f, resumeWindowSeconds);
    }

    private void ClearPendingResumeSnapshot()
    {
        _pendingResumeBundle = null;
        _pendingResumeNpcId = string.Empty;
        _pendingResumeExchangeIndex = -1;
        _pendingResumeLastCompletedPlayerLine = string.Empty;
        _pendingResumeLastCompletedNpcLine = string.Empty;
        _pendingResumeAbortCause = NPCInformalChatLeaveCause.Any;
        _pendingResumeLeavePhase = NPCInformalChatLeavePhase.Any;
        _pendingResumeExpiresAt = -1f;
    }

    private void RecordResumeOutcome(string npcId, ConversationResumeOutcome resumeOutcome)
    {
        _lastResumeOutcome = resumeOutcome;
        _lastResumeNpcId = resumeOutcome == ConversationResumeOutcome.None
            ? string.Empty
            : (npcId ?? string.Empty);
    }

    private void MarkResumeIntroCooldown(string npcId)
    {
        _resumeIntroCooldownNpcId = npcId ?? string.Empty;
        _resumeIntroCooldownEndsAt = Time.unscaledTime + Mathf.Max(0f, resumeIntroCooldownSeconds);
    }

    private static ConversationEndReason ResolveEndReasonForAbortCause(NPCInformalChatLeaveCause abortCause)
    {
        return abortCause switch
        {
            NPCInformalChatLeaveCause.BlockingUi => ConversationEndReason.SystemTakeover,
            NPCInformalChatLeaveCause.DialogueTakeover => ConversationEndReason.SystemTakeover,
            NPCInformalChatLeaveCause.TargetInvalid => ConversationEndReason.TargetInvalid,
            NPCInformalChatLeaveCause.PlayerUnavailable => ConversationEndReason.PlayerUnavailable,
            NPCInformalChatLeaveCause.ServiceDisabled => ConversationEndReason.ServiceDisabled,
            NPCInformalChatLeaveCause.DistanceGraceExceeded => ConversationEndReason.WalkAwayInterrupt,
            _ => ConversationEndReason.Cancelled
        };
    }

    private NPCDialogueContentProfile.InformalConversationBundle ResolveBundle(string npcId, NPCDialogueContentProfile.InformalConversationBundle[] bundles)
    {
        List<NPCDialogueContentProfile.InformalConversationBundle> playableBundles = new List<NPCDialogueContentProfile.InformalConversationBundle>();
        if (bundles != null)
        {
            for (int index = 0; index < bundles.Length; index++)
            {
                NPCDialogueContentProfile.InformalConversationBundle bundle = bundles[index];
                if (bundle != null && bundle.HasPlayableExchanges())
                {
                    playableBundles.Add(bundle);
                }
            }
        }

        if (playableBundles.Count == 0)
        {
            return null;
        }

        if (string.IsNullOrEmpty(npcId))
        {
            return playableBundles[0];
        }

        _bundleCursorByNpcId.TryGetValue(npcId, out int cursor);
        NPCDialogueContentProfile.InformalConversationBundle selectedBundle = playableBundles[cursor % playableBundles.Count];
        _bundleCursorByNpcId[npcId] = cursor + 1;
        return selectedBundle;
    }

    private static List<NPCDialogueContentProfile.InformalChatExchange> CollectPlayableExchanges(NPCDialogueContentProfile.InformalConversationBundle bundle)
    {
        List<NPCDialogueContentProfile.InformalChatExchange> exchanges = new List<NPCDialogueContentProfile.InformalChatExchange>();
        if (bundle == null)
        {
            return exchanges;
        }

        NPCDialogueContentProfile.InformalChatExchange[] rawExchanges = bundle.Exchanges;
        for (int index = 0; index < rawExchanges.Length; index++)
        {
            NPCDialogueContentProfile.InformalChatExchange exchange = rawExchanges[index];
            if (exchange == null)
            {
                continue;
            }

            if (!HasAnyPlayableLines(exchange.PlayerLines) || !HasAnyPlayableLines(exchange.NpcReplyLines))
            {
                continue;
            }

            exchanges.Add(exchange);
        }

        return exchanges;
    }

    private static NPCDialogueContentProfile.InformalChatInterruptReaction CreateFallbackInterruptReaction(
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase)
    {
        return leaveCause switch
        {
            NPCInformalChatLeaveCause.DistanceGraceExceeded => CreateFallbackDistanceReaction(leavePhase),
            NPCInformalChatLeaveCause.BlockingUi => CreateFallbackBlockingUiReaction(leavePhase),
            NPCInformalChatLeaveCause.DialogueTakeover => CreateFallbackDialogueTakeoverReaction(leavePhase),
            NPCInformalChatLeaveCause.TargetInvalid => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "人呢",
                new[] { "欸，人怎么一下不见了？" },
                System.Array.Empty<string>(),
                relationshipDelta: 0,
                npcReactionDelay: 0f),
            NPCInformalChatLeaveCause.PlayerUnavailable => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先缓一下",
                new[] { "我先缓一下，刚才那段等会儿再接。" },
                System.Array.Empty<string>(),
                relationshipDelta: 0,
                npcReactionDelay: 0f),
            NPCInformalChatLeaveCause.ServiceDisabled => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先停一下",
                new[] { "这会儿先停一下，待会儿再把话接上。" },
                System.Array.Empty<string>(),
                relationshipDelta: 0,
                npcReactionDelay: 0f),
            _ => new NPCDialogueContentProfile.InformalChatInterruptReaction()
        };
    }

    private static NPCDialogueContentProfile.InformalChatInterruptReaction CreateFallbackDistanceReaction(
        NPCInformalChatLeavePhase leavePhase)
    {
        return leavePhase switch
        {
            NPCInformalChatLeavePhase.PlayerSpeaking => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先走一步",
                new[] { "我先走一步，等会儿回来把刚才那句接上。" },
                new[] { "好，我先替你把这句记着，回来再接着说。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.22f),
            NPCInformalChatLeavePhase.NpcThinking => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先缓一下",
                new[] { "我先走一下，你先别急着答我。" },
                new[] { "好，我把刚才那句先留在这儿。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f),
            NPCInformalChatLeavePhase.NpcSpeaking => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先打断一下",
                new[] { "抱歉，先打断一下，我得先走开。" },
                new[] { "好吧，我先把话停在这里，等你回来。" },
                relationshipDelta: -1,
                npcReactionDelay: 0.18f),
            NPCInformalChatLeavePhase.BetweenTurns => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "等会儿再接",
                new[] { "我先去忙一下，等会儿回来接着说。" },
                new[] { "好，我记着我们刚才聊到这儿。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f),
            _ => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先走一步",
                new[] { "我先走一步，等会儿再聊。" },
                new[] { "好，等你有空了我们再接上。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.22f)
        };
    }

    private static NPCDialogueContentProfile.InformalChatInterruptReaction CreateFallbackBlockingUiReaction(
        NPCInformalChatLeavePhase leavePhase)
    {
        return leavePhase switch
        {
            NPCInformalChatLeavePhase.NpcSpeaking => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先忙正事",
                new[] { "先打住，我得先处理一下眼前这件事。" },
                new[] { "好，你先忙，我把这句停在这里。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.18f),
            NPCInformalChatLeavePhase.BetweenTurns => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先忙正事",
                new[] { "先等等，我得先处理一下眼前这件事。" },
                new[] { "好，你先忙，回头我们从刚才那儿接着说。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f),
            _ => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先忙正事",
                new[] { "先等等，我得先处理一下眼前这件事。" },
                new[] { "好，你先忙，等你有空了我们再接着说。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f)
        };
    }

    private static NPCDialogueContentProfile.InformalChatInterruptReaction CreateFallbackDialogueTakeoverReaction(
        NPCInformalChatLeavePhase leavePhase)
    {
        return leavePhase switch
        {
            NPCInformalChatLeavePhase.NpcSpeaking => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先插一句",
                new[] { "先等一下，眼下这件事得先说清楚。" },
                new[] { "去吧，我把话先停在这里，回头再接。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.18f),
            NPCInformalChatLeavePhase.BetweenTurns => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先插一句",
                new[] { "先等一下，眼下这件事得先说清楚。" },
                new[] { "去吧，回头我们从刚才那段接上。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f),
            _ => NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                "先插一句",
                new[] { "先等一下，眼下这件事得先说清楚。" },
                new[] { "去吧，回头我们再把刚才的话接上。" },
                relationshipDelta: 0,
                npcReactionDelay: 0.2f)
        };
    }

    private ResumeIntroPlan ResolveResumeIntroPlan(
        NPCInformalChatLeaveCause abortCause,
        NPCInformalChatLeavePhase leavePhase,
        int startExchangeIndex)
    {
        if (_activeInteractable != null)
        {
            NPCDialogueContentProfile.InformalChatResumeIntro configuredIntro =
                _activeInteractable.ResolveResumeIntro(_activeRelationshipStage, abortCause, leavePhase);
            if (configuredIntro != null && configuredIntro.HasAnyContent())
            {
                return new ResumeIntroPlan
                {
                    PlayerLine = PickLine(configuredIntro.PlayerResumeLines, Mathf.Max(0, startExchangeIndex - 1)),
                    NpcLine = PickLine(configuredIntro.NpcResumeLines, Mathf.Max(0, startExchangeIndex - 1)),
                    NpcReplyDelay = configuredIntro.NpcResumeDelay
                };
            }
        }

        return CreateFallbackResumeIntroPlan(abortCause, leavePhase, startExchangeIndex);
    }

    private static ResumeIntroPlan CreateFallbackResumeIntroPlan(
        NPCInformalChatLeaveCause abortCause,
        NPCInformalChatLeavePhase leavePhase,
        int startExchangeIndex)
    {
        if (abortCause == NPCInformalChatLeaveCause.DistanceGraceExceeded && startExchangeIndex <= 0)
        {
            return null;
        }

        return abortCause switch
        {
            NPCInformalChatLeaveCause.DistanceGraceExceeded => CreateDistanceResumeIntroPlan(leavePhase),
            NPCInformalChatLeaveCause.BlockingUi => new ResumeIntroPlan
            {
                PlayerLine = "刚才被别的事打断了，我们接着说。",
                NpcLine = "好，我们就从刚才那儿接着往下说。",
                NpcReplyDelay = 0.18f
            },
            NPCInformalChatLeaveCause.DialogueTakeover => new ResumeIntroPlan
            {
                PlayerLine = "刚才那边插进来一句，我们继续。",
                NpcLine = "嗯，刚才那段我还替你记着。",
                NpcReplyDelay = 0.18f
            },
            _ => null
        };
    }

    private static ResumeIntroPlan CreateDistanceResumeIntroPlan(NPCInformalChatLeavePhase leavePhase)
    {
        return leavePhase switch
        {
            NPCInformalChatLeavePhase.NpcSpeaking => new ResumeIntroPlan
            {
                PlayerLine = "我回来了，刚才那句你继续说。",
                NpcLine = "好，我刚才正说到一半，我们接着来。",
                NpcReplyDelay = 0.16f
            },
            NPCInformalChatLeavePhase.BetweenTurns => new ResumeIntroPlan
            {
                PlayerLine = "我回来了，我们把刚才那段接上吧。",
                NpcLine = "好，我也正等着你回来继续。",
                NpcReplyDelay = 0.16f
            },
            _ => new ResumeIntroPlan
            {
                PlayerLine = "我回来了，我们继续刚才那段吧。",
                NpcLine = "好，刚才的话我还记得。",
                NpcReplyDelay = 0.16f
            }
        };
    }

    private static bool TryResolveResumeExchangeIndex(
        NPCInformalChatLeaveCause abortCause,
        NPCInformalChatLeavePhase leavePhase,
        int completedExchangeCount,
        int playableExchangeCount,
        out int startExchangeIndex)
    {
        startExchangeIndex = -1;
        if (playableExchangeCount <= 0 || completedExchangeCount >= playableExchangeCount)
        {
            return false;
        }

        if (!IsResumableAbortCause(abortCause) || !IsResumableLeavePhase(leavePhase))
        {
            return false;
        }

        startExchangeIndex = Mathf.Clamp(completedExchangeCount, 0, playableExchangeCount - 1);
        return true;
    }

    private static ConversationResumeOutcome ResolveMatchedResumeOutcome(bool hasResumeIntro, bool suppressedByCooldown)
    {
        if (suppressedByCooldown)
        {
            return ConversationResumeOutcome.SuppressedByCooldown;
        }

        return hasResumeIntro
            ? ConversationResumeOutcome.ResumedWithIntro
            : ConversationResumeOutcome.ResumedSilently;
    }

    private static ConversationResumeOutcome ResolveUnavailablePendingResumeOutcome(float expiresAt, float now)
    {
        return expiresAt >= 0f && now > expiresAt
            ? ConversationResumeOutcome.Expired
            : ConversationResumeOutcome.InvalidSnapshot;
    }

    private static bool ShouldSuppressResumeIntro(
        string npcId,
        string cooldownNpcId,
        float cooldownEndsAt,
        float now)
    {
        if (string.IsNullOrEmpty(npcId) ||
            string.IsNullOrEmpty(cooldownNpcId) ||
            !string.Equals(npcId, cooldownNpcId, System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return cooldownEndsAt >= 0f && now < cooldownEndsAt;
    }

    private static bool CanConsumePendingResumeSnapshot(
        string pendingNpcId,
        string requestedNpcId,
        float expiresAt,
        float now,
        int startExchangeIndex,
        int playableExchangeCount)
    {
        if (string.IsNullOrEmpty(pendingNpcId) ||
            string.IsNullOrEmpty(requestedNpcId) ||
            !string.Equals(pendingNpcId, requestedNpcId, System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (expiresAt < 0f || now > expiresAt)
        {
            return false;
        }

        return startExchangeIndex >= 0 && startExchangeIndex < playableExchangeCount;
    }

    private static bool IsResumableAbortCause(NPCInformalChatLeaveCause abortCause)
    {
        return abortCause is NPCInformalChatLeaveCause.DistanceGraceExceeded
            or NPCInformalChatLeaveCause.BlockingUi
            or NPCInformalChatLeaveCause.DialogueTakeover;
    }

    private static bool IsResumableLeavePhase(NPCInformalChatLeavePhase leavePhase)
    {
        return leavePhase is NPCInformalChatLeavePhase.PlayerSpeaking
            or NPCInformalChatLeavePhase.NpcThinking
            or NPCInformalChatLeavePhase.NpcSpeaking
            or NPCInformalChatLeavePhase.BetweenTurns;
    }

    private static string PickLine(string[] lines, int fallbackIndex)
    {
        List<string> playableLines = new List<string>();
        if (lines != null)
        {
            for (int index = 0; index < lines.Length; index++)
            {
                if (!string.IsNullOrWhiteSpace(lines[index]))
                {
                    playableLines.Add(lines[index].Trim());
                }
            }
        }

        if (playableLines.Count == 0)
        {
            return string.Empty;
        }

        return playableLines[fallbackIndex % playableLines.Count];
    }

    private static bool HasAnyPlayableLines(string[] lines)
    {
        if (lines == null)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }
}
