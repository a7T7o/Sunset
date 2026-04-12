# UI系统 - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_1.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/memory_1.md)。本卷只保留 UI 主线分流与恢复点。

## 当前定位
- 本工作区只保留 Sunset UI 主线总览与分流入口。
- 具体玩家面集成、workbench、prompt、proximity、打包字体链与 live 体验问题，不再继续堆回根卷。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `0.0.2_玩家面集成与性能收口`

## 阶段索引
- `0.0.1 SpringUI`：已冻结，只保留早期 prefab-first 与基线方案
- `0.0.2_玩家面集成与性能收口`：当前唯一活跃阶段

## 当前稳定结论
- UI 当前最值钱的活线不是“做新壳”，而是：
  - 玩家面集成
  - workbench / prompt / proximity 刷新风暴止血
  - task-list / formal dialogue / modal 之间的玩家面治理
  - packaged / live 最终证据补齐

## 当前恢复点
- 后续 UI 新进展只写到：
  - [0.0.2_玩家面集成与性能收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/memory.md)
- 查旧 SpringUI 方案和早期 prefab-first 路线时，再看：
  - [0.0.1 SpringUI/memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.1%20SpringUI/memory_0.md)

## 2026-04-10 只读补记｜PromptOverlay 不是 Toolbar 同类 HUD
- 当前最重要的新结论：
  - 任务清单现在反复出不同 bug，不是因为单次尺寸又算错了，而是因为 `SpringDay1PromptOverlay` 仍是“半独立 overlay 壳”，没有收成和 `toolbar / state` 同类的固定 HUD 治理
- 当前阶段判断：
  - 这件事应该按“治理模型问题”处理，不该继续按“再补一个隐藏补丁 / 再调一次 alpha”处理
- 体验语义对齐：
  - 背包、箱子、workbench page 打开时，prompt 应只是处在下层，被 page 正常盖住
  - formal dialogue 才是需要统一淡出非对话 HUD 的高优先场景
- 额外结论：
  - 右上角调试字跨分辨率不一致，根因在 `TimeManagerDebugger.OnGUI()` 的 IMGUI 固定像素方案，不在 Spring HUD 本体

## 2026-04-10 实装补记｜当前 UI 主线最新落点
- UI 线程这轮已真正进入真实施工并完成一刀收口：
  - `005` 放置模式状态提示已在 `InteractionHintOverlay` 这条底部 HUD lane 上落地
  - 任务清单已切掉“跟随父 Canvas 排序漂移”的核心根因，收回固定 HUD 排序
  - 右上角调试字已接入参考分辨率缩放
- 当前恢复点：
  - 若用户下轮反馈是 placement status 的 live 细节，再回 `InteractionHintOverlay`
  - 若用户下轮反馈是 task list / package / dialogue 的层级体验，再回 `SpringDay1PromptOverlay`
  - 若用户下轮反馈是不同分辨率下的右上角字，再回 `TimeManagerDebugger`

## 2026-04-10 只读补记｜UI 性能风险图
- 当前 UI 新增一个明确结论：
  - `Workbench` 卡顿已经不是“用户感觉卡”，而是代码层可以直接读出的结构性布局风暴
- 当前最危险的 confirmed hotspot：
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
  - 它把：
    - 大量 runtime `AddComponent`
    - `LayoutGroup + ContentSizeFitter`
    - `ForceRebuildLayoutImmediate`
    - `Canvas.ForceUpdateCanvases`
    - `ForceMeshUpdate`
    - 行项 unreadable/hard-recovery 时的销毁重建
    全都叠进了正常刷新路径
