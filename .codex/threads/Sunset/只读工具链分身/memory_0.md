# 只读工具链分身 线程记忆

## 2026-04-18｜packaged build 与 profiler 入口只读审计
- 用户目标：
  - 只读查清当前最短最安全的 packaged build 与 profiler spot check 入口，明确是否已有脚本/菜单、没有的话最小执行链是什么、日志/产物/失败信号在哪里。
- 当前主线目标：
  - 为 `Sunset` 输出一份可直接执行的最短操作方案，不改业务代码、不发明不存在的一键链。
- 本轮子任务 / 阻塞：
  - 只查看 `scripts`、`Assets/Editor`、`ProjectSettings/EditorBuildSettings.asset`、`.kiro/specs/900_开篇/spring-day1-implementation/100-重新开始/0417.md` 及相关 memory，确认 build/profiler 入口真实边界。
- 已完成事项：
  1. 确认仓内没有现成 `BuildPipeline.BuildPlayer`、`BuildPlayerOptions` 或项目自定义 `-executeMethod` 打包入口。
  2. 确认当前最短安全的 packaged build 入口仍是 Unity 自带 `Build Profiles / Build Settings`。
  3. 确认 `ProjectSettings/EditorBuildSettings.asset` 已含 `Town / Primary / Home`，当前至少不再卡在“场景未进 Build Profiles”。
  4. 从 `Editor.log` 实锤当前 Unity 路径：`D:\1_BBB_Platform\Unity\6000.0.62f1\Editor\Unity.exe`。
  5. 确认 profiler 侧已有 menu/artifact helper：
     - `SpringDay1LiveSnapshotArtifactMenu`
     - `SpringDay1LatePhaseValidationMenu`
     - `SpringDay1ResidentControlProbeMenu`
     - `SpringDay1ActorRuntimeProbeMenu`
     - `NpcRoamSpikeStopgapProbeMenu`
     - `CodexEditorCommandBridge`
  6. 确认这些 helper 只能覆盖 targeted/live probe，`0417.md` 当前仍明确要求“真正的人工 Unity Profiler 手工 spot check”。
- 关键决策：
  1. 不把“命令行打开 Unity”包装成“命令行一键打包已具备”。
  2. 不把 `Library/CodexEditorCommands/*.json` 的 probe/artifact 包装成 profiler 已自动化闭环。
- 涉及文件 / 路径：
  - `D:\Unity\Unity_learning\Sunset\scripts\sunset_mcp.py`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\CodexEditorCommandBridge.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\NpcRoamSpikeStopgapProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LiveSnapshotArtifactMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1LatePhaseValidationMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1ResidentControlProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1ActorRuntimeProbeMenu.cs`
  - `D:\Unity\Unity_learning\Sunset\ProjectSettings\EditorBuildSettings.asset`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0417.md`
  - `C:\Users\aTo\AppData\Local\Unity\Editor\Editor.log`
- 验证结果：
  - 纯只读审计；未改业务文件，未进 Unity，未跑 `Begin-Slice`。
- 给用户的最短口径：
  1. packaged build：直接走 Unity Editor 自带 build。
  2. profiler spot check：先用现有 validation 菜单把现场推进到目标时段，再人工打开 Unity Profiler 采样。
  3. 日志优先级：`Editor.log` -> `Library/CodexEditorCommands/*.json` -> `Player.log`。
- 恢复点：
  - 如果下一轮继续，只需要基于这份结论执行 packaged smoke 或 profiler 手工 spot check；不要先去补不存在的 build automation。

## 2026-04-21｜C:\Users\aTo 全局 AI 工具与治理痕迹只读盘点
- 用户目标：
  - 只读排查本机 `C:\Users\aTo` 下与 Codex / Kiro / Cursor / Windsurf / Claude / Gemini / OpenClaw / provider bridge / MCP 相关的配置、memory、日志和注册表式文档，并输出按工具分类的证据清单：实际看到了什么、能证明什么、哪些只是推断、哪些尚未直接找到。
- 当前主线目标：
  - 为用户整理一份“本机 AI 工具生态与治理痕迹”的证据化盘点，不改业务代码，不把存在目录误说成真实使用方式。
- 本轮子任务 / 阻塞：
  - 先按 Sunset 规则做手工前置核查；确认这是纯只读分析，暂不跑 `Begin-Slice`，然后只读扫描 `C:\Users\aTo` 顶层、`AppData\Roaming`、`Library\Application Support` 及各工具根目录。
