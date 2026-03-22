# NPC 1.0.0 初步规划 memory

## 2026-03-16

- 当前主线目标：围绕固定 4 向 3 帧 NPC 模板，完成从 PNG 生成到运行时漫游/气泡行为的第一阶段闭环，并为后续脑暴保留统一规划底稿。
- 本轮服务的子任务：修复当前现场编译阻断，恢复 NPC 第二阶段真实代码与资产，并用 Unity live 证据重新确认自动漫游确实可运行。
- 本轮完成：
  - 解除 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs(2512,34)` 对 `ClearAllQueuePreviews(bool)` 的调用阻断，实际通过更新 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 完成兼容。
  - 恢复 `NPCAutoRoamController`、`NPCBubblePresenter`、`NPCRoamProfile`、`NPCAutoRoamControllerEditor`、`NPC_DefaultRoamProfile.asset`、`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/`。
  - 更新 `NPCAnimController` 防守性逻辑，运行时在 Animator 尚未绑定控制器时会跳过参数驱动，避免旧报错路径反复刷屏。
  - 更新 `NPCPrefabGeneratorTool` 的切片逻辑，消除 `TextureImporter.spritesheet` 废弃 warning。
- 本轮验证：
  - `Primary` 场景进入 Play 后，Console 只剩 `There are no audio listeners in the scene` 非 NPC 阻断 warning。
  - 读取 `001` / `002` / `003` 的 `NPCAutoRoamController` 单组件资源，确认 `IsRoaming=true`，并再次捕获到 NPC 附近聊天正样本。
  - 重新进入 Play 后，`Missing Script` 瞬时日志未再次复现，可判定不是当前稳定阻断。
- 遗留问题：
  - 当前真实 Git 现场仍位于 `codex/farm-1.0.2-correct001`，不是语义最优的 NPC 分支；后续若继续扩展 NPC 主线，应在提交说明里明确白名单范围。
- 恢复点：自动漫游第二阶段已恢复，下一轮可以继续做更细的 NPC 行为策划实现，或围绕生成工具做进一步压缩和验收。

## 2026-03-17

- 本轮主线：处理用户发现的 NPC 预制体与动画 `Sprite` 全部丢失问题，查清到底是哪一步把 `001/002/003` 弄坏了。
- 已证实根因：
  - `Assets/222_Prefabs/NPC/001.prefab` 以及对应动画 `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 等文件都在引用 `guid: b7b3124ae2dcef34e9e08258e832d515` 下的方向化 Sprite 子资源，例如 `001_Down_1 -> fileID 3869598651494229176`。
  - 当前现场的 `Assets/Sprites/NPC/001.png.meta` 在修复前仍是旧的自动切片版，只包含 `001_0 / 001_1 / ...` 这套子资源 ID，不包含 `001_Down_1 / 001_Left_1 / ...` 这套方向化 ID。
  - 上一轮回填时带回了 Prefab、动画和运行时代码，但漏带了 `Assets/Sprites/NPC/001.png.meta`、`002.png.meta`、`003.png.meta`，导致 Prefab 和动画同时出现 `缺失(精灵)`。
- 本轮修复：
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/001.png.meta`
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/002.png.meta`
  - 从 `codex/npc-roam-phase2-001` 恢复 `Assets/Sprites/NPC/003.png.meta`
  - 执行 Unity refresh，让 AssetDatabase 重新解析子 Sprite 引用。
- 本轮验证：
  - `001` 场景实例的 `SpriteRenderer.sprite` 已恢复为 `Assets/Sprites/NPC/001.png`，不再是 `null`。
  - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 与 `001_Move_Down.anim` 的 `m_PPtrCurves` 当前引用的 fileID 已与修复后的 `.meta` 对齐。
  - 当前 Unity Console 为 0 条，未再出现本轮的 `Missing Sprite` / `Missing Script` 噪音。
- 当前恢复点：NPC 的 PNG 子资源 ID 已重新和 Prefab/动画链路对齐，三张 NPC 贴图依赖已补全。

