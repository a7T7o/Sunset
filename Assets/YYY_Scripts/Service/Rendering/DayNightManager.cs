using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 昼夜管理器 - 核心协调器（单例）
/// 订阅 TimeManager / WeatherSystem / SeasonManager 事件，
/// 计算最终叠加颜色并驱动 DayNightOverlay（路线 B）。
/// 路线 A（URP 增强层）通过 enableRouteA 开关控制。
/// </summary>
public class DayNightManager : MonoBehaviour
{
    private const string DefaultConfigResourcesPath = "DayNightConfig";
    private const string RuntimeConfigNamePrefix = "RuntimePrepared_";
#if UNITY_EDITOR
    private const string DefaultConfigEditorAssetPath = "Assets/111_Data/DayNightConfig.asset";
#endif

    #region 单例
    private static DayNightManager instance;
    public static DayNightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<DayNightManager>();
            }
            return instance;
        }
    }
    #endregion

    #region Inspector 配置

    [Header("━━━━ 配置 ━━━━")]
    [SerializeField] private DayNightConfig config;

    [Header("━━━━ 路线控制 ━━━━")]
    [Tooltip("路线 A 开关（URP 增强层）")]
    [SerializeField] private bool enableRouteA = false;

    [Header("━━━━ 组件引用 ━━━━")]
    [SerializeField] private DayNightOverlay overlay;

    [Tooltip("路线 A：全局光源控制器（可选）")]
    [SerializeField] private GlobalLightController globalLight;

    [Tooltip("路线 A：局部光源管理器（可选）")]
    [SerializeField] private PointLightManager pointLightMgr;

    [Header("━━━━ 调试 ━━━━")]
    [SerializeField] private bool showDebugInfo = true;

    [Header("━━━━ 夜晚视野 ━━━━")]
    [SerializeField] private bool enableNightVision = true;
    [SerializeField] private float duskStartHour = 18f;
    [SerializeField] private float fullNightHour = 21f;
    [SerializeField] private float dawnRecoverHour = 6f;
    [SerializeField] private float dawnClearHour = 7f;
    [SerializeField] private float dayVisionRadiusNormalized = 1.15f;
    [SerializeField] private float nightVisionRadiusNormalized = 0.4f;
    [SerializeField] private float visionSoftness = 0.78f;
    [SerializeField] private float visionOuterDarkness = 0.48f;
    [SerializeField] private float visionAspect = 0.84f;
    [SerializeField] private float dawnResidualVisionStrength = 0.22f;

    [Header("━━━━ 夜灯 Overlay ━━━━")]
    [SerializeField] private bool enableNightLightOverlay = true;
    [SerializeField] private float nightLightWarmth = 0.42f;
    [SerializeField] private float nightLightIntensityScale = 0.9f;
    [SerializeField] private float nightLightSearchInterval = 1f;
    [SerializeField] private int maxOverlayNightLights = 8;

    [Tooltip("当前叠加颜色（只读，调试用）")]
    [SerializeField] private Color debugFinalColor;

    [Tooltip("当前亮度（只读，调试用）")]
    [SerializeField] private float debugBrightness;

    [Tooltip("是否为夜晚模式（只读，调试用）")]
    [SerializeField] private bool debugIsNight;

    #endregion

    #region 内部状态

    // ═══ 颜色状态 ═══
    private Color currentBaseColor = Color.white;
    private Color currentWeatherTint = Color.white;
    private Color targetWeatherTint = Color.white;
    private Color finalOverlayColor = Color.white;

    // ═══ 天气过渡 ═══
    private float weatherTransitionTimer;
    private float weatherTransitionDuration;
    private Color weatherTransitionStartTint;
    private bool isWeatherTransitioning;

    // ═══ 时间跳跃过渡 ═══
    private bool isTimeJumpTransitioning;
    private float timeJumpTransitionTimer;
    private Color timeJumpStartColor;
    private Color timeJumpTargetColor;

    // ═══ 季节过渡 ═══
    private bool isSeasonTransitioning;
    private float seasonTransitionTimer;
    private Gradient previousSeasonGradient;
    private Gradient currentSeasonGradient;

    // ═══ 路线 A/B 融合过渡 ═══
    private bool previousEnableRouteA;
    private bool isRouteTransitioning;
    private float routeTransitionTimer;
    private float routeTransitionStartStrength;
    private float routeTransitionTargetStrength;
    private const float ROUTE_TRANSITION_DURATION = 1.0f; // 路线切换过渡时长（秒）

#if USE_URP
    // ═══ 夜间光源状态 ═══
    private bool nightLightsActive = false;
