# SpringUI-Day1基线复刻与长期技术路线总方案

## 0. 文档定位

本文件不是实现说明，不是类图，不是直接开工清单。

本文件的唯一目的，是在 `spring-day1` 这次 UI 事故之后，把下面 7 件事彻底说死：

1. 这次问题本体到底是什么
2. 为什么当前 runtime UI 不等于用户手调 prefab
3. 为什么现在不能先抽象，必须先把 prefab 接回运行时
4. 为什么正确答案不是三选一，而是“视觉 prefab / 行为代码 / 差异数据”
5. 为什么顺序必须是“先抄 -> 再稳 -> 再抽”
6. 用户那 6 条需求分别属于哪一层、哪个阶段
7. 哪些路线当前明确不能走

在本文件过审前，不进入实现。

---

## 1. 问题重新定义

### 1.1 这次事故的第一问题，不是“架构还不够高级”

这次 `spring-day1` UI 事故的第一问题不是：

- 没有 binder
- 没有 provider
- 还没模板化
- 代码写得不够优雅

而是：

**用户已经亲手把两个 UI 在运行时调顺眼，并拖回项目保存成 prefab，但这两个 prefab 没有重新成为 runtime 的权威视觉源。**

换句话说，这次首先坏掉的不是“长期抽象能力”，而是“正式视觉基线回流链路”。

### 1.2 用户真正不满意的点，是顺序被做反了

用户真实要求的顺序一直很稳定：

1. 先把手调 prefab 逐项照抄回 runtime
2. 再在这个正确 formal-face 上补体验增强
3. 最后才讨论如何抽成长期可复用方案

也就是：

**先抄 -> 再稳 -> 再抽**

前一轮线程并不是完全没看懂长期方向，但它把这三步做成了：

1. 参考 prefab
2. 先做自己理解后的更稳实现
3. 再说以后可以模板化

这会让用户感受到的不是“升级”，而是“跳过我已经调好的结果，重新发明一版”。

---

## 2. 谁是谁，顺序为什么会被误读

### 2.1 两个 Day1 prefab 的真实身份

`Assets/222_Prefabs/UI/Spring-day1/` 下的两个 prefab，不是普通参考稿，也不是单纯验收截图壳。

它们的真实身份是：

- 用户在运行游戏时，因无法接受当时排版、字号、字体、位置和整体观感
- 亲手把两套 UI 调到“至少能入眼”
- 再拖回项目里，保存成 prefab

所以对当前阶段来说，它们不是“灵感来源”，而是：

**Phase 1 唯一视觉基线**

### 2.2 当前 runtime UI 的真实身份

当前 runtime UI 不是“这两个 prefab 被实例化以后在场上运行”。

当前 runtime UI 的真实身份是：

- 运行时由代码 `new GameObject + AddComponent + BuildUi()`
- 当场重新长出一套 UI 壳
- 然后再用代码填内容、刷新状态

所以现在实际上并存着两套东西：

- `prefab formal-face`
- `runtime face`

这两套东西不是自动同步关系。

### 2.3 前线程错在哪

前线程的长期方向判断并非全部错误。它提出的下面这件事，长期上仍然成立：

- 不应该继续纯 `BuildUi()`
- 长期应该走 `prefab + 绑定代码 + 数据/规则`

但它错在顺序：

- 还没把 `Phase 1` 的 formal-face 抄准
- 就提前滑向“更稳的抽象实现”

因此它真正的问题不是“完全看反”，而是：

**方向部分可保留，但顺序必须被强制纠偏。**

---

## 3. 当前 runtime 与手调 prefab 的关系判断

## 3.1 结论

当前 `spring-day1` 运行时 UI，**不是**对用户手调 prefab 的直接照抄。

更准确地说，当前状态是：

**prefab formal-face 与 runtime face 双源并存**

### 3.2 为什么这个结论成立

因为当前运行入口决定了 runtime UI 不会自动继承用户后来存进 prefab 的参数。

只要运行入口还是：

- `new GameObject`
- `AddComponent`
- `BuildUi()`

那么 runtime 长出来的就仍然是一套“代码自己定义的 UI”。

即使代码参考过 prefab 的某些尺寸、颜色或结构，也仍然不是“prefab 成为了运行时唯一真源”。

### 3.3 这个双源状态会造成什么后果

双源并存的后果，不是抽象层面的“不优雅”，而是非常具体：

