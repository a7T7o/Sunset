# 阶段任务：main-branch-only 回归与 worktree 退役收口

## 阶段目标
- 彻底结束 Sunset 当前事故期对临时 `worktree` 的依赖。
- 让项目回到“共享根目录 + 分支”的默认开发模型。
- 达到“用户不需要再为了 `farm / NPC / spring-day1` 的日常开发和验收打开多个 Unity 工程”的可用状态。

## 当前口径
- 用户视角可用度仍按 `0%` 计，原因是：
  - `worktree` 还没有全部退役
  - 共享根目录还没回到中性 `main`
  - 日常验收仍可能被迫跳到独立容器
- 治理推进度不按“是否已经聊明白”算，而按“是否已经可直接执行并真正消灭 worktree 常态化”算。

## 已完成
- [x] 正式立项 `11`，承接 `09/10` 之后全部“回归 branch-only 常态”的剩余工作。
- [x] 锁定用户红线：`worktree` 只允许作为事故隔离例外，不能升级为 Sunset 默认开发流。
- [x] 盘点当前全部已注册 worktree 与其 `branch / HEAD / clean-dirty` 状态。
- [x] 确认 `farm` 合法 branch carrier：
  - `codex/farm-1.0.2-cleanroom001 @ 66c19fa1`
- [x] 确认 `NPC` 合法 continuation branch carrier：
  - `codex/npc-roam-phase2-002 @ 6e2af71b`
- [x] 确认 `spring-day1` clean checkpoint：
  - `codex/spring-day1-story-progression-001 @ a9c952b7`
- [x] 确认 `NPC_roam_phase2_rescue` 中 4 个 TMP 资源 dirty 归属 `spring-day1`，不归属 NPC。
- [x] 明确 `导航检查 / 遮挡检查 / 项目文档总览` 当前都不是 worktree 问题核心，不需要为了这轮 branch-only 回归再新建 worktree。
- [x] 产出本阶段执行方案文档。
- [x] 产出本阶段总进度与收口清单文档。
- [x] 产出可直接下发的 branch-only 回归 prompt 成品。
- [x] 建立线程回包统一汇总位：
  - `所有线程回归誓言.md`（索引）
  - `所有线程回归誓言\*.md`（真实线程回包）
- [x] 产出 shared root 第一版归属图：
  - `共享根目录dirty归属初版_2026-03-17.md`
- [x] 完成第二批待退役容器的最新核验：
  - `第二批worktree核验表_2026-03-17.md`
- [x] 收齐第一轮 branch-only 回包：
  - `spring-day1`
  - `NPC`
  - `农田交互修复V2`
  - `导航检查`
  - `遮挡检查`
  - `项目文档总览`
- [x] 已真实退役第一批纯历史 worktree：
  - `D:\Unity\Unity_learning\Sunset_worktrees\main-reflow-carrier`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC`
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-10.2.2-patch002`

## 待办

### A. 三大业务线归位
- [x] `spring-day1`
  - 已给出 shared root 恢复后的 branch-only 唯一入口：
    - `D:\Unity\Unity_learning\Sunset @ codex/spring-day1-story-progression-001 @ a9c952b7`
  - 已明确 `Primary.unity / DialogueUI.cs` 当前不是未固化尾巴
- [x] `spring-day1` 二轮裁定
  - shared root 的 `BitmapSong / Pixel / SoftPixel / V2` 与 `NPC_roam_phase2_rescue` 的 `BitmapSong / Pixel / SDF / V2` 是否同源
  - 这两组 dirty 各自是“导出证据后丢弃”还是“有一组仍需保留”
  - 处理完后，何时允许直接删 `NPC_roam_phase2_rescue`
- [x] 已完成 `spring-day1` 二轮裁定与执行
  - 5 个相关字体资产继续归 `spring-day1`
  - 两处字体 dirty 的证据已导出
  - shared root 与 rescue 的字体 dirty 已全部丢弃，保留已提交版本
