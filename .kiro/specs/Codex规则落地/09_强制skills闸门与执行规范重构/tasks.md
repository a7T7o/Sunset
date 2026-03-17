# 阶段任务：强制 skills 闸门与执行规范重构

## 阶段目标
- 把 Sunset 当前“有规则但容易被跳过”的问题，收成真正的项目级启动闸门。

## 已完成
- [x] 建立 `sunset-startup-guard`
- [x] 更新 Sunset `AGENTS.md`
- [x] 将 `NPC/farm` 分支漂移事故纳入闸门需求
- [x] 完成真实回合验证：
  - 主线锚定
  - `cwd / branch / HEAD` 核验
  - shared root 占用校验
  - 首条 commentary 结构约束
- [x] 为 shared root 占用单独落盘：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md`
- [x] 为 Unity / MCP 单实例层单独落盘：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`
- [x] 统一线程回复最小结构
- [x] 确认当前 session 与后续新线程都可稳定看到 `sunset-startup-guard`
- [x] 把 `worktree` 只作例外机制写入现行规则与 skills
- [x] 完成至少一轮真实治理回合验证

## 当前裁定
- 本阶段已完成。

## 完成标准
- [x] 项目级启动闸门可读可用
- [x] AGENTS 已写成硬规则
- [x] shared root 占用已被吸收入闸门体系
- [x] 线程回复最小结构已形成统一口径
