# 2026-04-01 UI线程｜Story 向 UI/UE 集成 owner 边界收口任务书

## 先读

请先完整读取并吸收以下材料，再开始本轮：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\场景搭建（外包）\memory_0.md`
4. 你自己上一轮已回给用户的那份“我不能当全项目 UI/UE 总包，但可以当 Story 向 UI/UE 集成外包”的回执原文

这轮不是继续实现，不是继续修 `Prompt / Workbench`，也不是让你去接全项目 UI。

这轮只做一件事：**把你这条线后续到底该接什么、不该接什么、怎么接，收成一份可直接派工的 owner 边界文档。**

---

## 当前已接受的基线

下面这些判断，当前已经可以当作本轮基线，不要再回退重讲：

1. 你**不能**被默认当成 `Sunset` 全项目 `UI/UE` 总包。
2. 你**可以**被默认当成：`Story 向 UI/UE 集成外包`。
3. `SpringUI/UI-V1` 更偏 `formal-face / 壳体 / Day1 两张面` 这类 formal-face 基线。
4. `NPC` 线程如果仍活跃，就仍然是 NPC 专项玩家面体验的 active owner，不应被你默认吞并。
5. 你更适合接的是：**Story/NPC/Day1 这条玩家可见体验链里的提示、overlay、气泡、任务卡、工作台、E 键交互一致性与体验收口。**

---

## 本轮唯一主刀

**把“Story 向 UI/UE 集成外包”这句话，收成一份真正可执行、可派工、可审回执的 owner contract。**

注意，是 `owner contract`，不是泛泛而谈的岗位感想。

---

## 这轮允许的 scope

你这轮允许做的只有：

1. 只读分析 `Story/NPC/Day1` 玩家可见 UI / 交互链相关代码与现有记忆
2. 在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI` 下新增 / 更新**一份主文档**
3. 最小更新 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`

你可以引用和分析的重点代码面，至少应覆盖：

- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\NpcWorldHintBubble.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcNearbyFeedbackService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\DialogueManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`

---

## 这轮明确禁止的漂移

这轮**不允许**：

1. 不允许进入任何业务实现、UI 修复、prefab 调整、运行时整改
2. 不允许把本轮写成“再做一次全盘大调研”
3. 不允许把自己扩成“全项目 UI 总包”
4. 不允许把 `通用 UI 系统` 和 `Story/NPC/Day1 玩家面体验链` 混成一锅
5. 不允许把“我要做统一交互系统”说成从零重做
6. 不允许回避 `NpcWorldHintBubble` 当前到底是不是主链这个问题
7. 不允许给出纯概念话术，必须落到**文件簇、owner 边界、默认派工规则**

---

## 你这轮必须正面回答的 3 个硬问题

### 1）你有没有识别出当前统一仲裁骨架已经存在

你必须明确回答：

- `SpringDay1ProximityInteractionService` 当前是不是已经是“最近目标 + 唯一提示 + 唯一 E 键仲裁”的骨架？
- 如果是，它已经做到哪一步？
- 你后续应接的是“重做系统”，还是“沿现有骨架做 contract 收口 / 玩家面 polish / 一致性修正”？

这一点不能模糊。

### 2）你有没有分清 `NpcWorldHintBubble` 和当前主链

你必须明确回答：

- `NpcWorldHintBubble` 当前是主链、并行旧链，还是 carried leaf / 残留链？
- 现在真正的玩家面提示主链到底是什么？
- 后续如果任务落到“唯一提示 / 唯一 E / 视觉归属一致”，你默认会围绕哪条链接，而不是误接旧链？

### 3）你到底接哪些，绝不该接哪些

你必须把后续派工压成三类：

1. **你默认该接的**
2. **你可协作但不是单独 owner 的**
3. **你默认不该接 / 必须转派的**

不能只用抽象词，必须落到文件簇 / 模块簇级别。

---

## 主文档完成定义

你这轮必须在：

`D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI`

下交付一份主文档，文件名请直接使用：

`2026-04-01_Story向UIUE集成owner边界与派工约定.md`

这份文档必须至少包含以下 8 个部分，缺一不可：

1. **一句话定位**
   - 以后默认怎么称呼你这条线最准确

2. **当前代码地图**
   - 把 `Story/NPC/Day1` 玩家面链路拆成几层
   - 每层的代表文件是什么

3. **当前已存在的中枢与主链**
   - 明确写清 `SpringDay1ProximityInteractionService`
   - 明确写清当前提示主链
   - 明确写清 `NpcWorldHintBubble` 的定位

4. **默认 own 边界**
   - 你默认吃哪些模块 / 文件簇

5. **协作边界**
   - 哪些任务需要你和 `SpringUI formal-face owner`、`NPC`、`spring-day1`、`场景搭建（外包）` 协作

6. **默认禁止边界**
   - 哪些面你不该直接吞

7. **派工口径**
   - 如果用户以后要给你发令，哪些词 / 哪些任务类型应该发给你
   - 哪些类型不该发给你
   - 最好给出 6~10 条“任务类型 -> 默认归属”的派工表

8. **后续接盘顺序**
   - 如果以后真的把你当 `Story 向 UI/UE 集成外包` 来用，最合理的接盘顺序是什么
   - 只允许给 3 步，不要扩散

---

## 证据要求

这轮不是体验终验，所以你必须把证据层级说清楚：

- 哪些判断站在**代码结构 / 现有链路证据**
- 哪些判断仍然只是**owner / contract 推断**
- 哪些内容如果后续真的要过“玩家体验线”，还需要 live / capture / 用户终验

不允许把 owner 判断包装成“体验已经过线”。

---

## 回执格式

聊天里不要贴大段正文，不要再重复整份文档内容。

你只按下面格式回：

### 用户可读层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### 用户补充层（可选）

- 如果你还有 1~3 条特别值得提醒用户的判断，再补这里

### 技术审计层

- 主文档路径
- `memory.md` 路径
- 本轮读取了哪些关键文件
- 是否有代码改动（这轮正常应为 `无`）

---

## 这轮完成后仍然没做完的内容

你这轮完成，不代表：

- 你已经拿到了新的实现 owner
- 你已经开始接 `Prompt / Workbench` 新一轮修复
- 你已经开始做唯一提示 / 唯一 `E` / NPC 优先级实现
- 你已经正式吞并 NPC 玩家面体验线

这轮做完后，正确状态只应是：

**职责边界可以直接派工了。**

