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
  10. 上述纯资产尾巴又已继续收掉一刀：`a54e2342` `2026.04.13_Codex规则落地_11`
      - 含 `Assets/TextMesh Pro` 3 项；
      - 含 `Assets/ZZZ_999_Package/.../Farm Animals/*.png.meta` 5 项；
      - 同时把本线程治理记忆一起合法收口。
  11. 到第二刀后，`Assets` 非代码剩余已只剩 `3` 项，而且全是挂在代码目录上的 folder meta：
      - `Assets/Editor/Town.meta`
      - `Assets/YYY_Scripts/Story/Dialogue.meta`
      - `Assets/YYY_Scripts/UI/Save.meta`
      这 3 项不应再由治理线程在“非代码资产大包”里顺手吞并。
  12. 2026-04-13 继续顺手收掉的 safe tail 已再收一刀：
      - `Docx/大总结/Sunset_持续策划案/*.md`
      - `.kiro/specs/UI系统/*.md`
      - `.kiro/specs/屎山修复/*.md`
      - `.codex/threads/Sunset/UI/*.md`
      - `.codex/threads/Sunset/树石修复/*.md`
      - 两个孤立的只读线程记忆根
      - 以及 3 个代码目录 folder meta：
        - `Assets/Editor/Town.meta`
        - `Assets/YYY_Scripts/Story/Dialogue.meta`
        - `Assets/YYY_Scripts/UI/Save.meta`
  13. 同轮已把“其他线程怎样找回自己的剩余大头”正式落成治理产物，而不是继续靠聊天口述：
      - `2026-04-13_给典狱长_shared-root剩余大头拆分与owner定责矩阵_01.md`
      - `2026-04-13_shared-root剩余大头警匪分流批次_01.md`
      - `给 spring-day1 / NPC / UI / 树石修复` 四条专属 cleanup prompt
  14. 因此当前 shared-root 剩余的明确口径已经变成：
      - 安全可代收的 docs / folder-meta 尾巴已基本由治理线程收完；
      - 真正剩余的大头已经收窄成 `spring-day1 / NPC / UI / 树石修复 / Codex规则落地 own` 五组代码/配置/工具链尾账；
      - 后续不应再让业务线程靠猜文件名找 owner，而应直接按矩阵和专属 prompt 自收。
  15. 2026-04-13 当晚又继续把治理位 own 收口压深了一层：
      - `.kiro/xmind-pipeline` 当前不是“未验证源码堆”，而是一套完整工具链；
      - 初次 `npm run smoke` / `npm run test` 的真 blocker 已查实为 `topic-blueprints.ts` 仍引用旧标题 `### 7.4 当前阶段的治理与验收补记`；
      - 已就地修成新标题 `### 7.4 当前阶段的收口条件补记`，之后 `smoke` 与 `test` 均已通过。
  16. 同轮已把 shared-root 剩余代码面继续压成 `6` 组二级 owner 簇，而不是停在粗分组：
      - `spring-day1 = 48`
      - `NPC = 47`
      - `UI = 19`
      - `树石修复 = 8`
      - `Codex规则落地 own：Town/Home/Primary 基线链 = 76`
      - `Codex规则落地 own：工具链/配置 = 38`
      其中 `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset` 已明确改判到 `UI`，不再按“剩余纯资产”顺手吞。
  17. 上述二级拆分与第二轮分流入口已经正式落盘：
      - `2026-04-13_给典狱长_shared-root剩余大头二级拆分与owner定责矩阵_02.md`
      - `2026-04-13_shared-root剩余大头警匪分流批次_02.md`
      - `给 spring-day1 / NPC / UI / 树石修复` 的 `02` 版专属 prompt
      - `02` 版 prompt 已统一补上 `thread-state` 接线尾巴，不再沿用旧的粗口径 prompt。
  18. 上述 `.kiro/xmind-pipeline + v2 owner docs + 工作区/线程记忆` 已在治理模式下完成最小同步：
      - 实际提交为 `2a5f8236` `2026.04.13_Codex规则落地_14`
      - 本次只吞治理线程 own：
        - `.kiro/xmind-pipeline/*`
        - `Codex规则落地` 的 `02` 版矩阵/批次/prompt
        - 工作区与线程记忆
      - 没有吞 `Assets` 里的其他线程代码尾账，也没有碰 `ProjectSettings`
  19. 2026-04-13 深夜已按“先暂停进一步 shared-root 提交，转入打包态总复盘”的新口径做只读回看：
      - 当前 `git status --porcelain=v1 -uall` 已降到 `204`；
      - 当前活跃线程只有 `spring-day1` 与 `UI`，治理线程自己已回到 `PARKED`；
      - 剩余 dirty 已基本不是素材海，而是：
        - `Assets/YYY_Tests/Editor = 53`
        - `Assets/YYY_Scripts/Story = 27`
        - `Assets/YYY_Scripts/Service = 22`
        - `Assets/Editor/Story = 19`
        - `ProjectSettings = 2`
      - 这说明“打包前的大头”已经从素材/文档转成运行时代码、Editor/Test 尾账与少量配置面。
  20. 当前打包态准备口径也因此重新收束成三层：
      - `已站住的`：
        - 非代码资产大头已基本收完；
        - `.kiro/xmind-pipeline` 已验证可跑并已提交；
        - shared-root 剩余代码面已经有 `02` 版 owner 矩阵和专属 prompt，可随时重新启动分流。
      - `仍待真正处理的`：
        - `spring-day1` own Story/导演/测试尾账；
        - `UI` own 玩家面与字体材质尾账；
        - `Codex规则落地 own` 的 `Town/Home/Primary` 基线链；
        - `ProjectSettings/EditorBuildSettings.asset` 与 `QualitySettings.asset`。
      - `当前不应误判的`：
        - 现在不是“只差几张图/几个 meta 就能打包”的阶段；
        - 真正剩余风险已经转成代码、测试、配置和玩家面体验闭环。
  21. 由于用户本轮明确要求“先暂停进一步提交，先复盘打包态并随时准备”，治理线程本轮停在分析准备层：
      - 不继续吞新的 shared-root 提交；
      - 只保留最新打包态判断、待办口径和恢复点；
      - 下一轮如用户恢复执行，优先顺序应是：
        1. 先看 `spring-day1 / UI` 两条 active 线是否已停稳；
        2. 再决定是先补 packaged/live 验证，还是先继续收 `Codex规则落地 own` 的 `Town/Home/Primary` 基线链；
        3. `ProjectSettings` 继续最后单独处理，不混普通尾账。

