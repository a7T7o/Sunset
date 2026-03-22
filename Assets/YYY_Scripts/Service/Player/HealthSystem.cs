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

    public static HealthSystem Instance { get; private set; }
    public static event Action<int, int> OnHealthChanged;

    [Header("Health Config")]
    [SerializeField] private int maxHealth = DefaultMaxHealth;
    [SerializeField] private int currentHealth = DefaultMaxHealth;

    [Header("UI Reference")]
    [SerializeField] private Slider healthSlider;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsVisible => healthSlider != null && healthSlider.gameObject.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetHealthState(int current, int max)
    {
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
        }
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
