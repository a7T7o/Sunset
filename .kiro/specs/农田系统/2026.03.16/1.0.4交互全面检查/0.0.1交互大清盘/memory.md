# 0.0.1 交互大清盘 - 工作区记忆

## 2026-03-27：详细落地任务清单已落盘，`0.0.1` 正式成为后续施工总标准

**用户目标**：
- 用户把“大清盘”总账移入 `0.0.1交互大清盘` 目录后，要求我不要先写代码，而是先补一份彻底详尽、可直接落地的正式任务清单。

**当前主线目标**：
- 把“交互大清盘”从根因总账推进成正式施工标准，后续整条线统一按这份任务书施工和验收。

**本轮已完成事项**：
1. 已新增 `2026-03-27-交互大清盘_详细落地任务清单.md`。
2. 已把后续执行顺序正式固定为：
   - `A1` 树苗连续放置事务
   - `A2` 树木倒下事务
   - `A3` 工具失效强制收尾事务
   - `B1~B5` 交互一致性与体验统一
   - `C1~C3` 回归观察与兜底
   - `D` 最终验收与交付
3. 已把 runner pass、`git diff --check`、preflight 通过等信号正式降级为“可以继续推进”，不再允许单独冒充完工。

**恢复点 / 下一步**：
- 从这一刻起，`0.0.1` 已经不是讨论稿，而是农田 `1.0.4` 后续真实施工标准；
- 下一步如果继续，就必须按 `A1 -> A2 -> A3 -> B -> C -> D` 的顺序推进。

## 2026-03-27：`A1~B5` 代码闭环、自验与最终验收手册已补齐，但当前 git 白名单收口仍被同根 foreign dirty 阻断

**用户目标**：
- 用户要求我先提交之前做好的文档内容，然后直接按 `2026-03-27-交互大清盘_详细落地任务清单.md` 一条龙推进，不追求速度，只追求高质量和彻底收口。

**当前主线目标**：
- 不再停在 docs-only，而是把任务书里的 `A1 / A2 / A3 / B1 / B2 / B3 / B4 / B5` 主体真正落到代码、自验、验收包和记忆链里，再诚实评估是否能安全做 git 收口。

**本轮已完成事项**：
1. 已确认前一轮 docs-only 成果已提交到 `92bf811f`（`2026.03.27_农田交互修复V3_02`），本轮继续施工时 shared root 当前基线为 `main @ ee7ba4c1540e6cddb8c398f0762b36eb50c61516`。
2. 已把任务书中的主体实现落到 8 个 owned C# 文件：
   - `PlacementManager.cs`：连续放置 preview 刷新改成按世界候选格重判，补 `ResumePreviewAfterSuccessfulPlacement()`、`ShouldHoldPreviewAtLastPlacement(...)` 与 `IsSamePlacementCandidate(...)`，收 sapling ghost 和“鼠标不动不刷新”。
   - `TreeController.cs`：补单向死亡事务，倒下期间锁死再次命中，并补高树不足等级时的动作前拦截辅助判断。
   - `PlayerInteraction.cs`：改为动作前检查、动作完成后提交，并在提交导致工具移除时统一强制收尾。
   - `FarmToolPreview.cs`：hover 遮挡只上报中心格单格 bounds。
   - `GameInputManager.cs + PlacementManager.cs + ChestController.cs`：补“放置模式下右键可开箱且不开掉当前放置模式”，并把箱子交互距离收口。
   - `InventoryPanelUI.cs`：热槽选中态改回单一事实源。
   - `GameInputManager.cs + TreeController.cs`：低级斧头砍高树走输入层前置拦截。
   - `PlayerThoughtBubblePresenter.cs`：玩家气泡样式强制回到 NPC 同一语言。
3. 已再次完成最小代码自验：
   - `git diff --check` 对上述 8 个 C# 文件通过
   - `CodexCodeGuard` 对上述 8 个 C# 文件通过
   - 程序集检查结果为 `Assembly-CSharp`
4. 已补齐最终用户验收文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_最终验收手册.md`
5. 已把 `C1 / C2 / C3` 观察项并入最终验收手册，不再散落在聊天里。
6. 已重新核对 Unity live 现场：
   - `mcpforunity://instances -> instance_count = 0`
   - `mcpforunity://editor/state -> reason = no_unity_session`
   - 因此当前不能把任何代码闭环外推成新的 live 通过证据。
7. 已尝试按 stable launcher 做正式 `git-safe-sync preflight`，当前阻塞已被压实为：
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
   - 它位于当前白名单命中的 `Assets/YYY_Scripts/Controller` 同根 foreign dirty 中，导致 `own roots remaining dirty 数量 = 1`
   - 因此当前不能诚实 claim “白名单可直接 sync”

**关键决策**：
- 本轮确实按用户授权重开了 `GameInputManager.cs`，但仍未碰 `Primary.unity`、`ProjectSettings/TagManager.asset` 或其他 shared-root foreign dirty。
- 当前线程能 claim 的只有“代码层与验收包已闭环”，不能 claim “live 已过线”或 “git 已安全收口”。
- 当前 git 收口 blocker 不是 owned 代码还红，而是同根 foreign dirty `NPCAutoRoamController.cs` 仍在。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_最终验收手册.md`

**验证结果**：
- `git diff --check`：通过
- `CodexCodeGuard`：通过
- Unity live：不可用，原因是 `instance_count=0 / no_unity_session`
- `git-safe-sync preflight`：未通过，阻塞点为同根 foreign dirty `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

**恢复点 / 下一步**：
- 当前 `0.0.1` 已完成从“任务书”到“代码闭环 + 验收手册 + 记忆链”的升级；
- 下一步分两条并行口径：
  - 用户侧：按最终验收手册做人工终验
  - Git 侧：等待 `NPCAutoRoamController.cs` 同根 foreign dirty 清理或完成 owner 协调后，再继续白名单 sync

## 2026-03-27：用户要求改成聊天内可直接填写的回执清单模板

**用户目标**：
- 用户认可最终验收手册的结构，但明确要求我再提供一份“专门写给我自己看的回执清单模板”，直接输出在对话框，不再额外写文档。

**当前主线目标**：
- 保持 `0.0.1` 当前“代码闭环 + 用户终验”状态不变；
- 本轮子任务仅补一份聊天内可直接复制填写的验收回执模板，方便用户逐项回填结果。

**本轮已完成事项**：
1. 已决定不新增文档文件。
2. 已按现有 `A1 / A2 / A3 / B1 / B2 / B3 / B4 / B5 / C1 / C2 / C3` 验收范围，准备输出聊天内模板。

**恢复点 / 下一步**：
- 下一步由用户直接按聊天模板回填各项是否通过、实际现象和差异；
- 收到回执后，再按模板逐项判定哪些已过线、哪些仍需返工。

## 2026-03-27：用户验收回执后二次返工已补到代码层，当前直改项完成、自验恢复 compile-clean，但仍未进入新的 Unity live 与 safe sync

**用户目标**：
- 用户在验收回执里明确要求：不要再分什么优先级，能直接改的就继续彻底改掉；他说过暂时不要直接改的项，只输出他要的分析与方案。
- 当前真实边界已经固定为：
  - 直接落地：`A1 / A2(仅特效层) / A3 / B1 / B2 / B5`
  - 只给分析 / 方案：`B3 / B4`
  - 聊天里要用大白话解释：`C1 / C2 / C3`

**当前主线目标**：
- 继续完成 `0.0.1交互大清盘` 的返工收口，不回头扩到别的系统，也不把这轮再漂移成 runner / placeable 新主线。

**本轮已完成事项**：
1. 已继续补完 `A1` 树苗连续放置手感收口：
   - `PlacementManager.cs` 新增“已占格边缘意图偏向”的候选格重判；
   - preview 刷新与点击判定重新统一到同一套候选格来源；
   - 刚种下一棵后，不再必须依赖鼠标像素位移才刷新下一格。
2. 已继续补完 `A3` 工具失效强制收尾：
   - `PlayerInteraction.cs` 继续把“动作前前检 / 动作完成后提交 / 提交导致工具移除时强制收尾”收实；
   - `GameInputManager.cs` 新增农具自动链的“尾巴中断”和“立即彻底中断”两套清理入口；
   - 锄头 / 水壶 / 清作物三类农具动作在前检失败时，都会把 queue preview、导航、锁和执行态一起清掉。
3. 已继续补完 `B1 / B2 / B5`：
   - `FarmToolPreview.cs` 维持中心焦点遮挡口径；
   - `ChestController.cs + GameInputManager.cs` 现在按箱子碰撞体 bounds 判点，并把开启距离收回到更近口径；
   - `PlayerThoughtBubblePresenter.cs` 移除了硬换行，并把文本宽度计算改成按自然文本宽度再限宽，不再提前把短句挤成怪异折行。
4. 已按用户允许范围补完 `A2` 的表现层优化，但没有重写树木倒下判定事务：
   - `TreeController.cs` 仅在 `FallAnimationCoroutine(...)` / `FallAndDestroyCoroutine(...)` 内补了预备反压、主摔、落地回弹、压扁和更晚淡出；
   - 没有碰掉落、经验、状态切换、树桩生成和死亡单向锁的业务口径。
5. 已重新完成最小 no-red 自验：
   - `git diff --check` 对当前 8 个 owned C# 文件通过；
   - `CodexCodeGuard` 对同 8 个文件通过，程序集为 `Assembly-CSharp`，`Diagnostics = []`。
6. 已重新跑 stable launcher 的 scoped `preflight`：
   - 当前白名单 own roots 仍被 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 卡住；
   - 这说明当前不能 claim `safe sync`，blocker 仍是同根 foreign dirty，而不是我这轮 owned 代码还红。

**关键决策**：
- `B3` 背包点击手感与 `B4` 高树冷却输入层，本轮继续按用户要求只做分析 / 方案，不继续重开逻辑。
- 本轮没有碰 `Primary.unity`、`ProjectSettings/TagManager.asset`，也没有吞并 `NPCAutoRoamController.cs`。
- 当前能 claim 的只有：直改项已落代码、代码闸门已恢复 compile-clean；不能 claim 的仍是：新的 Unity live 通过、Git 已安全白名单收口。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`

**验证结果**：
- `git diff --check`：通过
- `CodexCodeGuard`：通过
- Unity live：仍未进入；当前没有新的 `unityMCP` 实例与 PlayMode 证据
- `git-safe-sync preflight`：未通过；scoped blocker 仍是 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

**恢复点 / 下一步**：
- 下一步对用户侧要一次性讲清三件事：
  - 这轮哪些项已经直接落代码
  - `B3 / B4` 的真实根因与后续方案
  - `C1 / C2 / C3` 到底分别在验什么
- Git 侧仍需等待 `NPCAutoRoamController.cs` 同根 foreign dirty 清理或 owner 协调；
- 在那之前，不诚实 claim `safe sync`，也不把这轮包装成新的 live 已过。

## 2026-03-27：网络恢复后继续施工，`B3` 拖拽选中真源已补完，当前 compile-clean 范围扩到 15 个 C# 文件

**用户目标**：
- 用户在网络恢复后明确要求继续，不切主线；当前仍是 `0.0.1交互大清盘` 的返工收口。

**当前主线目标**：
- 把这轮已经落到代码里的交互返工继续收实到用户最近点名的背包拖拽选中手感，同时重新测准 no-red 与 safe sync 的真实边界。

**本轮已完成事项**：
1. 已把 `B3` 当前最明显的剩余漏口直接落码：
   - `InventoryInteractionManager.cs`：`OnSlotBeginDrag(...)` 现在在清空源槽位前会先选中起始格；
   - `SlotDragContext.cs`：`Begin(...)` 现在会同步选中拖拽源槽位；
   - `InventorySlotUI.cs`：`Select()` 现在优先回写 `InventoryPanelUI.SetSelectedInventoryIndex(...)`，因此拖拽起始格和最终落点格都会走背包内部真源，不再只是 Toggle 表皮亮灭。
2. 已重新核对真实编译闭环：
   - 首次只对白名单 13 文件跑 `CodexCodeGuard` 时，`GameInputManager.cs` 暴露出对 working tree `PlayerInteraction.LastActionFailureReason` 的真实依赖；
   - 因此当前 no-red 闭环不能排除 `PlayerInteraction.cs`；
   - 已把核验范围扩到 15 个 C# 文件，重跑 `CodexCodeGuard` 后 `Diagnostics=[]`，程序集为 `Assembly-CSharp`。
3. 已重新跑 `git diff --check`：
   - 当前 15 文件范围内无新的 diff 格式错误；
   - 仅剩 `InventorySlotInteraction.cs / InventorySlotUI.cs / SlotDragContext.cs / ToolbarSlotUI.cs` 的 LF 归一化提示。
4. 已重新跑 stable launcher scoped `preflight`：
   - 当前 safe sync 仍无法继续；
   - 当前白名单 own roots 下 remaining dirty/untracked 数量为 9；
   - 其中既包含 foreign dirty：`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`NPCBubblePresenter.cs`；
   - 也包含当前线程 own 同根残留：`Assets/YYY_Scripts/Service/Player/EnergySystem.cs`、`HealthSystem.cs`、`PlayerAutoNavigator.cs` 等。

