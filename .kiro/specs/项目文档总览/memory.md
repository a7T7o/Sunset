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
## 2026-04-16｜只读审计 `系统全局`：已分清哪些是 Sunset 母卷该继续吸收的“方法层源头”

**当前主线目标**：
- 用户要求只读审计 `D:\迅雷下载\开始\.codex\threads\系统全局`，不是再写简历文案，而是回答四件事：
  - 哪些材料能直接提升对 Sunset 系统层 / AI 治理层的理解
  - 哪些已被当前母卷 / 主文吸收
  - 哪些还没被当前简历母卷充分吸收，但对“主控方式 / 系统设计方法 / AI 协同方式”很关键
  - 哪些虽然精彩，但和 Sunset 求职表达无关或容易带偏

**本轮实际完成**：
1. 已把外部目录按价值分成 4 层：
   - 核心源头层：`全局skills / 典狱长-V2 / 对话丢失修复 / UnityMCP转CLI价值分析 / 谁是卧底 / git治理多线程回应`
   - 历史交叉说明层：`000_git治理交叉内容`
   - 低相关外部调研层：`CLI-Anything外部调研 / mcporter外部调研 / opencli外部调研`
   - 与 Sunset 求职表达弱相关或偏题层：`XMind拆解路线分析 / 学长 / 本机ClashVerge端口核对 / ONNX导出需求判断 / 任务管理器异常排查`
2. 已与当前正式对照面完成比对：
   - `2026-04-08_给外部简历智能体_Sunset三维度母卷_01.md`
   - `2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`
   - `Docx/大总结/Sunset_持续策划案/06_工具链.md`
   - `Docx/大总结/Sunset_持续策划案/07_AI治理.md`
   - `2026-04-08_给外部简历智能体_压缩原则与禁写边界_01.md`
3. 已明确当前主文已经吸收较充分的是“结果层”：
   - 规则栈 / 工作区栈 / memory 栈 / hooks / locks / thread-state
   - `CLI first, direct MCP last-resort`
   - 本地化 skills 与 `skill-trigger-log`
   - 典狱长 / 看守长分流的结论层
4. 已明确当前母卷仍缺的主要是“方法层 / 源头层”：
   - `全局skills` 里“为什么不是原样装第三方 skill，而是拆思想再本地化”的演化链
   - `对话丢失修复` 里“全局事故恢复线程如何演化成治理总装线程”的职责迁移
   - `典狱长-V2` 里“治理不是继续发 prompt，而是先做四类裁定 + own-roots 闸门 + cleanup 退出默认化”的执法思路
   - `UnityMCP转CLI价值分析` 里“为什么 CLI 化目标应该是 orchestration CLI，而不是接口平铺包装”的路线判断
   - `谁是卧底` 里“先收窄 suspect、保留被证伪路径、只做 diagnostics-only 的安全取证”这一套故障分析方法

**关键判断**：
1. 外部 `系统全局` 对 Sunset 当前最有增量的，不是再补“用了哪些名词”，而是补“这些机制是怎么被逼出来的、为什么这么设计、它们替项目挡住了什么错路”。
2. 现在的简历母卷和长期主文，已经能讲“我们最后形成了什么治理结构”，但还不够能讲“用户作为主控，是如何把外部方法、失败试验和多轮纠偏压成今天这套系统的”。
3. 若后续要做第二批吸收，应优先把上面 5 条“方法层源头”择要回写到：
   - AI 治理卷
   - 工具链卷
   - 项目文档总览的后续简历 / 作品集上游母本
   同时继续避免把系统级故障细节、外部调研流水账和低相关工具实验直接塞进 Sunset 主叙事。

**涉及文件**：
- `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\典狱长-V2\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\V2交接文档\01_线程身份与职责.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\V2交接文档\06_证据索引_必读顺序_接手建议.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\UnityMCP转CLI价值分析\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-unity-mcp包层启动链设计审查.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-08_给外部简历智能体_Sunset三维度母卷_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-09_Codex工作区_AI工作流_MCP_skills_工具全盘点_01.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\06_工具链.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\07_AI治理.md`

**验证结果**：
- 本轮为只读外部目录审计 + 仓库内对照阅读：未改业务代码、未改 Scene、未改 Prefab。
- 结论属于“文档与记忆对照已站稳”，不是新的简历成品，也不是新的主文回写。

**下一步恢复点**：
1. 若继续本线，优先把这次审计确认的“方法层增量”择要回写到 `AI治理` / `工具链` / 对外母卷。
2. 不再对 `系统全局` 做泛扫；下一次只应针对这 5 条高价值源头继续精读。
## 2026-04-16｜Sunset 深度认知收口：系统层 / runtime 层 / AI 主控层分层判断

**当前主线目标**：
- 用户明确要求先暂停简历成稿，转为彻底读懂 `Sunset` 当前项目本体、Day1 / NPC / 存档 / 工具链 / AI 治理之间的真实关系；
- 目标不是再拼一版项目介绍，而是先得到一套稳定的“我现在到底如何理解这个项目”的认知底图。

**本轮实际完成**：
1. 已把当前对 `Sunset` 的理解，从旧“系统很多的生活模拟 Demo”升级为：
   - 一个已经进入 `多系统运行底座 + Town -> Primary -> Home 主线承接 + resident 聚落秩序 + persistent baseline + runtime 收口` 阶段的 Unity 6 项目。
2. 已重新核准 `Day1` 当前不该再被理解成“收尾 polish”：
   - 当前更准确的 live 事实是：主线承接已成，但 `SpringDay1Director / SpringDay1NpcCrowdDirector` 仍存在 owner 越界，核心问题已升级为 `Day1 runtime decoupling`。
3. 已把 `NPC` 当前真实完成面重新压实为：
   - `NpcCharacterRegistry` 角色主表
   - `NpcResidentRuntimeContract / Snapshot`
   - `NPCAutoRoamController` facade 化
   - `Town` scene-side resident slot / order 语义
   而不再只是“NPC 会漫游 / 会说话”。
4. 已把 `存档 / 持久化` 当前真实完成面重新压实为：
   - 普通存档槽 + 默认开局旧壳清理
   - `Primary / Town / Home` 三场景 persistent baseline
   - `scene-aware` load / restore
   - 玩家 runtime roots + inventory/hotbar + 部分 resident / world continuity
   同时明确不能再误写成“完整通用全世界态持久化已经做完”。
5. 已把 `策划侧工具链` 的价值重新压实为：
   - 它不只是批量造 SO，而是把数据生产、世界资产接线、场景整理、剧情 / NPC / UI 验证和运行态探针收成一条真正的生产与验证链。
