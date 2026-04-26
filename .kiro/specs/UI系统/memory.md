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

## 2026-04-12 UI 续记｜ContextHintCard 行内重排与 PackagePanel 最小恢复口
- 当前主线判断：
  - 用户这轮给的截图已经把右侧卡的真实问题说得很具体：不是“风格大方向错”，而是行内中线、字号、页脚键位提示都还没收精。
  - 同时用户明确指出：打开背包时，任务清单没有再像 toolbar 那样被 page 压住。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 右侧 `ContextHintCard` 再次整体抬一档体量
    - 行内描述改成 `MidlineLeft`，重新对齐键帽与说明文字的水平中心
    - 标题 / 行文 / 键帽 / 页脚字号整体增大
    - 页脚新增真正的 `Backspace` 键帽，不再只写“退格关闭”纯文字
  - [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs)
    - 补了一个最小恢复口：背包打开、关闭、以及箱子模式拉起 PackagePanel 时，直接调用当前运行中 `PromptOverlay` 的 `SetExternalVisibilityBlock(...)`
    - 这刀只用现成接口恢复“背包打开时任务清单退场”，不去碰正在大改中的 `PromptOverlay` 脚本脏现场
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `validate_script PackagePanelTabsUI.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs` => clean
- 当前恢复点：
  - 下轮优先等用户 live 看：
    1. 右卡每行是否终于“字与键帽中线平”
    2. `Backspace` 页脚是否足够清楚
    3. 背包/箱子打开时任务清单是否恢复退场

## 2026-04-13 UI 续记｜右卡最终两处直修
- 当前主线判断：
  - 用户最新截图把右卡最后两处未过线问题直接钉死了：
    1. 行内键帽和说明文字仍没真正平到同一条中线
    2. 页脚 `Backspace` 还是英文单词，不是图形退格键
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 行内说明改成固定中心点布局，不再靠拉伸 offset 猜对齐
    - 页脚新增 runtime 退格图形 icon，原英文文字键隐藏
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 下一轮只等用户 live 看右卡最后一眼，不再扩别的 UI

## 2026-04-13 UI 续记｜页脚退格键重画
- 当前主线判断：
  - 用户已裁定“其他的过了，只剩退格键丑”，所以这轮只做一件事：重画页脚退格键。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 重画 runtime `Backspace` icon
    - 键帽和 icon 尺寸一起轻微放大
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 下轮只等用户看这个退格键是否顺眼

## 2026-04-13 UI 续记｜顶部单行化
- 当前主线判断：
  - 用户进一步要求右卡顶部也像放置状态提示那样，`标签 + 标题` 放一行，同时整卡再略缩一点。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 顶部 `玩法` 与 `常用操作` 改成单行结构
    - 卡宽、最小高度、行间距同步再收一档
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 下轮只等用户看顶部这一行是否终于顺手

