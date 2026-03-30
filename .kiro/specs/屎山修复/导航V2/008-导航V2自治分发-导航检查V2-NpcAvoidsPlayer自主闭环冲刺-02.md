# 008-导航V2自治分发-导航检查V2-NpcAvoidsPlayer自主闭环冲刺-02

你继续作为 `导航检查V2` 实现线程施工。

这轮不再把你当成“每改一步都要等下一条 prompt”的执行壳。
用户已明确授权：这条线接下来以你的思考为主，但前提是你必须守住同一条主刀，不得借“自主”扩成散刀施工。

## 当前已接受基线

1. 上位依据仍固定为：
   - `006-Sunset专业导航系统需求与架构设计.md`
   - `007-Sunset专业导航底座后续开发路线图.md`
   - `002-导航V2开发宪法与阶段推进纲领.md`
   - `004-导航V2接班准入与自治规约.md`
   - `005-导航V2偏差账本.md`
2. 你当前不是规范 owner，不接管全局阶段拍板；
   - 但你已经被认可为这条 runtime 切片上的强主刀。
3. 当前唯一活目标不变：
   - 真实运行里把 `NpcAvoidsPlayer` 这条 NPC 侧 release / recover 执行链打到可判定结果。
4. 当前已排掉、不要重炒的旧责任点：
   - `TickMoving()` 中 `ClearedOverrideWaypoint -> StopMotion -> linearVelocity = 0`
5. 当前冻结项不变：
   - `Primary.unity`
   - `DialogueChinese*`
   - `NPCAutoRoamControllerEditor.cs`
   - `HomeAnchor` 补口链
   - `TrafficArbiter / MotionCommand / DynamicNavigationAgent`
   - broad cleanup / broad scene hygiene / 长窗 live / 多场同轮

## 这轮唯一主刀

只做这一刀，并允许你围绕它自主连续推进到你认为在本轮内可以安全完成的全部内容：

> 围绕 NPC 侧 `TryReleaseSharedAvoidanceDetour(... rebuildPath:false) -> TryHandleSharedAvoidance() -> TickMoving()` 这条 release / recover 链，完成从责任点确认、最小补口、运行取证到本刀收口的同轮闭环。

一句人话：
- 你不需要在“补一刀之后”立刻停住等下一份 prompt；
- 你应该在同一 vertical slice 里自己把能做完的都做完；
- 但只允许在这一条 slice 里做深，不允许横向开新战场。

## 你这轮的工作方式

按下面顺序自主循环，不必每走一步都停：

1. 先继续压实当前第一责任点
2. 只对这一责任点做最小补口
3. 尝试恢复并使用 Unity 会话拿 runtime 证据
4. 基于 fresh 结果决定：
   - 继续同链小迭代
   - 还是到此收口

允许你在同一轮内做多次紧贴本刀的小迭代；
但每一次新增动作都必须直接服务于同一个问题：
`NpcAvoidsPlayer` 的 NPC 侧 release 后恢复窗口到底能不能真正成立。

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\005-导航V2偏差账本.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\007-导航V2自治分发-导航检查V2-NpcAvoidsPlayer释放恢复窗口-01.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
6. 如确属必要，允许继续只读 / 最小修改：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`

## 允许的 scope

主写面默认只允许：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\memory.md`
4. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
5. `C:\Users\aTo\.codex\memories\skill-trigger-log.md`

如果且仅如果你能给出直接证据表明当前责任点已经跨出 `NPCAutoRoamController.cs` 自身，允许你最小触碰：

6. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`

`PlayerAutoNavigator.cs` 默认只读，不默认升级为主写面。

## 自主权限

这轮你被明确授权自行决定以下事情，不必再等新 prompt：

1. 是否继续留在当前 release / recover 链
2. 是否对同一责任点做第二次最小补口
3. 是否先尝试恢复 Unity session，再跑 live
4. 是否在拿到 fresh 后继续做同链微迭代
5. 是否以“本刀已闭环”或“本刀到 blocker checkpoint 为止”收口

但你无权自行决定：

1. 改写总阶段顺序
2. 把 `P0-A` 扩成并列多刀
3. 把实现入口改给别的线程
4. 吞并 `P0-B`、`HomeAnchor`、scene / 字体 / editor 线
5. 重开大重构或已判废方向

## 这轮明确禁止

1. 不准把“自主推进”理解成自由扩刀。
2. 不准回漂 `NavigationLocalAvoidanceSolver` 泛调。
3. 不准重开 `TrafficArbiter / MotionCommand / DynamicNavigationAgent / 全量 linearVelocity`。
4. 不准碰 `Primary.unity`、字体、`NPCAutoRoamControllerEditor.cs`、`NPCV2` 线。
5. 不准把 `NpcNpcCrossing`、多场同轮、broad cleanup 混进来。
6. 不准把“Unity session 不可用”当成第一反应式结束语。
   - 你应先尝试恢复会话或切换到可用的 Unity 验证路径；
   - 只有确认短时内确实恢复不了，才把它作为真实 blocker 落盘。
7. 不准只交结构结论，不交用户可理解的结果说明。

## 你这轮真正的完成定义

你这轮结束时，必须落到下面三种结局之一，不能停在模糊中间态：

### 结局 A：通过

1. 你已在同一条 slice 内完成责任点确认与最小补口
2. 你已拿到至少 `1` 条 fresh `NpcAvoidsPlayer scenario_end`
3. 结果足以支持你明确写出：
   - `pass`
   - 或至少“当前这刀已把 failure 压窄到新的单点”
4. 你已把本刀 own 路径收回到可说明状态

### 结局 B：未过，但责任点继续压窄

1. 你已做完这轮能安全做的全部最小动作
2. 你已拿到新的 runtime 证据，或拿到足够硬的静态链路证据
3. 你把责任点继续压到比现在更小的单分支 / 单条件
4. 你能明确说明下一刀为什么还在同一条 slice 内

### 结局 C：真实外部 blocker

1. 你已先尝试恢复 Unity session 或替代验证路径
2. 你有证据证明当前 blocker 真实存在
3. 你已经把本轮可做部分做完
4. 你明确说明：
   - 哪一步被卡住
   - 为什么这不是你偷停
   - blocker 一旦解除你会从哪一行 / 哪一条 fresh 继续

## 交付要求

### 1. 聊天最小回执

只按下面格式回复：

```text
已回写文件路径：
当前在改什么：
当前第一责任点：
这轮进入了哪种结局：`A / B / C`
这轮是否已产出 fresh `scenario_end`：`yes / no`
若有 fresh：结果一句话写清
若无 fresh：你为恢复验证做了什么、为什么仍被卡住
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

### 2. 用户可读详细汇报

除最小回执外，你还必须另落一份详细汇报文件，给用户直接看懂：

1. 这轮到底改了什么功能点
2. 每个功能点为什么改
3. 你实际验证了什么
4. 用户现在可以怎么测
5. 如果还没过，失败解释是什么
6. 下一步会继续打哪一刀

要求按“功能点”组织，不要写成一团流水账。

## 一句话总口径

这轮不是再证明你会跟着 prompt 走，而是要你在同一条 `NpcAvoidsPlayer` release / recover slice 里，按自己的判断把能安全做完的内容一条龙做完；做深，不做散；做完，不漂移。