1. 用户手调的视觉参数不会自动回流到 runtime
2. 代码里继续写死的默认值会再次偏离 prefab
3. 用户以后再手调 prefab，runtime 仍可能不跟
4. 评审时会反复出现“prefab 看着对，运行时还是不像”
5. 一旦拿当前 runtime 去抽模板，就会把错误脸面继续传播到所有工作台

---

## 4. 关键代码与参数证据

本节不是为了堆细节，而是为了让“不进 Unity 也能看出为什么 runtime UI 不等于手调 prefab”。

### 4.1 运行时入口证据

#### 证据 A：PromptOverlay 是运行时自建

文件：

- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`

关键事实：

- `EnsureRuntime()` 中直接 `new GameObject`
- 然后 `AddComponent<SpringDay1PromptOverlay>()`
- 随后调用 `BuildUi()`

这意味着：

- Prompt UI 不是通过 `Instantiate(SpringDay1PromptOverlay.prefab)` 进入运行态
- 用户后来手调后保存到 prefab 的 RectTransform、Canvas、局部尺寸和位置，不会自动成为 runtime 参数

#### 证据 B：WorkbenchOverlay 也是运行时自建

文件：

- `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`

关键事实：

- `EnsureRuntime()` 中同样直接 `new GameObject`
- 然后 `AddComponent<SpringDay1WorkbenchCraftingOverlay>()`
- 随后调用 `BuildUi()`

这意味着：

- 工作台 UI 也不是 prefab 实例化链
- runtime 面板骨架、默认尺寸、默认位置来自 C#，不是来自你手调后的 prefab

#### 证据 C：工作台交互入口确实在走这条 runtime 自建链

文件：

- `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`

关键事实：

- `ResolveWorkbenchOverlay()` 中调用 `SpringDay1WorkbenchCraftingOverlay.EnsureRuntime()`

这进一步坐实：

- 当前用户实际看到的工作台 UI，是代码现场造出来的那套
- 不是把 `SpringDay1WorkbenchCraftingOverlay.prefab` 直接接回去运行

### 4.2 参数级差异证据

下面这些差异足以证明“不是同一套东西”，而不是“只差一点点”。

#### 证据 D：PromptOverlay 的 Canvas 模式已经分叉

Prefab：

- `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
- Canvas `m_RenderMode: 2`

代码：

- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- `overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay`

这说明至少在 Canvas 基础配置层，runtime 和 prefab 已不是同一权威源。

#### 证据 E：PromptOverlay 的卡片位置与高度也分叉

Prefab：

- `TaskCardRoot`
- `AnchoredPosition = {x: 11.900024, y: -12.9672}`
- `SizeDelta = {x: 328, y: 229.9346}`

代码：

- `cardRect.anchoredPosition = new Vector2(26f, 8f)`
- `cardRect.sizeDelta = new Vector2(328f, 188f)`

这不是“运行时自动微调”，而是两边本来就不是一套参数。

#### 证据 F：WorkbenchOverlay 的 Canvas 模式也分叉

Prefab：

- `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
- Canvas `m_RenderMode: 2`

代码：

- `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- `overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay`

#### 证据 G：WorkbenchOverlay 的 PanelRoot 位置和尺寸直接分叉

Prefab：

- `PanelRoot`
- `AnchoredPosition = {x: 87.79856, y: -126.66856}`
- `SizeDelta = {x: 428, y: 257.1085}`

代码：

- `panelWidth = 428`
- `panelHeight = 236`
- 然后 `panelRect.sizeDelta = new Vector2(panelWidth, panelHeight)`

也就是说：

- 宽度有参考
- 但高度已经不是你手调出来的正式值
- 位置也不是 prefab 那套

#### 证据 H：Workbench prefab 不是完整 runtime 壳，说明 runtime 功能后来又在代码里长了一层

Prefab 中仍有空绑定，例如：

- `materialsViewportRect: {fileID: 0}`
- `progressRoot: {fileID: 0}`
- `floatingProgressRoot: {fileID: 0}`

但代码里这些区域是在 `BuildUi()` 过程中继续创建和接线的。

这说明现在不是：

- “prefab 完整定义结构，代码只填数据”

而是：

- “prefab 里保留了一套正式视觉面”
- “代码又继续扩长运行时结构和行为”

这正是双源并存的实证。

---

## 5. 为什么不能先抽象，必须先把 prefab 接回运行时

### 5.1 因为当前真正缺的不是抽象层，而是权威视觉源回流

如果当前立刻进入：

