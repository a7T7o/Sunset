**《Sunset 导航系统 V2 架构师最高圣旨 (The Architect's Edict)》**

**致 V2 导航重构线程 (AI Agent)：**
当你接手这个项目时，你将继承 `006` 与 `007` 文档的宏伟蓝图，但你**必须彻底抛弃 V1 线程在代码落地时的所有伪架构**。V1 线程的失败不在于数学公式，而在于它用“面向过程的静态工具类”去伪装“面向对象的状态机”，导致了控制流的彻底崩塌。

从这一刻起，你编写的每一行代码，都必须绝对服从以下五大架构纪律。任何试图绕过这些纪律、继续在旧的 `PlayerAutoNavigator` 里打补丁的行为，都将被视为破坏项目根基。

---

### 第一法则：绝对的控制反转 (Inversion of Control) 与三层物理隔离

现有的 `PlayerAutoNavigator` 和 `NPCAutoRoamController` 犯了经典的“上帝类 (God Class)”反模式错误——它们既发号施令，又自己寻路，还自己躲避，最后还要自己挪动刚体。

**重构最高指令：必须将实体强行撕裂为完全不知道彼此存在的三个独立组件！**

#### 1. 大脑层 (The Brain): 纯粹的意图发射器
* **代表组件**：修改后的 `PlayerAutoNavigator` (应更名为 `PlayerNavigationBrain`)、修改后的 `NPCAutoRoamController` (应更名为 `NPCRoamBrain`)。
* **输入入口现状**：基于对 `GameInputManager.cs` 的审计，它当前只是将目标点传递给导航器，这是正确的。**严禁**让 `GameInputManager` 接触任何射线、碰撞、避让的几何计算。
* **职责红线**：大脑层**只允许**输出两种东西：
    1.  `MotionIntent` (目标坐标 / 跟随目标实体)。
    2.  `DesiredSpeed` (散步 / 奔跑 / 追击速度)。
* **剥夺权力**：大脑层**绝对不允许**持有 `Path` 数组，**绝对不允许**调用 `NavGrid2D`，**绝对不允许**知道什么是 `Detour`。发出意图后，大脑层的工作就结束了。

#### 2. 核心状态机 (The Core): 唯一的统帅 `NavigationAgent`
* **代表组件**：**[必须新建]** `DynamicNavigationAgent : MonoBehaviour`
* **职责红线**：这是挂载在所有移动体（玩家、NPC、怪物）身上的**唯一且核心的导航组件**。它是一个**有记忆、有状态**的实体。
* **控制权收口**：
    * 它接收来自大脑层的 `MotionIntent`。
    * 它内部私有化调用 `NavGrid2D` 获取宏观静态走廊 (Path Corridor)。
    * 它内部私有化调用 `TrafficArbiter` (原 Solver) 决定当前的社会交通状态 (`Proceed / Yield / Wait / SidePass`)。
    * 它维护状态的最小时长（Hysteresis），绝不允许单帧横跳。
    * 它计算出当前这一物理帧的 `FinalVelocity`。

#### 3. 盲从底盘 (The Locomotion): 无脑的物理执行器
* **代表组件**：修改后的 `PlayerMovement.cs` 和 `NPCMotionController.cs`。
* **剥夺权力**：彻底删除 `PlayerMovement` 中的 `ApplyBlockedNavigationVelocity` 等丑陋的法线剔除逻辑。底盘**不应该知道**前面有没有障碍物！
* **唯一接口**：只对外暴露 `public void SetVelocity(Vector2 velocity, Vector2 facingDirection)`。如果 `NavigationAgent` 传过来的是 `Vector2.zero`，底盘就老老实实地停下并播放 Idle 动画；如果传过来的是绕行的速度，底盘就执行移动。

---

### 第二法则：彻底废除“动态绕行点 (Dynamic Detour Waypoint)”的异端邪说

V1 架构最愚蠢的设计，就是为了躲避一个会动的 NPC，强行去 `NavGrid2D` 里找一个相邻的格子作为 `OverrideWaypoint` 塞进路径里。这不仅污染了宏观路径，还导致了死锁、清空风暴和“走到一半突然掉头”的灾难。

**重构最高指令：微观避让绝对不允许产生路径点 (Waypoint)！**

1.  **宏观走廊 (Macro Corridor) 的神圣不可侵犯**：`NavGrid2D` 算出来的路径点，**只负责绕开死物（墙壁、水坑）**。只要终点没变，这条路径就是神圣的。如果玩家在去终点的路上遇到 NPC，**绝对不允许**去修改这条路径。
2.  **微观转向 (Micro Steering) 是速度向量，不是坐标点**：当遇到动态 NPC 时，`NavigationAgent` 进入 `SidePass` 状态，此时系统只需要计算出一个**侧向的速度向量 (Velocity Vector)**。
3.  **流体绕行 (Fluid Steering)**：角色叠加这个侧向速度后，会在物理空间中画出一条平滑的弧线绕过 NPC。绕过之后，NPC 不在视野内，角色继续朝着原本神圣不可侵犯的宏观路径点前进。**全程没有生成和销毁任何 Detour 点！**

---

### 第三法则：交通记忆与状态惯性 (Traffic Hysteresis) 的强制实现

V1 系统中 `ShouldRepath` 返回 True 然后瞬间被单帧 Raycast 清空的弱智逻辑，必须被彻底碾碎。没有任何交通系统是每秒钟改变 60 次主意的。

**重构最高指令：`NavigationAgent` 内部必须维护强约束的交通状态机！**

* **状态定义**：
    * `Proceed` (路权在我，按设定速度向目标移动)
    * `Yield` (路权在人，减速并寻找错身空间)
    * `Wait` (死胡同或极高优先级目标通过，速度严格为 0)
    * `SidePass` (空间足够，执行侧向 Steering 绕行)
* **惯性锁 (State Lock)**：任何导致降级的状态切换（如 `Proceed -> Wait` 或 `Proceed -> Yield`），必须拥有**最小保持时间（例如 0.4 秒）**。即使下一帧 NPC 突然消失，角色也必须把这 0.4 秒的 `Wait` 状态执行完（表现为短暂的愣神或礼让惯性），**严禁出现高频鬼畜抽搐！**
* **物理平滑 (Velocity Blending)**：状态切换导致的 `FinalVelocity` 变化，必须在 `NavigationAgent` 内部经过平滑阻尼 (Damping) 处理。`Proceed (速度4) -> Wait (速度0)` 必须产生 0.2 秒的刹车滑行，而不是撞空气墙般的瞬停。

---

### 第四法则：形状认知 (Shape-Awareness) 与真实的 Clearance

大圆壳（Interaction Radius）的存在，是因为 V1 为了偷懒，把长方形的人当成了碰碰球。

**重构最高指令：避让计算必须基于 `Collider2D.ClosestPoint`，废除基于圆心的距离加减法！**

1.  在计算两个人是否会相撞时：
    * **淘汰的旧公式**：`Distance(A.Center, B.Center) < A.Radius + B.Radius + Padding`
    * **必须采用的新公式**：`Distance(A.Center, B.Collider.ClosestPoint(A.Center)) < A.Radius + Padding`
2.  这种做法完美适配俯视角 2D 游戏的 `BoxCollider2D`，让角色可以在横向和纵向上以完全不同的容差擦肩而过，彻底消灭“明明旁边还有半个身位的空隙，角色却觉得被堵死”的假象。

---

### 第五法则：V2 代码开荒的绝对执行顺序

新 Agent 接手后，**严禁一上来就全面开花修改十几个文件**。必须采用“挖心、造脑、换腿”的极简分步手术：

* **手术第一刀（换腿）：洗净运动底盘。**
    * 进入 `PlayerMovement.cs` 和 `NPCMotionController.cs`。删掉所有关于 Navigation 避让、Blocker 的特判。让它们变成纯粹的 `SetVelocity(...)` 接收器，统一全部使用 `Rigidbody2D.linearVelocity` 作为唯一位移手段。废止 `MovePosition`。
* **手术第二刀（造脑）：构建全新核心。**
    * 新建 `DynamicNavigationAgent.cs`。把旧的 `NavigationPathExecutor2D` 里的寻路逻辑、卡顿检测逻辑吸纳进来作为私有方法。
    * 实现上文要求的交通状态机（Proceed/Yield/Wait）和惯性锁。
* **手术第三刀（挖心）：剥夺旧控制器的权力。**
    * 进入 `PlayerAutoNavigator.cs` 和 `NPCAutoRoamController.cs`。
    * **删掉它们里面 80% 的代码**。它们不再需要 `Update` 里巨大的 `switch-case`，不再需要维护 `pathIndex`，不再需要自己去射射线。
    * 它们唯一需要做的，就是在 `StartRoam` 或接收到玩家点击时，调用 `GetComponent<DynamicNavigationAgent>().SetDestination(target)`。

**最高警告：** 如果新的代码提交中，再次出现了把“动态避让”和“更新 A* 路径点”混为一谈的逻辑，或者出现了 `if (cooldown < 0) return false` 这种把控制权丢回黑洞的代码，本次 V2 迁移将被判定为**严重违规并立即中止**。

以架构师的名义，去执行这套真正的控制反转吧！