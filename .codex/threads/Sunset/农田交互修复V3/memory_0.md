# 农田交互修复V3 线程记忆

## 2026-03-26：首轮只做 `013` sapling-only runner 稳定性收尾

**用户目标**：
- 继续沿农田交互修复主线推进，但这轮只允许处理 `013` 的 sapling-only runner 稳定性收尾。
- 必须先完整读取 `2026-03-26-农田交互修复V3首轮启动委托-02.md`，先确认当前是否拿到了没有 `NavValidation` 并发干扰的 live 窗口，再只跑 `SaplingGhostOccupancy` 的最小 fresh 验证。
- 不准重新审判 `012`，不准大改 `PlacementManager.cs / GameInputManager.cs`，也不准提前扩到新增 6 条需求。

**当前主线目标**：
- 维持 `012` 已成立的 placeable 恢复基线不动，只把 `013` 剩余的 sapling runner 尾差继续压窄。

**本轮子任务 / 阻塞**：
- 子任务：做 startup preflight、判定 sapling-only live 窗口是否独占、必要时只在 runner/menu 侧做最小补口并重跑最小 fresh。
- 当前阻塞已从“入口能不能起跑”收缩成两层：
  1. 首轮独占 fresh 里，runner 自己的 preview 锁格稳定性失败；
  2. 补口后的复跑窗口又重新混入 `NavValidation` 并发。

**已完成事项**：
1. 已读取：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V2交接文档\00_交接总纲.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V2交接文档\05_当前现场_高权重事项_风险与未竟问题.md`
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\V2交接文档\06_证据索引_必读顺序_接手建议.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\013-placeable主链checkpoint后只收runner稳定性与遗漏卫生.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\全面理解需求与分析.md`
   - 当前子工作区 / 父工作区记忆。
2. 已按 `skills-governor` + `sunset-unity-validation-loop` + `unity-mcp-orchestrator` 做 Sunset 启动闸门等价核查，并显式说明 `sunset-startup-guard` 走手工等价流程。
3. 已确认当前 git / live 入口：
   - `cwd = D:\Unity\Unity_learning\Sunset`
   - `branch = main`
   - `HEAD = 45e4e89baf6c75d8803c1458e08f28bf1b217a66`
   - `shared-root-branch-occupancy.md` 仍为 `main + neutral`
   - `check-unity-mcp-baseline.ps1` 返回 `baseline_status: pass`
   - 当前唯一实例为 `Sunset@21935cd3ad733705`
   - 活动场景为 `Primary`，进入前处于 `Edit Mode`
4. 已确认本轮允许范围内的 live 资产现状：
   - `PlacementSecondBladeLiveValidationMenu.cs` 已支持 `SaplingOnly`
   - `PlacementSecondBladeLiveValidationRunner.cs` 已存在 sapling-only 分支
   - `FarmRuntimeLiveValidationRunner.cs / FarmRuntimeLiveValidationMenu.cs` 当前应保留为有效 farm live 资产，而不是删除残留
5. 已在清 Console 且确认场景内没有遗留 validation runner 对象后，执行第一轮最小 sapling-only fresh：
   - 这轮 fresh Console 中没有新的 `NavValidation` 日志
   - `PlacementSecondBlade` 已真实起跑
   - 真实失败点为：
     - `scenario_end=SaplingGhostOccupancy pass=False details=attempt=2 preview_not_ready=true state=Preview previewPos=(-6.50, 7.50, 0.00) target=(-6.50, 6.50, 0.00)`
6. 已只在允许范围内对 `Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs` 做 runner 侧最小补口：
   - 第一次种植改成先 `InvokeRefreshPlacementValidation(target, true)` 强制把 preview prime 到候选格，再立刻 `TriggerPlacementAttempt()`
   - 第二次占位复验也改成先把 preview 强制拉回已种下格，再立即走同一条锁格入口
   - 增加 `previewAligned / previewStayedOnOccupiedCell` 这组更窄的失败细节，避免把一格漂移继续混成“外部窗口脏”
7. 已做最小 no-red 闸门：
   - `PlacementSecondBladeLiveValidationRunner.cs` 的 `validate_script` 为 `0 error / 0 warning`
   - `git diff --check -- Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs` 通过
