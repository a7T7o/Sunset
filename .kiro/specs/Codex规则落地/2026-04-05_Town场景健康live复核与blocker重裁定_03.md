# 2026-04-05_Town场景健康live复核与blocker重裁定_03

## A. 用户可读层

1. 当前主线  
当前唯一主线仍是：把 `Town` 收成可稳定承接后续生活面、可继续建设、可继续交给别的线程消费的基础设施基线。

2. 这轮实际做成了什么  
已把 `Town` 从“怀疑 scene 本体损坏”推进到“live 可直接打开、主故障已缩清”的阶段。  
已拿到 direct MCP 的编辑态 live 证据：`Town.unity` 可稳定加载，`Main Camera + AudioListener` 实际存在且启用。  
已把 `Primary -> Town` fresh console 真正压到只剩外线问题。  
已把 `Town` 的 blocker matrix 重新改写，不再让 `Town` 背 `scene 坏了`、`audio listener 缺了` 这类旧锅。

3. 现在还没做成什么  
`Town` 还不能判成 `sync-ready`。  
当前仍未闭环的，已经不再是 `Town.unity` 自身场景健康，而是外线 blocker：  
- `Town` 相机跟随 / frustum / active camera 运行态  
- `Town` 中文 DialogueUI / 字体链  
- `PlacementManager.cs` 编译红错对 Town live 验证的阻断

4. 当前阶段  
当前不是“Town 没修”，而是：  
- `Town` 自身 scene health 基线已站住  
- `Town` 的主 blocker 已重压回外线 owner  
- `Town` 进入“可继续消费前的最后外线阻断阶段”

5. 下一步只做什么  
下一步只做 3 件事：  
- 继续让 `工具-V1线程` 收 `Town` 相机跟随 / frustum 闭环  
- 继续让 `UI` 收 `Town` 中文 DialogueUI / 字体链  
- 把 `PlacementManager.cs` 的 compile red 正式压回 `农田交互修复V3`

6. 需要我现在做什么  
如果要继续分发，我这轮已经补出一份给 `农田交互修复V3` 的精确 prompt。  
如果不需要手动转发，这份重裁定也已经足够给后续 Town 总闸继续沿用。

## B. 新证据

### 1. 直接成立的 live 事实

1. `Town.unity` 在编辑态可直接作为 active scene 打开。  
2. `Town` 当前 `rootCount = 10`。  
3. `Main Camera` 是 root，且组件为：  
   - `Transform`
   - `Camera`
   - `AudioListener`
4. `AudioListener.enabled = true`。  
5. `UI / DialogueCanvas / EventSystem / CinemachineCamera` 都能直接读到有效组件明细。  
6. `Town` 编辑态里没有：  
   - `PersistentManagers`
   - `PersistentObjectRegistry`
   - `TimeManager`
   - `SeasonManager`
   - `WeatherSystem`

### 2. 由这些证据带来的裁定变化

1. `Town.unity` 当前不能再定性成“磁盘版 scene 自身损坏”。  
2. `There are no audio listeners in the scene` 当前不能再直接归到 `Town` scene wiring。  
3. 用户此前看到的 runtime manager duplicate / bootstrap warning，当前也不能再归到 `Town.unity` 自带配置。  
4. `Town` 自己这条线当前更像“scene 承载基线已站住，剩余 blocker 在 runtime 外线”。

### 3. fresh console 重新压实后的结果

在 `Edit Mode` 下按下面顺序取证：  
- clear console  
- load `Primary`  
- clear console  
- load `Town`

得到的结论是：

1. 初次 fresh console 只剩：  
   - `CloudShadowManager.cs:359` 的卸载态 `MissingReferenceException`
2. 对 [CloudShadowManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs) 补最小 destroyed-self guard 后，再做同样 fresh load：  
   - 这条异常不再复现
3. 目前 fresh console 剩余可稳定复现的红错，是：  
   - [PlacementManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Placement/PlacementManager.cs):1694 `CS0034`

## C. 更新后的 Town blocker matrix

