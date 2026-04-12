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

## 2026-04-03｜2.0.0 总线补记：`NPC-v` 的 preflight 已从人工检查升级为工具级全链护栏，旧三只 `HomeAnchor` 漂移已被定性为 scene blocker

- 当前主线目标：
  - 继续把“春一日新 NPC 群像”拆成：
    - `NPC-v` 本体层
    - `spring-day1` integration 层
    - legacy `Primary` 旧 NPC scene blocker
- 本轮新增稳定事实：
  - `NPC-v` 侧继续补强了：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
  - 当前这条验证链已经不只看 asset 在不在，而是会一起检查：
    - sprite
    - animator controller
    - 6 条 clip
    - prefab 核心组件挂载
    - prefab sprite / controller 引用一致性
    - `pairDialogueSets` 的 partner 合法性与 reciprocal 闭合
  - 当前 Unity 里再次执行：
    - `Tools/NPC/Spring Day1/Validate New Cast`
    已稳定 `PASS`
  - 当前最新通过信号仍为：
    - `npcCount=8`
    - `totalPairLinks=16`
- 本轮同时追加了一条只读边界判断：
  - 用户贴出的 `002 roam interrupted => StuckCancel` 当前不应直接算成 `NPC-v` 自己的 crowd pair/data 失败。
  - 只读核对 `Primary.unity` 后，legacy `001 / 002 / 003` 都仍绑定各自 `HomeAnchor`，但至少：
    - `002`
    - `003`
    的本体位置与 anchor 已严重分离；
    - `001` 也有轻度偏离。
  - 因此这一组 warning 更像：
    - `Primary / scene / Day1` 侧待善后的旧锚点漂移
    - 而不是当前 `NPC-v` 本体层新增回归
- 当前合并判断：
  - 新群像总线现在可以更清楚地拆成三份账：
    1. `NPC-v` own：
       - 新 8 人的 prefab / anim / content / roam / pair 数据与 preflight
    2. `spring-day1` own：
       - manifest phase 消费
       - runtime crowd 拉起
       - scene 接入
    3. legacy scene blocker：
       - 旧 `001 / 002 / 003` 的 `HomeAnchor` 漂移与 `StuckCancel`
- 当前恢复点：
  - 后续若继续推进 `NPC-v`，优先应做新 8 人 runtime targeted probe；
  - 不应再把旧三只 NPC 的 scene 位置善后混进 `NPC-v` own 完成定义里。

## 2026-04-03｜总线最新判断：距离用户原始目标还差两段 runtime 证据，不再差结构设计

- 当前主线目标：
  - 回答用户“现在距离最初需求还差多远、下一步到底是什么”，并把总线剩余项重新压成可直接执行的两刀。
- 本轮只读核定：
  - 用户原始目标已经收束为：
    - 在已实现的 `spring-day1` 剧情基础上，把新增 8 名 NPC 真正扩进 Day1，让玩家在对应阶段看见他们、能触发他们、并感受到“村长逃跑已发生”的群像对白。
  - 当前离这个目标不再主要差：
    - 角色设定
    - prefab / asset / manifest 结构
    - integration 代码骨架
  - 当前真正还差的是两段运行态证据：
    1. `NPC-v` 本体层 runtime targeted probe
    2. `spring-day1V2` 的 Day1 phase/runtime consumption probe
- 当前总线判断：
  - 结构完成度已经明显高于运行态完成度。
  - 当前最准确的阶段说法是：
    - “能生成、能接线、能预检”已经站住
    - “玩家真的在 Day1 里看到并消费这批人”还未闭环
- 当前恢复点：
  - 后续并行推进必须继续严格分工：
    - `NPC-v` 只拿新 8 人本体 probe 和 own 归仓
    - `spring-day1V2` 只拿 Day1 消费矩阵和 integration 归因
  - 不再让任何一侧重新吞整案。

## 2026-04-03｜Day1 integration 侧补了最小 crowd probe，但 probe 现场被 `NPC-v` compile red 截停

- 当前主线目标：
  - 继续沿“新 8 人群像”总线拿 Day1 `phase/runtime consumption` 证据。
- 本轮新增稳定事实：
  - `spring-day1V2` 这侧已经先补了 own 最小 probe：
    - `SpringDay1NpcCrowdDirector.CurrentRuntimeSummary`
    - 现在会带：
      - active 列表
      - anchor 名
      - fallback 标记
      - 当前位置
  - 这一步是为了后续不改 editor 工具，也能直接从现有 `BuildSnapshot` 判断：
    - 哪些 NPC 在
    - anchor/fallback 是否命中
    - 是否有明显漂移
- 当前 blocker：
  - 运行态矩阵本轮仍未拿到，因为 Unity 被 `NPC-v` own 文件
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    的 `CS1061` compile red 卡住
  - 当前现象：
    - `isCompiling = true`
    - `CodexEditorCommandBridge` 停在 `menu:Assets/Refresh`
- 当前恢复点：
  - 新群像总线仍然保持原来的两段剩余闭环判断不变：
    1. `NPC-v` 先清 own compile / runtime probe
    2. `spring-day1V2` 再继续 `CrashAndMeet -> DayEnd` 的消费矩阵

## 2026-04-03｜总线再更新：`NPC-v` compile red 已不是第一现场，pair 问题改判为本体层局部失败

- 当前主线目标：
  - 用 `NPC-v` 最新回执修正总线阶段判断，避免继续把旧 compile red 当成当前第一 blocker。
- 本轮最新判断：
  - `NPC-v` 当前已经能跑完新 8 人 runtime targeted probe，说明此前挡住 Unity 的 compile red 已不再是第一现场。
  - 总线现状更新为：
    - 单体 runtime 侧大部分成立
    - pair dialogue 仍未成立
    - Day1 phase/runtime consumption 仍未拿证
  - 这意味着当前两条线的剩余项分工更清楚了：
    1. `NPC-v`
       - 修 ambient pair bubble emission
       - 收 own roots 历史尾账
    2. `spring-day1V2`
       - 跑 `CrashAndMeet -> DayEnd` 的 crowd 消费矩阵
