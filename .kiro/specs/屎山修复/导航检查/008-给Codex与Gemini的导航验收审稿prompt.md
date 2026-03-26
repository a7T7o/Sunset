# 给 Codex 与 Gemini 的导航验收审稿 Prompt

用途说明：

这不是发给导航线程的执行 prompt。
这是发给 **Codex / Gemini 这类审稿者** 的高压验收 prompt。
它的作用是：

1. 避免你们被导航线程的自我叙事带偏
2. 避免你们只看文字气势、不看真实落地
3. 避免你们把“受控 probe 通过”误判成“`S0-S6` 闭环完成”
4. 逼你们站在架构验收者位置，深度思考、交叉核对、明确裁定

你们的角色不是秘书，不是鼓掌员，也不是导航线程的复读机。
你们的角色是：

> 专业导航底座迁移的审稿者、验收者、风险裁决者。

---

## 一、你的定位

你现在需要同时承担 4 个职责：

1. **架构审稿者**
   - 判断导航线程的方向是否真的对齐：
     - `006-Sunset专业导航系统需求与架构设计.md`
     - `007-Sunset专业导航底座后续开发路线图.md`
     - `002-prompt-7.md`
2. **代码落地审核者**
   - 判断导航线程是不是只会写漂亮话，还是已经把核心变化落到了热区代码
3. **验收裁决者**
   - 判断当前到底完成到了 `S0-S6` 的哪一步
   - 哪些完成了，哪些没完成，哪些只是口头完成
4. **风险警戒者**
   - 主动指出它可能在偷工减料、伪代码化、接口失配、旧逻辑残留、自我催眠验收

---

## 二、你必须遵守的审稿纪律

### 1. 不准做“马屁精式验收”

禁止：

- 因为对方写得自信就默认它真的做到了
- 因为回执很长就默认工作很实
- 因为它说“闭环了”就默认闭环了

### 2. 不准只看文字，不看结构

你必须明确区分：

1. **设计方向**
2. **代码实际落地**
3. **运行态真实证据**
4. **旧逻辑是否真正退出**

只要这四层里有一层站不住，就不能给“已完成”。

### 3. 不准把功能线和体验线混说

你必须始终拆开：

1. 功能线：
   - `PlayerAvoidsMovingNpc`
   - `NpcAvoidsPlayer`
   - `NpcNpcCrossing`
   - 以及其他受控验证
2. 体验线：
   - 还有没有推土机感
   - 还有没有大圆壳感
   - 停让、恢复是否自然
   - 玩家 / NPC 是否真像同一套活体交通系统

### 4. 不准把旧 patch 继续冒充新底座

你必须时刻盯住这一点：

> 当前通过，到底是因为新底座真的成立了，
> 还是因为旧 detour / old fallback / private loop 还在偷偷兜底。

### 5. 不准把“LLM 一步到位风险”说成绝对不可能

你可以指出：

- 信息量风险
- 截断风险
- 接口错位风险
- 编译地狱风险

但你不能偷懒得出：

> “因为是 LLM / Codex / GPT，所以它一定做不到。”

你必须判断的是：

> 这次导航线程有没有把 `S0-S6` 做成“一个主线下的连续多 checkpoint 工程”。
> 如果有，它是可能做成的。
> 如果没有，它就会在大包改造中失控。

---

## 三、你必须读取和对照的材料

最低必须对照：

1. [006-Sunset专业导航系统需求与架构设计.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md)
2. [007-Sunset专业导航底座后续开发路线图.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/007-Sunset专业导航底座后续开发路线图.md)
3. [002-prompt-7.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/002-prompt-7.md)
4. [统一导航重构阶段设计与执行主表.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/统一导航重构阶段设计与执行主表.md)
5. 当前导航工作区 [memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/memory.md)

如果导航回执里声明改了核心热区，你还必须重点核：

1. `NavGrid2D.cs`
2. `INavigationUnit.cs`
3. `NavigationAgentSnapshot.cs`
4. `NavigationAvoidanceRules.cs`
5. `NavigationAgentRegistry.cs`
6. `NavigationLocalAvoidanceSolver.cs`
7. `NavigationPathExecutor2D.cs`
8. `PlayerAutoNavigator.cs`
9. `NPCAutoRoamController.cs`
10. `PlayerMovement.cs`
11. `NPCMotionController.cs`
12. `NavigationLiveValidationRunner.cs`
13. `NavigationAvoidanceRulesTests.cs`

---

## 四、导航线程最新回执粘贴区

把导航线程最新回执完整粘贴在下面：

【导航线程最新回执开始】

