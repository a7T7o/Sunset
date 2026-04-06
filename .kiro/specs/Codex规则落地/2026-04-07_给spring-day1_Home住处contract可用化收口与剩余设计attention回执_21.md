# Home 住处 contract 可用化收口与剩余设计 attention 回执

## 1. 当前主线

把 `Home` 从“探针能跑、但结果看起来像失败”继续收成一个真正可交接的屋内住处承接层：  
不是再回头补基础语义，而是把能安全补掉的 attention 收掉，把不能硬猜的部分明确收窄成设计 attention。

## 2. 这轮实际做成了什么

1. 已把 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 里的 `PersistentManagers.prefabDatabase` 显式指向现成的 [PrefabDatabase.asset](/D:/Unity/Unity_learning/Sunset/Assets/111_Data/Database/PrefabDatabase.asset)。
2. 已把 [HomeSceneRestContractMenu.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/Home/HomeSceneRestContractMenu.cs) 的 probe 结果语义收紧为：
   - 只要没有 blocker，就视为 `success = true`
   - 如果仍有体验/设计层问题，就保留 `status = attention`
3. 已重新跑通：
   - `Tools/Sunset/Scene/Run Home Rest Contract Probe`
4. 已重新确认 fresh CLI：
   - `manage_script validate` = `clean`
   - `errors` = `0 error / 0 warning`

## 3. 当前 live 结果

最新 `home-rest-contract-probe.json` 现在返回：

- `status = attention`
- `success = true`
- `firstBlocker = ""`
- `message = Home 的住处 rest contract 已可用，但仍有 attention 需要后续决定。`

这代表：

- `Home` 现在已经是可消费、可验、可继续接 day1 屋内住处链的 scene-side 承接层
- 剩下的不是 blocker，而是 2 类明确保留项：
  - 镜头 framing / 首屏读感
  - 屋内门的正式出口 contract

## 4. 这轮被站住的新事实

1. `PersistentManagers.prefabDatabaseAssigned = true`
2. `Home` 当前不再因为 `PrefabDatabase` 缺显式引用而留下那条已知 warning attention
3. `HomeBed` 继续保持：
   - `Collider2D = true`
   - `isTrigger = true`
   - `tag = Interactable`
   - `SpringDay1BedInteractable = true`
4. `Home_Contracts / HomeDoor / HomeEntryAnchor / HomeBed` 层级仍然成立，且用户手摆门位没有被我改坏：
   - `Home_Contracts.position = (-18.03, -5.93, 0)`

## 5. 当前剩余 attention

当前只剩 4 条，而且都已经不属于“随手就该硬修”的 blocker：

1. `Home_Contracts` 不在主相机初始视野里
2. `HomeDoor` 不在主相机初始视野里
3. `HomeEntryAnchor` 不在主相机初始视野里
4. `HomeDoor` 还没有显式的 scene exit 组件

## 6. 当前判断

`Home` 这条线现在已经收到了“可用化完成”的阶段。

更准确地说：

- `住处 rest contract` 已站住
- `probe` 已能直接看日志判定
- `Home` 现在缺的不是基础设施，而是“屋内入口镜头要不要重构”和“最终出门到底回哪一个外部 contract”

## 7. 为什么我这轮停在这里

我没有继续硬补 `HomeDoor` 的 exit component，也没有强行把门位拉回镜头里，原因只有一个：

- 这两件事都已经不是“通用正确答案”，而是明确依赖屋内外承接设计的选择题
- 如果我现在替你猜，很容易做成“技术上有组件、体验上却是错出口”那种假收口

所以这轮最稳的收口方式是：

- 把 `Home` 先收成 `usable + probeable`
- 把剩余问题压缩成清楚、有限的 4 条 design attention

## 8. 对 spring-day1 的真实帮助

这轮之后，`day1` 不需要再把 `Home` 当成“屋内住处还没站住”的 blocker 了。

现在它可以直接把 `Home` 当成：

- 已有 `HomeBed`
- 已有 `HomeDoor/HomeEntryAnchor`
- 已有可重跑 probe
- 已有明确剩余项边界

的可承接屋内 scene-side。

## 9. 当前阶段

`Home` 当前阶段应改判为：

- `线程自测已过（usable with attention）`
- 不是 `fully ready`
- 也不是 `still blocked`

## 10. No-Red 证据卡 v2

- `cli_red_check_command`: `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/Editor/Home/HomeSceneRestContractMenu.cs --count 10 --output-limit 5`
- `cli_red_check_scope`: `Home contract usable-closeout`
- `cli_red_check_assessment`: `blocked`
- `unity_red_check`: `pass`
- `mcp_fallback`: `required`
- `mcp_fallback_reason`: `scene_live_flow_required`
- `current_owned_errors`: `0`
- `current_external_blockers`: `0`
- `current_warnings`: `0`

补充说明：

- 本轮 `validate_script` 仍然超时，说明 compile-first CLI assessment 这张卡没有完全闭环；
- 但更轻量的 `manage_script validate` 已返回 `clean`，fresh `errors` 也是 `0 error / 0 warning`；
- 再加上菜单 probe 已真实跑通，所以当前可以诚实表述为：
  - `代码层 clean`
  - `Unity live 已过`
  - `compile-first CLI assessment 本轮不稳`