## 本卷纪律
- 根卷只保留：
  - 工作区定位
  - 当前活跃阶段
  - 稳定结论
  - 恢复点
- 任何新的详细治理过程、批次执行、分卷动作或事故复盘，都不得再回灌本卷。

## 2026-04-13｜续记：切场掉落物丢失链路审计与打包前修复口径

- 用户目标：
  - 排查 `Primary -> Home -> Primary` 这类普通切场后，地面掉落物为什么直接消失，并给出最安全、最适合打包前的一刀修复方案。
- 当前主线：
  - 本轮仍是 `Codex规则落地` 的只读分析，不进入真实施工；线程状态维持 `PARKED`。
- 这轮实际做成了什么：
  1. 已把掉落物的创建 / 注册 / 正式存档 / 普通切场四段链路串清：
     - `ItemDropHelper` 只负责把物品生成到 `WorldItemPool`
     - `WorldItemPickup` 会注册进 `PersistentObjectRegistry`
     - `WorldItemPickup.Save/Load + DropDataDTO + DynamicObjectFactory.TryReconstructDrop` 说明系统确实支持“正式存档/读档恢复掉落物”
     - 但普通切场走的是 `SceneTransitionTrigger2D -> PersistentPlayerSceneBridge`
  2. 已确认普通切场桥当前只接：
     - 背包快照
     - 快捷栏选择
     - crowd/native resident runtime snapshot
     - 没有任何 scene-local 掉落物或一般 world object snapshot/restore
  3. 已确认 `SaveManager` 的 worldObjects 链只在 `SaveGame/LoadGame` 时生效，不会在普通 `LoadSceneMode.Single` 切场时自动介入。
