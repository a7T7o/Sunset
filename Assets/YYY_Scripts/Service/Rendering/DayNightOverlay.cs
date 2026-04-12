using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 昼夜全屏叠加组件。
/// 使用世界空间 Sprite 覆盖视口，并在 shader 中同时处理：
/// 1. 全局昼夜压色
/// 2. 夜晚玩家周围视野洞
/// 3. 路灯等局部暖光池
/// </summary>
[ExecuteAlways]
public class DayNightOverlay : MonoBehaviour
{
    public struct LightSample
    {
        public Vector2 position;
        public Color color;
        public float radius;
        public float feather;
        public float intensity;
        public float coreRatio;
    }

    private const string NightVisionShaderName = "Custom/NightVisionOverlay";
    private const string MultiplyShaderName = "Custom/SpriteMultiply";
    private const int MaxNightLights = 8;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int OuterColorId = Shader.PropertyToID("_OuterColor");
    private static readonly int VisionCenterId = Shader.PropertyToID("_VisionCenter");
    private static readonly int VisionParamsId = Shader.PropertyToID("_VisionParams");
    private static readonly int LightCountId = Shader.PropertyToID("_LightCount");
    private static readonly int LightPositionsId = Shader.PropertyToID("_LightPositions");
    private static readonly int LightColorsId = Shader.PropertyToID("_LightColors");
    private static readonly int LightDataId = Shader.PropertyToID("_LightData");

    [Header("━━━━ 渲染设置 ━━━━")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material overlayMaterial;

    [Header("━━━━ 覆盖参数 ━━━━")]
    [SerializeField] private float coverageMargin = 1.25f;
    [SerializeField] private string playerTag = "Player";

    [Header("━━━━ Sorting 设置 ━━━━")]
    [SerializeField] private string sortingLayerName = "CloudShadow";
    [SerializeField] private int sortingOrder = 100;

    private Camera mainCamera;
    private Transform playerTransform;
    private Color currentColor = Color.white;
    private float currentStrength = 1f;
    private float currentVisionRadiusNormalized = 1.4f;
    private float currentVisionSoftness = 0.2f;
    private float currentVisionStrength = 0f;
    private float currentOuterDarkness = 0f;
    private float currentVisionAspect = 0.86f;
    private Texture2D whiteTexture;
    private Sprite whiteSprite;
    private Material runtimeMaterial;
    private bool supportsNightVisionShader;
    private readonly LightSample[] activeLights = new LightSample[MaxNightLights];
    private readonly Vector4[] shaderLightPositions = new Vector4[MaxNightLights];
    private readonly Vector4[] shaderLightColors = new Vector4[MaxNightLights];
    private readonly Vector4[] shaderLightData = new Vector4[MaxNightLights];
    private int activeLightCount;
    private float nextPlayerSearchTime;
    private bool overlayRenderingAvailable = true;
    private bool overlayVisible = true;
    private bool editorGlobalScenePreview;
#if UNITY_EDITOR
    private double nextEditorSceneBoundsRefreshTime;
#endif

    private void Awake()
    {
        EnsureInitialized();
    }

    private void OnEnable()
    {
        EnsureInitialized();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EditorApplication.delayCall -= HandleEditorValidate;
        EditorApplication.delayCall += HandleEditorValidate;
    }

    private void HandleEditorValidate()
    {
        if (this == null)
        {
            return;
        }

        EnsureInitialized();
        SceneView.RepaintAll();
    }
#endif

    private void LateUpdate()
    {
        if (!overlayVisible)
        {
            return;
        }

        if (mainCamera == null)
        {
            CacheCamera();
            if (mainCamera == null)
            {
                return;
            }
        }

        if (playerTransform == null)
        {
            RefreshPlayerTransformIfNeeded();
        }

        UpdateSpriteSize();
        UpdateShaderRuntimeState();
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            DestroyOverlayAsset(runtimeMaterial);
            runtimeMaterial = null;
        }

        if (whiteTexture != null)
        {
            DestroyOverlayAsset(whiteTexture);
            whiteTexture = null;
        }

        if (whiteSprite != null)
        {
            DestroyOverlayAsset(whiteSprite);
            whiteSprite = null;
        }
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        ApplyOverlayState();
    }

    public void SetStrength(float strength)
    {
        currentStrength = Mathf.Clamp01(strength);
        ApplyOverlayState();
    }

    public void SetVisionProfile(float radiusNormalized, float softness, float strength, float outerDarkness)
    {
        currentVisionRadiusNormalized = Mathf.Max(0.1f, radiusNormalized);
        currentVisionSoftness = Mathf.Clamp(softness, 0.05f, 1.2f);
        currentVisionStrength = Mathf.Clamp01(strength);
        currentOuterDarkness = Mathf.Clamp01(outerDarkness);
        ApplyOverlayState();
    }