6. 已把 `AI 主控` 当前最值钱的部分重新压实为：
   - 不是会说 `hooks / steering / memory / thread-state` 这些黑话；
   - 而是用户以策划主控身份，持续做需求定义、边界裁定、回执审核、停发决策、黑盒回归、终验回拉，把多执行体开发压回同一条主线。
7. 已把外部 `系统全局` 目录的作用重新定性为：
   - 它最有价值的不是给 `Sunset` 再补结果层名词，而是补“为什么会形成今天这套治理 / 工具 / CLI / suspect 收窄方法”的源头逻辑。

**关键判断**：
1. 以后对 `Sunset` 的项目认知，必须至少分成 4 层：
   - `项目本体层`：生活模拟 + 聚落复兴 + Day1 三场景主线承接
   - `系统运行层`：经营成长、剧情导演、NPC resident、交互 / 背包 / 放置 / 农田 / 工作台、跨场景 continuity
   - `工程支撑层`：策划侧 Editor 工具链、运行态探针、scene-aware persistence、CLI-first 验证链
   - `主控治理层`：多 Agent 并行开发的边界裁定、回执审核、停发与终验
2. 以后再写项目表达时，不能把第 3 / 4 层直接骑在第 1 层头上；正确顺序仍应是：
   - 先讲项目和系统跑成了什么
   - 再讲你是怎么靠工具链和 AI 主控把它推进出来的
3. 以后凡是出现以下旧口径，应直接视为过时或禁写：
   - `resident 化`、`formal 一次性消费`、`runtime 语义`、`placement 公共链`
   - `hooks / steering / memory / Begin-Slice / Ready-To-Sync / Park-Slice`
   - 把 `Day1` 写成“只差一点终验”的收尾项目
   - 把 `存档` 写成“全世界态通用持久化已完全完成”

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\02_经营成长.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\06_工具链.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\07_AI治理.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\08_进度总表.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\102-owner冻结与受控重构\2026-04-15_Day1-V3_Day1现存问题总览与整体施工总表.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\004_runtime收口与导演尾账\2026-04-15_spring-day1_Day1历史语义_系统边界_现状问题_重构交接总文档.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
- `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\典狱长-V2\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\UnityMCP转CLI价值分析\memory_0.md`
- `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底\2026-03-25-unity-mcp包层启动链设计审查.md`

**验证结果**：
- 本轮为只读深挖：未改业务代码、未改 Scene、未改 Prefab。
- 结论属于“项目理解和表达边界已重新站稳”，不是新的项目正文成稿。

**下一步恢复点**：
1. 若继续本线，下一步不应直接“写项目经历”，而应先把这 4 层认知压成一份稳定表达骨架。
2. 在用户认可这套认知骨架后，再进入真正的《Sunset》项目条目重写。
### 补记｜运行时主链再校准

- 刚补到的一条关键结论是：`Sunset` 当前最容易被低估的不是“功能数量”，而是这些系统已经被压成几条共享状态、共享取消权、共享恢复权、还能跨场景续跑的 runtime 链。
- 当前最值得长期保留的 6 条 runtime 主链理解是：
  1. 时间 / 季节驱动链
  2. 背包 / Hotbar / 手持真值链
  3. 世界目标选择与交互优先级链
  4. 放置 / 农田执行链
  5. 资源转化与工作台 / 箱子链
  6. 正式存档 + 跨场景 runtime 承接链
- 这意味着后续无论写主文、作品集还是简历，都不能再把它退回成“背包、农田、箱子、工作台、存档五个并列小功能”；更准确的理解应该是“一套共享状态语言下的多链 runtime 系统”。
### 补记｜项目表达骨架版已落盘，但本轮 sync 被锁阻断

