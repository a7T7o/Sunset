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

## 2026-03-27｜P3 分轨与 P4 关系成长底座已连续收口

- 当前主线目标：
  - 在不碰 `Primary.unity`、不碰 `GameInputManager.cs`、不动导航 runtime 核心的前提下，继续沿非热区把 NPC 线往前推进，不在 scene 热窗前空转。
- 本轮子任务：
  - 完成 `T-P3-04`
  - 完成 `T-P3-05`
  - 完成 `T-P4-01 ~ T-P4-04`
- 本轮完成：
  - `T-P3-04`
    - `PlayerNpcNearbyFeedbackService` 现在会监听：
      - `DialogueStartEvent`
      - `DialogueEndEvent`
    - 同时也会主动复核：
      - `DialogueManager.IsDialogueActive`
    - 正式对话开始时，会主动回收此前的日常 NPC 轻反馈气泡
    - 正式对话进行中，不再继续弹新的日常近身反馈
  - `T-P3-05`
    - 已补出：
      - `2026-03-27-NPC-P3-05-轻交互与双气泡验收包.md`
  - `T-P4-01`
    - 已新增：
      - `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`
      - `Assets/YYY_Scripts/Service/Player/PlayerNpcRelationshipService.cs`
    - 当前已具备：
      - `陌生 / 认识 / 熟悉 / 亲近`
      四档关系阶段
  - `T-P4-02`
    - 已把 `PlayerNpcNearbyFeedbackService` 改成：
      - 先按 `npcId` 读取当前关系阶段
      - 再按阶段从 `NPCDialogueContentProfile` 取近身句
    - `001 / 002 / 003` 三份内容资产都已补出按阶段分流的玩家近身句
  - `T-P4-03`
    - 当前关系阶段已支持最小持久化：
      - `PlayerPrefs`
    - 内容层现在已可被关系阶段驱动
  - `T-P4-04`
    - 已新增最小验收入口：
      - `Tools/NPC/Relationship/全部设为/陌生`
      - `Tools/NPC/Relationship/全部设为/认识`
      - `Tools/NPC/Relationship/全部设为/熟悉`
      - `Tools/NPC/Relationship/全部设为/亲近`
      - `Tools/NPC/Relationship/全部清除持久化`
      - `Tools/NPC/Relationship/打印当前阶段`
    - 已补出：
      - `2026-03-27-NPC-P4-04-关系成长首版验收包.md`
- 本轮验证：
  - `git diff --check`
    - 已通过
  - 已补最小静态回归：
    - `SpringDay1DialogueProgressionTests`
      - 覆盖 `P3-04` 的对话占用抑制与气泡回收接线
    - `NPCToolchainRegularizationTests`
      - 覆盖关系阶段分流与阶段持久化
  - 当前没有把 live 体验覆盖率包装成已拿到
