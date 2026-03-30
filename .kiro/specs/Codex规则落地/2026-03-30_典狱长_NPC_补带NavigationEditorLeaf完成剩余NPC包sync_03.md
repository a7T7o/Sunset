# 2026-03-30_典狱长_NPC_补带NavigationEditorLeaf完成剩余NPC包sync_03

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\2026-03-30_典狱长_NPC_剩余NPC包接盘开工回执_02.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_剩余NPC包接盘开工_02.md`

你上一轮的 blocker 我已经复核过了。

这次不是让你停表，也不是重唤 `导航检查V2`。

当前最准确的改判是：

- 你上一轮做的“先把 `NPCInformalChatValidationMenu.cs` 迁入 `Assets/Editor/NPC/`”是对的
- 但这一步 **没有** 让 `sunset-git-safe-sync.ps1` 把你的 Editor own root 从 `Assets/Editor` 缩成 `Assets/Editor/NPC`
- 所以当前真实问题已经不是 `DialogueDebugMenu.cs`
- 也不是你还没迁 menu
- 而是：**这轮物理上必须把 `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)` 作为 carried foreign editor leaf 一起带走，`Assets/Editor` 这根才会 clean**

一句话翻成人话：

- 这轮别再试图靠“子目录隔离”躲开 `Assets/Editor` 父根了
- 当前正确动作是：**承认 `Assets/Editor` 仍是这刀的真实 own root，并把唯一剩下的导航 Editor 菜单 leaf 一起收掉**

---

## 一、当前已接受基线

下面这些事实已经成立，不要再回到旧口径：

1. 你当前身份仍然是 `NPC integrator`
2. `UI-V1`、`导航检查V2`、`spring-day1V2` 都已经 sync，不重开
3. `NPCInformalChatValidationMenu.cs(.meta)` 已经真实迁到：
   - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
   - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs.meta`
4. 你上一轮 `preflight` 的第一真实 blocker 已经变成：
   - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
   - `Assets/Editor/NavigationStaticPointValidationMenu.cs.meta`
5. 当前这个 blocker 不是“导航线程必须回来继续做完整导航功能”
   而是“你这轮要在自己的包里最小补带一个 foreign editor leaf”

---

## 二、本轮唯一主刀

只做这一刀：

- **把 `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)` 作为 carried foreign editor leaf 纳入当前 NPC package，然后真实重跑同组 `preflight -> sync`**

这轮不要再改你上一轮已经完成的迁根动作。

这轮也不要再讨论“为什么 `Assets/Editor/NPC` 没能变成独立 own root”。

当前目标不是分析脚本实现，而是把这刀真正收过线。

---

## 三、本轮允许纳入的范围

### 1. 继续沿用上一轮 NPC package

- `Assets/YYY_Scripts/Controller/NPC/**`
- `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
- `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`
- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs`
- `Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs.meta`
- `Assets/Editor/NPC/**`
- `Assets/Editor/NPC.meta`
- `.codex/threads/Sunset/NPC/**`

### 2. 本轮新增允许带走的 foreign editor leaf

- `Assets/Editor/NavigationStaticPointValidationMenu.cs`
- `Assets/Editor/NavigationStaticPointValidationMenu.cs.meta`

这两份这轮的口径必须写成：

- `carried foreign editor leaf`

不是：

- `导航语义 owner 转移给 NPC`

---

## 四、这轮绝对不要碰

### 1. 不准扩到导航运行时根

- `Assets/YYY_Scripts/Service/Player/**`
- `Assets/YYY_Scripts/Navigation/**`
- `Assets/Editor/NavigationStaticPointValidationRunner.cs`

### 2. 不准回漂 Story peeled roots

- `Assets/YYY_Scripts/Story/UI/**`
- `Assets/YYY_Scripts/Story/Interaction/**`
- `Assets/YYY_Scripts/Story/Managers/**`
- `Assets/Editor/Story/**`
- `Assets/YYY_Tests/Editor/**`

### 3. 不准碰 scene / 字体 / 渲染

