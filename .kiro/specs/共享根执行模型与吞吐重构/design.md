# 共享根执行模型与吞吐重构 - 设计说明

## 1. 设计目标
- 把 Sunset 当前的“安全治理层”和“执行调度层”明确拆开。
- 保留已经稳定的安全能力，不再让治理层继续吞掉执行层职责。
- 让 shared root 的使用方式从“长时间占据式工作”改成“最小写事务式工作”。

## 2. 现状分层重判

### 2.1 应继续留在 `Codex规则落地` 的内容
- `AGENTS.md` 入口闸门
- `skills-governor / sunset-startup-guard` 的前置核查口径
- `git-safe-sync.ps1` 的安全校验与白名单提交
- shared root / Unity / MCP 占用规则
- 事故复盘、runbook、历史阶段封板、基线记忆

### 2.2 必须迁移出旧治理工作区的内容
- 高频 queue 编排细节
- tracked 回执单驱动的运行时调度
- 等待态线程的持续 tracked 写回
- “谁现在能进场”这种高频 runtime 消息分发

## 3. 新执行模型

### 3.1 三层结构
1. 安全治理层
   - 负责规则、硬闸机、事故恢复、基线定义。
   - 主承载面：`Codex规则落地`

2. 运行态调度层
   - 负责 request / yield / granted / wake / cancel / requeue 这类短期状态流转。
   - 主承载面应以 ignored runtime 文件和命令输出为主，而不是 tracked Markdown。

3. 线程工作层
   - 负责每个业务线程自己的只读准备、方案整理、最小 checkpoint、业务落地。
   - 持有 shared root 时只允许做最小写事务。

### 3.2 线程生命周期
1. `只读准备态`
   - 线程读取规则、live 现场、carrier 分支内容。
   - 允许整理方案、checkpoint 规划、恢复点。
   - 不占用 shared root 写槽位。

2. `申请槽位态`
   - 通过 stable launcher 发起申请。
   - 返回结果只分为：
     - `GRANTED / ALREADY_GRANTED`
     - `LOCKED_PLEASE_YIELD`
     - `FATAL`

3. `等待 / 挂起态`
   - 收到 `LOCKED_PLEASE_YIELD` 后，不视为失败。
   - 线程只保留最小聊天回执或写入非 tracked runtime 恢复点。
   - 不应再向 tracked repo 写回回执、memory 或批次文档。

4. `最小写事务态`
   - 只有拿到 grant 后，线程才允许 `ensure-branch` 进入真实任务分支。
   - 在槽位内只做最小必要动作：
     - 落一个 docs-first checkpoint
     - 落一个最小代码 checkpoint
     - 明确无法继续时快速 return-main

5. `归还 / 事后落盘态`
   - 完成 `return-main` 后，槽位立即归还。
   - tracked memory、治理补卷、审计总结应尽量放到槽位释放之后处理。

## 4. 关键设计原则

### 4.1 持槽期最小化
- 不在持槽期做长时间只读分析。
- 不在持槽期补治理回执。
- 不在持槽期顺手修改与当前任务无关的治理文档。
- 能在进入槽位前准备好的内容，全部提前准备。

### 4.2 运行态与证据层分离
- 运行态：
  - ignored JSON
  - gitignored Draft 沙盒
  - 命令行状态输出
  - 短期 ticket / wake 信息
- 证据层：
  - 事后 memory
  - 阶段总结
  - runbook
  - 治理快照

### 4.3 carrier 可读，不等于必须 checkout
- 线程等待期间，仍可通过只读方式查看自己的 carrier 分支内容。
- 目的不是“让所有线程都切到自己分支上挂着”，而是“让只有真正要写的人才切分支”。

## 5. 推荐的运行态承载面

### 5.1 保留
- `.kiro/locks/shared-root-branch-occupancy.md`
  - 继续作为人可读的 shared root 基线状态文档。
- `.kiro/locks/active/*.lock.json`
  - 继续作为 ignored runtime 状态承载面。

### 5.2 收紧
- tracked 批次分发文件
  - 只用于一轮分发入口与事后证据，不再承载运行时连续流转。
- tracked 线程回收卡
  - 不再要求 waiting 线程在 `main` 上实时写回。

### 5.3 Draft 沙盒
- 位置：
  - `D:\Unity\Unity_learning\Sunset\.codex\drafts\<OwnerThread>\`
- 规则：
  - 仅用于等待态、挂起态、或 `return-main` 后但队列仍未清空时的草稿承载
  - 默认不进入 Git dirty 检查
  - 不替代正式 memory、回执或代码提交
- 作用：
  - 让线程在没拿到槽位时仍能把思路和代码草稿准备好
  - 拿到 grant 后只迁入最小 checkpoint 所需内容，实现快进快出

## 6. 第一轮实施方向
1. 先定义“持槽期 / 非持槽期”的明确边界。
2. 再把 waiting、wake、cancel、requeue 的 runtime 状态尽量抽到 ignored 层。
3. 给 waiting / post-return 线程补一个 gitignored Draft 沙盒。
4. 最后才决定哪些 tracked 文档还值得保留为事后证据，以及何时允许它们最小落盘。

## 7. 风险
- 如果继续把 tracked Markdown 当运行态消息总线，shared root 仍会被治理动作重新写脏。
- 如果不定义持槽时间边界，线程就算有 queue 也仍会长时间霸占 shared root。
- 如果把执行层全部交给人工群发与人工回填，吞吐最终还是卡在人。

## 8. 一句话口径
- 新设计不是降低安全，而是把安全层固定住之后，把执行层从“厚治理”里拆出来。

## 9. 2026-03-20 补充设计

### 9.1 `carrier-ready` 与 `main-ready` 必须分离
- `carrier-ready` 只回答一件事：
  - 某条 continuation branch 内是否已经拥有相对完整的业务内容
- `main-ready` 回答的是另一件事：
  - 当前 `main` 是否已经具备生产场景、运行入口和共享资源真正依赖的运行时资产
- 这次 NPC 事故证明，仅凭 `carrier` 更干净、`sync` 成功、`return-main` 成功，并不能推出 `main` 上的生产场景已经恢复正常。
- 因此后续真实业务批次的治理回收需要新增一类验收：
  - 若本轮涉及生产场景、共享 Prefab、Sprite meta、Animator Controller、ScriptableObject、运行时代码链路，则必须显式回答 `main-ready` 是否成立。

### 9.2 当前不开放隐式 dirty 容忍
- 用户提出的“清扫推送 / dirty 分级容忍”方向是合理议题，但它现在仍处于讨论阶段，不属于已批准机制。
- 在新方案落地前，执行层继续坚持：
  - 不允许把“当前看起来能跑”直接等同于“可以带脏交接”
  - 不允许跳过 ownership、白名单、回收口径，直接把 shared root 当公共脏工作台
- 如果未来要开放 dirty 容忍，必须至少同时具备：
  - 明确的 dirty 分级
  - 明确的 takeover 条件
  - 明确的禁止域
  - 明确的回滚与自愈方式

### 9.3 重度 MCP 场景搭建线程属于特殊执行类型
- 这类线程不是普通“快进快出”的共享根短事务。
- 它的特点是：
  - 长时间占用 Unity / MCP
  - 高频 Inspector / Scene / Prefab 回读
  - 强依赖连续视觉判断，不适合多线程拆分
- 对它的推荐模型是：
  - 线程级单写者独占
  - 优先在验证场景或独立搭建场景中工作
  - `worktree` 只解决 Git/WIP 隔离，不解决 Unity / MCP 并行
  - 真正触碰 `Primary.unity` 时，应作为单独波次处理，而不是混入普通代码批次
