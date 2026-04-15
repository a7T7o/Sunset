# 存档系统 - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/memory_0.md)。本卷只保留当前主线与恢复点。

## 当前定位
- 本工作区继续作为存档主线索引，但不再把旧 3.7.x 基底、三场景持久化、Home seed 化、工作台刷新、存档 UI 尾差继续混在一条根卷里。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `4.0.0_三场景持久化与门链收口`

## 当前稳定结论
- 当前重点已经不是“该不该做持久化”，而是：
  - `Primary / Town / Home` 三场景 persistent baseline 的 live 门链收口
  - Home seed 化后的体验尾差
  - 持久化 UI / runtime-first 资产定位 / 工作台刷新合同

## 当前恢复点
- 后续三场景持久化、Home、persistent player、workbench 刷新、存档 UI 尾差统一先归：
  - [4.0.0_三场景持久化与门链收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
- 查旧 3.7.x 基底和历史索引时，再回看 `memory_0.md`

## 2026-04-13 追加索引｜Day1 restore hygiene 已落到 4.0.0，off-scene world-state 仍停合同层
- 当前 `存档系统` 主线没有换：
  - 仍统一回 `4.0.0_三场景持久化与门链收口`
- 今日新增的有效进展是：
  - `SaveManager` 已补 `load/restart` 前的统一恢复卫生链
  - `Day1` 的 stale prompt / stale modal / stale pause / stale input 已不再只靠 story snapshot 自己兜底
  - 新增了 `SaveManagerDay1RestoreContractTests` 作为轻量 source-contract 护栏
- 今日明确没有越权继续做的是：
  - `off-scene world-state` 仍未正式入存档
  - 当前结论仍是：
    - 正式存档只收当前 loaded scene 的 `worldObjects`
    - 已离场 scene continuity 仍在 `PersistentPlayerSceneBridge`
    - 后续若真要入盘，必须走单独 per-scene snapshot 合同，而不是把它粗暴并进现有 `worldObjects`
- 详情恢复点已写入：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-14 追加索引｜build fail 归因已落到 4.0.0
- 本轮用户贴出的打包日志已经做过只读归因。
- 当前明确结论：
  - 这次真正阻塞 Player build 的不是 `SaveManager`
  - 真实红错是 `DayNightManager` 在 runtime 编译面调用了仅 `UNITY_EDITOR` 下存在的 `EditorRefreshNow()`
- `CloudShadowManager` 的 `ExecuteAlways / EditorUpdate / DontSave` 断言噪音已记录为次级治理项，但不是当前第一主因。
- 后续若进入真实施工，仍统一先回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
