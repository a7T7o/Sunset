# mcp-live-baseline

## 文件身份
- 本文件是 Sunset 当前唯一的 MCP live 基线说明。
- 它不替代：
  - `mcp-single-instance-occupancy.md`
  - `mcp-hot-zones.md`
  - `shared-root-branch-occupancy.md`
- 它只回答一件事：
  - 当前 Codex / Unity / MCP live 验证前，哪条桥、哪个端口、哪份 pidfile、哪种会话资源暴露才算“同一份事实”。

## 当前唯一基线
- Codex 配置文件：
  - `C:\Users\aTo\.codex\config.toml`
- 当前唯一 MCP server 名：
  - `unityMCP`
- 当前唯一 HTTP 端点：
  - `http://127.0.0.1:8888/mcp`
- 当前唯一 shared root pidfile：
  - `D:\Unity\Unity_learning\Sunset\Library\MCPForUnity\RunState\mcp_http_8888.pid`

## 明确判定为旧口径 / 失效口径的内容
- `http://127.0.0.1:8080/mcp`
- `http://localhost:8080/mcp`
- `mcp-unity`
- 任何只写“Session Active / Configured 成功”，但没有补充端口、pidfile、resources server 的结论

## 进入 Unity / MCP live 验证前必须先过的四项
1. 配置层
   - `config.toml` 里只能保留 `unityMCP`
   - 不应再存在 `[mcp_servers.mcp-unity]`
2. 服务层
   - `127.0.0.1:8888` 必须正在监听
   - 目标实例的 `mcp_http_8888.pid` 必须存在
3. 会话层
   - 当前 Codex 会话 `list_mcp_resources` / `list_mcp_resource_templates` 暴露的 server 必须是 `unityMCP`
   - 如果资源为空、或仍暴露 `mcp-unity`，一律视为会话未回正
4. 实例层
   - 当前 `manage_scene(get_active)` / `project_info` 读到的实例必须是你本轮真正要操作的 Unity 实例
   - shared root 的 `Primary` 与 worktree 的 `SceneBuild_01` 不能混成一套结论

## 三类常见故障的区分口径
- 服务未启动
  - 8888 无监听
  - pidfile 缺失
  - `/mcp` 无法连接
- 会话未回正
  - `config.toml` 已改对
  - 但当前会话 resources 仍为空或仍是 `mcp-unity`
- 旧线程缓存未刷新
  - `config.toml` 已改对
  - `127.0.0.1:8888` 监听正常
  - pidfile 存在
  - 基线脚本已 `pass`
  - 但旧线程 / 旧会话日志仍继续打旧端口或旧桥名
  - 这类情况优先视为会话内 MCP 路由缓存未刷新，不直接判服务端回滚
- 插件短断 / Unity 侧重连
  - server 仍监听
  - pidfile 仍存在
  - 但 Unity 面板报 `[WebSocket] Connection failed`
  - 这时优先看插件重连与实例稳定性，不要先怀疑端口回滚

## 推荐自检顺序
1. 读本文件
2. 运行：
   - `D:\Unity\Unity_learning\Sunset\scripts\check-unity-mcp-baseline.ps1`
3. 再做当前会话 `list_mcp_resources`
4. 如果基线脚本已 `pass`，但旧线程仍报旧端口 / 旧桥名 / 资源为空，先换新线程 / 新会话，或先手工对 `http://127.0.0.1:8888/mcp` 做 initialize + `tools/list`
5. 再做 `manage_scene(get_active)` / `read_console(get)`

## 一句话口径
- 以后任何 Sunset 线程只要还在写 `8080`、`mcp-unity`、只写“Session Active 所以可用”，或在基线已绿时仍把旧线程缓存误判成服务端回滚，都视为没有通过当前 MCP live 基线核查。