- 关键判断：
  - 用户看到的“回 Primary 掉落物全没了”不是单点 bug，而是当前普通切场 continuity 只桥接玩家自身，不桥接 scene-local 世界对象的结构缺口。
  - 更进一步，从代码静态面看，这个缺口不只打在 `Drop`，理论上也会波及 `Tree/Stone` 这类同样依赖 `PersistentObjectRegistry + Save/Load` 的 scene-local world object；只是用户这次首先撞到的是掉落物。
- 打包前最安全修复口径：
  - 不建议走 `DontDestroyOnLoad` 掉落物常驻
  - 不建议把普通切场硬绑成一次磁盘 `SaveGame/LoadGame`
  - 推荐把补丁落在 `PersistentPlayerSceneBridge` 上，做“内存态 scene-local world snapshot bridge”
  - 但打包前的白名单不建议一口气放太大；最稳的主方案是先接住与当前玩家循环直接相关的 `Drop / Tree / Stone`，避免只补 `Drop` 导致“树重生但掉落还在”的复制漏洞
- 当前恢复点：
  - 如果下一轮进入真实施工，优先补一份场景内存快照桥设计：
    - 离场前 capture source scene 的白名单 world objects
    - 回场时按 scene key restore
    - 与正式磁盘存档语义分离
    - 新游戏 / fresh start 时清空这层 runtime snapshot

## 2026-04-13｜续记：已按“普通切场 continuity / 正式存档合同”拆成两份 prompt

- 用户目标：
  - 不把“掉落物跨场景消失”继续混成一锅，要求拆成两份可直接下发的 prompt：
    1. `Codex规则落地` 负责普通切场 continuity
    2. `存档系统` 负责正式存档入盘语义
- 当前主线：
  - 这轮仍是治理位 docs/prompt 收口，不进入真实施工。
- 这轮实际做成了什么：
  1. 已新增 `Codex规则落地` prompt：
     - `2026-04-13_给Codex规则落地_普通切场scene-local-world-continuity最小runtime桥prompt_01.md`
  2. 已新增 `存档系统` prompt：
     - `2026-04-13_给存档系统_离场场景world-state入盘语义审计与最小合同prompt_01.md`
  3. 已明确两线边界：
     - runtime 线只管 `PersistentPlayerSceneBridge` 的内存态 scene-local world snapshot bridge
     - save 线只管 off-scene world state 到正式存档的数据合同与最小入盘语义
- 关键判断：
  - “掉落物正式写入存档”这件事不是从零开始加字段，因为正式 `worldObjects` 主链已经存在；
  - 真正需要拆开的是：
    - 普通切场 continuity 缺口
    - off-scene runtime state 入盘语义
  - 如果不拆，这两条线最容易在 `PersistentPlayerSceneBridge / SaveManager` 的边界上互相覆盖。

## 2026-04-13｜续记：普通切场 scene-local world continuity 最小 runtime bridge 已落代码

- 用户目标：
  - 不改正式存档链，只在 `PersistentPlayerSceneBridge.cs` 上补普通切场的 `scene-local world continuity` 最小 runtime bridge，白名单先接 `Drop / Tree / Stone`。
- 当前主线：
  - 本轮已从只读进入真实施工；线程已补登记 `Begin-Slice`，当前仍在本刀收口阶段。
- 这轮实际做成了什么：
  1. 已在 `PersistentPlayerSceneBridge` 新增按 scene key 存放的 runtime snapshot 容器，单独承接 `Drop / Tree / Stone`，不与正式磁盘存档混用。
  2. 已把 `CaptureSceneRuntimeState()` 扩成：
     - 继续保留背包 / 快捷栏 / resident snapshot
     - 追加 capture 当前 scene 内的 `WorldItemPickup / TreeController / StoneController`
  3. 已把 `RebindScene()` 扩成：
     - 在 resident restore 之后，追加一段 deferred restore 协程
     - 通过 `scene handle + scene key` 只对当前回到的同场景应用 snapshot
  4. 已把 restore 语义收成最小白名单逻辑：
     - 当前 scene 中已存在且 snapshot 仍应存在的对象：按 snapshot `Load`
     - snapshot 标记为失活的 `Tree / Stone`：回场后继续保持失活
     - 当前 scene 中多出来但 snapshot 不存在的 `Drop`：回收/销毁，避免回场复制
     - 当前 scene 中缺失但 snapshot 里仍活着的对象：通过现有 `DynamicObjectFactory.TryReconstruct(...)` 重建后再 `Load`
  5. `ResetPersistentRuntimeForFreshStartInternal()` 已新增清空这层 runtime snapshot，并停止挂起的 restore coroutine，避免 fresh start 继承旧场景内存态。
