# Codex规则落地 - 线程活跃记忆

> 2026-04-10 起，旧线程母卷已归档到 [memory_7.md](D:/Unity/Unity_learning/Sunset/.codex/threads/Sunset/Codex规则落地/memory_7.md)。本卷只保留当前线程角色、当前主线和恢复点。

## 线程定位
- 线程名称：`Codex规则落地`
- 线程作用：治理总线的执行与审稿入口
- 线程边界：只负责治理动作、裁定、续工 prompt、规则修订，不吞业务运行时细节

## 当前主线
- 维持 `Codex规则落地` 作为治理层的单一事实入口
- 把 memory 治理、入口修正和阶段路由继续收紧

## 当前稳定结论
- 任何新的治理动作都应优先落到对应阶段目录，而不是回灌线程根卷
- 线程根卷只保留：
  - 当前角色
  - 当前主线
  - 当前阶段判断
  - 恢复点

## 当前恢复点
- 后续若继续治理线，先看：
  - [D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory.md)
  - [D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/26_memory主卷退场与活跃卷重建/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/26_memory主卷退场与活跃卷重建/memory.md)
- 查旧治理现场、旧裁定或历史批次时，再回看 `memory_1..7.md`

## 2026-04-12｜补记：shared-root 百万级脏面进入打包前紧急清账审计

- 用户目标：
  - 在不回退、不伤正常功能、且项目已接近打包的前提下，先查清当前 `Sunset` 为什么又堆到近百万级未提交内容，以及怎样才能安全分批提交干净。
- 当前主线：
  - 这是 `Codex规则落地` 的治理只读清扫，不进入真实施工，也不对业务文件做盲修或盲删。