- binder
- provider
- 通用模板
- 所有工作台复用

那等于默认承认当前 runtime 这张“没抄准的脸”可以继续当母版。

这会导致两个问题：

1. 错的视觉结果被进一步技术化、结构化
2. 用户手调出来的正确基线被永久降级成“参考图”

所以这时候先抽象，不是升级，而是把错误固化。

### 5.2 因为 Phase 1 的目标是“像不像”，不是“优不优雅”

在当前阶段，最先要恢复的验收标准不是：

- 代码够不够通用
- presenter 命名够不够好
- 数据模型是不是已经抽干净

而是：

- 运行时 UI 和用户手调 prefab 到底像不像

只要这一点没先站稳，后面的增强和抽象全都会建立在错误脸面上。

### 5.3 因为只有先把 formal-face 接回 runtime，后续增强才有稳定基座

Phase 2 的增强项，例如：

- 自适应
- 日历撕页
- 滚动链
- 按钮/进度状态机
- 离台小进度
- 固定锚定

都不是凭空发生的。

它们必须附着在一个已经被用户接受的视觉壳上。

如果这个壳本身就是偏的，那么：

- 每做一条增强，都可能继续偏
- 每做一次“修正”，都可能再次伤到视觉基线

因此正确顺序只能是：

1. 先把 formal-face 接回 runtime
2. 再在这张对的脸上做行为增强
3. 最后再总结哪些东西是通用的

---

## 6. 为什么正确答案是“视觉 prefab / 行为代码 / 差异数据”

这不是一句口号，而是对 3 类不同变化频率、不同负责人的内容做解耦。

### 6.1 视觉 prefab 层

这层负责：

- 层级结构
- RectTransform
- 锚点
- 字体 / 字号 / 留白 / 边距
- 背景、纸张、双页结构、按钮底板
- 左右栏骨架
- Viewport 外壳和滚动容器的固定视觉框架

为什么必须放 prefab：

- 这是用户会亲手精修的层
- 这是最需要 Inspector 可见、可拖、可微调的层
- 这是最怕被代码重新“猜一遍”的层

### 6.2 行为代码层

这层负责：

- 自适应布局重算
- 内容灌入后的 rebuild
- ScrollRect / Mask / Viewport 的运行时驱动
- 撕页动效
- 进度条与制作按钮状态机
- 离台小进度刷新
- 固定锚定与不漂的定位算法

为什么必须放代码：

- 这些都依赖运行时状态
- 它们需要随着文案长度、材料数量、制作状态、玩家位置实时变化
- 它们不能靠静态 prefab 自动完成

### 6.3 差异数据层

这层负责：

- 配方
- 材料列表
- 数量范围
- 工作台类型差异
- 标题、副标题、说明文案来源
- 某些工作台是否有额外状态、额外模块

为什么必须独立成数据：

- 不同工作台不应复制一份 UI 脚本
- 不同工作台也不应复制一份 prefab 再把业务文案写死进去
- 只有把差异收进数据，模板和行为才有资格真正复用

### 6.4 为什么这三层缺一不可

如果只有 prefab：

- 静态样式可以保住
- 但自适应、状态机、滚动链、离台进度全不够

如果只有代码：

- 行为能跑
- 但用户手调 formal-face 失去权威性，事故会反复

如果只有一套通用数据/抽象：

- 既保不住正式视觉面
- 也承不住运行时行为

所以正确答案不是三选一，而是：

**视觉 prefab / 行为代码 / 差异数据**

---

## 7. 为什么顺序必须是“先抄 -> 再稳 -> 再抽”

### 7.1 先抄

目的不是机械复刻，而是先恢复唯一权威视觉基线。

这一步的核心问题只有一个：

- runtime UI 能不能先和手调 prefab 对齐

这一阶段不应偷渡：

- 额外增强
- 额外抽象
- 长期模板化

### 7.2 再稳

在 formal-face 已经被接回 runtime 之后，才开始补：

- 自适应
- 滚动链
- 撕页
- 状态机
- 离台小进度
- 固定锚定

这一步叫“稳”，不是因为抽象更漂亮，而是因为：

- 用户先看到对的脸
- 再看到这张脸在复杂状态下仍然不崩

### 7.3 再抽

只有当前两步已经被用户接受，才允许做模板化。

因为此时抽出来的才是：

- 被验证过的视觉模板
- 被验证过的行为模式
- 被验证过的差异边界

否则抽出来的只是“带着错误 formal-face 的通用母版”。

