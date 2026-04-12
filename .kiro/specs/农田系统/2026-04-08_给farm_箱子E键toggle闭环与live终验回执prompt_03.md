# 2026-04-08｜给 Farm 线程｜箱子 E 键 toggle 闭环与 live 终验回执 prompt 03

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\World\Placeable\ChestController.cs`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\ChestProximityInteractionSourceTests.cs`
4. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\2026-04-08_给farm_箱子E键近身交互主刀prompt_01.md`

从这一条开始，不要再重写箱子运行时主链。

当前已接受的基线固定为：

1. `ChestController` 已经接入 `SpringDay1ProximityInteractionService.ReportCandidate(...)`
2. 近身 `E` 键触发继续复用 `ChestController.OnInteract()`
3. `GameInputManager` 的右键“点击箱子 -> 自动走近 -> 到点开箱”旧链仍然保留
4. 现在 `OnInteract()` 已经被收成 toggle：
   - 同一个箱子已打开时，再次交互会关闭
5. `SpringDay1ProximityInteractionService` 已补 `AllowWhilePageUiOpen`
   - 只有“同一个箱子自己已打开”时，才允许 proximity 候选在 page UI 打开期间继续存活
6. `ChestProximityInteractionSourceTests.cs` 已把以上结构真值锁成源码守门

---

## 这轮唯一主刀

只做：箱子 `E` 键 toggle 的 fresh runtime/live 终验，必要时只补最小边界修正。

不要回到：

1. 重写 `ChestController` 的开箱真入口
2. 重写 `GameInputManager` 的自动走近链
3. 泛修箱子 UI 壳体
4. 把这轮又扩成农田大收口

---

## 你这轮至少要验的真实 case

1. 玩家靠近箱子时：
   - 出现统一近身提示
   - 按 `E` 直接打开箱子
2. 同一个箱子已经打开时：
   - 近身仍能看到关闭语义
   - 按 `E` 能直接关闭
3. 右键点击箱子时：
   - 近处仍能直接交互
   - 远处仍能自动走近后开箱
4. 打开同一个箱子后：
   - 不会因为 page UI 阻塞把 toggle 候选整个杀掉
5. 打开别的页面或离开交互距离后：
   - 不会留下错误提示残影

---

## 完成定义

这轮完成至少要能明确回答：

1. 箱子 `E` 键现在是否真的是 toggle
2. 它是否仍和旧的右键自动走近链共用同一个真入口
3. 当前还有没有 live 才能看到的边界 bug
4. 如果还有，第一真实 blocker 在哪

---

## 回执时强制区分

1. 已接受的结构基线
2. 这轮 fresh live 做成了什么
3. 现在还没做成什么
4. 当前 blocker
5. 下一步只做什么

不要把“source guard 已有”冒充成“live 已过线”。

---

## 固定回执格式

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要我/UI 线程现在做什么
7. 箱子 `E` 键 toggle 是否已 live 过线
8. 右键自动走近链是否仍正常

