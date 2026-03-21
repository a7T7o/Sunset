# 设计文档：1.0.1 初步规划

## 1. 阶段设计定位

`1.0.1初步规划` 是“真实场景搭建前的稳定化阶段”。本阶段输出的不是效果图，而是后续施工蓝图。

## 2. 当前已验证事实

- 项目内存在独立 scene 目录：`Assets/000_Scenes`
- 当前 scene 名包括：
  - `Artist`
  - `Artist_Temp`
  - `DialogueValidation`
  - `Primary`
  - `SampleScene`
  - `矿洞口`
- 当前 Build Settings 内只有：
  - `Assets/000_Scenes/Primary.unity`
- 项目中已确认存在按字符串加载 scene 的入口：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\DoorTrigger.cs`
- 当前 prefab 规模抽样：
  - `House = 5`
  - `Tree = 3`
  - `Rock = 3`
  - `House_Tilemap = 4`
  - `Farm = 117`
  - `Dungeon props = 42`

## 3. 后续执行模型

真实场景搭建统一按以下顺序推进：

1. 资产普查
2. 场景骨架
3. 地图底稿
4. 结构层
5. 装饰层
6. 前景遮挡与逻辑层
7. 回读修正
8. 交付

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

### 4.3 新 scene 命名、路径与最小创建方案

- 推荐路径：`Assets/000_Scenes/SceneBuild_01.unity`
- 推荐名称：`SceneBuild_01`

采用这个名字的原因：

1. 不与 `Primary`、`DialogueValidation` 等现有职责名冲突；
2. 保持英文路径，降低后续字符串引用和工具链歧义；
3. `01` 允许后续继续扩成 `SceneBuild_02`；
4. 明确这是独立施工承载面，而不是主运行入口。

Build Settings 策略：

- `SceneBuild_01.unity` 在创建时**不加入** Build Settings；
- 当前 Build Settings 继续只保留 `Primary.unity`。

最小创建方案：

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

最小创建步骤：

1. 新建 Empty Scene
2. 保存到 `Assets/000_Scenes/SceneBuild_01.unity`
3. 不加入 Build Settings
4. 创建 `SceneRoot` 与六大根层
5. 在 `Systems` 下创建 `Grid`
6. 在 `Tilemaps` 下创建 8 个固定 Tilemap 层
7. 暂不放任何业务对象、门传送引用、剧情入口或热区脚本

### 4.4 首版 prefab 候选池粒度

#### A 档：全收录小集合

- `Assets/222_Prefabs/House`
- `Assets/222_Prefabs/Tree`
- `Assets/222_Prefabs/Rock`
- `Assets/223_Prefabs_TIlemaps/House_Tilemap`

#### B 档：按桶收录的大集合

- `Assets/222_Prefabs/Farm`
  - 当前先按目录桶识别：
    - `Food/1 ~ Food/7`
    - `T`

#### C 档：按需补充集合

- `Assets/222_Prefabs/Dungeon props`
- 其他未进入主候选池的 `UI / WorldItems / NPC`

## 5. MCP 在本主线中的职责

MCP 在本主线里只承担“施工辅助 + 回读验证”角色，不替代设计判断。

## 6. 本阶段与下一阶段切换条件

只有满足以下条件，才进入真实 scene 施工：

- 当前规划文档已稳定；
- 已明确新 scene 的命名与落点；
- shared root / live Git 允许进入真实写态；
- 已再次完成 scene 安全审视；
- 已准备好后续的 MCP 回读与验收路径。

