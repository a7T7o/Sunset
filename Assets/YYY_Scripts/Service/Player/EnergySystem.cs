using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 精力系统 - 管理玩家精力值
/// 工具使用成功时消耗精力，武器使用不消耗精力
/// </summary>
public class EnergySystem : MonoBehaviour
{
    #region 常量
    private const int DEFAULT_MAX_ENERGY = 150;
    #endregion

    #region 静态成员
    public static EnergySystem Instance { get; private set; }

    /// <summary>
    /// 精力变化事件 (当前值, 最大值)
    /// </summary>
    public static event Action<int, int> OnEnergyChanged;

    /// <summary>
    /// 精力耗尽事件
    /// </summary>
    public static event Action OnEnergyDepleted;
    #endregion

    #region 序列化字段
    [Header("=== 精力配置 ===")]
    [Tooltip("最大精力值")]
    [SerializeField] private int maxEnergy = DEFAULT_MAX_ENERGY;

    [Tooltip("当前精力值")]
    [SerializeField] private int currentEnergy;

    [Header("=== UI 引用 ===")]
    [Tooltip("精力条 Slider (UI/State/EP)")]
    [SerializeField] private Slider energySlider;

    [Header("=== 剧情表现 ===")]
    [SerializeField] private float revealDuration = 0.25f;
    [SerializeField] private float restoreFillDuration = 0.9f;
    [SerializeField] private float lowEnergyPulseSpeed = 4f;
    [SerializeField] private Color defaultFillColor = new Color(0.33f, 0.58f, 0.95f, 1f);
    [SerializeField] private Color restoreFlashColor = new Color(0.68f, 0.86f, 1f, 1f);
    [SerializeField] private Color lowEnergyPulseColorA = new Color(0.95f, 0.34f, 0.34f, 1f);
    [SerializeField] private Color lowEnergyPulseColorB = new Color(0.65f, 0.12f, 0.12f, 1f);

    [Header("=== 调试 ===")]
    [SerializeField] private bool showDebugInfo = false;
    #endregion

    #region 公共属性
    public int MaxEnergy => maxEnergy;
    public int CurrentEnergy => currentEnergy;
    public float EnergyPercent => maxEnergy > 0 ? (float)currentEnergy / maxEnergy : 0f;
    public bool IsExhausted => currentEnergy <= 0;
    public bool IsVisible => energySlider != null && energySlider.gameObject.activeSelf;
    #endregion

    #region 私有字段
    private Coroutine presentationCoroutine;
    private CanvasGroup energyCanvasGroup;
    private Image energyFillImage;
    private bool lowEnergyWarningVisual;
    private float restoreHighlightUntil;
    #endregion