- 已新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-16_Sunset项目表达骨架版_01.md`
- 当前这份骨架已把《Sunset》后续表达收成：
  - 1 段推荐项目简介
  - 4 条主成果
  - 1 条备选展开
  - 1 组禁写边界
- 本轮未完成白名单 `sync`：
  - `Ready-To-Sync` 被 `.kiro/state/ready-to-sync.lock` 阻断
  - 当前已按规则 `Park-Slice`
  - 口径应保持为：`骨架文件已落盘，sync 尚未完成`
### 补记｜用户明确否决“骨架口吻”和黑话表达，改为直接输出可投版正文

- 用户最新纠偏非常明确：
  1. 不要再回“骨架”
  2. 要直接在对话框给出简历最终可投版
  3. 严禁继续使用黑话和内部术语，如：
     - `Town - Primary - Home 的 Day1`
     - `Hotbar`
     - `Preview - Locked - Navigating - Executing`
     - `runtime`
     - `PersistentPlayerSceneBridge`
     - `inventory / placement / resident`
- 同时用户明确要求：
  1. 项目成果固定改为 `4` 点
  2. 原第 `3/4` 点合并
  3. AI 那一点必须写清：
     - 解决了什么问题
     - 你怎么主控
     - AI 到底怎么被你变成受控生产力
  4. 不能再写成“AI 协同很厉害”的空壳表达
### 补记｜《Sunset》项目正文最终定稿原则已收紧为 4 条

- 本轮最终判断已经进一步收紧：
  - 不是回到旧版长墙
  - 也不是继续写抽象总结
  - 而是保住旧版最硬的项目锚点，同时用现在更直接的人话痛点表达把它重新打亮
- 当前最终推荐的 4 条成果方向固定为：
  1. `主线收口与版本推进`
  2. `成长骨架与剧情驱动接线`
  3. `交互规则、连续世界与聚落秩序收口`
  4. `策划侧工具链与 AI 主控推进`
- 这 4 条的硬要求是：
  1. 每条都必须保留项目专属硬事实，例如：
     - `Day1 三场景可玩主线切片`
     - `四季五阶段`
     - `工作台成长链`
     - `36 格背包 + 12 格快捷栏`
     - `农田 1+8 邻接规则`
     - `围观、带路、散场、回家、恢复漫游`
     - `20+ 项高频交互边界问题`
     - `剧情导演验证 / NPC导航存档校验 / 运行态探针 / 回执审核 / 终验回拉`
  2. 每条都必须同时交代：
     - 你解决了什么问题
     - 你做了什么
     - 项目因此进入了什么状态
  3. 不能再把亮点洗成抽象词，例如：
     - `共享状态语言`
     - `运行底座`
     - `连续体验链`
     这些只能作为内部理解，不该成为最终正文主表达
### 补记｜AI项目融合单条的改判：标题不能再站在能力层

- 用户针对“策划侧工具链与AI主控推进”这条再次纠偏后，当前判断已经进一步明确：
  - 问题不只是句子太长
  - 而是标题与正文都站在“能力层”，所以天然会和核心能力第 `1/2/3` 条对冲
- 当前正确改法不是继续压短“工具链与AI主控推进”这几个字，而是先改标题语义：
  - 从“我会什么”改成“我在 Sunset 里解决了什么项目问题”
- 当前这条在项目部分更合理的定位应是：
  - `复杂 Demo 的并行推进与风险收口`
  - 或 `高耦合模块的并行落地与风险收口`
- AI 与工具链在项目条目中的正确位置是：
  - 作为推进手段出现
  - 不能再作为标题本体
  - 也不能复述成核心能力里的“AI 工作流 / 工具链能力”
### 补记｜《Sunset》项目 5 点终稿重写口径已确定

- 本轮已按最新纠偏把《Sunset》项目正文重新收成 `5` 点，而不是继续压成 `4` 点模板。
- 当前最终口径为：
  1. `从0到1搭建多系统运行底座`
  2. `主线收口与版本推进`
  3. `成长骨架与剧情驱动接线`
  4. `交互规则、连续世界与聚落秩序收口`
  5. `复杂Demo并行推进与风险收口`
- 最新稳定判断：
  - 简介只负责项目定位、主控身份、总体量和阶段，不再和第一条成果撞车
  - 第一条保留系统跑面与项目体量感
  - 第五条彻底改写为项目问题导向，不再复述核心能力里的工具链与 AI 能力名

## 2026-04-16｜经营成长 + 交互系统 + 运行时承接：按运行时链路重排的系统全景审计

**当前主线目标**：
- 用户明确要求只读分析，不写简历；围绕 `经营成长 + 交互系统 + 运行时承接` 做一次系统全景审计。
- 本轮子任务不是按旧文档目录复述，而是把系统按真实运行时链路重新分类，并区分：
  - 哪些事实已被主文稳定总结
  - 哪些仍只是局部文档或代码事实
  - 哪些最能体现项目复杂度与耦合深度
  - 哪些最容易被外部人误写成“做了一堆普通功能”

**本轮实际完成**：
1. 已通读并交叉核对：
   - `Docx/大总结/Sunset_持续策划案/02_经营成长.md`
   - `Docx/大总结/Sunset_持续策划案/03_交互系统.md`
   - `Docx/大总结/Sunset_持续策划案/08_进度总表.md`
   - `.kiro/about/01_系统架构与代码全景.md`
2. 已补读并对齐相关工作区记忆：
   - `.kiro/specs/项目文档总览/memory.md`
   - `.kiro/specs/制作台系统/memory.md`
   - `.kiro/specs/物品放置系统/memory.md`
   - `.kiro/specs/存档系统/memory.md`
   - `.kiro/specs/农田系统/memory.md`
3. 已核对关键运行时代码触点：
   - `GameInputManager.cs`
   - `InventoryService.cs`
   - `PlacementManager.cs`
   - `SaveManager.cs`
   - `PersistentPlayerSceneBridge.cs`
   - `ChestController.cs`
   - `CraftingService.cs`
   - `CraftingStationInteractable.cs`
   - `SpringDay1WorkbenchCraftingOverlay.cs`
   - `FarmTileManager.cs`
   - `CropController.cs`
   - `CropManager.cs`
   - `BoxPanelUI.cs`
   - `InventoryInteractionManager.cs`
4. 已把系统按真实运行时承接重排为 6 条主链，而不是旧文档那种“经营 / 交互 / 存档 / 箱子 / 制作台并排”：
   - 时间 / 季节驱动链
   - 库存 / Hotbar / 手持真值链
   - 世界目标选择与交互优先级链
   - 放置 / 农田执行链
   - 资源转化与工作台 / 箱子链
   - 正式存档与跨场景 runtime 承接链

**关键判断**：
1. 当前项目最值钱的复杂度，不在“功能数量多”，而在多条共享状态链已经被压成一套可恢复、可中断、可跨场景续跑的 runtime 合同。
2. 当前主文已经能稳定总结很多“结构层”事实，但仍明显低估了几处真正的承接复杂度：
   - `GameInputManager` 实际是交互总调度器，而不只是输入脚本
   - `PersistentPlayerSceneBridge` 实际是跨场景 runtime 总承接，而不只是“持久化玩家”
   - `SaveManager` 与 `PersistentPlayerSceneBridge` 实际形成双持久化层
   - 制作链并非单线：通用 `CraftingService` 与 `spring-day1` 专用 workbench overlay 并行存在
3. 一个关键反证已经站稳：
   - `RecipeData` 资产真实存在于 `Resources/Story/SpringDay1Workbench`
   - 但 `MasterItemDatabase.asset` 的 `allRecipes` 仍为空槽
   - 说明“故事工作台可跑”与“通用制作数据库已闭环”不是同一件事
4. 当前最容易被误写的，不是“有没有背包 / 农田 / 箱子 / 制作台 / 存档”，而是外部人会漏掉这些系统共享取消权、共享恢复权、共享数据真值与跨场景续跑语义。

**涉及文件**：
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\02_经营成长.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\03_交互系统.md`
- `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\08_进度总表.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\about\01_系统架构与代码全景.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\InventoryService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Crafting\CraftingService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Box\BoxPanelUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\111_Data\Database\MasterItemDatabase.asset`

**验证结果**：
- 本轮为只读审计：未改代码、未改 Scene、未改 Prefab、未进入真实施工。
- 结论建立方式为“主文 + 工作区记忆 + 关键代码 / 资产交叉核对”；属于分析性稳定结论，不是新的业务实现或 live 验证结果。

**下一步恢复点**：
1. 若继续这条线，应该把这次按运行时链路重排的结论择要吸收回主文，尤其补强：
   - 交互总调度
   - 双持久化层
   - 故事专用工作台与通用制作并行
   - world runtime restore
2. 若后续有人要做对外表达或系统介绍，不应再按“背包 / 农田 / 箱子 / 制作台 / 存档”平铺，而应优先按这 6 条运行时主链组织。

### 补记｜《Sunset》项目经历最终改判为 5 点正文，不再压成 4 点

**用户目标**：
- 用户最终要求不是继续看“骨架”或抽象分析，而是直接产出《Sunset》项目经历的对话框可投版正文。
- 用户明确要求：提炼压字，但不能删亮点、不能洗平项目体量、不能再把项目条目写成和核心能力重复的能力表。

**本轮完成**：
- 已正式把项目经历结构改判为 `5` 点，不再继续沿用上一轮被用户否掉的 `4` 点版。
- 已明确简介与成果条目的分工：
  - 简介只负责项目类型、个人职责、总体量、当前阶段
  - 成果条目负责“你在 Sunset 里具体解决了什么问题、做成了什么结果”
- 已明确项目正文的 5 条固定方向：
  1. `从0到1搭建多系统运行底座`
  2. `主线收口与版本推进`
  3. `成长骨架与剧情驱动接线`
  4. `交互规则、连续世界与聚落秩序收口`
  5. `复杂Demo并行推进与风险收口`

