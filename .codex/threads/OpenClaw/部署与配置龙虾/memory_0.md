# memory_0

## 线程范围
- 来源：当前 Codex 对话线程（含用户原始轮次、压缩转述、后续交接摘要）。
- 落盘时间：2026-03-08
- 目的：把本线程里已经完成的配置、决策、踩坑、未完成事项全部外部化，避免后续切换模型/会话/工作场景时丢失上下文。

## 用户核心目标演化
本线程的目标不是单一的“安装 OpenClaw”，而是逐步演化为一整套个人本地 AI 工作系统：

1. 在 Windows 本机稳定运行 OpenClaw，不依赖 WSL。
2. 让 OpenClaw 使用用户的 2API / CCSwitch / 中转模型配置正常工作。
3. 修复和稳定 Control UI / gateway / config 页面等基础能力。
4. 接入飞书（国内版 Feishu），实现用户与 OpenClaw 的私聊可用。
5. 为 OpenClaw 建立多 agent 工作流，用于 Unity 项目 `D:\Unity\Unity_learning\Sunset`。
6. 逐步把“龙虾 / OpenClaw”从一个会聊天的模型，推进成一个能持续工作的系统：有角色、有外部记忆、有任务池、有恢复能力。
7. 用户明确提出：需要把线程记忆外部化，否则切换上下文时会丢失大量关键信息。

## 用户偏好与硬约束
- 用户环境：Windows 原生，明确不希望被强推 WSL / Linux 方案。
- 用户更看重：高置信、基于源码或文档验证的结论，不喜欢拍脑袋回答。
- 用户接受：英文 UI 可以保留部分专有名词，不再继续折腾全面汉化。
- 用户当前主入口：飞书比网页更重要；Feishu 是主要人机交互面。
- 用户想要的多 agent 方式：先手动并行、后自动编排；先能稳定做分析，再谈自动分发。
- 用户强调：要把工作沉淀为外部记忆文档，而不是只存在聊天上下文里。
- 用户当前最关键的后续方向：网络/浏览器能力、持续工作系统、Sunset 项目的多 agent 协作。

## 已完成的大项

### 1. OpenClaw 本地环境与基础运行
- 已在 Windows 本机环境中完成 OpenClaw 的基本落地，不走 WSL 作为主方案。
- 曾处理过 gateway token、Control UI、配置页卡死/保存异常、启动脚本等问题。
- 过程中多次出现配置错乱、页面报错、服务重启影响使用等问题，但最终恢复到可用状态。
- 本地核心配置路径已明确：`C:\Users\aTo\.openclaw\openclaw.json`

### 2. 模型与 provider 思路
- 用户使用的是中转接口，不是官方 token 直连。
- 曾提供一组 provider 配置，provider 名称为 `LZ`，`base_url = https://synai996.space/v1`，模型名应直接写真实模型名，而不是加 `lz/` 前缀。
- 多模型配置方向已经明确：
  - 聊天、读图、任务、编程可以分不同模型。
  - 用户不满足于单一 `main`，希望多个 agent / 多模型协同。

### 3. 飞书接入（核心成果）
飞书接入已经从“研究可行性”推进到“实机可用”。

已完成：
- 使用国内版飞书（Feishu）。
- 采用企业自建应用方案。
- 已创建应用并拿到 `App ID` / `App Secret`。
- 已开启机器人能力。
- 已按最小权限原则配置消息相关能力。
- 已启用事件订阅，并选择长连接 / WebSocket 模式。
- 已添加事件：`im.message.receive_v1`。
- 已完成发布前检查与后续发布。
- 已完成 pairing / 私聊授权流程。
- 当前飞书私聊已经可以收到 OpenClaw 回复。

飞书接入过程中的重要原则：
- 采用最小权限，不按“文档大礼包”一次性全开。
- 以单人自用、本机、本地优先为原则。
- 初期只聚焦私聊，不默认开放群组能力。
- 不暴露公网 webhook，优先长连接模式。

### 4. 汉化方向的结论
- 曾进行过较多中文化尝试。
- 用户后来明确表示：不要继续折腾汉化，稳定和可用性优先。
- 当前共识：不再把“全面中文化”作为主任务；专有名词保留英文即可。

### 5. 浏览器能力方向的澄清
- 用户真正需要的，不是给浏览器写插件，而是让 OpenClaw 在对话中能够触发“去网上搜索 / 打开浏览器 / 读取页面 / 辅助网页操作”等能力。
- 该方向后来被收束为：优先做“浏览器只读巡检 / 网络能力接入 / 资料搜索与页面读取”，而不是先做浏览器端扩展本身。
- 该部分并未在本线程中彻底落地，但方向已经明确。

### 6. Skills / 第三方能力接入规范
本线程中已经确立了一套重要规范：
- 默认只学习、只审核，不直接安装第三方 skills。
- 借鉴思路，不整包照搬。
- 任何 skill / GitHub 方案 / ClawHub 候选，都需要先做安全审查。
- 审查维度包括：是否执行外部命令、是否要额外 secret、是否改配置、是否接触文件/浏览器/聊天消息、是否有注入风险、是否依赖来路不明脚本、是否会把数据发给第三方、是否真贴合当前项目。
- 对用户当前项目更适合的 skills 类型已形成共识：
  1. 项目结构扫描 / brief 生成
  2. 架构文档生成
  3. 代码 review / diff 风险检查
  4. OpenClaw 配置安全巡检
  5. 渠道接入 checklist
  6. 浏览器只读巡检
  7. Unity 专项只读分析

### 7. 多 agent 总体方案已定型
线程中已经反复讨论并形成稳定方向：
- 不立即做复杂自动编排。
- 不先开 `broadcast` / `bindings` / `dynamicAgentCreation`。
- 先采用“分析并行、修改串行”的手动多 agent 工作流。
- 主角色结构：
  - `main`：总入口 / 自然语言总控 / 飞书聊天入口
  - `pm`：项目经理助手，负责拆任务、汇总与优先级
  - `reader`：代码阅读助手，负责找入口、梳调用链、提炼事实
  - `reviewer`：风险审查助手，负责识别架构风险与回归风险
  - `builder`：实施助手，在边界明确后做最小改动

### 8. 多 agent 具体配置进度
已完成过以下动作（但需要注意后续可能仍需复核）：
- 创建了 `pm`、`reader`、`builder`，后来又补建/重建了 `reviewer`。
- 曾为这些 agent 设定过预期模型分工：
  - `pm` → `LZ/gpt-5.4-fast`
  - `reader` → `LZ/gpt-5.4-xhigh-fast`
  - `reviewer` → `LZ/gpt-5.4-high`
  - `builder` → `LZ/opus-codex-5.3`
  - `main` → `LZ/gpt-5.4-xhigh-fast`
- 为多 agent 创建过角色说明文档：
  - `C:\Users\aTo\.openclaw\workspace-pm\ROLE.md`
  - `C:\Users\aTo\.openclaw\workspace-reader\ROLE.md`
  - `C:\Users\aTo\.openclaw\workspace-reviewer\ROLE.md`
  - `C:\Users\aTo\.openclaw\workspace-builder\ROLE.md`
- 还创建过飞书 + Sunset 多 agent 说明文档：
  - `C:\Users\aTo\.openclaw\workspace\FEISHU-SUNSET-MULTIAGENT.md`

## 关键踩坑与真实根因

### 1. “agent 看不到 Sunset”不是模型笨，而是配置与权限问题
用户在给 `reader` / `reviewer` 下发分析任务时，agent 多次回答“不能看到目录”或要求用户手动贴搜索结果。经过源码级排查，线程中已经得到高置信结论：

根因有三层：
1. `reader` / `reviewer` 最初并没有真正以 `Sunset` 作为 workspace 根。
2. 全局或相关 agent 的 `tools.profile` 一度是 `messaging`，而 `messaging` profile 不包含 `read` 工具。
3. 用户下发的自然语言有时偏抽象，模型会直接口头回答，而不是主动调用文件工具。

已验证过的源码依据：
- `src/agents/tool-catalog.ts`
- `src/agents/path-policy.ts`
- `src/agents/apply-patch.test.ts`
- `src/agents/pi-tools.read.host-edit-access.test.ts`

