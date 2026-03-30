# 2026-03-31_典狱长_farm_TreeController完整包归仓_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_shared-runtime残面定责_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`

你这轮不是继续讨论它是不是你的，也不是继续把它叫成 shared runtime 小尾账。

你这轮唯一主刀只有一个：

- **把 `Assets/YYY_Scripts/Controller/TreeController.cs` 当前这整包农田 / 砍树表现 diff，作为一个完整包推进到真实 `preflight -> sync`；如果不行，就只回第一真实 blocker。**

## 一、当前已接受基线

1. 认领语义已经问完了：
   - `TreeController.cs` 可以回 farm
   - 而且它应被当成 `A｜一个完整包`
2. 当前 diff 规模已经钉死：
   - `1055` 行级 diff
   - `633 insertions / 426 deletions`
3. 这包的主题不是“小修”，而是：
   - 事务锁
   - 二次命中屏蔽
   - 倒下动画表现
   - 相关前置阻断

## 二、本轮允许纳入的范围

### 1. 代码面

- `Assets/YYY_Scripts/Controller/TreeController.cs`

### 2. 线程 / 工作区记忆

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`
- 如果你这轮有更新：
  - 农田工作区相关 `memory.md`

## 三、本轮明确禁止

- 不准碰 `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
- 不准碰 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- 不准碰 `Assets/000_Scenes/Primary.unity`
- 不准碰 TMP 字体
- 不准把 scope 扩成农田全系统大回收

## 四、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. 只靠 `TreeController.cs` 这一整包与 own docs / memory
2. 真实跑过 `preflight`
3. 真实跑过 `sync`
4. 当前 own 路径 clean
5. 你能给出提交 SHA

### B｜第一真实 blocker

如果它仍然过不了：

1. 只回第一真实 blocker
2. 不准顺手带回 `GameInputManager.cs`
3. 不准顺手带回 `OcclusionManager.cs`

## 五、你这轮最容易犯的错

- 因为它大，就重新把 `OcclusionManager.cs`、`GameInputManager.cs` 一起拖回来
- 一旦 compile / preflight 被挡住，就顺手改别的根
- 又把这包说成“顺手一收的小 runtime 尾账”

## 六、固定回执格式

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

## 七、A2 用户补充层必须额外显式回答

你必须额外补 5 条：

1. `这轮是否只碰了 TreeController.cs`
   - 只能写 `是 / 否`
2. `当前 own 路径是否 clean`
   - 只能写 `是 / 否`
3. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
4. `如果没过线，第一真实 blocker 是什么`
5. `这轮是否触碰任何超出白名单的文件`
   - 只能写 `是 / 否`

一句话收口：

- **这轮把 `TreeController.cs` 当完整包单独收，不准带别的文件。**
