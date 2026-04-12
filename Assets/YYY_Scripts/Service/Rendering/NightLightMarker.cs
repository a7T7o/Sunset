using UnityEngine;

/// <summary>
/// 夜间光源标记组件。
/// 它既可以只作为 Overlay 夜灯锚点使用，
/// 也可以在需要时额外挂载 Light2D 走 URP 路线。
/// </summary>
public class NightLightMarker : MonoBehaviour
{
    [Header("━━━━ 光源参数 ━━━━")]

    [Tooltip("光源最大强度（夜间完全激活时的强度）")]
    [SerializeField] private float maxIntensity = 1.08f;

    [Tooltip("光源颜色")]
    [SerializeField] private Color lightColor = Color.yellow;

    [Tooltip("光源照射半径")]
    [SerializeField] private float radius = 4.2f;

    [Tooltip("光源软边比例，越大越柔和")]
    [Range(0.05f, 0.95f)]
    [SerializeField] private float feather = 0.58f;

    [Tooltip("在 Overlay fallback 路线里的额外权重")]
    [Range(0.1f, 2f)]
    [SerializeField] private float overlayWeight = 1.12f;

    [Header("━━━━ URP 扩展 ━━━━")]
    [Tooltip("勾上后，PointLightManager 会把它当作需要绑定 Light2D 的灯位。")]
    [SerializeField] private bool bindLight2D = false;

    [Header("━━━━ 动态表现 ━━━━")]
    [Tooltip("呼吸频率")]
    [Range(0f, 5f)]
    [SerializeField] private float pulseSpeed = 1.45f;

    [Tooltip("呼吸幅度")]
    [Range(0f, 1f)]
    [SerializeField] private float pulseAmount = 0.2f;

    [Tooltip("左右轻微摇曳距离（世界单位）")]
    [Range(0f, 1f)]
    [SerializeField] private float swayAmplitude = 0.16f;

    [Tooltip("摇曳频率")]
    [Range(0f, 5f)]
    [SerializeField] private float swaySpeed = 1.15f;

    [Tooltip("每盏灯的随机相位偏移")]
    [SerializeField] private float animationSeed = 0.0f;

    // ═══ 公共属性（供 PointLightManager 读取）═══

    /// <summary>
    /// 光源最大强度
    /// </summary>
    public float MaxIntensity => maxIntensity;

    /// <summary>
    /// 光源颜色
    /// </summary>
    public Color LightColor => lightColor;

    /// <summary>
    /// 光源照射半径
    /// </summary>
    public float Radius => radius;

    /// <summary>
    /// 光源软边
    /// </summary>
    public float Feather => feather;

    /// <summary>
    /// Overlay fallback 里的额外权重
    /// </summary>
    public float OverlayWeight => overlayWeight;

    public bool BindLight2D => bindLight2D;

    public float PulseSpeed => pulseSpeed;

    public float PulseAmount => pulseAmount;

    public float SwayAmplitude => swayAmplitude;

    public float SwaySpeed => swaySpeed;

    public float AnimationSeed => animationSeed;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawGizmo(0.35f, 0.12f);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmo(0.9f, 0.18f);
    }

    private void DrawGizmo(float lineAlpha, float fillAlpha)
    {
        Color gizmoColor = new Color(lightColor.r, lightColor.g, lightColor.b, lineAlpha);
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = new Color(lightColor.r, lightColor.g, lightColor.b, fillAlpha);
        Gizmos.DrawSphere(transform.position, Mathf.Max(0.08f, radius * 0.08f));

        Vector3 swayOffset = Vector3.right * swayAmplitude;
        Gizmos.color = new Color(1f, 0.9f, 0.5f, Mathf.Clamp01(lineAlpha * 0.65f));
        Gizmos.DrawLine(transform.position - swayOffset, transform.position + swayOffset);
    }
#endif
}
