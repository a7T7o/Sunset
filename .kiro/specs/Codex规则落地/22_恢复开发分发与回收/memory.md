# 阶段22 Memory

## 2026-03-18｜阶段22建立：恢复开发分发与回收

**当前主线目标**
- 当前主线已经从“修 shared root / 修闸机”切换到“把恢复开发真正组织起来并回收结果”。
- 用户明确要求：
  - 这批 prompt 不应再漂在根目录文件里
  - 应有明确工作区
  - 应有专门收取线程回应的文件夹
  - 用户后续只需要发 prompt，不需要再手工补路径和格式

**本阶段定位**
- `22_恢复开发分发与回收` 是新的运营工作区。
- 它不重复实现 shared root 闸机，而是承接：
  - 线程 prompt 发放
  - 线程回收文件落点
  - 阶段一 / 阶段二调度
  - 剩余未完成项清单化

**本轮已完成**
- 新建阶段 22 工作区。
- 新建：
  - `可分发Prompt/`
  - `线程回收/`
- 为 5 个线程建立可直接发送的 prompt 文件。
- 为 5 个线程建立固定回收文件模板。
- 准备把旧的根层 `恢复开发唤醒与准入Prompt_2026-03-18.md` 降级为路由页，避免双源漂移。

**当前口径**
- 并发允许的是阶段一只读回收。
- 阶段二准入依旧必须串行。
- 任何线程只要进入 Unity / MCP / Play Mode，做完必须退回 `Edit Mode` 后再交还。

**恢复点 / 下一步**
- 同步本阶段文件到 `main`。
- 用户按本阶段 prompt 文件分发。
- 收回各线程回收文件后，再逐条裁定阶段二准入。

## 2026-03-18｜导航检查已完成阶段一只读回收

**当前主线目标**
- 当前主线仍是“把恢复开发真正组织起来并回收结果”，不是继续扩写 shared root 闸机。
- 本轮子任务是：接收 `导航检查` 的阶段一只读回包，并据此判断是否值得进入阶段二。

**本轮已完成**
- 已读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt\导航检查.md` 并按阶段一要求完成只读 preflight。
- 已在 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md` 填入阶段一区块。
- 已复核当前 live 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main`
  - `14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`
  - `git status --short --branch = ## main...origin/main`
- 已复核导航线程当前仍停在 `1.0.0初步检查` 审计基线，live 中没有 `2.0.0整改设计`、`requirements.md`、`design.md`、`tasks.md`。
- 已复核导航核心链仍保持旧审计结论：
  - `GameInputManager` 仍重且仍使用 `OverlapPointAll`
  - `PlayerAutoNavigator` / `NavGrid2D` 仍使用 `OverlapCircleAll`
  - `PlacementNavigator` 仍走 `ClosestPoint`
  - `ChestController` / `TreeController` 仍直接触发 `OnRequestGridRefresh`

**阶段一结论**
- `导航检查` 当前已经具备进入阶段二的理由，但只能按 branch-only 模式进入。
- 推荐 continuation branch 仍是 `codex/navigation-audit-001`。
- 最小 checkpoint 不是直接改热文件，而是先在分支里固化 `2.0.0` 的 `requirements.md / design.md / tasks.md`，把首批整改边界钉在 `NavGrid2D / PlayerAutoNavigator / GameInputManager` 三层。
- 本轮未进入 Unity / MCP / Play Mode，也未触发 A 类热文件写入。

**恢复点 / 下一步**
- 治理线程后续应先裁定是否批准 `导航检查` 进入阶段二。
- 若批准，先发 `grant-branch`，再允许 `ensure-branch` 进入 `codex/navigation-audit-001`。
- 若未批准，`导航检查` 继续保持阶段一只读结论，不得自行切出任务分支。

## 2026-03-18｜NPC 线程阶段一回收已回写

**当前主线目标**
- 继续用阶段 22 承接恢复开发分发与回收，让业务线程先完成阶段一只读唤醒，再由治理裁定是否进入阶段二。

**本轮完成**
- 读取 `可分发Prompt/NPC.md`，按其中阶段一要求完成 shared root 只读 preflight。
- 复核当前 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`，`git status --short --branch = ## main...origin/main`。
- 回写 [NPC.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/Codex规则落地/22_恢复开发分发与回收/线程回收/NPC.md) 的“阶段一：线程回收区”，明确 continuation branch 仍为 `codex/npc-roam-phase2-003 @ 7385d123`，且当前只通过阶段一、尚未拿到 branch grant。

**关键判断**
- 当前 shared root 仍是 `main + neutral`，但 `branch_grant_state = none`。
- Unity / MCP 当前 `current_claim = none` 不等于可直接写；若进入阶段二，仍需先做 live verify，并遵守 Play Mode 退出与 A 类热文件查锁纪律。

**恢复点 / 下一步**
- 等治理线程读取 `线程回收/NPC.md` 后裁定是否发放 NPC 的阶段二 grant。
