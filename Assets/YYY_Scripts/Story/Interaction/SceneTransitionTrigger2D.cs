using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class SceneTransitionTrigger2D : MonoBehaviour
    {
        private const string PrimaryHomeEntryAnchorName = "PrimaryHomeEntryAnchor";
        private const string HomeEntryAnchorName = "HomeEntryAnchor";
        private const string TownOpeningEntryAnchorName = "EnterVillageCrowdRoot";

        [Header("目标场景")]
#if UNITY_EDITOR
        [SerializeField] private SceneAsset targetSceneAsset;
#endif
        [SerializeField, HideInInspector] private string targetSceneName = string.Empty;
        [SerializeField, HideInInspector] private string targetScenePath = string.Empty;
        [SerializeField] private string targetEntryAnchorName = string.Empty;
        [SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Single;

        [Header("触发设置")]
        [SerializeField] private bool triggerOnPlayerEnter = true;
        [SerializeField] private string playerTag = "Player";

        [Header("转场")]
        [SerializeField, Min(0f)] private float fadeOutDuration = 0.2f;
        [SerializeField, Min(0f)] private float blackScreenHoldDuration = 0.05f;
        [SerializeField, Min(0f)] private float fadeInDuration = 0.2f;
        [SerializeField, Min(0)] private int postActivationSettleFrames = 2;

        public string TargetSceneName => targetSceneName;
        public string TargetScenePath => targetScenePath;
        public string TargetEntryAnchorName => ResolveTargetEntryAnchorName();
        public bool HasValidTarget => TryResolveTarget(out _, out _, out _);
        private static float s_playerEnterSuppressUntilUnscaledTime;

        private void Reset()
        {
            EnsureTriggerCollider();
        }

        private void Awake()
        {
            EnsureTriggerCollider();
        }

        private void OnValidate()
        {
            EnsureTriggerCollider();
            fadeOutDuration = Mathf.Max(0f, fadeOutDuration);
            blackScreenHoldDuration = Mathf.Max(0f, blackScreenHoldDuration);
            fadeInDuration = Mathf.Max(0f, fadeInDuration);
            postActivationSettleFrames = Mathf.Max(0, postActivationSettleFrames);

#if UNITY_EDITOR
            if (targetSceneAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(targetSceneAsset);
                targetScenePath = assetPath;
                targetSceneName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            }
            else
            {
                targetScenePath = string.Empty;
                targetSceneName = string.Empty;
            }
#endif
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggerOnPlayerEnter || other == null || !other.CompareTag(playerTag))
            {
                return;
            }

            if (Time.unscaledTime < s_playerEnterSuppressUntilUnscaledTime)
            {
                return;
            }

            TryStartTransition();
        }

        public bool TryStartTransition()
        {
            if (SceneTransitionRunner.IsBusy)
            {
                return false;
            }

            if (!TryResolveTarget(out string resolvedSceneName, out string resolvedScenePath, out string failureReason))
            {
                Debug.LogWarning($"[SceneTransitionTrigger2D] {name} {failureReason}", this);
                return false;
            }

            PersistentPlayerSceneBridge.QueueSceneEntry(
                resolvedSceneName,
                resolvedScenePath,
                ResolveTargetEntryAnchorName());

            SceneTransitionRunner.Begin(
                resolvedSceneName,
                resolvedScenePath,
                loadSceneMode,
                fadeOutDuration,
                blackScreenHoldDuration,
                fadeInDuration,
                postActivationSettleFrames);

            return true;
        }

        public void SetTargetScene(string sceneName, string scenePath = "", string entryAnchorName = "")
        {
            targetScenePath = string.IsNullOrWhiteSpace(scenePath)
                ? string.Empty
                : scenePath.Trim();
            targetSceneName = !string.IsNullOrWhiteSpace(sceneName)
                ? sceneName.Trim()
                : ResolveSceneName(string.Empty, targetScenePath);
            targetEntryAnchorName = string.IsNullOrWhiteSpace(entryAnchorName)
                ? string.Empty
                : entryAnchorName.Trim();
        }

        public void ClearTargetScene()
        {
            targetSceneName = string.Empty;
            targetScenePath = string.Empty;
            targetEntryAnchorName = string.Empty;
#if UNITY_EDITOR
            targetSceneAsset = null;
#endif
        }

        public void SetTargetEntryAnchor(string anchorName)
        {
            targetEntryAnchorName = string.IsNullOrWhiteSpace(anchorName)
                ? string.Empty
                : anchorName.Trim();
        }

        public static void SuppressPlayerEnter(float seconds)
        {
            if (seconds <= 0f)
            {
                return;
            }

            s_playerEnterSuppressUntilUnscaledTime = Mathf.Max(
                s_playerEnterSuppressUntilUnscaledTime,
                Time.unscaledTime + seconds);
        }

        private void EnsureTriggerCollider()
        {
            Collider2D triggerCollider = GetComponent<Collider2D>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }

        public bool TryResolveTarget(
            out string resolvedSceneName,
            out string resolvedScenePath,
            out string failureReason)
        {
            resolvedScenePath = string.IsNullOrWhiteSpace(targetScenePath)
                ? string.Empty
                : targetScenePath.Trim();
            resolvedSceneName = ResolveSceneName(targetSceneName, resolvedScenePath);

            if (string.IsNullOrWhiteSpace(resolvedSceneName) && string.IsNullOrWhiteSpace(resolvedScenePath))
            {
                failureReason = "未填写目标场景，已跳过切场。";
                return false;
            }

            failureReason = string.Empty;
            return true;
        }

        private static string ResolveSceneName(string sceneName, string scenePath)
        {
            if (!string.IsNullOrWhiteSpace(sceneName))
            {
                return sceneName.Trim();
            }

            if (string.IsNullOrWhiteSpace(scenePath))
            {
                return string.Empty;
            }

            return System.IO.Path.GetFileNameWithoutExtension(scenePath.Trim());
        }

        private string ResolveTargetEntryAnchorName()
        {
            if (!string.IsNullOrWhiteSpace(targetEntryAnchorName))
            {
                return targetEntryAnchorName.Trim();
            }

            if (string.Equals(name, "HomeDoor", System.StringComparison.Ordinal) &&
                string.Equals(TargetSceneName, "Primary", System.StringComparison.Ordinal))
            {
                return PrimaryHomeEntryAnchorName;
            }

            if (string.Equals(name, "PrimaryHomeDoor", System.StringComparison.Ordinal) &&
                string.Equals(TargetSceneName, "Home", System.StringComparison.Ordinal))
            {
                return HomeEntryAnchorName;
            }

            if (!string.Equals(name, "SceneTransitionTrigger", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (string.Equals(gameObject.scene.name, "Town", StringComparison.Ordinal) &&
                string.Equals(TargetSceneName, "Primary", StringComparison.Ordinal))
            {
                return PrimaryHomeEntryAnchorName;
            }

            if (string.Equals(gameObject.scene.name, "Primary", StringComparison.Ordinal) &&
                string.Equals(TargetSceneName, "Town", StringComparison.Ordinal))
            {
                return TownOpeningEntryAnchorName;
            }

            return string.Empty;
        }

    }

    internal sealed class SceneTransitionRunner : MonoBehaviour
    {
        private const string RunnerObjectName = "_SceneTransitionRunner";

        private static SceneTransitionRunner _instance;

        private CanvasGroup _canvasGroup;
        private bool _isBusy;
        private bool? _restoreInputEnabledState;

        public static bool IsBusy => _instance != null && _instance._isBusy;

        public static void Begin(
            string sceneName,
            string scenePath,
            LoadSceneMode loadSceneMode,
            float fadeOutDuration,
            float blackScreenHoldDuration,
            float fadeInDuration,
            int postActivationSettleFrames)
        {
            SceneTransitionRunner runner = EnsureInstance();
            runner.StartTransition(
                sceneName,
                scenePath,
                loadSceneMode,
                fadeOutDuration,
                blackScreenHoldDuration,
                fadeInDuration,
                postActivationSettleFrames);
        }

        public static bool TryBlink(
            Action onBlackout,
            float fadeOutDuration = 0.08f,
            float blackScreenHoldDuration = 0.04f,
            float fadeInDuration = 0.08f)
        {
            SceneTransitionRunner runner = EnsureInstance();
            return runner.StartBlink(onBlackout, fadeOutDuration, blackScreenHoldDuration, fadeInDuration);
        }

        private static SceneTransitionRunner EnsureInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            GameObject runnerObject = new GameObject(RunnerObjectName);
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(runnerObject);
            }

            _instance = runnerObject.AddComponent<SceneTransitionRunner>();
            return _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            BuildOverlay();
        }

        private void StartTransition(
            string sceneName,
            string scenePath,
            LoadSceneMode loadSceneMode,
            float fadeOutDuration,
            float blackScreenHoldDuration,
            float fadeInDuration,
            int postActivationSettleFrames)
        {
            if (_isBusy)
            {
                return;
            }

            StartCoroutine(
                TransitionRoutine(
                    sceneName,
                    scenePath,
                    loadSceneMode,
                    fadeOutDuration,
                    blackScreenHoldDuration,
                    fadeInDuration,
                    postActivationSettleFrames));
        }

        private bool StartBlink(
            Action onBlackout,
            float fadeOutDuration,
            float blackScreenHoldDuration,
            float fadeInDuration)
        {
            if (_isBusy)
            {
                return false;
            }

            StartCoroutine(BlinkRoutine(onBlackout, fadeOutDuration, blackScreenHoldDuration, fadeInDuration));
            return true;
        }

        private IEnumerator TransitionRoutine(
            string sceneName,
            string scenePath,
            LoadSceneMode loadSceneMode,
            float fadeOutDuration,
            float blackScreenHoldDuration,
            float fadeInDuration,
            int postActivationSettleFrames)
        {
            _isBusy = true;
            CacheAndBlockGameplayInput();
            SetOverlayInputBlock(true);
            try
            {
                yield return FadeTo(1f, fadeOutDuration);

                AsyncOperation loadOperation = LoadScene(sceneName, scenePath, loadSceneMode);
                if (loadOperation != null)
                {
                    // 把 scene activation 峰值尽量吞进黑屏里，减少切场时的可见卡顿。
                    loadOperation.allowSceneActivation = false;
                    while (loadOperation.progress < 0.9f)
                    {
                        yield return null;
                    }

                    loadOperation.allowSceneActivation = true;
                    while (!loadOperation.isDone)
                    {
                        yield return null;
                    }
                }

                for (int frameIndex = 0; frameIndex < postActivationSettleFrames; frameIndex++)
                {
                    yield return null;
                }

                if (blackScreenHoldDuration > 0f)
                {
                    yield return new WaitForSecondsRealtime(blackScreenHoldDuration);
                }

                yield return FadeTo(0f, fadeInDuration);
            }
            finally
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 0f;
                }

                SetOverlayInputBlock(false);
                RestoreGameplayInput();
                _isBusy = false;
            }
        }

        private IEnumerator BlinkRoutine(
            Action onBlackout,
            float fadeOutDuration,
            float blackScreenHoldDuration,
            float fadeInDuration)
        {
            _isBusy = true;
            CacheAndBlockGameplayInput();
            SetOverlayInputBlock(true);
            try
            {
                yield return FadeTo(1f, fadeOutDuration);
                onBlackout?.Invoke();

                if (blackScreenHoldDuration > 0f)
                {
                    yield return new WaitForSecondsRealtime(blackScreenHoldDuration);
                }

                yield return FadeTo(0f, fadeInDuration);
            }
            finally
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 0f;
                }

                SetOverlayInputBlock(false);
                RestoreGameplayInput();
                _isBusy = false;
            }
        }

        private static AsyncOperation LoadScene(string sceneName, string scenePath, LoadSceneMode loadSceneMode)
        {
            int buildIndex = ResolveBuildIndex(sceneName, scenePath);

#if UNITY_EDITOR
            if (!string.IsNullOrWhiteSpace(scenePath) &&
                buildIndex < 0)
            {
                return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, new LoadSceneParameters(loadSceneMode));
            }
