# Codex规则落地 - 线程记忆

## 线程概述
- 线程名称：Codex规则落地
- 线程分组：Sunset
- 线程定位：用于沉淀 Codex 在 Sunset 项目中的全局线程规则，而不是具体业务工作区的实现细节。
- 当前核心目标：建立一套适用于 Codex 全部线程的共通规则，尤其是线程级记忆维护规则，以及它与 Sunset 项目 steering 规则之间的分层关系。

## 当前状态
- **完成度**: 15%
- **最后更新**: 2026-03-08
- **状态**: 重新校准中

## 会话记录

### 会话 1 - 2026-03-08

**用户需求**:
> 纠正此前方向错误，明确：当前线程实际路径是 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地`。目标不是继续写 `.kiro/specs` 子工作区文档，而是为 Codex 的所有线程建立一套全局通用规则；其中最重要的是维护 Codex 自己的线程记忆，而不是把 `.kiro/specs` 的工作区记忆当成线程记忆。本线程还需要借鉴 `CLAUDE.md` 的映射和规范思路，重新输出对需求的理解。

**完成任务**:
1. 读取 `.codex\threads\Sunset\Codex规则落地` 当前目录，确认该线程目录目前为空。
2. 对照 `.codex\threads\OpenClaw\部署与配置龙虾\memory_0.md` 已有落盘样式，确认 Codex 线程确实存在独立线程级记忆的实际先例。
3. 重新校准理解：Codex 需要一层“线程级全局规则”，其中线程记忆是 Codex 自身跨回合连续性的主记忆；`.kiro/steering` 与 `.kiro/specs` 则是进入 Sunset 仓库任务后必须遵守的项目规则层。
4. 创建当前线程的首份 `memory_0.md`，用于承接后续线程级规则设计与讨论。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [新增]：当前线程第一份线程级记忆文档。

**解决方案**:
- 将 Codex 记忆体系拆成两层：
  - 线程级记忆：服务于 Codex 自己的线程连续性、规则沉淀、过程复盘；
  - 项目级工作区记忆：服务于 Sunset 项目工作区的业务推进与治理记录。
- 之后的全局规则设计必须围绕“这两层如何分工、何时同步、谁是主谁是辅”来展开，而不是再把它们混为一体。

**遗留问题**:
- [ ] 需要正式定义：什么情况下必须更新线程记忆，什么情况下还需要同步 `.kiro/specs` 工作区记忆。
- [ ] 需要正式定义：Codex 全局线程规则的落盘载体应是线程目录内专用规则文件、还是仓库级 `AGENTS.md` 映射文件。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 先建立线程级记忆，再继续谈全局规则 | 没有线程级记忆，后续所有“全局线程规则”讨论都无法沉淀 | 2026-03-08 |
| 将线程记忆与 `.kiro/specs` 工作区记忆明确分层 | 两者服务目标不同，不能再混成一套规则理解 | 2026-03-08 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `.codex/threads/OpenClaw/部署与配置龙虾/memory_0.md` | 已有线程记忆样例 |
| `.kiro/steering/workspace-memory.md` | Sunset 项目工作区记忆规范源 |
| `CLAUDE.md` | Claude 侧排序、映射与规范化参考 |

### 会话 2 - 2026-03-08

**用户需求**:
> 先搞清楚两点：第一，Codex 线程全局规则应该创建在哪里、怎么配置、四张截图里哪一个才是正确入口；第二，`CLAUDE.md` 对 Claude CLI 的地位，类比到 Codex 后，`AGENTS.md` 是否是每个对话都会一定遵守的最高宪法；要求先理解这些，再继续后续设计。

**完成任务**:
1. 读取本地 `C:\Users\aTo\.codex\config.toml`，确认“配置”页面对应的是全局 `config.toml`。
2. 检查 `$CODEX_HOME` 目录，确认 `C:\Users\aTo\.codex\AGENTS.md` 的确存在，但当前是空文件。
3. 读取 OpenAI 官方 Codex 文档，确认 Codex 会把 `$CODEX_HOME\AGENTS.md` / `$CODEX_HOME\AGENTS.override.md` 以及从项目根到当前目录链路上的 `AGENTS.md` / `AGENTS.override.md` 自动纳入会话指令上下文。
4. 结合本地界面截图，明确四张图的职责分层：
   - `工作树`：仅工作树管理，不是规则入口；
   - `环境`：环境级 `environment.toml` 与安装脚本，不适合作为所有线程通用行为规则主入口；
   - `配置`：全局 `config.toml`，适合技术配置；
   - `个性化`：全局自定义指令界面，适合短小的全局行为偏好。
5. 收敛结论：如果要做“文件化、长期、全局、所有线程通用”的 Codex 总规则，最合适的落点是 `$CODEX_HOME\AGENTS.md`；如果要做 Sunset 项目专属的 Codex 规则映射，最合适的落点是项目根 `AGENTS.md`。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮对 AGENTS / 配置 / 界面入口职责的结论。

**解决方案**:
- 将 Codex 规则载体分成三层：
  - 全局全部线程：`C:\Users\aTo\.codex\AGENTS.md`
  - 某一仓库/项目：项目根 `AGENTS.md`
  - 线程连续性：`.codex/threads/<分组>/<线程名>/memory_0.md`
- 界面四图里，真正最接近“全局行为指令”的是“个性化”；但如果追求像 `CLAUDE.md` 那样可版本化、可审查、自动参与会话的规则文件，则不该放在四图其中之一，而应直接使用 `$CODEX_HOME\AGENTS.md`。

**遗留问题**:
- [ ] 仍需后续决定：线程级记忆更新规则是否只写入 `$CODEX_HOME\AGENTS.md`，还是同时在项目根 `AGENTS.md` 中放一份 Sunset 专属补充映射。
- [ ] 仍需后续设计：项目根 `AGENTS.md` 与 `.kiro/steering` / `.kiro/specs` 的职责边界和最小映射结构。

### 会话 3 - 2026-03-08

**用户需求**:
> 不再继续偏向迁移分析文档本身，而是直接落地 Codex 的两层规则文件：一个约束所有线程的全局 `C:\Users\aTo\.codex\AGENTS.md`，一个约束 Sunset 仓库内工作的项目级 `D:\Unity\Unity_learning\Sunset\AGENTS.md`。同时要求真正消化 `.kiro/steering` 全部规则与 `900_开篇/spring-day1-implementation` 的落地方式，而不是机械借鉴 Claude。

**完成任务**:
1. 补读并串联 `sunset-workspace-router`、`spring-day1-implementation/memory.md`、`requirements.md`、`OUT_tasks.md`，确认 Sunset 的真实落地方式是“工作区路由 + steering 唯一规则源 + 追加式记录 + 需求拆分与执行闭环”。
2. 重写 `C:\Users\aTo\.codex\AGENTS.md`，将 Codex 全局规则收敛为线程级最小硬约束：线程命名确认、线程工作区约定、线程记忆强制更新、追加式记录、线程记忆与项目记忆分层。
3. 重写 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，把 Sunset 规则明确成项目级路由层：规定工作区判定、steering 读取顺序、三类技能使用入口、记忆更新顺序、风险任务处理和唯一规则源边界。
4. 明确最终分层：`C:\Users\aTo\.codex\AGENTS.md` 负责所有线程都通用的 Codex 纪律；`D:\Unity\Unity_learning\Sunset\AGENTS.md` 负责 Sunset 仓库专属映射；`.kiro/steering` 继续是项目正文规则源；`.codex/threads/.../memory_0.md` 继续承担线程连续性。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [重写]：落地 Codex 全局线程规则。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [重写]：落地 Sunset 项目级 Codex 工作规则。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮两层 AGENTS 正式落地结论。

**解决方案**:
- 不再把 Codex 迁移工作理解成“再造一套 Claude 风格大文档”，而是收敛为真正能被每个线程复用的全局最小规则内核。
- 将记忆更新纪律上提为 Codex 全局线程规则，把 Sunset 的复杂细则继续放回 `.kiro/steering` 与活跃工作区中，以避免全局规则臃肿和双源漂移。

**遗留问题**:
- [ ] 后续需要通过真实 Sunset 任务继续验证这两层 AGENTS 的稳定性与可执行性。
- [ ] 若未来还有其他仓库接入 Codex，需要再观察哪些规则应继续留在全局层，哪些应下沉到各自项目 `AGENTS.md`。

### 会话 6 - 2026-03-08
**用户需求**:
- 询问是否可以通过“在当前线程中重新读取 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 并据此更新规则”的方式，手动让当前线程对齐新的项目规则。

**完成任务**:
- 重新读取项目根 `AGENTS.md`，并在当前线程中按其规则口径重新对齐执行方式。
- 明确本轮结论：这种做法对当前线程是可行的，能作为手动对齐；但它本质上是本线程内的显式再对齐，不等于平台级“刷新初始系统注入”。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录当前线程可通过重读 `AGENTS.md` 完成手动对齐的结论。

**解决方案**:
- 将“重读项目 `AGENTS.md`”视为当前线程的手动规则同步动作，可立即影响后续本线程执行；若要保证新线程天然带上这份规则，仍以新开线程为准。

**遗留问题**:
- [ ] 若后续需要更强的一致性，仍应优先通过新开线程验证新规则是否按启动流程自动生效。

### 会话 7 - 2026-03-09
**用户需求**:
- 指出此前表述仍不够彻底：需要明确“所有线程的总记忆路径”在 `D:\Unity\Unity_learning\Sunset\.codex\threads\`，而不是 `C:`；要求重新厘清工作区并修正规则。

**完成任务**:
- 重新核对 `D:\Unity\Unity_learning\Sunset\.codex\threads\` 的实际目录结构，确认当前项目线程记忆总根路径确实位于该目录下，而非 `C:\Users\aTo\.codex\`。
- 修正 `C:\Users\aTo\.codex\AGENTS.md`：明确 `C:` 只是全局规则文件位置，不是线程记忆存储根；线程记忆应优先落在当前项目根目录下的 `.codex/threads/`。
- 修正 `D:\Unity\Unity_learning\Sunset\AGENTS.md`：明确 Sunset 项目全部 Codex 线程的记忆总根路径固定为 `D:\Unity\Unity_learning\Sunset\.codex\threads\`，并把线程记忆路径写成绝对口径。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：区分全局规则文件路径与线程记忆根路径。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：明确 Sunset 全部线程记忆总根路径。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮路径纠偏结论。

**解决方案**:
- 将路径概念分成两层：
  - 全局规则文件：`C:\Users\aTo\.codex\AGENTS.md`
  - Sunset 项目线程记忆总根：`D:\Unity\Unity_learning\Sunset\.codex\threads\`
- 以后凡是谈 Sunset 项目的“所有线程记忆总路径”，一律指向后者，不再混淆为 `C:`。

**遗留问题**:
- [ ] 后续若还有其他项目接入 Codex，需要继续沿用“规则文件路径”和“项目线程记忆根路径”分离的口径。

### 会话 8 - 2026-03-09
**用户需求**:
- 指出当前规则虽然已经覆盖了记忆和路径，但仍没有解决更核心的“回复漂移”问题：当中途去修工具、修环境或处理阻塞时，Codex 容易只响应最后一句输入，脱离原始主线目标。要求重新思考并给出完善方案与理解。

**完成任务**:
- 重新归因问题根源：此前规则更偏向“路径、记忆、路由”治理，但缺少“主线目标锚定 / 子任务恢复 / 防漂移”约束，所以一旦进入修复动作，容易把支撑子任务误当成新的主任务。
- 修正 `C:\Users\aTo\.codex\AGENTS.md`，新增“主线目标锚定与防漂移”规则：要求先判断当前输入是继续主线、支撑子任务、明确换线还是纯澄清；默认不因输入过短或修复动作丢失主线；回复子任务结果时必须同时说明主线恢复情况。
- 修正 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，新增“Sunset 任务连续性与主线恢复”规则：把修工具、修 MCP、修规则、查报错等统一定义为阻塞处理；阻塞结束后必须回到原工作区、原主线、原完成标准。
- 同步强化记忆书写口径：线程记忆与工作区记忆的新增条目都必须写明当前主线目标、本轮阻塞或子任务、恢复点或下一步主线动作。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：新增全局“主线目标锚定与防漂移”规则。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：新增 Sunset 项目“任务连续性与主线恢复”规则。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮防漂移规则补丁。

**解决方案**:
- 将规则重心从“只要求记下来”升级为“先别把主线丢了”：
  - 默认维持主线，除非用户明确换线；
  - 修复动作一律视为服务主线的支撑子任务；
  - 子任务结束时必须显式恢复主线并汇报主线状态；
  - 记忆里必须写清主线、阻塞、恢复点。
- 这样以后即使中途插入修工具、修搜索、修网络、修脚本，也不会再把原始目标吞掉。

**遗留问题**:
- [ ] 后续需要在真实多段任务里继续观察，这套防漂移规则是否还需要进一步压实成固定回复模板。

### 会话 9 - 2026-03-09
**用户需求**:
- 认可“防漂移”方向，并同意继续补强为一层更轻量的“回复前自检四问”。

**完成任务**:
- 在全局 `C:\Users\aTo\.codex\AGENTS.md` 的“主线目标锚定与防漂移”部分，新增“每次准备回复前快速自检四问”，用于在收尾前检查是否丢失主线、是否误把子任务当主任务、是否交代了恢复点。
- 在项目级 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 的“Sunset 任务连续性与主线恢复”部分，同步新增面向工作区的“回复前自检四问”，用于在 Sunset 项目内检查活跃工作区、主线进度、阻塞处理状态与下一步主线动作。
- 保持规则轻量化：只补一个可执行的四问，不额外扩张新的模板层级。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：新增全局回复前自检四问。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：新增 Sunset 项目回复前自检四问。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮“四问”落地。

**解决方案**:
- 将“主线锚定”从原则继续压实成临门一脚的检查动作：在真正发出回复前，再过一遍四问，减少因上下文长、修复动作多、最后一句输入很短而产生的回复漂移。

**遗留问题**:
- [ ] 后续可观察是否还需要把四问进一步收敛成极短的一行内部口令，但当前先保持可读、可执行即可。

### 会话 10 - 2026-03-09
**用户需求**:
- 认可继续补一层“换线判定词”规则，用来减少我对用户意图的误判。

**完成任务**:
- 在全局 `C:\Users\aTo\.codex\AGENTS.md` 中补入“明确换线表达”和“默认不算换线的插入式表达”示例，帮助区分主线切换与中途阻塞处理。
- 在项目级 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 中同步补入同类规则，并把插入式提问明确归类为当前工作区的阻塞处理或子任务。
- 进一步压实当前治理口径：只有用户明确说停掉原任务、另起任务、重新开始时，才真正重置主线；像“顺便问一句”“先修一下这个”默认仍从属于原主线。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：新增换线判定词规则。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：新增 Sunset 项目换线判定词规则。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮换线判定词补丁。

**解决方案**:
- 用一组很短、很明确的判定词，把“真的换线”和“只是插入一个支撑动作”分开，减少我后续对你意图的误判。

**遗留问题**:
- [ ] 后续可继续观察是否还需要补“优先级高但不换线”的表达样例，但当前规则已经足够实用。

### 会话 11 - 2026-03-09
**用户需求**:
- 指出当前记忆与规则中仍混有英文叙述，要求统一调整为：除专有名词与特殊情况外，语言描述一律使用中文。

**完成任务**:
- 修订 `C:\Users\aTo\.codex\AGENTS.md`，明确：除专有名词、文件名、路径、命令、代码标识、Unity 专有类型名等特殊情况外，所有说明文字一律使用中文。
- 修订 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，将通用叙述中的英文口径进一步改为中文，如“唯一正文规则源”“唯一有效依据”“工作区”“记忆”“评审”“追加式记录”等。
- 顺手修正当前线程记忆中最近几段由我写入的英文叙述，把 `prompt`、`workspace`、`memory` 等普通说明词替换为中文表达。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：新增“说明文字一律中文”的全局规则，并清理剩余英文叙述。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：新增并落实“说明文字一律中文”的项目规则。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [修正+追加]：修正最近几段英文叙述，并记录本轮语言统一结论。

**解决方案**:
- 以后默认口径是：路径、文件名、命令、代码标识、专有名词保留原文；其余说明性叙述一律中文。
- 这样既保留技术准确性，也保证记忆、规则、总结的阅读体验统一。

**遗留问题**:
- [ ] 后续若再发现旧记忆中有新增英文叙述，继续按同一口径逐步修正，不必一次性重写全部历史文件。

### 会话 12 - 2026-03-09
**用户需求**:
- 说明你已经手动汉化了一部分，要求我继续检查当前治理线相关文件里还有哪些普通英文叙述未汉化，并开始修正。

**完成任务**:
- 重新扫描并清理了当前治理线最活跃的几份文件：
  - `C:\Users\aTo\.codex\AGENTS.md`
  - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
  - `.codex/threads/Sunset/Codex规则落地/memory_0.md`
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/memory.md`
  - `.kiro/specs/Steering规则区优化/memory.md`
