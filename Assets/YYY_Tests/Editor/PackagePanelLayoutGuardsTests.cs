using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PackagePanelLayoutGuardsTests
{
    private readonly System.Collections.Generic.List<GameObject> _created = new();

    [TearDown]
    public void TearDown()
    {
        for (int index = _created.Count - 1; index >= 0; index--)
        {
            if (_created[index] != null)
            {
                UnityEngine.Object.DestroyImmediate(_created[index]);
            }
        }

        _created.Clear();
        Type promptOverlayType = ResolveType("Sunset.Story.SpringDay1PromptOverlay");
        if (promptOverlayType != null)
        {
            SetStaticField(promptOverlayType, "_instance", null);
        }
    }

    [Test]
    public void PackageMapOverviewPanel_ShouldBuildStructuredSidebarInsteadOfFreeFloatingCards()
    {
        GameObject panelRoot = CreatePackagePanelRoot();
        Transform pageMain = CreatePage(panelRoot.transform, "3_Map");
        CreateChild(pageMain, "LegacyNoise", typeof(RectTransform));

        InvokeStatic(ResolveTypeOrFail("PackageMapOverviewPanel"), "EnsureInstalled", panelRoot);

        Transform runtimeRoot = panelRoot.transform.Find("3_Map/Main/MapOverviewRuntimeRoot");
        Assert.That(runtimeRoot, Is.Not.Null, "地图页应生成运行时根节点。");
        Assert.That(runtimeRoot.GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "地图页根节点应改为纵向壳体布局。");
        Assert.That(pageMain.Find("LegacyNoise").gameObject.activeSelf, Is.False, "地图页 legacy 子节点应被压下，避免旧壳和新壳叠在一起。");

        Transform content = runtimeRoot.Find("Content");
        Assert.That(content, Is.Not.Null, "地图页应保留正文容器。");
        Assert.That(content.GetComponent<HorizontalLayoutGroup>(), Is.Not.Null, "地图页正文应是左右分栏布局。");

        Transform sideColumn = content.Find("SideColumn");
        Assert.That(sideColumn, Is.Not.Null, "地图页右栏应存在。");
        Assert.That(sideColumn.GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "地图页右栏应改为稳定的纵向分区，不再用自由悬挂块。");

        Transform boardCard = content.Find("BoardCard");
        Assert.That(boardCard, Is.Not.Null, "地图页主图卡应存在。");
        Assert.That(boardCard.GetComponent<LayoutElement>(), Is.Not.Null, "地图页主图卡应带布局约束，防止子元素顶爆父容器。");

        Assert.That(sideColumn.Find("OverviewCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "地图页焦点卡应走纵向布局，不再靠绝对锚点硬撑。");
        Assert.That(sideColumn.Find("PresenceCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "地图页出场卡应走纵向布局。");
        Assert.That(sideColumn.Find("RouteCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "地图页路径卡应走纵向布局。");
    }

    [Test]
    public void PackageNpcRelationshipPanel_ShouldUseScrollableDetailColumnAndStackedNarrativeCards()
    {
        GameObject panelRoot = CreatePackagePanelRoot();
        Transform pageMain = CreatePage(panelRoot.transform, "4_Relationship_NPC");
        CreateChild(pageMain, "LegacyNpcNoise", typeof(RectTransform));

        InvokeStatic(ResolveTypeOrFail("PackageNpcRelationshipPanel"), "EnsureInstalled", panelRoot);

        Transform runtimeRoot = panelRoot.transform.Find("4_Relationship_NPC/Main/NpcRelationshipRuntimeRoot");
        Assert.That(runtimeRoot, Is.Not.Null, "关系页应生成运行时根节点。");
        Assert.That(runtimeRoot.GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "关系页根节点应改为纵向壳体布局。");
        Assert.That(pageMain.Find("LegacyNpcNoise").gameObject.activeSelf, Is.False, "关系页 legacy 子节点应被压下，避免旧内容和新壳叠在一起。");

        Transform detailPanel = runtimeRoot.Find("Content/DetailPanel");
        Assert.That(detailPanel, Is.Not.Null, "关系页详情栏应存在。");

        Transform detailScrollRoot = detailPanel.Find("DetailScrollRoot");
        Assert.That(detailScrollRoot, Is.Not.Null, "关系页详情栏应有独立滚动根，避免内容互相挤压。");
        Assert.That(detailScrollRoot.GetComponent<ScrollRect>(), Is.Not.Null, "关系页详情栏应改为滚动容器。");
        Assert.That(detailScrollRoot.Find("Viewport"), Is.Not.Null, "关系页详情栏应有 viewport。");
        Assert.That(detailScrollRoot.Find("Viewport").GetComponent<RectMask2D>(), Is.Not.Null, "关系页详情栏 viewport 应带遮罩。");

        Transform detailRow = detailScrollRoot.Find("Viewport/DetailContent/DetailRow");
        Assert.That(detailRow, Is.Not.Null, "关系页人物叙事区应存在。");
        Assert.That(detailRow.GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "关系页人物叙事区应改为上下堆叠，避免横向挤压重叠。");

        Assert.That(detailScrollRoot.Find("Viewport/DetailContent/HeroCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "关系页头图卡应改为纵向容器，避免头像、身份、备注彼此抢锚点。");
        Assert.That(detailScrollRoot.Find("Viewport/DetailContent/StageCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "关系页阶段卡应走稳定容器布局。");
        Assert.That(detailScrollRoot.Find("Viewport/DetailContent/FooterCard").GetComponent<VerticalLayoutGroup>(), Is.Not.Null, "关系页底部提示卡也应留在父容器里，不再单独漂出去。");
    }

    [Test]
    public void PackagePanelTabsUI_ShowPanel_ShouldRaiseCanvasAndHidePromptOverlayThroughUnifiedModalRule()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        Canvas uiCanvas = uiRoot.GetComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        Type packageTabsType = ResolveTypeOrFail("PackagePanelTabsUI");
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");

        GameObject packagePanel = Track(new GameObject("PackagePanel", typeof(RectTransform)));
        packagePanel.AddComponent(packageTabsType);
        packagePanel.transform.SetParent(uiRoot.transform, false);
        packagePanel.SetActive(false);

        Transform top = CreateChild(packagePanel.transform, "Top", typeof(RectTransform));
        Transform main = CreateChild(packagePanel.transform, "Main", typeof(RectTransform));
        CreateTopToggle(top, "Top_0");
        CreatePage(main, "0_Props");

        InvokeStatic(promptOverlayType, "EnsureRuntime");
        object promptOverlay = promptOverlayType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        Assert.That(promptOverlay, Is.Not.Null, "测试前应能创建 PromptOverlay 运行时实例。");
        InvokeInstance(promptOverlay, "Show", "测试提示");

        Component tabsUi = packagePanel.GetComponent(packageTabsType);
        Assert.That(tabsUi, Is.Not.Null, "应能获取 PackagePanelTabsUI 组件。");
        InvokeInstance(tabsUi, "SetRoots", packagePanel, top, main);
        InvokeInstance(tabsUi, "ShowPanel", true);
        InvokeInstance(promptOverlay, "LateUpdate");

        Canvas packageCanvas = packagePanel.GetComponent<Canvas>();
        Assert.That(packageCanvas, Is.Not.Null, "PackagePanel 打开后应自动补 Canvas。");
        Assert.That(packagePanel.activeSelf, Is.True, "PackagePanel 打开请求后应进入可见态。");

        CanvasGroup promptCanvasGroup = GetPrivateField<CanvasGroup>(promptOverlay, "canvasGroup");
        Assert.That(promptCanvasGroup, Is.Not.Null, "PromptOverlay 应持有 CanvasGroup。");
        Assert.That(promptCanvasGroup.alpha, Is.LessThan(0.01f), "PackagePanel 打开时，PromptOverlay 应通过统一模态规则隐藏，而不是继续依赖分散 suppress。");
    }

    [Test]
    public void PackagePanelTabsUI_ShowPanel_ShouldExpandBackgroundToRootCanvasBounds()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        uiRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        RectTransform uiRect = uiRoot.GetComponent<RectTransform>();
        uiRect.anchorMin = new Vector2(0.5f, 0.5f);
        uiRect.anchorMax = new Vector2(0.5f, 0.5f);
        uiRect.anchoredPosition = Vector2.zero;
        uiRect.sizeDelta = new Vector2(2560f, 1440f);

        Type packageTabsType = ResolveTypeOrFail("PackagePanelTabsUI");
        GameObject packagePanel = Track(new GameObject("PackagePanel", typeof(RectTransform)));
        RectTransform panelRect = packagePanel.GetComponent<RectTransform>();
        panelRect.SetParent(uiRoot.transform, false);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(1920f, 1080f);
        packagePanel.AddComponent(packageTabsType);
        packagePanel.SetActive(false);

        Transform background = CreateChild(packagePanel.transform, "Background", typeof(RectTransform));
        RectTransform backgroundRect = (RectTransform)background;
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundRect.sizeDelta = Vector2.zero;
        RectTransform backgroundImageRect = (RectTransform)CreateChild(background, "Image", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backgroundImageRect.anchorMin = Vector2.zero;
        backgroundImageRect.anchorMax = Vector2.one;
        backgroundImageRect.anchoredPosition = Vector2.zero;
        backgroundImageRect.sizeDelta = Vector2.zero;

        Transform top = CreateChild(packagePanel.transform, "Top", typeof(RectTransform));
        Transform main = CreateChild(packagePanel.transform, "Main", typeof(RectTransform));
        CreateTopToggle(top, "Top_0");
        CreatePage(main, "0_Props");

        Component tabsUi = packagePanel.GetComponent(packageTabsType);
        InvokeInstance(tabsUi, "SetRoots", packagePanel, top, main);
        InvokeInstance(tabsUi, "ShowPanel", true);

        Assert.That(backgroundRect.anchorMin, Is.EqualTo(new Vector2(0.5f, 0.5f)), "背景扩展后应改为独立中心锚点，不再被固定面板壳的全拉伸约束住。");
        Assert.That(backgroundRect.anchorMax, Is.EqualTo(new Vector2(0.5f, 0.5f)), "背景扩展后应改为独立中心锚点，不再被固定面板壳的全拉伸约束住。");
        Assert.That(backgroundRect.sizeDelta.x, Is.EqualTo(2560f).Within(0.5f), "背景宽度应覆盖根 Canvas，而不是继续停在 1920。");
        Assert.That(backgroundRect.sizeDelta.y, Is.EqualTo(1440f).Within(0.5f), "背景高度应覆盖根 Canvas，而不是继续停在 1080。");
        Assert.That(backgroundRect.anchoredPosition, Is.EqualTo(Vector2.zero).Using(Vector2EqualityComparer.Instance), "当前背包根壳居中时，背景扩展后也应继续居中。");
    }

    [Test]
    public void PromptOverlay_ShouldStayInHudLaneBeforePackageModalSibling()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        uiRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        CreateChild(uiRoot.transform, "State", typeof(RectTransform));
        CreateChild(uiRoot.transform, "ToolBar", typeof(RectTransform));

        GameObject packagePanel = Track(new GameObject("PackagePanel", typeof(RectTransform)));
        packagePanel.AddComponent(ResolveTypeOrFail("PackagePanelTabsUI"));
        packagePanel.transform.SetParent(uiRoot.transform, false);

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        InvokeStatic(promptOverlayType, "EnsureRuntime");
        Component promptOverlay = promptOverlayType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null) as Component;
        Assert.That(promptOverlay, Is.Not.Null, "应能创建 PromptOverlay 运行时实例。");

        promptOverlay.transform.SetAsLastSibling();
        InvokeInstance(promptOverlay, "LateUpdate");

        Assert.That(
            promptOverlay.transform.GetSiblingIndex(),
            Is.LessThan(packagePanel.transform.GetSiblingIndex()),
            "PromptOverlay 应主动回到 HUD lane，停在 PackagePanel 这类 modal sibling 前面。");
    }

    [Test]
    public void PackagePanelTabsUI_ShowPanel_ShouldBlockReusablePromptOverlayEvenWhenStaticFieldPointsToStaleDuplicate()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        uiRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        Type packageTabsType = ResolveTypeOrFail("PackagePanelTabsUI");
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");

        GameObject packagePanel = Track(new GameObject("PackagePanel", typeof(RectTransform)));
        packagePanel.AddComponent(packageTabsType);
        packagePanel.transform.SetParent(uiRoot.transform, false);
        packagePanel.SetActive(false);

        Transform top = CreateChild(packagePanel.transform, "Top", typeof(RectTransform));
        Transform main = CreateChild(packagePanel.transform, "Main", typeof(RectTransform));
        CreateTopToggle(top, "Top_0");
        CreatePage(main, "0_Props");

        InvokeStatic(promptOverlayType, "EnsureRuntime");
        Component promptOverlay = promptOverlayType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null) as Component;
        Assert.That(promptOverlay, Is.Not.Null, "测试前应能创建 PromptOverlay 运行时实例。");
        InvokeInstance(promptOverlay, "Show", "测试提示");

        GameObject staleRoot = Track(new GameObject("PromptOverlay_Stale", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)));
        staleRoot.transform.SetParent(uiRoot.transform, false);
        Component staleOverlay = staleRoot.AddComponent(promptOverlayType);
        SetStaticField(promptOverlayType, "_instance", staleOverlay);

        Component tabsUi = packagePanel.GetComponent(packageTabsType);
        InvokeInstance(tabsUi, "SetRoots", packagePanel, top, main);
        InvokeInstance(tabsUi, "ShowPanel", true);
        InvokeInstance(promptOverlay, "LateUpdate");

        CanvasGroup promptCanvasGroup = GetPrivateField<CanvasGroup>(promptOverlay, "canvasGroup");
        Assert.That(promptCanvasGroup, Is.Not.Null);
        Assert.That(
            promptCanvasGroup.alpha,
            Is.LessThan(0.01f),
            "即使静态字段一度指到 stale duplicate，PackagePanel 也应通过统一入口压住真正可复用的 PromptOverlay。");
    }

    [Test]
    public void DialogueUi_ShouldSkipPromptOverlayWhenScanningNonDialogueSiblings()
    {
        Type dialogueUiType = ResolveTypeOrFail("Sunset.Story.DialogueUI");
        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");

        GameObject candidate = Track(new GameObject("SpringDay1PromptOverlay", typeof(RectTransform)));
        candidate.AddComponent(promptOverlayType);
        GameObject dialogueRoot = Track(new GameObject("DialogueRoot", typeof(RectTransform)));

        object result = InvokeStatic(dialogueUiType, "ShouldManageAsNonDialogueUi", candidate.transform, dialogueRoot.transform);
        Assert.That(result, Is.EqualTo(false), "PromptOverlay 应由自己的对话显隐链管理，不能再被 DialogueUI 当普通 sibling 淡出。");
    }

    [Test]
    public void PromptOverlay_BoundaryFocusAlpha_ShouldComposeWithVisibilityAlpha()
    {
        GameObject uiRoot = Track(new GameObject("UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        uiRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        Type promptOverlayType = ResolveTypeOrFail("Sunset.Story.SpringDay1PromptOverlay");
        InvokeStatic(promptOverlayType, "EnsureRuntime");
        Component promptOverlay = promptOverlayType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null) as Component;
        Assert.That(promptOverlay, Is.Not.Null, "测试前应能创建 PromptOverlay 运行时实例。");

        CanvasGroup promptCanvasGroup = GetPrivateField<CanvasGroup>(promptOverlay, "canvasGroup");
        Assert.That(promptCanvasGroup, Is.Not.Null);

        InvokeInstance(promptOverlay, "FadeCanvasGroup", 1f, true);
        InvokeInstance(promptOverlay, "SetBoundaryFocusAlpha", 0.4f);
        Assert.That(promptCanvasGroup.alpha, Is.EqualTo(0.4f).Within(0.001f), "边界透明只应压缩 PromptOverlay 的最终 alpha，不应改写它的显隐真值。");

        InvokeInstance(promptOverlay, "FadeCanvasGroup", 0f, true);
        InvokeInstance(promptOverlay, "SetBoundaryFocusAlpha", 1f);
        Assert.That(promptCanvasGroup.alpha, Is.EqualTo(0f).Within(0.001f), "当 PromptOverlay 自身已隐藏时，边界透明恢复也不能把它重新抬出来。");
    }

    private GameObject CreatePackagePanelRoot()
    {
        GameObject canvasRoot = Track(new GameObject("CanvasRoot", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)));
        canvasRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject panelRoot = Track(new GameObject("PackagePanel", typeof(RectTransform)));
        panelRoot.transform.SetParent(canvasRoot.transform, false);
        return panelRoot;
    }

    private Transform CreatePage(Transform parent, string pageName)
    {
        Transform page = CreateChild(parent, pageName, typeof(RectTransform));
        Transform main = CreateChild(page, "Main", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        return main;
    }

    private Transform CreateChild(Transform parent, string name, params System.Type[] components)
    {
        GameObject child = Track(new GameObject(name, components));
        child.transform.SetParent(parent, false);
        return child.transform;
    }

    private void CreateTopToggle(Transform parent, string name)
    {
        GameObject toggleObject = Track(new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Toggle)));
        toggleObject.transform.SetParent(parent, false);
    }

    private T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"应能读取私有字段 {fieldName}。");
        return (T)field.GetValue(target);
    }

    private void SetStaticField(System.Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, value);
        }
    }

    private GameObject Track(GameObject gameObject)
    {
        _created.Add(gameObject);
        return gameObject;
    }

    private static Type ResolveType(string fullName)
    {
        System.Type[] allTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToArray();

        for (int index = 0; index < allTypes.Length; index++)
        {
            if (string.Equals(allTypes[index].FullName, fullName, System.StringComparison.Ordinal)
                || string.Equals(allTypes[index].Name, fullName, System.StringComparison.Ordinal))
            {
                return allTypes[index];
            }
        }

        return null;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        Type resolved = ResolveType(fullName);
        Assert.That(resolved, Is.Not.Null, $"应能解析类型 {fullName}。");
        return resolved;
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(method, Is.Not.Null, $"应能找到静态方法 {type.FullName}.{methodName}。");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(method, Is.Not.Null, $"应能找到实例方法 {target.GetType().FullName}.{methodName}。");
        return method.Invoke(target, args);
    }

    private sealed class Vector2EqualityComparer : IEqualityComparer<Vector2>
    {
        public static readonly Vector2EqualityComparer Instance = new();

        public bool Equals(Vector2 x, Vector2 y)
        {
            return Mathf.Abs(x.x - y.x) < 0.001f && Mathf.Abs(x.y - y.y) < 0.001f;
        }

        public int GetHashCode(Vector2 obj)
        {
            return obj.GetHashCode();
        }
    }
}
