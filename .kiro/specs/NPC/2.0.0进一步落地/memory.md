# NPC 2.0.0 进一步落地 memory

## 2026-03-25

- 当前主线目标：
  - 把 NPC 从“功能验证对象”正式推进到“可持续扩展的角色系统设计阶段”，并把本轮用户新增的大需求收成稳定的 2.0.0 工作区入口。
- 本轮子任务：
  - 新建 `2.0.0进一步落地` 的工作区记忆。
  - 新建 `需求拆分.md`，完整记录用户本轮详细 prompt 与系统级拆分。
  - 将 `NPC-导航接入契约与联调验收规范` 迁移到 2.0.0，并把它确认为后续唯一维护版本。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC-导航接入契约与联调验收规范.md`
  - 将 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md` 收口为迁移说明，避免继续双份正文并存
- 本轮锁定的核心需求：
  - 文档治理上：NPC 同类设计文档要少而精，尤其导航契约必须只维护一个最终版
  - 业务设计上：开始进入 NPC 场景化真实落点、人设化气泡与相遇对话、受击/工具命中/反应兼容、好感度与玩家/NPC 双气泡体系
  - 阶段边界上：导航契约仍是独立协作文档，但 2.0.0 需要开始承接更大的 NPC 产品化设计
- 当前恢复点：
  - 后续 NPC 设计型工作优先进入 `2.0.0进一步落地`
  - `1.0.0初步规划` 仍保留历史救援与早期收口记录，但导航契约的当前唯一维护版本已经迁到 2.0.0

## 2026-03-25｜2.0.0 第一刀实现：角色化 profile 与生成器默认映射

- 当前主线目标：
  - 在用户批准 2.0.0 文档结构后，先把现在就能安全落地、又不撞导航主战场的 NPC 内容真正落进 `main`。
- 本轮子任务：
  - 补齐 `2.0.0进一步落地` 的 4 份主文件。
  - 为 `001 / 002 / 003` 建立独立角色化 `NPCRoamProfile`。
  - 让 prefab 与生成器默认映射到这套角色化 profile，而不再共享一份通用环境话术。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC场景化真实落点与角色日常设计.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC交互反应与关系成长设计.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC系统实施主表.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefRoamProfile.asset`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterRoamProfile.asset`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchReviewProfile.asset`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\001.prefab`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\002.prefab`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab`
- 本轮关键实现：
  - `001` 当前默认走村长 profile，日常自言自语和对聊口吻更偏“稳、看局面、管节奏”。
  - `002` 当前默认走村长女儿 profile，口吻更生活化、更柔和、更带环境感受。
  - `003` 当前默认走研究型 review profile，同时 `NPCBubbleStressTalker.testLines` 也被切成研究记录风格，不再是纯测试句。
  - 生成器现在会按 NPC 名称自动选 profile：`001 -> VillageChief`、`002 -> VillageDaughter`、`003 + BubbleReview -> ResearchReview`。
- 本轮验证：
  - `git diff --check` 已通过本轮 NPC 文档、asset、prefab 与生成器脚本。
  - prefab 当前引用的新 profile GUID 已与对应 `.meta` 闭合：
    - `001 -> 0c11f4f44e5a4dbd93de8c2fd8c06001`
    - `002 -> 0c22a5d55f6b4ecd84af9d3ea7d27002`
    - `003 -> 0c33b6e6607c4fed95b0ae4fb8e38003`
  - 本轮没有进入 Unity / MCP live 写，也没有触碰 `Primary.unity`、`NPCAutoRoamController.cs`、`NPCMotionController.cs`、导航核心或玩家导航核心。
- 当前恢复点：
  - `2.0.0进一步落地` 现在已经不是纯文档工作区，而是开始承接角色化数据与 prefab 基线。
  - 下一刀若继续安全推进，应优先考虑真实场景落点、双气泡规范或关系成长入口；导航运动语义仍保持不越界。

## 2026-03-25｜等待场景热区期间的独立推进：玩家失败反馈气泡与水壶运行时闭环

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 2adbc011`
- 当前主线目标：
  - 在用户暂时占用 Unity 场景、明确要求“不使用 MCP / 不打扰场景”的前提下，继续推进一条不撞热区、又能服务后续 NPC / 玩家双气泡与交互体验的独立代码切片。
- 本轮子任务：
  - 把玩家工具失败反馈与水壶运行时状态链收成可交付闭环。
  - 修掉水壶静态 tooltip 与运行时容量回退规则不一致的问题。
  - 用纯本地代码闸门验证，不进入 Unity / MCP / Play Mode。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerToolFeedbackService.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\TreeController.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\InventoryItem.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\ToolRuntimeUtility.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Items\ToolData.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerInteraction.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\InventorySlotUI.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Inventory\ItemTooltipTextBuilder.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\UI\Toolbar\ToolbarSlotUI.cs`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\ToolRuntimeFeedbackTests.cs`
- 本轮关键实现：
  - 玩家现在有独立的失败反馈服务，能在工具损坏、斧头等级不足、水壶没水时触发“玩家自己的想法气泡 + reject shake + 音效/粒子”。
  - 工具提交链已从简单 `bool` 升级为 `ToolUseCommitResult`，后续可继续承接更细的失败原因与反馈分流。
  - 水壶正式从“假装走耐久”切到“运行时水量属性”：
    - `watering_current`
    - `watering_max`
  - 背包 / 工具栏耐久条会对水壶隐藏，tooltip 会显示当前水量与水量上限。
  - `ToolData.GetTooltipText()` 已与 `ToolRuntimeUtility.GetWaterCapacity()` 对齐，消掉“tooltip 显示 1、运行时按 100”这类口径错位。
- 本轮验证：
  - 首次缩小白名单运行 `CodexCodeGuard` 时，暴露出 `ToolData.cs` 依赖 `ToolRuntimeUtility.cs` 的真实 owned 范围，这一轮据此把完整闭环重新圈定。
  - 之后对 11 个 owned C# 文件重跑 `CodexCodeGuard`，结果通过：
    - `utf8-strict`
    - `git-diff-check`
    - `roslyn-assembly-compile`
  - 本轮未触碰 `Primary.unity`、导航核心、玩家导航核心、`GameInputManager.cs`、Unity / MCP / Play Mode。
- 当前恢复点：
  - 这条切片本质上是在为后续 NPC / 玩家双气泡、关系反馈和玩家侧表达体验打基础，不依赖当前场景热区。
  - 后续只要用户仍在占用场景，我仍可继续沿“非热区、纯代码、可白名单收口”的方向补独立切片；一旦要做 live 表现验收，再等用户放开 Unity。

## 2026-03-25｜003 测试模式正规化：默认不再自动抢占正式漫游

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ bd5e588d`
- 当前主线目标：
  - 在用户释放 MCP 后，重新回到 NPC 自身的 live 验证，优先把“003 仍像测试 NPC 一样自动锁死自己”这类污染正式使用的问题真正收掉。
