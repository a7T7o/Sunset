# 2026-04-04｜NPC-v｜春一日原剧本群像回正与 NPC 本体收口 prompt

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-NPC-v_春一日新NPC群像联合完工续工prompt-02.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-03-本线程_春一日原剧本角色消费矩阵与群像整合回正任务单-05.md`

原案权威来源按下面顺序继续：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\002_事件编排重构\Deepseek聊天记录001.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\春1日_坠落_融合版.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\初步规划文档.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\Deepseek-2-P1.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\省流版VIP8.md`

## 0. 当前统一裁定

这轮不要再把自己当成 `spring-day1` 的代工位，也不要替我接 opening / PromptOverlay / UI。

当前边界已经重新说死：

1. `UI` 线程只做玩家面 `UI/UE`
   - 任务栏
   - Prompt
   - continue
   - Workbench
   - 玩家面提示壳
   - 玩家侧气泡层级/显示体验
2. `spring-day1` 这边只做：
   - opening 逻辑/剧情控制
   - Day1 事件顺序与约束
   - `PromptOverlay` 这类当前真实 blocker
3. 你 `NPC-v` 这轮只做 NPC own：
   - 原剧本角色回正
   - 新旧 NPC 本体承载
   - prefab / profile / content / roam
   - NPC own 的气泡运行态与样式回正
   - NPC own runtime probe

一句话：

- 我不接你的 prefab/content/bubble 本体。
- 你也不要回吞我的 opening / UI / PromptOverlay。

## 1. 当前已经成立的基线

下面这些不是待做，而是你必须继承的现场：

1. 新 8 人产物链已存在
   - prefab / anim / profile / dialogue / manifest 都已经生成过
2. 你自己的 preflight 已真实站住
   - `Validate New Cast` 为 `PASS`
   - `npcCount=8`
   - `totalPairLinks=16`
3. 你自己的 runtime targeted probe 已经跑过一轮
   - `8/8 instance` 过
   - `8/8 informal` 过
   - `2/2 walk-away` 过
   - `2/2 pair dialogue` 没过
4. 你已经只读查实过一件关键事实
   - `NPC001 / NPC002 / NPC003` 才是原 Day1 主角色链承载
   - 当前 `101~301` 里大多数不是原案正式具名角色，而是后补 crowd 槽位或写偏的人设

所以这轮不要再从“生成器没跑 / 结构还没搭”起步。

## 2. 用户这边最新的真实需求

这轮用户要的不是“继续扩新花样”，而是把 NPC 这条线彻底回正到原剧本，并把 NPC own 剩余问题收口。

最关键的 4 条是：

1. 不要继续自创人物设定
   - 原剧本里本来就有角色
   - 你必须按原案核，不准继续拿 `101~301` 的现编名字硬 claim “正式角色已经扩完”
2. 新 8 人如果证据不足以一对一映射原案具名角色
   - 就降级成匿名 / 次级群众层
   - 不要为了“看起来完整”强行补编
3. NPC own 的气泡还没过线
   - pair dialogue 的 bubble 现在根本没真正亮起来
   - 旧 NPC 气泡样式也没有稳定回到用户认可的旧版
   - 不允许新建样式，不允许再设计新花样
4. `town` 还没就位
   - 这轮不要扩去 Town 长期站位
   - 也不要去碰 `Primary.unity`
   - 你只在当前 NPC own 范围把“角色承载 + 对话包 + 气泡 + runtime 证据”收扎实

## 3. 你这轮唯一主刀

这轮你只做两段，顺序不要乱：

### 第一段：原剧本角色回正

你先把当前 NPC 资产层的口径回正。

至少要做清楚这 3 件事：

1. 老主角色链
   - `NPC001 / VillageChief`
   - `NPC002 / VillageDaughter`
   - `NPC003 / Research`
   这三条继续视为原 Day1 正式主链承载，不要被 `101~301` 抢位。
2. `101~301` 的处理结论
   - 哪些有足够证据可接近原案角色
   - 哪些只能降为匿名 / 次级群众层
   - 哪些现在明显写偏，不能再继续 claim 正式 Day1 人设
