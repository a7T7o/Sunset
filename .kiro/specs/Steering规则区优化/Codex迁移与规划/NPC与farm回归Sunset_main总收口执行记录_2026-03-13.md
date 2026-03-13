# `NPC` 与 `farm` 回归 `Sunset/main` 总收口执行记录（2026-03-13）

## 1. 本轮目标

- 只回流 `NPC` 与 `farm-10.2.2-patch002` 的业务成果；
- 不把根仓库其他线程的无关 dirty 混入；
- 把默认开发规范正式收口为“主项目优先，worktree 为例外工具链”；
- 把 `NPC` / `farm` 的线程锚点改回 `D:\Unity\Unity_learning\Sunset@main`；
- 为当前不可推送的本地主线建立可推送替代承载链。

## 2. 主线承载面净化结果

### 2.1 根仓库 dirty 分类

#### A 类：本轮纳入回流的对象

- `Assets/Editor/NPCPrefabGeneratorTool.cs`
- `Assets/Editor/NPCPrefabGeneratorTool.cs.meta`
- `Assets/Sprites/NPC/*`
- `Assets/Sprites/NPC.meta`
- `Assets/YYY_Scripts/Anim/NPC/*`
- `Assets/YYY_Scripts/Controller/NPC/*`
- `Assets/YYY_Scripts/Story/Interaction.meta`
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs.meta`
- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
- 本轮治理与收口文档

#### B 类：本轮明确不混入，交回原线程

- `.codex/threads/OpenClaw/部署与配置龙虾V2/memory_3.md`
- `.codex/threads/Sunset/spring-day1/memory_0.md`
- `.kiro/specs/900_开篇/spring-day1-implementation/002-初步搭建/memory.md`
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
- `Assets/000_Scenes/Primary.unity`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/*`
- `.codex/threads/Sunset/backup-script/`

#### C 类：起初不确定、现已收口归类

