# 阶段记忆：第一次唤醒复盘与 shared-root 分支租约闸门

## 阶段目标
- 承接阶段 `20` 之后暴露出的新缺口。
- 把“切分支也是 shared root 共享写态”升级成正式闸门。
- 用双阶段唤醒协议和认知钢印，补齐 `superpowers` 灵魂在 Sunset 的本地化落地。

## 当前状态
- **阶段状态**：进行中
- **最后更新**：2026-03-18
- **阶段定位**：证据收口、shared root 分支租约、认知闸门补强

## 当前稳定结论
- 阶段 `20` 已完成，但第一次唤醒证明：
  - 现有闸机能拦住错提交
  - 还不能拦住第一次唤醒阶段的提前切分支
- 第一次唤醒里，真正越线的主要是 `farm`：
  - 提前执行 `ensure-branch`
  - 把 shared root 切到 farm 分支
- `导航检查` 的只读审计结论因此被污染，不应直接视为长期基线
- 这轮后续治理的正确方向不是重做 `20`，而是立 `21` 补：
  - shared root 分支租约闸门
  - 两阶段唤醒协议
  - `AGENTS.md` / `openai.yaml` 级认知钢印

## 会话记录

### 会话 1 - 2026-03-18（阶段 21 立项）
**用户目标**：
> 结合第一次唤醒的真实回包、Gemini 对 superpower 的分析，以及 shared root 再次暴露出的流程漏洞，给出彻底分析，并判断是否需要新阶段继续推进。

**已完成事项**：
1. 通读 `20/第一次唤醒/` 下五份真实回包。
2. 锁定第一次唤醒的最短事故链：
   - `farm` 提前 `ensure-branch`
   - shared root 被切到 farm 分支
   - `导航检查` 在错误 live 分支上做了只读审计
3. 确认当前讨论的 `superpowers` 与此前审查对象一致：
   - `obra/superpowers`
   - 核心 skill：`using-superpowers`
4. 确认本地已有可承接“认知钢印”的入口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`

**关键决策**：
- `21` 不是重做 `20`，而是补 `20` 之后第一次唤醒才显现的新缺口。
- `21` 的核心不在业务代码，而在 shared root 的分支租约与认知入口闸门。
- 原版 `superpowers` 继续不装；本地化吸收它的灵魂才是正路。

**恢复点 / 下一步**：
- 先把 `第一次唤醒/` 证据正式纳入 Git。
- 然后按 `tasks.md` 依次推进：
  - 分支租约模型
  - 脚本补强
  - `AGENTS.md` / `openai.yaml` 认知钢印
  - 双阶段唤醒 prompt

### 会话 2 - 2026-03-18（阶段 21 物理闸机与钢印配置落地）
**用户目标**：
> 不再停留在规划或原理解说，直接把 shared root 分支租约闸机、`AGENTS.md` 钢印、skill `default_prompt` 和双阶段唤醒模板落到物理文件，并给出可验收成品。

**已完成事项**：
1. 在 `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1` 落地 shared root 分支租约机制：
   - 新增 `grant-branch`
   - 新增 `return-main`
   - `ensure-branch` 从 shared root 的 `main` 进入前必须先通过 `Assert-SharedRootBranchGrant`
2. 在 `D:\Unity\Unity_learning\Sunset\.kiro\locks\shared-root-branch-occupancy.md` 增补 grant 字段与解释口径：
   - `branch_grant_state`
   - `branch_grant_owner_thread`
   - `branch_grant_branch`
   - `branch_grant_updated`
3. 把强制钢印块插到 `D:\Unity\Unity_learning\Sunset\AGENTS.md` 最顶部，明确：
   - 第一次回复先过 `sunset-startup-guard` / `skills-governor`
   - 没有显式 Lease / Grant 前，禁止 `ensure-branch`
   - 第一次唤醒阶段必须纯只读
4. 在本机 live skill 配置中落地强制 `default_prompt`：
   - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
   - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
5. 低风险验证通过：
   - `preflight` 可正常读取并输出 grant 字段
   - 用匹配线程名的假分支执行 `ensure-branch` 时，会被“未拿到独占分支租约”硬阻断

**关键决策**：
- shared root 上的 `ensure-branch` 正式被定性为全局写态，不再允许把它当成线程局部动作。
- 第一次唤醒与第二次正式进入业务分支必须分成两阶段；第一次只读，第二次先发 grant 再准入。
- `superpowers` 继续只吸收“认知钢印 + 前置闸门”的灵魂，不引入其 worktree-first 实现。

**恢复点 / 下一步**：
- 产出并分发新的双阶段唤醒模板。
- 仓库内治理文件完成白名单同步后，可按新闸机重新组织业务线程恢复。
