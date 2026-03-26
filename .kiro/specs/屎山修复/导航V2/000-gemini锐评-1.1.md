# 《Sunset 导航底座 V2 架构演进与核心控制流重塑指南》

**文档定位**：本指南作为 V2 导航重构线程的最高认知输入，是对 [006-Sunset专业导航系统需求与架构设计.md] 与 [007-Sunset专业导航底座后续开发路线图.md] 在**微观控制流与代码状态机层面**的核心补全与硬性约束。

**阅读对象**：接手 Sunset 导航系统的 V2 代码编写智能体（Agent）。

---

## 1. V1 架构的病理诊断总结（为何必须重塑）

你的前任（V1 线程）留下了 `006` 和 `007` 的优秀蓝图，但在落地时犯下了极其致命的“控制流崩坏”错误。作为 V2 的接手者，你必须深刻理解以下病灶，并在你的代码中彻底根除：

1.  **虚假的交通裁决（上帝类未死）**：V1 虽然抽离了 `NavigationLocalAvoidanceSolver`，但它只是一个无状态的数学计算器。`PlayerAutoNavigator` 和 `NPCAutoRoamController` 依然是“上帝类（God Class）”，它们拿到避让结果后，甚至会因为“Cooldown（冷却时间）未到”而直接 `return false` 丢弃避让意图。这是典型的权力倒挂。
2.  **Detour（绕行点）污染主路径**：V1 试图将微观动态避让生成的临时绕行点，强行塞进宏观静态 A* 寻路的 `Path` 数组中，导致一遇到动态障碍就破坏全局寻路状态，引发无限循环的“清理-重建”风暴。
3.  **单帧失忆症（缺乏状态惯性）**：V1 的避让决策完全依赖单帧的 Raycast/Overlap。只要有一帧视野丢失，系统就立刻清空避让状态，导致角色在“避让”与“直走”之间高频鬼畜抽搐。
4.  **粗暴的大圆壳（形状失真）**：V1 将长方形的 2D 碰撞体粗暴地包裹为巨大的圆形（Interaction Radius），导致物理空间存在严重浪费与虚假阻挡。

---

## 2. 核心控制流的绝对反转 (Inversion of Control)

在 V2 的重构中，必须建立 **“交通裁决层（Layer C）绝对霸权”**。

### 2.1 权力的剥夺（大脑层的退化）
未来的 `PlayerAutoNavigator` 和 `NPCAutoRoamController` 必须被极度削薄。
* **输入**：它们只负责接收玩家点击或 AI 树发出的 `MotionIntent`（目标坐标/跟随目标）。
* **禁区**：**绝对禁止**它们私自调用 `NavGrid2D`；**绝对禁止**它们内部包含 `if (cooldown < 0) return false` 这种阻断交通流的代码。它们只负责“点菜”，不负责“做菜”。

### 2.2 权力的集中（状态仲裁者的建立）
必须将 `NavigationLocalAvoidanceSolver` 升级（或封装）为拥有绝对统治力的**有状态组件/模块**（例如集成入 `NavigationPathExecutor2D` 或独立的 `TrafficArbiter`）。
* **裁决即军令**：当仲裁者输出 `Yield`（让行）或 `Wait`（等待）时，控制器与底盘必须**无条件服从**。
* **状态维护**：仲裁者内部自己维护交通状态的流转、冷却与惯性。

### 2.3 盲从的底盘（统一的运动学语义）
* **废除“摩擦滑行”**：废除 `PlayerMovement.cs` 中那种剔除法线速度、强行贴边摩擦的伪避让逻辑。
* **统一响应**：无论底层是 `linearVelocity` 还是其他接口，底盘必须建立统一的运动学响应。当接收到 `Wait/Yield` 军令时，必须能将当前速度**平滑衰减至零**，而不是像撞到空气墙一样瞬停，也不能“一边让行一边摩擦前进”。

---

## 3. 四大核心机制重构规范 (The Four Pillars)

