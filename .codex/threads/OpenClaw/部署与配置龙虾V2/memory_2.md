# OpenClaw 线程记忆（全中文整合卷）

## 说明
- 本卷根据用户“彻底汉化 memory”的要求创建。
- 目标是把当前线程可延续、可执行、可接手的内容统一整理成全中文版本，避免后续继续依赖历史混合语言内容。
- 历史卷保留不改写：
  - `memory_0.md`
  - `memory_1.md`
- 从本卷开始，后续默认继续追加到 `memory_2.md`。
- 编码核对结论：
  - `memory_1.md` 文件本体是 UTF-8 中文内容。
  - 先前在 PowerShell 中看到的乱码属于终端显示问题，不是文件级内容损坏。

## 线程定位
- 线程名：`部署与配置龙虾V2`
- 当前工作区：`D:\1_AAA_Program\OpenClaw`
- 线程 continuity 根路径：`D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2`

## 当前主线目标
- 在 Windows 原生环境中把 OpenClaw 的真实可用链路稳定下来，并确保后续可以无缝接手、持续验证和继续扩展。
- 这条主线包含两段，但本质是一条路：
  - 本地 CLI -> agent 链路稳定可用
  - Feishu 外部入口 -> gateway -> agent -> visible reply 链路稳定可用
- 当前阶段不是重新部署，而是基于现有 live 配置持续修复、验证、留痕和固化。

## 用户目标摘要
- 接手前任线程 `部署与配置龙虾` 的全部上下文并继续推进。
- 对 OpenClaw 的部署、配置、多 agent 流程、Feishu 接入做完整理解、修复、验证和测试。
- 在仓库根目录维护 append-only 的 `ERROR_LOG.md`，把历史问题与本轮问题持续沉淀进去。
- 遇到报错时优先自主排查、修复和验证，尽量不依赖用户人工介入。
- 最终留下可继续接手的中文 memory，而不是依赖聊天上下文。

## 已完成事项
### 1. 接手与上下文整理
- 已完整接手前任线程上下文，并把主线重新锚定为“Windows 原生 OpenClaw 真实链路验证”。
- 已确认本仓库没有本地 `.codex/threads/`，因此继续使用 Sunset 侧线程路径承接 continuity。
- 已把当前线程的连续性记录稳定落在 `部署与配置龙虾V2` 对应 memory 路径下。

### 2. 错误日志体系
- 已在仓库根目录创建并维护 `ERROR_LOG.md`。
- 已按 append-only 方式回填历史问题，并记录本轮真实发现。
- 当前已沉淀的关键问题包括：
  - Windows 原生构建链路问题
  - `sessions_spawn` live 兼容性问题
  - `LZ` thinking 档位限制
  - CLI 延迟回包误判问题
  - Feishu `message read` 能力缺失
  - Feishu `replies=0` 的误判问题

### 3. 关键代码修复
- 已修复 `sessions_spawn` 在真实 live 路径中的兼容性断点。
- 已把 A2UI bundling 从依赖 `bash` 的脚本切换为 Windows 原生可跑的 Node 入口。
- 已修复 `openclaw agent` 在延迟 auto-announce 场景下过早输出 `No reply from agent.` 的问题。
- 当前轮最关键、并已验证通过的代码文件：
  - `src/commands/agent-via-gateway.ts`
  - `src/commands/agent-via-gateway.test.ts`

### 4. 已完成验证
- 已通过的本地验证：
  - `pnpm vitest run src/commands/agent-via-gateway.test.ts`
  - `pnpm tsgo`
  - `pnpm canvas:a2ui:bundle`
  - `pnpm build`
- 已通过的真实链路验证：
  - direct `pm`
  - `main -> pm`
  - `pm -> reader / reviewer`
  - `builder` 模糊请求拦截
  - Feishu outbound
  - Feishu inbound routing
  - Feishu 显式可见回包 `FEISHU_AGENT_OK`

### 5. Feishu 链路当前结论
- Feishu outbound：已验证可用。
- Feishu inbound -> agent：已验证可用。
- 之前看起来像故障的日志：
  - `dispatch complete (queuedFinal=false, replies=0)`
  实际不是丢消息，而是 agent 当时回复了 `NO_REPLY`，系统按预期保持静默。
- 后续又做过显式回包验证，成功返回：
  - `FEISHU_AGENT_OK`

