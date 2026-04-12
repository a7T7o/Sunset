# 当前背景

这不是继续泛修 UI，也不是让你回吞 Workbench / Toolbar / Status / Placement。

当前唯一求助点固定为：

`SpringDay1PromptOverlay / 任务清单` 在打包后仍然存在老问题，UI 线程已经多轮下刀但仍未真正收平，现在需要你从 day1 语义 owner 口径给出明确裁定：它到底应该是什么治理模型、谁来主刀最后一刀、下一安全切片该怎么切。

# 当前现场

用户最新明确反馈：

1. 打包后任务清单仍然异常，老问题没有真正消失。
2. 用户体感上，它仍然不像 `toolbar / state` 那样老老实实待在固定 HUD 语义里。
3. 当前 UI 线程已经确认：这不是“再调一次透明度”能解的问题，而是 `PromptOverlay` 的治理模型一直没彻底统一。

UI 线程当前已掌握的代码事实：

1. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
   - 当前仍是独立 `ScreenSpaceOverlay + overrideSorting`
   - 当前固定排序值：
     - `sortingOrder = 142`
   - 它有自己的：
     - runtime instantiate / prefab fallback
     - fade
     - queued reveal
     - page flip / transition
     - card height / row layout 调整
2. 它不是像 `toolbar / state` 那样简单挂在统一 HUD Canvas 下随同一套缩放和遮挡规则走。
3. 目前代码里虽然有：
   - `SetExternalVisibilityBlock(bool blocked)`
   但从之前的整体验证历史看，这条“外部 page UI 正式压住 prompt”的治理链并没有真正收平。
4. 最新 packaged/live 反馈说明：
   - UI 线程之前切的“固定 sorting order / 不跟随父 Canvas 漂移”这刀不够
   - 真正的问题仍然是：`PromptOverlay` 的性质和 `toolbar/state` 不一致

# 你这轮不要做什么

1. 不要回吞 `Workbench`
2. 不要回吞 `Placement`
3. 不要回吞 `Status`
4. 不要回吞 Town / NPC / Dialogue 泛 UI
5. 不要把这轮变成“day1 整体再审一遍”

# 你这轮唯一要做什么

请你只回答并最好顺手落成一个可执行裁定：

1. 从 day1 owner 语义看，`任务清单` 最终到底应该是：
   - `A. 像 toolbar/state 一样并入统一 HUD 治理`
   - 还是
   - `B. 保持独立 overlay，但必须完整吃 page/dialogue/modal 的统一 suppress 语义`
2. 结合当前 day1 剧情推进真源，你认为：
   - 现在究竟是 UI 线程继续主刀能收平
   - 还是这个问题已经必须由 day1 主刀收最后一刀
3. 如果让 UI 线程继续主刀，你要明确给出：
   - 下一刀唯一允许碰的文件
   - 下一刀唯一目标
   - 明确禁止再漂移的范围
4. 如果你判断应该 day1 自己接回去主刀，也请直接说明：
   - 你会从哪条 day1 语义链切入
   - UI 线程只需要做什么配合

# 你必须回答的重点

请不要只说“应该和 toolbar 同级”这种概念句。

你需要直接回答：

1. `PromptOverlay` 到底应不应该继续保留现在这种独立 overlay 运行时模型？
2. 如果不该保留，应该收回到哪条真治理链？
3. 当前 packaged 仍异常，第一真实 blocker 是什么？
4. 下一安全切片只砍哪一刀？

# 回执格式

请按下面顺序回：

1. 你的裁定结论
2. 你为什么这么判
3. 这问题该谁主刀
4. 下一刀唯一切片
5. UI 线程要不要继续碰 `SpringDay1PromptOverlay.cs`

# 附加说明

这轮不是让你大改，只是让你作为 day1 owner 给出 authoritative 裁定，避免 UI 线程继续在错误治理模型上反复返工。
