using System.Collections;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    /// <summary>
    /// spring-day1 任务提示卡片。
    /// 运行时动态创建，不修改场景资源。
    /// </summary>
    public class SpringDay1PromptOverlay : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese BitmapSong SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static SpringDay1PromptOverlay _instance;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rootRect;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private float postDialogueResumeDelay = 0.18f;

        private TMP_FontAsset _fontAsset;
        private bool _suppressWhileDialogueActive;
        private string _currentPromptText = string.Empty;
        private string _queuedPromptText = string.Empty;
        private Coroutine _visibilityCoroutine;
        private Coroutine _queuedRevealCoroutine;

        public static SpringDay1PromptOverlay Instance
        {
            get
            {
                if (_instance == null)
                {
                    EnsureRuntime();
                }

                return _instance;
            }
        }

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            Transform parent = ResolveParent();
            GameObject root = new GameObject(
                nameof(SpringDay1PromptOverlay),
                typeof(RectTransform),
                typeof(CanvasGroup));

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
            }

            _instance = root.AddComponent<SpringDay1PromptOverlay>();
            _instance.BuildUi();
            _instance.Hide();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
            EventBus.Subscribe<DialogueEndEvent>(OnDialogueEnd, owner: this);
        }

        private void OnDisable()
        {
            StopVisibilityCoroutine();
            StopQueuedRevealCoroutine();
            EventBus.UnsubscribeAll(this);
        }

        private void LateUpdate()
        {
            if (canvasGroup == null || titleText == null || progressText == null)
            {
                return;
            }

            UpdateContextTexts();

            if (ShouldDelayPromptDisplay())
            {
                if (canvasGroup.alpha > 0.001f)
                {
                    FadeCanvasGroup(0f, false);
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(_currentPromptText) && canvasGroup.alpha < 0.999f && _queuedRevealCoroutine == null)
            {
                FadeCanvasGroup(1f, false);
            }
        }

        public void Show(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Hide();
                return;
            }

            EnsureBuilt();
            _currentPromptText = text;
            _queuedPromptText = text;

            if (ShouldDelayPromptDisplay())
            {
                QueuePromptReveal();
                return;
            }

            ShowCurrentPromptImmediate(_currentPromptText);
        }

        public void Hide()
        {
            _currentPromptText = string.Empty;
            _queuedPromptText = string.Empty;
            StopQueuedRevealCoroutine();
            FadeCanvasGroup(0f, true);
        }

        private void OnDialogueStart(DialogueStartEvent _)
        {
            _suppressWhileDialogueActive = true;
            if (!string.IsNullOrWhiteSpace(_currentPromptText))
            {
                _queuedPromptText = _currentPromptText;
            }

            FadeCanvasGroup(0f, false);
        }

        private void OnDialogueEnd(DialogueEndEvent _)
        {
            _suppressWhileDialogueActive = false;
            if (!string.IsNullOrWhiteSpace(_queuedPromptText))
            {
                QueuePromptReveal();
            }
        }

        private void BuildUi()
        {
            rootRect = transform as RectTransform;
            _fontAsset = ResolveFontAsset();

            rootRect.anchorMin = new Vector2(0f, 1f);
            rootRect.anchorMax = new Vector2(0f, 1f);
            rootRect.pivot = new Vector2(0f, 1f);
            rootRect.anchoredPosition = new Vector2(20f, -18f);
            rootRect.sizeDelta = new Vector2(348f, 112f);

            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            GameObject background = new GameObject("PromptCard", typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Shadow));
            background.transform.SetParent(transform, false);
            backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = new Color(0.08f, 0.1f, 0.14f, 0.82f);
            backgroundImage.raycastTarget = false;

            Outline outline = background.GetComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.08f);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;

            Shadow shadow = background.GetComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.22f);
            shadow.effectDistance = new Vector2(0f, -6f);
            shadow.useGraphicAlpha = true;

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            RectTransform accentStrip = CreateRect(background.transform, "AccentStrip");
            accentStrip.anchorMin = new Vector2(0f, 0f);
            accentStrip.anchorMax = new Vector2(0f, 1f);
            accentStrip.pivot = new Vector2(0f, 0.5f);
            accentStrip.anchoredPosition = Vector2.zero;
            accentStrip.sizeDelta = new Vector2(4f, 0f);
            Image accentImage = accentStrip.gameObject.AddComponent<Image>();
            accentImage.color = new Color(0.96f, 0.78f, 0.42f, 0.92f);
            accentImage.raycastTarget = false;

            RectTransform titleTag = CreateRect(background.transform, "TitleTag");
            titleTag.anchorMin = new Vector2(0f, 1f);
            titleTag.anchorMax = new Vector2(0f, 1f);
            titleTag.pivot = new Vector2(0f, 1f);
            titleTag.anchoredPosition = new Vector2(18f, -12f);
            titleTag.sizeDelta = new Vector2(138f, 20f);
            Image titleTagImage = titleTag.gameObject.AddComponent<Image>();
            titleTagImage.color = new Color(0.97f, 0.82f, 0.48f, 0.16f);
            titleTagImage.raycastTarget = false;

            titleText = CreateText(titleTag, "TitleText", "Day1 任务", 11f, new Color(0.98f, 0.92f, 0.72f, 1f), TextAlignmentOptions.Center);
            StretchRect(titleText.rectTransform);

            promptText = CreateText(background.transform, "PromptText", string.Empty, 17f, new Color(0.96f, 0.97f, 1f, 1f), TextAlignmentOptions.TopLeft, true);
            RectTransform promptRect = promptText.rectTransform;
            promptRect.anchorMin = new Vector2(0f, 1f);
            promptRect.anchorMax = new Vector2(1f, 1f);
            promptRect.pivot = new Vector2(0.5f, 1f);
            promptRect.offsetMin = new Vector2(18f, -74f);
            promptRect.offsetMax = new Vector2(-18f, -38f);

            progressText = CreateText(background.transform, "ProgressText", string.Empty, 11f, new Color(0.78f, 0.84f, 0.93f, 0.96f), TextAlignmentOptions.BottomLeft, true);
            RectTransform progressRect = progressText.rectTransform;
            progressRect.anchorMin = new Vector2(0f, 0f);
            progressRect.anchorMax = new Vector2(1f, 0f);
            progressRect.pivot = new Vector2(0.5f, 0f);
            progressRect.offsetMin = new Vector2(18f, 10f);
            progressRect.offsetMax = new Vector2(-18f, 32f);
        }

        private void EnsureBuilt()
        {
            if (rootRect == null || canvasGroup == null || titleText == null || promptText == null || progressText == null)
            {
                BuildUi();
            }
        }

        private void UpdateContextTexts()
        {
            SpringDay1Director director = SpringDay1Director.Instance;
            if (director != null)
            {
                titleText.text = director.GetCurrentTaskLabel();
                progressText.text = director.GetCurrentProgressLabel();
                return;
            }

            titleText.text = "Day1 任务";
            progressText.text = string.Empty;
        }

        private TMP_FontAsset ResolveFontAsset()
        {
            for (int index = 0; index < PreferredFontResourcePaths.Length; index++)
            {
                TMP_FontAsset candidate = Resources.Load<TMP_FontAsset>(PreferredFontResourcePaths[index]);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return TMP_Settings.defaultFontAsset;
        }

        private void ShowCurrentPromptImmediate(string text)
        {
            EnsureBuilt();
            StopQueuedRevealCoroutine();
            promptText.text = text;
            UpdateContextTexts();
            FadeCanvasGroup(1f, false);
        }

        private void QueuePromptReveal()
        {
            if (string.IsNullOrWhiteSpace(_queuedPromptText))
            {
                return;
            }

            StopQueuedRevealCoroutine();
            _queuedRevealCoroutine = StartCoroutine(WaitAndRevealQueuedPrompt());
        }

        private IEnumerator WaitAndRevealQueuedPrompt()
        {
            while (ShouldDelayPromptDisplay())
            {
                yield return null;
            }

            if (postDialogueResumeDelay > 0f)
            {
                float remaining = postDialogueResumeDelay;
                while (remaining > 0f)
                {
                    if (ShouldDelayPromptDisplay())
                    {
                        remaining = postDialogueResumeDelay;
                        yield return null;
                        continue;
                    }

                    remaining -= Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            if (!string.IsNullOrWhiteSpace(_queuedPromptText))
            {
                ShowCurrentPromptImmediate(_queuedPromptText);
            }

            _queuedRevealCoroutine = null;
        }

        private bool ShouldDelayPromptDisplay()
        {
            if (_suppressWhileDialogueActive)
            {
                return true;
            }

            DialogueManager dialogueManager = DialogueManager.Instance;
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return true;
            }

            SpringDay1WorkbenchCraftingOverlay overlay = FindFirstObjectByType<SpringDay1WorkbenchCraftingOverlay>(FindObjectsInactive.Include);
            return overlay != null && overlay.IsVisible;
        }

        private void FadeCanvasGroup(float targetAlpha, bool immediate)
        {
            if (canvasGroup == null)
            {
                return;
            }

            StopVisibilityCoroutine();

            if (immediate || fadeDuration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }

            _visibilityCoroutine = StartCoroutine(FadeCanvasGroupRoutine(targetAlpha));
        }

        private IEnumerator FadeCanvasGroupRoutine(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / fadeDuration);
                float eased = 1f - ((1f - normalized) * (1f - normalized));
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            _visibilityCoroutine = null;
        }

        private void StopVisibilityCoroutine()
        {
            if (_visibilityCoroutine == null)
            {
                return;
            }

            StopCoroutine(_visibilityCoroutine);
            _visibilityCoroutine = null;
        }

        private void StopQueuedRevealCoroutine()
        {
            if (_queuedRevealCoroutine == null)
            {
                return;
            }

            StopCoroutine(_queuedRevealCoroutine);
            _queuedRevealCoroutine = null;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment, bool wrap = false)
        {
            RectTransform rect = CreateRect(parent, name);
            TextMeshProUGUI tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.font = _fontAsset;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            tmp.overflowMode = wrap ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void StretchRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Transform ResolveParent()
        {
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                return uiRoot.transform;
            }

            Canvas rootCanvas = Object.FindFirstObjectByType<Canvas>();
            return rootCanvas != null ? rootCanvas.transform : null;
        }
    }
}