**关键决策**：
- `B3` 不再继续按“只方案项”处理；至少用户最近点名的拖拽选中真源漏口已经真实落码。
- 当前能诚实 claim 的是：
  - `B3` 又补掉一块真实手感缺口；
  - 当前 15 文件编译闭环成立。
- 当前不能 claim 的仍是：
  - 新的 Unity live 通过；
  - Git safe sync 已可继续。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryInteractionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\SlotDragContext.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`

**验证结果**：
- `git diff --check`：通过，仅有 4 条 LF 归一化提示
- `CodexCodeGuard`：通过，当前闭环已扩到 15 个 C# 文件
- `git-safe-sync preflight`：未通过；own roots remaining dirty 数量为 9
- Unity live：未进入；当前没有新的运行态证据

**恢复点 / 下一步**：
- 当前对用户的真实汇报口径应更新为：
  - `B3` 选中真源已经继续落码；
  - 当前代码编译是干净的；
  - 但 shared root 收口现场并不干净。
- 下一步应等待用户继续按新实现做人手体验验收，同时单独面对 same-root remaining dirty 的 Git 收口问题。

## 2026-03-27：用户已明确重定后续对外汇报格式，必须先给 6 条人话层，再补技术审计层

**用户目标**：
- 用户明确要求：以后直接对他汇报时，不能再先讲参数、checkpoint、`changed_paths` 或其他技术 dump。
- 必须先按固定 6 条人话层回答：
  1. 当前主线
  2. 这轮实际做成了什么
  3. 现在还没做成什么
  4. 当前阶段
  5. 下一步只做什么
  6. 需要用户现在做什么（没有就写无）
- 然后才允许补技术审计层：
  - `changed_paths`
  - `验证状态`
  - `是否触碰高危目标`
  - `blocker_or_checkpoint`
  - `当前 own 路径是否 clean`

**当前主线目标**：
- 不切农田 `0.0.1交互大清盘` 主线；
- 本轮子任务是把这条新的汇报合同明确记进当前工作区，避免后续继续用旧汇报顺序。

**本轮已完成事项**：
1. 已确认这不是临时措辞建议，而是用户对后续直接汇报的硬格式要求。
2. 已把“先人话层、后技术审计层”的顺序记入当前子工作区记忆。

**关键决策**：
- 以后这条线对用户的直接汇报，默认必须先满足 6 条人话层；
- 如果先交技术 dump、后补人话，用户将直接判定汇报不合格；
- 这条要求属于当前线程和当前工作区的稳定协作口径，不再视为一次性聊天提醒。

**恢复点 / 下一步**：
- 从下一次直接汇报开始，统一改用这套顺序；
- 当前无需为这条格式要求额外改业务代码。

## 2026-03-27：主线状态审计已确认，当前不是“只剩测试”，而是停在“二次返工后待终验 + live/Git 双阻塞”

**用户目标**：
- 用户明确要求我不要再按前一轮固定模板回，而是直接说清楚：当前主线到底做到哪一步了、我现在还有什么没做完。

**当前主线目标**：
- 主线仍是农田 `V3 / 1.0.4 / 0.0.1交互大清盘`；
- 但当前真实阶段不是“全部做完只剩跑验收”，而是“代码返工已经做到第二轮，仍待用户终验、Unity live 证据和 Git 收口”。

**本轮已确认事项**：
1. 当前代码侧已经不是停在最早那版任务书，而是经过了一轮整包落码和一轮用户回执后的二次返工：
   - `A1 / A2(表现层) / A3 / B1 / B2 / B3 / B5` 都已有至少一轮针对性补刀；
   - 其中 `B3` 在网络恢复后又继续补了“拖拽起始格与最终落点格都走背包内部真源”。
2. 当前 no-red 闭环已经扩到 15 个 C# 文件，说明代码层并不是红着停住：
   - `git diff --check` 通过；
   - `CodexCodeGuard` 通过；
   - 但这只能证明“当前代码还能继续走”，不能证明“用户已经验过线”。
3. 当前 same-root dirty 里仍有一组农田线 own 文件没有收成正式 checkpoint：
   - `ToolRuntimeUtility.cs`
   - `EnergySystem.cs`
   - `HealthSystem.cs`
   - `PlayerAutoNavigator.cs`
   - `ItemTooltip.cs`
   - 以及与背包/状态条相关的 `InventorySlotUI.cs / ToolbarSlotUI.cs` 等
   这说明水壶水量条、耐久/水量显示策略、Tooltip 运行时入口这条线虽然已有工作树改动，但当前还没有被我诚实收成“已过终验的完成项”。
4. 当前没有新的 Unity live 证据：
   - 这条线到现在仍没有新的 `unityMCP` 会话和新的运行态回执；
   - 所以任何“已经彻底做完”的说法都不成立。
5. 当前也没有完成 Git 白名单收口：
   - `safe sync` 仍被同根 mixed dirty 阻断；
   - 其中既有 foreign dirty `NPCAutoRoamController.cs / NPCBubblePresenter.cs`，
   - 也有当前线程 own 同根残留。

**当前仍未完成 / 未过线项**：
1. `A1` 树苗连续放置：代码已继续补，但用户最新要求的“近身 9 宫格手感 / 相邻格直接放置”这套口径，还没有被我拿成你重新认可的结果。
2. `A3` 工具运行时展示链：动作前检和强制收尾已补，但水壶水量 UI、耐久/水量条“仅选中/悬浮/最近使用显示”、Tooltip 恢复入口，这整组当前仍属于“working tree 有改动、尚未收成正式通过 checkpoint”。
3. `B1` hover 遮挡：农田中心格口径虽然做过多轮，但你最后一次回执里已经明确说“农田还是不对，而且箱子/树苗等 placeable 的 hover 又不触发了”，这条不能算过。
4. `B2` 箱子交互链：现在不能再概括成“差不多好了”；你最后一次回执明确指出开启距离和不同角度/方向的到位判定仍不稳定。
5. `B3` 背包手感：拖拽选中真源已经继续落码，但整包“打开背包后单击选中、拖拽起手、拖拽落点、与 Toolbar 的双向映射手感”还没有拿到你的重新通过。
6. `B4` 高树冷却输入层：按你最新口径，这条仍没到完成态；当前最多只能算“有部分旧实现 + 仍待按你最新语义彻底重整”。
7. `B5` 玩家气泡样式：自然换行已经补过，但你后面又点了配色和整体风格可读性，这条现在也还不能 claim 最终通过。
8. `C1 / C2 / C3` 观察项还没有做整包终验闭环，尤其 `C2` Tooltip 你后来直接反馈成“现在根本没有”。

**恢复点 / 下一步**：
- 对用户说真话时，当前主线应表述为：
  - “已经做完两轮代码返工，但还没有过最终用户验收，也没有完成 live/Git 收口”；
- 如果继续，不该再说“只剩测试”，而应明确承认当前还剩：
  - 体验未过线项；
  - 水壶/UI/Tooltip 这组未正式收口项；
  - Unity live 证据；
  - Git safe sync。

## 2026-03-27：按最新全面整改口径继续落地，已补 A1/B1/B2/A3/C2/B4/B5 的关键缺口并恢复代码闸门通过

**用户目标**：
- 用户不再要阶段说明，而是要求我直接根据前面那组全面调整口径继续做真实整改，把还没落地的内容继续往前推。

**当前主线目标**：
- 继续服务农田 `V3 / 1.0.4 / 0.0.1交互大清盘`；
- 本轮子任务是把当前最明确仍未过线的 5 组缺口继续补到代码里：
  - `A1` 连放意图偏向
  - `B1` 农田/放置物 hover 口径
  - `B2` 箱子到位开启一致性
  - `A3/C2` 水量条与 Tooltip 自愈
  - `B4` 高树冷却对长按续挥砍的前置拦截
  - 并顺手把 `B5` 气泡可读性再往前拉一刀

**本轮已完成事项**：
1. `PlacementManager.cs`：
   - 把“已占格内边缘点击”的候选格判定从单纯阈值横跳，改成真正的相邻格意图偏向；
   - 当树苗/播种这类允许近身直放的对象卡在当前已占格时，会按鼠标倾向方向优先尝试邻格，而不是继续死抱当前格。
2. `PlacementPreview.cs + FarmToolPreview.cs`：
   - 放置物 hover 遮挡不再拿整张预览 sprite bounds 去上报，而改回占格 footprint；
   - 农田 hover 的中心格 footprint 继续缩小，避免“中心格像 3x3/4x4 一样外扩”。
3. `PlayerAutoNavigator.cs + GameInputManager.cs`：
   - 箱子这类交互目标的 stop radius 进一步收紧，角色会走得更近再停；
   - pending auto interaction 完成时会先停导航、再按同一套距离复核去开箱；
   - 箱子到位开启不再继续残留“导航停着但 UI 没开”的割裂感。
4. `PlayerInteraction.cs + GameInputManager.cs`：
   - 高树冷却前置拦截不再只覆盖“第一次点击之后的下一次普通点击”；
   - 现在连长按续挥砍前的动作前校验也会走同一层前置拦截，避免低级斧头在冷却期内继续假播动作。
5. `ToolRuntimeUtility.cs + ItemTooltipTextBuilder.cs + ItemTooltip.cs`：
   - 即使 runtime item 还没被初始化，工具状态条也会按“满值默认态”正常显示，不再整条 UI 消失；
   - Tooltip 构建会为工具补默认展示 runtime，水壶/耐久不再因为 runtimeItem 为空就整段空白；
   - `ItemTooltip` 会在场景里找到不完整旧实例时自愈 UI 引用，找不到可用 Tooltip 时也能自己补运行时实例，降低“明明悬浮了但什么都没有”的概率。
6. `PlayerThoughtBubblePresenter.cs`：
   - 继续把玩家气泡往更清晰的配色和字距上收，减少前一版“能弹出来但字不够清楚”的问题。

**验证结果**：
- `git diff --check`：通过
- `CodexCodeGuard`：通过
  - 本轮纳入闭环的 11 个文件：
    - `PlacementManager.cs`
    - `PlacementPreview.cs`
    - `FarmToolPreview.cs`
    - `PlayerAutoNavigator.cs`
    - `GameInputManager.cs`
    - `PlayerInteraction.cs`
    - `ToolRuntimeUtility.cs`
    - `ItemTooltipTextBuilder.cs`
    - `ItemTooltip.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `TreeController.cs`
- 当前仍无新的 Unity live 证据。
- 当前 `safe sync` 仍未恢复，不是代码闸门红，而是 same-root own/foreign dirty 仍在。

**恢复点 / 下一步**：
- 当前这条线已经不只是“停在上一轮返工结果”，而是又继续补了一轮关键缺口；
- 下一步最合理的是让用户按最新体感重点重新验：
  - 树苗/播种相邻格直放手感
  - 农田与 placeable hover
  - 箱子走近即开
  - 水壶/耐久条与 Tooltip
  - 高树冷却期长按续挥砍
  - 玩家气泡可读性
- Git 侧仍需等 same-root remaining dirty 进一步收口后，才能继续 safe sync。

## 2026-03-28：纯代码最终验收报告已落盘，当前最准结论更新为“主链有骨架，但 Tooltip / hover / chest / inventory 真源 仍未全过”

**用户目标**：
- 用户明确要求这轮不要再用 `UnityMCP`、不要跑运行态，而是只做一轮从头到尾的纯代码自测自省；
- 目标是把最近几轮待验项和历史返工代码一起重新审核，并给出一份最终验收报告和剩余问题总表。

**当前主线目标**：
- 主线仍是 `0.0.1交互大清盘`；
- 本轮子任务不是继续修业务，而是把整条交互线按纯代码口径重新分成：
  - 已有闭环待终验
  - 明确未过
  - 继续观察

