# 03_遮挡现状核实与差异分析（Codex视角）

## 1. 审计范围与方法

本轮不是继续扩写旧遮挡报告，而是把旧稿、历史文档、当前代码、Prefab、场景和 Unity 现场重新对齐，回答一个更基础的问题：

> 现在 Sunset 里的遮挡系统，到底真实长什么样，哪些结论还能信，哪些已经漂移了？

本轮核实范围包括：

- 线程目录既有分析：
  - `遮挡系统代码索引与调用链.md`
  - `遮挡-导航-命中一致性与性能风险报告.md`
- 历史工作区与 Docx 文档：
  - `.kiro/specs/云朵遮挡系统/*`
  - `Docx/分类/遮挡与导航/*`
  - `Docx/分类/树木/*`
  - `Docx/分类/交接文档/树林遮挡核心系统优化交接文档.md`
- 当前真实实现：
  - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
  - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
  - `Assets/YYY_Scripts/Controller/TreeController.cs`
  - `Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs`
  - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementPreview.cs`
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
  - `Assets/Editor/BatchAddOcclusionComponents.cs`
- 现场配置：
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/222_Prefabs/Tree/M1.prefab`
  - `Assets/222_Prefabs/Tree/M2.prefab`
  - `Assets/222_Prefabs/Tree/M3.prefab`
  - 代表性石头、房子、箱子 Prefab
  - 相关 `.meta` 与纹理可读配置
- Unity 只读复核：
  - 活跃场景
  - Console
  - 组件计数
  - EditMode 测试

当前判断基线：

- 这是只读审计，不对场景、Prefab、Inspector 做写入修改。
- 旧资料只能当线索，不能直接当现状。
- 只要旧结论与当前代码 / YAML / Unity 现场冲突，就以当前真实实现为准。

## 2. 先纠正三个前提

### 2.1 用户指定的遮挡检查工作区原本不存在

`D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\遮挡检查` 在本轮开始前并不存在。  
这意味着“遮挡检查”在当前全面重构总工作区里还没有自己的现状基线，线程旧稿并没有被工作区文档承接。

### 2.2 线程旧稿里的脚本 GUID 映射写反了

通过读取当前 `.meta` 文件，正确映射应为：

| 脚本 | 正确 GUID |
|------|-----------|
| `OcclusionManager.cs` | `4a3f1c967ec52c946a301ffbacd49b6c` |
| `OcclusionTransparency.cs` | `9b41652a450cc9447abb94ac5ce72c1a` |
| `DynamicSortingOrder.cs` | `45a3d1d4bd5a8744aa9ed982679f875a` |

这不是小笔误。因为线程旧稿后面对 Scene / Prefab 挂载的归因会直接受这个映射影响。

### 2.3 旧文档和当前实现已经明显漂移

旧资料里反复出现的这些说法，当前都不能直接当现状：

- `Bounds.Contains` 仍是遮挡主判定
- Y 预过滤仍在主链里
- `affectChildren`
- `occlusionTags`
- `upwardOffset`
- `CleanInvalidOcclusionComponents.cs`
- `BatchApplyTreeMaterial.cs`

其中最直接的证据是：`Assets/Editor/BatchAddOcclusionComponents.cs` 还在写 `affectChildren` 与 `occlusionTags`，但当前 `OcclusionTransparency` 脚本里已经没有这些字段。

## 3. 我确认的当前真实系统结构

### 3.1 遮挡主链仍然是“管理器调度 + 组件执行”

当前遮挡主链没有被废弃，核心结构仍然成立：

1. `OcclusionTransparency` 在启用时向 `OcclusionManager` 注册。
2. `OcclusionManager.Update()` 定时进入检测。
3. 管理器逐个处理 `registeredOccluders`。
4. 组件侧通过 `SetOccluding()`、`SetCanBeOccluded()`、像素采样等能力执行透明效果。

这一层旧稿大方向没错，但它漏掉了“当前集合到底是按组件计数，还是按物理对象计数”这个会决定风险级别的细节。

### 3.2 树木、放置预览、导航刷新都是真联动，不是纸面集成

当前系统并不是孤立的“一个透明脚本”：

- `TreeController` 会在阶段变化时调用 `SetCanBeOccluded(...)`
- `PlacementPreview` 会向 `OcclusionManager` 传 `previewBounds`
- `TreeController` 状态变化还会触发 `NavGrid2D.OnRequestGridRefresh`

换句话说，遮挡问题不是纯渲染问题，而是和树状态、可走区域、交互命中共同组成一条真实生产链。

### 3.3 Unity 现场与代码现场一致

本轮用 Unity MCP 复核到的现场事实如下：

