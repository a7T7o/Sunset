using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class NpcLocomotionFacadeSurfaceTests
{
    private static readonly string ProjectRoot = ResolveProjectRoot();

    private static readonly string NavigationLiveValidationRunnerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs");

    private static readonly string TraversalAcceptanceProbeMenuPath =
        Path.Combine(ProjectRoot, "Assets/Editor/NPC/CodexNpcTraversalAcceptanceProbeMenu.cs");

    [Test]
    public void NPCAutoRoamController_ShouldExposeMinimalFacadeSurface()
    {
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");

        Assert.That(GetMethod(roamType, "AcquireStoryControl").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "ReleaseStoryControl").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "RequestStageTravel").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "RequestReturnToAnchor").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "RequestReturnHome").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "SnapToTarget").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "BeginAutonomousTravel").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "BeginReturnHome").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "ResumeAutonomousRoam").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "AbortAndReplan").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "BindResidentHomeAnchor").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "SyncRuntimeProfileFromAsset").IsPublic, Is.True);
        Assert.That(GetMethod(roamType, "ApplyIdleFacing").IsPublic, Is.True);
    }

    [Test]
    public void NPCAutoRoamController_ShouldHideLowLevelResidentScriptedMethods()
    {
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");

        Assert.That(GetMethod(roamType, "AcquireResidentScriptedControl").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "ReleaseResidentScriptedControl").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "DriveResidentScriptedMoveTo").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "PauseResidentScriptedMovement").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "ResumeResidentScriptedMovement").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "HaltResidentScriptedMovement").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "SetHomeAnchor").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "ApplyProfile").IsPublic, Is.False);
        Assert.That(GetMethod(roamType, "RefreshRoamCenterFromCurrentContext").IsPublic, Is.False);
    }

    [Test]
    public void ContractMethods_ShouldCarryExplicitSurfaceScopeAnnotations()
    {
        Type roamType = ResolveTypeOrFail("NPCAutoRoamController");
        Type motionType = ResolveTypeOrFail("NPCMotionController");

        AssertSurfaceScope(roamType, "AcquireStoryControl", "ExternalFacade");
        AssertSurfaceScope(roamType, "RequestStageTravel", "ExternalFacade");
        AssertSurfaceScope(roamType, "SnapToTarget", "ExternalFacade");
        AssertSurfaceScope(roamType, "BeginAutonomousTravel", "ExternalFacade");
        AssertSurfaceScope(roamType, "BeginReturnHome", "ExternalFacade");
        AssertSurfaceScope(roamType, "ResumeAutonomousRoam", "ExternalFacade");
        AssertSurfaceScope(roamType, "AbortAndReplan", "ExternalFacade");
        AssertSurfaceScope(roamType, "StopRoam", "RuntimeOnly");
        AssertSurfaceScope(roamType, "DebugMoveTo", "DebugOnly");
        AssertSurfaceScope(motionType, "ApplyIdleFacing", "ExternalFacade");
        AssertSurfaceScope(motionType, "ApplyDirectedMotion", "ExternalFacade");
        AssertSurfaceScope(motionType, "SetFacingDirection", "RuntimeOnly");
        AssertSurfaceScope(motionType, "SetExternalVelocity", "RuntimeOnly");
        AssertSurfaceScope(motionType, "SetExternalFacingDirection", "RuntimeOnly");
    }

    [Test]
    public void NavigationLiveValidationRunner_ShouldUseFacadeContracts_ForValidationTravelAndRoamReset()
    {
        string runnerText = File.ReadAllText(NavigationLiveValidationRunnerPath);

        StringAssert.Contains("BeginAutonomousTravel(", runnerText, "导航 live validation 应走 navigation-facing directed travel facade。");
        StringAssert.Contains("ResumeAutonomousRoam(", runnerText, "导航 live validation 的 managed roam 续跑应走恢复 facade。");
        StringAssert.Contains("SnapToTarget(", runnerText, "导航 live validation 的瞬时摆位/停放应走显式 snap facade，而不是继续拼低级停 roam。");
        StringAssert.DoesNotContain(".DebugMoveTo(", runnerText, "导航 live validation 不应再用旧 DebugMoveTo 充当正式 travel contract。");
        StringAssert.DoesNotContain(".StartRoam(", runnerText, "导航 live validation 不应再直调底层 StartRoam。");
        StringAssert.DoesNotContain(".StopRoam(", runnerText, "导航 live validation 不应再直调低级 StopRoam 来拼 reset/parking。");
    }

    [Test]
    public void CodexNpcTraversalAcceptanceProbeMenu_ShouldUseFacadeContracts_ForProbeTravelAndSnapSetup()
    {
        string probeMenuText = File.ReadAllText(TraversalAcceptanceProbeMenuPath);

        StringAssert.Contains("BeginAutonomousTravel(", probeMenuText, "导航 acceptance probe 应走 navigation-facing directed travel facade。");
        StringAssert.Contains("SnapToTarget(", probeMenuText, "导航 acceptance probe 的 probe 摆位应走显式 snap facade。");
        StringAssert.DoesNotContain(".DebugMoveTo(", probeMenuText, "导航 acceptance probe 不应再用 DebugMoveTo 伪装正式 contract。");
        StringAssert.DoesNotContain(".StopRoam(", probeMenuText, "导航 acceptance probe 不应再先 StopRoam 再手工拼摆位。");
    }

    private static void AssertSurfaceScope(Type type, string methodName, string expectedScopeName)
    {
        MethodInfo method = GetMethod(type, methodName);
        object[] attributes = method.GetCustomAttributes(inherit: false);
        object attribute = null;
        for (int index = 0; index < attributes.Length; index++)
        {
            object candidate = attributes[index];
            if (candidate != null && string.Equals(candidate.GetType().Name, "NpcLocomotionSurfaceAttribute", StringComparison.Ordinal))
            {
                attribute = candidate;
                break;
            }
        }

        Assert.That(attribute, Is.Not.Null, $"方法缺少 surface 标记: {type.Name}.{methodName}");
        PropertyInfo scopeProperty = attribute.GetType().GetProperty(
            "Scope",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(scopeProperty, Is.Not.Null, $"surface 标记缺少 Scope 属性: {type.Name}.{methodName}");
        object scopeValue = scopeProperty.GetValue(attribute);
        Assert.That(scopeValue?.ToString(), Is.EqualTo(expectedScopeName), $"方法 surface scope 不符: {type.Name}.{methodName}");
    }

    private static MethodInfo GetMethod(Type type, string methodName)
    {
        MethodInfo method = type.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {type.Name}.{methodName}");
        return method;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Type type = assemblies[assemblyIndex].GetType(fullName, throwOnError: false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }

    private static string ResolveProjectRoot()
    {
        DirectoryInfo directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory != null)
        {
            bool hasAssets = Directory.Exists(Path.Combine(directory.FullName, "Assets"));
            bool hasProjectSettings = Directory.Exists(Path.Combine(directory.FullName, "ProjectSettings"));
            if (hasAssets && hasProjectSettings)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail($"未找到 Unity 项目根目录，TestDirectory={TestContext.CurrentContext.TestDirectory}");
        return TestContext.CurrentContext.TestDirectory;
    }
}
