# 2026-03-30_典狱长_spring-day1V2_剩余SpringStory根接盘开工_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_全局警匪定责清扫第十轮_Story大根拆包与NPC-Spring-UI分责_01.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\2026-03-30_UI-V1_StoryUI整根接盘开工回执_01.md`

你现在不是继续停表，也不是继续围着字体止血讲解释。

`UI-V1` 已经把这波 `Story/UI + prefab + registry sibling roots` 真正 sync 上去了，提交是：

- `2c6276eb`

这意味着：

- `Assets/YYY_Scripts/Story/UI`
- `Assets/222_Prefabs/UI/Spring-day1`
- `Assets/Resources/Story`

这一块已经 peeled 掉了。

你这轮被重新唤醒后的唯一主刀，是：

- **把 UI peel 之后剩下的 Spring-dominant Story 包真正接盘，并推进到真实 `preflight -> sync`；如果还不能 sync，就只允许回第一真实 blocker。**

---

## 一、当前已接受基线

下面这些事实已经成立，不要再重讲旧停表叙事：

1. `UI-V1` 当前这一刀已经过线：
   - `2c6276eb` 已在 `main`
   - `Story/UI`、`Spring-day1 prefab`、`Resources/Story registry`、对应 thread / UI workspace memory 都已归仓
   - 所以上述 roots 现在全部对你 **禁触**

2. 当前还没 peeled 的剩余 Story 包，已经明显变成 `Spring` 主导：
   - `Assets/YYY_Scripts/Story/Interaction`
   - `Assets/YYY_Scripts/Story/Managers`
   - `Assets/Editor/Story`
   - `Assets/YYY_Tests/Editor`

3. 这包里仍然有少量 foreign leaf，但现在已经不再适合先发 `NPC`：
   - `NPCInformalChatInteractable.cs(.meta)`
   - `NPCInformalChatInterruptMatrixTests.cs(.meta)`

4. 也不再适合回头再发 `UI-V1`：
   - `SpringUiEvidenceMenu.cs(.meta)` 还留在 `Assets/Editor/Story`
   - 但它所在 root 现在已经是 `Spring-dominant` 剩余包，不应再单独拆回 UI 线

一句话说透：

- **这轮你不是恢复旧 Day1 功能推进，而是以 `Spring` integrator 身份，接手 UI 剥离后剩下的 Story 主包。**

---

## 二、本轮唯一主刀

只处理这几个根，加你自己的 thread dir，以及你若更新的 Spring 工作区 memory：

- `Assets/YYY_Scripts/Story/Interaction/**`
- `Assets/YYY_Scripts/Story/Managers/**`
- `Assets/Editor/Story/**`
- `Assets/YYY_Tests/Editor/**`
- `.codex/threads/Sunset/spring-day1V2/**`
- `.kiro/specs/900_开篇/spring-day1-implementation/**`

### 这轮 `Spring` still-own core

- `Assets/Editor/Story/DialogueDebugMenu.cs`
- `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/SpringDay1BedInteractable.cs`
- `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`

### 这轮允许你带走的 foreign / support leaves

- `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
- `Assets/Editor/Story/SpringUiEvidenceMenu.cs.meta`
- `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs.meta`
- `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`
- `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs.meta`
- `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
- `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs.meta`

注意：

- `SpringUiEvidenceMenu.cs` 这轮不是改口成你的语义 own，只是它长在你要接的 `Assets/Editor/Story` 根里
- `NPCInformalChatInteractable.cs` 和 `NPCInformalChatInterruptMatrixTests.cs` 也不是改口成你的语义 own，只是这轮被批准按 `carried foreign leaf` 一起随根收口

---

## 三、这轮绝对不要碰

### 1. 不准回头碰已经 peeled 完的 UI roots

- `Assets/YYY_Scripts/Story/UI/**`
- `Assets/222_Prefabs/UI/Spring-day1/**`
- `Assets/Resources/Story/**`
- `.kiro/specs/UI系统/0.0.1 SpringUI/**`

### 2. 不准扩到导航 / Service / Data / 字体底座

