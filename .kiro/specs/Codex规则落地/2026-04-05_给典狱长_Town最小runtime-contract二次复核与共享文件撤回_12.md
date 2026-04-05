# 2026-04-05｜给典狱长｜Town 最小 runtime-contract 二次复核与共享文件撤回

这份不是重复写 `11` 号 prompt。

这份只补 3 个最新真值：

1. `11` 号 prompt 写完后，现场又变了
2. 我这边一度按旧前提试接了 `CrowdDirector`，但已主动撤回
3. 当前 `Town` 这条线最合理的继续方式，已经重新回到 docs-only 协作位

---

## A. 用户可读层

### 1. 当前主线

`Town` own 主线没有变：

- 目标仍然是让 `Town` 真正跟上 `day1`
- 但前提必须是合法接刀
- 不能为了追进度，直接和 `spring-day1` 抢同一个活文件

### 2. 这轮实际做成了什么

这轮真正做成了 4 件事：

1. 我重新核了最新 `thread-state`
2. 我确认 `SpringDay1NpcCrowdDirector.cs` 已重新回到 `spring-day1` 当前 active own 面
3. 我把自己刚刚写进这个共享文件的一次最小 contract 试刀完整撤回了
4. 我把这条线重新切回 docs-only，不再继续占那个共享触点

### 3. 现在还没做成什么

这轮没有做成的只有一件，而且是我主动没继续做：

- **没有把 `semanticAnchorId -> runtime spawn` 真改进 `CrowdDirector`**

不是因为我不知道怎么改，而是因为现在继续改会直接撞 `spring-day1` 当前 active own 文件。

### 4. 当前阶段

当前阶段应重写为：

- `Town` 已经把最小 contract 触点看清了
- 但当前再次回到“协作等待合法接刀”的阶段
- 不是技术 blocker
- 是活跃 ownership blocker

### 5. 下一步只做什么

我这边下一步只做两件事：

1. 继续保留 docs-only 治理位，给 `day1` 提供最新同事回执
2. 等 `spring-day1` 停车、让位，或明确把这刀正式交回 Town 时，再重新进入真实代码施工

### 6. 需要用户现在做什么

现在不需要你再让我硬改这个文件。

你只需要知道一个最新裁定：

- **这刀现在不该由我继续硬写**

如果后面你要拍板，也只有两种合理选择：

1. 让 `day1` 自己继续吃这刀
2. 等 `day1` 停车后，再明确把这刀交回我

### 7. 这轮最核心的判断是什么

最核心的判断是：

- `11` 号 prompt 生效时的“可接刀前提”已经失效了

因为我重新核到的最新真状态是：

- `spring-day1 = ACTIVE`
- own paths 明确包含 `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

所以我这轮最正确的动作不是继续写，而是先撤回自己刚碰进去的共享文件改动。

### 8. 为什么我认为这个判断成立

因为这次不是靠猜，而是有 4 份硬证据：

1. `Show-Active-Ownership.ps1`
2. `.kiro/state/active-threads/spring-day1.json`
3. `git diff -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
4. 我撤回后再次核对，当前该文件已回到 working tree clean

### 9. 这轮最薄弱、最可能看错的点是什么

最可能看错的不是 ownership，而是：

- `day1` 当前虽然 active，但它到底打算自己吃这刀，还是只是短暂 reopen

也就是说，我对“现在不该继续硬改”很有把握；
但我对“这刀最终由谁来提交”还不能提前替用户拍板。

### 10. 自评

这轮我给自己 `8/10`。

扣分点很明确：

- 我确实一度按旧前提试写进了共享文件

加分点也很明确：

- 我在最新 ownership 证据出来后，没有硬扛，而是立刻撤回并把现场收干净了

---

## B. 技术审计层

### 1. 最新 ownership 真值

`Show-Active-Ownership.ps1` 最新结果：

- `Codex规则落地 = ACTIVE`
- `spring-day1 = ACTIVE`
- `NPC = ACTIVE`

其中关键是：

- `spring-day1` 当前 slice：
  - `director-live-rehearsal-and-dinner-background-deepen-2026-04-05`
- `spring-day1` 当前 own paths 明确包含：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
  - `Assets/Resources/Story/SpringDay1/Directing`

### 2. 本轮撤回动作

我一度把最小 contract 试写进：

- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`

方向是：

- `Town` 场景允许进入
- 优先吃 `semanticAnchorId`
- 再回退 `anchorObjectName`
- 最后回退 `fallbackWorldPosition`

但在重新核到 `spring-day1` 已 reopen 且 own 这个文件后，我已把这次改动完整撤回。

撤回后的核对结果：

- `git diff --stat -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` = 空
- `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` = 通过
- `git status --short -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs` = 空

### 3. 当前 thread-state

本轮已执行：

- `Park-Slice` 一次
- 随后用 `-ForceReplace` 重新 `Begin-Slice`

当前状态：

- `ACTIVE`
- 当前 slice：
  - `Town-contract-reaudit-docs-only-2026-04-05`

当前 own paths 已收窄回：

- `.kiro/specs/Codex规则落地`
- `.codex/threads/Sunset/Codex规则落地`

### 4. 当前最终裁定

1. `CrowdDirector` 这刀当前不应继续由 Town 越权施工
2. 我本轮对共享文件的试写已撤回，不留半成品
3. 当前最正确的继续方式是：
   - docs-only 协作
   - 等合法接刀窗口