- 本轮子任务：
  - 查清 `003` 为什么进 Play 后始终 `Inactive`。
  - 让 `003` 的压力测试组件改成“默认不自启”，避免继续污染正式 NPC 漫游。
  - 用最小 Play 窗口验证修正是否生效，并确保最后退回 Edit Mode。
- 本轮证据：
  - 只读扫描 `Primary` 后确认：
    - `001 / 002` 在 Play 中会进入 `ShortPause / IsRoaming=true`
    - `003` 同时保持 `Inactive / IsRoaming=false`
  - 继续只读回查组件发现根因不是导航：
    - `NPCBubbleStressTalker.startOnEnable = true`
    - `NPCBubbleStressTalker.disableRoamWhileTesting = true`
    - 也就是 `003` 会在 `OnEnable` 时主动把自己的 `NPCAutoRoamController` 关掉
- 本轮完成：
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab`
  - 将 `NPCBubbleStressTalker.startOnEnable` 调整为 `false`
- 本轮验证：
  - 修改后，scene 实例 `NPCBubbleStressTalker.startOnEnable` 已同步显示为 `false`
  - 再次短窗口 Play 取证时：
    - `003` 已进入 `ShortPause`
    - `IsRoaming = true`
    - `NPCBubbleStressTalker.TestModeEnabled = false`
  - Stop 后已确认：
    - 当前回到 Edit Mode
    - `Primary` 仍 `isDirty = false`
    - console `error / warning = 0`
- 当前恢复点：
  - `003` 现在默认是正式 NPC，会正常参与漫游；若要做气泡压力测试，需要显式打开测试模式，而不会再自动污染日常场景。
  - 仍未推进的 scene 集成项是 `HomeAnchor` 真实配置；这一步需要动 `Primary.unity`，要继续看热场景占用与用户是否要我直接进 scene 写入。

## 2026-03-25｜003 测试模式工具链正规化与回归测试补位

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 4c62ef05`
- 当前主线目标：
  - 不让 `003.prefab` 上那次“默认不自启”的修正只停留在 prefab 一处，而是把生成器、scene 集成工具、巡检入口和回归测试一起收口，避免后续工具再把正式 NPC 污回测试模式。
- 本轮子任务：
  - 补齐 `NPCBubbleStressTalker` 的显式模式接口。
  - 让 `NPCPrefabGeneratorTool` 改为“BubbleReview 必须显式 opt-in”。
  - 让 `NPCSceneIntegrationTool` 在正式模式下禁用测试自动启动，而不是粗暴删除组件。
  - 让 `NPCAutoRoamControllerEditor` 能直接看到并切换 stress auto-start。
  - 增加一份纯 Editor 回归测试，固定这组语义。
- 本轮完成：
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubbleStressTalker.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCSceneIntegrationTool.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCToolchainRegularizationTests.cs`
- 本轮关键实现：
  - `NPCBubbleStressTalker` 现在默认 `startOnEnable = false`，并提供：
    - `StartOnEnable`
    - `DisableRoamWhileTesting`
    - `ConfigureMode(...)`
  - `NPCPrefabGeneratorTool` 不再默认把 `003` 自动归到 BubbleReview；只有显式填入验证样本名称时才会自动挂压测模式，且生产态 `003` 会继续走研究型 profile。
  - `NPCSceneIntegrationTool` 现在支持按 NPC 推荐 profile 批量落正式配置，并在 Production 模式下把 stress talker 调整为“手动触发”，而不是直接移除。
  - `NPCAutoRoamControllerEditor` 现在会直接显示 stress auto-start 当前状态，并提供开/关按钮，减少手改 Inspector 的误操作。
  - 新增 `NPCToolchainRegularizationTests`，覆盖：
    - stress talker 默认手动启动
    - `ConfigureMode(...)` 模式切换
    - scene integration 推荐 profile 映射
    - prefab generator 的显式 BubbleReview opt-in 口径
- 本轮验证：
  - `git diff --check` 对上述 5 个 C# 文件通过。
  - `CodexCodeGuard` 对上述 5 个 C# 文件通过，覆盖：
    - `utf8-strict`
    - `git-diff-check`
    - `roslyn-assembly-compile`
  - MCP 基线复核通过：
    - `unityMCP`
    - `http://127.0.0.1:8888/mcp`
    - 当前实例仍是 shared root `Sunset`
  - 已做一次脚本级 `refresh + compile`，确认我本轮最初引入的测试程序集直连类型错误已清零。
  - 当前 Unity console 剩余红错为外部 blocker：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
    - `PageRefs` 缺失
  - 本轮没有进入 Play Mode，也没有改 `Primary.unity`；当前 scene 仍 `isDirty = false`
- 当前恢复点：
  - `003` 测试模式从 prefab 到工具链已经完成一次完整正规化，不容易再被生成器或 scene 工具无意打回“默认压测 NPC”。
  - 由于 Unity console 当前仍被 `SpringDay1PromptOverlay.cs / PageRefs` 外部编译错误占住，这轮没有继续硬跑 Unity EditMode 测试作业；后续可在外部 blocker 清掉后直接点跑 `NPCToolchainRegularizationTests` 做完整 Unity 侧复验。
  - NPC 下一刀真正还值得做的，仍然是 scene 级 `HomeAnchor` 落点与正式集成，但那已经进入 `Primary.unity` 热区，不再属于这类非热区小事务。

## 2026-03-25｜工具链收口后的最小扫尾

- 当前主线目标：
  - 不把这轮新加测试文件的 `.meta` 和旧文档起稿残留继续留在 working tree 里脏着。
- 本轮子任务：
  - 删除已被正式 2.0 文档取代的早期起稿文件。
  - 把 `NPCToolchainRegularizationTests.cs.meta` 纳入正式版本控制。
- 本轮完成：
  - 删除 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-25_NPC导航接入契约与联调验收规范起稿.md`
  - 保留并准备收口 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCToolchainRegularizationTests.cs.meta`
- 当前恢复点：
  - NPC 这轮自己的直接尾巴只剩最小 follow-up 收口，不再留“早期起稿 + 新脚本没 meta”的脏尾巴。