- 当前恢复点：
  - 不再把 `NPC-v` compile red` 继续写成当前总线主 blocker；
  - 后续如果继续汇报，应直接按“哪些已能验、哪些未闭环”来讲，而不是沿用旧阻塞口径。

## 2026-04-03｜`NPC-v` 本体层 runtime probe 收束：三段已过，pair 缩到 ambient bubble 未发射

- 当前主线目标：
  - 继续把“春一日新 NPC 群像”拆成：
    1. `NPC-v` 本体层 runtime targeted probe
    2. `spring-day1V2` 的 Day1 phase/runtime consumption probe
- 本轮新增稳定事实：
  - `NPC-v` 这侧已经真实跑完 targeted probe，不再停留在静态护栏：
    - `8/8 instance = PASS`
    - `8/8 informal chat = PASS`
    - `2/2 walk-away interrupt = PASS`
    - `2/2 pair dialogue = FAIL`
  - pair 失败已经被压窄到同一个小点：
    - `101 <-> 103`
    - `201 <-> 202`
    都能进入 ambient pair 的 `chatting/joined chat` 决策
    且 pair 台词数组在运行时解析正常
    但 `NPCBubblePresenter` 仍保持：
    - `visible = false`
    - `suppressed = false`
    - `conversationOwner = none`
- 当前总线判断：
  - 新 8 人不是“整体没活”；
  - 更准确的说法是：
    - 单体实例、单体闲聊、离场收尾已经站住
    - 成对 ambient 聊天的最终 bubble 播放链还没站住
- 现场附加风险：
  - Unity 本轮多次被 `打开场景已在外部被修改。` 模态框打断；
  - 这是 shared scene/live 环境噪声，不应误报成 `NPC-v` own pair 结论。
- 当前恢复点：
  - `NPC-v` 下一刀若继续，应直接检查：
    - `NPCAutoRoamController.TryStartAmbientChat / StartAmbientChatRoutine / PlayAmbientChatBubble`
    - 以及 `NPCBubblePresenter.ShowText`
    在 pair 场景下为何没有真正点亮 bubble
  - `spring-day1V2` 则继续保持只做 Day1 phase 消费矩阵，不回吞这条本体链。

## 2026-04-03｜收口补记：`NPC-v` 线程已合法 `PARKED`，当前 first blocker 是 own roots 历史残包

- 本轮新增事实：
  - `Ready-To-Sync.ps1 -ThreadName NPC`
    已真实执行并返回：
    - `BLOCKED`
  - 随后已执行：
    - `Park-Slice.ps1 -ThreadName NPC`
  - 当前 live 状态：
    - `PARKED`
- 第一真实 blocker：
  - 不是 runtime probe 菜单自身；
  - 而是 `NPC` 当前 own roots 仍带历史残包，导致不能只带本轮白名单 sync
  - 已明确报实至少包括：
    - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - 未归仓的 `0.0.2` prompt 文档
    - `Assets/Editor/NPC/CodexEditorCommandBridge.cs(.meta)`
- 当前恢复点：
  - `NPC-v` 后续如果继续，一条线要看 pair ambient bubble runtime；
  - 另一条线要先把 NPC 历史 own roots 清干净，否则仍过不了真正归仓。

## 2026-04-03｜总线改判：新 8 人的技术可运行性与“是否符合原剧本”必须分开记账

- 当前主线目标：
  - 把“春一日新 NPC 群像”从“技术上已经能跑”与“角色上已经对齐原案”两件事彻底拆开。
- 本轮新增稳定事实：
  - 已重新回读原剧本文档，确认 Day1 与长线原案里稳定存在的角色链是：
    - `马库斯 / 艾拉 / 卡尔(研究儿子) / 老杰克 / 老乔治 / 老汤姆 / 小米 / 围观村民`
  - 当前工程里已有老 Day1 主角色底座：
    - `NPC_001_VillageChief*`
    - `NPC_002_VillageDaughter*`
    - `NPC_003_Research*`
  - 当前 crowd 新 8 人：
    - `莱札 / 炎栎 / 阿澈 / 沈斧 / 白槿 / 桃羽 / 麦禾 / 朽钟`
    只能先算当前实现现场，不能继续直接 claim 为原剧本正式角色
- 当前总线判断更新为：
  - `NPC-v` 最新 probe 结果仍然有效，证明新 8 人本体链大体能跑
  - 但“本体能跑”不等于“角色设定正确”
  - 后续必须先补“原案角色 -> 当前 crowd 槽位”映射，再决定哪些保留、哪些降级为匿名群众、哪些要回正
- 本轮新增接续文件：
  - `2026-04-03-NPC-v_春一日新NPC群像原剧本人设核实与NPC本体映射回正prompt-04.md`
  - `2026-04-03-本线程_春一日原剧本角色消费矩阵与群像整合回正任务单-05.md`
- 当前恢复点：
  - `NPC-v` 下一刀先做原剧本人设核实与 NPC 本体映射回正
  - `spring-day1V2` 下一刀先做原 Day1 角色消费矩阵与整合误差判断

## 2026-04-03｜收口补记：原剧本核实双 prompt 已形成本地提交，线程已停车等待网络恢复或手动推送

- 本轮新增事实：
  - 原剧本核实与双 prompt 重写这批文档已通过：
    - `Ready-To-Sync`
  - 并已本地提交为：
    - `e4ef0ad4`
    - `2026.04.03_spring-day1V2_03`
- 当前未闭点：
  - `git push` 因代理/网络失败，没有把这批文档送到远端
- 当前恢复点：
  - 线程当前已经：
    - `PARKED`
  - 仓库相对 upstream：
    - `ahead 1`

## 2026-04-03｜补记：新 8 人 runtime probe 的测试位置与正式 Day1 消费链已拆清

- 当前主线目标：
  - 向用户报实“为什么在正常场景里没看到新 8 人”，并把后续分工重新收回到原剧本回正。
- 本轮新增稳定事实：
  - `NPC-v` 之前拿到的 runtime probe 通过项，主要来自：
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
  - 这条 probe 会在 Play Mode 下临时生成：
    - `__SpringDay1NpcCrowdRuntimeProbeRoot`
    - `Probe_101 ~ Probe_301`
  - 其测试坐标落在隔离区，约：
    - `120 / 120`
  - 因而它证明的是“本体链能跑”，不是“玩家已在正式 Day1 村景里看到这批人”。
- 同时已钉实：
  - 正式把 crowd 动态生成到 `Primary` 的是：
    - `SpringDay1NpcCrowdDirector`
  - 但这条正式消费链目前仍缺：
    - 原剧本人设映射回正
    - Day1 phase/runtime consumption 闭环
- 当前恢复点：
  - `NPC-v` 下一轮只做原案角色到 `101~301` 的映射与最小本体回正
  - `spring-day1V2` 下一轮只做原 Day1 角色消费矩阵与老角色 / 新 crowd 的承载分层
  - 当前 live 状态已再次纠偏为：
    - `PARKED`

## 2026-04-03｜补记：原 Day1 消费矩阵已钉实，当前主问题是“老主链已知，新 crowd 未对齐原案”

- 当前主线目标：
  - 先厘清原 Day1 各阶段真正需要谁，再判断当前工程里的老 `NPC001/002/003` 与新 `101~301` 各算什么。
- 本轮新增稳定结论：
  - 原 Day1 的强主链仍是：
    - `马库斯`
    - `艾拉`
    - `卡尔 / 研究儿子`
  - 其中：
    - `CrashAndMeet / EnterVillage / FarmingTutorial / ReturnAndReminder`
      主要由 `马库斯` 承载
    - `HealingAndHP / WorkbenchFlashback`
      主要由 `艾拉 + 马库斯` 承载
    - `DinnerConflict`
      主要由 `马库斯 + 卡尔` 承载
  - `老杰克 / 老乔治 / 老汤姆 / 小米`
    在原案里属于 Day1 可见或后续可接触的重要人物，
    但不是当前白天主流程每一拍都必须上台的人。
- 当前工程承载判断：
  - 老主链里：
    - `NPC001 / VillageChief` 是最稳的 `马库斯` 壳
    - `NPC002 / VillageDaughter` 是最稳的 `艾拉` 壳
    - `NPC003 / Research` 只是最接近 `卡尔` 的语义壳，还不是当前 Day1 正式 runtime 演出
  - 新 `101~301` 里：
    - 只有少数能留作 `群众层 / 次级氛围层`
    - 不能继续整体 claim 为“原剧本已扩完”
  - 特别是：
    - `101 / 201 / 301`
      这类语义在原 Day1 剧本里没有直接来源
- 当前恢复点：
  - `NPC-v` 先补原案映射与最小回正
  - `spring-day1V2` 之后再决定是否要最小收窄 manifest 的 phase 语义

## 2026-04-03｜补记：NPC-v 已完成“新 8 人不是原案真相源”的只读核实

- 当前主线目标：
  - 先把春一日新 `101~301` 群像和原案角色表拆开，
    防止继续把现编名字、人设和对白当成原剧本既定事实。
- 本轮新增稳定结论：
  - 原案真相源优先级已经重新钉死：
    - 用户原始 Day1 剧情原文
    - `0.0.1剧情初稿`
    - 长线 NPC 角色表
    - 当前 `bootstrap / manifest`
  - 老 `NPC001/002/003` 比当前新 `101~301` 更接近原 Day1 主角色链：
    - `NPC001` = `马库斯`
    - `NPC002` = `艾拉`
    - `NPC003` = `卡尔 / 研究儿子` 的最接近语义壳
  - 当前 `101~301` 中：
    - `102 / 103 / 203` 只有局部语义接近
    - `101 / 104 / 201 / 202 / 301` 没有原案直接来源
    - 尤其 `301` 明显不应继续 claim 为 Day1 正式角色
  - 当前能合法确认的是“谁写偏了、谁只能降回群众层”，
    不是“这 8 个槽位已经能一对一回正成稳定具名角色”
- 本轮动作：
  - 只读核实完成
  - 未改 NPC 资产本体
  - 维持 `PARKED`
- 当前恢复点：
  - 后续若要继续 NPC-v，本线只能先拿到更高权威的 cast 映射证据，
    或显式改成“匿名 / 次级群众”收口策略，再做最小回正

## 2026-04-04｜补记：NPC 线已收到新的硬切片 prompt

- 当前主线未变：
  - NPC 线继续只做 `原剧本角色回正 + NPC 本体收口`，不再混入 `spring-day1` opening 或玩家面 UI。
- 本轮新增落点：
  - 已新增续工文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`
- 这份 prompt 明确要求：
  1. 先按原案权威来源回正 `101~301`
  2. 证据不足时只允许降级为匿名 / 次级群众层
  3. 把 `pair bubble` 真正修到 runtime 亮起来
  4. 把 NPC 旧气泡样式回到用户认可旧版
  5. 不碰 `Primary.unity / GameInputManager.cs / PromptOverlay / Workbench / opening / Town / 字体`
- 当前恢复点：
  - 下一轮以 `prompt_05` 为唯一入口继续，不再沿旧口径各说各话

## 2026-04-05｜补记：NPC 线已收到 Day1 owner 的真值补线

- 当前统一入口已更新为：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_NPC-v_Day1真值补线与NPC正式非正式优先级续工prompt_06.md`
- 这份 prompt 相比 `prompt_05` 新增 3 个硬要求：
  1. 明确 formal/casual/ambient 的 Day1 优先级真值
  2. 明确 `101~301` 的群众层 / 线索层 / 氛围层降级口径
  3. 明确外部编译红只阻断 live，不阻断静态 own 回正

## 2026-04-04｜补记：NPC 现阶段不再泛吞 UI / Day1，而是先拆清“玩家面 NPC own”与“NOC本体收口”

- 当前稳定判断：
  - `UI` 负责壳体与左下角提示底座。
  - `spring-day1` 负责 Day1 正式剧情顺序与状态。
  - `NPC` 当前应只守：
    - 玩家面里真正属于 NPC 的 speaking-owner / bubble / 正式-非正式闭环语义
    - 以及 `101~301` 的原剧本口径回正与 NPC own runtime 收口
- 当前下一步优先级：
  1. 先钉死 `exact-own / 协作切片 / 明确不归我`
  2. 再以 `NPC own bubble / speaking-owner / 正式-非正式闭环` 作为第一刀真实施工
- 本轮状态：
  - 只读分析
  - 未跑 `Begin-Slice`

## 2026-04-05｜补记：NPC own 本体继续落在“会话/气泡底座收口”，当前先因外部编译 blocker 停车

- 当前主线目标：
  - 继续只做 `NPC own` 的会话/气泡底座，不回吞 UI 壳与 Day1 integration。
- 本轮新增稳定结论：
  1. `NPC own` 当前有效施工面仍集中在：
     - `PlayerNpcChatSessionService`
     - `NPCBubblePresenter`
     - 相关 own 测试
  2. 这轮没有再动气泡主题样式；
     - 只确认了当前代码里的样式计数口径：
       - `4` 种 live 主样式
       - `7` 条工程壳
  3. 当前 `NPC own` 代码面仍是可编的：
     - own 脚本 `validate_script` 继续 `0 error`
  4. 但 Unity 现场现在被外部文件挡住：
     - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs(1694,23)`
     - 所以这轮不能 claim “Unity 运行态已验完”
- 当前恢复点：
  - 先等外部编译红解除；
  - 再恢复 NPC own targeted test / live probe。

## 2026-04-05｜补记：NPC own 剩余测试契约现已收平，当前正式缩到“内容细修 + 用户终验”阶段

- 当前主线目标未变：
  - 继续只守 NPC own 的会话/气泡底座、crowd 内容与 targeted probe；
  - 不回吞 UI、Day1 导演、Primary、Town、GameInputManager。
