# NPC 与 farm 回归 Sunset/main 评估（2026-03-13）

## 1. 评估目标

本评估不再回答“worktree 是否还能继续修”，而是回答三件事：

- `NPC` 是否应回归 `Sunset/main`
- `farm-10.2.2-patch002` 是否应回归 `Sunset/main`
- 如果回归，应该如何准备；如果暂时不能直接整支回归，唯一阻断点是什么

## 2. 新默认前提

当前新的默认前提已经固定：

- 用户的 Unity 开发现场固定为 `D:\Unity\Unity_learning\Sunset`
- 用户默认只打开这一份 Unity 项目
- `NPC` 与 `farm` 不再适合作为日常默认独立 worktree 现场
- 之前的 worktree 承接修复链路保留，但只能降级为：
  - 故障修复工具链
  - 高风险隔离工具链
  - 非默认、非首选流程

## 3. 真实现场

### 3.1 根仓库 `main`

- 当前分支：`main`
- 当前本地提交：`4e478dc8`
- 当前相对远端：本地领先 `6`
- 当前根仓库存在大量无关 dirty：
  - 其他线程 memory
  - `Assets/000_Scenes/Primary.unity`
  - 多个 TMP 资源
  - `Assets/YYY_Scripts/Service/Placement/*`
- 结论：
  - 当前根仓库不适合直接做无边界合流；
  - 只能做白名单回归准备。

### 3.2 `NPC` worktree

- 路径：`D:\Unity\Unity_learning\Sunset_worktrees\NPC`
- 分支：`codex/npc-generator-pipeline`
- HEAD：`7b3bdd6c48d669b71477da52c878ba952301b013`
- 相对 `main`：`main` 独有 `41`，分支独有 `5`
- `git status --short` 为空，当前 worktree 干净
- 与 `main` 的业务差异文件主要集中在：
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - `Assets/Sprites/NPC/*`
  - `Assets/YYY_Scripts/Anim/NPC/*`
  - `Assets/YYY_Scripts/Controller/NPC/*`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- 同时混入了非业务文件：
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/tasks.md`
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/memory.md`
  - `.kiro/specs/Steering规则区优化/memory.md`
  - `.codex/threads/Sunset/Codex规则落地/memory_0.md`
- `merge-tree` 结果显示：
  - `NPC` 对 `main` 的直接整支合流冲突集中在治理/记忆文件，而不是 NPC 业务代码文件

### 3.3 `farm` worktree

- 路径：`D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
- 分支：`codex/farm-10.2.2-patch002`
- HEAD：`11b81f98b1cf87783dbe64d20befd9d667eb9819`
- 相对 `main`：`main` 独有 `40`，分支独有 `3`
- `git status --short` 为空，当前 worktree 干净
- 与 `main` 的业务差异文件主要集中在：
  - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
- 同时混入了非业务文件：
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/memory.md`
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/当前仓库Git自动同步与治理现状说明_2026-03-11.md`
  - `.kiro/specs/Steering规则区优化/memory.md`
  - `.kiro/specs/农田系统/2026.03.01/10.2.2补丁002/memory.md`
  - `.kiro/specs/农田系统/memory.md`
  - `.codex/threads/Sunset/Codex规则落地/memory_0.md`
- `merge-tree` 结果显示：
  - `farm` 对 `main` 的直接整支合流冲突也集中在治理/记忆文件；
  - `PlacementManager.cs` 与 `PlacementValidator.cs` 当前在只读合流中可被自动合并。

## 4. 彼此冲突核查

- `NPC` 与 `farm` 的业务差异文件不重叠：
  - `NPC` 主要改 NPC 生成器、NPC 动画、NPC 控制器、NPC 交互
  - `farm` 主要改放置服务 `PlacementManager` / `PlacementValidator`
- 当前两条线彼此重叠的主要是：
  - 治理文档
  - memory
  - 线程治理记录
- 结论：
  - 两条线的业务成果可以在主项目体系并行存在；
  - 当前真正阻碍“直接整支回归”的不是业务冲突，而是治理/记忆污染混入分支。

## 5. 线程承接现状

- 当前外部线程锚点仍指向 worktree：
  - `NPC.cwd = D:\Unity\Unity_learning\Sunset_worktrees\NPC`
  - `farm.cwd = \\?\D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
