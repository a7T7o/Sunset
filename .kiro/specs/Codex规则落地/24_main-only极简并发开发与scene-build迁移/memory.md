# 24_main-only极简并发开发与scene-build迁移 - memory

## 2026-03-21｜阶段建立
**当前主线目标**
- 把用户提出的三件事正式收成一个新治理阶段：
  - 极简并发开发的文件分区重整
  - `scene-build / spring-day1` 的回执与交接 prompt
  - `scene-build` 迁出 `Sunset_worktrees` 体系的正式执行准则

**本轮完成**
1. 已新建阶段目录 `24_main-only极简并发开发与scene-build迁移`。
2. 已把用户原文完整写入 `tasks.md` 头部。
3. 已按三大点拆出理解、任务和优先级。

**关键判断**
- 这轮不是简单改两个 prompt，而是要把“极简并发开发”从临时聊天口径升级成一个更清晰、可长期维护的轻治理入口。

**恢复点 / 下一步**
- 下一步先建立新的 prompt 目录和当前入口，再补 `scene-build / spring-day1` 两个回执型 prompt。

## 2026-03-21｜新目录、现行入口与 scene-build / spring-day1 回执 prompt 已正式补齐
**当前主线目标**
- 把这轮 `main-only 极简并发开发 + scene-build 迁移` 从聊天口径变成一个真正可找、可发、可维护的新阶段入口。

**本轮完成**
1. 已把当前现行批次目录固定为：
   - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01`
2. 已在新目录下补齐 6 份直接开发 prompt：
   - `scene-build`
   - `NPC`
   - `农田交互修复V2`
   - `导航检查`
   - `遮挡检查`
   - `spring-day1`
3. 已补齐 2 份回执 / 冻结 / 交接 prompt：
   - `scene-build_当前任务回执与迁移前冻结.md`
   - `spring-day1_当前任务回执与向scene-build交接.md`
4. 已重写两个现行入口文件，使旧引用自动指向新目录：
   - `恢复开发总控与线程放行规范_2026-03-21.md`
   - `2026-03-21_批次分发_09_恢复开发总控与线程放行规范.md`
5. 已把旧目录 `2026.03.21_恢复开发总控_01` 明确标成历史阶段。
6. 已把 `scene-build` 的正式迁移口径写死：
   - 当前现场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
   - 目标路径：`D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
   - 迁移方式：`git worktree move`
   - 迁移前提：必须先收一份冻结回执，不能趁 dirty 直接硬搬

**关键决策**
- 这轮不再把“极简并发开发”停留在临时聊天层，而是给它单独的目录、单独的入口和单独的回执型 prompt。
- `scene-build` 被正式定性为“待迁移的独立项目现场”，不是 shared root 普通线程。
- `spring-day1` 被正式定性为“向 scene-build 交接的内容线”，不是另开一个平行大场景面。

**恢复点 / 下一步**
- 现在可以直接把新目录里的 prompt 发给相应线程。
- 等 `scene-build / spring-day1` 两份回执回来后，再执行 `scene-build` 的正式迁移。
