# 导航检查V2 - 线程记忆

## 当前主线目标
- 只做 detour owner 保活最小闭环，让 detour 一旦创建成功至少能稳定活过一个有效执行窗口。

## 会话记录

### 2026-03-26 - 首轮 detour owner 保活最小闭环

- 用户目标：
  - 先完整读取 `2026-03-26-导航检查V2首轮启动委托-02.md`，本轮唯一主刀固定为 detour owner 保活最小闭环，只允许锁 `PlayerAutoNavigator / NPCAutoRoamController / NavigationPathExecutor2D`，并按最小回执格式回复。
- 当前主线目标：
  - 不再回漂 solver 泛调、`TrafficArbiter / MotionCommand`、`Primary.unity` 或大窗 live，只把 detour create -> keepalive -> release 的最小执行闭环接上。
- 本轮子任务：
  - 给 player / NPC detour 加上最小保护窗口，把 no-blocker release 接回共享执行层，并用 1 组 fresh live 复核 `RealInputPlayerAvoidsMovingNpc`。
- 已完成事项：
  1. 读取委托、V2 交接包、工作区记忆、父工作区记忆与 Sunset live 规范/Unity MCP 基线。
  2. 在 `NavigationPathExecutor2D.cs` 中补齐 direct override 的 detour metadata stamping。
  3. 在 `PlayerAutoNavigator.cs` 和 `NPCAutoRoamController.cs` 中接入：
     - detour 创建后 `0.35s` 最小保护窗口
     - `TryClearDetourAndRecover(..., rebuildPath:false)` 的 no-blocker release
     - detour 保护窗内对旧 stuck/rebuild 的抑制
  4. 更新 `NavigationAvoidanceRulesTests.cs` 并完成 `18/18 passed`。
  5. 通过代码闸门：
     - `git diff --check`
     - `validate_script`（4 文件均 `errors=0`）
     - Console `0 error / 0 warning`
  6. 完成 1 组最小 fresh live：
     - `scenario_end=RealInputPlayerAvoidsMovingNpc pass=True minClearance=-0.005 pushDisplacement=0.000 playerReached=True npcReached=True`
     - Unity 已确认回到 `Edit Mode`
- 关键决策：
  1. 不继续泛调 solver，而是直接把 owner 保活窗与 release 闭环补到 controller + executor。
  2. 不做第二组 live，不扩到 `Primary.unity` / Scene / Prefab，只用 `RealInputPlayerAvoidsMovingNpc` 先判断这刀是否跨过“有效执行窗口”。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Navigation\NavigationPathExecutor2D.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PlayerAutoNavigator.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NavigationAvoidanceRulesTests.cs`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航检查\memory.md`
- 验证结果：
  - `NavigationAvoidanceRulesTests`：`18/18 passed`
  - fresh live：`RealInputPlayerAvoidsMovingNpc pass=True / pushDisplacement=0.000`
- 当前恢复点：
  - 当前单一第一进展已收敛为“detour owner 保活最小闭环已接上并拿到首个有效执行窗口”；
  - 如果下一轮继续，只应围绕同一闭环补更直接的 owner 命中证据或扩第二组 fresh live，不回漂旧主线。

### 2026-03-26 - 线程记忆边界纠偏与开工准入冻结

- 当前主线目标：
  - 本线程是 `导航检查V2` 的实现线程记忆，不是 `导航V2` 审核工作区记忆，也不是父线程 `导航检查` 的补充壳。
- 本轮子任务：
  - 响应 `导航V2` 的开工准入委托，只做线程边界纠偏与准入冻结；不做任何代码、验证或 live。
- 本轮完成事项：
  1. 明确钉死：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
       是 `导航检查V2` 自己的线程记忆；
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查\memory_0.md`
       如被引用，只能表述为父线程补同步，不能再冒充本线程记忆。
  2. 明确当前准入边界：
     - `导航V2` 工作区仍停留在“锐评审核 / 认知收口 / 开工准入裁定”；
     - 当前还不能因为锐评审核完成，就自动转入实现施工。
- 关键决策：
  1. `导航检查V2` 的实现续工入口，必须来自新的明确单切片委托；
  2. 该委托应把入口从 `导航V2` 审核工作区重新切回本线程，而不是继续在 `导航V2` 里边审边开工。
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\屎山修复\导航V2\000-gemini锐评-1.0.md`
- 验证结果：
  - 本轮无代码、无 compile、无 tests、无 Unity / MCP / live；只完成文档边界冻结。
- 当前恢复点：
  - 本线程当前应保持暂停，等待新的实现委托；
  - 在新的单切片实现委托到来前，不自行从锐评审核阶段跳回代码施工。
