# 阶段任务：共享根仓库分支漂移与现场占用治理
## 阶段目标
- 把 `NPC/farm` 混线事故从“口头混乱”收成可验证、可执行、可纳入规则的治理结论。

## 已完成
- [x] 核验共享根目录当前真实分支、`HEAD` 与 worktree 结构。
- [x] 核验关键 checkout 轨迹，确认 `main -> farm -> npc -> farm` 的漂移事实。
- [x] 核验关键提交归属，确认 `07ffe199` 为 `mixed`、`18f3a9e1` 为 `NPC`、`11e0b7b4` 为 `farm`。
- [x] 吸收 `farm` 证词，确认 cleanroom 起点与 farm-only 文件边界。
- [x] 吸收 `NPC` 证词，确认唯一救援基线与不应继续挂在 NPC 线的 farm 侧改动。
- [x] 形成共享根目录冻结、NPC 救援、farm cleanroom 的临时裁定。
- [x] 产出可直接下发给 `NPC/farm` 的执行令草稿。
- [x] 收到 `NPC` 的执行级回报，确认：
  - `f6b4db2f` 不缺 NPC 核心文件
  - 只需将 `FarmToolPreview.cs` 回退到 `8aed637f`
  - 最小验证口径已明确
- [x] 收到 `farm` 的执行级回报，确认：
  - cleanroom 起点仍为 `b9b6ac48`
  - farm-only 代码与正文文档清单已锁定
  - 最小回放顺序已锁定
- [x] 向 `NPC` 下发正式执行令，并收到独立 rescue worktree 回报：
  - `FarmToolPreview.cs` 已从 NPC 线剔除
  - 编译通过
  - `001/002/003` Prefab / Sprite / 动画 / roamProfile 抽查通过
- [x] 向 `farm` 下发正式执行令，并收到 cleanroom 第一轮回报：
  - 证明 cleanroom 边界方向正确
  - 同时暴露原白名单配方缺少 `11e0b7b4` 版 `FarmToolPreview.cs`
- [x] 完成 `farm` 第二轮 cleanroom 修正验证：
  - `FarmToolPreview.cs` 已对齐 `11e0b7b4` 口径
  - batchmode 编译通过
  - `codex/farm-1.0.2-cleanroom001 @ 66c19fa1` 已达到可接替污染分支状态
- [x] 明确 `NPC` rescue worktree 中 4 个 TMP 字体资源 dirty 的归属：
  - 不属于 NPC
  - 属于 `spring-day1`

## 待办
- [ ] 向 `NPC` 下发 continuation 执行令：
  - 当前已完成 `codex/npc-roam-phase2-002` continuation worktree 创建
  - 下一步改为：禁止继续把该 worktree 当长期常态现场，准备后续回归“共享根目录 + 分支”模型
- [ ] 向 `spring-day1` 下发所有权 / 续航执行令：
  - 核对其当前真实工作目录 / 分支 / HEAD
  - 明确是否认领 4 个 TMP 字体资源与相关 UI / 场景尾巴
  - 决定如何在“不新增长期 worktree”的前提下回归 branch-only 常态
- [ ] 把“共享根目录占用校验”并入 `09` 阶段的强制 skills 闸门设计。
- [ ] 明确共享根目录的正式治理名词与字段：
  - `root-workdir lease`
  - 或 `branch occupancy`
- [ ] 决定这层占用状态的落盘位置：
  - 新锁文件
  - 共享状态表
  - 或启动闸门只读校验规则
- [ ] 设计进入 Sunset 实质性任务前的最小强制核验：
  - 当前工作目录
  - 当前分支
  - 当前 `HEAD`
  - 当前目录 owner
- [ ] 设计并执行“临时 worktree 退役计划”：
  - `farm-1.0.2-cleanroom001`
  - `NPC_roam_phase2_rescue`
  - `NPC_roam_phase2_continue`
  - 以及可能新增的 `spring-day1` 隔离现场一律不得变成长期常态
- [ ] 明确最终目标状态：
  - 共享根目录恢复到 `main`
  - 后续默认开发重新回到“根目录 + 分支”
  - worktree 仅保留为事故隔离或特殊实验的例外机制
- [ ] 在 `NPC` continuation 与 `spring-day1` 所有权归位完成后，再决定共享根目录何时释放回 `main`。

## 暂缓
- [ ] 暂不直接对污染分支做历史重写。
- [ ] 暂不删除旧 NPC worktree。
- [ ] 暂不让任何业务线程继续把共享根目录当作可写业务现场。

## 完成标准
- `NPC` 已完成救援线最小清理与验证，且其后续开发入口已经回归 branch-only 常态，而不是长期停留在 continuation worktree。
- `farm` 已在 cleanroom 新现场完成白名单回放并通过编译。
- `spring-day1` 已明确认领并收口自己占用的 TMP / UI 相关尾巴。
- `farm` 已确认 cleanroom 回放边界与起点 commit，并以 cleanroom 作为后续唯一合法承载面。
- 临时 worktree 已完成退役或降级，项目重新回到“共享根目录 + 分支”为默认模型。
- 启动闸门设计已正式吸收“共享根目录占用校验”。
- 共享根目录重新回到可说明、可校验、可释放的中性状态。
