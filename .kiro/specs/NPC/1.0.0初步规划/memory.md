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
