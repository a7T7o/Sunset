# 打包窗口化方案分析（2026-04-18）

## 本轮定位

- 目标不是立刻实现，而是先把 `Sunset` 当前“窗口化该怎么做”想透。
- 本轮只做只读分析，不改代码、不改场景、不改 PlayerSettings。
- 结论面向 Windows Standalone 打包链，因为老师当前反馈就是桌面打包首发体验。

## 核心结论

1. 当前项目并不是“技术上完全不能窗口化”，而是“首发默认行为被配置成了全屏路径，而且项目里没有任何玩家可见的显示模式入口或偏好持久化链”。
2. 对 `Sunset` 现在最安全、最可靠的第一刀，不是直接上“任意拖拽缩放 + 运行时热切换 + 全分辨率支持”，而是先收成：
   - **首发默认进入固定 16:9 的 Windowed**
   - **先不开放自由拖拽缩放**
   - **后续再补明确的显示模式切换与本地偏好保存**
3. 当前真正高风险的是把“支持窗口化”误做成“立刻支持所有窗口态”。项目里已经存在：
   - 宽屏/全屏下相机 confiner 出过真实事故
   - 多套 UI 参考分辨率并不统一
   - 一批 HUD/提示层直接读取 `Screen.width/height`
   所以如果一步跨到 `resizableWindow + 任意宽高比 + 运行中热切换`，验证矩阵会瞬间膨胀。

## 已核实的项目事实

### 1. 当前打包默认就是全屏首发

来自 `ProjectSettings/ProjectSettings.asset`：

- `defaultScreenWidth: 1920`
- `defaultScreenHeight: 1080`
- `defaultIsNativeResolution: 1`
- `resizableWindow: 0`
- `fullscreenMode: 1`
- `allowFullscreenSwitch: 1`
- `resetResolutionOnWindowResize: 0`

至少可以确认两件事：

1. 当前默认启动不是普通可移动窗口。
2. 当前即使以后切到窗口态，也还没开放“用户拖拽改大小”的配置。

### 2. 项目里没有运行时显示模式管理链

全仓检索结果：

- 没找到运行时代码调用 `Screen.SetResolution(...)`
- 没找到 `Screen.fullScreenMode = ...`
- 没找到“显示模式 / 分辨率 / fullscreen / windowed”的玩家设置保存链

这说明现在的首发体验几乎完全由 PlayerSettings 决定，而不是由游戏内系统决定。

### 3. 现有“设置页入口”并不等于显示设置已存在

项目里确实有一个设置入口：

- `GameInputManager` 的 `ESC` 会走 `PackagePanelTabsUI.OpenSettings()`
- `PackagePanelTabsUI` 的第 `5` 页是 `OpenSettings()`

但这个页现在实际装的是：

- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`

它当前内容是**存档管理页**，不是显示设置页。也就是说：

- 有“设置入口”
- 但没有“显示设置内容”
- 后续真做窗口模式切换时，不能假装这页已经天然承载了显示系统

### 4. 相机系统已经有过“全屏 / 超宽”真实事故

`Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs` 当前存在一整套宽屏保护逻辑：

- `clampViewportOnWideScreens`
- `wideScreenViewportSafety`
- `snapViewportClampToPixelGrid`
- 基于 `Screen.width / Screen.height` 动态重算 `Camera.rect`

并且项目历史记忆里已经明确记过：

- 双击 `Game` 全屏或超宽窗口时，confiner 曾把镜头锁死
- 后来专门补了“宽屏保护 + viewport clamp + pixel grid snap”

这条历史说明：**窗口化不是纯 UI 题，也是相机题**。

### 5. UI 适配不是一条统一合同

当前至少有三类并存的 UI/屏幕适配方式：

#### A. CanvasScaler 路线

例如：

- `InteractionHintOverlay`：`referenceResolution = 1980 x 1080`
- `SpringDay1StatusOverlay`：`referenceResolution = 1980 x 1080`
- `DurabilityTestUI`：`referenceResolution = 1920 x 1080`

#### B. 直接读 `Screen.width / Screen.height`

例如：

- `TimeManagerDebugger`
- `HeldItemDisplay`
- `ItemTooltip`
- `CameraDeadZoneSync`

#### C. 手工“屏幕变化后重新摆正”路线

例如：

- `HealthSystem.UpdateResponsiveLayoutIfNeeded()`
- `EnergySystem.UpdateResponsiveLayoutIfNeeded()`

这不是“不能支持窗口化”的证据，但说明一个关键现实：

> 当前项目并没有一条“所有 UI 都在同一显示合同下工作”的单一规范。

所以“默认窗口化”可以先做，但“自由缩放窗口”不能想当然。

## 官方 Unity 口径

本轮补查了 Unity 官方文档，核心口径如下：

1. **Windows Player Settings 的 `Fullscreen Mode` 决定启动时的默认窗口模式**
   来源：Unity Manual, Windows Player settings
   链接：<https://docs.unity3d.com/cn/2022.3/Manual/playersettings-windows.html>

2. **`FullScreenWindow` 是无边框全屏（borderless full screen）**，覆盖整个屏幕，不等于普通窗口
   来源：Unity Scripting API, `FullScreenMode.FullScreenWindow`
   链接：<https://docs.unity3d.com/cn/2022.3/ScriptReference/FullScreenMode.FullScreenWindow.html>

3. **`Windowed` 是标准、可移动的非全屏窗口**
   来源：Unity Scripting API, `FullScreenMode`
   链接：<https://docs.unity3d.com/cn/2022.1/ScriptReference/FullScreenMode.html>

4. **`Screen.SetResolution(width, height, FullScreenMode...)` 可以在运行时切模式**，但分辨率切换并不是立即发生，而是在当前帧结束后生效
   来源：Unity Scripting API, `Screen.SetResolution`
   链接：<https://docs.unity3d.com/ScriptReference/Screen.SetResolution.html>

5. **`PlayerSettings.resizableWindow` 控制桌面版窗口是否允许拖拽改大小**
   来源：Unity Scripting API, `PlayerSettings.resizableWindow`
   链接：<https://docs.unity3d.com/ru/current/ScriptReference/PlayerSettings-resizableWindow.html>

6. **命令行参数可以临时覆盖首发窗口行为**，包括：
   - `-screen-fullscreen 0/1`
   - `-screen-width`
   - `-screen-height`
   - `-window-mode`（Windows only）
   来源：Unity Manual, Player command line arguments
   链接：<https://docs.unity3d.com/cn/2022.1/Manual/PlayerCommandLineArguments.html>

这些官方口径共同说明：

- 窗口化在 Unity 桌面版是标准能力，不是旁门方案。
- 但“支持窗口化”和“支持任意 resize / 任意比例 / 任意时机热切换”是三件不同难度的事。

## 方案对比

### 方案 A：只把首发默认改成固定 16:9 Windowed，不开放自由缩放

#### 做法

- 启动默认模式改成 `Windowed`
- 默认尺寸固定在一个 16:9 预设
- `resizableWindow` 先保持关闭
- 运行时先不做显示设置菜单切换

#### 优点

- 最符合“先让老师一打开就知道这游戏支持窗口化”
- 不会把项目一口气拉进“所有窗口尺寸都要验”的大矩阵
- 对现有 UI / Camera / HUD 影响最小
- 最适合当前 `Sunset` 打包前阶段

#### 风险

- 还不算“完整显示设置系统”
- 如果默认尺寸选得太大，在 1080p 显示器上会看起来接近全屏
- 如果默认尺寸选得太小，某些文字和面板可能显得拥挤

#### 结论

**这是当前最推荐、也最安全的第一刀。**

---

### 方案 B：首发窗口化，同时打开 `resizableWindow`

#### 做法

- 默认 Windowed
- 同时允许用户直接拖拽窗口大小

#### 优点

- 玩家感知上更像“正式支持窗口化”
- 不需要立刻做分辨率下拉

#### 风险

- 这等于把项目直接升级成“连续窗口尺寸变化”的实时适配问题
- 现有 `CameraDeadZoneSync`、`ItemTooltip`、`HeldItemDisplay`、调试 HUD、提示层都要跟着一起验
- 宽高比稍一变化，就可能把过去的镜头与 UI 边缘问题重新抬出来

#### 结论

**不建议作为第一刀。**
它不是“窗口化最小支持”，而是“自由缩放支持”。

---

### 方案 C：一步到位做完整显示设置系统

#### 做法

- 游戏内提供：
  - `Windowed / FullScreenWindow`
  - 分辨率预设
  - 本地持久化
  - 应用 / 取消 / 回退

#### 优点

- 长远看最完整
- 外发测试最像正式产品

#### 风险

- 这不是一个“只改构建配置”的问题，而是：
  - UI 设计问题
  - 偏好持久化问题
  - 切换时机问题
  - 验证矩阵问题
- 当前 `Settings` 页实际是“存档管理页”，不是天然的系统设置页
- 一步做到位容易让这轮从“窗口化”漂成“完整系统设置重构”

#### 结论

**适合作为第二阶段，不适合当前第一刀。**

---

### 方案 D：保持默认全屏，只依赖快捷键 / 命令行

#### 做法

- 不改默认行为
- 靠 `Alt+Enter` 或命令行参数让懂的人自己切

#### 优点

- 几乎不动项目

#### 风险

- 对老师和群测玩家都不友好
- 首发第一印象仍会是“这游戏不支持窗口化”
- 不符合当前反馈的真实问题

#### 结论

**直接排除。**

## 针对 Sunset 的推荐路线

## 第一阶段：最安全的“窗口化存在性”交付

### 推荐目标

让 Windows 打包版做到：

1. **首发默认不是全屏**
2. **首发窗口是固定 16:9**
3. **镜头与主要 HUD 在这个窗口下稳定**

### 推荐口径

- 默认模式：`Windowed`
- 默认尺寸：优先烟测 `1600 x 900`
- 若 `1600 x 900` 烟测发现文本过小或面板拥挤，再退到 `1920 x 1080` 的固定窗口
- `resizableWindow`：第一阶段保持关闭

### 为什么优先 `1600 x 900`

因为老师当前反馈的是“普通屏也应该有窗口化”。在 `1920 x 1080` 显示器上：

- `1920 x 1080` 的窗口即使 technically 是 windowed，体感也可能非常接近全屏
- `1600 x 900` 更明确地表达“这是窗口态”

但这只是**首选候选尺寸**，不是可以跳过烟测的拍脑袋默认值。

## 第二阶段：补“正式可见”的显示模式入口

### 推荐目标

补一个玩家可见的显示模式设置，但先只支持：

- `Windowed`
- `FullScreenWindow`

先不扩到：

- 任意分辨率列表
- `ExclusiveFullScreen`
- 任意拖拽 resize

### 推荐落点

沿用 `PackagePanelTabsUI.OpenSettings()` 这个入口，但不要误判为“现成显示设置页已经存在”。

当前更合理的做法是二选一：

1. 把第 `5` 页从“存档管理”升级成“系统 / 存档”双区页
2. 或者给 Package 面板新增独立的显示设置页

### 不推荐的做法

- 直接把显示选项偷偷塞进当前 `PackageSaveSettingsPanel` 的下半截
- 页面标题仍叫“存档管理”，但里面混显示设置

这会让发现性和语义都很差。

## 第三阶段：再考虑自由缩放与更多宽高比

只有在前两阶段过了之后，才值得继续谈：

- `resizableWindow = true`
- 任意拖拽改大小
- 超宽 / 16:10 / 小窗最小尺寸

否则就是把窗口化问题扩大成“全项目 UI / camera 响应式重验”。

## 偏好持久化的推荐归属

如果后续进入第二阶段，需要记住用户的窗口模式偏好，推荐：

- **用本地客户端偏好保存**
- **不要混进存档槽位**

推荐理由：

1. 显示模式是**设备偏好**，不是游戏进度。
2. 老师机器、玩家机器、开发机器的显示模式需求天然不同。
3. 项目里已经有 `InteractionHintDisplaySettings -> PlayerPrefs` 这类“本地 UI 偏好”先例。

因此更合理的归属是：

- `PlayerPrefs` 或独立本地配置

而不是：

- `SaveManager`
- 默认存档 / 普通存档

## 未来实现时的验证矩阵

哪怕只做第一阶段，最少也要跑这组烟测：

1. **启动行为**
   - 打包启动后是否直接进入窗口态
   - 窗口尺寸是否符合预期
   - 窗口是否不会超出普通 1080p 屏幕工作区

2. **三张主场景**
   - `Town`
   - `Primary`
   - `Home`
   检查镜头跟随、边界、黑边、白边、超出 scene 外的问题

3. **核心 HUD / 提示层**
   - 交互提示
   - 状态 HUD
   - 血条 / 精力条
   - 背包 / 箱子 / tooltip

4. **Package 面板**
   - 打开 / 切页 / 关闭
   - 设置页（当前其实是存档页）布局是否正常

5. **窗口焦点切换**
   - 最小化 / 切出 / 切回
   - 是否出现异常暂停、输入失焦、窗口错位

如果第二阶段再补显示模式切换，则新增：

6. **Windowed <-> FullScreenWindow 切换**
   - 切换后 camera 是否重新计算正确
   - HUD / tooltip / package panel 是否仍正常
   - 切换是否需要重进场景

## 当前不建议承诺的内容

在没有实现与验证前，不应对外承诺：

1. 已支持任意窗口大小
2. 已支持所有宽高比
3. 运行中热切换一定完全稳定
4. 当前 `Settings` 页已经天然具备显示设置能力

当前能诚实说的只有：

- 这件事在 Unity 与本项目结构上都可做
- 但最安全的收法是分阶段
- 第一阶段应当只收“首发默认固定窗口化”

## 最终推荐

### 推荐一句话版本

> 对 `Sunset` 当前最安全的窗口化方案，是先把 Windows 打包首发改成固定 16:9 的 Windowed，并暂时保持不可自由缩放；等这条稳定后，再补玩家可见的 `Windowed / FullScreenWindow` 切换与本地偏好保存。

### 为什么是这个方案

因为它同时满足三件事：

1. 回应老师“游戏应该支持窗口化”的真实反馈
2. 不把项目一下子拖进超宽/自由缩放的大验证坑
3. 不会把当前工作区从“打包窗口化”漂成“完整系统设置重构”

## 本轮未做

- 未改任何代码
- 未改 PlayerSettings
- 未跑任何 live 打包
- 未验证具体默认尺寸哪一个最终最合适

## 本轮证据分层

- **结构 / checkpoint**：成立
  已确认当前默认全屏来源、现有设置入口形态、相机与 UI 风险面

- **targeted probe / 局部验证**：成立
  已确认项目没有运行时显示模式链，且相机/UI 对屏幕尺寸确实敏感

- **真实入口体验**：未成立
  因为本轮没有真正改成窗口化并跑打包

所以当前正确口径只能是：

> 方案已经想清，但体验结论还不能冒充已经验证通过。

## 窗口化后的 UI 调整判断

这一段专门回答用户后来追问的那句：

> “是不是其实只有背景需要适配，其他 UI 不需要自适应？”

当前只站住 **结构 / 局部验证**，还没到真实入口体验，但就现有 `Town` 场景、UI 层级和脚本合同来看，**这个方向大体是对的**。

更准确地说，不是“所有 UI 都要做响应式”，而是要拆成三组：

### A. 必须调整 / 真正要优先盯的

#### 1. 世界可见区域 / 相机合同

这不是传统 UI，却是窗口化后最先出问题的“背景层”：

- `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`
- `Town` 当前主地表是多层 `Grid/Tilemap`
- 相机历史上已经有过全屏/超宽导致 confiner 与 viewport 异常的真实事故

如果窗口化后玩家看到“背景露边、蓝缝、世界裁切、左右露出空色”，优先修这里，而不是先怪 UI。

#### 2. 全屏背景 / 遮罩类面片

在 `Town` 当前 UI 里，真正会跟窗口边界直接发生关系的，不是卡片正文，而是这种“铺满整屏”的背景层：

- `UI/PackagePanel/Background`
- `UI/PackagePanel/Main`
- 根 `UI` Canvas 自身的屏幕合同

原因不是它们内容复杂，而是它们天然贴着屏幕边。

这类层在固定 `16:9` 窗口下通常风险不大，但一旦放开任意比例，就会比卡片正文更先暴露“空边、拉伸、遮罩不满、暗底露白”等问题。

### B. 建议重点检查，但大概率不用重做响应式

这批对象更像“边缘 containment 检查”，不是“重新设计一套自适应布局”：

- `Assets/YYY_Scripts/Service/Player/HealthSystem.cs`
- `Assets/YYY_Scripts/Service/Player/EnergySystem.cs`
- `Assets/YYY_Scripts/UI/Inventory/HeldItemDisplay.cs`
- `Assets/YYY_Scripts/UI/Inventory/ItemTooltip.cs`
- `Assets/YYY_Scripts/TimeManagerDebugger.cs`

原因：

1. 这几条链都直接读了 `Screen.width/height`，或者在屏幕尺寸变化时主动重算位置。
2. 它们已经在代码里承认“窗口变化会影响自己”，所以窗口化后需要复核。
3. 但它们当前更像“别跑出边界”的 containment 逻辑，而不是要把视觉壳体改成流式布局。

其中：

- `HealthSystem` / `EnergySystem` 已经有 `UpdateResponsiveLayoutIfNeeded()`，说明它们本来就在做“边缘保底”。
- `HeldItemDisplay` / `ItemTooltip` 也已经在 clamp 到 canvas / screen rect。
- `TimeManagerDebugger` 更像调试 HUD；如果最终发行不露它，它不是首要风险。

### C. 当前不建议动 / 不该先改成响应式的

这批更像“正式像素风壳体”，应该优先守住固定视觉语言：

- `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
- `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
- `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
- `Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs`

