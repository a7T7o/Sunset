# 导航检查 2.0.0 任务清单

## 当前 checkpoint

- [x] 在 `main + neutral` 下完成 live preflight
- [x] 执行 `request-branch`
- [x] 执行 `ensure-branch`
- [x] 固化 `2.0.0整改设计/requirements.md`
- [x] 固化 `2.0.0整改设计/design.md`
- [x] 固化 `2.0.0整改设计/tasks.md`
- [x] 固化 `2.0.0整改设计/memory.md`
- [x] 将 `codex/navigation-audit-001` 中仍有价值的 `2.0.0整改设计` 迁回 `main`
- [x] 判废旧分支上根层 docs-first 垫片，不再把它们当成默认 blocker
- [x] 在 `NavGrid2D.cs` 落下首个非热文件代码 checkpoint，收掉分配式阻挡查询
- [x] 在 `PlayerAutoNavigator.cs` 落下首个非热文件代码 checkpoint，补上移动 NPC / 动态导航单元的局部绕行与重规划入口

## 下一阶段候选动作

- [ ] 用 Unity / MCP / 实机操作验证“玩家自动导航绕移动 NPC”是否稳定成立
- [ ] 继续补 `NPC/NPC` 局部规避与让行的真实接线，而不是只停在玩家侧入口
- [ ] 设计 `NavGrid` 刷新调度边界，避免继续把局部节流散落到业务模块
- [ ] 如后续必须触碰 `GameInputManager.cs`，先单独确认热文件准入

## 本轮不做

- [ ] 不进入 Unity / MCP / Play Mode
- [ ] 不修改 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- [ ] 不修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