**本轮已完成事项**：
1. 已完整回读当前子工作区任务书、当前子工作区记忆、父层记忆、线程记忆，并重新串读核心代码链：
   - 放置链：`PlacementManager.cs / PlacementValidator.cs / PlacementPreview.cs`
   - 树木链：`TreeController.cs`
   - 玩家 / 工具链：`PlayerInteraction.cs / ToolRuntimeUtility.cs / GameInputManager.cs`
   - 遮挡链：`FarmToolPreview.cs / OcclusionManager.cs / OcclusionTransparency.cs`
   - 箱子 / 背包 / Tooltip 链：`ChestController.cs / ChestInventoryV2.cs / InventoryPanelUI.cs / InventoryInteractionManager.cs / InventorySlotUI.cs / SlotDragContext.cs / ToolbarSlotUI.cs / ItemTooltip.cs / ItemTooltipTextBuilder.cs / EnergyBarTooltipWatcher.cs`
2. 已新增最终审计文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_纯代码最终验收报告.md`
3. 已把当前纯代码结论正式收成 3 类：
   - 代码层已补出骨架、待真人终验：`A1 / A2 / A3 / B4 / C3`
   - 代码层仍明显未过：`B1 / B2 / B3 / B5 / C2`
   - 继续观察：`C1`
4. 已把当前最硬 blocker 明确收敛为 `C2 Tooltip`：
   - `ItemTooltip.QueueShow(...)` 现在会先 `gameObject.SetActive(false)`，再启动延迟显示协程；
   - `EnergyBarTooltipWatcher` 也走 `ItemTooltip.ShowCustom(...)`，因此精力条 tooltip 与普通物品 tooltip 共享同一条高风险断链。
5. 已把 `B1 / B2 / B3` 的真实根因重新写实：
   - `B1`：preview footprint 与 occluder bounds 没有统一到同一事实源；
   - `B2`：导航停点与实际交互距离仍保留分叉标尺；
   - `B3`：背包面板与 hotbar 仍是“一次同步 + 分头持有”，不是真正持续同源。

**关键决策**：
- 这轮结论必须明确改口：当前绝不能说“只剩测试”；
- 更准确的状态是：
  - 核心事务骨架已经补出来了；
  - 但 `Tooltip / hover / chest / inventory 真源 / 玩家气泡终线` 还没有全部关门。
- 如果下一轮继续按代码优先级扫盘，顺序应固定为：
  - `C2 Tooltip -> B1 hover -> B2 箱子交互链 -> B3 选中态真源 -> B5 玩家气泡`

**涉及文件 / 路径**：
- 审计文档：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-03-27-交互大清盘_纯代码最终验收报告.md`
- 重点复核代码：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\ToolRuntimeUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\OcclusionManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\OcclusionTransparency.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`

**验证结果**：
- 本轮只做纯代码审计，没有使用 `UnityMCP`
- 本轮没有新的 Unity live 证据
- 本轮没有新增业务代码改动，只新增审计文档与记忆同步

**恢复点 / 下一步**：
- 当前主线恢复点应更新为：
  - “不是全线待点测，而是事务主链已补骨架，若要继续，需要先扫纯代码 blocker”
- 下一步若继续实现，应先按新报告里的 blocker 顺序处理，而不是重新散打。

## 2026-03-28：非测试剩余代码已继续收口，当前进入“代码侧完成、待用户终验”阶段

**用户目标**：
- 用户最新明确要求这轮不要再用 `UnityMCP`、不要做运行态测试；
- 只把除测试外的剩余必要工作全部做完，最后只交一份全面验收测试清单。

**当前主线目标**：
- 主线仍是 `0.0.1交互大清盘`；
- 本轮子任务是把上轮纯代码总审里尚未关门的 `C2 / B1 / B2 / B3 / B5` 继续落码，并补完整条交互线的 no-red 自检，不再让当前阶段停在“还有纯代码 blocker”。

**本轮已完成事项**：
1. `ItemTooltip.cs`：
   - 修掉了 `QueueShow(...)` 先 `SetActive(false)` 再起协程的高风险断链；
   - `ItemTooltip.Instance` 现会优先自愈旧实例引用，找不到可用实例时可补运行时实例，降低“悬浮了但什么都没有”的概率。
2. `InventoryPanelUI.cs`：
   - 新增对 `HotbarSelectionService.OnSelectedChanged` 的持续订阅；
   - 背包打开后不再只同步一次 hotbar，而是按“第一行跟 hotbar、非第一行保持背包选中真源”的口径持续刷新选中态。
3. `OcclusionManager.cs`：
   - preview 遮挡检测改为使用 occluder 的 `GetColliderBounds()`，不再拿大 sprite 外扩范围去压透明。
4. `GameInputManager.cs + PlayerAutoNavigator.cs`：
   - 箱子交互距离改成精确尺子，不再靠对 `ChestController` 额外放大阈值去糊开箱；
   - pending auto interaction 现在会在角色进入距离后主动补交互，避免“走到了但 UI 没开”的断档；
   - 箱子的 stop radius 继续收紧，角色会更接近真实开箱距离再停。
5. `PlayerThoughtBubblePresenter.cs`：
   - 玩家气泡继续往 NPC 样式语言收口，重新调整配色、限宽、尾巴偏置、字距与布局，不再保留早前那种过窄换行。
6. 本轮同时把整条交互线相关代码重新纳入更宽的 compile 闸门，覆盖：
   - `A1 / A3 / B4 / C3` 相关的 `PlacementManager.cs / PlacementPreview.cs / FarmToolPreview.cs / TreeController.cs / PlayerInteraction.cs / ToolRuntimeUtility.cs / GameInputManager.cs`
   - `B2 / B3 / B5 / C2` 相关的 `PlayerAutoNavigator.cs / ChestController.cs / InventoryPanelUI.cs / InventoryInteractionManager.cs / InventorySlotUI.cs / SlotDragContext.cs / ToolbarSlotUI.cs / ItemTooltip.cs / ItemTooltipTextBuilder.cs / EnergyBarTooltipWatcher.cs / PlayerThoughtBubblePresenter.cs / OcclusionManager.cs`

**验证结果**：
- `git diff --check`：
  - 以整条交互线相关 18 个目标脚本重跑，通过。
- `CodexCodeGuard`：
  - 先对 9 个文件跑过一轮，暴露 `GameInputManager.cs` 依赖 `PlayerInteraction.cs / TreeController.cs` 的既有事务补丁；
  - 随后把闭环范围扩到 19 个 C# 文件重跑，结果 `Diagnostics=[]`，程序集 `Assembly-CSharp`，代码闸门通过。
- 本轮未使用 `UnityMCP`；
- 本轮未进入 `Play Mode`；
- 当前没有新的 Unity live 证据。

**关键决策**：
- 上轮纯代码报告里定义为 blocker 的 `C2 / B1 / B2 / B3 / B5`，当前代码侧已经继续补到位；
- 当前最准确阶段不该再写成“还有纯代码 blocker 未扫”，而应改成：
  - 代码侧需要补的非测试项已经补完；
  - 剩下的是用户终验、运行态证据和 Git 收口，不是继续猜代码。
- 这轮没有继续扩回 scene / prefab / `Primary.unity`，也没有把范围再拉回 placeable 主链大改。

**涉及文件 / 路径**：
- 重点新增或复核文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\OcclusionManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`

**恢复点 / 下一步**：
- 当前主线恢复点更新为：
  - “代码侧非测试剩余内容已继续做完，并且程序集级自检通过；下一步不该再补猜测性代码，而该转入用户终验清单。”
- 下一步如果继续这条线，应严格按最终验收清单收回用户回执，再只针对未通过项做下一刀。

## 2026-03-29：`全局警匪定责清扫` 第四轮已把 clean subroots 的 same-root 问题排除，当前 first blocker 更新为代码闸门

本轮子工作区新增的稳定事实是：用户这轮没有让我继续补交互，而是要求按 `2026-03-29_全局警匪定责清扫第四轮_可自归仓子根收口_01.md` 只把 `Service/Placement / Farm / UI/Inventory / UI/Toolbar / World/Placeable + own docs/thread` 这组 clean subroots 尝试真实上 git，并明确禁止把 `GameInputManager.cs`、`TreeController.cs`、`ToolRuntimeUtility.cs`、`Service/Player/*` 再带回白名单。线程本轮因此没有改任何业务代码，只按第四轮要求重新组了 12 个 clean subroots 代码文件加 own docs / memory 白名单，并真实运行了 `sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread 农田交互修复V3`。这轮最关键的新结论是：第四轮已经不能再沿用第三轮的 `same-root remaining dirty` 口径，因为这次 `preflight` 明确给出 `own roots remaining dirty 数量: 0`；真正把 clean subroots 挡住的第一真实 blocker 已经变成代码闸门。当前 `preflight` 的 first exact blocker path 是 `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs:285`，原因是 `PlacementManager.cs` 仍直接调用 `GameInputManager.ShouldPreservePlacementModeForCurrentRightClick(...)`，而第四轮执行书又明确禁止把 `GameInputManager.cs` 带回白名单；同轮还一并暴露 `InventorySlotUI.cs / ToolbarSlotUI.cs` 对 `ToolRuntimeUtility.TryGetToolStatusRatio(...)` 与 `WasSlotUsedRecently(...)` 的依赖缺口，因此这组 clean subroots 现在仍不是可独立编译包。当前子层恢复点因此更新为：第四轮已经完成了“clean subroots 是否还能被 same-root 卡住”的真实判定，而且答案是否；但它也同时证明当前这 12 个 clean subroots 仍未形成可独立 `sync` 的自洽包。后续如果继续，不该再重复跑同一套 `preflight`，而应等新的明确授权，决定是允许做最小解耦修补，还是把 mixed-root 依赖重新纳回新的治理切刀。

## 2026-03-29：`全局警匪定责清扫` 第五轮已把最小共享依赖带回，但 first blocker 继续前推到 `PlayerInteraction / TreeController` 依赖

