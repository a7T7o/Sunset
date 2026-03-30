# 2026-03-30_典狱长_farm_确认TreeController与OcclusionManager认领_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_shared-runtime残面定责_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\农田交互修复V3\memory_0.md`

你这轮不是继续补农田功能，不是继续改代码，也不是让你现在就跑 `preflight -> sync`。

你这轮唯一主刀只有一个：

- **基于你自己的聊天记录、thread memory、工作区 memory、当前 git diff 和当前 working tree，回答 `TreeController.cs` 和 `OcclusionManager.cs` 现在到底能不能由你这条 farm 线直接认领收口。**

## 一、当前已接受基线

1. mixed-root 清扫主线已经结束；这轮不是继续扩 integrator。
2. `farm` 线已有真实归仓提交：
   - `5e3fe609`
3. 当前仓库剩下的 runtime 残面里，和你最相关的是：
   - `Assets/YYY_Scripts/Controller/TreeController.cs`
   - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
4. 当前治理侧已做出的中间裁定是：
   - `OcclusionManager.cs` 更像农田 preview 遮挡尾差
   - `TreeController.cs` 不是 shared runtime 小尾账，而是农田/砍树表现包
5. 但这还不是你本人的最终认领回执；这轮要你自己核一次

## 二、本轮你必须回答的核心问题

### 1. `OcclusionManager.cs`

你必须明确回答：

1. 当前 working tree 上这份 diff，是否就是你认可的农田 preview 遮挡尾差
2. 如果现在就把它交还给你收口，你是否认可它可以作为一个独立的小提交面
3. 如果不认可，差在哪里

### 2. `TreeController.cs`

你必须明确回答：

1. 当前 working tree 上这份大 diff，是否就是你认可的农田/砍树表现包
2. 如果现在就把它交还给你收口，你是否认可它可以作为一个完整包继续认领
3. 如果认可，它应该被当成：
   - `A｜一个完整包`
   - `B｜需要拆包`
   - `C｜仍含 foreign / 未完成内容`
4. 如果不认可，哪些部分不是你要的、哪些部分还没到可提交状态

### 3. 这两份文件要不要分刀

你必须正面回答：

- `OcclusionManager.cs` 和 `TreeController.cs` 是否应该：
  - `一起交回 farm`
  - `分成两刀交回 farm`
  - `只收其中一份，另一份继续另案`

### 4. 如果现在就让你接盘

你必须给一个你认可的最小提交单元，不准只说“都像是我的”：

- `提交单元 1`
- `提交单元 2`

如果你认为只有 1 个单元，也要明确写只有 1 个。

## 三、这轮明确禁止

- 不准改 `TreeController.cs`
- 不准改 `OcclusionManager.cs`
- 不准顺手重开 `GameInputManager.cs`
- 不准扩回 `Primary.unity`
- 不准把 TMP 字体一起拖进来
- 不准只报“看起来像我的”，必须回答“现在能不能直接认领收口”

## 四、完成定义

你这轮只有一个合格结果：

- **把 `TreeController.cs` 和 `OcclusionManager.cs` 各自裁定清楚，并给出你认可的最小认领单元。**

注意：

- 这轮不是要你提交代码
- 这轮不是要你跑 sync
- 这轮只要你回答“这两份 diff 现在能不能直接回到 farm 线处理，以及怎么切最合理”

## 五、固定回执格式

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

## 六、A2 用户补充层必须额外显式回答

你必须额外补 6 条：

1. `OcclusionManager.cs 最终裁定`
   - 只能写 `可直接认领 / 不可直接认领`
2. `TreeController.cs 最终裁定`
   - 只能写 `A 一个完整包 / B 需要拆包 / C 仍含 foreign`
3. `如果现在交回 farm，我认可的最小提交单元`
4. `这两份文件是否应分刀`
   - 只能写 `是 / 否`
5. `我选这个裁定的直接证据`
   - 必须来自你自己的聊天记录、thread memory、workspace memory 或当前 git diff
6. `这轮是否触碰任何仓库文件`
   - 预期应为 `否`

## 七、你这轮最容易犯的错

- 把 `OcclusionManager.cs` 和 `TreeController.cs` 一起糊成“都是 shared runtime 小尾账”
- 因为两份文件都和农田相关，就不再区分“轻尾差”和“大包”
- 回去重讲第五轮、第六轮 mixed-root 清扫，却不回答“现在能不能直接认领收口”
- 顺手把 `GameInputManager.cs`、`Primary.unity` 或别的 shared 热根又带回来

一句话收口：

- **这轮只回答一件事：`OcclusionManager.cs` 和 `TreeController.cs` 现在能不能直接回到 farm 线收口，如果能，最小提交单元怎么切。**
