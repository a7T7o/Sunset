# Sunset/UnityMCP转CLI 线程记忆

## 2026-04-04｜compile-first P0 CLI 落地，当前已合法停车

- 用户目标：
  - 不再停在研究判断，而是先交一版能直接服务高频“编译 / 看错误 / 避免红错”的 `sunset-mcp` CLI。
- 当前主线目标：
  - 先把 `UnityMCP -> CLI` 落成 Sunset 内部 orchestration CLI 的 compile-first P0，而不是优先做低频 live 验收或通用产品化。
- 本轮实际做成了什么：
  1. 新增 `D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py`
  2. 将 `D:\Unity\Unity_learning\Sunset\scripts\sunset-mcp.ps1` 改成极薄 Python wrapper
  3. 已落地命令：
     - `baseline`
     - `status`
     - `doctor`
     - `errors`
     - `compile`
     - `no-red`
     - `recover-bridge`
  4. `compile / no-red` 已能串起：
     - `CodexCodeGuard`
     - `refresh_unity(mode=force, scope=scripts, compile=request)`
     - `read_console`
  5. 已加 shared-root 防噪音保护：
     - 不给路径且 changed `.cs` 过多时直接阻断，要求显式传路径或明确 `--all-changed`
- 关键决策：
  - 当前正确优先级是“compile gate truth first”，不是“先把 play/menu/control 面做全”
  - `compile` 的成功语义当前定为：
    - own 代码闸门通过
    - 没有 own compile red
    - 即使还有 external red，也先诚实报成 `external_red`
  - `no-red` 的成功语义更严格：
    - own red = 0
    - external red = 0
- 验证结果：
  - `baseline` 通过
  - `errors` 成功返回 fresh Console
  - 对 `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs` 运行 `compile / no-red` 时，工具已正确判定：
    - own `CodeGuard` 通过
    - Unity 当前仍有 external compile red
    - 结论应是 `external_red`，不是把外部红错误算成目标脚本 own 面
  - `python -m py_compile` 与 `git diff --check` 均通过
- 当前阻塞 / 未完成：
  - 还未迁入：
    - `instance ensure`
    - `play / stop / menu / route`
  - 还未正式接回 `validate_script`
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 下一步恢复点：
  - 继续时直接在这版脚本上扩：
    1. 控制类命令
    2. `manage_script / validate_script`
    3. 更细的 owned / external compile 分类与输出摘要

## 2026-04-04｜只读快测补记：当前 fresh 红错数 = 14，且全部是 external red

- 用户目标：
  - 快速确认 Sunset 当前 fresh compile 后到底还有多少个红错。
- 本轮实际做成了什么：
  1. 未进入真实施工，只做只读快测；
  2. 通过 `sunset_mcp.py no-red Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 100` 跑了一次最小 fresh compile + console 链。
- 当前稳定事实：
  - 当前 Console 红错 = `14`
  - 工具判定 = `external_red`
  - own 红错 = `0`
  - 红错集中在：
    - `Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`
- 当前恢复点：
  - 下一步若继续排红，应直接优先接这组 test compile red，而不是先回头怀疑 `SpringDay1PromptOverlay.cs` 自己。

## 2026-04-04｜对典狱长的正式汇报已生成

- 用户目标：
  - 先不要继续完善工具功能，而是先给典狱长一份完整说明，讲清这条线当前完成了什么、解决了什么需求、最终要做成什么，以及现在已经有哪些产出。
