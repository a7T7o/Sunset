# 2026-04-06｜给 spring-day1｜Town 驻村常驻化承接与 scene-side 正式回执

这份回执覆盖昨天的 [05](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/2026-04-06_给spring-day1_Town当前正式回执与接刀口径_05.md) 在“最小 runtime contract”上的旧重心。

新的共同真值已经变了：

- 这批 `101~301` 后续不是继续按“runtime 临时演员”去理解
- 而是朝“驻村常驻化”去做

所以这份回执的重点，不再是“你什么时候把 `CrowdDirector` 交回我”，而是：

- `Town` 这边如何把 resident root、anchor 语义和后续迁回 contract 先钉死

---

## 一、先说一句最重要的话

我接受这次方向正式改为：

- **驻村常驻化**

而且我认为这是对的。

因为当前这批人更像：

1. 本来就在村里的人
2. 只是玩家还没看到他们
3. 或导演还没接管到他们

而不是：

1. 到 cue 才突然被 instantiate 的临时演员

---

## 二、Town 这边现在对 resident root 的正式理解

我现在给 Town 这边的最小 scene-side 结构一个正式口径：

### 1. 现有根层继续保留

保留：

- `SCENE/Town_Day1Carriers`

它的职责不要改写。

它现在应该被理解成：

- **导演 takeover 锚点壳**

不是：

- 默认驻村 resident 的常驻根

### 2. 常驻层应该新增为一棵独立 sibling root

后面真要迁回 Town，我建议 scene-side 固定成：

- `SCENE/Town_Day1Residents`

并且下面至少固定 3 个 group root：

1. `Resident_DefaultPresent`
2. `Resident_DirectorTakeoverReady`
3. `Resident_BackstagePresent`

### 3. 这 3 个 group root 的职责

#### `Resident_DefaultPresent`

这里放的是：

1. 玩家入村前就应该已经算存在的人
2. 白天默认就在村里的生活层
3. 不需要靠导演“创建存在感”，只需要决定何时被玩家看见

#### `Resident_DirectorTakeoverReady`

这里放的是：

1. 语义上已经在 Town 里存在
2. 但这段需要被导演接走、站到特定锚点、完成一拍动作的人
3. 适合被吸到 `Town_Day1Carriers/*` 的 actor

#### `Resident_BackstagePresent`

这里放的是：

1. 本来就在村里
2. 但当前不该被玩家直接看到的人
3. 屋内 / 远处 / 夜间 / 村边 / 侧路 / 墓坡这类“语义存在、画面暂隐”的承载层

---

## 三、各 `semanticAnchorId` 现在该怎么理解

### 1. `EnterVillageCrowdRoot`

它现在不该被理解成“默认常驻停泊位”。

它更准确地是：

- **进村后第一拍的集体打量 / 让位 / 围观 takeover 位**

所以它是：

- director carrier

不是：

- resident parking root

它后面承接的 resident actor，主要来自：

1. `Resident_DefaultPresent`
2. `Resident_DirectorTakeoverReady`

### 2. `KidLook_01`

它也不是默认常驻位。

它更准确地是：

- **单点好奇视线位**

适合：

1. 小米式儿童
2. 一个被导演短暂接管的小孩 actor

不适合：

1. 当作一整天的常驻停泊根

所以它也是：

- director carrier

### 3. `DinnerBackgroundRoot`

这是第一批最适合向“常驻化”过渡的锚点之一。

它更像：

- **饭馆生活背景位**

它既可以：

1. 承接默认就在饭馆 / 饭馆周边的人
2. 也可以在需要时被导演接管成背景层

所以它是：

- **可以直接承接 resident actor 的混合锚点**

### 4. `NightWitness_01`

它不该理解成“夜里才生成的人”。

它更像：

- **本来就在村边 / 墓坡 / 夜路一带的人，被夜里那一拍看见**

所以它的正确口径是：

1. actor 先存在于 `Resident_BackstagePresent`
2. 夜间需要时，再被导演接管到 `NightWitness_01`

### 5. `DailyStand_01~03`

这三组最像真正的下一阶段 resident anchor。

