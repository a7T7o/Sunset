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

## 2026-03-23 NPC第二刀表现层与碰撞体感精修

- 当前主线目标：按 `main-only` 新口径继续推进 NPC 自己的下一刀，只修气泡表现与 NPC 自身碰撞体感，不越界去动导航核心。
- 本轮子任务：完成倒三角尾巴朝下、尾巴单独轻微跳动、气泡与字体适度放大，以及 NPC 默认刚体更难被玩家顶开的第二刀精修。
- 本轮完成：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 升到 `styleVersion = 6`，把默认气泡抬高、放大并增加留白；尾巴运行时纹理改成上宽下尖的倒三角；新增 `tailBobAmplitude / tailBobFrequency`，只让尾巴自己轻微上下跳动。
  - `Assets/Editor/NPCPrefabGeneratorTool.cs` 的默认 `Rigidbody2D` 改为 `mass = 6`、`linearDamping = 8`、`interpolation = Interpolate`，保持 Dynamic + Continuous，不改导航系统本体。
  - `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 已同步成新气泡参数与新刚体参数，确保“代码默认值 / 生成器默认值 / 样本 prefab 真相”一致。
- 本轮验证：
  - 对 `NPCBubblePresenter.cs`、`NPCPrefabGeneratorTool.cs`、`001~003.prefab` 执行了 `git diff --check`，通过。
  - 只读回看确认样本 prefab 当前已为 `m_Mass: 6`、`m_LinearDamping: 8`、`m_Interpolate: 1`、`tailBobAmplitude: 2.2`、`tailBobFrequency: 2.1`、`styleVersion: 6`。
  - 尝试通过 MCP 执行 `recompile_scripts` 获取最小 live 编译证据，但当前返回 `Connection failed: Unknown error`，因此本轮仍只能写实为“静态验证通过，Unity live 编译证据未取得”。
- 当前恢复点：NPC 现在已经具备第二刀本地 checkpoint 条件；下一步若继续，应优先做一次 `Primary` 场景的 live 目测验收，看尾巴指向感和玩家推挤体感是否达标。

## 2026-03-23 NPC气泡第三刀与003持续说话压测

- 当前主线目标：继续把 NPC 气泡的“太高、包边不稳、长文本压测不足”收紧到可直接验收的状态。
- 本轮子任务：只改 NPC 自己的气泡布局逻辑与 `003` 测试用 prefab，不碰导航核心。
- 本轮完成：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 升到 `styleVersion = 7`，加入自适应换行宽度与文本安全边距，让长文本不再只横向拉长；同时把气泡整体再下收一小段，避免尾巴离 NPC 头顶太远。
  - 新增 `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs`，把 `003` 做成持续说话测试 NPC，内置短句/中句/长句混合文案，最长约 50~60 字。
  - `Assets/222_Prefabs/NPC/003.prefab` 挂上 `NPCBubbleStressTalker`，并开启 `disableRoamWhileTesting`，确保压测时不再被漫游状态机抢走气泡。
  - `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 同步到最新气泡位置参数与 `styleVersion = 7`。
- 本轮验证：
  - `unityMCP.manage_scene(get_active)` 返回 `Primary`，`unityMCP.read_console(get)` 可正常读取，确认当前会话确实挂在新的 `unityMCP` 上。
  - `unityMCP` 读回 `003` 场景实例组件，确认 `NPCBubbleStressTalker` 已挂载，`ShowCount = 2`、`LastShowSucceeded = true`。
  - 同一轮 live 读回 `NPCBubblePresenter`，确认 `IsBubbleVisible = true` 且 `CurrentBubbleText` 为长句，说明持续说话压测已真正跑起来。
  - `git diff --check` 对本轮 NPC 相关脚本和 prefab 通过。
- 当前恢复点：现在你可以直接进 `Primary` 看 `003` 头顶的持续说话效果，重点验三件事：尾巴是否够低、长句是否被气泡完整包住、整体高度是否比上一版更自然。

## 2026-03-23 NPC第四刀：内边距收口与场景化入口