**关键决策**：
- 第一条必须保住“系统跑面”和“0~1落地体量”，不能再被抽象词洗掉。
- 第五条不能再写成“我会 AI 协同 / 我会工具链 / 我会拆需求”这种能力口吻，否则会直接和核心能力第 `1/2/3` 条对冲。
- 第五条的正确口径是：
  - 站在 Sunset 项目推进问题上写
  - 把 AI、工具验证、回执审核写成推进手段
  - 核心落点是“复杂 Demo 在单人条件下如何被稳住、如何避免越做越散”
- 最终正文继续保住这些项目锚点：
  - `Day1 三场景可玩主线切片`
  - `13 个核心系统`
  - `四季五阶段`
  - `工作台成长链`
  - `36 格背包 + 12 格快捷栏`
  - `农田 1+8 邻接规则`
  - `围观、带路、散场、回家、恢复漫游`
  - `20+ 项高频交互边界问题`

**验证状态**：
- 本轮仍为文本表达重写与结构判断，不涉及业务代码、Scene、Prefab 改动。
- 结论建立在：用户多轮强纠偏 + 已审计代码/文档/线程材料 + 旧版本对比后的稳定判断。

**下一步恢复点**：
- 以后若继续改《Sunset》项目经历，不要再回退到 4 点模板，也不要再把第 5 点写回能力条目。
- 直接以当前这 5 点结构为唯一正文基线继续微调字数和视觉长度。

**thread-state**：
- 本轮收尾前已执行 `Park-Slice`
- 线程名：`项目文档总览`
- 停车原因：项目正文已完成，本轮停在对话框交付

## 2026-04-22｜Sunset 项目彻查双层母文档已落地

- 当前主线目标：
  - 先把 `Sunset` 项目本体彻查清楚，再回到简历技术策划主轴重写。
- 本轮完成：
  1. 在外部目录建立两份长期母文档：
     - `000_Sunset项目彻查与技术策划转译总表.md`
     - `000_Sunset项目彻查批次草稿本.md`
  2. 把项目认知重新拆成两层：
     - 压缩总表层
     - 厚草稿本层
  3. 完成并记录四条高价值主链的首轮彻查：
     - 运行时交互与经营成长链
     - 存档、跨场景承接与持久化链
     - NPC、导演、导航与 Day1 runtime 边界
     - 工具链、AI 治理与 Git 协作
- 当前关键判断：
  1. `Sunset` 的项目价值不在“功能很多”，而在多系统运行底座、跨场景承接、runtime owner 边界、AI/Git/工具链治理这四层同时成立。
  2. 后续简历写法应优先从这四层抽取事实句，而不是继续堆系统清单。
  3. `项目文档总览` 后续可直接把 `总表` 当压缩事实池，把 `草稿本` 当原始认知池。