- 关键判断：
  - 这刀补的是“普通切场 continuity”，不是“正式存档入盘”；`SaveManager / SaveDataDTOs / DynamicObjectFactory` 仍保持原语义未改。
  - 当前第一显性问题“离场后掉落物蒸发”在结构上已经被前移成更窄的 live 验证问题，不再是桥文件完全没接 world object。
- 验证状态：
  - `validate_script Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`：`owned_errors = 0`，assessment=`unity_validation_pending`
  - `py -3 ... status`：baseline `pass`，fresh console `0 error / 0 warning`
  - `py -3 ... errors --count 20 --output-limit 10`：`errors=0 warnings=0`
  - 根级 `git diff --check` 仍被 shared root 里既有的 `Town.unity / House prefab` 行尾空格阻断；本刀 scope 级 `git diff --check -- Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs` 已通过
- 当前恢复点：
  - live 侧仍待用户或后续 runtime probe 真测：`Drop / Tree / Stone` 在 `Primary/Town/Home` 普通切场下是否都按预期延续
  - `Ready-To-Sync` 已实跑，但被既有 own-root 残留阻断：
    - `Assets/YYY_Scripts/Service/Player` 根下还留有本线程历史 dirty：`PlayerAutoNavigator.cs / PlayerInteraction.cs / PlayerNpcChatSessionService.cs / PlayerNpcNearbyFeedbackService.cs / PlayerNpcRelationshipService.cs / PlayerThoughtBubblePresenter.cs / PlayerToolFeedbackService.cs`
    - 因此这轮当前不能合法只带 `PersistentPlayerSceneBridge.cs` 单独 sync；后续要么先清这批历史尾账，要么等用户明确允许扩大同根切片

## 2026-04-16｜安全 docs/memory-only checkpoint 已提交

- 用户目标：
  - 在 shared-root 仍很脏的前提下，继续找“还能安全提交”的内容，并且必须优先安全可靠、不能误吞 active 线程。
- 本轮主刀：
  - 只收 `PARKED` 线程的 docs/memory-only 小批，加上治理位自己早已成型但未提交的 prompt/memory。
- 本轮提交：
  - `dbbbe858`
  - `2026.04.16_Codex规则落地_01`
- 本次实际收掉的范围：
  1. `Codex规则落地`
     - 工作区 memory
     - 线程 memory
     - `2026-04-13` 两份普通切场 / 存档合同 prompt
  2. `NPC`
     - 工作区 memory
     - `2.2.0_自漫游与避让收口/memory.md`
     - 线程 `memory_0.md / memory_1.md`
     - `2026-04-15_给NPC_Day1解耦后resident状态合同与外部调用面收口prompt_01.md`
  3. `云朵与光影`
     - 工作区 memory
     - 线程 memory
  4. `存档系统`
     - 工作区 memory
     - `4.0.0_三场景持久化与门链收口/memory.md`
     - 线程 memory
  5. 一个已停车的独立线程记忆：
     - `.codex/threads/Sunset/019d4d18-bb5d-7a71-b621-5d1e2319d778/memory_0.md`
- 本轮为什么判它安全：
  - 白名单全部是 `.md` docs/memory；
  - 命中的线程和工作区当前都不是 `ACTIVE`；
  - 明确排除了当前仍在跑的：
    - `Day1-V3`
    - `UI`
    - `导航检查`
  - 也没有吞任何 `.cs / .unity / .prefab / .asset / ProjectSettings`。
- 过程验证：
  - `git diff --cached --check`：通过
  - `Ready-To-Sync`：通过
  - staged diff：15 个 docs 文件，无代码/资源
- 当前恢复点：
  - 现在 shared-root 剩余的主要仍是代码、场景、配置和 active 线程尾账；
  - 下一轮如果继续“安全提交”，优先级应改成：
    1. 继续只读审 active 线程是否已经停稳
    2. 再找下一批 `PARKED + docs-only + owner 清楚` 的小批
    3. 不要回到盲吞 `Assets / ProjectSettings`