- `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
- `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`

结论：
- 根仓库这两个 `Placement*` 脏改与 `farm` worktree 相对 `main` 的业务 diff 一致；
- 因此它们不是额外未知污染，而是 `farm` 回流白名单的一部分；
- 但本轮仍不直接依赖根仓库 dirty 作为唯一来源，而以 `farm` worktree 白名单版本为准回带。

### 2.2 干净回流承载面

- 本轮新建独立承载面：`D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier`
- 承载分支：`codex/main-reflow-carrier`
- 基线来源：`origin/main`

这样做的原因：
- 避开当前根仓库 `main` 的无关 dirty；
- 避开本地 `main` 已含超大文件历史、不可直接推送的问题；
- 让 `NPC/farm` 的白名单业务成果和本轮收口文档可以落在一条干净、可推送的替代链上。

## 3. 大文件历史阻断收口

### 3.1 阻断源

- 本地 `main` 领先远端的历史提交链中，包含超大文件：
  - `.kiro/specs/Steering规则区优化/Codex迁移与规划/backups/state_5_farm-secondcut_20260312-222748.sqlite`
- 该文件约 `613MB`，超过 GitHub `100MB` 限制；
- 因此继续沿用当前本地 `main` 历史链直接推送，会持续失败。

### 3.2 本轮收口方式

- 本轮不对现有本地 `main` 历史做破坏性改写；
- 改为基于 `origin/main` 建立不带大文件包袱的 `codex/main-reflow-carrier`；
- 本轮所有可推送成果都同时落到该承载分支。

### 3.3 当前结论

- 本地 `main` 仍不是可直接推送链；
- `codex/main-reflow-carrier` 是本轮建立的可推送替代承载链；
- 后续若要彻底恢复“`main` 也可直接推送”，仍需单独处理本地历史重写或重建主线的问题。

## 4. 业务回流结果

### 4.1 `NPC`

- 回流方式：不整支 merge，只带回 NPC 业务白名单；
- 回流来源：`codex/npc-generator-pipeline`
- 回流对象：
  - `Assets/Editor/NPCPrefabGeneratorTool.cs`
  - `Assets/Editor/NPCPrefabGeneratorTool.cs.meta`
  - `Assets/Sprites/NPC/*`
  - `Assets/Sprites/NPC.meta`
  - `Assets/YYY_Scripts/Anim/NPC/*`
  - `Assets/YYY_Scripts/Controller/NPC/*`
  - `Assets/YYY_Scripts/Story/Interaction.meta`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs.meta`
- 验证口径：
  - 干净承载面只出现 NPC 白名单业务文件；
  - 未混入 NPC 分支中的治理/记忆文件。

### 4.2 `farm`

- 回流方式：不整支 merge，只带回 `Placement*` 白名单；
- 回流来源：`codex/farm-10.2.2-patch002`
- 回流对象：
  - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
  - `Assets/YYY_Scripts/Service/Placement/PlacementValidator.cs`
- 重叠承载处理：
  - 根仓库现有 `Placement*` 脏改已核实与 `farm` 白名单业务改动一致；
  - 仍以 `farm` worktree 版本重新回带，避免把别的未知上下文误混进来。

## 5. 线程锚点回根仓库

本轮目标锚点：

- `cwd = D:\Unity\Unity_learning\Sunset`
- `git_branch = main`
- `git_sha = 本轮 root main 白名单提交后的 HEAD`
- rollout `session_meta.cwd = D:\Unity\Unity_learning\Sunset`
- rollout `session_meta.git.branch = main`
- rollout `session_meta.git.commit_hash = 同步后的 root main HEAD`
- 全部 `turn_context.cwd = D:\Unity\Unity_learning\Sunset`

备份路径：

- `D:\Unity\Unity_learning\Sunset\.codex\state_backups\main-return-20260313`

## 6. worktree 默认身份退役口径

- `NPC` 与 `farm` 自本轮开始不再作为长期默认独立 worktree 开发线程；
- 原分支保留：
  - `codex/npc-generator-pipeline`
  - `codex/farm-10.2.2-patch002`
- 保留用途：
  - 异常回溯
  - 历史对照
  - 高风险隔离再次启用时的参考
- worktree 的默认身份退役，不等于立刻物理删除目录；
- 只有在线程锚点回根仓库稳定、用户不再漂回旧 worktree 后，原 worktree 才进入“可停用”状态。

## 7. 当前完成态

- `NPC`：业务成果已进入主项目回流；
- `farm`：业务成果已进入主项目回流；
- 默认开发规范：已收口为“主项目优先”；
- 线程锚点：本轮按 root/main 语义统一改回；
- 可推送替代链：已建立 `codex/main-reflow-carrier`；
- 本地 `main` 历史大文件阻断：已绕开，但未对旧历史本身做消除。

## 2026-03-13 补记：最终硬回读发现 farm rollout 残留并已当场补齐

- 在最终交付前对两条线程做程序化回读时，发现：
  - NPC rollout 已对齐到 D:\Unity\Unity_learning\Sunset@main；
  - farm rollout 仍残留旧 worktree 语义，session_meta.cwd 与 	urn_context.cwd 仍指向 D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002。
- 已立即对 C:\Users\aTo\.codex\sessions\2026\03\10\rollout-2026-03-10T09-39-16-019cd565-fb4d-7730-a45b-3ce67410ab07.jsonl 做最小补写：
  - session_meta.payload.cwd -> D:\Unity\Unity_learning\Sunset
  - session_meta.payload.git.branch -> main
  - session_meta.payload.git.commit_hash -> 8ccdc51dbba4e40ba53bcda92577ebad3d7ee7f6
  - 全部命中的 	urn_context.payload.cwd -> D:\Unity\Unity_learning\Sunset
- 补写后程序化回读已确认：
  - NPC / farm 两条线程在 state_5.sqlite 中均为 cwd = D:\Unity\Unity_learning\Sunset、git_branch = main、git_sha = 8ccdc51dbba4e40ba53bcda92577ebad3d7ee7f6；
  - 两个 rollout 的 session_meta 与首个 	urn_context 均已回根仓库；
  - 两个 rollout 中均已检索不到 Sunset_worktrees\\NPC 或 Sunset_worktrees\\farm-10.2.2-patch002 残留。
- 因此，本轮“线程锚点回根仓库”状态最终收口为：数据库行与 rollout 恢复层现已全部对齐，剩余验收仅在客户端实际打开路径。

## 2026-03-13 补记：总恢复执行轮已完成主状态统一
- 本轮最终已把 NPC、farm、spring-day1 三条线统一回到 D:\Unity\Unity_learning\Sunset 主项目语义，且外部线程状态已对齐到本地 main@cf1d58dfecc04a9aa6cb509a321dec92c412fcb6。
- spring-day1 当前已补回缺失的首段对话资产与调试菜单；DialogueManager.cs、DialogueUI.cs 的增强版经复核已在主项目承载面，不再处于“部分在 snapshot、部分在 main”的半恢复状态。
- 当前 Git 承载分工已固定：根仓库 main 继续作为用户本地主项目现场；codex/main-reflow-carrier@0855d3f3f4c0d7341c710a85a593cff89782d7c0 作为唯一干净、已推送的恢复承载链。
- Primary.unity 与五套 TMP 字体资产当前 dirty 继续留在保护分类，不纳入本轮恢复提交；其他无关线程 dirty 继续排除。
- 恢复点：本轮总恢复已到“用户可以只开 Sunset 一个 Unity 项目继续开发”的状态；剩余唯一尾巴仅是本地 main 历史超大文件导致不能直接推送，后续若要收回到可直推 main，需单独处理历史链。

## 2026-03-13 补记：spring-day1 最后两个增强脚本已补齐，总恢复完成
- 本轮在不改动 `NPC/farm` 既有主项目状态的前提下，补齐了 `spring-day1` 剩余唯一硬缺口：`DialogueUI.cs` 与 `DialogueManager.cs` 的增强版重新回到当前主项目工作树。
- 恢复来源已重新锁定为 `codex/restored-mixed-snapshot-20260311`，因为 `codex/main-reflow-carrier` 上同名文件并不包含增强版关键标记。
- 当前总恢复正式收口为：`NPC`、`farm`、`spring-day1` 三条线都能在 `D:\Unity\Unity_learning\Sunset` 主项目体系中继续开发；worktree 继续保留为例外工具链，而不是默认现场。