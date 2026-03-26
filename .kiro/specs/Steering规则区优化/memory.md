# Steering规则区优化 - 开发记忆（分卷）

> 本卷起始于 `memory_3.md` 之后。历史详细记录请继续查阅 `memory_1.md`、`memory_2.md`、`memory_3.md`。

## 模块概述
- 本工作区负责 Sunset 治理总线：规则收敛、迁移期收口、现行活文档入口治理、Git / 锁 / 线程协作口径与阶段归档。
- 当前它不再承担“迁移期当前入口”的身份；真正的现行入口已经下沉到 `当前运行基线与开发规则`，治理正文续办则下沉到 `Codex规则落地`，`000_代办/codex` 只保留 TD 镜像身份。

## 当前状态
- **完成度**: 100%
- **最后更新**: 2026-03-17
- **状态**: 结构归位完成，常态治理中

## 分卷索引
- `memory_1.md`：早期治理主卷，覆盖 2026-01-08 ~ 2026-03-02 的规则优化与历史整理。
- `memory_2.md`：2026-03-06 续卷，覆盖 Claude 迁移与 hook / append-only 治理闭环。
- `memory_3.md`：2026-03-07 ~ 2026-03-16 的完整长卷，覆盖 Codex 迁移、AGENTS 双层落地、L5 收口、活文档重组、治理 skills 四件套与旧首版退役。

## 承接摘要

### 最近归档长卷的稳定结论
- Sunset 的迁移期已经收口完成，`Codex迁移与规划` 当前只保留历史路由身份。
- 当前有效治理入口已经分层：
  - 现行入口：`当前运行基线与开发规则`
  - 治理续办正文：`Codex规则落地`
  - TD 镜像：`000_代办/codex`
  - 历史归档：`文档归档\2026-03-Codex恢复与迁移收口`
- 当前治理技能基础四件套已经落地：
  - 线程唤醒
  - 锁裁决
  - 编码审计
  - 阶段快照
- 当前 live 规则已经再补一层：
  - Unity / MCP 单实例占用、热区与冲突日志
- `03-13` 状态正文与 `03-15` 首版共享表已退出活目录，不再冒充当前入口。

### 当前恢复点
- 父治理工作区后续重点不再是重复写收口叙事，而是维护治理结构稳定：
  - 记忆分卷治理
  - 规则入口轻量化
  - 编码健康持续巡检
  - 新代办体系的持续验证
- 当前若需继续推进治理，优先从 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地` 进入；`000_代办/codex` 只用来读 TD，不再作为治理正文入口。

## 会话记录

### 会话 1 - 2026-03-16（父治理工作区 memory 分卷重建）

**用户需求**:
> 继续做，不要停；在不影响当前业务线程的前提下，把你已经识别出的治理续办继续往前推进。

**完成任务**:
1. 将父治理工作区旧长卷 `memory.md` 完整归档为 `memory_3.md`。
2. 新建当前精简主卷，补齐三卷历史索引、承接摘要与恢复点。
3. 将父治理工作区的职责重新收紧为“治理总线与稳定结构维护”，避免继续把所有长过程叠回主卷。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory_3.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`

**关键决策**:
- 父治理工作区保留完整历史卷，但活跃卷必须恢复为高密度摘要入口。
- 现行入口维护与治理续办已各自有固定承接点，父治理工作区不再回到“什么都往里塞”的状态。

**验证结果**:
- 历史卷链条现在已明确为 `memory_1.md`、`memory_2.md`、`memory_3.md`。
- 新主卷已具备后续继续追加所需的索引、摘要与恢复点。

**遗留问题**:
- [ ] 仍需完成线程记忆分卷。
- [ ] 仍需把本轮分卷约定补入现行规则文档与代办工作区记忆。

### 会话 2 - 2026-03-16（父治理工作区分卷治理已收口）

**完成任务**:
1. 父治理工作区分卷完成后，继续将分卷约定补入现行规则与项目 `AGENTS.md`。
2. 同步将本轮分卷治理登记到 `000_代办/codex/06_memory分卷治理与索引收紧/` 与 `000_代办/codex/memory.md`。

**验证结果**:
- 父治理工作区当前活跃卷 `memory.md` 为 `53` 行。
- 历史卷链条已明确为 `memory_1.md`、`memory_2.md`、`memory_3.md`。