- 本轮新增稳定事实：
  1. `NpcAmbientBubblePriorityGuardTests` 的残留红并不是 runtime contract 真坏，而是：
     - 测试里把 `InteractionContext` 误反射到 `UnityEditor.InteractionContext`
     - `NPCBubblePresenter` 在 EditMode `Awake / OnValidate` 里过早创建 UI，制造测试噪音
  2. 这两处都已收平：
     - `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  3. fresh `Tests.Editor` 结果里，NPC own 相关 fixture 当前已全部转绿：
     - `NpcAmbientBubblePriorityGuardTests` = `4/4`
     - `NpcBubblePresenterEditModeGuardTests` = `5/5`
     - `NPCInformalChatInterruptMatrixTests` = `16/16`
     - `PlayerThoughtBubblePresenterStyleTests` = `7/7`
  4. fresh targeted probe 继续维持：
     - `instance=8/8`
     - `informal=8/8`
     - `pair=2/2`
     - `walkAway=2/2`
  5. `Validate New Cast` 继续 `PASS | npcCount=8 | totalPairLinks=16`
  6. 清 console 后 CLI 已回到 `errors=0 warnings=0`
- 当前阶段判断：
  - 这轮之后，NPC own 的代码层 / 测试层 / targeted probe 层已经没有新的自家 blocker；
  - 剩余真正高价值工作只剩：
    - crowd 内容语气 / 强度细修
    - 用户真实体验终验
- thread-state：
  - 当前 slice：`npc-own-remaining-test-contract-closure-20260405`
  - 已跑：
    - `Park-Slice`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  - 后续若继续 NPC 本线，直接从：
    1. crowd 文案微调
    2. `pair / informal / walk-away` 体感终验
    继续；
  - 不必再回查这组 NPC own tests 为何红。

## 2026-04-05｜补记：只读复核再次确认旧 interrupt/style fail 与当前代码真相不一致

- 本轮补充稳定事实：
  1. 额外只读核查后，`ResumeIntroPlan` 与 `PlayerThoughtBubblePresenterStyleTests` 相关旧失败继续被判定为旧 runner/旧编译快照味道更重；
  2. 当前磁盘代码与当前已编 `Assembly-CSharp.dll` 已经对上：
     - `CreateFallbackResumeIntroPlan()` 的 continuity lines
     - 玩家气泡 preset 的 distinct / opaque 要求
     - 玩家气泡 `FormatBubbleText()` 的 10 字换行节奏
- 当前判断：
  - NPC own 现在更不该继续盲改这些点；
  - 只有 fresh runner 重新给出真实失败时，才值得再回到 interrupt/style 契约补口。
## 2026-04-05｜补记：NPC own formal/casual 门禁已回正，002/003 的 casual 闭环与玩家首句跑开中断都拿到 live 硬证据
- 当前主线未变：
  - NPC 线程继续只守非正式聊天会话底座、NPC 气泡与 speaking-owner / 中断链；
  - 不吞 UI 玩家面壳体，也不回改 spring-day1 正式剧情控制。
- 本轮新增稳定事实：
  1. 之前拦住  02 / 003 的关键原因，不是 casual 底座没写，而是 ormal phase 被误实现成“全局封杀所有 casual”。
  2. 现在这条门禁已改成：
     - 只有“同一个 NPC 自己的 formal dialogue 当前能接管”时，才 suppress casual。
  3. fresh console 已回到：
     - errors=0 warnings=0
  4. live probe 已证实：
     -  02 两轮闭环完成
     -  02 玩家首句跑开中断完成
     -  03 两轮闭环完成
     -  03 玩家首句跑开中断完成
- 当前恢复点：
  - 后续如继续本线，优先转回用户可感知体验层，而不是再把时间耗在“为什么 002/003 完全起不了聊”的静态门禁上。

## 2026-04-05｜更正：以上新增 NPC 门禁记录以本条中文校正版为准

- 本轮核心不是换 UI 壳，也不是改 Day1 正式链。
- 本轮核心是把 NPC casual 门禁从 phase 全局封杀改回对象级 takeover。
- 现有 live 结论：002 闭环通过，002 跑开中断通过，003 闭环通过，003 跑开中断通过。

## 2026-04-05｜补记：NPC own 新 8 人 crowd 本体链已再向前一格，pair runtime 与群众语义同轮收口

- 当前主线目标：
  - 继续把 `NPC own` 收在：
    - `pair / ambient / bubble`
    - `crowd content/profile/prefab`
    - `101~301` 的群众层/线索层/夜间见闻层口径
  - 不回吞 Day1 导演、UI、Town、Primary、GameInputManager。
- 本轮新增稳定事实：
  1. `pair bubble` 已不再是 live blocker：
     - fresh runtime targeted probe 已真实回到 `pair=2/2`
     - 同轮仍保持：
       - `instance=8/8`
       - `informal=8/8`
       - `walkAway=2/2`
  2. `NPCBubblePresenter` 与 `NPCAutoRoamController` 已补进：
     - stale prompt suppression 自动释放
     - ambient bubble 极短 retry
  3. `SpringDay1NpcCrowdValidationMenu` 已收掉：
     - prompt suppression / conversation owner 残留
     - pair 行文比较被格式化换行误伤
     - probe 离场 `DontSave Assert`
  4. `101~301` 这批 crowd 的内部 token / bootstrap 生成口径已进一步降级为：
     - 群众层
     - 线索层
     - 夜间见闻层
     不再继续往“Day1 正式具名角色”漂。
- 当前阶段判断：
  - NPC own 当前不再主要差 runtime 发声链；
  - 下一段更适合转回用户可感知体验层与更细的内容语义复核。
- 当前恢复点：
  - 后续若继续 NPC 本线，直接从：
    - 用户体验复核
    - crowd 内容细修
    - sync 前 own 范围收口
    继续。

## 2026-04-05｜补记：NPC own 又收一刀静态护栏，ambient 只在 formal 当前接管时才压，crowd 8 人 duty/phase matrix 已补满

- 当前主线目标：
  - 继续只收 `NPC own` 的会话/气泡优先级与 crowd 静态真值，不回吞 UI、Day1 导演、Primary、Town。
- 本轮稳定新增：
  1. `NpcInteractionPriorityPolicy.cs`
     - ambient 已不再按 formal phase 全局禁用；
     - 只有：
       - formal 对话正在播；
       - 或当前 focus 真落在可接管的 formal NPC；
       才 suppress ambient。
  2. `NpcAmbientBubblePriorityGuardTests.cs`
     - 新增“有 formal 接管才压 ambient、无接管不该全灭”的回归覆盖。
  3. `NpcInteractionPriorityPolicyTests.cs`
     - phase 测试回正为只直控 casual；
     - ambient 的无接管保留逻辑已补断言。
  4. `NpcCrowdManifestSceneDutyTests.cs`
     - 从抽查补成 8 人 exact matrix；
     - duty / anchor / growth / minPhase / maxPhase 现都在静态测试里被钉住。
- fresh 证据：
  - 脚本级 `manage_script validate` clean
  - fresh console：`errors=0 warnings=0`
  - `Validate New Cast`：`PASS`
  - runtime targeted probe：`PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
- 当前判断：
  - NPC own 这轮又推进了一格，但仍然属于“结构 + targeted probe 成立”；
  - 真正还没做完的只剩：
    - crowd 内容语气细修
    - 用户真实体验终验
- 当前恢复点：
  - 后续继续时，直接从体验复核或内容微调进入，不必再回查 ambient 是否被 formal phase 全局闷死。

## 2026-04-05｜补记：crowd dialogue 刷新链与人设化 interrupt/resume 已落到资产层，fresh runtime 复跑被 foreign 噪音打断

- 当前主线目标：
  - 继续只做 NPC own 的 crowd 内容与 crowd dialogue 资产链；
  - 不回吞 `Primary / Town / Day1 Director / UI / GameInputManager`。
- 本轮完成：
  1. `SpringDay1NpcCrowdBootstrap.cs`
     - 新增只刷新 crowd dialogue 的菜单；
     - 给 8 人补上 default `interrupt / resume` 人设化内容；
     - 用稳定旧 `AssetStem` 映射锁住 legacy asset 路径，避免 slug 改名后再生成第二套对话包。
  2. `NpcCrowdDialogueNormalizationTests.cs`
     - 新增 stale crowd 口气黑名单；
     - 新增 `defaultInterruptRules / defaultResumeRules` 不得为空的护栏；
     - 新增 refresh 菜单和 default rules 源头护栏。
  3. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 8 份 legacy dialogue asset 已刷新到新 crowd 文案与 default rules；
     - 误生出的新 slug 重复资产已全部清掉。
- fresh 证据：
  - `manage_script validate` clean：
    - `SpringDay1NpcCrowdBootstrap.cs`
    - `NpcCrowdDialogueNormalizationTests.cs`
  - `git diff --check` 通过。
  - `Editor.log` 已拿到：
    - `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - `Validate New Cast` 后 fresh console => `errors=0 warnings=0`
  - fresh runtime probe 复跑：
    - 已真实进 Play 再回 Edit；
    - 但只拿到 `START`，没拿到新的 `PASS / FAIL` 终行；
    - console 同轮混入 foreign：
      - 串台 menu request `Run Director Staging Tests`
      - `PersistentManagers DontDestroyOnLoad` editor exception
    - 因此这轮不能把 fresh runtime probe 继续 claim 成 clean pass。
- 当前判断：
  - crowd 这条线的代码/资产底座已继续站住；
  - 剩余高价值 own 工作已缩到：
    1. foreign 噪音停下后补 clean runtime targeted probe
    2. 用户真实体验终验与少量语气微调
- thread-state：
  - 本轮沿用 slice：`npc-own-crowd-content-deep-polish-20260405`
  - 已跑：`Begin-Slice`、`Park-Slice`
  - 未跑：`Ready-To-Sync`
  - 当前状态：`PARKED`

## 2026-04-05｜补记：quick test 已把 crowd runtime probe 真正补成 clean pass

- 本轮 quick test：
  - `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - `Run Runtime Targeted Probe` => `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  - fresh console => `errors=0 warnings=0`
  - Unity 已回到 Edit Mode
- 当前判断：
  - NPC own 这段现在不再缺 targeted runtime 证据；
  - 剩余主要转成用户体验终验与极少量文案微调。
- thread-state：
  - 本轮 slice：`npc-quick-clean-runtime-retest-20260405`
  - 已跑：`Begin-Slice`、`Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-05｜补记：只读评估 day1 导演续工单后，确认 NPC own 不再是 Town 轻量承接的主 blocker

