# 项目文档总览 - 工作区记忆

## 2026-04-10｜Sunset 与“开始”项目线程 memory 勘察首轮完成

- 当前主线目标：
  - 先对 `Sunset` 与 `D:\迅雷下载\开始` 两边的线程 / 工作区 `memory*.md` 现场做一次完整勘察，确认哪些 memory 已严重超长、哪些目录已分卷、哪些还在单卷硬顶，并把这些结论吸收到 `项目文档总览` 的全局理解里。
- 本轮完成：
  1. 已确认“开始”项目根路径为：
     - `D:\迅雷下载\开始`
  2. 对 `Sunset` 全仓 `memory*.md` 做了按路径、行数、类型、目录聚合的只读普查。
  3. 对“开始”项目 `.codex/threads` 下的 `memory*.md` 做了同样的只读普查。
  4. 对 `spring-day1 / UI / NPC / 项目文档总览 / Codex规则落地 / 900_开篇` 等关键 memory 做了头尾抽样，确认这些超长卷确实承载大量活跃现场，而不是纯历史垃圾。
- 当前最重要的现场判断：
  1. `Sunset` 当前不是个别 memory 超长，而是治理线、主线剧情线、导航线、UI 线、NPC 线、存档线和 `项目文档总览` 自身都在单卷膨胀。
  2. 其中最重的未分卷热点包括：
     - `.kiro/specs/屎山修复/导航检查/memory.md` = `5572`
     - `.codex/threads/Sunset/spring-day1/memory_0.md` = `5187`
     - `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` = `5183`
     - `.codex/threads/Sunset/导航检查/memory_0.md` = `4371`
     - `.codex/threads/Sunset/UI/memory_0.md` = `3630`
     - `.codex/threads/Sunset/NPC/memory_0.md` = `3375`
     - `.codex/threads/Sunset/项目文档总览/memory_0.md` = `2178`
  3. `Codex规则落地` 虽然已经存在多卷，但当前主卷仍异常肥大：
     - `.kiro/specs/Codex规则落地/memory.md` = `13227`
     - 同目录合计 `8` 卷，总行数 `13564`
     - `.codex/threads/Sunset/Codex规则落地/` 也已有 `7` 卷，总行数 `8401`
     这说明它不是“没分卷”，而是“分卷结构已经存在，但主卷仍然承担了过量活跃内容”。
  4. “开始”项目相对简单，主要超长点集中在：
     - `.codex/threads/系统全局/对话丢失修复/memory_0.md` = `3518`（已分到 `memory_3`）
     - `.codex/threads/系统全局/谁是卧底V2/memory_0.md` = `1832`（未分卷）
     - `.codex/threads/系统全局/全局skills/memory_0.md` = `1654`（未分卷）
- 关键判断：
  1. 如果后续要治理，`Sunset` 的优先级不该按“谁最大”机械排序，而该按“谁最影响当前项目整体理解”来排。
  2. 以 `项目文档总览` 线程视角，优先吸收和后续处理的顺序应当是：
     - `spring-day1`
     - `900_开篇/spring-day1-implementation`
     - `UI`
     - `NPC`
     - `导航检查`
     - `Codex规则落地`
     - `存档系统`
     - `项目文档总览` 自身
  3. 这轮不应该直接拆老卷；更正确的下一步是先形成“高优先级 memory 吸收名单 + 后续分卷治理顺序”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\memory_0.md`
- 验证结果：
  - 本轮属于 read-first + docs-only 结论整理：未改代码、未改 Scene、未改 Prefab
  - 收尾已执行 `Park-Slice -ThreadName 项目文档总览 -Reason memory-survey-round1-landed`
- 下一步恢复点：
  1. 若继续本线，优先输出一份“`Sunset memory` 治理优先级总表”。
  2. 再按优先级逐条吃大卷内容，补全 `项目文档总览` 对核心线程的理解。

## 2026-04-09｜Codex 工作区 / AI 工作流 / MCP / skills / Unity 工具全盘点母卷已落地

- 当前主线目标：
  - 把 `Sunset` 里和 `Codex` 工作区、AI 使用、MCP、skills、Editor 工具、验证流程直接相关的真实材料，收成一份可被简历 / 作品集 / 面试稿继续筛选的全量母卷。
- 本轮完成：
  1. 新建：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`
  2. 将当前项目里最相关的内容重新按一卷集中整理，包括：
     - Unity `Editor` 工具体系硬统计
     - `Assets/Editor` 实际文件清单
     - `.kiro/specs` 工作区清单
     - `.codex/threads/Sunset` 线程清单
     - `steering / hooks / memory / thread-state` 治理栈
     - 全局 / Sunset skills 清单
     - `Skills和MCP` 与 `UnityMCP转CLI` 两条主线沉淀出的稳定事实
  3. 额外把“这些材料分别适合投什么岗位”“哪些适合写进简历、哪些更适合面试展开”的筛选口径一并写进母卷。
