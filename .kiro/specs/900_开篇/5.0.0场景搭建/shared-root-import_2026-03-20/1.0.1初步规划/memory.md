# 1.0.1初步规划 - 工作区记忆

## 模块概述
- 子工作区名称：`1.0.1初步规划`
- 所属父工作区：`5.0.0场景搭建`
- 阶段目标：在真实场景施工前，固化资产基础、场景骨架、Tilemap 分层、scene 命名、prefab 候选池粒度与阶段准入条件。

## 当前状态
- **完成度**：80%
- **最后更新**：2026-03-20
- **状态**：规划正文已建立并收口关键前置判断，下一步可进入 create-only 级别的新 scene 施工准备。

## 会话记录

### 会话 1 - 2026-03-20（按修正口径重建正文）

**用户需求**：
> 用户明确要求父工作区不堆量，当前正文承载区必须是 `1.0.1初步规划`；请直接开始继续推进。

**完成任务**：
1. 复核现场后确认：用户指定的 `5.0.0场景搭建 / 1.0.1初步规划` 当前并不存在，需要按正确结构重建。
2. 新建当前子工作区正文文件：
   - `requirements.md`
   - `design.md`
   - `tasks.md`
   - `资产普查.md`
   - `memory.md`
3. 直接把当前已收口的关键结论落入正文：
   - 场景骨架与 Tilemap 八层结构
   - 新 scene 推荐名与路径
   - 首版 prefab 候选池三档粒度

**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\requirements.md` - [新增]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\design.md` - [新增]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md` - [新增]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\资产普查.md` - [新增]
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md` - [新增]

**已验证事实**：
- `Assets/000_Scenes` 当前采用扁平目录。
- Build Settings 当前只有 `Primary.unity`。
- 项目存在按字符串加载 scene 的入口：`DoorTrigger.cs`。
- 当前 prefab 规模足以支持独立 scene 开工，但不适合在本阶段做全量资产数据库。

**遗留问题**：
- [ ] 仍需在真实写入前执行一次 create-only 级别的新 scene 施工准入。

### 会话 2 - 2026-03-20（create-only 准入复核）

**用户需求**：
> 继续开始当前主线。

**完成任务**：
1. 只读复核 live Git 现场：
   - `cwd = D:\Unity\Unity_learning\Sunset`
   - `branch = main`
   - `HEAD = 1313977d`
   - 当前 dirty 为本轮新建的：
     - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Skills和MCP\memory_1.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\5.0.0场景搭建\**`
2. 只读复核 shared root 占用文档：
   - `owner_mode = neutral-main-ready`
   - `is_neutral = true`
3. 只读复核 queue：
   - 当前 `current_serving_ticket = null`
   - `next_ticket = 14`
4. 只读复核 Unity / MCP：
   - `mcp-single-instance-occupancy` 当前 `current_claim = none`
   - Unity 活动场景仍是 `Assets/000_Scenes/Primary.unity`
   - Console 抽样只有 MCP 注册日志，无新的场景级错误证据

**关键判断**：
- 当前**不是**“Unity/MCP 正在占用导致不能做 create-only”；
- 当前真正阻塞 create-only 新 scene 创建的，是：
  1. live Git 仍然脏；
  2. 当前还没有为本线程拿到新的 branch grant / create-only 写态准入。

**恢复点 / 下一步**：
- 下一步不是直接建 `SceneBuild_01.unity`，而是先把本轮文档落地后的 live Git / grant 准入问题处理干净，再进入 create-only 写态。

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| 当前推荐 scene 为 `SceneBuild_01` | 中性、稳定、低冲突，适合独立施工承载面 | 2026-03-20 |
| 新 scene 初建时不进入 Build Settings | 避免和 `Primary` 主运行入口混淆 | 2026-03-20 |
| 首版 prefab 候选池采用三档粒度 | 避免规划阶段陷入全量索引泥潭 | 2026-03-20 |