**恢复点**:
- 父治理工作区接下来继续承担“治理总线与结构稳定”角色，不再回到长过程堆积模式。
- 后续重点转向编码健康巡检与新代办体系持续验证。

### 会话 3 - 2026-03-16（项目文档总览线程已按现行入口降级为历史总览候选）

**完成任务**:
1. 按 `当前运行基线与开发规则` 的现行入口重新核对 `项目文档总览` 线程的真实位置与职责。
2. 确认 `2026.03.16冻结文档汇总` 当前应留在 `文档归档\2026-03-Codex恢复与迁移收口\06_L5解冻与阶段快照_2026-03-16\`，不再属于活入口。
3. 将 `项目文档总览` 线程冻结快照改写为“归档对齐版”，并收紧父层口径：这条线程默认不再作为持续活跃治理线程，而是历史总览线程 / 按需维护线程。

**修改文件**:
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\06_L5解冻与阶段快照_2026-03-16\2026.03.16冻结文档汇总\项目文档总览.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\文档归档\2026-03-Codex恢复与迁移收口\06_L5解冻与阶段快照_2026-03-16\2026.03.16冻结文档汇总\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\memory_0.md`

**关键结论**:
- `项目文档总览` 的冻结快照应固化，但应作为归档资料固化。
- 这条线程当前不再承担现行入口身份；如未来需要重新激活，应先明确是做“总索引层补建”，还是只做 about 正文增量维护。

### 会话 4 - 2026-03-16（父治理层吸收 07 阶段的结构性纠偏）

**完成任务**:
1. 将 07 阶段“编码分类 + Git 边界 + 总索引独立化”结论吸收到父治理层认知中。
2. 确认当前活文档层已经具备：
   - `Sunset当前唯一状态说明_2026-03-16.md`
   - `Sunset Git系统现行规则与场景示例_2026-03-16.md`
   - `Sunset现行入口总索引_2026-03-16.md`
   - `Sunset L5开工清单与治理续办总览_2026-03-16.md`
3. 明确父治理层当前最重要的结构稳定动作：
   - 不再让旧 `03-13` 命名文件继续承担现行入口身份
   - 不再把终端假乱码误判成活文档整体损坏
   - 不再把 `governance` 误解为会自动兜住全部治理文档树

**关键结论**:
- Sunset 治理总线当前已经从“迁移期入口混乱”进一步收紧到了“当前入口、历史路由、治理续办、编码分类、脚本边界”五线分明。
- `Skills和MCP` 已归档退出活跃排期，相关 skills / MCP 治理收口由 `Codex规则落地` 统一承接。

**恢复点**:
- 父治理工作区后续继续只记录结构性稳定结论，不再重复扩写细节过程。

### 会话 5 - 2026-03-16（父治理层确认共享层 branch 快照已重新对齐）

**完成任务**:
1. 用 `preflight` 实际输出校准了共享层的 branch / HEAD 快照。
2. 确认当前物理 worktree 已落在 `NPC` 线，而不是旧的 farm 分支。
3. 将这一事实回写到活跃线程总表与 dirty / WIP 归属表，避免共享层继续传播过时快照。

**关键结论**:
- 父治理层当前不只是在维护“默认基线 = main”，还要持续区分“默认基线”和“当前物理 worktree 落点”这两件事。

**恢复点**:
- 父治理层后续继续只保留结构性结论；具体验证证据可回查活文档层和阶段工作区。

### 会话 6 - 2026-03-16（第二批超长主卷治理已正式启动）

**完成任务**:
1. 在治理续办总代办下新建 `08_第二批超长活跃memory主卷治理/` 阶段。
2. 完成首个安全对象：
   - `SO设计系统与工具/memory.md` 从 `861` 行长卷拆为 `memory_0.md + 新 memory.md`

**关键结论**:
- 父治理层当前不仅要稳住现行入口，还要持续处理那些仍会拖慢接手成本的老工作区超长主卷。
- 第二批治理继续遵守“不碰活跃业务线程 owner 工作区”的边界。

**恢复点**:
- 后续可继续从 `UI系统` 或 `Steering规则区优化/2026.02.11智能加载` 中挑选下一个安全候选。

### 会话 7 - 2026-03-16（08 阶段第二刀完成）

**完成任务**:
1. 第二批超长主卷治理再完成一个安全对象：
   - `UI系统/0_背包交互系统升级`
