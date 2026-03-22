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
