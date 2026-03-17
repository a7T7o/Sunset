using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCAutoRoamController))]
public class NPCAutoRoamControllerEditor : Editor
{
    private bool showRuntimeStatus = true;
    private bool showRuntimeActions = true;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(8f);
        DrawProfileHint();
        EditorGUILayout.Space(8f);
        DrawRuntimeStatus();
        EditorGUILayout.Space(8f);
        DrawRuntimeActions();
    }

    private void DrawProfileHint()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        string profileName = controller.RoamProfile != null ? controller.RoamProfile.name : "未指定";
        EditorGUILayout.HelpBox(
            $"当前漫游配置资产：{profileName}\n" +
            "未指定 Profile 时继续使用组件内参数；指定后可用下方按钮重新应用到当前 NPC。",
            MessageType.None);
    }

    private void DrawRuntimeStatus()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        NPCBubblePresenter bubblePresenter = controller.GetComponent<NPCBubblePresenter>();
        NPCMotionController motionController = controller.GetComponent<NPCMotionController>();

        showRuntimeStatus = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeStatus, "运行时状态");
        if (showRuntimeStatus)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("当前状态", controller.DebugState);
            EditorGUILayout.LabelField("状态计时", $"{controller.DebugStateTimer:F2}s");
            EditorGUILayout.LabelField("短停计数", $"{controller.CompletedShortPauseCount}/{controller.LongPauseTriggerCount}");
            EditorGUILayout.LabelField("卡住重试", controller.CurrentStuckRecoveryCount.ToString());
            EditorGUILayout.LabelField("最近位移", $"{controller.DebugLastProgressDistance:F3}m");
            EditorGUILayout.LabelField("正在漫游", controller.IsRoaming ? "是" : "否");
            EditorGUILayout.LabelField("正在移动", controller.IsMoving ? "是" : "否");
            EditorGUILayout.LabelField("环境聊天", controller.IsInAmbientChat ? "进行中" : "未进行");
            EditorGUILayout.LabelField("聊天对象", string.IsNullOrEmpty(controller.ChatPartnerName) ? "无" : controller.ChatPartnerName);
            EditorGUILayout.LabelField("最近决策", string.IsNullOrEmpty(controller.LastAmbientDecision) ? "无" : controller.LastAmbientDecision);

            if (motionController != null)
            {
                EditorGUILayout.LabelField("移动速度", motionController.MoveSpeed.ToString("F2"));
                EditorGUILayout.LabelField("当前速度", motionController.CurrentVelocity.ToString("F2"));
            }

            if (bubblePresenter != null)
            {
                EditorGUILayout.LabelField("气泡可见", bubblePresenter.IsBubbleVisible ? "是" : "否");
                EditorGUILayout.LabelField("气泡文本", string.IsNullOrEmpty(bubblePresenter.CurrentBubbleText) ? "无" : bubblePresenter.CurrentBubbleText);
                EditorGUILayout.LabelField("自言自语条数", bubblePresenter.SelfTalkLineCount.ToString());
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("当前不在 Play 模式，状态值显示的是最近一次序列化快照。", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawRuntimeActions()
    {
        NPCAutoRoamController controller = (NPCAutoRoamController)target;
        NPCBubblePresenter bubblePresenter = controller.GetComponent<NPCBubblePresenter>();

        showRuntimeActions = EditorGUILayout.BeginFoldoutHeaderGroup(showRuntimeActions, "调试动作");
        if (showRuntimeActions)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("应用 Profile"))
            {
                Undo.RecordObject(controller, "Apply NPC Roam Profile");
                controller.ApplyProfile();
                EditorUtility.SetDirty(controller);
            }

            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("开始漫游"))
            {
                controller.StartRoam();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("停止漫游"))
            {
                controller.StopRoam();
            }

            if (GUILayout.Button("强制长停"))
            {
                controller.DebugEnterLongPause();
            }

            if (GUILayout.Button("尝试聊天"))
            {
                controller.DebugTryAmbientChat();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = Application.isPlaying && bubblePresenter != null;
            if (GUILayout.Button("显示自言自语"))
            {
                bubblePresenter.ShowRandomSelfTalk(2.5f);
            }

            if (GUILayout.Button("隐藏气泡"))
            {
                bubblePresenter.HideBubble();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
