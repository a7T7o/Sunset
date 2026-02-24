using UnityEngine;

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
    
    [Header("━━━━ 屏幕时钟 & 时间微调 ━━━━")]
    [Tooltip("启用屏幕右上角时钟显示 + 键盘加减号微调时间\n" +
             "NumPad+/= : 前进1小时\n" +
             "NumPad-/- : 后退1小时")]
    public bool enableScreenClock = true;
    
    // 时钟 GUI 缓存
    private GUIStyle clockStyle;
    private GUIStyle clockShadowStyle;
    
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
    /// 前进到下一个整点（分钟归零）
    /// 例：14:30 → 15:00，14:00 → 15:00
    /// 跨天时走 Sleep() 统一流程，与右方向键行为一致
    /// </summary>
    private void AdvanceOneHour()
    {
        var tm = TimeManager.Instance;
        int targetHour = tm.GetHour() + 1;

        // 超过 dayEndHour(26) → 走 Sleep() 统一流程进入下一天
        if (targetHour > 26)
        {
            tm.Sleep();
            if (showDebugInfo)
                Debug.Log($"<color=cyan>[Debugger] ⏩ +整点 → 跨天(Sleep) → {tm.GetFormattedTime()}</color>");
            return;
        }

        // 分钟归零，跳到下一个整点
        tm.SetTime(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[Debugger] ⏩ +整点 → {tm.GetFormattedTime()}</color>");
    }


    /// <summary>
    /// 后退到上一个整点（分钟归零）
    /// 例：14:30 → 14:00，14:00 → 13:00
    /// 最低不低于当天 06:00
    /// </summary>
    private void RewindOneHour()
    {
        var tm = TimeManager.Instance;
        int currentHour = tm.GetHour();
        int currentMinute = tm.GetMinute();

        int targetHour;
        if (currentMinute > 0)
        {
            // 有分钟余数：退到当前小时的整点（如 14:30 → 14:00）
            targetHour = currentHour;
        }
        else
        {
            // 已经是整点：退到上一个小时（如 14:00 → 13:00）
            targetHour = currentHour - 1;
        }

        if (targetHour < 6) targetHour = 6;

        tm.SetTime(tm.GetYear(), tm.GetSeason(), tm.GetDay(), targetHour, 0);

        if (showDebugInfo)
            Debug.Log($"<color=cyan>[Debugger] ⏪ -整点 → {tm.GetFormattedTime()}</color>");
    }

    private void OnGUI()
    {
        if (!enableDebugKeys) return;
        
        // ── 左上角：快捷键提示 ──
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.yellow;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        
        string helpText = "=== 调试快捷键 ===\n" +
                         "→  下一天\n" +
                         "↓  下一季节\n" +
                         "↑  上一季节\n" +
                         "T  切换倍速\n" +
                         "P  暂停/继续\n" +
                         "+  前进1小时\n" +
                         "-  后退1小时";
        
        GUI.Label(new Rect(10, 10, 200, 160), helpText, style);
        
        // ── 右上角：屏幕时钟 ──
        if (!enableScreenClock || TimeManager.Instance == null) return;
        
        // 懒初始化 GUIStyle
        if (clockStyle == null)
        {
            clockStyle = new GUIStyle(GUI.skin.label);
            clockStyle.fontSize = 22;
            clockStyle.fontStyle = FontStyle.Bold;
            clockStyle.normal.textColor = Color.white;
            clockStyle.alignment = TextAnchor.UpperRight;
            
            clockShadowStyle = new GUIStyle(clockStyle);
            clockShadowStyle.normal.textColor = new Color(0f, 0f, 0f, 0.7f);
        }
        
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
        // 26时制转24时制显示
        if (displayHour >= 24) displayHour -= 24;
        
        string clockText = $"Y{tm.GetYear()} {seasonCN} D{tm.GetDay()}  {displayHour:D2}:{tm.GetMinute():D2}";
        
        float w = 280f;
        float h = 36f;
        float margin = 12f;
        Rect shadowRect = new Rect(Screen.width - w - margin + 1, margin + 1, w, h);
        Rect clockRect = new Rect(Screen.width - w - margin, margin, w, h);
        
        GUI.Label(shadowRect, clockText, clockShadowStyle);
        GUI.Label(clockRect, clockText, clockStyle);
    }
}