支撑这个判断的现场证据包括：

1. `InteractionHintOverlay/HintCard` 当前就是固定卡片，约 `284x74`，固定贴左下角。
2. `SpringDay1PromptOverlay/TaskCardRoot` 当前就是固定任务卡，宽约 `328`，不是流式正文页。
3. `PackagePanel/Top` 当前也是固定宽度的主壳，约 `1265x128`，不是随窗口自由铺开的 tab 条。
4. `PackageSaveSettingsPanel` 用的是固定壳体 + 纵向内容流，不是依赖 `Screen.width` 的自由排版页。

所以如果后续真开工，默认策略不该是：

- 把这些正式壳体全部改成“会跟窗口宽度一起伸缩”

而应是：

- 先守住固定壳体
- 只处理屏幕边缘 containment
- 真要适配，也优先改背景层和可见区域，不先改卡片壳体

## 蓝线问题判断

用户后来补的“地上那条蓝线”，当前判断应与 UI 适配拆开看。

### 当前结论

**它更像世界渲染 seam / tile seam / viewport 采样缝，不像 UI 没适配。**

### 为什么这样判断

#### 1. 蓝线出现在地表 / 道路 / tile 世界里，不贴 UI 边

这首先就把它和下面这些问题区分开了：

- CanvasScaler 不统一
- 面板没自适应
- HUD 壳体偏位