- 当前主线目标：把 NPC 气泡收口到更适合正式验收的观感，并把后续场景化配置入口搭顺。
- 本轮子任务：修正气泡内边距，正规化 `003` 测试模式，并增强 `NPCAutoRoamController` 的配置入口。
- 本轮完成：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`：将 `textSafePadding` 从 `{12,10}` 提升到 `{18,16}`，并进一步缩小文字实际占用区域，确保文字四周离边框更远。
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs`：补齐正式测试入口能力，新增公开状态、一次性发话接口、显式测试模式开关与组件菜单；`003.prefab` 不再靠运行时猜引用，而是显式绑定 `bubblePresenter` 与 `roamController`。
  - `Assets/Editor/NPCBubbleStressTalkerEditor.cs`：新增测试组件专用编辑器，让 `003` 的持续说话测试模式成为明确的编辑器入口，而不是临时脚本状态。
  - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 与 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`：新增 Home Anchor 创建/吸附/清空、Profile 复制/选中/应用、测试组件提示与运行时状态读取，正式进入 NPC 场景化与集成阶段的配置入口建设。
- 本轮验证：
  - 本轮 NPC 相关脚本与 prefab 的 `git diff --check` 通过。
  - 本轮早些时候 `unityMCP` 曾成功读回 `Primary` 与 `003` 的 stress talker live 状态；但后续 RMCP 资源层开始对 `旧 MCP 端口口径（已失效）/mcp` 握手失败，因此当前收尾口径应写实为：代码与 prefab 静态检查通过，Unity live 需在下一次稳定连接下补终验。
- 当前恢复点：如果用户认可这轮收口，下一步就不是继续救火，而是直接进入 NPC 场景化配置与导航交付后的接入准备。

## 2026-03-23 NPC第五刀：内边距收口与场景化入口增强

- 当前主线目标：在用户已认可大部分效果的前提下，只收口剩余的气泡内边距，并把 NPC 场景化与集成阶段的入口搭顺。
- 本轮子任务：修正文字距边框过近的问题，正规化 `003` 的测试模式，并增强 `NPCAutoRoamController` 的配置入口。
- 本轮完成：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`：将 `textSafePadding` 提升到 `{18,16}`，同时进一步缩小文本实际占用区域，确保文字四周离边框更远。
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs`：从临时压测脚本升级为正式测试组件，新增公开状态、一次性发话接口、显式测试开关，并通过 `AddComponentMenu` 明确归类。
  - `Assets/Editor/NPCBubbleStressTalkerEditor.cs`：新增测试组件专用编辑器，允许在 Inspector 中重绑引用、查看状态、手动触发一次发话。
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`：新增 `HomeAnchor`、活动范围、路径点数等公开读取，以及 `SetHomeAnchor` / `SyncHomeAnchorToCurrentPosition` 入口。
  - `Assets/Editor/NPCAutoRoamControllerEditor.cs`：新增 Scene Integration 面板，可创建/选中/清空 Home Anchor、复制/应用/选中 Profile，并提示测试组件状态。
  - `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab`：同步内边距参数；`003.prefab` 的 `NPCBubbleStressTalker` 现已显式绑定 `bubblePresenter` 与 `roamController`。
- 本轮验证：
  - `git diff --check` 对本轮 NPC 相关脚本、编辑器脚本和 prefab 通过。
  - `unityMCP` 在本轮开始前一度可以正常读写，但收尾阶段通用 RMCP 握手回到 `旧 MCP 端口口径（已失效）` 失败；因此本轮 live 终验未补齐，只能写实为 transport 阻塞。
- 当前恢复点：NPC 线已正式进入“场景化与集成阶段”，下一步应围绕 `Home Anchor / 活动范围 / Profile` 的真实场景配置继续推进，并等待导航线交付后做接入验收。

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

## 2026-03-23 NPC场景化与集成首版入口

