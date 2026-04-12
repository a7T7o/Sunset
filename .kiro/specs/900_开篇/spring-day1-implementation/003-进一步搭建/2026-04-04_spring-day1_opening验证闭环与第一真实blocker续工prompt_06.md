# 2026-04-04 `spring-day1` opening 验证闭环与第一真实 blocker 续工 prompt `06`

这轮不是继续补 opening 结构，不是继续扩剧情文案，也不是继续碰 UI owner。

当前已接受的基线先写死：

- `spring-day1` 这条线当前只守 `CrashAndMeet / EnterVillage` 的非 UI opening 收口
- UI 玩家面仍归 UI 线程，不回吞
- opening 已补了专门的编辑器测试入口
- 一条残留旧口径的 opening 测试断言已回正到当前导演层真实语义
- 你上一轮已经明确报实：
  - 这轮最大缺口不是再加剧情字
  - 而是 `CLI CodexCodeGuard / dotnet` 验证链 600 秒超时，导致你拿不到 fresh 程序集级结果

## 本轮唯一主刀

把 `opening` 这条非 UI 前半段，从“结构和最小入口成立”推进到“验证闭环成立”，或者把它当前的**第一真实 blocker**钉死。

这轮只允许二选一收口：

1. 你成功拿到 fresh opening 验证证据
2. 你仍被工具卡住，但把第一真实 blocker 报到不能再含糊

不要第三种状态：

- 不要一边继续补结构，一边说验证以后再说
- 不要再继续扩 opening 内容来回避验证

## 允许的 scope

只允许碰和这把验证闭环直接相关的内容：

- opening 专项测试入口
- opening 相关 bridge / asset tests 的点跑链
- 为了让验证真正跑起来所必需的最小验证脚本、最小菜单、最小测试口径修正
- 如果验证链卡死，只允许碰“能定位第一真实 blocker”的最小探针与日志链

## 明确禁止的漂移

这轮不要进入：

- 新增剧情段落
- 扩 `HealingAndHP / DayEnd`
- 继续补 opening 文案美化
- UI / Prompt / Workbench / continue / 气泡
- NPC 玩家面体验
- `Primary.unity`
- `GameInputManager.cs`
- “顺手把别的 runtime bridge 一起补了”

## 完成定义

### 路径 A：验证闭环成立

你必须至少交出这 4 件事：

1. opening 专项入口实际点跑了哪 4 个 tests
2. 每个 test 的结果
3. opening 实际消费链的最新证据
4. 你据此给出的明确判断：
   - opening 前半段现在是“结构成立但 live 待补”
   - 还是已经到“非 UI opening 可单独验证”

如果你只跑了结构测试，没有补消费链证据，不算闭环。

### 路径 B：第一真实 blocker 成立

如果这轮还是被 `CLI CodexCodeGuard / dotnet` 卡住，你只能交：

1. 第一真实 blocker 是什么
2. 它出现在哪个命令、哪次尝试、哪个阶段
3. 你已经排除掉哪些伪 blocker
4. 为什么这个 blocker 已经真实阻断了本轮唯一主刀
5. 在这个 blocker 没解除前，opening 这条线还能不能再推进；如果不能，明确写 `不能`

不要再写：

- `验证待后续`
- `工具链还在看`
- `大概是 dotnet 慢`
- `先 park，后面再说`

这种都不算 blocker 报实。

## 结构线与体验线必须拆开

回执里强制分开回答：

- `结构线证据`
- `验证 / live 证据`

不要把“测试入口已存在”写成“opening 已可验”。

## 固定回执格式

先交用户可读层，再交技术审计层。

### A1 保底六点卡

- 当前主线
- 这轮实际做成了什么
- 现在还没做成什么
- 当前阶段
- 下一步只做什么
- 需要用户现在做什么

这 6 项必须逐项显式出现，顺序固定，不能合并。

### A2 用户补充层

只补两件事：

- 你这轮最核心的判断
- 你这轮最薄弱、最不放心的点

### B 技术审计层

- changed_paths
- 这轮实际跑了哪些命令
- 4 个 opening tests 各自结果
- `CLI CodexCodeGuard / dotnet` 是否仍超时
- 当前 own red / external blocker
- 当前 own 路径是否 clean
- 当前是否可继续推进 opening
- 一句话摘要

## thread-state

你上一轮已 `PARKED`。

如果这轮继续真实施工，就按当前 live 规则重新接回：

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
```