- 当前 owned / external 边界：
  - 当前 owned：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcRelationshipService.cs`
    - `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`
    - `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
    - `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
    - `Assets/Editor/NPC/PlayerNpcRelationshipDebugMenu.cs`
    - `Assets/111_Data/NPC/NPC_001_VillageChiefDialogueContent.asset`
    - `Assets/111_Data/NPC/NPC_002_VillageDaughterDialogueContent.asset`
    - `Assets/111_Data/NPC/NPC_003_ResearchDialogueContent.asset`
  - 当前 external / 暂不碰：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - 导航 runtime 与 mixed hot-file 现场
- 当前恢复点：
  - `P3` 现已完整收口
  - `P4` 现已具备“模型 + 内容分流 + 持久化 + 验收入口”
  - 下一步如继续推进，优先进入：
    - `T-P5-01`
  - 但若后续需要回到 scene / live 热窗，仍必须重新做准入复核，不能把 `main + neutral` 误当成 `Primary.unity` 可直接写。

## 2026-03-27｜0.0.2 清盘002 已拿到 `002` 首轮双向闲聊 live 证据

- 当前主线目标：
  - 从 `0.0.1` 的“单向短反馈”继续进入 `0.0.2清盘002`，把 `002 / 003` 的按 `E` 发起 NPC 非正式聊天闭环做实。
- 本轮子任务：
  - 先排掉 Unity 当前 `Play + Pause` 假阻塞。
  - 在不停止 Unity、不碰 `Primary.unity / GameInputManager.cs` 的前提下，补最小 Editor 验证入口并直接拿 `002` 的 live 证据。
- 本轮完成：
  - 已确认并解除一次：
    - `is_playing = true / is_paused = true / is_changing = true`
    的 Unity 混合态。
  - 已新增并持续扩展：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - 该验证菜单当前已具备：
    - 直接触发 `002 / 003` 的非正式聊天
    - 将玩家移出当前闲聊范围，触发跑开中断
    - 触发前清掉导航 live validation 的 pending key 与 runner 干扰
    - 闭环 trace 的第一版自动追踪骨架
- 本轮关键证据：
  - Console 已打印：
    - `[NPCValidation] 已触发 002 的非正式聊天，source=002, boundaryDistance=0.400`
  - Unity MCP 回读到：
    - `PlayerNpcChatSessionService.HasActiveConversation = true`
    - `PlayerThoughtBubblePresenter.IsVisible = true`
    - `PlayerThoughtBubblePresenter.CurrentBubbleText = "你好，我能在这儿和你说两句吗？"`
    - `002 -> NPCBubblePresenter.IsBubbleVisible = true`
    - `002 -> NPCBubblePresenter.CurrentBubbleText = "可以呀，这边正好不吵，你慢慢说。"`
  - 这说明当前至少已明确成立：
    - 玩家先说
    - NPC 延迟回复
    - 双气泡同场可见
    - `002` 已能进入 NPC 非正式聊天会话
- 仍未拿稳的点：
  - 第二轮推进与跑开中断的 live 证据还没稳定拿住。
  - 当前内容资产已复核：
    - `Assets/111_Data/NPC/NPC_002_VillageDaughterDialogueContent.asset`
    - `Assets/111_Data/NPC/NPC_003_ResearchDialogueContent.asset`
    每档关系都保有 2 条 `exchanges`，因此未拿稳更像是 live 现场和验证链稳定性问题，不是内容结构只剩 1 轮。
- 本轮验证：
  - `validate_script` 已通过：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `git diff --check` 已通过：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - 项目 compile 当前没有留下本轮 own 红错。
- 当前恢复点：
  - 后续优先继续压：
    - `002` 第二轮推进的稳定取证
    - 跑开中断是否确实进入玩家退出句 / NPC 反应句
  - 如果 `002` 仍不稳，再平移到 `003` 做同路径复核，判断是共性问题还是 `002` 单点问题。

## 2026-03-27｜0.0.2 清盘002 第二刀：trace 改成真实状态驱动，002 两轮已实证，003 与中断仍受 live 干扰

- 当前主线目标：
  - 把 NPC 非正式聊天从“首轮能亮相”继续推进到“至少两轮 + 最小中断入口”的真实闭环。
- 本轮完成：
  - 已继续只改代码与 Editor 验证层：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `PlayerNpcChatSessionService` 现已暴露验证所需的最小真实状态：
    - 会话状态名
    - 已完成 exchange 数
    - 最近一轮玩家句 / NPC 句
    - 最近一次中断句
    - 会话结束原因
  - `NPCInformalChatInteractable` 已补 `TryHandleInteract()`，验证入口不再把失败路由记成“已触发”
  - `NPCInformalChatValidationMenu` 已补：
    - `002 / 003` 的 closure trace
    - `002 / 003` 的 interrupt trace
    - 基于真实状态推进第二轮
    - 清导航 live runner 与 active dialogue 干扰的最小兜底
    - 明确失败日志，避免假阳性
- 本轮硬证据：
  - `002` 现已拿到完整两轮 live 日志：
    - 首轮玩家句 / NPC 句
    - 第二轮玩家句 / NPC 句
  - 这说明“玩家先说 -> NPC 回复 -> 再按一次推进到第二轮”的主链已经成立。
- 本轮验证：
  - `validate_script` 已通过上述 3 份脚本
  - `git diff --check` 已通过上述 3 份脚本
  - 当前没有留下本轮 own compile 红错
- 本轮 blocker：
  - Play 现场持续被外部 live 验证抢占：
    - `SpringDay1LiveValidation`
    - `DialogueDebugMenu`
    - `[NavValidation]`
  - 它们会在同一轮 Play 内自动拉起 `001` 正式对白 / 工作台回忆 / 导航实跑，直接打断 `002 / 003` 的闲聊 trace
  - 因此当前还不能 claim：
    - `002` 中断 trace 已实证
    - `003` closure / interrupt 已实证
- 当前恢复点：
  - 这条线的代码面已经继续向前，不需要再回退到“只写文档解释问题”
  - 下一步只要拿到安静的 Play 窗口，就优先补 `002 interrupt` 与 `003 closure / interrupt` 三条 live 证据。

## 2026-03-27｜0.0.2 清盘002 第三刀：剩余三条 live 证据补齐，NPC 非正式聊天首个闭环正式落地

- 当前主线目标：
  - 继续把 NPC 的 `0.0.2清盘002` 从“实现已前进、验证仍欠账”推进到“完整首个闭环已真实跑通”。
- 本轮完成：
  - 已在真实被外部导航 live 抢跑的 Play 窗口里，先后补齐：
    - `002 interrupt`
    - `003 closure`
    - `003 interrupt`
  - trace 过程中已明确看到：
    - `NavValidation runner_disabled`
    - `NavValidation runner_destroyed`
    说明当前 NPC trace 至少已经能在自己的窗口里把导航 live runner 让开。
  - 已拿到 `002` 中断证据：
    - 玩家退出句
    - NPC 反应句
    - `endReason=WalkAwayInterrupt`
  - 已拿到 `003` 两轮证据：
    - 首轮玩家句 / NPC 句
    - 第二轮玩家句 / NPC 句
  - 已拿到 `003` 中断证据：
    - 玩家退出句
    - NPC 反应句
    - `endReason=WalkAwayInterrupt`
- 本轮验证：
  - Unity 已退回 Edit Mode。
  - `validate_script` 已通过：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInteractable.cs`
    - `NPCInformalChatValidationMenu.cs`
    - `DialogueDebugMenu.cs`
    - `NavigationLiveValidationMenu.cs`
  - 仅剩 2 条非阻断 warning：
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInteractable.cs`
    - `String concatenation in Update() can cause garbage collection issues`
  - `git diff --check` 已通过当前 own 范围。
- 当前结论：
  - `0.0.2清盘002` 的业务目标当前已可判 `done`：
    - 玩家先说
    - NPC 延迟回复
    - 至少两轮推进
    - 中途跑开中断
    - `002 / 003` 两名 NPC 都已拿到 live 证据
- 当前边界：
  - 本轮仍未碰：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `NPCAutoRoamController.cs`
    - `DialogueChinese*`
- 当前恢复点：
  - NPC 当前不再卡在“验证未闭环”。
  - 已尝试白名单收口，但 `sunset-git-safe-sync.ps1` 明确阻断：
    - `Assets/YYY_Scripts/Service/Player`
    - `Assets/YYY_Scripts/Story/Interaction`
    - `.kiro/specs/NPC/2.0.0进一步落地`
    同根仍有更早的 NPCV2 tail 未一并纳入
  - 因此下一步优先不是继续补业务，而是先把 NPCV2 自己的 same-root dirty / untracked 收干净，再做正式 sync。

## 2026-03-27｜0.0.2 继续深化：用户原始 prompt 中的“自动续聊矩阵 + 双气泡分型 + 001 提示卡首轮美化”已实装

- 当前完成：
  - 已继续修改：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
  - 当前 NPC 线已进一步改成：
    - `002 / 003` 首轮后自动进入第二轮，不再依赖第二次 `E`
    - 玩家 / NPC 双气泡在会话期间会自动错位
    - 玩家气泡与 NPC 气泡已做出明显不同的样式方向
    - 跑开时的 `reactionCue` 走紧凑情绪 cue 入口，不再完全等同正常对白
    - `001` 正式对话与 `002 / 003` 非正式聊天共用的 `NpcWorldHintBubble` 已完成首轮重做
- 当前测试结果：
  - `validate_script`
    - 上述 5 份脚本已测通过
  - `git diff --check`
    - 已测通过
  - `live`
    - `002 interrupt` 再次拿到：
      - `已进入自动续聊观察`
      - `已在第二轮等待回复阶段直接触发跑开中断`
    - `003 interrupt` 同样拿到：
      - `已进入自动续聊观察`
      - `已在第二轮等待回复阶段直接触发跑开中断`
- 当前仍未放行：
  - `001` 提示卡审美
  - `002 / 003` 双气泡的画面级防重叠与观感
  - 玩家手按 `E` 的真实手感
  - 跑开情绪 cue 目前仍只是首版，不是完整表情系统
- 下一刀：
  - 如果用户先验收，就优先等用户用画面复测：
    - `001` 提示卡是否满意
    - `002 / 003` 双气泡是否还重叠、是否足够区分
  - 如果用户继续让我深化，就先往：
    - 跑开情绪 cue 扩展
    - 真实手按 `E` 的体验细修
    - same-root 尾账清理 / 白名单收口
    推进

## 2026-03-27｜“玩家离开矩阵”已从聊天分析升级成正式文档标准

- 当前完成：
  - 已把 NPC 非正式聊天的“玩家离开”问题整理成正式矩阵文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-03-27-NPC-非正式聊天完整交互矩阵与查漏补缺方案-01.md`
  - 当前文档不只写理想需求，也明确了：
    - 当前真实代码只存在 `WalkAwayInterrupt` 与 `Cancelled` 两大离开结果
    - 当前最大问题是 cause / phase 没正交
    - `PlayerTyping -> 跑开` 卡字 bug 已被正式列为 P0