- `Assets/000_Scenes/Primary.unity`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/**`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`

### 4. 不准再把这轮写回“旧 blocker 又换了一个”

这轮不是继续报一个更大的治理问题。

这轮是：

- **在当前 NPC package 里，最小补带一个导航 Editor leaf，然后再看能不能真正 sync**

---

## 五、你这轮必须做的事

### 1. 先接受这次改判

回执里要明确写：

- 当前 `Assets/Editor` 仍是这刀真实会被脚本聚合到的 own root
- `NavigationStaticPointValidationMenu.cs(.meta)` 这轮只是 `carried foreign editor leaf`
- 不代表导航 owner 语义转移

### 2. 再真实跑 preflight

```powershell
$include = @(
  'Assets/YYY_Scripts/Controller/NPC',
  'Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs',
  'Assets/YYY_Scripts/Data/NPCRelationshipStage.cs',
  'Assets/YYY_Scripts/Data/NPCRoamProfile.cs',
  'Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs',
  'Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs.meta',
  'Assets/Editor/NPC',
  'Assets/Editor/NPC.meta',
  'Assets/Editor/NavigationStaticPointValidationMenu.cs',
  'Assets/Editor/NavigationStaticPointValidationMenu.cs.meta',
  '.codex/threads/Sunset/NPC'
)
& 'C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1' `
  -Action preflight `
  -Mode task `
  -OwnerThread NPC `
  -IncludePaths $include
```

### 3. 如果 preflight 通过

立刻同白名单 `sync`：

```powershell
$include = @(
  'Assets/YYY_Scripts/Controller/NPC',
  'Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs',
  'Assets/YYY_Scripts/Data/NPCRelationshipStage.cs',
  'Assets/YYY_Scripts/Data/NPCRoamProfile.cs',
  'Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs',
  'Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs.meta',
  'Assets/Editor/NPC',
  'Assets/Editor/NPC.meta',
  'Assets/Editor/NavigationStaticPointValidationMenu.cs',
  'Assets/Editor/NavigationStaticPointValidationMenu.cs.meta',
  '.codex/threads/Sunset/NPC'
)
& 'C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1' `
  -Action sync `
  -Mode task `
  -OwnerThread NPC `
  -IncludePaths $include
```

### 4. 如果 preflight 仍不通过

不要扩题。

只允许回：

- 第一真实 blocker
- exact path
- exact reason
- 当前 own 路径是否 clean

---

## 六、本轮完成定义

你这轮只接受两种结果：

### A｜剩余 NPC package 已真实 sync

- `Controller/NPC + Data/NPC* + Editor/NPC + Navigation editor leaf + thread dir` 成功归仓
- 你给出提交 SHA
- 当前 own 路径 clean

### B｜新的第一真实 blocker 已钉死

- 你真实跑过这轮扩过的 `preflight`
- 仍然没过
- 你只报新的第一真实 blocker

不接受：

- 再回去讨论 `DialogueDebugMenu.cs`
- 再把“子目录隔离失败”当作本轮主结果
- 顺手扩到 `NavigationStaticPointValidationRunner.cs`
- 顺手扩到导航运行时 / scene / 字体 / 渲染

---

## 七、你这轮回执必须额外写清

继续按：

- `A1 保底六点卡`
- `A2 用户补充层`
- `停步自省`
- `B 技术审计层`

但这轮必须额外回答：

1. 你是否接受当前真实 own root 仍会被脚本聚合到 `Assets/Editor`
2. `NavigationStaticPointValidationMenu.cs(.meta)` 这轮是否已按 `carried foreign editor leaf` 纳入
3. 本轮是否碰了 `NavigationStaticPointValidationRunner.cs`
   - 正确答案默认应为：`否`
4. 如果这轮过了，究竟是“NPC 吞掉了导航语义 owner”，还是“只把 Editor leaf 随当前 root package 一起带走”
   - 正确口径默认应为后者
5. 当前 own 路径是否 clean

---

## 八、你这轮最容易犯的错

- 又回去解释为什么上轮 prompt 假设错了，却不继续收口
- 把 `NavigationStaticPointValidationMenu.cs` 说成“导航又回来开工了”
- 顺手扩到 `NavigationStaticPointValidationRunner.cs`
- 看到 `Assets/Editor` 还在就把 scope 炸回整个 Editor 根

一句话收口：

- **这轮别再试图躲开 `Assets/Editor` 父根；直接把唯一剩下的导航 Editor leaf 随当前 NPC package 一起带走，再真实重跑 `preflight -> sync`。**
