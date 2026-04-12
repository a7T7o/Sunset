# 遮挡透明与云朵阴影系统 - 开发记忆

## 模块概述

遮挡透明系统和云朵阴影系统，包括：
- 遮挡透明检测（玩家被物体遮挡时物体变透明）
- 树木渐变透明效果
- 树林连通算法（Flood Fill）
- 云朵阴影移动效果
- 天气联动

## 当前状态

- **完成度**: 95%
- **最后更新**: 2025-12-17
- **状态**: ✅ 已完成

## 核心设计

### 遮挡透明系统
- 使用玩家 Collider 中心点作为检测基准
- Bounds.Intersects 检测（性能提升 20 倍）
- 0.1 秒检测间隔
- 标签过滤：Trees, Buildings, Rocks

### 树林连通算法
- Flood Fill 算法查找连通树木
- 连通条件：树根距离 < 2.5m 或 树冠重叠 > 15%
- 最大搜索深度：50 棵树
- 最大搜索半径：15 米
- 边界扩展：2 米

### 树木渐变透明
- 根部透明度：0.8
- 树干透明度：0.5
- 树叶透明度：0.3
- 阴影保持不透明

### 云朵阴影系统
- 晴天/多云：显示云影
- 阴天/雨天/雪天：隐藏云影
- 最大云朵数量：32 个
- CloudShadow 排序层级
- 对象池管理

## 会话记录

*暂无详细会话记录 - 从已有规划文档导入*

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 使用 Collider 中心点检测 | 比脚底位置更准确 | 2025-12-17 |
| Flood Fill 树林连通 | 整片树林同时透明，导航更清晰 | 2025-12-17 |
| 垂直渐变透明 | 视觉效果更自然 | 2025-12-17 |
| 天气联动云影 | 增强天气系统表现力 | 2025-12-17 |

## 相关文件

### 规划文档
| 文件 | 说明 |
|------|------|
| `.kiro/specs/occlusion-cloud-system/requirements.md` | 需求文档 |
| `.kiro/specs/occlusion-cloud-system/design.md` | 设计文档 |
| `.kiro/specs/occlusion-cloud-system/tasks.md` | 任务清单 |

### 核心代码
| 文件 | 说明 |
|------|------|
| `Assets/Scripts/Service/Rendering/OcclusionManager.cs` | 遮挡管理器 |
| `Assets/Scripts/Service/Rendering/OcclusionTransparency.cs` | 遮挡透明组件 |
| `Assets/Scripts/Service/Rendering/CloudShadowManager.cs` | 云朵阴影管理器 |
| `Assets/Shaders/VerticalGradientOcclusion.shader` | 渐变透明着色器 |
| `Assets/Shaders/CloudShadowMultiply.shader` | 云影混合着色器 |

### 详细文档
| 文件 | 说明 |
|------|------|
| `Docx/分类/遮挡与导航/000_遮挡与导航系统完整文档.md` | 完整文档 |

---

## 会话补记（2026-04-03）