### 2. “把 workspace 做成 Junction 指向 Sunset”不可行
线程中验证过：
- 单纯在 agent workspace 中用 Junction / 目录链接指向 `Sunset` 根目录，并不能绕过 OpenClaw 的路径策略。
- 路径策略会解析真实路径，并阻止 workspace 逃逸。
- 因此，真正可行的方案不是做链接欺骗，而是直接把 agent 的 workspace 根配置到 `D:\Unity\Unity_learning\Sunset`，并配好允许读写的工具 profile。

### 3. 已部分修复并得到成功验证
线程中的关键修复结果：
- `reader.workspace` 已改为 `D:\Unity\Unity_learning\Sunset`
- `reader.tools.profile` 已改为 `coding`
- `reviewer.workspace` 已改为 `D:\Unity\Unity_learning\Sunset`
- `reviewer.tools.profile` 也曾被改为 `coding`

至少有一次已成功强制验证 `reader` 可读取 Sunset 顶层内容，能看到：
- `.claude`
- `.cursor`
- `.git`
- `.kiro`
- `.openclaw`
- `.vscode`
- `Assets`
- `Docx`
- `History`
- `Library`

说明问题的核心不是上游模型本身，而是：workspace 根、tools profile、提示方式。

## 当前工作系统层面的高价值共识

### 1. 飞书 + main 的作用
- 飞书 / `main` 应作为总控入口，适合自然语言发号施令、汇总、转派与总览。
- 用户可以直接在飞书中对 `main` 下达任务，而不是每次只在本地终端里敲命令。
- 但如果要精确调用某个特定 agent，目前手动命令方式仍更稳定可靠。

### 2. 多 agent 的正确节奏
- 先让 `reader` 与 `reviewer` 做并行分析。
- 让 `pm` 汇总，形成任务分解和优先级。
- 最后才让 `builder` 动手，且一次只做一个边界明确的小任务。
- 当前不应让多个 agent 同时改同一系统。

### 3. “持续工作系统”方向已被正式确认
通过阅读 `豆包_回应001.md`，本线程已经明确了未来方向：
- 真正要做的不是堆 prompt，而是搭建一个“持续工作操作系统”。
- 关键组成应包括：
  - 任务池（tasks）
  - 长期记忆（memory）
  - 当前进展（progress）
  - 阻塞记录（blockers）
  - 操作手册 / runbook
  - 验证闭环
  - 人工升级协议
- 目标不是“模型永远在线胡乱发挥”，而是“中断可恢复、任务可追踪、结果可验证、失败可升级”。

## 本线程中用户的显式需求与判断

### 用户的真实需求不是简单“换模型”
用户后续已经把需求说得很清楚：
- 希望通过多 agent 配置来解决不同任务类型，而不是仅仅在同一个 agent 里切模型。
- 聊天可以用更擅长交流或图片的模型。
- 编程 / 执行可以用更强的 coding 模型。
- 但工作流应该是“角色分工”，不是“单线程切换模型”。

### 用户对当前系统的期待
用户希望形成一种类似：
- 飞书作为自然语言入口
- main 负责总控
- pm / reader / reviewer / builder 形成专业分工
- 后续再接入浏览器能力、技能候选审核、外部记忆与任务流转

