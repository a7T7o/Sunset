# 2026-04-05｜spring-day1｜阶段承载边界与后续 runtime 落位清单

这份文档不是再讨论“Town 到底有没有价值”。

当前这件事已经判清了：

- `Town` 有价值，而且已经可以被导演层消费；
- 但它还没有 ready 到能承接整条 Day1 的最终 runtime。

所以我现在要做的，是把两件事硬分开：

1. 当前导演层已经可以写到哪里
2. 以后 `Town sync-ready` 后，哪些 runtime 内容再真正落进去

---

## 一、总边界

### 1. 当前已经可以落的，是导演层真值

当前已经可以稳定落的内容包括：

1. 分场
2. 锚点语义
3. 群像层分层
4. 背景层关系
5. 夜间见闻气质
6. 次日站位预示

### 2. 当前还不能 claim 的，是 runtime 全闭环

当前还不能 claim 的包括：

1. Town 场地最终切场
2. 精确路径
3. 相机联动
4. 最终 spawn
5. 秒级走位脚本
6. Town 作为完整 Day1 runtime 主场地

### 3. 当前第一真实 blocker

当前第一 blocker 不是导演线没想清楚，而是外线尚未彻底收平：

1. `Town` 还没 `sync-ready`
2. `camera / frustum` 仍属外线
3. `DialogueUI / 字体链` 仍属外线
4. `PlacementManager.cs` 编译红仍属外线

因此当前正确动作不是停工等待，而是：

- 先把导演层能决定的决定完。

---

## 二、phase 级承载表

| phase | 当前承载方式 | 当前导演可写深度 | 当前不落 runtime 的原因 | `Town sync-ready` 后的 runtime 落位 |
| --- | --- | --- | --- | --- |
| `CrashAndMeet` | 临时/抽象承载 | 开场戏、危险感、撤离意图 | 矿洞与前半段不属 `Town` 主承载 | 继续独立于 `Town`，不迁 |
| `EnterVillage` 前半段 | 临时/抽象承载 | 跟随进入、转场压缩 | 仍是从危险区到安全区的过桥 | 继续不迁，只和 `Town` 接口对齐 |
| `EnterVillage` 后半段 | `Town` 导演层 | 围观、让位、初见视线、小屋外沿过渡 | 还不能写精确进村路径和 live crowd spawn | 先落 `EnterVillageCrowdRoot / KidLook_01` 的 runtime 承接 |
| `HealingAndHP` | 当前小屋临时承载 | 情绪段与 UI 首显语义 | 核心仍在小屋室内，不是 `Town` 主承载 | 继续以小屋承载，不强迁 |
| `WorkbenchFlashback` | 当前小屋临时承载 | 回闪与技术落差语义 | 核心是工作台与玩家记忆，不是群像层 | 继续以小屋承载，不强迁 |
| `FarmingTutorial` | 当前院落临时承载 | 基础生存教学 | 当前仍依赖现有教学空间和玩法链 | 以后只让 `Town` 提供外围生活感，不迁主教学 |
| `DinnerConflict` | `Town` 导演层 + 当前主戏承载 | 饭馆背景层、旁听层、空间压力 | runtime 饭馆切场、站位、相机仍未 ready | 先落 `DinnerBackgroundRoot` 的背景承接 |
| `ReturnAndReminder` | `Town` 导演层 + 抽象承载 | 回屋路上的收口、收摊、避视线 | 精确回屋路线和灯光节奏未 ready | 以后落“回屋安静层”的路径与背景人 |
| `FreeTime` | `Town` 导演层 | 夜间见闻、夜间生活残响 | 当前不宜写死夜游路线和触发点 | 先落 `NightWitness_01` 的 runtime 触发与站位 |
| `DayEnd` | `Town` 导演层 | 夜间收束、次日站位预示 | 真正次日切换与站位刷新还未 ready | 以后落 `DailyStand_01~03` 的晨间站位 |

---

## 三、按段落拆开的“现在能做什么 / 以后再做什么”

## 3.1 `EnterVillage`

### 现在已经能做的

1. 拆成前半段和后半段
2. 后半段开始明确使用 `EnterVillageCrowdRoot`
3. 明确 `KidLook_01` 只承担单点观察
4. 明确围观之后如何把戏交回小屋

### 现在故意不做的

1. 进村路径精确导航
2. crowd 实时刷出逻辑
3. 相机如何跟进村人流

### 以后 Town ready 后再落的

1. crowd 的确切起点
2. 让位动作的 runtime 节奏
3. 小屋外沿过渡的 live 站位

---

