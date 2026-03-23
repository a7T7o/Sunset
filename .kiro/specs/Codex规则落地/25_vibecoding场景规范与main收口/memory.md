# 25_vibecoding场景规范与main收口 - memory

## 2026-03-22｜阶段建立：把当前治理主线从 scene-build 归位切回 vibecoding + main 收口
**当前主线目标**
- 在 `scene-build` Codex 归位已经完成后，把治理线程真正该做的主线重新钉死为：
  - `vibecoding` 场景规范适配
  - `main-only` 并发开发的统一收口机制

**本轮完成**
1. 新建当前正式阶段工作区：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口`
2. 已明确分层：
   - `2026.03.21_main-only极简并发开发_01` 继续作为执行批次壳
   - 本阶段作为治理正文主线
3. 已明确当前不再继续的错误路线：
   - `scene-build` 新路径迁移
   - queue / grant / ensure-branch 正文化扩建
   - `TD_14` 自动 hook 立即开发
4. 已把本阶段第一批任务收为：
   - `vibecoding` 场景规范适配 brief
   - dirty 归属说明
   - 统一回执窗口
   - main 收口批次表

**关键决策**
- 当前不是再造一套新规范帝国，而是把“并发线程怎么进 main”这件事变成一个你能直接使用的简单机制。
- `TD_14` 当前先降级为后手，因为全局报告显示更急的是 `sunset-startup-guard` 一级告警和 trigger-log 格式分叉。

**恢复点 / 下一步**
- 先落统一收口机制和给全局 skills 的窄范围委托。
- 然后直接进入 `vibecoding` 场景规范适配 brief。

## 2026-03-22｜第一版 vibecoding 场景规范适配 brief 已落盘
**当前主线目标**
- 不让 `25` 阶段停在“只有立项没有内容”，而是直接把 `SceneBuild_01` 当前该遵守的 `vibecoding` 场景规范压成第一版短 brief。

**本轮完成**
1. 已读取并吸收：
   - `spring-day1 -> scene-build` handoff
   - `spring-day1-implementation/requirements.md`
   - `scene-build` 当前工作区记忆里对 `SceneBuild_01` 的定位
2. 已新增第一版 brief：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\vibecoding场景规范适配Brief_2026-03-22.md`
3. 已把核心口径收成：
   - 住处安置优先
   - 东侧入口稳定
   - 院落对话留白
   - 工作台闪回焦点
   - 教学区贴着生活区但不吃掉主院
   - 禁止扩成整村大图

**关键决策**
- `vibecoding` 在 Sunset 当前场景语境里，不是乱搭气氛，而是“情绪和生活感先行，但必须服务剧情动作与动线”。

**恢复点 / 下一步**
- 下一步应继续补当前 shared root dirty 归属说明。
- 然后发起第一轮统一回执窗口。

## 2026-03-22｜shared root dirty 归属说明已落盘，后续回执不再靠脑内区分
**当前主线目标**
- 把 `main-only` 并发现场里最容易引发误判的一层先收住：当前 shared root 到底哪些 dirty 属于治理线，哪些属于 NPC、`spring-day1`、农田或旧剧情整理。

**本轮完成**
1. 已新增正式口径文件：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\当前shared_root_dirty归属说明_2026-03-22.md`
2. 已把现场 dirty 拆成 5 组：
   - 当前治理线自己认领
   - 治理线旧账但不再继续扩写
   - NPC 线程认领
   - `spring-day1` / 剧情整理线认领
   - 农田 / 库存交互线认领
3. 已把旧误区写死：
   - `scene-build` 新路径迁移作废
   - 旧 queue / grant / ensure-branch 正文化扩建作废
   - mixed dirty 不揉成一个超大 commit

**关键决策**
- 以后任何线程问“现在 dirty 怎么办”，先用这份归属说明过滤噪音，再进入统一回执窗口。
- 当前 shared root 脏，不等于所有线程都不能继续；真正要看的，是线程是否只认自己的 `changed_paths`，以及是否撞到高危目标。

**恢复点 / 下一步**
- 下一步应发起第一轮“统一回执窗口”。
- 再基于回执结果做第一版 main 收口批次表。

## 2026-03-22｜第一轮统一回执已收件，第一版 main 收口批次表已形成
**当前主线目标**
- 把 `main-only` 并发开发从“只有机制文档”推进到“已经真实收过一轮回执，并形成了第一版批次裁定”。

**本轮完成**
1. 已基于固定回收卡：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\000全员回执.md`
   收到以下线程的正式回执：
   - 农田
   - `spring-day1`
   - NPC
   - 导航
   - 遮挡检查
   - 全局 skills