## 过程中的文件/文档参考
本线程中被重点参考或讨论过的内容包括：
- OpenClaw Feishu 文档与扩展代码
- 多 agent / 工具 profile / path policy 的源码与测试
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\2.0.0龙虾养殖\2.0持续工作建立\豆包_回应001.md`

## 当前已完成状态总结

### 已真正落地的事项
1. OpenClaw 已在 Windows 本地可运行。
2. 用户的 provider / 模型配置路线已基本打通。
3. 飞书接入已经可用，私聊能正常回复。
4. 多 agent 思路、角色分工、模型意图已基本确定。
5. `reader` 至少一次已被验证能真实读取 `Sunset` 项目目录。
6. `reviewer` 的根因也已定位，修复方向与 `reader` 一致。
7. 汉化不再作为主任务。
8. 技能接入与第三方扩展的安全规范已经定下。
9. “持续工作系统”方向已明确，需要外部化记忆与任务系统。

## 当前未完成 / 待复核事项

### A. 多 agent 配置稳定性仍需复核
由于本线程中对 `openclaw.json` 做过多次编辑和重建，当前虽然已有较高概率是可用的，但仍建议下次开工前先复核：
- `reader.workspace` 是否仍指向 `D:\Unity\Unity_learning\Sunset`
- `reader.tools.profile` 是否仍为 `coding`
- `reviewer.workspace` 是否仍指向 `D:\Unity\Unity_learning\Sunset`
- `reviewer.tools.profile` 是否仍为 `coding`
- `pm` / `builder` 是否保持预期 workspace 与模型
- `main` 是否保持飞书入口与总控角色

### B. reviewer 读盘能力还缺一次最终强验证
- `reader` 已有成功读目录的实证。
- `reviewer` 虽然已按相同方法修复，但本线程里缺少一次同等强度的最终验证记录。

### C. 飞书与多 agent 的自然语言路由尚未完全落地
- 当前飞书私聊已可用。
- 但“在飞书里自然地指定某个 agent 干活”还没有形成用户端极简方案。
- 是否需要在 `main` 内部建立更自然的派单约定、标签语法或固定工作流，仍待后续设计。

### D. 浏览器 / 网络能力尚未成为稳定工作链路
- 用户非常重视浏览器与网络能力接入。
- 但当前线程中尚未把这部分做成稳定、可复用、可验证的工作流。
- 这仍是后续高优先级事项之一。

### E. “持续工作系统”文档与骨架尚未正式建完
虽然方向已经明确，但本线程内还没有真正落地这些文件：
- `tasks.md`
- `progress.md`
- `blockers.md`
- `龙虾操作手册.md`
- 多轮运行 / 验证 / 升级协议

本次 `memory_0.md` 只是第一块外部记忆落盘。

## 建议的后续顺序
1. 先把本线程记忆稳定保存，并在新会话中引用。
2. 复核多 agent 当前配置的最终状态，尤其是 `reader` / `reviewer`。
3. 做一次最小验收：让 `reader`、`reviewer` 在 `Sunset` 根目录下各完成一次真实文件读取任务。
4. 设计飞书 → main → 子 agent 的自然语言派单协议。
5. 建立持续工作骨架：`tasks.md`、`progress.md`、`blockers.md`、`龙虾操作手册.md`。
6. 再推进浏览器 / 网络能力，使 OpenClaw 能可靠地进行网页读取与资料搜索。

## 给后续会话的快速提示
如果下一次会话要继续，请优先记住：
- 飞书已经可用，不要重复从零接入。
- 汉化不是当前重点。
- 真正的多 agent 阻塞点不是模型，而是 workspace 根和 tools profile。
- `reader` 已经证明可以读 `Sunset`，说明路线正确。
- 当前应先做“外部记忆 + 持续工作骨架 + agent 配置复核”，再谈更复杂的自动化。

## 本次落盘的意义
这份 `memory_0.md` 是当前线程的第一份线程级长期记忆。
它的作用是：
- 在新会话里快速恢复历史上下文
- 减少重复解释过去做过的事
- 保留已踩过的坑与高价值结论
- 作为后续 `progress` / `tasks` / `blockers` 体系的起点

---

## 2026-03-09 追加更新 - 慢响应事故与当前纠偏

### 新增阶段结论
本线程在飞书接入成功、多 agent 初步建立之后，进入了“主入口明显变慢”的事故排查阶段。

这次事故带来的重要认知：
- 用户当前最关心的，不是继续扩功能，而是**恢复龙虾/OpenClaw 主入口的快速稳定响应**。
- 只要主入口卡顿，多 agent、持续工作、浏览器能力这些上层设计都会失去意义。
- 因此当前阶段目标必须收敛：**先恢复主聊天入口的速度和可用性，再继续多 agent 规划。**

### 用户当前真正想要的最终结果（重新澄清）
用户不是单纯想“把 OpenClaw 配上就行”，而是要一个：
1. **Windows 原生稳定可用** 的 OpenClaw；
2. **飞书可用**，能作为主要人机入口；
3. **主入口响应快、可日常直接聊天**；
4. **多 agent 存在，但不能破坏主入口体验**；
5. 最终可以服务 `Sunset` 项目分析、持续工作与后续网络/浏览器能力。

换句话说：
- `main` 必须先像一个好用的聊天入口；
- `reader/reviewer/pm/builder` 是增强层，不应反过来拖垮主入口。

### 对当前问题的高置信判断
已确认：
- `main` 实际使用的模型不是 gemini，而是 **`LZ/gpt-5.4-xhigh-fast`**。
- 飞书会话标识中的 `ou_xxx` 是飞书 `open_id`，表示该直连私聊会话。
- 慢响应并非 UI 幻觉，主会话日志里确实出现了几十秒到百秒级响应：
  - 约 105.8s
  - 约 70.4s
  - 约 69.1s
  - 约 43.5s
- 之后对 `main` 进行短消息压测，也能测到明显偏慢。

### 已定位的关键诱因
1. `main` 工作区的 `AGENTS.md` 原先会主动要求主会话启动时读取：
   - `SOUL.md`
   - `USER.md`
   - `memory/YYYY-MM-DD.md`
   - `MEMORY.md`
2. `main` 已累积了较重的旧会话历史，导致输入 token 持续膨胀。
3. 多 agent 折腾之后，当前的 `main` 已不再像“轻聊天入口”，而是变成了“重上下文编排入口”。
4. 不排除中转本身也不算快，但**当前主要矛盾是主入口过重**。

### 已做过的纠偏尝试
- 已将 `main` 工作区的启动规则改轻：
  - 不再默认每次自动加载全部记忆文件；
  - 普通短聊天应优先直接回复；
  - 仅在需要连续上下文时才加载长期/日记记忆。
- 已补创建缺失的 `MEMORY.md` 和 `memory/2026-03-08.md` / `memory/2026-03-09.md` 占位文件，避免空读报错。
- 新开 `main` 会话进行压测后，相比旧重会话已有一定改善，但仍未完全恢复到用户期望速度。

### 当前达成的关键判断
全量“回退到某个神秘旧版本”不是最优解，因为：
- 本地并没有一个精确覆盖“飞书已通、浏览器已通、但尚未污染主入口”的完美快照；
- 粗暴全量回退风险大，且未必能精准回到用户最满意状态。

当前更合理的方向是：
**软重置主入口，而不是硬回退整套系统。**

### 当前这一步真正该做什么
不是继续扩多 agent，也不是继续汉化，更不是继续加功能。

**当前阶段唯一正确任务：**
1. 保留飞书、保留可用模型配置、保留浏览器接入；
2. 只把 `main` 恢复成“轻、快、稳定”的单入口；
3. 将多 agent 临时降级为“存在但不干扰主入口”；
4. 必要时重置旧的主会话（尤其是飞书直连旧会话）；
5. 验证主入口恢复速度后，再谈后续规划。

### 给后续会话/后续执行者的指令性提示
如果继续本线程工作，不要偏离以下优先级：
- **第一优先级：恢复主入口速度**
- 第二优先级：确认飞书仍可正常使用
- 第三优先级：保证 `reader/reviewer` 等专项 agent 还在，但不影响日常聊天
- 第四优先级：等主入口恢复后，再继续多 agent 工作流和 Sunset 任务池建设

### 当前建议执行策略
采用“软重置主入口”策略：
- 不全量回退；
- 不先动飞书接入；
- 不先删所有 agent；
- 优先清理/重置 `main` 的重会话和重上下文负担；
- 目标是恢复到“昨晚飞书刚通时的体感”：快、直接、可日常聊天。

## 2026-03-09 最新对齐：先救 `main` 响应速度

### 当前最终目标（重新校准）
- 不再继续折腾“功能越堆越多”。
- 当前最高优先级是把 OpenClaw 恢复成“可稳定日常使用”的状态。
- 对用户而言，真正的目标顺序已经收敛为：
  1. Windows 原生稳定运行 OpenClaw。
  2. 保留飞书主入口可用。
  3. 保留当前模型与浏览器接入成果。
  4. 让 `main` 回到轻量、快速、适合日常聊天的状态。
  5. 多 agent 继续保留，但不能拖慢主聊天入口。
  6. 等主入口恢复后，再继续推进浏览器能力与多 agent 工作流。

### 当前问题的重新判断
- 用户当前最不满意的不是“缺少新功能”，而是“龙虾整体变慢、主入口体验变差”。
- 已确认这不是模型配错成 Gemini：当前 `main` 实际仍是 `LZ/gpt-5.4-xhigh-fast`。
- 已确认 UI 里出现的 `ou_...` 是飞书 `open_id` 对应的私聊会话，不是异常模型或异常 agent。
- 已确认慢响应是现实存在的问题，不是用户错觉；旧的 `main` 会话上下文已经偏重。
- 已确认最合理策略不是“全量回滚”，因为那会连飞书、浏览器、多模型配置一起倒退。

### 当前这一步要做什么
- 目标不是全盘重装，而是执行一次“主入口软重置”。
- 软重置原则：
  - 保留飞书。
  - 保留模型配置。
  - 保留浏览器接入。
  - 尽量保留多 agent 定义。
  - 只清理 `main` 的重会话与脏上下文。
- 当前计划中的直接动作：
  1. 备份 `main` 的 session store 与对应 transcript。
  2. 仅移除两个最重的 `main` 会话条目：
     - `agent:main:main`
     - `agent:main:feishu:direct:ou_3ca4e722c1ef9c31597342e79e6f9670`
  3. 保持 gateway 与飞书配置不回退。
  4. 清完后做短 smoke test，验证新会话是否恢复轻量。

### 当前阶段结论
- 现在不是继续扩展功能的阶段，而是“把主入口速度救回来”的阶段。
- 判断标准不是配置好不好看，而是：网页与飞书短消息能否快速响应。
- 若这一步成功，后续多 agent、浏览器能力、Feishu 工作流才值得继续深化。

## 2026-03-09 执行结果：`main` 软重置与轻量化完成

### 本轮实际动作
- 已备份 `main` 的 session store 与对应旧 transcript。
- 已执行 `main` 软重置：删除了重会话键，让主入口重新起新 session。
- 已保留飞书、浏览器、多 agent、模型目录；没有做全量回滚。
- 已将 `main` 从“重型万能入口”收缩为“轻量聊天入口”：
  - `main.tools.profile = messaging`
  - `main.tools.alsoAllow = [web_search, web_fetch, image, tts]`
  - `main.skills = []`
- 过程中曾尝试为 `main` 单独设置 `params.reasoningEffort`，但当前 OpenClaw 版本 schema 不接受 `agents.list[].params`，已即时回退，未留下脏配置。

### 本轮关键验证
- 已验证：当前慢响应并非 2API / 中转上游本身慢。
- 直接调用同一上游 `gpt-5.4-xhigh-fast`，最短测试约 2 秒完成。
- 因此本轮确认：瓶颈主要来自 OpenClaw 主入口自身的 system prompt / tool schema / skills prompt 负载，而不是 provider 本身。

### 优化前后对比（结论级）
- 旧 `main` / 飞书直聊旧会话：曾出现约 40s~100s 级别响应。
- 清旧会话后但未瘦身：新会话仍有约 10k~14k token 级别首包负担。
- 本轮瘦身后：
  - 新 `main` 首条测试输入负担降到约 6694 token。
  - 同轮 transcript 内模型实际首答时间约 9 秒级。
  - 连续第二条已进入 cache 命中路径，显著轻于旧重会话。
- 这说明当前已经从“异常缓慢”恢复到“可接受且明显改善”的状态，但距离用户体感中的“超快”仍有优化空间。

### 当前系统理解（更新后）
- `main` 的正确定位不是“什么都挂上去”，而是“轻量主聊天入口”。
- 重能力应尽量放到专门 agent（如 `pm` / `reader` / `reviewer` / `builder`）或后续专门网络/浏览器 agent 上。
- 飞书继续作为主用户入口没有问题；因为飞书直聊下一次会新建轻量 session，不再继承之前的重上下文。

### 当前已完成 / 未完成
**已完成**
- 主入口软重置完成。
- 主入口工具/技能瘦身完成。
- 主入口速度已明显恢复。
- 配置仍然有效，未破坏飞书或多 agent 目录。

**未完成**
- 还没有做“最终理想态”的浏览器/网络专用 agent 设计。
- 还没有对飞书新会话做一轮用户侧真实体验复测。
- 还没有继续推进多 agent 在飞书中的工作流编排。

### 下一阶段最合理动作
1. 让用户直接在网页主聊天与飞书各发一条极短消息做体感验证。
2. 如果网页/飞书仍嫌慢，再考虑把 `main` 收缩为纯 `messaging`，把 web/browser 能力彻底迁到专用 agent。
3. 在主入口稳定后，再继续浏览器能力接入与多 agent 编排，不再让这些能力反向拖慢 `main`。

## 2026-03-09 需求再校准：双入口、单系统、分层协作

### 这次用户的新澄清
用户明确指出：
- 不应把问题简单理解为“飞书替代 `main`”。
- `main` 窗口和飞书都应该可用，而且工作场景中需要可切换。
- 飞书需要承载完整自动化流程，但其本质仍然是“龙虾 / OpenClaw 系统本体”在工作，不是飞书自己形成独立系统。
- 需要重新梳理：
  - 用户真实需求是什么
  - 当前系统真实处境是什么
  - 后续工作该如何分优先级推进

### 重新理解后的需求结论
本线程的最终目标应重新明确为：
1. 保留 `main` 作为本地轻量入口、调试入口、救火入口。
2. 保留飞书作为日常工作主入口、总控派单入口。
3. 两者都属于同一个 OpenClaw / 龙虾系统的“表面层”，而不是彼此替代关系。
4. 真正要建设的是后面的控制层与执行层：
   - `pm` 负责总控
   - `reader` / `reviewer` / `builder` / 未来的浏览器角色负责执行
5. 最终目标不是“多 agent 存在”，而是“多 agent 可工作、可派单、可汇总、可沉淀”。

### 当前处境的重新判断
当前系统状态可概括为：
- `main` 已经被修复为轻量入口，速度问题阶段性缓解。
- 飞书入口已可用，但仍未成为真正的自动化总控入口。
- 多 agent 已配置，但尚未形成自动编排链。
- 浏览器基础设施已存在，但尚未纳入正式工作流。
- 当前核心问题不再是“龙虾打不开”，而是“角色关系与工作流尚未整理完成”。

### 当前最合理的结构理解
应把系统理解为四层：
- 表面层：`main` 窗口、飞书
- 控制层：`pm`
- 执行层：`reader` / `reviewer` / `builder` / future research
- 沉淀层：memory / tasks / progress / report

### 这轮分析后的优先级结论
后续工作优先级应重排为：
1. 保持 `main` 继续轻量、稳定、可救火。
2. 把飞书直聊正式绑定给 `pm`，让飞书成为日常总控入口。
3. 明确 `pm` 的派单模板与工作边界。
4. 建立第一版 `pm -> reader/reviewer/builder` 的编排链。
5. 增设独立的浏览器 / research 角色，不回塞 `main`。
6. 修正 `builder` 的工作区与实施边界。
7. 建立 tasks / progress / memory 的正式沉淀机制。
8. 在以上稳定后，再考虑让 `main` 也具备更自然的 PM 模式切换能力。

### 当前阶段最关键结论
当前最关键、最正确的下一步仍然是：
- 不是继续堆 `main`
- 不是继续纠结飞书是不是“另一个系统”
- 而是让飞书直聊真正绑定到 `pm`，使 `pm` 上岗成为总控角色

这一步是系统从“已恢复可用”进入“开始形成真实工作流”的分水岭。

---

## 会话追加 - 2026-03-09 10:30

**用户本轮目标**：
- 彻查当前 OpenClaw 模型配置是否被非 OpenAI 模型污染。
- 明确 `main` 必须使用 `gpt-5.4-xhigh-fast`。
- 删除 Gemini / Grok / Opus 等非 OpenAI 模型相关智能体与配置，只保留 OpenAI 系列。
- 保留线程记忆，避免后续再次迷失。

**本轮完成事项**：
1. 读取并核查当前实际生效配置文件：`C:\Users\aTo\.openclaw\openclaw.json`。
2. 发现配置文件存在真实损坏：`builder.identity.emoji` 一段 JSON 被写坏，导致原文件并非严格合法 JSON。
3. 在修改前创建备份：`C:\Users\aTo\.openclaw\openclaw.json.bak-20260309-102307`。
4. 修复损坏 JSON 后，完成 OpenAI-only 清理：
   - 删除 `gemini-3-pro-preview`
   - 删除 `gemini-2.5-flash`
   - 删除 `opus-codex-5.3`
   - 删除 `grok-3`
   - 删除 `grok-4.2-fast`
5. 保留的模型仅剩：
   - `gpt-5.4-xhigh`
   - `gpt-5.4-xhigh-fast`
   - `gpt-5.4-high`
   - `gpt-5.4-fast`
6. 同步清理 `agents.defaults.models` 中的非 OpenAI 条目。
7. 同步清理 `agents.list` 中的非 OpenAI 模型代理项。
8. 修正并确认当前核心 agent：
   - `main` -> `LZ/gpt-5.4-xhigh-fast`
   - `pm` -> `LZ/gpt-5.4-fast`
   - `reader` -> `LZ/gpt-5.4-xhigh-fast`
   - `reviewer` -> `LZ/gpt-5.4-high`
   - `builder` -> `LZ/gpt-5.4-xhigh`
9. 修正身份串线问题：
   - `pm.identity` 还原为 `Sunset PM / 📋`
   - `builder.identity` 固定为 `Sunset Builder / 🛠️`
10. 确认当前监听端口 `18789` 正在工作，gateway 进程仍在监听本地回环地址。

**本轮关键事实结论**：
- `main` 当前主模型已经明确为：`LZ/gpt-5.4-xhigh-fast`。
- `main` 当前 fallback 仅剩 OpenAI 系列：
  - `LZ/gpt-5.4-high`
  - `LZ/gpt-5.4-fast`
- 当前配置文件中已无 `gemini` / `grok` / `opus-codex` 模型残留。
- 用户在 `main` 里看到的 `Fallback active`，今后即使再次出现，也只会落到 OpenAI 备选，不再和 Gemini / Grok / Opus 有关。

**联网问题复核结论**：
- 飞书侧“没有联网操作”不是这次模型清理导致的。
- 已复核 `main` 会话日志：它实际调用过 `web_search`。
- 失败根因是搜索提供方没有配置 key，具体报错为：`missing_perplexity_api_key`。
- 也就是说：
  - 不是 `main` 没有 `web_search` 工具；
  - 不是 Gemini/Grok 模型污染导致；
  - 而是 `web_search (perplexity)` 缺少 API key。
- 若后续要恢复真正的联网搜索，需要补齐搜索 provider 配置，而不是继续折腾模型列表。

**本轮遗留事项**：
- `openclaw doctor` 仍会提示一个 Feishu 单账号兼容迁移提醒；当前判断它不是本轮 OpenAI-only 清理的阻塞项。
- 若用户后续还要“飞书内稳定联网搜索”，下一步应单独处理 `tools.web.search` 的 provider/key，而不是再动 agent 模型。
- 如需让 UI 立刻完全刷新到新配置，后续可在用户空闲时做一次温和重启，但本轮未强行中断现有 gateway。

**当前建议工作流状态**：
- `main`：总控入口，默认使用 `gpt-5.4-xhigh-fast`。
- `pm`：任务拆分与汇总。
- `reader`：Sunset 代码阅读。
- `reviewer`：Sunset 风险审查。
- `builder`：明确边界后的最小改动执行。
- 网络搜索：暂未真正恢复，缺搜索 provider key。

---

## 会话追加 - 2026-03-09 11:54

**用户本轮目标**：
- 在进入当前阶段任务之前，重新检查基础设施。
- 回顾之前提过但未真正落地的基础功能。
- 优先补齐当前阶段前必须完成的底座，而不是直接推进新任务。

**本轮完成事项**：
1. 重新读取线程 memory 与阶段重整文档，整理出当前仍未真正落地的基础能力。
2. 重新审计当前 `openclaw.json`、agent 配置、browser 配置、Feishu 配置。
3. 实测确认：
   - `reader` 能看到 `Sunset` 顶层目录
   - `reviewer` 能看到 `Sunset` 顶层目录
4. 发现并确认一个重要基础设施问题：
   - 之前 agent 有过 `Gateway agent failed; falling back to embedded`
   - 说明之前 gateway 并非始终稳定在线
5. 手动重新拉起 gateway，并确认：
   - 本地 `18789` 已重新监听
   - Feishu WebSocket 已重连
6. 启动托管浏览器 `openclaw` profile，并完成真实能力验证：
   - 打开 `https://example.com`
   - 列出标签页
   - 执行 snapshot 成功读出页面结构
