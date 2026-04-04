# 2026-04-04_给UI_Town中文Dialogue显示链闭环_01

你现在不要回到 `Prompt / Workbench / 任务列表` 的广义玩家面收口，也不要扩到 NPC 气泡、工作台 polish、Primary 或 Town 之外的字体底座整理。

你当前唯一主刀固定为：

- 只收 `Town` 内的中文 `DialogueUI` 基础显示链，让它达到“进入 Town 后不再缺字、不再把 continue 标签打成坏壳”的可消费基线。

## 一、先承认的 Town 基线

下面这些已经是当前 accepted baseline，不是这轮的争论点：

1. `Town.unity` 已提交在：
   - `8df1b4e0 2026.04.04_Codex规则落地_03`
2. `Town` 已是“村庄承载层”，不是 Day1 前半段剧情源。
3. `DialogueUI.cs` 已被纳入 `Town` 承接稳定化 slice。
4. 但用户最新 live 仍然坐实：
   - `Town` 内中文对话基础显示链还没闭环
   - 继续标签 / 对话按钮链仍可能落到不可渲染字体或缺字壳

## 二、你这轮只准做什么

你这轮只准围绕下面这一个问题施工：

- `Town` 进入后，`DialogueUI` 的中文基础显示链为什么还会缺字，以及怎样把它收成 live 可用

合法范围只允许：

1. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
2. `Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs`
3. `Assets/YYY_Scripts/Story/DialogueChineseFontRuntimeBootstrap.cs`
4. 必要时你 own 范围内与这条链直接相连的最小辅助点

明确禁止：

1. 不要回到 `Prompt / Workbench / 任务列表` 的大收口
2. 不要扩到 NPC 气泡或玩家面整体 polish
3. 不要碰 `Primary.unity`
4. 不要碰 `Town.unity`
5. 不要把这轮偷换成“全局 TMP / 全局字体底座重构”

## 三、完成定义

你这轮只有在下面几条同时成立时，才算命中完成定义：

1. 已解释清 `Town` 内中文缺字的真实责任点是不是 `DialogueUI` / 中文字体回退链本身。
2. 已做最小代码修复，而且没有扩题。
3. 代码闸门通过。
4. 已给出低负载 Unity 侧证据，证明：
   - `Town` 内 `continue` / 对话基础文案不再落到不可渲染字体
   - 不再留下新的中文缺字 warning
5. 如果仍有残留 blocker，必须精确说死：
   - 是不是已经不在你 own 范围
   - 对应 owner / 文件 / 类型是什么

## 四、回执要求

回我时继续先给用户可读层六点卡，再给技术审计层。  
如果你最后判断这条线还没过，不要说“差一点”；要明确写：

- 还差哪一点
- 谁来继续
- 为什么现在不能算 Town 中文显示基线健康

## 五、thread-state 接线尾巴

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