#### 2. 相机背景清屏色本身就是蓝色

`Main Camera` 当前背景色是偏蓝值：

- `r≈0.19`
- `g≈0.30`
- `b≈0.47`

如果玩家看到的是“蓝线”而不是黑线/白线，这和“tile 之间露出了相机背景色”是高度一致的。

#### 3. 项目里确实已经有同家族历史问题

仓库里的旧截图：

- `Assets/Screenshots/camera_left_edge_runtime_probe_farleft.png`
- `Assets/Screenshots/camera_after_fix.png`

都能看到典型的相机边缘蓝色露出。这说明：

- 这不是凭空猜的新问题
- 项目历史上确实已经存在“viewport / confiner / 边缘露底色”家族问题

#### 4. 当前世界层就是多层 Tilemap，而不是单张背景图

`Town` 现在的主要地表来自：

- `SCENE/LAYER 1/Tilemap/基础地皮/Layer 1 - Base`
- `SCENE/LAYER 2/Tilemap/Layer 2 - Base`
- 农田 / 轨道 / Props 等多层 `TilemapRenderer`

窗口化后如果出现蓝缝，更符合：

- 多层 tile 边缘采样
- camera rect / pixel snapping
- 非整数缩放下的 seam

而不是“某个 UI 背景没有拉满”。

