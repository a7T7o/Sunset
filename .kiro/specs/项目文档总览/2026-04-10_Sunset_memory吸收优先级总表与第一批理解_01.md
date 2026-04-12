# 2026-04-10｜Sunset memory 吸收优先级总表与第一批理解

## 1. 这份文档的用途

这不是新的 survey，也不是分卷整改方案。

它的唯一作用是把 `项目文档总览` 这条线已经确认要吃进脑子的高价值 memory，先按“当前项目理解价值”排出真正优先级，并把第一批已经读穿的结论写成后续总览、主文、进度表和 AI 卷都能直接复用的稳定口径。

这里的“优先级”不是按文件行数排序，而是按下面 3 条综合判断：

1. 这条 memory 是否直接影响对《Sunset》当前真实推进状态的理解。
2. 这条 memory 是否承载了近期真正改变项目推进方式的决策、contract 或 runtime 结论。
3. 这条 memory 吃进去之后，是否能反过来帮助 `项目文档总览` 少走弯路、少写假话、少把旧结论当现状。

---

## 2. 当前吸收优先级总表

### 2.1 业务理解优先级

| 优先级 | 对象 | 当前角色 | 为什么优先 |
| --- | --- | --- | --- |
| P0 | `.codex/threads/Sunset/spring-day1/memory_0.md` | day1 主控线最新真相 | 它直接决定当前主线剧情、跨场景推进、导演尾账和 runtime 终验口径，是理解项目现状的第一入口。 |
| P0 | `.kiro/specs/900_开篇/spring-day1-implementation/memory.md` + `003-进一步搭建/memory.md` | day1 父工作区 / 子阶段摘要 | 它补的是“这条线为什么从对白验证演进成 scene/runtime 收口”，能避免把旧 blocker 当当前 blocker。 |
| P1 | `.codex/threads/Sunset/UI/memory_0.md` | 玩家面集成与 UI 体验线 | 它现在不只是做 UI 壳，而是在收 workbench / prompt / proximity / task-list / packaged 字体 / live 体验证据。 |
| P1 | `.codex/threads/Sunset/NPC/memory_0.md` | resident 化、人物真源、自漫游线 | 它承载了 resident 化、`NPC_Hand` 真源、关系页结构、avoidance 恢复与障碍自动收集等近期重要事实。 |
| P1 | `.codex/threads/Sunset/导航检查/memory_0.md` | traversal / 跨场景桥接 / runtime 根因线 | 它已经把多轮“桥坏、水坏、卡顿、绕行失效”从泛导航问题压实为 scene contract、persistent player 重绑、driver 过重三类问题。 |
| P1 | `.codex/threads/Sunset/存档系统/memory_0.md` | 三场景持久化与运行链补完线 | 它直接决定 `Town / Primary / Home` 是否形成统一持久化基线，也承载 workbench 刷新、Home seed 化、runtime-first 资产解析等关键落地。 |

### 2.2 治理理解优先级

| 优先级 | 对象 | 当前角色 | 为什么优先 |
| --- | --- | --- | --- |
| G1 | `.kiro/specs/Codex规则落地/memory.md` | 安全治理 / shared-root / runbook / thread-state 总闸 | 这条线重要，但它的职责应被严格理解为治理层，而不是业务功能层；需要吃进去的是当前 live 口径，不是把它误当总业务工作区。 |
| G2 | `.codex/threads/Sunset/项目文档总览/memory_0.md` | 总览线程自身恢复层 | 这是最后一层自我同步，不该反过来主导业务事实，只负责把前面吸收过的结论稳定留住。 |

### 2.3 跨项目补充优先级

| 优先级 | 对象 | 当前角色 | 说明 |
| --- | --- | --- | --- |
| X1 | `D:\迅雷下载\开始\.codex\threads\系统全局\对话丢失修复\memory_0.md` | “开始”项目里最重的历史治理 / 修复线 | 当前只完成现场勘察，尚未进入这轮正文吸收。 |
| X2 | `D:\迅雷下载\开始\.codex\threads\系统全局\谁是卧底V2\memory_0.md` | 玩法与线程连续性样本 | 适合作为后续 cross-project AI 使用方式补充。 |
| X2 | `D:\迅雷下载\开始\.codex\threads\系统全局\全局skills\memory_0.md` | skills 治理样本 | 当前不进入 Sunset 第一批业务理解正文。 |

---

## 3. 第一批已吸收结论

## 3.1 `spring-day1` 现在的真实状态

`day1` 现在已经不能再被描述成“主线还没搭起来”。

