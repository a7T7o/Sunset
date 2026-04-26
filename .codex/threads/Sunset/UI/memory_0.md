# UI - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/UI/memory_1.md)。本卷只保留当前线程角色和恢复点。

## 线程定位
- 线程名称：`UI`
- 线程作用：玩家面主刀与 UI live 终验入口

## 当前主线
- 当前唯一活跃阶段：
  - [0.0.2_玩家面集成与性能收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/memory.md)

## 当前稳定判断
- 当前该线程只应承接：
  - workbench / prompt / proximity
  - task-list / formal dialogue / modal 玩家面治理
  - packaged 字体链
  - live 最终证据

## 当前恢复点
- 查旧大卷、旧 prompt 和早期 SpringUI 方案时看 `memory_1.md`
- 当前 live 问题直接回到 `0.0.2`

## 2026-04-10 只读补记｜任务清单与右上角调试字
- 本轮性质：
  - 只读分析，未进入真实施工
  - 按 Sunset 直聊口径，这轮未跑 `Begin-Slice`；当前 live 状态保持 `PARKED`
- 用户当前问题：
  - 任务清单为什么始终不像 `toolbar / state`
  - 右上角调试内容为什么在 `16:10 / 16:9` 下大小不一致
- 已确认结论：
  - `SpringDay1PromptOverlay` 不是稳定 HUD lane，而是独立 runtime overlay，自己找 parent canvas、自己 fade/show/hide、director 手动 Show/Hide、dialogue 再接一层治理
  - `SetExternalVisibilityBlock()` 在 prompt 里有定义但无调用点，说明背包/箱子从未正式接管过 prompt 的显示治理
  - `DialogueUI` 有 `FadeNonDialogueUi()`，所以正式对话时还算统一；`PackagePanelTabsUI` 没有对 prompt 做同类治理，所以打开背包时表现不像 toolbar/state
  - `TimeManagerDebugger` 用 `OnGUI()` 固定像素字号和 `Screen.width` 定位；不同分辨率下相对大小不一致是结构性结果
- 当前恢复判断：
  - 后续如果进入真实施工，PromptOverlay 应收回固定 HUD 层级/父层治理；不要再沿“透明度补丁”继续打补丁
  - 调试字若要跨分辨率一致，方向应是迁到 Canvas/CanvasScaler，或自己按 reference resolution 缩放

## 2026-04-10 只读补记｜放置模式状态提示
- 用户问题：
  - 玩家不知道什么时候开了/关了放置模式；提示应出现在交互提示上方，和交互提示形成同一条底部 HUD 语义带
- 代码现状：
  - `GameInputManager.IsPlacementMode` 已存在，且是真正覆盖锄头/种子/浇水等教学流程的总开关
  - `SpringDay1Director` 已有 `ShouldShowPlacementModeGuidance()` / `GetPlacementModeGuidanceText()`，说明 0.0.5 教学语义源已经存在
  - `InteractionHintOverlay` 有现成视觉壳，但当前是单卡实现，无法直接承载“状态卡在上、交互卡在下”的双卡关系
  - `PlacementManager.OnPlacementModeChanged` 不能单独作为状态提示真源，因为它只覆盖 placement preview，不覆盖锄头/水壶这种同样依赖 `IsPlacementMode` 的农田语义
- 恢复判断：
  - 若进入真实施工，放置模式提示应以 `GameInputManager.IsPlacementMode` 为主状态真源，以 `SpringDay1Director` 的 0.0.5 guidance 为文案/提醒触发源，并挂到 `InteractionHintOverlay` 同一条底部 HUD lane 中实现

## 2026-04-10 只读补记｜day1 prompt_09 审核结果
- 结论：
  - `day1` 这次给的 prompt 没有偏题，反而把 UI 线程最容易犯错的地方钉死了：上游真值归属
- 对齐点：
  - UI 线程此前的只读判断是对的，但现在应正式以 `prompt_09` 为施工边界
  - 后续若进入施工，唯一主刀就是 `005 放置模式状态提示`
  - 任务清单、workbench、toolbar/package 等其它 UI 问题本轮都不该吞回这一刀

## 2026-04-10 实装补记｜本轮已落地三刀
- 本轮性质：
  - 已从只读进入真实施工
  - `Begin-Slice` 已跑
  - 本轮结束前已 `Park-Slice`
  - 当前 live 状态：`PARKED`
- 已完成：
  - `InteractionHintOverlay.cs`
    - 新增放置模式状态卡
    - 同 lane 双卡布局：交互在下，状态在上
    - 消费 `GameInputManager.IsPlacementMode`
    - 消费 `SpringDay1Director.ShouldShowPlacementModeGuidance()` / `GetPlacementModeGuidanceText()`
  - `SpringDay1PromptOverlay.cs`
    - 固定 HUD 排序，不再跟随父 Canvas 漂移
  - `TimeManagerDebugger.cs`
    - 新增参考分辨率缩放
- 代码层验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `validate_script Assets/YYY_Scripts/TimeManagerDebugger.cs`
  - 三者均为：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - 阻断原因一致：
    - `No active Unity instance`
    - `CodexCodeGuard timeout-downgraded`
- 当前恢复点：
  - 下一轮若用户给 live 反馈，优先按 3 条体验线分诊：
    1. 放置模式状态卡的尺寸/重叠/时序
    2. 任务清单在 package/workbench/dialogue 下的真实层级体验
    3. 右上角调试字在不同分辨率下的体感一致性

## 2026-04-10 阻塞修复｜HideImmediate 残留调用
- 本轮子任务：
  - 修复 `InteractionHintOverlay.cs` 中我自己引入的 `HideImmediate` 缺失编译红
- 已完成：
  - `EnsureRuntime()` 入口已从旧调用 `HideImmediate()` 改为 `HideAllImmediate()`
- 代码层验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - 结果：
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
- 当前 live 状态：
  - `PARKED`

## 2026-04-10 状态卡补口｜只修状态 UI，定位 toolbar 病因
- 当前主线目标：
  - 只修 `InteractionHintOverlay` 的状态卡重叠，并汇报 `toolbar 点击失效 / 非放置模式工具不能用` 的病因
- 本轮子任务：
  - 调整状态卡尺寸和标题/正文布局
  - 只读核实 hotbar 点击链与滚轮链的分叉点
- 已完成：
  - `InteractionHintOverlay.cs`
    - 状态卡宽高增大
    - 标题区、正文区重新拆开
  - 病因定位：
    - `ToolbarSlotUI.OnPointerClick()` 里存在多层前置拦截
    - 真正 equip 发生在 `HotbarSelectionService.SelectIndex()` -> `RestoreSelection()` -> `EquipCurrentTool()`
    - 当前更像“点击 UI 视觉变了，但没稳定走到真正的 equip 主链”
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - `assessment=unity_validation_pending`
  - `owned_errors=0`
  - `external_errors=0`
  - `external_warnings=1`
- 恢复点：
  - 若用户下一轮让修 toolbar/工具链，就从 `ToolbarSlotUI` 点击分叉与 `GameInputManager.TryPrepareHotbarSelectionChange` 的 gating 开刀
- thread-state：
  - `Begin-Slice`: 已跑
  - `Park-Slice`: 已跑
  - 当前状态：`PARKED`

## 2026-04-10 状态卡补口 2｜修 `状态` 标签被标题压住
- 当前主线目标：
  - 继续只修状态卡显示问题
- 本轮子任务：
  - 修复截图里 `状态` 标签被标题覆盖
- 已完成：
  - `InteractionHintOverlay.cs`
    - 标签往左上角缩
    - 标题往右下错位
    - 标签和标题不再共享同一区域
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
  - `assessment=unity_validation_pending`
  - `owned_errors=0`
  - `external_errors=0`
- 当前 live 状态：
  - `PARKED`

## 2026-04-10 只读厘清｜day1 求助 prompt + 四条问题分诊
- 当前主线目标：
  - 不继续乱修，先把 `任务清单 / 状态条 16:10 / 状态提示卡 / 手持工具链` 四条线分清，并先发 day1 求助 prompt
- 已完成：
  - 已创建 day1 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-10_UI线程_向spring-day1求助_任务清单打包异常与治理口径确认prompt_32.md`
  - 已确认：
    - `PromptOverlay` 仍是独立 overlay 治理，不是统一 HUD 治理
    - `SpringDay1StatusOverlay` 没有 CanvasScaler，16:10 溢出属结构问题
    - `InteractionHintOverlay` 的状态卡仍是硬编码矩形切位
    - 正常读档 / 切场没有统一强制 placement-off，toolbar 点击链和滚轮链也不一致
- 下一步想法：
  - 任务清单先等 day1 给 authoritative 裁定
  - 我自己下一刀更适合先砍：
    1. `StatusOverlay` 的 16:10 缩放
    2. `GameInputManager + ToolbarSlotUI + HotbarSelectionService` 的手持/放置统一入口

## 2026-04-10 实装收口｜PromptOverlay 冻结后继续推进 own 链
- 当前主线目标：
  - 接受 day1 对 `PromptOverlay` 的 authoritative 裁定，不再碰它；继续只收 UI 自己的 `hotbar/placement/save-load` 与 `status HUD` 两条线
- 已完成：
  - [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
    - 新增统一即时切槽入口
    - 场景切换时统一 placement runtime reset
    - 世界左键只认活的 placement session
  - [ToolbarSlotUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs)
    - 点击切槽改为走 `GameInputManager.TryApplyHotbarSelectionChange`
  - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
    - 读档与 fresh start 前统一 `ForceResetPlacementRuntime`
  - [SpringDay1StatusOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1StatusOverlay.cs)
    - 补 `CanvasScaler`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 补 `CanvasScaler`
  - 已写给 day1 的阶段回执：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-10_UI线程_给spring-day1阶段回执_34.md`
- 验证结果：
  - `GameInputManager.cs` = `no_red`
  - `SaveManager.cs` = `no_red`
  - `InteractionHintOverlay.cs` = `no_red`
  - `SpringDay1StatusOverlay.cs` = `no_red`
  - `ToolbarSlotUI.cs` = `external_red`（外部 NPCMotionController 报错，不是 own）
- thread-state：
  - `Begin-Slice`: 已跑
  - `Park-Slice`: 已跑
  - 当前状态：`PARKED`

## 2026-04-10 只读核实｜day1 当前真实完成情况
- 本轮性质：
  - 只读核实 `spring-day1` 代码现场
  - 未进入真实施工，未跑 `Begin-Slice`
- 已核实结论：
  - `day1` 当前关键 runtime 落地不是只停在文档里，代码现场确实存在：
    - `SaveManager.cs` fresh start = `09:00`
    - `TimeManagerDebugger.cs` 已接 `TryNormalizeDebugTimeTarget(...)`
    - `SpringDay1NpcCrowdDirector.cs` 已有 `IsNightResting`
    - `StoryProgressPersistenceService.cs` 已去掉晚段私有位的 phase 偷推进
    - `StoryProgressPersistenceServiceTests.cs` 已补 `Load_DoesNotPromoteLateDayPrivateFlagsFromPhaseAlone`
  - 当前 git 现场与其 memory 报实一致：
    - 四个 runtime 文件 `Modified`
    - `StoryProgressPersistenceService.cs / StoryProgressPersistenceServiceTests.cs` 仍 `Untracked`
- 当前判断：
  - `spring-day1` 可以 claim `代码层推进真实成立`
  - 但还不能 claim `live / packaged 黑盒已闭环`
  - `PromptOverlay` 继续按 day1 authoritative 裁定冻结，UI 不再继续碰 `SpringDay1PromptOverlay.cs`
- 当前恢复点：
  - UI 线程继续 own 的 `hotbar / placement / save-load` 与 `status HUD`
  - `task list` owner 拆分继续等 day1 主刀完成后再接

## 2026-04-11 只读盘点｜我自己当前还没闭环的内容
- 本轮性质：
  - 只读盘点，未进入真实施工
  - 当前 live 状态继续 `PARKED`
- 当前未闭环项按 owner 划分：
  - `UI own`
    - `hotbar / placement / save-load`
      - 代码层统一入口、runtime reset 已落
      - 但 live 还没闭：
        - 点击切槽 vs 滚轮切槽是否完全同结果
        - 读档 / fresh start / 切场后 placement 是否始终归零
        - 空手 / 非放置模式是否还会触发残留 preview
    - `status HUD / hint HUD`
      - `CanvasScaler` 已补
      - 但 `16:10 / packaged` 下的血条、精力条、状态卡、提示卡还没拿到最终 live 票
    - `packaged 字体 / 缺字 / 显示异常`
      - 当前没有新的闭环证据，不能 claim 已过
  - `非 UI own`
    - `PromptOverlay / task list`
      - 已按 day1 authoritative 裁定转交
      - 当前第一真实 blocker 是 `formal task card` 和 `manual prompt` 仍混跑
      - 我不应继续在 `SpringDay1PromptOverlay.cs` 上追加治理补丁
