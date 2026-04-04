# UnityMCP转CLI 续工 Prompt

请先完整读取以下文件，再继续本轮真实施工：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-04_典狱长_UnityMCP转CLI_爆红规范高频CLI落地_02.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-04_给典狱长_UnityMCP转CLI当前阶段与产出汇报_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UnityMCP转CLI\memory_0.md`

## 当前已接受基线

- `validate_script` 已经落地，不需要你再回到“要不要先做这条命令”的讨论。
- 当前已接受的能力基线：
  - `validate_script`
  - `own_red / external_red / unity_validation_pending / blocked`
  - changed `.cs` 过多时主动阻断
  - `CodeGuard` warning 不再被当成 red
  - 资源护栏、超时、输出上限、失败回收都已经进了命令面
- 当前这条线服务的是：
  - “每次改完代码后快速确认当前有没有 fresh compile red，以及这些 red 是不是自己 own 的”
- 当前这条线不服务的是：
  - 低频 live 控制面
  - 大而全 Unity CLI

## 从这一轮起的治理裁定

你现在获得的是：

- `窄边界持续续工许可`

含义是：

- 只要你始终停留在同一条纵切片里，就不需要每完成一个微步骤都先回典狱长请示；
- 你可以连续推进这条子线，直到形成下一个稳定 checkpoint 或命中明确边界。

但这份许可**不是开放式自治**。它只在下面这组条件同时成立时有效：

1. 主线仍然是 `爆红规范高频 CLI`
2. 当前唯一主刀仍然是 `manage_script / validate_script 参数面对齐` 与其强绑定的语义稳定
3. 不扩到控制命令面
4. 不准备 `sync`
5. 不触碰业务代码、Scene、Prefab、运行时管理器链

只要你越出这条边界，就必须先停下回卡。

## 本轮唯一主刀

把 Unity 原生 `manage_script / validate_script` 的参数面，与当前 `sunset_mcp.py validate_script` 的单命令入口做一次最小、稳定、低负载的对齐。

这轮真正要解决的是：

- 当前 CLI 已经可用；
- 但它和 Unity 侧旧参数面的关系还没正式定下来；
- 现在要把这个关系定死，而不是继续悬着。

## 本轮允许的 scope

- `D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py`
- `D:\Unity\Unity_learning\Sunset\scripts\sunset-mcp.ps1`
- 与这条 CLI 直接相关的最小文档 / 线程记忆 / 工作区记忆

## 本轮明确禁止的漂移

- 不要补 `play / stop / menu / route`
- 不要扩到 `doctor / baseline / recover-bridge` 之外的新控制面设计
- 不要回到“大而全 Unity CLI 平台”思路
- 不要为了这轮去碰业务代码、测试代码、Scene、Prefab
- 不要削弱现有资源护栏
- 不要把“参数面对齐”偷换成“继续做更多命令”
- 不要把 `代码闸门通过` 包装成 `Unity 红错已过`

## 本轮完成定义

这轮只接受下面两种完成结果之一：

### A. 成功接回最小必要参数面

你已确认 Unity 原生 `manage_script / validate_script` 里哪些参数是 Sunset 当前高频链真正值得接回的，并且已经把**最小必要兼容面**接到当前 CLI。

完成时至少要满足：

1. 不是泛泛“兼容更多参数”，而是明确只接回最值钱、最常用、不会破坏低负载基线的那一小组参数
2. 接回后不会绕开当前已有护栏：
   - changed `.cs` 过多阻断
   - timeout
   - wait 上限
   - 输出上限
   - `unity_validation_pending`
   - `blocked`
3. 至少做 2 次真实命令验证：
   - 1 次显式脚本路径成功链
   - 1 次边界 / 降级链

### B. 明确裁定“当前不该接回”，并把结论做实

如果你经过最小审计后确认：

- Unity 原生参数面会明显拉高复杂度
- 或会破坏当前 CLI 的低负载 / 诚实语义
- 或当前真正高频链根本不需要它

那你可以裁定“暂不接回”，但不能只停在口头判断。

完成时至少要满足：

1. 明确写清：
   - 为什么不接回
   - 哪些参数不值得接
   - 当前 CLI 的等价入口已经覆盖了什么
2. 把当前 CLI 的帮助 / 输出 / 文档口径补清楚，让下一次使用者不会再误以为“还得先找旧参数面”
3. 至少做 2 次真实命令验证，证明当前等价入口已经能稳定覆盖本轮目标

## 本轮最重要的判断标准

不要追求“像不像 Unity 原生工具”。

这轮唯一判断标准是：

- 是否更有利于 Sunset 当前的高频爆红核查
- 是否继续保持轻量、低负载、诚实分层
- 是否没有把机器上已暴露出的 shell / wrapper 膨胀问题重新带回来

## 持续续工许可的边界

从这轮开始，只要你满足下面条件，就可以在同一条子线上连续推进，不必每个微步骤都回治理位：

1. 仍只在 `sunset_mcp.py / sunset-mcp.ps1` 这组文件里工作
2. 仍只在 `validate_script / manage_script 参数面对齐 / 资源护栏 / 输出语义` 这一圈内推进
3. 没准备 `sync`
4. 没打算扩到 `play / stop / menu / route`
5. 没打算触碰业务代码、Scene、Prefab、运行时链

一旦出现下面任一情况，你必须先回卡，再等下一轮裁定：

1. 准备 `sync`
2. 需要新增控制命令
3. 需要改判当前主线
4. 参数面对齐结论会明显扩大实现范围
5. 命中新的外部 blocker，需要治理位拍板

## 固定回执格式

先给用户可读汇报层，固定按这个顺序：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户 / 典狱长现在做什么

如果你这轮选择了 `A` 或 `B` 其中一种完成结果，必须在第 2、3、4 点里把它说死，不要模糊成“做了一些参数面整理”。

然后再给技术审计层，至少包含：

- `changed_paths`
- 这轮最终走的是 `A` 还是 `B`
- 实际接回了哪些参数，或明确拒绝了哪些参数
- 资源护栏是否保持不变
- 本轮真实验证命令
- 验证结果
- `thread-state` 报实
- 当前 own 路径是否 clean

## 交付口径

这条线现在真正要做成的是：

- Sunset 内部一条高频、轻量、低负载的爆红核查 CLI

不是：

- 一个大而全的 Unity 控制台

所以你后续所有判断都必须压回这个目标，不要再漂。

## [thread-state 接线要求｜本轮强制]

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
