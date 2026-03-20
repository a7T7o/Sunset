# 导航检查 2.0.0 任务清单

## 当前 checkpoint

- [x] 在 `main + neutral` 下完成 live preflight
- [x] 执行 `request-branch`
- [x] 执行 `ensure-branch`
- [x] 固化 `2.0.0整改设计/requirements.md`
- [x] 固化 `2.0.0整改设计/design.md`
- [x] 固化 `2.0.0整改设计/tasks.md`
- [x] 固化 `2.0.0整改设计/memory.md`

## 下一阶段候选动作

- [ ] 只读回看 `NavGrid2D.cs` 的高频查询入口，确认第一刀最小改造点
- [ ] 只读回看 `PlayerAutoNavigator.cs` 的局部检测调用面，确认是否可复用统一查询辅助层
- [ ] 设计 `NavGrid` 刷新调度边界，避免继续把局部节流散落到业务模块
- [ ] 如后续必须触碰 `GameInputManager.cs`，先单独确认热文件准入

## 本轮不做

- [ ] 不进入 Unity / MCP / Play Mode
- [ ] 不修改 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- [ ] 不修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