- 当前主线目标：把 NPC 线从“气泡与测试功能已经具备”推进到“正式/验证模式已经分层，Scene 接入已有统一入口”。
- 本轮子任务：先完成 NPC 自己能独立落地的 profile 分层、prefab 角色归类、批量 Scene 集成工具与收口说明，不越界碰导航核心。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_BubbleReviewProfile.asset`，作为 `003` 这类验证样本的专用 roam profile。
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCSceneIntegrationTool.cs`，支持批量补 Home Anchor、吸附 Anchor、切 `Production / BubbleReview` 模式、以及批量增删 `NPCBubbleStressTalker`。
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab` 现在默认指向 `NPC_BubbleReviewProfile.asset`，明确从“临时压测 NPC”转成“验证样本 prefab”。
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-23_NPC场景化与集成首版收口.md`，把原有配置、问题原因、建议修改、效果和影响写成正式收口说明。
- 本轮验证：
  - 只读核查 `Primary.unity`，确认当前场景里的 `001 / 002 / 003` 还没有 `homeAnchor` 场景级覆盖，因此场景化落点确实还没做完。
  - `git diff --check` 已通过本轮 NPC 相关改动。
- 当前恢复点：下一步如果继续推进，就不是再发散新需求，而是在 `Primary.unity` 中用 Scene Integration Tool 真正把正式 NPC 与验证样本的 Home Anchor / 活动范围落到场景里。

## 2026-03-23 NPC生成器继续收口：正式/验证样本自动分流

- 当前主线目标：继续把我自己还没做完的 NPC 场景化与集成工作往前推，但先只做不碰热场景的工具链收口。
- 本轮子任务：把“正式 NPC / 验证样本 NPC”的角色分流做进 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`，减少后续批量生成后的手工补配。
- 本轮完成：
  - 生成器新增 `GeneratedNpcRole`，支持在生成阶段区分 `Production` 与 `BubbleReview`。
  - Inspector 新增“角色预设”区域，可配置验证样本名称列表，默认把 `003` 识别为验证样本。
  - 生成器现在会自动给正式 NPC 绑定 `NPC_DefaultRoamProfile.asset`，给验证样本绑定 `NPC_BubbleReviewProfile.asset`。
  - 对验证样本可自动补挂 `NPCBubbleStressTalker`，避免后续再手工把 `003` 这类样本改成 review 模式。
  - 生成完成后的摘要会直接显示本次生成里“正式 / 验证样本”的数量。
- 本轮验证：
  - `git diff --check -- Assets/Editor/NPCPrefabGeneratorTool.cs` 已通过。
  - 本轮没有触碰 `Primary.unity`、Prefab、字体资源或其他 Unity 热区资产，只修改了编辑器工具脚本。
- 当前恢复点：NPC 线现在已经把“验证样本不要再靠人工口头区分”这件事收进了生成器；下一步若继续安全推进，应优先继续做工具链/配置入口层，而不是立刻去碰当前仍有他线 dirty 的 `Primary.unity`。

## 2026-03-23 导航回执最终版更新

- 当前主线目标：把 NPC 给导航线程的交接口径更新成真正可直接转发的最终版，而不是继续沿用旧 HEAD、旧推送状态和旧职责边界描述。
- 本轮子任务：重写 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\给导航线程的正式回执_2026-03-23.md`，只更新回执，不扩写业务代码。
- 本轮完成：
  - 将导航回执中的 live 基线更新到 `main @ 3d5978b088bfa9d910d04417501394f098fd26b2`。
  - 明确补入 `003` 已正式样本化、`NPCSceneIntegrationTool` 已在位、`NPCPrefabGeneratorTool` 已支持正式/验证样本自动分流。
  - 重新锁定导航线程应认领的问题：玩家右键导航推着 NPC 走、NPC/NPC 互撞缺少动态避让、缺少动态占位 / 礼让 / 重规划闭环。
  - 重新锁定 NPC 线程给导航线程的稳定承诺：状态语义、`IsMoving`、`rb.MovePosition(...)`、`001/002/003` 碰撞基线与 `003` 验证样本定位当前都视为稳定基线。
- 本轮验证：
  - 本轮只改导航回执文档，没有碰代码、Prefab、Scene、字体或其他热区资源。
- 当前恢复点：对导航线程的最终口径现在已经齐了；若用户确认，就可以把这份文档当成正式导航协作输入。 
