# NPCV2 线程记忆

## 2026-03-26｜委托-02：Primary / HomeAnchor 准入只读复核

- 当前主线目标：
  - 接班 `NPC` 后，先确认 `Primary.unity` 的 scene 写窗口是否成立，决定能否进入 `HomeAnchor` 最小 scene 切片。
- 本轮子任务：
  - 只读复核 `cwd / branch / HEAD`、`Primary.unity` 的 dirty / diff、shared root / MCP / hot-zones 口径，以及 scene 内 `HomeAnchor` 是否已有直接证据。
- 服务于什么：
  - 避免把 shared root 中性误判成 scene 可写，把 `NPCV2` 第一刀准确钉在“HomeAnchor 最小落点或 blocker”。
- 本轮完成：
  - 已确认 `D:\Unity\Unity_learning\Sunset @ main @ ee3187573b62891a5b0a8d974f43c192c4125a34`
  - 已确认 `Assets/000_Scenes/Primary.unity` 当前仍为 `M`
  - 已确认该 scene 当前 diff 仍为 `76 insertions / 4 deletions`
  - 已确认 `shared-root-branch-occupancy.md` 只说明 shared root `main + neutral`，不等于 scene 可写
  - 已确认 `mcp-single-instance-occupancy.md` 当前虽无 claim，但单实例口径仍是 `single-writer-only`
  - 已确认 `mcp-hot-zones.md` 仍把 `Primary.unity` 列为热区 B / C
  - 已在 `Primary.unity` 内直接搜索 `HomeAnchor`，结果无命中；当前 diff 片段以 `StoryManager`、Workbench overlay、debug flag、Transform 位置改动为主
- 当前是否确认 scene 写窗口成立：
  - `no`
- V2 第一刀或第一 blocker：
  - 第一 blocker：`Primary.unity` mixed dirty + 无明确 owner / 独占写窗口
- 当前恢复点：
  - 本轮不进入 scene，不碰气泡，不抢导航核心
  - 后续只有在 `Primary.unity` dirty 归属明确且拿到安全写窗口时，才恢复到 `scene audit -> HomeAnchor`

## 2026-03-26｜再次只读检查：仍停在 blocker

- 当前主线目标：
  - 复核 `Primary.unity` 的 scene 写窗口是否已变化，确认能否开始 `HomeAnchor` 最小 scene 切片。
- 本轮子任务：
  - 只读重跑 Git / lock / occupancy / hot-zones / `HomeAnchor` 命中检查。
- 本轮完成：
  - 已确认 `D:\Unity\Unity_learning\Sunset @ main @ 519d51bd20d98e662eafb94cea0c5bbbeb314cec`
  - 已确认 `Assets/000_Scenes/Primary.unity` 仍为 `M`
  - 已确认 scene diff 仍为 `76 insertions / 4 deletions`
  - 已确认物理锁仍是 `unlocked`
  - 已确认 `shared-root-branch-occupancy = main + neutral`、`mcp-single-instance-occupancy = single-writer-only`、`mcp-hot-zones` 仍将 `Primary.unity` 视为热区
  - 已确认在 `Primary.unity` 内再次搜索 `HomeAnchor` 仍无命中
- 当前是否确认 scene 写窗口成立：
  - `no`
- V2 第一刀或第一 blocker：
  - 第一 blocker 未变化：`Primary.unity` mixed dirty + 无明确 owner / 独占写窗口
- 当前恢复点：
  - 这轮只能继续只读等待；待 `Primary.unity` 当前 dirty 归属明确后，再恢复到 `scene audit -> HomeAnchor`

## 2026-03-26｜委托-03：共享根大扫除与 owner 报实

- 当前主线目标：
  - 只做 NPC own 文档尾账 cleanup、owner 报实和白名单收口；不把 cleanup 偷换成业务复工。
- 本轮子任务：
  - 分离 NPC own dirty / untracked 与 foreign/hot dirty；
  - 只对白名单路径执行 `sync`；
  - 明确 `Primary.unity` 仍不归本轮认领。
