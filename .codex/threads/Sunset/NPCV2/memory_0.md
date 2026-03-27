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

## 2026-03-26｜运行中 Inspector 自动补口回归修复

- 当前主线目标：
  - 不停 Unity 的前提下，把 `HomeAnchor` 自动补口从“会炸 Play Mode”修回“运行中可继续用”。
- 本轮子任务：
  - 只修 `Assets/Editor/NPCAutoRoamControllerEditor.cs` 的运行态分支。
- 本轮完成：
  - 已定位异常源头：
    - `TryAutoRepairPrimaryHomeAnchors()` 在 Play Mode 里调用了 `EditorSceneManager.MarkSceneDirty`
  - 已改成双路径：
    - `Play Mode`：只做运行时 anchor 创建/查找与 `controller.SetHomeAnchor(anchor)`，不碰 `Undo / MarkSceneDirty / 持久化`
    - `Edit Mode`：保留 scene 持久化补口逻辑
  - 这样用户当前运行中的 Unity 不需要停，也不会再因为 Inspector 自动补口而直接抛同一条 `InvalidOperationException`
- 当前恢复点：
  - 等 Unity 重新编译后，用户只要重新点回 `001 / 002 / 003` 的 Inspector，就可以看 `Home Anchor` 是否自动回正

## 2026-03-26｜当前认领边界与主线状态自校准

- 当前主线目标：
  - 别把 `NPCV2` 已完成的两刀偷换成“顺手吞 `Primary.unity` / 字体 / 导航 runtime 的 cleanup”。
- 本轮只读结论：
  - 我现在认领：
    - `65e1ee35` 的 `Primary.unity` 最小 `HomeAnchor` 集成
    - `24886aad` 的 `NPCAutoRoamControllerEditor.cs` Play Mode 热修
    - `NPCV2` 自己的工作区 / 线程记忆与用户验收承接
  - 我现在不认领：
    - 3 份 `DialogueChinese*` 字体 dirty
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs` 当前 diff
    - 当前整张 `Primary.unity` mixed hot 面
  - `Primary.unity` 里仍能看见 `homeAnchor / *_HomeAnchor` 痕迹，说明 scene 脏面中混有我这条线碰过的区域；
    但当前 diff 量级已经不是纯 residue，不能直接进入 cleanup
- 当前该不该 cleanup：
  - `no`
- 当前主线：
  - 等用户验证 `HomeAnchor` 自动补口 / 运行态是否真的回正
  - 同时把 own / non-own 边界守住，不再扩刀
- 当前支线：
  - 若治理继续追问，只做只读 owner 报实，不做 scene / 字体 / runtime 清扫
- 当前恢复点：
  - 现在最正确的动作是“停在边界说明与用户裁定”，而不是再去清 mixed scene

## 2026-03-26｜`NPCAutoRoamControllerEditor.cs` 继续只补运行中 `Home Anchor` 显示链

- 当前主线目标：
  - 继续围绕 `Assets/Editor/NPCAutoRoamControllerEditor.cs` 修 Play Mode Inspector，让 `Home Anchor` 从空变成可见非空，或者把剩余断点压成用户一眼能看到的小点。
- 本轮子任务：
  - 只改 Editor 脚本，不碰 `Primary.unity` mixed cleanup、不碰字体、不碰 `NPCAutoRoamController.cs` runtime。
- 本轮完成：
  1. 只读核实后确认：
     - `Primary.unity` 中 `001 / 002 / 003` 的 `homeAnchor` scene override 已在；
     - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 也都已在 `NPCs` 根下；
     - 因此问题更像 Inspector 的 Play Mode live 显示链，而不是 scene 本身缺 anchor。
  2. 已改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`：
     - 改为手工绘制 Inspector 字段；
     - Play Mode 下 `Home Anchor` 优先显示 `controller.HomeAnchor` 的 live 引用；
     - 若序列化缓存落后于 live 引用，则同步 `_homeAnchorProperty` 并 `Repaint()`；
     - 若运行中仍空，会在 Inspector 里显示精确 warning：
       - sibling anchor 存在但 runtime binding 仍空；
       - 或当前父节点下没有 anchor。
  3. 自动补口与 warning 都统一走 `FindExistingPrimaryHomeAnchor(...)`，避免两套查找路径再互相打架。
- 验证结果：
  - `git diff --check -- 'Assets/Editor/NPCAutoRoamControllerEditor.cs'`：通过
  - `unityMCP validate_script`：因 `no_unity_session` 被阻塞，当前无法在本线程拿到 Unity 侧编译/脚本验证回执