- 涉及文件：
  - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\000\000_Sunset项目彻查与技术策划转译总表.md`
  - `D:\QQ\Project\综合实训三NEW\刀\简历素材母卷\000\000_Sunset项目彻查批次草稿本.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\简历AI梳理\memory_0.md`
- 验证结果：
  - 本轮为只读审计与文档沉淀：未改业务代码、未改 Scene、未改 Prefab。
  - 结论建立在 live 文档、关键代码、项目文档总览与外部母卷交叉核对之上。
- 下一步恢复点：
  1. 若继续本线，后续优先从草稿本里抽取简历可写句。
  2. 若继续细分，还应补一轮把“项目经历 5 点 / 核心能力 4 条”再做一次基于这四条主链的最终映射。

## 2026-04-23｜README 与项目叙事素材只读梳理完成

- 当前主线目标：
  - 只读梳理 `Sunset` 仓库内外与 README、项目叙事、接手入口相关的历史演变和高价值素材，明确哪些能直接转成根 README，哪些更适合下沉为阅读地图或接手文档。
- 本轮完成：
  1. 已核当前根 README 现场，确认它仍是“`README 草案 v2（可落根版）`”，本质是候选稿而非最终对外页。
  2. 已交叉读取当前最强的对外表达母文：
     - `2026-04-16_Sunset项目表达骨架版_01.md`
     - `2026-04-08_给外部简历智能体_Sunset项目素材总包_01.md`
     - `2026-04-08_给外部简历智能体_Sunset三维度母卷_01.md`
     - `Docx/大总结/Sunset_持续策划案/01_总览.md`
  3. 已回看 `.kiro/about/00~02` 与 `History/2026.03.07-Claude-Cli-历史会话交接/总索引.md`，确认接手 / 阅读地图 / 代码全景这三层入口已经存在，不需要再让 README 首页吞下全部细节。
  4. 已补看更早期的：
     - `Docx/分类/全局/000_项目全局文档.md`
     - `Docx/大总结/第一阶段完结报告.md`
     用来对比叙事如何从“基础系统清单 + 完成度”演变到“跨场景主线 + 连续世界 + 项目推进方式”。
- 当前关键判断：
  1. README 叙事演变已经很清楚：
     - 早期更像阶段报告和系统盘点；
     - 中期转成项目总览、核心闭环和跨场景主线；
     - 当前最佳表达已经明确是“先项目本体，后协作治理”，并突出 `Town -> Primary -> Home`、连续世界与多系统承接。
  2. 根 README 首页最适合放：
     - 一句话定位
     - 核心玩法 / 叙事前提
     - 3~5 条项目亮点
     - 可视化证明（GIF / 截图 / Demo）
     - 当前状态
     - 阅读地图入口
     - 最小运行 / 打开方式
  3. `.kiro/about/00~02` 更适合继续承担：
     - 阅读地图
     - 系统架构与代码全景
     - 当前状态、漂移点与接手顺序
     而不应被整段平移到 README 首页。
  4. 真正可投递 / 可开源展示的 README 需要补齐当前草案缺口：
     - 可视化证据
     - 最小运行入口
     - 面向外部读者的体验路径
     - 对“我的作用 / AI 协作”的节制表达
     - 开源仓库常规信息（至少状态、依赖 / 运行前提、License 策略）
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-16_Sunset项目表达骨架版_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-08_给外部简历智能体_Sunset项目素材总包_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-08_给外部简历智能体_Sunset三维度母卷_01.md`
  - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\about\00_项目总览与阅读地图.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\about\01_系统架构与代码全景.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\about\02_当前状态、差异与接手指南.md`
  - `D:\Unity\Unity_learning\Sunset\History\2026.03.07-Claude-Cli-历史会话交接\总索引.md`
- 验证结果：
  - 本轮为只读梳理：未改代码、未改 Scene、未改 Prefab、未改 README 正文。
- 下一步恢复点：
  1. 若继续本线，可直接基于本轮结论把根 README 重写成“首页 + 阅读地图 + 最小运行说明”的正式公开版。
  2. 若先不改 README，也可以先把“首页内容 / 下沉内容 / 章节骨架”独立收成一份 README 施工提纲。

## 2026-04-23｜玩法内容与系统底座只读事实盘点

- 当前主线目标：
  - 基于仓库真实代码与 live 文档，重新回答 `Sunset` 当前“玩家 Day1 能做什么、核心系统落地到哪、README 最该高亮什么、哪些历史说法已经过时”。
- 本轮子任务：
  - 只读交叉核对 `README.md`、`Docx`、`.kiro/specs/900_开篇/spring-day1-implementation`、`Assets/YYY_Scripts` 与 `Assets/YYY_Tests/Editor`，输出一份高密度事实清单。
- 本轮完成：
  1. 确认 Day1 现行 runtime 主线以 `StoryPhase` 为准，当前阶段链为：
     `CrashAndMeet -> EnterVillage -> HealingAndHP -> WorkbenchFlashback -> FarmingTutorial -> DinnerConflict -> ReturnAndReminder -> FreeTime -> DayEnd`。
  2. 确认 Day1 当前真实教学目标不是旧文档里的“8 格耕种 + 独立砍树教学”，而是导演脚本里的 `5` 项：
     开垦 `1` 格、播种 `1` 次、浇水 `1` 次、收集 `3` 份木材、完成 `1` 次基础制作。
  3. 确认 Day1 已落地傍晚与夜间承接：
     `18:00` 晚餐冲突、`19:00` 归途提醒、`19:30` 自由活动开放、`20:00` resident 回家、`21:00` 统一隐藏、次日 `07:00` 重新放出。
  4. 确认 README 真正值得高亮的不是“功能列表越多越好”，而是：
     - 多场景 Day1 导演链
     - 农田/放置/工作台/时间/存档/NPC 的真实联动
     - resident crowd 的语义矩阵与夜间合同
     - workbench runtime 持久化与 save blocker 收束
     - 大量 Editor/测试基础设施已围绕这些链路建成
  5. 同时抽出一批应主动纠偏的旧口径：
     - “第一阶段完成 40%”
     - “主系统未完成”
     - “Day1 只做到白天教学”
     - “8 格耕种和砍树教学仍是主线硬门槛”
     - 以及一批只对内部线程有意义的黑话：`0.0.5/0.0.6`、`EP/HP`、`formal/informal`、`resident bridge/beat/cue`。
- 关键判断：
  1. `Sunset` 当前最能代表复杂度的不是孤立系统数量，而是 Day1 这条线已经把剧情导演、NPC crowd、工作台、农田教学、晚饭冲突、自由活动、夜间回家和次日释放串成了一个运行态合同。
  2. 旧需求文档仍有价值，但只能当“目标态与历史意图”；只要它与 `SpringDay1Director.cs`、测试和当前 dialogue asset 冲突，必须以后者为准。
  3. 代办区和旧全局文档已经明显落后于代码演化；后续所有 README / 简历 / 项目介绍如果还直接复述旧总述，极容易把项目说扁或说错。
- 主要证据入口：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_项目内容全清单_V1.md`
  - `D:\Unity\Unity_learning\Sunset\Docx\分类\全局\000_项目全局文档.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\100-重新开始\0418_打包终验包.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_代办清扫\2026.03.06\系统审查报告.md`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Data\StoryPhase.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1MiddayRuntimeBridgeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NpcCrowdManifestSceneDutyTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\StoryProgressPersistenceServiceTests.cs`
- 验证结果：
  - 本轮为只读事实盘点；未改业务代码、未改 Scene、未改 Prefab。
  - 结论建立在 live 代码、测试、dialogue asset 路径、README 草案与工作区文档交叉核对之上。
- 下一步恢复点：
  1. 若继续项目表达线，可直接把这轮四块产物继续拆成：
     - Day1 玩家流程版
     - README 高亮点版
     - 过时说法纠偏版
 2. 若继续简历/项目介绍线，优先从本轮已确认的“多场景 Day1 导演链 + runtime persistence + resident crowd 合同”抽一句话级亮点，而不是再回退到泛功能清单。

## 2026-04-23｜根 README 已完成项目公开首页式二次重写

- 当前主线目标：
  - 用户要求彻底吃透 `Sunset` 项目后，把仓库根 `README.md` 重写成真正能代表项目的公开首页，而不是内部候选稿、黑话稿或简历式摘要。
- 本轮子任务：
  1. 补读 README 相关母文、项目总览、Day1 关键代码与测试。
  2. 明确 README 应前置的项目本体 / 可玩内容 / 价值，以及应后置的 AI / 工具 / 治理内容。
  3. 完成 README 二次重写，并自检黑话残留、首页层级与图片可用性。
- 本轮完成：
  1. 已将根 README 重排为真正的项目首页结构，固定为：
     - 标题与一句话定位
     - 首屏视觉证据
     - 当前阶段说明
     - 项目速览表
     - `如果你只看三件事`
     - 项目简介
     - `Day1 现在能玩到什么`
     - 核心玩法循环
     - 项目当前最值得看的地方
     - 已接起来的系统
     - 开发与验证
     - 我的职责
     - 当前状态 / 打开方式 / 阅读入口
  2. 已把首屏图片从未跟踪的 opening 截图切到已跟踪素材：
     - `Assets/Screenshots/daynight_primary_2026-04-08_early_morning_v3.png`
     以避免 README 在外部仓库页面断图。
  3. 已把 Day1 外部表达从抽象标签细化为 9 拍完整节拍：
     - 矿洞醒来与首遇
     - 跟随进村与围观安置
     - 疗伤与 HP 显形
     - 工作台交代与闪回
     - 五步生存教学
     - 傍晚自由窗
     - 晚饭冲突
     - 归途提醒
     - 夜间自由活动到 `DayEnd`
  4. 已把当前 README 的主语固定回项目本体，而不是协作方式：
     - Day1 已成为跨 `Town -> Primary -> Home` 的连续体验
     - 场景切换后玩家与部分世界状态继续承接
     - 村民会在不同节拍承担不同场面职责
     - 存档已开始承接剧情、关系、生命精力、工作台与离场场景状态
  5. 已把 AI / 工具 / 验证留在后置章节，并统一翻译成人话：
     - 自定义 `Editor` 工具
     - `EditMode Tests`
     - 轻量检查优先、Unity 实机取证兜底
     - AI 协同参与实现，但主线与验收由人工主控
- 当前稳定判断：
  1. 这轮 README 真正修正的不是“句子不够好看”，而是“首页层级混乱”。
  2. 当前 README 已不再像简历稿或治理稿，而更接近 `公开首页 + 阅读地图入口` 的定位。
  3. 对外最值钱的表达，不是功能数量，而是多系统已经被压进同一条 Day1 主线。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-16_Sunset项目表达骨架版_01.md`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Data\StoryPhase.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NpcCrowdManifestSceneDutyTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\StoryProgressPersistenceServiceTests.cs`
- 验证结果：
  - `git diff --check -- README.md`：通过
  - 黑话扫查：未发现 `resident / formal / hooks / steering / thread-state / Begin-Slice / Park-Slice` 等内部术语残留
  - 本轮属于 docs-only 更新：未改业务代码、未改 Scene、未改 Prefab
  - 本轮 thread-state：已执行 `Begin-Slice` 与 `Park-Slice`
- 下一步恢复点：
  1. 若继续 README 线，可进一步补 GIF / 实机短视频或更强的展示素材。
  2. 若切回简历 / 项目介绍线，可直接以当前 README 作为对外项目本体基线，不要再回退到旧的黑话稿。

## 2026-04-23｜README 已补公开演示入口与录屏覆盖说明

- 当前主线目标：
  - 在不改项目实际内容、不处理 release 的前提下，继续补强 `Sunset` 的仓库展示层，把视频演示入口正式接进 README。
- 本轮子任务：
  1. 盘清外部视频素材目录与公开可访问入口。
  2. 判断当前机器是否具备 `gh` / `ffmpeg` / 无安装抽帧能力。
  3. 把“完整实录 + 本地短片覆盖面”整理成 README 可用的人话。
- 本轮完成：
  1. 已确认用户给出的 B 站完整演示入口可直接作为 README 对外公开视频主入口：
     - `https://www.bilibili.com/video/BV16Ed1BAEun/`
  2. 已盘清外部 `mp4` 素材的主要分布：
     - 完整 Day1 压缩实录：`121MB/4月20日.mp4`，约 `2分16秒`
     - 完整 BGM 版大文件：`256MB——BGM/李京涛_个人作品《Sunset》_演示实录视频.mp4`
     - 重点短片：`Primary观光`、`放置演示`、`箱子交互演示`、`存档操作演示`、`工作台交互`
  3. 已确认当前环境里：
     - `gh` 不在命令路径中
     - `ffmpeg` 不在命令路径中
     - 无法低成本走现成 GitHub / 视频处理工具链
  4. 已补入 [README.md](D:/Unity/Unity_learning/Sunset/README.md) 的 `演示与录屏` 章节，内容包括：
     - 完整 B 站实录链接
     - 本地录屏素材按 `主线体验 / 核心交互 / 连续承接` 三类的覆盖说明
