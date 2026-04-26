using System;
using FarmGame.Data.Core;
using Sunset.Story;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PackageSaveSettingsPanel : MonoBehaviour
{
    private const string RuntimeRootName = "SaveSettingsRuntimeRoot";
    private const int TitleFontSize = 32;
    private const int SectionTitleFontSize = 19;
    private const int BodyFontSize = 15;
    private const int SmallFontSize = 14;
    private const int ButtonFontSize = 15;

    private static readonly Color TextTint = new Color(0.22f, 0.13f, 0.07f, 1f);
    private static readonly Color MutedTint = new Color(0.45f, 0.28f, 0.15f, 1f);
    private static readonly Color AccentTint = new Color(0.68f, 0.36f, 0.09f, 1f);
    private static readonly Color AccentSoftTint = new Color(0.36f, 0.18f, 0.06f, 1f);
    private static readonly Color LineTint = new Color(0.47f, 0.27f, 0.10f, 0.32f);
    private static readonly Color HeaderSectionTint = new Color(0.96f, 0.78f, 0.49f, 0.48f);
    private static readonly Color CurrentSectionTint = new Color(0.96f, 0.82f, 0.58f, 0.38f);
    private static readonly Color DefaultSectionTint = new Color(0.96f, 0.80f, 0.54f, 0.44f);
    private static readonly Color OrdinarySectionTint = new Color(0.95f, 0.79f, 0.54f, 0.34f);
    private static readonly Color SectionOutlineTint = new Color(0.39f, 0.20f, 0.08f, 0.42f);
    private static readonly Color ViewportTint = new Color(0.98f, 0.89f, 0.75f, 1f);
    private static readonly Color ViewportOutlineTint = new Color(0.45f, 0.25f, 0.10f, 0.36f);
    private static readonly Color SceneCardTint = new Color(0.95f, 0.84f, 0.63f, 0.88f);
    private static readonly Color StoryCardTint = new Color(0.94f, 0.82f, 0.60f, 0.88f);
    private static readonly Color GaugeCardTint = new Color(0.96f, 0.86f, 0.67f, 0.88f);
    private static readonly Color DefaultSummaryTint = new Color(0.95f, 0.83f, 0.62f, 0.94f);
    private static readonly Color LoadTint = new Color(0.95f, 0.84f, 0.65f, 0.88f);
    private static readonly Color SlotTint = new Color(0.95f, 0.83f, 0.61f, 0.74f);
    private static readonly Color ActionPanelTint = new Color(0.88f, 0.74f, 0.54f, 0.82f);
    private static readonly Color LoadButtonTint = new Color(0.53f, 0.22f, 0.07f, 0.92f);
    private static readonly Color CopyTint = new Color(0.43f, 0.49f, 0.71f, 0.94f);
    private static readonly Color PasteTint = new Color(0.73f, 0.62f, 0.33f, 0.94f);
    private static readonly Color SaveTint = new Color(0.39f, 0.55f, 0.35f, 0.94f);
    private static readonly Color DangerTint = new Color(0.71f, 0.40f, 0.29f, 0.94f);
    private static readonly Color WarningTint = new Color(0.83f, 0.48f, 0.31f, 0.96f);
    private static readonly Color ModalTint = new Color(0.97f, 0.86f, 0.67f, 0.99f);
    private static readonly Color ModalOutlineTint = new Color(0.35f, 0.2f, 0.08f, 0.42f);
    private static readonly Color ModalCancelTint = new Color(0.56f, 0.40f, 0.24f, 0.92f);

    private Font _font;
    private bool _built;
    private SaveManager _managerHook;
    private Text _statusText;
    private Text _currentSceneText;
    private Text _currentStoryText;
    private Text _currentGaugeText;
    private Text _defaultSummaryText;
    private Text _clipText;
    private Button _defaultLoadButton;
    private Button _defaultCopyButton;
    private Button _defaultPasteButton;
    private Button _defaultOverwriteButton;
    private Button _newSlotButton;
    private Button _restartButton;
    private GameObject _modalOverlay;
    private Text _modalTitleText;
    private Text _modalBodyText;
    private Action _modalConfirmAction;
    private RectTransform _ordinaryContent;
    private ScrollRect _scrollRect;
    private LayoutElement _defaultSectionLayout;
    private LayoutElement _defaultSummaryLayout;
    private LayoutElement _ordinaryScrollLayout;
    private GameSaveData _copiedSaveData;
    private string _copiedLabel;

    public static void EnsureInstalled(GameObject panelRoot)
    {
        Transform settingsMain = FindSettingsMain(panelRoot != null ? panelRoot.transform : null);
        if (settingsMain == null) return;

        PackageSaveSettingsPanel panel = settingsMain.GetComponent<PackageSaveSettingsPanel>();
        if (panel == null) panel = settingsMain.gameObject.AddComponent<PackageSaveSettingsPanel>();
        panel.EnsureBuilt();
    }

    private void Awake() => EnsureBuilt();

    private void OnEnable()
    {
        EnsureBuilt();
        HookManager();
        ClearCopyBuffer();
        RefreshView();
    }

    private void OnDisable()
    {
        if (_managerHook != null) _managerHook.SaveSlotsChanged -= RefreshView;
        _managerHook = null;
        HideModal();
    }

    private void EnsureBuilt()
    {
        if (_built) return;

        _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        Transform old = transform.Find(RuntimeRootName);
        if (old != null)
        {
            if (Application.isPlaying) Destroy(old.gameObject);
            else DestroyImmediate(old.gameObject);
        }

        RectTransform root = Rect(RuntimeRootName, transform);
        Stretch(root, 68f, 22f, 68f, 18f);

        VerticalLayoutGroup rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
        rootLayout.spacing = 6f;
        rootLayout.padding = new RectOffset(0, 0, 0, 0);
        rootLayout.childControlWidth = true;
        rootLayout.childControlHeight = true;
        rootLayout.childForceExpandWidth = true;
        rootLayout.childForceExpandHeight = false;

        RectTransform headerSection = Section("HeaderSection", root, 5f);
        DecoratePanel(headerSection, HeaderSectionTint, SectionOutlineTint, 14, 14, 10, 10, new Vector2(2f, -2f));
        Layout(headerSection.gameObject).minHeight = 0f;
        Layout(headerSection.gameObject).preferredHeight = -1f;
        RectTransform headerMain = Row("HeaderMain", headerSection, 10f, false);
        RectTransform headerLeft = Section("HeaderLeft", headerMain, 2f);
        Flex(headerLeft.gameObject);
        RectTransform titleRow = Row("TitleRow", headerLeft, 12f, false);
        Text titleText = Text("Title", titleRow, "存档管理", TitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        Flex(titleText.gameObject);
        _statusText = Text("Status", headerLeft, "正在读取存档状态…", SmallFontSize, MutedTint, FontStyle.Normal, TextAnchor.MiddleLeft);
        _statusText.lineSpacing = 1.08f;

        RectTransform headerRight = Row("HeaderRight", headerMain, 10f, false);
        _restartButton = ActionButton(headerRight, "重新开始", WarningTint, HandleRestart, 116f);
        Layout(_restartButton.gameObject).minHeight = 54f;
        Layout(_restartButton.gameObject).preferredHeight = 54f;
        Button quitButton = ActionButton(headerRight, "退出游戏", DangerTint, HandleExitGame, 116f);
        Layout(quitButton.gameObject).minHeight = 54f;
        Layout(quitButton.gameObject).preferredHeight = 54f;

        RectTransform currentSection = Section("CurrentSection", root, 5f);
        DecoratePanel(currentSection, CurrentSectionTint, SectionOutlineTint, 10, 10, 6, 6, new Vector2(2f, -2f));
        Layout(currentSection.gameObject).minHeight = 0f;
        Layout(currentSection.gameObject).preferredHeight = -1f;
        Text("CurrentLabel", currentSection, "当前进度", SectionTitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        RectTransform currentRow = Row("CurrentRow", currentSection, 10f, true);
        Layout(currentRow.gameObject).minHeight = 0f;
        Layout(currentRow.gameObject).preferredHeight = -1f;

        RectTransform sceneCard = SummaryPanel("SceneCard", currentRow, SceneCardTint, SectionOutlineTint);
        TunePanelLayout(sceneCard, 6, 6, 4, 4, 1f);
        Layout(sceneCard.gameObject).flexibleWidth = 1f;
        Layout(sceneCard.gameObject).minHeight = 0f;
        Layout(sceneCard.gameObject).preferredHeight = -1f;
        Text("SceneLabel", sceneCard, "场景", SmallFontSize, MutedTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        _currentSceneText = Text("SceneValue", sceneCard, "读取中…", BodyFontSize, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
        _currentSceneText.lineSpacing = 1.02f;

        RectTransform storyCard = SummaryPanel("StoryCard", currentRow, StoryCardTint, SectionOutlineTint);
        TunePanelLayout(storyCard, 6, 6, 4, 4, 1f);
        Layout(storyCard.gameObject).flexibleWidth = 1f;
        Layout(storyCard.gameObject).minHeight = 0f;
        Layout(storyCard.gameObject).preferredHeight = -1f;
        Text("StoryLabel", storyCard, "剧情 / 语言", SmallFontSize, MutedTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        _currentStoryText = Text("StoryValue", storyCard, "读取中…", BodyFontSize, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
        _currentStoryText.lineSpacing = 1.02f;

        RectTransform gaugeCard = SummaryPanel("GaugeCard", currentRow, GaugeCardTint, SectionOutlineTint);
        TunePanelLayout(gaugeCard, 6, 6, 4, 4, 1f);
        Layout(gaugeCard.gameObject).flexibleWidth = 1f;
        Layout(gaugeCard.gameObject).minHeight = 0f;
        Layout(gaugeCard.gameObject).preferredHeight = -1f;
        Text("GaugeLabel", gaugeCard, "生命 / 精力", SmallFontSize, MutedTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        _currentGaugeText = Text("GaugeValue", gaugeCard, "读取中…", BodyFontSize, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
        _currentGaugeText.lineSpacing = 1.02f;

        RectTransform defaultSection = Section("DefaultSection", root, 6f);
        DecoratePanel(defaultSection, DefaultSectionTint, SectionOutlineTint, 12, 12, 8, 8, new Vector2(2f, -2f));
        _defaultSectionLayout = Layout(defaultSection.gameObject);
        _defaultSectionLayout.minHeight = 0f;
        _defaultSectionLayout.preferredHeight = -1f;
        RectTransform defaultTop = Row("DefaultTop", defaultSection, 10f, false);
        Text defaultTitle = Text("DefaultTitle", defaultTop, "默认存档", SectionTitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        Flex(defaultTitle.gameObject);
        Text("DefaultNote", defaultTop, "受保护槽", SmallFontSize, MutedTint, FontStyle.Normal, TextAnchor.MiddleRight);

        RectTransform defaultBody = Row("DefaultBody", defaultSection, 10f, false);
        RectTransform defaultSummaryPanel = SummaryPanel("DefaultSummaryPanel", defaultBody, DefaultSummaryTint, SectionOutlineTint);
        TunePanelLayout(defaultSummaryPanel, 10, 10, 10, 10, 3f);
        _defaultSummaryLayout = Layout(defaultSummaryPanel.gameObject);
        _defaultSummaryLayout.flexibleWidth = 1f;
        _defaultSummaryLayout.minHeight = 0f;
        _defaultSummaryLayout.preferredHeight = -1f;
        Text("DefaultTap", defaultSummaryPanel, "默认存档摘要", BodyFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        _defaultSummaryText = Text("DefaultSummary", defaultSummaryPanel, "读取中…", SmallFontSize, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
        RectTransform defaultActions = ActionPanel("DefaultActions", defaultBody, 6f);
        LayoutElement defaultActionsLayout = Layout(defaultActions.gameObject);
        defaultActionsLayout.minWidth = 166f;
        defaultActionsLayout.preferredWidth = 166f;
        defaultActionsLayout.minHeight = 0f;
        defaultActionsLayout.preferredHeight = -1f;
        _defaultLoadButton = ActionButton(defaultActions, "加载默认存档", LoadButtonTint, HandleDefaultLoad, 0f);
        Layout(_defaultLoadButton.gameObject).minHeight = 28f;
        Layout(_defaultLoadButton.gameObject).preferredHeight = 28f;
        _defaultCopyButton = ActionButton(defaultActions, "复制", CopyTint, () => HandleCopy(SaveManager.Instance != null ? SaveManager.Instance.DefaultSaveSlotName : string.Empty), 0f);
        _defaultPasteButton = ActionButton(defaultActions, "粘贴至当前存档", PasteTint, () => HandlePaste(SaveManager.Instance != null ? SaveManager.Instance.DefaultSaveSlotName : string.Empty), 0f);
        _defaultOverwriteButton = ActionButton(defaultActions, "覆盖当前存档", SaveTint, () => HandleOverwrite(SaveManager.Instance != null ? SaveManager.Instance.DefaultSaveSlotName : string.Empty), 0f);
        _defaultPasteButton.gameObject.SetActive(false);
        _defaultOverwriteButton.gameObject.SetActive(false);

        RectTransform ordinarySection = Section("OrdinarySection", root, 6f);
        DecoratePanel(ordinarySection, OrdinarySectionTint, SectionOutlineTint, 12, 12, 6, 6, new Vector2(2f, -2f));
        LayoutElement ordinarySectionLayout = Layout(ordinarySection.gameObject);
        ordinarySectionLayout.minHeight = 232f;
        ordinarySectionLayout.preferredHeight = 0f;
        ordinarySectionLayout.flexibleHeight = 1f;

        RectTransform ordinaryTop = Row("OrdinaryTop", ordinarySection, 10f, false);
        Text ordinaryTitle = Text("OrdinaryTitle", ordinaryTop, "普通存档", SectionTitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
        Layout(ordinaryTitle.gameObject).preferredWidth = 86f;
        _clipText = Text("Clip", ordinaryTop, "缓存：无", SmallFontSize, MutedTint, FontStyle.Normal, TextAnchor.MiddleLeft);
        Flex(_clipText.gameObject);
        _newSlotButton = ActionButton(ordinaryTop, "新建存档", SaveTint, HandleNewSlot, 108f);

        RectTransform scrollRoot = Rect("ScrollRoot", ordinarySection);
        _ordinaryScrollLayout = Layout(scrollRoot.gameObject);
        _ordinaryScrollLayout.minHeight = 210f;
        _ordinaryScrollLayout.preferredHeight = 210f;
        _ordinaryScrollLayout.flexibleHeight = 1f;

        RectTransform viewport = Rect("Viewport", scrollRoot);
        Stretch(viewport, 0f, 2f, 16f, 2f);
        Image viewportImage = viewport.gameObject.AddComponent<Image>();
        viewportImage.color = ViewportTint;
        Outline viewportOutline = viewport.gameObject.AddComponent<Outline>();
        viewportOutline.effectColor = ViewportOutlineTint;
        viewportOutline.effectDistance = new Vector2(2f, -2f);
        viewport.gameObject.AddComponent<RectMask2D>();

        _ordinaryContent = Rect("Content", viewport);
        _ordinaryContent.anchorMin = new Vector2(0f, 1f);
        _ordinaryContent.anchorMax = new Vector2(1f, 1f);
        _ordinaryContent.pivot = new Vector2(0.5f, 1f);
        _ordinaryContent.anchoredPosition = Vector2.zero;
        _ordinaryContent.sizeDelta = Vector2.zero;
        VerticalLayoutGroup ordinaryContentLayout = _ordinaryContent.gameObject.AddComponent<VerticalLayoutGroup>();
        ordinaryContentLayout.spacing = 6f;
        ordinaryContentLayout.padding = new RectOffset(0, 0, 6, 6);
        ordinaryContentLayout.childControlWidth = true;
        ordinaryContentLayout.childControlHeight = true;
        ordinaryContentLayout.childForceExpandWidth = true;
        ordinaryContentLayout.childForceExpandHeight = false;
        ContentSizeFitter ordinaryFitter = _ordinaryContent.gameObject.AddComponent<ContentSizeFitter>();
        ordinaryFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        ordinaryFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        _scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
        _scrollRect.viewport = viewport;
        _scrollRect.content = _ordinaryContent;
        _scrollRect.horizontal = false;
        _scrollRect.vertical = true;
        _scrollRect.movementType = ScrollRect.MovementType.Clamped;
        _scrollRect.inertia = false;
        _scrollRect.scrollSensitivity = 4f;

        RectTransform scrollbarRoot = Rect("Scrollbar", scrollRoot);
        scrollbarRoot.anchorMin = new Vector2(1f, 0f);
        scrollbarRoot.anchorMax = new Vector2(1f, 1f);
        scrollbarRoot.pivot = new Vector2(1f, 1f);
        scrollbarRoot.offsetMin = new Vector2(-10f, 6f);
        scrollbarRoot.offsetMax = new Vector2(0f, -4f);
        Image scrollbarTrack = scrollbarRoot.gameObject.AddComponent<Image>();
        scrollbarTrack.color = new Color(0.78f, 0.61f, 0.37f, 0.42f);
        Scrollbar scrollbar = scrollbarRoot.gameObject.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        RectTransform handleArea = Rect("HandleArea", scrollbarRoot);
        Stretch(handleArea, 1f, 1f, 1f, 1f);
        RectTransform handle = Rect("Handle", handleArea);
        Stretch(handle, 0f, 0f, 0f, 0f);
        Image handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.color = new Color(0.66f, 0.43f, 0.18f, 0.94f);
        scrollbar.handleRect = handle;
        scrollbar.targetGraphic = handleImage;
        _scrollRect.verticalScrollbar = scrollbar;
        _scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        BuildModal();

        _built = true;
    }

    private void HookManager()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null || manager == _managerHook) return;

        if (_managerHook != null) _managerHook.SaveSlotsChanged -= RefreshView;
        _managerHook = manager;
        _managerHook.SaveSlotsChanged += RefreshView;
    }

    private void RefreshView()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            _statusText.text = "未找到 SaveManager。";
            _statusText.color = WarningTint;
            _currentSceneText.text = "当前不可读";
            _currentStoryText.text = "当前不可读";
            _currentGaugeText.text = "-- / --";
            _defaultLoadButton.interactable = false;
            _defaultCopyButton.interactable = false;
            _defaultPasteButton.interactable = false;
            _defaultOverwriteButton.interactable = false;
            _newSlotButton.interactable = false;
            _restartButton.interactable = false;
            _defaultSummaryText.text = "默认存档当前不可用。";
            ApplyLayoutProfile(false);
            RebuildOrdinary(Array.Empty<string>());
            HideModal();
            return;
        }

        bool canSave = manager.CanExecutePlayerSaveAction(out string saveBlockerReason);
        bool canLoad = manager.CanExecutePlayerLoadAction(out string loadBlockerReason);
        if (canSave && canLoad)
        {
            _statusText.text = "默认槽支持 F5 快速保存 / F9 快速读取；重新开始不会改写默认存档。";
            _statusText.color = MutedTint;
        }
        else if (canLoad)
        {
            _statusText.text = $"当前默认槽可读但不可写：{saveBlockerReason}";
            _statusText.color = WarningTint;
        }
        else
        {
            _statusText.text = $"当前默认槽暂不可读：{loadBlockerReason}";
            _statusText.color = WarningTint;
        }

        StoryManager story = StoryManager.Instance ?? FindFirstObjectByType<StoryManager>(FindObjectsInactive.Include);
        HealthSystem hp = HealthSystem.Instance;
        EnergySystem ep = FindFirstObjectByType<EnergySystem>(FindObjectsInactive.Include);
        _currentSceneText.text = SceneLabel(SceneManager.GetActiveScene().name);
        _currentStoryText.text = $"{PhaseLabel(story != null ? story.CurrentPhase : 0)} / {(story != null && story.IsLanguageDecoded ? "已解码" : "未解码")}";
        _currentGaugeText.text = $"生 {(hp != null ? $"{hp.CurrentHealth}/{hp.MaxHealth}" : "--/--")}    精 {(ep != null ? $"{ep.CurrentEnergy}/{ep.MaxEnergy}" : "--/--")}";

        manager.TryGetDefaultSlotSummary(out SaveSlotSummary defaultSummary);
        bool defaultReady = defaultSummary != null && defaultSummary.exists && string.IsNullOrWhiteSpace(defaultSummary.loadError);
        _defaultLoadButton.interactable = defaultReady;
        _defaultCopyButton.interactable = defaultReady;
        _defaultPasteButton.interactable = false;
        _defaultOverwriteButton.interactable = false;
        _defaultSummaryText.text = defaultReady
            ? CompactSummary(defaultSummary)
            : !string.IsNullOrWhiteSpace(defaultSummary?.loadError)
                ? $"默认存档摘要读取异常：{defaultSummary.loadError}\n请先修复该槽位，或重新用 F5 写入。"
                : "默认存档尚未保存。\n按 F5 可把当前进度快速保存到默认存档。";
        _defaultSummaryText.lineSpacing = 1f;

        _newSlotButton.interactable = true;
        _restartButton.interactable = true;

        string[] ordinarySlotNames = manager.GetOrdinarySlotNames();
        ApplyLayoutProfile(ordinarySlotNames.Length > 0);
        RebuildOrdinary(ordinarySlotNames);
        UpdateClipText();
    }

    private void ApplyLayoutProfile(bool hasOrdinarySlots)
    {
        if (_ordinaryScrollLayout == null) return;

        if (hasOrdinarySlots)
        {
            _ordinaryScrollLayout.minHeight = 210f;
            _ordinaryScrollLayout.preferredHeight = 210f;
            return;
        }

        _ordinaryScrollLayout.minHeight = 188f;
        _ordinaryScrollLayout.preferredHeight = 188f;
    }

    private void RebuildOrdinary(string[] slotNames)
    {
        if (_ordinaryContent == null) return;

        for (int i = _ordinaryContent.childCount - 1; i >= 0; i--)
        {
            Transform child = _ordinaryContent.GetChild(i);
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }

        SaveManager manager = SaveManager.Instance;
        if (slotNames == null || slotNames.Length == 0 || manager == null)
        {
            RectTransform emptyRoot = Section("Empty", _ordinaryContent, 6f);
            DecoratePanel(emptyRoot, LoadTint, SectionOutlineTint, 14, 14, 12, 12, new Vector2(2f, -2f));
            Layout(emptyRoot.gameObject).minHeight = 86f;
            Text("EmptyTitle", emptyRoot, "还没有普通存档", SectionTitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
            Text("EmptyHint", emptyRoot, "点击右上角“新建存档”，把当前进度另存为普通槽位。", BodyFontSize, TextTint, FontStyle.Normal, TextAnchor.UpperLeft);
            ResetScroll();
            return;
        }

        bool canSave = manager.CanExecutePlayerSaveAction(out _);
        foreach (string slotName in slotNames)
        {
            string currentSlotName = slotName;
            manager.TryGetSlotSummary(currentSlotName, out SaveSlotSummary summary);

            RectTransform slotRoot = Section(currentSlotName, _ordinaryContent, 8f);
            LayoutElement slotRootLayout = Layout(slotRoot.gameObject);
            slotRootLayout.flexibleWidth = 1f;
            slotRootLayout.minHeight = 92f;
            slotRootLayout.preferredHeight = 92f;
            DecoratePanel(slotRoot, SlotTint, SectionOutlineTint, 10, 10, 8, 8, new Vector2(2f, -2f));

            RectTransform body = Row("Body", slotRoot, 8f, false);
            LayoutElement bodyLayout = Layout(body.gameObject);
            bodyLayout.flexibleWidth = 1f;
            bodyLayout.minHeight = 76f;
            bodyLayout.preferredHeight = 76f;

            RectTransform summaryPanel = SummaryPanel("Summary", body, LoadTint, SectionOutlineTint);
            TunePanelLayout(summaryPanel, 10, 10, 6, 6, 2f);
            LayoutElement summaryLayout = Layout(summaryPanel.gameObject);
            summaryLayout.flexibleWidth = 1f;
            summaryLayout.minWidth = 0f;
            summaryLayout.preferredWidth = 0f;
            summaryLayout.minHeight = 76f;
            summaryLayout.preferredHeight = 76f;
            Text("Tap", summaryPanel, $"{manager.GetSlotDisplayName(currentSlotName)}", BodyFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleLeft);
            string detail = summary != null && summary.exists && string.IsNullOrWhiteSpace(summary.loadError)
                ? CompactSummary(summary)
                : !string.IsNullOrWhiteSpace(summary?.loadError)
                    ? $"当前无法读取：{summary.loadError}\n你可以直接覆盖它，或者删除后重建。"
                    : "这个普通槽位当前没有存档内容。";
            Text detailText = Text("Detail", summaryPanel, detail, 13, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
            detailText.lineSpacing = 1.02f;
            Layout(detailText.gameObject).preferredWidth = 0f;
            Layout(detailText.gameObject).minWidth = 0f;

            RectTransform actionColumn = ActionPanel("ActionColumn", body, 4f);
            TunePanelLayout(actionColumn, 8, 8, 4, 4, 2f);
            LayoutElement actionColumnLayout = Layout(actionColumn.gameObject);
            actionColumnLayout.flexibleWidth = 0f;
            actionColumnLayout.minWidth = 220f;
            actionColumnLayout.preferredWidth = 220f;
            actionColumnLayout.minHeight = 76f;
            actionColumnLayout.preferredHeight = 76f;

            Button loadButton = ActionButton(actionColumn, "加载存档", LoadButtonTint, () => HandleLoad(currentSlotName), 0f, summary != null && summary.exists && string.IsNullOrWhiteSpace(summary.loadError));
            Layout(loadButton.gameObject).minHeight = 24f;
            Layout(loadButton.gameObject).preferredHeight = 24f;

            RectTransform actionRowTop = Row("ActionRowTop", actionColumn, 4f, true);
            LayoutElement actionRowTopLayout = Layout(actionRowTop.gameObject);
            actionRowTopLayout.minHeight = 20f;
            actionRowTopLayout.preferredHeight = 20f;
            Button copyButton = ActionButton(actionRowTop, "复制", CopyTint, () => HandleCopy(currentSlotName), 0f);
            Layout(copyButton.gameObject).minHeight = 20f;
            Layout(copyButton.gameObject).preferredHeight = 20f;
            Button pasteButton = ActionButton(actionRowTop, "粘贴", PasteTint, () => HandlePaste(currentSlotName), 0f, _copiedSaveData != null);
            Layout(pasteButton.gameObject).minHeight = 20f;
            Layout(pasteButton.gameObject).preferredHeight = 20f;

            RectTransform actionRowBottom = Row("ActionRowBottom", actionColumn, 4f, true);
            LayoutElement actionRowBottomLayout = Layout(actionRowBottom.gameObject);
            actionRowBottomLayout.minHeight = 20f;
            actionRowBottomLayout.preferredHeight = 20f;
            Button overwriteButton = ActionButton(actionRowBottom, "覆盖", SaveTint, () => HandleOverwrite(currentSlotName), 0f, canSave);
            Layout(overwriteButton.gameObject).minHeight = 20f;
            Layout(overwriteButton.gameObject).preferredHeight = 20f;
            Button deleteButton = ActionButton(actionRowBottom, "删除", DangerTint, () => HandleDelete(currentSlotName), 0f);
            Layout(deleteButton.gameObject).minHeight = 20f;
            Layout(deleteButton.gameObject).preferredHeight = 20f;

        }

        ResetScroll();
    }

    private void HandleNewSlot()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (!manager.CanExecutePlayerSaveAction(out string blockerReason))
        {
            SaveActionToastOverlay.Show(blockerReason);
            return;
        }

        if (manager.CreateNewOrdinarySlotFromCurrentProgress(out string slotName))
        {
            SaveActionToastOverlay.Show($"已新建 {manager.GetSlotDisplayName(slotName)}");
            RefreshView();
            return;
        }

        SaveActionToastOverlay.Show("新建存档失败，请稍后重试。");
    }

    private void HandleDefaultLoad()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (!manager.CanExecutePlayerLoadAction(out string blockerReason))
        {
            SaveActionToastOverlay.Show(blockerReason);
            return;
        }

        ShowConfirmation(
            "读取默认存档",
            "确认读取默认存档吗？\n当前未保存的实时进度会被默认存档替换。",
            () =>
            {
                ClosePanel();
                manager.QuickLoadDefaultSlot(success => SaveActionToastOverlay.Show(success ? "已读取默认存档" : "默认存档读取失败"));
            });
    }

    private void HandleLoad(string slotName)
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (!manager.CanExecutePlayerLoadAction(out string blockerReason))
        {
            SaveActionToastOverlay.Show(blockerReason);
            return;
        }

        string displayName = manager.GetSlotDisplayName(slotName);
        ShowConfirmation(
            "读取普通存档",
            $"确认读取 {displayName} 吗？\n当前未保存的实时进度会被替换。",
            () =>
            {
                ClosePanel();
                manager.LoadGame(slotName, success => SaveActionToastOverlay.Show(success ? "已读档" : "读档失败"));
            });
    }

    private void HandleCopy(string slotName)
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (!manager.TryCopySlotData(slotName, out _copiedSaveData, out string error))
        {
            SaveActionToastOverlay.Show(string.IsNullOrWhiteSpace(error) ? "复制失败" : error);
            return;
        }

        _copiedLabel = manager.GetSlotDisplayName(slotName);
        UpdateClipText();
        RefreshView();
        SaveActionToastOverlay.Show($"已复制 {_copiedLabel}");
    }

    private void HandlePaste(string slotName)
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (_copiedSaveData == null)
        {
            SaveActionToastOverlay.Show("请先复制存档内容");
            return;
        }

        string targetLabel = manager.GetSlotDisplayName(slotName);
        ShowConfirmation(
            "粘贴存档内容",
            $"确认把 {_copiedLabel} 的内容粘贴到 {targetLabel} 吗？\n这会覆盖目标槽位原有内容。",
            () =>
            {
                if (!manager.PasteSaveDataToSlot(_copiedSaveData, slotName, out string error))
                {
                    SaveActionToastOverlay.Show(string.IsNullOrWhiteSpace(error) ? "粘贴失败" : error);
                    return;
                }

                SaveActionToastOverlay.Show($"已把 {_copiedLabel} 粘贴到 {targetLabel}");
                RefreshView();
            });
    }

    private void HandleOverwrite(string slotName)
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (manager.IsDefaultSlot(slotName))
        {
            SaveActionToastOverlay.Show("默认存档不允许手动覆盖，请使用 F5 快速保存。");
            return;
        }

        string displayName = manager.GetSlotDisplayName(slotName);
        string body = $"确认用当前实时进度覆盖 {displayName} 吗？\n原有内容会被替换。";
        ShowConfirmation(
            "覆盖存档",
            body,
            () =>
            {
                if (manager.SaveGame(slotName))
                {
                    SaveActionToastOverlay.Show($"已覆盖 {displayName}");
                    RefreshView();
                    return;
                }

                SaveActionToastOverlay.Show(manager.CanExecutePlayerSaveAction(out string reason) ? "覆盖失败" : reason);
            });
    }

    private void HandleDelete(string slotName)
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        string displayName = manager.GetSlotDisplayName(slotName);
        ShowConfirmation(
            "删除普通存档",
            $"确认删除 {displayName} 吗？\n删除后不能恢复。",
            () =>
            {
                SaveActionToastOverlay.Show(manager.DeleteSave(slotName) ? $"{displayName} 已删除" : "删除失败");
                RefreshView();
            });
    }

    private void HandleRestart()
    {
        SaveManager manager = SaveManager.Instance;
        if (manager == null)
        {
            SaveActionToastOverlay.Show("未找到 SaveManager");
            return;
        }

        if (!manager.CanExecutePlayerRestartAction(out string blockerReason))
        {
            SaveActionToastOverlay.Show(blockerReason);
            return;
        }

        ShowConfirmation(
            "重新开始游戏",
            "确认重新开始游戏吗？\n当前进度会回到原生起点，但不会改写默认存档。",
            () =>
            {
                ClosePanel();
                manager.RestartToFreshGame(success => SaveActionToastOverlay.Show(success ? "已重新开始" : "重新开始失败"));
            });
    }

    private void ClosePanel()
    {
        PackagePanelTabsUI tabsUi = GetComponentInParent<PackagePanelTabsUI>(true);
        if (tabsUi != null) tabsUi.ClosePanelForExternalAction();
    }

    private void HandleExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ClearCopyBuffer()
    {
        _copiedSaveData = null;
        _copiedLabel = string.Empty;
        UpdateClipText();
    }

    private void UpdateClipText()
    {
        if (_clipText == null) return;

        SaveManager manager = SaveManager.Instance;
        bool noOrdinarySlots = manager != null && manager.GetOrdinarySlotNames().Length == 0;
        if (noOrdinarySlots)
        {
            _clipText.text = "缓存：无";
            return;
        }

        _clipText.text = _copiedSaveData == null
            ? "缓存：无"
            : $"缓存：{_copiedLabel}";
    }

    private void ResetScroll()
    {
        if (_scrollRect == null || _ordinaryContent == null) return;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_ordinaryContent);
        if (_scrollRect.viewport != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.viewport);
        }
        Canvas.ForceUpdateCanvases();
        _scrollRect.StopMovement();
        _ordinaryContent.offsetMin = new Vector2(0f, _ordinaryContent.offsetMin.y);
        _ordinaryContent.offsetMax = new Vector2(0f, _ordinaryContent.offsetMax.y);
        _ordinaryContent.anchoredPosition = Vector2.zero;
        _scrollRect.verticalNormalizedPosition = 1f;
        Canvas.ForceUpdateCanvases();
    }

    private string Summary(SaveSlotSummary summary)
    {
        string worldTime = $"第 {Mathf.Max(summary.year, 1)} 年 · {SeasonLabel(summary.season)} {Mathf.Max(summary.day, 1):00} 日 {Mathf.Clamp(summary.hour, 0, 23):00}:{Mathf.Clamp(summary.minute, 0, 59):00}";
        string health = summary.healthCurrent >= 0 && summary.healthMax > 0 ? $"{summary.healthCurrent}/{summary.healthMax}" : "--/--";
        string energy = summary.energyCurrent >= 0 && summary.energyMax > 0 ? $"{summary.energyCurrent}/{summary.energyMax}" : "--/--";
        return
            $"最近保存 · {TimeLabel(summary.lastSaveTime)}\n" +
            $"场景 · {SceneLabel(summary.sceneName)}    {worldTime}\n" +
            $"剧情 · {summary.storyPhaseLabel} / 语言 · {(summary.isLanguageDecoded ? "已解码" : "未解码")} / 背包 {Mathf.Max(summary.filledInventorySlots, 0)} / 生 {health} / 精 {energy}";
    }

    private string DefaultSummary(SaveSlotSummary summary)
    {
        string worldTime = $"{SeasonLabel(summary.season)} {Mathf.Max(summary.day, 1):00}日 {Mathf.Clamp(summary.hour, 0, 23):00}:{Mathf.Clamp(summary.minute, 0, 59):00}";
        return
            $"固定入口 · {TimeLabel(summary.lastSaveTime)}    {worldTime}\n" +
            $"场景 · {SceneLabel(summary.sceneName)}    剧情 · {summary.storyPhaseLabel}";
    }

    private string CompactSummary(SaveSlotSummary summary)
    {
        string worldTime = $"{SeasonLabel(summary.season)} {Mathf.Max(summary.day, 1):00}日 {Mathf.Clamp(summary.hour, 0, 23):00}:{Mathf.Clamp(summary.minute, 0, 59):00}";
        string health = summary.healthCurrent >= 0 && summary.healthMax > 0 ? $"{summary.healthCurrent}/{summary.healthMax}" : "--/--";
        string energy = summary.energyCurrent >= 0 && summary.energyMax > 0 ? $"{summary.energyCurrent}/{summary.energyMax}" : "--/--";
        return
            $"最近 · {TimeLabel(summary.lastSaveTime)}    {SceneLabel(summary.sceneName)} / {worldTime}\n" +
            $"剧情 · {summary.storyPhaseLabel} / {(summary.isLanguageDecoded ? "已解码" : "未解码")}    生 {health} / 精 {energy}";
    }

    private RectTransform Section(string name, Transform parent, float spacing)
    {
        RectTransform rect = Rect(name, parent);
        VerticalLayoutGroup group = rect.gameObject.AddComponent<VerticalLayoutGroup>();
        group.spacing = spacing;
        group.padding = new RectOffset(0, 0, 0, 0);
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
        return rect;
    }

    private void DecoratePanel(RectTransform rect, Color fill, Color outline, int leftPadding, int rightPadding, int topPadding, int bottomPadding, Vector2 outlineDistance)
    {
        Image image = rect.gameObject.GetComponent<Image>();
        if (image == null) image = rect.gameObject.AddComponent<Image>();
        image.color = fill;

        Outline panelOutline = rect.gameObject.GetComponent<Outline>();
        if (panelOutline == null) panelOutline = rect.gameObject.AddComponent<Outline>();
        panelOutline.effectColor = outline;
        panelOutline.effectDistance = outlineDistance;

        VerticalLayoutGroup group = rect.gameObject.GetComponent<VerticalLayoutGroup>();
        if (group != null)
        {
            group.padding = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
        }
    }

    private RectTransform ActionPanel(string name, Transform parent, float spacing)
    {
        RectTransform rect = Section(name, parent, spacing);
        DecoratePanel(rect, ActionPanelTint, SectionOutlineTint, 8, 8, 6, 6, new Vector2(2f, -2f));
        return rect;
    }

    private RectTransform SummaryPanel(string name, Transform parent)
    {
        return SummaryPanel(name, parent, LoadTint, SectionOutlineTint);
    }

    private RectTransform SummaryPanel(string name, Transform parent, Color fill, Color outline)
    {
        RectTransform rect = Rect(name, parent);
        VerticalLayoutGroup group = rect.gameObject.AddComponent<VerticalLayoutGroup>();
        group.padding = new RectOffset(8, 8, 6, 6);
        group.spacing = 3f;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
        DecoratePanel(rect, fill, outline, 10, 10, 8, 8, new Vector2(2f, -2f));
        return rect;
    }

    private void TunePanelLayout(RectTransform rect, int left, int right, int top, int bottom, float spacing)
    {
        VerticalLayoutGroup group = rect != null ? rect.GetComponent<VerticalLayoutGroup>() : null;
        if (group == null) return;

        group.padding = new RectOffset(left, right, top, bottom);
        group.spacing = spacing;
    }

    private RectTransform Row(string name, Transform parent, float spacing, bool expandWidth)
    {
        RectTransform rect = Rect(name, parent);
        HorizontalLayoutGroup group = rect.gameObject.AddComponent<HorizontalLayoutGroup>();
        group.spacing = spacing;
        group.padding = new RectOffset(0, 0, 0, 0);
        group.childAlignment = TextAnchor.MiddleLeft;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = expandWidth;
        group.childForceExpandHeight = false;
        return rect;
    }

    private void Divider(Transform parent)
    {
        Divider(parent, 1f);
    }

    private void Divider(Transform parent, float height)
    {
        RectTransform rect = Rect("Divider", parent);
        LayoutElement layout = Layout(rect.gameObject);
        layout.minHeight = height;
        layout.preferredHeight = height;
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = LineTint;
    }

    private Button SummaryButton(string name, Transform parent, Action onClick)
    {
        RectTransform rect = Rect(name, parent);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = LoadTint;
        Outline outline = rect.gameObject.AddComponent<Outline>();
        outline.effectColor = SectionOutlineTint;
        outline.effectDistance = new Vector2(2f, -2f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => onClick?.Invoke());
        Colorize(button, LoadTint, 0.05f);

        VerticalLayoutGroup group = rect.gameObject.AddComponent<VerticalLayoutGroup>();
        group.padding = new RectOffset(12, 12, 9, 9);
        group.spacing = 1f;
        group.childControlWidth = true;
        group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
        return button;
    }

    private void BuildModal()
    {
        RectTransform overlay = Rect("SaveConfirmModal", transform);
        Stretch(overlay, 0f, 0f, 0f, 0f);
        overlay.SetAsLastSibling();
        Image overlayImage = overlay.gameObject.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.46f);
        overlayImage.raycastTarget = true;

        RectTransform panel = Rect("Panel", overlay);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(432f, 212f);
        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = ModalTint;
        Outline panelOutline = panel.gameObject.AddComponent<Outline>();
        panelOutline.effectColor = ModalOutlineTint;
        panelOutline.effectDistance = new Vector2(1f, -1f);

        VerticalLayoutGroup panelLayout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        panelLayout.padding = new RectOffset(22, 22, 18, 18);
        panelLayout.spacing = 12f;
        panelLayout.childControlWidth = true;
        panelLayout.childControlHeight = true;
        panelLayout.childForceExpandWidth = true;
        panelLayout.childForceExpandHeight = false;

        _modalTitleText = Text("Title", panel, "确认操作", SectionTitleFontSize, AccentSoftTint, FontStyle.Bold, TextAnchor.MiddleCenter);
        Layout(_modalTitleText.gameObject).minHeight = 34f;
        _modalBodyText = Text("Body", panel, "确认继续吗？", BodyFontSize, TextTint, FontStyle.Bold, TextAnchor.UpperLeft);
        _modalBodyText.lineSpacing = 1.08f;
        Layout(_modalBodyText.gameObject).minHeight = 58f;
        Layout(_modalBodyText.gameObject).preferredHeight = 58f;

        RectTransform buttonRow = Row("Buttons", panel, 10f, true);
        Layout(buttonRow.gameObject).minHeight = 40f;
        Button cancelButton = ActionButton(buttonRow, "取消", ModalCancelTint, HideModal, 0f);
        Button confirmButton = ActionButton(buttonRow, "确认", WarningTint, ConfirmModal, 0f);
        Layout(cancelButton.gameObject).flexibleWidth = 1f;
        Layout(confirmButton.gameObject).flexibleWidth = 1f;

        _modalOverlay = overlay.gameObject;
        _modalConfirmButton = confirmButton;
        _modalCancelButton = cancelButton;
        HideModal();
    }

    private Button _modalConfirmButton;
    private Button _modalCancelButton;

    private void ShowConfirmation(string title, string body, Action onConfirm)
    {
        if (_modalOverlay == null)
        {
            onConfirm?.Invoke();
            return;
        }

        _modalTitleText.text = title;
        _modalBodyText.text = body;
        _modalConfirmAction = onConfirm;
        _modalOverlay.SetActive(true);
        _modalOverlay.transform.SetAsLastSibling();
        _modalConfirmButton.Select();
    }

    private void ConfirmModal()
    {
        Action confirmAction = _modalConfirmAction;
        HideModal();
        confirmAction?.Invoke();
    }

    private void HideModal()
    {
        _modalConfirmAction = null;
        if (_modalOverlay != null) _modalOverlay.SetActive(false);
    }

    private Button ActionButton(Transform parent, string label, Color color, Action onClick, float width = 78f, bool interactable = true)
    {
        RectTransform rect = Rect(label, parent);
        LayoutElement layout = Layout(rect.gameObject);
        layout.minHeight = 34f;
        if (width > 0f)
        {
            layout.preferredWidth = width;
        }
        else
        {
            layout.flexibleWidth = 1f;
        }

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        Outline outline = rect.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.11f, 0.03f, 0.42f);
        outline.effectDistance = new Vector2(1f, -1f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.interactable = interactable;
        button.onClick.AddListener(() => onClick?.Invoke());
        Colorize(button, color, 0.86f);

        RectTransform labelRect = Rect("Label", rect);
        Stretch(labelRect, 0f, 0f, 0f, 0f);
        Text labelText = labelRect.gameObject.AddComponent<Text>();
        labelText.font = _font;
        labelText.text = label;
        labelText.fontSize = ButtonFontSize;
        labelText.fontStyle = FontStyle.Bold;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.horizontalOverflow = HorizontalWrapMode.Wrap;
        labelText.verticalOverflow = VerticalWrapMode.Overflow;
        labelText.supportRichText = false;
        return button;
    }

    private void Colorize(Button button, Color color, float disabledAlpha)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.08f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.14f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(color.r, color.g, color.b, disabledAlpha);
        button.colors = colors;
    }

    private Text Text(string name, Transform parent, string content, int size, Color color, FontStyle style, TextAnchor anchor)
    {
        RectTransform rect = Rect(name, parent);
        Text text = rect.gameObject.AddComponent<Text>();
        text.font = _font;
        text.text = content;
        text.fontSize = size;
        text.color = color;
        text.fontStyle = style;
        text.alignment = anchor;
        text.lineSpacing = 1.08f;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        ContentSizeFitter fitter = rect.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return text;
    }

    private static LayoutElement Layout(GameObject target)
    {
        LayoutElement element = target.GetComponent<LayoutElement>();
        if (element == null) element = target.AddComponent<LayoutElement>();
        return element;
    }

    private static void Flex(GameObject target)
    {
        Layout(target).flexibleWidth = 1f;
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static void Stretch(RectTransform rect, float left, float top, float right, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private static Transform FindSettingsMain(Transform root)
    {
        if (root == null) return null;

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name != "5_Settings") continue;
            Transform main = child.Find("Main");
            return main != null ? main : child;
        }

        return null;
    }

    private static string TimeLabel(string raw) => string.IsNullOrWhiteSpace(raw) ? "未记录" : (DateTime.TryParse(raw, out DateTime parsed) ? parsed.ToString("yyyy-MM-dd HH:mm") : raw);
    private static string SeasonLabel(int season) => season switch { 1 => "夏", 2 => "秋", 3 => "冬", _ => "春" };
    private static string SceneLabel(string scene) => string.IsNullOrWhiteSpace(scene) ? "当前场景" : scene switch
    {
        "Primary" => "坠落开局场景",
        "Town" => "村镇场景",
        "DontDestroyOnLoad" => "当前场景",
        _ => scene
    };
    private static string PhaseLabel(StoryPhase phase) => phase switch { StoryPhase.CrashAndMeet => "坠落与相遇", StoryPhase.EnterVillage => "进入村口", StoryPhase.HealingAndHP => "疗伤教学", StoryPhase.WorkbenchFlashback => "工作台回想", StoryPhase.FarmingTutorial => "农田教学", StoryPhase.DinnerConflict => "晚餐冲突", StoryPhase.ReturnAndReminder => "返程提醒", StoryPhase.FreeTime => "自由行动", StoryPhase.DayEnd => "日终", _ => "开局" };
}
