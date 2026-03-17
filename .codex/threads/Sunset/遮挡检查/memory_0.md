# 遮挡检查 - 线程记忆

## 线程概述

本线程用于对 Sunset 遮挡系统做只读审计，目标是把线程旧稿、历史文档和当前真实实现重新对齐，形成后续整改前的可接手基线。

## 当前状态

- 线程分组：`Sunset`
- 当前主线目标：核实现有遮挡系统的真实实现、风险与旧资料漂移，并沉淀一份 Codex 视角的分析
- 当前工作目录：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`main`
- 最后更新：2026-03-15

## 会话记录

### 会话 1 - 2026-03-15

**用户目标**：
- 彻底阅读线程目录内容，核实与遮挡系统相关的当前代码、场景、Prefab、测试与文档，并在 `.codex/threads/Sunset/遮挡检查/1.0.0初步检查` 下新增一份 Codex 视角分析文件，同时按规则补齐工作区与线程 memory。

**当前主线目标**：
- 对遮挡系统建立“旧稿结论 vs 当前真实实现”的证据化基线，避免后续整改继续沿用已经漂移的前提。

**本轮子任务 / 阻塞**：
- 用户指定的 Sunset 子工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查` 原本不存在，需要先补建工作区记忆，再把本轮结论按“子工作区 -> 父工作区 -> 线程记忆”的顺序落盘。

**已完成事项**：
1. 读取线程目录既有内容：
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/memory.md`
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/遮挡系统代码索引与调用链.md`
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/遮挡-导航-命中一致性与性能风险报告.md`
2. 读取路由/规则与历史背景：
   - `.kiro/steering/README.md`
   - `.kiro/steering/rules.md`
   - `.kiro/steering/workspace-memory.md`
   - `.kiro/steering/documentation.md`
   - `.kiro/steering/scene-modification-rule.md`
   - `.kiro/steering/systems.md`
   - `.kiro/steering/trees.md`
   - `.kiro/steering/layers.md`
   - `.codex/threads/线程分支对照表.md`
   - `.kiro/specs/云朵遮挡系统/*`
   - `Docx/分类/遮挡与导航/*`
   - `Docx/分类/树木/*`
   - `Docx/分类/交接文档/树林遮挡核心系统优化交接文档.md`
3. 核实现有核心代码、Editor、测试、场景、Prefab 与 `.meta`。
4. 使用 Unity MCP 复核：
   - `Primary.unity` 当前已加载且未 dirty
   - Console 有 1 条 NPC 编辑器工具 warning
   - `OcclusionManager = 1`、`OcclusionTransparency = 44`、`TreeController = 20`
   - `OcclusionSystemTests` EditMode 11/11 通过
5. 补建 / 更新记忆文件：
   - `.kiro/specs/999_全面重构_26.03.15/遮挡检查/memory.md`
   - `.kiro/specs/999_全面重构_26.03.15/memory.md`
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/memory.md`
6. 新增分析文件：
   - `.codex/threads/Sunset/遮挡检查/1.0.0初步检查/03_遮挡现状核实与差异分析（Codex视角）.md`

**关键决策**：
- 这是 A 类审计线程，继续留在根目录 `main`，不进入功能分支。
- 当前只做核实与记忆沉淀，不擅自修改场景、Prefab、Inspector。
- 旧线程旧稿只能作为线索，不能直接当现状真相；必须以当前代码、Prefab、场景和 Unity 现场为准。

**关键结论**：
- 旧稿中的 GUID 映射写反，正确映射为：
  - `OcclusionManager.cs.meta` -> `4a3f1c967ec52c946a301ffbacd49b6c`
  - `OcclusionTransparency.cs.meta` -> `9b41652a450cc9447abb94ac5ce72c1a`
  - `DynamicSortingOrder.cs.meta` -> `45a3d1d4bd5a8744aa9ed982679f875a`
- 当前场景参数与旧资料存在明显漂移：`rootConnectionDistance = 1.5`、`useOcclusionRatioFilter = 1`、`minOcclusionRatio = 0.4`、`enableSmartEdgeOcclusion = 1`。
- 三棵树 Prefab 都存在父/子双 `OcclusionTransparency`；`TreeController` 只控制当前节点组件，因此树苗/树桩阶段的遮挡禁用规则可能被父组件绕开。
- `OcclusionManager` 以 `OcclusionTransparency` 组件而不是“物理树”为单位做注册和树林判定，单棵树就可能因双组件命中 `overlappingTreeCount >= 2` 的保底逻辑。
- 点击交互与工具命中使用不同边界标准，当前体感一致性没有被保证。
- 像素采样在树上是真实开启的；当前测试和编辑器批处理工具都落后于真实实现。

**验证结果**：
- `Primary.unity` 当前 `isDirty = false`
- Console 当前仅见 1 条 NPC 编辑器工具过时 API warning
- `OcclusionSystemTests` 11/11 通过，但覆盖不足以证明当前主链安全

**恢复点 / 下一步**：
- 主线已经恢复到“遮挡系统现状基线已建立”的阶段。
- 如果后续继续推进，应优先进入整改设计，而不是继续补更多旧资料摘要。
- 整改设计的第一步建议是先处理树 Prefab 双组件粒度问题，再谈树林保底逻辑、命中判定统一和测试补强。

### 会话 2 - 2026-03-16

**用户目标**：
- 将本线程冻结快照写入 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总`，文件名按线程名命名，并保持冻结，不恢复开发。

