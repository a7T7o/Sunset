using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class CameraDeadZoneSyncViewportGuardsTests
{
    [Test]
    public void ShouldApplyWideScreenViewportClamp_WindowedModeWithoutOptIn_ReturnsFalse()
    {
        bool shouldClamp = InvokeShouldApplyWideScreenViewportClamp(
            clampViewportOnWideScreens: true,
            clampViewportInWindowedMode: false,
            fullScreenMode: FullScreenMode.Windowed);

        Assert.That(shouldClamp, Is.False, "窗口模式默认不应继续裁 Camera.rect；否则自定义窗口仍会把蓝边/露底风险带回来。");
    }

    [Test]
    public void ShouldApplyWideScreenViewportClamp_WindowedModeWithOptIn_ReturnsTrue()
    {
        bool shouldClamp = InvokeShouldApplyWideScreenViewportClamp(
            clampViewportOnWideScreens: true,
            clampViewportInWindowedMode: true,
            fullScreenMode: FullScreenMode.Windowed);

        Assert.That(shouldClamp, Is.True, "如果后续确认必须在窗口模式保留旧策略，应能通过单一开关恢复原有 viewport clamp。");
    }

    [Test]
    public void ShouldApplyWideScreenViewportClamp_FullScreenModeStillReturnsTrue()
    {
        bool shouldClamp = InvokeShouldApplyWideScreenViewportClamp(
            clampViewportOnWideScreens: true,
            clampViewportInWindowedMode: false,
            fullScreenMode: FullScreenMode.FullScreenWindow);

        Assert.That(shouldClamp, Is.True, "这刀只收窗口模式；全屏路径仍应保留原有宽屏保护，避免把旧 confiner 事故重新放回来。");
    }

    private static bool InvokeShouldApplyWideScreenViewportClamp(
        bool clampViewportOnWideScreens,
        bool clampViewportInWindowedMode,
        FullScreenMode fullScreenMode)
    {
        MethodInfo method = ResolveCameraDeadZoneSyncType().GetMethod(
            "ShouldApplyWideScreenViewportClamp",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.That(method, Is.Not.Null, "守卫测试需要拿到 CameraDeadZoneSync 的显示模式判定入口。");

        object result = method.Invoke(null, new object[]
        {
            clampViewportOnWideScreens,
            clampViewportInWindowedMode,
            fullScreenMode
        });

        return (bool)result;
    }

    private static System.Type ResolveCameraDeadZoneSyncType()
    {
        System.Type type = System.Type.GetType("Sunset.Service.Camera.CameraDeadZoneSync, Assembly-CSharp");
        if (type != null)
        {
            return type;
        }

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType("Sunset.Service.Camera.CameraDeadZoneSync");
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail("无法解析 CameraDeadZoneSync 类型，守卫测试不能继续。");
        return null;
    }
}