- 当前结构性高危但不一定是当前第一热点的文件：
  - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
  - [PackageMapOverviewPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
  - [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
- 当前另一类独立风险：
  - 打开 UI / 跨场景重绑时的全树扫描与强制刷新
  - 代表文件：
    - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
- 当前恢复点：
  - 后续若继续做 UI 性能，不应再泛说“继续优化”
  - 应按 3 类分别下刀：
    1. 热路径布局风暴
    2. 热路径字体/文本自愈
    3. 打开 UI / rebind 时的全树扫描

## 2026-04-10 UI 续记｜状态提示卡排版与 toolbar 病因
- `状态 UI`
  - 已对 `InteractionHintOverlay` 的放置模式状态卡做第二轮尺寸补口
  - 目标是解决用户反馈的标题/正文重叠，而不是改别的 HUD 语义
- `toolbar / 工具链`
  - 已确认这不是“滚轮链也死了”的全局坏死
  - 更接近：
    - `滚轮` 仍走 selection/equip 主链
    - `点击` 前面多了 restore / reject / panel-open / lock 的分叉，导致视觉切换和真实换手持脱钩
  - 这轮按用户要求只做病因定位，未开始修输入链

## 2026-04-10 UI 续记｜状态标签遮挡修正
- 追加确认一个更细的 UI 根因：
  - 不是字体本身糊，而是 `StatusTag` 和 `StatusTitleText` 本来就在同一块左上区域
- 已改成：
  - `StatusTag` = 左上角标
  - `StatusTitleText` = 右侧标题行
- 这轮仍只收 `InteractionHintOverlay`，没有扩回 toolbar / prompt / workbench

## 2026-04-10 UI 审计｜任务清单打包异常 + 状态条 16:10 溢出 + 手持链漏洞
- 本轮没有继续写业务代码，先做了一轮只读厘清，并补了一条给 `spring-day1` 的求助 prompt。
- 当前确认的 4 个独立问题：
  1. `任务清单`
     - packaged/live 仍未真正收平
     - 代码上仍是独立 `ScreenSpaceOverlay + overrideSorting` 的 `PromptOverlay`
     - 不是和 `toolbar/state` 一样走统一 HUD canvas + 缩放 + 遮挡治理
     - `SetExternalVisibilityBlock()` 存在，但整体 modal/page 接管链仍未站稳
  2. `状态条 16:10 溢出`
     - `SpringDay1StatusOverlay` 仍是纯固定像素：
       - root `anchoredPosition = (-24,-26)`
       - root `sizeDelta = (236,118)`
       - card `sizeDelta = (228,48)`
     - 没有 `CanvasScaler` / 参考分辨率缩放
     - 所以 16:9 正常更像是“刚好没炸”，不是结构正确
  3. `状态提示卡重叠`
     - `InteractionHintOverlay` 同样没有 `CanvasScaler`
     - `StatusTag / StatusTitle / StatusDetail` 仍靠手写像素区间切位
     - 当前属于“局部补口有效，但根上还是硬编码几何”
  4. `手持工具链 / 放置模式 / toolbar`
     - 点击链与滚轮链不一致
     - 世界左键链优先看 `PlacementManager.Instance.IsPlacementMode`，存在 stale placement manager 抢先吃输入的风险
     - 正常读档与切场路径没有统一强制 placement-off
- 本轮新增协作物：
  - 已创建给 day1 的窄求助 prompt：
    - `2026-04-10_UI线程_向spring-day1求助_任务清单打包异常与治理口径确认prompt_32.md`
- 当前恢复点：
  - 若用户继续，我下一轮优先修：
    1. `SpringDay1StatusOverlay` 的 16:10 缩放基线
    2. `GameInputManager / ToolbarSlotUI / HotbarSelectionService` 的 selection-placement 统一入口
    3. 如 day1 给出裁定，再按裁定重砍 `PromptOverlay`

## 2026-04-10 UI 实装｜冻结 PromptOverlay，继续收 own 链
- 已读取并接受 day1 裁定：
  - `PromptOverlay` 当前 packaged 异常的第一真实 blocker 是 `formal task card` 与 `manual prompt` 混跑
  - UI 线程这轮停止继续碰 `SpringDay1PromptOverlay.cs`
- 本轮 own 代码推进：
  1. `hotbar / placement / save-load`
     - [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
       - 新增统一即时切槽入口 `TryApplyHotbarSelectionChange`
       - 点击/滚轮/数字键的即时切槽开始共用同一业务入口
       - 场景切换时统一 `ResetPlacementRuntimeState`
       - 世界左键改成只认“活的放置会话”
     - [ToolbarSlotUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs)
       - 点击不再自己跑一套 panel/reject/selection 逻辑
       - 改为调用 `GameInputManager.TryApplyHotbarSelectionChange`
     - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
       - 读档前统一 `ForceResetPlacementRuntime`
       - 原生 fresh start 前统一 `ForceResetPlacementRuntime`
  2. `状态 HUD 缩放`
     - [SpringDay1StatusOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1StatusOverlay.cs)
       - 新增 `CanvasScaler`
       - 参考分辨率：`1980x1080`
       - `matchWidthOrHeight = 1`
     - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
       - 新增 `CanvasScaler`
       - 参考分辨率：`1980x1080`
       - `matchWidthOrHeight = 1`
- fresh validate：
  - `GameInputManager.cs` = `assessment=no_red`
  - `SaveManager.cs` = `assessment=no_red`
  - `InteractionHintOverlay.cs` = `assessment=no_red`
  - `SpringDay1StatusOverlay.cs` = `assessment=no_red`
  - `ToolbarSlotUI.cs` = `assessment=external_red`
    - external 来自：
      - [NPCMotionController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs)
- 新增 day1 协作物：
  - [2026-04-10_UI线程_给spring-day1阶段回执_34.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/2026-04-10_UI线程_给spring-day1阶段回执_34.md)
- 当前恢复点：
  - 等 day1 先主刀拆 `PromptOverlay` owner
  - UI 自己后续优先继续补：
    1. `toolbar/placement/save-load` live 边界
    2. `status HUD` 的 live 收口

## 2026-04-11 UI 续记｜状态提示卡双实例与双状态根因收平
- 已确认这轮不是单纯“再调一点位置”：
  - 老场景里的 `InteractionHintOverlay` 本来就存在，但缺少新字段
  - 旧 runtime 会无脑新建 overlay
  - 旧 `EnsureBuilt()` 又会在老实例上整棵重建
  - 所以玩家侧看到的“重叠 / 双状态 / 奇怪遮挡”本质上是实例复用和补件策略都错了
- 已落代码：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 优先认领现有 overlay
    - 退休重复 overlay
    - 旧场景对象改为只补缺件，不再整棵重建
    - 同帧 guidance/toggle 双触发时只保留一个状态语义
    - `EnsureBuilt()` 恢复快路径，避免 `LateUpdate()` 走重扫描
- 当前状态：
  - 代码层 own red 已清
  - live / packaged 仍待用户终验
- 协作外放：
  - 已补给 `farm` 的 toolbar 窄刀 prompt：
    - [2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-11_UI线程_给farm_toolbar空选中与手持链闭环协作prompt_10.md)

## 2026-04-11 UI 续记｜状态提示卡纯视觉精修
- 按用户最新截图反馈，又补了一轮纯 UI 数值，不改别的链：
  - 状态卡加宽
  - `状态` 标签与标题字号都放大
  - 顶行整体下移
  - 标题行与说明行间距收紧
  - 开关态说明文案压缩成更稳定的单行版本
- 仍只动：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
- 当前阶段：
  - 代码层无 own red
  - 等用户继续看 runtime 观感

## 2026-04-11 UI 续记｜V0.5 checkpoint 已提交，PromptOverlay owner 回 day1，包裹页开始重构
- 当前主线判断：
  - `PromptOverlay / 任务清单` authoritative owner 已回 `spring-day1`
  - UI 线程这轮不再继续碰 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 做治理补丁
- 已提交本地 V0.5 checkpoint：
  - commit: `6ff90a48`
  - message: `checkpoint(ui): V0.5 HUD and overlay fixes`
  - 这次只提交了 UI own 的 HUD / overlay 收口：
    - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - [SpringDay1StatusOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1StatusOverlay.cs)
    - [HealthSystem.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/HealthSystem.cs)
    - [EnergySystem.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/EnergySystem.cs)
- thread-state 事实：
  - `Ready-To-Sync` 仍被 UI 历史 own roots 大量残留阻断，所以这次不是按 sync 收口
  - 用户要求先交 `V0.5`，因此本轮改用最小白名单本地 commit 留 checkpoint，不吞旧尾账
- 包裹页新施工切片：
  - 新 slice：`Package关系页与地图页V0.5体验重构_2026-04-11`
  - 目标只收：
    - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - [PackageMapOverviewPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs)
    - [PackagePanelRuntimeUiKit.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs)
    - [PackagePanelLayoutGuardsTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/PackagePanelLayoutGuardsTests.cs)
- 这轮已落地的玩家面优化：
  - `关系页`
    - 加入当前阶段 chip
    - 名册按 `presence + stage + 当前阶段相关性` 排序
    - 默认选中不再用“第一个不是陌生的人”，改成当前阶段最相关的人，避免 `卡尔` 被莫名其妙推成默认主角
    - 左侧卡片改成 `出场方式 + 关系阶段` 双 chip，并把预览改成当前 beat 的真实印象
  - `地图页`
    - 地图区从“空底板 + 几条线 + 批注感标签”改成分区块 + 路线节点 + active halo 的正式面方向
    - 移除底部 legend 栏，减少开发示意板味道
    - 当前重点点位增加 halo，主次更直观
- 代码层验证：
  - `validate_script PackageNpcRelationshipPanel.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackageMapOverviewPanel.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelRuntimeUiKit.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelLayoutGuardsTests.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `errors --count 20` => `errors=0 warnings=0`
- 当前恢复点：
  - 等用户 live 看包裹里的 `人物关系页 / 地图页`
  - `任务清单` 问题继续由 `day1` 主刀，不再由 UI 线程兜底乱补

## 2026-04-11 UI 续记｜状态提示卡二次微调
- 又按最新截图再收了 3 个纯关系值：
  - 左侧竖条更短
  - `状态` 标签和标题的水平距离更大
  - 说明行再往上贴，缩小上下两行间距
- 仍只动：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)

## 2026-04-12 UI 新刀｜右侧上下文操作提示面板首版落地
- 当前主线：
  - 给玩家面补一张屏幕右侧靠边中部的轻量操作提示卡，只先收 `玩法态 / 背包态` 两组，不动左下角交互提示。
- 本轮完成：
  - 在 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 内新增右侧 `ContextHintCard` runtime scaffold。
  - 上下文分流先直接吃真值：
    - `PackagePanelTabsUI.IsPanelOpen()` => 背包态
    - `DialogueManager / BoxPanelUI / SpringDay1WorkbenchCraftingOverlay` => 阻断态隐藏
    - 其他正常玩家游玩态 => 玩法态
  - 每组提示可用 `Backspace` 单独关闭：
    - 玩法态关闭只影响玩法态
    - 背包态关闭只影响背包态
    - 左下角交互提示完全不受影响
  - 首版文案矩阵：
    - `玩法态`：`右键 / 左键 / E / Tab / V / 1-5`
    - `背包态`：`左键 / Shift+左键 / Ctrl+左键 / B/M/L/O / Tab / Esc`
- 当前判断：
  - 这轮已站住 `结构 / checkpoint`
  - 还没有玩家侧 live 画面，不能 claim 体验过线
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`, `external_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 仍待：
  - 玩家侧看右侧卡实际大小、位置、遮挡关系、文案体感

## 2026-04-12 UI 窄刀｜toolbar 槽位绑定顺序修正
- 当前主线判断：
  - 这轮仍是 UI own 的窄刀，不回漂 `PromptOverlay`、`farm` 输入链或别的 HUD 泛修。
- 根因结论：
  - `toolbar 第五格不显示` 的第一真实根因是 `ToolbarUI.Build()` 把 hotbar 索引绑定在了错误的 hierarchy 顺序上。
  - `ToolBar.prefab` 与 `Home / Primary / Town` 里的 `Bar_00_TG` 子物体顺序都不是视觉顺序，所以“只拿 5 个物体时第五格不显示”本质是视觉槽位和 inventory index 错位。
- 本轮完成：
  - [ToolbarUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)
    - 改成按 `Bar_00_TG` 名字里的 clone 后缀排序后再绑定索引。
    - 非 hotbar 槽位子物体会自动排到后面，不再可能抢占 `0` 号槽位。
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 玩法态右侧提示补上 `Shift 加速移动`
    - `1-5` 提示扩成 `1-5 / 滚轮 切换手持`
- 代码层验证：
  - `validate_script ToolbarUI.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 等用户 live 看 `toolbar` 第五格与右侧玩法态提示
  - 这轮只站住 `结构 / targeted probe`，不是体验终验

## 2026-04-12 UI 精修｜ContextHintCard 结构重排
- 当前主线判断：
  - 用户新回传的截图已经把问题说透：右侧提示卡当前最大缺陷是“像测试说明板”，不是“某个字没对齐”。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 卡片整体收窄
    - accent 改成短条，不再贯穿整块
    - 键帽全部收进壳体内，变成稳定左列
    - 右侧说明列统一起点，不再断裂
    - 标题/说明/列表/页脚层次重新压平
    - 文案从“系统说明”减成更像正式玩家面的提示语
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 等用户看新 live 图
  - 这轮依旧不 claim 体验过线

## 2026-04-12 UI 精修｜ContextHintCard 第二刀减法
- 当前主线判断：
  - 右侧提示卡已经脱离“明显坏相”，这轮继续只做减法和精致度，不碰功能逻辑。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 再次收窄卡片
    - 表头更紧凑
    - 列表文案更短
    - 键帽更小、更收敛
    - 页脚缩成 `退格关闭` 并改成右对齐
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 等用户继续看 live 图
  - 这轮仍只站住 `targeted probe`

## 2026-04-12 UI 续记｜右卡边界透明与右上角隐藏治理确认
- 当前主线判断：
  - 这轮不是继续泛修 UI，而是把 `ContextHintCard` 的右边界透明方案与 `TimeManagerDebugger` 的正式剧情退场条件压实。
- 关键治理结论：
  - 右侧提示卡不能走“整根 overlay 一起 fade”的路子。
  - 因为 [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 根下同时承载：
    - 左下角交互提示
    - 放置模式状态提示
    - 右侧上下文提示卡
  - 整根 fade 会误伤左下角和状态卡，所以安全做法必须是：只对 `contextCard` 自己合成边界 alpha。
- 当前代码状态已确认：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - `SetContextCardAlpha()` 改成只写 `_contextRequestedAlpha`
    - `ApplyContextCardAlpha()` 用 `_contextRequestedAlpha * _contextBoundaryAlpha`
    - `UpdateContextBoundaryFade()` / `ResolveContextBoundaryTargetAlpha()` 只作用右卡
    - 边界阈值与 [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 保持同语义
    - `Package` 页时保持 fully visible；`Dialogue / Box / Workbench` 时直接隐藏
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 新增 `ShouldHideTopRightHud()`
    - `Dialogue active` 或 `SpringDay1Director.ShouldForceHideTaskListForCurrentStory()` 时不再绘制右上角时钟 / 调试帮助
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script TimeManagerDebugger.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/TimeManagerDebugger.cs` => clean
  - blocker 仍是本机没有 active Unity instance，不是本轮 own red
- 当前恢复点：
  - 下轮优先等用户 live 验右边界淡出体感与正式剧情退场效果
  - 这条线当前已站住 `结构 / targeted probe`，但尚未到 `真实入口体验`
