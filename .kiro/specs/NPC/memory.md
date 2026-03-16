# NPC 工作区 memory

## 2026-03-16

- 当前主线目标：完成 Sunset 项目的 NPC 通用模板生成、预制体落地，以及自动漫游与环境气泡行为的可运行闭环。
- 当前子工作区：`D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划`
- 本轮阻塞：项目当前真实现场不在预期的 NPC 分支，且 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/` 等 NPC 第二阶段资产在当前工作树缺失。
- 本轮完成：
  - 从 `codex/npc-roam-phase2-001` 精确回填 NPC 第二阶段运行时代码与资产到当前现场。
  - 回填 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的兼容版本，解除 `GameInputManager -> ClearAllQueuePreviews(bool)` 编译阻断。
  - 修复 `Assets/Editor/NPCPrefabGeneratorTool.cs` 使用 `TextureImporter.spritesheet` 的废弃 API，改为 `ISpriteEditorDataProvider`。
  - 在 `Primary` 场景重新验证 NPC 001/002/003：自动漫游正常、短停/长停节奏正常、附近聊天正样本已再次复现。
- 已验证事实：
  - Unity Console 当前无 NPC 项目级红错。
  - 用户此前给出的 `Animator is not playing an AnimatorController` 本轮未复现。
  - `001` / `002` / `003` 的 `NPCAutoRoamController` 实时状态均可读到 `IsRoaming=true`，且出现了 `chatting with 002`、`joined chat with 001` 等正样本。
- 当前恢复点：NPC 第二阶段代码与基础资产已恢复到可继续开发状态，下一步应围绕 `npc规划001.md` 继续推进后续行为迭代或更精细的手工验收。
