using UnityEngine;
using Sunset.Story;

/// <summary>
/// TimeManager调试器 - 独立组件，方便删除
/// 提供快捷键控制时间跳转
/// ⚠️ 仅用于开发调试，发布前可直接删除此脚本
/// </summary>
public class TimeManagerDebugger : MonoBehaviour
{
    [Header("━━━━ 调试快捷键 ━━━━")]
    [Tooltip("启用调试快捷键")]
    public bool enableDebugKeys = true;
    
    [Header("方向键控制")]
    [Tooltip("→ 右箭头：跳到下一天")]
    public KeyCode nextDayKey = KeyCode.RightArrow;
    
    [Tooltip("↓ 下箭头：跳到下一季节")]
    public KeyCode nextSeasonKey = KeyCode.DownArrow;
    
    [Tooltip("↑ 上箭头：跳到上一季节")]
    public KeyCode prevSeasonKey = KeyCode.UpArrow;
    
    [Header("其他快捷键")]
    [Tooltip("T键：切换时间倍速（1x/5x）")]
    public KeyCode toggleSpeedKey = KeyCode.T;
    
    [Tooltip("P键：暂停/继续")]
    public KeyCode pauseKey = KeyCode.P;
    
    [Header("显示设置")]
    [Tooltip("显示调试信息")]
    public bool showDebugInfo = true;

    [Header("━━━━ 右上角调试 UI 缩放 ━━━━")]
    [Tooltip("用于统一不同分辨率下右上角调试信息的参考分辨率")]
    public Vector2 guiReferenceResolution = new Vector2(1920f, 1080f);

    [Range(0f, 1f)]
    [Tooltip("0=更偏宽度，1=更偏高度，语义对齐 CanvasScaler 的 Match")]
    public float guiMatchWidthOrHeight = 0.5f;

    [Tooltip("限制调试 UI 的缩放上下界，避免超宽或超高分辨率下体感失控")]
    public Vector2 guiScaleClamp = new Vector2(0.85f, 1.2f);
    
    [Header("━━━━ 屏幕时钟 & 时间微调 ━━━━")]
    [Tooltip("启用屏幕右上角时钟显示 + 键盘加减号微调时间\n" +
             "NumPad+/= : 前进1小时\n" +
             "NumPad-/- : 后退1小时")]
    public bool enableScreenClock = true;
    
    // 右上角 HUD GUI 缓存
    private GUIStyle clockTagStyle;
    private GUIStyle clockTimeStyle;
    private GUIStyle clockMetaStyle;
    private GUIStyle helpTitleStyle;
    private GUIStyle helpKeyStyle;
    private GUIStyle helpDescriptionStyle;
    private float cachedGuiScale = -1f;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapRuntimeDebugger()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var existingDebugger = FindFirstObjectByType<TimeManagerDebugger>(FindObjectsInactive.Include);
        if (existingDebugger != null)
        {
            return;
        }

        EnsureAttached(TimeManager.Instance);
    }
