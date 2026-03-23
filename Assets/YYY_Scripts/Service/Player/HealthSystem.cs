using System.Collections;
using System;
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
    public bool IsVisible => healthSlider != null && healthSlider.gameObject.activeSelf;

    private Coroutine _presentationCoroutine;
    private CanvasGroup _healthCanvasGroup;

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
        if (healthSlider == null)
        {
            TryFindHealthSlider();
        }

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(visible);
            if (visible)
            {
                EnsureCanvasGroup();
                _healthCanvasGroup.alpha = 1f;
            }
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
        if (healthSlider == null)
        {
            TryFindHealthSlider();
        }

        maxHealth = Mathf.Max(1, max);
        currentHealth = Mathf.Clamp(start, 0, maxHealth);

        if (healthSlider == null)
        {
            currentHealth = Mathf.Clamp(target, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            _presentationCoroutine = null;
            yield break;
        }

        healthSlider.gameObject.SetActive(true);
        EnsureCanvasGroup();

        float revealSeconds = Mathf.Max(0f, customRevealDuration ?? revealDuration);
        float fillSeconds = Mathf.Max(0f, customFillDuration ?? healFillDuration);
        float clampedTarget = Mathf.Clamp(target, 0, maxHealth);

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        _healthCanvasGroup.alpha = 0f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (revealSeconds > 0f)
        {
            float revealElapsed = 0f;
            while (revealElapsed < revealSeconds)
            {
                revealElapsed += Time.unscaledDeltaTime;
                _healthCanvasGroup.alpha = Mathf.Clamp01(revealElapsed / revealSeconds);
                yield return null;
            }
        }

        _healthCanvasGroup.alpha = 1f;

        if (fillSeconds <= 0f)
        {
            currentHealth = Mathf.RoundToInt(clampedTarget);
            healthSlider.value = currentHealth;
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

            healthSlider.value = currentValue;
            if (roundedValue != lastReportedHealth)
            {
                currentHealth = roundedValue;
                lastReportedHealth = roundedValue;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }

            yield return null;
        }

        currentHealth = Mathf.RoundToInt(clampedTarget);
        healthSlider.value = currentHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        _presentationCoroutine = null;
    }

    private void UpdateUI()
    {
        if (healthSlider == null)
        {
            return;
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
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
}
