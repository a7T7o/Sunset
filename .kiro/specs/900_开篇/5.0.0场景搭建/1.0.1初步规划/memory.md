# 1.0.1初步规划 - 工作区记忆

## 模块概述
- 子工作区名称：`1.0.1初步规划`
- 所属父工作区：`5.0.0场景搭建`
- 阶段目标：在真实场景施工前，固化资产基础、场景骨架、Tilemap 分层、执行顺序与阶段准入条件。

## 当前状态
- **完成度**：60%
- **最后更新**：2026-03-20
- **状态**：文档基线与首轮资产普查已完成，等待进入下一阶段 scene 命名与创建方案收敛。

## 会话记录

### 会话 1 - 2026-03-20

**用户需求**：
> 将 `1.0.1初步规划` 作为当前 Kiro 子工作区，建立后续开发可遵守的文档基石和任务清单；不是为了堆文档，而是要能直接服务后续场景搭建，并先把资产普查、场景骨架、Tilemap 分层和执行顺序落下来。

**完成任务**：
1. 建立 `requirements.md`、`design.md`、`tasks.md`、`memory.md` 基础文档。
2. 只读确认本阶段不直接改 `Primary`，也不直接进入真实 scene 施工。
3. 完成首轮资产普查，确认 scene / prefab / Tilemap / sprite 入口齐备。
4. 固化后续独立 scene 的根层级草案与 Tilemap 八层结构。
5. 固化后续执行顺序：资产普查 → 场景骨架 → 地图底稿 → 结构层 → 装饰层 → 前景遮挡与逻辑层 → 回读修正 → 交付。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md` - [新增]：定义本阶段目标与验收标准。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [新增]：定义本阶段输出、骨架与分层方案。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [新增]：定义当前阶段任务清单与下一步动作。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\资产普查.md` - [新增]：沉淀本轮只读资产普查结果。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [新增]：建立子工作区记忆。

**解决方案**：
把“后续如何搭场景”从聊天共识收束为可执行工作区基线：先确认项目确实有足够资产，再把骨架、分层、任务与交付标准固定住，为下一阶段真实 scene 施工准备稳定入口。

**已验证事实**：
- `Assets/000_Scenes` 下当前可见 6 个 scene。
- `Assets/222_Prefabs` 下当前可见 `House`、`Farm`、`Tree`、`Rock`、`Dungeon props` 等场景相关目录。
- `Assets` 下大约存在 `387` 个 prefab、`3756` 个 png、`4717` 个 asset。
- `Assets/223_Prefabs_TIlemaps`、`FarmTileManager.cs`、`LayerTilemaps.cs` 证明当前项目存在 Tilemap 相关资产与脚本基础。

**遗留问题**：
- [ ] 仍需最终确定下一阶段新 scene 命名、路径与最小创建方案。
- [ ] 仍需确认 prefab 候选池需要整理到什么粒度才足够开工。
- [ ] 真实施工前仍需按当时 live 规则重新确认准入。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 当前阶段只做规划与普查，不直接施工 | 先把方法和边界定稳，降低后续返工 | 2026-03-20 |
| Tilemap 固定为 6 视觉层 + 2 逻辑层 | 满足秩序与可维护性，同时避免过度拆层 | 2026-03-20 |
| 先做环境与结构，不先扩成 NPC / UI 细节填充 | 当前主目标是独立场景骨架，而不是交互填满 | 2026-03-20 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md` | 当前阶段需求 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` | 当前阶段设计 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` | 当前阶段任务清单 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\资产普查.md` | 首轮资产普查 |
| `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md` | 父工作区记忆 |

## 涉及的代码与资产入口

| 路径 | 关系 |
|------|------|
| `D:\Unity\Unity_learning\Sunset\Assets\223_Prefabs_TIlemaps` | Tilemap prefab 候选入口 |
| `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs` | 环境 prefab 候选入口 |
| `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs` | Tilemap / 农田脚本入口 |
| `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Data\LayerTilemaps.cs` | 分层 Tilemap 数据入口 |
| `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` | 热区参考场景，本阶段只读避开 |

### 会话 2 - 2026-03-20（父子工作区口径纠偏）

**用户需求**：
> 用户明确指出：`5.0.0场景搭建` 是父工作区，不应在其根目录堆放三件套；真正的正文承载区应是当前子工作区 `1.0.1初步规划`，要求立即修正。

**完成任务**：
1. 复核父子工作区语义后，承认此前把“父工作区”误当成“当前正文承载区”，属于理解偏差。
2. 把父层三件套中的有效内容下沉整合到当前子工作区的 `requirements.md`、`design.md`、`tasks.md`。
3. 删除父层误建的 `requirements.md`、`design.md`、`tasks.md`，让子工作区成为当前唯一正文承载区。
4. 保留父层 `memory.md` 作为父工作区的承接与索引层，不再让它承担当前阶段正文。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md` - [修改]：吸收父层长期主线目标与硬约束。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [修改]：吸收父层执行模型、MCP 职责与交付定义。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：吸收父层后续主线路线。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\requirements.md` - [删除]：父层不再承载当前正文。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\design.md` - [删除]：父层不再承载当前正文。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\tasks.md` - [删除]：父层不再承载当前正文。

**解决方案**：
把当前结构修正为“父层记忆承接 + 子层正文承载”的模型。这样既保留父子关系，又避免在父层重复堆文档。

**遗留问题**：
- [ ] 下一步仍需在当前子工作区继续完成新 scene 命名、路径与最小创建方案。

### 会话 3 - 2026-03-20（新 scene 命名与最小创建方案收口）

**用户需求**：
> 继续开始，直接把当前子工作区往前推进。