| 编号 | 名称 | 当前定性 | owner | 主要文件 | 类型 | 当前判断 |
| --- | --- | --- | --- | --- | --- | --- |
| T-B1 | Town 相机跟随 / frustum / active camera | 真 blocker | `工具-V1线程` | `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs` | camera runtime | 用户 live 仍坐实“Town 不跟随”，Town 结构基线已在，但 runtime 相机链未闭环 |
| T-B2 | Town 中文 DialogueUI / 字体链 | 真 blocker | `UI` | `Assets/YYY_Scripts/Story/UI/DialogueUI.cs` 等 | UI / font runtime | 中文继续按钮与对话字体链仍未通过 live 闭环 |
| T-B3 | Town live 验证被 Placement 编译红阻断 | 真 blocker | `农田交互修复V3` | `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs` | compile blocker | 当前 fresh console 可稳定复现 `CS0034`，它会阻断 Town 后续 live 验证，不属于 Town own |
| T-N1 | Town scene 本体损坏 | 非 blocker | `Town` 自身 | `Assets/000_Scenes/Town.unity` | scene health | 现有 direct MCP 证据表明 Town 编辑态可稳定加载，不能再按坏 scene 处理 |
| T-N2 | Town 缺 AudioListener | 非 blocker | `Town` 自身 | `Assets/000_Scenes/Town.unity` | scene wiring | `Main Camera` 上的 `AudioListener` 已被 live 证明存在且启用 |
| T-N3 | PersistentManagers / Registry 是 Town scene 自带 duplicate 根因 | 非 blocker | `Town` 自身 | `Assets/YYY_Scripts/Service/PersistentManagers.cs` / `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs` | runtime bootstrap | Town 编辑态里压根没有这些根物体，当前不能继续把 duplicate warning 直接归 Town scene |
| T-N4 | CloudShadowManager 卸载态异常 | 侧向问题，当前已压低 | `云朵与光影` | `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs` | scene unload/runtime | 这条在 Town fresh load 中已被最小 guard 压掉，但 owner 仍不在 Town 线 |

## D. 新的阶段判断

### 1. Town 自身是否已站住

当前判断：**基本已站住，但还不能放行。**

原因是：

1. `Town.unity` 自身打开能力已成立。  
2. `Town` 的关键承载物仍在：  
   - `Town_Day1Carriers`
   - `Main Camera`
   - `_CameraBounds`
   - `UI`
   - `DialogueCanvas`
3. 当前剩余问题都更像“Town 进入后由外线 runtime 链继续消费时暴露的问题”。

### 2. Town 现在能否判成 sync-ready

不能。  

但不能的原因已经变化了：

- 不是 `Town` scene 本体还坏  
- 而是 `camera / UI / compile blocker` 仍未闭环

## E. 四类裁定（更新版）

### `继续发 prompt`

1. `工具-V1线程`  
   - 原因：Town 结构已在，剩余是纯 runtime 相机链问题。
2. `UI`  
   - 原因：Town 中文对话显示链仍未闭环。
3. `农田交互修复V3`  
   - 原因：当前 fresh console 的 `PlacementManager.cs` 编译红正在阻断 Town live 验证。

### `停给用户验收`

无。  
原因：Town 仍有真 blocker，用户终验时机还没到。

### `停给用户分析 / 审核`

无。  
原因：当前不是再做方案拍板，而是继续最小收口。

### `无需继续发`

1. `Codex规则落地` 自续工的 `Town scene health` 子线  
   - 原因：这条线的核心结论已经拿到，继续再删改 `Town.unity` 反而容易过度施工。
2. `spring-day1`
3. `NPC`
4. `导航检查`

## F. 技术审计层

1. 本轮新读取 / 使用的核心证据  
   - direct MCP `manage_scene load/get_hierarchy/get_active`
   - direct MCP `find_gameobjects`
   - direct MCP `read_console`
   - direct MCP `mcpforunity://scene/gameobject/{id}` / `.../components`
2. 本轮对 Town 的最关键 live 结果  
   - `Town` 可打开
   - `Main Camera + AudioListener` 存在
   - fresh console 已不再支持“Town 自身 scene 坏了”的旧结论
3. 本轮是否改了 `Town.unity`
   - 否
4. 本轮是否触碰了 Town 以外文件
   - 是，临时补了 [CloudShadowManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs) 的卸载态防守
5. 这是否改变 Town owner 边界
   - 否，Town 仍不 owns `CloudShadowManager`
6. 当前精确外线 blocker
   - `CameraDeadZoneSync / Cinemachine 跟随链`
   - `DialogueUI / 字体链`
   - `PlacementManager.cs` 编译红
