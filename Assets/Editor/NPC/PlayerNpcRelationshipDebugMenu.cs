using System.Text;
using UnityEditor;
using UnityEngine;

public static class PlayerNpcRelationshipDebugMenu
{
    private const string MenuRoot = "Tools/NPC/Relationship/";
    private static readonly string[] DemoNpcIds = { "001", "002", "003" };

    [MenuItem(MenuRoot + "全部设为/陌生")]
    private static void SetAllToStranger()
    {
        SetAllStages(NPCRelationshipStage.Stranger);
    }

    [MenuItem(MenuRoot + "全部设为/认识")]
    private static void SetAllToAcquainted()
    {
        SetAllStages(NPCRelationshipStage.Acquainted);
    }

    [MenuItem(MenuRoot + "全部设为/熟悉")]
    private static void SetAllToFamiliar()
    {
        SetAllStages(NPCRelationshipStage.Familiar);
    }

    [MenuItem(MenuRoot + "全部设为/亲近")]
    private static void SetAllToClose()
    {
        SetAllStages(NPCRelationshipStage.Close);
    }

    [MenuItem(MenuRoot + "全部清除持久化")]
    private static void ClearAllPersistedStages()
    {
        for (int index = 0; index < DemoNpcIds.Length; index++)
        {
            PlayerNpcRelationshipService.ResetStage(DemoNpcIds[index], true);
        }

        Debug.Log("[PlayerNpcRelationshipDebugMenu] 已清除 001 / 002 / 003 的关系阶段持久化。");
    }

    [MenuItem(MenuRoot + "打印当前阶段")]
    private static void LogCurrentStages()
    {
        StringBuilder builder = new StringBuilder("[PlayerNpcRelationshipDebugMenu] 当前关系阶段：");
        for (int index = 0; index < DemoNpcIds.Length; index++)
        {
            string npcId = DemoNpcIds[index];
            builder.Append(' ');
            builder.Append(npcId);
            builder.Append('=');
            builder.Append(PlayerNpcRelationshipService.GetStage(npcId));
        }

        Debug.Log(builder.ToString());
    }

    private static void SetAllStages(NPCRelationshipStage relationshipStage)
    {
        for (int index = 0; index < DemoNpcIds.Length; index++)
        {
            PlayerNpcRelationshipService.SetStage(DemoNpcIds[index], relationshipStage, true);
        }

        Debug.Log($"[PlayerNpcRelationshipDebugMenu] 已将 001 / 002 / 003 统一设为 {relationshipStage}。");
    }
}
