# Codex规则落地 - 活跃入口记忆

> 2026-04-10 起，旧超长母卷已归档到 [memory_7.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_7.md)。本卷只保留当前治理定位、活跃阶段入口和恢复点，不再承接长流水。

## 当前定位
- 本工作区只负责 `Sunset` 的安全治理、shared-root 事故恢复、live 规则入口和历史审计。
- 不再充当业务执行总线、运行时调度总线或所有线程回执的总堆场。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `25_vibecoding场景规范与main收口`
  - `26_memory主卷退场与活跃卷重建`

## 历史卷索引
- [memory_index.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_index.md)
- [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_0.md)
- [memory_1.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_1.md)
- [memory_2.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_2.md)
- [memory_3.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_3.md)
- [memory_4.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_4.md)
- [memory_5.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_5.md)
- [memory_6.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_6.md)
- [memory_7.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/memory_7.md)

## 当前稳定结论
- 当前唯一 live 规范层级始终以：用户裁定 -> `AGENTS.md` -> 当前规范快照 -> 命中的治理正文 为准。
- `CLI first, direct MCP last-resort`、`thread-state`、`用户可读汇报`、`No-Red 证据卡 v2` 仍是当前有效治理基线。
- 后续若出现新的 shared-root、分发、线程续工、停发裁定问题，应优先进入对应阶段目录，不再把细节倒回根主卷。
- 当 shared root 进入“大量 parked 线程 + 数百 dirty/untracked + 热文件仍脏”的收口期时，第一动作必须先做只读分层盘点：区分 `零功能风险垃圾`、`docs/memory`、`Editor/Test only`、`运行时/Scene/ProjectSettings`，不能直接为了清 status 盲删或盲提交通用大包。
- 如果当前 `sunset_mcp.py status` 落 `listener_missing`，就不能对运行时切片 claim `可安全提交/可打包`；此时最多只允许先推进不影响正常功能的垃圾清扫与分批提交规划。
- 2026-04-12 已把 shared-root 紧急清扫再收窄到最保守的一刀：
  - 只提交 `.gitignore` 对 `Save/`、`.codex` 临时日志、`Assets/__CodexEditModeScenes*` 和本地 `.codex/archives/` 的忽略；
  - 只删除当前运行生成的 `Save/`、两份 `.codex` 临时日志、`Assets/__CodexEditModeScenes*`；
  - `Assets/Screenshots/`、`Assets/Sprites/Temp000/`、`.codex/archives/` 当前一律不删，避免误伤验收证据、草图或人工归档。
  - 上述最小 safe cleanup 已单独提交为 `1a138a92`，没有吞并其他线程的 memory、scene 或业务改动。
- 2026-04-12 已把当前 shared-root 最大头 `Assets/000_Scenes` 安全收成独立 checkpoint：
  - 实际提交为 `3ee66fb4`；
  - 白名单只包含 `Artist.unity / Home.unity / Primary.unity / SampleScene.unity / SampleScene.unity.meta / Town.unity / 矿洞口.unity` 这 7 项；
  - 证明当前 scene 大头可以按“5 个现存 scene target + 2 个删除项 expected sync paths”独立收口，不必为了解决大头而顺手吞并其他业务根、memory 或 docs。
- `Ready-To-Sync` 对带删除项的 scene 批次会额外依赖 `expected_sync_paths` 是否包含删除目标；只把现存文件放进 `TargetPaths` 还不够，像 `SampleScene.unity(.meta)` 这种已删除项必须一并写进 `ExpectedSyncPaths`，否则会被 own-roots remaining dirty 规则拦下。
- 2026-04-12 已确认“Codex 仍爆卡”的第一真元凶不是 scene，而是 `.kiro/xmind-pipeline/node_modules` 这坨未跟踪依赖海：
  - `git status --porcelain=v1 -uall` 原先是 `2617` 条，其中 `.kiro/xmind-pipeline` 单根就占 `1861` 条，且 `node_modules` 占 `1825` 条；
  - 已把 `.kiro/xmind-pipeline/node_modules/` 加入 `.gitignore` 并清掉本地目录，最小提交为 `19e31c4c`；
  - 清完后总量已直接降到 `792` 条，说明这坨依赖海才是当前“显示没变、Codex 卡爆”的最大噪音源。
- 当前 remaining untracked 的新顺序已变成：
  - `.kiro/specs` = `108`
  - `Assets/100_Anim` = `86`
  - `Assets/Editor` = `59`
  - `Assets/Screenshots` = `58`
  - `Assets/Sprites` = `54`
  - `.kiro/xmind-pipeline` 仅剩 `36` 条源码/输出/测试文件，不再是第一大头。
- 2026-04-12 已继续把 `.kiro/specs + .codex/threads` 这层 parked 文档海分四刀安全压平：
  - `e9026d7e` `2026.04.12_Codex规则落地_04`：白名单提交 `NPC / 存档系统 / 屎山修复 / 项目文档总览 / Codex规则落地` 及对应 parked 线程记忆，共 `83` 条 docs-only 项；
  - `7efc47de` `2026.04.12_Codex规则落地_05`：白名单提交 `900_开篇` 与 `spring-day1 / spring-day1V2` parked 线程记忆，共 `43` 条 docs-only 项；
  - `5665560d` `2026.04.12_Codex规则落地_06`：白名单提交剩余 parked docs 尾巴（`000_Gemini / Z_光影系统 / 云朵遮挡系统 / 农田系统 / 箱子系统` 及多条 parked 线程记忆），共 `20` 条 docs-only 项；
  - `54c9eae3` `2026.04.12_Codex规则落地_08`：白名单提交 `UI系统` 与 `.codex/threads/Sunset/UI`，共 `13` 条 docs-only 项。
