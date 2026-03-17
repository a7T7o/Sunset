# 阶段任务：强制 skills 闸门与执行规范重构
## 阶段目标
- 把 `Sunset` 当前“有 skills 但不够强制”的缺口，收成真正会在每次实质性任务前触发的项目级启动闸门。

## 已完成
- [x] 确认当前最关键的问题不是“没有规则”，而是“规则没有形成强制启动顺序”。
- [x] 建立 `09_强制skills闸门与执行规范重构` 阶段目录。
- [x] 建立 `03-17` 版现行状态入口与强制闸门专项说明。
- [x] 确认同伴技能框架已经具备基础骨架，但还缺少 Sunset 项目级总闸门。
- [x] 把 `NPC/farm` 分支漂移事故纳入本阶段需求，作为新闸门必须吸收的现实案例。
- [x] 创建 `C:\Users\aTo\.codex\skills\sunset-startup-guard\SKILL.md` 第一版。
- [x] 为 `sunset-startup-guard` 补齐 `agents/openai.yaml` 与 `references/checklist.md`。
- [x] 更新 `D:\Unity\Unity_learning\Sunset\AGENTS.md`，将“实质性任务先过启动闸门”写成项目级硬规则。
- [x] 更新全局 `C:\Users\aTo\.codex\AGENTS.md`，补入“若项目要求启动闸门 skill，则必须先触发”的通用规则。
- [x] 在真实 Sunset 治理回合里实际使用 `sunset-startup-guard` 审计 `NPC / farm / spring-day1` 相关任务，确认它已能承接：
  - 主线锚定
  - `cwd / branch / HEAD` 核验
  - 共享根目录占用校验
  - 首条 commentary 结构约束

## 待办
- [ ] 决定是否需要为共享根目录占用单独落盘：
  - `root-workdir lease`
  - 或 `branch occupancy` 状态表
- [ ] 统一线程回复最小结构，避免继续出现“只答最后一句、主线漂移、分支上下文缺失”的问题。
- [ ] 检查当前 session 的 skills 列表与后续新线程是否已能稳定看到 `sunset-startup-guard`。
- [ ] 把“worktree 只作为事故隔离例外，不作为 Sunset 常态开发模型”写成明确口径，并让启动闸门优先引导回“根目录 + 分支”。
- [ ] 在至少一轮真实 Sunset 治理回合中验证：
  - 输出结构是否稳定
  - 是否减少了“明明有规则却没遵守”的偏航
  - 在跨线程、跨 session 时是否仍稳定生效

## 暂缓
- [ ] 暂不一次性重写所有历史规则正文。
- [ ] 暂不在没有真实验证前宣称“强制 skills 已彻底落地”。

## 完成标准
- Sunset 项目级启动闸门 skill 已创建并可读。
- `AGENTS.md` 已把闸门写成明确硬规则。
- 线程回复最低结构已形成统一口径。
- 闸门已吸收共享根目录占用 / 分支上下文校验。
- 至少一轮真实任务验证通过，且后续新线程 / 新 session 中仍稳定可见。
