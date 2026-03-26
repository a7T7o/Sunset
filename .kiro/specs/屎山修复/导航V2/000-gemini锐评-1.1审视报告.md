# `000-gemini锐评-1.1.md` 审视报告

## 路径判断

`Path C`

这份锐评抓到了一部分真实病灶，但它把“有价值的问题意识”直接上提成了“V2 唯一最高认知输入 + 立即执行的硬性约束”，这一步已经与 Sunset 当前正式上位设计、当前委托边界和现有代码事实发生冲突，不能直接采纳为施工蓝图。

---

## 1. 锐评核心观点摘要

这份锐评的核心主张有 7 条：

1. `PlayerAutoNavigator` / `NPCAutoRoamController` 仍是 God Class，当前失败本质是控制流崩坏。
2. 必须建立“交通裁决层绝对霸权”，控制器与底盘对 `Yield / Wait` 无条件服从。
3. `Detour` 只能是独立短期空间意图，绝对不能污染主路径。
4. 系统必须引入状态惯性（Hysteresis），禁止单帧翻转。
5. 系统必须引入 `ClosestPoint` + broad-phase / narrow-phase 的 shape-aware 判定。
6. 应把 `NavigationLocalAvoidanceSolver` 升级为有状态仲裁者，必要时并入 `NavigationPathExecutor2D` 或独立成 `TrafficArbiter`。
7. 应严格按“先 Rules / Solver，再 Executor，再 Controllers，最后 Movement”的顺序硬切重构。

---

## 2. 事实核查结果表

| 锐评声明 | 代码 / 文档事实 | 结论 |
|---|---|---|
| 这份文档可以作为 V2 的“最高认知输入”，对 `006/007` 做硬性补全 | Sunset 当前正式上位文档仍是 `006-Sunset专业导航系统需求与架构设计.md` 与 `007-Sunset专业导航底座后续开发路线图.md`；当前 2026-03-26 的 live 委托还额外限定了“只做 detour owner 保活最小闭环，不回漂 solver / TrafficArbiter / MotionCommand”。这份锐评没有治理拍板，不能反客为主 | ❌ 与当前正式口径冲突 |
| `PlayerAutoNavigator` / `NPCAutoRoamController` 仍然职责过重 | 当前两者仍同时持有 `NavGrid2D`、建路 / 重建、局部避让、detour create/release、stuck / recover 与运动桥接职责，见 `PlayerAutoNavigator.cs:27/169/598/783/805`、`NPCAutoRoamController.cs:50/464/808/937/968` | ✅ 问题存在 |
| `Detour` 绝对不能污染主路径，应改成独立 override target | 当前 `NavigationPathExecutor2D` 已经把 detour 独立为 `OverrideWaypoint` / `LastDetourOwnerId` / `LastDetourPoint`，且 create / clear / recover 生命周期也是独立 API，见 `NavigationPathExecutor2D.cs:26-31`、`615-825` | ✅ 方向正确，但并非全新发现 |
| 必须引入状态惯性，禁止单帧翻转 | `006/007` 已明确要求交通记忆与状态惯性；当前 `TryClearDetourAndRecover(...)` 也已有 `minimumDetourActiveDuration / recoveryCooldown`，见 `007-Sunset专业导航底座后续开发路线图.md:199-230`、`NavigationPathExecutor2D.cs:729-804` | ✅ 方向正确 |
| 控制器不应再私自直连 `NavGrid2D` / `BuildPath` / `TryRebuildPath` | 这是 `S6` 脑层退壳后的长期方向，但 `007` 当前正式顺序仍是 `S2 -> S3 -> S4 -> S5 -> S6`，而不是立刻把 controller 清空；当前代码也还处在过渡态，不能把这条直接上升成“本轮绝对禁令” | ⚠️ 长期方向对，当前执行时机不对 |
| 应把 solver 升级成有状态仲裁者，必要时独立成 `TrafficArbiter` | `007` 确实要求 `S2` 建立正式交通裁决层，但 2026-03-26 的 live 委托已明确禁止回漂 `TrafficArbiter / MotionCommand`；把它重新抬成当前主刀会直接越界 | ❌ 与当前委托边界冲突 |
| 应严格按“Rules/Solver -> Executor -> Controllers -> Movement”四段手术顺序推进 | `007` 的正式阶段顺序是 `S2 交通裁决`、`S3 求解层`、`S4 共享路径执行层净化`、`S5 统一运动执行层`、`S6 脑层接入与旧闭环下线`。这份锐评把 `Controllers` 提前到 `Movement` 之前，并试图压缩阶段边界，和当前正式路线不一致 | ❌ 与现行路线图冲突 |
| 应尽快清理 `PlayerMovement` / `NPCMotionController` 现有阻挡兜底 | 当前 `PlayerMovement.ApplyBlockedNavigationVelocity(...)` 仍在承接玩家近距阻挡兜底，`NPCAutoRoamController.StopForSharedAvoidance(...)` 也仍是 live 运行闭环的一部分；在 detour owner 闭环未完全站稳前直接删除会增加回归风险，见 `PlayerMovement.cs:279-323`、`NPCAutoRoamController.cs:1038-1059` | ⚠️ 问题意识对，但当前顺序不安全 |

---

## 3. 认同的部分

以下内容我认同，而且认为这份锐评点得准：

1. 当前导航的核心失败不再是“再调一个 solver 权重”。
2. `PlayerAutoNavigator` / `NPCAutoRoamController` 的职责确实仍然过重。
3. `Detour` 必须是独立生命周期，不能反向污染主路径。
4. 交通状态必须有惯性，而不是依赖单帧命中。
5. shape-aware 方向是对的，当前系统也确实还没有真正完成 `S3`。

