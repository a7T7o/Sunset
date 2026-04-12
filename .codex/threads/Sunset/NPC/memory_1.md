# NPC 线程记忆

## 2026-03-16

- 当前主线目标：完成 Sunset 项目的 NPC 固定模板生成器与第二阶段自动漫游/气泡行为闭环。
- 本轮子任务：根据用户贴出的控制台报错，先确认实时阻断，再修复当前现场并把 NPC 第二阶段恢复到可继续开发的状态。
- 本轮真实现场：
  - 工作目录：`D:\Unity\Unity_learning\Sunset`
  - 当前分支：`codex/farm-1.0.2-correct001`
  - 起手时发现 `.kiro/specs/NPC/` 与 `.codex/threads/Sunset/NPC/` 在当前现场缺失。
  - 当前现场真实编译阻断不是 Animator，而是 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs(2512,34)` 调用 `ClearAllQueuePreviews(bool)` 与 `FarmToolPreview` 版本不匹配。
- 本轮完成：
  - 从 `codex/npc-roam-phase2-001` 精确回填 `FarmToolPreview` 兼容版本，以及 NPC 第二阶段代码与资产。
  - 从 `codex/npc-fixed-template-main` 回填并继续修正 `Assets/Editor/NPCPrefabGeneratorTool.cs`，把 `TextureImporter.spritesheet` 改成 `ISpriteEditorDataProvider`。
  - 新建并恢复 `NPC` 工作区文档链路：父 memory、子 memory、tasks、npc规划001、线程 memory。
- 本轮验证：
  - Unity Console 当前无 NPC 项目级红错。
  - `Animator is not playing an AnimatorController` 本轮未复现。
  - `Primary` 场景进入 Play 后，`001` / `002` / `003` 的 `NPCAutoRoamController` 均可读到 `IsRoaming=true`。
  - 再次抓到附近聊天正样本：
    - `001.LastAmbientDecision = "chatting with 002"`
    - `002.LastAmbientDecision = "joined chat with 001"`
- 当前恢复点：NPC 第二阶段已经恢复到可继续开发状态；若继续推进，优先沿 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\npc规划001.md` 迭代新的行为需求。

## 2026-03-17

- 当前主线目标：修复 NPC 预制体与动画的 Sprite 丢失问题，查清是 Prefab 坏了还是资源引用链断了。
- 本轮子任务：对照 Prefab、动画 clip 与 `Assets/Sprites/NPC/*.meta` 的子资源 ID，定位用户看到的 `缺失(精灵)`。
- 本轮结论：
  - 不是 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 自身结构损坏。
  - 真正损坏点是上轮恢复时漏带了 `Assets/Sprites/NPC/001.png.meta`、`002.png.meta`、`003.png.meta`。
  - Prefab 与动画都在引用方向化切片 ID，例如 `001_Down_1 -> fileID 3869598651494229176`，而修复前现场的 `.meta` 仍是旧的 `001_0/001_1` 自动切片版，所以两边一起失配。
- 本轮修复：
  - 从 `codex/npc-roam-phase2-001` 恢复三张 NPC PNG 的 `.meta` 文件。
  - 重新刷新 Unity。
- 本轮验证：
  - `001` 的场景实例 `SpriteRenderer.sprite` 已恢复非空。
  - `001_Idle_Down.anim` / `001_Move_Down.anim` 的 Sprite 曲线引用已和 `.meta` 对齐。
  - 当前 Unity Console 为 0 条。
- 当前恢复点：NPC 的 Sprite 资源链路已补齐，可以继续让用户在 Unity 里复测 Prefab 和动画窗口。

- 当前主线目标：在继续 NPC 主线开发前，先澄清这条线程为什么会出现“嘴上是 NPC，Git 现场却在 farm”的分支/工作树漂移。
- 本轮子任务：仅做 Git 与规则取证，不改代码、不改场景，给治理线程准备可直接引用的写实结论。
- 本轮已证实：
  - 当前根仓库真实现场是 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`，`HEAD = 11e0b7b4`。
  - 当前 repo 仍保留旧 NPC worktree：`D:\Unity\Unity_learning\Sunset_worktrees\NPC @ codex/npc-generator-pipeline`。
  - 分支不是 worktree 私有对象；`main` 与多条 `codex/npc-*` 分支都在仓库里正常存在，旧 NPC worktree 只是其中一条历史分支的检出位置。
  - `git reflog` 说明 2026-03-17 01:35 曾从 `codex/farm-1.0.2-correct001` 切到 `codex/npc-roam-phase2-001`，01:44 又切回 `codex/farm-1.0.2-correct001`；后续 `07ffe199`、`18f3a9e1`、`11e0b7b4` 三个提交都落在 farm 分支。
  - 其中 `07ffe199`、`18f3a9e1` 实际承载了 NPC 二阶段恢复与 PNG meta 修复内容，当前只被 `codex/farm-1.0.2-correct001` 包含。
- 本轮反思：
  - 我前面的表述把“历史上有 NPC 独立 worktree”说得过于像“现在 NPC 就等于那个 worktree”，这是不准确的。
  - 更准确的写法应是：规则层已经把 NPC 的默认入口改回根仓库 `main`，但 Git 现场同时残留旧 NPC worktree，而本轮根仓库又实际停在 farm 分支上；我后来回到当前根分支继续提交，才导致 NPC 成果挂到了 farm。
- 当前恢复点：现在已经能够把“规则口径”和“Git 现场”拆开说明，下一步应交由治理线程裁定 NPC 成果与旧 worktree 的收口方式。

- 当前主线目标：按治理裁定把 NPC 的唯一救援基线锁定到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再借用共享根目录的 farm 事故现场继续推进 NPC。
- 本轮子任务：只读确认 `f6b4db2f` 是否已经包含 NPC 核心资产 / Prefab / meta / 运行时代码，并给出最小收口建议。
- 本轮已证实：
  - `f6b4db2f` 已经包含 NPC 所需的动画、控制器、默认 roam profile、001~003 Prefab、001~003 PNG 与 `.meta`、`NPCAnimController`、`NPCMotionController`、`NPCAutoRoamController`、`NPCBubblePresenter`、`NPCRoamProfile`、`NPCPrefabGeneratorTool`。
  - `f6b4db2f` 对比 farm 的 `18f3a9e1`，NPC 范围内只剩 `Assets/Editor/NPCPrefabGeneratorTool.cs` 与 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 两个差异；说明这条救援线已经不缺 NPC 核心文件。
  - `8aed637f -> f6b4db2f` 的对比表明，真正应从 NPC 线剔除的 farm 侧拖带改动是 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - `NPCPrefabGeneratorTool.cs` 保持 `f6b4db2f` 版本即可作为救援线版本；下一步最小验证应是“回退 `FarmToolPreview.cs` 后，做 Unity 编译 + `001/002/003` Prefab/动画引用检查”。
- 当前恢复点：NPC 已经有了明确的唯一救援基线和最小收口边界，后续只需在独立 NPC 可写现场里执行剔除与验证，不需要再从 farm 搬回更多 NPC 核心文件。
- 2026-03-18：按 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt\NPC.md` 领取阶段一只读唤醒 prompt，并完成只读 preflight。
- 本轮 live 现场：`D:\Unity\Unity_learning\Sunset @ main @ 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`，`git status --short --branch = ## main...origin/main`，shared root 仍是 `main + neutral`。
- 只读确认 continuation branch 当前仍为 `codex/npc-roam-phase2-003 @ 7385d1236d0b85c191caff5c5c19b08678d1cf80`。
- 已回写线程回收单：[NPC.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/22_恢复开发分发与回收/线程回收/NPC.md)；结论是阶段一通过，但因 `branch_grant_state = none` 且 Unity / MCP 仍需 live verify，本轮不能直接进入阶段二写入。

## 2026-03-22

- 当前主线目标：只在 `main` 上收口 NPC 两项返工，不扩写新系统。
- 用户已明确纠正：`main` 是唯一开发现场，`codex/npc-roam-phase2-003` 现在只作为只读参考源。
- 本轮真实现场：
  - 工作目录：`D:\Unity\Unity_learning\Sunset`
  - 当前分支：`main`
  - 当前 `HEAD`：`c6af2657`
  - shared root 占用文档当前仍显示 `main + neutral`
- 本轮实际认领的业务文件：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - `Assets/222_Prefabs/NPC/001.prefab`
  - `Assets/222_Prefabs/NPC/002.prefab`
  - `Assets/222_Prefabs/NPC/003.prefab`
- 本轮完成：
  - 气泡样式与锚点提升到新版，目标是从“压脸”改成“真正离开头顶”。
  - NPC 漫游位移改成优先使用 `Rigidbody2D`，为实体碰撞让路。
  - 生成器与三个样本 prefab 都改成下半身 `BoxCollider2D + Rigidbody2D`，不再使用整身 trigger。
- 本轮验证：
  - `git diff --check` 对上述 6 个 NPC 文件通过。
  - 只读回看 prefab YAML，确认三个样本都已是 `BoxCollider2D + Rigidbody2D`、`m_IsTrigger: 0`、`styleVersion: 5`。
  - Unity / MCP 本轮未形成可靠 live 验证，`recompile_scripts` 返回连接失败，因此不能冒充“Unity 已验收通过”。
- 当前恢复点：NPC 两项核心返工已经真实落到 `main`，后续只需要围绕主项目现场验证这两项效果是否达标，再决定是否对白名单 NPC 文件做 Git 收口。

## 2026-03-22 导航缺陷与下一轮需求澄清

- 当前主线目标：只读核查 NPC 当前气泡、碰撞与导航能力边界，给用户一份可直接拿去申请修正的写实清单。
- 本轮真实现场：`D:\Unity\Unity_learning\Sunset @ main @ c6af2657`。
- 本轮已证实：
  - NPC 漫游确实调用 `NavGrid2D.TryFindPath()`，不是完全没接导航。
  - 但 NPC 当前没有任何局部避障逻辑，只会沿静态路径前进；撞住后靠 stuck 检测重建路径。
  - 当前 NavGrid2D 是静态网格，不跟踪玩家/NPC 的实时占位；`Primary.unity` 的 `obstacleTags` 也不含 NPC，而 `Assets/222_Prefabs/NPC/001.prefab` 当前 tag 为 `Untagged`。
  - 玩家自动导航存在局部碰撞偏转，因此“玩家路上偶尔会侧一下”是有代码依据的；但它仍不是动态让行系统，不能保证面对移动 NPC 时稳定绕过。
  - 玩家与 NPC 当前都为 `Dynamic Rigidbody2D + mass 1`，而玩家移动速度更高，所以“玩家把 NPC 顶开太容易”是现配置下的真实问题。
  - 当前气泡已经有轻微浮动，但浮的是整颗气泡，不是箭头单独浮；箭头方向本身朝下，但体量偏小、指向性不够强。
- 用户刚刚新增并确认的需求：
  - 箭头要更明显朝下指向 NPC，并带轻微悬浮感。
  - 气泡和文字都要再大一点，达到清晰可读。
  - NPC 需要更难被玩家顶开。
  - 后续希望玩家导航遇到路过 NPC 能自动规避，NPC 与 NPC 也能互相规避和路口礼让。
- 需要申请修正的系统缺口：
  - NPC 局部避障缺失
  - 动态障碍占用与实时重规划缺失
  - 双向让行 / 会车 / 路口优先级缺失
  - 玩家/NPC 物理权重与接触策略设计缺失
  - 气泡视觉参数仍未达到可读性验收标准
- 当前恢复点：现在已经能明确区分“当前已有静态 A* 调路能力”和“当前没有动态人群导航能力”，后续返工可以避免再把两个问题混成一团。

## 2026-03-22 输出口径纠正：职责分离与导航交接

- 用户本轮纠正成立：我上一轮没有把 NPC 线程自己的问题和导航线程的问题分开，输出偏题。
- 正确口径：
  - 我只认领 NPC 自己的代码：`NPCBubblePresenter.cs`、`NPCPrefabGeneratorTool.cs`、`001~003.prefab`、`NPCMotionController.cs`、`NPCAutoRoamController.cs` 中属于 NPC 消费导航结果的部分。
  - 我不认领导航系统核心增强：`NavGrid2D.cs`、`PlayerAutoNavigator.cs` 及动态占位/让行/避障框架，应明确移交导航线程。
- 用户视觉需求纠正：
  - 当前尾巴从最终观感看是正三角朝上，不符合需求。
  - 正确需求是倒三角朝下指向 NPC，并做尾巴单独的轻微上下跳动。
  - 气泡和文字都要适度放大，提高可读性。
- 给导航线程的交接要点：
  - 当前 NPC 已调用静态 A*，问题不在“有没有调导航”，而在“导航系统没有动态避让能力”。
  - 需要导航线程解决玩家绕移动 NPC、NPC/NPC 互相规避、路口礼让、动态占位和 agent 分型。
- 当前恢复点：现在已经有了一份正确的职责分离口径，后续汇报与实现都必须按这个边界执行。

## 2026-03-22 用户强纠正后的回炉重写

- 当前主线目标：把 NPC 线程的责任边界重新写清楚，避免再把 NPC 自身返工与导航系统增强混为一谈。
- 本轮子任务：只读复核 `main @ c6af2657` 下 NPC / 导航代码证据，重写给用户和导航线程的正式口径。
- 本轮确认：
  - 上一轮边界混乱是我自己的问题，现已纠正为“NPC 只认 NPC 自己的代码，导航只认导航核心代码”。
  - `NPCBubblePresenter.cs` 当前尾巴生成与布局仍会呈现更像“正三角朝上”的观感；用户要的是“倒三角朝下指向 NPC”，且尾巴要单独轻微跳动。
  - `NPCAutoRoamController.cs` 已接静态 A*，但动态避让不是 NPC 线程该补的核心代码。
  - 给导航线程的核心需求已锁定为：动态障碍占位、玩家绕移动 NPC、NPC/NPC 互相规避、路口礼让、agent 分型。
- 当前恢复点：下一轮继续开发时，NPC 线程只做气泡视觉和 NPC 侧 collider / movement-consumer 返工；导航增强改走跨线程交接。

## 2026-03-22 双Prompt交付

- 当前主线目标：把这条 NPC 线和导航线之间的协作方式彻底写成可执行 prompt。
- 本轮子任务：起草“导航线程 prompt”与“NPC 线程自压 prompt”两份成稿。
- 本轮完成：
  - 已按 `main @ c6af2657` 的 live 现场准备两份 prompt，均会锁定代码归属、禁止越界范围、验证要求和回执格式。
  - 这次不做业务代码变更，只沉淀协作执行文案。
- 当前恢复点：后续直接按 prompt 派工即可，减少再次误解职责边界的风险。

## 2026-03-23 NPC第二刀继续开发 checkpoint

- 当前主线目标：在 `main-only` 新口径下继续推进 NPC 自己的第二刀返工，不再停留在“上一刀已经收口”的观察态。
- 本轮子任务：只在 NPC 自己的代码范围内继续修气泡与碰撞体感。
- 本轮完成：
  - `NPCBubblePresenter.cs`：把尾巴纹理从“视觉更像正三角朝上”改成“倒三角朝下”，并新增尾巴单独轻微跳动；同时继续放大气泡、字体和头顶留白。
  - `NPCPrefabGeneratorTool.cs`：提高新生成 NPC 的刚体质量和阻尼，并开启插值。
  - `001/002/003.prefab`：同步成第二刀参数真相。

## 2026-04-09｜只读分析：NPC FixedUpdate 热链最安全的缓存/节流建议

- 当前主线目标：
  - 不改代码，只读分析 `NPCAutoRoamController / NavGrid2D / NavigationTraversalCore`，找出最安全的性能修复切口。
- 本轮子任务：
  - 回答“哪些地方适合做同帧缓存/短窗口节流，既显著减轻 `FixedUpdate` 热链，又最不容易伤 NPC 业务语义”。
- 本轮已完成：
  - 复核 `TickMoving()`、`TryHandleSharedAvoidance()`、`TryRebuildPath()`、`TryBeginMove()`、`CanOccupyNavigationPoint()`、`TryFindNearestWalkable()`、`TryFindPath()` 的调用链。
  - 确认当前代码已经有可沿用的安全前例：
    - `CanReuseHeavyMoveDecisionThisFrame()` 证明同帧决策复用在 NPC 侧是可接受的。
    - `TryAcquirePathBuildBudget()`、`ShouldSkipRebuildPathRequest()`、失败退避证明 path rebuild 已接受短窗口去重思路。
  - 收敛出 5 个建议刀口：
    1. `NavigationTraversalCore.CanOccupyNavigationPoint()` + `NavGrid2D.IsWalkable()/IsPointBlocked()` 的同帧 probe 缓存。
    2. `NavGrid2D.TryFindNearestWalkable()` 的 `50~100ms` 短窗口缓存，`RefreshGrid/RebuildGrid` 失效。
    3. `NavGrid2D.TryFindPath()` 的小型路径缓存，键为 `startCell + endCell + gridVersion`。
    4. `NPCAutoRoamController.TryRebuildPath()/TryBeginMove()` 顺着现有 budget 做同阻塞窗口合并。
    5. `NPCAutoRoamController.TryHandleSharedAvoidance()` 仅在前几刀不足时，再补“只限同帧”的 solver 结果缓存。
- 关键判断：
  - 最安全的修法不是“让 NPC 更慢想”，而是优先合并同一帧/同一阻塞窗口里的重复查询。
  - 我最不建议先动的是跨帧缓存动态避让结果，或粗暴放大 avoidance/repath 冷却；那更容易伤语义。
- 验证结果：
  - 本轮只读，无代码、无编译、无 Unity live。
- 当前恢复点：
  - 如果后续真的要做性能刀，优先顺序应是 `occupancy probe -> nearest walkable -> path cache -> rebuild 合并`，最后才考虑 avoidance solver。
- 本轮验证：
  - `git diff --check` 通过。
  - MCP 重编译尝试失败，当前报 `Connection failed: Unknown error`；因此本轮还没有 Unity live 编译证据。
- 当前恢复点：这刀已经形成新的 NPC 本地 checkpoint，可以继续走 live 目测验收，或者直接按白名单提交到 `main`。

## 2026-03-23 NPC第三刀继续开发：003持续说话样本落地

- 当前主线目标：把 `003` 变成真正可用的气泡压测 NPC，并把气泡第三刀做成能 live 证明的版本。
- 本轮子任务：追加测试脚本、更新 prefab、用 `unityMCP` 拿到场景内运行证据。
- 本轮完成：
  - 新增 `NPCBubbleStressTalker.cs` 并挂到 `003.prefab`。
  - `003` 测试模式下会持续随机输出短中长不同长度的话术，最长约 50~60 字。
  - `NPCBubblePresenter` 第三刀加入更偏审美的自适应尺寸策略，并把整体高度往下压了一点。
- 本轮验证：
  - `unityMCP` live 读回确认 `003` 的测试器在场景里真实工作，`ShowCount` 增长且 `LastShowSucceeded = true`。
  - live 读回确认 `NPCBubblePresenter.IsBubbleVisible = true`，长句文本已进入气泡。
  - 本轮结束前已退回 Edit Mode，没有把 Unity 留在 Play。
- 当前恢复点：下一步只差把这轮提交到 `main`，然后等用户按主项目真实观感做验收。

## 2026-03-23 NPC第四刀继续开发：从临时压测到正式配置入口

- 当前主线目标：不再只修气泡外观，而是把 NPC 后续场景化落地所需的测试入口和配置入口一起搭好。
- 本轮子任务：完成内边距微调、`003` 测试模式正规化、漫游配置编辑器增强。
- 本轮完成：
  - `NPCBubblePresenter` 完成文字内边距收口。
  - `NPCBubbleStressTalker` 从临时压测脚本提升为正式测试组件，配套了专用 Editor。
  - `NPCAutoRoamControllerEditor` 已具备场景化配置入口能力，不再只是默认 Inspector。
- 本轮验证：
  - 静态检查通过。
  - 本轮前段拿到过 `unityMCP` live 读回 `003` 测试组件的证据；收尾阶段 MCP 传输层转为 `8080` 握手失败，故最终 live 闭环待补。
- 当前恢复点：下一步可以直接继续做 NPC 活动区域、路线、profile 的场景化收口，而不是再造新的临时测试手段。

## 2026-03-23 NPC第五刀继续开发：进入场景化与集成阶段

- 当前主线目标：在用户认可大部分外观与碰撞结果后，把 NPC 线切入真正的 scene integration 阶段。
- 本轮子任务：完成气泡内边距收口、003 测试模式正规化、配置入口增强。
- 本轮完成：
  - `NPCBubblePresenter` 的文字内边距问题已再次收口。
  - `NPCBubbleStressTalker` 和它的 Editor 已经形成正式测试入口。
  - `NPCAutoRoamControllerEditor` 现在可以支撑 Home Anchor / Profile / 活动范围的后续场景化配置。
- 本轮验证：
  - 本地静态检查通过。
  - `unityMCP` 当前握手失败，因此这轮收尾时没有追加新的 live 终验结论。
- 当前恢复点：NPC 线下一步就可以继续做真正的场景化配置，而不是再做临时测试补丁。

## 2026-03-23 NPC_04 推送阻塞补记

- 当前状态：`2026.03.23_NPC_04 @ 27fda5c5` 已在本地 `main` 提交完成，但推送远端失败。
- 失败原因：当前 Git 远端访问被代理 `127.0.0.1:7897` 阻断；先是 LFS locks verify 报错，关闭该仓库 `locksverify` 后，普通 `git push origin main` 仍然因代理不可连接失败。
- 结论：这不是 NPC 代码冲突，也不是白名单问题，而是当前机器网络/代理层阻塞远端推送。
- 恢复点：本地 `main` 已包含本轮 NPC 修改与导航回执所需结论；下一步若要远端同步，只需在代理恢复后重跑 `git push origin main`。

## 2026-03-23 给导航线程的正式回执已落盘

- 当前主线目标：把 NPC 线程对导航线程的正式口径沉淀成可直接转交的回执文档。
- 本轮子任务：围绕运动语义、状态语义、prefab 基线、测试入口定位与后续联调边界，整理一份专业回执。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\给导航线程的正式回执_2026-03-23.md`
  - 文档已明确写清：
    - `Moving / ShortPause / LongPause` 的当前稳定语义
    - `IsMoving` 与最终位移方式 `rb.MovePosition(...)` 的当前稳定承诺
    - `001 / 002 / 003` 当前 `Rigidbody2D + BoxCollider2D + roam` 基线
    - `NPCBubbleStressTalker` 是长期保留测试入口，但不是正式生产行为组件
    - 后续若 NPC 线要动 `NPCAutoRoamController` / `NPCMotionController`，会先同步“是否动 IsMoving / pause 状态 / 最终位移方式”
- 当前恢复点：导航线程现在已经有一份可直接使用的 NPC 正式回执，后续联调不需要再靠聊天里口头摘录。

## 2026-03-23 NPC第六刀：更激进的气泡可见改动

- 当前主线目标：把气泡改到“肉眼一眼就看得出变化”的程度，而不是继续做保守微调。
- 本轮子任务：继续下压箭头与气泡整体位置，显著加大内边距，限制长文本横向扩张，并提高 `003` 的持续说话频率。
- 本轮完成：
  - `NPCBubblePresenter`：将 `bubbleLocalOffset` 下压到 `1.58`，`tailYOffset` 下压到 `-28`，`tailSize` 扩到 `34x24`，并把尾巴往返改成“低位到高位”的明显运动，而不是围绕中点的小抖动。
  - `NPCBubblePresenter`：将 `bubblePadding` 提到 `52x38`、`textSafePadding` 提到 `28x24`，并将 `maxTextWidth` 收到 `220`、`minAdaptiveTextWidth` 收到 `144`，让长文本更倾向于长高，而不是横向摊开。
  - `NPCBubbleStressTalker`：将 `003` 的持续发话间隔压到 `0.05~0.18s`，让它更接近“持续测试 NPC”而不是偶尔说一句。
  - `001/002/003.prefab`：同步到这套更激进的气泡参数。
- 本轮对导航线程的影响说明：
  - 本轮没有改 `NPCAutoRoamController` 的状态语义。
  - 本轮没有改 `NPCMotionController` / `NPCAutoRoamController` 的最终位移语义。
  - 本轮只改气泡表现层与测试频率，不改变 NPC 运动基线。
- 当前恢复点：下一步应优先做一次新的用户目测验收，看箭头是否终于足够低、内边距是否终于足够明显。

## 2026-03-23 NPC第七刀：更明显的气泡参数收口

- 当前主线目标：把气泡改动从“细微”提升到“肉眼一眼能看出变化”的程度。
- 本轮子任务：在用户已确认认知一致后，直接加大箭头下压、放大字体、收窄长文本宽度、重调内边距，并降低 `003` 的持续说话速度。
- 本轮完成：
  - `NPCBubblePresenter`：`bubbleLocalOffset` 下压到 `1.46`；`tailYOffset` 保持大幅下压 `-28`；`textSafePadding` 调整为 `{22,18}`；`textVerticalOffset = -6`；`bubblePadding = {48,34}`；`maxTextWidth = 224`；`fontSize = 26`；`bubbleGapAboveRenderer = 0.02`；`tailBobFrequency = 0.85`，让箭头更低、更慢、更明显。
  - `NPCBubbleStressTalker`：将 `003` 的持续说话间隔调慢到 `0.75~1.35s`，不再像机关枪一样刷屏。
  - `001/002/003.prefab`：同步到这套更新后的更激进参数。
- 本轮验证：
  - `git diff --check` 对本轮 NPC 相关脚本与 prefab 通过。
  - 当前 `unityMCP` 仍然卡在 `旧 MCP 端口口径（已失效）` 握手失败，因此本轮 live 终验未取得。
- 当前恢复点：用户现在可以直接在主项目里看这套“明显变化版”气泡；若方向正确，下一步只剩微收审美，不再需要大幅改参数。

## 2026-03-23 MCP 口径纠偏
- 本文件中若出现“旧 MCP 端口口径（已失效）”或“旧 MCP 桥口径（已失效）”，均视为历史阶段事实，不再作为当前 live 口径使用。
- 当前唯一有效 live 基线以 D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md 为准：unityMCP + http://127.0.0.1:8888/mcp。

## 2026-03-23 NPC第八刀：确定性10字换行规则

- 当前主线目标：把气泡从“按宽度猜换行”改成“按明确规则换行”，彻底对齐用户最新规范。
- 本轮子任务：实现确定性的 10 字一行规则，并让 `003` 的测试改成顺序播放、慢速审核模式。
- 本轮根因结论：
  - 之前出错的根因不是单纯参数大小，而是我一直在用“按宽度猜换行”的布局逻辑。
  - 这会导致短句也可能被错误拆成两行，例如 `先缓一缓。` 这种本应一行的内容也被拆开。
  - 这与用户的明确规范冲突：应先按“每行 10 个字”做确定性分行，再基于分行结果去计算气泡尺寸。
- 本轮完成：
  - `NPCBubblePresenter`：`ShowText(...)` 现在先调用 `FormatBubbleText(...)`，把文本强制整理成“每行最多 10 个可见字符”的最终文本。
  - `NPCBubblePresenter`：`UpdateLayout()` 不再先猜宽度再逼文本换行，而是先分析已经分好行的文本，再计算 longest line / total visible characters，进而决定气泡尺寸。
  - `NPCBubblePresenter`：`preferredCharactersPerLine = 10` 已显式入字段；`maxTextWidth = 290`、`minAdaptiveTextWidth = 64` 用于在“10 字一行”的规则下做短句收窄与长句稳定上限。
  - `NPCBubbleStressTalker`：`003` 改成顺序播放样本，不再随机；节奏改慢，便于截图审核。
  - `003.prefab`：已同步 `sequentialPlayback = 1`、`minGapSeconds = 1.8`、`maxGapSeconds = 2.6`、`minDuration = 2.6`、`maxDuration = 6.2`。
- 本轮验证：
  - `git diff --check` 通过。
  - `003.prefab` 当前已明确序列化 `preferredCharactersPerLine: 10` 与顺序播放测试参数。
- 当前恢复点：现在进入主项目验证时，短句应明显保持单行，小框；长句应按 10 字一行逐步长高，不再随机出现错乱换行。

## 2026-03-23 NPC场景化与集成首版推进

- 当前主线目标：继续兑现我自己前面已经认领的那部分 NPC 场景化与集成工作，而不是继续把主线漂去导航线程。
- 本轮子任务：先把 NPC 自己能安全落地的部分做完，包括正式/验证 profile 分层、`003` 的样本定位、批量 Scene 集成工具和首版收口说明。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_BubbleReviewProfile.asset`，用于验证样本 NPC 的专用 roaming 基线。
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCSceneIntegrationTool.cs`，支持批量切换 `Production / BubbleReview`、补 Home Anchor、吸附 Anchor、批量移除或补挂 `NPCBubbleStressTalker`，并自动标记场景 dirty。
  - 将 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab` 的默认 `roamProfile` 切到 BubbleReviewProfile，正式把 `003` 从“一次性压测 NPC”整理成“长期保留的验证样本 prefab”。
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-23_NPC场景化与集成首版收口.md`。
- 本轮只读取证：
  - `Primary.unity` 当前确实还是 prefab 实例 + 位置/缩放覆盖，没有 `homeAnchor` 场景级覆盖；这证明“真正的 Scene 落点”仍是下一步，不是我之前的幻觉。
- 本轮验证：
  - `git diff --check` 对本轮 NPC 文件通过。
- 当前恢复点：NPC 线现在已经有了可操作的 Scene 集成入口；后续继续开发时，可以直接进入 `Primary` 里的 Home Anchor / 活动区域真实落点，而不是再重讲“还没做完”的空话。

## 2026-03-23 NPC遗留项继续推进：生成器角色分流落地

- 当前主线目标：继续完成我前面已经反复认领但还没做完的 NPC 场景化与集成尾巴，不过先只做非热区的工具链收口。
- 本轮子任务：在 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs` 中补上正式 NPC / 验证样本 NPC 的自动分流逻辑，减少以后批量生 prefab 后的手修量。
- 本轮完成：
  - 生成器新增 `GeneratedNpcRole`，支持 `Production` 与 `BubbleReview` 两类默认角色。
  - 生成器界面新增“角色预设”，可直接配置验证样本名称列表，默认把 `003` 视为 review 样本。
  - 验证样本现在会自动绑定 `NPC_BubbleReviewProfile.asset`，并可自动补挂 `NPCBubbleStressTalker`；正式 NPC 则继续走 `NPC_DefaultRoamProfile.asset`。
  - 生成完成后的摘要会明确回报本轮生成里“正式 / 验证样本”的数量，方便快速自检。
