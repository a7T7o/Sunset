# 2026-03-31_典狱长_farm_OcclusionManager小尾差归仓_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_shared-runtime残面定责_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`

你这轮不是继续补农田大包，也不是顺手动 `TreeController.cs`。

你这轮唯一主刀只有一个：

- **把 `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs` 当前这份 preview 遮挡小尾差，单独收成一个真实 `preflight -> sync` 的小提交面。**

## 一、当前已接受基线

1. 认领语义已经问完了：
   - `OcclusionManager.cs` 可以直接回 farm
2. 当前 diff 的核心主题已经钉死：
   - preview 遮挡从 `GetBounds()` 改成 `GetColliderBounds()`
3. 这刀应该被当成：
   - **一个小尾差**

不是：

- 重新设计 shared runtime
- 顺手把 `TreeController.cs` 一起吞进来

## 二、本轮允许纳入的范围

### 1. 代码面

- `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`

### 2. 线程 / 工作区记忆

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`
- 如果你这轮有更新：
  - 农田工作区相关 `memory.md`

## 三、本轮明确禁止

- 不准碰 `Assets/YYY_Scripts/Controller/TreeController.cs`
- 不准碰 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- 不准碰 `Assets/000_Scenes/Primary.unity`
- 不准碰 TMP 字体
- 不准重新扩大 scope 到农田其他系统

## 四、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. 只靠这份 `OcclusionManager.cs` 小尾差与 own docs / memory
2. 真实跑过 `preflight`
3. 真实跑过 `sync`
4. 当前 own 路径 clean
5. 你能给出提交 SHA

### B｜第一真实 blocker

如果它仍被别的东西挡住：

1. 只回第一真实 blocker
2. 不准顺手扩到 `TreeController.cs`
3. 不准顺手扩到 `GameInputManager.cs`

## 五、你这轮最容易犯的错

- 因为它太小，就顺手把 `TreeController.cs` 一起带上
- 被 blocker 挡住后，直接顺手重开 `GameInputManager.cs`
- 把这刀重新写成“农田 shared runtime 大收口”

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

1. `这轮是否只碰了 OcclusionManager.cs`
   - 只能写 `是 / 否`
2. `当前 own 路径是否 clean`
   - 只能写 `是 / 否`
3. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
4. `如果没过线，第一真实 blocker 是什么`
5. `这轮是否触碰任何超出白名单的文件`
   - 只能写 `是 / 否`

一句话收口：

- **这轮只收 `OcclusionManager.cs` 这一刀，不准带任何别的 runtime 包。**
