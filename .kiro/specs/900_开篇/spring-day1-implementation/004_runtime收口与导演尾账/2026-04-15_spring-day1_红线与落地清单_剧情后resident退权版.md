# 2026-04-15｜spring-day1｜红线与落地清单｜剧情后 resident 退权版

## 1. 这份文档是干什么的

这不是汇报文档，也不是检讨替代品。

这份文档的作用只有一个：

`把 spring-day1 以后不准再犯的红线、正确事件流、职责边界、施工前检查项、以及本轮允许/不允许落的刀口，正式写死成我自己的自限文档。`

## 2. 当前总判断

当前 Day1 最大的问题，不是“导航又坏了一个参数”，也不是“NPC 少了一个 API”，而是：

1. `SpringDay1Director` 和 `SpringDay1NpcCrowdDirector` 都越过了自己该有的边界。
2. `SpringDay1NpcCrowdDirector` 已经被我写成了“剧情后继续代管 resident 生命周期”的重控制器。
3. 用户用 live 反证已经钉死：
   - 只要关掉 `SpringDay1NpcCrowdDirector`
   - Town resident 就恢复正常 roam
   - 剩下只有“不会自动回家”
4. 所以当前第一优先不是“让 CrowdDirector 更聪明”，而是“让它退权”。

## 3. 永久红线

### 3.1 不准再碰的行为

1. 不准再手搓 return-home 的 transform fallback。
2. 不准再用 `StopRoam / RestartRoam / DebugMoveTo` 组合拳代替导航。
3. 不准再把 `003` 差异化成半剧情半 resident。
4. 不准再碰 `Primary` 已稳定链。
5. 不准再碰 UI 壳体。
6. 不准再用隐藏 / suppress / freeze Town resident 规避 bug。
7. 不准再把 `20:00~26:00` 写成 Day1 私房 resident runtime。
8. 不准再把“结构推断成立”说成“体验已过线”。

### 3.2 不准再犯的历史错误

1. 不准再在 opening 还没收平时顺手扩修 dinner。
2. 不准再在 resident release 还没收平时顺手扩修夜间总合同。
3. 不准再把 `release` 写成“从当前点直接开始 roam”。
4. 不准再让对白开始时 NPC 还没站对，等对白结束后反而才跑到位。
5. 不准再让 `CrowdDirector` 在对白结束后下一帧重新抓回 resident。
6. 不准再在没有 fresh live 票时说“我已经知道问题彻底没了”。

## 4. 两个导演的最终职责边界

### 4.1 SpringDay1Director 只该 own

1. opening / dinner 的剧情语义。
2. staged movement contract：
   - 起点
   - 终点
   - 5 秒等待
   - 超时 snap
   - 对白前就位
   - 对白中冻结
3. `001/002` 的 house lead。
4. 什么时候 acquire story control。
5. 什么时候 release 到 resident。
6. Day1 第一夜怎么接入共享夜间合同。

### 4.2 SpringDay1Director 不该 own

1. resident 回 anchor 的执行细节。
2. resident 到 anchor 后如何恢复 roam。
3. resident 在对白后的 runtime lifecycle。
4. 通过一堆 latch/hold/release 条件继续偷偷抓着 resident。

### 4.3 SpringDay1NpcCrowdDirector 只该 own

1. crowd roster / manifest / binding。
2. 显式 crowd beat 里的 staged crowd 布局辅助。
3. 必要的 debug summary。

### 4.4 SpringDay1NpcCrowdDirector 不该 own

1. resident 剧情后的下半生。
2. resident return-home tick。
3. resident finish-return-home / restart-roam。
4. resident 夜间 schedule 私房逻辑。

## 5. Day1 -> NPC -> Navigation 正确事件流

### 5.1 opening

1. 进入 `Town`。
2. Day1 收集所有 opening 参与 NPC。
3. 所有人先传送到 scene authored `起点`。
4. 所有人从 `起点` 走到 scene authored `终点`。
5. 最多等待 `5` 秒。
6. 超时仍未到位者 snap 到 `终点`。
7. 只有全部归位后，才允许对白开始。
8. 对白进行中不再走位。

### 5.2 opening 结束

1. Day1 发 release intent。
2. `001/002` 继续进入 `house lead`。
3. `003` 与 `101~203` 进入同一 resident contract。
4. NPC 收到“已离开剧情”后，自己判断是否远离 anchor。
5. 若远离，NPC 发起 return-to-anchor/home 意图。
6. 导航只负责把这段路走好。
7. 回到 anchor 后，NPC 再恢复自由 roam。