**当前主线目标**：
- 在不继续推进遮挡整改的前提下，交出当前线程现场快照，供全局排期、锁裁决和后续恢复使用。

**本轮子任务 / 阻塞**：
- 这是治理汇总支撑动作，不是新的业务主线；当前真正阻塞点是 A 类共享热文件的锁目录尚未落地，而且已有未持锁 dirty。

**已完成事项**：
1. 只读复核当前分支、`HEAD`、Git dirty、Unity 活动场景与 Console。
2. 确认 `D:\Unity\Unity_learning\Sunset\.kiro\locks\` 当前不存在。
3. 将冻结快照文档写入：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\遮挡检查.md`
4. 同步更新：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.03.16冻结文档汇总\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\遮挡检查\memory_0.md`

**关键事实**：
- 本轮未新增代码、场景、Prefab、资源修改，只做只读复核与冻结文档落盘。
- `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 与 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs` 当前已存在未持锁 dirty。
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs` 当前未出现在 dirty 清单，但仍属于共享热文件观察对象。

**恢复点 / 下一步**：
- 当前主线已切到“保持冻结并等待统一排期”状态。
- 冻结解除前，不再自行恢复共享热文件相关开发；后续先等 owner / 锁状态裁决，再决定是否进入遮挡整改设计。

### 会话 3 - 2026-03-16

**用户目标**：
- 解冻后先核对当前工作目录 / 分支 / `HEAD` / dirty，说明当前持有的文档重组 / 删除 / 未跟踪内容，先固化本轮遮挡审计成果，再判断后续是否进入代码整改阶段并给出最小目标。

**当前主线目标**：
- 先把 `遮挡检查` 的审计成果和阶段口径固定下来，再决定是否开后续整改。

**本轮子任务 / 阻塞**：
- 这轮不是直接写代码，而是做“阶段口径澄清”。当前主要阻塞点是：现场已从 2026-03-15 的 `main` 漂移到 `codex/npc-main-recover-001`，且工作树混入大量与 `遮挡检查` 无关的变更。

**已完成事项**：
1. 复核当前现场：
   - 工作目录 `D:\Unity\Unity_learning\Sunset`
   - 当前分支 `codex/npc-main-recover-001`
   - 当前 `HEAD` `b9b6ac48`
   - 当前 Git dirty 含治理重组、线程文档重组和 NPC 恢复内容
2. 复核当前 `遮挡检查` 文档状态：
   - 根目录旧文档被标记删除
   - `1.0.0初步检查` 子目录中审计文档仍存在
   - 子工作区此前只有 `memory.md`
3. 新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查\审计成果固化与阶段口径.md`

**关键结论**：
- 当前更稳的口径是“审计基线已建立，但还没有正式进入代码整改执行”。
- 后续若进入整改阶段，最小目标应先锁定树双 `OcclusionTransparency` 粒度问题，而不是直接大范围修改遮挡主链。

**恢复点 / 下一步**：
- 当前主线恢复到“审计成果已固化，等待是否进入整改设计的决策”。
- 如果继续推进，下一步应先决定是否在当前混合工作树上开展遮挡整改；若不合适，应先切回更干净的遮挡专用现场。

### 会话 4 - 2026-03-17

**用户目标**：
- 只做只读核查，确认本线程后续是否需要独立 worktree、是否接受“共享根目录 + 分支”模式，以及给出最合理的 branch-only 开工入口。

**当前主线目标**：
- 先把 `遮挡检查` 后续整改的进入姿势讲清楚，再决定何时真正开工。

**本轮子任务 / 阻塞**：
- 这是路由澄清，不是实现；本轮阻塞不是技术理解，而是要避免把“当前偶然站在某个 `codex/...` 分支”误读成线程默认入口已经改变。

**已完成事项**：
1. 只读复核：
   - `.codex/threads/线程分支对照表.md`
   - `Sunset当前唯一状态说明_2026-03-16.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `.kiro/steering/git-safety-baseline.md`
2. 明确当前线程后续默认入口：
   - 不需要独立 worktree
   - 接受“共享根目录 + 分支”模式
   - branch-only 开工入口应回到 `D:\Unity\Unity_learning\Sunset @ main`，再视整改需要切到新的 `codex/...`

**关键结论**：
- `遮挡检查` 不属于当前必须依赖 worktree 的核心线程。
- 后续如果进入实际整改，默认模式应是共享根目录 + 新任务分支，而不是先开独立 worktree。
- 推荐分支入口名：`codex/occlusion-remediation-001`。

**恢复点 / 下一步**：
- 当前主线已具备明确的 branch-only 路由口径。
- 如果后续开整改，先回到 `main` 做 `preflight`，确认现场干净度和热文件占用，再创建新的遮挡整改分支。