#endif

            if (buildIndex >= 0)
            {
                return SceneManager.LoadSceneAsync(buildIndex, loadSceneMode);
            }

            if (!string.IsNullOrWhiteSpace(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName))
            {
                return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            }

            Debug.LogError(
                $"[SceneTransitionTrigger2D] 无法加载场景。Name='{sceneName}', Path='{scenePath}'。Editor PlayMode 下可按 scene path 直载；打包运行时该场景必须加入 Build Profiles / shared scene list。",
                _instance);
            return null;
        }

        private static int ResolveBuildIndex(string sceneName, string scenePath)
        {
            if (!string.IsNullOrWhiteSpace(scenePath))
            {
                int pathBuildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
                if (pathBuildIndex >= 0)
                {
                    return pathBuildIndex;
                }
            }

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return -1;
            }

            for (int index = 0; index < SceneManager.sceneCountInBuildSettings; index++)
            {
                string buildScenePath = SceneUtility.GetScenePathByBuildIndex(index);
                if (string.IsNullOrWhiteSpace(buildScenePath))
                {
                    continue;
                }

                string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(buildScenePath);
                if (string.Equals(buildSceneName, sceneName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }

        private void SetOverlayInputBlock(bool blocked)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            _canvasGroup.blocksRaycasts = blocked;
            _canvasGroup.interactable = blocked;
        }

        private void CacheAndBlockGameplayInput()
        {
            GameInputManager inputManager = ResolveGameInputManager();
            if (inputManager == null)
            {
                _restoreInputEnabledState = null;
                return;
            }

            _restoreInputEnabledState = inputManager.IsInputEnabledForDebug;
            inputManager.SetInputEnabled(false);
        }

        private void RestoreGameplayInput()
        {
            if (!_restoreInputEnabledState.HasValue)
            {
                return;
            }

            GameInputManager inputManager = ResolveGameInputManager();
            if (inputManager != null)
            {
                inputManager.SetInputEnabled(_restoreInputEnabledState.Value);
            }

            _restoreInputEnabledState = null;
        }

        private static GameInputManager ResolveGameInputManager()
        {
            return GameInputManager.Instance ??
                   UnityEngine.Object.FindFirstObjectByType<GameInputManager>(FindObjectsInactive.Include);
        }

        private static AsyncOperation LoadSceneFallbackByName(string sceneName, LoadSceneMode loadSceneMode)
        {
            return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            if (_canvasGroup == null)
            {
                yield break;
            }

            float startAlpha = _canvasGroup.alpha;
            if (duration <= 0f)
            {
                _canvasGroup.alpha = targetAlpha;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
        }

        private void BuildOverlay()
        {
            GameObject canvasObject = new GameObject("FadeCanvas");
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = short.MaxValue;

            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            _canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(canvasObject.transform, false);
            RectTransform imageRect = imageObject.AddComponent<RectTransform>();
            imageRect.anchorMin = Vector2.zero;
            imageRect.anchorMax = Vector2.one;
            imageRect.offsetMin = Vector2.zero;
            imageRect.offsetMax = Vector2.zero;

            Image fadeImage = imageObject.AddComponent<Image>();
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false;
        }
    }
}