- 当前 owned 边界：
  - owned 只有：
    - `Assets/Editor/NPCAutoRoamControllerEditor.cs`
    - 本线程 / 工作区记忆
  - 非 owned 仍然包括：
    - `Assets/000_Scenes/Primary.unity`
    - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
    - 3 份 `DialogueChinese*`
- 当前恢复点：
  - 现在回到用户最直接可验的那一步：
    - 在不停 Unity 的前提下重选 `001 / 002 / 003`；
    - 若 `Home Anchor` 仍空，Inspector 内 warning 已把断点压窄到一个明确可见的小点

## 2026-03-26｜口径自纠：我报的是当前局部尾项，不是整条 NPC 线的全量待办

- 用户对我提出的质疑是成立的：
  - 如果直接说“现在代办只剩这些”，容易让人误以为 NPC 整条线只剩 `HomeAnchor` Inspector 补口链这几个尾项。
- 更准确的表达应是：
  1. 对我这个 `NPCV2` 当前刀口来说，只剩：
     - 运行中 `HomeAnchor` 是否稳定 non-empty 的最终实证
     - 若仍为空，把断点继续压窄到更小的 Editor / Inspector 事实
     - own 文档与记忆收口 / sync
  2. 但对整个 NPC 主线来说，仍还有：
     - 正式 roam 区域 / 路线 / 相遇节奏
     - 导航交付后的 NPC 联调
     - Unity Test Runner 的工具链实跑
  3. 另外还有一批当前不归我吞并的内容：
     - `Primary.unity` mixed cleanup
     - `DialogueChinese*` 字体
     - `NPCAutoRoamController.cs` runtime / 导航 diff
- 后续对用户汇报时必须分层：
  - 我当前这刀的尾项
  - NPC 总线的剩余项
  - 非我认领范围

## 2026-03-27｜按用户要求完成文档型总回溯：总需求总表 + 时间线都写进 `NPCV2` 目录

- 当前主线目标：
  - 不再让 `NPCV2` 的局部尾项替代整条 NPC 线的历史需求底账，而是基于文档重新完整回溯用户前后提出过的全部要求。
- 本轮完成：
  - 新建：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPCV2\2026-03-27_NPC全需求总表与时间线_文档回溯版.md`
  - 这份文档不是聊天复述，而是只基于：
    - `NPC` 主工作区 `memory`
    - `1.0.0初步规划` / `2.0.0进一步落地`
    - `NPC` / `NPCV2` 线程记忆
    - V2 交接文档
    - 3 月 26 日委托链
    - 外部治理交叉文档
  - 文档里已把每条需求尽量按：
    - 原文
    - 稳定转述
    - 来源文件
    - 当前状态
    - 是否已完成
    落盘，并单独说明了“逐字原话缺失”的部分。
- 当前恢复点：
  - 现在如果再问“NPC 到底全部提过什么要求”，应先读这份总表，而不是继续靠我当前刀口的局部回执去猜。

## 2026-03-27｜我已按用户要求进入“全盘交接模式”

- 用户纠正点：
  - 用户明确要求我停止“只围着眼前这一个 `HomeAnchor` 点转”，必须基于总表彻底治好上下文失忆，进入全盘交接模式。
- 本轮重新校准后的认知：
  1. 我之前最大的偏差是：
     - 把 `NPCV2` 当前局部刀口误当成整条 NPC 主线的剩余项
  2. 现在以后固定要用的框架是：
     - 第一层：整条 NPC 总线
     - 第二层：`NPCV2` 当前切片
     - 第三层：明确不认领范围
  3. 在这个框架下，`NPCV2` 当前只是一把很窄的刀：
     - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 的运行中 `Home Anchor` live 实证 / 继续压窄
     - own 记忆 / 白名单收口
  4. 但整条 NPC 总线还远没有结束，至少仍包括：
     - 正式场景化真实落点
     - roam 区域 / 路线 / 相遇节奏
     - 角色化日常与专属对话
     - 玩家 / NPC 双气泡
     - 好感度 / 关系成长
     - 受击 / 工具命中反应
     - 导航线程交付后的 NPC 联调
     - Unity Test Runner 的工具链正式实跑
- 当前恢复点：
  - 以后任何“还剩什么 / 下一步是什么 / 你该不该 cleanup / 你该不该继续主线”的回答，都先按这三层说清楚，再落到当前最近一刀。

## 2026-03-27｜已把“全量未完项 + 优先级顺序”正式落成清盘文档

- 当前主线目标：
  - 不再只停留在“我脑子里知道三层结构”，而是把接下来整个 NPC 线该怎么推进，正式写成一份可验收、可执行的清盘清单。
- 本轮完成：
  - 新建：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\2026-03-27-NPC全盘清盘清单与后续优先级方案.md`
  - 文档已明确：
    1. 当前哪些部分已可视为稳定基线
    2. 全量未完成项到底分哪几类
    3. 当前明确不该做什么
    4. 后续正式优先级应如何重排
  - 最终排序被固定为：
    - `P0` `HomeAnchor` 基线闭环
    - `P1` 场景化真实落点最小可玩切片
    - `P2` 角色化内容稳定
    - `P3` 轻交互 / 双气泡
    - `P4` 关系成长最小接线
    - `P5` 命中 / 工具反应兼容
    - `P6` 导航联调与正式测试