- 当前恢复点：
  - 后续再做 NPC 非正式聊天，不应再脱离这份矩阵文档单点修补

## 2026-03-28｜`0.0.2` 已把离开矩阵 P0 压到代码与 live 证据层

- 当前完成：
  - `PlayerNpcChatSessionService.cs`
    - 已落地离开宽限
    - 已拆取消原因
    - 已加入 `LeavePhase` 快照
    - 已修 `PlayerTyping -> 跑开` 的原子接管
  - `NPCInformalChatValidationMenu.cs`
    - 已新增 `002 / 003 PlayerTyping Interrupt` trace
    - 已把验证日志补成 `endReason / abortCause / leavePhase`
    - 已修正闭环 trace 的误导性收尾日志
- 当前 live 结果：
  - `002 PlayerTyping Interrupt`
    - `leavePhase=PlayerSpeaking`
  - `003 PlayerTyping Interrupt`
    - `leavePhase=PlayerSpeaking`
  - `003` 二轮等待中断
    - `leavePhase=NpcThinking`
  - `002` 闭环收尾
    - `endReason=Completed`
- 当前判断：
  - 这条线不再只是“有设计文档”，而是 P0 已经真正写进服务层并被 live 复核。
  - 但这还不是整套矩阵 fully done，后面仍有：
    - `LeaveCause` 数据驱动化
    - `SystemTakeover / TargetInvalid` 专门 trace
    - 视觉与手感终验
- 当前恢复点：
  - 若继续编码，下一刀应从 P1 进入，不再回头重修 P0。

## 2026-03-28｜P1 已开始进场：非正式聊天中断续接与 fallback 矩阵底座已落地

- 当前主线目标：
  - 继续推进 `0.0.2清盘002`，把 NPC 非正式聊天从“能中断”推进到“中断后有记忆、有续接底座、有空资产兜底”。
- 本轮完成：
  - `PlayerNpcChatSessionService.cs`
    - 已补可续接中断快照与续接窗口
    - 已补同 NPC 重新发起时的 bundle / exchange 续接
    - 已补 `DistanceGraceExceeded / BlockingUi / DialogueTakeover` 的 phase-aware fallback reaction
  - `NPCInformalChatInterruptMatrixTests.cs`
    - 已补续接命中、过期失效、跨 NPC 不串线、`NpcSpeaking` fallback 的 Editor 测试
- 本轮验证：
  - 脚本级 `validate_script` 通过
  - `git diff --check` 通过
  - Console 当前未见本轮 own 新红；仍被外部 `SpringDay1WorldHintBubble.HideIfExists` blocker 占住
- 当前恢复点：
  - `0.0.2` 已正式进入 P1，不再只是 P0 修补
  - 下一刀仍应留在 NPC 非正式聊天底层，不应漂移去 `Primary.unity` 或 spring-day1 业务施工

## 2026-03-28｜P1 再推进：resume 规则已接入数据层，后续 continuity 不再只能写死在服务层

- 当前主线目标：
  - 继续深化 NPC 非正式聊天的 continuity，让“回来后怎么接话”具备数据驱动能力与独立验证入口。
- 本轮完成：
  - `NPCDialogueContentProfile`
    - 已新增 resume intro / resume rule 数据结构
    - 已新增 `GetResumeIntro(...)`
  - `NPCRoamProfile / NPCInformalChatInteractable`
    - 已把 resume rule 暴露到运行时交互链
  - `PlayerNpcChatSessionService`
    - 已先查配置化 resume rule，再 fallback 到通用补口
  - `NPCInformalChatValidationMenu`
    - 已新增 `BlockingUi / DialogueTakeover` 的续聊 trace 入口
  - `NPCInformalChatInterruptMatrixTests`
    - 已补 resume rule 命中与暴露链测试
- 本轮验证：
  - 6 个相关脚本 `validate_script` 通过
  - `git diff --check` 通过
  - 当前 project-level 红错仍是 spring-day1 外部 blocker，不是 NPC own 新红
- 当前恢复点：
  - NPC 非正式聊天的 continuity 已从“服务层记住断点”推进到“数据层可配置”
  - 仍可继续做纯底层，不必现在就停给用户测

## 2026-03-28｜编译错已清，P1 继续推进到 `TargetInvalid` 与续聊冷却