#endif

    public static TimeManagerDebugger EnsureAttached(TimeManager timeManager, bool enableScreenClockByDefault = true, bool showDebugInfoByDefault = true)
    {
        if (timeManager == null)
        {
            return null;
        }

        var debugger = timeManager.GetComponent<TimeManagerDebugger>();
        if (debugger == null)
        {
            debugger = timeManager.gameObject.AddComponent<TimeManagerDebugger>();
        }

        debugger.enableDebugKeys = true;
        debugger.enableScreenClock = enableScreenClockByDefault;
        debugger.showDebugInfo = showDebugInfoByDefault;
        debugger.enabled = true;
        return debugger;
    }

    private void Awake()
    {
        enableDebugKeys = true;
    }
    
    private void Update()
    {
        if (!enableDebugKeys || TimeManager.Instance == null) return;
        
        // → 右箭头：下一天
        if (Input.GetKeyDown(nextDayKey))
        {
            AdvanceDay();
        }
        
        // ↓ 下箭头：下一季节
        if (Input.GetKeyDown(nextSeasonKey))
        {
            AdvanceSeason();
        }
        
        // ↑ 上箭头：上一季节
        if (Input.GetKeyDown(prevSeasonKey))
        {
            PreviousSeason();
        }
        
        // T键：切换倍速
        if (Input.GetKeyDown(toggleSpeedKey))
        {
            ToggleTimeScale();
        }
        
        // P键：暂停/继续
        if (Input.GetKeyDown(pauseKey))
        {
            TimeManager.Instance.TogglePause();
        }
        
        // 屏幕时钟模式：键盘 +/- 微调时间
        if (enableScreenClock)
        {
            // 小键盘 + 或主键盘 =（同一个物理键）
            if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
            {
                AdvanceOneHour();
            }
            // 小键盘 - 或主键盘 -
            if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
            {
                RewindOneHour();
            }
        }
    }
    
    /// <summary>
    /// 前进到下一天
    /// </summary>
    private void AdvanceDay()
    {
        if (TryApplyDay1ManagedTimeTarget(TimeManager.Instance.GetYear(), TimeManager.Instance.GetSeason(), TimeManager.Instance.GetDay() + 1, 6, 0))
        {
            return;
        }

        TimeManager.Instance.Sleep();
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[Debugger] → 跳到下一天: {TimeManager.Instance.GetFormattedTime()}</color>");
        }
    }
    
    /// <summary>
    /// 前进到下一季节
    /// </summary>
    private void AdvanceSeason()
    {
        SeasonManager.Season currentSeason = TimeManager.Instance.GetSeason();
        int nextSeasonIndex = ((int)currentSeason + 1) % 4;
        SeasonManager.Season nextSeason = (SeasonManager.Season)nextSeasonIndex;
        
        // 跳到下一季的第1天
        int currentYear = TimeManager.Instance.GetYear();
        if (nextSeason == SeasonManager.Season.Spring)
        {
            currentYear++; // 新年
        }
        
        if (TryApplyDay1ManagedTimeTarget(currentYear, nextSeason, 1, 6, 0))
        {
            return;
        }

        TimeManager.Instance.SetTime(currentYear, nextSeason, 1, 6, 0);
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=orange>[Debugger] ↓ 跳到下一季节: {nextSeason} (Year {currentYear})</color>");
        }
    }
    
    /// <summary>
    /// 返回到上一季节
    /// </summary>
    private void PreviousSeason()
    {
        SeasonManager.Season currentSeason = TimeManager.Instance.GetSeason();
        int prevSeasonIndex = ((int)currentSeason - 1 + 4) % 4;
        SeasonManager.Season prevSeason = (SeasonManager.Season)prevSeasonIndex;
        
        // 跳到上一季的第1天
        int currentYear = TimeManager.Instance.GetYear();
        if (prevSeason == SeasonManager.Season.Winter)
        {
            currentYear = Mathf.Max(1, currentYear - 1); // 上一年（最小Year 1）
        }
        
        if (TryApplyDay1ManagedTimeTarget(currentYear, prevSeason, 1, 6, 0))
        {
            return;
        }

        TimeManager.Instance.SetTime(currentYear, prevSeason, 1, 6, 0);
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=yellow>[Debugger] ↑ 跳到上一季节: {prevSeason} (Year {currentYear})</color>");
        }
    }
    
    /// <summary>
    /// 切换时间倍速
    /// </summary>
    private void ToggleTimeScale()
    {
        // 在1x和5x之间切换
        float currentScale = TimeManager.Instance.GetType()
            .GetField("timeScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(TimeManager.Instance) as float? ?? 1f;
        
        float newScale = currentScale >= 5f ? 1f : 5f;
        TimeManager.Instance.SetTimeScale(newScale);
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=lime>[Debugger] ⚡ 时间倍速: {newScale}x</color>");
        }
    }

    /// <summary>
    /// 前进 1 小时，始终跳到整点
    /// 例：14:30 → 15:00，26:45 → 第二天 06:00
    /// </summary>
    private void AdvanceOneHour()
    {
        var tm = TimeManager.Instance;
        int targetHour = tm.GetHour() + 1;

        // 超过 dayEndHour(26) → 直接走 Sleep() 真链，避免再补分钟污染 Day1 夜间语义。
        if (targetHour > 26)
        {
            tm.Sleep();

            if (showDebugInfo)
                Debug.Log($"<color=cyan>[Debugger] ⏩ +1小时 → 跨天(Sleep) → {tm.GetFormattedTime()}</color>");
            return;
        }

        if (TryApplyDay1ManagedTimeTarget(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0))
        {
            return;
        }

        tm.SetTime(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[Debugger] ⏩ +1小时 → {tm.GetFormattedTime()}</color>");
    }


    /// <summary>
    /// 后退 1 小时，始终跳到整点，最低不低于当天 06:00
    /// </summary>
    private void RewindOneHour()
    {
        var tm = TimeManager.Instance;
        int currentHour = tm.GetHour();
        int targetHour = Mathf.Max(6, currentHour - 1);

        if (TryApplyDay1ManagedTimeTarget(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0))
        {
            return;
        }

        tm.SetTime(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[Debugger] ⏪ -1小时 → {tm.GetFormattedTime()}</color>");
    }

    private void OnGUI()
    {
        if (!enableDebugKeys) return;

        float guiScale = GetGuiScaleFactor();
        EnsureGuiStyles(guiScale);

        float panelWidth = 272f * guiScale;
        float clockHeight = 68f * guiScale;
        float margin = 12f * guiScale;
        float helpHeight = 134f * guiScale;
        bool hasClock = enableScreenClock && TimeManager.Instance != null;
        Rect clockRect = new Rect(Screen.width - panelWidth - margin, margin, panelWidth, clockHeight);

        if (showDebugInfo)
        {
            float helpTop = hasClock ? clockRect.yMax + (8f * guiScale) : margin;
            Rect helpRect = new Rect(Screen.width - panelWidth - margin, helpTop, panelWidth, helpHeight);
            DrawShortcutsPanel(helpRect, guiScale);
        }

        if (!hasClock) return;

        var tm = TimeManager.Instance;
        string seasonCN = tm.GetSeason() switch
        {
            SeasonManager.Season.Spring => "春",
            SeasonManager.Season.Summer => "夏",
            SeasonManager.Season.Autumn => "秋",
            SeasonManager.Season.Winter => "冬",
            _ => "?"
        };
        
        int displayHour = tm.GetHour();
        if (displayHour >= 24) displayHour -= 24;

        DrawClockPanel(
            clockRect,
            $"Y{tm.GetYear()} {seasonCN} D{tm.GetDay()}",
            $"{displayHour:D2}:{tm.GetMinute():D2}",
            tm.IsTimePaused() ? "已暂停" : "运行中",
            guiScale);
    }

    private float GetGuiScaleFactor()
    {
        float referenceWidth = Mathf.Max(1f, guiReferenceResolution.x);
        float referenceHeight = Mathf.Max(1f, guiReferenceResolution.y);
        float logWidth = Mathf.Log(Mathf.Max(1f, Screen.width) / referenceWidth, 2f);
        float logHeight = Mathf.Log(Mathf.Max(1f, Screen.height) / referenceHeight, 2f);
        float weightedAverage = Mathf.Lerp(logWidth, logHeight, guiMatchWidthOrHeight);
        float scale = Mathf.Pow(2f, weightedAverage);

        float minScale = Mathf.Min(guiScaleClamp.x, guiScaleClamp.y);
        float maxScale = Mathf.Max(guiScaleClamp.x, guiScaleClamp.y);
        return Mathf.Clamp(scale, minScale, maxScale);
    }

    private void EnsureGuiStyles(float guiScale)
    {
        if (clockTagStyle != null
            && clockTimeStyle != null
            && clockMetaStyle != null
            && helpTitleStyle != null
            && helpKeyStyle != null
            && helpDescriptionStyle != null
            && Mathf.Abs(cachedGuiScale - guiScale) < 0.001f)
        {
            return;
        }

        cachedGuiScale = guiScale;

        clockTagStyle = new GUIStyle(GUI.skin.label);
        clockTagStyle.fontSize = Mathf.Max(9, Mathf.RoundToInt(9.5f * guiScale));
        clockTagStyle.fontStyle = FontStyle.Bold;
        clockTagStyle.normal.textColor = new Color(0.97f, 0.95f, 0.90f, 1f);
        clockTagStyle.alignment = TextAnchor.MiddleCenter;

        clockTimeStyle = new GUIStyle(GUI.skin.label);
        clockTimeStyle.fontSize = Mathf.Max(18, Mathf.RoundToInt(23f * guiScale));
        clockTimeStyle.fontStyle = FontStyle.Bold;
        clockTimeStyle.normal.textColor = new Color(0.98f, 0.97f, 0.93f, 1f);
        clockTimeStyle.alignment = TextAnchor.MiddleCenter;

        clockMetaStyle = new GUIStyle(GUI.skin.label);
        clockMetaStyle.fontSize = Mathf.Max(10, Mathf.RoundToInt(11f * guiScale));
        clockMetaStyle.fontStyle = FontStyle.Bold;
        clockMetaStyle.normal.textColor = new Color(0.91f, 0.88f, 0.80f, 0.82f);
        clockMetaStyle.alignment = TextAnchor.MiddleCenter;

        helpTitleStyle = new GUIStyle(GUI.skin.label);
        helpTitleStyle.fontSize = Mathf.Max(12, Mathf.RoundToInt(14f * guiScale));
        helpTitleStyle.fontStyle = FontStyle.Bold;
        helpTitleStyle.normal.textColor = new Color(0.98f, 0.96f, 0.92f, 1f);
        helpTitleStyle.alignment = TextAnchor.MiddleLeft;

        helpKeyStyle = new GUIStyle(GUI.skin.label);
        helpKeyStyle.fontSize = Mathf.Max(10, Mathf.RoundToInt(10.5f * guiScale));
        helpKeyStyle.fontStyle = FontStyle.Bold;
        helpKeyStyle.normal.textColor = new Color(0.20f, 0.11f, 0.03f, 1f);
        helpKeyStyle.alignment = TextAnchor.MiddleCenter;

        helpDescriptionStyle = new GUIStyle(GUI.skin.label);
        helpDescriptionStyle.fontSize = Mathf.Max(11, Mathf.RoundToInt(11.5f * guiScale));
        helpDescriptionStyle.fontStyle = FontStyle.Bold;
        helpDescriptionStyle.normal.textColor = new Color(0.96f, 0.94f, 0.89f, 0.96f);
        helpDescriptionStyle.alignment = TextAnchor.MiddleLeft;
    }

    private void DrawClockPanel(Rect rect, string dateText, string timeText, string stateText, float guiScale)
    {
        DrawPanelChrome(rect, guiScale);
        DrawTagPill(new Rect(rect.x + (18f * guiScale), rect.y + (12f * guiScale), 40f * guiScale, 16f * guiScale), "时间");

        GUI.Label(
            new Rect(rect.x + (10f * guiScale), rect.y + (8f * guiScale), rect.width - (20f * guiScale), 28f * guiScale),
            timeText,
            clockTimeStyle);
        GUI.Label(
            new Rect(rect.x + (10f * guiScale), rect.y + (38f * guiScale), rect.width - (20f * guiScale), 18f * guiScale),
            $"{dateText}  {stateText}",
            clockMetaStyle);
    }

    private void DrawShortcutsPanel(Rect rect, float guiScale)
    {
        DrawPanelChrome(rect, guiScale);
        DrawTagPill(new Rect(rect.x + (18f * guiScale), rect.y + (12f * guiScale), 40f * guiScale, 16f * guiScale), "调试");

        GUI.Label(
            new Rect(rect.x + (68f * guiScale), rect.y + (10f * guiScale), rect.width - (88f * guiScale), 20f * guiScale),
            "快捷键",
            helpTitleStyle);

        DrawShortcutRow(rect, 0, "→", "下一天", guiScale);
        DrawShortcutRow(rect, 1, "↓/↑", "切换季节", guiScale);
        DrawShortcutRow(rect, 2, "T", "倍速", guiScale);
        DrawShortcutRow(rect, 3, "P", "暂停", guiScale);
        DrawShortcutRow(rect, 4, "+/-", "调整时间", guiScale);
    }

    private void DrawShortcutRow(Rect panelRect, int rowIndex, string keyText, string description, float guiScale)
    {
        float rowTop = panelRect.y + (40f * guiScale) + rowIndex * (17f * guiScale);
        float keyWidth = (keyText.Length >= 3 ? 48f : 40f) * guiScale;
        Rect keyRect = new Rect(panelRect.x + (18f * guiScale), rowTop, keyWidth, 15f * guiScale);
        Rect descRect = new Rect(panelRect.x + (keyRect.xMax - panelRect.x) + (12f * guiScale), rowTop - (1f * guiScale), panelRect.width - (keyRect.xMax - panelRect.x) - (32f * guiScale), 18f * guiScale);
        DrawKeyPill(keyRect, keyText, guiScale);
        GUI.Label(descRect, description, helpDescriptionStyle);
    }

    private void DrawPanelChrome(Rect rect, float guiScale)
    {
        DrawFilledRect(rect, new Color(0.10f, 0.13f, 0.18f, 0.92f));
        DrawFilledRect(new Rect(rect.x, rect.y, 3f * guiScale, rect.height), new Color(0.93f, 0.72f, 0.27f, 0.96f));
        DrawFilledRect(new Rect(rect.x, rect.y, rect.width, 1f * guiScale), new Color(0.78f, 0.61f, 0.19f, 0.24f));
        DrawFilledRect(new Rect(rect.x, rect.yMax - (1f * guiScale), rect.width, 1f * guiScale), new Color(0.04f, 0.05f, 0.08f, 0.32f));
        DrawFilledRect(new Rect(rect.xMax - (1f * guiScale), rect.y, 1f * guiScale, rect.height), new Color(0.04f, 0.05f, 0.08f, 0.32f));
    }

    private void DrawTagPill(Rect rect, string text)
    {
        DrawFilledRect(rect, new Color(0.93f, 0.72f, 0.27f, 0.23f));
        GUI.Label(rect, text, clockTagStyle);
    }

    private void DrawKeyPill(Rect rect, string text, float guiScale)
    {
        DrawFilledRect(rect, new Color(0.93f, 0.72f, 0.27f, 0.98f));
        DrawFilledRect(new Rect(rect.x, rect.yMax - (1f * guiScale), rect.width, 1f * guiScale), new Color(0.52f, 0.35f, 0.09f, 0.28f));
        GUI.Label(rect, text, helpKeyStyle);
    }

    private static void DrawFilledRect(Rect rect, Color color)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, alphaBlend: true);
        GUI.color = previousColor;
    }

    private bool TryApplyDay1ManagedTimeTarget(
        int requestedYear,
        SeasonManager.Season requestedSeason,
        int requestedDay,
        int requestedHour,
        int requestedMinute)
    {
        SpringDay1Director director = SpringDay1Director.Instance;
        if (director == null)
        {
            return false;
        }

        if (!director.TryNormalizeDebugTimeTarget(
                requestedYear,
                requestedSeason,
                requestedDay,
                requestedHour,
                requestedMinute,
                out int normalizedYear,
                out SeasonManager.Season normalizedSeason,
                out int normalizedDay,
                out int normalizedHour,
                out int normalizedMinute))
        {
            return false;
        }

        TimeManager.Instance.SetTime(normalizedYear, normalizedSeason, normalizedDay, normalizedHour, normalizedMinute);
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[Debugger] Day1 守门已接管时间跳转 → Y{normalizedYear} {normalizedSeason} D{normalizedDay} {normalizedHour:D2}:{normalizedMinute:D2}</color>");
        }

        return true;
    }
}

