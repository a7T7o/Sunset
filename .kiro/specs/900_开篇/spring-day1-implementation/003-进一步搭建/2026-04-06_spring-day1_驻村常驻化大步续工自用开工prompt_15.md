请先完整读取：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_驻村常驻化大步续工执行清单_14.md]

从这一条开始，不再回到方案、提醒、判断复读或轻量试探。

直接按 `14` 号执行清单进入真实施工，并在本轮内把这条线推进到你当前证据和边界下能推进的最深处。

这轮唯一总目标固定为：

- 把 `day1` 从“导演工具已可用、剧情数据写回了一部分”推进到“后半段 crowd 的存在方式正式改成驻村常驻化，并继续用导演工具把多个真实段落压成可消费数据和可资产化文本”。

这轮必须同时推进两组：

1. `A组`：resident deployment / director runtime 承接
2. `B组`：剧情与导演数据真实落地

硬要求如下：

1. 先做 `CrowdDirector` 的 resident deployment 第一刀  
   - 不允许再把 `101~301` 默认当成“到 cue 才 instantiate 的临时演员”
   - 先以 `Primary` 代理 resident 承接站住
   - 第一批至少优先吃：
     - `EnterVillageCrowdRoot`
     - `KidLook_01`
     - `NightWitness_01`
   - 如果还能继续，再吃：
     - `DinnerBackgroundRoot`
     - `DailyStand_01`

2. resident 化后，立刻继续用导演工具生产更多真实 cue  
   - 至少尽量继续压：
     - `EnterVillage_PostEntry`
     - `DinnerConflict_Table`
     - `ReturnAndReminder_WalkBack`
     - `FreeTime_NightWitness`
     - `DayEnd_Settle`
     - `DailyStand_Preview`
   - 不允许只改 deployment 不继续产数据

3. 不允许只做 runtime，不继续压剧情正式产物  
   - 至少尽量继续推进：
     - `CrashAndMeet`
     - `EnterVillage`
     - `HealingAndHP / WorkbenchFlashback / FarmingTutorial`
     - `DinnerConflict / ReturnAndReminder / FreeTime / DayEnd`
   - 能落对白资产就落对白资产，能落正式稿就落正式稿，能挂到事件/导演消费链就挂进去

4. 把“正式剧情不可重复触发”当成本轮硬约束，不允许漏掉  
   - 只要某段正式剧情已经走过，就不能再次以正式剧情身份重播
   - 只要某个任务阶段已经完成，就不能再次触发同一正式推进
   - 只要某个 NPC 的正式剧情聊天已经消费过，再次对话只能走：
     - 闲聊
     - resident 日常对话
     - phase 后的非正式补句
   - 不允许再次回到那段已消费的正式剧情
   - 这不是小尾巴，而是 runtime 语义硬要求；如果不守住，就是 bug
   - 所以这轮如果你动到剧情状态、对话入口、phase 推进、director beat 消费链，必须顺手把这条“单次消耗、不重入”的约束一并落稳

5. 本轮优先顺序固定  
   - 先 `deployment`
   - 再 `director data`
   - 再 `剧情正式产物`
   - 再 `验证与收尾`

6. 这轮允许你走大一步，但不允许漂  
   - 可以一轮内同时改 `deployment + director data + dialogue assets`
   - 可以一次推进多个 phase
   - 但不要碰：
     - `UI`
     - `Town.unity`
     - `Primary.unity`
     - `GameInputManager.cs`
     - NPC own 气泡/会话底座

7. no-red 纪律继续严格执行  
   - 每完成一个最小责任簇，就做最小自检
   - 一旦 own red，立即止血
   - 不准带红继续扩写

8. 并行线程不是摆设  
   - `Town` 会并行补 resident root / anchor / scene-side contract
   - `NPC` 会并行补常驻居民语义矩阵
   - 你自己仍是最后的导演总整合位，不要把关键判断甩出去

9. 这轮停止的唯一合法理由  
   - 命中真实 blocker：
     - resident deployment 第一刀撞到真实结构断点
     - 工具消费 resident actor 时出现明确 runtime 断点
     - 剧情资产落地被真实引用链卡住
   - 否则不许提前停

10. 收尾必须做  
   - 最小 no-red 自检
   - 必要的菜单/测试/console 验证
   - 更新子工作区 / 父层 / 线程 memory
   - 更新 skill-trigger-log 并做健康检查
   - 如果达到可提交状态，提交这轮 own 成果

11. 最终给用户的汇报必须先说人话  
   - `当前主线`
   - `这轮实际做成了什么`
   - `现在还没做成什么`
   - `当前阶段`
   - `下一步只做什么`
   - `需要用户现在做什么`
   - 然后再给技术审计层

这轮不要再把导演工具当成果本身。
这轮要把它当生产工具，狠狠干出一个真正的大 checkpoint。