- 关键判断：
  1. 这轮最重要的不是继续压一句简历话术，而是先把“能站得住的完整上游素材池”补齐。
  2. 这份母卷和 `2026-04-08_给外部简历智能体_*` 三层材料不冲突，前者更偏“AI / tools / workflow / governance”专项盘点，后者更偏“对外压缩交接”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
- 验证结果：
  - `git diff --check -- .kiro/specs/项目文档总览/2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`：待本轮收尾统一复核
  - 本轮属于 docs-only 更新：未改代码、未改 Scene、未改 Prefab
  - 收尾已执行 `Park-Slice -ThreadName 项目文档总览 -Reason ai-tools-workflow-mother-volume-landed`
- 当前阶段：
  - `项目文档总览` 现已同时拥有：
    1. 外部简历智能体素材总包
    2. 压缩原则与禁写边界
    3. 三维度母卷
    4. 统一交接提示词
    5. AI / 工具 / workflow / 治理专项全盘点
- 下一步恢复点：
  1. 若继续补这条线，优先再拆：
     - “简历里可直接抽取的 AI / tools 版短母卷”
     - “作品集里可展开的治理 / 工具链 case 版”
  2. 若交给外部智能体压缩，可直接以这份全盘点作为 AI / 技术策划方向的上游事实池。

## 2026-04-09｜XMind 工程化管线正式落地

- 当前主线目标：
  - 把 `Sunset` 的项目总文与 `项目文档总览` 母材料，落成一套可持续生成 `.xmind` 的工程化管线，而不是继续停留在路线讨论。
- 本轮完成：
  1. 在 `D:\Unity\Unity_learning\Sunset\.kiro\xmind-pipeline\` 新建完整管线目录。
  2. 落下 `source-registry.json`、`topic-blueprints.ts`、`xmind-schema.json`、Markdown 抽取、归一化、稳定 ID、校验与 XMind 写出逻辑。
  3. 落下 `build:graph / validate:graph / generate:xmind / clean:output / smoke / experiment:incremental` 命令。
  4. 落下 `tests/xmind-pipeline.test.ts`。
  5. 用真实 `Sunset` 文档跑出 `sunset-master-graph.json` 和 `7` 张 `.xmind`。
- 本轮验证：
  - `npm run smoke`：通过
  - `npm test`：通过（6/6）
  - `output/validation-report.json`：`ok = true`
  - `output/generation-report.json`：`7/7` 导图 archive smoke 通过
  - `output/incremental-update-report.json`：`idStable = true`、`contentChanged = true`
- 当前正式口径：
  - 第一层长期主文 + 第二层 `项目文档总览` 母材料可以生成主节点。
  - 一次性治理 prompt 只做风险提醒和 source refs 补口，不直接长成主图结构。
  - `xmind-generator` 是当前主生成链；`xmind` SDK 作为已接入的 patch / adapter 预留层，不抢第一阶段主链。
- 恢复点：
  - 下一轮优先手工审 `output/` 下的 `7` 张导图，再按审图结果继续精修 `topic-blueprints.ts`。

## 2026-04-10｜Sunset memory 第一批吸收总表已落地

- 当前主线目标：
  - 不是继续做“有哪些超长 memory”的 survey，而是把 `Sunset` 当前最关键的大卷 memory 真正吸收到 `项目文档总览` 的理解里，并形成后续长期主文可直接复用的稳定口径。
- 本轮完成：
  1. 新建：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-10_Sunset_memory吸收优先级总表与第一批理解_01.md`
  2. 将本轮已读穿的高价值对象正式分成三层优先级：
     - 业务理解优先级：`spring-day1 / 900_开篇 day1 / UI / NPC / 导航检查 / 存档系统`
     - 治理理解优先级：`Codex规则落地 / 项目文档总览自身`
     - 跨项目补充优先级：`D:\迅雷下载\开始` 下的 3 条重点线程
  3. 把第一批已吸收结论写成正式文本，包括：
     - `day1` 已进入“主线已成、导演尾差待收、runtime 终验待补”
     - `900_开篇` 的价值在于校正阶段理解，不再把旧对白验证当当前 blocker
     - `UI` 已转成玩家面集成、workbench/prompt/proximity 止血与体验证据线
     - `NPC` 已推进到 resident 化、`NPC_Hand` 真源、关系页结构、自漫游响应恢复
     - `导航检查` 已把问题压成 scene contract / runtime bridge / driver 三层责任
     - `存档系统` 当前不是“没做”，而是三场景 persistent baseline 已落地、live 门链仍待收口
     - `Codex规则落地` 的正确角色应固定为治理层，而不是业务总工作区
