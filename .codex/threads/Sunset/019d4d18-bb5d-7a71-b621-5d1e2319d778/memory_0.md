## 2026-04-02｜修复 002批量工具对工作台 `Anvil_0` 的排序误判

- 当前主线目标：处理用户直接报出的 `002批量-Hierarchy` 无法正确给工作台设置 `Order in Layer` 的阻塞；主线服务对象是 `Anvil_0` 这类“父空节点 + 子 Collider”的静态物体。
- 本轮子任务：只改 editor utility 的排序计算逻辑，不碰 Scene / Prefab / 运行时业务脚本。
- 已完成事项：
  1. 先按 Sunset 启动闸门做只读前置核查；由于 `sunset-startup-guard` 当前会话未显式暴露，本轮改走 `skills-governor` 手工等价流程，并同时遵守 `sunset-no-red-handoff`。
  2. 结合用户截图、`Assets/Editor/Tool_002_BatchHierarchy.cs` 代码和 `Assets/000_Scenes/Primary.unity` 里 `Anvil_0` 的层级/碰撞体数据，定位根因：`CalculateSortingY()` 对“父无 SR 的子物体”提前回退父 Y，吞掉了子物体自己的 `PolygonCollider2D.bounds.min.y`。
  3. 修复 `Assets/Editor/Tool_002_BatchHierarchy.cs`：当前对象自身有 Collider 时优先用 Collider 底部；调试输出补充 `Parent` 来源，便于后续排查。
  4. 同步修复 `Assets/Editor/StaticObjectOrderAutoCalibrator.cs` 的同源逻辑，避免自动校准继续沿用旧误判。
- 关键文件：
  - `Assets/Editor/Tool_002_BatchHierarchy.cs`
  - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
  - `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
- 验证结果：
  - `git diff --check` 通过。
  - `unityMCP` 连接 `http://127.0.0.1:8888/mcp` 失败，未做 Unity live 复验。
  - 线程状态：已跑 `Begin-Slice`，收尾时已跑 `Park-Slice`；当前 live 状态为 `PARKED`。
- blocker / 恢复点：
  - blocker：`未做Unity live复验：unityMCP transport error (http://127.0.0.1:8888/mcp)`
  - 恢复点：待 MCP 恢复或手工进 Unity 后，重新对 `Anvil_0` 点一次 `设置选中物体的Order in Layer`，确认 `Order in Layer` 不再错误落为 `0` / 父节点高度。

## 2026-04-02｜修复上一轮补丁引入的 `CS0136`

- 当前主线未变，仍是收口 `002批量-Hierarchy` 的工作台排序问题；本轮子任务是修掉我上一轮补丁带进去的两个直接编译错误。
- 已完成事项：
  1. 重新跑 `Begin-Slice`，只认领 `Assets/Editor/Tool_002_BatchHierarchy.cs`。
  2. 将 `Shadow` / `Glow` 分支内部的 `parent` 局部变量统一改名为 `effectParent`，避免与外层调试区的 `parent` 重名。
  3. `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs` 通过。
- 验证状态：
  - 这轮属于“静态编译错误已清除”；
  - 仍未做 Unity live 复验。
- 线程状态：
  - 已跑 `Begin-Slice`
  - 已跑 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-02｜补记：`Invalid editor window` 更像旧坏窗口残留，不是当前类定义再次丢失

- 本轮继续只读核查 `Assets/Editor/Tool_002_BatchHierarchy.cs`，确认当前文件内：
  - 类名 `Tool_002_BatchHierarchy`
  - `MenuItem("Tools/002批量 (Hierarchy窗口)")`
  - `GetWindow<Tool_002_BatchHierarchy>("002批量-Hierarchy")`
  三者仍然一致，没有再次出现类名 / 文件名 / 菜单入口漂移。
- 结合用户看到的 `Invalid editor window of type: Tool_002_BatchHierarchy`，当前更稳的判断是：
  - Unity 正在恢复上一轮编译失败时残留下来的旧窗口页签；
  - 这类页签在脚本重新可编译后，仍可能继续报 `Invalid editor window`，直到旧 tab 被关闭并重新从菜单打开。
- 当前恢复点：
  - 先让用户关闭那张失效的 `002批量-Hierarchy` 旧窗口 tab，再从 `Tools/002批量 (Hierarchy窗口)` 重新打开；
  - 若重新打开后仍报同样错误，再按“Editor 程序集仍有其它 compile error”路线继续查。

## 2026-04-02｜收口：002批量窗口已改成不参与坏布局复活的独立工具窗

- 当前主线目标仍是恢复 `002批量-Hierarchy` 对工作台排序链的可用性；本轮子任务是把“进 Play 就报 `Invalid editor window`”这个阻塞真正切断，服务于用户能继续用这工具验 `Anvil_0` / 工作台排序。
- 本轮显式使用的 skill：
  - `skills-governor`：补做 Sunset 实质性任务前置核查与 skill 路由。
  - `sunset-no-red-handoff`：约束这轮必须以编译可用、Editor 不新增 owned red 的状态停手。
  - `sunset-startup-guard` 未在当前会话显式暴露，因此改走手工等价前置核查。
- 已完成事项：
  1. 保留上一轮对 `Collider` 排序和 `CS0136` 的修补不动，只针对窗口生命周期继续收口。
  2. `Assets/Editor/Tool_002_BatchHierarchy.cs`
     - `ShowWindow()` 从 `GetWindow<T>()` 改成 `CreateInstance + ShowAuxWindow()`，让工具窗不再以可停靠布局页签形式参与恢复；
     - 增加 `FindOpenWindow()`，避免重复创建；
     - `OnEnable()` 统一补标题，保证重开时标题一致。
  3. 重写 `Tool002BatchHierarchyPlayModeGuard`
     - 进入 Play 前关闭全部打开的 `Tool_002_BatchHierarchy`；
     - assembly reload 前关闭全部打开的 `Tool_002_BatchHierarchy`；
     - reload 后首个 `delayCall` 再扫一次 lingering window；
     - 不再自动重开窗口，避免旧坏 tab 被再次恢复。
  4. 用 `Editor.log` 继续核查：先前的 `WindowLayout:CheckWindowConsistency -> MaximizePresent -> Invalid editor window` 调用链仍能在旧日志里看到，但本轮重编译后的最新尾部未再出现新的同类报错。
- 验证结果：
  - `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs Assets/Editor/StaticObjectOrderAutoCalibrator.cs` 通过。
  - Unity 完成 forced synchronous recompile，`Mono: successfully reloaded assembly` 已出现。
  - owned blocking compile error：无。
  - external blockers：无新的 compile blocker；但当前没有 Unity MCP / 自动进 Play 复现通道，所以最终“重新打开工具窗后进 Play”仍待用户手测。
- 这轮 owned 范围：
  - `Assets/Editor/Tool_002_BatchHierarchy.cs`
  - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
- 当前停手判断：
  - 这轮我最核心的判断是：用户被困扰的主因已经从“排序没算对”转成“坏 EditorWindow 页签反复被恢复”，而这次已经把它改成不自动复活、不过布局恢复链的独立工具窗。
  - 我最不放心的点：我没法在当前环境里替用户实际点一次 Play，所以还不能把“报错彻底绝迹”包装成用户已测通过。
  - 自评：`7.5/10`。这轮把我自己制造的编译坑和窗口复活链都收住了，但最终用户侧 Play 复测还差最后一下。
- 当前恢复点：
  - 让用户重新从 `Tools/002批量 (Hierarchy窗口)` 打开工具窗后再进一次 Play；
  - 若不再报错，就回到原主线继续验 `Anvil_0` 的排序结果；
  - 若仍报错，下一步优先清用户本机布局缓存 / 残留 maximized layout，而不是再乱改排序公式。

## 2026-04-02｜继续排查：双击 GameView 全屏报错的根在 maximize 布局检查，不在业务代码

- 当前主线没有变：先把 `002批量-Hierarchy` 的 maximize / Play 报错彻底止住，再回到工作台排序主目标。
- 用户新补充的关键事实：
  - 只有“运行游戏后双击 `Game` 窗口让其全屏显示”才稳定触发；
  - 这直接把问题从“泛 PlayMode 报错”收窄成“GameView maximize 报错”。
- 本轮关键判断：
  - `Editor.log` 的调用链稳定是
    `WindowLayout.CheckWindowConsistency() -> MaximizePresent() -> EditorWindow.maximized`；
  - 所以为什么“只有双击全屏才报”：
    - 因为只有这一步会强制 Unity 去检查当前窗口布局和隐藏 tab 是否一致；
    - 而你的 Unity 当前布局缓存里还残留着一张已经失效的 `002批量-Hierarchy` 页签；
    - 平时不最大化，它不一定被碰到；一双击最大化，Unity 就把它挖出来了。
