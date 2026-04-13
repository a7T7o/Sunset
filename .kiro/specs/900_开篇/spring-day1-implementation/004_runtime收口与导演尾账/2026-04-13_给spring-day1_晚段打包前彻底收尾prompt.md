## 当前已接受的基线

下面这些进展已经成立，但都还不是“Day1 可以直接打包”的同义词：

1. resident scripted move 已重新接回 shared avoidance / blocked abort
2. `Town` resident runtime registry 丢失时，`SpringDay1NpcCrowdDirector` 已补 `Town rebind` 恢复
3. 已有 editor-only probe / late-phase validation jump 工具

你这一轮不要再回头重打这些旧账，也不要再把锅泛甩给导航。

## 当前唯一主刀

只做一件大事：

`把 Day1 晚段（0.0.6 晚饭冲突 -> 归途提醒 -> 自由时段 -> 睡觉收束）在 fresh 进入、主动触发、+ 快进、重新开始、恢复重入时的 own runtime 闭环彻底收平，直到用户可以继续往打包验证走。`

这是一个完整闭环主刀，不是让你四处顺手修别的系统。

## 这轮必须解决的真实问题

### P0. 晚饭入口会卡死

这是当前最硬的 blocker，而且用户已经补充了更明确的现场：

- 晚饭不只是 `+` 快进会卡
- 用户主动去找村长触发，也会卡在这里
- 当前画面会停在 `0.0.6 晚餐冲突`
- 村里环境已经切到晚段，但剧情不继续接管

这说明：

- 不是单一“时间快进”专属问题
- 而是 Day1 晚饭开场链本身还有真断点

### P1. 001 / 002 在晚饭剧情里没有硬就位

用户已经明确裁定：

- 重新开始时，`001/002` 可能不在剧情该在的位置
- 读档只会更危险
- 剧情既然开始了，必要时就应该只在该入口、该时机强制改当前位置和朝向，保证剧情位成立
- 但不允许再回到“全局过渡管理、到处乱抢控制”的坏模式

### P2. PromptOverlay 独立提示条位置不对

只改你新增的 `bridge prompt` 显示区：

- 它要和任务卡同根
- 放到任务卡下面
- 不要再飘在任务卡上方抢视觉
- 不要动已经过线的任务卡主体结构

### P3. 你必须把给存档线程的语义交代清楚

这轮你不要越权自己去修通用 save/load。

但你必须把 `Day1` own 的恢复语义边界明确暴露出来，至少包括：

- 晚饭 phase 恢复后，day1 runtime 需要怎样的 re-entry
- 归途提醒恢复后，day1 runtime 需要怎样的 re-entry
- 自由时段恢复后，day1 runtime 需要怎样的 re-entry
- 哪些 UI / prompt / dialogue / time gate 由 day1 自己重新接管

也就是说：

- 你要把自己的 contract 做清楚
- 但不要自己去接管 save thread 的序列化工作

## 允许的作用域

这轮允许你动的 own 路径，优先只限于：

- `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
- `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs`
- 必要时补你 own 的 editor-only validation menu / probe

如果你真的需要再扩文件，必须能证明它仍然是为上面这一个主刀服务，不是顺手漂移。

## 明确禁止的漂移

- 不要去修通用 save system
- 不要去修 PackagePanel / UI 主系统的广义壳体
- 不要再回导航线程要答案
- 不要顺手扩到农田、放置、工作台泛收口
- 不要再分成“这一刀先改一点、下一刀再补一点”的松散小尾巴

这轮就是要把晚段 own 清单一次性收平。

## 这轮建议的解题顺序

### 1. 先钉死晚饭入口为什么会卡

用户最新反馈已经说明：

- 主动对话会卡
- `+` 快进也会卡

所以你必须先自己复核：

- `TryRequestDinnerGatheringStart(...)`
- `ActivateDinnerGatheringOnTownScene()`
- `BeginDinnerConflict()`
- `SpringDay1NpcCrowdDirector.IsBeatCueSettled(...)`
- 晚饭对白真正 queue/start 的条件

必须回答清楚：

- 卡死到底是 cue settle 永远等不到
- 还是剧情 actor 没就位
- 还是 phase 已切到 DinnerConflict，但 dialogue/prompt/state gate 没继续推进

### 2. 再把晚饭开场做成必要时的强制就位自愈

只在必要入口、必要时机做：

- fresh 进入晚饭段
- 重新开始恢复到晚饭段
- 读档恢复到晚饭段
- Town 场景里 phase 已经是晚饭段，但 `001/002/player` 还没落到剧情位

你要做到：

- `001/002` 至少在对白开始前强制就位
- 朝向正确
- 不把这套硬就位扩成全局常驻接管

### 3. 再把 PromptOverlay 的 bridge prompt 挂到任务卡下面

这刀只改布局，不改语义：

- 同根
- 下面
- 不变宽
- 不乱动任务卡主体

### 4. 最后把“给存档线程的 re-entry contract”写明白

至少要能明确给 save thread 这些结论：

- DinnerConflict 恢复后，day1 需要做什么 runtime re-entry
- ReturnAndReminder 恢复后，day1 需要做什么 runtime re-entry
- FreeTime 恢复后，day1 需要做什么 runtime re-entry
- 哪些状态由 day1 自己重建

## 这轮完成定义

只有下面都站住，你才能 claim 这轮 own 过线：

1. 用户主动找村长，能进晚饭，不再卡死
2. 用户用 `+` 快进到晚饭，也能进晚饭，不再卡死
3. 晚饭对白开始前，`001/002` 会在必要时强制就位，不再带着旧位置开戏
4. PromptOverlay 独立提示条落到任务卡下面
5. 你补了回归测试，不靠口头保证
6. 你把给存档线程的 day1 late-day re-entry contract 写清楚了

## 回执必须怎么写

先交 `A1 保底六点卡`：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

再交 `A2 用户补充层`：

- 你这轮最核心的判断
- 你为什么这样判断
- 你最薄弱、最可能看错的点
- 自评
- 这轮离“可打包”还有多远

最后才交 `B 技术审计层`：

- changed_paths
- 新增/修改测试
- 验证结果
- 当前是否 ready for retest
- 当前是否可直接 claim“晚段 own 已闭环”

## 额外硬要求

- 如果你最后还是发现某一层问题必须由 save thread 接，你不能只说“可能有关”
- 你必须明确给出 day1 这边的 contract
- 但不允许把自己的 own bug 再往外甩

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