2. 该工作区当前也已从 820 行长卷切换为 `memory_0.md + 新 memory.md`。

**关键结论**:
- 父治理层当前可以确认：第二批主卷治理方法已经不是试点，而是可连续复用的成熟动作。

**恢复点**:
- 后续若继续推进 08 阶段，应先复核 `UI系统/1_背包V4飞升`、`UI系统/memory.md`、`Steering规则区优化/2026.02.11智能加载` 的活跃性。

### 会话 8 - 2026-03-16（08 阶段第三刀完成）

**完成任务**:
1. 继续完成 08 阶段第三刀：
   - `UI系统/1_背包V4飞升/memory.md`
2. 该工作区也已从 `811` 行长卷切换为 `memory_0.md + 新 memory.md`。

**关键结论**:
- 父治理层现在已经拥有 3 个连续样本，足以证明“第二批超长主卷治理”不是偶发动作，而是可稳定推广的治理流程。

**恢复点**:
- 后续若继续 08 阶段，只需在 `Steering规则区优化/2026.02.11智能加载` 与 `UI系统/memory.md` 之间决定下一刀即可。

### 会话 9 - 2026-03-16（08 阶段候选表已清空）

**完成任务**:
1. 继续完成 08 阶段第四刀与第五刀：
   - `Steering规则区优化/2026.02.11智能加载`
   - `UI系统`
2. 这两份工作区也已从超长主卷切换为 `memory_0.md + 新 memory.md`。

**关键结论**:
- 父治理层当前已经完成一整批 5 个安全对象的超长主卷治理。
- 08 阶段若继续扩写，不应再沿用旧候选表，而应重新盘点新的高风险对象。

**恢复点**:
- 父治理层当前可以把 08 阶段视为阶段性完成，并把注意力转回其他治理债。

### 会话 10 - 2026-03-17（只读确认现行入口、skills 现状与真实 Git 现场）

**用户目标**:
- 不要求继续修改规则，而是要求我先把“当前 skills 和规则体系的真实理解”讲清楚。

**完成任务**:
1. 只读复核了 `03-16` 现行入口组：
   - 当前状态说明
   - 当前 Git 规则入口
   - 当前总索引
   - 当前四件套与代办规范
2. 只读复核了 Sunset 项目 `AGENTS.md` 的当前路由口径。
3. 只读复核当前已安装治理型 skills 清单与 `02_skills_AGENTS_与执行机制重构/tasks.md`。
4. 只读确认当前物理 worktree 仍是：
   - `codex/npc-main-recover-001`
   - 且仓库内仍有大量多线程 dirty / untracked。

**关键结论**:
- 父治理层当前可以明确区分三件事：
  1. 默认基线与现行口径；
  2. 当前物理 worktree 落点；
  3. 当前 skills 体系是否已经足以支撑治理。
- 现阶段真正缺口不是“没有规则”，而是“如何把现有 rules + skills 进一步变成更强执行闸门”。
- `Skills和MCP` 已归档，相关 skills / MCP 治理事实上一并回收到 `Codex规则落地` 主线。

**恢复点**:
- 父治理层后续如果继续推进，优先方向仍是：
  - `skills / AGENTS / 执行机制重构`
  - `四件套与代办体系重构`
  - 而不是继续扩写旧历史入口。

### 会话 11 - 2026-03-17（09 阶段已建立，治理线程准备切回 NPC）

**用户目标**:
- 要求我不要继续拖在大分析里；
- 先快速落盘下一阶段文档和初步记录，再把剩余治理重活全部沉淀成代办，以便立刻回到 `NPC` 修复。

**完成任务**:
1. 建立 `09_强制skills闸门与执行规范重构` 阶段目录，并落下 `tasks.md` 与 `design.md`。
2. 新建当前活文档层的 `03-17` 状态与总索引文件。
3. 新建 `Sunset强制技能闸门与线程回复规范_2026-03-17.md`，明确当前缺口与剩余动作。
4. 将 `03-16` 当前状态说明与 `03-16` 当前总索引复制留档到新的历史归档阶段目录。

**关键结论**:
- 父治理层已经把“强制 skills 闸门缺失”从口头批评转成正式下一阶段任务。
- 但当前只完成初步记录，不宣称已经做完技能闸门本体。

**恢复点**:
- 父治理层当前允许暂停进一步治理扩写；
- 当前线程可以基于已建好的 `09` 阶段代办，直接切回 `NPC` 问题处理。

