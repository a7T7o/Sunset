using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(NPCAutoRoamController))]
public class NPCAutoRoamControllerEditor : Editor
{
    private static readonly string[] PrimaryHomeAnchorNpcNames = { "001", "002", "003" };
    private static readonly Dictionary<int, string> AutoRepairDiagnostics = new Dictionary<int, string>();
    private const string PrimaryScenePath = "Assets/000_Scenes/Primary.unity";

    private bool showSetup = true;
    private bool showRuntimeStatus = true;
    private bool showRuntimeActions = true;

    private SerializedProperty _homeAnchorProperty;
    private SerializedProperty _roamProfileProperty;

    private void OnEnable()
    {
        _homeAnchorProperty = serializedObject.FindProperty("homeAnchor");
        _roamProfileProperty = serializedObject.FindProperty("roamProfile");
    }

    public override void OnInspectorGUI()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        serializedObject.Update();
        TryAutoRepairPrimaryHomeAnchors();
        serializedObject.UpdateIfRequiredOrScript();
        SyncHomeAnchorPropertyFromRuntime(controller);
        DrawControllerProperties(controller);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(8f);
        DrawSetupTools();
        EditorGUILayout.Space(8f);
        DrawProfileHint();
        EditorGUILayout.Space(8f);
        DrawRuntimeStatus();
        EditorGUILayout.Space(8f);
        DrawRuntimeActions();
    }

    private void DrawControllerProperties(NPCAutoRoamController controller)
    {
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (property.propertyPath == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(property, includeChildren: true);
                }

                continue;
            }

            if (property.name == "homeAnchor")
            {
                DrawHomeAnchorProperty(controller);
                continue;
            }

            EditorGUILayout.PropertyField(property, includeChildren: true);
        }
    }

    private void DrawHomeAnchorProperty(NPCAutoRoamController controller)
    {
        Transform runtimeAnchor = controller != null ? controller.HomeAnchor : null;
        Transform serializedAnchor = _homeAnchorProperty != null ? _homeAnchorProperty.objectReferenceValue as Transform : null;
        if (Application.isPlaying && runtimeAnchor != null)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Home Anchor", runtimeAnchor, typeof(Transform), true);
            }

            if (_homeAnchorProperty != null && _homeAnchorProperty.objectReferenceValue != runtimeAnchor)
            {
                EditorGUILayout.HelpBox(
                    $"Play Mode live anchor is {runtimeAnchor.name}. The inspector is showing the runtime reference directly.",
                    MessageType.None);
            }

            if (serializedAnchor == runtimeAnchor)
            {
                return;
            }
        }
        else if (_homeAnchorProperty != null)
        {
            EditorGUILayout.PropertyField(_homeAnchorProperty, includeChildren: true);
        }
        else
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Home Anchor", runtimeAnchor, typeof(Transform), true);
            }
        }

        if (Application.isPlaying && controller != null && controller.gameObject.scene.path == PrimaryScenePath)
        {
            string detail = BuildHomeAnchorDiagnostic(controller, runtimeAnchor, serializedAnchor);
            EditorGUILayout.HelpBox(detail, runtimeAnchor == null ? MessageType.Warning : MessageType.Info);
        }
    }

    private void SyncHomeAnchorPropertyFromRuntime(NPCAutoRoamController controller)
    {
        if (!Application.isPlaying || controller == null || _homeAnchorProperty == null)
        {
            return;
        }

        Transform runtimeAnchor = controller.HomeAnchor;
        if (_homeAnchorProperty.objectReferenceValue == runtimeAnchor)
        {
            return;
        }

        _homeAnchorProperty.objectReferenceValue = runtimeAnchor;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        serializedObject.UpdateIfRequiredOrScript();
        Repaint();
    }

    private void TryAutoRepairPrimaryHomeAnchors()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        if (controller == null || controller.gameObject.scene.path != PrimaryScenePath)
        {
            return;
        }

        bool isPlayMode = Application.isPlaying;
        bool touchedAny = false;
        foreach (string npcName in PrimaryHomeAnchorNpcNames)
        {
            NPCAutoRoamController npcController = FindControllerInScene(controller.gameObject.scene, npcName);
            if (npcController == null)
            {
                continue;
            }

            if (npcController.HomeAnchor != null)
            {
                SetAutoRepairDiagnostic(npcController, $"Runtime already bound to {npcController.HomeAnchor.name}.");
                continue;
            }

            Transform parent = npcController.transform.parent;
            if (parent == null)
            {
                SetAutoRepairDiagnostic(npcController, "Missing parent transform, auto-repair cannot search or create a Home Anchor.");
                continue;
            }

            string anchorSource;
            Transform anchor = FindExistingPrimaryHomeAnchor(npcController, out anchorSource);
            if (anchor == null)
            {
                string anchorName = $"{npcController.name}_HomeAnchor";
                GameObject anchorObject = new GameObject(anchorName);
                if (!isPlayMode)
                {
                    Undo.RegisterCreatedObjectUndo(anchorObject, "Auto Repair NPC Home Anchor");
                    Undo.SetTransformParent(anchorObject.transform, parent, "Parent NPC Home Anchor");
                }
                else
                {
                    anchorObject.transform.SetParent(parent, worldPositionStays: true);
                }

                anchorObject.transform.position = npcController.transform.position;
                anchorObject.transform.rotation = Quaternion.identity;
                anchor = anchorObject.transform;
                anchorSource = isPlayMode ? "runtime-created-under-parent" : "editor-created-under-parent";
                if (!isPlayMode)
                {
                    EditorUtility.SetDirty(anchorObject);
                }
            }

            if (!isPlayMode)
            {
                Undo.RecordObject(npcController, "Assign NPC Home Anchor");
            }

            npcController.BindResidentHomeAnchor(anchor);
            if (npcController.HomeAnchor == anchor)
            {
                SetAutoRepairDiagnostic(npcController, $"Bound runtime Home Anchor to {anchor.name} via {anchorSource}.");
            }
            else
            {
                SetAutoRepairDiagnostic(npcController, $"Called SetHomeAnchor({anchor.name}) via {anchorSource}, but HomeAnchor is still empty.");
            }

            if (!isPlayMode)
            {
                SerializedObject npcSerializedObject = new SerializedObject(npcController);
                SerializedProperty npcHomeAnchorProperty = npcSerializedObject.FindProperty("homeAnchor");
                if (npcHomeAnchorProperty != null)
                {
                    npcHomeAnchorProperty.objectReferenceValue = anchor;
                    npcSerializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                EditorUtility.SetDirty(npcController);
                EditorUtility.SetDirty(anchor);
                touchedAny = true;
            }
        }

        if (touchedAny && !isPlayMode)
        {
            EditorSceneManager.MarkSceneDirty(controller.gameObject.scene);
        }
    }

    private static Transform FindExistingPrimaryHomeAnchor(NPCAutoRoamController controller)
    {
        return FindExistingPrimaryHomeAnchor(controller, out _);
    }

    private static Transform FindExistingPrimaryHomeAnchor(NPCAutoRoamController controller, out string source)
    {
        source = "not-found";
        if (controller == null)
        {
            return null;
        }

        string anchorName = $"{controller.name}_HomeAnchor";
        Transform parent = controller.transform.parent;
        if (parent == null)
        {
            Transform selfChildAnchor = controller.transform.Find(anchorName);
            if (selfChildAnchor != null)
            {
                source = $"self-child:{controller.name}";
                return selfChildAnchor;
            }

            return FindSceneAnchorByName(controller.gameObject.scene, anchorName, out source);
        }

        Transform siblingAnchor = parent.Find(anchorName);
        if (siblingAnchor != null)
        {
            source = $"parent-sibling:{parent.name}";
            return siblingAnchor;
        }

        Transform childAnchor = controller.transform.Find(anchorName);
        if (childAnchor != null)
        {
            source = $"self-child:{controller.name}";
            return childAnchor;
        }

        return FindSceneAnchorByName(controller.gameObject.scene, anchorName, out source);
    }

    private static Transform FindSceneAnchorByName(Scene scene, string anchorName, out string source)
    {
        source = "not-found";
        if (!scene.IsValid())
        {
            return null;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            Transform match = FindTransformRecursive(roots[rootIndex].transform, anchorName);
            if (match != null)
            {
                source = $"scene-search:{roots[rootIndex].name}";
                return match;
            }
        }

        return null;
    }

    private static Transform FindTransformRecursive(Transform root, string targetName)
    {
        if (root == null)
        {
            return null;
        }

        if (string.Equals(root.name, targetName, System.StringComparison.Ordinal))
        {
            return root;
        }

        for (int childIndex = 0; childIndex < root.childCount; childIndex++)
        {
            Transform match = FindTransformRecursive(root.GetChild(childIndex), targetName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static void SetAutoRepairDiagnostic(NPCAutoRoamController controller, string message)
    {
        if (controller == null)
        {
            return;
        }

        AutoRepairDiagnostics[controller.GetInstanceID()] = message;
    }

    private static string GetAutoRepairDiagnostic(NPCAutoRoamController controller)
    {
        if (controller == null)
        {
            return "No controller.";
        }

        if (AutoRepairDiagnostics.TryGetValue(controller.GetInstanceID(), out string message))
        {
            return message;
        }

        return "No auto-repair attempt recorded for this inspector yet.";
    }

    private static string GetAnchorDisplayName(Transform anchor)
    {
        return anchor != null ? anchor.name : "None";
    }

    private static string BuildHomeAnchorDiagnostic(
        NPCAutoRoamController controller,
        Transform runtimeAnchor,
        Transform serializedAnchor)
    {
        Transform detectedAnchor = FindExistingPrimaryHomeAnchor(controller, out string source);
        string parentName = controller != null && controller.transform.parent != null
            ? controller.transform.parent.name
            : "None";

        return
            $"Runtime Home Anchor: {GetAnchorDisplayName(runtimeAnchor)}\n" +
            $"Serialized Home Anchor: {GetAnchorDisplayName(serializedAnchor)}\n" +
            $"Detected Anchor Candidate: {GetAnchorDisplayName(detectedAnchor)} ({source})\n" +
            $"Parent: {parentName}\n" +
            $"Auto-repair: {GetAutoRepairDiagnostic(controller)}";
    }

    private void DrawSetupTools()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();

        showSetup = EditorGUILayout.BeginFoldoutHeaderGroup(showSetup, "Scene Integration");
        if (showSetup)
        {
            EditorGUI.indentLevel++;

            string centerSource = controller.HomeAnchor != null ? "Home Anchor" : "NPC Self";
            string profileName = controller.RoamProfile != null ? controller.RoamProfile.name : "None";

            EditorGUILayout.HelpBox(
                $"Roam center source: {centerSource}\n" +
                $"Activity radius: {controller.ActivityRadius:F2}\n" +
                $"Min move distance: {controller.MinimumMoveDistance:F2}\n" +
                $"Sample attempts: {controller.PathSampleAttempts}\n" +
                $"Profile: {profileName}\n" +
                $"Current debug path points: {controller.DebugPathPointCount}",
                MessageType.None);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(controller.HomeAnchor == null ? "Create Home Anchor" : "Select Home Anchor"))
            {
                if (controller.HomeAnchor == null)
                {
                    CreateHomeAnchor(controller);
                }
                else
                {
                    Selection.activeObject = controller.HomeAnchor.gameObject;
                }
            }

            if (GUILayout.Button("Snap Anchor To NPC"))
            {
                Undo.RecordObject(controller, "Snap NPC Home Anchor");
                controller.SyncHomeAnchorToCurrentPosition();
                EditorUtility.SetDirty(controller);
                if (controller.HomeAnchor != null)
                {
                    EditorUtility.SetDirty(controller.HomeAnchor);
                }
            }

            if (GUILayout.Button("Clear Home Anchor"))
            {
                Undo.RecordObject(controller, "Clear NPC Home Anchor");
                controller.BindResidentHomeAnchor(null);
                _homeAnchorProperty.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(controller);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Profile Now"))
            {
                Undo.RecordObject(controller, "Apply NPC Roam Profile");
                controller.SyncRuntimeProfileFromAsset();
                EditorUtility.SetDirty(controller);
            }

            if (GUILayout.Button("Select Profile"))
            {
                Selection.activeObject = controller.RoamProfile != null ? controller.RoamProfile : controller.gameObject;
            }

            if (GUILayout.Button("Duplicate Profile Copy"))
            {
                DuplicateProfileCopy(controller);
            }
            EditorGUILayout.EndHorizontal();

            if (stressTalker != null)
            {
                EditorGUILayout.Space(4f);
                EditorGUILayout.HelpBox(
                    $"This NPC has an attached bubble stress talker.\nAuto Start: {(stressTalker.StartOnEnable ? "On" : "Off")}\nDisable Roam While Testing: {(stressTalker.DisableRoamWhileTesting ? "Yes" : "No")}",
                    MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Stress Talker"))
                {
                    Selection.activeObject = stressTalker;
                }

                if (GUILayout.Button("Enable Stress Auto Start"))
                {
                    Undo.RecordObject(stressTalker, "Enable NPC Stress Auto Start");
                    stressTalker.ConfigureMode(enableOnStart: true, disableRoamDuringTest: true);
                    EditorUtility.SetDirty(stressTalker);
                }

                if (GUILayout.Button("Disable Stress Auto Start"))
                {
                    Undo.RecordObject(stressTalker, "Disable NPC Stress Auto Start");
                    stressTalker.ConfigureMode(enableOnStart: false, disableRoamDuringTest: true);
                    EditorUtility.SetDirty(stressTalker);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.HelpBox(
                "Tip: select this NPC in Scene view to see the activity radius gizmo and current path preview.",
                MessageType.Info);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawProfileHint()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        string profileName = controller.RoamProfile != null ? controller.RoamProfile.name : "None";
        EditorGUILayout.HelpBox(
            $"Current roam profile: {profileName}\n" +
            "If no profile is assigned, the component keeps using its local serialized values.",
            MessageType.None);
    }

    private void DrawRuntimeStatus()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        NPCBubblePresenter bubblePresenter = controller.GetComponent<NPCBubblePresenter>();
        NPCMotionController motionController = controller.GetComponent<NPCMotionController>();
        NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();

        showRuntimeStatus = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeStatus, "Runtime Status");
        if (showRuntimeStatus)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("State", controller.DebugState);
            EditorGUILayout.LabelField("State Timer", $"{controller.DebugStateTimer:F2}s");
            EditorGUILayout.LabelField("Short Pause Count", $"{controller.CompletedShortPauseCount}/{controller.LongPauseTriggerCount}");
            EditorGUILayout.LabelField("Stuck Recoveries", controller.CurrentStuckRecoveryCount.ToString());
            EditorGUILayout.LabelField("Recent Progress", $"{controller.DebugLastProgressDistance:F3}m");
            EditorGUILayout.LabelField("Is Roaming", controller.IsRoaming ? "Yes" : "No");
            EditorGUILayout.LabelField("Is Moving", controller.IsMoving ? "Yes" : "No");
            EditorGUILayout.LabelField("Ambient Chat", controller.IsInAmbientChat ? "Active" : "Idle");
            EditorGUILayout.LabelField("Chat Partner", string.IsNullOrEmpty(controller.ChatPartnerName) ? "None" : controller.ChatPartnerName);
            EditorGUILayout.LabelField("Last Decision", string.IsNullOrEmpty(controller.LastAmbientDecision) ? "None" : controller.LastAmbientDecision);

            if (motionController != null)
            {
                EditorGUILayout.LabelField("Move Speed", motionController.MoveSpeed.ToString("F2"));
                EditorGUILayout.LabelField("Current Velocity", motionController.CurrentVelocity.ToString("F2"));
            }

            if (bubblePresenter != null)
            {
                EditorGUILayout.LabelField("Bubble Visible", bubblePresenter.IsBubbleVisible ? "Yes" : "No");
                EditorGUILayout.LabelField("Bubble Text", string.IsNullOrEmpty(bubblePresenter.CurrentBubbleText) ? "None" : bubblePresenter.CurrentBubbleText);
            }

            if (stressTalker != null)
            {
                EditorGUILayout.LabelField("Stress Talk Count", stressTalker.ShowCount.ToString());
                EditorGUILayout.LabelField("Stress Last Success", stressTalker.LastShowSucceeded ? "Yes" : "No");
                EditorGUILayout.LabelField("Stress Last Line", string.IsNullOrEmpty(stressTalker.LastLine) ? "None" : stressTalker.LastLine);
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Not in Play Mode. Values shown here are serialized snapshot values.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawRuntimeActions()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        NPCBubblePresenter bubblePresenter = controller.GetComponent<NPCBubblePresenter>();
        NPCBubbleStressTalker stressTalker = controller.GetComponent<NPCBubbleStressTalker>();

        showRuntimeActions = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeActions, "Debug Actions");
        if (showRuntimeActions)
        {
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Start Roam"))
                {
                    controller.StartRoam();
                }

                if (GUILayout.Button("Stop Roam"))
                {
                    controller.StopRoam();
                }

                if (GUILayout.Button("Force Long Pause"))
                {
                    controller.DebugEnterLongPause();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Try Ambient Chat"))
                {
                    controller.DebugTryAmbientChat();
                }

                if (GUILayout.Button("Show Random Bubble"))
                {
                    bubblePresenter?.ShowRandomSelfTalk(2.5f);
                }

                if (GUILayout.Button("Hide Bubble"))
                {
                    bubblePresenter?.HideBubble();
                }
                EditorGUILayout.EndHorizontal();

                if (stressTalker != null && GUILayout.Button("Stress Talk Once"))
                {
                    stressTalker.TrySpeakOnce();
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void CreateHomeAnchor(NPCAutoRoamController controller)
    {
        GameObject anchorObject = new GameObject($"{controller.name}_HomeAnchor");
        Transform anchorParent = controller.transform.parent != null ? controller.transform.parent : controller.transform;
        Undo.RegisterCreatedObjectUndo(anchorObject, "Create NPC Home Anchor");
        Undo.SetTransformParent(anchorObject.transform, anchorParent, "Parent NPC Home Anchor");
        anchorObject.transform.position = controller.transform.position;
        anchorObject.transform.rotation = Quaternion.identity;
        controller.BindResidentHomeAnchor(anchorObject.transform);
        _homeAnchorProperty.objectReferenceValue = anchorObject.transform;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(anchorObject);
        Selection.activeObject = anchorObject;
    }

    private static NPCAutoRoamController FindControllerInScene(UnityEngine.SceneManagement.Scene scene, string npcName)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            NPCAutoRoamController[] controllers = roots[rootIndex].GetComponentsInChildren<NPCAutoRoamController>(true);
            for (int controllerIndex = 0; controllerIndex < controllers.Length; controllerIndex++)
            {
                NPCAutoRoamController controller = controllers[controllerIndex];
                if (controller != null && string.Equals(controller.name, npcName, System.StringComparison.Ordinal))
                {
                    return controller;
                }
            }
        }

        return null;
    }

    private void DuplicateProfileCopy(NPCAutoRoamController controller)
    {
        if (controller.RoamProfile == null)
        {
            EditorUtility.DisplayDialog("Duplicate Profile", "Assign a roam profile first.", "OK");
            return;
        }

        string sourcePath = AssetDatabase.GetAssetPath(controller.RoamProfile);
        string folderPath = Path.GetDirectoryName(sourcePath) ?? "Assets";
        string assetName = $"{controller.name}_{controller.RoamProfile.name}.asset";
        string targetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath.Replace('\\', '/')}/{assetName}");

        NPCRoamProfile copy = Object.Instantiate(controller.RoamProfile);
        AssetDatabase.CreateAsset(copy, targetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        _roamProfileProperty.objectReferenceValue = copy;
        serializedObject.ApplyModifiedProperties();
        Undo.RecordObject(controller, "Assign NPC Roam Profile Copy");
        controller.SyncRuntimeProfileFromAsset();
        EditorUtility.SetDirty(controller);
        Selection.activeObject = copy;
    }
}
