# 2026-04-14｜UI 线程｜Day1 最终 UI 语义与时间 owner 边界收口 prompt - 12

## 当前已接受的基线

1. 从这一轮开始，Day1 玩家可见 UI 全部归 `UI` 线程 own：
   - 任务清单
   - bridge prompt / 新增提示
   - PromptOverlay
   - dialogue 期间显隐
   - modal 层级
   - re-entry 后的 UI 重建
   - `TimeManagerDebugger +/-`
2. `spring-day1` 不再自己碰 UI 壳体，只负责提供 runtime canonical state。
3. 下面这些真逻辑不归你 own，也不要再碰：
   - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `HandleSleep()`
   - `RecoverFromInvalidEarlySleep()`
   - `CanFinalizeDayEndFromCurrentState()`
   - Day1 的 `20:00/21:00/26:00` 夜晚状态机
   - resident / actor 的 anchor / staging / release
4. 你 own 的不只是“把一个框挪好看”，而是：
   - 任务清单与次级提示的层级关系
   - 与 toolbar / 背包 / 箱子 / 工作台 / 对话框的模态秩序
   - restart / restore / re-entry 后玩家真正看到的 UI 面
   - `+ / -` 调试跳时语义不再污染 Day1 判断

## 当前唯一主刀

只收这一刀：

`把 Day1 玩家可见 UI contract 收成真正稳定、统一、不会再误导 Day1 live 判断的正式面；同时把时间 owner 边界写死，不再越权碰 Day1 真逻辑。`

## 这轮必须接住的用户要求

1. 新增提示不再自占一个漂浮位置。
2. 它要和任务清单同根。
3. 位置放在任务清单下面。
4. 它是次级提示，不准抢主任务卡的主体。
5. 它的文案不准和 footer / 主任务文本高重复。
6. 它要按统一层级被背包、箱子等模态页正确压住，不再出现“看起来被盖住但实际还在悬着”的假活状态。

## 这轮你 own 的 UI 语义

### 1. 任务清单 vs bridge prompt

正确关系必须是：

1. 任务清单 = Day1 当前唯一主任务面
2. bridge prompt / 新增提示 = 次级短时引导
3. 次级提示必须从属于任务清单，不得单独抢画面
4. 文案要避免和任务卡正文 / footer 做高重复自我复读

### 2. dialogue / modal 层级

你必须把这些状态收成统一语义：

1. 对话中，任务清单不能抢对白焦点
2. modal 打开时，任务清单 / 次级提示必须统一退让
3. modal 关闭后，按当前 Day1 canonical state 恢复
4. 不允许留下 stale alpha、stale block、stale 悬浮残影

### 3. re-entry / restart / restore

你这轮必须把 UI 重建收成 canonical 重建，而不是“延续上一帧壳体残留”。

也就是说：

1. 重新开始
2. 读档恢复
3. 跨 scene / 重回 Day1
4. 对话切拍

这些入口都必须重新按当前 phase / task / prompt / modal state 建 UI，而不是沿用 stale UI。

### 4. `TimeManagerDebugger +/-`

这条是你 own 的自收项，必须明确修回：

1. `+ / -` 回到整点跳转语义
2. 不再保留分钟
3. 跨过 `26` 时不再制造“Sleep 后再补分钟”的假语义
4. 不再把 Day1 真时间线误导成 UI 输入问题

## 这轮明确禁止的漂移

1. 不要碰 Day1 opening marker / 晚饭 5 秒等待 / resident release / `20:00/21:00/26:00` 真逻辑。
2. 不要碰导航 core。
3. 不要自己去猜 NPC / actor 的 runtime canonical state。
4. 如果缺少 Day1 runtime 信号，不要拍脑袋发明 UI 逻辑；要明确报 Day1 contract 缺口。
5. 不要把 UI 壳体问题包装成“Day1 真逻辑已经修好”。

## 这轮完成定义

1. 新增提示与任务清单同根，稳定落在任务清单下方。
2. 新增提示不再和主任务 / footer 高重复，不再自占错误焦点。
3. toolbar / 背包 / 箱子 / 工作台 / dialogue 的层级秩序统一。
4. restart / restore / re-entry 后，任务清单 / Prompt / 次级提示会按当前 Day1 canonical state 重建，而不是保留 stale 壳。
5. `TimeManagerDebugger +/-` 已收回整点跳转语义，不再继续干扰 Day1 夜间判断。
6. 你没有越权去碰 Day1 `HandleSleep / DayEnd / anchor / resident release` 真逻辑。

## 收口纪律

1. 这轮如果形成 UI own 的高风险改动，必须在本轮收成最小可回退 checkpoint。
2. 不允许把任务清单 / Prompt / modal / debugger 的半成品高风险改动继续挂在 shared root 上离场。
3. 回执里要报实：
   - 当前是否已经形成可直接 `sync / commit` 的最小 checkpoint
   - 如果还没有，exact blocker 是什么
   - 当前回退时最小应该回到哪一个 own checkpoint

## 固定回执格式

### A1 保底六点卡

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### A2 用户补充层

必须额外回答：

1. 任务清单与新增提示现在的主次关系是什么
2. 你这轮具体收掉了哪些 UI stale / modal / re-entry 问题
3. `TimeManagerDebugger +/-` 现在的正确语义是什么
4. 你没有越权接哪些 Day1 真逻辑
5. 你这轮最核心的判断
6. 最薄弱、最可能看错的点
7. 自评

### B 技术审计层

至少包含：

- 当前在改什么
- changed_paths
- 当前玩家可见 UI 入口
- exact UI contract / modal / debugger touchpoints
- code_self_check
- pre_sync_validation
- 当前是否可以直接提交到 `main`
- blocker_or_checkpoint
- 当前 own path 是否 clean
- 一句话摘要

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
