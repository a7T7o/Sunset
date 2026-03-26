# 002-prompt-15

这轮我先把口径彻底纠正到位：

1. 我不接受把当前导航状态继续表述成“结构 checkpoint 之后只差一点体验收口”。
2. 我现在接受的真实现场只有一句话：
   - 当前动态导航在用户真实手测里，已经比之前更差。
3. 用户最新现场结论不是情绪噪音，而是治理事实：
   - 右键时每个 NPC 像有保护罩；
   - 被 NPC 围住时玩家会原地抽搐 / 鬼畜；
   - 现在的体感甚至不如先弱化动态导航、只保留更朴素的接近行为。

所以这轮你不准再把：

- shared owner 迁移
- synthetic probe 绿灯
- detour 节制三场 fresh 通过

包装成“离真实落地已经很近”。

---

## 一、先纠正两条治理事实

### 1. `002-prompt-14` 不算有效 live 入口

当前工作区 `memory.md` 写过：

- `002-prompt-14.md` 已落盘

但磁盘现场没有这个文件。

所以从现在开始：

- 缺失的 `002-prompt-14` 不再作为有效依据继续外推；
- 这份 `002-prompt-15.md` 才是当前唯一有效续工文件。

### 2. 当前骨架连“是否真正进基线”都还没收干净

当前 live `git status` 已明确出现：

- `Assets/YYY_Scripts/Service/Navigation/NavigationTrafficArbiter.cs`
- `Assets/YYY_Scripts/Service/Navigation/NavigationMotionCommand.cs`

仍是 `??` untracked。

这不只是 Git 卫生问题，而是说明：

- 你这条线连“现在到底哪些核心代码算正式现场”都还没收清楚。

所以这轮不能一边继续宣称底座推进，一边放着核心骨架文件既未撤回、也未正式纳入。

---

## 二、当前已接受的基线

当前导航线我只接受这些基线：

1. `006/007` 的目标架构本身没有全错：
   - 动静态分层
   - 交通裁决优先
   - 最终运动语义统一
   - 玩家 / NPC 私有导航主循环最终下线
   这些方向仍成立。
2. 当前已经有一些真实结构进展：
   - `NavigationPathExecutor2D` 接住了一批 path / stuck / detour lifecycle owner
   - detour clear / recover 节制簇曾拿到三场同轮 fresh 无回归
   - real-input probe 骨架开始存在
3. 但这些基线不等于真实动态导航体验已过线。
4. 用户最新现场已经明确否定当前体感：
   - 保护罩感
   - 推着 NPC 走
   - 被围时抽搐
5. 当前 review 里指出的 3 个 P0 级结构缺口仍成立：
   - `NavigationTrafficArbiter` 仍是“先 solver，后裁决”
   - 玩家 / NPC 私有导航主循环仍未真正下线
   - `NavigationMotionCommand` 只统一命令边界，没有统一运动语义消费

所以这轮正确起点不是“继续往专业底座方向铺一层”。

而是：

> 先把真实点击入口下最糟的回归压掉，  
> 先恢复到至少不比旧基线更差的可接受体验。

---

## 三、这轮唯一主刀

### 这轮唯一主刀固定为：

> 以真实点击入口为准，把“保护罩 / 推挤 / 被围抽搐”这组最坏回归先压掉。

更直白一点：

- 这轮先不追求把 `S2/S3/S5/S6` 全部讲完；
- 这轮先把用户最痛、最直接能看到的失败压回到“至少可玩、至少不明显更差”。

如果你需要在这一轮里做取舍，优先级固定为：

1. 玩家右键接近移动 NPC 时，不再明显把 NPC 顶着走
2. 玩家被多个 NPC 围住时，不再原地抽搐 / 高频来回翻转
3. NPC 不再表现得像自带巨大保护罩，导致玩家很远就被挡停

只要这三件里还有一件明显存在，你就不准 claim 这轮通过。

---

## 四、这轮允许你怎么做

这轮我明确授权你不要再按“只补一个小 if”那种过窄节奏去拖。

### 允许：

1. 先用代码 + Git 历史找出“最后一个至少不比现在更差的行为基线”。
2. 在当前导航主线内，自主决定这轮到底走哪条恢复路径：
   - selective rollback
   - selective restore
   - 或 forward fix