## 2026-03-25｜本轮 NPC 安全推进边界确认

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 55e2bccd`
- 当前主线目标：
  - 在已经完成 `003` 工具链正规化与尾巴清理后，确认这轮 NPC 是否还存在可继续安全推进的独立实现切口。
- 本轮只读核查结论：
  - 当前已经没有“既属于 NPC 自己、又不撞热区、也不依赖外部编译 blocker”的下一刀可直接继续实现。
  - 后续真正仍属于 NPC 的未完成项，按优先级主要是：
    - `001 / 002 / 003` 的 `HomeAnchor` scene 落点
    - 正式活动区域 / 路线 / 相遇节奏的 scene 级配置
    - 导航线程交付后的 NPC 侧联调与体验验收
    - `NPCToolchainRegularizationTests` 的 Unity Test Runner 实跑复验
  - 其中：
    - `HomeAnchor / 路线 / 活动区域` 会进入 `Primary.unity` 热区
    - Unity 测试复验当前仍受外部 blocker 影响：
      - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
      - `PageRefs` 缺失
- 当前恢复点：
  - 后续如果继续 NPC 实现，最合理的下一条是 scene 级正式集成，而不是继续在非热区里硬凑无关小刀。
  - 在 `Primary.unity` 热区未释放、或外部编译 blocker 未清之前，本线程应停在当前 checkpoint，不再扩题。

## 2026-03-26｜重新复核 Primary 场景开工条件

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 359331b9`
- 本轮目标：
  - 只读判断 `Primary.unity` 现在是否已经满足 NPC scene 级开工条件。
- 本轮证据：
  - shared root 仍为：
    - `owner_mode = neutral-main-ready`
    - `current_branch = main`
    - `is_neutral = true`
  - `Primary.unity` 当前没有 active lock：
    - `Check-Lock.ps1` 返回 `state = unlocked`
  - Unity / MCP live 基线通过：
    - `unityMCP`
    - `http://127.0.0.1:8888/mcp`
    - 当前会话 resources / templates 均来自 `unityMCP`
  - Editor 当前状态：
    - Edit Mode
    - 非 compiling
    - 非 domain reload
    - `Primary` 已加载
    - scene 内存态 `isDirty = false`
  - console 当前仅剩 2 条字体 warning：
    - `DialogueChinese V2 SDF` 缺 Ellipsis 字符
  - 但 Git working tree 中：
    - `Assets/000_Scenes/Primary.unity` 仍为 `M`
    - 且 `git diff --stat` 显示该 scene 不是零漂移，而是实质性 diff
  - 同时共享表现层资源仍为 dirty：
    - `DialogueChinese BitmapSong SDF.asset`
    - `DialogueChinese Pixel SDF.asset`
    - `DialogueChinese SoftPixel SDF.asset`
    - `DialogueChinese V2 SDF.asset`
    - `LiberationSans SDF - Fallback.asset`
- 本轮裁定：
  - `Primary.unity` 的“安全写窗口”仍未成立。
  - 原因不是 Unity 正在忙，也不是锁被别人占着，而是：
    - 热 scene 文件本身已经在 Git working tree 里 dirty
    - 当前又没有 active lock 把它显式归属到本线程
    - 同时共享表现层资源也在 dirty，继续写 scene 会放大串扰风险
- 当前恢复点：
  - 若要正式进入 NPC scene 集成，下一步不是直接开工，而是先明确 `Primary.unity` 当前 dirty 归属，再建立独占写窗口。

## 2026-03-26｜NPCV2 首轮启动委托-02：Primary / HomeAnchor 准入裁定

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ ee3187573b62891a5b0a8d974f43c192c4125a34`
- 当前主线目标：
  - `NPCV2` 接班后先判断 `Primary.unity` 的安全写窗口是否已经成立，只决定能否进入 `HomeAnchor` 最小 scene 切片。
- 本轮子任务：
  - 严格按 `2026-03-26-NPCV2首轮启动委托-02.md` 只读复核 `cwd / branch / HEAD`、`git status --short -- 'Assets/000_Scenes/Primary.unity'`、`git diff --stat -- 'Assets/000_Scenes/Primary.unity'`、`shared-root-branch-occupancy.md`、`mcp-single-instance-occupancy.md`、`mcp-hot-zones.md`，并只在 `Primary.unity` 内搜索 `HomeAnchor` 直接证据。
- 本轮关键证据：
  - `cwd = D:\Unity\Unity_learning\Sunset`
  - `branch = main`
  - `HEAD = ee3187573b62891a5b0a8d974f43c192c4125a34`
  - `git status --short -- 'Assets/000_Scenes/Primary.unity'` 仍为 `M`
  - `git diff --stat -- 'Assets/000_Scenes/Primary.unity'` 仍为 `80 lines / 76 insertions / 4 deletions`
  - `shared-root-branch-occupancy.md` 仍写 `main + neutral`，但 `last_verified_head = 98cbb88b`，它只能说明 shared root 入口中性，不能当成 `Primary.unity` 写许可
  - `mcp-single-instance-occupancy.md` 当前 `current_claim = none`，但默认口径仍是 `single-writer-only`
  - `mcp-hot-zones.md` 仍将 `Primary.unity` 列为热区 B / C
  - 在 `Primary.unity` 内直接搜索 `HomeAnchor` 无命中；当前 diff 片段读到的是 `StoryManager`、Workbench overlay、debug flag 与位置改动，不是 NPC `HomeAnchor` 半成品
- 本轮裁定：
  - 当前是否确认 scene 写窗口成立：`no`
- V2 第一刀或第一 blocker：
  - 第一 blocker 是 `Primary.unity` 已 mixed dirty 且归属未明；不能把 `shared-root-branch-occupancy = neutral` 误当成 `Primary.unity` 可直接写
- 当前恢复点：
  - `NPCV2` 这轮停在 blocker；只有在 `Primary.unity` 当前 dirty 归属说清、scene 热区拿到独占写窗口后，下一刀才回到 `scene audit -> HomeAnchor`

## 2026-03-26｜NPCV2 再次只读复核：条件仍未满足

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 519d51bd20d98e662eafb94cea0c5bbbeb314cec`
- 当前主线目标：
  - 再次只读确认 `Primary.unity` 的 scene 写窗口是否已成立，判断 `NPCV2` 能否进入 `HomeAnchor` 最小 scene 切片。
- 本轮关键证据：
  - `git status --short -- 'Assets/000_Scenes/Primary.unity'` 仍为 `M`
  - `git diff --stat -- 'Assets/000_Scenes/Primary.unity'` 仍为 `76 insertions / 4 deletions`
  - `Check-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity'` 返回 `state = unlocked`
  - `shared-root-branch-occupancy.md` 仍是 `main + neutral`
  - `mcp-single-instance-occupancy.md` 仍是 `single-writer-only` 且 `current_claim = none`
  - `mcp-hot-zones.md` 仍将 `Primary.unity` 列为热区 B / C
  - 在 `Primary.unity` 内再次搜索 `HomeAnchor` 仍无命中
- 本轮裁定：
  - 当前是否确认 scene 写窗口成立：`no`
- 当前恢复点：
  - 这轮与上一轮相比，`HEAD` 已变化，但决定准入的 blocker 没变化；仍需先明确 `Primary.unity` 当前 mixed dirty 的归属，再谈 `scene audit -> HomeAnchor`

## 2026-03-26｜NPCV2 委托-03：共享根大扫除与 owner 报实

- 当前 live 基线：
  - 启动基线：`D:\Unity\Unity_learning\Sunset @ main @ 1452bebb1171235b454d1d4fd961639caabdc930`