### 背景
用户要求对 `D:\Unity\Unity_learning\Sunset\.kiro\specs\云朵遮挡系统` 和 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Z_光影系统` 做一次彻底扫盘，重点不是继续开发，而是先弄清楚需求、历史完成度，以及当前 live 场景里到底哪些功能真的开着。

### 当前主线与本轮定位
- **当前主线目标**：厘清云朵/遮挡/光影两套系统的真实 live 状态，为后续是否继续接线或修复提供判断基线
- **本轮子任务**：只读审计 `云朵遮挡系统`
- **本轮服务于什么**：给用户一份可直接下判断的“现状盘点”，避免把“仓库里有代码”误判成“场景里已经启用”
- **恢复点**：如果后续进入实现，应先决定是只恢复云影表现，还是连同 `Z_光影系统` 一起做重新接线

### 本轮完成
1. 读取本工作区 `memory.md` 与 `old/requirements.md`、`old/design.md`、`old/tasks.md`，确认旧 spec 的目标能力与历史口径。
2. 对照当前仓库脚本、材质和场景，确认 `OcclusionManager.cs`、`OcclusionTransparency.cs`、`CloudShadowManager.cs` 仍在项目内。
3. 结合 Unity MCP 只读现场确认 `Primary` 当前真实挂载：
   - `CloudShadowManager` 1 个
   - `OcclusionManager` 1 个
   - `OcclusionTransparency` 105 个
   - `DynamicSortingOrder` 4 个
4. 核出当前真正开启的是“遮挡透明链”，不是“云影显示链”。

### 关键结论
- **当前 live 真正开启**：
  - 基于玩家中心 / Bounds 的遮挡透明
  - 标签过滤
  - 遮挡占比过滤
  - 树林整体透明
  - 智能边缘遮挡
  - 预览遮挡联动
  - 砍树高亮联动
- **当前 live 明确未开启**：
  - 标签自定义参数（`useTagCustomParams=false`）
  - 云影显示（`enableCloudShadows=false`）
  - 天气门控云影（`useWeatherGate=false`）
- **旧 spec 与 live 的已确认差异**：
  - 旧文档写树根连通距离 `2.5m`，当前 live 场景读数为 `1.5`
  - 旧文档把云影作为完成特性记录，但当前主场景里它是“对象在场、材质在场、默认关闭”

### 现场证据
- `Primary` live 参数读取到的 `OcclusionManager` 关键值：
  - `detectionRadius=8`
  - `detectionInterval=0.1`
  - `useTagFilter=true`
  - `useTagCustomParams=false`
  - `occludableTags=[Interactable, Building, Tree, Rock, Buildings, Placed]`
  - `sameSortingLayerOnly=true`
  - `useOcclusionRatioFilter=true`
  - `minOcclusionRatio=0.4`
  - `enableForestTransparency=true`
  - `rootConnectionDistance=1.5`
  - `maxForestSearchDepth=50`
  - `maxForestSearchRadius=15`
  - `enableSmartEdgeOcclusion=true`
- `TreeController.cs` 仍通过 `SetTreeOcclusionEnabled(true/false)` 调用 `OcclusionTransparency.SetCanBeOccluded`，说明树遮挡并非无条件常开，而是受树阶段和状态控制。
- `CloudShadowManager` 当前 live 参数显示：
  - `enableCloudShadows=false`
  - `useWeatherGate=false`
  - `previewInEditor=true`
  - `cloudMaterial=Assets/444_Shaders/Material/CloudShadow.mat`
  - `sortingLayerName=CloudShadow`

### 涉及文件
- `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
- `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
- `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
- `Assets/YYY_Scripts/Controller/TreeController.cs`
- `Assets/444_Shaders/Material/CloudShadow.mat`
- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Town.unity`

### 验证结果
- **静态代码 / 场景 YAML**：成立
- **Unity MCP live Inspector 只读读取**：成立
- **PlayMode / 用户体感验证**：未执行

### 遗留问题 / 下一步
- 如果后续要恢复云影，第一步不是重写系统，而是先决定是否只开启 `CloudShadowManager` 并重新校参数。
- 这轮仅确认“系统是否存在、是否接线、是否默认开启”，尚未验证玩家主观观感是否合格。

---

## 会话补记（2026-04-03 第二轮深化分析）

### 背景
用户进一步澄清：当前重点不是“云朵系统开没开”，而是要彻底说清楚他对这条线的真实核心需求、现状哪些只是结构存在、哪些体验已经失败，以及目前能确认的 bug / 方向性错误。

### 当前主线 / 本轮定位
- **当前主线目标**：把“遮挡透明”和“云影表现”拆开评价，避免把遮挡可用误报成“云朵系统整体可交付”
- **本轮子任务**：只读深挖 `云朵遮挡系统` 工作区、代码、场景 YAML、历史交接文档和素材本体
- **本轮服务于什么**：为后续“直接废弃当前云影路线”还是“仅保留遮挡系统并重做云影表现”提供判断锚点
- **恢复点**：如果后续继续推进，应先冻结“遮挡系统继续保留”为既定前提，再单独决定云影是否重做

### 新增完成
1. 对照旧需求、交接文档和开发进度报告，确认云影这条线长期存在“文档口径高估完成度”的问题：
   - `.kiro/specs/云朵遮挡系统/old/requirements.md` 写“云朵阴影系统已完成核心功能”
   - 但 `Docx/分类/遮挡与导航/遮挡透明系统-开发进度报告.md` 同时又写“等待素材配置 / 场景配置 / 用户测试”，总完成度只有 `43%`
2. 直接检查 `Primary.unity` 与 `Town.unity` 的 `CloudShadowManager` 序列化数据，确认不是“没接线”，而是“接了 6 张 sprite 和材质，但显式关闭总开关与天气门控”：
   - `enableCloudShadows=0`
   - `useWeatherGate=0`
   - `previewInEditor=1`
   - `maxClouds=8`
3. 深读 `CloudShadowManager.cs` 与 shader / material，确认“生成了不消失”有代码级根因：
   - `SimulateStep()` 在 `!enableCloudShadows`、天气不允许、`cloudSprites` 为空时直接 `return`
   - 没有先 `DespawnAll()`
   - 只有 `density<=0` 或 `maxClouds<=0` 才会清场
4. 直接查看 `Cloud_001.png` 与其 `.meta`，确认当前云影素材本身就是大块深灰剪影，且 `filterMode=Point`，足以支持“根本不像云”的主观反馈不是错觉。
5. 对照 `WeatherSystem.cs` 与 `CloudShadowManager.cs`，确认天气语义存在结构性漂移：
   - `CloudShadowManager` 自己维护 `Sunny / PartlyCloudy / Overcast / Rain / Snow`
   - `WeatherSystem` 只有 `Sunny / Rainy / Withering`
   - 冬天下雪在 `WeatherSystem` 里被编码成 `Withering`
   - 映射进入 `CloudShadowManager` 后又变成 `Overcast`
   - 因此 `PartlyCloudy` 与真正的 `Snow` 在项目主天气链中都没有稳定来源
6. 对照需求 / 测试 / 代码，确认云影循环语义已经漂移：
   - 旧需求要求“移出边界后传送到对侧边界”
   - 测试也按“传送”断言
   - 但当前实现实际上是“超界后销毁回池，再在边缘重生”
7. 确认遮挡系统本体仍是当前 live 真正成立的部分：
   - 玩家 `Collider.bounds.center` 检测
   - 遮挡占比过滤
   - 树林 Flood Fill 连通
   - 智能边缘遮挡
   - 预览遮挡联动
   - 树阶段控制 `SetTreeOcclusionEnabled()`

### 新增关键结论
- **用户对“云朵 / 遮挡系统”的真实需求已经分裂成两层**：
  1. **遮挡透明层**：玩家在树林、建筑、放置预览等复杂场景里始终看得清自己，不迷路，不出戏
  2. **云影表现层**：世界上方扫过的暗影要像真的云，不残留、不突兀、不像硬邦邦灰块
- **当前真正满足的主要是第一层，不是第二层**。
- **云影这条线不是“小修参数就能过线”的状态**：
  - 素材语言不过关
  - 运行逻辑存在残留 bug
  - 天气语义和主系统不一致
  - 测试覆盖不到真实体验和现场残留问题
- **最合理的方向锚点**：
  - 保留并继续使用遮挡透明系统
  - 不把当前精灵云影当成“差一点就能用”的方案
  - 如果未来还要云影，应把它当成一条重做的表现方案，而不是当前 `CloudShadowManager` 的简单复活

### 本轮新增涉及文件
- `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
- `Assets/YYY_Scripts/Service/WeatherSystem.cs`
- `Assets/444_Shaders/Shader/CloudShadowMultiply.shader`
- `Assets/444_Shaders/Material/CloudShadow.mat`
- `Assets/Sprites/0_Tool & Weapon 'Anim/Sheet/ZZZ_CloudShadow/Cloud_001.png`
- `Assets/Sprites/0_Tool & Weapon 'Anim/Sheet/ZZZ_CloudShadow/Cloud_001.png.meta`
- `Assets/YYY_Tests/Editor/CloudShadowSystemTests.cs`
- `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
- `Docx/分类/遮挡与导航/遮挡透明系统-开发进度报告.md`
- `Docx/分类/交接文档/遮挡透明 + 云朵阴影 交接文档2.0.md`

### 验证状态
- **代码 / 场景 YAML / 文档交叉印证**：成立
- **素材肉眼检查**：已做（仅静态查看 PNG，不涉及运行时）
- **PlayMode / 用户终验**：未做

### 遗留问题 / 下一步
- [ ] 若继续云影，不建议从“重新打开开关”开始，而应先重定“目标视觉语言”和天气语义
- [ ] 若仅求当前主线稳定，可直接把“遮挡透明保留、云影暂时判废”作为阶段结论

---

## 会话补记（2026-04-03 第三轮需求锚定）

### 背景
用户继续追问“你到底有没有把 memory 通读并真正理解我的核心需求”，并再次强调：云影不是“暂时关掉的完成系统”，而是因为“不像云、效果垃圾、还有生成了不消失等 bug”而被否掉的失败方案。

### 当前主线 / 本轮定位
- **当前主线目标**：把 `云朵遮挡系统` 从“旧文档里好像 95% 完成”改写成“遮挡保留、云影判定失败”的真实基线
- **本轮子任务**：补读线程 memory、旧 spec、代码、测试、场景 YAML 和素材本体，确认残留 bug 与方向性错误
- **本轮服务于什么**：给后续“直接重做云影”还是“仅保留遮挡不再碰云影”提供更硬的判断依据
- **恢复点**：如果后续进入施工，不再从“把开关打开试试”开始，而是先决定是否彻底废弃当前云影路线

### 本轮新增完成
1. 通读当前线程记忆 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\云朵与光影\memory_0.md`，确认旧线程口径仍停在“现状盘点”，尚未充分吸收“体验失败、需求要重锚”的修正。
2. 进一步核实 `Primary.unity` 与 `Town.unity`：两张场景里虽然 `enableCloudShadows=0`，但都各自静态保存了 1 个激活中的 `CloudShadow` 子物体，父节点正是 `CloudShadowManager`。
3. 对照 `CloudShadowManager.cs` 与 `CloudShadowManagerEditor.cs`，确认“关掉云影后所有云影消失”只是 Inspector 文案，不是实际行为：
   - `SimulateStep()` 在 `!enableCloudShadows` / 天气不允许时直接返回，不会清场
   - Inspector 在总开关关闭时直接 `return`，也不会调用 `EditorDespawnAll()`
