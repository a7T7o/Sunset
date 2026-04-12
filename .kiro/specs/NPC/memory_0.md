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

## 2026-04-03｜补记：NPC 对话头像已按“静态大头照”方向重做落盘

- 当前主线目标：
  - 按用户最新裁定，把 `Assets/Sprites/NPC/*.png` 的 `3x4` 行走图角色，重做成真正可用于 RPG 对话框的静态大头照，而不是再输出 walking sprite 裁切版。
- 本轮子任务：
  - 只处理外部头像产物，输出到：
    - `D:\UUUnity\NPC\<角色名>\01.png ~ 10.png`
  - 不修改 Sunset 内的 NPC 源图、meta、Prefab、动画和 DialogueUI。
- 本轮实际完成：
  - 清空并重建了外部输出根目录：
    - `D:\UUUnity\NPC`
  - 对当前 `11` 个 NPC 源图：
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
    全部重新生成了 `10` 张透明底静态大头照。
  - 当前总落盘结果为：
    - `11` 个角色目录
    - `110` 张 PNG
- 本轮采用的新方法：
  - 不再把右向 walking frame 直接裁半截放大。
  - 改成从右向帧里拆出：
    - 头部层
    - 肩胸层
    再重新组合成 bust 构图，强行提高头部占比，只保留对话头像需要的静态上半身信息。
  - `10` 张版本通过：
    - 不同头部裁框
    - 不同肩胸宽度
    - 不同近远与落位
    做轻微可选差异，但都保持：
    - 朝向右
    - 平和基础状态
    - 透明背景
- 已验证事实：
  - 每个角色目录当前都有且只有 `10` 张 PNG。
  - 抽查总览后，当前首张样式已经不再是此前那种“整张 walking sprite 裁半截”的错误产物，而是明显偏对话 bust 的静态大头照构图。
  - 本轮未修改：
    - `Assets/Sprites/NPC/*.png`
    - `Assets/Sprites/NPC/*.png.meta`
    - 任何 Sunset 内的 tracked 美术资产
- 当前阶段判断：
  - 这轮站住的是：
    - `targeted probe / 局部验证`
  - 也就是：
    - 外部头像资产已经可挑选、可使用
    - 但还没有进入 DialogueUI 或 Unity 对话框里的真实玩家视面终验
- 当前风险：
  - 部分强侧脸角色，如：
    - `201`
    - `202`
    - `301`
    脸部可读性仍然天然受原始右向帧遮挡影响。
  - 所以这轮不能包装成“最终审美一定过线”，只能说：
    - 静态大头照方向已真正纠偏
    - 当前基础资产已可进入挑选与接入阶段
- thread-state：
  - 已跑：
    - `Begin-Slice.ps1 -ThreadName NPC -CurrentSlice NPC静态大头照重做`
- 当前恢复点：
  - 用户现在可以直接在：
    - `D:\UUUnity\NPC`
    查看和挑选每个角色的 `10` 张静态大头照。
  - 如果继续下一刀，最合理的是：
    - 先选每个角色的默认头像
    - 再决定是否对少数角色做更强的局部人工精修

## 2026-04-03｜纠偏补记：第三轮“静态大头照”仍被用户明确判定为丑陋且不可用

- 当前主线目标：
  - 不再把第三轮 `D:\UUUnity\NPC` 静态大头照包装成“已经基本可用”，而是按用户最新审美否决收成真实失败结论。
- 本轮新增稳定结论：
  - 当前 `D:\UUUnity\NPC` 下面这批第三轮静态大头照，用户已明确判定：
    - 丑
    - 恶心
    - 不是人
    - 不可用
  - 因此 NPC 对话头像这条线的当前最诚实状态是：
    - 仍然没有产出一批真正过线的可用头像
- 当前失败不再是“小修可救”的层级，而是两层根因叠加：
  1. 方法错位：
     - 我仍然试图从 `32x32` walking sprite 的右向帧里，通过裁框、重组、放大，去“推导”出最终对话头像
     - 这类输入信息太少，顶多能做结构试探，不能稳定做出好看的正式人像
  2. 审美能力不足：
     - 我没有拿到一个已经过稿的正式头像母版，就直接批量量产
     - 结果是“结构上像 bust”，但美术上依然很丑
- 当前阶段判断：
  - 这条线现在不能再说 `targeted probe 已基本可用`
  - 正确口径应回退为：
    - `结构性试错失败，体验层明确不通过`
- 正确纠偏路线已经收窄为：
  1. 停止继续批量生成 `11 x 10`
  2. 先只做 `1` 个角色的单样张
  3. 这个单样张必须是：
     - 手工重绘或至少重度手工修型
     - 在 Aseprite 里按“正式头像母版”去做
     - 不是再从行走帧自动裁切 / 自动拼装
  4. 只有当单样张被用户明确点头后，才允许进入：
     - 同角色小变体
     - 其他角色迁移
     - 批量导出和命名自动化
- Aseprite / MCP 的最新定位：
  - 仍然有价值
  - 但它们的价值应降回：
    - 画布与像素编辑底座
    - 导出与命名自动化
    - 批次管理
  - 不应再被当成“能自动解决审美”的生成主脑
- 当前恢复点：
  - 后续如果继续 NPC 对话头像，下一刀不该再是“重跑整批”
  - 而应固定为：
    - 选 `1` 个代表角色
    - 做 `1` 张正式头像母版
    - 只验证“这张是否终于像人、像对话头像、像正式美术”

## 2026-04-03｜补记：已按单样张路线落出 `102` 的 A/B/C 验牌包

- 当前主线目标：
  - 不再批量量产，而是按纠偏路线先做 `1` 个代表角色的正式样张，让用户先验“方向”。
- 本轮子任务：
  - 只做 `102` 的单角色验牌包，不扩到其他角色。
- 本轮实际完成：
  - 已在外部目录落出：
    - `D:\UUUnity\NPC\_验牌_102_2026-04-03_单样张`
  - 当前样张包包含：
    - `102_master_A.png`
    - `102_master_B.png`
    - `102_master_C.png`
    - 对应的 `.aseprite` 源文件
    - `102_contact_sheet.png`
- 本轮方法：
  - 不再从 walking sprite 自动裁切 / 自动拼装。
  - 改成围绕 `102` 的角色特征：
    - 灰褐色碎发
    - 红色发带 / 发束
    - 蓝色上衣金色饰线
    - 灰色肩部披挂
    做单角色手工修型母版探索。
  - 这轮只比较：
    - `A / B / C` 三个脸型与发型方向
    - 不比较别的角色，也不比较批量流程
- 当前阶段判断：
  - 这是：
    - `targeted probe / 单样张验牌`
  - 不是“正式头像已经过线”，而是“终于把任务收缩到可被人真正验方向的一张牌”
- 当前恢复点：
  - 用户现在应优先只看：
    - `D:\UUUnity\NPC\_验牌_102_2026-04-03_单样张\102_contact_sheet.png`
    - 或三个 `.aseprite / .png`
  - 后续是否继续，取决于：
    - 哪个变体相对最接近
    - 还是三张都继续否掉

## 2026-04-03｜补记：用户给出新参考图后，单样张主刀已从 `102` 改回更贴近参考的 `001`

- 当前主线目标：
  - 不再围绕失败的 `102 A/B/C` 验牌包兜圈，而是按用户新给的参考图，只做 `001` 的一张参考图学习样张，先验“像不像人、像不像正式对话头像”。
- 本轮子任务：
  - 只做 `001`，只落 `1` 张 PNG，不再同时开多版本，不再重跑整批。
- 本轮新增稳定结论：
  - 用户新给的参考图已经成为这条线的最新视觉权威。
  - 参考图最关键的学习点不是“细节更多”，而是：
    - 大头占比更高
    - 三分之四朝右的人脸结构更清楚
    - 单眼、鼻梁、额头、胡子是大块分面
    - 绿披风只做大轮廓，不抢脸
  - 因此这轮不再尝试从 walking sprite 裁切出多个候选，而是手工重画一个更贴近参考图结构的 `001`。
