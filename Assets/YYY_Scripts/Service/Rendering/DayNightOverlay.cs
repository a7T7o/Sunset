using UnityEngine;

/// <summary>
/// 昼夜全屏颜色叠加组件（路线 B 核心）
/// 使用 SpriteRenderer + Multiply 混合模式实现全屏色调叠加
/// 在 LateUpdate 中跟随摄像机位置，动态调整尺寸覆盖整个视口
/// </summary>
public class DayNightOverlay : MonoBehaviour
{
    // ═══ Inspector 配置 ═══

    [Header("━━━━ 渲染设置 ━━━━")]
    [Tooltip("SpriteRenderer 组件引用，为空时自动获取或创建")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Multiply 混合模式材质，为空时使用 SpriteRenderer 默认材质")]
    [SerializeField] private Material multiplyMaterial;

    [Header("━━━━ 覆盖参数 ━━━━")]
    [Tooltip("视口覆盖边距倍率，确保摄像机移动时不露边")]
    [SerializeField] private float coverageMargin = 1.2f;

    [Header("━━━━ Sorting 设置 ━━━━")]
    [Tooltip("Sorting Layer 名称")]
    [SerializeField] private string sortingLayerName = "CloudShadow";

    [Tooltip("Sorting Order（高于 CloudShadowManager 的 0）")]
    [SerializeField] private int sortingOrder = 100;

    // ═══ 内部状态 ═══

    private Camera mainCamera;
    private Color currentColor = Color.white;
    private float currentStrength = 1f;

    // 程序生成的白色纹理和 Sprite（避免依赖外部资产）
    private Texture2D whiteTexture;
    private Sprite whiteSprite;

    // ═══ 生命周期 ═══

    private void Awake()
    {
        InitializeSpriteRenderer();
        GenerateWhiteSprite();
        ApplySortingSettings();
        CacheCamera();
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
        {
            CacheCamera();
            if (mainCamera == null) return;
        }

        // 跟随摄像机位置（XY），Z 保持 0 不干扰游戏物体
        Vector3 camPos = mainCamera.transform.position;
        transform.position = new Vector3(camPos.x, camPos.y, 0f);

        // 根据摄像机正交大小动态计算 Sprite 尺寸，确保覆盖整个视口
        UpdateSpriteSize();
    }

    private void OnDestroy()
    {
        // 清理程序生成的纹理资源
        if (whiteTexture != null)
        {
            Destroy(whiteTexture);
            whiteTexture = null;
        }
        if (whiteSprite != null)
        {
            Destroy(whiteSprite);
            whiteSprite = null;
        }
    }

    // ═══ 公共接口 ═══

    /// <summary>
    /// 设置叠加颜色（Multiply 混合模式下，白色 = 无变化，深色 = 变暗）
    /// </summary>
    public void SetColor(Color color)
    {
        currentColor = color;
        ApplyColorAndStrength();
    }

    /// <summary>
    /// 控制叠加强度（通过 RGB 向白色靠拢）
    /// 0 = 白色（无叠加效果），1 = 完全显示色调效果
    /// </summary>
    public void SetStrength(float strength)
    {
        currentStrength = Mathf.Clamp01(strength);
        ApplyColorAndStrength();
    }

    // ═══ 内部方法 ═══

    /// <summary>
    /// 初始化 SpriteRenderer，如果未在 Inspector 中指定则自动获取或创建
    /// </summary>
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

        // 优先使用 Inspector 指定的材质，否则自动查找 Custom/SpriteMultiply shader 创建材质
        if (multiplyMaterial != null)
        {
            spriteRenderer.material = multiplyMaterial;
        }
        else
        {
            Shader multiplyShader = Shader.Find("Custom/SpriteMultiply");
            if (multiplyShader != null)
            {
                multiplyMaterial = new Material(multiplyShader);
                spriteRenderer.material = multiplyMaterial;
            }
            else
            {
                Debug.LogError("[DayNightOverlay] 未找到 Custom/SpriteMultiply shader！Multiply 混合将不生效。请确认 SpriteMultiply.shader 已导入项目。");
            }
        }
    }

    /// <summary>
    /// 程序生成 4x4 纯白色纹理和 Sprite，避免依赖外部资产
    /// </summary>
    private void GenerateWhiteSprite()
    {
        whiteTexture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        whiteTexture.filterMode = FilterMode.Point;

        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++)
        {
            pixels[i] = Color.white;
        }
        whiteTexture.SetPixels(pixels);
        whiteTexture.Apply();

        // pixelsPerUnit = 4，使得 1 个 Sprite 单位 = 1 世界单位，方便缩放计算
        whiteSprite = Sprite.Create(
            whiteTexture,
            new Rect(0, 0, 4, 4),
            new Vector2(0.5f, 0.5f),
            4f
        );

        spriteRenderer.sprite = whiteSprite;
    }

    /// <summary>
    /// 应用 Sorting Layer 和 Order 设置
    /// </summary>
    private void ApplySortingSettings()
    {
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// 缓存主摄像机引用，Camera.main 为空时回退到 FindFirstObjectByType
    /// </summary>
    private void CacheCamera()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        if (mainCamera == null)
        {
            Debug.LogWarning("[DayNightOverlay] 未找到摄像机，Overlay 无法跟随视口");
        }
    }

    /// <summary>
    /// 将当前颜色和强度合并后应用到 SpriteRenderer
    /// Multiply 混合模式下：白色(1,1,1) = 无变化，深色 = 变暗并染色
    /// 强度通过 Color.Lerp(白色, 目标色, strength) 控制，Alpha 固定为 1
    /// </summary>
    private void ApplyColorAndStrength()
    {
        if (spriteRenderer == null) return;

        // Multiply 混合模式（Blend DstColor Zero）下：
        // - RGB 决定色调：白色 = 无效果，深色 = 变暗染色
        // - Alpha 不参与混合（被 shader 忽略），固定为 1
        // - 强度通过 RGB 向白色靠拢控制：strength=0 → 白色（无效果），strength=1 → 完全目标色
        Color finalColor = Color.Lerp(Color.white, currentColor, currentStrength);
        finalColor.a = 1f;

        spriteRenderer.color = finalColor;
    }

    /// <summary>
    /// 根据摄像机正交大小动态调整 Sprite 尺寸，确保覆盖整个视口
    /// </summary>
    private void UpdateSpriteSize()
    {
        if (mainCamera == null || spriteRenderer == null) return;

        float orthoSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;

        // 视口宽高（世界单位）
        float viewHeight = orthoSize * 2f;
        float viewWidth = viewHeight * aspect;

        // 乘以边距倍率，确保摄像机移动时不露边
        float targetWidth = viewWidth * coverageMargin;
        float targetHeight = viewHeight * coverageMargin;

        // Sprite 原始大小为 1x1 世界单位（4px / 4ppu），直接用目标尺寸作为缩放
        transform.localScale = new Vector3(targetWidth, targetHeight, 1f);
    }
}
