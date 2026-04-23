# 给 spring-day1 的状态说明

这不是施工命令，也不是要你停下当前主线。

这是 UI 线程基于刚刚的只读彻查，给你的一个 owner 边界与协作状态说明，目的是避免我们同时去碰第三点，把时间链继续写乱。

## 我这边已经确认的边界

### 1. “超过凌晨 2:00 的玩家稳定回家睡觉”整体应由你主刀

我这边把真源链读通后，判断是：

- `TimeManager.cs`
  - 负责全局时钟真源
  - 负责到点后触发通用 `Sleep()`
  - 不负责 Day1 的剧情收束、玩家摆位、NPC 回家、DayEnd 相位

- `SpringDay1Director.cs`
  - 已经订阅 `TimeManager.OnSleep += HandleSleep`
  - `HandleSleep()` 才是真正处理 Day1 特有“怎么睡 / 能不能睡 / 睡到哪 / 谁回家 / 是否进 DayEnd”的地方
  - FreeTime 下到 `hour >= 26` 时，也已经是你这边主动调用 `TimeManager.Sleep()`

所以第三点整体不是 UI 线程 own，也不该由我继续碰。

### 2. 我不会再碰你这几块

我后续不会再去动下面这些 Day1 真逻辑：

- `SpringDay1Director.HandleSleep()`
- `SpringDay1Director.RecoverFromInvalidEarlySleep()`
- `SpringDay1Director.CanFinalizeDayEndFromCurrentState()`
- Day1 的 time guardrail / FreeTime 压力提示 / DayEnd 收束
- 玩家回住处、居民回 anchor、剧情角色回 anchor

## 我这边会自己收掉什么

### 1. 我会自收 `TimeManagerDebugger.cs` 的 `+ / -` 调试跳时语义

我已经确认，我上轮把 `+ / -` 改错了：

- 当前被我改成了“保留分钟”
- 这会让 `14:37 +` 真实落到 `15:37`
- 也会让跨过 `26` 时出现 `Sleep()` 后再补分钟的错误语义

这部分是我 own 的调试输入 / HUD 范围，我会自己修回去，不把它丢给你。

### 2. 我会把它收成不再干扰你验 Day1 的状态

我准备收成的目标是：

- 普通 `+` / `-` 回到整点跳转语义
- 不再把分钟保留带进 Day1
- 跨 `26` 点边界时仍然走 `TimeManager.Sleep()` 真链

也就是说，我会把“调试快进语义”修回稳定，不再让这条误导你对 Day1 夜间收束的 live 判断。

## 你这边可以继续放心主刀什么

基于现代码，我认为你这边继续 own 的内容包括：

- Day1 时间锁的整体设计与真源
- 各 phase 的最小 / 最大允许时间
- `09:00 / 16:59 / 18:00 / 19:00 / 19:30 / 22:00 / 24:00 / 25:00 / 26+` 这些节点在 Day1 的真实含义
- FreeTime 到 DayEnd 的收束
- 非法早睡拦截
- 超过 2 点后玩家稳定回住处睡觉
- 居民 / 剧情角色回 home anchor

## 我这边给你的一个提醒

在我把 `TimeManagerDebugger +/-` 修回之前，当前这条调试跳时语义不是权威基线。

如果你这轮在 live 里用 `+ / -` 看 Day1 夜间收束，请先把它当成“已知被 UI 线程误改过”的调试输入，不要把它直接当成 Day1 真逻辑已经错乱的唯一证据。

## 结论

一句话就是：

- 第三点整体你主刀
- 我只修我自己误改出来的 `TimeManagerDebugger +/-`
- 我不会再继续伸手碰你的 `SpringDay1Director` 睡觉 / DayEnd 逻辑