- 本轮已完成：
  1. 已确认当前所有业务线程都处于 `PARKED`，没有 active writer 正在抢 shared root。
  2. 已确认当前工作树规模：
     - `git status --porcelain` 共 `580` 条
     - 其中 `Modified=226`
     - `Deleted=10`
     - `Untracked=344`
     - `git diff --stat` 涉及 `236` 个 tracked 文件、约 `151545` 行新增和 `111689` 行删除
  3. 已确认当前 hot / mixed 目标仍然是脏的：
     - [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity)
     - [Town.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
     - [GameInputManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs)
     - [StaticObjectOrderAutoCalibrator.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/StaticObjectOrderAutoCalibrator.cs)
  4. 已确认当前不能直接 claim 运行时切片可安全提交：
     - `py -3 scripts/sunset_mcp.py status` = `baseline fail`
     - 原因是 `listener_missing`
     - `errors` 也拿不到 fresh console
  5. 已确认 `git diff --check` 当前会被大量 trailing whitespace 阻断，尤其集中在：
     - `Home.unity`
     - `Town.unity`
     - 部分 memory / asset 文本
  6. 已分出一批“零功能风险垃圾”候选：
     - [Save](/D:/Unity/Unity_learning/Sunset/Save)
     - [.codex/archives](/D:/Unity/Unity_learning/Sunset/.codex/archives)
     - [.codex/sunset-mcp-trace.log](/D:/Unity/Unity_learning/Sunset/.codex/sunset-mcp-trace.log)
     - [.codex/tmp_inventory_select_scan.txt](/D:/Unity/Unity_learning/Sunset/.codex/tmp_inventory_select_scan.txt)
     - [Assets/Screenshots](/D:/Unity/Unity_learning/Sunset/Assets/Screenshots)
     - [Assets/Sprites/Temp000](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/Temp000)
     其中 `Assets/Screenshots` 与 `Assets/Sprites/Temp000` 已额外做文本引用核实，当前只被各自 `.meta` 自身命中，未发现被 scene/prefab/script 直接引用。
- 关键判断：
  - 当前最安全的收口方式不是“先把所有脏东西一起提一把”，而是先冻结运行时切片，只做：
    1. `零功能风险垃圾` 清扫
    2. `docs/memory/prompt` 单独整理
    3. 等 MCP/Unity fresh 验证恢复后，再收运行时和 hot scene 批次
- 当前恢复点：
  - 如果继续这条治理线，下一步优先输出一份“紧急收口批次清单”：
    - 哪些现在就能删/移出/不提交
    - 哪些可以 docs-only 提交
    - 哪些必须等 fresh runtime 验证恢复后再提交
  - 本轮补充核实结果：
    - [Assets/Screenshots](/D:/Unity/Unity_learning/Sunset/Assets/Screenshots) 当前未发现被 scene/prefab/script 直接引用，可视作取证副产物候选；
    - [Assets/Sprites/Temp000](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/Temp000) 当前未发现被 scene/prefab/script 直接引用，可视作 UI 草图副产物候选；
    - [Assets/__CodexEditModeScenes](/D:/Unity/Unity_learning/Sunset/Assets/__CodexEditModeScenes) 与测试代码里的临时场景输出约定一致，可视作 EditMode scratch 候选；
    - [Assets/Prefabs/场景物品](/D:/Unity/Unity_learning/Sunset/Assets/Prefabs/场景物品)、[Assets/Sprites/Generated](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/Generated)、[Assets/100_Anim/FarmAnimals](/D:/Unity/Unity_learning/Sunset/Assets/100_Anim/FarmAnimals)、[Assets/222_Prefabs/FarmAnimals](/D:/Unity/Unity_learning/Sunset/Assets/222_Prefabs/FarmAnimals) 不能按垃圾处理，应先视为真实功能资产。

## 2026-04-12｜续记：safe-garbage-cleanup-20260412 实际执行结果

- 当前主线：
  - 在不伤正常功能、不让其他线程失忆的前提下，只收 shared-root 里最安全的一小批生成垃圾，并尽量形成单独可提交切片。
- 这轮实际做成了什么：
  1. 重新核对了 `Show-Active-Ownership.ps1`，确认当前仍有 `spring-day1` 与 `UI` 处于 `ACTIVE`，因此继续保持“只动 shared-root 生成物，不碰活跃业务面”的边界。
  2. 复核了治理 memory 后，把候选垃圾再次收窄：
     - `Assets/Screenshots/` 虽不是正式运行时资产，但治理记忆里已有“未结案前不要直接删”的结论，本轮保留；
     - `.codex/archives/` 里实际存在 `navigation/nav-unification-anchor-20260406-01.bundle`，更像人工归档，本轮保留；
     - `Assets/Sprites/Temp000/` 仅判为“未被直接引用的草图候选”，本轮保留不删。
  3. 已修改 `.gitignore`，新增忽略：
     - `.codex/archives/`
     - `.codex/sunset-mcp-trace.log`
     - `.codex/tmp_inventory_select_scan.txt`
     - `Save/`
     - `Assets/__CodexEditModeScenes/`
     - `Assets/__CodexEditModeScenes.meta`
  4. 已实际删除当前确定安全的生成垃圾：
     - [Save](/D:/Unity/Unity_learning/Sunset/Save)
     - [.codex/sunset-mcp-trace.log](/D:/Unity/Unity_learning/Sunset/.codex/sunset-mcp-trace.log)
     - [.codex/tmp_inventory_select_scan.txt](/D:/Unity/Unity_learning/Sunset/.codex/tmp_inventory_select_scan.txt)
     - [Assets/__CodexEditModeScenes](/D:/Unity/Unity_learning/Sunset/Assets/__CodexEditModeScenes)
     - [Assets/__CodexEditModeScenes.meta](/D:/Unity/Unity_learning/Sunset/Assets/__CodexEditModeScenes.meta)
  5. 已复查结果：
     - 上述已清路径都已从工作树消失；
     - 当前这批剩余未决只剩 [Assets/Screenshots](/D:/Unity/Unity_learning/Sunset/Assets/Screenshots) 与 [Assets/Sprites/Temp000](/D:/Unity/Unity_learning/Sunset/Assets/Sprites/Temp000) 两组未跟踪目录；
     - `git diff --check -- .gitignore` 已通过。
- 当前阶段：
  - 这条治理切片已从“只读盘点”进入“最小安全清扫 + 可提交收口”阶段。
- 当前恢复点：
  - 下一步只需要做两件事：
    1. 跑 `Ready-To-Sync`；
    2. 仅对白名单 `(.gitignore + own memory)` 做最小提交。

- 本轮最终结果：
  - `Ready-To-Sync` 首次被挡，不是因为这刀不安全，而是最初 slice 把 `Assets/.codex/Save` 都带进了 own roots，导致整个 shared root 残留被一并算进 preflight。
  - 已先 `Park-Slice` 旧切片，再改成仅包含 `.gitignore` 的新切片 `gitignore-only-safe-cleanup-20260412`。
  - 新切片已 `Ready-To-Sync -> sync -> push -> Park-Slice` 全走完。
  - 实际提交为：`1a138a92` `2026.04.12_Codex规则落地_01`
  - 这笔提交只包含 [.gitignore](/D:/Unity/Unity_learning/Sunset/.gitignore)，没有吞并本线程 own memory、其他线程 memory、scene 或业务代码。
- 当前真实恢复点：
  - 如果后续还要继续 shared-root 清账，下一刀不要再把 `Assets/.codex/Save` 整根挂进白名单切片；要继续沿“先只读分层，再切成最小可 sync 的单路径/单根切片”推进。

## 2026-04-12｜续记：scene-folder-safe-checkpoint-20260412 实际收口结果

- 用户目标：
  - 用最安全的方式先处理 near-million dirty 里最大的 scene 大头，重点是不误吞其他线程现场。
- 当前主线：
  - 这是 `Codex规则落地` 的治理真实施工，目标是把 `Assets/000_Scenes` 作为单独 checkpoint 收掉，而不是继续分析。
- 这轮实际做成了什么：
  1. 重新核对 `Show-Active-Ownership.ps1`，确认当前活跃线程只剩 `spring-day1`、`UI` 与本线程；scene 这刀仍由 `Codex规则落地` 持有。
  2. 重新核对 [Assets/000_Scenes](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes) 当前 dirty，确认本轮实际要收的就是 7 项：
     - [Artist.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Artist.unity)
     - [Home.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity)
     - [Primary.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity)
     - [SampleScene.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/SampleScene.unity)
     - [SampleScene.unity.meta](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/SampleScene.unity.meta)
     - [Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
     - [矿洞口.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/矿洞口.unity)
  3. 用稳定 launcher 重新跑白名单 preflight，确认：
     - `是否允许按当前模式继续 = True`
     - `own roots = Assets/000_Scenes`
     - `own roots remaining dirty 数量 = 0`
  4. 真正踩到了一个规则层小 blocker：
     - `Ready-To-Sync` 首次失败，不是因为 scene 内容不安全，而是旧 slice 只把 5 个现存 scene 放进了 `TargetPaths`，漏了两个删除项 `SampleScene.unity(.meta)`；
     - 结果被 own-roots remaining dirty 规则准确拦下。
  5. 已按规则修正：
     - 先 `Park-Slice` 旧 slice；
     - 再重开同名 slice，保持 5 个现存 scene 作为 `TargetPaths`；
     - 同时把 7 个实际提交项全部写进 `ExpectedSyncPaths`；
     - 之后 `Ready-To-Sync` 正式通过。
  6. 已完成正式白名单 sync：
     - 提交为 `3ee66fb4` `2026.04.12_Codex规则落地_02`
     - `git log -1 --stat` 显示只收了上述 7 个 scene 项
     - `git status --short -- Assets/000_Scenes` 已 clean
  7. 已在提交后立刻 `Park-Slice`，释放 [Primary.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity)、[Town.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)、[Home.unity](D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 等 scene 写窗，不继续占用 A 类锁。
- 关键判断：
  - 这轮最值钱的不是“终于提了一大包”，而是证明 shared-root 最大头可以被安全切成一个独立 scene checkpoint，而且保护网会把遗漏的删除项先拦下来，不会让半截 slice 混过去。
- 当前恢复点：
  - 如果后续继续 shared-root 清账，优先找下一个最大的同根脏面做独立 checkpoint，不要重新把 `Assets/000_Scenes` 和别的根混成一锅。

## 2026-04-12｜续记：xmind-pipeline-node-modules-safe-cleanup-20260412 实际收口结果

- 用户目标：
  - 既然 Codex/VS Code 角标仍然爆量，就立刻从真实最大头继续往下砍，不要停在“scene 已经提交”这句上。
- 当前主线：
  - 这是 `Codex规则落地` 的 shared-root 紧急降噪切片，目标是先把 `.kiro/xmind-pipeline/node_modules` 这坨未跟踪依赖海清掉。
- 这轮实际做成了什么：
  1. 重新对齐了口径，确认 VS Code 里看到的 `2617` 不是错觉，而是 `git status --porcelain=v1 -uall` 的真实总数。
  2. 已确认当前爆量的第一真元凶不是刚刚那笔 scene，而是 `.kiro/xmind-pipeline`：
     - 单根 `1861` 条 untracked；
     - 其中 `node_modules` 独占 `1825` 条。
  3. 已确认 `.kiro/xmind-pipeline` 当前没有任何 tracked 文件，且目录结构清晰分成：
     - `src / tests / fixtures / package*.json / README / sunset-master-graph.json`
     - `node_modules`
     - `output`
     这使得“只清依赖、不动源码与输出”成为安全可行的一刀。
  4. 已修改 [.gitignore](D:/Unity/Unity_learning/Sunset/.gitignore)，新增：
     - `.kiro/xmind-pipeline/node_modules/`
  5. 已实际删除本地 [node_modules](D:/Unity/Unity_learning/Sunset/.kiro/xmind-pipeline/node_modules)。
  6. 已完成最小提交：
     - `19e31c4c` `2026.04.12_Codex规则落地_03`
     - 只包含 `.gitignore`
  7. 已复查结果：
     - `git status --porcelain=v1 -uall` 从 `2617` 直接降到 `792`
     - 当前 `Untracked` 从 `2403` 直接降到 `578`
     - `.kiro/xmind-pipeline` 只剩 `36` 条源码/输出/测试文件，不再是第一爆量源
     - 当前新的 untracked 前几名已变成：`.kiro/specs(108)`、`Assets/100_Anim(86)`、`Assets/Editor(59)`、`Assets/Screenshots(58)`、`Assets/Sprites(54)`
- 关键判断：
  - 这轮最值钱的不是“又提了一个 commit”，而是把真正把 Codex 撑爆的第一噪音源先干掉了；这证明接下来的 shared-root 降噪应该继续优先从 untracked 大根动手，而不是再盯 scene。
- 当前恢复点：
  - 如果继续清账，下一刀先审 `.kiro/specs` 这 `108` 条未跟踪正文；只有在确认它们可归类后，才考虑继续碰 `Assets/Screenshots / Assets/Sprites / Assets/Editor`。

## 2026-04-12｜续记：parked-doc-roots 连续三刀收口结果

- 用户目标：
  - 继续把 shared-root 爆量往下收，但必须保持“最安全方式”，不要误吞 active 线程现场。
- 当前主线：
  - 这是 `Codex规则落地` 的 docs-only 治理真实施工；目标是把已经 `PARKED` 的工作区正文与线程记忆海继续压平，把剩余爆量重新收窄到真正的 `Assets` 业务面。
- 这轮实际做成了什么：
  1. 先按 `core.quotepath=false` 重新精确分层，确认 `.kiro/specs` 的最大头是 `900_开篇(40)`、`NPC(26)`、`存档系统(14)`、`屎山修复(12)`、`UI系统(10)`、`Codex规则落地(9)`、`项目文档总览(9)`。
  2. 结合 `Show-Active-Ownership.ps1`，把 `spring-day1 / UI` 活跃线排除在外，只对白名单 parked 根继续收口。
  3. 第一刀提交 `e9026d7e` `2026.04.12_Codex规则落地_04`：
     - 白名单包含 `NPC / 存档系统 / 屎山修复 / 项目文档总览 / Codex规则落地`
     - 同时带上对应 parked 线程记忆
     - 实际收掉 `83` 条 docs-only 项，提交后这些根已 clean。
  4. 第二刀提交 `7efc47de` `2026.04.12_Codex规则落地_05`：
     - 白名单包含 `900_开篇` 与 `spring-day1 / spring-day1V2` parked 线程记忆
     - 实际收掉 `43` 条 docs-only 项，提交后该根已 clean。
  5. 第三刀提交 `5665560d` `2026.04.12_Codex规则落地_06`：
     - 白名单包含 `000_Gemini / Z_光影系统 / 云朵遮挡系统 / 农田系统 / 箱子系统`
     - 同时带上多条 parked 线程记忆尾巴
     - 中途只补了一个最小止血：去掉 `农田系统` 记忆里两处行尾空格，让 `git diff --check` 重新通过
     - 实际收掉 `20` 条 docs-only 项。
  6. 三刀收完后重新盘点真实总量：
     - `git status --porcelain=v1 -uall` 已从 `792` 继续降到 `644`
     - 当前 `Assets = 583`
     - 当前 `.kiro = 46`
     - 当前 `.codex = 5`
     - 说明 docs 海已经基本压平，剩余主战场已回到 `Assets`。
- 关键判断：
  - 现在再继续“为了收而收”去碰 `.kiro/.codex` 已经不值钱，因为剩余那点 docs 基本都挂在 `UI` 活跃线；真正的大头已经是 `Assets/100_Anim/FarmAnimals / Assets/Editor / Assets/Sprites / Assets/Screenshots / Assets/YYY_Tests` 这些真实业务或证据资产。
- 当前恢复点：
  - 如果继续 shared-root 清账，下一步不要再盯 docs；应改成对 `Assets` 做只读四分层：
    1. `证据副产物`
    2. `草图/临时图`
    3. `真实功能资产`
    4. `active 线程输出`
  - 只有分清这四类后，才适合决定下一刀是继续做 safe checkpoint、局部忽略，还是必须停给用户拍板。