- 已把普通说明性英文叙述继续改为中文，重点包括：`UI`、`workspace`、`memory`、`append-only`、`canonical rule source`、`prompt`、`skill`、`SOP`、`tool`、`plan` 等在叙述语境中的用法。
- 当前剩余的英文内容已基本收敛为可接受例外：文件名、路径、命令、代码标识、技能名、产品名、专有技术名词，以及诸如 `memory.md`、`AGENTS.md`、`workspace-memory.md`、`code-reaper-review.md`、`SKILL.md` 这类文件名本体。

**修改文件**:
- `C:\Users\aTo\.codex\AGENTS.md` - [修正]：继续清理普通英文说明词。
- `D:\Unity\Unity_learning\Sunset\AGENTS.md` - [修正]：继续清理普通英文说明词。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [修正+追加]：继续汉化当前线程历史记录中的普通英文叙述。
- `.kiro/specs/Steering规则区优化/Codex迁移与规划/memory.md` - [修正+追加]：继续汉化子工作区记忆中的普通英文叙述。
- `.kiro/specs/Steering规则区优化/memory.md` - [修正+追加]：继续汉化父治理记忆中的普通英文叙述。

**解决方案**:
- 采用“只改普通叙述，不动专有名词和文件名”的方式收口，既统一语言风格，也避免误伤路径、文件名和技术标识。
- 这一轮之后，当前治理线中还能检出的英文，大多属于保留项而非漏汉化项。