本轮子工作区新增的稳定事实是：用户这轮继续要求的不是 broad mixed 包归仓，而是在第四轮 clean subroots 基础上，只最小扩包引入 `GameInputManager.cs` 和 `ToolRuntimeUtility.cs`，然后重新真实跑一次 `preflight -> sync`。线程本轮依旧没有修改业务代码，只按第五轮执行书把 clean subroots 白名单最小扩到 14 个代码文件，并重新执行了 `sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread 农田交互修复V3`。这轮最关键的新结论有两句。第一句是：第五轮没有打脸第四轮，same-root blocker 仍然没有回来，因为本次 `preflight` 依旧返回 `own roots remaining dirty 数量: 0`。第二句是：第四轮暴露的“缺 `GameInputManager.cs / ToolRuntimeUtility.cs`”这层依赖已经被补平，但第一真实 blocker 又继续前推，新的 first exact blocker path 已变成 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:2666`；当前 `GameInputManager.cs` 自身还依赖 `PlayerInteraction.LastActionFailureReason`，同时在高树冷却前置拦截链上又依赖 `TreeController.ShouldBlockAxeActionBeforeAnimation(...)`，因此这组“clean subroots + 2 个最小共享依赖”现在仍不是可独立编译包。当前子层恢复点因此更新为：第五轮已经把 blocker 进一步钉实成“`GameInputManager.cs` 自己还在跨根咬 `PlayerInteraction.cs / TreeController.cs`”，而不是继续停留在第四轮的浅层缺依赖叙事。后续如果继续，应先重新裁定是继续最小扩包，还是改走更严格的解耦修补；在此之前，不应再把这组包包装成“已经接近能 sync”。

## 2026-03-29：子层补记，第六轮已在 `GameInputManager.cs` 内切断更深 mixed 依赖并完成归仓

当前子层新增的稳定事实是：这轮不是继续补 `0.0.1` 业务交互，而是按 `全局警匪定责清扫` 第六轮执行书，只在 `GameInputManager.cs` 内把第五轮继续暴露出来的两条更深 mixed 依赖切成本地 compat / fallback。线程本轮唯一新增代码改动只落在 `GameInputManager.cs`：一条把 `LastActionFailureReason` 的 compile-time 读取改成 `GetLastActionFailureReasonCompat()` 反射兼容口，另一条把 `ShouldBlockAxeActionBeforeAnimation(...)` 的 compile-time 调用改成 `ShouldBlockAxeActionBeforeAnimationCompat(...)` 反射兼容口；两条链在缺失成员时分别 fallback 到 `ToolUseFailureReason.None` 和 `false`。这轮最关键的新结论已经不是“又多做了一层兼容”，而是 compat 口确实把第五轮 first blocker 切断了，因为真实 `preflight` 已返回 `是否允许按当前模式继续: True`、`代码闸门通过: True`，说明新的 first blocker 已不再是 `PlayerInteraction / TreeController`。随后线程又按第五轮同组 14 个代码文件真实执行了 `sync`，代码归仓 SHA 为 `5e3fe6097ead976df3ebd967e044edf7cd031637`；再加上本轮 own docs / memory 补记归仓后，当前这条子线 own 路径已 clean。子层恢复点因此更新为：`0.0.1` 当前不再只是“代码已补完待验”，还新增了一条治理层的稳定事实，即农田线这组最小共享依赖包已经能在不把 `PlayerInteraction.cs / TreeController.cs` 带回白名单的前提下真实归仓；后续若继续，应由新的用户委托重新决定业务线而不是继续在这轮扩题。

## 2026-04-01：闭门纯代码整改补上收获回归、农具彻底中断、连放 hold、箱子距离、preview 遮挡、背包选中与玩家气泡尾差

**用户目标**：
- 用户这轮明确要求不要使用 `UnityMCP`、不要先做 live 测试，而是基于已有多轮反馈做一次真正的闭门纯代码大扫盘；同时又补充了新的真实回归：成熟作物收不起来、枯萎成熟作物也没法 collect，并强调不能为了修新问题把之前已存在的业务再打回去。

**当前主线目标**：
- 在 `0.0.1交互大清盘` 下继续完成“除测试外的剩余必要工作”，把当前最像真 blocker 的交互回归和状态机不统一问题继续压成代码闭环。

**本轮子任务 / 阻塞**：
- 子任务 1：恢复成熟 / 枯萎成熟作物的收获优先级，避免农具放置模式把收获链挡死。
- 子任务 2：把农具自动链在“工具损坏 / 空水壶 / 精力不足”等失败原因下统一改成彻底中断，不再只清半套状态。
- 子任务 3：补齐树苗连放 hold 只认鼠标、不认玩家主占格变化的问题。
- 子任务 4：继续收箱子交互距离口径、preview 遮挡回归、背包选中真源和玩家气泡表现尾差。
- 当前阻塞仍然不是编译，而是这轮仍没有新的 Unity live 证据，因此所有“已修好”都只能先停在纯代码层成立。

**已完成事项**：
1. `GameInputManager.cs`：
   - 删除了 `ShouldSkipHarvestPriorityForPlacementTool()` 对锄头 / 水壶放置模式的收获跳过逻辑，恢复 `TryDetectAndEnqueueHarvest()` 对 `Mature / WitheredMature` 的真实优先级。
   - `NotifyFarmToolAutomationTailInterrupted(...)` 现在直接走 `AbortFarmToolOperationImmediately(...)`，统一清理导航、执行 preview、queue、锁与 snapshot，解决“锄头碎了但耕地队列预览还留着”这一类半中断状态。
   - `HandleInteractable(...)` 与 pending auto interaction 改成围绕同一套 `GetInteractionDistanceThreshold(...)` 尺子执行，箱子不再一套距离开走、另一套距离开箱。
   - `GetPendingAutoInteractionCompletionDistance()` 改成优先使用已解析的 stop radius，避免 threshold 二次放大。
2. `PlacementManager.cs`：
   - 连放后的 preview hold 现在除了认鼠标是否移动，还会记录并比对玩家 `dominant cell`；只要玩家主占格发生变化，就会释放 hold，允许预览基于当前位置立即重判下一格。
   - 这刀与之前已经在的“相邻格 3x3 直放 + 已占格边缘意图偏向”一起组成完整链路，不再只会盯着旧格死 hold。
3. `OcclusionManager.cs`：
   - preview 遮挡检测不再被 `Tree / Building / Rock` tag 白名单误杀，改成只要对象本身已经注册为 `OcclusionTransparency` 就参与 preview 遮挡判断；这用于修复“农田中心块要缩小，但箱子 / 树苗 / placeable preview 完全不触发遮挡”的回归。
4. `InventoryPanelUI.cs`：
   - 加回了更明确的 `followHotbarSelection` 状态，背包打开时仍以 hotbar 为初始映射，但一旦用户主动点到非第一行格子，背包选中就不再被 hotbar 变化轻易抢回。
5. `PlayerThoughtBubblePresenter.cs`：
   - 玩家气泡重新往 NPC 气泡的暖色高对比语言靠拢，去掉了上一版绿色低对比方案；
   - 同时把最大宽度和每行偏好字符数拉宽，避免再次出现“气泡本身宽度够了却仍然显得挤和怪”的换行体验。

**关键决策**：
- 这轮严格保持纯代码整改：未使用 `UnityMCP`，未进入 `Play Mode`，也没有碰 `Primary.unity`、Prefab 或 Scene。
- 箱子距离这轮继续只在 `GameInputManager.cs` 内统一尺子，没有顺手重开存在 foreign dirty 的 `PlayerAutoNavigator.cs`。
- 当前结论必须继续写成“代码层收口成立，但真实体验仍待用户终验”，不能把这轮纯代码闭环包装成最终通过。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\OcclusionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventoryPanelUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`

**验证结果**：
- `git diff --check` 已对上述 5 个文件通过。
- `CodexCodeGuard` 已对白名单 5 文件重跑，结果 `Diagnostics=[]`，程序集 `Assembly-CSharp`，代码闸门通过。
- 本轮没有新的 Unity live 证据，也没有新的用户手测回执。

**恢复点 / 下一步**：
- 当前主线恢复点更新为：
  - “纯代码层剩余高频回归已继续压实，当前最需要的不是再猜实现，而是按最新验收清单回收用户对收获、农具中断、连放手感、箱子距离、preview 遮挡、背包选中和玩家气泡的现场回执。”
- 下一步如果继续这条线，应该先停给用户做终验；仅当回执明确指出仍有未过项时，再按那张回执对未通过触点做下一刀。

## 2026-04-01：纯代码回扫继续补 tooltip 挂载与 preview footprint 过大，当前仍停在待用户终验

**本轮新增稳定事实**：
- 这轮继续保持纯代码施工，没有使用 `UnityMCP`，也没有碰 scene / prefab / `Primary.unity`。
- 在上一轮 5 文件整改基础上，又补了 2 个此前仍有明显风险的尾差：
  1. `OcclusionTransparency.cs`
     - `GetColliderBounds()` 不再优先吃父级 `CompositeCollider2D`；
     - preview 遮挡现在优先取自身 / 子物体的局部碰撞 footprint，只有本地没有可用 collider 时才回退到父级 bounds；
     - 这用于继续收紧“农田中心格 hover 仍被远处树体误透明”的结构性风险。
  2. `ItemTooltip.cs`
     - runtime tooltip 不再盲挂到“第一个找到的 Canvas”，而是优先挂到当前最合适的激活根 Canvas；
     - 最小显示延迟从 `0.6s` 收紧到 `0.15s`；
     - tooltip 已显示时，切换物品改成立即刷新内容并置顶，而不是每次重新吃一轮长延迟；
     - 这用于修复“物品提示框看起来像根本没有”的高概率结构原因。
- 成熟 / 枯萎成熟作物收获恢复、农具失败彻底中断、连放 hold 解锁、箱子距离统一、背包选中真源与玩家气泡样式这几条上一轮补口继续保留，没有被这轮新改动覆盖掉。

**本轮验证**：
- `git diff --check` 已对白名单 7 文件通过：
  - `GameInputManager.cs`
  - `PlacementManager.cs`
  - `OcclusionManager.cs`
  - `OcclusionTransparency.cs`
  - `InventoryPanelUI.cs`
  - `ItemTooltip.cs`
  - `PlayerThoughtBubblePresenter.cs`
- `CodexCodeGuard` 已对白名单 7 文件重跑，结果 `Diagnostics=[]`，程序集 `Assembly-CSharp`。
- 这轮依旧没有新的 Unity live 证据，因此当前结论仍然只能停在“结构 / checkpoint 成立，真实体验待用户终验”。

**下一步恢复点**：
- 当前最准确状态更新为：
  - “成熟 / 枯萎收获链已从代码层恢复；tooltip 可见性与 preview footprint 又各补了一刀；剩下不该再靠猜，而该回到用户现场回执。”
- 若继续下一轮，应优先让用户重验：
  - 成熟作物收获
  - 枯萎成熟作物 collect
  - 农具失败后的彻底中断
  - 箱子 / 农田 / 树苗 preview 遮挡
  - 物品 tooltip 显示

## 2026-04-01：只读补记，连续放置边界模型仍不完整，当前已明确新的高风险漏项清单

**本轮性质**：
- 用户没有要求我这轮直接改代码，而是明确质疑“连续放置的边界情况想得还不够全面”，并点名树苗 / 播种连放手感仍会在边界细节上出错。
- 这轮保持只读分析，没有进入真实施工，也没有使用 `UnityMCP`。

**本轮新增稳定判断**：
1. 当前 `PlacementManager.cs` 已经有“相邻格意图偏向”逻辑，但它仍然是偏数学阈值，不是玩家手感语义：
   - 现状阈值为 `AdjacentIntentBiasThreshold = 0.14f`
   - 代码位置：`ResolvePreviewCandidatePosition()` / `TryResolveAdjacentIntentBiasedCandidate()` / `BuildAdjacentIntentDirections()`
2. 用户这次补充的“边界百分之10左右才往哪个方向延伸”，已经把正确语义说得更清楚了：
   - 不是“鼠标离中心超过一点就偏向邻格”
   - 而是“只有鼠标进入当前占格靠边缘那条很窄的意图区，才允许往该方向延伸”
3. 结合当前代码结构，可明确识别出新的高风险漏项：
   - 当前偏向触发带过宽，离边界还远时也可能过早偏向邻格
   - 当前偏向来源只看“当前格是否被树/作物占用”，没区分是不是“本次连续放置链刚刚放下的那个格”
   - 当前角落 / 对角 / 轴向 fallback 顺序虽已存在，但仍没有被定义成玩家可感知的稳定手感规则
   - 玩家主占格 `60%` 阈值与边界偏向规则之间仍然是两套并列尺子，存在突变点风险

**下一步恢复点**：
- 如果继续做这条线，下一刀不该再只调一个常量，而应把“连续放置的边界语义”单独收成规则：
  - 什么情况下允许从已占格延伸
  - 只在多窄的边界带里延伸
  - 对角时先走哪格
  - 什么情况下必须回到原格而不是偷偷跳邻格

## 2026-04-01：连续放置边界语义已改成“边缘窄带 + 连放链 owner”，当前停在代码层成立待用户复验

**用户目标**：
- 用户认可上一轮的判断，并要求我不要再停在分析上，而是直接把“连续放置边界语义重构”真正落进代码。
- 当前唯一主刀仍然只限 `PlacementManager.cs` 这条树苗 / 播种高频连放手感链，不扩到其他交互面，也不使用 `UnityMCP`。

**当前主线目标**：
- 把连续放置从“离中心偏一点就乱跳邻格”的旧阈值模型，改成用户明确要求的“只有进入边界约 10% 窄带才顺延”的玩家意图模型。

**本轮子任务 / 阻塞**：
- 子任务 1：把旧的 `AdjacentIntentBiasThreshold` 口径替换成边缘窄带语义。
- 子任务 2：把顺延来源从“任何已占格”收紧成“本轮连续放置刚刚落下的那个格”。
- 子任务 3：把角落 / 对角 fallback 顺序固定下来，并继续保证 preview / click 同源。
- 当前仍未跨过的边界是：这轮只有纯代码证据，没有新的 Unity live / 用户手感回执，所以不能把它写成体验已过线。

