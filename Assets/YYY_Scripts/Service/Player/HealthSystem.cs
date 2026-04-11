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
    public bool IsVisible => _overlayVisible;

    private Coroutine _presentationCoroutine;
    private CanvasGroup _healthCanvasGroup;
    private bool _overlayVisible;
    private RectTransform _healthSliderRect;
    private Vector2 _preferredHealthAnchoredPosition;
    private bool _hasPreferredHealthAnchoredPosition;
    private int _lastHealthLayoutScreenWidth = -1;
    private int _lastHealthLayoutScreenHeight = -1;

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

        EnsureHealthUi();
        UpdateUI();
    }

    private void LateUpdate()
    {
        UpdateResponsiveLayoutIfNeeded();
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
        if (EnsureHealthUi())
        {
            healthSlider.gameObject.SetActive(visible);
            if (_healthCanvasGroup != null)
            {
                _healthCanvasGroup.alpha = visible ? 1f : 0f;
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
        maxHealth = Mathf.Max(1, max);
        currentHealth = Mathf.Clamp(start, 0, maxHealth);
        _overlayVisible = true;
        EnsureHealthUi();

        if (!EnsureHealthUi())
        {
            currentHealth = Mathf.Clamp(target, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            _presentationCoroutine = null;
            yield break;
        }

        healthSlider.gameObject.SetActive(true);

        float revealSeconds = Mathf.Max(0f, customRevealDuration ?? revealDuration);
        float fillSeconds = Mathf.Max(0f, customFillDuration ?? healFillDuration);
        float clampedTarget = Mathf.Clamp(target, 0, maxHealth);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        if (_healthCanvasGroup != null)
        {
            _healthCanvasGroup.alpha = revealSeconds > 0f ? 0f : 1f;
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (revealSeconds > 0f)
        {
            float revealElapsed = 0f;
            while (revealElapsed < revealSeconds)
            {
                revealElapsed += Time.unscaledDeltaTime;
                if (_healthCanvasGroup != null)
                {
                    _healthCanvasGroup.alpha = Mathf.Clamp01(revealElapsed / revealSeconds);
                }
                yield return null;
            }
        }

        if (_healthCanvasGroup != null)
        {
            _healthCanvasGroup.alpha = 1f;
        }

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

            healthSlider.value = roundedValue;
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
        if (EnsureHealthUi())
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(_overlayVisible);
            if (_healthCanvasGroup != null)
            {
                _healthCanvasGroup.alpha = _overlayVisible ? 1f : 0f;
            }

            if (_overlayVisible)
            {
                _lastHealthLayoutScreenWidth = -1;
                _lastHealthLayoutScreenHeight = -1;
                UpdateResponsiveLayoutIfNeeded();
            }
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

    private bool EnsureHealthUi()
    {
        if (healthSlider == null)
        {
            TryFindHealthSlider();
        }

        if (healthSlider == null)
        {
            return false;
        }

        EnsureCanvasGroup();
        UpdateResponsiveLayoutIfNeeded();
        healthSlider.maxValue = Mathf.Max(1, maxHealth);
        return true;
    }

    private void UpdateResponsiveLayoutIfNeeded()
    {
        if (healthSlider == null)
        {
            _healthSliderRect = null;
            _hasPreferredHealthAnchoredPosition = false;
            _lastHealthLayoutScreenWidth = -1;
            _lastHealthLayoutScreenHeight = -1;
            return;
        }

        RectTransform sliderRect = healthSlider.transform as RectTransform;
        if (sliderRect == null)
        {
            return;
        }

        if (_healthSliderRect != sliderRect)
        {
            _healthSliderRect = sliderRect;
            _preferredHealthAnchoredPosition = sliderRect.anchoredPosition;
            _hasPreferredHealthAnchoredPosition = true;
            _lastHealthLayoutScreenWidth = -1;
            _lastHealthLayoutScreenHeight = -1;
        }
        else if (!_hasPreferredHealthAnchoredPosition)
        {
            _preferredHealthAnchoredPosition = sliderRect.anchoredPosition;
            _hasPreferredHealthAnchoredPosition = true;
        }

        if (Screen.width == _lastHealthLayoutScreenWidth && Screen.height == _lastHealthLayoutScreenHeight)
        {
            return;
        }

        if (!TryResolveResponsiveLayoutContext(sliderRect, out RectTransform rootRect, out RectTransform parentRect))
        {
            _lastHealthLayoutScreenWidth = Screen.width;
            _lastHealthLayoutScreenHeight = Screen.height;
            return;
        }

        const float screenPadding = 24f;
        Canvas.ForceUpdateCanvases();
        sliderRect.anchoredPosition = _preferredHealthAnchoredPosition;

        if (!TryCalculateVisibleBoundsInRoot(sliderRect, rootRect, out Vector2 min, out Vector2 max))
        {
            _lastHealthLayoutScreenWidth = Screen.width;
            _lastHealthLayoutScreenHeight = Screen.height;
            return;
        }

        Rect rootBounds = rootRect.rect;

        float shiftX = 0f;
        float shiftY = 0f;
        float minX = rootBounds.xMin + screenPadding;
        float maxX = rootBounds.xMax - screenPadding;
        float minY = rootBounds.yMin + screenPadding;
        float maxY = rootBounds.yMax - screenPadding;

        if (min.x < minX)
        {
            shiftX = minX - min.x;
        }
        else if (max.x > maxX)
        {
            shiftX = maxX - max.x;
        }

        if (min.y < minY)
        {
            shiftY = minY - min.y;
        }
        else if (max.y > maxY)
        {
            shiftY = maxY - max.y;
        }

        if (Mathf.Abs(shiftX) > 0.01f || Mathf.Abs(shiftY) > 0.01f)
        {
            Vector3 worldOrigin = rootRect.TransformPoint(Vector3.zero);
            Vector3 worldShift = rootRect.TransformPoint(new Vector3(shiftX, shiftY, 0f));
            Vector2 parentDelta = (Vector2)(parentRect.InverseTransformPoint(worldShift) - parentRect.InverseTransformPoint(worldOrigin));
            sliderRect.anchoredPosition = _preferredHealthAnchoredPosition + parentDelta;
        }

        _lastHealthLayoutScreenWidth = Screen.width;
        _lastHealthLayoutScreenHeight = Screen.height;
    }

    private static bool TryCalculateVisibleBoundsInRoot(RectTransform sliderRect, RectTransform rootRect, out Vector2 min, out Vector2 max)
    {
        min = Vector2.zero;
        max = Vector2.zero;

        if (sliderRect == null || rootRect == null)
        {
            return false;
        }

        RectTransform[] rects = sliderRect.GetComponentsInChildren<RectTransform>(true);
        if (rects == null || rects.Length == 0)
        {
            return false;
        }

        bool hasSample = false;
        Vector3[] corners = new Vector3[4];
        for (int rectIndex = 0; rectIndex < rects.Length; rectIndex++)
        {
            RectTransform rect = rects[rectIndex];
            if (rect == null)
            {
                continue;
            }

            rect.GetWorldCorners(corners);
            for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
            {
                Vector2 point = rootRect.InverseTransformPoint(corners[cornerIndex]);
                if (!hasSample)
                {
                    min = point;
                    max = point;
                    hasSample = true;
                    continue;
                }

                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }
        }

        return hasSample;
    }

    private static bool TryResolveResponsiveLayoutContext(RectTransform sliderRect, out RectTransform rootRect, out RectTransform parentRect)
    {
        rootRect = null;
        parentRect = sliderRect != null ? sliderRect.parent as RectTransform : null;
        if (sliderRect == null || parentRect == null)
        {
            return false;
        }

        Canvas canvas = sliderRect.GetComponentInParent<Canvas>(includeInactive: true);
        if (canvas == null)
        {
            return false;
        }

        Canvas rootCanvas = canvas.rootCanvas != null ? canvas.rootCanvas : canvas;
        rootRect = rootCanvas.transform as RectTransform;
        return rootRect != null;
    }
}