## 2026-04-13 UI 续记｜右卡整体几何再收口
- 当前主线判断：
  - 用户继续指出“位置不对、细节不对”，所以这轮继续只收右卡的几何关系。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 头部竖线 / 标签 / 标题再统一到同一条中心线
    - 整卡继续缩小
    - 内容整体上提，底部空腔压缩
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=unity_validation_pending`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
- 当前恢复点：
  - 下轮只等用户看右卡的整体位置关系是否终于顺手

## 2026-04-13 UI 续记｜退格符号标准化
- 当前主线判断：
  - 用户直接指出页脚回退键“没有意义”，所以这轮继续只收退格符号本体。
- 本轮完成：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - 重画退格图形为更标准的退格轮廓符号
- 代码层验证：
  - `validate_script InteractionHintOverlay.cs` => `assessment=external_red`, `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs` => clean
  - 当前 external 是场景现存 missing script，不是本轮 own red
- 当前恢复点：
  - 下轮只等用户看这个退格符号是否终于顺眼

## 2026-04-13 UI 续记｜最小安全补丁四连
- 当前主线目标：
  - 按最小可回退方案集中收 4 个玩家可见点：
    - 任务清单边界透明
    - 箱子页右侧提示
    - 关系页左上空头像框
    - 工作台完成其他配方时误刷当前详情
- 本轮完成：
  - `PersistentPlayerSceneBridge.cs`
    - `SpringDay1PromptOverlay` 重新接回边界透明链，最低透明度改为 `40%`
  - `InteractionHintOverlay.cs`
    - 箱子态右侧提示补上
    - 右卡 footer 键帽/图标/文案重新按冷暖组态走
    - backspace 图标放大重画，footer 中线对齐
    - 右卡边界透明改为 `40%` 最低值 + 渐进曲线
  - `PackageNpcRelationshipPanel.cs`
    - 无头像时左上框缩小并显示“暂无画像”
  - `SpringDay1WorkbenchCraftingOverlay.cs`
    - 仅当完成的正是当前选中配方时才清零数量与重建详情
    - 解决“A 完成把 B 的数量选择刷掉”的最小真因
- 验证：
  - `manage_script validate` 四个文件均 `errors=0`
  - fresh `errors` 读取 `errors=0`, `warnings=0`
  - `validate_script` 仍受 Unity `stale_status` 阻塞，当前没有 own red 证据
- 当前恢复点：
  - 等用户集中 live 检查这 4 个点

## 2026-04-13 UI 续记｜箱子页遮挡与关系页大方块二次纠偏
- 当前主线目标：
  - 用户继续 fresh live 指出：
    - 箱子页打开时任务清单仍压在上面
    - 右卡 footer 图标仍然丑
    - 关系页左侧的大阶段方块根本没收住
- 本轮完成：
  - `BoxPanelUI.cs`
    - 让箱子 open/close 也正式接管 PromptOverlay block
  - `InteractionHintOverlay.cs`
    - 把 footer 图标改成更小的纯退格箭头
    - 缩小 footer keycap
  - `PackageNpcRelationshipPanel.cs`
    - 把左侧头部阶段芯片改成固定锚点布局和固定小尺寸
- 验证：
  - `manage_script validate BoxPanelUI` => clean
  - `manage_script validate PackageNpcRelationshipPanel` => clean
  - `manage_script validate InteractionHintOverlay` => warning-only
  - fresh `errors` => `errors=0`, `warnings=0`
- 当前恢复点：
  - 等用户继续看箱子页遮挡、右卡 footer、关系页左侧头部 3 处

## 2026-04-13 UI 续记｜Prompt block 双链统一
- 当前主线目标：
  - 把箱子页/背包页对任务清单的 block 真值统一，避免谁后刷新谁就把另一个顶掉
- 本轮完成：
  - `PackagePanelTabsUI.RefreshPromptOverlayVisibilityBlock()` 现在会把 `IsBoxUIOpen()` 也算进去
  - `BoxPanelUI.RefreshPromptOverlayVisibilityBlock()` 现在会把 `PackagePanelTabsUI.IsPanelOpen()/IsBoxUIOpen()` 一起算进去
- 验证：
  - `manage_script validate PackagePanelTabsUI` => clean
  - `manage_script validate BoxPanelUI` => clean
- 当前恢复点：
  - 等用户 fresh live 看箱子页/背包页任务清单遮挡是否 finally 稳定

## 2026-04-13 UI 续记｜Day1 PromptOverlay contract 收口第一刀
- 当前主线目标：
  - 按 day1 新 contract，把 `任务清单 / Prompt / bridge prompt / 对话显隐 / modal 层级` 这条玩家面主线收成真正稳定的 UI 责任，不再只补外围 suppress。
- 本轮完成：
  - `SpringDay1PromptOverlay.cs`
    - 新增统一的 runtime 单例 block 入口，包裹页/箱子页不再靠 `FindFirstObject` 去打不确定实例
    - 运行时会主动回到 HUD 正确兄弟位，停在 `Package / Dialogue / Workbench / Settings` 这类 modal siblings 前
    - 对话开始/结束改成直接吃事件真值，不再留下 stale alpha 空窗
  - `PackagePanelTabsUI.cs` / `BoxPanelUI.cs`
    - 统一改用 PromptOverlay 全局 block 入口
  - `InteractionHintOverlay.cs`
    - footer 退格键重画成更正常的 backspace 轮廓
  - `PackageNpcRelationshipPanel.cs`
    - 无头像时左上占位继续缩小，去掉大空框感
  - `PackagePanelLayoutGuardsTests.cs`
    - 新增“PromptOverlay 必须回到 HUD lane 且停在 PackagePanel 前”
    - 新增“即使静态字段指到 stale duplicate，也要压住真正 runtime PromptOverlay”
- 验证：
  - `PackagePanelLayoutGuardsTests` => `5/5 passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldHideDuringDialogueAndRecoverAfterwards` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_UsesParentCanvasGovernance_WhenUiRootCanvasExists` => `passed`
  - `SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldPreferBaseCanvasUnderUiRoot_InsteadOfModalPackageCanvas` => `passed`
  - fresh `errors` => `errors=0`, `warnings=0`
- 当前恢复点：
  - 继续等用户 live / packaged 看真正玩家面
  - 如果还有 PromptOverlay 问题，下一刀继续只收 `SpringDay1PromptOverlay` 本体，不再退回外围补丁

## 2026-04-13 UI 续记｜PromptOverlay 闪烁根因收口第二刀
- 当前主线目标：
  - Day1 PromptOverlay 继续从“外围 suppress 补丁”往“真正稳定的玩家面 owner”推进；用户本轮新增纠偏是卡尔列表右侧 chip 栏不统一。
- 本轮完成：
  - `SpringDay1PromptOverlay.cs`
    - 增加 `_visibilityAlpha + _boundaryFocusAlpha` 合成口，开始把最终 alpha 收回单一 owner
  - `PersistentPlayerSceneBridge.cs`
    - 边界焦点命中 PromptOverlay 时改走 `SetBoundaryFocusAlpha(...)`，不再直写 PromptOverlay 的 `CanvasGroup.alpha`
  - `DialogueUI.cs`
    - NonDialogue sibling 扫描现在会明确跳过 PromptOverlay，避免对话期双写 alpha
  - `PackageNpcRelationshipPanel.cs`
    - 卡尔所在的列表右侧 chip 栏改成固定宽高、禁换行、顶部对齐
  - `InteractionHintOverlay.cs`
    - backspace footer 又收一刀
  - `PackagePanelLayoutGuardsTests.cs`
    - 增加 PromptOverlay alpha owner 相关两条护栏