**完成任务**：
1. 只读复核当前 scene 现场：`Assets/000_Scenes` 为扁平目录，当前 scene 名包括 `Artist`、`Artist_Temp`、`DialogueValidation`、`Primary`、`SampleScene`、`矿洞口`。
2. 通过 Unity 只读确认 Build Settings 当前只有 `Assets/000_Scenes/Primary.unity`。
3. 检索到 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\DoorTrigger.cs` 存在按字符串加载 scene 的入口，因此新 scene 名必须稳、清楚、低冲突。
4. 在 `design.md` 中正式收口推荐方案：
   - 推荐路径：`Assets/000_Scenes/SceneBuild_01.unity`
   - 推荐名称：`SceneBuild_01`
   - 初建时不加入 Build Settings
   - 使用 Empty Scene 起步，并创建 `SceneRoot / Systems / Tilemaps / PrefabSetDress / GameplayAnchors / LightingFX / DebugPreview`
5. 更新 `tasks.md`，将“确认新 scene 命名、路径与最小创建方案”标记为已完成。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [修改]：新增新 scene 命名、路径、Build Settings 策略与最小创建方案。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：将 scene 命名方案收口为已完成项。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮方案收口。

**解决方案**：
在不提前绑定具体地块主题的前提下，先给当前主线一个中性、稳定、低冲突的独立施工 scene 名称，并明确它暂不进入 Build Settings，从源头上把“施工承载面”和“正式运行入口”分开。

**已验证事实**：
- `Assets/000_Scenes` 当前没有子目录，采用扁平管理。
- 当前 Build Settings 只有 `Primary.unity`。
- 项目中至少已有一处 scene 字符串加载入口：`DoorTrigger.cs`。

**遗留问题**：
- [ ] 仍需确认首版 prefab 候选池需要整理到什么粒度才足够开工。
- [ ] 仍需在真实写入前执行一次 create-only 级别的新 scene 施工准入。

### 会话 4 - 2026-03-20（工作面迁入专属 worktree）

**用户需求**：
> 用户要求给当前场景搭建线分配独立 branch，让它继续写文档和后续工作，不再把 shared root 当成长寿命 WIP 台面。

**完成任务**：
1. 当前子工作区已迁入专属 branch/worktree：
   - branch：`codex/scene-build-5.0.0-001`
   - worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
2. 当前已有的 `requirements / design / tasks / 资产普查 / memory` WIP 已保留在该 branch 中继续承载。

**关键结论**：
- 当前阶段可以继续推进：
  - prefab 候选池整理粒度
  - scene 骨架前的索引与规划
  - 不触碰 Unity / MCP 的文档工作
- 当前阶段暂不扩到：
  - 真实 scene 创建
  - `Primary.unity` 相关操作
  - Unity / MCP 高频写链路

**恢复点 / 下一步**：
- 下一步优先把“首版 prefab 候选池要整理到什么粒度才足够开工”收口。

### 会话 5 - 2026-03-20（新 worktree 现场复核与 prefab 粒度收口）

**用户需求**：
> 用户明确当前专属现场为 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 @ codex/scene-build-5.0.0-001`，要求本轮只做：复核 `cwd / branch / git status`，继续推进 prefab 候选池整理粒度、文档、资产普查与 scene 规划；禁止回 shared root、禁止进入 Unity / MCP 高频施工、禁止创建真实 scene、禁止碰 `Primary.unity`。

**完成任务**：
1. 只读复核当前现场：
   - `cwd`：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch`：`codex/scene-build-5.0.0-001`
   - 初始 `git status --short` 为：
     - `M .codex/threads/Sunset/Skills和MCP/memory_0.md`
     - `?? .kiro/specs/900_开篇/5.0.0场景搭建/`
2. 回读当前子工作区正文与相关规则，确认此前已经收口：
   - 新 scene 推荐：`Assets/000_Scenes/SceneBuild_01.unity`
   - Tilemap 八层结构
   - 当前阶段仍为“规划 / 普查阶段”，不进入真实 scene 创建
3. 继续做只读资产普查，补齐 prefab 候选池粒度证据：
   - `House`：5 个 prefab
   - `Tree`：3 个 prefab
   - `Rock`：3 个 prefab
   - `House_Tilemap`：4 个 prefab
   - `Farm`：117 个 prefab（`Food` 98，`T` 19）
   - `Dungeon props`：42 个 prefab
   - `UI`：27 个 prefab
   - `WorldItems`：71 个 prefab
   - `NPC`：3 个 prefab
   - `UI_Tilemap`：16 个 prefab
4. 正式把首版 prefab 候选池整理粒度收口为：
   - `A`：全收录（`House / Tree / Rock / House_Tilemap`）
   - `B`：按桶收录（`Farm -> Food / T`）
   - `C`：按需补入（`Dungeon props / UI / WorldItems / NPC / UI_Tilemap`）
5. 更新 `design.md`、`资产普查.md`、`tasks.md`，让当前 worktree 内的 WIP 直接承载这轮结论。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [修改]：增加首版 prefab 候选池 A/B/C 三档粒度。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\资产普查.md` - [修改]：补齐各候选目录的数量证据与分档结论。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：将“确认首版 prefab 候选池整理粒度”标记为完成。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮 worktree 现场复核与粒度收口。

**解决方案**：
这轮不再追求“先把所有 prefab 做成最终索引”，而是把候选池整理到“足够开工”的粒度：小体量环境件整组收下，大体量 `Farm` 只保留到目录桶，洞穴/UI/交互类资产延后按需补入。这样既不会卡在文档治理，也不会把真实施工前的准备做成新阻塞。

**已验证事实**：
- 当前工作面确实已切换到专属 worktree：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`。
- 当前分支确实为：`codex/scene-build-5.0.0-001`。
- 当前子工作区正文已经存在并承载此前结论：`requirements.md / design.md / tasks.md / 资产普查.md / memory.md`。
- `Farm` 是当前唯一明显不适合在规划阶段逐个平铺梳理的候选大类。

**遗留问题**：
- [ ] 仍需在准入允许后进入 `SceneBuild_01` 的 create-only 级别真实 scene 骨架创建。
- [ ] 进入真实施工前仍需再次确认 live Git / scene 安全审视 / Unity-MCP 占用现场。

### 会话 6 - 2026-03-21（回到当前进度并做全量状态盘点）

**用户需求**：
> 回到之前进度，完整说明当前还有哪些内容没有完成；所有未完成、完成到一半、以及已经做完的内容都要一起盘清。

**完成任务**：
1. 只读复核当前 worktree 现场：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = f1507f59`
   - 当前 `git status` 仍为：
     - `M .codex/threads/Sunset/Skills和MCP/memory_0.md`
     - `?? .codex/threads/Sunset/Skills和MCP/memory_1.md`
     - `?? .kiro/specs/900_开篇/5.0.0场景搭建/`
2. 回读当前主正文、线程记忆续卷 `memory_1.md`、以及 `shared-root-import_2026-03-20` 迁入快照，确认当前主线的已完成项、半完成项与未完成项。
3. 收口当前阶段判断：
   - **已完成**：父子工作区结构、正文基线、首轮资产普查、scene 命名、Tilemap 八层、prefab 候选池 A/B/C 粒度、专属 worktree 承载面。
   - **半完成**：shared-root 迁入内容已被我吸收，但尚未把导入快照本身清理成最终归档状态；`memory_1.md` 已存在且内容有效，但还未决定是否继续作为长期续卷还是只保留为迁入快照。
   - **未完成**：create-only 写态准入、真实 `SceneBuild_01` 骨架创建、底稿、结构层、装饰层、逻辑层、MCP 回读自检、最终高质量场景初稿交付。

**关键结论**：
- 当前最核心的文档规划工作已经完成，不再缺“怎么做”的蓝图。
- 当前真正阻塞主线继续往前的，不是 prefab 粒度，不是 scene 命名，也不是工作区结构，而是**还没进入允许写态的真实 scene 施工阶段**。
- 迁入内容没有丢，但它们现在更像“迁入快照 / 续卷证据”，还不是已经完全收束干净的最终归档形态。

**恢复点 / 下一步**：
- 下一步优先做的不是继续扩文档，而是：在当前专属 worktree 里确认 create-only 准入口径，然后再进入 `SceneBuild_01` 的最小骨架施工。

