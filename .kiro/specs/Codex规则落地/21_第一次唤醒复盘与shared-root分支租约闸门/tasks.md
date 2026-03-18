# 阶段任务：第一次唤醒复盘与 shared-root 分支租约闸门

## 阶段目标
- 修掉阶段 `20` 后第一次唤醒暴露出的新缺口。
- 让 shared root 的“切分支”也进入物理闸门控制。
- 把线程复工协议从“一轮混合 prompt”升级成“两阶段准入协议”。

## 当前状态
- **阶段状态**：待执行
- **工作性质**：证据收口 + 规则升级 + 脚本闸门 + 认知钢印
- **前置事实**：阶段 `20` 已完成，但第一次唤醒已证明还缺 shared root 租约层

## A. 第一次唤醒证据收口
- [ ] 将 `20/第一次唤醒/` 正式纳入治理证据
- [ ] 对五个线程回包做可信度分级：
  - `NPC`
  - `spring-day1`
  - `farm`
  - `导航检查`
  - `遮挡检查`
- [ ] 明确哪几份结论可以继续沿用，哪几份必须作废或降级为污染样本

## B. shared root 分支租约模型
- [ ] 定义 shared root 的分支租约状态机，至少包含：
  - `main-readonly-window`
  - `exclusive-branch-switch-granted`
  - `task-active`
  - `returned-main`
- [ ] 设计 grant / claim / release 的最小数据载体：
  - 继续沿用 occupancy 文档
  - 或新增独立 lease 文件
- [ ] 明确 `ensure-branch` 在 shared root 上的准入条件：
  - clean `main`
  - occupancy neutral
  - 显式拿到 grant
  - 当前线程身份匹配

## C. 物理闸机补强
- [ ] 修改 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
- [ ] 让 `ensure-branch` 在 shared root 上必须校验独占租约，否则 `exit 1`
- [ ] 明确“切分支也是共享写态”，第一次唤醒阶段禁止发生
- [ ] 设计至少一条负例回归：
  - 第一次唤醒阶段尝试 `ensure-branch`
  - 预期结果：直接 FATAL

## D. 认知钢印补强
- [ ] 审计并重写：
  - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
  - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
  - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
- [ ] 在这些入口里写清：
  - 第一次回复必须先过哪个闸门
  - 未过闸门前禁止切分支、禁止写实现、禁止假设现场
- [ ] 明确这一步是在吸收 `using-superpowers` 的灵魂，而不是安装原版包

## E. 双阶段唤醒协议
- [ ] 重写第一次唤醒 prompt：
  - 只允许只读
  - 明确禁止 `ensure-branch`
  - 只允许输出 carrier 判断、现场判断、最小 checkpoint
- [ ] 新增第二次“准入 prompt”：
  - 只发给拿到 grant 的线程
  - 明确允许执行 `ensure-branch`
  - 明确本轮独占窗口何时归还
- [ ] 重新生成 `NPC / farm / spring-day1 / 导航检查 / 遮挡检查` 的双阶段 prompt

## 当前裁定
- 阶段 `20` 保持已完成，不回滚。
- 阶段 `21` 专门承接：
  - 第一次唤醒复盘
  - shared root 分支租约闸门
  - superpower 式认知钢印本地化补强

## 完成标准
- [ ] 第一次唤醒的证据与分级全部入库
- [ ] shared root 上的 `ensure-branch` 拿不到 grant 就无法执行
- [ ] 新版唤醒协议拆成两轮
- [ ] 关键入口完成 Prompt Injection 级别补强
- [ ] 完成至少一轮负例回归，证明确实能挡住提前切分支