- 活跃场景：`Primary`
- 路径：`Assets/000_Scenes/Primary.unity`
- `isDirty = false`
- Console 当前有 1 条与 `Assets/Editor/NPCPrefabGeneratorTool.cs` 相关的 warning
- 组件计数：
  - `OcclusionManager = 1`
  - `OcclusionTransparency = 44`
  - `TreeController = 20`
- `OcclusionSystemTests` EditMode：11/11 通过

这些事实和 YAML / Prefab / 代码核实得到的判断是相互吻合的。

## 4. 我认为最关键的现状差异

### 4.1 `Primary.unity` 中的真实参数与旧稿不同

`Primary.unity` 当前 `OcclusionManager` 参数块显示：

- `occludableTags = Interactable / Building / Tree / Rock / Buildings / Placed`
- `useOcclusionRatioFilter = 1`
- `minOcclusionRatio = 0.4`
- `rootConnectionDistance = 1.5`
- `enableSmartEdgeOcclusion = 1`

这意味着旧资料里常出现的 `2.5` 根连通距离，并不是当前主场景真实值。

### 4.2 树 Prefab 普遍存在父/子双 `OcclusionTransparency`

这是我认为本轮最关键的发现。

`Tree/M1.prefab`、`M2.prefab`、`M3.prefab` 都能命中两次 `guid: 9b41652a450cc9447abb94ac5ce72c1a`，分别挂在：

- 子物体 `Tree`
- 父物体 `M1/M2/M3`

同时，父节点和子节点都带 `Tree` 标签。

这说明管理器眼里的“遮挡对象数”并不等于“物理树数量”。

### 4.3 `TreeController` 只会控制当前节点那一份组件

`TreeController` 内部缓存方式是：

```csharp
occlusionTransparency = GetComponent<OcclusionTransparency>();
```

这会直接带来一个很具体的后果：

- 阶段切换时 `SetCanBeOccluded()` 只会命中当前挂脚本的节点组件
- 父节点上那份 `OcclusionTransparency` 不会被同步关闭

因此，“树苗 / 树桩不应遮挡”的规则在当前结构下并不稳固，父组件很可能继续参与遮挡判定。

### 4.4 单棵物理树就可能触发“多树重叠保底透明”

`OcclusionManager` 当前这些集合都是：

- `HashSet<OcclusionTransparency> registeredOccluders`
- `HashSet<OcclusionTransparency> currentForest`

再往下看：

- `FindConnectedForest()` 是按 `OcclusionTransparency` 组件走图搜索
- `HandleForestOcclusion()` 里存在 `overlappingTreeCount >= 2` 的保底逻辑

如果一棵树自身就带父/子两份遮挡组件，那么它不是“可能影响判断”，而是直接具备把单棵树误算成“至少两棵重叠树”的结构条件。

这会动摇旧资料里对“树林整体透明”正确性的默认信任。

### 4.5 像素采样在树上是真开着的

旧稿把像素采样更多当成可选能力来谈，但当前树 Prefab 现实是：

- `usePixelSampling: 1`
- 树纹理 `Size_02.png.meta` 为 `isReadable: 1`
- `OcclusionTransparency` 内部会真实进入 `GetPixel` 路径

这说明性能风险不是理论上的“如果打开会怎样”，而是当前项目配置下已经成立的常态路径。

### 4.6 点击交互与工具命中仍然是双标准

当前两条链的边界基准不同：

- `PlayerToolHitEmitter` 使用 `GetColliderBounds()`
- `GameInputManager` 的右键资源节点分支使用 `GetBounds().Contains(world)`

树和箱子都实现了 `GetBounds / GetColliderBounds`。  
所以“看起来点得到”和“挥工具打得到”并不一定落在同一边界规则上。

旧线程报告已经提出过这类风险，但这次核实后我认为它不只是体验层瑕疵，而是当前资源节点交互统一性仍未真正建立的证据。

### 4.7 编辑器工具和测试都落后于真实系统

#### 编辑器工具

`Assets/Editor/BatchAddOcclusionComponents.cs` 仍在操作不存在的字段：

- `affectChildren`
- `occlusionTags`

它未必会在编译期报错，但在实际使用时已经不值得再被当作可靠的批处理入口。

#### 测试

`Assets/YYY_Tests/Editor/OcclusionSystemTests.cs` 虽然 11/11 通过，但我核完后认为它只能证明两件事：

1. 旧数学片段没有明显坏掉
2. 一些旧前提下的计算仍成立

它并不能证明：

- 双组件树 Prefab 结构正确
- `TreeController -> SetCanBeOccluded()` 联动真实有效
- 单棵树不会误触树林保底逻辑
- 当前 `Primary.unity` 参数组合下的主链没有回归