3. 如果当前某段动态交通层 / 运动语义接线就是这轮最坏回归的直接来源，允许你暂时弱化、回退或撤掉它的一部分。
4. 允许你围绕以下文件做成套修正，只要目标仍然是“恢复真实点击体验”：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationTrafficArbiter.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationMotionCommand.cs`
   - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
5. 允许你把当前 untracked 的核心骨架文件做出明确处置：
   - 要么正式纳入这轮基线
   - 要么撤回到这轮不再依赖它们的状态
   - 但不允许继续悬空
6. 允许你跑真实点击入口 live：
   - 必须走 `GameInputManager` 真实入口或等价真实点击链
   - 不允许只靠 `SetDestination(...)` / `DebugMoveTo(...)` 直接喂路径

---

## 五、这轮明确禁止

### 不允许：

1. 不准再拿 synthetic probe 单独充当通过证明。
2. 不准继续宣讲大架构，却不先压掉最坏体感回归。
3. 不准把这轮漂成：
   - 又一轮 detour 节制簇微调
   - 又一轮 solver 参数泛调
   - 又一轮“先把更多 owner 迁出去再说”
4. 不准只修数字指标，肉眼仍明显有保护罩 / 推挤 / 抽搐却声称过线。
5. 不准继续放着：
   - 缺失的 prompt 文件
   - untracked 核心导航文件
   这类治理脱节不处理。
6. 不准动全局 Play 设置、Domain Reload 总开关、Scene / Prefab / 包结构。

---

## 六、这轮完成定义

只有满足下面任一结局，这轮才算完成。

### 结局 A：最坏体验回归已被压掉

你要明确给出：

1. 你选定的“恢复路径”是什么：
   - selective rollback
   - selective restore
   - 或 forward fix
2. 你认定的“最后一个至少不比现在更差的行为基线”是谁：
   - 具体 commit / 文件集合 / 逻辑状态
3. 真实点击入口 live 结果：
   - 玩家接近移动 NPC
   - 玩家被多个 NPC 围住
   - 当前最容易触发保护罩感的场景
4. 你实际压掉了哪类现象：
   - 推着 NPC 走
   - 原地抽搐
   - 很远就停
5. 当前核心骨架文件的基线处置：
   - `NavigationTrafficArbiter.cs`
   - `NavigationMotionCommand.cs`
   现在到底是正式纳入，还是已撤回

### 结局 B：本轮先恢复到“至少不比旧基线更差”

如果你判断这轮来不及把新动态交通完全修成专业体验，也可以接受。

但前提是你必须先做到：

1. 明确恢复到一个用户可接受或至少不明显更差的状态；
2. 这个状态必须经过真实点击入口 live 证明；
3. 你必须说明：
   - 暂时收掉了什么激进动态行为
   - 保住了什么底座
   - 下一刀真正该从哪个语义边界重建

如果结果还是“当前比旧基线更差”，这轮不算完成。

---

## 七、live 纪律继续钉死

1. 每次 live 前先写清：
   - 这次只验证什么
   - 最多跑几轮
   - 看到什么现象就立刻 `Stop`
2. 只要已经拿到足够证据，就立刻 `Stop`。
3. 完成后必须确认回到 `Edit Mode`。
4. 如果你自己在真实窗口里肉眼仍然能看见：
   - 明显保护罩
   - 明显推着 NPC 走
   - 明显抽搐
   就不准因为某个 runner 数值过了而 claim 通过。

---

## 八、下一次回执固定格式

- 当前在改什么
- 这轮是否仍只锁“真实点击入口下压掉最坏回归”
- 当前认定的最后可接受 / 至少不更差基线是谁
- 这轮实际采用的是 `selective rollback / selective restore / forward fix` 哪一条
- `NavigationTrafficArbiter.cs / NavigationMotionCommand.cs` 当前已正式纳入还是已撤回
- changed_paths
- 真实点击入口实际跑了哪几组 live
- 现场最明显的 3 个坏现象里，哪些已经被压掉，哪些还在
- 如果仍未过线，当前新的第一责任点是什么
- live 是否都在拿到证据后立刻 `Stop`
- 当前是否已退回 `Edit Mode`
- 当前 owned git status 是否已可交接
- blocker_or_checkpoint
- 一句话摘要
