# Home 住处 contract 探针 live 结果与剩余 attention 回执

## 1. 当前主线

在 [HomeSceneRestContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/HomeSceneRestContractMenu.cs) 已经落下之后，继续把 `Home` 从“probe 代码已存在”推进到“probe 菜单真实跑通并产出可读 JSON”。

## 2. 这轮实际做成了什么

1. 重新开了一个极小 live 验证 slice，只接：
   - [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity)
   - [HomeSceneRestContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/HomeSceneRestContractMenu.cs)
2. 直接执行了菜单：
   - `Tools/Sunset/Scene/Run Home Rest Contract Probe`
3. 这次菜单已真实成功执行，不再是“there is no menu named ...”
4. 已读取产物：
   - `Library/CodexEditorCommands/home-rest-contract-probe.json`
5. fresh console 这次也已回到：
   - `0 error / 0 warning`

## 3. 当前 live 结果

本次 `home-rest-contract-probe.json` 返回：

- `status = attention`
- `success = false`
- `firstBlocker = ""`

也就是说：

- `Home` 当前已经没有 blocking failure
- 但还有几条需要继续收口的 attention

## 4. 已被 live 证据站住的事实

1. `Main Camera` 存在，且：
   - `MainCamera tag = true`
   - `AudioListener = true`
   - `orthographic = true`
2. `PersistentManagers` 存在
3. `Home_Contracts / HomeDoor / HomeEntryAnchor / HomeBed` 全部存在，且层级关系正确：
   - `HomeDoor` 在 `Home_Contracts` 下
   - `HomeEntryAnchor` 在 `HomeDoor` 下
4. `Home_Contracts` 的手摆位置已经真实落到 probe 结果里：
   - `contractsPosition = (-18.030, -5.930, 0.000)`
5. `HomeBed` 已经站到更自持的一层：
   - `Collider2D = true`
   - `isTrigger = true`
   - `tag = Interactable`
   - `SpringDay1BedInteractable = true`

## 5. 当前剩余 attention

当前 probe 给出的 attention 有 5 条：

1. `PersistentManagers` 仍未显式配置 `PrefabDatabase`
2. `Home_Contracts` 当前不在主相机初始视野里
3. `HomeDoor` 当前不在主相机初始视野里
4. `HomeEntryAnchor` 当前不在主相机初始视野里
5. `HomeDoor` 还没有显式的 scene exit 组件

## 6. 当前阶段

`Home` 现在已经从：

- `scene-side 语义层已落地`
- `probe 代码已落`

继续推进到了：

- `live probe 已真实跑通`
- `剩余问题已经从 blocker 收缩成 attention`

## 7. 当前第一判断

`Home` 这条线当前已经不是“缺基础设施”，而是“住处 rest contract 已成立，剩下的是体验和出口层的升级项”。

## 8. 下一步只做什么

如果后续继续 `Home`，最值钱的顺序是：

1. 判定 `PersistentManagers.prefabDatabase` 是否该在 `Home` 显式配置
2. 判定 `HomeDoor/HomeEntryAnchor` 是否需要继续保持在屋内镜头外，还是应该改成玩家一进屋就能看到门位
3. 最后再决定 `HomeDoor` 要不要挂正式的 scene exit 组件，以及它到底该回哪一个外部落位

## 9. 需要用户现在做什么

暂时不需要。

## 10. No-Red 证据卡 v2

- `cli_red_check_command`: `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 10 --output-limit 5`
- `cli_red_check_scope`: `Home live probe retry`
- `cli_red_check_assessment`: `no_red`
- `unity_red_check`: `pass`
- `mcp_fallback`: `required`
- `mcp_fallback_reason`: `scene_live_flow_required`
- `current_owned_errors`: `0`
- `current_external_blockers`: `0`
- `current_warnings`: `0`