- 本轮实际做成了什么：
  1. 已在 `Codex规则落地` 工作区生成正式汇报文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-04_给典狱长_UnityMCP转CLI当前阶段与产出汇报_01.md`
  2. 报告里已明确说明：
     - 当前主线是 compile-first / no-red first / Python first
     - 当前已解决的关键需求是：
       - 快速查红
       - own / external red 分离
       - changed `.cs` 过多时主动拦噪音
       - 减少 PowerShell 参与
     - 最终目标不是大而全 CLI，而是“每次改代码后都能轻量确认 fresh compile red”的高频开发动作
     - 下一刀不该只补控制命令，必须把 `validate_script + 资源护栏 + 失败回收` 一起上
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 下一步恢复点：
  - 现在可以直接把这份汇报转给典狱长；
  - 若典狱长放行继续，再回到 CLI 线本身继续施工。

## 2026-04-04｜典狱长续工要求已完成：`validate_script` + 资源护栏 + 失败回收 已落地

- 用户目标：
  - 按典狱长文件继续真实施工，但唯一主刀固定为“爆红规范高频 CLI 落地”，只补：
    - `validate_script`（或等价单命令入口）
    - 资源护栏
    - 超时/输出上限
    - 失败回收
  - 不碰控制命令面。
- 本轮实际做成了什么：
  1. 在 `sunset_mcp.py` 新增正式命令：
     - `validate_script`
  2. 这条命令当前能明确输出：
     - `no_red`
     - `own_red`
     - `external_red`
     - `unity_validation_pending`
     - `blocked`
  3. 这轮补齐的资源护栏：
     - timeout 上限 `60s`
     - wait 上限 `90s`
     - Console 条数上限 `200`
     - 输出条目上限 `20`
  4. 这轮补齐的失败回收：
     - subprocess timeout 明确退出
     - MCP / Unity 不可用时落 `unity_validation_pending`
     - 范围过宽时落 `blocked`
     - 本命令不启动额外后台进程
  5. 修正了一个关键语义问题：
     - `CodeGuard` warning 不再等价成 red
- 真实验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 100 --output-limit 5`
    - `assessment = external_red`
    - own red = `0`
    - external red = `3`
  - `validate_script --count 50`
    - `assessment = blocked`
    - 原因：当前 changed `.cs` = `82`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --skip-mcp --count 20 --output-limit 5`
    - `assessment = unity_validation_pending`
  - `python -m py_compile`：通过
  - `git diff --check`：通过
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 下一步恢复点：
  - 若继续，优先判断是否要把 Unity 侧原生 `manage_script / validate_script` 参数面接回来；
  - 在这件事没判断清前，不先漂去 `play / stop / menu / route`。

## 2026-04-04｜A 方案已完成：最小参数面对齐落地并已合法停车

- 用户目标：
  - 只做 Unity 原生 `manage_script / validate_script` 与当前 CLI 的最小稳定参数面对齐，不扩到控制命令面。
- 当前主线目标：
  - 回答“这条高频 CLI 是否值得接回原生参数面，以及最小该接哪层”。
- 本轮实际做成了什么：
  1. `validate_script` 新增 `--name / --path / --level`
  2. 新增 `manage_script` 窄边界入口，只支持：
     - `validate`
     - `get_sha`
  3. `validate_script` 结果里新增 `manage_script_compat`：
     - 原生 `manage_script(action=validate)` 的 target / level / diagnostics 会被明确回显
     - compile-first 的 `assessment` 仍然独立，不让 native strict warning 偷改红错语义
  4. `--skip-mcp` 时也会诚实写清：
     - `native_validation = skipped`
     - `skip_reason = skip_mcp`
  5. 旧的“直接传脚本路径”入口未回归
- 关键决策：
  - 当前 live 口径里没有单独注册的 `validate_script` Unity 自定义工具；真实原生面是 `manage_script(action=validate)`
  - 因此这轮不去伪造一个大而全 `validate_script` 兼容层，而是只接回最值钱的：
    - `name`
    - `path`
    - `level`
    - `validate / get_sha`
- 验证结果：
  - `validate_script --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --count 20 --output-limit 5`
    - `assessment = external_red`
    - `native_validation = warning`
  - `validate_script --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --skip-mcp --count 20 --output-limit 5`
    - `assessment = unity_validation_pending`
    - `native_validation = skipped`
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level strict --output-limit 5`
    - `status = failed_on_warnings`
  - `manage_script get_sha --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --output-limit 5`
    - 成功返回当前文件 SHA
  - `py_compile` / `git diff --check`
    - 均通过
- 当前阻塞 / 未完成：
  - 未 sync
  - 未扩控制命令面
  - `scripts/sunset_mcp.py` / `scripts/sunset-mcp.ps1` 当前在 `git status` 下仍是 untracked，own 路径不 clean
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 下一步恢复点：
  - 若继续，只该补 help / 文档或继续收紧输出摘要；
  - 不该跳去 `play / stop / menu / route`。

## 2026-04-04｜help / doctor 收尾已补齐，当前已到可 sync 状态

- 用户目标：
  - 用户明确要求不要停在“这轮做完了”的口头状态，而是继续把这条线补到真正可交付。
- 本轮实际做成了什么：
  1. 为 `sunset_mcp.py` 补了顶层与子命令 help：
     - 顶层 examples
     - `validate_script --help`
     - `manage_script --help`
  2. 把 `doctor` 口径改成当前真实状态：
     - `validate_script` 已落地
     - 原生兼容优先走 `--name / --path / --level`
     - `manage_script` 只开放 `validate|get_sha`
  3. 首次 `Ready-To-Sync` 被 own roots 同根残留阻断后，已按脚本要求扩大 expected sync paths，并再次通过：
     - 当前状态 = `READY_TO_SYNC`
- 当前验证结果：
  - `--help`、`doctor`、`manage_script get_sha`、`validate_script --skip-mcp`
    - 均已复测成立
  - `py_compile` / `git diff --check`
    - 仍通过
- 当前阻塞 / 未完成：
  - 只剩白名单 sync 尚未执行
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑并通过
  - `Park-Slice`：尚未补最终停车
  - 当前状态：`READY_TO_SYNC`
- 下一步恢复点：
  - 直接执行白名单 sync；
  - sync 后立刻 `Park-Slice`，结束这条切片。
