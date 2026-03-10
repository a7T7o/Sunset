# OpenClaw 线程记忆（中文版续卷）

## 线程定位
- 线程名称：`部署与配置龙虾V2`
- 当前工作区：`D:\1_AAA_Program\OpenClaw`
- 线程 continuity 记忆根：`D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2`
- 说明：`memory_0.md` 保留为历史原卷；从本卷开始，后续统一使用全中文记录。

## 当前主线目标
- 在 Windows 原生环境下，把 OpenClaw 的真实可用链路稳定下来。
- 这条主线包含两部分，但本质上是一条路：
  - 本地 CLI 多 agent 链路可用
  - 飞书外部入口链路可用
- 原则上不回到“重装/重配/重设计”，而是基于现有 live 配置把真实链路跑通、修稳、留痕。

## 用户目标摘要
- 接手前任线程 `部署与配置龙虾` 的全部上下文并继续推进。
- 对 OpenClaw 的部署、配置、多 agent 流程、飞书接入做完整理解、修复、验证、测试。
- 在仓库根目录维护 append-only 的 `ERROR_LOG.md`，把历史问题和本轮问题都沉淀进去。
- 遇到报错时直接排查并修，不把用户拉进人工介入流程。
- 最终要有可以继续接手的 memory，而不是依赖聊天上下文。

## 已完成事项

### 1. 接手与上下文整理
- 已完成对前任线程内容的读取、归纳和接手。
- 已确认本线程继续沿用 Sunset 侧线程 memory，而不是误写到全局目录。
- 已建立当前线程的连续性记录，后续接手点明确。

### 2. 错误日志体系
- 已在仓库根目录创建并维护 `ERROR_LOG.md`。
- 已把本轮发现的问题和之前已知问题按 append-only 方式回填进去。
- 当前 `ERROR_LOG.md` 已包含：
  - Windows 构建问题
  - `sessions_spawn` 兼容性问题
  - `LZ` 配额与 thinking 限制
  - CLI 延迟回包问题
  - 飞书消息读取限制
  - 飞书 `NO_REPLY` 静默误判问题

### 3. 多 agent 关键修复
- 已修复 `sessions_spawn` 在真实 live 路径中的兼容性断点。
- 关键结果：
  - `runtime=subagent` 时会忽略误传的 `streamTo`
  - `mode="session"` 且 `thread=false` 时会回退到默认 run 模式，而不是直接失败
- 相关文件：
  - `src/agents/tools/sessions-spawn-tool.ts`
  - `src/agents/tools/sessions-spawn-tool.test.ts`

### 4. Windows 原生构建修复
- 已把 A2UI bundling 从依赖 `bash` 的脚本切换为 Windows 原生可跑的 Node 入口。
- 已消除首次 bundler 版本产生的假失败噪音。
- 相关文件：
  - `package.json`
  - `scripts/bundle-a2ui.mjs`

### 5. CLI 延迟回包修复
- 已修复 `openclaw agent` 在多 agent / auto-announce 场景下，首个 RPC 为空却被误判成 “No reply from agent.” 的问题。
- 修复逻辑：
  - 当最终 payload 为空时，CLI 会在剩余超时预算内轮询 `chat.history`
  - 若同 session 中稍后出现真正的 assistant 回复，则自动补回 plain text 和 `--json`
- 相关文件：
  - `src/commands/agent-via-gateway.ts`
  - `src/commands/agent-via-gateway.test.ts`

### 6. 已完成的真实验证
- 已通过的代码/构建验证：
  - `pnpm vitest run src/commands/agent-via-gateway.test.ts`
  - `pnpm tsgo`
  - `pnpm canvas:a2ui:bundle`
  - `pnpm build`
- 已通过的真实链路验证：
  - `main -> pm`
  - `pm -> reader / reviewer`
  - `builder` 模糊请求拦截
  - 飞书 outbound 发送
  - 飞书 inbound 路由到 `pm`
  - 飞书显式 agent 回复投递