- 本轮完成：
  - 已确认 own 面集中在 `NPC` / `NPCV2` 线程记忆与 `NPC` 工作区文档。
  - 已确认 `Primary.unity`、导航、农田、字体等 mixed 面不属于本轮可认领 cleanup。
  - 已执行白名单收口并生成提交：
    - `eb6284fa`（`2026.03.26_NPCV2_01`，已推送）
  - 发现残留 own 尾账：
    - `.codex/threads/Sunset/NPCV2/memory_0.md` 仍未纳入收口
- 当前恢复点：
  - 本轮业务主线没有变化，仍是 blocker 态
  - 下一步只做最小 follow-up，把 `NPCV2/memory_0.md` 与本轮 cleanup 补记一起收口，然后继续待命

## 2026-03-26｜恢复开工委托-04：Primary.unity 的 001/002/003 HomeAnchor 最小 scene 集成

- 当前主线目标：
  - 把 `Primary.unity` 里 `001 / 002 / 003` 的 `HomeAnchor` 最小 scene 集成真正落下，并把用户验收步骤写清楚。
- 本轮子任务：
  - 先复核 `Primary.unity` 的写窗口、A 类热文件锁、shared root / MCP 基线；
  - 再只做三只 NPC 的 anchor 层级与 scene 引用补口；
  - 最后补一份可直接给用户看的详细验收汇报。
- 服务于什么：
  - 把之前一直卡在 blocker 的 `HomeAnchor` 最小切片正式从“只读裁定”推进到“scene 里已有明确落点”。
- 本轮完成：
  - 已确认开工基线为 `D:\Unity\Unity_learning\Sunset @ main @ 18cf7427d97e749b0557f6d835124e44787c3e17`
  - 已确认 `Check-Lock.ps1` 开工前返回 `Primary.unity = unlocked`
  - 已用 `Acquire-Lock.ps1` 为 `NPCV2` 获取 `Primary.unity` 写锁
  - 已确认 `unityMCP` 当前不可用：
    - `list_mcp_resources / list_mcp_resource_templates` 握手失败
    - `scripts/check-unity-mcp-baseline.ps1` 返回 `listener_missing`
  - 已在 `Primary.unity` 中新增：
    - `NPCs/001_HomeAnchor`
    - `NPCs/002_HomeAnchor`
    - `NPCs/003_HomeAnchor`
  - 已把三只 NPC 的 `homeAnchor` scene 引用补上：
    - `001 -> 001_HomeAnchor`
    - `002 -> 002_HomeAnchor`
    - `003 -> 003_HomeAnchor`
  - 已固定最小基础位置：
    - `001_HomeAnchor`：局部 `(1.86, 0.63, 0)`，世界 `(-6.19, 6.29, 0)`
    - `002_HomeAnchor`：局部 `(-0.68, 0.49, 0)`，世界 `(-8.73, 6.15, 0)`
    - `003_HomeAnchor`：局部 `(1.7, -1.83, 0)`，世界 `(-6.35, 3.83, 0)`
  - 已完成离线 YAML 级自验：
    - 三条 `homeAnchor` override 可回读
    - 三个 anchor 节点可回读
    - `git diff --check -- 'Assets/000_Scenes/Primary.unity'` 通过
  - 已生成用户详细汇报：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-26-NPCV2恢复开工详细汇报-04.md`
- 当前是否把 live 验证也做完：
  - `no`
- 第一 blocker / 现存边界：
  - 当前 blocker 已不再是 `Primary.unity` mixed dirty，而是 `unityMCP` 监听缺失导致无法做 MCP / Play 短窗 live 实证
  - 所以这轮只能如实交付“scene 最小落点已完成 + 需用户在 Unity 里终验”，不能把 live 稳定误报为已确认
- 当前恢复点：
  - 主线已从“只读等待”恢复到“最小 scene 集成完成，待白名单收口与用户终验”
  - 收口后若用户在 Unity 终验中发现运行态异常，下一步从 `HomeAnchor` 赋值链或运行态启动链排查，而不是重新怀疑这轮最小场景层级本身