- 当前主线目标：在继续 NPC 主线前，先把“为什么这条 NPC 线程后来会落到 farm 分支上继续提交”做成可交给治理线程的写实分析。
- 本轮子任务：只读核查 NPC 线程相关的 Git 分支、worktree、reflog 与线程规则，澄清分支和 worktree 的关系并重建时间线。
- 已证实事实：
  - 当前根仓库真实现场是 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`，`HEAD = 11e0b7b4`，不是 `main`，也不是某条 `codex/npc-*` 分支。
  - 旧 NPC 独立 worktree 仍然存在于 `D:\Unity\Unity_learning\Sunset_worktrees\NPC`，其检出分支是 `codex/npc-generator-pipeline`；这说明“NPC 默认不再用独立 worktree”是规则口径，但旧 worktree 现场并未物理消失。
  - `codex/npc-fixed-template-main`、`codex/npc-asset-solidify-001`、`codex/npc-main-recover-001`、`codex/npc-roam-phase2-001` 这些 NPC 分支都是真实存在的仓库分支，不“属于”某个 worktree；只有 `codex/npc-generator-pipeline` 当前仍附着在旧 NPC worktree 上。
  - `git reflog` 显示 2026-03-17 01:35 从 `codex/farm-1.0.2-correct001` 切到 `codex/npc-roam-phase2-001`，01:44 又切回 `codex/farm-1.0.2-correct001`；随后 `07ffe199`、`18f3a9e1`、`11e0b7b4` 连续提交都发生在 `codex/farm-1.0.2-correct001`。
  - `07ffe199` 与 `18f3a9e1` 这两次提交实际承载了 NPC 二阶段代码、动画、Prefab 与 PNG meta 修复，且 `git branch --contains` 结果表明它们当前只在 `codex/farm-1.0.2-correct001` 上。
- 当前判断：
  - “为什么会回到 farm” 的主观动机无法从 Git 直接证明，但从时间线看，最合理解释是：根仓库当时已经在 farm 线工作，短暂切去 NPC 分支做恢复后，又回到了原活跃根分支，导致后续 NPC 成果提交到了 farm。
  - 我此前把“历史上存在 NPC worktree”说成了“现在 NPC 就是在那个 worktree 上”，表述失真；更准确的说法应是“规则上 NPC 现在线程默认应从根仓库 `main` 进入，但 Git 现场仍保留旧 NPC worktree，同时本轮根仓库又实际停在 farm 分支上”。
- 恢复点：NPC 这条线当前新增了完整的 Git 取证结论，下一步应由治理线程裁定如何处理“NPC 成果落在 farm 分支”与“旧 NPC worktree 残留”的收口方案。

- 当前主线目标：按治理裁定，把 NPC 的后续可写基线收敛到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再借用共享根目录 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`。
- 本轮子任务：只读确认 `f6b4db2f` 是否已经包含 NPC 核心资产 / Prefab / meta / 运行时代码，并判断 `FarmToolPreview.cs` 与 `NPCPrefabGeneratorTool.cs` 的最小收口策略。
- 已证实事实：
  - `git ls-tree -r f6b4db2f` 已包含 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/001~003.prefab`、`Assets/Sprites/NPC/001~003.png(.meta)`、`Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`NPCBubblePresenter.cs`、`NPCMotionController.cs`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs` 与 `Assets/Editor/NPCPrefabGeneratorTool.cs`。
  - `f6b4db2f` 对比 farm 上 `18f3a9e1`，NPC 范围内只剩 `Assets/Editor/NPCPrefabGeneratorTool.cs` 与 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 两个差异文件；说明 `f6b4db2f` 本身已经带齐 NPC 核心资产，不缺额外 Prefab / meta / 脚本。
  - `f6b4db2f` 对比其上游 `8aed637f`，真正额外拖入 NPC 线的 farm 侧文件只有 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - `git grep` 显示在 `f6b4db2f` 中，`GameInputManager.cs` 仍只调用无参 `ClearAllQueuePreviews()`，NPC 线内没有其他代码依赖 `FarmToolPreview.ClearAllQueuePreviews(bool)` 这组 farm 侧扩展。
- 关键判断：
  - `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的这次改动不应继续留在 NPC 救援线中；最小剔除方式是把该文件回退到 `8aed637f` 版本。
  - `Assets/Editor/NPCPrefabGeneratorTool.cs` 继续保持 `f6b4db2f` 版本即可作为 NPC 救援基线，因为它已经和这条线里的 Prefab / 漫游组件 / 默认 profile 链路一致；不需要再借用 farm 上更旧的那版。