---

## 8. 用户 6 条需求的分层与分阶段归属

本节的目标，是防止后续再次把“视觉基线问题”和“行为增强问题”混成一锅。

| 用户需求 | 主要归属层 | 所属阶段 | 原因 |
|---|---|---|---|
| 1. 文字变多后不能重叠，必须自适应 | 行为代码层为主，视觉 prefab 层提供容器骨架 | Phase 2 | 这不是先把脸抄准的问题，而是内容注入后的动态布局与重排问题；但前提是 Phase 1 先把容器、间距、壳体比例复刻对。 |
| 2. 翻页要像日历撕页，前页掀起时下面就是下一页 | 视觉 prefab 层 + 行为代码层 | Phase 2 | 双页骨架、页角、纸张层级应由 prefab 确立；真正的撕页动画、前后页切换与状态驱动属于代码。 |
| 3. RecipeColumn / Viewport / Materials 现在看不到或不对，且要可滑动、自适应 | Phase 1 先恢复正确视觉框架，Phase 2 修行为链；层上属于 prefab + 代码 + 数据共同参与 | Phase 1 + Phase 2 | 先要确认 prefab 中可视区、层级、外观、尺寸是否对，再修 ScrollRect/Mask/Content 生成与数据灌入。 |
| 4. 右侧制作按钮与进度条要做成状态机 | 行为代码层为主，差异数据层提供进度/队列信息，prefab 提供状态外观承载 | Phase 2 | 这是典型运行时状态问题，不应挤进 Phase 1；但按钮底板、进度条背景、悬浮样式槽位需要 prefab 提前提供承载。 |
| 5. 离开工作台后持续显示悬浮小进度 | 行为代码层 + 差异数据层，必要时辅以单独 prefab 样式模板 | Phase 2 | 这明显是新行为，不是 simple formal-face 复刻；它依赖当前制作项、进度、显示时机和世界锚定。 |
| 6. UI 要固定在工作台附近，不要漂，只保留可接受的切边/上下转换 | 行为代码层为主，prefab 提供基础 pivot / anchor / offset | Phase 2 | 这属于定位和跟随逻辑；视觉层只能提供初始锚定语义，真正是否漂、如何稳，必须由代码控制。 |

### 8.1 关于第 3 条的特别说明

第 3 条是最容易混相位的一条。

它同时包含两种问题：

1. 视觉正式面是否已经复刻对
2. 运行时滚动链和内容生成是否正确

所以它不是单纯 Phase 1，也不是单纯 Phase 2。

正确处理方式是：

- Phase 1 先恢复它“应该长什么样”
- Phase 2 再修它“为什么现在显示不出来、滚不起来、内容撑不开”

---

## 9. 三阶段正式路线

## Phase 1：基线复刻

### 目标

让 `day1` runtime UI 先与用户手调 prefab 对齐。

### 本阶段必须解决什么

1. runtime 创建链不再绕开正式 prefab
2. PromptOverlay 和 WorkbenchOverlay 的正式视觉壳回到 prefab 权威源
3. 运行时默认位置、尺寸、字体、字号、留白、层级，先以手调 prefab 为准
4. 形成 prefab 与 runtime 的差异矩阵，并把差异压到最小

### 本阶段不能偷渡什么

1. 不能把“增强体验”包装成“顺手一起做”
2. 不能先做 binder/provider 通用化实现
3. 不能一边说“先抄”，一边继续在 C# 里写新的视觉默认值
4. 不能把现在 runtime 这张错脸继续当成下一代母版

### 本阶段完成定义

不是“功能变多了”，而是：

- 用户手调出来的 formal-face 重新成为 runtime 的视觉真源
- 用户再对 prefab 微调时，runtime 不再天然失联

## Phase 2：体验增强

### 目标

在不破坏 Phase 1 视觉结果的前提下，补齐用户那 6 条里所有动态体验项。

### 本阶段必须解决什么

1. 自适应布局
2. 双页日历撕页
3. ScrollRect / Viewport / Mask / Content 正确链路
4. 制作按钮与进度条状态机
5. 离台小进度
6. 固定锚定与不漂

### 本阶段不能偷渡什么

1. 不能为了增强体验，再把 prefab 正式面降级成“只参考一下”
2. 不能为了省事，又把壳重新塞回 `BuildUi()`
3. 不能以“我先做一个通用版”为名，绕开 Day1 当前这套已被用户精修过的 formal-face

### 本阶段完成定义

