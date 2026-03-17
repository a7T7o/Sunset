# 分析：共享根仓库分支漂移与现场占用治理
## 1. 问题定义
- 这次事故不是“没有分支”或“没有锁”，而是共享根目录 `D:\Unity\Unity_learning\Sunset` 的真实 Git 上下文没有被当作显式共享资源管理。
- 结果是：线程语义已经切到 `NPC`，但物理现场仍停在 `farm` 分支，后续提交就顺着错误上下文落到了 `farm`。

## 2. 已确认事实
### 2.1 共享根目录真实现场
- 工作目录：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`codex/farm-1.0.2-correct001`
- 当前 `HEAD`：`11e0b7b4`

### 2.2 已确认的 worktree 结构
- `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002 @ codex/farm-10.2.2-patch002`
- `D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier @ codex/main-reflow-carrier`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC @ codex/npc-generator-pipeline`
- `D:\Unity\Unity_learning\Sunset_worktrees\spring-day1-story-progression-001 @ codex/spring-day1-story-progression-001`

### 2.3 关键 checkout 轨迹
- `main -> codex/farm-1.0.2-correction001`
- `codex/farm-1.0.2-correction001 -> codex/npc-main-recover-001`
- `codex/npc-main-recover-001 -> codex/farm-1.0.2-correct001`
- `codex/farm-1.0.2-correct001 -> codex/npc-roam-phase2-001`
- `codex/npc-roam-phase2-001 -> codex/farm-1.0.2-correct001`

### 2.4 关键提交归属
- `07ffe199`：`mixed`
- `18f3a9e1`：`NPC`
- `11e0b7b4`：`farm`
- 上述三次提交当前都只挂在 `codex/farm-1.0.2-correct001`

## 3. NPC 与 farm 的交叉证词
### 3.1 farm 已确认
- 共享根目录当前仍属于 `farm` 占用现场。
- `b9b6ac48` 是 cleanroom 最合适的干净起点。
- `07ffe199` 中除 [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs) 外，没有其他文件应保留在 farm。
- `18f3a9e1` 不应继续留在 farm。
- `11e0b7b4` 属于 farm cleanroom 应回放的主体提交。
- 第一轮 cleanroom 实测已证明：只回放 `07ffe199` 版本的 [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs)，再叠加 `11e0b7b4` 的其余 7 个代码文件，会在编译期产生接口不闭环。
- 具体证据：
  - `GameInputManager.cs@11e0b7b4` 已调用 `PromoteToExecutingPreview(layerIndex, cellPos)` 与 `RemoveExecutingPreview(layerIndex, cellPos)`。
  - `FarmToolPreview.cs@07ffe199` 仍只提供旧签名与旧集合类型，导致 cleanroom 出现 `CS1501 / CS1503 / CS0104`。
- 因此，farm cleanroom 的最终目标文件口径应以 `11e0b7b4` 版 [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs) 为准；此前“只带 07ffe199 版该文件”的配方已被实测否定。

### 3.2 NPC 已确认
- 唯一救援基线是 `codex/npc-roam-phase2-001 @ f6b4db2f`。
- `f6b4db2f` 已包含 NPC 核心资产、Prefab、meta 和运行时代码。
- 不该继续留在 NPC 线的是 [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs) 的 farm 侧改动。
- [NPCPrefabGeneratorTool.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPCPrefabGeneratorTool.cs) 以 `f6b4db2f` 版本为准。

## 4. 根因归纳
- A 类热文件锁机制解决的是“谁能写某个高风险文件”。
- 这次事故暴露的是另一层问题：共享根目录当前 checkout 到哪个分支、由谁占用、是否允许别的线程继续借用这个现场，并没有被强制核验。
- 所以不能把“分支”粗暴并入文件锁；正确做法是补一层新的共享资源治理：
  - `root-workdir lease`
  - 或 `branch occupancy`

## 5. 当前临时裁定
### 裁定 A：共享根目录进入业务写入冻结
- `D:\Unity\Unity_learning\Sunset` 当前只允许：
  - 只读取证
  - 治理线程写治理文档与记忆
- 不允许：
  - `NPC`
  - `farm`
  - 其他业务线程
  在此目录继续做新的业务提交

### 裁定 B：NPC 不从共享根目录继续推进
- `NPC` 后续基线固定为 `codex/npc-roam-phase2-001 @ f6b4db2f`
- 先做最小收口：
  - 剔除 NPC 线里不应背着的 farm 改动
  - 再验证 NPC 资产 / Prefab / 动画引用链

### 裁定 C：farm 不沿污染分支继续开发
- `farm` 不再继续沿 `codex/farm-1.0.2-correct001` 写新提交。
- 正确方向是从 `b9b6ac48` 启动 cleanroom，按白名单重放真正的 farm 代码与正文文档。

## 6. 对规则层的直接影响
- 以后进入任何 Sunset 实质性任务，不仅要查热文件锁，还必须强制核验：
  - 当前工作目录
  - 当前分支
  - 当前 `HEAD`
  - 当前目录是否已被其他线程显式占用
- 这项校验应被纳入新的“Sunset 启动闸门 skill”，而不是继续依赖口头纪律。

## 7. 当前一句话结论
- 这次事故不是锁失效，而是“共享根工作目录的分支上下文没有被纳入强制治理”；当前最安全的收口方式是：共享根目录冻结、NPC 走现有救援线、farm 走 cleanroom。

## 8. 最新执行级结论
- `NPC` 已在独立救援现场完成最小收口：
  - 工作目录：`D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`
  - 分支：`codex/npc-roam-phase2-001`
  - 新 `HEAD`：`f7a1c0f562a476febe50084124dbeee382d31ac9`
  - 结论：NPC 线内误带入的 farm 改动已剔除，编译通过，Prefab / 动画 / Sprite / roamProfile 抽查通过
- 但 `NPC` 当前 worktree 仍有 4 个无关 TMP 字体资源 dirty，说明“NPC 分支已恢复”不等于“该 worktree 已绝对 clean”。
- `farm` 已在独立 cleanroom 进入第一轮重建，但尚未可接替污染分支：
  - 工作目录：`D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
  - 分支：`codex/farm-1.0.2-cleanroom001`
  - 当前 `HEAD`：`b9b6ac4881f4436abbc1f3232f14706ca76bb869`
  - 结论：边界干净，但代码接口尚未闭环，仍需第二轮 cleanroom 修正