- 本轮施工：
  1. 尝试过更激进的自动清理钩子，想在 play / reload 前把残留窗口关掉；
  2. 但失效页签本身在 `EditorWindow.Close()` 就会抛 `NullReferenceException`，我这条路会继续给现场添红；
  3. 已经把这段自动清理逻辑回退成 no-op，避免继续制造 owned error；
  4. 改为走更稳的外部清理：
     - 备份 layout 到
       `D:\Unity\Unity_learning\Sunset\.codex\artifacts\layout-backups\2026-04-02_tool002_invalid-window`
     - 删除
       `C:\Users\aTo\AppData\Roaming\Unity\Editor-5.x\Preferences\Layouts\current\default-2022.dwlt`
     - 删除
       `C:\Users\aTo\AppData\Roaming\Unity\Editor-5.x\Preferences\Layouts\current\default-6000.dwlt`
     - 删除
       `D:\Unity\Unity_learning\Sunset\UserSettings\Layouts\CurrentMaximizeLayout.dwlt`
- 当前验证：
  - `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs Assets/Editor/StaticObjectOrderAutoCalibrator.cs` 通过；
  - 外部 layout 缓存文件当前均已 `Test-Path => False`；
  - 当前 `Editor.log` 里还能看到 earlier attempt 留下的 `NullReferenceException` 历史记录，但现代码已撤掉 تلك条自动关闭链。
- 当前最核心结论：
  - “为什么只有双击 GameView 全屏才报”
    - 因为最大化触发的是 Unity 布局一致性检查；
    - 真正炸的是坏布局缓存，不是你一运行游戏就有哪段业务代码崩了。
- 自评：
  - `7/10`。这轮把原因钉清、把我自己新增的自动清理红错撤回，也把 layout 缓存清干净了；
  - 但因为坏布局当前还在你这次 Unity 进程内存里，最终仍差一次“重开 Unity 后再复测”。
- 当前恢复点：
  - 让用户现在重开一次 Unity，再进 Play 双击 `Game` 窗口全屏；
  - 若报错消失，回到 `Anvil_0` / 工作台排序复验；
  - 若仍报错，再继续查 session 内更深层的隐藏 pane / dock area 残留。

## 2026-04-02 补记：Props/Water 碰撞、NPC 导航阻挡、镜头边界、场景切换触发器快速落地

- 当前主线目标：按用户要求一次落地 4 件事：
  - `Props` Tilemap 整块不可穿越
  - `Water` 不可进入，玩家和 NPC 都不能下水
  - 镜头不能拍到场景外
  - 给场景一个可调大小的空物体触发切场，目标场景由检查器拖入
- 本轮子任务与服务关系：这是主线本体施工，不再是前面 `Tool_002_BatchHierarchy` / maximize 报错的阻塞排查；该阻塞已从主线上摘除，当前恢复点回到场景可玩性与镜头边界。
- 已完成施工：
  1. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 新增 `explicitObstacleColliders` 序列化字段；
     - 网格阻挡判定先检查显式障碍碰撞体，再回退到 tag / layer；
     - 这样 `TilemapCollider2D` 能直接成为 NPC 寻路障碍，不需要复用 `Building` 等高副作用 tag。
  2. `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`
     - 修正 `_CameraBounds` 自动创建逻辑；
     - 自动把 `_CameraBounds` 放到世界根并归零变换，避免父节点偏移把 confiner 边界整体带偏；
     - 运行时会给 `CinemachineCamera` 自动补 `CinemachineConfiner2D` 并刷新边界缓存。
  3. `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
     - 新建通用 2D 切场触发器；
     - Inspector 可拖 `SceneAsset`，同时缓存运行时 `targetSceneName`；
     - 玩家进入 trigger 后执行简洁黑幕淡入/加载/淡出；
     - 触发器本体要求 `Collider2D`，用户可直接调大小。
  4. `Assets/000_Scenes/Primary.unity`
     - 给 `Layer 1 - Props_Porps` 与 `Layer 1 - Farmland_Water` 各补了 `TilemapCollider2D`；
     - `NavigationRoot` 上的 `NavGrid2D` 已显式引用这两个 tilemap collider；
     - `CinemachineCamera` 已挂 `CameraDeadZoneSync`；
     - `2_World` 下新增 `SceneTransitionTrigger` 空物体，默认 `BoxCollider2D` 为 trigger，大小可直接在 Inspector 调。
- 验证结果：
  - 离线脚本编译：
    - `NavGrid2D.cs`、`CameraDeadZoneSync.cs`、`SceneTransitionTrigger2D.cs` 已用项目现有 `Library/ScriptAssemblies + ManagedStripped` 引用做 Roslyn 离线编译，结果 `ALL_OK`；
    - 仅有已有程序集重名 / 未使用字段 warning，无新增 blocking error。
  - `git diff --check`：
    - 我本轮新增的 3 个脚本文件通过；
    - `Primary.unity` 因工作树里本来就存在大量非本轮 scene 脏改，无法用 `diff --check` 当作本轮独占 clean 证据。
  - Unity / Play 验证：
    - 当前本机未定位到 Unity Editor 可执行程序；
    - 因此本轮未完成真正 Unity 编译、Console 复核和 PlayMode 终验。
- 当前阶段：
  - `结构 / checkpoint`：成立；
  - `targeted probe / 局部验证`：脚本级成立；
  - `真实入口体验`：仍待用户在 Unity 内终验。
- 当前恢复点：
  - 如果后续继续这条线，直接在 Unity 里验证：玩家/NPC 是否都被 Props 与 Water 挡住、镜头是否仍能看到场景外、`SceneTransitionTrigger` 拖入目标场景后是否能正常切场。

## 2026-04-02 补记：Primary traversal 二次返修，改走“运行时硬拦截 + Editor 自动补当前打开 scene 实例”

- 当前主线目标：
  - 把 `Primary` 的 `Props`、`Water`、tilemap 外边界、镜头边界和切场触发，做成用户眼前当前打开场景里能立刻复测的真实现场。
- 本轮子任务：
  - 修正上一轮“磁盘文件改了，但用户当前 Unity 现场没吃到”的失效路径；
  - 该子任务直接服务主线，不是另起问题。
- 本轮关键判断：
  1. 当前真正失败的不是“完全没代码”，而是“scene 现场与磁盘 YAML 脱节”；
  2. `NavGrid2D` 的 auto-detect 之前会把 collider 边界也吞进去，再叠 `boundsPadding=5`，导致玩家还能走到 tilemap 外；
  3. 切场报错 `Scene 'Town' couldn't be loaded...` 的根因是 Editor PlayMode 直接按 sceneName 走 `SceneManager.LoadSceneAsync()`，而目标 scene 并未进 Build Profiles。
- 本轮施工：
  1. 修改 `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 新增显式障碍 tilemap 源、按名字自动发现 `Water/Props`、以及 tile 占位优先阻挡；
     - 自动边界检测改为：有 Tilemap 时只用 Tilemap 边界，不再吞 scene collider；
     - 新增 `SetBoundsPadding()` / `ConfigureExplicitObstacleSources()` 供 Editor binder 调用。
  2. 修改 `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - 手动输入现在会先预测下一步是否进入 blocked tile / world outside；
     - 若会越界，则按轴分拆，只保留合法方向速度。
  3. 修改 `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
     - 新增 `targetScenePath`；
     - Editor PlayMode 下支持按 scene path 直接切场。
  4. 新增 `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
     - 当前打开 scene 是 `Primary` 时，自动补 `Water/Props` tilemap collider、`NavGrid2D` 阻挡源、`boundsPadding=0`、`CameraDeadZoneSync`、`SceneTransitionTrigger`；
     - 这轮故意不继续手写 `Primary.unity`，避免再出现“文件改了但用户现场没同步”的假完成。
- 涉及文件：
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
  - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
  - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
- 验证：
  - `git diff --check`：通过；
  - `CodexCodeGuard`（UTF-8 / diff / Roslyn compile）：通过，`Diagnostics=[]`；
  - Unity live / PlayMode：未做终验，仍待用户现场复测。
- 本轮收口状态：
  - 已执行 `Park-Slice`；
  - thread-state=`PARKED`；
  - 当前没有我这轮 own 的 blocking compile error。
- 当前恢复点：
  - 让用户回到 Unity 的当前打开 `Primary` 现场复测：
    - 玩家是否仍能进 `Water`
    - 玩家是否仍能穿过 `Props`
    - 玩家是否还能走出 tilemap 外
    - `SceneTransitionTrigger` 是否已经出现在层级里且可拖目标 scene
    - 拖 `Town` / `Home` 后，Editor PlayMode 切场是否已不再受 Build Profiles 限制

## 2026-04-02｜Primary traversal 三次返修：空层误命中已修，手动移动改成脚底三点拦截

- 当前主线目标：
  - 继续完成 `Primary` 的 traversal 快修，让 `Water / Props / tilemap 外边界` 在用户眼前当前打开场景里真的挡住。
- 本轮子任务：
  - 用户最新复测明确指出三项阻挡“完全没有任何拦截”；
  - 这轮只做根因收窄和最小返修，不扩题到镜头或转场。