- 恢复点：NPC 现已可以以 `codex/npc-roam-phase2-001 @ f6b4db2f` 作为唯一救援基线继续收口；下一步最小动作应是在独立 NPC 可写现场里，仅剔除 `FarmToolPreview.cs` 的 farm 侧拖带改动，并做一次 Unity 编译 + `001/002/003` Prefab/动画引用验证。

## 2026-03-22

- 当前主线目标：在 `main-only` 口径下，把 NPC phase2 这轮返工直接落到 `main`，只解决两件事：气泡真正抬离头顶、NPC 改成下半身实体碰撞。
- 本轮子任务：参考 `codex/npc-roam-phase2-003` 已做对的修正，但不再去该分支开发，只把必要内容重落到 `D:\Unity\Unity_learning\Sunset @ main`。
- 本轮完成：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 升到 `styleVersion = 5`，加入基于 `SpriteRenderer.bounds` 顶边、气泡底边与稳定留白的抬高逻辑，并同步调整字号、尾巴、阴影、显隐与轻微浮动参数。
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 接入 `Rigidbody2D`，移动阶段优先改为 `FixedUpdate + rb.MovePosition()`，只在缺少刚体时 fallback 到 `transform.position`。
  - `Assets/Editor/NPCPrefabGeneratorTool.cs` 默认改为生成下半身 `BoxCollider2D + Rigidbody2D` 的 NPC 模板，不再生成整身 `CircleCollider2D + isTrigger=true`。
  - `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 已同步成新版碰撞体、刚体、气泡参数与 `rb` 绑定。
- 本轮验证：
  - `git diff --check` 已通过上述 6 个 NPC 业务文件。
  - 只读回看 prefab YAML，`001/002/003` 当前都已存在 `BoxCollider2D`、`Rigidbody2D`、`m_IsTrigger: 0`、`styleVersion: 5` 和 `rb` 绑定。
  - Unity / MCP 本轮未形成可靠 live 验证，`recompile_scripts` 返回连接失败；因此当前只能写实为“代码与 prefab 已按 main-only 返工落地，仍待主项目手测确认气泡观感与碰撞体感”。
- 兼容边界：本轮没有扩写攻击 / 工具命中 / 交互系统，只保留后续兼容所需的刚体与碰撞基础。
- 当前恢复点：`main` 已带上 NPC 两项核心返工；下一步真实验收应回到主项目，看气泡是否离脸、以及玩家/NPC 与 NPC/NPC 是否仍可穿透。

## 2026-03-22 导航与气泡返工需求澄清

- 当前主线目标：只读厘清 NPC 当前气泡、碰撞与导航能力边界，为下一轮申请修正和加强准备准确口径。
- 本轮已证实事实：
  - NPC 漫游当前确实调用了导航：`Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 中 `TryBeginMove()` 会调用 `navGrid.TryFindPath(...)`，卡住后 `TryRebuildPath()` 也会重新求路。
  - 但 NPC 当前没有局部动态避让：`NPCAutoRoamController` 移动阶段只是沿静态路径点 `rb.MovePosition(nextPosition)` 前进；如果撞住，只会靠卡住检测后“重建同一条或重新抽一条路径”，没有 agent-agent separation / yielding / reservation / 路口礼让。
  - 当前 NavGrid2D 是静态网格 A*：`Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs` 通过 `Physics2D.OverlapCircleAll` 重建 `walkable[,]`，只有在 `Awake/OnEnable/Start` 或外部 `OnRequestGridRefresh` 时刷新；并不会随着 NPC/玩家移动持续更新动态占用。
  - `Primary.unity` 里 NavGrid2D 的 `obstacleTags` 当前是 `Interactable / Building / Tree / Rock / Placed`，不包含 NPC；而 NPC prefab 当前 `m_TagString: Untagged`。这说明导航网格默认并不把 NPC 当成高层路径障碍。
  - 玩家自动导航比 NPC 多一层“局部碰撞偏转”：`Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` 的 `AdjustDirectionByColliders()` 会对前方非 Trigger 碰撞体做排斥修正，所以玩家导航路上遇到 NPC 可能会“尝试侧一下”，但这只是局部 steering，不是全局动态重规划，更不是双向礼让系统。
  - 玩家和 NPC 当前都使用 `Dynamic Rigidbody2D`，且质量都为 `1`：`Primary.unity` 中玩家 `RunSpeed=6 / WalkSpeed=4`、`m_Mass: 1`；NPC prefab 中 `m_Mass: 1`。在这种配置下，玩家持续直接写 `rb.linearVelocity`，而 NPC 用 `MovePosition`，体感上就会出现“玩家比较容易把 NPC 顶开”。
  - 当前气泡已经有轻微上下浮动，但不是“箭头单独浮”，而是整颗气泡一起浮：`NPCBubblePresenter` 当前 `visibleFloatAmplitude = 0.024`、`visibleFloatFrequency = 1.6`；尾巴本身就是朝下的，但尺寸和视觉指向性仍偏弱。