**已完成事项**：
1. `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
   - 连放顺延新增 `adjacentContinuationSourceValid / adjacentContinuationSourceCell`，现在只认“本轮刚放下的格子”作为连续放置源，不再把世界里原本已有的树苗 / 作物占位也误当成顺延触发源。
   - `ResumePreviewAfterSuccessfulPlacement()` 现在会在成功放置后登记这枚连放源格，再立刻用同一套 `ResolvePreviewCandidatePosition(...)` 重算下一格；因此 preview 和点击仍然同源，没有拆成两套判定。
   - `TryResolveAdjacentIntentBiasedCandidate(...)` 不再按“离中心偏一点就偏向”的旧阈值判断，而是要求当前鼠标先落在连放源格，且该格仍被真实占用，然后才允许顺延。
   - `BuildAdjacentIntentDirections(...)` 已改成“边界 10% 窄带”模型：
     - 不在窄带里时，返回空方向，保持原格；
     - 只进单轴窄带时，只尝试对应轴邻格；
     - 同时进角落窄带时，先尝试对角，再按更深穿透的轴优先，最后才试另一轴；
     - 不再像旧实现那样把 8 个方向都扫一遍，避免手感漂移成“鼠标没碰边也乱跳格”。
   - `EnterPlacementMode()`、`ExitPlacementMode()` 和 `HandleInterrupt()` 现在都会清掉连放链 owner，避免上一轮连放状态污染下一轮。
2. `Assets/Editor/PlacementManagerAdjacentIntentTests.cs`
   - 新增最小编辑器单测，只钉这轮的边界语义：
     - 格子内部不应顺延
     - 进入 10% 单轴窄带时只应偏向对应邻格
     - 角落窄带时应先对角，再按更深边界轴 fallback

**关键决策**：
- 这轮明确把“边界 10%”做成结构语义，而不是继续靠调一个模糊阈值碰运气。
- 顺延只认本轮连放链 owner，是为了修掉“普通世界占位也会误触发顺延”的结构错判；这不是参数问题，而是来源定义问题。
- 当前可认的是“代码 / checkpoint 成立”，不是“体验已过线”。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Editor\PlacementManagerAdjacentIntentTests.cs`

**验证结果**：
- `git diff --check`：已对白名单 2 文件通过。
- `CodexCodeGuard`：已对白名单 2 文件通过，`Diagnostics=[]`，程序集覆盖 `Assembly-CSharp` 与 `Assembly-CSharp-Editor`。
- 新增编辑器测试文件已通过程序集级编译，但本轮未进入 Unity Test Runner / PlayMode。
- `thread-state`
  - `Begin-Slice`：已在本轮继续真实施工前登记。
  - `Ready-To-Sync`：未跑；原因是这轮不是归仓回合。
  - `Park-Slice`：已在本轮停手前执行，当前状态已切回 `PARKED`。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “连续放置边界语义已经从旧阈值模型切成了边缘窄带 + 连放链 owner 模型，下一步不该再靠猜，而该回到用户手上复验树苗 / 播种的真实手感。”
- 下一步若继续，应优先让用户复验：
  - 鼠标停在已放下格子的边界约 10% 位置时，是否能稳定顺延到用户倾向的下一格；
  - 鼠标停在格子内部时，是否还会提前乱跳邻格；
  - 静止鼠标 + 玩家缓慢过格时，preview 与点击是否仍保持同一候选格。

## 2026-04-01：只读全面审计结论，当前不能诚实宣称“全部历史需求都已经没问题”

**用户目标**：
- 用户这轮不是要我继续改，而是要我把这条线从历史需求、历史返工和当前代码三个层面重新审一遍，并明确回答：我自己现在能不能诚实地说“除了用户验收外，看不到任何问题了”。

**当前主线目标**：
- 继续服务 `0.0.1交互大清盘`，但本轮子任务是只读审计与自省，不进入新的真实施工。

**本轮新增稳定判断**：
1. 当前绝不能宣称“全部历史需求都已经彻底没问题了”。
   - 我能诚实说的是：这条线已经做了多轮代码返工，很多主链从“明显错误”推进到了“结构上更对了”。
   - 但我不能诚实说“现在除了你验收外，我自己已经看不到任何问题”。
2. 以历史任务书 `A1~C3` 为主轴，再叠加后续你新增的回归与口径修正，当前最准确状态应分成三类：
   - **代码结构相对较强，但仍待用户终验**：
     - `A2` 树木倒下事务
     - `A3` 工具失效主语义
     - `B4` 高树冷却输入层前置拦截
     - `C1` 箱子双库存 / SaveLoad authoritative source
     - `C3` 无碰撞体脚下放置
     - 以及后续新增的“成熟 / 枯萎成熟作物收获恢复”
   - **代码已多轮返工，但我从代码层仍能看到明显体验风险，不能 claim 过线**：
     - `A1` 树苗 / 播种连续放置手感
     - `B1` hover 遮挡最终口径
     - `B2` 箱子走近与到位开启一致性
     - `B3` 背包 / Toolbar 选中真源与点击拖拽手感
     - `B5` 玩家气泡最终样式
     - `C2` Tooltip / 状态条整包体验
   - **已修逻辑但仍带主观或场景依赖不确定性**：
     - `A2` 树倒下动画好不好看
     - `B5` 气泡配色、换行和整体风格
3. 我这轮从当前代码里仍能直接看见 4 组高风险交界：
   - `A1` 连放现在同时受“边缘 10% 窄带顺延”和“玩家主占格 60% 直放”两套规则共同影响，结构上比以前对，但交界手感仍可能有突变。
   - `B1` 遮挡链虽然已经朝“中心格 / 物理 footprint”收口，但农田 hover、placeable preview、occluder collider 仍然跨多个脚本协同，属于最容易出现“这边修了那边偏”的区域。
   - `B2/B3` 仍然不是完全单点真源：箱子到位开启依赖 `GameInputManager + AutoNavigator + ChestController` 三段一致，背包选中依赖 `InventoryPanelUI + InventorySlotUI + InventoryInteractionManager + ToolbarSlotUI` 多段一致，所以从架构上就比单文件事务更容易残留边缘问题。
   - `C2/B5` 本质上还带 scene / prefab / Canvas / 运行态表现依赖，只靠代码静态阅读无法诚实宣布“已经完全好看且完全稳定”。

**本轮结论**：
- 如果只问“代码有没有比之前更接近正确”，答案是有，而且不少主链已经明显前进。
- 如果问“我现在能不能拍胸口说这条线除了你验收我看不到任何问题了”，答案是否。
- 当前最诚实的总判断应写成：
  - “这条线已经不是早期那种到处漏逻辑的状态了；
  - 但也绝不是全部历史需求都已彻底消灭问题；
  - 其中 `A1 / B1 / B2 / B3 / B5 / C2` 我现在仍不敢在没有你 live 回执的前提下说已经过线。”

**恢复点 / 下一步**：
- 如果后续继续，不该再用‘全部都差不多了’这种口径。
- 更准确的下一步应是：
  - 先按今天这轮审计的分类，把“结构较强但待验”和“我自己仍看见明显风险”的项分开；
  - 然后只对你最新回执里的未通过项继续开刀，而不是把整条线假装已经全部没问题。

## 2026-04-01：纯代码再收口一轮已落地到 `A1 / B1 / B2 / B3 / C2 / B5`，当前阶段仍是“结构成立、等待用户终验”

**用户目标**：
- 用户这轮明确要求继续把所有“还能靠代码继续收口”的剩余项再往前推一轮，并收成一个新的静态完成面。
- 范围固定为：`A1` 连放边界与近身直放、`B1` 农田与 placeable hover 遮挡、`B2` 箱子走近停下与到位开启、`B3` 背包 / Toolbar 选中真源、`C2` Tooltip / 工具状态条、`B5` 玩家气泡。

**当前主线目标**：
- 继续服务 `0.0.1交互大清盘`，但这轮只做纯代码深化收口，不做 Unity live，不碰 scene / prefab / `Primary.unity`。

**已完成事项**：
1. `A1` 连放边界与近身直放
   - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
   - 成功放置后的 preview hold 现在除了鼠标移动外，也会在玩家中心真实移动后自动释放，不再死等“鼠标必须抖一下”。
   - 连放源格在 hold 期间允许直接作为顺延来源，补掉“刚放下时占位回写慢一帧导致预览仍卡旧格”的结构风险。
2. `B1` hover 遮挡统一事实源
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
   - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementPreview.cs`
   - 预览遮挡上报不再共用一个会互相清空的单槽位，而是拆成 `FarmTool` 与 `PlaceablePlacement` 两个来源；placeable preview 不会再被农田 preview 的 `Hide()` 顺手清空。
   - 农田 hover 中心 footprint 又缩小一轮，继续逼近“只看中心格那一点”的口径。
3. `B2` 箱子走近停下与到位开启
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
   - 箱子自动交互改成 pending 轮询链：没到距离就继续走，到位立刻停并开箱，不再只吃一次到点回调。
   - 箱子 stop radius 继续收紧，目标是“真的走近了再开”，不是靠放大交互距离冒充修好。
4. `B3` 背包 / Toolbar 选中真源
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - 箱子槽位单击现在也会先进入选中态。
   - 跨区域拖拽放下后，旧源格会显式回收选中状态，减少“真源已切走、源格还亮着”的残影。
5. `C2` Tooltip / 工具状态条运行时入口
   - `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs`
   - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
   - tooltip 现在会优先挂到当前槽位所在的正确 Canvas，再显示并置顶，减少“逻辑触发了但挂到错误画布里看不见”的风险。
   - 同一轮 hover 切换不同物品时，tooltip 已显示状态会直接刷新，不再每次重新吃完整延迟。
6. `B5` 玩家气泡
   - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
   - 颜色、边框、阴影、气泡宽度和自然换行参数继续往 NPC 当前正式样式语言靠拢，至少先回到同一套视觉语法。
7. 结构性测试与 no-red
   - `Assets/Editor/PlacementManagerAdjacentIntentTests.cs`
   - 新增“刚好压在 10% 边界时仍应顺延”的编辑器测试。
   - scoped `git diff --check` 与 `CodexCodeGuard` 已通过，`Diagnostics=[]`。

**仍未宣称完成的边界**：
- 本轮没有新的 Unity live 证据。
- 本轮没有新的用户手感回执。
- 因此当前只能写成“结构 checkpoint 又推进了一轮”，不能写成“体验正式过线”。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmToolPreview.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\OcclusionManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Editor\PlacementManagerAdjacentIntentTests.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-04-01-交互大清盘_静态再收口验收清单.md`

**验证结果**：
- `git diff --check`：通过
- `CodexCodeGuard`：通过，`Diagnostics=[]`
- 本轮未做 Unity live / Test Runner / 用户手测

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “本轮所有还能靠代码继续收口的核心剩余项，已经又推进了一轮；下一步不该继续散修，而该按新的静态验收清单让用户只对 `A1 / B1 / B2 / B3 / C2 / B5` 做集中回执。”

## 2026-04-01：静态再收口本轮已合法 `Park`，当前只剩用户终验回执，不得包装成体验已过线

**用户目标**：
- 用户要求我把还能靠代码继续推进的剩余项再往前收一轮，但也明确表示这轮不能诚实承诺“最终完成整条线”。

**当前主线目标**：
- 继续服务 `0.0.1交互大清盘`；
- 当前这轮已经完成纯代码推进，并正式收成一个新的静态完成面与终验清单。

**本轮最终结算**：
1. 当前 slice 已执行 `Park-Slice`，线程 live 状态切回 `PARKED`，不再停留在 `ACTIVE`。
2. 本轮用户终验入口已固定为：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\0.0.1交互大清盘\2026-04-01-交互大清盘_静态再收口验收清单.md`
3. 当前这轮可以诚实 claim 的只有：
   - `A1 / B1 / B2 / B3 / C2 / B5` 又完成了一轮纯代码深化收口；
   - `git diff --check` 与 `CodexCodeGuard` 已过，代码层 compile-clean 仍成立。
4. 当前这轮不能 claim 的仍然是：
   - 新的 Unity live / 玩家体感已通过；
   - 整条交互线已经彻底完成；
   - Git 白名单 sync 已在本轮继续推进。

**验证结果**：
- `thread-state`
  - `Begin-Slice`：已在本轮真实施工前完成
  - `Ready-To-Sync`：未跑；原因是这轮不是归仓回合
  - `Park-Slice`：已完成；当前状态为 `PARKED`
- 代码自检
  - `git diff --check`：通过
  - `CodexCodeGuard`：通过，`Diagnostics=[]`
- 本轮未做 Unity live / 用户手测

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “这轮能靠代码继续收口的核心剩余项已经又推进了一轮，线程已合法停车；下一步不该再继续自由散修，而应等待用户按静态再收口验收清单回填 `A1 / B1 / B2 / B3 / C2 / B5` 的真实结果，再只对未通过项继续开刀。”

## 2026-04-01：用户要求改成人工执行的详细测试矩阵，本轮停止 live 自测并改交聊天内验收清单

**用户目标**：
- 用户在我准备继续 Unity live 自测时，明确要求“先别测了”，改为由他自己执行测试；
- 要求我像前几轮一样，直接在聊天里给出一份非常详细、可直接照着走的测试清单，而且要覆盖完整情况矩阵。

**当前主线目标**：
- 主线仍是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`；
- 但本轮子任务已经从“线程自己继续 live 试跑”切换成“把所有需要用户手感终验的项，重新打包成更细的人工测试矩阵”。