- 当前稳定判断：
  1. 这轮最稳的展示层补法，不是给仓库塞大视频文件，而是：
     - 公开页挂完整 B 站视频
     - README 说明本地已有哪几类录屏素材
  2. 在当前没有 `ffmpeg` / `gh` 的环境下，继续折腾抽帧或仓库媒体自动化，收益不如先把公开视频入口讲清楚。
  3. 下一轮如果继续做展示层，优先级应是：
     - 补 GIF / 公开视频短片
     - 而不是继续写更多解释性文字
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\121MB\4月20日.mp4`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\素材\Primary观光.mp4`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\素材\放置演示.mp4`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\素材\箱子交互演示.mp4`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\素材\存档操作演示.mp4`
  - `D:\QQ\Project\综合实训三NEW\刀\000_通用\素材\工作台交互.mp4`
- 验证结果：
  - `git diff --check -- README.md`：通过
  - 本轮为 docs-only 更新：未改业务代码、未改 Scene、未改 Prefab
  - 本轮 thread-state：已补登记 `README媒体入口补充`，并已执行 `Park-Slice`
- 下一步恢复点：
  1. 若继续表面展示线，可优先把 `Primary观光 / 放置 / 箱子 / 存档 / 工作台` 这几段短片挑成 2~3 个正式公开视频或 GIF。
  2. 若切回仓库文本线，当前 README 已具备“项目首页 + 演示入口 + 阅读地图”的完整骨架，不必再回到空骨架阶段。

## 2026-04-23｜README 首页职责与前后置边界只读定稿

- 当前主线目标：
  - 只读对照当前根 `README.md`、`01_总览.md`、`06_工具链.md`、`07_AI治理.md` 与 `2026-04-16_Sunset项目表达骨架版_01.md`，回答根 README 首页到底应该承担什么、哪些内容必须前置、哪些 AI / 治理 / 工具链内容必须后置，以及这些内部词该怎样翻译成人话。
- 本轮完成：
  1. 已确认当前根 `README.md` 的强项是：
     - 项目定位、叙事前提、Day1 三场景主线、核心玩法循环、阶段状态都已出现；
     - 但首页职责与下沉职责仍有混放，导致“项目本体”和“开发方式 / AI 协作”在同一层竞争注意力。
  2. 已用 `项目表达骨架版` 再次钉死当前对外顺序：
     - `先项目本体，后推进方式`
     - 先讲 `Town -> Primary -> Home`、多系统主线承接、连续世界与居民运行；
     - AI 只是推进方式，不是项目主体。
  3. 已用 `01_总览.md` 校准首页必须前置的真实主语：
     - 题材与叙事前提
     - `资源积累 -> 经营生产 -> 能力成长 -> 事件 / 探索解锁 -> 聚落复兴反哺`
     - 当前阶段不再是“系统原型”，而是 `Town -> Primary -> Home` 主链已成、导演尾差与 live 终验待补。
  4. 已用 `06_工具链.md` 校准工具链在 README 里的正确位置：
     - 可作为“为什么这个项目不像普通学生 Demo”的支撑亮点；
     - 但不应在首页首屏展开 `ID 分段 / SO 分类 / Editor 菜单清单 / 工具名大全`。
  5. 已用 `07_AI治理.md` 校准 AI / 治理内容的正确边界：
     - 这些内容只负责解释“项目是怎么被稳住和推进的”；
     - 不能替代业务总览，更不能把 `hooks / steering / memory / thread-state / skill-trigger-log / unityMCP` 直接端到外部首页。
- 当前稳定判断：
  1. 根 README 首页应承担的 6 件事固定为：
     - 这是什么游戏
     - 玩家在 Day1 真能做什么
     - 为什么它不是普通系统拼盘 Demo
     - 当前项目进到哪个阶段
     - 如何最快看到证据或打开项目
     - 去哪里继续深读
  2. 首页必须前置的内容固定为：
     - 一句话定位 + 叙事前提
     - 一张最强截图 / GIF
     - Day1 三场景主线与当前可玩事项
     - 核心玩法闭环
     - 3~5 条“项目价值 / 差异化亮点”
     - 当前状态与最小打开方式
  3. 必须后置到次级章节或阅读地图的内容包括：
     - `.kiro/about` 接手路线
     - AI 协同治理细节
     - CLI / MCP / 验证顺序
     - `Editor` 工具清单、SO 规范、ID 体系
     - `hooks / locks / thread-state / memory / 工作区栈`
  4. 这些后置内容如果要在 README 出现，只能翻译成外部可懂的人话，例如：
     - `多 Agent 并行治理` -> `我以策划主控方式组织 AI 并行开发，并通过分工、回归和终验把范围收住`
     - `CLI first, MCP last-resort` -> `日常验证优先走轻量自动检查，只有需要真实场景证据时才进入 Unity 实机验证`
     - `运行态探针 / 守卫测试` -> `项目内建了定向验证工具，用来快速定位跨场景、NPC、UI 和剧情承接问题`
     - `数据治理 / SO 规范 / ID 体系` -> `项目已经形成批量生产与维护内容资产的工具链，而不是纯手工堆资源`
  5. 当前 README 最该避免的不是“技术内容太多”本身，而是：
     - 过早讲内部流程名；
     - 用黑话替代结果；
     - 把项目写成多个并列模块；
     - 把 AI 写成主角。
- 本轮禁写边界：
  1. 不要在首页首屏直接写：
     - `resident 化`
     - `formal 一次性消费`
     - `runtime 语义`
     - `placement 公共链`
     - `hooks / steering / memory / thread-state`
     - `Begin-Slice / Ready-To-Sync / Park-Slice`
  2. 不要把项目写成：
     - “做了很多系统”
     - “复杂工业化流程”
     - “背包、农田、箱子、工作台、存档五个并列模块”
     - “AI 协同 = 一堆内部工具名”
  3. 不要夸大成：
     - “完整通用全世界态持久化”
     - “Day1 只剩一点收尾”
     - “README 已经能兼顾首页、接手手册和治理入口”
- 验证结果：
  - 本轮为只读分析：未改代码、未改 Scene、未改 Prefab、未改 README 正文。
- 下一步恢复点：
  1. 若继续本线，可直接把根 README 重写成：
     - 首页首屏
     - 可玩内容 / 核心闭环
     - 差异化亮点
     - 当前状态 / 打开方式
     - 深读入口
     - AI / 工具链补充说明
  2. 若暂不施工，后续凡是再讨论 README 首页结构，应默认沿用这轮“首页职责 / 必须前置 / 必须后置 / 人话翻译 / 禁写边界”五件套，不再回到泛分析。

## 2026-04-23｜Day1 可玩主线节拍与 README 对外表达只读深挖

- 用户目标：
  - 只读分析 `StoryPhase.cs`、`SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs` 与 3 份相关测试，回答当前项目的 Day1 可玩主线到底由哪些具体节拍组成、每拍接入了哪些系统、完成了什么教学/叙事/状态承接，以及哪些点最适合写进仓库 README。
- 本轮完成：
  1. 已把 Day1 主线稳定拆成 9 个外部可懂节拍：
     - 矿洞醒来与首遇
     - 跟随进村与围观安置
     - 疗伤与 HP 显形
     - 工作台交代与闪回
     - 五步生存教学
     - 白天收口后的傍晚自由窗
     - 晚饭冲突
     - 归途提醒
     - 夜间自由活动到睡觉收束
  2. 已确认白天教学的硬目标不是泛泛“体验农田”，而是 5 步最小闭环：
     - 开垦 1 格
     - 播种 1 次
     - 浇水 1 次
     - 收集 3 份木材
     - 完成 1 次基础制作
  3. 已确认状态承接的关键时间点：
     - 晚饭入口最早夹到 `18:00`
     - 归途提醒最早夹到 `19:00`
     - 自由时段最早夹到 `19:30`
     - 夜间压力提示分别在 `22:00`、`24:00`、`25:00`
     - `26:00` 会强制睡觉并收束到 `DayEnd`
  4. 已确认 Day1 不只是剧情串场，还把以下系统绑进主线：
     - `Town -> Primary -> Home` 三场景承接
     - 对话导演与提示卡
     - HP / Energy 显示与低精力减速
     - 工作台 / 制作 UI
     - 农田放置与资源采集
     - 床交互与夜间收束
     - 7 名村民 crowd 的分层调度
  5. 已确认 crowd 不是随机背景：
     - 资源里当前有 7 名正式 Day1 村民
     - 进村、晚饭、夜间见闻三拍会在 `前台 / 背景 / 离屏压力 / 痕迹` 之间换位
     - README 可对外写成“村庄会随剧情重排视线与气氛”，不必暴露内部 beat key。
- 当前稳定判断：
  1. README 最值得写的 Day1 说明，不是系统名清单，而是一句完整体验链：
     - `被救起 -> 被带进村 -> 被治疗 -> 学会最基本的谋生手艺 -> 用一轮干活证明自己 -> 经历晚饭上的敌意 -> 在夜里决定现在睡还是再看看这个村子`
  2. 最适合对外高亮的 5 个卖点是：
     - Day1 已有完整可收束的一天，而不是零散 Demo 房间
     - 三场景会随剧情连续切换，不是互相孤立的地图
     - 新手教学直接绑在叙事里，不是独立教程关
     - 晚饭与夜晚不是空档，而是会继续推进人物关系和村庄态度
     - 村民 crowd 会按剧情重排站位、围观、背景动作和夜间目击
  3. 对外应避免的黑话包括：
     - `beat key`
     - `director consumption role`
     - `semantic anchor`
     - `resident baseline`
     - `formal sequence`
- 验证结果：
  - 本轮为只读分析；未改代码、未改 Scene、未改 Prefab、未改 README。
- 下一步恢复点：
  1. 若继续 README 线，可直接把这轮结论改写成“Day1 玩家会经历什么”的首屏段落。
  2. 若继续只读，可再下钻成两份素材：
     - `README 一段版`
     - `README 表格式节拍版`

## 2026-04-23｜README 展示层再次增强：首图直链与截图画面补强

- 当前主线目标：
  - 在不改项目实际内容、不碰 release 的前提下，继续把 `Sunset` 的仓库首页往“更像作品页”的方向推进。
- 本轮子任务：
  1. 回收并关闭当前等待中的 3 个子线程结果。
  2. 基于仓库已跟踪截图和现有公开视频入口，继续增强 [README.md](D:/Unity/Unity_learning/Sunset/README.md) 的展示力。
  3. 收口后补做 `git diff --check` 与 `gh` 可用性核查。
- 本轮完成：
  1. 已回收 3 个子线程结论，并确认：
     - 演示主入口继续以 B 站完整实录为主；
     - 当前最稳的短片方向仍是放置 / 箱子 / 存档承接；
     - 无安装条件下不值得硬搞精确抽帧。
  2. 已把 README 首屏大图改成可点击跳转的公开视频入口：
     - 首图现在直接链接到 `https://www.bilibili.com/video/BV16Ed1BAEun/`
  3. 已新增 `画面一瞥` 章节，并只使用仓库已跟踪截图：
     - `Assets/Screenshots/spring-day1-opening-dialogue-start.png`
     - `Assets/Screenshots/screenshot-20260407-125700.png`
  4. 已继续精炼 `演示与录屏` 章节，把本地素材表达收成：
     - 完整主线体验
     - `Primary` 观光 / 放置 / 箱子 / 工作台的核心交互短片
     - 存档操作与重新开始的连续承接短片
  5. 已确认当前环境下：
     - `git diff --check -- README.md` 通过
     - `gh` 仍不在 `PATH`
