# 2026-03-30_全局警匪定责清扫第十轮_Story大根拆包与NPC-Spring-UI分责_01

## 当前主线

- 治理位当前唯一主线不是继续虚开线程，而是先把 `Story / Editor / Tests` 这批 shared story roots 拆成可执行的责任矩阵。
- 这轮要解决的不是“谁名字更像 owner”，而是两件事同时钉死：
  - 语义上，这个文件到底属于 `NPC / Spring / UI` 哪条线。
  - 物理上，这个文件所在 root 现在该由谁接盘，才能真的 `preflight -> sync`。

## 当前现场只读结论

- 当前 dirty 规模：
  - `Assets/Editor = 8`
  - `Assets/YYY_Scripts/Story/Interaction = 5`
  - `Assets/YYY_Scripts/Story/Managers = 2`
  - `Assets/YYY_Scripts/Story/UI = 10`
  - `Assets/YYY_Tests/Editor = 5`
- 这说明现在的真实问题已经不是单线程 own dirty，而是 `Story 大根 mixed-root`。
- 如果还按旧口径让 `NPC` 或 `UI-V1` 各自只认自己想认的几条 exact file，再去跑 `preflight`，结果只会继续撞 same-root。

## 语义 owner 裁定

### 1. `NPC` 语义 owner

- `Assets/Editor/NPCInformalChatValidationMenu.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
- `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
- `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`

### 2. `UI-V1` 语义 owner

- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1UiPrefabRegistry.cs`
- `Assets/YYY_Scripts/Story/UI/SpringUiEvidenceCaptureRuntime.cs`
- `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
- `Assets/Resources/Story/SpringDay1UiPrefabRegistry.asset`
- `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
- `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`

### 3. `Spring` 语义 owner

- `Assets/Editor/Story/DialogueDebugMenu.cs`
- `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/SpringDay1BedInteractable.cs`
- `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`

### 4. `混合测试 / 交叉支撑` 改判

- `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
  - 不再按“UI-V1 纯 own 测试”处理。
  - 当前内容同时覆盖：
    - `Prompt / Workbench` runtime shell 自愈
    - `FreeTime -> DayEnd` 晚段推进
    - `BedBridge` 收束
  - 所以这份测试当前应改判为 `Spring / UI 交叉支撑面`，不是 NPC 线，也不是这轮 UI 根接盘的首要必吞项。

## 物理 root carrier 裁定

### 1. `Assets/YYY_Scripts/Story/UI`

- 当前 dirty：
  - `SpringDay1PromptOverlay.cs`
  - `SpringDay1UiLayerUtility.cs`
  - `SpringDay1WorkbenchCraftingOverlay.cs`
  - `SpringDay1WorldHintBubble.cs`
  - `NpcWorldHintBubble.cs(.meta)`
  - `SpringDay1UiPrefabRegistry.cs(.meta)`
  - `SpringUiEvidenceCaptureRuntime.cs(.meta)`
- 物理裁定：
  - 这整根应由 `spring-day1(UI-V1)` 接盘。
  - `NpcWorldHintBubble.cs(.meta)` 虽然语义 owner 是 `NPC`，但当前它是长在 `Story/UI` 这根里的 presentation leaf。
  - 这轮为了让 `Story/UI` 真正能收口，允许 `UI-V1` 以 `carried foreign leaf` 口径把它一起带走。
- 结论：
  - 这是本轮唯一已经足够清楚、也最接近可真 sync 的 story root。

### 2. `Assets/YYY_Scripts/Story/Interaction`

- 当前 dirty：
  - `CraftingStationInteractable.cs`
  - `NPCDialogueInteractable.cs`
  - `SpringDay1BedInteractable.cs`
  - `NPCInformalChatInteractable.cs(.meta)`
- 物理裁定：
  - 这根现在仍然不能放 `NPC` 开工。
  - 原因很直接：3 个 Spring 文件 + 1 组 NPC 文件已经在同一根里缠死。
  - 这根后续应交给 `Spring` integrator 处理，而不是让 `NPC` 继续假开。

### 3. `Assets/YYY_Scripts/Story/Managers`

- 当前 dirty：
  - `DialogueManager.cs`
  - `SpringDay1Director.cs`
- 物理裁定：
  - 纯 `Spring` 根。
  - 但它与 `Interaction / Editor/Story / Tests/Editor` 仍是同一波 Spring story integrator 事务，不宜单独先发。