更准确的口径是：

1. `005` 后主链已经被主控改成：
   - `Primary` 村长收口对白
   - 傍晚自由活动窗口
   - `Town` 可主动找村长提前开晚饭
   - `19:00` 自动切晚饭
   - 晚饭 crowd 复用 `EnterVillageCrowdRoot`
2. `0.0.5` 农田教学收口的真正 blocker 已经被压实并分层修过：
   - formal 入口原先没吃到 override，现已补到 `NPCDialogueInteractable`
   - `001/002` 在收口完成后仍停在 story actor 锁态，现已补自愈刷新
   - 自由活动黑屏切入把对白锁态错误继承给 world input，现已改成对白真正结束后再进入 explore window
3. 当前 `day1` 最值钱的风险，不再是“逻辑还没写”，而是：
   - 导演尾账还没彻底吃平
   - contract 虽然外围已经给出很多，但主控还要继续吞回
   - runtime 黑盒终验仍缺最后一轮稳定票
4. `farm` 的 placement 问题已经被 `day1` 主控明确切出，不应再混成剧情禁用语义问题。

一句话总结：`day1` 当前已经进入“主线已成、导演尾差待收、runtime 终验待补”的阶段。

## 3.2 `900_开篇/spring-day1-implementation` 的价值不在补新事实，而在校正阶段理解

父工作区和 `003` 子工作区最关键的价值，不是提供另一份重复流水账，而是帮助 `项目文档总览` 校正时间层级：

1. 这条线已经从最早的对白验证，正式演进成 scene-side / runtime-side 收口。
2. `DialogueValidation` 一类早期 blocker 已不再是当前主风险。
3. 当前值得继续关注的是：
   - chief wrap formal 入口补口
   - `001/002` 锁态释放
   - explore window 输入恢复
   - 等待气泡从 ambient 提升到 conversation 并保持续命
   - `FarmingTutorial` 不应再被误解释为“剧情禁止播种”

这意味着后续总览如果再写 `spring-day1`，必须把重点落到“runtime 收口”和“场景承接”，不能回到旧式验证口吻。

## 3.3 `UI` 线程已经从“做界面”转成“玩家面集成与性能止血主刀”

这条线现在的真实重心有 4 个：

1. `Workbench / PromptOverlay / ProximityService` 的高频刷新风暴已经做了第一刀止血。
2. workbench 这边做过玩家面文案回正，不再直接把内部名、开发口吻描述、材料前缀原样抛给玩家。
3. 这条线吸收了一个非常关键的用户硬约束：
   - 性能优化不能牺牲功能和需求。
4. 当前 `UI` 真正还没收完的是：
   - packaged/live 最终体验证据
   - task-list / prompt / modal / formal dialogue 之间的最终玩家面表现
   - 部分页面的最终 live 终验，而不是再回去做泛型 UI 重构

所以 `UI` 不应再被写成“做了一些界面和面板”，而应被理解成“玩家面整合、提示治理、工作台体验和刷新性能止血的当前 active 刀口”。

## 3.4 `NPC` 线程的核心成果已经从“角色能出现”升级为“resident 化 + 真源统一 + 漫游行为纠偏”

第一批吸收后，这条线最值得保留的口径是：

1. `NPC_Hand` 已被推进为 hand portrait 真源目录，`NpcCharacterRegistry` 已转成 runtime 字典缓存 + editor 自动同步，不再是临时手工挂图。
2. 玩家 / 旁白头像已经统一切到 `000` 特殊条目，剧情头像链更明确了。
3. 关系页 / 人物简介这条线，已经先把信息架构和边界梳理出来，而不是乱写文案。
4. 自漫游线近期最重要的事实有两层：
   - `TraversalBlockManager2D` 已补场景静态阻挡自动收集与关键词过滤
   - `NPCAutoRoamController / NPCMotionController` 已把一部分“避让恢复”和“方向稳定误伤”纠回来
5. 当前这条线不是没做完，而是结构已经站住，差 live 终验和与 `UI/day1` 的最终并面验证。

所以 `NPC` 现在的正确定位，不再是“写了几个 NPC 功能”，而是“resident 化、人物真源、关系页结构、自漫游响应”四条线一起推进。

## 3.5 `导航检查` 线已经把问题从泛导航 bug 压缩成 3 个真正的责任层

这条线当前最该记住的不是单个修补，而是它已经完成了问题分层：

1. scene contract 层：
   - `Town` 农田挡路更像运行时边界 tile 被 traversal 自动补收越权吃掉
   - `Primary` 桥/水并非静态没配，场景落盘本身是有桥配置的
