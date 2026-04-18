using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class ScenePartialSyncToolTests
{
    private const string ToolTypeName = "Sunset.EditorTools.SceneSync.ScenePartialSyncTool";

    private static Type ResolveToolType()
    {
        Type direct = Type.GetType($"{ToolTypeName}, Assembly-CSharp-Editor");
        if (direct != null)
        {
            return direct;
        }

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetType(ToolTypeName, false))
            .FirstOrDefault(type => type != null);
    }

    private static T InvokeToolMethod<T>(string methodName, params object[] args)
    {
        Type toolType = ResolveToolType();
        Assert.That(toolType, Is.Not.Null, $"未找到类型：{ToolTypeName}");

        MethodInfo method = toolType.GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Static);
        Assert.That(method, Is.Not.Null, $"未找到静态方法：{methodName}");

        object result = method.Invoke(null, args);
        return (T)result;
    }

    [Test]
    public void NormalizeSelectedPaths_ShouldRemoveDescendants_WhenAncestorAlreadySelected()
    {
        List<string> normalized = InvokeToolMethod<List<string>>("NormalizeSelectedPaths", new[]
        {
            "RootA/ChildA",
            "RootA",
            "RootB/ChildB",
            "RootB/ChildB/Leaf"
        });

        CollectionAssert.AreEqual(
            new[]
            {
                "RootA",
                "RootB/ChildB"
            },
            normalized);
    }

    [Test]
    public void FindMissingParentBlockers_ShouldReportNestedPath_WhenParentMissingInTarget()
    {
        List<string> blockers = InvokeToolMethod<List<string>>(
            "FindMissingParentBlockers",
            new[]
            {
                "RootA/ChildA",
                "RootB"
            },
            new[]
            {
                "RootB"
            });

        CollectionAssert.AreEqual(new[] { "RootA/ChildA" }, blockers);
    }

    [Test]
    public void FindMissingParentBlockers_ShouldAllowRootPath_AndExistingParentPath()
    {
        List<string> blockers = InvokeToolMethod<List<string>>(
            "FindMissingParentBlockers",
            new[]
            {
                "RootA",
                "RootB/ChildB"
            },
            new[]
            {
                "RootB"
            });

        Assert.That(blockers, Is.Empty);
    }

    [Test]
    public void GetParentPath_ShouldReturnExpectedParent()
    {
        Assert.That(
            InvokeToolMethod<string>("GetParentPath", "RootA/ChildA/Leaf"),
            Is.EqualTo("RootA/ChildA"));
        Assert.That(
            InvokeToolMethod<string>("GetParentPath", "RootA"),
            Is.EqualTo(string.Empty));
        Assert.That(
            InvokeToolMethod<string>("GetParentPath", string.Empty),
            Is.EqualTo(string.Empty));
    }
}
