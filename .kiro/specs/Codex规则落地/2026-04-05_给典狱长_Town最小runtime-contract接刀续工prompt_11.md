## 先读这份，再继续施工

这不是让你回到“Town 边界说明线程”。  
这是基于当前最新现场，对你这条 `Town` own 线重新下的一刀：现在 `spring-day1` 已经 `PARKED`，你要重新审一次接刀权，并且优先去补那条最值钱的最小 runtime contract。

---

## 当前主线固定为

继续把 `Town` 这条线往“真正跟上 day1 下一步”推进。

这轮不再停在泛分析，不再停在协作解释。  
你最优先要做的，是重新判断：现在是否已经轮到你补 `semanticAnchorId -> runtime spawn` 这刀最小 contract。

---

## 你必须先继承的最新真状态，不要再沿用旧回执口径

1. `spring-day1` 已不再是旧的 `ACTIVE` 现场。  
当前最新 `Show-Active-Ownership.ps1` 真值是：
- `spring-day1` = `PARKED`
- `UI` = `PARKED`
- `NPC` = `PARKED`

2. `spring-day1` 刚刚又往前推进了一刀，而且已经停稳：
- 提交：`d31a590d9e5f22dc2f25cb3c929fb6903279f8d6`
- 关键新真值：
  - `Primary live capture` 已打通
  - `Run Director Staging Tests` 已 fresh `7/7 PASS`
  - 当前导演工具不是第一 blocker

3. 当前 touchpoint 的最新 dirty 面要重新认，不要沿用旧判断：
- [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  - 当前不在 dirty 面
- [SpringDay1DirectorStageBook.json](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json)
  - 当前不在 dirty 面
- [SpringDay1NpcCrowdManifest.asset](/D:/Unity/Unity_learning/Sunset/Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset)
  - 当前仍在 dirty 面

4. 这意味着你这轮必须重新分流，而不是照抄上轮“不该接刀”的旧结论：
- `CrowdDirector.cs` 现在很可能已经可以接
- `StageBook.json` 现在不是当前必要刀口
- `Manifest.asset` 仍然要谨慎，不要吞不清楚的脏改

5. 旧的 Town 真值仍成立，但位置变了：
- `Town` 语义层：跟得上
- `Town` 静态锚点层：跟得上
- 当前真正欠 day1 的，就是最小 runtime contract

---

## 这轮唯一主刀

优先尝试直接补 `CrowdDirector` 的最小 runtime contract。

说白话：
- 不要再先写解释文档
- 先看现在能不能直接动代码
- 能动就直接动
- 不能动再退回 docs-only

---

## 第一组：先重审接刀权，不允许沿用旧结论

开工前你必须重新核：

1. `Show-Active-Ownership.ps1`
2. `git status --short --`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
   - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
   - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
3. `python scripts/sunset_mcp.py status`

然后必须给自己一个新分流：

### 如果现在可安全接 `CrowdDirector.cs`
直接进入第二组真实施工。

### 如果 `Manifest.asset` 仍是脏面但 `CrowdDirector.cs` 已干净
优先走“代码-only 最小 contract”，不要因为 `Manifest` 脏就把整刀都不做。

### 如果连 `CrowdDirector.cs` 也重新进入别人 active 面
不要硬碰，直接退回第三组 docs-only 最深推进。

---

## 第二组：如果现在可写，直接补最小 runtime contract

这轮不要大改，不要豪华重构。

只补这条最小、最值钱、最不漂移的逻辑：

1. 在 [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 里，让当前导演链在 resolve spawn 时：
- 优先吃当前 cue 的 `semanticAnchorId`
- 找不到时再回退旧 `anchorObjectName`
- 最后才回退 `fallbackWorldPosition`

2. 这刀优先覆盖：
- `EnterVillageCrowdRoot`
- `KidLook_01`
- `NightWitness_01`

3. 如果前面站住，再考虑：
- `DinnerBackgroundRoot`
- `DailyStand_01`

4. 这刀必须守住：
- 不碰 `Town.unity`
- 不碰 `Primary.unity`
- 不碰 `GameInputManager.cs`
- 不回吞 NPC own
- 不去改导演工具
- 不去重写整套系统

5. 如果当前 `Manifest.asset` 不安全，就尽量不碰它。  
只要现有 `StageBook` 和 scene anchor 足够喂进 `CrowdDirector`，这轮就优先收成代码-only contract。

6. 如果需要补护栏，优先补最小测试，不要扩成大套件。

---

## 第三组：如果当前仍不宜写代码，就把 docs-only 推到最新现场的最深处

只有在你重新核完后，确认这轮真不该写代码，才走这组。

这时你要做的不是再写一份旧口径边界说明，而是更硬的窄回执：

1. 明确写清：
- 现在为什么仍然不能接 `CrowdDirector`
- 是谁 active
- 哪个文件 dirty
- 具体卡在什么

2. 写清当前最新接刀前提：
- 现在 `spring-day1` 已经 `PARKED`
- 哪个文件已经可接
- 哪个文件仍不可接

3. 写给 `spring-day1` 的回球回执必须更新到最新时点：
- 不准再沿用“spring-day1 仍 ACTIVE”的旧事实
- 不准再沿用“导演工具自身还是最大 blocker”的旧事实

---

## 验证与 no-red 纪律

这轮全过程继续守：

1. `git diff --check`
2. `python scripts/sunset_mcp.py status`
3. 如果你改了代码，再做最小 CLI 自检
4. 如涉及 runtime / scene flow，坚持 `CLI first, MCP last-resort`

不要把这些当你 own blocker：
- editor-only `PersistentManagers`
- test framework 副产物
- 字体 importer 噪音

不要把“代码层 clean”包装成“Town 已 live-ready”。  
当前正确目标只是：`Town 跟上 day1 的最小 runtime contract`。

---

## 收尾要求

1. 更新：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_6.md`
- `C:\Users\aTo\.codex\memories\skill-trigger-log.md`

2. 跑：
- `check-skill-trigger-log-health.ps1`

3. 如果这轮停下：
- 必须 `Park-Slice`

4. 如果这轮有新的 own 成果达到可提交状态：
- 立刻提交
- 不要拖

---

## 回执格式

回我时必须先给用户可读汇报层，顺序固定：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么
7. 你这轮最核心的判断是什么
8. 你为什么认为这个判断成立
9. 你这轮最薄弱、最可能看错的点是什么
10. 你给自己这轮结果的自评

然后再给技术审计层，至少补：
- changed_paths
- 这轮是否真实改到 `CrowdDirector`
- 是否碰到 `Manifest`
- 是否新增测试/验证
- 当前 own 路径是否 clean
- 当前 thread-state
- blocker_or_checkpoint
- 如有提交，提交 SHA

---

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