- 当前主线目标：
  - 只做 NPC 线 own dirty / untracked 的卫生清扫、owner 报实与白名单收口；不进入 `Primary.unity` / `HomeAnchor` 施工。
- 本轮 owner 报实：
  - 已确认 NPC own 面：
    - `.codex/threads/Sunset/NPC/memory_0.md`
    - `.kiro/specs/NPC/memory.md`
    - `.kiro/specs/NPC/2.0.0进一步落地/memory.md`
    - `.codex/threads/Sunset/NPC/V2交接文档/*`
    - `.kiro/specs/NPC/2.0.0进一步落地/2026-03-25_NPC工具链收口后卫生清扫与待命.md`
    - `.kiro/specs/NPC/2.0.0进一步落地/2026-03-26-NPC进入下一代交接前状态确认委托-01.md`
    - `.kiro/specs/NPC/2.0.0进一步落地/2026-03-26-NPCV2首轮启动委托-02.md`
    - `.kiro/specs/NPC/2.0.0进一步落地/2026-03-26-NPCV2共享根大扫除与owner报实-03.md`
  - 已明确不属于本轮可认领面：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - 导航线、农田线、TMP 字体与其他线程 memory / prompt 脏改
- 本轮白名单收口：
  - 已执行：
    - `scripts/git-safe-sync.ps1 -Action sync -Mode task -OwnerThread NPCV2 -IncludePaths '.codex/threads/Sunset/NPC/' '.codex/threads/Sunset/NPCV2/' '.kiro/specs/NPC/'`
  - 首次收口结果：
    - 提交 `eb6284fa`（`2026.03.26_NPCV2_01`）已推送到 `main`
    - NPC own 文档 / memory 大部分已收口
    - 仍残留一个 own 尾账：`.codex/threads/Sunset/NPCV2/memory_0.md` 未纳入，需最小 follow-up
- 当前恢复点：
  - cleanup 没有被偷换成业务复工，`Primary.unity` owner 裁定口径不变
  - 下一步只做最小 follow-up，把 `NPCV2/memory_0.md` 与本轮 cleanup 记忆补记一起白名单收口

## 2026-03-26｜NPCV2 恢复开工委托-04：Primary.unity 的 001/002/003 HomeAnchor 最小 scene 集成

- 当前 live 基线：
  - 开工前基线：`D:\Unity\Unity_learning\Sunset @ main @ 18cf7427d97e749b0557f6d835124e44787c3e17`
  - `Check-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity'` 开工前返回 `state = unlocked`
  - 本轮已通过 `Acquire-Lock.ps1` 为 `NPCV2` 获取 `Primary.unity` 的 A 类热文件锁
  - `shared-root-branch-occupancy.md` 仍是 `main + neutral`
  - `scripts/check-unity-mcp-baseline.ps1` 返回 `baseline_status: fail`、`issues: listener_missing`
- 当前主线目标：
  - 只在 `Primary.unity` 内完成 `001 / 002 / 003` 的 `HomeAnchor` 最小 scene 集成，并给用户交付可直接验收的详细汇报。
- 本轮子任务：
  - 先按热区陪跑口径复核 `cwd / branch / HEAD`、锁状态、shared root / MCP / hot-zones；
  - 再对 `001 / 002 / 003` 做 scene audit；
  - 最后只补 anchor 层级、scene 引用和基础位置，不扩到路线、停留点、相遇节奏、气泡或导航核心。
- 本轮完成：
  - 已在 `Assets/000_Scenes/Primary.unity` 的根层级 `NPCs` 下新增：
    - `NPCs/001_HomeAnchor`
    - `NPCs/002_HomeAnchor`
    - `NPCs/003_HomeAnchor`
  - 已把三者 `NPCAutoRoamController.homeAnchor` 分别回填为 scene Transform：
    - `001 -> {fileID: 910010002}`
    - `002 -> {fileID: 920020002}`
    - `003 -> {fileID: 930030002}`
  - 已固定三者基础位置：
    - `001_HomeAnchor`：局部 `(1.86, 0.63, 0)`，世界 `(-6.19, 6.29, 0)`
    - `002_HomeAnchor`：局部 `(-0.68, 0.49, 0)`，世界 `(-8.73, 6.15, 0)`
    - `003_HomeAnchor`：局部 `(1.7, -1.83, 0)`，世界 `(-6.35, 3.83, 0)`
  - 已生成用户验收文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工详细汇报-04.md`
  - 已完成离线 scene 自验：
    - `git diff --check -- 'Assets/000_Scenes/Primary.unity'` 通过
    - scene 内可直接回读三条 `homeAnchor` override 与三个 anchor 节点
- 本轮明确未做：
  - 未改 `GameInputManager.cs`
  - 未改导航核心、路线、停留点、相遇节奏、气泡、关系成长
  - 未把 `Primary.unity` 扩成 NPC 全量 scene 化
  - 未把 `unityMCP` 不可用时的离线回读误报成 live 验证通过
- 当前恢复点：
  - 结构层最小 scene 集成已落地；下一步是对白名单路径执行 `sync`，然后交给用户按详细汇报做 Unity 终验
  - 若后续用户在 Editor / Play 下看到 `HomeAnchor` 丢失或起点异常，应优先回到 `NPCAutoRoamController` 的运行态赋值链排查，而不是回头重开这轮 scene 最小落点

## 2026-03-26｜NPCV2 运行中 Inspector 补口回归修复

- 当前主线目标：
  - 保持 Unity 不停机的前提下，修掉 `NPCAutoRoamControllerEditor` 在运行中自动补 `HomeAnchor` 时抛出的 `InvalidOperationException`。
- 本轮子任务：
  - 只修编辑器侧的自动补口逻辑，不重开 scene 主刀，不改导航核心，不要求用户停 Unity。
- 本轮问题：
  - `TryAutoRepairPrimaryHomeAnchors()` 在 Play Mode 下仍走了 `Undo + EditorSceneManager.MarkSceneDirty` 持久化路径，导致：
    - `InvalidOperationException: This cannot be used during play mode.`
- 本轮修复：
  - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 已改成：
    - `Edit Mode` 才走 `Undo / ApplyModifiedPropertiesWithoutUndo / MarkSceneDirty`
    - `Play Mode` 只做运行时 anchor 创建与 `homeAnchor` 赋值，不再碰 scene dirty 持久化接口
  - 这样当前正在运行的 Unity 不会再因为 Inspector 自动补口而报 `MarkSceneDirty` 异常
- 当前恢复点：
  - 用户现在只需要等待脚本重新编译，并重新点回 `001 / 002 / 003` 的 Inspector，看 `Home Anchor` 是否自动回正

## 2026-03-26｜治理补充：`24886aad` 之后只允许做 `Primary.unity` own residue 报实

- 当前主线目标：
  - 在承认 `NPCAutoRoamControllerEditor.cs` 的 Play Mode 报错已修复的前提下，避免把当前 mixed hot 面一口吞成 `NPCV2` own cleanup。
- 治理新增事实：
  1. `24886aad` 的有效范围只到：
     - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
     - `NPCV2` 自己的记忆
     - 本工作区记忆
  2. 它不自动证明当前 `Primary.unity` 全部 dirty、TMP 字体 dirty、或导航线 `NPCAutoRoamController.cs` dirty 都归 `NPCV2`。
- 治理新增委托：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Primary归属报实与最小cleanup委托-06.md`
- 当前恢复点：
  - 若 `NPCV2` 继续施工，只允许：
    - 对 `Primary.unity` 做 own residue 报实；
    - 能切出纯 `HomeAnchor / Inspector auto-repair` 残留时，做最小 cleanup；
  - 不允许：
    - 吞整张 `Primary.unity`
    - 碰 `NPCAutoRoamController.cs`
    - 碰导航线脚本
    - 碰 TMP 字体
    - 进 Unity / MCP / Play Mode 再做 live 写

