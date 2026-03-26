# 002-prompt-16

这轮我先把上一轮回执和最新用户验收之间的冲突钉死：

1. 我接受你上一轮做成的一件事：
   - 你已经把 runtime 从未收口的 `TrafficArbiter + MotionCommand` 接线撤回到旧 solver 直出链；
   - 当前运行基线优先回到 `4613255c` 对应语义。
2. 我不接受你把这件事外推成：
   - “真实点击入口下最坏回归已经压掉”
   - “现在只剩 NPC 提前停摆”
3. 因为用户最新现场复测已经直接否掉了这层外推：
   - 还是之前的毛病
   - 保护罩感还在
   - 被围时还是逆天
   - 体感上没有变成可接受

所以从现在开始，上一轮 runner 的数值不能再单独当封条。

如果用户肉眼仍然觉得：

- NPC 像有保护罩
- 很远就被挡停
- 被围时还是抽搐 / 鬼畜

那这轮就仍然是失败态。

---

## 一、当前已接受的基线

当前导航线我只接受这些基线：

1. `002-prompt-15` 只成立了一件结构性 checkpoint：
   - 当前 runtime 已回到旧 solver 直出链
   - `NavigationTrafficArbiter.cs / NavigationMotionCommand.cs` 已撤回，不再是当前运行时基线
2. 代码闸门、白名单 sync、`NavigationAvoidanceRulesTests=16/16` 这些只能证明代码截面自洽
3. 这些都不等于真实体验已过线
4. 用户最新现场结论优先级更高，而且已经很明确：
   - 之前那组坏体验没有被真正压掉
5. 所以当前正确定性不是：
   - “剩余问题已经收窄到 NPC 提前停摆”
   而是：
   - “真实点击体验仍未过线，上一轮 narrowing 还没有被用户现场接受”

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 只围绕真实右键入口下的近身包络、阻挡起效距离、以及多 NPC 围堵稳定性，  
> 真正压掉用户仍能肉眼看见的“保护罩 / 很远就停 / 被围抽搐”。

更直白一点：

- 这轮不准继续拿 `pushDisplacement=0.000` 单独讲故事
- 这轮不准继续把问题偷换成“只剩 NPC 提前停摆”
- 这轮先把用户仍然骂的那组可见坏体验压掉

如果你需要在这一轮里做取舍，优先级固定为：

1. 玩家右键接近单个 NPC 时，不再明显提前被一层“大气罩”挡住
2. 玩家右键进入多个 NPC 的围堵区时，不再原地抽搐 / 高频来回翻转
3. 玩家与移动 NPC 的真实接近过程里，不再出现“体感上还是推不过去 / 靠不近 / 一直被弹开”

---

## 三、这轮必须纠正的误区

你这轮必须先接受一个治理现实：

- 上一轮的 `minClearance / pushDisplacement` 指标，不足以证明用户体感已经过线。

所以这轮你不能继续：

1. 用旧的 live 结果外推当前体验
2. 继续沿用“同轮既有结果保留”来顶掉 fresh 复跑
3. 用中心点或宽壳层数值，掩盖“脚底接近距离仍然很大”的真实保护罩感

这轮必须把“真实看起来像什么”重新压回到第一优先级。

---

## 四、这轮允许你怎么做

### 允许：

1. 继续留在当前 `4613255c` 语义基线内解决问题
2. 围绕以下运行时热区做成套修正，只要目标仍然是“真实入口体验回正”：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
3. 如果当前动态避让壳层就是保护罩感来源，允许你直接弱化 / 收紧 / 回退它的一部分
4. 允许你新增或改造 real-input live 场景，但必须直接服务于用户当前骂的 3 个现象
5. 允许你用玩家脚底点 / collider footprint 边界来重新量“真正开始被挡住的距离”，不要再只看中心点空隙

---

## 五、这轮必须 fresh 重跑的 live

这轮不接受“上一轮结果保留”。

你必须同轮 fresh 至少跑这些：

1. `RealInputPlayerAvoidsMovingNpc`
   - 继续看真实右键接近移动 NPC
2. 一个“单 NPC 近身接近” real-input 场景
   - 如果现有 runner 没有，就自己加最小场景
   - 目标是回答“保护罩 / 很远就停”是否还在
3. 一个“多 NPC 围堵 / 穿群” real-input 场景
   - 如果现有 runner 没有，就自己加最小场景
   - 目标是回答“被围抽搐 / 鬼畜”是否还在
4. `NpcAvoidsPlayer`
5. `NpcNpcCrossing`

这轮必须是 same-round fresh，不准再拿旧轮结果顶账。

---

## 六、这轮明确禁止

### 不允许：

1. 不准重新引入 `TrafficArbiter / MotionCommand` 这条架构线
2. 不准回去继续讲 `S2/S3/S5/S6` 的大架构复盘
3. 不准再拿 synthetic probe 或旧轮结果充当这轮 live 证明
4. 不准只报 `pushDisplacement / minClearance`，却不回答肉眼保护罩感是否还在
5. 不准把问题过早缩成“只剩 NPC 提前停摆”，除非这轮 fresh real-input 已经证明用户当前抱怨的 3 个现象真的消失了
6. 不准动全局 Play 设置、Domain Reload 开关、Scene / Prefab / 包结构

---

## 七、这轮完成定义

只有满足下面任一结局，这轮才算完成。

### 结局 A：真实入口坏体验已被真正压掉

你要明确给出：

1. 这轮继续采用的运行时基线是否仍是 `4613255c` 语义
2. fresh real-input 结果里：
   - 单 NPC 近身时已不再有明显保护罩
   - 多 NPC 围堵时已不再抽搐 / 高频翻转
   - 移动 NPC 场景下玩家能自然靠近，不再体感上“还是老毛病”
3. fresh `NpcAvoidsPlayer / NpcNpcCrossing` 没有出现新的更坏回归
4. 你实际收掉了哪类动态行为或阈值，才把这组体验拉回来

### 结局 B：这轮先恢复到“至少不比用户当前骂的现场更差”

如果你判断这轮还来不及把所有导航体验做漂亮，也可以接受。

但前提是你必须先做到：

1. 当前 real-input 现场，肉眼体验已经明显优于用户这次骂的状态
2. 你明确说明：
   - 暂时弱化 / 关闭了什么激进动态行为
   - 保住了什么基线
   - 为什么现在至少不再是“病入膏肓”的状态
3. 剩余问题被压成单一剩余点

如果用户当前骂的 3 个现象仍然基本没变，这轮不算完成。

---

## 八、live 纪律继续钉死

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
   就不准因为 runner 某个数值好看而 claim 通过

---

## 九、下一次回执固定格式

- 当前在改什么
- 这轮是否仍只锁“真实右键入口下压掉保护罩 / 很远就停 / 被围抽搐”
- 当前运行时是否仍以 `4613255c` 对应旧 solver 直出链为基线
- 这轮实际采用的是 `selective rollback / selective restore / forward fix` 哪一条
- changed_paths
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