**遗留问题**:
- [ ] 若后续你还想继续做更彻底的历史清洗，可以再针对更早期的治理记录做一轮“旧档案中文化”，但那将属于历史整理而不是当前规则落地的必要项。

### 会话 13 - 2026-03-10
**用户需求**:
- 在继续推进 `Unity MCP` 迁移相关主线时，担心我误把 MCP 接入动作做到了不该动的地方；要求先解释当前在做什么，并改为“由用户手动配置，我只提供链接与步骤”。

**完成任务**:
- 说明当前尚未做实际接入，之前停留在“读活跃工作区、核对旧配置、准备确认切换写法”的阶段。
- 将本轮子任务明确归类为主线下的阻塞处理：目标不是切走主线，而是避免因自动改配置造成新的混乱。
- 收束本轮策略为“我提供手动接入路径，用户自行安装启动；后续再回到主线继续验证闭环”。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮关于 MCP 自动接入边界与手动接入恢复点的结论。

**解决方案**:
- 继续落实“阻塞处理不等于换线”的线程规则：修工具与改接入方式仍然服务于原主线，收尾时必须写清主线未变、阻塞如何解除、下一步回到哪里。

**遗留问题**:
- [ ] 待用户手动完成 `CoplayDev/unity-mcp` 安装与启动后，继续验证 Codex 侧接入与 `sunset-unity-validation-loop` 的稳定性。

