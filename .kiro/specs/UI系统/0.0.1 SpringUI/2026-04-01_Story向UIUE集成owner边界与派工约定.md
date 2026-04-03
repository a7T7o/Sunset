# 2026-04-01 Story 向 UI/UE 集成 owner 边界与派工约定

## 1. 一句话定位

`SpringUI` 这条线后续若作为外包继续承接，最准确的默认称呼不是“全项目 UI/UE 总包”，而是：

**`Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`**

它默认负责的是：

- 把玩家实际会看到、会按到、会感受到的 `提示 / overlay / 气泡 / 工作台 / 任务卡 / 唯一 E 键交互` 串成一条成立的体验链
- 在 `formal-face` 已存在时，把壳体接回 runtime，并把行为、状态、边界、中断和关闭逻辑收顺

它默认**不**负责：

- `Sunset` 全项目所有 UI 系统
- 通用背包 / 工具栏 / tooltip / 旧 crafting panel 主系统
- 纯 formal-face 美术壳体定稿
- 完整剧情状态机和全局输入系统

---

## 2. 当前代码地图

当前 `Story/NPC/Day1` 玩家可见链，按代码职责可拆成 5 层。

### A. formal-face / overlay 壳体层

代表文件：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiPrefabRegistry.cs`

这一层的职责是：

- Day1 两张正式面的 prefab-first 接回
- overlay 壳体、排版、runtime 绑定和 formal-face 几何纪律

### B. 玩家面提示 / world-space 与 screen-space 表现层

代表文件：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`

这一层的职责是：

- 真正把提示、气泡、卡片显示给玩家
- 决定提示在世界里还是屏幕里出现
- 决定“当前焦点是谁时，玩家看到的视觉归属是什么”

### C. 交互体 / 候选上报层

代表文件：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`

这一层的职责是：

- 谁能被交互
- 谁向统一仲裁中枢上报候选
- 哪个目标在当前距离 / 优先级 / 状态下应该成为唯一焦点

### D. 会话 / 提示内容 / 玩家反馈调度层

代表文件：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcNearbyFeedbackService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\DialogueManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`

这一层的职责是：

- 对话和闲聊会话如何进行
- Prompt / 任务卡要显示什么内容
- 剧情阶段、工作台阶段、夜间压力这些逻辑如何驱动玩家面

### E. 通用 UI 主系统

代表路径：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\**`

这一层是：

- 背包
- 工具栏
- tooltip
- 旧 crafting panel
- 通用状态 UI

它与 `Story/NPC/Day1` 玩家面体验链并存，但**不是**本 contract 的默认 own 面。

---

## 3. 当前已存在的中枢与主链

### 3.1 `SpringDay1ProximityInteractionService` 是否已经是统一仲裁骨架

结论：**是，但它是“Story/NPC/Day1 玩家面交互链”的统一仲裁骨架，不是全项目通用输入重做入口。**

它当前已经做到：

- 所有候选交互体都可以通过 `ReportCandidate(...)` 上报统一候选
- 当前焦点的选择顺序已经存在：
  - `forceFocus`
  - `boundaryDistance`
  - `priority`
  - `instance id`
- 当前焦点确定后，会统一驱动：
  - `SpringDay1WorldHintBubble`
  - `InteractionHintOverlay`
- 当前焦点的 `E` 键消费、冷却与触发也已经集中在这里
- 当前焦点摘要 `CurrentFocusSummary` 已经对外暴露，供导演层和快照读取

它当前**还没**做到：

- 覆盖全项目所有非 Story 交互体
- 取代 `GameInputManager` 变成全局输入中枢
- 把所有旧链和 carried leaf 全部彻底摘掉

因此后续正确动作不是“重做一个统一交互系统”，而是：

**沿现有骨架继续做 contract 收口、主链统一、旧链退场和玩家面 polish。**

### 3.2 当前真正的提示主链是什么

当前主链应理解为：

1. `NPCDialogueInteractable / NPCInformalChatInteractable / CraftingStationInteractable / SpringDay1BedInteractable`
2. 上报到 `SpringDay1ProximityInteractionService`
3. 由它选出唯一焦点
4. 然后驱动：
   - `SpringDay1WorldHintBubble`
   - `InteractionHintOverlay`
5. 再由它统一消费 `E` 键

也就是说，当前真正的“唯一提示 / 唯一 E / 视觉归属一致”的主链，是：

**`交互体 -> SpringDay1ProximityInteractionService -> SpringDay1WorldHintBubble + InteractionHintOverlay`**

### 3.3 `NpcWorldHintBubble` 当前到底是什么

结论：**当前更应视为旧并行链 / carried legacy leaf，不应再被当成默认主链中枢。**

直接证据：

- `NPCDialogueInteractable.cs` 与 `NPCInformalChatInteractable.cs` 当前都已经改成通过 `SpringDay1ProximityInteractionService.ReportCandidate(...)` 上报候选
- 它们在当前代码里对 `NpcWorldHintBubble` 的主要动作已经变成 `HideIfExists(...)`
- `NpcWorldHintBubble.cs` 自己仍保留 `RequestShow(...)` 这套老入口，说明它还存在可运行旧链
- 但当前主交互体已经不再把它当默认上报出口

因此后续 contract 必须写死：

- 它**不是**当前唯一提示主链
- 它是旧并行链 / carried leaf
- 后续任何“唯一提示 / 唯一 E / 视觉归属一致”任务，默认应围绕 `SpringDay1ProximityInteractionService` 主链推进，而不是误接 `NpcWorldHintBubble`

---

## 4. 默认 own 边界

以下模块簇，默认由“Story 向 UI/UE 集成 owner”承接。

### 4.1 Story 玩家面表层与接回层

默认 own：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiPrefabRegistry.cs`

