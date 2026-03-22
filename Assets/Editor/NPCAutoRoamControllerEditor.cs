using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCAutoRoamController))]
public class NPCAutoRoamControllerEditor : Editor
{
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
        serializedObject.Update();
        DrawDefaultInspector();
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
                controller.SetHomeAnchor(null);
                _homeAnchorProperty.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(controller);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply Profile Now"))
            {
                Undo.RecordObject(controller, "Apply NPC Roam Profile");
                controller.ApplyProfile();
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
                    "This NPC has an attached bubble stress talker. Treat it as a validation NPC, not a normal production NPC.",
                    MessageType.Info);

                if (GUILayout.Button("Select Stress Talker"))
                {
                    Selection.activeObject = stressTalker;
                }
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
        Undo.RegisterCreatedObjectUndo(anchorObject, "Create NPC Home Anchor");
        Undo.SetTransformParent(anchorObject.transform, controller.transform, "Parent NPC Home Anchor");
        anchorObject.transform.position = controller.transform.position;
        anchorObject.transform.rotation = Quaternion.identity;
        controller.SetHomeAnchor(anchorObject.transform);
        _homeAnchorProperty.objectReferenceValue = anchorObject.transform;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(anchorObject);
        Selection.activeObject = anchorObject;
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
        controller.ApplyProfile();
        EditorUtility.SetDirty(controller);
        Selection.activeObject = copy;
    }
}
