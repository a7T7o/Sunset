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
        private static SpringDay1PromptOverlay _instance;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image backgroundImage;

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

        public void Show(string text)
        {
            if (promptText == null)
            {
                BuildUi();
            }

            promptText.text = text;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (canvasGroup == null)
            {
                return;
            }

            promptText.text = string.Empty;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
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
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.fontSize = 28f;
            promptText.color = new Color(0.98f, 0.96f, 0.9f, 1f);
            promptText.enableWordWrapping = true;
            promptText.text = string.Empty;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(24f, 14f);
            textRect.offsetMax = new Vector2(-24f, -14f);
        }
    }
}