- 当前主线目标：
  - 修掉本轮 own 编译错误后，不停手，继续补 NPC 非正式聊天的纯底层稳固项。
- 本轮完成：
  - `NPCInformalChatValidationMenu`
    - 已清掉 `StartValidationTrace(...)` 漏参导致的 `CS7036`
    - 已新增 `TargetInvalid` trace 入口
  - `PlayerNpcChatSessionService`
    - 已补 `TargetInvalid / PlayerUnavailable / ServiceDisabled` 的 fallback 收束
    - 已补续聊补口 cooldown
  - `NPCInformalChatInterruptMatrixTests`
    - 已补 `TargetInvalid` fallback 与 cooldown 测试
- 本轮验证：
  - 相关脚本 `validate_script` 通过
  - `git diff --check` 通过
  - 编译刷新后，这轮 own `CS7036` 已从 Console 消失
- 当前恢复点：
  - 还没到“只剩测试”的阶段
  - 后续仍可继续做小块纯底层优化，再到必须 live 的节点

## 2026-03-28｜P1 结果语义补齐：resume outcome 已接入服务层与验证链

- 当前主线目标：
  - 把 NPC 非正式聊天 continuity 从“能续、能补口”推进到“系统自己知道这次续接结果是什么”。
- 本轮完成：
  - `PlayerNpcChatSessionService`
    - 已新增 `ConversationResumeOutcome`
    - 已记录 different NPC / expired / invalid snapshot / suppressed cooldown 等结果
  - `NPCInformalChatValidationMenu`
    - 续聊 trace 日志已带 `resumeOutcome`
  - `NPCInformalChatInterruptMatrixTests`
    - 已补 outcome 相关 Editor 测试
- 本轮验证：
  - 脚本级 `validate_script` 通过
  - `git diff --check` 通过
  - 当前 Console 无本轮 own 编译红；新可见的是外部 missing script / font / occlusion 警告
- 当前恢复点：
  - NPC continuity 现在不只“能跑”，也开始“能解释”
  - 仍可继续纯底层，不必现在就停给用户测

## 2026-03-28｜live 复测新增：`BlockingUi Resume` 已从代码面走到实跑证据

- 当前主线目标：
  - 在用户放开的 Unity 窗口里，把这轮新增的 continuity / resume 能力至少拿到一条新的 live 硬证据。
- 本轮结果：
  - `002 BlockingUi Resume` 已拿到完整 live：
    - 强制中断
    - pending resume snapshot
    - resume intro
    - 第二轮继续完成
    - `resumeOutcome=ResumedWithIntro`
  - `002 TargetInvalid Abort` 当前仍被导航自动实跑抢占，暂列外部 live blocker
- 当前恢复点：
  - NPC 这条线现在既有脚本级证据，也开始补齐新的 live 证据
  - 尚未 fully 进入“只剩测试”，但已明显逼近

## 2026-03-28｜补记 live 断点：`002 DialogueTakeover Resume` 在首句 `PlayerTyping` 卡住一次，当前应从 fresh Play 重新复核

- 当前主线目标：
  - 继续推进 NPC 非正式聊天 continuity 的 live 取证，不让上一轮半截现场变成记忆黑洞。
- 本轮补记：
  - `002 DialogueTakeover Resume` 曾在一次 fresh Play 里超时，停在：
    - `state=PlayerTyping`
    - `playerText="我"`
    - `npcText=""`
  - 这条现象更像“首句玩家打字卡首字”，还不能直接归因成 resume 分支后半段出错。
  - 紧接着尝试跑 `002 closure` 做对照时，验证菜单没再拿到稳定 Play，会话不宜继续外推。
  - 随后已确认 Unity 当时已退出 Play，并清过 Console。
- 当前恢复点：
  - 下一次 live 应从：
    - fresh Play
    - 先看是否有 `[NavValidation]`
    - 再跑 `002 closure`
    - 再跑 `002 DialogueTakeover Resume`
    的顺序重新取证。

## 2026-03-28｜fresh Play 复核后：NPC 非正式聊天的 `resume / target-invalid` live 组已补齐

- 当前主线目标：
  - 继续把 NPC 非正式聊天从“逻辑面大体成立”推进到“关键 live 证据闭环齐全”。
- 本轮新增完成：
  - `002 closure` 再次实跑通过，确认：
    - `首轮完成`
    - `第二轮完成`
    - `闭环收尾完成`
    - `endReason=Completed`
  - `002 / 003 DialogueTakeover Resume` 均已实跑通过。
  - `003 BlockingUi Resume` 已实跑通过。
  - `002 / 003 TargetInvalid Abort` 均已实跑通过。
  - `NPCInformalChatValidationMenu` 现已修正续聊 trace 的过早成功日志，`endReason` 会等到真正收尾后再落。
- 当前判断：
  - 此前 `002 DialogueTakeover Resume` 的 `PlayerTyping / "我"` 卡首字，应判为不稳定现场，而不是当前主链逻辑坏掉。
  - NPC validation 这轮还看到了：
    - `[NavValidation] pending_action_suppressed_by_npc_validation`
    - `[NavValidation] dispatch_suppressed_by_npc_validation source=entered_play_mode`
    说明当前这套 trace 对导航抢跑已有抑制。
- 当前恢复点：
  - NPC 非正式聊天这条线的底层 live 证据，现在已经逼近“只剩用户体验终验”。
  - 后续更像：
    - 内容继续做厚
    - 视觉与手感终验

## 2026-03-29｜`NPCV2` 全局警匪定责清扫第一轮自查结论已固定

- 当前主线目标：
  - 这轮不是继续推进 NPC 功能，而是把 `NPCV2` 当前真正 still-own 的实现面、历史碰过但不该继续吞的面、以及与 Day1 / 旧 NPC / scene 事故面的边界重新认死。