4. 结合 `previewInEditor=1`、`ExecuteAlways`、场景里保存的 `CloudShadow` 子物体，确认云影系统存在“编辑器预览残留污染场景”的高概率问题。
5. 重新核对云影测试与实现：测试仍在验证“越界传送到对侧边界”，但当前代码真实语义是“越界回池、稍后重生”，测试已经不能代表 live 行为。

### 本轮新增关键结论
- **“生成了不消失”不只是主观抱怨，代码和场景现场都能支撑**：
  - 关总开关不会主动清场
  - 天气不允许也不会主动清场
  - 场景里已经实际残留激活的 `CloudShadow` 子物体
- **当前云影方案的失败是多层叠加，不是单点参数问题**：
  - 素材语言失败
  - 生命周期清理失败
  - 天气语义漂移
  - 测试与实现漂移
- **更稳的项目结论**：
  - 遮挡透明继续保留
  - 当前精灵云影路线不再按“待调参数功能”处理，而应按“失败方案 / 待重做表现路线”处理

### 本轮新增涉及文件
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\云朵与光影\memory_0.md`
- `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
- `Assets/Editor/CloudShadowManagerEditor.cs`
- `Assets/YYY_Tests/Editor/CloudShadowSystemTests.cs`
- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Town.unity`

### 验证状态
- **线程 memory / 工作区 memory / 代码 / 场景 YAML / 素材交叉核对**：成立
- **PlayMode / 用户终验**：未做

### 遗留问题 / 下一步
- [ ] 若继续云影，下一轮应先定“是否彻底废弃当前 sprite + multiply 路线”
- [ ] 若仅求主线稳定，可直接把“云影判废、遮挡保留”写成阶段结论

---

## 会话补记（2026-04-03 第四轮落地方案）

### 背景
在用户认可“重新锚定需求与现状判断”的前提下，用户进一步要求直接给出落地方案，而不是继续做现状盘点。

### 当前主线 / 本轮定位
- **当前主线目标**：把“遮挡保留、云影失败”收成可执行方案
- **本轮子任务**：给出云影线的落地执行顺序、停损线、完成定义和验证口径
- **本轮服务于什么**：为后续真实施工提供起手顺序，避免误从“重新打开旧云影”开始
- **恢复点**：若后续真开工，第一刀先处理云影路线裁定与替代方案，不动遮挡主链

### 本轮新增结论
1. **云影不建议直接修旧方案**：当前 sprite + multiply 路线直接判定为失败方案，不按“小修参数”进入施工。
2. **云影推荐落地顺序**：
   - 第 0 步：冻结旧路线，明确 `CloudShadowManager` 仅作历史参考，不作为近期直接恢复目标
   - 第 1 步：先定新目标视觉语言与验收图
   - 第 2 步：重定天气状态模型和关闭/切天气时的清场语义
   - 第 3 步：再决定是否保留旧管理器壳，还是直接做新表现链
3. **当前最稳的阶段裁定**：
   - 遮挡透明继续保留并视为现役底座
   - 云影表现进入“判废 / 待重做”状态

### 建议的一刀完成定义
- 不再使用当前深灰硬剪影素材作为最终云影素材
- 关闭总开关 / 天气不允许 / 退出预览时，场景中不残留任何 `CloudShadow` 子物体
- 天气状态和显示语义与主 `WeatherSystem` 一致，不再存在 `PartlyCloudy / Snow` 无稳定来源的问题
- GameView 真实观感先过线，再谈是否接回 live

### 遗留问题 / 下一步
- [ ] 若继续云影，下一轮应先出“云影重做方向 spec”，不是先改 `CloudShadowManager`
- [ ] 若用户只想尽快止损，可直接把当前云影路线冻结，不进入实现

---

## 会话补记（2026-04-03 第五轮云影止血刀施工）

### 背景
用户允许直接开工，但边界明确收紧为：先做 `cloud-shadow-stopgap-no-primary`，只止血“旧云影关不掉 / 预览残留 / 无素材残留”这类生命周期 bug，不进入云影重做，也不触碰 `Primary.unity`。

### 当前主线 / 本轮定位
- **当前主线目标**：在不认可旧云影表现路线的前提下，先把最明显的残留 / 关不掉问题止血，避免失败方案继续污染场景和误导后续判断
- **本轮子任务**：修 `CloudShadowManager` 与其 Inspector 的清场语义，补最小回归测试
- **本轮服务于什么**：让“旧云影已判失败”这件事在工程现场也成立，而不是逻辑上说停、层级里还残留 `CloudShadow`
- **恢复点**：如果后续继续云影，不从这个旧 manager 继续堆表现，只把它当历史止血后的稳定壳

### 本轮新增完成
1. 已按 `Begin-Slice` 进入真实施工，slice 名为 `cloud-shadow-stopgap-no-primary`，own 路径限定在：
   - `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
   - `Assets/Editor/CloudShadowManagerEditor.cs`
   - `Assets/YYY_Tests/Editor/CloudShadowSystemTests.cs`
