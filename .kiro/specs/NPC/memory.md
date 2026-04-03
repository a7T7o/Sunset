# NPC 工作区 memory

## 2026-03-16

- 当前主线目标：完成 Sunset 项目的 NPC 通用模板生成、预制体落地，以及自动漫游与环境气泡行为的可运行闭环。
- 当前子工作区：`D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划`
- 本轮阻塞：项目当前真实现场不在预期的 NPC 分支，且 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`、`Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/100_Anim/NPC/`、`Assets/222_Prefabs/NPC/` 等 NPC 第二阶段资产在当前工作树缺失。
- 本轮完成：
  - 从 `codex/npc-roam-phase2-001` 精确回填 NPC 第二阶段运行时代码与资产到当前现场。
  - 回填 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs` 的兼容版本，解除 `GameInputManager -> ClearAllQueuePreviews(bool)` 编译阻断。
  - 修复 `Assets/Editor/NPCPrefabGeneratorTool.cs` 使用 `TextureImporter.spritesheet` 的废弃 API，改为 `ISpriteEditorDataProvider`。
  - 在 `Primary` 场景重新验证 NPC 001/002/003：自动漫游正常、短停/长停节奏正常、附近聊天正样本已再次复现。
- 已验证事实：
  - Unity Console 当前无 NPC 项目级红错。
  - 用户此前给出的 `Animator is not playing an AnimatorController` 本轮未复现。
  - `001` / `002` / `003` 的 `NPCAutoRoamController` 实时状态均可读到 `IsRoaming=true`，且出现了 `chatting with 002`、`joined chat with 001` 等正样本。
- 当前恢复点：NPC 第二阶段代码与基础资产已恢复到可继续开发状态，下一步应围绕 `npc规划001.md` 继续推进后续行为迭代或更精细的手工验收。

## 2026-03-17

- 当前主线目标：继续收口 NPC 资产链路，确保生成出来的 Prefab、动画和源 PNG 子资源引用一致。
- 本轮阻塞：用户在 Unity 中发现 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 的 `SpriteRenderer.sprite` 为空，同时动画窗口里的 Sprite 曲线也全部丢失。
- 本轮完成：
  - 对照 Prefab YAML、动画 YAML 与 `Assets/Sprites/NPC/*.png.meta`，确认断点在子 Sprite fileID 不匹配。
  - 恢复 `Assets/Sprites/NPC/001.png.meta`、`Assets/Sprites/NPC/002.png.meta`、`Assets/Sprites/NPC/003.png.meta` 到与当前 Prefab/动画匹配的版本。
  - 重新刷新 Unity AssetDatabase。
- 已验证事实：
  - `Assets/222_Prefabs/NPC/001.prefab` 当前引用 `m_Sprite.fileID = 3869598651494229176`，修复后的 `Assets/Sprites/NPC/001.png.meta` 已重新包含 `001_Down_1 -> 3869598651494229176`。
  - `Assets/100_Anim/NPC/001/Clips/001_Idle_Down.anim` 与 `001_Move_Down.anim` 的 `m_PPtrCurves` 当前引用的 fileID 均可在修复后的 `.meta` 中找到对应子 Sprite。
  - 读取 `Primary` 场景中 `001` 的 `SpriteRenderer` 实时组件时，`sprite` 已恢复为 `Assets/Sprites/NPC/001.png`。
  - 当前 Unity Console 为 0 条。
- 关键决策：
  - 这次不重生 Prefab、不重切 PNG、不重做动画，只补回缺失的三份 `.meta`，避免再次漂移资源 GUID / fileID。
- 当前恢复点：NPC 资源链路已经从“运行时代码恢复”推进到“源 PNG 子资源引用恢复”，可以继续做用户现场复测。

