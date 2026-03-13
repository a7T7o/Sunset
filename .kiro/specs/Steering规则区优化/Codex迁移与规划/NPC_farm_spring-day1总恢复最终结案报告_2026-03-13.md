# NPC / farm / spring-day1 总恢复最终结案报告（2026-03-13）

## 1. 最终结论
- `NPC` 已回正轨。
- `farm` 已回正轨。
- `spring-day1` 已补齐最后两个增强脚本，回到可按原完成进度继续开发的状态。
- 用户现在默认只打开 `D:\Unity\Unity_learning\Sunset` 就可以继续正常开发。

## 2. 本轮最终执行
- `DialogueUI.cs` 从 `codex/restored-mixed-snapshot-20260311` 白名单恢复到当前主项目工作树。
- `DialogueManager.cs` 从 `codex/restored-mixed-snapshot-20260311` 白名单恢复到当前主项目工作树。
- 当前工作树直接回读已检出：
  - `DialogueUI.cs`：`CanvasGroup`、`CurrentCanvasAlpha`、`IsCanvasInteractable`、`IsCanvasBlockingRaycasts`、输入推进增强、调试可观测接口。
  - `DialogueManager.cs`：`PauseTime`、`ResumeTime`、`ForceCompleteOrAdvance`、`CompleteCurrentNodeImmediately`。
- `SpringDay1_FirstDialogue.asset`、`DialogueDebugMenu.cs`、`NPCDialogueInteractable.cs` 持续在主项目工作树中。
- `NPC/farm` 线程锚点保持在 `D:\Unity\Unity_learning\Sunset@main`，未发生倒退。

## 3. 默认开发规范
- 默认开发现场：`D:\Unity\Unity_learning\Sunset`
- 默认工作流：主项目优先
- 小改动：允许直接在当前主项目推进；提交时必须白名单，不混无关 dirty。
- 独立分支：当改动跨度大、风险高、需要独立提交链时再开 `codex/` 分支。
- worktree：只保留为故障修复、高风险隔离、特殊实验，不再作为默认日常开发现场。

## 4. Git 承载链
- 当前实际开发基线：`D:\Unity\Unity_learning\Sunset@main`
- 当前可推送基线：`codex/main-reflow-carrier`
- 两者分工明确：
  - `main` 继续承担本地真实开发现场；
  - `codex/main-reflow-carrier` 承担干净、可推送的恢复承载链。
- 当前唯一剩余 Git 问题不是恢复阻断，而是：本地 `main` 的历史链仍带超大 SQLite 文件，不能直接推送到远端。

## 5. 保护对象
- 本轮继续保护、未混入恢复提交：
  - `Assets/000_Scenes/Primary.unity`
  - 五套中文 TMP 字体资产 dirty
  - 其他线程无关 memory / dirty / backup-script
