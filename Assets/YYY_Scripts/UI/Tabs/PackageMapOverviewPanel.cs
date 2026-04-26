using System.Collections.Generic;
using TMPro;
using Sunset.Story;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PackageMapOverviewPanel : MonoBehaviour
{
    private const string RuntimeRootName = "MapOverviewRuntimeRoot";

    private static readonly Color PageTint = new Color(0.97f, 0.92f, 0.84f, 0.97f);
    private static readonly Color CardTint = new Color(0.97f, 0.90f, 0.79f, 0.96f);
    private static readonly Color CardStrongTint = new Color(0.96f, 0.88f, 0.74f, 0.98f);
    private static readonly Color BoardTint = new Color(0.95f, 0.88f, 0.75f, 0.99f);
    private static readonly Color BoardZoneTint = new Color(0.77f, 0.58f, 0.30f, 0.04f);
    private static readonly Color MineZoneTint = new Color(0.56f, 0.42f, 0.28f, 0.18f);
    private static readonly Color VillageZoneStrongTint = new Color(0.84f, 0.68f, 0.43f, 0.18f);
    private static readonly Color HomeZoneTint = new Color(0.82f, 0.62f, 0.38f, 0.20f);
    private static readonly Color WorkZoneTint = new Color(0.79f, 0.59f, 0.32f, 0.20f);
    private static readonly Color FieldZoneTint = new Color(0.63f, 0.69f, 0.34f, 0.20f);
    private static readonly Color DinnerZoneTint = new Color(0.86f, 0.56f, 0.33f, 0.18f);
    private static readonly Color NightZoneTint = new Color(0.61f, 0.54f, 0.44f, 0.18f);
    private static readonly Color InkTint = new Color(0.22f, 0.13f, 0.08f, 1f);
    private static readonly Color SubtleTint = new Color(0.40f, 0.27f, 0.18f, 1f);
    private static readonly Color RoadTint = new Color(0.47f, 0.31f, 0.19f, 0.70f);
    private static readonly Color ActiveTint = new Color(0.80f, 0.46f, 0.12f, 1f);
    private static readonly Color UnlockedTint = new Color(0.49f, 0.35f, 0.19f, 1f);
    private static readonly Color LockedTint = new Color(0.50f, 0.43f, 0.37f, 0.44f);
    private static readonly Color HighlightCardTint = new Color(0.98f, 0.91f, 0.78f, 0.99f);
    private static readonly Color RouteCurrentTint = new Color(0.82f, 0.47f, 0.14f, 1f);
    private static readonly Color RouteDoneTint = new Color(0.48f, 0.58f, 0.35f, 1f);
    private static readonly Color RoutePendingTint = new Color(0.56f, 0.43f, 0.31f, 0.46f);

    private readonly List<LandmarkVisual> _landmarkVisuals = new List<LandmarkVisual>();
    private readonly LandmarkDefinition[] _landmarks =
    {
        new LandmarkDefinition("mine", "矿洞口", new Vector2(92f, 330f), StoryPhase.CrashAndMeet),
        new LandmarkDefinition("gate", "村口", new Vector2(188f, 248f), StoryPhase.EnterVillage),
        new LandmarkDefinition("street", "集市街", new Vector2(314f, 246f), StoryPhase.EnterVillage),
        new LandmarkDefinition("lodge", "临时住处", new Vector2(240f, 120f), StoryPhase.HealingAndHP),
        new LandmarkDefinition("workbench", "工作台", new Vector2(398f, 122f), StoryPhase.WorkbenchFlashback),
        new LandmarkDefinition("field", "农地", new Vector2(558f, 120f), StoryPhase.FarmingTutorial),
        new LandmarkDefinition("canteen", "食堂", new Vector2(544f, 270f), StoryPhase.DinnerConflict),
        new LandmarkDefinition("hillside", "后坡小路", new Vector2(566f, 366f), StoryPhase.FreeTime),
        new LandmarkDefinition("graveyard", "墓园", new Vector2(402f, 382f), StoryPhase.DayEnd)
    };

    private readonly RouteStepDefinition[] _routeSteps =
    {
        new RouteStepDefinition(StoryPhase.EnterVillage, "进村安置", "先把村口、集市街和住处看清，地图从这里开始成形。"),
        new RouteStepDefinition(StoryPhase.HealingAndHP, "疗伤照料", "住处一带是眼下的安全区，照料人与路过村民逐渐被认出来。"),
        new RouteStepDefinition(StoryPhase.WorkbenchFlashback, "工作台回想", "屋边工作台与木匠活动区会成为新的注意点。"),
        new RouteStepDefinition(StoryPhase.FarmingTutorial, "农田熟路", "农地、花草摊和工作台串成白天的实际动线。"),
        new RouteStepDefinition(StoryPhase.DinnerConflict, "晚餐人群", "食堂和集市街会把整条村子的情绪都挤到一起。"),
        new RouteStepDefinition(StoryPhase.FreeTime, "夜里巡看", "回屋小路、后坡与住处周边会在夜里重新被看见。")
    };

    private RectTransform _runtimeRoot;
    private TextMeshProUGUI _phaseChipText;
    private Image _phaseChipImage;
    private TextMeshProUGUI _sceneValueText;
    private TextMeshProUGUI _focusTitleText;
    private TextMeshProUGUI _focusBodyText;
    private TextMeshProUGUI _presenceSummaryText;
    private TextMeshProUGUI _routeCurrentText;
    private TextMeshProUGUI _routeSummaryText;
    private bool _built;

    private sealed class LandmarkDefinition
    {
        public LandmarkDefinition(string id, string label, Vector2 position, StoryPhase unlockPhase)
        {
            Id = id;
            Label = label;
            Position = position;
            UnlockPhase = unlockPhase;
        }

        public string Id { get; }
        public string Label { get; }
        public Vector2 Position { get; }
        public StoryPhase UnlockPhase { get; }
    }

    private sealed class LandmarkVisual
    {
        public LandmarkDefinition Definition;
        public Image Halo;
        public Image Dot;
        public Image LabelBackplate;
        public TextMeshProUGUI Label;
    }

    private sealed class RouteStepDefinition
    {
        public RouteStepDefinition(StoryPhase phase, string title, string detail)
        {
            Phase = phase;
            Title = title;
            Detail = detail;
        }

        public StoryPhase Phase { get; }
        public string Title { get; }
        public string Detail { get; }
    }

    public static void EnsureInstalled(GameObject panelRoot)
    {
        RectTransform main = FindPageMain(panelRoot != null ? panelRoot.transform : null, "3_Map");
        if (main == null)
        {
            return;
        }

        PackageMapOverviewPanel panel = main.GetComponent<PackageMapOverviewPanel>();
        if (panel == null)
        {
            panel = main.gameObject.AddComponent<PackageMapOverviewPanel>();
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
        if (_built && _runtimeRoot != null)
        {
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
        _landmarkVisuals.Clear();
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

    private void BuildRuntimeUi()
    {
        _runtimeRoot = PackagePanelRuntimeUiKit.CreatePanel(RuntimeRootName, transform, PageTint, false, new Color(0.31f, 0.18f, 0.08f, 0.24f), new Vector2(2f, -2f));
        PackagePanelRuntimeUiKit.Stretch(_runtimeRoot, 14f, 14f, 14f, 14f);

        VerticalLayoutGroup shellLayout = _runtimeRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        shellLayout.padding = new RectOffset(18, 18, 18, 18);
        shellLayout.spacing = 12f;
        shellLayout.childAlignment = TextAnchor.UpperLeft;
        shellLayout.childControlWidth = true;
        shellLayout.childControlHeight = true;
        shellLayout.childForceExpandWidth = true;
        shellLayout.childForceExpandHeight = false;

        RectTransform header = PackagePanelRuntimeUiKit.CreatePanel("Header", _runtimeRoot, CardStrongTint, false, new Color(0.35f, 0.20f, 0.09f, 0.20f), new Vector2(2f, -2f));
        AddLayoutElement(header, preferredHeight: 80f);

        HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
        headerLayout.padding = new RectOffset(20, 20, 16, 14);
        headerLayout.spacing = 14f;
        headerLayout.childAlignment = TextAnchor.MiddleLeft;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        headerLayout.childForceExpandHeight = true;

        RectTransform headerTextColumn = PackagePanelRuntimeUiKit.CreateRect("HeaderTextColumn", header);
        AddLayoutElement(headerTextColumn, flexibleWidth: 1f, flexibleHeight: 1f);

        VerticalLayoutGroup headerTextLayout = headerTextColumn.gameObject.AddComponent<VerticalLayoutGroup>();
        headerTextLayout.padding = new RectOffset(0, 0, 0, 0);
        headerTextLayout.spacing = 3f;
        headerTextLayout.childAlignment = TextAnchor.UpperLeft;
        headerTextLayout.childControlWidth = true;
        headerTextLayout.childControlHeight = true;
        headerTextLayout.childForceExpandWidth = true;
        headerTextLayout.childForceExpandHeight = false;

        TextMeshProUGUI title = PackagePanelRuntimeUiKit.CreateText("Title", headerTextColumn, "春日村地图", 28f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)title.transform, preferredHeight: 34f);

        TextMeshProUGUI subtitle = PackagePanelRuntimeUiKit.CreateText(
            "Subtitle",
            headerTextColumn,
            "把今天真正走熟的地方记下来。",
            14f,
            SubtleTint,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            false,
            TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)subtitle.transform, preferredHeight: 24f);

        RectTransform phaseChip = PackagePanelRuntimeUiKit.CreatePanel("PhaseChip", header, new Color(0.91f, 0.73f, 0.33f, 0.98f), false, new Color(0.33f, 0.20f, 0.09f, 0.24f));
        AddLayoutElement(phaseChip, preferredWidth: 156f, preferredHeight: 42f);
        _phaseChipImage = phaseChip.GetComponent<Image>();

        _phaseChipText = PackagePanelRuntimeUiKit.CreateText("PhaseChipText", phaseChip, string.Empty, 16f, InkTint, FontStyles.Bold, TextAlignmentOptions.Center);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)_phaseChipText.transform, 12f, 12f, 6f, 6f);

        RectTransform content = PackagePanelRuntimeUiKit.CreateRect("Content", _runtimeRoot);
        AddLayoutElement(content, minHeight: 348f, flexibleHeight: 1f);

        HorizontalLayoutGroup contentLayout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 0, 0);
        contentLayout.spacing = 12f;
        contentLayout.childAlignment = TextAnchor.UpperLeft;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = false;
        contentLayout.childForceExpandHeight = true;

        BuildBoard(content);
        BuildSideColumn(content);
    }

    private void BuildBoard(RectTransform content)
    {
        RectTransform boardCard = PackagePanelRuntimeUiKit.CreatePanel("BoardCard", content, CardTint, false, new Color(0.35f, 0.22f, 0.12f, 0.20f), new Vector2(2f, -2f));
        AddLayoutElement(boardCard, minWidth: 438f, flexibleWidth: 1.18f, flexibleHeight: 1f);

        VerticalLayoutGroup boardLayout = boardCard.gameObject.AddComponent<VerticalLayoutGroup>();
        boardLayout.padding = new RectOffset(16, 16, 16, 14);
        boardLayout.spacing = 10f;
        boardLayout.childAlignment = TextAnchor.UpperLeft;
        boardLayout.childControlWidth = true;
        boardLayout.childControlHeight = true;
        boardLayout.childForceExpandWidth = true;
        boardLayout.childForceExpandHeight = false;

        RectTransform boardHeader = PackagePanelRuntimeUiKit.CreateRect("BoardHeader", boardCard);
        AddLayoutElement(boardHeader, preferredHeight: 28f);
        HorizontalLayoutGroup boardHeaderLayout = boardHeader.gameObject.AddComponent<HorizontalLayoutGroup>();
        boardHeaderLayout.padding = new RectOffset(0, 0, 0, 0);
        boardHeaderLayout.spacing = 10f;
        boardHeaderLayout.childAlignment = TextAnchor.MiddleLeft;
        boardHeaderLayout.childControlWidth = true;
        boardHeaderLayout.childControlHeight = true;
        boardHeaderLayout.childForceExpandWidth = false;
        boardHeaderLayout.childForceExpandHeight = true;

        TextMeshProUGUI boardTitle = PackagePanelRuntimeUiKit.CreateText("BoardTitle", boardHeader, "今日路线图", 19f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)boardTitle.transform, flexibleWidth: 1f, preferredHeight: 28f);

        RectTransform boardNoteChip = PackagePanelRuntimeUiKit.CreatePanel("BoardNoteChip", boardHeader, new Color(0.98f, 0.91f, 0.78f, 0.96f), false, new Color(0.35f, 0.22f, 0.12f, 0.12f));
        AddLayoutElement(boardNoteChip, minWidth: 154f, preferredWidth: 168f, preferredHeight: 24f);
        HorizontalLayoutGroup boardNoteLayout = boardNoteChip.gameObject.AddComponent<HorizontalLayoutGroup>();
        boardNoteLayout.padding = new RectOffset(8, 8, 2, 2);
        boardNoteLayout.spacing = 0f;
        boardNoteLayout.childAlignment = TextAnchor.MiddleCenter;
        boardNoteLayout.childControlWidth = true;
        boardNoteLayout.childControlHeight = true;
        boardNoteLayout.childForceExpandWidth = true;
        boardNoteLayout.childForceExpandHeight = true;
        TextMeshProUGUI boardNoteText = PackagePanelRuntimeUiKit.CreateText("BoardNoteText", boardNoteChip, "只记今天走熟的地方", 11.5f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
        AddLayoutElement((RectTransform)boardNoteText.transform, flexibleWidth: 1f, flexibleHeight: 1f);

        RectTransform boardSurface = PackagePanelRuntimeUiKit.CreatePanel("BoardSurface", boardCard, BoardTint, false, new Color(0.36f, 0.22f, 0.12f, 0.18f), new Vector2(2f, -2f));
        AddLayoutElement(boardSurface, minHeight: 314f, flexibleHeight: 1f);

        CreateZoneBlock(boardSurface, "MineZone", new Vector2(34f, 274f), new Vector2(164f, 394f), MineZoneTint, "矿坡");
        CreateZoneBlock(boardSurface, "GateZone", new Vector2(140f, 224f), new Vector2(242f, 292f), VillageZoneStrongTint, "村口");
        CreateZoneBlock(boardSurface, "StreetZone", new Vector2(214f, 214f), new Vector2(430f, 302f), VillageZoneStrongTint, "街巷");
        CreateZoneBlock(boardSurface, "HomeZone", new Vector2(176f, 82f), new Vector2(326f, 178f), HomeZoneTint, "住处");
        CreateZoneBlock(boardSurface, "WorkbenchZone", new Vector2(336f, 84f), new Vector2(470f, 176f), WorkZoneTint, "工位");
        CreateZoneBlock(boardSurface, "FieldZone", new Vector2(486f, 72f), new Vector2(646f, 172f), FieldZoneTint, "农地");
        CreateZoneBlock(boardSurface, "DinnerZone", new Vector2(470f, 218f), new Vector2(640f, 314f), DinnerZoneTint, "食堂");
        CreateZoneBlock(boardSurface, "NightZone", new Vector2(438f, 308f), new Vector2(652f, 406f), NightZoneTint, "后坡");

        CreateRoad(boardSurface, new Vector2(144f, 298f), new Vector2(226f, 258f), 8f);
        CreateRoad(boardSurface, new Vector2(228f, 252f), new Vector2(308f, 250f), 8f);
        CreateRoad(boardSurface, new Vector2(308f, 250f), new Vector2(404f, 250f), 8f);
        CreateRoad(boardSurface, new Vector2(402f, 250f), new Vector2(520f, 270f), 8f);
        CreateRoad(boardSurface, new Vector2(318f, 232f), new Vector2(402f, 144f), 10f);
        CreateRoad(boardSurface, new Vector2(404f, 142f), new Vector2(544f, 126f), 10f);
        CreateRoad(boardSurface, new Vector2(520f, 270f), new Vector2(546f, 352f), 8f);
        CreateRoad(boardSurface, new Vector2(404f, 250f), new Vector2(404f, 368f), 8f);

        for (int index = 0; index < _landmarks.Length; index++)
        {
            LandmarkDefinition definition = _landmarks[index];
            RectTransform node = PackagePanelRuntimeUiKit.CreateRect(definition.Id, boardSurface);
            node.anchorMin = Vector2.zero;
            node.anchorMax = Vector2.zero;
            node.pivot = new Vector2(0.5f, 0.5f);
            node.sizeDelta = new Vector2(148f, 38f);
            node.anchoredPosition = definition.Position;

            RectTransform halo = PackagePanelRuntimeUiKit.CreatePanel("Halo", node, new Color(1f, 1f, 1f, 0f), false, new Color(0f, 0f, 0f, 0f));
            PackagePanelRuntimeUiKit.SetAnchors(halo, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-4f, -14f), new Vector2(24f, 14f), new Vector2(0f, 0.5f));

            RectTransform dot = PackagePanelRuntimeUiKit.CreatePanel("Dot", node, UnlockedTint, false, new Color(0f, 0f, 0f, 0f));
            PackagePanelRuntimeUiKit.SetAnchors(dot, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(2f, -9f), new Vector2(18f, 9f), new Vector2(0f, 0.5f));

            RectTransform labelBackplate = PackagePanelRuntimeUiKit.CreatePanel("LabelBackplate", node, new Color(0.99f, 0.94f, 0.86f, 0.72f), false, new Color(0.35f, 0.22f, 0.12f, 0.18f));
            PackagePanelRuntimeUiKit.SetAnchors(labelBackplate, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(24f, -14f), new Vector2(0f, 14f), new Vector2(0f, 0.5f));

            TextMeshProUGUI label = PackagePanelRuntimeUiKit.CreateText("Label", labelBackplate, definition.Label, 13.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.Center);
            PackagePanelRuntimeUiKit.Stretch((RectTransform)label.transform, 8f, 8f, 3f, 3f);

            _landmarkVisuals.Add(new LandmarkVisual
            {
                Definition = definition,
                Halo = halo.GetComponent<Image>(),
                Dot = dot.GetComponent<Image>(),
                LabelBackplate = labelBackplate.GetComponent<Image>(),
                Label = label
            });
        }
    }

    private void BuildSideColumn(RectTransform content)
    {
        RectTransform sideColumn = PackagePanelRuntimeUiKit.CreateRect("SideColumn", content);
        AddLayoutElement(sideColumn, minWidth: 264f, preferredWidth: 280f, flexibleWidth: 0.54f, flexibleHeight: 1f);

        VerticalLayoutGroup sideLayout = sideColumn.gameObject.AddComponent<VerticalLayoutGroup>();
        sideLayout.padding = new RectOffset(0, 0, 0, 0);
        sideLayout.spacing = 10f;
        sideLayout.childAlignment = TextAnchor.UpperLeft;
        sideLayout.childControlWidth = true;
        sideLayout.childControlHeight = true;
        sideLayout.childForceExpandWidth = true;
        sideLayout.childForceExpandHeight = false;

        RectTransform overviewCard = PackagePanelRuntimeUiKit.CreatePanel("OverviewCard", sideColumn, HighlightCardTint, false, new Color(0.36f, 0.22f, 0.12f, 0.18f), new Vector2(2f, -2f));
        AddLayoutElement(overviewCard, minHeight: 126f, preferredHeight: 132f);
        VerticalLayoutGroup overviewLayout = overviewCard.gameObject.AddComponent<VerticalLayoutGroup>();
        overviewLayout.padding = new RectOffset(14, 14, 14, 14);
        overviewLayout.spacing = 5f;
        overviewLayout.childAlignment = TextAnchor.UpperLeft;
        overviewLayout.childControlWidth = true;
        overviewLayout.childControlHeight = true;
        overviewLayout.childForceExpandWidth = true;
        overviewLayout.childForceExpandHeight = false;
        TextMeshProUGUI sceneLabel = PackagePanelRuntimeUiKit.CreateText("SceneLabel", overviewCard, "你站在", 14f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)sceneLabel.transform, preferredHeight: 20f);
        _sceneValueText = PackagePanelRuntimeUiKit.CreateText("SceneValue", overviewCard, string.Empty, 15f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)_sceneValueText.transform, preferredHeight: 24f);
        _focusTitleText = PackagePanelRuntimeUiKit.CreateText("FocusTitle", overviewCard, string.Empty, 22f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)_focusTitleText.transform, minHeight: 28f, preferredHeight: 32f);
        _focusBodyText = PackagePanelRuntimeUiKit.CreateText("FocusBody", overviewCard, string.Empty, 13.5f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _focusBodyText.lineSpacing = 4f;
        AddLayoutElement((RectTransform)_focusBodyText.transform, minHeight: 40f, flexibleHeight: 1f);

        RectTransform presenceCard = PackagePanelRuntimeUiKit.CreatePanel("PresenceCard", sideColumn, CardTint, false, new Color(0.36f, 0.22f, 0.12f, 0.18f), new Vector2(2f, -2f));
        AddLayoutElement(presenceCard, minHeight: 98f, preferredHeight: 104f);
        VerticalLayoutGroup presenceLayout = presenceCard.gameObject.AddComponent<VerticalLayoutGroup>();
        presenceLayout.padding = new RectOffset(14, 14, 14, 14);
        presenceLayout.spacing = 6f;
        presenceLayout.childAlignment = TextAnchor.UpperLeft;
        presenceLayout.childControlWidth = true;
        presenceLayout.childControlHeight = true;
        presenceLayout.childForceExpandWidth = true;
        presenceLayout.childForceExpandHeight = false;
        TextMeshProUGUI presenceLabel = PackagePanelRuntimeUiKit.CreateText("PresenceLabel", presenceCard, "这会儿最容易遇见", 15f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)presenceLabel.transform, preferredHeight: 20f);
        _presenceSummaryText = PackagePanelRuntimeUiKit.CreateText("PresenceSummary", presenceCard, string.Empty, 13.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _presenceSummaryText.lineSpacing = 4f;
        AddLayoutElement((RectTransform)_presenceSummaryText.transform, minHeight: 46f, flexibleHeight: 1f);

        RectTransform routeCard = PackagePanelRuntimeUiKit.CreatePanel("RouteCard", sideColumn, CardTint, false, new Color(0.36f, 0.22f, 0.12f, 0.18f), new Vector2(2f, -2f));
        AddLayoutElement(routeCard, minHeight: 176f, flexibleHeight: 1f);
        VerticalLayoutGroup routeLayout = routeCard.gameObject.AddComponent<VerticalLayoutGroup>();
        routeLayout.padding = new RectOffset(14, 14, 14, 14);
        routeLayout.spacing = 8f;
        routeLayout.childAlignment = TextAnchor.UpperLeft;
        routeLayout.childControlWidth = true;
        routeLayout.childControlHeight = true;
        routeLayout.childForceExpandWidth = true;
        routeLayout.childForceExpandHeight = false;
        TextMeshProUGUI routeLabel = PackagePanelRuntimeUiKit.CreateText("RouteLabel", routeCard, "今天的脚步", 16f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)routeLabel.transform, preferredHeight: 20f);

        RectTransform routeCurrentCard = PackagePanelRuntimeUiKit.CreatePanel("RouteCurrentCard", routeCard, new Color(1f, 1f, 1f, 0.48f), false, new Color(0.35f, 0.22f, 0.12f, 0.14f));
        AddLayoutElement(routeCurrentCard, minHeight: 72f, preferredHeight: 76f);
        VerticalLayoutGroup routeCurrentLayout = routeCurrentCard.gameObject.AddComponent<VerticalLayoutGroup>();
        routeCurrentLayout.padding = new RectOffset(12, 12, 10, 10);
        routeCurrentLayout.spacing = 4f;
        routeCurrentLayout.childAlignment = TextAnchor.UpperLeft;
        routeCurrentLayout.childControlWidth = true;
        routeCurrentLayout.childControlHeight = true;
        routeCurrentLayout.childForceExpandWidth = true;
        routeCurrentLayout.childForceExpandHeight = false;
        TextMeshProUGUI routeCurrentLabel = PackagePanelRuntimeUiKit.CreateText("RouteCurrentLabel", routeCurrentCard, "眼下这段", 13f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)routeCurrentLabel.transform, preferredHeight: 18f);
        _routeCurrentText = PackagePanelRuntimeUiKit.CreateText("RouteCurrentText", routeCurrentCard, string.Empty, 17f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left, true);
        AddLayoutElement((RectTransform)_routeCurrentText.transform, minHeight: 26f, flexibleHeight: 1f);

        RectTransform routeSummaryCard = PackagePanelRuntimeUiKit.CreatePanel("RouteSummaryCard", routeCard, new Color(1f, 1f, 1f, 0.34f), false, new Color(0.35f, 0.22f, 0.12f, 0.12f));
        AddLayoutElement(routeSummaryCard, minHeight: 98f, flexibleHeight: 1f);
        VerticalLayoutGroup routeSummaryLayout = routeSummaryCard.gameObject.AddComponent<VerticalLayoutGroup>();
        routeSummaryLayout.padding = new RectOffset(12, 12, 10, 10);
        routeSummaryLayout.spacing = 6f;
        routeSummaryLayout.childAlignment = TextAnchor.UpperLeft;
        routeSummaryLayout.childControlWidth = true;
        routeSummaryLayout.childControlHeight = true;
        routeSummaryLayout.childForceExpandWidth = true;
        routeSummaryLayout.childForceExpandHeight = false;
        _routeSummaryText = PackagePanelRuntimeUiKit.CreateText("RouteSummary", routeSummaryCard, string.Empty, 13.5f, InkTint, FontStyles.Bold, TextAlignmentOptions.TopLeft, true);
        _routeSummaryText.lineSpacing = 4f;
        AddLayoutElement((RectTransform)_routeSummaryText.transform, minHeight: 52f, flexibleHeight: 1f);
        TextMeshProUGUI routeHint = PackagePanelRuntimeUiKit.CreateText("RouteHint", routeSummaryCard, "这一页只记今天真的走过的路。", 12f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)routeHint.transform, preferredHeight: 18f);
    }

    private void CreateRoad(RectTransform boardSurface, Vector2 start, Vector2 end, float thickness)
    {
        RectTransform road = PackagePanelRuntimeUiKit.CreatePanel("Road", boardSurface, RoadTint, false, new Color(0f, 0f, 0f, 0f));
        road.pivot = new Vector2(0.5f, 0.5f);
        road.anchorMin = Vector2.zero;
        road.anchorMax = Vector2.zero;
        road.sizeDelta = new Vector2(Vector2.Distance(start, end), thickness);
        road.anchoredPosition = (start + end) * 0.5f;
        Vector2 direction = end - start;
        road.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private static void CreateZoneBlock(RectTransform parent, string name, Vector2 min, Vector2 max, Color color, string label)
    {
        RectTransform zone = PackagePanelRuntimeUiKit.CreatePanel(name, parent, color, false, new Color(0.34f, 0.21f, 0.11f, 0.10f));
        PackagePanelRuntimeUiKit.SetAnchors(zone, new Vector2(0f, 0f), new Vector2(0f, 0f), min, max, new Vector2(0f, 0f));

        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }

        RectTransform plate = PackagePanelRuntimeUiKit.CreatePanel("LabelPlate", zone, new Color(1f, 1f, 1f, 0.18f), false, new Color(0f, 0f, 0f, 0f));
        PackagePanelRuntimeUiKit.SetAnchors(plate, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(10f, -28f), new Vector2(78f, -8f), new Vector2(0f, 1f));

        TextMeshProUGUI text = PackagePanelRuntimeUiKit.CreateText("Label", plate, label, 11f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)text.transform, 6f, 6f, 2f, 2f);
    }

    private static void CreateLegendPill(Transform parent, string name, string label, Color dotColor)
    {
        RectTransform pill = PackagePanelRuntimeUiKit.CreatePanel(name, parent, new Color(1f, 1f, 1f, 0.52f), false, new Color(0.35f, 0.22f, 0.12f, 0.16f));
        AddLayoutElement(pill, preferredWidth: 78f, preferredHeight: 24f);
        HorizontalLayoutGroup pillLayout = pill.gameObject.AddComponent<HorizontalLayoutGroup>();
        pillLayout.padding = new RectOffset(10, 8, 4, 4);
        pillLayout.spacing = 6f;
        pillLayout.childAlignment = TextAnchor.MiddleLeft;
        pillLayout.childControlWidth = false;
        pillLayout.childControlHeight = true;
        pillLayout.childForceExpandWidth = false;
        pillLayout.childForceExpandHeight = false;

        RectTransform dot = PackagePanelRuntimeUiKit.CreatePanel("Dot", pill, dotColor, false, new Color(0f, 0f, 0f, 0f));
        AddLayoutElement(dot, preferredWidth: 12f, preferredHeight: 12f);

        TextMeshProUGUI text = PackagePanelRuntimeUiKit.CreateText("Label", pill, label, 12f, InkTint, FontStyles.Bold, TextAlignmentOptions.Left);
        AddLayoutElement((RectTransform)text.transform, preferredHeight: 18f);
    }

    private static void CreateRegionLabel(RectTransform parent, string textValue, Vector2 anchoredPosition, Vector2 size)
    {
        RectTransform plate = PackagePanelRuntimeUiKit.CreatePanel("RegionLabel", parent, new Color(0.99f, 0.93f, 0.84f, 0.78f), false, new Color(0.32f, 0.20f, 0.11f, 0.10f));
        plate.anchorMin = new Vector2(0f, 1f);
        plate.anchorMax = new Vector2(0f, 1f);
        plate.pivot = new Vector2(0f, 1f);
        plate.anchoredPosition = anchoredPosition;
        plate.sizeDelta = size;

        TextMeshProUGUI text = PackagePanelRuntimeUiKit.CreateText("Label", plate, textValue, 12f, SubtleTint, FontStyles.Bold, TextAlignmentOptions.Center, false, TextOverflowModes.Ellipsis);
        PackagePanelRuntimeUiKit.Stretch((RectTransform)text.transform, 6f, 6f, 2f, 2f);
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

    private void RefreshView()
    {
        StoryPhase phase = ResolveCurrentPhase();
        PhaseMapInfo phaseInfo = ResolvePhaseInfo(phase);

        _sceneValueText.text = ResolveSceneLabel();
        _phaseChipText.text = phaseInfo.PhaseLabel;
        _phaseChipImage.color = phaseInfo.ChipColor;
        _focusTitleText.text = phaseInfo.FocusTitle;
        _focusBodyText.text = phaseInfo.FocusBody;
        _presenceSummaryText.text = BuildPresenceSummary(phaseInfo.BeatKey);
        _routeCurrentText.text = BuildCurrentRouteTitle(phase);
        _routeSummaryText.text = BuildRouteSummary(phase);

        for (int index = 0; index < _landmarkVisuals.Count; index++)
        {
            LandmarkVisual visual = _landmarkVisuals[index];
            bool unlocked = phase >= visual.Definition.UnlockPhase;
            bool isActive = unlocked && string.Equals(visual.Definition.Id, phaseInfo.ActiveLandmarkId, System.StringComparison.OrdinalIgnoreCase);

            visual.Dot.color = isActive ? ActiveTint : unlocked ? UnlockedTint : LockedTint;
            visual.Halo.color = isActive
                ? new Color(0.96f, 0.79f, 0.42f, 0.46f)
                : unlocked
                    ? new Color(0.86f, 0.73f, 0.54f, 0.18f)
                    : new Color(1f, 1f, 1f, 0f);
            visual.LabelBackplate.color = isActive
                ? new Color(0.98f, 0.90f, 0.76f, 0.98f)
                : unlocked
                    ? new Color(1f, 1f, 1f, 0.44f)
                    : new Color(0.90f, 0.90f, 0.90f, 0.14f);
            visual.Label.color = unlocked ? InkTint : new Color(0.38f, 0.33f, 0.28f, 0.64f);
        }
    }

    private static string BuildPresenceSummary(string beatKey)
    {
        SpringDay1NpcCrowdManifest manifest = Resources.Load<SpringDay1NpcCrowdManifest>("Story/SpringDay1/SpringDay1NpcCrowdManifest");
        if (manifest == null)
        {
            return "先把脚下这条路认熟，人会慢慢和地点对上。";
        }

        List<string> lead = new List<string>();
        List<string> support = new List<string>();
        string pressure = JoinDisplayNamesByPresence(manifest, beatKey, SpringDay1CrowdResidentPresenceLevel.Pressure);
        string visible = JoinDisplayNamesByPresence(manifest, beatKey, SpringDay1CrowdResidentPresenceLevel.Visible);
        string background = JoinDisplayNamesByPresence(manifest, beatKey, SpringDay1CrowdResidentPresenceLevel.Background);
        string trace = JoinDisplayNamesByPresence(manifest, beatKey, SpringDay1CrowdResidentPresenceLevel.Trace);

        if (!string.IsNullOrWhiteSpace(pressure))
        {
            lead.AddRange(pressure.Split('、'));
        }

        if (!string.IsNullOrWhiteSpace(visible))
        {
            lead.AddRange(visible.Split('、'));
        }

        if (!string.IsNullOrWhiteSpace(background))
        {
            support.AddRange(background.Split('、'));
        }

        if (!string.IsNullOrWhiteSpace(trace))
        {
            support.AddRange(trace.Split('、'));
        }

        string leadLine = lead.Count == 0 ? string.Empty : "眼下会先碰见 " + TrimNames(lead, 3) + "。";
        string supportLine = support.Count == 0 ? string.Empty : "远一点还能看见 " + TrimNames(support, 3) + "。";

        if (string.IsNullOrWhiteSpace(leadLine) && string.IsNullOrWhiteSpace(supportLine))
        {
            return "先把路认熟，人会慢慢和地点对上。";
        }

        if (string.IsNullOrWhiteSpace(leadLine))
        {
            return supportLine;
        }

        if (string.IsNullOrWhiteSpace(supportLine))
        {
            return leadLine;
        }

        return leadLine + "\n" + supportLine;
    }

    private static string BuildCurrentRouteTitle(StoryPhase phase)
    {
        int currentIndex = ResolveCurrentRouteIndex(phase);
        if (currentIndex < 0)
        {
            return "先认出离开矿洞的那条路";
        }

        return _routeStepsStatic[currentIndex].Title;
    }

    private static string BuildRouteSummary(StoryPhase phase)
    {
        int currentIndex = ResolveCurrentRouteIndex(phase);
        if (currentIndex < 0)
        {
            return "走熟的地方：还没有\n下一步：先把村口和住处连起来";
        }

        List<string> completed = new List<string>();
        for (int index = 0; index < currentIndex; index++)
        {
            completed.Add(_routeStepsStatic[index].Title);
        }

        string completedText = completed.Count == 0 ? "还没有真正走熟的段落" : TrimNames(completed, 2);
        string nextText = currentIndex >= _routeStepsStatic.Length - 1
            ? "今晚先把路线收回住处"
            : _routeStepsStatic[currentIndex + 1].Title;

        return
            $"走熟的地方：{completedText}\n" +
            $"眼下这段：{_routeStepsStatic[currentIndex].Title} · {_routeStepsStatic[currentIndex].Detail}\n" +
            $"接下来：{nextText}";
    }

    private static int ResolveCurrentRouteIndex(StoryPhase phase)
    {
        for (int index = _routeStepsStatic.Length - 1; index >= 0; index--)
        {
            if (phase >= _routeStepsStatic[index].Phase)
            {
                return index;
            }
        }

        return -1;
    }

    private static string TrimNames(IReadOnlyList<string> names, int maxCount)
    {
        if (names == null || names.Count == 0)
        {
            return string.Empty;
        }

        List<string> trimmed = new List<string>();
        for (int index = 0; index < names.Count && trimmed.Count < maxCount; index++)
        {
            string name = names[index]?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            trimmed.Add(name);
        }

        if (trimmed.Count == 0)
        {
            return string.Empty;
        }

        return names.Count > trimmed.Count
            ? string.Join("、", trimmed) + " 等"
            : string.Join("、", trimmed);
    }

    private static string JoinDisplayNamesByPresence(
        SpringDay1NpcCrowdManifest manifest,
        string beatKey,
        SpringDay1CrowdResidentPresenceLevel targetLevel)
    {
        if (manifest == null || string.IsNullOrWhiteSpace(beatKey))
        {
            return string.Empty;
        }

        List<string> names = new List<string>();
        SpringDay1NpcCrowdManifest.Entry[] entries = manifest.Entries;
        for (int index = 0; index < entries.Length; index++)
        {
            SpringDay1NpcCrowdManifest.Entry entry = entries[index];
            if (entry == null
                || !entry.TryGetResidentBeatSemantic(beatKey, out SpringDay1NpcCrowdManifest.ResidentBeatSemantic semantic)
                || semantic == null
                || semantic.presenceLevel != targetLevel
                || string.IsNullOrWhiteSpace(entry.displayName))
            {
                continue;
            }

            names.Add(entry.displayName.Trim());
        }

        return names.Count == 0 ? string.Empty : string.Join("、", names);
    }

    private static StoryPhase ResolveCurrentPhase()
    {
        StoryManager storyManager = StoryManager.Instance;
        return storyManager != null && storyManager.CurrentPhase != StoryPhase.None
            ? storyManager.CurrentPhase
            : StoryPhase.CrashAndMeet;
    }

    private static string ResolveSceneLabel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (string.Equals(sceneName, "Primary", System.StringComparison.OrdinalIgnoreCase))
        {
            return "春日村";
        }

        return string.IsNullOrWhiteSpace(sceneName) ? "今日活动区" : sceneName;
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
            if (!string.Equals(child.name, pageName, System.StringComparison.Ordinal))
            {
                continue;
            }

            return child.Find("Main") as RectTransform;
        }

        return null;
    }

    private readonly struct PhaseMapInfo
    {
        public PhaseMapInfo(string phaseLabel, string focusTitle, string focusBody, string activeLandmarkId, string beatKey, Color chipColor)
        {
            PhaseLabel = phaseLabel;
            FocusTitle = focusTitle;
            FocusBody = focusBody;
            ActiveLandmarkId = activeLandmarkId;
            BeatKey = beatKey;
            ChipColor = chipColor;
        }

        public string PhaseLabel { get; }
        public string FocusTitle { get; }
        public string FocusBody { get; }
        public string ActiveLandmarkId { get; }
        public string BeatKey { get; }
        public Color ChipColor { get; }
    }

    private static PhaseMapInfo ResolvePhaseInfo(StoryPhase phase)
    {
        return phase switch
        {
            StoryPhase.CrashAndMeet => new PhaseMapInfo("坠落与相遇", "先认出离开矿洞的路", "矿洞口到村口，是今天第一条该记住的安全线。", "mine", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.85f, 0.54f, 0.24f, 0.96f)),
            StoryPhase.EnterVillage => new PhaseMapInfo("进入村口", "村口开始有名字了", "先把村口、街巷和住处连成一张能认路的图。", "gate", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.83f, 0.61f, 0.22f, 0.96f)),
            StoryPhase.HealingAndHP => new PhaseMapInfo("疗伤教学", "住处是眼下的缓冲区", "这一段先把住处周边认熟，别再走丢。", "lodge", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.73f, 0.58f, 0.28f, 0.96f)),
            StoryPhase.WorkbenchFlashback => new PhaseMapInfo("工作台回想", "工作台也进图了", "工作台会把住处和白天做活的路线接起来。", "workbench", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.76f, 0.52f, 0.18f, 0.96f)),
            StoryPhase.FarmingTutorial => new PhaseMapInfo("农田教学", "农地已经成了白天主路", "白天最常走的，就是去农地和工作台这一段。", "field", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.63f, 0.60f, 0.23f, 0.96f)),
            StoryPhase.DinnerConflict => new PhaseMapInfo("晚餐冲突", "晚上的人群都往食堂挤", "到晚餐时，整条村子的情绪都会压到食堂附近。", "canteen", SpringDay1CrowdResidentBeatKeys.DinnerConflictTable, new Color(0.88f, 0.48f, 0.20f, 0.96f)),
            StoryPhase.ReturnAndReminder => new PhaseMapInfo("返程提醒", "先把回屋这条路记住", "冲突之后，更重要的是知道自己怎么平安走回去。", "gate", SpringDay1CrowdResidentBeatKeys.ReturnAndReminderWalkBack, new Color(0.70f, 0.55f, 0.29f, 0.96f)),
            StoryPhase.FreeTime => new PhaseMapInfo("自由行动", "夜里会看见白天没注意到的边路", "后坡和边路会在夜里重新显出来。", "hillside", SpringDay1CrowdResidentBeatKeys.FreeTimeNightWitness, new Color(0.67f, 0.56f, 0.35f, 0.96f)),
            StoryPhase.DayEnd => new PhaseMapInfo("第一夜收束", "路线开始往住处收拢", "这会儿该记住的，是怎么把脚步安稳地收回去。", "graveyard", SpringDay1CrowdResidentBeatKeys.DayEndSettle, new Color(0.60f, 0.50f, 0.38f, 0.96f)),
            _ => new PhaseMapInfo("春日村", "先把村子的骨架认熟", "地图先把今天最该记住的地点摆出来。", "gate", SpringDay1CrowdResidentBeatKeys.EnterVillagePostEntry, new Color(0.83f, 0.61f, 0.22f, 0.96f))
        };
    }

    private static readonly RouteStepDefinition[] _routeStepsStatic =
    {
        new RouteStepDefinition(StoryPhase.EnterVillage, "进村安置", "先把村口、街巷和住处连起来。"),
        new RouteStepDefinition(StoryPhase.HealingAndHP, "疗伤照料", "住处一带是眼下的安全区。"),
        new RouteStepDefinition(StoryPhase.WorkbenchFlashback, "工作台回想", "工作台开始把住处和白天接起来。"),
        new RouteStepDefinition(StoryPhase.FarmingTutorial, "农田熟路", "农地、花草摊和工作台串成白天主路。"),
        new RouteStepDefinition(StoryPhase.DinnerConflict, "晚餐人群", "食堂和街口会把人群情绪挤在一起。"),
        new RouteStepDefinition(StoryPhase.FreeTime, "夜里巡看", "回屋小路和后坡会在夜里重新被看见。")
    };
}
