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

## 2026-04-23｜只读总审：当前未提交内容已从“数量恐慌”收束成明确线程尾账图

- 用户目标：
  - 项目已明确进入“不能回退、不能删改成果”的收口期；这轮要求治理位只读彻查当前全部未提交内容，按线程与工作区重新分清，不做任何业务修改。
- 当前主线：
  - 这轮是 `Codex规则落地` 的治理只读总审；不进入真实施工，线程状态保持 `PARKED`。
- 这轮实际做成了什么：
  1. 重新核了 `AGENTS.md`、`Codex规则落地/memory.md`、线程记忆、`线程分支对照表.md` 和 `Show-Active-Ownership.ps1`，把“线程状态”和“git 脏改状态”重新对齐。
  2. 重新核实当前 shared-root 真实总量：
     - `git status --porcelain=v1 -uall = 447`
     - `modified = 172`
     - `deleted = 2`
     - `untracked = 273`
  3. 已确认一个比总数更重要的事实：
     - `thread-state` 里当前只有 `导航检查` 仍是 `ACTIVE`
     - 其余主线程基本都显示 `PARKED`
     - 但工作树里仍躺着大量业务改动、memory 和 prompt
     - 这说明当前第一问题不是“谁还在写”，而是“谁已经停了但还没把自己的尾账合法提交/收口”
  4. 已把当前盘面重新压成 4 类，而不是再按总数看恐慌：
     - `spring-day1`：导演/runtime/Story/UI/tests 一整簇还在树上
     - `NPC`：resident/roam/profile/editor/tool/runtime 合同一整簇还在树上
     - `UI`：玩家面、Prompt、背包箱子、字体材质和运行时 UI 仍有明显尾账
     - `Codex规则落地/Town-Home-Primary`：4 个 scene + ProjectSettings + Town/Home editor 工具仍未收完
     - 另有 `农田交互修复V3`、`存档系统`、`导航检查` 等明确 own 面
     - 再加一大坨与业务无关的 `tmp/.codex/backups/只读审计子线程`
  5. 已确认当前最该先清的不是业务代码，而是“噪音和真实尾账要分开看”：
     - `tmp/`、`.codex/backups/`、多条只读审计子线程 memory 现在大量抬高数字
     - 但真正决定能不能安全打包/归仓的，仍是 scene、ProjectSettings、运行时代码和 active/parked 线程的 own 文件
- 当前关键判断：
  - 现在已经不是“继续让治理位代收一大包”最安全的阶段。
  - 最真实的正确动作应改成：
    1. 先按线程 owner 收
    2. 每条线程只提自己的 own 批次
    3. shared/mixed 面最后再由治理位做最小整合
  - 否则最容易出的问题不是“漏提一点”，而是“治理位为了清数字把别人的运行时现场吞掉”。
- 当前恢复点：
  - 如果下一轮继续做 shared-root 收口，优先顺序应固定为：
    1. 先把 `tmp / .codex/backups / 只读审计子线程` 这类噪音和业务尾账剥开
    2. 再按 `spring-day1 / NPC / UI / 农田交互修复V3 / 存档系统 / 导航检查 / Codex规则落地(Town-Home-Primary)` 七组去做 owner 自收
    3. `Primary.unity / Town.unity / Home.unity / ProjectSettings / GameInputManager.cs` 继续按 shared/mixed 高危面单独看，不混常规批次

## 2026-04-23｜续记：第一波“完整保本上传”分发 prompt 已落地

- 用户目标：
  - 用户认可第一波转发线程名单，要求治理位直接把 prompt 写出来；重点不是继续清数字，而是确保“所有当前本地真实成果都能在不回退、不删改的前提下，被正确上传”。
- 当前主线：
  - 本轮已从只读盘面切到治理施工，但仍然只做 docs/prompt 分发层；不代替业务线程吞并它们的代码/scene/asset。
- 这轮实际做成了什么：
  1. 已在 `Codex规则落地` 根层新增一份本轮批次入口：
     - `2026-04-23_shared-root完整保本上传分发批次_01.md`
  2. 已新增 6 条专属 prompt：
     - `2026-04-23_给spring-day1_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_给NPC_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_给UI_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_给农田交互修复V3_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_给存档系统_shared-root完整保本上传与own尾账归仓prompt_01.md`
     - `2026-04-23_给导航检查_shared-root完整保本上传与own尾账归仓prompt_01.md`
  3. 本轮 prompt 已统一钉死 3 个核心口径：
     - 这是 `上传轮`，不是 `继续开发轮`
     - 当前成果按“原样归仓”处理，不允许为了清 status 擅自回退、删改、重做
     - shared/mixed 热面继续留到后续治理位最后整合，不许业务线程借 cleanup 名义硬吞
  4. 已按当前盘面，把每条线程各自当前最明显的 dirty 簇写进 prompt，避免它们再靠猜 owner。
- 当前关键判断：
  - 这波最值钱的不是“又发了一轮 prompt”，而是把接下来的 shared-root 收口从“治理位代收”改成了“各线程先保本上传 own，治理位最后收 shared/mixed”。
