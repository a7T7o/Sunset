using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UObject = UnityEngine.Object;

[TestFixture]
public class NpcInteractionPriorityPolicyTests
{
    private static readonly string[] PhaseNames =
    {
        "None",
        "CrashAndMeet",
        "EnterVillage",
        "HealingAndHP",
        "WorkbenchFlashback",
        "FarmingTutorial",
        "DinnerConflict",
        "ReturnAndReminder",
        "FreeTime",
        "DayEnd"
    };

    private static readonly bool[] ExpectedSuppression =
    {
        false,
        true,
        true,
        true,
        true,
        true,
        true,
        true,
        false,
        false
    };

    private GameObject storyObject;
    private GameObject playerObject;
    private GameObject npcObject;
    private GameObject dialogueObject;

    [TearDown]
    public void TearDown()
    {
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        InvokeStatic<object>(policyType, "SetEditorValidationPhaseOverride", (object)null);
        SetPrivateStaticField(ResolveTypeOrFail("Sunset.Story.StoryManager"), "_instance", null);
        SetStaticProperty(ResolveTypeOrFail("Sunset.Story.DialogueManager"), "Instance", null);

        if (npcObject != null)
        {
            UObject.DestroyImmediate(npcObject);
        }

        if (playerObject != null)
        {
            UObject.DestroyImmediate(playerObject);
        }

        if (storyObject != null)
        {
            UObject.DestroyImmediate(storyObject);
        }

        if (dialogueObject != null)
        {
            UObject.DestroyImmediate(dialogueObject);
        }
    }

    public static IEnumerable<TestCaseData> StoryPhaseSuppressionCases()
    {
        for (int index = 0; index < PhaseNames.Length; index++)
        {
            yield return new TestCaseData(PhaseNames[index], ExpectedSuppression[index]);
        }
    }