- 验证：
  - `manage_script validate` 当前 touched scripts / tests 均 `errors=0`
  - fresh `errors` => `errors=0`, `warnings=0`
- 当前恢复点：
  - 等用户继续看 live：任务清单在背包 / 边界 / 正式对话里是否还闪；卡尔 chip 栏是否终于统一

## 2026-04-13 UI 续记｜BridgePromptRoot 手调布局对齐
- 当前主线目标：
  - 用户本轮把 UI 任务收窄为 `BridgePromptRoot` 一刀，明确要求运行时直接对齐他手动摆放出来的布局真值。
- 本轮完成：
  - `SpringDay1PromptOverlay.cs`
    - `BuildBridgePromptShell()` 现在按 bridge prompt 专用真值创建 root：
      - `x = 13`
      - `y = 368`
      - `w = 328`
      - `h = 34`
    - 不再复用任务卡的左边距常量，避免后续任务卡微调再次把 bridge prompt 带偏
- 验证：
  - `validate_script SpringDay1PromptOverlay` => `owned_errors=0`，但当前 Unity 现场有 external missing-script 与 `stale_status`
  - `manage_script validate SpringDay1PromptOverlay` => `errors=0`, `warnings=2`
- 当前恢复点：
  - 等用户直接看 bridge prompt 的玩家面位置；如果还差，就继续只收这一处，不扩散

## 2026-04-13 UI 续记｜箱子提示卡色相统一与 footer 键帽收口
- 当前主线目标：
  - 用户继续只收右侧提示卡，指出箱子场景下这张卡“上下颜色不一致”，并要求 `关闭提示` 的键帽再顺一刀。
- 本轮完成：
  - `InteractionHintOverlay.cs`
    - `Chest` 组提示卡改成一整套统一暖色 palette，不再保留顶部冷色、底部暖色的割裂感
    - footer keycap 宽高收紧，icon 轻微放大并下压，减少空腔感
- 验证：
  - `manage_script validate InteractionHintOverlay` => `errors=0`, `warnings=1`
  - `validate_script InteractionHintOverlay` => `owned_errors=0`，但当前 Unity 现场仍有 external tooltip runtime red
- 当前恢复点：
  - 等用户直接看箱子场景下这张提示卡；如果还怪，下一刀继续只收这张卡的配色和 footer 比例

## 2026-04-13 UI 续记｜Tooltip 视觉 contract 微调落地
- 当前主线目标：
  - 接 farm 的 tooltip 视觉委托，并按用户补充要求只做微调：字体、边框、留白、正式感，以及跟鼠标时的边界显示，不改既有功能语义。
- 本轮完成：
  - `ItemTooltip.cs`
    - tooltip 壳体尺寸、边框、padding、header spacing、quality icon 和四段文字字号整体上调
    - 根框补了轻微 outline，内板补了轻微 shadow，正式感比原来的“测试黑片”更稳
    - 跟鼠标时的 bounds 改成“局部容器真的放得下才用局部 bounds，否则退回 canvas 全局 bounds”，所以 toolbar 窄条 hover 不会再被错误局限在太薄的区域里
    - 右/左/上/下越界时都先翻边，再做带安全边距的 clamp
- 验证：
  - `manage_script validate ItemTooltip` => `errors=0`, `warnings=1`
  - `validate_script ItemTooltip` => `owned_errors=0`，但当前 Unity 现场有 external missing-script 与 `stale_status`
- 当前恢复点：
  - 等用户直接看 tooltip 在 toolbar 和 inventory 两处的玩家面；如果还差，只继续收 `ItemTooltip.cs`

## 2026-04-13 UI 续记｜右侧提示卡黄系统一 + 右上角调试提示恢复
- 当前主线：
  - 用户本轮要求两条并收：右侧提示卡继续微调，以及右上角调试提示恢复常驻。
- 本轮完成：
  - `InteractionHintOverlay.cs`
    - `Gameplay / Package / Chest` 三组右侧提示卡统一回暖黄系
    - footer 退格图标改成 `BACKSPACE` 文字键帽
    - 放置模式状态卡再收一刀，宽高和文案区都缩小
  - `TimeManagerDebugger.cs`
    - 去掉运行时对右上角调试提示的隐藏链
    - 调试提示默认重新打开
  - `PersistentManagers.cs`
    - 常驻初始化重新按 `showDebugInfoByDefault: true` 接回 `TimeManagerDebugger`
- 验证：
  - `manage_script validate InteractionHintOverlay` => warning-only
  - `manage_script validate TimeManagerDebugger` => warning-only
  - `manage_script validate PersistentManagers` => clean
  - `validate_script` 当前只拿到 `blocked / external_red`，根因是用户运行中的 Unity 现场已有 `DialogueUI.cs:1975` 运行态异常与 missing-script，不是这轮 touched 文件 own red
- 当前恢复点：
  - 等用户继续看右上角调试提示、右侧提示卡黄系统一和状态卡体量是否 finally 过线

## 2026-04-14 UI 只读续记｜六图审计与 farm 常用操作提示 contract 合并
- 当前主线：
  - 不切题，只读压实 3 条线：`tooltip 成熟命名与可读性`、`右侧常用操作提示的视觉与语义`、`右上角调试时间 UI 的美化边界`