- 本轮完成：
  - 已完整回读 `NPCV2 / NPC` 线程记忆、`0.0.2清盘002` 材料与 `spring-day1V2` 最新回执
  - 已正式认定：
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs` = `NPCV2 current own`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` = 旧 `NPC` 历史 own 延续到 `NPCV2` 的 active bubble face
    - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs` = `mixed`
    - `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs` = `foreign`
  - 已确认旧 `NPC` 线程不再保留 active repo-side owner 身份，只保留历史责任证据
  - 已确认当前整张 `Assets/000_Scenes/Primary.unity` 不再挂 `NPCV2`；`NPCV2` 只认历史 `HomeAnchor / Inspector auto-repair` residue
  - 已确认按最终 own file set 计算，当前 own 路径仍为 `no`
- 当前恢复点：
  - 若后续进入第二轮 cleanup，`NPCV2` 只应继续清：
    - NPC 非正式聊天
    - NPC 气泡 / 提示壳
    - NPC 关系 / 内容 / 近身反馈
  - 不应再把整张 `Primary.unity`、`DialogueManager.cs` 或整份 mixed `NPCDialogueInteractable.cs` 继续拨给 `NPCV2`

## 2026-03-29｜`NPCV2` 第二轮 cleanup execution 已落地

- 当前主线目标：
  - 这轮不是继续推进功能，而是按第二轮执行书，把 `NPCV2` 的 still-own 面收成一个不再乱吞 Day1 / scene / mixed 的 cleanup 包。
- 本轮完成：
  - 已把 second-round still-own cleanup set 固定为：
    - NPC 非正式聊天链
    - `NpcWorldHintBubble.cs`
    - `NPCBubblePresenter.cs`
    - NPC 关系 / 内容 / 近身反馈
    - `NPCV2` 自己的线程 / 工作区 docs 与 memory
  - 已明确没有再吞：
    - `DialogueManager.cs`
    - 整份 `NPCDialogueInteractable.cs`
    - 整张 `Primary.unity`
  - 已把 `NPCDialogueInteractable.cs` 只压成 `NpcWorldHintBubble` 相关 exact residue 报实，不整份 claim
  - 已确认按 second-round allowed scope 计算，当前 own 路径仍为 `no`
- 当前恢复点：
  - `NPCV2` 现在离真正 clean 还差一步，但差的已经不是 owner 边界不清，而是 still-own 自己这组 `M / ??` 尚未做成白名单 cleanup 包

## 2026-03-29｜`NPCV2` 第三轮归仓尝试结果：preflight 已挡在 same-root blocker

- 当前主线目标：
  - 这轮不是继续讲边界，而是把 second-round still-own 包真实拿去跑 `preflight -> sync`。
- 本轮完成：
  - 已真实运行 third-round still-own package 的 `preflight`
  - `preflight` 结果为 `False`
  - 当前未进入 `sync`
  - 第一真实 blocker 已固定为：
    - `same-root remaining dirty/untracked`
    - first preview path = `Assets/Editor/Story/DialogueDebugMenu.cs`
  - 当前 own 路径结论仍为：
    - `no`
- 当前恢复点：
  - 这轮已经从“分析能不能上 git”推进到“真实 preflight 已跑且 blocker 已钉死”
  - 后续若继续，应先清 still-own 白名单所属同根残留，再重新跑 `preflight`

## 2026-03-29｜`NPCV2` 第三轮复跑：`main@6aaf4e93` 仍未放行 `sync`

- 当前主线目标：
  - 用户要求不要再扩 mixed / foreign 说明，只对 second-round still-own 包继续做真实 `preflight -> sync`。
- 本轮完成：
  - 已重新回读第三轮执行书，并在当前现场 `main @ 6aaf4e93` 再次真实运行 still-own package 的 `preflight`
  - `preflight = False`
  - 本轮未进入 `sync`
  - 第一真实 blocker 仍固定为：
    - `same-root remaining dirty/untracked`
    - `first exact path = Assets/Editor/Story/DialogueDebugMenu.cs`
    - `exact reason = 位于本轮白名单所属 own root Assets/Editor 下，但未纳入 IncludePaths`
  - 脚本当前给出的 own-root 关键信号：
    - `own roots remaining dirty 数量 = 27`
  - 当前 own 路径结论仍为：
    - `no`
- 当前恢复点：
  - `NPCV2` 这轮依旧停在 `B｜第一真实阻断已钉死`
  - 若后续还要推进，不是继续解释边界，而是先把 still-own 所属同根残留收干净，再重跑 `preflight`

## 2026-03-29｜`NPCV2` 第四轮：`Assets/111_Data/NPC + own docs/thread` 已先独立上 git

- 当前主线目标：
  - 不再继续拿 mixed-root 大根撞 preflight，而是只把 `Assets/111_Data/NPC + own docs/thread` 这组可自归仓子根先独立收口。
- 本轮完成：
  - 已完整回读第四轮执行书
  - 已在 `main @ 6aaf4e93` 真实运行这组子根白名单 `preflight`
  - `preflight = True`
  - 脚本关键信号：
    - `own roots = Assets/111_Data/NPC, .kiro/specs/NPC/2.0.0进一步落地, .codex/threads/Sunset/NPCV2`
    - `own roots remaining dirty 数量 = 0`
  - 已继续真实运行 `sync`
  - 当前这组可自归仓子根首次上 git 提交 SHA：
    - `70fdd44f`
  - 当前 own 路径结论：
    - `yes`
- 当前恢复点：
  - `NPCV2` 已从“第三轮 same-root blocker”推进到“第四轮可自归仓子根先独立收口完成”
  - 后续若继续，应把 mixed-root 大根作为独立 cleanup 处理，不再重新污染这组已可归仓子根

## 2026-03-29｜`NPCV2` 线程停表：mixed-root backlog 转治理位

- 当前主线目标：
  - 把 `NPCV2` 在“全局警匪定责清扫”里的收口状态正式定格，避免后续继续沿旧 prompt 自转。
- 本轮完成：
  - 用户已明确裁定：
    - `NPCV2` 当前停表
    - 不再继续发新 prompt
    - 第四轮已完成 `Assets/111_Data/NPC + own docs/thread` 真实上 git
    - `NPCV2 current own path = yes`
    - 剩余 mixed-root backlog 由治理位接盘，不再继续分给 `NPCV2`
- 当前恢复点：
  - `NPCV2` 当前状态应视为：
    - `clean-subroot 已收口`
    - `mixed-root backlog 已外转治理位`
    - `线程停表，等待明确新裁定后再唤醒`

## 2026-04-01｜NPC 开发认知台账重新校准：底层完成度不等于体验闭环完成

- 当前主线目标：
  - 用户明确不要再听治理清扫叙事，而是要我重新钉死 NPC 线真正的开发完成度、未完成项和下一步顺序。
- 本轮完成：
  - 已重新回读并交叉核对：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-03-27-NPC-非正式聊天完整交互矩阵与查漏补缺方案-01.md`
  - 已再按当前真实代码复核：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
  - 当前重新钉死的开发事实：
    1. 底层和中层已做了不少：
       - NPC 生成器 / prefab / profile / 漫游基础已成
       - `HomeAnchor` 最小补口链做过
       - 非正式聊天会话服务已经具备：
         - 玩家先说
         - NPC 延迟回复
         - 自动续下一轮
         - 距离越界中断
         - resume snapshot
       - 玩家 / NPC 双气泡底座和关系成长最小底座已经存在
    2. 但这些不能等同于“玩家体验闭环完成”：
       - `Primary.unity` 的 `001 / 002 / 003` 最小可玩 scene 仍没有收成真正可玩闭环
       - 非正式聊天体验层明确未过线
       - `P5` 受击 / 工具命中 / 反应系统仍基本未做
       - 联调与最终终验未完成
  - 当前必须按“明确未通过”处理的问题：
    - 玩家聊到一半跑开时，玩家气泡会鬼畜式重复打字机
    - 玩家气泡可能停在半透明残留态
    - NPC 会停在等待态或保留对话框，收尾不干净
    - 双气泡当前只做了左右偏移，不足以解决多 NPC / 遮挡 / 归属层级问题
    - 玩家气泡与 NPC 气泡当前视觉差异不足，未达到正式体验要求
    - NPC 交互提示当前还是世界空间提示壳，显眼度和正式感都不够