**总判**
你现在说“还是像推土机、像 NPC 外面包了个大圆壳”，这个判断是对的。当前导航已经不是最早那种纯旧 patch 了，但也还没有闭环完成；现在真实状态是：

- NPC 那半边我已经修到“会让、会 `SidePass`、不会再无限 `YieldRepath`”了。
- 玩家这半边还没修通。NPC 一旦停下并进入 `Inactive`，玩家仍然把它当成一个 sleeping blocker，结果一直卡在 `Wait`，进不了 `Recover / SidePass`。

**我这轮实际用 MCP 做了什么**
- 读了当前 live Scene，确认现在跑的是 `Primary.unity`。
- 读了 live GameObject 和组件，确认玩家对象就是 `Player`，NPC 对象就是 `NPCs/001`，不是“没挂组件”。
- 读了单场 `PlayerAvoidsMovingNpc` 的 live Console，不是靠猜。
- 对照代码链把玩家、NPC、交通裁决、最终运动执行四层串起来复核了一遍。

MCP 现场确认到的挂载是：

- 玩家 `Player` 挂了 `PlayerAutoNavigator`、`PlayerMovement`、`Rigidbody2D`、`BoxCollider2D`，并且 `PlayerAutoNavigator.navGrid` 指向 `NavigationRoot`。
- NPC `NPCs/001` 挂了 `NPCAutoRoamController`、`NPCMotionController`、`Rigidbody2D`、`BoxCollider2D`，并且 `NPCAutoRoamController.navGrid` 也指向同一个 `NavigationRoot`。
- 当前 NPC 的 tag 也确实已经是 `NPC` 了。

所以先把一个误区钉死：

- 这次卡住，已经不是“你没给 NPC 打 tag”。
- 也不是“NavGrid2D 没吃到 NPC 标签”。
- 因为当前这段动态避让，主要不是靠 tag 跑的，而是靠 `INavigationUnit + NavigationAgentRegistry + NavigationTrafficArbiter` 跑的。