## 2026-03-26｜治理补充：`24886aad` 之后的下一轮 prompt 改为“Primary + 字体 owner 复核”

- 当前主线目标：
  - 不把 `NPCV2` 刚修掉的 editor 报错偷换成 runtime 继续施工，而是把它下一轮动作进一步压成底盘 owner 报实。
- 本轮新增事实：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 当前 dirty 仍在导航线，不属于 `NPCV2`。
  2. 当前 working tree 里的 3 份 `DialogueChinese*` dirty 字体，最近提交历史来自：
     - `3b2c0f1e`
     - `ee318757`
     也不是 `NPCV2` 的 `65e1ee35 / 24886aad`。
  3. 因此 `NPCV2` 下一轮不能碰导航 runtime，也不能直接吞字体 cleanup。
- 本轮新增委托：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-Editor修复后Primary与字体owner复核-07.md`
- 当前恢复点：
  - `NPCV2` 若继续，只允许：
    - 对 `Primary.unity` 剩余 mixed diff 做 own / non-own 复核；
    - 对 3 份 `DialogueChinese*` 字体做只读排除归属；
    - 仅在 `Primary.unity` 能切出纯 NPC own residue 时，做最小 cleanup。

## 2026-03-26｜根据最新自述回切主线：当前先修运行中 `Home Anchor` 补口链

- 当前主线目标：
  - 按 `NPCV2` 自己最新自述回切主线：当前优先先把运行中的 Inspector 里 `Home Anchor` 从空修到非空。
- 本轮新增事实：
  1. 用户当前最直接看到的阻塞仍是：
     - 运行中的 `Home Anchor` 可能还是空；
  2. 这一步比 owner cleanup 更接近用户体验，也更窄；
  3. 因此这轮不再让 `NPCV2` 先做 `Primary.unity + 字体` owner 报实主刀。
- 本轮新增委托：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2-HomeAnchor运行中补口链续工-08.md`
- 当前恢复点：
  - `NPCV2` 下一轮先继续窄修 `NPCAutoRoamControllerEditor.cs`；
  - `Primary.unity` mixed owner 与字体归属问题先退回支撑位，不作为这轮唯一主刀。

## 2026-03-26｜NPCV2 当前主线/支线与认领边界再确认

- 当前现场基线：
  - 当前仓库：`D:\Unity\Unity_learning\Sunset @ main @ 83d0a93319a365115f1d3354186e76693ac566e4`
  - working tree 仍有 mixed dirty：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 3 份 `DialogueChinese*` 字体
- 我现在明确认领什么：
  1. 已经落到 `main` 的两刀：
     - `65e1ee35`：`Primary.unity` 的 `001 / 002 / 003` 最小 `HomeAnchor` scene 集成
     - `24886aad`：`NPCAutoRoamControllerEditor.cs` 的 Play Mode 安全热修
  2. `NPCV2` 自己的线程记忆、工作区记忆、以及围绕上述两刀的解释与用户验收承接
- 我现在明确不认领什么：
  1. 当前 working tree 中的 3 份 `DialogueChinese*` 字体 dirty
  2. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 的当前 runtime / 导航 diff
  3. 当前整张 `Primary.unity` 的 mixed hot diff
- 对 `Primary.unity` 的进一步判断：
  - 当前 diff 中还能看到 `homeAnchor / *_HomeAnchor` 的痕迹，说明里面确实混有 NPC 这条线碰过的区域；
  - 但整体 diff 已经远超纯 `HomeAnchor / Inspector auto-repair residue` 的量级，当前不能安全把整张 scene 当成 `NPCV2 own cleanup`
- 当前该不该 cleanup：
  - `no`
- 当前主线：
  - 不是继续清 scene，也不是去接导航 runtime；
  - 而是把 `HomeAnchor` 最小集成 + Inspector 自动补口这两刀的责任边界守住，并等待用户对运行态结果做终验 / 继续反馈
- 当前支线：
  - 若治理侧需要收口，只做只读 owner 报实；
  - 只有未来能把 `Primary.unity` 中的纯 NPC residue 单独切干净时，才进入最小 cleanup
- 当前恢复点：
  - 从执行上，`NPCV2` 现在应停在“别再吞 mixed hot 面，先把边界说清楚并等用户裁定”

## 2026-03-26｜运行中 `Home Anchor` 显示链补口继续压窄

- 当前主线目标：
  - 继续只修 `Assets/Editor/NPCAutoRoamControllerEditor.cs`，让 Play Mode Inspector 里的 `Home Anchor` 从空变成可见非空；若还空，则把断点压成 Inspector 内直接可见的小点。
- 本轮子任务：
  - 不碰 `Primary.unity`、不碰 `DialogueChinese*`、不碰 `NPCAutoRoamController.cs` runtime，只补 Inspector 的 live 显示链。
- 本轮完成：
  1. 已复核 `Primary.unity` YAML：
     - `001 / 002 / 003` 的 `homeAnchor` prefab override 都已存在；
     - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 也都存在于 `NPCs` 根下；
     - 所以当前主要断点不再是 scene 集成缺口，而是运行中 Inspector 的显示 / 刷新链。
  2. 已在 `Assets/Editor/NPCAutoRoamControllerEditor.cs` 做最小补口：
     - 不再直接 `DrawDefaultInspector()`，改成手工绘制序列化字段；
     - `Home Anchor` 字段在 Play Mode 下优先显示 `controller.HomeAnchor` 的 live 引用；
     - 若序列化缓存与 live 引用不一致，会先同步 `_homeAnchorProperty` 再重绘；
     - 若运行中仍为空，会在 Inspector 里直接显示：
       - 找到了 sibling `*_HomeAnchor` 但 runtime 绑定仍空；
       - 或当前父节点下根本没找到可用 anchor。
  3. `TryAutoRepairPrimaryHomeAnchors()` 里复用了同一套 `FindExistingPrimaryHomeAnchor(...)` 查找逻辑，确保“自动补口”和“Inspector 提示”看的是同一个锚点来源。
