# `005-genimi锐评-4.0.md` 审视报告

## 1. 锐评核心观点摘要

这份 Gemini 锐评的核心主张有 5 条：

1. 当前 V1 导航落地失败的根因不是 solver 数学，而是控制流与职责切分失败。
2. 现有 `PlayerAutoNavigator` / `NPCAutoRoamController` 是 God Class，必须拆成：
   - Brain
   - Core `DynamicNavigationAgent`
   - Locomotion
3. 必须彻底废除“动态 detour waypoint”思路，微观避让只能产出速度向量，不能再落临时绕行点。
4. 必须引入交通状态机、状态惯性、形状认知和真实 clearance。
5. 必须按“换腿 -> 造脑 -> 挖心”的顺序，迅速把当前代码切成新架构，并统一到 `Rigidbody2D.linearVelocity`。

这份锐评的价值在于，它敏锐抓住了“职责割裂导致控制流崩坏”这个大方向；  
但它的问题也很明显：它把正确的问题诊断，包装成了一组过度绝对、且与当前 `006/007` 上位设计直接冲突的实现圣旨。

---

## 2. 事实核查结果表

| 锐评声明 | 代码/文档事实 | 结论 |
|---|---|---|
| `PlayerAutoNavigator` / `NPCAutoRoamController` 是 God Class | 当前两者仍同时持有目标、路径、局部避让、stuck/rebuild、运动收口等职责，见 [PlayerAutoNavigator.cs:750](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L750)、[PlayerAutoNavigator.cs:764](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L764)、[NPCAutoRoamController.cs:862](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L862)、[NPCAutoRoamController.cs:968](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L968) | ✅ 问题存在 |
| 当前应新建唯一核心 `DynamicNavigationAgent`，并把一切导航状态收进去 | 当前代码里不存在 `DynamicNavigationAgent`，但 `006`/`007` 只要求按 Layer A-E 分层与分阶段迁移，并未要求“必须一个 MonoBehaviour 包圆”，见 [006-Sunset专业导航系统需求与架构设计.md:167](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L167)、[007-Sunset专业导航底座后续开发路线图.md:111](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/007-Sunset专业导航底座后续开发路线图.md#L111) | ⚠️ 问题方向对，但处方过度绝对 |
| `GameInputManager` 当前只是把目标点传给导航器，这是正确的 | 当前 `GameInputManager` 不只传点，还负责 deadzone、点击限流、资源节点 bounds 交互、Box 关闭等逻辑，见 [GameInputManager.cs:1018](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs#L1018) | ❌ 事实不完整 |
| 微观避让绝对不允许产生路径点/Waypoint | `006` 明确写了“允许存在临时中间目标”，禁止的是“把它写成主路径状态的一部分”，见 [006-Sunset专业导航系统需求与架构设计.md:214](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L214) | ❌ 与现行上位设计冲突 |
| 交通惯性/状态锁是必须的 | `006` 与 `007` 都明确要求交通状态与状态惯性，当前代码也已部分接入 detour 最小时长，见 [006-Sunset专业导航系统需求与架构设计.md:323](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L323)、[PlayerAutoNavigator.cs:915](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L915)、[NPCAutoRoamController.cs:1015](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1015) | ✅ 方向正确 |
| 必须基于 `Collider2D.ClosestPoint`，彻底废除圆心距离 | `006` 的真实要求是 broad-phase / narrow-phase 分离，允许 broad-phase 继续近似，但 narrow-phase 要更贴脚底 footprint，见 [006-Sunset专业导航系统需求与架构设计.md:277](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L277) | ⚠️ 方向正确，但“彻底废除”过度绝对 |
| 必须统一全部使用 `linearVelocity`，废止 `MovePosition` | `006` 明确写了“是否全部用 `linearVelocity`，是实现选择”，硬要求是统一运动学语义而非强制同一函数，见 [006-Sunset专业导航系统需求与架构设计.md:232](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L232) | ❌ 与现行上位设计冲突 |
| 应先删 `PlayerMovement.ApplyBlockedNavigationVelocity` 之类底盘特判 | 当前玩家侧仍然依赖 blocked-navigation 语义承接临时阻挡；在新 owner 闭环没站稳前直接删，会让运行态进一步失去兜底，见 [PlayerMovement.cs:172](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerMovement.cs#L172)、[PlayerMovement.cs:285](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerMovement.cs#L285) | ⚠️ 长期方向可讨论，但当前执行顺序不安全 |
| 当前 detour 是绝对异端，应该整体废止 | 当前真正的问题不是“存在临时绕行点”本身，而是 detour owner 没稳定接管、clear/rebuild 风暴与主路径污染；当前 `NavigationPathExecutor2D.TryCreateDetour()` 与 `TryClearDetourAndRecover()` 已经是过渡资产的一部分，见 [NavigationPathExecutor2D.cs:615](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs#L615)、[NavigationPathExecutor2D.cs:691](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationPathExecutor2D.cs#L691) | ⚠️ 批判对象抓到了，但结论下得过头 |

---

## 3. 认同的部分

以下内容我认同，而且认为 Gemini 这次点得比较准：

1. 当前导航失败不只是 solver 参数问题。
   - 这一点与我们最新收敛高度一致。
   - 当前真正的问题确实是控制权在多层之间来回掉落。

2. 当前 `PlayerAutoNavigator` 与 `NPCAutoRoamController` 的职责仍然过重。
   - 这两个类仍然同时承担：
     - 脑层
     - 路径执行
     - 避让分支
     - 运动桥接
   - 这和 `006/007` 最终要去的脑层/执行层分离方向一致。

3. 交通语义应该先于方向修正。
   - 这一点和 [006-Sunset专业导航系统需求与架构设计.md:241](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L241) 完全一致。

4. 当前系统需要更强的状态惯性与交通记忆。
   - 这一点也与 `006` 的交通语义设计一致。

5. 当前体验问题已经不能再靠“继续调一个 avoidance 权重”来解决。
   - 这一点是事实。

---

## 4. 疑虑与异议

### 异议 1：把“必须新建唯一 `DynamicNavigationAgent`”当成唯一正确实现，过度绝对

当前 `006/007` 的上位设计要求是：

- 分层
- 统一语义
- 分阶段迁移

它没有要求：

- 必须用一个新的 `MonoBehaviour` 单类吞并所有导航状态
- 必须立刻重命名 `PlayerAutoNavigator -> PlayerNavigationBrain`
- 必须立刻重命名 `NPCAutoRoamController -> NPCRoamBrain`

这份锐评的问题不在方向，而在它把“一个可能的实现方案”写成了“唯一合法形态”。

这在 Sunset 当前上下文里是危险的，因为：

1. `007` 明确规定当前迁移要按 `S0 -> S8` 分阶段推进，而不是一刀硬切，见 [007-Sunset专业导航底座后续开发路线图.md:109](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/007-Sunset专业导航底座后续开发路线图.md#L109)
2. 当前运行态还没有完成 `S5/S6`，直接硬切“单核大 agent”会让迁移风险远高于收益

### 异议 2：把“绝对禁止临时中间目标”写成铁律，直接违背现行 `006`

Gemini 这条最危险。

现行 `006` 的红线是：

- 动态避让不能再以“修改主路径列表”为默认策略
- 但**允许存在临时中间目标**
- 前提是：
  - 生命周期独立
  - 不反向改写主路径走廊
  - 退出后自动回归主走廊
  - 不触发 clear/rebuild 风暴

证据在 [006-Sunset专业导航系统需求与架构设计.md:218](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L218)。

也就是说，Gemini 锐评把“当前 detour 写坏了”误上升成了“detour 这个概念本身是原罪”。

这会直接把当前可继承的 `NavigationPathExecutor2D` 过渡资产一刀判死，不符合 Sunset 现行设计。

### 异议 3：把“统一运动学语义”误写成“统一全部 `linearVelocity`”

这也是与 `006` 硬冲突的一条。

`006` 已经明确说：

- 玩家 / NPC 的最终运动学语义必须统一
- 但“是否全部用 `linearVelocity`，是实现选择”

证据在 [006-Sunset专业导航系统需求与架构设计.md:232](D:/Unity/Unity_learning/Sunset/.kiro/specs/屎山修复/导航检查/006-Sunset专业导航系统需求与架构设计.md#L232)。

Gemini 的问题是把“统一语义”偷换成了“统一 API 实现”。

这一步没有足够代码证据支撑，也会把当前 Sunset 的迁移问题从“语义闭环”错误引导成“强制换底盘实现”。

### 异议 4：对 `GameInputManager` 的事实陈述不准确

锐评说它“当前只是将目标点传给导航器，这是正确的”。

这并不准确。

当前 [GameInputManager.cs:1018](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs#L1018) 之后还承担了：

- nav click deadzone
- rate limit
- `ResourceNodeRegistry` sprite bounds 交互判定
- `BoxPanel` 关闭流程

它当然不该直接做局部避让几何，但“只是传点”这句在事实层面不成立。

### 异议 5：执行顺序过于激进，直接跳过当前 `007` 的阶段约束

Gemini 给的执行顺序是：

1. 先换腿
2. 再造脑
3. 再挖心

这套顺序的问题是，它想直接跳过当前真正还没站稳的 `S2-S4`。

当前 `007` 的正式路线是：

- `S2` 交通裁决层 MVP
- `S3` 微观运动求解层替换
- `S4` 共享路径执行层二次净化
- `S5` 统一运动执行层
- `S6` 玩家/NPC 行为脑接入与旧闭环下线

也就是说，Gemini 想把 `S5/S6` 先做掉，再反过来逼 `S2-S4` 就位。  
这在当前代码热区里风险非常高。

---

## 5. 自我审视

这一节不是审 Gemini，而是审我自己。

### 5.1 我前面真正犯过的错

1. 我长期把“结构线前进了”误说成“结果快交卷了”。
   - 用户骂得对。
   - 当前这一条线最大的失败，不是没写代码，而是没有持续产出用户能验收到的体验结果。

2. 我在很长一段时间里，把主刀错放在 solver 参数。
   - 尽管那里面确实有问题
   - 但在执行层握手断裂没修前，继续调 solver 基本是在沙盒里自娱自乐

3. 我迁了很多底座能力，但没有及时下线 controller 私有闭环。
   - 这导致“新骨架包旧黑盒”的状态持续太久

4. 我一度过早相信局部 runner 结果。
   - 用户现场复测多次证明，这是不够的

### 5.2 我刚刚还必须再纠正自己一次

我上一轮在 `导航检查V2` 交接材料里说过：

- `NavigationPathExecutor2D.TryClearDetourAndRecover()` 当前 runtime controller 没有调用点

这句话按当前代码已经不成立。

证据：

- 玩家 release 分支已经调用：
  - [PlayerAutoNavigator.cs:915](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs#L915)
- NPC release 分支也已经调用：
  - [NPCAutoRoamController.cs:1015](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs#L1015)

这说明我前一轮交接口径里有一处过度简化，必须当场承认。

更准确的当前事实应改写为：

- `TryClearDetourAndRecover()` 已经**部分接进 release 分支**
- 但 detour owner 仍然**没有稳定成为主执行状态**
- 问题不再是“clear/recover API 完全没接”
- 而是“create / keep / release 整体闭环仍没形成稳定 owner 统治”

### 5.3 我对当前问题的更客观认识

当前真正的问题已经不是：

- “要不要有临时 detour 点”
- “是不是一定要一个新 `DynamicNavigationAgent`”
- “是不是先统一 `linearVelocity`”

当前最真实、最窄、最不该回避的问题是：

1. solver 已经会产出 `ShouldRepath + SuggestedDetourDirection`
2. 玩家/NPC controller 也已经开始尝试 create / release detour
3. 但 controller 里仍有多处旧分支把 owner 吞掉、打断或退回旧路径
4. 因而 detour 没有真正统治执行层

所以，我认同 Gemini 的问题嗅觉；  
但我不认同它把问题翻译成“立刻硬切成单核 `DynamicNavigationAgent` + 禁 detour + 全部 linearVelocity”的处方。

---

## 6. 独立分析

综合 `005-genimi锐评-4.0.md`、`006`、`007`、当前代码与 `导航检查V2` 交接包，当前最稳妥的独立判断是：

### 6.1 当前主问题

当前主问题不是“底座不存在”，也不是“完全没有状态机”。

当前主问题是：

- 共享资产已经有了
- detour lifecycle 也已经有一部分了
- 但 controller 私有闭环还没退出
- 执行层 owner 还没坐稳

这是一种“过渡架构卡在半路”的问题。

### 6.2 当前最该做的不是“全盘拥抱 Gemini”，而是“用 Gemini 纠偏当前迁移路线”

具体说：

1. 吸收它对 God Class、控制反转、交通惯性、shape-awareness 的提醒
2. 拒绝它对：
   - `DynamicNavigationAgent` 唯一化
   - 绝对禁 detour 临时目标
   - 一刀切 `linearVelocity`
   - 立刻删掉 80% 旧控制器
   这些过度绝对处方

### 6.3 对 `导航检查V2` 的真实影响

`导航检查V2` 不应该被这份锐评带去做下面这些事：

1. 先写一个全新的 `DynamicNavigationAgent` 再说
2. 先删除所有 detour waypoint 逻辑再说
3. 先把 `MovePosition` 全部清掉再说

`导航检查V2` 真正应该做的是：

1. 修正我上一轮交接包里“clear/recover API 未接入”的失真描述
2. 继续围绕 create / keep / release 的 owner 闭环打执行层热区
3. 只有在 `S2-S4` 真正站稳后，才评估 Gemini 提出的“单核 NavigationAgent”是否值得本地化吸收

---

## 7. 建议的执行方向

### 路径判断

本次审核结论：**Path C**

原因：

1. 锐评指出的核心问题真实存在；
2. 但它给出的若干关键实现命令与 `006/007` 上位设计直接冲突；
3. 如果现在按它字面直接执行，会把当前导航线从“受控迁移”变成“高风险硬切”，不安全。

### 当前建议

1. 不直接采纳 Gemini 这份锐评作为实现指令
2. 把它当作一份：
   - 问题诊断层有价值
   - 方案处方层需要本地化重写
   的审计输入
3. 后续若继续导航，应以：
   - `006`
   - `007`
   - `导航检查V2` 交接包
   为主线依据
4. 若要吸收 Gemini 的价值，建议只吸收这 4 条：
   - God Class 不能继续放任
   - 交通裁决先于方向修正
   - 交通惯性必须加强
   - shape-aware narrow-phase 必须继续推进

---

## 8. 给 Gemini 的信息补充

如果要把这份审视反馈给 Gemini，最重要的信息不是“你错了”，而是下面这几点：

1. 你抓到的“大方向”是有价值的：
   - 当前确实不是继续调 solver 就能好
   - 当前确实存在 God Class 和职责割裂

2. 但你不了解 Sunset 当前已经冻结的上位设计：
   - `006` 明确允许临时中间目标，但要求生命周期独立
   - `006` 明确不强制统一到 `linearVelocity`
   - `007` 明确要求分阶段迁移，而不是一刀硬切

3. 当前代码比你假设的更复杂：
   - detour release/hysteresis 已经不是完全不存在
   - 问题更像“闭环没站稳”，而不是“状态机完全为零”

4. 如果要继续给 Sunset 导航提建议，下一版更有效的方向应是：
   - 不要给绝对化圣旨
   - 改给“如何在 `S2-S4` 之间把 owner 闭环站稳”的局部方案

---

## 9. 最终裁定

- 我认同这份 Gemini 锐评的**问题意识**；
- 我不认同它的**落地处方**；
- 我也必须承认，我自己前面的落地与交接里确实存在“过早乐观”和“一处事实口径已过期”的问题。

因此，本轮正式裁定为：

> **Path C：存在架构误读与不安全的绝对化建议，必须以本审视报告为准，不直接照单执行。**

---

## 10. 与 v1 并行审查一致性回执

本轮已完成并行复核（v1 独立审查）并与主审交叉核对，结论一致：

1. 这份锐评应判 `Path C`，不能作为当前实现指令直接落地。
2. 它指出的“控制器职责过重、运动语义分裂”是成立问题。
3. 它给出的“必须新建唯一 `DynamicNavigationAgent`、彻底废除 detour waypoint、立刻统一到 `linearVelocity`”属于过度绝对化处方，与 Sunset 当前 `006/007` 口径存在直接冲突。
4. 当前更稳妥路线是：继续按现有分阶段迁移推进，吸收其问题意识，不照单执行其硬切处方。