### 6. Gateway 与 Control UI
- 已按要求重启本地 gateway。
- 已确认 18789 端口恢复监听。
- Control UI 一度提示 assets not found，但随后刷新恢复。
- 已确认以下静态资源实际存在：
  - `dist/control-ui/index.html`
  - `dist/control-ui/assets/*`
- 当前判断：这是重启后的瞬时状态，不是持续性的资源缺失故障。

## 当前进度结论
- 当前主线状态：健康，已回到“已修复、已验证、可继续固化”的阶段。
- 已完成的核心工作：
  - 前任线程接手
  - 错误日志体系建立并回填
  - 多 agent / CLI / Windows 构建关键修复
  - Feishu 出入站链路验证
  - Gateway / Control UI 恢复核实
- 当前没有新的已确认阻塞故障。

## 剩余事项
- 将当前手工验证进一步固化为更系统的自动化回归流程或标准化 SOP。
- 继续补齐长期运维所需的启动、停止、健康检查与异常复盘说明。
- 仅在发现新的真实故障时继续修改代码，不为了“看起来更完整”而盲目改动。

## 当前已知限制
- `openclaw message read` 目前不支持 Feishu。
- `LZ` 当前 live 路径不接受 `thinking=minimal`，应使用 `low` 起步。
- Chrome / Edge 真标签页接管有历史遗留，但不属于当前 OpenClaw 主线阻塞。

## 用户偏好与协作约束
- 汇报尽量简明，先说进度和状态。
- 倾向自主执行，不希望频繁人工介入。
- 如需 UI 操作，可直接使用工具接管并完成测试。
- 要求中文输出，并要求 memory 中文化。
- 不接受“只靠聊天上下文记住”，必须持续落文件。
- 目标是稳定 Windows 原生路径，不转向 WSL。

