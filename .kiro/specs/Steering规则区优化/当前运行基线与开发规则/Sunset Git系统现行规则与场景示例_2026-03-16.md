# Sunset Git 系统现行规则与场景示例（2026-03-16）

## 1. 当前默认模型
- shared root：
  - `D:\Unity\Unity_learning\Sunset @ main`
- 治理类改动：
  - 默认留在 `main`
- 业务类改动：
  - 默认也留在 `main` 语义下推进
  - 完成一刀后，按白名单直接 `sync`
- `worktree`：
  - 只作例外，不作日常
- 当前 live 模型应理解为：
  - `main-only + whitelist-sync + exception-escalation`

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
4. 如果没有命中高危撞车、Unity / MCP 单写冲突、branch carrier 迁入窗口，就继续留在 `main`
5. 完成后执行：
   - `git-safe-sync.ps1 -Action sync -Mode task -OwnerThread <线程名> -IncludePaths ...`

### 场景 C：branch carrier / worktree 例外窗口
1. 只在这些情形进入：
   - branch carrier 迁入 `main`
   - 高危隔离
   - 特殊实验
   - 独立施工 worktree（如 `scene-build`）
2. 这时才继续走：
   - `request-branch`
   - `grant-branch`
   - `ensure-branch`
   - `return-main`

### 场景 D：shared root 不在 `main`
1. 先判断是不是合法占用
2. 未裁定前只允许只读或治理取证
3. 不直接把它当中性现场继续写业务

### 场景 E：当前任务要用 Unity / MCP
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
- 当前允许业务实现直接按白名单收在 `main`
- 但不会放宽高危撞车判断
- 不会允许你把别人的改动一起夹带进去
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
- 先从 shared root 读现场，普通开发默认 `main + whitelist-sync`，只有少数例外才升级到 `codex/...` 或 worktree；进入 Unity / MCP 前还要再过单实例层。