- 本轮结论：
  - tooltip 名称问题不是偶发脏数据，而是 `ItemTooltip` 直接使用 `itemData.itemName`
  - 工作台已有成熟命名映射，可抽成共享 helper 给 tooltip 复用
  - 右侧提示卡已分组，但 footer 结构和文案层还没完全对齐 farm contract
  - 右上角时间的“10 分钟一跳”是 `TimeManager` 真时间粒度，不是单纯 UI 显示问题
- 下一步建议：
  - 先收 tooltip 命名/字体/对比度
  - 再收右侧提示卡 footer 结构和 world/package/chest 文案 contract
  - 最后单独决定是否开启 `TimeManager` 的 1 分钟粒度 runtime 改造

## 2026-04-14 UI 续记｜tooltip 正式命名 + 右侧提示卡 contract + 右上角时间 HUD 与分钟粒度
- 当前主线：
  - 用户允许直接继续落地，不切主线；这轮把 tooltip 正式命名、右侧提示卡正式玩家面、以及右上角时间/调试 HUD 一起收。
- 本轮完成：
  - tooltip：
    - [ItemTooltipTextBuilder.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs) 新增 `BuildPlayerFacingTitle(...)`
    - 复用工作台成熟语义，把工具 / 剑 / 箱子名称收成玩家向中文名
    - 描述预算放宽到 `3 行 / 72 字`
    - [ItemTooltip.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs) 标题改走玩家向命名，壳体与字号继续上调
  - 右侧提示卡：
    - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 正式区分 `Gameplay / Package / Chest`
    - 文案按 farm contract 对齐，footer 固定成 `关闭提示 + BACKSPACE`
    - 边界透明改成渐进，下限 40%
  - 右上角时间/调试 HUD：
    - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 改成正式小 HUD 壳，包含时间卡和快捷键卡
    - `+/-` 现在是真正的 `±1 小时并保留分钟`
  - 分钟粒度真源：
    - [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs) 改成逐分钟流逝
    - `minuteStepsPerHour=60`，`AdvanceMinute()` 改 `+1`，`SetTime()` 接受 `0~59`
    - 旧序列化值 `6` 会在 `OnValidate()` 自动修到 `60`
- 验证：
  - `validate_script` 覆盖 5 个 touched 文件时，`owned_errors=0`
  - 当前 assessment 为 `unity_validation_pending`
  - fresh `errors` 仍有 external missing-script，不能 claim 全量 no-red
- 当前恢复点：
  - 等用户 live 看提示卡、tooltip、右上角 HUD 和分钟流逝

## 2026-04-14 UI 只读续记｜`+` 快进时间语义误改根因
- 当前主线：
  - 用户反馈 `+` 快进后分钟没有归零，这轮只读彻查真实情况。
- 结论：
  - 不是 HUD 显示滞后，而是 [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs) 的 `AdvanceOneHour()` 被改成了“保留分钟”
  - 当前真实行为：
    - `14:37 +` => `15:37`
    - `26:37 +` => `Sleep()` 后再补回分钟，结果会落到 `06:37`
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 Day1 time guardrail 也会保留 minute，不会自动归零
  - 用户提出的“先到 :59 再自然 +1 分钟”方向抓到了跨 2 点边界的要害，但若直接用现有 `SetTime(:59)` 会额外广播一次 `:59` 时间事件，不适合原样照搬
- 判断：
  - 真正要修的是 `TimeManagerDebugger` 的调试跳时语义
  - 最稳修法应是“普通整点直接到 `xx:00`，跨 2 点必须走 `Sleep()` 真链”

## 2026-04-14 UI 只读续记｜Day1 第三点 owner 判断
- 当前主线：
  - 用户要求我先判断“超过凌晨 2:00 的玩家稳定回家睡觉”这一条到底该谁主刀，避免 UI 和 spring-day1 线程并改。
- 结论：
  - [TimeManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/TimeManager.cs) 负责的是全局时钟与通用 `Sleep()` 事件
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 才负责 Day1 专属的夜间收束：非法早睡拦截、合法 DayEnd、blink/换场、玩家回住处、NPC/剧情角色回 anchor、相位切到 `DayEnd`
  - 因此第三点整体应由 `spring-day1` 主刀，不应继续落在 UI 线程
  - UI 线程只应保留自己误改出来的 `TimeManagerDebugger +/-` 调试语义修复

## 2026-04-14 UI 续记｜给 spring-day1 的状态说明文件
- 当前主线：
  - 用户要求我只做好自己的 own，并给 `spring-day1` 写一封“状态说明 / 边界告知”，不是命令。
- 本轮完成：
  - 新增告知文件：
    - [2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_时间owner边界与自收内容告知_11.md)
  - 核心内容：
    - 第三点整体 owner 属于 `spring-day1`
    - UI 不再碰 `SpringDay1Director` 的睡觉 / DayEnd 逻辑
    - UI 会自己收掉 `TimeManagerDebugger +/-` 的调试整点语义

## 2026-04-14 UI 续记｜Day1 玩家面 contract 第一刀已落
- 当前主线：
  - UI 线程按 `prompt_12` 接管 Day1 玩家可见任务清单 / bridge prompt / modal 层级 / re-entry 可见面，并自收 `TimeManagerDebugger +/-`。