- 本轮验证：
  - `git diff --check -- 'Assets/Editor/NPCAutoRoamControllerEditor.cs'`：通过
  - `unityMCP validate_script`：未通过，原因是当前会话拿不到 Unity session，属于外部验证阻塞，不是本轮已知脚本语法结论
- 当前恢复点：
  - 现在用户在不停 Unity 的前提下，重新选中 `Primary.unity` 的 `001 / 002 / 003` 时，应能直接看到：
    - `Home Anchor` 显示 live 引用；
    - 或最小 warning 文案，明确剩余断点已经压窄到“绑定没生效”而不是“scene 没 anchor”

## 2026-03-26｜口径纠偏：`NPCV2` 最新回执只代表局部待办，不代表整条 NPC 线只剩这些

- 当前主线目标：
  - 防止后续接手者把 `NPCV2` 在 `委托-08` 下的局部剩余工作，误听成整个 NPC 主线或整个项目只剩这些尾项。
- 本轮新增事实：
  1. `2026-03-26-NPCV2-HomeAnchor运行中补口链续工-08.md` 明确把 `NPCV2` 当前唯一主刀压成：
     - 只继续窄修 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
     - 目标是把运行中的 `Home Anchor` 从空修到非空，或把断点继续压窄
  2. 因此它后面报的“只剩这些”，准确含义是：
     - 只剩这条 `Editor / Inspector` 局部刀口的尾项
     - 不是整个 NPC 工作区、整个 NPC 系统、也不是整个 Sunset 的全量待办
  3. 从 `NPC` 主工作区与本工作区已有记录看，NPC 更大层面的未完成项仍包括：
     - `001 / 002 / 003` 的运行态最终实证
     - 正式 roam 区域 / 路线 / 相遇节奏
     - 导航线程交付后的 NPC 联调整体验收
     - `NPCToolchainRegularizationTests` 的 Unity Test Runner 实跑
  4. 另外还有一批当前明确不归 `NPCV2` 这轮吞并的内容：
     - `Assets/000_Scenes/Primary.unity` 的 mixed cleanup
     - 3 份 `DialogueChinese*` 字体
     - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 的 runtime / 导航 diff
- 当前恢复点：
  - 后续对用户汇报时，必须显式分成三层：
    - 整条 NPC 主线还没做完什么
    - `NPCV2` 当前这一刀还没做完什么
    - 哪些东西当前明确不归 `NPCV2` 认领

## 2026-03-26｜全需求回溯：补建 NPC 主线的完整需求底账

- 当前主线目标：
  - 不再只围绕 `NPCV2` 的局部尾项说话，而是把用户在 NPC 线上前后提出过的业务需求、设计诉求和工作流要求重新拉平成完整底账。
- 本轮完成：
  - 重新回读：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC场景化真实落点与角色日常设计.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC交互反应与关系成长设计.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC系统实施主表.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\V2交接文档\*.md`
  - 确认当前需求应至少分成 4 个阶段理解：
    1. 最早的 `4 向 3 帧 PNG -> 动画/控制器/Prefab/可运行 NPC` 生成器需求
    2. NPC phase2 的气泡、碰撞、漫游、测试入口与工具链收口需求
    3. 2.0.0 的角色化 / 场景化 / 双气泡 / 关系成长 / 命中反应需求
    4. `Primary.unity` 中 `001/002/003` 的 `HomeAnchor` 最小 scene 集成与运行中 Inspector 补口链
  - 纠偏结论：
    - 以后任何人都不能再把 `NPCV2` 当前 `HomeAnchor` 局部尾项，当成整条 NPC 主线的全量待办。
- 当前恢复点：
  - 现在已经可以按“阶段需求总表”的方式向用户汇报，而不是继续被某一刀的局部回执带偏。

## 2026-03-27｜文档回溯版总表落盘：把 NPC 全需求总表与时间线正式写进 `NPCV2` 目录

- 当前主线目标：
  - 用户要求不再依赖聊天残余记忆，而要完全基于历史 `memory`、交接文档、正式设计文档，把 NPC 线所有提出过的需求和设计要求重新拉平成一份可核查总表。
- 本轮完成：
  - 新建文档：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
  - 文档内容同时包含：
    1. 证据源清单
    2. 业务需求总表
    3. 协作 / 治理需求总表
    4. 时间线回溯
  - 每条需求都尽量按以下字段落盘：
    - 原文
    - 稳定转述
    - 来源文件
    - 当前状态
    - 是否已完成
  - 对找不到逐字聊天原话的部分，已明确标注为：
    - 最早保留到文档中的稳定记录
    - 或设计整理原文
  - 另外把缺口也写清了：
    - 旧 `npc规划001.md` 迁移指向的新路径在当前仓库中缺失，因此更早原话并不完整可回溯
- 当前恢复点：
  - 现在后续任何人如果要回答“你最早到底提了什么需求”“哪些是逐字原话、哪些是后续整理”“整条 NPC 线还剩什么”，都不应再只盯某一份尾部 `memory`，而应先读这份总表。

## 2026-03-27｜当前线程已切换为“全盘交接模式”，不再把局部尾项误当总线

- 当前新增结论：
  - 本轮已重新完整读取：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
  - 并据此确认：以后 `NPCV2` 的汇报与执行判断，必须固定分成 3 层：
    1. 整条 NPC 总线的完整需求与阶段位置
    2. `NPCV2` 当前这一刀的局部责任与尾项
    3. 当前明确不归 `NPCV2` 认领的 mixed / runtime / cross-thread 内容
- 当前三层口径重新钉死如下：
  1. NPC 总线未完成项仍包括：
     - `HomeAnchor` 运行态最终实证
     - 正式 roam 区域 / 路线 / 相遇节奏
     - 角色化日常内容大规模落地
     - 玩家 / NPC 双气泡正式实现
     - 好感度 / 关系成长实现
     - 命中 / 工具反应兼容实现
     - 导航线程交付后的 NPC 联调
     - Unity Test Runner 中与 NPC 工具链相关的正式实跑
  2. `NPCV2` 当前局部主刀仍只剩：
     - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 的运行中 `Home Anchor` live 实证 / 继续压窄断点
     - own 记忆与白名单收口
  3. 当前明确不归 `NPCV2` 吞并：
     - `Primary.unity` mixed cleanup
     - `DialogueChinese*` 字体
     - `NPCAutoRoamController.cs` 当前 runtime / 导航 diff
     - 导航核心中的动态占位 / 局部避障 / 会车 / 礼让框架
- 当前恢复点：
  - 后续如果用户再问“现在还剩什么”“下一步该干嘛”，必须先答这三层，再答当前最近一刀；不能再把 `委托-08` 的局部剩余项压扁成整条 NPC 主线只剩这些。

## 2026-03-27｜基于历史总表输出正式“全盘清盘清单”

- 当前主线目标：
  - 用户要求把 NPC 历史需求总览、当前未完成项和后续合理优先级重新做一次全盘梳理，并直接落成可验收文档。
- 本轮完成：
  - 新建正式文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-27-NPC全盘清盘清单与后续优先级方案.md`
  - 文档没有重复历史总表，而是基于它完成了 4 件事：
    1. 重新列出当前已可视为稳定基线的部分
    2. 把全量未完成项按：
       - 基线闭环
       - 场景化真实落点
       - 角色化内容
       - 轻交互 / 双气泡 / 关系成长
       - 受击 / 工具命中 / 反应
       - 联调与外部依赖
       重新归类
    3. 明确写出当前不该做的事：
       - `Primary.unity` mixed cleanup
       - `DialogueChinese*` 字体
       - 导航核心吞并
       - 在 `HomeAnchor` 基线未闭环前乱扩 scene 行为
    4. 给出正式优先级：
       - `P0` 当前基线闭环
       - `P1` 场景化真实落点最小可玩切片
       - `P2` 角色化内容稳定
       - `P3` 轻交互与双气泡
       - `P4` 关系成长最小接线
       - `P5` 命中 / 工具反应兼容
       - `P6` 导航联调与正式测试
