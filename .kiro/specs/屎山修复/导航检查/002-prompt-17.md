# 002-prompt-17

这轮我先把你上一轮回执和当前 shared root 现场之间的冲突钉死：

1. 我接受你上一轮只成立了一个 checkpoint：
   - 你继续把 `NavigationAvoidanceRules / NavigationLocalAvoidanceSolver` 往“更贴近 collider footprint”方向收紧了。
2. 我不接受你把这件事包装成“当前只是外部 blocker 卡住 fresh live”。
3. 因为治理线程已经主动做了 shared root refresh / compile 与 Console 复核，当前现场事实是：
   - 这次你和农田共同引用的 `_panelVelocity` 外部 blocker 已经不再成立；
   - 最新 Console 已经清到 `0` 条 `error/warning`。
4. 我还不接受你这轮对 `changed_paths` 的口径。
   - 你回执里只报了 3 个文件；
   - 但当前 shared root 里，至少这些导航热区仍在 dirty：
     - `Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
     - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
     - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
     - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

所以从现在开始：

- 你不能再拿“外部 blocker”当停车位；
- 你也不能继续少报 own dirty；
- 这轮必须把“卫生清扫 + same-round fresh real-input live”收成一个真刀闭环。

---

## 一、当前已接受的基线

当前导航线我只接受这些基线：

1. runtime 仍以 `4613255c` 对应的旧 solver 直出链为运行语义基线；
2. `TrafficArbiter / MotionCommand` 这条 runtime 接线当前仍未重新引入；
3. 用户对真实体验的否定仍然有效：
   - 保护罩感还在
   - 很远就停
   - 被围抽搐 / 鬼畜
4. `002-prompt-16` 这一轮没有拿到 1 组有效 fresh `scenario_start / scenario_end`，所以它没有命中完成定义；
5. 你当前最多只能 claim：
   - “源码继续收紧” checkpoint
   - 不能 claim “真实入口体验回正”

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 先把导航自家当前脏范围报实并清卫生，  
> 再在同一轮 fresh real-input live 里，真正回答“保护罩 / 很远就停 / 被围抽搐”现在到底还在不在。

更直白一点：

- 这轮不是继续交“静态推算”
- 这轮不是继续交“源码又收紧了一点”
- 这轮也不是继续停在“外部 blocker 阻断”

这轮先把现场说实，再把 real-input 跑实。

---

## 三、这轮先做什么，后做什么

### 第一步：先做 own hygiene

你必须先把当前 own dirty 全部对齐成真实口径：

1. 重新核对并回执你当前真实拥有的 dirty 文件；
2. 凡是这轮不需要的导航残留，先回退；
3. 凡是这轮必须保留的文件，明确说明为什么保留；
4. 不准再出现“回执只写 3 个文件，但 shared root 实际脏了 6 个热区”的情况。

### 第二步：再做 same-round fresh live

在 hygiene 收完后，同一轮 fresh 至少重跑：

1. `RealInputPlayerAvoidsMovingNpc`
2. 一个单 NPC 近身 real-input 场景
   - 专门回答“保护罩 / 很远就停”
3. 一个多 NPC 围堵 / 穿群 real-input 场景
   - 专门回答“被围抽搐 / 高频翻转”
4. `NpcAvoidsPlayer`
5. `NpcNpcCrossing`

这轮不接受“旧结果保留”。

---

## 四、这轮允许做什么

### 允许：

1. 继续留在当前旧 solver 直出链基线内解决问题；
2. 围绕这些热区继续最小修正，但必须直接服务于 real-input 坏体验：
   - `Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
3. 如果 current forward fix 证明方向错了，允许你在这轮里做 `selective rollback / selective restore / forward fix` 的真实切换；
4. 允许你补最小 real-input runner，但必须只服务于用户当前骂的三类现象；
5. 允许你用玩家脚底点 / collider footprint 边界重量“真实起挡距离”。

---

## 五、这轮明确禁止

### 不允许：

1. 不准再拿“外部 blocker”当默认停车位；
2. 不准继续少报 `changed_paths`；
3. 不准只报静态推算，不给 fresh live；
4. 不准只报 `minClearance / pushDisplacement`，却不回答肉眼保护罩感是否还在；
5. 不准回头再讲 `S2/S3/S5/S6` 大架构复盘；
6. 不准重新引入 `TrafficArbiter / MotionCommand` runtime 线；
7. 不准在 current shared root 已经能进 Play 的情况下，再交一轮“短窗触发尝试但没真正起跑”。

---

## 六、这轮完成定义

只有满足下面任一结局，这轮才算完成。

### 结局 A：own hygiene 收口 + real-input 体验真正过线

你要明确给出：

1. 当前真实 own dirty 已经报实；
2. 哪些残留已清掉，哪些文件保留为本轮必要改动；
3. fresh live 里：
   - 单 NPC 近身时不再明显有保护罩
   - 多 NPC 围堵时不再抽搐 / 高频翻转
   - 移动 NPC 场景下不再“还是老毛病”
4. `NpcAvoidsPlayer / NpcNpcCrossing` 没有引入更坏回归；
5. 当前 owned status 已达到可交接口径。

### 结局 B：先恢复到“至少不比当前用户骂的现场更差”，并压成单一剩余点

如果这轮仍来不及彻底做漂亮，也可以接受。

但前提是你必须先做到：

1. hygiene 已完成，own dirty 不再说不清；
2. 当前 real-input 现场肉眼体验已经明显优于用户这次骂的状态；
3. 剩余问题被压成单一剩余点；
4. 你明确说明：
   - 暂时弱化 / 回退了什么
   - 保住了什么
   - 剩余点精确卡在哪

如果这轮仍然没有 fresh live，或者 still 只是“源码继续收紧”，这轮不算完成。

---

## 七、live 纪律继续钉死

1. 每组 live 前先写清：
   - 这组只验证什么
   - 最多跑几次
   - 看见什么现象立刻 `Stop`
2. 拿到足够证据就立刻 `Stop`
3. 结束后必须退回 `Edit Mode`
4. 如果你自己肉眼仍然能看见：
   - 明显保护罩
   - 很远就停
   - 被围抽搐
   就不准因为某个数值好看而 claim 通过

---

## 八、下一次回执固定格式

- 当前在改什么
- 这轮是否仍只锁“真实右键入口下压掉保护罩 / 很远就停 / 被围抽搐”
- 当前运行时是否仍以 `4613255c` 对应旧 solver 直出链为基线
- 这轮实际采用的是 `selective rollback / selective restore / forward fix` 哪一条
- 当前真实 `changed_paths`
- 这轮先清掉了哪些 own hygiene；还保留了哪些必要 dirty
- 这轮 fresh 实际跑了哪几组 live
- 单 NPC 近身场景里，保护罩感是否还在；若不在，真实起挡距离大约是多少
- 多 NPC 围堵场景里，抽搐 / 高频翻转是否还在
- `推着 NPC 走 / 很远就停 / 被围抽搐` 这 3 个坏现象里，哪些已经被压掉，哪些还在
- `NpcAvoidsPlayer / NpcNpcCrossing` 当前 fresh 结果如何
- 如果仍未过线，当前新的第一责任点是什么
- live 是否都在拿到证据后立刻 `Stop`
- 当前是否已退回 `Edit Mode`
- 当前 owned git status 是否已可交接
- blocker_or_checkpoint
- 一句话摘要
