using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackagePanelTabsUI : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject panelRoot;         // PackagePanel 根
    [SerializeField] private Transform topParent;           // 6_Top
    [SerializeField] private Transform pagesParent;         // Main 下的 0_Props,1_Recipes,...

    private readonly Dictionary<int, Toggle> topToggles = new Dictionary<int, Toggle>();
    private readonly Dictionary<int, GameObject> pages = new Dictionary<int, GameObject>();
    private int currentIndex = -1;
    private bool suppressToggleCallbacks = false;
    private bool initialStateApplied = false;

    void Awake()
    {
        TryAutoLocate();
        Collect();
        ApplyInitialState();
        WireToggles();
    }

    public void SetRoots(GameObject root, Transform top, Transform pagesRoot)
    {
        panelRoot = root; topParent = top; pagesParent = pagesRoot; Collect(); ApplyInitialState(); WireToggles();
    }

    void Collect()
    {
        topToggles.Clear();
        pages.Clear();
        if (topParent != null)
        {
            foreach (Transform t in topParent)
            {
                if (t.name.StartsWith("Top_"))
                {
                    if (int.TryParse(t.name.Substring(4), out int idx))
                    {
                        var tg = t.GetComponent<Toggle>();
                        if (tg != null) topToggles[idx] = tg;
                    }
                }
            }
        }
        if (pagesParent != null)
        {
            foreach (Transform t in pagesParent)
            {
                if (t.name.Length > 2 && char.IsDigit(t.name[0]) && t.name[1] == '_')
                {
                    int idx = t.name[0] - '0';
                    pages[idx] = t.gameObject;
                }
            }
        }
    }

    void TryAutoLocate()
    {
        if (panelRoot == null)
        {
            panelRoot = LocatePanelRoot();
        }
        if (topParent == null && panelRoot != null)
        {
            topParent = FindChildByName(panelRoot.transform, "6_Top");
            if (topParent == null) topParent = FindChildContains(panelRoot.transform, "Top");
        }
        if (pagesParent == null && panelRoot != null)
        {
            // 优先找名为 Main 的节点；找不到就根据 0_Props 的父来推断
            pagesParent = FindChildByName(panelRoot.transform, "Main");
            if (pagesParent == null)
            {
                var props = FindChildByName(panelRoot.transform, "0_Props");
                if (props != null) pagesParent = props.parent;
            }
        }
    }

    Transform FindChildByName(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == name) return t;
        }
        return null;
    }

    Transform FindChildContains(Transform root, string part)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.Contains(part)) return t;
        }
        return null;
    }

    GameObject LocatePanelRoot()
    {
        // 优先在自身及父级中寻找名称包含 "PackagePanel" 的对象
        Transform walker = transform;
        while (walker != null)
        {
            if (walker.name.IndexOf("PackagePanel", StringComparison.OrdinalIgnoreCase) >= 0)
                return walker.gameObject;
            walker = walker.parent;
        }

        // 退而求其次：在场景根节点下遍历查找
        if (gameObject.scene.IsValid())
        {
            var roots = gameObject.scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name.IndexOf("PackagePanel", StringComparison.OrdinalIgnoreCase) >= 0)
                        return t.gameObject;
                }
            }
        }

        // 最后兜底：若仍未找到，则使用父物体，若无父则自身
        return transform.parent != null ? transform.parent.gameObject : gameObject;
    }

    void WireToggles()
    {
        // 给Top的Toggle绑定点击事件，使鼠标点击和键盘快捷键逻辑统一
        foreach (var kv in topToggles)
        {
            var idx = kv.Key;
            var tg = kv.Value;
            if (tg == null) continue;
            // 先移除旧监听避免重复
            tg.onValueChanged.RemoveAllListeners();
            // 绑定：Toggle被选中时调用OpenOrToggle
            tg.onValueChanged.AddListener(isOn => HandleToggleChanged(idx, isOn));
        }
    }

    // 供外部在运行时激活UI后调用，确保自动定位与绑定完成
    public void EnsureReady()
    {
        TryAutoLocate();
        Collect();
        WireToggles();
        if (panelRoot != null && !panelRoot.activeSelf)
        {
            ApplyInitialState();
        }
    }

    public void ShowPanel(bool visible)
    {
        if (visible) OpenPanel(); else ClosePanel();
    }

    public void TogglePanel()
    {
        if (panelRoot == null) return;
        bool now = !panelRoot.activeSelf;
        ShowPanel(now);
    }

    public bool IsPanelOpen()
    {
        return panelRoot != null && panelRoot.activeSelf;
    }

    public void ShowPage(int idx)
    {
        EnsureCollected();
        // 若找不到该idx，尝试按名称部分匹配
        if (panelRoot == null)
        {
            Debug.LogError("[PackagePanelTabsUI] ShowPage called but panelRoot is null!");
            return;
        }
        if (!pages.ContainsKey(idx)) return;

        SetVisiblePage(idx);
    }

    // 快捷方法
    public void OpenProps()      { OpenOrToggle(0); }
    public void OpenRecipes()    { OpenOrToggle(1); }
    public void OpenEx()         { OpenOrToggle(2); }
    public void OpenMap()        { OpenOrToggle(3); }
    public void OpenRelations()  { OpenOrToggle(4); }
    public void OpenSettings()   { OpenOrToggle(5); }

    private void OpenOrToggle(int idx)
    {
        EnsureCollected();
        bool isOpen = panelRoot != null && panelRoot.activeSelf;
        if (!isOpen)
        {
            OpenPanel();
            SetVisiblePage(idx);
            return;
        }
        if (currentIndex == idx)
        {
            ClosePanel();
            return;
        }
        SetVisiblePage(idx);
    }

    void SetToggleSelection(int idx)
    {
        suppressToggleCallbacks = true;
        foreach (var kv in topToggles)
        {
            var tg = kv.Value;
            if (tg == null) continue;
#if UNITY_2021_2_OR_NEWER
            tg.SetIsOnWithoutNotify(idx >= 0 && kv.Key == idx);
#else
            tg.isOn = idx >= 0 && kv.Key == idx;
#endif
        }
        suppressToggleCallbacks = false;
    }

    void HandleToggleChanged(int idx, bool isOn)
    {
        if (suppressToggleCallbacks) return;
        if (isOn)
        {
            // 鼠标点击Toggle：只负责打开面板和切换页面，不关闭
            bool isOpen = panelRoot != null && panelRoot.activeSelf;
            if (!isOpen)
            {
                OpenPanel();
            }
            SetVisiblePage(idx);
            return;
        }
        // Toggle被取消选中时不做任何操作，因为会有其他Toggle被选中
    }

    void SetVisiblePage(int idx)
    {
        if (!pages.ContainsKey(idx)) return;

        foreach (var kv in pages)
        {
            if (kv.Value != null) kv.Value.SetActive(kv.Key == idx);
        }

        SetToggleSelection(idx);
        currentIndex = idx;
    }

    void SetVisiblePageInactive()
    {
        foreach (var kv in pages)
        {
            if (kv.Value != null) kv.Value.SetActive(false);
        }
        currentIndex = -1;
        SetToggleSelection(-1);
    }

    void OpenPanel()
    {
        if (panelRoot == null || panelRoot.activeSelf) return;
        panelRoot.SetActive(true);
        OnPanelJustOpened();
    }

    void ClosePanel()
    {
        if (panelRoot == null || !panelRoot.activeSelf) return;
        panelRoot.SetActive(false);
        SetVisiblePageInactive();
    }

    void ApplyInitialState()
    {
        if (initialStateApplied) return;
        initialStateApplied = true;
        SetVisiblePageInactive();
        if (panelRoot != null && panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
        }
    }

    void EnsureCollected()
    {
        if (topParent == null || pagesParent == null) TryAutoLocate();
        if (topToggles.Count == 0 || pages.Count == 0) Collect();
    }

    void OnPanelJustOpened()
    {
        // 主面板从关闭到打开：重置道具栏与装备栏选择（满足“关闭再打开才映射Toolbar选择”的需求）
        var invPanel = panelRoot != null ? panelRoot.GetComponentInChildren<InventoryPanelUI>(true) : null;
        if (invPanel != null)
        {
            invPanel.EnsureBuilt();
            invPanel.ResetSelectionsOnPanelOpen();
        }
    }

    // 备用：通过名称片段打开（以防层级命名不同）
    public void OpenByName(string pageNamePart)
    {
        EnsureCollected();
        int idx = -1;
        foreach (var kv in pages)
        {
            if (kv.Value != null && kv.Value.name.IndexOf(pageNamePart, System.StringComparison.OrdinalIgnoreCase) >= 0)
            { idx = kv.Key; break; }
        }
        if (idx >= 0) { if (!IsPanelOpen()) ShowPanel(true); ShowPage(idx); }
    }
}