7. 为 `main` 补齐 `browser` 工具权限，使主入口具备浏览器调用入口。
8. 执行 `openclaw browser extension install`，将 Chrome / Edge relay 扩展安装到稳定路径。
9. 更新 `main` 会自动加载的本地上下文文件：
   - `IDENTITY.md`
   - `USER.md`
   - `TOOLS.md`
   - `AGENTS.md`
10. 在线程工作区新建基础沉淀文件：
   - `2026-03-09_基础设施复核与补齐.md`
   - `tasks.md`
   - `progress.md`
   - `blockers.md`
   - `龙虾操作手册.md`

**本轮关键结论**：
- 当前系统已经不再是“看起来能用、实则大量靠兜底”。
- `reader` / `reviewer` 的读盘能力现在都有实测支撑。
- gateway 已重新回到真实在线状态。
- 托管浏览器链路已经真正能打开网页并读取页面。
- `main` 入口现在也拥有浏览器权限，不再只有 `web_search/web_fetch` 这类入口。

**当前剩余阻塞**：
1. `web_search` 仍缺 provider key，报错仍是 `missing_perplexity_api_key`
2. Chrome / Edge relay 还需要在浏览器里手动 `Load unpacked`
3. 自动派单 / 自动编排仍未正式落地，当前仍以“飞书或 main 收口 + 手动派单”为主