#### 5. 当前相机脚本就有最可疑的一条链

`CameraDeadZoneSync` 里现在仍然会：

- 根据 `Screen.width / Screen.height` 算宽屏 clamp
- 改 `mainCamera.rect`
- 再做 `SnapViewportRectToPixelGrid`

也就是：

- `ApplyWideScreenViewportClamp()`
- `SetMainCameraRect(...)`
- `SnapViewportRectToPixelGrid(...)`

这一类“先改 viewport，再按屏幕像素取整”的逻辑，本来就是窗口态 / 宽屏 / 非整数尺寸最容易出缝的位置。

#### 6. 资产侧也支持 seam 风险

当前额外查到：

- 项目里没有找到 `.spriteatlas / .spriteatlasv2`
- 相关 tile 贴图虽然是 `filterMode: 0`（Point）
- 但 `spriteExtrude` 只有 `1`

这意味着：

- “像素风 + Point Filter” 本身成立
- 但没有 atlas padding / 足够 extrude 时，窗口化或非整数缩放仍可能露缝

### 当前最可能的根因排序

#### 第一位：相机 viewport / clamp / pixel snap 导致的世界露底色

这是我当前最怀疑的一条。

#### 第二位：tile 采样缝在非整数缩放下暴露

尤其在没有 SpriteAtlas 的前提下，这个风险会更大。