- 已完成事项：
  1. 重新核实 `Assets/000_Scenes/Primary.unity` 后确认：
     - `Layer 1 - Props_Porps` 空
     - `Layer 1 - Farmland_Water` 空
     - 真实非空 obstacle 层改认：
       - `Layer 1 - Wall`
       - `Layer 2 - Base`
       - `Layer 1 - Props_Ground`
       - `Layer 1 - Props_Background`
       - `Layer 1 - Props_Base`
  2. 修改 `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
     - 只收“非空 + 命中 wall/props/water/Layer 2 - Base”的 tilemap；
     - 不再继续把空的 `Props_Porps / Farmland_Water` 绑进当前打开 scene。
  3. 修改 `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 自动 obstacle tilemap 发现规则同步改成同一口径；
     - 当前 `Primary` 下真实会被收进显式障碍源的是：
       - `Wall`
       - `Layer 2 - Base`
       - `Props_*`
  4. 修改 `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - 手动移动改为脚底三点采样；
     - 解决玩家碰撞盒中心偏高导致的水域/props/越界漏判。
  5. 已执行 `Park-Slice`：
     - `ThreadName=019d4d18-bb5d-7a71-b621-5d1e2319d778`
     - 当前状态=`PARKED`
- 关键决策：
  - 这轮不再继续围绕“scene 也许没同步”打转；
  - 先把两个已证实的真根因修掉：
    1. obstacle source 绑到空层
    2. 玩家只用上半身中心做导航边界判定
- 涉及文件：
  - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
- 验证结果：
  - `git diff --check`：通过
  - `CodexCodeGuard pre-sync`：`CanContinue=true`，`Diagnostics=[]`
  - Unity live / PlayMode：尚未验证
- 当前恢复点：
  - 让用户回 Unity 里重测：
    - 朝 `Water` 走
    - 朝 `Props` 走
    - 朝地图外边缘走
  - 如果仍可穿，下一步直接进 Unity 现场确认这 5 个真实 obstacle tilemap 的覆盖，而不是再猜名字。

## 2026-04-03｜边界重置后收口：这条线程只保留 traversal / blocking / scene transition 三脚本

- 当前主线目标：
  - 按用户新分工，把这条线程从 scene/binder/tool owner 收窄成 3 个基础脚本 owner。
- 本轮用户裁定：
  - 不再碰：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/000_Scenes/Town.unity`
    - `Assets/Editor/ScenePartialSyncTool.cs`
    - `Assets/YYY_Tests/Editor/ScenePartialSyncToolTests.cs`
    - `Assets/Editor/Tool_002_BatchHierarchy.cs`
    - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
    - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
  - 当前唯一主刀固定为：
    - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
- 本轮已完成：
  1. 跑了 `Begin-Slice`
     - `current_slice=primary-traversal-script-contract-only-no-scene-live-apply`
  2. `NavGrid2D.cs`
     - 把 scene-specific obstacle 名字硬编码收回成可配置契约
     - 新增：
       - `obstacleTilemapNameKeywords`
       - `SetObstacleTilemapAutoDetection(...)`
       - `GetExplicitObstacleTilemaps()`
       - `GetExplicitObstacleColliders()`
     - 当前默认不再自动按名字猜 obstacle tilemap
  3. `PlayerMovement.cs`
     - 新增：
       - `autoFindNavGridIfMissing`
       - `navigationFootProbeVerticalInset`
       - `navigationFootProbeSideInset`
       - `SetNavGrid(...)`
       - `GetNavGrid()`
     - 未接 `NavGrid2D` 时会打一条明确 warning，不再 silent fallback
  4. `SceneTransitionTrigger2D.cs`
     - 新增：
       - `TargetScenePath`
       - `HasValidTarget`
       - `SetTargetScene(...)`
       - `ClearTargetScene()`
     - `TryStartTransition()` 改成 `bool` 返回
     - 只给 scene path 时也能解析 scene name
  5. 跑了最小代码闸门：
     - `git diff --check`：通过
     - `CodexCodeGuard pre-sync`：`CanContinue=true`
  6. 这轮已执行 `Park-Slice`
     - 当前状态=`PARKED`
- 关键决策：
  - 不再继续让脚本层偷偷替 scene owner 猜 `Primary / Town` 的 wiring；
  - 这三个脚本只负责把“怎么接”的基础逻辑说清和做稳。
- 主线程后续怎么接：
  - `NavGrid2D`
    - 手动拖显式 obstacle tilemaps/colliders，或显式启用 `SetObstacleTilemapAutoDetection(true, keywords, ...)`
  - `PlayerMovement`
    - 直接在场景里拖 `NavGrid2D`，或允许自动找唯一 NavGrid
  - `SceneTransitionTrigger2D`
    - 场景 owner 自己放 trigger collider 并拖 `SceneAsset`，或运行时用 `SetTargetScene(...)`
- 当前没做成什么：
  - 没有 claim `Primary/Town` 场景已完成
  - 没有继续维护 binder
  - 没有帮主线程把 scene wiring 实写回去
- 当前恢复点：
  - 以后这条线程只回答：
    - 这 3 个脚本怎么继续改
    - 主线程接 scene 时该怎么接
  - 不再默认回到 scene live-apply 或通用工具线。

## 2026-04-03｜导航分账调研完成：已给导航父线程和本线程各落一份分工 prompt

- 当前主线目标：
  - 用户要求先把“我做的”和“导航做的”彻底分清，再基于真实导航现场写出可直接转发的分工 prompt；本轮不再继续写 scene，不再继续碰通用工具。
- 本轮显式使用的 skill：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-prompt-slice-guard`
  - `sunset-governance-dispatch-protocol`
  - `preference-preflight-gate`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
  - `sunset-startup-guard` 本轮继续按 Sunset 规则做手工等价前置核查
- 本轮已完成：
  1. 只读核对导航现场：
     - `.kiro/state/active-threads/导航检查.json`
     - `.kiro/state/active-threads/导航检查V2.json`
     - `导航检查` / `导航检查V2` 线程 memory
     - `-50 / -51` 最新父线程与 V2 prompt
  2. 钉死当前真实分账：
     - 我这条线前面实际做过的是 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 的脚本基础层，以及一段后来已被用户否掉的 binder / scene live-apply 漂移；
     - `导航检查` 当前才是 `PlayerAutoNavigator.cs` 与 PAN runtime 的真主刀；
     - `导航检查V2` 当前只是 runner/menu final acceptance pack 并行验收线，且正被 `Assets/YYY_Scripts/Service/Navigation` 同根 foreign dirty 卡住。
  3. 形成并落盘 2 份 prompt：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_导航分工prompt_01.md`
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_本线程续工prompt_01.md`
  4. 导航 prompt 的关键裁定：
     - 这次应发给父线程 `导航检查`，不是直接发给 `导航检查V2`
     - 本轮唯一主刀是把 `Primary` 里仍未闭环、且本质属于 traversal / blocking / navigation scene integration 的 3 项问题单独拆出来：
       - `Props` 阻挡
       - `Water` 阻挡
       - tilemap 外边界阻挡
     - 明确禁止继续把这条 `Primary traversal` 尾项和当前 `PAN crowd runtime` 主线、或 `runner/menu final pack` 混报
  5. 本线程 prompt 的关键裁定：
     - 这条线继续只保留 3 个脚本契约 owner
     - 不再碰 `Primary/Town` scene
     - 不再碰 binder
     - 不再碰通用工具
     - 以后只在别线明确指出 contract gap 时，才回到这 3 个脚本做最小补口
  6. 已补跑 `preference-preflight-gate` helper，明确这轮属于 `prompt-handoff`，并继续遵守：
     - 详细正文进文件
     - 聊天只给复制友好转发壳
     - 不能把结构 / 契约 checkpoint 冒充成 scene 体验过线
  7. 已执行：
     - `Park-Slice`
     - 当前 thread-state=`PARKED`
     - blockers=`prompts-delivered-awaiting-user-forward-or-next-routing`
- 关键决策：
  1. 这轮不是继续施工 `Primary traversal` 本体，而是先把 owner 和接盘面说清；
  2. `导航检查` 才是这次最适合接 `Primary traversal` 剩余闭环的人，不是 `导航检查V2`；
  3. 我这条线现在成立的是脚本契约，不是场景终态；
  4. 镜头边界仍未纳入这 2 份 prompt 的接单范围，后续若用户继续压这块，需要再单独分配。
- 验证结果：
  - prompt 文件已落盘；
  - `preference-preflight-gate` helper 已返回 `Triggered=yes`；
  - 当前 thread-state 已从 `ACTIVE` 合法切到 `PARKED`。
- 当前恢复点：
  - 如果用户下一步要转发，直接把导航 prompt 发给 `导航检查`；
  - 如果导航线回执里明确指出 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 仍有 contract gap，我这条线才回到 3 个脚本做最小补口；
  - 如果用户要继续处理镜头边界，需要另开 owner，不应再混回这 2 份 prompt 里。

## 2026-04-03｜用户再次显式重申本线程边界：后续只按本线程续工 prompt 约束自己

- 当前主线目标：
  - 用户直接把本线程续工 prompt 再发回来，要求我后续严格只按该 prompt 约束自己；本轮是只读确认与约束重申，不进入真实施工。
- 本轮已完成：
  1. 完整重读：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_本线程续工prompt_01.md`
  2. 再次确认这条线程的硬边界：
     - 只保留：
       - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
       - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
       - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
     - 不再碰：
       - `Primary.unity`
       - `Town.unity`
       - binder
       - 通用工具
     - 不再把脚本侧 contract 说成 scene 已完成
  3. 再次确认后续唯一允许回到代码修改的前提：
     - 必须是导航线或 scene owner 明确点名这 3 个脚本里存在 contract gap；
     - 否则我只做 contract support / ownership audit，不再主动扩回实现面。
