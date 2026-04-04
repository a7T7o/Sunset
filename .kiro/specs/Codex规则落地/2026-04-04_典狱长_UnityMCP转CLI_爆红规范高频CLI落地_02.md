# UnityMCP转CLI 续工 Prompt

请先完整读取以下文件，再继续本轮真实施工：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-04_给典狱长_UnityMCP转CLI当前阶段与产出汇报_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UnityMCP转CLI\memory_0.md`

## 当前已接受基线

- `compile-first / no-red first / Python first` 方向已接受，不需要你再回到“要不要这样做”的研究阶段。
- 当前可用 P0 已成立：
  - `baseline`
  - `status`
  - `doctor`
  - `errors`
  - `compile`
  - `no-red`
  - `recover-bridge`
- `own red / external red` 分离、changed `.cs` 过多时主动拦截、PowerShell 只留薄壳，这几条都已经是当前基线，不要回退。

## 本轮唯一主刀

把 Sunset 的“爆红规范”真正落成一条更轻量、更稳定的高频 CLI 动作：

- 每次修改代码后，都能用一条明确命令快速确认：
  1. 目标脚本 own 代码闸门是否通过
  2. Unity fresh compile 后是否仍有 red
  3. 这些 red 是 own 还是 external

这轮不要再停在“已有 `compile/no-red` 也能凑合用”的状态，而是把它收成更明确的一条日常动作。默认目标是：

- 正式补出 `validate_script`（或同等语义的一条单命令入口）

## 本轮允许的 scope

- `D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py`
- `D:\Unity\Unity_learning\Sunset\scripts\sunset-mcp.ps1`
- 与这条 CLI 直接相关的最小文档 / 线程记忆 / 工作区记忆

如果本轮没有必要，不要扩到其他系统、业务代码、Scene、Prefab、PlayMode 现场。

## 本轮明确禁止的漂移

- 不要补 `play / stop / menu / route`
- 不要回到“大而全 Unity CLI 平台”思路
- 不要把“代码闸门通过”重新包装成“Unity 红错已过”
- 不要为了这轮去做低频 live 验收体验面
- 不要继续加重 PowerShell 参与

## 这轮必须一起补上的 4 件事

### 1. 单命令入口

给出一条明确、稳定、低负载的脚本级验证命令，语义上等价于：

- `validate_script Assets/你的脚本.cs --count N`

命令结果至少要能清楚区分：

- `no_red`
- `own_red`
- `external_red`
- `unity_validation_pending` 或等价“Unity 红错验证未闭环”
- `blocked`

### 2. 资源护栏

这轮必须把资源护栏一起做进去，至少包括：

- 超时上限
- 输出上限
- Console 读取条数上限
- 不允许无限等待 / 无限刷日志

### 3. 失败回收

这轮必须考虑失败后的收口，不允许把新 CLI 做成新的资源放大器。至少要做到：

- 超时 / MCP 异常 / compile 失败后，命令能明确退出
- 不留下额外长命后台进程
- 不把 shell / 包装层残留扩散出去

### 4. shared root 防噪音延续

当前“不给路径且 changed `.cs` 过多就阻断”的保护必须保留；如果本轮做 `validate_script`，这条保护也要完整继承，不允许因为新入口又退回全仓噪音模式。

## 完成定义

本轮做完，至少要满足下面这些条件：

1. 我可以直接运行一条脚本级验证命令，例如：
   - `python D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/某脚本.cs --count 20`
2. 命令输出能明确说明：
   - own 代码闸门结果
   - own red 数
   - external red 数
   - 最终 assessment
3. 如果 Unity 当前还有 external red，必须诚实报 `external_red`，不能 claim pass
4. 如果 MCP / Unity 当前不可用，必须诚实报“Unity 红错验证未闭环”或等价状态
5. `python -m py_compile` 通过
6. `git diff --check` 通过
7. 至少做 1 次真实脚本级验证，不要只交静态说明

## 如果本轮被 external blocker 卡住

可以停车，但只能这样表述：

- 这轮脚本级验证入口已落地到什么程度
- 还差什么是 external blocker
- 当前是不是：
  - CLI 自己已可用
  - 只是现场 external red 还没清

不要把“工具已能诚实报 external red”误说成“整轮已经 no-red 完成”。

## 固定回执格式

先给用户可读汇报层，固定按这个顺序：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户 / 典狱长现在做什么

然后再给技术审计层，至少包含：

- `changed_paths`
- 新增 / 修改了哪些命令面
- 资源护栏具体补了什么
- 本轮真实验证命令
- 验证结果
- `thread-state` 报实

## 交付口径

这轮目标不是“命令更多”，而是：

- 先把 Sunset 的爆红规范真正落地成一条轻量、高频、低负载的开发动作
- 每次修改代码后，都能快速确认当前有没有 fresh compile red，以及这些红错是不是自己 own 的

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