- 本轮验证：
  - `git diff --check -- Assets/Editor/NPCPrefabGeneratorTool.cs` 通过。
  - 本轮没有触碰 `Primary.unity`、字体热区、Prefab 热资源，也没有进入 Unity live 写。
- 当前恢复点：我现在没有再丢主线。NPC 自己还能继续推进的内容，已经被重新收束成“先把生成器 / 集成工具继续做顺”；真正的 Scene Home Anchor / 活动范围落点，仍然要等热场景安全后再做。

## 2026-03-23 导航线程最终回执更新

- 当前主线目标：把要发给导航线程的内容更新成当前真正可转发的最终版本。
- 本轮子任务：只更新导航交接文档与记忆，不碰实现代码。
- 本轮完成：
  - 将 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\给导航线程的正式回执_2026-03-23.md` 重写为最终版。
  - 新版回执已经跟上当前 `main @ 3d5978b0` 的真实状态，并补进 `003` 验证样本化、Scene Integration Tool、生成器自动分流这些旧回执还没包含的内容。
  - 也明确写清了：导航线程现在真正要解决的是动态避让闭环，不是再怀疑 NPC 有没有接入口。
- 本轮验证：
  - 本轮没有新增业务代码，只更新了对导航线程的文档口径。
- 当前恢复点：如果用户现在要把需求直接发给导航线程，这份回执已经够用了；NPC 这边后面还会继续做导航交付后的接入与场景化终验，不是彻底没事做。

## 2026-03-25 NPC只读世界扫描

- 当前主线目标：用户要我只做一次扫描检测，判断现在有没有新的用武之地，不开始干活。
- 本轮子任务：只读核查当前 `main`、shared root、MCP 占用和 NPC/导航相关 dirty 面。
- 本轮已证实：
  - 当前 live 现场是 `D:\Unity\Unity_learning\Sunset @ main @ 84fc3818`。
  - `main` 虽然与远端一致，但 working tree 很脏，而且核心业务面正在被多线程推进。
  - NPC/导航最相关的 dirty 已覆盖 `Primary.unity`、`GameInputManager.cs`、`NPCAutoRoamController.cs`、`NPCMotionController.cs`、`NavGrid2D.cs`、`NavigationPathExecutor2D.cs`、`PlayerAutoNavigator.cs`、`PlayerMovement.cs` 等。
  - 仅这组 NPC/导航相关 diff 就已经接近 2000 行新增级别，说明当前是明显的集成施工现场，不是适合我直接插进去补刀的空档。
  - MCP 占用文档当前没有显式 claim，但热区文件本身正在 dirty，不能把“没人 claim”当成“适合随便 live 写”。
- 当前判断：
  - 我现在不是完全没用，而是更适合先做观察、验收准备、边界守门和后续接入等待。
  - 真正适合我重新下场的时机，应该是这波导航/场景集成先收一段，再由我接手 NPC 侧适配、场景化落点和最终联调。

## 2026-03-25 导航自检后的自我任务重排

- 当前主线目标：基于导航线程最新自检，重新判断 NPC 线程接下来最值得自发认领的工作。
- 本轮子任务：只做认知同步和任务设计，不开始实现。
- 本轮已确认：
  - 导航线程已经明确承认：共享底座方向没错，但 `S2 / S3 / S5 / S6` 远未收口，这意味着我现在不该抢碰导航核心，也不该提前改 NPC 运动语义去搅乱联调。
  - NPC 线程当前仍有价值，但更适合先做“接入契约、验收清单、场景化落点设计、后续交互兼容设计”这些不会撞主战场的准备项。
- 如果后续让我自发领活，我优先会做：
  1. NPC-导航接入契约与联调验收清单
  2. NPC 场景化真实落点设计
  3. NPC 受击 / 工具命中 / 反应系统兼容设计草案

## 2026-03-25 docs-only 收口：NPC导航接入契约定稿

- 当前主线目标：
  - 执行治理线程下发的 `2026-03-25_NPC导航接入契约与联调验收规范起稿.md`，把 NPC 当前能做的那部分跨线程协作前置文档真正收稳。
- 本轮子任务：
  - 不碰导航核心、不碰 NPC 业务实现、不碰 Unity live，只把 NPC -> 导航的唯一契约文件落盘，并结束同类文件继续堆量。
- 本轮完成：
  - 新增正式文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md`
  - 文档已经明确声明：
    - 单文件持续维护
    - supersede 历史 `给导航线程的正式回执_2026-03-23.md`
    - 本轮只覆盖冻结语义、最小接入契约、联调验收清单、红线和 deferred 项
- 本轮关键证据：
  - live Git 仍是 `D:\Unity\Unity_learning\Sunset @ main @ 84fc3818f8049d3cd6a5697f87f288429b2b361c`
  - live working tree 当前极脏，docs-only 是本轮唯一正确动作
  - 旧回执已不在 live 文件树，但可从 `e339ccd65d48c56e35e7984e8b524be8124d8d45` 回读正文，说明“吸收旧回执而不复活旧文件”是合理收口方式
  - 当前 `NPCAutoRoamController.cs` / `NPCMotionController.cs` / `001~003.prefab` 仍支持文档中冻结下来的稳定语义
- 当前恢复点：
  - NPC 这轮对导航线程的入口已经不再靠聊天临时口述
  - 等导航线程下一次正式回包时，我应直接按这份新契约验收，而不是再重起平行版文档

## 2026-03-25 NPC 2.0.0 工作区正式起步

- 当前主线目标：
  - 响应用户新增的大需求，把 NPC 从“只做导航协作文档”推进到“2.0.0 进一步落地”的设计阶段。
- 本轮子任务：
  - 落一份能长期回看对照的 `需求拆分.md`
  - 把导航契约迁入 2.0.0，并结束 1.0.0 双份正文并存
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC-导航接入契约与联调验收规范.md`
  - 将 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md` 改为迁移说明
- 本轮新锁定的需求结构：
  - 文档治理收口：导航契约只维护一个当前版本
  - 场景化与角色化：001/002/003 不再只是测试对象
  - 轻交互与关系层：人设气泡、相遇对话、好感度、玩家/NPC 双气泡
  - 反应兼容层：受击 / 工具命中 / 反应系统
- 当前恢复点：
  - 后续如果继续推进 NPC 大设计，优先在 `2.0.0进一步落地` 下展开

## 2026-03-25 续｜用户批准后开始 2.0.0 第一刀实现

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 84fc3818f8049d3cd6a5697f87f288429b2b361c`
- 当前主线目标：
  - 在用户已批准 2.0.0 文档后，把现在就能安全落地的 NPC 角色化内容直接落到 `main`，但不碰导航热区与场景热区。
- 本轮子任务：
  - 补齐 2.0.0 主文件族。
  - 建立三名 NPC 的独立角色化 profile。
  - 让 prefab 与生成器默认接入这套新 profile。
- 本轮完成：
  - 已补齐 `2.0.0进一步落地` 的 4 份主文件：
    - `NPC-导航接入契约与联调验收规范.md`
    - `NPC场景化真实落点与角色日常设计.md`
    - `NPC交互反应与关系成长设计.md`
    - `NPC系统实施主表.md`
  - 已新增：
    - `Assets/111_Data/NPC/NPC_001_VillageChiefRoamProfile.asset`
    - `Assets/111_Data/NPC/NPC_002_VillageDaughterRoamProfile.asset`
    - `Assets/111_Data/NPC/NPC_003_ResearchReviewProfile.asset`
  - 已修改：
    - `Assets/Editor/NPCPrefabGeneratorTool.cs`
    - `Assets/222_Prefabs/NPC/001.prefab`
    - `Assets/222_Prefabs/NPC/002.prefab`
    - `Assets/222_Prefabs/NPC/003.prefab`
- 本轮关键结论：
  - `001 / 002 / 003` 现在不再共享一套通用环境话术。
  - `003` 的验证样本文案也开始带研究型角色感，而不是继续停留在“纯压测句库”。
  - 生成器后续重新生 `001 / 002 / 003` 时，会自动延续这套角色化 profile 基线，减少回退到旧通用配置的风险。
- 本轮验证：
  - `git diff --check` 通过本轮 NPC 文档、asset、prefab 与生成器脚本。
  - prefab 指向的新 profile GUID 与对应 `.meta` 已闭合。
  - 本轮没有触碰 `Primary.unity`、`NPCAutoRoamController.cs`、`NPCMotionController.cs`、导航核心、玩家导航核心，也没有做 Unity / MCP live 写。
- 当前恢复点：
  - 如果继续 NPC 2.0.0，下一刀更适合做场景真实落点、双气泡样式规范或关系成长入口。
  - 导航运动语义、动态避让和玩家导航闭环仍由导航线程主刀，我保持不越界。

## 2026-03-25｜用户占用场景期间的无干扰续推

- 当前主线目标：
  - 用户正在使用 Unity 场景，明确要求我“先别用 MCP，把场景先留给他”，所以本线程要继续推进但不能打扰 live 编辑器。
- 本轮子任务：
  - 从整个脏现场里筛出一条不碰 Unity / MCP / 场景 / 导航热区 / `GameInputManager.cs` 的独立代码切片，继续把能落地的内容推进到不能再推进为止。
- 本轮实际选择的切片：
  - 玩家工具失败反馈气泡 + 水壶运行时状态链
- 本轮完成：
  - 只读核查后确认这不是散乱脏改，而是一条完整闭环：
    - 玩家失败反馈服务与想法气泡
    - `ToolUseCommitResult`
    - 水壶 `watering_current / watering_max`
    - UI 耐久条隐藏与 tooltip 水量显示
  - 新增 `Assets/YYY_Tests/Editor/ToolRuntimeFeedbackTests.cs`
  - 修正 `ToolData.GetTooltipText()`，让它与 `ToolRuntimeUtility.GetWaterCapacity()` 的回退逻辑一致
  - 对完整 owned 白名单跑通 `CodexCodeGuard`，结果通过
- 本轮重要判断：
  - 第一次只拿 `ToolData.cs + 测试文件` 跑闸门时失败，暴露出这条链真实 owned 范围比想象更大；据此收窄出了 11 个真正需要一起白名单收口的文件
  - 这轮没有进 Unity / MCP，没有抢用户场景，也没有碰导航主战场
- 当前恢复点：
  - 若用户继续占用场景，我仍可继续沿“非热区、纯代码、能过 `CodexCodeGuard` 的小闭环”推进
  - 若用户释放场景，我优先回到 NPC 自身的 live 集成与表现验收

## 2026-03-25｜用户释放 MCP 后的第一刀 live 收口

- 当前主线目标：
  - 用户明确说可以重新使用 MCP 后，我先不乱开大题，而是回到 NPC 自身最明显的 live 遗留问题：`003` 仍被测试模式污染。
- 本轮子任务：
  - 只读确认 Unity / MCP 基线
  - 做一次短窗口 Play，验证 `001 / 002 / 003` 的真实 runtime 状态
  - 如果问题定位清楚，只做最小修正并再次短窗口复验，然后立刻 Stop
- 本轮关键过程：
  - MCP preflight 已通过：
    - `unityMCP`
    - `http://127.0.0.1:8888/mcp`
    - `Primary` 已加载且 `isDirty = false`
    - console 初始 `error / warning = 0`
  - 第一次短窗口 Play 发现：
    - `001 / 002` 正常 `IsRoaming = true`
    - `003` 保持 `Inactive`
  - 只读定位根因：
    - `NPCBubbleStressTalker.startOnEnable = true`
    - `NPCBubbleStressTalker.disableRoamWhileTesting = true`
    - 003 是被自己的测试组件关掉了 roam，不是导航没起
  - 已通过 MCP 只改一处 prefab 字段：
    - `Assets/222_Prefabs/NPC/003.prefab`
    - `NPCBubbleStressTalker.startOnEnable = false`
  - 第二次短窗口 Play 复验：
    - `003` 已恢复 `IsRoaming = true / DebugState = ShortPause`
    - 测试模式默认未再自动启动
  - 每次 Play 后都已主动 `Stop`，最终现场已回 Edit Mode
- 当前恢复点：
  - `003` 测试模式正规化这件事现在终于真正闭环了。
  - 接下来如果还要继续 NPC live 推进，最自然的下一刀是 `HomeAnchor` 与 scene 集成；但那会正式进入 `Primary.unity` 写入范围，不能再假装是 prefab-only 小刀。

## 2026-03-25｜把 003 正规化继续推进到工具链层，并补齐 Editor 回归测试

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 4c62ef05`
- 当前主线目标：
  - 用户要求我继续做到“当前确实没有安全可推进项”为止，所以我没有停在 `003.prefab` 一处，而是继续把这次修正向工具链和测试层推进，避免后续生成器 / scene 工具把正式 NPC 再次污染回测试模式。
- 本轮子任务：
  - 补 `NPCBubbleStressTalker` 的显式模式接口
  - 修 `NPCPrefabGeneratorTool` / `NPCSceneIntegrationTool` / `NPCAutoRoamControllerEditor`
  - 增加纯 Editor 回归测试
- 本轮完成：
  - 修改 `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs`
  - 修改 `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - 修改 `Assets/Editor/NPCSceneIntegrationTool.cs`
  - 修改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
  - 新增 `Assets/YYY_Tests/Editor/NPCToolchainRegularizationTests.cs`
- 本轮关键结论：
  - 这组改动已经不再只是“Prefab 上手改一个 bool”，而是把 `003` 的正式语义推进到了生成器、scene 集成和 inspector 入口层。
  - 我最初新增的测试文件曾在 Unity console 里报过“测试程序集直连类型”的 own 错误；我已把它改成反射式写法，并重新通过代码闸门与脚本级编译复核。
  - 当前 Unity console 剩余错误不属于我这轮：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `PageRefs` 缺失
- 本轮验证：
  - `CodexCodeGuard` 对 5 个 C# 文件通过（`Assembly-CSharp / Assembly-CSharp-Editor / Tests.Editor`）
  - MCP 基线通过，当前实例仍是 shared root `Sunset`
  - 做过一次脚本级 `refresh + compile`
  - 未进入 Play Mode
  - `Primary.unity` 仍 `isDirty = false`
- 当前恢复点：
  - 这刀自己的代码与工具链已经形成可白名单收口的独立 checkpoint。
  - 进一步的 Unity 测试作业现在会被 `SpringDay1PromptOverlay.cs / PageRefs` 外部 blocker 截住；除非对方先清掉，否则我不应该再把这条 NPC 切片继续硬拖进 shared root 长时间红态。

## 2026-03-25｜主 checkpoint 后的尾巴清理

- 当前主线目标：
  - 把主 checkpoint 之后直接由我这刀产生的剩余尾巴一起收干净，不把 `.meta` 和旧起稿残留继续丢在现场。
- 本轮完成：
  - 删除 `.kiro/specs/NPC/1.0.0初步规划/2026-03-25_NPC导航接入契约与联调验收规范起稿.md`
  - 确认 `Assets/YYY_Tests/Editor/NPCToolchainRegularizationTests.cs.meta` 是我新增测试脚本自动生成的必要文件，需跟下一次最小收口一起提交
- 当前恢复点：
  - 这轮剩余自己的直接尾巴已经缩到一个极小 follow-up 提交；收完后再继续判断还有没有别的 NPC 自己能安全推进的活。

## 2026-03-25｜剩余项盘点后的线程结论

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 55e2bccd`
- 本轮最终判断：
  - 我已经把这条 NPC 线推进到当前安全边界。
  - 还能继续做的内容不是没有，但都已经进入：
    - `Primary.unity` scene 热区
    - 导航联调主战场
    - 或被外部编译 blocker 卡住的 Unity 复验阶段
- 当前仍属于我后续责任的内容：
  - `HomeAnchor` scene 落点
  - 正式活动区域 / 路线 / 相遇节奏
  - 导航交付后的 NPC 侧联调与体验验收
  - `NPCToolchainRegularizationTests` 的 Unity Test Runner 复验
- 当前不该继续硬做的原因：
  - `Primary.unity` 热区未确认给我独占写窗口
  - Unity console 仍有外部 blocker：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `PageRefs` 缺失
  - shared root 里还有大量他线 dirty，继续扩刀会明显增加误伤概率

## 2026-03-26｜再次复核是否可直接进入 Primary 开工

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 359331b9`
- 本轮只读结论：
  - 现在还不能直接进 `Primary.unity` 开工。
- 证据摘要：
  - shared root：`main + neutral`
  - `Primary.unity`：无 active lock
  - Unity：Edit Mode、非编译、非 domain reload、scene 内存态 `isDirty = false`
  - console：仅剩字体 warning
  - 但 Git working tree 中：
    - `Assets/000_Scenes/Primary.unity` 仍是 `M`
    - 且 diff 非零
  - 共享表现层字体资源仍是 dirty
- 当前线程判断：
  - 这不是“Unity 不稳”的问题，而是“热 scene 磁盘态已 dirty 且没有独占写归属”的问题。
  - 所以正确动作仍是 `read-only-first`，不是直接开始 scene 集成。

## 2026-03-26｜确认满足进入下一代交接条件，并生成 V1 重型交接包

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ ee3187573b62891a5b0a8d974f43c192c4125a34`
- 本轮主线目标：
  - 只判断 `NPC` 是否已经满足进入下一代交接的条件；若满足，则直接生成交接包，不继续 scene 写入，不碰 `Primary.unity`。
- 本轮结论：
  - 判断结果为 `yes`，当前已经满足进入下一代交接条件。
- 判断依据：
  - 当前主线已经足够清楚：下一刀应是 `Primary.unity` 的最小 scene 集成，而不是继续做气泡微调或导航核心改造。
  - 当前 own / non-own 边界已经足够清楚：NPC 负责 prefab / profile / stress talker / scene integration tool / NPC 侧联调；导航核心与 shared root 治理不再由本线程吞并。
  - 当前 blocker 已足够清楚且可交代：`Assets/000_Scenes/Primary.unity` 仍是 dirty，`git diff --stat` 仍显示非零改动，同时没有明确独占写归属，所以继续开工会失真。
  - 当前没有必须由我本轮先补完、否则交接会失真的关键动作；继续补只会变成硬闯热区或越界抢写。
- 本轮完成：
  - 已生成 `NPC` -> `NPCV2` 的 V1 重型交接包目录：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\V2交接文档\`
  - 已落盘文件：
    - `00_交接总纲.md`
    - `01_线程身份与职责.md`
    - `02_主线与支线迁移编年.md`
    - `03_关键节点_分叉路_判废记录.md`
    - `04_用户习惯_长期偏好_协作禁忌.md`
    - `05_当前现场_高权重事项_风险与未竟问题.md`
    - `06_证据索引_必读顺序_接手建议.md`
- 推荐阅读顺序：
  1. `05_当前现场_高权重事项_风险与未竟问题.md`
  2. `01_线程身份与职责.md`
  3. `02_主线与支线迁移编年.md`
  4. `memory_0.md`
  5. `2.0.0进一步落地\memory.md`
  6. `NPC-导航接入契约与联调验收规范.md`
  7. `NPC系统实施主表.md`
- 当前恢复点：
  - `NPC` 这条 V1 线程已经完成“恢复、返工、定边界、写 blocker、准备 scene 集成入口”的职责。
  - 后续更适合由 `NPCV2` 在新的线程语义下继续推进 scene 集成与联调，而不是继续让当前 V1 线程留在 shared root 等热区机会。

## 2026-03-26｜全需求回溯：重新钉死用户在 NPC 线前后提过的完整要求

- 当前主线目标：
  - 因用户明确要求“回顾所有前置需求原文与设计诉求”，本轮重新把 NPC 线从最早生成器需求到 2.0.0 设计、再到 `HomeAnchor` scene 集成与运行中补口链全部回看一遍。
- 本轮完成：
  - 已从以下来源重建需求底账：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC场景化真实落点与角色日常设计.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC交互反应与关系成长设计.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\memory_0.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\V2交接文档\*.md`
  - 已确认后续对用户汇报时必须分阶段说明：
    1. 最早 `PNG -> 动画/控制器/Prefab` 的生成器主线
    2. phase2 的气泡 / 碰撞 / 漫游 / 测试入口 / 工具链主线
    3. 2.0.0 的角色化 / 场景化 / 双气泡 / 好感度 / 命中反应主线
    4. 当前 `HomeAnchor` scene 集成与 Inspector 补口链
- 当前恢复点：
  - 后续若再有人问“到底还剩什么”，不能再只报最后一刀；必须把整条 NPC 线的完整需求层级一起带上。

## 2026-03-29｜全局警匪定责清扫第一轮：NPC 非正式聊天残包认领归仓尝试

- 当前主线目标：
  - 这轮不是继续做 NPC 新功能，而是只对指定的 NPC 非正式聊天残包 exact paths 做真实 `preflight -> sync`。
- 本轮子任务：
  - 完整回读 `2026-03-29_全局警匪定责清扫第一轮_NPC非正式聊天残包认领归仓_01.md`
  - 只用执行书里列出的 exact paths 组白名单
  - 真实运行 `preflight`
  - 若放行才继续 `sync`，否则只钉第一真实 blocker
- 本轮完成：
  - 已在 `main @ d82d15cc` 真实运行：
    - `sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread NPC -IncludePaths <exact package>`
  - 本轮实际纳入白名单的 exact paths：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs.meta`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs.meta`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs.meta`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs.meta`
    - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs`
    - `Assets/YYY_Tests/Editor/NPCInformalChatInterruptMatrixTests.cs.meta`
    - `.codex/threads/Sunset/NPC/memory_0.md`
    - `.codex/threads/Sunset/NPC/2026-03-29_全局警匪定责清扫第一轮_NPC非正式聊天残包认领归仓_01.md`
  - `preflight` 结果：
    - `False`
  - 本轮未进入 `sync`
  - 第一真实 blocker 已钉死为：
    - `type = same-root remaining dirty/untracked`
    - `first exact path = Assets/Editor/Story/DialogueDebugMenu.cs`
    - `exact reason = 位于本轮白名单所属 own root Assets/Editor 下，但未纳入 IncludePaths`
  - 当前脚本同时给出：
    - `own roots remaining dirty 数量 = 27`
  - 当前 own 路径结论：
    - `no`
- 当前恢复点：
  - 这轮已满足 `B｜第一真实 blocker 已钉死`
  - 若后续继续，不该扩题，不该转去做新功能；只能先处理本轮 exact package 所在 same-root 残留，再重跑 `preflight`

## 2026-03-29｜补记：skill-trigger-log 审计层健康复核

- 当前主线目标：
  - 不改动本轮 `NPC` 残包归仓结论，只补核前面已追加的 `STL-20260329-110` 是否让审计层失健康。
- 本轮完成：
  - 已真实运行：
    - `C:\Users\aTo\.codex\tools\check-skill-trigger-log-health.ps1`
  - 返回结果：
    - `Health: ok`
    - `Canonical-Duplicate-Groups: 0`
- 结论：
  - 审计层健康，且不影响本轮既有业务判断；当前对用户仍只应回 `B｜第一真实 blocker 已钉死`。

## 2026-03-30｜剩余 NPC package 接盘开工：integrator 认定与 preflight

- 当前主线目标：
  - 接受当前身份已经改成 `NPC integrator`，不再沿用旧的“非正式聊天残包 exact 回补”叙事，只对剩余 `NPC-dominant package` 做 integrator 认定并真实运行 `preflight`。
- 本轮子任务：
  - 完整回读：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_剩余NPC包接盘开工_02.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\2026-03-29_全局警匪定责清扫第一轮回执_01.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1V2\2026-03-30_断开DialogueDebugMenu对NPC菜单编译耦合回执_02.md`
  - 先把 `NPCInformalChatValidationMenu.cs` 从 `Assets/Editor` 直系根迁入 `Assets/Editor/NPC/`
  - 再按执行书允许范围，对 `Controller/NPC + Data/NPC* + Editor/NPC + .codex/threads/Sunset/NPC` 做 integrator 分层并运行 `preflight`