**本轮已完成事项**：
1. 已停止继续进行 Unity live 自测，没有继续扩大本轮运行态写入或测试范围。
2. 已把当前 slice 合法切回 `PARKED`：
   - `Park-Slice.ps1 -ThreadName 农田交互修复V3 -Reason user-requested-manual-test-checklist-instead-of-live-run`
3. 已确定本轮对用户的交付物不再写入新的验收文档，而是改为聊天内直接给出更细的测试矩阵：
   - 覆盖 `A1 / A2 / A3 / B1 / B2 / B3 / B4 / B5 / C1 / C2 / C3`
   - 同时把最近静态再收口和 live 里暴露过的重点风险并回矩阵，例如：
     - 连放边界 10% 顺延与近身 9 宫格直放
     - 农田 hover 与 placeable preview 的遮挡分流
     - 箱子多方向走近、停下、到位开启
     - 工具状态条、水壶水量条与 tooltip 运行时入口
4. 本轮没有新增代码修改、没有新增 Unity live 证据、没有新增 compile 结论；当前只是把测试职责切回用户侧，并补清晰的验收入口。

**关键判断**：
- 当前最合理的下一步不是我继续盲跑更多 live，而是先让用户按更细的矩阵去测；
- 原因是这条线剩下的大头已经主要是“手感 / 观感 / 到位时机 / UI 可见性 / 角度差异”类问题，人工实操比我继续在同一轮里机械加 runner 更有价值。

**thread-state**：
- `Begin-Slice`：上一轮 live 自测前已执行
- `Ready-To-Sync`：未执行；原因是本轮不是归仓回合
- `Park-Slice`：本轮已补执行
- 当前 live 状态：`PARKED`

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “线程已停止继续自测，改为等待用户按更细的聊天内测试矩阵做人工终验；收到回执后，再只对未通过项继续开刀，而不是继续无差别扩测。”

## 2026-04-02：用户纠偏“看守长”交付对象，本轮改为直接接上一轮完成面并一次性交付完整验收包

**用户目标**：
- 用户明确指出：这次“看守长”的对象不是治理线程，也不是模式解释；
- 默认就接我上一轮刚完成并已向他汇报的那一刀，直接交完整验收包，内容必须包括：
  - 总判断
  - 已自验
  - 仍需用户终验的点
  - 建议顺序
  - 详细矩阵
  - 最少必测包
  - 快捷回执单
  - 完整版回执单

**当前主线目标**：
- 主线仍是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`；
- 本轮子任务不是继续施工，而是把上一轮静态再收口 + live 自测暴露点，整理成一次性、可直接执行的完整人工验收包。

**本轮已完成事项**：
1. 已明确这次交付的基线应接在“上一轮已完成并已汇报的那一刀”上，而不是重新解释 `看守长 / 典狱长`。
2. 已把验收包口径重新压实为：
   - 既包含上一轮静态再收口的主验范围 `A1 / B1 / B2 / B3 / C2 / B5`
   - 也并回最近 live 自测里真实暴露出的重点风险：
     - `PreviewRefreshAfterPlacement` 不再是第一失败点
     - `ChestReachEnvelope` 成为新的最高风险 live 失败点
3. 已决定最终交付必须明确区分三层：
   - 线程已自验通过的部分
   - 线程已定位但尚未 live 过线的部分
   - 必须交给用户亲手终验的部分

**关键判断**：
- 这轮最核心的判断是：
  - “结构面比前一版更扎实了，但不能诚实包装成整条线已过线；
     当前最大 live 风险已经从 `A1` 局部刷新，转移到 `B2` 箱子 reach / 到位开启链。”
- 因此完整验收包必须重点把 `A1 / B1 / B2 / B3 / C2 / B5` 做成主矩阵，再把 `A2 / A3 / B4 / C1 / C3 / 收获回归` 作为回归组一起交给用户。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “这轮对用户的正确交付不是再解释模式，而是直接给出完整验收包；等用户按包回填后，再只对未通过项继续开刀。”

## 2026-04-02：用户 9 条直验问题后再推一轮纯代码返修，本轮已合法 `Park`

**用户目标**：
- 用户直接给出 9 条高压反馈，要求不要中断同步、不要再甩锅导航，而是把放置失败、边走边放、tooltip/状态条、Sword/水壶/木质工具状态、hover 遮挡、成熟/枯萎收获、玩家气泡和树倒下表现一起全盘清扫；
- 同时要求木质 `0` 档工具、水壶与 `Weapon_200_Sword_0` 先改成“一次可用”的测试口径。

**当前主线目标**：
- 主线仍是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`；
- 本轮子任务是把用户刚点名的高频失败项尽量压回新的静态完成面，而不是继续扩题去碰 scene / prefab / `Primary.unity`。

**本轮已完成事项**：
1. 放置事务主链继续改口为“距离驱动提交，不再把导航完成回调当唯一提交门”：
   - `PlacementManager.cs` 新增 `TryExecuteLockedPlacementWhenPlayerIsNear()`，Locked/Navigating 状态下只要玩家进入可放距离，就直接提交锁定放置；
   - `GameInputManager.cs` 不再因为手动移动 + 放置流而直接吞掉左键，也不再在 WASD 时把放置事务整段打断。
2. 收获入口回到“任何模式都能收”：
   - `TryDetectAndEnqueueHarvest()` 已前置到放置/工具分发之前；
   - 动画期入队链也不再因为有移动就直接跳过收获，因此成熟与 `WitheredMature` 的 collect 不再依赖当前手持物前提。
3. `Tooltip / 状态条` 被拆回两条独立语义：
   - `ItemTooltip.cs` 改成 `1s` 悬浮延迟、`0.3s` 渐显渐隐、拖拽/拿起/Shift/Ctrl suppress、像素字体优先加载、正式框体配色和更不挡视野的跟鼠定位；
   - `InventorySlotInteraction.cs / InventorySlotUI.cs / ToolbarSlotUI.cs` 把 tooltip 触发条件重新收紧，并把底部状态条改成 `0.3s` 淡入淡出。
4. `Sword / 工具 / 水壶` 运行时状态链补齐：
   - `ToolRuntimeUtility.cs` 现在会把 `WeaponData` 也纳入运行时耐久初始化与状态条读取；
   - `ItemTooltipTextBuilder.cs` 对武器补了 runtime fallback；
   - 木质 `0` 档斧头 / 锄头 / 镐子、水壶和 `Weapon_200_Sword_0` 都已改成单次测试口径。
5. hover 遮挡重新统一到“占格 footprint 是真源”：
   - `PlacementPreview.cs` 的 preview 遮挡改看占格 footprint，而不是继续吃 sprite 外扩；
   - `FarmToolPreview.cs` 的农田 hover footprint 调回接近整格，只认当前中心单元；
   - `PreviewOcclusionSource.cs` 已拆成独立文件，补掉 preview 链对 shared runtime 文件本体的隐式编译依赖。
6. 表现层继续补两刀：
   - `PlayerThoughtBubblePresenter.cs` 放宽了玩家气泡自然排版宽度，减轻过早换行；
   - `TreeController.cs` 收敛了倒下动画的过冲与回弹参数，先去掉明显卡通弹簧感。

**验证结果**：
- `git diff --check` 已重新通过（仅有 3 个现有文件的 CRLF 提示，不构成 blocker）；
- `CodexCodeGuard` 已对白名单 14 个 C# 文件通过，`Diagnostics=[]`，程序集 `Assembly-CSharp`；
- 本轮没有新的 Unity live 证据，也没有新的用户终验结果。

**当前边界 / 阻塞**：
- 本轮已执行 `Park-Slice`，当前 `thread-state = PARKED`；
- 当前真正剩下的不是代码红错，而是用户按最新版终验清单做人工终验；
- 仍不能诚实 claim “整条交互线已经最终过线”，因为 `A1 / B1 / C2 / 气泡 / 倒下表现` 仍然需要现场体感确认。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “这轮纯代码可确认的问题已继续往前推一轮，并已合法停车；下一步先让用户按新的终验清单集中回填结果，再只对未通过项继续开刀。”

## 2026-04-02：父层补记，用户喊“看守长”后当前阶段已切回完整人工终验交接

父层当前新增的稳定事实是：用户随后没有要求我继续补代码，而是明确要求默认接“上一轮刚完成并已向他汇报的那一刀”，直接交完整验收包，不再讲模式切换，也不再索要 prompt 或额外回执材料。因此本轮父层没有重新进入真实施工，没有新增代码改动，也没有追加 Unity live 验证，而是把当前阶段重新压实为“线程停止补刀，转入完整人工终验交接”。这轮父层最重要的新增判断有三点。第一，验收对象已经重新锚定为上一轮最新完成面，而不是某个抽象的大包：主验应继续围绕 `A1` 连放与近身直放、收获与工具中断回归、`C2` Tooltip 与工具状态条、`B1` 农田 / placeable hover 遮挡、`B2 / B3` 箱子与背包 / Toolbar 交互链，以及 `B5 / A2` 玩家气泡与树倒下表现。第二，线程自验和用户终验的边界已重新分开：当前能诚实 claim 的仍然只是 `git diff --check` 与 `CodexCodeGuard` 已通过、代码结构推进了一轮；当前不能诚实 claim 的仍然是“体验已经过线”或“整条交互线已经正式完成”。第三，这轮交接必须把用户真正需要判断的风险再排序清楚：最该优先抓的仍是 `A1 / 收获回归 / C2 / B1` 这几条最容易影响继续验别的项的入口链。父层恢复点因此更新为：农田 `1.0.4 / 0.0.1` 当前最新阶段应写成“代码层保持 compile-clean，线程已合法 `PARKED`，当前转入完整人工终验交接”；后续只有在拿到用户集中回执后，才继续只对未通过项返工。

## 2026-04-02：子层补记，放置 / 遮挡线在新的静态 checkpoint 内完成规则纠偏并恢复 compile-clean

子层当前新增的稳定事实是：用户继续明确追责“放置失败不是导航问题，而是我自己的放置与遮挡规则被改歪了”，并特别指出 preview 遮挡已经退化成接近“碰撞体重叠才触发”的错误口径。线程因此重新进入真实施工，先按 live 规则执行了 `Begin-Slice`，随后只围绕 `PlacementGridCalculator.cs`、`PlacementPreview.cs`、`FarmToolPreview.cs`、`OcclusionManager.cs`、`OcclusionTransparency.cs` 这 5 个文件继续纯代码收口，不碰 scene / prefab / `Primary.unity`，也没有再扩题到别的业务链。当前子层这轮最重要的新变化有四块。第一块是 compile gate 纠偏：上一版把 `PreviewOcclusionSource` 单独拆成未归仓文件，导致 preview 链在代码闸门里出现“类型不可见”的硬断点；这轮已把该枚举收回 `OcclusionManager.cs` 这个已跟踪文件，并删除独立新文件，避免 preview 遮挡链再被 untracked 类型卡红。第二块是放置到位口径纠偏：`PlacementGridCalculator.TryGetPlacementReachEnvelopeBounds(...)` 不再只偷用格子几何中心，而是改为根据真实 `GetPlacementPosition(...)`、本地 collider 包络中心和底部对齐偏移推导 reach envelope 的世界中心，目的是把“导航/近距判定”重新对齐到真实放置后的 collider 包络，而不是继续把 reach 判定钉死在抽象格心。第三块是 preview 遮挡重新分事实源：`FarmToolPreview` 继续把 hover footprint 固定为中心格焦点，`PlacementPreview` 则只把占格 footprint 送给 preview 遮挡；`OcclusionManager` 现在按 `FarmTool / PlaceablePlacement / Generic` 三类来源分流，农田 hover 不再额外放大，placeable preview 则恢复小幅缓冲。第四块是 occluder 遮挡口径纠偏：`OcclusionTransparency` 对 `FarmTool` 重新走最小物理 footprint，对 placeable / generic 则改为优先聚合可见 sprite 包络并补 root collider，而不是继续退化成几乎只认物理重叠。验证层已再次写实：针对上述 5 个 C# 文件重新执行 `git diff --check` 与 `CodexCodeGuard`，结果 `Diagnostics=[]`、程序集 `Assembly-CSharp`，说明这轮结构纠偏后的代码面已经恢复 compile-clean；但本轮仍没有新的 Unity live 证据，也没有新的用户终验。当前子层恢复点因此更新为：放置 / 遮挡链当前已经回到“规则重新贴近真实需求、静态编译通过、线程已合法 `Park-Slice`”的状态；下一步不应再继续盲改，而应由用户优先重验 `A1` 连放 / 近身直放与 `B1` 农田 / placeable hover 遮挡，再只对现场仍未过线的点继续返工。

