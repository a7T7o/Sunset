# 阶段任务：memory 分卷治理与索引收紧

## 目标
- 把当前关键治理 `memory` 从“继续硬扛”收回到“历史卷 + 精简活跃卷”的健康结构。
- 明确工作区记忆与线程记忆的分卷差异，避免后续继续写出 1000 行以上主卷。
- 让 `000_代办/codex` 对本轮分卷治理有独立阶段承接，而不是把动作散落回旧阶段。

## 待办
- [x] 建立本阶段目录并固定任务范围。
- [x] 盘点并分卷以下超长记忆：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
- [x] 为新活跃卷补齐分卷索引、承接摘要、当前恢复点。
- [x] 将线程记忆与工作区记忆的分卷约定补入现行规则文档。
- [x] 按“子工作区 -> 父工作区 -> 线程记忆”补写本轮分卷治理记录。

## 完成标准
- 四层关键治理 `memory` 均不再继续直接叠写旧长卷。
- 工作区活跃卷统一恢复为精简 `memory.md`。
- 线程活跃卷保留固定入口 `memory_0.md`，历史内容转入增量旧卷。
- 当前活文档中已经写清楚“什么时候该分卷、怎么命名、怎么承接”。
