# 2026-04-06｜给 Town｜day1 驻村常驻化 scene 第一刀续工 prompt

请先完整读取：

- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给spring-day1_Town驻村常驻化承接与scene-side正式回执_07.md]
- [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给典狱长_Town驻村常驻化scene-side审计与最小改动建议_13.md]

这轮不要再停在 docs-only，也不要再只回“等 day1 回球”。

现在按最新现场，`Town` 已经被允许进入下一阶段：

- **直接做 `Town.unity` 的 resident scene-side 第一刀**

但只做这一刀，不扩散。

---

## 一、当前已接受基线

这些都已经不是待讨论项，直接继承：

1. `101~301` 这批 crowd 的方向已经正式改为：
   - `驻村常驻化`
2. `SCENE/Town_Day1Carriers`
   - 继续保留
   - 它是 director carrier 壳
   - 不是 resident 常驻根
3. `Town` 当前第一真实 blocker 已经压窄成：
   - 没有 resident 根层
   - 7 个 carrier 还全是 `(0,0,0)` 空壳
4. 这轮不让你去抢：
   - `spring-day1` 当前 active 代码文件
   - `CrowdDirector`
   - `day1` runtime 部署逻辑

---

## 二、这轮唯一主刀

只做：

- **`Town.unity` 的 resident scene-side 第一刀**

具体就 3 件事：

1. 在 `SCENE` 下新增：
   - `Town_Day1Residents`
2. 在其下新增并固定 3 个子组：
   - `Resident_DefaultPresent`
   - `Resident_DirectorTakeoverReady`
   - `Resident_BackstagePresent`
3. 把现有 `Town_Day1Carriers` 下的这 7 个 child 从 `(0,0,0)` 空壳推进到“有粗粒度空间语义的非零位”：
   - `EnterVillageCrowdRoot`
   - `KidLook_01`
   - `DinnerBackgroundRoot`
   - `NightWitness_01`
   - `DailyStand_01`
   - `DailyStand_02`
   - `DailyStand_03`

---

## 三、允许的 scope

这轮你允许碰的东西只限：

1. [Town.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Town.unity)
2. 与这刀直接相关的最小 scene-side 取证/验证
3. 你 own 的 `Codex规则落地` 文档与 memory

如果为了 scene-side 第一刀必须补一个极小的辅助说明文档，可以补。

但不要把范围扩到别的系统。

---

## 四、明确禁止的漂移

1. 不要改 `spring-day1` 当前 active 代码文件
2. 不要去碰 `Primary.unity`
3. 不要去碰 `GameInputManager.cs`
4. 不要顺手把 resident actor 实体整批塞进场景
5. 不要把这一轮升级成“整个 Town 常驻系统重构”
6. 不要又回到 docs-only 泛分析

---

## 五、这轮完成定义

只有同时满足下面这些，才算这一刀完成：

1. `Town.unity` 里已经真实存在：
   - `SCENE/Town_Day1Residents`
   - `Resident_DefaultPresent`
   - `Resident_DirectorTakeoverReady`
   - `Resident_BackstagePresent`
2. `Town_Day1Carriers` 保留原名和原 7 个 child 名，不被改坏
3. 这 7 个 carrier 不再全是 `(0,0,0)` 空壳
4. 你能明确说出每个 carrier 当前的大致空间语义
5. 这轮没有带出 own red
6. 你能给 `day1` 一个明确结论：
   - Town 的 resident scene-side 第一刀已站住
   - 后面什么时候适合把 resident actor 或迁回 contract 继续往下做

---

## 六、如果遇到 blocker，只允许卡在这里

如果你这轮被卡，blocker 必须具体到以下之一：

1. `Town.unity` 当前有真实 hot 冲突，不能合法写入
2. 现场已有他线 scene 脏改，导致你不能安全加 resident root
3. 你在摆 7 个 carrier 时发现现有场景层级/相机/地形关系让这些位置根本无法形成最小空间语义

如果不是这种 blocker，不要提前停。

---

## 七、回执必须先说人话

这轮回执固定先给：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

然后再给技术审计层。

技术层请至少带：

1. 你到底改了 `Town.unity` 哪些 scene-side 根层
2. 7 个 carrier 现在是否都已脱离零位
3. 这轮有没有引入 own red
4. 当前 `Town` 是否仍 `PARKED/READY/ACTIVE`

---

## 八、这轮做完后，最值钱的一句话目标

把 `Town` 从：

- “只有 carrier 名字和常驻方向说明”

推进到：

- “真正拥有 resident scene-side 容器层，并且 7 个 Day1 carrier 已经是可承接的空间位”

---

## 九、thread-state

如果你从只读进入真实施工：

1. 先 `Begin-Slice`
2. 准备收口前 `Ready-To-Sync`
3. 如果这轮停下，最后 `Park-Slice`

不要把 scene 写完却还挂在旧状态里。