    #region Unity生命周期
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentEnergy = maxEnergy;
    }

    private void Start()
    {
        if (energySlider == null)
        {
            TryFindEnergySlider();
        }

        UpdateUI();
    }

    private void Update()
    {
        UpdateFillVisual();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 尝试消耗精力（成功操作时调用）
    /// </summary>
    public bool TryConsumeEnergy(int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (currentEnergy < amount)
        {
            if (showDebugInfo)
            {
                Debug.Log($"<color=yellow>[EnergySystem] 精力不足！需要 {amount}，当前 {currentEnergy}</color>");
            }

            return false;
        }

        currentEnergy -= amount;

        if (showDebugInfo)
        {
            Debug.Log($"<color=cyan>[EnergySystem] 消耗精力 {amount}，剩余 {currentEnergy}/{maxEnergy}</color>");
        }

        UpdateUI();
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (currentEnergy <= 0)
        {
            OnEnergyDepleted?.Invoke();
            if (showDebugInfo)
            {
                Debug.Log("<color=red>[EnergySystem] 精力耗尽！</color>");
            }
        }

        return true;
    }

    /// <summary>
    /// 恢复精力
    /// </summary>
    public void RestoreEnergy(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int oldEnergy = currentEnergy;
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);

        if (showDebugInfo)
        {
            Debug.Log($"<color=green>[EnergySystem] 恢复精力 {currentEnergy - oldEnergy}，当前 {currentEnergy}/{maxEnergy}</color>");
        }

        UpdateUI();
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    public Coroutine PlayRevealAndAnimateTo(int start, int target, int max, float? customRevealDuration = null, float? customFillDuration = null)
    {
        StopPresentationCoroutine();
        presentationCoroutine = StartCoroutine(RevealAndAnimateToRoutine(
            start,
            target,
            max,
            Mathf.Max(0f, customRevealDuration ?? revealDuration),
            Mathf.Max(0f, customFillDuration ?? restoreFillDuration),
            false));
        return presentationCoroutine;
    }

    public Coroutine PlayRestoreAnimation(int amount, float? customDuration = null)
    {
        if (amount <= 0)
        {
            return null;
        }

        StopPresentationCoroutine();
        int targetEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        presentationCoroutine = StartCoroutine(RevealAndAnimateToRoutine(
            currentEnergy,
            targetEnergy,
            maxEnergy,
            0f,
            Mathf.Max(0f, customDuration ?? restoreFillDuration),
            true));
        return presentationCoroutine;
    }

    /// <summary>
    /// 完全恢复精力（睡觉时调用）
    /// </summary>
    public void FullRestore()
    {
        currentEnergy = maxEnergy;

        if (showDebugInfo)
        {
            Debug.Log($"<color=green>[EnergySystem] 精力完全恢复！{currentEnergy}/{maxEnergy}</color>");
        }

        UpdateUI();
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// 设置最大精力值
    /// </summary>
    public void SetMaxEnergy(int newMax)
    {
        maxEnergy = Mathf.Max(1, newMax);
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        UpdateUI();
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// 直接设置精力上限与当前值
    /// </summary>
    public void SetEnergyState(int current, int max)
    {
        maxEnergy = Mathf.Max(1, max);
        currentEnergy = Mathf.Clamp(current, 0, maxEnergy);
        UpdateUI();
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
    }

    /// <summary>
    /// 控制精力条显隐
    /// </summary>
    public void SetVisible(bool visible)
    {
        EnsureEnergyUiReferences();

        if (energySlider != null)
        {
            energySlider.gameObject.SetActive(visible);
        }

        if (energyCanvasGroup != null)
        {
            energyCanvasGroup.alpha = visible ? 1f : 0f;
        }
    }

    public void SetLowEnergyWarningVisual(bool enabled)
    {
        lowEnergyWarningVisual = enabled;
        if (!enabled && restoreHighlightUntil <= Time.unscaledTime)
        {
            ApplyFillColor(defaultFillColor);
        }
    }

    /// <summary>
    /// 检查是否有足够精力
    /// </summary>
    public bool HasEnoughEnergy(int amount)
    {
        return currentEnergy >= amount;
    }
    #endregion

    #region 私有方法
    private void UpdateUI()
    {
        EnsureEnergyUiReferences();

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }

    private IEnumerator RevealAndAnimateToRoutine(int start, int target, int max, float revealSeconds, float fillSeconds, bool highlightRestore)
    {
        EnsureEnergyUiReferences();

        maxEnergy = Mathf.Max(1, max);
        currentEnergy = Mathf.Clamp(start, 0, maxEnergy);

        if (energySlider == null)
        {
            currentEnergy = Mathf.Clamp(target, 0, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            presentationCoroutine = null;
            yield break;
        }

        energySlider.gameObject.SetActive(true);
        energySlider.maxValue = maxEnergy;
        energySlider.value = currentEnergy;
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        if (energyCanvasGroup != null)
        {
            energyCanvasGroup.interactable = false;
            energyCanvasGroup.blocksRaycasts = false;
            energyCanvasGroup.alpha = revealSeconds > 0f ? 0f : 1f;
        }

        if (highlightRestore)
        {
            restoreHighlightUntil = Time.unscaledTime + fillSeconds + 0.2f;
        }

        if (revealSeconds > 0f && energyCanvasGroup != null)
        {
            float revealElapsed = 0f;
            while (revealElapsed < revealSeconds)
            {
                revealElapsed += Time.unscaledDeltaTime;
                energyCanvasGroup.alpha = Mathf.Clamp01(revealElapsed / revealSeconds);
                yield return null;
            }

            energyCanvasGroup.alpha = 1f;
        }

        if (fillSeconds <= 0f)
        {
            currentEnergy = Mathf.Clamp(target, 0, maxEnergy);
            energySlider.value = currentEnergy;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            presentationCoroutine = null;
            yield break;
        }

        float startValue = currentEnergy;
        float clampedTarget = Mathf.Clamp(target, 0, maxEnergy);
        float fillElapsed = 0f;
        int lastReportedEnergy = currentEnergy;
        while (fillElapsed < fillSeconds)
        {
            fillElapsed += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(fillElapsed / fillSeconds);
            float eased = 1f - ((1f - normalized) * (1f - normalized));
            float currentValue = Mathf.Lerp(startValue, clampedTarget, eased);
            int roundedValue = Mathf.RoundToInt(currentValue);

            energySlider.value = currentValue;
            if (roundedValue != lastReportedEnergy)
            {
                currentEnergy = roundedValue;
                lastReportedEnergy = roundedValue;
                OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            }

            yield return null;
        }

        currentEnergy = Mathf.Clamp(Mathf.RoundToInt(clampedTarget), 0, maxEnergy);
        energySlider.value = currentEnergy;
        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        presentationCoroutine = null;
    }

    private void EnsureEnergyUiReferences()
    {
        if (energySlider == null)
        {
            TryFindEnergySlider();
        }

        if (energySlider == null)
        {
            return;
        }

        if (energyCanvasGroup == null)
        {
            energyCanvasGroup = energySlider.GetComponent<CanvasGroup>();
            if (energyCanvasGroup == null)
            {
                energyCanvasGroup = energySlider.gameObject.AddComponent<CanvasGroup>();
            }
        }

        energyCanvasGroup.interactable = false;
        energyCanvasGroup.blocksRaycasts = false;

        if (energyFillImage == null)
        {
            if (energySlider.fillRect != null)
            {
                energyFillImage = energySlider.fillRect.GetComponent<Image>();
            }

            if (energyFillImage == null)
            {
                Transform fillTransform = energySlider.transform.Find("Fill Area/Fill");
                if (fillTransform != null)
                {
                    energyFillImage = fillTransform.GetComponent<Image>();
                }
            }
        }
    }

    private void UpdateFillVisual()
    {
        if (energyFillImage == null)
        {
            EnsureEnergyUiReferences();
        }

        if (energyFillImage == null)
        {
            return;
        }

        if (restoreHighlightUntil > Time.unscaledTime)
        {
            float pulse = Mathf.PingPong(Time.unscaledTime * lowEnergyPulseSpeed, 1f);
            ApplyFillColor(Color.Lerp(defaultFillColor, restoreFlashColor, pulse));
            return;
        }

        if (lowEnergyWarningVisual)
        {
            float pulse = Mathf.PingPong(Time.unscaledTime * lowEnergyPulseSpeed, 1f);
            ApplyFillColor(Color.Lerp(lowEnergyPulseColorA, lowEnergyPulseColorB, pulse));
            return;
        }

        ApplyFillColor(defaultFillColor);
    }

    private void ApplyFillColor(Color color)
    {
        if (energyFillImage != null && energyFillImage.color != color)
        {
            energyFillImage.color = color;
        }
    }

    private void TryFindEnergySlider()
    {
        GameObject uiRoot = GameObject.Find("UI");
        if (uiRoot == null)
        {
            return;
        }

        Transform stateTransform = uiRoot.transform.Find("State");
        if (stateTransform == null)
        {
            return;
        }

        Transform epTransform = stateTransform.Find("EP");
        if (epTransform == null)
        {
            return;
        }

        energySlider = epTransform.GetComponent<Slider>();
        if (energySlider != null && showDebugInfo)
        {
            Debug.Log("<color=cyan>[EnergySystem] 自动找到 EP Slider</color>");
        }
    }

    private void StopPresentationCoroutine()
    {
        if (presentationCoroutine == null)
        {
            return;
        }

        StopCoroutine(presentationCoroutine);
        presentationCoroutine = null;
    }
    #endregion

#if UNITY_EDITOR
    #region 编辑器方法
    [ContextMenu("测试 - 消耗10点精力")]
    private void DEBUG_ConsumeEnergy() => TryConsumeEnergy(10);

    [ContextMenu("测试 - 恢复50点精力")]
    private void DEBUG_RestoreEnergy() => RestoreEnergy(50);

    [ContextMenu("测试 - 完全恢复")]
    private void DEBUG_FullRestore() => FullRestore();
    #endregion
#endif
}