- 当前恢复点：
  - 后续再汇报 NPC 进度时，必须分开讲：
    - “底层能力做到了哪里”
    - “玩家真正能验收到的体验做到哪里”
  - 不能再把底层已接线、日志跑通过、或清扫已收口，表述成“NPC 聊天已经基本做完”

## 2026-04-01｜NPC 体验补口继续推进：提示壳统一到左下角，闲聊中断链不再复用鬼畜打字机

- 当前主线目标：
  - 继续收紧 NPC 非正式聊天体验层，而不是扩写治理、owner 或 cleanup 叙事。
- 本轮完成：
  - 提示壳继续统一：
    - 头顶提示进一步收窄成更小的倒三角
    - 左下角统一提示区继续作为主提示壳
    - 非正式聊天进行中，头顶箭头已不再显示，只保留左下角提示
  - 中断链继续收紧：
    - 玩家跑开时，旧会话气泡会先被强清
    - 离场句 / NPC 反应改为直接显示，不再复用容易鬼畜的 typed interrupt 链
    - `E` 在 `AutoContinuing / Interrupting / Completing` 这些阶段已能正常跳过等待或收尾
  - 气泡表现继续收紧：
    - 玩家 / NPC 打字机开始时不再做透明渐显
    - 正常聊天结束时，双方气泡都改回正常隐藏，不再只有玩家一侧硬切
- 本轮验证：
  - Unity `Editor.log` 已拿到 fresh compile 绿证：
    - `ExitCode: 0`
    - `*** Tundra build success (4.34 seconds)`
  - `git diff --check` 通过
  - 仍未完成：
    - 运行态 live 复测
    - 用户肉眼体验终验
  - 当前主要 blocker：
    - MCP / `127.0.0.1:8888` 仍不可用
- 当前恢复点：
  - NPC 线当前已从“只补会话状态机”继续推进到“提示壳和中断链也开始按真实体验收紧”
  - 下一刀优先级不变：
    - 直接做 `002 / 003` 的运行态复测和体验验收

## 2026-04-01｜NPC 运行态自测补记：`002 / 003` 四条闭环 trace 已连续过线

- 当前主线目标：
  - 把 NPC 非正式聊天从“结构上接好了”推进到“Unity 自测链真能过”。
- 本轮完成：
  - `NPCInformalChatValidationMenu.cs`
    - 补 `runInBackground` 兜底，validation trace 不再因为失焦卡死在首字。
  - `PlayerNpcChatSessionService.cs`
    - 会话进行中改为直接压掉头顶 world hint，并主动刷新左下角提示。
  - `InteractionHintOverlay.cs`
    - 增加只读状态暴露，便于后续继续做 UI 运行态取证。
  - `SpringDay1InteractionPromptRuntimeTests.cs`
    - 改成纯反射取 Sunset runtime 类型，顺手清掉这轮把 Play Mode 堵死的 Editor compile blocker。
- 本轮验证：
  - `002 closure` / `002 interrupt` / `003 closure` / `003 interrupt`
    - 全部拿到成功日志
    - closure = `endReason=Completed`
    - interrupt = `endReason=WalkAwayInterrupt + playerExitSeen=True + npcReactionSeen=True`
  - `validate_script` 通过
  - `git diff --check` 通过（仅剩 CRLF warning）
- 当前恢复点：
  - NPC 非正式聊天当前已进入“逻辑自测全绿、等待人工体验终验”的阶段
  - 下一刀不该再先补状态机，而应优先做：
    - 人工看提示壳和气泡的最终体验验收

## 2026-04-01｜owner 边界重裁：NPC 当前应从“共享提示壳”退回四件套 own

- 当前主线目标：
  - 在 `UI` 打断后，重新校准 NPC 这条线对“非正式聊天体验 blocker”的真实 owner 边界。
