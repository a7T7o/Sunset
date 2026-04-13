## 当前已接受的基线

从这一轮开始，`UI` 不再是你 own 的施工面。

你仍然对 `UI 语义` 负责，但不再自己改 Day1 玩家可见 UI 壳体、位置、样式和最终布局。

也就是说：

- `spring-day1` own：整条 Day1 的 runtime、剧情、时间、NPC staging、re-entry、给 save/UI 的 contract
- `UI` own：任务清单 / Prompt / bridge prompt / 对话期显隐 / modal 层级 / 最终玩家可见呈现

## 当前唯一主刀

`把整条 Day1 从开场到睡觉的 own runtime / own staging / own re-entry / own time gate / own NPC 夜晚行为彻底收尾，并把 UI 与 save 所需语义 contract 交全。`

你这轮不要再自己碰 UI 壳体。

## 当前必须解决的 P0

### P0-1. 晚饭剧情卡顿的真根已被用户确认

用户最新明确指出：

- 晚饭卡住不是抽象“可能条件不对”
- 是因为有一个 NPC 没在规定时间内走到位

所以你这轮必须落实的运行时规则是：

`晚饭开场等待剧情 actor 到位时，最多等待 5 秒；5 秒内仍未到位，就只对必要的剧情 actor 做强制瞬移到位，然后立刻开始剧情。`

这里的硬要求：

1. 只作用于晚饭开场必要剧情 actor
2. 不扩成全局 NPC 管理
3. 不回到旧的“到处抢控制”的过渡管理模式
4. 主动找村长触发与 `+` 快进触发都必须吃到同一套兜底

### P0-2. 001 / 002 的剧情就位与朝向

剧情既然开始：

- `001/002` 必须在必要入口被强制拉到正确位置
- 朝向正确
- 不允许带着旧位置开戏

fresh / restart / restore / `+` 快进 全都要一致。

## 这轮你自己的职责清单

### 1. 整条 Day1 时间与门禁

- Day1 初始 = `09:00`
- `005` 前时间 guardrail 有效
- `18:00` 晚饭
- `19:00` 归途提醒
- `19:30` 自由时段
- 超过 `2:00` 未睡时，强制走一次床边睡觉转场

### 2. 整条 Day1 runtime re-entry

你要先把自己的语义说全并真正落地：

- `CrashAndMeet`
- `EnterVillage`
- `HealingAndHP`
- `WorkbenchFlashback`
- `FarmingTutorial`
- `DinnerConflict`
- `ReturnAndReminder`
- `FreeTime`
- `DayEnd`

每个 phase 在 fresh 进入、restart、restore 后，day1 自己该重建什么、清理什么、重新接什么，都得由你 own 站住。

### 3. NPC 夜晚行为与次日起步

- `20:00` 开始回 anchor
- `21:00` 没回到位的强制 snap 到 anchor
- 次日从 anchor 起步再恢复漫游

### 4. 给 save thread 的整条 Day1 恢复 contract

你必须把完整 contract 交出去，不准再留空白让 save thread 自己猜。

### 5. 给 UI thread 的完整语义 contract

你已经不再自己碰 UI 代码，但你必须保证：

- UI 能拿到整条 Day1 的 phase/task/prompt/dialogue/modal/re-entry 语义
- UI 不需要靠猜去决定什么时候显示什么

## 这轮明确不要做的事

- 不要自己改 PromptOverlay / 任务清单视觉布局
- 不要自己改 bridge prompt 的视觉位置
- 不要越权去修 save/load 底层
- 不要把任意 NPC 的站位修法扩成全局系统

## 这轮建议顺序

1. 晚饭 5 秒到位等待 + 超时瞬移兜底
2. `001/002` 晚饭开场必要强制就位与朝向
3. restart / restore / `+` 快进 共享同一套晚饭开场兜底
4. 夜晚 NPC anchor 逻辑与 forced sleep
5. 整条 Day1 runtime re-entry 梳理
6. 给 save/UI 的 contract 文档与回执

## 完成定义

只有下面都站住，才算这轮真的过线：

1. 晚饭无论主动触发还是 `+` 快进，都不会再因为单个 NPC 没到位而卡住
2. 最多等待 5 秒，超时就瞬移必要剧情 actor 后直接开戏
3. `001/002` 不再带着旧位置进入晚饭剧情
4. 夜间 NPC / free-time / sleep / forced sleep 链闭环
5. save thread 与 UI thread 都拿到了你定义完整的整条 Day1 contract

## 固定回执格式

先给 `A1 保底六点卡`

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

再给 `A2 用户补充层`

- 你这轮最核心的判断
- 为什么这样判断
- 最薄弱点
- 离打包还有多远
- 自评

最后才给 `B 技术审计层`

[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
