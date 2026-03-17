# mcp-single-instance-log

## 文件身份
- 本文件是 Sunset 的 Unity / MCP 单实例冲突日志。
- 用法：
  - 记录已经发生过的冲突事实
  - 记录裁定后的固定口径
  - 不拿它替代线程 `memory` 或工作区 `memory`

## 记录模板
- 日期：
- 背景线程：
- 当时工作目录 / 分支 / `HEAD`：
- 触发动作：
- 冲突信号：
- 裁定：
- 后续固定规则：

## 记录 1 - 2026-03-17（单实例层正式建立）
- 日期：`2026-03-17`
- 背景线程：`Codex规则落地 / NPC-farm-spring-day1 并行复盘`
- 当时工作目录 / 分支 / `HEAD`：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `952a1f23`
- 触发动作：
  - 复盘 NPC、farm、spring-day1 对 Unity / MCP 并行安全的判断
- 冲突信号：
  - Play / Compile / Domain Reload 会让 MCP 读写拿到中间态或失效对象
  - 单实例 Editor 下，读操作也不应假设天然稳定
  - Git 现场安全与 Unity / MCP 现场安全是两层不同问题
- 裁定：
  - 为 Sunset 新增独立的 Unity / MCP 单实例占用层与热区层
- 后续固定规则：
  - 进入 Unity / MCP 读写前，必须先核：
    - `mcp-single-instance-occupancy.md`
    - `mcp-hot-zones.md`
  - 以后如再次出现 MCP 冲突，先追加这里，再同步到相关工作区与线程 `memory`