### 会话 7 - 2026-03-21（按执行顺序收口未完成清单）

**用户需求**：
> 用户明确判断我当前不是卡在规划，而是卡在“什么时候正式进入施工写态”；要求我先不要进入 Unity / MCP / create-only 的 scene 写态，而是先整理“按执行顺序排好的未完成清单”，并把 imported 快照、`memory_1.md`、create-only 准入复核整理成自己的开工前清单。

**完成任务**：
1. 只读回看当前主正文、`memory_1.md` 与 `shared-root-import_2026-03-20`，确认当前主线没有信息丢失，但存在“主正文 / 迁入快照 / 续卷”三层并行承载。
2. 在 `tasks.md` 中新增“当前按执行顺序排好的未完成清单”，把当前主线剩余工作重排为：
   - 先收口 imported 快照
   - 再收口 `memory_1.md`
   - 再复核 create-only 准入口径
   - 再等待你裁定我是否成为唯一 Unity / MCP 写线程
   - 然后才进入真实 scene 骨架 / 底稿 / 结构 / 装饰 / 逻辑 / 自检 / 交付
3. 在 `tasks.md` 中新增“开工前三个尾项”，专门固定：
   - imported 快照角色
   - `memory_1.md` 角色
   - create-only 准入复核

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：新增按执行顺序排好的未完成清单与开工前三个尾项。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮收口。

**关键结论**：
- 当前已经不是“继续补规划”的阶段，而是“先把施工前尾项收干净”的阶段。
- 在你完成当前第二波回执裁定之前，我这条线继续只做 docs / 规划 / 普查 / 候选池整理，不进入真实 scene 写态。

**恢复点 / 下一步**：
- 下一步严格按新顺序执行：先收口 imported 快照、`memory_1.md`、create-only 准入口径，再等你的写线程裁定。

### 会话 8 - 2026-03-21（开工前三个尾项已收口）

**用户需求**：
> 用户明确要求先完成三个尾项：imported 快照收口、`memory_1.md` 角色收口、create-only 准入复核；完成前仍不要进入 Unity / MCP / create-only 写 scene。

**完成任务**：
1. 收口 imported 快照角色：
   - 将 `shared-root-import_2026-03-20` 明确定性为“迁入证据快照层”
   - 不再把它当作 live 正文并行追加
2. 收口 `memory_1.md` 角色：
   - 将其明确为“迁入续卷 / 历史快照卷”
   - 后续线程活跃记忆继续写 `memory_0.md`
3. 以当前专属 worktree 口径重做 create-only 准入复核：
   - 当前分支：`codex/scene-build-5.0.0-001`
   - 当前 `HEAD`：`f1507f59`
   - 本地 `mcp-single-instance-occupancy.md` 仍显示 `current_claim = none`
   - 本地 `shared-root-branch-occupancy.md` 仍显示 shared root `main + neutral`
   - 本地 `.kiro/locks/active/` 目录当前只有 `.gitkeep`，没有 queue 现场文件
4. 收口判断：
   - 当前未见 Unity / MCP 占用冲突证据
   - 当前不进入 create-only scene 写态的主因，是你尚未下发“由我进入 scene 写态”的正式裁定

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：将前三个尾项收口为已完成。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_1.md` - [追加]：明确本卷仅作为迁入续卷 / 历史快照卷保留。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮收口。

**关键结论**：
- 开工前三个尾项现在已经全部完成。
- 当前只剩你的写线程裁定；一旦你放行，我这条线就直接进入 `SceneBuild_01 -> Grid + Tilemaps` 施工准备。

**恢复点 / 下一步**：
- 下一步等待你完成第二波回执并正式裁定我是否成为下一位唯一的 Unity / MCP 写线程。

### 会话 9 - 2026-03-21（首个施工窗口尝试时发现 Unity 仍连 shared root）

**用户需求**：
> 用户正式放行，允许进入 Unity / MCP / create-only 写态；但首个施工 checkpoint 只做 `SceneBuild_01 -> Grid + Tilemaps`，进入前需先复核：不在 Play Mode、没有 Compile / Domain Reload 中间态、Console 没有明显冲突信号。

**完成任务**：
1. 按 `sunset-scene-audit + sunset-unity-validation-loop` 做进入前复核：
   - `editor_state` 显示：
     - `is_playing = false`
     - `is_paused = false`
     - `is_changing = false`
     - `is_compiling = false`
     - `is_domain_reload_pending = false`
   - Console 当前只有 1 条 error：
     - `UnityEditor.Graphs.Edge.WakeUp()` 相关 `NullReferenceException`
     - 当前看起来更像 Unity Editor 内部图形系统噪音，而非项目脚本编译/场景报错
2. 读取现有项目里的 `Grid / Tilemap` 组织方式，确认当前项目里 Tilemap 确实挂在带 `Grid` 组件的父物体下。
3. 在用户已放行的前提下尝试进入首个 create-only checkpoint，但立刻发现：
   - `mcpforunity://project/info` 返回 `projectRoot = D:/Unity/Unity_learning/Sunset`
   - 当前 Unity / MCP 连接的是 **shared root**，不是当前 worktree
4. 因为没先识别到这一层根目录错连，`manage_scene create/save` 已经误把 scene 写到 shared root：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity`
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity\SceneBuild_01.unity`
5. 发现后立即停止，**没有继续创建 Grid、没有继续创建 Tilemaps，也没有继续扩施工面**。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：将“等待用户裁定”标记为完成，并新增“Unity 根目录错连 / shared root 误写资产待处理”两个新阻塞。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮首个施工窗口尝试与即时刹车。

**关键结论**：
- 写线程裁定已经到位，但当前真正阻塞已经切换成：**Unity / MCP 仍连着 shared root，而不是当前 worktree**。
- 因此此刻不能把“用户已放行”误解成“现在可以继续写 scene”；必须先修正连接根目录并处理本次误写资产。

**恢复点 / 下一步**：
- 下一步不再继续 scene 施工，而是先处理：
  1. Unity / MCP 根目录错连
  2. shared root 中误建的 `SceneBuild_01` 错位资产

### 会话 10 - 2026-03-21（Unity / MCP 已对准 worktree 并完成状态复核）

**用户需求**：
> 用户要求先不要继续施工，也不要再做 create-only 写入；先把 Unity / MCP 指向 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`，然后仅回报 `project_root`、是否不在 Play Mode、是否不在 Compile / Domain Reload 中间态。只有 `project_root` 确认是 worktree 后，才允许继续 `SceneBuild_01 -> Grid + Tilemaps`。

**完成任务**：
1. 只读复核当前专属现场仍为：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001 @ codex/scene-build-5.0.0-001 @ f1507f59`。
2. 通过 `mcpforunity://instances` 确认当前只有 1 个 Unity 实例在线：`scene-build-5.0.0-001@b4abdcc2b4706d2c`。
3. 调用 `set_active_instance` 将当前 `unityMCP` 客户端显式绑定到该 worktree 实例。
4. 重新读取 `mcpforunity://project/info`，确认当前 `projectRoot = D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`。
5. 重新读取 `mcpforunity://editor/state`，确认：
   - `is_playing = false`
   - `is_paused = false`
   - `is_changing = false`
   - `is_compiling = false`
   - `is_domain_reload_pending = false`
