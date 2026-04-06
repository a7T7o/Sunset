# 2026-04-06｜给 spring-day1｜Town 相机 / 转场 / 玩家位 contract probe 与回球阈值

这份回执只做一件事：

把 `Town` 下一安全切片里最容易先撞上的 `相机 / 转场 / 玩家位` 这一层，从“手读 scene 推断”推进到“已被 Unity Editor 菜单实际跑过的 scene-side contract probe”。

---

## 一、先说结论

### 1. 这轮我没有继续写 `Town.unity`

不是因为这条线不重要。

而是因为我先重核了当前 mixed dirty 现场后确认：

- `Town.unity`
- `CameraDeadZoneSync.cs`

这两处这轮都不适合在没有再次接刀的前提下继续直写。

所以我这轮选择的推进方式是：

- 不硬碰 mixed dirty
- 先把 `Town` 入口这一层做成一个可重复跑的 probe
- 先把“当前到底站没站住”判死

### 2. 这轮已经拿到了真实 probe 结果，而且结果是 `completed`

当前我已经通过 Unity 编辑器命令桥实际执行：

- `Tools/Sunset/Scene/Run Town Entry Contract Probe`

并真实生成：

- `Library/CodexEditorCommands/town-entry-contract-probe.json`

当前结果是：

- `status = completed`
- `success = true`
- `blockingFindings = 0`
- `attentionFindings = 0`

也就是说，至少在 `Town` 的**入口 contract** 这一层：

- `Main Camera`
- `CinemachineCamera`
- `CameraDeadZoneSync`
- `Player / PlayerMovement`
- `SceneTransitionTrigger2D -> Primary`

当前已经形成可直接消费的 scene-side 契约。

### 3. 这意味着 `Town` 当前第一 blocker 已经不再是入口层

这轮之后，`day1` 不该再把 `Town` 当前第一问题理解成：

- 入口相机链没站住
- 转场对象没对齐
- 玩家对象没挂好

这几项我这轮都已经拿到了真实 probe 的 `completed` 结果。

更诚实的改判应该是：

- `Town` 的 resident contract：已站住
- `Town` 的 entry contract：已站住
- `Town` 当前更可能的下一撞点，已经转到更深一层的 runtime 消费或玩家体验链

---

## 二、这轮真实新增了什么

### 1. 新增 Editor probe 入口

已新增：

- `Assets/Editor/Town/TownSceneEntryContractMenu.cs`

菜单路径：

- `Tools/Sunset/Scene/Run Town Entry Contract Probe`

### 2. 真实输出文件

运行后会输出：

- `Library/CodexEditorCommands/town-entry-contract-probe.json`

### 3. 当前 probe 实际覆盖面

当前这份 probe 会检查：

- `Main Camera` 是否存在、是否是 `MainCamera` tag、是否有 `AudioListener`、是否有 `CinemachineBrain`
- `CinemachineCamera` 是否存在
- `CameraDeadZoneSync` 是否存在，且是否显式引用当前 `Main Camera / CinemachineCamera / boundingCollider`
- `Player` 是否存在、是否仍是 `Player` tag、是否挂有 `PlayerMovement`
- `SceneTransitionTrigger2D` 是否存在、是否可触发、是否仍然返回 `Primary`

### 4. 这轮真实跑出的结果

当前 probe JSON 的关键事实如下：

- `Main Camera`
  - `hasMainCameraTag = true`
  - `hasAudioListener = true`
  - `hasCinemachineBrain = true`
- `CinemachineCamera`
  - 已存在
  - `CameraDeadZoneSync` 已挂载
  - `mainCamera / cinemachineCamera / boundingCollider` 都已显式连上
- `Player`
  - `PlayerMovement` 存在
  - `taggedPlayer = true`
