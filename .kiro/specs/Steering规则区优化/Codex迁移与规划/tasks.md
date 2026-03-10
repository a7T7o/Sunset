# Codex迁移与规划 - 任务列表

**创建日期**: 2026-03-07

---

## 任务列表

- [x] T1: 阅读 `CLAUDE.md` 与 `Claude迁移与规划` 核心文档
- [x] T2: 阅读 `.kiro/steering` 与 Hook 机制关键文档
- [x] T3: 提炼 Kiro / Claude / Codex 的能力边界差异
- [x] T4: 创建 `Codex迁移与规划` 子工作区基础四件套
- [x] T5: 形成《Codex工作流与技巧手册》初版
- [ ] T6: 用后续真实 Codex 会话验证手册是否足够稳定
- [ ] T7: 评估是否需要上提为项目级 Codex 指南
- [ ] T8: 若上提，设计最小化同步策略，避免与 Steering 双源漂移
- [x] T9: 输出 Unity MCP 候选对比文档
- [x] T10: 创建工作区路由 Skill
- [x] T11: 创建场景安全审视 Skill
- [x] T12: 创建锐评路由 Skill
- [x] T13: 为 History 交接目录建立总索引
- [x] T14: 创建 Unity 验证闭环 Skill
- [x] T15: 输出 Unity MCP 迁移试装方案

---

## T1: 阅读 `CLAUDE.md` 与 `Claude迁移与规划` 核心文档

**状态**: ✅ 完成

**说明**:
- 已抽读 `CLAUDE.md`、`全面理解报告.md`、`工作流反思与整改方案_2026-03-06.md` 等关键文档。
- 已确认 Claude 迁移的主结论：排序器定位、hooks 降级、memory 收尾必做。

---

## T2: 阅读 `.kiro/steering` 与 Hook 机制关键文档

**状态**: ✅ 完成

**说明**:
- 已阅读 `rules.md`、`workspace-memory.md`、`规则优先级分析.md`、`hook事件触发README.md`。
- 已确认 Kiro 的优势是原生 Hook 语义，而不是单纯 shell 脚本。

---

## T3: 提炼 Kiro / Claude / Codex 的能力边界差异

**状态**: ✅ 完成

**说明**:
- 已明确哪些能力可以迁移到 Codex，哪些只能转为人工 SOP 或显式交接。

---

## T4: 创建 `Codex迁移与规划` 子工作区基础四件套

**状态**: ✅ 完成

**说明**:
- 已创建 `requirements.md`、`design.md`、`tasks.md`、`memory.md`。

---

## T5: 形成《Codex工作流与技巧手册》初版

**状态**: ✅ 完成

**说明**:
- 已输出面向 Codex 的执行协议、工具映射、Memory 收尾 SOP 与实操技巧清单。

---

## T6: 用后续真实 Codex 会话验证手册是否足够稳定

**状态**: ⏳ 待执行

**说明**:
- 需要在未来真实工程任务中验证手册是否能持续降低漂移与遗漏。

---

## T7: 评估是否需要上提为项目级 Codex 指南

**状态**: ⏳ 待执行

**说明**:
- 若后续多轮会话都证明这套手册稳定，再考虑是否上提到仓库级说明文件。

---

## T8: 若上提，设计最小化同步策略，避免与 Steering 双源漂移

**状态**: ⏳ 待执行

**说明**:
- 若真的上提，只保留“排序、SOP、工具边界”，不重复抄写 Steering 业务规则正文。

---

## T9: 输出 Unity MCP 候选对比文档

**状态**: ✅ 完成

**说明**:
- 已新增 `Unity-MCP候选对比_2026-03-07.md`，对比当前 MCP 与 `CoplayDev/unity-mcp`、`IvanMurzak/Unity-MCP`、`nurture-tech/unity-mcp-server`、`notargs/UnityNaturalMCP`。
- 已明确当前优先级：先继续完成可直接落地的 skill / 索引工作，再在后续阶段并行试装新的 Unity MCP。

---

## T10: 创建工作区路由 Skill

**状态**: ✅ 完成

**说明**:
- 已在用户技能目录创建 `sunset-workspace-router`，用于定位活跃工作区、必读文件顺序和 memory 追加层级。

---

## T11: 创建场景安全审视 Skill

**状态**: ✅ 完成

**说明**:
- 已在用户技能目录创建 `sunset-scene-audit`，固化场景/Prefab/Inspector 修改前的五段式审视流程。

---

## T12: 创建锐评路由 Skill

**状态**: ✅ 完成

**说明**:
- 已在用户技能目录创建 `sunset-review-router`，固化 Sunset 项目中 `锐评/审视报告/Diff` 的 A/B/C 路由流程。

---

## T13: 为 History 交接目录建立总索引

**状态**: ✅ 完成

**说明**:
- 已新增 `History/2026.03.07-Claude-Cli-历史会话交接/总索引.md`，统一标记主线、对应工作区与接手优先级。

---

## T14: 创建 Unity 验证闭环 Skill

**状态**: ✅ 完成

**说明**:
- 已在用户技能目录创建 `sunset-unity-validation-loop`，固化 Sunset 项目的 Unity 验证顺序、MCP 失败分类与手动 Play 验收边界。

---

## T15: 输出 Unity MCP 迁移试装方案

**状态**: ✅ 完成

**说明**:
- 已新增 `Unity-MCP迁移试装方案_2026-03-10.md`，针对 Unity `6000.0.62f1` 现场给出候选顺序、并行试装策略、验证矩阵与通过标准。