### 会话 14 - 2026-03-10
**用户需求**:
- 贴出 Unity 侧 `MCP Setup` 截图，担心自己是不是已经成功启动服务；要求我判断这是不是我真正需要的界面。

**完成任务**:
- 明确说明当前尚未成功 `Start Server`，截图显示的是 `CoplayDev/unity-mcp` 的依赖检测界面。
- 在本机验证到：`python` 当前为 `3.9.9`，系统没有 `uv`，因此该界面提示与本机事实一致。
- 将本轮阻塞归类为“基础设施缺依赖”，继续服务于原主线，而不是切换成新的项目问题。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮 Unity Setup 判断与阻塞归因。

**解决方案**:
- 当前最小恢复路径是：先安装 `Python 3.10+` 与 `uv`，再回到 Unity 面板 `Refresh` 并启动服务，之后继续主线验证。

**遗留问题**:
- [ ] 依赖安装完成后，还需继续确认 `http://localhost:8080/mcp` 是否已真正启动，并完成 Codex 侧接线。

### 会话 15 - 2026-03-10
**用户需求**:
- 反馈桌面持续出现“终端窗口一闪而过”，直接追问这是否是我这边留下的问题。

**完成任务**:
- 先复核 `C:\Users\aTo\.codex\config.toml`，确认 `mcp_servers.playwright` 仍处于禁用状态，因此当前闪窗已不是此前那条 `npx @playwright/mcp@latest` 问题。
- 对当前控制台相关进程链做现场采样，抓到新的真实来源：`D:\1_AAA_Program\OpenClaw\openclaw.mjs gateway run --verbose` 会周期性拉起 `cmd.exe /c "arp -a | findstr /C:\"---\""`，这正是本轮闪窗来源。
- 继续核对 `OpenClaw` 本地代码，确认 `OPENCLAW_DISABLE_BONJOUR=1` 可关闭 Bonjour 广播路径；随后将网关重启为“隐藏启动 + 禁用 Bonjour”模式。
- 重启后验证通过：新网关 PID 为 `8088`，`127.0.0.1:18789` 正常监听，连续 20+ 秒未再抓到 `openclaw -> cmd.exe/conhost.exe` 新子进程。

