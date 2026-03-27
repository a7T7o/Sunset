# 2026-03-26-NPCV2-Editor修复后Primary与字体owner复核-07

你现在继续作为 `NPCV2`，但这轮不是继续做 `HomeAnchor` 新功能，也不是去接导航 runtime。

## 当前已确认的事实

1. 你在提交 `24886aad` 中真实修掉了：
   - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
   - `Play Mode` 下误用 `MarkSceneDirty` 的 Inspector 报错。
2. 这不自动等于：
   - `Primary.unity` 当前所有 dirty 都归你；
   - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 的 runtime 导航问题归你；
   - 三份 `DialogueChinese*.asset` TMP 字体 dirty 归你。
3. 当前 shared root 现场里：
   - `Assets/000_Scenes/Primary.unity` 仍为 mixed hot 面；
   - 三份 dirty 字体的最近提交历史是 `spring-day1 / spring-day1V2`，不是你这条线的 `65e1ee35 / 24886aad`。

## 这轮唯一主刀

只做：

> 对 `Primary.unity` 与当前 3 份 dirty `DialogueChinese*` 字体做一次 NPC 线 own / non-own 报实；如果且仅如果能从 `Primary.unity` 中切出你自己的 `HomeAnchor / Inspector auto-repair` residue，才做最小 cleanup

换句话说：

1. 这轮重点不是“再改什么”，而是把底盘说清。
2. 你要明确回答：
   - `Primary.unity` 里现在还有没有你这条线自己留下、并且能安全切开的 residue；
   - 三份 dirty 字体是不是你的。
3. 当前基于 git 历史，我的初判是：
   - 字体不是你的；
   - `Primary.unity` 可能含有你这条线的历史 residue，但当前 diff 很可能仍是 mixed，需要你只读核实。

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Primary归属报实与最小cleanup委托-06.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工详细汇报-04.md`
5. 提交：
   - `65e1ee35`
   - `24886aad`
   - 以及对照字体历史：
   - `3b2c0f1e`
   - `ee318757`
6. 当前现场文件：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
   - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `D:\Unity\Unity_learning\Sunset\Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `D:\Unity\Unity_learning\Sunset\Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`

## 允许的 scope

这轮只允许你读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Editor修复后Primary与字体owner复核-07.md`
2. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
4. `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
5. `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`

## 明确禁止

1. 不准碰 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`。
2. 不准碰导航线任何脚本。
3. 不准把字体 dirty 因为“现在看着也在飘”就直接吞成 `NPCV2 own`。
4. 不准修改这 3 份 `DialogueChinese*` 字体资产；这轮只允许只读排除归属。
5. 不准重开 `HomeAnchor` 新功能、scene 新改造或导航联调。
6. 不准进 Unity / MCP / Play Mode 再做 live 写。
7. 如果 `Primary.unity` 当前 dirty 仍是 mixed 且切不开，不准硬吞整张 scene cleanup。

## 完成定义

只有满足下面二选一，这轮才算有效完成：

### 结局 A：可切出 own residue，并完成最小 cleanup

1. 你明确证明 `Primary.unity` 当前 dirty 中确实包含你这条线自己的：
   - `HomeAnchor`
   - 或 `Inspector auto-repair`
   residue；
2. 这部分 residue 可以与 foreign diff 清楚分离；
3. 你只清这部分 own residue，并白名单收口；
4. 回执里明确说明剩余 mixed 面哪些不是你 own；
5. 三份 dirty 字体被你明确排除为 non-own。

### 结局 B：当前是 mixed hot 面，不能安全认领 cleanup

1. 你明确证明当前 `Primary.unity` dirty 无法切出纯 NPC own residue；
2. 你停止，不硬吞 cleanup；
3. 你把“哪些像是你的、哪些明确不是你的、为什么当前不能清”报实；
4. 三份 dirty 字体被你明确排除为 non-own；
5. 你本轮 own 路径 clean。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
`Primary.unity` 当前 dirty 是否包含可切出的 NPC own residue：`yes / no`
三份 `DialogueChinese*` dirty 是否属于 NPCV2 own：`yes / no`
若 yes，具体是哪一块：
若 no，当前 mixed / 排除依据：
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮不是让你吞整张 `Primary.unity`，也不是让你碰字体或导航 runtime；只把 `24886aad` 之后你自己到底还剩什么底盘 residue 报实，能切就最小清，切不开就明确停。
