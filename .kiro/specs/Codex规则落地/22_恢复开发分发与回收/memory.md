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

## 2026-03-18｜遮挡检查完成阶段一只读回收

**当前主线目标**
- 用阶段 22 的可分发 prompt 和固定回收文件，完成 `遮挡检查` 线程的第一阶段只读唤醒，不让结果只漂在聊天里。

**本轮子任务 / 阻塞**
- 本轮是 `遮挡检查` 的阶段一回收，不是进入遮挡整改。
- 当前没有 shared root 脏树阻塞；真正边界是阶段二尚未授权，不能越过只读进入写入。

**已完成事项**
- 读取并执行 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\可分发Prompt\遮挡检查.md` 的阶段一要求。
- 只读复核 shared root live 现场：本轮首次 clean preflight 为 `D:\Unity\Unity_learning\Sunset @ main @ 14838753b4ae9b09b2146b92fb3bfdc9ac82b2a0`，回写收口前 latest live `HEAD` 已前移到 `d0c6bb72ae1100b0ef5626685c6cfe1ee6a9d958`。
- 回读 shared root / MCP 占用文档、遮挡线程记忆、遮挡工作区记忆和当前治理入口。
- 重新核对遮挡主链 live 证据，并把结果写入：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\遮挡检查.md`

**关键结论**
- `遮挡检查` 的核心审计结论在当前 live `HEAD` 仍成立：树 Prefab 双 `OcclusionTransparency`、组件粒度树林判定、命中标准不一致、像素采样成本和旧 Editor 工具漂移都还在。
- 阶段一回写过程中 live `HEAD` 从 `14838753...` 前移到 `d0c6bb72...`，已对关键遮挡证据做抽样复核，结论未变。
- 阶段二可以申请，但前提仍是显式 grant，然后在 `codex/occlusion-audit-001` 上进入最小 checkpoint。
- 最小 checkpoint 仍应先锁定“树 Prefab 父/子双组件是否为误判主根因”，不应一上来扩大到整条主链重写。

**恢复点 / 下一步**
- 等治理线程读取 `线程回收\遮挡检查.md` 后裁定是否发放阶段二 grant。
- 在 grant 之前，本线程继续保持只读口径。

## 2026-03-18｜阶段一收件完成，统一领取入口与治理裁定已补齐

**当前主线目标**
- 当前主线仍是“把恢复开发真正组织起来并回收结果”，不是回到 shared root 闸机修补。
- 本轮子任务是：正式接收 5 份阶段一回收，补治理裁定，并把“统一群发领取入口”做成以后可复用的固定流程。

**本轮已完成**
- 复核当前 live Git 现场：
  - `D:\Unity\Unity_learning\Sunset`
  - `main @ d0c6bb72ae1100b0ef5626685c6cfe1ee6a9d958`
  - 当前 dirty 主要来自阶段 22 治理回写与线程 memory 回写，不属于 shared root 失控。
- 已正式接收并审阅 5 份阶段一回收卡：
  - `NPC.md`
  - `农田交互修复V2.md`
  - `spring-day1.md`
  - `导航检查.md`
  - `遮挡检查.md`
- 已把治理裁定回写进 5 张固定回收卡。
- 已新增统一群发领取入口：
  - `可分发Prompt/00_统一群发领取入口.md`
- 已新增阶段一审核汇总：
  - `阶段一审核结论与阶段二分发建议.md`
- 已修正 5 份可分发 prompt 中写死的 `HEAD` 口径，改为“以 live preflight 为准”。

**治理结论**
- 5 条线程阶段一全部通过。
- `导航检查` 适合作为低风险首个阶段二准入对象，因为首个 checkpoint 只固化文档，不必先碰 Unity / Play Mode / 热文件。
- `农田交互修复V2`、`spring-day1`、`NPC` 都是可进入阶段二的业务候选，但必须共享同一个串行写入槽位，由用户按业务优先级三选一。
- `遮挡检查` 可进入阶段二候选，但建议排在导航之后。

**流程补强**
- 以后类似群发，默认第一跳不再手工复制每个线程专属 prompt，而是先发：
  - `可分发Prompt/00_统一群发领取入口.md`
- 阶段一允许并发，但必须只读。
- 阶段二一律单发，不群发。

**恢复点 / 下一步**
- 若用户现在要开始恢复真实开发，建议先给 `导航检查` 发阶段二，完成一次低风险 `grant -> ensure-branch -> checkpoint -> return-main` 闭环。
- 导航闭环完成后，再从 `农田交互修复V2 / spring-day1 / NPC` 中按业务优先级选择下一写入线程。

## 2026-03-18｜导航检查阶段二已完成 docs-first 闭环

**当前主线目标**
- 当前主线仍是“通过阶段 22 组织恢复开发”，本轮子任务是验证 `导航检查` 能否完成一次低风险阶段二闭环。

**本轮已完成**
- 重新核对阶段二前提成立：
  - `D:\Unity\Unity_learning\Sunset @ main`
  - `HEAD = 6b32ac248a67ad9bab37f68d7fab2d8757a28b21`
  - `git status --short --branch = ## main...origin/main`
  - occupancy = `neutral`
- 已执行：
  - `git-safe-sync.ps1 -Action grant-branch -OwnerThread "导航检查" -BranchName "codex/navigation-audit-001"`
  - `git-safe-sync.ps1 -Action ensure-branch -OwnerThread "导航检查" -BranchName "codex/navigation-audit-001"`
- `导航检查` 已在 `codex/navigation-audit-001` 上完成最小 checkpoint，并已推送：
  - 提交：`71905387`
  - 产物：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\requirements.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\design.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查\tasks.md`
- 已执行 `return-main`，当前 shared root 已回到 `main`。
- 已将阶段二结果回写到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\导航检查.md`

**关键结论**
- 阶段 22 现在已经完成了第一条真正的阶段二闭环，不再只有阶段一只读回收。
- `导航检查` 的低风险路线成立：
  - 先 branch-only 准入
  - 只做 docs-first checkpoint
  - 不碰 Unity / Play Mode / 热文件
  - 完成后 return-main 归还 shared root
- 这证明当前 shared root 的 `grant -> ensure-branch -> task sync -> return-main` 流程对低风险文档线程是可工作的。

**恢复点 / 下一步**
- 若继续导航主线，下一跳应直接回到 `codex/navigation-audit-001`，从 `T1-T3` 开始进入真实代码整改准备。
- 治理侧下一步可决定：
  - 继续让 `导航检查` 承接代码整改
  - 或切换到下一条业务线程占用阶段二槽位
