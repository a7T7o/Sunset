# 并发 `thread-state` 开工与收口规范（2026-03-31）

## 1. 这份文件解决什么

这不是新方案讨论稿，而是把已经进入 `main` 的 `thread-state` 脚本，正式写成 Sunset 当前 live 执行口径。

它解决的是：

- 线程开工时没人知道“这刀现在是谁在做”
- 共享热点只有到后期清扫时才暴露冲突
- 线程停下来了，但现场还像在施工
- 提交前才回头猜“我这轮到底改了哪些东西”

## 2. 当前模型一句话

当前 Sunset 的并发模型是：

- `Begin-Slice -> 施工 -> Ready-To-Sync -> sync`

如果这一刀中途不继续，则改走：

- `Begin-Slice -> 施工/分析 -> Park-Slice`

## 3. 三个脚本分别是什么

### 3.1 `Begin-Slice.ps1`

作用：

- 开始真实施工前登记这一刀是谁、改什么、共享热点碰哪里

必须在什么时候跑：

- 不是纯只读分析时
- 而是一旦准备开始改 tracked 内容、接盘热文件、或形成这轮白名单切片时

最少要报：

- `ThreadName`
- `CurrentSlice`
- `TargetPaths`
- 如果命中共享热点文件，再加 `SharedTouchpoints`

### 3.2 `Ready-To-Sync.ps1`

作用：

- 在真正 `sync` 之前做最后一层并发与收口闸门

必须在什么时候跑：

- 任何白名单 `sync` 之前

它做什么：

- 检查当前 live 状态
- 检查 B 类共享热点 touchpoint 是否撞车
- 检查 A 类热文件锁是否仍属于自己
- 调稳定 launcher 跑 `preflight`

### 3.3 `Park-Slice.ps1`

作用：

- 合法停车

必须在什么时候跑：

- 当前这轮先停
- 卡住等别人
- 需要把现场让出来
- 本轮不再继续收口

它的意义不是“记日志”，而是把 live 状态从 `ACTIVE` 改成真正的暂停态。

## 4. A / B / C 三类对象怎么理解

### 4.1 A 类

少数真正危险的热文件，按硬锁处理。

当前典型例子：

- `Primary.unity`
- `ProjectSettings/TagManager.asset`
- `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`

### 4.2 B 类

共享热点文件，不做整文件死锁，但必须声明触点。

当前典型例子：

- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`

规则：

- 不同触点可以并行
- 同一触点在 `Begin-Slice` 阶段直接阻断

### 4.3 C 类

普通 own slice，自由施工，但仍然要登记和收口。

## 5. 典狱长 / integrator / 治理位现在怎么看现场

默认先看：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Show-Active-Ownership.ps1`

不再只靠：

- memory
- 回执
- 聊天记录
- 人脑回忆

这些仍然重要，但它们不再承担“实时施工态总览”的职责。

## 6. 过渡规则

当前处于新规接线期，所以规则是：

- 已经在跑的旧线程不需要废弃重开
- 但最晚必须在下一次真实继续施工前补一次 `Begin-Slice`
- 最晚必须在第一次准备 `sync` 前补一次 `Ready-To-Sync`
- 如果当前决定先停，就直接补 `Park-Slice`

也就是说：

- 不重开旧线程
- 但也不允许无限期继续按旧方式裸跑

### 6.1 过渡期统一 prompt 尾巴

为了把旧线程稳定接进这套规则，治理位从现在起可以统一追加：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\线程续工统一尾巴_thread-state补登记_2026-04-01.md`

它的作用不是替代业务正文，而是把：

- `Begin-Slice`
- `Ready-To-Sync`
- `Park-Slice`

这 3 个动作，稳定接到旧线程的下一轮续工 prompt 末尾。

正确理解是：

- 规则本身仍然以 `AGENTS.md`、规范快照和本正文为准
- 统一尾巴只是过渡期的默认接线件
- 新线程第一轮真实施工 prompt，也应默认带上同样口径，直到更上层 launcher / 模板已经自动注入为止

## 7. 现在的真实开发方式，用大白话说

不是“每个线程一开口就自动被系统完全托管”。

现在的真实状态是：

1. 规则已经成立
2. 脚本已经进 `main`
3. 新线程应该按这套方式开工
4. 旧线程需要补登记接入
5. 治理位从现在起默认用这套 live 状态层看现场

所以当前不是“还在纯讨论期”，也不是“已经完全自动驾驶”。

当前阶段更准确的说法是：

- `规则已生效`
- `执行接线正在完成`

## 8. 本文件边界

这份文件只定义：

- 开工登记
- 收口前闸门
- 合法停车
- live 状态总览

它不替代：

- `git-safe-sync.ps1`
- A 类锁规则
- 用户向回执规范
- Unity / MCP live 占用规则

## 9. 为什么不是“强制 skill”

因为 `skill` 和 `live 规范` 不是一回事。

### 9.1 `skill` 是什么

`skill` 更像：

- 某个 Codex 会话里会不会显式暴露
- 助手有没有先读到这套工作流
- 模型有没有按这套流程思考和回复

它擅长的是：

- 帮当前助手理解流程
- 帮当前助手少犯错
- 做前置核查、路由、提示和模板

### 9.2 `skill` 不擅长什么

它不适合单独承担这些事情：

- 作为全项目唯一硬约束
- 作为所有线程都一定会自动执行的入口
- 作为跨线程实时并发状态的物理事实源

原因很简单：

- 有的会话 skill 没显式暴露
- 有的线程是旧 prompt 唤醒的
- 有的现场需要的是“脚本真拦住”，不是“模型应该记得”

### 9.3 为什么 `thread-state` 必须落在仓库脚本 + 规则正文

因为这件事要解决的是：

- 谁正在施工
- 谁准备提交
- 谁已经停车
- 共享热点有没有撞车

这些都属于“运行时现场事实”，不能只靠 skill。

所以当前正确分工是：

- `skill`
  - 负责提醒、路由、让助手知道该走这套流程
- `AGENTS.md + 规范快照 + 治理正文`
  - 负责把它写成项目 live 规则
- `Begin-Slice / Ready-To-Sync / Park-Slice / Show-Active-Ownership`
  - 负责把规则变成现场可执行事实

### 9.4 现在最准确的一句话

不是“强制 skill”，而是：

- `强制 live 规则 + repo 内脚本执行 + skill 作为提示与协助层`

这才是现在 Sunset 该用的口径。
