using System.IO;
using NUnit.Framework;

[TestFixture]
public class SaveManagerDefaultSlotContractTests
{
    private static readonly string ProjectRoot =
        Directory.GetParent(UnityEngine.Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    private static readonly string SaveManagerPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/Data/Core/SaveManager.cs");

    private static readonly string SavePanelPath =
        Path.Combine(ProjectRoot, "Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs");

    [Test]
    public void DefaultSlot_ShouldBeRealProtectedSaveSlot()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("private const string DefaultProgressDisplayName = \"默认存档\";", saveManagerText,
            "默认槽正式名称必须回到“默认存档”，不能继续叫默认开局。");
        StringAssert.Contains("return SaveGameInternal(DefaultProgressSlotName, enforceSaveBlockers: true, raiseSlotChangedEvent: true, allowProtectedDefaultSlotWrite: true);", saveManagerText,
            "F5 对应的默认槽快速保存必须是真写默认槽文件。");
        StringAssert.Contains("return LoadGameInternal(DefaultProgressSlotName, refreshUi: true, raiseSlotChangedEvent: true, onCompleted);", saveManagerText,
            "F9 / 默认槽加载必须是真读默认槽文件，而不是继续走原生重开。");
        StringAssert.Contains("if (IsDefaultSlot(slotName) && !allowProtectedDefaultSlotWrite)", saveManagerText,
            "默认槽应是受保护槽：允许快速保存，但不允许混进普通覆盖入口。");
        StringAssert.Contains("summary = CreateEmptyDefaultSlotSummary();", saveManagerText,
            "默认槽摘要应先按真实默认槽文件读取，空槽时再给空态摘要。");
    }

    [Test]
    public void Restart_ShouldStaySeparateFromDefaultLoad()
    {
        string saveManagerText = File.ReadAllText(SaveManagerPath);

        StringAssert.Contains("public bool RestartToFreshGame(Action<bool> onCompleted = null)", saveManagerText);
        StringAssert.Contains("return BeginNativeFreshRestart(onCompleted);", saveManagerText,
            "重新开始仍应只负责回到原生起点。");
        StringAssert.Contains("if (!SaveExists(DefaultProgressSlotName))", saveManagerText,
            "默认槽读取与重新开始必须彻底分开，空默认槽时不能假装去读。");
    }

    [Test]
    public void SavePanel_ShouldExposeDefaultSaveInsteadOfDefaultOpening()
    {
        string panelText = File.ReadAllText(SavePanelPath);

        StringAssert.Contains("\"F5 快速保存默认存档    F9 快速读取默认存档\"", panelText,
            "设置页快捷说明必须回到 F5 快存 / F9 快读默认存档。");
        StringAssert.Contains("\"默认存档\"", panelText,
            "设置页默认区标题必须改回默认存档。");
        StringAssert.Contains("\"加载默认存档\"", panelText,
            "默认槽按钮必须是加载默认存档，不再是加载默认开局。");
        StringAssert.DoesNotContain("\"只读入口\"", panelText,
            "默认槽不应再作为只读开局入口展示。");
        StringAssert.Contains("\"默认槽：加载 / 复制    普通槽：加载 / 复制 / 粘贴 / 覆盖 / 删除\"", panelText,
            "默认槽与普通槽的能力矩阵必须在 UI 上明确区分。");
    }
}