- 当前最重要的判断：
  1. `项目文档总览` 后续写法必须明显减少“功能清单化”和“治理现场话术化”。
  2. 现在更该写“每条线已经站到哪一层、还差哪一层”，而不是“做过哪些点”。
  3. 后续主文更新时，`day1 / UI / NPC / 导航 / 存档` 都应优先引用这份第一批吸收结论，而不是重新回头从旧长卷里捞阶段性表述。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-10_Sunset_memory吸收优先级总表与第一批理解_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\项目文档总览\memory_0.md`
- 验证结果：
  - 本轮属于 docs-only 更新：未改代码、未改 Scene、未改 Prefab
  - 收尾前需补：
    - `git diff --check`
    - `Park-Slice -ThreadName 项目文档总览 -Reason memory-absorption-wave1-landed`
- 下一步恢复点：
  1. 若继续本线，优先把这份第一批吸收结论继续回写到长期主文、进度总表和 AI 治理卷。
  2. 第二批再进入：
     - `Docx/大总结/Sunset_持续策划案/` 里仍带旧口径的主文
     - `D:\迅雷下载\开始` 的重点 memory

## 2026-04-10｜memory 分卷治理方向已重新校正

- 当前主线目标：
  - 回到用户真正要的事情：先按现行 steering / 规范快照把几个病态工作区的 memory 治理方向校正清楚，明确哪些该立即分卷、哪些该开新阶段、哪些只该把老卷冻住。
- 本轮完成：
  1. 已重新核对 `workspace-memory.md`、`rules.md`、当前规范快照与 `Codex规则落地/06_memory分卷治理与索引收紧`。
  2. 已确认当前硬口径不是“随便拆文本”，而是：
     - 工作区活跃卷固定为 `memory.md`
     - 历史卷归档为 `memory_N.md`
     - 线程活跃卷延续当前项目既有口径为 `memory_0.md`
     - 老卷归边后不再续写
  3. 已重新盘清几个核心病灶：
     - `Codex规则落地` = 已有分卷结构，但根主卷仍异常肥大
     - `spring-day1-implementation` = 父卷与 `003` 子卷都已超线，且阶段语义漂移
     - `导航检查` = 单主卷吞了多个不同问题域
     - `项目文档总览` = 工作区 healthy，但线程卷过胖
- 当前关键判断：
  1. 现在最该修的是“活跃卷失控 + 阶段混层 + 旧卷没真正退场”，不是继续在超长主卷上追加。
  2. 最好的治理方式不是回填式重写历史，而是：
     - 冻结当前病态主卷
     - 新建瘦身活跃卷
     - 让后续内容只进新阶段 / 新子工作区
- 下一步恢复点：
  1. 若继续本线，优先按工作区逐个执行真正分卷，而不是再做一轮泛盘点。
  2. 分卷优先级建议为：
     - `Codex规则落地`
     - `spring-day1-implementation`
     - `导航检查`
     - `项目文档总览` 线程

## 2026-04-10｜核心病态 workspace / thread memory 已完成第一轮真分卷

- 当前主线目标：
  - 不停留在方案层，直接把核心病态工作区和线程的活跃卷降回可维护状态，同时保留母卷内容。
- 本轮完成：
  1. 已完成母卷退场 + 新活跃卷重建：
     - `Codex规则落地` 工作区 / 线程
     - `spring-day1-implementation` 根卷 / `003` / 线程
     - `导航检查` 工作区 / 线程
     - `UI系统` 根卷 / `0.0.1 SpringUI` / 线程
     - `NPC` 根卷 / `2.0.0进一步落地` / 线程
     - `存档系统` 根卷 / 线程
     - `项目文档总览` 线程
  2. 已补新的语义承接阶段：
     - `Codex规则落地/26_memory主卷退场与活跃卷重建`
     - `spring-day1-implementation/004_runtime收口与导演尾账`
     - `导航检查/01_场景Traversal配置与桥水`
     - `导航检查/02_跨场景Persistent重绑`
     - `导航检查/03_NPC自漫游与峰值止血`
     - `UI系统/0.0.2_玩家面集成与性能收口`
     - `NPC/2.1.0_resident与关系页收口`
     - `NPC/2.2.0_自漫游与避让收口`
     - `存档系统/4.0.0_三场景持久化与门链收口`
- 当前关键判断：
  1. 这轮真正做成的不是“把长卷拆碎”，而是把活跃入口和历史母卷分开了。
  2. 下一轮如果继续治理，重点应从“继续拆卷”切到“盯住新活跃卷不要再次失控”。
- 下一步恢复点：
  1. 若继续本线，优先抽检新活跃卷是否又被错误回灌。
  2. 再决定是否扩到第二批历史重病区，如 `SO设计系统与工具` 等。

## 2026-04-10｜Codex规则落地补齐索引层

- 当前主线目标：
  - 把 `Codex规则落地` 这条刚退场成功的 memory 主线补成完整治理样板，避免“瘦身了，但旧卷没人知道怎么查”。
- 本轮完成：
  1. 新增工作区历史索引：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory_index.md`
  2. 新增线程历史索引：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_index.md`
  3. 为阶段 `26_memory主卷退场与活跃卷重建` 补齐：
     - `tasks.md`
- 当前关键判断：
  - `Codex规则落地` 这条线现在已经从“先手分卷”推进到“可持续维持”，后续不要再清洗旧卷正文，只做索引解释与活跃卷守门。

## 2026-04-10｜第一批吸收结论已正式回写长期主文四卷

- 当前主线目标：
  - 不是继续做 `memory` 勘察或分卷治理，而是把第一批已吸收结论真正回写进长期主文体系，校正旧阶段口径。
- 本轮完成：
  1. 已正式回写：
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\07_AI治理.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\08_进度总表.md`
     - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\04_剧情NPC.md`
  2. 已把第一批吸收后的正式口径落进主文：
     - `day1` 不再写成“主线没搭好”，而是“主线已成、导演尾差与 runtime 终验待补”
     - `UI` 不再写成泛界面建设，而是玩家面集成、刷新止血与 live 证据线
     - `NPC` 不再写成基础功能，而是 resident 化、真源、关系结构与自漫游纠偏
     - `导航` 不再写成泛 bug，而是 `scene contract / runtime bridge / driver` 三层责任
     - `存档` 不再写成底座缺失，而是三场景 persistent baseline 已落地、剩 live 门链和体验尾差
     - `Codex规则落地` 被重新钉死为治理层，不再代替业务主文
  3. 已把总览和进度总表里的阶段判断改成“已到哪一层、当前还差哪一层”，不再只保留旧三档口径。
- 当前关键判断：
  1. `Sunset` 当前主风险已经不是“什么都没做”，而是多条核心线已有真 contract，但导演尾账、placement 公共链和 live 终验仍未完全收平。
  2. 这轮之后，`项目文档总览` 可以直接把 `01 / 04 / 07 / 08` 当成最新母口径使用，不需要再从旧长卷里手动翻阶段说法。
- 验证结果：
  - 本轮属于 docs-only 更新：未改代码、未改 Scene、未改 Prefab
  - `git diff --check -- Docx/大总结/Sunset_持续策划案/{01_总览,04_剧情NPC,07_AI治理,08_进度总表}.md` 已通过
  - 当前 `项目文档总览` slice 已执行 `Park-Slice`，thread-state = `PARKED`
- 下一步恢复点：
  1. 若继续本线，优先用这套新口径继续回写仍带旧阶段话术的长期主文卷。
  2. 若继续吸收第二批 memory，应只服务后续主文校正，不再回到泛 survey。

## 2026-04-10｜简历素材母卷已落到外部目录并形成直取版

- 当前主线目标：
  - 不再继续扩写 `Sunset` 总览体系，而是把已经吃进来的项目事实、阶段判断、工具链、AI 协作和游戏履历，沉淀成一套可直接拼装简历的素材母卷。
- 本轮完成：
  1. 在外部目录新建：
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷`
  2. 已落下 10 份母卷文件：
     - `00_总索引与使用说明.md`
     - `01_项目事实基线.md`
     - `02_系统策划素材.md`
     - `03_执行策划与规则收口素材.md`
     - `04_Unity策划落地与工具链素材.md`
     - `05_AI原生策划推进素材.md`
     - `06_项目主导权与拍板案例.md`
     - `07_量化事实、慎写边界与简历句库.md`
     - `08_面试展开题池.md`
     - `09_简历积木母本_直取版.md`
  3. 其中 `09_简历积木母本_直取版.md` 已按用户最急需求整理成 5 大类：
     - 自我评价
     - 项目经历
     - 技能掌握
     - 游戏经历
     - 游戏拆解