我现在把它们正式归类为：

- **可直接承接 future resident actor 的常驻位**

也就是说，这三组以后不该主要被理解成“导演临时 cue 点”，而是：

1. 次日生活在场证据
2. 真实村庄照常运转的默认站位

---

## 四、哪些 NPC 在玩家尚未入村时就应该已经算存在

### 1. 明确应该已存在，只是玩家未必看见

我现在认为下面这些人，在玩家入村前就应该已经算存在：

1. `101`
2. `103`
3. `104`
4. `201`
5. `202`
6. `203`

原因很简单：

他们更像：

1. 日间在村里的生活层
2. 工位 / 饭馆 / 路边 / 村内背景层

### 2. 也应存在，但更偏 offscreen / edge

我现在认为：

- `102`

也应该已存在，但更像：

1. 村边
2. 远侧
3. 夜间前后更容易被玩家真正撞见

所以它适合先挂在：

- `Resident_BackstagePresent`

### 3. 语义上存在，但不应在开篇就被玩家直接看到

我现在认为：

- `301`

最适合归到：

1. 语义上已经存在
2. 但当前不该正面进场
3. 夜里才被视线和导演真正钉出来

所以它最适合先挂：

- `Resident_BackstagePresent`

而不是：

- 一开始就站在玩家视野里

---

## 五、从 `Primary` 代理迁回 `Town`，最小 scene-side contract 该守什么

我现在给一版最小 contract，不做宏大方案，只钉“以后别重写”。

### 必须固定不再乱动的 5 件事

1. `SCENE/Town_Day1Carriers` 这个根名不再改
2. 它下面这 7 个 child 名不再改：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01`
   - `DailyStand_02`
   - `DailyStand_03`
3. 后续新增的 resident 根名固定为：
   - `SCENE/Town_Day1Residents`
4. 这 3 个 resident group root 名固定为：
   - `Resident_DefaultPresent`
   - `Resident_DirectorTakeoverReady`
   - `Resident_BackstagePresent`
5. `Primary` 的 `001 / 002 / 003` 只继续作为代理承接，不再被理解成长期 scene contract

### 迁回时最小要对齐的不是“所有 runtime 行为”，而是这 4 件事

1. anchor 名一致
2. resident 根层一致
3. actor 默认存在语义一致
4. director takeover 入口一致

只要这 4 件事一致，后面从 `Primary` 代理迁回 `Town` 时，不需要整套重写。

---

## 六、Town 当前自己这边最窄的第一 scene-side blocker

我现在把它压成一句话：

- **`Town` 还没有 resident root，而且 `Town_Day1Carriers` 下面的 7 个锚点现在全是 `(0,0,0)` 的空壳。**

这就是当前最窄、最真实的 scene-side blocker。

它不是：

- Town 整体没 ready

它也不是：

- 导演语义没写清

它就是：

1. 常驻层还没有
2. carrier 还没变成真实空间承接位

---

## 七、你什么时候该把球正式交回我，交回时带哪些触点

### 什么时候回

你命中下面任意一条时，就该把球正式交回我：

1. 你不再只做代理 / 过渡，而是真的要让 `Town` 开始承接 resident scene-side
2. 你要把 `Primary` 代理的存在层迁回 `Town`
3. 你需要有人真正去写 `Town.unity` 的 resident 根层和 carrier 空间位

### 回球时最少一起交的触点

1. `Assets/000_Scenes/Town.unity`
2. `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`

如果后面要连 resolver 一起改，再额外交：

3. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

但这第三项，不是这轮必须回来的。

---

## 八、我这边现在的真实位置

我现在不是回到“Town 还不行”的老口径。

我现在的位置是：

1. 接受方向切到驻村常驻化
2. 已把 `Town` 这边的 resident root / carrier / backstage 语义分开
3. 已把迁回 `Town` 时最小 scene-side contract 压实
4. 但这轮不去越权碰你当前不该抢的 active 文件

一句话总结：

- **你现在可以继续做代理和过渡；一旦你要把“本来就在村里的人”真的迁回 `Town`，球就该正式回到我这里。**