2. 已新增正式裁定文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\第一轮main收口批次表_2026-03-22.md`
3. 已把本轮批次结论收成：
   - A组：农田
   - B组：`spring-day1`、遮挡检查
   - C组：NPC、导航
   - 支援：全局 skills
4. 已把治理侧初裁回写到回收卡底部，避免 `000全员回执.md` 只剩原始回复堆叠。

**关键决策**
- 第一轮真实收件之后，当前最适合先入 `main` 的只有农田。
- `spring-day1` 与遮挡检查是“branch carrier 已有 checkpoint，但需要后续定向迁入窗口”的下一批对象。
- NPC 与导航当前都不值得为了文档/日志或 docs-first 基线单独制造一刀 `main` 提交。

**恢复点 / 下一步**
- 下一步优先给农田发第一批 `main` 收口执行 prompt。
- 然后再决定 B组 先迁 `spring-day1` 还是先迁遮挡检查。
- 同时把全局 skills 对 `sunset-startup-guard` 的裁定吸收到本地正文。

## 2026-03-22｜A组首个执行 prompt 已备好，直接可发农田
**当前主线目标**
- 不让第一轮批次表停在“只有裁定没有动作”，而是立刻给 A组 产出首个可执行的 `main` 收口 prompt。

**本轮完成**
1. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\农田_第一批main收口执行.md`
2. 已把农田本轮动作压成：
   - 只核对白名单
   - 不再扩写
   - 只收农田自己的 `main` checkpoint
   - 回执只给提交路径、SHA、`git status`、`blocker_or_checkpoint`

**关键决策**
- 当前第一批不再继续空谈“谁该先提”，而是已经明确到“农田现在可以领哪份 prompt 去执行”。

**恢复点 / 下一步**
- 现在可以直接把这份 prompt 发给农田。
- 农田回执后，再决定 B组 谁先进入下一批。

## 2026-03-22｜“每线程只提自己白名单”已上升为当前默认收口原则
**当前主线目标**
- 回答用户对并发最终解法的追问：是不是每个线程以后都可以只提交自己改的内容，并把这个结论从聊天判断升级成机制正文。

**本轮完成**
1. 已更新：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\并发线程统一回执与main收口机制_2026-03-22.md`
2. 已把当前主流程写死为：
   - 一线程只提交自己修改的内容
   - 一线程只提交自己的白名单路径
   - 一线程一次只收一刀自己的 checkpoint
3. 已补清一个关键口径：
   - Git SHA 不能自定义
   - 可标准化的是 checkpoint 名称 / commit message
   - 推荐格式：`YYYY.MM.DD_<线程名>_<编号>`

**关键决策**
- 这套“每线程白名单提交”的思路不是临时技巧，而是当前 `main-only` 并发模型下最接近最终方案的主流程。
- 真正的例外只剩少数几类：高危撞车、跨线程耦合、以及 branch carrier / worktree 迁入 `main` 的窗口。

**恢复点 / 下一步**
- 后续线程正常完成一刀后，默认都按这个口径收口。
- 只有命中少数例外时，治理线程才额外介入。

## 2026-03-22｜B组已定先收 spring-day1，并已生成迁入 main 执行 prompt
**当前主线目标**
- 不让 B组 继续停在“先迁谁”的口头选择上，而是直接把更高优先级的 `spring-day1` 定下来并给出可执行 prompt。

**本轮完成**
1. 已裁定：
   - B组 优先顺序先 `spring-day1`
   - 再看遮挡检查
2. 已新增：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\spring-day1_B组main迁入执行.md`
3. 已把本轮动作压成：
   - 停止继续扩写 `0.0.2`
   - 只把 `4ff31663` 这刀基础脊柱代码最小面迁入 `main`
   - 如需补记，只限 `spring-day1` 自己的两层 memory

**关键决策**
- 当前 `spring-day1` 比遮挡检查更应先迁入 `main`，因为它承接 Day1 主骨架代码，而不是 editor + docs checkpoint。

**恢复点 / 下一步**
- 现在可以直接把这份 prompt 发给 `spring-day1`。
- 等它回执后，再给遮挡检查发下一份 B组 prompt。

## 2026-03-22｜白名单提交规则已从正文同步到执行层入口
**当前主线目标**
- 回应用户“只写正文不够，应该直接落到执行层”的提醒，把默认提交规则真正压进批次壳入口与通用分发模板。

**本轮完成**
1. 已更新执行层入口：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\README.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\治理线程_职责与更新规则.md`
2. 已新增通用模板：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\线程完成后_白名单main收口模板.md`
3. 已把默认口径同步到执行层：
   - 线程默认只提交自己修改的内容
   - 默认只提交自己的白名单路径
   - 默认一线程一次只收一刀 checkpoint
   - checkpoint 名称 / commit message 统一为 `YYYY.MM.DD_<线程名>_<编号>`

**关键决策**
- 现在这条规则已经不是“只在正文里成立”，而是进入了执行层入口和分发模板。
- 后续并发分发时可以直接复用模板，不必每次重新口头解释。

**恢复点 / 下一步**
- 当前继续等 `spring-day1` 回执。
- 它回执后再接遮挡检查，或继续并行吸收全局 skills 的窄裁定。

## 2026-03-22｜硬入口已纠偏，`spring-day1` 新回执和 startup-guard 窄裁定已并入本阶段
**当前主线目标**
- 不再让当前阶段停留在“正文和执行壳已经改过”，而是把真正会误导线程的活入口、批次状态和告警裁定一起收成同一条现行口径。

