# 共享根仓库分支漂移与现场占用治理 - 开发记忆
## 模块概述
- 本阶段专门处理 `NPC/farm` 混线事故，以及由此暴露出的“共享根目录分支上下文未被强制治理”的缺口。

## 当前状态
- **完成度**: 82%
- **最后更新**: 2026-03-17
- **状态**: 已完成取证、临时裁定与双线执行方案，等待实际收口执行

## 会话记录
### 会话 1 - 2026-03-17（完成交叉取证并形成临时裁定）
**用户目标**:
> 彻底厘清 `NPC/farm` 为什么会混线，判断这是不是“分支也要纳入锁”，并把这次事故收成可执行治理动作。

**本轮子任务**:
- 只读核验共享根目录、worktree、checkout 轨迹、关键提交归属。
- 吸收 `farm` 与 `NPC` 两条线程的补充证词。
- 把当前裁定沉淀为后续执行令。

**已完成事项**:
1. 确认共享根目录真实现场是 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001 @ 11e0b7b4`。
2. 确认旧 NPC worktree 仍存在，但只是历史检出点，不是当前默认开发现场。
3. 确认 `07ffe199` 为 `mixed`、`18f3a9e1` 为 `NPC`、`11e0b7b4` 为 `farm`。
4. 吸收 `farm` 证词，确认 cleanroom 起点应为 `b9b6ac48`，以及 farm-only 回放边界。
5. 吸收 `NPC` 证词，确认唯一救援基线是 `codex/npc-roam-phase2-001 @ f6b4db2f`。
6. 形成临时裁定并落盘到本阶段分析与执行方案。

**关键决策**:
- 共享根目录立刻进入业务写入冻结。
- `NPC` 走 `f6b4db2f` 救援线，不从共享根目录继续写。
- `farm` 不沿污染分支继续开发，转入 `b9b6ac48` cleanroom。
- 后续新增的规则不是“把分支并进文件锁”，而是补一层 `root-workdir lease / branch occupancy`。

**涉及文件**:
- [analysis.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/10_共享根仓库分支漂移与现场占用治理/analysis.md)
- [tasks.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/10_共享根仓库分支漂移与现场占用治理/tasks.md)
- [执行方案.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/10_共享根仓库分支漂移与现场占用治理/执行方案.md)

**验证结果**:
- `git worktree list --porcelain`、`git reflog --all --grep-reflog='checkout:'` 与双方线程证词相互印证。
- 当前裁定已具备执行基础，不再是纯推测。

**恢复点 / 下一步**:
- 下一步要把这里的裁定纳入 `09_强制skills闸门与执行规范重构`，让“共享根目录占用校验”进入启动闸门。
- 对外可直接下发两条执行令：
  - `NPC` 进入救援线最小收口
  - `farm` 准备 cleanroom 白名单重放

### 会话 2 - 2026-03-17（NPC 与 farm 都已返回执行级收口方案）
**用户目标**:
> 吸收 `NPC` 与 `farm` 的正式回报，继续推进，不停在分析层，并产出下一跳的正式 prompt。

**本轮子任务**:
- 接收两条线程的执行级方案。
- 判断当前是否已具备进入实际收口执行的条件。
- 给出可直接转发的下一步 prompt。

**已完成事项**:
1. `NPC` 明确确认：
   - 唯一救援基线仍是 `codex/npc-roam-phase2-001 @ f6b4db2f`
   - 该线不缺 NPC 核心文件
   - 最小剔除动作是将 [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs) 回退到 `8aed637f`
   - 最小验证是编译通过、Console 无 NPC 红错，并抽查 `001/002/003` 三个样本 Prefab / 动画链
2. `farm` 明确确认：
   - `codex/farm-1.0.2-correct001` 只保留为事故现场
   - cleanroom 起点仍是 `b9b6ac48`
   - farm-only 代码与正文文档边界已锁定
   - 回放顺序已锁定为：
     1. 从 `07ffe199` 只回放 `FarmToolPreview.cs`
     2. 从 `11e0b7b4` 回放其余 7 个 farm 代码文件和四份正文文档
     3. 在新现场重写三层 farm memory 与线程 memory

**关键决策**:
- 当前不再缺“分析证据”，而是已经进入“可以执行收口”的状态。
- `NPC` 与 `farm` 可以并行推进，但都不得继续借用共享根目录。
- 共享根目录冻结保持不变，直到 `NPC` 最小收口与 `farm` cleanroom 至少有一侧完成稳定落地。

**恢复点 / 下一步**:
- 向 `NPC` 下发正式执行令：在独立 NPC 可写现场完成一刀回退和最小验证。
- 向 `farm` 下发正式执行令：在独立 cleanroom 可写现场按白名单顺序重建。

### 会话 4 - 2026-03-17（正式执行令已交付用户）
**用户目标**:
> 不再停在治理分析层，而是直接拿到可转发给 `NPC` 与 `farm` 的正式执行 prompt。

**本轮子任务**:
- 将前序取证、临时裁定和双方执行级回报收束成对外执行令。
- 维持共享根目录冻结口径不变，避免用户在发令前后再次把业务线程送回事故现场。

**已完成事项**:
1. 完成面向 `NPC` 的正式执行令定稿：
   - 进入独立可写现场
   - 以 `codex/npc-roam-phase2-001 @ f6b4db2f` 作为唯一救援基线
   - 只做 `FarmToolPreview.cs -> 8aed637f` 的最小回退
   - 执行编译、Console、Prefab / Sprite / 动画 / 漫游组件链抽查
2. 完成面向 `farm` 的正式执行令定稿：
   - 禁止继续沿 `codex/farm-1.0.2-correct001` 开发
   - 在独立 cleanroom 以 `b9b6ac48` 起步
   - 严格按 `07ffe199` / `11e0b7b4` 的白名单顺序回放 farm-only 内容
   - 在新现场重写 farm 三层 memory 与线程记忆
3. 维持项目级裁定：
   - 共享根目录 `D:\Unity\Unity_learning\Sunset` 继续视为冻结事故现场
   - 两条业务线程可以并行推进，但不得再借用共享根目录写业务

**恢复点 / 下一步**:
- 等待用户将正式执行令发给 `NPC` 与 `farm`。
- 收到执行结果后继续做：
  - 共享根目录释放判定
  - `root-workdir lease / branch occupancy` 规则化补齐

### 会话 5 - 2026-03-17（首轮执行结果回流后的二次裁定）
**用户目标**:
> 吸收 `NPC` 与 `farm` 的实际执行结果，判定谁已经收口、谁还没收口，并直接给出下一轮动作。

**本轮子任务**:
- 核验 `NPC` 的最小救援是否已经成立。
- 核验 `farm` cleanroom 的编译阻断究竟是执行失误还是治理配方缺口。
- 形成下一轮更精确的执行令。

**已完成事项**:
1. 核验 `NPC` 成果：
   - 独立现场：`D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`
   - 当前 tip 已到 `28aef95d16f176ac5538bbd93c73769b2c686a8c`，其中 `f7a1c0f5` 是业务救援提交，`28aef95d` 仅为治理记忆同步
   - `FarmToolPreview.cs` 已从 NPC 线剔除
   - 编译通过，且 `001/002/003` 的 Prefab / Sprite / 动画 / roamProfile 抽查通过
2. 核验 `farm` 阻断：
   - cleanroom 编译错误真实存在，不是误报
   - 错误集中在 `FarmToolPreview.cs` 与 `GameInputManager.cs` 的接口闭环
   - 通过对比 `07ffe199 -> 11e0b7b4` 的源码，确认治理侧原先给的回放配方少带了 `11e0b7b4` 版 `FarmToolPreview.cs` 的后续 farm-only 改动
3. 形成二次裁定：
   - `NPC` 业务线本身已恢复，但当前 worktree 仍有 4 个无关 TMP 字体资源 dirty，不能直接宣称“整个工作树完全 clean”
   - `farm` cleanroom 需要第二轮修正，不得现在接替污染分支

**关键决策**:
- 共享根目录继续冻结，暂不释放。
- `NPC` 的下一步不是继续救火，而是因为那 4 个 TMP dirty 已明确归属 `spring-day1`，所以直接新起 continuation worktree，避免继续借用 rescue 现场。
- `farm` 的下一步不是推翻 cleanroom，而是修正白名单目标版本：`FarmToolPreview.cs` 应以 `11e0b7b4` 为准。

**恢复点 / 下一步**:
- 对外给 `NPC` 一条“收口后整理现场”的跟进令。
- 对外给 `farm` 一条“第二轮 cleanroom 修正版执行令”。

### 会话 6 - 2026-03-17（NPC rescue worktree 的 4 个 dirty 归属已锁定）
**用户目标**:
> 明确 `NPC` rescue worktree 里 4 个 TMP 字体资源 dirty 到底是谁的，避免后续继续模糊归属。

**已完成事项**:
1. 用户补充了 `spring-day1` 现场证据，确认以下 4 个文件属于 `spring-day1`：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
2. 由此锁定：`NPC` rescue worktree 当前 remaining dirty 的来源不是 NPC 业务，而是 `spring-day1` 的共享资源改动。
3. 补读线程记忆后进一步确认：`spring-day1` 的 [memory_0.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/spring-day1/memory_0.md) 在 `2026-03-13` 的补记里，已经把 `Primary.unity` 与多套 TMP 字体资产标成 spring-day1 保护对象；这说明归属并非临时猜测，而是历史链路一致。

**关键决策**:
- `NPC` 不应碰这 4 个文件，也不需要为它们做清理提交。
- `NPC` 的正确续航动作是从 `28aef95d16f176ac5538bbd93c73769b2c686a8c` 新起 continuation 现场，把当前 rescue worktree 降级为阶段性收口现场。

**恢复点 / 下一步**:
- 后续给 `NPC` 的 prompt 可以直接要求其按方案 A 落地，不再讨论 B。