### 会话 12 - 2026-03-17（10 阶段立项：共享根仓库分支漂移）

**用户目标**:
- 把 `NPC` / `farm` 这次分支与 worktree 混线问题彻底厘清；
- 并判断这是否属于需要新增治理阶段的真实反馈。

**完成任务**:
1. 取证核对当前根仓库分支、旧 worktree、reflog 和关键提交归属。
2. 确认 `07ffe199`、`18f3a9e1`、`11e0b7b4` 当前都只在 `codex/farm-1.0.2-correct001`。
3. 确认锁脚本会记录 `owner_branch`，但不会把“当前分支是否匹配线程语义”当作硬门槛。
4. 新建立项：
   - `10_共享根仓库分支漂移与现场占用治理`

**关键结论**:
- 父治理层当前新增了一类真实事故模型：
  - 文件锁存在；
  - 但共享根目录分支上下文仍可被错误复用。
- 这已经超出原先 `09` 阶段“强制 skills 闸门”单线问题，因此单独立项是合理的。

**恢复点**:
- 父治理层后续如继续治理，应并行关注：
  - `09`：强制 skills 闸门
  - `10`：共享根仓库分支占用治理

### 会话 13 - 2026-03-17（10 阶段已从取证转入执行裁定）

**用户目标**:
- 希望我不是停在“调查报告”，而是继续给出真正的后续安排。

**完成任务**:
1. 吸收 farm 的补证与立场。
2. 结合 `f6b4db2f` 与 farm 线提交差异，确认 NPC 更像已有救援线，farm 更需要 cleanroom。
3. 在 `10` 阶段新增 `执行方案.md`，把临时冻结、分流和线程执行令写死。

**关键结论**:
- 父治理层当前已经从“这是什么问题”推进到“现在先怎么做”。
- 后续若要真正把这次事故清完，需要让 NPC / farm 分别按新的执行令汇报，而不是继续在共享根目录里混跑。

**恢复点**:
- 父治理层当前已具备向两条线程下发执行令的条件。

### 会话 14 - 2026-03-17（治理工作区归位完成并补建 MCP 单实例层）

**用户需求**：
> 不只要文档迁移，还要把 `000_代办` 与正式工作区彻底拆干净，并顺手把 Unity / MCP 并行冲突补成正式规则层。

