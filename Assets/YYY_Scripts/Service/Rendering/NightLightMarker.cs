using UnityEngine;

/// <summary>
/// 夜间光源标记组件（路线 A 辅助）
/// 挂载在需要夜间亮灯的物体上，供 PointLightManager 识别和读取参数。
/// 配合 URP Point Light 2D 使用，定义光源的最大强度、颜色和半径。
/// </summary>
public class NightLightMarker : MonoBehaviour
{
    [Header("━━━━ 光源参数 ━━━━")]

    [Tooltip("光源最大强度（夜间完全激活时的强度）")]
    [SerializeField] private float maxIntensity = 1.0f;

    [Tooltip("光源颜色")]
    [SerializeField] private Color lightColor = Color.yellow;

    [Tooltip("光源照射半径")]
    [SerializeField] private float radius = 3f;

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
}