    public void SetVisionAspect(float aspect)
    {
        currentVisionAspect = Mathf.Clamp(aspect, 0.5f, 1.4f);
        ApplyOverlayState();
    }

    public void SetVisionFocus(Transform focusTarget)
    {
        playerTransform = focusTarget;
        UpdateShaderRuntimeState();
    }

    public void SetVisible(bool visible)
    {
        overlayVisible = visible;
        ApplyOverlayState();
    }

    public void SetEditorGlobalScenePreview(bool enabled)
    {
        editorGlobalScenePreview = enabled;
        UpdateShaderRuntimeState();
    }

    public void SetNightLights(LightSample[] lights, int count)
    {
        activeLightCount = Mathf.Clamp(count, 0, MaxNightLights);
        for (int i = 0; i < activeLightCount; i++)
        {
            activeLights[i] = lights[i];
        }

        for (int i = activeLightCount; i < MaxNightLights; i++)
        {
            activeLights[i] = new LightSample();
            shaderLightPositions[i] = Vector4.zero;
            shaderLightColors[i] = Vector4.zero;
            shaderLightData[i] = Vector4.zero;
        }

        UpdateShaderRuntimeState();
    }

    private void InitializeSpriteRenderer()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        Shader overlayShader = Shader.Find(NightVisionShaderName);
        if (overlayShader != null)
        {
            supportsNightVisionShader = true;
            runtimeMaterial = new Material(overlayShader);
        }
        else
        {
            Shader multiplyShader = Shader.Find(MultiplyShaderName);
            supportsNightVisionShader = false;
            runtimeMaterial = multiplyShader != null
                ? new Material(multiplyShader)
                : (overlayMaterial != null ? new Material(overlayMaterial) : null);
            if (runtimeMaterial == null)
            {
                Debug.LogError("[DayNightOverlay] 未找到 NightVisionOverlay/SpriteMultiply shader，Overlay 将无法正常生效。");
            }
        }