**完成任务**：
1. 确认正式治理工作区固定为：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`
2. 确认 `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办\codex` 已降级为 TD-only 镜像区。
3. 新建并接入：
   - `mcp-single-instance-occupancy.md`
   - `mcp-hot-zones.md`
   - `mcp-single-instance-log.md`
4. 新建 [外部历史容器退役说明_2026-03-17.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/12_治理工作区归位与彻底清盘/外部历史容器退役说明_2026-03-17.md)，并真实删除：
   - `D:\Unity\Unity_learning\Sunset_external_archives`
   - `D:\Unity\Unity_learning\Sunset_backups`

**关键结论**：
- 当前 live 结构已经不再允许“代办区兼任工作区”。
- shared root 中性与 Unity / MCP 单实例安全也已被明确拆成两层。

**恢复点**：
- 父治理工作区后续只继续维护结构性稳定结论。
- 真正的治理正文续办继续落在 `Codex规则落地`。

### 会话 15 - 2026-03-17（现行入口补强：shared root 实时状态、Play Mode 离场与表现层验收）
**用户目标**：
> 把当前 live 规则补到真正能约束线程的程度：既要说清 shared root 当前是否真中性，也要把 Play Mode 离场纪律和 UI / 字体 / 气泡 / 样式的验收标准并进现行入口。

**完成任务**：
1. 实核 shared root 现场已在 `main @ 663af03c`，但 working tree 仍有其他线程 dirty，主体是 NPC 业务文件并夹杂少量非治理记忆。
2. 更新 live 入口文档：
   - `Sunset当前唯一状态说明_2026-03-17.md`
   - `基础规则与执行口径.md`
   - `Sunset强制技能闸门与线程回复规范_2026-03-17.md`
3. 更新占用文档：
   - `shared-root-branch-occupancy.md`
   - `mcp-single-instance-occupancy.md`
   - `mcp-hot-zones.md`
4. 更新 steering 正文：
   - `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`

**关键结论**：
- shared root 现在是“默认入口模型已恢复到 `main + branch-only`”，不是“当前现场完全干净可直接切分支”。
- Play Mode 退出纪律和表现层审美验收，已经进入 live 规则，不再只存在于聊天提醒。

**恢复点**：
- 现行入口层已完成这轮补强。
- 后续若再遇到 shared root 不干净，优先看占用文档与 preflight，而不是机械把 `main` 当安全许可证。

## 2026-03-19（父治理层补记：Unity MCP 当前读链已复核通过，图驱动搭景的现行承诺值已明确）
- 当前父治理主线不变，仍服务于 Sunset 当前活入口与工具链口径统一；本轮用户要求先确认“现在的 MCP 链接是否正常”，再判断是否能基于图片和 prefab 索引自动搭高精度基础场景。
- 本轮已完成的稳定取证：
  - `mcp__unityMCP__manage_scene(action=get_active)` 成功返回活动场景 `Primary`
  - `mcp__unityMCP__read_console(action=get)` 成功返回最新 Console
  - `mcp__unityMCP__manage_scene(action=get_hierarchy)` 成功返回当前根层级
  - Console 中直接出现 `MCP-FOR-UNITY` 在 `8080` 端口停止后重新启动本地 HTTP server 的日志，说明当前读链不是历史缓存
- 父层稳定结论：
  - 当前 Unity MCP 至少在“场景 / Console / 层级”三条只读链路上是正常的
  - 本轮还没有复测写链、编译链和 EditMode tests，因此现行准确口径应是“读链正常，完整验证链本轮未复测”
  - 图驱动搭景当前统一承诺值为：
    - 2D / 正交 / prefab 对照清晰：`85%`
    - UI 布局或平面式摆放：`80%~90%`
    - 3D 单张透视图基础还原：`55%~70%`
    - 对用户当前的总体承诺值：`75%~85%`
- 父层恢复点：
  - 若后续进入这条主线，优先让用户提供图片、prefab 索引、目标场景路径、尺度参考和自动化边界
  - 再由 `Skills和MCP` 线程承接“图驱动初稿搭建 + 回读校正 + 微调收口”的执行闭环
## 2026-03-23（父层补记：MCP 根因已从端口漂移继续收紧到旧线程缓存未刷新）
- 当前父层主线仍是“维护 Sunset 活入口与规则口径”，本轮继续处理的不是业务功能，而是 MCP live 误判根因。
- 新补到的稳定事实是：
  - shared root `main` 下的 `config.toml`、`8888` 监听、pidfile 与 `check-unity-mcp-baseline.ps1` 已能恢复一致；
  - 但旧线程 / 旧会话即使在读到新配置后，也可能继续沿用旧 MCP 路由缓存发请求。
- 因此父层 live 口径已升级为：
  - 若 `config + 8888 + pidfile + baseline` 都通过，但旧线程仍报旧端口 / 旧桥名 / `resources/templates` 为空，默认先判为“旧会话 MCP 路由缓存未刷新”，不要直接宣布服务端回滚。
- 本轮已同步更新：
  - `AGENTS.md`
  - `Sunset当前规范快照_2026-03-22.md`
  - `mcp-live-baseline.md`
  - 批次 `README.md`
  - 通用前缀 prompt / `scene-build` 前缀 prompt
  - `check-unity-mcp-baseline.ps1`
- 父层恢复点：
  - 后续线程若再贴“我这条会话还是打旧端口”的报错，优先回到这条规则判断是旧线程缓存问题，还是服务端真的坏了。

## 2026-03-25（父层补记：Unity Play Busy 第一刀已复测，不得误报“已解决”）
- 当前父层主线仍是维护 Sunset 的活入口与治理口径；本轮继续处理的是 Unity Play Busy 这条支线的“证据收口”，不是业务开发换线。
- 本轮补到父层的稳定事实：
  - 已按 `2026-03-24-UnityPlayBusy续查prompt-01.md` 做了一次最小受控复测，实例为 `Sunset@21935cd3ad733705`，并确认复测后已退回 Edit Mode。
  - `StaticObjectOrderAutoCalibrator.cs` 相关的进 Play 前扫描日志在新样本里已消失，说明第一刀确实移除了那条 Editor-only 前置扫描源。
  - 但新的关键耗时仍然几乎贴着旧基线：
    - `Domain Reload Profiling = 19053ms`
    - `ProcessInitializeOnLoadAttributes = 6120ms`
    - `AwakeInstancesAfterBackupRestoration = 7277ms`
    - `Asset Pipeline Refresh = 19.599s`
  - 因此当前父层准确口径必须是：
    - “第一刀有效，但收益不足以宣布 Play 慢已解决”
    - “当前新的第一责任点应继续锁在 `ProcessInitializeOnLoadAttributes` 及其项目自有 Editor-only 候选上”
- 父层当前收缩出的下一组候选是：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1BedSceneBinder.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1WorkbenchSceneBinder.cs`
  - 理由是它们同属 `[InitializeOnLoad]`，且含有面向 `Primary` 的全层级对象遍历路径，最贴近当前仍然很重的 Editor 装配段。