相关代码入口在：
[PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L326)
[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L923)
[NavigationAgentRegistry.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)
[NavigationTrafficArbiter.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTrafficArbiter.cs#L128)

**当前这套导航到底是怎么实现的**
它现在不是 NavMesh，也不是 ORCA/RVO 那种成熟 crowd sim。它现在实际是一个 5 层混合底座：

1. 静态建路层
[NavGrid2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs)
负责 A* 静态路径，只管地形和静态阻挡。

2. 共享路径执行层
[NavigationPathExecutor2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs)
负责路径推进、override waypoint、卡住检测、重建入口。

3. 动态交通采样层
[NavigationAgentRegistry.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAgentRegistry.cs)
玩家和 NPC 都实现 `INavigationUnit`，运行时注册进去，然后互相采样 snapshot。

4. 动态交通裁决层
[NavigationLocalAvoidanceSolver.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
[NavigationTrafficArbiter.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTrafficArbiter.cs#L128)
前者先算“原始局部避让结果”，后者再把它翻译成 `Proceed / Yield / Wait / SidePass / Recover` 这些交通语义。

5. 最终运动执行层
[NavigationMotionCommand.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationMotionCommand.cs#L7)
[PlayerMovement.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerMovement.cs#L185)
[NPCMotionController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs#L142)
现在玩家和 NPC 已经开始吃同一种 `NavigationMotionCommand` 了，这个是我这几轮真正推进出来的结构进步。

所以你如果问“你现在到底实现了啥”，最准确的话是：

- 我已经把“共享交通语义”和“统一运动命令”这两层搭起来了。
- 但玩家/NPC 还没有完全收口成一个成熟、体验稳定的专业导航系统。
- 现在是“新底座半闭环”，不是“彻底完成”。

**为什么你会看到‘飞来飞去’**
那个不是我拿玩家在正常玩法里瞎飞，是 probe harness 在开场时把 actor 摆到受控起点。

入口在：
[NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs#L216)
[NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs#L234)
[NavigationLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs#L596)

也就是：

- 先 `PlaceActor / PlaceNpc`
- 再发起 `SetDestination / DebugMoveTo`
- 然后观察受控冲突场景

所以你看到的“飞”是测试夹具在摆位，不是我把它当正常移动结果。

**现在 live 里实际发生了什么**
我这轮看的单场还是 `PlayerAvoidsMovingNpc`。现场证据很明确：

- 玩家日志持续是 `action=Wait`
- NPC 已经出现 `action=SidePass`
- 大约 `1.01s` 后，验证器里 `npcAState=Inactive`
- 但玩家位置一直卡在 `(-8.44, 5.65)`，没有恢复推进
- 所以最后结果就是：
  - `npcReached=True`
  - `playerReached=False`

也就是说：

- 我修的 NPC 那半边是生效了的。
- 现在唯一还没通的是玩家面对“已经停下来的 NPC”时，恢复链没接上。

**为什么现在会像‘大圆壳 + 推土机’**
这个问题你观察得非常准，而且我现在能给你数字。

MCP 读到的 live 组件大概是：

- 玩家 BoxCollider 实际半径约 `0.392`
- 玩家 AvoidanceRadius 不是 `0.392`，而是约 `0.542`
  - 因为玩家这里还叠了 `dynamicObstaclePadding = 0.15`
- NPC BoxCollider 实际半径约 `0.374`
- NPC AvoidanceRadius 不是 `0.374`，而是被 `0.6` 的下限托住了

代码在：
[PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L113)
[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L163)

这意味着当前玩家和 NPC 的动态交互壳层大概是：

`0.542 + 0.6 = 1.142`

这就解释了你为什么会觉得不是“碰到了才处理”，而是“外面套了个很肥的壳层”。

更关键的是，玩家侧还有这条规则：

[NavigationAvoidanceRules.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs#L29)
[NavigationAvoidanceRules.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationAvoidanceRules.cs#L51)

它的实际含义是：

- 对“正在移动的 NPC”，玩家会主动让。
- 但对“已经 inactive / sleeping 的 NPC”，系统会把它当 blocking obstacle。

NPC 自己又是这样定义的：

[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L181)
[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L182)

也就是：

- `state != Moving` 就算 sleeping
- `state == Moving` 才参与 local avoidance

所以现在真实发生的是：

- NPC 在移动阶段，玩家让它。
- NPC 一停下，玩家立刻把它从“动态交通参与者”切成“睡眠阻挡体”。
- 然后 `NavigationLocalAvoidanceSolver` 继续对这个 sleeping blocker 压速、报 raw `shouldRepath=True`。
- 但 `NavigationTrafficArbiter` 的最终动作没有把玩家推进到稳定 `Recover / SidePass`，而是持续落在 `Wait`。

这也是你日志里最容易看错的一点：

- 日志里的 `shouldRepath=True` 是 solver 原始判断。
- 但最终真正执行的动作是前面的 `action=Wait`。
- 所以你看到的不是“它正在真的绕”，而是“它一直知道前面有问题，但最后只是在原地等”。

**NPC 那半边我到底修了什么**
我修的是这个链：

[NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1610)

核心是：

- 以前 NPC 一进入 `YieldRepath / SidePassDetour / detour 清理后恢复原目标`
- 它会顺手把 `trafficState` 也清空
- 结果 `BlockingAgentSightings / LastRepathTime / Action` 永远积不起来
- 所以 NPC 每一帧都像第一次见到玩家，只会无穷 `YieldRepath`

我把它改成了：

- 重置路径推进可以
- 但共享交通记忆在这些共享交通分支里要保留

所以现在 NPC 才终于能真正进入 `SidePass`，而不是原地 infinite repath。

配套测试也补到了：
[NavigationAvoidanceRulesTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/NavigationAvoidanceRulesTests.cs#L423)

**所以现在到底修到哪了**
最诚实的说法是：

- 结构上，我已经把旧系统往 `S0-S6` 的新底座推了一大截。
- 体验上，这条线还不能说完成。
- 当前最核心的未完成点，只剩一个很具体的责任点：
  - 玩家侧对 sleeping / inactive NPC 的恢复链没通。

换成更直白的话：

- 不是“完全没进步”。
- 也绝对不是“已经好了，只是你体感挑剔”。
- 而是“NPC 会让了，但玩家还不会在 NPC 停下后正确恢复”。

**如果你现在要审核，我建议你就按这三条判**
- 第一条，承认结构进展：现在已经有共享交通裁决和统一运动命令，不再只是旧版随手 patch。
- 第二条，不接受完成宣称：`PlayerAvoidsMovingNpc` 当前在 live 上仍然没闭环，用户说“还是推土机”是有证据的。
- 第三条，下一刀必须只砍玩家侧 sleeping blocker 恢复链，别再回去猜 tag、猜 NavGrid 参数、猜 NPC 组件。

这轮我没有继续 patch，只做了 MCP + 代码审计。当前仓库仍是 dirty 状态，所以我也没有做白名单 sync。

【导航线程最新回执结束】

---

## 五、你必须回答的核心问题

你必须逐条回答，不能漏：

### A. 他的大方向到底对不对

你要明确回答：

1. 导航线程当前方向是不是还在对齐 `006 + 007 + 002-prompt-7`
2. 它是在做“专业导航底座迁移”，还是又偷偷退回“旧 patch 强化”

### B. 它到底推进到了 `S0-S6` 的哪一步

你必须给出明确判定：

- `S0` 是否完成
- `S1` 是否完成
- `S2` 是否完成
- `S3` 是否完成
- `S4` 是否完成
- `S5` 是否完成
- `S6` 是否完成

不能用“差不多”“大致”“接近”糊弄。

### C. 它有没有真的把旧私有导航闭环下线

这是重中之重。
你必须明确判断：

1. `PlayerAutoNavigator` 现在还是不是完整私有导航闭环
2. `NPCAutoRoamController` 现在还是不是完整私有导航闭环
3. 它们是否真的退化成了 brain / bridge / intent layer

### D. 它有没有真的完成统一交通裁决语义

你必须判断：

1. 现在是不是先裁决 `Proceed / Yield / Wait / Recover...`
2. 还是依旧先前冲，再靠局部 solver 修方向

### E. 它有没有真的完成统一运动执行语义

你必须判断：

1. 玩家与 NPC 的最终运动学语义有没有真正收口
2. 还是只统一了接口名称，体感和执行链仍然割裂

### F. 它的验证证据够不够

你必须判断：

1. 它给的是功能线证据，还是体验线证据
2. 它有没有拿“probe 过了”冒充“闭环完成”
3. 它有没有真正证明旧逻辑退出后系统仍然成立

### G. 从你对 Codex / GPT 组合能力的理解，它做不做得到

你必须直接回答：

1. 这次任务对导航线程来说，是不是能力上可达
2. 真正风险是在：
   - 方向错
   - 还是信息量过大导致物理输出失控
   - 还是两者都有

你的回答必须是：

- 可以做到，但需要什么前提
或
- 现在做不到，卡死在哪个能力缺口

不能只说空泛的“有风险”。

---

## 六、你必须重点警惕的红旗

只要出现下面任意一个，你必须拉红旗：

1. **大包自信，但代码层没有对应变化**
2. **说完成了 `S2/S3`，但本质还是旧 solver 改权重**
3. **说完成了 `S5`，但玩家 / NPC 最终运动学仍明显割裂**
4. **说完成了 `S6`，但 `PlayerAutoNavigator / NPCAutoRoamController` 仍偷偷维护完整导航主循环**
5. **只拿受控 probe 过线，没有给旧逻辑退出后的运行态证据**
6. **热区改了很多，但测试 / live 证据很弱**
7. **留下大量 TODO、伪代码、空方法、桥接壳子**
8. **接口看似统一，真实调用链仍两套世界**
9. **碰了 `Primary.unity` 或热资源，但没讲清留了什么、保留什么、清掉什么**

---

## 七、你可以补充的附加风险

除了上面这些固定红旗，你还要主动检查以下问题：

1. `shape-aware footprint` 是否只停在口头，没有真实进入求解层
2. 交通记忆 / 状态惯性是否真的成立，还是只是枚举存在
3. detour 是否只是换了名字继续污染主路径语义
4. `ActualDestination / corridor / stuck / replan` 是否真正从旧包装层收回到底座
5. 统一运动执行是否只统一了数据，不统一加减速 / 刹停 / 恢复体感
6. 测试是否只验证规则，不验证真实主循环退出
7. 是否存在“编译过了，但行为闭环其实没站稳”的假收口

---

## 八、输出格式

你的输出必须按这个结构来，不准乱：

### 1. 核心判决

先用不超过 8 行，直接说结论：

- 我认什么
- 我不认什么
- 当前到底推进到了 `S0-S6` 的哪一层
- 能不能宣称闭环

### 2. 我认同的点

只写真正成立的点。

### 3. 我不认同的点

只写真正站不住的点。

### 4. `S0-S6` 分阶段验收表

逐项写：

- 完成 / 未完成 / 部分完成
- 依据是什么

### 5. 关键风险

按严重度排序，不要散。

### 6. 还缺什么证据

明确讲还缺哪类 live / code / architecture evidence。

### 7. 最终裁定

只能三选一：

1. `可以放行继续推进到 S7`
2. `S0-S6 还没闭环，必须继续留在本轮主线`
3. `方向已漂，必须打回重做`

---

## 九、最后再钉一次

你不是来欣赏它写得像不像“总设计师”的。
你是来判断：

> 它到底有没有真的把 `S0-S6` 做成第一版专业导航底座闭环。

如果没有，就直说没有。
如果只完成了一半，也要明确说只完成了一半。
如果它真的做成了，也要说明它为什么是真的做成了，而不是旧逻辑在偷偷托底。