- 本轮只读判断对象：
  - 用户转述的 `spring-day1` 自己给自己的续工 prompt；
  - 核心关注：
    1. day1 当前主线是否站稳
    2. Town 现在是否跟得上 day1
    3. 从 NPC own 完成度看，NPC 还能不能担起 Day1 群像承接这份量
- 当前稳定判断：
  1. 这份 day1 prompt 本身方向是对的：
     - 没漂去方案模式；
     - 继续把“后半段导演消费”与“轻量导演工具 MVP live 可用”并行往下打；
     - 并且没有误把 Town 当成“已经全 runtime 闭环”的大场景。
  2. 从 NPC own 视角看，当前不再是 NPC 底座拖 day1 后腿：
     - 8 人 crowd cast 已有稳定资产链；
     - `Validate New Cast` 已 pass；
     - `Runtime Targeted Probe` 已 pass；
     - formal / casual / ambient priority、pair / informal / walk-away、crowd interrupt / resume 都已落到 own 底座。
  3. 因此 Town 现在“能跟上”，但前提是继续按窄口径推进：
     - `EnterVillageCrowdRoot`：能跟
     - `KidLook_01`：能跟
     - `NightWitness_01`：能跟
     - `DinnerBackgroundRoot`：能继续吃一层，但别一口气做复杂豪华层
     - `DailyStand_01`：在前面站稳后可跟
  4. Town 现在“不能被当成已经完全跟上”的部分：
     - 不能默认整张 `Town` 都 ready
     - 不能直接扩成复杂多人导演编排
     - 不能跳过具体 anchor / cue / live 保存，直接宣称后半段已全闭环
  5. 当前真正更像 day1 主 blocker 的，不是 NPC own，而是导演 live 工具链自己：
     - `Run Director Staging Tests` 菜单桥接识别问题
     - live 排练/写回是否能持续稳定保存
     - 外部噪音不要反复污染导演验证
- 对“NPC 在 day1 里有份量，要担得起”的结论：
  - 可以认这个责任；
  - 现在 NPC own 已经担得起“Day1 后半段群像/背景/夜见闻的底座承接”；
  - 但还不能把“NPC own 底座 ready”偷换成“Day1 导演线全完工”。

## 2026-04-05｜补记：day1 / Town / NPC 三方关系的口径进一步压实

- 当前进一步压实后的判断：
  1. day1 现在最值钱的动作，不是再扩分析，而是把导演窗口真实接到几个既定 anchor 上，证明排练、录制、写回这条链能持续工作。
  2. `NPC own` 当前对 day1 的价值，已经从“先补底层可用性”切到“可稳定承接群像内容与 runtime probe”。
  3. `Town` 现阶段的定位应该是“可被导演消费的承接层”，不是“完整自由编排舞台”。
- 对后续协作的稳态理解：
  - NPC 这边已经能托住 day1 给出的那批轻量群像承接点；
  - 但如果 day1 自己的 live 工具链没有持续站住，Town 再能承接也会显得像没接上。

## 2026-04-05｜补记：Day1 后半段群像内容层已完成第一轮实装与资产刷新

- 本轮不是补 NPC 底座，而是补 `Day1` 后半段会被导演线消费的群像内容层。
- 当前已实装的内容方向：
  - `EnterVillage`
    - `101 / 103` 补成“停手围观 / 偷看 / 压低嗓子 / 次日照常站位”口径
  - `Dinner / ReturnAndReminder`
    - `104 / 201 / 202 / 203` 补成“桌上压力 / 散场回屋 / 低声议论 / 仍要照旧开门开摊开火”口径
  - `NightWitness / DayEnd`
    - `102 / 301` 补成“后坡 / 夜路 / 回声 / 收夜规矩 / 白天照常但夜里露底”口径
- 本轮结果：
  - `SpringDay1NpcCrowdBootstrap.cs` 已更新 crowd 文本源；
  - 8 份 `Assets/111_Data/NPC/SpringDay1Crowd/*.asset` 已通过刷新菜单重新生成；
  - `NpcCrowdDialogueNormalizationTests.cs` 已新增资产级语义护栏；
  - `Validate New Cast` 和 `Runtime Targeted Probe` 复核仍是 `PASS`。
- 当前判断：
  - 现在 `NPC own` 对 day1 的价值已经不只是“底座通了”，而是“后半段群像真的有可消费内容了”；
  - 这能继续支撑 day1 往导演消费链下沉，而不需要我去碰导演工具或 Town runtime。

## 2026-04-06｜补记：已确认 crowd “突然生成”是 runtime deployment contract 问题

- 这轮只读排查的重要结论：
  1. 用户看到的“新 NPC 像是到点才蹦出来”，判断是对的。
  2. 真正执行这套逻辑的是：
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  3. 它当前的设计是：
     - 只在 `Primary` 场景下维护 crowd
     - 不在 `Primary` 时 `TeardownAll()`
     - 需要时 runtime `Instantiate(entry.prefab, spawnPosition, ...)`
  4. 所以这不是单纯的文本/内容问题，而是 crowd 部署契约问题：
     - “按阶段临时生成 + 显隐”
     - 而不是“村里本来就有这批常驻 NPC”
- 当前边界判断：
  - 这件事不应被我这条 NPC 内容线默认吞掉；
  - 因为它已经进入 `SpringDay1NpcCrowdDirector.cs / Town runtime contract / scene 常驻布置` 这一层。

## 2026-04-06｜补记：已把 NPC -> day1 的阶段性回执与引导 prompt 正式落盘

- 当前新增交接材料：
  1. `2026-04-06_NPC给day1_后半段群像回执与驻村部署问题汇总_09.md`
  2. `2026-04-06_NPC给day1_读取回执并判断crowd驻村部署prompt_10.md`
- 这两份材料的作用：
  - 先把我这边近几轮已完成的 crowd 底座与后半段群像内容层结果讲清；
  - 再把“突然生成”问题从内容层剥离出来，正式交给 day1 从 runtime deployment / Town contract 角度判断下一步。

## 2026-04-06｜补记：已把 crowd 压成 resident 可消费矩阵，并补上 formal 一次性消耗契约

- 这轮不再只是“内容方向对”，而是把 `101~301` 真正压成：
  1. `residentBaseline`
  2. 6 段 `residentBeatSemantics`
  3. `presenceLevel / flags / note`
  4. `IsDirectorPriorityBeat / IsDirectorSupportBeat / IsDirectorTraceBeat`
- 同时补上 formal 一次性消费硬约束：
  - `NPCDialogueInteractable` 的 formal 段现在消费后不再重复；
  - 同 NPC 后续自动回落到 informal / resident / phase 后补句。
- 对 day1 的实际意义：
  - `NPC own` 现在已经提供：
    1. 可直接吃回的 resident actor semantic contract
    2. formal 与 post-consume 闲聊的清晰边界
  - 下一步更像 day1 去接 deployment / director consumption，而不是我继续补 NPC 内容壳。

## 2026-04-06｜补记：resident / formal 现场后的下一层建议已收敛到 helper + contract + bridge test

- 当前主线目标：
  - 在不越界到 `CrowdDirector / scene / UI / GameInputManager` 的前提下，判断 NPC 还能补哪层内容，最方便 day1 直接消费。
- 本轮只读判断：
  1. NPC 自己下一层最值钱的，不再是继续补 runtime/deployment；
     而是把已有 resident manifest 与 formal once-only 压成更易消费的只读 contract。
  2. manifest 侧目前最值得补的是 beat 级消费快照 helper；
     formal 侧目前最值得补的是公开的消费状态 contract。
  3. 测试侧目前最值得补的是 manifest / stagebook / formal 回落三者之间的 bridge test；
     因为单边测试已经有了，桥接断言还不够。
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
  - `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
  - `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
- 当前恢复点：
  - 若后续继续 NPC 自己的一刀，应优先落：
    1. beat-consumption helper
    2. formal state contract
    3. bridge tests

## 2026-04-06｜补记：beat-consumption helper 与 formal state contract 已真实落地

- 这轮不是继续补 runtime/deployment，而是把 NPC own 的可消费 contract 再压一层：
  1. `SpringDay1NpcCrowdManifest.cs`
     - 已补 `SpringDay1CrowdDirectorConsumptionRole`
     - 已补 beat 级 `BuildBeatConsumptionSnapshot()`
     - 已补按 role 直接取 roster 的入口
     - 已把“前台 priority / 后台 pressure / trace”从 resident flags 里显式拆出来
  2. `NPCDialogueInteractable.cs`
     - 已补 `NPCFormalDialogueState`
     - 已补 `GetFormalDialogueStateForCurrentStory()`
     - 已补 `HasConsumedFormalDialogueForCurrentStory()`
     - 已补 `WillYieldToInformalResident()`
- 同步补到的护栏：
  - `NpcCrowdManifestSceneDutyTests.cs`
    - direct-consumption role + snapshot helper
  - `NpcInteractionPriorityPolicyTests.cs`
    - formal consumed 后公开状态 contract
  - `SpringDay1DialogueProgressionTests.cs`
    - public formal-state 文本护栏
  - `SpringDay1NpcCrowdValidationMenu.cs`
    - `Validate New Cast` 已把 resident consumption roster 也纳入检查
