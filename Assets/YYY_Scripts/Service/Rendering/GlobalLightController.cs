using UnityEngine;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif

/// <summary>
/// 全局光源控制器（路线 A）
/// 封装 URP Global Light 2D 的控制逻辑，仅在路线 A 启用时激活。
/// 使用条件编译 #if USE_URP 隔离 URP 依赖。
/// 非 URP 环境下所有方法为空实现，确保编译通过。
/// </summary>
public class GlobalLightController : MonoBehaviour
{
#if USE_URP
    /// <summary>
    /// 缓存的 Light2D 组件引用
    /// </summary>
    private Light2D light2D;
#endif

    private void Awake()
    {
#if USE_URP
        // 尝试获取挂载在同一 GameObject 上的 Light2D 组件
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogWarning("[GlobalLightController] 未找到 Light2D 组件，请确保已添加 Global Light 2D");
        }
#endif
    }

    /// <summary>
    /// 设置全局光源颜色
    /// </summary>
    public void SetLightColor(Color color)
    {
#if USE_URP
        if (light2D != null)
        {
            light2D.color = color;
        }
#endif
    }

    /// <summary>
    /// 设置全局光源强度
    /// </summary>
    public void SetLightIntensity(float intensity)
    {
#if USE_URP
        if (light2D != null)
        {
            light2D.intensity = intensity;
        }
#endif
    }

    /// <summary>
    /// 设置启用/禁用状态
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }
}
