# 12_治理工作区归位与彻底清盘 - 工作区记忆

## 当前主线目标
- 把 Sunset 的治理正文工作区正式迁到 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`。
- 把 `000_代办/codex` 降级为只负责记录和读取 TD 的镜像层。
- 清掉所有不会影响当前 branch carrier 的历史外部容器与文档尾项。

## 当前状态
- 完成度：100%
- 最后更新：2026-03-17
- 状态：已完成

## 会话记录

### 会话 1 - 2026-03-17（治理工作区归位与清盘）
**用户目标**：
> 不要再把 `000_代办/codex` 当工作区；正式工作区必须回到 `.kiro/specs/` 下；把能直接完成的遗留全部一次性做完，并厘清 `Sunset_external_archives` 与 `Sunset_backups` 是否需要清理。

**完成事项**：
1. 读取 `skills-governor`、`sunset-workspace-router`、`sunset-startup-guard` 与 Sunset 项目 `AGENTS.md`，确认本轮属于 Sunset 治理工作区级重构。
2. 核验 shared root 真实现场：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `git worktree list --porcelain` 仅剩 shared root
   - 工作树 clean
3. 将原 `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex` 中的：
   - `01-11` 阶段目录
   - 根层 `memory.md`
   - 根层 `memory_0.md`
   迁移至 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`
4. 新建本工作区根层 `design.md`，明确“正式工作区 / 代办镜像 / steering 活入口”的职责边界。
5. 新建 `12_治理工作区归位与彻底清盘/` 阶段目录，并写入本 `tasks.md / design.md / memory.md`。
6. 在 `000_代办/codex` 重建 TD-only 结构，不再保留工作区正文。
7. 新建 `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`，把 shared root 占用层独立落盘。
8. 统一修正当前 live 文档、项目 AGENTS 与本地 skills 的治理入口路径与 worktree 口径。
9. 形成 `Sunset_external_archives` 与 `Sunset_backups` 的退役说明，并删除两个目录。
10. 对 `01/02/03/07/09/10/11` 的剩余文档尾项做最终裁定与封板。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\**`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex\**`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
- `D:\Unity\Unity_learning\Sunset\AGENTS.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\**`
- `C:\Users\aTo\.codex\skills\sunset-workspace-router\SKILL.md`
- `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md`
- `C:\Users\aTo\.codex\skills\sunset-startup-guard\references\checklist.md`

**验证结果**：
- shared root 仍为 `main`
- `git worktree list --porcelain` 仅剩 shared root
- `000_代办/codex` 已不再承载阶段正文
- `Sunset_external_archives` 与 `Sunset_backups` 已被裁定为退役历史容器

**恢复点 / 下一步**：
- 本阶段已完成。
- 后续治理如继续推进，应在 `Codex规则落地` 的新阶段目录中继续，而不是回退到 `000_代办/codex`。

### 会话 2 - 2026-03-17（真实执行仓库外历史容器删除）
**用户目标**：
> 不只要说明，还要把仓库外两个历史目录真的处理干净。

**完成事项**：
1. 新建 [外部历史容器退役说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/12_治理工作区归位与彻底清盘/外部历史容器退役说明_2026-03-17.md)。
2. 真实删除：
   - `D:\Unity\Unity_learning\Sunset_external_archives`
   - `D:\Unity\Unity_learning\Sunset_backups`
3. 复核结果：
   - 两个目录都已不存在。

**关键决策**：
- 从现在开始，历史文档里若再出现这两个路径，只能按“历史曾存在、现已退役”理解。

**恢复点 / 下一步**：
- `12` 阶段至此彻底完成；后续不再回到本阶段继续补尾巴。

## 关键决策

| 决策 | 原因 | 日期 |
|---|---|---|
| 正式治理工作区迁到 `Codex规则落地` | `000_代办` 只能做代办镜像，不能继续做正文工作区 | 2026-03-17 |
| `000_代办/codex` 只保留 TD 镜像 | 修正“代办区兼任工作区”的结构性错误 | 2026-03-17 |
| 外部 `archives/backups` 直接退役删除 | 当前 branch carrier 已稳定承载正式内容，外部目录只剩历史证据意义 | 2026-03-17 |

## 一句话恢复点
- 治理工作区归位已经完成；后续任何 Sunset 治理续办都应从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地` 继续，而不是回到 `000_代办/codex`。

### 会话 3 - 2026-03-17（12 阶段收官后复核：终局结构保留，实时占用改写为真相）
**用户目标**：
> 在不重开 `12` 阶段的前提下，确认治理提交是否真的落在 `main`，并把 shared root 当前是否仍属中性入口写成真实口径。

**完成事项**：
1. 复核治理提交 `ece0c0ea` 已在 `main`，且 `main...origin/main` 无 ahead / behind。
2. 复核 `git worktree list --porcelain` 仍只剩 shared root，`Sunset_external_archives` 与 `Sunset_backups` 继续不存在。
3. 发现 shared root 当前虽然在 `main`，但 working tree 仍带其他线程 dirty，主体是 NPC 业务文件并夹杂少量非治理记忆。
4. 因此改写：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-17.md`
   把 “`main` 且中性” 修正为 “默认模型已恢复，但实时现场仍需 preflight”。

**关键决策**：
- `12` 阶段的结构性清盘已经完成，不需要重开。
- 但 live 状态文档必须说真话：shared root 当前不是“无脑可切分支”的干净现场。

**恢复点 / 下一步**：
- `12` 继续保持完成状态。
- 后续治理重点回到 `09` 的执行面，而 shared root 的实时中性判断交给占用文档和 preflight。