- 当前稳定判断：
  1. 这轮补的不是更多说明文字，而是 README 的“首屏触达”和“第二屏视觉证据”。
  2. README 现在更像“项目首页 + 视频入口 + 画面补图”，而不只是文档说明页。
  3. 下一轮如果继续做表面展示，最值钱的仍然是 GIF / 短片，而不是再继续扩文字。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\memory_0.md`
- 验证结果：
  - 本轮为 docs-only 更新：未改业务代码、未改 Scene、未改 Prefab
  - `git diff --check -- README.md`：通过
  - `gh --version`：失败，当前命令不可用
  - 本轮 thread-state：已执行 `Begin-Slice` 与 `Park-Slice`
- 下一步恢复点：
  1. 若用户继续 README / 展示层，优先把现有短片整理成 2~3 个正式公开短视频或 GIF。
  2. 若用户先停在这里，当前 README 已具备“首页主图可直达视频 + 截图补强 + 演示入口说明”的更完整公开页形态。

## 2026-04-23｜README 公开页重写完成，并纠正了截图方向判断

- 当前主线目标：
  - 把 `Sunset` 根 `README.md` 真正改成面向外部展示的作品页文本，只讲游戏和项目本体；同时把用户指出“截图很尴尬”的问题一起处理掉，而不是继续拿开发留证图硬凑版面。
- 本轮子任务：
  1. 基于 `04_剧情NPC.md`、`02_经营成长.md`、`spring-day1 requirements` 与箱子系统需求，重写 README 的主叙事、玩家行为和后续方向。
  2. 把 AI / 治理 / 内部系统说明从 opening 大幅压后，不再抢项目主语。
  3. 重审 README 用图，确认原仓库截图为什么尴尬，并补做更适合公开页的图源。
- 本轮完成：
  1. 已把根 README 改成更接近作品页的结构：
     - 一句话定位
     - 当前可玩内容
     - `Day1 现在能玩到什么`
     - `你会在 Sunset 里做什么`
     - `为什么工作台、种地、晚饭和记忆会连在一起`
     - `这个世界接下来会往哪里长`
  2. 已明确把 README 的 Day1 主叙事固定为：
     - `先活下来 -> 被带进村 -> 被治疗 -> 重新摸到工作台 -> 用第一轮劳动证明自己 -> 经历晚饭冲突 -> 在夜里决定现在睡还是再看看这个村子`
  3. 已根据用户对截图的反馈，否决原先那批“开发留证图”作为公开页主图的方案。
  4. 已找到本机可用的 `ffmpeg`，从主演示视频抽帧，再手工裁掉字幕、快捷键面板、底栏和调试信息，生成两张更适合 README 的展示图：
     - `D:\Unity\Unity_learning\Sunset\.github\readme\hero_day1_labor.png`
     - `D:\Unity\Unity_learning\Sunset\.github\readme\day1_arrival.png`
  5. 已把 README 展示素材落到 `.github/readme/`，而不是继续塞进 `Assets/`，避免只为公开页图片引出 Unity `.meta` 脏改。
- 当前稳定判断：
  1. 这轮最有价值的不是“多写了一版 README”，而是把 README 的主语重新钉回了游戏本体。
  2. 这轮第二个关键判断是：作品页图片不能拿开发留证图硬顶；如果现成图不对，就应该明确承认，再从公开视频里抽出真正能讲 Day1 的画面。
  3. 当前 README 终于更像“这是一款什么游戏、玩家会经历什么”，而不是“项目做了哪些系统”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\README.md`
  - `D:\Unity\Unity_learning\Sunset\.github\readme\hero_day1_labor.png`
  - `D:\Unity\Unity_learning\Sunset\.github\readme\day1_arrival.png`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