- 用户新确认的表现层需求：
  - 箭头需要更明确地朝下指向 NPC，并带轻微悬浮感。
  - 气泡整体需要再大一点。
  - 字体需要再大一点，达到“看得清但不夸张”。
  - 气泡与头顶之间的留白要继续稳定，不能压脸。
- 用户新确认的碰撞/导航需求：
  - 玩家不应太容易把 NPC 顶开，NPC 需要更强的移动坚持度或抗推动能力。
  - NPC 与 NPC 不能只会撞成一团后原地互怼。
  - 玩家自动导航过程中，如果路上突然有 NPC 经过，理想上应能自动规避，而不是只靠硬顶或卡住取消。
  - 后续希望支持 NPC 之间互相规避、路口礼让、动态占位，而不只是静态 A* + 卡住重算。
- 当前需要申请修正和加强的系统缺口：
  - 缺少 NPC 本地避障 / 分离 steering。
  - 缺少动态障碍占用与实时重规划机制。
  - 缺少 agent 优先级 / 路口礼让 / 会车协商。
  - 缺少玩家与 NPC 的物理交互权重设计（质量、推挤、接触策略、移动模式统一）。
  - 缺少针对 NPC 的独立导航验收标准，当前系统更多是“能走”而不是“走得像人”。
- 当前恢复点：这轮已经把“当前有没有调用导航”“导航具体缺在哪”“气泡下一步要怎么改”说实了；下一轮若进入实现，应优先先收表现层小修，再立项处理动态导航与让行系统增强。

## 2026-03-22 职责边界纠正（NPC线程 vs 导航线程）

- 用户纠正点已确认：上一轮输出把“NPC线程自己的返工”和“导航线程应认领的导航增强”混在了一起，职责边界不清，口径错误，需要重写。
- 用户视觉验收口径纠正：
  - 当前气泡尾巴从最终观感看是“正三角朝上”，不符合需求。
  - 正确需求应为“倒三角朝下，明确指向 NPC”。
  - 需要尾巴本身做轻微上下跳动，形成指向 NPC 的提示感，而不是只让整颗气泡一起浮。
  - 气泡整体要再大一点，文字要再大一点，但仍保持克制可读。
- NPC 线程本轮应认领的代码范围：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - 倒三角尾巴朝下指向 NPC
    - 尾巴单独轻微上下跳动
    - 气泡尺寸与文字尺寸增大
    - 气泡留白与头顶距离继续调优
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
    - NPC 默认 prefab 的碰撞体尺寸、offset、刚体默认参数
  - `Assets/222_Prefabs/NPC/001.prefab` ~ `003.prefab`
    - 样本 NPC 的碰撞体、刚体、气泡参数同步
  - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 只负责“NPC 如何消费导航结果并驱动自己的移动/动画/卡住恢复”
    - 不负责重写导航系统本体