- 本轮完成：
  - `TimeManagerDebugger +/-` 已回到整点跳转；跨 `26` 直接走 `Sleep()`，不再补分钟
  - `BridgePromptRoot` 已从漂浮 overlay 壳改为挂在任务卡下方，跟随任务卡壳一起重建
  - Prompt modal block 新增 workbench 退让判定，package / box / workbench 语义统一
  - 新增给 `spring-day1` 的 UI owner 状态说明：
    - [2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/UI系统/0.0.2_玩家面集成与性能收口/2026-04-14_UI线程_给spring-day1_Day1玩家面UI收口状态告知_13.md)
- 验证：
  - 代码层未见 owned red
  - `validate_script` 仍停在 `unity_validation_pending`
  - 原因不是脚本 owned error，而是 Unity 当前卡在 `playmode_transition / stale_status`
- 边界：
  - UI 没有继续越权碰 Day1 的 `HandleSleep / DayEnd / anchor / resident release`

## 2026-04-14 UI 补记｜workbench 自关回归已修回
- 用户阻塞反馈：
  - 工作台打开后立刻关闭，表现为 UI 闪一下就没了。
- 根因：
  - UI 线程把 workbench 放进了 `IsBlockingPageUiOpen()`
  - 但 workbench 自己的 `LateUpdate()` 也读这条判定
  - 导致工作台把自己识别成“外部 page modal”，一打开就立即 `Hide()`
- 修复：
  - 全局 `IsBlockingPageUiOpen()` 回到只认 `package / box`
  - 只在 `ShouldHidePromptOverlayForParentModalUi()` 单独补认 workbench
  - 结果是 PromptOverlay 继续对 workbench 退让，但 workbench 本体不再自关

## 2026-04-14 UI 续记｜可见面微调第二刀
- 当前主线：
  - 用户要求继续只收 UI 可见面细节，不扩到别的逻辑，并额外要一段可转发给 `day1` 且不打断当前进程的引导壳。
- 本轮完成：
  - 右上角时间/调试卡：
    - 调试面板加宽加高
    - 时间与日期状态改为真正居中
    - 快捷键列表行高和文字容器加大，解决底部裁切
  - 任务清单下方提示条：
    - 左边缘重新贴齐任务卡左边
  - 右侧常用操作卡：
    - footer `BACKSPACE` 改成低强调关闭区，不再像普通操作键帽
    - 关闭提示区的颜色、间距、keycap 尺寸重新分配
  - 左下交互卡：
    - `DetailCard / CompactCard` 整体轻缩，箱子提示更紧凑
- 验证：
  - 三个 touched 脚本都没有 error
  - 仅剩既有 warning（字符串拼接 / GameObject.Find 提示）

## 2026-04-14 UI 补记｜右侧常用操作卡二次细调
- 当前主线：
  - 用户继续要求只收右侧常用操作卡的 spacing / footer / tag pill 观感。
- 本轮完成：
  - footer `关闭提示` 与 `BACKSPACE` 间距继续拉开
  - 行距轻微增加，但不把整体撑散
  - `背包 / 玩法 / 箱子` tag pill 更窄，字更大
- 验证：
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs) 无 error，仅剩既有 warning

## 2026-04-14 UI 只读续记｜任务清单完成态切换错位由 PromptOverlay own
- 当前主线：
  - 用户要求查清任务清单为什么没有先完整显示完成态，而是和下一条未完成任务内容错位。
- 本轮结论：
  - 根因在 `PromptOverlay` 自己的状态机时序，不是先去改 Day1 真逻辑。
  - `LateUpdate()` 会先把 `_pendingState` 刷成最新真值；只要任务一完成，最新真值就已经指向下一条未完成任务。
  - `TransitionToPendingState()` 只对旧 row 播完成动画，但随后整张卡的 `title / subtitle / focus / footer / rows` 会整体切到新的 `_pendingState`。
  - `PromptCardViewState.FromModel(...)` 又会优先拿“第一个未完成项”作为主项，因此 UI 天然会先跳下一条。
- 现阶段判断：
  - 用户要求的“完成框和任务内容必须属于同一个状态”是对的，当前实现确实没守住。
  - 下一刀应由 UI 在 `SpringDay1PromptOverlay.cs` 内补“completed snapshot”过渡；先显示旧任务完成态整卡，再切下一条。
- 当前阶段：
  - 只读根因已定位，尚未开始改代码

## 2026-04-14 UI 续记｜任务清单 completed snapshot 修复
- 当前主线：
  - 用户要求正式落地任务清单切换修复，并且先做可回退 checkpoint。
- 本轮完成：
  - 本地 checkpoint：
    - `33985c17` `checkpoint: save UI prompt baseline before completion snapshot fix`
  - 修复提交：
    - `f172ec62` `fix: keep completed prompt card state before advancing`
  - 核心修法只落在 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)：
    - 新增 completed hold state
    - 任务完成时先显示旧卡 completed snapshot
    - 再切到下一条未完成任务
- 验证：
  - `validate_script` 四文件结果：`no_red`
  - fresh console：`0 error / 0 warning`
- 当前阶段：
  - 代码层完成，live 体验待用户复测

