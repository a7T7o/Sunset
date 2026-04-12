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

## 2026-04-04｜已按用户要求生成给典狱长的最终裁定 prompt

- 用户目标：
  - 如果当前切片已经全部完成，就直接给典狱长写一份 prompt。
- 本轮实际做成了什么：
  1. 已确认当前锁死切片完成；
  2. 已生成文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-04_给典狱长_UnityMCP转CLI参数面对齐已完成请裁定收口_04.md`
  3. prompt 已明确要求典狱长：
     - 在 `无需继续发` 与 `停给用户分析 / 审核` 之间裁定
     - 不要再给这条线继续发施工 prompt
- 当前 thread-state：
  - 生成 prompt 后应直接合法停车，不再继续施工。

## 2026-04-05｜CLI 与 MCP 依赖关系已澄清：当前是“本地护栏壳 + MCP live 能力层”

- 用户目标：
  - 先确认 Sunset 现在这版 CLI 是否必须配合 MCP 启动，还是可以完全独立存在。
- 当前主线目标：
  - 继续围绕 `UnityMCP -> CLI` 高频爆红链路，厘清架构边界，不把“CLI”误解成“完全脱离 Unity live 的离线工具”。
- 本轮实际确认的事实：
  1. `sunset_mcp.py` 默认端点就是 `http://127.0.0.1:8888/mcp`，当前主实现首先是一个 MCP 客户端壳。
  2. `baseline` / `doctor` 这类命令可以在没有可用 MCP 会话时独立运行，因为它们主要读本地配置、端口、pidfile 与建议口径。
  3. `status` / `errors` / `manage_script` / `validate_script` / `compile` / `no-red` 都依赖 live MCP；没有 MCP bridge 时，这些命令不能诚实给出 fresh console / Unity live 验证结论。
  4. `compile` / `no-red` / `validate_script` 虽然支持 `--skip-mcp`，但这只是保留代码层护栏；结果会停在 `unity_validation_pending`，不等于真正独立完成红错验证。
- 关键决策：
  - 当前 Sunset CLI 可以独立“存在”，但不能独立“完成高频 Unity 爆红验证”。
  - 如果未来要做更独立的 CLI，正确方向不是把现有 live 命令硬说成离线可用，而是明确拆成：
    - 本地壳层
    - live MCP 能力层
    - 可选离线兜底层
- 验证依据：
  - 只读核对了 `scripts/sunset_mcp.py` 当前实现与命令定义，未改代码。
- 当前恢复点 / 下一步：
  - 后续若继续推进这条线，讨论重点应放在“哪些命令必须 live，哪些能离线降级”，而不是笼统问 CLI 要不要存在。

## 2026-04-05｜`validate_script` 卡顿根因与最小修正已落地

- 用户目标：
  - 解释为什么真实场景里 `validate_script` 会卡很久、无输出、最后被外层 `124` 杀掉，并优先修正最关键的护栏失真。
- 当前主线目标：
  - 继续把 `UnityMCP -> CLI` 压到真正适合高频 no-red 工作流，而不是停在“命令存在但体验发闷”。
- 本轮实际做成了什么：
  1. 坐实快慢分层：
     - `baseline / doctor / errors / manage_script validate` 当前都在约 `0.3s ~ 0.5s`
     - `validate_script` 的卡点不在 MCP，而在前置 `CodexCodeGuard`
  2. 直接验证：
     - 单独跑 `CodexCodeGuard.dll` 对 `SpringDay1PromptOverlay.cs` 的检查，`90s` 仍超时
  3. 读源码确认根因：
     - `CodexCodeGuard` 会扫 tracked `.cs`、dirty/untracked，并对受影响程序集做 current / baseline 两轮 Roslyn 编译
     - `sunset_mcp.py` 原先对 `codeguard()` 仍是 `timeout=600`
  4. 已改 `scripts/sunset_mcp.py`：
     - `codeguard()` 改为继承 CLI `--timeout-sec`
     - compile/no-red/validate_script 阶段新增 `stderr` phase 输出
  5. 新实测：
     - `py -3 scripts/sunset_mcp.py --timeout-sec 5 validate_script --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --count 5 --output-limit 3`
       - 约 `5.3s` 返回
       - `assessment=blocked`
       - `message=subprocess_timeout:dotnet:5s`
       - `stderr` 显示 `phase=codeguard timeout=5s targets=1`
     - `manage_script validate` / `errors` 仍秒回
- 关键决策：
  - 这轮先不大改命令面，只先修“资源护栏没有包住最重阶段”这个明显 bug。
  - 下一刀如果继续，应优先考虑把高频快路径和重型 codeguard 证明分层，而不是继续把所有语义硬塞进同一条 `validate_script`。