6. 抽样读取最新 Console，当前未见红编译；可见 1 条测试代码 warning 与若干 Inspector/配置缺失提示。

**关键结论**：
- 当前阻塞已经从“Unity / MCP 仍指向 shared root”修正为“Unity / MCP 已重新指向当前 worktree”。
- `editor/state` 本轮仍带有 `stale_status` 提示，但状态字段已经成功返回，不影响这次最小口径确认：当前不在 Play Mode，也不在 Compile / Domain Reload 中间态。
- 本轮按用户要求只交付状态复核，不继续进入 `SceneBuild_01 -> Grid + Tilemaps` 写态。

**恢复点 / 下一步**：
- 下一步在你确认后，直接回到 `SceneBuild_01 -> Grid + Tilemaps` 的最小施工窗口。

### 会话 11 - 2026-03-21（首个 create-only checkpoint 已在 worktree 收口）

**用户需求**：
> 继续当前主线，在不扩施工面的前提下，把 `SceneBuild_01 -> Grid + Tilemaps` 的首个 checkpoint 真正收口。

**完成任务**：
1. 按 `skills-governor + sunset-workspace-router + sunset-scene-audit + sunset-unity-validation-loop` 继续做手工等价闸门与证据回读，先确认当前主线仍是 `1.0.1初步规划 -> SceneBuild_01 首个 checkpoint 收口`。
2. 只读复核当前专属现场：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = f1507f59fa9142a900f6451b43623d14f154bf0c`
3. 回读首个 checkpoint 证据文件：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\drafts\scene-build-5.0.0-001\scene_build_01_checkpoint_result.json`
   - 已确认 `success = true`
   - 已确认 `sceneAssetPath = Assets/000_Scenes/SceneBuild_01.unity`
   - 已确认 `gridHierarchyPath = SceneRoot/Tilemaps/Grid`
4. 回读 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`，确认以下对象已实际落入 YAML：
   - 根层级：`SceneRoot / Systems / Tilemaps / PrefabSetDress / GameplayAnchors / LightingFX / DebugPreview`
   - 网格层级：`Grid`
   - Tilemap：`TM_Ground_Base / TM_Ground_Detail / TM_Path_Water / TM_Structure_Back / TM_Structure_Front / TM_Decor_Front / TM_Occlusion / TM_Logic`
   - `MainCamera`
5. 额外回读关键字段：
   - `MainCamera` 带 `MainCamera` tag
   - Camera 为正交模式，`orthographic size = 6`
   - Tilemap `sortingOrder` 已覆盖从 `0` 到 `7`
6. 删除临时施工脚本：
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01Checkpoint.cs`
   - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01Checkpoint.cs.meta`
7. 只读复核 shared root 历史错位资产：
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity` 仍存在
   - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity\SceneBuild_01.unity` 已不存在

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：把首个 checkpoint 的完成状态、历史阻塞复盘与下一阶段待裁定状态同步进去。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录首个 checkpoint 完成、证据回读与临时脚本清理。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md` - [追加]：把父工作区状态从“等待施工”推进到“首个 checkpoint 已完成”。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_0.md` - [追加]：同步线程层恢复点。

**关键结论**：
- 首个 `SceneBuild_01 -> Grid + Tilemaps` create-only checkpoint 已经在专属 worktree 内完成，不再只是“准备开工”。
- 当前真实交付物是场景资产本身，而不是临时施工脚本；临时脚本已经删除。
- shared root 的同名 `SceneBuild_01.unity` 是否保留，仍需单独裁定；但它不再阻断本 worktree 内已完成的首个 checkpoint。

**恢复点 / 下一步**：
- 当前主线恢复点已从“创建首个骨架”推进到“等待是否扩到地图底稿阶段”的裁定点。

### ?? 12 - 2026-03-21????? v1 ????? SceneBuild_01?
**????**?
> ???? shared root ?????? `codex/scene-build-5.0.0-001` ?? `SceneBuild_01 -> Grid + Tilemaps` ???? Git checkpoint ?????????????????

**????**?
1. ?? `skills-governor + sunset-workspace-router + sunset-scene-audit + sunset-unity-validation-loop` ????????? worktree ????? `cwd / branch / HEAD / git status`??????? `1.0.1???? -> SceneBuild_01 ????`?
2. ?? Unity / MCP ????????? MCP ?????
   - `unityMCP` ???? `Sub2API` HTML ??????????
   - `mcp_unity` ???? `Connection failed: Unknown error`?
3. ?????? `Assets/Editor/CodexSceneBuild01MapDraft.cs` ??????????? `TM_Ground_Base / TM_Ground_Detail / TM_Path_Water` ?? Tilemap ???? YAML ????? 5 ? Tilemap????????????????????
4. ? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` ??? `farmland-clearing v1` ???
   - `TM_Ground_Base`?159 ?????`m_Origin = {-9,-5,0}`?`m_Size = {19,11,1}`??? `Tile_Farm_C0.asset`
   - `TM_Ground_Detail`?107 ??? / ????`m_Origin = {-12,-6,0}`?`m_Size = {24,12,1}`??? `Tile_Farm_C1.asset`
   - `TM_Path_Water`?21 ?????`m_Origin = {4,0,0}`?`m_Size = {7,5,1}`??? `Tile_Water_A/B/C.asset`
5. ????????????
   - ?? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01MapDraft.cs`
   - ?? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01MapDraft.cs.meta`
   - ?? gitignored ???? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\drafts\scene-build-5.0.0-001\scene_build_01_map_draft_command.json`
6. ????????? Tilemap ??????????????? tile_count / origin / size?
   - `TM_Ground_Base`?159 / `(-9,-5,0)` / `(19,11,1)`
   - `TM_Ground_Detail`?107 / `(-12,-6,0)` / `(24,12,1)`
   - `TM_Path_Water`?21 / `(4,0,0)` / `(7,5,1)`

**????**?
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` - [??]???? 3 ??? Tilemap ? YAML ?????????? v1?
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01MapDraft.cs` - [??]?????? Unity ????????????
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\Editor\CodexSceneBuild01MapDraft.cs.meta` - [??]????????? meta?
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_??\5.0.0????\1.0.1????\tasks.md` - [??]??????? v1 checkpoint?
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_??\5.0.0????\1.0.1????\memory.md` - [??]?????????????????

**????**?
? MCP ????????Unity ?????????????????????????????? YAML ????????????? `SceneBuild_01`??? 3 ? Tilemap ?????? shared root??????? / ??? / ????????????? sorting / hierarchy???? RuleTile ??? YAML ???????????????? `Ground_Base` ?? `Tile_Farm_C0` ???????????????? MCP ?????????? RuleTile ????

