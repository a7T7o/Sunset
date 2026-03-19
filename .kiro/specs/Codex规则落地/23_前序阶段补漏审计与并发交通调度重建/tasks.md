# 阶段任务：前序阶段补漏审计与并发交通调度重建

## 阶段目标
- 全面回看 `01-12、20-22` 的正文、任务板和记忆，确认哪些阶段真的完成、哪些只是复选框没回填、哪些待办必须显式迁移或封板。
- 在不推翻 `20-22` 已有闸机成果的前提下，补出用户真正要的“并发交通系统”：
  - 可排队
  - 可等待
  - 中断不丢进度
  - 多 checkpoint 持续推进
  - Unity / MCP 单实例下的写入调度
- 防止新阶段错误假设“基础已经全部做好”，也防止旧阶段继续用过时任务板误导后续判断。

## A. 前序阶段全面审计
- [x] 逐阶段复核 `01-12、20-22` 的 `tasks.md / memory.md / live 事实`
- [x] 对每个阶段给出统一分类：
  - 已完成封板
  - 已完成但任务板未回填
  - 有非阻塞 backlog
  - 真正未完且需迁移
- [x] 形成一份前序阶段审计结论文档
- [x] 对 `20/21/22` 补写审计补记，明确已完成、迁移与仍在 scope 的内容
- [x] 对 `05/07/08/09` 的历史遗留做“非阻塞 / 后续再议 / 迁移”裁定

## B. 阶段边界与入口纠偏
- [x] 明确 `22` 与 `23` 的边界：
  - `22` 负责恢复开发的发放 / 回收 / 串行准入运营
  - `23` 负责并发调度模型与前序阶段补漏审计
- [x] 更新根层工作区记忆，避免继续把 `single-writer 串行` 误写成终局模型
- [ ] 如有必要，补写 `Codex规则落地` 根层入口说明，指明当前治理主线已转入 `23`

## C. 并发交通模型重建
- [x] 定义 shared root 的“写入槽位 / 排队 / 等待 / 归还”模型
- [x] 定义线程切不进去时的标准动作：
  - 继续只读
  - 补文档
  - 准备下一个 checkpoint
  - 维持 branch carrier 不丢进度
- [x] 定义并落地稳定 launcher，避免 live 调度命令受旧任务分支脚本漂移影响
- [ ] 定义多 checkpoint 持续推进模型：
  - checkpoint 触发条件
  - 归还条件
  - 再次申请准入的条件
- [ ] 定义 Unity / MCP 单实例下的调度边界：
  - Git 层可做什么
  - Unity 写态何时必须独占
  - Play Mode / Edit Mode 交接口径

## D. 验证与落地
- [x] 产出新的并发交通调度文档或运行口径
- [x] 设计至少一轮“等待态线程 + 获准线程”并发验证路线
- [x] 明确哪些动作可并发，哪些只能排队
- [x] 将当前 live 调度入口统一切到稳定 launcher，并回写现行文档
- [x] 明确治理线程之后如何发放本轮批次入口与收件回执，避免重复再造固定群发模板

## 当前裁定
- `23` 是当前新的治理主线。
- `22` 继续承担恢复开发运营，不再独自背并发调度模型重建。
- 旧阶段如果存在未回填或未迁移项，必须先在 `23` 里审计澄清，再决定是否回补旧任务板。

## 完成标准
- [ ] 前序阶段没有“看起来未完、其实已迁移”的误导性任务板残留
- [ ] `22` 与 `23` 的边界被明确写死
- [ ] 并发交通调度模型形成正式正文，而不再只存在于聊天纠偏里
- [ ] 新阶段不会再错误假设旧基础已全部完工

## 2026-03-19｜queue 运行时兼容性修补与最小验证补记
- [x] 修复 `request-branch` / queue runtime 读取在当前 PowerShell 上因 `ConvertFrom-Json -Depth` 直接报错的问题
- [x] 将 queue runtime 空文件 / 坏 JSON 的失败口径收紧为显式 `FATAL`
- [x] 在当前治理 dirty 场景下完成一次最小实测：
  - `git-safe-sync.ps1 -Action request-branch -OwnerThread "导航检查" -BranchName "codex/navigation-audit-001" -CheckpointHint "docs-ready" -QueueNote "stage23-validation"`
  - 返回 `STATUS: LOCKED_PLEASE_YIELD`
- [x] 验证 queue runtime 会落到 `.kiro/locks/active/shared-root-queue.lock.json`，且不会污染 tracked Git 现场
- [x] 清空本轮验证生成的测试票据，避免把假 `waiting` 留给后续真实调度
- [x] 继续补 `waiting -> granted -> task-active -> completed/cancelled` 的完整状态流与负例矩阵
- [x] 设计真正的 queue 消费、取消、唤醒与回执动作
- [x] 增加 queue runtime 自愈，修补旧任务分支脚本造成的 stale `task-active / granted`
- [x] 完成一轮 Git 层实盘演习，并在演习后把 runtime queue 恢复为空基线
- [x] 增加仓库外稳定 launcher，固定从 `main` 读取 canonical `git-safe-sync.ps1`
- [x] 用稳定 launcher 完成一轮最小实测，并把结果回写到阶段 23
- [x] 发现 shared root 当前被 `Skills和MCP` 线的治理 dirty 阻断后，按根层协议新建一轮“清场优先”批次分发文件、线程专属 prompt 与固定回收卡
- [x] shared root 清场完成后，生成一轮 `queue-aware` 业务线程 live 准入批次，并明确 `spring-day1` 延后到下一轮 `Unity/MCP-aware` 准入
