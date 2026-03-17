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

## 2026-03-17

- 当前主线目标：修复 NPC 预制体与动画的 Sprite 丢失问题，查清是 Prefab 坏了还是资源引用链断了。
- 本轮子任务：对照 Prefab、动画 clip 与 `Assets/Sprites/NPC/*.meta` 的子资源 ID，定位用户看到的 `缺失(精灵)`。
- 本轮结论：
  - 不是 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 自身结构损坏。
  - 真正损坏点是上轮恢复时漏带了 `Assets/Sprites/NPC/001.png.meta`、`002.png.meta`、`003.png.meta`。
  - Prefab 与动画都在引用方向化切片 ID，例如 `001_Down_1 -> fileID 3869598651494229176`，而修复前现场的 `.meta` 仍是旧的 `001_0/001_1` 自动切片版，所以两边一起失配。
- 本轮修复：
  - 从 `codex/npc-roam-phase2-001` 恢复三张 NPC PNG 的 `.meta` 文件。
  - 重新刷新 Unity。
- 本轮验证：
  - `001` 的场景实例 `SpriteRenderer.sprite` 已恢复非空。
  - `001_Idle_Down.anim` / `001_Move_Down.anim` 的 Sprite 曲线引用已和 `.meta` 对齐。
  - 当前 Unity Console 为 0 条。
- 当前恢复点：NPC 的 Sprite 资源链路已补齐，可以继续让用户在 Unity 里复测 Prefab 和动画窗口。
