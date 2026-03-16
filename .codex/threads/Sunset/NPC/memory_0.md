# NPC 线程记忆

## 2026-03-16

- 当前主线目标：完成 Sunset 项目的 NPC 固定模板生成器与第二阶段自动漫游/气泡行为闭环。
- 本轮子任务：根据用户贴出的控制台报错，先确认实时阻断，再修复当前现场并把 NPC 第二阶段恢复到可继续开发的状态。
- 本轮真实现场：
  - 工作目录：`D:\Unity\Unity_learning\Sunset`
  - 当前分支：`codex/farm-1.0.2-correct001`
  - 起手时发现 `.kiro/specs/NPC/` 与 `.codex/threads/Sunset/NPC/` 在当前现场缺失。
  - 当前现场真实编译阻断不是 Animator，而是 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs(2512,34)` 调用 `ClearAllQueuePreviews(bool)` 与 `FarmToolPreview` 版本不匹配。
- 本轮完成：
  - 从 `codex/npc-roam-phase2-001` 精确回填 `FarmToolPreview` 兼容版本，以及 NPC 第二阶段代码与资产。
  - 从 `codex/npc-fixed-template-main` 回填并继续修正 `Assets/Editor/NPCPrefabGeneratorTool.cs`，把 `TextureImporter.spritesheet` 改成 `ISpriteEditorDataProvider`。
  - 新建并恢复 `NPC` 工作区文档链路：父 memory、子 memory、tasks、npc规划001、线程 memory。
- 本轮验证：
  - Unity Console 当前无 NPC 项目级红错。
  - `Animator is not playing an AnimatorController` 本轮未复现。
  - `Primary` 场景进入 Play 后，`001` / `002` / `003` 的 `NPCAutoRoamController` 均可读到 `IsRoaming=true`。
  - 再次抓到附近聊天正样本：
    - `001.LastAmbientDecision = "chatting with 002"`
    - `002.LastAmbientDecision = "joined chat with 001"`
- 当前恢复点：NPC 第二阶段已经恢复到可继续开发状态；若继续推进，优先沿 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\npc规划001.md` 迭代新的行为需求。
