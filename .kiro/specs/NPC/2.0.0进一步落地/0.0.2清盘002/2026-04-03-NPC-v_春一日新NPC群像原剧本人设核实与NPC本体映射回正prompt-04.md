# 2026-04-03｜NPC-v｜春一日新 NPC 群像原剧本人设核实与 NPC 本体映射回正 prompt

这轮不要继续把当前 `101~301` 的现编名字、人设和对白当成既定正确事实往前推。

用户已经明确质疑：

- 这些设定很可能偏离了原先已经写好的 Day1 剧本和长线 NPC 设计
- 这轮必须先回到原剧本文档核实，再决定哪些能保留、哪些要回正

你这轮不是继续做 `Primary`、不是继续做 Day1 integration、也不是继续修 pair bubble。

你这轮唯一主刀固定为：

- 只做 `NPC-v` 本体侧的原剧本人设核实
- 把“原案角色 -> 当前 `101~301` 槽位”的映射关系查实
- 如证据足够，只在 `NPC-v own` 范围做最小命名 / 文案 / 角色摘要回正

---

## 0. 当前统一认知

### 0.1 这轮先接受的事实源

后续判断优先级按下面顺序，不要倒过来：

1. 用户原始 Day1 剧情原文
2. `0.0.1剧情初稿` 里的 Day1 固化文本
3. 长线 NPC 角色表
4. 当前 `SpringDay1NpcCrowdBootstrap.cs` / `SpringDay1NpcCrowdManifest.asset`

也就是说：

- `bootstrap / manifest` 现在只是“当前实现现场”
- 不是“原剧本真相来源”

### 0.2 当前已经查实的原案人物线

下面这些都不是我现编出来的，而是项目里已经能追溯到的原案：

#### 来自用户原始 Day1 剧情原文

文件：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\002_事件编排重构\Deepseek聊天记录001.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\春1日_坠落_融合版.md`

已能确认的 Day1 角色与关系：

- `马库斯`
  - 村长
- `艾拉`
  - 村长女儿
  - 治疗者 / 牧师向角色
- `卡尔`
  - 村长二儿子
  - 研究 / 魔法学徒向角色
- `大儿子`
  - 在外打猎
  - 早期设定里存在，但 Day1 没有被写成当前主镜头内的稳定在场角色
- `老铁匠 / 老乔治`
  - 原剧本里已存在“村里只有老铁匠会做这些”的设定
- `老杰克`
  - 农田区老人
- `老汤姆`
  - 码头渔夫
- `小米`
  - 小孩 / 孤儿
- `围观村民 / 饭馆村民 / 小孩`
  - 原剧本里明确存在，但大多是群像氛围角色，不是当场全都具名

#### 来自长线 NPC 角色表

文件：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\Deepseek-2-P1.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\省流版VIP8.md`

长线 6 核心 NPC 已明确写出：

- `艾拉`
- `马库斯`
- `老杰克`
- `老乔治`
- `老汤姆`
- `小米`

### 0.3 当前工程里已存在的老 Day1 角色底座

当前工程现状已经说明：

- `Assets/111_Data/NPC/NPC_001_VillageChief*`
- `Assets/111_Data/NPC/NPC_002_VillageDaughter*`
- `Assets/111_Data/NPC/NPC_003_Research*`

这表示：

- `NPC001` 已经是 `VillageChief` 语义
- `NPC002` 已经是 `VillageDaughter` 语义
- `NPC003` 已经是 `Research` 语义

它们显然比当前 `101~301` 更接近原 Day1 主角色链。

### 0.4 当前 `101~301` 不能再被默认当成真角色表

当前 `bootstrap / manifest` 里的：

- `莱札 / 炎栎 / 阿澈 / 沈斧 / 白槿 / 桃羽 / 麦禾 / 朽钟`

只能先判为：

- 当前实现现场里新增的一批角色槽位

不能继续默认判成：

- 用户原剧本已经确认过的 Day1 正式角色

其中目前看起来最可疑的是：

- `101 LedgerScribe`
- `201 Seamstress`
- `202 Florist`
- `301 GraveWardenBone`

它们目前没有在原 Day1 剧本里找到直接来源。

---

## 1. 你这轮唯一主刀

只做一刀：

- 继承当前已有的 `101~301` 产物链和 probe 结果
- 但先回到原剧本文档，查清这些槽位到底哪些是原案、哪些是后补自创
- 然后只在 `NPC-v own` 范围做最小映射回正

一句话：

- 这轮不是继续“把 8 人做得更会说话”
- 这轮是先回答“这 8 人到底该不该这样叫、这样写、这样演”

---

## 2. 这轮必须先读的文件

