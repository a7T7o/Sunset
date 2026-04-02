# 2026-04-03｜NPC-v｜春一日新 NPC 群像联合完工续工 prompt

这不是一条“你单独接一半、我单独接一半、彼此不知道对方在干什么”的 prompt。

这是为了让你和我下一轮分开做各自区域，但最终一起把这组“春一日新 NPC 群像”完整做完。

你先完整读完这份文件，再开工。

配对并行位不是空口说的，我自己的自用 prompt 已经单独落在：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-本线程_春一日新NPC群像Day1整合并行任务单-03.md`

---

## 0. 当前统一目标

用户的真实目标不是“只把 8 张图导进去”，而是这一整组内容要在 `spring-day1` 里成为一套可落地、可触发、可被剧情消费的群像：

1. 新塞进 `Assets/Sprites/NPC` 的 8 个 NPC 要全部接上。
2. 要有明确身份、人设、叙事语义。
3. 要有 prefab / 动画 / profile / 对话包 / 脚本挂载。
4. 要能在春一日正确时机出场。
5. 要围绕“村长逃跑已经发生”这条现实说话。
6. 最终不是只看结构产物，而是要走到“运行态可消费”的程度。

---

## 1. 我刚刚已经做成了什么

下面这些不是待做，而是我这边已经做完并落盘的基线，你不要重做：

### 1.1 新群像运行时底座已经补上

我已经新增并落盘：

- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`

其中：

- `SpringDay1NpcCrowdManifest.cs`
  - 定义 crowd entry：`npcId / displayName / roleSummary / prefab / anchorObjectName / spawnOffset / fallbackWorldPosition / initialFacing / minPhase / maxPhase`
- `SpringDay1NpcCrowdDirector.cs`
  - 会在 `Primary` 运行时拉起
  - 从 `Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest` 读取 manifest
  - 按 `StoryPhase` 控 NPC 出场 / 退场
  - 动态实例化 prefab
  - 自动补 `HomeAnchor`
  - 如果 prefab 有 roam profile 且没有正式对话组件，会补 `NPCInformalChatInteractable`
- `SpringDay1NpcCrowdBootstrap.cs`
  - 已经写成整套生成入口
  - 会生成 prefab / anim / dialogue content / roam profile / manifest

### 1.2 8 个 NPC 的人设和 phase 基线已经写进 bootstrap

这 8 人的视觉、人设、台词方向、phase、锚点、偏移，我已经基于立绘和“村长逃跑”背景写死进 `SpringDay1NpcCrowdBootstrap.cs`。

当前继承基线如下：

- `101 莱札`
  - 紫帽抄录员
  - 主题：账页、补抄、村长留下的账窟窿
  - phase：`EnterVillage -> DayEnd`
- `102 炎栎`
  - 猎户
  - 主题：追踪村长逃跑路线、怀疑有人接应
  - phase：`FarmingTutorial -> DayEnd`
- `103 阿澈`
  - 跑腿少年
  - 主题：目击村长逃跑当晚背影
  - phase：`EnterVillage -> DayEnd`
- `104 沈斧`
  - 木匠
  - 主题：补门修栅、把村子撑住
  - phase：`WorkbenchFlashback -> DayEnd`
- `201 白槿`
  - 织补师
  - 主题：缝补、照应、稳情绪
  - phase：`HealingAndHP -> DayEnd`
- `202 桃羽`
  - 花铺姑娘
  - 主题：安神草、花束、亮色
  - phase：`FarmingTutorial -> DayEnd`
- `203 麦禾`
  - 饭馆大姐
  - 主题：热汤、夜里照应、收住人心
  - phase：`DinnerConflict -> DayEnd`
- `301 朽钟`
  - 守墓骨差
  - 主题：夜路、回声、对逃跑之人的“怕”
  - phase：`ReturnAndReminder -> FreeTime`

### 1.3 真实资产已经在当前工程里生成完毕

当前项目里已经真实存在：