- 这说明：
  - 即使业务成果回归 `main`，线程承接定义仍不会自动回到 `D:\Unity\Unity_learning\Sunset`
  - 回归后还必须单独处理线程锚点回根仓库与 worktree 退役

## 6. 评估结论

### 6.1 `NPC`

- 结论：可以回归 `Sunset/main`
- 证据：
  - 当前 worktree 干净；
  - 分支独有改动集中在 NPC 业务文件；
  - 与 `farm` 的业务改动不重叠；
  - 直接整支合流的冲突集中在治理/记忆文件，不在 NPC 业务实现本身
- 当前唯一阻断点：
  - 不能直接整支 merge `codex/npc-generator-pipeline -> main`，必须先剥离分支内混入的治理/记忆提交，只回归 NPC 业务成果

### 6.2 `farm`

- 结论：可以回归 `Sunset/main`
- 证据：
  - 当前 worktree 干净；
  - 分支独有改动集中在农田放置服务文件；
  - 与 `NPC` 的业务改动不重叠；
  - `PlacementManager.cs` 与 `PlacementValidator.cs` 在当前只读合流中可被自动合并
- 当前唯一阻断点：
  - 不能直接整支 merge `codex/farm-10.2.2-patch002 -> main`，必须先剥离分支内混入的治理/记忆提交，只回归农田业务成果

## 7. 推荐合入顺序

推荐顺序：

1. 先做 `NPC` 业务成果回归
2. 再做 `farm` 业务成果回归
3. 最后统一处理线程锚点回根仓库与 worktree 退役

理由：

- `NPC` 业务面更独立，先回归可尽早验证“主项目优先”路线在 NPC 线上的落地；
- `farm` 仍涉及当前主仓库也在修改的 `Placement*` 文件，放在第二步更稳妥；
- 两条线都回归后再统一退役 worktree，能减少反复改线程锚点的次数。

## 8. 最小执行预案

### 8.1 合入前保护快照

必须先做：

- 根仓库 `main` 当前白名单快照
- `NPC` worktree 当前白名单快照
- `farm` worktree 当前白名单快照
- 当前线程承接状态快照：
  - `state_5.sqlite` 对应线程行
  - 对应 rollout 文件

### 8.2 回归方式

不走“整支 merge”，走“白名单业务成果回归”：

1. 从 `main` 建立一条临时回归执行分支
2. 只把 `NPC` 的业务文件带回主项目
3. 验证后再只把 `farm` 的业务文件带回主项目
4. 不带回治理/记忆污染文件

### 8.3 线程锚点回归

业务成果回归后，需要把：

- `NPC`
- `farm`

对应线程锚点统一改回：

- `cwd = D:\Unity\Unity_learning\Sunset`
- 对应 `git_branch = main`
- 对应 rollout 恢复语义改回主项目现场

### 8.4 worktree 退役

只有在以下前提同时满足时才退役原 worktree：

- 业务成果已在 `main` 落稳；
- 线程锚点已改回根仓库；
- 用户重新打开线程不会再漂回旧 worktree；
- 必要的备份快照已保留

## 9. 当前阶段

- 当前已经进入“回归执行准备”；
- 当前不应继续把 `NPC` / `farm` 保留为长期独立 worktree 默认线程；
- 当前也还不能把“应回归主项目”直接写成“已经安全完成回归”；
- 下一步唯一最小主动作是：
  - 进入 `NPC` 与 `farm` 的白名单业务成果回归执行预案，而不是再继续扩展热恢复修复。

## 10. 2026-03-13 执行更新

### 10.1 评估已进入执行

本评估文档自本节开始，不再停留在“可回归判断”。

当前已推进到：

- 建立 `origin/main` 基线的干净承载面：`codex/main-reflow-carrier`
- 按白名单开始回带 `NPC` 与 `farm` 的业务成果
- 准备把线程锚点统一改回 `D:\Unity\Unity_learning\Sunset@main`

### 10.2 当前唯一未变的阻断点

唯一未变的阻断点仍是：

- 不能整支 merge 独立分支；
- 只能白名单回归业务成果。

这条阻断点不会阻止执行，只会决定执行方式。
