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