## 2026-04-02：子层补记，看守长交接前完成最终静态复核，当前能诚实 claim 的只有“代码层成立”

子层当前新增的稳定事实是：用户随后没有再要求继续扩题施工，而是要求我“再去检查代码，确保完全没有问题后走看守人模式”。线程因此本轮先保持只读复核，回看当前放置 / 遮挡切片的 5 个目标文件、相关测试以及最近 memory；在确认没有新的结构性问题后，只补了两个纯清洁尾差：一是删掉 `PlacementGridCalculator.TryGetPlacementReachEnvelopeBounds(...)` 里已无实际作用的旧格心临时量，二是删掉 `PlacementPreview.cs` 里已经不再使用的 `TryGetPreviewSpriteBounds(...)` 私有 helper。验证层随后再次重跑：`git diff --check` 通过（仍只有 `PlacementGridCalculator.cs` 的 CRLF 提示，不构成 blocker），`CodexCodeGuard` 继续对白名单 5 文件通过，`Diagnostics=[]`，程序集 `Assembly-CSharp`。当前子层必须明确保留的边界也更清楚了：这轮完成的是“最终静态复核 + 纯清洁收尾”，不是新的 Unity live 或用户手感结论，因此当前最准确的对外口径仍然只能是“代码层成立、体验待用户终验”，不能把静态 clean 包装成体验过线。子层恢复点因此更新为：当前这 5 文件切片已完成本轮能做的全部静态检查，线程已再次合法 `Park-Slice`，下一步直接进入看守长交接，优先让用户重验 `A1 / B1`，其余项目按验收矩阵顺序继续终验。

## 2026-04-03：子层补记，放置卡顿与农田 hover 过紧已做最小纯代码修复，`placeable` 遮挡保持冻结

**用户目标**：
- 用户最新 live 反馈把范围重新收窄成两件事：
  - 放置时“放一下就卡一下”，严重影响继续测试；
  - 农田 preview 遮挡仍然过紧，只有接近碰撞体重合才触发。
- 同时用户明确裁定：`placeable` 遮挡“现在完全正确，可以用了”，这条不要再动。

**本轮子任务 / 主线关系**：
- 主线仍是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`。
- 本轮子任务只收 `放置卡顿 + FarmTool hover 过紧` 这一刀，不扩回其它交互链，也不回头重做 `placeable` 遮挡。
- 这条子任务服务于让用户先恢复可继续测试的最小现场，再决定后续未验部分怎么返工。

**本轮已完成事项**：
1. `PlacementManager.cs`
   - `ResumePreviewAfterSuccessfulPlacement()` 现在在“刚放下后仍停在同一格”的场景里，先直接把树苗 / 种子的 hold 预览切成已占位红态，不再立刻重跑一轮完整占位验证；
   - `TryPrepareSaplingPlacement(...)` 不再在树苗落地后马上再做一次 `validator.HasTreeAtPosition(...)` 全场找树重扫描，而是改成只确认新实例的本地占格根节点是否落在目标格。
2. `PlacementValidator.cs`
   - 树木 / 箱子的 `FindObjectsByType` 全场扫描已改成“同帧缓存一次”，避免树苗放下后的同一帧里被 preview 刷新、占位确认和后续验证链重复扫场。
3. `OcclusionManager.cs`
   - 只给 `PreviewOcclusionSource.FarmTool` 补了一圈很小的中心格 hover 缓冲（总 expand=`0.24f`），目的只是避免农田 preview 退化成“必须碰撞体真正重合才触发”；
   - `PlaceablePlacement / Generic` 继续保持现有 `0.14f` 缓冲，不改用户已经确认正确的 `placeable` 遮挡口径。
4. `OcclusionSystemTests.cs`
   - 新增了两条反射式单测，分别钉住 `FarmTool` 的小缓冲和 `placeable` 的原缓冲；
   - 同时把测试写法改回 `Tests.Editor.asmdef` 能合法编译的反射风格，避免测试程序集直接引用运行时类型导致新的编译红错。

**验证结果**：
- `git diff --check`：通过（仅 `PlacementValidator.cs / OcclusionSystemTests.cs` 的 CRLF 提示，不构成 blocker）。
- `CodexCodeGuard`：已对白名单 5 文件通过，`Diagnostics=[]`，程序集覆盖 `Assembly-CSharp` 与 `Tests.Editor`。
- Unity / live：本轮未跑，因此当前只能 claim 结构 / checkpoint 成立，不能 claim 体验已经过线。

**thread-state**：
- `Begin-Slice`：本轮继续沿用上一条真实施工 slice。
- `Ready-To-Sync`：未执行；原因是本轮不是归仓回合。
- `Park-Slice`：已执行。
- 当前 live 状态：`PARKED`。

**当前边界 / 下一步**：
- 当前明确冻结不动的内容：`placeable` 遮挡链。
- 当前最需要用户下一轮优先重验的只有两项：
  - 放置时是否还会出现“每放一下就明显卡一下”的卡顿；
  - 农田 preview 是否已经从“必须碰撞体重合”恢复成“中心格 + 很小 hover 缓冲”。

## 2026-04-03：白名单静态闸门补记，`OcclusionTransparency.cs` 已被确认是这刀的真实依赖并最小扩包归入当前切片

**用户目标**：
- 用户这轮没有重新扩题，只是让我继续把当前缩窄到的 `放置卡顿 + 农田 hover 过紧` 这刀收干净。
- 在结算前，我先重新钉死“这刀在最小白名单下到底能不能真正 compile-clean”。

**本轮子任务 / 主线关系**：
- 主线仍然是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`。
- 本轮子任务不是继续改业务逻辑，而是把这刀的真实静态依赖和白名单边界钉死，避免把 working tree 才成立的半成品误报成可交面。

**本轮新增稳定事实**：
1. `OcclusionSystemTests.cs` 里用户刚报的 `OcclusionManager / PreviewOcclusionSource` 类型找不到红错，现在已经不是 blocker：
   - 测试文件当前已改回反射写法，`Tests.Editor` 不再直接直引运行时类型。
2. 这轮真正暴露出的 blocker 是白名单依赖遗漏，而不是功能链再次写坏：
   - 当只对白名单 4 文件跑 `CodexCodeGuard` 时，`OcclusionManager.cs` 新调用的 `OcclusionTransparency.GetPreviewOcclusionBounds(...)` 无法在白名单快照里被看到；
   - 原因不是方法没写，而是 `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs` 当前也带着这条 preview 遮挡事实源改动，却没有被纳入同一刀。
3. 因此这轮已做最小扩包，而不是回退 placeable 遮挡逻辑：
   - 当前切片已从 4 文件最小扩到 5 文件，把 `OcclusionTransparency.cs` 明确认领为本刀真实依赖；
   - 之所以不回退调用，是因为用户已经明确裁定 `placeable` 遮挡“现在完全正确，可以用了”，硬回退会把刚冻结的正确口径重新打坏。
4. 最小扩包后，这刀的静态闸门结果已经再次钉死：
   - `git diff --check`：通过（仅 `PlacementValidator.cs / OcclusionSystemTests.cs` 的 CRLF 提示，不构成 blocker）；
   - `CodexCodeGuard`：对白名单 5 文件通过，`Diagnostics=[]`，程序集覆盖 `Assembly-CSharp` 与 `Tests.Editor`。

**thread-state**：
- 本轮重新执行了 `Begin-Slice`，随后在静态闸门通过后执行了 `Park-Slice`。
- `Ready-To-Sync`：未执行；原因仍然是这轮不是归仓回合。
- 当前 live 状态：`PARKED`。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “这刀现在的真实静态完成面是 5 文件，而不是之前误以为的 4 文件；placeable 遮挡链继续冻结不动，接下来只等用户重验放置卡顿与农田 hover 两项 live 结果。”

## 2026-04-03：继续按用户追责只打两点，放置提交重活与 FarmTool 遮挡事实源已再次定点收口

**用户目标**：
- 用户明确追责上一轮“根本没有修到点上”，并再次把范围严格收窄成两项：
  - 放置一个物品就会明显卡顿；
  - 农田 preview 遮挡依然不存在。
- 同时用户明确禁止我再动别的逻辑，只允许对这两项做定点爆破。

**本轮子任务 / 主线关系**：
- 主线仍然是农田交互修复 `V3 / 1.0.4 / 0.0.1交互大清盘`。
- 本轮子任务是重新追查“放置提交瞬间到底还有什么重路径”和“FarmTool 预览遮挡为什么仍像没接上”，不扩回箱子 / tooltip / 工具 / 气泡。

**本轮新增稳定事实**：
1. 放置卡顿这次补到的不是导航，也不是控制台噪音：
   - 我额外只读核了 `Primary.unity` 里 `PlacementManager.showDebugInfo`，当前序列化值是 `0`，所以这次 live 的明显卡顿不是因为 PlacementManager 把超密集 debug log 打开了。
2. 放置提交瞬间当前最可疑、而且确实只会在“放下这一刻”触发的两条重路径，已经被直接削掉：
   - `ResumePreviewAfterSuccessfulPlacement()` 现在不再只对树苗 / 种子做“同格立即占位红态”，而是对当前占格的整块 preview 一律直接切成已占位红态，不再在刚放下这一帧立刻重跑完整验证；
   - `ResolvePlacementParent()` 不再每次放置都优先递归整棵 active scene 去找 `Props`，而是先走农田层 `propsContainer`，scene 层再走缓存命中，避免同一楼层名反复整场景 DFS。
3. 农田 preview 遮挡失效的根因这次也被重新钉死：
   - 之前 `FarmTool` 仍然走 `GetColliderBounds()` 这条脚印口径，本质上就是把遮挡判定退化成“几乎要和碰撞体重合才触发”；
   - 现在 `OcclusionTransparency.GetPreviewOcclusionBounds(...)` 已改回按可见遮挡面返回 visual bounds。由于 FarmTool 的 preview 自身已经只收中心格，因此现在应当重新变成“中心格看可见遮挡”，而不是“中心格去撞碰撞体”。
4. 为了防止 FarmTool 遮挡再回退，当前还补了一条编辑器测试：
   - `OcclusionSystemTests.cs` 新增 `PreviewOcclusion_FarmToolSource_UsesVisualBoundsInsteadOfColliderFootprint()`，直接钉死 FarmTool 必须看可见遮挡范围，而不是退化成 collider footprint。

**验证结果**：
- `git diff --check`：通过（仍只有现有 CRLF 提示，不构成 blocker）；
- `CodexCodeGuard`：对白名单 6 文件通过，`Diagnostics=[]`，程序集覆盖 `Assembly-CSharp` 与 `Tests.Editor`。

**thread-state**：
- 本轮已重新执行 `Begin-Slice`，并在代码闸门通过后再次执行 `Park-Slice`。
- `Ready-To-Sync`：未执行；原因是这轮仍然不是归仓回合。
- 当前 live 状态：`PARKED`。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “这轮真正重新落下去的只有两枪：放置提交瞬间的重活削减、FarmTool 遮挡事实源回正；下一步只等用户重新验证这两项 live 结果，不再继续动别的逻辑。”

## 2026-04-03：左键放置卡顿只读深查，当前已把“起步卡一下 / 落地再卡一下”拆成两段真实重路径

**用户目标**：
- 用户明确反馈：重启电脑、只开 Codex 和 Unity 后，运行时放置仍会卡顿，而且是两个时点都卡：
  - 左键点下去、开始准备走过去时卡一次；
  - 真正走到并完成放置时再卡一次；
- 这轮要求我不要再泛泛甩锅给环境，而是继续彻查并给出分析详情与解决方案；输出在对话框，不落业务文档。

**本轮子任务 / 主线关系**：
- 主线仍然是农田交互修复 V3 最后尾差排障。
- 本轮子任务是只读深查“放置起步卡顿 + 放置提交卡顿”的真实回归点，不继续扩题，也不先盲改代码。

**本轮新增稳定事实**：
1. 左键起步这一卡，当前最像“我最近把同一份放置验证在一次点击里重复跑了 2~3 次”，而不是单纯导航老本身：
   - `PlacementManager.OnLeftClick()` 在 Preview 态先跑一次 `RefreshPlacementValidationAt(...)`；
   - 通过后又进入 `LockPreviewPosition()`，里面对同一目标格再次 `RefreshPlacementValidationAt(...)`；
   - 如果属于近身直放，还会直接走 `TryExecuteLockedPlacement()`，里面第三次 `RefreshPlacementValidationAt(...)`；
   - 对树苗/种子/单格放置来说，这些重验还会连带触发 `HasTreeAtPosition / HasChestAtPosition / ValidateSeedPlacement` 这类 Physics 查询与场景对象查询。