- 当前恢复点：
  - 后续 NPC 线如果要继续排刀，应优先以这份“清盘清单”作为施工顺序入口，而不是再靠单个委托或局部尾项临时判断。

## 2026-03-27｜补建“可直接执行”的详细任务列表，作为后续唯一施工标准

- 当前主线目标：
  - 用户明确指出上一份“清盘清单”仍然偏宽，不足以直接作为后续逐条施工和验收的标准，因此本轮继续把它下钻成详细任务列表。
- 本轮完成：
  - 在：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\`
    下新增：
    - `2026-03-27-NPC全盘详细落地任务列表.md`
  - 新文档相对上一份宽口径清单，新增了这些硬字段：
    1. 任务编号
    2. 当前状态
    3. 任务类型
    4. 责任归属
    5. 前置条件
    6. 可碰范围 / 禁碰范围
    7. 具体动作
    8. 产出物
    9. 完成标准
    10. 失败判定
  - 任务已按 `P0 -> P6` 全量展开，覆盖：
    - `HomeAnchor` 基线闭环
    - 场景化真实落点最小可玩切片
    - 角色化内容正式成型
    - 轻交互 / 双气泡
    - 关系成长最小接线
    - 命中 / 工具反应兼容
    - 导航联调与正式测试
  - 同时显式把任务类型拆成：
    - `可直接施工`
    - `联合前置后施工`
    - `外部阻塞项`
    避免后续把不归当前 NPC 刀口直接吞并的内容误写成“我马上就能独立一条龙做完”
- 当前恢复点：
  - 后续如果继续推进，不能再只参考 `2026-03-27-NPC全盘清盘清单与后续优先级方案.md`；
  - 必须改以：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    作为逐条施工与逐条验收的正式标准。

## 2026-03-27｜P0 首次实做：只推进 HomeAnchor Editor 诊断链与基线复验包

- 当前主线目标：
  - 按 `2026-03-27-NPC全盘详细落地任务列表.md` 从 `P0` 正式开工，但不跳 `P1/P2`，先把 `HomeAnchor` 运行态闭环所需的 Editor 断点和复验材料补齐。
- 本轮子任务：
  - 只推进：
    - `T-P0-02`
    - `T-P0-03`
  - 同时如实复核：
    - `T-P0-01`
    - `T-P0-05`
    当前为什么还没法闭环。
- 本轮现场结论：
  - `Primary.unity` 内 `001 / 002 / 003` 的 `homeAnchor` scene override 仍在；
  - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 也仍在，且都挂在 `NPCs` 根下；
  - 这说明当前更像运行中 Inspector / auto-repair 补口链问题，而不是 scene 自身丢了 `HomeAnchor`。
  - `unityMCP` 当前是“基线 pass、会话层 no_unity_session”的混合状态：
    - `check-unity-mcp-baseline.ps1 = pass`
    - 但 `editor/state`、`project/info`、`manage_scene(get_active)` 仍返回 `no_unity_session`
  - 因此：
    - `T-P0-01` 当前应从 `pending` 改判为 `blocked-external`
    - `T-P0-05` 仍是 `blocked-external`
- 本轮完成：
  - 修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
  - 新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P0-HomeAnchor基线复验与失败判读.md`
  - 更新：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
- 本轮 Editor 侧最小补口：
  - `NPCAutoRoamControllerEditor` 现在会在 Play Mode 下输出更窄的 `Home Anchor` 诊断：
    - `Runtime Home Anchor`
    - `Serialized Home Anchor`
    - `Detected Anchor Candidate`
    - `Parent`
    - `Auto-repair`
  - 自动补口结果现在会记录并直接回显：
    - 已绑定成功
    - 调用了 `SetHomeAnchor(...)` 但 runtime 仍空
    - 缺 parent
    - 已存在 runtime 绑定
  - `FindExistingPrimaryHomeAnchor(...)` 现在会按：
    - `parent-sibling`
    - `self-child`
    - `scene-search`
    三层查找来源返回结果，避免继续只说“可能没找到”。
  - `Create Home Anchor` 现在与 scene 既有口径统一为“优先 parent sibling”，不再把新 anchor 挂成 child 后又让自动补口只找 sibling。
- 本轮验证：
  - `git diff --check` 已通过：
    - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
    - `2026-03-27-NPC全盘详细落地任务列表.md`
    - `2026-03-27-NPC-P0-HomeAnchor基线复验与失败判读.md`
  - 从 `Editor.log` 可见脚本改动后发生了多轮：
    - `Requested script compilation`
    - `Reloading assemblies`
    - `CompileScripts`
  - 当前没抓到我这轮新增的 `error CS` 证据；
  - 但由于 `unityMCP` 仍无 live session，本轮还不能把 Unity 运行态最终值声称为已确认。
- 当前 owned / external 边界：
  - 当前 owned 改动：
    - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
    - `0.0.1全面清盘` 下两份文档
  - 当前 external blocker：
    - `unityMCP` 会话层 `no_unity_session`
  - 当前明确未认领：
    - `Primary.unity` mixed cleanup
    - `NPCAutoRoamController.cs` runtime / 导航 diff
    - `DialogueChinese*` 字体
- 当前恢复点：
  - 下一步不是跳去 `P1`，而是先等这份新 Inspector 诊断在 Unity 里被用户复验；
  - 一旦拿到 `001 / 002 / 003` 的实际读数，再继续判断 `T-P0-02` 是否可判 `done`，以及 `T-P0-05` 是否能补成正式验证。

## 2026-03-27｜P1 方案卡定稿，并前置铺开 P2 内容资产层

- 当前主线目标：
  - 在 `Primary.unity` 热区仍未开放的情况下，不空转等待 scene 写窗口，先把 `T-P1-01` 正式收口，再把 `P2` 的角色内容资产层和映射骨架提前落地。
