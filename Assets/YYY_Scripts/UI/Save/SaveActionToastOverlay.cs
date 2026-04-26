using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 正式存档提示层。
/// 用于 F5/F9、设置页读档/重开、复制/粘贴等存档动作后的居中提示。
/// </summary>
[DisallowMultipleComponent]
public sealed class SaveActionToastOverlay : MonoBehaviour
{
    private const string RuntimeRootName = "SaveActionToastOverlay";
    private const float FadeDuration = 0.5f;
    private const float HoldDuration = 1.1f;

    private static SaveActionToastOverlay _instance;

    private CanvasGroup _canvasGroup;
    private Text _messageText;
    private Coroutine _toastCoroutine;

    public static SaveActionToastOverlay Instance
    {
        get
        {
            EnsureRuntime();
            return _instance;
        }
    }

    public static void EnsureRuntime()
    {
        if (_instance != null)
        {
            return;
        }

        SaveActionToastOverlay existing = FindFirstObjectByType<SaveActionToastOverlay>(FindObjectsInactive.Include);
        if (existing != null)
        {
            _instance = existing;
            _instance.EnsureBuilt();
            return;
        }

        GameObject root = new GameObject(
            RuntimeRootName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(CanvasGroup),
            typeof(SaveActionToastOverlay));
        DontDestroyOnLoad(root);

        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 4096;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        _instance = root.GetComponent<SaveActionToastOverlay>();
        _instance.EnsureBuilt();
    }

    public static void Show(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        Instance.ShowInternal(message);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureBuilt();
    }

    private void EnsureBuilt()
    {
        if (_canvasGroup != null && _messageText != null)
        {
            return;
        }

        RectTransform rootRect = transform as RectTransform;
        if (rootRect == null)
        {
            return;
        }

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        Transform panel = transform.Find("Panel");
        if (panel == null)
        {
            RectTransform panelRect = CreateRect("Panel", transform);
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(540f, 104f);
            panelRect.anchoredPosition = Vector2.zero;

            Image background = panelRect.gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.78f);

            Outline outline = panelRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.08f);
            outline.effectDistance = new Vector2(1f, -1f);

            RectTransform labelRect = CreateRect("Label", panelRect);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(28f, 18f);
            labelRect.offsetMax = new Vector2(-28f, -18f);

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            _messageText = labelRect.gameObject.AddComponent<Text>();
            _messageText.font = font;
            _messageText.fontSize = 28;
            _messageText.fontStyle = FontStyle.Bold;
            _messageText.alignment = TextAnchor.MiddleCenter;
            _messageText.color = Color.white;
            _messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
            _messageText.verticalOverflow = VerticalWrapMode.Overflow;
        }
        else
        {
            _messageText = panel.GetComponentInChildren<Text>(true);
        }
    }

    private void ShowInternal(string message)
    {
        EnsureBuilt();
        if (_messageText == null || _canvasGroup == null)
        {
            return;
        }

        _messageText.text = message;

        if (_toastCoroutine != null)
        {
            StopCoroutine(_toastCoroutine);
        }

        _toastCoroutine = StartCoroutine(ToastRoutine());
    }

    private IEnumerator ToastRoutine()
    {
        yield return FadeCanvas(0f, 1f, FadeDuration);
        yield return WaitUnscaled(HoldDuration);
        yield return FadeCanvas(1f, 0f, FadeDuration);
        _toastCoroutine = null;
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (_canvasGroup == null)
        {
            yield break;
        }

        _canvasGroup.alpha = from;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration <= 0.0001f ? 1f : Mathf.Clamp01(elapsed / duration);
            _canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        _canvasGroup.alpha = to;
    }

    private static IEnumerator WaitUnscaled(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }
}