**修改文件**:
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录本轮闪窗阻塞的真实来源、止血动作与主线恢复点。

**涉及路径 / 运行对象**:
- `C:\Users\aTo\.codex\config.toml`
- `D:\1_AAA_Program\OpenClaw\openclaw.mjs`
- `D:\1_AAA_Program\OpenClaw\src\infra\bonjour.ts`
- `D:\1_AAA_Program\OpenClaw\src\gateway\server-discovery-runtime.ts`
- `D:\1_AAA_Program\OpenClaw\openclaw-gateway.stdout.log`

**解决方案**:
- 当前主线没有变化，仍然是 Unity MCP 闭环；本轮只是清理一个会持续打断操作体验的工具阻塞。
- 已形成稳定结论：此前 `playwright` 的弹窗问题已关闭；这次新的闪窗来自 `OpenClaw gateway` 的 Bonjour/局域网探测分支。
- 当前恢复点已经明确：桌面闪窗阻塞已先止血，下一步回到 `CoplayDev/unity-mcp` 的 `localhost:8080/mcp` 启动与验证链路。

**遗留问题**:
- [ ] 若用户仍继续看到终端闪窗，需要继续排查是否存在第三来源。
- [ ] 阻塞解除后，继续回到 Unity MCP 主线，完成 `8080` 启动确认与首轮验证闭环。