## 默认恢复点
- 如果后续从这里继续，默认按以下顺序往前推进：
  1. 把当前关键链路整理成更稳定的自动化回归或 SOP。
  2. 维持 `ERROR_LOG.md` 的 append-only 记录习惯。
  3. 仅在出现新的真实故障时再进入代码修复。

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
- 线程 memory：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_1.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾V2\memory_2.md`

## 一句话恢复口径
- OpenClaw 这条主线目前已经从“真实链路不稳定”推进到“关键故障已修、CLI 和 Feishu 都已跑通、当前主要剩系统化回归和运维固化”。

---

## 2026-03-10 Append - 多 agent 目标态与当前成熟度判断

### 当前主线目标
- 用户要求的不只是“多 agent 能启动”，而是“多 agent 可以真实落地使用并可验收”，因此需要把目标态、当前阶段、可验收范围、剩余缺口说清楚。

### 本轮结论
- 结合仓库文档与当前 live 验证结果，当前多 agent 的真正目标态应至少包含四层：
  1. 联通：外部入口、gateway、主 agent、子 agent、回包链路稳定打通。
  2. 闭环：主 agent 能把任务分发给子 agent，子 agent 完成后能回传并形成最终用户可见结果。
  3. 可运维：重启、异常、超时、静默回复、日志排查都有明确处理方式。
  4. 可持续：有自动化回归、SOP、错误日志和交接 memory，不依赖某个人现场盯着。
- 当前实际所处阶段不是“刚接通”，也不是“全面生产化完成”，而是：
  - 已完成“联通 + 关键闭环验证”
  - 正准备进入“运维固化 + 自动化回归”阶段

### 当前可以验收的内容
- Windows 原生路径可运行、可构建、可启动 gateway。
- `main -> pm -> reader/reviewer` 这条关键多 agent 主链已经真实跑通。
- Feishu 外部入口到 agent，再到显式可见回复，已经做过 live 验证。
- CLI 延迟回包误判问题已修复，避免“其实有结果却显示 No reply from agent.” 的假失败。
- `ERROR_LOG.md` 与中文 memory 已建立，可支持后续持续接手。

### 当前还不能宣称完全验收的部分
- 还没有把当前手工 live 验证固化成一套系统化自动回归。
- 还没有把更多复杂编排场景做成常态验证，例如更深层嵌套、多分支并发、长期运行后的恢复演练。
- 还没有把“长期无人值守运维”所需的 SOP、健康检查、异常恢复流程完全固化。

### 当前距离“彻底落地使用多 agent”还有多远
- 如果把“彻底落地”定义为生产级、可持续、可重复验收的状态，那么当前大致处于 70% 到 80% 阶段。
- 剩下的主要不是“核心功能完全不能用”，而是最后一段工程化工作：
  - 自动化回归
  - 运维 SOP
  - 更广覆盖的场景验证
  - 长稳性验证

### 恢复点
- 后续继续推进时，默认优先做“把已跑通主链固化为可重复验收流程”，而不是重新回到基础联通排障。

---

## 2026-03-10 Append - 多 agent 自动化验收流程已落地并通过实跑

### 当前主线目标
- 把此前已经跑通的多 agent 主链，从“人工逐条验证”推进到“有固定命令入口、有结构化报告、有 transcript 证据的可重复验收流程”。

### 本轮子任务
- 新增一套 Windows 原生可执行的本地多 agent 自动化验收脚本。
- 把它接到仓库命令入口中。
- 跑通本地验收与 Feishu 可见投递验收。

### 本轮已完成
- 新增脚本：
  - `scripts/verify-local-multiagent.mjs`
- 新增命令入口：
  - `pnpm verify:local-multiagent`
- 更新本地协议文档，加入自动化验收说明：
  - `LOCAL_MULTIAGENT_PROTOCOL.md`
- 自动化验收覆盖项已包含：
  1. `channels status --probe`
  2. direct `pm`
  3. `main -> pm`
  4. `pm -> reader/reviewer`
  5. builder gate
  6. 可选 Feishu visible delivery

### 本轮验证结果
- 直接运行脚本并带 Feishu 投递参数，整套验收通过。
- 通过命令入口再次运行 `pnpm verify:local-multiagent -- --sunset-workspace "D:\\Unity\\Unity_learning\\Sunset" --skip-feishu`，再次通过。
- 生成的验收报告：
  - `D:\1_AAA_Program\OpenClaw\reports\local-multiagent-acceptance-20260310-092216.md`
  - `D:\1_AAA_Program\OpenClaw\reports\local-multiagent-acceptance-20260310-092353.md`

### 本轮结论
- 多 agent 主链现在已经不只是“之前跑通过”，而是已经具备“一键复验”的工程化入口。
- 当前阶段已从“live 验证成功”进一步推进到“可重复验收成功”。

### 当前恢复点
- 如果后续继续推进，默认不再回到基础联通排障。
- 下一阶段应优先考虑：
  1. 是否把报告格式再标准化
  2. 是否把更多异常场景纳入同一验收脚本
  3. 是否把长期运维 SOP 补齐

---

## 2026-03-10 Append - 重启恢复验收与本地运维固化已完成

### 当前主线目标
- 在“主链可重复验收”基础上，再向前推进一层，把 gateway 重启后的恢复验证与本地运维步骤一起固化下来。

### 本轮子任务
- 扩展本地多 agent 验收脚本，使其支持：
  - 先重启 gateway
  - 等待 gateway 恢复
  - 再跑完整主链验收
- 补一份本地运维说明，便于后续重复执行和接手。

### 本轮已完成
- 扩展脚本：
  - `scripts/verify-local-multiagent.mjs`
- 新增本地运维文档：
  - `LOCAL_MULTIAGENT_OPERATIONS.md`
- 更新协议文档：
  - `LOCAL_MULTIAGENT_PROTOCOL.md`
- 验收脚本现已支持：
  - `--restart-gateway`
  - 可选 Feishu visible delivery 验证
  - 更稳健的 JSON 输出解析
  - Windows 下超时子进程的更稳健清理

### 本轮遇到并解决的问题
- 第一版重启恢复实现错误地把 `start-openclaw-hidden.ps1` 当成前台进程等待，导致恢复步骤超时。
- 同时，gateway 不可用时 `agent --json` 的前置日志会污染 stdout，导致脚本直接 JSON 解析失败。
- 以上两点都已经在脚本中修复，并已回写到 `ERROR_LOG.md`。

### 本轮验证结果
- 曾出现一次失败报告：
  - `D:\1_AAA_Program\OpenClaw\reports\local-multiagent-acceptance-20260310-095821.md`
  该报告用于保留故障证据。
- 修复后，完整恢复验收通过：
  - `D:\1_AAA_Program\OpenClaw\reports\local-multiagent-acceptance-20260310-100630.md`
- 通过项包括：
  1. gateway restart recovery
  2. gateway probe
  3. direct `pm`
  4. `main -> pm`
  5. `pm -> reader/reviewer`
  6. builder gate
  7. Feishu visible delivery

### 本轮结论
- 当前不仅“主链可验收”，而且“重启后的恢复链路也可验收”。
- 这说明多 agent 现在已经从“关键主链可复验”继续推进到“具备基本本地运维与恢复能力”这一层。

### 当前恢复点
- 后续如果继续往前推进，优先方向应是：
  1. 进一步补异常场景，例如 provider 抖动、gateway 不可达、会话污染后的恢复
  2. 视需要再决定是否清理 `channels status --probe` 中 Feishu doctor 迁移提示这一非阻塞噪音

---

## 2026-03-10 Append - 终端窗口频繁弹出原因定位

### 本轮子任务
- 用户反馈“终端窗口一直弹出”，需要判断是不是本轮自动化验收与恢复脚本导致，并确认当前是否还有可见残留终端窗口。

### 本轮结论
- 是，这一轮确实执行了较多 Windows 原生命令，尤其是：
  - gateway 重启恢复验收
  - PowerShell 启动/停止脚本
  - 多轮自动化验收命令
- 在 Windows 桌面环境里，这类 `powershell` / `cmd` 子进程有机会短暂闪出终端窗口。
- 当前检查结果显示：
  - 没有仍然保持可见主窗口句柄的 `powershell` / `cmd` 终端进程
  - 当前 gateway 进程继续以隐藏的 node 方式运行

### 处理结果
- 自动化恢复脚本已经避免继续依赖 `start-openclaw-hidden.ps1` 作为验收默认启动路径，改为更稳定的 detached node 启动方式，后续应明显减少这类窗口闪出。
- 如果后续继续做重型本地验证，需要优先减少显式 shell 拉起次数，避免桌面侧反复闪窗。

### 后续排查补充
- 在用户反馈“终端窗口仍在反复闪出”后，已再次执行清理：
  - 停掉本轮由验收脚本拉起的 OpenClaw gateway 进程
  - 短时间监听新创建的 `powershell/cmd/conhost` 进程
- 监听结果：
  - 清理后未再观察到新的 `powershell/cmd/conhost` 终端进程被持续拉起
- 当前结论：
  - 本轮由我触发的持续闪窗源头已停止
  - 若此后仍继续闪窗，应优先怀疑本轮任务之外的外部常驻程序或其他自动化

---

## 2026-03-10 Append - gateway 已恢复，当前可用能力与测试样例已整理

### 本轮子任务
- 按用户要求恢复 gateway 运行。
- 重新整理“现在到底已经做成什么样、可以怎么测”的可用说明。

### 本轮已完成
- 已恢复本地 gateway 运行，并重新确认：
  - `channels status --probe` 显示 gateway reachable
  - Feishu main 显示 running / works
- 已整理当前多 agent 的四条核心可用路径：
  1. direct `pm`
  2. `main -> pm`
  3. `pm -> reader/reviewer`
  4. Feishu -> `pm` / Feishu visible delivery

### 当前对外可说明的状态
- `pm`：可作为直接执行与项目总控入口。
- `main`：可作为本地控制台入口，把高层任务交给 `pm`。
- `pm -> reader/reviewer`：可作为并行事实读取与风险审视路径。
- `builder`：仍然是 gated，不会在模糊需求下自动开工。
- Feishu：当前日常入口默认直达 `pm`，并已验证 visible delivery。

### 当前恢复点
- 后续如果继续推进，可以直接基于这四类路径扩展更多异常场景回归，不需要再重复解释主链结构。

---

## 2026-03-10 Append - Control UI 可用性问题已定位并完成第一轮修复

### 当前主线目标
- 主线已从“后端多 agent 链路能跑”切到“Control UI 必须真实可用、能实时看到正确结果、不能暴露内部 runtime 文本”。

### 本轮子任务
- 围绕用户反馈的“必须多次刷新才看到更新”“聊天区出现内部 runtime 上下文”做代码级根因定位、修复与回归验证。

### 本轮关键结论
- 问题不是后端链路没跑通，而是 UI 展示层存在两类缺陷叠加：
  1. `inter_session` 内部消息没有被过滤，导致 `OpenClaw runtime context (internal)` 和子 agent 原始结果泄漏到用户聊天区。
  2. `loadChatHistory()` 异步完成后会整包覆盖 `chatMessages`，缺少并发保护；当历史刷新晚于 live final 事件时，会把刚到的真实回复再次覆盖掉，于是用户只能手动刷新后才看到。

### 本轮已完成
- 前端修复：
  - `ui/src/ui/controllers/chat.ts`
    - 增加 `inter_session` 运行时消息过滤
    - 忽略来自其他 run 的内部 runtime final 事件
    - 给 `loadChatHistory()` 增加“晚到历史合并”保护，避免覆盖新到的 final reply
  - `ui/src/ui/chat-event-reload.ts`
    - `inter_session` final 不再触发多余的历史 reload
- 网关修复：
  - `src/gateway/server-methods/chat.ts`
    - `chat.history` 只对 UI / webchat 客户端隐藏 `inter_session` runtime 消息
    - 非 UI/internal 调用方仍保留原始历史，避免误伤 agent 内部能力
- 回归测试：
  - `ui/src/ui/controllers/chat.test.ts`
  - `src/gateway/server.chat.gateway-server-chat-b.test.ts`
- 错误日志已追加：
  - `D:\1_AAA_Program\OpenClaw\ERROR_LOG.md`

### 本轮验证结果
- 已通过：
  - `pnpm exec oxfmt --check ui/src/ui/controllers/chat.ts ui/src/ui/controllers/chat.test.ts ui/src/ui/chat-event-reload.ts src/gateway/server-methods/chat.ts src/gateway/server.chat.gateway-server-chat-b.test.ts`
  - `pnpm test -- ui/src/ui/controllers/chat.test.ts src/gateway/server.chat.gateway-server-chat-b.test.ts`
- 结论：当前已经完成“内部消息泄漏 + 历史晚到覆盖”这一轮一阶修复，并拿到针对性回归通过。

### 涉及文件
- `D:\1_AAA_Program\OpenClaw\ui\src\ui\controllers\chat.ts`
- `D:\1_AAA_Program\OpenClaw\ui\src\ui\controllers\chat.test.ts`
- `D:\1_AAA_Program\OpenClaw\ui\src\ui\chat-event-reload.ts`
- `D:\1_AAA_Program\OpenClaw\src\gateway\server-methods\chat.ts`
- `D:\1_AAA_Program\OpenClaw\src\gateway\server.chat.gateway-server-chat-b.test.ts`
- `D:\1_AAA_Program\OpenClaw\ERROR_LOG.md`

### 遗留问题 / 下一步
- 需要做一次真实 Control UI 交互复验，确认在你实际页面里“不刷新也能看到最终结果”已经恢复。
- 若继续深挖，可再看：
  1. `main` 对真实派单任务的用户话术是否仍然过早确认“已转交”
  2. 超时子 agent 结果是否还需要进一步摘要/净化，避免即使进入内部链路也过于冗长

### 恢复点
- 下一轮默认从“真实 Control UI 交互复验 + 异常场景回归”继续，不再回到基础联通排查。

---

## 2026-03-10 Append - 用户追问当前剩余未完成项，已明确验收边界

### 当前主线目标
- 当前主线仍是把多 agent 从“后端能跑”推进到“Control UI 可稳定使用并可验收”。

### 本轮子任务
- 向用户明确：在完成本轮 UI 一阶修复后，还剩哪些内容没有完成。

### 本轮结论
- 当前“代码级一阶修复”已完成，但“真实页面复验”和“更深一层工程化收尾”仍未完成。

### 当前仍未完成的内容
- 真实 Control UI 交互复验：
  - 需要再用真实页面跑一轮，确认“不刷新也能看到最终结果”在用户实际环境中已经恢复。
- 真实任务编排体验优化：
  - `main` 对派单任务的对外话术仍可能过早确认“已转交”，这部分还没有收口。
- 超时 / 异常结果净化：
  - 超时子 agent 的内部结果虽然已不再泄漏到聊天区，但是否还需要更好的摘要与失败呈现，还没继续做。
- 异常场景回归：
  - 还没把 gateway 抖动、provider 波动、恢复后二次验收、长时间运行稳定性纳入同一套回归。

### 当前可验收 vs 未验收
- 已可验收：
  - 多 agent 主链路
  - Feishu 可见投递
  - Control UI 内部消息泄漏修复
  - Control UI 历史晚到覆盖修复
- 仍未最终验收：
  - 真实页面长链路使用体验
  - 异常恢复与长期稳定性

### 恢复点
- 下一轮默认直接进入“真实 Control UI 复验 + 异常场景回归”。

---

## 2026-03-10 Append - Control UI 第二处泄漏已修复并完成真实页面验收

### 当前主线目标
- 当前主线已经从“后端多 agent 能跑”推进到“Control UI 在真实页面里可稳定使用并可验收”。

### 本轮子任务
- 继续完成此前尚未收口的真实 Control UI 验证。
- 定位为什么在第一轮 chat/history 修复之后，页面里仍然会看到内部 runtime 文本。
- 完成真实页面级别的 direct `pm`、`main -> pm`、以及 gateway 重启后同页恢复验证。

### 本轮关键结论
- 第一轮修复只覆盖了 `chat.history` 与 final chat event，仍未覆盖默认开启的 tool/workflow stream。
- 用户看到的 `OpenClaw runtime context (internal)` 不是新的 chat history 泄漏，而是 `sessions_spawn` tool result 中的内部 handoff 文本，被 Control UI 的 thinking/work-output 视图直接渲染出来了。
- 这说明此前“后端链路能跑但页面体验仍像坏的”这个判断是对的，问题在 UI 第二条渲染链。

### 本轮已完成
- 新增 UI 内部 runtime 清洗辅助：
  - `ui/src/ui/chat/internal-runtime.ts`
- 把内部 runtime 清洗接入到以下路径：
  - `ui/src/ui/app-tool-stream.ts`
  - `ui/src/ui/chat/message-extract.ts`
  - `ui/src/ui/chat/tool-cards.ts`
- 新增/补充测试：
  - `ui/src/ui/chat/internal-runtime.node.test.ts`
  - `ui/src/ui/app-tool-stream.node.test.ts`
  - 先前的 `ui/src/ui/controllers/chat.test.ts`
  - 先前的 `src/gateway/server.chat.gateway-server-chat-b.test.ts`
- 重新构建 UI：
  - `pnpm ui:build`
- 生成真实 UI 验收报告：
  - `reports/control-ui-live-verification-20260310-1217.md`
- 生成恢复诊断证据：
  - `reports/ui-recovery-diagnostic-latest.json`
  - `reports/ui-recovery-verification-latest.json`
- 追加错误日志：
  - `ERROR_LOG.md`

### 本轮验证结果
- 代码级验证通过：
  - `pnpm exec oxfmt --check ...`
  - `pnpm --dir ui exec vitest run --config vitest.node.config.ts src/ui/chat/internal-runtime.node.test.ts src/ui/app-tool-stream.node.test.ts`
  - `pnpm exec vitest run ui/src/ui/controllers/chat.test.ts src/gateway/server.chat.gateway-server-chat-b.test.ts`
- 真实页面验证通过：
  1. fresh `agent:pm:*` UI session：
     - 回复 `UI_PM_FRESH_OK_20260310`
     - 无需手动刷新
     - 无内部 runtime 标记
  2. fresh `agent:main:*` UI session：
     - 完成 `main -> pm`
     - 回复 `UI_MAIN_PM_OK_20260310 | child=UI_MAIN_PM_CHILD_OK_20260310`
     - 无需手动刷新
     - 无内部 runtime 标记
  3. same-page gateway restart recovery：
     - 重启前回复 `UI_RECOVERY_BEFORE_OK_20260310`
     - 同一页面自动断开再恢复
     - 重启后继续回复 `UI_RECOVERY_AFTER_OK_20260310`
     - 全程无内部 runtime 标记

### 本轮遇到并解决的问题
- 用 PowerShell hidden launcher 做浏览器自动化恢复时，一度把“网关没完全起来”和“页面等待条件写得过脆”混在一起，导致第一次 recovery 复验超时。
- 进一步拆分诊断后确认：
  - 页面在 gateway stop 后会正确进入 disconnected 状态
  - 约 15 秒后可在同页自动恢复
  - 更稳的自动化恢复方式是直接用 detached node 启动 gateway，而不是依赖那条 PowerShell hidden launcher 作为浏览器回归默认路径

### 当前可对外确认的验收范围
- 多 agent 后端主链已通。
- Control UI 聊天历史不再被 inter-session runtime message 污染。
- Control UI thinking/work-output 视图不再泄漏内部 subagent handoff 文本。
- Control UI 真实页面已通过：
  - direct `pm`
  - `main -> pm`
  - same-page restart recovery

### 遗留问题 / 下一步
- 当前还未彻底工程化完成的部分，主要是：
  1. provider overload / 抖动 的页面级异常回归
  2. 更长时间、多轮次的稳定性验证
  3. 如有需要，把这次真实页面验收进一步固化成单独的自动化脚本入口，而不是依赖临时执行

### 恢复点
- 下一轮默认从“把 provider 抖动/失败恢复纳入同一套 Control UI 回归流程”继续，不再回到基础联通排查。