**对下一阶段的判断**：
- 现在已经可以进入“当前阶段任务本身”的正式拆解。
- 不需要再回到“先救系统能不能活”那种阶段。
- 但进入前必须继续明确区分：
  - 已补齐的基础能力
  - 仍然存在的真实 blocker

---

## 会话追加 - 2026-03-09 13:50

**用户本轮目标**：
- 自动完成能做的浏览器与联网基础设施配置。
- 查清 `web_search` 为什么仍不可用。
- 在基础设施稳定后，开始下一阶段任务拆解。
- 用户允许自主运行，并要求最后汇报可交付状态。

**本轮完成事项**：
1. 重新审计 Chrome / Edge relay 认证链路，定位到 relay 漂移并非模型问题。
2. 发现仓库根 `.env` 仍残留旧的 `OPENCLAW_GATEWAY_TOKEN`，它会覆盖/污染运行时环境。
3. 将仓库根 `.env` 中的 `OPENCLAW_GATEWAY_TOKEN` 对齐到当前 `C:\Users\aTo\.openclaw\openclaw.json` 中的 token。
4. 补强 `D:\1_AAA_Program\OpenClaw\start-openclaw-hidden.ps1`：启动 Gateway 前显式读取当前配置 token 并写入进程环境。
5. 修复 `C:\Users\aTo\.openclaw\browser\edge-extension\*` 与其嵌套副本中的硬编码旧 token，避免 Edge 扩展继续回退到旧凭据。
6. 使用修复后的启动脚本重新拉起 Gateway，确认以下端口重新监听：
   - `18789` Gateway
   - `18791` browser control
   - `18792` chrome relay
   - `18793` edge relay
7. 重新验证 relay 认证：
   - `18791` 当前 token 认证通过
   - `18792` 当前 token/派生 token 认证通过
   - `18793` 当前 token/派生 token 认证通过
8. 验证托管浏览器能力：
   - `pnpm openclaw browser open https://example.com` 成功
   - `openclaw` profile 处于 running 状态
9. 验证 `main` 的联网能力边界：
   - `web_fetch` 成功
   - `web_search` 仍失败，但已确认失败根因是缺真实搜索 provider key，而不是网关、模型或浏览器配置问题。
10. 在线程工作区重写/新增阶段文档：
   - `progress.md`
   - `blockers.md`
   - `tasks.md`
   - `2026-03-09_浏览器与WebSearch说明.md`
   - `2026-03-09_下一阶段拆解.md`

**本轮关键结论**：
- 浏览器 relay 的主要根因已经查实并修复，Chrome/Edge 两条 relay 认证链路现在都恢复正常。
- 当前真正未完成的不是底层认证，而是浏览器 UI 最后一跳：用户仍需在 Chrome/Edge 中手动 `Load unpacked` 并点击扩展附着标签页。
- `web_search` 之所以还不行，是因为它要求独立搜索 provider key；现有模型中转 key 不能直接替代。
- 当前最现实可用的联网方案是：`browser + web_fetch`；`web_search` 属于待补 provider key 的增强项。
- 现在可以开始设计多 agent 的入口统一与半自动派单，但不建议立刻跳到全自动自治。

**涉及文件/路径**：
- `D:\1_AAA_Program\OpenClaw\.env`
- `D:\1_AAA_Program\OpenClaw\start-openclaw-hidden.ps1`
- `C:\Users\aTo\.openclaw\browser\edge-extension\background.js`
- `C:\Users\aTo\.openclaw\browser\edge-extension\options.js`
- `C:\Users\aTo\.openclaw\browser\edge-extension\chrome-extension\background.js`
- `C:\Users\aTo\.openclaw\browser\edge-extension\chrome-extension\options.js`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\progress.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\blockers.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\2026-03-09_浏览器与WebSearch说明.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\2026-03-09_下一阶段拆解.md`

**验证结果**：
- `pnpm openclaw browser profiles`：`openclaw/chrome/edge` 三个 profile 已正常注册
- `http://127.0.0.1:18791/profiles`：当前 token 返回 `200`
- `http://127.0.0.1:18792/json/version`：当前 token 与派生 token 返回 `200`
- `http://127.0.0.1:18793/json/version`：当前 token 与派生 token 返回 `200`
- `pnpm openclaw agent --agent main --message "...web_fetch..."`：返回“成功”
- `pnpm openclaw agent --agent main --message "...web_search..."`：返回“失败”，与缺搜索 key 一致

**遗留问题 / 下一步**：
1. 用户需要在 Chrome / Edge 内手动加载扩展并附着实际标签页。
2. 若要启用真正的 `web_search`，需要补一个真实可用的搜索 provider key。
3. 下一阶段建议先做：
   - `main/Feishu -> pm` 统一入口协议
   - `pm -> reader/reviewer/builder` 半自动派单协议
4. 自动派单建议放到浏览器最后一跳和搜索策略稳定之后再试点。

---

## 会话追加 - 2026-03-09 14:05

**本轮稳定结论**：
- 当前 Codex 线程在这台机器上不需要额外 MCP，就可以借助本地 OpenClaw 的浏览器 relay / browser 控制链路去操作 `http://127.0.0.1:18789/agents` 这类本地控制台页面。
- 这和“我直接看用户真实屏幕”不是一回事；本质是：通过 OpenClaw 本地浏览器控制能力读取页面结构、点击、输入、切换页面。
- 前提是目标浏览器标签页已经通过 OpenClaw Browser Relay 成功附着（扩展徽标 `ON`）。
- 因此后续若用户要求“像龙虾一样直接在 agents 页面点配置”，可以直接走本地浏览器控制链路，不必再引入额外 MCP。
- 若用户更看重可复现和稳定性，也仍可退回到直接改配置文件的方式；两条路径都可用。

---

## 会话追加 - 2026-03-09 16:16

**用户本轮目标**：
- 核实 `main`、`pm`、`reader`、`reviewer`、`builder` 的工具权限是否已经按“最小权限、多 agent 分工”真正落地。
- 判断用户在 `/agents` 页面看到的“几乎全允许”是否符合设计，还是配置/运行态/UI 展示存在偏差。
- 在不再把系统搞乱的前提下，把本轮结论写入线程 memory，避免后续继续丢上下文。

