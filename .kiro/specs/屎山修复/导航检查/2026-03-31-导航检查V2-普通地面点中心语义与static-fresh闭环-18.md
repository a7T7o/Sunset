# 2026-03-31-导航检查V2-普通地面点中心语义与static-fresh闭环-18

这轮不要再继续沿 crowd、moving、终点有 NPC 停留、被动 blocker 或大架构发散。

父线程自己的 static fresh 已经重新跑完，
而且这次不是“没跑起来”，是已经跑到了 `case_end / all_completed`。

所以你这轮唯一主刀已经改判为：

- 只锁 `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
- 只收“普通地面点导航到底按什么位置语义算到点”这一刀
- 不碰 `NavigationStaticPointValidationRunner.cs`
- 不碰 `NavigationStaticPointValidationMenu.cs`
- 不碰 crowd / NPC / solver / `NavigationPathExecutor2D.cs`

---

## 一、父线程这次 fresh 已经钉死的事实

### 1. static runner 现在已经不是第一阻塞

父线程已经补了两刀静态线 own scope：

1. `NavigationStaticPointValidationMenu.cs`
   - static validation 运行前临时关掉 `Console Error Pause`
   - 运行结束恢复原值
   - 这样 `GridEditorUtility.cs` 的 editor-only frustum error 不再把 Play 直接暂停卡死
2. `NavigationStaticPointValidationRunner.cs`
   - `ResetPlayerToRunStart()` 改成每个 case 现算 `runStartActorPosition`
   - 不再在 `StartRun()` 时把起点 actor 位置冻结死

这两刀之后，fresh static run 已能稳定走完：

- `runtime_launch_request`
- `runner_started`
- `case_start`
- `case_end`
- `all_completed`

所以这轮不要再把主刀回漂到 static runner。

### 2. `case_start origin` 已经恢复正常

这轮最新 fresh：

- `StaticPointCase1 case_start origin=(-8.16, 7.38)`
- `StaticPointCase2 case_start origin=(-8.16, 7.38)`

这说明父线程刚修的 runner 起点链已经把“origin=-16.33,15.96”那层异常收掉了。

### 3. 现在剩下的是普通点 runtime 语义错层，不是 runner 起点错层

最新 fresh 结果：

- `StaticPointCase1`
  - `pass=False`
  - `centerDistance=1.116`
  - `rigidbodyDistance=0.155`
  - `transformDistance=0.155`
  - `target=(-6.56, 7.38)`
  - `resolved=(-6.56, 7.38)`
- `StaticPointCase2`
  - `pass=False`
  - `centerDistance=1.040`
  - `rigidbodyDistance=0.161`
  - `transformDistance=0.161`
  - `target=(-8.16, 8.98)`
  - `resolved=(-8.16, 8.98)`

同一轮里：

- path 请求对了
- resolved 终点对了
- `Transform / Rigidbody` 已经靠到目标点附近
- 但 `ColliderCenter` 仍稳定高出约 `1.04 ~ 1.12`

这说明现在真正没收的是：

- 普通地面点导航的“到点语义 / 完成语义 / 目标语义”
- 它当前仍更像按 `Transform / Rigidbody` 收口
- 而不是按玩家实际占位中心收口

---

## 二、这轮唯一主刀

只做这一件事：

把 `PlayerAutoNavigator.cs` 里“普通地面点导航”的到点语义重新收回到玩家实际占位中心，
并且 fresh 证明它真的能让 static point accuracy 过线。

你这轮不是去解决 crowd，
也不是继续碰 detour owner、passive blocker、终点 NPC 停留。

只收普通地面点。

---

## 三、允许怀疑的最窄热区

只允许怀疑这类分支：

1. `GetPlayerPosition()`
2. `HasReachedArrivalPoint()`
3. `TryFinalizeArrival(...)`
4. 普通点导航与跟随目标导航共用的“终点判定 / stopDistance / target 位置语义”分流

你必须明确区分：

- 普通地面点导航
- 跟随 / 交互目标导航

不要再把这两套语义混在一起。

当前最像的问题是：

- 普通点导航当前被收成了 `Transform / Rigidbody` 语义
- 但 static fresh 已证明用户关心的仍是 `ColliderCenter` 语义
- 所以最终会出现：
  - 路径终点正确
  - 玩家脚底或刚体位置接近目标
  - 但实际占位中心仍高出一截

---

## 四、这轮明确禁止

1. 不准再改 `NavigationStaticPointValidationRunner.cs`
2. 不准再改 `NavigationStaticPointValidationMenu.cs`
3. 不准回漂 crowd / moving / passive blocker / NPC
4. 不准顺手修 `GridEditorUtility.cs` 或别的 editor 噪声
5. 不准把 follow target / interaction target 的 stop 语义一起重写
6. 不准再拿“结构上差不多”代替 fresh 结果

---

## 五、执行顺序固定

1. 先只读确认 `PlayerAutoNavigator.cs` 里普通点 vs 跟随目标的当前位置语义
2. 只补 1 刀最小 runtime 分支
3. fresh compile
4. 只跑 1 次：
   - `Tools/Sunset/Navigation/Run Static Point Accuracy Validation`
5. 只回答这 5 件事：
   - `case_start origin` 是否仍正常
   - `StaticPointCase1` 当前 pass / fail
   - `StaticPointCase2` 当前 pass / fail
   - `centerDistance / rigidbodyDistance / transformDistance` 是否已经收拢到同一语义
   - 当前剩余问题是否还在 `PlayerAutoNavigator.cs`

---

## 六、完成定义

### 结局 A：普通点中心语义 fresh 过线

你必须给出：

1. `StaticPointCase1 pass=True`
2. `StaticPointCase2 pass=True`
3. `centerDistance` 收到可接受阈值内
4. 不再出现“Transform/Rigidbody 到点但 ColliderCenter 高出 1.x”这种错层

### 结局 B：仍 fail

你必须给出：

1. 当前最新 fresh 数值
2. 它现在更像哪一个分支仍在偷用旧语义
3. 为什么这次不能 claim done

---

## 七、固定回执格式

### A1. 用户可读汇报层

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

1. 当前在改什么
2. 这轮具体改了 `PlayerAutoNavigator.cs` 的哪几个普通点到点分支
3. fresh compile / console 结果
4. `StaticPointCase1` 结果
5. `StaticPointCase2` 结果
6. `centerDistance / rigidbodyDistance / transformDistance` 关键值
7. `case_start origin` 是否正常
8. 当前第一责任点是否仍在 `PlayerAutoNavigator.cs`
9. changed_paths
10. 当前 own 路径是否 clean
11. blocker_or_checkpoint
12. 一句话摘要

---

## 八、一句话提醒

父线程已经把 static runner 从“跑不起来”推进到了“跑完并给出 fresh verdict”。

你这轮不要再碰验证工具链，
只把普通地面点导航的中心语义收回来。
