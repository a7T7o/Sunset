# 线程完成后 白名单 main 收口模板

你这一轮不要继续扩写新内容，直接进入自己的白名单 `main` 收口。

## 默认原则
1. 只提交你自己修改的内容
2. 只提交你自己的白名单路径
3. 一次只收一刀自己的 checkpoint
4. 不替别的线程解释 dirty，不替别的线程收口
5. 默认执行纪律是：`一刀一收`

## 如果你碰过热文件
- 只要这轮碰过 `Scene / Prefab / Primary.unity / 热 Unity 资源`，收口回执必须额外补：
  - 我留了什么
  - 保留什么
  - 清掉什么
  - 当前是否已对我这条线 clean

## 如果你这轮包含资源文件
- 只要白名单里有 `.unity / .prefab / .asset`，收口前必须自己先分类：
  - `有效内容`
  - `自动副产物`
  - `调试残留`
  - `他线脏改`
- 自动副产物默认不得混入这次 checkpoint，除非你明确说明它是本轮必须保留的有效改动

## 如果你这轮改了 `.cs`
- `git-safe-sync.ps1` 收口前会自动触发代码闸门
- 代码闸门会先做：
  - 目标文件 UTF-8 检查
  - `git diff --check`
  - 程序集级编译检查
- 代码闸门不过，就不能继续收口
- 代码闸门通过，也只代表“代码层 / 文本层暂未见阻断”，不等于 Unity 红错已经验收完成

## 如果你要做 Unity / MCP live 验证或 live 写
- 先不要直接进，先说明：
  - 需要什么实例
  - 最多占多久
  - 只做什么
  - 做完退回什么状态

## 如果你要对外说“无红错 / 可交接 / 可直接提交”
- 只要本轮改动触及运行时代码、scene、prefab、asset、UI、剧情、交互、输入或管理器链，就必须同时补：
  - 默认先给 CLI 侧的 fresh recompile
  - 默认先给 CLI 侧的 fresh console
  - CLI 覆盖不到时，再补 direct MCP / live 取证，或明确写 `live 待验证`
- `validate_script`、`CodexCodeGuard`、`git diff --check` 不得单独充当“红错验收完成”
- Sunset 当前默认顺序是：`CLI first, direct MCP last-resort`
- 如果 CLI 与 direct MCP / Unity 当前不可用，只能老实写：
  - `代码闸门通过`
  - `Unity 红错验证未闭环`
  - `live / console 待补`
- 如果你对外要写任何同义的 `无红错 / 可交接 / 可直接提交` 结论，技术审计层还必须原样补一张 `No-Red 证据卡 v2`，至少包含：
  - `cli_red_check_command`
  - `cli_red_check_scope`
  - `cli_red_check_assessment`
  - `unity_red_check`
  - `mcp_fallback`
  - `mcp_fallback_reason`
  - `current_owned_errors`
  - `current_external_blockers`
  - `current_warnings`
- `cli_red_check_assessment` 只能直接沿用 CLI 原值：
  - `no_red`
  - `own_red`
  - `external_red`
  - `unity_validation_pending`
  - `blocked`
- 如果 `mcp_fallback != not-needed`，`mcp_fallback_reason` 只允许：
  - `baseline_fail`
  - `unity_validation_pending`
  - `blocked`
  - `scene_live_flow_required`
  - `playmode_required`
  - `inspector_required`
- 缺任一项 `No-Red 证据卡 v2`，或只写“通过 / 没问题”却不给命令和 assessment，就视为“日志不可判定”。

## 只有这些情况才不要自己直接收口
- 撞到同一个高危目标
- 同一个 Scene / Prefab / 热脚本被多人改
- 跨线程强耦合
- 你的成果还在 branch carrier / worktree，尚未迁入 `main`

## 回执落点
- 不要让项目经理手动再建文件
- 直接把最终回执追加到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`

## 你回我只要这些
- 实际提交到 `main` 的路径
- 提交 SHA
- `hot_file_hygiene: pass / fail / not-applicable`
- `asset_hygiene: pass / fail / not-applicable`
- `code_self_check: pass / fail / not-applicable`
- `pre_sync_validation: pass / fail / not-run`
- `unity_red_check: pass / blocked / live-pending / not-required`
- `cli_red_check_command`
- `cli_red_check_scope`
- `cli_red_check_assessment: no_red / own_red / external_red / unity_validation_pending / blocked`
- `mcp_fallback: not-needed / used / unavailable`
- `mcp_fallback_reason: baseline_fail / unity_validation_pending / blocked / scene_live_flow_required / playmode_required / inspector_required`
- `current_owned_errors`
- `current_external_blockers`
- `current_warnings`
- 当前 `git status` 是否 clean
- blocker_or_checkpoint
- 一句话摘要

## 一句话口径
- 默认主流程不是等治理线程代提，而是线程自己按白名单收自己这一刀；治理线程只在少数例外时介入。
