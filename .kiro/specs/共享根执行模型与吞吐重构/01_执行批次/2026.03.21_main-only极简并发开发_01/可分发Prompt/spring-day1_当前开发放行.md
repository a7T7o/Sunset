# spring-day1 - 向 scene-build 输出 Day1 空间职责表

```text
你这轮不是继续做 UI / 字幕 / 对话实现，也不是另起一张新 scene。
你当前唯一职责是：把 `spring-day1` 的剧情流程，翻译成 `scene-build` 可以立刻施工的空间 brief，并把这份交付落到 `spring-day1` 工作区。

当前已知基线：
- shared root：`D:\Unity\Unity_learning\Sunset @ main`
- `scene-build` 现场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- `SceneBuild_01` 当前应被视为：春 1 日 `14:20` 进村后，到 `15:10` 农田 / 砍树教学前后的“废弃小屋 + 院落 + 小块农田”叙事场景
- 这张 scene 现在真正要承载的是：
  - 被带到住处
  - 屋内疗伤后出屋
  - 工作台闪回
  - 院内站位对话
  - 农田教学
  - 砍树教学
- 这张 scene 现在不该被扩成：
  - 整村总图
  - 矿洞骷髅威胁区
  - 饭馆晚餐冲突区
  - 纯美术摆场

你这轮必须交付 3 件事：
1. 把 Day1 全流程压成“场景线程可执行版本”
2. 给 `SceneBuild_01` 下正式定义：它是什么、它不是什么
3. 给 `scene-build` 一份可直接施工的空间职责表

你交付出来的内容，至少必须包含下面这些块：
- `Day1 场景模块清单`
  - 把 Day1 真正拆成几个场景模块
  - 每个模块各自承载哪段剧情
- `当前就该施工的模块`
  - 哪些模块现在就该进入 `scene-build`
  - 哪些模块现在只要留接口和认知，不急着搭
- `SceneBuild_01 的正式身份`
  - 它是什么
  - 它不是什么
- `SceneBuild_01 的强制承载动作`
  - 东侧进入
  - NPC 带入
  - 院落中心站位对话
  - 工作台交互与闪回
  - 农田教学落点
  - 砍树教学落点
  - 回屋 / 室内衔接
- `SceneBuild_01 的禁止误扩边界`
  - 明确哪些内容不要塞进去
- `给 scene-build 的精修优先级`
  - 先入口动线
  - 再院落留白与视线焦点
  - 再工作台与教学区关系
  - 再室内外衔接
  - 最后才是泛装饰扩张
- `给 scene-build 的落地提示`
  - 关键站位
  - 入口 / 出口
  - 互动点
  - 教学落点
  - 需要预留的触发 / 镜头 / 对话空间

这轮的产物不是“剧情复述”，而是 `scene-build` 看完就能继续搭的正式口径。
如果你写出来的东西还停留在“故事讲了什么”，那就说明这轮没做完。

建议落盘方式：
- 必改：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`
- 如需单独交付件，固定新增：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\scene-build_handoff.md`
- 同步你自己的线程记忆

这轮允许落点：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\`
- `spring-day1` 相关任务 / memory / 设计文档
- 你自己的线程记忆

这轮不要做：
- 不要直接改 `SceneBuild_01`
- 不要去 `scene-build` 的 worktree 里替它施工
- 不要做 Unity / MCP live 写入
- 不要顺手扩成另一个大场景项目
- 不要把重点又拉回 UI / 字幕 / 对话实现
- 不要去碰治理、Git 规则或别的线程现场

只有命中下面情况你才停：
1. 你发现这轮必须直接改 `scene-build` 正在写的同一个 scene 或同一批高危资源
2. 你发现自己已经不是在写“空间职责表”，而是在偷偷转成另一个施工线程
3. 你把 `spring-day1` 正文本身写乱了，需要先收口

聊天只回：
- 当前在改什么
- 正式交付文件路径
- changed_paths
- 是否触碰 Unity / MCP live 写
- 是否撞到高危目标
- handoff_ready: yes / no
- next_scene_build_focus
- blocker_or_checkpoint
- 一句话摘要
```
