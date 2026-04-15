请先严格继承你当前 `存档系统` 线程的最新现场，不要回到 `day1` 旧问题，也不要把这轮扩成“整个存档系统大重做”。

这轮唯一主线固定为：
只处理一个问题：`普通切场` 之外，`正式存档` 对“离开后未加载场景的 world state”到底应该怎么入盘，以及这件事现在是否需要、如何最安全地补。

你必须先继承并且不要推翻的当前真状态：

1. 这次用户显性撞到的问题是：
- `Primary` 里 runtime 掉落物在去 `Home` 再回来后消失

2. 当前已查实的结构事实：
- `WorldItemPickup.Save/Load`、`DropDataDTO`、`DynamicObjectFactory.TryReconstructDrop` 已存在
- `SaveManager.CollectFullSaveData()` / `ApplyLoadedSaveData()` 已有 `worldObjects` 主链
- 也就是说，“当前已加载 scene 内的 world object 正式入盘”这条链是存在的
- 但普通切场 continuity 当前不走 `SaveGame/LoadGame`，这部分由另一条线程去补 `PersistentPlayerSceneBridge` 的 runtime bridge

3. 这轮不是让你去抢 runtime bridge：
- 不要碰 `PersistentPlayerSceneBridge.cs`
- 不要碰 `SceneTransitionTrigger2D.cs`
- 不要去接玩家切场 continuity 本身
- 不要把 scope 扩成 `Town/Home/Primary` 全域 runtime 收尾

4. 这轮你要处理的，是更窄但也更关键的一层：
- 如果玩家在 `Home` 手动存档，此时 `Primary/Town` 已卸载
- 那么这些“已离场 scene 的 runtime world state”是否应该进入正式存档？
- 如果应该，当前 `GameSaveData.worldObjects` 是否足够承载？
- 如果不够，最小、最安全、最不破旧档的合同应该是什么？

这轮唯一主刀：
给“离场 scene 的 world state 入盘语义”做一次只读优先、必要时最小落代码的审计与合同收口。

允许你触碰的范围：
- `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
- `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
- 如确实有必要，可新增一个极小的 DTO / helper
- 你自己的 memory / thread memory / 审计日志

明确禁止漂移：
1. 不要碰 `PersistentPlayerSceneBridge.cs`
2. 不要碰 `SceneTransitionTrigger2D.cs`
3. 不要抢 `Town/Home/Primary` scene-side
4. 不要把这轮扩成 async save/load、旧档大迁移或全部 world object 再建模
5. 不要把“当前 loaded scene 的 worldObjects 已能入盘”误说成“off-scene state 也已经解决”

你这轮必须完成的事情，按顺序执行：

第一部分：先查实合同，不要先写代码
1. 重新核：
- `GameSaveData`
- `WorldObjectSaveData`
- `SaveManager.CollectFullSaveData()`
- `SaveManager.ApplyLoadedSaveData()`
- `ResolveTargetLoadSceneName()` / 切场读档入口
2. 必须先回答清楚 4 个问题：
- 当前玩家如果在 `Home` 存档，未加载的 `Primary/Town` runtime world state 会不会进盘？
- 如果不会，这是否符合当前项目想要的正式存档语义？
- 如果要进盘，现有 `worldObjects` 是否会在读档时造成“未加载 scene 也被立刻 restore / 重建”的错误副作用？
- 最小新合同是“扩现有字段”还是“单独新增 per-scene runtime snapshot 字段”

第二部分：如果需要落代码，也只能落最小合同
只有当你确认可以不抢 runtime bridge 文件、且不会把读档语义带偏时，才允许真实施工。
默认优先方案应是：
1. 给 `GameSaveData` 增加单独的 `scene-local world snapshot` 容器
2. 不要把 off-scene snapshot 直接粗暴并进现有 `worldObjects`
3. 读档时要能区分：
- 当前已加载 scene 立即 restore 的 world objects
- 其他 scene 先缓存、待 scene 真加载时再 restore 的 world state
4. 必须考虑旧档兼容和空字段兼容

第三部分：你这轮必须显式回答的边界
1. `Drop` 是否只是问题表面，正式入盘语义是否也必须覆盖 `Tree / Stone`
2. 手动存档时，如果玩家刚从 `Primary` 回到 `Home`，离场前 scene 的 runtime state 是否应该被视为“世界状态的一部分”
3. 新游戏 / fresh start / slot 切换 时，这层 off-scene snapshot 应如何清空
4. 是否存在 duplicated restore / 复制漏洞 / 错 scene restore 风险

第四部分：完成定义
这轮完成至少要满足二选一：

A. 审计型完成
- 你已把上面 4 个关键问题查实
- 已给出“应不应该入盘”的明确判断
- 已给出最小字段/合同方案
- 已给出不该现在就落代码的具体原因

B. 最小落代码完成
- 代码只改 `SaveManager / SaveDataDTOs / 极小 helper`
- 不碰 runtime bridge 文件
- 不破旧档
- 不引入 own red
- 已明确新的读取/缓存边界

第五部分：验证纪律
1. 全程守 `no-red`
2. 至少给出：
- `git diff --check`
- 最小 CLI 编译/红错检查
3. 如果只做审计，也要把“为什么这轮停在合同层而不是直接把存档改穿”说清

第六部分：收尾
1. 更新：
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\存档系统\memory_0.md`
- 如需，也同步你的工作区 memory
- `C:\Users\aTo\.codex\memories\skill-trigger-log.md`
2. 跑：
- `check-skill-trigger-log-health.ps1`
3. 如果这轮停下：
- 按规则 `Park-Slice`

你这轮结束时，必须明确回答：
1. 正式存档现在到底能不能覆盖离场 scene 的 runtime world state
2. 如果不能，最小正确合同是什么
3. 为什么这件事不是“再加一个字段”就结束
4. 你是否建议在 runtime bridge 落地前就直接动存档代码

thread-state 统一尾巴：
- 如果这轮从只读进入真实施工，先跑 `Begin-Slice`
- 第一次准备 sync 前，先跑 `Ready-To-Sync`
- 如果中途停下或本轮收尾，先跑 `Park-Slice`
