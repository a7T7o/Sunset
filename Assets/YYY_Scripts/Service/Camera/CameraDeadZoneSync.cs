using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;

namespace Sunset.Service.Camera
{
    /// <summary>
    /// 摄像头边界限制组件
    /// 自动检测场景边界并同步到 CinemachineConfiner2D，确保摄像头视野不超出场景
    /// </summary>
    public class CameraDeadZoneSync : MonoBehaviour
    {
        private const string AutoBoundsObjectName = "_CameraBounds";
        private const int SceneLoadTrackingTargetRetryFrames = 4;
        private static readonly string[] LegacyOvertightExcludedKeywords = { "water", "props", "farmland", "old" };
        private static readonly string[] SoftenedExcludedKeywords = { "old" };

        #region 序列化字段

        [Header("边界检测")]
        [SerializeField] private bool autoDetectBounds = true;
        [SerializeField] private string[] worldLayerNames = { "LAYER 1", "LAYER 2", "LAYER 3" };
        [SerializeField] private float boundsPadding = 0f;
        [SerializeField] private Tilemap[] explicitBoundsTilemaps = new Tilemap[0];
        [SerializeField] private Collider2D[] explicitBoundsColliders = new Collider2D[0];
        [SerializeField] private string[] preferredExactTilemapNames = { "Layer 2 - Base", "Layer 1 - Base" };
        [SerializeField] private string[] preferredAutoBoundsKeywords = { "base" };
        [SerializeField] private string[] excludedAutoBoundsKeywords = { "water", "props", "farmland", "old" };
        [SerializeField, Min(0f)] private float autoBoundsInset = 0.5f;
        [SerializeField] private bool includeWorldSpriteRenderersInAutoBounds = true;

        [Header("宽屏保护")]
        [SerializeField] private bool clampViewportOnWideScreens = true;
        [SerializeField] private bool clampViewportInWindowedMode = false;
        [SerializeField, Range(0.8f, 1f)] private float wideScreenViewportSafety = 0.95f;
        [SerializeField] private bool snapViewportClampToPixelGrid = true;

        [Header("手动边界（当 autoDetectBounds = false）")]
        [SerializeField] private Bounds manualBounds = new Bounds(Vector3.zero, new Vector3(50, 50, 0));

        [Header("引用")]
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private UnityEngine.Camera mainCamera;

        [Header("Confiner 设置")]
        [Tooltip("用于定义边界的 PolygonCollider2D，会自动创建")]
        [SerializeField] private PolygonCollider2D boundingCollider;

        [Header("调试")]
#pragma warning disable CS0414 // 仅在 UNITY_EDITOR 的 Gizmos 绘制里使用
        [SerializeField] private bool showDebugGizmos = true;
#pragma warning restore CS0414
        [SerializeField] private bool logDebugInfo = false;

        #endregion

        #region 私有字段

        private CinemachineConfiner2D _confiner;
        private CinemachineBrain _brain;
        private Bounds _worldBounds;
        private bool _isInitialized;
        private bool _capturedDefaultCameraRect;
        private Rect _defaultCameraRect = new Rect(0f, 0f, 1f, 1f);
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private Coroutine _sceneLoadRecoveryCoroutine;

        #endregion

        #region 公共属性

        /// <summary>
        /// 当前检测到的世界边界
        /// </summary>
        public Bounds WorldBounds => _worldBounds;

        #endregion


        #region Unity 生命周期

        private void Awake()
        {
            // 自动获取引用
            if (cinemachineCamera == null)
            {
                cinemachineCamera = ResolveCinemachineCamera();
            }

            if (mainCamera == null)
            {
                mainCamera = ResolveMainCamera();
            }

            CaptureDefaultCameraRect();
            EnsureCinemachineBrain();

            // 获取或创建 Confiner2D
            SetupConfiner();

            ValidateReferences();
        }

        private void Start()
        {
            RefreshSceneReferences(SceneManager.GetActiveScene());

            if (_isInitialized)
            {
                RefreshBounds();
            }
        }

        private void LateUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (!HasUsableTrackingTarget())
            {
                TryBindTrackingTarget(SceneManager.GetActiveScene());
            }

