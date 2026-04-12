# 2026-04-04 UI线程｜缺字链 fresh live 与第一真实 blocker 续工 prompt

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
2. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\memory_0.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_剧情源协同开发提醒_03.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_继续施工引导prompt_04.md`

这轮不要回成“我会继续盯 UI/UE 全链”。

这轮也不要再泛收：

1. `多悬浮框 3x2`
2. `全部 Workbench polish`
3. `气泡全量层级`
4. `更大范围交互矩阵`

除非它们直接阻断本轮唯一主刀。

---

## 当前已接受基线

下面这些已经算你这条线当前 accepted checkpoint，不要回头重打：

1. `Workbench` 的“领取 / 取消”已经开始从错误的制作按钮收回到制作条本体。
2. `Prompt / Workbench` 已补过“真正写入文本后，再做一次字体/alpha/mesh 可读性兜底”。
3. `spring-day1` 剧情源仍可能继续变长，但不由你接回剧情 owner。
4. 你当前真正最像主矛盾的，不再是“根本没数据”，而是“玩家面仍可能出现有组件没字的假活状态”。

---

## 这轮唯一主刀

只做一件事：

把 `缺字链` 打到 **fresh live 证据**，或者第一真实 blocker 报实。

翻成人话就是：

不要再先讲很多代码层推进，
而是把下面 4 个玩家真能看见的 case 逐个跑实：

1. 开局左侧任务栏
2. 中间任务卡
3. 村长对话右下角 `继续`
4. Workbench 左列 recipe

---

## 本轮唯一强绑定支撑目标

如果上面 4 个 case 里仍有任意一个还会“有框无字 / 能点无字 / 中文不显示”，
你这轮只允许沿着 **那一条文本链** 继续最小续修：

1. 运行时数据注入链
2. runtime 坏壳复用链
3. prefab 壳绑定链
4. 文本写入后字体可读性校正链

不要顺手把别的 backlog 一起吞掉。

---

## 允许 scope

这轮允许触碰：

1. `Assets/YYY_Scripts/Story/UI/*`
2. `Assets/222_Prefabs/UI/Spring-day1/*`
3. 为上面 4 个 case 所必需的最小 UI own 验证/恢复代码

---

## 明确禁止

1. 不要碰 `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
2. 不要碰 `Assets/111_Data/Story/Dialogue/*`
3. 不要碰 `Primary.unity`
4. 不要碰 `GameInputManager.cs`
5. 不要回到 “Workbench 全套 polish / 悬浮框 3x2 / 气泡总整合” 这种大散切片
6. 不要把“代码层 clean”包装成“玩家体验已过线”

---

## 和 spring-day1 的协同口径

你这轮继续默认：

1. `spring-day1` 会继续改早期开场剧情源长度与节奏
2. 但不会接回 UI owner
3. 所以你现在要做的是把显示层做成 **能承受增长**，不是围当前字串打一刀死补丁

如果你最后发现真缺的是剧情状态，而不是 UI 自己的显示链，
只允许报第一真实 blocker，精确写清：

1. 缺哪个字段
2. 缺哪个状态
3. 缺哪个事件
4. 缺哪个阶段标签

不要顺手去改剧情 owner。

---

## 完成定义

只有下面二选一成立，这轮才算合格：

### A. fresh live 证据成立

你拿到新的玩家面 / GameView / live 证据，能明确说明下面 4 个 case 各自结果：

1. 开局左侧任务栏：有字 / 仍无字
2. 中间任务卡：有字 / 仍无字
3. 村长 `继续`：有字 / 仍无字
4. Workbench 左列 recipe：有字 / 仍无字

并且如果通过，要明确写：

- `玩家面 fresh live 已确认`

### B. 第一真实 blocker 报实

如果 fresh live 仍没压住，
那你就不要继续扩 backlog，
只报第一真实 blocker：

1. 卡在哪个 case
2. 卡在 UI 自己哪条链
3. 为什么当前还不能 claim live 通过
4. 下一刀最小只该切哪一刀

---

## 强制拆开“结构证据”和“体验证据”

这轮回执里必须分开讲：

1. `结构层`
   - 改了哪条文本链
   - 为什么认为根因更接近了
2. `体验层`
   - 这 4 个 case 玩家面到底有没有真显示出来

如果只有结构，没有 fresh live，
只能写：

- `结构成立，live 待验证`

不能再偷写成“已修好”。

---

## 固定回执格式

先给用户可读层：

1. 当前主线：
2. 这轮实际做成了什么：
3. 现在还没做成什么：
4. 当前阶段：
5. 下一步只做什么：
6. 需要我现在做什么（没有就写无）：

然后补一个 `缺字链 live 矩阵`：

1. 开局左侧任务栏：`PASS / FAIL / 待验证`
2. 中间任务卡：`PASS / FAIL / 待验证`
3. 村长 continue：`PASS / FAIL / 待验证`
4. Workbench 左列 recipe：`PASS / FAIL / 待验证`

最后再补技术审计层：

- changed_paths：
- fresh live 证据：
- 代码层 no-red：
- 当前 owned errors：
- 当前 external blockers：
- 当前 own 路径是否 clean：

---

## thread-state

如果你这轮继续真实施工：

1. 沿用当前 `ACTIVE` slice 继续
2. 第一次准备 sync 前跑 `Ready-To-Sync`
3. 如果这轮先停、卡住、让位或不继续，跑 `Park-Slice`

回执里额外补：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 最后一句话

你这轮不是继续“泛修 UI”，而是只把 `缺字链` 这 4 个玩家面 case 跑实：

要么 fresh live 证明它们真好了，
要么第一真实 blocker 报实，
不要再把更大的 backlog 一起拖进来。