- 当前恢复点：
  - 下一轮先收各线程回执，看：
    1. 哪些 clearly-own 已经 push
    2. 哪些 exact blocker 仍卡在 shared/mixed
    3. 哪些临时证据/备份/截图需要治理位最后统一处理

## 2026-04-23｜回执复核完成：第二波已切成“历史小批次上传”

- 用户目标：
  - 用户明确纠偏：后续上传不能再按“当前 clearly-own 全量先交”，而应改成“按过去每一小刀开发历史慢慢还原批次上传”。
- 本轮主线：
  - 治理位继续只做回执复核与 prompt 分发，不代替业务线程继续开发。
- 本轮实际做成了什么：
  1. 已核实 5 条线程回执与仓库现场基本一致：
     - `spring-day1`
     - `UI`
     - `NPC`
     - `存档系统`
     - `导航检查`
  2. 已确认这 5 条线都不是“乱报完成”，而是：
     - 第一波都各自安全上传了一部分
     - 但都还没把剩余尾账收完
  3. 已新增第二波批次入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root历史小批次上传分发批次_02.md`
  4. 已新增 5 条第二波专属 prompt：
     - `2026-04-23_给spring-day1_shared-root历史小批次上传prompt_02.md`
     - `2026-04-23_给UI_shared-root历史小批次上传prompt_02.md`
     - `2026-04-23_给NPC_shared-root历史小批次上传prompt_02.md`
     - `2026-04-23_给存档系统_shared-root历史小批次上传prompt_02.md`
     - `2026-04-23_给导航检查_shared-root历史小批次上传prompt_02.md`
  5. 在 `农田交互修复V3` 回执补齐后，又追加了第 6 条第二波专属 prompt：
     - `2026-04-23_给农田交互修复V3_shared-root历史小批次上传prompt_02.md`
  6. 额外新增一条通用补交通知：
     - `2026-04-23_给其它已施工线程_shared-root上传回执补交通用prompt_01.md`
- 当前关键判断：
  - 第二波不能再写成“清 blocker 模式”，而必须强制每条线程一次只碰 `1` 个历史小批次；撞到 `CodeGuard / mixed / own-root 扩根` 就立刻停车。
- 当前缺口：
  - 第一波 6 条线程回执现已全部收齐。
- 当前恢复点：
  - 下一轮优先继续收：
    1. `spring-day1 / UI / NPC / 存档系统 / 导航检查 / 农田交互修复V3` 的第二波小批次回执
    2. 其它若其实已做过上传动作但还没报实的线程，统一先走通用补交 prompt

## 2026-04-23｜第二波回执总审：问题已从“有没有试”升级成“三类不同 blocker”

- 用户目标：
  - 用户认为这批第二波回执后面的问题可能比较严重，要求治理位不要只复述回执，而要把真实严重性、根因分类、后续方向和解决方案用人话彻底说清。
- 本轮主线：
  - 继续做治理只读总审；不新开业务上传，不代线程重跑同一批次。
- 本轮实际做成了什么：
  1. 已再次核实以下 6 条第二波回执与现场大体一致：
     - `spring-day1`
     - `UI`
     - `NPC`
     - `存档系统`
     - `导航检查`
     - `农田交互修复V3`
  2. 已把这 6 条线分成 3 种本质不同的阻塞类型：
     - `A｜同根 / 父根扩根型`
       - `spring-day1`
       - `导航检查`
       - `农田交互修复V3`
       - `UI`
     - `B｜真实工具链 / preflight 型`
       - `存档系统`
     - `C｜历史批次本身不独立 / 需先补一致性`
       - `NPC`
  3. 已额外确认两个关键真相：
     - `UI` 这批里 `PackagePanelTabsUI.cs` 不是外围噪音，而是代码里直接安装 `PackageMapOverviewPanel / PackageNpcRelationshipPanel` 的入口，因此大概率属于同一批核心件。
     - `存档系统` 那组 `Data/Core` 三文件用普通 `git diff --name-status HEAD --` 会瞬间返回；结合 `CodexCodeGuard` 程序本体“异常时也会吐 JSON 再退出”的实现，当前更像 launcher / preflight / process 管理层问题，不像这三文件内容本身的 same-root 阻塞。
  4. 已再次确认 `NPC` 的 `104` 删除不是独立批次：
     - `NpcCharacterRegistry.asset` 里 `npcId: 104` 仍直接挂着旧 `handPortrait` 引用
     - 当前先删图会留下悬空引用
- 当前关键判断：
  - 现在严重，但不是“成果坏了”那种严重，而是：第二波已经把所有上传问题从“模糊 blocker”压成了 3 类明确 blocker。
  - 真正危险的是如果治理位继续拿同一种 prompt 口径去打 6 条线，会让：
    - `same-root` 问题被误当成“再试一次”
    - `toolchain` 问题被误当成“业务线程自己排”
    - `不独立批次` 问题被误当成“删除就能上传”
- 当前建议的解决方向：
  1. `spring-day1 / 导航检查 / 农田交互修复V3 / UI`
     - 不再重复发同类 `prompt_02`
     - 改发“已确认父根 blocker 后的下一刀”prompt
  2. `存档系统`
     - 先停业务 prompt
     - 改由治理 / 工具位单独处理 `Ready-To-Sync / CodexCodeGuard` incident
  3. `NPC`
     - 下一刀不该再问“删图能不能传”
     - 应先做 `NpcCharacterRegistry.asset` 的 `104` 引用一致性小刀
- 当前恢复点：
  - 下一轮治理位不要再把 6 条线一起按“继续试历史小批”处理；
  - 而要先按上面 3 类 blocker 分流，再决定每条线的下一份 prompt。

## 2026-04-23｜第三波已开：按 blocker 类型正式分流，不再统一群发

- 用户目标：
  - 用户直接要求开始第三波，不再停在分析层；同时要求把后来补进来的 `README/019d...`、`树石修复`、`云朵与光影` 只读补交通知也一并纳入总判断。
- 本轮主线：
  - 治理位进入真实 prompt 分发施工；不继续让第二波线程重复撞同一小批。
- 本轮实际做成了什么：
  1. 已补核三条只读补交通知：
     - `019d4d18-bb5d-7a71-b621-5d1e2319d778`
     - `树石修复`
     - `云朵与光影`
  2. 已确认它们对盘面的真实影响：
     - `019d...` 的 `ee7754b4` README docs-only 上传属实且已在远端
     - 但它当前 thread-state 仍是 `ACTIVE`，且 `README.md` 有新本地脏改，因此不能按 parked 候选混入本波
     - `云朵与光影` 的 `7e4508d0` 已在远端，但当前 `DayNightManager.cs` 仍 dirty，继续后置
     - `树石修复` 补出来的是历史 blocked 回执，不是漏报成功上传
  3. 已新增第三波批次入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root第二波blocker分流批次_03.md`
  4. 已新增 5 条第三波定向 prompt：
     - `2026-04-23_给UI_shared-root同根整合上传prompt_03.md`
     - `2026-04-23_给spring-day1_EditorStory同根整合上传prompt_03.md`
     - `2026-04-23_给导航检查_ServiceNavigation同根整合上传prompt_03.md`
     - `2026-04-23_给NPC_104主表引用一致性先修prompt_03.md`
     - `2026-04-23_给存档系统_DataCore预同步工具incident排查prompt_03.md`
  5. 本轮明确后置、不发的组：
     - `农田交互修复V3`
     - `019d4d18-bb5d-7a71-b621-5d1e2319d778`
     - `云朵与光影`
     - `树石修复`