- 当前稳定结论：
  1. 这轮最重要的产出不是再写一份项目总览，而是给后续简历压缩、网申裁剪和面试展开提供可直接拿来组装的“积木原件”。
  2. `01_项目事实基线.md + 07_量化事实、慎写边界与简历句库.md + 09_简历积木母本_直取版.md` 已经构成当前最稳的简历上游材料组合。
  3. 当前简历母卷已显式避开旧高风险写法，如：
     - 把 `53` 个工具写死
     - 把 `13` 个核心系统写死
     - 把 `30x / 300% / 3 人团队工作量` 当硬事实
     - 把 AI 协作写成“独立完成全部代码”
- 验证结果：
  - 本轮属于 docs-only / external-docs-only 更新：未改代码、未改 Scene、未改 Prefab
  - 外部目录文件已成功落盘，可直接作为后续简历组装母库使用
- 下一步恢复点：
  1. 若继续本线，优先直接用 `09 + 07 + 01` 压一版策划实习简历。
  2. 若继续补素材，只做针对岗位的定向压缩，不再回到泛总览扩写。

## 2026-04-10｜策划实习简历初稿两版已落地

- 当前主线目标：
  - 从“简历素材母卷”进一步压到“可直接投递 / 可直接删改”的简历正文，而不是继续停留在原件库层。
