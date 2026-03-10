# 2.0持续工作建立 - 开发记忆

## 模块概览

用于记录 OpenClaw「持续工作底座」的真实落地过程，重点包括：
- 主入口可用性与响应速度
- 飞书 / `main` 双入口协同
- 多 agent 工作流的可运行基础
- 线程记忆、任务池、进度与阻塞的外部化

## 当前状态

- **完成度**：45%
- **最后更新**：2026-03-09
- **状态**：基础设施已可用，当前重点转为稳定主入口体验

## 会话记录

### 会话 1 - 2026-03-09

**用户需求**：
> 不忘初心，重新理清我们要的最终结果是什么；把 `main` / 飞书恢复成真正可用、响应更快的总入口；保留多 agent 与浏览器能力，但不要让主入口继续被历史包袱拖慢。

**完成任务**：
1. 修复 OpenClaw 在 Windows 下的原子写入问题，避免 `paired.json` / 会话索引写入异常继续污染运行态。
2. 核对运行中 Gateway 实际生效配置，确认 `LZ` 已固定走 `openai-completions`，并且 `main` 仍是 `gpt-5.4-xhigh-fast`。
3. 对比直连上游与 OpenClaw 运行态耗时，确认真正的慢点已不再是上游模型，而是主入口 prompt 与旧会话负担。
4. 精简 `C:\Users\aTo\.openclaw\workspace` 下的主入口提示文件，删除已无必要的 `BOOTSTRAP.md`，压缩 `AGENTS.md`、`SOUL.md`、`TOOLS.md`、`IDENTITY.md`、`USER.md`。
5. 备份旧主会话后，对 `main` 做软重置：保留 transcript 备份，只移除当前 `agent:main:main` 索引，让 Gateway 在重启后创建新主会话。
6. 温和重启 Gateway 并复测，确认新主会话已启用、更轻的系统提示已生效。

**修改文件**：
- `src/infra/json-files.ts` - 增加 Windows 下原子写失败时的覆盖回退逻辑。
- `src/infra/json-files.test.ts` - 补充对应回归测试。
- `C:\Users\aTo\.openclaw\workspace\AGENTS.md` - 压缩为轻量主入口规则。
- `C:\Users\aTo\.openclaw\workspace\SOUL.md` - 压缩为最小行为准则。
- `C:\Users\aTo\.openclaw\workspace\IDENTITY.md` - 重写为简明角色说明。
- `C:\Users\aTo\.openclaw\workspace\TOOLS.md` - 只保留当前有价值的本地事实。
- `C:\Users\aTo\.openclaw\workspace\USER.md` - 压缩为用户偏好与当前目标。
- `C:\Users\aTo\.openclaw\workspace\BOOTSTRAP.md` - 删除，避免继续注入无效启动提示。
- `C:\Users\aTo\.openclaw\agents\main\sessions\sessions.json` - 移除 `agent:main:main` 旧索引，触发新主会话建立。

**验证结果**：
- 直连上游 `https://synai996.space/v1/chat/completions`，短消息响应约 **1455ms**。
- 修复前，`main` 旧会话短答复常见在 **25s~60s**，prompt token 约 **10151**。
- 修复后，`main` 新会话短答复降至约 **9397ms**，prompt token 降至 **5325**。
- 修复后，`main` 调用 `web_fetch` 获取 `https://example.com` 并返回“成功”通过。
- Gateway 重启后恢复监听 `127.0.0.1:18789` / `[::1]:18789`。

**关键判断**：
- 当前主要矛盾已经从“模型链不通”转为“主入口 prompt 与会话负担过重”。
- 本轮最有效的修复不是换模型，而是 **瘦身主工作区提示 + 软重置主会话**。
- 多 agent、飞书、浏览器能力都应继续保留，但必须服从“主入口先轻快可用”这个目标。

**遗留问题**：
- [ ] 飞书入口还没有做同级别的“新会话 + 轻 prompt”实测，只能高置信推断会同步受益。
- [ ] `web_search` 仍缺真实搜索 provider key，当前可靠联网链路仍是 `browser` / `web_fetch`。
- [ ] `main / Feishu -> pm -> reader/reviewer/builder` 的半自动派单协议仍待进入下一阶段落地。

## 相关文件

- `src/infra/json-files.ts`
- `src/infra/json-files.test.ts`
- `C:\Users\aTo\.openclaw\workspace\AGENTS.md`
- `C:\Users\aTo\.openclaw\workspace\SOUL.md`
- `C:\Users\aTo\.openclaw\workspace\IDENTITY.md`
- `C:\Users\aTo\.openclaw\workspace\TOOLS.md`
- `C:\Users\aTo\.openclaw\workspace\USER.md`
- `C:\Users\aTo\.openclaw\agents\main\sessions\sessions.json`