- 当前关键判断：
  - 第三波现在不该按“线程名”分，而应按“问题类型”分：
    1. `same-root / 父根扩根`：`spring-day1 / UI / 导航检查`
    2. `引用一致性先修`：`NPC`
    3. `工具链 incident`：`存档系统`
  - `农田交互修复V3` 虽也属 same-root / 扩根型，但跨 `Assets/Editor / Assets/YYY_Scripts/Controller / Assets/222_Prefabs/Rock` 三个宽根，当前故意后置。
- 当前恢复点：
  - 下一轮用户可直接转发这 5 条第三波 prompt；
  - 其余 4 条暂不推进，除非用户改判。

## 2026-04-24｜第三波回执总审：主问题已收敛成“4 条工具 incident + 1 条资源根同步 blocker”

- 用户目标：
  - 用户要求在第三波回执全部回齐后，给出一份真正可调度的人话总审，不要再停在“谁说了什么”。
- 本轮主线：
  - 治理位保持只读审计；不再继续发同类业务上传 prompt。
- 本轮实际做成了什么：
  1. 已再次核实以下 5 条第三波回执与现场大体一致：
     - `spring-day1`
     - `UI`
     - `存档系统`
     - `导航检查`
     - `NPC`
  2. 已确认当前真正主问题不是“各线程业务切片还不完整”，而是：
     - `spring-day1 / UI / 存档系统 / 导航检查` 已共同推进到 `CodexCodeGuard / preflight` 工具 incident 层
     - `NPC` 已把 `npcId:104` 的主表内容一致性补平，新的 blocker 是 `Assets/Resources/Story` 目录级同步被他线资源挡住
  3. 关键现场再次钉死：
     - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 里确有
       - `GitDirtyState.Load(repoRoot)`
       - `RunGit(repoRoot, "diff", "--name-status", "HEAD", "--")`
       - `RunProcess("git", ...)`
       - `StandardOutput.ReadToEnd() / StandardError.ReadToEnd()`
     - [NpcCharacterRegistry.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/NpcCharacterRegistry.asset) 中 `npcId:104` 当前已是 `handPortrait: {fileID: 0}`
     - [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 当前确实直接安装 `PackageMapOverviewPanel / PackageNpcRelationshipPanel`
  4. 已修正一个此前风险判断：
     - `019d4d18-bb5d-7a71-b621-5d1e2319d778` 当前最新状态已是 `PARKED`
     - 但它仍属于独立 README 线，不并入当前工具 incident 主处理波次
- 当前关键判断：
  - 第三波已经完成它的任务：把问题从“业务上传还能不能继续试”压成了两条真正的下一步：
    1. `4` 条统一工具 incident 处理线
    2. `1` 条 `NPC` 资源目录同步阻塞处理线
- 当前恢复点：
  - 下一轮不该再给 `spring-day1 / UI / 存档系统 / 导航检查` 发业务上传 prompt；
  - 应改成一条统一工具 incident 线；
  - `NPC` 单独发“`Assets/Resources/Story` 根同步 blocker”线；
  - `农田交互修复V3 / 树石修复 / 云朵与光影 / 019d...` 继续后置观察。

## 2026-04-23｜其它已施工线程补交通用回执：019d4d18-bb5d-7a71-b621-5d1e2319d778 已补实 README docs-only 上传

- 用户目标：
  - 按 `2026-04-23_给其它已施工线程_shared-root上传回执补交通用prompt_01.md`，先只读核实这条线程是否真的有“已提交 / 已 push 但未正式回执”的 shared-root 上传结果，不默认继续开发或继续上传。
- 本轮主线：
  - 这轮仍是治理只读补交；未进入真实施工，未补跑 `Begin-Slice`。
- 本轮实际做成了什么：
  1. 已核实这条线程确有一笔此前未正式回执、但已经真实落到 `origin/main` 的 docs-only 上传：
     - `ee7754b4`
     - `docs: refresh README showcase`
  2. 已核实该提交当前就在：
     - `main`
     - `origin/main`
  3. 已核实该批次实际提交内容只有：
     - `README.md`
     - `.github/readme/day1_arrival.png`
     - `.github/readme/hero_day1_labor.png`
  4. 已核实这条上传对应的业务 own 路径当前是 clean；本轮没有发现新的未 push 本地提交。
- 关键判断：
  - 这条线程不是“无上传结果”，而是“有一笔已经 push 的 docs-only 上传，但之前只写进了线程工作记录，没有正式按 shared-root 上传回执口径交回来”。
  - 因此这轮正确动作是补交，不是继续替它开第二刀。
- 当前恢复点：
  - 治理位后续不需要再为这条线程补发“继续上传 README”一类 prompt；
  - 如果要继续这条线程的 shared-root 上传，必须等用户或治理位重新点名下一批，而不是沿这次补交顺手扩写。

## 2026-04-24｜spring-day1 `prompt_03` 执行结果：`Editor/Story` 根内整合批停在 preflight / CodeGuard incident

- 用户目标：
  - 用户要求 `spring-day1` 不再重跑刚执行完的 `prompt_02`，而是严格按 `2026-04-23_给spring-day1_EditorStory同根整合上传prompt_03.md`，只对白名单 `23` 个 `Assets/Editor/Story` 文件做一次根内整合批的真实上传尝试，绝不扩到 `Managers / Directing / Tests`。
- 当前主线：
  - 这轮是 shared-root 上传施工，不是继续 Day1 runtime 开发。
- 本轮实际做成了什么：
  1. 已读取：
     - `2026-04-23_给spring-day1_EditorStory同根整合上传prompt_03.md`
     - `2026-04-23_shared-root第二波blocker分流批次_03.md`
  2. 已只读核实当前 `Assets/Editor/Story` 现场：
     - 同根当前只剩 `7` 个旧改 `M` + `16` 个新增 validation/probe/snapshot/cleanup 文件 `??`
     - 没有新的同根剩余 dirty 混入
  3. 已静态确认这 `7` 个旧改不是跨根 runtime 杂质，而是同一组 `Editor/Story` 工具链里的窗口、菜单、validation、evidence 旧尾巴，因此当前可诚实视作 `Editor/Story` 根内整合批。
  4. 已执行 `Begin-Slice`：
     - `thread=spring-day1`
     - `slice=shared-root EditorStory root-integration upload 2026-04-24`
  5. 已只对白名单这 `23` 个文件跑 stable launcher `preflight`：
     - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action preflight -OwnerThread spring-day1 -Mode task -IncludePaths ...`
  6. 已确认这次不是 own-root 新 blocker，也不是我越权扩根，而是 preflight 本身没有返回稳定 JSON：
     - launcher 在工具调用窗口内超时
     - 残留进程明确显示挂住的是 `CodexCodeGuard.dll --phase pre-sync`
  7. 已把残留的 `powershell` 与其子 `dotnet/CodexCodeGuard` 进程清掉，并执行 `Park-Slice`：
     - `reason=preflight-timeout-no-json`
     - 当前状态已回到 `PARKED`
- 当前关键判断：
  - `spring-day1` 这轮 `prompt_03` 的新第一真实 blocker，已经从前一轮的 `same-root remaining dirty` 升级成了 `preflight / CodexCodeGuard` incident。
  - 这不是“23 文件不自洽”导致的失败，而是工具链在这组 `Editor/Story` 根内整合批上挂住。
  - 本轮没有越权扩到：
    - `Assets/YYY_Scripts/Story/Managers/*`
    - `Assets/YYY_Scripts/Story/Directing/*`
    - `Assets/YYY_Tests/Editor/*`
- 当前恢复点：
  - 如果后续继续这条上传线，正确下一步不是重跑 `prompt_03`，而是把 `CodexCodeGuard pre-sync` 挂死提升成治理/工具 incident，再决定是否还能继续这组 `Editor/Story` 根内整合批上传。

## 2026-04-24｜第四波已改写成“统一工具 incident + NPC 资源根同步 blocker”分发

- 用户目标：
  - 用户同意治理位不再停在口头分析，而是直接把最新总审结论落成可转发的下一轮 prompt。
- 当前主线：
  - 治理位进入真实 prompt 产出；不再给 `spring-day1 / UI / 存档系统 / 导航检查` 继续发业务上传 prompt。
- 本轮实际做成了什么：
  1. 已再次核实现场关键事实与回执一致：
     - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 里确有
       - `GitDirtyState.Load(repoRoot)`
       - `RunGit(repoRoot, "diff", "--name-status", "HEAD", "--")`
       - `RunProcess(...)`
       - `StandardOutput.ReadToEnd() / StandardError.ReadToEnd()`
     - [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 直接安装 `PackageMapOverviewPanel / PackageNpcRelationshipPanel`
     - [NpcCharacterRegistry.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/NpcCharacterRegistry.asset) 中 `npcId:104` 当前已是 `handPortrait: {fileID: 0}`
     - `spring-day1 / UI / 导航检查 / 存档系统 / NPC` 的状态文件当前都写在 `active-threads/` 目录下，但文件内容状态均已是 `PARKED`
  2. 已新增第四波治理入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_shared-root工具incident与资源根同步分发批次_04.md`
  3. 已新增统一工具 incident prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给只读工具链分身_统一CodexCodeGuard预同步incident排查prompt_04.md`
  4. 已新增 `NPC` 目录级同步 blocker prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给NPC_AssetsResourcesStory目录同步blocker处理prompt_04.md`
  5. 已在本波入口里明确停发：
     - `spring-day1`
     - `UI`
     - `存档系统`
     - `导航检查`
     - `农田交互修复V3`
     - `019d4d18-bb5d-7a71-b621-5d1e2319d778`
     - `云朵与光影`
     - `树石修复`
- 当前关键判断：
  - 当前真正需要继续动的，不是 `4` 条业务上传线程，而是：
    1. `只读工具链分身` 去把 `CodexCodeGuard / pre-sync` 统一 incident 收成共因或分因结论
    2. `NPC` 去把 `Assets/Resources/Story` 目录同步 blocker 收成 exact 边界或等待 foreign blocker 自然消失
  - 也就是说，本波已经从“按线程推进上传”改成“按问题类型推进治理”。
- 当前恢复点：
  - 下一步优先等：
    1. `只读工具链分身` 的统一 incident 结论
    2. `NPC` 的目录 blocker 结论
  - 在这两条线回来前，不再给 `spring-day1 / UI / 存档系统 / 导航检查` 原业务线程继续发上传 prompt。

## 2026-04-24｜统一 CodexCodeGuard / pre-sync incident 已收成 1 个共因工具事故

- 用户目标：
  - 按 `2026-04-24_给只读工具链分身_统一CodexCodeGuard预同步incident排查prompt_04.md`，只读判断 `spring-day1 / UI / 存档系统 / 导航检查` 这 4 条线到底是 1 个共因 incident 还是 2+ 个分裂 incident，并明确下一刀该修哪层。
- 当前主线：
  - 本轮保持只读 incident 审计；不替任何业务线程继续上传，不进入真实施工。
- 本轮实际做成了什么：
  1. 已对齐 4 条线的 `prompt_03`、`thread-state`、线程 memory 和工具代码调用链。
  2. 已确认这 4 条线当前都已 `PARKED`，且 blocker 都已从业务 same-root 问题推进到工具层：
     - `spring-day1`：stable preflight 超时，残留 `CodexCodeGuard.dll --phase pre-sync` 进程。
     - `UI`：同一整合批里 3 个脚本直接报 `CodexCodeGuard returned no JSON`，另 1 个脚本只落到 `unity_validation_pending / baseline_fail`。
     - `存档系统`：已有静态证据把挂点压到 `Program.cs -> GitDirtyState.Load -> RunProcess(git diff --name-status HEAD --)`。
     - `导航检查`：`Ready-To-Sync` 已进入工具 incident，不再是同根 remaining dirty。
  3. 已确认 `Ready-To-Sync.ps1` 和 `StateCommon.ps1` 只是调用稳定 launcher；stable launcher 也只是把 canonical `git-safe-sync.ps1` 转发执行。
  4. 已确认真正最靠近根因的层不是 `Ready-To-Sync / StateCommon`、不是 stable launcher，也不是业务线程白名单本身，而是 `CodexCodeGuard Program.cs` 的重型 pre-sync 路径。
- 当前关键判断：
  1. 这 4 条线应收成 `1` 个统一 incident，不建议拆成 `2+` 个独立事故。
  2. `UI` 的 `baseline_fail` 目前只算同批次里的次级表象，不足以单独升级成第二个根因；因为同一批里已有 3 个文件直接落 `CodexCodeGuard returned no JSON`。
  3. 当前最像真根因的不是业务内容，而是：
     - `Program.cs` 在 `Run()` 里无差别先做 `GitDirtyState.Load(repoRoot)`；
     - `GitDirtyState.Load()` 又会跑整仓 `git diff --name-status HEAD --` 和 `ls-files --others --exclude-standard`；
     - `RunProcess()` 使用串行 `StandardOutput.ReadToEnd()` / `StandardError.ReadToEnd()` 且没有 timeout；
     - 一旦卡住，`Main()` 就到不了“异常也输出 JSON”的收尾分支，于是外层统一表现成 `no JSON / preflight hang`。
  4. 所以下一刀不该回交业务线程各自继续排，而应交给 `Codex规则落地` 的工具修复线。
- 下一刀最小修复边界：
  1. `D:\Unity\Unity_learning\Sunset\scripts\CodexCodeGuard\Program.cs`
     - `Run()`
     - `GitDirtyState.Load()`
     - `RunGit()`
     - `RunProcess()`
  2. `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
     - `Invoke-CodeGuard()`
     - 只做“超时/挂死时的兜底报错与 stale process 清理增强”，不扩大到业务白名单逻辑。
- 当前恢复点：
  - `spring-day1 / UI / 存档系统 / 导航检查` 继续保持 `PARKED`；
  - 下一轮如果继续，应新开工具修复切片，先修 `Program.cs` 主因，再视情况给 `git-safe-sync.ps1` 补结构化兜底。

## 2026-04-24｜第四波回执审核：统一 incident 结论通过，NPC 目录 blocker 结论通过，但工具审计回执存在落账错线程

- 用户目标：
  - 用户要求对最新两份第四波回执做治理审核，不只看“说得像不像”，而要看“是否属实、是否应停发、下一步到底归谁”。
- 当前主线：
  - 治理位继续只读审单；不新开业务上传 prompt。
- 本轮实际做成了什么：
  1. 已核实统一工具 incident 回执的核心结论成立：
     - `spring-day1 / UI / 存档系统 / 导航检查` 当前更合理地收成 `1` 个共因 `CodexCodeGuard / pre-sync` incident；
     - [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 的 `GitDirtyState.Load -> RunGit(diff --name-status HEAD --) -> RunProcess -> ReadToEnd` 仍是最靠近根因的工具层；
     - `4` 条原业务线程继续保持 `PARKED` 的裁定正确。
  2. 已核实 `NPC` 回执的核心结论成立：
     - `npcId:104` 当前仍保持 `handPortrait: {fileID: 0}`；
     - `104.png / 104.png.meta` 仍是删除态；
     - [Recipe_9102_Pickaxe_0.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset) 仍是当前 `Assets/Resources/Story` 第一真实 foreign blocker；
     - `NPC` 当前继续 `PARKED` 的裁定正确。
  3. 已额外核到一个治理缺口：
     - 统一 incident 这份回执虽然内容基本可信，但它把审计落账写到了 `.codex/threads/Sunset/019d4d18-bb5d-7a71-b621-5d1e2319d778/memory_0.md`；
     - `.codex/threads/Sunset/只读工具链分身/memory_0.md` 本轮并没有同步落这份 incident 结论；
     - 因此这份回执存在“线程归档错位”，不能当成完全干净的线程记忆闭环。
- 当前关键判断：
  - 这两份回执里，`事实判断` 基本都能采纳；
  - 但 `统一工具 incident` 这份只能算“结论通过、归档不通过”；
  - `NPC` 这份则可视作“结论通过、停发成立、下一步应转 foreign owner”。
- 当前恢复点：
  - `spring-day1 / UI / 存档系统 / 导航检查` 继续不发业务上传 prompt；
  - 如果继续，应由 `Codex规则落地` 自己进入工具修复线；
  - `NPC` 当前不再继续发 prompt，下一步优先转 `spring-day1/workbench` 方向认领 `Recipe_9102_Pickaxe_0.asset`，若 owner 仍不清则再升治理位。

## 2026-04-24｜统一工具 incident 首轮修复已落地：不再挂死/无 JSON，现会稳定吐出真实代码闸门结果

- 用户目标：
  - 用户要求我继续执行下一步，并提醒还有很多线程尚未开工，因此这轮只推进当前最高优先级的工具修复线，不额外开启其它线程。
- 当前主线：
  - `Codex规则落地` 进入真实工具修复施工，只修：
    - `scripts/CodexCodeGuard/Program.cs`
    - `scripts/git-safe-sync.ps1`
  - 不碰任何业务文件，不新开未开工线程。
- 本轮实际做成了什么：
  1. 已对 [Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 落下两类修复：
     - `RunProcess()` 从同步 `ReadToEnd()` 改成异步收流 + 超时 + 超时后 kill 整个进程树；
     - `GitDirtyState.Load()` 从整仓 `git diff/ls-files` 改成只扫描 `*.cs`，不再把全仓非代码脏改卷进代码闸门前置。
  2. 已对 [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1) 的 `Invoke-CodeGuard()` 落下结构化兜底：
     - 新增带超时的外部进程捕获；
     - 对 `timeout / no JSON / bad JSON / exit without block` 都会返回结构化 `CodeGuard` 报告，而不再直接炸成黑盒异常。
  3. 已完成代表性验证：
     - `dotnet build scripts/CodexCodeGuard/CodexCodeGuard.csproj -c Release`：通过；
     - `git-safe-sync.ps1` PowerShell 语法解析：通过；
     - 直跑 `CodexCodeGuard.dll` 处理 `Data/Core` 三文件：约 `34.7s` 内正常返回 JSON，不再挂死，只真实报出 `4` 条 `CS1061`；
     - 经 `git-safe-sync.ps1 -Action preflight` 处理 `Data/Core` 三文件：约 `50.0s` 内正常返回 preflight 文本，不再是 `no JSON`；
     - 经 `git-safe-sync.ps1 -Action preflight` 处理 `UI/Tabs` 七文件：约 `50.5s` 内正常返回 preflight 文本，不再是 `no JSON`。
- 当前关键判断：
  - 这轮修复真正推进的是“工具不再挂死/无 JSON”，不是“业务问题都已经被修好”。
  - 当前 `Data/Core` 和 `UI/Tabs` 现在都能稳定吐出真实代码闸门结果，所以业务线程后面拿到的将是可判断的 compile/blocker，而不是一团黑盒 incident。
  - 也因此，前面那些还没开工的线程仍应继续后置；当前最值钱的事就是先把这条统一工具 incident 线打通。
- 当前恢复点：
  - 下一步优先重发最小复核给：
    1. `存档系统`
    2. `UI`
    3. 如有必要再到 `spring-day1 / 导航检查`
  - 目标不是继续业务上传，而是验证它们现在会返回“真实 blocker”而不是 `CodexCodeGuard / no JSON / hang`。
## 2026-04-24｜工具修复后的第五波最小复核已成批：只唤醒存档系统与 UI

- 用户目标：
  - 在工具 incident 第一刀已生效的前提下，继续推进当前下一步，但明确要求不要顺手把前面那些还没开工的线程一起叫起来。
- 当前主线：
  - `Codex规则落地` 继续做治理收口；
  - 本轮不再修业务代码，只把“工具修复后的最小复核分发”正式落成可同步批次。
- 本轮实际做成了什么：
  1. 已新增第 `05` 波入口：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_统一工具incident修复后最小复核分发批次_05.md`
  2. 已新增两条专属复核 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给存档系统_工具修复后DataCore最小复核prompt_05.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给UI_工具修复后UITabs最小复核prompt_05.md`
  3. 已在正文里明确约束：
     - `存档系统` 只准对白名单 `InventoryItem.cs / SaveDataDTOs.cs / SaveManager.cs` 做 `1` 次真实最小复核；
     - `UI` 只准对白名单 `UI/Tabs` 七文件做 `1` 次真实最小复核；
     - 两条线这轮都不准顺手修业务代码、不准换第二批。
  4. 已在批次入口里明确停发：
     - `spring-day1`
     - `导航检查`
     - `NPC`
     - `农田交互修复V3`
     - 其它仍未开工线程
- 当前关键判断：
  - 当前最值钱的不是“全面重启线程”，而是先验证工具黑盒 incident 是否已经稳定降级成真实 blocker；
  - 因此第五波只唤醒 `存档系统 / UI` 两条最关键样本线最合理。
- 当前恢复点：
  - 下一步应把本轮工具修复 + 第五波 prompt 一起同步；
  - 同步后直接转发给 `存档系统 / UI`；
  - 其余未开工线程继续保持后置，不在这轮顺手推进。

## 2026-04-24｜工具修复与第五波 prompt 已正式同步，治理线程回到 PARKED

- 用户目标：
  - 继续当前下一步，并把这轮治理收口做完整，不留下“已经做完但 memory 还停在待同步”的半拍状态。
- 当前主线：
  - `Codex规则落地` 已完成本轮工具修复与第 `05` 波最小复核分发文件的正式同步；当前回到 `PARKED`。
- 本轮实际做成了什么：
  1. 已通过 `Ready-To-Sync`；
  2. 已用仓库 working tree 版 `git-safe-sync.ps1` 完成 `7` 个目标文件同步；
  3. 已形成并推送提交：
     - `111b00a3`
     - `2026.04.24_Codex规则落地_04`
  4. 已确认：
     - `origin/main...HEAD = 0 0`
     - 本轮 own 路径 `clean`
     - 当前治理线程状态已回到 `PARKED`
- 当前关键判断：
  - 这轮正式完成的不是“业务已修好”，而是“工具 incident 第一刀 + 第五波最小复核分发”已经安全归仓；
  - 当前可以直接进入下一步转发，但仍只该转给 `存档系统 / UI` 两条线。
- 当前恢复点：
  - 现在只需要把第 `05` 波转发壳交给用户；
  - 等 `存档系统 / UI` 回执回来后，再判断是否扩到其它线程。

## 2026-04-26｜第五波两份回执审核：工具验证通过，但 UI 与存档系统都不宜直接机械续发

- 用户目标：
  - 审核 `UI` 与 `存档系统` 两份第 `05` 波真实最小复核回执，判断它们是否属实，以及现在该不该直接发下一轮 prompt。
- 当前主线：
  - `Codex规则落地` 继续做治理总闸审单；本轮不写新的业务 prompt，先判四类裁定。
- 本轮实际做成了什么：
  1. 已核实两份回执的共同核心事实都成立：
     - 两条线这次都不再出现 `CodexCodeGuard hang / no JSON / baseline_fail` 黑盒；
     - 说明第四波工具修复确实生效；
     - 两条线程当前都已合法回到 `PARKED`。
  2. 已核实 `UI` 回执里的 `3` 个 `CS0103 + 1` 个 `CS0649` 口径基本属实，但又额外发现更深一层：
     - [PackagePanelTabsUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs) 当前确实新增了 `PackageSaveSettingsPanel.EnsureInstalled(...)` 三处调用；
     - 但真正的 [PackageSaveSettingsPanel.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs) 只存在于本地磁盘，不在 `HEAD`，而且被 `.gitignore` 里的 `Save/` 规则忽略；
     - 因此这条线现在暴露的并不只是“UI/Tabs 内三行坏引用”，而是“当前有一个未正式纳管的本地 Save 面板依赖”。
  3. 已核实 `存档系统` 回执里的 `4` 条 `CS1061` 口径成立，并额外确认：
     - 这 `4` 个方法在 `HEAD` 里确实不存在；
     - 它们只出现在当前工作树未同步的 [InventorySortService.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs)、[CraftingService.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Crafting/CraftingService.cs)、[ToolbarUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs)、[InventoryInteractionManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs) 里；
     - 所以它也不是“只修 SaveManager.cs 四行调用”这么简单，而是一个跨 `Data/Core + Service + UI` 的运行时上下文联动口。
  4. 已量到 `存档系统` 下一刀的扩根风险：
     - `Assets/YYY_Scripts/Service/Inventory` 目录当前不只 `InventorySortService.cs` 一处脏改；
     - `Assets/YYY_Scripts/UI/Inventory` 与 `Assets/YYY_Scripts/UI/Toolbar` 也各自挂着多处同根 dirty；
     - 因此如果现在直接把这条线扩成跨根修复 prompt，很可能立刻撞回 same-root / mixed dirty 问题。
- 当前关键判断：
  - 第 `05` 波已经完成它真正要验证的事：工具黑盒确实已经被拨开；
  - 但 `UI` 与 `存档系统` 当前都不适合未经拍板就直接续发“第 `06` 波业务修复 prompt”。
- 四类裁定：
  - `UI`：`停给用户分析 / 审核`
  - `存档系统`：`停给用户分析 / 审核`
- 当前恢复点：
  - 暂不生成新的 `prompt_06`；
  - 先由用户决定：
    1. `UI` 是要把 `PackageSaveSettingsPanel` 正式纳入 repo，还是先撤掉 `UI/Tabs` 对这套本地依赖的挂钩；
    2. `存档系统` 是要授权一刀更宽的跨根运行时上下文集成，还是继续保持 `Data/Core` 这条线停车。

## 2026-04-26｜总收口继续推进：项目内容已过 working-tree preflight，最后收口改成直接走仓库版 sync

- 用户目标：
  - 用户明确不再停在线程级分析，要求把当前项目有效内容按正常批次真正提交上去；允许从“分线程裁定”切到“总收口模式”。
- 当前主线：
  - `Codex规则落地` 继续承担总收口；本轮不再生成新的业务 prompt，而是直接清治理链最后一道提交流程阻断。
- 本轮实际做成了什么：
  1. 已复现并钉死 `Ready-To-Sync` 的真实误判：
     - `CodexCodeGuard` 已输出 `CanContinue:true` 的最终 JSON；
     - 但 [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1) 仍因空退出码把它改判成 `CODEGUARD_EXIT_NO_BLOCK`。
  2. 已对 [git-safe-sync.ps1](/D:/Unity/Unity_learning/Sunset/scripts/git-safe-sync.ps1) 补两刀：
     - `Invoke-CodeGuard()` 现在只要最后一行 JSON 成功解析，就直接信任该报告，不再用空退出码二次否决；
     - 候选 `*.cs` 现在只对白名单里的 `Assets/**` 生效，不再把 `scripts/CodexCodeGuard/Program.cs` 误当 Unity 业务脚本去编。
  3. 已重新验证工具侧：
     - `dotnet build scripts/CodexCodeGuard/CodexCodeGuard.csproj -c Release --nologo` 通过；
     - 仓库 working-tree 版 `git-safe-sync.ps1 -Action preflight`，对白名单
       - 当前总收口 owned 路径
       - `scripts/git-safe-sync.ps1`
       - `scripts/CodexCodeGuard/Program.cs`
       做真实预检，结果已返回：
       - `是否允许按当前模式继续: True`
       - `代码闸门通过: True`
       - 当前仅余 `32` 条 warning，不再有 compile error。
- 当前关键判断：
  - 项目这次已经不再被 `CodexCodeGuard` 黑盒或 launcher 误判挡住；
  - 现在可以诚实进入“正式同步当前总收口切片”的最后一步。
- 当前恢复点：
  - 下一步直接用仓库 working-tree 版 `git-safe-sync.ps1` 做总收口同步；
  - 同步完成后再补最终提交 SHA、push 结果、线程状态与审计日志。
