# 2026-03-31_典狱长_spring-day1_Primary新路径duplicate处置_02

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_Primary.unity删除面_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`

你这轮不是恢复旧 canonical path，那一刀已经做完了。

你这轮也不是继续 UI，不是继续 Day1 剧情，不是继续讨论 `Primary` 迁移意图。

你这轮唯一主刀只有一个：

- **把 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)` 这份新路径 same-GUID duplicate scene 做最小处置，让仓库里不再同时存在两份同 GUID 的 `Primary`。**

## 一、当前已接受基线

1. 旧 canonical path 已经恢复：
   - `Assets/000_Scenes/Primary.unity(.meta)` 已回到 `HEAD` 基线
2. stale NPC active lock 已经释放：
   - `Primary.unity` 现在是 `unlocked`
3. 当前 live canonical 面已经重新站回旧路径：
   - `ProjectSettings/EditorBuildSettings.asset` 指向 `Assets/000_Scenes/Primary.unity`
   - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 也指向 `Assets/000_Scenes/Primary.unity`
4. 但仓库里现在仍同时存在：
   - `Assets/000_Scenes/Primary.unity.meta`
   - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta`
   - 且两者 GUID 相同：`a84e2b409be801a498002965a6093c05`
5. `UI-V1` 已经完成只读裁定：
   - 新路径 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 不是最终 canonical path
   - 它只是迁移 sibling / 临时复制面

所以这轮不再问“它是不是 canonical”，而是直接处理：

- **这份 duplicate sibling 到底怎么最小收掉**

## 二、这轮唯一主刀

只做这一刀：

1. 只处理：
   - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
   - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta`
2. 目标只有一个：
   - 仓库里不再同时存在两份同 GUID 的 `Primary`

## 三、这轮允许的合格处置方式

你这轮只允许在下面两种处置里二选一，不准自己扩题：

### A｜直接删除 duplicate sibling

适用条件：

- 你确认新路径 scene 现在没有必要继续保留为 tracked 资产
- 且当前 live canonical 已经明确回到旧路径

动作：

- 删除 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
- 删除 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity.meta`

### B｜保留文件，但改成非同 GUID sibling

适用条件：

- 你认为它当前仍有保留必要
- 但绝不能再和旧 canonical path 共用同 GUID

动作：

- 只把这份新路径 scene 改成独立 GUID 的非 canonical sibling
- 不改 scene 内容
- 不改旧 canonical path

## 四、本轮明确禁止

- 不准再碰 `Assets/000_Scenes/Primary.unity(.meta)`
- 不准改 `ProjectSettings/EditorBuildSettings.asset`
- 不准改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
- 不准顺手改任何 scene 内容
- 不准继续处理锁
- 不准顺手处理 TMP 字体
- 不准把这轮扩成 `Primary` 整案

## 五、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. 你已按 `A` 或 `B` 完成这份 duplicate sibling 的最小处置
2. 仓库里不再同时存在两份同 GUID `Primary`
3. 本轮没有顺手改任何其他 scene / settings / editor code
4. 真实跑过同组 `preflight`
5. 如果过线，真实跑过 `sync`
6. 你能给出提交 SHA

### B｜第一真实 blocker

如果你发现它不能被安全删除，也不能被安全改成独立 GUID：

1. 只回第一真实 blocker
2. 必须说清：
   - exact path
   - 为什么不能按 `A` 也不能按 `B`
   - 当前还差什么证据或现实条件

## 六、本轮最容易犯的错

- 又回头去碰旧 canonical path
- 顺手改 `Build Settings` 或编辑器硬编码
- 把这份新路径 scene 当成 canonical 继续保
- 因为它叫 `Primary`，就又去扩成 Day1 / UI / 导航混战
- 顺手改 scene 内容，而不是只处理 duplicate 身份

## 七、固定回执格式

回执按这个顺序：

- `A1 保底六点卡`
- `A2 用户补充层`
- `B 技术审计层`

其中 `A1 保底六点卡` 仍必须逐项显式写：

1. `当前主线`
2. `这轮实际做成了什么`
3. `现在还没做成什么`
4. `当前阶段`
5. `下一步只做什么`
6. `需要用户现在做什么`

## 八、A2 用户补充层必须额外显式回答

你必须额外补 6 条：

1. `这轮最终采用的是 A 还是 B`
2. `duplicate sibling 是否已完成处置`
   - 只能写 `是 / 否`
3. `旧 canonical path 是否完全未触碰`
   - 只能写 `是 / 否`
4. `当前 own 路径是否 clean`
   - 只能写 `是 / 否`
5. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
6. `如果没过线，第一真实 blocker 是什么`

一句话收口：

- **这轮只收新路径那份 `Primary` duplicate，不再回头碰旧 canonical path。**