## 5. 从 Codex 视角的风险重排

### P0：树 Prefab 的父/子双组件结构

这是我认为目前最应该被优先处理的点，因为它同时影响：

- 树阶段遮挡开关
- 树林连接判定
- 遮挡对象计数
- 现场调试理解

如果对象粒度本身就是错的，后面继续调树林阈值、采样参数、透明策略，都会像在歪地基上修墙。

### P0：单棵树可能误触“多树重叠”保底逻辑

这一条是上条的直接放大器。  
它会让玩家实际体验到的“整林透明”与设计想表达的“树群边缘智能透明”出现偏差。

### P1：命中判定仍是双标准

这会继续制造：

- 点得到但打不到
- 打得到但点击交互不一致
- 同类资源节点之间的交互边缘不统一

它已经不是小优化项，而是系统一致性问题。

### P1：像素采样带来的实时成本

当前树上真实开启像素采样，而遮挡管理器又是周期性全量遍历。  
这会让“树量增加”与“检测精度提高”叠加成真实的 CPU 压力。

### P1：工具链与测试链已经失真

当批处理工具写的是旧字段、测试验证的是旧前提时，团队会被一种假象误导：

> 文档有、工具有、测试也绿，所以系统应该是稳的。

本轮核实后的结论恰恰相反：这些外层保障已经部分失真。

### P2：文档与 GUID 事实漂移

它短期不会直接造成运行时 bug，但会持续放大沟通成本，并误导后续审计或整改路线。

## 6. 我对后续整改顺序的建议

如果下一阶段要从“审计”转入“整改设计”，我建议顺序不要反过来。

### 第一步：先解决对象粒度问题

先回答：

- 树到底应该保留一份还是两份 `OcclusionTransparency`？
- 父节点那份组件是否还有正当用途？
- 如果保留双组件，`TreeController` 为什么只控制一份？

在这一步之前，不建议直接微调树林连通阈值。

### 第二步：做一个最小复现实验

目标不是立刻修，而是验证两个关键问题：

1. 单棵双组件树是否会触发 `overlappingTreeCount >= 2`
2. 树苗 / 树桩阶段父组件是否仍然参与遮挡

只有把这两个现象变成“可复现 / 可否定”的证据，后面的设计才不会停留在阅读推断。

### 第三步：统一资源节点命中规则

要尽早明确项目最终想要的是：

- 视觉完整交互优先（Sprite bounds）
- 物理可达性优先（Collider bounds）
- 还是两条链各自保留，但要有显式说明和补偿机制

### 第四步：再补工具、测试和文档

整改真正落地前，至少要同步修复三层外围支撑：

- 编辑器批处理工具
- EditMode 测试覆盖
- 工作区 / 线程 / 设计文档

否则下一轮接手时还会重复今天这轮“旧稿可信度需要重新核对”的成本。

## 7. 本轮我最信任的证据清单

### 代码事实

- `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
  - `Awake()` 用 `GetComponentsInChildren<SpriteRenderer>()`
  - `mainRenderer = childRenderers[0]`
  - `SetCanBeOccluded()` 只影响当前组件
- `Assets/YYY_Scripts/Controller/TreeController.cs`
  - `occlusionTransparency = GetComponent<OcclusionTransparency>()`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
  - 用 `HashSet<OcclusionTransparency>` 管注册与树林集合
  - `FindConnectedForest()`
  - `HandleForestOcclusion()`
  - `overlappingTreeCount >= 2`
- `Assets/YYY_Scripts/Combat/PlayerToolHitEmitter.cs`
  - 使用 `GetColliderBounds()`
- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
  - 使用 `GetBounds().Contains(world)`

### Prefab / Scene 事实

- `Assets/222_Prefabs/Tree/M1.prefab`
- `Assets/222_Prefabs/Tree/M2.prefab`
- `Assets/222_Prefabs/Tree/M3.prefab`
- `Assets/000_Scenes/Primary.unity`

### Unity 现场事实

- `Primary` 当前已加载且未 dirty
- Console 仅见 1 条 NPC 编辑器工具 warning
- `OcclusionSystemTests` EditMode 11/11 通过
- `OcclusionTransparency = 44`、`TreeController = 20`

## 8. 本轮结论

如果只用一句话概括这次核实，我的判断是：

> 遮挡系统不是“坏掉了”，而是“外层文档和旧稿看起来完整，但底层对象粒度、判定标准和验证手段已经与当前真实实现脱节”。

当前最危险的不是某个单点 bug，而是团队很容易在一个已经漂移的认知底板上继续做判断。

所以这轮审计真正完成的事情不是“宣布系统有问题”，而是把后续整改必须先站稳的地面重新找出来了。