**本轮完成事项**：
1. 重新核对 `C:\Users\aTo\.openclaw\openclaw.json` 当前代理配置。
2. 确认当前文件层配置已经是按角色收紧后的版本，而不是全权限版本：
   - `main`：`messaging` + `web_search/web_fetch/browser/image/tts`
   - `pm`：`messaging` + `read/web_search/web_fetch/memory_search/memory_get/sessions_spawn/subagents/browser/agents_list`
   - `reader`：`minimal` + `read/exec/web_search/web_fetch/memory_search/memory_get/browser`
   - `reviewer`：与 `reader` 同级别
   - `builder`：仅 `group:fs/exec/group:memory/sessions_list/sessions_history/session_status`
3. 重新执行 `pnpm openclaw agents list`，确认四个 Sunset worker 都已绑定到 `D:\Unity\Unity_learning\Sunset` 工作区。
4. 重新执行 `pnpm openclaw gateway status`，确认网关 RPC probe 正常。
5. 直接通过 `pnpm openclaw gateway call config.get --json` 向运行中的 Gateway 读取当前生效配置，确认运行态已经读到了上述收紧后的 agent 工具策略，而不是只有磁盘文件被改了。
6. 查阅 `src/agents/tool-catalog.ts` 与 `docs/tools/index.md`，确认：
   - `messaging` 不是“全权限”，而是消息相关基础能力
   - `minimal` 不是“全权限”，默认只有 `session_status`
   - `alsoAllow` 是在 profile 基础上附加少量工具，不等于 unrestricted

**本轮关键结论**：
- 现在 `pm`、`reader`、`reviewer`、`builder` 在配置层和运行态都已经不是“全部允许”。
- 如果 `/agents` 页面仍看起来像“全都开了”，更大概率是：
  1. 页面没有刷新到最新配置；
  2. UI 把 profile 展开成了多个有效勾选，视觉上像“很多都开了”，但这不等于 `full`。
- 当前权限设计是符合我们之前的目标的：
  - `main`：通用入口
  - `pm`：调度/汇总
  - `reader` / `reviewer`：只读分析为主
  - `builder`：仅执行修改与必要命令

**涉及文件或路径**：
- `C:\Users\aTo\.openclaw\openclaw.json`
- `src/agents/tool-catalog.ts`
- `docs/tools/index.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\memory_0.md`

**验证结果**：
- `pnpm openclaw agents list`：通过
- `pnpm openclaw gateway status`：RPC probe `ok`
- `pnpm openclaw gateway call config.get --params "{}" --json`：返回的 `resolved.agents.list` 与当前最小权限设计一致
- `pnpm openclaw doctor --non-interactive`：配置可解析；仍提示 Feishu 结构迁移建议与 gateway auth 建议，但不影响本轮 agent 工具权限判断

**遗留问题 / 下一步**：
1. 让用户在 `http://127.0.0.1:18789/agents` 页面做一次硬刷新或点击 `Reload Config`，确认 UI 不再展示旧状态。
2. 如果 UI 仍异常，再进一步检查 `/agents` 页面是否存在“profile 展开后视觉上像全开”的展示问题。
3. 后续再继续推进 `main/Feishu -> pm -> reader/reviewer/builder` 的统一入口与半自动派单，不再回头折腾已落地的基础权限设计。

---

## 会话追加 - 2026-03-09 16:22

**本轮澄清结论**：
- `builder` 在 `/agents` 的 Tools 页面出现 `This agent is using an explicit allowlist in config` 是预期行为，不是新 bug。
- 原因是 `builder` 使用了显式白名单：
  - `group:fs`
  - `exec`
  - `group:memory`
  - `sessions_list`
  - `sessions_history`
  - `session_status`
- 这会让 UI 把 `builder` 视为“由配置严格接管的工具集合”，因此提示去 `Config` 页或配置文件里改，而不是依赖 Tools 页的自由切换。
- `pm / reader / reviewer` 用的是 `profile + alsoAllow`，所以它们看起来更像普通可调模式；`builder` 不一样，是故意锁得更死，因为它承担真实修改职责，风险最高。

**关键判断**：
- 这是符合我们“最小权限 + builder 最危险所以要硬锁白名单”的设计的。
- 如果后续想让 `builder` 在 Tools 页更容易手动调，可以改成 `profile + alsoAllow` 路线，但安全边界会更松，不建议现在改。

---

## 会话追加 - 2026-03-09 16:28

**本轮阶段判断**：
- 当前不再处于“修基础设施”的阶段，而是正式进入“拆分步骤的第一步”。
- 经过回顾，当前最该推进的不是继续折腾 `main`、浏览器展示或权限 UI，而是开始落实：
  - `main / Feishu -> pm` 统一入口协议
  - `pm -> reader / reviewer / builder` 半自动派单协议

**为什么是这一步**：
- 基础设施已经够用：
  - Gateway 正常
  - Feishu 可用
  - `main` 可用
  - 浏览器基础链路可用
  - worker 权限已收紧
- 当前真正缺的是“控制层与执行层的协作秩序”，不是“入口能不能回话”。

**下一步应做的事情**：
1. 先定义阶段一文档：入口协议与角色契约。
2. 明确 `main` 和 Feishu 发来高层任务时，`pm` 必须先输出什么格式的拆解单。
3. 明确 `reader` / `reviewer` / `builder` 各自只接什么类型的子任务、输出什么结果、禁止做什么。
4. 明确回收链：worker -> `pm` -> 用户 的汇总格式。
5. 明确第一阶段只做“半自动派单”，不做全自动自治。

**执行边界**：
- 第一阶段优先产出规则、模板、派单格式和验收标准。
- 不直接扩大到自动执行或自动改代码。

---

## 会话追加 - 2026-03-09 16:45

**用户本轮目标**：
- 排查 `main` / 飞书消息发出后无法正常处理的问题。
- 判断这是不是严重问题，并优先修复到“至少消息能稳定回话”。

**本轮完成事项**：
1. 检查 `main` 与飞书会话 transcript：
   - `agent:main:main`
   - `agent:main:feishu:direct:ou_3ca4e722c1ef9c31597342e79e6f9670`
2. 在 transcript 中确认故障不是前端假象，而是真实的模型链超时：
   - 飞书消息 `今天天气怎么样？` 先出现 `terminated`
   - 随后 fallback 到其他模型仍继续出现 `Request timed out.`
   - `main` 会话中新的“只回答两个字：收到”同样超时
3. 直接绕过 OpenClaw，用同一上游 `https://synai996.space/v1` 做独立验证：
   - `POST /v1/responses` 可快速返回
   - `POST /v1/chat/completions` 也可快速返回
4. 由此判断当前问题不是“整条上游完全死了”，而是 OpenClaw 当前给 `LZ` 使用的 `openai-responses` 适配路径与该中转的兼容性/稳定性存在问题。
5. 对本地 OpenClaw 配置做最小修复：
   - 将 `C:\Users\aTo\.openclaw\openclaw.json` 中 `models.providers.LZ.api`
   - 从 `openai-responses`
   - 改为 `openai-completions`
6. 验证修复效果：
   - `pnpm openclaw config validate` 通过
   - `pnpm openclaw gateway call config.get --json` 确认运行态已读到 `api: openai-completions`
   - `pnpm openclaw agent --agent main --message "只回答两个字：收到"` 成功返回 `收到`

**关键结论**：
- 这是严重问题，属于 P0 级可用性故障；因为它会直接导致 `main` 与飞书入口都“看起来能发消息，但实际不回话”。
- 根因更接近：
  - 当前 `LZ` 中转对 OpenClaw 所走的 `openai-responses` 适配链不稳定
  - 而不是 Gateway、会话路由、Feishu 链路、或者 agent 权限问题
- 目前最小且有效的修复方案是：对该中转改用 `openai-completions` 适配。

**涉及文件/路径**：
- `C:\Users\aTo\.openclaw\openclaw.json`
- `C:\Users\aTo\.openclaw\agents\main\sessions\3d2ea71a-559f-4569-a7ce-71ec2f37ef12.jsonl`
- `C:\Users\aTo\.openclaw\agents\main\sessions\f1e7cbb9-e277-42e1-acd7-0e967a2235ed.jsonl`

