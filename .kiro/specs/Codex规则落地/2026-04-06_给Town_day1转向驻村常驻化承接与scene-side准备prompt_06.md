# 2026-04-06｜给 Town｜day1 转向驻村常驻化承接与 scene-side 准备

这不是让你来抢 `spring-day1` 当前 active 的代码文件。

这份 prompt 的目的只有一个：

- 把 `day1` 对 `Town` 的协作方向，从“先用 runtime spawn 止血”正式改成“朝驻村常驻化去做”。

如果你继续，先按这个新真值理解，不要再把这批 crowd 默认理解成“到点才 instantiate 的临时演员”。

---

## 一、先说新的共同真值

`day1` 这边刚做了重新判断，方向已经改了：

1. 这批 `101~301` crowd，后续目标不是“继续 runtime spawn，只是更早预热”
2. 最终要做成的，是“他们本来就是村里的人”
3. 导演层以后更像是：
   - 接管已有 resident actor
   - 调整站位 / 朝向 / 让位 / 路径 / 出声时机
4. 而不是：
   - 到 cue 才决定“人有没有存在”

这不是审美偏好，而是 `day1` 当前真实叙事决定的：

1. 开篇真正跟着玩家动的，只有村长
2. 艾拉是在治疗段跟着村长来给玩家回血
3. 村里的 crowd 不该像“阶段到了才突然蹦出来”
4. 他们应该像本来就在村里，只是玩家视线和导演调度逐步把他们显出来

---

## 二、这轮不是让你接 `CrowdDirector`，而是让你把 Town 这边的“常驻承接层”想清楚

当前 `spring-day1` 自己会继续吃：

1. `SpringDay1NpcCrowdDirector.cs` 这一刀
2. `semanticAnchorId -> runtime deployment` 这条最小 contract
3. 当前 `Primary` 代理承接向后续真实承接的过渡

所以你这轮先不要把球理解成“代码文件正式交回 Town”。

你现在最值钱的工作，是把 `Town` 作为“驻村常驻承接层”这件事压实。

---

## 三、你现在需要给 day1 的，不是泛边界，而是 5 类更窄的真值

### 1. 常驻 resident root 该怎么理解

请你基于 `Town` 当前现场，给出一个清楚口径：

1. 这批 `101~301` 后续在 `Town` 里应该挂在哪一层 resident root / group root
2. 哪些 root 是“默认日常在场”
3. 哪些 root 是“导演 takeover 时的承接层”
4. 哪些 root 只是背景层，不该被误当成主戏位

### 2. `semanticAnchorId` 到驻村位的语义映射

请你不要只说“anchor 已经有了”，而要继续往前讲清：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `NightWitness_01`
4. `DinnerBackgroundRoot`
5. `DailyStand_01~03`

它们分别更像：

1. 默认常驻位
2. 临时围观位
3. 夜间观察位
4. 背景生活位
5. 次日站位预示位

以及：

- 哪些应该长期挂着 actor
- 哪些只该在特定 phase 被 director 接管

### 3. 从 `Primary` 代理承接迁回 `Town` 时，最小 scene-side contract 应该长什么样

请你按“以后要迁回真实 `Town`”这个前提，给一个最小 contract 形状：

1. `day1` 这边如果先在 `Primary` 做 resident 化代理
2. 后面迁到 `Town`
3. 哪些 scene-side 前提必须一致
4. 哪些 anchor 命名、root 结构、默认启用状态不能再变

重点不是写宏大方案，而是让 `day1` 后面迁回去时不用整套重写。

### 4. 哪些 NPC 在“玩家尚未入村时”就应该已经算存在

这里要按叙事语义给判断，不要只按技术方便：

1. 哪几位本来就该在村里
2. 哪几位应该只是玩家还没看见
3. 哪几位可以在屋内 / 背景 / 远处成立
4. 哪几位不该在开篇就被看见，但也不应被理解成“根本不存在”

### 5. 你认为当前第一 scene-side blocker 到底还剩什么

如果你现在判断：

- `Town` 还不能立刻吃 runtime resident

也请你继续压缩成非常窄的一条，不要回到“大而泛的 Town 还没 ready”。

---

## 四、这轮你不要做的事

1. 不要把 `CrowdDirector` 这刀抢回去
2. 不要回到“Town 目前还没完全 live-ready，所以先不说”这种泛口径
3. 不要把“常驻化”偷换成“更早 spawn 就算了”
4. 不要把 `day1` 当前 active 文件再碰脏

---

## 五、你回来的东西最好至少讲清这 6 件事

1. 你是否接受这次方向正式改为“驻村常驻化”
2. `Town` 这边认为最合理的 resident root / anchor 语义是什么
3. 哪些 `semanticAnchorId` 后续可以直接承接 resident actor
4. 从 `Primary` 代理迁回 `Town`，最小 scene-side contract 该守什么
5. 当前 `Town` 自己这边还剩的第一 scene-side blocker 是什么
6. 你建议 `day1` 什么时候把球正式交回你，交回时应一并交哪些触点

---

## 六、最短一句话目标

请你把 `Town` 从“能承接导演语义的后半段场地”继续推进成：

- “未来能承接驻村常驻 crowd 的真实村庄承载层”

但这轮先不要越权去改 `spring-day1` 当前 active 的核心代码文件。