**本轮完成**
1. 已纠偏当前会被线程真实读取的硬入口：
   - `D:\Unity\Unity_learning\Sunset\AGENTS.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前唯一状态说明_2026-03-17.md`
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset Git系统现行规则与场景示例_2026-03-16.md`
2. 已把当前硬入口统一成：
   - 普通线程默认 `main-only + whitelist-sync + exception-escalation`
   - `branch / worktree / request-branch / ensure-branch` 只再属于例外窗口
3. 已把 `spring-day1` 新回执并入当前阶段口径：
   - 基础脊柱代码已进入 `main @ 83d809a9`
   - 当前 shared root 里这条线剩余的主要是 handoff / requirements / memory / 旧剧情整理
4. 已把农田和 `spring-day1` 的执行结果回写到：
   - `000全员回执.md`
   - `第一轮main收口批次表_2026-03-22.md`
   - `当前shared_root_dirty归属说明_2026-03-22.md`
5. 已吸收全局 skills 的窄裁定：
   - `sunset-startup-guard` 继续保留为 Sunset 硬前置
   - 但退出当前 `manual-equivalent` 一级告警统计
   - 本阶段开始，Sunset 治理线程只再往 `skill-trigger-log.md` 追加 `STL-v2`
6. 已做一轮真实脚本验证：
   - 在 `D:\Unity\Unity_learning\Sunset @ main @ 83d809a9`
   - 以 `task + main + IncludePaths` 对治理线白名单做 `preflight`
   - 结果为 `True`
   - 关键理由是：`shared root 当前位于 main-only 白名单 sync；允许保留 unrelated dirty，不再因 remaining dirty 一刀切阻断`

**关键决策**
- 现在这套机制不再只是文档假设，而是已经被当前 working tree 实测通过。
- 当前最值得继续推进的，不是再解释“能不能在 main 提交”，而是直接把治理线自己这批白名单文件同步掉，给后续线程减少 shared root 噪音。

**恢复点 / 下一步**
- 现在直接进入治理线自己的白名单 `sync`。
- 这刀收完后，shared root 里应只剩业务线程和剧情整理线自己的 dirty。

## 2026-03-22｜已补两份“规则已更新，可直接 self-sync 到 main”的直接转发 prompt
**当前主线目标**
- 把“规则已更新、线程现在可以自己白名单提交”这件事，直接变成可转发给线程的最短 prompt，而不是继续只停留在治理说明里。

**本轮完成**
1. 已新增 NPC 直达 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\NPC_继续修复并直接main收口.md`
2. 已新增 `spring-day1` 文档尾巴收口 prompt：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\spring-day1_剩余文档整理并直接main收口.md`
3. 两份 prompt 都已明确写入：
   - 当前规则已更新
   - 当前可以自己白名单提交到 `main`
   - 不再沿用“shared root 有 unrelated dirty 所以默认不能提交”的旧口径

**关键决策**
- 这一步现在可以直接发给线程，不需要再额外配一段人工解释。
- `NPC` 走“继续修复 + 完成后 self-sync 到 main”。
- `spring-day1` 走“不要再碰已进 `main` 的 Story 代码，只收剩余文档整理面”。

**恢复点 / 下一步**
- 现在可以直接把这两份 prompt 发出去。
- 下一步等它们回新的 checkpoint / blocker 回执。

## 2026-03-22｜固定回执箱已接入当前批次 prompt
**当前主线目标**
- 不再让项目经理自己手动想“这轮回执要放到哪里”，而是由治理线提前准备固定回执落点，并直接写进 prompt。

**本轮完成**
1. 已把当前批次固定回执箱明确为：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\001部分回执.md`
2. 已更新：
   - `NPC_继续修复并直接main收口.md`
   - `spring-day1_剩余文档整理并直接main收口.md`
   - `线程完成后_白名单main收口模板.md`
3. 三份 prompt 都已明确要求：
   - 不要让项目经理手动建文件
   - 直接把回执追加到固定回执箱

**关键决策**
- 从这一轮开始，当前批次的回执机制改成：
  - 我准备回执箱
  - prompt 直接写回执落点
  - 线程自己往固定文件回

**恢复点 / 下一步**
- 现在你直接转发 prompt 即可，不需要再额外给线程解释回执文件该怎么建。

## 2026-03-22｜新版开工前缀已落盘，线程问题从“能不能开工”转成“按哪种口径开工”
**当前主线目标**
- 回答用户最新的现场问题：现在是不是因为 dirty 基本清掉、规范已经落下来，所以线程可以直接开工；同时把这件事压成可直接转发的 prompt 前缀，而不是继续靠聊天口头解释。

**本轮完成**
1. 已新增普通线程通用前缀：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\并发线程_当前版本更新前缀.md`
2. 已新增 `scene-build` 专用前缀：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\scene-build_当前版本更新前缀.md`
3. 已把当前线程分组重新钉死为：
   - 可直接发普通前缀继续干活：`NPC`、农田、导航检查、遮挡检查
   - 可直接开工但必须走 worktree 专用前缀：`scene-build`
   - 当前职责已完成、先不唤醒：`spring-day1`、全局 skills
