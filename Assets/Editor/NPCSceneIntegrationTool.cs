using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class NPCSceneIntegrationTool : EditorWindow
{
    private const string ProductionProfilePath = "Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset";
    private const string BubbleReviewProfilePath = "Assets/111_Data/NPC/NPC_BubbleReviewProfile.asset";

    private enum IntegrationMode
    {
        Production = 0,
        BubbleReview = 1
    }

    private IntegrationMode mode = IntegrationMode.Production;
    private NPCRoamProfile assignedProfile;
    private bool createHomeAnchorIfMissing = true;
    private bool snapAnchorToNpc = true;
    private bool selectAnchorAfterCreate = false;
    private bool removeStressTalkerInProduction = true;
    private bool addStressTalkerInBubbleReview = true;

    [MenuItem("Tools/NPC/NPC Scene Integration Tool")]
    private static void ShowWindow()
    {
        NPCSceneIntegrationTool window = GetWindow<NPCSceneIntegrationTool>("NPC Scene Integration");
        window.minSize = new Vector2(520f, 520f);
        window.Show();
    }

    private void OnEnable()
    {
        if (assignedProfile == null)
        {
            assignedProfile = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(ProductionProfilePath);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("NPC Scene Integration Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Use this tool on selected scene NPCs to batch create home anchors, assign roam profiles, and switch between production mode and bubble review mode.",
            MessageType.Info);

        mode = (IntegrationMode)EditorGUILayout.EnumPopup("Integration Mode", mode);
        DrawModeHint();
        assignedProfile = (NPCRoamProfile)EditorGUILayout.ObjectField("Assigned Profile", assignedProfile, typeof(NPCRoamProfile), false);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Use Default Production Profile"))
        {
            assignedProfile = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(ProductionProfilePath);
        }

        if (GUILayout.Button("Use Bubble Review Profile"))
        {
            assignedProfile = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(BubbleReviewProfilePath);
        }

        if (GUILayout.Button("Use Recommended Profile"))
        {
            assignedProfile = LoadRecommendedProfile(mode);
        }
        EditorGUILayout.EndHorizontal();
        createHomeAnchorIfMissing = EditorGUILayout.Toggle("Create Home Anchor If Missing", createHomeAnchorIfMissing);
        snapAnchorToNpc = EditorGUILayout.Toggle("Snap Anchor To NPC", snapAnchorToNpc);
        selectAnchorAfterCreate = EditorGUILayout.Toggle("Select Anchor After Create", selectAnchorAfterCreate);
        removeStressTalkerInProduction = EditorGUILayout.Toggle("Remove Stress Talker In Production", removeStressTalkerInProduction);
        addStressTalkerInBubbleReview = EditorGUILayout.Toggle("Add Stress Talker In Bubble Review", addStressTalkerInBubbleReview);

        EditorGUILayout.Space(10f);
        DrawSelectionSummary();
        EditorGUILayout.Space(10f);

        using (new EditorGUI.DisabledScope(GetSelectedControllers().Count == 0))
        {
            if (GUILayout.Button("Apply To Selected NPCs", GUILayout.Height(36f)))
            {
                ApplyToSelected();
            }
        }
    }

    private void DrawSelectionSummary()
    {
        List<NPCAutoRoamController> controllers = GetSelectedControllers();
        EditorGUILayout.LabelField("Selected Scene NPCs", EditorStyles.boldLabel);

        if (controllers.Count == 0)
        {
            EditorGUILayout.HelpBox("Select one or more scene NPCs that already have NPCAutoRoamController.", MessageType.None);
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField($"Count: {controllers.Count}");
        for (int i = 0; i < controllers.Count; i++)
        {
            NPCAutoRoamController controller = controllers[i];
            NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();
            string profileName = controller.RoamProfile != null ? controller.RoamProfile.name : "None";
            string anchorState = controller.HomeAnchor != null ? controller.HomeAnchor.name : "No Anchor";
            string stressState = stressTalker != null ? "StressTalker" : "No StressTalker";
            EditorGUILayout.LabelField($"- {controller.name} | {profileName} | {anchorState} | {stressState}", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawModeHint()
    {
        string modeDescription = mode == IntegrationMode.Production
            ? "Production mode keeps normal roam behavior, uses the default production profile, and removes BubbleStressTalker from formal NPCs."
            : "Bubble Review mode keeps 003-style validation behavior, uses the BubbleReview profile, and ensures BubbleStressTalker is present.";

        EditorGUILayout.HelpBox(modeDescription, MessageType.None);
    }

    private void ApplyToSelected()
    {
        List<NPCAutoRoamController> controllers = GetSelectedControllers();
        if (controllers.Count == 0)
        {
            return;
        }

        if (assignedProfile == null)
        {
            assignedProfile = LoadRecommendedProfile(mode);
        }

        List<GameObject> createdAnchors = new List<GameObject>();
        HashSet<UnityEngine.SceneManagement.Scene> touchedScenes = new HashSet<UnityEngine.SceneManagement.Scene>();

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("NPC Scene Integration Apply");

        for (int i = 0; i < controllers.Count; i++)
        {
            NPCAutoRoamController controller = controllers[i];
            if (controller == null)
            {
                continue;
            }

            touchedScenes.Add(controller.gameObject.scene);

            if (assignedProfile != null)
            {
                Undo.RecordObject(controller, "Assign NPC Roam Profile");
                SerializedObject serializedObject = new SerializedObject(controller);
                SerializedProperty profileProperty = serializedObject.FindProperty("roamProfile");
                SerializedProperty applyOnAwakeProperty = serializedObject.FindProperty("applyProfileOnAwake");
                if (profileProperty != null)
                {
                    profileProperty.objectReferenceValue = assignedProfile;
                }
                if (applyOnAwakeProperty != null)
                {
                    applyOnAwakeProperty.boolValue = true;
                }
                serializedObject.ApplyModifiedProperties();
                controller.ApplyProfile();
                EditorUtility.SetDirty(controller);
            }

            Transform anchor = controller.HomeAnchor;
            if (createHomeAnchorIfMissing && anchor == null)
            {
                GameObject anchorObject = new GameObject($"{controller.name}_HomeAnchor");
                Undo.RegisterCreatedObjectUndo(anchorObject, "Create NPC Home Anchor");
                Undo.SetTransformParent(anchorObject.transform, controller.transform.parent, "Parent NPC Home Anchor");
                anchorObject.transform.position = controller.transform.position;
                anchorObject.transform.rotation = Quaternion.identity;
                controller.SetHomeAnchor(anchorObject.transform);
                EditorUtility.SetDirty(controller);
                createdAnchors.Add(anchorObject);
                anchor = anchorObject.transform;
            }

            if (snapAnchorToNpc)
            {
                controller.SyncHomeAnchorToCurrentPosition();
                if (controller.HomeAnchor != null)
                {
                    EditorUtility.SetDirty(controller.HomeAnchor);
                }
            }

            if (mode == IntegrationMode.Production && removeStressTalkerInProduction)
            {
                NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();
                if (stressTalker != null)
                {
                    Undo.DestroyObjectImmediate(stressTalker);
                }
            }

            if (mode == IntegrationMode.BubbleReview && addStressTalkerInBubbleReview)
            {
                NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();
                if (stressTalker == null)
                {
                    stressTalker = Undo.AddComponent<NPCBubbleStressTalker>(controller.gameObject);
                }

                if (stressTalker != null)
                {
                    Undo.RecordObject(stressTalker, "Configure NPC Bubble Stress Talker");
                    stressTalker.RebindReferences();
                    EditorUtility.SetDirty(stressTalker);
                }
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        foreach (UnityEngine.SceneManagement.Scene scene in touchedScenes)
        {
            if (scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        if (selectAnchorAfterCreate && createdAnchors.Count > 0)
        {
            Selection.objects = createdAnchors.ToArray();
        }

        EditorUtility.DisplayDialog(
            "NPC Scene Integration",
            $"Applied {mode} mode to {controllers.Count} selected NPC(s).",
            "OK");
    }

    private static List<NPCAutoRoamController> GetSelectedControllers()
    {
        List<NPCAutoRoamController> controllers = new List<NPCAutoRoamController>();
        HashSet<NPCAutoRoamController> uniqueControllers = new HashSet<NPCAutoRoamController>();

        foreach (GameObject gameObject in Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.ExcludePrefab))
        {
            if (gameObject == null)
            {
                continue;
            }

            NPCAutoRoamController[] nestedControllers = gameObject.GetComponentsInChildren<NPCAutoRoamController>(true);
            for (int index = 0; index < nestedControllers.Length; index++)
            {
                NPCAutoRoamController controller = nestedControllers[index];
                if (controller == null || !uniqueControllers.Add(controller))
                {
                    continue;
                }

                controllers.Add(controller);
            }
        }

        return controllers;
    }

    private static NPCRoamProfile LoadRecommendedProfile(IntegrationMode integrationMode)
    {
        string assetPath = integrationMode == IntegrationMode.Production
            ? ProductionProfilePath
            : BubbleReviewProfilePath;

        return AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(assetPath);
    }
}