- 当前主线目标：在继续 NPC 功能开发前，先把 NPC 线程的 Git / worktree 漂移问题取证清楚，避免后续治理与收口继续混线。
- 本轮阻塞：用户发现我口径里反复把 NPC 和旧 worktree 绑定在一起，同时当前根仓库又真实落在 `codex/farm-1.0.2-correct001`，需要解释“NPC 分支、main、worktree、farm”之间到底发生了什么。
- 本轮完成：
  - 只读核查 `git worktree list`、`git branch -vv`、`git reflog --all`，确认当前默认根仓库现场并不在 `main`，而是在 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001`。
  - 确认旧 `D:\Unity\Unity_learning\Sunset_worktrees\NPC` 仍存在，但它只是 `codex/npc-generator-pipeline` 的一个历史检出位置，不代表 NPC 相关分支都“在 worktree 里”。
  - 重建时间线：`codex/npc-asset-solidify-001 -> main -> codex/farm-1.0.0-1.0.1 -> main -> codex/farm-1.0.2-correction001 -> codex/npc-main-recover-001 -> codex/farm-1.0.2-correct001 -> codex/npc-roam-phase2-001 -> codex/farm-1.0.2-correct001`。
  - 确认 `07ffe199`、`18f3a9e1` 两个关键 NPC 修复提交实落在 `codex/farm-1.0.2-correct001`，并未进入 `main`，也不在现存的 `codex/npc-*` 线上。
- 已证实事实：
  - 分支是仓库级对象，worktree 只是某个分支/提交的检出目录；`main` 分支始终存在，当前指向 `b9b6ac48`。
  - `codex/npc-fixed-template-main`、`codex/npc-asset-solidify-001`、`codex/npc-main-recover-001`、`codex/npc-roam-phase2-001` 都存在于仓库中，并非 worktree 私有。
  - 03-16 起项目规则已经改成“NPC 默认不再使用独立 worktree，默认从根仓库 `main` 进入，再切 `codex/...` 分支实现”，但 Git 现场仍保留旧 NPC worktree 残留。
- 当前恢复点：NPC 功能本身的代码与资产恢复结论不变，但新增一个明确的治理问题：NPC 二阶段成果当前挂在 farm 分支上，需要治理线程决定后续如何回收或重排。

- 当前主线目标：按治理裁定把 NPC 后续开发基线收敛到 `codex/npc-roam-phase2-001 @ f6b4db2f`，不再把共享根目录的 farm 事故现场当作 NPC 可写现场。
- 本轮阻塞：需要确认 `f6b4db2f` 是否已经足够自洽，以及 farm 侧混入的最小剔除范围到底有多大。
- 本轮完成：
  - 只读核对 `f6b4db2f` 的树内容，确认 NPC 核心资产、Prefab、PNG meta、运行时代码与生成器都已经在这条线内齐备。
  - 只读对比 `f6b4db2f` 与 farm 上 `07ffe199` / `18f3a9e1`，确认 NPC 范围不缺核心文件；差异主要集中在 `Assets/Editor/NPCPrefabGeneratorTool.cs` 和 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
  - 继续对比 `8aed637f -> f6b4db2f`，确认真正不该继续留在 NPC 线中的 farm 侧拖带改动只有 `Assets/YYY_Scripts/Farm/FarmToolPreview.cs`。
- 已证实事实：
  - `f6b4db2f` 已包含 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/*.png.meta`、`Assets/YYY_Scripts/Anim/NPC/`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/Editor/NPCPrefabGeneratorTool.cs`。
  - 在 `f6b4db2f` 内，`GameInputManager.cs` 只调用无参 `ClearAllQueuePreviews()`；没有 NPC 代码依赖 `FarmToolPreview` 里新增的 bool 重载。
  - `f6b4db2f` 版 `NPCPrefabGeneratorTool.cs` 足以作为救援基线版本保留，但这表示“先保证救援线自洽”，不等于“最终工具 UX 已经做完”。
- 当前恢复点：NPC 现在的最小干净收口方案已经清楚了，后续应在独立 NPC 可写现场里只做一刀：回退 `FarmToolPreview.cs` 到 `8aed637f` 版本，再做最小验证。

## 2026-03-20｜NPC phase2 主场景断链补救
- 当前主线目标：把 `Primary.unity` 里已经引用到的 NPC phase2 运行时资产真正补回 `main`，彻底消除 001/002/003 再次红掉的问题。
- 本轮阻塞：用户反馈 `NPC` batch03 一度看起来正常，但 `farm` 收口回到 `main` 后，`Primary` 场景里的 `001/002/003` 又出现 `Missing Prefab / Missing Sprite / Missing RuntimeAnimatorController`。
- 本轮完成：重新按 Git 与场景 YAML 取证，确认不是 `farm` 覆盖了 NPC，而是 `main` 的 [Primary.unity](D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity) 早已引用 `codex/npc-roam-phase2-003` 才有的 prefab / profile / meta / runtime 脚本；随后已从 `codex/npc-roam-phase2-003` 精确恢复 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/001~003.png.meta`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` 与 `Assets/Editor/NPCPrefabGeneratorTool.cs` 到当前 `main` 工作树。
- 已验证事实：`Primary.unity` 中 `001/002/003` 的 prefab GUID 现在都能反查到恢复后的 `Assets/222_Prefabs/NPC/*.prefab.meta`；prefab 内的 Sprite / Animator / roamProfile / AutoRoam / BubblePresenter 引用也都能在恢复后的资源上闭合；`Unity` Editor 日志显示本轮发生了脚本重编译与 Asset Refresh，且尾部未再出现新的 NPC missing 报错；MCP 桥本轮不可用，所以没有做 Inspector 侧二次读取。
- 当前恢复点：下一步应把这批 NPC 运行时恢复文件与事故记忆一起最小提交到 `main`，并在治理层补一条验收口径：不能只验 `carrier` 在位，还必须验 `main` 是否已经拥有生产场景实际依赖的运行时资产。

## 2026-04-03｜NPC 对话头像批量导出（外部产物）

- 当前主线目标：
  - 基于 `Assets/Sprites/NPC/*.png` 现有的 `3x4` 角色图集，为 RPG 对话框准备一批可直接消费的人物头像。
- 本轮子任务：
  - 不改 Sunset 内的 NPC 源图，不接 DialogueUI，只把每个角色各自导出 `10` 张透明底头像到外部目录：
    - `D:\UUUnity\NPC`
- 本轮实际完成：
  - 核实当前源目录共有 `11` 张角色图：
    - `001`
    - `002`
    - `003`
    - `101`
    - `102`
    - `103`
    - `104`
    - `201`
    - `202`
    - `203`
    - `301`
  - 核实每张源图均为：
    - `96x128`
    - `3x4`
    - 单帧 `32x32`
  - 已按统一口径为每个角色导出 `10` 张头像：
    - 正面 `3` 张
    - 左侧 `3` 张
    - 右侧 `3` 张
    - 正面近景 `1` 张
  - 统一输出到：
    - `D:\UUUnity\NPC\<角色名>\*.png`
  - 当前总产出为：
    - `11` 个角色文件夹
    - `110` 张头像 PNG
- 当前导出规则：
  - 不使用背面那一排做头像
  - 头像统一为透明底
  - 从原始帧中自动裁出“头 + 上半身”范围后，按像素风 nearest-neighbor 放大到统一尺寸
  - 文件命名统一为：
    - `01_front_left`
    - `02_front_idle`
    - `03_front_right`
    - `04_left_step_a`
    - `05_left_idle`
    - `06_left_step_b`
    - `07_right_step_a`
    - `08_right_idle`
    - `09_right_step_b`
    - `10_front_closeup`
- 验证结果：
  - 已抽查：
    - `001/10_front_closeup.png`
    - `101/10_front_closeup.png`
    的裁框与可读性
  - 已核对每个角色文件夹均为 `10` 张 PNG
  - 本轮没有修改：
    - `Assets/Sprites/NPC/*.png`
    - `Assets/Sprites/NPC/*.png.meta`
    - 任何 Prefab / Animation / Scene / DialogueUI 资源
- 当前恢复点：
  - 这批头像现在已经可供外部对话框系统直接挑选和接入
  - 如果后续继续，最合理的下一步是：
    - 决定对话系统最终采用哪一张作为每个角色的默认头像
    - 再决定是否需要第二轮“表情版 / 情绪版”头像，而不是重做这一轮基础导出

## 2026-04-03｜NPC 对话头像严格重做（严格右向半身胸像版）

- 当前主线目标：
  - 按用户的严格修正版要求，彻底推翻上一轮错误产物，重新为 `Assets/Sprites/NPC/*.png` 的每个角色生成只适用于 RPG 对话框的右向半身胸像头像。
- 本轮子任务：
  - 只保留：
    - 侧向右
    - 平和基础状态
    - 半身 / 胸像构图
    - 透明底
  - 严禁再次出现：
    - 全身
    - 多朝向
    - 行走帧感过重
    - 正脸 / 朝左 / 背面
- 本轮实际完成：
  - 再次核实当前源目录仍为 `11` 张角色 PNG：
    - `001`
    - `002`
    - `003`
    - `101`
    - `102`
    - `103`
    - `104`
    - `201`
    - `202`
    - `203`
    - `301`
  - 再次核实所有源图均为：
    - `96x128`
    - `3x4`
    - 单帧 `32x32`
  - 本轮严格只取每张源图中“朝向右”的那一排作为头像源，不再混用正面 / 左向 / 背面行。
  - 已按新规则为每个角色重新生成：
    - `10` 张右向半身胸像头像
  - 最终落盘目录：
    - `D:\UUUnity\NPC\<角色名>\01.png ~ 10.png`
  - 当前总产出为：
    - `11` 个角色文件夹
    - `110` 张 PNG
- 本轮导出规则：
  - 统一只展示头部 + 肩部 / 胸部以上区域
  - 统一透明背景
  - 统一 `128x128`
  - 统一按像素风 nearest-neighbor 放大
  - `10` 张版本都保持：
    - 同角色
    - 同风格
    - 无情绪
    - 朝向右
  - 变化只允许体现在：
    - 轻微构图差异
    - 轻微远近差异
    - 同一右向帧组内的细微可选版本
- 当前验证：
  - 已核对每个角色目录都有且只有 `10` 张 PNG
  - 已核对当前全部产物尺寸统一为：
    - `128x128`
  - 已抽查：
    - `D:\UUUnity\NPC\001\01.png`
    - `D:\UUUnity\NPC\101\01.png`
    - `D:\UUUnity\NPC\301\01.png`
    当前构图均为右向胸像，不再是上一轮那种全身 / 多角度错误产物
- 本轮没有做：
  - 没改 `Assets/Sprites/NPC/*.png`
  - 没改 `Assets/Sprites/NPC/*.png.meta`
  - 没接 DialogueUI / 对话框逻辑
  - 没进 Unity / Play Mode / MCP
- 当前恢复点：
  - 这批头像现在已经满足“RPG 对话框旁的人物右向胸像候选”这一基础资产要求
  - 如果后续继续，最合理的下一步是：
    - 给每个角色从 `01~10` 中选默认头像
    - 或继续做第二轮“情绪版 / 表情版”头像

## 2026-04-03｜纠偏：上一轮“严格右向胸像版”仍然失败，当前 `D:\UUUnity\NPC` 产物不得继续使用

- 当前主线目标：
  - 诚实纠正本线程刚刚那轮 NPC 对话头像导出的失败结论，避免后续把错误产物继续当作可用资产消费。
- 本轮新增稳定结论：
  - 我上一轮虽然把产物统一成了“朝右”，但本质上仍然只是：
    - 把原始右向行走帧裁切放大
  - 它并没有真正变成用户要的：
    - RPG 对话框半身 / 胸像头像
  - 因此当前 `D:\UUUnity\NPC` 下这批图应视为：
    - 失败产物
    - 暂不可用
    - 不得作为默认对话头像继续接入
- 这次失败的核心原因不是“审美不够好”，而是理解层就错了：
  - 我把“半身人像 / 胸像头像”错误执行成了“从行走图右向帧里裁一块上半部分再放大”
  - 这会导致：
    - 仍然保留明显的行走帧语义
    - 角色头部占比太小
    - 肩胸构图不成立
    - 10 张之间只是轻微裁切差，不是真正可选的人像版本
    - 结果放进对话框后仍然像“被裁过的角色行走小人”，而不是“角色对话头像”
- 当前恢复点：
  - 后续如果重做，正确方向不能再是“继续裁右向行走帧”
  - 正确理解应改为：
    - 以原始行走图的人设、配色、服饰、特征为参考
    - 重构真正的右向胸像 / 半身人像
    - 让头部、脸部、肩胸在对话框尺度上真正可读

## 2026-03-22

- 当前主线目标：在 `main-only` 新口径下，把 NPC 返工直接收口到 `main`，不再回到 `codex/npc-roam-phase2-003` 开发。
- 本轮锁定范围：
  - 气泡真正抬离头顶，不能压脸，留白更稳定。
  - NPC 改成下半身实体碰撞，不再是整身 trigger，玩家与 NPC、NPC 与 NPC 不再继续互相穿透。
- 本轮已落到 `main` 的业务文件：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - `Assets/222_Prefabs/NPC/001.prefab`
  - `Assets/222_Prefabs/NPC/002.prefab`
  - `Assets/222_Prefabs/NPC/003.prefab`
- 本轮关键结论：
  - `NPCBubblePresenter` 已提升到 `styleVersion = 5`，包含基于渲染器顶部计算的稳定抬高逻辑与新版样式参数。
  - 三个样本 prefab 当前均为 `BoxCollider2D + Rigidbody2D`，`m_IsTrigger: 0`，并已绑定 `NPCMotionController.rb` 与 `NPCAutoRoamController.rb`。
  - `git diff --check` 已通过上述 6 个 NPC 文件。
- 本轮阻塞：
  - `main` 中同时存在大量与 NPC 无关的其他线程 dirty，当前还不能把“是否立即 Git 收口”写成已完成。
  - Unity / MCP 本轮连接失败，不能替代主项目手测。
- 当前恢复点：NPC 的 main-only 返工已经真实落到 `main`；下一步应先做主项目手测验收这两项效果，再决定是否对白名单 NPC 文件做 Git 收口。

## 2026-03-22 导航能力边界只读核查

- 当前主线目标：为 NPC 下一轮返工提供准确的问题定义，不扩写实现。
- 本轮结论：
  - NPC 当前“有导航调用，但没有动态避让系统”。
  - 玩家自动导航当前“有一点局部绕障能力，但没有动态礼让与双向协同能力”。
- 已证实依据：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 会调用 `NavGrid2D.TryFindPath()`，卡住后只会 `TryRebuildPath()` 或重新抽点，不会做局部 steering。
  - `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs` 是静态网格 A*，网格刷新不跟随 NPC/玩家实时移动；`Primary.unity` 当前 `obstacleTags` 也不包含 NPC。
  - `Assets/YYY_Scripts/Service/Player/PlayerAutoNavigator.cs` 存在 `AdjustDirectionByColliders()`，所以玩家导航能做有限的局部偏转，但不是动态 reroute，也不保证窄路会车成功。
  - 玩家与 NPC 当前质量同为 `1`，而玩家移动速度高于 NPC，因此“玩家容易顶开 NPC”是现配置下的真实体感问题，不是错觉。
- 用户新增需求口径：
  - 气泡箭头要更明显朝下指向 NPC，并带轻微悬浮感。
  - 气泡和文字都要适度放大，达到清晰可读。
  - NPC 不应轻易被玩家顶开。
  - 后续需要更完整的动态避让、互相规避与路口礼让能力。
- 需要申请加强的缺口：
  - NPC 局部避障 / 分离 steering
  - 动态障碍占用与实时重规划
  - agent 优先级 / 路口礼让
  - 玩家/NPC 物理交互权重与抗推动策略
  - NPC 导航专项验收标准
- 当前恢复点：问题定义已经厘清，可以据此申请“表现层小修 + 动态导航增强”两类后续修正。

## 2026-03-22 职责边界更正与导航交接口径

- 用户已明确纠正：这一轮我要输出的是“NPC线程自己的认领范围 + 给导航线程的详细需求”，不是泛泛谈整套物理系统。
- NPC线程认领范围：
  - 气泡视觉代码与 prefab 参数
  - NPC 自身碰撞体与刚体默认值
  - NPC 如何消费导航结果移动与做卡住恢复
- 导航线程认领范围：
  - NavGrid2D 动态障碍与动态重规划
  - 玩家自动导航绕移动 NPC
  - NPC/NPC 局部避障、会车、路口礼让
  - agent 半径/占位策略分型
- 用户重新明确的 NPC 视觉需求：
  - 尾巴必须是倒三角朝下指向 NPC
  - 尾巴本身需要轻微上下跳动
  - 气泡与字体都要适度放大到清晰可读
- 当前恢复点：后续对外沟通时必须先分清“NPC代码问题”和“导航核心问题”，不能再把两个线程的职责混着汇报。

## 2026-03-22 用户强纠正后的职责边界定稿

- 当前主线目标：修正 NPC 线程的对外口径，确保 NPC 只认 NPC 自己的代码，导航缺口单独移交导航线程。
- 本轮子任务：只读核查 `main @ c6af2657` 中 NPC / 导航相关脚本和样本 prefab，输出最终职责分离结论。
- 本轮定稿结论：
  - 上一轮边界混乱是 NPC 线程自己的问题，不能再把“NPC 表现返工”和“导航核心增强”打包成一锅。
  - `NPCBubblePresenter.cs` 当前尾巴视觉仍不对，用户验收口径明确要求它改成“倒三角朝下 + 尾巴单独轻微跳动 + 气泡与字适度放大”。
  - `NPCAutoRoamController.cs` 当前职责只应包括“NPC 如何使用现有导航结果去移动与恢复”，不能越界去定义导航系统的动态避让框架。
  - `NavGrid2D.cs` / `PlayerAutoNavigator.cs` 以及动态障碍、礼让、让行、会车等能力缺口必须由导航线程认领。
- 当前恢复点：后续如果继续推进，NPC 线程先做 NPC 自己的 UI / collider / movement-consumer 修正；导航线程单独接动态避让需求。

## 2026-03-22 导航 / NPC 双Prompt定稿

- 当前主线目标：把 NPC 与导航线程的后续推进方式产品化成两份标准 prompt。
- 本轮子任务：输出一份给导航线程的专家级需求 prompt，以及一份给 NPC 线程自己的执行 prompt。
- 本轮结论：
  - 导航 prompt 负责把“动态障碍、玩家绕移动 NPC、NPC/NPC 互相规避、路口礼让、agent 分型”讲透并锁定验收。
  - NPC prompt 负责把“倒三角尾巴、尾巴单独跳动、气泡/字体放大、下半身实体碰撞、后续接导航成果适配”讲透并锁定验收。
- 当前恢复点：后续对外协作可以直接复用这两份 prompt，不再临时口述需求。

## 2026-03-23 NPC第二刀 main-only 精修

- 当前主线目标：继续在 `main` 上把 NPC 自己的表现层与碰撞体感修到更接近用户验收口径。
- 本轮子任务：只改 NPC 自己的代码范围，不碰 `NavGrid2D`、`PlayerAutoNavigator` 等导航核心文件。
- 本轮完成：
  - 气泡第二刀：倒三角尾巴方向修正为明确朝下，尾巴单独轻微跳动，整体气泡尺寸、字体大小、头顶留白继续上调。
  - 体感第二刀：NPC 默认刚体质量与阻尼提升，目标是让玩家更难把 NPC 当推土机顶着走，但不把 NPC 改成完全僵死的柱子。
  - 样本同步：`001/002/003` 与生成器默认值已同步到同一套参数。
- 本轮验证：
  - 静态 diff 检查通过。
  - MCP live 编译未取得证据，原因是 `recompile_scripts` 当前连接失败；因此这轮还欠一次 live 目测验收。
- 当前恢复点：下一步可以直接拿 `Primary.unity` 做一次 live 目测，重点看尾巴观感与玩家推挤体感；若通过即可按白名单继续收口。

## 2026-03-23 NPC第三刀：自适应气泡与003压测器

- 当前主线目标：让 NPC 气泡从“能用”继续收口到“长短句都好看、003 可以稳定当测试样本”。
- 本轮子任务：追加一个专门的 003 持续说话压测器，并继续细修气泡下沉和自适应尺寸。
- 本轮完成：
  - 气泡布局：加入 `minAdaptiveTextWidth` 和 `textSafePadding`，长句不再只横向变宽，气泡会更自然地兼顾长高和变宽。
  - 气泡位置：把 `bubbleLocalOffset / minBubbleHeight / bubbleGapAboveRenderer` 再往下压一档，专门修“箭头太高”的反馈。
  - 003 压测：新增 `NPCBubbleStressTalker`，让 `003` 持续随机说短中长不同长度的话，并在测试模式下关闭自身漫游，避免状态机抢气泡。
- 本轮 live 证据：
  - `unityMCP` 已正常工作。
  - `003` 在 `Primary` 里 live 读回为：`NPCBubbleStressTalker.ShowCount = 2`、`LastShowSucceeded = true`、`NPCBubblePresenter.IsBubbleVisible = true`、`CurrentBubbleText` 为长句。
- 当前恢复点：用户现在可以直接在主项目里拿 `003` 做气泡自适应验收，不再需要手动反复触发对话。

## 2026-03-23 NPC第四刀：气泡内边距与场景化配置入口

- 当前主线目标：把 NPC 的表现层细节收紧到接近最终验收，并把后续场景配置入口整理顺。
- 本轮子任务：只改 NPC 自己的代码与编辑器入口，不碰 `Primary.unity` 和导航核心。
- 本轮完成：
  - 气泡内边距修正：文字框四周进一步向内收，解决“文字贴边框”的观感问题。
  - 003 测试模式正规化：新增 `NPCBubbleStressTalkerEditor`，并把 `003.prefab` 的测试引用显式序列化，避免测试逻辑继续像一次性临时脚本。
  - 漫游配置入口增强：`NPCAutoRoamControllerEditor` 现在可直接创建/选择/清空 Home Anchor、复制 Profile、本地应用 Profile，并显示当前活动范围与路径点状态。
- 本轮验证：
  - 静态 diff 检查通过。
  - 当前 `unityMCP` 连接在收尾阶段出现 `旧 MCP 端口口径（已失效）` 握手失败，因此 live 终验尚未闭环；但这轮前段已经拿到过 `003` 压测器在场景中工作的 live 证据。
- 当前恢复点：这轮之后，NPC 线已经正式进入“场景化与集成阶段”，后续重点就是配置、验收和等待导航交付后的接入。

## 2026-03-23 NPC第五刀：气泡内边距与配置入口完成首版

- 当前主线目标：把 NPC 这条线从“继续修气泡”推进到“具备后续场景化配置能力”。
- 本轮子任务：继续收口气泡内边距，并搭建 `003` 测试入口与 `NPCAutoRoamController` 配置入口。
- 本轮完成：
  - 气泡内边距已再次向内收，重点修正你指出的“文字太贴边框”。
  - `003` 的测试模式已从临时压测状态转为正式测试组件 + 正式编辑器入口。
  - 漫游控制器现在有了更像工具而不是裸 Inspector 的配置面板。
- 本轮验证：
  - 静态 diff 检查通过。
  - 当前 `unityMCP` transport 在收尾阶段失联，live 终验待补。
- 当前恢复点：接下来可以直接开始做 NPC 活动区域、路线和 Profile 的场景化配置，而不需要再回头搭测试基础设施。

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

## 2026-03-23 NPC场景化与集成首版入口落地

- 当前主线目标：把我之前一直口头承认“还没做完”的 NPC 场景化与集成工作推进到真正可复用的首版入口，而不是继续停留在零散功能状态。
- 本轮子任务：先不去硬写 `Primary.unity` 这类高风险 Scene，而是把 NPC 自己能独立完成的 profile 分层、批量集成工具和收口文档先落稳。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_BubbleReviewProfile.asset`，作为 `003` 这类验证样本的默认漫游/停留配置。
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCSceneIntegrationTool.cs`，支持对选中的 Scene NPC 批量切 `Production / BubbleReview` 模式、批量补 Home Anchor、吸附 Anchor、移除或补挂 `NPCBubbleStressTalker`，并在落地后自动 `MarkSceneDirty`。
  - 将 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab` 的默认 `roamProfile` 切到 `NPC_BubbleReviewProfile.asset`，把 `003` 正式归类为长期保留的验证样本 prefab。
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-23_NPC场景化与集成首版收口.md`，把“正式 NPC / 验证样本 NPC / Scene 仍待人工落点的部分”明确写清。
- 本轮已验证事实：
  - `Primary.unity` 里 `001 / 002 / 003` 当前只有名称、位置、缩放等 prefab 实例覆盖，没有读到 `homeAnchor` 场景级覆盖证据，这说明“场景化与集成还没真正落进 Scene”是现场事实。
  - `git diff --check` 已通过本轮新增/修改的 NPC 文件。
- 当前恢复点：NPC 这条线现在已经不再只是“会动会说”的功能集合，而是有了正式/验证分层与批量场景接入工具；下一步若继续，就该在 `Primary.unity` 中用这套工具把 Home Anchor 和实际活动范围真正摆进场景。

## 2026-03-23 NPC工具链继续收口：生成器支持正式/验证样本分流

- 当前主线目标：在不碰 `Primary.unity` 这类热场景的前提下，继续把 NPC 自己的生成器和集成工具做顺，减少后续落场景时的手修量。
- 本轮子任务：继续收口 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`，让它直接理解“正式 NPC”和“验证样本 NPC”的差异，而不是生成后再人工补。
- 本轮完成：
  - 新增生成器内的 `GeneratedNpcRole` 分流，支持 `Production / BubbleReview` 两类角色。
  - 生成器 UI 新增“角色预设”区，可填写验证样本名称列表，默认识别 `003` 为 BubbleReview 样本。
  - 正式 NPC 现在会自动绑定 `NPC_DefaultRoamProfile.asset`；验证样本会自动绑定 `NPC_BubbleReviewProfile.asset`。
  - 验证样本可自动补挂 `NPCBubbleStressTalker`，把 `003` 这类 review 样本的默认真相收进生成器，而不是靠后补。
  - 生成结果摘要会直接统计“正式 / 验证样本”数量，方便回看本次批量生成有没有串线。
- 已验证事实：
  - `git diff --check -- Assets/Editor/NPCPrefabGeneratorTool.cs` 已通过。
  - 本轮没有改 `Primary.unity`、没有碰 4 个 TMP 字体资源、没有写 Prefab / Scene 热区资源。
- 当前恢复点：NPC 当前还能继续安全推进的遗留项，已经从“再讲一遍场景化没做完”收缩成“继续做工具链减手修”；等热场景安全后，再用这些工具去做真实的 Home Anchor / 活动范围场景落点。

## 2026-03-23 导航回执最终版已更新

- 当前主线目标：把 NPC 线程发给导航线程的正式回执更新成当前可直接转交的最终版。
- 本轮子任务：只更新导航回执文档与对应记忆，不做新的实现改动。
- 本轮完成：
  - 重写 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\给导航线程的正式回执_2026-03-23.md`。
  - 回执已补进最新事实：`main @ 3d5978b0`、`003` 已作为验证样本正式化、场景化入口已在位、生成器已支持正式/验证样本自动分流。
  - 回执中也重新钉死了导航线程当前真正应认领的问题与 NPC 线程当前不会乱动的稳定基线。
- 已验证事实：
  - 本轮没有新增业务代码修改，只更新了导航交接文档。
- 当前恢复点：如果用户现在就要把 NPC 需求正式发给导航线程，这份回执已经可以直接使用；NPC 自己接下来则主要等待导航交付，再做接入与场景化终验。

## 2026-03-25 NPC只读扫描：当前世界图景

- 当前主线目标：只做一次只读扫描，判断 NPC 线程现在有没有新的切入点，不开始实现。
- 本轮子任务：核 shared root、MCP 占用、NPC/导航相关 dirty 面和最近提交走势，只讨论局势，不动业务代码。
- 本轮已证实：
  - shared root 当前仍在 `main @ 84fc3818f8049d3cd6a5697f87f288429b2b361c`，远端同步一致。
  - 但 live 工作树并不干净，且已经出现大面积业务 dirty：`Primary.unity`、`GameInputManager.cs`、`NPCAutoRoamController.cs`、`NPCMotionController.cs`、`NavGrid2D.cs`、`NavigationPathExecutor2D.cs`、`PlayerAutoNavigator.cs`、`PlayerMovement.cs` 等都在改。
  - 仅就 NPC/导航相关核心面，`git diff --stat` 已显示约 1994 行新增、423 行删除，属于明显的进行中集成态，不适合 NPC 线程此刻再下场补刀。
  - `shared-root-branch-occupancy.md` 仍写着 `neutral-main-ready`，但这只说明分支口径是 `main`；它并不能反映当前真实 working tree 已经非常繁忙。
  - `mcp-single-instance-occupancy.md` 当前 `current_claim = none`，说明没有显式 MCP 占用声明；但因为热区文件正在脏改，仍不应把“没人 claim”误解成“现在适合随便 live 写”。
- 当前判断：
  - NPC 线程现在当然还有用武之地，但主要不是“立刻继续写”，而是准备做后续接入、验收、边界守门和问题归因。
  - 当前最像真实主战场的是“导航 / 玩家移动 / Primary 场景 / 输入链”这一大块集成面；NPC 此刻贸然下场，很容易撞上正在进行中的联调现场。
- 当前恢复点：NPC 现在最合理的姿势是保持只读观察，等这波导航/场景集成收口后，再接手 NPC 侧适配、场景化落点和联调整体验收。

## 2026-03-25 导航自检后的 NPC 认知同步

- 当前主线目标：基于导航线程最新自检，重新校准 NPC 线程的后续切入点，但本轮只做讨论，不开始实现。
- 本轮子任务：判断导航当前真正没做完的是什么，以及 NPC 线程除了“等导航”之外还能提前准备什么。
- 本轮已确认：
  - 导航线程当前最大的问题不是方向错了，而是共享底座还停在过渡态，`S2 / S3 / S5 / S6` 都没有真正闭环。
  - 这和 NPC 线程之前的判断是对得上的：我现在不该去碰 `NPCAutoRoamController`、`NPCMotionController` 的核心运动语义，更不该抢写导航核心。
  - 但这不等于 NPC 线程已经完全没事做；我还能提前做的主要是“设计、契约、验收”和“场景化落点方案”，而不是代码实现。
- 当前判断：
  - 如果让我给自己布置后续任务，最合理的不是再写一刀功能，而是先准备三类设计内容：
    1. `NPC-导航接入契约与联调验收清单`
    2. `NPC 场景化真实落点设计（Home Anchor / 活动范围 / 正式NPC与验证样本摆位规则）`
    3. `NPC 受击 / 工具命中 / 反应系统兼容设计草案`
  - 这三类工作都能服务后续真实开发，而且不会和当前导航主战场正面冲突。
- 当前恢复点：NPC 线程现在的最佳策略不是“直接开写”，而是等你拍板后，优先把这些设计型前置产物先做出来，为导航交付后的联调和下一阶段 NPC 扩展铺路。

## 2026-03-25 NPC治理裁定更新：先做接入契约，不提前碰导航核心实现

- 当前主线目标：
  - 在导航线已被重新定性为“结构 checkpoint 成立，但真实动态导航落地失败”后，重新给 NPC 线程安排一条不撞主战场的新切入点。
- 本轮完成：
  - 已新增 NPC 新开工 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\2026-03-25_NPC导航接入契约与联调验收规范起稿.md`
- 当前关键判断：
  1. NPC 这轮不该碰实现主战场；
  2. 最值钱的当前任务是先把：
     - `NPC-导航接入契约`
     - `联调验收清单`
     - `进入 NPC 集成的红线`
     收成一份正式文档；
  3. `场景化真实落点设计` 与 `受击/命中/反应系统兼容设计` 继续延后，不与本轮混做。
- 当前恢复点：
  - NPC 工作区当前已从“只读观察阶段”推进到“docs-only 设计开工阶段”，但仍保持不碰导航核心实现的边界。

## 2026-03-25 NPC导航接入契约收口为唯一维护文件

- 当前主线目标：
  - 把 NPC 给导航线程的协作文档从“历史回执 + 新 prompt 起稿”收敛成一个真正可长期维护的唯一文件。
- 本轮子任务：
  - 在不碰导航代码、不碰 NPC 业务实现、不碰 Scene / Prefab 的前提下，完成 `NPC-导航接入契约与联调验收规范` 的正式定稿。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md`
  - 该文件已明确 supersede 历史 `给导航线程的正式回执_2026-03-23.md`
  - 新文件已统一吸收：
    - NPC 当前冻结语义
    - 导航 -> NPC 最小接入契约
    - 联调验收清单
    - 进入 NPC 集成的红线
    - 本轮明确延后的设计项
- 已证实事实：
  - 当前 live 现场仍是 `D:\Unity\Unity_learning\Sunset @ main @ 84fc3818f8049d3cd6a5697f87f288429b2b361c`
  - 当前 shared root 正处于高活跃脏现场，因此本轮选择 docs-only 是正确动作
  - 当前旧回执文件已不在 live 文件树中，但可从 Git 历史 `e339ccd65d48c56e35e7984e8b524be8124d8d45` 还原正文作为合并依据
- 当前恢复点：
  - 后续导航线程若要回包，应直接以 `NPC-导航接入契约与联调验收规范.md` 为唯一 NPC 协作输入
  - `NPC 场景化真实落点设计` 与 `NPC 受击 / 工具命中 / 反应系统兼容设计` 继续延后到下一阶段

## 2026-03-25 NPC 2.0.0 工作区启用

- 当前主线目标：
  - 把用户本轮新增的“大需求层”从 `1.0.0初步规划` 提升到新的 `2.0.0进一步落地` 工作区，避免旧工作区继续承载越来越大的产品化设计。
- 本轮子任务：
  - 建立 2.0.0 工作区入口文档，并把导航契约迁入 2.0.0 作为唯一维护版本。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC-导航接入契约与联调验收规范.md`
  - 将 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md` 收口为迁移说明
- 2.0.0 当前明确承接的方向：
  - 文档治理收口
  - NPC 场景化真实落点与角色化设计
  - 轻交互 / 好感度 / 双气泡体系
  - 受击 / 工具命中 / 反应兼容框架
- 当前恢复点：
  - 后续 NPC 的大设计型工作优先进入 `2.0.0进一步落地`
  - `1.0.0初步规划` 继续保留为历史规划与早期收口现场，但不再承接最新主设计正文

## 2026-03-25 NPC 2.0.0 第一刀已开始直接落地

- 当前主线目标：
  - 在不抢导航主战场、不碰热 Scene 的前提下，把用户已经批准的 2.0.0 设计先落一刀真正可运行的 NPC 自身内容。
- 本轮子任务：
  - 完成 2.0.0 文档族收口。
  - 让 `001 / 002 / 003` 从“共享测试句库”进入“各自有人设的 profile 基线”。
- 本轮完成：
  - `2.0.0进一步落地` 当前已经补齐 4 份主文件：
    - `NPC-导航接入契约与联调验收规范.md`
    - `NPC场景化真实落点与角色日常设计.md`
    - `NPC交互反应与关系成长设计.md`
    - `NPC系统实施主表.md`
  - 新增 `NPC_001_VillageChiefRoamProfile.asset`
  - 新增 `NPC_002_VillageDaughterRoamProfile.asset`
  - 新增 `NPC_003_ResearchReviewProfile.asset`
  - 修改 `NPCPrefabGeneratorTool.cs`，让生成器按 `001 / 002 / 003` 自动绑定角色化 profile，并给 `003` 的 review 样本自动灌入研究型测试文案
  - 修改 `001.prefab`、`002.prefab`、`003.prefab`，把默认 `roamProfile` 切到新的角色化资产
- 本轮明确没做：
  - 没有碰 `Primary.unity`
  - 没有碰 `NPCAutoRoamController.cs`
  - 没有碰 `NPCMotionController.cs`
  - 没有碰导航核心或玩家导航核心
  - 没有做 Unity / MCP live 写
- 当前恢复点：
  - NPC 现在已经开始从“测试型系统”转向“角色化系统”。
  - 后续若继续推进，优先继续在 NPC 自己的 profile / 交互 / 场景化入口层推进，不越界去抢导航运动语义。

## 2026-03-25 续｜场景热区等待期间追加一条非热区代码切片

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 2adbc011`
- 当前主线目标：
  - 在用户暂时占用 Unity 场景、NPC 线无法继续做 live 表现验收时，不停工，转而推进一条不撞场景与导航热区的独立代码闭环。
- 本轮子任务：
  - 落地玩家工具失败反馈气泡与水壶运行时状态链，顺手为后续 NPC / 玩家双气泡与关系反馈体验打基础。
- 本轮完成：
  - 新增玩家侧气泡与失败反馈组件：
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
    - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
  - 把工具提交链升级为 `ToolUseCommitResult`，并把水壶运行时口径切到 `watering_current / watering_max`
  - 让背包 / 工具栏 / tooltip 正确区分“耐久工具”和“水壶”
  - 新增 `Assets/YYY_Tests/Editor/ToolRuntimeFeedbackTests.cs`
- 本轮关键结论：
  - 用户暂停释放场景时，NPC 线程仍然有“非热区、可直推 main”的有效用武之地，不必空转等导航。
  - 这条切片虽不属于 NPC 本体漫游与导航，但它确实服务后续 NPC / 玩家交互表达系统，因此被记入 NPC 主线的并行推进记录。
- 本轮验证：
  - `CodexCodeGuard` 对完整 owned 白名单通过，覆盖 UTF-8、diff 与程序集级编译检查。
  - 没有使用 Unity / MCP，也没有碰 `Primary.unity`、`GameInputManager.cs`、导航核心或玩家导航核心。
- 当前恢复点：
  - 场景一旦释放，我仍优先回到 NPC 自身的 live 表现与集成验收。
  - 在场景未释放前，仍可继续寻找同等级别的非热区独立切片推进。

## 2026-03-25 续｜003 测试模式污染正式 NPC 的问题已收口

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ bd5e588d`
- 当前主线目标：
  - MCP 放开后，回到 NPC 自身 live 验证，优先收掉“003 还是默认测试 NPC”这个遗留问题。
- 本轮关键事实：
  - `Primary` 中 `001 / 002 / 003` 的 profile、碰撞链都在位。
  - 但短窗口 Play 取证时，只有 `001 / 002` 进入漫游；`003` 仍 `Inactive`。
  - 根因不是导航，而是 `NPCBubbleStressTalker` 仍默认：
    - `startOnEnable = true`
    - `disableRoamWhileTesting = true`
- 本轮完成：
  - 仅修改 `Assets/222_Prefabs/NPC/003.prefab`
  - 把 `NPCBubbleStressTalker.startOnEnable` 改为 `false`
- 本轮 live 验证结果：
  - 修改后再次进入短窗口 Play，`003` 已恢复 `IsRoaming = true / DebugState = ShortPause`
  - `NPCBubbleStressTalker.TestModeEnabled = false`
  - Stop 后 `Primary` 仍未脏、console 仍无新 `error / warning`
- 当前恢复点：
  - `003` 默认不再自动抢占正式 NPC 漫游。
  - 后续若要继续 NPC 场景化，最明显还没做的就是 `HomeAnchor` 真正落进 scene；这一步属于 scene 集成写入，不再是 prefab-only。

## 2026-03-25｜003 工具链正规化补齐，防止正式 NPC 被工具重新污染

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 4c62ef05`
- 当前主线目标：
  - 把前一刀 `003.prefab` 的默认修正继续向工具链层补齐，确保生成器、scene 集成工具和巡检入口不会把正式 NPC 又打回“默认测试模式”。
- 本轮完成：
  - 修改：
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs`
    - `Assets/Editor/NPCPrefabGeneratorTool.cs`
    - `Assets/Editor/NPCSceneIntegrationTool.cs`
    - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
  - 新增：
    - `Assets/YYY_Tests/Editor/NPCToolchainRegularizationTests.cs`
- 本轮关键结果：
  - stress talker 现在有显式模式接口，默认仍是手动启动。
  - prefab generator 现在把 BubbleReview 改成显式 opt-in，且生产态 `003` 明确走研究型 profile。
  - scene integration tool 在正式模式下会禁用 stress auto-start，而不是删除组件。
  - roam inspector 现在可以直接看到并切换 stress auto-start。
  - 新增纯 Editor 回归测试，把默认口径和 profile 映射钉成断言。
- 本轮验证：
  - `git diff --check` 通过。
  - `CodexCodeGuard` 对上述 5 个 C# 文件通过。
  - MCP 基线、实例和 scene 状态复核通过；本轮未进 Play，`Primary.unity` 仍 `isDirty = false`。
  - 做过一次脚本级 compile 复核后，当前我 own 的错误已清零。
  - 当前 Unity console 仍有外部 blocker：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `PageRefs` 缺失
- 当前恢复点：
  - `003` 的“默认正式 NPC，测试模式显式开启”现在已经从 prefab 修正推进到工具链语义。
  - 下一刀如果继续 NPC 自己的集成，最自然还是 `HomeAnchor` scene 落点；但这一步要正式进入 `Primary.unity` 热区。

## 2026-03-25｜最小扫尾：清掉旧起稿残留并补测文件 meta

- 当前主线目标：
  - 不把这轮新增测试带出的 `.meta` 和旧的 NPC 起稿残留继续留在 shared root 里脏着。
- 本轮完成：
  - 删除 `1.0.0初步规划/2026-03-25_NPC导航接入契约与联调验收规范起稿.md`
  - 将 `Assets/YYY_Tests/Editor/NPCToolchainRegularizationTests.cs.meta` 纳入后续收口范围
- 当前恢复点：
  - 这轮 NPC 工具链 checkpoint 还差一个极小 follow-up 提交，之后我会再判断当前是否还存在安全可推进的 NPC 自身内容。

## 2026-03-25｜NPC 本轮已推到安全边界

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 55e2bccd`
- 当前结论：
  - `003` 的正式化修正、工具链正规化、回归测试补位、尾巴清理都已收口并推上 `main`。
  - 当前没有新的非热区独立 NPC 代码切口可以继续安全推进。
- 仍未完成但属于 NPC 后续主线的内容：
  - `HomeAnchor` 的 scene 落点
  - 正式 roam 区域 / 路线 / 相遇节奏
  - 导航交付后的 NPC 联调
  - Unity Test Runner 里的 `NPCToolchainRegularizationTests` 实跑
- 当前阻塞：
  - `Primary.unity` 属于共享热 scene
  - Unity console 仍有外部 blocker：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `PageRefs` 缺失

## 2026-03-26｜Primary 场景开工条件复核

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 359331b9`
- 当前结论：
  - 现在仍不满足进入 `Primary.unity` 做 NPC scene 级写入的开工条件。
- 直接原因：
  - `Primary.unity` 没有 active lock，但 working tree 里它本身仍是 dirty
  - Unity 当前可用、MCP 当前可用、scene 内存态也不脏
  - 真正卡点是“热文件磁盘态已漂移但尚未建立独占归属”
- 伴随风险：
  - 共享字体 / 表现层资源也仍是 dirty：
    - `DialogueChinese BitmapSong SDF.asset`
    - `DialogueChinese Pixel SDF.asset`
    - `DialogueChinese SoftPixel SDF.asset`
    - `DialogueChinese V2 SDF.asset`
    - `LiberationSans SDF - Fallback.asset`
- 当前恢复点：
  - 后续如果要让我开始 scene 集成，应先把 `Primary.unity` 的 dirty 归属说清，并给这次写入建立独占窗口。

## 2026-03-26｜NPCV2 接班首轮只读复核：暂不进入 Primary

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ ee3187573b62891a5b0a8d974f43c192c4125a34`
- 当前主线目标：
  - 由 `NPCV2` 接班后，只判断 `Primary.unity` 的 scene 写窗口是否已足够安全，以决定能否把第一刀钉到 `HomeAnchor` 最小 scene 切片。
- 本轮只读结论：
  - 当前 scene 写窗口仍未成立，`NPCV2` 不能直接进入 `Primary.unity` 开工。
- 关键证据：
  - `Assets/000_Scenes/Primary.unity` 当前仍为 `M`
  - `git diff --stat -- 'Assets/000_Scenes/Primary.unity'` 仍为 `76 insertions / 4 deletions`
  - `shared-root-branch-occupancy.md` 仍是 `main + neutral`，但 `last_verified_head` 已落后于当前 `HEAD`，只能作为 shared root 入口提示，不能外推成 scene 可写
  - `mcp-single-instance-occupancy.md` 当前没有显式 claim，但默认策略仍是 `single-writer-only`
  - `mcp-hot-zones.md` 继续把 `Primary.unity` 列为热区 B / C
  - 在 `Primary.unity` 内直接搜索 `HomeAnchor` 无命中；当前 diff 片段主要是 `StoryManager`、Workbench overlay、debug flag 与位置漂移，不是 NPC 自己的 `HomeAnchor` 半成品
- 当前恢复点：
  - `NPC` 主线的下一步仍然是 `HomeAnchor` scene 落点，但恢复前提没有变化：先把 `Primary.unity` 当前 dirty 归属说清，再建立独占写窗口

## 2026-03-26｜再次只读复核：Primary 准入条件未变化

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 519d51bd20d98e662eafb94cea0c5bbbeb314cec`
- 当前只读结论：
  - 现在仍不满足进入 `Primary.unity` 做 `HomeAnchor` 最小切片的条件。
- 关键证据：
  - `Primary.unity` 仍为 `M`
  - scene diff 仍为 `76 insertions / 4 deletions`
  - 物理锁状态仍是 `unlocked`，不是显式授予给 `NPCV2`
  - `shared-root-branch-occupancy = neutral` 仍只代表 shared root 入口中性，不代表热 scene 可写
  - `Primary.unity` 内再次搜索 `HomeAnchor` 仍无命中
- 当前恢复点：
  - `NPC` / `NPCV2` 这条线当前仍停在同一个 blocker：先把 `Primary.unity` 的 mixed dirty 归属说清，再进入 `scene audit -> HomeAnchor`

## 2026-03-26｜NPCV2 共享根 cleanup：owner 报实与白名单收口

- 当前 live 基线：
  - `D:\Unity\Unity_learning\Sunset @ main @ 1452bebb1171235b454d1d4fd961639caabdc930`
- 当前主线目标：
  - 只收 NPC own 文档尾账，不重开 `Primary.unity / HomeAnchor`。
- 本轮完成：
  - 已把 NPC own dirty / untracked 与 foreign/hot dirty 明确分离。
  - 已对白名单路径执行 `git-safe-sync`，首次形成提交 `eb6284fa` 并推送 `main`。
  - `Primary.unity`、导航、农田、TMP 字体等 mixed hot-file 本轮均未触碰。
- 当前恢复点：
  - cleanup 主体已完成，但仍有一个 own 尾账 `.codex/threads/Sunset/NPCV2/memory_0.md` 未纳入；下一步只做最小 follow-up 收口，不进入业务续工。

## 2026-03-26｜口径补记：不要把 `NPCV2` 的局部尾项误当成 NPC 全量交接

- 当前新增结论：
  - `NPCV2` 最新那段“只剩这些”的回执，只是在汇报它自己当前围绕 `HomeAnchor` 运行中 Inspector 补口链的局部尾项。
  - 这不等于整条 NPC 主线只剩这些，更不等于整个项目只剩这些。
- 当前仍属于 NPC 更大层面的未完成项：
  - `HomeAnchor` 的运行态最终实证
  - 正式 roam 区域 / 路线 / 相遇节奏
  - 导航交付后的 NPC 联调
  - `NPCToolchainRegularizationTests` 的 Unity Test Runner 实跑
- 当前明确不应被 `NPCV2` 顺手吞并的项：
  - `Primary.unity` mixed cleanup
  - `DialogueChinese*` 字体
  - `NPCAutoRoamController.cs` runtime / 导航 diff
- 当前恢复点：
  - 以后 NPC 相关交接必须分层写清：
    - 总主线
    - 当前子线程局部待办
    - 明确不归本线程的范围

## 2026-03-26｜全需求回溯补记：NPC 用户需求不能再被局部回执替代

- 当前新增结论：
  - 本轮已重新从 `NPC` 主工作区、`1.0.0初步规划`、`2.0.0进一步落地`、`NPC` / `NPCV2` 线程记忆与 V2 交接文档中，完整回读用户此前在 NPC 线上提出过的需求与设计诉求。
  - 回读后可确认，NPC 主线不是单阶段需求，而是至少包含：
    1. `4 向 3 帧 PNG` 导入生成器与 prefab 流水线
    2. phase2 的自动漫游、环境气泡、碰撞体感、测试入口和工具链
    3. 2.0.0 的角色化 / 场景化 / 轻交互 / 关系成长 / 双气泡 / 命中反应设计
    4. `Primary.unity` 的 `HomeAnchor` scene 最小集成与运行中补口链
  - 因此后续任何“现在只剩这些”的回执，都必须明确说明自己说的是哪一层，不能再压扁成整条 NPC 线只剩某个局部尾项。
- 当前恢复点：
  - 这条 NPC 线已经重新有了完整的需求底账；后续汇报必须同时交代：
    - 原始业务需求
    - 当前所处阶段
    - 已完成 / 未完成 / 非本轮认领范围

## 2026-03-27｜NPC 全需求总表与时间线已在 `NPCV2` 目录落盘

- 当前新增结论：
  - 用户要求基于历史文档而不是聊天残余记忆，重新完整回顾 NPC 线的全部需求与设计诉求。
  - 本轮已将这份“全需求总表 + 时间线回溯”正式写入：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
- 文档已明确分出：
  1. 生成器与 phase2 运行态需求
  2. 气泡 / 碰撞 / 测试入口 / 工具链正规化需求
  3. 2.0.0 的角色化 / 场景化 / 双气泡 / 好感度 / 命中反应需求
  4. `HomeAnchor` scene 集成与运行中 Inspector 补口链需求
  5. 协作 / 治理要求
- 当前恢复点：
  - 以后 NPC 主线的需求回顾应以这份总表为总入口，而不是再靠零散 `memory` 尾项拼凑。

## 2026-03-27｜NPC 当前交接口径正式改成“三层汇报”

- 当前新增结论：
  - `NPCV2` 目录中的：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
    已经足以作为 NPC 主线的总入口文档。
  - 因此从现在开始，NPC 相关交接必须固定按三层汇报：
    1. NPC 总线完整需求与当前阶段
    2. 当前执行线程的局部刀口
    3. 明确不归当前线程认领的范围
- 当前总线与局部切片的重新区分：
  - NPC 总线不是只剩 `HomeAnchor` Inspector 补口链；
  - `HomeAnchor` 补口链只是 `NPCV2` 当前非常窄的一刀；
  - 场景化真实落点、角色化内容、双气泡、关系成长、命中反应、导航联调与测试实跑仍属于 NPC 更大层面的未完主线。
- 当前恢复点：
  - 以后如果再出现“尾部 memory 看起来只剩一个小点”，也必须先回到总表核对这只是局部切片，而不是整条 NPC 线已经只剩这一点。

## 2026-03-27｜NPC 总线已补出正式优先级清盘方案

- 当前新增结论：
  - 基于历史总表，`2.0.0进一步落地` 工作区已经新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-27-NPC全盘清盘清单与后续优先级方案.md`
  - 这份文档把 NPC 当前剩余工作重新拆成：
    1. 基线闭环
    2. 场景化真实落点
    3. 角色化内容
    4. 轻交互 / 双气泡 / 关系成长
    5. 受击 / 工具命中 / 反应
    6. 联调 / 测试 / 外部依赖
- 当前正式优先级已重新钉死：
  - `P0` 先闭 `HomeAnchor` 运行态基线与当前 own 收口
  - `P1` 再做真实场景化落点最小可玩切片
  - `P2` 再稳角色化内容
  - `P3` 再上轻交互与双气泡
  - `P4` 再接关系成长
  - `P5` 再接受击 / 工具命中 / 反应
  - `P6` 最后做导航联调与正式测试
- 当前恢复点：
  - 以后 NPC 主线的“下一步是什么”应先以这份优先级方案回答，再落到当前具体线程。

## 2026-03-27｜NPC 总线已补出“可直接执行”的详细任务标准

- 当前新增结论：
  - `0.0.1全面清盘` 目录下已经新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
  - 这份文档不再只是优先级说明，而是 NPC 后续真正的执行账本。
- 当前正式口径更新为：
  1. 宽口径优先级看：
     - `2026-03-27-NPC全盘清盘清单与后续优先级方案.md`
  2. 真正逐条施工与逐条验收看：
     - `2026-03-27-NPC全盘详细落地任务列表.md`
- 当前恢复点：
  - 以后如果再回答“下一刀做什么”，必须直接引用详细任务编号，而不是只说 `P0 / P1 / P2` 这种阶段名。

## 2026-03-27｜NPC 总线正式进入 `P0` 实做，但仍停在 HomeAnchor live 外部阻塞前

- 当前主线目标：
  - 按 NPC 最新详细任务账本，真正从 `P0` 开始推进，而不再只停在总览文档。
- 本轮完成：
  - `NPCV2` 已在：
    - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
    上继续只做 `HomeAnchor` Editor 诊断链补口；
  - 已补出用户复验文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P0-HomeAnchor基线复验与失败判读.md`
  - 已把详细任务列表中的 `P0` 当前状态更新为更贴近现场：
    - `T-P0-01 -> blocked-external`
    - `T-P0-03 -> done`
- 当前关键结论：
  - `Primary.unity` 内三只 NPC 的 `homeAnchor` scene override 与 `*_HomeAnchor` 节点仍在，scene 结构不是当前首要断点；
  - 当前首要阻塞已经更明确地变成：
    - `unityMCP` 基线层可达，但会话层 `no_unity_session`
  - 所以 NPC 总线虽然已经正式开始 `P0` 实做，但还不能把 `HomeAnchor` live 现场复核和正式验证说成 done。
- 当前恢复点：
  - 整条 NPC 总线下一步仍然先守 `P0`，不应跳去 `P1/P2`；
  - 一旦拿到 Unity 里 `001 / 002 / 003` 的新 Inspector 读数，就继续闭 `T-P0-01 / T-P0-02 / T-P0-05`。

## 2026-03-27｜NPC 总线已在热区未开时提前铺好 P2 内容资产层

- 当前主线目标：
  - 用户已明确允许“高速模式”下在测试或热区受阻时继续推进后续能落的切片，因此本轮在不碰 `Primary.unity` 的前提下，先把 `P1-01` 收口，并把 `P2` 的角色内容层提前落地。
- 本轮完成：
  - `P1-01`
    - 已正式收口为完成态
  - `P2`
    - 已新增独立 `NPCDialogueContentProfile`
    - 已为 `001 / 002 / 003` 分别补齐：
      - 单人环境句池
      - 玩家轻响应句池
      - 两两相遇矩阵
    - 已把 `NPCAutoRoamController` 的现有 ambient chat 接到 partner-specific 内容解析
    - 已把 content asset 映射通过 `roamProfile` 接回 prefab / 生成器 / 测试链
- 本轮验证：
  - `git diff --check` 通过
  - `Editor.log` 已出现脚本编译与程序集重载迹象
  - `unityMCP` 会话层仍是 `no_unity_session`，因此当前不把这轮内容层 claim 成 live 验收完成
- 当前恢复点：
  - NPC 总线现在不再只有 scene / HomeAnchor 这一条路可走；
  - 即便 `Primary.unity` 仍 blocked，也可以继续沿：
    - `P3-01`
    - `P2-07`
    这类非 live 切片继续推进。

## 2026-03-27｜NPC 总线已补齐 P2 正式验收包与 P3-01 双气泡正式规范

- 当前新增结论：
  - 在 `Primary.unity` 继续 blocked 的前提下，NPC 线没有停在热区前空转，而是继续收掉了两项关键非 live 文档任务：
    - `T-P2-07`
      - `2026-03-27-NPC-P2-07-角色化内容验收包.md`
    - `T-P3-01`
      - `2026-03-27-NPC-P3-01-玩家与NPC双气泡视觉规范正式稿.md`
- 当前正式口径更新为：
  - `P2`
    - 不再只是“内容资产已写完”，而是已经具备可直接执行的正式验收包
  - `P3`
    - 不再只是“以后再说的双气泡方向”，而是已经有正式视觉规范稿
- 当前边界继续保持：
  - 没有借这轮文档继续去写：
    - `Primary.unity`
    - `DialogueChinese*`
    - 玩家侧 mixed hot-file
- 当前恢复点：
  - NPC 总线下一步应先看 `T-P3-02` 是否存在安全代码切口；
  - 如果玩家侧热文件仍不适合动，就继续沿“不撞热区的独立切片”推进，而不是回到 `Primary.unity` 原地等待。

## 2026-03-27｜NPC 总线已把玩家气泡样式层推进到 `T-P3-02`

- 当前新增结论：
  - `T-P3-02` 不必等 `Primary.unity` 或输入 hot-file 解锁才开工；
  - 先只做 `PlayerThoughtBubblePresenter.cs` 的样式层，也能形成一刀独立闭环。
- 本轮完成：
  - 玩家气泡已切到正式玩家视觉语言：
    - 晨雾青绿色填充
    - 深青灰边框
    - 深墨色文字
    - 更轻一点的动效
  - 同时继续与 NPC 共用：
    - `10` 字一行
    - 边距骨架
    - 尾巴骨架
    - 显隐节奏
- 当前边界：
  - 这轮没有把轻响应接线偷扩到：
    - `GameInputManager.cs`
  - 但在准备白名单收口时，闸机指出：
    - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
    仍是同根 remaining dirty
  - 复核后确认它属于此前 NPC 线玩家反馈 / `ToolUseCommitResult` 链的 own tail，不按外线 mixed 处理
- 当前恢复点：
  - `T-P3-02` 已 done；
  - 先把 `Player` 根下这组 own tail 安全收口，再继续看 `T-P3-03` 是否能找到不撞 hot-file 的安全接线入口。

## 2026-03-28｜NPC 总线已用 player-side 服务完成 `T-P3-03`

- 当前新增结论：
  - `T-P3-03` 不必碰输入 hot-file，也不必碰 `Primary.unity`；
  - 只靠 player-side 邻近反馈服务，就能把“玩家靠近时的短反馈”先接到系统里。
- 本轮完成：
  - 新增：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
  - 修改：
    - `Assets/YYY_Scripts/Service/Player/PlayerToolFeedbackService.cs`
  - 当前实现为：
    - 扫描附近 NPC
    - 取 `roamProfile.PlayerNearbyLines`
    - 驱动对应 NPC 冒一句短反馈
- 当前恢复点：
  - `T-P3-03` 已 done；
  - 下一步继续看 `T-P3-04` 是否还能以非热区方式先完成轨道分工。

## 2026-03-27｜NPC 总线已完成 P3 分轨与 P4 关系成长最小闭环

- 当前新增结论：
  - 即便 `Primary.unity` 继续 blocked，NPC 总线也仍然可以沿“纯代码 + 内容资产 + 文档验收包”的方式继续推进；
  - 当前已把：
    - `T-P3-04`
    - `T-P3-05`
    - `T-P4-01`
    - `T-P4-02`
    - `T-P4-03`
    - `T-P4-04`
    连续收口。
- 本轮完成：
  - 正式对话轨 / 日常气泡轨已分工：
    - 对话开始时，玩家侧日常 NPC 轻反馈会被压掉
    - 对话进行中，不再继续探测和弹新的日常气泡
  - `P3` 验收包已补出：
    - `2026-03-27-NPC-P3-05-轻交互与双气泡验收包.md`
  - 关系成长最小底座已具备：
    - `陌生 / 认识 / 熟悉 / 亲近`
    - 按阶段分流的近身句
    - `PlayerPrefs` 最小持久化
    - `Tools/NPC/Relationship/*` 手动切阶段入口
  - `P4` 验收包已补出：
    - `2026-03-27-NPC-P4-04-关系成长首版验收包.md`
- 当前主线状态：
  - `P0 = done`
  - `P1-01 = done`
  - `P1-02 = blocked-hotfile`
  - `P2 = done`
  - `P3 = done`
  - `P4 = done`
- 当前恢复点：
  - 若继续高速推进，下一刀优先进入：
    - `P5`
  - 但 scene / hot-file 边界仍不变：
    - 不碰 `Primary.unity`
    - 不碰 `GameInputManager.cs`
    - 不吞 `DialogueChinese*`
    - 不把 `neutral-main-ready` 误判成 scene 可直接写。

## 2026-04-01｜NPC 总线开发完成度重新校准：当前更像“底座厚，体验薄”

- 当前主线目标：
  - 用户要求我不要再拿治理清扫和 package 收口来描述 NPC 进度，而是直接回答“实际开发做到了哪里、还差哪里、下一步该做什么”。
- 本轮完成：
  - 已把 NPC 总线的当前状态重新分成两层：
    1. `底层 / 中层`
       - 完成度较高
       - 已有生成、漫游、内容载体、玩家 / NPC 气泡底座、关系成长最小模型、非正式聊天状态机基础
    2. `玩家体验 / 场景可玩层`
       - 完成度明显不足
       - 当前仍不能宣称 NPC 线已经接近最终完成
  - 当前总线真实未收口点重新认定为：
    - `P1`
      - `Primary.unity` 中 `001 / 002 / 003` 的最小可玩 scene 集成仍未真正闭环
    - `P3`
      - 非正式聊天虽已有会话状态机，但体验层未过线
      - 尤其是跑开中断、气泡重叠、提示可见性、玩家/NPC 样式区分
    - `P5`
      - 受击 / 工具命中 / 反应系统仍未完成
    - `P6`
      - 联调、正式测试、最终用户终验仍未收
- 当前恢复点：
  - NPC 总线之后的优先级不应再简单写成“继续做 P5”；
  - 更准确的顺序应是：
    1. 先把 `P1` 的最小可玩 scene 闭环补出来
    2. 再把 `P3` 的非正式聊天体验闭环补严
    3. 然后再进入 `P5`

## 2026-04-01｜NPC 总线体验补口续记：交互提示改成“左下角为主，头顶小箭头为辅”

- 当前主线目标：
  - 把 `P3` 的 NPC 非正式聊天体验从“只有底层能力”继续推到“至少交互壳和中断链不再明显违和”。
- 本轮完成：
  - 交互提示层已继续往用户要求靠：
    - 头顶不再允许挂大 `E`
    - 头顶提示继续收窄成更小的倒三角
    - 左下角统一提示区保留为主提示壳
    - 非正式聊天进行中，头顶箭头已被压掉，避免和聊天气泡冲突
  - 聊天体验层继续补口：
    - 玩家 / NPC 打字机不再靠透明渐显开句
    - 跑开中断不再复用旧 typed interrupt 链
    - `E` 的职责进一步收紧到：
      - 跳过打字机
      - 跳过等待
      - 跳过收尾
      - 而不是驱动整段对话必须靠手动一轮轮推进
  - 当前总线阶段判断更新为：
    - `P3` 仍未验收通过
    - 但这轮已经在真正往“体验修补”推进，不再只是底层补链
- 本轮验证：
  - Unity 已 fresh compile 成功
  - 但运行态 live 复测和用户肉眼终验仍未完成
- 当前恢复点：
  - 下一次继续应优先补：
    - `002 / 003` 运行态验证
    - 跑开中断是否彻底止血
    - 左下角提示与头顶箭头是否符合最终观感

## 2026-04-01｜NPC 总线验证更新：非正式聊天四条 runtime trace 已过

- 当前主线目标：
  - 先把 `P3` 的 NPC 非正式聊天从“逻辑上能解释”推进到“运行态 trace 能自证”。
- 本轮完成：
  - `P3` 线新增一轮真正的 Unity 自测：
    - `002 closure`
    - `002 interrupt`
    - `003 closure`
    - `003 interrupt`
    - 全部通过
  - 顺手清掉了一个会阻断 Play 的 Editor compile blocker：
    - `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
  - 提示壳继续收紧：
    - 会话服务现在会直接压头顶 world hint，并主动同步左下角提示。
- 当前阶段：
  - `P3`
    - 逻辑自测层已明显前进
    - 但玩家观感层仍未终验
- 下一步恢复点：
  - 优先做人工终验：
    - 闲聊进行中的左下角提示
    - 头顶 world hint / 小箭头是否彻底不打架
    - 气泡与提示的最终观感是否还要再压

## 2026-04-02｜NPC 总线补记：跑开中断二次修正后，`002 / 003` 的闭环与中断证据已经补到 6 条

- 当前主线目标：
  - 把 `P3` 里最反复的体验失败点从“靠描述解释”推进到“真实 trace 已补齐”的状态。
- 本轮补记的稳定事实：
  - 本次真正继续动到的 own 代码集中在：
    - `PlayerNpcChatSessionService.cs`
    - `NPCBubblePresenter.cs`
    - `PlayerThoughtBubblePresenterStyleTests.cs`
  - 关键修正是：
    - 超距时不再推进整段正常聊天，只快进当前打字句。
    - NPC 气泡样式回到先前认可的暖金基线。
    - 玩家气泡样式测试护栏改成反射版并收掉编译红。
  - 运行态硬证据现在是 6 条：
    - `002 closure`
    - `003 closure`
    - `002 interrupt`
    - `003 interrupt`
    - `002 player-typing interrupt`
    - `003 player-typing interrupt`
    全部通过。
- 当前阶段判断：
  - NPC 总线现在更准确的状态是：
    - 底层和 targeted probe 已进一步站住
    - 玩家真实观感仍未完成最终放行
  - 所以这条线依旧要守住旧口径：
    - 不把 trace 全绿包装成体验全绿
- 当前恢复点：
  - 这轮补完审计层后，线程应继续 `PARKED`。
  - 下一步仍以用户终验后留下的 `NPC own` 失败项为准，不再顺手扩到 shared/UI 壳。

## 2026-04-02｜NPC 总线收尾补记：当前已重新 `PARKED`

- 当前状态：
  - `NPC` thread-state 已重新回到 `PARKED`
- 审计补记：
  - `skill-trigger-log` 已补到 `STL-20260402-036`
  - 当前唯一额外审计尾账不是 NPC 线自己新造成的，而是全局 canonical skill log 里已有的旧重号：
    - `STL-20260402-029`
- 当前恢复点：
  - NPC 总线当前应继续按“用户终验驱动的下一刀”推进。
  - 不再因为这轮补记动作把自己误判成仍在 ACTIVE 施工。

## 2026-04-02｜NPC/UI 边界与进度只读核对：理论边界基本清楚，但现场现实仍有交叉

- 当前主线目标：
  - 用户要求直接审我对 `UI` 线的认知是否清楚，重点不是再改代码，而是把：
    - `UI` 实际做了什么
    - `NPC` 与 `UI` 的边界该怎么切
    - 当前现场哪里已经切清、哪里其实还没彻底切清
    一次性说实。
- 本轮只读核对来源：
  - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\UI.json`
  - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\NPC.json`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-02-NPC给UI的左下角任务提示接管委托-01.md`
- 当前对 `UI` 实际进度的核定：
  - `UI` 已明确做过两组事：
    1. shared/UI 提示链：
       - `SpringDay1ProximityInteractionService.cs`
       - `InteractionHintOverlay.cs`
       - `SpringDay1WorldHintBubble.cs`
       - `SpringDay1InteractionPromptRuntimeTests.cs`
       目标是“左下角提示 / 头顶提示 / 任务优先文案”的玩家面整合。
    2. 玩家气泡 formal-face：
       - `PlayerThoughtBubblePresenter.cs`
       - `PlayerThoughtBubblePresenterStyleTests.cs`
       目标是把玩家气泡拉回与 NPC 正式面一致，并加厚测试护栏。
  - `UI` 当前仍未真正 claim 完成的，主要是：
    - 玩家真实观感终验
    - 一部分 targeted test / live test
- 当前边界判断：
  - 理论上更合理的分界仍是：
    - `NPC` 守：
      - `NPCBubblePresenter.cs`
      - `PlayerNpcChatSessionService.cs`
      - `PlayerThoughtBubblePresenter.cs`
      - `NPCInformalChatInteractable.cs`
      - 自动续聊 / 跑开中断 / NPC 会话节奏 / 双气泡底座
    - `UI` 守：
      - `SpringDay1ProximityInteractionService.cs`
      - `InteractionHintOverlay.cs`
      - `SpringDay1WorldHintBubble.cs`
      - 左下角 / 头顶提示 / 任务优先文案 / shared prompt shell
  - 但现场现实并没有完全按这条线收干净：
    - `UI.json` 当前仍把
      - `PlayerThoughtBubblePresenter.cs`
      - `PlayerNpcChatSessionService.cs`
      - `NPCBubblePresenter.cs`
      - `PlayerThoughtBubblePresenterStyleTests.cs`
      挂在自己的 `ACTIVE` slice 名下
    - working tree 里这些文件也确实仍是脏的
  - 所以当前最准确的说法是：
    - “我已经知道应该怎么切边界”
    - 但“现场 state 与 dirty 归属”还残留历史交叉，没有彻底回正成干净两条线
- 当前恢复点：
  - 后续如果继续推进 NPC，本线应继续只认 NPC own 失败项。
  - 若要把 NPC/UI 边界真正收干净，后面还需要治理或用户再裁一次“当前这组四文件到底由谁最终收口”。

## 2026-04-03｜新主线切到“春一日新 NPC 群像”：已形成给 `NPC-v` 的联合完工 prompt

- 当前主线目标：
  - 把新增 8 个 NPC 资源真正接进 `spring-day1`，并明确区分：
    - `NPC-v` 负责 NPC 本体层
    - 我负责 `spring-day1` integration 层
- 当前已确认的现场基线：
  - 已真实生成：
    - `Assets/222_Prefabs/NPC/101~301.prefab`
    - `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
    - `Assets/100_Anim/NPC/101~301/*`
    - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
  - 已新增：
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
    - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
- 本轮完成：
  - 已新增联合完工续工 prompt：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-NPC-v_春一日新NPC群像联合完工续工prompt-02.md`
  - 这份 prompt 已明确写清：
    - 我已完成的部分
    - 总需求盘点
    - `NPC-v` 的 own 范围
    - 我的 own 范围
    - 还没闭掉的运行态与集成态项
- 当前恢复点：
  - 下一轮由 `NPC-v` 先补 NPC 本体层缺口。
  - 我随后只接 `spring-day1` 的 phase / runtime integration 层，不再把两条线混做。

## 2026-04-03｜NPC 总线补记：新群像 8 人的本体层 preflight 已落地，首个 own blocker 已被修掉

- 当前主线目标：
  - 按联合完工 prompt，先把“春一日新 NPC 群像”的 `NPC-v` 本体层收扎实，再把 Day1 integration 留给另一侧。
- 本轮稳定事实：
  - 8 套产物表面齐全这一点已经被只读 preflight 坐实：
    - `8` 张 sprite
    - `8` 套动画目录
    - `8` 个 prefab
    - `8` 份 roam profile
    - `8` 份 dialogue content
    - `1` 份 crowd manifest（`8` 条 entry）
  - 8 个 crowd prefab 本体挂载也齐：
    - `NPCAutoRoamController`
    - `NPCInformalChatInteractable`
    - `roamProfile` 引用
    - `homeAnchor` 引用
  - 但这轮真正抓到的 first blocker 不是 prefab 漏挂，而是：
    - 8 份 `DialogueContent.asset` 开工前全部是空壳
    - 因而“可聊”在本体层是假的
- 本轮修复：
  - 已修 `SpringDay1NpcCrowdBootstrap.cs`，把 content 资产写入链改成真实回填 `NPCDialogueContentProfile` 私有序列化字段。
  - 已通过当前打开的 Unity 实例重跑：
    - `Tools/NPC/Spring Day1/Bootstrap New Cast`
  - 修后 8 份 content 资产都已具备：
    - `npcId`
    - `playerNearbyLines`
    - `defaultInformalConversationBundles`
    - `defaultWalkAwayReaction`
    - `defaultChatInitiatorLines`
    - `defaultChatResponderLines`
- 当前阶段判断：
  - `NPC-v` 这一层现在从“结构齐但 content 空壳”推进到了“本体层基础内容成立”
  - 但还不能说整条新群像已经收口，因为：
    - pair-specific 语义仍空
    - runtime phase 消费仍是 integration 侧未闭项
- 当前恢复点：
  - 若继续 `NPC-v`，下一刀优先级应是：
    - pair-specific 内容是否补
    - 8 人逐个 runtime 的 roam/chat/facing 校验
  - `Primary` 与 Day1 phase 消费，继续不由 NPC 线主拿。

## 2026-04-03｜并行协作口径补齐：已补出本线程自用任务单

- 用户补充裁定：
  - 下一轮不是“只给 `NPC-v` 一个 prompt”，而是要明确“我和 `NPC-v` 并行，各自有自己的任务单”。
- 本轮补齐：
  - 已新增本线程自用任务单：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-本线程_春一日新NPC群像Day1整合并行任务单-03.md`
  - `NPC-v` 的联合 prompt 也已回写引用这份任务单，形成双文件配对。
- 当前恢复点：
  - 下一轮的正确并行关系已经固定为：
    - `NPC-v` 做 NPC 本体层
    - 我做 Day1 integration 层
  - 两边最后以同一组“春一日新 NPC 群像”闭环为目标，不再互相吞区。

## 2026-04-03｜NPC 总线补记：Day1 integration 侧已 ready，群像不再只停在本体层

- `spring-day1` 这侧已按并行任务单完成一轮真实 integration 收口：
  - 稳定 anchor 解析已接回 `*_HomeAnchor`
  - Day1 snapshot 已能带出 crowd / world-hint / player-facing 三组摘要
  - `Ready-To-Sync` 已通过
- 这意味着“春一日新 NPC 群像”当前不再只是资源和 prefab 已生成：
  - Day1 integration 侧也已经进入可白名单提交状态
- 当前总线恢复点：
  - `NPC-v` 继续只盯本体层剩余问题
  - `spring-day1V2` 这条 slice 下一步只剩白名单 sync 和收尾 state，不应再扩回 NPC 生产链

## 2026-04-03｜总线收盘补记：Day1 integration 白名单提交已完成

- `spring-day1V2` 这条并行 slice 已完成：
  - white-list sync
  - push
  - `Park-Slice`
- 提交基线：
  - `03c0bf87`
- 当前恢复点：
  - “春一日新 NPC 群像”这件事现在已经不是未提交现场。
  - 后续只需要围绕剩余运行态问题继续，不必再替这轮 integration 做 same-root hygiene 尾账。

## 2026-04-03｜NPC 总线补记：`NPC-v` 的 preflight 已升级成工具护栏，`002 StuckCancel` 当前更像 scene 旧账

- 当前主线目标：
  - 在“春一日新 NPC 群像”这条总线上，继续把 `NPC-v` own 和 legacy scene blocker 分干净，不再把所有 NPC 异常揉成一团。
- 本轮新增稳定事实：
  - `NPC-v` 这轮继续只认了：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
  - 当前新群像验证菜单已经扩成全链 preflight：
    - sprite
    - anim controller
    - clip 数量
    - prefab 组件挂载
    - prefab 引用一致性
    - `pairDialogueSets` 的 partner 合法性 / reciprocal 闭合
  - 通过当前 Unity 再跑：
    - `Tools/NPC/Spring Day1/Validate New Cast`
    已再次拿到：
    - `PASS`
    - `npcCount=8`
    - `totalPairLinks=16`
- 本轮只读边界结论：
  - 用户刚贴的 `002 roam interrupted => StuckCancel`，当前更像 `Primary.unity` 里旧 `001 / 002 / 003` 和各自 `HomeAnchor` 分离导致的 scene 旧账。
  - 只读核对结果：
    - `001`
      - 轻度偏离
    - `002`
      - 大幅偏离
    - `003`
      - 大幅偏离
  - 所以这组 warning 目前不应直接回灌成“`NPC-v` 的新 8 人本体层又写坏了”。
- 当前恢复点：
  - `NPC-v` 下次若继续，应优先做“新 8 人 runtime targeted probe”；
  - 旧 `001 / 002 / 003` 的 `HomeAnchor` 漂移，应交回 `Primary / scene / Day1` 侧单独善后。

## 2026-04-03｜NPC 总线最新收束：新群像当前以“运行态闭环”作为唯一剩余主线

- 当前主线目标：
  - 基于用户重新盘问“现在到底差多远、下一步到底是什么”，把 NPC 总线从旧 `002 / 003` 闲聊体验线和新 8 人群像线里彻底分清。
- 本轮结论：
  - 旧 `002 / 003` 非正式聊天验收包、自省单、UI 接管委托，当前都退为历史背景材料。
  - NPC 总线当前对用户最重要的新主线只有一个：
    - 春一日新 8 人群像要在 Day1 里真正活起来。
  - 这条线现在的剩余项不是“继续想设定”，而是两段运行态闭环：
    1. `NPC-v`
       - 新 8 人本体 runtime probe
       - own dirty / memory 归仓
    2. `spring-day1V2`
       - Day1 phase 消费矩阵
       - integration 归因
- 当前恢复点：
  - 后续若继续看 NPC 总线进度，优先看：
    - 新 8 人是否真能在 runtime 里实例化、聊天、成对说话、跑开收尾
    - Day1 是否真的在 `CrashAndMeet -> DayEnd` 的各阶段消费了这批人
  - 不再把“结构和 preflight 已成立”误表述成“群像已经扩完”。

## 2026-04-03｜总线补记：Day1 probe 现场先被 `NPC-v` compile red 挡住，但 own 快照缺口已补

- 当前主线目标：
  - 继续推进“新 8 人在 Day1 里是否真的被消费”的运行态闭环。
- 本轮新增事实：
  - `spring-day1V2` 先在 own 范围补了最小 crowd probe 字段：
    - active 列表
    - anchor 命中名
    - fallback 标记
    - 当前位置
  - 这说明 Day1 integration 这侧已经把“现有快照信息不够”的 own 缺口先补上了。
- 当前 blocker：
  - 真正挡住这轮 runtime 的不是 `spring-day1V2` 自己，而是 `NPC-v` 当前 own 文件：
    - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    仍在 compile red
  - 所以现在总线最准确的阻塞关系是：
    - `NPC-v own compile red`
    -> 挡住 Unity 退出 compiling
    -> 挡住 Day1 runtime consumption probe
- 当前恢复点：
  - 等 `NPC-v` compile red 清掉后，下一步应直接回到：
    - `CrashAndMeet -> DayEnd` 的 crowd 消费矩阵
  - 不需要再为 Day1 侧额外扩新工具。

## 2026-04-03｜NPC 总线现状重排：用户现在可验“单体成立”，未完项收敛为 pair + Day1 消费

- 当前主线目标：
  - 收到 `NPC-v` 最新 runtime probe 回执后，把 NPC 总线进度重新压成用户能直接判断的状态图。
- 本轮结论：
  - 新 8 人当前已经可以明确分成三层：
    1. 单体本体层：
       - 已基本站住
    2. pair 群像层：
       - 仍有局部失败
    3. Day1 剧情消费层：
       - 仍未拿 runtime 矩阵
  - 现在用户已经能验收到的内容包括：
    - 新 8 人是否会实例化
    - 新 8 人是否能单体闲聊
    - 新 8 人在跑开时是否能正常收尾
  - 现在仍不能宣称完成的内容包括：
    - 代表性 pair dialogue 是否真正亮起来
    - `CrashAndMeet -> DayEnd` 是否按阶段消费了他们
- 当前恢复点：
  - NPC 总线当前最准确的一句话结论是：
    - “人已经大体活了，但群像还没完全说起来，Day1 也还没把他们逐阶段验完。”

## 2026-04-03｜总线补记：`NPC-v` runtime targeted probe 已拿到硬结果，pair 问题压缩到 ambient bubble 发射链

- 当前主线目标：
  - 把“春一日新 8 人群像”从结构完成推进到运行态结论完成。
- 本轮真实完成：
  - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
    - 修掉 own compile red
    - 扩成可重复执行的 runtime targeted probe
    - 增补 pair timeout 诊断，能直接回报决策、解析到的台词、bubble 状态与 conversation owner
  - Unity 里再次稳定通过：
    - `Tools/NPC/Spring Day1/Validate New Cast`
    - `PASS | npcCount=8 | totalPairLinks=16`
  - 已真实跑完新 8 人 runtime targeted probe，稳定结果为：
    - `8/8 instance`
    - `8/8 informal chat`
    - `0/2 pair dialogue`
    - `2/2 walk-away interrupt`
- 当前最关键结论：
  - 两组 pair：
    - `101 <-> 103`
    - `201 <-> 202`
    都已经进入：
    - `initiatorDecision = chatting with ...`
    - `responderDecision = joined chat with ...`
  - 且 pair 台词在运行时已能解析出来：
    - `count = 2`
    - 首句也能读到
  - 但同一时刻：
    - `NPCBubblePresenter.visible = false`
    - `suppressed = false`
    - `channel = Ambient`
    - `conversationOwner = none`
  - 所以当前 pair 问题已经压成：
    - `ambient pair decision 已成立`
    - 但 `ambient bubble` 没真正播出来
  - 这不再像 Day1 integration 问题，也不再像玩家提示压制或旧 `Primary` scene 干扰。
- 现场额外事实：
  - 本轮 Unity 多次弹出：
    - `打开场景已在外部被修改。`
  - 这个模态框会直接冻结 `CodexEditorCommandBridge`。
  - 为避免把外部 scene 改动吞进当前会话，本轮统一点：
    - `忽略`
    只恢复 probe 通道，不改 scene 主刀边界。
- 当前恢复点：
  - `NPC-v` 如果继续下一刀，不该再回旧 `002 / 003`，也不该再扩 prefab / asset 结构；
  - 应直接盯：
    - `NPCAutoRoamController` 的 ambient pair bubble 发射链
    - 为什么台词已解析、决策已成立，但 `NPCBubblePresenter` 没亮起来。
  - 当前收口还差：
    - `Ready-To-Sync`
    - own dirty / memory 是否能真实归仓

## 2026-04-03｜总线补记：`Ready-To-Sync` 未过，当前不是 probe blocker，而是 NPC 历史 own roots 尾账 blocker

- 本轮新增事实：
  - `Ready-To-Sync.ps1 -ThreadName NPC`
    已真实执行。
  - 返回结果：
    - `status = BLOCKED`
  - 第一真实 blocker 不是新 8 人 runtime probe 本身，而是：
    - `NPC` 线程 own roots 下仍有历史残包未收净
- 当前最关键 blocker：
  - 至少已报实包含：
    - `M Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
    - 未归仓的 `0.0.2` prompt 文档
    - `Assets/Editor/NPC/CodexEditorCommandBridge.cs(.meta)`
  - 因此当前不能只带：
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - 本轮 memory
    直接 sync
- 当前恢复点：
  - 总线层面现在要明确区分两类未闭项：
    1. 运行态未闭：
       - pair ambient bubble 发射链
    2. 收口未闭：
       - NPC 历史 own roots 尾账清理 / scope 收口

## 2026-04-03｜补记：春一日新群像线先回到原剧本人物核实，不再默认接受现编 8 人设

- 当前主线目标：
  - 继续推进“春一日新 NPC 群像”时，先把角色真相来源与当前实现现场分开，避免把现编 8 人设误写成原案。
- 本轮查实：
  - 用户原始 Day1 剧情与 `0.0.1剧情初稿` 已经明确存在：
    - `马库斯`
    - `艾拉`
    - `卡尔 / 研究儿子`
    - `老杰克`
    - `老乔治 / 老铁匠`
    - `老汤姆`
    - `小米`
    - `围观村民 / 饭馆村民 / 小孩`
  - 当前工程中也已存在老 Day1 主角色底座：
    - `NPC_001_VillageChief*`
    - `NPC_002_VillageDaughter*`
    - `NPC_003_Research*`
  - 因此当前 crowd 新 8 人：
    - `莱札 / 炎栎 / 阿澈 / 沈斧 / 白槿 / 桃羽 / 麦禾 / 朽钟`
    不能继续默认 claim 为原 Day1 正式人物
- 当前恢复点：
  - `NPC-v` 下一轮先做“原案角色 -> 当前 `101~301` 槽位”映射与最小本体回正
  - `spring-day1V2` 下一轮先做 Day1 角色消费矩阵与老角色 / 新 crowd 的整合分层

## 2026-04-03｜补记：原剧本核实双 prompt 已本地提交，当前外部 blocker 是远端推送失败

- 本轮新增事实：
  - 原剧本核实与双 prompt 重写已本地提交：
    - `e4ef0ad4`
    - `2026.04.03_spring-day1V2_03`
  - 远端推送未完成，失败原因是：
    - 代理 `127.0.0.1:7897` 无法连接
- 当前恢复点：
  - 当前线程状态：
    - `PARKED`
  - 仓库相对 upstream：
    - `ahead 1`

## 2026-04-03｜补记：新 8 人目前主要停留在 probe 与 manifest 层，不应再被误说成“玩家已正式看到”

- 当前主线目标：
  - 在“春一日新 NPC 群像”总线里，纠正用户可见层的错误预期。
- 本轮新增稳定事实：
  - 新 8 人此前的核心运行态证据，来自 `NPC-v` 的 targeted runtime probe；
  - probe 在 Play Mode 下会把测试实例生到隔离坐标区，而不是直接摆在玩家当前正常观察到的村中演出位。
  - 因此用户现在在 `Primary` 里没直接看到他们，是符合当前工程现场的。
- 当前总线判断：
  - 这批角色目前已经具备：
    - prefab / 对话资产 / roam profile / manifest / targeted runtime probe
  - 但还不具备：
    - 已对齐原剧本的人设结论
    - 已完成 Day1 正式消费的结论
- 当前恢复点：
  - 后续必须先做：
    - 原案角色映射回正
    - Day1 消费矩阵
  - 之后才谈“这批人已经把原剧本真正扩进 Day1”

## 2026-04-03｜补记：原 Day1 角色消费矩阵与现有工程承载分层已完成第一轮钉实

- 当前主线目标：
  - 把“原 Day1 剧本里真正需要谁”和“当前工程里谁在承载、谁只是 crowd 槽位”彻底拆开。
- 本轮新增稳定结论：
  - 原 Day1 的稳定主角色链是：
    - `马库斯`
    - `艾拉`
    - `卡尔 / 研究儿子`
  - `老杰克 / 老乔治 / 老汤姆 / 小米`
    已在原案中明确存在，但当前仍未在 Day1 正式 runtime 里获得稳定承载。
  - 当前工程里：
    - `NPC001 / VillageChief`
      是 `马库斯` 主链壳
    - `NPC002 / VillageDaughter`
      是 `艾拉` 主链壳
    - `NPC003 / Research`
      只是 `卡尔` 的语义近邻壳
    - `101~301`
      仍是 crowd 槽位层，不是“原剧本正式角色已扩完”
- 当前恢复点：
  - `NPC-v` 先做原案映射回正
  - `spring-day1V2` 再据此决定未来是否收窄 crowd manifest 的 phase 与语义

## 2026-04-03｜补记：春一日新 8 人原剧本人设核实已完成第一轮只读裁定

- 当前主线目标：
  - 弄清楚春一日新 `101~301` 到底哪些是原案角色、哪些只是后补 crowd 槽位。
- 本轮新增稳定结论：
  - 原案 Day1 主角色链仍然是：
    - `马库斯`
    - `艾拉`
    - `卡尔 / 研究儿子`
  - 原案长线稳定人物仍然是：
    - `老杰克`
    - `老乔治 / 老铁匠`
    - `老汤姆`
    - `小米`
  - 老 `NPC001/002/003` 是当前最接近原案主角色链的工程承载。
  - 当前新 `101~301` 不能再整体 claim 为“原剧本正式角色已经扩完”；
    其中大多数仍只能视作 crowd 槽位或明显写偏的后补设定。
- 本轮状态：
  - 只读完成
  - 无 NPC 本体资产改动
  - 当前 thread-state 继续维持 `PARKED`
- 当前恢复点：
  - 后续先补 cast 映射证据或明确降级策略，再继续最小 NPC-own 回正