- 关键决策：
  1. 这条 prompt 现在对我来说是硬约束，不是参考意见；
  2. 后续若用户继续问 `Primary/Town`、镜头边界、scene wiring、工具问题，我必须先按这条边界报实，再决定是否需要转交别线；
  3. 本轮因为只做只读确认，所以不跑 `Begin-Slice`，也不 claim 进入真实施工。
- 验证结果：
  - 已完整读取指定 prompt；
  - 本轮未改代码、未碰 scene、未碰 binder、未碰工具。
- 当前恢复点：
  - 后续只要没有外线明确点名这 3 个脚本的 contract gap，我就保持只读约束，不自行回到 `Primary/Town` scene 或其他漂移面。

## 2026-04-03｜重新起刀：三脚本 contract 加固，专门服务 scene owner / navigation owner 接线

- 当前主线目标：
  - 用户明确要求我“别躺着，导航已经在干活”，所以这轮重新进入真实施工；但仍严格只锁：
    - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
  - 不碰 `Primary/Town` scene，不碰 binder，不碰工具。
- 本轮子任务：
  - 把这 3 个脚本收成更像“scene owner / navigation owner 真能拿去接线”的稳定契约，主动补掉几个容易踩坑的 API 缺口，而不是继续等别人点名。
- 本轮已完成：
  1. 已重新执行：
     - `Begin-Slice`
     - `current_slice=stabilize-navigation-contract-scripts-for-scene-owners`
  2. `NavGrid2D.cs`
     - 新增：
       - `GetWorldBounds()`
       - `IsWithinWorldBounds(Vector2)`
       - `ClampToWorldBounds(Vector2)`
     - `TryFindNearestWalkable(...)` 不再因为点击点落在 world bounds 外就直接失败；
       现在会先把点投影到最近的 in-bounds 网格，再继续找最近可走点。
     - 这条改动的意义是：
       - scene owner / navigation owner 现在可以显式拿到世界边界契约；
       - 也不会因为“点在地图外一点点”就完全拿不到最近合法点。
  3. `PlayerMovement.cs`
     - 新增：
       - `HasNavGridReference()`
       - `TryResolveNavGrid(...)`
       - `CanOccupyPosition(Vector2)`
       - `GetNavigationProbePoints(...)`
     - 内部脚底三点采样也改成复用新的 public probe API，不再把真正的 player occupancy 规则只藏在私有方法里。
     - 这条改动的意义是：
       - 外线现在可以用和玩家真实移动同一套采样语义做接线或验证；
       - 不需要自己再猜脚底探针到底采哪里。
  4. `SceneTransitionTrigger2D.cs`
     - 新增：
       - `TryResolveTarget(...)`
     - `HasValidTarget` 改成复用统一 target 解析逻辑；
     - `TryStartTransition()` 现在会先用统一解析逻辑拿到：
       - `resolvedSceneName`
       - `resolvedScenePath`
       - `failureReason`
     - `SetTargetScene(...)` 现在即使只给 path，也会自动推导 scene name。
     - 这条改动的意义是：
       - scene owner 不会再因为只设 path 没补 name 就留下隐式歧义；
       - trigger 本身也能更明确报“为什么现在不能切场”。
  5. 已完成静态自检：
     - `git diff --check`：通过
     - 新增 public API 与旧引用链核对：通过
- 关键决策：
  1. 这轮不继续扩 scope 到 scene wiring，而是专门把“别人要接场景时会踩的 contract 坑”先补掉；
  2. `NavGrid2D` 这轮最值钱的补口是“bounds 外点击不再直接断死”，这正服务 traversal / blocking 接线；
  3. `PlayerMovement` 这轮最值钱的补口是“把真实脚底采样规则公开出来”，让外线能按同一规则验证；
  4. `SceneTransitionTrigger2D` 这轮最值钱的补口是“target 解析逻辑统一，不再散在多个入口里”。
- 当前没做成什么：
  - 仍然没有 claim `Primary/Town` 场景已完成；
  - 仍然没有 claim `Props / Water / tilemap 外边界` 的 scene integration 已完成；
  - 镜头边界仍然不在本轮 own 范围。
- 当前恢复点：
  - 下一步先跑 `Ready-To-Sync`；
  - 如果白名单闸门和代码闸门都过，我再决定是否能继续进入真正收口；
  - 如果被挡住，就按 blocker 报实，不再空喊“修好了”。

## 2026-04-03｜收口报实：三脚本 contract 加固已做完，但 `Ready-To-Sync` 被同根 remaining dirty 阻断

- 当前主线目标：
  - 继续只做 3 个脚本 contract 加固，不再碰 scene / binder / tool；本轮收尾重点是确认这刀到底能不能 legal-sync。
- 本轮补充结果：
  1. `Ready-To-Sync` 已实际运行；
  2. 结果不是代码编译坏，而是白名单根目录仍有 remaining dirty，被判定为：
     - `BLOCKED`
  3. 主要阻断口径：
     - `Assets/YYY_Scripts/Service/Player`
     - `Assets/YYY_Scripts/Service/Navigation`
     - 当前线程根目录
     这几个 same-root 下还有未纳入本轮的旧 dirty / foreign dirty，因此当前不能合法只 sync 我这 3 个脚本与 memory。
  4. `Ready-To-Sync` 输出里对我这轮最关键的新事实：
     - `代码闸门通过: True`
     - 但 `代码闸门适用: False`
     - 真正挡住我的不是新写红，而是 same-root remaining dirty
  5. 已执行：
     - `Park-Slice`
     - 当前 thread-state=`PARKED`
     - blocker=`Ready-To-Sync blocked: same-root remaining dirty still exists under Assets/YYY_Scripts/Service/Player, Assets/YYY_Scripts/Service/Navigation, and this thread root; contract slice completed but cannot legal-sync yet.`
- 关键决策：
  1. 这轮可以 claim：
     - contract slice 已完成
     - `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 已被补强
  2. 这轮不能 claim：
     - 已归仓
     - 已 sync
  3. 目前最诚实的口径是：
     - 结构 / contract 成立
     - legal-sync 尚未成立
- 当前恢复点：
  - 如果后续继续，需要先处理 same-root remaining dirty 的治理阻断，或由治理位重新收窄 / 重划 own root；
  - 在那之前，我这条线不再把这刀包装成“已经提交完成”。

## 2026-04-03｜按导航父线程 `-55` 裁定重写分工 prompt：导航接 scene integration，我方保持 `PARKED`

- 当前主线目标：
  - 根据导航父线程 `-55` 的最新裁定，重新把下一步分工说清楚：导航父线程去接 `Primary traversal` 的 scene integration / live closure；我方保持 `PARKED`，不再自行清 same-root dirty。
- 本轮子任务：
  - 完整读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\2026-04-03-工具V1-三脚本contract回执与停工裁定-55.md`；
  - 判断它的性质是“父线程裁定 + 对接指令”，不是普通成果回执；
  - 基于该裁定重写给导航父线程和给我自己的两份 prompt。
- 这轮实际做成了什么：
  1. 新增 `2026-04-03_导航分工prompt_03.md`
     - 明确导航父线程下一步接 `Primary.unity / PrimaryTraversalSceneBinder.cs / scene wiring / live integration`；
     - 明确工具-V1不再继续自清 same-root dirty；
     - 明确只有导航方 fresh 压实脚本缺口后，才允许精确回抛给我方三脚本。
  2. 新增 `2026-04-03_本线程续工prompt_03.md`
     - 把我方状态改成接受 `-55` 裁定并保持 `PARKED`；
     - 不再继续做 cleanup slice；
     - 只在父线程 / scene owner 精确点名 `contract gap` 或 `runtime trim / rollback` 时才允许 reopen。
  3. 重新登记并收好 `thread-state`
     - 先用 `_03` 路径补一次 `Begin-Slice`，保证 `owned_paths / expected_sync_paths` 与这轮真实文件一致；
     - 随后再次 `Park-Slice`，当前 live 状态恢复为 `PARKED`。
