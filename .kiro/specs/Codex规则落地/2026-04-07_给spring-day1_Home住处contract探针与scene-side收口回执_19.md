# Home 住处 contract 探针与 scene-side 收口回执

## 1. 当前主线

把 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 从“语义第一层刚落地”继续推进到“更自持的住处 scene-side contract”，并把这条线收成像 `Town` 一样能直接看日志判定的程度。

## 2. 这轮实际做成了什么

1. 已把你手摆后的 `Home` 门位真正落盘：
   - direct MCP 读到 live 现场里：
     - `Home_Contracts.position = (-18.03, -5.93, 0)`
   - 当时 Unity 明确报：
     - `active_scene = Home`
     - `isDirty = true`
   - 随后我直接执行了场景保存，所以现在磁盘版 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 已经带着这组坐标。
2. 已把 `HomeBed` 从“只有 trigger 锚点”推进到“更自持的住处承载位”：
   - 在 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 上，给 `HomeBed` 直接补了 `SpringDay1BedInteractable`
   - 这样它不再只依赖 `day1` 运行时临时补口，scene 自己就知道“这是一张可休息的床”
3. 已新增 `Home` 自己的 editor probe：
   - [HomeSceneRestContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/HomeSceneRestContractMenu.cs)
   - 菜单：
     - `Tools/Sunset/Scene/Run Home Rest Contract Probe`
   - 输出目标：
     - `Library/CodexEditorCommands/home-rest-contract-probe.json`
4. 这份 probe 当前会检查：
   - `Main Camera / AudioListener`
   - `PersistentManagers`
   - `Home_Contracts / HomeDoor / HomeEntryAnchor / HomeBed`
   - `HomeBed` 的 `Collider2D/isTrigger`
   - `HomeBed` 是否显式挂 `SpringDay1BedInteractable`
   - `HomeDoor` 当前有没有 scene exit 组件
   - 上述关键节点是否在主相机初始视野里
5. 代码侧最小闸门已过：
   - `git diff --check -- Assets/Editor/Home/HomeSceneRestContractMenu.cs Assets/000_Scenes/Home.unity` = clean
   - `manage_script validate --name HomeSceneRestContractMenu --path Assets/Editor/Home --level standard` = `clean`

## 3. 当前还没做成什么

1. 新 probe 的菜单还没拿到 live 成功执行结果。
2. `HomeDoor` 仍没有正式 scene exit 组件，所以 `Home` 现在更像“住处/休息 interior 已站住”，还不是“完整可自由进出的室内场景”。

## 4. 当前第一 blocker

当前第一 blocker 已经不是 `Home` own，而是外部 fresh compile red：

- [PackageSaveSettingsPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs):275

这条外部红会直接卡住 editor menu 注册链，所以我这边新加的菜单当前执行时会报：

- `there is no menu named 'Tools/Sunset/Scene/Run Home Rest Contract Probe'`

## 5. 当前阶段

`Home` 当前已经从：

- `住处语义层已落地`

继续推进到了：

- `住处 contract 更自持`
- `probe 已落代码`
- `live 菜单执行被 external red 挡住`

## 6. 下一步只做什么

下一步最值钱的只有一件事：

1. 等 [PackageSaveSettingsPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs):275 这条 external red 清掉
2. 立刻重跑：
   - `Tools/Sunset/Scene/Run Home Rest Contract Probe`
3. 读取：
   - `Library/CodexEditorCommands/home-rest-contract-probe.json`

## 7. 需要用户现在做什么

暂时不需要你再摆位置了。

## 8. 这轮最核心的判断

`Home` 现在最值钱的不是强行给 `HomeDoor` 猜一个错误的出门转场，而是先把“住处自己能站住”和“以后怎么直接看日志验它”这两层做实。

## 9. 为什么我认为这个判断成立

因为当前真实缺口已经很清楚：

1. `HomeBed/HomeDoor/HomeEntryAnchor` 语义位已经有了
2. 你手摆的位置也已经保存进场景了
3. `HomeBed` 现在连 `SpringDay1BedInteractable` 都直接在 scene 里了
4. 剩下没闭环的不是 scene-side 本体，而是 `probe menu` 被别处的 compile red 卡住

## 10. 这轮最薄弱、最可能看错的点

最薄弱的点是：

- `HomeDoor` 的最终出门 contract 仍然没做

## 11. 自评

这轮我给自己 `9/10`。

## 12. No-Red 证据卡 v2

- `cli_red_check_command`: `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/Editor/Home/HomeSceneRestContractMenu.cs --count 20 --output-limit 5`
- `cli_red_check_scope`: `Assets/Editor/Home/HomeSceneRestContractMenu.cs`
- `cli_red_check_assessment`: `external_red`
- `unity_red_check`: `blocked`
- `mcp_fallback`: `required`
- `mcp_fallback_reason`: `blocked`
- `current_owned_errors`: `0`
- `current_external_blockers`: `[PackageSaveSettingsPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs):275`
- `current_warnings`: `0`