- 本轮只读结论：
  - `NPC` 当前最小 own 生产面应收窄为：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 不应继续由 NPC 单线主拿的 shared 提示壳：
    - `SpringDay1ProximityInteractionService.cs`
    - `SpringDay1WorldHintBubble.cs`
    - `InteractionHintOverlay.cs`
    - `InteractionHintDisplaySettings.cs`
    - `SpringDay1DialogueProgressionTests.cs`
    - `SpringDay1InteractionPromptRuntimeTests.cs`
  - 关键原因：
    - 统一近身仲裁、头顶箭头、左下角提示卡与设置开关都已经跨出 NPC 闲聊，变成 Story / Day1 / UI 共用壳。
    - 因此 NPC 不能再把“左下角提示与头顶箭头最终观感”整包当作自己单线必收项。
- 当前恢复点：
  - 当前 live 状态最准确仍是 `PARKED`
  - 若后续恢复 NPC 真实施工，应先重新 `Begin-Slice`，并把 slice 严格收窄到四件套 own

## 2026-04-01｜用户最新裁定后继续 PARKED：NPC 退回底座协作位

- 当前主线目标：
  - 按用户最新裁定，不继续主刀 shared prompt shell，而是让 `UI` 接后半段玩家面 UI/UE 整合。
- 本轮只读结论：
  - `NPC` 当前继续 `PARKED`
  - 只保留 NPC 底座守门面：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - shared prompt shell 与玩家面统一提示/唯一 E/视觉归属一致，转由 `UI` 主刀
  - 只有 `UI` 越界吞到 NPC 会话状态机、自动续聊/中断行为或双气泡体验整包时，`NPC` 才重新报 blocker
- 当前恢复点：
  - `NPC` 不主动恢复这刀实现
  - 继续等待 `UI` 接手后半段玩家面整合

## 2026-04-01｜state 与卫生已收正：NPC live 边界正式退到 3 件底座文件

- 当前主线目标：
  - 在不恢复 shared prompt shell 实现的前提下，把 `NPC` 线程当前 live state 收回最新 exact-own。
- 本轮完成：
  - 已先执行 `Park-Slice`，继续保持合法 `PARKED`
  - 已把 `NPC` 当前 active state 收窄为 3 件底座文件：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 已从当前 state 中释放：
    - `SpringDay1ProximityInteractionService.cs`
    - `SpringDay1WorldHintBubble.cs`
    - `InteractionHintOverlay.cs`
    - `InteractionHintDisplaySettings.cs`
    - shared prompt tests
    - 广义 Day1 提示壳
- 当前恢复点：
  - `NPC` 当前已经完成边界和卫生收正
  - 后续除非用户直接重新授权，否则不再默认回拿 shared prompt shell / 玩家面 UI 壳主刀

## 2026-04-01｜2.0.0 用户终验入口已建立：NPC 进入“底层已站住，等待体验回单”阶段

- 当前主线目标：
  - 用户当前要的不是继续施工，而是先把 `NPC` 这条线的真实开发进度、可测范围、待验范围和回单格式一次性说清楚。
- 本轮完成：
  - 已在 `0.0.2清盘002` 下落一份正式验收总包：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-01-NPC当前阶段用户验收总包-01.md`
  - 总包已明确给出：
    - 当前主线
    - 当前阶段
    - `NPC own` 可测矩阵
    - `shared/UI` 观察矩阵
    - 分步手测说明
    - 可直接回填的回执单
- 当前关键判断：
  - `2.0.0` 这条线现在最准确的说法是：
    - NPC 非正式聊天的底层闭环已经做到了“线程自测已过”
    - 但真实入口体验仍未被用户正式放行
  - 因此后续真正的判断基准，不该再是“我这边代码解释得通”，而应是：
    - 用户按矩阵回单后，到底哪些体验项仍未过线
- 当前恢复点：
  - 下一步应等待用户按验收包给回单。
  - 若后续恢复施工，应只对回单里仍属于 `NPC own` 的问题继续开刀，不再顺手吞 shared prompt shell。

## 2026-04-01｜验收包交付后线程已回到 PARKED，等待用户终验回单

- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前 blocker：
  - `等待用户按NPC验收总包回单；当前保持NPC底座协作位，不恢复shared prompt shell主刀`
- 当前恢复点：
  - `2.0.0` 这条线现在不再主动继续扩做。
  - 后续只有在用户回单指出 `NPC own` 失败项后，才重新开工。

## 2026-04-02｜用户带图补充体验反馈后：2.0.0 已新增 UI prompt 与 NPC 自省清单

- 当前主线目标：
  - 用户当前要求我先把最新体验反馈拆成：
    - 一份发给 `UI` 的明确 prompt
    - 一份 `NPC` 自己的复盘与下一轮施工清单
- 本轮完成：
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC本轮自省与下一轮施工清单-01.md`
- 当前新增被钉死的体验要求：
  - 气泡只做适度避让，不要飞得太远
  - 箭头必须锚定说话方头顶
  - 自动跳过时长改成：
    - `1.0s + 字数 * 0.08s`
  - NPC 气泡样式回到最初认可的基线
  - scene 层要开始处理 NPC 扎堆与互撞后的内容反应
  - 左下角任务优先提示外包给 `UI`
- 当前恢复点：
  - `2.0.0` 下一轮若恢复施工，最该优先做的已不再是“继续解释逻辑”，而是：
    - 气泡观感回调
    - 节奏公式化
    - scene 反扎堆

## 2026-04-02｜文档交付后线程已回到 PARKED，等待用户决定下一刀

- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前 blocker：
  - `等待用户决定：先转发UI prompt，还是直接让我按自省清单恢复NPC own施工`
- 当前恢复点：
  - `2.0.0` 这条线目前已把最新体验裁定和下一轮优先级讲清。
  - 后续不再自行延长 scope，等用户决定先走 UI 合同链还是 NPC own 实装链。

## 2026-04-02｜NPC 体验补口已继续落地：提示壳/气泡/scene 收窄完成，但 live 复测被外部编译红阻断

- 当前主线目标：
  - 把用户最新指出的 NPC 体验问题直接落地，而不是再停在文档层。