**?????**?
- `git status --short` ?? `M Assets/000_Scenes/SceneBuild_01.unity`?
- `TM_Ground_Base / TM_Ground_Detail / TM_Path_Water` ?? Tilemap ????? `m_Tiles` ???? `m_TileAssetArray` ????? tile ?????
- ??? `CodexSceneBuild01MapDraft.cs` / `.meta` ?? `Assets/Editor` ???
- ?? MCP ?????????????????????????

**????**?
- [ ] ???? Unity live / Console / PlayMode ????????? YAML ???????? Editor ??????
- [ ] `shared root` ??? `SceneBuild_01.unity` ???????????????
- [ ] ?????????? v1???????????????? Grid checkpoint?

### 会话 13 - 2026-03-21（结构层最小版本完成）

**用户需求**：
> 继续做 `结构层`。当前 `7b92abe0` 已经是稳定 checkpoint，你就在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 里继续推进；这轮口径仍然只是施工推进，不是 Unity live 验收通过，也不要碰 shared root 残留。做完结构层后，再按“最小 checkpoint + clean + 回执”回来。

**当前主线目标**：
- 在专属 worktree 的 `SceneBuild_01` 内继续场景搭建主线，从“地图底稿 v1”推进到“结构层最小版本”，仍不宣称 Unity live 验收通过。

**本轮子任务 / 阻塞**：
- 子任务是完成结构层里的主建筑、边界、围栏、入口和主视觉模块。
- 阻塞没有来自 Git 现场，而是 `unityMCP` 仍被错误路由到 `Sub2API` HTML，无法依赖 Unity live 写入或回读，所以本轮继续走 Scene YAML 兜底。

**完成任务**：
1. 先做等价前置核查，确认当前现场仍是：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 7b92abe0`
2. 只读复核结构层资产入口与样本：
   - `Assets\222_Prefabs\House\House 3_0.prefab`
   - `Assets\222_Prefabs\Farm\T\Farm_27.prefab`
   - `Assets\222_Prefabs\Farm\T\Farm_28.prefab`
   - `Assets\222_Prefabs\Farm\T\Farm_29.prefab`
3. 在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` 内完成结构层最小写入：
   - 在 `SceneRoot/PrefabSetDress` 下新增 `Structure_Farmstead`
   - 新增主建筑 `Structure_House_Main`
   - 新增围栏与入口：`Fence_North_01 / Fence_North_02 / Fence_South_01 / Fence_South_02 / Fence_East_Lower / Fence_East_Upper`
4. 主建筑方案采用 `House 3_0` 的 Sprite + PolygonCollider；围栏方案采用 `Farm/T` 的横段与竖段 prefab 序列化参数直接落入 scene。
5. 文件级回读已确认：
   - `PrefabSetDress` 已包含 `Structure_Farmstead`
   - `Structure_Farmstead` 已包含 7 个子物件
   - 新增对象的 `fileID` 唯一
   - 所有 `m_Father` 引用均指向存在的 Transform
6. 本轮没有进入 Unity live 验收，也没有处理 shared root 残留。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` - [修改]：追加结构层最小版本的主建筑、围栏与入口。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [修改]：将“地图底稿 / 结构层”标记为完成，并追加结构层最小收口清单。
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮结构层施工、MCP 失败口径与恢复点。

**关键结论**：
- 结构层最小版本已经在 worktree 内落地，当前 `SceneBuild_01` 不再只有骨架和底稿，也已经具备第一批结构视觉锚点。
- 本轮的“结构层完成”是施工推进结论，不是 Unity live / Console / PlayMode 闭环结论。
- MCP 传输失败仍然是独立工具债务；它没有阻止本轮 Scene YAML 落地，但它仍阻断 Unity 侧自动验证闭环。

**恢复点 / 下一步**：
- 主线恢复点已从“继续做结构层”推进到“进入装饰层最小版本”。
- 下一步最小动作是继续补装饰层：植被、小物件与中景节奏；之后再集中处理逻辑层与 Unity live 验证。

### 会话 18 - 2026-03-21（装饰层最小版本完成，Unity 本地 MCP 已活但当前 Codex 会话仍未回正）

**用户需求**：
> 用户重新开启 Unity 侧 MCP，希望确认当前状态；主线仍是继续 `SceneBuild_01` 的装饰层，边界保持在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 内、最小真实推进、不宣称 Unity live 验收通过。

**完成任务**：
1. 按 `skills-governor + sunset-workspace-router + sunset-scene-audit + sunset-unity-validation-loop` 继续做手工等价闸门，确认当前现场仍是 `codex/scene-build-5.0.0-001 @ 11d99609`。
2. 回读 `C:\Users\aTo\.codex\config.toml` 与 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Library\MCPForUnity\TerminalScripts\mcp-terminal.cmd`，确认 Unity 本地 HTTP MCP 服务确实配置为 `http://127.0.0.1:8888/mcp`。
3. 通过本机只读探测确认：
   - `127.0.0.1:8888` 当前由本地 `python` 进程监听；
   - 直接访问 `http://127.0.0.1:8888/mcp` 已返回 MCP 协议级错误，说明 Unity 侧本地服务本身已活。
4. 但当前 Codex 会话里的 `unityMCP` 工具仍返回 `Sub2API` HTML，`mcp__mcp_unity__*` 仍返回 `Connection failed: Unknown error`；因此本轮不升级到 Unity / MCP live 写态或 live 验证，继续遵守用户口径走 Scene YAML 兜底。
5. 在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` 的 `SceneRoot/PrefabSetDress` 下新增 `Decor_Farmstead`，落入 7 个纯视觉装饰对象：
   - `Decor_Tree_WestBig_01`
   - `Decor_Sapling_SouthWest_01`
   - `Decor_Sapling_NorthEast_01`
   - `Decor_Rock_NorthWest_01`
   - `Decor_Rock_SouthEast_01`
   - `Decor_Prop_Yard_01`
   - `Decor_Prop_Yard_02`
6. 本轮装饰层明确只使用 `Transform + SpriteRenderer`，不带脚本、不带 Collider，避免提前越界到逻辑层。
7. 文件级回读已确认：
   - `PrefabSetDress` 已新增子挂点 `Decor_Farmstead`
   - 7 个装饰对象均挂在 `Decor_Farmstead` 下
   - 新增 `fileID` 全部唯一

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`

**关键结论**：
- Unity 侧本地 MCP Server 已经活起来，但“当前这条 Codex 会话”仍没有真正吃到正确的 Unity live 路由，所以当前最准确的口径仍然是：**本地服务正常，当前会话工具仍未回正**。
- `SceneBuild_01` 的装饰层最小版本已经在 worktree 内稳定落地，且没有越界去碰 shared root，也没有冒充 Unity live 验收结论。