            if (mainCamera == null)
            {
                return;
            }

            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                if (UpdateWideScreenViewportClamp())
                {
                    InvalidateConfinerCache();
                }
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StopSceneLoadRecovery();
            RestoreDefaultCameraRect();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 刷新边界检测并应用到 Confiner
        /// </summary>
        public void RefreshBounds()
        {
            if (!_isInitialized)
            {
                if (logDebugInfo)
                    Debug.LogWarning("[CameraConfiner] 组件未正确初始化，无法刷新边界");
                return;
            }

            if (autoDetectBounds)
            {
                DetectWorldBounds();
            }
            else
            {
                _worldBounds = manualBounds;
            }

            UpdateBoundingCollider();
            UpdateWideScreenViewportClamp();
            InvalidateConfinerCache();

            if (logDebugInfo)
            {
                Debug.Log($"[CameraConfiner] 边界已更新: Center={_worldBounds.center}, Size={_worldBounds.size}");
            }
        }

        #endregion


        #region 私有方法

        /// <summary>
        /// 设置 Confiner2D 组件
        /// </summary>
        private void SetupConfiner()
        {
            if (cinemachineCamera == null) return;

            // 获取或添加 CinemachineConfiner2D
            _confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
            if (_confiner == null)
            {
                _confiner = cinemachineCamera.gameObject.AddComponent<CinemachineConfiner2D>();
                if (logDebugInfo)
                    Debug.Log("[CameraConfiner] 已添加 CinemachineConfiner2D 组件");
            }

            // 创建或获取边界碰撞体
            if (boundingCollider == null)
            {
                // 在场景中创建一个专用的边界物体
                GameObject boundsObj = GameObject.Find(AutoBoundsObjectName);
                if (boundsObj == null)
                {
                    boundsObj = new GameObject(AutoBoundsObjectName);
                }

                boundingCollider = boundsObj.GetComponent<PolygonCollider2D>();
                if (boundingCollider == null)
                {
                    boundingCollider = boundsObj.AddComponent<PolygonCollider2D>();
                    boundingCollider.isTrigger = true;
                }

                if (logDebugInfo)
                    Debug.Log("[CameraConfiner] 已创建边界碰撞体");
            }

            // 设置 Confiner 的边界碰撞体
            NormalizeAutoBoundsTransform();
            _confiner.BoundingShape2D = boundingCollider;
        }

        /// <summary>
        /// 验证必要引用
        /// </summary>
        private void ValidateReferences()
        {
            _isInitialized = true;

            if (cinemachineCamera == null)
            {
                Debug.LogWarning("[CameraConfiner] 未找到 CinemachineCamera，功能已禁用");
                _isInitialized = false;
            }

            if (_confiner == null)
            {
                Debug.LogWarning("[CameraConfiner] 未找到 CinemachineConfiner2D，功能已禁用");
                _isInitialized = false;
            }

            if (boundingCollider == null)
            {
                Debug.LogWarning("[CameraConfiner] 未找到边界碰撞体，功能已禁用");
                _isInitialized = false;
            }

            if (mainCamera == null)
            {
                Debug.LogWarning("[CameraConfiner] 未找到主摄像头，功能已禁用");
                _isInitialized = false;
            }
        }

        /// <summary>
        /// 场景加载回调
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (logDebugInfo)
                Debug.Log($"[CameraConfiner] 场景加载: {scene.name}，刷新边界");

            RefreshSceneReferences(scene);

            StopSceneLoadRecovery();
            _sceneLoadRecoveryCoroutine = StartCoroutine(DelayedRefresh(scene));
        }

        private System.Collections.IEnumerator DelayedRefresh(Scene scene)
        {
            for (int attempt = 0; attempt < SceneLoadTrackingTargetRetryFrames; attempt++)
            {
                yield return null;
                RefreshSceneReferences(scene);

                if (HasUsableTrackingTarget())
                {
                    break;
                }
            }

            RefreshBounds();
            _sceneLoadRecoveryCoroutine = null;
        }

        private void StopSceneLoadRecovery()
        {
            if (_sceneLoadRecoveryCoroutine == null)
            {
                return;
            }

            StopCoroutine(_sceneLoadRecoveryCoroutine);
            _sceneLoadRecoveryCoroutine = null;
        }

        private void RefreshSceneReferences()
        {
            RefreshSceneReferences(SceneManager.GetActiveScene());
        }

        private void RefreshSceneReferences(Scene preferredScene)
        {
            CinemachineCamera resolvedCinemachine = ResolveCinemachineCamera();
            if (resolvedCinemachine != cinemachineCamera)
            {
                cinemachineCamera = resolvedCinemachine;
            }

            UnityEngine.Camera resolvedMainCamera = ResolveMainCamera();
            if (resolvedMainCamera != mainCamera)
            {
                mainCamera = resolvedMainCamera;
                _capturedDefaultCameraRect = false;
            }

            EnsureCinemachineBrain();
            TryBindTrackingTarget(preferredScene);
            CaptureDefaultCameraRect();
            SetupConfiner();
            ValidateReferences();
        }