- `Assets/Sprites/NPC/101.png ~ 301.png`
- `Assets/222_Prefabs/NPC/101.prefab ~ 301.prefab`
- `Assets/111_Data/NPC/SpringDay1Crowd/*.asset`
- `Assets/100_Anim/NPC/101~301/*`
- `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`

并且 `Editor.log` 已明确出现：

- `[SpringDay1NpcCrowdBootstrap] 已生成 8 名 spring-day1 新 NPC 的 prefab、profile、对话包与 crowd manifest。`

所以你不要再从“生成器还没跑”起步。

### 1.4 当前我和你的边界已经重新钉死

以后这条线不许再混：

#### 你的区：`NPC-v`

你只负责 NPC 自己本体这一层：

- 新 NPC 资源导入与切片
- anim / prefab / data / dialogue content / roam profile
- NPC prefab 挂载与 NPC 自己的可聊可跑成立
- NPC 文案、闲聊包、pair/ambient 语义是否站住
- 必要的 NPC 生成工具补口

典型就是这些根：

- `Assets/Sprites/NPC`
- `Assets/100_Anim/NPC`
- `Assets/111_Data/NPC`
- `Assets/222_Prefabs/NPC`
- `Assets/Editor/NPC*`
- NPC 相关 controller / data / interactable

#### 我的区：`spring-day1 story integration`

我只负责 `Day1` 侧的剧情接入和运行态消费：

- 这些 NPC 在春一日什么时候出现
- `StoryPhase` 到 crowd 出场/退场的消费
- `SpringDay1Director / crowd director / manifest` 这一层
- 与现有 Day1 顺序、时机、事件的整合
- 最后面向玩家的 Day1 整体运行判断

一句话：

- 你负责“把人做出来、让人能聊、让人能跑”。
- 我负责“把这些人接进春一日，让他们在对的时机出现并被剧情系统消费”。

---

## 2. 现在总盘里一共有哪些需求

这一轮不要只盯“8 张图导入”。

这条新群像线我现在已经整理成 5 类需求：

### 2.1 产物层需求

要有：

- 8 张 sprite
- 8 套 animation/controller
- 8 个 prefab
- 8 份 dialogue content
- 8 份 roam profile
- 1 份 crowd manifest

### 2.2 角色内容层需求

要成立：

- 身份
- 性格
- 说话方向
- 与“村长逃跑”现实的一致性
- spring-day1 的群像语义

### 2.3 NPC 本体运行层需求

要成立：

- prefab 上 roam 生效
- informal chat 可触发
- facing / anchor / spawn offset 不乱
- prefab 本体不会因为 profile/脚本漏挂而变成空壳

### 2.4 Day1 接入层需求

要成立：

- 不同 `StoryPhase` 下按 manifest 正确出场
- phase 之外不乱冒
- anchor 找得到，找不到时 fallback 行为可解释
- `Primary` 里能被 crowd director 实际消费

### 2.5 最终闭环需求

最终要能说清：

- 哪些已经由你做完
- 哪些已经由我做完
- 哪些还没做完
- 第一真实 blocker 是什么
- 这条线当前能不能继续进 sync-ready

---

## 3. 现在还剩哪些事没闭

下面这些才是接下来要继续收的，不要把已完成和未完成混掉：

### 3.1 已完成

- 8 人设定基线：已完成
- 生成器 / bootstrap：已完成
- prefab / asset / manifest 实物生成：已完成
- crowd director / manifest 代码底座：已完成

### 3.2 还没闭完

- `Primary` 运行时是否按 phase 正确拉起这 8 人：未闭
- crowd director 与现有 Day1 时序的真实消费证据：未闭
- 各 prefab 在 runtime 下 informal chat / roam / facing / home anchor 是否全部成立：未闭
- 这 8 人的说话内容在真实触发时是否符合场景和 phase：未闭
- 同根 dirty / foreign dirty / shared dirty 的收边界：未闭
- 当前能不能进 `sync-ready` 的最终判断：未闭

---

## 4. 你这轮唯一主刀