2. `CloudShadowManager.cs` 已补齐停用态清场链：
   - `SimulateStep()` 遇到总开关关闭 / 天气不允许 / 无可用 Sprite / 密度或数量为 0 时，统一先 `ClearCloudsForInactiveState()` 再返回
   - `OnDisable()` 现在无论运行时还是编辑器都会先清场，补上“组件停了但旧云影还挂着”的洞
   - 编辑器态新增 `DestroyEditorCloudObjects()`，会清掉 active / pool / manager 子层级中的 `CloudShadow`
   - `previewInEditor=false` 时，`EditorUpdate()` 会主动清空预览残留
3. `CloudShadowManagerEditor.cs` 已在 Inspector 总开关关闭分支主动调用 `manager.EditorDespawnAll()`，让文案“关闭后所有云影消失”和实际行为一致。
4. 继续补了一针更小但关键的止血：
   - 旧补丁最初只把“数组为空”视为无素材，漏掉了 Unity 常见脏配置“数组非空但元素全是 null”
   - 现在 `HasConfiguredCloudSprite()` 会把“全 null Sprite”也判成无素材并立即清场
   - `PickSprite()` 也会跳过 null 元素，避免半脏数组继续抽中空素材
5. `CloudShadowSystemTests.cs` 不再只做“把生产逻辑抄一遍”的 helper 测试，改成直接通过反射驱动 `Assembly-CSharp` 里的真实 `CloudShadowManager`，补了 4 条行为测试：
   - 组件禁用时清场
   - 总开关关闭时清场
   - 天气门控阻断时清场
   - 仅剩 null Sprite 引用时清场

### 本轮新增验证
- `validate_script`：
  - `CloudShadowManager.cs`：无 error，仅有历史级泛化 warning
  - `CloudShadowManagerEditor.cs`：无 error，仅有历史级泛化 warning
  - `CloudShadowSystemTests.cs`：0 warning / 0 error
- Unity Console：
  - 本轮收口后无新的编译错误
  - 仅见 1 条 MCP WebSocket 初始化 warning，可判为工具噪声