按这个顺序读，不要跳：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\002_事件编排重构\Deepseek聊天记录001.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\春1日_坠落_融合版.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\初步规划文档.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\Deepseek-2-P1.md`
5. `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\省流版VIP8.md`
6. `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\SpringDay1NpcCrowdBootstrap.cs`
7. `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\SpringDay1NpcCrowdManifest.asset`
8. `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefDialogueContent.asset`
9. `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterDialogueContent.asset`
10. `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchDialogueContent.asset`

---

## 3. 这轮具体要做什么

### 3.1 先产出一张“原案角色 -> 当前实现槽位”对照表

至少覆盖这些角色：

- `马库斯`
- `艾拉`
- `卡尔 / 研究儿子`
- `大儿子 / 猎户线索`
- `老杰克`
- `老乔治 / 老铁匠`
- `老汤姆`
- `小米`
- `围观村民 / 饭馆村民 / 小孩`

对照表至少要回答：

1. 原案里这个角色是否明确存在
2. 当前工程里有没有现成老角色底座
3. 当前 `101~301` 里是否有哪个槽位在“试图扮演它”
4. 这个映射是：
   - `直接可确认`
   - `语义接近但未确认`
   - `当前明显写偏`
   - `原案根本没有`

### 3.2 再把当前 `101~301` 逐个归类

对每个当前 crowd entry 都要归类到下面四类之一：

1. `可直接映射到原案角色`
2. `可降级为匿名/次级群众角色`
3. `明显是后补自创，当前不应继续当正式人设`
4. `证据不足，先停在待核实`

### 3.3 如果映射足够清楚，只做最小 NPC-own 回正

只有在证据已经足够清楚时，才允许你做最小回正。

允许的最小回正包括：

- `SpringDay1NpcCrowdBootstrap.cs` 里的
  - `displayName`
  - `roleSummary`
  - NPC 自己的话术方向
- `Assets/111_Data/NPC/SpringDay1Crowd/*` 里的内容语义
- `SpringDay1NpcCrowdManifest.asset` 里的
  - `displayName`
  - `roleSummary`

优先原则：

- 优先改“显示与内容语义”
- 不要上来就大规模改文件 basename / GUID / prefab 路径

### 3.4 如果映射不清楚，停在第一真实 blocker

如果你发现：

- 原案并没有足够证据支持当前 8 人直接转正

那么这轮不要硬改成另一套新编版本。

你要做的是：

- 明确报出哪些槽位必须先降级成“匿名群像 / 待核实”
- 哪些槽位当前不能再 claim 为正式 Day1 角色

---

## 4. 当前不再允许做的事

- 不要继续把 `莱札 / 炎栎 / 阿澈 / 沈斧 / 白槿 / 桃羽 / 麦禾 / 朽钟` 当成已确认原案
- 不要继续扩写这些角色的长对白、关系线、剧情意义
- 不要回吞 `Primary.unity`
- 不要改 `GameInputManager.cs`
- 不要碰 UI / 字体 / Story/UI
- 不要继续修 `pair bubble` 这类运行态技术尾巴
- 不要主拿 `SpringDay1Director.cs`
- 不要因为想省事，把当前 8 人全部包装成“匿名路人”就算完事

---

## 5. 这轮允许 scope

优先只写：

- `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPC\SpringDay1NpcCrowdBootstrap.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\SpringDay1Crowd\*`
- `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1\SpringDay1NpcCrowdManifest.asset`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`

只读参考允许：

- `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChief*`
- `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughter*`
- `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_Research*`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`

---

## 6. 完成定义

这轮完成，不是指“你又多写了一批台词”。

而是至少要同时满足：

1. 你能给出一张原案角色总表
2. 你能给出一张当前 `101~301` 归类表
3. 你能明确指出：
   - 哪些当前名字 / 人设可保留
   - 哪些只能降为匿名群众
   - 哪些明显是后补自创，需要回正
4. 如果证据足够，你已经把 `NPC-v own` 范围内能回正的最小内容回正
5. 如果证据不足，你能报第一真实 blocker，
   而不是继续拿 runtime probe 结果顶替角色正确性

---

## 7. 固定回执格式

回复时先按这个顺序：

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
- 原案角色总表：
  - 角色
  - 来源文件
  - 当前工程承载物
  - 映射状态
- 当前 `101~301` 归类表：
  - 当前槽位
  - 当前名字/身份
  - 原案对应
  - 处理结论
- thread-state：
  - 是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
  - 如果没跑，原因是什么
  - 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

---

## 8. 最后一条

这轮你不是来证明“新 8 人本体技术上能跑”。

这轮你是来回答：

- 这些人到底是不是原剧本的人
- 如果不是，当前哪些必须回正
- 哪些可以作为后来再扩的群众层保留

在这个问题没说死之前，不准再把当前 `101~301` 包装成“春一日正式人物已经扩完”。

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
