# 2026-04-15｜spring-day1｜Day1 历史语义、系统边界、现状问题与重构交接总文档

## 1. 这份文档是什么

这不是续工 prompt。

这也不是阶段汇报、检讨书，或者只针对某一刀的局部说明。

这份文档的目标只有一个：

`把 Day1 这条线从“用户真正要什么”“代码现在到底长成什么鬼样子”“为什么一直反复出新坏相”“最终正确架构应该怎么拆”“接班重构应该从哪一刀开始”一次性彻底说清。`

后续如果要：

1. 起新线程做正式重构
2. 让别的线程接班
3. 审核这条线为什么老出反效果
4. 判断哪些历史补丁要保留、哪些必须推倒

都应优先读这份文档，而不是再去拼长聊天。

---

## 2. 一句话总判断

Day1 这条线一直反复出问题，不是因为用户语义不清，也不是因为单个参数难调，而是因为我把 `SpringDay1Director` 和 `SpringDay1NpcCrowdDirector` 都写越界了。

最核心的错误只有一条：

`SpringDay1NpcCrowdDirector` 被我写成了“剧情后 resident 生命周期 owner”，而它本来根本不该拥有这个权力。

用户已经用 live 反证把这件事钉死：

1. `CrowdDirector` 开着时，Town resident 会出现高频小幅抖动、罚站、假 roam、性能爆炸等坏相
2. 直接关掉 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 后，NPC 反而恢复正常 roam，且这轮甚至会自己回 anchor

这说明：

1. 根病不是“NPC 自己不会动”
2. 根病是 `CrowdDirector` 仍在剧情后反复抓 resident
3. 继续补丁只能止血，不能当最终解法
4. 下一阶段应该是 dedicated refactor thread，正式做 `Day1 runtime decoupling`

---

## 3. 用户最终语义定稿

下面这些不是“建议”，而是当前 authoritative 基线。

### 3.1 opening 的正确语义

1. `001/002` 在 opening 里仍然是必要剧情 actor。
2. 但 opening 的 staged movement contract 不是只给 `001/002`，而是给所有参与 opening 的 Town NPC，包括：
   - `003`
   - `101~203`
3. 正确顺序固定为：
   - 进入 `Town`
   - 所有参与 opening 的 NPC 先传送到 scene authored `起点`
   - 再从 `起点` 走到 scene authored `终点`
   - 最多等待 `5` 秒
   - 超时仍未到位者直接 snap 到 `终点`
   - 只有全部归位后，才允许对白开始
   - 对白进行中不再发生任何走位
   - 对话结束后才允许放人
4. 绝对不允许再出现：
   - `对白开始时人没站对，等对白结束后反而才跑到位`

### 3.2 opening 结束后的正确语义

1. `001/002` 继续吃 `house lead` 这条导演语义。
2. `003` opening 后不再特殊化，必须与 `101~203` 进入同一 resident contract。
3. 所有已 release 的 Town resident，正确语义是：
   - 先判断自己是否远离 anchor
   - 若远离，则使用导航回 anchor
   - 回到 anchor 后，再进入自由 roam
4. 也就是说，opening 结束后不该是：
   - Day1 再继续拖着 resident 走人生
   - CrowdDirector 自己继续持有 resident 的下半生
5. 而应该是：
   - Day1 发 release intent
   - NPC 收到 release 后切 resident state
   - 导航只负责把这段路走好

### 3.3 Healing / Workbench / Farming 阶段

1. `Primary` 现状正确，不允许破坏。
2. `001/002` 在这段时间只能出现在 `Primary`。
3. 玩家此时返回 `Town`，Town resident 不应被冻结、隐藏或被 suppress。
4. 正确语义是：
   - 玩家离开 `Town` 时他们在哪
   - 玩家回来时他们就仍在哪
   - 并保持自由活动

### 3.4 dinner 的正确语义

1. dinner 与 opening 使用同一份 staged contract。
2. `001/002` 在 dinner 里也不再作为特殊 staged actor。
3. 正确流程仍然是：
   - 起点
   - 终点
   - 最多 `5` 秒
   - 超时 snap
   - 朝向正确
   - 对白中冻结
   - 对话结束统一 release

