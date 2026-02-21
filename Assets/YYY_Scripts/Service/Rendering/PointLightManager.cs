using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if USE_URP
using UnityEngine.Rendering.Universal;
#endif

/// <summary>
/// 局部光源管理器（路线 A）
/// 管理场景中的夜间 Point Light 2D 光源。
/// 通过 NightLightMarker 组件识别夜间光源，使用协程实现淡入淡出动画。
/// 使用条件编译 #if USE_URP 隔离 URP 依赖。
/// </summary>
public class PointLightManager : MonoBehaviour
{
#if USE_URP
    /// <summary>
    /// 缓存的夜间光源列表（NightLightMarker + 对应的 Light2D）
    /// </summary>
    private List<NightLightEntry> nightLights = new List<NightLightEntry>();

    /// <summary>
    /// 夜间光源条目：关联 Marker 和 Light2D
    /// </summary>
    private struct NightLightEntry
    {
        public NightLightMarker marker;
        public Light2D light;
    }

    /// <summary>
    /// 当前是否处于激活状态
    /// </summary>
    private bool isActivated = false;

    /// <summary>
    /// 当前正在运行的淡入淡出协程
    /// </summary>
    private Coroutine fadeCoroutine;
#endif

    /// <summary>
    /// 渐变激活夜间光源
    /// </summary>
    public void ActivateNightLights(float fadeDuration)
    {
#if USE_URP
        if (isActivated) return;
        isActivated = true;

        // 停止正在进行的淡入淡出
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeLights(true, fadeDuration));
#endif
    }

    /// <summary>
    /// 渐变关闭夜间光源
    /// </summary>
    public void DeactivateNightLights(float fadeDuration)
    {
#if USE_URP
        if (!isActivated) return;
        isActivated = false;

        // 停止正在进行的淡入淡出
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeLights(false, fadeDuration));
#endif
    }

    /// <summary>
    /// 重新扫描场景中带 NightLightMarker 组件的物体，刷新光源列表
    /// </summary>
    public void RefreshLightList()
    {
#if USE_URP
        nightLights.Clear();

        // 使用 Unity 6 API 查找所有 NightLightMarker 组件
        NightLightMarker[] markers = FindObjectsByType<NightLightMarker>(FindObjectsSortMode.None);

        foreach (var marker in markers)
        {
            Light2D light = marker.GetComponent<Light2D>();
            if (light != null)
            {
                nightLights.Add(new NightLightEntry
                {
                    marker = marker,
                    light = light
                });

                // 应用 Marker 中配置的参数到 Light2D
                light.color = marker.LightColor;
                light.pointLightOuterRadius = marker.Radius;
            }
            else
            {
                Debug.LogWarning($"[PointLightManager] {marker.gameObject.name} 有 NightLightMarker 但缺少 Light2D 组件");
            }
        }

        Debug.Log($"<color=cyan>[PointLightManager] 扫描完成，找到 {nightLights.Count} 个夜间光源</color>");
#endif
    }

#if USE_URP
    /// <summary>
    /// 淡入/淡出协程：渐变控制所有夜间光源的强度
    /// </summary>
    /// <param name="fadeIn">true=淡入（激活），false=淡出（关闭）</param>
    /// <param name="duration">淡入淡出时长（秒）</param>
    private IEnumerator FadeLights(bool fadeIn, float duration)
    {
        if (nightLights.Count == 0)
        {
            fadeCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 淡入：0→maxIntensity，淡出：当前→0
            float progress = fadeIn ? t : (1f - t);

            foreach (var entry in nightLights)
            {
                if (entry.light != null && entry.marker != null)
                {
                    entry.light.intensity = entry.marker.MaxIntensity * progress;
                }
            }

            yield return null;
        }

        // 确保最终值精确
        foreach (var entry in nightLights)
        {
            if (entry.light != null && entry.marker != null)
            {
                entry.light.intensity = fadeIn ? entry.marker.MaxIntensity : 0f;
            }
        }

        fadeCoroutine = null;
    }
#endif

    /// <summary>
    /// 当前夜间光源是否处于激活状态
    /// </summary>
    public bool IsActivated
    {
        get
        {
#if USE_URP
            return isActivated;
#else
            return false;
#endif
        }
    }
}
