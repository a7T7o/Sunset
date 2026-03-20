# 共享根执行模型与吞吐重构 - 需求说明

## 1. 文件身份
- 本文件定义 `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构` 的目标边界。
- 它回答的不是“旧治理规则是否成立”，而是“在安全基线已经建立后，Sunset 应如何恢复更高吞吐的多线程开发”。

## 2. 背景问题
- `Codex规则落地` 前半段已经解决了 shared root 失控、错分支写入、无法回正、规则只停在 Markdown 等问题。
- 但后续补漏证明，当前系统又暴露出新的主矛盾：
  - shared root 单槽位占用时间过长。
  - 持槽期混入了只读核对、治理写回、回执补填、memory 更新等非必要动作。
  - tracked Markdown 被过度拿来承担 queue、回执、运行时等待态，导致 `main` 很容易再次被治理动作写脏。
  - 线程之间虽然“安全”了，但没有真正获得持续并发推进能力。
- 因此新主线必须从“继续补治理层文档”改题为“重构执行层模型”。

## 3. 核心目标
1. 把 shared root 明确定义为“短事务写入槽位”，而不是长期工作台。
2. 允许多个线程在未持有 shared root 槽位时并发执行只读准备工作：
   - 读取规则、锁状态、现有代码与分支 carrier 内容
   - 整理方案、checkpoint 规划、风险清单
   - 保持等待态而不是被当成失败线程
3. 让运行时等待、排队、唤醒状态尽量脱离 tracked repo 写入，避免再次把 `main` 写脏。
4. 给 waiting / 挂起线程提供 gitignored Draft 沙盒，让代码草稿和复盘草稿有合法落点，而不是重新污染 tracked repo。
5. 保留现有安全基线：
   - `main-common + branch-task + checkpoint-first + merge-last`
   - shared root 单写者
   - Unity / MCP 单实例
6. 让治理线程从“人工发车钥匙的保安”转回“规则维护与事故恢复层”，不再承担高频运行时编排。

## 4. 非目标
- 不回退到“无闸机、无占用判定、谁都能随便切分支”的旧模式。
- 不恢复 `worktree` 常态化。
- 不在本工作区中直接修改业务系统逻辑、场景或 Prefab。
- 不把所有等待问题都继续堆进 `Codex规则落地` 里做成更厚的 queue 文档。

## 5. 约束条件
- 物理上仍只有一个 shared root：`D:\Unity\Unity_learning\Sunset`。
- `git checkout / ensure-branch` 仍然是全局写态动作，不能并发。
- Unity / MCP 仍然必须遵守单实例占用与热区规则。
- 线程已有的 branch carrier 仍然有效，但不等于每个线程都拥有独立 working tree。
- 所有执行层新机制都必须兼容现有：
  - `C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1`
  - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
  - `.kiro/locks/shared-root-branch-occupancy.md`
  - `.kiro/locks/active/*.lock.json`

## 6. 验收标准
1. 线程在 shared root 被占用时，不会因为 tracked 回执或治理写回而二次污染 `main`。
2. 线程可以在未拿到 shared root 槽位前保持“有效等待”：
   - 有明确等待态
   - 有明确恢复点
   - 有明确下一次被唤醒后该做什么
3. shared root 持槽期被压缩到“最小写事务”：
   - docs-first checkpoint 应能快速进出
   - 代码最小 checkpoint 以白名单提交为主
   - 长时间只读分析不应再占着 shared root 不放
4. 治理线程不再依赖 tracked Markdown 作为运行时消息总线。
5. waiting 线程和 `return-main` 后的线程，在队列未清空时仍有可用的非 tracked 草稿区，不会被迫把 `main` 再次写脏。
6. 即使排队、取消、重排、唤醒发生，安全基线仍不能退化。

## 7. 本轮应产出的结果
- 一套新的执行层分层定义：
  - 什么继续留在 `Codex规则落地`
  - 什么迁移到执行层
- 一套新的线程生命周期：
  - 只读准备
  - 申请槽位
  - 等待 / 挂起
  - 最小写事务
  - 归还 shared root
  - 事后证据落盘
- 一套新的运行态承载面方案：
  - 哪些状态进 ignored runtime
  - 哪些状态只在事后进入 tracked 文档
- 一套 rollout 顺序与负例矩阵。

## 8. 一句话口径
- 这条新主线不是继续修“交通规则文档”，而是把 shared root 从“低吞吐单人游乐设施”重构成“安全单写入、并发多准备、短事务快进快出”的执行系统。

## 9. 2026-03-20 事故补洞与新增约束
1. 真实业务批次今后必须区分两种完成态：
   - `carrier-ready`
   - `main-ready`
2. 如果 `main` 上的生产场景、Prefab 或运行入口已经实际依赖某条业务分支才有的运行时资产，则该批次在这些资产真正进入 `main` 前，不得宣称“已恢复正常”。
3. 当前仍未批准“脏改容忍 / 清扫推送”机制；在新的分级方案正式落地前，治理默认口径继续保持：
   - `main` 必须 clean
   - shared root 必须 neutral
   - takeover 必须基于明确白名单与显式验收
4. 重度 Unity / MCP 场景搭建线程应被视为特殊执行类型：
   - Git 隔离与 WIP 承载可以讨论 `worktree`
   - 但 Unity / MCP 仍然只能按单线程独占来执行，不能被误判成普通短事务业务线