- 本轮实际完成：
  - 已在外部目录落出新的单样张目录：
    - `D:\UUUnity\NPC\_验牌_001_2026-04-03_只看这一张`
  - 当前只保留一张主要验牌图：
    - `D:\UUUnity\NPC\_验牌_001_2026-04-03_只看这一张\001_refstudy_v1.png`
  - 这张图采用的是：
    - `001` 源角色配色
    - 手工像素簇重画
    - 更紧的胸像裁切
    - 单眼 + 右向三分之四脸 + 老年胡子 + 绿披风的大关系
- 本轮没有做：
  - 没扩到别的 NPC
  - 没继续做 `A/B/C`
  - 没把这张图包装成“已经过线”
  - 没修改 Sunset 内的任何 NPC 源图、Prefab、DialogueUI 或 runtime 资源
- 当前阶段判断：
  - `targeted probe / 单样张验牌`
  - 这是新的参考学习单样张，不是最终正式头像已通过
- 当前风险：
  - 这张 `001` 虽然已经从“walking sprite 裁图”纠偏成了真正的头像构图，但美术完成度仍然只能等用户看图拍板，当前线程无权自判“已经好看”
- 当前恢复点：
  - 后续只能有两种走法：
    1. 如果用户认可这张方向，再继续局部精修或迁移
    2. 如果用户仍然否掉，就继续围绕 `001` 单样张重画，不回到批量线

## 2026-04-04｜补记：NPC 当前正式续工口径已改成 `prompt_05`

- 当前稳定认知：
  - NPC 线最新续工目标已经改成：
    - 原剧本角色回正
    - NPC 本体 prefab/content/profile/roam 收口
    - `pair bubble / 旧气泡样式` 的 runtime 收口
  - 不再继续自创 `101~301` 人设来 claim “原剧本已扩完”
  - 不再进入 `spring-day1` opening、UI、Town、字体等外域
- 当前统一入口：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`

## 2026-04-05｜补记：NPC 当前最新正式入口已切到 `prompt_06`

- 当前最新统一入口：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_NPC-v_Day1真值补线与NPC正式非正式优先级续工prompt_06.md`
- 这份入口相对上一版新增：
  - Day1 owner 明确给出的 `formal > casual > ambient` 真值
  - `NPC001/002/003` 与 `101~301` 的承载分层真值
  - “外部编译红只阻断 live，不阻断静态回正”的执行口径

## 2026-04-04｜补记：结合 UI / Day1 最新 prompt 后，NPC 当前应先做分工矩阵，再开 NPC own 第一刀

- 当前稳定认知：
  - `NPC` 不是 UI 总包 owner，也不是 Day1 正式剧情 owner。
  - `NPC` 现在最该守的是：
    - 玩家面 NPC 气泡与会话语义
    - `101~301` 的原剧本口径回正
    - NPC own 的旧气泡样式 / pair bubble / runtime probe 收口
- 当前不再默认吞：
  - `PromptOverlay`
  - `Workbench`
  - `DialogueUI`
  - `Primary.unity`
  - `GameInputManager.cs`
  - Town / 全局字体底座
- 当前下一步：
  - 先给用户交 `exact-own / 协作切片 / 明确不归我` 和第一刀排序；
    用户认可后，再正式开 `NPC own bubble / speaking-owner / 正式-非正式闭环` 这一刀

## 2026-04-05｜补记：NPC / 气泡提示代码归口只读盘点

- 本轮主线：
  - 只读盘点当前 `NPC own` 相关气泡/提示宿主，不进入实现。
- 当前稳定结论：
  1. [NPCBubblePresenter.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs) 仍是 NPC own 的主气泡宿主。
     - 有显式 `BubbleDisplayMode`：`Default / ReactionCue`
     - 也有 `BubbleChannelPriority`：`Ambient / Conversation / ReactionCue`
     - 但 `UpdateStyleVisuals()` 只读取统一的一组颜色/字号字段，`ReactionCue` 目前只改变优先级与前景聚焦，不形成独立视觉 preset。
  2. [NPCBubbleStressTalker.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCBubbleStressTalker.cs)、[NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)、[PlayerNpcNearbyFeedbackService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs) 都还在直接调用 `NPCBubblePresenter.ShowText(...)`，说明 NPC 环境自言自语 / 路过反馈仍走 NPC own 气泡链。
  3. [PlayerNpcChatSessionService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs) 仍通过 `SetConversationChannelActive()`、`ShowConversationImmediate()`、`ShowReactionCueImmediate()`、`SetConversationLayoutShift()`、`SetConversationSortBoost()` 驱动 NPC 对话态气泡。
  4. [NpcWorldHintBubble.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/NpcWorldHintBubble.cs) 从命名与宿主意图上仍偏 NPC old hint，但当前代码里只剩 `HideIfExists()` 调用，没有任何 `RequestShow()` / `Show()` 入口接到 live 运行链，属于“NPC 旧提示壳仍在，当前未上线”。
  5. 结论上，NPC own 当前真正在线的视觉样式按壳算只有 1 套：`NPCBubblePresenter` 的 NPC 气泡 preset；`ReactionCue` 是行为模式，不是独立视觉壳。
- 当前验证状态：
  - `静态代码盘点成立`
  - 未改代码、未跑 live。
- 当前恢复点：
  - 如果以后要继续收 NPC own 边界，优先处理两件事：
    1. 明确 `NpcWorldHintBubble` 是删历史壳还是重新接线；
    2. 若要做 `NPC pair bubble / speaking-owner` 样式扩展，先决定是否真的让 `ReactionCue` 落到独立视觉参数，而不是继续只当优先级模式。

## 2026-04-05｜补记：NPC 线程已把样式盘点与 own 底座修复口径继续钉死，现因外部编译 blocker 停在 PARKED

- 当前稳定认知：
  - `NPC` 线程这轮继续做的是：
    - 非正式聊天会话底座
    - NPC 气泡 presenter
    - 气泡布局 / speaking-owner / stale owner 护栏
  - 没有继续吞：
    - `Primary.unity`
    - `GameInputManager.cs`
    - `PromptOverlay / Workbench / UI shell`
- 新增结论：
  1. 当前代码盘点下：
     - live 主样式 = `4`
     - 工程总壳 = `7`
  2. NPC 侧 own 脚本当前脚本级无红；
     但项目级 Unity 编译被 `PlacementManager.cs` 外部错误挡住。
  3. 因此正确阶段判断只能是：
     - `结构 / checkpoint` 已继续站住
     - `Unity 运行态验证` 暂被外部 blocker 卡住
- 当前恢复点：
  - 等 shared 外部编译错误解除后，再恢复 NPC 线 targeted probe 与用户终验准备。

## 2026-04-05｜补记：NPC 线程已把本线测试编译红清掉，当前剩余 console 红已回到 foreign

- 当前稳定判断：
  1. 这轮用户指出的“大量报错”，在 NPC 线内真正属于我的，是 3 份 Editor tests 对 runtime 类型的强绑定编译红。
  2. 这批红已经清掉。
  3. 当前 console 剩余红不再属于这批 NPC own 测试：
     - `SpringDay1PromptOverlay.cs` 运行态 coroutine/inactive 红
     - `DialogueChinese Pixel SDF.asset` importer 红
- 当前有效结果：
  - `NpcAmbientBubblePriorityGuardTests.cs`
  - `NpcInteractionPriorityPolicyTests.cs`
  - `NpcCrowdDialogueTruthTests.cs`
  已全部改成反射/非泛型资产加载口径，避免继续受 `Tests.Editor.asmdef` 与 runtime 预定义程序集边界影响。
- 当前恢复点：
  - NPC 线下一步可以直接继续本体收口，不必再为这组三份测试编译红绕路。

## 2026-04-05｜补记：只读定位 NPC 撞墙静默卡死的吞没分支

- 当前主线目标：
  - 只读分析 NPC 撞墙 / 零推进时，哪些状态机会把现场吞成 pause，而不是留下显性 `RoamMoveInterrupted`。