### 3.5 自由活动与夜间

1. `19:30`：dinner 相关剧情收完，玩家恢复自由活动
2. `20:00`：resident 自主回家
3. `21:00`：未到位者才 snap
4. `26:00`：forced sleep
5. 这条时间链不是 Day1 私房逻辑，而是全日通用合同

---

## 4. 正确的最终职责切分

### 4.1 `SpringDay1Director` 应该只拥有的东西

文件：
[SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)

它只该回答这些问题：

1. 当前是哪一段戏
2. 哪些角色要上场
3. opening / dinner 的起点终点在哪
4. staged contract 的 `5 秒等待 / snap / 对白前就位 / 对白中冻结`
5. 什么时候 acquire story control
6. 什么时候 release
7. `001/002` 的 `house lead`
8. Day1 如何接到共享夜间合同

### 4.2 `SpringDay1Director` 不该再拥有的东西

1. resident 回 anchor 的执行细节
2. resident 回到 anchor 后如何恢复 roam
3. resident 在对白后的 runtime lifecycle
4. 通过各种 release / latch / hold 条件继续偷偷抓 resident

### 4.3 `SpringDay1NpcCrowdDirector` 应该只拥有的东西

文件：
[SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)

它只该做：

1. crowd roster
2. manifest / marker / anchor / binding
3. 显式 crowd beat 里的 staged crowd 辅助
4. 必要的 debug summary

### 4.4 `SpringDay1NpcCrowdDirector` 不该再拥有的东西

1. 剧情后的 resident lifecycle
2. resident return-home tick
3. resident finish-return-home
4. resident restart-roam
5. resident 夜间 schedule 私房逻辑
6. 对已 release resident 的 runtime reacquire

### 4.5 `NPC` 最终应该拥有的东西

`NPC` 线程最终应提供一个干净 facade，只处理 resident state，而不重写 Day1 语义，也不接管导航策略。

它应该负责：

1. `AcquireStoryControl`
2. `ReleaseToAnchorThenRoam`
3. `ReleaseToHome`
4. `ResumeAutonomousRoam`
5. `SnapToTarget`

### 4.6 `导航` 最终应该拥有的东西

导航线程只应该回答：

1. 怎么去 anchor
2. 怎么去 home
3. 怎么避障
4. 怎么局部避让
5. 卡住了怎么重算
6. 到位后如何通知 NPC `Arrived / Failed / Stuck`

### 4.7 共享日夜调度最终应该拥有的东西

共享 schedule 最终只负责：

1. `20:00 return home`
2. `21:00 snap`
3. `26:00 sleep`

它不应该继续深埋在 Day1 私房 runtime 中。

---

## 5. 当前代码世界里，两个“罪犯”分别犯了什么罪

### 5.1 `SpringDay1Director` 当前的问题

文件：
[SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)

它有一部分是健康的：

1. opening / dinner 的 staged contract
2. `001/002` 的 `house lead`
3. `Primary` 里的剧情推进

但它现在多做了不该做的事情：

1. 通过 `ShouldReleaseEnterVillageCrowd()`、`ShouldLatchEnterVillageCrowdRelease()`、`ShouldHoldEnterVillageCrowdCue()` 等接口继续深度影响 crowd runtime
2. 把 Day1 语义和 resident runtime 生命周期绑得太深
3. 没有真正做到“我只发 release intent，不继续持有剧情后 resident”

最关键的一句是：

`它本来该是编剧+场记，现在却还兼职去影响 resident 下半生。`

### 5.2 `SpringDay1NpcCrowdDirector` 当前的问题

文件：
[SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)

这才是当前最大的结构病源。

它现在不只是管理 crowd 名单，它还同时在做：