- 现场提醒：
  - 当前 git 里我这边相关文件仍 dirty：
    - `GameInputManager.cs`
    - `ToolbarSlotUI.cs`
    - `SaveManager.cs`
    - `SpringDay1StatusOverlay.cs`
    - `InteractionHintOverlay.cs`
    - `SpringDay1PromptOverlay.cs`
- 当前恢复点：
  - 我下一轮如果继续真实施工，优先顺序应是：
    1. `hotbar / placement / save-load` live 闭环
    2. `status HUD / hint HUD` 的 packaged 分辨率收口
    3. `packaged 字体链`

## 2026-04-11 实装补口｜修 16:10 状态条与状态卡排版
- 本轮性质：
  - 已进入真实施工
  - `Begin-Slice` 已跑
- 已完成：
  - [HealthSystem.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/HealthSystem.cs)
    - responsive layout 改为按整条血条的真实可见内容 bounds 收边，不再只看 slider 小父框
    - HUD 重新显示时强制重算一次布局
  - [EnergySystem.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/EnergySystem.cs)
    - 同步改为按整条精力条的真实可见内容 bounds 收边
    - HUD 重新显示时强制重算一次布局
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `状态` 标签缩小
    - 标题移到标签下方独立一行
    - 正文与标题重新分区
    - 左侧竖线下移缩短，避免再刺进标题
- 当前判断：
  - 这轮已修正两个关键根因：
    - `16:10` 溢出之前是 clamp 错对象
    - 状态卡之前是标题/标签/装饰线共占同一区域
  - 但还需用户 live 终验
- 验证结果：
  - `HealthSystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `EnergySystem.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部项为 Unity `stale_status / GridEditorUtility` 噪音
- 当前恢复点：
  - 下一轮若继续真实施工，优先仍是：
    1. `hotbar / placement / save-load`
    2. `status HUD / hint HUD` live 终验失败项回修
    3. `packaged 字体链`

## 2026-04-11 实装补口｜InteractionHintOverlay 去重、补件与 farm 协作物
- 当前主线目标：
  - 继续只收我 own 的 `InteractionHintOverlay`，把用户实测中的“重叠 / 双状态 / 奇怪遮挡”先从代码根上收平，不再碰 `PromptOverlay`
- 本轮子任务：
  - 修 `InteractionHintOverlay` 的实例复用链、老场景升级补件链、状态卡同帧双触发链
  - 顺手整理给 `farm` 的 toolbar 空选中窄刀 prompt
- 关键根因：
  1. 场景里已有 inactive 的 `InteractionHintOverlay`
  2. 旧 `EnsureRuntime()` 只会新建，不会认领现有实例
  3. 旧 `EnsureBuilt()` 对缺字段的老实例会整棵 `BuildUi()`，在同一个 overlay 上再长第二套子树
  4. 放置模式 toggle 和 guidance 会在同一帧连续打两次状态卡
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `EnsureRuntime()` 先认领现有 overlay，再清重复实例
    - `BuildUi()` 改为复用现有子树、只补缺件
    - 场景旧实例补 `CanvasScaler / interactionCardCanvasGroup / status card`
    - 状态标签与标题重新分区，标题右移
    - 同帧 guidance / toggle 只保留一个状态语义
    - `EnsureBuilt()` 恢复快路径，避免 `LateUpdate()` 走全树扫描
  - 新增协作物：
    - [2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md)
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - Unity editor `stale_status` 仍是外部阻断
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - 通过
- 遗留与恢复点：
  - 这条 slice 现在停在“代码层已收平，等用户 live 看重叠/双状态是否消失”
  - 其他仍未闭环项还是：
    1. `toolbar / placement / save-load` live 闭环
    2. `status HUD / hint HUD` packaged/live 终验
    3. `packaged 字体链`
- thread-state：
  - `Park-Slice` 已跑
  - 当前状态：`PARKED`
  - blockers：
    1. `InteractionHintOverlay live validation pending`
    2. `toolbar empty-selection handoff prepared for farm`

## 2026-04-11 UI 精修｜状态提示卡字号与文案长度
- 当前主线目标：
  - 按用户最新截图反馈，继续只做 `InteractionHintOverlay` 的纯 UI 精修
- 本轮子任务：
  - 放大状态标签/标题/说明字号
  - 顶行整体下移
  - 缩小标题行与说明行间距
  - 把开关态文案压成更稳的单行
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `StatusCardWidth = 312`
    - `状态` 标签壳体与字体增大
    - 标题字号增大并整体下移
    - 说明字号增大并向上贴近标题
    - 开关态文案改成：
      - `农田/播种/浇水输入已开启，按 V 关闭。`
      - `农田/播种/浇水输入已关闭，按 V 开启。`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
- 恢复点：
  - 这轮只差用户再看一眼 runtime 观感
  - `Park-Slice` 已跑，当前状态：`PARKED`
  - blocker：`status card UI retest pending`

## 2026-04-11 UI 微调｜缩短竖条并继续收紧行距
- 当前主线目标：
  - 继续只收状态提示卡的视觉关系
- 本轮子任务：
  - 让左侧竖条更短
  - 拉开 `状态` 标签和标题的水平间距
  - 再缩小上下两行间距
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 竖条高度 `30 -> 24`
    - 标题 X 偏移 `78 -> 88`
    - 说明行顶部再上提
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部为 Unity 运行中现场的 `Missing Script (Unknown)` 与 TMP warning

## 2026-04-11 UI 微调｜修正“缩错对象”
- 用户最新纠正：
  - 不该继续缩左竖条
  - 该缩的是 `状态` 这两个字的底板
  - 左竖条反而可以略长一点
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 左竖条高度 `24 -> 28`
    - `状态` 底板宽度 `50 -> 44`
    - `状态` 文字自身可用区域同步收窄
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - 当前外部 blocker 为：
      - [SpringDay1LateDayRuntimeTests.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs): `SpringDay1BedInteractable` 缺符号

## 2026-04-11 UI 微调｜按检视器截图直接抄值
- 用户最新要求：
  - 不再让我猜，直接按截图里的检视器数值抄
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `StatusAccentLine`
      - `sizeDelta = (4, 47.08)`
    - `StatusTag`
      - `anchoredPosition = (29, -16)`
      - `sizeDelta = (44, 18)`
    - `StatusTitleText`
      - 保持截图对应布局：`left = 88`, `right = 18`, `posY = -12`, `height = 24`
    - `StatusDetailText`
      - 改成截图对应布局：`left = 29.8`, `right = 10.2`, `posY = 12`, `height = 22`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部是当前 Unity/Town 运行现场里的 `Missing Script (Unknown)` 噪音，不是本轮 own red

## 2026-04-11 UI 续记｜V0.5 checkpoint 与包裹页关系/地图重构
- 当前主线目标：
  - 先把 UI own 能诚实提交的 HUD / overlay 收成 `V0.5` checkpoint
  - 然后不再碰 `PromptOverlay`，直接收包裹里的 `人物关系页 / 地图页`
- 本轮子任务：
  1. 提交 UI own 的 `V0.5`
  2. 明确 `任务清单` owner 到底是谁
  3. 开始重构 `PackageNpcRelationshipPanel / PackageMapOverviewPanel`