你这轮只做 `NPC-v` 自己那一层，不要代我把 Day1 整案吞掉。

### 4.1 先做 preflight

你先检查并报实：

1. 这 8 套 prefab / anim / content / roam / manifest 是否齐了
2. prefab 本体上的 `NPCAutoRoamController / NPCInformalChatInteractable / roamProfile` 是否都真正接上
3. 现成的 content 资产有没有空引用、坏引用、明显漏挂
4. 哪些是你这轮自己的 own 新增，哪些是旧 NPC 根历史脏账，哪些是 Day1 shared 侧的 foreign 产物

### 4.2 再把你这一层该补的真正补完

你只补：

1. NPC prefab / asset 本体缺口
2. NPC content / roam / informal chat 本体缺口
3. bootstrap / generator / prefab 配置层缺口
4. 与 NPC 本体直接相关的运行态问题

### 4.3 不要漂到我的区

你不要主拿：

- `Primary.unity` 大改
- `SpringDay1Director` 旧主线整案
- `GameInputManager.cs`
- UI / 字体 / prefab-first / 玩家面提示壳
- 别的 shared 热根

### 4.4 你做完后要交给我的内容

你做完后必须能明确回答：

1. 8 个 NPC 本体层现在哪些已经完全成立
2. 还缺哪些 NPC own 问题
3. 哪些问题已经越界到 Day1 integration，应该交给我接
4. 当前 NPC own 路径是否 clean

---

## 5. 我下一轮会做的内容

这段不是让你做，是让你知道我会接什么，避免撞车。

我下一轮只接：

1. `SpringDay1NpcCrowdDirector` 在 `Primary` 的真实 phase 消费判断
2. 8 人在春一日不同阶段的出场/退场是否符合玩家体验
3. `Day1` 现有剧情链是否需要最小接线来消费这批人
4. 最终“这条线能不能进 sync-ready”的 Day1 侧判断

也就是说：

- 你补好 NPC 本体
- 我接 Day1 runtime integration
- 最后一起把剩余 blocker 收敛成同一张账

---

## 6. 完成定义

这轮你完成，不是指“文件都在”。

而是至少要达到：

1. 你能明确说出 8 个 NPC 的 prefab / profile / dialogue / roam 在本体层是成立的
2. 你能明确指出哪些问题还属于 NPC own，哪些已经是 Day1 integration
3. 你不再把 shared / foreign 的脏改默认吞成自己的
4. 你的回执能让我直接接着做下一轮，而不是还得重新考古一遍现场

---

## 7. 固定禁止事项

- 不要重写 8 人设定主轴
- 不要回头重做 bootstrap 的主结构
- 不要把 `NPC001` 正式主剧情整案一起掀翻
- 不要顺手把 UI / 字体 / `GameInputManager` / `Primary` 大改一起吞掉
- 不要模糊回答“差不多好了”

---

## 8. 固定回执格式

只按下面顺序回：

1. 当前主线：
2. 这轮实际做成了什么：
3. 现在还没做成什么：
4. 当前阶段：
5. 下一步只做什么：
6. 需要我现在做什么（没有就写无）：

然后再补技术审计层：

- changed_paths：
- 验证状态：
- 是否触碰高危目标：
- blocker_or_checkpoint：
- 当前 own 路径是否 clean：
- 这轮总需求盘点：
  - 一共识别出几类需求
  - 哪些是你的
  - 哪些是我的
- thread-state：
  - 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 如果没跑，原因是什么
  - 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 9. 你应该先读的文件

- `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
- `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs`
- `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
- `Assets/111_Data/NPC/SpringDay1Crowd/*`
- `Assets/222_Prefabs/NPC/101.prefab ~ 301.prefab`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`

---

## 10. 最后一条

不要把这轮当成“你单独做完全部”。

这轮的正确做法是：

- 你把 `NPC-v` 自己那一层彻底做扎实
- 我把 `spring-day1` integration 那一层接起来
- 我们下一轮一起把这组新 NPC 群像推进到真正可交接状态