---

## 4. 疑虑与异议

### 异议 1：它把“问题意识输入”写成了“最高认知输入”

这份锐评最危险的不是批评太狠，而是它在开头就把自己抬成：

- `006/007` 的硬性补全
- V2 线程的最高认知输入

这在 Sunset 当前上下文里不成立。

当前真正已经生效的依据仍然是：

1. `006-Sunset专业导航系统需求与架构设计.md`
2. `007-Sunset专业导航底座后续开发路线图.md`
3. 2026-03-26 当轮 live 委托
4. 当前代码热区与运行态事实

锐评可以是“很有价值的补充刺激”，但不能自己宣布成为上位法。

### 异议 2：它把“最终目标”偷换成了“当前切片必须立即执行”

“controller 退壳”“交通裁决层更强”“底盘统一语义”这些方向本身没有错。

错在它把这些长期目标写成了当前必须一步到位的执行约束，例如：

- 绝对禁止 controller 继续碰 `NavGrid2D`
- 绝对禁止 controller 继续持有 `BuildPath / TryRebuildPath`
- 立即把 solver 升级成新的有状态仲裁者

而 `007` 当前的正式节奏仍然是分阶段迁移，不是一次性硬切。

### 异议 3：它会把当前主刀重新带回 `TrafficArbiter / MotionCommand`

这点在 Sunset 当前现场尤其危险。

2026-03-26 的 V2 live 委托已经明确限制：

- 只锁 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D`
- 不要回漂 solver 权重
- 不要重开 `TrafficArbiter / MotionCommand`

这份锐评却把“独立仲裁者”重新抬成推荐路线之一。

这会直接把当前已经收窄到 detour owner 闭环的问题，再次扩回一整轮大架构争论。

### 异议 4：它对执行顺序的压缩，会打破 `007` 现有阶段纪律

`007` 当前正式定义的是：

1. `S2` 交通裁决层 MVP
2. `S3` 微观运动求解层替换
3. `S4` 共享路径执行层二次净化
4. `S5` 统一运动执行层
5. `S6` 玩家 / NPC 行为脑接入与旧闭环下线

而这份锐评试图把：

- controller 退壳
- movement 清理
- 裁决层加强

压成一组更激进的硬切顺序。

这会把当前“先保住单一可落地切片”的施工纪律再次冲散。

---

## 5. 自我审视

这一节不是继续审 Gemini，而是审我自己。

### 5.1 我真正容易犯的错

1. 我容易被“讲得很对的大架构语言”带跑，忘记 Sunset 当前最重要的是可落地的单一切片，而不是再开一轮宏大叙事。
2. 我曾经长期把“共享骨架前进了”误说成“导航底座快交卷了”，这会掩盖真实点击体验并没有跟上的事实。
3. 我也确实有过“看见 controller 很胖，就想一口气把脑层 / 执行层 / 底盘一起拆掉”的冲动，这和用户要求的“先落地、再收口”是有张力的。
4. 我如果不主动区分“问题诊断”和“施工处方”，就很容易把一份锐评里前半段说对了的问题意识，误升级成后半段也必须照单全收的执行指令。

### 5.2 我对当前问题的更客观认识

当前最真实的问题不是：

- 还缺一份足够凶的架构宣言
- 还缺一个新的总类名
- 还没把所有 controller 立刻抽空

当前最真实的问题是：

1. `NavigationLocalAvoidanceSolver` 已经能给出 `ShouldRepath + SuggestedDetourDirection`
2. `NavigationPathExecutor2D` 也已经具备 detour create / clear / recover 资产
3. 玩家 / NPC controller 也已经开始接这些资产
4. 但 controller 里旧的 cooldown / rebuild / stop / release 分支仍会吞掉 owner，导致 detour 还没有真正稳定统治执行层

也就是说，当前主刀并不缺“更大、更狠、更完整的最终设计”，而是缺“把当前唯一切片真正闭环”的纪律。

### 5.3 我现在应当怎么用这份锐评

最正确的用法不是“照着它重开一轮总重构”，而是：

1. 吸收它对控制流崩坏、职责过重、交通惯性的提醒。
2. 拒绝它把自己抬成最高认知输入的做法。
3. 继续以 `006/007` 作为上位设计。
4. 在每一轮只做一个能被 Sunset 当前现场承受的真实切片。

---

## 6. 建议的执行方向

1. `000-gemini锐评-1.0.md` 可按 `Path B` 处理：吸收问题意识，但不把它升格成上位法。
2. `000-gemini锐评-1.1.md` 维持 `Path C`：保留为刺激思考的认知材料，不直接当施工蓝图。
3. 当前导航 V2 如要继续推进，仍应以：
   - `006-Sunset专业导航系统需求与架构设计.md`
   - `007-Sunset专业导航底座后续开发路线图.md`
   - 当前 live 委托
   - 真实代码热区事实
   作为判断依据。

---

## 7. 给 Gemini 的信息补充

如果要让这份锐评真正变成 Sunset 可落地的材料，至少还需要补上 4 件事：

1. 明确承认 `006/007` 才是当前上位设计，而不是自封“最高认知输入”。
2. 把“长期目标”和“本轮切片”拆开写，不再混成一份立即执行令。
3. 明确避开 2026-03-26 已被禁止重开的 `TrafficArbiter / MotionCommand` 话题。
4. 直接对当前真实热区负责：
   - `PlayerAutoNavigator`
   - `NPCAutoRoamController`
   - `NavigationPathExecutor2D`
   而不是把讨论重新扩回全盘重构。