- 本轮完成：
  - 已继续真实施工：
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `PlayerNpcChatSessionService.cs`
    - `SpringDay1ProximityInteractionService.cs`
    - `Primary.unity`
    - `NPC_002 / NPC_003` 对话数据
  - 关键结果：
    - 头顶世界提示只在真正可交互时出现，并与左下角提示重新同拍。
    - 当前焦点 NPC 进入可交互态时，会压掉自己的 ambient bubble，避免头顶冲突。
    - 玩家 / NPC 跑开中断句改为直接实心接管，不再重新透明淡入。
    - 自动续聊停顿改成 `1.0s + 字数 * 0.08s`，不再把长句硬截到 `1.8s`。
    - 对话气泡避让进一步收窄。
    - `Primary` 里 `001 / 002 / 003` 当前编辑态站位已回到各自 `HomeAnchor`，不再读回成 `002 / 003` 扎堆。
    - `NPC_002 / NPC_003` 里的 `……` cue 已改成 ASCII，避免字体缺字 warning。
- 本轮验证：
  - `validate_script` 通过：
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `PlayerNpcChatSessionService.cs`
    - `SpringDay1ProximityInteractionService.cs`
  - `git diff --check` 通过本轮 own 范围。
  - MCP `8888` listener 中途掉过一次，已通过项目自带 `mcp-terminal.cmd` 拉回，基线脚本重新 `pass`。
- 当前 blocker：
  - 本轮没法继续做 `Play` 自测，不是 `NPC` own 代码红，而是外部 compile blocker：
    - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
    - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
    缺失 `TickStatusBarFade / ApplyStatusBarAlpha`
  - Unity 因此持续 `isCompiling=true`，当前 `002 / 003` validation trace 不能继续跑。
- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前恢复点：
  - 外部 compile blocker 清掉后，直接恢复 fresh Play 复测：
    - `002 / 003 closure`
    - `002 / 003 interrupt`
    - `001` 任务优先提示
    - 头顶箭头 / 左下角提示 / 气泡距离最终画面验收

## 2026-04-02｜NPC 跑开中断主 bug 已在 own 层压实，6 条 runtime trace 补齐

- 当前主线目标：
  - 把用户最新指出的“跑开后鬼畜 / 卡首字 / NPC 还继续等 / 正常对话自己往下滚”压到真正可自证的状态，而不是停在日志解释。
- 本轮真实完成：
  - 实际继续落地的代码只集中在：
    - `PlayerNpcChatSessionService.cs`
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenterStyleTests.cs`
  - 关键修正是：
    - 超距快进现在只结束当前打字句，不再推进完整正常对话链。
    - NPC 气泡样式回正到用户更早认可的暖金基线。
    - 玩家气泡样式护栏测试改成反射版，并收掉 `Object` 二义性红错。
- 本轮真实自验：
  - 已连续拿到：
    - `002 closure = Completed`
    - `003 closure = Completed`
    - `002 interrupt = WalkAwayInterrupt`
    - `003 interrupt = WalkAwayInterrupt`
    - `002 player-typing interrupt = PlayerSpeaking + WalkAwayInterrupt`
    - `003 player-typing interrupt = PlayerSpeaking + WalkAwayInterrupt`
- 当前阶段判断：
  - `2.0.0` 这条线在 `P3` 上最新应判为：
    - `结构 / checkpoint = 成立`
    - `targeted probe / 自测 = 成立`
    - `真实入口体验 = 仍待用户终验`
  - 不能再把“底层已经修通 + trace 已过”偷换成“玩家体验已经过线”。
- 当前边界：
  - 这轮没有回拿：
    - shared prompt shell
    - `Primary.unity`
    - 导航 runtime
    - 字体资产
    - `GameInputManager.cs`
- 当前恢复点：
  - `P3` 里最硬的运行态止血点已经站住。
  - 这轮补完记忆和 state 后，线程应回到 `PARKED`。
  - 下次继续时，只应对用户终验里仍属于 `NPC own` 的体验失败项开新 slice。

## 2026-04-02｜2.0.0 收尾补记：线程已重新 `PARKED`，但全局 skill 审计层仍有旧重号

- 本轮收尾动作：
  - `NPC` 已重新执行 `Park-Slice`
  - 当前 live 状态：
    - `PARKED`
- 审计层补记：
  - `skill-trigger-log.md` 已追加 `STL-20260402-036`
  - 健康检查仍报：
    - `Canonical-Duplicate-Groups = 1`
    - 旧重号：
      - `STL-20260402-029`
- 当前恢复点：
  - `2.0.0` 这条线自己的自验与记忆已经闭合。
  - 但如果后续要 claim 全局审计 clean，还需要治理层处理这条旧 skill-log 重号。

## 2026-04-03｜并行闭环补记：spring-day1 integration 侧已过收口闸门

- 并行关系当前稳定为：
  - `NPC-v` 负责 NPC 本体层
  - `spring-day1V2` 负责 Day1 integration / phase consumption / runtime summary
- integration 侧这轮已完成：
  - `SpringDay1NpcCrowdDirector` 改成优先消费 `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor`
  - `SpringDay1Director` 补上 crowd/world-hint/player-facing snapshot，并把对 slice 外底座的几处硬引用改成反射/兜底桥接
  - 当前 crowd manifest 的 8 个 entry 都能解析到稳定锚点
- integration 侧本轮验证：
  - `git diff --check` 通过
  - `Ready-To-Sync` 已通过
- 当前合并判断：
  - 新群像这条总线已经不再只有 `NPC-v` 本体层在动，`spring-day1` 侧的 phase/runtime integration 也已经进入可白名单 sync 状态
  - 后续总账应继续拆成三类：
    - `NPC-v` own 的内容/运行时问题
    - `spring-day1` own 的 Day1 integration 问题
    - shared root 的外部噪音

## 2026-04-03｜收盘补记：spring-day1 integration 已进主分支并停车

- `spring-day1` 这侧本轮白名单提交已完成：
  - `03c0bf87`
- 当前 live 状态：
  - `PARKED`
- 当前恢复点：
  - 新群像总线现在同时具备：
    - `NPC-v` 本体层基线
    - `spring-day1` integration 提交基线
  - 下一轮应基于这个合并基线继续判断剩余运行态问题，而不是回退到“只有资源生成”的旧口径。