- 父层恢复点：
  - 后续如继续沿这条支线推进，必须坚持“再做一刀前先做更窄的取证”；
  - 不得因为第一刀被证明确实打掉了一条重活，就把整体问题对外表述成“已经解决”。

## 2026-03-25（父层补记：rg 默认搜索已提升为全局硬规则；Binder 续查被无关 compile red 卡住）
- 当前父层主线仍是维护 Sunset 活入口与治理口径；本轮包含两个层级动作：
  1. 把“搜索默认优先 `rg`”从 learning 提升为全局 `AGENTS.md` 的硬规则
  2. 继续推进 Unity Play Busy prompt-02 的 Binder 取证
- 新补到的稳定事实：
  - `C:\Users\aTo\.codex\AGENTS.md` 已新增“搜索默认口径”，以后搜索类任务默认优先 `rg` / `rg --files`，系统对象查询继续优先 PowerShell。
  - `SpringDay1BedSceneBinder.cs` / `SpringDay1WorkbenchSceneBinder.cs` 已加入默认关闭、可回退的 diagnostics 和 Auto Bind 短路开关。
  - 两个 Binder 已通过单文件校验，当前没有新增脚本级错误。
  - 但共享 Unity 现场当前已有无关 compile red：
    - `NPCDialogueInteractable.cs`
    - `CraftingStationInteractable.cs`
    - `SpringDay1WorldHintBubble.cs`
  - 因为这些不在本轮允许改动范围内，所以这轮未进入 `Play -> Stop` 复测，不能假装已经拿到 Binder 的 live 结论。
- 父层恢复点：
  - 后续如果无关 compile red 被清掉，这条支线就直接恢复到“启用 Binder diagnostics -> 默认样本 -> 短路对照样本 -> 立刻 Stop”的最小复测链；
  - 当前不得把这轮阻断误写成“Binder 已被排除”。

## 2026-03-25（父层补记：Unity / MCP live 新增 stop-early 与验收交接口径）
- 当前父层主线仍是“维护 Sunset 活入口与规则口径”；本轮继续处理的不是业务功能，而是把用户对验收效率和 live 噪音的要求真正并进现行基线。
- 本轮已同步更新：
  - `Sunset当前规范快照_2026-03-22.md`
- 父层新增稳定事实：
  - 所有 Unity / MCP live 取证进入前都必须先写清：
    - 需要什么证据
    - 最多跑几轮
    - 看到什么信号就 `Pause / Stop`
    - 最后退回什么状态
  - 一旦已拿到当前步骤所需证据，必须立刻 `Pause / Stop` 并退回 `Edit Mode`，不能继续刷日志洪水。
  - MCP / 自动化默认只负责逻辑自验与辅助证据；凡是进入“需要用户最终判断”的阶段，线程必须先完成自己能做的自验，再交专业验收指南与回执单。
- 父层恢复点：
  - 后续若再遇到线程长时间跑 Play / MCP 只为“多看点日志”，应直接按新的 live 基线判为执行偏差，而不是继续容忍成默认做法。

## 2026-03-25（父层补记：prompt-03 已完成 3 组最小样本，两个 Binder 已被实证排除为这一刀真核心）
- 当前父层主线仍是 Unity Play Busy 的证据式收窄；本轮继续的不是泛排查，而是只对两个 `SpringDay1*SceneBinder` 做 live 对照闭环。
- 新补到的稳定事实：
  - compile red 解除后，已按 prompt-03 跑完 3 组最小 `Play -> Stop`：
    - baseline
    - 只短路 BedBinder
    - 只短路 WorkbenchBinder
  - 对照结果里，`ProcessInitializeOnLoadAttributes` 没有因短路任何一个 Binder 而下降：
    - baseline：`7913ms`
    - Bed short-circuit：`7928ms`
    - Workbench short-circuit：`7963ms`
  - Binder diagnostics 显示其执行体本身代价只有 `0~4ms`，且在多个实际 Play 进入窗口内都出现 `skipped=editor-busy`。
