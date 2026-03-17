# 设计说明：强制 skills 闸门与执行规范重构

## 1. 问题定义
- 当前 `Sunset` 已经有：
  - 现行入口
  - Git 规则
  - 物理锁
  - 多个治理型 skills
- 但真正缺的是：
  - 每次进入 Sunset 实质性任务时，一个会被优先触发的项目级启动闸门；
  - 一个把“主线锚定、工作区路由、Git 判路、锁风险、同伴 skills 选择、回复结构”一次性定清的强制前置层。

## 2. 当前失败点
- 规则写在文档里，但没有稳定变成“先执行什么”的硬门槛。
- `skills-governor` 是通用治理 skill，不等于 Sunset 项目专用的启动守卫。
- `AGENTS.md` 虽然已经写了很多路由与优先级，但对“必须先触发哪个 skill”仍然不够硬。
- 线程回复规范更多停留在模板和文档层，没有上升到“先触发再输出”的执行闸门。
- Unity / MCP 一直被默认当成“有空就能读写”的共享资源，缺少独立的单实例占用和热区层。

## 3. 目标架构
### 第 0 层：用户当前指令
- 继续是最高优先级。

### 第 1 层：Sunset 项目级启动闸门
- 任何 Sunset 实质性任务先经过它。
- 它负责回答：
  - 当前主线是什么；
  - 本轮是不是阻塞处理；
  - 当前应该进入哪个工作区；
  - 先读哪些活文档；
  - 本轮是治理 / task / 只读核查；
  - 是否涉及 A 类热文件；
  - 是否涉及 Unity / MCP 单实例热区；
  - 下一步需要哪些同伴 skills。

### 第 1.5 层：Unity / MCP 单实例层
- 它不替代 Git、shared root 占用或 A 类热文件锁。
- 它只负责回答：
  - 当前 Editor / MCP 是否适合继续读写；
  - 当前是否命中 Play / Compile / Domain Reload / Scene / Prefab / Inspector 热区；
  - 出现对象失效、端口占用、中间态读数时是否必须先退回只读。
- 对应落盘位置：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-hot-zones.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-log.md`

### 第 2 层：同伴 skills
- 由启动闸门选择并转交，例如：
  - `sunset-workspace-router`
  - `skills-governor`
  - `sunset-lock-steward`
  - `sunset-thread-wakeup-coordinator`
  - `sunset-doc-encoding-auditor`
  - `sunset-release-snapshot`
  - `sunset-unity-validation-loop`

### 第 3 层：现行活文档与 AGENTS
- 用来提供项目当前口径与边界。
- 不再单独承担“强制触发”的职责。

## 4. 最小输出契约
- 启动闸门完成后，第一条 `commentary` 至少要包含：
  - 当前主线目标；
  - 本轮子任务或阻塞；
  - 命中的工作区；
  - 先要读取/核查的文件；
  - 需要使用的同伴 skills；
  - 第一执行动作。

## 5. 线程回复契约
- 后续需要给各线程统一到的最低结构：
  - 当前主线目标
  - 当前工作目录 / 分支 / `HEAD`
  - 本轮实际动作
  - 是否触及 Unity / MCP 单实例热区
  - 是否触及共享热文件
  - 已验证事实
  - 当前阻塞
  - 下一步

## 6. 当前策略
- 这轮先把该架构写成正式阶段设计与代办。
- 实际 skill、`AGENTS.md`、活文档重写放入本阶段后续动作。
- 这样可以避免在 `NPC` 修复阻塞出现时，治理线程继续沉迷大范围重写。

## 7. 非目标
- 不是现在就重写全部历史规则文档。
- 不是把所有现有 skills 推翻重来。
- 不是为了“形式统一”去制造新的文档膨胀。
