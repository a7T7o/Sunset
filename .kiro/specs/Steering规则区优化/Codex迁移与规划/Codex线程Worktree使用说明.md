# Codex 线程 Worktree 使用说明

## 1. 这份说明解决什么
- 告诉你：以后 `Sunset` 的不同线程该进入哪个目录。
- 告诉你：进入线程后第一件事应该检查什么。
- 告诉你：如果目录或分支不对，应该先修哪里。
- 明确当前红线：本轮只完成线程 / worktree 固化，不提前执行 NPC 合并到 `main`。

## 2. 当前固定目录

| 用途 | 目录 | 默认分支 |
|------|------|---------|
| 稳定主路 / 治理主路 | `D:\Unity\Unity_learning\Sunset` | `main` |
| NPC 线程目录 | `D:\Unity\Unity_learning\Sunset_worktrees\NPC` | `codex/npc-generator-pipeline` |
| 农田线程目录 | `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002` | `codex/farm-10.2.2-patch002` |

## 3. 以后怎么进入线程

- 我已经把当前活跃 `Sunset` 线程在 `Codex` 状态层中的默认 `cwd` 对齐到对应目录。
- 如果某个线程因为已打开太久，暂时还显示旧目录，优先关闭后重开该线程，或重新从目标目录进入。

### A 类：治理 / 总览 / 审计线程
- 例如：`Codex规则落地`、`spring-day1`、`项目文档总览`、`导航检查`、`遮挡检查`
- 这些线程默认都应在：
  - `D:\Unity\Unity_learning\Sunset`
  - 分支：`main`

### B 类：功能实现线程
- `NPC` 线程默认应在：
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
  - 分支：`codex/npc-generator-pipeline`
- `农田交互修复V2` 线程默认应在：
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`
  - 分支：`codex/farm-10.2.2-patch002`

## 4. 每次进入线程后的固定核验

### 第一步：看当前目录
- 先确认当前工作目录是不是该线程的默认目录。
- 不要先看 UI 底部提示；先看实际目录。

### 第二步：看真实分支
- 运行：`git branch --show-current`
- 只认它返回的真实结果。

### 第三步：如果目录或分支不对
- 不要直接继续改代码。
- 先把线程切回正确工作目录，再重新核验分支。
- 如果还不对，再检查 `git worktree list`。

## 5. 最容易混淆的地方
- 线程不是分支。
- UI 里的历史分支提示不是当前真实分支。
- 看到 NPC 工具只在 `NPC` 目录里，不代表它丢了；这恰恰说明 worktree 隔离已经生效。
- 在 `main` 根目录看不到 NPC 工具，是当前阶段的正常现象。

## 6. 当前不允许做的事
- 不允许提前把 `codex/npc-generator-pipeline` 合并到 `main`
- 不允许让功能线程继续共用根目录 `D:\Unity\Unity_learning\Sunset`
- 不允许只看 UI 提示就开始做实现

## 7. 如果你只想快速判断自己现在在哪
- 看目录：`Get-Location`
- 看分支：`git branch --show-current`
- 看全部 worktree：`git worktree list`

## 8. 验收时你应该看到什么
- 根目录 `D:\Unity\Unity_learning\Sunset` 保持 `main`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC` 存在并指向 `codex/npc-generator-pipeline`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002` 存在并指向 `codex/farm-10.2.2-patch002`
- `.codex/threads/线程分支对照表.md` 能明确告诉你哪个线程该去哪

## 9. 后续动作边界
- 本轮完成后，只说明“线程与 worktree 已固定”。
- 是否让 NPC 功能回到 `main`，等单独验收通过后再谈 merge。