#### 第三位：个别 tile / road 资源切片边缘本身不够安全

这不是不可能，但当前证据还不够支持把锅先甩给某张素材。

### 当前明确不建议误判成

1. “只是 UI 没适配”
2. “背包/对话/任务卡需要响应式重做”
3. “单纯某张地板图画坏了”

## 把两件事分开后的正式判断

所以对这轮问题，当前最稳的项目级结论应写成：

1. **窗口化后的 UI 不是要全面响应式重做。**
   更像是“世界可见区域 + 全屏背景层 + 少数边缘 HUD containment”需要处理。
2. **用户说‘可能只有背景需要适配’这个方向，当前看大体成立。**
   但这里的“背景”不只是 UI 背景，还包括相机可见世界和 viewport 合同。
3. **那条蓝线不是 UI 主问题。**
   当前更像窗口态下被放大的 `camera rect / tile seam / 采样缝` 问题。

## 用户最新纠偏后的收窄结论

用户最新一句的纠偏是对的：这轮不该再泛谈“哪些 UI 都要适配”，而应收窄成：

- 背包里真正要单独考虑全屏适配的，是 `PackagePanel/Background`
- 其他背包正文壳体先不要动
- 地上蓝线要单独按世界渲染问题看

### `PackagePanel/Background` 能不能单独做

当前结构证据支持：**能，而且这是相对安全的方向。**

原因不是它现在没拉伸，而是它已经在自己的层里拉满了，只是被更外层固定壳体限制住：

- `PackagePanel` 根本身是固定中心锚点，尺寸 `1920 x 1080`
- `PackagePanel/Background` 容器已经是 `anchorMin=(0,0)`、`anchorMax=(1,1)`、`sizeDelta=(0,0)`
- 真正铺图的 `PackagePanel/Background/Image` 子物体也已经是全拉伸

所以当前真正的问题不是 `Background` 自己没铺满，而是：

- 它只能铺满固定的 `PackagePanel`
- 还没有脱离 `1920 x 1080` 这层面板壳体去覆盖整个窗口

### 为什么说这条路相对安全

因为 `Background` 当前更像视觉层，不是交互核心区。

`InventoryInteractionManager` 里已经明确写了：

- 回退背包区域只看 `Main + Top`
- **不包括 `Background`**

这意味着如果后续真施工，正确方向会是：

- 让 `PackagePanel/Background` 这一层单独贴窗口全屏
- `Main`、`Top`、`InventoryBounds` 继续保持现在的固定壳体

这样更符合用户现在的真实需求，也比“把整套背包都改成响应式”安全。

### 蓝线还能不能处理

**不是完全没办法，但它不属于这条 `Background` 适配责任链。**

当前更像两类原因：

1. `CameraDeadZoneSync` 这条 `Camera.rect + viewport clamp + pixel snap` 链在窗口态下露出了蓝色清屏底色
2. Tilemap 在非整数缩放下出现 seam，而项目当前又没有 SpriteAtlas，只靠 `spriteExtrude: 1`

所以蓝线不是“无解”，只是它更像第二优先级的世界渲染问题，不该拦住这轮对 `PackagePanel/Background` 的结构判断。

### 这轮能站住到哪一层

这轮目前只站住：

- `结构 / checkpoint`

也就是说：

- **我可以负责任地说，`PackagePanel/Background` 单独适配这条路是可行且相对安全的**
- **但我还不能把它说成已经过体验线**

真要过体验线，还是得等后续真实实现和窗口态烟测。