- 本轮完成：
  - 已真实迁移：
    - `Assets/Editor/NPCInformalChatValidationMenu.cs -> Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - `Assets/Editor/NPCInformalChatValidationMenu.cs.meta -> Assets/Editor/NPC/NPCInformalChatValidationMenu.cs.meta`
  - 已确认：
    - `Assets/Editor/NavigationStaticPointValidationMenu.cs` 本轮未触碰
    - 未回漂到 `Story/UI`、`Story/Interaction`、`Story/Managers`、`Editor/Story`、`Tests/Editor`、`Service/Player`、`Primary.unity`、字体资产、`OcclusionManager`
  - 本轮 integrator 认定：
    - `NPC still-own core`
      - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
      - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
      - `Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs`
      - `Assets/YYY_Scripts/Data/NPCInformalChatExitModel.cs.meta`
      - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
      - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs.meta`
      - `Assets/Editor/NPC.meta`
      - `.codex/threads/Sunset/NPC/**`
    - `carried foreign but can ride with this package`
      - `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
      - `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`
      - `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
  - 已在 `main @ 72e52a51` 真实运行：
    - `sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread NPC -IncludePaths <Controller/NPC + Data/NPC* + Editor/NPC + thread dir>`
  - `preflight` 结果：
    - `False`
  - 本轮未进入 `sync`
  - 第一真实 blocker 已变更为：
    - `type = same-root remaining dirty/untracked`
    - `first exact path = Assets/Editor/NavigationStaticPointValidationMenu.cs`
    - `exact reason = 位于本轮白名单所属 own root Assets/Editor 下，但未纳入 IncludePaths`
  - 当前脚本同时给出：
    - `own roots = Assets/Editor, Assets/YYY_Scripts/Data, .codex/threads/Sunset/NPC, Assets/YYY_Scripts/Controller/NPC`
    - `own roots remaining dirty 数量 = 2`
  - 当前 own 路径结论：
    - `no`
- 当前恢复点：
  - 这轮已满足 `B｜第一真实 blocker 已钉死`
  - 旧的 `DialogueDebugMenu` blocker 不再成立；当前真正挡住本轮归仓的是 `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - 按本轮执行书，后续若继续只能围绕这个首 blocker 裁定，不能越过脚本，也不能擅自吞导航菜单本体

## 2026-03-30｜补带 Navigation editor leaf：剩余 NPC package 通过 preflight 并进入 sync

- 当前主线目标：
  - 在不重开导航、不扩到运行时/scene/字体/渲染禁区的前提下，把 `Assets/Editor/NavigationStaticPointValidationMenu.cs(.meta)` 作为 `carried foreign editor leaf` 纳入当前 `NPC package`，并真实重跑 `preflight -> sync`。
- 本轮子任务：
  - 完整回读：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_NPC_补带NavigationEditorLeaf完成剩余NPC包sync_03.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\2026-03-30_典狱长_NPC_剩余NPC包接盘开工回执_02.md`
  - 接受这次改判：
    - 当前脚本聚合下，真实 own root 仍会落到 `Assets/Editor`
    - `NavigationStaticPointValidationMenu.cs(.meta)` 这轮只按 `carried foreign editor leaf` 带走，不代表导航 owner 语义转移
  - 真实重跑扩过的白名单 `preflight`
- 本轮完成：
  - 已确认：
    - 本轮未触碰 `Assets/Editor/NavigationStaticPointValidationRunner.cs`
    - 未扩到导航运行时、`Story` peeled roots、`Primary.unity`、字体资产或 `OcclusionManager`
  - 本轮白名单在上一轮基础上新增：
    - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
    - `Assets/Editor/NavigationStaticPointValidationMenu.cs.meta`
  - 已在 `main @ 158a4a02` 真实运行扩过白名单的 `preflight`
  - `preflight` 结果：
    - `True`
  - 脚本关键信号：
    - `own roots = Assets/Editor, Assets/YYY_Scripts/Data, .codex/threads/Sunset/NPC, Assets/YYY_Scripts/Controller/NPC`
    - `own roots remaining dirty 数量 = 0`
    - `代码闸门通过 = True`
- 当前恢复点：
  - 当前切片已满足进入同白名单 `sync` 的条件；本轮不再继续扩题，只按同组白名单推进 `sync`

## 2026-03-31｜Primary 过期锁善后：恢复旧 canonical path 并释放 stale NPC active lock

- 当前主线目标：
  - 这轮不是继续 NPC 功能、不是继续 scene 主线、也不是吞 `Primary` 整案；唯一目标是把我名下这把 `Primary.unity` 过期 active lock 做合法善后，并把旧 canonical path `Assets/000_Scenes/Primary.unity` 恢复回 `HEAD` 基线。
- 本轮子任务：
  - 完整回读：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_NPC_Primary过期锁善后与旧canonical恢复_01.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_Primary.unity删除面_01.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`
  - 按执行书顺序执行：
    1. 确认旧 canonical path 是删除面
    2. 从 `HEAD` 恢复 `Assets/000_Scenes/Primary.unity(.meta)`
    3. 以 `NPC` owner 身份运行 `Release-Lock.ps1`
    4. 再对白名单跑 `preflight -> sync`
- 本轮完成：
  - 已确认旧 canonical path 删除面存在：
    - `git status --short -- Assets/000_Scenes/Primary.unity Assets/000_Scenes/Primary.unity.meta`
    - 返回 `D`
  - 已从 `HEAD` 原样恢复：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/000_Scenes/Primary.unity.meta`
  - 已核对恢复后现场：
    - `git diff --name-status -- Assets/000_Scenes/Primary.unity Assets/000_Scenes/Primary.unity.meta`
    - 返回空结果，说明旧 canonical path 已回到 `HEAD` 基线
  - 已在恢复后先查锁，再以 `NPC` owner 身份合法释放：
    - `Check-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity'`
      - 返回 `state = locked`
    - `Release-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity' -OwnerThread 'NPC' ...`
      - 返回 `state = released`
      - 新 history：
        - `.kiro/locks/history/A__Assets__000_Scenes__Primary.unity.lock.20260331-011357.released.json`
    - 再次 `Check-Lock.ps1`
      - 返回 `state = unlocked`
  - 本轮确认：
    - 未触碰 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)`
    - 未改 `ProjectSettings/EditorBuildSettings.asset`
    - 未改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
    - 未顺手改任何 `Primary.unity` 场景内容
- 当前恢复点：
  - 旧 canonical path 和 stale lock 的物理现场都已经回到合法状态
  - 接下来只需按最小白名单 `preflight -> sync` 收口本轮回执

## 2026-03-31｜用户要求回忆“这几轮前的实际全局进度”：已重新钉死当时的总图

- 当前主线目标：
  - 用户不是在问最后一刀做了什么，而是要我回忆“在最近这几轮 integrator / stale lock 善后开始之前，项目当时真正推进到了哪、还剩什么、主次矛盾是什么”。
- 本轮完成：
  - 已重新对齐：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
    - 当前 `NPC` 线程记忆
    - 当前 working tree 现场
  - 已重新确认当时的真实全局进度应分两层看：
    1. `mixed-root 清扫波` 的工程治理进度
    2. `Sunset 整体业务/体验` 的真实完成度
  - 已重新钉死当时的关键判断：
    - 如果只看 mixed-root 清扫，这几轮开始前已经接近尾声：
      - `UI-V1` 已 sync
      - `导航检查V2` 已 sync
      - `spring-day1V2` 已 sync
      - 唯一还没收掉的是 `NPC` 剩余 package
    - 但如果看整个项目真实业务进度，当时远没有“全局快做完”：
      - `NPC` 体验主线并未真正终验通过
      - `Primary.unity` 仍是热根事故面
      - TMP 中文字体稳定性仍是共享资产问题
      - `farm` 方向还压着 `OcclusionManager / TreeController`
  - 已确认当时最准确的主叙事不是“项目整体快完了”，而是：
    - `乱根清扫快收尾`
    - `少数热根与大包问题刚开始被单独立案`
- 当前恢复点：
  - 以后若再向用户汇报那一阶段，必须明确区分：
    - “治理清扫进度很高”
    - 不等于
    - “业务功能与玩家体验已经接近完成”

## 2026-04-01｜用户要求核对我对 NPC 需求的理解：已切回“开发台账”而不是“治理台账”

- 当前主线目标：
  - 用户当前不是要我继续讲清扫、owner、sync 或 lock，而是要我把 NPC 这条线“已经实现什么、明确没实现什么、接下来先做什么、再做什么”的认知重新讲清楚。
- 本轮子任务：
  - 用当前文档和当前代码，重新建立一份只讲开发事实的 NPC 总账。
- 本轮完成：
  - 已重新回读：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-03-27-NPC-非正式聊天完整交互矩阵与查漏补缺方案-01.md`
  - 已再按代码现场复核：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs`
  - 当前重新钉死的开发结论：
    1. 我之前做出来的主要是底层和中层：
       - 会话状态机
       - 自动续聊
       - 距离中断
       - resume snapshot
       - 双气泡底座
       - 关系成长最小底座
    2. 但用户当前明确指出并亲测到的问题说明：
       - 非正式聊天体验闭环没有完成
       - 当前不能把这块说成“已完成只剩微调”
    3. 当前明确未通过的体验项包括：
       - 跑开时玩家气泡鬼畜 / 半透明残留
       - NPC 等待态和对话框残留
       - 双方气泡防重叠和多 NPC 归属不成立
       - 交互提示弱，缺正式感
       - 玩家 / NPC 气泡样式差异还不够清晰
  - 当前对总线优先级的最新判断：
    - 清扫前我的真实开发进度不是“快收尾”，而是：
      - 底层较厚
      - 体验层和 scene 层仍明显缺口
    - 下一步不应继续讲治理，而应优先做：
      1. `Primary.unity` 中 `001 / 002 / 003` 的最小可玩集成
      2. NPC 非正式聊天体验闭环补严
      3. `P5` 受击 / 工具命中 / 反应系统
- 当前恢复点：
  - 以后若用户再次问“现在到底做到哪”，必须先按开发事实回答：
    - 做成了什么
    - 没做成什么
    - 当前阶段
    - 下一步
  - 不能再把治理清扫进度混进主叙事里

## 2026-04-01｜NPC 闲聊体验补口继续施工后停车：左下角统一提示已接管，跑开中断链已止血到代码层

- 当前主线目标：
  - 继续收口 NPC 非正式聊天体验，优先修：
    - 跑开时鬼畜打字机
    - NPC/玩家气泡与头顶交互提示打架
    - 头顶大 `E` 违和
- 本轮子任务：
  - 只做体验补口，不回漂治理清扫：
    - 头顶提示壳继续收窄
    - 左下角统一提示区接管
    - 跑开中断链止血
    - 打字机不再透明渐显
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\NPCInformalChatInteractable.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerThoughtBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
  - 已实现的关键变化：
    1. 统一提示壳：
       - 左下角提示继续作为主提示区
       - 头顶只保留更小的倒三角
       - 非正式聊天进行中不再显示头顶箭头，避免和聊天气泡撞车
    2. 打字机表现：
       - 玩家 / NPC 开句时不再做透明渐显
       - 气泡开始出现后只改文本，不再一边打字一边淡入
    3. 跑开中断链：
       - 先强清旧会话气泡
       - 再直接显示玩家离场句 / NPC 反应
       - 不再复用容易鬼畜的 typed interrupt 链
    4. `E` 职责收紧：
       - 只负责跳过打字机 / 等待 / 收尾
       - 不再把自动续聊又拉回“靠 E 一轮轮驱动”
  - 验证结果：
    - `git diff --check` 通过
    - Unity `Editor.log` fresh compile 通过：
      - `ExitCode: 0`
      - `*** Tundra build success (4.34 seconds)`
  - 当前 blocker：
    - MCP / `127.0.0.1:8888` 仍不可用
    - 因此本轮还没补到新的运行态 live 复测
    - 用户肉眼终验也还没做
- 当前恢复点：
  - 代码侧这轮已经把最刺眼的三个问题先压了一层：
    - 头顶大 `E`
    - 打字机透明渐显
    - 跑开中断仍走旧 typed chain
  - 下一次继续时，最确信的下一步就是：
    - 直接做 `002 / 003` 的运行态重测
    - 重点看：
      - 跑开中断
      - 头顶箭头 / 左下角提示
      - 双气泡是否仍重叠

## 2026-04-01｜NPC 线程自测续记：四条 trace 全绿，顺手清掉了 Play blocker

- 当前主线目标：
  - NPC 非正式聊天这条线继续做真实自测，不再停留在“代码解释得通”。
- 本轮子任务：
  - 跑 `002 / 003` 的四条 `NPCValidation` trace
  - 同时把会话中的提示壳再收紧一层
  - 若 Unity 现场有 compile blocker，一并清掉再继续自测
- 本轮完成：
  - `NPCInformalChatValidationMenu.cs`
    - validation trace 增加 `runInBackground` 兜底
  - `PlayerNpcChatSessionService.cs`
    - 闲聊进行中直接压 world hint，并主动同步左下角提示
  - `InteractionHintOverlay.cs`
    - 增加只读状态暴露，便于后续继续取证
  - `SpringDay1InteractionPromptRuntimeTests.cs`
    - 改成纯反射版，清掉它对 Sunset runtime 的直接编译依赖
  - 自测结果：
    - `002 closure` 过
    - `002 interrupt` 过
    - `003 closure` 过
    - `003 interrupt` 过
- 关键判断：
  - 这轮最核心的判断是：
    - NPC 非正式聊天主链已经不是“只靠说”，而是真拿到了四条运行态 success 证据
  - 但提示 UI 的“中间过程观感”还没拿到完全稳定的 MCP 截点，不能把它装成已终验
- 当前阶段：
  - `P3` 进入“线程自测已过，等待人工体验终验”
- 下一步恢复点：
  - 不再优先补状态机
  - 直接等用户看：
    - 闲聊进行中的提示壳
    - 头顶箭头 / 左下角提示是否符合最终体验

## 2026-04-01｜退回 NPC 底座协作位：继续 PARKED，等待 UI 接后半段玩家面整合

- 当前主线目标：
  - 不恢复 shared prompt shell 实现，继续按用户最新裁定退回 `NPC` 底座 / 体验守门 / 协作位。
- 本轮子任务：
  - 只读回看：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-01_典狱长_NPC_退回底座协作位等待UI接手_02.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-01_典狱长_UI_接管玩家面UIUE整合主刀_03.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\NPC.json`
- 本轮结论：
  - `NPC` 继续保持 `PARKED`。
  - 当前只保留的 NPC 底座边界是：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 当前不再恢复主刀：
    - `SpringDay1WorldHintBubble.cs`
    - `InteractionHintOverlay.cs`
    - `SpringDay1ProximityInteractionService.cs`
    - shared prompt shell 相关 UI/UE 整合链
  - 只有当 `UI` 在接玩家面整合时越界吞到：
    - NPC 会话状态机
    - 跑开中断/自动续聊行为
    - 双气泡体验整包
    我才应重新报 blocker。
- 当前恢复点：
  - 继续停车等待 `UI` 接后半段玩家面整合。
  - 后续若没有越界信号，`NPC` 不主动恢复实现。

## 2026-04-01｜按用户最新口径完成 stale state 收正：NPC exact-own 收窄为 3 件底座文件

- 当前主线目标：
  - 不恢复 shared prompt shell 主刀，先把 `NPC` 这条线的 live state 与边界卫生收正，再等待后续直接业务任务。
- 本轮子任务：
  - 先执行：
    - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1 -ThreadName NPC ...`
  - 再把：
    - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\NPC.json`
    的 stale `owned_paths / expected_sync_paths / current_slice`
    收窄到最新口径。
- 本轮完成：
  - `thread-state` 已继续合法保持：
    - `PARKED`
  - `NPC.json` 当前 exact-own 只保留：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 已明确从当前 state 释放，不再默认挂在 `NPC` 名下的 shared / foreign 面：
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/InteractionHintDisplaySettings.cs`
    - shared prompt tests
    - 广义 Day1 提示壳
  - 当前 `current_slice` 已改成：
    - `NPC底座协作位-PARKED等待UI接手`
- 当前 first blocker：
  - `等待 UI 接手 shared prompt shell；NPC 仅保留底座协作位`
- 当前恢复点：
  - 现在这条线已经完成“先收 state 和卫生”这一步。
  - 下一轮如果用户直接给 `NPC` 本线业务任务，再决定是否 `Begin-Slice` 开工。

## 2026-04-01｜用户要求“把现在做到哪、还剩什么要验、矩阵和回单全列出来”：已改走正式验收总包

- 当前主线目标：
  - 用户这轮不是要我继续埋头补代码，而是要我把 NPC 线当前真实阶段、可测矩阵、待验矩阵和回执单一次性整理出来，方便直接手测后给回单。
- 本轮子任务：
  - 先按规则做：
    - `skills-governor`
    - `preference-preflight-gate`
    - `user-readable-progress-report`
    - `sunset-acceptance-handoff`
    的前置核查与等价流程
  - 然后补跑：
    - `Begin-Slice`
  - 再把验收总包、工作区记忆与线程记忆同步落盘
- 本轮完成：
  - 已新建：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-01-NPC当前阶段用户验收总包-01.md`
  - 这份总包已明确拆开：
    - `NPC own` 真正需要用户终验的项
    - `shared/UI` 只做观察、不记入本轮 NPC 过线的项
  - 文档里已写清：
    - 当前 exact-own 边界
    - 当前阶段
    - 情况矩阵
    - 测试矩阵
    - 长回执单与短回单模板
- 本轮关键判断：
  - 当前最准确的说法不是“NPC 聊天已经全做完了”，而是：
    - 底层闭环站住了
    - 线程自测站住了
    - 真实入口体验还需要用户正式回单
  - 同时必须继续守住边界：
    - shared prompt shell
    - 左下角统一提示壳
    - `001` 正式提示卡
    不再由 `NPC` 单线主刀
- 当前恢复点：
  - 这条线下一步不是先扩代码，而是先等用户按验收包回单。
  - 收到回单后，只对仍属于 `NPC own` 的失败项继续恢复施工。

## 2026-04-01｜验收总包已交，线程已合法停车等待用户回单

- 当前主线目标：
  - 不继续恢复 shared prompt shell，不继续顺手补 UI 壳，而是把这轮停在“用户按验收包手测并回单”。
- 本轮完成：
  - 已执行：
    - `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
  - 当前 `NPC` live 状态：
    - `PARKED`
  - 当前 blocker：
    - `等待用户按NPC验收总包回单；当前保持NPC底座协作位，不恢复shared prompt shell主刀`
- 当前恢复点：
  - 下一次若继续，不是直接顺着这轮再写，而是：
    - 先看用户回单
    - 再只对回单里仍属于 `NPC own` 的失败项重新 `Begin-Slice`

## 2026-04-02｜用户带图指出最新体验问题：已先产出给 UI 的 prompt 和我的自省清单

- 当前主线目标：
  - 用户这轮先不要我继续改实现，而是要我基于最新截图和体验反馈：
    - 给 `UI` 一份可直接转发的 prompt
    - 再把我自己没做好的点、下一轮必修项、以及对话包结构讲清楚
- 本轮完成：
  - 已新增 UI prompt 文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
  - 已新增自省清单文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC本轮自省与下一轮施工清单-01.md`
- 当前用户最新硬要求已被重新钉死：
  - 气泡只做适度避让，不要飞太远
  - 箭头必须跟说话方头顶
  - 自动跳过时长按：
    - `1.0s + 字数 * 0.08s`
  - NPC 气泡样式回退到最初认可版本
  - scene 层处理 NPC 扎堆与互撞内容包
  - 左下角任务优先提示外包给 `UI`
- 当前关键判断：
  - 我前面做对的是底层和结构，
  - 但这轮截图证明：
    - 体验归属
    - 节奏
    - 样式基线
    - scene 分布
    这些都还没做扎实
- 当前恢复点：
  - 这轮交付重点已经从“继续解释现在做到了哪”切成：
    - 把 shared/UI contract 写清
    - 把 NPC 下一轮真正该修的点写清

## 2026-04-02｜继续主线后的真实修复与自验补记：跑开中断链已重新站住

- 当前主线目标：
  - 不是重开 shared/UI 壳，也不是再讲一轮分析，而是把用户点名最严重的 `P3` 问题真修掉：
    - 跑开后玩家气泡鬼畜
    - 玩家气泡卡首字/半透明残留
    - NPC 卡在等待态
    - 正常对话在超距后继续自己往下滚
- 本轮子任务：
  - 继续在 `NPC own` 层收这组 bug，并在 Unity 里补到真正的 runtime trace，而不是只停在静态推断。