- 本轮子任务：
  - 收口 `T-P1-01`
  - 新增独立 `NPCDialogueContentProfile` 内容资产类型
  - 为 `001 / 002 / 003` 落正式内容资产，并把两两相遇矩阵接到现有 ambient chat
- 本轮完成：
  - 新增：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCDialogueContentProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchDialogueContent.asset`
  - 修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCRoamProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefRoamProfile.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterRoamProfile.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchReviewProfile.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCToolchainRegularizationTests.cs`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC高速推进与测试排队日志.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P1-01-三名NPC场景点位方案卡.md`
- 本轮关键实现：
  - `NPCRoamProfile` 现在支持挂接独立 `dialogueContentProfile`，从而把“漫游参数”和“角色内容”拆成两层。
  - 三名 NPC 现在各自拥有独立的：
    - 单人环境气泡
    - 玩家轻响应句池
    - 两两相遇矩阵
  - `NPCAutoRoamController` 当前相遇链已可按 partner `npcId` 取 pair-specific 句池，但没有改动导航路径算法本身。
  - 现有 prefab / 生成器仍继续通过 `roamProfile` 进内容层，不需要新开一条并行挂载链。
- 本轮验证：
  - `git diff --check` 已通过本轮代码、资产与文档。
  - `Editor.log` 已看到：
    - `Requested script compilation`
    - `CompileScripts`
    - `Reloading assemblies`
  - 当前未抓到与本轮文件直接对应的新 `error CS`。
  - `refresh_unity` 等待 ready 超时，`read_console` 仍返回 `no_unity_session`，因此这轮 live 验收仍记为外部验证受阻。
- 当前恢复点：
  - `T-P1-01` 已完成；
  - `T-P2-01 ~ T-P2-06` 的纯资产 / 代码层内容铺底已完成；
  - 下一步优先先回看 `T-P1-02` 的 scene 准入；如果热区仍未开放，就继续做不依赖 live 写的 `T-P3-01 / T-P2-07`。

## 2026-03-27｜Primary 继续 blocked 时，先把 P2 验收包与 P3-01 正式规范补齐

- 当前主线目标：
  - 用户已明确要求高速推进，因此在 `Primary.unity` 仍不能安全写的情况下，不在 scene 热区空转，而是继续完成不依赖 live 写的 `P2 / P3` 文档切片。
- 本轮完成：
  - 已再次只读复核：
    - `T-P1-02`
  - 当前结论仍是：
    - `Primary.unity` 保持 `blocked-hotfile`
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P2-07-角色化内容验收包.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P3-01-玩家与NPC双气泡视觉规范正式稿.md`
  - 已更新：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC高速推进与测试排队日志.md`
- 本轮关键结论：
  - `T-P2-07`
    - 当前已可判 `done`
    - 因为角色内容资产、映射链和 pair-specific 内容验收方式都已落出正式文档
  - `T-P3-01`
    - 当前已可判 `done`
    - 因为双气泡已经不再停留在口头方向，而是收成了正式视觉规范
  - 这轮没有碰：
    - `Primary.unity`
    - `DialogueChinese*`
    - `GameInputManager.cs`
    - 玩家侧 mixed hot-file
- 本轮验证：
  - `git diff --check` 已通过本轮两份新文档、任务账本和高速日志
  - 当前没有把 live 触发覆盖率包装成“已经拿到”
  - `unityMCP` 会话层不稳仍按外部受阻记账
- 当前恢复点：
  - 现在 `P2` 已经具备正式验收包，`P3-01` 也已定稿；
  - 下一步如果继续往前推，应优先评估：
    - `T-P3-02`
    是否存在不吞 mixed hot-file 的安全代码切口；
  - 如果没有，就继续按高速模式寻找下一项不撞热区的独立切片，而不是回去硬碰 `Primary.unity`。

## 2026-03-27｜P3-02 已先完成玩家气泡样式层

- 当前主线目标：
  - 在 `P3-01` 规范已定的基础上，先把玩家气泡的正式样式层落到代码里，但不扩到输入、轻响应触发或 scene。
- 本轮完成：
  - 已只修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
  - 玩家气泡当前已经具备：
    - 晨雾青绿色填充
    - 深青灰边框
    - 深墨色文字
    - 更轻一点的浮动
    - 更轻一点的尾巴跳动
  - 同时继续共用当前 NPC 已锁定的：
    - `10` 字一行
    - 边距骨架
    - 尾巴尺寸
    - show / hide 节奏
- 本轮边界：
  - 没有碰：
    - `GameInputManager.cs`
    - `Primary.unity`
    - `DialogueChinese*`
  - 但在准备白名单 sync 时，闸机额外指出：
    - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
    仍是当前 same-root remaining dirty
  - 经过复核，这份 `PlayerInteraction.cs` 未提交 diff 属于此前 NPC 线自己的：
    - 玩家失败反馈 / `ToolUseCommitResult`
    支撑链尾账
  - 当前不再把它当作外线 mixed 文件，而是作为同根旧尾账一并纳入收口判断
- 本轮验证：
  - `git diff --check` 已通过：
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 当前还没有 live 视觉实证，因此这轮只 claim 样式层实现完成
- 当前恢复点：
  - `T-P3-02` 可判 `done`
  - 下一步应继续转入：
    - `T-P3-03`
  - 重点是：
    1. 先把 `PlayerThoughtBubblePresenter.cs + PlayerInteraction.cs` 这组同根玩家侧改动安全收口
    2. 再找一条不碰 `GameInputManager.cs` hot-file 的轻响应接线入口

## 2026-03-28｜`T-P3-03` 已以纯 player-side 邻近反馈服务接上

- 当前主线目标：
  - 在不碰 `GameInputManager.cs`、不碰 `Primary.unity`、不改 `NPCAutoRoamController.cs` 的前提下，把“玩家靠近时的短反馈”正式接起来。
- 本轮完成：
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcNearbyFeedbackService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcNearbyFeedbackService.cs.meta`
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerToolFeedbackService.cs`
  - 当前实现路径：
    - 由 player-side service 自己扫描附近 `NPCAutoRoamController`
    - 读取各自 `roamProfile.PlayerNearbyLines`
    - 直接驱动对应 NPC 的 `NPCBubblePresenter` 播一句短反馈
- 本轮边界：
  - 没有碰：
    - `GameInputManager.cs`
    - `Primary.unity`
    - `NPCAutoRoamController.cs`
  - 所以这轮仍属于“安全非热区接线”
- 本轮验证：
  - `git diff --check` 已通过新增 service、`.meta` 与 `PlayerToolFeedbackService.cs`
  - 当前还没有 live 场景覆盖率证据，因此这轮只 claim 接线层完成
- 当前恢复点：
  - `T-P3-03` 可判 `done`
  - 下一步应继续进入：
    - `T-P3-04`
  - 重点是把“正式对话轨 / 日常气泡轨”分工收成不撞热区的安全切片。
