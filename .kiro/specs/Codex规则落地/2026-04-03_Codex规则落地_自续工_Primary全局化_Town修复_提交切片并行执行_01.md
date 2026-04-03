# 2026-04-03_Codex规则落地_自续工_Primary全局化_Town修复_提交切片并行执行_01

你下一轮是治理主线程自己的真实施工，不再只做判断。用户已经把后续主刀明确收成三件事，而且要求你一轮内并行推进，但不能再交叉污染：

1. 把真正该全局化的管理器/注册器，从 `Primary` 抽离或统一到持久层
2. 单独把 `Town.unity` 自己的损坏引用修干净
3. 尝试提交尽可能多的内容

## 当前已接受的基线

- 当前 `farm` 不再碰 `Primary`
- 当前 `Town` 仍由你独占处理
- 用户现在不再自己占用 `Primary`，但你不能把它理解成“可以无边界乱改”；仍需按单 writer / 热文件纪律施工
- 你已经确认：
  - `Town` 当前真正的问题是 scene YAML 损坏引用，不是“少复制一点框架”
  - 真正该全局化的部分应优先走 `DontDestroyOnLoad` / 持久层 / 全局注册，而不是继续把 `Primary` 当模板手工拷场景内容

## 本轮唯一主刀

- 唯一主刀：
  - 让“跨场景应该常驻的框架”脱离 `Primary` 单场景依赖，并让 `Town` 回到可正常进入/可继续建设的健康基线
- 这轮第三项“尽量多提交”是支撑目标，不准反客为主变成一轮纯 Git 清仓

## 三件事的硬拆分

### A. 主线程本地亲自做

你本地亲自负责：

1. `Primary` / 持久层 / 全局注册这条线
2. 最终集成判断
3. 最终提交动作

这部分优先文件范围：

- `Assets/YYY_Scripts/Service/PersistentManagers.cs`
- `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
- `Assets/YYY_Scripts/Service/TimeManager.cs`
- `Assets/YYY_Scripts/Service/SeasonManager.cs`
- `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
- `Assets/000_Scenes/Primary.unity`（只有在确实无法完全脚本化/持久化时，才做最小 scene 接线）

目标不是把 `Primary` 再补成一个更肥的单场景，而是：

- 能脚本化/持久化的尽量脚本化/持久化
- 能统一进 `PersistentManagers` / 现有常驻链的，就不要继续 scene copy
- 只把“必须留在场景里”的最小锚点留在 `Primary`

### B. 子智能体 A：`Town` 修复

只允许用 `gpt-5.4`。

它的唯一写范围固定为：

- `Assets/000_Scenes/Town.unity`
- `Assets/000_Scenes/Town.unity.meta`
- `Assets/Editor/TownCameraRecoveryMenu.cs`
- `Assets/Editor/TownCameraRecoveryMenu.cs.meta`
- `scripts/scene_partial_sync_offline.py`

它的唯一目标是：

- 修掉 `Broken text PPtr`
- 修掉 `dangling Transform`
- 修掉 `Problem detected while opening the Scene file`

它不准：

- 碰 `Primary`
- 碰持久层
- 碰 `farm` 的 runtime 线
- 碰 memory / 治理文档

### C. 子智能体 B：提交切片审计

只允许用 `gpt-5.4`。

它是只读/轻审计位，不默认给它写权限。

它的任务是：

- 重新扫描当前 20 多万 dirty / untracked
- 给出“现在还能立刻白名单提交”的切片列表
- 明确哪些必须继续排除：
  - `Primary`
  - `Town`
  - 任何 hot / mixed / owner 不清目标

它输出给你的必须是：

1. 可直接提交的批次清单
2. 每批的精确 include paths
3. 为什么能提
4. 为什么还不能提的大头名单

它不准自己擅自 commit；最终提交由主线程统一执行。

## 子智能体使用纪律

- 本轮最多只开 2 个子智能体
- 模型固定 `gpt-5.4`
- 一个修 `Town`
- 一个做提交切片审计
- 不要再出现多开后忘关、长期挂在列表里的情况
- 子智能体一旦交卷且无剩余价值，立刻 `close_agent`

## 明确禁止的漂移

1. 不要把“Primary 全局化”偷换成“再从 Primary 拷一堆 scene 内容到 Town”
2. 不要把“Town 修复”偷换成“继续只做分析不动手”
3. 不要把“尽量多提交”偷换成“把 hot/mixed 大头一把梭”
4. 不要让提交切片审计子智能体自己动 `Primary/Town`
5. 不要让 `Town` 修复子智能体顺手碰 `Primary`
6. 不要在这一轮里把 `farm` 又拉回 `Primary`

## 本轮完成定义

这轮只有在下面条件尽量都达成时，才算合格：

1. 你已经明确把 `Primary` 中真正该全局化的部分迁到持久层/常驻链，或至少把这条迁移做成最小可验证切片
2. `Town` 的 scene 文件损坏引用已被修掉，至少静态上不再存在那批已知 Broken PPtr / dangling Transform
3. 你已经拿到一份新的“当前还能提交哪些内容”的批次清单，并实际提交了能安全提交的部分
4. 本轮结束时没有无价值子智能体残留

## 回执必须分三块

### A. 用户可读汇报层

按顺序逐项写：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 三刀分账层

再分开写：

1. `Primary` 全局化 / 持久层统一：做了什么、没做什么
2. `Town` 修复：做了什么、没做什么
3. 提交切片：提了什么、没提什么

### C. 技术审计层

最后再补：

1. `changed_paths`
2. 子智能体使用情况
3. `Begin-Slice / Ready-To-Sync / Park-Slice`
4. 提交 SHA / 未提交大头
5. blocker / checkpoint

## 一句话底线

这轮必须把三件事并行推进，但 ownership 必须完全切开：主线程只抓 `Primary` 全局化和最终提交，`Town` 交给一个 `gpt-5.4` worker，提交切片审计交给另一个 `gpt-5.4` worker；谁也不准越界。

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