- 当前阻塞 / 未完成：
  - full `validate_script` 仍受 `CodexCodeGuard` 过重影响；只是现在会在 CLI timeout 内诚实 blocked，而不是黑盒长挂。
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 若继续这条线，下一步最值钱的是“快路径 compile/error”与“重型 codeguard”拆层，避免高频动作继续被程序集级检查绑死。

## 2026-04-05｜客观成本对照已完成：CLI 有轻微开销，但当前真正重的是 heavy validate_script

- 用户目标：
  - 要一份最客观、最真实的分析：在 Sunset 当前高频 no-red 场景里，`CLI + MCP` 到底是不是在加重负担；如果慢，具体慢在哪里。
- 当前主线目标：
  - 继续判断 `UnityMCP -> CLI` 对 Sunset 是不是值得保留的高频基础设施路线，但这次只按现场成本与收益说话。
- 本轮实际做成了什么：
  1. direct MCP 原始 HTTP 实测：
     - initialize：`0.015s`
     - `manage_script validate`：`0.076s`
     - `read_console`：`0.204s`
  2. thin CLI 实测：
     - `manage_script validate`：`0.451s`
     - `errors`：`0.534s`
  3. heavy `validate_script` 实测：
     - `5.396s`
     - `assessment=blocked`
     - `stderr = [sunset_mcp] phase=codeguard timeout=5s targets=1`
  4. 同时核到当前 shared root 体量：
     - tracked `.cs`：`297`
     - dirty entries：`113`
     - untracked entries：`109`
- 关键决策：
  - `CLI + MCP` 确实比 direct MCP 更慢，但当前实测只慢几百毫秒，属于可以接受的壳层成本。
  - 真正把体感拖垮的，不是 thin CLI，而是把 `CodexCodeGuard` 这种重型程序集级检查塞进了 `validate_script` 高频动作里。
- 当前判断：
  - 如果问题是“CLI 本身值不值得留”，当前答案偏向：值得；它的薄壳成本不高。
  - 如果问题是“当前默认高频动作该不该是 validate_script”，当前答案偏向：不该；它太重。
- 当前最薄弱 / 最可能看错的点：
  - 这轮测的是当前一台机器、当前 shared root 脏度、当前 SpringDay1PromptOverlay 目标脚本，不代表所有仓位永远同样比例。
  - 但“薄壳约几百毫秒、heavy validate_script 主要被 CodeGuard 拖住”这两个方向，我把握很高。
- 当前恢复点：
  - 若继续推进这条线，下一步最值钱的是做一张正式分层：
    - 高频快路径
    - 中频确认路径
    - 重型证明路径

## 2026-04-05｜外部调研后已给出 Sunset 纯 CLI 方案边界

- 用户目标：
  - 用户明确要求去找“纯 CLI 使用 Unity”的现成路线和资源，不再只谈 `CLI + MCP`，并要结合：
    - `mcporter`
    - `CLI-Anything`
    - `opencli`
    给出一套 Sunset 可用的纯 CLI 方案。
- 当前主线目标：
  - 继续判断 `UnityMCP -> CLI` 这条线对 Sunset 的价值，但这次把“真纯 CLI 可行性”单独拎出来。
- 本轮实际确认的事实：
  1. 市面上有纯 CLI Unity 例子：
     - Unity 官方 CLI / batchmode / tests / builds
     - `Unity-Technologies/com.unity.cli-project-setup`
     - `devchan97/unity-cli`
     - `SmartAddresser` 的 CLI 方法
  2. 这些方案的共同点是：
     - 启动一个新的 Unity worker process
     - 依赖 `-batchmode / -executeMethod / -runTests`
     - 更像 headless task runner，不是 live editor bridge
  3. 当前最关键的技术边界是：
     - 真纯 CLI 方案可以做
     - 但不适合直接拿来替代“开着 Editor 改代码时的高频 fresh red / console 查询”
  4. 三份前序参考各自更适合借的层次：
     - `mcporter`：orchestrator / facade 分层
     - `CLI-Anything`：自描述命令面 + JSON 输出
     - `opencli`：adapter/plugin 边界
- 当前给出的 Sunset 纯 CLI 方案轮廓：
  - CLI 名：`sunset-unity`
  - 底座：Unity / Hub 官方 CLI
  - 项目内入口：`Assets/Editor/SunsetCli/EntryPoint.cs`
  - 调用形态：`Unity.exe -batchmode -quit -projectPath ... -executeMethod Sunset.Cli.EntryPoint.Run --sunset-cli ...`
  - 输出形态：stdout 摘要 + `Library/SunsetCli/*.json`
  - 核心命令：`doctor / compile / errors / validate-script / test editmode / build / exec-method`
