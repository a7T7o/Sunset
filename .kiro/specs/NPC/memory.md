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