- 本轮实际做成了什么：
  - 真实继续改的文件只认：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerNpcChatSessionService.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCBubblePresenter.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\PlayerThoughtBubblePresenterStyleTests.cs`
  - `PlayerNpcChatSessionService.cs`
    - 把超距快进收窄成只跳完当前打字句，避免跑开后正常对话链继续自动滚。
    - 顺手把双气泡避让压回更轻的区间。
  - `NPCBubblePresenter.cs`
    - 把 NPC 气泡边框色恢复回更早认可的暖金样式。
  - `PlayerThoughtBubblePresenterStyleTests.cs`
    - 改成反射版，避免 `Tests.Editor` 直接引用默认运行时程序集类型。
    - 收掉 `Object` 的二义性编译红。
- 本轮验证结果：
  - 通过 `NPCInformalChatValidationMenu` + `CodexEditorCommandBridge` + `Editor.log`，已拿到 6 条硬证据：
    - `002 closure = Completed`
    - `003 closure = Completed`
    - `002 interrupt = WalkAwayInterrupt`
    - `003 interrupt = WalkAwayInterrupt`
    - `002 player-typing interrupt = PlayerSpeaking + WalkAwayInterrupt`
    - `003 player-typing interrupt = PlayerSpeaking + WalkAwayInterrupt`
  - 中间有一次 Play 自动退回 Edit，导致一轮 trace `menu-fail`；重新 `PLAY` 后补跑成功，不再把它认成逻辑失败。
  - 本轮结束时 Unity 已退回 Edit，`status.json` 最后成功看到过：
    - `isPlaying=false`
    - `isCompiling=false`
    - `lastCommand=playmode:EnteredEditMode`
- 当前关键判断：
  - 这轮可以诚实 claim 的只有：
    - 跑开中断主 bug 已经在 own 层被压住
    - targeted probe 已经全绿
  - 这轮仍不能 claim 的是：
    - 玩家真实观感已经彻底过线
- 当前边界：
  - 这轮没有继续重开：
    - `Primary.unity`
    - shared prompt shell / UI 玩家面整合
    - 导航 runtime
    - 字体资产
    - `GameInputManager.cs`
- 当前恢复点：
  - 现在只差审计层落盘与 state 收尾。
  - 本轮完成后，线程应重新 `PARKED`。
  - 下次若继续，只应根据用户终验里仍属于 `NPC own` 的失败项重开新 slice。

## 2026-04-02｜收尾动作已完成：线程重新 `PARKED`，但全局 skill-log 审计层仍有旧重号

- 当前主线目标：
  - 把这轮已经完成的代码与自验真正收成可交接状态，不把 thread-state 和审计层留在半截。
- 本轮子任务：
  - 补 `NPC` 工作区记忆、父记忆、线程记忆。
  - 补 `skill-trigger-log`。
  - 重新 `Park-Slice`。
- 本轮完成：
  - 已执行：
    - `Begin-Slice`（收尾落盘专用）
    - `Park-Slice`
  - 当前 `NPC.json`：
    - `PARKED`
  - `skill-trigger-log.md` 已追加：
    - `STL-20260402-036`
  - `check-skill-trigger-log-health.ps1` 结果：
    - `Canonical-Duplicate-Groups = 1`
    - 旧重号：
      - `STL-20260402-029`
- 当前关键判断：
  - 这轮自己的 NPC 审计尾账已经补齐。
  - 现在不能 claim 完全审计 clean 的唯一原因，不是本轮新写坏了日志，而是全局 canonical skill log 里本来就还有一条旧重号。
- 当前恢复点：
  - 线程现在已经合法停车。
  - 后续如果继续，直接回到用户终验后的 `NPC own` 失败项，不必重复做这一轮收尾。

## 2026-04-02｜只读核对 NPC/UI 边界与进度：我已经分得清理论边界，但现场 reality 仍有交叉

- 当前主线目标：
  - 用户要审的不是代码，而是我对 `UI` 线到底做了什么、和 `NPC` 线边界怎么切、现在是否真的心里有数。
- 本轮子任务：
  - 只读核对：
    - `UI.json`
    - `NPC.json`
    - `UI系统` 父/子工作区 memory
    - `UI` 线程 memory
    - 我之前写给 `UI` 的接管委托
- 本轮稳定结论：
  - `UI` 这段时间真实做过两类事：
    1. shared prompt / 任务优先提示：
       - `SpringDay1ProximityInteractionService`
       - `InteractionHintOverlay`
       - `SpringDay1WorldHintBubble`
       - 对应 runtime tests
    2. 玩家气泡 formal-face 回正：
       - `PlayerThoughtBubblePresenter`
       - `PlayerThoughtBubblePresenterStyleTests`
  - 这说明我不能再说“UI 只做左下角提示”了；它后来还继续接手了玩家气泡样式线。
  - 但我也不能说“UI 把 NPC 聊天整条线都做了”：
    - 它没有主拿 `Primary`
    - 没有主拿 `GameInputManager`
    - 没有 claim 自己完成 `002/003` 的 closure/interrupt runtime 链
    - 没有接走 NPC 会话状态机整案
  - 更真实的边界判断是：
    - 理论边界：
      - `NPC = 会话底座 / 自动续聊 / 跑开中断 / NPC 双气泡`
      - `UI = 左下角/头顶提示整合 + 玩家面气泡 formal-face`
    - 现场 reality：
      - `UI.json` 现在仍把
        - `PlayerThoughtBubblePresenter.cs`
        - `PlayerNpcChatSessionService.cs`
        - `NPCBubblePresenter.cs`
        - `PlayerThoughtBubblePresenterStyleTests.cs`
        挂成自己的 `ACTIVE` slice
      - 说明 dirty 与 state 归属并没有完全回正
- 当前关键判断：
  - 我现在是清楚“该怎么切”的。
  - 但我也必须诚实承认“现场还没切干净”，不能假装现在已经是两条完全不重叠的线。
- 当前恢复点：
  - 如果后续继续由我推进 `NPC`，我应只按 NPC own 失败项继续，不再主动吞 shared/UI 壳。
  - 如果要把 NPC/UI 现场边界彻底收正，后面仍需要一次治理或用户再裁定。

## 2026-04-03｜主线已切到“春一日新 NPC 群像”，本轮先完成了 `NPC-v` 的本体层 preflight 与首个 own 修复

- 当前主线目标：
  - 不再围着旧 `0.0.2` 闲聊体验打转，而是按新 prompt 把新增 8 人群像分成两半：
    - 我做 `NPC-v` 本体层
    - 对方做 `spring-day1` integration 层
- 本轮子任务：
  - 先做 `NPC-v` 自己这层的真实 preflight：
    - 8 套 sprite / anim / prefab / roam / content / manifest 是否齐
    - prefab 上 roam/chat/profile 是否真接上
    - 现成 content 资产是不是空壳
  - 然后只补 `NPC-v own` 的首个真实缺口
- 本轮实际做成了什么：
  - 已 `Begin-Slice`：
    - `春一日新NPC群像-NPC本体层preflight与内容资产补口`
  - 只读核对后确认：
    - 8 张 sprite、8 套 anim、8 个 prefab、8 份 roam、8 份 content、1 份 manifest 全在
    - 8 个 crowd prefab 也都挂了：
      - `NPCAutoRoamController`
      - `NPCInformalChatInteractable`
      - `roamProfile`
      - `homeAnchor`
  - 但第一真实 blocker 是：
    - 8 份 `DialogueContent.asset` 全空
    - 所以 prefab 看着齐，本体层其实“不可聊”
  - 已修：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
  - 修法：
    - 不再靠 `EditorJsonUtility.FromJsonOverwrite(...)` 假装写 content
    - 改成逐层构造 `NPCDialogueContentProfile` 的真实嵌套对象并回填私有字段
  - 然后通过当前开的 Unity 实例重跑：
    - `Tools/NPC/Spring Day1/Bootstrap New Cast`
  - 修后 8 份 crowd `DialogueContent.asset` 都不再是空壳，已具备：
    - `npcId`
    - `playerNearbyLines`
    - `defaultInformalConversationBundles`
    - `defaultWalkAwayReaction`
    - `defaultChatInitiatorLines`
    - `defaultChatResponderLines`
  - 还顺手把 `101` 的 `……` cue 改成了 ASCII `...`
- 当前关键判断：
  - 这轮最重要的不是“又多生成了一遍文件”，而是把 `NPC-v` 本体层里最假的那一层补真了：
    - 之前只是产物壳齐
    - 现在至少 content 真能支撑闲聊链了
  - 但这不等于整组新群像已经完工
- 当前仍未闭的项：
  - 8 份新 crowd `DialogueContent.asset` 的 `pairDialogueSets` 仍是空的
  - `Primary` 里的 runtime phase 拉起和 Day1 消费还没轮到我这边去 claim
- 当前收尾动作：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
  - 当前 blocker：
    - `春一日新NPC群像：8人本体层已补 content 空壳；pair-specific 语义仍未闭，Day1 phase/runtime 消费待 integration 侧接手`
- 当前恢复点：
  - 下次若继续，我应只围绕 `NPC-v` 本体层剩余 own 项推进，不再漂到 Day1 integration。

## 2026-04-02｜本轮只交文档与判断，线程已重新停车

- 当前主线目标：
  - 不继续顺手实现，把这轮停在“用户可直接拿 prompt 转给 UI，也可直接看自省清单决定 NPC 下一刀”的位置。
- 本轮完成：
  - 已执行：
    - `Park-Slice`
  - 当前 live 状态：
    - `PARKED`
  - 当前 blocker：
    - `等待用户决定：先转发UI prompt，还是直接让我按自省清单恢复NPC own施工`
- 当前恢复点：
  - 后续若继续，优先按用户裁定：
    - 要么先走 shared/UI 合同链
    - 要么直接回到 NPC own 的气泡 / 节奏 / scene 反扎堆施工

## 2026-04-01｜UI 打断后共享文件 owner 收边界：NPC 应收窄回四件套

- 当前主线目标：
  - 在 `UI` 已 `PARKED` 后，重新确认为了继续完成 NPC 非正式聊天体验，NPC 这条线到底还应继续 own 哪些文件，哪些共享提示壳应释放回 `Story / Day1 / UI` owner。
- 本轮子任务：
  - 只读核对：
    - `.kiro/state/active-threads/NPC.json`
    - `.codex/threads/Sunset/NPC/memory_0.md`
    - 当前 working tree 相关 dirty / untracked
    - `PlayerNpcChatSessionService.cs`
    - `NPCInformalChatInteractable.cs`
    - `SpringDay1ProximityInteractionService.cs`
    - `SpringDay1WorldHintBubble.cs`
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `SpringDay1DialogueProgressionTests.cs`
    - `SpringDay1InteractionPromptRuntimeTests.cs`
- 本轮结论：
  - `NPC` 真实仍应继续 own 的最小生产文件集：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 应释放回 shared `Story / Day1 / UI` owner 的文件：
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
  - 额外现场事实：
    - working tree 里还有未跟踪的
      - `Assets/YYY_Scripts/Story/UI/InteractionHintOverlay.cs`
      - `Assets/YYY_Scripts/Story/UI/InteractionHintDisplaySettings.cs`
    - 这两个文件属于“左下角统一提示壳 / 设置开关”，不应继续算作 `NPC` long-term own。
  - 关键判断：
    - `SpringDay1ProximityInteractionService.cs` 已经是工作台 / 床 / 正式 NPC 对话 / Day1 导演都会消费的统一近身仲裁壳，不再是 NPC 单线私有。
    - `SpringDay1WorldHintBubble.cs` 当前承担的是共享头顶提示视觉壳，继续微调它会同时影响多个非 NPC 目标。
    - 因此当前 blocker 若仍包含“左下角提示与头顶箭头最终观感”，NPC 单线不能再把它装成自己四件套内就能独立收完的事。
- 当前 live 状态判断：
  - 继续保持 `PARKED`
  - 如果后续恢复 NPC 自己的最小施工，应先重新 `Begin-Slice`，并把 slice 收窄到四件套，不再把 shared 提示壳一起吞进来。
- 当前恢复点：
  - 后续若继续做 NPC，应只围绕：
    - 自动续聊
    - 跑开收束
    - 双气泡占位/表现
    这组四件套继续压实
  - shared 提示壳的最终观感与唯一 E 仲裁，应改由 `Story / Day1 / UI` owner 接盘

## 2026-04-02｜NPC 线程恢复真实施工：体验补口已写进代码和 scene，但 live 复测被外部编译红阻断

- 当前主线目标：
  - 按用户最新反馈，把 NPC 非正式聊天这条线的提示壳、气泡节奏、跑开中断和 `Primary` 站位继续收干净。
- 本轮子任务：
  - 延用已有 `thread-state ACTIVE` 切片，不重开新锁。
  - 先把头顶箭头 / 左下角提示 / ambient bubble 冲突收窄。
  - 再把自动续聊时长、跑开中断接管、scene 站位一起补掉。
- 本轮完成：
  - 已改：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs`
    - `Assets/111_Data/NPC/NPC_002_VillageDaughterDialogueContent.asset`
    - `Assets/111_Data/NPC/NPC_003_ResearchDialogueContent.asset`
    - `Assets/000_Scenes/Primary.unity`
  - 实际效果：
    - 头顶世界提示只在真正可交互时出现，和左下角提示重新同拍。
    - 当前焦点 NPC 可交互时，会压掉自己的 ambient bubble，不再和提示/气泡互抢。
    - 跑开中断句改成直接实心接管，不再重新淡入。
    - 自动续聊停顿改成 `1.0s + 字数 * 0.08s`。
    - 对话气泡避让进一步收窄。
    - `Primary` 当前读回里 `001 / 002 / 003` 已在各自 `HomeAnchor` 位置，不再是 `002 / 003` 编辑态扎堆。
    - `NPC_002 / NPC_003` 的 `……` cue 已改成 ASCII，避免字体缺字 warning。
- 本轮验证：
  - `validate_script` 通过：
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenter.cs`
    - `PlayerNpcChatSessionService.cs`
    - `SpringDay1ProximityInteractionService.cs`
  - `git diff --check` 通过本轮 own 范围。
  - MCP `8888` listener 掉过一次，已用项目自带 `mcp-terminal.cmd` 拉回，基线脚本重新 `pass`。
- 当前 blocker：
  - 无法继续做本轮 `Play` 自测，原因不是 own 文件红，而是外部 compile blocker：
    - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
    - `Assets/YYY_Scripts/UI/Toolbar/ToolbarSlotUI.cs`
    缺失 `TickStatusBarFade / ApplyStatusBarAlpha`
  - Unity 因此持续 `isCompiling=true`，validation trace 暂时跑不起来。
- 当前 live 状态：
  - 已执行：
    - `Park-Slice`
  - 当前状态：
    - `PARKED`
- 当前恢复点：
  - 外部编译红清掉后，下一步直接恢复 fresh Play：
    - `002 / 003 closure`
    - `002 / 003 interrupt`
    - `001` 任务优先提示
    - 头顶箭头 / 左下角提示 / 气泡距离的最终画面验收

## 2026-04-03｜春一日新 NPC 群像联合完工 prompt 已转交给 `NPC-v`

- 当前主线目标：
  - 用户已切到新的紧急主线：把新增 8 个 NPC 资源接入 `spring-day1`，并明确这轮要我发 prompt 给 `NPC-v`，而不是继续由我代做后半刀。
- 本轮已确认现场：
  - `Assets/Sprites/NPC/101~104、201~203、301` 已存在。
  - 当前工程里已经真实落盘：
    - `Assets/222_Prefabs/NPC/101~301.prefab`
    - `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
    - `Assets/100_Anim/NPC/101~301/*`
    - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
  - `Editor.log` 已明确记录：
    - `[SpringDay1NpcCrowdBootstrap] 已生成 8 名 spring-day1 新 NPC 的 prefab、profile、对话包与 crowd manifest。`
- 本轮完成：
  - 已新增可直接转发给 `NPC-v` 的联合完工续工 prompt 文件：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-NPC-v_春一日新NPC群像联合完工续工prompt-02.md`
  - prompt 已写清：
    - 我这边已经做成的基线
    - 总需求盘点
    - `NPC-v` 的 exact-own 区
    - 我下一轮会接的 `spring-day1` integration 区
    - 还没闭完的项
    - 固定回执格式
- 当前恢复点：
  - 这轮我转为“给 `NPC-v` 分发联合完工 prompt”，不继续替它主做后半刀。

## 2026-04-03｜补齐“我自己的并行任务单”：下一轮按我和 `NPC-v` 双文件并行推进

- 用户纠正成立：
  - 不是“用户和 `NPC-v`”，而是“我和 `NPC-v`”并行。
- 本轮补齐：
  - 已新增我的自用并行任务单：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-本线程_春一日新NPC群像Day1整合并行任务单-03.md`
  - 并回写 `NPC-v` prompt，使其显式引用这份自用任务单，避免再出现“只有对方有任务书，我这边没有”的歧义。
- 当前恢复点：
  - 下一轮应严格按双文件并行：
    - `NPC-v` 负责 NPC 本体层
    - 我负责 `spring-day1` / Day1 integration 层

## 2026-04-03｜NPC-v 本体层继续补口：验证菜单已扩成全链护栏，`002 StuckCancel` 当前被压回 scene blocker

- 当前主线目标：
  - 继续沿“春一日新 NPC 群像”的 `NPC-v` 本体层推进，不碰 Day1 integration 主区，不把 legacy `Primary` 旧账混成这轮 own 完成定义。
- 本轮子任务：
  - 把新群像的 preflight 从“人工看过”提升成“工具自己查全链”。
  - 同时对用户刚贴的 `002 roam interrupted => StuckCancel` 做只读归因，不擅自改 scene。
