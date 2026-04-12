# 2026-04-08｜给 Farm 线程｜箱子 E 键近身交互主刀 prompt 01

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
4. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1BedInteractable.cs`
6. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCDialogueInteractable.cs`

从这一条开始，你的唯一主刀固定为：

把“箱子支持近身 E 键交互”接进现有 runtime / input / proximity 主链。

不要把这件事再甩给 UI 线程主刀。

---

## 你必须继承的硬事实

1. `ChestController` 已经有真开箱入口：
   - `OnInteract()`
   - `OpenBoxUI()`
2. `GameInputManager` 已经有：
   - 点击箱子
   - 自动走近
   - 到点交互
   这条现成链
3. 当前真正缺的不是箱子 UI 面板，也不是开箱逻辑，而是：
   - 箱子没有接入近身 `E` 键 candidate / proximity / 提示抑制
4. UI 线程当前只保留配合边界：
   - 如果你后续需要补 `E 打开箱子` 的提示文案、提示壳样式或 overlay 表现，再回给 UI
   - 但 runtime 主链不要回丢给 UI

---

## 这轮唯一主刀

### 让箱子接入现有近身 E 键交互体系

完成定义至少包括：

1. 玩家站在箱子附近时，会出现统一近身交互提示
2. 按 `E` 能直接打开箱子
3. 鼠标右键“点击箱子 -> 自动走近 -> 到点打开”原链保持不坏
4. 两条链最终都复用箱子的同一个真入口
   - 不要新造第二套开箱逻辑
5. 如果箱子 UI 已打开，近身链要有合理抑制或不重复触发

---

## 允许的 scope

1. 箱子 `E` 键近身 candidate
2. proximity / 距离判定 / 优先级
3. 箱子已打开时的交互抑制
4. 必要的 runtime 提示接回
5. 必要测试

---

## 明确禁止事项

1. 不要重写箱子 UI
2. 不要重写 `ChestController` 的开箱真入口
3. 不要把箱子单独做成一套平行于现有 proximity 体系的新系统
4. 不要顺手泛修别的 UI
5. 不要把提示样式大改吞成你自己的主刀

---

## 推荐落地方式

目标体验固定为：

1. 远处：
   - 右键点箱子
   - 自动走近
   - 到点打开
2. 近处：
   - 出现统一 `E` 键提示
   - 按 `E` 直接打开
3. 两条链最后都汇到：
   - `ChestController.OnInteract()`

---

## 回执时必须写清

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要我/UI 线程现在做什么
7. 箱子 `E` 键链是否已接入 proximity 主链
8. 右键自动走近链是否保持正常

---

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
