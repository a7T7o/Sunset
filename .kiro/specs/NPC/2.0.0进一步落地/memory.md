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