- `Tests.Editor` EditMode 整体回归：
  - 总数由 194 变为 195，说明本轮新增测试已被编入
  - 当前失败仍是既有的 5 条无关测试：
    - `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume`
    - `ScenePartialSyncToolTests.NormalizeSelectedPaths_ShouldRemoveDescendants_WhenAncestorAlreadySelected`
    - `SpringDay1DialogueProgressionTests.HealingAndEnergyPacing_RemainsBoundToDay1FormalSequence`
    - `SpringDay1InteractionPromptRuntimeTests.ProximityService_UsesTaskFirstOverlayCopyWhenDialogueBeatsInformalChatOnSameNpc`
    - `SpringDay1LateDayRuntimeTests.WorkbenchInteractable_ShouldStayQuietBeforeWorkbenchPhase`
  - 本轮未观察到 `CloudShadowSystemTests` 进入失败列表

### 本轮新增关键判断
- 这刀现在已经不只是“编辑器预览会清理”，而是把**运行时停用**和**null Sprite 脏配置**这两个真实漏口也补上了。
- 当前这刀仍然是**工程止血**，不是**体验完成**：
  - 它解决的是“旧云影关不掉 / 场景残留 / 错误素材配置还留旧影子”
  - 它没有解决“像不像云、好不好看、要不要重做表现路线”
- 当前最稳的阶段判断是：
  - **遮挡透明**：继续保留
  - **旧云影表现路线**：仍然判失败
  - **CloudShadowManager 当前状态**：可作为止血后的历史壳存在，但不该被当成最终方案

