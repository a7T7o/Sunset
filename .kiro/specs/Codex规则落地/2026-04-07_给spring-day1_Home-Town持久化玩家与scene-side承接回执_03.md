# 2026-04-07｜给 `spring-day1`：`Home / Town` 持久化玩家与 scene-side 承接回执

## 1. 这轮我实际做成了什么

### 1.1 持久化玩家主链已经落代码

- 已修改 [SceneTransitionTrigger2D.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs)
  - 新增 `targetEntryAnchorName`
  - 新增 `TargetEntryAnchorName` 读取口
  - `TryStartTransition()` 现在会把目标 scene + entry anchor 交给 runtime bridge
  - 新增入场 grace，避免切场后一落地就被门口 trigger 反咬回去
  - 对 `HomeDoor -> PrimaryHomeEntryAnchor` 与 `PrimaryHomeDoor -> HomeEntryAnchor` 补了代码级 fallback，不要求 scene 先手工补字段也能吃入口锚点

- 已新增 [PersistentPlayerSceneBridge.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
  - 运行时保证只保留一只 `Player`
  - 首次开局会接管当前 scene player 并转成 `DontDestroyOnLoad`
  - 后续切场优先吃 `entry anchor`
  - 找不到 entry anchor 时，回退到目标 scene 里原本 player placeholder 的位置
  - 会回收 `Primary / Town` 的重复 scene-local player
  - 会把 `PlayerAutoNavigator` 重新绑到当前 scene 的 `NavGrid2D`
  - 会把当前 scene 里的 `GameInputManager` 私有引用重绑回 persistent player
  - 会在无 `CinemachineCamera` 的场景里接管一个轻量 fallback camera follow
  - 会在无 `GameInputManager` 的场景里提供最小轴向移动 fallback

- 已修改 [PlayerAutoNavigator.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs)
  - 新增 `BindRuntimeSceneReferences(...)`
  - 允许切场后把 `movement / player / navGrid` 重新绑回当前 scene

### 1.2 Home / Primary 的 scene-side 合同仍然成立

- 已重跑 `Tools/Sunset/Scene/Run Home <-> Primary Door Contract Probe`
- 结果文件：[home-primary-door-contract-probe.json](/D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/home-primary-door-contract-probe.json)
- 当前结论：
  - `status = attention`
  - `success = true`
  - `HomeDoor -> Primary`
  - `PrimaryHomeDoor -> Home`
  - 两边 door / entry anchor / trigger collider / target scene 都还在

### 1.3 Town scene-side 当前可继续被 day1 消费

- 已重跑 `Tools/Sunset/Scene/Run Town Player-Facing Contract Probe`
- 结果文件：[town-player-facing-contract-probe.json](/D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/town-player-facing-contract-probe.json)
- 当前结论：
  - `trackingTargetMatchesPlayer = true`
  - `virtualCameraXYMatchesPlayer = true`
  - `EnterVillageCrowdRoot / KidLook_01 / DinnerBackgroundRoot / NightWitness_01 / DailyStand_01` 都存在
  - 当前不是 blocked，而是一个 scene-side attention：
    - 玩家起步位距离返回 `Primary` 的 trigger 只剩 `0.97`

## 2. 我拿到的最值钱真值

### 2.1 persistent player 不是纸面设计，已拿到 runtime 现场

- 通过新菜单 [PersistentPlayerSceneRuntimeMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/PersistentPlayerSceneRuntimeMenu.cs) 写出：
  - [persistent-player-scene-probe.json](/D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/persistent-player-scene-probe.json)
- 关键值：
  - `hasPersistentPlayerBridge = true`
  - `totalPlayerCount = 1`
  - `activeScenePlayerCount = 0`
  - `dontDestroyOnLoadPlayerCount = 1`
  - `persistentPlayerScene = DontDestroyOnLoad`
- 这说明：
  - `Primary` 运行时的 scene-local player 已经被接管掉
  - 当前活着的是一只跨场景保留玩家

### 2.2 这轮 runtime 终验没完全闭环，但 blocker 已查实

- 我尝试用 runtime 菜单直接触发：
  - `Tools/Sunset/Runtime/Trigger Primary Home Door`
- 结果没有拿到稳定的 `Primary -> Home -> Primary` 完整闭环
- 当前第一 blocker 不是这条桥接代码 own compile red，而是：
  - 进入 `Play` 后 Unity 现场会冒出一批 `The referenced script (Unknown) on this Behaviour is missing!`
  - 这批错误没有指向我的新文件，且会污染 runtime 取证
- 更诚实的口径是：
  - `持久化玩家主链已落地`
  - `scene-side contract 已在 edit-mode 站住`
  - `runtime 门链终验仍被外部 play 现场 missing-script blocker 卡住`

## 3. 现在 day1 可以放心吃什么

- 可以继续把 `Town` 当成 day1 的导演语义承接层
- 可以继续把 `Town_Day1Carriers` 这些 anchor 当作稳定承接位
- 可以把 `Primary / Town` 看作已经开始吃“唯一玩家跨场景保留”这条新底座
- 不要把 `Home` 再当成“必须先补 scene-local Player / GameInput / Nav / Camera 才能继续”的老模型
  - 这部分现在已经改成：
    - `persistent player + scene-local camera or fallback + Home 最小输入 fallback`

## 4. 现在还不能被我包装成已过线的部分

- 我还不能说：
  - `Primary -> Home -> Primary` runtime 已完整自测通过
- 因为这轮 runtime 终验被外部 play 现场污染了
- 同样我也不能说：
  - `Town` 已完全没有 player-facing 风险
- 目前 `Town` 还留着一个 attention：
  - 玩家起步位距离返回 trigger 太近

## 5. 我对 day1 的直接建议

- 你现在不用回头抢我这边的 persistent player 主链代码
- 你可以直接把它当作：
  - `Town / Home` 已经开始具备跨场景保留玩家的承接底座
- 如果你后续要继续压 runtime，更值得优先关注的是：
  1. `Town` 返回 `Primary` trigger 的误触边缘
  2. `Play` 现场那批 `missing script` 外部红到底来自哪些对象
  3. `HomeEntryAnchor / PrimaryHomeEntryAnchor` 目前仍与 door 同位，虽然现在有入场 grace 兜底，但长期仍建议手摆出真正落点

## 6. 我这轮的自我判断

- 我这轮最核心的判断：
  - `Home / Town` 这条线最值钱的一刀已经不再是继续加 scene-local player，而是把“唯一玩家跨场景保留”真正落下去
- 我为什么认为它成立：
  - runtime probe 已经直接证明运行中只剩一只 `DontDestroyOnLoad` 玩家
  - edit-mode probe 证明 `Home / Primary / Town` 的 scene-side 合同没有被打坏
- 我这轮最薄弱的地方：
  - `Primary -> Home -> Primary` 的完整 runtime 门链没拿到最终过线证据
  - 当前只能诚实停在 `外部 play 现场 blocker`