- `SceneTransitionTrigger2D`
  - `hasCollider2D = true`
  - `colliderIsTrigger = true`
  - `triggerOnPlayerEnter = true`
  - `playerTag = Player`
  - `targetSceneName = Primary`
  - `targetScenePath = Assets/000_Scenes/Primary.unity`

---

## 三、给 day1 的直接消费口径

### 1. 你现在可以直接信什么

你现在可以直接把下面这句话当真值继续往前走：

`Town` 当前入口这一层的 `相机 / 玩家 / 转场` scene-side contract，已经不只是“结构上看起来像对”，而是已经被 Unity Editor probe 实际跑成 `completed`。

### 2. 你现在不要再把什么误判成 Town 首撞点

至少在下一轮里，不要再优先把这些怀疑扔回 Town：

- `Town Main Camera 没脑子`
- `CinemachineCamera 没连上`
- `CameraDeadZoneSync` 没挂或没引用`
- `Town 玩家对象不是 Player`
- `Town 返回 Primary 的 SceneTransition 配置不对`

因为这一层这轮已经被 probe 实际判过。

### 3. 你下一次继续吃 Town 时，先按什么顺序判断

当前建议顺序是：

1. 先继续推进你自己的 runtime 消费链
2. 只有在真实玩家现场再次出现以下类型问题时，再把球回给我：
   - 进 Town 后镜头不跟
   - 玩家落位错误或进场点不对
   - Town -> Primary 返回链实际失效
   - Town 内 camera/trigger/player 的真实 live 行为与 probe 结果不一致

也就是说：

- probe 已经证明 scene-side 入口 contract 站住
- 但它还没有替代完整玩家体验验收

---

## 四、当前不可代接边界

### 1. 我这轮仍然没有直接代接的面

这轮我仍然没有去继续直写：

- `Assets/000_Scenes/Town.unity`
- `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`

原因很简单：

- 它们当前仍处于 mixed dirty 面
- 这轮继续硬写，收益不如先把 probe 真跑出来

### 2. 这轮 probe 也不等于完整 live-ready

这轮 probe 已经证明：

- scene-side entry contract 当前是完整的

但还没有证明：

- 所有更深的 Town runtime 行为都已经完全闭环
- 所有导演消费、驻村常驻化迁回链都已经完全无后顾之忧

所以当前正确口径只能是：

- `Town 入口层已过`
- `更深 runtime/player-facing 层仍按真实现象继续窄口径推进`

---

## 五、回球阈值

### 1. 什么情况下不用回球给我

如果你继续推进 `day1` 时：

- `Town` 能正常进入
- 镜头跟随正常
- 玩家落位正常
- 返回 `Primary` 正常

那这条线暂时不用再回球给我。

### 2. 什么情况下应该第一时间回球给我

只要命中以下任一类，就该回球：

1. 真实 live 行为和 probe 结果相反
2. 玩家在 Town 的入口位出现明显错位
3. 镜头进入 Town 后不跟或 confiner 明显失效
4. Town 返回 `Primary` 的 trigger 失效

### 3. 回球时优先给我哪一个触点

回球优先级固定为：

1. `Town.unity` 中的 `Main Camera / CinemachineCamera / SceneTransitionTrigger`
2. `CameraDeadZoneSync.cs`
3. 再往下才是更深的 runtime/player-facing 触点

---

## 六、这轮之后 Town 当前第一 blocker 改判

这轮之后，`Town` 当前第一 blocker 不再是：

- resident slot
- entry camera/transition/player contract

当前更真实的第一 blocker 改判为：

- 当 `day1` 真正继续吃更深 Town runtime 时，下一撞点更可能出现在更深的消费链或真实玩家体验层，而不是入口 scene-side 基础合同层

---

## 七、一句话给 spring-day1

`Town` 这边当前已经把 resident contract 和 entry contract 都站住了；你可以继续推进自己的 runtime 消费，不需要再先为 Town 的入口相机 / 转场 / 玩家位兜底，除非真实 live 现象再次和这份 probe 结果相反。
