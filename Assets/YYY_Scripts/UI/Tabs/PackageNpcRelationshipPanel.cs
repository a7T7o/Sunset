using System.Collections.Generic;
using System.Text;
using TMPro;
using Sunset.Story;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PackageNpcRelationshipPanel : MonoBehaviour
{
    private const string RuntimeRootName = "NpcRelationshipRuntimeRoot";

    private static readonly Color PageTint = new Color(0.97f, 0.92f, 0.84f, 0.97f);
    private static readonly Color CardTint = new Color(0.97f, 0.90f, 0.79f, 0.96f);
    private static readonly Color CardStrongTint = new Color(0.96f, 0.88f, 0.74f, 0.98f);
    private static readonly Color ViewportTint = new Color(0.95f, 0.89f, 0.81f, 0.98f);
    private static readonly Color InkTint = new Color(0.22f, 0.13f, 0.08f, 1f);
    private static readonly Color SubtleTint = new Color(0.40f, 0.27f, 0.18f, 1f);
    private static readonly Color MutedTint = new Color(0.49f, 0.35f, 0.24f, 0.92f);
    private static readonly Color SelectedCardTint = new Color(0.98f, 0.89f, 0.75f, 0.98f);
    private static readonly Color DefaultCardTint = new Color(0.99f, 0.95f, 0.87f, 0.62f);
    private static readonly Color SelectedAccentTint = new Color(0.78f, 0.44f, 0.12f, 1f);
    private static readonly Color DefaultAccentTint = new Color(0.69f, 0.54f, 0.36f, 0.46f);
    private static readonly Color ScrollbarTint = new Color(0.43f, 0.29f, 0.18f, 0.28f);
    private static readonly Color ScrollbarHandleTint = new Color(0.71f, 0.50f, 0.25f, 0.90f);
    private const float ListChipColumnWidth = 76f;
    private const float ListPresenceChipWidth = 68f;
    private const float ListStageChipWidth = 64f;
    private const float ListChipHeight = 23f;
    private const float ListChipTextFontSize = 10.5f;

    private readonly List<NpcCardRefs> _cardRefs = new List<NpcCardRefs>();
    private readonly List<NpcEntryViewModel> _entries = new List<NpcEntryViewModel>();
    private readonly Image[] _stageSegments = new Image[4];

    private RectTransform _runtimeRoot;
    private RectTransform _listContent;
    private TextMeshProUGUI _summaryText;
    private TextMeshProUGUI _listPhaseChipText;
    private Image _listPhaseChipImage;
    private LayoutElement _portraitFrameLayout;
    private Image _portraitImage;
    private TextMeshProUGUI _portraitPlaceholderText;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _roleText;
    private TextMeshProUGUI _quoteText;
    private TextMeshProUGUI _heroPresenceValueText;
    private TextMeshProUGUI _heroBaselineValueText;
    private TextMeshProUGUI _heroPhaseValueText;
    private TextMeshProUGUI _stageChipText;
    private Image _stageChipImage;
    private TextMeshProUGUI _stageHintText;
    private TextMeshProUGUI _roleSummaryText;
    private TextMeshProUGUI _presenceText;
    private TextMeshProUGUI _footerText;
    private bool _built;
    private string _selectedNpcId;

    private sealed class NpcCardRefs
    {
        public string NpcId;
        public Button Button;
        public Image Accent;
        public Image Background;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Preview;
    }

    private sealed class NpcEntryViewModel
    {
        public string NpcId;
        public string DisplayName;
        public string RoleSummary;
        public string Preview;
        public NPCRelationshipStage Stage;
        public SpringDay1CrowdResidentPresenceLevel PresenceLevel;
        public string PresenceNote;
        public SpringDay1CrowdResidentBaseline Baseline;
        public Sprite Portrait;
    }

    public static void EnsureInstalled(GameObject panelRoot)
    {
        RectTransform main = FindPageMain(panelRoot != null ? panelRoot.transform : null, "4_Relationship_NPC");
        if (main == null)
        {
            return;
        }

        PackageNpcRelationshipPanel panel = main.GetComponent<PackageNpcRelationshipPanel>();
        if (panel == null)
        {
            panel = main.gameObject.AddComponent<PackageNpcRelationshipPanel>();
        }

        panel.EnsureBuilt();
        panel.RefreshView();
    }

    private void Awake() => EnsureBuilt();

    private void OnEnable()
    {
        EnsureBuilt();
        RefreshView();
    }

    private void EnsureBuilt()
    {
        if (_built && _runtimeRoot != null && HasRequiredRuntimeBindings())
        {
            PreparePageHost();
            return;
        }

        Transform oldRoot = transform.Find(RuntimeRootName);
        if (oldRoot != null)
        {
            if (Application.isPlaying)
            {
                Destroy(oldRoot.gameObject);
            }
            else
            {
                DestroyImmediate(oldRoot.gameObject);
            }
        }

        PreparePageHost();
        _cardRefs.Clear();
        BuildRuntimeUi();
        _built = true;
    }

    private void PreparePageHost()
    {
        for (int index = transform.childCount - 1; index >= 0; index--)
        {
            Transform child = transform.GetChild(index);
            if (child == null || string.Equals(child.name, RuntimeRootName, System.StringComparison.Ordinal))
            {
                continue;
            }

            child.gameObject.SetActive(false);
        }
    }

    private bool HasRequiredRuntimeBindings()
    {
        if (_runtimeRoot == null
            || _listContent == null
            || _summaryText == null
            || _portraitImage == null
            || _nameText == null
            || _roleText == null
            || _quoteText == null
            || _heroPresenceValueText == null
            || _heroBaselineValueText == null
            || _heroPhaseValueText == null
            || _stageChipText == null
            || _stageChipImage == null
            || _stageHintText == null
            || _roleSummaryText == null
            || _presenceText == null
            || _footerText == null)
        {
            return false;
        }

        return _stageSegments != null && _stageSegments.Length == 4;
    }

    private void BuildRuntimeUi()
    {
        _runtimeRoot = PackagePanelRuntimeUiKit.CreatePanel(RuntimeRootName, transform, PageTint, false, new Color(0.30f, 0.19f, 0.10f, 0.22f), new Vector2(2f, -2f));
        PackagePanelRuntimeUiKit.Stretch(_runtimeRoot, 14f, 14f, 14f, 14f);

        VerticalLayoutGroup shellLayout = _runtimeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        shellLayout.padding = new RectOffset(18, 18, 18, 18);
        shellLayout.spacing = 12f;
        shellLayout.childAlignment = TextAnchor.UpperLeft;
        shellLayout.childControlWidth = true;
        shellLayout.childControlHeight = true;
        shellLayout.childForceExpandWidth = true;
        shellLayout.childForceExpandHeight = false;

        RectTransform header = PackagePanelRuntimeUiKit.CreatePanel("Header", _runtimeRoot, CardStrongTint, false, new Color(0.35f, 0.21f, 0.11f, 0.22f), new Vector2(2f, -2f));
        AddLayoutElement(header, preferredHeight: 84f);

        VerticalLayoutGroup headerLayout = header.gameObject.AddComponent<VerticalLayoutGroup>();
        headerLayout.padding = new RectOffset(20, 20, 16, 14);
        headerLayout.spacing = 3f;
        headerLayout.childAlignment = TextAnchor.UpperLeft;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = true;
        headerLayout.childForceExpandHeight = false;

        TextMeshProUGUI title = PackagePanelRuntimeUiKit.CreateText("Title", header, "村民关系册", 28f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)title.transform, preferredHeight: 34f);

        TextMeshProUGUI subtitle = PackagePanelRuntimeUiKit.CreateText(
            "Subtitle",
            header,
            "今天真正会碰见的人，会在这里留下印象。",
            14f,
            SubtleTint,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            false,
            TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)subtitle.transform, preferredHeight: 24f);

        RectTransform content = PackagePanelRuntimeUiKit.CreateRect("Content", _runtimeRoot);
        AddLayoutElement(content, minHeight: 352f, flexibleHeight: 1f);

        HorizontalLayoutGroup contentLayout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 0, 0);
        contentLayout.spacing = 12f;
        contentLayout.childAlignment = TextAnchor.UpperLeft;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = false;
        contentLayout.childForceExpandHeight = true;

        BuildListPanel(content);
        BuildDetailPanel(content);
    }

    private void BuildListPanel(RectTransform content)
    {
        RectTransform listPanel = PackagePanelRuntimeUiKit.CreatePanel("ListPanel", content, CardTint, false, new Color(0.35f, 0.21f, 0.11f, 0.18f), new Vector2(2f, -2f));
        AddLayoutElement(listPanel, minWidth: 300f, preferredWidth: 326f, flexibleWidth: 0.68f, flexibleHeight: 1f);

        VerticalLayoutGroup listPanelLayout = listPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        listPanelLayout.padding = new RectOffset(18, 18, 18, 18);
        listPanelLayout.spacing = 12f;
        listPanelLayout.childAlignment = TextAnchor.UpperLeft;
        listPanelLayout.childControlWidth = true;
        listPanelLayout.childControlHeight = true;
        listPanelLayout.childForceExpandWidth = true;
        listPanelLayout.childForceExpandHeight = false;

        RectTransform listHeader = PackagePanelRuntimeUiKit.CreateRect("ListHeader", listPanel);
        AddLayoutElement(listHeader, preferredHeight: 28f);
        TextMeshProUGUI listLabel = PackagePanelRuntimeUiKit.CreateText("ListLabel", listHeader, "今日名册", 19f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left);
        RectTransform listLabelRect = (RectTransform)listLabel.transform;
        listLabelRect.anchorMin = new Vector2(0f, 0.5f);
        listLabelRect.anchorMax = new Vector2(1f, 0.5f);
        listLabelRect.pivot = new Vector2(0f, 0.5f);
        listLabelRect.anchoredPosition = Vector2.zero;
        listLabelRect.sizeDelta = new Vector2(-102f, 26f);

        RectTransform listPhaseChip = PackagePanelRuntimeUiKit.CreatePanel("ListPhaseChip", listHeader, new Color(0.92f, 0.76f, 0.36f, 0.98f), false, new Color(0.34f, 0.20f, 0.10f, 0.20f));
        listPhaseChip.anchorMin = new Vector2(1f, 0.5f);
        listPhaseChip.anchorMax = new Vector2(1f, 0.5f);
        listPhaseChip.pivot = new Vector2(1f, 0.5f);
        listPhaseChip.anchoredPosition = Vector2.zero;
        listPhaseChip.sizeDelta = new Vector2(92f, 22f);
        _listPhaseChipImage = listPhaseChip.GetComponent<Image>();

        _listPhaseChipText = PackagePanelRuntimeUiKit.CreateText("ListPhaseChipText", listPhaseChip, string.Empty, 12.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)_listPhaseChipText.transform, 6f, 6f, 4f, 4f);

        _summaryText = PackagePanelRuntimeUiKit.CreateText("SummaryText", listPanel, string.Empty, 13.5f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, true, TextOverflowModes.Ellipsis);
        _summaryText.lineSpacing = 3f;
        AddLayoutElement((RectTransform)_summaryText.transform, minHeight: 42f, preferredHeight: 44f);

        RectTransform scrollRoot = PackagePanelRuntimeUiKit.CreateRect("ScrollRoot", listPanel);
        AddLayoutElement(scrollRoot, minHeight: 0f, flexibleHeight: 1f);

        RectTransform viewport = PackagePanelRuntimeUiKit.CreatePanel("Viewport", scrollRoot, ViewportTint, false, new Color(0.35f, 0.21f, 0.11f, 0.18f));
        PackagePanelRuntimeUiKit.SetAnchors(viewport, Vector2.zero, Vector2.one, new Vector2(0f, 0f), new Vector2(-12f, 0f), new Vector2(0.5f, 0.5f));
        viewport.gameObject.AddComponent<RectMask2D>();

        _listContent = PackagePanelRuntimeUiKit.CreateRect("Content", viewport);
        _listContent.anchorMin = new Vector2(0f, 1f);
        _listContent.anchorMax = new Vector2(1f, 1f);
        _listContent.pivot = new Vector2(0.5f, 1f);
        _listContent.anchoredPosition = Vector2.zero;
        _listContent.sizeDelta = Vector2.zero;

        VerticalLayoutGroup contentLayout = _listContent.gameObject.AddComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 0, 0);
        contentLayout.spacing = 10f;
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        ContentSizeFitter contentFitter = _listContent.gameObject.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = _listContent;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.inertia = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 6f;

        RectTransform scrollbarRoot = PackagePanelRuntimeUiKit.CreatePanel("Scrollbar", scrollRoot, ScrollbarTint, false, new Color(0f, 0f, 0f, 0f));
        PackagePanelRuntimeUiKit.SetAnchors(scrollbarRoot, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-8f, 0f), Vector2.zero, new Vector2(1f, 0.5f));

        RectTransform slidingArea = PackagePanelRuntimeUiKit.CreateRect("SlidingArea", scrollbarRoot);
        PackagePanelRuntimeUiKit.Stretch(slidingArea, 0f, 0f, 6f, 6f);

        RectTransform handle = PackagePanelRuntimeUiKit.CreatePanel("Handle", slidingArea, ScrollbarHandleTint, false, new Color(0f, 0f, 0f, 0f));
        PackagePanelRuntimeUiKit.Stretch(handle, 0f, 0f, 0f, 0f);

        Scrollbar scrollbar = scrollbarRoot.gameObject.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.handleRect = handle;
        scrollbar.targetGraphic = handle.GetComponent<Image>();
        scrollbar.size = 0.35f;
        scrollRect.verticalScrollbar = scrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
    }

    private void BuildDetailPanel(RectTransform content)
    {
        RectTransform detailPanel = PackagePanelRuntimeUiKit.CreatePanel("DetailPanel", content, CardStrongTint, false, new Color(0.35f, 0.21f, 0.11f, 0.20f), new Vector2(2f, -2f));
        AddLayoutElement(detailPanel, minWidth: 420f, flexibleWidth: 1.24f, flexibleHeight: 1f);

        VerticalLayoutGroup detailPanelLayout = detailPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        detailPanelLayout.padding = new RectOffset(18, 18, 18, 18);
        detailPanelLayout.spacing = 12f;
        detailPanelLayout.childAlignment = TextAnchor.UpperLeft;
        detailPanelLayout.childControlWidth = true;
        detailPanelLayout.childControlHeight = true;
        detailPanelLayout.childForceExpandWidth = true;
        detailPanelLayout.childForceExpandHeight = false;

        TextMeshProUGUI detailLabel = PackagePanelRuntimeUiKit.CreateText("DetailLabel", detailPanel, "人物档案", 17f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)detailLabel.transform, preferredHeight: 24f);

        RectTransform detailScrollRoot = PackagePanelRuntimeUiKit.CreateRect("DetailScrollRoot", detailPanel);
        AddLayoutElement(detailScrollRoot, minHeight: 276f, flexibleHeight: 1f);

        RectTransform viewport = PackagePanelRuntimeUiKit.CreatePanel("Viewport", detailScrollRoot, ViewportTint, false, new Color(0.35f, 0.21f, 0.11f, 0.14f));
        PackagePanelRuntimeUiKit.SetAnchors(viewport, Vector2.zero, Vector2.one, new Vector2(0f, 0f), new Vector2(-12f, 0f), new Vector2(0.5f, 0.5f));
        viewport.gameObject.AddComponent<RectMask2D>();

        RectTransform detailContent = PackagePanelRuntimeUiKit.CreateRect("DetailContent", viewport);
        detailContent.anchorMin = new Vector2(0f, 1f);
        detailContent.anchorMax = new Vector2(1f, 1f);
        detailContent.pivot = new Vector2(0.5f, 1f);
        detailContent.anchoredPosition = Vector2.zero;
        detailContent.sizeDelta = Vector2.zero;

        VerticalLayoutGroup detailLayout = detailContent.gameObject.AddComponent<VerticalLayoutGroup>();
        detailLayout.padding = new RectOffset(12, 12, 12, 12);
        detailLayout.spacing = 12f;
        detailLayout.childAlignment = TextAnchor.UpperLeft;
        detailLayout.childControlWidth = true;
        detailLayout.childControlHeight = true;
        detailLayout.childForceExpandWidth = true;
        detailLayout.childForceExpandHeight = false;

        ContentSizeFitter detailFitter = detailContent.gameObject.AddComponent<ContentSizeFitter>();
        detailFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect detailScrollRect = detailScrollRoot.gameObject.AddComponent<ScrollRect>();
        detailScrollRect.viewport = viewport;
        detailScrollRect.content = detailContent;
        detailScrollRect.horizontal = false;
        detailScrollRect.vertical = true;
        detailScrollRect.inertia = false;
        detailScrollRect.movementType = ScrollRect.MovementType.Clamped;
        detailScrollRect.scrollSensitivity = 6f;

        RectTransform scrollbarRoot = PackagePanelRuntimeUiKit.CreatePanel("Scrollbar", detailScrollRoot, ScrollbarTint, false, new Color(0f, 0f, 0f, 0f));
        PackagePanelRuntimeUiKit.SetAnchors(scrollbarRoot, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-8f, 0f), Vector2.zero, new Vector2(1f, 0.5f));

        RectTransform slidingArea = PackagePanelRuntimeUiKit.CreateRect("SlidingArea", scrollbarRoot);
        PackagePanelRuntimeUiKit.Stretch(slidingArea, 0f, 0f, 6f, 6f);

        RectTransform handle = PackagePanelRuntimeUiKit.CreatePanel("Handle", slidingArea, ScrollbarHandleTint, false, new Color(0f, 0f, 0f, 0f));
        PackagePanelRuntimeUiKit.Stretch(handle, 0f, 0f, 0f, 0f);

        Scrollbar scrollbar = scrollbarRoot.gameObject.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.handleRect = handle;
        scrollbar.targetGraphic = handle.GetComponent<Image>();
        scrollbar.size = 0.35f;
        detailScrollRect.verticalScrollbar = scrollbar;
        detailScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

        BuildHeroCard(detailContent);
        BuildStageCard(detailContent);
        BuildNarrativeRow(detailContent);
        BuildFooterCard(detailContent);
    }

    private void BuildHeroCard(RectTransform parent)
    {
        RectTransform heroCard = PackagePanelRuntimeUiKit.CreatePanel("HeroCard", parent, new Color(0.98f, 0.91f, 0.79f, 0.98f), false, new Color(0.35f, 0.21f, 0.11f, 0.20f), new Vector2(2f, -2f));
        AddLayoutElement(heroCard, minHeight: 194f);

        VerticalLayoutGroup heroLayout = heroCard.gameObject.AddComponent<VerticalLayoutGroup>();
        heroLayout.padding = new RectOffset(18, 18, 18, 18);
        heroLayout.spacing = 12f;
        heroLayout.childAlignment = TextAnchor.UpperLeft;
        heroLayout.childControlWidth = true;
        heroLayout.childControlHeight = true;
        heroLayout.childForceExpandWidth = true;
        heroLayout.childForceExpandHeight = false;

        RectTransform heroTopRow = PackagePanelRuntimeUiKit.CreateRect("HeroTopRow", heroCard);
        AddLayoutElement(heroTopRow, minHeight: 118f, flexibleHeight: 1f);

        HorizontalLayoutGroup heroTopLayout = heroTopRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        heroTopLayout.padding = new RectOffset(0, 0, 0, 0);
        heroTopLayout.spacing = 12f;
        heroTopLayout.childAlignment = TextAnchor.UpperLeft;
        heroTopLayout.childControlWidth = true;
        heroTopLayout.childControlHeight = true;
        heroTopLayout.childForceExpandWidth = false;
        heroTopLayout.childForceExpandHeight = false;

        RectTransform identityRow = PackagePanelRuntimeUiKit.CreateRect("IdentityRow", heroTopRow);
        AddLayoutElement(identityRow, minWidth: 204f, flexibleWidth: 1f);

        HorizontalLayoutGroup identityRowLayout = identityRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        identityRowLayout.padding = new RectOffset(0, 0, 0, 0);
        identityRowLayout.spacing = 12f;
        identityRowLayout.childAlignment = TextAnchor.UpperLeft;
        identityRowLayout.childControlWidth = true;
        identityRowLayout.childControlHeight = true;
        identityRowLayout.childForceExpandWidth = false;
        identityRowLayout.childForceExpandHeight = false;

        RectTransform portraitFrame = PackagePanelRuntimeUiKit.CreatePanel("PortraitFrame", identityRow, new Color(0.93f, 0.84f, 0.70f, 0.94f), false, new Color(0.34f, 0.20f, 0.10f, 0.20f));
        _portraitFrameLayout = AddLayoutElement(portraitFrame, minWidth: 82f, preferredWidth: 82f, minHeight: 92f, preferredHeight: 92f);
        portraitFrame.gameObject.AddComponent<RectMask2D>();

        _portraitImage = PackagePanelRuntimeUiKit.AddImage(PackagePanelRuntimeUiKit.CreateRect("Portrait", portraitFrame).gameObject, Color.white);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)_portraitImage.transform, 6f, 6f, 6f, 6f);
        _portraitImage.preserveAspect = true;
        _portraitImage.raycastTarget = false;
        _portraitPlaceholderText = PackagePanelRuntimeUiKit.CreateText("PortraitPlaceholder", portraitFrame, "待补", 10.5f, MutedTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)_portraitPlaceholderText.transform, 6f, 6f, 6f, 6f);

        RectTransform identityColumn = PackagePanelRuntimeUiKit.CreateRect("IdentityColumn", identityRow);
        AddLayoutElement(identityColumn, minWidth: 164f, flexibleWidth: 1f);

        VerticalLayoutGroup identityLayout = identityColumn.gameObject.AddComponent<VerticalLayoutGroup>();
        identityLayout.padding = new RectOffset(0, 0, 0, 0);
        identityLayout.spacing = 7f;
        identityLayout.childAlignment = TextAnchor.UpperLeft;
        identityLayout.childControlWidth = true;
        identityLayout.childControlHeight = true;
        identityLayout.childForceExpandWidth = true;
        identityLayout.childForceExpandHeight = false;

        _nameText = PackagePanelRuntimeUiKit.CreateText("Name", identityColumn, string.Empty, 25.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)_nameText.transform, preferredHeight: 32f);

        _roleText = PackagePanelRuntimeUiKit.CreateText("Role", identityColumn, string.Empty, 15f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, true, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)_roleText.transform, minHeight: 26f, preferredHeight: 28f);

        RectTransform statusCard = PackagePanelRuntimeUiKit.CreatePanel("StatusCard", heroTopRow, new Color(1f, 1f, 1f, 0.24f), false, new Color(0.35f, 0.21f, 0.11f, 0.14f));
        AddLayoutElement(statusCard, minWidth: 174f, preferredWidth: 194f);

        VerticalLayoutGroup statusLayout = statusCard.gameObject.AddComponent<VerticalLayoutGroup>();
        statusLayout.padding = new RectOffset(12, 12, 12, 12);
        statusLayout.spacing = 8f;
        statusLayout.childAlignment = TextAnchor.UpperLeft;
        statusLayout.childControlWidth = true;
        statusLayout.childControlHeight = true;
        statusLayout.childForceExpandWidth = true;
        statusLayout.childForceExpandHeight = false;

        CreateMetaRow(statusCard, "PresenceRow", "今日出场", out _heroPresenceValueText);
        CreateMetaRow(statusCard, "BaselineRow", "常驻方式", out _heroBaselineValueText);
        CreateMetaRow(statusCard, "PhaseRow", "当前剧情", out _heroPhaseValueText);

        RectTransform quotePlate = PackagePanelRuntimeUiKit.CreatePanel("QuotePlate", heroCard, new Color(1f, 1f, 1f, 0.28f), false, new Color(0.35f, 0.21f, 0.11f, 0.12f));
        AddLayoutElement(quotePlate, minHeight: 80f);

        VerticalLayoutGroup quoteLayout = quotePlate.gameObject.AddComponent<VerticalLayoutGroup>();
        quoteLayout.padding = new RectOffset(12, 12, 10, 10);
        quoteLayout.spacing = 0f;
        quoteLayout.childAlignment = TextAnchor.UpperLeft;
        quoteLayout.childControlWidth = true;
        quoteLayout.childControlHeight = true;
        quoteLayout.childForceExpandWidth = true;
        quoteLayout.childForceExpandHeight = false;

        _quoteText = PackagePanelRuntimeUiKit.CreateText("QuoteText", quotePlate, string.Empty, 14f, InkTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _quoteText.lineSpacing = 5f;
        AddLayoutElement((RectTransform)_quoteText.transform, minHeight: 48f, flexibleHeight: 1f);
    }

    private void BuildStageCard(RectTransform parent)
    {
        RectTransform stageCard = PackagePanelRuntimeUiKit.CreatePanel("StageCard", parent, new Color(1f, 1f, 1f, 0.30f), false, new Color(0.35f, 0.21f, 0.11f, 0.14f));
        AddLayoutElement(stageCard, minHeight: 116f);

        VerticalLayoutGroup stageLayout = stageCard.gameObject.AddComponent<VerticalLayoutGroup>();
        stageLayout.padding = new RectOffset(14, 14, 14, 14);
        stageLayout.spacing = 9f;
        stageLayout.childAlignment = TextAnchor.UpperLeft;
        stageLayout.childControlWidth = true;
        stageLayout.childControlHeight = true;
        stageLayout.childForceExpandWidth = true;
        stageLayout.childForceExpandHeight = false;

        RectTransform stageHeader = PackagePanelRuntimeUiKit.CreateRect("StageHeader", stageCard);
        AddLayoutElement(stageHeader, preferredHeight: 30f);

        HorizontalLayoutGroup stageHeaderLayout = stageHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
        stageHeaderLayout.padding = new RectOffset(0, 0, 0, 0);
        stageHeaderLayout.spacing = 8f;
        stageHeaderLayout.childAlignment = TextAnchor.MiddleLeft;
        stageHeaderLayout.childControlWidth = true;
        stageHeaderLayout.childControlHeight = true;
        stageHeaderLayout.childForceExpandWidth = false;
        stageHeaderLayout.childForceExpandHeight = false;

        TextMeshProUGUI stageLabel = PackagePanelRuntimeUiKit.CreateText("StageLabel", stageHeader, "关系阶段", 14.5f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)stageLabel.transform, preferredWidth: 72f, flexibleWidth: 0f);

        RectTransform stageChip = PackagePanelRuntimeUiKit.CreatePanel("StageChip", stageHeader, new Color(0.82f, 0.66f, 0.34f, 0.96f), false, new Color(0.34f, 0.20f, 0.10f, 0.24f));
        AddLayoutElement(stageChip, minWidth: 96f, preferredWidth: 96f, preferredHeight: 24f);
        _stageChipImage = stageChip.GetComponent<Image>();

        HorizontalLayoutGroup chipLayout = stageChip.gameObject.AddComponent<HorizontalLayoutGroup>();
        chipLayout.padding = new RectOffset(8, 8, 4, 4);
        chipLayout.spacing = 0f;
        chipLayout.childAlignment = TextAnchor.MiddleCenter;
        chipLayout.childControlWidth = true;
        chipLayout.childControlHeight = true;
        chipLayout.childForceExpandWidth = true;
        chipLayout.childForceExpandHeight = true;

        _stageChipText = PackagePanelRuntimeUiKit.CreateText("StageChipText", stageChip, string.Empty, 14f, InkTint, FontStyles.Bold, TextAlignmentOptions.Center);
        AddLayoutElement((RectTransform)_stageChipText.transform, flexibleWidth: 1f, flexibleHeight: 1f);

        _stageHintText = PackagePanelRuntimeUiKit.CreateText("StageHint", stageCard, string.Empty, 13f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true, TextOverflowModes.Ellipsis);
        _stageHintText.lineSpacing = 3f;
        AddLayoutElement((RectTransform)_stageHintText.transform, minHeight: 34f, preferredHeight: 38f);

        RectTransform stageTrack = PackagePanelRuntimeUiKit.CreatePanel("StageTrack", stageCard, new Color(1f, 1f, 1f, 0.18f), false, new Color(0.35f, 0.21f, 0.11f, 0.10f));
        AddLayoutElement(stageTrack, preferredHeight: 32f);

        HorizontalLayoutGroup stageTrackLayout = stageTrack.gameObject.AddComponent<HorizontalLayoutGroup>();
        stageTrackLayout.padding = new RectOffset(6, 6, 6, 6);
        stageTrackLayout.spacing = 8f;
        stageTrackLayout.childAlignment = TextAnchor.MiddleCenter;
        stageTrackLayout.childControlWidth = true;
        stageTrackLayout.childControlHeight = true;
        stageTrackLayout.childForceExpandWidth = true;
        stageTrackLayout.childForceExpandHeight = true;

        for (int index = 0; index < _stageSegments.Length; index++)
        {
            RectTransform segment = PackagePanelRuntimeUiKit.CreatePanel("Segment_" + index, stageTrack, new Color(0.47f, 0.37f, 0.28f, 0.24f), false, new Color(0f, 0f, 0f, 0f));
            AddLayoutElement(segment, flexibleWidth: 1f, flexibleHeight: 1f);
            _stageSegments[index] = segment.GetComponent<Image>();
        }
    }

    private void BuildNarrativeRow(RectTransform parent)
    {
        RectTransform detailRow = PackagePanelRuntimeUiKit.CreateRect("DetailRow", parent);
        AddLayoutElement(detailRow, minHeight: 224f, flexibleHeight: 1f);

        VerticalLayoutGroup detailRowLayout = detailRow.gameObject.AddComponent<VerticalLayoutGroup>();
        detailRowLayout.padding = new RectOffset(0, 0, 0, 0);
        detailRowLayout.spacing = 12f;
        detailRowLayout.childAlignment = TextAnchor.UpperLeft;
        detailRowLayout.childControlWidth = true;
        detailRowLayout.childControlHeight = true;
        detailRowLayout.childForceExpandWidth = true;
        detailRowLayout.childForceExpandHeight = false;

        RectTransform roleCard = PackagePanelRuntimeUiKit.CreatePanel("RoleCard", detailRow, new Color(1f, 1f, 1f, 0.24f), false, new Color(0.35f, 0.21f, 0.11f, 0.12f));
        AddLayoutElement(roleCard, minHeight: 110f, flexibleHeight: 1f);
        VerticalLayoutGroup roleLayout = roleCard.gameObject.AddComponent<VerticalLayoutGroup>();
        roleLayout.padding = new RectOffset(16, 16, 14, 14);
        roleLayout.spacing = 10f;
        roleLayout.childAlignment = TextAnchor.UpperLeft;
        roleLayout.childControlWidth = true;
        roleLayout.childControlHeight = true;
        roleLayout.childForceExpandWidth = true;
        roleLayout.childForceExpandHeight = false;

        TextMeshProUGUI roleLabel = PackagePanelRuntimeUiKit.CreateText("RoleLabel", roleCard, "第一印象", 16f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)roleLabel.transform, preferredHeight: 22f);

        _roleSummaryText = PackagePanelRuntimeUiKit.CreateText("RoleSummary", roleCard, string.Empty, 14f, InkTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _roleSummaryText.lineSpacing = 5f;
        AddLayoutElement((RectTransform)_roleSummaryText.transform, minHeight: 64f, flexibleHeight: 1f);

        RectTransform presenceCard = PackagePanelRuntimeUiKit.CreatePanel("PresenceCard", detailRow, new Color(1f, 1f, 1f, 0.22f), false, new Color(0.35f, 0.21f, 0.11f, 0.12f));
        AddLayoutElement(presenceCard, minHeight: 104f, flexibleHeight: 1f);
        VerticalLayoutGroup presenceLayout = presenceCard.gameObject.AddComponent<VerticalLayoutGroup>();
        presenceLayout.padding = new RectOffset(16, 16, 14, 14);
        presenceLayout.spacing = 10f;
        presenceLayout.childAlignment = TextAnchor.UpperLeft;
        presenceLayout.childControlWidth = true;
        presenceLayout.childControlHeight = true;
        presenceLayout.childForceExpandWidth = true;
        presenceLayout.childForceExpandHeight = false;

        TextMeshProUGUI presenceLabel = PackagePanelRuntimeUiKit.CreateText("PresenceLabel", presenceCard, "今天会怎么遇见", 16f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)presenceLabel.transform, preferredHeight: 22f);

        _presenceText = PackagePanelRuntimeUiKit.CreateText("PresenceText", presenceCard, string.Empty, 13.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _presenceText.lineSpacing = 4f;
        AddLayoutElement((RectTransform)_presenceText.transform, minHeight: 68f, flexibleHeight: 1f);
    }

    private void BuildFooterCard(RectTransform parent)
    {
        RectTransform footerCard = PackagePanelRuntimeUiKit.CreatePanel("FooterCard", parent, new Color(1f, 1f, 1f, 0.18f), false, new Color(0.35f, 0.21f, 0.11f, 0.10f));
        AddLayoutElement(footerCard, minHeight: 72f);

        VerticalLayoutGroup footerLayout = footerCard.gameObject.AddComponent<VerticalLayoutGroup>();
        footerLayout.padding = new RectOffset(14, 14, 11, 11);
        footerLayout.spacing = 3f;
        footerLayout.childAlignment = TextAnchor.MiddleLeft;
        footerLayout.childControlWidth = true;
        footerLayout.childControlHeight = true;
        footerLayout.childForceExpandWidth = true;
        footerLayout.childForceExpandHeight = false;

        _footerText = PackagePanelRuntimeUiKit.CreateText("FooterText", footerCard, string.Empty, 12.5f, MutedTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true, TextOverflowModes.Ellipsis);
        _footerText.lineSpacing = 3f;
        AddLayoutElement((RectTransform)_footerText.transform, minHeight: 38f, flexibleHeight: 1f);
    }

    private void RefreshView()
    {
        if (!HasRequiredRuntimeBindings())
        {
            EnsureBuilt();
            if (!HasRequiredRuntimeBindings())
            {
                return;
            }
        }

        StoryPhase currentPhase = ResolveCurrentPhase();

        _entries.Clear();
        BuildEntries();
        SortEntries(currentPhase);
        BuildListCards();
        RefreshSummary(currentPhase);

        if (_entries.Count == 0)
        {
            ApplyEmptyState();
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedNpcId) || _entries.FindIndex(entry => entry.NpcId == _selectedNpcId) < 0)
        {
            _selectedNpcId = ResolvePreferredSelectionId(currentPhase);
        }

        ApplySelection(_selectedNpcId);
    }

    private void BuildEntries()
    {
        NpcCharacterRegistry registry = NpcCharacterRegistry.LoadRuntime();
        SpringDay1NpcCrowdManifest manifest = Resources.Load<SpringDay1NpcCrowdManifest>("Story/SpringDay1/SpringDay1NpcCrowdManifest");
        if (registry == null && manifest == null)
        {
            return;
        }

        StoryPhase currentPhase = ResolveCurrentPhase();
        IReadOnlyDictionary<string, NPCRelationshipStage> snapshot = PlayerNpcRelationshipService.GetSnapshot();
        string beatKey = ResolveBeatForPhase(currentPhase);
        Dictionary<string, SpringDay1NpcCrowdManifest.Entry> manifestByNpcId = BuildManifestMap(manifest);
        HashSet<string> registryNpcIds = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        if (registry != null)
        {
            NpcCharacterRegistry.Entry[] registryEntries = registry.Entries;
            for (int index = 0; index < registryEntries.Length; index++)
            {
                NpcCharacterRegistry.Entry registryEntry = registryEntries[index];
                if (registryEntry == null
                    || !registryEntry.showInRelationshipPanel
                    || string.IsNullOrWhiteSpace(registryEntry.npcId))
                {
                    continue;
                }

                string npcId = registryEntry.npcId.Trim();
                registryNpcIds.Add(npcId);
                manifestByNpcId.TryGetValue(npcId, out SpringDay1NpcCrowdManifest.Entry manifestEntry);
                _entries.Add(BuildRegistryBackedEntry(registryEntry, manifestEntry, snapshot, currentPhase, beatKey));
            }
        }

        if (manifest == null)
        {
            return;
        }

        SpringDay1NpcCrowdManifest.Entry[] manifestEntries = manifest.Entries;
        for (int index = 0; index < manifestEntries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = manifestEntries[index];
            if (entry == null
                || string.IsNullOrWhiteSpace(entry.npcId)
                || registryNpcIds.Contains(entry.npcId.Trim()))
            {
                continue;
            }

            snapshot.TryGetValue(entry.npcId, out NPCRelationshipStage stage);
            stage = NPCRelationshipStageUtility.Sanitize(stage);

            SpringDay1CrowdResidentPresenceLevel presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
            string presenceNote = "这位村民当前还没有在这一段剧情里正面露出位置。";
            if (entry.CanAppear(currentPhase) && entry.TryGetResidentBeatSemantic(beatKey, out SpringDay1NpcCrowdManifest.ResidentBeatSemantic semantic))
            {
                presenceLevel = semantic.presenceLevel;
                if (!string.IsNullOrWhiteSpace(semantic.note))
                {
                    presenceNote = semantic.note.Trim();
                }
            }
            else if (!string.IsNullOrWhiteSpace(entry.roleSummary))
            {
                presenceNote = entry.roleSummary.Trim();
            }

            _entries.Add(new NpcEntryViewModel
            {
                NpcId = entry.npcId,
                DisplayName = entry.displayName,
                RoleSummary = entry.roleSummary,
                Preview = BuildPreview(entry.roleSummary),
                Stage = stage,
                PresenceLevel = presenceLevel,
                PresenceNote = presenceNote,
                Baseline = entry.residentBaseline,
                Portrait = ResolvePortrait(entry.prefab)
            });
        }
    }

    private static Dictionary<string, SpringDay1NpcCrowdManifest.Entry> BuildManifestMap(SpringDay1NpcCrowdManifest manifest)
    {
        Dictionary<string, SpringDay1NpcCrowdManifest.Entry> result = new Dictionary<string, SpringDay1NpcCrowdManifest.Entry>(System.StringComparer.OrdinalIgnoreCase);
        if (manifest == null)
        {
            return result;
        }

        SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries;
        for (int index = 0; index < entries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = entries[index];
            if (entry == null || string.IsNullOrWhiteSpace(entry.npcId))
            {
                continue;
            }

            result[entry.npcId.Trim()] = entry;
        }

        return result;
    }

    private static NpcEntryViewModel BuildRegistryBackedEntry(
        NpcCharacterRegistry.Entry registryEntry,
        SpringDay1NpcCrowdManifest.Entry manifestEntry,
        IReadOnlyDictionary<string, NPCRelationshipStage> snapshot,
        StoryPhase currentPhase,
        string beatKey)
    {
        snapshot.TryGetValue(registryEntry.npcId, out NPCRelationshipStage stage);
        stage = NPCRelationshipStageUtility.Sanitize(stage);

        SpringDay1CrowdResidentPresenceLevel presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
        string presenceNote = "这位村民当前还没有在这一段剧情里正面露出位置。";
        SpringDay1CrowdResidentBaseline baseline = registryEntry.relationshipBaseline;

        if (manifestEntry != null)
        {
            baseline = manifestEntry.residentBaseline;
            if (manifestEntry.CanAppear(currentPhase)
                && manifestEntry.TryGetResidentBeatSemantic(beatKey, out SpringDay1NpcCrowdManifest.ResidentBeatSemantic manifestSemantic)
                && manifestSemantic != null)
            {
                presenceLevel = manifestSemantic.presenceLevel;
                if (!string.IsNullOrWhiteSpace(manifestSemantic.note))
                {
                    presenceNote = manifestSemantic.note.Trim();
                }
            }
            else if (!string.IsNullOrWhiteSpace(manifestEntry.roleSummary))
            {
                presenceNote = manifestEntry.roleSummary.Trim();
            }
        }
        else if (registryEntry.TryGetRelationshipBeatSemantic(beatKey, out NpcCharacterRegistry.RelationshipBeatSemantic registrySemantic) && registrySemantic != null)
        {
            presenceLevel = registrySemantic.presenceLevel;
            if (!string.IsNullOrWhiteSpace(registrySemantic.note))
            {
                presenceNote = registrySemantic.note.Trim();
            }
        }
        else if (!string.IsNullOrWhiteSpace(registryEntry.roleSummary))
        {
            presenceNote = registryEntry.roleSummary.Trim();
        }

        string displayName = registryEntry.RelationshipDisplayNameOrCanonical;
        if (string.IsNullOrWhiteSpace(displayName) && manifestEntry != null)
        {
            displayName = manifestEntry.displayName;
        }

        string roleSummary = !string.IsNullOrWhiteSpace(registryEntry.roleSummary)
            ? registryEntry.roleSummary
            : manifestEntry != null ? manifestEntry.roleSummary : string.Empty;

        Sprite portrait = registryEntry.ResolveRelationshipPortrait();
        if (portrait == null && manifestEntry != null)
        {
            portrait = ResolvePortrait(manifestEntry.prefab);
        }

        return new NpcEntryViewModel
        {
            NpcId = registryEntry.npcId,
            DisplayName = displayName,
            RoleSummary = roleSummary,
            Preview = BuildPreview(roleSummary),
            Stage = stage,
            PresenceLevel = presenceLevel,
            PresenceNote = presenceNote,
            Baseline = baseline,
            Portrait = portrait
        };
    }

    private void BuildListCards()
    {
        PackagePanelRuntimeUiKit.DestroyChildren(_listContent);
        _cardRefs.Clear();

        for (int index = 0; index < _entries.Count; index++)
        {
            NpcEntryViewModel entry = _entries[index];
            RectTransform card = PackagePanelRuntimeUiKit.CreatePanel("Entry_" + entry.NpcId, _listContent, DefaultCardTint, true, new Color(0.35f, 0.21f, 0.11f, 0.16f));
            AddLayoutElement(card, minHeight: 102f, preferredHeight: 102f, flexibleWidth: 1f);

            HorizontalLayoutGroup cardLayout = card.gameObject.AddComponent<HorizontalLayoutGroup>();
            cardLayout.padding = new RectOffset(0, 14, 11, 11);
            cardLayout.spacing = 10f;
            cardLayout.childAlignment = TextAnchor.MiddleLeft;
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = false;
            cardLayout.childForceExpandHeight = true;

            Image background = card.GetComponent<Image>();
            Button button = PackagePanelRuntimeUiKit.AddButton(card.gameObject, background);
            string npcId = entry.NpcId;
            button.onClick.AddListener(() => ApplySelection(npcId));

            RectTransform accent = PackagePanelRuntimeUiKit.CreatePanel("Accent", card, DefaultAccentTint, false, new Color(0f, 0f, 0f, 0f));
            AddLayoutElement(accent, minWidth: 8f, preferredWidth: 8f, flexibleHeight: 1f);

            RectTransform portraitFrame = PackagePanelRuntimeUiKit.CreatePanel("PortraitFrame", card, new Color(0.92f, 0.83f, 0.69f, 0.98f), false, new Color(0.34f, 0.20f, 0.10f, 0.24f));
            AddLayoutElement(portraitFrame, minWidth: 50f, preferredWidth: 50f, minHeight: 50f, preferredHeight: 50f);
            portraitFrame.gameObject.AddComponent<RectMask2D>();

            Image portrait = PackagePanelRuntimeUiKit.AddImage(PackagePanelRuntimeUiKit.CreateRect("Portrait", portraitFrame).gameObject, Color.white);
            PackagePanelRuntimeUiKit.Stretch((RectTransform)portrait.transform, 4f, 4f, 4f, 4f);
            portrait.preserveAspect = true;
            portrait.sprite = entry.Portrait;

            RectTransform textColumn = PackagePanelRuntimeUiKit.CreateRect("TextColumn", card);
            AddLayoutElement(textColumn, minWidth: 108f, flexibleWidth: 1f);

            VerticalLayoutGroup textColumnLayout = textColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            textColumnLayout.padding = new RectOffset(0, 0, 0, 0);
            textColumnLayout.spacing = 5f;
            textColumnLayout.childAlignment = TextAnchor.MiddleLeft;
            textColumnLayout.childControlWidth = true;
            textColumnLayout.childControlHeight = true;
            textColumnLayout.childForceExpandWidth = true;
            textColumnLayout.childForceExpandHeight = false;

            TextMeshProUGUI name = PackagePanelRuntimeUiKit.CreateText("Name", textColumn, entry.DisplayName, 17f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
            AddLayoutElement((RectTransform)name.transform, preferredHeight: 23f);

            TextMeshProUGUI preview = PackagePanelRuntimeUiKit.CreateText("Preview", textColumn, BuildListPreview(entry), 12.5f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
            preview.lineSpacing = 2f;
            AddLayoutElement((RectTransform)preview.transform, minHeight: 36f, preferredHeight: 40f, flexibleHeight: 1f);

            RectTransform chipColumn = PackagePanelRuntimeUiKit.CreateRect("ChipColumn", card);
            AddLayoutElement(chipColumn, minWidth: ListChipColumnWidth, preferredWidth: ListChipColumnWidth, flexibleHeight: 1f);

            VerticalLayoutGroup chipColumnLayout = chipColumn.gameObject.AddComponent<VerticalLayoutGroup>();
            chipColumnLayout.padding = new RectOffset(0, 0, 8, 4);
            chipColumnLayout.spacing = 7f;
            chipColumnLayout.childAlignment = TextAnchor.UpperRight;
            chipColumnLayout.childControlWidth = true;
            chipColumnLayout.childControlHeight = true;
            chipColumnLayout.childForceExpandWidth = false;
            chipColumnLayout.childForceExpandHeight = false;

            RectTransform presenceChip = PackagePanelRuntimeUiKit.CreatePanel("PresenceChip", chipColumn, GetPresenceChipColor(entry.PresenceLevel), false, new Color(0f, 0f, 0f, 0f));
            AddLayoutElement(presenceChip, minWidth: ListPresenceChipWidth, preferredWidth: ListPresenceChipWidth, preferredHeight: ListChipHeight);

            HorizontalLayoutGroup presenceChipLayout = presenceChip.gameObject.AddComponent<HorizontalLayoutGroup>();
            presenceChipLayout.padding = new RectOffset(5, 5, 2, 2);
            presenceChipLayout.spacing = 0f;
            presenceChipLayout.childAlignment = TextAnchor.MiddleCenter;
            presenceChipLayout.childControlWidth = true;
            presenceChipLayout.childControlHeight = true;
            presenceChipLayout.childForceExpandWidth = true;
            presenceChipLayout.childForceExpandHeight = true;

            TextMeshProUGUI presenceChipText = PackagePanelRuntimeUiKit.CreateText("PresenceChipText", presenceChip, PresenceLabel(entry.PresenceLevel), ListChipTextFontSize, InkTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
            AddLayoutElement((RectTransform)presenceChipText.transform, flexibleWidth: 1f, flexibleHeight: 1f);

            RectTransform stageChip = PackagePanelRuntimeUiKit.CreatePanel("StageChip", chipColumn, GetStageChipColor(entry.Stage), false, new Color(0f, 0f, 0f, 0f));
            AddLayoutElement(stageChip, minWidth: ListStageChipWidth, preferredWidth: ListStageChipWidth, preferredHeight: ListChipHeight);

            HorizontalLayoutGroup stageChipLayout = stageChip.gameObject.AddComponent<HorizontalLayoutGroup>();
            stageChipLayout.padding = new RectOffset(5, 5, 2, 2);
            stageChipLayout.spacing = 0f;
            stageChipLayout.childAlignment = TextAnchor.MiddleCenter;
            stageChipLayout.childControlWidth = true;
            stageChipLayout.childControlHeight = true;
            stageChipLayout.childForceExpandWidth = true;
            stageChipLayout.childForceExpandHeight = true;

            TextMeshProUGUI stageText = PackagePanelRuntimeUiKit.CreateText("StageText", stageChip, StageLabel(entry.Stage), ListChipTextFontSize, InkTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
            AddLayoutElement((RectTransform)stageText.transform, flexibleWidth: 1f, flexibleHeight: 1f);

            _cardRefs.Add(new NpcCardRefs
            {
                NpcId = entry.NpcId,
                Button = button,
                Accent = accent.GetComponent<Image>(),
                Background = background,
                Name = name,
                Preview = preview
            });
        }
    }

    private void RefreshSummary(StoryPhase phase)
    {
        int acquaintedCount = 0;
        int visibleTodayCount = 0;
        for (int index = 0; index < _entries.Count; index++)
        {
            if (_entries[index].Stage >= NPCRelationshipStage.Acquainted)
            {
                acquaintedCount++;
            }

            if (_entries[index].PresenceLevel != SpringDay1CrowdResidentPresenceLevel.None)
            {
                visibleTodayCount++;
            }
        }

        _listPhaseChipText.text = PhaseLabel(phase);
        _listPhaseChipImage.color = GetPhaseChipColor(phase);
        _summaryText.text = $"这段剧情里会正面撞见 {visibleTodayCount} 人，真正已经认熟 {acquaintedCount} 人。先看当前更容易遇见、也更会影响路线的人。";
    }

    private void ApplySelection(string npcId)
    {
        if (!HasRequiredRuntimeBindings())
        {
            return;
        }

        _selectedNpcId = npcId;
        NpcEntryViewModel selected = _entries.Find(entry => entry.NpcId == npcId);
        if (selected == null)
        {
            return;
        }

        for (int index = 0; index < _cardRefs.Count; index++)
        {
            NpcCardRefs refs = _cardRefs[index];
            bool isSelected = string.Equals(refs.NpcId, npcId, System.StringComparison.Ordinal);
            refs.Background.color = isSelected ? SelectedCardTint : DefaultCardTint;
            if (refs.Accent != null) refs.Accent.color = isSelected ? SelectedAccentTint : DefaultAccentTint;
            refs.Name.color = InkTint;
            refs.Preview.color = isSelected ? InkTint : SubtleTint;
        }

        StoryPhase currentPhase = ResolveCurrentPhase();
        string roleSummary = NormalizeText(selected.RoleSummary, "这位村民的身份描述还在补录。");
        string presenceNarrative = BuildPresenceNarrative(selected, roleSummary);
        string stageHint = BuildStageHint(selected.Stage, selected.PresenceLevel);

        bool hasPortrait = selected.Portrait != null;
        _portraitImage.sprite = selected.Portrait;
        _portraitImage.enabled = hasPortrait;
        if (_portraitPlaceholderText != null)
        {
            _portraitPlaceholderText.gameObject.SetActive(!hasPortrait);
        }

        if (_portraitFrameLayout != null)
        {
            _portraitFrameLayout.minWidth = hasPortrait ? 82f : 48f;
            _portraitFrameLayout.preferredWidth = hasPortrait ? 82f : 48f;
            _portraitFrameLayout.minHeight = hasPortrait ? 92f : 54f;
            _portraitFrameLayout.preferredHeight = hasPortrait ? 92f : 54f;
        }
        _nameText.text = selected.DisplayName;
        _roleText.text = $"{BaselineLabel(selected.Baseline)} · 这段里更像“{PresenceLabel(selected.PresenceLevel)}”";
        _quoteText.text = $"“{presenceNarrative}”";
        _heroPresenceValueText.text = PresenceLabel(selected.PresenceLevel);
        _heroBaselineValueText.text = BaselineLabel(selected.Baseline);
        _heroPhaseValueText.text = PhaseLabel(currentPhase);
        _stageChipText.text = StageLabel(selected.Stage);
        _stageChipImage.color = GetStageChipColor(selected.Stage);
        _stageHintText.text = stageHint;
        _roleSummaryText.text = roleSummary;
        _presenceText.text = BuildStructuredFacts(selected, currentPhase, presenceNarrative, roleSummary);
        _footerText.text = BuildFooterLine(selected, currentPhase);

        int filled = (int)NPCRelationshipStageUtility.Sanitize(selected.Stage) + 1;
        for (int index = 0; index < _stageSegments.Length; index++)
        {
            if (_stageSegments[index] == null)
            {
                continue;
            }

            _stageSegments[index].color = index < filled ? GetStageChipColor(selected.Stage) : new Color(0.46f, 0.36f, 0.29f, 0.22f);
        }
    }

    private void ApplyEmptyState()
    {
        if (!HasRequiredRuntimeBindings())
        {
            return;
        }

        _listPhaseChipText.text = "暂无";
        _listPhaseChipImage.color = GetPhaseChipColor(StoryPhase.None);
        _summaryText.text = "还没找到可消费的村民关系数据。";
        _nameText.text = "暂无记录";
        _roleText.text = "等村民资源链恢复后，这里会显示正式内容。";
        _quoteText.text = "“关系册还没拿到可展示的村民档案。”";
        _heroPresenceValueText.text = "未录";
        _heroBaselineValueText.text = "未录";
        _heroPhaseValueText.text = PhaseLabel(ResolveCurrentPhase());
        _stageChipText.text = "未录";
        _stageChipImage.color = GetStageChipColor(NPCRelationshipStage.Stranger);
        _stageHintText.text = "当前还没有关系阶段可显示。";
        _roleSummaryText.text = "目前没有可消费的村民条目。";
        _presenceText.text = "阶段：未录\n今日出场：未录\n常驻方式：未录\n当前剧情：" + PhaseLabel(ResolveCurrentPhase());
        _footerText.text = "关系册会在村民数据链恢复后重新生成。";
        _portraitImage.enabled = false;
        _portraitImage.sprite = null;
        if (_portraitPlaceholderText != null)
        {
            _portraitPlaceholderText.gameObject.SetActive(true);
        }

        if (_portraitFrameLayout != null)
        {
            _portraitFrameLayout.minWidth = 48f;
            _portraitFrameLayout.preferredWidth = 48f;
            _portraitFrameLayout.minHeight = 54f;
            _portraitFrameLayout.preferredHeight = 54f;
        }

        for (int index = 0; index < _stageSegments.Length; index++)
        {
            if (_stageSegments[index] != null)
            {
                _stageSegments[index].color = new Color(0.46f, 0.36f, 0.29f, 0.22f);
            }
        }
    }

    private static Sprite ResolvePortrait(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        SpriteRenderer[] renderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
        Sprite bestSprite = null;
        float bestArea = -1f;
        for (int index = 0; index < renderers.Length; index++)
        {
            SpriteRenderer renderer = renderers[index];
            if (renderer == null || renderer.sprite == null)
            {
                continue;
            }

            Rect rect = renderer.sprite.rect;
            float area = rect.width * rect.height;
            if (area > bestArea)
            {
                bestArea = area;
                bestSprite = renderer.sprite;
            }
        }

        return bestSprite;
    }

    private static RectTransform FindPageMain(Transform panelRoot, string pageName)
    {
        if (panelRoot == null)
        {
            return null;
        }

        Transform[] children = panelRoot.GetComponentsInChildren<Transform>(true);
        for (int index = 0; index < children.Length; index++)
        {
            Transform child = children[index];
            if (string.Equals(child.name, pageName, System.StringComparison.Ordinal))
            {
                return child.Find("Main") as RectTransform;
            }
        }

        return null;
    }

    private static StoryPhase ResolveCurrentPhase()
    {
        StoryManager storyManager = StoryManager.Instance;
        return storyManager != null && storyManager.CurrentPhase != StoryPhase.None
            ? storyManager.CurrentPhase
            : StoryPhase.CrashAndMeet;
    }

    private static string BuildPreview(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "村里对这位居民的细节还在补录。";
        }

        string trimmed = text.Trim();
        return trimmed.Length <= 14 ? trimmed : trimmed.Substring(0, 14) + "…";
    }

    private static string BuildListPreview(NpcEntryViewModel entry)
    {
        string lead = entry.PresenceLevel == SpringDay1CrowdResidentPresenceLevel.None
            ? "这段剧情里还没正面露出。"
            : NormalizeText(entry.PresenceNote, BuildPresenceFallback(entry.Stage, entry.PresenceLevel, entry.Baseline));

        if (!string.IsNullOrWhiteSpace(entry.RoleSummary) && !AreSemanticallySame(lead, entry.RoleSummary))
        {
            return $"{lead}\n{BuildPreview(entry.RoleSummary)}";
        }

        return lead;
    }

    private static string BuildStageHint(NPCRelationshipStage stage, SpringDay1CrowdResidentPresenceLevel presenceLevel)
    {
        return stage switch
        {
            NPCRelationshipStage.Stranger => $"还停在先看见的阶段，今天主要以“{PresenceLabel(presenceLevel)}”方式出现。",
            NPCRelationshipStage.Acquainted => $"已经能把这个人和村里的一处位置对上，今天偏“{PresenceLabel(presenceLevel)}”。",
            NPCRelationshipStage.Familiar => $"开始会主动留意他的日常，今天的存在感更像“{PresenceLabel(presenceLevel)}”。",
            NPCRelationshipStage.Close => $"已经会影响你的路线判断，这一页记的是他今天“{PresenceLabel(presenceLevel)}”的一面。",
            _ => $"这一阶段更像“{PresenceLabel(presenceLevel)}”。"
        };
    }

    private static string StageLabel(NPCRelationshipStage stage)
    {
        return stage switch
        {
            NPCRelationshipStage.Acquainted => "初识",
            NPCRelationshipStage.Familiar => "熟悉",
            NPCRelationshipStage.Close => "亲近",
            _ => "陌生"
        };
    }

    private static string PresenceLabel(SpringDay1CrowdResidentPresenceLevel presenceLevel)
    {
        return presenceLevel switch
        {
            SpringDay1CrowdResidentPresenceLevel.Trace => "余波",
            SpringDay1CrowdResidentPresenceLevel.Background => "路过",
            SpringDay1CrowdResidentPresenceLevel.Visible => "正面",
            SpringDay1CrowdResidentPresenceLevel.Pressure => "压场",
            _ => "未露面"
        };
    }

    private static string BaselineLabel(SpringDay1CrowdResidentBaseline baseline)
    {
        return baseline switch
        {
            SpringDay1CrowdResidentBaseline.DaytimeResident => "白天常在",
            SpringDay1CrowdResidentBaseline.NightResident => "夜里更常见",
            _ => "偏在边路"
        };
    }

    private static Color GetStageChipColor(NPCRelationshipStage stage)
    {
        return stage switch
        {
            NPCRelationshipStage.Acquainted => new Color(0.69f, 0.75f, 0.90f, 0.94f),
            NPCRelationshipStage.Familiar => new Color(0.66f, 0.79f, 0.61f, 0.94f),
            NPCRelationshipStage.Close => new Color(0.90f, 0.74f, 0.28f, 0.96f),
            _ => new Color(0.79f, 0.71f, 0.63f, 0.86f)
        };
    }

    private static Color GetPresenceChipColor(SpringDay1CrowdResidentPresenceLevel presenceLevel)
    {
        return presenceLevel switch
        {
            SpringDay1CrowdResidentPresenceLevel.Trace => new Color(0.78f, 0.71f, 0.64f, 0.92f),
            SpringDay1CrowdResidentPresenceLevel.Background => new Color(0.75f, 0.70f, 0.58f, 0.94f),
            SpringDay1CrowdResidentPresenceLevel.Visible => new Color(0.70f, 0.82f, 0.66f, 0.94f),
            SpringDay1CrowdResidentPresenceLevel.Pressure => new Color(0.90f, 0.69f, 0.34f, 0.96f),
            _ => new Color(0.84f, 0.77f, 0.70f, 0.88f)
        };
    }

    private static Color GetPhaseChipColor(StoryPhase phase)
    {
        return phase switch
        {
            StoryPhase.CrashAndMeet => new Color(0.86f, 0.55f, 0.27f, 0.96f),
            StoryPhase.EnterVillage => new Color(0.84f, 0.63f, 0.28f, 0.96f),
            StoryPhase.HealingAndHP => new Color(0.76f, 0.62f, 0.32f, 0.96f),
            StoryPhase.WorkbenchFlashback => new Color(0.78f, 0.56f, 0.24f, 0.96f),
            StoryPhase.FarmingTutorial => new Color(0.69f, 0.67f, 0.27f, 0.96f),
            StoryPhase.DinnerConflict => new Color(0.90f, 0.51f, 0.24f, 0.96f),
            StoryPhase.ReturnAndReminder => new Color(0.72f, 0.57f, 0.32f, 0.96f),
            StoryPhase.FreeTime => new Color(0.67f, 0.58f, 0.38f, 0.96f),
            StoryPhase.DayEnd => new Color(0.60f, 0.51f, 0.40f, 0.96f),
            _ => new Color(0.72f, 0.63f, 0.55f, 0.92f)
        };
    }

    private static string ResolveBeatForPhase(StoryPhase phase)
    {
        return phase switch
        {
            StoryPhase.DinnerConflict => SpringDay1CrowdResidentBeatKeys.DinnerConflictTable,
            StoryPhase.ReturnAndReminder => SpringDay1CrowdResidentBeatKeys.ReturnAndReminderWalkBack,
            StoryPhase.FreeTime => SpringDay1CrowdResidentBeatKeys.FreeTimeNightWitness,
            StoryPhase.DayEnd => SpringDay1CrowdResidentBeatKeys.DayEndSettle,
            _ => SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry
        };
    }

    private static string PhaseLabel(StoryPhase phase)
    {
        return phase switch
        {
            StoryPhase.CrashAndMeet => "坠落与相遇",
            StoryPhase.EnterVillage => "进入村口",
            StoryPhase.HealingAndHP => "疗伤教学",
            StoryPhase.WorkbenchFlashback => "工作台回想",
            StoryPhase.FarmingTutorial => "农田教学",
            StoryPhase.DinnerConflict => "晚餐冲突",
            StoryPhase.ReturnAndReminder => "返程提醒",
            StoryPhase.FreeTime => "自由行动",
            StoryPhase.DayEnd => "第一夜收束",
            _ => "春日村"
        };
    }

    private void SortEntries(StoryPhase phase)
    {
        _entries.Sort((left, right) =>
        {
            int scoreCompare = GetEntryPriority(right, phase).CompareTo(GetEntryPriority(left, phase));
            if (scoreCompare != 0)
            {
                return scoreCompare;
            }

            int stageCompare = ((int)right.Stage).CompareTo((int)left.Stage);
            if (stageCompare != 0)
            {
                return stageCompare;
            }

            return string.Compare(left.DisplayName, right.DisplayName, System.StringComparison.Ordinal);
        });
    }

    private string ResolvePreferredSelectionId(StoryPhase phase)
    {
        if (_entries.Count == 0)
        {
            return string.Empty;
        }

        NpcEntryViewModel best = _entries[0];
        int bestScore = GetEntryPriority(best, phase);
        for (int index = 1; index < _entries.Count; index++)
        {
            NpcEntryViewModel candidate = _entries[index];
            int candidateScore = GetEntryPriority(candidate, phase);
            if (candidateScore <= bestScore)
            {
                continue;
            }

            best = candidate;
            bestScore = candidateScore;
        }

        return best.NpcId;
    }

    private static int GetEntryPriority(NpcEntryViewModel entry, StoryPhase phase)
    {
        int score = ((int)entry.PresenceLevel * 100) + ((int)entry.Stage * 24);
        if (entry.PresenceLevel == SpringDay1CrowdResidentPresenceLevel.Pressure)
        {
            score += 30;
        }
        else if (entry.PresenceLevel == SpringDay1CrowdResidentPresenceLevel.Visible)
        {
            score += 18;
        }

        if (entry.PresenceLevel == SpringDay1CrowdResidentPresenceLevel.None)
        {
            score -= 40;
        }

        if (phase >= StoryPhase.DinnerConflict && entry.Baseline == SpringDay1CrowdResidentBaseline.NightResident)
        {
            score += 8;
        }

        return score;
    }

    private static LayoutElement AddLayoutElement(
        RectTransform rect,
        float preferredWidth = -1f,
        float preferredHeight = -1f,
        float minWidth = -1f,
        float minHeight = -1f,
        float flexibleWidth = -1f,
        float flexibleHeight = -1f)
    {
        LayoutElement layout = rect.gameObject.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = rect.gameObject.AddComponent<LayoutElement>();
        }

        if (preferredWidth >= 0f) layout.preferredWidth = preferredWidth;
        if (preferredHeight >= 0f) layout.preferredHeight = preferredHeight;
        if (minWidth >= 0f) layout.minWidth = minWidth;
        if (minHeight >= 0f) layout.minHeight = minHeight;
        if (flexibleWidth >= 0f) layout.flexibleWidth = flexibleWidth;
        if (flexibleHeight >= 0f) layout.flexibleHeight = flexibleHeight;
        return layout;
    }

    private static RectTransform CreateMetaRow(Transform parent, string name, string label, out TextMeshProUGUI valueText)
    {
        RectTransform row = PackagePanelRuntimeUiKit.CreatePanel(name, parent, new Color(1f, 1f, 1f, 0.28f), false, new Color(0.35f, 0.21f, 0.11f, 0.10f));
        AddLayoutElement(row, minHeight: 44f);

        VerticalLayoutGroup rowLayout = row.gameObject.AddComponent<VerticalLayoutGroup>();
        rowLayout.padding = new RectOffset(11, 11, 7, 8);
        rowLayout.spacing = 2f;
        rowLayout.childAlignment = TextAnchor.UpperLeft;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = true;
        rowLayout.childForceExpandHeight = false;

        TextMeshProUGUI labelText = PackagePanelRuntimeUiKit.CreateText(name + "Label", row, label, 11.5f, MutedTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)labelText.transform, preferredHeight: 14f);

        valueText = PackagePanelRuntimeUiKit.CreateText(name + "Value", row, string.Empty, 13.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)valueText.transform, preferredHeight: 17f);
        return row;
    }

    private static string NormalizeText(string value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string BuildPresenceNarrative(NpcEntryViewModel entry, string roleSummary)
    {
        string rawPresence = NormalizeText(entry.PresenceNote, BuildPresenceFallback(entry.Stage, entry.PresenceLevel, entry.Baseline));
        return AreSemanticallySame(rawPresence, roleSummary)
            ? BuildPresenceFallback(entry.Stage, entry.PresenceLevel, entry.Baseline)
            : rawPresence;
    }

    private static string BuildPresenceFallback(
        NPCRelationshipStage stage,
        SpringDay1CrowdResidentPresenceLevel presenceLevel,
        SpringDay1CrowdResidentBaseline baseline)
    {
        return $"{StageLabel(stage)}阶段里，他更像一个“{PresenceLabel(presenceLevel)}”的在场点，平时属于{BaselineLabel(baseline)}。";
    }

    private static string BuildStructuredFacts(
        NpcEntryViewModel entry,
        StoryPhase phase,
        string presenceNarrative,
        string roleSummary)
    {
        StringBuilder builder = new StringBuilder(160);
        builder.Append("这段剧情里：").Append(PresenceLabel(entry.PresenceLevel));
        builder.Append('\n').Append("平常在场：").Append(BaselineLabel(entry.Baseline));
        builder.Append('\n').Append("当前阶段：").Append(PhaseLabel(phase));
        builder.Append("\n\n");
        builder.Append(AreSemanticallySame(presenceNarrative, roleSummary) ? "这页先记住：" : "今天更该记住的是：");
        builder.Append('\n').Append(presenceNarrative);
        return builder.ToString();
    }

    private static string BuildFooterLine(NpcEntryViewModel entry, StoryPhase phase)
    {
        return $"关系册按当前阶段排序，只记今天真的会撞见的样子 · {StageLabel(entry.Stage)} · {PhaseLabel(phase)}";
    }

    private static bool AreSemanticallySame(string left, string right)
    {
        string normalizedLeft = NormalizeComparable(left);
        string normalizedRight = NormalizeComparable(right);
        return normalizedLeft.Length > 0 && normalizedLeft == normalizedRight;
    }

    private static string NormalizeComparable(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(value.Length);
        for (int index = 0; index < value.Length; index++)
        {
            char current = value[index];
            if (char.IsLetterOrDigit(current) || current > 127)
            {
                builder.Append(char.ToLowerInvariant(current));
            }
        }

        return builder.ToString();
    }
}