        private void EnsureCinemachineBrain()
        {
            if (mainCamera == null)
            {
                _brain = null;
                return;
            }

            _brain = mainCamera.GetComponent<CinemachineBrain>();
            if (_brain == null)
            {
                _brain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
                if (logDebugInfo)
                {
                    Debug.Log($"[CameraConfiner] 已为主相机补挂 CinemachineBrain: {mainCamera.name}", mainCamera);
                }
            }

            if (!_brain.enabled)
            {
                _brain.enabled = true;
            }
        }

        private void TryBindTrackingTarget(Scene preferredScene)
        {
            if (cinemachineCamera == null)
            {
                return;
            }

            Transform resolvedTarget = ResolveTrackingTarget(preferredScene);
            if (!IsUsableTrackingTarget(resolvedTarget))
            {
                return;
            }

            if (cinemachineCamera.Follow == resolvedTarget)
            {
                return;
            }

            cinemachineCamera.Follow = resolvedTarget;
            cinemachineCamera.PreviousStateIsValid = false;

            if (logDebugInfo)
            {
                Debug.Log($"[CameraConfiner] 已重绑跟随目标: {resolvedTarget.name}", resolvedTarget);
            }
        }

        private Transform ResolveTrackingTarget(Scene preferredScene)
        {
            Transform bestCandidate = null;
            int bestScore = int.MinValue;
            var playerMovements = FindObjectsByType<global::PlayerMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < playerMovements.Length; index++)
            {
                Transform candidate = playerMovements[index] != null ? playerMovements[index].transform : null;
                if (!IsUsableTrackingTarget(candidate))
                {
                    continue;
                }

                int score = ScoreTrackingTargetCandidate(candidate, preferredScene);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCandidate = candidate;
                }
            }

            if (bestCandidate != null)
            {
                return bestCandidate;
            }

            return null;
        }

        private bool HasUsableTrackingTarget()
        {
            if (cinemachineCamera == null || !IsUsableTrackingTarget(cinemachineCamera.Follow))
            {
                return false;
            }

            Transform bestCandidate = ResolveTrackingTarget(SceneManager.GetActiveScene());
            return bestCandidate != null && cinemachineCamera.Follow == bestCandidate;
        }

        private static bool IsUsableTrackingTarget(Transform candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            GameObject gameObject = candidate.gameObject;
            Scene scene = gameObject.scene;
            return gameObject.activeInHierarchy && scene.IsValid() && scene.isLoaded;
        }

        private static int ScoreTrackingTargetCandidate(Transform candidate, Scene preferredScene)
        {
            if (candidate == null)
            {
                return int.MinValue;
            }

            int score = 0;
            GameObject candidateObject = candidate.gameObject;
            Scene activeScene = SceneManager.GetActiveScene();

            if (candidateObject.TryGetComponent<global::PlayerMovement>(out _))
            {
                score += 100;
            }

            if (candidateObject.TryGetComponent<Rigidbody2D>(out _))
            {
                score += 40;
            }

            if (candidateObject.name == "Player")
            {
                score += 10;
            }

            if (preferredScene.IsValid() && candidateObject.scene == preferredScene)
            {
                score += 50;
            }

            if (activeScene.IsValid() && candidateObject.scene == activeScene)
            {
                score += 25;
            }

            return score;
        }

        private CinemachineCamera ResolveCinemachineCamera()
        {
            if (IsUsableSceneObject(cinemachineCamera))
            {
                return cinemachineCamera;
            }

            CinemachineCamera parentCamera = GetComponentInParent<CinemachineCamera>();
            if (IsUsableSceneObject(parentCamera))
            {
                return parentCamera;
            }

            Scene preferredScene = mainCamera != null
                ? mainCamera.gameObject.scene
                : SceneManager.GetActiveScene();
            CinemachineCamera bestCamera = null;
            int bestScore = int.MinValue;
            var cameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
            for (int index = 0; index < cameras.Length; index++)
            {
                CinemachineCamera candidate = cameras[index];
                if (!IsUsableSceneObject(candidate))
                {
                    continue;
                }

                int score = ScoreCinemachineCameraCandidate(candidate, preferredScene);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCamera = candidate;
                }
            }

            return bestCamera;
        }

        private UnityEngine.Camera ResolveMainCamera()
        {
            if (IsUsableWorldCamera(mainCamera))
            {
                return mainCamera;
            }

            Scene preferredScene = cinemachineCamera != null
                ? cinemachineCamera.gameObject.scene
                : SceneManager.GetActiveScene();

            UnityEngine.Camera bestCamera = null;
            int bestScore = int.MinValue;
            var cameras = FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None);
            for (int index = 0; index < cameras.Length; index++)
            {
                UnityEngine.Camera candidate = cameras[index];
                if (!IsUsableWorldCamera(candidate))
                {
                    continue;
                }

                int score = ScoreWorldCameraCandidate(candidate, preferredScene);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCamera = candidate;
                }
            }

            return bestCamera;
        }

        private static bool IsUsableSceneObject(UnityEngine.Object candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            if (candidate is Component component)
            {
                Scene scene = component.gameObject.scene;
                return component.gameObject.activeInHierarchy && scene.IsValid() && scene.isLoaded;
            }

            return true;
        }

        private static bool IsUsableWorldCamera(UnityEngine.Camera candidate)
        {
            return candidate != null &&
                   candidate.enabled &&
                   candidate.targetTexture == null &&
                   IsUsableSceneObject(candidate);
        }

        private static int ScoreWorldCameraCandidate(UnityEngine.Camera candidate, Scene preferredScene)
        {
            if (!IsUsableWorldCamera(candidate))
            {
                return int.MinValue;
            }

            int score = 0;
            Scene activeScene = SceneManager.GetActiveScene();
            Scene candidateScene = candidate.gameObject.scene;

            if (candidate.CompareTag("MainCamera"))
            {
                score += 220;
            }

            CinemachineBrain brain = candidate.GetComponent<CinemachineBrain>();
            if (brain != null && brain.enabled)
            {
                score += 200;
            }

            if (preferredScene.IsValid() && preferredScene.isLoaded && candidateScene == preferredScene)
            {
                score += 180;
            }

            if (activeScene.IsValid() && activeScene.isLoaded && candidateScene == activeScene)
            {
                score += 140;
            }

            score += Mathf.RoundToInt(Mathf.Clamp(candidate.depth, -20f, 20f));
            return score;
        }

        private static int ScoreCinemachineCameraCandidate(CinemachineCamera candidate, Scene preferredScene)
        {
            if (!IsUsableSceneObject(candidate))
            {
                return int.MinValue;
            }

            int score = 0;
            Scene activeScene = SceneManager.GetActiveScene();
            Scene candidateScene = candidate.gameObject.scene;

            if (preferredScene.IsValid() && preferredScene.isLoaded && candidateScene == preferredScene)
            {
                score += 180;
            }

            if (activeScene.IsValid() && activeScene.isLoaded && candidateScene == activeScene)
            {
                score += 140;
            }

            if (candidate.Follow != null && candidate.Follow.TryGetComponent<global::PlayerMovement>(out _))
            {
                score += 220;
            }

            if (candidate.Priority.Enabled)
            {
                score += Mathf.RoundToInt(Mathf.Clamp(candidate.Priority.Value, -50f, 50f));
            }

            return score;
        }


        /// <summary>
        /// 自动检测世界边界（基于 Tilemap）
        /// </summary>
        private void DetectWorldBounds()
        {
            if (TryCalculateExplicitBounds(out Bounds explicitBounds))
            {
                _worldBounds = explicitBounds;
                if (logDebugInfo)
                {
                    Debug.Log($"[CameraConfiner] 使用显式 bounds source: Center={_worldBounds.center}, Size={_worldBounds.size}");
                }

                return;
            }

            if (TryCalculateAutoBounds(out Bounds autoBounds))
            {
                _worldBounds = autoBounds;
                if (logDebugInfo)
                {
                    Debug.Log($"[CameraConfiner] 自动检测到世界边界: Center={_worldBounds.center}, Size={_worldBounds.size}");
                }

                return;
            }

            Debug.LogWarning("[CameraConfiner] 未检测到任何有效 bounds source，使用手动边界");
            _worldBounds = manualBounds;
        }

        /// <summary>
        /// 检查物体是否在指定世界层级下
        /// </summary>
        private bool IsInWorldLayers(Transform t)
        {
            if (worldLayerNames == null || worldLayerNames.Length == 0)
                return true;

            Transform current = t;
            while (current != null)
            {
                foreach (var layerName in worldLayerNames)
                {
                    if (current.name == layerName)
                        return true;
                }
                current = current.parent;
            }
            return false;
        }

        private bool TryCalculateExplicitBounds(out Bounds bounds)
        {
            bounds = default;
            bool hasBounds = false;

            Tilemap[] normalizedTilemaps = NormalizeTilemaps(explicitBoundsTilemaps);
            for (int index = 0; index < normalizedTilemaps.Length; index++)
            {
                if (TryGetTilemapWorldBounds(normalizedTilemaps[index], out Bounds tilemapBounds))
                {
                    EncapsulateBounds(ref bounds, ref hasBounds, tilemapBounds);
                }
            }

            Collider2D[] normalizedColliders = NormalizeColliders(explicitBoundsColliders);
            for (int index = 0; index < normalizedColliders.Length; index++)
            {
                Collider2D collider = normalizedColliders[index];
                if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
                {
                    continue;
                }

                EncapsulateBounds(ref bounds, ref hasBounds, collider.bounds);
            }

            if (!hasBounds)
            {
                return false;
            }

            ApplyBoundsPadding(ref bounds, applyAutoInset: false);
            return true;
        }

        private bool TryCalculateAutoBounds(out Bounds bounds)
        {
            bounds = default;
            bool hasBounds = false;

            Tilemap[] autoTilemaps = SelectAutoBoundsTilemaps();
            for (int index = 0; index < autoTilemaps.Length; index++)
            {
                if (TryGetTilemapWorldBounds(autoTilemaps[index], out Bounds tilemapBounds))
                {
                    EncapsulateBounds(ref bounds, ref hasBounds, tilemapBounds);
                }
            }

            // world layer 下已有 Tilemap 并集时，以三层 Tilemap 的并集为准；Sprite 只作缺省 fallback。
            if (includeWorldSpriteRenderersInAutoBounds && autoTilemaps.Length == 0)
            {
                SpriteRenderer[] spriteRenderers = SelectAutoBoundsSpriteRenderers();
                for (int index = 0; index < spriteRenderers.Length; index++)
                {
                    SpriteRenderer spriteRenderer = spriteRenderers[index];
                    if (spriteRenderer == null || !spriteRenderer.enabled || !spriteRenderer.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    Bounds rendererBounds = spriteRenderer.bounds;
                    if (rendererBounds.size.x <= 0f || rendererBounds.size.y <= 0f)
                    {
                        continue;
                    }

                    EncapsulateBounds(ref bounds, ref hasBounds, rendererBounds);
                }
            }

            if (!hasBounds)
            {
                Collider2D[] autoColliders = SelectAutoBoundsColliders();
                for (int index = 0; index < autoColliders.Length; index++)
                {
                    Collider2D collider = autoColliders[index];
                    if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    EncapsulateBounds(ref bounds, ref hasBounds, collider.bounds);
                }
            }

            if (!hasBounds)
            {
                return false;
            }

            ApplyBoundsPadding(ref bounds, applyAutoInset: true);
            return true;
        }

        private Tilemap[] SelectAutoBoundsTilemaps()
        {
            var results = new System.Collections.Generic.List<Tilemap>();
            var tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            foreach (Tilemap tilemap in tilemaps)
            {
                if (!ShouldIncludeTilemapInAutoBounds(tilemap))
                {
                    continue;
                }

                results.Add(tilemap);
            }

            return results.ToArray();
        }

        private static bool MatchesAnyExactName(Tilemap tilemap, string[] names)
        {
            if (tilemap == null || names == null || names.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < names.Length; index++)
            {
                string candidateName = names[index];
                if (string.IsNullOrWhiteSpace(candidateName))
                {
                    continue;
                }

                if (string.Equals(tilemap.name, candidateName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private int ComparePreferredTilemaps(Tilemap left, Tilemap right)
        {
            if (left == right)
            {
                return 0;
            }

            if (left == null)
            {
                return 1;
            }

            if (right == null)
            {
                return -1;
            }

            int leftPriority = GetExactNamePriority(left.name, preferredExactTilemapNames);
            int rightPriority = GetExactNamePriority(right.name, preferredExactTilemapNames);
            if (leftPriority != rightPriority)
            {
                return leftPriority.CompareTo(rightPriority);
            }

            float rightArea = GetTilemapBoundsArea(right);
            float leftArea = GetTilemapBoundsArea(left);
            return rightArea.CompareTo(leftArea);
        }

        private Collider2D[] SelectAutoBoundsColliders()
        {
            var results = new System.Collections.Generic.List<Collider2D>();
            var colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
            foreach (Collider2D collider in colliders)
            {
                if (collider == null ||
                    !collider.enabled ||
                    !collider.gameObject.activeInHierarchy ||
                    !IsInWorldLayers(collider.transform) ||
                    ShouldExcludeFromAutoBounds(collider.transform))
                {
                    continue;
                }

                results.Add(collider);
            }

            return results.ToArray();
        }

        private SpriteRenderer[] SelectAutoBoundsSpriteRenderers()
        {
            var results = new System.Collections.Generic.List<SpriteRenderer>();
            var spriteRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer == null ||
                    !spriteRenderer.enabled ||
                    !spriteRenderer.gameObject.activeInHierarchy ||
                    !IsInWorldLayers(spriteRenderer.transform) ||
                    ShouldExcludeFromAutoBounds(spriteRenderer.transform))
                {
                    continue;
                }

                results.Add(spriteRenderer);
            }

            return results.ToArray();
        }

        private bool ShouldIncludeTilemapInAutoBounds(Tilemap tilemap)
        {
            if (tilemap == null ||
                !tilemap.isActiveAndEnabled ||
                !IsInWorldLayers(tilemap.transform))
            {
                return false;
            }

            return TryGetTilemapWorldBounds(tilemap, out _);
        }

        private bool ShouldExcludeFromAutoBounds(Transform target)
        {
            return MatchesAnyKeywordInHierarchy(target, GetEffectiveExcludedAutoBoundsKeywords());
        }

        private static bool MatchesAnyKeywordInHierarchy(Transform target, string[] keywords)
        {
            if (target == null || keywords == null || keywords.Length == 0)
            {
                return false;
            }

            Transform current = target;
            while (current != null)
            {
                string loweredName = current.name.ToLowerInvariant();
                for (int index = 0; index < keywords.Length; index++)
                {
                    string keyword = keywords[index];
                    if (!string.IsNullOrWhiteSpace(keyword) && loweredName.Contains(keyword.Trim().ToLowerInvariant()))
                    {
                        return true;
                    }
                }

                current = current.parent;
            }

            return false;
        }

        private void ApplyBoundsPadding(ref Bounds bounds, bool applyAutoInset)
        {
            if (boundsPadding > 0f)
            {
                bounds.Expand(boundsPadding * 2f);
            }

            if (applyAutoInset && autoBoundsInset > 0f)
            {
                float insetX = Mathf.Min(autoBoundsInset * 2f, Mathf.Max(0f, bounds.size.x - 0.1f));
                float insetY = Mathf.Min(autoBoundsInset * 2f, Mathf.Max(0f, bounds.size.y - 0.1f));
                if (insetX > 0f || insetY > 0f)
                {
                    bounds.Expand(new Vector3(-insetX, -insetY, 0f));
                }
            }
        }

        private static bool TryGetTilemapWorldBounds(Tilemap tilemap, out Bounds bounds)
        {
            bounds = default;
            if (tilemap == null || !tilemap.gameObject.activeInHierarchy)
            {
                return false;
            }

            BoundsInt cellBounds = tilemap.cellBounds;
            if (cellBounds.size.x <= 0 || cellBounds.size.y <= 0)
            {
                return false;
            }

            bool hasBounds = false;
            Bounds occupiedBounds = default;
            foreach (Vector3Int cellPosition in cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(cellPosition) || !TryGetTileWorldBounds(tilemap, cellPosition, out Bounds tileBounds))
                {
                    continue;
                }

                EncapsulateBounds(ref occupiedBounds, ref hasBounds, tileBounds);
            }

            if (hasBounds)
            {
                bounds = occupiedBounds;
                return true;
            }

            Bounds localBounds = tilemap.localBounds;
            if (localBounds.size.x <= 0f || localBounds.size.y <= 0f)
            {
                return false;
            }

            Vector3 worldMin = tilemap.transform.TransformPoint(localBounds.min);
            Vector3 worldMax = tilemap.transform.TransformPoint(localBounds.max);
            bounds.SetMinMax(Vector3.Min(worldMin, worldMax), Vector3.Max(worldMin, worldMax));
            return true;
        }

        private static bool TryGetTileWorldBounds(Tilemap tilemap, Vector3Int cellPosition, out Bounds bounds)
        {
            bounds = default;
            if (tilemap == null)
            {
                return false;
            }

            Vector3 localMin = tilemap.CellToLocal(cellPosition);
            Vector3 localMax = tilemap.CellToLocal(new Vector3Int(cellPosition.x + 1, cellPosition.y + 1, cellPosition.z));
            Vector3 worldMin = tilemap.transform.TransformPoint(localMin);
            Vector3 worldMax = tilemap.transform.TransformPoint(localMax);
            bounds.SetMinMax(Vector3.Min(worldMin, worldMax), Vector3.Max(worldMin, worldMax));
            return bounds.size.x > 0f && bounds.size.y > 0f;
        }

        private static float GetTilemapBoundsArea(Tilemap tilemap)
        {
            if (TryGetTilemapWorldBounds(tilemap, out Bounds bounds))
            {
                return Mathf.Abs(bounds.size.x * bounds.size.y);
            }

            return 0f;
        }

        private static void EncapsulateBounds(ref Bounds totalBounds, ref bool hasBounds, Bounds candidate)
        {
            if (!hasBounds)
            {
                totalBounds = candidate;
                hasBounds = true;
                return;
            }

            totalBounds.Encapsulate(candidate.min);
            totalBounds.Encapsulate(candidate.max);
        }

        private static Tilemap[] NormalizeTilemaps(Tilemap[] source)
        {
            if (source == null || source.Length == 0)
            {
                return new Tilemap[0];
            }

            var results = new System.Collections.Generic.List<Tilemap>(source.Length);
            foreach (Tilemap tilemap in source)
            {
                if (tilemap != null && !results.Contains(tilemap))
                {
                    results.Add(tilemap);
                }
            }

            return results.ToArray();
        }

        private static Collider2D[] NormalizeColliders(Collider2D[] source)
        {
            if (source == null || source.Length == 0)
            {
                return new Collider2D[0];
            }

            var results = new System.Collections.Generic.List<Collider2D>(source.Length);
            foreach (Collider2D collider in source)
            {
                if (collider != null && !results.Contains(collider))
                {
                    results.Add(collider);
                }
            }

            return results.ToArray();
        }

        private string[] GetEffectiveExcludedAutoBoundsKeywords()
        {
            if (excludedAutoBoundsKeywords == null || excludedAutoBoundsKeywords.Length == 0)
            {
                return System.Array.Empty<string>();
            }

            if (explicitBoundsTilemaps.Length == 0 &&
                explicitBoundsColliders.Length == 0 &&
                MatchesKeywordSet(excludedAutoBoundsKeywords, LegacyOvertightExcludedKeywords))
            {
                return SoftenedExcludedKeywords;
            }

            return excludedAutoBoundsKeywords;
        }

        private static bool MatchesKeywordSet(string[] candidate, string[] expected)
        {
            if (candidate == null || expected == null || candidate.Length != expected.Length)
            {
                return false;
            }

            for (int index = 0; index < candidate.Length; index++)
            {
                if (!string.Equals(candidate[index]?.Trim(), expected[index], System.StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetExactNamePriority(string candidateName, string[] preferredNames)
        {
            if (string.IsNullOrWhiteSpace(candidateName) || preferredNames == null)
            {
                return int.MaxValue;
            }

            for (int index = 0; index < preferredNames.Length; index++)
            {
                string preferredName = preferredNames[index];
                if (string.IsNullOrWhiteSpace(preferredName))
                {
                    continue;
                }

                if (string.Equals(candidateName, preferredName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return int.MaxValue;
        }

        /// <summary>
        /// 更新边界碰撞体的形状
        /// </summary>
        private void UpdateBoundingCollider()
        {
            if (boundingCollider == null) return;

            NormalizeAutoBoundsTransform();

            // 创建矩形边界的顶点（顺时针）
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(_worldBounds.min.x, _worldBounds.min.y); // 左下
            points[1] = new Vector2(_worldBounds.min.x, _worldBounds.max.y); // 左上
            points[2] = new Vector2(_worldBounds.max.x, _worldBounds.max.y); // 右上
            points[3] = new Vector2(_worldBounds.max.x, _worldBounds.min.y); // 右下

            boundingCollider.SetPath(0, points);

            if (logDebugInfo)
            {
                Debug.Log($"[CameraConfiner] 边界碰撞体已更新: {points[0]} -> {points[2]}");
            }
        }

        /// <summary>
        /// 使 Confiner 缓存失效，强制重新计算
        /// </summary>
        private void InvalidateConfinerCache()
        {
            if (_confiner != null)
            {
                _confiner.InvalidateBoundingShapeCache();
            }
        }

        private void NormalizeAutoBoundsTransform()
        {
            if (boundingCollider == null || boundingCollider.name != AutoBoundsObjectName)
            {
                return;
            }

            Transform boundsTransform = boundingCollider.transform;
            if (boundsTransform.parent != null)
            {
                boundsTransform.SetParent(null, false);
            }

            boundsTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            boundsTransform.localScale = Vector3.one;
        }

        private void CaptureDefaultCameraRect()
        {
            if (_capturedDefaultCameraRect || mainCamera == null)
            {
                return;
            }

            _defaultCameraRect = mainCamera.rect;
            _capturedDefaultCameraRect = true;
        }

        private bool UpdateWideScreenViewportClamp()
        {
            if (mainCamera == null)
            {
                return false;
            }

            if (!ShouldApplyWideScreenViewportClamp(clampViewportOnWideScreens, clampViewportInWindowedMode, Screen.fullScreenMode))
            {
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                return RestoreDefaultCameraRect();
            }

            return ApplyWideScreenViewportClamp();
        }

        private static bool ShouldApplyWideScreenViewportClamp(
            bool clampViewportOnWideScreens,
            bool clampViewportInWindowedMode,
            FullScreenMode fullScreenMode)
        {
            if (!clampViewportOnWideScreens)
            {
                return false;
            }

            if (!clampViewportInWindowedMode && fullScreenMode == FullScreenMode.Windowed)
            {
                return false;
            }

            return true;
        }

        private bool ApplyWideScreenViewportClamp()
        {
            CaptureDefaultCameraRect();

            if (mainCamera == null || !_capturedDefaultCameraRect || !mainCamera.orthographic)
            {
                return false;
            }

            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            float visibleHeight = mainCamera.orthographicSize * 2f;
            if (visibleHeight <= 0.01f || _worldBounds.size.x <= 0.01f || Screen.height <= 0)
            {
                return RestoreDefaultCameraRect();
            }

            float screenAspect = Screen.width / (float)Screen.height;
            float maxSafeAspect = _worldBounds.size.x / visibleHeight;
            float targetAspect = maxSafeAspect * wideScreenViewportSafety;

            if (screenAspect > targetAspect + 0.001f && targetAspect > 0.01f)
            {
                float normalizedWidth = Mathf.Clamp01(targetAspect / screenAspect);
                float centeredX = (1f - normalizedWidth) * 0.5f;
                Rect desiredRect = new Rect(centeredX, _defaultCameraRect.y, normalizedWidth, _defaultCameraRect.height);
                return SetMainCameraRect(desiredRect);
            }

            return RestoreDefaultCameraRect();
        }

        private bool RestoreDefaultCameraRect()
        {
            if (mainCamera == null || !_capturedDefaultCameraRect)
            {
                return false;
            }

            return SetMainCameraRect(_defaultCameraRect);
        }

        private bool SetMainCameraRect(Rect desiredRect)
        {
            if (mainCamera == null)
            {
                return false;
            }

            Rect snappedRect = SnapViewportRectToPixelGrid(desiredRect);
            if (AreRectsApproximatelyEqual(mainCamera.rect, snappedRect))
            {
                return false;
            }

            mainCamera.rect = snappedRect;
            return true;
        }

        private Rect SnapViewportRectToPixelGrid(Rect rect)
        {
            if (!snapViewportClampToPixelGrid || Screen.width <= 0 || Screen.height <= 0)
            {
                return rect;
            }

            float xMin = Mathf.Round(rect.xMin * Screen.width) / Screen.width;
            float xMax = Mathf.Round(rect.xMax * Screen.width) / Screen.width;
            float yMin = Mathf.Round(rect.yMin * Screen.height) / Screen.height;
            float yMax = Mathf.Round(rect.yMax * Screen.height) / Screen.height;

            if (xMax <= xMin)
            {
                xMax = Mathf.Min(1f, xMin + (1f / Screen.width));
            }

            if (yMax <= yMin)
            {
                yMax = Mathf.Min(1f, yMin + (1f / Screen.height));
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        private static bool AreRectsApproximatelyEqual(Rect left, Rect right)
        {
            const float epsilon = 0.0001f;
            return Mathf.Abs(left.x - right.x) <= epsilon &&
                   Mathf.Abs(left.y - right.y) <= epsilon &&
                   Mathf.Abs(left.width - right.width) <= epsilon &&
                   Mathf.Abs(left.height - right.height) <= epsilon;
        }

        #endregion


        #region 编辑器方法

#if UNITY_EDITOR
        /// <summary>
        /// 在 Scene 视图绘制边界 Gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            // 绘制世界边界（绿色）
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawWireCube(_worldBounds.center, _worldBounds.size);
        }

        /// <summary>
        /// Inspector 按钮：手动刷新边界
        /// </summary>
        [ContextMenu("刷新边界")]
        private void DEBUG_RefreshBounds()
        {
            if (cinemachineCamera == null)
            {
                cinemachineCamera = ResolveCinemachineCamera();
            }

            if (mainCamera == null)
            {
                mainCamera = ResolveMainCamera();
            }

            SetupConfiner();
            ValidateReferences();

            if (_isInitialized)
            {
                RefreshBounds();
                Debug.Log("[CameraConfiner] 边界已刷新");
            }
        }
#endif

        #endregion
    }
}