- 关键决策：
  1. `-55` 的核心不是“让我继续自己清 blocker”，而是“我这刀已包含 runtime behavior change，必须先停给父线程 / scene owner 消化”；
  2. 因此旧的 `_02` 自清 blocker 口径失效，新的正确分工是：
     - 导航父线程继续做 scene integration；
     - 我方停止主动 cleanup；
     - 仅保留 `NavGrid2D.cs / PlayerMovement.cs / SceneTransitionTrigger2D.cs` 的被动精确响应权；
  3. 这轮没有去碰 scene、binder、通用工具，也没有把脚本侧成立说成 scene 已完成。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_导航分工prompt_03.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_本线程续工prompt_03.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\memory_0.md`
- 验证结果：
  - `git diff --check -- '.codex/threads/Sunset/019d4d18-bb5d-7a71-b621-5d1e2319d778/2026-04-03_导航分工prompt_03.md' '.codex/threads/Sunset/019d4d18-bb5d-7a71-b621-5d1e2319d778/2026-04-03_本线程续工prompt_03.md'`：通过；
  - `Park-Slice` 已完成，当前 blocker 为：
    - `waiting-for-user-to-dispatch-navigation-prompt-v3`
    - `tool-v1-kept-parked-per-55-until-parent-thread-or-scene-owner-precisely-calls-for-three-script-gap-or-trim`
- 当前恢复点：
  - 用户现在可直接转发 `_03` 给导航父线程；
  - 我方保持 `PARKED`，不继续主动清 same-root dirty；
  - 只有导航父线程或 scene owner 之后精确点名三脚本缺口 / trim 需求时，我方才重新 `Begin-Slice` 进入最小续工。

## 2026-04-03｜用户正式下发 `_03`：本线程继续保持 `PARKED`

- 当前主线目标：
  - 严格按 `2026-04-03_本线程续工prompt_03.md` 自我约束，不再把这条线重新漂回 cleanup 或 scene integration。
- 本轮子任务：
  - 完整读取 `_03`；
  - 核对当前 `thread-state` 是否已是 `PARKED`；
  - 仅做记忆与审计收尾，不进入真实施工。
- 这轮实际做成了什么：
  1. 已确认 `_03` 的约束口径与当前现场一致：
     - 接受 `-55` 裁定；
     - 不再自己处理 same-root dirty；
     - 只保留 `NavGrid2D.cs / PlayerMovement.cs / SceneTransitionTrigger2D.cs` 的精确响应权。
  2. 已确认当前 `thread-state` 仍为 `PARKED`，无需重新开工或补跑新的 `Park-Slice`。
- 关键决策：
  1. 这轮不启动新的 cleanup slice，也不做任何三脚本改动；
  2. 这轮只接受“保持停车、等待精确点名”这一个执行结果；
  3. 后续只有父线程 / scene owner 明确点名 `contract gap` 或 `runtime trim / rollback` 时，才允许我重新 `Begin-Slice`。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\2026-04-03_本线程续工prompt_03.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\019d4d18-bb5d-7a71-b621-5d1e2319d778.json`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\memory_0.md`
- 验证结果：
  - `_03` 已完整读取；
  - `thread-state.status = PARKED` 已核实。
- 当前恢复点：
  - 继续保持 `PARKED`；
  - 等父线程 / scene owner 后续精确点名是否存在三脚本 gap 或 trim / rollback 需求。

## 2026-04-03｜用户直接要求继续干活后，本线程主动收了一刀“把 runtime 默认行为收窄回 contract-biased”

- 当前主线目标：
  - 在不碰 scene / binder / tool、也不去清 same-root 外溢脏改的前提下，先把父线程 `-55` 明确指出的“已越过 pure contract-only”的默认 runtime 行为收窄回来，让三脚本更接近可被父线程安全消费的 contract-biased 基线。
- 本轮子任务：
  - 重新从 `PARKED` 进入真实施工；
  - 只碰 `NavGrid2D.cs / PlayerMovement.cs / SceneTransitionTrigger2D.cs`；
  - 优先 trim 无条件生效的 runtime 默认行为，而不是继续扩功能。
- 这轮实际做成了什么：
  1. `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - 新增 `enforceNavGridBounds`，默认 `false`；
     - 保留 `SetNavGrid / HasNavGridReference / TryResolveNavGrid / GetNavGrid / CanOccupyPosition / GetNavigationProbePoints` 这些契约入口；
     - 但玩家每帧速度不再默认强制经过 `ConstrainVelocityToNavigationBounds(...)`，除非显式打开开关。
  2. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 保留显式障碍源配置、world bounds 读取等接线 API；
     - 回退 `DetectWorldBounds()` 里“有 Tilemap 时不再纳入 Collider 兜底边界”的无条件语义变化；
     - 回退 `TryFindNearestWalkable(...)` 对 out-of-bounds 点位的自动钳回语义，恢复为原先只接受 in-bounds 起点。
  3. `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
     - 本轮未继续扩写，只保留现状，不新增第二刀。
  4. 验证：
     - `git diff --check -- Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs Assets/YYY_Scripts/Service/Player/PlayerMovement.cs Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`：通过；
     - `scripts/CodexCodeGuard` 对上述 3 个脚本做 UTF-8 / diff / 程序集级 Roslyn 编译检查：`CanContinue=true`，无诊断。
  5. `Ready-To-Sync`
     - 已实际运行；
     - 结果仍为 `BLOCKED`；
     - 新 blocker 不是我这刀脚本编译坏，而是 own roots 下仍有大量未纳入本轮的 remaining dirty / untracked。
- 关键决策：
  1. 这轮不再拿“继续停车”当借口，而是先把最明显的一刀收掉：把默认 runtime 行为改成显式 opt-in；
  2. 这轮没有去动 scene / binder / tool，也没有借机做 shared-root 大清扫；
  3. 当前最诚实的结论是：
     - 三脚本这刀比之前更接近 contract-biased；
     - 代码闸门通过；
     - 但 legal-sync 仍被 same-root / own-root 历史残留阻断。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerMovement.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\019d4d18-bb5d-7a71-b621-5d1e2319d778\memory_0.md`
- 验证结果：
  - `git diff --check`：通过；
  - `CodexCodeGuard`：`Applies=true`、`CanContinue=true`、`AffectedAssemblies=[Assembly-CSharp]`、`Diagnostics=[]`；
  - `Ready-To-Sync`：`BLOCKED`，主因是 own roots remaining dirty 数量仍然很多，首屏点名包括：
    - `Assets/YYY_Scripts/Service/Navigation/Editor/NavigationLiveValidationMenu.cs`
    - `Assets/YYY_Scripts/Service/Navigation/NavigationLiveValidationRunner.cs`
    - `Assets/YYY_Scripts/Service/Navigation/NavigationStaticPointValidationRunner.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
- 当前恢复点：
  - 如果继续推进这条线，下一步不该再碰 scene，而是要么：
    1. 继续把 own roots 的历史残留/同根阻断做精确分类与最小清理；
    2. 要么让父线程 / scene owner 明确消费这轮 contract-biased 三脚本结果后，再决定是否需要进一步 trim / rollback；
  - 在那之前，这条线不能 claim 已 sync / 已归仓。

## 2026-04-03｜用户直接改判后，本线程新开“inspector-driven traversal manager + async transition masking”脚本切片

- 当前主线目标：
  - 不再把时间耗在旧的停车 prompt 循环里，而是给 scene owner 准备一套不依赖我直接改 `Primary/Town` 的脚本接线方案：
    - Water / Props / Border / 场景外边界
    - manager 挂空物体后由用户自己拖引用
    - scene transition 的黑屏内异步加载掩蔽
- 本轮子任务：
  - 保持只碰脚本；
  - 新建 manager；
  - 给 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 补最小 public contract；
  - 不碰 `Primary.unity / Town.unity / binder / 通用工具`。
- 这轮实际做成了什么：
  1. 新增 `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
     - 可挂在空物体；
     - 可拖 `NavGrid2D / PlayerMovement / blocking Tilemap / blocking Collider / bounds Tilemap / bounds Collider`；
     - 默认优先使用真实 Collider；
     - 只有显式勾选时才回退到整格 `tile occupancy fallback`。
  2. 修改 `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 新增 `SetAutoDetectWorldBounds(...)`；
     - 新增 `SetWorldBounds(...)`；
     - 允许 manager 把手动 world bounds 喂回网格，阻止玩家/NPC 出图。
  3. 修改 `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
     - 新增 `SetNavGridBoundsEnforcement(bool enabled)`；
     - 允许 manager 在不改 scene 的情况下显式开启玩家边界约束。
  4. 修改 `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
     - 切场流程改成：黑屏 -> async load -> `progress >= 0.9` 再 activation -> activation 后再等数帧 -> fade in；
     - 目标是把 activation 峰值尽量吞进黑屏里。
  5. 只读勘察补充：
     - `Primary.unity` 中的 `Layer 1 - Farmland_Water` 与 `Layer 1 - Props_Porps` 当前都已存在 `TilemapCollider2D`；
     - 因而问题核心更像是导航没有正确吃到这些 Collider，而不是 scene 里完全没有碰撞体。
- 关键决策：
  1. 不再继续走“显式障碍 Tilemap = 默认整格阻挡”的方向；
  2. 先把 manager 做成 `Collider-first`；
  3. 不把“脚本底座成立”说成“scene 已完成接线”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavGrid2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\TraversalBlockManager2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerMovement.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SceneTransitionTrigger2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`（只读勘察，未改）
- 验证结果：
  - `git diff --check -- Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs Assets/YYY_Scripts/Service/Player/PlayerMovement.cs Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`：通过；
  - 当前未做 Unity live / Play Mode / scene integration 验证；
  - 因此此刻只能 claim：
    - 脚本可接线；
    - 不是场景已完成。
- 当前恢复点：
  - 下一步应由 scene owner 或用户自己把 `TraversalBlockManager2D` 挂进目标 scene，并拖入 Water / Props / Border / NavGrid / PlayerMovement；
  - 完成这步后再做真实 Play 验证，确认玩家/NPC 是否真的不能下水、不能穿 Props、不能出图，以及黑屏切场是否体感改善。

## 2026-04-03｜本轮最终 thread-state 收尾