- 本轮子任务：
  - 聚焦 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - 必要时补读 `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
- 本轮已证实：
  1. `CheckAndHandleStuck()` 命中 `progress.ShouldCancel` 后，会先走 `TryHandleTerminalStuck()`；而 `TryHandleTerminalStuck()` 在连续 terminal stuck 达到阈值时直接 `EnterLongPause()`，会绕过原本应走的 `TryInterruptRoamMove(StuckCancel)`。
  2. `TickShortPause()` 与 `TickLongPause()` 在 `TryBeginMove()` 失败时，都只会重新 `EnterShortPause(false)`；这会把“贴墙后还是抽不到新可走点 / 新路径”的现场继续吞成 pause 循环。
  3. 当前最危险的静默卡墙链，不只是 pause 分支本身，而是 `TickMoving()` 先 `NoteSuccessfulAdvance(currentPosition)`、再 `rb.MovePosition(nextPosition)`；如果物理碰撞实际挡住位移，`NPCMotionController.ResolveVelocity()` 仍优先返回 `_externalVelocity`，会继续把动画和 `IsMoving` 维持在“正在走”，而不是真实零推进。
  4. shared avoidance 的动态 blocker 分支还会在 hard-stop 前主动 `RefreshProgressCheckpoint(...)`，容易把真实零推进长期伪装成“正常让行等待”。
- 最小修复建议：
  - 第一优先：`TryHandleTerminalStuck()`
  - 第二优先：`TickMoving()` 或 `TryHandleSharedAvoidance()`
- 本轮验证：
  - 只读代码核查，无代码修改
  - 未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
    - 原因：本轮始终停留在只读分析
- 当前恢复点：
  - 如果下一轮真的动手修，先把 terminal stuck 改成显性 interruption；
  - 再把“实际没动但逻辑先记成功前进”的链条改掉。

## 2026-04-05｜补记：NPC 自然漫游撞墙静默卡死修复已落双 checkpoint，现有 traversal probe fresh 转绿

- 当前主线目标：
  - 在不改玩家已认可 traversal 业务逻辑的前提下，修 NPC 自然漫游的撞墙/贴墙零推进静默吞没问题。
- 本轮实际推进：
  1. 只锁 `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  2. 把“发了移动命令但实体没动”的情况补成真实 blocked-advance 检测
  3. 把 blocked/stuck recover 成功时误清零 blocked/terminal 计数的问题收紧
  4. 把 `Moving` 态下 `pathCount=0 / waypoint missing` 的静默短停改成显性 interruption
  5. 已落两次代码 checkpoint：
     - `263f4ed0` `npc-wall-stall-recovery-tighten`
     - `bf386811` `npc-moving-path-loss-interrupt`
- fresh 证据：
  - `Tools/Codex/NPC/Run Natural Roam Bridge Probe` => `PASS natural-roam-bridge`
  - `Tools/Codex/NPC/Run Traversal Acceptance Probe` => `PASS bridge+water+edge`
  - `status.json` 结束态：`isPlaying=false`、`isCompiling=false`
  - 最新 `Editor.log` 末段有 `Tundra build success`
- 当前阶段判断：
  - NPC traversal 的桥 / 水 / 边缘 contract 当前 fresh 站住；
  - 之前“撞墙但 warning 被吞掉”的主漏洞已经修到代码和现有 probe 都能对上；
  - remaining risk 已缩到“正式场景其他 choke point 是否还有新样本”，不再是当前已知恢复链本身没补上。
- 当前恢复点：
  - 若用户后续继续报正式场景个别 NPC 卡点，直接以新样本位置做 targeted probe；
  - 若没有新反例，NPC 这条导航恢复链当前可以按“已修到可用”对外报实。
## 2026-04-05｜补记：NPC 非正式聊天本线当前已把“002/003 起不了聊 + 跑开链不稳”压回到 live 可复现通过态
- 当前稳定判断：
  1. NPC own 这轮最关键的修正，是把 formal/casual 门禁从“按剧情 phase 全局禁 casual”改成了“same-object formal takeover only”。
  2. 因此  02 / 003 已不再被 CrashAndMeet 这类 formal phase 整段误杀。
  3. 当前 live 证据已覆盖：
     -  02 两轮 casual 闭环
     -  02 玩家首句打字时跑开中断
     -  03 两轮 casual 闭环
     -  03 玩家首句打字时跑开中断
- 当前边界不变：
  - 这不等于 shared prompt / 左下角提示 / 玩家面 UI 壳已经归我；
  - 这轮仍只成立在 NPC own 的会话与气泡底座层。
- 当前恢复点：
  - 后续如用户继续让 NPC 线收口，优先进入“真实体验复核与细节补口”，而不是再回头排查  02 / 003 为何完全不起聊。

## 2026-04-05｜更正：本层关于 002/003 的新增记录以本条为准

- 正确结论只有三句：
  1. 002/003 不再被 formal phase 整段误杀。
  2. 002/003 的 casual 闭环都能 live 跑通。
  3. 002/003 的玩家首句跑开中断都能 live 跑通。

## 2026-04-05｜补记：NPC 当前又补一层 formal/ambient 静态护栏

- 当前主线未变：
  - 仍只守 `NPC own` 的非正式聊天、NPC 气泡优先级与 crowd 真值；
  - 不回吞 UI 玩家面壳体、Day1 导演和 scene 热根。
- 本轮新增稳定事实：
  1. ambient 已从“formal phase 全局全灭”改成“formal 当前真接管时才压”。
  2. crowd 8 人的 `sceneDuties / semanticAnchorIds / growthIntent / minPhase / maxPhase`
     已全部补到 NUnit 静态矩阵里。
  3. fresh runtime targeted probe 仍保持：
     - `instance=8/8`
     - `informal=8/8`
     - `pair=2/2`
     - `walkAway=2/2`
  4. `Validate New Cast` 继续 `PASS`。
- 当前恢复点：
  - 再往下就不该继续盲补结构了；
  - 优先转用户可感知体验复核，其次才是 crowd 台词语气微调。

## 2026-04-05｜补记：NPC 线程本轮已把 own 测试契约和 EditMode 气泡底座收成可交接状态

- 当前稳定结论：
  1. NPC 线这轮真正补的是两处底座口子：
     - `NpcAmbientBubblePriorityGuardTests` 的同名类型误解析
     - `NPCBubblePresenter` 的 EditMode 过早建 UI
  2. 这两处已收完后，NPC own 相关 fixture 当前都在 full `Tests.Editor` 结果里转为 `Passed`：
     - `NpcAmbientBubblePriorityGuardTests`
     - `NpcBubblePresenterEditModeGuardTests`
     - `NPCInformalChatInterruptMatrixTests`
     - `PlayerThoughtBubblePresenterStyleTests`
  3. NPC 自己的 fresh 运行态证据也还在：
     - `Validate New Cast` = `PASS`
     - runtime targeted probe = `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  4. 清 console 后 fresh CLI 已回到 `errors=0 warnings=0`
- 当前阶段判断：
  - NPC own 现在已经不是“还有代码层硬 blocker 没收掉”；
  - 剩余主要变成两类：
    - crowd 内容语气 / 强度细修
    - 用户亲自做的真实体验终验
- 当前恢复点：
  - 后续继续 NPC 线时，不必再回到“测试为什么还红”的排障态；
  - 直接从内容微调或体验终验进入即可。

## 2026-04-05｜补记：额外只读复核继续支持“旧 interrupt/style fail 已过期”的判断

- 当前稳定结论补强：
  1. `ResumeIntroPlan` 与 `PlayerThoughtBubblePresenterStyleTests` 那批旧失败，现在更像旧 runner / 旧编译快照；
  2. 当前代码与当前已编 DLL 都已对上这些测试要求；
  3. NPC 线后续不该再把时间耗在这批旧失败上，除非 fresh runner 再次给出新的真实失败。

## 2026-04-05｜补记：crowd 内容刷新链与 8 人 interrupt/resume 文案已补齐，当前停在 foreign 噪音后的 fresh runtime 复核

- 本轮完成：
  1. `SpringDay1NpcCrowdBootstrap.cs`
     - 新增只刷新 crowd dialogue 的菜单；
     - 给 8 人补上 default `interrupt / resume` 人设化文案；
     - 用稳定旧 `AssetStem` 锁住 legacy 资产路径，避免再生第二套对话包。
  2. `NpcCrowdDialogueNormalizationTests.cs`
     - 新增 stale crowd 口气黑名单与 default rules 非空护栏。
  3. `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
     - 8 份 legacy dialogue asset 已刷新；
     - 误生成的重复新 slug 资产已清掉。