### 5.3 Healing / Workbench / Farming

1. `001/002` 只在 `Primary`。
2. Town resident 始终维持自由活动。
3. 玩家离开 Town 时他们在哪，回来就仍在哪。
4. Day1 不准为了省事把 resident 冻住或藏起来。

### 5.4 dinner

1. 所有参与 dinner 的 NPC 同 opening 一套 staged contract。
2. `001/002` 在 dinner 里也不再作为特殊 staged actor。
3. 对白结束后统一 release。
4. release 后不再由 Day1 持续拖着 resident 走人生。

### 5.5 夜间

1. `19:30`：dinner 相关剧情收完，玩家恢复自由活动。
2. `20:00`：共享夜间合同开始，resident 自主回家。
3. `21:00`：未到位者才 snap。
4. `26:00`：forced sleep。
5. 这条链未来应是全日通用，不应继续塞在 Day1 私房逻辑里。

## 6. 每次真实施工前的固定检查

### 6.1 冲突盘点

开始改之前，必须先回答：

1. 当前有没有重复实现同一段 release 语义的代码。
2. 当前有没有相反语义并存：
   - 一边 release
   - 一边下一帧 reacquire
3. 当前还有哪些地方残留：
   - `DebugMoveTo`
   - `StopRoam`
   - `RestartRoamFromCurrentContext`
   - transform step fallback
4. 当前有没有地方还在特殊化 `003`。
5. 当前有没有地方还把 Town resident 的可见性当规避 bug 的工具。

### 6.2 风险确认

1. 这轮是不是只改一个垂直切片。
2. 这轮有没有触碰 `Primary`。
3. 这轮有没有触碰 UI 壳体。
4. 这轮有没有触碰导航 core。
5. 这轮有没有 fresh live 入口来验证它。

只要上面任一项答案不对，先停，不准开改。

## 7. 本轮只做 / 不做

### 7.1 本轮只做

1. 给 `spring-day1` 自己写新版唯一主刀 prompt。
2. 给导航写新版边界 prompt。
3. 给 NPC 写新版合同 prompt。
4. 把我的红线、事件流、施工 checklist 沉淀成这份文档。

### 7.2 本轮不做

1. 不继续泛修代码。
2. 不顺手再改 dinner。
3. 不顺手再改全日夜间合同。
4. 不顺手再改导航执行层。
5. 不顺手再改 NPC locomotion 实现。

## 8. 下一轮真实施工唯一允许的刀口

只允许先收：

`SpringDay1NpcCrowdDirector 从“剧情后 resident 生命周期 owner”退权。`

具体表现为：

1. daytime 无显式 crowd beat 时，`CrowdDirector` 不再继续抓 resident。
2. opening 结束后，`003` 与 `101~203` 进入同一 release contract。
3. `SpringDay1Director` 只负责发 release / story handoff intent。
4. resident 后续的 return-to-anchor / roam，由 NPC + Navigation 合同接住。

## 9. 施工 checklist

1. 先读最新用户裁定。
2. 先读本文件。
3. 先盘点重复实现与相反实现。
4. 明确这轮唯一刀口。
5. 明确哪些文件允许碰，哪些绝对不碰。
6. 开改前先报实当前是只读还是实改。
7. 改完先做最小代码层自检。
8. 明确标注：
   - 代码层 clean
   - 还是 live 已验证
9. 回写子工作区 memory。
10. 回写父工作区 memory。
11. 回写线程 memory。

## 10. 完成定义

只有同时满足下面这些，才允许我说这轮真的收住了：

1. 文档里已把红线和边界写死。
2. 以后施工前必须先有冲突盘点，不再直接扑代码。
3. opening / dinner / release / night 的责任链已经在文档里拆清。
4. 给 `spring-day1 / 导航 / NPC` 的 prompt 已分别落文件。
5. memory 已同步，不再只靠聊天上下文。

## 11. 自我约束结语

如果我后面再犯下面任何一种：

1. 又把 `CrowdDirector` 做重。
2. 又把 `003` 特殊化。
3. 又去碰 UI 壳体或 `Primary`。
4. 又在没有冲突盘点前直接改运行时。
5. 又把结构说成体验。

那不是“这轮没想清楚”，而是我明知故犯。
