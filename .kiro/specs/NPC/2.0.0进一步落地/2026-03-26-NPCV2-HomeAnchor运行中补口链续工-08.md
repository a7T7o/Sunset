# 2026-03-26-NPCV2-HomeAnchor运行中补口链续工-08

你现在继续作为 `NPCV2`，这轮请以你自己最新自述里的主线判断为准：

> 当前真主线不是 cleanup，而是把运行中的 `Home Anchor` 补口链真正接上，让 Inspector 里不再是空。

这轮不再把你拉回 `Primary.unity` mixed owner 报实主刀，也不让你去碰导航 runtime。

## 当前已接受的基线

1. 你已经进 `main` 的两刀成立：
   - [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 里的 `001/002/003 HomeAnchor` 最小集成；
   - [NPCAutoRoamControllerEditor.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPCAutoRoamControllerEditor.cs) 的 `Play Mode -> MarkSceneDirty` 报错修复。
2. 用户当前肉眼看到的真实阻塞点是：
   - 运行中的 Inspector 里 `Home Anchor` 仍然可能是空。
3. 当前这条主线最接近用户体验，而且只需要锁 Editor / Inspector 补口链，不必先回头 cleanup。
4. mixed hot 面仍然存在，但这轮不是它的处理窗口：
   - [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 整张 mixed diff 先不吞；
   - 3 份 `DialogueChinese*` 字体先不碰；
   - [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 的 runtime / 导航 diff 不归你这轮。

## 这轮唯一主刀

只做：

> 继续窄修 [NPCAutoRoamControllerEditor.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPCAutoRoamControllerEditor.cs) 的运行中 `Home Anchor` 补口链，让 Inspector 里的 `Home Anchor` 从空变成非空；如果还做不到，就把断点继续压窄到可见事实

更具体地说，这轮只允许在 Editor / Inspector 侧回答下面这些问题：

1. 运行时自动补口到底有没有找到 anchor？
2. 找到后到底有没有对当前 controller 调用 `SetHomeAnchor(...)`？
3. 赋值后为什么 Inspector 仍可能显示为空？
4. 如果它还空，问题到底是：
   - anchor 没找到；
   - 赋值没发生；
   - 赋值发生了但 Inspector / Serialized 刷新没跟上；
   - 还是别的更小断点。

## 先完整读取

1. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工详细汇报-04.md`
4. `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
5. 如确属必要，只读：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`

## 允许的 scope

这轮只允许你读取和必要时修改这些范围：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-HomeAnchor运行中补口链续工-08.md`
2. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
4. `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
5. 如确属必要，只读：
   - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`

## 明确禁止

1. 不准碰 [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) 的 runtime 导航逻辑。
2. 不准碰 `Primary.unity` 的 mixed cleanup。
3. 不准碰 3 份 `DialogueChinese*` 字体。
4. 不准把这轮扩成 owner 报实大盘、scene 新功能或导航联调。
5. 不准为了取证重开 broad live；只允许围绕当前运行中的 Inspector / 补口链做最小确认。

## 完成定义

只有满足下面任一结局，这轮才算有效完成：

### 结局 A：运行中的 `Home Anchor` 补口链接上

1. 你已让 `001/002/003` 在运行中的 Inspector 里 `Home Anchor` 不再是空；
2. 不再出现 `MarkSceneDirty during play mode` 报错；
3. 你已明确说明这轮靠什么条件生效；
4. 你没有把刀漂回 cleanup 或 runtime 导航。

### 结局 B：仍未接上，但断点被继续压窄

1. `Home Anchor` 仍可能为空；
2. 但你已把问题缩成一个更小的可见断点，例如：
   - 只剩 anchor 查找失败；
   - 只剩 `SetHomeAnchor` 未命中；
   - 只剩 Inspector 刷新问题；
3. 你没有碰 `Primary.unity` mixed cleanup、字体或导航 runtime。

## 验证纪律

1. 以“当前运行中的 Inspector 可观察事实”为第一证据，不先做 cleanup。
2. 如果需要补可观察性，也只能补在 `NPCAutoRoamControllerEditor.cs` 这一层。
3. 一旦拿到足够证据，立刻停，不要顺手扩到 scene / runtime 导航。

## 固定回执格式

```text
已回写文件路径：
当前在改什么：
运行中的 `Home Anchor` 当前状态：`non-empty / still-empty`
当前最小断点：
changed_paths：
当前 own 路径是否 clean：
blocker_or_checkpoint：
一句话摘要：
```

## 一句话总口径

这轮先把用户眼前这个“运行中的 Inspector 里 `Home Anchor` 还是空”的问题打穿；在它没接上前，不回 cleanup，不碰导航 runtime。