## 2026-04-14 UI 续记｜调试卡宽度回调、footer 间距回调、任务清单根因二次确认
- 当前主线：
  - 用户要求直接修两处可见面细节，并重新说明任务清单为什么还是“绿一下就换字”。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 调试卡宽度收窄到 `248`
  - [InteractionHintOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs)
    - footer 行下移
    - `关闭提示` 向 `BACKSPACE` 靠近
- 对任务清单的新判断：
  - 上一刀只统一了主任务卡，没有统一 bridge/manual prompt
  - 现在剩下的真问题是 `PromptCard`、`BridgePrompt`、hidden->resume 三条过渡链没有放进同一个 completion transaction
  - 所以用户仍会看到“完成框、正文、提示各走各的”

## 2026-04-14 UI 续记｜PromptOverlay completion transaction 与 modal sibling 止血
- 当前主线：
  - 用户批准继续彻底解决任务清单，并补查 `PackagePanel` 与 `PromptOverlay` 的层级互跳。
- 本轮完成：
  - `PromptOverlay` 现在会把主卡和 bridge prompt 一起过渡，不再让 bridge prompt 提前抢文字
  - `PromptOverlay` 在 modal 解锁后，不再直接跳最新态，而是改走正式 transition
  - `PromptOverlay` 在 modal 打开时停止 sibling 纠正，不再继续和 `PackagePanel` 抢 hierarchy 位置
  - 同轮顺手微调了：
    - 右上角调试卡宽度
    - 右侧 footer 的 `BACKSPACE / 关闭提示` 间距
- 验证：
  - `SpringDay1PromptOverlay` 代码层无 error
  - fresh console 为 `0 error / 0 warning`
  - 三文件 `validate_script` 这轮停在 `unity_validation_pending`，原因是 Unity 正处于 `playmode_transition`，不是 owned red

## 2026-04-17 UI 补记｜工作台木稿配方降为纯木头
- 用户临时要求：
  - 把工作台木稿子配方改成和木斧一样，只需要木头。
- 本轮完成：
  - [Recipe_9102_Pickaxe_0.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset)
    - 移除石头材料项
    - 保留 `3200 x3`
    - 描述同步改成纯木头语义
- 对照：
  - [Recipe_9100_Axe_0.asset](D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset) 也是 `3200 x3`
- 验证：
  - `git diff --check` 通过
  - fresh console：`0 error / 0 warning`
- 边界：
  - 这轮只改 recipe asset，不碰 crafting service / overlay / inventory 逻辑。

## 2026-04-17 UI 补记｜右上角时间/调试 HUD 卡片收窄
- 用户反馈：
  - 右上角时间卡和调试快捷键卡仍然测试味重、空白多、不够精致，要求只优化显示。
- 本轮完成：
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 面板宽度与高度整体收窄
    - `时间 / 调试` 标签 pill 缩短
    - `快捷键` 标题左收
    - key pill 与说明文字距离重排，减少右侧空旷
- 验证：
  - `manage_script validate TimeManagerDebugger`：`errors=0 warnings=1`
  - fresh console：`0 error / 0 warning`
- 边界：
  - 这轮不碰时间推进、暂停、下一天、切季节、加减号逻辑，只收玩家可见几何排版。

## 2026-04-17 UI 只读补记｜工作台“点击即扣料”补丁方案已厘清
- 用户要求：
  - 只要点击制作，不论开始还是追加，都应立刻从背包扣材料，材料等于交给工作台；中断时再返还。
- 代码结论：
  - 当前不满足该语义。
  - 首件材料是在 CraftRoutine() 里按件预扣；追加/排队只改 entry.totalCount，不立即扣料。
  - 直接复用 CraftingService.TryCraft(recipe, false) 做点击预扣是危险的，因为它会提前触发 OnCraftSuccess，进而污染 Day1 的 _craftedCount。
- 最小安全方案：
  - 只改 CraftingService.cs 和 SpringDay1WorkbenchCraftingOverlay.cs
  - 在 CraftingService 加独立“预扣材料”入口，不触发 success 事件
  - WorkbenchOverlay 点击时先预扣再入队
  - CraftRoutine 只跑进度和完成
  - CancelActiveCraftQueue / CancelQueuedRecipeEntry 统一按“未完成件数”退款
  - GetSelectableQuantityCap() 去掉旧的 queuedAlready 二次扣减
- 当前状态：
  - 只读方案已成，等待用户批准真实施工。

## 2026-04-17 UI 补记｜工作台点击即扣料与退款闭环已落地
- 用户要求：
  - 工作台点击制作 / 追加制作时立即扣背包材料，等同把材料交给工作台；中断与取消排队时按未完成件数退款；跨场景回到工作台场景时要用当前场景事实源刷新。
- 已落地：
  - [CraftingService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs)
    - 新增独立预扣入口 `TryReserveMaterials(...)`
    - 新增独立完成通知 `NotifyCraftCompleted(...)`
  - [SpringDay1WorkbenchCraftingOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)
    - 点击开始 / 加入队列 / 追加时先预扣，再写入队列
    - 制作协程不再临时扣料，只负责进度和完成
    - 中断当前制作、取消排队、硬停协程都会按未完成件数退款
    - 场景切回绑定工作台场景时会重绑当前场景背包事实源并刷新状态
    - 数量上限去掉旧的 queuedAlready 二次扣减
  - [WorkbenchInventoryRefreshContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs)
    - 新增预扣/退款/场景回切相关 contract
