using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCBubbleStressTalker))]
public class NPCBubbleStressTalkerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        NPCBubbleStressTalker talker = (NPCBubbleStressTalker)target;

        EditorGUILayout.Space(8f);
        EditorGUILayout.HelpBox(
            "Test-only NPC helper. Use it on dedicated validation NPCs like 003, not on normal production NPC prefabs.",
            MessageType.Info);

        EditorGUILayout.LabelField("Bindings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Bubble Presenter", talker.BubblePresenter != null ? talker.BubblePresenter.name : "Missing");
        EditorGUILayout.LabelField("Roam Controller", talker.RoamController != null ? talker.RoamController.name : "Missing");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rebind References"))
        {
            Undo.RecordObject(talker, "Rebind NPC Bubble Stress Talker");
            talker.RebindReferences();
            EditorUtility.SetDirty(talker);
        }

        if (GUILayout.Button("Select Bubble"))
        {
            Selection.activeObject = talker.BubblePresenter != null ? talker.BubblePresenter : talker.gameObject;
        }

        if (GUILayout.Button("Select Roam"))
        {
            Selection.activeObject = talker.RoamController != null ? talker.RoamController : talker.gameObject;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Runtime", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Show Count", talker.ShowCount.ToString());
        EditorGUILayout.LabelField("Last Show Succeeded", talker.LastShowSucceeded ? "Yes" : "No");
        EditorGUILayout.LabelField("Last Line", string.IsNullOrEmpty(talker.LastLine) ? "None" : talker.LastLine);

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Speak Once Now"))
            {
                talker.TrySpeakOnce();
            }
        }
    }
}