own 内容只限于：

- 玩家可见表层
- runtime 接回
- 状态表现
- 关闭逻辑
- 壳体与行为之间的接驳

### 4.2 Story 玩家面统一仲裁 contract

默认 own：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`

own 内容包括：

- 候选 contract
- 唯一焦点规则
- 唯一提示 / 唯一 E 逻辑
- 当前焦点摘要与玩家面一致性

### 4.3 Day1 自有交互体验收口切片

默认 own 的不是整文件全部业务，而是**玩家面体验切片**：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`

仅默认 own 这些切片：

- 提示文案如何对外展示
- 当前焦点如何汇总给玩家面
- overlay / prompt 的开关、阻挡、关闭、一致性
- 工作台 / 回屋这种 Day1 自有交互的体验链收口

不默认 own：

- 完整剧情状态推进
- 所有导演逻辑
- 所有存档或 phase 数据语义

### 4.4 旧链退场 contract

默认 own：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`

但 owner 类型是：

**主链收口 owner，不是 NPC 专项体验 owner。**

也就是说：

- 它负责把这个旧叶子在 contract 上说清楚、收窄、转接或退场
- 不默认借机吞并 NPC 会话体验本身

---

## 5. 协作边界

### 5.1 与 `SpringUI formal-face owner` 的协作

协作对象：

- Day1 `Prompt / Workbench` 的正式壳体、prefab 真源、formal-face 基线

协作规则：

- `SpringUI formal-face owner` 负责壳体真值、prefab 视觉基线和几何纪律
- `Story 向 UI/UE 集成 owner` 负责把这些壳体真正接回运行时，并把状态、行为、阻挡和玩家面体验接顺
- 如果任务本质是“改壳体长相”，默认先发给 `SpringUI formal-face owner`
- 如果任务本质是“壳体已定，运行时接得不好 / 用起来不顺”，默认发给本 owner

### 5.2 与 `NPC active owner` 的协作

协作对象：

- `NPCBubblePresenter`
- `PlayerNpcChatSessionService`
- `PlayerNpcNearbyFeedbackService`
- `NPCDialogueInteractable`
- `NPCInformalChatInteractable`

协作规则：

- 当 `NPC` 线程仍活跃时，NPC 聊天体验、气泡样式、闲聊节奏和双气泡协同，默认仍由 `NPC active owner` 主刀
- 本 owner 只接：
  - 与统一仲裁 contract 直接相关的接口收口
  - 与玩家面“唯一提示 / 唯一 E / 视觉归属一致”相关的整合问题
- 不应趁“统一体验”名义，默认吞掉 NPC 专项体验线

### 5.3 与 `spring-day1` 逻辑 owner 的协作

协作对象：

- `DialogueManager`
- `DialogueUI`
- `SpringDay1Director`

协作规则：

- `spring-day1` 逻辑 owner 负责剧情阶段、任务推进、对话内容与阻塞条件
- 本 owner 负责这些逻辑如何投射到玩家面：
  - Prompt 显示
  - Workbench 体验
  - 世界提示
  - 当前焦点摘要
- 如果任务本质是“下一阶段剧情是什么 / 条件怎么变”，默认不是本 owner 单独主刀

### 5.4 与 `场景搭建（外包）` 的协作

协作对象：

- 场景里的工作台、床、入口、回屋落点、玩家感知 affordance

协作规则：

- `场景搭建（外包）` 负责场景中的视觉可见摆位与空间 affordance
- 本 owner 负责告诉它：
  - 哪个点需要被玩家一眼识别成可交互目标
  - 提示和 overlay 需要怎样的锚点关系
- 本 owner 不默认 owning 场景美术和空间构图施工

---

## 6. 默认禁止边界

以下内容默认不该由“Story 向 UI/UE 集成 owner”直接吞并。

### 6.1 通用 UI 主系统

默认禁止：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\**`

