请先严格继承当前 `Codex规则落地` / `TownHomePrimary` 这条线的最新现场，不要回到泛分析，不要把 scope 扩成“整个存档系统重做”，也不要把这轮再写成说明文档。

当前唯一主线固定为：
只做 `普通切场` 的 `scene-local world continuity` 最小 runtime bridge，让玩家离开 `Primary/Town` 再回来时，当前场景里的关键 world state 不会直接蒸发。

你必须先继承并且不要推翻的当前真状态：

1. 用户这次撞到的显性问题是：
- 在 `Primary` 砍树，背包满了，地面有掉落物
- 去 `Home` 清背包
- 再回 `Primary`，发现地面掉落物全部消失

2. 当前代码链已经查实：
- `ItemDropHelper` 只负责生成掉落物到 `WorldItemPool`
- `WorldItemPickup` 会注册进 `PersistentObjectRegistry`
- `WorldItemPickup.Save/Load`、`DropDataDTO`、`DynamicObjectFactory.TryReconstructDrop`、`SaveManager.worldObjects` 说明“正式存档/读档恢复掉落物”主链已经存在
- 但普通切场走的是：
  - `SceneTransitionTrigger2D.TryStartTransition()`
  - `PersistentPlayerSceneBridge.QueueSceneEntry()`
  - `PersistentPlayerSceneBridge.CaptureSceneRuntimeState()`
- 当前 `PersistentPlayerSceneBridge` 只 capture/restore：
  - 背包
  - 快捷栏
  - resident runtime snapshot
- 当前它不会 capture/restore：
  - 掉落物
  - 树
  - 石头
  - 其他 scene-local world object

3. 当前关键判断已经钉死：
- 这不是 `ItemDropHelper` 单点 bug
- 这是“普通切场 continuity 只桥接玩家自身，不桥接 scene-local world objects”的结构缺口
- 而且打包前不能只补 `Drop`
- 因为从静态逻辑看，`Tree/Stone` 这类同样依赖 `PersistentObjectRegistry + Save/Load` 的对象，在普通切场里大概率也没有 continuity
- 如果只补掉落物，很容易出现“树/石头回场重置，但掉落还在”的复制漏洞

4. 这轮不是让你碰正式存档系统：
- 不要改 `SaveManager.cs`
- 不要改 `SaveDataDTOs.cs`
- 不要改 `DynamicObjectFactory.cs`
- 不要把普通切场偷改成一次磁盘 `SaveGame/LoadGame`
- 不要走 `DontDestroyOnLoad` 让掉落物全局常驻

这轮唯一主刀：
在 `PersistentPlayerSceneBridge` 侧补一个“内存态 scene-local world snapshot bridge”，白名单先只接：
- `Drop`
- `Tree`
- `Stone`

允许你触碰的范围：
- `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
- 如确实有必要，可新增一个极小的 runtime snapshot helper / DTO 文件
- 如要补最小测试，只补与你这刀直接相关的 targeted test

明确禁止漂移：
1. 不要碰 `SceneTransitionTrigger2D.cs`
2. 不要碰 `SaveManager.cs`
3. 不要碰 `SaveDataDTOs.cs`
4. 不要碰 `DynamicObjectFactory.cs`
5. 不要把 scope 扩成“所有 world object continuity 一次收完”
6. 不要去接 NPC / UI / Day1 / 导演 / 农田别线
7. 不要做 docs-only 停车

你这轮必须完成的事情，按顺序执行：

第一部分：先重新审现场和 owner
1. 重新确认当前 `PersistentPlayerSceneBridge.cs` 是否安全可接
2. 重新确认 `存档系统` 当前仍是 `PARKED`，避免误抢 `SaveManager` active 面
3. 如果当前 scene/player 桥接文件可安全写，立刻进入真实施工

第二部分：只打最小 runtime bridge
1. 在离场前 capture 当前 source scene 的白名单 world snapshot
2. snapshot 至少要能覆盖：
- `WorldItemPickup`
- `TreeController`
- `StoneController`
3. snapshot key 必须带 scene 维度，不能做成全局一锅
4. 回到同 scene 时 restore 这份 runtime snapshot
5. restore 应发生在玩家 regain control 前，避免玩家先看到错误基线
6. `fresh start` / runtime reset 时必须清掉这层 snapshot

第三部分：守住边界
1. 这层 bridge 只解决“普通切场 continuity”
2. 不改变正式磁盘存档语义
3. 不让 unloaded scene 的对象在别的 scene 中被直接重建
4. 不保留 world object 常驻根
5. 不把预摆 scene authoring 基线无脑覆盖掉；只对白名单 runtime state 做最小恢复

第四部分：完成定义
这轮完成，不是“写个结构”。
必须至少满足：
1. 代码已落地
2. 普通切场链已经具备 `Drop / Tree / Stone` runtime continuity
3. `fresh start` 不会继承旧 runtime snapshot
4. 不改正式 `SaveGame/LoadGame` 行为
5. 无 own red

第五部分：验证纪律
1. 全程守 `no-red`
2. 至少给出：
- `git diff --check`
- 最小 CLI 编译/红错检查
- 如能做 targeted probe，就补一份最小逻辑验证
3. 如果 live 黑盒还没跑到，也要把“结构已站住 / live 仍待验”分开写清

第六部分：收尾
1. 更新：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
- `C:\Users\aTo\.codex\memories\skill-trigger-log.md`
2. 跑：
- `check-skill-trigger-log-health.ps1`
3. 如果停下：
- 按规则 `Park-Slice`
4. 如果形成 own 小批且安全：
- 直接提交

你这轮结束时，必须明确回答：
1. 这轮实际把哪些 world object continuity 接上了
2. 哪些仍没接
3. 当前普通切场第一 blocker 是否已从“掉落物蒸发”前移
4. 正式存档这只球为什么还没交给自己处理

thread-state 统一尾巴：
- 如果这轮从只读进入真实施工，先跑 `Begin-Slice`
- 第一次准备 sync 前，先跑 `Ready-To-Sync`
- 如果中途停下或本轮收尾，先跑 `Park-Slice`