- 验证：
  - 窄 EditMode 合同测试：`7 passed / 0 failed`
  - fresh console：`0 error / 0 warning`
- 边界：
  - 这轮不碰 SaveManager / PersistentBridge / SpringDay1Director 真逻辑，只在工作台 own 语义里收口。

## 2026-04-17 UI 补记｜工作台 stale contract 尾账
- 背景：
  - 用户确认“已交材料后显示红字 = 不能继续追加制作”的玩家语义成立，因此这轮不动材料红字表现，只清 stale contract。
- 已处理：
  - [SpringDay1DialogueProgressionTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs)
    - 木稿配方断言从旧石料需求改为当前真实木料需求
    - 工作台提示记忆断言从旧 `PlayerPrefs` 直写改为 `StoryProgressPersistenceService` 真语义
- 验证：
  - `validate_script` 两个测试文件均 `0 error / 0 warning`
  - `WorkbenchInventoryRefreshContractTests`：`7 passed / 0 failed`
- 风险与剩余：
  - 单条 Day1 大测试目前被 Unity MCP test job 初始化超时卡住，暂时只能确认代码与窄合同 clean，未把这条包装成“已过”。

## 2026-04-18 UI 只读补记｜关系页 chip 的“区别对待感”来自同色重块
- 用户贴新截图后再次追问：为什么 `卡尔` 及其以下几行右侧标签框看起来更突出。
- 复核结论：
  - 不是 `卡尔` 单独特判，也不是我上一轮说的“主要是选中态”。
  - 真正更准确的原因是：
    1. `PresenceLevel.None -> 未露面`
    2. `Stage.Stranger -> 陌生`
    3. 两枚 chip 当前用的是同一套棕灰色值
  - 因此从 `卡尔` 开始这批“未露面 + 陌生”条目，会在右侧形成两块完全同色、连续堆叠的重块；上面的 `压场 / 正面 / 路过` 则因为 presence chip 是橙/绿/浅卡其，不会形成同样的重块感。
- 代码锚点：
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - `ListChipWidth = 72`
    - `ListChipHeight = 24`
    - `PresenceLabel(None) => 未露面`
    - `GetPresenceChipColor(None)` 与 `GetStageChipColor(Stranger)` 当前相同

## 2026-04-18 UI 补记｜关系页 chip 减重与右上角调试卡对齐
- 用户要求：
  - 把关系页里 `卡尔` 往下那批“看起来更长更重”的右侧标签修掉，并顺手把右上角时间调试卡的按钮列对齐做精一点。
- 本轮完成：
  - [PackageNpcRelationshipPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs)
    - chip 列改成右对齐、不再强制横向撑满
    - presence / stage 两级宽度分离，避免继续堆成两块等宽重砖
    - `未露面` 与 `陌生` 的底色拆开，降低同色连续重块感
  - [TimeManagerDebugger.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/TimeManagerDebugger.cs)
    - 快捷键标题与说明列统一到固定起点
    - key pill 改成固定列宽
    - 行距与标题基线收平
- 验证：
  - `manage_script validate PackageNpcRelationshipPanel`：`0 error / 0 warning`
  - `validate_script TimeManagerDebugger`：`no_red`
  - fresh console：`0 error / 0 warning`
- 边界：
  - 这轮只动关系页 chip 和调试卡排版，不碰别的 UI 逻辑。

## 2026-04-18 UI 补记｜存档设置页重复快捷说明删除
- 背景：
  - 用户指出设置页左侧标题下已经有 `F5/F9` 与默认槽说明，右侧重复再来一张介绍卡既多余又发生重叠。
