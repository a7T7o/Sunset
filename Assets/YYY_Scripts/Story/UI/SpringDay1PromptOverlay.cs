using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class SpringDay1PromptOverlay : MonoBehaviour
    {
        private const string PrefabAssetPath = "Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab";

        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private const string FontCoverageProbeText = "当前任务工作台继续制作";

        private static SpringDay1PromptOverlay _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform bridgePromptRect;
        [SerializeField] private CanvasGroup bridgePromptCanvasGroup;
        [SerializeField] private RectTransform cardRect;
        [SerializeField] private RectTransform pageRect;
        [SerializeField] private TextMeshProUGUI bridgePromptText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI focusText;
        [SerializeField] private TextMeshProUGUI footerText;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private float completionStepDuration = 0.32f;
        [SerializeField] private float pageFlipDuration = 0.42f;
        [SerializeField] private float postDialogueResumeDelay = 0.18f;
        [SerializeField] private int maxVisibleTaskRows = 1;
        [SerializeField] private bool preferStableInstantTransitions = true;

        private const float MinPageHeight = 112f;
        private const float MinLegacyPageHeight = 84f;
        private const float MaxPageHeight = 460f;
        private const float LegacyPageWidth = 316f;
        private const float LegacyBottomPadding = 10f;
        private const float LegacyFocusFooterGap = 4f;
        private const float LegacyTaskToBottomBandMinGap = 10f;
        private const int FixedHudSortingOrder = 142;
        private const float TaskCardLeftPadding = 26f;
        private const float TaskCardBaselineOffsetY = 8f;
        private const float BridgePromptOffsetX = 0f;
        private const float BridgePromptGapBelowCard = 10f;
        private const float BridgePromptWidth = 328f;
        private const float BridgePromptHeight = 34f;

        private readonly List<RowRefs> _rows = new();
        private TMP_FontAsset _fontAsset;
        private Vector2 _cardDefaultPosition;
        private float _cardDefaultHeight;
        private bool _suppressWhileDialogueActive;
        private string _manualPromptText = string.Empty;
        private string _manualPromptPhaseKey = string.Empty;
        private string _queuedPromptText = string.Empty;
        private Coroutine _visibilityCoroutine;
        private Coroutine _queuedRevealCoroutine;
        private Coroutine _transitionCoroutine;
        private PromptCardViewState _displayedState;
        private PromptCardViewState _pendingState;
        private bool _hasDisplayedState;
        private bool _externallySuppressedByModalUi;
        private float _visibilityAlpha;
        private float _boundaryFocusAlpha = 1f;
        private PageRefs _frontPage;
        private PageRefs _backPage;
        private string _displayedBridgePromptSignature = string.Empty;

        public float CurrentBoundaryFocusAlpha => _boundaryFocusAlpha;

        public static SpringDay1PromptOverlay Instance
        {
            get
            {
                if (_instance == null)
                {
                    EnsureRuntime();
                }

                return _instance;
            }
        }

        public static SpringDay1PromptOverlay CurrentRuntimeInstanceOrNull
        {
            get
            {
                if (_instance != null && !CanReuseRuntimeInstance(_instance, requireScreenOverlay: true))
                {
                    RetireRuntimeInstance(_instance);
                    _instance = null;
                }

                if (_instance != null)
                {
                    return _instance;
                }

                SpringDay1PromptOverlay existing = FindReusableRuntimeInstance(requireScreenOverlay: true);
                if (existing == null)
                {
                    return null;
                }

                _instance = existing;
                _instance.EnsureBuilt();
                RetireOtherRuntimeInstances(_instance);
                return _instance;
            }
        }

        public static void SetGlobalExternalVisibilityBlock(bool blocked)
        {
            SpringDay1PromptOverlay runtimeOverlay = CurrentRuntimeInstanceOrNull;
            if (runtimeOverlay == null)
            {
                return;
            }

            runtimeOverlay.SetExternalVisibilityBlock(blocked);
        }

        public void SetBoundaryFocusAlpha(float alpha)
        {
            float clamped = Mathf.Clamp01(alpha);
            if (Mathf.Abs(_boundaryFocusAlpha - clamped) <= 0.001f)
            {
                return;
            }

            _boundaryFocusAlpha = clamped;
            ApplyComposedCanvasGroupState();
        }

        public static void EnsureRuntime()
        {
            if (_instance != null && !CanReuseRuntimeInstance(_instance, requireScreenOverlay: true))
            {
                RetireRuntimeInstance(_instance);
                _instance = null;
            }

            RetireIncompatibleRuntimeInstances();

            if (TryUseExistingRuntimeInstance(requireScreenOverlay: true))
            {
                RetireOtherRuntimeInstances(_instance);
                return;
            }

            SpringDay1PromptOverlay prefabInstance = InstantiateRuntimePrefab();
            if (prefabInstance != null)
            {
                _instance = prefabInstance;
                _instance.EnsureBuilt();
                RetireOtherRuntimeInstances(_instance);
                _instance.FadeCanvasGroup(0f, true);
                return;
            }

            Transform parent = ResolveParent();
            GameObject root = new GameObject(
                nameof(SpringDay1PromptOverlay),
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasGroup));

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            _instance = root.AddComponent<SpringDay1PromptOverlay>();
            _instance.BuildUi();
            RetireOtherRuntimeInstances(_instance);
            _instance.FadeCanvasGroup(0f, true);
        }

        private static bool TryUseExistingRuntimeInstance(bool requireScreenOverlay)
        {
            SpringDay1PromptOverlay existing = FindReusableRuntimeInstance(requireScreenOverlay);
            if (existing == null)
            {
                return false;
            }

            _instance = existing;
            _instance.EnsureBuilt();
            _instance.FadeCanvasGroup(0f, true);
            return true;
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

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
            EventBus.Subscribe<DialogueEndEvent>(OnDialogueEnd, owner: this);
        }

        private void OnDisable()
        {
            StopVisibilityCoroutine();
            StopQueuedRevealCoroutine();
            StopTransitionCoroutine();
            EventBus.UnsubscribeAll(this);
        }

        private void LateUpdate()
        {
            EnsureBuilt();

            bool blockedByParentModalUi = SpringDay1UiLayerUtility.ShouldHidePromptOverlayForParentModalUi();
            if (_externallySuppressedByModalUi != blockedByParentModalUi)
            {
                SetExternalVisibilityBlock(blockedByParentModalUi);
            }

            if (ShouldDelayPromptDisplay())
            {
                if (_visibilityAlpha > 0.001f)
                {
                    FadeCanvasGroup(0f, false);
                }

                return;
            }

            PromptCardViewState nextState = BuildCurrentViewState();
            BridgePromptViewState bridgePromptState = BuildCurrentBridgePromptState();
            _pendingState = nextState;

            if (_pendingState == null)
            {
                ApplyBridgePromptState(null);
                FadeCanvasGroup(0f, false);
                return;
            }

            ApplyBridgePromptState(bridgePromptState);

            if (NeedsReadableContentRecovery(_pendingState))
            {
                ApplyState(_pendingState);
                _displayedState = _pendingState;
                _hasDisplayedState = true;
                FadeCanvasGroup(1f, false);
                return;
            }

            if (_hasDisplayedState && _displayedState == null)
            {
                ApplyState(_pendingState);
                _displayedState = _pendingState;
                FadeCanvasGroup(1f, false);
                return;
            }

            if (!_hasDisplayedState)
            {
                ApplyState(_pendingState);
                _displayedState = _pendingState;
                _hasDisplayedState = true;
                FadeCanvasGroup(1f, false);
                return;
            }

            if (_transitionCoroutine == null)
            {
                if (_displayedState.DisplaySignature != _pendingState.DisplaySignature)
                {
                    _transitionCoroutine = StartCoroutine(TransitionToPendingState());
                }
                else if (_displayedState.Signature != _pendingState.Signature)
                {
                    ApplyPendingStateWithoutTransition();
                }
            }

            if (_visibilityAlpha < 0.999f && _queuedRevealCoroutine == null)
            {
                FadeCanvasGroup(1f, false);
            }
        }

        public void Show(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Hide();
                return;
            }

            EnsureRuntimeObjectActive();
            EnsureBuilt();
            string currentPhaseKey = SpringDay1Director.Instance != null
                ? SpringDay1Director.Instance.BuildPromptCardModel()?.PhaseKey ?? string.Empty
                : string.Empty;
            if (_manualPromptText == text
                && string.Equals(_queuedPromptText, text)
                && string.Equals(_manualPromptPhaseKey, currentPhaseKey, StringComparison.Ordinal))
            {
                SpringDay1Director.Instance?.ShowTaskListBridgePrompt(text);
                return;
            }

            _manualPromptText = text;
            _manualPromptPhaseKey = currentPhaseKey;
            _queuedPromptText = text;
            SpringDay1Director.Instance?.ShowTaskListBridgePrompt(text);

            if (ShouldDelayPromptDisplay())
            {
                QueuePromptReveal();
                return;
            }

            RefreshPendingState();
            if (_pendingState != null && _transitionCoroutine == null)
            {
                if (_displayedState == null || _displayedState.DisplaySignature != _pendingState.DisplaySignature)
                {
                    _transitionCoroutine = StartCoroutine(TransitionToPendingState());
                }
                else if (_displayedState.Signature != _pendingState.Signature)
                {
                    ApplyPendingStateWithoutTransition();
                }
            }
            else
            {
                FadeCanvasGroup(1f, false);
            }
        }

        private void EnsureRuntimeObjectActive()
        {
            GameObject targetObject = overlayCanvas != null ? overlayCanvas.gameObject : gameObject;
            if (targetObject != null && !targetObject.activeSelf)
            {
                targetObject.SetActive(true);
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            if (!enabled)
            {
                enabled = true;
            }
        }

        public void Hide()
        {
            SpringDay1Director.Instance?.HideTaskListBridgePrompt();
            _manualPromptText = string.Empty;
            _manualPromptPhaseKey = string.Empty;
            _queuedPromptText = string.Empty;
            StopQueuedRevealCoroutine();
            RefreshPendingState();
            if (_pendingState == null)
            {
                FadeCanvasGroup(0f, false);
            }
        }

        public void SetExternalVisibilityBlock(bool blocked)
        {
            if (_externallySuppressedByModalUi == blocked)
            {
                return;
            }

            _externallySuppressedByModalUi = blocked;
            StopQueuedRevealCoroutine();
            RefreshPendingState();

            if (blocked)
            {
                FadeCanvasGroup(0f, false);
                return;
            }

            if (_pendingState == null)
            {
                FadeCanvasGroup(0f, false);
                return;
            }

            if (ShouldDelayPromptDisplay())
            {
                QueuePromptReveal();
                return;
            }

            if (!_hasDisplayedState || _displayedState == null)
            {
                ApplyState(_pendingState);
                _displayedState = _pendingState;
                _hasDisplayedState = true;
            }
            else if (_displayedState.DisplaySignature != _pendingState.DisplaySignature)
            {
                ApplyState(_pendingState);
                _displayedState = _pendingState;
            }
            else if (_displayedState.Signature != _pendingState.Signature)
            {
                ApplyPendingStateWithoutTransition();
            }

            FadeCanvasGroup(1f, false);
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            _suppressWhileDialogueActive = true;
            StopQueuedRevealCoroutine();
            FadeCanvasGroup(0f, true);
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            if (ShouldIgnoreDialogueEndEvent())
            {
                return;
            }

            _suppressWhileDialogueActive = false;
            RefreshPendingState();
            if (IsLikelyManagedByDialogueUiTransition())
            {
                return;
            }

            if (_pendingState == null)
            {
                FadeCanvasGroup(0f, true);
                return;
            }

            FadeCanvasGroup(1f, true);
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            overlayCanvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
            _fontAsset = ResolveFontAsset();

            ApplyRuntimeCanvasDefaults();
            ClearRuntimeShellForRebuild();

            cardRect = CreateRect(transform, "TaskCardRoot");
            cardRect.anchorMin = new Vector2(0f, 0.5f);
            cardRect.anchorMax = new Vector2(0f, 0.5f);
            cardRect.pivot = new Vector2(0f, 0.5f);
            cardRect.anchoredPosition = new Vector2(TaskCardLeftPadding, TaskCardBaselineOffsetY);
            cardRect.sizeDelta = new Vector2(328f, 188f);
            _cardDefaultPosition = cardRect.anchoredPosition;
            _cardDefaultHeight = cardRect.sizeDelta.y;

            Image shadowPlate = cardRect.gameObject.AddComponent<Image>();
            shadowPlate.color = new Color(0.19f, 0.14f, 0.1f, 0.9f);
            shadowPlate.raycastTarget = false;
            Shadow shadow = cardRect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.24f);
            shadow.effectDistance = new Vector2(0f, -7f);
            shadow.useGraphicAlpha = true;

            _backPage = CreatePage(cardRect, "BackPage", true);
            _frontPage = CreatePage(cardRect, "Page", false);
            pageRect = _frontPage.root;
            ApplyPageVisibility(_backPage, false);
            SetPageCurlVisible(_frontPage, false);
            SetPageCurlVisible(_backPage, false);
            CacheFrontPageRefs();
            BuildBridgePromptShell();
        }

        private void BuildBridgePromptShell()
        {
            if (cardRect == null)
            {
                return;
            }

            bridgePromptRect = CreateRect(cardRect, "BridgePromptRoot");
            RefreshBridgePromptLayout();

            Image background = bridgePromptRect.gameObject.AddComponent<Image>();
            background.color = new Color(0.16f, 0.11f, 0.08f, 0.72f);
            background.raycastTarget = false;

            Outline outline = bridgePromptRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.66f, 0.46f, 0.21f, 0.38f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;

            bridgePromptCanvasGroup = SpringDay1UiLayerUtility.EnsureComponent<CanvasGroup>(bridgePromptRect.gameObject);
            bridgePromptCanvasGroup.alpha = 0f;
            bridgePromptCanvasGroup.interactable = false;
            bridgePromptCanvasGroup.blocksRaycasts = false;

            RectTransform textRoot = CreateRect(bridgePromptRect, "BridgePromptText");
            Stretch(textRoot, new Vector2(10f, 4f), new Vector2(-10f, -4f));

            bridgePromptText = CreateText(
                textRoot,
                "Text",
                string.Empty,
                10f,
                new Color(0.93f, 0.84f, 0.67f, 1f),
                TextAlignmentOptions.MidlineLeft,
                true);
            Stretch(bridgePromptText.rectTransform, Vector2.zero, Vector2.zero);
            bridgePromptRect.gameObject.SetActive(false);
        }

        private void RefreshBridgePromptLayout()
        {
            if (bridgePromptRect == null)
            {
                return;
            }

            float targetWidth = BridgePromptWidth;
            float targetHeight = BridgePromptHeight;
            if (bridgePromptText != null)
            {
                float innerWidth = Mathf.Max(64f, targetWidth - 20f);
                Vector2 preferredSize = bridgePromptText.GetPreferredValues(bridgePromptText.text, innerWidth, 0f);
                targetHeight = Mathf.Max(BridgePromptHeight, preferredSize.y + 8f);
            }

            bridgePromptRect.anchorMin = new Vector2(0f, 0f);
            bridgePromptRect.anchorMax = new Vector2(0f, 0f);
            bridgePromptRect.pivot = new Vector2(0f, 1f);
            bridgePromptRect.anchoredPosition = new Vector2(BridgePromptOffsetX, -BridgePromptGapBelowCard);
            bridgePromptRect.sizeDelta = new Vector2(targetWidth, targetHeight);
        }

        private void EnsureBuilt()
        {
            EnsureAttachedToPreferredParent();

            if (rootRect == null)
            {
                rootRect = transform as RectTransform;
            }

            if (overlayCanvas == null)
            {
                overlayCanvas = GetComponent<Canvas>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            ApplyRuntimeCanvasDefaults();

            if (HasLivePromptBindings())
            {
                return;
            }

            if (TryBindRuntimeShell() && HasLivePromptBindings())
            {
                return;
            }

            ClearRuntimeShellForRebuild();
            BuildUi();
        }

        private bool NeedsReadableContentRecovery(PromptCardViewState state)
        {
            if (state == null || _frontPage == null)
            {
                return false;
            }

            EnsurePageTextChainReady(_frontPage);

            if (!HasReadablePrimaryText(_frontPage.titleText, state.StageLabel)
                || !HasReadablePrimaryText(_frontPage.subtitleText, state.Subtitle)
                || !HasReadablePrimaryText(_frontPage.focusText, state.FocusText)
                || !HasReadablePrimaryText(_frontPage.footerText, state.FooterText))
            {
                return true;
            }

            if (state.Items.Count == 0)
            {
                return false;
            }

            if (_frontPage.rows == null || _frontPage.rows.Count == 0)
            {
                return true;
            }

            if (_frontPage.rows.Count < state.Items.Count)
            {
                return true;
            }

            for (int index = 0; index < state.Items.Count; index++)
            {
                if (!HasReadablePromptRow(_frontPage.rows[index], state.Items[index]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasReadablePrimaryText(TextMeshProUGUI text, string expected)
        {
            if (string.IsNullOrWhiteSpace(expected))
            {
                return true;
            }

            return text != null
                && text.enabled
                && text.gameObject.activeSelf
                && text.color.a > 0.01f
                && text.rectTransform.rect.width > 2f
                && text.rectTransform.rect.height > 2f
                && CanFontRenderText(text.font, expected)
                && HasExpectedPromptText(text.text, expected);
        }

        private static bool HasReadablePromptRow(RowRefs row, PromptRowState expected)
        {
            if (!IsRowBindingAlive(row) || expected == null)
            {
                return false;
            }

            return row.root.gameObject.activeSelf
                && row.group.alpha > 0.98f
                && HasReadablePrimaryText(row.label, expected.Label)
                && HasReadablePrimaryText(row.detail, expected.Detail);
        }

        private static bool HasExpectedPromptText(string actual, string expected)
        {
            if (string.IsNullOrWhiteSpace(expected))
            {
                return true;
            }

            return string.Equals(
                NormalizePromptComparisonText(actual),
                NormalizePromptComparisonText(expected),
                StringComparison.Ordinal);
        }

        private static string NormalizePromptComparisonText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(value.Length);
            bool insideTag = false;
            bool previousWasWhitespace = false;
            for (int index = 0; index < value.Length; index++)
            {
                char current = value[index];
                if (current == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (current == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (char.IsWhiteSpace(current))
                {
                    if (!previousWasWhitespace)
                    {
                        builder.Append(' ');
                        previousWasWhitespace = true;
                    }

                    continue;
                }

                if (char.IsControl(current))
                {
                    continue;
                }

                builder.Append(current);
                previousWasWhitespace = false;
            }

            return builder.ToString().Trim();
        }

        private void ClearRuntimeShellForRebuild()
        {
            if (rootRect == null)
            {
                return;
            }

            DestroyDirectChildIfExists(rootRect, "TaskCardRoot");
            DestroyDirectChildIfExists(rootRect, "BridgePromptRoot");
            bridgePromptRect = null;
            bridgePromptCanvasGroup = null;
            bridgePromptText = null;
            cardRect = null;
            _cardDefaultPosition = Vector2.zero;
            _cardDefaultHeight = 0f;
            pageRect = null;
            titleText = null;
            subtitleText = null;
            focusText = null;
            footerText = null;
            _rows.Clear();
            _frontPage = null;
            _backPage = null;
        }

        private static void DestroyDirectChildIfExists(RectTransform parent, string childName)
        {
            RectTransform child = FindDirectChildRect(parent, childName);
            if (child == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private bool TryBindRuntimeShell()
        {
            if (rootRect == null)
            {
                rootRect = transform as RectTransform;
            }

            if (overlayCanvas == null)
            {
                overlayCanvas = GetComponent<Canvas>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_fontAsset == null)
            {
                _fontAsset = ResolveFontAsset();
            }

            if (rootRect == null || overlayCanvas == null || canvasGroup == null)
            {
                return false;
            }

            ApplyRuntimeCanvasDefaults();

            if (cardRect == null)
            {
                cardRect = FindDirectChildRect(rootRect, "TaskCardRoot");
            }
            if (cardRect == null)
            {
                return false;
            }

            if (!TryBindBridgePromptShell())
            {
                DestroyBridgePromptShell();
                BuildBridgePromptShell();
            }

            _cardDefaultPosition = cardRect.anchoredPosition;
            _cardDefaultHeight = cardRect.rect.height > 1f ? cardRect.rect.height : cardRect.sizeDelta.y;

            RectTransform frontRoot = pageRect != null ? pageRect : FindDirectChildRect(cardRect, "Page");
            if (frontRoot == null)
            {
                return false;
            }

            _frontPage = BindPage(frontRoot);
            if (_frontPage == null)
            {
                return false;
            }

            RectTransform backRoot = FindDirectChildRect(cardRect, "BackPage");
            if (backRoot == null)
            {
                backRoot = ClonePageRoot(frontRoot);
            }

            _backPage = backRoot != null ? BindPage(backRoot) : null;
            if (_backPage == null)
            {
                return false;
            }

            ApplyResolvedFontToShellTexts();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            ApplyPageVisibility(_frontPage, true);
            ApplyPageVisibility(_backPage, false);
            SetPageCurlVisible(_frontPage, false);
            SetPageCurlVisible(_backPage, false);
            CacheFrontPageRefs();
            return true;
        }

        private bool TryBindBridgePromptShell()
        {
            if (rootRect == null)
            {
                rootRect = transform as RectTransform;
            }

            if (rootRect == null)
            {
                return false;
            }

            bridgePromptRect = bridgePromptRect != null ? bridgePromptRect : FindDescendantRect(rootRect, "BridgePromptRoot");
            if (bridgePromptRect == null || cardRect == null || bridgePromptRect.parent != cardRect)
            {
                return false;
            }

            bridgePromptCanvasGroup = SpringDay1UiLayerUtility.EnsureComponent<CanvasGroup>(bridgePromptRect.gameObject);
            bridgePromptText = bridgePromptText != null
                ? bridgePromptText
                : FindDescendantComponent<TextMeshProUGUI>(bridgePromptRect, "Text");
            if (bridgePromptText == null)
            {
                return false;
            }

            bridgePromptCanvasGroup.interactable = false;
            bridgePromptCanvasGroup.blocksRaycasts = false;
            RefreshBridgePromptLayout();
            return true;
        }

        private void DestroyBridgePromptShell()
        {
            RectTransform target = bridgePromptRect != null ? bridgePromptRect : FindDescendantRect(rootRect, "BridgePromptRoot");
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target.gameObject);
            }
            else
            {
                DestroyImmediate(target.gameObject);
            }

            bridgePromptRect = null;
            bridgePromptCanvasGroup = null;
            bridgePromptText = null;
        }

        private void ApplyRuntimeCanvasDefaults()
        {
            if (overlayCanvas != null)
            {
                if (TryGetParentCanvas(out Canvas parentCanvas))
                {
                    overlayCanvas.renderMode = parentCanvas.renderMode;
                    overlayCanvas.worldCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                        ? null
                        : parentCanvas.worldCamera;
                    overlayCanvas.planeDistance = parentCanvas.planeDistance > 0f
                        ? parentCanvas.planeDistance
                        : 100f;
                    overlayCanvas.overrideSorting = false;
                    overlayCanvas.sortingOrder = parentCanvas.sortingOrder;
                    overlayCanvas.sortingLayerID = parentCanvas.sortingLayerID;
                    overlayCanvas.pixelPerfect = parentCanvas.pixelPerfect;
                    overlayCanvas.targetDisplay = parentCanvas.targetDisplay;
                }
                else
                {
                    // 没有父级 HUD Canvas 时，才退回独立 screen-overlay 壳兜底。
                    overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    overlayCanvas.worldCamera = null;
                    overlayCanvas.planeDistance = 100f;
                    overlayCanvas.overrideSorting = true;
                    overlayCanvas.sortingOrder = FixedHudSortingOrder;
                    overlayCanvas.pixelPerfect = true;
                }
            }

            if (rootRect != null)
            {
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;
                rootRect.localScale = Vector3.one;
                rootRect.localRotation = Quaternion.identity;
            }

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private PageRefs BindPage(RectTransform pageRoot)
        {
            if (pageRoot == null)
            {
                return null;
            }

            PageRefs page = new PageRefs
            {
                root = pageRoot,
                canvasGroup = SpringDay1UiLayerUtility.EnsureComponent<CanvasGroup>(pageRoot.gameObject),
                contentRoot = FindDirectChildRect(pageRoot, "ContentRoot"),
                titleText = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "TitleText"),
                subtitleText = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "SubtitleText"),
                focusText = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FocusText"),
                footerText = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FooterText"),
                defaultPosition = pageRoot.anchoredPosition,
                defaultPivot = pageRoot.pivot,
                defaultHeight = pageRoot.rect.height > 1f ? pageRoot.rect.height : pageRoot.sizeDelta.y
            };

            page.taskListRoot = page.contentRoot != null
                ? FindDirectChildRect(page.contentRoot, "TaskList")
                : FindDirectChildRect(pageRoot, "TaskList");
            page.pageCurl = EnsurePageCurl(pageRoot);
            page.pageCurlImage = page.pageCurl.GetComponent<Image>();

            if (page.titleText == null || page.subtitleText == null || page.focusText == null || page.footerText == null || page.taskListRoot == null || page.pageCurl == null)
            {
                return null;
            }

            if (page.contentRoot == null)
            {
                page.usesLegacyManualLayout = true;
                page.contentRoot = page.root;
                PrepareLegacyPage(page);
            }

            page.stageTagRoot ??= FindDirectChildRect(page.contentRoot, "StageTag") ?? FindDirectChildRect(pageRoot, "StageTag") ?? ResolveLegacySectionRoot(pageRoot, null, page.titleText);
            page.subtitleRoot ??= FindDirectChildRect(page.contentRoot, "SubtitleRoot") ?? ResolveLegacySectionRoot(pageRoot, "SubtitleText", page.subtitleText);
            page.headerDividerRoot ??= FindDirectChildRect(page.contentRoot, "HeaderDivider") ?? FindDirectChildRect(pageRoot, "HeaderDivider");
            page.focusRibbonRoot ??= FindDirectChildRect(page.contentRoot, "FocusRibbon") ?? ResolveLegacySectionRoot(pageRoot, "FocusText", page.focusText);
            page.footerRoot ??= FindDirectChildRect(page.contentRoot, "FooterRoot") ?? ResolveLegacySectionRoot(pageRoot, "FooterText", page.footerText);

            BindExistingRows(page);
            return page;
        }

        private void BindExistingRows(PageRefs page)
        {
            page.rows.Clear();
            if (page.taskListRoot == null)
            {
                return;
            }

            List<RowRefs> boundRows = new();
            for (int index = 0; index < page.taskListRoot.childCount; index++)
            {
                RectTransform rowRect = page.taskListRoot.GetChild(index) as RectTransform;
                if (rowRect == null || !rowRect.name.StartsWith("TaskRow_"))
                {
                    continue;
                }

                RowRefs row = BindRow(rowRect);
                if (row != null)
                {
                    boundRows.Add(row);
                }
            }

            boundRows.Sort((left, right) => ParseTrailingIndex(left.root.name).CompareTo(ParseTrailingIndex(right.root.name)));
            page.rows.AddRange(boundRows);
        }

        private RowRefs BindRow(RectTransform rowRect)
        {
            if (rowRect == null)
            {
                return null;
            }

            CanvasGroup group = SpringDay1UiLayerUtility.EnsureComponent<CanvasGroup>(rowRect.gameObject);
            Image plate = rowRect.GetComponent<Image>();
            Image bulletFill = FindDescendantComponent<Image>(rowRect, "BulletFill");
            TextMeshProUGUI label = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Label");
            TextMeshProUGUI detail = FindDescendantComponent<TextMeshProUGUI>(rowRect, "Detail");
            if (plate == null || bulletFill == null || label == null || detail == null)
            {
                return null;
            }

            return new RowRefs
            {
                root = rowRect,
                group = group,
                plate = plate,
                bulletFill = bulletFill,
                bulletRoot = FindDescendantRect(rowRect, "Bullet"),
                label = label,
                labelRect = label.rectTransform,
                detail = detail,
                detailRect = detail.rectTransform,
                usesLegacyManualLayout = rowRect.GetComponent<HorizontalLayoutGroup>() == null
            };
        }

        private RectTransform ClonePageRoot(RectTransform template)
        {
            if (template == null || cardRect == null)
            {
                return null;
            }

            RectTransform clone = Instantiate(template, cardRect);
            clone.name = "BackPage";
            clone.SetSiblingIndex(0);
            return clone;
        }

        private void PrepareLegacyPage(PageRefs page)
        {
            if (page == null)
            {
                return;
            }

            NormalizeLegacyPageRoot(page);
            page.stageTagRoot ??= FindDirectChildRect(page.root, "StageTag") ?? ResolveLegacySectionRoot(page.root, null, page.titleText);
            page.subtitleRoot ??= ResolveLegacySectionRoot(page.root, "SubtitleText", page.subtitleText);
            page.headerDividerRoot ??= FindDirectChildRect(page.root, "HeaderDivider");
            page.focusRibbonRoot ??= ResolveLegacySectionRoot(page.root, "FocusText", page.focusText);
            page.footerRoot ??= ResolveLegacySectionRoot(page.root, "FooterText", page.footerText);

            page.titleText.textWrappingMode = TextWrappingModes.NoWrap;
            page.titleText.overflowMode = TextOverflowModes.Ellipsis;
            page.subtitleText.textWrappingMode = TextWrappingModes.Normal;
            page.subtitleText.overflowMode = TextOverflowModes.Overflow;
            page.focusText.textWrappingMode = TextWrappingModes.Normal;
            page.focusText.overflowMode = TextOverflowModes.Overflow;
            page.footerText.textWrappingMode = TextWrappingModes.Normal;
            page.footerText.overflowMode = TextOverflowModes.Overflow;
            page.footerBaselineTop = page.footerRoot != null ? GetTopInParent(page.footerRoot) : 0f;
            page.footerDefaultHeight = page.footerRoot != null
                ? Mathf.Clamp(
                    page.footerRoot.rect.height > 1f ? page.footerRoot.rect.height : page.footerRoot.sizeDelta.y,
                    14f,
                    20f)
                : 14f;
            page.defaultPosition = page.root.anchoredPosition;
            page.defaultPivot = page.root.pivot;
            page.defaultHeight = page.root.rect.height > 1f ? page.root.rect.height : page.root.sizeDelta.y;
        }

        private void NormalizeLegacyPageRoot(PageRefs page)
        {
            if (page?.root == null || cardRect == null)
            {
                return;
            }

            float width = page.root.rect.width > 1f
                ? page.root.rect.width
                : (cardRect.rect.width > 1f ? cardRect.rect.width : Mathf.Max(1f, page.root.sizeDelta.x));
            float height = page.root.rect.height > 1f
                ? page.root.rect.height
                : (cardRect.rect.height > 1f ? cardRect.rect.height : Mathf.Max(MinLegacyPageHeight, page.root.sizeDelta.y));

            page.root.anchorMin = new Vector2(0f, 0.5f);
            page.root.anchorMax = new Vector2(0f, 0.5f);
            page.root.pivot = new Vector2(0f, 0.5f);
            page.root.anchoredPosition = Vector2.zero;
            page.root.sizeDelta = new Vector2(width, height);
        }

        private static RectTransform ResolveLegacySectionRoot(RectTransform pageRoot, string directChildName, TextMeshProUGUI text)
        {
            if (pageRoot == null)
            {
                return text != null ? text.rectTransform : null;
            }

            if (!string.IsNullOrWhiteSpace(directChildName))
            {
                RectTransform directChild = FindDirectChildRect(pageRoot, directChildName);
                if (directChild != null)
                {
                    return directChild;
                }
            }

            if (text == null)
            {
                return null;
            }

            RectTransform textRect = text.rectTransform;
            RectTransform parentRect = textRect.parent as RectTransform;
            return parentRect == pageRoot || parentRect == null ? textRect : parentRect;
        }

        private RectTransform EnsurePageCurl(RectTransform pageRoot)
        {
            RectTransform pageCurl = FindDirectChildRect(pageRoot, "PageCurl");
            if (pageCurl == null)
            {
                pageCurl = CreateRect(pageRoot, "PageCurl");
                pageCurl.anchorMin = new Vector2(1f, 0f);
                pageCurl.anchorMax = new Vector2(1f, 0f);
                pageCurl.pivot = new Vector2(1f, 0f);
                pageCurl.anchoredPosition = new Vector2(-8f, 8f);
                pageCurl.sizeDelta = new Vector2(24f, 24f);
                pageCurl.localRotation = Quaternion.Euler(0f, 0f, 45f);
            }

            Image curlImage = SpringDay1UiLayerUtility.EnsureComponent<Image>(pageCurl.gameObject);
            curlImage.color = new Color(0.88f, 0.77f, 0.58f, 0.46f);
            curlImage.raycastTarget = false;
            pageCurl.gameObject.SetActive(false);
            return pageCurl;
        }

        private static void SetPageCurlVisible(PageRefs page, bool visible)
        {
            if (page == null || page.pageCurl == null || page.pageCurlImage == null)
            {
                return;
            }

            page.pageCurl.gameObject.SetActive(visible);
            page.pageCurlImage.enabled = visible;
        }

        private void RefreshLegacyPageLayout(PageRefs page)
        {
            if (page == null || !page.usesLegacyManualLayout)
            {
                return;
            }

            float pageWidth = page.root.rect.width > 1f ? page.root.rect.width : LegacyPageWidth;
            float minimumPageHeight = GetLegacyMinimumPageHeight(page);
            float currentTop = 4f;

            if (page.stageTagRoot != null)
            {
                SetTopKeepingHorizontal(page.stageTagRoot, currentTop, Mathf.Max(22f, page.stageTagRoot.rect.height));
                currentTop += Mathf.Max(28f, page.stageTagRoot.rect.height + 6f);
            }

            bool hasSubtitle = page.subtitleRoot != null && !string.IsNullOrWhiteSpace(page.subtitleText.text);
            if (page.subtitleRoot != null)
            {
                page.subtitleRoot.gameObject.SetActive(hasSubtitle);
            }

            if (hasSubtitle)
            {
                float subtitleWidth = page.subtitleRoot.rect.width > 1f
                    ? page.subtitleRoot.rect.width
                    : pageWidth - 36f;
                float subtitleHeight = MeasureTextHeight(page.subtitleText, subtitleWidth, 18f);
                SetTopKeepingHorizontal(page.subtitleRoot, currentTop, subtitleHeight);
                currentTop += subtitleHeight + 6f;
            }

            float taskWidth = page.taskListRoot != null && page.taskListRoot.rect.width > 1f
                ? page.taskListRoot.rect.width
                : pageWidth - 36f;
            float taskHeight = LayoutLegacyRows(page, taskWidth);
            bool hasTasks = taskHeight > 0.01f;
            bool hasFocus = page.focusRibbonRoot != null && !string.IsNullOrWhiteSpace(page.focusText.text);
            bool hasFooter = page.footerRoot != null && !string.IsNullOrWhiteSpace(page.footerText.text);
            bool hasDivider = page.headerDividerRoot != null && (hasSubtitle || hasTasks || hasFocus || hasFooter);
            if (page.headerDividerRoot != null)
            {
                page.headerDividerRoot.gameObject.SetActive(hasDivider);
            }

            if (hasDivider)
            {
                SetTopKeepingHorizontal(page.headerDividerRoot, currentTop, Mathf.Max(2f, page.headerDividerRoot.rect.height));
                currentTop += Mathf.Max(8f, page.headerDividerRoot.rect.height + 6f);
            }

            if (page.taskListRoot != null)
            {
                page.taskListRoot.gameObject.SetActive(hasTasks);
                if (hasTasks)
                {
                    SetTopKeepingHorizontal(page.taskListRoot, currentTop, taskHeight);
                }
            }

            if (hasTasks)
            {
                currentTop += taskHeight + 2f;
            }

            float focusHeight = 0f;
            if (page.focusRibbonRoot != null)
            {
                page.focusRibbonRoot.gameObject.SetActive(hasFocus);
            }

            if (hasFocus)
            {
                float focusWidth = page.focusRibbonRoot.rect.width > 1f
                    ? Mathf.Max(1f, page.focusRibbonRoot.rect.width - 24f)
                    : pageWidth - 58f;
                float focusTextHeight = MeasureTextHeight(page.focusText, focusWidth, 12f);
                focusHeight = Mathf.Max(18f, focusTextHeight + 6f);
            }

            float footerWidth = page.footerRoot != null && page.footerRoot.rect.width > 1f
                ? page.footerRoot.rect.width
                : pageWidth - 44f;
            float footerMinHeight = string.IsNullOrWhiteSpace(page.footerText.text)
                ? 12f
                : Mathf.Max(14f, page.footerDefaultHeight > 0.01f ? page.footerDefaultHeight : 14f);
            float footerHeight = MeasureTextHeight(page.footerText, footerWidth, footerMinHeight);
            if (page.footerRoot != null)
            {
                page.footerRoot.gameObject.SetActive(hasFooter);
            }

            float bottomBandHeight = 0f;
            if (hasFocus)
            {
                bottomBandHeight += focusHeight;
            }

            if (hasFocus && hasFooter)
            {
                bottomBandHeight += LegacyFocusFooterGap;
            }

            if (hasFooter)
            {
                bottomBandHeight += footerHeight;
            }

            float minimumTaskGap = hasTasks && bottomBandHeight > 0.01f ? LegacyTaskToBottomBandMinGap : 0f;
            float preferredContentHeight = Mathf.Max(
                currentTop + minimumTaskGap + bottomBandHeight + LegacyBottomPadding,
                ResolveLegacyVisibleHeight(page));
            float preferredHeight = preferredContentHeight + GetLegacyPageShellInset(page);
            float legacyHeightCap = Mathf.Min(
                MaxPageHeight,
                Mathf.Max(
                    minimumPageHeight,
                    (page.defaultHeight > 0.01f ? page.defaultHeight : MinPageHeight) + 52f));
            page.preferredHeight = Mathf.Clamp(preferredHeight, minimumPageHeight, legacyHeightCap);

            float contentHeight = Mathf.Max(0f, page.preferredHeight - GetLegacyPageShellInset(page));
            float bottomCursor = Mathf.Max(currentTop + minimumTaskGap, contentHeight - LegacyBottomPadding);
            if (hasFooter && page.footerRoot != null)
            {
                float footerTop = Mathf.Max(currentTop + minimumTaskGap, bottomCursor - footerHeight);
                SetTopKeepingHorizontal(page.footerRoot, footerTop, footerHeight);
                bottomCursor = footerTop - LegacyFocusFooterGap;
            }

            if (hasFocus && page.focusRibbonRoot != null)
            {
                float focusTop = Mathf.Max(currentTop + minimumTaskGap, bottomCursor - focusHeight);
                SetTopKeepingHorizontal(page.focusRibbonRoot, focusTop, focusHeight);
            }
        }

        private float LayoutLegacyRows(PageRefs page, float rowWidth)
        {
            if (page.taskListRoot == null)
            {
                return 30f;
            }

            float currentTop = 0f;
            int activeCount = 0;
            for (int index = 0; index < page.rows.Count; index++)
            {
                RowRefs row = page.rows[index];
                if (row == null || row.root == null || !row.root.gameObject.activeSelf)
                {
                    continue;
                }

                float rowHeight = RefreshLegacyRowLayout(row, rowWidth);
                SetTopKeepingHorizontal(row.root, currentTop, rowHeight);
                currentTop += rowHeight + 8f;
                activeCount++;
            }

            if (activeCount == 0)
            {
                return 0f;
            }

            currentTop -= 8f;
            return Mathf.Max(30f, currentTop);
        }

        private float RefreshLegacyRowLayout(RowRefs row, float rowWidth)
        {
            if (row == null || row.root == null)
            {
                return 40f;
            }

            if (!row.usesLegacyManualLayout)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(row.root);
                return Mathf.Max(40f, row.root.rect.height);
            }

            float width = Mathf.Max(180f, rowWidth);
            float textWidth = row.labelRect != null && row.labelRect.rect.width > 1f
                ? row.labelRect.rect.width
                : Mathf.Max(72f, width - 48f);
            float labelHeight = MeasureTextHeight(row.label, textWidth, 16f);
            float detailHeight = string.IsNullOrWhiteSpace(row.detail.text) ? 0f : MeasureTextHeight(row.detail, textWidth, 14f);
            float minimumRowHeight = detailHeight > 0f ? 50f : 38f;
            float rowHeight = Mathf.Max(minimumRowHeight, 8f + labelHeight + (detailHeight > 0f ? 2f + detailHeight : 0f) + 8f);

            if (row.bulletRoot != null)
            {
                SetTopKeepingHorizontal(row.bulletRoot, 10f, Mathf.Max(18f, row.bulletRoot.rect.height));
            }

            if (row.labelRect != null)
            {
                SetTopKeepingHorizontal(row.labelRect, 8f, labelHeight);
            }

            if (row.detailRect != null)
            {
                row.detailRect.gameObject.SetActive(detailHeight > 0.01f);
                if (detailHeight > 0.01f)
                {
                    SetTopKeepingHorizontal(row.detailRect, 10f + labelHeight, detailHeight);
                }
            }

            row.preferredHeight = rowHeight;
            return rowHeight;
        }

        private void ApplyResolvedFontToShellTexts()
        {
            if (rootRect == null)
            {
                return;
            }

            TextMeshProUGUI[] texts = rootRect.GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int index = 0; index < texts.Length; index++)
            {
                TextMeshProUGUI text = texts[index];
                if (text == null)
                {
                    continue;
                }

                TMP_FontAsset resolvedFont = ResolveFontAsset(text.text);
                if (resolvedFont == null)
                {
                    continue;
                }

                text.font = resolvedFont;
                if (resolvedFont.material != null)
                {
                    text.fontSharedMaterial = resolvedFont.material;
                }

                text.ForceMeshUpdate();
            }
        }

        private float MeasureTextHeight(TextMeshProUGUI text, float width, float minHeight)
        {
            if (text == null)
            {
                return minHeight;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFontAsset(text.text);
                if (_fontAsset != null)
                {
                    text.font = _fontAsset;
                    if (_fontAsset.material != null)
                    {
                        text.fontSharedMaterial = _fontAsset.material;
                    }
                }
            }

            Vector2 preferred = text.GetPreferredValues(text.text, Mathf.Max(1f, width), 0f);
            return Mathf.Max(minHeight, Mathf.Ceil(preferred.y));
        }

        private static float GetTopInParent(RectTransform rect)
        {
            RectTransform parent = rect != null ? rect.parent as RectTransform : null;
            if (rect == null || parent == null)
            {
                return 0f;
            }

            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector3 topLeft = parent.InverseTransformPoint(corners[1]);
            return parent.rect.yMax - topLeft.y;
        }

        private static float GetCurrentHeight(RectTransform rect, float fallback)
        {
            return rect != null && rect.rect.height > 0.01f ? rect.rect.height : fallback;
        }

        private static void SetTopKeepingHorizontal(RectTransform rect, float top, float height)
        {
            if (rect == null)
            {
                return;
            }

            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            Vector2 pivot = rect.pivot;
            bool stretchesVertically = Mathf.Abs(anchorMin.y - anchorMax.y) > 0.001f;

            rect.anchorMin = new Vector2(anchorMin.x, 1f);
            rect.anchorMax = new Vector2(anchorMax.x, 1f);
            rect.pivot = new Vector2(pivot.x, 1f);

            if (stretchesVertically)
            {
                Vector2 offsetMin = rect.offsetMin;
                Vector2 offsetMax = rect.offsetMax;
                rect.offsetMin = new Vector2(offsetMin.x, -top - height);
                rect.offsetMax = new Vector2(offsetMax.x, -top);
                return;
            }

            Vector2 anchoredPosition = rect.anchoredPosition;
            Vector2 sizeDelta = rect.sizeDelta;
            rect.anchoredPosition = new Vector2(anchoredPosition.x, -top);
            rect.sizeDelta = new Vector2(sizeDelta.x, height);
        }

        private RowRefs CreateRow(Transform parent, int index)
        {
            RectTransform rowRect = CreateRect(parent, $"TaskRow_{index}");
            Image linePlate = rowRect.gameObject.AddComponent<Image>();
            linePlate.color = new Color(1f, 1f, 1f, 0.04f);
            linePlate.raycastTarget = false;
            LayoutElement layoutElement = rowRect.gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 40f;
            ContentSizeFitter rowFitter = rowRect.gameObject.AddComponent<ContentSizeFitter>();
            rowFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rowFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            HorizontalLayoutGroup rowLayout = rowRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.padding = new RectOffset(10, 10, 8, 8);
            rowLayout.spacing = 10f;
            rowLayout.childAlignment = TextAnchor.UpperLeft;
            rowLayout.childControlWidth = false;
            rowLayout.childControlHeight = false;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;

            CanvasGroup rowGroup = rowRect.gameObject.AddComponent<CanvasGroup>();
            rowGroup.interactable = false;
            rowGroup.blocksRaycasts = false;

            RectTransform bulletRect = CreateRect(rowRect, "Bullet");
            LayoutElement bulletLayout = bulletRect.gameObject.AddComponent<LayoutElement>();
            bulletLayout.minWidth = 18f;
            bulletLayout.preferredWidth = 18f;
            bulletLayout.minHeight = 18f;
            bulletLayout.preferredHeight = 18f;
            Image bulletFrame = bulletRect.gameObject.AddComponent<Image>();
            bulletFrame.color = new Color(0.45f, 0.31f, 0.18f, 0.18f);
            bulletFrame.raycastTarget = false;

            RectTransform bulletFillRect = CreateRect(bulletRect, "BulletFill");
            Stretch(bulletFillRect, new Vector2(3f, 3f), new Vector2(-3f, -3f));
            Image bulletFill = bulletFillRect.gameObject.AddComponent<Image>();
            bulletFill.color = new Color(0.44f, 0.66f, 0.46f, 0f);
            bulletFill.raycastTarget = false;

            RectTransform textColumn = CreateRect(rowRect, "TextColumn");
            LayoutElement textLayoutElement = textColumn.gameObject.AddComponent<LayoutElement>();
            textLayoutElement.flexibleWidth = 1f;
            ContentSizeFitter textFitter = textColumn.gameObject.AddComponent<ContentSizeFitter>();
            textFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            VerticalLayoutGroup textLayout = textColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            textLayout.spacing = 2f;
            textLayout.padding = new RectOffset(0, 0, 0, 0);
            textLayout.childAlignment = TextAnchor.UpperLeft;
            textLayout.childControlWidth = true;
            textLayout.childControlHeight = false;
            textLayout.childForceExpandWidth = true;
            textLayout.childForceExpandHeight = false;

            TextMeshProUGUI label = CreateText(textColumn, "Label", string.Empty, 14f, new Color(0.2f, 0.17f, 0.12f, 1f), TextAlignmentOptions.TopLeft, true);
            label.fontStyle = FontStyles.Bold;
            label.margin = new Vector4(0f, 0f, 0f, 1f);
            label.gameObject.AddComponent<LayoutElement>().minHeight = 16f;

            TextMeshProUGUI detail = CreateText(textColumn, "Detail", string.Empty, 11f, new Color(0.42f, 0.31f, 0.18f, 0.92f), TextAlignmentOptions.TopLeft, true);
            detail.gameObject.AddComponent<LayoutElement>().minHeight = 14f;

            return new RowRefs
            {
                root = rowRect,
                group = rowGroup,
                plate = linePlate,
                bulletFill = bulletFill,
                label = label,
                detail = detail
            };
        }

        private PageRefs CreatePage(Transform parent, string name, bool isBackPage)
        {
            PageRefs page = new PageRefs();
            page.root = CreateRect(parent, name);
            page.root.anchorMin = new Vector2(0f, 0.5f);
            page.root.anchorMax = new Vector2(0f, 0.5f);
            page.root.pivot = new Vector2(0f, 0.5f);
            page.root.anchoredPosition = isBackPage ? new Vector2(6f, -4f) : Vector2.zero;
            page.root.sizeDelta = new Vector2(316f, MinPageHeight);
            page.canvasGroup = page.root.gameObject.AddComponent<CanvasGroup>();

            Image pageImage = page.root.gameObject.AddComponent<Image>();
            pageImage.color = new Color(0.95f, 0.9f, 0.78f, 0.93f);
            pageImage.raycastTarget = false;
            Outline pageOutline = page.root.gameObject.AddComponent<Outline>();
            pageOutline.effectColor = new Color(0.26f, 0.18f, 0.08f, 0.22f);
            pageOutline.effectDistance = new Vector2(1f, -1f);
            pageOutline.useGraphicAlpha = true;

            RectTransform strap = CreateRect(page.root, "Strap");
            strap.anchorMin = new Vector2(0f, 0f);
            strap.anchorMax = new Vector2(0f, 1f);
            strap.pivot = new Vector2(0f, 0.5f);
            strap.sizeDelta = new Vector2(8f, 0f);
            strap.anchoredPosition = Vector2.zero;
            Image strapImage = strap.gameObject.AddComponent<Image>();
            strapImage.color = new Color(0.71f, 0.44f, 0.2f, 0.78f);
            strapImage.raycastTarget = false;

            RectTransform content = CreateRect(page.root, "ContentRoot");
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(1f, 1f);
            content.offsetMin = new Vector2(18f, 14f);
            content.offsetMax = new Vector2(-16f, -14f);
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(4, 4, 0, 0);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform headerRow = CreateRect(content, "HeaderRow");
            HorizontalLayoutGroup headerLayout = headerRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 8f;
            headerLayout.childAlignment = TextAnchor.MiddleLeft;
            headerLayout.childControlWidth = false;
            headerLayout.childControlHeight = false;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;
            headerRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 22f;

            RectTransform tag = CreateRect(headerRow, "StageTag");
            LayoutElement tagElement = tag.gameObject.AddComponent<LayoutElement>();
            tagElement.preferredWidth = 132f;
            tagElement.preferredHeight = 22f;
            Image tagImage = tag.gameObject.AddComponent<Image>();
            tagImage.color = new Color(0.56f, 0.32f, 0.14f, 0.2f);
            tagImage.raycastTarget = false;

            page.titleText = CreateText(tag, "TitleText", "Day1 任务页", 11f, new Color(0.44f, 0.24f, 0.08f, 1f), TextAlignmentOptions.Center);
            Stretch(page.titleText.rectTransform, Vector2.zero, Vector2.zero);

            RectTransform subtitleRoot = CreateRect(content, "SubtitleRoot");
            ContentSizeFitter subtitleFitter = subtitleRoot.gameObject.AddComponent<ContentSizeFitter>();
            subtitleFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            subtitleFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            subtitleRoot.gameObject.AddComponent<LayoutElement>().minHeight = 0f;
            page.subtitleText = CreateText(subtitleRoot, "SubtitleText", string.Empty, 13f, new Color(0.25f, 0.2f, 0.14f, 0.96f), TextAlignmentOptions.TopLeft, true);
            Stretch(page.subtitleText.rectTransform, Vector2.zero, Vector2.zero);
            page.subtitleText.fontStyle = FontStyles.Bold;
            page.subtitleText.margin = new Vector4(0f, 0f, 0f, 1f);

            RectTransform divider = CreateRect(content, "HeaderDivider");
            divider.gameObject.AddComponent<LayoutElement>().preferredHeight = 2f;
            Image dividerImage = divider.gameObject.AddComponent<Image>();
            dividerImage.color = new Color(0.35f, 0.24f, 0.12f, 0.12f);
            dividerImage.raycastTarget = false;

            page.taskListRoot = CreateRect(content, "TaskList");
            VerticalLayoutGroup taskLayout = page.taskListRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            taskLayout.spacing = 8f;
            taskLayout.childAlignment = TextAnchor.UpperLeft;
            taskLayout.childControlWidth = true;
            taskLayout.childControlHeight = false;
            taskLayout.childForceExpandWidth = true;
            taskLayout.childForceExpandHeight = false;
            ContentSizeFitter taskFitter = page.taskListRoot.gameObject.AddComponent<ContentSizeFitter>();
            taskFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            taskFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            page.taskListRoot.gameObject.AddComponent<LayoutElement>().minHeight = 0f;

            RectTransform focusRibbon = CreateRect(content, "FocusRibbon");
            focusRibbon.gameObject.AddComponent<LayoutElement>().minHeight = 0f;
            Image focusRibbonImage = focusRibbon.gameObject.AddComponent<Image>();
            focusRibbonImage.color = new Color(0.78f, 0.67f, 0.48f, 0.42f);
            focusRibbonImage.raycastTarget = false;
            Outline focusOutline = focusRibbon.gameObject.AddComponent<Outline>();
            focusOutline.effectColor = new Color(0.35f, 0.24f, 0.12f, 0.12f);
            focusOutline.effectDistance = new Vector2(1f, -1f);
            focusOutline.useGraphicAlpha = true;
            page.focusText = CreateText(focusRibbon, "FocusText", string.Empty, 11f, new Color(0.28f, 0.2f, 0.12f, 1f), TextAlignmentOptions.Left, true);
            Stretch(page.focusText.rectTransform, new Vector2(12f, 4f), new Vector2(-12f, -4f));

            RectTransform footerRoot = CreateRect(content, "FooterRoot");
            ContentSizeFitter footerFitter = footerRoot.gameObject.AddComponent<ContentSizeFitter>();
            footerFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            footerFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            footerRoot.gameObject.AddComponent<LayoutElement>().minHeight = 0f;
            page.footerText = CreateText(footerRoot, "FooterText", string.Empty, 10f, new Color(0.42f, 0.31f, 0.18f, 0.92f), TextAlignmentOptions.BottomLeft, true);
            Stretch(page.footerText.rectTransform, Vector2.zero, Vector2.zero);

            page.pageCurl = EnsurePageCurl(page.root);
            page.pageCurlImage = page.pageCurl.GetComponent<Image>();

            page.contentRoot = content;
            page.defaultPosition = page.root.anchoredPosition;
            page.defaultPivot = page.root.pivot;
            page.defaultHeight = page.root.sizeDelta.y;
            return page;
        }

        private PromptCardViewState BuildCurrentViewState()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            if (director == null)
            {
                return string.IsNullOrWhiteSpace(_manualPromptText)
                    ? null
                    : BuildManualState(_manualPromptText);
            }

            SpringDay1Director.TaskListVisibilitySemanticState visibilityState = director.GetTaskListVisibilitySemanticState();
            if (visibilityState.ManagedByDay1 && visibilityState.ForceHidden)
            {
                ClearStaleLocalManualPrompt(string.Empty);
                return null;
            }

            SpringDay1Director.PromptCardModel model = director.BuildPromptCardModel();
            if (model == null)
            {
                ClearStaleLocalManualPrompt(string.Empty);
                return null;
            }

            ClearStaleLocalManualPrompt(model.PhaseKey);

            return PromptCardViewState.FromModel(
                model,
                model.FocusText,
                maxVisibleItems: Mathf.Max(1, maxVisibleTaskRows));
        }

        private BridgePromptViewState BuildCurrentBridgePromptState()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null)
            {
                SpringDay1Director.TaskListBridgePromptDisplayState directorState = director.GetTaskListBridgePromptDisplayState();
                if (directorState.Visible)
                {
                    return new BridgePromptViewState(directorState.Text, directorState.SemanticKey);
                }
            }

            SpringDay1Director.PromptCardModel currentModel = director != null ? director.BuildPromptCardModel() : null;
            string currentPhaseKey = currentModel?.PhaseKey ?? string.Empty;
            ClearStaleLocalManualPrompt(currentPhaseKey);
            if (string.IsNullOrWhiteSpace(_manualPromptText))
            {
                return null;
            }

            if (currentModel != null && IsRedundantBridgePrompt(currentModel, _manualPromptText))
            {
                return null;
            }

            return new BridgePromptViewState(
                _manualPromptText,
                $"legacy-bridge:{currentPhaseKey}:{_manualPromptText}");
        }

        private void ClearStaleLocalManualPrompt(string currentPhaseKey)
        {
            if (string.IsNullOrWhiteSpace(_manualPromptText)
                || string.IsNullOrWhiteSpace(_manualPromptPhaseKey)
                || string.Equals(_manualPromptPhaseKey, currentPhaseKey, StringComparison.Ordinal))
            {
                return;
            }

            _manualPromptText = string.Empty;
            _manualPromptPhaseKey = string.Empty;
            _queuedPromptText = string.Empty;
        }

        private static bool IsRedundantBridgePrompt(SpringDay1Director.PromptCardModel model, string bridgePromptText)
        {
            if (model == null || string.IsNullOrWhiteSpace(bridgePromptText))
            {
                return false;
            }

            string normalizedBridgePrompt = NormalizePromptComparisonText(bridgePromptText);
            if (string.IsNullOrEmpty(normalizedBridgePrompt))
            {
                return true;
            }

            if (ContainsPromptOverlap(model.FocusText, normalizedBridgePrompt)
                || ContainsPromptOverlap(model.Subtitle, normalizedBridgePrompt)
                || ContainsPromptOverlap(model.FooterText, normalizedBridgePrompt))
            {
                return true;
            }

            if (model.Items == null)
            {
                return false;
            }

            for (int index = 0; index < model.Items.Length; index++)
            {
                SpringDay1Director.PromptTaskItem item = model.Items[index];
                if (ContainsPromptOverlap(item.Label, normalizedBridgePrompt)
                    || ContainsPromptOverlap(item.Detail, normalizedBridgePrompt))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsPromptOverlap(string source, string normalizedNeedle)
        {
            if (string.IsNullOrEmpty(normalizedNeedle))
            {
                return true;
            }

            string normalizedSource = NormalizePromptComparisonText(source);
            if (string.IsNullOrEmpty(normalizedSource))
            {
                return false;
            }

            if (normalizedSource.IndexOf(normalizedNeedle, StringComparison.Ordinal) >= 0
                || normalizedNeedle.IndexOf(normalizedSource, StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return CalculatePromptOverlapScore(normalizedSource, normalizedNeedle) >= 0.58f;
        }

        private static float CalculatePromptOverlapScore(string normalizedSource, string normalizedNeedle)
        {
            HashSet<string> sourceNgrams = BuildPromptNgrams(normalizedSource);
            HashSet<string> needleNgrams = BuildPromptNgrams(normalizedNeedle);
            if (sourceNgrams.Count == 0 || needleNgrams.Count == 0)
            {
                return 0f;
            }

            HashSet<string> smaller = sourceNgrams.Count <= needleNgrams.Count ? sourceNgrams : needleNgrams;
            HashSet<string> larger = ReferenceEquals(smaller, sourceNgrams) ? needleNgrams : sourceNgrams;
            int overlapCount = 0;
            foreach (string gram in smaller)
            {
                if (larger.Contains(gram))
                {
                    overlapCount++;
                }
            }

            return overlapCount / (float)Mathf.Max(1, smaller.Count);
        }

        private static HashSet<string> BuildPromptNgrams(string normalizedText)
        {
            HashSet<string> grams = new HashSet<string>(StringComparer.Ordinal);
            if (string.IsNullOrEmpty(normalizedText))
            {
                return grams;
            }

            if (normalizedText.Length == 1)
            {
                grams.Add(normalizedText);
                return grams;
            }

            for (int index = 0; index < normalizedText.Length - 1; index++)
            {
                grams.Add(normalizedText.Substring(index, 2));
            }

            return grams;
        }

        private PromptCardViewState BuildManualState(string text)
        {
            return new PromptCardViewState
            {
                PhaseKey = "manual",
                StageLabel = "Day1 任务页",
                Subtitle = "当前仅显示手动提示。",
                FocusText = text,
                FooterText = string.Empty,
                Items = new List<PromptRowState>
                {
                    new()
                    {
                        Label = "当前提示",
                        Detail = text,
                        Completed = false
                    }
                },
                DisplaySignature = "manual",
                Signature = $"manual|{text}"
            };
        }

        private void RefreshPendingState()
        {
            _pendingState = BuildCurrentViewState();
        }

        private void ApplyBridgePromptState(BridgePromptViewState state)
        {
            if (bridgePromptRect == null || bridgePromptCanvasGroup == null || bridgePromptText == null)
            {
                return;
            }

            if (state == null || string.IsNullOrWhiteSpace(state.Text))
            {
                bridgePromptText.text = string.Empty;
                bridgePromptCanvasGroup.alpha = 0f;
                bridgePromptRect.gameObject.SetActive(false);
                _displayedBridgePromptSignature = string.Empty;
                return;
            }

            bridgePromptRect.gameObject.SetActive(true);
            if (!string.Equals(_displayedBridgePromptSignature, state.Signature, StringComparison.Ordinal)
                || !string.Equals(bridgePromptText.text, state.Text, StringComparison.Ordinal))
            {
                bridgePromptText.text = state.Text;
                bridgePromptText.ForceMeshUpdate();
                _displayedBridgePromptSignature = state.Signature;
            }

            RefreshBridgePromptLayout();
            bridgePromptCanvasGroup.alpha = 1f;
        }

        private void ApplyPendingStateWithoutTransition()
        {
            if (_pendingState == null)
            {
                return;
            }

            ApplyState(_pendingState);
            _displayedState = _pendingState;
            _hasDisplayedState = true;
        }

        private IEnumerator TransitionToPendingState()
        {
            while (_pendingState == null)
            {
                _transitionCoroutine = null;
                yield break;
            }

            PromptCardViewState targetState = _pendingState;

            if (_displayedState != null)
            {
                int sharedCount = Mathf.Min(_displayedState.Items.Count, targetState.Items.Count);
                for (int index = 0; index < sharedCount; index++)
                {
                    PromptRowState oldItem = _displayedState.Items[index];
                    PromptRowState newItem = targetState.Items[index];
                    if (!oldItem.Completed && newItem.Completed && oldItem.Label == newItem.Label && index < _rows.Count)
                    {
                        yield return AnimateRowCompletion(_rows[index], oldItem, newItem);
                    }
                }

                if (_displayedState.Items.Count > 0 && _rows.Count > 0)
                {
                    PromptRowState oldPrimary = _displayedState.Items[0];
                    bool targetMatchesOld = targetState.Items.Count > 0 && targetState.Items[0].Label == oldPrimary.Label;
                    if (!oldPrimary.Completed && !targetMatchesOld)
                    {
                        PromptRowState completedState = new PromptRowState
                        {
                            Label = oldPrimary.Label,
                            Detail = oldPrimary.Detail,
                            Completed = true
                        };
                        yield return AnimateRowCompletion(_rows[0], oldPrimary, completedState);
                    }
                }
            }

            PromptCardViewState completedHoldState = BuildCompletedHoldState(_displayedState, targetState);
            if (completedHoldState != null)
            {
                ApplyState(completedHoldState);
                _displayedState = completedHoldState;

                float holdDuration = Mathf.Max(0.12f, completionStepDuration * 0.75f);
                if (holdDuration > 0.001f)
                {
                    yield return new WaitForSecondsRealtime(holdDuration);
                }
            }

            bool requiresFlip = ShouldUsePageFlipTransition(_displayedState, targetState);

            if (requiresFlip)
            {
                yield return PlayPageFlip(targetState);
            }
            else
            {
                ApplyState(targetState);
            }

            _displayedState = targetState;
            _hasDisplayedState = true;
            _transitionCoroutine = null;

            if (_pendingState != null)
            {
                if (_displayedState.DisplaySignature != _pendingState.DisplaySignature)
                {
                    _transitionCoroutine = StartCoroutine(TransitionToPendingState());
                }
                else if (_displayedState.Signature != _pendingState.Signature)
                {
                    ApplyPendingStateWithoutTransition();
                }
            }
        }

        private PromptCardViewState BuildCompletedHoldState(PromptCardViewState currentState, PromptCardViewState targetState)
        {
            if (currentState == null || targetState == null)
            {
                return null;
            }

            PromptCardViewState completedState = currentState.Clone();
            bool changed = false;

            for (int index = 0; index < completedState.Items.Count; index++)
            {
                PromptRowState holdItem = completedState.Items[index];
                if (holdItem == null || holdItem.Completed)
                {
                    continue;
                }

                PromptRowState targetItem = FindStateItemByLabel(targetState, holdItem.Label);
                if (targetItem != null && targetItem.Completed)
                {
                    holdItem.Completed = true;
                    changed = true;
                    continue;
                }

                if (index == 0 && targetItem == null)
                {
                    holdItem.Completed = true;
                    changed = true;
                }
            }

            if (!changed)
            {
                return null;
            }

            completedState.RefreshSignatures();
            return completedState;
        }

        private static PromptRowState FindStateItemByLabel(PromptCardViewState state, string label)
        {
            if (state?.Items == null || string.IsNullOrWhiteSpace(label))
            {
                return null;
            }

            for (int index = 0; index < state.Items.Count; index++)
            {
                PromptRowState item = state.Items[index];
                if (item == null)
                {
                    continue;
                }

                if (string.Equals(item.Label, label, StringComparison.Ordinal))
                {
                    return item;
                }
            }

            return null;
        }

        private static bool HasSameRowLayout(PromptCardViewState previousState, PromptCardViewState nextState)
        {
            if (previousState == null || nextState == null || previousState.Items.Count != nextState.Items.Count)
            {
                return false;
            }

            for (int index = 0; index < previousState.Items.Count; index++)
            {
                if (previousState.Items[index].Label != nextState.Items[index].Label)
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator PlayPageFlip(PromptCardViewState state)
        {
            ApplyStateToPage(_backPage, state);
            ApplyPageVisibility(_backPage, true);
            _backPage.root.SetSiblingIndex(0);
            _frontPage.root.SetAsLastSibling();
            RefreshCardLayout(_displayedState, state);

            SetPivotKeepingPosition(_frontPage.root, new Vector2(1f, 0f));

            Vector2 frontStart = _frontPage.root.anchoredPosition;
            Vector2 frontEnd = frontStart + new Vector2(22f, 30f);
            Vector3 startScale = Vector3.one;
            Vector3 endScale = new Vector3(0.92f, 0.98f, 1f);
            Quaternion startRotation = Quaternion.identity;
            Quaternion endRotation = Quaternion.Euler(0f, 0f, 26f);
            Vector3 curlStartScale = Vector3.one;
            Vector3 curlEndScale = new Vector3(1.84f, 1.72f, 1f);
            Quaternion curlStartRotation = Quaternion.Euler(0f, 0f, 45f);
            Quaternion curlEndRotation = Quaternion.Euler(0f, 0f, -26f);
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, pageFlipDuration);

            _backPage.root.localScale = new Vector3(0.985f, 0.99f, 1f);
            _backPage.canvasGroup.alpha = 0.92f;
            SetPageCurlVisible(_frontPage, true);
            SetPageCurlVisible(_backPage, false);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - normalized, 3f);
                _frontPage.root.anchoredPosition = Vector2.Lerp(frontStart, frontEnd, eased);
                _frontPage.root.localScale = Vector3.Lerp(startScale, endScale, eased);
                _frontPage.root.localRotation = Quaternion.Lerp(startRotation, endRotation, eased);
                _frontPage.canvasGroup.alpha = Mathf.Lerp(1f, 0f, eased);
                _frontPage.pageCurl.localScale = Vector3.Lerp(curlStartScale, curlEndScale, eased);
                _frontPage.pageCurl.localRotation = Quaternion.Lerp(curlStartRotation, curlEndRotation, eased);
                _backPage.root.localScale = Vector3.Lerp(new Vector3(0.985f, 0.99f, 1f), Vector3.one, eased);
                _backPage.canvasGroup.alpha = Mathf.Lerp(0.92f, 1f, eased);
                yield return null;
            }

            ResetPageVisual(_frontPage);
            PageRefs previousFront = _frontPage;
            _frontPage = _backPage;
            _backPage = previousFront;
            CacheFrontPageRefs();
            ResetPageVisual(_frontPage);
            ApplyPageVisibility(_backPage, false);
            RefreshCardLayout(state, null);
        }

        private IEnumerator AnimateRowCompletion(RowRefs row, PromptRowState oldItem, PromptRowState newItem)
        {
            if (!IsRowBindingAlive(row) || oldItem == null || newItem == null)
            {
                yield break;
            }

            float elapsed = 0f;
            Color startPlate = row.plate.color;
            Color endPlate = new Color(0.42f, 0.56f, 0.33f, 0.12f);
            Color startFill = row.bulletFill.color;
            Color endFill = new Color(0.44f, 0.66f, 0.46f, 0.98f);
            Color startLabel = row.label.color;
            Color endLabel = new Color(0.22f, 0.28f, 0.18f, 0.82f);

            row.label.text = oldItem.Label;
            row.detail.text = oldItem.Detail;

            while (elapsed < completionStepDuration)
            {
                if (!IsRowBindingAlive(row))
                {
                    yield break;
                }

                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / completionStepDuration);
                float eased = 1f - ((1f - normalized) * (1f - normalized));
                row.plate.color = Color.Lerp(startPlate, endPlate, eased);
                row.bulletFill.color = Color.Lerp(startFill, endFill, eased);
                row.label.color = Color.Lerp(startLabel, endLabel, eased);
                row.group.alpha = Mathf.Lerp(1f, 0.92f, eased);
                yield return null;
            }

            if (!IsRowBindingAlive(row))
            {
                yield break;
            }

            ApplyRow(row, newItem, true);
        }

        private void ApplyState(PromptCardViewState state)
        {
            ApplyStateToPage(_frontPage, state);
            ApplyPageVisibility(_frontPage, true);
            ApplyPageVisibility(_backPage, false);
            CacheFrontPageRefs();
            RefreshCardLayout(state, null);
        }

        private void ApplyStateToPage(PageRefs page, PromptCardViewState state)
        {
            page = EnsurePageBindingsReady(page, state?.Items.Count ?? 0);
            if (page == null || state == null)
            {
                return;
            }

            page.appliedState = state;
            EnsurePageTextChainReady(page);
            page.titleText.text = state.StageLabel;
            page.subtitleText.text = state.Subtitle;
            page.focusText.text = state.FocusText;
            page.footerText.text = state.FooterText;
            EnsurePromptTextContent(page.titleText, state.StageLabel, forceOpaque: true);
            EnsurePromptTextContent(page.subtitleText, state.Subtitle, forceOpaque: false);
            EnsurePromptTextContent(page.focusText, state.FocusText, forceOpaque: true);
            EnsurePromptTextContent(page.footerText, state.FooterText, forceOpaque: false);
            page.titleText.ForceMeshUpdate();
            page.subtitleText.ForceMeshUpdate();
            page.focusText.ForceMeshUpdate();
            page.footerText.ForceMeshUpdate();
            ApplyGeneratedPageSectionVisibility(page, state);

            EnsureRows(page, state.Items.Count);
            ApplyRowsToPage(page, state);
            if (!PageMatchesState(page, state))
            {
                RebuildPageRows(page, state.Items.Count);
                ApplyRowsToPage(page, state);
            }

            if (page.usesLegacyManualLayout)
            {
                RefreshLegacyPageLayout(page);
            }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(page.taskListRoot);
                LayoutRebuilder.ForceRebuildLayoutImmediate(page.contentRoot);
            }
        }

        private bool ShouldUsePageFlipTransition(PromptCardViewState currentState, PromptCardViewState nextState)
        {
            if (preferStableInstantTransitions
                || currentState == null
                || nextState == null
                || _suppressWhileDialogueActive)
            {
                return false;
            }

            if (NeedsReadableContentRecovery(nextState))
            {
                return false;
            }

            if (currentState.Items.Count != nextState.Items.Count
                || currentState.Items.Count > 1
                || nextState.Items.Count > 1)
            {
                return false;
            }

            if (!string.Equals(currentState.PhaseKey, nextState.PhaseKey, StringComparison.Ordinal))
            {
                return false;
            }

            return currentState.DisplaySignature != nextState.DisplaySignature;
        }

        private void ApplyRowsToPage(PageRefs page, PromptCardViewState state)
        {
            page = EnsurePageBindingsReady(page, state?.Items.Count ?? 0);
            if (page == null || state == null)
            {
                return;
            }

            for (int index = 0; index < page.rows.Count; index++)
            {
                RowRefs row = page.rows[index];
                if (index < state.Items.Count && !IsRowBindingAlive(row))
                {
                    page = EnsurePageBindingsReady(page, state.Items.Count);
                    if (page == null || index >= page.rows.Count)
                    {
                        return;
                    }

                    row = page.rows[index];
                    if (!IsRowBindingAlive(row))
                    {
                        return;
                    }
                }

                if (index < state.Items.Count)
                {
                    ApplyRow(row, state.Items[index], false);
                    if (row.root != null && !row.root.gameObject.activeSelf)
                    {
                        row.root.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (row?.root != null && row.root.gameObject.activeSelf)
                    {
                        row.root.gameObject.SetActive(false);
                    }
                }
            }
        }

        private bool PageMatchesState(PageRefs page, PromptCardViewState state)
        {
            if (page == null || state == null || page.rows == null)
            {
                return false;
            }

            if (!HasReadablePrimaryText(page.titleText, state.StageLabel)
                || !HasReadablePrimaryText(page.subtitleText, state.Subtitle)
                || !HasReadablePrimaryText(page.focusText, state.FocusText)
                || !HasReadablePrimaryText(page.footerText, state.FooterText))
            {
                return false;
            }

            if (state.Items.Count == 0)
            {
                return true;
            }

            if (page.rows.Count < state.Items.Count)
            {
                return false;
            }

            for (int index = 0; index < state.Items.Count; index++)
            {
                RowRefs row = page.rows[index];
                PromptRowState expected = state.Items[index];
                if (!HasReadablePromptRow(row, expected))
                {
                    return false;
                }
            }

            return true;
        }

        private void RebuildPageRows(PageRefs page, int count)
        {
            if (page?.taskListRoot == null)
            {
                return;
            }

            for (int index = page.taskListRoot.childCount - 1; index >= 0; index--)
            {
                if (page.taskListRoot.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("TaskRow_"))
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(rowRect.gameObject);
                }
                else
                {
                    DestroyImmediate(rowRect.gameObject);
                }
            }

            page.rows.Clear();
            EnsureRows(page, count);
        }

        private void EnsureRows(PageRefs page, int count)
        {
            if (page.rows.Count == 0)
            {
                BindExistingRows(page);
            }

            while (page.rows.Count < count)
            {
                RectTransform template = page.rows.Count > 0 ? page.rows[0].root : null;
                if (template != null)
                {
                    RectTransform clone = Instantiate(template, page.taskListRoot);
                    clone.name = $"TaskRow_{page.rows.Count}";
                    RowRefs boundClone = BindRow(clone);
                    if (boundClone != null)
                    {
                        page.rows.Add(boundClone);
                    }
                    else
                    {
                        DestroyImmediate(clone.gameObject);
                        page.rows.Add(CreateRow(page.taskListRoot, page.rows.Count));
                    }
                }
                else
                {
                    page.rows.Add(CreateRow(page.taskListRoot, page.rows.Count));
                }
            }

            if (ReferenceEquals(page, _frontPage))
            {
                _rows.Clear();
                _rows.AddRange(page.rows);
            }
        }

        private void ApplyRow(RowRefs row, PromptRowState state, bool immediate)
        {
            if (!IsRowBindingAlive(row) || state == null)
            {
                return;
            }

            row.group.alpha = 1f;
            EnsureRowTextChainReady(row);
            row.label.text = state.Label;
            row.detail.text = state.Detail;
            EnsurePromptTextContent(row.label, state.Label, forceOpaque: true);
            EnsurePromptTextContent(row.detail, state.Detail, forceOpaque: false);
            row.label.ForceMeshUpdate();
            row.detail.ForceMeshUpdate();
            row.label.color = state.Completed
                ? new Color(0.22f, 0.28f, 0.18f, 0.82f)
                : new Color(0.2f, 0.17f, 0.12f, 1f);
            row.detail.color = state.Completed
                ? new Color(0.34f, 0.46f, 0.27f, 0.86f)
                : new Color(0.42f, 0.31f, 0.18f, 0.92f);
            row.plate.color = state.Completed
                ? new Color(0.42f, 0.56f, 0.33f, 0.12f)
                : new Color(1f, 1f, 1f, 0.04f);
            row.bulletFill.color = state.Completed
                ? new Color(0.44f, 0.66f, 0.46f, 0.98f)
                : new Color(0.44f, 0.66f, 0.46f, 0f);

            if (immediate)
            {
                row.group.alpha = 1f;
            }

            if (row.usesLegacyManualLayout)
            {
                RefreshLegacyRowLayout(row, row.root.rect.width);
            }
            else
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(row.root);
            }
        }

        private void EnsurePageTextChainReady(PageRefs page)
        {
            if (!IsPageBindingAlive(page))
            {
                return;
            }

            EnsurePromptTextReady(page.titleText, forceActive: true);
            EnsurePromptTextAncestorsVisible(page.titleText, page.root, 0.9f);
            EnsurePromptTextReady(page.subtitleText, forceActive: true);
            EnsurePromptTextAncestorsVisible(page.subtitleText, page.root, 0.82f);
            EnsurePromptTextReady(page.focusText, forceActive: true);
            EnsurePromptTextAncestorsVisible(page.focusText, page.root, 0.9f);
            EnsurePromptTextReady(page.footerText, forceActive: true);
            EnsurePromptTextAncestorsVisible(page.footerText, page.root, 0.82f);

            if (page.rows == null)
            {
                return;
            }

            for (int index = 0; index < page.rows.Count; index++)
            {
                EnsureRowTextChainReady(page.rows[index]);
            }
        }

        private void EnsureRowTextChainReady(RowRefs row)
        {
            if (!IsRowBindingAlive(row))
            {
                return;
            }

            if (row.root != null && !row.root.gameObject.activeSelf)
            {
                row.root.gameObject.SetActive(true);
            }

            if (row.group != null && row.group.alpha < 0.98f)
            {
                row.group.alpha = 1f;
            }

            EnsurePromptTextReady(row.label, forceActive: true);
            EnsurePromptTextAncestorsVisible(row.label, row.root, 0.9f);
            EnsurePromptTextReady(row.detail, forceActive: true);
            EnsurePromptTextAncestorsVisible(row.detail, row.root, 0.82f);
        }

        private void EnsurePromptTextReady(TextMeshProUGUI text, bool forceActive)
        {
            if (text == null)
            {
                return;
            }

            if (forceActive && !text.gameObject.activeSelf)
            {
                text.gameObject.SetActive(true);
            }

            if (!text.enabled)
            {
                text.enabled = true;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFontAsset(text.text);
                if (_fontAsset != null)
                {
                    text.font = _fontAsset;
                    if (_fontAsset.material != null)
                    {
                        text.fontSharedMaterial = _fontAsset.material;
                    }
                }
            }

            if (text.color.a <= 0.01f)
            {
                Color color = text.color;
                color.a = 1f;
                text.color = color;
            }
        }

        private void EnsurePromptTextContent(TextMeshProUGUI text, string expected, bool forceOpaque)
        {
            if (text == null)
            {
                return;
            }

            bool changed = false;
            if (!string.IsNullOrWhiteSpace(expected) && !HasExpectedPromptText(text.text, expected))
            {
                text.text = expected;
                changed = true;
            }

            if (!CanFontRenderText(text.font, text.text))
            {
                _fontAsset = ResolveFontAsset(expected);
                if (_fontAsset != null)
                {
                    text.font = _fontAsset;
                    if (_fontAsset.material != null)
                    {
                        text.fontSharedMaterial = _fontAsset.material;
                    }

                    changed = true;
                }
            }

            if (forceOpaque && text.color.a <= 0.01f)
            {
                Color color = text.color;
                color.a = 1f;
                text.color = color;
                changed = true;
            }

            if (changed)
            {
                text.ForceMeshUpdate();
            }
        }

        private static void EnsurePromptTextAncestorsVisible(TextMeshProUGUI text, RectTransform stopAtExclusive, float minAlpha)
        {
            if (text == null)
            {
                return;
            }

            Transform current = text.transform.parent;
            while (current != null && current != stopAtExclusive)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }

                if (current.TryGetComponent(out CanvasGroup canvasGroup) && canvasGroup.alpha < minAlpha)
                {
                    canvasGroup.alpha = minAlpha;
                }

                current = current.parent;
            }
        }

        private void RefreshCardLayout(PromptCardViewState frontState, PromptCardViewState backState)
        {
            RefreshPageContentLayout(_frontPage, frontState);
            RefreshPageContentLayout(_backPage, backState);
            RefreshCardShellHeight();
            ResetPageVisual(_frontPage);
            if (!_backPage.root.gameObject.activeSelf)
            {
                ResetPageVisual(_backPage);
            }
        }

        private void RefreshPageContentLayout(PageRefs page, PromptCardViewState state)
        {
            if (page == null || state == null)
            {
                return;
            }

            if (page.usesLegacyManualLayout)
            {
                RefreshLegacyPageLayout(page);
                ApplyPagePreferredHeight(page, page.preferredHeight);
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(page.taskListRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(page.contentRoot);
            float preferredHeight = CalculateGeneratedPagePreferredHeight(page);
            page.preferredHeight = preferredHeight;
            ApplyPagePreferredHeight(page, preferredHeight);
        }

        private void ApplyGeneratedPageSectionVisibility(PageRefs page, PromptCardViewState state)
        {
            if (page == null || state == null || page.usesLegacyManualLayout)
            {
                return;
            }

            SetSectionVisible(page.subtitleRoot, !string.IsNullOrWhiteSpace(state.Subtitle));
            SetSectionVisible(page.focusRibbonRoot, !string.IsNullOrWhiteSpace(state.FocusText));
            SetSectionVisible(page.footerRoot, !string.IsNullOrWhiteSpace(state.FooterText));
            SetSectionVisible(page.taskListRoot, state.Items.Count > 0);
        }

        private float CalculateGeneratedPagePreferredHeight(PageRefs page)
        {
            if (page?.contentRoot == null)
            {
                return MinPageHeight;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(page.contentRoot);
            float contentHeight = CalculateVisibleLayoutStackHeight(page.contentRoot);
            return Mathf.Clamp(
                Mathf.Max(MinPageHeight, contentHeight + 28f),
                MinPageHeight,
                MaxPageHeight);
        }

        private static float CalculateVisibleLayoutStackHeight(RectTransform root)
        {
            if (root == null)
            {
                return 0f;
            }

            float total = GetVerticalPadding(root);
            float spacing = GetVerticalSpacing(root);
            int visibleCount = 0;

            for (int index = 0; index < root.childCount; index++)
            {
                if (root.GetChild(index) is not RectTransform child || !child.gameObject.activeSelf)
                {
                    continue;
                }

                float sectionHeight = ResolveLayoutSectionHeight(child);
                if (sectionHeight <= 0.01f)
                {
                    continue;
                }

                total += sectionHeight;
                visibleCount++;
            }

            if (visibleCount > 1)
            {
                total += spacing * (visibleCount - 1);
            }

            return total;
        }

        private static float ResolveLayoutSectionHeight(RectTransform rect)
        {
            if (rect == null || !rect.gameObject.activeSelf)
            {
                return 0f;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

            float preferredHeight = LayoutUtility.GetPreferredHeight(rect);
            if (preferredHeight > 0.01f)
            {
                return Mathf.Ceil(preferredHeight);
            }

            float minHeight = LayoutUtility.GetMinHeight(rect);
            if (minHeight > 0.01f)
            {
                return Mathf.Ceil(minHeight);
            }

            return Mathf.Ceil(GetCurrentHeight(rect, 0f));
        }

        private static float ResolveLegacyVisibleHeight(PageRefs page)
        {
            if (page?.root == null)
            {
                return MinLegacyPageHeight;
            }

            float bottom = 0f;
            bottom = Mathf.Max(bottom, GetVisibleBottom(page.stageTagRoot));
            bottom = Mathf.Max(bottom, GetVisibleBottom(page.subtitleRoot));
            bottom = Mathf.Max(bottom, GetVisibleBottom(page.headerDividerRoot));
            bottom = Mathf.Max(bottom, GetVisibleBottom(page.focusRibbonRoot));
            bottom = Mathf.Max(bottom, GetVisibleBottom(page.footerRoot));

            if (page.taskListRoot != null && page.taskListRoot.gameObject.activeSelf)
            {
                bottom = Mathf.Max(bottom, GetVisibleBottom(page.taskListRoot));
                for (int index = 0; index < page.rows.Count; index++)
                {
                    if (page.rows[index]?.root != null && page.rows[index].root.gameObject.activeSelf)
                    {
                        bottom = Mathf.Max(bottom, GetVisibleBottom(page.rows[index].root));
                    }
                }
            }

            return Mathf.Max(MinLegacyPageHeight, bottom + 8f);
        }

        private static float GetLegacyPageShellInset(PageRefs page)
        {
            if (page?.contentRoot == null || page.contentRoot == page.root)
            {
                return 0f;
            }

            float inset = GetRectVerticalInsetWithinParent(page.contentRoot) + GetVerticalPadding(page.contentRoot);
            return Mathf.Max(20f, inset);
        }

        private static float GetRectVerticalInsetWithinParent(RectTransform rect)
        {
            if (rect == null)
            {
                return 0f;
            }

            bool stretchesVertically = Mathf.Abs(rect.anchorMin.y) <= 0.001f
                && Mathf.Abs(rect.anchorMax.y - 1f) <= 0.001f;
            if (!stretchesVertically)
            {
                return 0f;
            }

            return Mathf.Abs(rect.offsetMin.y) + Mathf.Abs(rect.offsetMax.y);
        }

        private static float GetVisibleBottom(RectTransform rect)
        {
            if (rect == null || !rect.gameObject.activeSelf)
            {
                return 0f;
            }

            return GetTopInParent(rect) + GetCurrentHeight(rect, 0f);
        }

        private static float GetVerticalPadding(RectTransform rect)
        {
            if (rect == null || !rect.TryGetComponent(out VerticalLayoutGroup layout))
            {
                return 0f;
            }

            return layout.padding.top + layout.padding.bottom;
        }

        private static float GetVerticalSpacing(RectTransform rect)
        {
            return rect != null && rect.TryGetComponent(out VerticalLayoutGroup layout) ? layout.spacing : 0f;
        }

        private static void SetSectionVisible(RectTransform rect, bool visible)
        {
            if (rect != null && rect.gameObject.activeSelf != visible)
            {
                rect.gameObject.SetActive(visible);
            }
        }

        private void RefreshCardShellHeight()
        {
            if (cardRect == null)
            {
                return;
            }

            float frontHeight = ResolvePageShellHeight(_frontPage);
            float backHeight = ResolvePageShellHeight(_backPage);
            float minimumShellHeight = Mathf.Max(
                ResolveLegacyShellMinimumHeight(_frontPage),
                ResolveLegacyShellMinimumHeight(_backPage));
            float targetHeight = Mathf.Max(minimumShellHeight, Mathf.Max(frontHeight, backHeight) + 8f);
            SetHeightKeepingTop(cardRect, targetHeight, _cardDefaultPosition, _cardDefaultHeight);
            RefreshBridgePromptLayout();
        }

        private static float ResolveLegacyShellMinimumHeight(PageRefs page)
        {
            if (page == null || !page.usesLegacyManualLayout)
            {
                return MinLegacyPageHeight + 8f;
            }

            return GetLegacyMinimumPageHeight(page) + 8f;
        }

        private static float ResolvePageShellHeight(PageRefs page)
        {
            if (page?.root == null)
            {
                return MinPageHeight;
            }

            bool isVisiblePage = page.root.gameObject.activeSelf
                && page.root.gameObject.activeInHierarchy
                && (page.canvasGroup == null || page.canvasGroup.alpha > 0.01f);
            if (!isVisiblePage)
            {
                return 0f;
            }

            if (page.preferredHeight > 0.01f)
            {
                return page.preferredHeight;
            }

            float currentHeight = page.root.rect.height > 1f ? page.root.rect.height : page.root.sizeDelta.y;
            float minHeight = page.usesLegacyManualLayout ? GetLegacyMinimumPageHeight(page) : MinPageHeight;
            return Mathf.Clamp(currentHeight, minHeight, MaxPageHeight);
        }

        private void ApplyPagePreferredHeight(PageRefs page, float preferredHeight)
        {
            if (page?.root == null)
            {
                return;
            }

            float minHeight = page.usesLegacyManualLayout ? GetLegacyMinimumPageHeight(page) : MinPageHeight;
            float clampedHeight = Mathf.Clamp(preferredHeight, minHeight, MaxPageHeight);
            SetHeightKeepingTop(page.root, clampedHeight, page.defaultPosition, page.defaultHeight);
            page.preferredHeight = clampedHeight;
        }

        private static float GetLegacyMinimumPageHeight(PageRefs page)
        {
            if (page == null)
            {
                return MinLegacyPageHeight;
            }

            float baselineHeight = page.defaultHeight > 0.01f ? page.defaultHeight : MinPageHeight;
            return Mathf.Max(MinLegacyPageHeight, baselineHeight);
        }

        private static void SetHeightKeepingTop(RectTransform rect, float targetHeight, Vector2 defaultPosition, float defaultHeight)
        {
            if (rect == null)
            {
                return;
            }

            float baselineHeight = defaultHeight > 0.01f
                ? defaultHeight
                : (rect.rect.height > 1f ? rect.rect.height : rect.sizeDelta.y);
            if (baselineHeight <= 0.01f)
            {
                baselineHeight = targetHeight;
            }

            if (Mathf.Abs(rect.sizeDelta.y - targetHeight) <= 0.01f
                && Vector2.Distance(rect.anchoredPosition, defaultPosition) <= 0.01f)
            {
                return;
            }

            Vector2 anchoredPosition = defaultPosition;
            anchoredPosition.y -= (targetHeight - baselineHeight) * 0.5f;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, targetHeight);
        }

        private void ResetPageVisual(PageRefs page)
        {
            if (!IsPageBindingAlive(page))
            {
                return;
            }

            SetPivotKeepingPosition(page.root, page.defaultPivot);
            page.root.anchoredPosition = page.defaultPosition;
            page.root.localRotation = Quaternion.identity;
            page.root.localScale = Vector3.one;
            page.canvasGroup.alpha = 1f;
            page.pageCurl.localScale = Vector3.one;
            page.pageCurl.localRotation = Quaternion.Euler(0f, 0f, 45f);
            SetPageCurlVisible(page, false);
        }

        private static void SetPivotKeepingPosition(RectTransform rect, Vector2 targetPivot)
        {
            if (rect == null || rect.pivot == targetPivot)
            {
                return;
            }

            Vector2 size = rect.rect.size;
            Vector2 deltaPivot = targetPivot - rect.pivot;
            rect.pivot = targetPivot;
            rect.anchoredPosition += new Vector2(size.x * deltaPivot.x, size.y * deltaPivot.y);
        }

        private void CacheFrontPageRefs()
        {
            if (!IsPageBindingAlive(_frontPage))
            {
                pageRect = null;
                titleText = null;
                subtitleText = null;
                focusText = null;
                footerText = null;
                _rows.Clear();
                return;
            }

            pageRect = _frontPage.root;
            titleText = _frontPage.titleText;
            subtitleText = _frontPage.subtitleText;
            focusText = _frontPage.focusText;
            footerText = _frontPage.footerText;
            _rows.Clear();
            _rows.AddRange(_frontPage.rows);
        }

        private void ApplyPageVisibility(PageRefs page, bool visible)
        {
            if (!IsPageBindingAlive(page))
            {
                return;
            }

            page.root.gameObject.SetActive(visible);
            page.canvasGroup.alpha = visible ? 1f : 0f;
        }

        private bool ShouldDelayPromptDisplay()
        {
            if (_externallySuppressedByModalUi)
            {
                return true;
            }

            if (_suppressWhileDialogueActive)
            {
                return true;
            }

            return false;
        }

        private bool TryGetParentCanvas(out Canvas parentCanvas)
        {
            parentCanvas = null;
            if (overlayCanvas == null)
            {
                return false;
            }

            Canvas[] parents = GetComponentsInParent<Canvas>(includeInactive: true);
            for (int index = 0; index < parents.Length; index++)
            {
                Canvas candidate = parents[index];
                if (candidate == null || candidate == overlayCanvas)
                {
                    continue;
                }

                parentCanvas = candidate;
                return true;
            }

            return false;
        }

        private bool ShouldIgnoreDialogueEndEvent()
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            return dialogueManager != null && dialogueManager.IsDialogueActive;
        }

        private bool IsLikelyManagedByDialogueUiTransition()
        {
            DialogueUI dialogueUi = FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
            if (dialogueUi == null)
            {
                return false;
            }

            Transform dialogueParent = dialogueUi.transform.parent;
            return dialogueParent != null && transform.parent == dialogueParent;
        }

        private void QueuePromptReveal()
        {
            if (string.IsNullOrWhiteSpace(_queuedPromptText))
            {
                return;
            }

            EnsureRuntimeObjectActive();
            StopQueuedRevealCoroutine();
            _queuedRevealCoroutine = StartCoroutine(WaitAndRevealQueuedPrompt());
        }

        private IEnumerator WaitAndRevealQueuedPrompt()
        {
            while (ShouldDelayPromptDisplay())
            {
                yield return null;
            }

            if (postDialogueResumeDelay > 0f)
            {
                float remaining = postDialogueResumeDelay;
                while (remaining > 0f)
                {
                    if (ShouldDelayPromptDisplay())
                    {
                        remaining = postDialogueResumeDelay;
                        yield return null;
                        continue;
                    }

                    remaining -= Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            RefreshPendingState();
            if (_pendingState != null)
            {
                if (_displayedState == null || _displayedState.DisplaySignature != _pendingState.DisplaySignature)
                {
                    if (_transitionCoroutine == null)
                    {
                        _transitionCoroutine = StartCoroutine(TransitionToPendingState());
                    }
                }
                else if (_displayedState.Signature != _pendingState.Signature)
                {
                    ApplyPendingStateWithoutTransition();
                    FadeCanvasGroup(1f, false);
                }
                else
                {
                    FadeCanvasGroup(1f, false);
                }
            }

            _queuedRevealCoroutine = null;
        }

        private void FadeCanvasGroup(float targetAlpha, bool immediate)
        {
            if (canvasGroup == null)
            {
                return;
            }

            StopVisibilityCoroutine();

            if (immediate || fadeDuration <= 0f || !Application.isPlaying || !CanAnimateCanvasVisibility())
            {
                ApplyCanvasGroupImmediate(targetAlpha);
                return;
            }

            _visibilityCoroutine = StartCoroutine(FadeCanvasGroupRoutine(targetAlpha));
        }

        private bool CanAnimateCanvasVisibility()
        {
            if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
            {
                return false;
            }

            return overlayCanvas == null || overlayCanvas.gameObject.activeInHierarchy;
        }

        private void ApplyCanvasGroupImmediate(float targetAlpha)
        {
            _visibilityAlpha = Mathf.Clamp01(targetAlpha);
            ApplyComposedCanvasGroupState();
        }

        private IEnumerator FadeCanvasGroupRoutine(float targetAlpha)
        {
            float startAlpha = _visibilityAlpha;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / fadeDuration);
                float eased = 1f - ((1f - normalized) * (1f - normalized));
                _visibilityAlpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
                ApplyComposedCanvasGroupState();
                yield return null;
            }

            _visibilityAlpha = Mathf.Clamp01(targetAlpha);
            ApplyComposedCanvasGroupState();
            _visibilityCoroutine = null;
        }

        private void ApplyComposedCanvasGroupState()
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = _visibilityAlpha * _boundaryFocusAlpha;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void StopVisibilityCoroutine()
        {
            if (_visibilityCoroutine == null)
            {
                return;
            }

            StopCoroutine(_visibilityCoroutine);
            _visibilityCoroutine = null;
        }

        private void StopQueuedRevealCoroutine()
        {
            if (_queuedRevealCoroutine == null)
            {
                return;
            }

            StopCoroutine(_queuedRevealCoroutine);
            _queuedRevealCoroutine = null;
        }

        private void StopTransitionCoroutine()
        {
            if (_transitionCoroutine == null)
            {
                return;
            }

            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }

        private TMP_FontAsset ResolveFontAsset(string preferredText = null)
        {
            return DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(
                preferredText,
                _fontAsset,
                FontCoverageProbeText);
        }

        private static bool CanFontRenderText(TMP_FontAsset fontAsset, string currentText)
        {
            return DialogueChineseFontRuntimeBootstrap.CanRenderText(
                fontAsset,
                currentText,
                FontCoverageProbeText);
        }

        private static string GetFontProbeText(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
            {
                return FontCoverageProbeText;
            }

            StringBuilder builder = new StringBuilder(currentText.Length);
            bool insideTag = false;
            for (int index = 0; index < currentText.Length; index++)
            {
                char current = currentText[index];
                if (current == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (current == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (!char.IsControl(current))
                {
                    builder.Append(current);
                }
            }

            return builder.Length > 0 ? builder.ToString() : FontCoverageProbeText;
        }

        private static bool IsFontAssetUsable(TMP_FontAsset fontAsset)
        {
            if (fontAsset == null || fontAsset.material == null)
            {
                return false;
            }

            Texture[] atlasTextures = fontAsset.atlasTextures;
            if (atlasTextures == null || atlasTextures.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < atlasTextures.Length; index++)
            {
                Texture atlasTexture = atlasTextures[index];
                if (atlasTexture != null && atlasTexture.width > 1 && atlasTexture.height > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment, bool wrap = false)
        {
            RectTransform rect = CreateRect(parent, name);
            TextMeshProUGUI tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.font = _fontAsset;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            tmp.overflowMode = wrap ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static Transform ResolveParent()
        {
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                Canvas preferredCanvas = ResolvePreferredUiRootCanvas(uiRoot.transform);
                if (preferredCanvas != null)
                {
                    return preferredCanvas.transform;
                }

                return uiRoot.transform;
            }

            return SpringDay1UiLayerUtility.ResolveUiParent();
        }

        private void EnsureAttachedToPreferredParent()
        {
            Transform preferredParent = ResolveParent();
            if (preferredParent != null && transform.parent != preferredParent)
            {
                transform.SetParent(preferredParent, false);
                rootRect = transform as RectTransform;
            }

            EnsureHudSiblingOrder();
        }

        private void EnsureHudSiblingOrder()
        {
            Transform parent = transform.parent;
            if (parent == null)
            {
                return;
            }

            int currentIndex = transform.GetSiblingIndex();
            int desiredIndex = ResolveDesiredHudSiblingIndex(parent, currentIndex);
            if (desiredIndex != currentIndex)
            {
                transform.SetSiblingIndex(desiredIndex);
            }
        }

        private int ResolveDesiredHudSiblingIndex(Transform parent, int currentIndex)
        {
            int afterHudIndex = -1;
            int firstModalIndex = int.MaxValue;

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform sibling = parent.GetChild(index);
                if (sibling == null || sibling == transform)
                {
                    continue;
                }

                if (ShouldTreatAsPromptModalSibling(sibling))
                {
                    firstModalIndex = Mathf.Min(firstModalIndex, index);
                    continue;
                }

                if (ShouldTreatAsPromptHudSibling(sibling))
                {
                    afterHudIndex = Mathf.Max(afterHudIndex, index);
                }
            }

            if (afterHudIndex < 0 && firstModalIndex == int.MaxValue)
            {
                return currentIndex;
            }

            int desiredIndex = afterHudIndex >= 0 ? afterHudIndex + 1 : currentIndex;
            if (firstModalIndex != int.MaxValue)
            {
                desiredIndex = Mathf.Min(desiredIndex, firstModalIndex);
            }

            return Mathf.Clamp(desiredIndex, 0, Mathf.Max(0, parent.childCount - 1));
        }

        private static bool ShouldTreatAsPromptHudSibling(Transform sibling)
        {
            if (sibling == null)
            {
                return false;
            }

            switch (sibling.gameObject.name)
            {
                case "State":
                case "ToolBar":
                case "InteractionHintOverlay":
                case "SpringDay1StatusOverlay":
                    return true;
                default:
                    return false;
            }
        }

        private static bool ShouldTreatAsPromptModalSibling(Transform sibling)
        {
            if (sibling == null)
            {
                return false;
            }

            if (sibling.GetComponent<PackagePanelTabsUI>() != null
                || sibling.GetComponent<global::FarmGame.UI.BoxPanelUI>() != null
                || sibling.GetComponent<DialogueUI>() != null
                || sibling.GetComponent<PackageSaveSettingsPanel>() != null
                || sibling.GetComponent<SpringDay1WorkbenchCraftingOverlay>() != null)
            {
                return true;
            }

            string lowerName = sibling.gameObject.name.ToLowerInvariant();
            return lowerName.Contains("package")
                || lowerName.Contains("box")
                || lowerName.Contains("dialogue")
                || lowerName.Contains("workbench")
                || lowerName.Contains("settings");
        }

        private static Canvas ResolvePreferredUiRootCanvas(Transform uiRoot)
        {
            if (uiRoot == null)
            {
                return null;
            }

            Canvas[] canvases = uiRoot.GetComponentsInChildren<Canvas>(includeInactive: true);
            Canvas bestCanvas = null;
            int bestScore = int.MinValue;

            for (int index = 0; index < canvases.Length; index++)
            {
                Canvas candidate = canvases[index];
                if (candidate == null)
                {
                    continue;
                }

                int score = ScoreUiRootCanvasCandidate(uiRoot, candidate);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCanvas = candidate;
                }
            }

            return bestScore > int.MinValue ? bestCanvas : null;
        }

        private static int ScoreUiRootCanvasCandidate(Transform uiRoot, Canvas candidate)
        {
            if (candidate == null)
            {
                return int.MinValue;
            }

            string candidateName = candidate.gameObject.name;
            string lowerName = candidateName.ToLowerInvariant();
            int score = 0;

            if (candidate.GetComponent<SpringDay1PromptOverlay>() != null)
            {
                score -= 900;
            }

            if (candidate.GetComponent<PackagePanelTabsUI>() != null)
            {
                score -= 900;
            }

            if (candidate.GetComponent<DialogueUI>() != null)
            {
                score -= 750;
            }

            if (candidate.GetComponent<SpringDay1WorkbenchCraftingOverlay>() != null)
            {
                score -= 750;
            }

            if (lowerName.Contains("packagepanel")
                || lowerName.Contains("package")
                || lowerName.Contains("box")
                || lowerName.Contains("dialogue")
                || lowerName.Contains("workbench")
                || lowerName.Contains("prompt")
                || lowerName.Contains("tooltip")
                || lowerName.Contains("modal")
                || lowerName.Contains("popup")
                || lowerName.Contains("confirm"))
            {
                score -= 600;
            }

            if (candidate.transform.parent == uiRoot)
            {
                score += 180;
            }

            if (!candidate.overrideSorting)
            {
                score += 220;
            }
            else
            {
                score -= Mathf.Clamp(candidate.sortingOrder, 0, 400);
            }

            if (lowerName == "canvas" || lowerName.Contains("main") || lowerName.Contains("hud") || lowerName.Contains("ui"))
            {
                score += 160;
            }

            if (candidate.gameObject.activeInHierarchy)
            {
                score += 25;
            }

            return score;
        }

        private static SpringDay1PromptOverlay InstantiateRuntimePrefab()
        {
            GameObject prefab = LoadRuntimePrefabAsset();
            if (!CanInstantiateRuntimePrefab(prefab))
            {
                return null;
            }

            Transform parent = ResolveParent();
            GameObject instance = parent != null ? Instantiate(prefab, parent, false) : Instantiate(prefab);
            instance.name = prefab.name;
            return instance.GetComponent<SpringDay1PromptOverlay>();
        }

        private static SpringDay1PromptOverlay FindReusableRuntimeInstance(bool requireScreenOverlay)
        {
            SpringDay1PromptOverlay[] candidates = FindObjectsByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1PromptOverlay candidate = candidates[index];
                if (CanReuseRuntimeInstance(candidate, requireScreenOverlay))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static GameObject LoadRuntimePrefabAsset()
        {
            GameObject prefab = SpringDay1UiPrefabRegistry.LoadPromptOverlayPrefab();
            if (prefab != null)
            {
                return prefab;
            }

#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
#else
            return null;
#endif
        }

        private static bool CanInstantiateRuntimePrefab(GameObject prefab)
        {
            return prefab != null && prefab.GetComponent<SpringDay1PromptOverlay>() != null;
        }

        private static bool CanReuseRuntimeInstance(SpringDay1PromptOverlay candidate, bool requireScreenOverlay)
        {
            if (candidate == null || candidate.gameObject == null || !candidate.gameObject.scene.IsValid())
            {
                return false;
            }

            RectTransform candidateRoot = candidate.rootRect != null ? candidate.rootRect : candidate.transform as RectTransform;
            Canvas candidateCanvas = candidate.overlayCanvas != null ? candidate.overlayCanvas : candidate.GetComponent<Canvas>();
            CanvasGroup candidateCanvasGroup = candidate.canvasGroup != null ? candidate.canvasGroup : candidate.GetComponent<CanvasGroup>();

            if (candidateRoot == null || candidateCanvas == null || candidateCanvasGroup == null)
            {
                return false;
            }

            if (requireScreenOverlay && candidateCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return false;
            }

            RectTransform candidateCard = candidate.cardRect != null ? candidate.cardRect : FindDirectChildRect(candidateRoot, "TaskCardRoot");
            if (candidateCard == null)
            {
                return false;
            }

            RectTransform candidateBridgePrompt = candidate.bridgePromptRect != null
                ? candidate.bridgePromptRect
                : FindDescendantRect(candidateCard, "BridgePromptRoot");
            if (candidateBridgePrompt == null || candidateBridgePrompt.parent != candidateCard)
            {
                return false;
            }

            RectTransform frontRoot = candidate.pageRect != null ? candidate.pageRect : FindDirectChildRect(candidateCard, "Page");
            if (frontRoot == null || !CanBindPageRoot(frontRoot))
            {
                return false;
            }

            RectTransform backRoot = FindDirectChildRect(candidateCard, "BackPage");
            return backRoot == null || CanBindPageRoot(backRoot);
        }

        private bool HasLivePromptBindings()
        {
            return rootRect != null
                && canvasGroup != null
                && cardRect != null
                && bridgePromptRect != null
                && bridgePromptCanvasGroup != null
                && bridgePromptText != null
                && IsPageBindingAlive(_frontPage)
                && IsPageBindingAlive(_backPage);
        }

        private PageRefs EnsurePageBindingsReady(PageRefs page, int requiredRowCount)
        {
            if (page?.root == null)
            {
                return null;
            }

            if (!IsPageBindingAlive(page))
            {
                PageRefs rebound = BindPage(page.root);
                if (!IsPageBindingAlive(rebound))
                {
                    return null;
                }

                CopyPageRefs(page, rebound);
            }

            bool hasDeadRow = false;
            for (int index = 0; index < page.rows.Count; index++)
            {
                if (!IsRowBindingAlive(page.rows[index]))
                {
                    hasDeadRow = true;
                    break;
                }
            }

            if (hasDeadRow)
            {
                BindExistingRows(page);
            }

            if (page.rows.Count < requiredRowCount)
            {
                EnsureRows(page, requiredRowCount);
            }

            if (ReferenceEquals(page, _frontPage))
            {
                CacheFrontPageRefs();
            }

            return page;
        }

        private static void CopyPageRefs(PageRefs target, PageRefs source)
        {
            if (target == null || source == null)
            {
                return;
            }

            PromptCardViewState appliedState = target.appliedState;

            target.root = source.root;
            target.canvasGroup = source.canvasGroup;
            target.contentRoot = source.contentRoot;
            target.taskListRoot = source.taskListRoot;
            target.pageCurl = source.pageCurl;
            target.pageCurlImage = source.pageCurlImage;
            target.stageTagRoot = source.stageTagRoot;
            target.subtitleRoot = source.subtitleRoot;
            target.headerDividerRoot = source.headerDividerRoot;
            target.focusRibbonRoot = source.focusRibbonRoot;
            target.footerRoot = source.footerRoot;
            target.titleText = source.titleText;
            target.subtitleText = source.subtitleText;
            target.focusText = source.focusText;
            target.footerText = source.footerText;
            target.rows.Clear();
            target.rows.AddRange(source.rows);
            target.defaultPosition = source.defaultPosition;
            target.defaultPivot = source.defaultPivot;
            target.defaultHeight = source.defaultHeight;
            target.usesLegacyManualLayout = source.usesLegacyManualLayout;
            target.preferredHeight = source.preferredHeight;
            target.footerBaselineTop = source.footerBaselineTop;
            target.footerDefaultHeight = source.footerDefaultHeight;
            target.appliedState = appliedState;
        }

        private static bool IsPageBindingAlive(PageRefs page)
        {
            return page != null
                && page.root != null
                && page.canvasGroup != null
                && page.contentRoot != null
                && page.taskListRoot != null
                && page.pageCurl != null
                && page.pageCurlImage != null
                && page.titleText != null
                && page.subtitleText != null
                && page.focusText != null
                && page.footerText != null;
        }

        private static bool IsRowBindingAlive(RowRefs row)
        {
            return row != null
                && row.root != null
                && row.group != null
                && row.plate != null
                && row.bulletFill != null
                && row.label != null
                && row.detail != null;
        }

        private static bool CanBindPageRoot(RectTransform pageRoot)
        {
            if (pageRoot == null)
            {
                return false;
            }

            TextMeshProUGUI title = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "TitleText");
            TextMeshProUGUI subtitle = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "SubtitleText");
            TextMeshProUGUI focus = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FocusText");
            TextMeshProUGUI footer = FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FooterText");
            if (!CanReusePromptText(title)
                || !CanReusePromptText(subtitle)
                || !CanReusePromptText(focus)
                || !CanReusePromptText(footer))
            {
                return false;
            }

            RectTransform contentRoot = FindDirectChildRect(pageRoot, "ContentRoot");
            RectTransform taskList = contentRoot != null
                ? FindDirectChildRect(contentRoot, "TaskList")
                : FindDirectChildRect(pageRoot, "TaskList");

            return taskList != null && HasBindableRowChain(taskList);
        }

        private static bool HasBindableRowChain(RectTransform taskListRoot)
        {
            if (taskListRoot == null)
            {
                return false;
            }

            for (int index = 0; index < taskListRoot.childCount; index++)
            {
                if (taskListRoot.GetChild(index) is not RectTransform rowRect || !rowRect.name.StartsWith("TaskRow_"))
                {
                    continue;
                }

                if (CanBindRowRect(rowRect))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanBindRowRect(RectTransform rowRect)
        {
            return rowRect != null
                && rowRect.GetComponent<Image>() != null
                && FindDescendantComponent<Image>(rowRect, "BulletFill") != null
                && CanReusePromptText(FindDescendantComponent<TextMeshProUGUI>(rowRect, "Label"))
                && CanReusePromptText(FindDescendantComponent<TextMeshProUGUI>(rowRect, "Detail"));
        }

        private static bool CanReusePromptText(TextMeshProUGUI text)
        {
            return text != null
                && text.enabled
                && text.gameObject.activeSelf
                && text.color.a > 0.01f
                && text.rectTransform.rect.width > 2f
                && text.rectTransform.rect.height > 2f
                && CanFontRenderText(text.font, text.text);
        }

        private static void RetireIncompatibleRuntimeInstances()
        {
            SpringDay1PromptOverlay[] candidates = FindObjectsByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1PromptOverlay candidate = candidates[index];
                if (candidate == null || CanReuseRuntimeInstance(candidate, requireScreenOverlay: true))
                {
                    continue;
                }

                RetireRuntimeInstance(candidate);
            }
        }

        private static void RetireOtherRuntimeInstances(SpringDay1PromptOverlay keep)
        {
            SpringDay1PromptOverlay[] candidates = FindObjectsByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < candidates.Length; index++)
            {
                SpringDay1PromptOverlay candidate = candidates[index];
                if (candidate == null || candidate == keep)
                {
                    continue;
                }

                RetireRuntimeInstance(candidate);
            }
        }

        private static void RetireRuntimeInstance(SpringDay1PromptOverlay candidate)
        {
            if (candidate == null || candidate.gameObject == null)
            {
                return;
            }

            if (candidate.canvasGroup != null)
            {
                candidate.canvasGroup.alpha = 0f;
                candidate.canvasGroup.interactable = false;
                candidate.canvasGroup.blocksRaycasts = false;
            }

            if (candidate.overlayCanvas != null)
            {
                candidate.overlayCanvas.gameObject.SetActive(false);
            }
            else
            {
                candidate.gameObject.SetActive(false);
            }
        }

        private static RectTransform FindDirectChildRect(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            Transform child = parent.Find(childName);
            return child as RectTransform;
        }

        private static RectTransform FindDescendantRect(Transform parent, string childName)
        {
            Transform child = FindDescendant(parent, childName);
            return child as RectTransform;
        }

        private static T FindDescendantComponent<T>(Transform parent, string childName) where T : Component
        {
            Transform child = FindDescendant(parent, childName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static Transform FindDescendant(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (child.name == childName)
                {
                    return child;
                }

                Transform nested = FindDescendant(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private static int ParseTrailingIndex(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return int.MaxValue;
            }

            int underscoreIndex = name.LastIndexOf('_');
            if (underscoreIndex < 0 || underscoreIndex >= name.Length - 1)
            {
                return int.MaxValue;
            }

            int value;
            return int.TryParse(name.Substring(underscoreIndex + 1), out value) ? value : int.MaxValue;
        }

        private sealed class PageRefs
        {
            public RectTransform root;
            public CanvasGroup canvasGroup;
            public RectTransform contentRoot;
            public RectTransform taskListRoot;
            public RectTransform pageCurl;
            public Image pageCurlImage;
            public RectTransform stageTagRoot;
            public RectTransform subtitleRoot;
            public RectTransform headerDividerRoot;
            public RectTransform focusRibbonRoot;
            public RectTransform footerRoot;
            public TextMeshProUGUI titleText;
            public TextMeshProUGUI subtitleText;
            public TextMeshProUGUI focusText;
            public TextMeshProUGUI footerText;
            public readonly List<RowRefs> rows = new();
            public Vector2 defaultPosition;
            public Vector2 defaultPivot;
            public float defaultHeight;
            public bool usesLegacyManualLayout;
            public float preferredHeight;
            public float footerBaselineTop;
            public float footerDefaultHeight;
            public PromptCardViewState appliedState;
        }

        private sealed class RowRefs
        {
            public RectTransform root;
            public CanvasGroup group;
            public Image plate;
            public Image bulletFill;
            public RectTransform bulletRoot;
            public TextMeshProUGUI label;
            public RectTransform labelRect;
            public TextMeshProUGUI detail;
            public RectTransform detailRect;
            public bool usesLegacyManualLayout;
            public float preferredHeight;
        }

        private sealed class BridgePromptViewState
        {
            public BridgePromptViewState(string text, string signature)
            {
                Text = text ?? string.Empty;
                Signature = signature ?? string.Empty;
            }

            public string Text;
            public string Signature;
        }

        private sealed class PromptRowState
        {
            public string Label;
            public string Detail;
            public bool Completed;

            public PromptRowState Clone()
            {
                return new PromptRowState
                {
                    Label = Label,
                    Detail = Detail,
                    Completed = Completed
                };
            }

            public string GetSignature()
            {
                return $"{Label}|{Detail}|{Completed}";
            }
        }

        private sealed class PromptCardViewState
        {
            public string PhaseKey;
            public string StageLabel;
            public string Subtitle;
            public string FocusText;
            public string FooterText;
            public List<PromptRowState> Items = new();
            public string DisplaySignature;
            public string Signature;

            public PromptCardViewState Clone()
            {
                PromptCardViewState clone = new PromptCardViewState
                {
                    PhaseKey = PhaseKey,
                    StageLabel = StageLabel,
                    Subtitle = Subtitle,
                    FocusText = FocusText,
                    FooterText = FooterText
                };

                for (int index = 0; index < Items.Count; index++)
                {
                    clone.Items.Add(Items[index]?.Clone());
                }

                clone.RefreshSignatures();
                return clone;
            }

            public static PromptCardViewState FromModel(SpringDay1Director.PromptCardModel model, string focusText, int maxVisibleItems)
            {
                PromptCardViewState state = new PromptCardViewState
                {
                    PhaseKey = model.PhaseKey ?? "unknown",
                    StageLabel = model.StageLabel ?? "Day1 任务页",
                    Subtitle = model.Subtitle ?? string.Empty,
                    FocusText = focusText ?? string.Empty,
                    FooterText = model.FooterText ?? string.Empty
                };

                if (model.Items != null)
                {
                    int visibleLimit = Mathf.Max(1, maxVisibleItems);
                    int primaryIndex = ResolvePrimaryItemIndex(model.Items);
                    bool hasIncompleteItems = HasIncompleteItems(model.Items);
                    bool[] consumed = new bool[model.Items.Length];

                    TryAddVisibleItem(state, model.Items, consumed, primaryIndex, focusText, model.Subtitle, model.FooterText);

                    for (int index = 0; index < model.Items.Length && state.Items.Count < visibleLimit; index++)
                    {
                        if (index == primaryIndex)
                        {
                            continue;
                        }

                        if (hasIncompleteItems && model.Items[index].Completed)
                        {
                            continue;
                        }

                        if (!TryAddVisibleItem(state, model.Items, consumed, index, focusText, model.Subtitle, model.FooterText))
                        {
                            continue;
                        }
                    }

                    for (int index = 0; index < model.Items.Length && state.Items.Count < visibleLimit; index++)
                    {
                        TryAddVisibleItem(state, model.Items, consumed, index, focusText, model.Subtitle, model.FooterText);
                    }
                }

                if (state.Items.Count == 0)
                {
                    state.Items.Add(new PromptRowState
                    {
                        Label = "当前任务",
                        Detail = FirstNonEmpty(focusText, model.Subtitle, model.FooterText, "等待剧情推进。"),
                        Completed = false
                    });
                }

                state.RefreshSignatures();
                return state;
            }

            public void RefreshSignatures()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(PhaseKey).Append('|').Append(StageLabel).Append('|').Append(Subtitle).Append('|').Append(FocusText).Append('|').Append(FooterText);
                for (int index = 0; index < Items.Count; index++)
                {
                    builder.Append('|').Append(Items[index]?.GetSignature() ?? "null");
                }

                StringBuilder displayBuilder = new StringBuilder();
                displayBuilder.Append(PhaseKey).Append('|').Append(StageLabel).Append('|').Append(Items.Count);
                for (int index = 0; index < Items.Count; index++)
                {
                    displayBuilder.Append('|').Append(Items[index]?.Label ?? "null");
                }

                DisplaySignature = displayBuilder.ToString();
                Signature = builder.ToString();
            }

            private static PromptRowState CreateSanitizedRowState(SpringDay1Director.PromptTaskItem item, string focusText, string subtitle, string footerText)
            {
                string detail = FirstNonEmpty(item.Detail, focusText, footerText, subtitle, "等待剧情推进。");
                return new PromptRowState
                {
                    Label = FirstNonEmpty(item.Label, "当前任务"),
                    Detail = detail,
                    Completed = item.Completed
                };
            }

            private static string FirstNonEmpty(params string[] values)
            {
                if (values == null)
                {
                    return string.Empty;
                }

                for (int index = 0; index < values.Length; index++)
                {
                    if (!string.IsNullOrWhiteSpace(values[index]))
                    {
                        return values[index];
                    }
                }

                return string.Empty;
            }

            private static int ResolvePrimaryItemIndex(SpringDay1Director.PromptTaskItem[] items)
            {
                if (items == null || items.Length == 0)
                {
                    return 0;
                }

                for (int index = 0; index < items.Length; index++)
                {
                    if (!items[index].Completed)
                    {
                        return index;
                    }
                }

                return 0;
            }

            private static bool HasIncompleteItems(SpringDay1Director.PromptTaskItem[] items)
            {
                if (items == null)
                {
                    return false;
                }

                for (int index = 0; index < items.Length; index++)
                {
                    if (!items[index].Completed)
                    {
                        return true;
                    }
                }

                return false;
            }

            private static bool TryAddVisibleItem(
                PromptCardViewState state,
                SpringDay1Director.PromptTaskItem[] items,
                bool[] consumed,
                int index,
                string focusText,
                string subtitle,
                string footerText)
            {
                if (state == null
                    || items == null
                    || consumed == null
                    || index < 0
                    || index >= items.Length
                    || consumed[index])
                {
                    return false;
                }

                state.Items.Add(CreateSanitizedRowState(items[index], focusText, subtitle, footerText));
                consumed[index] = true;
                return true;
            }
        }
    }
}
