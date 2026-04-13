## 当前已接受的基线

从这一轮开始，`spring-day1` 不再自己碰 Day1 的玩家可见 UI 布局与视觉实现。

这条线现在改成：

- `UI` 线程：负责 Day1 任务清单 / Prompt / bridge prompt / 对话期间的玩家可见呈现、层级、位置、版式、最终观感与玩家侧 UI 稳定性
- `spring-day1`：负责提供 phase / task / bridge prompt / dialogue / modal / sleep / re-entry 的语义 contract，不再越权自己动 UI 壳体

所以你这轮接到的不是“帮忙微调一下位置”，而是：

`按 Day1 语义 contract，把整条 Day1 的任务清单与 Prompt 表现收成稳定、可打包、不会被剧情恢复/重开/读档轻易打坏的正式玩家可见面。`

## 当前唯一主刀

只收：

`整条 Day1 的任务清单 / PromptOverlay / bridge prompt / 对话期显隐 / modal 层级 / 恢复重入后的玩家可见 UI contract。`

不要扩到：

- save/load 底层恢复
- Day1 NPC 站位 / 朝向 / staging
- 导航 / 运行时剧情推进

## 你这轮要接住的 Day1 语义

下面这些不是 UI 自己发明的产品定义，而是 `spring-day1` 要求 UI 按此呈现的 contract。

### 1. 任务清单是 Day1 当前唯一主任务面

- 它必须持续表达“当前该做什么”
- 不应被临时 bridge prompt 反客为主
- 不应被对话框、箱子页、背包页、工作台页、关系页、地图页的残留状态搞坏

### 2. bridge prompt 是次级提示，不是主任务本体

- 它是 `spring-day1` runtime 用来补充当前阶段短时引导的话
- 它不能篡改正式任务卡标题/主任务/焦点文本
- 它不能和正式任务卡重复表达同一句话
- 它现在的最终视觉布局、位置、尺寸、是否与任务卡合体或同根呈现，都归 UI own

### 3. 任务清单 / Prompt 的显隐语义

#### 对话期间

- Day1 正式对话进行中时，任务清单不能抢对白焦点
- 但也不能留下坏的 stale 残影、错误 alpha、错误交互 block
- 对话结束后应能正确恢复

#### 包裹 / 箱子 / 其他 modal 页

- 这些页打开时，任务清单应按统一语义退场或让位
- 关闭后应能恢复
- 不允许出现“实际上被模态页压住了，但 Day1 prompt 还半悬着”这种假活状态

#### 工作台页

- 这条需要你结合你们自己已有 UI 体系裁定最终表现
- 但你必须尊重 `spring-day1` 的语义边界：
  - 哪些阶段 workbench 页面出现时任务清单仍要保留阶段语义
  - 哪些阶段应该完全退场

如果你需要最终裁一刀表现方案，可以回执里明确说你按什么语义做了最终决定。

### 4. 整条 Day1 的阶段语义

你这轮的 UI 不能只看晚饭。

至少要按整条 Day1 去理解：

1. `CrashAndMeet`
2. `EnterVillage`
3. `HealingAndHP`
4. `WorkbenchFlashback`
5. `FarmingTutorial`
6. `DinnerConflict`
7. `ReturnAndReminder`
8. `FreeTime`
9. `DayEnd`

任务清单 / Prompt 在这些 phase 切换时的标题、主任务、辅助提示、显隐节奏必须是连贯的。

### 5. 恢复重入语义

这是当前最容易被打坏的点。

用户已经明确指出：

- `重新开始` 会出问题
- `读档` 只会更高风险

所以 UI 这边必须按下面口径做：

- 不把某一帧的 transient UI 状态当成真值
- 恢复后按当前 `Day1` canonical state 重新建立任务清单/Prompt
- 清掉 stale manual prompt / stale bridge prompt / stale alpha / stale modal 残留

换句话说：

`UI 必须能在 day1 runtime 重新发出正确语义后，稳定回到“当前这个 phase 应该给玩家看到的正式 UI 面”，而不是延续旧壳。`

## 当前最新用户反馈，必须纳入

1. UI 这条线现在由你全权接管，`spring-day1` 不再自己碰视觉布局。
2. 你要接住的不只是“一个位置不好看”，而是：
   - 任务清单
   - bridge prompt
   - 对话期间显隐
   - 不同 phase 过渡
   - 恢复重入后 UI 重建
3. 之前 bridge prompt 飘在任务卡上方抢画面这类问题，已经说明 UI 表现不该再由 runtime 线程临时拍脑袋解决。

## 你这轮需要输出的东西

1. 一份你认可的 Day1 任务清单 / Prompt 表现方案
2. 代码落地
3. 回归测试或最小验证
4. 清楚说明：
   - 你接住了哪些 UI 语义
   - 哪些仍然必须由 `spring-day1` 提供 runtime 信号

## 明确禁止的漂移

- 不要越权去修 Day1 NPC 站位
- 不要越权去修晚饭 staging
- 不要自己去定 save/load 恢复 contract
- 不要把 runtime own bug 包装成 UI 问题

## 固定回执格式

先给 `A1 保底六点卡`

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

再给 `A2 用户补充层`

- 你这轮接住了哪些 UI 语义
- 你没有越权接哪些 runtime 问题
- 你这轮最核心的判断
- 最薄弱点
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
