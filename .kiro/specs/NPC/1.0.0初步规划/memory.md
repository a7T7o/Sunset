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