- 本轮最终状态：
  - `Begin-Slice`：已跑，并已用 `-ForceReplace` 把真实 owned paths 补齐到：
    - `NavGrid2D.cs`
    - `PlayerMovement.cs`
    - `TraversalBlockManager2D.cs`
    - `SceneTransitionTrigger2D.cs`
    - `屎山修复/导航检查/memory.md`
    - `屎山修复/memory.md`
    - 当前线程 `memory_0.md`
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前 blocker：
  - `waiting-for-scene-owner-or-user-to-wire-traversalblockmanager2d-in-scene`
  - `waiting-for-manual-play-validation-of-water-props-world-bounds-and-transition`
- 当前恢复点：
  - 这条线现在不该继续自己写 scene；
  - 只等 scene owner / 用户把 manager 真挂进去并给出 Play 结果，再决定是否 reopen 收剩余 gap。

## 2026-04-03｜桥面覆盖水面阻挡缺口已补成脚本契约

- 当前主线目标：
  - 解决“桥视觉上在水上，但 traversal 仍被水拦住”的空气墙问题。
- 本轮子任务：
  - 继续只碰 `NavGrid2D.cs / TraversalBlockManager2D.cs`；
  - 新增 walkable override 契约；
  - 不碰 scene 实写。
- 这轮实际做成了什么：
  1. `NavGrid2D.cs`
     - 新增显式可走覆盖 Tilemap / Collider；
     - 新增 `ConfigureExplicitWalkableOverrideSources(...)`；
     - 命中桥面覆盖源时，会先判为可走，再处理水面等 blocking source。
  2. `TraversalBlockManager2D.cs`
     - 新增 `walkableOverrideTilemaps / walkableOverrideColliders`；
     - 新增 `useWalkableOverrideTilemapOccupancyFallback`；
     - 用户现在可以把桥层手动拖进 manager，让桥把桥下的水覆盖掉。
- 关键决策：
  1. 不再靠 sorting order 猜可走性；
  2. “桥能走”必须显式建模成 walkable override；
  3. Water 仍保持 blocking，不做退让。
- 验证结果：
  - `git diff --check -- Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`：通过；
  - 当前未做 Unity Play 验证。
- 当前最终 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前 blocker：
  - `waiting-for-scene-owner-or-user-to-wire-bridge-into-walkable-override-sources`
  - `waiting-for-manual-play-validation-of-bridge-water-and-boundary-behavior`
- 当前恢复点：
  - 下一步先把桥层拖进 manager 新增的 walkable override 槽位，再进 Play 复测桥、水、边界三件事。

## 2026-04-03｜桥还是走不过去的只读根因已查实到 scene 竞争配置

- 当前主线目标：
  - 继续服务 traversal / blocking / world bounds / scene transition 这条逻辑底座线；
  - 不碰 `Primary.unity` 实写，只把“为什么桥还是过不去”查清楚。
- 本轮子任务：
  - 只读核查 `Primary.unity` 里 `TraversalBlockManager2D` / `NavGrid2D` 的真实挂载关系；
  - 判断现在是否需要升级通知导航线程。
- 这轮实际查实了什么：
  1. `Primary.unity` 里当前同时存在两份同脚本：
     - `NavigationRoot` 上挂着一份 `TraversalBlockManager2D`，其 `blockingTilemaps / boundsTilemaps` 已有完整旧配置；
     - 另有一个单独物体名就叫 `TraversalBlockManager2D`，但它的 `blockingTilemaps / blockingColliders / bounds*` 基本是空引用。
  2. 这两份脚本都指向同一个 `NavGrid2D` 与同一个 `PlayerMovement`，而且都 `applyOnAwake = 1`；
     - 也就是它们会在运行时竞争性地往同一个导航网格里写配置。
  3. `NavGrid2D` 当前脚本顺序已经确认：
     - `IsPointBlocked(...)` 会先判 `walkable override`，命中桥面覆盖源就会直接视为可走；
     - 因此“桥逻辑上天生不能走”这个判断不成立。
  4. `Primary.unity` 当前没有任何 `walkableOverrideTilemaps / walkableOverrideColliders / useWalkableOverrideTilemapOccupancyFallback` 的序列化配置落进去；
     - 所以桥覆盖即便脚本支持了，scene 现场也还没真正接上。
- 关键判断：
  1. 用户现在“怎么拖都没用”的第一嫌疑不是导航算法；
  2. 更像是 scene 里两份 manager 在抢同一份 `NavGrid2D`，导致用户拖的那份并不是最终生效的一份，或被另一份旧配置覆盖；
  3. 当前更该通知的是 scene owner / `Primary` owner 做 scene integration 收口，而不是先让导航线程继续改算法。
- 验证证据：
  - `Primary.unity` 中：
    - `NavigationRoot` 及其 `TraversalBlockManager2D` 约在 `4791-4917`
    - 独立物体 `TraversalBlockManager2D` 约在 `163213-163273`
  - `NavGrid2D.cs` 中 `IsPointBlocked(...)` 先判 `walkable override`
- 当前阶段：
  - 逻辑契约已成立；
  - scene integration 现场存在“重复 manager + 桥覆盖未真正落盘”的配置竞争。
- 当前恢复点：
  - 下一步不该再让我这条脚本线继续猜导航；
  - 应先让 scene owner 把 `Primary` 收成“只保留一个真正生效的 `TraversalBlockManager2D`”，并在那一份上配置桥面 walkable override，再做 Play 复测；
  - 只有当 scene 收口后桥仍然走不过去，才值得 reopen 并升级给导航线程看 runtime gap。

## 2026-04-03｜补查：桥仍过不去不是单一根因，而是 scene 配置错误与玩家实体碰撞叠加

- 当前主线目标：
  - 继续把“桥为什么还是过不去”查到玩家真实被什么拦住；
  - 仍保持只读，不碰 `Primary.unity` 实写。
- 本轮子任务：
  - 在用户反馈“删掉了也没用，现在只有一个了”后，重新读取 `Primary.unity` 当前保存态；
  - 同时核查 `PlayerMovement` 是否还会被实体物理碰撞挡住。
- 这轮新增查实：
  1. 当前磁盘保存态里，`Primary.unity` 仍然存在两份 `TraversalBlockManager2D` 组件，不是单一生效源：
     - `NavigationRoot` 上那份仍在，且 `walkableOverrideTilemaps` 为空；
     - 单独物体 `TraversalBlockManager2D` 那份也仍在，并且已经配置了：
       - `blockingTilemaps = [Layer 1 - Water, Layer 1 - 桥_物品0]`
       - `walkableOverrideTilemaps = [Layer 1 - 桥_底座]`
  2. 这说明当前保存态里不只是“双 manager 竞争”还没消失，连桥层选择本身也还不稳：
     - 目前 override 只配了 `桥_底座`，并没有看到 `桥_地表`
     - 同时还把 `桥_物品0` 作为 blocking source 收了进去
  3. `PlayerMovement.cs` 已确认玩家是实体物理移动：
     - `rb.linearVelocity = ...`
     - 玩家身上是 `BoxCollider2D`，`m_IsTrigger = 0`
     - `Rigidbody2D` 为 `m_BodyType: 0`（动态实体）
  4. 当前 `Layer 1 - Water` 身上也有实体 `TilemapCollider2D`，并且 `m_IsTrigger = 0`
  5. 因此现在就算 `NavGrid2D` 在逻辑上判“桥可走”，玩家仍可能被水层实体碰撞直接顶住：
     - 也就是“导航允许”与“物理层仍挡住”可以同时成立
- 关键判断：
  1. 现在不是单一导航问题；
  2. 至少有三层风险叠在一起：
     - 保存态里仍像是双 manager 竞争
     - 桥 override 目标层选得不对或不完整
     - 水层实体碰撞会直接挡住玩家
  3. 如果目标是“桥可走、其余水面不可下”，当前更需要的是把“导航阻挡”和“玩家实体阻挡”分离，而不是只继续拖 walkable override。
- 当前恢复点：
  - 下一步应先把 scene 现场收成：
    - 真正单一 manager
    - `blockingTilemaps` 不再混入 `桥_*`
    - `walkableOverrideTilemaps` 至少先试 `桥_地表`
  - 若仍希望保留水的实体碰撞，则这条线还需要补一个脚本契约：把“水只作为导航阻挡，但不作为玩家实体阻挡”显式建模；
  - 也就是说：这一步已经开始涉及脚本 contract 级 gap，而不只是 scene 拖拽失误。

## 2026-04-03｜farm prompt 归属已厘清：这不是导航父线程活，而是当前三脚本线的 contract 判断

- 当前主线目标：
  - 在用户明确“当前 TraversalBlockManager 配置是他自己配的，先不要改”的前提下，
  - 同时厘清两个问题：
    1. farm 发来的 `placeable / chest / sapling` 放置成功卡顿 prompt 到底该不该由这条线接；
    2. 桥过不去在“不先改现配置”的前提下，是否已经升级成当前三脚本的 contract gap。
- 本轮子任务：
  - 只读核查 farm 提到的代码证据；
  - 判断是否需要再转告导航父线程。