**验证结果**：
- 修复前：`main` / 飞书 transcript 连续出现 `Request timed out.`
- 修复后：CLI 直接调用 `main` 已能稳定返回短答复 `收到`

**遗留问题 / 下一步**：
1. 让用户在网页 `main` 窗口和飞书里各自再发一条最简单的短消息，确认真实入口已恢复。
2. 若恢复稳定，则继续进入“拆分步骤第一步”，不再回头折腾模型链。
3. `web_search` 仍是独立问题，继续受制于 `BRAVE_API_KEY` 缺失，与本轮主故障无关。

---

## 会话追加 - 2026-03-09 20:20

**用户本轮目标**：
- 继续收口当前线程，把“真正拖慢 `main` 的东西”彻底查清并处理。
- 在不粗暴回滚飞书、多 agent、浏览器能力的前提下，让主入口重新轻快可用。
- 按项目规则把这轮高价值结论写回 workspace memory 和线程 memory。

**本轮完成事项**：
1. 复核当前运行态与磁盘配置，确认 Gateway 正在使用：
   - `LZ`
   - `openai-completions`
   - `main -> gpt-5.4-xhigh-fast`
2. 直接测上游 `https://synai996.space/v1/chat/completions`，确认短消息响应约 `1455ms`，说明中转本身不是当前主瓶颈。
3. 用 `node openclaw.mjs agent --agent main --json` 对 `main` 做真实压测，定位到：
   - 主入口旧会话仍在复用同一 `agent:main:main`
   - prompt token 约 `10151`
   - 短答复常见在 `20s~60s`
4. 读取当前 `main` 的 `sessions.json` 与 transcript，确认瓶颈来自：
   - 旧主会话历史持续膨胀
   - `C:\Users\aTo\.openclaw\workspace` 下被注入的提示文件过重
5. 无损备份：
   - `C:\Users\aTo\.openclaw\workspace\prompt-backup-20260309-201141`
   - `C:\Users\aTo\.openclaw\agents\main\sessions\backup-soft-reset-main-20260309-201141`
6. 精简主工作区提示文件：
   - 重写 `C:\Users\aTo\.openclaw\workspace\AGENTS.md`
   - 重写 `C:\Users\aTo\.openclaw\workspace\SOUL.md`
   - 重写 `C:\Users\aTo\.openclaw\workspace\IDENTITY.md`
   - 重写 `C:\Users\aTo\.openclaw\workspace\TOOLS.md`
   - 重写 `C:\Users\aTo\.openclaw\workspace\USER.md`
   - 删除 `C:\Users\aTo\.openclaw\workspace\BOOTSTRAP.md`
7. 对 `main` 做软重置：
   - 仅删除 `C:\Users\aTo\.openclaw\agents\main\sessions\sessions.json` 中的 `agent:main:main` 索引
   - 不删除旧 transcript 文件
8. 通过 `start-openclaw-hidden.ps1` 温和重启 Gateway，让运行态吃到新 prompt 与新主会话。
9. 重启后复测：
   - 新 `main` 会话 id：`13b6ef5a-c015-494c-99e6-eea2ef81643d`
   - 主入口 prompt token 降至 `5325`
   - 短答复耗时降至约 `9397ms`
   - `main` 调用 `web_fetch` 访问 `https://example.com` 并返回“成功”通过

**本轮关键结论**：
- 当前最主要的慢点已从“模型 / 中转 / Gateway 失效”切换成了“主入口 prompt 与旧主会话过重”。
- 本轮最有效的修复不是换模型，而是：
  1. **瘦身主工作区注入文件**
  2. **软重置 `main` 主会话**
  3. **重启 Gateway 让新状态真正生效**
- 这次修复后，`main` 的真实体感已明显恢复；虽然还没有接近上游直连的 1.5 秒级，但已经从之前的 20~60 秒级别显著下降。

**涉及文件或路径**：
- `src/infra/json-files.ts`
- `src/infra/json-files.test.ts`
- `C:\Users\aTo\.openclaw\workspace\AGENTS.md`
- `C:\Users\aTo\.openclaw\workspace\SOUL.md`
- `C:\Users\aTo\.openclaw\workspace\IDENTITY.md`
- `C:\Users\aTo\.openclaw\workspace\TOOLS.md`
- `C:\Users\aTo\.openclaw\workspace\USER.md`
- `C:\Users\aTo\.openclaw\workspace\BOOTSTRAP.md`
- `C:\Users\aTo\.openclaw\agents\main\sessions\sessions.json`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\2.0.0龙虾养殖\2.0持续工作建立\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\2.0.0龙虾养殖\memory.md`

**验证结果**：
- 上游直连短答复：约 `1455ms`
- 修复前 `main`：约 `10151` prompt token，短答复常见 `20s~60s`
- 修复后 `main`：约 `5325` prompt token，短答复约 `9397ms`
- 修复后 `main` 的 `web_fetch` 工具调用通过
- Gateway 重启后重新监听 `127.0.0.1:18789`

**遗留问题 / 下一步**：
1. 飞书入口最好再做一条真实短消息验收，确认也享受到新 prompt 的收益。
2. `web_search` 仍缺搜索 provider key，联网搜索继续以 `browser` / `web_fetch` 为主。
3. 在主入口恢复后，再回到“阶段一：`main/Feishu -> pm -> worker` 半自动派单协议”的正线。

---

## 会话追加 - 2026-03-09 21:10

**用户本轮目标**：
- 复核“`main` 为什么又慢了”，判断这是不是正常现象。
- 基于最近真实日志，而不是主观体感，重新确认瓶颈位置。
- 在不改掉 `main = gpt-5.4-xhigh-fast` 的前提下，尽量做一次低风险收口。

**本轮完成事项**：
1. 复查近期 `main` transcript 与 Gateway 日志，确认最近几次代表性耗时：
   - `20:41` 消息 `1`：约 `34s`
   - `20:41` `你是谁？`：约 `7.5s`
   - `20:41` `你好？`：约 `3.7s`
   - `20:44` `你还有哪些同伴？`：约 `46.1s`
   - `20:49` 同类一句话问题：约 `25.2s`
   - `21:02` 同类一句话问题：约 `15.2s`
   - `21:05` 同类一句话问题：约 `35.7s`
2. 证明这不是队列堵塞：
   - 对应 run 在日志里都是立刻开始
   - `queueDepth` 维持 `0`
   - 真正耗时主要卡在 **首 token 出来之前**
3. 复查当前运行态，确认仍然是：
   - `provider = LZ`
   - `api = openai-completions`
   - `main = gpt-5.4-xhigh-fast`
4. 发现当前运行包 **不接受** `agents.list[].params`，虽然源码/文档已经支持；因此 live 配置必须退回到：
   - `agents.defaults.models["LZ/gpt-5.4-xhigh-fast"].params`
5. 对 `LZ/gpt-5.4-xhigh-fast` 落了一个保守上限：
   - `maxTokens = 1024`
   - 保留 `reasoningEffort = "medium"`
6. 做过一次 `main` 工具瘦身实验（临时把 `messaging` 改成更小工具集），结果：
   - prompt token 从约 `6312` 降到约 `4889`
   - 但单句响应并未稳定变快，甚至出现约 `35.7s` 的更慢样本
   - 结论：**工具 schema / prompt 体积不是当前主因**
7. 已将该工具瘦身实验回滚，恢复 `main` 的原工具入口，只保留真正有意义的 `maxTokens` 收口。
8. 额外确认：
   - 当前 Gateway 不是已安装服务模式
   - `openclaw gateway restart` 不能可靠热重启当前实例
   - 实际生效方式是 `gateway run --force`

**本轮关键结论**：
- 这次“又慢了”**不正常，但也不是本地网关坏了**。
- 真实根因更接近：
  1. `LZ / gpt-5.4-xhigh-fast / openai-completions` 这条路径本身存在明显的 **首 token 抖动**
  2. 抖动幅度已经大到会把一句话问题拉到 `15s ~ 46s`
  3. 本地 prompt 大小会有影响，但不是决定性主因
- 也就是说：**问题主要在当前上游模型链路的响应稳定性，不在 OpenClaw 的排队逻辑**。

**涉及文件或路径**：
- `C:\Users\aTo\.openclaw\openclaw.json`
- `C:\Users\aTo\.openclaw\agents\main\sessions\13b6ef5a-c015-494c-99e6-eea2ef81643d.jsonl`
- `D:\1_AAA_Program\OpenClaw\openclaw-gateway.stdout.log`
- `D:\1_AAA_Program\OpenClaw\openclaw-gateway.stderr.log`

**验证结果**：
- `queueDepth = 0`，说明不是队列阻塞
- `main` 保持 `gpt-5.4-xhigh-fast`，未换默认模型
- `maxTokens=1024` 后出现过 `25s -> 15s` 的改善样本，但仍有约 `35s` 慢样本
- 工具瘦身导致 prompt token 明显下降，但并没有稳定换来更快首 token

**遗留问题 / 下一步**：
1. 若坚持 `main` 必须继续用 `gpt-5.4-xhigh-fast`，那就要接受当前上游存在明显波动；我们只能继续做边际优化，无法保证恢复到昨晚那种“超快稳定”。
2. 如果要真正解决“入口体感慢”，最有效路线会是：
   - `main/pm` 用更稳更快模型做入口与派单
   - `reader/builder` 再继续挂高成本模型做深任务
3. `feishu` 当前还能工作，但 `gateway status` 仍持续提示单账号字段可被 doctor 迁移；这不是本轮主故障，但后续值得顺手清一次。

---

## 会话追加 - 2026-03-09 21:35

**用户本轮目标**：
- 继续深挖“上游到底哪里有配置问题”。
- 让我多做几轮真实压测，别只凭感觉判断。
- 给出“到底该用哪个 API / 哪个模型更合适”的明确建议。

**本轮完成事项**：
1. 核对 OpenClaw 当前可用配置入口与源码，确认对当前这条 `openai-completions` 链路：
   - 真正常见有效的模型 `params` 是 `maxTokens` / `temperature` / `transport` 等
   - 当前配置里的 `reasoningEffort` 放在 `agents.defaults.models[*].params` 中，**对这条链路基本不是一个可靠主开关**
2. 直接对上游 `https://synai996.space/v1/chat/completions` 做了多轮 **流式** 压测，测：
   - 首 chunk 时间
   - 首文本时间
   - 总耗时
   - 是否返回 `reasoning_content`