- 本轮完成：
  1. 在外部目录新增：
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\10_策划实习简历初稿_通投版.md`
     - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\11_策划实习简历初稿_技术型策划版.md`
  2. 两版都已按真实项目口径收成完整简历正文，覆盖：
     - 基本信息
     - 教育经历
     - 核心策划能力 / 核心能力
     - `Sunset` 项目经历
     - 游戏拆解与游玩经验
     - 自我评价
  3. 其中：
     - `10` 更偏通投游戏策划实习
     - `11` 更偏技术型策划 / 工具策划 / 系统策划方向
- 当前稳定结论：
  1. 当前外部母卷链路已经形成三层可直接使用结构：
     - 原件层：`01 ~ 09`
     - 简历正文层：`10 / 11`
     - 后续若继续，只需要做定向删改和岗位适配
  2. 这轮之后，用户如果要立刻投递，已经不需要再从长母卷自己手拼第一版。
- 验证结果：
  - 本轮属于 docs-only / external-docs-only 更新：未改代码、未改 Scene、未改 Prefab
  - 当前 slice 已执行 `Park-Slice -Reason resume-draft-files-landed`
- 下一步恢复点：
  1. 若继续本线，优先从 `10 / 11` 中择一压成最终一页版。
  2. 若按岗位继续细分，可再做：
     - 系统策划偏
     - 执行策划偏
     - 腾讯 / 大厂技术型策划偏

## 2026-04-10｜简历重写口径已校正到“沿用户现有框架深改”

- 当前主线目标：
  - 不再输出“我自造的一版通用简历初稿”，而是沿用户当前更认可的简历框架，在原有分类下做深度重写与提纯。
- 本轮新增判断：
  1. 用户当前明显更认可“已有框架 + 深度精修”这条路，而不认可过于通用、过于平铺的一页稿。
  2. 后续简历优化应优先保持这些板块：
     - 自我评价
     - 教育背景
     - 个人项目经历
     - 核心策划能力
     - 奖励荣誉
     - 游戏经历与拆解
  3. 项目经历必须继续以：
     - 策划主控权
     - `Town -> Primary -> Home` 主线收口
     - NPC resident 化
     - 连续世界 / 持久化体验
     - Unity 策划侧工具链
     - AI 协同治理
     作为最主要亮点，而不是退回普通学生式能力清单。
  4. 游戏经历部分不能只堆时长，必须继续转成：
     - 品类深耕
     - 设计认知
     - 对 `Sunset` 的实际反哺
- 当前稳定结论：
  - 简历这条线的正确方向已经进一步收紧为：
    - 保留用户认可的成熟分类框架
    - 用项目真实细节和更强的策划语言重写每一块
    - 少做“重新发明结构”，多做“主控权、设计价值和落地事实”的强化
- 下一步恢复点：
  1. 若继续本线，直接基于用户当前这版框架继续打磨最终投递稿。
  2. 不再优先扩外部母卷，除非某一块事实明显不够支撑写法。
