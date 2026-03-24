# 004-prompt-4

你这轮我先给明确裁定：

- `NpcAvoidsPlayer = pass`，接受
- `NpcNpcCrossing = pass`，接受
- `PlayerAvoidsMovingNpc = fail`，但失败证据有效，接受

也就是说，这条线已经不是“大面积不通”，而是已经收到了：

- 只剩一个场景没过
- 而且这个场景也不是完全不动了

你现在真正剩下的 blocker 很具体：

- `PlayerAvoidsMovingNpc` 里，玩家先到位了，但移动中的 `001` 在后半程收尾不稳

## 一、这轮首先纠偏一件事

你已经说清楚了：

- 之前那种“玩家飞来飞去”的观感，主要来自 full-run harness 在不同场景之间反复摆位

这一步我认账。  
但从现在开始，下一轮明确禁止再跑那种串行 full-run 方式。

## 二、这一轮只允许做单场、最小复现

下一轮只准保留这一条场景：

- `PlayerAvoidsMovingNpc`

只允许用最小复现链：

- `Probe Setup/Player Avoids Moving NPC`

不再跨场景连跑，不再让角色在三条场景之间来回搬运。

## 三、这一轮主线已经进一步收缩

你现在不能再把主线表述成：

- 继续查统一导航哪里还不对

你现在的主线必须写死成：

- `001` 在玩家已到位后，为什么后半段还不能稳定收尾

而且你自己已经给出了当前最大嫌疑：

- [NavigationLocalAvoidanceSolver.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavigationLocalAvoidanceSolver.cs)
- 具体是它对“静止 blocker / sleeping blocker”的裁决分支

所以这轮不要再回头查结构层，也不要重新大面积扫控制器层。

## 四、这轮的第一优先级不是继续铺日志，而是锁死 solver 分支

你这轮必须正面回答：

1. 玩家到位后，`001` 看到的 blocker 到底是什么
2. solver 是否把已到位玩家持续当成高优先 stationary blocker
3. `001` 的后半程为什么只是慢蹭而不是稳定绕开 / 收尾
4. 第一责任点到底是不是：
   - stationary blocker 裁决
   - sleeping blocker 裁决
   - clearance / stop distance 判定
   - 还是 path-end 收尾条件

也就是说：

- 这轮可以还没完全修通
- 但不能继续停在“我怀疑是 solver 某个分支”

你必须把它压到一个具体方法 / 具体条件分支。

## 五、这轮允许改的范围

默认只允许优先动：

- `NavigationLocalAvoidanceSolver.cs`

只有在你拿到强证据证明 solver 改完还不够时，才允许再回到：

- `PlayerAutoNavigator.cs`
- `NPCAutoRoamController.cs`

并且如果回到这两处，必须明确说清：

- solver 为什么不是第一责任点
- 你为什么要回退到控制器层

## 六、这一轮先做的 hygiene

你回执里明确提到：

- `Primary.unity` 当前 `isDirty = true`
- 你没有保存 scene

这轮第一步先把这件事说清楚：

1. 这个 dirty 是不是 probe setup / harness 摆位留下的
2. 你这轮是否需要保存 scene
3. 如果不需要保存，就不要把 harness 摆位混进 scene 落盘

换句话说：

- 继续取证可以
- 但不要把验证现场脏改悄悄变成 scene 正式改动

## 七、通过条件

这轮只接受两种结果：

### 结果 A：最后一条也通过

如果 `PlayerAvoidsMovingNpc` 通过了，而且另外两条保持通过：

- 不要继续补丁式推进
- 直接新增一份阶段验收报告

建议路径：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\阶段验收报告_2026-03-24.md`

### 结果 B：还没完全过，但第一责任点锁死

如果还没完全过，也可以接受。  
但前提是你必须把 blocker 压缩成：

- 某个具体文件
- 某个具体方法
- 某个具体条件分支

不能再停在“现在第一嫌疑大概是 solver”。

## 八、本轮回执格式

下一次回执，只按这个格式回复：

- 当前在改什么：
- 当前是否只保留单场 `PlayerAvoidsMovingNpc` 复现：
- `Primary.unity isDirty` 的来源是否已查清：
- 是否已锁定第一责任点：
- 第一责任点位于哪个文件 / 方法 / 条件分支：
- 是否已完成代码修复：
- `PlayerAvoidsMovingNpc` 的最新真实结果：
- 另外两条场景是否保持通过：
- 当前剩余 blocker：
- blocker_or_checkpoint：
- 一句话摘要：

## 九、最后再钉一次

你这轮已经把问题从“大范围不稳”收成了“只剩最后一条后半段收尾不稳”。  
这其实已经很接近了。

但越到这里越不能再 full-run 乱飞。  
下一轮就盯这一条，把 solver 分支钉死。