- 当前恢复点：
  - 现在后续如果要继续推进，我不该再临场凭感觉排刀，而应先回到这份文档，再确认当前是总线中的哪一层、哪一刀。

## 2026-03-27｜清盘文档已继续下钻成“详细任务列表”

- 当前主线目标：
  - 用户要求把 NPC 后续所有要做的内容进一步压成“可以直接照着做、后面直接拿来卡我验收”的详细任务列表。
- 本轮完成：
  - 新建：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
  - 这份新文档已经把后续任务压到任务卡颗粒度，不再只是阶段规划：
    - 每条都有任务编号
    - 每条都有前置条件
    - 每条都有责任归属
    - 每条都有可碰范围 / 禁碰范围
    - 每条都有产出物与完成标准
    - 每条都有失败判定
  - 也就是说，后续我如果继续推进，不应再只说“接下来做 P0”，而必须明确说：
    - `T-P0-01`
    - `T-P0-02`
    - `T-P0-03`
    这种级别的具体任务编号
- 当前恢复点：
  - 后续真正的施工标准已经从“宽口径清盘清单”升级成“详细任务列表”；我下一轮如果继续推进，必须按这份列表逐条执行。

## 2026-03-27｜正式回到 `P0` 开工：先补 Editor 诊断，不误跳 `P1`

- 当前主线目标：
  - 用户要求“做完 git 就直接开始彻底落盘所有任务”，我已按详细任务列表重新开工，但严格从 `T-P0-01 ~ T-P0-05` 开始，不跳到 `P1/P2`。
- 本轮子任务：
  - 用 `skills-governor + 手工 startup 闸门` 重新核现场；
  - 只推进：
    - `T-P0-02`
    - `T-P0-03`
  - 对：
    - `T-P0-01`
    - `T-P0-05`
    做真实阻塞判定。
- 本轮完成：
  - 已确认：
    - `Primary.unity` 里 `001 / 002 / 003` 的 `homeAnchor` scene override 仍在
    - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 仍在，且父级结构一致
  - 已确认：
    - 当前并不是 scene 丢 anchor
    - 更像运行中 Inspector / auto-repair 链问题
  - 已修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCAutoRoamControllerEditor.cs`
  - 这次修改新增了：
    - `Runtime / Serialized / Detected / Parent / Auto-repair` 五段诊断
    - 自动补口结果回显
    - `parent-sibling / self-child / scene-search` 三层查找来源
    - `Create Home Anchor` 与当前 scene sibling 口径统一
  - 已新增用户复验文档：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P0-HomeAnchor基线复验与失败判读.md`
  - 已同步更新详细任务账本中的实时状态。
- 本轮验证：
  - `git diff --check` 已通过上述代码与文档。
  - `Editor.log` 可见脚本修改后触发了：
    - `Requested script compilation`
    - `Reloading assemblies`
    - `CompileScripts`
  - 当前未抓到我这轮新引入的 `error CS` 证据。
- 当前 blocker：
  - `unityMCP` 当前是“基线 pass、session missing”：
    - `check-unity-mcp-baseline.ps1 = pass`
    - 但 `editor/state`、`project/info`、`manage_scene(get_active)` 仍返回 `no_unity_session`
  - 因此：
    - `T-P0-01` 还不能 claim done
    - `T-P0-05` 也还不能 claim done
- 当前恢复点：
  - 这轮最正确的下一步是让用户按新文档复验 `001 / 002 / 003` 的 Inspector；
  - 只有拿到这组 live 读数后，才能决定我是继续补 `Editor` 这一小刀，还是把 `T-P0-02` 判成 done 并进入 `T-P0-05`。

## 2026-03-27｜在高速模式下继续前推：先收 `P1-01`，再把 `P2` 内容层提前落盘

- 当前主线目标：
  - 用户已明确授权“做一部分、能测就测、测不了就记日志继续”，所以在 `Primary.unity` 热区未开的情况下，我这轮选择不空转等待，而是继续推进 NPC 自己能独立完成的下一批非 live 切片。
- 本轮子任务：
  - 正式收口 `T-P1-01`
  - 新增 `P2` 的内容资产载体
  - 为 `001 / 002 / 003` 建立正式角色内容资产，并接回当前相遇链
