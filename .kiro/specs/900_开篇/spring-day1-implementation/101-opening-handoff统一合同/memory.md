# 101-opening-handoff统一合同 - 开发记忆

## 模块概述
- 模块名称：101-opening-handoff统一合同
- 模块目标：把 opening handoff 的唯一合同落成真实代码，不再让 opening release latch 之后继续走 `direct autonomy` 旧口径；同时把 `003` 的导演放手改成交还给 crowd baseline，而不是直接恢复 roam。

## 当前状态
- **完成度**：已完成第一刀真实施工与最小验证
- **最后更新**：2026-04-15
- **状态**：已停车，等待用户 retest 或下一轮冻结表

## 会话记录

### 会话 1 - 2026-04-15

**用户需求**:
> 我完全批准，请你直接落地

**完成任务**:
1. 按 Sunset 直聊施工规则先执行：
   - `Begin-Slice`
   - slice=`101_opening-handoff-owner-unification`
2. 真实落下本轮唯一窄刀口：
   - opening handoff 不再 `direct autonomy`
   - `003` 不再被导演直接放回 roam
3. 具体代码改动：
   - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `ApplyResidentBaseline(...)` 去掉 `EnterVillage` release latch 后直接 `ReleaseResidentToAutonomousRoam(...)` 的分支
     - 现在 opening handoff 会优先保持 `return-home` 合同；只有本来就在 home/anchor 附近时，才会落回 autonomy
   - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `ReleaseOpeningThirdResidentControlIfNeeded(...)` 改成：
       - 放手 `003` 时不再直接恢复 roam
       - 改为交还给 crowd baseline
       - 放手后立即 `ForceImmediateSync()`，避免 handoff 空窗
   - [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 将 opening handoff 测试从“release 后直接 autonomy”改成“release 后先 return-home”
     - 将 `003` 测试从“导演放手后立刻 roam”改成“导演放手后交回 crowd release contract”
4. 最小代码验证：
   - `manage_script validate SpringDay1NpcCrowdDirector` => `errors=0 warnings=2`
   - `manage_script validate SpringDay1Director` => `errors=0 warnings=3`
   - `manage_script validate SpringDay1DirectorStagingTests` => `errors=0 warnings=0`
   - `errors` fresh console => `errors=0 warnings=0`
   - `git diff --check` 覆盖 touched files 通过
5. 最小 EditMode 定向测试：
   - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldQueueOpeningResidentReturnHomeAfterEnterVillageCrowdReleaseLatches` => `Passed`
   - `SpringDay1DirectorStagingTests.Director_ShouldYieldOpeningThirdResidentToCrowdReleaseContractWhenVillageGateHandsOff` => `Passed`
   - `SpringDay1DirectorStagingTests.CrowdDirector_ShouldKeepOpeningResidentsOnBaselineReleasePathDuringEnterVillage` => `Passed`
6. 额外验证发现：
   - 整个 `SpringDay1DirectorStagingTests` 类仍有一批既有失败，但失败列表里不包含本轮改动的 3 条相关测试。
7. 收尾执行：
   - `Park-Slice`

**修改文件**:
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` - [修改]：opening handoff 不再 direct-autonomy
- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` - [修改]：`003` opening 放手改成交还 crowd baseline
- `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` - [修改]：测试改为保护新 opening handoff 合同
- `.kiro/specs/900_开篇/spring-day1-implementation/101-opening-handoff统一合同/memory.md` - [新增]：本轮子工作区记忆

**解决方案**:
- 这轮没有假装完成“完整退权”，而是只收第一阶段能真正站住的合同统一：
  1. opening handoff 语义统一成“先回 anchor/home，再恢复 roam”
  2. `003` 不再被导演从 opening 终点直接放回 roam
- 这轮明确没有做的事：
  1. 没把 `return-home owner` 正式外移到 NPC facade
  2. 没去改 night schedule / `_spawnStates == 0` 恢复链
  3. 没去扩 dinner / 导航 core / Primary / UI

**遗留问题**:
- [ ] `CrowdDirector` 仍在持有 `return-home` 这段 owner；这轮只是把 opening handoff 的旧合同清掉，不是完整退权。
- [ ] `SpringDay1DirectorStagingTests` 整类仍有既有失败，需要后续单独分层处理 stopgap tests。
- [ ] `compile/no-red` 的 CLI 票仍被 `CodeGuard dotnet timeout` 卡住，所以本轮不能宣称完整 Unity no-red 已闭环。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 先只落 opening handoff 合同统一，不直接做完整 owner 外移 | 当前 `NPCAutoRoamController` 还不支持安全“发起 scripted move 后立刻放手” | 2026-04-15 |
| `003` 本轮一起并入 opening handoff 合同 | 不处理 `003` 就等于继续保留一条导演直放旧链 | 2026-04-15 |
| 用定向 EditMode 测试确认这 3 条相关用例 | 整类已有历史失败，必须把本轮结果与既有失败分开 | 2026-04-15 |

## 相关文件

### 核心文件（修改）
| 文件 | 操作 | 说明 |
|------|------|------|
| `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` | 修改 | opening handoff 不再 direct-autonomy |
| `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` | 修改 | `003` handoff 改成交回 crowd baseline |
| `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs` | 修改 | 改成保护新的 opening handoff 合同 |
