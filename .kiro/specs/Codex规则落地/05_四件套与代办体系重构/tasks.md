# 阶段任务：四件套与代办体系重构

## 目标
- 彻底反思并重构 `requirements / design / tasks / memory` 四件套在 Sunset 中的使用方式。
- 建立“按阶段拆 tasks、必要时再开 design、memory 必须分卷”的新规范。

## 待办
- [x] 回读 `.kiro/specs/Steering规则区优化/2026.02.15_代办规则体系/` 相关资料，提炼当前仍有效的代办规则。
- [x] 输出 Sunset 当前“四件套失真点”清单。
- [x] 制定新的 workspace 轻量规范：
  - 根层默认仅 `memory.md`
  - 分阶段目录各自持有 `tasks.md`
  - `design.md` 按需创建
  - 不再默认长期维护全局单一 `tasks.md`
- [x] 制定 memory 分卷规范与旧卷索引规范。
- [x] 定义“什么时候应新建代办工作区，什么时候应留在原工作区推进”的判断标准。
- [x] 评估是否需要新建一个专门的 `sunset-todo-router` 或同类 skill。
- [x] 将旧全局 `tasks.md` 缩退为兼容路由页，不再承接新的治理续办。
- [x] 把新规范补入合适的活文档与 AGENTS/skills 约束中。

## 当前结论
- 当前先不新建 `sunset-todo-router`。
- 现阶段由 `skills-governor + sunset-workspace-router + Sunset工作区四件套与代办规范_2026-03-16.md` 共同承担代办路由。
- 升级条件固定为：
  - 同类代办误路由在两个以上独立场景重复出现；
  - 或用户明确要求更强自动化 / 更强强制执行。

## 完成标准
- 后续新的 Sunset 工作区不再默认产出灾难级膨胀 `tasks.md`。
- memory 分卷、阶段 tasks、代办承接入口都有清晰统一规则。
- 旧全局 `tasks.md` 不再承担新增治理续办职责。