**恢复点 / 下一步**：
- 主线恢复点已从“继续做装饰层”推进到“进入逻辑层最小版本，或先修当前 Codex 会话的 Unity / MCP live 路由”。
- 如果下一轮仍按当前边界继续施工，最小动作就是补逻辑层（碰撞 / 遮挡 / 锚点）；如果你要求先闭合 live 验证，则应先处理当前 Codex 会话的 MCP 路由问题。

### 会话 19 - 2026-03-21（按 shared-root Prompt 对照 Primary 后重做 Decor_Farmstead）

**用户需求**：
> 用户下发 `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_恢复开发总控_01\可分发Prompt\scene-build_装饰层纠偏与Primary参考重做.md`，要求先只读对照 shared root 的 `Assets/000_Scenes/Primary.unity`，再回到当前 worktree 重做 `SceneBuild_01` 的装饰层，不允许只口头总结、不允许继续把装饰做成散点堆物。

**按 scene-modification-rule 收敛后的五段式判断**：
1. 原有配置：
   - `SceneBuild_01` 里的 `Decor_Farmstead` 直接挂了 7 个装饰对象，虽然没有越界到逻辑层，但在层级上仍是“同一父节点下的散点摆放”。
2. 问题原因：
   - 缺少对地表 / 边界 / 入口 / 留白的组织关系回应；
   - 建筑周边没有形成明确的“框景边”“院内生活组”“东侧边界组”；
   - 结果就是装饰像附加物，而不是环境叙事的一部分。
3. 建议修改：
   - 先以 `Primary.unity` 为只读参考，总结出“先地形分块、再道具叠加”“树石成簇”“院内道具服务入口与留白”三条原则；
   - 再把 `Decor_Farmstead` 重构为 3 个组簇而不是 7 个散点。
4. 修改后效果：
   - 西北框景组负责把左上角和屋后边缘立住；
   - 东侧边界组只占上段，不堵东侧入口；
   - 院内生活组回到房前右侧，让院落关系更像一个小场景而不是临时摆件。
5. 对原有功能的影响：
   - 仍然只改纯视觉 `Transform + SpriteRenderer`；
   - 仍未触碰 shared root 残留，也未宣称 Unity live 验收通过；
   - 对后续逻辑层只增加了更清晰的结构锚点，没有提前引入脚本 / Collider 风险。

**完成任务**：
1. 只读解析 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`，确认 `Primary` 的 farm 组织不是“先扔 props”，而是：
   - `SCENE/LAYER 1/Tilemap/Layer 1 - Farmland_Center`
   - `SCENE/LAYER 1/Tilemap/Layer 1 - Farmland_Border`
   - `SCENE/LAYER 1/Tilemap/Layer 1 - Farmland_Water`
   - `SCENE/LAYER 1/Props/Farm`
   - `SCENE/LAYER 1/Test Tree`
   - `SCENE/LAYER 1/Test Rock`
2. 从 `Primary` 抽出 3 条直接用于本轮纠偏的组织原则：
   - 先把地表 / 边界 / 水体分块立住，再让装饰覆盖其上；
   - 树 / 石头以成组簇的方式参与边角和外缘，而不是随机散点；
   - 建筑周边道具只围绕院内生活与入口动线，不占主通路。
3. 在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` 中把 `Decor_Farmstead` 重构为：
   - `DecorCluster_NorthWestFrame`
   - `DecorCluster_EastBorder`
   - `DecorCluster_YardLife`
4. 将原先 7 个装饰对象重新编组并纠偏命名 / 坐标：
   - `Decor_Tree_WestBig_01`
   - `Decor_Rock_WestBorder_01`
   - `Decor_Plant_EastBorder_01`
   - `Decor_Rock_EastBorder_01`
   - `Decor_Plant_Yard_01`
   - `Decor_YardSupplies_01`
   - `Decor_YardSupplies_02`
5. 文件级回读确认：
   - 3 个组簇已全部挂在 `Decor_Farmstead` 下；
   - 世界坐标上，东侧入口仍位于 `Fence_East_Lower(11,-0.5)` 与 `Fence_East_Upper(11,3)` 之间保持通行；
   - 东侧装饰重心被抬到 `(10.75, 3.65)` 一带，不再把下段入口堵死。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`

**关键结论**：
- 这次不只是“再摆几个物件”，而是把装饰层从散点堆叠，纠偏成了 3 个服务地形、入口和院内生活逻辑的组簇。
- 当前最准确口径仍然是：**装饰层纠偏重做已完成，SceneBuild_01 的审美方向已明显向 Primary 靠拢，但这仍不是 Unity live 验收通过结论。**

**恢复点 / 下一步**：
- 主线恢复点已从“装饰层最小版”推进到“装饰层已完成一轮 Primary 参考纠偏重做”。
- 下一步最小动作：进入逻辑层最小版本，或先修当前 Codex 会话的 Unity / MCP live 路由。

### 会话 20 - 2026-03-21（装饰层纠偏进入最小 checkpoint 收口）

**用户需求**：
> 继续按 worktree 口径推进，但这轮先不要扩到逻辑层；先把 `Primary` 参考下完成的装饰层纠偏收成一次最小 checkpoint，并按最小格式回执。

**完成任务**：
1. 复核当前现场仍为：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 0717172a`
2. 复核当前 dirty 仍限定在：
   - `Assets\000_Scenes\SceneBuild_01.unity`
   - 当前子工作区 `tasks.md / memory.md`
   - 父工作区 `memory.md`
   - 线程记忆 `memory_0.md`
3. 保持本轮施工边界不变：
   - 不回 shared root 写任何业务内容；
   - 不宣称 Unity live 验收通过；
   - 不把 `unityMCP` 当前会话未回正的状态包装成已恢复。
4. 确认当前已达到“最小白名单 sync 收口”条件，下一动作只应是 checkpoint，同步后再决定是否进入逻辑层。

**关键结论**：
- `Decor_Farmstead` 的纠偏重做已经不是 WIP 草稿，而是可以被收成稳定 checkpoint 的一轮真实推进。
- 本轮收口目标是“先 clean，再回执”，不是继续扩大施工面。

**恢复点 / 下一步**：
- 收口完成后，主线恢复点保持为：`SceneBuild_01` 已完成一轮 `Primary` 参考下的装饰层纠偏重做。
- 后续是否进入逻辑层，等待下一轮明确放行后再推进。

### 会话 21 - 2026-03-21（逻辑层最小版本继续施工）

