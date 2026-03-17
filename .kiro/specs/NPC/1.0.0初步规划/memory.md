# NPC 1.0.0 初步规划 memory

## 2026-03-16

- 当前主线目标：围绕固定 4 向 3 帧 NPC 模板，完成从 PNG 生成到运行时漫游/气泡行为的第一阶段闭环，并为后续脑暴保留统一规划底稿。
- 本轮服务的子任务：修复当前现场编译阻断，恢复 NPC 第二阶段真实代码与资产，并用 Unity live 证据重新确认自动漫游确实可运行。
- 本轮完成：
  - 解除 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs(2512,34)` 对 `ClearAllQueuePreviews(bool)` 的调用阻断，实际通过更新 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 完成兼容。
  - 恢复 `NPCAutoRoamController`、`NPCBubblePresenter`、`NPCRoamProfile`、`NPCAutoRoamControllerEditor`、`NPC_DefaultRoamProfile.asset`、`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`。
  - 更新 `NPCAnimController` 防守性逻辑，运行时在 Animator 尚未绑定控制器时会跳过参数驱动，避免旧报错路径反复刷屏。
  - 更新 `NPCPrefabGeneratorTool` 的切片逻辑，消除 `TextureImporter.spritesheet` 废弃 warning。
- 本轮验证：
  - `Primary` 场景进入 Play 后，Console 只剩 `There are no audio listeners in the scene` 非 NPC 阻断 warning。
  - 读取 `001` / `002` / `003` 的 `NPCAutoRoamController` 单组件资源，确认 `IsRoaming=true`，并再次捕获到 NPC 附近聊天正样本。
  - 重新进入 Play 后，`Missing Script` 瞬时日志未再次复现，可判定不是当前稳定阻断。
- 遗留问题：
  - 当前真实 Git 现场仍位于 `codex/farm-1.0.2-correct001`，不是语义最优的 NPC 分支；后续若继续扩展 NPC 主线，应在提交说明里明确白名单范围。
- 恢复点：自动漫游第二阶段已恢复，下一轮可以继续做更细的 NPC 行为策划实现，或围绕生成工具做进一步压缩和验收。

## 2026-03-17

- 本轮主线：处理用户发现的 NPC 预制体与动画 `Sprite` 全部丢失问题，查清到底是哪一步把 `001/002/003` 弄坏了。
- 已证实根因：
  - `Assets/222_Prefabs/NPC/001.prefab` 以及对应动画 `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 等文件都在引用 `guid: b7b3124ae2dcef34e9e08258e832d515` 下的方向化 Sprite 子资源，例如 `001_Down_1 -> fileID 3869598651494229176`。
  - 当前现场的 `Assets/Sprites/NPC/001.png.meta` 在修复前仍是旧的自动切片版，只包含 `001_0 / 001_1 / ...` 这套子资源 ID，不包含 `001_Down_1 / 001_Left_1 / ...` 这套方向化 ID。
  - 上一轮回填时带回了 Prefab、动画和运行时代码，但漏带了 `Assets/Sprites/NPC/001.png.meta`、`002.png.meta`、`003.png.meta`，导致 Prefab 和动画同时出现 `缺失(精灵)`。
