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

        private static SpringDay1PromptOverlay _instance;

        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private RectTransform cardRect;
        [SerializeField] private RectTransform pageRect;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI focusText;
        [SerializeField] private TextMeshProUGUI footerText;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private float completionStepDuration = 0.32f;
        [SerializeField] private float pageFlipDuration = 0.42f;
        [SerializeField] private float postDialogueResumeDelay = 0.18f;

        private const float MinPageHeight = 178f;
        private const float MaxPageHeight = 460f;
        private const float LegacyPageWidth = 316f;

        private readonly List<RowRefs> _rows = new();
        private TMP_FontAsset _fontAsset;
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
        private PageRefs _frontPage;
        private PageRefs _backPage;

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

            PromptCardViewState nextState = BuildCurrentViewState();
            _pendingState = nextState;

            if (ShouldDelayPromptDisplay())
            {
                if (canvasGroup.alpha > 0.001f)
                {
                    FadeCanvasGroup(0f, false);
                }

                return;
            }

            if (_pendingState == null)
            {
                FadeCanvasGroup(0f, false);
                return;
            }

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

            if (canvasGroup.alpha < 0.999f && _queuedRevealCoroutine == null)
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

            EnsureBuilt();
            string currentPhaseKey = SpringDay1Director.Instance != null
                ? SpringDay1Director.Instance.BuildPromptCardModel()?.PhaseKey ?? string.Empty
                : string.Empty;
            if (_manualPromptText == text
                && string.Equals(_queuedPromptText, text)
                && string.Equals(_manualPromptPhaseKey, currentPhaseKey, StringComparison.Ordinal))
            {
                return;
            }

            _manualPromptText = text;
            _manualPromptPhaseKey = currentPhaseKey;
            _queuedPromptText = text;

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

        public void Hide()
        {
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

        private void OnDialogueStart(DialogueStartEvent _)
        {
            _suppressWhileDialogueActive = false;
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            _suppressWhileDialogueActive = false;
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
            cardRect.anchoredPosition = new Vector2(26f, 8f);
            cardRect.sizeDelta = new Vector2(328f, 188f);

            Image shadowPlate = cardRect.gameObject.AddComponent<Image>();
            shadowPlate.color = new Color(0f, 0f, 0f, 0.18f);
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
        }

        private void EnsureBuilt()
        {
            if (rootRect != null && canvasGroup != null && cardRect != null && _frontPage != null && _backPage != null)
            {
                return;
            }

            if (TryBindRuntimeShell())
            {
                return;
            }

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

            RowRefs firstRow = _frontPage.rows[0];
            PromptRowState firstState = state.Items[0];
            return firstRow?.root == null
                || !firstRow.root.gameObject.activeSelf
                || !HasReadablePrimaryText(firstRow.label, firstState.Label)
                || !HasReadablePrimaryText(firstRow.detail, firstState.Detail);
        }

        private static bool HasReadablePrimaryText(TextMeshProUGUI text, string expected)
        {
            if (string.IsNullOrWhiteSpace(expected))
            {
                return true;
            }

            return text != null
                && text.enabled
                && text.gameObject.activeInHierarchy
                && IsFontAssetUsable(text.font)
                && !string.IsNullOrWhiteSpace(text.text);
        }

        private void ClearRuntimeShellForRebuild()
        {
            if (rootRect == null)
            {
                return;
            }

            DestroyDirectChildIfExists(rootRect, "TaskCardRoot");
            cardRect = null;
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

        private void ApplyRuntimeCanvasDefaults()
        {
            if (overlayCanvas != null)
            {
                overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                overlayCanvas.worldCamera = null;
                overlayCanvas.planeDistance = 100f;
                overlayCanvas.overrideSorting = true;
                overlayCanvas.sortingOrder = 152;
                overlayCanvas.pixelPerfect = true;
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
                defaultPivot = pageRoot.pivot
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
                page.stageTagRoot = FindDirectChildRect(pageRoot, "StageTag") ?? page.titleText.rectTransform.parent as RectTransform;
                page.subtitleRoot = page.subtitleText.rectTransform;
                page.headerDividerRoot = FindDirectChildRect(pageRoot, "HeaderDivider");
                page.focusRibbonRoot = FindDirectChildRect(pageRoot, "FocusRibbon") ?? page.focusText.rectTransform.parent as RectTransform;
                page.footerRoot = FindDirectChildRect(pageRoot, "FooterRoot") ?? page.footerText.rectTransform;
                PrepareLegacyPage(page);
            }

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

            page.titleText.textWrappingMode = TextWrappingModes.NoWrap;
            page.titleText.overflowMode = TextOverflowModes.Ellipsis;
            page.subtitleText.textWrappingMode = TextWrappingModes.Normal;
            page.subtitleText.overflowMode = TextOverflowModes.Overflow;
            page.focusText.textWrappingMode = TextWrappingModes.Normal;
            page.focusText.overflowMode = TextOverflowModes.Overflow;
            page.footerText.textWrappingMode = TextWrappingModes.Normal;
            page.footerText.overflowMode = TextOverflowModes.Overflow;
            page.footerBaselineTop = page.footerRoot != null ? GetTopInParent(page.footerRoot) : 0f;
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
            float currentTop = 4f;

            if (page.stageTagRoot != null)
            {
                SetTopKeepingHorizontal(page.stageTagRoot, currentTop, Mathf.Max(22f, page.stageTagRoot.rect.height));
                currentTop += Mathf.Max(28f, page.stageTagRoot.rect.height + 6f);
            }

            float subtitleWidth = page.subtitleRoot != null && page.subtitleRoot.rect.width > 1f
                ? page.subtitleRoot.rect.width
                : pageWidth - 36f;
            float subtitleHeight = MeasureTextHeight(page.subtitleText, subtitleWidth, 18f);
            if (page.subtitleRoot != null)
            {
                SetTopKeepingHorizontal(page.subtitleRoot, currentTop, subtitleHeight);
            }

            currentTop += subtitleHeight + 8f;
            if (page.headerDividerRoot != null)
            {
                SetTopKeepingHorizontal(page.headerDividerRoot, currentTop, Mathf.Max(2f, page.headerDividerRoot.rect.height));
                currentTop += Mathf.Max(10f, page.headerDividerRoot.rect.height + 8f);
            }

            float taskWidth = page.taskListRoot != null && page.taskListRoot.rect.width > 1f
                ? page.taskListRoot.rect.width
                : pageWidth - 36f;
            float taskHeight = LayoutLegacyRows(page, taskWidth);
            if (page.taskListRoot != null)
            {
                SetTopKeepingHorizontal(page.taskListRoot, currentTop, taskHeight);
            }

            currentTop += taskHeight + 4f;
            bool hasFocus = page.focusRibbonRoot != null && !string.IsNullOrWhiteSpace(page.focusText.text);
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
                float focusHeight = Mathf.Max(20f, focusTextHeight + 8f);
                SetTopKeepingHorizontal(page.focusRibbonRoot, currentTop, focusHeight);
                currentTop += focusHeight + 4f;
            }

            bool hasFooter = page.footerRoot != null && !string.IsNullOrWhiteSpace(page.footerText.text);
            float footerWidth = page.footerRoot != null && page.footerRoot.rect.width > 1f
                ? page.footerRoot.rect.width
                : pageWidth - 44f;
            float footerMinHeight = page.footerRoot != null
                ? Mathf.Max(14f, page.footerRoot.rect.height)
                : (string.IsNullOrWhiteSpace(page.footerText.text) ? 12f : 14f);
            float footerHeight = MeasureTextHeight(page.footerText, footerWidth, footerMinHeight);
            if (page.footerRoot != null)
            {
                page.footerRoot.gameObject.SetActive(hasFooter);
            }

            if (hasFooter && page.footerRoot != null)
            {
                float footerTop = Mathf.Max(page.footerBaselineTop, currentTop);
                SetTopKeepingHorizontal(page.footerRoot, footerTop, footerHeight);
                currentTop = footerTop + footerHeight;
            }

            page.preferredHeight = Mathf.Clamp(currentTop + 10f, MinPageHeight, MaxPageHeight);
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

            if (activeCount > 0)
            {
                currentTop -= 8f;
            }

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
            float rowHeight = Mathf.Max(58f, 8f + labelHeight + (detailHeight > 0f ? 2f + detailHeight : 0f) + 8f);

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
            if (_fontAsset == null || rootRect == null)
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

                text.font = _fontAsset;
                if (_fontAsset.material != null)
                {
                    text.fontSharedMaterial = _fontAsset.material;
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

            if (!IsFontAssetUsable(text.font))
            {
                if (_fontAsset == null)
                {
                    _fontAsset = ResolveFontAsset();
                }

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

        private static void SetTopKeepingHorizontal(RectTransform rect, float top, float height)
        {
            if (rect == null)
            {
                return;
            }

            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            Vector2 pivot = rect.pivot;

            if (Mathf.Abs(rect.anchorMin.x - rect.anchorMax.x) > 0.001f)
            {
                Vector2 offsetMin = rect.offsetMin;
                Vector2 offsetMax = rect.offsetMax;
                rect.anchorMin = new Vector2(anchorMin.x, 1f);
                rect.anchorMax = new Vector2(anchorMax.x, 1f);
                rect.pivot = new Vector2(pivot.x, 1f);
                rect.offsetMin = new Vector2(offsetMin.x, -top - height);
                rect.offsetMax = new Vector2(offsetMax.x, -top);
                return;
            }

            Vector2 anchoredPosition = rect.anchoredPosition;
            Vector2 sizeDelta = rect.sizeDelta;
            rect.anchorMin = new Vector2(anchorMin.x, 1f);
            rect.anchorMax = new Vector2(anchorMax.x, 1f);
            rect.pivot = new Vector2(pivot.x, 1f);
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
            page.root.sizeDelta = new Vector2(316f, 178f);
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
            subtitleRoot.gameObject.AddComponent<LayoutElement>().minHeight = 26f;
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
            page.taskListRoot.gameObject.AddComponent<LayoutElement>().minHeight = 30f;

            RectTransform focusRibbon = CreateRect(content, "FocusRibbon");
            focusRibbon.gameObject.AddComponent<LayoutElement>().minHeight = 30f;
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
            footerRoot.gameObject.AddComponent<LayoutElement>().minHeight = 18f;
            page.footerText = CreateText(footerRoot, "FooterText", string.Empty, 10f, new Color(0.42f, 0.31f, 0.18f, 0.92f), TextAlignmentOptions.BottomLeft, true);
            Stretch(page.footerText.rectTransform, Vector2.zero, Vector2.zero);

            page.pageCurl = EnsurePageCurl(page.root);
            page.pageCurlImage = page.pageCurl.GetComponent<Image>();

            page.contentRoot = content;
            page.defaultPosition = page.root.anchoredPosition;
            page.defaultPivot = page.root.pivot;
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

            SpringDay1Director.PromptCardModel model = director.BuildPromptCardModel();
            if (model == null)
            {
                return string.IsNullOrWhiteSpace(_manualPromptText)
                    ? null
                    : BuildManualState(_manualPromptText);
            }

            if (!string.IsNullOrWhiteSpace(_manualPromptText)
                && !string.IsNullOrWhiteSpace(_manualPromptPhaseKey)
                && !string.Equals(_manualPromptPhaseKey, model.PhaseKey, StringComparison.Ordinal))
            {
                _manualPromptText = string.Empty;
                _manualPromptPhaseKey = string.Empty;
                _queuedPromptText = string.Empty;
            }

            return PromptCardViewState.FromModel(
                model,
                string.IsNullOrWhiteSpace(_manualPromptText) ? model.FocusText : _manualPromptText,
                maxVisibleItems: 1);
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

            bool requiresFlip = _displayedState == null
                || _displayedState.DisplaySignature != targetState.DisplaySignature;

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
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / completionStepDuration);
                float eased = 1f - ((1f - normalized) * (1f - normalized));
                row.plate.color = Color.Lerp(startPlate, endPlate, eased);
                row.bulletFill.color = Color.Lerp(startFill, endFill, eased);
                row.label.color = Color.Lerp(startLabel, endLabel, eased);
                row.group.alpha = Mathf.Lerp(1f, 0.92f, eased);
                yield return null;
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
            page.appliedState = state;
            EnsurePageTextChainReady(page);
            page.titleText.text = state.StageLabel;
            page.subtitleText.text = state.Subtitle;
            page.focusText.text = state.FocusText;
            page.footerText.text = state.FooterText;
            page.titleText.ForceMeshUpdate();
            page.subtitleText.ForceMeshUpdate();
            page.focusText.ForceMeshUpdate();
            page.footerText.ForceMeshUpdate();

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

        private void ApplyRowsToPage(PageRefs page, PromptCardViewState state)
        {
            for (int index = 0; index < page.rows.Count; index++)
            {
                if (index < state.Items.Count)
                {
                    ApplyRow(page.rows[index], state.Items[index], false);
                    page.rows[index].root.gameObject.SetActive(true);
                }
                else
                {
                    page.rows[index].root.gameObject.SetActive(false);
                }
            }
        }

        private bool PageMatchesState(PageRefs page, PromptCardViewState state)
        {
            if (page == null || state == null || page.rows == null)
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
                if (row?.root == null || row.label == null || row.detail == null || !row.root.gameObject.activeSelf)
                {
                    return false;
                }

                if (!string.Equals(row.label.text, expected.Label, StringComparison.Ordinal)
                    || !string.Equals(row.detail.text, expected.Detail, StringComparison.Ordinal))
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
            row.group.alpha = 1f;
            EnsureRowTextChainReady(row);
            row.label.text = state.Label;
            row.detail.text = state.Detail;
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
            if (page == null)
            {
                return;
            }

            EnsurePromptTextReady(page.titleText, forceActive: true);
            EnsurePromptTextReady(page.subtitleText, forceActive: true);
            EnsurePromptTextReady(page.focusText, forceActive: true);
            EnsurePromptTextReady(page.footerText, forceActive: true);

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
            if (row == null)
            {
                return;
            }

            if (row.root != null && !row.root.gameObject.activeSelf)
            {
                row.root.gameObject.SetActive(true);
            }

            EnsurePromptTextReady(row.label, forceActive: true);
            EnsurePromptTextReady(row.detail, forceActive: true);
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

            if (!IsFontAssetUsable(text.font))
            {
                if (_fontAsset == null)
                {
                    _fontAsset = ResolveFontAsset();
                }

                if (_fontAsset != null)
                {
                    text.font = _fontAsset;
                    if (_fontAsset.material != null)
                    {
                        text.fontSharedMaterial = _fontAsset.material;
                    }
                }
            }
        }

        private void RefreshCardLayout(PromptCardViewState frontState, PromptCardViewState backState)
        {
            RefreshPageContentLayout(_frontPage, frontState);
            RefreshPageContentLayout(_backPage, backState);
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
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(page.taskListRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(page.contentRoot);
        }

        private void ResetPageVisual(PageRefs page)
        {
            if (page == null)
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
            if (page == null)
            {
                return;
            }

            page.root.gameObject.SetActive(visible);
            page.canvasGroup.alpha = visible ? 1f : 0f;
        }

        private bool ShouldDelayPromptDisplay()
        {
            if (_suppressWhileDialogueActive)
            {
                return true;
            }

            if (SpringDay1UiLayerUtility.IsBlockingPageUiOpen())
            {
                return true;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = UnityEngine.Object.FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            return overlay != null && overlay.IsVisible;
        }

        private bool ShouldIgnoreDialogueEndEvent()
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            return dialogueManager != null && dialogueManager.IsDialogueActive;
        }

        private void QueuePromptReveal()
        {
            if (string.IsNullOrWhiteSpace(_queuedPromptText))
            {
                return;
            }

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

            if (immediate || fadeDuration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                return;
            }

            _visibilityCoroutine = StartCoroutine(FadeCanvasGroupRoutine(targetAlpha));
        }

        private IEnumerator FadeCanvasGroupRoutine(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / fadeDuration);
                float eased = 1f - ((1f - normalized) * (1f - normalized));
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            _visibilityCoroutine = null;
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

        private TMP_FontAsset ResolveFontAsset()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (IsFontAssetUsable(candidate))
                {
                    return candidate;
                }
            }

            return TMP_Settings.defaultFontAsset;
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
            return SpringDay1UiLayerUtility.ResolveUiParent();
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

            RectTransform frontRoot = candidate.pageRect != null ? candidate.pageRect : FindDirectChildRect(candidateCard, "Page");
            if (frontRoot == null || !CanBindPageRoot(frontRoot))
            {
                return false;
            }

            RectTransform backRoot = FindDirectChildRect(candidateCard, "BackPage");
            return backRoot == null || CanBindPageRoot(backRoot);
        }

        private static bool CanBindPageRoot(RectTransform pageRoot)
        {
            if (pageRoot == null)
            {
                return false;
            }

            if (FindDescendantComponent<TextMeshProUGUI>(pageRoot, "TitleText") == null
                || FindDescendantComponent<TextMeshProUGUI>(pageRoot, "SubtitleText") == null
                || FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FocusText") == null
                || FindDescendantComponent<TextMeshProUGUI>(pageRoot, "FooterText") == null)
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
                && FindDescendantComponent<TextMeshProUGUI>(rowRect, "Label") != null
                && FindDescendantComponent<TextMeshProUGUI>(rowRect, "Detail") != null;
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
            public bool usesLegacyManualLayout;
            public float preferredHeight;
            public float footerBaselineTop;
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

        private sealed class PromptRowState
        {
            public string Label;
            public string Detail;
            public bool Completed;

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
                    int primaryIndex = ResolvePrimaryItemIndex(model.Items);
                    int visibleCount = 0;
                    for (int index = 0; index < model.Items.Length; index++)
                    {
                        if (index != primaryIndex && visibleCount >= Mathf.Max(1, maxVisibleItems))
                        {
                            continue;
                        }

                        if (index != primaryIndex && maxVisibleItems <= 1)
                        {
                            continue;
                        }

                        state.Items.Add(CreateSanitizedRowState(model.Items[index], focusText, model.Subtitle, model.FooterText));
                        visibleCount++;

                        if (visibleCount >= Mathf.Max(1, maxVisibleItems))
                        {
                            break;
                        }
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

                StringBuilder builder = new StringBuilder();
                builder.Append(state.PhaseKey).Append('|').Append(state.StageLabel).Append('|').Append(state.Subtitle).Append('|').Append(state.FocusText).Append('|').Append(state.FooterText);
                for (int index = 0; index < state.Items.Count; index++)
                {
                    builder.Append('|').Append(state.Items[index].GetSignature());
                }

                StringBuilder displayBuilder = new StringBuilder();
                displayBuilder.Append(state.PhaseKey).Append('|').Append(state.StageLabel).Append('|').Append(state.Items.Count);
                for (int index = 0; index < state.Items.Count; index++)
                {
                    displayBuilder.Append('|').Append(state.Items[index].Label);
                }

                state.DisplaySignature = displayBuilder.ToString();
                state.Signature = builder.ToString();
                return state;
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
        }
    }
}