2. runtime bridge 层：
   - 跨场景桥面失效的真因已改判为 persistent player 重绑时序
   - `PersistentPlayerSceneBridge + TraversalBlockManager2D.BindRuntimeSceneReferences(...)` 已是最小修补
3. driver 层：
   - shared traversal kernel 方向仍应保留
   - `NPCAutoRoamController.TickMoving()` 太重
   - `NPCMotionController` 朝向容易被 avoidance 抖动污染

再往前走，这条线应该继续遵守“止血刀 / 全修刀”两段式，而不是把所有问题都打成“导航全推倒重来”。

## 3.6 `存档系统` 线程最重要的改判，是它早就不是“80% 没做完”，而是“旧底座有了，但要跟上项目扩张”

第一批吸收后，这条线至少要明确 6 个事实：

1. `Home` 不是抽象规划，而是已经被 seed 化并补进三场景持久化基线。
2. `PersistentPlayerSceneBridge` 已经在补：
   - `Systems / UI / Inventory / Equipment / EventSystem / DialogueCanvas` 等 persistent 运行根
   - `PackagePanelTabsUI / InventoryPanelUI / BoxPanelUI` 这条 persistent UI 断链
3. `AssetLocator` 已经转向 runtime-first，不再把 build 可用性偷偷寄托在 Editor-only `AssetDatabase`。
4. `Primary / Town / Home` 三场景的持久化主链已经真正落过一版，不应再写成“还在讨论该不该持久化”。
5. workbench 从箱子拖材料进背包后不刷新的问题，已经补到底层数据通知合同和 overlay 兜底订阅，并配了 `WorkbenchInventoryRefreshContractTests`。
6. 当前这条线剩的主要是：
   - live 门链终验
   - 个别 UI / scene-side 体验尾差
   - 而不是三场景持久化底座完全不存在

所以 `存档系统` 在总览里的定位，应该是“三场景 persistent baseline 已落地，当前进入 runtime/live 收口”，而不是“长期未完成的大坑”。

## 3.7 `Codex规则落地` 这条 memory 的正确角色必须重新钉住

这条线当然很重，但它的重，不等于它应该吞掉整个项目理解。

当前第一批吸收后，应固定以下认识：

1. 它的职责是治理层、安全层、shared-root 事故恢复层、dispatch/runbook/thread-state 规范层。
2. 它最值钱的近期变化包括：
   - `CLI first, direct MCP last-resort`
   - `thread-state` 进入 live 规则
   - 主文工程化整改意识
   - 用户本周对跨场景推进、resident 化、persistent player、UI 结构、Home 镜头、no-red 与停发判断的统筹被正式吸收
3. 但它不应该继续被误用为“业务事实的大总表”，否则总览很容易写成治理现场总结，而不是项目真实内容。

所以后续凡是引用这条线，都应优先提炼“当前 live 口径改变了什么”，而不是把治理线语言直接搬进长期主文。

---

## 4. 对 `项目文档总览` 后续更新的直接影响

基于这一批吸收，后续总览、主文和相关母卷至少要统一 4 个口径：

1. 《Sunset》当前主风险不是“什么都没做”，而是“多条核心线都已有真 contract，但导演尾账、placement 公共链、live 终验还没收平”。
2. `day1 / UI / NPC / 导航 / 存档` 这几条线都不能再写成泛功能清单，而要写成“已经搭到哪一层、当前剩哪一层”。
3. `Codex规则落地` 要继续读，但只能当治理层依据，不能反向压扁业务总览文风。
4. `项目文档总览` 自身接下来最该补的不是再做一轮 survey，而是把这些已吸收结论继续回写进：
   - 总览主文
   - 进度总表
   - AI 治理卷
   - 与剧情/NPC/存档相关的长期主文

---

## 5. 第二批待吸收对象

当前第一批之后，下一轮值得继续吃的对象应优先是：

1. `项目文档总览` 现有主文中仍沿用旧阶段口径的卷。
2. `Docx/大总结/Sunset_持续策划案/` 下与当前 live 事实明显脱节的章节，尤其是 AI 治理、进度和剧情/NPC 卷。
3. `D:\迅雷下载\开始` 项目里的 `对话丢失修复 / 谁是卧底V2 / 全局skills`，用于补 cross-project AI 使用方式与 memory 治理对照样本。

当前不应该做的事：

1. 现在就机械拆旧卷。
2. 重新发明一套和现有 memory 脱节的新总览。
3. 把治理线现场话术原样抄进长期主文。