- 本轮验证：
  - `manage_script validate`：
    - `SpringDay1NpcCrowdBootstrap.cs` clean
    - `NpcCrowdDialogueNormalizationTests.cs` clean
  - `git diff --check` 通过
  - `Editor.log`：
    - `Validate New Cast` => `PASS | npcCount=8 | totalPairLinks=16`
  - 仅跑 `Validate New Cast` 后 fresh console => `errors=0 warnings=0`
  - fresh runtime probe 复跑：
    - 已进 Play 再回 Edit；
    - 但没拿到新的 `PASS / FAIL` 终行；
    - console 混入 foreign 的 menu 串台与 `PersistentManagers` exception
    - 所以这轮 runtime 只算“已尝试、未 clean 归因完成”。
- 当前恢复点：
  - 若继续 NPC 本线，优先先等 foreign 噪音停下后再补一次 clean runtime targeted probe；
  - 再往下才是用户体验终验或 sync 前收口。

## 2026-04-05｜补记：quick test 已重新拿到 clean runtime 结果

- 本轮 quick test 结果：
  - `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
  - `Run Runtime Targeted Probe` = `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  - fresh console = `errors=0 warnings=0`
  - Unity 现场已退回 Edit Mode
- 当前恢复点：
  - NPC 本线当前主要只剩用户真实体验终验；
  - 若体验反馈里还挑出 crowd 文案问题，再做最后一小轮微调。

## 2026-04-05｜补记：从 NPC 视角只读评估 day1 导演续工单，Town 当前可跟但只能按窄口径跟

- 当前稳定判断：
  1. day1 这轮 prompt 没漂，主轴是对的：
     - 真施工
     - 真 live 工具闭环
     - 真后半段导演消费下沉
  2. 从 NPC own 完成度看，NPC 现在已经不再是 Day1/Town 的主要底层 blocker：
     - 8 人 crowd cast / asset / probe / priority 底座都已站住
     - 因而 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01` 这类 anchor 承接，NPC 侧跟得上
  3. `DinnerBackgroundRoot` 也能继续推进一层，但不要直接做成复杂重层
  4. `DailyStand_01` 可以跟，但应排在前面几处 anchor 稳住之后
  5. 当前更像真 blocker 的是 day1 自己的 live 导演工具链：
     - `Run Director Staging Tests` 的菜单桥识别
     - 排练/录制/写回链是否持续稳定
     - 外部噪音不要反复污染导演 live
- 总结：
  - Town 现在“跟得上 day1”，但仅限轻量、具体 anchor、具体 cue 的承接；
  - 还不能把 Town 当成“已经 fully ready 的后半段大舞台”。

## 2026-04-05｜补记：关于“NPC 要不要担起 Day1 份量”的口径再次压实

- 当前补充结论：
  1. 可以担，但担的是“NPC 本体层与群像承接层”的责任，不是替 day1 扛导演工具链。
  2. 现在更该防的误判是：
     - 把 `NPC ready` 误说成 `day1 ready`
     - 把 `Town 可承接` 误说成 `Town 全闭环`
  3. 只要 day1 把导演窗口 live 接上既定 anchor，NPC 这边当前完成度已经足够接住那批内容。

## 2026-04-05｜补记：Day1 后半段群像内容层第一轮已落到 source-of-truth 与资产

- 本轮真实施工范围：
  - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
  - `Assets/YYY_Tests/Editor/NpcCrowdDialogueNormalizationTests.cs`
  - `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
- 本轮完成：
  1. 把 `101/103` 的群像口气往 `EnterVillage / DailyStand` 调整成：
     - 围观停手
     - 孩子偷看
     - 次日照常站位但昨夜议论未散
  2. 把 `104/201/202/203` 的群像口气往 `Dinner / ReturnAndReminder / DailyStand` 调整成：
     - 晚饭冲突背景压力
     - 散场回屋
     - 低声议论
     - 第二天仍得照旧开门、摆摊、开火
  3. 把 `102/301` 的群像口气往 `NightWitness / DayEnd` 调整成：
     - 后坡夜路
     - 回声脚步
     - 收夜规矩
     - 白天照常、夜里露底
  4. 新增资产级语义护栏测试，避免后续再退回“底座在，内容空”。
- 本轮复核：
  - `Validate New Cast` = `PASS | npcCount=8 | totalPairLinks=16`
  - `Run Runtime Targeted Probe` = `PASS | instance=8/8 | informal=8/8 | pair=2/2 | walkAway=2/2`
  - Unity 已回到 `Edit Mode`
- 当前恢复点：
  - NPC 本线接下来优先转用户体验终验；
  - 若终验还指出某个 scene-duty crowd 口气不够，再做最后一小轮定点微调。

## 2026-04-06｜补记：已确认“新 NPC 突然生成”不属于本轮内容层，而属于 crowd runtime 契约

- 用户新增质疑：
  - 新 NPC 应该本来就在村子里；
  - 现在却像是对话/阶段到了才突然生成；
  - 用户要求我判断这件事该怎么办、是谁做的、我现在应不应该接。
- 我这轮只读核查后的稳定结论：
  1. 负责这套行为的核心文件是：
     - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  2. 它的当前机制不是场景常驻，而是：
     - 只在 `Primary` 中维护 crowd
     - 非 `Primary` 切走时 `TeardownAll()`
     - 对缺失的 crowd actor 运行时 `Instantiate(...)`
  3. 所以用户感受到的“突然生成”是真问题，不是幻觉；
     - 但它不是我刚做的群像文本层导致的；
     - 它是更上一层的 runtime deployment / 常驻布置契约问题。
  4. 按当前分工，这件事我现在默认不该直接吞；
     - 因为它已经越到 `SpringDay1NpcCrowdDirector.cs / Town runtime contract / scene 常驻摆放`；
     - 当前这层边界更接近 day1 / Town runtime owner。

## 2026-04-06｜补记：已给 day1 准备好正式回执信与转发 prompt