3. 如果能只在 NPC own 路径内做最小回正
   - 就直接做
   - 例如 dialogue content、profile、prefab 命名/摘要、NPC 自身语义字段
   - 但不要碰 `SpringDay1NpcCrowdDirector.cs`、`SpringDay1NpcCrowdManifest.asset`、`Primary.unity`

如果某个槽位需要动到 `spring-day1` 的 manifest / director 才能回正，
你不要越界改，改成：

- 明确写出你建议我接的 exact mapping / exact patch 建议

### 第二段：NPC 本体运行与旧气泡收口

这段只做你自己的 runtime 本体，不要再漂去玩家面 UI。

你至少要收这 4 件事：

1. 把 `pair dialogue bubble` 真正修到亮起来
   - 不是 ambient pair 决策成立就算
   - 而是 bubble 真正在 runtime 中出现
2. 把 NPC 旧气泡样式回到用户认可的旧版
   - 不准自己创新
   - 不准新建一套风格
   - 以之前旧版本/现场已经认可的样式为准
3. 普通 informal bubble、pair bubble、打断短气泡
   - 都要按旧样式和当前 NPC 语义做现场核
   - 该一致的一致
   - 该区分的区分
4. 修完后只重跑你 own 的 targeted probe
   - 不要回吞 Day1 opening
   - 不要回吞 UI 测试

## 4. 允许写的范围

这轮默认只允许写你 own 路径：

- `Assets/Sprites/NPC`
- `Assets/100_Anim/NPC`
- `Assets/111_Data/NPC`
- `Assets/222_Prefabs/NPC`
- `Assets/Editor/NPC*`
- `Assets/YYY_Scripts/Controller/NPC/*`
- 与 NPC prefab / roam / informal chat / pair bubble / NPCBubblePresenter 直接强绑定的 NPC own 文件

如果你需要改 shared 运行时代码，只有一种情况允许：

- 该文件直接就是 NPC bubble presenter / NPC own 对话呈现链的唯一宿主

而且你必须在回执里显式报：

- 为什么这是 NPC own 必须触碰
- 触碰了哪个方法/链路

## 5. 明确禁止事项

这轮不要碰：

- `Primary.unity`
- `GameInputManager.cs`
- `SpringDay1Director.cs`
- `SpringDay1NpcCrowdDirector.cs`
- `SpringDay1NpcCrowdManifest.asset`
- `PromptOverlay`
- `Workbench`
- `DialogueUI`
- 玩家面任务栏 / continue / world hint / 提示壳
- opening tests / opening validation
- Town 场景施工
- 字体 / TMP / UI 材质

也不要把 shared / foreign 脏改默认吞成自己的。

## 6. 这轮完成定义

这轮完成，不是“又看了一遍现场”，而是至少满足：

1. 你能给出 `101~301` 的回正结论
   - `可映射原案`
   - `只能降级为群众层`
   - `当前不能再 claim 正式角色`
2. 你在 NPC own 路径内已经做了最小可落地回正
   - 或者你明确报出必须交给 `spring-day1` 接的 exact patch 建议
3. `pair dialogue bubble` 已有 fresh runtime 证据
   - 不是“理论应该亮”
4. NPC 旧气泡样式是否回到用户要的旧版，有明确 live 证据
5. 你能把当前问题清楚分成三类：
   - `NPC own`
   - `需要 spring-day1 接`
   - `暂因 Town 未就位而冻结`

## 7. 固定回执格式

先交用户可读层：

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
- 回正矩阵：
  - `101`
  - `102`
  - `103`
  - `104`
  - `201`
  - `202`
  - `203`
  - `301`
- 问题归类：
  - `NPC own`
  - `spring-day1 接盘`
  - `Town 冻结`
- thread-state：
  - 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 如果没跑，原因是什么
  - 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

## 8. thread-state

你当前如果要继续真实施工，先按 Sunset live 规则接回：

- 先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 第一次准备 sync 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

## 9. 最后一句

这轮你的目标不是“把 NPC 这条线看起来做很多”，而是把两件事真正做实：

1. 不再让后补群像继续冒充原案正式角色
2. 把 NPC own 的 bubble / pair / content / runtime 真正站住
