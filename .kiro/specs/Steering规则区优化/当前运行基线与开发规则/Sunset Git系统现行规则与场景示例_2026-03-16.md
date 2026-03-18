# Sunset Git 系统现行规则与场景示例（2026-03-16）

## 1. 当前默认模型
- shared root：
  - `D:\Unity\Unity_learning\Sunset @ main`
- 治理类改动：
  - 留在 `main`
- 业务类改动：
  - 从 shared root 读取现场后切到对应 `codex/...`
- `worktree`：
  - 只作例外，不作日常

## 2. 四层边界
- shared root 占用层：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
- A 类热文件锁层：
  - `.kiro/locks/active`
- Unity / MCP 单实例层：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
- Git 白名单同步层：
  - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`

## 3. 场景示例

### 场景 A：改治理文档
1. 留在 `main`
2. 更新工作区 `memory`
3. 更新线程 `memory`
4. 用 `git-safe-sync.ps1 -Action sync -Mode governance -IncludePaths ...`

### 场景 B：改业务代码
1. 从 shared root 读取现场
2. 先过 `sunset-startup-guard`
3. 必要时查锁
4. 执行 `ensure-branch`
5. 在 `codex/...` 上改动并 task 模式同步

### 场景 C：shared root 不在 `main`
1. 先判断是不是合法占用
2. 未裁定前只允许只读或治理取证
3. 不直接把它当中性现场继续写业务

### 场景 D：当前任务要用 Unity / MCP
1. 先确认 Git 现场是否允许进入
2. 再核：
   - `mcp-single-instance-occupancy.md`
   - `mcp-hot-zones.md`
3. 如果命中 Play / Compile / Domain Reload / 共享场景热区：
   - 先停写
   - 先重新读 Editor / Console / 当前分支
   - 必要时只读取证并记入 `mcp-single-instance-log.md`

## 4. 白名单不是免责卡
- 白名单只决定“本轮带哪些文件”
- 不会放宽分支规则
- 不会允许你把业务实现直接收在 `main`
- 仍然禁止无边界 `git add -A`

## 5. 当前与治理工作区的关系
- 正式治理工作区：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`
- TD 镜像区：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex`
- 结论：
  - Git 规则、skills/AGENTS、四件套、阶段清盘，都进入正式工作区推进
  - 不再把 `000_代办/codex` 当工作区

## 6. 一句话执行口径
- 先从 shared root 读现场，再决定是 `main + governance` 还是 `codex/... + task`；进入 Unity / MCP 前还要再过单实例层；任何时候都不要把 `worktree` 当默认路径。