#endif

    // ═══ 缓存 ═══
    private SeasonManager.Season cachedSeason;
    private float cachedDayProgress;
    private Transform cachedPlayerTransform;
    private NightLightMarker[] cachedNightLightMarkers = new NightLightMarker[0];
    private float nextNightLightRefreshTime;
    private DayNightOverlay.LightSample[] overlayNightLights = new DayNightOverlay.LightSample[8];

    #endregion

    #region 生命周期

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad 由 PersistentManagers 统一处理
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        EnsureDependencies();

        if (config == null || overlay == null)
        {
            Debug.LogWarning("[DayNightManager] 关键依赖缺失，组件已禁用");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // 订阅事件
        TimeManager.OnMinuteChanged += OnMinuteChanged;
        TimeManager.OnHourChanged += OnHourChanged;
        TimeManager.OnSeasonChanged += OnSeasonChanged;
        TimeManager.OnSleep += OnSleep;
        WeatherSystem.OnWeatherChanged += OnWeatherChanged;

        // 初始化缓存
        InitializeState();

        if (showDebugInfo)
        {
            Debug.Log("<color=cyan>[DayNightManager] 初始化完成</color>");
        }
    }

    private void OnDestroy()
    {
        // 取消所有订阅
        TimeManager.OnMinuteChanged -= OnMinuteChanged;
        TimeManager.OnHourChanged -= OnHourChanged;
        TimeManager.OnSeasonChanged -= OnSeasonChanged;
        TimeManager.OnSleep -= OnSleep;
        WeatherSystem.OnWeatherChanged -= OnWeatherChanged;

        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        bool needsUpdate = false;

        // 驱动天气过渡计时器
        if (isWeatherTransitioning)
        {
            weatherTransitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(weatherTransitionTimer / weatherTransitionDuration);
            currentWeatherTint = Color.Lerp(weatherTransitionStartTint, targetWeatherTint, t);

            if (t >= 1f)
            {
                isWeatherTransitioning = false;
                currentWeatherTint = targetWeatherTint;
            }
            needsUpdate = true;
        }

        // 驱动季节过渡计时器
        if (isSeasonTransitioning)
        {
            seasonTransitionTimer += Time.deltaTime;
            float duration = config != null ? config.seasonTransitionDuration : 2f;
            float t = Mathf.Clamp01(seasonTransitionTimer / duration);

            if (t >= 1f)
            {
                isSeasonTransitioning = false;
            }
            needsUpdate = true;
        }

        // 驱动时间跳跃过渡计时器
        if (isTimeJumpTransitioning)
        {
            timeJumpTransitionTimer += Time.deltaTime;
            float duration = config != null ? config.timeJumpTransitionDuration : 0.8f;
            float t = Mathf.Clamp01(timeJumpTransitionTimer / duration);

            if (t >= 1f)
            {
                isTimeJumpTransitioning = false;
            }
            needsUpdate = true;
        }

        // 驱动路线 A/B 切换过渡计时器
        if (isRouteTransitioning)
        {
            routeTransitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(routeTransitionTimer / ROUTE_TRANSITION_DURATION);
            float currentStrength = Mathf.Lerp(routeTransitionStartStrength, routeTransitionTargetStrength, t);

            if (overlay != null)
            {
                overlay.SetStrength(currentStrength);
            }

            if (t >= 1f)
            {
                isRouteTransitioning = false;
            }
            needsUpdate = true;
        }

        // 检测路线 A 开关变化（运行时切换）
        if (previousEnableRouteA != enableRouteA)
        {
            OnRouteAToggleChanged();
            previousEnableRouteA = enableRouteA;
        }

        if (SyncStateFromSources())
        {
            needsUpdate = true;
        }

        // 有过渡进行中时重新计算并更新 overlay
        if (needsUpdate)
        {
            RecalculateAndApply();
        }
    }

    #endregion

    #region 事件回调

    /// <summary>
    /// 每 10 分钟触发，根据当前时间更新光影
    /// </summary>
    private void OnMinuteChanged(int hour, int minute)
    {
        if (config == null) return;

        RefreshCachedDayProgress();
        RecalculateAndApply();
    }

    /// <summary>
    /// 整点触发，执行整点光影状态检查
    /// </summary>
    private void OnHourChanged(int hour)
    {
        // 更新路线 A 组件状态
        UpdateRouteAState();

        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[DayNightManager] 整点检查: {hour}:00, " +
                      $"夜晚={IsNightMode()}, 亮度={GetCurrentBrightness():F2}</color>");
        }
    }

    /// <summary>
    /// 季节变化回调，启动季节过渡
    /// </summary>
    private void OnSeasonChanged(SeasonManager.Season newSeason, int year)
    {
        if (config == null) return;

        // 保存旧季节 Gradient 用于过渡
        previousSeasonGradient = currentSeasonGradient;
        currentSeasonGradient = config.GetSeasonGradient(newSeason);
        cachedSeason = newSeason;

        // 启动季节过渡
        if (previousSeasonGradient != null)
        {
            isSeasonTransitioning = true;
            seasonTransitionTimer = 0f;
        }

        if (showDebugInfo)
        {
            Debug.Log($"<color=orange>[DayNightManager] 季节变化: {newSeason}，启动过渡</color>");
        }
    }

    /// <summary>
    /// 天气变化回调，启动天气过渡
    /// </summary>
    private void OnWeatherChanged(WeatherSystem.Weather weather)
    {
        if (config == null) return;

        // 记录过渡起点
        weatherTransitionStartTint = currentWeatherTint;

        // 确定目标天气修正色
        switch (weather)
        {
            case WeatherSystem.Weather.Rainy:
                targetWeatherTint = config.rainyTint;
                break;
            case WeatherSystem.Weather.Withering:
                targetWeatherTint = config.witheringTint;
                break;
            case WeatherSystem.Weather.Sunny:
            default:
                targetWeatherTint = Color.white; // 晴天无修正
                break;
        }

        // 启动天气过渡
        weatherTransitionDuration = config.weatherTransitionDuration;
        weatherTransitionTimer = 0f;
        isWeatherTransitioning = true;

        if (showDebugInfo)
        {
            Debug.Log($"<color=yellow>[DayNightManager] 天气变化: {weather}，启动过渡</color>");
        }
    }

    /// <summary>
    /// 睡觉事件回调，启动时间跳跃过渡
    /// </summary>
    private void OnSleep()
    {
        // 记录跳跃前的颜色
        timeJumpStartColor = finalOverlayColor;
        RefreshCachedDayProgress();
        isTimeJumpTransitioning = true;
        timeJumpTransitionTimer = 0f;
        RecalculateAndApply();

        if (showDebugInfo)
        {
            Debug.Log("<color=magenta>[DayNightManager] 检测到睡觉，启动时间跳跃过渡</color>");
        }
    }

    #endregion

    #region 颜色计算核心

    /// <summary>
    /// 初始化状态（首次启动时调用）
    /// </summary>
    private void InitializeState()
    {
        EnsureDependencies();

        if (TimeManager.Instance != null)
        {
            cachedSeason = TimeManager.Instance.GetSeason();
            cachedDayProgress = TimeManager.Instance.GetDayProgress();
        }
        else
        {
            cachedSeason = SeasonManager.Season.Spring;
            cachedDayProgress = 0f;
        }

        // 初始化季节 Gradient
        currentSeasonGradient = config.GetSeasonGradient(cachedSeason);
        previousSeasonGradient = currentSeasonGradient;

        // 初始化天气修正色
        if (WeatherSystem.Instance != null)
        {
            var weather = WeatherSystem.Instance.GetCurrentWeather();
            switch (weather)
            {
                case WeatherSystem.Weather.Rainy:
                    currentWeatherTint = config.rainyTint;
                    targetWeatherTint = config.rainyTint;
                    break;
                case WeatherSystem.Weather.Withering:
                    currentWeatherTint = config.witheringTint;
                    targetWeatherTint = config.witheringTint;
                    break;
                default:
                    currentWeatherTint = Color.white;
                    targetWeatherTint = Color.white;
                    break;
            }
        }

        // 初始化路线 A/B 融合
        previousEnableRouteA = enableRouteA;
        UpdateRouteAState();

        // 初始化夜间光源状态
        if (pointLightMgr != null && enableRouteA)
        {
            pointLightMgr.RefreshLightList();
        }

        // 首次计算并应用
        RecalculateAndApply();
    }

    /// <summary>
    /// 重新计算最终颜色并应用到 overlay
    /// </summary>
    private void RecalculateAndApply()
    {
        // 1. 计算基础色调
        Color baseColor = CalculateOverlayColor(cachedDayProgress);

        // 2. 叠加天气修正
        ApplyWeatherModifier(ref baseColor);

        // 3. 时间跳跃过渡处理
        if (isTimeJumpTransitioning)
        {
            float duration = config != null ? config.timeJumpTransitionDuration : 0.8f;
            float t = Mathf.Clamp01(timeJumpTransitionTimer / duration);
            // 在跳跃前颜色和当前计算颜色之间插值
            timeJumpTargetColor = baseColor;
            baseColor = Color.Lerp(timeJumpStartColor, timeJumpTargetColor, t);
        }

        // 4. 更新最终颜色
        finalOverlayColor = baseColor;
        currentBaseColor = baseColor;

        // 5. 应用到 overlay
        UpdateOverlay();

        // 6. 更新路线 A（如果启用）
        UpdateGlobalLight();
        UpdatePointLights();

        // 7. 更新调试字段
        debugFinalColor = finalOverlayColor;
        debugBrightness = GetCurrentBrightness();
        debugIsNight = IsNightMode();
    }

    /// <summary>
    /// 根据 dayProgress 计算基础叠加颜色（考虑季节过渡）
    /// </summary>
    private Color CalculateOverlayColor(float dayProgress)
    {
        if (config == null || currentSeasonGradient == null)
            return Color.white;

        // 季节过渡中：在两个季节 Gradient 之间插值
        if (isSeasonTransitioning && previousSeasonGradient != null)
        {
            float duration = config.seasonTransitionDuration;
            float t = Mathf.Clamp01(seasonTransitionTimer / duration);

            Color oldColor = previousSeasonGradient.Evaluate(dayProgress);
            Color newColor = currentSeasonGradient.Evaluate(dayProgress);
            return Color.Lerp(oldColor, newColor, t);
        }

        return currentSeasonGradient.Evaluate(dayProgress);
    }

    /// <summary>
    /// 叠加天气修正色到基础颜色上
    /// 晴天时 currentWeatherTint 为白色，Lerp 结果不变
    /// </summary>
    private void ApplyWeatherModifier(ref Color baseColor)
    {
        if (config == null) return;

        // 天气修正强度：晴天时 tint 为白色，Lerp 无效果
        float strength = config.weatherTintStrength;
        baseColor = ApplyWeatherTint(baseColor, currentWeatherTint, strength);
    }

    /// <summary>
    /// 将最终颜色应用到 DayNightOverlay
    /// </summary>
    private void UpdateOverlay()
    {
        if (overlay == null)
        {
            overlay = EnsureOverlayReference();
        }

        if (overlay == null) return;
        ApplyConfiguredOverlayStrength();
        overlay.SetColor(finalOverlayColor);
        ApplyOverlayEnvironment();
    }

    /// <summary>
    /// 更新路线 A/B 融合状态
    /// </summary>
    private void UpdateRouteAState()
    {
        if (config == null) return;

        bool routeAActive = enableRouteA;

        #if !USE_URP
        // URP 未安装时强制禁用路线 A
        if (routeAActive)
        {
            routeAActive = false;
            if (showDebugInfo)
            {
                Debug.LogWarning("[DayNightManager] URP 未安装，路线 A 已自动禁用");
            }
        }
        #endif

        // 设置 overlay 强度
        if (overlay != null)
        {
            float strength = routeAActive
                ? config.overlayStrengthWithURP
                : config.overlayStrengthWithoutURP;
            overlay.SetStrength(strength);
        }

        // 控制路线 A 组件启用状态
        if (globalLight != null)
        {
            globalLight.SetEnabled(routeAActive);
        }
        if (pointLightMgr != null)
        {
            pointLightMgr.enabled = routeAActive;
        }
    }

    #endregion

    #region 路线 A 控制方法

    /// <summary>
    /// 更新全局光源（路线 A）
    /// 根据 dayProgress 从 config 曲线读取强度和颜色，应用到 GlobalLightController
    /// </summary>
    private void UpdateGlobalLight()
    {
        if (!enableRouteA || globalLight == null || config == null) return;

#if USE_URP
        // 从配置曲线获取当前时间对应的光照强度
        float intensity = config.globalLightIntensityCurve.Evaluate(cachedDayProgress);
        globalLight.SetLightIntensity(intensity);

        // 从配置 Gradient 获取当前时间对应的光照颜色
        if (config.globalLightColorGradient != null)
        {
            Color lightColor = config.globalLightColorGradient.Evaluate(cachedDayProgress);
            globalLight.SetLightColor(lightColor);
        }
#endif
    }

    /// <summary>
    /// 更新局部光源（路线 A）
    /// 根据当前小时判断是否需要激活/关闭夜间光源
    /// </summary>
    private void UpdatePointLights()
    {
        if (!enableRouteA || pointLightMgr == null || config == null) return;

#if USE_URP
        if (TimeManager.Instance == null) return;

        int hour = TimeManager.Instance.GetHour();
        float activateHour = config.nightLightActivateHour;
        float deactivateHour = config.nightLightDeactivateHour;
        float fadeDuration = config.pointLightFadeDuration;

        // 判断当前是否应处于夜间光源激活时段
        // 例：activateHour=18, deactivateHour=6 → 18:00~05:59 为激活时段
        bool shouldBeActive;
        if (activateHour > deactivateHour)
        {
            // 跨午夜：18~6 → hour >= 18 || hour < 6
            shouldBeActive = (hour >= activateHour || hour < deactivateHour);
        }
        else
        {
            // 不跨午夜（异常配置兜底）
            shouldBeActive = (hour >= activateHour && hour < deactivateHour);
        }

        // 状态变化时触发淡入/淡出
        if (shouldBeActive && !nightLightsActive)
        {
            nightLightsActive = true;
            pointLightMgr.ActivateNightLights(fadeDuration);

            if (showDebugInfo)
            {
                Debug.Log($"<color=yellow>[DayNightManager] 夜间光源激活 ({hour}:00)</color>");
            }
        }
        else if (!shouldBeActive && nightLightsActive)
        {
            nightLightsActive = false;
            pointLightMgr.DeactivateNightLights(fadeDuration);

            if (showDebugInfo)
            {
                Debug.Log($"<color=yellow>[DayNightManager] 夜间光源关闭 ({hour}:00)</color>");
            }
        }
#endif
    }

    /// <summary>
    /// 路线 A 开关切换时的平滑过渡处理
    /// 在 overlay 强度之间做平滑插值，避免视觉跳变
    /// </summary>
    private void OnRouteAToggleChanged()
    {
        if (config == null) return;

        bool routeAActive = enableRouteA;

#if !USE_URP
        // URP 未安装时强制禁用路线 A
        if (routeAActive)
        {
            routeAActive = false;
            enableRouteA = false;
            if (showDebugInfo)
            {
                Debug.LogWarning("[DayNightManager] URP 未安装，路线 A 已自动禁用");
            }
            return;
        }
#endif

        // 启动 overlay 强度平滑过渡
        if (overlay != null)
        {
            routeTransitionStartStrength = routeAActive
                ? config.overlayStrengthWithoutURP  // 从无 URP 强度开始
                : config.overlayStrengthWithURP;    // 从有 URP 强度开始
            routeTransitionTargetStrength = routeAActive
                ? config.overlayStrengthWithURP     // 过渡到有 URP 强度
                : config.overlayStrengthWithoutURP; // 过渡到无 URP 强度

            isRouteTransitioning = true;
            routeTransitionTimer = 0f;
        }

        // 控制路线 A 组件启用状态
        if (globalLight != null)
        {
            globalLight.SetEnabled(routeAActive);
        }
        if (pointLightMgr != null)
        {
            pointLightMgr.enabled = routeAActive;

            // 激活时刷新光源列表
            if (routeAActive)
            {
                pointLightMgr.RefreshLightList();
            }
        }

        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[DayNightManager] 路线 A {(routeAActive ? "启用" : "禁用")}，启动平滑过渡</color>");
        }
    }

    #endregion

    #region 公共查询接口

    /// <summary>
    /// 获取当前最终叠加颜色
    /// </summary>
    public Color GetCurrentOverlayColor()
    {
        return finalOverlayColor;
    }

    /// <summary>
    /// 获取当前亮度（颜色灰度值，0=全黑, 1=全白）
    /// 使用标准灰度公式：0.299R + 0.587G + 0.114B
    /// </summary>
    public float GetCurrentBrightness()
    {
        return 0.299f * finalOverlayColor.r +
               0.587f * finalOverlayColor.g +
               0.114f * finalOverlayColor.b;
    }

    /// <summary>
    /// 是否为夜晚模式（委托给 TimeManager）
    /// </summary>
    public bool IsNightMode()
    {
        if (TimeManager.Instance != null)
        {
            return TimeManager.Instance.IsNighttime();
        }
        return false;
    }

    /// <summary>
    /// 调整叠加强度（手动覆盖路线融合逻辑）
    /// </summary>
    public void SetOverlayStrength(float strength)
    {
        if (overlay != null)
        {
            overlay.SetStrength(strength);
        }
    }

    #endregion

    #region 静态测试辅助方法

    /// <summary>
    /// 纯函数：根据季节 Gradient 和 dayProgress 计算基础色调
    /// 可在单元测试中直接调用，无需 MonoBehaviour 实例
    /// </summary>
    public static Color CalculateColor(Gradient seasonGradient, float dayProgress)
    {
        if (seasonGradient == null) return Color.white;
        return seasonGradient.Evaluate(dayProgress);
    }

    /// <summary>
    /// 纯函数：在基础色和天气修正色之间按强度插值
    /// 可在单元测试中直接调用，无需 MonoBehaviour 实例
    /// </summary>
    public static Color ApplyWeatherTint(Color baseColor, Color weatherTint, float tintStrength)
    {
        // 晴天（tint 接近白色）时不做修正
        bool isSunny = (weatherTint.r > 0.99f &&
                        weatherTint.g > 0.99f &&
                        weatherTint.b > 0.99f);

        if (isSunny) return baseColor;

        float clampedStrength = Mathf.Clamp01(tintStrength);
        Color clampedTint = new Color(
            Mathf.Clamp01(weatherTint.r),
            Mathf.Clamp01(weatherTint.g),
            Mathf.Clamp01(weatherTint.b),
            baseColor.a);
        Color limitedTint = new Color(
            baseColor.r * clampedTint.r,
            baseColor.g * clampedTint.g,
            baseColor.b * clampedTint.b,
            baseColor.a);

        return Color.Lerp(baseColor, limitedTint, clampedStrength);
    }

    private void RefreshCachedDayProgress()
    {
        cachedDayProgress = TimeManager.Instance != null ? TimeManager.Instance.GetDayProgress() : 0f;
    }

    private void EnsureDependencies()
    {
        EnsureOverlayNightLightCapacity();

        if (config == null)
        {
            config = TryResolveConfig();
        }
        else
        {
            config = PrepareRuntimeConfig(config);
        }

        if (overlay == null)
        {
            overlay = EnsureOverlayReference();
        }

        ApplyConfiguredOverlayStrength();
    }

    private DayNightConfig TryResolveConfig()
    {
        DayNightConfig resolved = AssetLocator.LoadDayNightConfig(DefaultConfigResourcesPath);
        if (resolved != null)
        {
            return PrepareRuntimeConfig(resolved);
        }

        return PrepareRuntimeConfig(CreateFallbackConfig());
    }

    private DayNightConfig PrepareRuntimeConfig(DayNightConfig source)
    {
        if (source == null)
        {
            return null;
        }

        if (source.name.StartsWith(RuntimeConfigNamePrefix, StringComparison.Ordinal))
        {
            return source;
        }

        DayNightConfig runtimeConfig = Instantiate(source);
        runtimeConfig.name = RuntimeConfigNamePrefix + source.name;
        runtimeConfig.hideFlags = HideFlags.DontSave;

        if (ShouldUpgradeLegacyConfig(runtimeConfig))
        {
            ApplyRecommendedBaseline(runtimeConfig);
        }

        return runtimeConfig;
    }

    private DayNightOverlay EnsureOverlayReference()
    {
        DayNightOverlay existing = GetComponentInChildren<DayNightOverlay>(true);
        if (existing != null)
        {
            existing.gameObject.SetActive(true);
            existing.transform.localPosition = Vector3.zero;
            return existing;
        }

        GameObject overlayObject = new GameObject("DayNightOverlay");
        overlayObject.transform.SetParent(transform, false);
        return overlayObject.AddComponent<DayNightOverlay>();
    }

    private bool SyncStateFromSources()
    {
        bool changed = false;

        if (TimeManager.Instance != null)
        {
            float liveDayProgress = TimeManager.Instance.GetDayProgress();
            if (!Mathf.Approximately(cachedDayProgress, liveDayProgress))
            {
                cachedDayProgress = liveDayProgress;
                changed = true;
            }

            SeasonManager.Season liveSeason = TimeManager.Instance.GetSeason();
            if (liveSeason != cachedSeason)
            {
                cachedSeason = liveSeason;
                currentSeasonGradient = config != null ? config.GetSeasonGradient(liveSeason) : currentSeasonGradient;
                previousSeasonGradient = currentSeasonGradient;
                isSeasonTransitioning = false;
                changed = true;
            }
        }

        if (!isWeatherTransitioning && config != null && WeatherSystem.Instance != null)
        {
            Color liveWeatherTint = GetWeatherTint(WeatherSystem.Instance.GetCurrentWeather());
            if (!ColorsApproximatelyEqual(currentWeatherTint, liveWeatherTint))
            {
                currentWeatherTint = liveWeatherTint;
                targetWeatherTint = liveWeatherTint;
                changed = true;
            }
        }

        return changed;
    }

    private Color GetWeatherTint(WeatherSystem.Weather weather)
    {
        if (config == null)
        {
            return Color.white;
        }

        switch (weather)
        {
            case WeatherSystem.Weather.Rainy:
                return config.rainyTint;
            case WeatherSystem.Weather.Withering:
                return config.witheringTint;
            default:
                return Color.white;
        }
    }

    private static bool ColorsApproximatelyEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }

    private void ApplyConfiguredOverlayStrength()
    {
        if (overlay == null || config == null)
        {
            return;
        }

        overlay.SetStrength(GetDesiredOverlayStrength());
    }

    private float GetDesiredOverlayStrength()
    {
        if (config == null)
        {
            return 1f;
        }

        return enableRouteA ? config.overlayStrengthWithURP : config.overlayStrengthWithoutURP;
    }

    private void ApplyOverlayEnvironment()
    {
        if (overlay == null)
        {
            return;
        }

        overlay.SetVisionAspect(visionAspect);
        overlay.SetVisionFocus(ResolvePlayerTransform());

        float visionStrength = enableNightVision ? EvaluateNightVisionStrength() : 0f;
        float radiusNormalized = Mathf.Lerp(dayVisionRadiusNormalized, nightVisionRadiusNormalized, visionStrength);
        float outerDarkness = visionOuterDarkness * visionStrength;
        overlay.SetVisionProfile(radiusNormalized, visionSoftness, visionStrength, outerDarkness);

        if (!enableNightLightOverlay)
        {
            overlay.SetNightLights(overlayNightLights, 0);
            return;
        }

        float nightLightStrength = EvaluateNightLightStrength();
        int lightCount = BuildOverlayNightLights(nightLightStrength);
        overlay.SetNightLights(overlayNightLights, lightCount);
    }

    private Transform ResolvePlayerTransform()
    {
        if (cachedPlayerTransform != null)
        {
            return cachedPlayerTransform;
        }

        GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            cachedPlayerTransform = taggedPlayer.transform;
            return cachedPlayerTransform;
        }

        GameObject[] taggedPlayers = GameObject.FindGameObjectsWithTag("Player");
        if (taggedPlayers != null && taggedPlayers.Length > 0)
        {
            cachedPlayerTransform = taggedPlayers[0].transform;
        }

        return cachedPlayerTransform;
    }

    private float EvaluateNightVisionStrength()
    {
        float currentHour = GetCurrentTimeAsHourFloat();

        if (currentHour >= duskStartHour && currentHour < fullNightHour)
        {
            return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(duskStartHour, fullNightHour, currentHour));
        }

        if (currentHour >= fullNightHour || currentHour < dawnRecoverHour)
        {
            return 1f;
        }

        if (currentHour >= dawnRecoverHour && currentHour < dawnClearHour)
        {
            return Mathf.Lerp(dawnResidualVisionStrength, 0f, Mathf.InverseLerp(dawnRecoverHour, dawnClearHour, currentHour));
        }

        return 0f;
    }

    private float EvaluateNightLightStrength()
    {
        float currentHour = GetCurrentTimeAsHourFloat();

        if (currentHour >= duskStartHour && currentHour < fullNightHour)
        {
            return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(duskStartHour, fullNightHour, currentHour));
        }

        if (currentHour >= fullNightHour || currentHour < dawnRecoverHour)
        {
            return 1f;
        }

        if (currentHour >= dawnRecoverHour && currentHour < dawnClearHour)
        {
            return Mathf.Lerp(0.3f, 0f, Mathf.InverseLerp(dawnRecoverHour, dawnClearHour, currentHour));
        }

        return 0f;
    }

    private float GetCurrentTimeAsHourFloat()
    {
        if (TimeManager.Instance == null)
        {
            return 12f;
        }

        return TimeManager.Instance.GetHour() + TimeManager.Instance.GetMinute() / 60f;
    }

    private int BuildOverlayNightLights(float lightStrength)
    {
        EnsureOverlayNightLightCapacity();

        if (lightStrength <= 0.001f)
        {
            return 0;
        }

        RefreshNightLightMarkersIfNeeded();
        int maxCount = Mathf.Min(maxOverlayNightLights, overlayNightLights.Length);
        int count = 0;
        if (cachedNightLightMarkers != null)
        {
            for (int i = 0; i < cachedNightLightMarkers.Length && count < maxCount; i++)
            {
                NightLightMarker marker = cachedNightLightMarkers[i];
                if (marker == null || !marker.isActiveAndEnabled)
                {
                    continue;
                }

                float animatedIntensity = ComputeAnimatedLightIntensity(marker, lightStrength);
                float animatedRadius = ComputeAnimatedLightRadius(marker);
                Vector2 animatedPosition = ComputeAnimatedLightPosition(marker);
                Color lampColor = Color.Lerp(Color.white, marker.LightColor, nightLightWarmth);
                overlayNightLights[count] = new DayNightOverlay.LightSample
                {
                    position = animatedPosition,
                    color = lampColor,
                    radius = Mathf.Max(0.5f, animatedRadius * 1.12f),
                    feather = Mathf.Clamp01(marker.Feather),
                    intensity = Mathf.Clamp01(animatedIntensity),
                    coreRatio = Mathf.Clamp01(Mathf.Lerp(0.22f, 0.38f, 1f - marker.Feather))
                };
                count++;
            }
        }

        return count;
    }

    private void RefreshNightLightMarkersIfNeeded()
    {
        if (Application.isPlaying)
        {
            if (Time.time < nextNightLightRefreshTime)
            {
                return;
            }

            nextNightLightRefreshTime = Time.time + Mathf.Max(0.2f, nightLightSearchInterval);
        }

        cachedNightLightMarkers = FindObjectsByType<NightLightMarker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private float ComputeAnimatedLightIntensity(NightLightMarker marker, float lightStrength)
    {
        float baseIntensity = marker.MaxIntensity * marker.OverlayWeight * nightLightIntensityScale * lightStrength;
        if (!Application.isPlaying)
        {
            return baseIntensity;
        }

        float time = Time.time + marker.AnimationSeed;
        float pulseSpeed = Mathf.Max(0.01f, marker.PulseSpeed);
        float slowWave = Mathf.Sin(time * pulseSpeed * Mathf.PI * 2f);
        float fastWave = Mathf.Sin(time * (pulseSpeed * 2.17f + 0.36f) * Mathf.PI * 2f + 0.9f);
        float emberWave = Mathf.Sin(time * (pulseSpeed * 3.45f + 0.51f) * Mathf.PI * 2f + 1.8f);
        float noiseWave = (Mathf.PerlinNoise(marker.AnimationSeed * 1.37f, time * (0.55f + pulseSpeed * 0.24f)) - 0.5f) * 2f;
        float pulseOffset = (slowWave * 0.42f + fastWave * 0.22f + emberWave * 0.16f + noiseWave * 0.20f) * marker.PulseAmount;
        return baseIntensity * Mathf.Max(0.2f, 1f + pulseOffset);
    }

    private float ComputeAnimatedLightRadius(NightLightMarker marker)
    {
        float baseRadius = Mathf.Max(0.5f, marker.Radius);
        if (!Application.isPlaying)
        {
            return baseRadius;
        }

        float time = Time.time + marker.AnimationSeed;
        float pulseSpeed = Mathf.Max(0.01f, marker.PulseSpeed);
        float radiusWave = Mathf.Sin(time * (pulseSpeed * 0.74f + 0.19f) * Mathf.PI * 2f);
        float secondaryWave = Mathf.Sin(time * (pulseSpeed * 1.21f + 0.07f) * Mathf.PI * 2f + 1.2f);
        float radiusOffset = (radiusWave * 0.7f + secondaryWave * 0.3f) * Mathf.Min(0.14f, marker.PulseAmount * 0.32f);
        return baseRadius * Mathf.Max(0.72f, 1f + radiusOffset);
    }

    private Vector2 ComputeAnimatedLightPosition(NightLightMarker marker)
    {
        Vector3 basePosition = marker.transform.position;
        if (!Application.isPlaying || marker.SwayAmplitude <= 0.0001f)
        {
            return basePosition;
        }

        float time = Time.time + marker.AnimationSeed;
        float swaySpeed = Mathf.Max(0.01f, marker.SwaySpeed);
        float swayX = Mathf.Sin(time * swaySpeed * Mathf.PI * 2f) * marker.SwayAmplitude;
        float swayY = Mathf.Cos(time * (swaySpeed * 0.63f + 0.11f) * Mathf.PI * 2f) * marker.SwayAmplitude * 0.4f;
        float emberJitter = (Mathf.PerlinNoise(marker.AnimationSeed * 0.73f, time * (1.1f + swaySpeed * 0.35f)) - 0.5f) * marker.SwayAmplitude * 0.22f;
        return new Vector2(basePosition.x + swayX + emberJitter, basePosition.y + swayY);
    }

    private void EnsureOverlayNightLightCapacity()
    {
        int desiredCapacity = Mathf.Clamp(maxOverlayNightLights, 1, 8);
        if (overlayNightLights == null || overlayNightLights.Length != desiredCapacity)
        {
            overlayNightLights = new DayNightOverlay.LightSample[desiredCapacity];
        }
    }

    private bool ShouldUpgradeLegacyConfig(DayNightConfig candidate)
    {
        if (candidate == null)
        {
            return false;
        }

        if (candidate.springGradient == null ||
            candidate.summerGradient == null ||
            candidate.autumnGradient == null ||
            candidate.winterGradient == null)
        {
            return true;
        }

        float noonBrightness = GetBrightness(candidate.springGradient.Evaluate(0.30f));
        float afternoonBrightness = GetBrightness(candidate.springGradient.Evaluate(0.50f));

        bool dayPlateauMissing = noonBrightness - afternoonBrightness > 0.05f;
        bool routeBTooHeavy = candidate.overlayStrengthWithoutURP >= 0.98f;
        bool routeAMixTooStrong = candidate.overlayStrengthWithURP >= 0.39f;

        return dayPlateauMissing || routeBTooHeavy || routeAMixTooStrong;
    }

    private void ApplyRecommendedBaseline(DayNightConfig target)
    {
        if (target == null)
        {
            return;
        }

        target.springGradient = CreateSpringGradient();
        target.summerGradient = CreateSummerGradient();
        target.autumnGradient = CreateAutumnGradient();
        target.winterGradient = CreateWinterGradient();
        target.rainyTint = new Color(0.55f, 0.55f, 0.6f, 1f);
        target.witheringTint = new Color(0.85f, 0.75f, 0.45f, 1f);
        target.weatherTintStrength = 0.5f;
        target.weatherTransitionDuration = 1.5f;
        target.timeJumpTransitionDuration = 0.8f;
        target.seasonTransitionDuration = 2f;
        target.globalLightIntensityCurve = CreateGlobalLightIntensityCurve();
        target.globalLightColorGradient = CreateGlobalLightColorGradient();
        target.nightLightActivateHour = 18f;
        target.nightLightDeactivateHour = 6f;
        target.pointLightFadeDuration = 1f;
        target.overlayStrengthWithURP = 0.35f;
        target.overlayStrengthWithoutURP = 0.92f;
    }

    private DayNightConfig CreateFallbackConfig()
    {
        DayNightConfig fallback = ScriptableObject.CreateInstance<DayNightConfig>();
        fallback.name = RuntimeConfigNamePrefix + "Fallback_DayNightConfig";
        fallback.hideFlags = HideFlags.DontSave;
        ApplyRecommendedBaseline(fallback);
        return fallback;
    }

    private static Gradient CreateSpringGradient()
    {
        return CreateGradient(
            new[]
            {
                new GradientColorKey(new Color(0.76f, 0.78f, 0.82f, 1f), 0.00f),
                new GradientColorKey(new Color(0.98f, 0.98f, 0.95f, 1f), 0.30f),
                new GradientColorKey(new Color(0.98f, 0.98f, 0.95f, 1f), 0.40f),
                new GradientColorKey(new Color(0.95f, 0.93f, 0.86f, 1f), 0.50f),
                new GradientColorKey(new Color(0.86f, 0.76f, 0.60f, 1f), 0.60f),
                new GradientColorKey(new Color(0.33f, 0.38f, 0.52f, 1f), 0.80f),
                new GradientColorKey(new Color(0.24f, 0.27f, 0.42f, 1f), 1.00f)
            });
    }

    private static Gradient CreateSummerGradient()
    {
        return CreateGradient(
            new[]
            {
                new GradientColorKey(new Color(0.80f, 0.78f, 0.76f, 1f), 0.00f),
                new GradientColorKey(new Color(1.00f, 0.98f, 0.95f, 1f), 0.30f),
                new GradientColorKey(new Color(1.00f, 0.98f, 0.95f, 1f), 0.40f),
                new GradientColorKey(new Color(0.97f, 0.93f, 0.86f, 1f), 0.50f),
                new GradientColorKey(new Color(0.88f, 0.72f, 0.54f, 1f), 0.60f),
                new GradientColorKey(new Color(0.31f, 0.35f, 0.50f, 1f), 0.80f),
                new GradientColorKey(new Color(0.22f, 0.24f, 0.40f, 1f), 1.00f)
            });
    }

    private static Gradient CreateAutumnGradient()
    {
        return CreateGradient(
            new[]
            {
                new GradientColorKey(new Color(0.74f, 0.68f, 0.62f, 1f), 0.00f),
                new GradientColorKey(new Color(0.98f, 0.95f, 0.90f, 1f), 0.30f),
                new GradientColorKey(new Color(0.98f, 0.95f, 0.90f, 1f), 0.40f),
                new GradientColorKey(new Color(0.95f, 0.88f, 0.76f, 1f), 0.50f),
                new GradientColorKey(new Color(0.82f, 0.62f, 0.40f, 1f), 0.60f),
                new GradientColorKey(new Color(0.32f, 0.30f, 0.46f, 1f), 0.80f),
                new GradientColorKey(new Color(0.24f, 0.23f, 0.39f, 1f), 1.00f)
            });
    }

    private static Gradient CreateWinterGradient()
    {
        return CreateGradient(
            new[]
            {
                new GradientColorKey(new Color(0.64f, 0.70f, 0.82f, 1f), 0.00f),
                new GradientColorKey(new Color(0.95f, 0.95f, 0.98f, 1f), 0.30f),
                new GradientColorKey(new Color(0.95f, 0.95f, 0.98f, 1f), 0.40f),
                new GradientColorKey(new Color(0.88f, 0.90f, 0.95f, 1f), 0.50f),
                new GradientColorKey(new Color(0.62f, 0.58f, 0.78f, 1f), 0.60f),
                new GradientColorKey(new Color(0.28f, 0.26f, 0.46f, 1f), 0.80f),
                new GradientColorKey(new Color(0.20f, 0.20f, 0.36f, 1f), 1.00f)
            });
    }

    private static AnimationCurve CreateGlobalLightIntensityCurve()
    {
        return new AnimationCurve(
            new Keyframe(0.00f, 0.7f),
            new Keyframe(0.15f, 0.9f),
            new Keyframe(0.30f, 1.0f),
            new Keyframe(0.45f, 0.9f),
            new Keyframe(0.60f, 0.6f),
            new Keyframe(0.80f, 0.3f),
            new Keyframe(1.00f, 0.2f));
    }

    private static Gradient CreateGlobalLightColorGradient()
    {
        return CreateGradient(
            new[]
            {
                new GradientColorKey(new Color(1.0f, 0.85f, 0.65f, 1f), 0.00f),
                new GradientColorKey(new Color(1.0f, 0.98f, 0.95f, 1f), 0.30f),
                new GradientColorKey(new Color(1.0f, 0.80f, 0.55f, 1f), 0.60f),
                new GradientColorKey(new Color(0.50f, 0.55f, 0.80f, 1f), 0.80f),
                new GradientColorKey(new Color(0.40f, 0.42f, 0.70f, 1f), 1.00f)
            });
    }

    private static Gradient CreateGradient(GradientColorKey[] colorKeys)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            colorKeys,
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            });
        return gradient;
    }

    private static float GetBrightness(Color color)
    {
        return 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
    }

    #endregion

    #region ContextMenu 调试方法

    [ContextMenu("🌅 预览 06:00（日出）")]
    private void DebugPreview0600()
    {
        DebugPreviewTime(0f); // dayProgress=0 → 06:00
    }

    [ContextMenu("☀️ 预览 12:00（正午）")]
    private void DebugPreview1200()
    {
        DebugPreviewTime(0.3f); // dayProgress=0.3 → 12:00
    }

    [ContextMenu("🌆 预览 18:00（日落）")]
    private void DebugPreview1800()
    {
        DebugPreviewTime(0.6f); // dayProgress=0.6 → 18:00
    }

    [ContextMenu("🌙 预览 22:00（深夜）")]
    private void DebugPreview2200()
    {
        DebugPreviewTime(0.8f); // dayProgress=0.8 → 22:00
    }

    [ContextMenu("🌌 预览 02:00（凌晨）")]
    private void DebugPreview0200()
    {
        DebugPreviewTime(1.0f); // dayProgress=1.0 → 02:00
    }

    /// <summary>
    /// 调试用：预览指定 dayProgress 的颜色效果
    /// </summary>
    private void DebugPreviewTime(float dayProgress)
    {
        if (config == null)
        {
            Debug.LogWarning("[DayNightManager] config 为空，无法预览");
            return;
        }

        cachedDayProgress = dayProgress;
        RecalculateAndApply();

        Debug.Log($"<color=cyan>[DayNightManager] 预览 dayProgress={dayProgress:F2}, " +
                  $"颜色={finalOverlayColor}, 亮度={GetCurrentBrightness():F2}</color>");
    }

    #endregion
}