- 因此父层准确口径已更新为：
  - 这两个 Binder 不是这一刀的真核心
  - 当前新的第一责任点应从“两处 Binder 候选”继续收窄为：
    - `ProcessInitializeOnLoadAttributes` 段里的非 Binder 初始化链
- 父层恢复点：
  - 后续如果继续这条支线，不应再围绕这两个 Binder 重复做同类对照；
  - 下一轮应直接从同一重段里的非 Binder 初始化链重新收窄。

## 2026-03-25（父层补记：prompt-04 双线并查后，`SaveManager` 已被排除，包层 `unity-mcp` 升级为单一第一责任点）
- 当前父层主线仍是 Unity Play Busy 的证据式收窄；本轮继续处理的不是泛排查，而是按 `prompt-04` 同时检验：
  - `SaveManager` / `AwakeInstancesAfterBackupRestoration`
  - 非 Binder `ProcessInitializeOnLoadAttributes`
- 新补到的稳定事实：
  - `SaveManager` 在 baseline 样本里稳定命中：
    - `[SaveManager] 从 AssetDatabase 加载 PrefabDatabase: Assets/111_Data/Database/PrefabDatabase.asset`
  - 但加上 editor-only diagnostics 后，`InitializeDynamicObjectFactory()` 自身耗时只有 `15~16ms`。
  - 在“禁用 editor AssetDatabase fallback”对照样本里，`SaveManager` 已完全不走 fallback，且 `dynamicFactoryInitialized=False`，但关键重段变化极小：
    - `ProcessInitializeOnLoadAttributes: 8906ms -> 9063ms`
    - `AwakeInstancesAfterBackupRestoration: 10281ms -> 10209ms`
    - `Asset Pipeline Refresh: 28.037s -> 28.012s`
  - 因而 `SaveManager` 已可判定为“命中 suspect 行为，但不是这一刀主真凶”。
  - 项目内剩余非 Binder 候选继续收缩后也明显偏弱：
    - `PersistentIdAutomator.cs` 只注册 `sceneSaving`
    - `DialogueChineseFontAssetCreator.cs` 更贴近 `ProcessInitializeOnLoadMethodAttributes`
- 父层准确口径已更新为：
  - 当前单一第一责任点不再是 `SaveManager`
  - 包层 `com.coplaydev.unity-mcp` 的 `InitializeOnLoad` 集群已经从背景噪音升级为真 suspect
  - 其中当前排在最前面的具体入口是：
    - `Editor/Services/Transport/Transports/StdioBridgeHost.cs`
- 父层恢复点：
  - 后续若继续这条支线，应优先围绕 `unity-mcp` 包层 `InitializeOnLoad` 做更窄取证；
  - 不应再回头重复 `SaveManager` 对照，也不应把项目内轻量非 Binder 入口误报成第一真凶。

## 2026-03-25（父层补记：用户要求先补“禁用项的设计初衷与安全边界”复盘）
- 当前父层主线仍是 Unity Play Busy 证据式收窄；本轮继续处理的不是新取证，而是用户对“我们到底动过什么、理解到什么深度”的追问。
- 父层新增稳定事实：
  - 当前这条线里真正被临时禁用过的，只有：
    - 两个 Editor-only Binder 的 Auto Bind
    - `SaveManager` 的 editor `AssetDatabase` fallback
  - 它们都不等于“包层 suspect 已被禁用”：
    - Binder 本质是编辑器内容补挂便利器
    - `SaveManager` fallback 本质是编辑器里对非 `Resources` 资产路径的兜底装载
    - `unity-mcp` 包层目前只做了设计级 suspect 排名，还没有动它的任何功能开关
  - `SaveManager` fallback 不能被误解成“业务主路径”：
    - 项目里还有 `PersistentManagers` 通过序列化 `prefabDatabase` 初始化 `DynamicObjectFactory`
    - 所以这条 fallback 更像 editor rescue / compatibility path，而不是唯一初始化来源