- 本轮现场修复：
  - `SpringDay1NpcCrowdManifest.asset` 一度是旧脏资源，`presenceLevel` 全为 `0`
  - 已跑 `Refresh Crowd Resident Manifest` 刷回
  - 最新 `Validate New Cast` 已重新 `PASS`
- 当前验证：
  - touched scripts `manage_script validate` 全 clean
  - `Assets/Refresh` 后 fresh `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
  - `git diff --check` 通过
  - fresh console `errors=0 warnings=0`
  - Unity 已回到 / 保持 `Edit Mode`
- 当前结论：
  - `0.0.2` 里 NPC own 这轮最值钱的 helper / contract 层已经压完；
  - 继续往下就是 `day1` deployment / director consumption / Town scene 承接，不该再默认由 NPC 线程吞。

## 2026-04-06｜补记：manifest 与 stagebook 的桥接护栏也已补完

- 本轮继续只守 NPC own，不去改 director runtime：
  1. 新增 `Assets/YYY_Tests/Editor/NpcCrowdResidentDirectorBridgeTests.cs`
     - 把 manifest resident roster 与 stagebook cue 的桥接关系钉住
  2. 新增 `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
     - 让这 3 个 bridge tests 有可直接运行的菜单入口
- 本轮真实结果：
  - 新菜单初次没注册，查实是脚本里 `TestStatus` 写法导致的 own 编译错
  - 已修正后重新 `Assets/Refresh`
  - 最新桥接测试菜单结果：
    - `passed / total=3`
- 当前验证：
  - touched files `manage_script validate` 全 clean
  - `git diff --check` 通过
  - fresh console `errors=0 warnings=0`
- 当前阶段：
  - resident semantic matrix
  - beat roster helper
  - formal once-only public contract
  - resident <-> stagebook bridge tests
  以上 4 层都已站住
- 当前恢复点：
  - NPC 这条线后续若没有新的明确授权，已经没有太多值得继续独写的代码面了；
  - 主 blocker 已稳定转到 day1 / deployment / Town 常驻落位。

## 2026-04-06｜补记：已向 day1 追加一份全量进度与边界回执，准备先停车等调度

- 当前主线：
  - `NPC` 本线先停在“把这几轮全部真实进度、边界和后续可协作层说透”这一步；
  - 不再继续扩新的 `NPC` runtime 代码。
- 本轮实际新增：
  1. 回执文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_全量进度与承接边界回执_14.md`
  2. 文档已完整交代：
     - resident semantic matrix / beat consumption helper / formal once-only contract / bridge tests 的当前完成度
     - `NPC` 这边已经做成什么
     - `day1` 真正该接的 blocker 是什么
     - 我最深还能继续帮忙扛到哪一层
     - 我明确不建议默认回吞的边界
     - 我建议 `day1` 继续调度的顺序
- 当前判断：
  - `NPC` own 现在不是“还缺概念层”，而是“底座已交，deployment/runtime 才是主 blocker”。
  - 如果后续 `day1` 还要我继续帮忙，收益最高的协作点仍是：
    - `manifest / content profile / formal fallback contract / tests / targeted probe`
  - 不该默认继续吞的是：
    - `CrowdDirector / Town runtime / scene 落位 / UI`
- 当前恢复点：
  - 当前 slice 准备 `Park-Slice`
  - 等 `day1` 审回执后，再决定是否给 `NPC` 新授权

## 2026-04-06｜补记：resident fallback 已补到 selfTalk + phase walkAway，compile red 已清

- 当前主线：
  - 按 `day1` 27 号补充口径，继续只做 `NPC` own 的 resident 常驻语义、formal consumed 回落，以及给 `day1` 真要吃的 validation/contract；
  - 不继续围绕 `runtime spawn / deployment` 扩写。
- 本轮新增完成：
  1. `phase-aware selfTalk`
     - 已从 `NPCDialogueContentProfile -> NPCRoamProfile -> NPCAutoRoamController -> crowd dialogue assets` 全链路打通
  2. `phase-aware walkAwayReaction`
     - `phaseInformalChatSets` 现在会优先吃 phase-specific `walkAwayReaction`
     - 8 个 crowd NPC 已补对应 cue 与文本
  3. validation / tests
     - `SpringDay1NpcCrowdValidationMenu.cs` 新增 `ValidatePhaseSelfTalkCoverage`
     - `NpcCrowdDialogueNormalizationTests.cs` 新增 selfTalk / walkAway 两条 coverage
     - `SpringDay1DialogueProgressionTests.cs` 新增 selfTalk / walkAway 两条 contract test
  4. carried foreign blocker fix
     - 最小补了 `SpringDay1NpcCrowdDirector.cs` 缺失的 `EnumerateAnchorNames(...)`
     - fresh `errors` 已回到 `errors=0 warnings=0`
  5. 新回执：
     - `2026-04-06_NPC给day1_阶段安全点回执_16.md`
     - `2026-04-06_NPC_存档边界回执_02.md`
- 本轮验证：
  - `Validate New Cast` 仍 `PASS | npcCount=8 | totalPairLinks=16`
  - targeted EditMode 小集 job `succeeded`
  - `git diff --check` 对 own + carried 路径通过
  - Unity 保持 `Edit Mode`
- 当前判断：
  - `NPC` 本线当前最值钱的继续推进，仍然是把 resident contract / content density / tests 压厚；
  - “看起来像突然生成”的问题，责任中心仍在 `day1 / Town / deployment`。
- 当前恢复点：
  - 这轮代码安全点已经形成；
  - 准备写完 memory 后 `Park-Slice` 离场，等待后续新授权。

## 2026-04-06｜收口：NPC 当前状态已 PARKED

- `Park-Slice` 已执行：
  - `status = PARKED`
  - `blockers = 无`
- 当前现场：
  - Unity `Edit Mode`
  - fresh `errors = 0`
  - 可以把后续接力点直接交给 `day1 / Town` 或下一轮 NPC 自己

## 2026-04-06｜续记：resident fallback 新增 phase-aware nearby，已同步回执 day1 与存档系统

- 当前主线：
  - 继续把 `NPC` own 的 resident fallback 压到更完整的 contract；
  - 这轮新增的关键不是再补一层闲聊，而是把玩家靠近 NPC 时的 nearby 轻反馈也做成 `StoryPhase aware`；
  - 完成后在安全点同时回执给 `day1` 与 `存档系统`。
- 本轮新增完成：
  1. 代码层：
     - `NPCDialogueContentProfile.cs` 新增 `PhaseNearbySet / phaseNearbyLines / TryGetPhaseNearbySet`
     - `NPCRoamProfile.cs` 新增 phase-aware nearby 透传
     - `PlayerNpcNearbyFeedbackService.cs` 改为按当前 `StoryPhase` 解析 nearby resident lines
  2. 资产生成层：
     - `SpringDay1NpcCrowdBootstrap.cs` 已能生成 `phaseNearbyLines`
     - 新 8 人 crowd dialogue assets 已刷新写回该字段
  3. 验证层：
     - `SpringDay1NpcCrowdValidationMenu.cs` 新增 `ValidatePhaseNearbyCoverage`
     - tests 已补 `NpcInteractionPriorityPolicyTests / NpcCrowdDialogueNormalizationTests / SpringDay1DialogueProgressionTests`
  4. 文档回执：
     - `2026-04-06_NPC给day1_阶段安全点回执_15.md`
     - `2026-04-06_NPC_存档边界回执_01.md`
- 本轮验证：
  - own 改动脚本 `manage_script validate` 基本 clean；
  - `SpringDay1DialogueProgressionTests.cs` 自身 `native_validation=clean / owned_errors=0`，但被现场 external console 噪音打成 `external_red`
  - `Validate New Cast` 仍然 `PASS | npcCount=8 | totalPairLinks=16`
  - own `git diff --check` 通过
- 当前判断：
  - `NPC` 本体现在已经把“formal consumed 后怎么回 resident”压到了 `闲聊 + nearby` 两层
  - 后续更该由 `day1` 继续接的是：
    - deployment
    - director consumption
    - Town 常驻落位
  - `存档系统` 第一刀只该接长期态，不该碰聊天过程态
- 当前恢复点：
  - 这轮不再继续扩新的 NPC 功能；
  - 准备 `Park-Slice` 离场，等待后续新授权。

## 2026-04-07｜农场动物 prefab 切片：5 个动物已生成可漫游预制体

- 当前主线：
  - 响应用户新任务，快速把 `Assets/ZZZ_999_Package/001_OK/Await/Farm RPG FREE 16x16 - Tiny Asset Pack/Farm Animals` 里的鸡 / 牛做成可直接落场景的可漫游 prefab；
  - 不碰 `Primary.unity`，只做 prefab、动画、profile 和最小运行时映射补口。
- 本轮完成：
  1. 运行时：
     - `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`
     - 新增 `useCustomFacingVisualMap` 与 4 向独立 `AnimatorDirection / FlipX` 映射
     - 保持旧 NPC 默认行为不变，动物 prefab 才启用自定义映射
  2. Editor 生成链：
     - 新增 `Assets/Editor/FarmAnimalPrefabBuilder.cs`
     - 菜单：`Tools/NPC/Farm Animals/生成可漫游动物预制体`
     - 自动完成：切片 -> clip -> controller -> prefab -> roam profile
  3. 产物：
     - `Assets/222_Prefabs/FarmAnimals/`
       - `Baby Chicken Yellow.prefab`
       - `Chicken Blonde  Green.prefab`
       - `Chicken Red.prefab`
       - `Female Cow Brown.prefab`
       - `Male Cow Brown.prefab`
     - `Assets/100_Anim/FarmAnimals/` 下每只动物各自的 `Idle/Move + Down/Side/Up` clip 与 controller
     - `Assets/111_Data/NPC/FarmAnimals/`
       - `FarmAnimal_ChickenRoamProfile.asset`
       - `FarmAnimal_CowRoamProfile.asset`
     - 5 张源图 `.meta` 已重切片
- 动物朝向真值：
  - 鸡：
    - `Down/Left -> Row0`
    - `Up/Right -> Row1`
    - 不做左右翻转
    - `Baby Chicken Yellow` 第 3 行是蛋，本轮只取前 2 行做运动动画
  - 牛：
    - `Left -> Row0`
    - `Right -> Row0 + flip`
    - `Down -> Row1`
    - `Up -> Row2`
- 本轮验证：
  - `validate_script Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`：`owned_errors=0`，但被外部 `missing script` 现场打成 `external_red`
  - `validate_script Assets/Editor/FarmAnimalPrefabBuilder.cs`：`owned_errors=0`，同样只剩外部 `missing script`
  - direct MCP `validate_script` 两文件 `errors=0 warnings=0`
  - `execute_menu_item("Tools/NPC/Farm Animals/生成可漫游动物预制体")` 成功
  - 菜单执行后 fresh console `0` 条新 `error/warning`
  - own 路径 `git diff --check` 通过
- 当前未做：
  - 还没有把这些动物真正摆进具体 scene 做 live 漫游体验验证；
  - 当前只完成“prefab + 动画 + roam profile + 运行时映射”这层。
- 当前现场：
  - `thread-state = PARKED`
  - Unity 保持打开，已回到 `Edit Mode`
  - 全局 external blocker 仍是现场已有的 `The referenced script (Unknown) on this Behaviour is missing!`
- 当前恢复点：
  - 后续如果用户要继续，只需把这些 prefab 摆进目标 scene，再按需要补 `HomeAnchor / activityRadius / live roam` 调整即可；
  - 本轮自己的 prefab 生产链已经可复跑，不需要手工重做。

## 2026-04-07｜Town 卡顿只读复核：热点已从 roam 转到 NPCInformalChatInteractable.Update

- 用户现象：
  - 在 `Town` 里过完正式对话后出现持续卡顿；
  - Profiler 截图显示 `NPCInformalChatInteractable.Update()` 常驻 `35ms ~ 37ms`，`NPCAutoRoamController.FixedUpdate()` 只有 `0.18ms ~ 0.30ms`。
- 这轮只读结论：
  1. 这次不是之前那种 roam/navigation 雪崩
     - `NPCAutoRoamController.FixedUpdate` 已经不是主热点
  2. 当前主热点确实是 `NPCInformalChatInteractable.Update`
     - 但不是一句“它在 `FindObjectsOfType`”就能概括完
     - 真正贵的是整个 per-NPC per-frame 提示链
  3. 代码证据：
     - `NPCInformalChatInteractable.Update()` 每个 NPC 每帧都会先 `BuildInteractionContext()`：
       - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs:110`
       - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs:382`
       - 里面直接 `FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include)`
     - `ReportProximityInteraction()` 里又会：
       - 再次 `ResolveSessionService(context)`：`333`
       - 再次调用 `CanInteract(context)`：`345`
       - `CanInteract()` 内部又再次 `ResolveSessionService(effectiveContext)`：`139`
     - `ResolveSessionService()` 不是直连，而是反射链：
       - `405 ~ 427`
       - `InvokeSessionBool/InvokeSessionString` 也都是反射调用
  4. 为什么“过完正式对话后”更明显：
     - 正式对话进行时，`ReportProximityInteraction()` 会在
       `326 ~ 330` 因 `DialogueManager.IsDialogueActive` 提前返回
     - 对话结束后，这个提前返回没了，Town 里所有启用的 informal NPC 都开始走完整提示链
     - Profiler 里的 `9 calls` 基本就对应当前场上的多只 NPC
- 当前判断：
  - 外部分析方向是对的，`下一刀应该砍 NPCInformalChatInteractable.cs`
  - 但更准确的说法应该是：
    - `FindObjectsOfType` 只是看得见的一块硬证据
    - 真正要砍的是“每个 NPC 每帧自己找玩家 + 反射找会话服务 + 重复判定 + 再上报提示”的整条链
- 下一刀最该改的点：
  1. 去掉 `BuildInteractionContext()` 里的 per-frame 全场找玩家
  2. 去掉 `ResolveSessionService()` 的反射调用链，改成直连 / 缓存
  3. 合并 `ReportProximityInteraction -> CanInteract` 的重复计算
  4. 如果还不够，再把“每个 NPC 自己轮询”改成“玩家侧统一收集附近候选”
- 当前状态：
  - 这轮只读分析
  - 未进入真实施工
  - 未跑 `Begin-Slice`

## 2026-04-07｜Town 卡顿第一刀已落：informal/formal prompt 链缓存化

- 当前主线：
  - 针对 `Town` 里“过完正式对话后卡顿”的真热点，直接压第一刀性能修正；
  - 不改 scene，不改内容，只砍 `NPC` 提示链的 per-frame 热路径。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - 去掉每帧反射找 `PlayerNpcChatSessionService`
     - 改成直连 `PlayerNpcChatSessionService`
     - 把玩家 `Transform / sample point` 改成按帧缓存
     - 合并 `ReportProximityInteraction -> CanInteract` 的重复会话/提示判定
  2. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - 同样把 `BuildInteractionContext()` 的玩家查找改成按帧缓存
     - 避免 formal 提示链继续保留同类热点
  3. `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
     - `StoryManager` 改成缓存查找，不再每次判定都全场找
     - `BuildCurrentInteractionContext()` 里的玩家查找也改成按帧缓存