- 关键决策：
  - 如果用户问“有没有纯 CLI 方案”，答案是有；
  - 如果用户问“它能不能无损替代当前 live 高频 no-red 工作流”，答案是不能直接替代。
- 当前判断：
  - 真纯 CLI 更适合作为：
    - CI / batch / setup / build / test 路线
  - 当前开着 Editor 的高频 no-red 工作流，仍更适合：
    - thin CLI + live bridge
    - 或者未来另一种本地 bridge，而不是硬上真纯 CLI
- 当前 thread-state：
  - 这轮为记忆结算切片；完成后应直接 `Park-Slice`
- 当前恢复点：
  - 若继续，下一步就不是再证明“有没有纯 CLI”，而是把这套纯 CLI 方案具体拆成命令面、批处理入口和 Sunset 适配边界。

## 2026-04-05｜当前 Sunset 用的 Unity MCP 来源已核实

- 用户目标：
  - 一句确认：官网的 MCP 是哪个，我们现在用的是不是 `IvanMurzak/Unity-MCP`。
- 当前主线目标：
  - 继续把 `UnityMCP -> CLI` 这条线的来源、边界和真实依赖说清楚，不让名字混淆。
- 本轮实际确认的事实：
  1. 当前 Sunset 安装包来源是：
     - `com.coplaydev.unity-mcp`
     - `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`
  2. 锁文件同样指向这套来源，并记录 hash `a7c715fb1f2b7741b46f5ee48c70aa3bb1189bd2`
  3. 你问到的 `IvanMurzak/Unity-MCP` 是另一套项目，不是当前 Sunset 安装来源。
  4. 我没有查到 Unity 官方站点 / Unity-Technologies 官方仓库发布的“官方 Unity MCP 包”。
- 当前判断：
  - 当前 Sunset 用的是 `CoplayDev/unity-mcp`。
  - 不是 `IvanMurzak/Unity-MCP`。
  - 也不是我目前能确认到的 Unity 官方 MCP。
- 当前 thread-state：
  - 本轮为来源核实记忆切片；完成后应直接 `Park-Slice`

## 2026-04-06｜`IvanMurzak/Unity-MCP` vs `CoplayDev/unity-mcp` 立项判断已形成

- 用户目标：
  - 要一份直接的对比和落地方案，判断是否要把 Sunset 下一阶段方向转到 `IvanMurzak/Unity-MCP`。
- 当前主线目标：
  - 继续围绕 `UnityMCP -> CLI` 与更大平台方向做判断，但这次改成“仓库级横向对比 + Sunset 迁移判断”。
- 本轮实际确认的事实：
  1. `IvanMurzak/Unity-MCP` 更偏：
     - AI Skills / CLI / Reflection / Runtime / In-Game
     - 更像 AI 平台化方向
  2. `CoplayDev/unity-mcp` 更偏：
     - Unity Editor bridge / Python server / 多实例 / 结构化工具面
     - 更像 Editor 自动化底座
  3. Sunset 当前已经深度耦合 `CoplayDev/unity-mcp`：
     - 包依赖
     - 8888 live 基线
     - CLI 命令面
     - autostart
     - 规则与治理文档
- 当前判断：
  - 如果问“哪套更贴 Sunset 当前高频 no-red 工作流”，答案偏向 `CoplayDev/unity-mcp`
  - 如果问“哪套更像下一代 AI 平台方向”，答案偏向 `IvanMurzak/Unity-MCP`
  - 如果问“要不要立刻切换”，当前答案是否；更合理的是把 Ivan 作为平行实验线单独立项
- 当前最薄弱点：
  - 这轮主要依据公开 README、项目依赖与现有 Sunset 落地，不是实际跑通 Ivan 在 Sunset 上的 PoC
  - 但“当前现网深度绑定 Coplay，切 Ivan 绝不是无成本换包”这一点把握很高
- 当前 thread-state：
  - 本轮为记忆结算切片；完成后应直接 `Park-Slice`

## 2026-04-06｜Ivan 平台化路线正式文稿已产出

- 用户目标：
  - 直接要一份成稿，而不是继续听口头判断。
- 当前主线目标：
  - 把 `IvanMurzak/Unity-MCP vs CoplayDev/unity-mcp` 的判断收成可直接推进的平台化立项文稿。
- 本轮实际做成了什么：
  1. 已新建文稿：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_Sunset_Ivan平台化路线对比与落地方案_01.md`
  2. 文稿已写清：
     - 当前现状
     - 核心对比
     - 立项判断
     - 平行实验线建议
     - 分阶段推进路线
     - 先做什么 / 别先做什么
- 关键决策：
  - 这一轮不再只是分析，而是正式把 Ivan 方向写成了可继续执行的立项底稿。
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：待本轮回复后合法停车