### 本轮涉及文件
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\CloudShadowManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Editor\CloudShadowManagerEditor.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\CloudShadowSystemTests.cs`

### 验证状态
- **静态验证 / EditMode 回归 / Console**：已做
- **GameView 真实观感 / 用户终验**：未做

### 遗留问题 / 下一步
- [ ] 若继续本线，下一轮应优先做一次 GameView / hierarchy 级手动验证，确认关闭总开关、禁用组件、关闭预览后三个入口都不会再残留 `CloudShadow`
- [ ] 当前仍未处理“同名 `CloudShadow` 子物体误删”的低概率风险，因为这刀坚持 stopgap，不扩大为对象来源重构
- [ ] 如果用户允许继续功能方案，不应顺手继续修旧云影表现，而应转去“新云影方向 spec / 目标画面”那一刀

### 本轮收口补记
- `Ready-To-Sync` 已真实尝试：
  - 先撞到 `.kiro/state/ready-to-sync.lock` 的 stale 锁超时
  - 参考项目既有先例，仅把 stale 锁改名让位后重跑
  - 第二次返回的已是真 blocker，不是工具误报
- 真 blocker 口径：
  - 当前线程历史 own roots `Assets/Editor`、`Assets/YYY_Tests/Editor`、`Assets/YYY_Scripts/Service/Rendering` 下还有大量与本刀无关的 remaining dirty / untracked
  - 因此这轮 **不能合法 sync**
- 当前 live 状态：
  - 已执行 `Park-Slice`
  - 状态为 `PARKED`
  - blocker 已写入 thread-state：`Ready-To-Sync blocked: own roots ... still contain historical remaining dirty/untracked outside this cloud-shadow stopgap slice`

### 2026-04-04（只读分析：Inspector 重排与“拉满后卡住”根因锚定）

- **当前主线目标**：不继续给旧云影表现抛光，先把 Inspector 交互重排方向和“数量/密度/强度/速度拉满后云会卡住”的逻辑高风险点说清楚
- **本轮子任务**：只读审 `CloudShadowManager.cs`、`CloudShadowManagerEditor.cs`、`CloudShadowSystemTests.cs`，给出最可落地的布局方案、根因优先级和最小修改建议
- **本轮服务于什么**：为后续最小代码修补定锚，避免继续在失败路线里盲修
- **恢复点**：如果下一轮继续真实施工，优先做 editor 布局重排 + 生成/清理链修复，不先碰 scene

#### 本轮稳定判断
1. 当前 `CloudShadowManagerEditor` 最大问题不是“缺控件”，而是**工具动作位置过低、长表单直出、没有按高频调参和低频配置分层**：
   - 顶部只有标题和总开关，真正高频操作按钮放在后段，导致每次刷新/重建都要长距离滚动
   - 当前外观、移动、区域、渲染、天气、随机、限制、操作全部平铺在同一纵向流里，缺少 foldout / toolbar / 紧凑行
2. 结合 Sunset 现有 inspector 习惯，最合适的重排不是“继续堆 box”，而是：
   - 顶部固定工具条：`刷新区域` / `立即重建` / `随机种子` / `清空`
   - 中间折叠成 4 组：`快速调试`、`区域`、`素材与渲染`、`联动与高级`
   - 高频量改成 slider / min-max slider，低频配置折叠，减少滚动
3. “拉满后卡住”最像**生成链逐渐生成失败，但旧云没有及时腾位**，不是单点“速度太大”：
   - `CleanupInvalidClouds()` 只按主轴单向越界销毁，完全不处理副轴漂移/滞留
   - `TrySpawnOneAtEdge()` 与 `IsOverlappingWithExisting()` 使用固定 `maxSpawnAttempts` + 固定最小间距；密度/数量/尺寸/速度一起拉高后，边缘很容易持续找不到可生成位置
   - `target` 被强制 `Clamp(..., 1, maxClouds)`，意味着 `density > 0` 时系统永远追求至少 1 朵云，调参空间被压扁
   - 当前测试只覆盖“停用态清理”，没有覆盖“高密度连续运行后生成失败/滞留/回收恢复”

#### 这轮不建议夸大的点
- 这只是**结构层分析**，不是体验过线判断
- 目前没有新增 GameView 证据，不能把“建议的 inspector 布局”说成用户一定会喜欢

#### 下一步建议
- 若继续本线，最小顺序应为：
  1. 先重排 Inspector：顶部工具条 + foldout + slider，减少长滚动
  2. 再修 `CleanupInvalidClouds()` / 生成失败恢复链
  3. 补 2-3 条针对“高密度连续运行”的 EditMode 测试
  4. 最后才做用户手调与观感判断

### 2026-04-04（真实施工：云影 Inspector 重排 + 高负载自愈）

**当前主线目标**:
- 主线目标：把旧云影路线先收成“更能调、更不容易在高负载下卡住”的可继续验证状态，而不是继续盲修表现
- 本轮子任务：落实用户点名的 4 件事：`大小改滑动条`、`刷新按钮上移`、`减少滚动`、`修数量/密度/速度拉满后卡住`
- 服务对象：为后续用户手调和真实 GameView 终验提供更像正式工具面的控制台，并先把最硬的生成/回收逻辑风险收干净
- 恢复点：若继续本线，下一步应优先做用户实机调参与 GameView 验证，不先扩到 scene / prefab

**本轮完成**:
1. 重做 `CloudShadowManagerEditor` 的 Inspector 结构：
   - 顶部固定 `启用云影` / `编辑器预览`
   - 顶部工具条固定为 `刷新区域` / `立即重建` / `随机种子` / `清空`
   - 高频参数收进默认展开的 `快速调试`
   - `scaleRange` 改成 `MinMaxSlider + 数值框`
   - 低频项拆到 `区域`、`素材与渲染`、`联动与高级` 折叠区，去掉原来一长串 box 的长滚动结构
2. 修补 `CloudShadowManager` 的高负载运行链：
   - `SimulateStep()` 改成“移动前清理 + 移动后再清理”，避免高速下越界云多挂一帧
   - 目标数量改为允许 `0..maxClouds`，不再把非零密度硬钉成至少 1 朵
   - 补云从“固定每次最多 1 朵”改成按缺口一次补多朵
   - 连续生成失败达到阈值后，允许回收最靠出口的旧云来腾位
   - 生成位置搜索加入逐步放宽间距的降级策略
   - 水平/垂直边缘补云时，副轴位置改为考虑 `halfWidth/halfHeight`，不再把整朵云半截塞出边界
   - 运行时统一对 `scaleRange` 做排序和最小值清洗，避免输入顺序或极值把逻辑拖崩
3. 补两条新的 `CloudShadowSystemTests`：
   - `SimulateStep_HighSpeed_ShouldRecycleAndRefillMultipleCloudsInSameStep`
   - `EdgeSpawn_HorizontalMovement_ShouldKeepCloudHeightInsideArea`

**关键判断**:
- 这轮最核心的代码判断成立了：问题不只是“按钮放错地方”，而是原先补云链在高密度/高速度下没有自愈能力；现在至少把“补不进去就一直干等”的硬缺口收掉了
- 这轮仍然只是**结构 + targeted probe 过线**，不是**体验过线**：
  - 已证明 Inspector 更适合调
  - 已证明高负载逻辑链比原来稳
  - 还没有证明“云像不像云、现场到底好不好看”

**验证状态**:
- `validate_script`：`CloudShadowManager.cs`、`CloudShadowManagerEditor.cs`、`CloudShadowSystemTests.cs` 均无脚本级错误
- `Tests.Editor`：
  - `CloudShadowSystemTests` 整套 13 条通过
  - `TestResults.xml` 已确认新加的 2 条云影回归被真实编入并通过
- Console：
  - 未见本轮云影代码带出的新编译错误
  - 当前 Console 里仍有别线既有 `TreeController` / `PersistentManagers` 相关历史噪声，不属于本刀新增

**本轮边界 / 未完成**:
- 未碰 `Primary.unity`
- 未碰任何 scene / prefab
- 未做用户 GameView / 手感终验
- 未把旧云影路线包装成“最终方案”；当前仍只是把它修到值得继续验证，而不是宣称已经好看

**本轮涉及文件**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Rendering\CloudShadowManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\Editor\CloudShadowManagerEditor.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\CloudShadowSystemTests.cs`

**收口状态**:
- 已执行 `Begin-Slice`（后续补登记扩到时间链时又做了一次 `-ForceReplace`）
- 已执行 `Park-Slice`
- 当前 live 状态：`PARKED`
- 当前 blocker：
  - 用户尚未做云影/光影的 GameView 终验
  - 本轮没有做 `Ready-To-Sync`
  - 当前不在本回合做 git sync 收口

### 2026-04-04（提交前补记：白名单代码可提交，但 sync gate 被外线程占用）

- 这轮在用户要求“先提交当前工作区所有可提交内容”后，重新把代码与 memory 纳入 `Begin-Slice` 白名单，并尝试 `Ready-To-Sync`。
- 结果不是内容层 blocker，而是流程层 blocker：
  - 当前白名单内容已经过 `git diff --check`
  - 但 `Ready-To-Sync` 被 `.kiro/state/ready-to-sync.lock` 挡住
  - 进一步核实后，当前 `UI` 线程处于 `READY_TO_SYNC`，这把 lock 对本线程来说是合法外部门，不应越权抢