- 这刀砍掉的真实热点类型：
  - `FindFirstObjectByType<PlayerMovement>` 的 per-NPC per-frame 全场查找
  - `ResolveSessionService()` 的反射链
  - 同一帧内对 suppression / session / prompt 的重复计算
- 本轮验证：
  - `git diff --check` 对 own 3 文件通过
  - direct MCP `validate_script`：
    - `NPCInformalChatInteractable.cs`：`errors=0 warnings=1`
    - `NPCDialogueInteractable.cs`：`errors=0 warnings=0`
    - `NpcInteractionPriorityPolicy.cs`：`errors=0 warnings=0`
  - CLI `errors` 仍被外部 blocker 挡住：
    - `ItemTooltip` 缺符号，涉及 `EnergyBarTooltipWatcher / Inventory UI`
- 当前没法继续拿 live 证据的原因：
  - Unity 当前存在外部 compile red，不是这轮 own red；
  - 所以这轮只能 claim “代码层热点已砍”，不能 claim “Town live 帧率已重新验完”。
- 当前恢复点：
  - 只要外部 `ItemTooltip` 编译红清掉，下一轮就该直接进 `Town` 复跑你的同一路径，再看 `NPCInformalChatInteractable.Update` 是否明显下沉；
  - 如果还不够，再继续第二刀：把“每个 NPC 自己轮询”进一步收束到玩家侧集中仲裁。
- 当前状态：
  - 这轮已 `Begin-Slice`
  - 收尾已 `Park-Slice`
  - `thread-state = PARKED`
  - blocker：
    - `external compile red: ItemTooltip missing in EnergyBarTooltipWatcher / Inventory UI files; live performance retest blocked until external compile is clean`

## 2026-04-07｜只读补判：Town 对话后 prompt 链第二刀仍可限制在 NPC own 三文件内

- 当前主线：
  - 用户没有让我继续改代码，而是要求只读评估 `Town` 里正式对话结束后，很多 NPC 同时恢复 prompt `Update` 时，NPC own 还剩哪些安全性能刀口。
- 本轮结论：
  1. 这次真正还值得继续砍的热点，仍主要集中在：
     - `NPCInformalChatInteractable.ReportProximityInteraction(...)`
     - `NPCDialogueInteractable.Update()/ReportProximityInteraction(...)`
     - `NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory(...)`
  2. 最安全的大头不是再碰 shared service，而是继续压：
     - informal 链先做远距早退
     - formal takeover 改成轻量 availability 判定
     - 两处 `ReportCandidate` 去 capture closure
  3. 一旦改成玩家侧集中仲裁、共享 proximity collector、或去动 UI/bubble/session manager contract，就开始越界到 shared / UI / day1。
- 当前恢复点：
  - 如果用户后续批准继续真实施工，下一刀仍可保持 `NPC own` 边界；
  - 施工优先级已经收敛为：
    1. coarse distance 早退
    2. suppression 轻量化
    3. callback 去闭包

## 2026-04-07｜Town prompt 链第二刀已继续压下，随后顺手清掉 NPCInformalChatInteractable 头部重复 using 警告

- 当前主线：
  - 继续围绕 `Town` 里正式对话后 NPC prompt 链热点，只做 `NPC own` 的性能压缩与最小止血；
  - 不扩到 shared prompt shell / UI / day1 runtime。
