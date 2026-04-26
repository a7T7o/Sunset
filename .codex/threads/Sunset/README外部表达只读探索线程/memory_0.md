# README外部表达只读探索线程

## 2026-04-23｜README 首页职责与禁写边界只读分析

- 当前主线目标：
  - 作为 `Sunset README 外部表达` 的只读探索线程，只读分析当前 `README.md` 与项目主文，回答仓库首页应该承担哪些信息职责、哪些内容必须前置、哪些 AI / 治理 / 工具链内容应该后置，以及这些内部词该如何翻译成人话。
- 本轮子任务：
  - 对照以下 5 份核心材料，抽出可直接用于 README 重写的结构结论和禁用表达清单：
    - `D:\Unity\Unity_learning\Sunset\README.md`
    - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\01_总览.md`
    - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\06_工具链.md`
    - `D:\Unity\Unity_learning\Sunset\Docx\大总结\Sunset_持续策划案\07_AI治理.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\项目文档总览\2026-04-16_Sunset项目表达骨架版_01.md`
- 本轮完成：
  1. 已按 Sunset 前置核查完成：
     - `skills-governor`
     - `sunset-workspace-router`
     - `preference-preflight-gate` 手工等价流程
     - 本轮只读，未跑 `Begin-Slice`
  2. 已形成稳定结论：
     - README 首页的核心职责是“先让外部人一眼看懂项目本体和当前阶段”，不是把接手文档、治理机制和内部流程一次性铺满。
     - 当前最佳顺序固定为：
       1. 项目定位与叙事前提
       2. Day1 可玩主线与当前可玩内容
       3. 核心玩法闭环
       4. 为什么它不是普通 Demo
       5. 当前状态与最小打开方式
       6. 深读入口
       7. AI / 工具链补充说明
     - `AI / 工具链 / 治理` 必须后置，只能作为“为什么项目推进得住、为什么验证和内容生产效率高”的差异化补充，不能抢首屏 opening。
  3. 已稳定列出禁写边界：
     - 黑话：`resident 化`、`formal 一次性消费`、`runtime 语义`、`placement 公共链`
     - 内部机制名：`hooks / steering / memory / thread-state / Begin-Slice / Ready-To-Sync / Park-Slice`
     - 漂移表达：“Day1 只剩收尾”“完整通用全世界态持久化”“五个并列系统”
     - 自嗨表达：“做了很多系统”“复杂工业化流程”“效率提升很多”
- 关键决策：
  1. 对外 README 的主语必须始终是：
     - `Town -> Primary -> Home` 的 Day1 连续体验链
     - 多系统主线承接
     - 连续世界与居民运行
  2. AI 的正确翻译方向是：
     - “我以策划主控方式组织 AI 并行开发，并用分工、回归和终验把项目收住”
     - 不是内部治理名词堆叠
  3. 工具链的正确翻译方向是：
     - “项目内建了批量生产、配置和定向验证工具，让内容扩张和问题定位更稳定”
     - 不是 `Tool_Batch* / ValidationMenu / unityMCP` 清单
- 验证结果：
  - 本轮为只读分析：未改代码、未改 Scene、未改 Prefab、未改 README 正文。
- 当前恢复点：
  1. 若用户下一轮要求真实施工，可直接按“首页职责 / 前置内容 / 后置内容 / 人话翻译 / 禁写清单”五件套重写根 `README.md`。
  2. 若继续只读，下一轮最值得补的是：
     - 把这轮结构结论压成一版真正可落笔的 README 章节草图和标题级提纲。