- 本轮完成：
  - 新增：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCDialogueContentProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterDialogueContent.asset`
    - `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchDialogueContent.asset`
  - 修改：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\NPCRoamProfile.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\NPCAutoRoamController.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NPCToolchainRegularizationTests.cs`
    - 三份角色 roam profile
    - 高速日志 / 任务账本 / `P1-01` 方案卡
- 本轮关键判断：
  - 我没有去碰：
    - `Primary.unity`
    - `DialogueChinese*`
    - 导航路径算法本体
  - 我只把 NPC 自己的“内容怎么装、怎么按角色分、怎么按搭档分”这一层先做实。
- 本轮验证：
  - `git diff --check` 已过
  - `Editor.log` 已看到脚本编译与程序集重载
  - `refresh_unity` 超时、`read_console` 为 `no_unity_session`
  - 因此本轮 live 验证仍视为外部受阻，而不是我 own 代码失败
- 当前恢复点：
  - `T-P1-01` 已 done
  - `T-P2-01 ~ T-P2-06` 的纯资产 / 代码层铺底已完成
  - 下一步先回看 `T-P1-02`；若热区仍未开，就继续转入 `T-P3-01`

## 2026-03-27｜Primary 仍 blocked 时，继续收掉 `T-P2-07` 与 `T-P3-01`

- 当前主线目标：
  - 用户要求高速推进，不要因为 `Primary.unity` 暂时不能写就停下；因此本轮先把当前最稳的两份非 live 文档切片收掉。
- 本轮子任务：
  - 再次只读复核 `T-P1-02`
  - 完成：
    - `T-P2-07`
    - `T-P3-01`
- 本轮完成：
  - 已确认：
    - `Primary.unity` 仍是大体量 mixed diff
    - `shared-root-branch-occupancy = neutral-main-ready` 不能当作 scene 直接写许可
    - `T-P1-02` 当前继续保持 `blocked-hotfile`
  - 已新增：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P2-07-角色化内容验收包.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC-P3-01-玩家与NPC双气泡视觉规范正式稿.md`
  - 已更新：
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC全盘详细落地任务列表.md`
    - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.1全面清盘\2026-03-27-NPC高速推进与测试排队日志.md`
- 本轮验证：
  - `git diff --check` 已通过本轮文档和账本改动
  - 当前没有新增 live 验收假证据
  - `unityMCP` session 仍不稳，因此这轮只 claim 静态 / 规范层完成
- 当前 owned / external 边界：
  - 当前 owned：
    - `0.0.1全面清盘` 下两份新文档
    - 任务账本
    - 高速日志
  - 当前 external / blocked：
    - `Primary.unity`
    - `PlayerThoughtBubblePresenter.cs`
    - `PlayerInteraction.cs`
    - `GameInputManager.cs`
- 当前恢复点：
  - 现在 `P2` 已经有正式验收包，`P3-01` 也有正式规范稿；
  - 下一步优先去评估 `T-P3-02` 有没有不吞 mixed hot-file 的安全代码切口；
  - 如果没有，就继续找下一项可独立推进的非 live 任务，不回去硬撞 `Primary.unity`。

## 2026-03-27｜`T-P3-02` 已先用单文件样式刀落地

- 当前主线目标：
  - 不等输入 hot-file 解锁，先把玩家气泡样式层独立做完，继续保持高速推进。
- 本轮子任务：
  - 只修改：
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
- 本轮完成：
  - 玩家气泡已从“直接照抄 NPC”改成正式玩家视觉：
    - 晨雾青绿色填充
    - 深青灰边框
    - 深墨色文字
    - 更轻一点的浮动和尾巴跳动
  - 同时保留与 NPC 的统一骨架：
    - `10` 字一行
    - 同级边距
    - 同级尾巴尺寸
    - 同级 show / hide 节奏
- 本轮验证：
  - `git diff --check` 已通过该文件
  - 当前没有 live 目测证据，因此这轮只 claim 样式层完成，不 claim 体验终验完成
- 当前 owned / external 边界：
  - 当前 owned：
    - `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`
  - 当前 same-root tail：
    - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs`
    - 白名单 sync 时被闸机点名为 remaining dirty
    - 复核后确认它属于此前玩家失败反馈 / `ToolUseCommitResult` 链的 own tail
    - 当前不再按外线 mixed 处理，而是准备一并收口
  - 当前 external / 暂不碰：
    - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
    - `Assets/000_Scenes/Primary.unity`
- 当前恢复点：
  - `T-P3-02` 可判 `done`
  - 下一步继续看：
    - `T-P3-03`
  - 但先要把 `Service/Player` 根下当前 own tail 收干净，再继续找不碰 `GameInputManager.cs` 的轻响应接线方案。
