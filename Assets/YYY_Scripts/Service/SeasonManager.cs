using UnityEngine;
using System;

/// <summary>
/// 季节管理器 - 统一管理游戏中的日历季节和植被季节
/// 挂载到场景中的GameManager或空物体上
/// </summary>
public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; private set; }
    
    /// <summary>
    /// 日历季节枚举（4个）
    /// </summary>
    public enum Season
    {
        Spring = 0,  // 春
        Summer = 1,  // 夏
        Autumn = 2,  // 秋
        Winter = 3   // 冬
    }
    
    /// <summary>
    /// 植被季节枚举（5个，用于渐变换季）
    /// </summary>
    public enum VegetationSeason
    {
        EarlySpring,            // 早春 (春1-14)
        LateSpringEarlySummer,  // 晚春早夏 (春15-夏14)
        LateSummerEarlyFall,    // 晚夏早秋 (夏15-秋14)
        LateFall,               // 晚秋 (秋15-28)
        Winter                  // 冬季 (冬1-28)
    }
    
    [Header("━━━━ 日历季节 ━━━━")]
    [Tooltip("当前日历季节（春/夏/秋/冬）")]
    [SerializeField] private Season currentSeason = Season.Spring;
    
    [Header("━━━━ 植被季节（渐变系统）━━━━")]
    [Tooltip("当前植被季节（5个细分季节）")]
    [SerializeField] private VegetationSeason currentVegetationSeason = VegetationSeason.EarlySpring;
    
    [Tooltip("季节过渡进度（0.0-1.0）\n0.2 = 20%植被显示新季节\n0.8 = 80%植被显示新季节")]
    [SerializeField, Range(0f, 1f)] private float seasonTransitionProgress = 0f;
    
    [Header("━━━━ 集成设置 ━━━━")]
    [Tooltip("是否使用TimeManager（推荐）")]
    [SerializeField] private bool useTimeManager = true;
    
    [Header("━━━━ 测试用 ━━━━")]
    [Tooltip("仅当不使用TimeManager时有效")]
    [SerializeField] private bool enableDebugControl = false;
    
    [Tooltip("显示调试信息")]
    [SerializeField] private bool showDebugInfo = true;
    
    /// <summary>
    /// 日历季节改变事件
    /// </summary>
    public static event Action<Season> OnSeasonChanged;
    
    /// <summary>
    /// 植被季节改变事件（用于树木更新显示）
    /// </summary>
    public static event Action OnVegetationSeasonChanged;
    
    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 如果使用TimeManager，订阅其事件
        if (useTimeManager)
        {
            TimeManager.OnSeasonChanged += OnTimeManagerSeasonChanged;
            TimeManager.OnDayChanged += OnTimeManagerDayChanged;
            
            // 同步TimeManager的当前季节
            if (TimeManager.Instance != null)
            {
                currentSeason = TimeManager.Instance.GetSeason();
            }
        }
        
        // 初始化植被季节
        UpdateVegetationSeason();
        
        // 初始化时通知所有植物当前季节
        OnSeasonChanged?.Invoke(currentSeason);
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[SeasonManager] 初始化完成 - {(useTimeManager ? "使用TimeManager" : "独立模式")}</color>");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅
        if (useTimeManager)
        {
            TimeManager.OnSeasonChanged -= OnTimeManagerSeasonChanged;
            TimeManager.OnDayChanged -= OnTimeManagerDayChanged;
        }
    }
    
    /// <summary>
    /// TimeManager季节变化回调
    /// </summary>
    private void OnTimeManagerSeasonChanged(Season newSeason, int year)
    {
        SetSeason(newSeason);
        UpdateVegetationSeason();
        
        if (showDebugInfo)
        {
            Debug.Log($"<color=orange>[SeasonManager] 季节变化 → {GetSeasonName(newSeason)} (Year {year})</color>");
        }
    }
    
    /// <summary>
    /// TimeManager每日变化回调（用于更新植被季节渐变进度）
    /// </summary>
    private void OnTimeManagerDayChanged(int year, int seasonDay, int totalDays)
    {
        UpdateVegetationSeason();
    }
    
    /// <summary>
    /// 更新植被季节和渐变进度
    /// </summary>
    private void UpdateVegetationSeason()
    {
        if (!useTimeManager || TimeManager.Instance == null)
        {
            return;
        }
        
        int dayInSeason = TimeManager.Instance.GetDay(); // 1-28
        VegetationSeason newVegSeason;
        float newProgress = 0f;
        
        switch (currentSeason)
        {
            case Season.Spring:
                if (dayInSeason <= 14)
                {
                    // 早春（春1-14）：100%春季
                    newVegSeason = VegetationSeason.EarlySpring;
                    newProgress = 0f;
                }
                else
                {
                    // 晚春早夏（春15-夏14，共28天）
                    newVegSeason = VegetationSeason.LateSpringEarlySummer;
                    // 春15-28天是前14天，进度：0.2 → ~0.489
                    int dayInTransition = dayInSeason - 14; // 1-14
                    newProgress = 0.2f + (dayInTransition - 1) / 27f * 0.6f;
                }
                break;
                
            case Season.Summer:
                if (dayInSeason <= 14)
                {
                    // 晚春早夏后半段（夏1-14）
                    newVegSeason = VegetationSeason.LateSpringEarlySummer;
                    // 夏1-14天是后14天，进度：~0.511 → 0.8
                    int dayInTransition = 14 + dayInSeason; // 15-28
                    newProgress = 0.2f + (dayInTransition - 1) / 27f * 0.6f;
                }
                else
                {
                    // 晚夏早秋（夏15-秋14，共28天）
                    newVegSeason = VegetationSeason.LateSummerEarlyFall;
                    // 夏15-28天是前14天，进度：0.2 → ~0.489
                    int dayInTransition = dayInSeason - 14; // 1-14
                    newProgress = 0.2f + (dayInTransition - 1) / 27f * 0.6f;
                }
                break;
                
            case Season.Autumn:
                if (dayInSeason <= 14)
                {
                    // 晚夏早秋后半段（秋1-14）
                    newVegSeason = VegetationSeason.LateSummerEarlyFall;
                    // 秋1-14天是后14天，进度：~0.511 → 0.8
                    int dayInTransition = 14 + dayInSeason; // 15-28
                    newProgress = 0.2f + (dayInTransition - 1) / 27f * 0.6f;
                }
                else
                {
                    // 晚秋（秋15-28）：100%晚秋
                    newVegSeason = VegetationSeason.LateFall;
                    newProgress = 0f;
                }
                break;
                
            case Season.Winter:
                // 冬季：100%冬季
                newVegSeason = VegetationSeason.Winter;
                newProgress = 0f;
                break;
                
            default:
                newVegSeason = VegetationSeason.EarlySpring;
                newProgress = 0f;
                break;
        }
        
        bool changed = false;
        if (currentVegetationSeason != newVegSeason)
        {
            currentVegetationSeason = newVegSeason;
            changed = true;
        }
        
        if (Mathf.Abs(seasonTransitionProgress - newProgress) > 0.01f)
        {
            seasonTransitionProgress = newProgress;
            changed = true;
        }
        
        if (changed)
        {
            // 通知所有植被更新显示
            OnVegetationSeasonChanged?.Invoke();
            
            if (showDebugInfo)
            {
                Debug.Log($"<color=cyan>[SeasonManager] 植被季节: {currentVegetationSeason}, 进度: {seasonTransitionProgress:F2} ({seasonTransitionProgress * 100:F0}%树显示新季节)</color>");
            }
        }
    }
    
    private void Update()
    {
        // 测试用：仅当不使用TimeManager时，允许手动切换季节
        if (!useTimeManager && enableDebugControl && Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetSeason(Season.Spring);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetSeason(Season.Summer);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetSeason(Season.Autumn);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetSeason(Season.Winter);
        }
    }
    
    /// <summary>
    /// 设置日历季节
    /// </summary>
    public void SetSeason(Season newSeason)
    {
        if (currentSeason != newSeason)
        {
            currentSeason = newSeason;
            
            if (showDebugInfo)
            {
                Debug.Log($"[SeasonManager] 切换到 {GetSeasonName(newSeason)}");
            }
            
            // 通知所有订阅者
            OnSeasonChanged?.Invoke(currentSeason);
        }
    }
    
    /// <summary>
    /// 获取当前日历季节
    /// </summary>
    public Season GetCurrentSeason()
    {
        return currentSeason;
    }
    
    /// <summary>
    /// 获取当前植被季节
    /// </summary>
    public VegetationSeason GetCurrentVegetationSeason()
    {
        return currentVegetationSeason;
    }
    
    /// <summary>
    /// 获取季节过渡进度（0.0-1.0）
    /// 用于树木判断是否显示新季节外观
    /// </summary>
    public float GetTransitionProgress()
    {
        return seasonTransitionProgress;
    }
    
    /// <summary>
    /// 获取季节名称（中文）
    /// </summary>
    public string GetSeasonName(Season season)
    {
        switch (season)
        {
            case Season.Spring: return "春天";
            case Season.Summer: return "夏天";
            case Season.Autumn: return "秋天";
            case Season.Winter: return "冬天";
            default: return "未知";
        }
    }
}