- 这轮新增完成：
  1. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - 新增 coarse distance 早退，远处 NPC 不再先跑整条 session / suppression 链
     - NPC 自身 presentation bounds 改成组件缓存，不再每帧 `GetComponentsInChildren`
     - `InteractionContext` 改为实例复用
     - prompt trigger 改成 cached action，不再每帧 capture lambda
     - 顺手清掉文件头重复 `using System;` 警告
  2. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - formal prompt 链同步加 coarse distance 早退
     - presentation bounds / `InteractionContext` / cached action 同步收口
     - 新增 `HasFormalDialoguePriorityAvailable()`，给 informal suppression 走轻量 formal availability 判断
  3. `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
     - 新增 blocking page UI 的按帧缓存
     - informal suppression 不再借完整 `CanInteract(context)` 走重复 formal 判定
     - 去掉已不再需要的玩家上下文构造链
  4. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - 运行态 live smoke 暴露的 `SendMessage cannot be called during Awake...` 来源已定位
     - 最小补口：把世界空间气泡壳的主动创建从 `Awake()` 挪到 `Start()`
- 本轮验证：
  - `git diff --check` 对 own 文件通过
  - 多次 fresh `errors` 一度回到 `errors=0 warnings=0`
  - `manage_script validate`
    - `NpcInteractionPriorityPolicy.cs` clean
    - `NPCDialogueInteractable.cs` / `NPCInformalChatInteractable.cs` 仅剩通用 GC warning
  - `NPCInformalChatInteractable.cs` 的重复 `using` 警告已清
- 当前没闭掉的部分：
  - `NPCBubblePresenter.cs` 的 `Awake -> Start` 补口还没拿到最终干净 play smoke
  - 原因不是 own compile red，而是现场随后被外部 console 噪音重新污染：
    - `SpringDay1WorkbenchCraftingOverlay.cs` 一度 external red
    - 当前又有 repeated `The referenced script (Unknown) on this Behaviour is missing!`
- 当前恢复点：
  - `NPC` own 代码层第二刀已到安全点
  - 下一轮只要外部 compile / console 现场干净，就直接回 `Town` 做最终 play smoke / 性能复测 / bubble residue 复核

## 2026-04-07｜最终 live 复测尝试被外部 CraftingStation 编译红拦截

- 当前主线：
  - 用户明确授权“Unity 现在可测”，所以我按 `Town` 最终 live 复测切片重新进场；
  - 目标是验证：
    1. `NPCBubblePresenter` 的 `Awake -> Start` 补口是否止住运行时红
    2. `Town` 对话后 NPC prompt 链当前能否进入最终 smoke
- 这轮实际执行：
  1. `Begin-Slice` 已登记
  2. fresh `status / errors` 一度为：
     - `Town.unity`
     - `Edit Mode`
     - `errors=0 warnings=0`
  3. 随后在继续准备 live smoke 时，现场重新刷出新的 external compile red：
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs(328,35): error CS0841`
     - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs(328,54): error CS0841`
     - 含义：`overlay` 在声明前被使用
- 当前判断：
  - 这不是 `NPC` own 红
  - 但它已经足够阻断“这轮继续进 Play 做最终复测”
  - 所以本轮只能诚实停在 `external compile blocker`
- 当前恢复点：
  - `NPC` 自己要测的内容没有被否定，只是还没拿到最终 live 结论
  - 只要 `CraftingStationInteractable.cs` 这两个 external error 清掉，下一步直接回：
    1. 清 console
    2. 进 `Town` Play smoke
    3. 看 `NPCBubblePresenter` residue
    4. 立刻退回 `Edit Mode`

## 2026-04-07｜补记：NPC 气泡“方泡”根因已压到旧 scene 空壳，脚本补口与 EditMode 守卫已落地

- 用户最新追的不是性能，而是 `NPC` 气泡为什么还是方框感。
- 本轮结论：
  1. 根因不是“代码现在就只会画方框”，而是 scene 里旧 `NPCBubbleCanvas` 空壳被绑定后没有回刷 sprite；
  2. `NPCBubblePresenter.cs` 已补成：旧壳重绑后强制刷新 body/tail sprite、`Image.Type`、字体和 layout；
  3. 新增 EditMode 守卫测试，专门验证“旧空壳 -> ShowText -> sprite 被刷回”。
- 本轮验证：
  - 新增旧壳回刷测试 `Passed`
  - 整份 `NpcBubblePresenterEditModeGuardTests` `6/6 Passed`
- 当前边界：
  - 代码层补口已成立；
  - 最终 `Town` GameView 观感仍待人工终验，不包装成已经完全过线。

## 2026-04-07｜Primary/001 续查：方泡问题进一步收窄到缓存残留补口

- 用户继续反馈 `Primary/001` 真入口里 still-square，说明“只修旧空壳初次绑定”还不够。
- 这轮新确认：
  - runtime 生成的 body/tail 贴图本身仍是圆角 body + 三角 tail，不是生成函数自己画成方块；
  - 仍可能漏掉的只剩这层：
    - bubble UI 缓存还在；
    - 但 body/tail sprite 被某次 residue 打回空；
    - 下一次 `ShowText()` 以前因为早退，不会再回刷。
- 已补：
  - `NPCBubblePresenter.cs`
    - 新增 `HasResolvedBubbleUi()`；
    - 已解析 bubble UI 时，每次展示前都强制重刷 body/tail sprite/type；
    - 缓存半残时先 reset 再重绑。
  - `NpcBubblePresenterEditModeGuardTests.cs`
    - 新增 cached sprite null recovery test。
  - `Assets/Editor/NPC/NpcBubblePresenterGuardValidationMenu.cs`
    - 新增命令桥可调用的 bubble guard tests 菜单。
- 已验：
  - `Editor.log` compile success，无本轮 own red；
  - bubble guard tests：`7/7 Passed`
  - `git diff --check`：通过
- 当前状态：
  - `PARKED`
  - blocker 只剩用户回 `Primary/001` 做真实入口复测；
  - 若仍见方泡，下一步优先怀疑“不是 `NPCBubblePresenter` 这条链在出图”，而不是继续原地重复改同一补口。

## 2026-04-08｜补记：Town/002 持续自动冒测试气泡的根因已查实

- 用户在 `Town` 运行时看到 `002` 一直自己冒测试气泡，怀疑已经被当成测试 NPC。
- 这轮按只读口径查实：
  1. 根因不是正常闲聊 / 常驻居民逻辑失控；
  2. `Town.unity` 里的 `002` 场景实例被额外挂了 `NPCBubbleStressTalker`；
  3. 该实例当前还是自动测试态：
     - `startOnEnable: 1`
     - `disableRoamWhileTesting: 1`
     - 带整段 `testLines`
  4. `NPCBubbleStressTalker.cs` 注释本身就写着“仅用于压测 NPC 气泡布局”。
- 判断：
  - 这是“场景实例误留测试组件 / 测试态参数”，不是 NPC 正常业务链整体坏掉；
  - 最小恢复不是去重写聊天系统，而是把 `Town/002` 从这个测试态退出来。
- 建议恢复方式：
  1. 最稳：移除 `Town` 里 `002` 身上的 `NPCBubbleStressTalker`
  2. 最小：只把 `startOnEnable` 改回 `0`
- 本轮边界：
  - 只读分析，无任何 tracked 修改
  - `thread-state` 不变，继续 `PARKED`

## 2026-04-08｜补记：NPC 气泡字体显示发糙的根因已收窄到 Pixel 字体链与材质同步缺口

- 用户贴图反馈 `NPC` 气泡文字显示明显不对。
- 这轮只读结论：
  1. 当前 `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 仍把 `DialogueChinese Pixel SDF` 当成主用字体链；
  2. 全部 NPC prefab 当前也都序列化绑定到 `DialogueChinese Pixel SDF`；
  3. 该字体本体是 `Fusion Pixel 10px Mono zh_hans`，再叠当前 `characterSpacing / negative lineSpacing / outlineWidth 0.18` 的重样式，观感会发挤发糙；
  4. `DialogueChineseFontRuntimeBootstrap` 虽有 `V2` 优先序，但气泡链传入的 preferred font 是 `Pixel` 时，会优先继续吃它；
  5. `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 在换字体时没有同步 `fontSharedMaterial`，比 `DialogueUI / NpcWorldHintBubble` 少了一步材质收口。
- 当前判断：
  - 这不是单个 font asset 丢字，而是：
    - 字体选择不合适
    - 文本样式参数过重
    - 运行时材质同步不完整
    三层叠加。
- 本轮边界：
  - 只读分析
  - 继续 `PARKED`

## 2026-04-08｜补记：NPC 气泡字体修复已做完代码层收口，当前只差 live 观感终验

- 本轮真实施工：
  1. `NPCBubblePresenter.cs`
     - 字体优先序改为非 `Pixel` 主链优先
     - prefab 旧 `Pixel` 绑定不再能锁死 runtime
     - 补齐 `fontSharedMaterial`
     - 文本参数从重描边/重字距回正到轻量组合
  2. `PlayerThoughtBubblePresenter.cs`
     - 同步改成同口径修复
  3. 两份 editor tests
     - 新增字体链 / 材质 / 轻参数 guard
- 本轮验证：
  - `git diff --check` 对 4 个 touched files clean
  - `Editor.log` 最新 compile 成功：
    - `Tundra build success (4.30 seconds)`
  - 未见本轮 own `error CS`
  - 当前 external 仍只见旧 warning，不是本轮 blocker
- 当前判断：
  - 这刀已经把“为什么会丑”的三层根因都落到了代码补口上；
  - 当前不再是“结构没修”，而是只剩用户回现场看观感是否达到你要的正式面。

## 2026-04-08｜NPC resident 接管 contract 已交付到 day1 / 存档协作面

- `NPC` 线程按 `2026-04-08_给NPC_day1原生resident接管与持久态协作prompt_17.md` 完成了一刀新的 owner contract 收口。
- 这轮真实新增：
  1. `NPCAutoRoamController` resident scripted control contract
  2. `NpcResidentRuntimeSnapshot` 最小 DTO
  3. `NpcResidentRuntimeContract` scene helper
  4. informal / nearby / active session 对 scripted control 的 gating
  5. `SpringDay1DirectorStaging` 现有 takeover 入口接入 acquire/release
  6. 对应 edit-mode tests
- 当前对协作面的含义：
  1. `day1` 已经可以消费 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl`
  2. `存档系统` 已经可以消费 `NpcResidentRuntimeSnapshot + NpcResidentRuntimeContract`
  3. `NPC` 没有回吞 `runtime spawn / Town/Primary scene writer / CrowdDirector` 主消费逻辑
- 本轮验证口径：
  1. fresh console：`errors=0 warnings=0`
  2. `validate_script`：`unity_validation_pending`，`owned_errors=0`
  3. `compile`：两次都被 `dotnet/codeguard timeout` 卡住，没拿到 compile-pass 小票
  4. `git diff --check`：本轮 own/carried 文件 clean
- 产物：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_NPC给day1_原生resident接管与持久态协作回执_18.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
     - 已追加 resident snapshot 边界补充
- 当前线程状态：
  - `PARKED`
  - 无 blocker

## 2026-04-09｜只读核查：剧情对白 / 关系页 / 场景 NPC 目前不是同源一套