- NPC 线程不应认领、应移交导航线程的代码范围：
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  - 以及后续所有“动态障碍占用、局部避障、双向让行、路口礼让、动态重规划”相关导航核心逻辑
- 给导航线程的准确需求：
  - 当前 NPC 已经接入 `NavGrid2D.TryFindPath()`，但这只是静态 A* 求路。
  - 现在缺的不是“NPC 有没有调导航”，而是“导航系统没有给 NPC 和玩家提供动态避让能力”。
  - 需要导航线程认领并解决：
    - 玩家自动导航遇到移动中的 NPC 时，能稳定自动规避，而不是只会硬顶或卡住取消
    - NPC 与 NPC 相遇时，能互相规避，而不是撞成一坨后靠 stuck 检测反复重算
    - 窄路/路口存在基础礼让与让行，不再是单纯先到先撞
    - 导航层支持 moving agents 的动态占位，而不是只看静态场景障碍
    - 玩家 agent 与 NPC agent 支持独立半径/占位策略，而不是共用单一 agent 半径
- 当前恢复点：后续实现分工已经明确，NPC 线程先只做 NPC 自己的气泡与 NPC 侧碰撞/消费逻辑；导航增强由导航线程专门接管。

## 2026-03-22 用户强纠正后的最终口径重写

- 当前主线目标：把 NPC 线程自己的返工口径重新写对，严格按“NPC 自己的代码”和“导航线程的代码”分离，不再混淆职责。
- 本轮子任务：只读复核 `main @ c6af2657` 下的 NPC / 导航相关代码，重写给用户与导航线程的交接口径，不做业务修改。
- 本轮已证实：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 当前尾巴贴图生成方式仍是“上尖下宽”的三角形，再配合当前布局，最终观感会更像“正三角朝上”，这和用户要的“倒三角朝下指向 NPC”不一致。
  - `NPCBubblePresenter` 目前做的是“整颗气泡轻微浮动”，不是“尾巴单独轻微上下跳动”。
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 确实已接 `NavGrid2D.TryFindPath()`，说明 NPC 不是完全没接导航；但它只负责消费静态路径并做 stuck 后重算，不负责动态避让系统本身。
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs` 与 `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` 才是导航核心能力边界；动态障碍、会车礼让、局部避让、双向让行应移交导航线程认领。
- 本轮重新锁定的 NPC 线程认领范围：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - `Assets/222_Prefabs/NPC/001.prefab`
  - `Assets/222_Prefabs/NPC/002.prefab`
  - `Assets/222_Prefabs/NPC/003.prefab`
  - `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 中仅限 NPC 如何消费导航结果移动、动画、卡住恢复的部分
- 本轮重新锁定的不归 NPC 线程认领范围：
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
  - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs`
  - 所有动态障碍占位、agent-agent 避让、路口礼让、双向让行、动态重规划、agent 分型策略
- 用户新确认且必须照做的 NPC 表现层需求：
  - 尾巴必须改成“倒三角朝下，明确指向 NPC”
  - 尾巴要单独做轻微上下跳动
  - 气泡整体再大一点
  - 文字再大一点，但保持克制可读
- 当前恢复点：下一轮若进入真实实现，NPC 线程只准继续修 NPC 自己的气泡与碰撞；导航能力缺口改为向导航线程发正式需求单，不再混写成 NPC 线程问题。

## 2026-03-22 导航交接 Prompt 与 NPC自压 Prompt 定稿

- 当前主线目标：把 NPC 与导航的协作边界沉淀成两份可直接执行的 prompt，避免后续再口头反复解释造成跑偏。
- 本轮子任务：基于 `main @ c6af2657` 已核实的 live 事实，分别起草“给导航线程的详细需求 prompt”和“给 NPC 线程自己的详细执行 prompt”。
- 本轮定稿内容：
  - 导航 prompt 会明确写清：当前 NPC 已接静态 A*、真实缺口是动态避让与动态占位、导航线程应认领的代码范围、禁止越界修改 NPC 表现层代码、必须回交带证据的验收报告。
  - NPC 自压 prompt 会明确写清：NPC 线程只修气泡观感、默认碰撞体、NPC 如何消费导航结果，不重写导航核心；并要求收到导航交付后再做 NPC 侧适配验收。
  - 两份 prompt 都会带：目标、现场基线、代码归属、非目标、硬验收项、验证方式、回执格式。
- 当前恢复点：后续只要把这两份 prompt 发出去，就能让导航线程和 NPC 线程按统一边界并行推进，不再混线。
