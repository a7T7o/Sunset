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

- 当前主线目标：在继续 NPC 主线开发前，先澄清这条线程为什么会出现“嘴上是 NPC，Git 现场却在 farm”的分支/工作树漂移。
- 本轮子任务：仅做 Git 与规则取证，不改代码、不改场景，给治理线程准备可直接引用的写实结论。
- 本轮已证实：
  - 当前根仓库真实现场是 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`，`HEAD = 11e0b7b4`。
  - 当前 repo 仍保留旧 NPC worktree：`D:\Unity\Unity_learning\Sunset_worktrees\NPC @ codex/npc-generator-pipeline`。
  - 分支不是 worktree 私有对象；`main` 与多条 `codex/npc-*` 分支都在仓库里正常存在，旧 NPC worktree 只是其中一条历史分支的检出位置。
  - `git reflog` 说明 2026-03-17 01:35 曾从 `codex/farm-1.0.2-correct001` 切到 `codex/npc-roam-phase2-001`，01:44 又切回 `codex/farm-1.0.2-correct001`；后续 `07ffe199`、`18f3a9e1`、`11e0b7b4` 三个提交都落在 farm 分支。
  - 其中 `07ffe199`、`18f3a9e1` 实际承载了 NPC 二阶段恢复与 PNG meta 修复内容，当前只被 `codex/farm-1.0.2-correct001` 包含。
- 本轮反思：
  - 我前面的表述把“历史上有 NPC 独立 worktree”说得过于像“现在 NPC 就等于那个 worktree”，这是不准确的。
  - 更准确的写法应是：规则层已经把 NPC 的默认入口改回根仓库 `main`，但 Git 现场同时残留旧 NPC worktree，而本轮根仓库又实际停在 farm 分支上；我后来回到当前根分支继续提交，才导致 NPC 成果挂到了 farm。
- 当前恢复点：现在已经能够把“规则口径”和“Git 现场”拆开说明，下一步应交由治理线程裁定 NPC 成果与旧 worktree 的收口方式。

- 当前主线目标：按治理裁定把 NPC 的唯一救援基线锁定到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再借用共享根目录的 farm 事故现场继续推进 NPC。
- 本轮子任务：只读确认 `f6b4db2f` 是否已经包含 NPC 核心资产 / Prefab / meta / 运行时代码，并给出最小收口建议。
- 本轮已证实：
  - `f6b4db2f` 已经包含 NPC 所需的动画、控制器、默认 roam profile、001~003 Prefab、001~003 PNG 与 `.meta`、`NPCAnimController`、`NPCMotionController`、`NPCAutoRoamController`、`NPCBubblePresenter`、`NPCRoamProfile`、`NPCPrefabGeneratorTool`。
  - `f6b4db2f` 对比 farm 的 `18f3a9e1`，NPC 范围内只剩 `Assets/Editor/NPCPrefabGeneratorTool.cs` 与 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 两个差异；说明这条救援线已经不缺 NPC 核心文件。
  - `8aed637f -> f6b4db2f` 的对比表明，真正应从 NPC 线剔除的 farm 侧拖带改动是 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - `NPCPrefabGeneratorTool.cs` 保持 `f6b4db2f` 版本即可作为救援线版本；下一步最小验证应是“回退 `FarmToolPreview.cs` 后，做 Unity 编译 + `001/002/003` Prefab/动画引用检查”。
- 当前恢复点：NPC 已经有了明确的唯一救援基线和最小收口边界，后续只需在独立 NPC 可写现场里执行剔除与验证，不需要再从 farm 搬回更多 NPC 核心文件。
