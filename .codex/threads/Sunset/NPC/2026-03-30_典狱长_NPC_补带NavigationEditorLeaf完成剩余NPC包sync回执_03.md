# 2026-03-30｜NPC｜补带 Navigation editor leaf 完成剩余 NPC 包 sync 回执｜03

## A1 保底六点卡

1. 当前主线

- 我继续按 `NPC integrator` 身份收这刀。
- 这轮唯一主刀就是：把 `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)` 作为 `carried foreign editor leaf` 纳入当前 `NPC package`，然后真实重跑同组 `preflight -> sync`。

2. 这轮实际做成了什么

- 我接受了这次改判：
  - 当前脚本聚合下，真实 own root 仍会落到 `Assets/Editor`
  - `NavigationStaticPointValidationMenu.cs(.meta)` 这轮只是 `carried foreign editor leaf`
  - 不代表导航 owner 语义转移给 NPC
- 已用扩过的白名单真实重跑 `preflight`
- 当前 `preflight` 已通过：
  - `CanContinue = True`
  - `own roots remaining dirty = 0`
  - `代码闸门通过 = True`

3. 现在还没做成什么

- 本文件落盘这一刻，最终 `sync` 还未回填 SHA
- 但这轮已经不再停在 blocker，接下来只剩同白名单 `sync`

4. 当前阶段

- 当前阶段是：
  - `Navigation` editor leaf 已被纳入当前切片
  - `preflight` 已通过
  - 当前只剩按同白名单执行 `sync`

5. 下一步只做什么

- 下一步只做同白名单 `sync`
- 不重开导航，不扩到 `NavigationStaticPointValidationRunner.cs`、导航运行时、`Story` peeled roots、`Primary.unity`、字体资产或 `OcclusionManager`

6. 需要用户现在做什么

- 暂不需要。

## A2 用户补充层

### 1. 是否接受当前真实 own root 仍会被脚本聚合到 Assets/Editor

- `是`
- 这轮我不再试图靠 `Assets/Editor/NPC` 子目录去躲开 `Assets/Editor` 父根，而是直接按真实脚本行为补带唯一剩下的 Editor leaf。

### 2. NavigationStaticPointValidationMenu.cs(.meta) 这轮是否已按 carried foreign editor leaf 纳入

- `是`
- 这轮新增纳入：
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs.meta`

### 3. 本轮是否碰了 NavigationStaticPointValidationRunner.cs

- `否`

### 4. 如果这轮过了，究竟是什么语义

- 正确口径是：
  - 只把 `Navigation` 的 Editor leaf 随当前 `NPC package` 一起带走
  - 不是 `NPC` 吞掉导航语义 owner

### 5. 当前 own 路径是否 clean

- 在本轮 `preflight` 结论里：`是`
- 即：
  - `own roots remaining dirty = 0`

## 停步自省

- 自评：
  - `8.5/10`
- 我这轮最核心的判断：
  - 既然脚本真实聚合出的 own root 还是 `Assets/Editor`，那这轮最小正确动作就不是继续解释 why，而是把唯一剩下的 `Navigation` Editor leaf 一起带走
- 我为什么认为这个判断成立：
  - 因为扩白名单后，真实 `preflight` 已经恢复成 `CanContinue = True`
- 我这轮最薄弱的点：
  - 本文件落盘时还没回填最终提交 SHA
- 如果我可能判断错，最可能错在哪：
  - 只可能错在 `sync` 阶段出现新的、与 `preflight` 不同的外层阻塞；但就当前脚本输出看，这轮已经不再卡在 own-root 清洁度上
- 如果现在继续，我最确信的下一步：
  - 只做同白名单 `sync`

## B 技术审计层

### 本轮真实纳入白名单的 include paths

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
```

### preflight 状态

- 已真实运行 `preflight`：`yes`
- 已真实运行 `sync`：`pending`
- 当前 `branch`：`main`
- 当前 `HEAD`：`158a4a02`
- `preflight` 结果：`True`

### preflight 核心结果

- `own roots remaining dirty = 0`
- `代码闸门通过 = True`
- `NavigationStaticPointValidationMenu.cs(.meta)` 本轮口径：`carried foreign editor leaf`
- 本轮是否碰了 `NavigationStaticPointValidationRunner.cs`：`no`