### 会话 2 - 2026-03-10（持续工作目标复盘与纠偏）

**用户需求**:
- 要求重新回读本工作区最初文档，反思为什么后续执行越做越偏，并给出最终检讨与后续规划。

**完成任务**:
1. 重新阅读 `豆包001.md`、`豆包_回应001.md` 与当前 `memory.md`，恢复本工作区最初目标。
2. 明确识别本工作区的核心不是“多跑几条链路”，而是建立一套可持续工作的外部操作系统：任务池、外部记忆、标准循环、验证闭环、回滚机制、人工升级协议。
3. 对照当前 OpenClaw 侧推进结果，确认偏差在于过度关注局部链路跑通与临时排障，没有优先沉淀成你可直接验收的持续工作系统交付物。
4. 重新锚定后续方向：主交付必须转为操作手册、任务池、进度记录、阻塞登记、验收指南与最小可重复演示闭环。

**关键结论**:
- 我前面的工作有推进，但交付形态错了。
- 我把支撑子任务做重了，把主线“持续工作系统建设”做轻了。
- 真正要落地的不是“模型一直干活”，而是“AI 在规则、任务池、记忆、验证、回滚里持续工作”。

**后续恢复点**:
- 以后继续这条线时，优先补齐：`龙虾操作手册.md`、`tasks.md`、`progress.md`、`blockers.md`、验收指南，以及最小可复现运行闭环。

### 会话 3 - 2026-03-10（工具骨架补齐：最小闭环与任务样例）

**用户需求**：
- 用户重新强调主线必须回到 `Sunset`，要求先把龙虾做成一个专业、可接任务、可验收的工具，而不是继续把注意力放在链路展示或跨项目漂移上。

**完成任务**：
1. 重新对齐 `Sunset` 根规则、当前工作区 `memory.md`、`豆包001.md`、`豆包_回应001.md` 与已补的三份基础文档，确认本阶段优先交付的是工具骨架而不是正式账本。
2. 新增 `龙虾最小运行闭环.md`，把龙虾在 `Sunset` 中形成一次合格任务闭环所需的步骤、边界、工作模式、可承接任务与当前非目标写清楚。
3. 新增 `龙虾当前可承接任务与分工样例.md`，把 `直接 PM`、`main -> pm`、`pm -> reader/reviewer/builder`、飞书入口分派这几种当前阶段可试跑的方式落成了可直接验收的任务样例。
4. 明确当前阶段的真实口径：重点不是先证明多 agent 可以拉多长，而是先证明龙虾能把一个 `Sunset` 真实任务稳稳做完，并把结果、验证、记忆与恢复点都交代清楚。

**修改文件**：
- `龙虾最小运行闭环.md` - 新增当前阶段最小可用闭环定义，明确 8 步标准流程、3 种工作模式、可承接任务与合格标准。
- `龙虾当前可承接任务与分工样例.md` - 新增当前可直接试跑的任务样例与分工方式，提供 `直接 PM`、`main -> pm`、`pm -> reader/reviewer/builder`、飞书入口的现实测试口径。

**验证结果**：
- 已逐份回读两份新文档，确认内容没有重新漂回“无限运行”或“链路演示”，而是围绕 `Sunset` 主线、任务闭环、分工方式、验收口径展开。
- 两份文档都给出了用户可直接拿来试的输入样例、预期产物与通过条件，已经具备当前阶段的可验收属性。

**关键结论**：
- 龙虾当前阶段已经不再只是“会聊天”或“会跑局部链路”的形态，而是开始形成一个服务于 `Sunset` 的专业任务工具骨架。
- `pm / reader / reviewer / builder` 当前首先是专业分工方法，不强制等于已经建成长期常驻的多 agent 集群；本阶段验收重点是闭环是否成立，而不是编队形式是否复杂。
- 正式的 `tasks.md / progress.md / blockers.md` 三件套，应该建立在这些骨架文档和最小闭环已经明确之后，再进入账本化阶段。

**后续恢复点**：
- 下一轮优先不再泛泛讨论，而是基于当前文档直接挑选第一批 `Sunset` 真实低风险任务做试跑验收。
- 试跑通过后，再决定是否正式引入 `tasks.md / progress.md / blockers.md`，把当前阶段转入账本推动。
