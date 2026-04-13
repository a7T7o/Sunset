# 2026-04-13｜给 UI 线程｜Tooltip 视觉 contract 与边界说明 01

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\2026-04-06_给UI_Tooltip视觉外包prompt_01.md`
2. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltip.cs`
3. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltipTextBuilder.cs`
4. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
5. `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotInteraction.cs`

当前唯一主刀：

把 `ItemTooltip` 从“测试味、廉价、挡视线、字小、信息像没排版”的临时状态，收成正式玩家面。

这轮是视觉 contract，不是逻辑主刀。

`farm` 线程继续自己负责：

- tooltip 的显示/隐藏时机
- hover / drag / ctrl / shift 抑制
- 跟随鼠标逻辑
- fade / 报错修复
- 运行时边界情况

你这条 UI 线只主刀视觉结果和必要的 prefab / 视觉字段接线。

---

## 一、必须继承的最新用户裁定

用户最新已经明确否定当前 tooltip 的观感，核心反馈是：

1. 现在的 tooltip 很丑，很像开发期测试浮层。
2. 不是正式“框”，像一块硬塞出来的小片。
3. 字太小，信息层级弱，看着费劲。
4. 背景、边框、文字对比和项目里其他 UI 语言不统一。
5. 同一套 tooltip 在：
   - 世界态底部 toolbar hover
   - 打开背包后的 up/down 区 hover
   的正式感都不够。

用户最新真实截图也说明了两个硬事实：

1. 当前 tooltip 已经接到 runtime 里了，不是“还没显示出来”。
2. 当前最差的不是“没有功能”，而是“功能已经在玩家脸上，但很廉价、很糙、很不正式”。

所以这轮不要再做“先能用就行”的测试味方案。

---

## 二、当前已接受的逻辑基线

这些逻辑不归你改：

1. tooltip 什么时候 show
2. tooltip 什么时候 hide
3. tooltip 跟鼠标还是锁位置
4. tooltip 在拖拽、拿起、切换状态时是否抑制
5. tooltip 内容字段拼装规则
6. tooltip 的 runtime 协程报错和 active/inactive 安全问题

你可以：

1. 为了视觉结构补最小的 prefab 层级
2. 为了版式补最小的序列化字段
3. 为了排版补文本容器、边框、背景、标题区、信息区的壳体

但不要趁机把 runtime 逻辑一起吞了。

---

## 三、本轮唯一主刀

### 把 `ItemTooltip` 收成正式 UI 壳

这轮完成后，玩家应该直接感知到：

1. 它是一个清楚、正式、有边框和内边距的游戏内说明框。
2. 标题、主属性、描述、补充信息有明确视觉主次。
3. 不是“黑片 + 小字 + 临时文本块”的测试样。
4. 在 toolbar 和 inventory 两种使用场景下都像同一套正式系统。

---

## 四、视觉 contract

### 1. 壳体

tooltip 必须有清楚的外框、内框或边缘语言。

要求：

1. 有明确边界，不再像一块裸底色。
2. 不要纯黑平片。
3. 颜色可以参考当前项目 UI，但不能和底部 toolbar / 背包底板糊成一片。
4. 背景要压住世界背景噪声，但不能厚重到像遮挡牌。

### 2. 字体与层级

要求：

1. 标题要一眼能看出来是标题。
2. 关键属性行比说明文本更醒目。
3. 描述和补充信息不能继续拥挤发灰。
4. 字号整体比现在更大、更清楚。
5. 对比度必须足够，不允许继续“看得见但费眼”。

### 3. 版式

要求：

1. 标题区、主体信息区、补充说明区有分层。
2. 左右留白、上下留白要像正式面，不要像把文本塞进一个小格子。
3. 宽高可以略放大，但不要大到遮住一大片背包格子或视野。
4. 行间距、块间距、边距要统一，不要一部分挤、一部分空。

### 4. 场景适配

必须同时对齐这两个场景：

1. 世界态：
   - 底部 toolbar hover 时
   - 玩家在场景中移动时
2. 背包态：
   - 打开 package / inventory 后
   - up/down 区 hover 时

要求是“同一套视觉语言”，不是两个地方像两套系统。

### 5. 不要做成什么

明确禁止这些方向：

1. 不要做成网页卡片感
2. 不要做成科幻蓝紫发光面
3. 不要做成高度装饰化、喧宾夺主的 fancy 面
4. 不要继续保留现在这种“开发测试提示框”观感
5. 不要为了花哨牺牲信息可读性

---

## 五、允许的 scope

优先允许：

1. `ItemTooltip.prefab` 或等价 tooltip prefab 壳体
2. `ItemTooltip.cs` 中只和视觉字段绑定相关的最小接线
3. 文本容器、背景、边框、图标区、标题区、属性区的层级重排
4. 颜色、字号、间距、框体尺寸、边距、背景与描边语言

如果你需要改脚本，也只能改“视觉字段接线”和“版式支撑字段”。

---

## 六、禁止漂移

这轮禁止：

1. 改 tooltip show / hide 时机
2. 改 tooltip fade 逻辑
3. 改 tooltip 跟随鼠标语义
4. 改 toolbar / inventory / box 交互逻辑
5. 顺手改右侧 `常用操作` 卡片逻辑
6. 顺手改拖拽、选中、背包行为
7. 顺手改 day1、placement、primary、farm runtime

如果你觉得某个逻辑会限制视觉，请只在回执里报“逻辑侧建议”，不要自己吞掉。

---

## 七、完成定义

完成后至少要同时满足：

1. toolbar hover 时的 tooltip，看起来像正式游戏 UI。
2. inventory hover 时的 tooltip，看起来还是同一套正式 UI。
3. 标题、主要属性、描述三层主次清晰。
4. 字号、对比、留白明显优于当前版本。
5. 玩家第一眼不会再觉得这是开发期临时浮层。

---

## 八、回执时必须分开报

### 结构线

1. 你改了哪些 prefab / 脚本
2. 哪些只是视觉字段接线
3. 是否完全没碰 runtime 逻辑

### 体验线

1. 玩家最终会感受到什么变化
2. toolbar 和 inventory 两处的观感是否统一
3. 你最不放心的视觉点是什么

不要只交路径清单。

---

## 九、用户向回复格式

聊天里先用人话写清：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

然后再补技术审计层：

- changed_paths
- 验证状态
- blocker_or_checkpoint
- 当前 own 路径是否 clean

---

## 十、thread-state 接线要求

如果你这轮会继续真实施工，先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `sync` 前，必须先跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不准备收口，改跑：

`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED` 还是被 blocker 卡住
