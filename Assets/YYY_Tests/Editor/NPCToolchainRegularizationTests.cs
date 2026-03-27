using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class NPCToolchainRegularizationTests
{
    private const string ProductionProfilePath = "Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset";
    private const string BubbleReviewProfilePath = "Assets/111_Data/NPC/NPC_BubbleReviewProfile.asset";
    private const string VillageChiefProfilePath = "Assets/111_Data/NPC/NPC_001_VillageChiefRoamProfile.asset";
    private const string VillageDaughterProfilePath = "Assets/111_Data/NPC/NPC_002_VillageDaughterRoamProfile.asset";
    private const string ResearchReviewProfilePath = "Assets/111_Data/NPC/NPC_003_ResearchReviewProfile.asset";
    private const string VillageChiefContentPath = "Assets/111_Data/NPC/NPC_001_VillageChiefDialogueContent.asset";
    private const string VillageDaughterContentPath = "Assets/111_Data/NPC/NPC_002_VillageDaughterDialogueContent.asset";
    private const string ResearchDialogueContentPath = "Assets/111_Data/NPC/NPC_003_ResearchDialogueContent.asset";

    [Test]
    public void StressTalker_ShouldDefaultToManualStart_AndExposeExplicitModeConfiguration()
    {
        Type talkerType = ResolveTypeOrFail("NPCBubbleStressTalker");
        GameObject go = new GameObject("NPCBubbleStressTalker_Test");

        try
        {
            Component talker = go.AddComponent(talkerType);

            Assert.That((bool)GetFieldOrProperty(talker, "StartOnEnable"), Is.False);
            Assert.That((bool)GetFieldOrProperty(talker, "TestModeEnabled"), Is.False);
            Assert.That((bool)GetFieldOrProperty(talker, "DisableRoamWhileTesting"), Is.True);

            InvokeInstance(talker, "ConfigureMode", true, false);

            Assert.That((bool)GetFieldOrProperty(talker, "StartOnEnable"), Is.True);
            Assert.That((bool)GetFieldOrProperty(talker, "TestModeEnabled"), Is.True);
            Assert.That((bool)GetFieldOrProperty(talker, "DisableRoamWhileTesting"), Is.False);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void SceneIntegrationTool_ShouldResolvePerNpcRecommendedProfiles_ForProductionAndBubbleReview()
    {
        Type toolType = ResolveTypeOrFail("NPCSceneIntegrationTool");
        Type integrationModeType = ResolveNestedTypeOrFail(toolType, "IntegrationMode");
        object productionMode = Enum.Parse(integrationModeType, "Production");
        object bubbleReviewMode = Enum.Parse(integrationModeType, "BubbleReview");

        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "001", productionMode),
            Is.EqualTo(VillageChiefProfilePath));
        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "002", productionMode),
            Is.EqualTo(VillageDaughterProfilePath));
        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "003", productionMode),
            Is.EqualTo(ResearchReviewProfilePath));
        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "NPC_Unknown", productionMode),
            Is.EqualTo(ProductionProfilePath));

        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "003", bubbleReviewMode),
            Is.EqualTo(ResearchReviewProfilePath));
        Assert.That(
            (string)InvokeStatic(toolType, "ResolveRecommendedProfilePath", "001", bubbleReviewMode),
            Is.EqualTo(BubbleReviewProfilePath));
    }

    [Test]
    public void PrefabGeneratorTool_ShouldRequireExplicitBubbleReviewOptIn_AndKeep003OnResearchProfile()
    {
        Type toolType = ResolveTypeOrFail("NPCPrefabGeneratorTool");
        ScriptableObject tool = ScriptableObject.CreateInstance(toolType);

        try
        {
            Assert.That((bool)GetFieldOrProperty(tool, "autoAssignBubbleReviewRole"), Is.False);
            Assert.That((string)GetFieldOrProperty(tool, "bubbleReviewNpcNames"), Is.Empty);

            Type generatedNpcRoleType = ResolveNestedTypeOrFail(toolType, "GeneratedNpcRole");
            object productionRole = Enum.Parse(generatedNpcRoleType, "Production");
            object bubbleReviewRole = Enum.Parse(generatedNpcRoleType, "BubbleReview");

            Assert.That(
                (string)InvokeInstance(tool, "ResolveGeneratedProfilePath", "003", productionRole),
                Is.EqualTo(ResearchReviewProfilePath));
            Assert.That(
                (string)InvokeInstance(tool, "ResolveGeneratedProfilePath", "NPC_Test", bubbleReviewRole),
                Is.EqualTo(BubbleReviewProfilePath));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(tool);
        }
    }

    [Test]
    public void RoleProfiles_ShouldReferenceDedicatedDialogueContentAssets()
    {
        Type roamProfileType = ResolveTypeOrFail("NPCRoamProfile");
        UnityEngine.Object chiefProfile = AssetDatabase.LoadAssetAtPath(VillageChiefProfilePath, roamProfileType);
        UnityEngine.Object daughterProfile = AssetDatabase.LoadAssetAtPath(VillageDaughterProfilePath, roamProfileType);
        UnityEngine.Object researchProfile = AssetDatabase.LoadAssetAtPath(ResearchReviewProfilePath, roamProfileType);

        Assert.That(chiefProfile, Is.Not.Null);
        Assert.That(daughterProfile, Is.Not.Null);
        Assert.That(researchProfile, Is.Not.Null);

        Assert.That(
            AssetDatabase.GetAssetPath((UnityEngine.Object)GetFieldOrProperty(chiefProfile, "DialogueContentProfile")),
            Is.EqualTo(VillageChiefContentPath));
        Assert.That(
            AssetDatabase.GetAssetPath((UnityEngine.Object)GetFieldOrProperty(daughterProfile, "DialogueContentProfile")),
            Is.EqualTo(VillageDaughterContentPath));
        Assert.That(
            AssetDatabase.GetAssetPath((UnityEngine.Object)GetFieldOrProperty(researchProfile, "DialogueContentProfile")),
            Is.EqualTo(ResearchDialogueContentPath));

        Assert.That(((string[])GetFieldOrProperty(chiefProfile, "PlayerNearbyLines")).Length, Is.GreaterThan(0));
        Assert.That(((string[])GetFieldOrProperty(daughterProfile, "PlayerNearbyLines")).Length, Is.GreaterThan(0));
        Assert.That(((string[])GetFieldOrProperty(researchProfile, "PlayerNearbyLines")).Length, Is.GreaterThan(0));
    }

    [Test]
    public void DialogueContentProfiles_ShouldResolvePairSpecificAmbientChatLines()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        UnityEngine.Object chiefContent = AssetDatabase.LoadAssetAtPath(VillageChiefContentPath, dialogueContentType);
        UnityEngine.Object daughterContent = AssetDatabase.LoadAssetAtPath(VillageDaughterContentPath, dialogueContentType);
        UnityEngine.Object researchContent = AssetDatabase.LoadAssetAtPath(ResearchDialogueContentPath, dialogueContentType);

        Assert.That(chiefContent, Is.Not.Null);
        Assert.That(daughterContent, Is.Not.Null);
        Assert.That(researchContent, Is.Not.Null);

        string[] chiefToDaughter = (string[])InvokeInstance(chiefContent, "GetAmbientChatLines", "002", true);
        string[] daughterToChief = (string[])InvokeInstance(daughterContent, "GetAmbientChatLines", "001", false);
        string[] researchToChief = (string[])InvokeInstance(researchContent, "GetAmbientChatLines", "001", true);

        Assert.That(chiefToDaughter.Length, Is.GreaterThan(0));
        Assert.That(daughterToChief.Length, Is.GreaterThan(0));
        Assert.That(researchToChief.Length, Is.GreaterThan(0));

        Assert.That(chiefToDaughter[0], Is.Not.EqualTo(((string[])GetFieldOrProperty(chiefContent, "DefaultChatInitiatorLines"))[0]));
        Assert.That(daughterToChief[0], Is.Not.EqualTo(((string[])GetFieldOrProperty(daughterContent, "DefaultChatResponderLines"))[0]));
        Assert.That(researchToChief[0], Is.Not.EqualTo(((string[])GetFieldOrProperty(researchContent, "DefaultChatInitiatorLines"))[0]));
    }

    [Test]
    public void DialogueContentProfiles_ShouldExposeRelationshipStageSpecificNearbyLines()
    {
        Type dialogueContentType = ResolveTypeOrFail("NPCDialogueContentProfile");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");
        object strangerStage = Enum.Parse(relationshipStageType, "Stranger");
        object closeStage = Enum.Parse(relationshipStageType, "Close");

        UnityEngine.Object chiefContent = AssetDatabase.LoadAssetAtPath(VillageChiefContentPath, dialogueContentType);
        UnityEngine.Object daughterContent = AssetDatabase.LoadAssetAtPath(VillageDaughterContentPath, dialogueContentType);
        UnityEngine.Object researchContent = AssetDatabase.LoadAssetAtPath(ResearchDialogueContentPath, dialogueContentType);

        Assert.That(chiefContent, Is.Not.Null);
        Assert.That(daughterContent, Is.Not.Null);
        Assert.That(researchContent, Is.Not.Null);

        string[] chiefStranger = (string[])InvokeInstance(chiefContent, "GetPlayerNearbyLines", strangerStage);
        string[] chiefClose = (string[])InvokeInstance(chiefContent, "GetPlayerNearbyLines", closeStage);
        string[] daughterStranger = (string[])InvokeInstance(daughterContent, "GetPlayerNearbyLines", strangerStage);
        string[] daughterClose = (string[])InvokeInstance(daughterContent, "GetPlayerNearbyLines", closeStage);
        string[] researchStranger = (string[])InvokeInstance(researchContent, "GetPlayerNearbyLines", strangerStage);
        string[] researchClose = (string[])InvokeInstance(researchContent, "GetPlayerNearbyLines", closeStage);

        Assert.That(chiefStranger.Length, Is.GreaterThan(0));
        Assert.That(chiefClose.Length, Is.GreaterThan(0));
        Assert.That(daughterStranger.Length, Is.GreaterThan(0));
        Assert.That(daughterClose.Length, Is.GreaterThan(0));
        Assert.That(researchStranger.Length, Is.GreaterThan(0));
        Assert.That(researchClose.Length, Is.GreaterThan(0));

        Assert.That(chiefClose[0], Is.Not.EqualTo(chiefStranger[0]));
        Assert.That(daughterClose[0], Is.Not.EqualTo(daughterStranger[0]));
        Assert.That(researchClose[0], Is.Not.EqualTo(researchStranger[0]));
    }

    [Test]
    public void RelationshipService_ShouldPersistPerNpcStage()
    {
        Type serviceType = ResolveTypeOrFail("PlayerNpcRelationshipService");
        Type relationshipStageType = ResolveTypeOrFail("NPCRelationshipStage");
        object familiarStage = Enum.Parse(relationshipStageType, "Familiar");
        string key = (string)InvokeStatic(serviceType, "BuildPlayerPrefsKey", "001");

        InvokeStatic(serviceType, "ResetStage", "001", true);

        try
        {
            InvokeStatic(serviceType, "SetStage", "001", familiarStage, true);

            Assert.That(PlayerPrefs.GetInt(key, -1), Is.EqualTo(Convert.ToInt32(familiarStage)));
            Assert.That(InvokeStatic(serviceType, "GetStage", "001"), Is.EqualTo(familiarStage));
        }
        finally
        {
            InvokeStatic(serviceType, "ResetStage", "001", true);
        }
    }

    private static Type ResolveNestedTypeOrFail(Type parentType, string nestedTypeName)
    {
        Type nestedType = parentType.GetNestedType(nestedTypeName, BindingFlags.NonPublic);
        Assert.IsNotNull(nestedType, $"未找到嵌套类型：{parentType.Name}+{nestedTypeName}");
        return nestedType;
    }

    private static Type ResolveTypeOrFail(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type direct = assembly.GetType(typeName);
            if (direct != null)
            {
                return direct;
            }

            Type[] candidates;
            try
            {
                candidates = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                candidates = ex.Types;
            }

            foreach (Type candidate in candidates)
            {
                if (candidate != null && (candidate.FullName == typeName || candidate.Name == typeName))
                {
                    return candidate;
                }
            }
        }

        Assert.Fail($"未找到类型：{typeName}");
        return null;
    }

    private static object InvokeStatic(Type type, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, args);
        Assert.IsNotNull(method, $"未找到静态方法：{type.Name}.{methodName}");
        return method.Invoke(null, args);
    }

    private static object InvokeInstance(object target, string methodName, params object[] args)
    {
        MethodInfo method = ResolveMethodByArguments(target.GetType(), methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, args);
        Assert.IsNotNull(method, $"未找到实例方法：{target.GetType().Name}.{methodName}");
        return method.Invoke(target, args);
    }

    private static object GetFieldOrProperty(object target, string name)
    {
        Type type = target.GetType();

        FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            return field.GetValue(target);
        }

        PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null)
        {
            return property.GetValue(target);
        }

        Assert.Fail($"未找到字段或属性：{type.Name}.{name}");
        return null;
    }

    private static MethodInfo ResolveMethodByArguments(Type type, string methodName, BindingFlags bindingFlags, object[] args)
    {
        MethodInfo fallback = null;
        MethodInfo[] methods = type.GetMethods(bindingFlags);

        for (int methodIndex = 0; methodIndex < methods.Length; methodIndex++)
        {
            MethodInfo method = methods[methodIndex];
            if (method.Name != methodName)
            {
                continue;
            }

            fallback ??= method;

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            bool match = true;
            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }

                object arg = args[parameterIndex];
                if (arg == null)
                {
                    if (parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType) == null)
                    {
                        match = false;
                        break;
                    }

                    continue;
                }

                if (!parameterType.IsAssignableFrom(arg.GetType()))
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                return method;
            }
        }

        return fallback;
    }
}