- 本轮修复：
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/001.png.meta`
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/002.png.meta`
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/003.png.meta`
  - 执行 Unity refresh，让 AssetDatabase 重新解析子 Sprite 引用。
- 本轮验证：
  - `001` 场景实例的 `SpriteRenderer.sprite` 已恢复为 `Assets/Sprites/NPC/001.png`，不再是 `null`。
  - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 与 `001_Move_Down.anim` 的 `m_PPtrCurves` 当前引用的 fileID 已与修复后的 `.meta` 对齐。
  - 当前 Unity Console 为 0 条，未再出现本轮的 `Missing Sprite` / `Missing Script` 噪音。
- 当前恢复点：NPC 的 PNG 子资源 ID 已重新和 Prefab/动画链路对齐，三张 NPC 贴图依赖已补全。

- 当前主线目标：在继续 NPC 主线前，先把“为什么这条 NPC 线程后来会落到 farm 分支上继续提交”做成可交给治理线程的写实分析。
- 本轮子任务：只读核查 NPC 线程相关的 Git 分支、worktree、reflog 与线程规则，澄清分支和 worktree 的关系并重建时间线。
- 已证实事实：
  - 当前根仓库真实现场是 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`，`HEAD = 11e0b7b4`，不是 `main`，也不是某条 `codex/npc-*` 分支。
  - 旧 NPC 独立 worktree 仍然存在于 `D:\Unity\Unity_learning\Sunset_worktrees\NPC`，其检出分支是 `codex/npc-generator-pipeline`；这说明“NPC 默认不再用独立 worktree”是规则口径，但旧 worktree 现场并未物理消失。
  - `codex/npc-fixed-template-main`、`codex/npc-asset-solidify-001`、`codex/npc-main-recover-001`、`codex/npc-roam-phase2-001` 这些 NPC 分支都是真实存在的仓库分支，不“属于”某个 worktree；只有 `codex/npc-generator-pipeline` 当前仍附着在旧 NPC worktree 上。
  - `git reflog` 显示 2026-03-17 01:35 从 `codex/farm-1.0.2-correct001` 切到 `codex/npc-roam-phase2-001`，01:44 又切回 `codex/farm-1.0.2-correct001`；随后 `07ffe199`、`18f3a9e1`、`11e0b7b4` 连续提交都发生在 `codex/farm-1.0.2-correct001`。
  - `07ffe199` 与 `18f3a9e1` 这两次提交实际承载了 NPC 二阶段代码、动画、Prefab 与 PNG meta 修复，且 `git branch --contains` 结果表明它们当前只在 `codex/farm-1.0.2-correct001` 上。
- 当前判断：
  - “为什么会回到 farm” 的主观动机无法从 Git 直接证明，但从时间线看，最合理解释是：根仓库当时已经在 farm 线工作，短暂切去 NPC 分支做恢复后，又回到了原活跃根分支，导致后续 NPC 成果提交到了 farm。
  - 我此前把“历史上存在 NPC worktree”说成了“现在 NPC 就是在那个 worktree 上”，表述失真；更准确的说法应是“规则上 NPC 现在线程默认应从根仓库 `main` 进入，但 Git 现场仍保留旧 NPC worktree，同时本轮根仓库又实际停在 farm 分支上”。
- 恢复点：NPC 这条线当前新增了完整的 Git 取证结论，下一步应由治理线程裁定如何处理“NPC 成果落在 farm 分支”与“旧 NPC worktree 残留”的收口方案。

- 当前主线目标：按治理裁定，把 NPC 的后续可写基线收敛到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再借用共享根目录 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`。
- 本轮子任务：只读确认 `f6b4db2f` 是否已经包含 NPC 核心资产 / Prefab / meta / 运行时代码，并判断 `FarmToolPreview.cs` 与 `NPCPrefabGeneratorTool.cs` 的最小收口策略。
- 已证实事实：
  - `git ls-tree -r f6b4db2f` 已包含 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/001~003.prefab`、`Assets/Sprites/NPC/001~003.png(.meta)`、`Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`NPCBubblePresenter.cs`、`NPCMotionController.cs`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs` 与 `Assets/Editor/NPCPrefabGeneratorTool.cs`。
  - `f6b4db2f` 对比 farm 上 `18f3a9e1`，NPC 范围内只剩 `Assets/Editor/NPCPrefabGeneratorTool.cs` 与 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 两个差异文件；说明 `f6b4db2f` 本身已经带齐 NPC 核心资产，不缺额外 Prefab / meta / 脚本。
  - `f6b4db2f` 对比其上游 `8aed637f`，真正额外拖入 NPC 线的 farm 侧文件只有 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - `git grep` 显示在 `f6b4db2f` 中，`GameInputManager.cs` 仍只调用无参 `ClearAllQueuePreviews()`，NPC 线内没有其他代码依赖 `FarmToolPreview.ClearAllQueuePreviews(bool)` 这组 farm 侧扩展。
- 关键判断：
  - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的这次改动不应继续留在 NPC 救援线中；最小剔除方式是把该文件回退到 `8aed637f` 版本。
  - `Assets/Editor/NPCPrefabGeneratorTool.cs` 继续保持 `f6b4db2f` 版本即可作为 NPC 救援基线，因为它已经和这条线里的 Prefab / 漫游组件 / 默认 profile 链路一致；不需要再借用 farm 上更旧的那版。
- 恢复点：NPC 现已可以以 `codex/npc-roam-phase2-001 @ f6b4db2f` 作为唯一救援基线继续收口；下一步最小动作应是在独立 NPC 可写现场里，仅剔除 `FarmToolPreview.cs` 的 farm 侧拖带改动，并做一次 Unity 编译 + `001/002/003` Prefab/动画引用验证。