- 本轮新增文件：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_后半段群像回执与驻村部署问题汇总_09.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_读取回执并判断crowd驻村部署prompt_10.md`
- 目的：
  - 把 `NPC own` 已完成内容、已吸收的 `day1` 真值、以及“crowd 突然生成”这一层的新判断，正式交到 day1 手里；
  - 避免后续再把“内容层问题”和“runtime 驻村部署问题”混成一团。

## 2026-04-06｜补记：NPC resident 语义层与 formal 一次性消费都已站住

- 当前 NPC 本线对 day1 的最新可交付物，不再只是 crowd 文本内容：
  1. `SpringDay1NpcCrowdManifest` 已变成可直接消费的 resident matrix；
  2. `Validate New Cast` 已能检查 resident baseline / beat semantics；
  3. `formal` 段已改成一次性消耗；
  4. formal 消费后只回落到 informal / resident / post-phase 非正式补句。
- 当前最关键判断：
  - `NPC own` 这边能继续独立落地的核心 contract 基本已经压完；
  - 真正剩余的主 blocker 更像：
    - day1 resident deployment
    - director consumption
    - Town / scene 常驻落位

## 2026-04-06｜补记：resident 直接消费 contract 已压到 beat roster，formal 状态也已公开

- 本轮真实新增：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
     - 新增 `SpringDay1CrowdDirectorConsumptionRole`
     - 新增 `GetDirectorConsumptionRole() / IsDirectorBackstagePressureBeat()`
     - 新增 `TryGetEntry()`
     - 新增 `GetEntriesForDirectorConsumptionRole()`
     - 新增 `BuildBeatConsumptionSnapshot()`，可直接给 day1 一次拿到同一 beat 的：
       - `priority`
       - `support`
       - `trace`
       - `backstagePressure`
       - 以及每个 NPC 的 `semanticAnchorIds / note / flags / presenceLevel`
  2. `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
     - `Validate New Cast` 现在会额外检查：
       - 进村段前台 priority roster
       - 晚饭段 backstage pressure roster
       - 夜见闻段 priority roster
  3. `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
     - 新增 `NPCFormalDialogueState`
     - 新增 `HasFormalDialogueConfigured`
     - 新增 `GetFormalDialogueStateForCurrentStory()`
     - 新增 `HasConsumedFormalDialogueForCurrentStory()`
     - 新增 `WillYieldToInformalResident()`
  4. `Assets/YYY_Tests/Editor/NpcCrowdManifestSceneDutyTests.cs`
     - 补了 direct-consumption role + snapshot helper 的护栏
  5. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - 补了 formal consumed 后公开状态 contract 的回归
  6. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补了 formal state public contract 的文本护栏
- 本轮现场插曲：
  - `SpringDay1NpcCrowdManifest.asset` 一度是旧脏资源，`presenceLevel` 全部掉成 `0`
  - 已通过 `Refresh Crowd Resident Manifest` 刷回正确值
  - 随后重新过了 `Validate New Cast`
- 本轮验证：
  - `manage_script validate` clean：
    - `SpringDay1NpcCrowdManifest.cs`
    - `SpringDay1NpcCrowdValidationMenu.cs`
    - `NPCDialogueInteractable.cs`
    - `NpcCrowdManifestSceneDutyTests.cs`
    - `NpcInteractionPriorityPolicyTests.cs`
    - `SpringDay1DialogueProgressionTests.cs`
  - `Assets/Refresh` 后重新跑
    - `Tools/NPC/Spring Day1/Validate New Cast`
    - 最新结果：`PASS | npcCount=8 | totalPairLinks=16`
  - `git diff --check` 通过
  - fresh console：
    - `errors=0 warnings=0`
  - Unity 保持 `Edit Mode`
- 当前边界判断：
  - NPC own 这边这轮已经把最值钱的 helper / contract 层压完；
  - 还没做、也不该默认继续吞的是：
    - `SpringDay1NpcCrowdDirector.cs`
    - `Town / Primary` 常驻布置
    - stage book / deployment runtime 本体

## 2026-04-06｜补记：resident <-> director stagebook 桥接测试已补齐并真实跑过

- 本轮新增：
  1. `Assets/YYY_Tests/Editor/NpcCrowdResidentDirectorBridgeTests.cs`
     - 新增 3 组桥接测试：
       - `EnterVillage_PostEntry / FreeTime_NightWitness` 的 priority resident 必须能在 stagebook 里 resolve 到 cue
       - `DinnerConflict_Table` 当前被导演接管的 cue roster 必须留在 resident priority 白名单内
       - 关键 beat 的 cue `semanticAnchorId / duty` 不得漂出 manifest 合同
  2. `Assets/Editor/NPC/NpcResidentDirectorBridgeValidationMenu.cs`
     - 新增 `Tools/NPC/Spring Day1/Run Resident Director Bridge Tests`
     - 直接跑上面 3 个桥接测试并把结果写到：
       - `Library/CodexEditorCommands/npc-resident-director-bridge-tests.json`
- 本轮真实排障：
  - 菜单第一次没注册不是神秘丢失，而是我新菜单脚本有一处真实编译错：
    - `TestStatus` 被我误写成了可空链
  - 已修正，并重新 `Assets/Refresh`
- 本轮验证：
  - `manage_script validate` clean：
    - `NpcCrowdResidentDirectorBridgeTests.cs`
    - `NpcResidentDirectorBridgeValidationMenu.cs`
  - `Tools/NPC/Spring Day1/Run Resident Director Bridge Tests`
    - `status=passed | total=3 | passed=3 | failed=0`
  - `git diff --check` 对新增 bridge test / menu 通过
  - fresh console：
    - `errors=0 warnings=0`
    - 仅有 test runner 写 XML 的信息项，不算业务红
- 当前判断：
  - NPC own 里还能独立压的“director 直接消费层”现在也基本压完；
  - 再往下就是 day1 / deployment / Town 现场承接，不再是我继续单线往下写的收益最高区。

## 2026-04-06｜补记：已把 NPC 全量进度、边界与协作建议正式回写给 day1

- 当前主线：
  - `NPC` 本线先停在“给 day1 一份足够全面、可直接拿去分配和调度的总回执”；
  - 不继续扩新的 `NPC` 业务代码。
- 本轮新增：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_NPC给day1_全量进度与承接边界回执_14.md`
- 这份回执里已经明确写清：
  1. `NPC` 已做成：
     - resident semantic matrix
     - beat consumption helper
     - formal once-only public contract
     - bridge tests / validation menu
  2. 当前真正 blocker：
     - `day1` resident deployment
     - director consumption
     - Town / runtime 常驻落位
  3. 我还能继续分担的最深层次：
     - `manifest / content profile / formal fallback / tests / targeted probe`
  4. 我明确不建议默认回吞的边界：
     - `CrowdDirector / Town runtime / scene / UI`
- 当前恢复点：
  - 当前 slice 准备 `Park-Slice`
  - 等 `day1` 读完并重新调度后，再决定是否重新开工

## 2026-04-06｜续记：NPC 本线又补完一层 phase-aware resident nearby，并已双回执收口

- 当前主线：
  - 继续只做 `NPC` 本体 own；
  - 不回吞 `director / Town / UI / scene`；
  - 把 `formal consumed -> resident fallback` 从闲聊 bundle 继续压到 nearby 轻反馈。
- 本轮新增：
  1. `NPCDialogueContentProfile.cs`
     - `PhaseNearbySet`
     - `phaseNearbyLines`
     - `GetPlayerNearbyLines(relationshipStage, storyPhase)`
  2. `NPCRoamProfile.cs`
     - phase-aware nearby 透传
  3. `PlayerNpcNearbyFeedbackService.cs`
     - 玩家靠近 NPC 的轻反馈按当前 `StoryPhase` 走 resident nearby lines
  4. `SpringDay1NpcCrowdBootstrap.cs`
     - 8 个 Day1 新居民的 `phaseNearbyLines` 已能生成并落资产
  5. `SpringDay1NpcCrowdValidationMenu.cs`
     - 新增 phase nearby coverage 验证
  6. tests：
     - `NpcInteractionPriorityPolicyTests.cs`
     - `NpcCrowdDialogueNormalizationTests.cs`
     - `SpringDay1DialogueProgressionTests.cs`
  7. 回执：
     - 给 `day1` 的阶段安全点回执已写
     - 给 `存档系统` 的边界回执已写
- 当前验证：
  - own 改动脚本最小自检基本 clean
  - `Validate New Cast` 仍 PASS
  - own `git diff --check` 通过
  - `Run Resident Director Bridge Tests` 最后一份稳定结果仍是 pass；本轮 rerun 没拿到新的 completed 结果，不乱 claim
- 当前判断：
  - `NPC` own 现在已经把 resident fallback 压到了：
    - `phaseInformalChatSets`
    - `phaseNearbyLines`
  - 也就是说后续 runtime 常驻居民语义至少已经有“按 E 闲聊”和“靠近轻反馈”两层可吃回 contract
  - 真正还没做的是 `day1` 那边的 deployment / director / Town 落位承接
- 当前恢复点：
  - 本轮已到安全点，准备 `Park-Slice`
  - 等后续新授权再继续。

## 2026-04-06｜续记：NPC resident contract 再补两层，并顺手清掉 CrowdDirector 爆红

- 当前主线：
  - 继续只做 `NPC` 自己的 resident 常驻语义、formal consumed 回落、以及给 `day1` 可直接消费的 validation/contract；
  - 不回吞 `Town / Primary / CrowdDirector 主逻辑 / UI`。
- 本轮新增：
  1. `phaseSelfTalkLines`
     - 已真实落到 8 份 `SpringDay1Crowd` dialogue assets
     - 长停自语会按 `StoryPhase` 变味
  2. phase-specific `walkAwayReaction`
     - `phaseInformalChatSets` 被玩家中途走开时，现在会按当前 beat 变味
  3. validation / tests
     - selfTalk / walkAway 的 coverage 与 contract tests 已补
  4. carried foreign blocker fix
     - `SpringDay1NpcCrowdDirector.cs` 缺失 helper 已补
     - fresh `errors=0 warnings=0`
  5. 回执
     - `2026-04-06_NPC给day1_阶段安全点回执_16.md`
     - `2026-04-06_NPC_存档边界回执_02.md`
