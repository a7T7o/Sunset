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