- 上述四刀之后，shared-root 总量已从 `792` 继续降到 `631`；当前 `.kiro + .codex/threads` 基本只剩：
  - 两个未进入当前 `thread-state` 池的孤立线程记忆根；
  - 真正的剩余大头已明确回到 `Assets`，而不是治理 docs 海。
- 当前 `Assets` 只读分层的新头部已经钉死：
  - `Assets/100_Anim/FarmAnimals` = `85`
  - `Assets/Editor` = `78`
  - `Assets/Sprites` = `58`
  - `Assets/Screenshots` = `58`
  - `Assets/YYY_Tests/Editor` = `54`
  - 这些根里目前没有像 `node_modules` 那样显而易见、可直接闭眼清掉的零功能风险大头；继续推进前必须先做“证据副产物 / 草图 / 真实功能资产 / 活跃线程输出”四分层，而不能盲删或盲提交通用大包。

## 当前恢复点
- 继续做 memory 治理、治理入口收口或规范修订时，优先进入：
  - [26_memory主卷退场与活跃卷重建](D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/26_memory主卷退场与活跃卷重建/memory.md)
- 如果只是查旧治理事实、批次分发、典狱长裁定或历史事故过程，直接查 `memory_0..7.md` 与对应阶段目录。
- 如果后续继续 shared-root 清账，优先沿“先拿最大同根大头做独立 checkpoint”推进；`Assets/000_Scenes` 这一根已经单独收口，不要再把 scene 与 docs/memory 混成通用大包。
- 当前下一刀若继续以“让 Codex/VS Code 不再爆卡”为优先目标，主战场已经从 `.kiro/specs` 转成 `Assets`；正确顺序应是：
  1. 先只读细分 `Assets/Screenshots / Assets/Sprites/Temp000 / Assets/Sprites/Generated / Assets/100_Anim/FarmAnimals / Assets/Editor / Assets/YYY_Tests/Editor`
  2. 明确哪些是证据副产物、哪些是草图、哪些是 parked 线程的真实成果、哪些仍受 active 线程影响
  3. 只对白名单安全根继续做最小 checkpoint 或忽略策略
  4. 2026-04-13 的第一轮复勘已经给出更硬结论：
     - `Assets/Screenshots` 代表样本 guid 未搜到外部引用，当前更像验收/排障截图海；
     - `Assets/Sprites/Temp000` 代表样本 guid 未搜到外部引用，当前更像 UI 草图池；
     - `Assets/Sprites/Generated` 代表样本 `床.png` 已被 `Assets/Prefabs/场景物品/床_0.prefab` 真引用，不能再按临时图处理；
     - `Assets/100_Anim/FarmAnimals` 代表样本 controller 已被 `Assets/222_Prefabs/FarmAnimals/Baby Chicken Yellow.prefab` 真引用，属于完整功能资产链；
     - `Assets/Editor / Assets/YYY_Tests/Editor` 已直接混着 `spring-day1 / NPC / TownHome / UI / 渲染验证` 的 active 或近活跃输出。
  5. 因此当前最接近“可单独再谈安全白名单”的只剩 `Assets/Screenshots` 与 `Assets/Sprites/Temp000`；其余 `Generated / FarmAnimals / Editor / Tests / YYY_Scripts / Resources / 111_Data / 222_Prefabs` 已连到 active 业务链，治理线程不应再借 cleanup 名义硬吞。
  6. 2026-04-13 用户已明确改判：`Assets/222_Prefabs / Assets/Resources / Assets/111_Data / Assets/100_Anim / Assets/Sprites / Assets/Screenshots / Assets/Prefabs / Assets/000_Scenes/Primary.unity` 这批非代码资产允许直接提交，不再按“等待 owner 各自回收”处理。
  7. 基于这次显式授权，治理线程已改走“手工白名单 staging”而不是继续卡在 `git-safe-sync` 的 own-root 阻断上：
     - `git-safe-sync preflight` 仍会因为 `Assets` 根下另有代码 dirty 而阻断；
     - 但在用户明确批准“只提非代码资产”的前提下，手工仅 stage 资产路径、显式排除 `.cs / .asmdef / *.cs.meta / *.asmdef.meta` 是可接受的最小 override。
  8. 上述资产大包已提交为 `8a3ad181` `2026.04.13_Codex规则落地_10`：
     - 实际提交 `329` 个非代码资产文件；
     - 含 `Primary.unity`、`FarmAnimals` 动画与 prefab 链、`Generated` 精灵及 `Assets/Prefabs/场景物品`、`SpringDay1` 相关 `Resources/111_Data/222_Prefabs` 资产、以及 `Screenshots`。
  9. 第一刀之后，`Assets` 非代码剩余已降到 `11` 项，其中只有：
     - `Assets/TextMesh Pro/...` 3 项；
     - `Assets/ZZZ_999_Package/.../Farm Animals/*.png.meta` 5 项；
     - 以及 3 个挂在代码目录上的 folder meta：`Assets/Editor/Town.meta`、`Assets/YYY_Scripts/Story/Dialogue.meta`、`Assets/YYY_Scripts/UI/Save.meta`。

## 本卷纪律
- 根卷只保留：
  - 工作区定位
  - 当前活跃阶段
  - 稳定结论
  - 恢复点
- 任何新的详细治理过程、批次执行、分卷动作或事故复盘，都不得再回灌本卷。
