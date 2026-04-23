# 给 spring-day1 的状态说明

这不是施工命令，也不是要你停下当前主线。

这是 UI 线程在 `prompt_12` 这一刀完成后的 owner 告知，目的是把“我已经自收了什么、后面还会继续 own 什么、我不会再碰什么”说清楚，避免我们再次在 Day1 UI 和真逻辑上互相越界。

## 我这边这轮已经自收的内容

### 1. `TimeManagerDebugger +/-` 已回到 UI own 的整点语义

我已经把下面这条收回来了：

- [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)

当前语义是：

- `+ / -` 只跳整点
- 普通小时跳转落到 `xx:00`
- 跨过 `26` 时直接走 `TimeManager.Sleep()` 真链
- 不再保留分钟
- 不再出现 `Sleep()` 之后再把 minute 补回去的假语义

也就是说，这条调试输入不该再继续干扰你对 Day1 夜间收束的 live 判断。

### 2. 任务清单下方的 bridge prompt 已收回任务卡语义链

我已经把下面这条收掉了：

- [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)

当前 UI 壳关系变成：

- `BridgePromptRoot` 不再挂在 overlay 顶层漂浮
- 它现在从属于 `TaskCardRoot`
- 位置稳定挂在任务卡下方
- 会随任务卡壳体一起重建、一起布局、一起退让

这条改动的目的不是改你 Day1 的文案真值，而是把“玩家真正看到的主次关系”收平。

### 3. workbench 也进了 prompt 的 modal block 判定

我已经把下面这条补上了：

- [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs)

现在背包 / 箱子 / 工作台这条 modal 退让语义，PromptOverlay 会按同一口径处理，不再只认 package / box。

## 我后面继续 own 的范围

接下来如果用户继续让我收 Day1 玩家面，我 own 的还是这些：

- 任务清单玩家可见壳体
- bridge prompt / 次级提示
- PromptOverlay 的 modal / dialogue / re-entry 可见面
- `TimeManagerDebugger +/-` 这类 UI own 的调试输入语义

如果后续你发现的是：

- 文案真值不对
- phase / task / prompt canonical state 不对
- Day1 在某个时点根本不该显示任务清单 / 不该允许恢复

那仍然应该由你给 canonical state，我再接 UI 壳。

## 我不会再碰的内容

我后续不会去动下面这些 Day1 真逻辑：

- `SpringDay1Director.HandleSleep()`
- `RecoverFromInvalidEarlySleep()`
- `CanFinalizeDayEndFromCurrentState()`
- Day1 的 `20:00 / 21:00 / 26:00` 夜间状态机
- resident / actor 的 anchor / staging / release

## 当前我这边的状态

- 代码层：这轮 own 改动没有看到 owned red
- Unity 侧：`validate_script` 目前只能拿到 `unity_validation_pending`
- 原因：Unity 现场卡在 `playmode_transition / stale_status`
- 所以我现在能诚实 claim 的只有：
  - 代码层自检站住了
  - Day1 真逻辑没有被我继续越权改
  - 玩家面 live 体验仍待用户手测

## 一句话结论

- UI 线程已经把 Day1 玩家面这条 own 继续往前收了一刀
- 这刀只碰玩家可见 UI contract 和 `TimeManagerDebugger +/-`
- 我没有再碰你 Day1 的睡觉 / DayEnd / anchor 真逻辑