- 本轮验证：
  - `Validate New Cast`：PASS
  - targeted EditMode 小集：job `succeeded`
  - `git diff --check`：通过
  - Unity：`Edit Mode`
- 当前判断：
  - `NPC` 现在已经把 resident fallback 压到了：
    - `phaseInformalChatSets`
    - `phaseNearbyLines`
    - `phaseSelfTalkLines`
    - phase-specific `walkAwayReaction`
  - 继续往下更该由 `day1 / Town` 吃回的是原生 resident deployment 与 scene 落位。
- 当前恢复点：
  - 本轮到此先停，不继续扩 runtime 主逻辑；
  - 写完线程审计后 `Park-Slice`。

## 2026-04-06｜收口：NPC 线程已合法停车

- `Park-Slice` 已执行：
  - 当前 `thread-state = PARKED`
  - `blockers = 无`
- 当前可接力状态：
  - Unity 在 `Edit Mode`
  - fresh `errors = 0`
  - 下一手不需要先替我收爆红或退 Play Mode

## 2026-04-07｜只读结论补记：Town 正式对话后的 NPC 卡顿，第二刀仍先压 NPC own prompt 链

- 当前主线：
  - 用户要求在不扩到更多运行时代码文件的前提下，评估 NPC own 还能安全做哪些性能优化。
- 本轮确认：
  - 第一刀已经砍掉找玩家、反射找 session、部分重复判定；
  - 但 `NPCInformalChatInteractable` 的 session/suppression 链仍先于 distance gate；
  - `NpcInteractionPriorityPolicy` 仍借 `NPCDialogueInteractable.CanInteract(context)` 支付偏重的 formal takeover 判定；
  - 两个 interactable 的 `ReportCandidate(..., () => OnInteract(context), ...)` 仍保留 per-frame capture closure。
- 当前建议：
  - 继续施工时，第二刀先只碰上述 3 个脚本；
  - 不建议本轮扩到玩家侧集中仲裁、shared interaction service、bubble/UI、day1/Town 运行时链路。

## 2026-04-07｜NPC 气泡旧壳补口已落地，旧空 sprite 不会再直接接管现场

- 用户最新把问题重新压回 `NPC` 气泡本身：现场看到的是方泡，不是用户要的圆角气泡。
- 本轮已完成：
  1. `NPCBubblePresenter.cs` 补了旧壳重绑后的 sprite/type/layout 强制刷新；
  2. `NpcBubblePresenterEditModeGuardTests.cs` 新增旧空壳回刷测试；
  3. 整份 `NpcBubblePresenterEditModeGuardTests` 已跑到 `6/6 Passed`。
- 当前判断：
  - 这轮已经把“为什么会方泡”这件事压到代码和守卫层；
  - 但最终观感是否完全回正，仍要靠 `Town` 里人工肉眼终验。

## 2026-04-07｜续刀：Primary/001 方泡继续压到“缓存残留也会回刷”

- 用户继续反馈 `Primary/001` 真实入口 still-square。
- 本轮新结论：
  - scene 旧空壳 `Image.sprite = null` 会直接显示 Unity 默认矩形色块；
  - 首轮补口解决了“初次绑定旧空壳”；
  - 本轮再解决“缓存还在但 sprite 被打回空时，下一次展示前不会再回刷”的漏点。
- 本轮改动：
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    - 新增 `HasResolvedBubbleUi()`；
    - `EnsureBubbleUi()` 不再因为 `_canvas/_bubbleText` 有值就直接早退；
    - 每次展示前都会对已解析 bubble UI 再执行一次 `RefreshBoundBubbleUiAssets()`。
  - `Assets/YYY_Tests/Editor/NpcBubblePresenterEditModeGuardTests.cs`
    - 新增 cached sprite null recovery test。
  - `Assets/Editor/NPC/NpcBubblePresenterGuardValidationMenu.cs`
    - 新增命令桥可调的 bubble guard tests 菜单。
- 本轮验证：
  - `Editor.log` compile success
  - `npc-bubble-presenter-guard-tests.json` = `passed / 7 / 7 / 0`
  - `git diff --check`：通过
- 当前阶段：
  - 代码/守卫测试层已闭环；
  - 真实入口体验仍待用户在 `Primary/001` 复测。

## 2026-04-08｜收到 Day1 新协作 prompt：原生 resident 接管与持久态桥接
- 新 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_给NPC_day1原生resident接管与持久态协作prompt_17.md`
- 主控台当前要求已更新为：
  1. 不再继续补 `runtime spawn`
  2. 把 `native resident scripted control contract` 收出来
  3. 顺带暴露给 `存档系统` 的 resident 最小 snapshot surface

## 2026-04-09｜NPC 走位鬼畜止血：同帧重决策复用补丁

- 用户目标：
  - 不改 NPC 业务语义，只修“走得对但疯狂转向”的底层 bug，并确保不把前面的性能控制打回去。
- 本轮实际改动：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 只改 `CanReuseHeavyMoveDecisionThisFrame(...)`
    - 对 `rb != null` 的物理驱动 NPC，允许同一渲染帧内复用首个重决策结果
    - 目的：减少同帧重复跑共享避让 / repath / 边界约束热链，压低方向来回翻转和性能雪崩
- 关键判断：
  - 这刀不回退既有 traversal contract，不改“去哪里/能不能过”，只收 NPC 调度层
  - 风险主要是极短窗口内响应略钝，但不会推翻玩家桥/水/边界那条主线
- 验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=unity_validation_pending`
    - 当前 external 仅剩 2 条 warning：
      - `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs:105 CS0618`
      - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs:2245 CS0618`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 通过
- thread-state：
  - `Begin-Slice`：已跑（`NPC / npc-anti-jitter-hotpath-fix`）
  - `Ready-To-Sync`：未跑
  - `Park-Slice`：已跑（原因：等待用户是否允许占用 Unity 做最小 live 验证）
- 当前状态：
  - `PARKED`
  - 下一步如果继续，优先做最小 Unity live 验证，确认鬼畜转向和爆卡是否一起回落；不继续扩写业务逻辑。

## 2026-04-09｜NPC 走位鬼畜止血第二刀：原地反向发速度去抖

- 当前主线目标：
  - 继续沿着 `NPCAutoRoamController` 热链收 `NPC` 原地鬼畜转向，不扩到玩家 traversal、scene、binder 或工具。
- 本轮实际改动：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 在 `TickMoving(...)` 里给 `adjustedDirection` 加了 `StabilizeMoveDirection(...)`
    - 新增 `ShouldHoldPreviousMoveDirection()`
    - 只在 `rb != null`、几乎没前进、且当前处于 pending-move / blocked-advance / shared-avoidance / detour 这些阻挡态时，如果新方向几乎要和上一条已发方向打反，就先短暂沿用上一条方向
- 关键判断：
  - 第一刀解决的是“同一渲染帧多次 `FixedUpdate` 重算”
  - 第二刀解决的是“跨渲染帧但仍卡在原地时，方向一左一右来回翻”
  - 两刀都只收调度层 bug，不改 NPC 目的地选择、桥/水/边界 contract，也不回退前面的性能止血
- 静态验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    - `assessment=no_red`
    - `owned_errors=0`
    - `external_errors=0`
    - `current_warnings=0`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 通过
- 当前阶段：
  - 代码层止血已到“可申请 live 验”的状态
  - 但还没有拿到新的 Unity 实跑证据，所以不能宣称 NPC 已完全修好
- 当前恢复点：
  - 若用户允许占用 Unity，下一步只做最小 live 验证：看 NPC 是否还会原地疯狂翻向，以及是否引入新的停滞/爆卡

## 2026-04-09｜只读复盘：鬼畜已收，当前剩余核心 bug 是 FixedUpdate 导航热链过重

- 用户最新反馈：
  - 当前 `NPC` 已经“不抽搐了”
  - 这轮只要求彻底讲清楚：问题核心原因是什么，为什么修了很多轮还会持续冒新问题
- 当前结论：
  1. `鬼畜转向` 已基本被前两刀压住，它主要是调度层的方向抖动 bug
  2. 现在真正显形的大 bug 是另一层：`NPCAutoRoamController.FixedUpdate()` 内的导航恢复/占位/避让热链过重
  3. 热链是：
     - `FixedUpdate() -> TickMoving(...)`
     - `TickMoving(...) -> TryHandleSharedAvoidance / TryHandleBlockedAdvance / TryResolveOccupiableDestination`
     - `NavigationTraversalCore -> NavGrid2D.IsWalkable / IsPointBlocked`
     - `NavGrid2D` 再继续打 `Physics2D.OverlapCircle / OverlapCircleAll`
- Profiler 对应证据：
  - `NPCAutoRoamController.FixedUpdate()[Invoke]` 自耗时约 `1601ms`
  - `Physics2D.OverlapCircleAll` 约 `30ms`
  - `GC.Alloc` 约 `12.8ms`
  - 单帧 GC 分配约 `15.3MB`
- 为什么会修这么多轮：
  1. 不是同一个 bug 一直没修对
  2. 而是多个层级问题叠在一起：
     - 行为层：鬼畜转向
     - 性能层：阻挡态热链太重
     - 契约层：resident / day1 takeover 状态切换
  3. 每次修掉上层症状，下一层问题才有机会真正露出来
- 当前最稳判断：
  - 我这边这两刀收的是“鬼畜转向”
  - 现在该由后续线程继续主刀的是 `FixedUpdate` 热链降本，而不是再回头重修同一套去抖

## 2026-04-09｜继续主线：FixedUpdate 热链降本补丁已落地到 NPC / Traversal / NavGrid

- 当前主线目标：
  - 在不改 NPC 业务语义、不回退玩家 traversal contract 的前提下，把 `FixedUpdate` 导航热链继续砍到更可用的水平，朝“打包态闭环”推进。
- 本轮实际改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - 新增 `PHYSICS_HEAVY_DECISION_REUSE_SECONDS = 0.04f`
     - 让物理驱动 NPC 的重决策缓存从“同一渲染帧”扩到一个极短跨帧窗口
     - 目的：避免每个 `FixedUpdate` 都重跑整条 `TickMoving -> avoidance / repath / bounds` 热链
  2. `Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs`
     - 新增同帧动态 `IsWalkable(...)` 查询缓存
     - 新增 `TryFindNearestWalkable(...)` 的 `80ms` 短窗口缓存
     - 新增 `TryFindPath(...)` 的 `startCell + endCell + gridVersion` 路径缓存
     - `RebuildGrid / RefreshGridRegion` 时自动失效上述缓存
  3. `Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
     - 新增 `CanOccupyNavigationPoint(...)` 同帧 occupancy probe 缓存
     - 目的：减少同一候选点在一帧内被反复做三脚 probe + NavGrid live physics 查询