- 本轮完成：
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\SpringDay1NpcCrowdValidationMenu.cs`
  - 当前验证菜单新增检查：
    - `Assets/Sprites/NPC/{id}.png`
    - `Assets/100_Anim/NPC/{id}/Controller/*`
    - `Assets/100_Anim/NPC/{id}/Clips/*` 数量
    - prefab 的 `SpriteRenderer / Animator / NPCAnimController / NPCMotionController / NPCAutoRoamController / NPCInformalChatInteractable / NPCBubblePresenter`
    - prefab 的 sprite / controller 引用一致性
    - `pairDialogueSets` 的 partner 合法性、无重复、无自指、双向 reciprocal 闭合
  - 已通过当前 Unity 再次执行：
    - `Tools/NPC/Spring Day1/Validate New Cast`
  - 当前硬证据：
    - `status.json`
      - `lastCommand = menu:Tools/NPC/Spring Day1/Validate New Cast`
      - `success = true`
    - `Editor.log`
      - `[SpringDay1NpcCrowdValidation] PASS | npcCount=8 | totalPairLinks=16 | ...`
- 本轮只读判断：
  - `Primary.unity` 里旧 `001 / 002 / 003` 仍明确绑定各自 `HomeAnchor`，但 scene 位置偏差为：
    - `001`
      - delta = `(1.016, 1.196)`
    - `002`
      - delta = `(-9.620, -1.444)`
    - `003`
      - delta = `(-10.166, -3.763)`
  - 因此用户贴出的 `002 StuckCancel` 当前更像 scene 旧锚点漂移，不像我这轮 `NPC-v` crowd pair/data 又写坏。
- 当前恢复点：
  - `NPC-v` own 层当前已更接近“产物链 + preflight 都成立”。
  - 如果下一刀继续，我最该做的是：
    - 新 8 人 runtime targeted probe
  - 而不是去吞旧 `001 / 002 / 003` 的 scene 位置善后。

## 2026-04-03｜审计补记：本轮已补 skill-trigger-log 并重新 `PARKED`

- 本轮审计层已完成：
  - `skill-trigger-log.md`
    - `STL-20260403-025`
  - `check-skill-trigger-log-health.ps1`
    - `Canonical-Duplicate-Groups = 0`
- 当前 live 状态：
  - `NPC`
    - `PARKED`
- 当前恢复点：
  - 这轮如果再继续，不需要先补尾账；
  - 直接从“新 8 人 runtime targeted probe”或“legacy scene blocker 交接”继续即可。

## 2026-04-03｜继续施工：新 8 人 runtime targeted probe 已拿到硬结果，当前只剩收口与 pair runtime 归因

- 当前主线目标：
  - 只做 `NPC-v` 本体层的：
    - 新 8 人 runtime targeted probe
    - own dirty / memory 归仓
- 本轮真实完成：
  - `SpringDay1NpcCrowdValidationMenu.cs`
    - 修掉了本轮 own compile red
    - 把 runtime probe 跑通到可重复复现
    - 补了 pair timeout 缩窄诊断
  - Unity 里再次拿到：
    - `Validate New Cast = PASS`
    - `npcCount = 8`
    - `totalPairLinks = 16`
  - 运行态 probe 稳定结果：
    - `8/8 instance = PASS`
    - `8/8 informal chat = PASS`
    - `0/2 pair dialogue = FAIL`
    - `2/2 walk-away interrupt = PASS`
- 当前最核心判断：
  - `101 <-> 103`、`201 <-> 202` 的 pair 并不是没进入聊天；
  - 它们已经进入 ambient chat decision，pair 台词也能在运行时解析出来；
  - 但 `NPCBubblePresenter` 一直没真正 visible，且：
    - `suppressed = false`
    - `conversationOwner = none`
  - 所以我现在最确信的判断是：
    - 失败点已经缩到 `ambient pair bubble` 发射链本身
    - 不再是 Day1 integration、旧 `Primary` scene，或玩家提示壳误伤
- 本轮额外现场事实：
  - Unity 被外部 scene 改动弹窗反复卡过：
    - `打开场景已在外部被修改。`
  - 为了不吞 scene 现场，我都只点：
    - `忽略`
  - 这个会冻结 bridge，但不是这轮 pair 失败的根因。
- 当前恢复点：
  - 如果继续开发，不要再回旧 `002 / 003`；
  - 下一刀应直查：
    - `NPCAutoRoamController` 的 ambient pair bubble 播放链
  - 如果当前先收口，则下一步就是：
    - 更新审计层
    - 跑 `Ready-To-Sync`
    - 能 sync 就 sync；不能 sync 就只报第一真实 blocker

## 2026-04-03｜本轮阶段收尾：runtime probe 完成，sync 前置被历史 own roots 尾账拦下

- 本轮最终新增事实：
  - `Ready-To-Sync.ps1 -ThreadName NPC`
    已执行，结果：
    - `BLOCKED`
  - 随后为了合法停表，已执行：
    - `Park-Slice.ps1 -ThreadName NPC`
  - 当前 live 状态：
    - `PARKED`
- 第一真实 blocker：
  - 不是 `SpringDay1NpcCrowdValidationMenu.cs` 本轮新改动本身；
  - 而是 `NPC` 历史 own roots 还挂着同根旧脏，导致这轮不能只带当前 validation menu + memory 收口
  - 已明确报实至少有：
    - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - `Assets/Editor/NPC/CodexEditorCommandBridge.cs(.meta)`
    - `0.0.2` 目录下未归仓 prompt 文档
- 当前恢复点：
  - 下一轮如果继续收口，要先解决历史 own roots 尾账；
  - 下一轮如果继续功能诊断，则直接盯 ambient pair bubble 发射链，不再回旧 `002 / 003`。

## 2026-04-03｜补记：按 prompt-04 完成春一日新 8 人原剧本人设核实，只读结束

- 当前主线目标：
  - 回到原 Day1 剧本与长线 NPC 角色表，核实当前 `101~301` 新 8 人到底哪些有原案来源，哪些只是后补 crowd 槽位。
- 本轮子任务：
  - 按 `2026-04-03-NPC-v_春一日新NPC群像原剧本人设核实与NPC本体映射回正prompt-04.md`
    做原案角色核实、当前槽位归类和是否足够最小回正的判断。
- 本轮实际完成：
  - 已完整读取 prompt 指定的 10 个文件。
  - 已查实真相源优先级：
    - 用户原始 Day1 剧情原文
    - `0.0.1剧情初稿`
    - 长线 NPC 角色表
    - 当前 `bootstrap / manifest`
  - 已查实：
    - `NPC001` = `马库斯`
    - `NPC002` = `艾拉`
    - `NPC003` = `卡尔 / 研究儿子` 的最接近语义壳
  - 已查实当前 `101~301` 中：
    - `102 / 103 / 203` 只到“语义接近 / 可降级为群众层”
    - `101 / 104 / 201 / 202 / 301` 没有原案直接来源
    - `301` 与当前 Day1 主线气质冲突最大
  - 已完成判断：
    - 当前足够判“写偏”
    - 但不够合法做“每个槽位一对一回正”
- 本轮没有做：
  - 没改 `SpringDay1NpcCrowdBootstrap.cs`
  - 没改 `SpringDay1NpcCrowdManifest.asset`
  - 没改 `Assets/111_Data/NPC/SpringDay1Crowd/*`
  - 没跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
    - 原因：本轮始终停留在只读核实；线程状态沿用上轮 `PARKED`
- 第一真实 blocker：
  - 目前缺的是“8 个 crowd 槽位对原案具名角色的一对一权威映射”，
    而不是缺运行态 probe 或缺更多自创对白。
- 修复后恢复点：
  - 如继续本线，要么先补更高权威的 cast 映射证据，
    要么显式授权把明显写偏槽位降级成匿名 / 次级群众口径，再做最小回正。

## 2026-04-03｜补记：按用户直委托完成 NPC 对话头像外部批量导出

- 当前主线目标：
  - 不再让 `美术生` 接手，直接由当前线程基于 `Assets/Sprites/NPC/*.png` 的 `3x4` 源图批量导出对话头像。
- 本轮子任务：
  - 为每个 NPC 源图各自生成 `10` 张可用于 RPG 对话框的人物头像，输出到：
    - `D:\UUUnity\NPC\<角色名>\`
- 本轮实际完成：
  - 核实 `Assets/Sprites/NPC/` 当前共有 `11` 张角色 PNG，且尺寸统一为 `96x128`，即单帧 `32x32`
  - 已对每个角色导出：
    - 正面 3 张
    - 左侧 3 张
    - 右侧 3 张
    - 正面近景 1 张
  - 已实际落盘：
    - `11` 个角色目录
    - `110` 张透明底头像 PNG
  - 输出根目录：
    - `D:\UUUnity\NPC`
- 本轮没有做：
  - 没改 `Assets/Sprites/NPC/*.png`
  - 没改 `Assets/Sprites/NPC/*.png.meta`
  - 没接 DialogueUI / 对话框配置
  - 没进 Unity / Play Mode / MCP
- 当前验证：
  - 已核对每个角色文件夹都有且只有 `10` 张 PNG
  - 已抽查：
    - `001/10_front_closeup.png`
    - `101/10_front_closeup.png`
    的头像裁框
- thread-state：
  - 本轮前段对 Sunset tracked 资源保持只读，因此一开始未跑 `Begin-Slice`
  - 在准备补 Sunset 侧 tracked `memory` 时，已补跑：
    - `Begin-Slice.ps1 -ThreadName NPC -CurrentSlice npc-dialogue-portraits-batch-export-20260403`
  - 本轮收尾时已补跑：
    - `Park-Slice.ps1 -ThreadName NPC -Reason npc-dialogue-portraits-export-finished-no-sunset-sync`
  - 当前状态：
    - `PARKED`
- 当前恢复点：
  - 这批头像基础导出已完成，可直接给对话框系统消费
  - 如果继续下一刀，最合理的不是重做基础导出，而是决定：
    - 每个角色默认选哪一张
    - 是否需要第二轮情绪 / 表情版头像

## 2026-04-03｜补记：用户判定上一轮头像方向完全错误，已按严格右向胸像规范重做

- 当前主线目标：
  - 按用户新下发的“严格修正版”彻底重做 NPC 对话头像，完全放弃上一轮那批被用户判定为错误方向的头像。
- 本轮子任务：
  - 对 `Assets/Sprites/NPC/*.png` 每个角色重新导出 `10` 张头像，但这次必须严格满足：
    - 侧向右
    - 仅半身 / 胸像
    - 平和基础状态
    - 透明底
    - 同角色 10 个可选版本
- 本轮实际完成：
  - 重新核对源图仍为 `11` 张 `96x128` 的 `3x4` 角色图集
  - 严格只使用右向行做头像源
  - 重新为每个角色导出：
    - `01.png ~ 10.png`
  - 最终落盘：
    - `D:\UUUnity\NPC\001 ~ 301`
    - 共 `110` 张 PNG
  - 已再次核对：
    - 每个角色目录都是 `10` 张
    - 全部尺寸统一为 `128x128`
- 本轮没有做：
  - 没碰 Sunset 内的 NPC 源图和 meta
  - 没接入 DialogueUI
  - 没做表情版
- thread-state：
  - 这轮前半段仍然只做外部产物生成
  - 在准备写 Sunset tracked `memory` 时已跑：
    - `Begin-Slice.ps1 -ThreadName NPC -CurrentSlice npc-dialogue-portraits-strict-redo-20260403`
  - 当前收尾阶段仍需：
    - `Ready-To-Sync`
    - 如不能收口则 `Park-Slice`
- 当前恢复点：
  - 用户现在可直接在 `D:\UUUnity\NPC` 查看新一轮严格右向胸像产物
  - 如果后续继续，下一步应是“默认头像选型 / 情绪版规划”，不是再重跑基础胸像导出

## 2026-04-03｜补记：用户再次明确判定这批头像仍然不可用，当前应按失败处理

- 当前主线目标：
  - 不再把刚才那轮 `D:\UUUnity\NPC` 产物包装成“已经满足半身胸像要求”，而是按真实失败结论纠偏。
- 本轮新增稳定结论：
  - 我已经承认并确认：
    - 当前 `D:\UUUnity\NPC` 这批图仍然用不了
    - 它们不是合格的 RPG 对话框半身头像
  - 失败根因不是只差一点润色，而是：
    - 我错误地把“半身人像”理解成“右向行走帧裁切放大”
  - 这会直接导致：
    - 仍然像走路小人的局部裁图
    - 脸和头的阅读性不够
    - 构图不是真正胸像
    - 10 张之间只是裁切微调，不是合理的人像可选版本
- 当前状态：
  - 这批外部 PNG 应按失败产物处理
  - 不应继续接入任何对话框默认头像位
- thread-state：
  - 本轮在做失败纠偏与 memory 修正，因此已补跑：
    - `Begin-Slice.ps1 -ThreadName NPC -CurrentSlice npc-portrait-requirement-failure-audit-20260403`
  - 收尾时仍需：
    - `Park-Slice`
- 当前恢复点：
  - 如果后续继续重做，正确方向必须改成：
    - 基于角色设定重构真正的右向胸像
    - 而不是继续裁现成行走帧

## 2026-04-03｜补记：NPC 静态大头照第三轮已按 bust 重组法重做

- 当前主线目标：
  - 把 `Assets/Sprites/NPC/*.png` 的 NPC 行走图，重做成真正可用于 RPG 对话框的静态大头照。
- 本轮子任务：
  - 只做外部头像产物，不碰 Sunset 内的 NPC 源图和运行时资源。
  - 输出位置固定为：
    - `D:\UUUnity\NPC`
- 本轮实际完成：
  - 清空旧的错误输出后，重新为 `11` 个角色各生成了 `10` 张 PNG。
  - 当前目录结构为：
    - `D:\UUUnity\NPC\001`
    - `D:\UUUnity\NPC\002`
    - `D:\UUUnity\NPC\003`
    - `D:\UUUnity\NPC\101`
    - `D:\UUUnity\NPC\102`
    - `D:\UUUnity\NPC\103`
    - `D:\UUUnity\NPC\104`
    - `D:\UUUnity\NPC\201`
    - `D:\UUUnity\NPC\202`
    - `D:\UUUnity\NPC\203`
    - `D:\UUUnity\NPC\301`
  - 总产出为：
    - `110` 张透明底静态大头照
- 关键决策：
  - 彻底放弃“把右向 walking frame 直接裁半截放大”的旧错误。
  - 新方案改成：
    - 从右向帧拆出头部层和肩胸层
    - 重新组合成 bust 构图
    - 通过不同裁框、缩放和落位生成 `10` 个可选版本
- 已验证事实：
  - 每个角色目录当前都有且只有 `10` 张 PNG。
  - 抽查看到的新图已经不是 walking sprite 的半截裁图，而是静态 bust 构图。
  - 本轮没有修改：
    - `Assets/Sprites/NPC/*.png`
    - `Assets/Sprites/NPC/*.png.meta`
    - 任何 Sunset 运行时资源
- 当前阶段：
  - `targeted probe / 局部验证`
  - 外部头像资产已具备可挑选性，但还没有在真实 DialogueUI 里做终验。
- 当前风险：
  - 强遮脸角色的侧脸可读性仍然受源帧限制，当前更适合先选默认图，再决定要不要做局部人工精修。
- thread-state：
  - 已跑：
    - `Begin-Slice.ps1 -ThreadName NPC -CurrentSlice NPC静态大头照重做`
- 当前恢复点：
  - 用户可直接查看：
    - `D:\UUUnity\NPC`
  - 若继续下一刀，应先做：
    - 每角色默认头像选型
    - 少数角色的二次精修判定

## 2026-04-03｜纠偏补记：第三轮 bust 头像也被用户明确否决，当前应停止批量线

- 当前主线目标：
  - 诚实承认第三轮 `D:\UUUnity\NPC` bust 头像并没有过线，而不是继续把它描述成“可挑选可使用”。
- 本轮新增稳定结论：
  - 用户已明确判定这批图：
    - 很丑
    - 很恶心
    - 不像人
    - 根本不能用
  - 因此当前最真实状态不是“已有可用基础资产”，而是：
    - 头像线仍未成功
- 当前失败根因：
  1. 仍在误用输入：
     - 试图从 `32x32` walking sprite 直接推导最终人像
  2. 仍在误用流程：
     - 没有先做 `1` 张正式母版过稿，就直接量产
  3. 仍在高估自己当前的美术生成能力：
     - 结构能拼出来，不代表审美能成立
- 当前阶段判断：
  - 应从“局部验证已可用”回退为：
    - `结构性试错失败，真实体验明确不通过`
- 正确下一步已收窄为：
  - 停止继续批量
  - 先做 `1` 个角色、`1` 张正式头像母版
  - 这张母版必须在 Aseprite 里按手工重绘 / 重手工修型路线去做
  - 只有单样张被用户点头后，才允许继续谈：
    - 同角色变体
    - 其他角色扩展
    - 批量命名 / 导出自动化
- Aseprite / MCP 的定位修正：
  - 它们可以继续作为：
    - 编辑底座
    - 导出工具
    - 批次工具
  - 但不能再被当成“审美自动生成器”
- 当前恢复点：
  - 如果继续这条线，唯一合理的主刀是：
    - 单角色正式样张验证
  - 不再允许直接重跑 `11 x 10`

## 2026-04-03｜补记：`102` 单角色验牌包已落地

- 当前主线目标：
  - 按新的正确路线，只做 `1` 个代表角色的正式样张验证。
- 本轮子任务：
  - 选 `102` 作为代表角色，落一包可直接验牌的 `A/B/C` 候选。
- 本轮实际完成：
  - 已输出到：
    - `D:\UUUnity\NPC\_验牌_102_2026-04-03_单样张`
  - 当前内容包含：
    - `102_master_A.png / .aseprite`
    - `102_master_B.png / .aseprite`
    - `102_master_C.png / .aseprite`
    - `102_contact_sheet.png`
- 当前阶段：
  - `targeted probe / 单样张验牌`
- 当前恢复点：
  - 用户下一步只需要判断：
    - `A / B / C` 哪个最接近
    - 还是三张都不过线

## 2026-04-03｜补记：用户给出新参考图后，主刀已改为 `001` 单样张参考学习

- 当前主线目标：
  - 不再围绕失败的 `102` 包继续扩，而是按用户新给的参考图，先把更贴近参考气质的 `001` 做成一张真正可验牌的单图。
- 本轮子任务：
  - 只做 `001`，只落 `1` 张 PNG。
- 本轮实际完成：
  - 已输出：
    - `D:\UUUnity\NPC\_验牌_001_2026-04-03_只看这一张\001_refstudy_v1.png`
  - 这张图改成了：
    - 更高头部占比
    - 右向三分之四脸
    - 单眼、鼻梁、额头、胡子的大块分面
    - 绿披风只做大轮廓
- 当前阶段：
  - `targeted probe / 单样张验牌`
  - 尚未验证通过
- 当前恢复点：
  - 这轮停在等用户验这张 `001_refstudy_v1.png`
  - 如果继续，只能围绕这张图继续精修或继续重画；不允许回到批量线

## 2026-04-04｜补记：已对齐 UI / spring-day1 给 NPC 的最新两份 prompt，当前先停在只读拆分

- 当前主线目标：
  - 结合 `UI / spring-day1 / NPC` 当前真实产物与最新 prompt，明确：
    - 哪些玩家面需求真归 `NPC`
    - `NPC` 现在第一刀最该先做什么
- 本轮实际读取：
  - `2026-04-04_NPC线程_玩家面NPC方向分工与第一刀认领prompt_01.md`
  - `2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`
  - 以及此前已抽查的 `UI / NPC / Day1` 真实代码产物
- 当前稳定判断：
  1. `prompt_01` 是分工裁定刀，不是实现刀。
  2. `prompt_05` 是 NPC own 真实施工刀，但范围严格收窄到：
     - 原剧本角色回正
     - NPC own bubble / pair / content / profile / prefab / runtime probe
  3. 结合现场进度，我现在不该再吞 UI 壳或 Day1 主剧情；
     真正该接的第一刀，应是 `NPC own bubble / speaking-owner / 正式-非正式闭环`。
- thread-state：
  - 本轮仍是只读分析，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前恢复点：
  - 先把三段观后感、第一刀排序和总任务清单对用户说清；
    用户拍板后再决定是否进入真实施工

## 2026-04-05｜补记：已继续真实施工 NPC own 会话/气泡底座，并在外部编译 blocker 下合法停车

- 当前主线目标：
  - 继续只做 `NPC own` 的非正式聊天会话/气泡底座收口；
  - 不改现有气泡主题样式，只统计现代码里到底有几种样式。
- 本轮子任务：
  - 沿用当前 slice：
    - `npc-own-conversation-runtime-fixes-20260405`
  - 回收测试、盘点样式、复核 own 脚本验证，并在无法继续 Unity 自测时合法 `Park-Slice`。
- 本轮实际完成：
  1. 已确认这轮 own 代码面集中在：
     - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
  2. 已确认当前有效功能点：
     - 会话气泡避让收紧；
     - speaking-owner 置顶与前景排序带保留；
     - 背景不透明；
     - ambient 气泡可忽略 hidden stale conversation owner；
     - 自动停留时长公式锁定为 `1 + 0.08 * 字数`。
  3. 已完成样式盘点结论：
     - `4` 种 live 主样式
     - `7` 条工程壳
  4. 已完成脚本级验证：
     - own 脚本 / own 测试脚本 `validate_script` 均 `0 error`
     - `git diff --check` 对 own 文件通过
- 当前 blocker：
  - Unity console 存在外部编译错误：
    - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs(1694,23): error CS0034`
  - 这不是我这轮 own 文件引入的红；
    但它会阻断当前 EditMode tests / runtime probe 的可信推进。
- thread-state：
  - 已跑：
    - `Begin-Slice`（本轮继续施工前已补）
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  - 等 `PlacementManager.cs` 外部编译红解除后，
    直接回到：
    1. fresh console
    2. NPC own targeted tests
    3. NPC own runtime probe
  - 然后再决定是否可以进入 `Ready-To-Sync`

## 2026-04-05｜补记：已复核“NPC 线程遗留项完成度”并统一口径

- 当前主线未变：
  - 仍是 `NPC own` 会话 / 气泡底座与 NPC 本体收口。
- 本轮核实结论：
  1. 我对“哪些已完成、哪些未完成”现在是有把握的；
     - 已完成 = 代码底座和边界
     - 未完成 = runtime 体验闭环与本体收口
     - 待验证 = 被外部编译红挡住的 live 项
  2. 当前不确定的不是分类，而只是 live 何时能恢复。
- 当前恢复点：
  - 后续直接按这套三分法向用户回报，不再把“已写好代码”和“体验已验完”混成一类。

## 2026-04-05｜补记：memory 写口测试占位

- 这是一条临时占位。

## 2026-04-05｜补记：已处理“你有大量报错”这轮现场，NPC own 测试编译红已清零

- 当前主线目标：
  - 继续只做 `NPC own` 的 formal/casual/ambient 优先级、crowd 真值与 pair/ambient bubble 护栏。
- 本轮子任务：
  - 先对用户指出的“你有大量报错”做真实核查；
  - 优先清掉我这轮 own 的测试编译红，不去碰 `Primary.unity`、`GameInputManager.cs`、UI 壳和 Day1 热根。
- 本轮实际完成：
  1. 已确认报错根因是：
     - `Tests.Editor.asmdef` 下的三份 NPC tests 直接强绑 runtime 类型，导致 `CS0246 / CS0103`。
  2. 已重写以下文件：
     - `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - `Assets/YYY_Tests/Editor/NpcCrowdDialogueTruthTests.cs`
  3. 统一改成：
     - 反射取类型 / 方法 / 属性
     - `AssetDatabase.LoadAssetAtPath(path, type)` 非泛型载入
  4. Unity fresh console 已验证：
     - 这批 NPC own 测试编译红已不再出现
- 本轮验证：
  - `sunset_mcp.py errors --count 30 --output-limit 30`
    - 最新 fresh 结果曾返回 `errors=0 warnings=0`
  - `sunset_mcp.py status`
    - 当前剩余是 foreign/runtime/importer 红：
      - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs:260`
      - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
  - `git diff --check`
    - 被 shared foreign 文件拦住，不属于我这轮 own 测试文件自身
- 关键决策：
  - 不再尝试通过 asmdef 扩引用来“掩盖”程序集边界；
  - 继续保留反射测试口径，避免下一轮再因 assembly 切面炸回同类红。
- 恢复点：
  - 下一轮继续 NPC 本线时，直接回到：
    1. fresh console/status
    2. formal/casual/ambient targeted probe
    3. crowd truth / pair bubble 收口
  - 不必再回头处理这组三份测试编译红。

## 2026-04-05｜补记：只读分析 NPC 撞墙静默卡死的最可能代码路径

- 当前主线目标：
  - 只读回答“NPC 为什么会撞墙后不报错、还可能被吞成 pause / 卡在墙边”的代码级原因。
- 本轮子任务：
  - 重点核查 `NPCAutoRoamController.cs`
  - 必要时补读 `NPCMotionController.cs`
- 本轮已完成：
  1. 已定位最明确的吞没分支：
     - `CheckAndHandleStuck()` 在 terminal stuck 场景会先走 `TryHandleTerminalStuck()`；
     - `TryHandleTerminalStuck()` 连续命中阈值后直接 `EnterLongPause()`，不会留下 `TryInterruptRoamMove(StuckCancel)` 的显性中断记录。
  2. 已定位第二类吞没分支：
     - `TickShortPause()` / `TickLongPause()` 的 `TryBeginMove()` 失败只会重新进 `ShortPause`，会把“贴墙后还是采样不到新路”的现场继续吞成 pause 循环。
  3. 已定位最可能造成“还卡墙但不显性报错”的组合链：
     - `TickMoving()` 在真正执行 `rb.MovePosition(nextPosition)` 之前，就先 `NoteSuccessfulAdvance(currentPosition)`；
     - 如果最终被物理碰撞挡住，`NPCMotionController.ResolveVelocity()` 又会优先返回 `_externalVelocity`，于是动画 / `IsMoving` 仍像在走，但真实位置没推进。
  4. 已补充确认 shared avoidance 风险：
     - 动态 blocker 的 hard-stop 分支会刷新 progress checkpoint，更容易把零推进伪装成“正常让行等待”。
- 关键判断：
  - 这轮我最确信的不是“所有卡墙都只有一个原因”，而是：
    - 当前代码里确实存在“先把零推进包装成正常 moving / normal pause，再错过显性 interruption”的链。
- 验证状态：
  - `静态推断成立`
  - 无代码修改、无 Unity live 验证
- thread-state：
  - 本轮未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
    - 原因：始终停留在只读分析
- 当前恢复点：
  - 如果继续这条线，最小实现切口应先落：
    1. `TryHandleTerminalStuck()`
    2. `TickMoving()` 或 `TryHandleSharedAvoidance()`
## 2026-04-05｜补记：NPC mainline 已把 002/003 的 casual 门禁从“全局误杀”修回对象级 takeover，并拿到 4 条 live 硬证据
- 当前主线目标：
  - 延续 NPC mainline slice，继续只做非正式聊天会话底座、会话中断链与 NPC 气泡本体；
  - 不碰 Primary.unity、GameInputManager.cs、PromptOverlay / Workbench / 左下角 shared UI 壳。
- 本轮子任务：
  - 先 fresh 核查这轮自己有没有新红；
  - 再定位  02 / 003 还是起不了聊的真实原因；
  - 最后把门禁修正后直接用 live targeted probe 复测闭环与玩家首句中断。
- 本轮实际完成：
  1. fresh 核查结果：
     - py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20
       - errors=0 warnings=0
     - py -3 scripts/sunset_mcp.py status
       - baseline=pass
       - console 无 blocking error
     - git diff --check -- Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs
       - 通过
  2. 代码修正：
     - Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs
       - 新增对象级 formal takeover 口径；
       - 只有同一个 NPC 自己的 NPCDialogueInteractable 当前可接管时，才 suppress casual。
     - Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs
       - CanInteract / TryHandleInteract / ReportProximityInteraction 全部改走对象级 suppression；
       - context == null 时自动 fallback 到 BuildInteractionContext()；
       - proximity 早退统一补 NpcWorldHintBubble.HideIfExists(transform)。
     - Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs
       - 测试矩阵改成贴近真实需求：
         - formal phase 下，没挂 same-object formal dialogue 的 NPC 仍允许 casual；
         - 挂了 same-object formal dialogue 的 NPC 才应让位。
  3. live targeted probe：
     - 使用 CodexEditorCommandBridge + DialogueDebugMenu bootstrap 做干净入口；
     - 直接在 CrashAndMeet + Dialogue=idle 的 bootstrap 现场验证：
       -  02 可起聊；
       -  03 可起聊。
     - 后续单独 trace 结果：
       -  02 两轮 casual 闭环完成，endReason=Completed
       -  02 PlayerTyping Interrupt 跑开中断完成，playerExitSeen=True、
pcReactionSeen=True、leavePhase=PlayerSpeaking
       -  03 两轮 casual 闭环完成，endReason=Completed
       -  03 PlayerTyping Interrupt 跑开中断完成，playerExitSeen=True、
pcReactionSeen=True、leavePhase=PlayerSpeaking
- 关键判断：
  - 这轮最核心的 bug 不是“中断链没写”，而是“002/003 先被全局门禁误杀，后面的自动续聊/跑开中断根本进不了链”；
  - 现在这层已经修回来了。
- 验证状态：
  - 线程自测已过：
    - 代码层 green
    - live targeted probe green
  - 用户终验：
    - 尚未进行
- thread-state：
  - 本轮继续真实施工前沿用了已登记 slice；
  - 收尾已跑：
    - Park-Slice
  - 未跑：
    - Ready-To-Sync
  - 当前 live 状态：
    - PARKED
- 当前恢复点：
  - 如果下一轮继续 NPC 本线，直接从“用户可感知体验复核 + NPC own 细节补口”继续，不必再回头排查 002/003 为何完全起不了聊。

## 2026-04-05｜更正：上一条线程补记中的 002/003 脏字样由本条校正

- 线程最终确认的稳定事实：
  1. NpcInteractionPriorityPolicy 已改成 same-object formal takeover only。
  2. NPCInformalChatInteractable 已全面接入对象级 suppression。
  3. 002 闭环通过，002 PlayerTyping Interrupt 通过。
  4. 003 闭环通过，003 PlayerTyping Interrupt 通过。
  5. 当前 live 状态已 Park-Slice，后续可直接从用户体验复核继续。

## 2026-04-05｜补记：本线程继续深推 `pair ambient`，已把新 8 人 crowd 的 pair pass 和静态群众口径一起收住

- 当前主线目标：
  - 继续只做 `NPC own`：
    - `pair ambient / bubble`
    - `crowd data / bootstrap / prefab truth`
  - 不回吞 UI、Day1 导演、Primary、Town、GameInputManager。
- 本轮子任务：
  - 先把 `pair=0/2` 这个 runtime blocker 真正压掉；
  - 再把 `101~301` 的内部 token 与 bootstrap 口径继续降回群众层。
- 本轮完成：
  1. `NPCBubblePresenter.cs`
     - ambient show 前会自动释放 stale prompt suppression。
  2. `NPCAutoRoamController.cs`
     - ambient bubble 增加短 retry，避免单帧 race 吞掉气泡。
  3. `SpringDay1NpcCrowdValidationMenu.cs`
     - pair probe 前主动清 suppression / conversation owner；
     - pair line 比较改为去换行后再比；
     - probe root 去掉 `HideFlags.DontSave`，清掉退出时 Assert。
  4. `NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 stale suppression 回归测试。
  5. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 8 组 crowd 的内部 `m_Name` 已统一降级为群众/线索/夜间见闻 token。
  6. `SpringDay1NpcCrowdBootstrap.cs`
     - bootstrap token、`displayName`、`roleSummary` 已同步降级。
  7. `NpcCrowdDialogueTruthTests.cs`、`NpcCrowdPrefabBindingTests.cs`
     - 测试真值已同步。
- 本轮验证：
  - direct `validate_script`：
    - 以上脚本全部 `0 error`
  - `git diff --check`：
    - own 范围通过
  - fresh runtime targeted probe：
    - `instance=8/8`
    - `informal=8/8`
    - `pair=2/2`
    - `walkAway=2/2`
  - fresh CLI console：
    - `errors=0 warnings=0`
  - direct console：
    - 只剩 `There are no audio listeners in the scene` warning
- 关键判断：
  - `pair ambient` 这条原 blocker 已经被拿下；
  - 当前下一步不该再回头卡在“pair 为什么不亮”，而该转回用户体验复核和更细的 crowd 内容语义。
- thread-state：
  - 本轮沿用了既有 slice；
  - 已跑：
    - `Park-Slice`
  - 当前 `.kiro/state/active-threads/NPC.json` 为 `PARKED`
  - `Show-Active-Ownership.ps1` 同轮仍短暂显示 `ACTIVE`，判断为展示层滞后。
- 当前恢复点：
  - 下轮若继续，优先做：
    1. 用户可感知的 pair / informal / walk-away 体验复核
    2. 仍待细修的 crowd 内容语义
    3. sync 前 own 范围与 dirty 收口

## 2026-04-05｜补记：本轮又把 ambient/formal 优先级与 crowd manifest 护栏往前推了一刀

- 当前主线目标：
  - 继续只做 `NPC own`：
    - formal/casual/ambient priority
    - crowd duty/anchor/phase 真值
  - 不碰 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Story/Interaction/NpcInteractionPriorityPolicy.cs`
     - ambient suppress 现在只在：
       - formal dialogue active
       - 当前 focus 确认是可接管的 formal NPC
       时生效；
     - 不再把 formal phase 整段做成 ambient 全灭。
  2. `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - 新增：
       - formal active 应压 ambient
       - formal phase 无接管应保留 ambient
       - formal prompt focus 应压 ambient
  3. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - phase 矩阵回正为“formal 直接压 casual”；
     - ambient 无接管保留逻辑补断言。
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - 从 3 个样本抽查补到 8 人 exact matrix；
     - 现会校验：
       - duty
       - semantic anchor
       - growth intent
       - min/max phase
- 本轮验证：
  - `manage_script validate` clean：
    - `NpcInteractionPriorityPolicy.cs`
    - `NpcAmbientBubblePriorityGuardTests.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `NpcCrowdManifestSceneDutyTests.cs`
  - fresh compile/console：
    - 清 console 后重新 request compile
    - `py -3 scripts/sunset_mcp.py status` => `isCompiling=false`
    - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
  - Unity 验证：
    - `Tools/NPC/Spring Day1/Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe` => `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
    - 本轮已回到 Edit Mode，未把 Unity 留在 Play。
- thread-state：
  - 本轮已跑：
    - `Begin-Slice`
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 slice：
    - `npc-own-priority-and-crowd-runtime-closure-20260405`
  - 当前状态：
    - `PARKED`
- 当前判断：
  - 这轮之后，NPC own 剩余高价值工作已明显缩窄到：
    1. crowd 内容语气/强度微调
    2. 用户可感知体验终验
  - 不需要再回头重修 pair blocker，也不需要再把 ambient global suppress 当成现有主问题。

## 2026-04-05｜补记：NPC own 剩余测试契约与 EditMode 气泡底座已收平，本轮收口后改为 PARKED

- 当前主线目标：
  - 继续只做 NPC own 的会话/气泡底座、crowd 内容与 targeted probe；
  - 不碰 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮子任务：
  - 核实 `Tests.Editor` 里疑似残留的 NPC own 失败到底是不是 stale；
  - 只修属于 NPC own 的测试夹具和 EditMode 气泡底座。
- 本轮完成：
  1. `Assets/YYY_Tests/Editor/NpcAmbientBubblePriorityGuardTests.cs`
     - 将 `InteractionContext` 解析固定到 `Assembly-CSharp`；
     - 避免再误撞 `UnityEditor.InteractionContext`。
  2. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - EditMode 不再在 `Awake / OnValidate` 抢先建 UI；
     - 改为真正显示气泡时再懒创建，清掉 EditMode tests 的 `SendMessage` 噪音；
     - 未改气泡样式，只收初始化时机。
- 本轮验证：
  - `validate_script`：
    - `NPCBubblePresenter.cs` -> `0 error`
    - `NpcAmbientBubblePriorityGuardTests.cs` -> `0 error`
  - `git diff --check` 对本轮 touched files 通过。
  - targeted tests PASS：
    - `NpcAmbientBubblePriorityGuardTests.FormalPriorityPhase_ShouldHideVisibleAmbientBubble_WhenFormalPromptOwnsCurrentFocus`
    - `NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldMakeBubbleVisible`
    - `PlayerThoughtBubblePresenterStyleTests.ConversationLayout_ShouldStayCloseToSpeakerHeads_WhileKeepingReadableSeparation`
    - `PlayerThoughtBubblePresenterStyleTests.ReadableHoldSeconds_ShouldFollowFixedPacingFormula`
    - `NPCInformalChatInterruptMatrixTests.ResumeIntroPlan_ShouldReturnContinuityLines_ForBlockingUiResume`
  - full `Tests.Editor` 后查 `C:\Users\aTo\AppData\LocalLow\DefaultCompany\Sunset\TestResults.xml`：
    - `NpcAmbientBubblePriorityGuardTests` = `4/4 Passed`
    - `NpcBubblePresenterEditModeGuardTests` = `5/5 Passed`
    - `NPCInformalChatInterruptMatrixTests` = `16/16 Passed`
    - `PlayerThoughtBubblePresenterStyleTests` = `7/7 Passed`
    - 当前剩余失败均为 foreign：`Occlusion / ScenePartialSync / SpringDay1 Prompt/Director/Workbench`
  - Unity / MCP：
    - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe` => `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
    - `Tools/NPC/Spring Day1/Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
    - 清 console 后 `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20` => `errors=0 warnings=0`
    - 本轮结束已回到 Edit Mode。
- 当前判断：
  - `NPC own` 当前已经不再卡在“剩余测试契约没收平”；
  - 代码层 / targeted probe 层这轮可视为再次站住；
  - 再往下就是：
    1. crowd 内容语气 / 强度细修
    2. 用户真实体验终验
    不适合继续盲补结构。
- thread-state：
  - 本轮沿用 slice：`npc-own-remaining-test-contract-closure-20260405`
  - 已跑：
    - `Park-Slice`
  - 未跑：
    - `Ready-To-Sync`
  - 当前 live 状态：
    - `PARKED`
- 当前恢复点：
  - 若后续继续 NPC 本线，直接从：
    1. crowd 文案微调
    2. `pair / informal / walk-away` 体感终验
    继续；
  - 不需要再回头把这组 NPC own fixture 当成 blocker。

## 2026-04-05｜补记：额外只读复核再次确认旧 interrupt/style fail 更像旧快照，不像当前代码真失败

- 本轮补充结论：
  1. 只读核查 `ResumeIntroPlan` / `PlayerThoughtBubblePresenterStyleTests` 相关点后，结论与本轮实际验证一致：
     - 当前磁盘代码和当前已编 `Assembly-CSharp.dll` 已对上测试要求；
     - 旧失败列表更像旧 runner / 旧编译快照。
  2. 因此后续若继续 NPC 本线，不应再为这批旧 failure 盲改：
     - `CreateFallbackResumeIntroPlan()`
     - `ApplyPlayerBubbleStylePreset()`
     - `FormatBubbleText()`
     除非 fresh runner 再次给出新的真实失败。

## 2026-04-05｜补记：继续完成 crowd 内容深推，补齐 dialogue 刷新链与 crowd-specific interrupt/resume，当前已停车

- 当前主线目标：
  - 继续只做 NPC own 的 crowd 内容语气细修、crowd dialogue 资产刷新链与 targeted runtime 复核；
  - 不碰 `Primary.unity / Town.unity / SpringDay1Director.cs / SpringDay1NpcCrowdDirector.cs / UI / GameInputManager.cs`。
- 本轮子任务：
  - 把 `SpringDay1NpcCrowdBootstrap.cs` 中未真正落到资产的 crowd 文案同步到 8 份 legacy dialogue asset；
  - 把 `defaultInterruptRules / defaultResumeRules` 从空数组补成人设化内容；
  - 给这条 crowd 刷新链补测试护栏，并避免 slug 改名后重复生资产。
- 本轮实际完成：
  1. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 新增 `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets`；
     - `101~301` 的 crowd 文案继续收口；
     - 新增 8 人 default `interrupt / resume` 文案；
     - 新增稳定旧 `AssetStem` 映射，锁住 legacy dialogue / roam 资产路径。
  2. `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
     - 扩充 stale crowd 口气黑名单；
     - 新增 default rules 不得为空和 refresh 菜单存在的护栏。
  3. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 8 份 legacy dialogue asset 已刷新到最新 crowd 文案与 default rules；
     - 误生出的新 slug 重复资产已在 own root 内清理干净。
- 本轮验证：
  - `manage_script validate` clean：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
    - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
  - `git diff --check` 对 own 范围通过。
  - `Editor.log`：
    - `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - `Validate New Cast` 后 fresh console => `errors=0 warnings=0`
  - fresh runtime probe 复跑：
    - 已真实进入 Play，再回到 Edit；
    - 但这次只拿到 `START`，没拿到新的 `PASS / FAIL`；
    - console 同轮混入 foreign：
      - `ExecuteMenuItem failed because there is no menu named 'Sunset/Story/Validation/Run Director Staging Tests'`
      - `PersistentManagers DontDestroyOnLoad` editor exception
    - 因此这轮 runtime probe 只可报“attempted but polluted by foreign noise”，不能继续 claim clean pass。
- 当前阶段判断：
  - NPC own 当前不再缺 crowd 资产刷新链、interrupt/resume 落盘链、或 legacy 路径稳定性这类代码层补口；
  - 剩余主要是：
    1. foreign 噪音停下后的 fresh runtime targeted probe
    2. 用户真实体验终验与极少量语气微调
- thread-state：
  - 本轮沿用 slice：`npc-own-crowd-content-deep-polish-20260405`
  - 已跑：`Begin-Slice`、`Park-Slice`
  - 未跑：`Ready-To-Sync`
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 若后续继续 NPC 本线，先别再扩 shared / UI / Day1；
  - 先等 foreign 串台停下后补一次 clean runtime targeted probe；
  - 再决定是否进入用户体验终验或 sync 前收口。

## 2026-04-05｜补记：子智能体已关闭，当前对用户的稳定口径已收成“已完成 / 未完成 / 下一步最深可压”

- 用户本轮额外要求：
  - 先确认子智能体是否还要用；
  - 再用最短大白话说明：
    1. 这轮已完成哪些需求
    2. NPC 历史全局还剩哪些未完成
    3. 下一步最深能做到哪里
- 当前稳定结论：
  1. 子智能体已关闭，不再占用。
  2. 本轮代码层真正完成的是：
     - crowd dialogue 刷新链
     - 8 人 default interrupt/resume 人设化文案
     - legacy asset 路径稳定化
     - stale crowd 口气与空 rules 的测试护栏
     - 8 份 legacy dialogue asset 已刷新并清掉重复资产
  3. 当前 NPC 历史全局剩余主要只剩：
     - clean runtime targeted probe 再补一轮
     - 用户真实体验终验
     - 必要时极少量 crowd 语气微调
  4. 下一步最深可压到：
     - 如果外部串台停下，可直接把 clean runtime targeted probe 补完；
     - 再往下就不该继续盲写结构代码，而应转体验验收与最后少量文案修边。

## 2026-04-05｜补记：用户开放 Unity 现场后，已完成 quick live retest 并再次停车

- 当前主线目标：
  - 继续只做 NPC own 的 crowd 内容与 runtime 验证收口；
  - 不碰 `Primary / Town / Day1 Director / UI / GameInputManager`。
- 本轮快速测试：
  1. 重新 `Begin-Slice`
  2. `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  3. 重新进 Play 跑 `Run Runtime Targeted Probe`
     - `Editor.log` 已拿到新的 evidence 与终行：
       - `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  4. fresh console：
     - `errors=0 warnings=0`
  5. 最后显式 `STOP`，Unity 已回到 Edit Mode
- 当前阶段判断：
  - NPC own 这段现在已经不再缺 targeted runtime 证据；
  - 历史上剩下的核心未完成项，主要只剩用户真实体验终验；
  - 若体验里再挑出 crowd 文案问题，才需要最后一小轮微调。
- thread-state：
  - 本轮 slice：`npc-quick-clean-runtime-retest-20260405`
  - 已跑：`Begin-Slice`、`Park-Slice`
  - 未跑：`Ready-To-Sync`
  - 当前状态：`PARKED`

## 2026-04-05｜补记：用户要求从多个角度评价 day1 自续工 prompt，并判断 Town 是否跟得上

- 本轮只读分析目标：
  - 评估 `spring-day1` 当前给自己的续工单是否方向正确；
  - 从 NPC own 已完成度出发，判断 `Town` 现在能不能跟上 day1；
  - 明确 NPC 在 Day1 里的份量现在能不能担得起。
- 当前核心判断：
  1. day1 prompt 本身是稳的：
     - 没有回到方案模式；
     - 目标明确落在“导演工具 live 接入 + 后半段导演消费下沉”；
     - 对 Town 的口径也足够克制，没有装成全闭环。
  2. 从 NPC own 完成度看，NPC 现在担得起 Day1 后半段群像承接这份量：
     - 8 人 crowd cast / asset / probe / pair / informal / walk-away / priority 底座都已经站住；
     - 所以 NPC own 不再是 `Town` 轻量承接的主 blocker。
  3. Town 当前能跟上的范围：
     - `EnterVillageCrowdRoot`
     - `KidLook_01`
     - `NightWitness_01`
     - `DinnerBackgroundRoot` 可继续下一层
     - `DailyStand_01` 在前面稳住后也可跟
  4. Town 当前不能被当成 fully ready 的部分：
     - 不能泛化到整张 Town 的复杂多人导演编排；
     - 不能跳过具体 anchor/cue/live 保存，直接宣称后半段全闭环。
  5. 我现在更担心的真实风险，不在 NPC 底座，而在 day1 自己的导演 live 工具链：
     - `Run Director Staging Tests` 菜单桥接识别问题
     - 排练录制写回的长期稳定性
     - 外部噪音不要污染导演验证
- 结论：
  - `Town` 现在跟得上 `day1`，但只能按“轻量、锚点优先、live 一个个打穿”的方式跟；
  - NPC 自己这边当前已经能扛住这部分份量，但不会把“NPC ready”冒充成“day1 全完工”。

## 2026-04-05｜补记：对 day1 prompt 的最终大白话判断已稳定

- 用户这轮真正要的不是文档，而是判断：
  1. day1 方向对不对；
  2. NPC 现在担不担得起；
  3. Town 到底跟不跟得上。
- 我给出的稳定判断：
  - day1 方向对，重点应该继续压导演工具 live 接入和 anchor 消费；
  - NPC 现在担得起自己这部分份量，已经不是主要底层 blocker；
  - Town 跟得上，但只是在 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01 / DinnerBackgroundRoot / DailyStand_01` 这类轻量承接口径上跟得上，不是整张后半段舞台已经 ready。
- 这轮性质：
  - 只读分析；
  - 不开工；
  - 不跑 `Begin-Slice`；
  - 当前 thread-state 维持 `PARKED`。

## 2026-04-05｜补记：Day1 后半段群像内容并行续工已完成一轮真实施工

- 用户新要求：
  - 结合 `spring-day1` 最新 prompt 与我自己线程现状，不要只机械做 prompt；
  - 在不碰导演工具、Town runtime、UI、Primary、GameInputManager 的前提下，继续做 `Day1` 后半段群像内容层；
  - 允许使用 subagent，但用完即关。
- 本轮真实动作：
  1. 先 `Begin-Slice`，进入 `ACTIVE`。
  2. 完整读取：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_NPC线程_day1后半段群像内容并行续工prompt_03.md`
  3. 确认这轮最值钱落点仍是：
     - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
     - `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
  4. 真实改动：
     - `101 / 103`：补 `EnterVillage / DailyStand` 的停手围观、偷看、压低嗓子、照常站位感
     - `104 / 201 / 202 / 203`：补 `Dinner / ReturnAndReminder / DailyStand` 的桌上压力、回屋、低声议论、照旧开门/摆摊/开火感
     - `102 / 301`：补 `NightWitness / DayEnd` 的后坡、夜路、脚步、回声、规矩感
     - 新增资产级语义护栏测试，防止 crowd 内容回退成空壳
  5. 通过命令桥真跑：
     - `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets`
     - `Tools/NPC/Spring Day1/Validate New Cast`
     - 先进 `Play`
     - 再跑 `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
     - probe 自动收尾并退回 `Edit Mode`
- 本轮验证真值：
  - `manage_script validate --name SpringDay1NpcCrowdBootstrap --path Assets/Editor/NPC --level standard` => clean
  - `manage_script validate --name NpcCrowdDialogueNormalizationTests --path Assets/YYY_Tests/Editor --level standard` => clean
  - `git diff --check -- Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs Assets/111_Data/NPC/SpringDay1Crowd` => 通过
  - `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - runtime probe 新一轮 `PASS` 计数从 `1 -> 2`
  - 最新终行：
    - `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  - Unity 已回到 `Edit Mode`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - summary = `errors=0 warnings=0`
    - 但结果体还带 1 条未被 error/warning 计数的 assert 型 editor 噪音，已报实，不包装成绝对纯净控制台
- 当前判断：
  - 这轮不只是“day1 可以继续吃 NPC”；
  - 而是后半段 crowd 内容现在真的已经有一层更具体的可消费语义了；
  - 当前最合理的下一步不再是盲补底座，而是等用户做体验终验，再按反馈定点微调某个 scene-duty。

## 2026-04-06｜补记：用户指出“NPC 不在村里而是突然生成”，已确认这是 crowd director/runtime contract 问题

- 用户新增问题：
  - 新 NPC 应该本来就在村子里 / `Town` 里；
  - 现在看起来像是到对话或阶段时才突然蹦出来；
  - 需要我判断：
    1. 这是谁做的
    2. 该怎么办
    3. 我现在应不应该接这件事
- 本轮只读核查结果：
  1. 罪魁祸首不是内容资产，而是：
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  2. 已确认的机制：
     - 只在 `Primary` 工作
     - 非 `Primary` 会 `TeardownAll()`
     - 缺 actor 时 `Instantiate(entry.prefab, spawnPosition, ...)`
  3. 因此用户看到的“突然生成”是真问题，但它属于 deployment/runtime contract，不是 crowd 文本层 bug。
  4. 以当前边界判断，我现在默认不该直接吞这件事；
     - 因为它会越到：
       - `SpringDay1NpcCrowdDirector.cs`
       - `Town runtime contract`
       - 可能还有 `Town/Primary` 的常驻摆位策略
     - 这不属于我上一刀被授权的“Day1 后半段群像内容层”。

## 2026-04-06｜补记：已按用户要求给 day1 写完回执信与引导 prompt

- 本轮用户新要求：
  - 不只是口头总结；
  - 要把：
    1. 我前几轮做完的相关内容
    2. 已吸收的 day1 相关真值
    3. 最近这轮对“NPC 移民/驻村部署”的思考
  - 正式写成给 day1 的回执信文档；
  - 再补一份可直接转发的引导 prompt。
- 本轮已落文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_后半段群像回执与驻村部署问题汇总_09.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_读取回执并判断crowd驻村部署prompt_10.md`
- 本轮状态：
  - 只做文档交接；
  - 已 `Begin-Slice` 后 `Park-Slice`；
  - 当前 `thread-state = PARKED`；
  - 没有继续进入新的代码施工。

## 2026-04-06｜补记：驻村常驻矩阵与 formal 一次性消费已一起落地

- 用户后续继续给了 `prompt_11 + prompt_12`，要求我不要停在解释，而是把 `101~301` 真正压成 day1 可直接吃的 resident 语义矩阵，并补上“formal 只能消费一次”的硬边界。
- 本轮真实施工：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
     - 新增 resident baseline / presence level / beat flags / beat helper methods；
  2. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 为 8 人补齐 6 段 resident beat 语义；
     - 刷新 manifest 资源；
  3. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - resident baseline / beat semantics / presenceLevel 都进了验证；
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - resident matrix 和 helper contract 护栏落地；
  5. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - formal 剧情一旦消费过，不再重播 formal / followup；
  6. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - formal 已消费后，同 NPC 应回落 informal 的回归测试已补；
  7. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 文本级护栏已补，防止 formal 重播逻辑回流。
- 当前稳定结论：
  - 白天默认常在：`101 / 103 / 201 / 203`
  - 白天背景常在：`104 / 202`
  - 白天低可见、夜里更强：`102 / 301`
  - 优先给 day1 吃回的 beat：
    1. `EnterVillage_PostEntry`
    2. `DinnerConflict_Table`
    3. `FreeTime_NightWitness`
- 本轮验证：
  - `Refresh Crowd Resident Manifest` 成功
  - `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
  - touched scripts `manage_script validate` 全 clean
  - `git diff --check` 通过
  - fresh console `errors=0 warnings=0`
  - Unity 已回到 `Edit Mode`
- 当前阶段：
  - `NPC own` 这边已经把 resident semantic contract + formal once-only contract 压到头；
  - 下一步真正该由 day1 往前狠狠干的是 deployment / director consumption，而不是我再继续吞 runtime。

## 2026-04-06｜补记：只读回答“NPC 线程还可补哪层更方便 day1 直接消费”

- 当前主线目标：
  - 用户要求在不碰 `SpringDay1NpcCrowdDirector.cs`、`Town/Primary scene`、`UI`、`GameInputManager` 的前提下，
    只读判断 NPC 线程还可补哪层 helper / contract / test，最值钱且不越界。
- 本轮子任务：
  - 读取：
    - `SpringDay1NpcCrowdManifest.cs`
    - `NPCDialogueInteractable.cs`
    - `SpringDay1DirectorStaging.cs`
    - 以及对应 tests / workspaces memory
  - 收敛最多 3 条具体建议。
- 本轮稳定结论：
  1. `SpringDay1NpcCrowdManifest` 现在已经有 entry 级 resident helper，但缺一个 beat 级消费快照；
     day1 如果要直接吃 roster，当前仍要自己拆多次查询。
  2. `NPCDialogueInteractable` 的 formal once-only 行为虽然已站住，但没有公开的只读消费状态；
     day1 / validation 只能间接从 `CanInteract()` 或私有逻辑推断。
  3. 当前缺的最关键护栏是桥接测试：
     - manifest resident matrix
     - stage book cue
     - formal consumed -> informal/resident fallback
     这三层还没有独立成一组对齐断言。
- 收敛后的 3 条建议：
  1. 先补 `SpringDay1NpcCrowdManifest.cs` 的 beat-consumption helper。
  2. 再补 `NPCDialogueInteractable.cs` 的 public read-only formal state contract。
  3. 最后补 manifest <-> stagebook <-> formal fallback 的 bridge tests。
- 明确不建议：
  - 不建议继续补 resident parent / active / root 之类 helper；
  - 那已经逼近 `CrowdDirector` runtime deployment 边界。
- 验证状态：
  - 纯静态代码 / 文档取证；
  - 未改任何业务文件；
  - 未跑 `Begin-Slice`
    - 原因：本轮始终是只读分析。
- 当前恢复点：
  - 如果后续用户真要我继续 NPC 线里的“最后一层 day1 友好收口”，就按上面 `1 -> 2 -> 3` 的顺序开。

## 2026-04-06｜续工结果：NPC own 的 beat roster helper 与 formal state contract 已做完

- 当前主线：
  - 继续承接 `prompt_11 / prompt_12`；
  - 只做 NPC 本线；
  - 把 `101~301` 压成 day1 可直接消费的 resident contract；
  - 同时守住 formal 一次性消费后只回落 informal / resident。
- 本轮子任务：
  1. 补 manifest 的 beat-consumption helper
  2. 补 `NPCDialogueInteractable` 的 public formal-state contract
  3. 补对应 tests / validation，不吞 director/runtime
- 本轮实际做成：
  1. `SpringDay1NpcCrowdManifest.cs`
     - 新增 `SpringDay1CrowdDirectorConsumptionRole`
     - 新增 `TryGetEntry()`
     - 新增 `GetEntriesForDirectorConsumptionRole()`
     - 新增 `BuildBeatConsumptionSnapshot()`
     - 把 `priority / support / trace / backstagePressure` 真正拆出来给 day1 直接拿
  2. `SpringDay1NpcCrowdValidationMenu.cs`
     - `Validate New Cast` 现在会查 resident direct-consumption roster
  3. `NPCDialogueInteractable.cs`
     - 新增 `NPCFormalDialogueState`
     - 新增 `HasFormalDialogueConfigured`
     - 新增 `GetFormalDialogueStateForCurrentStory()`
     - 新增 `HasConsumedFormalDialogueForCurrentStory()`
     - 新增 `WillYieldToInformalResident()`
  4. 测试护栏
     - `NpcCrowdManifestSceneDutyTests.cs`
     - `NpcInteractionPriorityPolicyTests.cs`
     - `SpringDay1DialogueProgressionTests.cs`
- 本轮真实排障：
  - 初次重跑 `Validate New Cast` 报 48 条 `presenceLevel = None`
  - 查实是 `SpringDay1NpcCrowdManifest.asset` 旧脏值，不是新 helper 逻辑错
  - 已跑 `Refresh Crowd Resident Manifest` 刷回
  - 后续又修掉“离屏 + KeepRoutine 被误算成前台 signal”的角色层判断
  - 再补 `Assets/Refresh`，避免 Unity 继续拿旧编译快照跑菜单
- 本轮验证：
  - `manage_script validate` clean：
    - `SpringDay1NpcCrowdManifest.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NPCDialogueInteractable.cs`
    - `NpcCrowdManifestSceneDutyTests.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `git diff --check` 通过
  - fresh console：
    - `errors=0 warnings=0`
  - `Tools/NPC/Spring Day1/Validate New Cast`
    - 最新结果：`PASS | npcCount=8 | totalPairLinks=16`
  - Unity 始终保持 / 回到 `Edit Mode`
- 当前边界：
  - 没碰：
    - `SpringDay1NpcCrowdDirector.cs`
    - `Town / Primary scene`
    - `UI`
    - `GameInputManager`
    - director runtime 本体
- 当前恢复点：
  - NPC own 这轮已基本压完
  - 真正剩余主 blocker 已收敛成 day1 deployment / director consumption / Town 常驻落位

## 2026-04-06｜续工结果：resident-stagebook bridge tests 也已补完并拿到 3/3 PASS

- 当前主线：
  - 用户让我继续把未完成的内容往下压
  - 在 NPC own 范围内，最后还值得补的就是 resident manifest 与 day1 stagebook 之间的桥接护栏
- 本轮子任务：
  1. 新建 bridge test 文件
  2. 给这组 tests 补一个 NPC 自己可直接跑的菜单
  3. 真跑一次拿结果
- 本轮实际做成：
  1. `Assets/YYY_Tests/Editor/NpcCrowdResidentDirectorBridgeTests.cs`
     - 新增 3 个 bridge tests
  2. `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
     - 新增菜单：
       - `Tools/NPC/Spring Day1/Run Resident Director Bridge Tests`
  3. 最新结果文件：
     - `Library/CodexEditorCommands/npc-resident-director-bridge-tests.json`
     - `status=passed`
     - `total=3`
     - `passed=3`
     - `failed=0`
- 本轮真实排障：
  - 菜单第一次失效不是路径错，而是我自己新写的菜单脚本有编译错
  - 已修掉 `TestStatus` 可空链写法
  - 重新 `Assets/Refresh` 后菜单可用
- 本轮验证：
  - `manage_script validate` clean：
    - `NpcCrowdResidentDirectorBridgeTests.cs`
    - `NpcResidentDirectorBridgeValidationMenu.cs`
  - `git diff --check` 通过
  - fresh console：
    - `errors=0 warnings=0`
- 当前判断：
  - NPC own 现在基本真的压到头了
  - 再继续写就会明显跨到 day1 / deployment / Town scene 承接
- 当前恢复点：
  - 如果没有新授权，这条线应准备停车
  - 主 blocker 继续是：
    - day1 deployment / director consumption / Town 常驻落位

## 2026-04-06｜续工补记：已给 day1 落全量承接回执，并准备先 Park 当前 slice

- 当前主线：
  - 用户要求我先停在“可完整汇报给 day1 的刀口”，把这几轮 `NPC` 已完成进度、思考、边界和最深可协作层完整交给 `day1`；
  - 本轮不再继续扩新的 `NPC` 代码。
- 本轮子任务：
  1. 汇总 `NPC` 当前真实完成度
  2. 写一份足够让 `day1` 做分配和调度的详细回执
  3. 更新 memory 并把当前 slice 收到 `PARKED`
- 本轮实际完成：
  1. 新增回执文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_全量进度与承接边界回执_14.md`
  2. 文档中已完整写清：
     - resident semantic matrix / beat consumption helper / formal once-only public contract / bridge tests 的真实完成度
     - 当前真正 blocker 已转到 `day1` 的 resident deployment / director consumption / Town 常驻落位
     - 我最深还能继续帮的层次：`manifest / content profile / formal fallback / tests / targeted probe`
     - 我明确不该默认回吞的层次：`CrowdDirector / Town runtime / scene / UI`
- 当前判断：
  - `NPC` own 已基本压到头；
  - 没有新授权前，再继续往下写会明显越界。
- 验证状态：
  - 本轮只新增回执文档与 memory；
  - 之前最近一次业务真值仍然是：
    - `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
    - `Run Resident Director Bridge Tests` = `3/3 PASS`
    - fresh console = `errors=0 warnings=0`
    - Unity 保持 `Edit Mode`
- 当前恢复点：
  - 现在执行 `Park-Slice`
  - 等 `day1` 审完回执再决定是否重新 `Begin-Slice`

## 2026-04-06｜续记：已把 phase-aware resident nearby 落完，并同步 day1 / 存档系统双回执

- 当前主线目标：
  - 在不越界去吞 `SpringDay1Director / SpringDay1NpcCrowdDirector / Town / UI / Primary` 的前提下，
  - 继续把 `formal consumed -> resident fallback` 从“闲聊 bundle”扩到“玩家靠近 NPC 的 nearby 轻反馈”，
  - 然后在最近安全点写给 `day1` 和 `存档系统` 的两份回执并停表。
- 本轮实际完成：
  1. `Assets/YYY_Scripts/Data/NPCDialogueContentProfile.cs`
     - 新增 `PhaseNearbySet / phaseNearbyLines / TryGetPhaseNearbySet / GetPlayerNearbyLines(relationshipStage, storyPhase)`
  2. `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
     - 新增 phase-aware nearby 透传
  3. `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
     - nearby 轻反馈改为按当前 `StoryPhase` 读 resident nearby lines
  4. `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
     - 新增 `BuildPhaseNearbyPayloads / BuildPhaseNearbySets / PhaseNearbyPayload`
     - 8 份 `Assets/111_Data/NPC/SpringDay1Crowd/*DialogueContent.asset` 已刷新写入 `phaseNearbyLines`
  5. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - 新增 `ValidatePhaseNearbyCoverage`
  6. tests：
     - `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  7. 回执文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_阶段安全点回执_15.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
- 本轮验证：
  - `manage_script validate` clean：
    - `NPCDialogueContentProfile.cs`
    - `NPCRoamProfile.cs`
    - `PlayerNpcNearbyFeedbackService.cs`
    - `SpringDay1NpcCrowdBootstrap.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `NpcCrowdDialogueNormalizationTests.cs`
  - `SpringDay1DialogueProgressionTests.cs`
    - `native_validation=clean`
    - `owned_errors=0`
    - 但被现场已有 external console 噪音打成 `external_red`
  - 命令桥已执行：
    - `Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets`
    - `Tools/NPC/Spring Day1/Validate New Cast`
  - `Editor.log` 最新仍为：
    - `[SpringDay1NpcCrowdValidation] PASS | npcCount=8 | totalPairLinks=16`
  - own `git diff --check` 通过
  - `npc-resident-director-bridge-tests.json`
    - 上一次稳定结果仍是 `passed`
    - 本轮 rerun 结果文件停在 `running/started`，未 claim 新通过
- 当前关键判断：
  - `NPC` 本线当前已经把 resident fallback 压到了两层：
    - `phaseInformalChatSets`
    - `phaseNearbyLines`
  - 现在更该由 `day1` 吃回的是：
    - resident deployment
    - director consumption
    - Town 常驻落位
  - 给 `存档系统` 的边界判断也已固定：
    - 第一版只接长期态（`relationshipStage + formal consumed / completed sequence`）
    - 不接聊天过程态 / 气泡态 / nearby 触发痕迹
- 当前恢复点：
  - 本轮不再继续往下压新功能；
  - 下一步只做 `Park-Slice`、追加 `skill-trigger-log`，然后离场等待调度。

## 2026-04-06｜只读核实：NPC 给 day1 的真实 contract 与 Town-native 续工边界

- 当前主线目标：
  - 用户要求我只读核实 `NPC` 线当前真实状态；
  - 只回答三件事：
    1. 现在已经给 `day1` 交了哪些可直接消费的 contract
    2. 哪些内容仍然属于 runtime spawn / deployment，不该再继续由 `NPC` 线往下吞
    3. 如果方向改成 `Town` 原生 resident，`NPC` 下一轮最该补什么
- 本轮子任务：
  1. 对照两份最新回执：
     - `2026-04-06_NPC给day1_全量进度与承接边界回执_14.md`
     - `2026-04-06_NPC给day1_阶段安全点回执_15.md`
  2. 核对两份关键代码：
     - `SpringDay1NpcCrowdManifest.cs`
     - `NPCDialogueInteractable.cs`
  3. 只读搜索 `phaseNearby`、bridge tests 与实际消费点，确认回执不是空 claim
- 本轮实际完成：
  1. 核实 `SpringDay1NpcCrowdManifest` 确实已经公开：
     - `TryGetEntry`
     - `GetEntriesForDirectorConsumptionRole`
     - `BuildBeatConsumptionSnapshot`
     - resident baseline / beat semantics / director consumption role
  2. 核实 `NPCDialogueInteractable` 确实已经公开：
     - `NPCFormalDialogueState`
     - `HasFormalDialogueConfigured`
     - `GetFormalDialogueStateForCurrentStory`
     - `HasConsumedFormalDialogueForCurrentStory`
     - `WillYieldToInformalResident`
  3. 额外搜索确认：
     - `phaseNearbyLines` 已真实落在 8 份 `SpringDay1Crowd` 对话资产
     - `NPCDialogueContentProfile / NPCRoamProfile / PlayerNpcNearbyFeedbackService` 确实已有 phase-aware nearby 链路
     - bridge tests / validation menu 仍在
     - `SpringDay1NpcCrowdDirector` 与 `SpringDay1Director` 已经调用 `BuildBeatConsumptionSnapshot(...)`
     - `SpringDay1ProximityInteractionService`、`NPCInformalChatInteractable` 已经读取 formal-state contract
- 关键判断：
  1. 现在 `NPC` 已可直接给 `day1` 吃的 contract，至少包括：
     - resident semantic matrix / beat snapshot helper
     - formal once-only -> informal/resident fallback contract
     - phase-aware informal / nearby resident 数据层
     - bridge tests / validation 护栏
  2. 现在真正不该继续由 `NPC` 线单吞的，是：
     - `CrowdDirector / SpringDay1Director` 的 resident deployment 细化
     - `Town / Primary` 的常驻摆位、可见性、root/anchor 落位
     - runtime “到点刷出来 / 看起来不像原住民” 这类 deployment 问题
  3. 如果方向改成 `Town` 原生 resident，`NPC` 下一轮最该补的不是再写 spawn/runtime，而是补一份更硬的 native-resident-facing contract / probe：
     - 把 `semanticAnchorIds / residentBaseline / beat role / formal-consumed fallback / phaseInformal / phaseNearby`
       压成 `Town` 可直接消费和验收的映射与护栏
- 验证结果：
  - 本轮纯静态文档 / 代码 / repo 搜索核对；
  - 未改任何业务代码或资源；
  - 未跑 `Begin-Slice`
    - 原因：本轮始终是只读分析。
- 当前恢复点：
  - 如果用户后续要继续施工，最合理的切口是：
    1. 要么让 `day1 / Town` 主刀吃 runtime resident deployment
    2. 要么只让 `NPC` 再补 native-resident contract / probe / tests

## 2026-04-06｜续记：按 day1 27 号补充口径收了一刀 phase selfTalk / phase walkAway / blocker fix

- 当前主线目标：
  - 继续按 `day1` 27 号补充口径做 `NPC` own：
    1. resident 常驻语义继续做实
    2. formal consumed 后的 resident / informal 回落继续补深
    3. 继续补 `day1` 可直接吃的 contract / validation
  - 明确不再往 `runtime spawn / deployment / Town resident 主逻辑` 漂。
- 本轮子任务：
  1. 把 `phase-aware selfTalk` 从数据层一路打到资产层
  2. 把 `phaseInformalChatSets` 的 `walkAwayReaction` 做成 phase-aware
  3. 补对应 validation / tests
  4. 处理用户中途抛来的 `SpringDay1NpcCrowdDirector.cs` 编译红
- 本轮实际完成：
  1. `NPCDialogueContentProfile.cs`
     - 新增 `PhaseSelfTalkSet / phaseSelfTalkLines / GetSelfTalkLines(StoryPhase) / TryGetPhaseSelfTalkSet`
  2. `NPCRoamProfile.cs`
     - 新增 phase-aware selfTalk 透传
  3. `NPCAutoRoamController.cs`
     - 长停自语优先走 `StoryPhase`
     - 新增 `TryShowResidentSelfTalk(...)`
  4. `SpringDay1NpcCrowdBootstrap.cs`
     - 新增 `BuildPhaseSelfTalkSets / BuildPhaseSelfTalkPayloads / BuildPhaseWalkAwayReactionPayload`
     - 8 个 crowd NPC 已补 phase selfTalk 与 phase walkAway
  5. `SpringDay1NpcCrowdValidationMenu.cs`
     - 新增 `ValidatePhaseSelfTalkCoverage(...)`
     - runtime targeted probe 已补 selfTalk 入口代码（本轮未 live claim）
  6. tests：
     - `NpcCrowdDialogueNormalizationTests.cs`
       - 新增 selfTalk / walkAway 资产 coverage
     - `SpringDay1DialogueProgressionTests.cs`
       - 新增 selfTalk / walkAway contract tests
  7. carried foreign blocker fix：
     - `SpringDay1NpcCrowdDirector.cs` 最小补了 `EnumerateAnchorNames(...)`
  8. 回执：
     - 新增 `2026-04-06_NPC给day1_阶段安全点回执_16.md`
     - 新增 `2026-04-06_NPC_存档边界回执_02.md`
- 本轮验证结果：
  - `validate_script`：
    - `NPCDialogueContentProfile.cs` clean
    - `NPCRoamProfile.cs` clean
    - `NPCAutoRoamController.cs` `errors=0 warnings=1`
    - `SpringDay1NpcCrowdBootstrap.cs` clean
    - `SpringDay1NpcCrowdValidationMenu.cs` clean
    - `NpcCrowdDialogueNormalizationTests.cs` clean
    - `SpringDay1DialogueProgressionTests.cs` clean
    - `SpringDay1NpcCrowdDirector.cs` `errors=0 warnings=2`
  - 菜单：
    - `Refresh Crowd Dialogue Assets` 已执行
    - `Validate New Cast` 结果：
      - `PASS | npcCount=8 | totalPairLinks=16`
  - targeted EditMode 小集：
    - job `succeeded`
  - CLI：
    - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
  - `git diff --check`：
    - own + carried 路径通过
  - Unity：
    - 保持 `Edit Mode`
- 当前关键判断：
  - `NPC` 本线现在已经把 resident fallback 压到了：
    - `phaseInformalChatSets`
    - `phaseNearbyLines`
    - `phaseSelfTalkLines`
    - phase-specific `walkAwayReaction`
  - 继续往下最该由 `day1 / Town` 吃回的是：
    - 原生 resident deployment
    - director consumption
    - scene / anchor 落位
  - 存档边界不变：
    - 这轮新增都是内容资产，不是新的长期态存档面
- 当前恢复点：
  - 这轮代码安全点已经形成；
  - 写完审计日志后 `Park-Slice`，等待下一轮新授权。

## 2026-04-06｜收口：当前 slice 已正式 PARKED

- `Park-Slice` 已执行：
  - 当前 `thread-state = PARKED`
  - `blockers = 无`
- 当前最终离场状态：
  - Unity 在 `Edit Mode`
  - fresh `errors = 0`
  - 这轮可直接交 `day1 / 用户` 继续判下一刀

## 2026-04-07｜农场动物 prefab 快速切片已完成并停车

- 当前主线：
  - 用户临时切到“农场动物 prefab + NPC 漫游接入”；
  - 目标是快速把鸡 / 牛做成可直接拖进场景的 roam prefab，不回旧的 day1 / resident 线。
- 本轮子任务：
  - 给鸡 / 牛补特殊朝向逻辑；
  - 生成 prefab / clip / controller / roam profile；
  - 不关 Unity，不碰 scene 主线。
- 本轮完成：
  1. `NPCAnimController.cs`
     - 新增可序列化的 4 向独立 `AnimatorDirection / FlipX` 映射；
     - 默认 old path 不变。
  2. `FarmAnimalPrefabBuilder.cs`
     - 新增菜单 `Tools/NPC/Farm Animals/生成可漫游动物预制体`
     - 自动切片并生成 5 个动物 prefab、clip、controller、roam profile。
  3. 真实产物已落盘：
     - `Assets/222_Prefabs/FarmAnimals/*`
     - `Assets/100_Anim/FarmAnimals/*`
     - `Assets/111_Data/NPC/FarmAnimals/*`
     - 5 张动物源图 `.meta` 已回写新切片
- 特殊映射结论：
  - 鸡：`左/下 = Row0`，`右/上 = Row1`，不 flip
  - 牛：`左 = Row0`，`右 = Row0 + flip`，`下 = Row1`，`上 = Row2`
  - `Baby Chicken Yellow` 第 3 行是蛋，不进运动动画
- 本轮验证：
  - CLI `validate_script` 两文件均 `owned_errors=0`，但现场有外部 `missing script`，assessment 只能报 `external_red`
  - direct MCP `validate_script` 两文件 `errors=0 warnings=0`
  - 菜单已执行成功，菜单后 console 没有新报错
  - own 路径 `git diff --check` 通过
- 当前 blocker / 风险：
  - 全局 console 还挂着外部 `The referenced script (Unknown) on this Behaviour is missing!`
  - 本轮未做具体 scene live 摆放，所以“prefab 能 roam”是结构成立，不是具体场景体验已验
- 当前恢复点：
  - 如果下一轮继续，直接去目标 scene 摆 prefab 并做最小 live roam probe；
  - 如果不继续，这轮 prefab 生产链已经可直接交用户使用。
- 当前状态：
  - `Park-Slice` 已执行
  - `thread-state = PARKED`
  - Unity 保持打开并回到 `Edit Mode`

## 2026-04-07｜只读复核：Town 过完对话后卡顿，主热点不是 roam 而是 informal prompt 链

- 当前主线：
  - 用户给出 Town Profiler 截图，要求我判断“当前卡顿到底是不是 NPC 导航雪崩，还是别的热点”。
- 这轮子任务：
  - 只读审 `NPCInformalChatInteractable.cs` 与相关会话链，不开工改代码。
- 结论：
  1. 这次主因不是 `NPCAutoRoamController.FixedUpdate`
     - 截图里它只有 `0.18ms ~ 0.30ms`
  2. 当前最大热点确实是 `NPCInformalChatInteractable.Update()`
     - 但问题不止 `FindObjectsOfType`
     - 真正的热点是整条“每个 NPC 每帧自己做提示判定”的链
  3. 关键代码位：
     - `Update -> BuildInteractionContext`：`NPCInformalChatInteractable.cs:110-118`
     - `BuildInteractionContext` 内 `FindFirstObjectByType<PlayerMovement>`：`382-395`
     - `ReportProximityInteraction` 里重复 `ResolveSessionService + CanInteract`：`333-367`
     - `ResolveSessionService` 走反射：`405-427`
  4. “过完正式对话后更卡”的原因成立：
     - 对话进行时会在 `326-330` 因 `DialogueManager.IsDialogueActive` 早退
     - 对话结束后，所有 informal NPC 都恢复走完整提示链
- 我的判断：
  - 我认可“下一刀该砍 `NPCInformalChatInteractable.cs`”这个方向
  - 但真正要改的是：
    - 缓存玩家引用
    - 去反射
    - 去重判定
    - 必要时改成玩家侧集中收集
- 当前状态：
  - 本轮只读
  - 没有进入真实施工
  - 因此没跑 `Begin-Slice`

## 2026-04-07｜真实施工：Town 对话后卡顿第一刀性能修正已落并停车

- 当前主线：
  - 用户直接要求“开干”，所以我从只读结论进入真实施工；
  - 目标是先把 `NPC` own 热点里最贵的 3 块砍掉：找玩家、找会话服务、重复判定。
- 本轮子任务：
  - 改 `NPCInformalChatInteractable.cs`
  - 顺手把同结构的 `NPCDialogueInteractable.cs` 和 `NpcInteractionPriorityPolicy.cs` 一起缓存化
- 本轮完成：
  1. `NPCInformalChatInteractable.cs`
     - 反射调用 `PlayerNpcChatSessionService` 全部改成直连
     - 玩家 `Transform / sample point` 改成按帧缓存
     - `ReportProximityInteraction` 不再重复做 suppression / session 判定
  2. `NPCDialogueInteractable.cs`
     - formal prompt 链的玩家查找也改成按帧缓存
  3. `NpcInteractionPriorityPolicy.cs`
     - `StoryManager` 和玩家上下文都改成缓存查找
- 本轮验证：
  - direct MCP `validate_script`
    - `NPCInformalChatInteractable.cs`: `errors=0 warnings=1`
    - `NPCDialogueInteractable.cs`: `errors=0 warnings=0`
    - `NpcInteractionPriorityPolicy.cs`: `errors=0 warnings=0`
  - `git diff --check` own 3 文件通过
  - CLI fresh `errors` 仍然 external red：
    - `ItemTooltip` 缺符号，位于 `EnergyBarTooltipWatcher / Inventory UI`
- 当前判断：
  - 这刀已经把 Profiler 里最明显的 per-NPC per-frame 查找链切掉了；
  - 但我还没有拿到新的 Town live Profiler 对比图，所以当前只能说“代码层热点已实砍”，不能说“体感和帧率已经验完”。
- 当前 blocker：
  - 外部编译红阻断 live retest：
    - `ItemTooltip missing in EnergyBarTooltipWatcher / Inventory UI files`
- 当前恢复点：
  - 外部 compile 一旦清掉，下一轮直接回 Town 复跑同一路径，优先看：
    - `NPCInformalChatInteractable.Update`
    - `FindObjectsOfType / FindFirstObjectByType`
    - `NPCDialogueInteractable.Update`
  - 如果还不够，再做第二刀集中仲裁
- 当前状态：
  - `Begin-Slice` 已执行
  - `Park-Slice` 已执行
  - `thread-state = PARKED`

## 2026-04-07｜续记：Town prompt 链第二刀继续压下，warning cleanup 已止血

- 当前主线目标：
  - 继续把 `Town` 里正式对话后 NPC prompt 链的剩余热点，限制在 `NPC own` 文件面内往下压；
  - 不回吞 shared/UI/day1。
- 本轮子任务：
  1. 在三文件内继续做第二刀性能压缩
  2. live smoke 时若暴露 NPC own runtime error，就最小止血
  3. 用户中途报 `NPCInformalChatInteractable.cs` 头部重复 `using System` warning，顺手清掉
- 本轮实际完成：
  1. `NPCInformalChatInteractable.cs`
     - coarse distance 早退
     - cached bounds / cached context / cached action
     - 头部重复 `using System` 已删除
  2. `NPCDialogueInteractable.cs`
     - coarse distance 早退
     - cached bounds / cached context / cached action
     - 新增 `HasFormalDialoguePriorityAvailable()`
  3. `NpcInteractionPriorityPolicy.cs`
     - blocking page UI 按帧缓存
     - informal suppression 改走轻量 formal availability
     - 去掉已不需要的玩家上下文构造链
  4. `NPCBubblePresenter.cs`
     - runtime smoke 暴露 `SendMessage cannot be called during Awake...`
     - 最小补口：把 bubble UI 主动创建从 `Awake()` 挪到 `Start()`
- 本轮验证结果：
  - fresh `errors` 多次回到 `errors=0 warnings=0`
  - `git diff --check` own 文件通过
  - `manage_script validate`
    - `NpcInteractionPriorityPolicy.cs` clean
    - `NPCDialogueInteractable.cs` / `NPCInformalChatInteractable.cs` 只剩 generic GC warning
  - 用户抛出的重复 `using` warning 已清
- 当前 blocker：
  - live 最终复测没闭完，不是因为 own red，而是现场后续被 external console 噪音挡住：
    - 一度出现 `SpringDay1WorkbenchCraftingOverlay.cs` external compile red
    - 当前又出现 repeated `The referenced script (Unknown) on this Behaviour is missing!`
- 当前恢复点：
  - `NPC` own 第二刀代码已经到安全点
  - 只要外部现场回净，下一步就直接回 `Town` 做：
    1. play smoke
    2. `NPCBubblePresenter` residue 复核
    3. Town prompt 链最终性能体感 / Profiler 对照

## 2026-04-07｜续记：用户放出 Unity 后已尝试最终 live 复测，但被 external CraftingStation red 卡住

- 当前主线目标：
  - 趁 Unity 空出来，直接把 `NPC` 这轮最后的 live smoke 做掉；
  - 重点验 `NPCBubblePresenter` 补口和 Town prompt 链最终 smoke。
- 本轮实际执行：
  1. 重新 `Begin-Slice`
  2. fresh `status / errors` 起步是：
     - active scene=`Town.unity`
     - `Edit Mode`
     - `errors=0 warnings=0`
  3. 继续准备进 live 前，CLI fresh console 刷出 external compile red：
     - `CraftingStationInteractable.cs(328)` 两条 `CS0841`
- 当前 blocker：
  - `external compile red: Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs(328) uses local variable overlay before declaration`
- 当前判断：
  - 这次不是 `NPC` own 失败，而是 live 复测入口被别线红面重新挡住；
  - 所以我不能 claim “final live retest 已完成”。
- 当前恢复点：
  - 只要这个 external compile red 清掉，直接回 `Town` 做最后 smoke；
  - 本轮已 `Park-Slice`，等待下次放行。

## 2026-04-07｜只读续判：Town prompt 链第二刀的安全边界已收敛

- 当前主线：
  - 用户没有要求继续改，而是要求我只读评估：
    - 在不扩到更多运行时代码文件的前提下，
    - `NPCInformalChatInteractable.cs`
    - `NPCDialogueInteractable.cs`
    - `NpcInteractionPriorityPolicy.cs`
    - 这 3 个文件里还剩哪些安全性能优化。
- 本轮子任务：
  - 只读复盘第一刀后的剩余热点；
  - 给出“还能砍什么 / 最安全高收益是什么 / 哪些改法会越界”的结论。
- 本轮确认：
  1. 最大剩余热点仍在 `NPCInformalChatInteractable.ReportProximityInteraction(...)`
     - 当前仍是先做 session / suppression / formal takeover，再做 distance gate；
     - 远距 NPC 也会先支付重判定链。
  2. `NpcInteractionPriorityPolicy.ShouldSuppressInformalInteractionForCurrentStory(...)`
     - 仍借 `NPCDialogueInteractable.CanInteract(context)` 走偏重的 formal 链；
     - 和 informal 当前已有的 UI/dialogue gate 存在重复。
  3. `NPCDialogueInteractable.Update()` 在 formal 已 consumed 的 NPC 上仍会持续做 context/bounds/CanInteract 判定。
  4. 两处 `ReportCandidate(..., () => OnInteract(context), ...)` 仍保留 per-frame capture closure。
- 当前最值得继续做的安全优化：
  1. informal 先做 coarse distance 早退，再查 session / policy
  2. 给 formal takeover 拆轻量 availability 判定，别再让 policy 调完整 `CanInteract`
  3. 把两处 capture lambda 改成 cached action + 最近一次采样数据
- 当前不建议本轮做：
  - 玩家侧统一收集附近候选
  - 改 shared proximity/session/story service
  - 改 bubble/UI/day1/Town 运行时链路
- 验证状态：
  - 本轮纯静态只读分析；
  - 未进入真实施工；
  - 未跑 `Begin-Slice`。
- 当前恢复点：
  - 如果用户后续要我继续开干，第二刀仍可保持在这 3 个脚本内；
  - 下一步优先级已经固定为：
    1. distance 早退
    2. suppression 轻量化
    3. callback 去闭包

## 2026-04-07｜真实施工：NPC 气泡“方泡”根因已落到旧 scene 空壳，脚本补口和守卫测试已完成

- 当前主线目标：
  - 不再泛讲“样式回正”，而是直接解决用户指出的真问题：
    - `NPC` 现场为什么还是方泡/方框尾巴。
- 本轮子任务：
  - 只做 `NPCBubblePresenter` 的旧壳补口；
  - 不扩到 UI/day1/shared prompt shell；
  - 用最小验证把“旧空壳被绑定后是否会真正刷回气泡 sprite”钉死。
- 本轮查实：
  1. `Town.unity / Primary.unity` 都存在旧的 `NPCBubbleCanvas` 壳；
  2. 这些壳上的 body/tail `Image` 节点还在，但 `m_Sprite=0`；
  3. 原逻辑 `TryBindExistingBubbleUi()` 只绑引用，不回刷 sprite/type，所以会直接把旧空壳带进现场。
- 本轮完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - 旧壳绑定成功后改为先走 `RefreshBoundBubbleUiAssets()`；
     - body/tail 六张图都会重新赋上 runtime rounded-rect / tail sprite；
     - 同步回刷 `Image.Type`、字体、layout、sorting；
     - 继续保留 `Start()` 期补口，避免 `Awake()` 期 residue。
  2. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 `TemporaryEditObjectPresenter_ShowText_ShouldRefreshLegacyBubbleSprites`；
     - 直接造一套旧空壳，验证 `ShowText()` 后：
       - body sprite 非空且三张共用同一 sprite
       - tail sprite 非空且三张共用同一 sprite
       - body/tail 不是同一 sprite
       - body=`Sliced`、tail=`Simple`
  3. 整份 `NpcBubblePresenterEditModeGuardTests` 已重跑：
     - `6/6 Passed`
- 本轮验证结果：
  - 单条新增测试：`Passed`
  - 整份 guard tests：`Passed (6/6)`
  - `git diff --check` 对本轮 own 文件通过
  - `validate_script` 对 own 文件没有 owned error，但 CLI 因 Unity `stale_status` 只能报 `unity_validation_pending`
  - fresh console 仍有 external red：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 缺 `EnsureProgressFillGraphic / EnsureProgressLabelBinding`
- 本轮对 live 的诚实判断：
  - 我不能 claim “用户现场观感已经最终过线”；
  - 因为这轮虽然代码补口和守卫测试都站住了，但 `Town` 的最终 GameView 还缺用户肉眼终验。
- 当前恢复点：
  - 线程已 `Park-Slice`
  - 下一步如果继续，只需要回 `Town` 看三件事：
    1. 气泡主体是否从方框恢复为圆角气泡
    2. 尾巴是否从方块恢复为三角尾巴
    3. 如果仍不对，再继续沿旧壳 `RectTransform/Image` 参数链往下查

## 2026-04-07｜续刀：Primary/001 仍见方泡，补口进一步收窄到“缓存存在但 sprite 回空”的 residue

- 当前主线目标：
  - 继续修 `Primary/001` 的 NPC 气泡 still-square 现场；
  - 不再把结构 pass 说成体验 pass。
- 本轮子任务：
  - 查 `NPCBubblePresenter` 自己是否还有第二层漏点；
  - 用最小 editor guard tests 把这层洞钉死；
  - 不扩 UI/day1/shared shell，不关 Unity。
- 本轮关键判断：
  1. runtime 生成的 body/tail sprite 本体仍是圆角 body + 三角 tail，不是生成函数自己画成方块；
  2. 真根因进一步收窄为：
     - 旧 scene 壳的 `Image.sprite = null` 会直接画出 Unity 默认矩形；
     - 以前 `EnsureBubbleUi()` 只要 `_canvas/_bubbleText` 缓存还在就直接 return；
     - 如果运行中某次 residue 把 body/tail sprite 又打空，后续 `ShowText()` 不会再补口，于是用户看到的还是方泡。
- 本轮完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
     - 新增 `HasResolvedBubbleUi()`；
     - `EnsureBubbleUi()` 改成：
       - 已解析 UI 时也强制 `RefreshBoundBubbleUiAssets()`；
       - 缓存半残时先 `ResetBubbleUiCache()` 再重绑；
     - 让“第二次显示前重新回刷 body/tail sprite”变成硬逻辑。
  2. `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
     - 新增 `TemporaryEditObjectPresenter_ShowText_ShouldRecoverCachedBubbleSpritesAfterTheyGoNull`；
     - 覆盖“缓存还在，但六张 image 的 sprite/type 被打坏后，第二次显示仍能回正”。
  3. `Assets/Editor/NPC/NpcBubblePresenterGuardValidationMenu.cs`
     - 新增真正挂在 Editor 装配里的菜单：
       `Tools/Sunset/NPC/Validation/Run Bubble Presenter Guard Tests`
     - 用命令桥直接跑 bubble guard tests。
- 本轮验证：
  - `git diff --check`：
    - `NPCBubblePresenter.cs`
    - `NpcBubblePresenterEditModeGuardTests.cs`
    - `NpcBubblePresenterGuardValidationMenu.cs`
    全部 clean
  - `Editor.log` fresh compile：
    - `Tundra build success`
    - 无 own red
    - 只有 external warning：`SpringDay1WorkbenchCraftingOverlay.cs(2232)` 过时 API warning
  - 命令桥测试结果：
    - `Library/CodexEditorCommands/npc-bubble-presenter-guard-tests.json`
    - `passed / success=true / total=7 / passed=7 / failed=0`
- thread-state：
  - 开工前沿用已有 `ACTIVE` 切片：`npc-bubble-shape-primary001-rootcause-20260407`
  - 本轮结束已跑 `Park-Slice`
  - 当前 live 状态：`PARKED`
- 当前 blocker：
  - 等用户回 `Primary/001` 真入口复测气泡形状；
  - 若仍见方泡，下一步优先怀疑“不是 `NPCBubblePresenter` 这条链在出图”，继续追别的头顶壳/提示链，而不是原地重复改同一补口。

## 2026-04-08｜Town/002 只读 incident：自动测试气泡根因定位到场景实例残留测试组件

- 当前主线目标：
  - 继续守 `NPC` 本体体验与气泡链，但这轮用户插入的子任务是只读排查 `Town/002` 为什么像测试 NPC 一样持续冒泡。
- 本轮子任务：
  - 只读核实 `Town` 场景、脚本 guid 与 prefab 对照，判断是不是误挂了测试组件。
- 本轮完成：
  1. 在 `Assets/000_Scenes/Town.unity` 查到 `002` 的场景实例确实额外挂了 `NPCBubbleStressTalker`
     - `m_Script guid = 2a83cf9837b058742b73c9ff3ad3796a`
  2. 对应 meta 已确认指向：
     - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs.meta`
  3. 该实例当前参数是测试态：
     - `startOnEnable: 1`
     - `disableRoamWhileTesting: 1`
     - `testLines` 非空
  4. 源码注释确认这是测试脚本：
     - `NPCBubbleStressTalker.cs` 顶部写明“仅用于压测 NPC 气泡布局”
  5. 对照 `002.prefab` 未见该脚本、`003.prefab` 只是挂着但 `startOnEnable: 0`
- 关键判断：
  - 这不是 `002` 正常 NPC 逻辑坏了，而是 `Town` 场景实例上的测试脚本被开成了自动运行；
  - 这也解释了为什么它一直冒测试气泡、同时正常漫游像被压住。
- 最小恢复建议：
  1. 直接从 `Town/002` 实例移除 `NPCBubbleStressTalker`
  2. 或至少把 `startOnEnable` 改回 `0`
- 验证状态：
  - 只读静态取证成立
  - 未进入真实施工
  - 未改任何 tracked 文件
- 当前恢复点：
  - 这轮子任务已经结束；
  - 若用户允许改动，下一步就是做 `Town/002` 的最小退测修复，然后只窄测“是否不再自动循环冒测试气泡”。

## 2026-04-08｜只读诊断：NPC 气泡字体显示问题已定位到字体链选型 + 文本样式过重 + 材质同步缺口

- 当前主线目标：
  - 继续守 `NPC` 本体体验；本轮用户插入的子任务是只读解释“为什么 NPC 气泡字体会变成这样”。
- 本轮子任务：
  - 只读核查气泡组件、prefab 绑定、字体 bootstrap 和字体资产现场，判断是字体资产坏、还是气泡参数链把字体显示做坏。
- 本轮完成：
  1. 查到 `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 都优先使用：
     - `DialogueChinese Pixel SDF`
  2. 查到所有当前 NPC prefab 也都直接序列化绑定：
     - `fontAsset = DialogueChinese Pixel SDF`
  3. 查到 `DialogueChinese Pixel SDF` 本体其实是：
     - `Fusion Pixel 10px Mono zh_hans`
  4. 查到气泡文本样式当前很重：
     - `characterSpacing = 1.25`
     - `lineSpacing = -5 / -2.5`
     - `outlineWidth = 0.18`
  5. 查到 runtime bootstrap 虽然有 `DialogueChinese V2 SDF` 优先序，但气泡链传入 `Pixel` 作为 preferred font 时，会继续优先保留 `Pixel`
  6. 查到 `NPCBubblePresenter` / `PlayerThoughtBubblePresenter` 换字体时没有同步 `fontSharedMaterial`，而 `DialogueUI / NpcWorldHintBubble` 有做这一步
- 关键判断：
  - 这次字体显示问题不是单个字库坏了，也不是只缺字；
  - 真因是三层叠加：
    1. 气泡链当前选了不合适的像素单宽字体
    2. 同时套了过重的字距 / 行距 / 描边参数
    3. 运行时材质同步链不完整
- 验证状态：
  - 只读静态取证成立
  - 未开工、未改代码、未动 Unity 现场
- 当前恢复点：
  - 如果后续允许改，最优先不是重做所有气泡，而是先做最小收口：
    1. 气泡链字体选型回正
    2. `fontSharedMaterial` 补齐
    3. 再调字距 / 行距 / 描边参数

## 2026-04-08｜字体修复施工：NPC/玩家气泡已回正到非 Pixel 主链，编译已过

- 当前主线目标：
  - 把用户截图里暴露出来的字体显示问题，直接修到可打包状态。
- 本轮施工范围：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
  - `Assets/YYY_Tests/Editor/PlayerThoughtBubblePresenterStyleTests.cs`
- 本轮完成：
  1. `NPCBubblePresenter`
     - `CurrentStyleVersion` 升到 `14`
     - 字体优先序改成 `V2/SDF/SoftPixel/Pixel`
     - 运行时不再被 prefab 里旧 `Pixel SDF` 锁死
     - 补 `fontSharedMaterial`
     - 文本参数改成：
       - `characterSpacing = 0`
       - `lineSpacing = -0.8`
       - `outlineWidth = 0.08`
  2. `PlayerThoughtBubblePresenter`
     - 同步补 `fontSharedMaterial`
     - 同步改非 `Pixel` 主链优先
     - 玩家文本参数改成：
       - `characterSpacing = 0`
       - `lineSpacing = -0.4`
       - `outlineWidth = 0.08`
  3. 两份 tests
     - 新增 guard，明确防止回退到：
       - `Pixel` 主字体
       - 材质不同步
       - 重描边/重字距
- 本轮验证：
  - `git diff --check` 对 4 文件通过
  - `Editor.log` 最新 compile：
    - `*** Tundra build success (4.30 seconds), 7 items updated, 862 evaluated`
  - 最新日志未见本轮 own `error CS`
  - 只剩 external warnings：
    - `PackagePanelRuntimeUiKit.cs(105)`
    - `SpringDay1WorkbenchCraftingOverlay.cs(2245)`
- 当前诚实状态：
  - 代码层：已修
  - 编译层：已过
  - live 观感层：还没拿到 direct MCP 现场复测，因为当前 listener 掉线
- 当前恢复点：
  - 这轮可以停在这里给用户终验；
  - 若后续继续，只需要回现场看：
    1. NPC 气泡是否不再是粗糙 Pixel 味
    2. 描边是否明显变轻
    3. 中文行内是否不再发挤

## 2026-04-08｜继续主线：resident 接管 contract 与最小 snapshot surface 已做完，停在可交接安全点

- 当前主线目标：
  - 执行 `prompt 17`，给 `day1` 和 `存档系统` 提供真正能用的 `native resident` contract；
  - 不回去补 `runtime spawn`，不漂去 `Town/Primary` scene writer，也不主刀 `CrowdDirector` 主消费逻辑。
- 本轮子任务：
  1. 让 scene-owned resident 被导演接管时，不再乱跑、乱打招呼、乱回旧逻辑
  2. 给外部暴露最小 resident runtime snapshot surface
  3. 在安全点写回 `day1` 回执、`存档系统` 边界补充和 memory
- 本轮完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 resident scripted control owner 栈与公开状态：
       - `IsResidentScriptedControlActive`
       - `ResidentScriptedControlOwnerKey`
       - `ResidentStableKey`
       - `ResumeRoamWhenResidentControlReleases`
       - `IsNativeResidentRuntimeCandidate`
     - 新增：
       - `AcquireResidentScriptedControl`
       - `ReleaseResidentScriptedControl`
       - `ClearResidentScriptedControl`
       - `CaptureResidentRuntimeSnapshot`
       - `ApplyResidentRuntimeSnapshot`
     - resident scripted control active 时，`Update / FixedUpdate` 统一冻结 resident runtime。
  2. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeSnapshot.cs`
     - 新增最小 DTO。
  3. `Assets/YYY_Scripts/Controller/NPC/NpcResidentRuntimeContract.cs`
     - 新增 `CaptureSceneSnapshots / TryApplySnapshot / TryFindResident / ResolveSceneTransform / BuildHierarchyPath`
  4. `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
     - resident 被 scripted control 接管时，不再开放 informal prompt/interaction。
  5. `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
     - resident 被 scripted control 接管时，不再发 nearby bubble。
  6. `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
     - active NPC 若半路被系统接管，闲聊会按 `SystemTakeover / DialogueTakeover` 收束。
  7. `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
     - 现有 takeover 入口接入 `Acquire/ReleaseResidentScriptedControl("spring-day1-director", ...)`
  8. tests：
     - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
- 关键决策：
  - 这轮只做 `NPC` owner contract，不做 `day1` runtime consumer 主链；
  - snapshot 只表达 resident 最小运行态，不混入 typing / bubble / nearby / formal 半句过程态；
  - formal consumed / relationship / story one-shot 仍留给长期剧情态来源，不塞进 resident snapshot。
- 回执与文档：
  1. 新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_NPC给day1_原生resident接管与持久态协作回执_18.md`
  2. 更新：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_NPC_存档边界回执_01.md`
- 验证：
  1. `py -3 scripts/sunset_mcp.py errors --count 30 --output-limit 20`
     - `errors=0 warnings=0`
  2. `py -3 scripts/sunset_mcp.py compile ...`
     - 被 `dotnet/codeguard timeout` 卡成 `assessment=blocked`
  3. `py -3 scripts/sunset_mcp.py validate_script ...`
     - `assessment=unity_validation_pending`
     - `owned_errors=0`
     - `external_errors=0`
     - `codeguard=timeout-downgraded`
     - `wait_ready` 卡在 `stale_status`
  4. `git diff --check`
     - 对本轮 own/carried 文件通过
- thread-state：
  1. 继续施工前已 `Begin-Slice`
  2. 中途用 `Begin-Slice -ForceReplace` 把 `NpcResidentRuntimeSnapshot.cs` 补进 owned paths
  3. 本轮先不 `Ready-To-Sync`
  4. 已 `Park-Slice`
  5. 当前状态：`PARKED`
- 当前恢复点：
  - 这轮已经能把 contract 正式交给 `day1 / 存档系统`；
  - 如果后面继续，优先是补 contract 缺口或对接消费方反馈，而不是自己回吞 deployment / scene writer。

## 2026-04-09｜继续主线：NPC 走位鬼畜调度层止血第二刀已落地

- 当前主线目标：
  - 不改 NPC 业务语义，只修“走得对但原地疯狂转向/鬼畜”的底层调度 bug，并守住前面玩家 traversal 与性能止血不回退。
- 本轮子任务：
  1. 继续只碰 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  2. 在第一刀“同帧重决策复用”之后，再补一层“原地反向发速度去抖”
  3. 用最小 CLI 自检确认 own red 为 0
- 本轮完成：
  1. `NPCAutoRoamController.cs`
     - 保留上一刀 `CanReuseHeavyMoveDecisionThisFrame(...)` 的物理 NPC 同帧复用补丁
     - 新增 `MOVE_DIRECTION_STABILIZE_*` 常量
     - 在 `TickMoving(...)` 里对 `adjustedDirection` 增加 `StabilizeMoveDirection(...)`
     - 新增 `ShouldHoldPreviousMoveDirection()`
     - 作用：当 NPC 还卡在原地，且刚刚发过移动命令/处在 blocked-advance、shared-avoidance 或 detour 阻挡态时，如果本帧新方向几乎要跟上一条方向打反，就先沿用上一条方向，避免左右来回抽搐式翻向
- 关键决策：
  - 继续坚持“只修调度，不改目标/路径语义”
  - 不碰玩家桥/水/边界 contract
  - 不碰 scene、binder、tool、Town/Primary 接线
- 验证：
  1. `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
     - `assessment=no_red`
     - `owned_errors=0`
     - `external_errors=0`
     - `current_warnings=0`
  2. `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 通过
- thread-state：
  - `Begin-Slice`：已跑（`npc-anti-jitter-hotpath-fix-2`）
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：待本轮收尾时补
- 当前恢复点：
  - 代码层已经到“可以申请最小 Unity live 验”的状态
  - 如果 live 仍抖，下一刀仍只应继续压 `NPCAutoRoamController` 的阻挡态去抖/节流，不应扩到 traversal 主规则

## 2026-04-09｜只读根因收敛：鬼畜已关，当前主 bug 切换为 FixedUpdate 导航热链

- 当前主线目标：
  - 用户这轮不要我继续改代码，只要我把 NPC 当前 bug 的真正根因彻底说清
- 本轮只读分析结论：
  1. 现在 `NPC` 不再抽搐，说明前两刀已经命中了“方向翻转/同帧重复重决策”这层 bug
  2. 用户提供的 Profiler 已把当前更深层的主热点钉死：
     - `NPCAutoRoamController.FixedUpdate()[Invoke]` 自耗时约 `1601ms`
     - `Physics2D.OverlapCircleAll` 约 `30ms`
     - `GC.Alloc` 约 `12.8ms`
     - 单帧 GC 分配约 `15.3MB`
  3. 结合代码，当前性能 bug 主链为：
     - `FixedUpdate() -> TickMoving(...)`
     - `TryHandleSharedAvoidance / TryHandleBlockedAdvance / TryResolveOccupiableDestination`
     - `NavigationTraversalCore.CanOccupyNavigationPoint / TryResolveOccupiableDestination`
     - `NavGrid2D.IsWalkable / IsPointBlocked / OverlapCircle`
  4. 所以“为什么修了很多轮还会有问题”的真实答案是：
     - 之前修掉的都是真 bug
     - 但修掉的是不同层，不是同一个点
     - 鬼畜关掉后，性能热链才完全暴露出来
- 当前恢复点：
  - 后续验收必须拆成两类：
    1. `鬼畜转向是否已关闭`
    2. `FixedUpdate 热链性能是否已收`
  - 不能再把这两类问题混成一个“NPC 还没修完”

## 2026-04-09｜一轮继续施工：FixedUpdate 热链降本补丁已落地，停在可验收安全点

- 当前主线目标：
  - 用户明确要求“一轮内彻底落地所有内容，朝打包态闭环推进”
  - 本轮只允许做性能修复与 bug 修复，不改 NPC 业务意图
- 本轮子任务：
  1. `NPCAutoRoamController` 扩大物理 NPC 重决策复用窗口
  2. `NavGrid2D` 增加动态查询 / nearest walkable / path 的低风险缓存
  3. `NavigationTraversalCore` 增加 occupancy probe 同帧缓存
  4. 做最小 Unity live smoke 看是否引出新的导航红错
- 本轮完成：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 增加 `PHYSICS_HEAVY_DECISION_REUSE_SECONDS`
     - 增加 `lastHeavyMoveDecisionTime`
     - 让物理 NPC 的重决策结果在极短跨帧窗口内可复用
  2. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 新增 `DynamicWalkableQueryKey / NearestWalkableQueryKey / PathCacheKey`
     - `IsWalkable(...)` 动态查询同帧缓存
     - `TryFindNearestWalkable(...)` 80ms 短窗口缓存
     - `TryFindPath(...)` 路径缓存
     - `RebuildGrid / RefreshGridRegion` 自动失效缓存
  3. `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
     - 新增 `OccupancyQueryKey`
     - `CanOccupyNavigationPoint(...)` 同帧缓存
  4. Unity live smoke：
     - `Town` 短窗 `15s`
     - 未出现新的导航运行时红错
- 关键决策：
  - 按“occupancy probe -> nearest walkable -> path cache -> heavy decision reuse”的安全顺序下刀
  - 不先改 `FixedUpdate` 频率，不先粗暴拉大 avoidance/repath 冷却
  - 不碰 NPC 去哪、不碰玩家 bridge/water/edge 语义
- 验证：
  1. `validate_script NPCAutoRoamController.cs`
     - `owned_errors=0`
     - 但存在 external red：
       - `NpcCharacterRegistryTests.cs` 缺符号
       - `HomeFoundationBootstrapMenu.cs` 交叉场景引用
  2. `manage_script validate NavGrid2D`
     - `clean`
  3. `manage_script validate NavigationTraversalCore`
     - `clean`
  4. `git diff --check`（三份脚本）
     - 通过
- thread-state：
  - `Begin-Slice`：已跑（`npc-fixedupdate-hotpath-package-closure`）
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
- 当前恢复点：
  - 这轮已经把我这条线能安全做的性能刀落下去了
  - 但 external red 仍在，所以不能宣称“整个项目 clean”
  - 如果用户接下来要我做验收，最该看的是长时间运行下的导航稳定性，而不是再回去验同一个鬼畜问题

## 2026-04-09｜只读结算：人物真值链已查明，当前不是同源一套

- 当前主线目标：
  - 回答用户“剧情 NPC、关系页 NPC、场景 NPC 现在到底是不是三套东西”，并把人物 id / 名字 / 简介 / 头像来源查实。
- 本轮子任务：
  1. 只读核对剧情对白资产
  2. 只读核对关系页数据入口
  3. 只读核对场景 prefab 与 NPC 内容资产 id
  4. 不改代码，只给结构结论
- 本轮完成：
  1. 已确认场景 / NPC 本体 numeric id 全量为：
     - `001/002/003/101/102/103/104/201/202/203/301`
  2. 已确认关系页只读 `SpringDay1NpcCrowdManifest.asset`
     - 当前只包含 `101/102/103/104/201/202/203`
     - `001/002/003/301` 都不在关系页 manifest
  3. 已确认 formal 剧情对白目前不认 `npcId`
     - `DialogueNode` 只有 `speakerName + speakerPortrait`
     - 头像空时 `DialogueUI` 只兜底到 `Assets/Sprites/NPC/001.png`
  4. 已确认 `Assets/Sprites/NPC_Hand` 当前只有 `001/002`
     - 也没有被 formal 剧情自动消费
  5. 已确认 formal 说话人标签和 numeric id 没有统一绑定层
     - 有：`村长 / 艾拉 / 卡尔 / 旅人 / 村民 / 小孩 / 小米 / 饭馆村民`
     - 其中 `村长 -> 001`、`艾拉 -> 002` 有强语义证据
     - 其余未见统一 `npcId` 映射
- 关键决策：
  - 这轮不下手改，因为问题不是单点 bug，而是结构上至少 3 条链并行：
    1. prefab + NPC content 的 `npcId` 实体链
    2. 关系页 manifest 子集链
    3. formal 剧情 `speakerName / speakerPortrait` 文本链
  - 真要收口，必须先统一人物真值表，再决定 UI / 剧情 / 头像如何回接，不该继续靠猜名字补洞
- 关键证据：
  - `Assets/YYY_Scripts/Story/Data/DialogueNode.cs`
  - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
  - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
  - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
  - `Assets/111_Data/NPC/`
  - `Assets/111_Data/NPC/SpringDay1Crowd/`
  - `Assets/222_Prefabs/NPC/`
- thread-state：
  - 本轮只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前恢复点：
  - 后续若继续这条线，第一合理动作应该是输出“统一人物真值表 + 映射缺口表”，而不是直接改 UI 表现层

## 2026-04-09｜统一人物主表已从只读结论进入真实落地

- 当前主线目标：
  - 把“剧情 NPC / 关系页 NPC / 场景 NPC 不是同源”这件事，从只读分析推进成第一刀真实实现。
- 本轮子任务：
  1. 建统一人物主表
  2. 让 formal 对白头像吃主表
  3. 让关系页身份真值吃主表
  4. 给 dialogue / NPC content / crowd manifest 补一致性护栏
- 本轮真实完成：
  - 新增：
    - `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `Assets/Resources/Story/NpcCharacterRegistry.asset`
    - `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
  - 修改：
    - `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
    - `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
- 这轮主表当前承载的真值：
  - roster：
    - `001/002/003/101/102/103/104/201/202/203/301`
  - alias bridge：
    - `村长 / 马库斯 -> 001`
    - `艾拉 -> 002`
    - `卡尔 -> 003`
    - `村民 / 围观村民 -> 101`
    - `小孩 / 小米 -> 103`
    - `饭馆村民 -> 203`
    - `老杰克 -> 102`
    - `老乔治 -> 104`
    - `老汤姆 -> 301`
- 本轮护栏新增：
  1. authored dialogue 里的 `speakerName` 都必须能回到主表
  2. `Assets/111_Data/NPC/**` 里的 `NPCDialogueContentProfile` 都必须能回到主表
  3. `SpringDay1NpcCrowdManifest.asset` 条目都必须能回到主表
  4. `DialogueUI` / `PackageNpcRelationshipPanel` 源码里必须真的消费主表
- 本轮验证：
  - `validate_script Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
  - `validate_script Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
  - 当前不是 own red 卡住，而是：
    - `codeguard timeout-downgraded`
    - `wait_ready/stale_status`
    - 中途被外部 PlayMode 状态抢占
- 本轮关键判断：
  - 这刀已经把“统一表”做成真资产 + 真消费口 + 真护栏；
  - 但还不能 claim 全闭环，因为 Unity live 终验没补完，而且 `PackageNpcRelationshipPanel.cs` 与 `UI` 线程同路径重叠，后续不应再继续单方面叠改。
- thread-state：
  - `Begin-Slice`：已跑，slice=`npc-unified-character-registry`
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑
  - 当前状态：`PARKED`
  - 当前 blocker：
    - `live终验未补：主表资产/正式对白头像/关系页实际观感仍待Unity现场确认`
    - `共享消费口重叠：PackageNpcRelationshipPanel.cs 当前与UI线程同路径重叠，后续不再继续叠改`
- 当前恢复点：
  - 下一步若继续，先做 Unity 现场终验；
  - 如果还要继续扩统一表，优先去接更多非重叠消费口，不先回改关系页壳。

## 2026-04-09｜slice=`npc-facing-stabilization-closure`：只修 NPC 偏头，不回动导航主规则

- 用户最新目标：
  - 彻底修掉 `NPC` 仍残留的偏头 / 摇头晃脑；
  - 不要拿“砍避让 / 改业务逻辑”换稳定；
  - 玩家桥/水/边缘和前面的导航性能补丁都不能回退。
- 本轮真实施工：
  - 已在 `ACTIVE` slice 上继续施工，主刀文件只有：
    - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
- 本轮关键判断：
  - 当前残留问题不在导航主规则，而在 `NPCMotionController.Update()` 直接拿瞬时速度做朝向。
  - 正确修法是把“怎么走”和“朝哪看”拆开，让朝向只跟稳定趋势，不跟 avoidance / blocked 窗口里的瞬时反向速度摆动。
- 已落代码：
  1. 增加朝向稳定参数：
     - `facingDirectionReversalMinHoldSeconds`
     - `facingVelocitySmoothing`
     - `facingAxisCommitBias`
     - `facingVelocityMinSpeed`
  2. `Update()` 改为：
     - `ResolveVelocity()` -> `ResolveStableFacingVelocity(...)` -> `ResolveFacingDirection(...)`
     - 只有轴向真正占优时才换方向，否则沿用当前稳定朝向
  3. 反向切头新增更长 hold
  4. `SetFacingDirection / StopMotion / ResetMotionObservation` 一并维护 `_smoothedFacingVelocity`
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
    - 阻断点不是 own red，而是：
      - `CodexCodeGuard timed out`
      - `mcp_error=<urlopen error [WinError 10061] ... 127.0.0.1:8888 拒绝连接>`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
    - 通过
- 本轮没做：
  - 没有回动 `NPCAutoRoamController`、`NavGrid2D`、`NavigationTraversalCore`
  - 没有改 NPC 去哪走
  - 没有做 live 验证，因为 Unity/MCP 桥当前没连上
- 当前恢复点：
  - 如果下轮继续，这条线最该补的是最小 Unity live 验，验证偏头是否收敛，而不是再回去重动导航主规则。

## 2026-04-09 11:15 NPC_Hand 全局头像统一第一刀
- 当前主线：
  - 统一 NPC 人物主表、正式对白头像、关系页头像和后续人物资料页的真值来源，避免剧情、关系页、场景里三套头像继续分裂。
- 本轮子任务：
  - 接入用户刚放进 `Assets/Sprites/NPC_Hand` 的手绘头像，并把 formal / relationship 两个当前真实消费口统一回主表。
- 已完成：
  1. `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
     - `ResolveRelationshipPortrait()` 回正为 `handPortrait -> prefab 默认帧`。
  2. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - `ApplyPortrait()` 回正为 `registry portrait -> node.speakerPortrait`，让手绘头像成为 NPC 侧 canonical。
  3. `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
     - 详情头像框与列表头像框新增 `RectMask2D`；
     - 边距改成详情 `8px`、列表 `4px`，更贴边但不出框。
  4. `Assets/Resources/Story/NpcCharacterRegistry.asset`
     - 手绘头像已接上 `001/002/003/103`。
  5. `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
     - 新增“handPortrait 是双端 canonical”的护栏测试。
- 关键判断：
  - 用户后续提出的方向更优：不要继续手工逐项挂图，而应该把 `NPC_Hand` 做成真源路径，并在 runtime 侧只吃一次性字典缓存；
  - 正确 contract 应是：
    1. 先按 NPC ID 找 `NPC_Hand`
    2. 找不到再回退默认动画帧
    3. 不允许每次现搜，更不允许全局低性能扫目录
- 验证：
  - `validate_script` 对 `NpcCharacterRegistry.cs / DialogueUI.cs / PackageNpcRelationshipPanel.cs / NpcCharacterRegistryTests.cs`
    - 统一结果：`owned_errors=0`
    - `assessment=unity_validation_pending`
  - `git diff --check`
    - 无空白错误
    - 仅 `DialogueUI.cs` 存在 `CRLF -> LF` 提示
  - CLI/MCP 当前没拿到活动 Unity 实例，所以 live 终验未补，不 claim 已经现场过线。
- 当前未完成：
  - 还没把 `NPC_Hand` 真源字典真正落地；
  - 还没把更多新加头像（如 `101/104` 及后续）自动吃进系统。
- 恢复点：
  - 下一刀直接做“`NPC_Hand` 一次性字典索引 + fallback contract”，把现在的手工挂图降级成兼容层。

## 2026-04-09｜slice=`npc-scene-obstacle-autocollect-and-response-rebalance`

- 用户最新裁定：
  - 认可“问题更像场景处理 + 被止血补丁压钝的响应”，要求直接落地，不再继续误伤避让核心。
- 本轮真实施工：
  - 已强制把 thread-state 切到：
    - `npc-scene-obstacle-autocollect-and-response-rebalance`
  - 本轮 owned paths：
    - `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- 本轮真正做成：
  1. `TraversalBlockManager2D`
     - 增加场景静态阻挡自动补全，不再只依赖手填阻挡列表
     - 默认自动收集名字命中 `wall/props/fence/rock/tree/border` 的同场景静态 tilemap
     - 自动排除：
       - 动态导航单位
       - trigger
       - `walkableOverride`
       - `base/grass/ground/background/bridge`
     - `water` 仍通过 soft-pass 关键词进入软穿越链
  2. `NPCMotionController`
     - 撤回前一版重平滑朝向方案
     - 回到“移动矢量四扇区 + 轻去抖”
     - 方向和物理移动重新贴紧，不再让朝向自己漂开
  3. `NPCAutoRoamController`
     - 动态刚体 NPC 仍保留短窗口决策复用以止血性能
     - 但在 `blocked / sharedAvoidance / detour / no-progress` 状态下，禁止继续复用旧决策
     - 目标是把避让即时性拉回来，而不是整体取消性能止血
- 代码层验证：
  - `manage_script validate TraversalBlockManager2D`
    - `clean`
  - `manage_script validate NPCMotionController`
    - `warning only`
    - `Consider using FixedUpdate() for Rigidbody operations`
  - `manage_script validate NPCAutoRoamController`
    - `warning only`
    - `String concatenation in Update() can cause garbage collection issues`
  - `git diff --check`
    - 通过
- 这轮没做：
  - 没有直接改 `Town.unity` / `Primary.unity`
  - 没有回退玩家桥/水/边缘 traversal
  - 没有拿到新的 Unity live 证据
- 当前恢复点：
  - 如果继续，这条线最该做的是最小 live 验证，不再继续盲改算法。

## 2026-04-09｜slice=`npc-avoidance-response-restore`

- 用户最新反馈：
  - `NPC` 现在“还没有任何避让”。
- 本轮只收一个真责任点：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
- 我这轮钉死的根因：
  - 不是 `TryHandleSharedAvoidance(...)` 没有解算出绕行方向；
  - 而是我之前加的 `StabilizeMoveDirection(...)` 在 `sharedAvoidanceBlockingFrames > 0` / `hasDynamicDetour` 这些本该允许变向的窗口里，继续把方向压回 `lastIssuedMoveDirection`。
  - 所以视觉与运动结果看起来就像“完全不避让”。
- 已落修复：
  1. `TickMoving()` 里只在“没有 shared avoidance block、没有 detour”时才继续做方向稳定
  2. `ShouldHoldPreviousMoveDirection()` 去掉：
     - `sharedAvoidanceBlockingFrames > 0`
     - `hasDynamicDetour`
  3. 保留：
     - `hasPendingMoveCommandProgressCheck`
     - `blockedAdvanceFrames > 0`
     的稳定条件，用来继续挡住真正的原地翻向抖动
- 验证：
  - `manage_script validate NPCAutoRoamController`
    - `warning only`
    - `errors=0`
  - `git diff --check`
    - 通过
- 当前恢复点：
  - 这轮真正值得补的是 live，看避让是否已经恢复；不是再回去乱砍更多逻辑。

## 2026-04-09 12:53 NPC_Hand 真源字典与自动同步完成
- 当前主线：
  - 把 NPC 头像从“formal / 关系页 / 人物资料三套来源”继续往同源主表收，并且必须面向打包态。
- 本轮已完成：
  1. `Assets/YYY_Scripts/Story/Data/NpcCharacterRegistry.cs`
     - 加入 runtime 字典缓存：
       - `npcId -> Entry`
       - `speaker/alias -> Entry`
       - `npcId -> handPortrait`
     - 新增 `TryResolveHandPortrait`
     - fallback 固定为：`handPortrait -> prefab 默认帧`
  2. `Assets/Editor/NPC/NpcCharacterRegistryHandPortraitAutoSync.cs`
     - 正式把 `Assets/Sprites/NPC_Hand` 变成 editor 真源目录
     - 目录变化会自动同步到 `NpcCharacterRegistry.asset`
     - 也提供手动同步菜单
  3. `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
     - 测试不再写死头像名单
     - 改成按 `NPC_Hand` 当前目录真实文件来验
  4. `Assets/Resources/Story/NpcCharacterRegistry.asset`
     - 当前已真实同步：`001 / 002 / 003 / 101 / 103 / 104`
- 关键判断：
  - 这条链现在是打包态安全的，因为 runtime 不扫目录，只吃序列化引用和字典缓存；
  - `NPC_Hand` 目录扫描只发生在 editor 自动同步器里。
- 验证：
  - `validate_script`：
    - `NpcCharacterRegistry.cs`
    - `NpcCharacterRegistryHandPortraitAutoSync.cs`
    - `NpcCharacterRegistryTests.cs`
  - 统一结果：
    - `owned_errors=0`
    - `manage_script validate = clean`
    - `assessment=unity_validation_pending`
  - Unity console：
    - own red = 0
    - 可见唯一 error 为外部编辑器噪音：
      - `GridEditorUtility.cs: Screen position out of view frustum`
  - `run_tests(NpcCharacterRegistryTests)`：
    - 没成功启动，停在 `tests did not start within timeout`
- 当前恢复点：
  - 这轮结构已经站住；
  - 下一刀最值钱的是 live 终验，看 formal 对白与关系页是否真变成同一张 hand 头像，而不是再写新壳。

## 2026-04-09 16:58 NPC 简介内容梳理完成
- 当前主线：
  - 继续把 NPC 线从“能显示”往“内容和结构都像正式人物册”推进。
- 本轮子任务：
  - 不写代码，先把 NPC 简介的排版、分区和内容规划收成一份统一梳理稿。
- 已完成：
  - 新增文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-09-NPC人物简介排版分区与内容梳理.md`
  - 文档已明确：
    1. 关系页不是百科，而是“当前剧情阶段里你对这个人的记录”
    2. 固定分区应该是：
       - 顶部识别区
       - 身份与位置
       - 今日你会看到的样子
       - 你为什么会记住他
       - 后续预留区
    3. 全部 11 个 NPC 的当前真值梳理与推荐文案方向
    4. 现在不该做的事：不要百科化、不要过度剧透、不要把系统字段塞满正文
- 依据：
  - 回看了 `NpcCharacterRegistry.asset`
  - 回看了 `PackageNpcRelationshipPanel.cs`
  - 回看了 Day1 正式对白中的已有角色真值
- 当前判断：
  - NPC 简介现在最大的短板不是“句子不够”，而是“信息架构没彻底统一”；
  - 这份文档已经把后续精修的标准先立住了。
- 当前恢复点：
  - 下一刀如果继续，优先应按这份梳理先收关系页内容结构，再逐人精修文案，不要直接散修单句。

## 2026-04-09 17:41 NPC 简介分工 prompt 已生成
- 当前主线：
  - 把 NPC 简介这条线从“结构梳理”推进到“可并行分工执行”。
- 本轮已完成：
  - 基于简介结构梳理稿，拆出 3 份可直接转发的 prompt：
    1. 给 `UI`：只收关系页排版与分区结构
    2. 给 `Day1`：只核剧情真值与曝光边界
    3. 给 `NPC`：只回填 NPC own 的简介文案与关系册内容层
- 关键判断：
  - 这一步必须做，否则前一份梳理稿还只是“知道该怎么做”，不是“已经能开始分工执行”。
  - 现在这条线的边界已经压清，不该再出现 UI 编内容、Day1 改壳、NPC 回吞整页的漂移。

## 2026-04-09 18:15 紧急修复：剧情玩家/旁白头像切到 000
- 当前主线里的插入式子任务：
  - 用户要求先紧急处理正式剧情里玩家与旁白头像，统一换成 `Assets/Sprites/NPC_Hand/000.png`
- 已完成：
  1. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 新增玩家/旁白特殊头像分支
     - inner monologue 不再直接隐藏头像
     - 玩家台词和旁白改为统一回 `000`
  2. `Assets/Resources/Story/NpcCharacterRegistry.asset`
     - 新增 `000` 特殊条目，作为剧情玩家头像源
  3. `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs`
     - `旅人 / 陌生旅人` 必须解析到 `000`
- 验证：
  - `validate_script DialogueUI.cs`
    - 首轮 own red 已修掉
    - 复跑后 `owned_errors=0`
  - `validate_script NpcCharacterRegistryTests.cs`
    - `owned_errors=0`
  - `errors`
    - `0 error / 0 warning`
- 当前恢复点：
  - 这刀已经站住；
  - 如果下一步继续，优先补 live 验证“旅人台词 + 内心独白”是否真实显示 000 头像，再回到原主线。
