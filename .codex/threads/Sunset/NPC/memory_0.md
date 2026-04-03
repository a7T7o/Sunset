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
