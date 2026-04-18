# 2026-04-15｜给导航｜Day1-V3 语义同步与执行边界 contract

## 1. 你先需要同步的事实

`Day1` 这条线已经切到 `Day1-V3`。

当前最新方向不是继续局部补丁，而是：

`把 Day1 / NPC / 导航 三方边界重新收成单一真相。`

## 2. Day1 当前语义基线

你后续判断必须服从下面这些语义，不要再回吞 Day1 phase：

1. `opening / dinner` 仍是 Day1 own 的 staged contract。
2. `001/002` 的 `house lead / Primary` 特殊性仍归 Day1。
3. `003` opening 之后最终应并回普通 resident release contract。
4. resident 剧情结束后，Day1 只该发 release intent，不该继续代管下半生。
5. `20:00 / 21:00 / 26:00` 最终应是共享 schedule，不应继续作为 Day1 私房 runtime。

## 3. 你 own 什么

你 own 的不是剧情语义，而是：

1. 任何真实 movement execution 的质量
2. 去 anchor / 去 home / 去 staged target 的路径执行
3. blocked / stuck / fail / abort / replan
4. static navigation / body clearance / local avoidance / detour

## 4. 你不 own 什么

你不 own：

1. `Day1 phase / beat / cue`
2. 谁该 release
3. `001/002/003` 的剧情身份
4. opening / dinner 的 staged cue 语义
5. NPC 最终进入哪种 resident state

## 5. 当前三方边界

| 组件 | 应负责 | 不应负责 |
|---|---|---|
| `Day1` | `谁要动 / 去哪 / 何时放手 / 何时 snap / 何时进入下一段戏` | movement execution 细节 |
| `NPC` | `release 后进哪种 resident state / facade / 内部状态机` | phase / beat / 路径策略 |
| `导航` | `任何真实位移如何执行` | phase / release 语义 |

## 6. 当前我对“谁动就调导航”的判断

我当前的主张是：

`只要 NPC 发生真实位移，原则上都应该优先走统一导航执行合同。`

原因：

1. 如果剧情移动和日常移动用两套 execution contract，owner 一定会再次缠回去。
2. Day1 的 staged movement、release 后回家、普通 free-roam，本质上只是“为什么动”不同，不是“怎么动”必须分两套。
3. 当前用户看到的很多坏相，本质就是 `autonomous / scripted / debug / fallback` 混用。

明确例外：

1. staged timeout 后的显式 snap
2. load/repair 场景下的显式 snap
3. authored 失败时的强制摆位兜底

也就是说：

1. 正常走路走导航
2. 明确 snap 才 snap

## 7. 当前你最需要对齐的红线

1. 不再接受 `return-home` 继续走 `debug / scripted / manual fallback` 半混合合同。
2. 不再默认把 `DriveResidentScriptedMoveTo(...)` 视为“剧情移动自己一套，日常移动自己一套”的合理终局。
3. 不再替 Day1 决定谁该 release。
4. 不再把“导航自己能走”包装成“Day1 边界已经正确”。

## 8. 当前需要你配合 Day1 的地方

你后续如果继续施工，应围绕下面 3 件事判断，而不是回吞 Day1 语义：

1. released 后 movement execution 是否已统一
2. return-home / free-roam / stage-travel 是否共用同一套正式导航执行合同
3. stuck / blocked / replan 是否能作为通用执行层被 NPC facade 消费

## 9. 你后续回执应该回答什么

你后续如果接这条线，回执只需要回答这些：

1. 导航执行层当前是否还能区分成多套 movement contract
2. 哪一套是必须合并的
3. 合并后会不会伤到 Day1 既有 staged contract
4. 你的唯一下一刀是什么

不要再回到：

1. Day1 phase 应该怎么设计
2. 哪些 NPC 应该上场
3. House lead 何时算开始

这些都不是你 own。
