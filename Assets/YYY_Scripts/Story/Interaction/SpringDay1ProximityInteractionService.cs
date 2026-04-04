using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(180)]
    public class SpringDay1ProximityInteractionService : MonoBehaviour
    {
        private const string TaskPriorityOverlayCaption = "进入任务";
        private const string TaskPriorityOverlayDetail = "按 E 开始任务相关对话";

        private readonly struct Candidate
        {
            public Candidate(
                Transform anchorTarget,
                KeyCode interactionKey,
                string keyLabel,
                string caption,
                string detail,
                float boundaryDistance,
                int priority,
                float cooldownSeconds,
                bool canTriggerNow,
                bool forceFocus,
                bool showWorldIndicator,
                SpringDay1WorldHintBubble.HintVisualKind visualKind,
                Action trigger)
            {
                AnchorTarget = anchorTarget;
                InteractionKey = interactionKey;
                KeyLabel = string.IsNullOrWhiteSpace(keyLabel) ? interactionKey.ToString() : keyLabel.Trim();
                Caption = string.IsNullOrWhiteSpace(caption) ? "交互" : caption.Trim();
                Detail = detail ?? string.Empty;
                BoundaryDistance = Mathf.Max(0f, boundaryDistance);
                Priority = priority;
                CooldownSeconds = Mathf.Max(0f, cooldownSeconds);
                CanTriggerNow = canTriggerNow;
                ForceFocus = forceFocus;
                ShowWorldIndicator = showWorldIndicator;
                VisualKind = visualKind;
                Trigger = trigger;
            }

            public Transform AnchorTarget { get; }
            public KeyCode InteractionKey { get; }
            public string KeyLabel { get; }
            public string Caption { get; }
            public string Detail { get; }
            public float BoundaryDistance { get; }
            public int Priority { get; }
            public float CooldownSeconds { get; }
            public bool CanTriggerNow { get; }
            public bool ForceFocus { get; }
            public bool ShowWorldIndicator { get; }
            public SpringDay1WorldHintBubble.HintVisualKind VisualKind { get; }
            public Action Trigger { get; }
        }

        private static SpringDay1ProximityInteractionService _instance;

        private readonly Dictionary<int, float> _lastTriggeredAtByAnchorId = new();
        private Candidate _pendingCandidate;
        private bool _hasPendingCandidate;
        private int _pendingCandidateFrame = -1;
        private Candidate _currentCandidate;
        private bool _hasCurrentCandidate;
        private NPCBubblePresenter _suppressedNpcBubblePresenter;

        public static Transform CurrentAnchorTarget =>
            _instance != null && _instance._hasCurrentCandidate
                ? _instance._currentCandidate.AnchorTarget
                : null;

        public static string CurrentFocusSummary =>
            _instance != null
                ? _instance.BuildFocusSummary()
                : "none";

        public static bool CurrentCanTriggerNow =>
            _instance != null &&
            _instance._hasCurrentCandidate &&
            _instance._currentCandidate.CanTriggerNow;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            EnsureRuntime();
        }

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            SpringDay1ProximityInteractionService existing =
                FindFirstObjectByType<SpringDay1ProximityInteractionService>(FindObjectsInactive.Include);
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            GameObject runtimeObject = new GameObject(nameof(SpringDay1ProximityInteractionService));
            _instance = runtimeObject.AddComponent<SpringDay1ProximityInteractionService>();
        }

        public static void ReportCandidate(
            Transform anchorTarget,
            KeyCode interactionKey,
            string keyLabel,
            string caption,
            string detail,
            float boundaryDistance,
            int priority,
            float cooldownSeconds,
            bool canTriggerNow,
            Action trigger,
            SpringDay1WorldHintBubble.HintVisualKind visualKind = SpringDay1WorldHintBubble.HintVisualKind.Interaction,
            bool forceFocus = false,
            bool showWorldIndicator = true)
        {
            if (!Application.isPlaying || anchorTarget == null || trigger == null)
            {
                return;
            }

            EnsureRuntime();
            _instance.ReportCandidateInternal(
                new Candidate(
                    anchorTarget,
                    interactionKey,
                    keyLabel,
                    caption,
                    detail,
                    boundaryDistance,
                    priority,
                    cooldownSeconds,
                    canTriggerNow,
                    forceFocus,
                    showWorldIndicator,
                    visualKind,
                    trigger));
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDisable()
        {
            ClearFocus(hideBubble: true);
            ClearPendingCandidate();
            ClearNpcPromptFocusSuppression();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            RefreshFocusFromCurrentFrame();
        }

        private void RefreshFocusFromCurrentFrame()
        {
            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen()
                || (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive))
            {
                ClearPendingCandidate();
                ClearFocus(hideBubble: true);
                return;
            }

            if (_pendingCandidateFrame != Time.frameCount || !_hasPendingCandidate)
            {
                ClearFocus(hideBubble: true);
                return;
            }

            _currentCandidate = _pendingCandidate;
            _hasCurrentCandidate = true;
            ClearPendingCandidate();
            UpdateNpcPromptFocusSuppression();
            bool shouldShowWorldIndicator = ShouldShowWorldIndicator(_currentCandidate);

            if (InteractionHintDisplaySettings.AreHintsVisible)
            {
                if (shouldShowWorldIndicator)
                {
                    SpringDay1WorldHintBubble.EnsureRuntime();
                    SpringDay1WorldHintBubble.Instance.Show(
                        _currentCandidate.AnchorTarget,
                        BuildWorldHintKeyLabel(_currentCandidate),
                        _currentCandidate.Caption,
                        BuildWorldHintDetail(_currentCandidate),
                        _currentCandidate.VisualKind);
                }
                else
                {
                    SpringDay1WorldHintBubble.HideIfExists();
                }

                if (_currentCandidate.CanTriggerNow || ShouldKeepOverlayVisibleForTeaser(_currentCandidate))
                {
                    ResolveOverlayPromptContent(_currentCandidate, out string overlayCaption, out string overlayDetail);
                    InteractionHintOverlay.EnsureRuntime();
                    InteractionHintOverlay.Instance.ShowPrompt(
                        _currentCandidate.CanTriggerNow ? _currentCandidate.KeyLabel : string.Empty,
                        overlayCaption,
                        overlayDetail);
                }
                else
                {
                    InteractionHintOverlay.HideIfExists();
                }
            }
            else
            {
                SpringDay1WorldHintBubble.HideIfExists();
                InteractionHintOverlay.HideIfExists();
            }

            TryTriggerCurrentCandidate();
        }

        private void ReportCandidateInternal(Candidate candidate)
        {
            if (_pendingCandidateFrame != Time.frameCount || !_hasPendingCandidate)
            {
                _pendingCandidate = candidate;
                _hasPendingCandidate = true;
                _pendingCandidateFrame = Time.frameCount;
                return;
            }

            if (ShouldReplaceCandidate(candidate, _pendingCandidate))
            {
                _pendingCandidate = candidate;
            }
        }

        private void TryTriggerCurrentCandidate()
        {
            if (!_hasCurrentCandidate || !_currentCandidate.CanTriggerNow)
            {
                return;
            }

            if (!Input.GetKeyDown(_currentCandidate.InteractionKey))
            {
                return;
            }

            Transform anchorTarget = _currentCandidate.AnchorTarget;
            int anchorId = anchorTarget != null ? anchorTarget.GetInstanceID() : 0;
            if (_lastTriggeredAtByAnchorId.TryGetValue(anchorId, out float lastTriggeredAt)
                && Time.unscaledTime - lastTriggeredAt < _currentCandidate.CooldownSeconds)
            {
                return;
            }

            _lastTriggeredAtByAnchorId[anchorId] = Time.unscaledTime;
            _currentCandidate.Trigger?.Invoke();
        }

        private static bool ShouldReplaceCandidate(Candidate candidate, Candidate current)
        {
            if (candidate.ForceFocus != current.ForceFocus)
            {
                return candidate.ForceFocus;
            }

            if (candidate.CanTriggerNow != current.CanTriggerNow)
            {
                return candidate.CanTriggerNow;
            }

            if (!Mathf.Approximately(candidate.BoundaryDistance, current.BoundaryDistance))
            {
                return candidate.BoundaryDistance < current.BoundaryDistance;
            }

            if (candidate.Priority != current.Priority)
            {
                return candidate.Priority > current.Priority;
            }

            int candidateId = candidate.AnchorTarget != null ? candidate.AnchorTarget.GetInstanceID() : int.MaxValue;
            int currentId = current.AnchorTarget != null ? current.AnchorTarget.GetInstanceID() : int.MaxValue;
            return candidateId < currentId;
        }

        private static string BuildWorldHintKeyLabel(Candidate candidate)
        {
            return candidate.CanTriggerNow ? candidate.KeyLabel : string.Empty;
        }

        private static string BuildWorldHintDetail(Candidate candidate)
        {
            if (candidate.CanTriggerNow)
            {
                return candidate.Detail;
            }

            return "再靠近一些";
        }

        private static bool ShouldShowWorldIndicator(Candidate candidate)
        {
            if (!candidate.ShowWorldIndicator || candidate.AnchorTarget == null)
            {
                return false;
            }

            if (IsNpcPromptTarget(candidate.AnchorTarget))
            {
                return false;
            }

            if (IsWorkbenchPromptTarget(candidate.AnchorTarget))
            {
                return false;
            }

            return !LooksLikeGenericDialoguePrompt(candidate.Caption, candidate.Detail);
        }

        private static void ResolveOverlayPromptContent(Candidate candidate, out string caption, out string detail)
        {
            caption = candidate.Caption;
            detail = candidate.Detail;

            if (!ShouldUseTaskPriorityOverlayCopy(candidate))
            {
                if (!candidate.CanTriggerNow && IsWorkbenchPromptTarget(candidate.AnchorTarget))
                {
                    caption = string.IsNullOrWhiteSpace(candidate.Caption) ? "工作台" : candidate.Caption;
                    detail = "再靠近一些，进入工作台交互范围。";
                }

                return;
            }

            caption = TaskPriorityOverlayCaption;
            detail = TaskPriorityOverlayDetail;
        }

        private static bool ShouldKeepOverlayVisibleForTeaser(Candidate candidate)
        {
            return !candidate.CanTriggerNow
                && candidate.AnchorTarget != null
                && IsWorkbenchPromptTarget(candidate.AnchorTarget);
        }

        private static bool ShouldUseTaskPriorityOverlayCopy(Candidate candidate)
        {
            if (!candidate.CanTriggerNow || candidate.AnchorTarget == null)
            {
                return false;
            }

            NPCDialogueInteractable dialogueInteractable = ResolveNpcDialogueInteractable(candidate.AnchorTarget);
            if (dialogueInteractable == null || !dialogueInteractable.isActiveAndEnabled)
            {
                return false;
            }

            if (!LooksLikeGenericDialoguePrompt(candidate.Caption, candidate.Detail))
            {
                return false;
            }

            return true;
        }

        private static bool LooksLikeGenericDialoguePrompt(string caption, string detail)
        {
            string normalizedCaption = string.IsNullOrWhiteSpace(caption) ? string.Empty : caption.Trim();
            string normalizedDetail = string.IsNullOrWhiteSpace(detail) ? string.Empty : detail.Trim();

            return normalizedCaption == "交谈"
                || normalizedCaption == "对话"
                || normalizedDetail == "按 E 开始对话";
        }

        private void ClearFocus(bool hideBubble)
        {
            _hasCurrentCandidate = false;
            ClearNpcPromptFocusSuppression();
            if (hideBubble)
            {
                SpringDay1WorldHintBubble.HideIfExists();
                InteractionHintOverlay.HideIfExists();
            }
        }

        private void ClearPendingCandidate()
        {
            _hasPendingCandidate = false;
            _pendingCandidateFrame = -1;
        }

        private void UpdateNpcPromptFocusSuppression()
        {
            if (!InteractionHintDisplaySettings.AreHintsVisible || !_hasCurrentCandidate || !_currentCandidate.CanTriggerNow)
            {
                ClearNpcPromptFocusSuppression();
                return;
            }

            NPCBubblePresenter presenter = TryResolveNpcBubblePresenter(_currentCandidate.AnchorTarget);
            if (presenter == _suppressedNpcBubblePresenter)
            {
                return;
            }

            ClearNpcPromptFocusSuppression();
            _suppressedNpcBubblePresenter = presenter;
            if (_suppressedNpcBubblePresenter != null)
            {
                _suppressedNpcBubblePresenter.SetInteractionPromptSuppressed(true);
            }
        }

        private void ClearNpcPromptFocusSuppression()
        {
            if (_suppressedNpcBubblePresenter == null)
            {
                return;
            }

            _suppressedNpcBubblePresenter.SetInteractionPromptSuppressed(false);
            _suppressedNpcBubblePresenter = null;
        }

        private static bool IsNpcPromptTarget(Transform anchorTarget)
        {
            return TryResolveNpcBubblePresenter(anchorTarget) != null
                || ResolveNpcDialogueInteractable(anchorTarget) != null
                || ResolveNpcInformalChatInteractable(anchorTarget) != null;
        }

        private static bool IsWorkbenchPromptTarget(Transform anchorTarget)
        {
            return anchorTarget != null
                && anchorTarget.GetComponentInParent<CraftingStationInteractable>() != null;
        }

        private static NPCBubblePresenter TryResolveNpcBubblePresenter(Transform anchorTarget)
        {
            if (anchorTarget == null)
            {
                return null;
            }

            return anchorTarget.GetComponent<NPCBubblePresenter>()
                ?? anchorTarget.GetComponentInParent<NPCBubblePresenter>()
                ?? anchorTarget.GetComponentInChildren<NPCBubblePresenter>(true);
        }

        private static NPCDialogueInteractable ResolveNpcDialogueInteractable(Transform anchorTarget)
        {
            if (anchorTarget == null)
            {
                return null;
            }

            return anchorTarget.GetComponent<NPCDialogueInteractable>()
                ?? anchorTarget.GetComponentInParent<NPCDialogueInteractable>()
                ?? anchorTarget.GetComponentInChildren<NPCDialogueInteractable>(true);
        }

        private static NPCInformalChatInteractable ResolveNpcInformalChatInteractable(Transform anchorTarget)
        {
            if (anchorTarget == null)
            {
                return null;
            }

            return anchorTarget.GetComponent<NPCInformalChatInteractable>()
                ?? anchorTarget.GetComponentInParent<NPCInformalChatInteractable>()
                ?? anchorTarget.GetComponentInChildren<NPCInformalChatInteractable>(true);
        }

        private string BuildFocusSummary()
        {
            if (!_hasCurrentCandidate || _currentCandidate.AnchorTarget == null)
            {
                return "none";
            }

            return $"{SanitizeSummaryText(_currentCandidate.AnchorTarget.name)}|{SanitizeSummaryText(_currentCandidate.KeyLabel)}|{SanitizeSummaryText(_currentCandidate.Caption)}|{SanitizeSummaryText(_currentCandidate.Detail)}|distance={_currentCandidate.BoundaryDistance:F2}|priority={_currentCandidate.Priority}|ready={_currentCandidate.CanTriggerNow}";
        }

        private static string SanitizeSummaryText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value
                .Replace('\n', ' ')
                .Replace('\r', ' ')
                .Replace('|', '/')
                .Trim();
        }
    }
}