8. 已做补口后的第二轮 sapling-only fresh 复跑：
   - 这轮 `PlacementSecondBlade` 仍能起跑
   - 但同一批 fresh Console 中重新出现了 `NavValidation` 并发日志，因此该窗口不能作为新的独占样本
   - 同轮失败细节为：
     - `scenario_end=SaplingGhostOccupancy pass=False details=secondPlantBlocked=True previewInvalid=False previewStayedOnOccupiedCell=False treeDelta=1 target=(-6.50, 6.50, 0.00)`
9. 本轮结束前已把 Unity 明确退回 `Edit Mode`。

**关键决策**：
- `012` 不重判，当前仍视为 placeable 主链恢复基线已成立。
- 本轮 live 结论必须拆成两句，不允许混讲：
  1. 第一轮 fresh 证明本线至少能拿到一轮没有 `NavValidation` 并发的 sapling-only 窗口；
  2. patched runner 仍未在新的干净窗口里把 occupied-cell 锁格稳定住。
- `FarmRuntimeLiveValidationRunner.cs / FarmRuntimeLiveValidationMenu.cs` 当前继续保留并纳入 `changed_paths`，不是删除对象。
- 不继续开第三次 live，不把 `013` 又放大回 placeable 主链，也不提前扩到新增 6 条需求。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3首轮启动委托-02.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementSecondBladeLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\Editor\PlacementSecondBladeLiveValidationMenu.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmRuntimeLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Editor\FarmRuntimeLiveValidationMenu.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\memory.md`

**验证结果**：
- `unityMCP@8888` 基线通过，实例/场景指向正确。
- 第一轮 sapling-only fresh：无 `NavValidation` 并发日志，但 `preview_not_ready`，失败仍在 runner 自身。
- 补口后的 runner 文件脚本级验证通过，`git diff --check` 通过。
- 第二轮 sapling-only fresh：窗口重新混入 `NavValidation` 并发日志，不能作为独占 fresh 证据；失败细节已缩到 `previewStayedOnOccupiedCell=False`。
- 本轮结束时 `Edit Mode` 已恢复。
- 当前 Console 尚有一条 `Some objects were not cleaned up when closing the scene.` 的 Play 退出噪音；未伴随新的编译红错。

**恢复点 / 下一步**：
- 当前主线没有切走，仍然是农田 `1.0.4 / 013` 的 sapling runner 收尾。
- 当前第一单点阻塞已经不是“菜单起不来”或“必须先重判 012”，而是：
  - patched `PlacementSecondBladeLiveValidationRunner` 还没在新的干净窗口里把 occupied-cell 锁格稳定住。
- 如果下一轮继续，唯一正确动作是：
  1. 先重新确认本轮是否又拿到了没有 `NavValidation` 并发干扰的 live 窗口；
  2. 只重跑 patched sapling-only；
  3. 不扩到 placeable 主链，不提前转新增 6 条需求。

## 2026-03-26：共享根大扫除与白名单收口

**用户目标**：
- 这轮不是继续种地，而是先把农田线自己留在 shared root 的 own dirty / untracked 尾账收干净。
- 必须先完整读取 `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3共享根大扫除与白名单收口-04.md`。
- 明确禁止：不继续 placeable / runner 业务推进，不碰 `Primary.unity`，不吞并 `GameInputManager.cs`、`TagManager.asset`，也不顺手扩到导航 / NPC / spring-day1。

**当前主线目标**：
- 主线仍然服务农田 `V3`，但这一轮是清扫型子任务：只把农田线当前 own dirty / untracked 认清、分离、并准备白名单收口。

**本轮子任务 / 阻塞**：
- 子任务 1：核对农田 own dirty / untracked 与 foreign dirty 的边界。
- 子任务 2：按 `sunset-no-red-handoff` 做最小代码闸门，避免在 cleanup 轮次把 shared root 留红。
- 子任务 3：在不碰 mixed/hot-file 的前提下，准备 `git-safe-sync` 白名单同步。
- 本轮初始唯一真实 blocker 不是 foreign dirty，而是 `PlacementManager.cs` 中 3 条会卡住代码闸门的 CS0649 warning。

**已完成事项**：
1. 已读取 cleanup 委托文档，并回到农田工作区 / 线程记忆核对当前主线和作用域。
2. 已核对当前 live git 现场：
   - `branch = main`
   - 当前 shared root 实际 HEAD 已推进到 `12ce08149716c72e4c76c5fac805fcaa7fd315f7`
   - `origin/main` 与本地 `HEAD` 一致。
3. 已用 `git status --short --untracked-files=all -- <农田白名单>` 把农田 own 面收束出来，当前确认属于农田线 own 的 dirty / untracked 主要包括：
   - `Assets/YYY_Scripts/Service/Placement/Editor/PlacementSecondBladeLiveValidationMenu.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementSecondBladeLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Farm/Editor.meta`
   - `Assets/YYY_Scripts/Farm/Editor/FarmRuntimeLiveValidationMenu.cs`
   - `Assets/YYY_Scripts/Farm/Editor/FarmRuntimeLiveValidationMenu.cs.meta`
   - `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs`
   - `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs.meta`
   - `.codex/threads/Sunset/农田交互修复V2/memory_0.md`
   - `.codex/threads/Sunset/农田交互修复V2/V2交接文档/*`
   - `.codex/threads/Sunset/农田交互修复V3/memory_0.md`
   - `.kiro/specs/农田系统/*` 下当前农田工作区记忆、`全面理解需求与分析.md`、`008~013`、`2026-03-26-*`、`当前续工计划与日志.md`
4. 已明确不属于本轮农田清扫范围、因此没有去碰的 dirty：
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`：mixed hot-file，只报实不吞并
   - `ProjectSettings/TagManager.asset`：owner 未明确，只报异常不认领
   - `Assets/000_Scenes/Primary.unity`
   - 导航链、NPC、spring-day1、`StaticObjectOrderAutoCalibrator.cs`
   - 字体材质与其他非农田白名单路径
5. 已按 `git-safe-sync.ps1 -Action preflight` 对农田白名单做正式预检；首轮 preflight 把唯一真实 blocker 钉成：
   - `PlacementManager.cs` 中 3 条 CS0649 warning：
     - `placeSuccessSound`
     - `placeFailSound`
     - `placeEffectPrefab`
6. 为通过代码闸门，本轮只做了一刀无行为变化的 cleanup：
   - 给上面 3 个仅靠 Inspector 注入的序列化字段补了显式 `= null;` 初值
   - 未改 placeable / runner 业务逻辑
7. 已重新做白名单 `git diff --check` 与 `git-safe-sync` preflight：
   - 当前 `git-safe-sync` 预检已变为 `是否允许按当前模式继续: True`
   - 代码闸门结论为：`已对 5 个 C# 文件完成 UTF-8、diff 和程序集级编译检查`
   - mixed / foreign dirty 仍被正确留在 `Remaining` 区，不会自动纳入农田白名单

**关键决策**：
- 这轮 cleanup 没有把 `GameInputManager.cs`、`TagManager.asset`、`Primary.unity` 吞进来。
- 当前农田 own dirty / untracked 与 foreign dirty 已分清，且农田白名单已经通过 preflight。
- `PlacementManager.cs` 的 warning-cleanup 属于收口动作，不属于业务续工。

**涉及文件 / 路径**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\2026.03.16\1.0.4交互全面检查\2026-03-26-农田交互修复V3共享根大扫除与白名单收口-04.md`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\Editor\PlacementSecondBladeLiveValidationMenu.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementSecondBladeLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Editor.meta`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Editor\FarmRuntimeLiveValidationMenu.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Editor\FarmRuntimeLiveValidationMenu.cs.meta`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmRuntimeLiveValidationRunner.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmRuntimeLiveValidationRunner.cs.meta`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V2\`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\`

**验证结果**：
- 白名单 `git diff --check` 已通过，仅剩 CRLF/LF 提示，不构成 blocker。
- `git-safe-sync` preflight 当前为通过状态。
- 代码闸门已不再报 owned error / warning。
- 当前 mixed / foreign dirty 仍存在于 shared root，但没有被纳入农田白名单。

**恢复点 / 下一步**：
- 当前主线仍是农田 `V3`，但本轮子任务已推进到“白名单可直接 sync”的阶段。
- 下一步只剩真正执行农田白名单 sync 到 `main` 并拿提交 SHA；不需要、也不应该在这轮再扩回业务续工。
