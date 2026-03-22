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