3. `chat/completions` 路线实测结论（ASCII 稳定提示词）：
   - `gpt-5.4-xhigh-fast`
     - 一次约 `4.9s`
     - 一次约 `17.8s`
     - 波动很大
   - `gpt-5.4-fast`
     - 一次约 `5.0s`
     - 一次约 `18.7s / 31.9s`
     - 更不稳
   - `gpt-5.4-high`
     - 约 `12s ~ 17s`
     - 两次都带 reasoning
   - `gpt-5.4-xhigh`
     - 约 `5.8s ~ 7.8s`
     - 当前样本里最稳
4. 额外测了更复杂提示，确认：
   - `xhigh-fast` 在某些提示下即使不显式要求 reasoning，也会收到 `reasoning_content`
   - 说明 **上游代理本身就有“不完全稳定/不完全一致”的行为**
5. 直接测了 `https://synai996.space/v1/responses`：
   - 确实可用
   - 单次可到约 `4.3s ~ 10.9s`
   - 但它把请求的 `gpt-5.4-xhigh-fast / gpt-5.4-xhigh / gpt-5.4-fast` 都统一回成了 **`gpt-5.4`**
   - 说明 `/responses` 这条路大概率会 **偷换/折叠模型档位**

**本轮关键结论**：
- 你现在这个上游没有一个“单纯配错了某一项”的问题，真正的问题是：
  1. `chat/completions` 会保留你选的具体模型，但 **延迟抖动大**
  2. `/responses` 体感有时更快，但 **不尊重你配置的细分模型名**
  3. 所以两条路是“精确模型控制”和“体感更快”之间的权衡，不是两全其美
- 以当前实测看：
  - **稳定优先**：`openai-completions + gpt-5.4-xhigh`
  - **想赌体感快但接受抖动**：`openai-completions + gpt-5.4-xhigh-fast`
  - **只想要快，不在乎具体 suffix 模型失真**：`openai-responses + gpt-5.4`
- 不建议继续把希望押在 `gpt-5.4-fast` 上；当前样本里它并不比 `xhigh` 更稳或更快。

**推荐配置决策**：
1. **主入口 `main` / `pm`**
   - 推荐：`LZ/gpt-5.4-xhigh`
   - 原因：当前样本里最稳，首文本与总耗时都比 `xhigh-fast` 更可控
2. **深任务 `reader` / `builder`**
   - 可以继续保留 `xhigh-fast` / `xhigh`
   - 因为深任务比入口聊天更能容忍波动
3. **API 路线**
   - 推荐继续用 `openai-completions`
   - 因为你明确在意具体模型档位，而 `/responses` 当前会把它们折叠成 `gpt-5.4`

**涉及文件或路径**：
- `C:\Users\aTo\.openclaw\openclaw.json`
- `src/agents/pi-embedded-runner/extra-params.ts`
- `docs/gateway/configuration-reference.md`
- `docs/concepts/model-providers.md`

**遗留问题 / 下一步**：
1. 如果用户接受“稳定优先”，下一步应把 `main` 从 `gpt-5.4-xhigh-fast` 切到 `gpt-5.4-xhigh`。
2. 如果用户坚持 `main` 必须是 `xhigh-fast`，那就继续保留现状，但要明确接受上游 jitter。
3. 后续可再把 `openclaw.json` 里的无效/误导性 `reasoningEffort` 配置清掉，减少认知噪音。

---

## 会话追加 - 2026-03-09 22:10

**用户本轮目标**：
- 当前窗口对话过长，希望生成一份“从最开始到现在”的完整交接文档。
- 交接文档要基于现有 `memory`，但不能只复制 `memory`，要重新梳理整条线程的演化、完成项、坑点、当前状态与后续优先级。

**本轮完成事项**：
1. 重新读取并复核当前线程与 Sunset 项目的规则来源：
   - `C:\Users\aTo\.codex\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\workspace-memory.md`
2. 复核线程沉淀材料：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\memory_0.md`
   - `tasks.md`
   - `progress.md`
   - `blockers.md`
   - 各阶段文档与操作手册
3. 复核当前 live config：
   - `C:\Users\aTo\.openclaw\openclaw.json`
   - 确认当前仍是 `LZ + openai-completions`
   - 确认当前 `main` 仍是 `gpt-5.4-xhigh-fast`
4. 复核仓库内已落地的 Windows 原子写修复：
   - `src/infra/json-files.ts`
   - `src/infra/json-files.test.ts`
5. 新建完整交接文档：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\2026-03-09_完整交接文档.md`

**本轮关键结论**：
- 当前线程已经不再需要“再补一份零碎摘要”，而是需要一份可直接交给后续执行者的结构化 handoff。
- 新交接文档已经把以下内容统一串起来：
  1. 需求如何从“先跑起来”演化为“持续工作系统”
  2. 飞书、浏览器、多 agent、慢响应、Windows 持久化修复各自的真实结论
  3. 当前 live 状态与未完成事项
  4. 接下来最合理的优先级与接手顺序
- 本轮没有再改运行配置，只做了高置信复核与文档沉淀，避免在交接阶段引入新的运行态波动。

**涉及文件或路径**：
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\2026-03-09_完整交接文档.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\tasks.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\progress.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\OpenClaw\部署与配置龙虾\blockers.md`
- `C:\Users\aTo\.openclaw\openclaw.json`
- `src/infra/json-files.ts`
- `src/infra/json-files.test.ts`

**验证结果**：
- 完整交接文档已创建成功。
- 当前 thread memory 已按 append-only 方式追加本轮记录。
- 当前未对 gateway / model / agent live 配置做新的变更，运行态风险为零变更。

**遗留问题 / 下一步**：
1. 若下一轮继续推进运行态问题，优先决定是否将 `main` / `pm` 切换到 `gpt-5.4-xhigh` 以换稳定性。
2. 若下一轮继续推进工作流，优先做 `main/Feishu -> pm` 的统一入口协议。
3. 若下一轮继续推进联网能力，优先决定是补搜索 provider key，还是正式接受 `browser + web_fetch` 作为当前联网基线。
