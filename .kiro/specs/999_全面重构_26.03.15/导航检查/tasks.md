# 导航检查 2.0.0 任务清单

## 阶段目标

把导航线程从 `1.0.0` 审计基线推进到“可执行整改”的状态，同时尽量压低 shared root、热文件和 Unity 热区风险。

## 任务状态约定

- `[x]` 已完成
- `[ ]` 待执行
- `暂停` 当前明确不在本 checkpoint 内执行

## 本次 checkpoint

### T0：阶段二准入与 docs-first checkpoint

- [x] 在 `main + clean + occupancy neutral` 下完成 live preflight
- [x] 执行 `grant-branch -OwnerThread 导航检查 -BranchName codex/navigation-audit-001`
- [x] 执行 `ensure-branch -OwnerThread 导航检查 -BranchName codex/navigation-audit-001`
- [x] 创建 `requirements.md`
- [x] 创建 `design.md`
- [x] 创建 `tasks.md`
- [x] 在回收卡中记录阶段二 checkpoint 结果

验收点：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\requirements.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\design.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\tasks.md`

## 后续整改任务

### T1：列清高频分配调用点

- [ ] 梳理 `NavGrid2D`、`PlayerAutoNavigator`、`GameInputManager` 中所有 `OverlapCircleAll / OverlapPointAll` 调用点
- [ ] 标注每个调用点的频率、是否处于主链、是否可立即替换为 NonAlloc 路径
- [ ] 产出一份“先改什么、后改什么”的调用点清单

验收点：

- 调用点清单能区分：
  - 网格阻挡探测
  - 执行器近障查询
  - 输入层点击筛选

### T2：设计并落 `NavGrid2D` 查询辅助层

- [ ] 为网格层定义统一的非分配阻挡探测接口
- [ ] 明确缓存数组生命周期与容量策略
- [ ] 保证现有 `ClosestPoint` / `IInteractable` 语义不受影响

验收点：

- `NavGrid2D` 不再把分配式查询作为默认高频路径
- 设计能被 `PlayerAutoNavigator` 复用

### T3：收紧 `PlayerAutoNavigator` 的查询职责

- [ ] 把执行器中与网格可共享的查询收回到统一辅助层
- [ ] 保持路径平滑、停距计算与 `ClosestPoint` 行为不回退
- [ ] 重新确认 `HasLineOfSight`、近障分析和卡住诊断的边界

验收点：

- 执行器不再散落重复的高频分配查询
- 当前导航体验语义不因整改而倒退

### T4：规划 `GameInputManager` 的桥接收缩

- [ ] 先把导航相关查询与命中筛选边界整理成可替换块
- [ ] 再决定是否需要真实修改 `GameInputManager.cs`
- [ ] 如果要改，先确认热文件准入条件

验收点：

- 有一份明确的输入桥接整改切口
- 未经批准不直接扩大成整文件拆分

### T5：设计统一刷新调度器

- [ ] 梳理树、箱子、放置链当前如何请求刷新
- [ ] 定义统一调度入口、合并窗口和真实执行者
- [ ] 决定它应落在 `NavGrid2D` 内部还是独立服务

验收点：

- 能说清：
  - 谁发请求
  - 谁合并
  - 谁真正执行重建

### T6：修正文档漂移

- [ ] 回写 `chest-interaction.md` 的导航目标点表述
- [ ] 必要时补充当前 `ClosestPoint` 统一语义
- [ ] 避免旧工作区继续散布“已 Zero GC”的误导性口径

验收点：

- 文档表述与 live 实现一致

## 暂停项

### 多层级支持

- 暂停

原因：

- 这是旧导航重构工作区的长期目标，但不是当前 live 导航主链的第一阻断点。

### Unity / Play Mode 验证

- 暂停

原因：

- 当前 checkpoint 是 docs-first，不是验收轮。

### `Primary.unity` 场景改动

- 暂停

原因：

- 当前不需要碰共享热区。

## 推荐执行顺序

1. `T1` 高频分配清单
2. `T2` NavGrid 查询辅助层
3. `T3` PlayerAutoNavigator 收口
4. `T4` GameInputManager 桥接收缩
5. `T5` 刷新调度器
6. `T6` 文档回写

## 本轮结论

`2.0.0` 已经从“还没立项的想法”推进到“有 requirements / design / tasks 的可执行整改阶段”。  
下一轮若继续推进，应优先进入 `T1-T3`，并尽量在碰 `GameInputManager.cs` 之前先拿到更低冲突的实证收益。