### 4. `Assets/Editor/Story`

- 当前 dirty：
  - `DialogueDebugMenu.cs`
  - `SpringUiEvidenceMenu.cs(.meta)`
- 物理裁定：
  - 这里已经不是 UI-only，也不是 NPC-only。
  - `DialogueDebugMenu.cs` 主体是 Spring Day1 调试入口，只是本轮被 NPC trace 加了一层“独占验证窗口”抑制。
  - 所以这根当前仍应归入 `Spring` integrator 波次，不应让 `UI-V1` 先吞。

### 5. `Assets/YYY_Tests/Editor`

- 当前 dirty：
  - `SpringDay1DialogueProgressionTests.cs`
  - `SpringDay1LateDayRuntimeTests.cs(.meta)`
  - `NPCInformalChatInterruptMatrixTests.cs(.meta)`
- 物理裁定：
  - 这根当前也是 mixed test root。
  - `NPC` 不能因为一份 interrupt matrix 就开这整根。
  - `UI-V1` 也不能因为一份 late-day runtime test 就开这整根。
  - 当前先冻结，等 `Story/UI` peeling 完成后，再由 `Spring` integrator 作为 tests root 主 carrier 接盘。

### 6. `Assets/Editor` 直系子文件

- 当前 dirty：
  - `NPCInformalChatValidationMenu.cs(.meta)`
  - `NavigationStaticPointValidationMenu.cs(.meta)`
- 物理裁定：
  - 这块现在不能混进 Story 波次里硬吞。
  - `NPCInformalChatValidationMenu.cs` = `NPC`
  - `NavigationStaticPointValidationMenu.cs` = `导航`
  - 但它们都长在 `Assets/Editor` 直系根上，这也是为什么 `NPC` 之前一进 `preflight` 就先撞 `DialogueDebugMenu`。
- 额外判断：
  - `Assets/Editor/NPC/` 已经是真实存在的目录，后续 `NPCInformalChatValidationMenu.cs` 更合理的物理落点应是 `Assets/Editor/NPC/`，而不是继续挂在 `Assets/Editor` 直系根。
  - `NavigationStaticPointValidationMenu.cs` 也不应长期挂在 `Assets/Editor` 直系根，但这不属于本轮 Story 首开线。

## 四类裁定

### 继续发 prompt

- `spring-day1(UI-V1)`
  - 当前唯一允许首开的硬切片：
    - `Assets/YYY_Scripts/Story/UI` 整根
    - `Assets/222_Prefabs/UI/Spring-day1` 同根
    - `Assets/Resources/Story/SpringDay1UiPrefabRegistry.asset`

### 暂停等待下一波

- `NPC`
  - 原因：
    - `Story/Interaction` 还没从 Spring mixed 根里剥出来
    - `Assets/Editor` 直系根还被 `NavigationStaticPointValidationMenu.cs` 卡着
    - `Service/Player` 已经先交给 `导航检查V2` 做 root-integrator，不应再平行重发

- `spring-day1V2`
  - 当前不是“永久停尸”，而是“本波先不重唤”。
  - 原因：
    - 现在先开它，会被迫吞 `Interaction / Editor/Story / Tests/Editor` 三根里的过量 foreign
    - 应先让 `UI-V1` 把 `Story/UI` peel 掉，再决定要不要把剩余 Spring roots 成包交给它

### 无需继续发

- `项目文档总览`

## 本轮最终判断

- 这轮真正该开的不是 `NPC`，也不是 `spring-day1V2`，而是 `spring-day1(UI-V1)`。
- 不是因为 UI 更重要，而是因为它现在是 Story 大根里唯一一条：
  - 物理边界已经够清楚
  - foreign 量最小
  - 可以通过“整根接盘 + 批准 carry 一个 NPC UI leaf”的方式真实进 `preflight -> sync`
- `NPC` 和 `spring-day1V2` 现在继续发，只会再撞同样的 same-root。

## 下一步

- 已批准生成下一条唯一继续施工 prompt：
  - `spring-day1(UI-V1) -> Story/UI 整根接盘`
- 本轮不再同时群发第二条 Story prompt。
- 等 `UI-V1` 回执后，再决定：
  - 是不是重唤 `spring-day1V2` 吃剩余 Spring roots
  - 还是先等 `导航检查V2` 回来，把 `Service/Player` 与 `Assets/Editor` 直系根一起再拆一刀