### 规范 A：Detour (绕行) 的降维与隔离
**禁令**：绝对不允许将微观避让产生的点（Point）或向量（Vector）插入或移出 A* 生成的 `PathCorridor`（宏观走廊数组）！

* **正解设计**：Detour 必须被定义为一种 **“短期空间意图（Short-term Spatial Intent / OverrideTarget）”**。
* **生命周期**：
    1.  当发生拥堵且优先级允许时，仲裁者计算出一个 `OverrideTarget`（或独立的 `SuggestedDetourDirection`）。
    2.  执行层**挂起（Suspend）**对主路径的追踪，完全转向此 Detour 目标移动。
    3.  一旦满足到达条件或脱离冲突区，执行层**清除** Detour 状态，**恢复（Resume）**对原主路径的追踪。
    4.  全程不增删主路径 `List` 里的任何一个元素。

### 规范 B：交通记忆与状态惯性 (Traffic Hysteresis)
这是消灭“单帧抽搐”和“清空风暴”的唯一解药。

* **惯性锁机制**：系统必须引入状态保持的最小时间（例如 `0.3s - 0.5s`）或空间滞后量。
* **示例**：如果仲裁者决定 `SidePass`（侧绕），即使在侧绕的第二帧因为角度变化“看”不到前方的 NPC，也**必须**维持 `SidePass` 状态直到最小时间耗尽或成功抵达 Detour 容差范围内。**严禁单帧翻转决策。**

### 规范 C：形状认知 (Shape-Aware Narrow-phase)
淘汰基于圆心距离的粗暴加减法，释放 2D Tilemap 宝贵的物理空间。

* **计算基准**：微观避让和接触判定，必须优先使用 `Collider2D.ClosestPoint` 来获取真实碰撞体边缘的极近点。
* **性能兜底**：为了防止 O(N^2) 的性能爆炸，必须坚持 **Broad-phase（宽阶段，粗筛）与 Narrow-phase（窄阶段，精筛）分离** 的原则。先通过 Grid 或距离粗筛周边动态实体，只有进入关注半径的实体，才调用昂贵的 `ClosestPoint` 进行精细的间隙（Clearance）判断。

### 规范 D：剥离双重阻挡语义
* **静态归静态**：`NavGrid2D` 及相应的全局 A* 寻路，**只探测死物（墙、树木、建筑、固定的障碍）**。移动的 NPC、玩家、宠物，绝对不允许被写入静态网格的阻挡检测中。
* **动态归动态**：所有动态实体的避让，全部交由 Layer C 的注册表（Registry）和仲裁者，在运行时通过速度干预和 Detour 隔离来解决。

---

## 4. V2 执行纪律与落地步骤

在接下来的代码编写中，你必须严格遵循 `006` 和 `007` 既定的 S2-S4 路线，**禁止进行高风险的硬切（一口气重写所有类）**。

请严格按照以下顺序执行手术：

1.  **阶段一（固本）**：先处理 `NavigationAvoidanceRules` 和 `NavigationLocalAvoidanceSolver`。引入状态机（Proceed/Yield/Wait/SidePass），加入 Hysteresis 惯性锁代码。将距离判断替换为基于 `ClosestPoint` 的机制。
2.  **阶段二（接线）**：处理 `NavigationPathExecutor2D`。实现 Detour 的“挂起与恢复”逻辑，确保 OverrideWaypoint 彻底独立于主路径数组。
3.  **阶段三（挖心）**：进入 `PlayerAutoNavigator` 和 `NPCAutoRoamController`。无情地删去它们内部所有的 Cooldown 拦截、私自的 `BuildPath` 重建逻辑。让它们变成纯粹向 Executor 提交意图的“瘦客户端”。
4.  **阶段四（修腿）**：清理 `PlayerMovement` 和 `NPCMotionController`。确保它们能够优雅地接收 `Wait/Yield` 带来的零速度指令，并实现平滑刹车。

**架构师寄语**：
不要迷信复杂的数学算法，不要在 A* 上强行插点。让静态的路径保持静态，让动态的避让回归流动的速度与状态。拿起这份指南，去终结这座屎山吧！