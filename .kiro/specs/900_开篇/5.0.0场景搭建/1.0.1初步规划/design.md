# 设计文档：1.0.1 初步规划

## 1. 阶段设计定位

`1.0.1初步规划` 是“真实场景搭建前的稳定化阶段”。本阶段输出的不是效果图，而是后续施工蓝图。

## 2. 当前已验证事实

- 项目内存在独立 scene 目录：`Assets/000_Scenes`。
- 当前已读到的 scene 包括：
  - `Artist.unity`
  - `Artist_Temp.unity`
  - `DialogueValidation.unity`
  - `Primary.unity`
  - `SampleScene.unity`
  - `矿洞口.unity`
- 当前已读到的 prefab 主目录包括：
  - `Box`
  - `Crops`
  - `Dungeon props`
  - `Farm`
  - `House`
  - `NPC`
  - `Pan`
  - `Rock`
  - `Tree`
  - `UI`
  - `WorldItems`
- 当前 `Assets` 下大约存在：
  - `387` 个 `.prefab`
  - `3756` 个 `.png`
  - `4717` 个 `.asset`
- 已确认存在 Tilemap 相关入口：
  - `Assets/223_Prefabs_TIlemaps`
  - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
  - `Assets/YYY_Scripts/Farm/Data/LayerTilemaps.cs`

结论：当前主线不是从零开始，具备真实的独立场景搭建基础。

## 3. 本阶段输出物设计

本阶段固定输出 5 类内容：

1. 父工作区基线文档
2. 子工作区基线文档
3. 首轮资产普查
4. 场景骨架与 Tilemap 分层方案
5. 下一阶段准入条件

## 3.1 后续执行模型

本阶段之后，真实场景搭建统一按以下顺序推进：

1. 资产普查
2. 场景骨架
3. 地图底稿
4. 结构层
5. 装饰层
6. 前景遮挡与逻辑层
7. 回读修正
8. 交付

这条顺序从现在起直接作为本主线的默认施工路线，不再分散写在父层文档里。

## 4. 后续施工蓝图

### 4.1 场景骨架

后续进入真实施工时，优先创建如下骨架：

```text
SceneRoot
├─ Systems
├─ Tilemaps
├─ PrefabSetDress
├─ GameplayAnchors
├─ LightingFX
└─ DebugPreview
```

### 4.2 Tilemap 分层

后续使用固定 8 层结构：

```text
TM_Ground_Base
TM_Ground_Detail
TM_Path_Water
TM_Structure_Back
TM_Structure_Front
TM_Decor_Front
TM_Occlusion
TM_Logic
```

设计原则：

- `Ground` 与 `Structure` 分离；
- 前后遮挡明确；
- 逻辑层不混进视觉层；
- 保持“够用、清楚、能维护”，不为了显得专业而滥拆。

### 4.3 资产选择优先级

后续优先从以下入口中选材：

1. `Assets/223_Prefabs_TIlemaps`
2. `Assets/222_Prefabs/House`
3. `Assets/222_Prefabs/Farm`
4. `Assets/222_Prefabs/Tree`
5. `Assets/222_Prefabs/Rock`
6. `Assets/222_Prefabs/Dungeon props`
7. `Assets/Sprites`

### 4.3.1 首版 prefab 候选池整理粒度

首版不追求把全部 `222_Prefabs` 做成最终索引，而是只整理到“足够开工”的三级粒度：

#### A 档：全收录，直接逐个试配

- `Assets/222_Prefabs/House`：5 个 prefab
- `Assets/222_Prefabs/Tree`：3 个 prefab
- `Assets/222_Prefabs/Rock`：3 个 prefab
- `Assets/223_Prefabs_TIlemaps/House_Tilemap`：4 个 prefab

处理方式：

- 首版直接把这 4 组完整纳入候选池；
- 后续进入真实施工时可以逐个试摆，不需要再二次拆桶。

#### B 档：按桶收录，不在规划阶段展开到单 prefab

- `Assets/222_Prefabs/Farm`：117 个 prefab
  - `Food`：98 个
  - `T`：19 个

处理方式：

- 当前阶段只把 `Farm` 记为大类；
- 把 `Food / T` 作为二级桶保留；
- 真正施工时再按底稿主题挑小批量样本，不做 117 个逐条梳理。

#### C 档：按需补入，不进入首版常驻候选池

- `Assets/222_Prefabs/Dungeon props`：42 个 prefab
- `Assets/222_Prefabs/UI`：27 个 prefab
- `Assets/222_Prefabs/WorldItems`：71 个 prefab
- `Assets/222_Prefabs/NPC`：3 个 prefab
- `Assets/223_Prefabs_TIlemaps/UI_Tilemap`：16 个 prefab

处理方式：

- 只有当首版场景已经需要洞穴感、交互投放、NPC 站位或 UI 辅助验证时才回补；
- 当前规划阶段不把它们纳入首版常驻候选池。

结论：

- 以 A / B / C 三档收口后，首版施工入口已经足够明确；
- 后续真正开工时，不需要再重新思考“要不要把所有 prefab 先整理完”。

### 4.4 新 scene 命名、路径与最小创建方案

#### 已验证现场

