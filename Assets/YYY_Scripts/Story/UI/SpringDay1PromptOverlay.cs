using System.Collections;
using Sunset.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sunset.Story
{
    /// <summary>
    /// spring-day1 轻量教程提示层。
    /// 运行时动态创建，不修改场景资源。
    /// </summary>
    public class SpringDay1PromptOverlay : MonoBehaviour
    {
        private static readonly string[] PreferredFontResourcePaths =
        {
            "Fonts & Materials/DialogueChinese SoftPixel SDF",
            "Fonts & Materials/DialogueChinese Pixel SDF",
            "Fonts & Materials/DialogueChinese V2 SDF",
            "Fonts & Materials/DialogueChinese SDF"
        };

        private static SpringDay1PromptOverlay _instance;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private float postDialogueResumeDelay = 0.18f;
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

            Transform parent = null;
            GameObject uiRoot = GameObject.Find("UI");
            if (uiRoot != null)
            {
                parent = uiRoot.transform;
            }
            else
            {
                Canvas rootCanvas = Object.FindFirstObjectByType<Canvas>();
                parent = rootCanvas != null ? rootCanvas.transform : null;
            }

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

        public void Show(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Hide();
                return;
            }

            if (promptText == null)
            {
                BuildUi();
            }

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
            RectTransform rootRect = transform as RectTransform;
            rootRect.anchorMin = new Vector2(0.5f, 0f);
            rootRect.anchorMax = new Vector2(0.5f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0f);
            rootRect.anchoredPosition = new Vector2(0f, 56f);
            rootRect.sizeDelta = new Vector2(760f, 84f);

            canvasGroup = GetComponent<CanvasGroup>();

            GameObject background = new GameObject("PromptBackground", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(transform, false);
            backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = new Color(0.08f, 0.09f, 0.12f, 0.88f);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            GameObject textObject = new GameObject("PromptText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(transform, false);
            promptText = textObject.GetComponent<TextMeshProUGUI>();
            promptText.font = ResolveFontAsset();
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.fontSize = 28f;
            promptText.color = new Color(0.98f, 0.96f, 0.9f, 1f);
            promptText.textWrappingMode = TextWrappingModes.Normal;
            promptText.text = string.Empty;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(24f, 14f);
            textRect.offsetMax = new Vector2(-24f, -14f);
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
            if (promptText == null)
            {
                return;
            }

            StopQueuedRevealCoroutine();
            promptText.text = text;
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
                _currentPromptText = _queuedPromptText;
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

            DialogueManager dialogueManager = Object.FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
            {
                return true;
            }

            DialogueUI dialogueUi = Object.FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
            return dialogueUi != null && dialogueUi.CurrentCanvasAlpha > 0.01f;
        }

        private void FadeCanvasGroup(float targetAlpha, bool clearTextOnComplete)
        {
            if (canvasGroup == null)
            {
                return;
            }

            StopVisibilityCoroutine();
            _visibilityCoroutine = StartCoroutine(FadeCanvasGroupRoutine(canvasGroup.alpha, targetAlpha, clearTextOnComplete));
        }

        private IEnumerator FadeCanvasGroupRoutine(float from, float to, bool clearTextOnComplete)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (fadeDuration <= 0f || Mathf.Approximately(from, to))
            {
                canvasGroup.alpha = to;
                if (clearTextOnComplete && promptText != null)
                {
                    promptText.text = string.Empty;
                }

                _visibilityCoroutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(elapsed / fadeDuration);
                float eased = to > from
                    ? 1f - ((1f - normalized) * (1f - normalized))
                    : normalized * normalized;

                canvasGroup.alpha = Mathf.Lerp(from, to, eased);

                if (normalized >= 1f)
                {
                    break;
                }

                yield return null;
            }

            canvasGroup.alpha = to;
            if (clearTextOnComplete && promptText != null)
            {
                promptText.text = string.Empty;
            }

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
    }
}
