# 阶段分析：第一次唤醒复盘与 shared-root 分支租约闸门

## 1. 立项原因
- 阶段 `20` 已经把 shared root 拉回了 `main + neutral`，也落下了：
  - `git-safe-sync.ps1` 的分支语义闸机
  - `main` 禁止 `task` 模式写入
  - shared root occupied 时的 owner / remaining dirty 阻断
- 但 `20` 结束后的第一次线程唤醒，仍然暴露出一个新缺口：
  - 在 shared root 上，**切分支本身就是共享写态**
  - 现有闸机拦住了“错分支提交”，却没有拦住“第一次唤醒阶段就提前切分支”
- 这意味着 `20` 不是失败，而是只完成了第一段物理防线；新的缺口应由 `21` 接管。

## 2. 第一次唤醒的真实表现
- `NPC`
  - 基本合规
  - 停留在只读比对，没有切分支，没有进入 Unity 写态
- `spring-day1`
  - 基本合规
  - 停留在只读 preflight，没有切分支
- `farm`
  - 越线样本
  - 在第一次唤醒阶段提前执行 `ensure-branch`
  - 把 shared root 从 `main` 切到 `codex/farm-1.0.2-cleanroom001`
  - 之后在用户指出后切回 `main`
- `导航检查`
  - 结果被污染
  - 虽然自称只读，但其审计时 live 分支已经是 farm 分支
  - 因此它关于“当前现场不是 main”的判断只反映当时被污染后的现场，不代表长期基线
- `遮挡检查`
  - 结果基本可信
  - 它的只读审计是在 `main` 现场上完成的，污染较小

## 3. 最短事故链
1. 阶段 `20` 结束后，shared root 的默认口径已回到 `main + neutral`
2. 第一次唤醒 prompt 允许线程在同一轮里“如果要进入真实任务，就可以 ensure-branch”
3. `farm` 把“基线复核”直接推进成了 `ensure-branch`
4. shared root 被切到 farm 分支
5. `导航检查` 在错误 live 分支上继续做了只读审计
6. 用户从 VS 文档视图异常中发现 shared root 已被切偏

## 4. 问题本质
- 不是“现有闸机没用”
- 而是“现有闸机只管提交与同步，还没把 shared root 的**分支切换**纳入独占租约”

更具体地说，当前还缺 3 件事：

### 4.1 shared root 分支租约闸门
- `ensure-branch` 现在只校验：
  - 当前在 `main`
  - occupancy 文档为 neutral
  - 工作树干净
- 但它没有要求“先拿到 shared root 的独占授权”
- 结果就是：任何线程只要看到 shared root clean，就可能把它切走

### 4.2 双阶段唤醒协议
- 第一次唤醒应该只有一个目标：
  - 只读确认当前 carrier、当前 live 现场、当前最小 checkpoint
- 进入任务分支应当是第二次、且拿到准入后才能做的事
- 目前把这两步混在一轮 prompt 里，太容易让线程“理解过度”

### 4.3 认知钢印还不够硬
- 当前本地已经有：
  - `C:\Users\aTo\.codex\skills\skills-governor\agents\openai.yaml`
  - `C:\Users\aTo\.codex\skills\sunset-startup-guard\agents\openai.yaml`
- 也已经有 `AGENTS.md` 与 `git-safe-sync.ps1`
- 但这些入口还没有写成“**第一次回复就必须先过哪一个闸门，否则直接视为违规**”的强硬表述
- 这正是 `superpowers` 的“灵魂”真正要补上的部分

## 5. 我对 Gemini / superpowers 结论的综合判断
我确认这次我们讨论的 `superpowers`，与此前审查的对象是同一个：
- 项目：`obra/superpowers`
- 核心 skill：`using-superpowers`
- 安装入口：`D:\Temp\superpowers-vet\.codex\INSTALL.md`

我也确认 Gemini 对它的理解，与当前本地审查结果高度一致：
- 我们要的是它的“灵魂”：
  - 强制技能路由
  - 极端 Git 安全
  - 系统级 Prompt Injection
- 不是它的“肉体”：
  - `worktree-first`
  - 私有 Skill 工具前提
  - 不适配当前 Sunset 的 Node/JS 测试与工作树框架

所以 `21` 的路线不是“安装原版 superpowers”，而是：
1. 保持 `obra/superpowers = rejected-as-is`
2. 吸收其认知钢印与入口劫持方法
3. 在本地环境里做两层落地：
   - 认知闸门：`AGENTS.md` + `openai.yaml default_prompt`
   - 物理闸门：`git-safe-sync.ps1` + shared root lease / grant 机制

## 6. 21 阶段的目标
- 把第一次唤醒事故收成正式证据和可信度分级
- 把“shared root 分支切换 = 共享写态”写成正式治理规则
- 为 `ensure-branch` 补 shared root 独占租约闸门
- 把线程恢复协议拆成两阶段：
  - 第一轮：只读唤醒
  - 第二轮：获准进入任务分支
- 把 `superpowers` 的认知钢印落实到：
  - `Sunset/AGENTS.md`
  - `skills-governor/agents/openai.yaml`
  - `sunset-startup-guard/agents/openai.yaml`

## 7. 完成判据
- 第一次唤醒的新证据全部入库，不再悬空未跟踪
- `farm` 这类“第一次唤醒就提前切分支”的行为会被脚本或租约闸门直接阻断
- 新版唤醒 prompt 清晰拆成两轮，不再在第一轮夹带条件式 `ensure-branch`
- `AGENTS.md` 和本地关键 skill 的 `default_prompt` 中写入强硬的“先过闸门，再允许思考实现”口径
- 完成一次受控回归，确认 shared root 在第一次唤醒阶段保持 `main`