- 本轮处理：
  - [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
    - 删除右侧 `GuideCard`
    - 清掉关联的 `_guideShortcutText / _helpText` 运行时字段与刷新文案
    - 保留右侧操作按钮区，不改变保存/读取/重开行为
- 当前判断：
  - 这是结构性去重，不是整页视觉重做；代码层已 clean，但最终观感是否完全过线仍以用户 live 体验为准。
- 验证：
  - `validate_script PackageSaveSettingsPanel`：`0 error / 0 warning`
  - fresh console：无新增 own 红黄

## 2026-04-23 UI 上传补记｜已推两笔，代码批次停在 codeguard blocker
- 目标：
  - 只做 UI 当前 own 成果的完整保本上传，不扩功能、不吞 shared/mixed。
- 已完成：
  - UI docs/specs/prompt 批已推到 `origin/main`
    - commit: `ea6ac827338bb1a382e5ef1b41ac49cc3b392353`
  - UI 字体材质批已推到 `origin/main`
    - commit: `edd3baea26acf78fd4327a16cfe0e81656e83cc3`
- 未完成但已精确报实：
  - `Assets/YYY_Scripts/UI/*`
  - `Assets/YYY_Scripts/Story/UI/*`
  - 这批代码未继续 push；真实 blocker 是 `CodexCodeGuard` 在 `pre-sync` 阶段无法稳定返回 JSON，不是我把 shared/mixed 偷吞进去了。
- thread-state：
  - 这轮已跑 `Begin-Slice`
  - docs 批 `Ready-To-Sync` 正常通过
  - 代码批 `Ready-To-Sync` 报 codeguard blocker 后未再硬撞
  - 当前已 `PARKED`

## 2026-04-23 UI 上传续记｜第二波历史小批次只试 Tabs 新 panel/runtime-kit，卡在同根残留
- 用户最新裁定：
  - 第二波不再按“clearly-own 整批先交”推进，而是按真实开发历史一小刀一小刀上传；这轮只允许 `1` 个历史小批次。
- 本轮唯一小批：
  - `PackageMapOverviewPanel`
  - `PackageNpcRelationshipPanel`
  - `PackagePanelRuntimeUiKit`
  - 及各自 `.meta`
- 结果：
  - 已真实尝试 pre-sync
  - 没有提交、没有 push
- 精确 blocker：
  - 第一真实 blocker 不是 `CodexCodeGuard`
  - 而是同根 `Assets/YYY_Scripts/UI/Tabs` 下还有未纳入本轮的 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 脏改
  - 因此 preflight 直接以 `own roots remaining dirty` 阻断，本轮没进入第二层 codegate
- 判断：
  - 这 3 个新增 panel/helper 本身可视作同一历史创建批
  - 但从 shared-root 白名单 sync 角度，它们并不是独立可交的小批，因为 `PackagePanelTabsUI.cs` 仍是同根接线残留
- 边界：
  - 本轮没有借机吞：
    - `Assets/YYY_Scripts/UI/Inventory/*`
    - `Assets/YYY_Scripts/UI/Toolbar/*`
    - `Assets/YYY_Scripts/UI/Box/*`
    - `Assets/YYY_Scripts/Story/UI/*`
    - 也没有把 `PackagePanelTabsUI.cs` 一起带上
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑，命中 blocker
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-23 UI 复核补记｜同一小批次 prompt 再次下发，保持停车
- 我已重新读取同一份 `prompt_02`。
- 复核结果：
  - 当前仍是同一 Tabs 新 panel/runtime-kit 小批次
  - 当前仍是同一个 same-root mixed blocker
  - 当前 `thread-state` 仍为 `PARKED`
- 处理：
  - 本轮不重新发起第二次上传尝试
  - 不切换到第二批
  - 维持“已尝试过一次并已精确报死”的现场结论

## 2026-04-24 UI 上传续记｜PackagePanelTabsUI 正式并入 Tabs 整合批，阻断改判为 CodeGuard
- 用户最新裁定：
  - 不再重跑 `prompt_02`；这轮只把 [PackagePanelTabsUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 正式纳入同批核心件，按 `UI/Tabs` 根内整合批再做一次真实上传尝试。
- 我这轮的批次判断：
  - 正式承认 `PackagePanelTabsUI.cs` 属于这批核心件。
  - 因为它自身直接安装：
    - `PackageMapOverviewPanel`
    - `PackageNpcRelationshipPanel`
  - 所以这不是新的临时扩包，而是把前一轮 already-proven 的同根集成件补进来。
- 本轮根内结果：
  - `Assets/YYY_Scripts/UI/Tabs` 下相关现场只剩这 7 个文件
  - 不再存在新的 same-root remaining dirty
- 真实阻断：
  - 新的第一真实 blocker 已经从 same-root mixed 切换成 `CodexCodeGuard`
  - 3 个核心文件 `validate_script` 直接报：
    - `CodexCodeGuard returned no JSON`
  - `PackagePanelTabsUI.cs` 则只拿到：
    - `assessment=unity_validation_pending`
    - `mcp_fallback_reason=baseline_fail`
- 边界：
  - 本轮没有扩到：
    - `Assets/YYY_Scripts/UI/Inventory/*`
    - `Assets/YYY_Scripts/UI/Toolbar/*`
    - `Assets/YYY_Scripts/UI/Box/*`
    - `Assets/YYY_Scripts/Story/UI/*`
- thread-state：
  - `Begin-Slice`：已跑
  - 因代码闸门已阻断，未进入 `Ready-To-Sync`
  - `Park-Slice`：已跑，当前 `PARKED`

## 2026-04-26 UI 复核续记｜工具修复后最小复核已证明确实进入真实业务 blocker
- 用户要求：
  - 只做 `UI/Tabs` 七文件在工具修复后的 `1` 次真实最小复核，不修业务代码，不换第二批。
- 本轮结果：
  - 已真实跑到 `Ready-To-Sync`
  - 工具黑盒已消失，不再返回：
    - `CodexCodeGuard returned no JSON`
    - `baseline_fail`
  - 当前新的第一真实 blocker 已明确是 `PackagePanelTabsUI.cs` 里的 `3` 条编译错误 + `1` 条 warning
- 诊断摘要：
  - `CS0103` x3：
    - `PackageSaveSettingsPanel` 在 `53/67/215` 行不存在
  - `CS0649` x1：
    - `boxUIRoot` 在 `20` 行从未赋值
- 判断：
  - 这组现在已经从“工具 incident”降级成“真实业务 blocker”
  - 不是 same-root mixed，也不是工具黑盒
- 边界：
  - 本轮没有修这些错误
  - 也没有扩到 `Inventory / Toolbar / Story/UI`
- thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：已跑
  - `Park-Slice`：已跑，当前 `PARKED`