    [TestCaseSource(nameof(StoryPhaseSuppressionCases))]
    public void StoryPhase_ShouldDecideWhetherFormalSuppressesCasualAndAmbient(string phaseName, bool shouldSuppress)
    {
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        object phase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), phaseName);

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressInformalInteraction", phase),
            Is.EqualTo(shouldSuppress),
            $"闲聊阶段门禁判断异常：{phaseName}");

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressAmbientBubble", phase),
            Is.EqualTo(shouldSuppress),
            $"环境气泡阶段门禁判断异常：{phaseName}");
    }

    [Test]
    public void InformalChatInteractable_ShouldRemainAvailableDuringFormalPriorityPhase_WhenSameNpcHasNoFormalDialogue()
    {
        CreateStoryManager("CrashAndMeet");
        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void InformalChatInteractable_ShouldYieldOnlyWhenSameNpcFormalDialogueCanTakeOver()
    {
        Component storyManager = CreateStoryManager("CrashAndMeet");
        CreateDialogueManager();
        Component interactable = CreateInformalInteractable(withFormalDialogueCandidate: true);
        object context = CreateContext();

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.False);

        InvokeInstance<object>(
            storyManager,
            "ResetState",
            ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FreeTime"),
            false);

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void InformalChatInteractable_ShouldKeepCurrentConversationAliveEvenInFormalPhase()
    {
        CreateStoryManager("CrashAndMeet");
        Component interactable = CreateInformalInteractable();
        object context = CreateContext();

        Component sessionService = playerObject.AddComponent(ResolveTypeOrFail("PlayerNpcChatSessionService"));
        SetPrivateField(sessionService, "_activeInteractable", interactable);
        SetPrivateField(sessionService, "_state", ParseNestedEnum(sessionService.GetType(), "SessionState", "PlayerTyping"));

        Assert.That(InvokeInstance<bool>(interactable, "CanInteract", context), Is.True);
    }

    [Test]
    public void EditorValidationPhaseOverride_ShouldAllowAmbientChecksToProbePairBubbleOutsideFormalPhase()
    {
        CreateStoryManager("CrashAndMeet");
        Type policyType = ResolveTypeOrFail("Sunset.Story.NpcInteractionPriorityPolicy");
        object freeTimePhase = ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), "FreeTime");

        InvokeStatic<object>(policyType, "SetEditorValidationPhaseOverride", freeTimePhase);

        Assert.That(
            InvokeStatic<bool>(policyType, "ShouldSuppressAmbientBubbleForCurrentStory"),
            Is.False,
            "编辑器 probe 临时覆写到 FreeTime 后，不应继续把 pair ambient 当成 formal 期压掉。");
    }

    private Component CreateStoryManager(string phaseName)
    {
        storyObject = new GameObject("StoryManagerTestHost");
        Component storyManager = storyObject.AddComponent(ResolveTypeOrFail("Sunset.Story.StoryManager"));
        SetPrivateStaticField(storyManager.GetType(), "_instance", storyManager);
        InvokeInstance<object>(
            storyManager,
            "ResetState",
            ParseEnum(ResolveTypeOrFail("Sunset.Story.StoryPhase"), phaseName),
            false);
        return storyManager;
    }

    private Component CreateInformalInteractable(bool withFormalDialogueCandidate = false)
    {
        npcObject = new GameObject("InformalNpc");

        Component roamController = npcObject.AddComponent(ResolveTypeOrFail("NPCAutoRoamController"));
        Component interactable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCInformalChatInteractable"));

        ScriptableObject roamProfile = ScriptableObject.CreateInstance(ResolveTypeOrFail("NPCRoamProfile"));
        Type contentProfileType = ResolveTypeOrFail("NPCDialogueContentProfile");
        ScriptableObject contentProfile = ScriptableObject.CreateInstance(contentProfileType);

        Type exchangeType = ResolveNestedTypeOrFail(contentProfileType, "InformalChatExchange");
        object exchange = Activator.CreateInstance(exchangeType);
        SetPrivateField(exchange, "playerLines", new[] { "玩家句" });
        SetPrivateField(exchange, "npcReplyLines", new[] { "NPC句" });

        Type bundleType = ResolveNestedTypeOrFail(contentProfileType, "InformalConversationBundle");
        object bundle = Activator.CreateInstance(bundleType);
        Array exchangeArray = Array.CreateInstance(exchangeType, 1);
        exchangeArray.SetValue(exchange, 0);
        SetPrivateField(bundle, "exchanges", exchangeArray);

        Array bundleArray = Array.CreateInstance(bundleType, 1);
        bundleArray.SetValue(bundle, 0);
        SetPrivateField(contentProfile, "defaultInformalConversationBundles", bundleArray);
        SetPrivateField(roamProfile, "dialogueContentProfile", contentProfile);
        SetPrivateField(roamController, "roamProfile", roamProfile);

        if (withFormalDialogueCandidate)
        {
            Component dialogueInteractable = npcObject.AddComponent(ResolveTypeOrFail("Sunset.Story.NPCDialogueInteractable"));
            ScriptableObject sequence = CreateDialogueSequence();
            SetPrivateField(dialogueInteractable, "initialSequence", sequence);
            SetPrivateField(interactable, "dialogueInteractable", dialogueInteractable);
        }

        return interactable;
    }

    private object CreateContext()
    {
        playerObject = new GameObject("PlayerContext");

        object context = Activator.CreateInstance(ResolveProjectTypeOrFail("InteractionContext"));
        SetPublicProperty(context, "PlayerTransform", playerObject.transform);
        SetPublicProperty(context, "PlayerPosition", (Vector2)playerObject.transform.position);
        return context;
    }

    private Component CreateDialogueManager()
    {
        dialogueObject = new GameObject("DialogueManagerTestHost");
        Component dialogueManager = dialogueObject.AddComponent(ResolveTypeOrFail("Sunset.Story.DialogueManager"));
        SetStaticProperty(dialogueManager.GetType(), "Instance", dialogueManager);
        return dialogueManager;
    }

    private static ScriptableObject CreateDialogueSequence()
    {
        Type sequenceType = ResolveTypeOrFail("Sunset.Story.DialogueSequenceSO");
        Type nodeType = ResolveTypeOrFail("Sunset.Story.DialogueNode");

        ScriptableObject sequence = ScriptableObject.CreateInstance(sequenceType);
        SetPublicField(sequence, "sequenceId", "test-formal-sequence");

        object node = Activator.CreateInstance(nodeType);
        SetPublicField(node, "speakerName", "村长");
        SetPublicField(node, "text", "正式对白");

        IList nodeList = GetPublicField<IList>(sequence, "nodes");
        nodeList.Add(node);
        return sequence;
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到类型: {typeName}");
        return null;
    }

    private static Type ResolveProjectTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type resolvedType = TryResolveProjectType(assembly, typeName);
            if (resolvedType != null)
            {
                return resolvedType;
            }
        }

        Assert.Fail($"未找到项目类型: {typeName}");
        return null;
    }

    private static Type TryResolveType(Assembly assembly, string typeName)
    {
        Type direct = assembly.GetType(typeName, false);
        if (direct != null)
        {
            return direct;
        }

        try
        {
            return assembly
                .GetTypes()
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null)
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private static Type TryResolveProjectType(Assembly assembly, string typeName)
    {
        try
        {
            return assembly
                .GetTypes()
                .Where(type => type != null &&
                               !string.Equals(type.Namespace, "UnityEditor", StringComparison.Ordinal))
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types
                .Where(type => type != null &&
                               !string.Equals(type.Namespace, "UnityEditor", StringComparison.Ordinal))
                .FirstOrDefault(type => string.Equals(type.FullName, typeName, StringComparison.Ordinal) ||
                                        string.Equals(type.Name, typeName, StringComparison.Ordinal));
        }
    }

    private static Type ResolveNestedTypeOrFail(Type parentType, string nestedTypeName)
    {
        Type nestedType = parentType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {parentType.Name}.{nestedTypeName}");
        return nestedType;
    }

    private static object ParseEnum(Type enumType, string value)
    {
        Assert.That(enumType, Is.Not.Null, $"未找到枚举类型: {value}");
        return Enum.Parse(enumType, value);
    }

    private static object ParseNestedEnum(Type targetType, string nestedTypeName, string value)
    {
        Type nestedType = targetType.GetNestedType(nestedTypeName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(nestedType, Is.Not.Null, $"未找到嵌套类型: {targetType.Name}.{nestedTypeName}");
        return Enum.Parse(nestedType, value);
    }

    private static T InvokeStatic<T>(Type targetType, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(
            targetType,
            methodName,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            args);
        Assert.That(method, Is.Not.Null, $"未找到静态方法: {targetType.Name}.{methodName}");
        return (T)method.Invoke(null, args);
    }

    private static T InvokeInstance<T>(object target, string methodName, params object[] args)
    {
        MethodInfo method = FindMethod(
            target.GetType(),
            methodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            args);
        Assert.That(method, Is.Not.Null, $"未找到方法: {target.GetType().Name}.{methodName}");
        return (T)method.Invoke(target, args);
    }

    private static MethodInfo FindMethod(Type targetType, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo[] candidates = targetType
            .GetMethods(bindingFlags)
            .Where(method => string.Equals(method.Name, methodName, StringComparison.Ordinal))
            .ToArray();

        for (int methodIndex = 0; methodIndex < candidates.Length; methodIndex++)
        {
            MethodInfo method = candidates[methodIndex];
            ParameterInfo[] parameters = method.GetParameters();
            if (!ParametersMatch(parameters, args))
            {
                continue;
            }

            return method;
        }

        return null;
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object[] args)
    {
        object[] effectiveArgs = args ?? Array.Empty<object>();
        if (parameters.Length != effectiveArgs.Length)
        {
            return false;
        }

        for (int index = 0; index < parameters.Length; index++)
        {
            object argument = effectiveArgs[index];
            Type parameterType = parameters[index].ParameterType;
            if (argument == null)
            {
                if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                {
                    return false;
                }

                continue;
            }

            if (!parameterType.IsInstanceOfType(argument))
            {
                return false;
            }
        }

        return true;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetPrivateStaticField(Type targetType, string fieldName, object value)
    {
        FieldInfo field = targetType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"未找到静态字段: {targetType.Name}.{fieldName}");
        field.SetValue(null, value);
    }

    private static T GetPublicField<T>(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到公开字段: {target.GetType().Name}.{fieldName}");
        return (T)field.GetValue(target);
    }

    private static void SetPublicField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"未找到公开字段: {target.GetType().Name}.{fieldName}");
        field.SetValue(target, value);
    }

    private static void SetPublicProperty(object target, string propertyName, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到属性: {target.GetType().Name}.{propertyName}");
        property.SetValue(target, value);
    }

    private static void SetStaticProperty(Type targetType, string propertyName, object value)
    {
        PropertyInfo property = targetType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"未找到静态属性: {targetType.Name}.{propertyName}");
        property.SetValue(null, value);
    }
}