1. `Update()` 高频 `SyncCrowd()`
2. `ApplyStagingCue(...)`
3. `ApplyResidentBaseline(...)`
4. `AcquireResidentDirectorControl(...)`
5. `ReleaseResidentDirectorControl(...)`
6. `TryBeginResidentReturnHome(...)`
7. `TickResidentReturns()`
8. `TickResidentReturnHome(...)`
9. `TryDriveResidentReturnHome(...)`
10. `FinishResidentReturnHome(...)`
11. `ForceRestartResidentRoam(...)`
12. `SyncResidentNightRestSchedule()`

这意味着它把以下权力都吞了进去：

1. crowd 参演
2. resident 分组
3. resident release
4. resident return-home
5. resident restart-roam
6. 夜间调度

这就是为什么它是“越权怪物本体”。

---

## 6. 为什么你一关 `CrowdDirector`，NPC 就正常

用户的 fresh live 已经给出最强反证：

1. `CrowdDirector` 开着时，NPC 会抖、会罚站、会假 roam
2. 关掉它后，NPC 反而恢复正常 roam

这说明当前真正锁住 NPC 的，不是“NPC 自己不会 roam”，而是：

1. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 的 `Update()` 还在反复 `SyncCrowd()`
2. resident 仍被留在 `scriptedControlActive` 或其它 crowd 自己的 lifecycle 持有链里
3. 或者刚被放手，又被下一帧重新抓回
4. 或者被错误 release 到一个坏的 runtime 状态

所以你看到的：

1. 高频小幅抖动
2. 原地罚站
3. profiler 里它占大头

本质上都属于同一个根：`剧情后 resident lifecycle owner 写错了`

---

## 7. 历史上到底发生了哪些误实现

这一段很重要，因为它解释了为什么这条线一直在反复，而不是一次修掉。

### 7.1 第一类错误：Day1 越权继续养 resident

我之前一直在做的错误，是把 Day1 理解成：

`戏演完以后，我还要继续照顾他们怎么回去、怎么重启 roam、怎么继续活。`

这是错的。

正确语义应该是：

1. 戏开始：Day1 接管
2. 戏内：Day1/Staging 只做一次 scripted movement
3. 对白中：冻结
4. 戏结束：Day1 只发 release intent
5. 剩下由 NPC + Navigation 自己接

### 7.2 第二类错误：`CrowdDirector` 被我做成 resident 下半生 owner

这条是最大的结构病。

我把 `CrowdDirector` 从“群演表+场景绑定”越写越厚，最后变成了：

1. 谁该出现
2. 谁该隐藏
3. 谁该 release
4. 谁该回家
5. 怎么回家
6. 到家后怎么 restart roam
7. 晚上怎么 schedule

这直接导致它从薄层工具变成了 resident 运行态中央集权。

### 7.3 第三类错误：用低级状态机原语拼业务语义

旧实现里反复出现这些危险组合：

1. `StopRoam`
2. `RestartRoamFromCurrentContext`
3. `DebugMoveTo`
4. `ApplyProfile`
5. `SetHomeAnchor`
6. `RefreshRoamCenterFromCurrentContext`
7. transform 手搓 fallback

这类做法的问题不是“看起来丑”而已，而是：

`它会让 Day1 线程直接拼 NPC 内部状态机。`

于是只要 handoff 没收好，新的反效果就会不断出现。

### 7.4 第四类错误：从旧 freeze 修成了新风暴

这条是最近几轮最典型的反效果。

我先是把 resident 从 `CrowdDirector` 过度持有里放出来；
但中间合同没补完整，于是又把它们错误地直接放回 `autonomous roam`。

结果就从：

1. 旧错误：`被 CrowdDirector 抓住不放`

变成了：

2. 新错误：`从拥挤的 crowd cue 终点直接 StartRoam()，炸 autonomous roam 风暴`

这不是新问题和旧问题无关，而是同一条 owner 错位链，在不同阶段表现出的不同坏相。

---

## 8. 当前 fresh live / profiler 已经钉死的事实

### 8.1 已被用户 fresh live 钉死的事实

1. `CrowdDirector` 开着时，NPC 会高频小幅抖动、罚站、假 roam
2. 关掉 `CrowdDirector` 后，NPC 恢复正常 roam
3. 这轮甚至会自己回 anchor