**用户需求**：
> 继续使用 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`，不要回头再解释装饰层，也不要处理 shared root 残留；目标是把 `SceneBuild_01` 从“结构 + 装饰”推进到“逻辑层最小版本”。

**按 scene-modification-rule 的五段式判断**：
1. 原有配置：
   - `GameplayAnchors / LightingFX / DebugPreview` 仍为空；
   - `Systems` 目前只有 `MainCamera`；
   - `Structure_House_Main` 已自带 `PolygonCollider2D`，但围栏仍只有 `SpriteRenderer`；
   - 东侧入口、院内站位和后续触发点仍停留在“视觉成立、逻辑未落地”的状态。
2. 问题原因：
   - 当前 scene 已具备地表、结构和装饰层，但缺少可接手的逻辑层最小框架；
   - 东侧入口和院内生活区已有明确空间关系，却没有对应锚点、阻挡或触发结构；
   - `LightingFX / DebugPreview / Systems` 作为后续施工承接层，还没有真正有用途的挂点。
3. 建议修改：
   - 在 `GameplayAnchors` 下补 4 个真实用途锚点；
   - 在 `Systems` 下补 `LogicLayer_Farmstead`，落 4 个围栏阻挡体与 2 个触发区；
   - 在 `LightingFX / DebugPreview` 下各补 1 个最小挂点。
4. 修改后效果：
   - `SceneBuild_01` 不再只有“看起来像院落”，而是具备最小可延续的出入口 / 可站位 / 触发框架；
   - 东侧入口保留开口，但围栏的阻挡逻辑已能与结构层对齐；
   - 后续光照、调试视角和系统接线都有明确落点。
5. 对原有功能的影响：
   - 本轮仍只改当前 worktree 的 scene YAML，不碰 shared root；
   - 不把逻辑层推进表述成 Unity live 验收通过；
   - 当前 Codex 会话中的 `unityMCP` 仍返回 `Sub2API` HTML，所以继续采用 Scene YAML 兜底。

**完成任务**：
1. 在 `GameplayAnchors` 下新增：
   - `Anchor_Spawn_EastApproach`
   - `Anchor_Entry_EastGate`
   - `Anchor_Stand_YardCenter`
   - `Anchor_Interact_HouseYardSide`
2. 在 `Systems` 下新增 `LogicLayer_Farmstead`，并补齐：
   - `Blocker_NorthFence`
   - `Blocker_SouthFence`
   - `Blocker_EastFence_Lower`
   - `Blocker_EastFence_Upper`
   - `Trigger_EastGateApproach`
   - `Trigger_YardCore`
3. 在 `LightingFX / DebugPreview` 下新增：
   - `LightAnchor_YardWarmth`
   - `PreviewFocus_FarmsteadLogic`
4. 通过文件级回读确认：
   - 新增 `fileID` 唯一；
   - `m_Father` 全部指向已存在 Transform；
   - `GameplayAnchors / Systems / LightingFX / DebugPreview` 的新子节点均已挂入；
   - 东侧入口通行口仍保持在 `Fence_East_Lower` 与 `Fence_East_Upper` 之间。
5. 只读复核 MCP 现状：`unityMCP/manage_scene` 仍返回 `Sub2API` HTML，说明这轮依然不能宣称 Unity live 闭环恢复。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`

**关键结论**：
- `SceneBuild_01` 的逻辑层最小版本已经落地，不再只有视觉层面的“农舍院落”，而是具备了后续可接手的锚点、围栏阻挡与触发框架。
- 当前最准确口径仍然是：**逻辑层施工推进已完成最小版本，但这仍不是 Unity live / Console 验收通过结论。**

**恢复点 / 下一步**：
- 主线恢复点已从“装饰层纠偏完成”推进到“逻辑层最小版本完成”。
- 下一步最小动作：做一轮施工自检（优先 Unity / MCP live 恢复后回读；若仍不可用，则继续补 Scene YAML 级验证与交付收口）。

### 会话 22 - 2026-03-21（回读自检与高质量初稿收口）

**用户需求**：
> 不回头再解释逻辑层，也不处理 shared root 残留；本轮目标是把当前 `SceneBuild_01` 做一次真正的回读自检，并在不失控扩面的前提下收成“可继续精修的高质量初稿”。

**验证范围**：
- Scene / prefab / serialized config change；
- 重点验证当前 scene 的层级、命名、挂点、逻辑对象组织和明显布局失衡风险；
- 不做脚本重编译，不冒充 Unity live 闭环已经恢复。

**已执行检查**：
1. Scene YAML 层级回读：
   - `SceneRoot` 下保持 `GameplayAnchors / DebugPreview / Systems / LightingFX / PrefabSetDress / Tilemaps` 六大根层；
   - `Systems` 下为 `MainCamera + LogicLayer_Farmstead`；
   - `GameplayAnchors` 下为 4 个 `Anchor_*`；
   - `PrefabSetDress` 下为 `Structure_Farmstead + Decor_Farmstead`；
   - `LogicLayer_Farmstead` 下为 4 个 `Blocker_*` 与 2 个 `Trigger_*`。
2. 逻辑对象一致性检查：
   - 4 个 `Blocker_*` 全部为 `BoxCollider2D` 且 `m_IsTrigger = 0`；
   - 2 个 `Trigger_*` 全部为 `BoxCollider2D` 且 `m_IsTrigger = 1`；
   - 锚点、光照挂点、调试挂点没有混入无验证价值的空节点命名。
3. MCP / Unity 传输层探测：
   - `unityMCP/manage_scene` 仍返回 `Sub2API` HTML；
   - 旧 `mcp_unity/get_console_logs` 仍报 `Connection failed: Unknown error`。
4. Unity 本机日志只读检查：
   - 读取 `C:\Users\aTo\AppData\Local\Unity\Editor\Editor.log` 最近 200 行；
   - 未见新的显式 `error / exception` 关键字，仅见编译耗时记录。

**项目级结果**：
- 当前 `SceneBuild_01` 的结构层、装饰层、逻辑层已经形成清楚的父子组织与命名体系；
- 没有发现需要本轮继续扩面的明显“穿帮、命名失控、逻辑挂点乱挂”问题；
- 以当前 YAML 证据看，场景已经可以作为“可继续精修的高质量初稿”交付。

**MCP 级结果**：
- 当前会话的 `unityMCP` 仍未回正；
- 因此这轮只能确认“YAML 回读自检通过 + 本机日志未见新显式错误”，不能确认“Unity live / Console 验收通过”。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md`

**关键结论**：
- 当前场景已达到“项目经理直接看也不至于太糙”的初稿口径，且不是靠继续堆物件换来的。
- 最准确口径是：**高质量初稿已收口，但 Unity live / MCP 验证闭环仍未恢复。**

**恢复点 / 下一步**：
- 主线恢复点已从“逻辑层最小版本完成”推进到“高质量初稿已收口，待 live 验证链恢复或后续精修”。
- 下一步最小动作：若优先闭环，则修当前 Codex 会话的 Unity / MCP live 路由；若优先交付后续施工，则基于当前初稿继续做精修需求拆分。

### 会话 23 - 2026-03-21（回读 `900_开篇` 最新剧情，并把场景理解重新对齐到 `spring-day1`）

**用户需求**：
> 先切回当前工作树，完整阅读 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇` 下的最新剧情 / 实现文档，再明确说明“我现在实际要搭建的场景是什么”；随后又补充一份本应指导本线程的 prompt，强调这条线后续不要把旧 worktree 当长期终点，而应以 `D:\Unity\Unity_learning\Sunset @ main` 为唯一真实基线，把场景继续往 `spring-day1` 剧本承载能力上推。

