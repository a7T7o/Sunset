using System.Collections;
using System;
using Sunset.Story;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 轻量生命值系统。
/// 当前优先服务 spring-day1 的剧情展示与状态推进，不接战斗完整伤害链。
/// </summary>
public class HealthSystem : MonoBehaviour
{
    private const int DefaultMaxHealth = 100;

    private static HealthSystem _instance;
    public static HealthSystem Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = FindFirstObjectByType<HealthSystem>();
            if (_instance != null)
            {
                return _instance;
            }

            GameObject runtimeObject = new GameObject(nameof(HealthSystem));
            _instance = runtimeObject.AddComponent<HealthSystem>();
            return _instance;
        }
    }
    public static event Action<int, int> OnHealthChanged;

    [Header("Health Config")]
    [SerializeField] private int maxHealth = DefaultMaxHealth;
    [SerializeField] private int currentHealth = DefaultMaxHealth;

    [Header("UI Reference")]
    [SerializeField] private Slider healthSlider;

    [Header("Story Presentation")]
    [SerializeField] private float revealDuration = 0.35f;
    [SerializeField] private float healFillDuration = 0.9f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsVisible => _overlayVisible;

    private Coroutine _presentationCoroutine;
    private CanvasGroup _healthCanvasGroup;
    private SpringDay1StatusOverlay _statusOverlay;
    private bool _overlayVisible;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        currentHealth = Mathf.Clamp(currentHealth, 0, Mathf.Max(1, maxHealth));
    }

    private void Start()
    {
        if (healthSlider == null)
        {
            TryFindHealthSlider();
        }

        HideLegacySlider();
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void SetHealthState(int current, int max)
    {
        StopPresentationCoroutine();
        maxHealth = Mathf.Max(1, max);
        currentHealth = Mathf.Clamp(current, 0, maxHealth);
        UpdateUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RestoreHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        StopPresentationCoroutine();
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateUI();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetVisible(bool visible)
    {
        _overlayVisible = visible;
        HideLegacySlider();
        if (EnsureOverlay())
        {
            _statusOverlay.SetSectionVisible(SpringDay1StatusOverlay.Channel.Health, visible);
            _statusOverlay.SetSectionAlpha(SpringDay1StatusOverlay.Channel.Health, visible ? 1f : 0f);
            _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, currentHealth, maxHealth);
        }
    }

    public Coroutine PlayRevealAndAnimateTo(int start, int target, int max, float? customRevealDuration = null, float? customFillDuration = null)
    {
        StopPresentationCoroutine();
        _presentationCoroutine = StartCoroutine(RevealAndAnimateToRoutine(start, target, max, customRevealDuration, customFillDuration));
        return _presentationCoroutine;
    }

    private IEnumerator RevealAndAnimateToRoutine(int start, int target, int max, float? customRevealDuration = null, float? customFillDuration = null)
    {
        maxHealth = Mathf.Max(1, max);
        currentHealth = Mathf.Clamp(start, 0, maxHealth);
        _overlayVisible = true;
        HideLegacySlider();

        if (!EnsureOverlay())
        {
            currentHealth = Mathf.Clamp(target, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            _presentationCoroutine = null;
            yield break;
        }

        _statusOverlay.SetSectionVisible(SpringDay1StatusOverlay.Channel.Health, true);

        float revealSeconds = Mathf.Max(0f, customRevealDuration ?? revealDuration);
        float fillSeconds = Mathf.Max(0f, customFillDuration ?? healFillDuration);
        float clampedTarget = Mathf.Clamp(target, 0, maxHealth);
        _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, currentHealth, maxHealth);
        _statusOverlay.SetSectionAlpha(SpringDay1StatusOverlay.Channel.Health, revealSeconds > 0f ? 0f : 1f);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (revealSeconds > 0f)
        {
            float revealElapsed = 0f;
            while (revealElapsed < revealSeconds)
            {
                revealElapsed += Time.unscaledDeltaTime;
                _statusOverlay.SetSectionAlpha(SpringDay1StatusOverlay.Channel.Health, Mathf.Clamp01(revealElapsed / revealSeconds));
                yield return null;
            }
        }

        _statusOverlay.SetSectionAlpha(SpringDay1StatusOverlay.Channel.Health, 1f);

        if (fillSeconds <= 0f)
        {
            currentHealth = Mathf.RoundToInt(clampedTarget);
            _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            _presentationCoroutine = null;
            yield break;
        }

        float startValue = currentHealth;
        float fillElapsed = 0f;
        int lastReportedHealth = currentHealth;
        while (fillElapsed < fillSeconds)
        {
            fillElapsed += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(fillElapsed / fillSeconds);
            float eased = 1f - ((1f - normalized) * (1f - normalized));
            float currentValue = Mathf.Lerp(startValue, clampedTarget, eased);
            int roundedValue = Mathf.RoundToInt(currentValue);

            _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, roundedValue, maxHealth);
            if (roundedValue != lastReportedHealth)
            {
                currentHealth = roundedValue;
                lastReportedHealth = roundedValue;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }

            yield return null;
        }

        currentHealth = Mathf.RoundToInt(clampedTarget);
        _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        _presentationCoroutine = null;
    }

    private void UpdateUI()
    {
        if (EnsureOverlay())
        {
            _statusOverlay.SetValues(SpringDay1StatusOverlay.Channel.Health, currentHealth, maxHealth);
            _statusOverlay.SetSectionVisible(SpringDay1StatusOverlay.Channel.Health, _overlayVisible);
            _statusOverlay.SetSectionAlpha(SpringDay1StatusOverlay.Channel.Health, _overlayVisible ? 1f : 0f);
        }
    }

    private void EnsureCanvasGroup()
    {
        if (healthSlider == null)
        {
            return;
        }

        if (_healthCanvasGroup == null)
        {
            _healthCanvasGroup = healthSlider.GetComponent<CanvasGroup>();
            if (_healthCanvasGroup == null)
            {
                _healthCanvasGroup = healthSlider.gameObject.AddComponent<CanvasGroup>();
            }
        }

        _healthCanvasGroup.interactable = false;
        _healthCanvasGroup.blocksRaycasts = false;
    }

    private void StopPresentationCoroutine()
    {
        if (_presentationCoroutine == null)
        {
            return;
        }

        StopCoroutine(_presentationCoroutine);
        _presentationCoroutine = null;
    }

    private void TryFindHealthSlider()
    {
        var uiRoot = GameObject.Find("UI");
        if (uiRoot == null)
        {
            return;
        }

        var stateTransform = uiRoot.transform.Find("State");
        if (stateTransform == null)
        {
            return;
        }

        var hpTransform = stateTransform.Find("HP");
        if (hpTransform == null)
        {
            return;
        }

        healthSlider = hpTransform.GetComponent<Slider>();

        if (healthSlider != null && showDebugInfo)
        {
            Debug.Log("<color=cyan>[HealthSystem] 自动找到 HP Slider</color>");
        }
    }

    private bool EnsureOverlay()
    {
        if (_statusOverlay == null)
        {
            SpringDay1StatusOverlay.EnsureRuntime();
            _statusOverlay = SpringDay1StatusOverlay.Instance;
        }

        return _statusOverlay != null;
    }

    private void HideLegacySlider()
    {
        if (healthSlider == null)
        {
            TryFindHealthSlider();
        }

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }
    }
}