- [x] `NPC`
  - 已明确 `codex/npc-roam-phase2-002` 是唯一 continuation branch carrier
  - 已给出从 shared root 恢复后回到 branch-only 的唯一入口
  - 已明确 `NPC_roam_phase2_rescue` 现在可降级为只读取证点
- [ ] `NPC` 最终迁回动作
  - shared root 回 `main` 后，从根目录检出 `codex/npc-roam-phase2-002`
  - 完成一次 `git status` clean 的 branch-only 验证
  - 验证通过后退役 `NPC_roam_phase2_continue`
- [x] `farm`
  - 已明确 `codex/farm-1.0.2-cleanroom001` 是唯一 continuation branch carrier
  - 已明确污染分支 `codex/farm-1.0.2-correct001` 永久只保留事故取证身份
  - 已给出 shared root 恢复后的 branch-only 唯一入口
- [ ] `farm` 最终迁回动作
  - shared root 回 `main` 后，从根目录检出 `codex/farm-1.0.2-cleanroom001`
  - 完成一次 `git status` clean 的 branch-only 验证
  - 验证通过后退役 `farm-1.0.2-cleanroom001`

### B. 剩余临时容器退役
- [x] 已处理 `NPC_roam_phase2_rescue` 上的 `spring-day1` 字体 dirty
- [ ] shared root 回 `main` 后，对以下分支做根目录 branch-only 验证：
  - `codex/spring-day1-story-progression-001`
  - `codex/npc-roam-phase2-002`
  - `codex/farm-1.0.2-cleanroom001`
- [ ] 验证通过后退役第二批 worktree：
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_continue`
  - `D:\Unity\Unity_learning\Sunset_worktrees\farm-1.0.2-cleanroom001`
  - `D:\Unity\Unity_learning\Sunset_worktrees\spring-day1-story-progression-001`

### C. 共享根目录恢复
- [x] 明确共享根目录当前 `dirty / deleted / untracked` 的第一版归属分类：
  - 治理线程自有
  - `spring-day1`
  - 历史遗留
  - 待归档
  - 待删除
- [x] 收齐三大业务线与三条辅助线的第一轮回包。
- [x] 将第一版归属图升级为最终可执行归属表。
- [x] 专门裁定 shared root 与 rescue 的字体 dirty 分裂问题。
- [x] 删除误复制残留 `Assets/111_Data/NPC 1.meta`。
- [ ] 裁定 `Assets/Screenshots*`、`npc_restore.zip` 的归属与去留。
- [ ] 给出 shared root 恢复为 `main` 的前置条件清单。
- [ ] 形成“共享根目录回到 `main` 后，第一份 branch-only 使用说明”。

### D. 规则与闸门固化
- [ ] 把“禁止把临时 worktree 升级为长期工作模式”写进现行治理口径。
- [ ] 让 `sunset-startup-guard` 在后续提示中优先推荐“根目录 + 分支”。
- [x] 明确 `导航检查 / 遮挡检查 / 项目文档总览` 当前不需要 worktree，默认也走 branch-only。
- [x] 把“`所有线程回归誓言` 目录是真实回包，`.md` 文件只是索引”写回 `11` 阶段口径。

### E. 最终验收
- [ ] 在第二批 worktree 退役完成后，验收 `git worktree list` 是否只剩共享根目录。
- [ ] 生成退役完成后的最终快照文档。

## 暂缓
- [ ] 暂不直接物理删除任何仍可能承载用户未交接现场的 worktree。
- [ ] 暂不在共享根目录脏现场上强行切回 `main`。
- [ ] 暂不把 `09/10` 继续当成剩余任务堆放地；后续收尾统一进入 `11`。

## 完成标准
- 用户后续不再需要为 `farm / NPC / spring-day1` 的正常开发或验收打开多个 Unity 工程。
- `farm / NPC / spring-day1` 的后续承载面都明确为“分支”，不是“某个 worktree 目录”。
- 共享根目录恢复到 `D:\Unity\Unity_learning\Sunset @ main`，重新成为默认中性现场。
- `git worktree list` 最终只剩共享根目录。
- `worktree` 被重新收束为事故隔离例外，不再是 Sunset 的新常态。