- 父层恢复点：
- 后续若继续对包层 suspect 动手，必须先补更深的设计初衷审查；
- 不能把这轮对本地 editor fallback 做过的取证方式，直接套到 `unity-mcp` 基础设施链上。

## 2026-03-25（父层补记：典狱长模式已纳入当前规范快照）
- 当前父层主线里，治理规则不再只靠聊天记忆维持。
- 新补到的稳定事实：
  - `Sunset当前规范快照_2026-03-22.md` 已新增 `6B. 典狱长模式`
  - 当前快照已正式写明：
    - 用户说出 `典狱长 / 典狱长上货 / 上货` 时，治理线程先审回执再判停发
    - 线程必须先判成 `继续发 prompt / 停给用户验收 / 停给用户分析审核 / 无需继续发`
    - 给用户的转发格式固定为 `对应文件在 + text 代码块`
  - 同时，旧的“规则更新必须同步到批次 README / 通用前缀 prompt”口径已收紧为：
    - `AGENTS.md`
    - 当前规范快照
    - 治理规范正文
    - 相关 skill
- 父层恢复点：
  - 典狱长模式现在已经进入当前快照，后续不会再只是线程记忆层的临时偏好；
  - 后续若继续治理 prompt 分发，应直接按这版快照执行。

## 2026-03-26（父层补记：own-root 清尾责任已进入当前快照与项目总规则）
- 当前父层主线仍是维护 Sunset 的活入口与现行基线；本轮继续处理的不是业务功能，而是把“不能再靠典狱长事后扫尾”压成当前 live 纪律。
- 父层新增稳定事实：
  - `AGENTS.md` 与 `Sunset当前规范快照_2026-03-22.md` 现已统一写明：
    - `git-safe-sync.ps1` 在 `main-only` 的 `task` 模式下，虽然允许 unrelated dirty 保留，但如果白名单所属 `own roots` 下还有未纳入本轮的 remaining dirty / untracked，就必须直接阻断
    - 线程回执现在必须带 `当前 own 路径是否 clean`
    - 只要 `当前 own 路径是否 clean != yes`，就不能算本轮闭环，也不能直接进入下一轮 feature prompt
    - 治理线程默认不再为常规 own dirty 尾账反复开 cleanup 批次
  - 当前快照中的高危 / 强制报实口径也已扩为：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `TagManager.asset`
    - `StaticObjectOrderAutoCalibrator.cs`
  - 这轮还做了正反两组脚本样本验证：
    - clean own path + unrelated dirty：允许继续
    - 同根仍有 own docs tail：直接阻断
- 父层恢复点：
  - 之后再出现“线程自己的尾账没收，却想先拿下一轮 prompt”的情况，应该直接按当前快照视为执行偏差；
  - 后续治理重心将回到 4 个 unresolved hot / mixed 目标与恢复开发后的高危陪跑，不再回到常规扫地模型。

## 2026-03-26（父层补记：4 个强制报实 hot / mixed 目标已清空，shared root 已从热区阻塞恢复为仅剩 foreign 文档）
- 当前父层主线仍是维护 Sunset 的活入口与治理口径；本轮继续推进的不是新规则写作，而是把此前固定报实的 4 个 hot / mixed 目标真正从 working tree 收掉。
- 父层新增稳定事实：
  - `GameInputManager.cs / TagManager.asset / StaticObjectOrderAutoCalibrator.cs` 已经按 stable launcher 的 task 白名单 sync 收口，提交：`70d9c20c`
  - `Primary.unity` 已先剥掉明确可判的导航 `NPC debug override` residue，再按当前 integration baseline 收口，提交：`3fc6c3a2`
  - 当前 shared root `git status` 只剩：
    - `.codex/threads/Sunset/项目文档总览/memory_0.md`
  - 这说明 shared root 当前不 clean 的原因，已经不再是热文件 / mixed 文件现场，而是单条 foreign 文档脏改
- 父层准确口径已更新为：
  - 当前默认开发恢复的主要前提已经具备；
  - 但后续若再碰 `Primary.unity / GameInputManager.cs / TagManager.asset / StaticObjectOrderAutoCalibrator.cs`，仍需按高危目标陪跑，不得把这次治理收口误读成“以后这些文件可随手混写”
- 父层恢复点：
  - 后续治理重点不再是追 4 个 unresolved target 的去向；
  - 而是围绕恢复开发后的热区写入、线程交叉和 foreign dirty 隔离继续做现场把控