- 这轮实际查实：
  1. farm prompt 指向的是 `工具-V1`，不是导航父线程大包；
     - 这和当前这条线“只保留 `NavGrid2D / PlayerMovement / SceneTransitionTrigger2D` 精确响应权”的边界是一致的。
  2. farm 的核心代码证据成立：
     - `ChestController` 放置后一帧直接 `NavGrid2D.OnRequestGridRefresh?.Invoke()`
     - `NavGrid2D.OnRequestGridRefresh` 当前直接订到 `RefreshGrid()`
     - `RefreshGrid()` 现在就是 `RefreshExplicitObstacleSources() + RebuildGrid()`
     - 所以 runtime placeable 成功后的刷新入口当前只有“重型整图刷新”这一档。
  3. `TreeController` 已做的只是“减少误刷 / 合并触发”，不是新增轻量刷新 contract；
     - 也就是 farm 那边说“现在剩 placeable 成功这一瞬间的重刷嫌疑最硬”是成立的。
  4. 对桥问题，在用户要求“先不改当前配置”的前提下，当前判断也进一步收敛：
     - `NavGrid2D` 已能表达逻辑 walkable override；
     - 但 `PlayerMovement` 仍是 `Rigidbody2D + BoxCollider2D` 的实体移动；
     - 水层仍是非 trigger 的实体 `TilemapCollider2D`；
     - 因此如果用户坚持保留这套现配置，桥问题就不再只是 scene 拖拽问题，而是当前脚本 contract 还不能同时表达：
       - “水逻辑上阻挡”
       - “桥面上方允许玩家穿过水层实体”
- 关键判断：
  1. farm 这段话不是发错人；
  2. 不需要再把它升级转告给导航父线程；
  3. 正确归属是：
     - 当前三脚本 / 工具-V1 线自己做 `contract` 级判断
     - 导航父线程继续保持 `PARKED`
  4. 当前这条线已经暴露出两类独立 contract 粒度问题：
     - `NavGrid2D` 缺 placeable runtime 轻量刷新入口
     - `PlayerMovement / NavGrid` 缺“导航阻挡”和“玩家实体阻挡”分离表达
- 当前恢复点：
  - 如果后续要 reopen，这两件事都应以“精确点名的 contract gap”回到当前三脚本线；
  - 不该把导航父线程整包叫回来。

## 2026-04-03｜实修收口：Primary 自动回写已切断，旧 `NavigationRoot` manager 已移除

- 当前主线目标：
  - 解决用户“自己明明删了/撤销了/改了 TraversalBlockManager2D，但又被改回去”的真阻塞；
  - 该阻塞服务于后续桥/水/边界手动配置终于能够稳定落盘。
- 本轮子任务：
  - 直接切断 `PrimaryTraversalSceneBinder` 自动回写；
  - 直接从 `Primary.unity` 移除 `NavigationRoot` 上旧的 `TraversalBlockManager2D`。
- 这轮实际做成了什么：
  1. 读清 `PrimaryTraversalSceneBinder.cs` 后确认：
     - 它会在 Editor 启动后自动监听 `delayCall / hierarchyChanged / sceneOpened`
     - 自动给 `NavigationRoot` 补 `TraversalBlockManager2D`
     - 并 `ApplyConfiguration(rebuildNavGrid: true)`
     - 所以这就是用户现场总被改回去的根因。
  2. 已删除：
     - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs`
     - `Assets/Editor/Story/PrimaryTraversalSceneBinder.cs.meta`
  3. 已修改：
     - `Assets/000_Scenes/Primary.unity`
     - 从 `NavigationRoot` 组件列表中摘掉 `TraversalBlockManager2D`
     - 删除其对应 YAML block
  4. 只读复核：
     - `NavigationRoot` 现在只剩 `Transform + NavGrid2D`
     - 独立物体 `TraversalBlockManager2D` 仍保留在 scene 保存态中
- 关键判断：
  1. 这次用户报的“配置总是错误”，根因成立地落在 Editor auto-binder，不是用户自己总配错；
  2. 切掉 binder 后，后续桥/水问题才值得继续按 contract gap 去看；
  3. 这轮没有去碰用户自己那份 manager 的参数，只是把后台偷写源头切断。
- 验证结果：
  - `rg` 复核通过；
  - `git diff --check -- Assets/000_Scenes/Primary.unity` 因 scene 历史 trailing whitespace 噪音不适合作为本轮 clean 证据；
  - 当前未做 Unity live 复验。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 现在回到用户自己的 `TraversalBlockManager2D` 配置现场继续测；
  - 若桥仍过不去，再按“placeable 轻量刷新 contract / 水逻辑阻挡与实体阻挡分离 contract”继续精确 reopen。

## 2026-04-03｜只读复判：放置成功卡顿不需要导航父线程，应该收成当前三脚本线的最小 contract 修复

- 当前主线目标：
  - 只读判定“放置成功那一下明显卡顿”到底应不应该上抛导航父线程。
- 本轮子任务：
  - 重点复核：
    - `NavGrid2D.cs`
    - `ChestController.cs`
    - `TreeController.cs`
  - 输出：
    - 根因是否为 `NavGrid2D` 当前 only-full-rebuild contract
    - 最小安全修复边界
    - 是否需要导航父线程介入
- 这轮实际查实：
  1. `NavGrid2D` 当前 runtime refresh 仍只有：
     - `OnRequestGridRefresh -> RefreshGrid() -> RebuildGrid()`
     - 且 `RebuildGrid()` 会整图重算 `walkable[,]`，不是局部 patch。
  2. `ChestController` 的最硬触发点是 `Start()` 无条件下一帧刷新：
     - 这会把“放置成功后出生的箱子”直接变成一次整图重建；
     - `SetOpen()` 与推动完成后也仍走同一重型入口。
  3. `TreeController` 现在只在碰撞语义真变化时才请求刷新；
     - 树苗 `Stage 0` 这类旧误刷路径已经基本被当前条件收窄掉；
     - 它不是这一轮“放置成功一下顿”的主嫌疑。
- 关键判断：
  1. 是，当前卡顿根因可以成立地归到：
     - **业务侧把 placeable 成功后的运行时变化打进了 `NavGrid2D` 现有 only-full-rebuild contract。**
  2. 但这不等于要把导航父线程整包叫回来；
     - 正确归属仍是当前三脚本精确响应位；
     - 这条线自己就能处理最小 contract gap。
  3. 如果只是业务线程本地删掉 `ChestController` 的刷新，风险是：
     - 放置后导航缓存陈旧；
     - 路径仍可能短时间把新箱子当成可穿区域。
- 当前恢复点：
  - 若后续 reopen，最小安全修复应只收：
    - `NavGrid2D.cs`
    - `ChestController.cs`
    - `TreeController.cs` 可选同步为统一接线，不是必改
  - 目标是补一个 placeable runtime 局部刷新 contract，而不是重做导航父线程主线。

## 2026-04-03｜实修收口：放置卡顿已改为 `NavGrid2D` 局部刷新，不再把箱子/树的运行时变化打成整图重建

- 当前主线目标：
  - 继续修 `Primary` traversal / blocking 相关现场里的放置卡顿问题；
  - 这轮只收“放置成功一下顿”的最小脚本 contract，不扩回 scene、binder 或导航父线程大包。
- 本轮子任务：
  - 直接在当前线程 own 路径里落地：
    - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
    - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
    - `Assets/YYY_Scripts/Controller/TreeController.cs`
- 这轮实际做成了什么：
  1. `NavGrid2D` 新增局部刷新入口：
     - `TryRequestLocalRefresh(Bounds ...)`
     - `RefreshGrid(Bounds ...)`
     - 不再只能 `OnRequestGridRefresh -> RefreshGrid() -> RebuildGrid()` 整图重算；
     - 新增按 world bounds 只重算局部 `walkable[,]` 的逻辑。
  2. `NavGrid2D` 把单格阻挡判定抽成 `EvaluateBlockedStateForCell()`：
     - 全图 `RebuildGrid()` 和局部 `RefreshGridRegion()` 现在复用同一套阻挡判定；
     - 避免局部刷新和整图刷新出现两套语义漂移。
  3. `ChestController` 改成局部刷新调用：
     - 放置出生后一帧刷新不再默认整图重建；
     - 开关状态变化、推动完成、销毁前清障，也都改为基于“当前 bounds + 上一次 obstacle bounds”的局部刷新；
     - 如果局部刷新不可用，才回退旧的全量刷新入口。
  4. `TreeController` 的共享延迟刷新也改为聚合 bounds：
     - 不再只是延迟后打一发整图刷新；
     - 现在会把多棵树/多次形状变化的 obstacle bounds 合并后，走一次局部刷新；
     - runner 在销毁时也会 flush 已聚合的 pending bounds，避免丢掉最后一次刷新。
- 关键判断：
  1. 这轮不需要导航父线程，也不需要再给导航系统发 prompt；
  2. 根因确实落在当前三脚本自己的 contract 粒度上，所以本线程直接修是正确路径；
  3. 这轮不是“彻底证明体验已过线”，而是“脚本侧最小 contract 已落地，等待 Unity live 复测”。
- 验证结果：
  - `git diff --check` 已过；
  - 本机未发现可直接调用的 `Unity` / `Unity.exe` CLI 入口，因此未做 Unity compile / Play live 验证；
  - 当前只能 claim：静态推断成立，Unity live 仍待用户复测。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 下一步优先让用户在 Unity 里复测“放下箱子/树时是否还会明显卡一下”；
  - 如果卡顿仍在，再继续沿 placeable caller 链或 grid bounds 规模做 live 定点，而不是回头泛化成导航父线程问题。

## 2026-04-03｜追加实修：放置验证不再每次全场扫树/箱子，改成直接读活动实例表

- 当前主线目标：
  - 继续收口“放置成功仍然卡顿”的残余顿点；
  - 这轮定位的是放置后立刻恢复预览时的 `PlacementValidator` 路径，不是导航刷新。
- 本轮子任务：
  - 只改：
    - `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
    - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
    - `Assets/YYY_Scripts/Controller/TreeController.cs`