- 关键判断：
  - 这轮主刀的是“重复查询 / 重复求路 / 重复重决策”
  - 没有改 `NPC` 目的地语义，没有改桥/水/边缘 traversal 规则，也没有改 avoidance 的业务目标
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `assessment=external_red`
    - 外部红来自：
      - `Assets/YYY_Tests/Editor/NpcCharacterRegistryTests.cs` 缺符号
      - `Assets/Editor/Home/HomeFoundationBootstrapMenu.cs` 交叉场景引用错误
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NavGrid2D --path Assets/YYY_Scripts/Service/Navigation --level standard --output-limit 5`
    - `clean`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NavigationTraversalCore --path Assets/YYY_Scripts/Service/Navigation --level standard --output-limit 5`
    - `clean`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs Assets/YYY_Scripts/Service/Navigation/NavigationTraversalCore.cs`
    - 通过
- live smoke：
  - `Town` 运行态短窗 `15s`
  - 没有刷出新的导航运行时红错
  - 仅看到外部 warning：
    - `PackagePanelRuntimeUiKit.cs:105 CS0618`
    - `SpringDay1WorkbenchCraftingOverlay.cs:2245 CS0618`
  - 以及一条 `MCP-FOR-UNITY [WebSocket] Unexpected receive error: WebSocket is not initialised`
  - 这条更像 MCP 通道噪声，不是导航 runtime 自身报错
- 当前阶段：
  - 代码补丁层已落地
  - 最小 live smoke 没出现新的导航红错
  - 但项目现场仍有 external red，不适合宣称“整个项目已 no-red”
- 当前恢复点：
  - 如果后面由我来验收，应优先验：
    1. NPC 运行态是否继续不抽搐
    2. Town/Primary 长时间运行时是否还出现导航导致的帧爆降
    3. 是否只剩外部 unrelated 红面

## 2026-04-09｜只读性能修复建议：FixedUpdate 热链最安全的缓存/节流刀口

- 当前主线目标：
  - 不改代码，只读分析 `NPCAutoRoamController / NavGrid2D / NavigationTraversalCore`，给出最安全的性能修复建议。
- 本轮子任务：
  - 只判断哪些“同帧缓存 / 短窗口节流”最能减轻 `FixedUpdate` 热链，又最不容易伤 NPC 业务语义。
- 本轮已证实：
  - `NPCAutoRoamController` 已经接受“同一渲染帧内重用重决策”这条语义前例：`TickMoving()` 内有 `CanReuseHeavyMoveDecisionThisFrame()`。
  - 当前热链最重的重复查询主要不是 roam 状态机本身，而是 `NavigationTraversalCore.CanOccupyNavigationPoint()` 反复下钻到 `NavGrid2D.IsWalkable()` / `IsPointBlocked()`，以及卡住/回退时的 `TryFindNearestWalkable()` / `TryFindPath()`。
  - `TryRebuildPath()` / `TryBeginMove()` 已经有 `TryAcquirePathBuildBudget()`、`ShouldSkipRebuildPathRequest()`、失败退避等预算口子，所以最安全的策略是沿这些已有边界继续做请求合并，而不是改 `FixedUpdate` 节奏或放大 NPC 行为冷却。
- 当前最值得下刀的点：
  1. `NavigationTraversalCore.CanOccupyNavigationPoint()` + `NavGrid2D.IsWalkable()` / `IsPointBlocked()`：做同帧探测结果缓存，按 `worldCenter/probePoint + queryRadius + ignoredCollider + Time.frameCount` 失效。
  2. `NavGrid2D.TryFindNearestWalkable()`：做 `50~100ms` 的短窗口缓存，并在 `RefreshGrid/RebuildGrid` 后失效；适合吸收卡边、回退、软约束 fallback 的重复查询。
  3. `NavGrid2D.TryFindPath()`：做小型路径结果缓存，键建议用 `startCell + endCell + gridVersion`；静态网格下同起终点反复求 A* 的结果天然稳定。
  4. `NPCAutoRoamController.TryRebuildPath()` / `TryBeginMove()`：沿现有 `rebuild` 去重与 budget 再加一层“同阻塞窗口合并”，只合并相同位置桶、相同目标桶、相同 blocker 语义的短时间重复请求。
  5. `NPCAutoRoamController.TryHandleSharedAvoidance()`：如果前 4 刀还不够，再补“仅同帧”的邻居快照/solver 结果缓存；不要直接做跨帧缓存。
- 当前不建议优先动的点：
  - 不建议先全局拉大 `sharedAvoidanceRepathCooldown` 或降低 `FixedUpdate` 响应频率。
  - 不建议先做跨帧的动态避让结果缓存；这最容易把“性能修复”变成“NPC 反应变钝”。
- 验证结果：
  - 本轮只读，无代码修改、无编译、无 Unity/MCP live 验证。
- 当前恢复点：
  - 如果后续进入真实施工，最稳的落刀顺序应是：`occupancy probe 同帧缓存 -> nearest walkable 短窗口缓存 -> path cache -> rebuild 合并`；只有前 4 刀仍不够时，再碰 avoidance solver 同帧缓存。

## 2026-04-09｜继续主线：NPC 朝向稳定层已从导航脑中拆开，专修偏头/摇头晃脑

- 当前主线目标：
  - 不改 NPC 去哪走、不回退玩家 traversal 语义，只修 `NPC` 运动表现层残留的偏头/摇头晃脑。
- 本轮子任务：
  - 只动 `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`，把“朝向跟瞬时速度抖动”改成“朝向跟稳定运动趋势”。
- 本轮实际改动：
  1. 新增朝向稳定参数：
     - `facingDirectionReversalMinHoldSeconds`
     - `facingVelocitySmoothing`
     - `facingAxisCommitBias`
     - `facingVelocityMinSpeed`
  2. `Update()` 不再直接拿 `ResolveVelocity()` 的原始结果决定朝向；
     - 改为先过 `ResolveStableFacingVelocity(...)`
     - 再用 `ResolveFacingDirection(...)` 只在轴向真正占优时才切朝向
  3. `GetSmoothedFacingDirection(...)` 新增“反向切头更长的最短保持时间”
     - 避免被 avoidance / blocked 窗口里的瞬时反向速度带着来回摆头
  4. `SetFacingDirection(...) / StopMotion() / ResetMotionObservation(...)`
     - 同步维护 `_smoothedFacingVelocity`
     - 保证脚本显式定头和停走时不会遗留旧的朝向噪声
- 关键判断：
  - 这刀只修表现层信号选择，不改 `NPCAutoRoamController` 的导航目的地、避让目标或桥/水/边界 contract。
  - 前面已经落下的性能止血补丁仍然保留，没有被这刀回退。
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs --count 20 --output-limit 5`
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
    - 原因：`CodexCodeGuard timed out`，且当前 `MCP` 端口 `127.0.0.1:8888` 拒绝连接，未能补到 fresh Unity 侧证据
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
    - 通过
