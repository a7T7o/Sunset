# 2026-04-04 spring-day1｜继续施工引导 prompt

请先完整读取：

1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\0.0.1剧情初稿\春1日_坠落_融合版.md`
2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\002_事件编排重构\Deepseek聊天记录001.md`
3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\000_Gemini\1.0.0策划\001_剧本篇章集\000_Deepseek-2\Deepseek-2-P1.md`
4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_原剧本回正与Town承接剧情扩充设计_01.md`
5. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_非UI剧情扩充框架落地任务单_01.md`
6. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_CrashAndMeet-EnterVillage剧情扩充任务单_02.md`
7. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_剧情源协同开发提醒_03.md`
8. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_非UI剧情扩充执行约束与任务单_03.md`

从这一条开始，不再回到“补文档 / 写提醒 / 写 prompt”模式。

接下来如果继续真实施工，只按上面这些文档执行。

---

## 这轮唯一主刀

只做 `CrashAndMeet / EnterVillage` 的内部剧情扩充。

翻成人话就是：

把当前被压扁掉的春一日开场前半段补回当前逻辑链：

1. 矿洞口醒来
2. 语言错位
3. 怪物逼近
4. 跟村长撤离
5. 进村围观
6. 闲置小屋安置

---

## 上位文档怎么分工

继续施工时，必须把 4 份文档分层使用：

1. `原剧本回正与Town承接剧情扩充设计_01`
   - 负责上位剧情方向、角色承载和 Town 承接语义
2. `非UI剧情扩充框架落地任务单_01`
   - 负责 Day1 各 phase 后续该怎么分段扩充
3. `CrashAndMeet-EnterVillage剧情扩充任务单_02`
   - 负责这一刀最初的切刀范围和完成定义
4. `非UI剧情扩充执行约束与任务单_03`
   - 负责当前真正执行时的唯一约束、禁区和顺序

一句话说：

1. `01` 是上位设计
2. `02` 是这刀原始任务定义
3. `03` 是当前执行闸门

后续不能只看其中一份就开工。

---

## 这轮施工时必须守住的 6 条

1. 不碰 `Assets/YYY_Scripts/Story/UI/*`
2. 不碰 `Assets/222_Prefabs/UI/Spring-day1/*`
3. 不碰 `Primary.unity`
4. 不碰 `GameInputManager.cs`
5. 不把 `101~301` 写成原案正式真名角色
6. 不顺手扩到 `HealingAndHP` 之后的大段落

---

## 正确顺序

这轮只允许按这个顺序继续：

1. 查清 `FirstDialogue / Followup` 与 `CrashAndMeet / EnterVillage` 的现有接点
2. 先补：
   - 醒来
   - 语言错位
   - 危险逼近
   - 跟随撤离
3. 再补：
   - 进村围观
   - 闲置小屋安置
4. 最后确认：
   - `HealingAndHP` 还能无缝接上
   - 没有破坏 UI 正在读取的稳定合同

---

## 和 UI 的协同口径

接下来必须始终记住：

1. 我不会碰 UI owner
2. 但我会影响 UI 读取的剧情源
3. 所以如果我改了稳定合同，必须同轮更新：
   - `UI线程_剧情源协同开发提醒_03`
   - 当前工作区 memory

不能只改剧情代码，不补协同说明。

---

## 完成定义

只有当下面这些都成立，这刀才算真正推进了一步：

1. `CrashAndMeet` 不再只是首段对话，而是真的补回“醒来 -> 错位 -> 危险 -> 撤离”
2. `EnterVillage` 不再只是后续说明，而是真的补回“进村 -> 围观 -> 安置”
3. 小屋仍然是闲置废屋，不是大儿子的房子
4. `HealingAndHP` 后续链没有断
5. 没把 UI 正在读取的稳定入口带崩

如果这轮没有 live，只能写：

- `结构成立，live 待验证`

---

## thread-state

如果继续真实施工：

1. 先保持或进入合法 `ACTIVE`
2. 第一次准备 sync 前跑 `Ready-To-Sync`
3. 如果这轮停下，就跑 `Park-Slice`

---

## 最后一句话

下一轮继续时，不再问“我大概要做什么”，而是直接按这些文档进入 `CrashAndMeet / EnterVillage` 这一刀。