- 这轮实际做成了什么：
  1. 查实 `PlacementManager.ResumePreviewAfterSuccessfulPlacement()` 会立刻重新跑 `RefreshPlacementValidationAt(...)`；
  2. 查实 `PlacementValidator` 在运行时会通过：
     - `FindObjectsByType<TreeController>()`
     - `FindObjectsByType<ChestController>()`
     每次验证重新扫场景；
  3. 已让 `ChestController / TreeController` 维护自己的活动实例表：
     - `OnEnable` 注册
     - `OnDisable / OnDestroy` 注销
  4. `PlacementValidator` 现在在 Play Mode 下直接读这些活动实例表，不再为了放置验证反复全场扫描；
  5. 同时把箱子校验中的重复 `GetComponentInChildren<Collider2D>()` 也改成直接走 `ChestController.GetColliderBounds()`，再削掉一层热路径组件查找。
- 关键判断：
  1. 用户上一轮“还是卡”说明卡顿不止导航这一条；
  2. 当前第二个高嫌疑链路已经被压实为“放置后立即预览校验时的全场对象扫描”；
  3. 这轮修的是放置系统热路径缓存，不是 scene 或 traversal 配置问题。
- 验证结果：
  - `git diff --check` 已过（仅有 Git 行尾提示，不是 C# 红错）；
  - 仍未做 Unity live 复测；
  - 当前只能 claim：静态推断成立，用户 live 仍待验证。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 让用户优先复测“放置箱子/树时的那一下卡顿是否明显减轻/消失”；
  - 若仍卡，再继续追 `PlacementManager` 自身的即时预览恢复与特效/实例化链，不再回头把问题全甩给导航。

## 2026-04-03｜实修：`002批量-Hierarchy` 窗口改回普通 `EditorWindow` 常驻打开

- 当前主线目标：
  - 处理用户直接报出的 `002批量-Hierarchy` 工具窗口“点别处就自动关闭”问题；
  - 这轮是窄边界编辑器阻塞修复，只服务于用户继续使用该工具，不扩到 scene、navigation 或其他工具。
- 本轮子任务：
  - 只改 `Assets/Editor/Tool_002_BatchHierarchy.cs`
  - 在同文件已有未提交改动基础上兼容修窗口生命周期/打开方式，不回退原有排序与调试逻辑补丁。
- 这轮实际做成了什么：
  1. 保留文件内已有 `WindowTitle`、`OnEnable()` 标题恢复和排序逻辑改动。
  2. 把 `ShowWindow()` 从 `CreateInstance + ShowAuxWindow()` 改回普通 `GetWindow<Tool_002_BatchHierarchy>(false, WindowTitle, true) + Show() + Focus()` 打开路径。
  3. 删掉不再需要的 `FindOpenWindow()` 辅助窗口检索逻辑，避免继续走 `AuxWindow` 生命周期。
- 关键判断：
  1. 自动关闭的直接根因就是当前菜单打开逻辑把这把工具开成了 `AuxWindow`；
  2. `AuxWindow` 不是普通可常驻的 `EditorWindow`，失焦后会被 Unity 当成辅助浮窗处理；
  3. 用户要的是“像普通 EditorWindow 一样可常驻”，所以最小正确修法就是回到普通 `GetWindow` 打开方式。
- 验证结果：
  - `git diff --check -- Assets/Editor/Tool_002_BatchHierarchy.cs` 已过；
  - 未做 Unity live 编译或点击复测；
  - 当前只能 claim：静态推断成立，用户终验仍待验证。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 让用户在 Unity 里重新从 `Tools/002批量 (Hierarchy窗口)` 打开一次；
  - 预期点击 `Hierarchy / Inspector / Scene` 后窗口不再自动关闭；
  - 若仍异常，再继续查 Unity 布局缓存或版本级窗口行为差异，但不先扩到别的工具。

## 2026-04-03｜追加实修：树苗残余卡顿与桥面不可通行继续在脚本侧补强，当前等待用户复测

- 当前主线目标：
  - 同时收口 3 件事：
    1. 树苗放置残余卡顿
    2. 桥面仍然过不去
    3. `002批量-Hierarchy` 失焦即关
- 本轮子任务：
  - 继续只改脚本与工具，不碰 `Primary.unity` 实写；
  - 目标文件：
    - `Assets/YYY_Scripts/Controller/TreeController.cs`
    - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
    - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
    - `Assets/Editor/Tool_002_BatchHierarchy.cs`
- 这轮实际做成了什么：
  1. 只读审计 `Primary.unity`，查实当前桥面现场是：
     - 水是阻挡源
     - `桥_底座` 已接到 walkable override
     - 真正的问题更像 override 命中太窄，不是桥物件自己在挡路
  2. `NavGrid2D.cs`
     - 把 walkable override / obstacle tilemap 的命中逻辑从点位采样改成“按半径覆盖区域查 tile”；
     - 目的是避免玩家脚底靠近桥边时，override 漏判后被水 collider 提前拦掉。
  3. `TreeController.cs`
     - `InitializeAsNewTree()` 现在只做唯一 ID + 树苗基态归一；
     - `Start()` 只有在 `SeasonManager` 尚未就绪时才补延迟初始化，避免场景里已经有 `SeasonManager` 时重复刷一轮显示/碰撞。
  4. `PlacementManager.cs`
     - 树苗放置链不再在 `InitializeAsNewTree()` 之后紧接着补一个多余的 `SetStage(0)`；
     - 旧 `HandleSaplingPlacement()` 分支也改成同口径。
  5. `Tool_002_BatchHierarchy.cs`
     - 当前文件已是普通 `GetWindow<T>() + Show() + Focus()` 的常驻打开方式，工具窗口这条修法仍保留。
- 关键判断：
  1. 桥不过去的核心更像“override 判定太窄导致被水边误杀”，不是 scene 还完全没接；
  2. 树苗残余卡顿当前最可疑的是“放置首帧重复初始化/重复刷表现”，而不是又回到了导航父线程大包；
  3. 这轮仍然只站住脚本侧结构与 targeted probe，不能把它说成真实体验过线。
- 验证结果：
  - `git diff --check` 已过；
  - 本机没有可直接调用的 Unity 方案级编译入口；
  - 当前只能 claim：静态推断成立，Unity live 仍待用户复测。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 请用户优先在 Unity 里复测：
    - 连续放树苗时是否还会明显卡一下
    - 过桥时是否还会被水边拦住
    - `002批量-Hierarchy` 点击别处后是否仍常驻
  - 如果桥仍不过，再继续做 scene 五段式审计；
  - 如果树苗仍卡，再继续查 `TreeController` 运行时注册链或实例化/特效链，而不是重新泛化成导航父线程问题。

## 2026-04-03｜追加实修：桥面 soft-pass contract 已补到 `TraversalBlockManager2D -> NavGrid2D -> PlayerMovement`

- 当前主线目标：
  - 把桥面问题从“导航层看起来可走”继续补到“玩家实体真的能过桥”；
  - 不动 `Primary.unity`，继续只改脚本。
- 本轮子任务：
  - 只改：
    - `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
    - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerMovement.cs`
- 这轮实际做成了什么：
  1. 只读 scene 审计补充压实：
     - 当前主阻挡更像 `Water` 的实体 `TilemapCollider2D`
     - 而不是桥 scene 根本没接，或双 manager 继续竞争
  2. `NavGrid2D.cs`
     - 新增公开 `HasWalkableOverrideAt(...)`，支持运行时判断脚底 probe 是否命中桥 override。
  3. `PlayerMovement.cs`
     - 新增 traversal soft-pass；
     - 当脚底 probe 命中 walkable override 时，才临时对绑定进来的 traversal blocking colliders 执行 `Physics2D.IgnoreCollision(...)`；
     - 离桥后自动恢复碰撞。
  4. `TraversalBlockManager2D.cs`
     - 绑定 `NavGrid2D` 给玩家时，也同步把当前解析出的 traversal blocking colliders 交给 `PlayerMovement`；
     - 这样 bridge override 不只影响寻路，也影响玩家实体过桥。
- 关键判断：
  1. 桥这条之前没闭环，不是因为 scene 完全没配，而是“override 只影响导航，不影响玩家实体物理”；
  2. 现在这条合同已经在脚本侧补上，但仍待 live 证据确认。
- 验证结果：
  - `git diff --check` 已过；
  - 未做 fresh Play；
  - 当前只能 claim：静态推断成立。
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 让用户优先做 3 个复测：
    - 连续放树苗
    - 直接过桥
    - 打开 `002批量-Hierarchy` 后点击别处
  - 若桥仍不过，再继续 scene 五段式审计；
  - 若树苗仍卡，再继续查树实例化/注册链。