2. 左键起步时的第二层成本来自点导航本身的立即建路：
   - `PlacementNavigator.StartNavigation(...)` 直接调用 `PlayerAutoNavigator.SetDestination(...)`；
   - `SetDestination(...)` 里同帧执行 `BuildPath()`；
   - `BuildPath()` 会进 `NavigationPathExecutor2D.TryRefreshPath(...)`，其内部不仅 A* 寻路，还会做 `SmoothPath(...)`；
   - `SmoothPath(...)` 的每次视线检查又会走 `PlayerAutoNavigator.HasLineOfSight(...)`，里面既会采样 `navGrid.IsWalkable(...)`，也会做 `Physics2D.CircleCast(...)`。
3. 放置落地这一卡，当前最重的真凶不是预览，而是“新实例提交后立刻触发的运行时重初始化”：
   - 树苗分支里，`TryPrepareSaplingPlacement(...)` 会调用 `TreeController.InitializeAsNewTree()` 再 `SetStage(0)`；
   - 但 `TreeController.SetStage(0)` 当前会直接走 `RefreshTreePresentation(syncColliderShape: true)`，随后马上 `RequestNavGridRefresh()`；
   - `RequestNavGridRefresh()` 最终触发 `NavGrid2D.RefreshGrid()`；
   - `NavGrid2D.RefreshGrid()` 内部是完整 `RebuildGrid()`，包括 `Physics2D.SyncTransforms()`、世界边界检测、全网格 walkable 重算、Tilemap / Collider 显式障碍刷新；
   - 对树苗阶段 0 而言，默认配置本身就是 `enableCollider = false`，也就是说现在是在“没有阻挡碰撞体的阶段”照样整张 NavGrid 重建。
4. 箱子链也有同类问题：
   - `ChestController.Start()` 在初始化完成后会 `StartCoroutine(RequestNavGridRefreshDelayed())`；
   - 下一帧 `RequestNavGridRefresh()` 同样直接触发整张 `NavGrid2D.RefreshGrid()`；
   - 所以箱子类放置会天然在落地后一帧补一刀大刷新。
5. `ResumePreviewAfterSuccessfulPlacement()` 仍然会在提交后立即决定是否再次 `RefreshPlacementValidationAt(...)`：
   - 虽然当前已经补了 `TryApplyImmediateOccupiedHoldState(...)` 避免一部分“刚放下就立刻重验”；
   - 但只要鼠标候选格已切到别的格，或 hold 条件不满足，提交完还是会在同一收尾链上再跑一次预览验证。
6. 调试噪音仍是次级放大器，不是这轮主因，但确实还在：
   - `Primary.unity` 里 `PlayerAutoNavigator.enableDetailedDebug = 1` 仍开着；
   - 这不会解释整件事，但会放大建路/卡顿恢复的体感和 Console 成本。

**当前判断**：
- 我这轮最核心的判断是：当前卡顿不是一个神秘单点，而是两段真实回归叠在一起。
- 第一段是“左键起步前的重复重验 + 同帧建路”；
- 第二段是“放置提交后的对象初始化链，尤其树苗/箱子把整张 NavGrid 重建一遍”。
- 其中第二段是当前最重、最该先砍掉的成本；第一段则是起步那一下的直接回归点。

**恢复点 / 下一步**：
- 当前恢复点更新为：
  - “放置卡顿现在已经能明确分成两段：点击起步链的重复重验/建路成本，和落地提交链的 NavGrid 全量重建成本；后续如继续真修，应优先把这两段分别做最小去重与去全量化，而不是再泛泛调导航参数。”

## 2026-04-03：左键放置卡顿按“可回退两小刀”继续收口，当前停在待用户复测

本轮从只读分析进入了真实施工，并已先执行 `Begin-Slice`，切片固定为“左键放置卡顿双阶段回归分析与可回退修复”。这轮没有再扩回 hover、tooltip、箱子或工具链，只围绕此前已经钉死的两个回归点继续下刀：一是一次左键里同一目标格会被重复验证 2~3 次；二是树苗 `Stage 0` 虽然默认 `enableCollider = false`，但当前 `TreeController.SetStage(0)` 仍会走到碰撞体形状同步并请求整张 `NavGrid` 刷新。针对第一点，`PlacementManager.cs` 已改成“点击当帧已验证通过时，锁定与近身直放复用这次结果”，因此 `OnLeftClick -> LockPreviewPosition -> TryExecuteLockedPlacement` 这条链不再在同一点击里对同一格重复重验 2~3 次；但导航到位后的 `OnNavigationReached()` 与走近触发的 `TryExecuteLockedPlacementWhenPlayerIsNear()` 仍保留落地前重验，避免把真正跨帧的环境变化跳过去。针对第二点，`TreeController.cs` 新增 `ShouldSyncColliderShapeForCurrentPresentation()`，并把 `Start()`、`InitializeDisplay()`、`SetStage()` 三处原本无条件 `syncColliderShape: true` 的入口收成“只有当前展示态确实需要 collider 参与时才同步形状”。这意味着新放下的树苗 `Stage 0` 不再因为无碰撞阶段也去跑 `UpdatePolygonColliderShape() -> RequestNavGridRefresh()`，但阶段 1+、树桩态和真正有 collider 的展示态仍会保持旧的刷新契约。当前静态验证已再次跑过：`git diff --check -- Assets/YYY_Scripts/Service/Placement/PlacementManager.cs Assets/YYY_Scripts/Controller/TreeController.cs` 通过，没有新的 diff 结构错误。本轮未做 Unity live 测试，因此当前能诚实 claim 的阶段仍是“代码层针对双峰卡顿做了最小可回退修复，尚待用户复测体感”。线程收尾前已执行 `Park-Slice`，当前 live 状态回到 `PARKED`。下一步用户最该优先只重验两件事：左键起步那一下是否明显变轻，树苗/近身放置真正提交那一下是否还会再卡一次；如果仍有残留，再继续只沿这两段成本追，不重新扩题。

## 2026-04-03：父层补记，最新只读排障已把“左键放置卡顿”更准确收敛成现场日志/Editor 负载问题

父层当前新增的稳定事实是：用户在最新一轮把范围再次收窄成“所有内容都过线了，只剩放置左键点下去会大概率卡顿”，并且明确允许我只做“先测试再查找，再给结论”，如果最后发现问题不在业务代码也可以直接报实。线程因此这轮没有继续真实施工，没有再跑 `Begin-Slice`，而是保持只读调查，并把排查重点从放置主链本体进一步转向运行现场。当前这轮最重要的结论有四句。第一句，`PlacementManager.showDebugInfo` 虽然在 `Primary.unity` 里确实是 `0`，但这并不代表当前现场安静；`Editor.log` 在最近点击放置/耕地的时间段里清楚显示，单次点击会同时伴随 `FarmlandBorderManager`、`FarmVisualManager`、`FarmTileManager`、`ToolRuntimeUtility`、`PlayerAutoNavigator`、`NPCAutoRoamController` 等多条同步 `Debug.Log / Debug.LogWarning`，而且当前编辑器普通 log 也在写完整堆栈，这种日志风暴本身就足以造成 Editor 内的体感卡顿。第二句，场景和 prefab 序列化里还能直接看到几处关键 debug 面仍然开着：`Primary.unity` 里的 `FarmTileManager.showDebugInfo = 1`、`FarmlandBorderManager.showDebugInfo = 1`、`FarmVisualManager.showDebugInfo = 1`，而箱子 prefab 例如 `Assets/222_Prefabs/Box/Box_1.prefab` 这组 `ChestController` 默认也带着 `showDebugInfo = 1`。第三句，导航/NPC 这边还有两处会把左键放置放大成日志抖动的实现事实：`PlayerAutoNavigator` 在 `Primary.unity` 中 `enableDetailedDebug = 1`，而且“开始导航”这条 log 本身还是无条件输出；`NPCAutoRoamController` 虽然 `showDebugLog = 0`，但 `TryInterruptRoamMove(...)` 里的 `roam interrupted` warning 仍是无条件打印，所以点击后只要 NPC 同步触发 stuck cancel，也会额外往 Console 里塞 warning。第四句，环境侧负载也不是 clean baseline：当前机器上同时开着主项目 `Sunset` 和另一个 `scene-build` worktree 的 Unity Editor 进程，各自还带着 AssetImportWorker；`Library/CodexEditorCommands/requests` 当前虽然已经为空，说明后台桥现在没有继续执行新命令，但 `status.json` 仍显示最近一次执行过 `Town` 基础骨架菜单命令，因此这台 Editor 最近确实不是“只跑放置链”的干净实验现场。父层恢复点因此更新为：当前“左键放置卡顿”更像是“放置动作触发的日志/Editor 噪音叠加环境负载”，而不是我还能单靠继续改 `PlacementManager` 主链就诚实解决的纯逻辑 bug；如果后续要继续真修，最小正确方向应先是清掉这些 live debug 面和后台现场噪音，再在更干净的窗口里复测，而不是继续盲目重写放置算法。

## 2026-04-03：放置成功卡顿与导航线程边界已重新分账，当前结论是“先判 contract，不直接甩锅导航父线程”

用户这轮明确把问题再次收窄成“现在只存在放置成功的时候会卡”，并要求我必须去看导航工作区、厘清是否和导航系统有关，再决定是让导航去修、让导航补接口，还是继续由农田线自己收口，同时还要产出一份可直接发给导航的 prompt。基于这轮只读复盘，我把结论重新钉成四句。第一句，这一下卡顿和导航确实有关，但更像“placeable 成功后业务侧误打进了 `NavGrid2D.RefreshGrid()` 这条重型全量刷新入口”，而不是“导航走路主链又坏了”。最硬代码证据是：`ChestController` 在放置完成后的 `Start()` 延迟一帧就会 `RequestNavGridRefresh()`，最终直达 `NavGrid2D.OnRequestGridRefresh -> RefreshGrid -> RebuildGrid`；而 `NavGrid2D.RefreshGrid()` 当前仍是完整的显式障碍刷新加整图重建。第二句，农田线上一轮已经把树苗 `Stage 0` 这类明显不该刷整图的情况切掉，所以现在剩下的主嫌疑点更集中在 placeable 链，尤其是箱子这类成功生成后自动请求整图刷新的对象。第三句，导航父线程当前 own 边界并不在这件事上，它现在 `PARKED` 且唯一主线仍是 `Primary traversal` 场景闭环，不该把这一下卡顿泛化成“导航父线程回来接一包大修”；工具-V1 也仍按 `-59` 保持 `PARKED`，只保留 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 的精确响应权。第四句，因此当前最合理的协作方式不是直接把修复甩给导航，而是给工具-V1发一份精确只读 prompt：请它只判断当前是不是缺一个 `NavGrid` 轻量 runtime refresh contract；如果答案是否定的，就由农田线继续在 placeable 调用点收口；如果答案是肯定的，再按精确 contract gap 最小 reopen。为避免导航侧误判现场，我还在 prompt 里明确同步了农田线历史上对 `GameInputManager / PlayerAutoNavigator` 的触点修改，说明我们确实碰过箱子 pending auto interaction 与 stop radius，但本轮“放置成功才卡”不是在那组触点上新增出来的。本轮同时新增一份导航 prompt：`2026-04-03-工具V1-只判放置成功卡顿是否需要NavGrid轻量刷新contract-60.md`，后续如要跨线程协作，直接发这份即可。

补记：以上分析与 prompt 落盘后，本线程已真实执行 `Park-Slice`，当前 live 状态重新回到 `PARKED`；当前 blocker 也已明确压成“等待用户决定：是现在就把 `-60` 转发给工具-V1，还是继续由农田线本地先收口”。

进一步补记：我随后尝试把这三份文档做最小 `Ready-To-Sync`，但被真实 preflight 阻断。阻断原因不是本轮新增文档有红，而是这条线程历史上在 `.kiro/specs/屎山修复/导航检查`、`.kiro/specs/农田系统/.../0.0.1交互大清盘` 与线程目录同根下还残留大量旧 dirty / untracked，系统不允许把这轮文档单独假装 clean sync。当前因此再次执行了 `Park-Slice`，live 状态仍为 `PARKED`，第一真实 blocker 已更新为：`ready-to-sync-blocked-by-historical-own-root-dirty-under-farm-and-navigation-roots`。
