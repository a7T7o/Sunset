# 2026-03-26-NPCV2-Primary归属报实与最小cleanup委托-06

你现在继续作为 `NPCV2`，但这轮不是继续做 `HomeAnchor` 新功能，也不是重开 `Primary.unity` scene 集成。

## 当前已确认的事实

1. 你在提交 `24886aad` 中修掉了：
   - `NPCAutoRoamControllerEditor.cs` 在 `Play Mode` 里误用 `MarkSceneDirty` 的问题。
2. 当前 working tree 仍存在：
   - `Assets/000_Scenes/Primary.unity`
   - 以及若干非你本轮 commit 直接涉及的脏面。
3. 当前不能把“你修过 Editor 报错”直接偷换成“现在所有 dirty 都是你 own 的”。

## 这轮唯一主刀

只做：

> 对 `Primary.unity` 当前 dirty 做 NPC 线 own residue 报实；如果且仅如果能明确切出你这条线自己的 `HomeAnchor / Inspector auto-repair` 残留，再做最小 cleanup

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工详细汇报-04.md`
4. 提交：
   - `65e1ee35`
   - `24886aad`
5. 当前现场文件：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
   - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`

## 允许的 scope

这轮只允许你读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Primary归属报实与最小cleanup委托-06.md`
2. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
4. `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
5. `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`

## 明确禁止

1. 不准碰 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`。
2. 不准碰导航线任何脚本。
3. 不准碰 `TMP` 字体资产。
4. 不准重开 `HomeAnchor` 新功能或 scene 新改造。
5. 不准进 Unity / MCP / Play Mode 再做 live 写。
6. 如果 `Primary.unity` 当前 dirty 是 mixed 且切不开，不准硬吞整张 scene cleanup。

## 完成定义

只有满足下面二选一，这轮才算有效完成：

### 结局 A：可切出 own residue，并完成最小 cleanup

1. 你明确证明 `Primary.unity` 当前 dirty 中确实包含你这条线自己的：
   - `HomeAnchor`
   - 或 `Inspector auto-repair`
   残留；
2. 这部分残留可以与 foreign diff 清楚分离；
3. 你只清这部分 own residue，并白名单收口；
4. 回执里必须明确剩余 mixed 面哪些不是你 own。

### 结局 B：当前是 mixed hot 面，不能安全认领 cleanup

1. 你明确证明当前 `Primary.unity` dirty 无法切出纯 NPC own residue；
2. 你停止，不硬吞 cleanup；
3. 你把“哪些像是你的、哪些明确不是你的、为什么当前不能清”报实。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
`Primary.unity` 当前 dirty 是否包含可切出的 NPC own residue：`yes / no`
若 yes，具体是哪一块：
若 no，当前 mixed 的原因：
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮不是让你吞整张 `Primary.unity`，只让你把 NPC 线自己在 `HomeAnchor / Inspector auto-repair` 上到底有没有残留讲清楚；能切就最小清，切不开就明确停。