        if (runtimeMaterial != null)
        {
            spriteRenderer.material = runtimeMaterial;
            spriteRenderer.enabled = true;
            overlayRenderingAvailable = true;
        }
        else
        {
            overlayRenderingAvailable = false;
            spriteRenderer.enabled = false;
        }
    }

    private void GenerateWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return;
        }

        whiteTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        whiteTexture.filterMode = FilterMode.Point;

        Color[] pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        whiteTexture.SetPixels(pixels);
        whiteTexture.Apply();

        whiteSprite = Sprite.Create(
            whiteTexture,
            new Rect(0f, 0f, 4f, 4f),
            new Vector2(0.5f, 0.5f),
            4f);

        spriteRenderer.sprite = whiteSprite;
    }

    private void ApplySortingSettings()
    {
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    private void CacheCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

#if UNITY_EDITOR
        if (!Application.isPlaying && mainCamera == null && SceneView.lastActiveSceneView != null)
        {
            mainCamera = SceneView.lastActiveSceneView.camera;
        }
#endif
    }

    private void CachePlayerTransform()
    {
        GameObject taggedPlayer = GameObject.FindGameObjectWithTag(playerTag);
        if (taggedPlayer != null)
        {
            playerTransform = taggedPlayer.transform;
            return;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(playerTag);
        if (taggedObjects != null && taggedObjects.Length > 0)
        {
            playerTransform = taggedObjects[0].transform;
        }
    }

    private void RefreshPlayerTransformIfNeeded()
    {
        if (!Application.isPlaying)
        {
            CachePlayerTransform();
            return;
        }

        if (Time.time < nextPlayerSearchTime)
        {
            return;
        }

        nextPlayerSearchTime = Time.time + 1f;
        CachePlayerTransform();
    }

    private void ApplyOverlayState()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (!overlayRenderingAvailable || !overlayVisible)
        {
            spriteRenderer.enabled = false;
            return;
        }

        Color baseColor = Color.Lerp(Color.white, currentColor, currentStrength);
        baseColor.a = 1f;

        if (!supportsNightVisionShader || runtimeMaterial == null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = baseColor;
            return;
        }

        spriteRenderer.enabled = true;
        Color outerColor = new Color(
            Mathf.Clamp01(baseColor.r * (1f - currentOuterDarkness)),
            Mathf.Clamp01(baseColor.g * (1f - currentOuterDarkness)),
            Mathf.Clamp01(baseColor.b * (1f - currentOuterDarkness)),
            1f);

        runtimeMaterial.SetColor(BaseColorId, baseColor);
        runtimeMaterial.SetColor(OuterColorId, outerColor);
        spriteRenderer.color = Color.white;
        UpdateShaderRuntimeState();
    }

    private void UpdateShaderRuntimeState()
    {
        if (!supportsNightVisionShader || runtimeMaterial == null || mainCamera == null)
        {
            return;
        }

        Vector2 focusPosition = ResolveFocusPosition();
        float orthoSize = mainCamera.orthographicSize;
        float aspect = Mathf.Max(mainCamera.aspect, 0.01f);
        float viewHeight = orthoSize * 2f;
        float viewWidth = viewHeight * aspect;
        float maxViewDimension = Mathf.Max(viewWidth, viewHeight);
        float radiusX = maxViewDimension * currentVisionRadiusNormalized * 0.5f;
        float radiusY = radiusX * currentVisionAspect;

        runtimeMaterial.SetVector(VisionCenterId, new Vector4(focusPosition.x, focusPosition.y, 0f, 0f));
        runtimeMaterial.SetVector(
            VisionParamsId,
            new Vector4(radiusX, radiusY, currentVisionSoftness, currentVisionStrength));

        for (int i = 0; i < activeLightCount; i++)
        {
            LightSample sample = activeLights[i];
            shaderLightPositions[i] = new Vector4(sample.position.x, sample.position.y, 0f, 0f);
            shaderLightColors[i] = new Vector4(sample.color.r, sample.color.g, sample.color.b, 1f);
            shaderLightData[i] = new Vector4(sample.radius, sample.feather, sample.intensity, sample.coreRatio);
        }

        runtimeMaterial.SetFloat(LightCountId, activeLightCount);
        runtimeMaterial.SetVectorArray(LightPositionsId, shaderLightPositions);
        runtimeMaterial.SetVectorArray(LightColorsId, shaderLightColors);
        runtimeMaterial.SetVectorArray(LightDataId, shaderLightData);
    }

    private Vector2 ResolveFocusPosition()
    {
        if (playerTransform != null)
        {
            return playerTransform.position;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying && SceneView.lastActiveSceneView != null)
        {
            return SceneView.lastActiveSceneView.pivot;
        }
#endif

        return transform.position;
    }

    private void UpdateSpriteSize()
    {
        if (mainCamera == null || spriteRenderer == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying && editorGlobalScenePreview && TryApplyEditorSceneCoverage())
        {
            return;
        }
#endif

        float orthoSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;
        float viewHeight = orthoSize * 2f;
        float viewWidth = viewHeight * aspect;
        float targetWidth = viewWidth * coverageMargin;
        float targetHeight = viewHeight * coverageMargin;

        Vector3 cameraPosition = mainCamera.transform.position;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);
        transform.localScale = new Vector3(targetWidth, targetHeight, 1f);
    }

#if UNITY_EDITOR
    private bool TryApplyEditorSceneCoverage()
    {
        double now = EditorApplication.timeSinceStartup;
        if (now < nextEditorSceneBoundsRefreshTime)
        {
            return true;
        }

        nextEditorSceneBoundsRefreshTime = now + 0.2d;

        if (!TryComputeSceneRenderBounds(out Bounds sceneBounds))
        {
            return false;
        }

        Vector3 center = sceneBounds.center;
        center.z = 0f;

        float targetWidth = Mathf.Max(sceneBounds.size.x * coverageMargin, 1f);
        float targetHeight = Mathf.Max(sceneBounds.size.y * coverageMargin, 1f);

        transform.position = center;
        transform.localScale = new Vector3(targetWidth, targetHeight, 1f);
        return true;
    }

    private bool TryComputeSceneRenderBounds(out Bounds sceneBounds)
    {
        sceneBounds = default;
        bool hasBounds = false;
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer candidate = renderers[i];
            if (candidate == null || candidate == spriteRenderer)
            {
                continue;
            }

            if (!candidate.enabled || !candidate.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (candidate.gameObject.scene != gameObject.scene)
            {
                continue;
            }

            if (!hasBounds)
            {
                sceneBounds = candidate.bounds;
                hasBounds = true;
            }
            else
            {
                sceneBounds.Encapsulate(candidate.bounds);
            }
        }

        return hasBounds;
    }
#endif

    private void EnsureInitialized()
    {
        InitializeSpriteRenderer();
        GenerateWhiteSprite();
        ApplySortingSettings();
        CacheCamera();
        CachePlayerTransform();
        ApplyOverlayState();
    }

    private static void DestroyOverlayAsset(Object target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
            return;
        }

#if UNITY_EDITOR
        DestroyImmediate(target);
#else
        Destroy(target);
#endif
    }
}