- 因此当前关于云影这条线最准确的状态是：
  - **代码层已可提交**
  - **流程层暂时不可提交**
- 本线程已重新 `Park-Slice`，等待 sync gate 释放或用户明确裁定顺序

### 2026-04-04（现场修补：云不进场景 + Inspector foldout 报错）

- 用户新反馈了两个现场问题：
  1. 云影虽然生成了，但主要停在场景边缘外，看起来“不进场景”
  2. Inspector 出现 `您不能嵌套折叠标题头，请以 EndFoldoutHeaderGroup 结束它。`
- 本轮直接做了两类修补：
  1. `CloudShadowManagerEditor.cs`
     - 不再继续用 `BeginFoldoutHeaderGroup`
     - 改成普通 `Foldout + box` 结构，避免现场出现折叠头嵌套报错
  2. `CloudShadowManager.cs`
     - `RebuildClouds()` 不再用“全区域完全随机”播撒首批云
     - 改成沿当前移动方向的流向分布：先从进场边缘取点，再按路径随机推进到活动区内部
     - 目标是让重建后的云至少有一部分直接分布在场景活动区里，而不是都挤在进场边缘外
     - 额外补了 `EditorRebuildNow_ShouldSeedAtLeastOneCloudInsideActiveArea` 回归
- 本轮静态结果：
  - 这 3 个文件已过 `git diff --check`
  - 但因为 Unity MCP 当前握手失败，本轮还没法做 Editor 自动验证
- 当前状态：
  - 代码已修
  - 线程已重新 `Park-Slice`
  - 等用户复看这两个现场症状是否消失

### 2026-04-06（续修：云卡左外侧 + 补云日志刷爆）

- 用户补了最新现场截图后，问题被进一步收窄：
  - Inspector 报错已经消失
  - 真问题变成：云影虽然在生成，但长期刷在活动区左外侧，看起来像“不进场景”
  - 同时 `enableDebug` 打开后，控制台持续刷 `无法找到不重叠的位置`，噪音过大
- 本轮继续真实施工，重新执行了 `Begin-Slice`，切片名为 `cloud-entry-and-log-spam-fix`。
- 代码层修补：
  1. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
     - 新增“进场带”生成逻辑，补云与首批播撒不再默认刷在区域外边缘，而是优先刷在活动区内部的入口带
     - `TrySpawnOneAtEdge()` 运行路径改为优先使用 `TryFindNonOverlappingEntryBandPosition()`
     - `TrySpawnOneAlongFlow()` 也增加最小可见推进距离，避免首批云刚生成时就贴在边缘外或只露极少一截
     - 新增 `TryLogSpawnStall()`，把连续失败日志改成按时间节流的汇总提示，不再一秒刷上千条
     - 新增 `GetEntryBandDepth()` / `GetInitialSpawnMinProgress()`，把“能看见地进场”变成显式规则
  2. `Assets/YYY_Tests/Editor/CloudShadowSystemTests.cs`
     - 新增 `EdgeRefillSpawn_ShouldPlaceCloudInsideLeadingBandInsteadOfOutsideArea`
     - 用来锁住“补云中心点必须在活动区前沿带内，而不是刷到左外侧”
- Unity 侧验证结果：
  - `validate_script`：
    - `CloudShadowManager.cs`：`0 error / 2 warning`
    - `CloudShadowSystemTests.cs`：`0 error / 0 warning`
  - 最新 Console 已看到新的生成坐标：
    - 例如 `(-14.55, 29.42)`、`(-13.59, -0.68)`、`(4.87, -21.14)`、`(8.92, 29.79)`
    - 这些点已经回到当前活动区 `Center=(-14,10), Size=(64,88)` 内，不再是之前那批 `x≈-55` 的左外侧刷点
  - EditMode 测试：
    - `CloudShadowSystemTests` 共 `15/15 passed`
  - 额外现场信息：
    - 当前 Console 仍有 unrelated 旧问题：多条 `The referenced script (Unknown) on this Behaviour is missing!`
    - 这不是本线程本轮新引入问题，也不属于云影代码面
- 当前判断：
  - “检查器报错”这条已基本闭环
  - “云不进场景”这条代码层和 Unity 证据都明显改善，但视觉体验是否过线仍待用户终验
- 当前收口状态：
  - 本轮不做 `Ready-To-Sync`
  - 先回到 `Park-Slice`
  - 等用户根据最新现场再判断“现在是否像真正进场了”，还是还要继续做观感层调优

### 2026-04-07（续修：整朵穿场生命周期 + 分场景运行态持久化）

- 主线目标：
  - 把云朵机制从“入口带补云”重写成“从场景外完整进场、完整离场才算一轮”
  - 同时让 `Primary` 和 `Town` 都有云，但两边状态彼此独立且支持快速切场恢复
- 本轮真实施工：
  - 已沿现有切片继续施工，并在收口前重新执行 `Park-Slice`