### 8.2 已被 profiler 钉死的热点形态

当前最直接热点链被定位为：

1. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `Update()`
2. -> `TickResidentReturns()`
3. -> `TickResidentReturnHome()`
4. -> `TryDriveResidentReturnHome()`
5. -> [NPCAutoRoamController.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs) `DriveResidentScriptedMoveTo(...)`
6. -> `DebugMoveTo(...)`

也就是说：

`CrowdDirector` 不是只发了一次“去 home/anchor”的意图，而是在 runtime 里持续代驾 resident 回家。`

### 8.3 次一级热点

还有一条次一级热点：

1. `HasPendingDaytimeAutonomyRelease()`
2. `YieldDaytimeResidentsToAutonomy()`
3. `NeedsDaytimeAutonomyRelease(...)`

这条会造成：

1. `_spawnStates` 双遍历
2. 重复 `GetComponent(...)`
3. release 后 resident 再次被碰

### 8.4 `NPCAutoRoamController` 侧说明了什么

当前还不能说“导航完全没问题”，因为：

1. `NPCAutoRoamController` 在 autonomous roam 下也还存在高成本候选采样和全场 agent clearance 扫描
2. 坏输入下它还没有完全做到 cheap fail

但这并不改变根本责任排序：

1. **第一触发器仍在 Day1/CrowdDirector**
2. **NPC/导航目前的责任是接住 release 后执行质量**

---

## 8A. 历史时间线与误修脉络

这一段专门回答一件事：

`为什么这条线不是“已经分析很多次了为什么还没修好”，而是“历史上几轮修法本身就在不断制造新坏相”。`

### 8A.1 第一阶段：最初把 Day1 写成“剧情内外一把抓”

最早的错误起点不是某个 if，而是整体理解错了：

1. 我把 Day1 理解成“不但要管戏怎么演，还要管戏后 NPC 怎么继续活”
2. 于是 `SpringDay1Director` 和 `SpringDay1NpcCrowdDirector` 都开始碰剧情后的 resident 生命周期
3. 这一步直接埋下了后面所有 owner 混线的根

### 8A.2 第二阶段：为了修“放人后站死”，把 `CrowdDirector` 越写越厚

当时看到的坏相主要是：

1. resident 放手后站死
2. resident 不回 anchor
3. resident 看起来像没恢复 roam

我当时的错误处理方向是：

1. 不去收 owner 边界
2. 而是继续往 `CrowdDirector` 里加：
   - baseline
   - return-home
   - finish-return-home
   - restart-roam
   - night schedule

结果就是：

`为了修“不会恢复”，把 CrowdDirector 做成了 resident 下半生 owner。`

### 8A.3 第三阶段：为了修“回不去/站不对”，继续拼低级状态机原语

这一阶段开始大量出现这些组合：

1. `StopRoam`
2. `RestartRoamFromCurrentContext`
3. `DebugMoveTo`
4. `ApplyProfile`
5. `SetHomeAnchor`
6. `RefreshRoamCenterFromCurrentContext`
7. transform 手搓 fallback

这一步的根本问题不是“写法粗暴”，而是：

`业务线程开始直接拼 NPC locomotion 内脏。`

从这一刻开始，只要 handoff 有一点没收干净，就一定会出现新的反效果。

### 8A.4 第四阶段：从“锁死”修成了“风暴”

后面几轮补丁最典型的错误，是我一边意识到 `CrowdDirector` 抓太多，一边又没有把 release contract 真正收干净。

于是坏相从旧版的：

1. resident 被留在 crowd/runtime 持有链里
2. 看起来像锁死、罚站、被抓住不放

变成新版的：

1. resident 过早被放回 autonomous roam
2. 但放手位置仍然是错误的 crowd cue 终点/人堆/拥挤区域
3. 随后 `NPCAutoRoamController` 吃到坏输入，进入高成本采样、clearance、重试循环
4. 玩家看到的就变成：
   - 高频小幅抖动
   - 假 roam
   - 像不动一样卡着
   - profiler 爆炸

### 8A.5 第五阶段：用户用 live 反证把根因最终钉死

直到用户给出最关键的现场反证：

1. `CrowdDirector` 开着就坏
2. 关掉 `CrowdDirector` 反而正常
3. 这轮甚至连回 anchor 都会自己做

这时才能把最终结论钉死：

1. 根病不是“NPC 自己不会走”
2. 根病不是“导航天生差”
3. 根病是 `CrowdDirector` 和 Day1 继续越权持有剧情后 resident lifecycle
4. 导航/NPC 的问题是第二层，要在 release 后正确接住，而不是继续让 Day1 代做

### 8A.6 为什么前面那些分析方向有一部分对了，但现场还是持续出反效果

因为前面几轮里，判断和落地并不处于同一个完成度：

1. **方向层面**
   - “CrowdDirector 权限太大”
   - “003 不该继续特殊化”
   - “剧情后应回 anchor 再 roam”
   - “导航只该负责怎么走”
   这些判断其实已经逐步对了
2. **落地层面**
   - 具体 runtime 写口没有彻底拔干净
   - `Update()` 里还在继续 `SyncCrowd()`
   - `TickResidentReturnHome()` 还在代驾
   - release 后仍会被重新触碰或过早 autonomy
3. 所以历史上多次出现：
   - 判断越来越接近真相
   - 但现场仍在继续出新坏相

这一点必须在交接里说清，因为它解释了为什么：

`不是分析方向完全错了，而是 owner 没有真正拔干净，导致“方向对 + 现场仍坏”长期并存。`

---

## 9. 当前已经落下去的止血补丁，到底是什么定位

为了不再混淆，这里明确区分：

### 9.1 这些是止血补丁，不是最终架构

当前已经落过或正在 WIP 的改动，包括：

1. opening direct-autonomy 短路收掉
2. daytime 重复触碰 guard
3. opening handoff stand-down
4. return-home 重下发节流

这些的定位只有一个：

`让当前 live 不要继续在最热链路上炸。`

### 9.2 它们不是最终解法的原因

因为真正的终局不是“把 CrowdDirector 调得聪明点”，而是：

1. 它根本不该继续拥有剧情后 resident lifecycle
2. 它应该被削成薄层
3. resident state 正式移交给 NPC facade
4. 走路质量正式移交给 Navigation contract

所以这些补丁最多是：

1. 保持现有分支别继续爆炸
2. 给 dedicated refactor thread 创造更干净的接手面

---

## 10. 当前 branch / 代码现场的最准确口径

### 10.1 当前已经能站住的

1. 结构级真相已经清晰
2. 用户语义已经定稿
3. exact 热链已被定位到方法级
4. 一部分最直接的 runtime 写口已经开始被削

### 10.2 当前还不能站住的

1. 不能说“体验已过线”
2. 不能说“CrowdDirector 开着已经和关掉一样正常”
3. 不能说“正式重构已完成”

### 10.3 当前最值得保留的补丁

如果 dedicated refactor thread 接手前要保留现场稳定性，当前最值得保留的是：

1. direct-autonomy 短路收口
2. daytime 重复触碰 guard
3. return-home 重下发节流

### 10.4 当前最不该再继续堆的补丁

1. 继续往 `CrowdDirector` 里塞新的 resident lifecycle 逻辑
2. 再次用 `StopRoam / RestartRoam / DebugMoveTo` 拼业务语义
3. 再次把 `003` 特殊化
4. 再次把 Town resident 冻住/藏起来规避 bug

---

## 11. dedicated refactor thread 应该接什么，不应该接什么

### 11.1 这条新线程应该接的核心范围

虽然这份文档不是 prompt，但为了交接清晰，必须把接班范围说死：

新线程应接：

`Day1 runtime decoupling`

### 11.2 它应该完成的最终目标

1. [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 收成剧情语义层
2. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 削成 crowd binding 薄层
3. `剧情后 resident lifecycle` 正式迁到 `NPC facade + Navigation contract`

### 11.3 它不应该顺手扩的东西

1. UI 壳体
2. `Primary` 已稳定链
3. 导航 core 的大重写
4. dinner 的额外扩题
5. 全日夜间总合同的全面改造

这些可以作为后续阶段，但不该污染第一阶段重构。

---

## 12. 建议的新线程重构阶段划分

### Phase 0：只读盘点

先把这些东西盘清，不改代码：

1. 当前重复实现的 release 语义
2. 当前 release 和 reacquire 并存的冲突点
3. 当前还残留的低级 public 写口
4. `003` 还在哪些地方被特殊化
5. `20:00~26:00` 还在哪些地方写成 Day1 私房逻辑

### Phase 1：剥掉 `CrowdDirector` 的剧情后 resident lifecycle owner

这是第一阶段最先要完成的目标。

至少要移走：

1. `ApplyResidentBaseline(...)` 中剧情后 release 持续续命的部分
2. `TryBeginResidentReturnHome(...)`
3. `TickResidentReturns()`
4. `TickResidentReturnHome(...)`
5. `TryDriveResidentReturnHome(...)`
6. `FinishResidentReturnHome(...)`
7. `ForceRestartResidentRoam(...)`

### Phase 2：建立 NPC facade

把 resident state 切到正式 facade：

1. `AcquireStoryControl`
2. `ReleaseToAnchorThenRoam`
3. `ReleaseToHome`
4. `ResumeAutonomousRoam`
5. `SnapToTarget`

同时把危险 public 口收窄。

### Phase 3：接入 Navigation contract

Navigation 只收执行，不再猜 Day1 phase：

1. `BeginReturnToAnchor`
2. `BeginReturnHome`
3. `AbortAndReplan`
4. `Arrived / Failed / Stuck`

### Phase 4：整理共享日夜调度

把：

1. `20:00 return home`
2. `21:00 snap`
3. `26:00 sleep`

正式迁出 Day1 私房 runtime。

---

## 13. 当前需要特别保护的正确内容

新线程接手时，这些东西不能被“顺手重构坏”：

1. `001/002` 的 `house lead`
2. `Primary` 当前已稳定工作流
3. opening / dinner 的 staged contract 总体语义
4. `003` opening 后应并回普通 resident
5. Town resident 在剧情外必须保持自由活动

---

## 14. 当前必须遵守的红线

### 14.1 业务红线

1. 不再手搓 return-home transform fallback
2. 不再用 `StopRoam / RestartRoam / DebugMoveTo` 组合拳
3. 不再把 `003` 特殊化
4. 不再碰 `Primary` 已稳定链
5. 不再碰 UI 壳体
6. 不再通过隐藏/冻结 Town resident 规避 bug
7. 不再把 `20:00~26:00` 深埋成 Day1 私房逻辑

### 14.2 叙事红线

1. opening / dinner 的 NPC staged contract 必须统一
2. 不允许对白开始时人没站对
3. 不允许对白期间继续乱走
4. 不允许对白结束后才慢半拍跑去站位

### 14.3 沟通红线

1. 不准再把“结构判断成立”说成“体验已过线”
2. 不准再把“止血补丁已落”说成“正式重构已完成”
3. 不准再把“方向对”包装成“现场已经好”

---

## 15. 为什么现在适合切 dedicated refactor thread

不是因为问题突然变简单了。

而是因为前置条件终于够了：

1. 用户语义定稿
2. 红线文档已落
3. `Day1 / 导航 / NPC` 的边界已清
4. `CrowdDirector` 的 exact 越权链和热点方法已钉到方法级
5. 当前最直接的止血补丁也已经替接班线程把现场稍微清了一点

所以现在继续在旧线程里边补边猜，收益已经很低。

更正确的组织方式是：

1. 旧线程停在这里，不再无限补丁
2. 新线程拿这份文档接手
3. 只做 `Day1 runtime decoupling`

---

## 15A. 接班线程应该怎么使用这份文档

这份文档不是让接班线程“全文读完再自由发挥”，而是有明确使用顺序的。

### 15A.1 第一遍必须先看的 4 段

1. `第 3 节：用户最终语义定稿`
2. `第 4 节：正确的最终职责切分`
3. `第 5 节：两个罪犯分别犯了什么罪`
4. `第 14 节：必须遵守的红线`

这四段决定的是：

1. 用户真正要什么
2. 正确 owner 应该长什么样
3. 当前错在哪
4. 什么东西绝对不能再碰坏

### 15A.2 第二遍再看的 3 段

1. `第 8 节：fresh live / profiler 已钉死的事实`
2. `第 8A 节：历史时间线与误修脉络`
3. `第 9 节：当前止血补丁的定位`

这三段决定的是：

1. 当前现场到底坏成什么样
2. 历史上为什么会一路修歪
3. 现有补丁哪些只是止血，哪些值得暂时保留

### 15A.3 真开工前必须先做的盘点

接班线程不要上来就改代码，而是先按这份文档做一轮只读盘点：

1. `SpringDay1Director` 里还剩哪些剧情后 resident 触碰口
2. `SpringDay1NpcCrowdDirector` 里还剩哪些 runtime owner 写口
3. 当前 `003` 是否还有残留特殊化
4. `20:00~26:00` 还有哪些逻辑深埋在 Day1 私房 runtime 里
5. 哪些补丁是为了止血暂留，哪些是应该在重构第一阶段就拆掉

### 15A.4 真开工时的第一原则

第一阶段不是“把所有坏点一次修完”，而是：

`先把 owner 写对。`

只要 owner 还没写对，下列动作都不该先做：

1. 大规模调导航参数
2. 继续给 `CrowdDirector` 塞 guard
3. 扩 dinner / 全日夜间总合同
4. 改 `Primary`
5. 改 UI 壳体

### 15A.5 接班线程完成第一阶段后应该向用户交什么

不是一句“我重构完了”，而是至少要交清楚：

1. 哪些 resident lifecycle 责任已经从 `CrowdDirector` 手里移走
2. `SpringDay1Director` 还保留了哪些剧情语义
3. `NPC` 和 `导航` 分别正式接到了什么合同
4. 现有止血补丁中哪些已经删除，哪些仍暂留
5. 当前站住的是：
   - 结构已改对
   - 还是 live 已过线

---

## 16. 当前未决风险与开放问题

### 16.1 已知但未彻底解决的剩余疑点

1. `_spawnStates == 0` 时的 `ShouldRecoverMissingTownResidents(...)` 恢复链
2. `HasPendingDaytimeAutonomyRelease()` 的双遍历
3. `NPCAutoRoamController` 在坏输入下的 cheap fail 仍不够强

### 16.2 这些风险为什么现在不该抢进第一阶段重构

因为第一阶段最值钱的不是“把所有坏点一次修完”，而是：

`先把 owner 写对。`

只要 owner 还错着，再修这些局部热点，后面还会反复污染。

---

## 17. 重构完成定义

只有同时满足下面这些，才算真的完成，而不是继续半成品：

1. `SpringDay1Director` 不再持有剧情后 resident lifecycle
2. `SpringDay1NpcCrowdDirector` 不再持有剧情后 resident lifecycle
3. `003` opening 后彻底并回普通 resident 合同
4. opening / dinner staged contract 统一且稳定
5. `001/002` 的 `house lead` 与 `Primary` 工作流不被破坏
6. Town resident 在剧情外始终自由活动
7. `CrowdDirector` 开着时也不再出现：
   - 高频小幅抖动
   - 原地罚站
   - 假 roam
8. `20:00 / 21:00 / 26:00` 的共享时序不再深埋为 Day1 私房逻辑

---

## 18. 给接班线程的最后一句话

如果你接手这条线，请先认两件事：

1. 当前最需要解决的不是“再补一个条件”
2. 当前最需要解决的是“把 resident 的剧情后下半生从 Day1/CrowdDirector 手里彻底抢回来”

如果你后面继续把 `CrowdDirector` 写成 resident 下半生 owner，那不是“还没想清楚”，而是已经在明知故犯。
