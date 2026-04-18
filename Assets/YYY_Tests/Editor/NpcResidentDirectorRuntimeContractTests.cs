using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[TestFixture]
public class NpcResidentDirectorRuntimeContractTests
{
    private readonly List<Scene> _createdScenes = new List<Scene>();

    [TearDown]
    public void TearDown()
    {
        for (int index = _createdScenes.Count - 1; index >= 0; index--)
        {
            Scene scene = _createdScenes[index];
            if (!scene.IsValid() || !scene.isLoaded)
            {
                continue;
            }

            EditorSceneManager.CloseScene(scene, removeScene: true);
        }

        _createdScenes.Clear();
    }

    [Test]
    public void CaptureResidentRuntimeSnapshot_ShouldExposeStableKeyAnchorAndControlState()
    {
        Scene scene = CreateScene();
        Component controller = CreateResident(scene, "101", new Vector2(3.25f, -1.5f), new Vector2(4.5f, -2.25f), out Transform groupRoot, out Transform homeAnchor);

        InvokeInstance(controller, "StartRoam");
        InvokeInstance(controller, "AcquireResidentScriptedControl", "day1-director");

        object snapshot = InvokeInstance(controller, "CaptureResidentRuntimeSnapshot");

        Assert.That(snapshot, Is.Not.Null);
        Assert.That(GetFieldValue(snapshot, "stableKey") as string, Is.EqualTo("101"));
        Assert.That(GetFieldValue(snapshot, "sceneName") as string, Is.EqualTo(controller.gameObject.scene.name));
        Assert.That(GetFieldValue(snapshot, "residentGroupName") as string, Is.EqualTo(groupRoot.name));
        Assert.That(
            GetFieldValue(snapshot, "residentGroupHierarchyPath") as string,
            Is.EqualTo((string)InvokeStatic("NpcResidentRuntimeContract", "BuildHierarchyPath", groupRoot)));
        Assert.That(GetFieldValue(snapshot, "homeAnchorName") as string, Is.EqualTo(homeAnchor.name));
        Assert.That(
            GetFieldValue(snapshot, "homeAnchorHierarchyPath") as string,
            Is.EqualTo((string)InvokeStatic("NpcResidentRuntimeContract", "BuildHierarchyPath", homeAnchor)));
        Assert.That((bool)GetFieldValue(snapshot, "hasHomeAnchor"), Is.True);
        Assert.That((bool)GetFieldValue(snapshot, "homeAnchorSceneOwned"), Is.True);
        Assert.That((Vector2)GetFieldValue(snapshot, "residentPosition"), Is.EqualTo((Vector2)controller.transform.position));
        Assert.That((Vector2)GetFieldValue(snapshot, "homeAnchorPosition"), Is.EqualTo((Vector2)homeAnchor.position));
        Assert.That((bool)GetFieldValue(snapshot, "scriptedControlActive"), Is.True);
        Assert.That(GetFieldValue(snapshot, "scriptedControlOwnerKey") as string, Is.EqualTo("day1-director"));
        Assert.That((bool)GetFieldValue(snapshot, "resumeRoamWhenReleased"), Is.True);
    }

    [Test]
    public void ApplySnapshot_ShouldRestoreResidentPositionAnchorAndScriptedControl()
    {
        Scene scene = CreateScene();
        Component controller = CreateResident(scene, "202", new Vector2(1.4f, 2.2f), new Vector2(1.8f, 2.7f), out _, out Transform homeAnchor);

        InvokeInstance(controller, "StartRoam");
        InvokeInstance(controller, "AcquireResidentScriptedControl", "spring-day1");
        object snapshot = InvokeInstance(controller, "CaptureResidentRuntimeSnapshot");

        InvokeInstance(controller, "ClearResidentScriptedControl");
        controller.transform.position = new Vector2(-5f, -3f);
        homeAnchor.position = new Vector2(-6f, -2f);

        bool applied = (bool)InvokeStatic("NpcResidentRuntimeContract", "TryApplySnapshot", scene, snapshot, false);

        Assert.That(applied, Is.True);
        Assert.That((Vector2)controller.transform.position, Is.EqualTo((Vector2)GetFieldValue(snapshot, "residentPosition")));
        Assert.That((Vector2)homeAnchor.position, Is.EqualTo((Vector2)GetFieldValue(snapshot, "homeAnchorPosition")));
        Assert.That((bool)GetPropertyValue(controller, "IsResidentScriptedControlActive"), Is.True);
        Assert.That((string)GetPropertyValue(controller, "ResidentScriptedControlOwnerKey"), Is.EqualTo("spring-day1"));
    }

    [Test]
    public void CaptureSceneSnapshots_ShouldOnlyReturnResidentsFromRequestedScene()
    {
        Scene firstScene = CreateScene();
        Scene secondScene = CreateScene();

        CreateResident(firstScene, "301", new Vector2(0.5f, 0.75f), new Vector2(0.75f, 1.1f), out _, out _);
        CreateResident(secondScene, "104", new Vector2(-1.5f, 1.25f), new Vector2(-1.1f, 1.6f), out _, out _);

        IList firstSnapshots = InvokeStatic("NpcResidentRuntimeContract", "CaptureSceneSnapshots", firstScene) as IList;
        IList secondSnapshots = InvokeStatic("NpcResidentRuntimeContract", "CaptureSceneSnapshots", secondScene) as IList;

        Assert.That(firstSnapshots, Is.Not.Null);
        Assert.That(secondSnapshots, Is.Not.Null);
        Assert.That(firstSnapshots.Count, Is.EqualTo(1));
        Assert.That(secondSnapshots.Count, Is.EqualTo(1));
        Assert.That(GetFieldValue(firstSnapshots[0], "stableKey") as string, Is.EqualTo("301"));
        Assert.That(GetFieldValue(secondSnapshots[0], "stableKey") as string, Is.EqualTo("104"));
    }

    private Scene CreateScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        _createdScenes.Add(scene);
        return scene;
    }

    private static Component CreateResident(
        Scene scene,
        string npcName,
        Vector2 residentPosition,
        Vector2 homeAnchorPosition,
        out Transform groupRoot,
        out Transform homeAnchor)
    {
        GameObject root = new GameObject($"{npcName}_ResidentRoot");
        SceneManager.MoveGameObjectToScene(root, scene);
        groupRoot = root.transform;

        GameObject npc = new GameObject(npcName);
        npc.transform.SetParent(groupRoot, false);
        npc.transform.position = residentPosition;
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component controller = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        GameObject anchorObject = new GameObject($"{npcName}_HomeAnchor");
        anchorObject.transform.SetParent(groupRoot, false);
        anchorObject.transform.position = homeAnchorPosition;
        SceneManager.MoveGameObjectToScene(anchorObject, scene);
        homeAnchor = anchorObject.transform;

        InvokeInstance(controller, "SetHomeAnchor", homeAnchor);
        return controller;
    }

    private static Type ResolveTypeOrFail(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName, throwOnError: false);
            if (type != null)
            {
                return type;
            }
        }

        Assert.Fail($"未找到类型: {fullName}");
        return null;
    }

    private static object InvokeStatic(string typeName, string methodName, params object[] args)
    {
        Type targetType = ResolveTypeOrFail(typeName);
        MethodInfo method = targetType.GetMethod(
            methodName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {typeName}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object GetFieldValue(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        return field.GetValue(target);
    }

    private static object GetPropertyValue(object target, string propertyName)
    {
        PropertyInfo property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        return property.GetValue(target);
    }
}