- 当前阶段：
  - 代码修复已落地，代码层没有看到 owned red
  - 但 Unity/MCP live 证据这轮还没补到，所以不能直接宣称“偏头已完全终验关闭”
- 当前恢复点：
  - 下一步若用户允许占用 Unity，应只跑最小 live 验证，重点看 NPC 是否还会在避让/贴边时疯狂偏头或左右摆头。

## 2026-04-09｜继续主线：按“场景处理优先”重收 NPC 导航，不再继续误伤避让核心

- 当前主线目标：
  - 在不改 NPC 业务目标、不回退玩家 traversal 语义的前提下，把 `NPC` 当前“走路怪、避让发钝、静态阻挡漏接入”重新收回到性能优先且可扩展的版本。
- 这轮关键判断：
  - 问题大头不像“避让算法本身坏了”，更像：
    1. 场景里新加的静态 `tilemap/collider` 很容易漏接到导航阻挡链
    2. 前面为了止血，把物理驱动 NPC 在阻挡/避让态的重决策复用压得过宽，导致响应性变钝
    3. `NPCMotionController` 需要把动画朝向重新贴回移动矢量本身，而不是再做会漂开的重平滑
- 本轮真实改动：
  1. `Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs`
     - 新增“自动场景静态阻挡补全”字段：
       - `autoCollectSceneBlockingTilemaps = true`
       - `autoCollectSceneBlockingColliders = false`
       - `sceneBlockingIncludeKeywords = [wall, props, fence, rock, tree, border]`
       - `sceneBlockingExcludeKeywords = [base, grass, ground, background, bridge]`
     - `CollectBlockingSources(...)` 和 `BuildBoundsColliderFallback()` 不再只吃手填列表，改为“手填 + 自动收集”合并
     - 自动规则会：
       - 只收同场景、激活态、静态、非 trigger 的源
       - 排除动态导航单位
       - 排除显式 `walkableOverride` 源
       - `water` 仍可通过 `traversalSoftPassNameKeywords` 进 soft-pass，而不是被误判成硬挡
  2. `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs`
     - 把朝向收回成“移动矢量四扇区 + 轻去抖”
     - `Update()` 先 `ResolveFacingVelocity(...)`，再用 `GetDirectionFromVelocity(...)`
     - 方向分区口径：
       - `-45~45 = Right`
       - `45~135 = Up`
       - `-135~-45 = Down`
       - 其余 = `Left`
     - 这刀去掉了前一版会把朝向漂开的重平滑参数，只保留 `facingDirectionMinHoldSeconds`
  3. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `PHYSICS_HEAVY_DECISION_REUSE_SECONDS` 收窄到 `0.02f`
     - `CanReuseHeavyMoveDecisionThisFrame(...)` 在以下状态下禁止复用旧决策：
       - `hasPendingMoveCommandProgressCheck`
       - `blockedAdvanceFrames > 0`
       - `sharedAvoidanceBlockingFrames > 0`
       - `hasDynamicDetour`
     - 结果：平稳直走仍可复用保性能，但一旦进入阻挡/避让态，恢复实时重算
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name TraversalBlockManager2D --path Assets/YYY_Scripts/Service/Navigation --level standard --output-limit 5`
    - `status=clean`
    - `errors=0`
    - `warnings=0`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NPCMotionController --path Assets/YYY_Scripts/Controller/NPC --level standard --output-limit 5`
    - `status=warning`
    - `errors=0`
    - `warnings=1`
    - warning=`Consider using FixedUpdate() for Rigidbody operations`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NPCAutoRoamController --path Assets/YYY_Scripts/Controller/NPC --level standard --output-limit 5`
    - `status=warning`
    - `errors=0`
    - `warnings=1`
    - warning=`String concatenation in Update() can cause garbage collection issues`
  - `git diff --check -- Assets/YYY_Scripts/Service/Navigation/TraversalBlockManager2D.cs Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 通过
- 当前阶段：
  - 代码层闭环已完成，当前没有 owned red
  - 但这轮还没补 Unity live 终验，所以不能宣称“NPC 导航体验已完全收口”
- 当前恢复点：
  - 下一步若继续，只值得补一轮最小 live 验证：
    1. 新加静态 `tilemap/collider` 是否会自动进入导航阻挡
    2. `NPC` 朝向是否已重新跟紧移动矢量
    3. 阻挡/避让态是否比前一版更灵敏但不回到性能雪崩

## 2026-04-09｜继续主线：定位到“避让还在算，但结果被方向稳定器盖掉”，已热修回避让响应

- 当前主线目标：
  - 继续收 `NPC` 当前“几乎没有动态避让”的问题，不再靠猜，直接把真正压掉避让结果的那层去掉。
- 本轮关键判断：
  - `TryHandleSharedAvoidance(...)` 并不是没算出新的 `adjustedDirection`；
  - 真正的问题是 `TickMoving()` 后面那句 `StabilizeMoveDirection(...)` 会在 `sharedAvoidanceBlockingFrames > 0` 或 `hasDynamicDetour` 时，把新方向强行压回 `lastIssuedMoveDirection`。
  - 结果就是：避让求解有结果，但最终发给移动层的还是旧方向，看起来就像“没有避让”。
- 本轮真实改动：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
     - `TickMoving()` 里只在：
       - `sharedAvoidanceBlockingFrames <= 0`
       - 且 `!hasDynamicDetour`
       时才调用 `StabilizeMoveDirection(...)`
     - `ShouldHoldPreviousMoveDirection()` 也收窄成只剩：
       - `hasPendingMoveCommandProgressCheck`
       - `blockedAdvanceFrames > 0`
     - 不再把：
       - `sharedAvoidanceBlockingFrames > 0`
       - `hasDynamicDetour`
       当成“继续抱住旧方向不放”的理由
- 代码层验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py manage_script validate --name NPCAutoRoamController --path Assets/YYY_Scripts/Controller/NPC --level standard --output-limit 5`
    - `status=warning`
    - `errors=0`
    - `warnings=1`
    - warning=`String concatenation in Update() can cause garbage collection issues`
  - `git diff --check -- Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 通过
- 当前阶段：
  - 代码层已经把“避让结果被自己盖掉”这条真 bug 拆掉了
  - 但还没做 live 验证，所以不能直接宣称“动态避让已完全恢复”
- 当前恢复点：
  - 下一步若继续，最值得做的不是再改算法，而是最小 live 验证：
    1. 两个 NPC 正面对撞时是否开始明显让路
    2. detour / 贴边时是否重新会拐，而不是硬顶旧方向
