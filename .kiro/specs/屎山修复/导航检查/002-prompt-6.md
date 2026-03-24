# 002-prompt-6

你这轮我先给明确裁定：

- `用户补充区已吸收`，接受
- `单场短窗`，接受
- `Primary.unity isDirty 来源已查清`，接受
- `第一责任点已从 solver 前移到 NPC detour 恢复链`，接受

也就是说，这轮不是“还在乱怀疑”，而是已经把问题继续压实了。

当前这条线的真实状态已经进一步收缩成：

- 玩家已经能先到位
- 净空已经转正
- 当前失败不再是“继续推着撞”
- 当前失败是：
  - `001` 在 detour 后没有稳定恢复原目标收尾，最后围着旧 detour 区摆动到超时

这一步是有效推进，不是原地踏步。

## 一、当前真实结论

你现在的真实结论不再是：

- `NavigationLocalAvoidanceSolver` 还需要继续泛泛调参数

你现在的真实结论应该写死成：

- solver 参数这刀已经把失败形态从“接触推挤”压成了“正净空但收尾超时”
- 当前第一责任点已经前移到：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
  - `TryHandleSharedAvoidance()`
  - 具体是：
    - `shouldAttemptDetour && TryCreateDynamicDetour(...)`
    - 以及其后 `OverrideWaypoint` 清理 / 恢复原目标路径的收尾链

所以从现在开始，下一轮不要再回头主要打 solver。

## 二、这轮禁止继续做的事

这轮明确禁止：

- 再继续泛泛抬高 sleeping / stationary blocker 参数
- 再把主线退回 `NavigationLocalAvoidanceSolver` 大范围调参
- 再跑长时间多场 full-run
- 再只做单场但不给 detour 恢复链的具体结论

你现在应该打的是：

- `NPC detour 恢复链 root-cause`

## 三、这一轮的唯一主线

这轮唯一主线写死为：

- `为什么 001 在玩家已到位、净空已为正后，仍没有清掉旧 detour 并恢复原目标路径`

换句话说，你要查穿的是：

1. dynamic detour 什么时候创建
2. detour 什么时候应该清掉
3. detour 实际有没有清掉
4. `OverrideWaypoint` 什么时候应该恢复原目标
5. 恢复原目标后，是否真的触发了新的有效路径推进
6. 为什么当前会出现：
   - `npcAState=Moving`
   - 但仍围着旧 detour 区摆动到超时

## 四、建议排查硬顺序

按这个顺序来，不要乱跳：

1. 锁 `NPCAutoRoamController.TryHandleSharedAvoidance()`
   - detour 创建条件
   - detour 保留条件
   - detour 清理条件
2. 锁 `OverrideWaypoint`
   - 它何时被设置
   - 何时被清除
   - 清除后是否真的恢复原目标
3. 锁 NPC 当前目标恢复链
   - detour 结束后，NPC 当前路径 / 当前目标 / waypoint index 到底变成了什么
4. 如有必要，再核：
   - `TryRebuildPath()`
   - `TryBeginMove()`
   - 对 `NavigationPathExecutor2D.BuildPathResult.ActualDestination` 的消费
5. 最后才做最小必要修复

## 五、你这一轮必须正面回答的问题

下一轮你必须明确回答：

1. detour 当前是“没清掉”还是“清掉了但没恢复原目标”
2. `OverrideWaypoint` 当前是：
   - 没被清除
   - 被清除了但没触发新路径
   - 还是清除了却恢复到了错误目标
3. `npcReached=False` 的第一责任点到底是哪一个：
   - detour 清理条件没达成
   - 原目标恢复条件没达成
   - 新路径没重建
   - 重建了但仍沿旧 waypoint 摆动
4. 第一责任点是否已经能压成：
   - 某个具体方法
   - 某个具体条件分支
   - 某个具体状态变量没切回来

## 六、这轮允许改的范围

默认优先允许改：

- `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`

如果你拿到强证据证明这里只是表象，才允许回到：

- `NavigationPathExecutor2D.cs`
- 或 NPC 对 `ActualDestination` 的消费链

但如果你回退到这些文件，必须明确解释：

- 为什么 `TryHandleSharedAvoidance()` 不是第一责任点

## 七、这一轮的完成标准

这轮只接受两种结果：

### 结果 A：修通最后一条

如果你修通了 `PlayerAvoidsMovingNpc`：

1. 先重跑单场确认
2. 再补跑：
   - `NpcAvoidsPlayer`
   - `NpcNpcCrossing`
3. 只有三条都过，才允许转阶段验收报告

注意：

- 本轮回执里另外两条 `pass` 只是“最近一次短窗仍为通过”
- 还不是本轮修复后的新鲜同轮证据
- 所以如果你宣称最后一条修通，必须把另外两条也补成同轮 fresh 结果

### 结果 B：还没彻底修通，但 detour 恢复链已锁死

也可以接受。  
但前提是你必须交出：

- 更具体的方法
- 更具体的条件分支
- 更具体的状态变量 / 清理条件 / 恢复条件

不接受继续停在：

- “现在大概是 detour 恢复链有问题”

## 八、本轮 live 验证要求

调试阶段仍然只允许：

- 单场 `PlayerAvoidsMovingNpc`

只有当你确认已经修通最后一条后，才补跑另外两条作新鲜验收证据。

## 九、本轮回执格式

下一次回执，只按这个格式回复：

- 当前在改什么：
- 当前是否仍保持单场短窗调试：
- detour 当前是“没清掉 / 没恢复原目标 / 恢复了但没重建路径 / 仍沿旧 waypoint 摆动”中的哪一种：
- 第一责任点位于哪个文件 / 方法 / 条件分支：
- 是否已完成代码修复：
- `PlayerAvoidsMovingNpc` 最新真实结果：
- 如果已修通，`NpcAvoidsPlayer / NpcNpcCrossing` 的同轮 fresh 结果：
- 当前剩余 blocker：
- blocker_or_checkpoint：
- 一句话摘要：

## 十、最后再钉一次

你这轮已经把问题从：

- “为什么还在撞”

推进成了：

- “为什么绕开后没有恢复原目标收尾”

这已经是很实的收口了。  
下一轮不要再发散，直接把 `001` 的 detour 恢复链钉死。