- 本轮完成：
  - authoritative owner 已核实：
    - `任务清单 / PromptOverlay` 现在是 `day1` 主刀，不是我继续修
  - 本地 commit 已落：
    - `6ff90a48`
    - `checkpoint(ui): V0.5 HUD and overlay fixes`
  - 包裹页新改：
    - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
      - 名册按 `presence + stage + 当前阶段相关性` 排序
      - 默认选中改成当前阶段最相关的人，避免 `卡尔` 莫名其妙成默认主角
      - 卡片新增 `出场方式 + 关系阶段` 双 chip
      - 列表预览改吃当前 beat 的真实印象
    - [PackageMapOverviewPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
      - 地图区改成 zone blocks + route nodes + active halo
      - 移除底部 legend 栏
      - 当前重点点位可读性增强
    - [PackagePanelRuntimeUiKit.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs)
    - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
- 验证：
  - `validate_script PackageNpcRelationshipPanel.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `validate_script PackageMapOverviewPanel.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `validate_script PackagePanelRuntimeUiKit.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `validate_script PackagePanelLayoutGuardsTests.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `errors --count 20`
    - `errors=0`
    - `warnings=0`
- 当前阶段：
  - `V0.5` checkpoint 已交
  - 关系页 / 地图页进入 live 前可验的代码层重构阶段
  - `PromptOverlay` 问题则已明确交还 `day1`
- 下一步只做什么：
  - 等用户 live 看 `人物关系页 / 地图页`
  - 如果继续不顺眼，再只收这两个页面，不回漂 `PromptOverlay`
- 需要用户现在做什么：
  - 看包裹里的 `人物关系页 / 地图页`
  - `任务清单` 问题交给 `day1`

## 2026-04-12 UI 线程续记｜右侧上下文操作提示面板首版
- 当前主线目标：
  - 给玩家面补一个右侧靠边中部的轻量操作提示，只做 `玩法态 / 背包态` 两组，不动左下角交互提示。
- 本轮子任务：
  - 在 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 内落一个 runtime `ContextHintCard`
  - 用真实上下文信号切组
  - 支持 `Backspace` 只关闭当前组提示
- 本轮完成：
  - 新增右侧提示卡结构、tag/title/detail/footer 和最多 6 行键位说明
  - `PackagePanelTabsUI.IsPanelOpen()` 直接作为背包态真值
  - `Dialogue / Box / Workbench` 打开时隐藏右侧提示
  - 玩法态与背包态文案矩阵已首版写入
  - 关闭逻辑按组隔离：
    - 玩法态关掉不影响背包态
    - 背包态关掉不影响玩法态
    - 左下角交互提示完全不受影响
- 关键决策：
  - 这刀不碰 [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
  - 先把提示卡挂在现有 HUD overlay 上，后续 live 再按截图调观感
  - 关闭状态当前只保 runtime 生命周期，不上持久化，避免用户没有恢复入口
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 10`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 当前因为没有 active Unity instance，运行态红错证据未闭环
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前恢复点：
  - 下一步优先等用户 live 看：
    - 右侧卡是否过大/过小
    - 文案是否还要再减法
    - 与背包/玩法视面的遮挡关系是否顺

## 2026-04-12 UI 续记｜toolbar 第五格错位修复 + 玩法态补 `Shift`
- 当前主线目标：
  - 继续做 UI own 收口，只砍两个点：
    1. `toolbar 第五格内容不会显示`
    2. 右侧 `ContextHintCard` 漏掉 `Shift 加速`
- 本轮子任务：
  - 查清 `toolbar` 第五格是资源坏、输入链坏，还是视觉绑定坏
  - 最小补齐玩法态提示矩阵
- 关键决策：
  - 根因确认偏 UI/prefab 绑定面，不是 `farm` 的 hotbar 输入真源
  - 不去改 [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs) 或 `HotbarSelectionService`
  - 直接在 [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs) 内把 `Build()` 改成按 `Bar_00_TG` 名字解析出的槽号排序绑定
- 本轮完成：
  - [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)
    - `Build()` 先对 `gridParent` 子物体排序，再按 `0..11` 顺序绑定索引
    - 非 `Bar_00_TG` 子物体不会再误抢 `0` 号槽位
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `MaxContextRowCount` `6 -> 7`
    - 玩法态文案补 `Shift 加速移动`
    - `1-5` 提示扩成 `1-5 / 滚轮 切换手持`
- 验证：
  - `validate_script ToolbarUI.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 遗留与恢复点：
  - 这轮代码层已收平，但还缺用户 live 终验
  - 用户下一步优先看：
    1. 只放 5 个物体时 toolbar 第五格是否恢复
    2. 右侧玩法态提示是否出现 `Shift`
    3. `1-5 / 滚轮` 的提示有没有把卡片撑得太大
- thread-state：
  - `Begin-Slice` 已跑
  - 下一步准备直接 `Park-Slice`
  - 当前状态应回到 `PARKED`

## 2026-04-12 UI 续记｜右侧提示卡视觉修复
- 当前主线目标：
  - 只修用户刚指出的 `ContextHintCard 不好看`，不扩别的 UI
- 本轮子任务：
  - 把它从“大黑板 + 流程图 + 键帽出框”收成正式玩家面的轻量提示卡
- 用户反馈映射出的真实问题：
  - 壳体太大、太重
  - 左侧长竖线像串流程
  - 键帽冲出面板边界
  - 键帽列和说明列断裂
  - 文案像开发说明
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `ContextCardWidth` `274 -> 238`
    - `ContextCardRowHeight` `24 -> 20`
    - `ContextCardRowGap` `6 -> 4`
    - accent 改成顶部短条，不再整卡贯穿
    - 键帽列改为卡内固定列，说明列统一起点
    - 文案改成：
      - `操作提示`
      - `导航、交互、加速与手持操作。`
      - `切页、拿取与拖拽操作。`
      - `退格关闭提示`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 遗留与恢复点：
  - 这轮只站住结构和 targeted probe
  - 还需要用户看新的 live 图，判断是否真的从“测试味”收回来了

## 2026-04-12 UI 续记｜右侧提示卡第二刀精修
- 当前主线目标：
  - 用户明确要求这轮只修 `ContextHintCard`
- 本轮子任务：
  - 在结构已经站住的前提下，再做一轮“更轻、更短、更像正式卡片”的减法
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `ContextCardWidth` `238 -> 220`
    - `ContextCardMinHeight` `176 -> 154`
    - `ContextCardRowHeight` `20 -> 17`
    - `ContextCardRowGap` `4 -> 3`
    - 表头与页脚字号进一步缩小
    - 键帽宽高与饱和度进一步收敛
    - 文案改成更短的 `常用操作 / 导航、交互、加速、手持。 / 切页、拿取、拖拽。 / 退格关闭`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 遗留与恢复点：
  - 这轮只继续推进 `ContextHintCard` 自身观感
  - 还需要用户继续看新的 live 图

## 2026-04-12 UI 续记｜右侧提示卡边界透明安全做法核实
- 当前主线目标：
  - 继续 UI own：只收 `ContextHintCard` 和 `TimeManagerDebugger`，把“右侧提示卡靠近边界时自适应透明”这件事固定成不会误伤别的 HUD 的安全方案。
- 本轮子任务：
  1. 核实右卡边界透明是否已经只作用于右卡本身
  2. 核实正式剧情 / 正式对话时右上角时间与调试提示的隐藏条件
  3. 把这轮结论补进线程 memory，避免后续又回到“整根 overlay 一起淡”的错误方向
- 本轮结论：
  - `InteractionHintOverlay` 根下面同时挂了左下角交互提示、放置状态卡和右侧上下文提示卡，不能把整根挂进 boundary focus 根层一起 fade。
  - 当前仓库里更安全的实现已经存在：
    - `SetContextCardAlpha()` 只设置 `_contextRequestedAlpha`
    - `ApplyContextCardAlpha()` 最终用 `_contextRequestedAlpha * _contextBoundaryAlpha`
    - `UpdateContextBoundaryFade()` / `ResolveContextBoundaryTargetAlpha()` 只作用 `contextCard`
    - 边界阈值直接复用 [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 的右边界语义常量
  - `Package` 页面打开时保持 fully visible；`Dialogue / Box / Workbench` 时右卡直接隐藏，不参与边界 fade。
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 里的 `ShouldHideTopRightHud()` 已接：
    - `DialogueManager.Instance.IsDialogueActive`
    - `SpringDay1Director.Instance.ShouldForceHideTaskListForCurrentStory()`
    - 命中任一就不绘制右上角时间 / 调试帮助
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `validate_script TimeManagerDebugger.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs`
    - clean
  - blocker 仍是：本机没有 active Unity instance，无法补 fresh live 证据，不是 own red
- 当前阶段：
  - 这轮已经把“怎么安全做右卡边界透明”压实到结构层，并确认代码方向正确
  - 还没到用户体感过线，仍缺 live 验证
- 下一步只做什么：
  - 若用户继续验这条线，就只看：
    1. 贴近右边界时右卡是否平滑减淡
    2. 左下角交互提示 / 放置状态卡是否完全不受影响
    3. 正式剧情 / 正式对话时右上角时间与调试提示是否彻底退场
- 需要用户现在做什么：
  - 当前无；这轮先把安全做法和代码链固定下来，等下一次 live 验即可

## 2026-04-12 UI 续记｜右卡排版重对齐 + 背包开关恢复任务清单压住
- 当前主线目标：
  - 用户最新反馈明确要求继续完善两个点：
    1. 右侧 `ContextHintCard` 要把中线、字号、页脚键位提示真正做好
    2. 背包打开时，左侧任务清单要恢复被 page 压住/退场
- 本轮子任务：
  1. 只改 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 的卡内布局与页脚提示
  2. 不碰已经脏着的 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)，只在 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 走现有 block 接口恢复背包退场
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右卡整体放大一档：
      - `ContextCardWidth 220 -> 238`
      - `ContextCardMinHeight 154 -> 176`
      - `ContextCardRowHeight 17 -> 20`
      - `ContextCardRowGap 3 -> 4`
    - 键帽 / 正文 / 页脚重新按中线关系布局
    - 说明文字改成 `MidlineLeft`
    - 标题、正文、键帽、页脚字号整体抬升
    - 页脚新增 `Backspace` 键帽，动作文字改成 `关闭提示`
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - 新增 `RefreshPromptOverlayVisibilityBlock()`
    - `OpenPanel()` / `ClosePanel()` / `EnsurePanelOpenForBox()` 都会在背包或箱子页拉起/收起时，显式把当前运行中的 `PromptOverlay` 走一次 `SetExternalVisibilityBlock(...)`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `validate_script PackagePanelTabsUI.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
    - clean
  - 当前 external 只剩 Unity 现场 warning / stale status，不是本轮 own red
- 当前阶段：
  - 这轮代码层已经把用户刚点名的 4 件事里收了 3 件半：
    - 右卡中线
    - 右卡字号
    - 退格键帽
    - 背包打开时任务清单退场恢复口
  - 还差用户 fresh live 终验来判断它是不是终于顺眼、是不是彻底恢复
- 下一步只做什么：
  - 等用户 live 看这两刀是否真的回正
- 需要用户现在做什么：
  - 测 3 个点：
    1. 背包/箱子打开时任务清单是否恢复退场
    2. 右卡每行键帽和正文是否中线对齐
    3. 页脚 `Backspace` 键帽是否清楚、不卡壳

## 2026-04-13 UI 续记｜右卡中心线与退格图形键
- 当前主线目标：
  - 继续只收 `ContextHintCard`，不扩别的 UI
- 本轮子任务：
  1. 把每行键帽和说明文字真正压到同一条中心线
  2. 把页脚 `Backspace` 英文单词键改成图形退格键
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - description 布局从左右 offset 拉伸改成固定中心点布局
    - 页脚新增 runtime `ContextFooterKeyIcon`
    - 新增 `GetOrCreateBackspaceIconSprite()`，运行时生成退格图形 sprite
    - 原 `ContextFooterKeyText` 保留兼容但运行时直接隐藏
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前阶段：
  - 这轮已经把用户刚点名的两个问题落实成代码
  - 仍缺用户 fresh live 终验
- 下一步只做什么：
  - 等用户看右卡最后一眼
- 需要用户现在做什么：
  - 看 2 个点：
    1. 行内中线是不是终于平了
    2. 页脚退格键是不是终于不像字母键了

## 2026-04-13 UI 续记｜退格键重画
- 当前主线目标：
  - 用户裁定：其他都过了，只剩页脚退格键太丑，要重做。
- 本轮子任务：
  - 只重画 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 的 runtime 退格图形键
- 本轮完成：
  - 退格键从上一轮的块状箭头重画成更接近真实退格键的图形：
    - 左侧箭头轮廓
    - 右侧主体
    - 中间删除 `X` 语义
  - 页脚键帽宽度与 icon 可见尺寸同步轻微放大
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前阶段：
  - 这轮是纯视觉重画，代码层已收住
- 下一步只做什么：
  - 等用户看这个退格键是否顺眼
- 需要用户现在做什么：
  - 看页脚退格键这一处

## 2026-04-13 UI 续记｜顶部单行化
- 当前主线目标：
  - 用户继续只收右卡，要求顶部更像放置状态提示：标签和标题同一行，整体再小一点。
- 本轮子任务：
  - 只改 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 的顶部布局和卡片尺寸
- 本轮完成：
  - 顶部 `玩法 + 常用操作` 改成单行
  - 整卡继续缩一档
  - 行间距继续收一档
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前阶段：
  - 这轮是纯布局微调，代码层已收住
- 下一步只做什么：
  - 等用户看顶部这一行是否顺手
- 需要用户现在做什么：
  - 看顶部 `玩法 + 常用操作` 这一行

## 2026-04-13 UI 续记｜头部中心线与整块留白再收口
- 当前主线目标：
  - 用户继续只收右卡，明确指出“位置不对、细节一堆问题”，所以这轮继续只收几何关系。
- 本轮子任务：
  1. 把头部竖线 / 标签 / 标题真正压在同一条中心线上
  2. 把整卡再缩一点，把底部空腔压掉
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 整卡缩到 `220 x 156`
    - 头部三件套统一中心线
    - 内容整体上提，footer 区与底部空腔继续压缩
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前阶段：
  - 这轮仍是纯几何微调，代码层已收住
- 下一步只做什么：
  - 等用户看右卡整体位置关系是否终于顺手
- 需要用户现在做什么：
  - 看右卡整体位置关系这一处

## 2026-04-13 UI 续记｜退格符号标准化
- 当前主线目标：
  - 用户继续只收右卡，直接指出页脚回退键“毫无意义”，所以这轮只重画退格符号本体。
- 本轮子任务：
  - 只改 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 的 runtime 退格符号绘制
- 本轮完成：
  - 退格符号改成更标准的退格轮廓：
    - 左指向箭头
    - 右侧主体
    - 中间删除 `X`
- 验证：
  - `validate_script InteractionHintOverlay.cs`
    - `assessment=external_red`
    - `owned_errors=0`
    - `external_errors=4`
  - 这 4 条 external 都是当前 Unity 场景已有的 missing script，不是本轮引入
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - clean
- 当前阶段：
  - 这轮 own 代码已收住，外部 scene red 仍在
- 下一步只做什么：
  - 等用户看退格符号是否终于顺眼
- 需要用户现在做什么：
  - 看页脚退格符号这一处

## 2026-04-13 UI 续记｜最小安全集中检查补丁
- 当前主线目标：
  - 用户要求这一轮只按最小、最安全、最可回退方案，集中检查 4 个点：
    - 任务清单边界透明
    - 箱子打开时也要有右侧提示
    - 关系页左上空头像框太大
    - 工作台 A 完成后不准刷新 B 的数量选择
- 本轮子任务：
  - 只改 4 个文件：
    - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
    - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
- 本轮完成：
  - PromptOverlay 重新接回边界透明，最低透明度改到 `40%`，并改成渐进式 `SmoothStep`
  - 右侧提示卡新增 `箱子` 组，箱子打开时也会显示；footer 键帽跟随蓝色组态、图标放大重画、底部中线重新对齐
  - 人物关系页左上无头像时改成更小的头像框，并显示“暂无画像”
  - 工作台完成其他配方时，不再清空当前选中配方的数量和详情上下文
- 验证：
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate SpringDay1WorkbenchCraftingOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PersistentPlayerSceneBridge` => `errors=0`, `warnings=2`
  - `errors --count 20 --output-limit 5` => `errors=0`, `warnings=0`
  - `validate_script PersistentPlayerSceneBridge` => `assessment=unity_validation_pending`, `owned_errors=0`，工具侧 blocker 仍是 Unity `stale_status`
- 当前阶段：
  - 代码补丁已完成，等待用户集中 live 检查
- 下一步只做什么：
  - 看用户集中检查回执，再决定是否继续只收局部细节
- 需要用户现在做什么：
  - 重点测这 4 个点，不需要再泛测别的 UI

## 2026-04-13 UI 续记｜fresh live 二次纠偏
- 当前主线目标：
  - 用户 fresh live 后新增 3 个具体问题：
    - 箱子页打开时任务清单仍压在上面
    - 右卡 footer 图标还是怪
    - 关系页左侧大方块根本没被收小
- 本轮子任务：
  - 只继续改 3 个文件：
    - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
    - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
- 本轮完成：
  - 箱子 open/close 也正式接管任务清单外部 block，关闭时按“箱子/背包是否还开着”一起算
  - 右卡 footer 图标改成更小的纯退格箭头，keycap 也一起缩小
  - 关系页左侧头部阶段芯片改成固定锚点布局，避免再长成大砖块
- 验证：
  - `manage_script validate BoxPanelUI` => clean
  - `manage_script validate PackageNpcRelationshipPanel` => clean
  - `manage_script validate InteractionHintOverlay` => warning-only
  - fresh `errors` => `errors=0`, `warnings=0`
- 当前阶段：
  - 第二轮纠偏已落完，等待用户继续看 live 结果
- 下一步只做什么：
  - 如果这 3 处还有偏差，继续只做这 3 处细调，不扩散
- 需要用户现在做什么：
  - 优先看箱子页任务清单、右卡 footer、关系页左侧头部

## 2026-04-13 UI 续记｜双链 block 稳定化
- 当前主线目标：
  - 用户继续要求“进行”，所以这轮只补最后一个稳定性口：箱子页和背包页都要一致地压住任务清单
- 本轮子任务：
  - 只改：
    - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
- 本轮完成：
  - `PackagePanelTabsUI` 的 prompt block 改成同时考虑 `panelRoot.activeSelf` 和 `IsBoxUIOpen()`
  - `BoxPanelUI` 的 prompt block 改成同时考虑 `_isOpen`、`packageTabs.IsPanelOpen()`、`packageTabs.IsBoxUIOpen()`
  - 目的就是避免箱子链/背包链互相把任务清单 block 顶掉
- 验证：
  - `manage_script validate PackagePanelTabsUI` => clean
  - `manage_script validate BoxPanelUI` => clean
- 当前阶段：
  - 本轮代码已收住，等用户 fresh live 测最后这条稳定性链
- 下一步只做什么：
  - 如果任务清单还会露出来，就只继续查 `SpringDay1PromptOverlay` 当前运行时实例和 block 来源，不扩散
- 需要用户现在做什么：
  - 优先打开箱子和背包，再看任务清单是否终于完全退场

## 2026-04-13 UI 续记｜Day1 PromptOverlay contract 第一刀已落
- 当前主线目标：
  - 用户这轮明确把 Day1 `任务清单 / Prompt / bridge prompt / 对话期间显隐 / modal 层级 / 恢复后玩家可见面` 全权交给 UI；本轮继续真实施工，不回漂 runtime staging/save-load。
- 本轮子任务：
  - 先收真正的 PromptOverlay 根因：
    - modal block 必须命中真实 runtime 单例
    - PromptOverlay 必须回到 HUD lane，而不是继续压在 package/box page 上
    - 正式对话开始/结束时，PromptOverlay 自己就要吃事件真值
  - 顺手把右卡 footer 退格键和关系页无头像大空框再收一刀
- 本轮完成：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 `CurrentRuntimeInstanceOrNull / SetGlobalExternalVisibilityBlock`
    - `EnsureAttachedToPreferredParent()` 现在会把 PromptOverlay 插回 HUD 正确兄弟位
    - 正式对话开始/结束改成直接立即收显隐，不再等待别的链补淡出
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - prompt block 改成统一打到 PromptOverlay 全局入口
  - [BoxPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs)
    - 箱子链也改成统一打到 PromptOverlay 全局入口
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - footer backspace 图标重画，底部键帽尺寸与 icon 中心重新收平
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 无头像时左上占位继续缩到更小的安全尺寸
  - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
    - 新增 PromptOverlay HUD lane / stale duplicate 两条护栏
- 验证结果：
  - `manage_script validate SpringDay1PromptOverlay` => `errors=0`, `warnings=2`
  - `manage_script validate PackagePanelTabsUI` => `errors=0`, `warnings=0`
  - `manage_script validate BoxPanelUI` => `errors=0`, `warnings=0`
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate PackagePanelLayoutGuardsTests` => `errors=0`, `warnings=0`
  - `errors --count 20 --output-limit 10` => `errors=0`, `warnings=0`
  - `PackagePanelLayoutGuardsTests` => `5/5 passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldHideDuringDialogueAndRecoverAfterwards` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldPreferBaseCanvasUnderUiRoot_InsteadOfModalPackageCanvas` => `passed`
- 当前阶段：
  - 结构 / targeted probe 已过；真正的 packaged/live 玩家面终验还得等用户继续测
- 下一步只做什么：
  - 继续等用户 fresh live / packaged 看任务清单在背包、箱子、正式对话里的玩家面
  - 如果还有问题，下一刀只回 `SpringDay1PromptOverlay` 本体，不再退回外围补丁
- 需要用户现在做什么：
  - 优先测：
    1. 开背包/箱子时任务清单是否终于不再压在 page 上
    2. 正式对话开始/结束时任务清单是否稳定退场与恢复
    3. 右卡 footer 的退格键观感
    4. 关系页无头像时左上空框是否终于不再扎眼

## 2026-04-13 UI 续记｜PromptOverlay 闪烁根因第二刀 + 卡尔 chip 栏纠偏
- 当前主线：
  - 继续收 Day1 `任务清单 / PromptOverlay / bridge prompt / 对话显隐 / modal 层级` 的玩家可见稳定性；本轮新增用户纠偏点是“卡尔的问题不是头像壳，而是列表右侧好感度小筹码栏不统一”。
- 本轮实际做成了什么：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 `_visibilityAlpha + _boundaryFocusAlpha` 合成口，PromptOverlay 开始改成单一 alpha owner
    - 边界透明不再直接改最终显隐真值，而是只作为乘法边界 alpha 参与合成
    - `LateUpdate` 的显隐判断改为看 `_visibilityAlpha`，避免边界透明把“已显示/未显示”判断误导成闪烁
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
    - 边界焦点系统命中 `SpringDay1PromptOverlay` 时，改成调用 `SetBoundaryFocusAlpha(...)`，不再直接抢写它的 `CanvasGroup.alpha`
  - [DialogueUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs)
    - `ShouldManageAsNonDialogueUi(...)` 现在会明确跳过 `SpringDay1PromptOverlay`，避免正式对话和 PromptOverlay 自己的事件显隐双写 alpha
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - 按用户纠偏，把卡尔所在的列表右侧 chip 栏收成固定栏宽、固定 chip 宽高、顶部对齐、禁止换行
    - `未露面` 这类长文案不再把这列视觉撑乱
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右侧提示卡 footer 的 backspace 键帽又收一刀：键帽略放大，icon 重新改成更正常的 backspace 轮廓
  - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
    - 新增护栏：`DialogueUI` 不能再把 PromptOverlay 当普通 sibling 淡出
    - 新增护栏：boundary alpha 只能压缩 PromptOverlay 最终 alpha，不能反向把它重新抬出来
- 验证结果：
  - `manage_script validate SpringDay1PromptOverlay` => `errors=0`, `warnings=2`
  - `manage_script validate PersistentPlayerSceneBridge` => `errors=0`, `warnings=2`
  - `manage_script validate DialogueUI` => `errors=0`, `warnings=1`
  - `manage_script validate PackageNpcRelationshipPanel` => `errors=0`, `warnings=0`
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate PackagePanelLayoutGuardsTests` => `errors=0`, `warnings=0`
  - fresh `errors --count 20 --output-limit 10` => `errors=0`, `warnings=0`
  - 说明：Unity 当前 live tests 这轮没再主动抢跑，因为用户正在用运行中的 Editor 看 UI；这轮只确认了代码层 clean 和 fresh console clean，还没 claim live 体验已过线
- 当前阶段：
  - 结构 / targeted probe 再往前推了一刀；真正的 live 终验现在聚焦两处：`PromptOverlay` 在背包/边界/对话时是否还闪，和 `卡尔` 这列 chip 栏是否终于统一
- 下一步只做什么：
  - 等用户 fresh live 继续验 `PromptOverlay` 闪烁
  - 如果还闪，下一刀继续只查 `PromptOverlay` 自身与 blocking 真值，不再回到关系页或右卡泛修
- 需要用户现在做什么：
  - 优先复看：
    1. 打开背包时任务清单是否还闪
    2. 靠近边界时任务清单是否还闪
    3. 正式对话开始/结束时任务清单是否还闪
    4. 卡尔这一列右侧小筹码是否终于统一

## 2026-04-13 UI 续记｜BridgePromptRoot manual-aligned layout match
- 当前主线目标：
  - 用户把本轮切成新的窄刀：`BridgePromptRoot` 必须对齐他手动摆好的正式布局，不再泛修别的 UI。
- 本轮子任务：
  - 只改 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 里 `BuildBridgePromptShell()` 的 root 布局真值。
- 本轮完成：
  - 新增 bridge prompt 专用常量：
    - `BridgePromptOffsetX = 13f`
    - `BridgePromptOffsetY = 368f`
    - `BridgePromptWidth = 328f`
    - `BridgePromptHeight = 34f`
  - `BridgePromptRoot` 继续保持左下锚、`pivot=(0, 0.5)`，但运行时位置和尺寸改为用户手调基线，不再借用 `TaskCardLeftPadding`
- 验证：
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - 外部 blocker 为用户当前运行态里的 missing-script 与 `stale_status`
  - `manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0`
    - `warnings=2`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - 仅现有 CRLF/LF 警告，无新 diff 格式错误
- 当前阶段：
  - 这轮代码已落到位，但还没有 live 终验
- 下一步只做什么：
  - 等用户直接看 bridge prompt 玩家面；如果还差，就继续只收这一个 root 的几何关系
- 需要用户现在做什么：
  - 看 `BridgePromptRoot` 是否已经回到他手动摆的那套布局

## 2026-04-13 UI 续记｜Chest context hint palette parity + footer keycap polish
- 当前主线目标：
  - 用户继续只收右侧提示卡，明确指出“箱子场景下颜色上下不一致”以及“关闭提示按钮还怪”。
- 本轮子任务：
  - 先审源头，确认这是 `InteractionHintOverlay` 的 `Chest` 组提示卡表现问题，不是 `BoxPanelUI` 主脚本逻辑问题
  - 再只改 `InteractionHintOverlay.cs` 的 chest palette 和 footer keycap 比例
- 本轮完成：
  - `ContextHintGroup.Chest` 现在拥有独立的整套暖色 palette：
    - 顶部 accent/tag
    - 行内 keycap
    - footer keycap / icon / 文案
    - 全部统一到同一组暖色语义
  - footer 的关闭提示键帽从 `44x20` 收到更紧的 `40x18`
  - footer icon 放大到 `24x14`，并轻微下压一点，减少“漂”和“空”
- 验证：
  - `manage_script validate --name InteractionHintOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0`
    - `warnings=1`
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - external blocker：`Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs:742` 运行态 inactive coroutine
- 当前阶段：
  - 这轮代码已落地，等待 live 看卡面是否顺眼
- 下一步只做什么：
  - 若还有问题，继续只收箱子提示卡这张卡，不扩散到别的 UI
- 需要用户现在做什么：
  - 直接看箱子场景下右侧提示卡的配色和 `关闭提示` 键帽

## 2026-04-13 UI 续记｜Tooltip visual contract polish + mouse follow edge clamp
- 当前主线目标：
  - 用户把主线切到 farm 委托的 tooltip 视觉 contract，并明确追加限制：不要改 tooltip 已有功能，只做他描述的微调。
- 本轮子任务：
  - 只改 [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs)
  - 不碰 `ToolbarSlotUI.cs` 和 `InventorySlotInteraction.cs` 的 hover 抑制 / show-hide / 交互逻辑
- 本轮完成：
  - tooltip 壳体从更小更挤的测试态收成更正式的纸卡感：
    - `TooltipWidth: 184 -> 212`
    - `TooltipMinHeight: 74 -> 88`
    - `PanelBorder: 3 -> 4`
    - 内容宽度、padding、行间距、header padding、quality icon 一起微调
    - 标题/状态/描述/价格字号全部上调
    - 根框补轻微 outline，内板补轻微 shadow
  - 跟鼠标与边界显示做了最小安全收口：
    - 局部 `_movementRect` 只有在真能容纳 tooltip 时才参与约束
    - toolbar 这种窄条 hover 会自动退回 canvas 级 bounds
    - 四个方向越界时先翻边，再做带 `TooltipEdgePadding` 的 clamp
- 验证：
  - `manage_script validate --name ItemTooltip --path Assets/YYY_Scripts/UI/Inventory --level standard --output-limit 5`
    - `errors=0`
    - `warnings=1`
  - `validate_script Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs --count 20 --output-limit 5`
    - `assessment=external_red`
    - `owned_errors=0`
    - external blockers：当前用户运行中的 Unity 现场里 existing missing-script / stale_status
  - `git diff --check -- Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
    - clean
- 当前阶段：
  - 代码已落位，等 live 看 tooltip 真观感
- 下一步只做什么：
  - 若用户还觉得 tooltip 不正式或贴鼠标不顺，继续只收 `ItemTooltip.cs`
- 需要用户现在做什么：
  - 分别看：
    1. toolbar hover 的 tooltip 会不会还越出屏幕或贴鼠标太差
    2. 背包 hover 的 tooltip 是否和 toolbar 同一套正式语言

## 2026-04-13 UI 续记｜右侧提示卡黄系统一 + BACKSPACE 键帽 + 状态卡缩小 + 右上角调试提示恢复
- 当前主线目标：
  - 用户把这轮明确钉成两条 own 一起收：继续把右侧提示卡和状态卡收精，同时把右上角调试提示恢复出来，不能再被我关掉。
- 本轮子任务：
  - 只改 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)、[TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)、[PersistentManagers.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/PersistentManagers.cs)
  - 不扩回别的 UI 逻辑链
- 本轮完成：
  - `InteractionHintOverlay.cs`
    - `Gameplay / Package / Chest` 三组右侧提示卡统一回暖黄语义，不再保留背包蓝卡
    - footer 彻底改成 `BACKSPACE` 文字键帽，不再用退格图标
    - 放置模式状态卡整体缩小，tag / title / detail / 阴影 / 内容区一起收紧
    - 默认状态详情文案缩短成单行优先：`耕地/播种/浇水已开启/关闭，按 V ...`
  - `TimeManagerDebugger.cs`
    - 去掉 `ShouldHideTopRightHud()` 隐藏链
    - `EnsureAttached(...)` 默认把 `showDebugInfo` 重新开回 `true`
  - `PersistentManagers.cs`
    - 常驻初始化也改回 `showDebugInfoByDefault: true`
- 验证：
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `manage_script validate TimeManagerDebugger` => `errors=0`, `warnings=1`
  - `manage_script validate PersistentManagers` => `errors=0`, `warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/TimeManagerDebugger.cs Assets/YYY_Scripts/Service/PersistentManagers.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - 只有既有 CRLF/LF 警告，无新 diff 格式错误
  - `validate_script`
    - `InteractionHintOverlay.cs` / `TimeManagerDebugger.cs` => `assessment=blocked`，原因是 `CodexCodeGuard returned no JSON`
    - `PersistentManagers.cs` => `assessment=external_red`, `owned_errors=0`
    - fresh console 仍是外部现场红：`DialogueUI.cs:1975 IndexOutOfRangeException` + missing script，非本轮 own 引入
- 当前阶段：
  - 结构 / targeted probe 已落地；还差用户 live 看最终观感
- 下一步只做什么：
  - 等用户继续看：右上角调试提示是否恢复、右侧提示卡黄系是否统一、`BACKSPACE` 键帽是否过线、状态卡大小是否顺眼
- 需要用户现在做什么：
  - 继续 live 看上面 4 项即可；这轮不需要用户补本地信息

## 2026-04-14 UI 只读续记｜六张图审计 + farm 常用操作 contract 并入
- 当前主线目标：
  - 用户要求这轮只做只读审计，不切主线：把 6 张图里的真实问题、补充进来的 farm `常用操作提示 contract`、以及我接下来该怎么改，一次讲清楚。
- 本轮核实结论：
  - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs)
    - tooltip 标题现在直接吃 `itemData.itemName`，所以会把 `Axe_0 / Hoe_0 / WateringCan` 原样打给玩家
    - tooltip 视觉虽然比旧版大，但当前字号仍偏小，且正文仍被 [ItemTooltipTextBuilder.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs) 的 `2 行 / 44 字` 护栏截断
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - 工作台这边已经有一套成熟的玩家向命名与说明映射：`GetWorkbenchPlayerFacingName / Description / GetItemDisplayName`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 当前右侧提示卡已经分成 `Gameplay / Package / Chest` 三组，但 footer 仍是 `BACKSPACE` 在左、`关闭提示` 在右，语义像两个平级标签，不像“动作说明 + 对应按键”
    - 当前 package/chest/gameplay 文案还没完全吃透 farm 新 contract：世界内基本够；背包缺更明确的 `Ctrl+左 = 单取 / 快捷装备`；箱子的 `双击` 还没写成“快速转移 / 双向”
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) + [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs)
    - 右上角现在只是裸 `OnGUI` 文本，没有正式壳体
    - “10 分钟一跳”不是显示问题，而是 `TimeManager` 真源当前就是 10 分钟步进：`currentMinute += 10`，`SetTime()` 也会强制 clamp 到 10 的倍数
- 下一步修改原则：
  - tooltip 命名必须复用或抽取工作台那套成熟命名，不允许继续把内部 SO 名原样给玩家
  - 右侧提示卡要同时做两层收口：
    1. 视觉：字体、对比度、footer 结构
    2. 语义：并入 farm contract 的 world / package / chest 场景差异
  - 右上角调试时间 UI 可以安全美化
  - 但“分钟精度改成个位数”不是 UI 微调，是 `TimeManager` 核心改造，必须单列成独立 runtime 切片，不能伪装成这轮顺手小修

## 2026-04-14 UI 续记｜tooltip 成熟命名 + 右侧提示卡 contract 落地 + 右上角时间 HUD 收口
- 当前主线目标：
  - 用户要求继续当前 UI 主线，不切题，直接把 tooltip 正式命名、右侧提示卡正式玩家面、以及右上角时间/调试 HUD 一起落地。
- 本轮完成：
  - [ItemTooltipTextBuilder.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs)
    - 新增 `BuildPlayerFacingTitle(ItemData itemData)`，把 tooltip 标题正式切到玩家向命名
    - 内置工作台同语义的成熟映射：工具 `0~18`、剑 `200~205`、箱子 `1400~1403`
    - 兜底把 `WateringCan / Pickaxe / Axe / Hoe / Storage / Sword` 这类内部名归一成中文玩家词
    - tooltip 描述预算从 `2 行 / 44 字` 放宽到 `3 行 / 72 字`
  - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs)
    - 标题正式改用 `BuildPlayerFacingTitle(...)`
    - 壳体、padding、quality icon、标题/状态/描述/价格字号继续放大一刀
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右侧提示卡正式分成 `Gameplay / Package / Chest`
    - 文案按 farm contract 收到当前真实语义
    - footer 固定为 `关闭提示  BACKSPACE` 结构，keycap 宽度和间距做了收口
    - 边界透明做成渐进，最低 alpha 固定 `40%`
  - [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs)
    - 真源时间粒度改成 1 分钟：`minuteStepsPerHour = 60`、`AdvanceMinute()` 改成 `+1`、`Update()` 改成 `while`
    - `SetTime()` 改成接受 `0~59`
    - 旧场景里的 `minuteStepsPerHour=6` 会在 `OnValidate()` 自动修正到 `60`
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 右上角调试区从裸字改成正式小 HUD：深色卡片、黄 accent、时间卡 + 快捷键卡两块
    - 时间显示固定精确到分钟个位数
    - `+/-` 改成真正的 `±1 小时且保留分钟`
    - 快捷键 keycap 宽度按文本自适应，`↓/↑`、`+/-` 不会再被挤坏
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs Assets/YYY_Scripts/TimeManagerDebugger.cs Assets/YYY_Scripts/Service/TimeManager.cs --count 20 --output-limit 10`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 当前卡点是 Unity 实例 `stale_status / compiling / playmode_transition`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - fresh console 仍有 external 现场红：一组 `The referenced script (Unknown) on this Behaviour is missing!`
    - 现阶段不能 claim 全量 no-red，只能报“本轮 touched 文件未见 owned red”
  - `git diff --check -- Assets/YYY_Scripts/Service/TimeManager.cs Assets/YYY_Scripts/TimeManagerDebugger.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs`
    - 无新 diff 格式错误；仅剩既有 `TimeManagerDebugger.cs` CRLF/LF 提示
- 当前阶段：
  - 结构 / targeted probe 已落地；等待用户 live 看最终观感与分钟流逝体感
- 下一步只做什么：
  - 如果用户 live 后仍觉得不顺，只继续收：
    1. 右侧提示卡 footer / 行距
    2. 右上角 HUD 体量
    3. 分钟粒度改完后的 Day1 时间体验边界
- 需要用户现在做什么：
  - live 看 4 件事：
    1. tooltip 是否还会出现内部名
    2. 右侧提示卡三组语义是否对
    3. 右上角 HUD 外观是否过线
    4. 时间是否真的按分钟流逝、`+/-` 是否保留分钟

## 2026-04-14 UI 只读续记｜`+` 快进时间语义彻查
- 当前主线目标：
  - 用户指出 `+` 快进后的分钟语义不对，这轮只读彻查，不改代码。
- 本轮核实结论：
  - 现在不是“显示没跟上”，而是我上轮把真实逻辑改成了“保留分钟”：
    - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 的 `AdvanceOneHour()` 会把 `currentMinute` 原样传进 `SetTime(...)`
    - 所以 `14:37 +` 当前真实结果就是 `15:37`
    - 跨过 `26` 时也会先 `Sleep()`，然后再把分钟补回去，所以会出现 `06:37` 这类状态
  - `SpringDay1Director` 的 Day1 guardrail 也不会帮我们归零分钟：
    - 它按“总分钟”做 clamp，`requestedMinute` 会被完整保留
  - 你说的“先到 :59 再自然 +1 分钟”抓住了跨 2 点边界的核心，但如果直接用现有 `SetTime(xx:59)` 去做，会多发一次 `:59` 的时间事件，这样也不稳
- 我现在的判断：
  - 当前 bug 根因不是 HUD 显示，而是 `TimeManagerDebugger` 的 `+/-` 调试语义被我改错了
  - 最稳修法不是简单继续保留分钟，也不是直接照搬“`SetTime(:59)` 再等自然流逝”
  - 最稳方案应是：
    1. `+` 回到“整点跳转”语义，最终落点必须是 `xx:00`
    2. 普通小时跳转可直接落 `SetTime(targetHour, 0)`
    3. 只有跨 `26 -> Sleep` 这一刀必须走 `Sleep()` 真链，不能伪造第二天整点
    4. 如果坚持做“最后一分钟自然跨整点”，需要补一个不会额外广播 `:59` 事件的内部 helper，不能直接拿现有 `SetTime(:59)` 硬拼
- 下一步只做什么：
  - 下一轮若你批准我修，我会只收 `TimeManagerDebugger` 的 `+/-` 语义，不再碰 tooltip / 提示卡 / HUD 其他内容

## 2026-04-14 UI 只读续记｜Day1 第三点 ownership 边界判断
- 当前主线目标：
  - 用户担心我和 `spring-day1` 线程同时碰“超过凌晨 2:00 稳定回家睡觉”这一条，会把 owner 搞乱；这轮要求先判断第三点到底该谁主刀。
- 本轮核实结论：
  - 当前代码里的职责实际上已经分成两层：
    1. [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs)
       - 负责全局时钟真源
       - `currentHour > dayEndHour(26)` 时触发 `Sleep()`
       - `Sleep()` 只做通用动作：进下一天、时间回到 `06:00`、发 `OnSleep`
       - 它**不负责** Day1 的剧情收束、玩家摆位、NPC 回 anchor、Prompt、相位切换
    2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
       - `OnEnable()` 已订阅 `TimeManager.OnSleep += HandleSleep`
       - `HandleSleep()` 才是真正处理 Day1 特有“稳定回家睡觉”的地方：
         - 校验当前 phase 是否允许进入 DayEnd
         - 非法早睡时走 `RecoverFromInvalidEarlySleep()`
         - 合法时执行 blink / 场景切换 / 玩家放回住处 / 居民与剧情角色 snap home anchor / 进 `DayEnd` / 回满精力
       - `HandleHourChanged()` 里 FreeTime 到 `hour >= 26` 也会主动调用 `TimeManager.Sleep()`，说明 Day1 已经显式接管了“到点该怎么收束”
  - 这说明“超过凌晨 2:00 的玩家稳定回家睡觉”不是单纯 `TimeManager` 问题：
    - `TimeManager` 只该负责“时间越界 -> 触发睡觉事件”
    - Day1 的“怎么睡、睡到哪、谁回家、什么时候算剧情结束”应该由 `SpringDay1Director` 负责
- 我现在的判断：
  - 第三点整体**不该由 UI 线程主刀**
  - 也**不该主要交给我继续碰**
  - 这条应以 `spring-day1` 为唯一主刀，因为它本质上是 Day1 的剧情时间治理和 DayEnd 收束
  - 只有当 `spring-day1` 查明 `TimeManager.Sleep()` 这个全局 contract 本身不够用时，才应最小补 `TimeManager`；但那也应作为 Day1 需要的 runtime 配套，不是 UI own
- 我应该负责什么：
  - 我只继续对自己这条线负责：
    - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 的 `+/-` 调试跳时语义
    - 这属于调试 HUD / 调试输入，不属于 Day1 夜间收束真逻辑
  - 我不应该再继续碰：
    - `SpringDay1Director.HandleSleep()`
    - `RecoverFromInvalidEarlySleep()`
    - `CanFinalizeDayEndFromCurrentState()`
    - FreeTime 警告时点与 DayEnd 收束
- 从现代码反推出的 Day1 时间节点设计：
  - `09:00`：Town opening 最低时间
  - `16:59` 前：教程主段时间上限
  - `18:00`：DinnerConflict 起点
  - `19:00`：Reminder 起点
  - `19:30`：FreeTime 起点
  - `22:00`：夜深提示
  - `24:00`：午夜提示
  - `25:00`：最终催促
  - `> 26:00`：应触发 Sleep / DayEnd 收束
- 下一步只做什么：
  - 对外判断上，我建议：
    1. 第三点整体交给 `spring-day1` 主刀
    2. 我只单独收 `TimeManagerDebugger.cs` 的 `+/-` 语义回归

## 2026-04-14 UI 续记｜给 spring-day1 的 owner 边界告知信
- 当前主线目标：
  - 用户要求我不要再越权碰第三点，只做好自己的 own，并给 `spring-day1` 写一份“这是我的状态说明，不是命令”的告知信。
- 本轮完成：
  - 已新增告知文件：
    - [2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md)
  - 文件内容只做三件事：
    1. 告知第三点整体 owner 属于 `spring-day1`
    2. 告知我不会再碰 `SpringDay1Director` 的睡觉 / DayEnd 逻辑
    3. 告知我会自己收掉 `TimeManagerDebugger +/-` 的调试跳时语义，不再让这条干扰 Day1 live 判断
- 当前阶段：
  - 告知信已落文件，可直接转发
- 下一步只做什么：
  - 等用户转发或继续下令；若后续需要我修 `TimeManagerDebugger +/-`，我再单开一刀，只收那一处

## 2026-04-14 UI 续记｜prompt_12 第一刀施工完成
- 当前主线目标：
  - 按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.2_玩家面集成与性能收口\2026-04-14_UI线程_Day1最终UI语义与时间owner边界收口prompt_12.md`
    只收 Day1 玩家可见 UI contract 与 `TimeManagerDebugger +/-`，不越权碰 Day1 真逻辑。
- 本轮子任务：
  - 修正 `TimeManagerDebugger +/-`
  - 把 `BridgePromptRoot` 收回任务卡语义链
  - 把 workbench 纳入 prompt modal block
  - 给 `spring-day1` 落一份 UI own 状态说明
- 已完成事项：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - `AdvanceOneHour()` / `RewindOneHour()` 恢复整点跳转
    - 跨 `26` 直接走 `Sleep()` 真链，不再补分钟
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `BridgePromptRoot` 改为挂在 `TaskCardRoot` 下方
    - 绑定阶段会校验 bridge prompt parent，不再接受旧漂浮壳
    - bridge prompt 已补进 live binding 与布局刷新链
  - [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs)
    - `IsBlockingPageUiOpen()` 新增 workbench 判定
  - 新增：
    - [2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md)
- 关键决策：
  - Day1 canonical state 仍由 `spring-day1` 提供，UI 只消费，不发明新 runtime 真值
  - `HandleSleep / DayEnd / anchor / resident release` 继续冻结，不由 UI 越权
  - 本轮只 claim“结构与代码层已站住”，不 claim“体验已过线”
- 验证结果：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0 warnings=2`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1UiLayerUtility --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0 warnings=0`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name TimeManagerDebugger --path Assets/YYY_Scripts --level standard --output-limit 5`
    - `errors=0 warnings=1`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs Assets/YYY_Scripts/TimeManagerDebugger.cs --count 20 --output-limit 5`
    - `owned_errors=0 / external_errors=0 / assessment=unity_validation_pending`
    - 当前 blocker 是 Unity 现场 `playmode_transition / stale_status`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs Assets/YYY_Scripts/TimeManagerDebugger.cs`
    - 无空白/补丁格式错误；仅有 CRLF/LF 提示
- 当前恢复点：
  - 如果继续这条线，下一步应是用户 live 测 Day1 任务清单 / bridge prompt / workbench 退让 / `+/-` 跳时，再按截图反馈做第二刀

## 2026-04-14 UI 补记｜workbench 打开即关闭阻塞已解除
- 当前主线目标：
  - 修复 `prompt_12` 第一刀引入的 workbench 自关回归，不扩到其他系统。
- 本轮子任务：
  - 查明是不是我刚加的 workbench modal 判定导致工作台把自己判成 blocker。
- 结论：
  - 是。
  - [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs) 把 workbench 塞进 `IsBlockingPageUiOpen()` 后，
    [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs) 的 `LateUpdate()` 会命中这条判定并立即 `Hide()`。
- 修复：
  - `IsBlockingPageUiOpen()` 回退为只认 `package / box`
  - `ShouldHidePromptOverlayForParentModalUi()` 单独补认 workbench
  - 这样 PromptOverlay 仍对 workbench 退让，但 workbench 本体不再把自己当成外部 modal
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1UiLayerUtility --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 5`
    - `errors=0 warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
    - clean

## 2026-04-14 UI 收尾｜thread-state 已停车
- 本轮 thread-state：
  - `Begin-Slice`：已于本轮开工前完成
  - `Ready-To-Sync`：未跑；因为这轮还停在用户 live 终验前，没有做 sync/commit
  - `Park-Slice`：已跑，当前状态为 `PARKED`
- 当前恢复点：
  - 代码层 checkpoint 已在：
    - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - [SpringDay1UiLayerUtility.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs)
  - 接下来只等用户 live 测：
    - Day1 任务清单 / bridge prompt 主次
    - workbench 打开与退让
    - `+/-` 跳时语义

## 2026-04-14 UI 续记｜prompt12 可见面微调 + day1 转发壳
- 当前主线目标：
  - 用户要求继续收 5 个 UI 细节，并附一段“不要打断 day1 当前进程”的转发壳。
- 本轮子任务：
  - 修右上角调试卡的裁切和居中
  - 修右侧常用操作卡 footer 语义层次
  - 修任务清单下方条子左对齐
  - 缩左下箱子交互卡
  - 准备给 `day1` 的引导壳
- 已完成事项：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 面板尺寸、文本容器、列表行高已调
    - 时间和日期状态改居中
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `BridgePromptOffsetX = 0`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - footer keycap 改成深底浅字、补 outline
    - footer 间距重排
    - `DetailCard / CompactCard` 尺寸下调
- 验证结果：
  - `manage_script validate TimeManagerDebugger` = `errors=0 warnings=1`
  - `manage_script validate SpringDay1PromptOverlay` = `errors=0 warnings=2`
  - `manage_script validate InteractionHintOverlay` = `errors=0 warnings=1`
  - `git diff --check -- Assets/YYY_Scripts/TimeManagerDebugger.cs Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` = clean（仅 CRLF/LF 提示）
- 关键决策：
  - 这一刀只收可见面，不继续扩 modal/runtime 真逻辑
  - `day1` 的详细状态告知正文继续以
    [2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md)
    为真源；聊天里只给转发壳
- 本轮 thread-state：
  - `Begin-Slice`：已重开
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-14 UI 补记｜右侧常用操作卡 footer/tag 二次细调
- 当前主线目标：
  - 继续只收右侧常用操作卡的美观细节，不改别的逻辑。
- 本轮子任务：
  - 拉开 `关闭提示` 与 `BACKSPACE`
  - 整体间距轻微放大
  - `背包/玩法` tag pill 更窄、字更大
- 已完成事项：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `ContextCardRowGap: 3 -> 4`
    - footer 顶部与左右间距继续拉开
    - tag pill 宽度 `40 -> 36`
    - tag 字号 `9 -> 9.75`
    - 标题起点轻微左收，整体更整齐
- 验证结果：
  - `manage_script validate InteractionHintOverlay` = `errors=0 warnings=1`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` = clean
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-14 UI 只读续记｜任务清单完成态与下一任务切换错位
- 当前主线目标：
  - 用户要求我先只读查清：任务完成后为什么没有先完整显示完成态，而是和下一条未完成任务内容发生错位。
- 本轮子任务：
  - 核对 `SpringDay1PromptOverlay` 的状态机构造、过渡协程和 `SpringDay1Director` 提供的 prompt 真值边界，确认问题属于 UI 还是 Day1 真逻辑。
- 已确认事项：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - `LateUpdate()` 会先把 `_pendingState` 直接刷新成最新 `BuildCurrentViewState()` 结果
    - `TransitionToPendingState()` 先对旧 row 播 `AnimateRowCompletion(...)`，但随后仍直接把整张卡切到新的 `targetState`
    - `ApplyStateToPage(...)` 会一次性重写 `StageLabel / Subtitle / FocusText / FooterText / Rows`
    - `PromptCardViewState.FromModel(...)` 当前优先拿“第一个未完成项”当 primary
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - `BuildPromptCardModel()` 当前只输出即时真值，没有“刚完成任务先保留整卡完成态”的过渡层
- 关键判断：
  - 用户体感成立：现在确实会出现“完成框还是旧任务，但正文已经切到下一条”的错位
  - 主因在 UI own 的 `PromptOverlay` 状态机时序，不是先去改 `spring-day1` 真逻辑
- 下一步最稳修法：
  - 只改 `SpringDay1PromptOverlay.cs`
  - 在 `TransitionToPendingState()` 引入 completed snapshot / completion hold
  - 先完整显示旧任务完成态整卡，再切到新的未完成任务态
- 验证状态：
  - 本轮是只读分析，结论属于 `静态推断成立`
  - 尚未开始代码施工，也未跑新的 `Begin-Slice`
- 当前恢复点：
  - 如果用户下一轮批准开修，直接从 `SpringDay1PromptOverlay.cs` 的 completed snapshot 切口进入，不再重复查 owner 边界

## 2026-04-14 UI 续记｜任务清单 completed snapshot 修复已提交
- 当前主线目标：
  - 在不碰 `SpringDay1Director` 真逻辑的前提下，修复任务清单“先显示完成态，再切下一条未完成任务”的展示时序，并确保这轮先有可回退 checkpoint。
- 本轮子任务：
  - 先把已有 UI 四文件状态固化成可回退基线
  - 再只改 `SpringDay1PromptOverlay.cs` 一个点完成 completed snapshot 修复
- 已完成事项：
  - 本地 checkpoint 提交：
    - `33985c17` `checkpoint: save UI prompt baseline before completion snapshot fix`
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 `PromptRowState.Clone()` 与 `PromptCardViewState.Clone()`
    - 新增 `PromptCardViewState.RefreshSignatures()`
    - 新增 `BuildCompletedHoldState(...)`
    - 在 `TransitionToPendingState()` 中先应用旧卡 completed snapshot，再把控制权交给新的 `targetState`
  - 修复提交：
    - `f172ec62` `fix: keep completed prompt card state before advancing`
- 验证结果：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs Assets/YYY_Scripts/TimeManagerDebugger.cs --count 20 --output-limit 5`
    - `assessment=no_red owned_errors=0 external_errors=0`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level standard --output-limit 10`
    - `errors=0 warnings=2`
    - 仅既有性能 warning
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - `0 error / 0 warning`
- 关键判断：
  - 这次修法把“完成框”和“正文”重新收回同一个 UI 状态
  - 根因仍属于 `PromptOverlay` 展示层，不需要把 Day1 真逻辑一起卷进来
- 当前阶段：
  - 代码已提交，等待用户 live 看体感
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：尝试过，但被 own roots 历史脏改阻断；未走白名单 sync
  - `Park-Slice`：已跑，当前 `PARKED`
- 当前恢复点：
  - 如果用户 live 反馈还不够顺，下一轮继续只改 `SpringDay1PromptOverlay.cs` 的 completed snapshot 持续时长 / 触发矩阵，不扩回 Day1 runtime 真值

## 2026-04-14 UI 续记｜右上角 / footer 微调 + 任务清单二次根因复盘
- 当前主线目标：
  - 用户要求本轮直接解决两个 UI 排版点，并重新厘清为什么任务清单仍然“绿一下就换字、动画和内容不同步”。
- 本轮子任务：
  - 微调 `TimeManagerDebugger` 面板宽度
  - 微调 `InteractionHintOverlay` footer 行距与 key/text 关系
  - 重新核查 0.0.5 农田教学的任务清单状态竞争
- 已完成事项：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - `panelWidth: 272 -> 248`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `footerY` 下移
    - `footerGap: 20 -> 12`
    - `footerLabelWidth: 78 -> 74`
- 二次复盘结论：
  - 用户这次指出的问题成立，而且比我上次总结得更完整。
  - 现在剩下的任务清单 bug 不止是“completed snapshot 持续时间短”，而是至少有 3 层：
    1. 主任务卡 completed snapshot 和 bridge/manual prompt 不是同一笔 transaction
    2. `SpringDay1Director` 在 0.0.5 农田教学推进时会立刻 `SpringDay1PromptOverlay.Instance.Show(...)` 下一步文案，bridge prompt 会先刷新
    3. 被 dialogue/modal 压住时，当前实现不会积累中间 completion 队列，恢复显示时只认最新态
  - 这解释了为什么用户会看到：
    - 左侧完成框绿一下
    - 但文字已经是下一步
    - 并且当任务清单中途被压住时，恢复后看不到应有的完成过渡
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs --count 20 --output-limit 5`
    - `owned_errors=0 / external_errors=0 / assessment=unity_validation_pending`
    - blocker 是 Unity 现场 `stale_status`
  - `manage_script validate InteractionHintOverlay` = `errors=0 warnings=1`
  - `manage_script validate TimeManagerDebugger` = `errors=0 warnings=1`
  - `errors --count 20 --output-limit 10`
    - fresh console 有 2 条外部 `EventSystem` 红，不归这轮 own
- 关键判断：
  - 下一刀如果继续修任务清单，不能再只盯主卡 row 动画，必须把 `PromptCard + BridgePrompt + hidden/re-entry` 一起收成同一个 completion transaction
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-17 UI 续记｜工作台木稿配方改成纯木头
- 当前主线：
  - 用户临时插入一个工作台小切片，要求把木稿子配方改成和木斧一样，只要木头。
- 本轮子任务：
  - 只改工作台配方资产，不碰当前已脏的 crafting 代码与 UI 脚本。
- 已完成事项：
  - [Recipe_9102_Pickaxe_0.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset)
    - 删除石头材料 `itemID: 3201 amount: 2`
    - 保留木头材料 `itemID: 3200 amount: 3`
    - 描述同步改成“用粗木料拼出的入门木稿...”
- 对照基线：
  - [Recipe_9100_Axe_0.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset) 目前就是 `3200 x3`，木稿现已和它保持同一基础门槛。
- 验证：
  - `git diff --check -- Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset` 通过
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 5` = `0 error / 0 warning`
- 本轮 thread-state：
  - `UI` 切片 `Workbench wood pickaxe recipe reduce to wood only`
  - 已 `Park-Slice`，当前 `PARKED`
- 恢复点：
  - 如果用户接着要调别的工作台配方，继续按“只改对应 recipe asset、避免卷入其他 dirty 脚本”的最小方案推进。

## 2026-04-17 UI 续记｜右上角时间/调试卡片只做显示优化
- 当前主线：
  - 用户贴图指出右上角 `TimeManagerDebugger` 卡片不好看，要求只优化显示，不动任何时间与快捷键逻辑。
- 偏好前置结论：
  - 这轮属于 `targeted probe / 局部验证`，证据来自用户截图与最新 live 反馈。
  - 高危反例仍是“只会显示但像测试味”，所以这轮优先收窄宽度、减少空旷、修顺标签和键帽关系，不加新装饰。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 整体面板宽度 `272 -> 224`
    - 时钟卡高度 `68 -> 64`
    - 快捷键卡高度 `134 -> 126`
    - 卡片间距 `8 -> 6`
    - `时间 / 调试` 标签 pill 缩短并左收
    - `快捷键` 标题左收，减少标题右侧空白
    - 每行 key pill 缩窄，说明文字起点前移，减少右侧大空块
- 验证：
  - `git diff --check -- Assets/YYY_Scripts/TimeManagerDebugger.cs` 通过
  - `manage_script validate TimeManagerDebugger` = `errors=0 warnings=1`
  - warning 仍是既有 `Update()` 字符串拼接提示，不是本轮新红
  - `errors --count 20 --output-limit 5` = `0 error / 0 warning`
- 本轮 thread-state：
  - `Begin-Slice` 已跑
  - `Park-Slice` 已跑，当前 `PARKED`
- 恢复点：
  - 如果用户 live 看后还嫌空或挤，下一轮继续只在 `TimeManagerDebugger.cs` 里微调几何参数，不扩到逻辑和别的面板。

## 2026-04-14 UI 续记｜completion transaction 收口 + Package/Prompt sibling 抢位止血
- 当前主线目标：
  - 在不越权碰 Day1 真逻辑的前提下，彻底收掉任务清单“主卡、bridge prompt、hidden/re-entry 各走各的”，并判断 `PackagePanel` 与 `SpringDay1PromptOverlay` 层级互跳是否可安全修复。
- 本轮子任务：
  - 只改 `SpringDay1PromptOverlay.cs`
  - 顺手保留本轮已做的 `TimeManagerDebugger` / `InteractionHintOverlay` 微调
- 已完成事项：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
    - 新增 `_pendingBridgePromptState`
    - 新增 `_suspendBridgePromptSync`
    - `LateUpdate()` 改成即使当前被 modal/dialogue 压住，也会先更新 pending card / pending bridge 状态
    - `TransitionToPendingState()` 改成在主任务卡切换期间先冻结 bridge prompt，待主卡 transition 完成后再统一应用最新 bridge prompt
    - `SetExternalVisibilityBlock(false)` 改成需要时直接起 transition，不再 `ApplyState(_pendingState)` 硬跳
    - `OnDialogueEnd()` 改成需要时也会接回 transition
    - `EnsureHudSiblingOrder()` 在 modal 打开期间直接停手，止住 `PackagePanel` 与 `PromptOverlay` 的 hierarchy 互跳
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - `panelWidth: 272 -> 248`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `footerY` 下移
    - `footerGap: 20 -> 12`
    - `footerLabelWidth: 78 -> 74`
- 验证结果：
  - `manage_script validate SpringDay1PromptOverlay` = `errors=0 warnings=2`
  - `errors --count 20 --output-limit 10` = `0 error / 0 warning`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs --count 20 --output-limit 5`
    - `owned_errors=0 / external_errors=0`
    - `assessment=unity_validation_pending`
    - blocker 是 Unity 当前 `playmode_transition`
    - external warnings 是字体省略号 fallback，不归本轮 own
- 关键判断：
  - 这轮已经把任务清单 remaining 问题从“主卡局部动画”升级成“统一 completion transaction”并实际收口
  - `PackagePanel` / `PromptOverlay` 互跳根因确认是 `PromptOverlay` 每帧 sibling 纠正；这轮在 `PromptOverlay` 内部止血是安全的，不需要碰 `PackagePanel` 逻辑
- 当前阶段：
  - 代码层已落，等待用户 live 验主观体感与 hierarchy 稳定性
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-17 UI 只读续记｜工作台材料预扣逻辑彻查
- 当前主线：
  - 用户要求彻查“点击制作就应立即扣材料，追加也一样，中断再返还”的真实代码链，并给出最小、最安全、最可靠的补丁方案。
- 本轮结论：
  - 现状并不满足“点击即扣料”。
  - 当前第一件在 SpringDay1WorkbenchCraftingOverlay.cs 的 CraftRoutine() 里才通过 CraftingService.TryCraft(..., false) 预扣一件材料；但“追加 / 加入队列”只是在 OnCraftButtonClicked() 里增加 entry.totalCount，并没有立即扣料。
  - 这会带来 4 个真实漏洞：
    1. 同配方多件只预扣当前正在做的 1 件，其余待做件数仍占着背包可用材料。
    2. 追加和非活动 recipe 排队不预扣，导致不同 recipe 之间可发生材料超订。
    3. 排队期间玩家还能把这些材料拿去做别的事，等轮到该队列才在 CraftRoutine() 里失败并把 totalCount 砍回 readyCount。
    4. 当前 CancelQueuedRecipeEntry() 不退款，因为旧模型默认“队列没预扣”；若改成点击即预扣，这里必须一起补退款。
- 关键风险判断：
  - 不能直接把“点击即扣料”粗暴改成复用 CraftingService.TryCraft(recipe, false) 多次调用。
  - 原因是 TryCraft() 会触发 OnCraftSuccess；而 SpringDay1Director.cs 现在用这个事件累计 _craftedCount。
  - 如果在点击时就提前触发 success，Day1 的“完成一次真正制作”会被提前甚至按多件一次性计数，属于高风险误伤。
- 推荐最小补丁边界：
  - 只动：
    - CraftingService.cs
    - SpringDay1WorkbenchCraftingOverlay.cs
  - 可选补 1 个窄测试文件，不碰 SpringDay1Director、InventoryService、PersistentPlayerSceneBridge、SaveManager。
- 推荐补丁方案：
  - 在 CraftingService 新增“只预扣材料、不触发 OnCraftSuccess、不入包”的独立入口，按 craftCount 一次性校验并扣料。
  - 保留现有 TryCraft() 语义不变，避免影响旧 CraftingPanel 和其他调用方。
  - WorkbenchOverlay.OnCraftButtonClicked() 改成：开始制作 / 加入队列 / 追加时，先调用新预扣入口；预扣成功后再增加对应 entry.totalCount。
  - CraftRoutine() 改成只负责计时、进度、readyCount++，不再在每一件开始时扣料。
  - 真正一件完成时，再显式走一次“制作成功事件”通知，让 Day1 的 _craftedCount 保持在“完成一件时+1”。
  - CancelActiveCraftQueue() 改成退回“该 recipe 所有未完成件数”的材料，保留已完成 readyCount 不动。
  - CancelQueuedRecipeEntry() 同样退回“该 entry 的未完成件数”材料。
  - GetSelectableQuantityCap() 去掉旧的 queuedAlready 扣减，因为新模型下材料已从背包移除，再减一次会双重扣减。
- 当前阶段：
  - 只读根因和补丁边界已经厘清，尚未动手写代码。
- 恢复点：
  - 若用户批准施工，下一轮直接按上面两文件一测试的窄切片开改，不扩到别的 UI 或剧情链。

## 2026-04-17 UI 续记｜工作台点击即扣料 + 中断退款 + 跨场景回切刷新
- 当前主线：
  - 把工作台制作改成“点击即交料给工作台，追加同样即时扣料，中断/取消按未完成件数退款”，并确认跨场景后仍能按当前场景事实源正确刷新。
- 本轮完成：
  - [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs)
    - 新增 `CanReserveMaterials(...)`
    - 新增 `TryReserveMaterials(...)`，只预扣材料，不触发 `OnCraftSuccess`
    - 新增 `NotifyCraftCompleted(...)`，把“真正完成一件”与“点击时预扣”拆开
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - `OnCraftButtonClicked()` 现在先 `TryReserveMaterials`，成功后才开始制作 / 加入队列 / 追加制作
    - `CraftRoutine(...)` 改成吃 `initialCraftCount`，只跑进度与完成，不再在协程里临时扣料
    - 每完成一件时调用 `NotifyCraftCompleted(recipe)`，确保 Day1 的 `_craftedCount` 仍在“完成时”累计
    - `CancelActiveCraftQueue()` / `CancelQueuedRecipeEntry()` 改成按未完成件数统一退款
    - `StopCraftRoutine()` 改成硬停时退款全部未完成预扣材料，避免异常停机吃料
    - `GetSelectableQuantityCap()` 去掉旧的 `queuedAlready` 二次扣减，避免预扣后又被错误再减一次
    - `OnSceneLoaded()` / `OnActiveSceneChanged()` 回到绑定场景时补 `RefreshBoundSceneRuntimeState()`，重绑当前场景背包事实源并刷新工作台状态
  - [WorkbenchInventoryRefreshContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs)
    - 新增预扣/完成分离、取消退款、场景回切刷新相关 contract 断言
- 关键判断：
  - 这次没有去改旧 `TryCraft()` 合同，因此没有把旧 CraftingPanel 和 Day1 success 计数链打乱；补丁边界保持在 `CraftingService + WorkbenchOverlay + 窄测试`。
- 验证：
  - `manage_script validate CraftingService`：`errors=0 warnings=0`
  - `manage_script validate SpringDay1WorkbenchCraftingOverlay`：`errors=0 warnings=1`（既有字符串拼接 warning）
  - `manage_script validate WorkbenchInventoryRefreshContractTests`：`errors=0 warnings=0`
  - `validate_script CraftingService + WorkbenchOverlay + Test`：`owned_errors=0 / external_errors=0 / assessment=unity_validation_pending`，卡在 Unity `stale_status`
  - `run_tests(EditMode, WorkbenchInventoryRefreshContractTests)`：`7 passed / 0 failed`
  - fresh console：`0 error / 0 warning`（仅测试结果路径提示）
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`
- 恢复点：
  - 下一轮如果用户 live 反馈还有漏洞，只继续补工作台 own 逻辑，不扩到存档 / Director / 切场大面。

## 2026-04-17 UI 续记｜工作台红字语义保持，stale contract 收尾
- 当前主线：
  - 在不改“已交材料后红字=不能再追加”的玩家语义前提下，把这轮自查里确认的测试尾账收平。
- 本轮子任务：
  - 修 `SpringDay1DialogueProgressionTests.cs` 里两条已经落后于真实实现的 stale contract：
    1. `Pickaxe_0` 仍要求旧石料需求
    2. 工作台提示记忆仍盯 `PlayerPrefs.SetInt`
- 已完成：
  - 木稿断言改成 `itemID: 3200`，并显式禁止 `itemID: 3201`
  - 工作台提示记忆断言改成 `StoryProgressPersistenceService.IsWorkbenchHintConsumed()/MarkWorkbenchHintConsumed()`
- 验证：
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：`0 error / 0 warning`
  - `validate_script Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`：`0 error / 0 warning`
  - `WorkbenchInventoryRefreshContractTests`：`7 passed / 0 failed`
  - fresh console 清空后只有 Unity MCP `TestJobManager failed to initialize within timeout` warning；没有新的 own error
- 当前阶段：
  - 代码尾账已落地，停在可交代状态；未做 sync / commit。
- 阻塞与恢复点：
  - `SpringDay1DialogueProgressionTests.WorkbenchInteraction_ContainsRuntimeBindingBridge` 通过 Unity MCP 单测连续两次初始化超时，没拿到通过/失败断言结果；如果下轮继续，要换验证路径，不要再把这条当“已测过”。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-18 UI 续记｜关系页 chip 视觉误判复核
- 当前主线：
  - 用户在关系页截图里质疑 `卡尔` 及其以下几行右侧标签框被区别对待；本轮要求重新看图并给出准确判断。
- 本轮子任务：
  - 只读核实 `PackageNpcRelationshipPanel.cs` 中右侧 chip 的真实尺寸、布局来源、颜色与文案映射，不改代码。
- 已确认：
  - 我上一轮把主因归到“选中态”上，是看偏了。
  - 当前更准确的真因不是单 NPC 特判，而是 `未露面 + 陌生` 这个状态组合本身更重：
    - 宽高常量仍统一：`72 x 24`
    - `PresenceLevel.None` 的文案是 `未露面`
    - `Stage.Stranger` 的文案是 `陌生`
    - 两枚 chip 目前颜色完全相同，导致右侧读成连续重块
- 用户可感知结果：
  - 从 `卡尔` 往下这批人看起来像“右侧标签更大更重”，不是因为尺寸真被做大，而是因为两块同色+更满的字面把视觉重量堆出来了。
- 恢复点：
  - 如果下一轮进入真实施工，应该改的是 `未露面 / 陌生` 这组状态的视觉分离，而不是去找 `003/卡尔` 的单独特殊分支。

## 2026-04-18 UI 续记｜关系页 chip 收口与 TimeDebugger 快捷键列优化
- 当前主线：
  - 直接把关系页右侧 chip “更长更重”的玩家面问题修掉，并优化 `TimeManagerDebugger` 右上角快捷键卡片的按钮列对齐。
- 本轮子任务：
  - 修改 [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs) 与 [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)。
- 已完成：
  - 关系页：
    - `chipColumnLayout.childForceExpandWidth = false`
    - `chipColumnLayout.childAlignment = UpperRight`
    - presence chip / stage chip 分别改成 `68 / 64` 宽
    - `未露面` 与 `陌生` 的底色拆开，避免继续连成同色重块
  - 时间调试卡：
    - 标题与说明列统一到固定起点
    - key pill 改为固定列宽
    - 行顶点与列间距重新收平
- 验证：
  - `manage_script validate PackageNpcRelationshipPanel --status clean`
  - `validate_script TimeManagerDebugger --assessment no_red`
  - `sunset_mcp.py errors --count 20 --output-limit 5`：`0 error / 0 warning`
- 当前阶段：
  - 代码层 clean，等待用户 live 看观感。
- 风险与说明：
  - 关系页文件目前仍是 `??` untracked 状态的一部分，我没有顺手处理别的 UI 文件；只改了你这轮点名的两处。
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-18 UI 续记｜设置页右侧重复说明卡删除
- 当前主线：
  - 用户要求把存档设置页右侧那块与左侧副说明重复、且已经发生重叠的 `F5/F9` 介绍直接删掉，不要再做位置微调。
- 本轮子任务：
  - 只修改 [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)，移除右侧冗余 `GuideCard`。
- 已完成：
  - 删除运行时构建的 `GuideCard / GuideTitle / GuideShortcut / GuideBody`
  - 删除 `_guideShortcutText`、`_helpText` 字段与 `RefreshView()` 中对应赋值
  - 右侧仅保留 `重新开始` 与 `退出游戏` 两个操作按钮
- 验证：
  - `validate_script Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`：`0 error / 0 warning`
  - `find_in_file HelpFontSize|GuideTint|_guideShortcutText|_helpText`：`0 match`
  - fresh console：无新增 own error / warning
- 当前阶段：
  - 结构修补已完成，等待用户 live 看设置页重叠是否消失。
- 风险与说明：
  - 这轮不宣称整页观感完全过线；只确认“右侧重复说明已删除，逻辑未扩改”。
- 本轮 thread-state：
  - 进入这轮时 `UI` slice 已处于 `ACTIVE(settings-save-help-overlap-fix)`，因此未重复开新 slice
  - 收尾已跑 `Park-Slice`，当前 `PARKED`

## 2026-04-23 UI 续记｜shared-root own 上传结果
- 当前主线：
  - 只做 UI 当前本地 own 成果的保本上传，不继续补玩家面体验。
- 本轮分批结果：
  - 批次 1：docs / specs / prompt
    - 已通过 `Ready-To-Sync`
    - 已 `sync + push`
    - commit = `ea6ac827338bb1a382e5ef1b41ac49cc3b392353`
  - 批次 2：字体材质
    - 直接 `sync + push`
    - commit = `edd3baea26acf78fd4327a16cfe0e81656e83cc3`
  - 批次 3：UI 代码
    - 未完成上传
- 当前 blocker：
  - `CodexCodeGuard` 在 UI 代码批 `pre-sync` 时报：
    - `FATAL: CodexCodeGuard 未返回 JSON 结果`
  - 直跑 `CodexCodeGuard.dll` 到单文件也会 hang / exit 1，无稳定 JSON 输出
  - 因此我没有继续硬吞：
    - `Assets/YYY_Scripts/UI/*`
    - `Assets/YYY_Scripts/Story/UI/*`
- 当前阶段：
  - 可安全归仓的非代码 own 已全部上到 `origin`
  - 剩余 own 代码明确停在 codeguard blocker，等待下一轮单独处理
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：docs 批通过；代码批被 codeguard 卡住
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-23 UI 续记｜历史小批次 Tabs 新 panel/runtime-kit 上传尝试
- 当前主线：
  - 遵循第二波治理 prompt，只还原 1 个历史小批次上传；如果撞 blocker 就停车，不换第二批。
- 本轮子任务：
  - 只尝试上传：
    - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
    - `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta`
    - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
    - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta`
    - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs`
    - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta`
- 历史判断：
  - 这 3 个新增文件都属于 `Tabs` runtime panel/helper 同一簇创建批，创建时间同在 `2026-04-08`。
  - 但它们不是“可单独 sync 的纯净历史批”，因为 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 已作为同根接线文件存在未纳入本轮的脏改。
- 本轮结果：
  - `Begin-Slice` 已跑
  - `Ready-To-Sync` 已跑
  - 未提交、未 push
- blocker：
  - 第一真实 blocker = same-root mixed / own-root remaining dirty
  - 不是 `CodexCodeGuard`
  - preflight 明确指出：
    - `own_roots=[Assets/YYY_Scripts/UI/Tabs]`
    - `remaining= M Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
- 恢复点：
  - 这组新 panel/runtime-kit 小批次已经完成“真实尝试 -> 精确报死”
  - 其它 UI 代码尾账先不动
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，结果 blocker
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-23 UI 续记｜重复收到同一小批次 prompt 的处理
- 用户再次发送同一份 `2026-04-23_给UI_shared-root历史小批次上传prompt_02.md`。
- 我重新读取 prompt，并复核：
  - [UI.json](D:/Unity/Unity_learning/Sunset/.kiro/state/active-threads/UI.json)
  - 当前 Tabs 白名单文件状态
- 判断：
  - 该小批次已经执行过真实尝试
  - 当前 blocker 没变，仍是 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 同根残留
  - 因此本轮不应再对同一批次重复发起第二次尝试
- 当前恢复点：
  - 继续维持 `PARKED`
  - 等待下一轮用户/治理对 `PackagePanelTabsUI.cs` 是否并入同批做裁定

## 2026-04-24 UI 续记｜Tabs 根内整合批上传尝试
- 当前主线：
  - 遵循 `prompt_03`，不再重跑上一轮 `prompt_02`；只把 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 正式并入 Tabs/runtime-kit 同批核心件，再做一次新的真实上传尝试。
- 本轮子任务：
  - 目标白名单固定为：
    - `PackagePanelTabsUI.cs`
    - `PackageMapOverviewPanel.cs`
    - `PackageMapOverviewPanel.cs.meta`
    - `PackageNpcRelationshipPanel.cs`
    - `PackageNpcRelationshipPanel.cs.meta`
    - `PackagePanelRuntimeUiKit.cs`
    - `PackagePanelRuntimeUiKit.cs.meta`
- 核心判断：
  - 现已正式承认 `PackagePanelTabsUI.cs` 属于这批核心件。
  - 证据是它在 `54/55/68/69/216/217` 行直接 `EnsureOptionalPanelInstalled(...)` 这两个新 panel。
- 本轮结果：
  - `Begin-Slice` 已跑
  - 根内只剩这 7 个文件，没有新的 `UI/Tabs` 同根残留
  - 但新的第一真实 blocker 已经不是 same-root mixed，而是 `CodexCodeGuard`
  - 3 个文件 `validate_script` 直接报 `CodexCodeGuard returned no JSON`
  - `PackagePanelTabsUI.cs` 只拿到 `unity_validation_pending / baseline_fail`
  - 因此本轮没有继续到 `Ready-To-Sync / sync`
  - 没有提交、没有 push
- 边界：
  - 本轮没有越权扩到 `Inventory / Toolbar / Box / Story/UI`
- 当前恢复点：
  - 这轮整合批已经完成“真实尝试 -> 新 blocker 报死”
  - 当前线程重新回到 `PARKED`

## 2026-04-26 UI 续记｜工具修复后 UI/Tabs 七文件最小复核
- 当前主线：
  - 只做工具修复后的 `UI/Tabs` 七文件最小复核，确认这条线现在吐出的是“真实 blocker / 或真实可上传”，不再是旧的 CodeGuard 黑盒。
- 本轮子任务：
  - 白名单仍固定为：
    - `PackagePanelTabsUI.cs`
    - `PackageMapOverviewPanel.cs`
    - `PackageMapOverviewPanel.cs.meta`
    - `PackageNpcRelationshipPanel.cs`
    - `PackageNpcRelationshipPanel.cs.meta`
    - `PackagePanelRuntimeUiKit.cs`
    - `PackagePanelRuntimeUiKit.cs.meta`
- 本轮实际做成：
  - 新起 `Begin-Slice(ui-tabs-post-toolfix-min-recheck-20260426)`
  - 顺序跑过 `Ready-To-Sync`
  - 得到真实代码闸门结果
  - 没有修业务代码
- 关键结论：
  - 这轮已经不再出现：
    - `CodexCodeGuard returned no JSON`
    - `baseline_fail 黑盒`
  - 当前新的第一真实 blocker 是 `PackagePanelTabsUI.cs` 的 `3 error + 1 warning`
- 具体诊断：
  - `CS0103` `PackagePanelTabsUI.cs:53`
  - `CS0103` `PackagePanelTabsUI.cs:67`
  - `CS0103` `PackagePanelTabsUI.cs:215`
  - 以上都是：当前上下文中不存在名称 `PackageSaveSettingsPanel`
  - `CS0649` `PackagePanelTabsUI.cs:20`
  - 内容：字段 `boxUIRoot` 从未赋值
- 当前判断：
  - 这组现在是“真实业务 blocker”
  - 不是工具 incident
  - 也还不是“可继续上传”
- 边界：
  - 本轮未修业务代码
  - 未扩到 `Inventory / Toolbar / Box / Story/UI`
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，得到真实 blocker
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-26 UI 续记｜UI-Tabs 与 UI-Save 忽略脚本只读排查
- 用户目标：
  - 只读确认 `UI/Tabs` 编译错误是否主要由被 `.gitignore` 忽略的 `UI/Save` 正式脚本导致，并列出关键引用链。
- 当前主线：
  - 继续 UI 线的只读审计，收紧 `PackagePanelTabsUI.cs` 的缺符号根因，不进入真实施工。
- 本轮子任务：
  - 检查 `Assets/YYY_Scripts/UI/Save/` 是否存在相关 `.cs/.meta`
  - 判断这些文件是否像正式业务代码
  - 统计谁直接引用了 `PackageSaveSettingsPanel`
  - 评估若把相关 `UI/Save` 文件纳入 git，理论上能否解开当前 `CS0103/CS0246`
- 本轮实际做成：
  - 确认 `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`、`PackageSaveSettingsPanel.cs.meta`、`SaveActionToastOverlay.cs`、`SaveActionToastOverlay.cs.meta` 实际存在于工作树
  - 确认它们都被仓库 `.gitignore` 第 36 行 `Save/` 命中，且当前不在 git 跟踪内
  - 确认 `PackageSaveSettingsPanel.cs` 是完整正式脚本：定义 `PackageSaveSettingsPanel : MonoBehaviour`，并提供 `EnsureInstalled(GameObject panelRoot)`
  - 确认 `SaveActionToastOverlay.cs` 是正式运行时提示层脚本，但当前主要通过 `SaveManager` 反射调用，不是 `UI/Tabs` 缺符号的直接编译前提
  - 确认直接静态/强类型引用 `PackageSaveSettingsPanel` 的文件有：
    - `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
    - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- 关键判断：
  - 对 `PackagePanelTabsUI.cs` 这组 `CS0103` 来说，`UI/Save` 被忽略且未进仓，确实是主要成因。
  - 若其他环境缺少 `PackageSaveSettingsPanel.cs/.meta`，则上述 3 处强类型引用都会理论上落成 `CS0103/CS0246`。
  - 仅从编译依赖看，真正必须纳管的是：
    - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
    - `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs.meta`
  - `SaveActionToastOverlay.cs/.meta` 更偏运行时 toast 功能完整性；因为 `SaveManager` 通过字符串反射取类型，它不是当前 `PackagePanelTabsUI` 缺符号的必要前提。
- 补充证据：
  - 当前本机最新 `Editor.log` 里可见这两个 `UI/Save` 脚本已进入 Unity 编译/导入输入，说明本机工作树具备这些文件。
  - 同一份最新 `Editor.log` 里当前显式可见的最新 `CS0246` 是 `WorkbenchRuntimeSaveData`，不是 `PackageSaveSettingsPanel`；因此“被忽略的 `UI/Save` 正式脚本会导致这类错误”成立，但“当前这台机器最新红错主要就是它”这件事，日志证据不成立。
- 本轮验证状态：
  - `静态推断成立`
  - 未做任何业务代码修改
  - 未进入 `Begin-Slice`