## 3.2 `DinnerConflict`

### 现在已经能做的

1. 写清饭馆背景层不是空的
2. 写清卡尔冲突发生时，谁在旁听、谁在装作没听见
3. 写清这场戏的压力来自“有人在场”

### 现在故意不做的

1. 饭馆空间最终 blocking
2. 桌椅精确占位
3. 卡尔与背景层的即时互动脚本

### 以后 Town ready 后再落的

1. `DinnerBackgroundRoot` 的群像站位
2. 收拾者/食客的 live 切换
3. 晚餐空间真正的视线遮挡与流线

---

## 3.3 `ReturnAndReminder`

### 现在已经能做的

1. 明确这是“村里收口”的段
2. 明确主感觉是规则变重，而不是继续认识新 NPC
3. 明确回屋途中背景层应是关门、收摊、低声提醒

### 现在故意不做的

1. 精确回屋路线
2. 灯光/时间节奏的 runtime 绑定
3. 回屋途中可交互点布置

### 以后 Town ready 后再落的

1. 回屋路径上的实位群像
2. 谁在何处收摊
3. 哪些门口在这时亮着灯

---

## 3.4 `FreeTime`

### 现在已经能做的

1. 写清夜间见闻层
2. 写清玩家夜里为何会感到不该久留
3. 写清哪些人适合夜里看你一眼就走

### 现在故意不做的

1. 夜间 roaming
2. 精确触发器
3. NPC 夜巡或路线切换

### 以后 Town ready 后再落的

1. `NightWitness_01` 的 live 占位
2. 触发条件与消失条件
3. 夜间背景点的亮灭与刷新

---

## 3.5 `DayEnd`

### 现在已经能做的

1. 写清夜里必须收束
2. 写清 Day1 结束前应给出次日生活预示
3. 明确 `DailyStand_01~03` 是“明天会继续活着”的位点

### 现在故意不做的

1. Day2 站位刷新时机
2. 次日人物精确出场表
3. 晨间路线和交互

### 以后 Town ready 后再落的

1. `DailyStand_01~03` 的实际站位实体
2. 次日村庄第一屏的生活编排
3. 与 Day2 首个可交互节点的衔接

---

## 四、后续 runtime 落位优先顺序

如果以后 `Town` 真到 `sync-ready`，导演线最合理的 runtime 落位顺序不是平均铺开，而是：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01`
6. `DailyStand_02`
7. `DailyStand_03`

原因很简单：

1. 进村 crowd 是 Day1 最先需要 `Town` 证明自己“不是空壳”的地方
2. 晚餐背景层是后半段冲突可信度的第一支撑点
3. 夜间见闻是 `FreeTime / DayEnd` 的氛围核心
4. 次日站位预示最适合放在最后，因为它不挡当前 Day1 主链

---

## 五、对 `NPC` 线与 `Town` 线的协作接口

## 5.1 给 `NPC` 的真值接口

导演线当前已经给出的稳定接口是：

1. 哪个锚点承担什么戏
2. 哪类人可以进
3. 哪类人必须让位
4. 发话强度上限是什么

`NPC` 接的是：

1. 群像层
2. 背景层
3. 观察层
4. 见闻层
5. 站位层

## 5.2 给 `Town` 的真值接口

导演线当前已经给出的稳定接口是：

1. 哪些 phase 可以按 `Town` 写
2. 哪些 phase 仍不能迁
3. 哪些锚点最先值得 runtime 化

`Town` 接的是：

1. 空间健康
2. 锚点真实可用
3. 后续 live 场地承接

---

## 六、当前必须明确不做的误动作

1. 不把 `CrashAndMeet` 硬迁进 `Town`
2. 不把 `HealingAndHP / WorkbenchFlashback / FarmingTutorial` 假装成 `Town` 主承载
3. 不在 `Town` 未 ready 时写死最终路径
4. 不在导演层越界去写 UI、气泡底座、相机、字体或 `Primary.unity`

---

## 七、当前导演线已经推进到的最深处

到这一步，导演线已经把：

1. phase 级边界
2. 分场级边界
3. 锚点级边界
4. 未来 runtime 落位顺序

全部拆开了。

也就是说，当前还能继续抽象推进的空白已经很少了。

接下来如果还往下走，就不再是“导演层继续想清楚”，而会变成：

1. `NPC` 去接群像 runtime 承接
2. `Town` 去接场地 runtime 承接
3. 或导演线再回到具体 phase 文本与正式剧情资产化

这就是当前“阶段承载边界与后续 runtime 落位”已经能做到的最深处。
