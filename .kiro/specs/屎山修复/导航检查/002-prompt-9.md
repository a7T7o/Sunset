# 002-prompt-9

我先纠正你对上一轮施工令的一处执行偏差。

你上一轮做的这件事本身没有错：

- 先把你自己刚引入的 `NavigationAvoidanceRulesTests.cs` 编译错误清掉

但你错在后半句：

> 你把“外部 compile blocker 已存在”理解成了“这轮可以先停在这里”。

我现在明确纠正：

## 外部 blocker 可以备案，但不能当停车位。

尤其是这条：

- [SpringDay1WorkbenchCraftingOverlay.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs)

它现在只允许占一个身份：

> **external blocker note**

它不再允许占另一个身份：

> **你这轮导航主线的收口理由**

换句话说：

- 你**不需要**去修 `spring-day1`
- 你也**不允许**因为它没修就把导航主线停住

---

## 一、从现在开始的硬纪律

### 1. 外部 blocker 只准记一次

你已经确认：

- 你自己引入的 `CS0246` 已经清掉
- 当前 fresh compile 的外部红错在 `SpringDay1WorkbenchCraftingOverlay.cs`

这件事到此为止。

后面你可以在回执里保留一行：

- `external_blocker_note: SpringDay1WorkbenchCraftingOverlay.cs 语法红错仍在，当前未由我修复`

但你不准再围着它转，不准再拿它做本轮主叙事。

### 2. 不准再交“验证恢复中”的回执

从这一轮开始，我不再接受这种回执节奏：

1. 先修一个小编译问题
2. 再发现外部 blocker
3. 然后停在“等 fresh compile 恢复”

这在导航主线里不算推进。

### 3. 不准再把“不能 fresh compile”直接等价成“不能继续施工”

只要还有不依赖 global fresh compile 才能开始的结构施工项，你就必须继续做。

只有在下面这种情况，才允许你再次把 external blocker 抬成真正 blocker：

> **你已经把本轮所有不依赖 fresh compile 的导航结构施工都做完了，**
> **且下一个具体代码动作客观上必须依赖 fresh compile / fresh live 才能决定。**

如果你做不到这一点，你就还不允许停。

---

## 二、你这轮真正要做的，不是继续清编译，而是继续拆私有闭环

你现在的主线仍然只有一条：

> **让 `S0-S6` 从“审稿认定未完成”继续往真正闭环推进。**

这轮我把“下一刀最小硬 checkpoint”给你钉得更具体一点：

## 这轮最少要交出一个“真实退壳 checkpoint”

这里的“退壳”不是写分析，是代码责任真的迁出去。

你必须在下面两类事情里，至少做成一类；最好两类一起推进：

### 方向 A：把交通裁决从 solver 解释器改成真正前置中轴

也就是你要真的推动：

- `NavigationTrafficArbiter`
- `NavigationLocalAvoidanceSolver`

从现在这种：

- solver 先算
- arbiter 再翻译

推进到至少部分成立的：

- arbiter 先形成真实交通裁决
- solver 只消费裁决并求出执行意图

### 方向 B：把玩家 / NPC 的私有导航责任真正迁出控制器

也就是你要真的推动：

- `PlayerAutoNavigator`
- `NPCAutoRoamController`
- `NavigationPathExecutor2D`
- `NavigationMotionCommand`
- 必要时 `PlayerMovement` / `NPCMotionController`

完成至少一组“责任簇”的真实迁移。

这里的“责任簇”指的是下面这类完整责任，而不是只改个变量名：

1. `BuildPath / RebuildPath / ActualDestination` 责任簇
2. `CheckAndHandleStuck / repath` 责任簇
3. `detour create / clear / recover` 生命周期责任簇
4. `arrival / cancel / path-end complete` 收尾责任簇

你至少要让其中一簇从控制器私有黑盒里真正迁到底座共享层。

---

## 三、这轮允许你的施工方式

因为 global compile 现在被别线卡着，所以这轮允许你进入：

> **导航结构施工模式**

这个模式下你要这么做：

1. 只改导航热区文件
2. 用 `validate_script`、静态调用链核对、局部差异审查来证明你不是盲改
3. 不等 fresh compile 恢复才开始代码迁移
4. 把“fresh compile / live”放到这轮结构 checkpoint 之后再补

也就是说：

你现在不是不能做，  
而是不能把“验证阻断”误写成“施工阻断”。

---

## 四、这轮不准碰什么

继续不准碰：

- `spring-day1`
- `农田`
- 非导航业务线
- 无关 UI

再次强调：

- `SpringDay1WorkbenchCraftingOverlay.cs` 你**不修**
- 但你也**不准**因为它没修就停车

---

## 五、这轮我要求你具体回答的问题

你这轮施工后，必须明确回答下面这几个问题，而且都要用代码事实回答：

### 1. 你这轮到底让哪一个共享层“真正多接手了一层责任”

比如：

- `NavigationTrafficArbiter` 现在多接手了什么
- `NavigationPathExecutor2D` 现在多接手了什么
- `NavigationMotionCommand` 现在多接手了什么

### 2. 你这轮到底让哪一个控制器“真正少养了一层私货”

比如：

- `PlayerAutoNavigator` 少了什么
- `NPCAutoRoamController` 少了什么

### 3. 当前还有哪些 old fallback / private loop 仍在托底

这条必须老老实实列，不准装没看到。

### 4. 如果你这轮仍然要停，你下一个具体动作是什么，为什么它客观上必须等 fresh compile / live

注意，是**下一个具体动作**，不是泛泛说：

- “后面要验证”
- “后面要 compile”

我要的是：

> 我下一步要改哪一个文件里的哪一组责任迁移，  
> 为什么这个动作不看 fresh compile / live 就会有高风险。 

如果你说不出这一层，就说明你还不该停。

---

## 六、用户补充区

如果用户继续给你新的现场观察、体验反馈、或主观吐槽：

- 先吸收
- 再判断它是落在交通裁决、私有闭环、统一运动语义还是体验证据层
- 然后继续施工

不要把用户的提醒再次执行成“先停下来修一个编译小尾巴”。

---

## 七、这轮回执格式

下一次回执，只按下面格式回复：

- 当前主线推进到 `S0-S6` 的哪一步：
- 当前在改什么：
- 这轮是否已完成一个真实“退壳 checkpoint”：
- 这轮具体迁出了哪一组责任簇：
- 这轮具体由哪个共享层接手了它：
- 这轮具体让 `PlayerAutoNavigator` 少了什么私有责任：
- 这轮具体让 `NPCAutoRoamController` 少了什么私有责任：
- 当前是否已推进“先裁决、后求解”：
- 当前功能线 live 结果：
- 当前体验线证据：
- changed_paths：
- external_blocker_note：
- 新增或修改了哪些测试 / 静态验证 / 脚本验证：
- 当前仍残留哪些 old fallback / private loop：
- 如果你再次停下，你下一个具体动作是什么，为什么必须等 fresh compile / fresh live：
- blocker_or_checkpoint：
- 一句话摘要：

---

## 八、最后再钉死一次

`002-prompt-8` 不是让你先修一圈 compile 再说。

`002-prompt-8` 的正确执行方式是：

1. 你自己的小红错清掉
2. 外部 blocker 记一次
3. **继续导航结构施工**
4. 做出真实退壳 checkpoint
5. 再回来看 fresh compile / fresh live

所以现在你下一步不是继续盯着 `SpringDay1WorkbenchCraftingOverlay.cs`。

你下一步是：

> **回到导航热区里，把“旧私有导航闭环到底怎么退壳”真的做出第一刀硬结果。**