- 当前主线目标：
  - 只读彻查“剧情 NPC、关系页 NPC、场景 NPC”到底是不是同一套数据链，并把人物 id / 名字 / 简介 / 头像来源的真实状态钉死。
- 本轮子任务：
  1. 核对白话剧情头像来源
  2. 核对背包关系页人物来源
  3. 核对场景 prefab / NPC content asset 的 id 真值
  4. 判断当前到底是“接线没接完”还是“结构上分裂”
- 本轮结论：
  1. 场景与 NPC 本体层目前有一套明确 numeric id：
     - `001/002/003`
     - `101/102/103/104/201/202/203/301`
     - prefab 位于 `Assets/222_Prefabs/NPC/`
     - 对应 content asset 位于 `Assets/111_Data/NPC/` 与 `Assets/111_Data/NPC/SpringDay1Crowd/`
  2. 关系页 `PackageNpcRelationshipPanel` 不吃全量 NPC，只吃 `Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
     - 当前 manifest 只有：
       - `101/102/103/104/201/202/203`
     - 不包含：
       - `001/002/003`
       - `301`
     - 所以关系页现在连群像都不是全量，更不是全项目 NPC 总表
  3. 关系页简介与显示名来自 manifest：
     - `displayName`
     - `roleSummary`
     - 头像不是 `Assets/Sprites/NPC_Hand`，也不是 `Assets/Sprites/NPC/*.png`
     - 当前是直接从 prefab 上抓最大 `SpriteRenderer.sprite`
  4. 正式剧情对白不是按 `npcId` 绑定：
     - `DialogueNode` 只有 `speakerName + speakerPortrait`
     - `speakerPortrait` 目前大多为空
     - `DialogueUI` 空头像时只兜底到 `Assets/Sprites/NPC/001.png`
     - 不会按 `001/002/003/...` 自动找头像
  5. `Assets/Sprites/NPC_Hand` 当前只有：
     - `001.png`
     - `002.png`
     - 这套资源现在没有被剧情对白自动消费
  6. formal 剧情里出现的说话人标签，和 numeric id 没有硬映射层：
     - 已确认出现：`村长 / 艾拉 / 卡尔 / 旅人 / 村民 / 小孩 / 小米 / 饭馆村民`
     - 其中 `村长 -> 001`、`艾拉 -> 002` 有强语义证据
     - `卡尔 / 小米 / 村民 / 小孩 / 饭馆村民` 目前未见统一的 `npcId` 绑定层
- 当前判断：
  - 现在不是“一套数据只差最后接线”，而是至少 3 条链并存：
    1. 场景 prefab + NPC 内容资产的 `npcId` 实体链
    2. 关系页专用的 manifest 子集链
    3. 正式剧情对白的 `speakerName / speakerPortrait` 文本链
  - 若后续要做到“同源且彻底同步”，正确目标应是统一到 `npcId` 真值表，而不是继续让 UI / 剧情 / prefab 各自维护名字和头像
- 关键证据：
  - `Assets/YYY_Scripts/Story/Data/DialogueNode.cs`
  - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
  - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
  - `Assets/111_Data/NPC/NPC_001_VillageChiefDialogueContent.asset`
  - `Assets/111_Data/NPC/NPC_002_VillageDaughterDialogueContent.asset`
  - `Assets/111_Data/NPC/NPC_003_ResearchDialogueContent.asset`
  - `Assets/111_Data/NPC/SpringDay1Crowd/NPC_301_GraveWardenBoneDialogueContent.asset`
- thread-state：
  - 本轮全程只读
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 原因：没有进入真实施工
- 当前恢复点：
  - 后续如果要真收口人物统一源，第一刀不该乱改 UI 壳或剧情文案，而该先出一张统一人物真值表，并明确：
    - 谁有 `npcId`
    - 谁进关系页
    - 谁进正式剧情
    - 谁的头像库是唯一源

## 2026-04-09｜统一人物主表第一刀落地：剧情头像、关系页身份、NPC 内容资产开始回到同一条真值链

- 当前主线目标：
  - 把此前已经查明的“三套并存”结构，先压成一份统一人物主表，而不是继续靠 `speakerName / manifest / prefab` 各自维护真值。
- 本轮完成：
  1. 新增统一主表脚本：
     - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
  2. 新增统一主表资产：
     - `Assets/Resources/Story/NpcCharacterRegistry.asset`
  3. 把正式对白头像 fallback 接回主表：
     - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
  4. 把关系页的人物身份真值切回主表，manifest 只留 Day1 在场语义：
     - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  5. 新增统一护栏测试：
     - `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
- 当前主表已经覆盖的统一信息：
  - `001/002/003/101/102/103/104/201/202/203/301`
  - 统一字段包括：
    - `npcId`
    - 名字
    - 关系页显示名
    - 角色简介
    - formal speaker alias
    - prefab
    - 手绘头像
    - 是否进关系页
    - 少量 beat 级 resident 语义
- 当前真正被接回主表的消费口：
  1. `DialogueUI`
     - formal 对白会先按 `speakerName -> 主表` 找 portrait
  2. `PackageNpcRelationshipPanel`
     - 关系页的“这个人是谁、叫什么、简介是什么、头像拿谁”改由主表给
  3. editor 护栏
     - authored dialogue / NPCDialogueContentProfile / crowd manifest 三边都必须能回到主表
- 本轮验证：
  - 两份脚本级 `validate_script` 都是：
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
  - 当前没有 own compile red；
  - 但还没拿到最终 Unity live 终验。
- 当前关键判断：
  - 这轮不是“彻底全收完”，但已经从“结构上分裂”推进到“有统一真值表，并且至少 2 个真实消费口已经吃回去”。
  - 后续最优先不是再扩题，而是先补 Unity 现场终验，确认：
    1. 主表资产引用没丢
    2. 关系页实际能看到 `001/002/003/301`
    3. formal 对白人物头像不再统一回默认 `001`
- thread-state：
  - `Begin-Slice`：已跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - 当前 blocker：
    - `live终验未补`
    - `PackageNpcRelationshipPanel.cs` 与 `UI` 线程同路径重叠，后续不再继续叠改

## 2026-04-09 11:15 NPC_Hand 全局头像统一进展
- 这轮不是只读讨论，已经先落了头像统一第一刀：
  - `DialogueUI`：对白头像优先吃主表里的 `handPortrait`
  - `NpcCharacterRegistry`：关系页头像优先级回正为 `handPortrait > prefab 默认帧`
  - `PackageNpcRelationshipPanel`：头像框新增 `RectMask2D`，并把边距收紧到更贴边但不出框
  - `NpcCharacterRegistry.asset`：补挂 `003/103` 手绘头像
  - `NpcCharacterRegistryTests`：新增“有 handPortrait 就必须双端同源”的护栏
- 当前真实接上的手绘头像 roster：
  - `001/002/003/103`
- 当前新的更优判断：
  - 用户提出“不要继续手工挂图，而是做 `NPC_Hand` 真源路径查询 + 字典缓存”是对的；
  - 下一刀应改成：
    1. 先按 NPC ID 从 `NPC_Hand` 找手绘头像
    2. 找不到再回退 prefab 默认动画帧
    3. 运行时只吃一次性字典，不做反复搜索
- 当前验证状态：
  - 代码层：`owned_errors=0`
  - Unity live：`pending`，因为当前 CLI/MCP 没拿到活动实例
- 当前 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑
  - 当前应在回合结束时补 `Park-Slice`

## 2026-04-09 12:53 NPC_Hand 真源字典已落地
- 这轮已经把“手工挂头像”升级成打包态安全链路：
  1. editor 侧：
     - 新增 `Assets/Editor/NPC/NpcCharacterRegistryHandPortraitAutoSync.cs`
     - `Assets/Sprites/NPC_Hand` 成为 hand 头像真源目录
     - 新增/删除/移动头像会自动写回 `NpcCharacterRegistry.asset`
  2. runtime 侧：
     - `NpcCharacterRegistry.cs` 新增字典缓存
     - 运行时不再线性扫整表，也不会去扫 `Assets/`
     - 统一规则为：`NPC_Hand handPortrait -> prefab 默认帧`
  3. 测试侧：
     - `NpcCharacterRegistryTests.cs` 改成直接按 `NPC_Hand` 当前目录真实头像 roster 验证，不再写死旧名单
- 当前已确认自动同步进主表的 hand 头像：
  - `001 / 002 / 003 / 101 / 103 / 104`
- 当前验证：
  - `validate_script` 对 runtime / editor / tests 三个关键脚本均为：
    - `owned_errors=0`
    - `manage_script validate = clean`
    - `assessment=unity_validation_pending`
  - Unity console 当前没有 own red
  - 当前唯一可见 error 为外部编辑器噪音：
    - `GridEditorUtility.cs: Screen position out of view frustum`
- 当前阶段：
  - 结构与代码合同已站住；
  - 下一步最值钱的事不再是写新逻辑，而是补 formal 对白与关系页的 live 终验。

## 2026-04-09 18:15 剧情玩家/旁白头像已切到 000
- 这轮做的是紧急剧情头像修复，不是继续做 NPC 简介。
- 已完成：
  1. `DialogueUI`
     - 玩家台词 `旅人 / 陌生旅人` 以及 inner monologue / 旁白分支都改为优先回 `000` 号头像条目
  2. `NpcCharacterRegistry.asset`
     - 新增 `000 = 旅人` 特殊条目
     - 头像直接绑定 `Assets/Sprites/NPC_Hand/000.png`
     - 不进入关系页
  3. `NpcCharacterRegistryTests`
     - `旅人 / 陌生旅人` 现在必须解析到 `000`
- 当前验证：
  - `validate_script DialogueUI.cs`
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `validate_script NpcCharacterRegistryTests.cs`
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `errors`
    - `0 error / 0 warning`
- 当前结论：
  - 这刀逻辑上已经把“玩家/旁白头像还是空的或回默认 NPC 图”的问题压掉了；
  - 还差最后一眼 live 终验。