包括但不限于：

- 背包
- 工具栏
- tooltip
- 旧 crafting panel
- 通用 inventory 交互

### 6.2 剧情主状态机与完整对话引擎

默认禁止整吞：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\DialogueManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`

允许的仅是：

- 玩家面呈现接口
- 提示 / overlay 接口
- 快照摘要接口

### 6.3 全局输入与 hot-file

默认禁止：

- `GameInputManager`
- 全局输入仲裁
- scene hot-file
- `Primary.unity`
- `ProjectSettings`
- 场景热区治理

### 6.4 NPC 专项体验线

当 `NPC` 线程仍为 active owner 时，默认禁止直接吞并：

- NPC 正式 / 非正式聊天体验整线
- NPC 气泡样式与节奏
- 玩家 / NPC 双气泡会话语义

### 6.5 把 `NpcWorldHintBubble` 误当主链

默认禁止：

- 再围绕 `NpcWorldHintBubble` 继续扩默认新功能
- 把“唯一提示 / 唯一 E”建立在旧并行链上

---

## 7. 派工口径

以后如果用户要发令，应优先按下面这张表分派。

| 任务类型 | 默认归属 | 说明 |
|---|---|---|
| `Prompt / Workbench` 正式壳体样式、字号、几何、prefab 真值 | `SpringUI formal-face owner` | 这是 formal-face 基线，不是本 owner 默认主刀 |
| `Prompt / Workbench` 壳体已定，但 runtime 接回不稳、状态显示不顺、关闭逻辑不顺 | `Story 向 UI/UE 集成 owner` | 本 owner 默认主刀 |
| 最近目标唯一提示、唯一 `E` 键、视觉归属一致 | `Story 向 UI/UE 集成 owner` | 围绕 `SpringDay1ProximityInteractionService` 主链推进 |
| `NpcWorldHintBubble` 旧链收口、主链迁移、退场 contract | `Story 向 UI/UE 集成 owner（需同步 NPC）` | 它是 contract 收口，不是 NPC 专项体验重做 |
| NPC 正式 / 非正式聊天节奏、气泡样式、双气泡体验、中断语义 | `NPC active owner` | 本 owner 只协作，不默认主刀 |
| Day1 阶段推进、任务条件、剧情阻塞、对话内容 | `spring-day1 逻辑 owner` | 本 owner 不默认改剧情语义 |
| `DialogueUI` 仅做玩家面表现 polish，不改对话状态机 | `协作任务：Story 向 UI/UE 集成 owner + spring-day1 逻辑 owner` | 需要分清表现层与逻辑层 |
| 工作台 / 床 / 入口在场景里的可见 affordance、摆位、空间提醒 | `场景搭建（外包）` | 本 owner 提需求，不默认施工 |
| 通用背包 / 工具栏 / tooltip / inventory / 旧 crafting panel | `通用 UI owner` | 默认不发给本 owner |
| 全局输入、`GameInputManager`、scene hot-file、`Primary.unity` | `转派或治理` | 不是本 owner 默认可碰面 |

### 适合直接发给本 owner 的指令关键词

适合：

- `把玩家面提示链收顺`
- `把唯一 E 和唯一提示收口`
- `把 Story/NPC/Day1 这条体验链串起来`
- `formal-face 已经有了，帮我把 runtime 接顺`
- `把工作台 / 任务卡 / 世界提示做成玩家真的能用的版本`

### 不适合直接发给本 owner 的指令关键词

不适合：

- `你把整个项目 UI/UE 都接了`
- `你顺手把背包 / 工具栏 / tooltip 也统一重做`
- `你把 NPC 整根聊天线吞了`
- `你把剧情状态机和输入系统也一起改了`

---

## 8. 后续接盘顺序

如果以后真的把这条线当 `Story 向 UI/UE 集成外包` 来使用，最合理的接盘顺序只应有 3 步。

### Step 1：先锁主链 contract

先做：

- `SpringDay1ProximityInteractionService` 的 candidate contract 和主链边界
- `NpcWorldHintBubble` 的旧链定位与退场约束
- `唯一提示 / 唯一 E / 当前焦点摘要` 的文档与代码约束统一

目标：

- 不再把主链和旧链混着讲

### Step 2：再做玩家面一致性收口

再做：

- `Prompt / Workbench / WorldHint / InteractionHint` 这组玩家面一致性
- `CraftingStation / Bed / NPC 交互体` 的提示、关闭、阻挡与焦点表现统一

目标：

- 玩家看到的提示、实际能触发的对象、按下去后的结果三者一致

### Step 3：最后做 formal-face-backed polish

最后做：

- 在 `formal-face` 已稳定的前提下，继续做壳体接回后的 runtime polish
- 只对已经纳入主链的玩家面继续走 live capture / 用户终验

目标：

- 不在壳体未定、主链未明时提前扩成“大一统 UI 重做”

---

## 9. 证据层级说明

### 9.1 本文已经站稳的证据

以下判断站在**代码结构 / 现有链路证据**上：

- `SpringDay1ProximityInteractionService` 已经存在，并承担统一候选仲裁
- 当前主交互体已经改成向它上报候选
- 当前提示主链是 `SpringDay1WorldHintBubble + InteractionHintOverlay`
- `NpcWorldHintBubble` 仍然存在自己的旧入口，但不再是当前主交互体的默认上报出口
- 通用 UI 主系统与 `Story/NPC/Day1` 玩家面链是并存而非同一条线

### 9.2 本文属于 owner / contract 推断的部分

以下判断属于**owner contract 推断**，不是 live 审美结论：

- 哪类任务以后默认该发给谁
- 哪些文件簇属于默认协作边界
- 哪些模块不该再由本 owner 默认吞并

### 9.3 后续若要宣称“玩家体验过线”仍需补的证据

若后续要把某一刀说成“体验已经过线”，还需要：

- live GameView / capture
- 真实运行态 proof
- 用户终验

本文**不能**被拿来替代：

- UI 美术终验
- 体验过线判断
- live 行为收口证明

---

## 最终约定

从这份 contract 起，后续默认口径固定为：

1. 这条线是 `Story / NPC / Day1 玩家面体验链的 UI/UE 集成 owner`
2. 当前统一仲裁骨架已存在，应继续在其上收口，不从零重造
3. `NpcWorldHintBubble` 不是当前主链中枢，默认按旧并行链 / carried leaf 处理
4. 派工必须先分清：
   - `formal-face`
   - `玩家面体验`
   - `剧情交互矩阵`
5. 只有这样，后续 owner 边界、回执审核和实现切片才不会再次漂移
