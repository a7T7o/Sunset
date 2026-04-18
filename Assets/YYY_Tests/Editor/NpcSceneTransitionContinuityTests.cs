using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[TestFixture]
public class NpcSceneTransitionContinuityTests
{
    private Scene _scene;

    [SetUp]
    public void SetUp()
    {
        _scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    [TearDown]
    public void TearDown()
    {
        if (_scene.IsValid() && _scene.isLoaded)
        {
            if (SceneManager.GetActiveScene().handle == _scene.handle)
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
            else
            {
                EditorSceneManager.CloseScene(_scene, removeScene: true);
            }
        }

        Type bridgeType = ResolveTypeOrFail("PersistentPlayerSceneBridge");
        TrySetStaticField(bridgeType, "s_instance", null);
    }

    [Test]
    public void CaptureResidentRuntimeSnapshot_ShouldRecordRoamState()
    {
        Component controller = CreateResident("101", new Vector2(1.25f, -0.5f), new Vector2(2f, -0.5f));
        InvokeInstance(controller, "StartRoam");

        object snapshot = InvokeInstance(controller, "CaptureResidentRuntimeSnapshot");

        Assert.That(snapshot, Is.Not.Null);
        Assert.That((bool)GetFieldValue(snapshot, "wasRoaming"), Is.True);
    }

    [Test]
    public void ApplyResidentRuntimeSnapshot_WithResumeResidentLogic_ShouldResumeRoamFromRestoredPosition()
    {
        Component controller = CreateResident("102", new Vector2(3.5f, 1.75f), new Vector2(4f, 2f));
        InvokeInstance(controller, "StartRoam");
        object snapshot = InvokeInstance(controller, "CaptureResidentRuntimeSnapshot");

        InvokeInstance(controller, "StopRoam");
        controller.transform.position = new Vector2(-5f, -3f);

        InvokeInstance(controller, "ApplyResidentRuntimeSnapshot", snapshot, true);

        Assert.That((Vector2)controller.transform.position, Is.EqualTo((Vector2)GetFieldValue(snapshot, "residentPosition")));
        Assert.That((bool)GetPropertyValue(controller, "IsRoaming"), Is.True);
    }

    [Test]
    public void PersistentPlayerSceneBridge_ClearNativeResidentRuntimeSnapshots_ShouldDropDay1SyntheticActors()
    {
        Type bridgeType = ResolveTypeOrFail("PersistentPlayerSceneBridge");
        GameObject bridgeObject = new GameObject("PersistentBridge_Test");
        SceneManager.MoveGameObjectToScene(bridgeObject, _scene);
        Component bridge = bridgeObject.AddComponent(bridgeType);
        SetStaticField(bridgeType, "s_instance", bridge);

        IDictionary snapshotsByScene = (IDictionary)GetFieldValue(bridge, "nativeResidentSnapshotsByScene");
        Type snapshotType = ResolveTypeOrFail("NpcResidentRuntimeSnapshot");
        IList snapshots = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(snapshotType));
        snapshots.Add(CreateResidentRuntimeSnapshot(snapshotType, "001"));
        snapshots.Add(CreateResidentRuntimeSnapshot(snapshotType, "NPC002"));
        snapshots.Add(CreateResidentRuntimeSnapshot(snapshotType, "003"));
        snapshots.Add(CreateResidentRuntimeSnapshot(snapshotType, "203"));
        snapshotsByScene["Town"] = snapshots;

        InvokeStatic(bridgeType, "ClearNativeResidentRuntimeSnapshots", (object)new string[] { "001", "002", "003" });

        Assert.That(snapshotsByScene.Contains("Town"), Is.True);
        Assert.That(snapshots, Has.Count.EqualTo(1));
        Assert.That(GetFieldValue(snapshots[0], "stableKey"), Is.EqualTo("203"),
            "Day1 收束时应只清 001/002/003 的 native bridge 快照，不能误删普通 native resident 快照。");
    }

    private Component CreateResident(string npcName, Vector2 residentPosition, Vector2 homeAnchorPosition)
    {
        GameObject root = new GameObject($"{npcName}_ResidentRoot");
        SceneManager.MoveGameObjectToScene(root, _scene);

        GameObject npc = new GameObject(npcName);
        npc.transform.SetParent(root.transform, false);
        npc.transform.position = residentPosition;
        npc.AddComponent(ResolveTypeOrFail("NPCMotionController"));
        Component controller = npc.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));

        GameObject anchorObject = new GameObject($"{npcName}_HomeAnchor");
        anchorObject.transform.SetParent(root.transform, false);
        anchorObject.transform.position = homeAnchorPosition;
        InvokeInstance(controller, "SetHomeAnchor", anchorObject.transform);

        return controller;
    }

    private static object CreateResidentRuntimeSnapshot(Type snapshotType, string stableKey)
    {
        object snapshot = Activator.CreateInstance(snapshotType);
        FieldInfo stableKeyField = snapshotType.GetField(
            "stableKey",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(stableKeyField, Is.Not.Null, "NpcResidentRuntimeSnapshot 应包含 stableKey 字段。");
        stableKeyField.SetValue(snapshot, stableKey);
        return snapshot;
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Type[] types;
            try
            {
                types = assemblies[assemblyIndex].GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                types = exception.Types;
            }

            if (types == null)
            {
                continue;
            }

            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type candidate = types[typeIndex];
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.FullName, typeName, StringComparison.Ordinal)
                    || string.Equals(candidate.Name, typeName, StringComparison.Ordinal))
                {
                    return candidate;
                }
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = target.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = type.GetMethod(
            methodName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static void SetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(
            fieldName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {type.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static void TrySetStaticField(Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(
            fieldName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(null, value);
        }
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