**当前主线目标**：
- 不改变 `scene-build` 主线：继续围绕 `SceneBuild_01` 做场景承载能力建设。
- 但本轮不是继续施工，而是把“当前场景到底在承载春1日的哪一段剧情、空间应该长成什么样”先重新讲清楚。

**本轮子任务 / 阻塞**：
- 子任务是跨工作区只读回看和剧情对齐，不做任何 scene / prefab / 脚本写入。
- 阻塞不是 Git 或 Unity 工具，而是如果不先把 `900_开篇` 的剧情基线和 `5.0.0场景搭建` 的现状重新对齐，后续精修很容易继续走成“泛化摆场”而不是“剧情承载”。

**完成任务**：
1. 复核当前 worktree 现场：
   - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 0a14b93c`
   - `git status = clean`
2. 只读回看 shared root 下 `900_开篇` 的两条主线文档：
   - `0.0.1剧情初稿`：确认春1日从 `14:00 坠落与相遇` 到 `17:00 归途与夜间提醒` 的完整剧情节拍；
   - `spring-day1-implementation`：确认需求、任务、记忆与阶段报告中对“村庄 / 小屋 / 农田 / 教学 / UI / NPC 引导”的正式承载要求。
3. 只读回看当前 worktree 下 `5.0.0场景搭建/1.0.1初步规划` 的 `requirements.md / design.md / tasks.md / 资产普查.md / memory.md`，把当前已落地的 `SceneBuild_01` 与春1日剧情重新对齐。
4. 同时按用户补充 prompt 复核 shared root 当前基线：
   - `D:\Unity\Unity_learning\Sunset @ main`
   - `HEAD = f1ac9872`
   - `git status = clean`
   并把它理解为后续“成果应尽快回到 main 精修”的策略信号，而不是继续把旧 worktree 当长期终点。

**本轮稳定结论**：
1. 当前 `SceneBuild_01` 不是“整座落日村总览场景”，也不是“矿洞口坠落场景”或“饭馆场景”。
2. 它现在最准确的定位是：**春1日 `14:20 进入村庄` 之后，到 `15:10 耕种/砍树教学` 这一段的“废弃小屋 + 院落 + 小块农田”的独立承载场景**。
3. 这个场景需要服务的不是泛化美术展示，而是以下剧情动作的空间支撑：
   - 玩家被引导抵达居住点；
   - 进入 / 离开小屋；
   - 屋侧工作台触发闪回；
   - 院内站位对话与视线焦点；
   - 东侧主入口的进入与 NPC 带路；
   - 农田教学与砍树教学的落点；
   - 后续床铺 / 自由时段 / 次日延展的接线可能。
4. 因此，当前 scene 的空间组织必须继续坚持：
   - 东侧是主进入方向，而不是随便留个缺口；
   - 中央院落需要保留站位和视线焦点，不能被杂物堵死；
   - 建筑右前侧是“生活 / 教学 / 互动”区，不应散乱；
   - 西北框景与东侧边界负责收住画面和边界，不是纯装饰堆物；
   - 农田与工具教学落点应明确可读，而不是只剩抽象地块。
5. 从剧情约束上看，当前 scene 后续精修必须持续遵守：
   - 房屋是“废弃闲置房”，不是谁的私人豪宅；
   - 村落技术水平是统一落后的基础水平；
   - 第一天作物固定是花菜；
   - 血条先于精力条出现，但这属于剧情 / UI 链，不应反向逼坏场景空间；
   - 骷髅兵威胁属于矿洞口段落，不应误塞进这个院落 scene 的核心视线。

**修改文件**：
- `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [追加]：记录本轮剧情回读、scene 定位纠偏与 `main` 基线信号。

**关键结论**：
- `SceneBuild_01` 当前最准确的叙事定位已经重新钉死：它承载的是春1日“安置到废弃小屋并开始生活 / 教学”的农舍院落段，而不是整村地图。
- 这意味着后续精修的正确方向不是“再摆更多好看的东西”，而是继续把入口、院落、工作台、农田教学、NPC 引导和视线焦点做成真正服务剧本的空间。
- 用户新补充的 prompt 已被吸收为策略信号：旧 worktree 只应作为过渡承载面，后续有效成果应尽快回到 `main` 继续精修。

**恢复点 / 下一步**：
- 当前主线恢复点已从“高质量初稿已收口”推进到“高质量初稿的剧情定位已重新对齐 spring-day1 剧本”。
- 下一步如果继续施工，优先级应从“泛化装饰”切到“主入口 / 建筑与院落 / 工作台与农田教学落点 / NPC 引导与视线焦点”的精修，而不是继续横向扩面。

### 会话 24 - 2026-03-21（迁移前冻结回执）

**用户需求**：
> 读取 `scene-build_当前任务回执与迁移前冻结.md`，不要再新开下一层施工；先把手头这一刀收口到可描述状态，再按固定字段给出迁移前冻结回执。

**当前主线目标**：
- 保持 `scene-build` 主线不变，但本轮不进入新施工，只对当前 worktree 现场做冻结前状态说明。

**本轮子任务 / 阻塞**：
- 子任务是冻结前回执，不是迁移执行，也不是继续精修 scene。
- 当前没有新的场景施工阻塞；唯一需要如实说明的是当前 worktree 仍有 3 个记忆文件 dirty。

**完成任务**：
1. 只读读取 prompt：`D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\scene-build_当前任务回执与迁移前冻结.md`
2. 复核当前真实现场：
   - `project_root = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - `branch = codex/scene-build-5.0.0-001`
   - `HEAD = 0a14b93c`
   - `git status = dirty`
3. 确认当前这一刀已经完成的内容不是新施工，而是：
   - 回读 `900_开篇` 最新剧情 / 实现文档；
   - 把 `SceneBuild_01` 的定位重新钉死为春1日“废弃小屋 + 院落 + 小块农田 + 教学互动”承载场景；
   - 把“后续应尽快对齐 main，而不是长期悬在旧 worktree”写回当前记忆链。
4. 确认当前 dirty 仅为：
   - `.kiro/specs/900_开篇/5.0.0场景搭建/1.0.1初步规划/memory.md`
   - `.kiro/specs/900_开篇/5.0.0场景搭建/memory.md`
   - `.codex/threads/Sunset/Skills和MCP/memory_0.md`

**关键结论**：
- 当前这一刀已经处于“可描述、可冻结”的状态，没有半写的 scene / prefab / 脚本改动。
- 当前如果立即冻结，缺的不是施工尾项，而是后续若要迁移，应由治理侧决定如何处理这 3 个记忆文件的 dirty。
- 因为当前 dirty 只在记忆层，且没有 Unity / MCP live 写进行中，所以 `can_freeze_now = yes`；但是否直接判定 `migration_ready = yes`，取决于治理侧是否接受“带着记忆 dirty 迁移”。