4. 已明确制度测试结论：
   - `NPC` 和 `spring-day1` 已经走过一轮“规则更新后 self-sync 到 main”的真实验证
   - 其他普通 `main-only` 线程现在不需要再重复做这轮制度测试
   - `scene-build` 也不是“再补一轮测试”，而是按独立 worktree 口径直接推进并在 worktree 内提交 checkpoint

**关键决策**
- 当前 shared root 是否完全 clean，不再是所有线程统一的开工前置。
- 现在真正要做的是：给线程发对前缀，让它们先完整回忆已做进度、剩余任务和下一刀最大推进范围，然后直接干到底。
- `scene-build` 当前最需要的不是迁目录解释，而是认清“你就在既有 worktree 内继续施工，并在 worktree 分支里自己收 checkpoint”。

**恢复点 / 下一步**
- 你现在可以直接拿这两份新前缀去对线程说话。
- 治理线自己的下一步，是继续把剩余还没明确唤醒状态的线程做一次有效任务梳理，而不是再扩写一层规则壳。

## 2026-03-22｜第二波直接开发唤醒顺序与 4 条线程专属 prompt 已落盘
**当前主线目标**
- 把“有了通用前缀”继续推进到“可以直接发下一波线程”的状态，不再让项目经理自己从聊天里拼装谁先醒、谁拿哪份 prompt。