- 验证结果：
  - `git diff --check -- README.md .github/readme/hero_day1_labor.png .github/readme/day1_arrival.png`：通过
  - 本轮为 docs-only / media-only 更新：未改代码、未改 Scene、未改 Prefab
  - thread-state：已执行 `Begin-Slice` 与 `Park-Slice`
- 下一步恢复点：
  1. 若继续 README 线，最值钱的下一刀不是再扩文字，而是继续补更强的成品级画面或短 GIF。
  2. 若切回简历 / 项目介绍线，当前 README 已可作为最稳的对外项目口径继续抽句。

## 2026-04-23｜网申页项目描述加急版已产出

- 当前主线目标：
  - 用户临时切回简历投递网页，要求先做“项目/活动经历”里的项目名称与项目描述成稿，而且明确不要把简历项目经历原样抄过去，要改成网页表单里的项目简介口径。
- 本轮子任务：
  1. 基于当前 README、持续策划案和简历沉淀，重写更适合网申页填写的项目简介。
  2. 保留项目气质和完成度，但避免写成项目经历 bullets 拼接稿。
  3. 同时给出一版推荐直填稿和一版更短备选稿。
- 本轮完成：
  1. 已形成推荐项目名称口径：
     - `个人项目《Sunset》｜2D像素奇幻生活模拟RPG`
  2. 已形成推荐项目描述主稿，核心表达为：
     - 以“失忆工匠在残破聚落中通过劳动、制作与关系建立逐步留下来”为叙事核心；
     - 当前已完成 Day1 可玩主线；
     - 强调自己主导项目定位、系统设计、玩法原型、规则收口与版本推进；
     - 点出剧情、采集耕种、工作台、背包/箱子、时间季节、存档与跨场景承接已接成连续体验。
  3. 已额外准备更短版，方便用户在网页字数受限时直接替换。
- 当前稳定判断：
  1. 网申页里的“项目描述”不该照搬简历 bullets，也不该写成 README opening。
  2. 最稳的写法是：`项目是什么 -> 玩家会经历什么 -> 我负责什么 -> 当前做到了哪一步`。
  3. AI / 治理 / workflow 不该抢这个字段的主语，除非岗位页面专门问协作方式。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\memory_0.md`
- 验证结果：
  - 本轮为文本产出与口径整理：未改代码、未改 Scene、未改 Prefab、未改公开仓库文件。
- 下一步恢复点：
  1. 若用户继续这条线，下一刀可再针对具体厂商网申字段长度做定制压缩。
  2. 若用户切回 README / release / 仓库展示，则继续沿当前公开展示口径推进。
