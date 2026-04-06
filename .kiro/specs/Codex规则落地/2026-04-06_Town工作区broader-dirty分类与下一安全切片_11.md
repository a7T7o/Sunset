# 2026-04-06｜Town 工作区 broader dirty 分类与下一安全切片

这份正文只回答一个问题：

- 在 `15d75285` 已经把 `Town` resident scene-side 最小 checkpoint 固化之后，当前 working tree 里那份更大的 `Town.unity` dirty，到底还剩什么，以及下一刀最安全该怎么切。

---

## 一、先说结论

当前 working tree 里的 `Town.unity` broader dirty，**已经不再包含新的 resident root / carrier / slot 可提取增量**。

也就是说：

1. `Town_Day1Residents`
2. `Town_Day1Carriers`
3. `ResidentSlot_*`
4. `DirectorReady_*`
5. `BackstageSlot_*`
6. `EnterVillageCrowdRoot / KidLook_01 / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03`

这些层级在当前 working tree 相对 `HEAD` 的变动筛查里，新增改动数是：

- `0`

所以后面如果还想继续从当前 working tree 里“再抠一刀 Town resident scene-side”，方向已经不对了。

---

## 二、这次只读复勘实际看到什么

我对 `HEAD:Assets/000_Scenes/Town.unity` 和 working tree 的 `Assets/000_Scenes/Town.unity` 做了一次轻量结构比对。

结果是：

- 总变更 section 数：`265`

但这些变更对象的重心已经明显转到 resident scene-side 之外。

### 当前最大的几类对象变更

#### 1. 桥 / 农田 / 大块 Tilemap 分层

高频对象包括：

1. `Layer 1 - Farmland_Water`
2. `Layer 1 - Farmland_Border`
3. `Layer 1 - Farmland_Center`
4. `Layer 1 - Wall`
5. `Layer 2 - Wall`
6. 多组桥体中文 layer
7. 多组植被 / 地皮 / 物品 layer

这说明当前 larger dirty 的主体之一，是一批环境层 / Tilemap 层重排与新增。

#### 2. 相机 / 玩家 / 转场链

筛到的明显对象有：

1. `Main Camera`
2. `CinemachineCamera`
3. `Player`
4. `SceneTransitionTrigger`

这说明另一个明显脏块，是 Town 相机跟随、玩家位、转场链相关现场。

#### 3. manager / bootstrap / 运行态基线对象

当前还能看到：

1. `PersistentManagers`
2. `CloudShadow`

这说明 working tree 里还混着一层 manager / 渲染 / 场景基线类现场。

#### 4. 未完成归类的大型层级壳

当前还能看到：

1. `LAYER 111`
2. `LAYER 222`
3. 一批纯数字名节点

这类对象当前更像半成品分层壳或批量搬运现场，不应直接混算到 resident 承接链。

---

## 三、当前最值钱的负结论

这轮最值钱的不是“又找到什么能提交”。

而是这个负结论已经成立：

- 当前 working tree 里的 `Town.unity` broader dirty，不存在一批“还没提交、但其实仍属于 resident scene-side contract”的隐藏增量。

换句话说：

`Town` resident scene-side 这条线，当前已经把能从这份 scene 现场里最小、最稳、最诚实地抽出来的内容，基本抽尽了。

---

## 四、这意味着下一刀不该怎么做

后面不该再这样做：

1. 继续把当前整份 `Town.unity` dirty 当“Town 还有第三刀 resident 承接”
2. 一边看到 `Town` 仍脏，就一边默认“肯定还有 resident scene-side 没收干净”
3. 把桥 / 农田 / 相机 / manager / layer 重排混成 Town resident 续工

这几条现在都已经不成立。

---

## 五、下一刀真正安全的切法

如果要继续压 `Town`，下一刀应该只在下面几类里选一类，不要再混：

### A. 相机 / 转场 / 玩家位 scene slice

只处理：

1. `Main Camera`
2. `CinemachineCamera`
3. `Player`
4. `SceneTransitionTrigger`

目标是把 Town 切场体验链独立成单 slice。

### B. 环境 / Tilemap / 桥农田 layer slice

只处理：

1. Farmland layer
2. bridge layer
3. wall / props / base / grass 层

目标是把环境层级重排单独归类，不再挂在 resident 承接名下。

### C. manager / bootstrap / scene baseline slice

只处理：

1. `PersistentManagers`
2. `CloudShadow`
3. 其他场景基线对象

目标是把 Town scene-side 里混入的运行态基线现场单独收口。

---

## 六、对当前 Town 主线的现实改判

所以从现在开始，`Town` 这条线的更准确口径应改成：

1. resident scene-side 最小 checkpoint：已成立
2. 当前 broader dirty：仍存在
3. 但 broader dirty 已不再属于 resident contract 延长线
4. 下一步若继续 Town，不是“继续 resident 第三刀”，而是“从 mixed-scene 里选一个明确子域开新 slice”

---

## 七、我当前给治理位和后续自己的最短建议

如果没有新的明确 ownership，不要再碰整份 `Town.unity`。

如果要继续，就只开下面两种之一：

1. `Town 相机/转场独立 slice`
2. `Town 环境/Tilemap 独立 slice`

而 `resident scene-side` 这条线，本轮已经可以视为：

- **在当前 shared dirty 条件下，已经压到不能再从这里继续安全榨出新增值的深度**
