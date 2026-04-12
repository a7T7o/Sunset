# Tooltip 视觉外包 Prompt

## 当前已接受的基线

- 当前 `Tooltip` 的显示时机、显示/消失逻辑、内容拼装、拖拽/Shift/Ctrl 抑制、以及和状态条的联动，不归你这条 UI 线改。
- 这些逻辑仍由 `farm / 交互` 线自己负责。
- 你这轮只接一个主刀：把 `ItemTooltip` 从“难看、像黑片、挡视线、字偏小”的测试味样子，收成一个真正能看的正式 UI 壳。

## 本轮唯一主刀

只做 `ItemTooltip` 的视觉壳与样式语言重做，让它：

1. 明确是一个“框”而不是一块黑片。
2. 颜色参考当前项目已有 UI 语言，但不要和现有背景/格子同色到看不清。
3. 字体更大一点、更清楚一点。
4. 整体看起来像正式游戏 UI，不像开发期临时浮层。

## 最新用户裁定，必须压住

用户最新明确反馈：

- `tooltips 的样式太难看太难看了`
- `做一个框，来显示详情`
- `颜色可以参考现在有的 ui`
- `不要一样的颜色，不然看不清`
- `字体可以大一些`

这轮不要自由发挥成“顺手优化 tooltip 逻辑”。  
只守视觉与可读性。

## 允许的 scope

优先允许改：

- `Assets/222_Prefabs/UI/0_Main/ItemTooltip.prefab`
- `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`

如果为了把 prefab 接回现有绑定，需要补极小的视觉字段/序列化引用接线，也允许。  
但只允许做“视觉结构需要的最小脚手架”，不要改 showDelay / fade / hover suppress / 内容生成语义。

可参考但不要大改：

- 当前项目里已存在的正式 UI 面板、边框、底色、字色语言
- `Assets/YYY_Scripts/UI/Inventory/ItemTooltipTextBuilder.cs` 只读参考内容结构

## 明确禁止的漂移

这轮禁止：

- 改 tooltip 出现时机
- 改 tooltip 消失时机
- 改 tooltip 延迟、渐隐语义
- 改 tooltip 文案内容、字段逻辑、运行时内容拼装
- 改 Inventory / Toolbar / Box 的交互逻辑
- 顺手改状态条逻辑
- 顺手改玩家气泡
- 顺手改别的 UI 面板

如果你觉得当前逻辑会影响视觉表现，可以在回执里报“建议”，但不要直接动逻辑。

## 视觉完成定义

这轮完成后，应该达到下面这些玩家可见结果：

1. `ItemTooltip` 有明确外框、内边距、层级和信息分区。
2. 标题、描述、价格/补充信息有清楚的视觉主次。
3. 字号比现在更易读，不能继续维持“小、挤、像测试字”的感觉。
4. 背景和文字必须有足够对比，不再出现“和现有 UI 太像所以糊成一片”的情况。
5. 整个 tooltip 不应是大块纯黑平面；要有正式 UI 壳体感，但不要花哨过头。
6. 尺寸与留白要克制，避免继续挡太多格子内容。

## 结构线和体验线必须分开报

回执里必须分开写：

- 结构线：
  - 你改了哪些 prefab / 脚本 / 绑定点
  - 是否只停留在 `ItemTooltip` 自己
- 体验线：
  - 玩家最终会看到的变化是什么
  - 相比旧版“黑片 + 小字 + 难看”，现在具体改善在哪
  - 你自己最不放心的视觉点是什么

不要只交 prefab/路径清单。

## 回执格式

聊天里请先用人话回：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要我现在做什么（没有就写无）

然后再补技术审计层：

- changed_paths
- 验证状态
- 是否触碰高危目标
- blocker_or_checkpoint
- 当前 own 路径是否 clean

## thread-state 接线要求

如果你这轮会继续真实施工，先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不准备收口，改跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住