- 本轮代码改动：
  1. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
     - `TrySpawnOneAtEdge()` 改为真正使用 `TryFindNonOverlappingEdgePosition()`，不再从场内入口带偷刷
     - `TrySpawnOneAlongFlow()` 改为“场外起点 + 全穿场进度”，不再要求中心点先落在活动区内
     - `GetTraversalDistance()` 改成按“场外生成点 -> 另一侧完全离场阈值”的完整距离计算
     - 移除 `RecycleCloudNearExit()/FindCloudClosestToExit()` 这类半路强拆腾位逻辑，避免云还没走完就被掐掉
     - 新增场景级运行态缓存：
       - key 使用 `scene.path + hierarchy path`
       - 缓存每个 manager 的活体云列表、`seed`、`cloudIdCounter`、冷却/失败计数
       - `OnDisable()` 先保存，再清运行对象；重新进入场景时优先恢复，而不是重新洗牌
     - `Cloud` 结构新增 `spriteIndex`、`scale`，支持精确恢复运行态
  2. `Assets/000_Scenes/Town.unity`
     - 把 `CloudShadowManager.enableCloudShadows` 从 `0` 改为 `1`
     - 把 `areaSizeMode` 从 `Manual(0)` 改为 `AutoDetect(2)`，与 `Primary` 对齐
- 本轮验证：
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs --count 30 --output-limit 40`
    - 结果：`assessment=no_red`
  - `python scripts/sunset_mcp.py errors --count 30 --include-warnings --output-limit 60`
    - 结果：`0 error / 1 warning`
    - 唯一 warning 为外部 `SpringDay1WorkbenchCraftingOverlay.cs` 的过时 API 警告，不属于本线程
- 当前判断：
  - 这轮已经把“死云”的根逻辑从机制层改掉了，不再是之前那套入口带补位模型
  - `Town` 现在也已接入云机制，但是否还存在新的边界观感问题，仍需要用户在真实场景里终验
- 当前收口状态：
  - 已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
  - 下一步只等用户重测 `Primary/Town` 的云朵进出场表现与快速切场恢复是否过线

### 2026-04-07（续修：死云残留累计根因确认 + 孤儿云清理）

- 用户最新反馈：
  - 运行中仍能看到“有些云不动”
  - 且残留会累计
- 本轮核心结论：
  1. `Primary.unity` 已经被错误保存进了多枚历史 `CloudShadow` 子物体
     - 这些对象直接挂在 `CloudShadowManager` 下面
     - 不是纯运行态列表里的云，而是 scene YAML 真脏了
  2. 旧恢复链只恢复 `active` 云，不会先扫这些历史孤儿云
     - 所以每次 `InitializeIfNeeded()/TryRestoreRuntimeState()/RebuildClouds()` 都可能在历史残留上再叠新云
  3. 旧创建链把云对象当普通 `GameObject` 创建
     - 在 `ExecuteAlways + previewInEditor=1` 条件下，预览云有机会被序列化进 scene
- 本轮修补：
  1. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
     - 新增 `NormalizeResidualCloudChildren()`：
       - 在 `OnEnable`、`InitializeIfNeeded`、`RebuildClouds`、`TryRestoreRuntimeState` 前先扫 `CloudShadowManager` 子节点
       - 把不在 `active`、但名字为 `CloudShadow` 的孤儿云收回 pool 并隐藏
     - 新建云对象改为统一使用 `RuntimeCloudObjectName`
     - fresh 创建的云对象加 `DontSaveInEditor | DontSaveInBuild`
       - 目标是后续预览/运行生成的云不再被序列化进 scene
     - 已有历史孤儿云不再强打 `DontSave`，避免对已持久化对象触发 Unity 断言
  2. `Assets/000_Scenes/Primary.unity`
     - 已清掉挂在 `CloudShadowManager` 下面的历史 `CloudShadow` scene 对象
     - 清理后 `rg \"m_Name: CloudShadow$\"` 已不再命中 `Primary.unity/Town.unity`
- 本轮验证：
  - `rg -n \"m_Name: CloudShadow$|CloudShadowManager\" Assets/000_Scenes/Primary.unity Assets/000_Scenes/Town.unity`
    - 结果：只剩两个 `CloudShadowManager`，不再有历史 `CloudShadow` scene 对象
  - `python scripts/sunset_mcp.py errors --count 30 --include-warnings --output-limit 60`
    - 一度出现我自己半途补口造成的 compile red，已收回
  - 最新 `errors`
    - 当前看到的是 MCP/PlayMode 外部错误：
      - `Setting and getting Body Position/Rotation... should only be done in OnAnimatorIK...`
      - `Can't call GetPlaybackTime while not in playback mode...`
    - 这两条来自 `com.coplaydev.unity-mcp` 包，不属于云线代码逻辑
- 当前判断：
  - “残留会累计”这条已经不是猜测，而是三段机制链合成的问题，且根因已锁定
  - 这轮已把“历史 scene 脏云 + 新云继续被存进 scene + 恢复前不扫孤儿云”三层同时处理
- 当前收口状态：
  - 已重新 `Park-Slice`
  - 当前 live 状态：`PARKED`
  - 下一步等用户重新看 `Primary/Town` 是否还会出现“不动云/残留累计”

### 2026-04-07（支撑子任务：给存档线程生成云朵正式持久化接入 prompt）

- 用户要求：
  - 既然当前云运行态数据已经足够支持正式持久化，就给 `存档系统` 线程写一份可直接续工的 prompt
- 本轮动作：
  - 已在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-07_云朵与光影_存档持久化接入prompt_01.md`
    落好续工正文
- prompt 已锚定的唯一主刀：
  - 把 `CloudShadowManager` 当前已有的运行态缓存，接入 `SaveDataDTOs / SaveManager` 正式存档链
  - 明确禁止扩大到天气/光影/特效统一存档重构
  - 明确要求按场景分离、不能退化成“只存 seed 再重抽”
- 当前收口状态：
  - 已 `Park-Slice`
  - 等用户把该 prompt 转发给 `存档系统` 线程