### 7. 飞书链路当前结论
- 飞书 outbound：已验证可用。
- 飞书 inbound 到 agent：已验证可用。
- 之前看起来像故障的这条记录：
  - `dispatch complete (queuedFinal=false, replies=0)`
  实际不是丢消息，而是 agent 当时回复了 `NO_REPLY`，系统按预期静默。
- 为了排除误判，后续又做过一条显式回包验证，成功返回：
  - `FEISHU_AGENT_OK`

### 8. 网关与 Control UI
- 已按要求重启网关。
- 当前网关监听正常，18789 已恢复。
- Control UI 一度提示资产缺失，但随后刷新恢复。
- 结合磁盘检查结果：
  - `dist/control-ui/index.html` 存在
  - `dist/control-ui/assets/*` 存在
- 当前判断：这是重启后的瞬时状态，不是持续性静态资源缺失故障。

## 当前剩余事项

### 已经不是阻塞、但仍可继续优化
- 做一轮更系统的全自动回归，把 CLI、飞书、主代理、子代理串成固定验证脚本。
- 继续清理/固化 OpenClaw 在本机的启动、停止、巡检脚本说明。
- 如需长期运维，可把“网关重启后健康检查”沉淀成更标准化的脚本或 SOP。

### 当前仍存在的已知限制
- `openclaw message read` 目前不支持 Feishu。
- `LZ` 当前 live 路径不接受 `thinking=minimal`，需要用 `low` 起步。
- Chrome / Edge 真标签页接管之前有历史遗留，不属于当前 OpenClaw 主线阻塞。

## 当前主线状态
- 主线状态：健康，可继续推进。
- 不是停在“故障抢修中”，而是已经回到“已修复、已验证、可继续扩展验证”的阶段。

## 后续默认恢复点
- 如果从这里继续，默认从以下顺序往前做：
  1. 把当前关键链路整理成更稳定的自动化回归
  2. 持续补齐飞书外部入口的回归用例
  3. 只在发现新的真实故障时再改代码

## 关键文件索引
- 代码修复：
  - `D:\1_AAA_Program\OpenClaw\src\agents\tools\sessions-spawn-tool.ts`
  - `D:\1_AAA_Program\OpenClaw\src\agents\tools\sessions-spawn-tool.test.ts`
  - `D:\1_AAA_Program\OpenClaw\src\commands\agent-via-gateway.ts`
  - `D:\1_AAA_Program\OpenClaw\src\commands\agent-via-gateway.test.ts`
- 构建修复：
  - `D:\1_AAA_Program\OpenClaw\package.json`
  - `D:\1_AAA_Program\OpenClaw\scripts\bundle-a2ui.mjs`
- 运维与记录：
  - `D:\1_AAA_Program\OpenClaw\ERROR_LOG.md`
  - `D:\1_AAA_Program\OpenClaw\start-openclaw-hidden.ps1`
  - `D:\1_AAA_Program\OpenClaw\stop-openclaw.ps1`
- 线程记忆：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_1.md`

## 一句话恢复口径
- OpenClaw 这条主线目前已经从“真实链路不稳定”推进到“关键故障已修、CLI 和飞书都已跑通、剩下主要是持续化回归和运维整理”。

---

## 2026-03-10 Append - 全中文整合卷切换说明

- 按用户“彻底汉化 memory”的明确要求，已新建全中文整合卷：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_2.md`
- `memory_0.md` 与 `memory_1.md` 保留为历史承接卷，不覆盖、不回写旧记录。
- 后续默认续写入口切换为 `memory_2.md`。
- 编码核对结论：
  - `memory_1.md` 文件本体是 UTF-8 中文内容。
  - 之前通过 PowerShell 直接查看时出现的“乱码”属于终端显示问题，不是 memory 文件内容损坏。
