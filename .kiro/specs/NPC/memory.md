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

## 2026-03-17

- 当前主线目标：继续收口 NPC 资产链路，确保生成出来的 Prefab、动画和源 PNG 子资源引用一致。
- 本轮阻塞：用户在 Unity 中发现 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 的 `SpriteRenderer.sprite` 为空，同时动画窗口里的 Sprite 曲线也全部丢失。
- 本轮完成：
  - 对照 Prefab YAML、动画 YAML 与 `Assets/Sprites/NPC/*.png.meta`，确认断点在子 Sprite fileID 不匹配。
  - 恢复 `Assets/Sprites/NPC/001.png.meta`、`Assets/Sprites/NPC/002.png.meta`、`Assets/Sprites/NPC/003.png.meta` 到与当前 Prefab/动画匹配的版本。
  - 重新刷新 Unity AssetDatabase。
- 已验证事实：
  - `Assets/222_Prefabs/NPC/001.prefab` 当前引用 `m_Sprite.fileID = 3869598651494229176`，修复后的 `Assets/Sprites/NPC/001.png.meta` 已重新包含 `001_Down_1 -> 3869598651494229176`。
  - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 与 `001_Move_Down.anim` 的 `m_PPtrCurves` 当前引用的 fileID 均可在修复后的 `.meta` 中找到对应子 Sprite。
  - 读取 `Primary` 场景中 `001` 的 `SpriteRenderer` 实时组件时，`sprite` 已恢复为 `Assets/Sprites/NPC/001.png`。
  - 当前 Unity Console 为 0 条。
- 关键决策：
  - 这次不重生 Prefab、不重切 PNG、不重做动画，只补回缺失的三份 `.meta`，避免再次漂移资源 GUID / fileID。
- 当前恢复点：NPC 资源链路已经从“运行时代码恢复”推进到“源 PNG 子资源引用恢复”，可以继续做用户现场复测。

- 当前主线目标：在继续 NPC 功能开发前，先把 NPC 线程的 Git / worktree 漂移问题取证清楚，避免后续治理与收口继续混线。
- 本轮阻塞：用户发现我口径里反复把 NPC 和旧 worktree 绑定在一起，同时当前根仓库又真实落在 `codex/farm-1.0.2-correct001`，需要解释“NPC 分支、main、worktree、farm”之间到底发生了什么。
- 本轮完成：
  - 只读核查 `git worktree list`、`git branch -vv`、`git reflog --all`，确认当前默认根仓库现场并不在 `main`，而是在 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`。
  - 确认旧 `D:\Unity\Unity_learning\Sunset_worktrees\NPC` 仍存在，但它只是 `codex/npc-generator-pipeline` 的一个历史检出位置，不代表 NPC 相关分支都“在 worktree 里”。
  - 重建时间线：`codex/npc-asset-solidify-001 -> main -> codex/farm-1.0.0-1.0.1 -> main -> codex/farm-1.0.2-correction001 -> codex/npc-main-recover-001 -> codex/farm-1.0.2-correct001 -> codex/npc-roam-phase2-001 -> codex/farm-1.0.2-correct001`。
  - 确认 `07ffe199`、`18f3a9e1` 两个关键 NPC 修复提交实落在 `codex/farm-1.0.2-correct001`，并未进入 `main`，也不在现存的 `codex/npc-*` 线上。
- 已证实事实：
  - 分支是仓库级对象，worktree 只是某个分支/提交的检出目录；`main` 分支始终存在，当前指向 `b9b6ac48`。
  - `codex/npc-fixed-template-main`、`codex/npc-asset-solidify-001`、`codex/npc-main-recover-001`、`codex/npc-roam-phase2-001` 都存在于仓库中，并非 worktree 私有。
  - 03-16 起项目规则已经改成“NPC 默认不再使用独立 worktree，默认从根仓库 `main` 进入，再切 `codex/...` 分支实现”，但 Git 现场仍保留旧 NPC worktree 残留。
- 当前恢复点：NPC 功能本身的代码与资产恢复结论不变，但新增一个明确的治理问题：NPC 二阶段成果当前挂在 farm 分支上，需要治理线程决定后续如何回收或重排。

- 当前主线目标：按治理裁定把 NPC 后续开发基线收敛到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再把共享根目录的 farm 事故现场当作 NPC 可写现场。
- 本轮阻塞：需要确认 `f6b4db2f` 是否已经足够自洽，以及 farm 侧混入的最小剔除范围到底有多大。
- 本轮完成：
  - 只读核对 `f6b4db2f` 的树内容，确认 NPC 核心资产、Prefab、PNG meta、运行时代码与生成器都已经在这条线内齐备。
  - 只读对比 `f6b4db2f` 与 farm 上 `07ffe199` / `18f3a9e1`，确认 NPC 范围不缺核心文件；差异主要集中在 `Assets/Editor/NPCPrefabGeneratorTool.cs` 和 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - 继续对比 `8aed637f -> f6b4db2f`，确认真正不该继续留在 NPC 线中的 farm 侧拖带改动只有 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
- 已证实事实：
  - `f6b4db2f` 已包含 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.png.meta`、`Assets/YYY_Scripts/Anim/NPC/`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/Editor/NPCPrefabGeneratorTool.cs`。
  - 在 `f6b4db2f` 内，`GameInputManager.cs` 只调用无参 `ClearAllQueuePreviews()`；没有 NPC 代码依赖 `FarmToolPreview` 里新增的 bool 重载。
  - `f6b4db2f` 版 `NPCPrefabGeneratorTool.cs` 足以作为救援基线版本保留，但这表示“先保证救援线自洽”，不等于“最终工具 UX 已经做完”。
- 当前恢复点：NPC 现在的最小干净收口方案已经清楚了，后续应在独立 NPC 可写现场里只做一刀：回退 `FarmToolPreview.cs` 到 `8aed637f` 版本，再做最小验证。
