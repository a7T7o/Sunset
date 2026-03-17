# 分析：main-branch-only 回归与 worktree 退役收口

## 1. 阶段定位
- `09` 解决的是“强制启动闸门、skills/AGENTS 先触发再做事”。
- `10` 解决的是“共享根目录分支漂移事故怎么止血、怎么找回可编译承载面”。
- `11` 解决的是最后一公里：
  - 不再接受事故期临时 `worktree` 变成新的常态工作方式。
  - 把 `farm / NPC / spring-day1` 当前被临时容器承载的成果，重新收回到“共享根目录 + 分支”的单工程常态。
  - 最终让用户不必再为了日常开发或验收打开多个 Unity 工程副本。

## 2. 用户红线
- `worktree` 只允许作为事故隔离、短期 cleanroom、紧急救援容器。
- `worktree` 不是 Sunset 的默认开发模型，更不是长期续航方案。
- 最终目标必须是：
  - 共享根目录：`D:\Unity\Unity_learning\Sunset`
  - 默认基线：`main`
  - 日常实现：在共享根目录里切 `codex/...` 分支
  - 验收：默认也在共享根目录完成，而不是跳到某个独立 worktree

## 3. 当前现场全景

### 3.1 共享根目录
- 当前仍是事故现场：`D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001 @ 11e0b7b4`
- 这里现在不是中性 `main`，也不是可以直接让任意线程继续写业务的正常现场。
- 它当前只适合作为：
  - 只读取证现场
  - 治理文档更新现场
  - branch-only 回归前的共享清理目标

### 3.2 已注册 worktree
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001 @ codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
  - 身份：farm 事故 cleanroom 成功承载面
  - 状态：`CLEAN`
- `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002 @ codex/farm-10.2.2-patch002 @ 11b81f98`
  - 身份：历史 farm patch 容器
  - 状态：`CLEAN`
- `D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier @ codex/main-reflow-carrier @ 26e765a6`
  - 身份：历史恢复承载链
  - 状态：`CLEAN`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC @ codex/npc-generator-pipeline @ 7b3bdd6c`
  - 身份：旧 NPC 历史 worktree
  - 状态：`CLEAN`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue @ codex/npc-roam-phase2-002 @ 6e2af71b`
  - 身份：NPC 事故后 continuation 临时容器
  - 状态：`CLEAN`
- `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue @ codex/npc-roam-phase2-001 @ 28aef95d`
  - 身份：NPC 救援/取证/收口现场
  - 状态：`DIRTY`
  - 剩余 dirty：4 个 TMP 资源，归属 `spring-day1`
- `D:\Unity\Unity_learning\Sunset_worktrees\spring-day1-story-progression-001 @ codex/spring-day1-story-progression-001 @ a9c952b7`
  - 身份：spring-day1 剧情推进 checkpoint 容器
  - 状态：`CLEAN`

## 4. 当前关键判断

### 4.1 现在不是“继续开新 worktree”的阶段
- 当前最大风险已经不是“没有隔离容器”，而是“临时容器正在反过来塑造默认工作流”。
- 如果此时默认接受 `continue in worktree`：
  - 用户会被迫记住多个物理工程目录
  - 线程会把“临时事故容器”误当“长期合法现场”
  - 验收和开发现场再次分离

### 4.2 正确身份是“分支是 carrier，worktree 只是容器”
- 合法长期对象应该是分支，不是某个 worktree 目录。
- 例如：
  - `codex/farm-1.0.2-cleanroom001` 可以保留为 Git 分支 carrier
  - 但 `farm-1.0.2-cleanroom001` 这个物理 worktree 最终应退役
- 同理：
  - `codex/npc-roam-phase2-002`
  - `codex/spring-day1-story-progression-001`
  这些都只能作为分支继续存在，不能把各自 worktree 目录当成常驻现场

### 4.3 三大业务线的真实状态
- `farm`
  - 好消息：`codex/farm-1.0.2-cleanroom001 @ 66c19fa1` 已编译通过，已具备合法承载面。
  - 未完成：还没有完成“从 cleanroom 承载面迁回 branch-only 常态”的方案。
- `NPC`
  - 好消息：`codex/npc-roam-phase2-002 @ 6e2af71b` 是 clean continuation branch。
  - 未完成：它还挂在 `NPC_roam_phase2_continue` 这个临时容器上；如果现在停工，验收就仍需打开另一个 Unity 工程。
- `spring-day1`
  - 好消息：`codex/spring-day1-story-progression-001 @ a9c952b7` 是 clean checkpoint。
  - 未完成：4 个 TMP 资源尾巴的最终归位、共享根目录复位后的 branch-only 续航入口，都还没裁定完。

## 5. 与导航检查 / 遮挡检查的对比判断
- `导航检查`、`遮挡检查` 当前都更接近“审计/重构准备线程”，不是典型事故救援线程。
- 它们没有形成新的活跃 worktree 依赖。
- 这反而说明：
  - 审计、方案、重构准备类线程，更适合 branch-only
  - 真正把事情推复杂的，是事故期遗留下来的临时容器没有被及时退役
- 当前结论：
  - `导航检查` 不需要新 worktree
  - `遮挡检查` 不需要新 worktree
  - 如果后续进入整改，先试“共享根目录 + 分支”，只有在共享热文件冲突不可接受时才讨论例外隔离

## 6. 11 阶段真正要做的事

### 6.1 先收口三条业务线 owner 结论
- `spring-day1`
  - 明确 4 个 TMP 资源尾巴怎么处理
  - 明确后续 branch-only 入口
  - 禁止再给出新长期 worktree 方案
- `NPC`
  - 确认 `codex/npc-roam-phase2-002` 是唯一 continuation branch carrier
  - 给出从 `NPC_roam_phase2_continue` 回归 branch-only 的迁回方案
  - 把 `NPC_roam_phase2_rescue` 降级为取证/待退役现场
- `farm`
  - 确认 `codex/farm-1.0.2-cleanroom001` 是唯一后续 branch carrier
  - 给出从 `farm-1.0.2-cleanroom001` 回归 branch-only 的迁回方案
  - 把 `codex/farm-1.0.2-correct001` 固定为事故取证分支，不再续写

### 6.2 再做 worktree 退役分组
- 第一批待退役：
  - `main-reflow-carrier`
  - `NPC`
  - `farm-10.2.2-patch002`
- 第二批待退役：
  - `NPC_roam_phase2_rescue`
  - `NPC_roam_phase2_continue`
  - `farm-1.0.2-cleanroom001`
  - `spring-day1-story-progression-001`

### 6.3 最后做共享根目录回 main
- 前提不是“现在就切 main”。
- 前提是：
  - 三条业务线的分支 carrier 都已锁定
  - worktree 退役顺序已明确
  - 共享根目录自己的 dirty / deleted / untracked 归属表已补齐
  - 根目录恢复为中性现场后，首份 branch-only 使用说明已落盘

## 7. 当前一句话结论
- `11` 阶段不是继续制造隔离容器，而是把已经存在的事故期 worktree 全部降级成“可退役的临时容器”，最终只留下“共享根目录 `main` + 业务分支”这一套低复杂度、单工程、可长期工作的常态。