### 会话 16 - 2026-03-10（闪窗报告沉淀）
**用户需求**:
- 在确认桌面已无弹窗后，希望我输出一份可转交给其他会话学习的处理报告。

**完成任务**:
- 新增 `.kiro/specs/Steering规则区优化/Codex迁移与规划/OpenClaw终端闪窗排查与止血报告_2026-03-10.md`，完整沉淀本轮闪窗问题的根因、排查过程、止血动作、验证结果与主线恢复点。
- 将用户最新确认“现在已经没有任何弹窗情况出现了”写入记录，明确本轮问题已在现场闭环。

**修改文件**:
- `.kiro/specs/Steering规则区优化/Codex迁移与规划/OpenClaw终端闪窗排查与止血报告_2026-03-10.md` - [新增]：供其他会话直接阅读复用。
- `.codex/threads/Sunset/Codex规则落地/memory_0.md` - [追加]：记录报告产出与主线恢复点。

**解决方案**:
- 把本轮从“现场止血”升级为“文档化可复用结论”，降低后续会话重复排查成本。
- 当前主线恢复点保持不变：继续回到 Unity MCP 的 `8080` 启动确认与验证闭环。

**遗留问题**:
- [ ] 仍需在后续会话中继续推进 `CoplayDev/unity-mcp` 的真实启动与验证。