- 已完成事项：
  1. 确认 `C:\Users\aTo` 顶层直接存在：
     - `.codex`
     - `.kiro`
     - `.cursor`
     - `.windsurf`
     - `.claude`
     - `.gemini`
     - `.openclaw`
     - 以及 `AppData\Roaming\Codex / Cursor / Kiro / UnityMCP / Windsurf`
  2. 确认 Codex 侧不只是基础配置，还存在完整治理层与桥接层：
     - `.codex\memories\global-learning-system.md`
     - `.codex\memories\global-skill-registry.md`
     - `.codex\memories\skill-trigger-log.md`
     - `.codex\provider-bridge\README.md`
     - `.codex\provider-bridge\data\bridge_state.json`
     - `.codex\provider-bridge\data\provider_history.json`
     - `.codex\provider-bridge\logs\bridge.log`
     - `.agents\skills\` 下对 `skills-governor / skill-vetter / sunset-startup-guard` 的 Junction 暴露
  3. 确认 Codex 当前配置与 MCP 痕迹：
     - `.codex\config.toml` 命中 `model_provider = "LZ"`、`model = "gpt-5.4"`、`rmcp_client = true`、`[mcp_servers.unityMCP] url = "http://127.0.0.1:8888/mcp"`
     - `provider_history.json` 与 `bridge_state.json` 显示 provider 被桥到 `LZ`，桥接状态为 `running`
     - `bridge.log` 有持续 reconcile 记录
  4. 确认 Kiro / Cursor / Gemini 都直接配置了 Unity MCP：
     - `C:\Users\aTo\.kiro\settings\mcp.json`
     - `C:\Users\aTo\.cursor\mcp.json`
     - `C:\Users\aTo\.gemini\settings.json`
     - `C:\Users\aTo\.gemini\antigravity\mcp_config.json`
     其中 Kiro / Cursor 同时保留了本地 `mcp-unity` node server 路径和 `unityMCP` 本地 HTTP 入口。
  5. 确认 Kiro / Cursor / Claude / Windsurf 都有明显实际使用痕迹：
     - Kiro：`AppData\Roaming\Kiro\User\settings.json` 含 `kiroAgent.modelSelection` 与 `kiroAgent.agentAutonomy = Autopilot`，并有大量 `logs`、`workspaceStorage`
     - Cursor：`.cursor\ide_state.json` 直接指向 `D:\Unity\Unity_learning\Sunset` 与 `.kiro/.claude` 文件；`.cursor\projects\d-Unity-Unity-learning-Sunset\` 下有 `agent-transcripts / mcps / terminals`
     - Claude：`.claude\history.jsonl`、`.claude\projects\D--Unity-Unity-learning-Sunset\`、`.claude\telemetry\`、`.claude\plugins\marketplaces\claude-plugins-official`
     - Windsurf：`AppData\Roaming\Windsurf\logs\`、`workspaceStorage\`、`globalStorage\`
  6. 确认 Claude 与 OpenClaw 都有“代理/模型桥接”痕迹：
     - `.claude\settings.json` 命中自定义 `ANTHROPIC_BASE_URL` 与将多条 Anthropic 默认模型名映射到 `gpt-5.4`
     - `.openclaw\openclaw.json` 命中 `models/providers`、多 agent 工作区、`LZ/gpt-5.4`、`browser provider = brave`
     - `.openclaw\agents\` 下存在 `main / pm / reader / builder / reviewer / gpt-5-4-fast / gpt-5-4-high`
     - `.openclaw\logs\config-audit.jsonl`、`.openclaw\logs\gateway-manual.log`、`.openclaw\memory\main.sqlite`
  7. 确认 UnityMCP 有独立运行与使用里程碑：
     - `AppData\Roaming\UnityMCP\milestones.json` 包含 `first_startup / first_tool_usage / first_scene_modification`
     - `Library\Application Support\UnityMCP\Logs\unity_mcp_server.log*` 存在
- 关键决策：
  1. 只把“文件/目录直接存在 + 内容命中明确字段”的部分当成强证据。
  2. 对 Cursor / Windsurf / Gemini 这类只看到配置或工作区痕迹、没去翻聊天正文的工具，明确保留“推断”层，不把它们包装成已核实的完整使用流程。
  3. 本轮不读取敏感 token 的完整值；只记录“存在此类配置/映射”，避免把只读审计变成泄露。
- 涉及文件 / 路径：
  - `C:\Users\aTo\.codex\`
  - `C:\Users\aTo\.kiro\`
  - `C:\Users\aTo\.cursor\`
  - `C:\Users\aTo\.windsurf\`
  - `C:\Users\aTo\.claude\`
  - `C:\Users\aTo\.gemini\`
  - `C:\Users\aTo\.openclaw\`
  - `C:\Users\aTo\AppData\Roaming\Cursor\`
  - `C:\Users\aTo\AppData\Roaming\Kiro\`
  - `C:\Users\aTo\AppData\Roaming\Windsurf\`
  - `C:\Users\aTo\AppData\Roaming\UnityMCP\`
  - `C:\Users\aTo\Library\Application Support\UnityMCP\Logs\`
- 验证结果：
  - 纯只读盘点；未改业务文件，未进 Unity，未跑 `Begin-Slice`。
- 恢复点：
  - 如果下一轮继续，优先从三条线里选一条深挖：
    1. `Codex provider-bridge` 的桥接时间线与 provider 演化
    2. `UnityMCP` 的日志时间线与各客户端接入关系
    3. `OpenClaw / Claude / Cursor / Kiro` 的单工具配置值级审计
