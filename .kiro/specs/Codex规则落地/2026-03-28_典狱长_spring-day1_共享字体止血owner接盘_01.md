# 2026-03-28｜典狱长委托｜spring-day1｜共享字体止血 owner 接盘｜01

## 当前唯一主刀

你这轮只做一件事：

把“共享字体止血”里属于 Day1 owner 的那一半，真正从典狱长位接回去，收成一个能说明白、能验证、能给用户下判断的 Day1 checkpoint。

这轮不是让你重建整套字体系统，也不是让你去接共享字体底座。

---

## 当前已接受基线

以下内容已经被典狱长位钉死，你不要再回到旧混战口径：

1. `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
   现在应明确看作：
   - 共享 TMP 动态字体稳定性风险
   - 不是 Day1 主线未完成
   - 不是你这轮要深做到底的业务主刀

2. 当前相关现场已经分成两类：
   - 共享字体风险线：
     - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
     - `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
     - 以及整批 `DialogueChinese*` 的底层稳定化
   - Day1 owner 线：
     - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
     - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - `Assets/111_Data/UI/Fonts/Dialogue/DialogueFontLibrary_Default.asset`
     - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
     - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`

3. 典狱长位已经确认：
   - 上面这 6 个 Day1-facing 文件都不是“无关缓存”
   - 它们会真实改变 Day1 的 Prefab、默认配置或运行时行为
   - 其中有些能在 Prefab / Inspector 直接看到，有些只能在 Play 里看到，但都属于真实产品面

4. 当前同文件里还混着 Day1 其他行为改动，所以这轮不是“纯换字体路径”。
   也就是说，现场已经存在 `same-file contamination`，你必须以 owner 身份把“哪些该留 / 哪些不该在这轮吞”真正判清。

---

## 你这轮要做什么

按下面顺序执行，但全程只锁 Day1 owner 这 6 个文件，不准扩写到底座线：

1. 先直接读这 6 个文件的当前 working tree diff，不要凭记忆想当然。

2. 把这 6 个文件里的当前改动，明确分成 3 类：
   - `A. 这轮为了 Day1 owner 止血必须保留的改动`
   - `B. 混进来的 Day1 行为改动，但你判断这轮可以一起留的改动`
   - `C. 不该在这轮吞进去、应回退或留到后刀的改动`

3. 然后按你的判断真正落一刀最小 owner 收口：
   - 你可以保留
   - 你可以改写
   - 你可以局部回退
   但只能围绕这 6 个文件本身完成，不准借机漂去共享字体底座治理。

4. 你必须把用户最关心的事实说清楚，而不是只报技术字段：
   - 这些改动里，哪些内容用户能在 Prefab / Inspector 直接看到
   - 哪些内容只有进 Play 后才能看到
   - 哪些内容是真正式面，不是缓存
   - 哪些问题仍属于共享字体线，不该被你伪装成“Day1 这轮已经全收完”

5. 如果你认为 owner 侧最合理的止血结果是：
   - Day1 默认链暂时不再把 `DialogueChinese V2 SDF.asset` 放在默认首选
   - 而改走更稳的 Day1 可接盘字体路径
   你可以这样做；
   但你必须把“为什么这是 Day1 owner 侧该留的一刀”讲清楚。

6. 你要尽量完成 own 范围内的最小验证：
   - 静态 / no-red / Prefab / Inspector 级
   - 如 Unity 窗口安全可用，也可以做最小可视确认
   但不要把“共享字体底座还没彻底重建”当成这轮不往前走的默认借口。

---

## 这轮明确禁止

1. 不碰 `Primary.unity`
2. 不深做 `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
3. 不深做 `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
4. 不扩成整批 `DialogueChinese*` 字体重建
5. 不把这轮偷换成“整套字体系统治理”
6. 不把共享字体底座残余问题包装成“Day1 还没做完”
7. 不准只交 `changed_paths / checkpoint / diff` 这种技术 dump

---

## 完成定义

只有同时满足下面几点，这轮才算你真的命中：

1. 你已经对这 6 个 Day1-facing 文件完成 owner 裁决：
   - 哪些留
   - 哪些改
   - 哪些回退

2. 你已经把这轮真正收成一个 Day1 owner checkpoint，而不是继续维持“字体止血 + 行为顺带改 + 底座风险”三团混在一起。

3. 你已经能用人话告诉用户：
   - 现在工程里实际会看到什么变化
   - 哪些变化在 Prefab / Inspector 可见
   - 哪些变化要 Play 才能看到

4. 你已经把共享字体残余问题明确留在 out-of-scope，不再混成 Day1 own 面未完成。

---

## 回执格式

你这轮的回复对象是用户，不是治理线程。

所以最终回执必须按下面顺序写，不准偷换：

### A1 保底六点卡

必须逐项显式写：

1. `当前主线`
2. `这轮实际做成了什么`
3. `现在还没做成什么`
4. `当前阶段`
5. `下一步只做什么`
6. `需要用户现在做什么`

空项也不准省，必须写明：
- `无`
- `尚未`
- `不需要`
- `仍待验证`

### A2 用户补充层

这层这次不是可省略的空话层，你必须至少补 2 件事：

1. `功能点验收清单`
   - 以功能点为单位写
   - 每个功能点都要说：
     - 你可以去哪里看
     - 怎么操作
     - 预期看到什么
     - 如果没看到，意味着哪一层还没过

2. `停步自省`
   - 你对自己这轮结果的自评
   - 你这轮最薄弱的地方
   - 你最可能看错的点
   - 为什么你判断“下一步只该做那一刀，而不是别的路”

### B 技术审计层

最后才允许写：
- `changed_paths`
- `验证状态`
- `是否触碰高危目标`
- `blocker_or_checkpoint`
- `当前 own 路径是否 clean`

---

## 最后提醒

你这轮不是来证明“典狱长分析得对不对”。

你这轮是来把本来就该属于 Day1 owner 的那 6 个文件，真正收回到 owner 手里，给用户一个能看懂、能验、能继续下令的结果。
