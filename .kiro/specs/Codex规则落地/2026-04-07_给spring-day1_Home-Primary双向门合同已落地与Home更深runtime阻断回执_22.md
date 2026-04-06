# Home <-> Primary 双向门合同已落地与 Home 更深 runtime 阻断回执

## 1. 当前主线

把 `Home` 从“只有住处 contract、没有正式双向门”推进到：

- `Home -> Primary` 真正可回
- `Primary -> Home` 真正有门可挂
- 并且这条线不只靠口头说明，而是有 Editor 菜单和 JSON probe 可重复验

## 2. 这轮真实做成了什么

1. 新增 [HomePrimaryDoorContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/HomePrimaryDoorContractMenu.cs)
   - 菜单：
     - `Tools/Sunset/Scene/Setup Home <-> Primary Door Contract`
     - `Tools/Sunset/Scene/Run Home <-> Primary Door Contract Probe`
2. 已真实执行 `Setup Home <-> Primary Door Contract`
   - `HomeDoor` 现在显式带：
     - `BoxCollider2D`
     - `SceneTransitionTrigger2D(target = Primary)`
   - `Primary/2_World` 现在已新增：
     - `Primary_HomeContracts`
     - `PrimaryHomeDoor`
     - `PrimaryHomeEntryAnchor`
   - `PrimaryHomeDoor` 现在显式带：
     - `BoxCollider2D`
     - `SceneTransitionTrigger2D(target = Home)`
3. 已真实执行 `Run Home <-> Primary Door Contract Probe`
   - 输出：
     - [home-primary-door-contract-probe.json](/D:/Unity/Unity_learning/Sunset/Library/CodexEditorCommands/home-primary-door-contract-probe.json)
4. 已再次重跑旧的 `Home Rest Contract Probe`
   - `HomeDoor` 那条“没有 exit component”的旧 attention 已被清掉

## 3. 当前已被 live 结果站住的事实

`home-primary-door-contract-probe.json` 当前返回：

- `status = attention`
- `success = true`
- `firstBlocker = ""`

并且明确站住：

1. `HomeDoor`
   - 路径：`SCENE/Home/Home_Contracts/HomeDoor`
   - 已有 trigger collider
   - 已有 `SceneTransitionTrigger2D`
   - `targetSceneName = Primary`
   - `targetScenePath = Assets/000_Scenes/Primary.unity`
2. `PrimaryHomeDoor`
   - 路径：`Primary/2_World/Primary_HomeContracts/PrimaryHomeDoor`
   - 已有 trigger collider
   - 已有 `SceneTransitionTrigger2D`
   - `targetSceneName = Home`
   - `targetScenePath = Assets/000_Scenes/Home.unity`
3. `PrimaryHomeEntryAnchor` 已存在，且层级在 `PrimaryHomeDoor` 下
4. 旧的 `home-rest-contract-probe.json` 现在也已经认到：
   - `HomeDoor.hasExitComponent = true`
   - `exitComponentType = SceneTransitionTrigger2D`
   - `targetSceneName = Primary`

## 4. 当前最重要的新判断

`Home <-> Primary` 的门合同已经站住了。

但 `Home` 还 **没有** 进入“像 Town 一样可自由跑”的最终状态。

当前第一 blocker 已从“没有门”改判为：

- `Home` 缺 scene-local runtime 基础链

probe 里已经直接报实：

- `homeHasPlayerMovement = false`
- `homeHasGameInputManager = false`
- `homeHasNavigationRoot = false`
- `homeHasCinemachineCamera = false`

## 5. 为什么我没有继续硬补成 Town 级 runtime

因为这已经不是“再补一个门物体”这么简单，而是会进入一条更重的 scene-side 迁移：

1. `Player`
2. `GameInputManager`
3. `InventorySystem / HotbarSelection / PackagePanel / UI`
4. `NavigationRoot`
5. `CinemachineCamera / 跟随链`

更关键的是，这些对象在 `Primary` 里互相带大量 scene-local 引用。

如果现在直接生搬硬拷，很容易做出：

- 门能进
- 但进去后输入链是空的
- 或 UI / inventory / camera 挂着跨场景旧引用

这种“结构上看起来更完整，运行时更假”的坏闭环。

## 6. 当前对你最有价值的结论

你现在可以把 `Home` 理解成两层状态：

### 已过线

- `Home` 自己的住处 contract 已可用
- `Home <-> Primary` 双向门合同已落地
- 两条日志链都能直接看：
  - `home-rest-contract-probe.json`
  - `home-primary-door-contract-probe.json`

### 还没过线

- `Home` 还不是 `Town` 等级的完整 runtime scene
- 现在如果目标改成“像 Town 一样能自由走、能自由出门、完整玩家链都在屋内”，那已经是下一刀，不再是本轮这个门合同的自然尾巴

## 7. 你现在只需要做什么

如果你当前只想把门位摆好：

1. `Home`
   - `Home_Contracts/HomeDoor`
   - `Home_Contracts/HomeDoor/HomeEntryAnchor`
2. `Primary`
   - `2_World/Primary_HomeContracts/PrimaryHomeDoor`
   - `2_World/Primary_HomeContracts/PrimaryHomeDoor/PrimaryHomeEntryAnchor`

你移动这些位置即可。

## 8. 如果下一轮要继续，最值钱的唯一方向

不是再补门。

而是单开一轮：

- `Home runtime baseline seed`

并且要明确按“最小可跑室内场景”去审：

1. 是否要给 `Home` 增 scene-local `Player`
2. 是否要给 `Home` 补最小 `GameInputManager + UI 依赖`
3. 是否只做静态室内镜头，还是正式补 `CinemachineCamera`
4. `NavigationRoot` 对小屋内是不是必须

在这四件事没被单独审清前，不应把当前状态包装成“Home 已经完全像 Town 一样可自由跑”。
