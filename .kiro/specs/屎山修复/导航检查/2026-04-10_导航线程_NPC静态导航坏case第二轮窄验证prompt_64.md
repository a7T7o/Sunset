# 当前已接受的基线

1. 这轮不是让你直接开修，也不是让你继续打“先止血再说”的补丁。
2. 第一轮认知同步已经基本过线，当前已接受的共识只有这些：
   - 用户真正不能接受的不是“只是朝向左右翻”，而是 `NPC 没有像玩家一样把静态世界走明白`
   - 当前问题不能再被偷换成“性能和正常导航天然冲突”
   - 当前问题至少已经拆出这几层：
     - `owner / facing`
     - `free-roam 目标生成 contract`
     - `静态世界可达性 / 不可达点失败语义`
     - `封闭区 / 养殖区 roam domain`
     - `bad case -> storm / 峰值`
3. 但 `spring-day1` 这边对你的上一轮回执还有一个明确保留意见，而且这轮必须补齐：
   - 你还没有把 `clearance / avoidance / agent 参数差异` 单独核出来
   - 也就是除了 `roam target / owner / rebuild` 之外，还要查：
     - `NPC` 的避让壳、clearance、blocking 判定
     - 和 `PlayerAutoNavigator` 是否存在 caller-level contract 之外的实体参数差异
4. 用户主诉仍然不变，而且你必须继续完整继承，不要再缩小：
   - `NPC` 不该在自己 `anchor` 附近撞树、撞围栏、原地乱顶
   - 不该在封闭区、养殖区边上持续犯傻
   - 不该因为不可达坏 case 就拖成 storm
   - 玩家不会出现的静态世界坏相，`NPC` 也不该稳定出现

# 当前唯一主刀

只做 `第二轮窄验证`：

用 **两个代表性坏 case** 把下面这件事钉死：

`NPC 在 free-roam 时，第一处真正失真到底发生在：坏目标采样、walkable 解析、clearance/avoidance 契约、还是外部 owner 接管。`

这轮目标不是再讲“可能是什么”，而是把 **第一失真点** 缩到足够窄，能直接支撑下一轮第一刀真修。

# 你必须先吃回的上下文

1. [导航检查/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/memory.md)
2. [03_NPC自漫游与峰值止血/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/03_NPC自漫游与峰值止血/memory.md)
3. [2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/2026-04-10_导航线程_NPC静态导航与owner问题认知同步prompt_63.md)
4. 你自己上一轮刚交的认知同步回执

# 本轮只允许的 scope

- [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
- [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- [PlayerAutoNavigator.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
- [NavigationTraversalCore.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs)
- 必要时补看 shared avoidance / local avoidance 相关调用链
- [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
- [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
- 只读 scene / anchor / pen /养殖区现场核对
- 最小 live 读证据

# 本轮明确禁止

1. 不要改运行时代码。
2. 不要继续补 stopgap。
3. 不要把这轮重新讲成“只是朝向 owner”。
4. 不要只拿旧 profiler / 旧 binary 差异当结论。
5. 不要扩成“全局导航重构方案”。
6. 不要再交一版泛分析，必须把代表性坏 case 钉到可裁判。

# 这轮必须验证的 4 件事

请严格围绕下面 4 件事组织调查，不要漂：

1. `坏 NPC 的 roam 采样点`
   - 它抽到的目标点是不是语义上就不该去
   - 或者静态上本来就不可达
   - 至少要明确：
     - `homeAnchor`
     - `activityRadius`
     - 采样点
     - 解析后的 walkable 点
     - 最终提交给 path/traversal 的目标点

2. `walkable / rebuild / blocked recovery`
   - 从采样点进入底层前后，目标点有没有被改写
   - `TryFindNearestWalkable / TryRebuildPath / blockedAdvance / stuck`
     到底是在哪一步开始进入坏相

3. `owner 接管链`
   - 坏 case 发生时，到底是哪条链还在继续写：
     - motion
     - facing
     - rebuild
   - 必须明确：
     - 是 `auto-roam` 自己在继续
     - 还是 `crowd / director / resident / external velocity` 在中途抢写

4. `NPC vs Player 的 clearance / avoidance / blocker 契约差异`
   - 不只比较“目标来源”
   - 还要比较：
     - avoidance shell
     - clearance / blocker 判定
     - passive blocker 处理
     - 是否存在“玩家能绕开，NPC 由于 contract/参数差异反而会持续撞”的情况

# 代表性验证对象最少要有两个

至少锁下面两类对象，各选一个真实代表：

1. `Town` 里会在树 / 围栏 / anchor 附近乱顶的 `NPC`
2. `封闭区 / 养殖区 / 不该进去的区域` 边上会出问题的 `NPC` 或小动物

如果第二类最终发现不是同一条 contract，也要明确说清楚：
- 哪部分是同根
- 哪部分不是

# 你必须回答的核心问题

请在回执里把下面这 5 个问题原样答掉：

1. `这两个坏 case 的第一失真点分别在哪？`
2. `这两个坏 case 的第一失真点是不是同一个根？`
3. `如果不是同一个根，哪一个才是用户当前最值钱的第一刀？`
4. `NPC 和玩家除了目标来源不同，是否还存在 clearance / avoidance / blocker 处理差异？`
5. `下一轮真正值得开的第一刀修复，应该只修什么，不该顺手修什么？`

# 完成定义

这轮完成不看你有没有修代码，只看你有没有把问题缩到可直接开第一刀真修：

1. 你是否用真实坏 case，而不是泛口头分析
2. 你是否钉出了“第一失真点”
3. 你是否回答了 `NPC vs Player` 是否还存在参数/clearance 层差异
4. 你是否明确给出“下一轮第一刀只修什么”
5. 你是否明确区分：
   - `结构/静态推断`
   - `live 证据`

# 回执格式

先写 `用户可读层`，顺序固定：

1. 当前主线
2. 你这轮实际钉死了什么
3. 现在还没钉死什么
4. 当前阶段
5. 下一轮第一刀只该修什么
6. 需要用户现在做什么

然后写 `用户补充层`：

- 这两个代表性坏 case 各自的第一失真点
- 你为什么认为它们同根或不同根
- 你为什么认为“参数/clearance 差异”是或不是实锤

最后才写 `技术审计层`：

1. 你这轮实际查看的文件/链路
2. 你做 live 窄验证时具体看的对象
3. 最关键的 1~3 个代码落点
4. 这轮是否纯只读
5. 当前是否已经到了可发第一刀施工 prompt 的阶段

# thread-state

这轮默认只读。

- 如果你严格保持只读，不需要 `Begin-Slice`
- 如果你中途决定进真实施工，才补 `Begin-Slice`
- 如果这轮只读结束，合法停在 `PARKED`