不是“能跑”，而是：

- 运行时行为已经附着在对的 formal-face 上
- 内容变多、状态变复杂、玩家离台、页面翻转时，这张脸仍然稳

## Phase 3：模板化

### 目标

把前两阶段已经被用户接受的结果，拆成真正可复用的长期 SpringUI 路线。

### 本阶段必须解决什么

1. 哪些视觉骨架对所有工作台共用
2. 哪些行为模式对所有工作台共用
3. 哪些字段必须走差异数据
4. 哪些逻辑仍应保留 Day1 专属适配

### 本阶段不能偷渡什么

1. 不能拿未抄准的 Day1 错版做母版
2. 不能把 Day1 的剧情、任务、阶段逻辑直接焊死进通用层
3. 不能把一切差异都推回代码分支判断

### 本阶段完成定义

不是“提取了几个基类”，而是：

- 新工作台能复用同一套视觉模板
- 同一套行为层
- 只通过差异数据和少量业务适配表达不同内容

---

## 10. 哪些路线现在明确不能走

### 10.1 不能继续纯 `BuildUi()`

理由：

- 这会继续让 runtime 自己再猜一遍 UI
- 用户手调 formal-face 永远回不来
- 每个工作台都可能重复同类事故

### 10.2 不能把 prefab 继续当“旁边摆着的参考图”

理由：

- 当前阶段它不是参考图
- 它是用户已经亲手验过的视觉结果
- 如果它不重新成为 runtime 真源，Phase 1 就没完成

### 10.3 不能现在直接跳进 binder / provider / 通用模板化实现

理由：

- 当前 runtime 这张脸还没抄准
- 现在抽象，只会把错脸模板化

### 10.4 不能走“纯 prefab 就够了”

理由：

- 自适应、撕页、滚动链、状态机、离台进度、固定锚定都需要代码

### 10.5 不能把用户 6 条体验增强混进 Phase 1 冒充“基线复刻”

理由：

- 这样会再次模糊验收边界
- 最后既说不清“像不像”，也说不清“稳不稳”

### 10.6 不能拿当前错版 runtime 直接推广到所有工作台

理由：

- 这会把 Day1 当前的问题从单点事故扩大成系统级污染

### 10.7 不能把视觉参数继续抄回 C# 常量当长期方案

理由：

- 这会形成新的隐藏双源
- 用户以后再次调 prefab 时，runtime 又会失联

---

## 11. 推广到所有工作台的长期路线判断

在 Phase 1 和 Phase 2 过审后，SpringUI 的长期路线应是：

### 11.1 共用视觉模板

例如：

- 标准工作台面板骨架
- 左列表 / 右详情 / 底部操作区
- 标准 Viewport / Scroll 容器
- 标准按钮底板与进度槽
- 标准世界锚定悬浮小卡

### 11.2 共用行为层

例如：

- 内容注入后的布局刷新
- 列表行生成与选择联动
- 材料需求区刷新
- 数量区与进度区状态刷新
- world anchor 稳定定位
- 双页翻页或其他通用动效驱动

### 11.3 差异数据与少量业务适配

例如：

- recipe 集合
- 文案
- 特殊按钮状态
- 制作规则
- 某些工作台是否支持队列
- 某些工作台是否显示离台进度
- 与剧情或任务系统的特定连接点

也就是说，未来“所有工作台都走这套 UI 模式”并不是：

- 把 `SpringDay1WorkbenchCraftingOverlay.cs` 整个复制出去

而是：

- 用 Day1 先做出一套被用户接受的 SpringUI 模板链
- 再让别的工作台吃这套模板、行为和数据接口

---

## 12. 当前阶段结论

一句话结论：

**这次 `spring-day1` UI 问题的本体，不是抽象能力不足，而是用户手调 prefab 这个唯一真视觉基线没有重新接回 runtime；因此当前正确路线只能是“先抄 -> 再稳 -> 再抽”，其长期答案也必须是“视觉 prefab / 行为代码 / 差异数据”，而不是继续纯 `BuildUi()`、纯 prefab、或立刻跳进 binder/provider 通用化。**

---

## 13. 过审前约束

在本文件过审前：

1. 不进入实现
2. 不把 Phase 2、Phase 3 偷渡成“顺手一起做”
3. 不再把 formal-face 逐项复刻偷换成“精神上参考它”
4. 不再让 runtime UI 脱离用户手调 prefab 独立生长

这是进入实现前的唯一有效技术路线基线。
