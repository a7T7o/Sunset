# 2026-04-06｜给 spring-day1｜Town 更深 player-facing contract 与下一撞点改判

这份回执只做一件事：

- 把 `Town` 从“入口组件契约已对齐”继续推进到更深的 `player-facing` 一层
- 让你现在能直接判断：`Town` 对 `day1` 的下一步支持，已经站住到哪，第一撞点又改判到了哪

---

## 一、先说结论

### 1. `Town` 现在已经不只是入口 contract 过线

这轮我没有回去重写 `Town.unity`，而是补了一份更深的 Editor probe，专门验证玩家真正会感受到的第一层体验：

1. 玩家起步位是不是安全
2. 相机是不是真的跟住玩家
3. 返回 `Primary` 的 trigger 会不会一进场就反咬
4. 入口第一屏该看到的 `Town` 群像锚点是不是已经在视野里

这层现在已经真实跑成：

- `status = completed`
- `success = true`

### 2. 当前 `Town` 的 player-facing 首层已经站住

这轮新 probe 的关键事实如下：

1. `CinemachineCamera -> Player`
   - tracking target 已真实对齐 `Player`
   - virtual camera 的 XY 位置也与玩家起步位对齐
2. 玩家起步位
   - 在 `_CameraBounds` 内
   - 当前不在返回 `Primary` 的 trigger 里
   - 与返回 trigger 的边缘距离约 `2.83`
3. 入口第一屏
   - `EnterVillageCrowdRoot` 在首屏内
   - `KidLook_01` 也在首屏内
   - 也就是说，玩家一进 `Town` 的第一眼不该再是“空场”

### 3. 这轮之后，`Town` 的第一 blocker 再次改判

这轮之后，`Town` 当前第一 blocker 已经不再是：

1. 入口相机链没对齐
2. 玩家起步位会立刻误触返回
3. 入口第一屏没有群像锚点

这些这轮都已经被更深 probe 站住了。

当前更真实的第一撞点改判为：

- 当你继续把 `day1` 往更深 `Town runtime / scene live` 消费推进时，下一撞点更可能出现在：
  - `DinnerBackgroundRoot`
  - `NightWitness_01`
  - `DailyStand_01`
  这类更深 anchor 的实际演员消费、驻村 deployment、以及后续 live 承接链

---

## 二、这轮我真实新增了什么

### 1. 新增更深 probe 菜单

已新增：

- `Assets/Editor/Town/TownScenePlayerFacingContractMenu.cs`

菜单路径：

- `Tools/Sunset/Scene/Run Town Player-Facing Contract Probe`

### 2. 新增真实输出文件

运行后会输出：

- `Library/CodexEditorCommands/town-player-facing-contract-probe.json`

### 3. 这份 probe 当前覆盖的 player-facing 面

它现在会查：

1. `Main Camera / CinemachineCamera`
2. `CinemachineCamera` 的 tracking target 是否真指向 `Player`
3. virtual camera 是否已站在玩家起步位
4. `Player` 是否在 `_CameraBounds` 内
5. 返回 `Primary` 的 `SceneTransitionTrigger` 是否在 bounds 内、是否与玩家起步位重叠
6. 玩家到返回 trigger 的边缘安全距离
7. `EnterVillageCrowdRoot / KidLook_01` 是否已经处在玩家第一屏里
8. 其他关键 runtime anchor 是否仍在 `_CameraBounds` 内

---

## 三、这轮实际跑出的关键结果

### 1. 玩家起步安全

当前结果：

1. `Player = (-37.010, 16.190)`
2. `_CameraBounds = (-41,-17) -> (13,49)`
3. 玩家在 bounds 内
4. 玩家不与返回 trigger 重叠
5. 玩家到返回 trigger 的边缘距离约 `2.83`

这意味着：

- `Town` 当前不再是“一进场就站在回切边缘上”的状态

### 2. 相机真实跟随基线

当前结果：

1. `CinemachineCamera.TrackingTarget = Player`
2. virtual camera XY 与玩家起步位对齐

这意味着：

- `Town` 当前不只是“有相机组件”，而是玩家起步镜头链已经对齐到了能直接服务玩家体验的层

### 3. 入口第一屏演出锚点

当前结果：

1. `EnterVillageCrowdRoot`
   - 在 `_CameraBounds` 内
   - 在初始视野内
   - 距玩家约 `2.54`
2. `KidLook_01`
   - 在 `_CameraBounds` 内
   - 在初始视野内
   - 距玩家约 `6.62`

这意味着：

- 你后面继续推进 `Town` 入口相关的群像消费时，不该再把“第一屏太空、进村第一眼看不到人”当成默认怀疑项

### 4. 更深 anchor 的当前位置性质

当前结果：

1. `DinnerBackgroundRoot`
2. `NightWitness_01`
3. `DailyStand_01`

它们都已经在 `_CameraBounds` 内，但不在玩家初始第一屏里。

这不是 bug。

更诚实的解释是：

- `Town` 当前已经把“入口 player-facing 首层”站住了
- 但更深 `mid-town / night / next-day` 的消费仍然是下一层问题

---

## 四、给 spring-day1 的直接消费口径

### 1. 你现在可以直接信什么

你现在可以把下面这句话当真值继续用：

`Town` 当前不只入口组件对齐了，连“玩家进 Town 的第一层体验”也已经站住了：镜头跟玩家、起步位不咬回切、第一屏能看到入口群像锚点。

### 2. 你现在不要再优先把什么扔回 Town

至少下一轮里，不要再默认优先怀疑这些：

1. `Town` 进场镜头没跟玩家
2. 玩家一进场就贴着返回 `Primary` 的 trigger
3. 入口第一屏没有 `EnterVillageCrowdRoot / KidLook_01`

因为这层这轮已经被真实 probe 判过。

### 3. 你现在继续吃 Town 时，第一关注点该改到哪

当前更值得你继续关注的是：

1. `DinnerBackgroundRoot`
2. `NightWitness_01`
3. `DailyStand_01`

也就是：

- 更深的 runtime actor 消费
- resident deployment / 迁回承接
- 更深 live 行为与 scene-side 的一致性

---

## 五、当前回球阈值

### 1. 下面这些，不需要先回球给我

如果你继续推进 `day1` 时：

1. 进 `Town` 后镜头正常跟随
2. 玩家起步位正常
3. 不会被立刻弹回 `Primary`
4. 入口第一屏群像仍正常

那这层暂时不用回球给我。

### 2. 下面这些，再第一时间回球给我

只要命中以下任一类，就该回球：

1. 真实 live 里镜头不再跟玩家
2. 玩家起步位错了，或一进场就贴回切 trigger
3. 入口第一屏重新变空
4. 更深 anchor 的 live 消费开始出现“scene-side 明明有位，但 runtime 拉人过去后体验不成立”

---

## 六、一句话给 spring-day1

`Town` 这边当前已经从“入口 contract 已过”继续推进到“玩家进 Town 的首层体验已站住”；你现在可以把注意力继续往更深的 `DinnerBackground / NightWitness / DailyStand` runtime 消费推进，而不是再先为入口镜头、起步位和第一屏群像兜底。
