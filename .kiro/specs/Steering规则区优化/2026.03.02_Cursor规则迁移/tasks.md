# 2026.03.02_Cursor规则迁移 - 任务列表

## 阶段 1：现状固化（本轮会话为主）

- [ ] 1.1 整理 `.kiro/steering` 与 Hook 现状摘要  
      - 基于 `rules优先级分析.md` / `README.md` / `hook事件触发README.md`，在《迁移计划》中写出当前规则与 Hook 架构的简要综述。
- [ ] 1.2 整理 Cursor 侧现状  
      - 记录现有 `.cursor/rules/rules.md` 的内容与加载策略，说明目前尚未使用 Skills/Subagents/Commands 的状态。
- [ ] 1.3 补齐本子工作区三件套  
      - 完成 `requirements.md` / `design.md` / `tasks.md` 初版，并在本子工作区 `memory.md` 中记录。

## 阶段 2：Steering → Cursor 映射设计

- [ ] 2.1 建立“Steering 规则 → Cursor 配置”映射表  
      - 至少覆盖 P-1 / P0 / P1 / P2 规则，标明各自在 Cursor 侧对应的：Rule / Skill / Subagent / 无需镜像。
- [ ] 2.2 定义 Cursor Rules 的拆分与命名原则  
      - 例如是否需要在 `.cursor/rules/` 下拆出 `workspace-memory.md` 镜像文件，还是保持单一 `rules.md`。
- [ ] 2.3 明确 Memory 与 TD 在迁移过程中的责任边界  
      - 规范迁移期间如何在主工作区与本子工作区以及 000_代办 中记录待办与执行结果。

## 阶段 3：Skills 与 Subagents 方案设计（规划级）

- [ ] 3.1 设计 2–3 个高价值 Skills 草案  
      - 示例：`specs-triple-file-generator`、`td-sync-from-memory`、`steering-audit-skill`，在《迁移计划》中写出输入/输出/步骤/约束。
- [ ] 3.2 设计 1–2 个 Subagents 草案  
      - 示例：`save-system-auditor`、`hook-architecture-reviewer`，说明其工作区范围、使用模式与 memory 责任。
- [ ] 3.3 为未来的 Commands 预留命名与触发规范  
      - 例如约定 `/sync-memory`、`/scan-td`、`/gen-phase-doc` 的参数形式和行为边界。

## 阶段 4：demo 级 Hook 试点（不改正式 Hook）

- [ ] 4.1 设计并在《迁移计划》中记录 1–2 个 demo Hook 方案  
      - 要求：`userTriggered` 类型、文件名带 `-demo` 或“测试”字样、只调用只读/弱副作用的 Skills。
- [ ] 4.2 明确 demo Hook 与正式 Hook 的隔离策略  
      - 包含：命名规范、启用/禁用方式、如何在 memory 中标注它们是实验性配置。

## 阶段 5：同步与收尾

- [ ] 5.1 在 `Steering规则区优化/memory.md` 中追加本子工作区的索引与本轮会话摘要（先子后主）。  
- [ ] 5.2 如有需要，在 `.kiro/specs/000_代办/` 下为“Cursor规则迁移”补充 TD 条目。  
- [ ] 5.3 审视本子工作区文档，整理“后续可执行的落地步骤”，为下一轮真正开始修改 Cursor 配置与新增 Skills/Subagents 做准备。  