**本轮完成**
1. 已新增第二波直接唤醒顺序：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\下一波直接唤醒顺序_2026-03-22.md`
2. 已新增 4 条线程专属 prompt：
   - `scene-build_当前版本更新并继续施工.md`
   - `导航检查_当前版本更新并继续直接开发.md`
   - `NPC_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`
3. 已把当前优先级明确为：
   - 第一优先：`scene-build`
   - 第二优先：`导航检查`
   - 第三优先：`NPC`
   - 第四优先：`遮挡检查`
4. 已把当前不再唤醒的线程明确钉死为：
   - `spring-day1`
   - 全局 skills

**关键决策**
- 当前不是“再收一轮统一回执”，而是直接恢复真实开发。
- `scene-build` 继续按独立 worktree 推进。
- 导航和遮挡都不再允许停在 docs-first / branch waiting 的旧口径上。
- `NPC` 下一轮只继续收 NPC 自己的表现层和体感，不越界做导航核心。

**恢复点 / 下一步**
- 你现在可以直接按第二波顺序去唤醒线程。
- 治理线下一步只需要继续吃回执、处理少数高危冲突，不再额外制造分发摩擦。

## 2026-03-22｜导航/遮挡 prompt 已做硬纠偏，`spring-day1` 重新回到可开工队列
**当前主线目标**
- 响应用户对 `002全员部分聊天记录` 的纠错：导航和遮挡上一轮仍把“历史成果还在分支上”当 blocker，说明我的 prompt 纠偏不够硬；同时 `spring-day1` 的真实状态已恢复到可继续推进，不该继续留在暂停队列。

**本轮完成**
1. 已回读：
   - `002全员部分聊天记录\导航检查.md`
   - `002全员部分聊天记录\遮挡检查.md`
   - `002全员部分聊天记录\spring-day1.md`
2. 已确认两条旧误判：
   - 导航线程仍把 `codex/navigation-audit-001` 当 blocker
   - 遮挡线程仍把 `codex/occlusion-audit-001 @ 295e8138` 当 blocker
3. 已对两份 prompt 做硬纠偏：
   - `导航检查_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`
   核心新增要求是：
   - 不能再把旧分支遗留当默认 blocker
   - 这轮必须自己处理旧分支遗留
   - 并且要在同一轮里完成“遗留处理 + main 上首个真实 checkpoint”
4. 已新增：
   - `spring-day1_当前版本更新并继续直接开发.md`
5. 已更新第二波顺序文件：
   - `spring-day1` 重新加入可开工队列
   - 全局 skills 仍继续停

**关键决策**
- 导航和遮挡当前真正需要的不是再解释制度，而是被强制从“分支停滞态”推回 `main-only` 主流程。
- `spring-day1` 当前不是“已完成所以停”，而是“文档和基础脊柱都已收完，应该直接回到 Day1 首段剧情真实闭环”。

**恢复点 / 下一步**
- 你现在不要分发我上一版导航/遮挡 prompt。
- 你可以直接发新版：
  - 导航检查
  - 遮挡检查
  - `spring-day1`
- `scene-build` 和 `NPC` 你自己直接开工即可。

## 2026-03-23｜并发正确率问题已定性：现在缺的不是更多 MCP，而是代码改完后的强制自检层
**当前主线目标**
- 回应用户对 3 段线程聊天记录的追问，判断当前多并发现场要不要上强制检查、hook，还是继续靠人工贴报错驱动修复。

**本轮完成**
1. 已综合吸收 3 类新信号：
   - `spring-day1`：obsolete warning 和 unused variable 都是在用户贴出后才补扫，说明“本地静态清理”没有被前置成默认动作。
   - 遮挡检查：`OcclusionTransparency.cs` 被错误编码重写，说明“改文件后最小 diff/编码健康检查”没有被强制执行。
   - 遮挡检查测试：`using` 缺失、`Object` 歧义、反射类型缺失等纯 C# 名称错误，说明“改完代码先做最小编译级自检”这一层也没有被前置。
2. 已把当前问题定性为：
   - 不是“只能靠 MCP 才能发现问题”
   - 也不是“需要一上来就做重型全局 hook”
   - 而是当前并发模式缺一层轻量但强制的代码后检查

**关键决策**
- 当前最该补的是两层小闸门，而不是继续把 Unity/MCP 当唯一验收入口：
  1. `post-edit` 自检层：
     - 改完 `.cs` 后先做目标文件静态自查、`git diff --check`、必要的编码/命名空间检查
  2. `pre-sync` 最小验证层：
     - 准备收 checkpoint 前，再做最小编译/测试/Console 级验证
- MCP / Unity 编译仍然重要，但它们应该是第二层，不该替代第一层低级错误清理。

**恢复点 / 下一步**
- 我下一步会给出一个“适合 Sunset 当前 main-only 并发”的最小正确率方案：
  - 不重
  - 不拖慢节奏
  - 但足够防住这次暴露出来的三类低级错误。

## 2026-03-23｜代码闸门已正式落地到 `git-safe-sync.ps1`，并完成直测与集成测试
**当前主线目标**
- 不再只停留在“并发现场缺 post-edit / pre-sync 两层自检”的诊断，而是直接把这套正确率闸门接进 Sunset 当前真实会走的收口主流程里。

**本轮完成**
1. 已新增独立检查器：
   - `D:\Unity\Unity_learning\Sunset\scripts\CodexCodeGuard\CodexCodeGuard.csproj`
   - `D:\Unity\Unity_learning\Sunset\scripts\CodexCodeGuard\Program.cs`
2. 已把代码闸门正式接入：
   - `D:\Unity\Unity_learning\Sunset\scripts\git-safe-sync.ps1`
3. 当前 `task` 模式只要白名单里含 `.cs`，收口前会自动做：
   - 目标文件 UTF-8 检查
   - `git diff --check`
   - 基于 Unity 程序集引用的程序集级编译检查
4. 该编译检查不是弱正则，而是：
   - 读取 Unity Editor 安装路径
   - 读取 `Library/ScriptAssemblies`
   - 按 `Assembly-CSharp / Assembly-CSharp-Editor / Tests.Editor` 分组编译
   - 用当前线程改动 + `HEAD` 基线做差分，只报这轮新引入的诊断
5. 已完成两轮真实测试：
   - 直测：对当前遮挡线 5 个 `.cs` 改动直接调用 `CodexCodeGuard.dll`，结果通过
   - 集成测：对当前工作树 dirty 做一次 `git-safe-sync.ps1 -Action preflight -Mode task`，代码闸门正常出现在输出中并通过
6. 已把线程侧口径同步到：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `2026.03.21_main-only极简并发开发_01/README.md`
   - `并发线程_当前版本更新前缀.md`
   - `线程完成后_白名单main收口模板.md`
   - 导航 / 遮挡 / `spring-day1` 当前开发 prompt

**关键决策**
- 现在的主流程已经不再允许“改完 `.cs`，先不查，等用户贴报错再补”。
- 也不再把 Unity/MCP 当成第一层低级错误筛查工具；第一层筛查已经进入 `git-safe-sync.ps1`。
- 当前代码闸门以“只报这轮新引入的诊断”为核心，这样才能在 shared root 并发 dirty 现场里真正可用。

**恢复点 / 下一步**
- 从现在开始，线程只要改 `.cs` 并准备收口，就会被脚本自动卡一次。
- 下一步不再是继续发明规则，而是观察线程真实使用一轮，看看是否还需要补“targeted test gate”或“warning 白名单”之类的细化策略。

## 2026-03-23｜全局 skills 旧历史问题已闭环，当前剩余项按 ownership 摊开
**当前主线目标**
- 回答用户两个问题：
  1. 全局 skills 之前那条历史回复现在还要不要继续回
  2. 治理线程自己到底还剩哪些未完成项

**本轮完成**
1. 已对账全局 skills 的旧回复与当前 live 事实，确认它当时说“还没代执行落盘”的那些动作，现在已经被治理线程实做完成：
   - `sunset-startup-guard` 保留硬前置、退出 `manual-equivalent` 一级告警
   - Sunset 治理线改写为只写 `STL-v2`
   - 相关裁定已写回当前阶段正文与 memory
2. 已得出当前裁定：
   - 这条全局 skills 历史问题现在不需要再回它，也不需要再批准它来补做同一批 Sunset 本地落地
   - 后续只有两类事再需要找它：
     - 旧 trigger log 归一化
     - 把新一轮稳定经验上升成 global learning
3. 已把当前剩余项按 ownership 分成 4 类：
   - 我这条治理线真未完成
   - 已明确暂缓 / 待裁定
   - 现场存在但属于别的线程
   - 已作废，不该再继续

**关键决策**
- 全局 skills 这条旧历史，在 Sunset 本地已经不是 blocker，也不是待批准事项。
- 现在最重要的是别再把“他过去说还能做”误认成“我这边还没做”。

**当前我这条治理线真未完成**
1. `TD_14` 是否继续做，仍缺一次明确裁定。
2. 代码闸门虽然已落地，但还没经历完整的一轮多线程真实使用回执，因此：
   - `warning` 是否一律阻断
   - 是否需要 targeted test gate
   - 是否需要更细的白名单/豁免
   这些还没定稿。
3. 当前批次目录里的回执与聊天记录原始材料还没整理归档：
   - `001部分回执.md`
   - `002全员部分聊天记录/*`

**已明确暂缓 / 待裁定**
1. `TD_14` 自动 hook 正式实现
2. 旧 trigger log 归一化
3. 新 global learning 上升

**现场存在但不属于我这条治理线的未收口**
1. `spring-day1` 继续推进链
2. 导航检查历史分支遗留 + 首个真实代码 checkpoint
3. 遮挡检查历史分支遗留 + 首个真实整改 checkpoint
4. NPC / scene-build 的当前实现现场

**已作废，不该再继续**
1. `scene-build` 新路径迁移
2. 旧 queue / grant / ensure-branch 正文化扩建
3. 继续让线程用“历史成果还在分支上”当默认 blocker
4. 继续重复整理 `spring-day1` 文档当主线

**恢复点 / 下一步**
- 后续对外口径应改成：
  - 全局 skills 旧历史：不用回，不用批；已吸收完
  - 当前治理线真未完：只剩 `TD_14` 裁定、代码闸门观察期、批次材料归档

## 2026-03-23｜`TD_14` 已裁定，批次材料已归档，当前治理线结构性尾项已收完
**当前主线目标**
- 把上一轮盘出来的 3 个剩余项真正做完，不再让这条治理线继续拖着“还有一点没收”的尾巴。

**本轮完成**
1. 已对 `TD_14` 做正式裁定：
   - 更新 [TD_14_自动化skill触发日志Hook待办.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/000_代办/codex/TD_14_自动化skill触发日志Hook待办.md)
   - 结论是：转入冻结后备项，不升级为当前正文阶段，不再算当前剩余项
2. 已新增批次归档文件：
   - [批次材料归档与关账摘要_2026-03-23.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/共享根执行模型与吞吐重构/01_执行批次/2026.03.21_main-only极简并发开发_01/批次材料归档与关账摘要_2026-03-23.md)
3. 已把批次目录里的材料正式分层：
   - `000全员回执.md`：第一轮总回执历史证据
   - `001部分回执.md`：持续回执箱
   - `002全员部分聊天记录/*`：原始聊天证据层
   - `README.md`：当前执行入口
4. 已更新当前阶段任务板，确认：
   - `TD_14` 裁定完成
   - 批次材料归档完成
   - 当前主线一句话口径也已正式完成

**关键决策**
- 到这一步，治理线不再有“还差一个结构性收尾动作”的尾巴。
- 后续这条线只剩两类正常工作：
  1. 跟进真实线程回执
  2. 根据真实使用反馈细化代码闸门

**恢复点 / 下一步**
- 当前治理线已经从“建设阶段”进入“运行维护阶段”。
- 如果下一步还要我继续，就不再是收尾，而是基于新回执做具体裁定或继续吃线程问题。

## 2026-03-23｜“历史尾账已清”只指治理线，不等于业务线程现场已全清
**当前主线目标**
- 回答用户对“历史尾账是不是都做完了”的追问，避免把治理尾账和业务线程正在跑的现场脏改混在一起。

**关键结论**
- 我说“历史尾账已清”，指的是：
  - 治理线自己的旧裁定、旧入口、旧批次材料、`TD_14`、代码闸门落地、遮挡验证副本裁定，这些历史账务类事项已经收完。
- 不指：
  - 业务线程当前正在进行中的实现
  - 业务线程自己的 memory / checkpoint / live 验证
  - 当前 shared root 上他线仍在推进的 dirty

**恢复点 / 下一步**
- 后续如果继续推进，默认视角应切回“运行态新问题”，而不是再回头翻治理老账。

## 2026-03-23｜遮挡线程验证副本已做“有条件批准”裁定并落成专用 prompt
**当前主线目标**
- 把用户刚刚点名的遮挡线程验证副本问题正式落地成可执行裁定，而不是停在聊天判断里。

**本轮完成**
1. 已定性：
   - 这次不是 Git / 主仓库破坏性事故
   - 是“未经先报备，擅自扩张验证策略”的流程越界
2. 已作出正式裁定：
   - 有条件批准保留 `D:\Unity\Unity_learning\Sunset_occlusion_validation`
   - 但只允许它作为一次性测试沙盒，跑完本轮 EditMode 测试后必须删除
3. 已新增专用 prompt：
   - `可分发Prompt\遮挡检查_验证副本有条件批准并测试后删除.md`

**关键决策**
- 这次不按事故推倒重来，也不允许把副本发展成第二现场。
- 当前正确处理方式是：让它把测试跑完、收结果、删副本、回执关账。

**恢复点 / 下一步**
- 你现在如果要回复遮挡线程，直接发这份新 prompt 即可。

## 2026-03-23｜快速出警 skill 与单实例遮挡验证批准 prompt 已一起落地
**当前主线目标**
- 把“快速定位责任线程”的想法真正做成可复用通道，同时处理用户刚刚要求的遮挡线程单实例验证审批。

**本轮完成**
1. 已新增本地 skill：
   - `C:\Users\aTo\.codex\skills\sunset-rapid-incident-triage`
2. skill 已包含：
   - `SKILL.md`
   - `agents/openai.yaml`
   - `references/owners-and-hotspots.md`
   - `scripts/rapid_incident_probe.py`
3. 已完成一次真实验证：
   - 用“背包异常”案例直测
   - 正确命中 `农田交互修复V2`
4. 已新增项目侧 prompt：
   - `可分发Prompt\快速出警_责任线程速查.md`
5. 已新增遮挡线程审批 prompt：
   - `可分发Prompt\遮挡检查_主工程单实例验证窗口批准.md`

**关键决策**
- 快速出警通道现在已经不是聊天技巧，而是：
  - 一个 skill
  - 一个项目 prompt
  - 一套 owner/hotspot 快速映射
- 遮挡线程这轮我倾向于批准，但批准的前提是给它一个短时、单实例、不可扩张的验证窗口。

**恢复点 / 下一步**
- 后续你一旦看见异常、回归、找不到责任线程，直接用“快速出警” prompt 或让我显式走这个 skill。
- 遮挡线程这轮如要继续，直接发新批准 prompt。

## 2026-03-23｜遮挡线程单实例验证窗口可以批准，但必须是短时清净窗口
**当前主线目标**
- 回应用户对遮挡线程最新回复的追问：它现在改用主工程单实例 + MCP 验证，这种方案能不能批，以及是否需要一个“清净时刻”。

**本轮完成**
1. 已核当前现场：
   - `shared-root-branch-occupancy.md` 仍是 `main + neutral`
   - `mcp-single-instance-occupancy.md` 当前 `current_claim = none`
   - 但遮挡相关主工程脏改真实存在：
     - `Primary.unity`
     - `OcclusionManager.cs`
     - `OcclusionTransparency.cs`
     - `TreeController.cs`
     - `OcclusionSystemTests.cs`
2. 已明确裁定：
   - 遮挡线程这轮可以批准继续
   - 但必须给它一个**短时、单实例、不可扩张**的验证窗口
3. 这里的“清净时刻”定义为：
   - 没有别的线程同时占用同一个 Unity/MCP live 写入口
   - 没有别的线程同时改 `Primary.unity` 或同一批遮挡相关脚本
   - 只允许它完成这一轮 `Edit Mode / Console / 测试 / 最小修正 / 重跑`

**关键决策**
- 这不是“全项目所有线程停工”的大清场。
- 这是“Unity/MCP 单实例验证窗口”意义上的小清净窗口。
- 只要当前有别的线程也要用同一个主工程 Unity 实例做 live 写或 live 验证，遮挡线程这轮就不该跟它并发撞车。

**恢复点 / 下一步**
- 当前对遮挡线程的正确动作不是再让它解释，而是直接发：
  - `可分发Prompt\遮挡检查_主工程单实例验证窗口批准.md`
## 2026-03-23｜现行规范需要补强，但不用推翻重来
**当前主线目标**
- 回答用户对当前并发制度的追问：这轮是不是已经证明现行规范还有漏洞；如果要继续加强，应该补哪里、怎么补，才能更贴近真实开发而不是回到规则帝国。

**本轮完成**
1. 已新增正式方案文档：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\规范补强方案_2026-03-23.md`
2. 已把这轮真实暴露出来的缺口压成 7 类具体补强项：
   - 规则入口四同步
   - 热文件卫生字段
   - `.unity/.prefab/.asset` 资源卫生闸门
   - branch / worktree 例外退场四步
   - Unity / MCP live 窗口四字段
   - 固定回执最小字段收紧
   - 一刀一收的时间纪律
3. 已明确当前判断：
   - 现行 `main-only + whitelist-sync + exception-escalation` 方向是对的
   - 问题主要在执行层补丁不够厚，不在主方向本身

**关键决策**
- 后续规范加强应优先补“线程真实会踩的坑”，而不是再扩一套新的总治理系统。
- 当前最值钱的补强，不是恢复 branch-first，而是把“热文件卫生、资源卫生、例外退场、live 占窗、回执字段”这些容易反复出事故的地方钉死。

**恢复点 / 下一步**
- 下一步如果继续推进，应把这 7 条补强项择要同步进当前规范快照与通用前缀 prompt。
- 在同步前，不要再额外发明新的治理概念层。

## 2026-03-23｜补强项已从总规范继续压到线程真实入口
**当前主线目标**
- 不让“补强方案只存在于治理正文里”，而是继续压到线程平时真的会打开和转发的 prompt 入口。

**本轮完成**
1. 已把补强项同步到：
   - `AGENTS.md`
   - `Sunset当前规范快照_2026-03-22.md`
   - `2026.03.21_main-only极简并发开发_01/README.md`
   - `并发线程_当前版本更新前缀.md`
   - `线程完成后_白名单main收口模板.md`
2. 已继续把统一追加口径压到当前活跃线程 prompt：
   - `NPC_当前开发放行.md`
   - `NPC_当前版本更新并继续直接开发.md`
   - `NPC_继续修复并直接main收口.md`
   - `scene-build_当前开发放行.md`
   - `scene-build_当前版本更新前缀.md`
   - `scene-build_当前版本更新并继续施工.md`
   - `spring-day1_当前开发放行.md`
   - `spring-day1_当前版本更新并继续直接开发.md`
   - `农田交互修复V2_当前开发放行.md`
   - `导航检查_当前开发放行.md`
   - `导航检查_当前版本更新并继续直接开发.md`
   - `遮挡检查_当前开发放行.md`
   - `遮挡检查_当前版本更新并继续直接开发.md`

**关键决策**
- 从这一步开始，用户后续即使直接拿线程 prompt 去发，也不应该再漏掉“热文件卫生 / 资源卫生 / live 占窗 / 一刀一收”这些补强项。

**恢复点 / 下一步**
- 下一步应对白名单继续收口这批 prompt 同步改动。

## 2026-03-23｜编码问题文章学习与本地检测已完成
**当前主线目标**
- 用户要求我完整阅读一篇关于 Codex / VS Code / PowerShell 中文乱码根因的文章，并判断 Sunset 本地是否也命中了同类问题，然后给出本地检测报告。

**已完成**
1. 已提炼文章的核心解决方法：
   - 问题根因不在 VS Code 单点，而在“编辑器 UTF-8”与“终端/PowerShell 编码口径”没有统一。
   - 真正要统一的是：
     - `files.encoding`
     - `chcp`
     - `[Console]::InputEncoding`
     - `[Console]::OutputEncoding`
     - `$OutputEncoding`
     - PowerShell Profile 里的默认文件写入编码
2. 已对 Sunset 本地环境完成取证：
   - 当前为 `Windows PowerShell 5.1.26100.7019`
   - `pwsh` 缺失
   - `chcp = 936`
   - `[Console]::InputEncoding = gb2312`
   - `[Console]::OutputEncoding = utf-8`
   - `$OutputEncoding = us-ascii`
   - `$PROFILE` 路径存在但文件缺失
   - VS Code 只见到 `"files.autoGuessEncoding": true`，未见 `"files.encoding": "utf8"`
3. 已区分“终端假乱码”和“文件真损坏”：
   - `Sunset当前规范快照_2026-03-22.md` 等样本属于 `healthy-terminal-noise`
   - `001部分回执.md` 在 UTF-8 解码下仍含替换字符，属于真实损坏样本
4. 已生成正式报告：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\本地编码检测报告_2026-03-23.md`

**关键决策**
- Sunset 确实命中了文章里说的同类问题，而且比“单纯终端显示错”更严重：
  - 环境层没有统一，仍在继续制造新乱码风险
  - 文件层已经有少量活文档出现真实损坏
- 正确治理顺序不是盲目批量转码，而是：
  1. 先统一环境层 UTF-8 口径
  2. 再定点修当前仍在使用的坏文档
  3. 最后按历史价值分级处理旧样本

**恢复点 / 下一步**
- 如果用户下一步要我继续直接修，这一阶段最自然的动作应是：
  - 先补 PowerShell Profile 与 VS Code `files.encoding`
  - 再定点处理 `001部分回执.md`

## 2026-03-23｜编码环境已经直接落地，不再停在报告层
**当前主线目标**
- 用户要求不要停在检测结论，直接把本机 PowerShell / VS Code 的 UTF-8 环境落地修掉。

**已完成**
1. 已修改 VS Code 用户设置：
   - `C:\Users\aTo\AppData\Roaming\Code\User\settings.json`
   - 新增 `"files.encoding": "utf8"`
2. 已创建 PowerShell Profile：
   - `C:\Users\aTo\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1`
3. 已在 Profile 中统一：
   - `chcp 65001`
   - `[Console]::InputEncoding`
   - `[Console]::OutputEncoding`
   - `$OutputEncoding`
   - `Get-Content / Set-Content / Add-Content / Out-File / Export-Csv` 默认编码
4. 已完成复测：
   - `chcp = 65001`
   - 输入 / 输出 / 管道编码全部为 `utf-8`
   - 默认 `Get-Content` 已能正确读取 UTF-8 无 BOM 文档

**关键决策**
- 这轮证明了一个关键细节：
  - 只修 `chcp + Console 编码 + $OutputEncoding` 还不够
  - 如果不补 `Get-Content:Encoding`，PowerShell 5.1 仍会把 UTF-8 无 BOM 文档默认错读成乱码
- 因此当前固定口径应升级为：
  - **控制台编码 + 默认读写编码 + 编辑器编码** 必须一起修

**恢复点 / 下一步**
- 当前环境层已收口。
- 下一步如果继续推进，应直接转入：
  - `001部分回执.md` 定点修复
