# 2026-03-31_典狱长_UI-V1_SpringUI记忆尾账docs-only归仓_02

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`

你这轮不是继续做 UI，不是继续碰 `Primary`，也不是继续补 `Story/UI` 业务实现。

你这轮唯一主刀只有一个：

- **把当前还没归仓的 `SpringUI` 工作区 memory 尾账，按 docs-only 最小白名单收掉。**

## 一、当前已接受基线

1. `Primary` 整案已经结束：
   - 旧 canonical path 已恢复
   - stale lock 已释放
   - 新路径 duplicate 已删除
2. 共享 TMP 中文字体 6 资产也已回到 `HEAD`
3. 当前仓库剩余 dirty 已只剩 1 条：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
4. 当前这条 dirty 的内容本质是：
   - 2026-03-31 那条 `Primary` 迁移意图只读裁定补记
   - 不是新的业务实现改动

所以这轮不再继续讨论“裁定对不对”，而是只做：

- **把这条已经写好的 memory 尾账，按 docs-only 收口。**

## 二、这轮唯一主刀

只做这一刀：

1. 必收：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
2. 仅在你按项目规则确实需要补线程记忆时，允许最小补 1 条：
   - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
3. 不准新增任何业务文件
4. 不准重写这条裁定内容，不准继续扩题

## 三、本轮允许的合格结果

### A｜docs-only 真过线

适用条件：

- 你确认当前唯一剩余 dirty 确实只是这条 `SpringUI` memory 尾账
- 你已按最小白名单真实跑过 `preflight -> sync`

动作：

1. 只对白名单 memory 文件做 docs-only 收口
2. 收口后复核：
   - `SpringUI/memory.md` 已 clean
   - 如果你补了线程记忆，也必须一起 clean
3. 若本轮完成后整仓 clean，也要老实写明

### B｜第一真实 blocker

如果你发现这条 docs-only sync 不能安全通过：

1. 只回第一真实 blocker
2. 必须写清：
   - exact path
   - 为什么当前不能按 docs-only 收口
   - 还差什么现实条件

## 四、本轮明确禁止

- 不准再碰任何 `Primary` 文件
- 不准再碰任何字体资产
- 不准再碰 `Story/UI`、prefab、scene、业务代码
- 不准把这轮扩成“顺手补更多 UI 记忆”
- 不准重跑业务 `preflight/sync`

## 五、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. `SpringUI/memory.md` 已完成 docs-only sync
2. 本轮没有新增任何业务 dirty
3. 如果你补了线程记忆，也已一起 sync
4. 你能给出提交 SHA
5. 你能明确回答：
   - 当前 own 路径是否 clean
   - 当前整仓是否 clean

### B｜第一真实 blocker

1. docs-only sync 不能通过
2. 只回 blocker，不扩题

## 六、本轮最容易犯的错

- 因为只剩 1 条 dirty，就顺手再补别的 memory
- 又把 `Primary` 裁定内容继续展开成长分析
- docs-only 变成 UI 业务续工
- 把“这条 memory 已 clean”误说成“整条 UI 线业务又推进了一刀”

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

1. `这轮最终是 A 还是 B`
2. `SpringUI/memory.md 是否已完成 docs-only 归仓`
   - 只能写 `是 / 否`
3. `本轮是否触碰任何业务文件`
   - 只能写 `是 / 否`
4. `当前 own 路径是否 clean`
   - 只能写 `是 / 否`
5. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
6. `如果没过线，第一真实 blocker 是什么`

一句话收口：

- **这轮只收 `SpringUI` 的 memory 尾账，不再回头碰任何业务现场。**
