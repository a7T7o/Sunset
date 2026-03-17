# memory 分卷治理与索引收紧 - 开发记忆

## 模块概述
- 本阶段负责治理 Sunset 当前三层超长记忆：活文档工作区、父治理工作区、Codex 治理线程。
- 目标不是重写历史，而是把旧长卷安全归档，并把活跃卷收成可继续接力的精简入口。

## 当前状态
- **完成度**: 10%
- **最后更新**: 2026-03-16
- **状态**: 进行中

## 会话记录

### 会话 1 - 2026-03-16

**用户需求**:
> 继续做，不要停；在不打扰 `farm` 业务现场的前提下，把你已经发现的治理续办继续推进，尤其是当前超长 `memory`、代办结构和规则补强问题。

**完成任务**:
1. 确认“超长 `memory` 分卷治理”应作为新的独立阶段进入 `000_代办/codex`，而不是回灌到旧阶段 `tasks.md`。
2. 新建 `06_memory分卷治理与索引收紧/`，并建立本阶段 `tasks.md` 与 `memory.md`。
3. 固定本阶段范围：三层超长 `memory` 分卷、分卷约定补强、三层记忆链同步。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\06_memory分卷治理与索引收紧\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\06_memory分卷治理与索引收紧\memory.md`

**关键决策**:
- 这轮分卷治理属于新的治理阶段，必须单独建账。
- 工作区记忆与线程记忆的分卷命名不完全相同，后续必须在现行规则里写清。

**验证结果**:
- `000_代办/codex` 当前已存在 `06_memory分卷治理与索引收紧/` 阶段目录。
- 本阶段已具备可继续执行的 `tasks.md + memory.md`。

**遗留问题**:
- [ ] 仍需完成三层超长 `memory` 的实际分卷。
- [ ] 仍需把分卷约定补入现行规则文档。

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\06_memory分卷治理与索引收紧\tasks.md` | 本阶段执行清单 |
| `D:\Unity\Unity_learning\Sunset\.kiro\steering\workspace-memory.md` | 工作区记忆分卷上位规则 |
| `D:\Unity\Unity_learning\Sunset\AGENTS.md` | Sunset 项目级线程与记忆路由规则 |

### 会话 2 - 2026-03-16（三层分卷与规则补丁完成）

**用户需求**:
> 继续做，不要停下来；把现在最安全且最值钱的治理动作继续真实落地。

**完成任务**:
1. 将三层超长记忆完成真实分卷：
   - 活文档工作区：`memory.md -> memory_0.md + 新 memory.md`
   - 父治理工作区：`memory.md -> memory_3.md + 新 memory.md`
   - 线程记忆：`memory_0.md -> memory_1.md + 新 memory_0.md`
2. 将工作区 / 线程的分卷差异补入：
   - `Sunset工作区四件套与代办规范_2026-03-16.md`
   - `基础规则与执行口径.md`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
3. 按顺序同步本阶段、`000_代办/codex` 根层、活文档工作区、父治理工作区与线程记忆。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory_3.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_1.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset工作区四件套与代办规范_2026-03-16.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\基础规则与执行口径.md`
- `D:\Unity\Unity_learning\Sunset\AGENTS.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\06_memory分卷治理与索引收紧\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\06_memory分卷治理与索引收紧\memory.md`

**验证结果**:
- 三份旧长卷行数分别为：
  - 活文档工作区 `memory_0.md`：`1879` 行
  - 父治理工作区 `memory_3.md`：`1407` 行
  - 线程历史卷 `memory_1.md`：`1286` 行
- 三份新活跃卷当前分别为：
  - 活文档工作区 `memory.md`：`53` 行
  - 父治理工作区 `memory.md`：`53` 行
  - 线程活跃卷 `memory_0.md`：`54` 行
- 规则文档已明确写清：
  - 工作区活跃卷固定为 `memory.md`
  - 线程活跃卷固定为 `memory_0.md`

**关键决策**:
- 线程记忆不能照抄工作区记忆的命名方式；线程活跃卷必须保留 `memory_0.md` 固定入口。
- 这轮分卷完成后，后续不允许再继续无边界往三份旧长卷叠写。

**遗留问题**:
- [ ] 后续仍需在真实推进中持续执行“写入前检查活跃卷长度”的纪律。
- [ ] 仍需继续处理历史错码 / 编码健康巡检，但这已不阻塞本阶段收口。

### 会话 3 - 2026-03-16（下一批超长卷候选盘点）

**完成任务**:
1. 对 `.kiro/specs` 与 `.codex/threads` 下的 `memory*.md` 进行了全量行数扫描。
2. 记录当前最值得下一批处理的超长卷候选，避免后续继续靠印象判断。

**候选结果**:
- 最高风险历史卷：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\memory_0.md`：`2297` 行
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.01\10.1.4补丁004\memory_0.md`：`1354` 行
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\memory_0.md`：`989` 行
- 仍在高风险区的活跃卷：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\SO设计系统与工具\memory.md`：`861` 行
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0_背包交互系统升级\memory.md`：`820` 行
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\1_背包V4飞升\memory.md`：`811` 行
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\2026.02.11智能加载\memory.md`：`701` 行
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`：`670` 行

**关键决策**:
- 下一批应优先处理“仍在活跃写入中的超长主卷”，而不是继续优先碰已经沉睡的历史卷。
- 当前三层分卷完成后，`SO设计系统与工具` 与 `UI系统` 应成为下一批最值得治理的活跃候选。

### 会话 4 - 2026-03-16（codex 根层主卷顺手完成分卷）

**完成任务**:
1. 复核发现 `000_代办/codex/memory.md` 在本轮追加后已到 `294` 行。
2. 立即将其归档为 `memory_0.md`，并重建新的精简 `memory.md`，避免这轮分卷治理出现“刚做完就再次超线”的反向失控。
3. 同步把本阶段目标从“三层”修正为“四层关键治理记忆”。

**关键决策**:
- 根层治理主记忆也必须遵守精简入口纪律，不能因为它是总代办入口就例外。
- 当前阶段到此为止可以视为完整闭环，不再需要继续往本阶段堆叠新动作。