- 当前 scene 目录为扁平结构：`Assets/000_Scenes`
- 当前已存在 scene：
  - `Artist`
  - `Artist_Temp`
  - `DialogueValidation`
  - `Primary`
  - `SampleScene`
  - `矿洞口`
- 当前 Build Settings 内只有：
  - `Assets/000_Scenes/Primary.unity`
- 当前代码里已确认存在按字符串加载 scene 的入口：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\DoorTrigger.cs`

#### 命名判断

基于以上事实，当前最稳妥的做法不是新开子目录，也不是提前绑定具体剧情/地块主题，而是：

- 继续沿用 `Assets/000_Scenes` 的扁平目录习惯；
- 先给本主线的独立施工 scene 一个中性、稳定、不会误导为正式主场景的名字；
- 在它真正进入跨 scene 引用或 build 流程前，不急着做主题化重命名。

#### 当前推荐

- 推荐路径：`Assets/000_Scenes/SceneBuild_01.unity`
- 推荐名称：`SceneBuild_01`

#### 采用这个名字的原因

1. 不与 `Primary`、`DialogueValidation` 这类现有职责名冲突；
2. 保持英文路径，降低后续字符串引用和工具链歧义；
3. `01` 允许后续继续扩成 `SceneBuild_02`，不用现在强绑具体地块主题；
4. 明确告诉所有线程：这是一块独立施工承载面，不是主运行入口。

#### Build Settings 策略

- `SceneBuild_01.unity` 在创建时**不加入** Build Settings。
- 当前 Build Settings 继续只保留 `Primary.unity`。
- 只有当该 scene 真正进入运行时入口、门传送链或流程承载时，才讨论是否纳入 Build Settings 或改成主题化正式名称。

#### 最小创建方案

推荐使用 Empty Scene 起步，最小结构如下：

```text
SceneBuild_01
├─ SceneRoot
│  ├─ Systems
│  │  ├─ MainCamera
│  │  └─ Grid
│  ├─ Tilemaps
│  │  ├─ TM_Ground_Base
│  │  ├─ TM_Ground_Detail
│  │  ├─ TM_Path_Water
│  │  ├─ TM_Structure_Back
│  │  ├─ TM_Structure_Front
│  │  ├─ TM_Decor_Front
│  │  ├─ TM_Occlusion
│  │  └─ TM_Logic
│  ├─ PrefabSetDress
│  ├─ GameplayAnchors
│  ├─ LightingFX
│  └─ DebugPreview
```

#### 最小创建步骤

1. 新建 Empty Scene。
2. 保存到 `Assets/000_Scenes/SceneBuild_01.unity`。
3. 不加入 Build Settings。
4. 创建 `SceneRoot` 与六大根层。
5. 在 `Systems` 下创建 `Grid`。
6. 在 `Tilemaps` 下创建 8 个固定 Tilemap 层。
7. 暂不放任何业务对象、门传送引用、剧情入口或热区脚本。

#### 五段式判断

1. 原有配置  
   - `Primary` 是当前唯一 build scene；`Assets/000_Scenes` 采用扁平命名；项目已有按 scene 名字符串加载的入口。
2. 问题原因  
   - 如果现在随意取名、过早加到 Build Settings 或绑定具体主题，后续很容易把“施工 scene”和“正式运行 scene”混淆。
3. 建议修改  
   - 创建一个独立但中性的 `SceneBuild_01.unity`，只作为施工承载面，不进入 Build Settings。
4. 修改后效果  
   - 后续可以放心在独立 scene 内搭骨架、画底稿、放结构，不干扰 `Primary`。
5. 对原有功能的影响  
   - 当前不会影响现有运行入口、门传送链或 build 流程，因为本阶段还不接入它们。

## 5. MCP 在本主线中的职责

MCP 在本主线里只承担“施工辅助 + 回读验证”角色，不替代设计判断。

适合交给 MCP 的事：

- 读取 scene 层级与对象状态；
- 创建 scene 骨架与基础对象；
- 批量建立 `Grid + Tilemap` 根结构；
- 回读 Console、层级与必要截图；
- 做最小验证闭环。

不应完全交给 MCP 的事：

- 单张参考图的审美级完整还原；
- 最终构图和节奏判断；
- 细节美术取舍；
- 空间呼吸感与高级摆场。

## 6. 本阶段与下一阶段切换条件

只有满足以下条件，才进入真实 scene 施工：

- 当前规划文档已稳定；
- 已明确新 scene 的命名与落点；
- shared root / live Git 允许进入真实写态；
- 已再次完成 scene 安全审视；
- 已准备好后续的 MCP 回读与验收路径。

## 7. 交付标准

本阶段交付不以“做了多少文档”为标准，而以“后续是否可以不再重想”作为标准。

如果下一阶段拿着这些文档可以直接开始：

- 建 scene；
- 搭 Grid / Tilemap；
- 画底稿；
- 放结构；
- 放装饰；
- 做回读；

那么本阶段就算交付成立。

后续真实场景阶段的交付定义也同步固定为：

- 结构清楚；
- 分层有秩序；
- 动线明确；
- 可直接继续精修；
- 明显降低后续微调成本。
