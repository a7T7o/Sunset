# Codex规则落地

1. 当前线程名称
- `Codex规则落地`

2. 当前主线目标
- 维护 `D:\Unity\Unity_learning\Sunset` 的现行治理规则与活文档基线，确保后续线程按统一 Git / 开发口径继续推进。

3. 当前子任务 / 当前阻塞
- 本轮是在清理阻塞：执行统一冻结后的只读现场快照，不推进主线实现。

4. 当前现场锚点
- 工作目录：`D:\Unity\Unity_learning\Sunset`
- 当前分支：`main`
- 当前 `HEAD`：`f5ac305c2ccd86da1aa373fcaadae5218fed9d59`
- 如涉及 Unity：
  - 活动场景：`Primary`
  - MCP 状态：`可用`
  - Console 关键状态：`本轮读取最近 9 条日志，未见红编译；存在 Animator IK warning、GetPlaybackTime 非 playback 模式 warning、MCP-FOR-UNITY WebSocket closed warning`

5. 当前实际修改文件
- `本轮未新增修改，只做复核`

6. 本轮已验证事实
- 通过 `git branch --show-current` 与 `git rev-parse HEAD` 直接确认：当前现场是 `D:\Unity\Unity_learning\Sunset@main`，`HEAD` 为 `f5ac305c2ccd86da1aa373fcaadae5218fed9d59`。
- 通过 `git status --short --untracked-files=all` 直接确认：当前工作树仍存在多线程 dirty / untracked，不是干净树。
- 通过 `git status` 直接确认，A 类共享热文件中：
  - `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 当前有 dirty
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs` 当前有 dirty
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs` 当前未显示 dirty
- 通过 Unity MCP 直接确认：
  - 活动场景是 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
  - 场景名为 `Primary`
  - `read_console` 本轮可正常返回结果
- 通过本轮 Console 读取直接确认：最近返回的 9 条日志里没有红编译项，当前可见为 warning，不是新的红错误。
- 通过本轮只读复核直接确认：本轮没有新增代码、场景、Prefab、资源、规则文档修改，也没有提交、推送动作。

7. 当前判断 / 仍待验证
- 当前判断：`D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 与 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs` 的既有 dirty 存在共享热文件冲突风险，需要统一裁决 owner / 锁归属后才能继续写入。
- 当前判断：MCP 本轮读操作可用，但 Console 中的 `MCP-FOR-UNITY [WebSocket] Connection closed` warning 是否代表后续写操作风险，本轮未做写向验证，仍待验证。
- 旧结论引用：此前有“当前 Unity 验证链基本恢复”的口径；本轮已复核其中只读部分，`manage_scene/read_console` 读链可用这一点成立，但本轮未复核写向调用与编译触发链。

8. 当前占用或申请的 A 类物理锁
- `已存在未持锁 dirty`
- 目标文件绝对路径：`D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
  - 当前状态：`已有 dirty`
  - 唯一目标：当前线程本轮只做现场申报，不继续写入；等待统一裁决是否需要补发锁、转移 owner 或要求回退
  - 预估最小 checkpoint：`先明确 owner / 锁归属，再决定是否允许后续任何增量修改`
- 目标文件绝对路径：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs`
  - 当前状态：`已有 dirty`
  - 唯一目标：当前线程本轮只做现场申报，不继续写入；等待统一裁决是否需要补发锁、转移 owner 或要求回退
  - 预估最小 checkpoint：`先明确 owner / 锁归属，再决定是否允许后续任何增量修改`
- 目标文件绝对路径：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
  - 当前状态：`只读观察`
  - 唯一目标：当前线程本轮仅确认其不在既有 dirty 中
  - 预估最小 checkpoint：`冻结解除后如需写入，先查锁并申请锁`

9. 当前唯一阻塞点
- A 类共享热文件已有未持锁 dirty，当前缺少统一 owner / 锁归属裁决，阻断后续任何安全写入。

10. 下一步动作
- 冻结解除后，先按统一裁决处理 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity` 与 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs` 的既有占用，再决定本线程是否继续只动非 A 类治理文档。

11. 每一步的验收点
- 动作 1：`A 类既有 dirty 的 owner / 锁归属明确，且不再存在“未持锁继续写入”的灰区`
- 动作 2：`若本线程恢复工作，只在获准范围内继续，且不新增未申报的 A 类改动`

12. 是否需要我做动作
- `不需要`