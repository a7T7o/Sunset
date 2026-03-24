# 005-prompt-5-续工裁决入口与用户补充区

你先不要把这条线继续当成“无限 live 验证，慢慢总会磨出来”的问题。

当前这条导航线已经非常明确地收口到一个很小的范围了：

- `NavigationRoot`：已完成
- S4 共享路径执行层：已完成
- live Scene / 组件挂载核查：已完成
- 当前主问题：`PlayerAvoidsMovingNpc`

并且当前已接受的 live 收口状态是：

- `NpcAvoidsPlayer = pass`
- `NpcNpcCrossing = pass`
- `PlayerAvoidsMovingNpc = fail`

所以这份文件不是让你回头大扫除，也不是让你继续跑几个小时。  
它现在承担的是：

- `续工入口`
- `用户补充入口`
- `后续裁决入口`

## 一、你先读哪些材料

开始下一轮前，先按这个顺序回读：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\004-prompt-4.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\统一导航重构阶段设计与执行主表.md`
3. 本文件中的“用户补充区”

注意：

- 不要再回到结构层
- 不要再重新解释 `NavigationRoot` 是什么
- 不要再把三场 full-run 来回摆位当成主要工作方式

## 二、用户补充区

这部分是专门留给用户继续加判断、加不满、加约束的地方。  
你每次续工前，都必须把这里视为最高优先级输入之一。

### 用户最新补充 / 裁决

- 我当前对你的最大不满，不是你没有用 MCP，而是你很容易在 live 验证里跑很久，看起来像进入死循环。
- 我不接受你把“长时间反复 Play / Stop / Probe Setup / clear / get”当成默认推进方式。
- 我现在要的是：你把问题压得更具体、更短链、更可验证，而不是继续让现场看起来像你在乱飞。
- 你后续续工时，必须给我保留继续发言、继续纠偏、继续补充体验问题的位置。

### 用户可继续追加的空位

- 我当前最不满意的导航现场表现：
- 我要求你下一轮优先解释清楚的现象：
- 我认为你前面判断过快 / 过慢的地方：
- 我最不能接受你继续重复的验证方式：
- 其他新增观察：

## 三、你这轮首先要纠正的，不是结构，而是推进方式

你现在必须先纠正以下误判：

1. 不能把“还剩最后一条没过”理解成“继续长时间跑 live 就行”
2. 不能把“已经缩到 solver 嫌疑”理解成“可以继续泛泛地怀疑”
3. 不能把“用 Probe Setup 复现”理解成“就可以无边界反复跑”

你这轮应该做到的是：

- 用更短的 MCP 窗口
- 用更小的单场链路
- 把剩余 blocker 压到更具体的文件 / 方法 / 条件分支

## 四、这轮允许做什么，不允许做什么

### 允许

- 只保留单场：
  - `PlayerAvoidsMovingNpc`
- 只做最小复现：
  - `Probe Setup/Player Avoids Moving NPC`
- 只优先排查：
  - `NavigationLocalAvoidanceSolver.cs`
  - 或你能证明更具体的下一责任点
- 每次 live 窗口都要尽量短，并在结束后退回 `Edit Mode`

### 不允许

- 再回头碰结构层
- 再跑长时间 full-run
- 再让角色在多个场景里来回飞
- 再只给“怀疑链”，不给更具体的责任点

## 五、你这轮真正要回答的 5 件事

1. 现在 `PlayerAvoidsMovingNpc` 剩余的失败，到底是：
   - stationary blocker 裁决
   - sleeping blocker 裁决
   - clearance / stop distance
   - path-end 收尾
   - 还是别的更具体条件分支
2. 第一责任点现在到底压到了哪个：
   - 文件
   - 方法
   - 条件分支
3. `Primary.unity isDirty` 当前到底是什么来源
4. 你这轮是否还能保持：
   - `NpcAvoidsPlayer = pass`
   - `NpcNpcCrossing = pass`
5. 你是已经修通了最后一条，还是只是把第一责任点继续锁死了

## 六、这轮你允许交出的结果

### 结果 A：最后一条修通

如果你已经把 `PlayerAvoidsMovingNpc` 修通，而且另外两条仍保持通过：

- 不要继续顺手扩题
- 直接转阶段验收报告

### 结果 B：还没完全修通，但责任点再次收窄

也可以接受。  
但前提是你必须交出：

- 更具体的文件
- 更具体的方法
- 更具体的条件分支
- 更具体的“为什么它还没过”

不接受继续停在：

- “现在大概怀疑 solver 某一段”

## 七、本轮回执格式

下一次回执，只按这个格式回复：

- 当前是否已完整吸收“用户补充区”：
- 当前是否仍只保留单场 `PlayerAvoidsMovingNpc`：
- 当前 live 窗口是否已明显收短：
- `Primary.unity isDirty` 的来源是否已查清：
- 现在第一责任点位于哪个文件 / 方法 / 条件分支：
- `PlayerAvoidsMovingNpc` 最新真实结果：
- `NpcAvoidsPlayer` 是否仍保持通过：
- `NpcNpcCrossing` 是否仍保持通过：
- 当前剩余 blocker：
- blocker_or_checkpoint：
- 一句话摘要：

## 八、最后再钉一次

这条线现在最怕的，不是最后一条难，  
而是你因为“只差最后一点”而再次把推进方式拖成死循环。

你先把推进方式收紧，再继续打最后这条。