- `Assets/YYY_Scripts/Service/Player/**`
- `Assets/YYY_Scripts/Data/**`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/**`
- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`

### 3. 不准碰 `Assets/Editor` 直系根

尤其继续禁止：

- `Assets/Editor/NPCInformalChatValidationMenu.cs`
- `Assets/Editor/NavigationStaticPointValidationMenu.cs`
- `Assets/Editor/NPC.meta`

### 4. 不准碰 scene hotfile

- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Primary.unity.meta`

---

## 四、你这轮必须做的事

### 1. 先按剩余 Spring 包做 integrator 认定

你这轮不用再从零讨论 owner，但必须在回执里诚实分开：

1. `Spring still-own core`
2. `carried foreign leaf`
3. `first exact blocker`（如果有）

### 2. 再真实跑整包 preflight

只允许用 stable launcher：

```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread spring-day1V2 -IncludePaths "Assets/YYY_Scripts/Story/Interaction,Assets/YYY_Scripts/Story/Managers,Assets/Editor/Story,Assets/YYY_Tests/Editor,.codex/threads/Sunset/spring-day1V2,.kiro/specs/900_开篇/spring-day1-implementation"
```

### 3. 如果 preflight 通过

立刻继续同白名单 `sync`：

```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread spring-day1V2 -IncludePaths "Assets/YYY_Scripts/Story/Interaction,Assets/YYY_Scripts/Story/Managers,Assets/Editor/Story,Assets/YYY_Tests/Editor,.codex/threads/Sunset/spring-day1V2,.kiro/specs/900_开篇/spring-day1-implementation"
```

### 4. 如果 preflight 不通过

不要越过脚本。

只允许回：

- 第一真实 blocker
- exact path
- exact reason
- 当前 own roots 是否 clean

---

## 五、这轮完成定义

你这轮只接受两种结果：

### A｜剩余 Spring Story 包已真实 sync

- 真实跑过 `preflight`
- 真实跑过 `sync`
- 这轮白名单对应 own roots 已 clean
- 你能给出提交 SHA

### B｜第一真实 blocker 已钉死

- 真实跑过 `preflight`
- 仍被挡住
- 你只回第一真实 blocker，不准扩题

不接受：

- 再讲字体止血旧叙事
- 再把自己说成“还是停表”
- 顺手回漂去 UI peeled roots
- 顺手扩到 `Service/Player`、`Data`、字体或 scene

---

## 六、你这轮回执必须额外写清

继续按：

- `A1 保底六点卡`
- `A2 用户补充层`
- `停步自省`
- `B 技术审计层`

但这轮必须额外回答这 5 件事：

1. 你这轮是 `Spring integrator`，不是旧的字体止血停表线  
   这句必须显式写出来

2. `UI-V1` 已 sync 的哪些 roots 现在对你禁触  
   你要明确承认 `Story/UI + prefab + registry` 已 peeled

3. `SpringUiEvidenceMenu.cs(.meta)` 这轮按什么口径被你带上  
   正确口径应是：`carried foreign/support leaf`

4. `NPCInformalChatInteractable.cs(.meta)` 与 `NPCInformalChatInterruptMatrixTests.cs(.meta)` 这轮按什么口径被你带上  
   正确口径应是：`carried foreign leaf`

5. `SpringDay1LateDayRuntimeTests.cs(.meta)` 这轮按什么口径处理  
   正确口径应是：`Spring / UI 交叉支撑测试，不是 NPC own`

---

## 七、你这轮最容易犯的错

- 继续沿用“我这条线已经停表”的旧话术，不承认自己现在被重新唤醒成 `Spring integrator`
- 回头重碰 `Story/UI`、prefab、registry 那些已经 sync 掉的 roots
- 看见 `SpringUiEvidenceMenu.cs` 就把整个 `Assets/Editor/Story` 再甩回 UI
- 看见 `NPCInformalChatInteractable.cs` 就把整个 `Assets/YYY_Scripts/Story/Interaction` 再甩回 NPC
- 顺手扩到 `Assets/Editor` 直系根
- 又回到 Day1 font / Service / Data / scene 混战里

一句话收口：

- **UI 那刀已经 peel 完了；你这轮只负责把剩余的 Spring-dominant Story 包真正接起来。**
