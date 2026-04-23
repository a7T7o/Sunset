# 存档系统 - 活跃入口记忆

> 2026-04-10 起，旧根母卷已归档到 [memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/memory_0.md)。本卷只保留当前主线与恢复点。

## 当前定位
- 本工作区继续作为存档主线索引，但不再把旧 3.7.x 基底、三场景持久化、Home seed 化、工作台刷新、存档 UI 尾差继续混在一条根卷里。

## 当前状态
- **最后校正**：2026-04-10
- **状态**：活跃卷已重建
- **当前活跃阶段**：
  - `4.0.0_三场景持久化与门链收口`

## 当前稳定结论
- 当前重点已经不是“该不该做持久化”，而是：
  - `Primary / Town / Home` 三场景 persistent baseline 的 live 门链收口
  - Home seed 化后的体验尾差
  - 持久化 UI / runtime-first 资产定位 / 工作台刷新合同

## 当前恢复点
- 后续三场景持久化、Home、persistent player、workbench 刷新、存档 UI 尾差统一先归：
  - [4.0.0_三场景持久化与门链收口](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
- 查旧 3.7.x 基底和历史索引时，再回看 `memory_0.md`

## 2026-04-13 追加索引｜Day1 restore hygiene 已落到 4.0.0，off-scene world-state 仍停合同层
- 当前 `存档系统` 主线没有换：
  - 仍统一回 `4.0.0_三场景持久化与门链收口`
- 今日新增的有效进展是：
  - `SaveManager` 已补 `load/restart` 前的统一恢复卫生链
  - `Day1` 的 stale prompt / stale modal / stale pause / stale input 已不再只靠 story snapshot 自己兜底
  - 新增了 `SaveManagerDay1RestoreContractTests` 作为轻量 source-contract 护栏
- 今日明确没有越权继续做的是：
  - `off-scene world-state` 仍未正式入存档
  - 当前结论仍是：
    - 正式存档只收当前 loaded scene 的 `worldObjects`
    - 已离场 scene continuity 仍在 `PersistentPlayerSceneBridge`
    - 后续若真要入盘，必须走单独 per-scene snapshot 合同，而不是把它粗暴并进现有 `worldObjects`
- 详情恢复点已写入：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-14 追加索引｜build fail 归因已落到 4.0.0
- 本轮用户贴出的打包日志已经做过只读归因。
- 当前明确结论：
  - 这次真正阻塞 Player build 的不是 `SaveManager`
  - 真实红错是 `DayNightManager` 在 runtime 编译面调用了仅 `UNITY_EDITOR` 下存在的 `EditorRefreshNow()`
- `CloudShadowManager` 的 `ExecuteAlways / EditorUpdate / DontSave` 断言噪音已记录为次级治理项，但不是当前第一主因。
- 后续若进入真实施工，仍统一先回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-16 追加索引｜背包/toolbar/剧情/Home 混合坏相已定位到 4.0.0
- 本轮用户反馈的“重新开始、进入剧情、进出 Home 后背包/toolbar/交互一起变怪”已做只读总排查。
- 当前最重要结论不是“存档又坏了一个点”，而是：
  1. 背包真选中、InventoryPanel 本地选中、BoxPanel 本地选中、sort 语义当前并不统一
  2. `restart / 剧情 / Home` 的 runtime rebind 又会把这套分叉放大
- 当前明确可钉死的显性问题包括：
  - `HotbarSelectionService.SelectInventoryIndex()` 对 `>=12` 槽位不发 `OnSelectedChanged`、不重装备
  - `InventorySlotUI.ResolveSelectionState()` 在箱子打开时把 Box 与 Inventory 两套选中态做 `OR`
  - `InventorySortService.SortInventory()` 和 `PlayerInventoryData.Sort()` 的 hotbar 处理语义不一致
- 详情与下一刀建议已落到：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜背包/toolbar rebind 代码层已过验证，Primary 农地离场丢状态已钉到 off-scene continuity 缺口
- 本轮先把上一刀的第二簇验证收完：
  - `PersistentPlayerSceneBridge.cs`
  - `InventoryRuntimeSelectionContractTests.cs`
  - `SaveManagerDay1RestoreContractTests.cs`
  - `WorkbenchInventoryRefreshContractTests.cs`
  当前口径是：代码层证据成立，但还没做 live 终验。
- 同时新增一个跨线程边界结论：
  - `toolbar 固定槽位 4/8 丢图标` 应继续由 `农田交互修复V3` 主刀
  - 但它不能再继续占 `PersistentPlayerSceneBridge.cs`
  - 已生成边界收口 prompt：
    - [2026-04-17_给农田交互修复V3_toolbar图标与scene-rebind边界收口prompt_01.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/2026-04-17_给农田交互修复V3_toolbar图标与scene-rebind边界收口prompt_01.md)
- 用户新补的 `Primary` 农地/浇水状态离场后消失，这轮已做只读归因：
  - 正式存档当前仍只收当前 loaded scene 的 `worldObjects`
  - `PersistentPlayerSceneBridge` 的离场 continuity snapshot 当前只覆盖 `WorldItemPickup / TreeController / StoneController`
  - `FarmTileManager / CropController` 虽然实现了 `IPersistentObject`，但没有进入离场 continuity 捕获名单
- 所以当前最准确判断是：
  - 这不是单纯 UI 显示问题
  - 是 `Primary` 农田 world-state 还没正式进入 off-scene continuity 合同
- 详细证据和恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜跨场景箱子/农地/背包整链坏相已确认不是单点 bug
- 本轮按用户最新反馈做了只读总排查，目标不是修一个点，而是回答“为什么现在跨场景后像整套箱子/背包系统全面瘫痪”。
- 当前已经钉实的人话结论是：
  1. 正式普通存档当前仍主要保存“当前已加载场景”的世界对象
  2. 切场 continuity 只覆盖掉落物 / 树 / 石头，不覆盖箱子 / 农地 / 作物
  3. 箱子因此会在重新进场后重新吃作者预设内容，表现成内容复活
  4. 农地与作物因此会在离场回来后像没种过、没浇过
  5. 持久 UI 壳体仍跨场景活着，world-state 却没统一连续，所以背包 / 箱子 / toolbar 会一起表现成显示、选中、手持、交互都不稳定
- 这轮不是 claim 修好，而是把后续施工必须面对的根因总图彻底收口。
- 随后又补到一层背包 / toolbar / 输入门控结论：
  1. 切场只保留 hotbar 高亮，不保留真实背包选槽来源
  2. 重开 / 切场时会把真实背包选槽压回 hotbar 同号格
  3. 剧情开始只关箱子不关背包，但工具使用 / 移动 / 切栏统一受“是否有面板打开”门控影响
  4. 所以玩家体感会变成“高亮像还在，但工具、种子、toolbar、交互一起像卡死”
- 详细证据与恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜0417 总控活文档已建立
- 本轮不是继续零散开刀，而是按用户要求把这条线收成一份新的长语义 + 长任务主控文档：
  - [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
- 当前对 `0417.md` 的定位已经固定：
  1. 不是 `memory.md`
  2. 不是一次性 `tasks.md`
  3. 是这条存档线下一轮持续维护、持续领取任务、持续回写验证的总控活文档
- 文档当前已包含：
  1. 文档定位与维护规则
  2. 用户裁定冻结
  3. 存档语义梳理
  4. 已证实 / 高概率问题树
  5. 需求拆分
  6. 执行顺序
  7. 测试矩阵
  8. 滚动任务区
  9. 迭代记录
- 当前默认恢复点也因此更新为：
  - 先看 [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
  - 再回 [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md) 补技术细节

## 2026-04-17 追加索引｜0417 一致性整改已完成
- 用户随后对 `0417.md` 做了一轮静态一致性审视，指出 6 类真实问题：
  1. `P0-B` 与真实施工 / 第二簇验证脱节
  2. 默认档语义误写成 `primary baseline`
  3. `F5 / F9` 描述过期
  4. 维护规则漏掉 `thread-state`、线程记忆与审计层
  5. 跨线程边界未回填最新 `PersistentPlayerSceneBridge.cs` 禁并写裁定
  6. 测试矩阵还是静态 checklist，不是活验证板
- 本轮已直接整改 `0417.md`，当前更准确状态是：
  1. 默认档固定为 `Town 原生入口 / 进村开局`
  2. `F5` 当前停用，`F9` 仍是默认档读取入口
  3. `P0-B` 改为：
     - 第一轮代码层修补已落地
     - 第二簇验证已完成
     - live / packaged 终验待测
  4. `当前建议的下一刀` 已从 `P0-B` 改成 `P0-C｜world-state 主链`
  5. 测试矩阵已补成 `代码层 / live / packaged` 三层状态板
  6. 维护规则已补入：
     - `Begin-Slice / Ready-To-Sync / Park-Slice`
     - 工作区 memory
     - 线程记忆
     - `skill-trigger-log`
- 当前对 `0417.md` 的最新判断是：
  - 仍不是“最终完稿”
  - 但已经从“初始化版主控稿”升级成“可继续领活的控制板”

## 2026-04-19 03:08 追加索引｜正式存档 payload 静态复审已修正若干旧结论
- 当前主线没有换：
  - 仍是打包前收住 `存档 / 读档 / 重开`
- 本轮不是施工，而是按用户指定文件做只读静态审计，专门回答：
  - 正式存档 payload 现在到底已经正式覆盖了哪些部分
  - 还剩哪些真实漏项
  - 最小最安全修法应怎么排序
- 这轮最重要的新校正不是“又发现一堆都没做”，而是把几条已经过时的旧判断纠正回来：
  1. `offSceneWorldSnapshots` 现在已经是 `GameSaveData` 正式字段，`SaveManager` 保存时导出、读档时导入；不能再说 `off-scene world-state` 完全没进正式存档文件。
  2. `PersistentPlayerSceneBridge` 当前的 off-scene snapshot capture 已覆盖：
     - `FarmTileManager`
     - `CropController`
     - `ChestController`
     - `WorldItemPickup`
     - `TreeController`
     - `StoneController`
  3. `ChestController.Save/Load()` 现在已经正式收：
     - 箱子内容
     - `isLocked`
     - `origin`
     - `ownership`
     - `hasBeenLocked`
     - `currentHealth`
     不能再沿用旧结论说它只存锁态和 contents。
  4. `WorldItemPickup` 现在也已经把 `runtimeItem` 通过 `DropDataDTO.runtimeItem` 正式写进 / 读回 payload；不能再说地面掉落物一定会被压扁成纯 `itemId/amount`。
  5. 玩家背包当前 live 正式入口不是根 `GameSaveData.inventory`，而是 `worldObjects` 里的 `PlayerInventory`；`PlayerSaveData` 只负责：
     - 玩家位置 / scene
     - `selectedHotbarSlot`
     - `selectedInventoryIndex`
  6. 装备栏当前已通过 `EquipmentService` 进入 `worldObjects`，并且 `SaveManager.ValidateRequiredSavePayloads()` 会在写盘前硬检查它是否存在。
- 这轮静态审计后，当前更准确的真实漏项只剩两类：
  1. `workbench` 制作队列 / ready output / floating progress 仍未正式持久化。
     - 现口径不是“已保存”，而是 `StoryProgressPersistenceService.CanSaveNow()` 显式阻止玩家在制作中、待领取或队列未收束时保存。
  2. 正式 payload 的写盘前硬守门仍偏窄。
     - `ValidateRequiredSavePayloads()` 现在只硬检查：
       - `StoryProgressState`
       - `PlayerInventory`
       - `EquipmentService`
     - 但不会在写盘前额外断言：
       - `Chest / FarmTileManager / Crop / Drop`
       - `offSceneWorldSnapshots`
       是否真的已被收进这次快照
- 对用户这轮关心的重点系统，当前最准确口径更新为：
  - 已正式入盘：
    - 玩家背包
    - toolbar / inventory 选中来源
    - 装备栏
    - 箱子规则态与内容
    - 地面掉落物 `runtimeItem`
    - 农地 / 作物
    - off-scene per-scene world snapshot
  - 仍未正式闭环：
    - workbench 活跃制作队列与待领取产物
    - 更广覆盖面的 payload completeness guard
- 如果后续进入真实施工，最小安全排序应改成：
  1. 先决定 `workbench` 是继续维持 blocker 语义，还是要正式入盘
  2. 若要继续打包前保守收口，就先补 payload completeness guard，而不是先改已有 DTO
  3. 不要再把 `Chest / Drop.runtimeItem / off-scene snapshot` 当成本轮第一优先漏项

## 2026-04-17 追加索引｜已生成下一轮以 0417 开工的自用引导 prompt
- 用户本轮不要求继续施工，只要求我基于当前最新 `0417.md` 状态，产出一份“下一轮的我”可直接接续的引导 prompt。
- 当前这份 prompt 的定位已经固定为：
  1. 先读 [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
  2. 当前唯一主刀固定为 `P0-C｜world-state 主链`
  3. 只收：
     - off-scene world-state 正式合同
     - 箱子跨场景 / 跨天闭环
     - 农地 / 浇水 / 作物跨场景闭环
  4. 禁止漂回 `P0-B`、`F5/F9`、Save UI、Day1 own staging、packaged smoke

## 2026-04-17 追加索引｜0417 复审后外发口径改为纯文本 prompt
- 用户随后明确要求：
  - 不再为这件事生成 tracked prompt 文档
  - 后续如果要继续外发，只给纯文本 prompt
- 因为 `0417.md` 整改已完成，我对当前态的最新复审结论是：
  1. 文档层纠偏已经到位
  2. 当前下一刀继续落在：
     - `P0-C｜world-state 主链`
  3. `P0-B` 不再是下一刀主刀，而是：
     - `live / packaged` 终验与必要补刀
- 我上一轮临时生成的 F5/F9 补充 prompt 文件已删除，不再保留 tracked 文档版本。

## 2026-04-18 追加索引｜Town 只剩 House 4_0 遮挡异常已收成组件自愈修口
- 用户把遮挡现场继续收窄后，当前最新有效结论是：
  1. `Town` 里“很多物体有两个 OcclusionTransparency”这个观察成立
  2. 但 `House 4_0` 的剩余异常，不能只归因为场景里多了一份组件
- 这轮真实修口落在：
  - `Assets/YYY_Scripts/Service/Rendering/OcclusionTransparency.cs`
- 当前代码层最核心变化是：
  - `OcclusionTransparency` 新增按需 renderer cache 自愈
  - 避免组件在 `Awake` 之外被访问时，`mainRenderer` 仍为空，导致 `GetSortingLayerName()` 读成空字符串，进而被 `OcclusionManager.sameSortingLayerOnly` 前置过滤直接跳过
- fresh 回归：
  - 建筑像素孔洞票已从 fail 变成 pass
  - 遮挡最小 5 票回归 `5/5 passed`
- 当前仍未 claim 的部分：
  - `Town / House 4_0` 用户 live 终验还没拿到
  - `Town` 场景里历史重复组件残留这件事，这轮只做了归因，没有直接清 production scene
- 详细恢复点已写入：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜P0-C 第一刀已把 off-scene world-state 合同与箱子/农地 continuity 收进代码
- 本轮不是继续讨论 `0417`，而是按 `P0-C｜world-state 主链` 进入真实施工。
- 当前已经落地的关键变化是：
  1. `GameSaveData` 新增 `offSceneWorldSnapshots`
  2. `SaveManager` 保存时导出、读档时导入 off-scene snapshots
  3. `PersistentPlayerSceneBridge` 在 continuity 里正式纳入：
     - `ChestController`
     - `FarmTileManager`
     - `CropController`
  4. 跨场景读档前会先抑制目标 scene 的 continuity restore，避免当前 load scene 被旧 runtime snapshot 抢先覆盖
- 当前最准确的人话结论是：
  - 正式存档与切场 continuity 的分工合同已经第一次真正统一：
    - 当前 scene 用 `worldObjects`
    - 已离场 scene 用 `offSceneWorldSnapshots`
- 本轮验证状态：
  - `SaveManagerDay1RestoreContractTests`：`6/6 passed`
  - `SaveManager.cs / PersistentPlayerSceneBridge.cs`：`owned_errors=0 external_errors=0`
  - `errors --count 20`：`errors=0 warnings=0`
- 当前仍未 claim 的部分：
  - `Primary / Home / Town` 往返的 live / packaged 终验还没做
  - `7.3 / 9.1 / 9.2 / 9.3` 仍待人工路径复测
- 详细技术恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-23｜shared-root own 保本上传收口

### 本轮目标
- 用户这轮明确要求：暂停存档功能施工，只做 `存档系统` 当前 own 成果的最小安全归仓与 push。
- 执行入口：
  - `.kiro/specs/Codex规则落地/2026-04-23_给存档系统_shared-root完整保本上传与own尾账归仓prompt_01.md`
  - `.kiro/specs/Codex规则落地/2026-04-23_shared-root完整保本上传分发批次_01.md`

### 已完成
1. 已重审 own 脏改并重新分类：
   - `A｜已成功归仓`
     - `.codex/threads/Sunset/存档系统/memory_0.md`
     - `.kiro/specs/存档系统/memory.md`
     - `.kiro/specs/存档系统/memory_0.md`
     - `.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md`
     - `.kiro/specs/存档系统/0417.md`
     - `.kiro/specs/存档系统/2026-04-17_给农田交互修复V3_toolbar图标与scene-rebind边界收口prompt_01.md`
   - `B｜shared/mixed，未吞`
     - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
     - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
     - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
     - `Assets/YYY_Tests/Editor/StoryProgressPersistenceServiceTests.cs`
     - `Assets/YYY_Tests/Editor/WorkbenchInventoryRefreshContractTests.cs`
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
   - `A 候选但未能归仓`
     - `Assets/YYY_Scripts/Data/Core/InventoryItem.cs`
     - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
     - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
2. docs/prompt 簇已用 `git-safe-sync` 成功提交并 push 到 `origin/main`。
   - 提交：`90e95fc7`
   - 结果：docs own 根已 clean

### 未完成与 blocker
1. `Assets/YYY_Scripts/Data/Core` 这 3 个文件没有归仓，不是因为 mixed，而是 `Ready-To-Sync` 的 preflight 运行态挂死：
   - 子链路卡在 `CodexCodeGuard -> git diff --name-status HEAD --`
   - 父级 `Ready-To-Sync` 进程一旦被 shell timeout 打断，就会留下 stale `ready-to-sync.lock`
2. 本轮已做运行态止血：
   - 确认 stale lock 无持有者后删除
   - 清理本线程自己留下的 `Ready-To-Sync / git-safe-sync / CodexCodeGuard / git diff` 挂起进程
   - 最后执行 `Park-Slice`，避免继续占用 shared-root 运行态

### 当前结论
- 这轮已经完成“own docs/prompt 保本上传”。
- 这轮没有完成 `Data/Core` 代码簇上传。
- 当前代码簇 blocker 是运行态工具链，不是语义归类争议；恢复时应从 `CodexCodeGuard preflight 为什么卡在 git diff` 继续，而不是重新审 own/mixed 边界。

## 2026-04-22 追加索引｜存档、跨场景承接与持久化链草稿本级只读审计已完成
- 本轮不是继续改代码，而是按用户指定入口做一轮“可反复迭代的技术提取底稿”式只读审计。
- 当前新钉实的总判断：
  1. `SaveManager + StoryProgressPersistenceService + PersistentPlayerSceneBridge + SceneTransitionTrigger2D` 已经构成 Sunset 现行的正式存档与跨场景承接主链。
  2. 正式存档不再只有当前 scene 的 `worldObjects`：
     - 当前 scene 仍走 `worldObjects`
     - 已离场 scene 现在正式走 `GameSaveData.offSceneWorldSnapshots`
  3. 玩家承接已经拆成三层：
     - `PlayerSaveData` 只收位置 / scene / hotbar 选中来源
     - 背包与装备分别由 `PlayerInventory / EquipmentService` 进 `worldObjects`
     - 当前工具态靠 `HotbarSelectionService.RestoreSelectionState()` 触发重装备，不是单独 DTO
  4. `StoryProgressState` 已正式收：
     - phase
     - completedDialogueSequenceIds
     - Day1 关键导演长期态
     - health / energy
     - npcRelationships
     - `workbenchHintConsumed`
     - `workbenchStates`
  5. 当前仍不能夸大的边界：
     - off-scene resident/NPC 位置仍只在 bridge runtime cache，不在正式 save 文件
     - payload 完整性 hard guard 仍偏窄
     - SaveSlot summary 仍混有旧根 DTO 视角，不等于当前主 payload 真相
- 这轮同时校正了两条旧口径：
  1. 旧 memory 里“bridge 不覆盖 chest/farm/crop”的结论已过时
  2. 旧 memory 里“workbench 仍只有 blocker、没有正式 DTO”的结论已过时
- 详细技术拆解、可写简历事实句与禁写边界，统一回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-20 18:01 追加索引｜工作台 runtime state 已进入正式持久化第一刀
- 本轮用户已明确批准直接开修“工作台跨场景 continuity + 正式存档保存/恢复”。
- 这轮真实施工已经落下的核心点：
  1. `SaveDataDTOs` 新增 `WorkbenchRuntimeSaveData / WorkbenchQueueEntrySaveData`
  2. `StoryProgressPersistenceService` 新增 `workbenchStates` 正式 snapshot，并接入 save 前 flush / load 后 replace + notify
  3. 工作台旧的 save blocker 口径已从“继续硬拦”切到“正式入盘”
  4. `SpringDay1WorkbenchCraftingOverlay` 不再在 `OnDisable()` 直接清空 session，而是先挂起协程、flush 持久态、再脱离场景展示壳
  5. 工作台各关键 mutation 点已补 `FlushCurrentRuntimeStateToPersistence()`
  6. `SpringDay1UiLayerUtility.ResolveUiParent()` 已优先走 `PersistentPlayerSceneBridge.GetPreferredRuntimeUiRoot()`
  7. 两份 Editor 合同测试已改到新语义：不再要求工作台 ready/floating state 必须 blocker，而是要求正式 save/load round-trip 与持久 UI root 合同成立
- 当前验证口径：
  - 4 个目标文件 `validate_script` 均无 own red
  - `errors` fresh console = `0 errors / 0 warnings`
  - compile-first 仍受 `CodexCodeGuard timeout + stale_status` 影响，CLI 只能诚实落成 `blocked / unity_validation_pending`
- 这条记录覆盖此前“工作台继续维持 blocker 口径”的临时判断；详细施工细节统一回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-20 18:20 追加索引｜读取存档语义已与保存语义正式拆开
- 用户本轮明确纠偏：
  - `读取存档` 不该继续吃 `保存` 的 Day1 时间窗
  - 正确语义是：只要不在剧情接管中，就应允许读取并刷新全局状态
- 这轮真实施工已落：
  1. `SpringDay1Director` 拆成：
     - `TryGetStorySaveBlockReason(...)`
     - `TryGetStoryLoadBlockReason(...)`
     - `TryGetStorySaveLoadBlockReasonInternal(...)`
  2. Day1 最小时间窗只保留在 `save` 路径，`load` 不再提前被 `IsDay1SaveLoadWindowOpen(...)` 放行/拦截
  3. `StoryProgressPersistenceService.CanSaveNow()` 走 `TryGetStorySaveBlockReason(...)`
  4. `StoryProgressPersistenceService.CanLoadNow()` 走 `TryGetStoryLoadBlockReason(...)`
  5. `PackageSaveSettingsPanel` 已显式区分 `canSave` 与 `canLoad`
  6. 设置页默认槽 / 普通槽读取动作都会先检查 `CanExecutePlayerLoadAction(...)`，命中 blocker 时直接提示真实原因
  7. 读档后的全局刷新链已补合同护栏：
     - `PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings()`
     - `RefreshAllUI()`
     - `PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot()`
- 当前验证：
  - 相关脚本/测试 `validate_script` 无 own red
  - `errors` fresh console = `0 errors / 0 warnings`
  - compile-first 仍受 `stale_status` 干扰，未拿到 fresh compile 闭环

## 2026-04-20 追加索引｜工作台跨场景 / 存档丢失根因已钉成“宿主 + blocker 盲区 + 生命周期”三层
- 本轮用户要求只读彻查“工作台有内容时切场回来会丢、存档加载也会丢”，并要一份最安全的彻底修法，不改代码。
- 新钉实的核心结论：
  1. 工作台队列真源现在在 `SpringDay1WorkbenchCraftingOverlay`，不在 `CraftingService`。
  2. 正式存档没有工作台 queue / ready outputs / partial progress DTO，`StoryProgressPersistenceService` 只保存长期剧情态，还会在恢复时把工作台 runtime 镜像字段清零。
  3. `PersistentPlayerSceneBridge` 切场只重绑 `CraftingService` 的背包事实源，不维护工作台队列。
  4. 当前 save blocker 还有一个关键离场盲区：
     - `HasReadyWorkbenchOutputs / HasWorkbenchFloatingState` 只在工作台状态绑定当前 active scene 时才会拦
     - 离开工作台所在 scene 后，仍有 queue/ready outputs 也能存，但存档并不会保存它们
  5. `SpringDay1WorkbenchCraftingOverlay.OnDisable()` 会 `StopCraftRoutine() + CleanupTransientState(resetSession: true)`，是真实清空点；而它的 runtime 父节点当前又是 `GameObject.Find("UI")`，不是 bridge 的 persistent UI root，因此切场/duplicate UI 清理时有机会直接把工作台状态一起带走。
- 当前判断：
  1. 这不是单字段漏存，而是工作台 persistence 合同根本没闭环。
  2. 最安全的彻底修法不是继续给 overlay 打补丁，而是把工作台真值下沉到正式 runtime 宿主，再接 save/load 和切场 continuity。
- 详细分层、修法顺序、不要走的假修法，已回写到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\4.0.0_三场景持久化与门链收口\memory.md`

## 2026-04-20 追加索引｜Day1 最小存档时间窗与新建槽提示已落地
- 用户后续把修法明确压成两个最小目标：
  1. `0.0.6` 后的 Day1 不要再一路卡到第二天才允许存/读档
  2. “新建存档”当前不能保存时要给明确提醒
- 这轮已落地的真实结果是：
  1. `SpringDay1Director` 现在在 `TryGetStorySaveLoadBlockReason(...)` 前先检查 Day1 最小放行窗
  2. 放行规则固定为：
     - `Year1 / Spring / Day1`
     - `0.0.6` 已打开时 `16:01 ~ 17:59`
     - `19:31+`
  3. 未命中时间窗时，仍继续沿用原本按阶段给出的 blocker 文案
  4. `PackageSaveSettingsPanel` 当前的“新建存档”行为会点击后显式 toast blocker reason，不再表现成无反应
- 当前恢复点继续统一回：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-18 追加索引｜toolbar 固定槽位 4/8 只读判断改收 ToolbarUI 自身
- 用户这轮把问题收窄成：
  - `toolbar` 固定槽位 `4/8`（第 `5/9` 格）切场后图标/显示异常
  - 进出 `Home` 又会恢复
  - 只要根因、解释和打包前最安全最小修法
- 本轮新的最高概率判断是：
  1. 主要脆弱点不再优先怀疑 `InventoryPanelUI / BoxPanelUI`
  2. 更像是 [ToolbarUI.Build()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Toolbar/ToolbarUI.cs) 仍靠“子物体名字解析索引 + 去重”在做重绑
  3. 而 [PersistentPlayerSceneBridge.RebindPersistentCoreUi()](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 每次切场都会重新调用这条 fragile build 链
- 本轮同时静态核掉了一条伪嫌疑：
  - `Home / Primary / Town / ToolBar.prefab` 的 `ToolBar/Grid` authored 子节点顺序本身是一致的
  - 所以 `4/8` 不是 scene authored 顺序写错，更像“通病在 toolbar，4/8 只是最稳定暴露的两个位置”
- 当前建议的最小安全修法也随之收窄为：
  - 只改 `ToolbarUI.Build()`
  - 让 toolbar 和 inventory panel / box panel 一样，直接按 sibling 顺序 `0..11` 绑定
  - 不再继续依赖 `ResolveToolbarSlotIndex()` 的名字推断
- 详细恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜P0-B 第二簇真修补与 P0-C 第二刀护栏已继续推进
- 本轮不是停在 `0417` 文档层，而是继续往代码和验证里推进了两块：
  1. `P0-B`：背包真选槽 / 剧情锁面板
  2. `P0-C`：运行时新种作物 GUID 护栏 + world-state live harness
- 当前新增的有效进展是：
  1. `GameInputManager / ToolbarSlotUI / PlayerInteraction`
     - runtime reassert 已统一改为保留背包偏好槽
  2. `GameInputManager.OnDialogueStart()`
     - 已改成统一关闭 `Package / Box`
  3. `CropController`
     - 现在会在启用和 `PersistentId` 读取时补齐 GUID
     - 新种出来的作物不再因为空 GUID 被 continuity 捕获链直接跳过
  4. 新增：
     - `PersistentWorldStateLiveValidationRunner`
     - `PersistentWorldStateLiveValidationMenu`
     - `WorldStateContinuityContractTests`
- 当前验证状态：
  1. `InventoryRuntimeSelectionContractTests`
  2. `WorkbenchInventoryRefreshContractTests`
  3. `SaveManagerDay1RestoreContractTests`
  4. `WorldStateContinuityContractTests`
  - 当前整体合同测试口径：`14/14 passed`
- 当前 live 取证状态：
  - 已证实 `Primary -> Home` 的切场本体能真正进入 `Home`
  - 但 world-state harness 本身还没稳定穿完整条 `Home / Town / Primary` 连续路径
  - 所以 `任务 9` 仍停在“已有抓手，live 终验待收”
- 详细恢复点已继续回写：
  - [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜只读复盘：Home 箱子上漂与箱子界面背包断联是当前最高优先阻塞
- 用户要求本轮暂停继续施工，先把当前持久化 / 箱子 / 背包真实坏相和前面已做未做内容讲清楚。
- 当前最新只读结论：
  1. `Home` 箱子每次回场越来越高，最高置信根因是 `ChestController.AlignSpriteBottom()` 改了箱子根节点 `transform.localPosition`，而不是只改 Sprite 子节点。
  2. 这会污染箱子真实世界位置，也会污染切场 continuity / save 记录的位置，和用户截图中的箱子上漂高度一致。
  3. 箱子内容回场复活仍不能视为闭环；如果运行态恢复没有命中，`ApplyAuthoringSlotsIfNeeded()` 仍会让空的新箱子重新吃作者预设内容。
  4. 箱子 UI 上半区能用、下半区背包像断联，说明坏点不在箱子整体打开，而在背包侧更长的 `InventorySlotInteraction -> InventoryInteractionManager -> GameInputManager` 输入/门控链。
  5. 当前 `14/14` 合同测试只能证明结构护栏成立，不覆盖 `Home / Town / packaged` 的真实箱子与背包交互体验。
- 下一轮若转施工，不能继续泛做 `0417`，应先收这两个玩家可见硬阻塞：
  1. 修 `ChestController` 根节点对齐污染。
  2. 钉 `Home / Town` 箱子面板下半区背包失联。
- 详细恢复点：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-17 追加索引｜新根因已从“world-state 单点缺口”收窄到“切场真源分叉”
- 用户这轮给出的新复现比“树复活 / 掉落物复活”更底层：
  - 在 `Primary` 丢掉一份物品
  - 进 `Home` 时背包看起来还对
  - 回 `Primary` 时，背包里那份物品又回来了，但地上那份还在
  - 捡起后会临时出现双份，再切场又回掉
- 本轮只读大调查后，当前最高置信判断已经改写为：
  1. 这不是单纯 world-state 没记住
  2. 也不是单纯 UI 显示错了
  3. 是切场时 `Inventory / Hotbar / Input / Package UI` 没统一唯一真源
- 当前钉实的结构事实：
  1. `Primary / Home / Town` 三个主场景都各自内置：
     - `Systems`
     - `InventorySystem`
     - `HotbarSelection`
     - `EquipmentSystem`
     - `UI`
     - `EventSystem`
     - `PersistentManagers`
  2. `PersistentPlayerSceneBridge` 不是场景对象，而是 runtime bootstrap
  3. `Primary` 虽然有 `InventoryBootstrap`，但序列化里 `runOnBuild = 0`，`Home / Town` 也没有它，所以这轮 build/live 回弹不是它直接重灌
- 当前最高嫌疑根因位于：
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
- 更具体的人话结论：
  1. `RebindScene()` 里先把新场景里的同名根标成 duplicate 去 `Destroy()`
  2. 但 `Destroy()` 是帧末才真正销毁
  3. 同一帧后续又立刻去解析并重绑：
     - `InventoryService`
     - `HotbarSelectionService`
     - `GameInputManager`
     - `PackagePanelTabsUI / InventoryPanelUI / ToolbarUI`
  4. 所以系统会出现“一边说这套要删，一边又拿这套去当当前真源”的自打架
- 这条根因还能解释两类旧问题：
  1. `There can be only one active Event System.` 这类 console 错误
  2. `Home / Town` 箱子上半区能动、下半区背包像断联
- 额外高风险点也已确认：
  1. 三个场景里的 `InventoryService` 都带同一个 `_persistentId`
  2. `EquipmentService` 代码也还是固定单例 ID
  3. 加上大量 `FindFirstObjectByType<InventoryService / HotbarSelectionService / PackagePanelTabsUI / EquipmentService>` 回退，整个库存链会在切场窗口里漂移到错误实例
- 这意味着下一刀的方向必须改：
  1. 不能继续只补树 / 掉落物 / 箱子 / 农地 case
  2. 要先收“切场后唯一库存/热栏/输入/UI 真源合同”
  3. 然后再回头做 `Primary -> Home -> Primary`、箱子下半区、world-state continuity 的 live 冒烟

## 2026-04-17 15:18 追加索引｜最小刀口已切完：作物 continuity 代码层补齐，但当前仍未到可打包最小态
- 用户把这一轮硬收成“先做完一个最小刀口，再完整汇报现状”，因此我没有继续扩修箱子/背包/工作台，而是先把作物 continuity 再补一刀并停刀盘点。
- 当前这轮真正新增完成的是：
  1. `CropController.Save()` 改为写格子中心位置
  2. `CropController.Load()` 会从 `seedId` 恢复 `seedData`
  3. `instanceData.seedDataID` 会在回盘时同步补齐
  4. `WorldStateContinuityContractTests` 新增对应合同断言
- 当前 fresh 代码层验证：
  - `CropController.cs`：`0 error / 1 warning`
  - `WorldStateContinuityContractTests.cs`：`0 error / 0 warning`
  - `PersistentWorldStateLiveValidationRunner.cs`：`0 error / 0 warning`
  - scoped `git diff --check`：通过
- 当前 fresh 现场验证最重要的新信号不是 compile red，而是 console 里稳定存在：
  - `There can be only one active Event System.`
  - `There are 2 event systems in the scene...`
- 所以当前最准确的人话判断已经变成：
  - 最小刀口做完了
  - 但 world-state 主链还没闭环
  - 打包最小态还没到
  - 现在最大头已经收窄成 `Primary <-> Home` 往返时的 duplicate runtime root / EventSystem / inventory rebind 冲突
- 当前这轮不再建议继续漂去：
  - Save UI 美化
  - F5/F9 文案
  - 默认档文案细修
  - Day1 staging own 问题
  - 泛 packaged 外发体系
- 下一刀若继续，优先级应固定为：
  1. 钉第二个 `EventSystem` 从哪里来
  2. 钉 `PlayerInventory` / persistent inventory root 是否也在重复生成
  3. 再回 `Home / Town` 箱子背包断联与 workbench 识别链

## 2026-04-17 16:05 追加索引｜最小打包刀已进代码：删除态合同、箱子底边对齐、读档/重开刷新闭环
- 用户把目标重新收得更硬：不是追完美，而是把打包前最必要的 bug 修到能稳住。
- 本轮因此没有再追 Save UI / 文案，而是只补最关键的 3 块：
  1. `树 / 石头 / 箱子 / 掉落物 / 作物` 的跨场景删除态不复活
  2. 箱子开关 sprite 底边对齐 + Collider 跟随，不再挤玩家
  3. 读档 / 重新开始后的 `背包 / toolbar / 选中态` 全量刷新
- 当前真正落地的代码点：
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
  - [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
  - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
  - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
- 当前已站住的新结论：
  1. off-scene world-state 不再只会处理“还活着的对象”
  2. 空快照也会作为权威删除态保存
  3. 树复活这类问题的共同根因，已经被真正收进主链语义，而不是继续靠 case-by-case 打补丁
  4. 读档 / 重开后背包和 toolbar 的刷新链也不再只刷第一个可见 UI
- 当前验证状态：
  - `validate_script`：4 个文件均 `0 error`
  - EditMode：`11 / 11 passed`
  - fresh console：`0 error / 0 warning`
- 当前仍未闭环的部分：
  - live 玩家路径
  - packaged smoke
- 也就是说：
  - 结构层这轮明显前进了
  - 但是否真的到“可打包最小态”，还要看下一刀 live 冒烟

## 2026-04-17 只读追加｜Primary world-state 复活/丢失的最小根因复核
- 用户要求本轮只读，不改代码，不写 0417，只回答：为什么 `Primary` 中砍树、耕地、浇水、种作物后，切到 `Home/Town` 再回 `Primary` 仍可能复活或丢失。
- 已复核链路：
  - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
  - [TreeController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/TreeController.cs)
  - [FarmTileManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmTileManager.cs)
  - [CropController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/CropController.cs)
  - [DynamicObjectFactory.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs)
  - [SceneTransitionTrigger2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs)
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
- 静态结论：
  1. `SceneTransitionTrigger2D.TryStartTransition()` 本身会先调用 `PersistentPlayerSceneBridge.QueueSceneEntry()` 再进 `SceneTransitionRunner.Begin()`，普通门链从代码上看是正确入口。
  2. 项目中仍存在绕过该合同的直载入口，尤其 `SpringDay1Director` 内多处 `SceneManager.LoadScene(..., LoadSceneMode.Single)`；这些路径不会自动做离场 world-state 捕获。
  3. 树被彻底 `Destroy` 后，当前 bridge 主要靠“回场快照里缺席 = 删除态”推断，恢复是否可靠取决于：离场捕获确实发生、回场恢复确实发生、场景原生树 GUID 稳定。
  4. `FarmTileManager.Save/Load` 和 `CropController.Save/Load` 当前结构是“耕地由 manager 保存，作物由 runtime controller 单独保存”，桥接恢复顺序已经把 `FarmTileManager` 放在 `Crop` 前面；如果耕地/浇水/作物一起丢，更像是整次离场快照没抓到或没恢复，而不是单个 Save 字段缺失。
- 下一轮若施工，推荐先动最小文件/方法：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的直接 `SceneManager.LoadScene` 调用点：统一走 bridge 捕获入口。
  2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 的 `CaptureSceneWorldRuntimeState / RestoreSceneWorldRuntimeState / RemoveUnexpectedSceneWorldBindings`：补显式删除态或至少把删除态恢复合同钉实。
  3. [TreeController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/TreeController.cs) 的砍倒/销毁路径与 `ShouldSave/Save`：不要只让“对象消失”承担删除态语义。

## 2026-04-17 追加｜最小打包收尾刀第二簇：切场唯一真源与直载入口收口
- 用户把这一轮明确收成“最小、最安全、一轮内砍到可直接打包”的完整收尾刀，因此这次没有再漂去 UI、美化、文案，而是继续补最容易把 demo 打炸的 runtime 真源与切场入口。
- 本轮实际补的代码点：
  1. `PersistentPlayerSceneBridge.CaptureSceneRuntimeState()` 改为通过 `ResolveRuntimeInventoryService(scene)` / `ResolveRuntimeHotbarSelectionService(scene)` 抓离场快照，不再误抓 scene-local duplicate。
  2. `SaveManager.RestoreInventoryData()` 的旧存档兼容回写改为优先 `PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()`。
  3. 下列底层运行时消费者全部改为优先 bridge 真源：
     - `CraftingService`
     - `HotbarSelectionService`
     - `InventorySortService`
     - `AutoPickupService`
     - `PlacementManager`
     - `PlayerInteraction`
  4. `SpringDay1Director` 的 3 处直切 `Town/Home` 改为统一走 `LoadSceneThroughPersistentBridge()`，先 `QueueSceneEntry()` 再 `LoadScene(...)`。
  5. 旧 `DoorTrigger` 也补了 `QueueSceneEntry()`，避免普通门链仍有漏网直载。
  6. `WorldStateContinuityContractTests` 增补到 17 条，锁住 resolved runtime 快照、直切场入口和底层消费者真源合同。
- 当前验证：
  - `manage_script validate`：本轮涉及脚本均 `0 error`，仅剩旧 warning
  - EditMode：`WorldStateContinuityContractTests + WorkbenchInventoryRefreshContractTests = 17/17 passed`
  - fresh console：`errors=0 warnings=0`
- 当前判断：
  1. 这轮已经真正收掉了“切场后抓错背包/热栏事实源，导致 Home 看着对、回 Primary 又回弹”的主根因。
  2. 也封住了剧情/旧门链绕过 bridge 的直载入口，这条之前会直接导致树/农地/作物离场态没入盘。
  3. 剩下的闭环不再是代码层大头，而是 live / packaged 冒烟。

## 2026-04-17 追加｜箱子摆位问题从“底边统一”纠偏到“场景 authored pose 为真”
- 用户最新纠正非常关键：
  - 这不是单纯的“Sprite 底边没对齐”问题。
  - 真正要守的是：场景里摆好的箱子视觉位置就是正式面，运行时不能再按数学公式重算一个默认位置。
- 本轮新结论：
  1. 之前止住“每次进场越飘越高”的逻辑还不够，因为它虽然不再移动 root transform，但仍会把 visual child 按 `-spriteBottomOffset` 重新摆位。
  2. 这会导致：
     - 编辑器场景里看着对
     - 一进 Play 或打包运行就整体挪位
  3. 所以正确语义必须改成：
     - 先捕获 authored `closed` pose / authored 底边基线
     - root SpriteRenderer 迁到 `__ChestSpriteVisual` 时，child 要先复位到 authored pose
     - open / close 后续只相对这个 authored 基线变化
- 本轮改动：
  1. `ChestController.Awake()` 先 `CaptureAuthoredVisualBaseline(_spriteRenderer)` 再 `EnsureSpriteRendererUsesVisualChild()`
  2. 新增 authored 基线字段与 `ApplyAuthoredVisualPoseToRenderer()`
  3. `AlignSpriteBottom()` 从 `localPos.y = -spriteBottomOffset` 改成 `localPos.y = _authoredBottomLocalY - spriteBottomOffset`
  4. `WorldStateContinuityContractTests` 已同步改成锁 authored baseline 合同，而不再锁旧的裸贴底公式
- 本轮验证：
  1. `ChestController.cs` native validate `0 error`，CLI assessment=`unity_validation_pending`（Unity 编译轮询超时，不是 owned red）
  2. `WorldStateContinuityContractTests.cs` native validate `clean`，CLI assessment=`unity_validation_pending`
  3. fresh console：`0 error / 0 warning`
  4. EditMode：`WorldStateContinuityContractTests.ChestController_ShouldNotMoveRootTransformWhenAligningSpriteBottom` 通过，`1/1 passed`
- 当前状态：
  1. 代码层已经把“箱子默认位置和场景摆位不一致”的最高置信根因修掉
  2. 还没有做 Home 箱子的 live / packaged 视觉复验，所以不能宣称最终体验已过线

## 2026-04-17 追加｜玩家背包收口：sort 改整包、垃圾桶改真删除、管理器操作前重绑 runtime 背包
- 用户最新裁定这轮不要再动箱子，主刀改成玩家背包：
  - 背包 `sort` 语义不对
  - `垃圾桶` 也要检查
  - 跨场景后背包问题会被放大
- 本轮代码改动：
  1. [InventorySortService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs)
     - `SortStartIndex` 从 `InventoryService.HotbarWidth` 改成 `0`
     - `ResolveRuntimeContext()` 每次优先吃 `PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()`
     - 数据库跟随当前 runtime inventory 更新
  2. [InventoryService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Inventory/InventoryService.cs)
     - `Sort()` fallback 改成与 `InventorySortService` 同语义：
       - 整包收集
       - 普通物品先合并
       - 再按统一优先级排序
       - 再逐槽写回 `SetInventoryItem`
  3. [InventoryInteractionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs)
     - 新增 `ResolveRuntimeContext()`
     - `GetSlot / GetRuntimeItem / SetSlot / ClearSlot / ReturnHeldItemToInventory / OnSortButtonClick` 都会先对准 bridge 当前真源
     - 新增 `DiscardHeldItem()`
     - 垃圾桶点击、拖到垃圾桶、held 状态点垃圾桶都不再掉地，而是真删除
  4. [InventorySlotInteraction.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventorySlotInteraction.cs)
     - `SlotDragContext` 拖到垃圾桶时改走 `DiscardItemFromContext()`
     - 面板外仍保持掉地逻辑，和垃圾桶删除语义分开
  5. [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
     - `ResolveRuntimeContextIfMissing()` 虽然名字没改，但现在不再只是“字段为 null 才补”
     - 会优先切回当前 runtime inventory / equipment / selection
- 定向合同测试：
  - [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)
  - 已新增：
    1. `InventorySort_ShouldUseWholeBackpackAndRuntimeAuthoritativeContext`
    2. `InventoryTrash_ShouldDiscardHeldItemInsteadOfDroppingAtPlayerFeet`
    3. `SceneRebind_ShouldRefreshInputAndSortRuntimeContext` 补强 inventory panel runtime 真源断言
- 当前验证：
  1. Unity `validate_script`
     - `InventorySortService / InventoryService / InventoryPanelUI / InventoryRuntimeSelectionContractTests` = `0 error / 0 warning`
     - `InventoryInteractionManager / InventorySlotInteraction` = `0 error / 1 warning`（既有字符串拼接 GC 提示）
  2. fresh console：
     - `errors=0 warnings=0`
  3. EditMode：
     - 上述 3 条定向合同 `3/3 passed`
- 当前判断：
  1. 背包 sort 这条线已经不再和箱子 sort 使用两套明显冲突的语义
  2. 垃圾桶现在回到了真正“删除”的玩家预期，不再把删除伪装成掉落物
  3. 还没做的只剩 live / packaged 复测：
     - `Primary/Home/Town` 的背包 sort
     - 垃圾桶真删除
     - sort 后 toolbar 与实际使用链是否仍一致

## 2026-04-17 21:05 箱子视觉/碰撞与 Home 切场强刷只读诊断

### 用户新增反馈
- 用户明确指出两件事：
  1. 箱子在编辑器里怎么放，运行就应该怎么看；现在视觉和碰撞都不对
  2. 进入 `Home` 后背包坏相会缓解，说明“切场默认刷新”是对的方向，`Town <-> Primary` 也必须学这一套

### 我这轮确认到的真问题
- 箱子现在不是单纯“底边公式差一点”，而是 `ChestController` 还在运行时偷偷换承载层：
  - 先拿当前 `_spriteRenderer` 抓 authored baseline
  - 再创建/接管 `__ChestSpriteVisual`
  - 再用 child 的 local pose 驱动视觉和碰撞
- 这意味着运行时结果不再完全等于场景作者摆好的正式面。
- 碰撞体漂移也不是独立问题：
  - root 上的 `PolygonCollider2D` 会跟随 visual child 的 local offset 重建路径
  - 视觉参考抓错，碰撞就一起错

### Home 与普通切场的差别
- `BoxPanelUI` 打开/刷新时会执行比普通切场更重的一套 runtime context 强刷：
  - `RefreshRuntimeContextFromScene()`
  - 逐格 `Bind + Refresh + RefreshSelection`
- 这可以解释为什么用户观察到“进 Home 后某些槽位会恢复”。
- 当前普通 `PersistentPlayerSceneBridge.RebindScene()` 虽然已经有 runtime rebind，但还没把这种“逐格强刷合同”完全推广到所有普通切场后的背包显示链。

### 当前判断
- 下一刀的正确方向已经重新收紧为两件事：
  1. 箱子必须回到“场景里看到什么，运行就是什么”
  2. 普通切场必须吃到和 `Home/BoxPanelUI` 同等级别的强制刷新，不再让 `4/8` 这类显示链靠运气恢复

### thread-state 报实
- 本轮性质：
  - 只读分析
- 未跑：
  - `Begin-Slice / Ready-To-Sync / Park-Slice`
- 当前 live 状态：
  - 保持 `PARKED`

## 2026-04-17 23:48 authored 箱子真值修正 + 普通切场统一强刷新补记

### 用户目标
- 用户要求本轮把两件事直接落地：
  1. 箱子运行态必须回到“编辑器里怎么摆，运行就该怎么看”
  2. `Home` 能把背包/toolbar 拉正的那套刷新，要推广成普通切场统一合同
- 同时要求我以存档线程视角复核 `farm` 的只读结论，但本轮不要越权乱改农田业务。

### 本轮完成
1. [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs)
   - `CaptureAuthoredVisualBaseline()` 新增 root-renderer 分支
   - 如果 authored sprite 原本就在 root，运行时迁到 `__ChestSpriteVisual` 时改用 root-local 真值：
     - `Vector3.zero / Quaternion.identity / Vector3.one`
   - `UpdateColliderShape()` 不再只加平移 offset
   - 已改成 `TransformSpritePhysicsPointToChestLocal(...)`，让 collider 跟完整视觉局部变换走
2. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - `InventoryPanelUI` 改成显式 `ConfigureRuntimeContext(...)`
   - `InventoryInteractionManager` 改成显式 `ConfigureRuntimeContext(...)`
   - 保留并继续走这轮前面已落的统一强刷：
     - `sceneInventory.RefreshAll()`
     - `sceneHotbarSelection.ReassertCurrentSelection(...)`
     - `activeBoxPanel.ConfigureRuntimeContext(...)`
     - `activeBoxPanel.RefreshUI()`
3. 合同测试同步补强：
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)
   - [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)

### farm 复核结论
1. `farm` 线程判断成立：主根因确实是 `Primary` 离场后农田/作物没有补跨天结算。
2. 正确合同应拆成：
   - bridge / save contract 负责提供离场到回场的 `elapsed days`
   - farm 负责消费天数做浇水迁移、缺水、成熟/枯萎、空地到期补算
3. 本轮没有动 `FarmTileManager / CropController`
   - 因为当前 off-scene snapshot 还没有权威天数锚点
   - 且 `FarmTileManager -> Crop` 恢复顺序存在误删风险

### 验证
1. `validate_script`
   - `ChestController.cs`：`owned_errors=0`，assessment=`unity_validation_pending`
   - `PersistentPlayerSceneBridge.cs`：`owned_errors=0`，assessment=`unity_validation_pending`
   - `WorldStateContinuityContractTests.cs + InventoryRuntimeSelectionContractTests.cs`：`owned_errors=0`，assessment=`unity_validation_pending`
   - 这几次 `pending` 的共同 blocker 是 Unity `stale_status`，不是本轮 own red
2. fresh console：
   - `errors=0 warnings=0`
3. scoped `git diff --check`
   - 本轮 4 个 owned 代码/测试文件通过

### 当前判断
1. 本轮用户要求的两件主刀都已落到代码层。
2. `farm` 这条也已完成存档线程 own 视角的安全复核，当前正确动作是停在合同裁定，不越权乱修。
3. 下一步最应该单开的，是 `farm off-scene catch-up contract`，不是再回头猜箱子偏移或继续补 `Home` 特例。

### thread-state 报实
1. 本轮已跑：
   - `Begin-Slice`
   - `Park-Slice`
2. 本轮未跑：
   - `Ready-To-Sync`
3. 当前 live 状态：
   - `PARKED`

## 2026-04-18 00:12 Home 背包 4/8 槽位异常补记

### 用户新反馈
- 用户实测确认：
  - 箱子前一刀基本好了
  - 但进入 `Home` 后，背包 `4/8` 槽仍然会出问题

### 本轮修正
1. [InventoryPanelUI.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs)
   - `ConfigureRuntimeContext(...)` 现在先 `InvalidateSlotBindings()`
   - 强制让 36 格背包在切场 runtime context 更新后重新 `Bind`
2. `RefreshAll()` 现在会先 `EnsureBuilt()`
   - 防止“数据刷新了，但槽位还是旧绑定”这种半刷新状态
3. [InventoryRuntimeSelectionContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs)
   - 补断言：runtime context 更新后必须强制失效旧绑定，再 `EnsureBuilt`

### 验证
1. `manage_script validate InventoryPanelUI`
   - `status=clean`
   - `errors=0 warnings=0`
2. `validate_script InventoryRuntimeSelectionContractTests.cs`
   - `owned_errors=0`
   - `assessment=unity_validation_pending`
   - blocker 仍是 Unity `stale_status`
3. fresh console：
   - `errors=0 warnings=0`
4. scoped `git diff --check`
   - 通过

### 当前判断
1. 这刀针对的是背包页自己“缓存槽位绑定”的缺口。
2. 现在更像是：bridge 主链已经把服务重绑过去了，但 `InventoryPanelUI` 因为看到引用对象没换，之前会误判成“不用重建格子”。

## 2026-04-17 22:53 树跨场景跨天补票 + Primary 遮挡运行时重绑

### 本轮用户反馈
1. 离场场景里的树不会随跨天一起成长。
2. `Primary` 里玩家被树/石/房屋挡住时不透明，但 `Town` 正常。
3. 用户补充口径：
   - `farm` 目前看起来又对了
   - 这轮不要偏回 UI 漫修，而是继续收打包前最小必修链

### 本轮完成
1. off-scene world snapshot 新增离场天数合同：
   - [SaveDataDTOs.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
   - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
2. 树回场补天数：
   - [TreeController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/TreeController.cs)
   - 当前只收树，不扩成所有 world-state 的统一离场跨天模拟
3. 场景遮挡重绑：
   - [OcclusionManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs)
   - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs)
   - 现在切场后会显式把 `OcclusionManager` 对准 persistent player，并在运行中自愈失效玩家引用
4. 合同测试补强：
   - [WorldStateContinuityContractTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs)

### 只读子线程裁定
1. `Fermat` 的 scene-only 回执有参考价值：
   - 它确认 `playerLayer` 的一次性缓存是嫌疑之一
   - 也提示 `Primary.unity` 后续仍值得继续看实例覆写
2. 但本轮没有直接去改 `Primary.unity`
   - 因为当前更安全的最小修复已经能落在脚本层
   - 如果回测仍失败，再按 scene-audit 口径收 scene authoring 面

### 验证
1. `validate_script`：
   - touched runtime scripts `owned_errors=0`
   - assessment=`unity_validation_pending`
   - blocker 是 Unity `stale_status`
2. `manage_script validate`：
   - `TreeController / OcclusionManager / PersistentPlayerSceneBridge`
   - 都是 `0 error`
3. fresh console：
   - `errors=0 warnings=0`
4. `git diff --check`：
   - 当前 owned 文件通过

### 当前判断
1. “离场树不会长”这条已经不是只读结论，而是正式进了 save/load/scene continuity 主链。
2. `Primary` 遮挡这条，这轮先收的是 runtime binding 和 player-layer 自愈，不是直接改 production scene。
3. `farm` 当前体感恢复不是我这轮直接修的，不报假功。

## 2026-04-17 23:02 完成度审计补记

### 用户问题
1. 用户要求我用人话重新盘清：
   - 历史还有什么没完成
   - `0417` 以及之前提过的主线内容现在到底完成到什么程度

### 当前统一判断
1. 如果按“代码结构收口 / live 实机验证 / packaged 验证”三层看：
   - 结构层大头已收住
   - live 层只完成一部分
   - packaged 层还没有真正收尾
2. 当前不能把整条线说成“全部完成”或“可放心打包后无问题”。
3. 当前最适合先放下、不作为打包第一阻塞的内容：
   - Save UI 继续美化
   - `F5/F9` 文案细修
   - Day1 own staging / 站位 / 美术层微调
4. 当前仍算硬尾项的内容：
   - `Primary/Home/Town` 的真实 live 回归没有跑透
   - `packaged smoke` 未完成
   - Day1 恢复 contract 仍缺最终端到端证明

### 对 0417 的结论
1. `0417` 不是空转，但也绝对没到“全部完成”。
2. 它现在更准确的状态是：
   - 代码层主问题树和大部分关键补口已经进文档并落到实现
   - 最后的 live / packaged 验证板还没收平

## 2026-04-18 00:26 打包前最小清单只读总审

### 用户目标
1. 用户要求不要再靠 `0417` 或历史记忆复述。
2. 这轮必须重新回看关键代码后，用人话总结：
   - 现在到底已经做成了什么
   - 还有什么没做完
   - 打包前最小且最重要的清单是什么
   - 每项具体应该怎么做

### 这轮重新核过的关键代码
1. 存档与默认档：
   - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
   - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
2. 切场 continuity / world-state：
   - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - `Assets/YYY_Scripts/Controller/TreeController.cs`
   - `Assets/YYY_Scripts/Controller/StoneController.cs`
3. 背包 / toolbar / 箱子：
   - `Assets/YYY_Scripts/UI/Inventory/InventoryPanelUI.cs`
   - `Assets/YYY_Scripts/UI/Inventory/InventoryInteractionManager.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventorySortService.cs`
   - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
   - `Assets/YYY_Scripts/UI/Box/BoxPanelUI.cs`
   - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
4. 农田 / 作物 / Day1：
   - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
   - `Assets/YYY_Scripts/Farm/CropController.cs`
   - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
5. 代码合同测试：
   - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
   - `Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs`

### 当前代码层统一判断
1. 已经真正落地的硬补口：
   - 默认档语义已是 `Town` 原生开局，`F5` 停用，`F9` 只读默认开局。
   - 读档 / 重开后会强刷背包、toolbar、选中态和画布。
   - 切场 world-state 已覆盖：耕地、作物、箱子、掉落物、树、石头。
   - 快照缺失即删除态已生效，树/石头/箱子/掉落物/作物不该再因“快照没写”而回场复活。
   - 树离场跨天成长合同已进正式链路。
   - 箱子 authored pose / 视觉子节点 / collider 跟随同一基线的代码已经在。
   - 背包 sort 已改成整包语义；垃圾桶已是真删除语义。
   - Day1 那个“按 phase 自动把剧情补消费”的直接写回逻辑已经不在 `SpringDay1Director.HasCompletedDialogueSequence()` 里了。
2. 还不能报“彻底做完”的硬原因：
   - 农田 / 作物离场跨天补票，代码里还没有像树那样进入 `ApplySceneWorldCatchUp(...)`。
   - 石头没有离场跨天结算需求，但它也没有额外 catch-up 入口；当前更像“删除态 continuity 已收，成长态不相关”。
   - Day1 虽然去掉了最明显的 completed 回写冲突，但整条 load / restart / restore 的端到端回归没有重新跑透。
   - Save UI 功能按钮链已经齐，但体验层和最终观感不能报终验。
   - packaged smoke 仍未完成，这意味着“代码看起来对”和“打包后确实对”之间还差最后一层确认。

### 对打包阻塞的判断
1. 现在最像真正挡打包的，不是 Save UI 漂不漂亮。
2. 真正挡打包的是：
   - 三场景来回时，背包 / 掉落物 / world-state 是否还会串档、回弹、复制。
   - `Primary` 的树、农地、作物、掉落物是否在离场后按正确规则继续存在或结算。
   - 重新开始 / 读档后，背包、toolbar、工作台、Day1 恢复是否全部回到同一真实状态。
   - 打包后是否仍按同样结果运行。
3. 目前可以先不作为第一阻塞的：
   - Save UI 继续美化
   - 快捷说明文案微调
   - Day1 自己那边的 staging / 站位 / 视觉尾账

### 恢复点
1. 如果下一轮继续，不该再回到大而散的“0417 全清”口径。
2. 正确顺序应改成：
   - 先收“会影响打包正确性”的 live / packaged 硬尾项
   - 再看 Save UI 体验尾账

## 2026-04-18 默认槽/F5/F9/重新开始语义只读彻查

### 用户目标
1. 用户要求我不要再凭印象解释，而是直接回看代码，彻底说清：
   - `默认开局` / `停用` 现在到底是什么意思
   - `加载默认开局` / `重新开始` 现在到底是不是同一个东西
   - 为什么默认槽没按“默认存档 + F5 快速保存 + F9 快速读取”工作
   - 默认槽左侧摘要、按钮和文案为什么整体都偏了
2. 这轮只做只读分析和整改方案，不进真实施工。

### 这轮重新核过的代码
1. [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
2. [PackageSaveSettingsPanel.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs)
3. [SaveDataDTOs.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
4. 历史语义来源：[memory_0.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/memory_0.md)

### 当前代码真相
1. 默认槽当前被硬编码成“原生开局入口”，不是正式的默认存档：
   - `DefaultProgressDisplayName = "默认开局"`
   - `TryGetDefaultSlotSummary()` 固定走 `CreateNativeFreshDefaultSummary()`
   - `CreateNativeFreshDefaultSummary()` 直接手造 `Town / Day1 / 09:00 / EnterVillage` 摘要
2. `F5` 当前不是快速保存，而是明确被停用：
   - `ExecuteQuickSaveHotkey()` 只弹 `F5 已停用...`
3. `F9` 当前不是读取默认存档文件，而是直接走“回到 Town 原生开局”：
   - `ExecuteQuickLoadHotkey()` -> `QuickLoadDefaultSlot(...)`
   - `QuickLoadDefaultSlot(...)` -> `BeginNativeFreshRestart(...)`
4. `加载默认开局` 和 `重新开始` 当前底层完全同源：
   - `QuickLoadDefaultSlot(...)` 和 `RestartToFreshGame(...)` 都只调用 `BeginNativeFreshRestart(...)`
5. 默认槽当前不能读真实文件、不能写真实文件：
   - `TryReadSaveData()` 遇到默认槽直接 `return false`
   - `QuickSaveDefaultSlot()` 直接拒绝写入
   - `SaveExists(default)` 被硬写成永远 `true`
6. 默认槽 UI 只是把这套错误语义忠实显示出来：
   - 标题是“默认开局”
   - 备注是“只读入口”
   - 快捷说明是 `F9 默认开局 / F5 停用`
   - 默认区按钮是“加载默认开局 + 重新开始”
   - 默认区复制/粘贴/覆盖按钮被直接隐藏
7. 默认槽左侧信息和普通槽不一样，不是偶发现象，而是代码故意分了两套摘要模板：
   - 普通槽走 `CompactSummary(summary)`
   - 默认槽走 `DefaultSummary(summary)`
   - 后者只显示“固定入口 / 场景 / 剧情”，故意省掉背包、生命、精力等普通参数
8. 默认区描述贴得太上，也是布局代码决定的：
   - 默认摘要卡 `SummaryPanel` 上方还有 `DefaultTap`
   - 文本 `_defaultSummaryText` 是 `UpperLeft`
   - 顶部 padding/spacings 又偏紧，所以视觉上更像“贴着标题”

### 这套语义为什么会变成这样
1. 不是随机写坏的，而是 2026-04-07 那轮为了先恢复 `Primary` 调试安全，主动把默认档降级成“原生开局入口”。
2. 那轮的临时裁定在历史记忆里写得很明确：
   - 默认槽只负责回原生开局
   - F5 停用
   - 默认槽不再落盘
3. 问题不在当时的应急止血本身，而在于这套临时语义后来没有重新切回你现在要的正式产品语义。

### 当前最应该改成的正式语义
1. 默认槽名称必须回到：`默认存档`
2. `F5` 必须回到：快速保存到默认存档
3. `F9` 必须回到：快速读取默认存档
4. 默认槽必须是一个真实落盘槽位，而不是假的“fresh-start 入口卡片”
5. 默认槽允许：
   - 加载
   - 复制
   - 被 `F5` 快速保存
   - 被 `F9` 快速读取
6. 默认槽禁止：
   - 粘贴覆盖
   - 手动“覆盖当前存档”
   - 删除
7. `重新开始` 必须独立出去，语义固定为：
   - 丢弃当前未保存进度
   - 回到 Town 原生起点
   - 不等于读默认槽
   - 不自动改写默认槽文件
8. 默认槽左侧摘要必须和普通槽同字段、同顺序、同模板，不再单独搞“默认开局摘要”

### 最小且完整的改造方案
1. `SaveManager.cs`
   - 把默认槽从“伪保留槽”改回“真实受保护槽”
   - `TryReadSaveData()` 允许读默认槽文件
   - `QuickSaveDefaultSlot()` 改成真实写默认槽
   - `QuickLoadDefaultSlot()` 改成真实读默认槽
   - `RestartToFreshGame()` 保留原生重开职责，但不再和 quick-load 共用语义
   - `TryGetDefaultSlotSummary()` 改为优先从默认槽文件建摘要；若文件不存在，则给“空默认槽”摘要，而不是伪造 Town 开局摘要
   - 把“Legacy baseline 保留槽”和“默认存档受保护规则”拆成两类 helper，避免继续用一个 `IsReservedSlot()` 把读写/删除/粘贴全混死
2. `PackageSaveSettingsPanel.cs`
   - 默认区标题改为“默认存档”
   - 删除“只读入口”
   - 快捷说明改为 `F5 快速保存默认存档 / F9 快速读取默认存档`
   - 默认槽左侧改成复用普通槽摘要模板，不再走 `DefaultSummary()`
   - 默认区只保留：加载、复制
   - `重新开始游戏` 从默认区移走，改放到顶部独立危险操作区
   - 默认区描述文本下移，优先用统一摘要卡和更稳的 padding/spacing 解决，不再继续叠专用小标题
3. UI 交互矩阵
   - 默认槽：加载 / 复制 / F5 保存 / F9 读取
   - 普通槽：加载 / 复制 / 粘贴 / 覆盖 / 删除
   - 独立危险操作：重新开始游戏
4. 空槽与首启策略
   - 默认槽如果还没有内容，UI 明确显示“默认存档尚未保存”
   - `F9` 在空默认槽上提示“请先按 F5 快速保存”
   - 真正的新游戏入口只走“重新开始游戏”
5. 验证
   - 代码合同测试至少补：默认槽 quick-save / quick-load、默认槽禁粘贴删除、restart 不再等于 quick-load
   - live 再测：F5、F9、默认槽复制、普通槽粘贴覆盖、重新开始

### 当前恢复点
1. 这轮结论已经足够支撑下一轮直接施工。
2. 下一轮如果开改，不应该再先修文案，而应先改 `SaveManager` 的默认槽能力矩阵，再改 `PackageSaveSettingsPanel` 展示和按钮布局。

## 2026-04-18 追加索引｜world-state live runner 最新静默卡住更像 runner 自身交接窗漏洞
- 用户这轮把问题重新收窄成：
  - `PersistentWorldStateLiveValidationRunner` 为什么在 `bootstrap passed` 之后静默停住
  - 既没有继续 `scenario_start`
  - 也没有 watchdog report
  - 也没有写 `world-state-live-validation.json`
- 本轮做的是只读静态审查，不改代码。
- 当前最重要的新判断不是“哪条业务 continuity 又坏了”，而是：
  1. 这次最像先卡在 runner 自己的 `post-bootstrap -> first scenario` 交接窗口
  2. `StartRun()` 当前是先起 `RunValidation()`，后起 `WatchdogRun()`
  3. 如果前一个 coroutine 在把控制权还给 `StartRun()` 前就同步卡进 scenario 前置准备区，watchdog 根本还没启动
  4. 所以才会同时出现：
     - 只有 `bootstrap passed`
     - 没有 `scenario_start`
     - 没有 watchdog 兜底
     - 没有 report
- 当前对业务链的次级排序也已收窄：
  1. 如果真有同步触发源，更像 `TryPreparePrimaryState()` 里的 `CreateTile / SetWatered`
  2. `CropController.Initialize` 次之
  3. `ChestController.SetSlot` 再次
  4. 这次不像已经走到 `QueueSceneEntry / LoadScene`
- 下一轮若进入真实施工，最小安全顺序应改成：
  1. 先修 runner 的 watchdog 启动时机与 disable/destroy 兜底 report
  2. 再在 `TryPreparePrimaryState()` 四个同步子步骤前后补进度点
  3. 不要第一刀就扩改 bridge 场景切换链
- 详细恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)

## 2026-04-18 追加索引｜默认存档/F5/F9/重新开始 新语义已正式同步进 0417 前板
- 用户最新补充不是换题，而是要求把另一个子线程对默认槽的语义审计结果真正吃回 `0417`，不要只留在聊天和尾部追加记录里。
- 本轮已完成的有效动作不是继续改代码，而是把当前真实施工结果同步回主控板：
  1. `默认槽 = 真实受保护默认存档`
  2. `F5 = 快速保存默认存档`
  3. `F9 = 快速读取默认存档`
  4. `重新开始 = 独立危险操作，不改写默认存档`
- 当前 `0417.md` 的前板、测试矩阵和任务 `12.x / R8-A~F` 已和真实代码现状对齐，不再继续把：
  - `默认开局`
  - `F5 已停用`
  - `F9 默认档读取`
  当成当前产品语义。
- fresh 验证也已补一轮：
  - `SaveManager.cs`：`assessment=no_red`
  - `PackageSaveSettingsPanel.cs`：`assessment=no_red`
  - `SaveManagerDefaultSlotContractTests.cs`：`assessment=no_red`
  - `errors --count 20`：`errors=0 warnings=0`
- 当前恢复点更新为：
  - `默认存档` 这条线已经进入“代码层已过、live / packaged 待测”
  - 后续若继续收 `R8`，优先做真实回归，不再回头争论旧语义

## 2026-04-18 追加索引｜restart 后 toolbar 固定槽位 4/8 异常已收敛到“旧态先重绑、fresh 后清空”
- 本轮用户把问题限定成：
  - `重新开始游戏` 后 toolbar 固定槽位 `4/8` 显示异常
  - NPC 对话后或进出 `Home` 又恢复
  - 只要根因链、恢复原因和最小安全修法
- 当前最高置信新判断：
  1. 代码里没有 `4/8` 特判；`ToolbarUI` 也已不再走旧的名字推断索引链
  2. 真正可钉死的是 restart 顺序：
     - `LoadSceneAsync` 触发 bridge `RebindScene()`，先把旧 inventory / hotbar snapshot 重新绑回 toolbar
     - 然后 `ApplyNativeFreshRuntimeDefaults()` 才清 persistent inventory / hotbar 到 fresh 状态
     - 最后只补一轮 `RefreshAllUI()`
  3. 这让 restart 独有地暴露出 toolbar 侧“单次刷新不够硬”的脆弱点
  4. `Home` 往返之所以能恢复，是因为它会再走一次更强的 `PersistentPlayerSceneBridge.RebindPersistentCoreUi() + ForceRuntimeUiRefreshAfterSceneRebind()`
  5. NPC 对话之所以也能恢复，高概率是因为 dialogue lock 会统一关闭 modal UI、重整输入/placement 状态，并再做一次 hotbar selection reassert
- 最小安全修法建议已收窄为：
  1. 优先调整 `SaveManager.RestartToFreshGame` 这条 fresh reset / UI rebind 顺序
  2. 次优先补硬 `ToolbarUI` 的重订阅合同，避免 disable/enable 后同引用 early-return 吃掉重新订阅
- 详细证据已落到：
  - `4.0.0_三场景持久化与门链收口/memory.md`

## 2026-04-18 只读审查｜0417 world-state / farm / Day1 尾项真实代码缺口复核
- 用户目标：围绕 `0417.md` 里仍标未闭环的 `world-state / farm / Day1` 尾项，只读核实“真正还缺哪些代码”，并区分“代码没做”与“只是 live / packaged 待测”。
- 本轮命中文件：
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
  - `Assets/YYY_Scripts/Farm/CropController.cs`
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
  - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
- 静态结论：
  1. `world-state` 主链里，`SaveManager + PersistentPlayerSceneBridge` 的正式合同已经落到代码：
     - 当前 scene 走 `worldObjects`
     - 离场 scene 走 `offSceneWorldSnapshots`
     - 读档切场前会 `SuppressSceneWorldRestoreForScene(targetSceneName)`
     - 导入离场 snapshots 后再让当前 scene 继续恢复
  2. `farm` 线程在 `0417`/memory 里被标成“还缺 off-scene catch-up”的那条，源码已比文档更前：
     - `PersistentPlayerSceneBridge.FinalizeDeferredSceneWorldCatchUp(...)` 现在会先跑 `CropController.ApplyOffSceneElapsedDays(...)`
     - 再跑 `FarmTileManager.ApplyOffSceneElapsedDays(...)`
     - DTO 侧也已有 `capturedTotalDays`
     - 所以“离场跨天结算完全没接”这个判断已经过时
  3. `Day1` 恢复链在本轮抽检文件里没看到新的高置信业务断链：
     - `SaveManager.ResetTransientRuntimeForRestore(...)` 已收 stale UI / pause / drag / overlay
     - `StoryProgressPersistenceService` 已保存并回放 `storyPhase / completedDialogueSequenceIds / springDay1 关键目标进度 / health / energy / resident runtime`
     - `SpringDay1Director` 里直切 `Town/Home` 已统一改走 `LoadSceneThroughPersistentBridge(...)`
  4. 当前真正还像“代码层缺口”的，不在已点名主链文件里大面积存在，而是更窄：
     - `SaveManagerDay1RestoreContractTests.cs` 只钉住了 restore hygiene、off-scene snapshot 容器和 suppress restore，没把 `StoryProgressPersistenceService` 对 Day1 phase canonical restore 的关键字段纳进同一组 source-contract；这更像“Day1 合同测试缺口”，不是主业务代码完全没接
     - `WorldStateContinuityContractTests.cs` 仍以源码文本断言为主，能防回退，但不能替代 `Primary/Home/Town` 三场景 live 冒烟
- 不应再误判成“代码没做”的项：
  1. `FarmTileManager / CropController` 已被纳入 bridge continuity 捕获与恢复序列，不再是只收树、不收农地/作物的旧状态。
  2. `SpringDay1Director` 在已检查文件里不再保留绕过 bridge 的 `Town/Home` 直切入口。
  3. `Day1` 的 stale prompt / stale modal / stale pause 清场链已在 `SaveManager` 落地；剩余主要是端到端 live 路径没跑透。
- 当前仍待真实验证、但不应再被写成“代码缺失”的项：
  - `Primary / Home / Town` 往返后的 world-state 真实一致性
  - `farm` 离场跨天玩家路径
  - `Day1` 的进村前 / 农田教学中 / 晚饭入口 / free-time 前后 save-load-restart 端到端
  - packaged smoke
- 最小最安全后续顺序（如果下一轮开改）：
  1. 不先重改 `farm` 主业务，先补 `SaveManagerDay1RestoreContractTests` 对 `StoryProgressPersistenceService` 的 Day1 canonical restore 覆盖，把“代码已接上”先钉成更稳的合同
  2. 然后做最短 live 冒烟，验证 `Primary / Home / Town` world-state 与 `Day1` 四段存档恢复
  3. 只有 live 真失败且指向新断点时，才回到具体业务脚本补刀
- 验证状态：本轮仅完成静态代码审查；结论属于“静态推断成立，live / packaged 未终验”。

## 2026-04-18 连续施工｜输入硬复位补口 + Day1 restore 合同补强 + world-state runner 自动落失败报告
- 用户目标：继续沿 `0417` 收打包前最小闭环，本轮优先补三件事：
  1. 切场 / 重开后的输入门控硬复位
  2. Day1 canonical restore 合同测试
  3. world-state live runner 自己必须自动写 report
- 本轮实际改动：
  - `GameInputManager.ResetPlacementRuntimeState()` 现在会：
    - `SetInputEnabled(true)`
    - 清掉 `InventoryInteractionManager` / `InventorySlotInteraction` held 壳
    - 强解 `ToolActionLockManager`
    - 取消自动导航与旧农具运行态
  - `InventoryPanelUI.ConfigureRuntimeContext(...)` / `BoxPanelUI.ConfigureRuntimeContext(...)`
    - 现在会按 `selectedInventoryIndex` 重新同步本地选槽与 follow 状态
  - `SaveManagerDay1RestoreContractTests.cs`
    - 新增覆盖：
      - `StoryProgressPersistenceService.ApplySnapshot`
      - `ApplySpringDay1Progress`
      - `SpringDay1Director.HasCompletedDialogueSequence`
      - `TryRecoverConsumedSequenceProgression`
      - `LoadSceneThroughPersistentBridge`
  - `PersistentWorldStateLiveValidationRunner.cs`
    - 先把 post-bootstrap 场景链拆成独立 coroutine
    - 再补递归 `RunNestedEnumerator(...)`
    - 再补 `Update + Timer` 双 watchdog 兜底
    - 当前已经能在卡住时自动写出失败 `world-state-live-validation.json`
- fresh 验证：
  - `validate_script GameInputManager.cs`：`assessment=no_red`
  - `validate_script PersistentWorldStateLiveValidationRunner.cs`：`assessment=no_red`
  - `validate_script InventoryRuntimeSelectionContractTests.cs`：`assessment=no_red`
  - `validate_script SaveManagerDay1RestoreContractTests.cs`：`assessment=no_red`
  - `validate_script WorldStateContinuityContractTests.cs`：`assessment=no_red`
  - `errors --count 30`：`errors=0 warnings=0`
- fresh live runner 真票：
  - 报告文件：`Library/CodexEditorCommands/world-state-live-validation.json`
  - 当前结果：
    - `bootstrap = passed`
    - `primary-home-primary = failed`
    - 失败详情：`watchdog_timeout phase=scene_loaded|scene=Home mode=Single active=Home lastSceneLoaded=Home lastActiveScene=Home`
- 当前判断：
  1. runner 这条已经不再是“只能手动 STOP 才有报告”的老问题。
  2. 新 live 失败点已经收窄到 `Primary -> Home -> Primary` 第一段在 `Home` re-entry / settle 阶段卡住。
  3. `P0-B` 与 `P0-D` 这轮新增代码层补口已经落地，但 `5.1 ~ 5.5` 与 `11.1 ~ 11.5` 仍待 live / packaged。
- 恢复点：
  - 下一刀若继续，应直接围绕这张新 world-state 真票收 `Home` 段 continuity / re-entry，不再回头修 runner 是否写报告。

## 2026-04-18 只读排查｜Day1 是否正在占用 Unity 现场并阻断存档线 live
- 用户目标：严谨判断 `Day1` 当前是否正在占用 Unity / PlayMode，进而影响 `存档系统` 继续做 live world-state / continuity 验证。
- 本轮只读核对：
  - `Show-Active-Ownership.ps1 -AsJson`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Day1-V3\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-single-instance-occupancy.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\mcp-live-baseline.md`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\status.json`
  - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\archive\*.cmd`
  - `spring-day1-live-snapshot.json / spring-day1-actor-runtime-probe.json`
- 当前结论：
  1. `Day1-V3` 在线程状态层确实是 `ACTIVE`，slice=`0417｜继续清剩余高优先项与staging验证尾账`。
  2. 更关键的是，Unity 现场不是空闲：`status.json` 显示 `2026-04-18 10:37:59 +08:00` 时 `isPlaying=true`，最近命令是 `playmode:EnteredPlayMode`。
  3. 最近归档命令明确属于 Day1 验证链，而不是存档线：
     - `Bootstrap Spring Day1 Validation`
     - `Reset Spring Day1 To Opening`
     - `Force Spring Day1 Dinner Validation Jump`
     - `Write Spring Day1 Live Snapshot Artifact`
     - `Write Spring Day1 Actor Runtime Probe Artifact`
  4. 同一时段结果文件也被刚刚刷新：
     - `spring-day1-live-snapshot.json` = `2026-04-18 10:38:33 +08:00`
     - `spring-day1-actor-runtime-probe.json` = `2026-04-18 10:38:34 +08:00`
  5. 因此，严格口径应拆成两层：
     - `代码/own 文件冲突`：当前没有看到 Day1 与存档线在 `SaveManager / PersistentPlayerSceneBridge / FarmTileManager / CropController / ToolbarUI` 这些主文件上的直接 shared touchpoint 报警。
     - `Unity 单实例现场占用`：有，而且就是 Day1 当前正在跑的 PlayMode / validation 现场。
- 对存档线的实际影响：
  1. 只读分析不受影响，可以继续。
  2. 但任何新的 Unity live / MCP live / PlayMode 验证现在都不该贸然接着跑，否则会把 Day1 当前运行态和存档线的验证现场互相污染。
  3. 所以如果下一刀要继续收 `world-state` 的 `Home` 段真票，正确前提是先等 Day1 退回 `Edit Mode`、不再继续消费 Day1 validation 命令，再重新取一次现场。
- 验证状态：`静态文件 + 现场命令归档 + 当前 status.json 交叉成立`。

## 2026-04-18 真实施工｜load / restart 背包旧快照回弹收口 + workbench / crop 边界复核
- 用户目标：
  1. 高优先先收 `重开 / 读档后背包 4/8 与交互回弹`
  2. 再核 `作物 / 农地 / 工作台保存恢复`，但要求坚持最小最安全，不搞大重构
- 本轮真实施工：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Tests/Editor/WorldStateContinuityContractTests.cs`
  - `Assets/YYY_Tests/Editor/InventoryRuntimeSelectionContractTests.cs`
  - 另外继续沿用前一刀中的：
    - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
    - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
- 本轮实质修口：
  1. `PersistentPlayerSceneBridge` 新增 `SyncActiveSceneInventorySnapshot()`，把当前 active runtime 的 `inventory + selectedIndex + selectedInventoryIndex` 主动同步回 bridge 内部 snapshot。
  2. `ApplyLoadedSaveData()` 现在在 authoritative rebind 前后各补一次 `SyncActiveSceneInventorySnapshot()`，避免 `LoadGame` 后下一次切场又把旧背包/旧选中态打回来。
  3. `NativeFreshRestartRoutine()` 同样补了 restart 后的 snapshot 同步，避免 `重新开始` 走完后 bridge 里仍残留空快照或旧快照。
  4. `RefreshSceneRuntimeBindingsInternal()` 不再只重绑 UI / 输入；现在会先：
     - `RestoreSceneInventoryState(sceneInventory)`
     - `RebindHotbarSelection(sceneHotbarSelection, sceneInventory)`
     也就是先把真数据重新落进当前 scene，再刷新各层壳。
  5. `SaveManager.cs` 里旧 `FarmTileSaveData` 兼容字段访问已用 `#pragma warning disable 0618` 收口，不再把 legacy crop 兼容链刷成新的构建噪音。
- 本轮只读结论（已吸收进主线判断）：
  1. `crop / farm` 新主链本身还在，当前更像要重点盯 `legacy tile crop -> Crop world object` 兼容面，而不是把整条保存链推翻重来。
  2. `workbench` 活跃制作队列当前仍未进入正式存档；但 `CanSaveNow()` 已明确阻止“制作中 / 待领取 / floating state”保存，所以这块当前最安全口径仍是 blocker，不是假装已闭环。
- fresh 验证：
  - `EditMode`：
    - `WorldStateContinuityContractTests.SaveManager_LoadAndRestart_ShouldRefreshInventoryToolbarAndSelection` = `passed`
    - `InventoryRuntimeSelectionContractTests.SceneRebind_ShouldRefreshInputAndSortRuntimeContext` = `passed`
    - `WorldStateContinuityContractTests.SaveManager_ShouldPromoteLegacyFarmTilesIntoCurrentCropWorldObjects` = `passed`
    - `WorkbenchInventoryRefreshContractTests.WorkbenchOpen_ShouldRebindCraftingServiceBeforeCountingMaterials` = `passed`
    - `StoryProgressPersistenceServiceTests.CanSaveNow_BlocksDialogueChatAndWorkbenchCrafting` = `passed`
    - `StoryProgressPersistenceServiceTests.CanSaveNow_ShouldAlsoBlockReadyWorkbenchOutputsAndFloatingQueueState` = `passed`
  - `errors --count 30` = `errors=0 warnings=0`
- 当前判断：
  1. `4/8 异常 / sort 后切场回弹 / 像旧背包盖回来` 这条主坏相，现在最高置信根因已经从“UI 偶发坏”收敛成“bridge 旧 snapshot 没被 load / restart 后的真值覆盖”；这轮已按最小正确路径补口。
  2. `workbench` 当前不能 claim “活跃队列可正式保存”，但也不是暗炸；目前是显式 blocker 语义，短期内继续维持它比仓促塞正式 DTO 更安全。
  3. 遮挡这轮主修已经进了 `OcclusionManager`，4 张最小回归票已过，但 `PlayerBehindBuilding_WithPixelSamplingHole_StillOccludesByBoundsFallback` 仍留 1 张待继续收；当前不是编译红，是单票语义还未完全钉死。
- 恢复点：
  1. 下一刀若继续背包线，优先做 `load / restart -> Primary/Home/Town 往返` 的 live / packaged 回归，确认是否还会出现旧快照回弹。
  2. 下一刀若继续 world-state，优先 fresh 复核用户看到的“作物没保留”到底是不是 legacy 档口，还是还有新的 save/restore 漏点。
  3. `workbench` 若没有新增裁定，继续保持 blocker 口径，不要擅自 claim 已正式持久化。
## 2026-04-18 追加索引｜Town 首屏遮挡 / restart Town / 0.0.4 workbench 四坏相代码层已收
- 本轮不是再开新方向，而是继续沿 `0417` 打包前最小安全闭环，把用户最新 4 个高优先坏相收成最小正式修口：
  1. 首次启动直进 `Town` 不遮挡
  2. `House 4_0` 遮挡闪一下后失效
  3. `重新开始` 后 `Town` 世界状态没 fresh
  4. `0.0.4` 到工作台阶段卡住
- 当前最新有效进展是：
  - `PersistentPlayerSceneBridge` 已补首屏 fallback 后的一轮 runtime refresh，并把 scene occlusion managers 纳入同级重绑
  - `SaveManager.NativeFreshRestartRoutine()` 已补 target scene restore suppress/cancel
  - `SpringDay1Director` 已补 workbench briefing 的距离/区域兜底
  - `Town.unity` 已只做最小 scene cleanup，删掉 `House 4_0` 的 scene-level 额外 `OcclusionTransparency`
- 当前代码层验证：
  - 四个主脚本 `owned_errors=0 / external_errors=0`
  - fresh console `errors=0 warnings=0`
  - 目标 `EditMode` 9 票 `9/9 passed`
- 当前恢复点：
  - 详细技术细节继续先回 [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
  - 活文档与任务控制板继续先回 [0417.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/0417.md)
## 2026-04-18 遮挡冷启动主问题收口
- 本轮目标：先修“全场启动不遮挡”的主问题，不再先纠缠 `House 4_0`。
- 根因已收窄到：`Town` / `Primary` 里存在 `Tool` 子物体也被打成 `Player` 标签，`OcclusionManager` 冷启动首绑可能抓到假目标。
- 本轮修口：
  - `Assets/YYY_Scripts/Service/Rendering/OcclusionManager.cs`
    - 改成先扫所有 `Player` 候选，再按 `PlayerAnimController / PlayerMovement / PlayerController / usable sprite / sorting` 选真正玩家本体
    - `ResolvePlayerSprite()` 优先吃 `PlayerAnimController.BodySpriteRenderer`
  - `Assets/YYY_Scripts/Anim/Player/PlayerAnimController.cs`
    - 补 `BodySpriteRenderer` getter
  - `Assets/YYY_Tests/Editor/OcclusionSystemTests.cs`
    - 增加“优先真实玩家本体，不抓 tagged Tool 子物体”的护栏
- 验证状态：
  - `validate_script` 三个改动脚本均 `errors=0`
  - `compile` 被 `dotnet` 60s timeout 卡住，属于工具超时，不是代码红
  - 上轮测试打断留下的 `Assets/__CodexEditModeScenes` 临时残留已清理
- 当前恢复点：
  - 下一步只该继续最小 live / 单测验证，优先确认冷启动首屏遮挡是否恢复
  - 当前新增最小遮挡护栏票 `OcclusionSystemTests.RefreshPlayerBindings_PrefersRealPlayerRootOverTaggedToolChild = passed`
  - 清理 `Assets/__CodexEditModeScenes` 后，测试控制台不再出现 `Files generated by test without cleanup`
## 2026-04-18 真实施工｜House 4 单点遮挡最终更正为 prefab 内柱子脚本冲突
- 用户目标：`全场冷启动遮挡` 已基本恢复后，继续收 `Town / House 4_0` 这一栋仍然异常的单点坏相，要求最小最安全，不再扩线。
- 本轮纠偏结论：
  1. `Town.unity` 现场并没有继续给 `House 4` 多挂一层 `OcclusionTransparency`；上一轮把它当 scene-level 重复脏挂并不准确。
  2. 真正独有的额外遮挡脚本在 `Assets/222_Prefabs/House/House 4.prefab` 内部：嵌套子物体 `House 4 柱子_0` 在 prefab 实例层被额外挂了一份 `OcclusionTransparency`。
  3. `House 4_0 / House 4_1` 本体继续保留 authored 遮挡；这轮不碰 `Town.unity`，也不改柱子源 prefab，只删 `House 4.prefab` 里柱子这一份 added `OcclusionTransparency`。
- 本轮真实改动：
  - `Assets/222_Prefabs/House/House 4.prefab`
- 本轮验证：
  - Unity `manage_prefabs get_hierarchy` 复读结果：`House 4 柱子_0` 现在只剩 `Transform + SpriteRenderer + PolygonCollider2D`，不再带 `OcclusionTransparency`
  - `refresh_unity(scope=assets)` 成功
  - `read_console(error+warning)` = `0`
  - `git diff --check -- Assets/222_Prefabs/House/House 4.prefab` = `pass`
- 当前判断：
  1. 这次是真正收 `House 4` 单点最小刀，不再是 scene cleanup 猜测。
  2. 当前仍缺用户 live 终验；代码层和 prefab 导入层已 clean，但还没有把“游戏里这栋房子现在稳定遮挡”假装成已实机确认。
- 恢复点：
  1. 用户回来后优先只验 `Town / House 4_0`
  2. 如果仍异常，下一刀才继续看“建筑多分片 occlusion root 是否要统一到父根”，当前先不扩大到全局算法
## 2026-04-18 真实施工｜House 4 纹理可读 + BatchHierarchy own warning 清理
- 用户目标：继续把这轮 own warning 收平，明确点名两类：
  1. `House 4_0` 纹理不可读导致 `OcclusionTransparency` 回退并刷 warning
  2. `Tool_002_BatchHierarchy` 打开时通过 `GlobalObjectIdentifierToObjectSlow` 拉出一串 `DontDestroyOnLoad / 交叉场景引用` warning
- 本轮原有配置：
  1. `Assets/Sprites/Generated/House 4.png.meta` 的 `TextureImporter.isReadable = 0`
  2. `Assets/Editor/Tool_002_BatchHierarchy.cs` 用 `GlobalObjectId` 持久化 Hierarchy 选择，并在 `OnEnable -> LoadPersistedSelection()` 里逐条 `GlobalObjectIdentifierToObjectSlow(...)` 反解
- 问题原因：
  1. `House 4_0` 开启了像素采样遮挡，但贴图不可读，只能在运行时回退到 Bounds，并刷出 warning
  2. 批量工具重开窗口时会尝试反序列化旧的 scene object GlobalObjectId；这一步会把 `Town` 里对 `DontDestroyOnLoad` 的无效场景引用 warning 一并拖出来
- 本轮真实改动：
  - `Assets/Sprites/Generated/House 4.png.meta`
    - `isReadable: 0 -> 1`
  - `Assets/Editor/Tool_002_BatchHierarchy.cs`
    - 不再用 `GlobalObjectId` 还原 scene 选择
    - 改成 `scene.path + sibling index hierarchy path` 的轻量持久化
    - 只在已加载 scene 内按层级路径恢复对象；旧 GlobalObjectId 记录自动忽略
- 本轮验证：
  1. `validate_script Assets/Editor/Tool_002_BatchHierarchy.cs`：`errors=0`
  2. `refresh_unity(scope=scripts, compile=request)` 后编译未留红
  3. 清空 Console 后重开 `Tools/002批量 (Hierarchy窗口)`：之前那串 `Tool_002_BatchHierarchy -> GlobalObjectIdentifierToObjectSlow` warning 未再出现
  4. `Assets/Sprites/Generated/House 4.png.meta` 已确认 `isReadable: 1`
  5. 最终 `read_console(error+warning) = 0`
  6. `git diff --check` 对两处改动通过
- 当前判断：
  1. 这轮 own warning 已在“配置层 + 工具层”两面收住，不是只压日志。
  2. `Tool_002_BatchHierarchy` 这次的修法属于安全纠偏：功能仍保留“重开窗口后恢复已加载 scene 的锁定对象”，只是去掉了会把 scene 序列化噪声拉出来的恢复路径。
- 恢复点：
  1. 这条线当前可回到用户 live 终验
2. 如果后续还有新的 `OcclusionTransparency` 贴图不可读 warning，再按同样口径只改对应贴图 importer，不扩到全局统一批量可读

## 2026-04-19 真实施工｜楼梯层级切换最小脚本
- 用户目标：
  1. 不再继续纠缠大系统，直接落地一个最简单、最稳的楼梯层级切换脚本
  2. 语义固定为：整块楼梯区域覆盖 trigger；进入不切层；只在从上/下边界离开时切层；左右离开忽略并用空气墙解决
- 本轮真实改动：
  - [StairLayerTransitionZone2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/StairLayerTransitionZone2D.cs)
  - [StairLayerTransitionZone2DTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/StairLayerTransitionZone2DTests.cs)
- 关键实现：
  1. `Trigger Collider2D` 覆盖整块楼梯区域
  2. `topExitTarget / bottomExitTarget` 分别配置离开上边界与下边界后的 `Unity Layer + Sorting Layer`
  3. 同步范围覆盖玩家根、所有子物体 `SpriteRenderer`、以及 `SortingGroup`
  4. 多碰撞器重叠只在最后一次真正离场时执行切层
  5. 为防止工具或其他子碰撞体把边界判断带偏，离场判定优先使用玩家根对象的主 `Collider2D`，再回退到 exiting collider
  6. 区域禁用时会清掉内部 overlap 状态，避免残留
- 验证：
  1. `mcp validate_script`
     - `StairLayerTransitionZone2D.cs`：`0 errors / 0 warnings`
     - `StairLayerTransitionZone2DTests.cs`：`0 errors / 0 warnings`
  2. `python scripts/sunset_mcp.py validate_script ...`
     - 两个目标均 `owned_errors=0 external_errors=0`
     - assessment 为 `unity_validation_pending`
     - 原因不是该刀留红，而是 Unity 现场 `stale_status`
  3. `git diff --check -- [两文件]`：通过
- 当前恢复点：
  1. 后续只需要在目标楼梯对象上完成配置：
     - 楼梯 trigger 覆盖整块平面
     - 填上/下出口层级
     - 左右补薄空气墙
  2. 等 Unity 现场恢复后，再做一次最小实机验证：
     - 从楼梯上边界离开是否升层
     - 从下边界离开是否降层
     - 左右离开是否保持原层不变
- thread-state：
  1. `Begin-Slice`：已补
  2. `Ready-To-Sync`：未通过，被 `ready-to-sync.lock` 卡住
  3. `Park-Slice`：已补，当前状态 `PARKED`
## 2026-04-19 只读审计｜正式存档/读档/重新开始 覆盖范围静态盘点
- 当前主线没换，仍是存档系统收口；本轮子任务是只读审计“正式存档 / 读档 / 重新开始”对世界状态与玩家状态的覆盖范围，并按高风险漏项做最小修复排序。
- 本轮只读核对了：
  - `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - `Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
  - `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
  - `Assets/YYY_Scripts/Farm/FarmTileManager.cs`
  - `Assets/YYY_Scripts/Farm/CropController.cs`
  - `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
  - `Assets/YYY_Scripts/World/WorldItemPickup.cs`
  - `Assets/YYY_Scripts/Service/Crafting/CraftingService.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - `Assets/YYY_Scripts/Service/Inventory/InventoryService.cs`
  - `Assets/YYY_Scripts/Service/Inventory/HotbarSelectionService.cs`
  - `Assets/YYY_Scripts/Service/Equipment/EquipmentService.cs`

- 已确认的正式覆盖域：
  1. `SaveManager.CollectFullSaveData()` 会正式写入：
     - 游戏时间
     - 当前玩家位置与 scene
     - 当前 loaded scene 的 `PersistentObjectRegistry` 对象
     - 云影持久态
     - off-scene world snapshot
  2. 当前 loaded scene 内，以下 world-state 已进入正式 save / load 主链：
     - `InventoryService`
     - `FarmTileManager`
     - `CropController`
     - `ChestController`
     - `WorldItemPickup`
     - `TreeController`
     - `StoneController`
  3. `StoryProgressPersistenceService` 已正式覆盖：
     - `StoryPhase`
     - 已完成对白序列
     - `SpringDay1Director` 的核心教程/自由行动进度
     - `Health / Energy`
     - NPC 关系阶段
     - `workbenchHintConsumed`
  4. `PersistentPlayerSceneBridge` 已正式覆盖 off-scene world continuity：
     - `FarmTileManager / Crop / Chest / Drop / Tree / Stone`
     - 并带离场天数补票给 `Tree / Crop / FarmTile`
  5. `重新开始` 当前会明确清掉：
     - bridge 缓存的 off-scene snapshot
     - 背包
     - 装备
     - hotbar 选中
     - 时间
     - 剧情长期态

- 已确认的高风险漏项 / 半覆盖：
  1. `EquipmentService`
     - 虽然实现了 `Save()/Load()`，但该文件和仓库里都没找到注册进 `PersistentObjectRegistry` 的入口。
     - `SaveManager.CollectFullSaveData()` 又只认 registry。
     - 结果是正式读档不会按存档回滚装备栏，`重新开始` 只会强清当前运行时装备。
  2. 玩家 DTO 半覆盖：
     - `PlayerSaveData` 里声明了 `selectedHotbarSlot / gold / stamina / maxStamina / currentLayer`
     - 但 `SaveManager.CollectPlayerData()` 实际只写 `positionX / positionY / sceneName`
     - `RestorePlayerData()` 也只恢复位置
     - 当前真正能影响玩家手感的是 hotbar 选中丢失；其余字段属于“DTO 写了但主链没接”。
  3. `HotbarSelectionService`
     - 正式存档没有自己的持久化入口。
     - bridge 只在跨 scene runtime continuity 里临时记 `selectedIndex / selectedInventoryIndex`。
     - 冷启动读档或换档时，toolbar 选中不是按 save 文件恢复。
  4. `ChestController`
     - `Save()` 只写了 `isLocked + slots + customName`
     - 没写 `origin / ownership / hasBeenLocked / currentHealth`
     - `Load()` 也只回了 `isLocked + contents`
     - 世界箱子的“归属/是否曾上锁/剩余血量”读档后会回弹到 prefab/runtime 默认。
  5. `WorldItemPickup`
     - 当前掉落物 `Save()` 只写 `itemId / quality / amount / sourceNodeGuid`
     - 没写 `runtimeItem` 的动态属性、耐久、实例态。
     - 掉在地上的工具、带属性物品、种子袋这类 runtime item 读档后会被压扁成普通堆叠物。
  6. workbench 队列态
     - `SpringDay1WorkbenchCraftingOverlay` 的 `readyCount / totalCount / currentUnitProgress` 全是纯 runtime。
     - `StoryProgressPersistenceService.CanSaveNow()` 目前靠 save blocker 阻止“制作中 / 待领取产物”时存档。
     - 这不是正式覆盖，而是显式排除。
  7. off-scene NPC resident runtime
     - bridge 里有 `nativeResidentSnapshotsByScene / crowdResidentSnapshots`
     - 但正式 `ExportOffSceneWorldSnapshotsForSaveInternal()` 只序列化 world object snapshot，不写 resident snapshot。
     - 所以离场 NPC 的 runtime continuity 目前更像 session 内缓存，不是正式 save 文件合同。

- 当前最高风险排序（偏用户体感）：
  1. 装备栏不进正式存档：读旧档后装备不回退，最容易造成“角色状态没回去 / 读档像半成功”。
  2. 世界箱子只存内容不存归属/锁史/血量：最容易造成锁箱回弹、可挖性回弹、世界规则失真。
  3. 掉落物 runtime item 被压扁：最容易造成耐久修复感、属性物丢属性、刷物/洗白。
  4. hotbar / inventory selection 不随档恢复：直接造成工具栏手感错位、读档后“选中的不是存档那格”。
  5. off-scene resident runtime 不入正式 save：跨场景/跨重进后 NPC 站位/控制态可能回弹。

- 建议的最小可打包修复优先级：
  1. 先补 `EquipmentService` 正式注册与 restore 合同。
  2. 再补 `ChestController` 的 `origin / ownership / hasBeenLocked / currentHealth`。
  3. 再给 `WorldItemPickup` 增加 `runtimeItem` 完整 DTO。
  4. 再把 hotbar selection 正式接进 save contract，而不是只靠 bridge session snapshot。
  5. 最后判断 resident snapshot 是否需要进正式 save 文件，而不再只留在 bridge 内存。

- 当前恢复点：
  1. 这轮已把“正式存档到底覆盖了什么”钉成静态结论，可直接拿去指导下一轮最小修复切片。
  2. 如果后续进入真实施工，第一刀不该再泛修 UI，而应先从：
     - `EquipmentService`
     - `ChestController`
     - `WorldItemPickup`
     这三条最容易造成回弹/刷物的真数据合同下手。
## 2026-04-19 真实施工｜剧情读档/重开 blocker 再收口 + 现实存档样本复核
- 当前主线仍是打包前的存档闭环；这轮只收“剧情导演态与 DayEnd 收束窗口禁止读档/重开”这条高风险口，不扩到 UI 美化或额外系统。

### 本轮新增修口
1. `SaveManager.cs`
   - 新增 `CanExecutePlayerRestartAction(out string blockerReason)`
   - `RestartToFreshGame()` 不再直接放行，而是先过同一套剧情/场景恢复 blocker
   - 这样 `重新开始` 不会再绕开 `CanLoadNow()` 与 scene-restore 保护
2. `SpringDay1Director.cs`
   - `TryGetStorySaveLoadBlockReason()` 新增 `StoryPhase.DayEnd`
   - 只在 `第一夜收束 / 回家安置` 真正完成后才放行
   - 当前判据是：`!IsHomeSceneActive()` 或 `_pendingForcedSleepRestPlacementFrames > 0`
3. 合同测试
   - `SaveManagerDay1RestoreContractTests.cs`
   - `SaveManagerDefaultSlotContractTests.cs`
   - 已补“重开复用 blocker”“DayEnd 收束窗口进权威 blocker”的字符串护栏

### 这轮现实样本核对
1. 直接核了玩家机器上的实际存档目录：
   - `C:\Users\aTo\AppData\LocalLow\DefaultCompany\Sunset\Save`
2. 发现当前已有 `__default_progress__.json` 与多个普通槽位都属于旧格式：
   - 都有 `StoryProgressState`
   - 都有 `PlayerInventory`
   - 都没有 `EquipmentService`
3. 这说明一件重要事实：
   - “旧档缺 `EquipmentService`”现在是历史现实，不是单个损坏样本
   - 所以本轮不能把这类旧档一刀切判成“不可读取”
   - 正确口径应是：`新版本以后拒绝再写出半截档，但旧档读取保持兼容`

### 验证
1. `manage_script validate`
   - `SaveManager.cs`：`errors=0`，仅旧 warning 3 条
   - `SpringDay1Director.cs`：`errors=0`，仅旧 warning 3 条
   - 两个合同测试文件：`clean`
2. `errors --count 20 --output-limit 10`
   - `errors=0 warnings=0`
3. `git diff --check`
   - 通过
   - 仅 `SaveManager.cs` 继续报 CRLF/LF 提示，不是编译错误

### 当前结论
1. `F9 直接走原生重开` 这条错链，当前静态代码里仍未出现；默认槽读取与 `RestartToFreshGame()` 仍然分开。
2. 这轮真正新收住的是：
   - `DayEnd` 那个之前没纳入 blocker 的窄窗口
   - `重新开始` 入口绕过剧情 blocker 的漏口
3. 这轮没有把“旧档缺 EquipmentService”粗暴升级成读档致命错误；这是刻意保兼容，不是漏改。

### 仍未闭环
1. 还没拿到 live 终验票去证明：
   - 剧情对白出现前的导演接管窗口现在真的拦住了 `读档 + 重开`
   - `DayEnd` 过夜收束窗口现在也真的拦住了
2. `validate_script / compile-first` 仍受 `stale_status / codeguard timeout` 干扰，所以这轮 Unity compile-first 证据仍不完整，只能诚实落到：
   - 代码层 clean
   - fresh console clean
   - live 终验待补

---

## 2026-04-19 toolbar 切场“像空了”补口

### 用户反馈
1. `Home` 里背包正常、工具也能切和使用，但 toolbar 看起来像没东西。
2. 切到 `Primary` 后，toolbar 图标又会回来。
3. 用户明确要求：空气墙继续只读，toolbar 这条要找到问题并修复。

### 这轮查到的关键点
1. 这次不像 inventory 数据丢失：
   - 背包显示正常
   - 工具使用正常
   - 更像 UI 壳层被误处理
2. 真根因落在 `PersistentPlayerSceneBridge.UpdatePersistentUiBoundaryFocus()`：
   - 它会把名为 `ToolBar` 的 UI 根纳入 boundary focus 目标
   - 再按玩家 viewport 动态给它的 `CanvasGroup.alpha` 做淡出
3. 这会造成一个很像“toolbar 空了”的假象：
   - `Home` 场景里玩家初始 viewport 更容易压到底边阈值
   - `Primary` 里玩家位置/镜头不同，toolbar 又显出来

### 已做修复
1. 从 `PersistentPlayerSceneBridge.ResolveNamedBoundaryFocusEdges()` 里移除了 `ToolBar` 的边界淡出映射。
2. toolbar 不再被 boundary HUD 淡出链误伤。

### 验证
1. `validate_script Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs`
   - `assessment=no_red`
   - `owned_errors=0`
   - `external_errors=0`
2. `git diff --check`
   - 通过
3. 当前仍缺：
   - `Home -> Primary` 的 live 终验票

### 当前结论
1. 这轮 toolbar 问题的最像根因不是背包数据和 save contract，而是桥上的边界 HUD 淡出策略误伤了 `ToolBar`。
2. 这条修复是最小刀口：
   - 不动 inventory/save 数据
   - 只切掉错误的 UI 淡出映射

## 2026-04-20 追加｜Day2 后仍无法新建/读取存档只读排查

- 用户目标：只读排查 `Day14` 已进入 Day2 后仍无法新建/读取存档的问题，限定核对 `SaveManager`、`StoryProgressPersistenceService`、`SpringDay1Director`、`PackageSaveSettingsPanel` 的 `can save/load/new slot blocker` 语义，不改代码。
- 本轮子任务：顺着 `SaveManager -> StoryProgressPersistenceService -> SpringDay1Director` 的 blocker 链，以及 `PackageSaveSettingsPanel` 的状态文案/按钮回调，区分主根因到底是 UI 文案错、条件判断错，还是剧情状态未正确退出。
- 结论先记：
  1. 最可能主根因不是 `SaveManager` 自己算错，而是 `SpringDay1Director` 在 `HandleSleep()` 后把 `StoryManager.CurrentPhase` 固定推进到 `StoryPhase.DayEnd`，同时把 `_dayEnded = true`，但当前代码里没有正常 Day2 入口去清掉这组状态或推进到 Day1 之外的新 phase。
  2. `TryGetStorySaveLoadBlockReason()` 对 `StoryPhase.DayEnd` 的判断又把“离开 Home 场景”直接视为未收束：`IsDayEndSceneSettlePending()` 里只要 `_dayEnded == true` 且 `!IsHomeSceneActive()` 就持续返回 `true`。因此玩家第二天一旦不在 `Home`，`CanSaveNow()` / `CanLoadNow()` 都会继续被导演态 blocker 卡住。
  3. `新建存档` 失败是同一主根因的直接外显：`CreateNewOrdinarySlotFromCurrentProgress()` 走 `SaveGame()`，最终仍会吃到 `CanSaveNow()` 的同一条导演 blocker。
  4. UI 层确实还有两处误导，但它们是“遮蔽问题”的次因，不是主根因：
     - `PackageSaveSettingsPanel.RefreshView()` 只用 `CanExecutePlayerSaveAction()` 决定顶部状态文案，所以会写出“当前默认槽可读但不可写”，但实际 `load` 往往也被同一条 story blocker 卡住。
     - `HandleDefaultLoad()` / `HandleLoad()` 只拿 `bool success` 回调，失败 toast 只有“默认存档读取失败 / 读档失败”，不会把真实 blocker reason 透给玩家。
- 证据链：
  - `SpringDay1Director.HandleSleep()`：睡觉后设 `_dayEnded = true`、`SetPhase(StoryPhase.DayEnd)`，但未见正常 Day2 退出。
  - `SpringDay1Director.IsDayEndSceneSettlePending()`：`_dayEnded` 且不在 `Home` 时永久视为 pending。
  - `SpringDay1Director.TryGetStorySaveLoadBlockReason()`：`StoryPhase.DayEnd` 命中上面 pending 就拦存/读档。
  - `StoryProgressPersistenceService.CanSaveNow()/CanLoadNow()`：二者都会调用 `director.TryGetStorySaveLoadBlockReason(...)`。
  - `SaveManager.CreateNewOrdinarySlotFromCurrentProgress()/LoadGame()/QuickLoadDefaultSlot()`：分别把新建槽位和读档都接到上述 blocker。
  - `PackageSaveSettingsPanel.RefreshView()/HandleDefaultLoad()/HandleLoad()`：顶部文案只看 save blocker，load 失败回调只给泛化失败文案。
- 最小安全修法建议：
  1. 这轮若只想安全解 Day2 存读档 blocker，最小刀口优先收在 `SpringDay1Director`：
     - 把 `IsDayEndSceneSettlePending()` 从“`_dayEnded && !IsHomeSceneActive()` 永久 pending”改成只覆盖真正的日终收束窗口，例如 scene blink / 强制摆位重试还没结束时才拦。
     - 这样不需要同时改 `SaveManager`、`StoryProgressPersistenceService` 和 UI 调用链。
  2. 如果要语义真正正确，后续还应补一条“DayEnd -> Day2 常态”退出口，把 `StoryManager.CurrentPhase` 和 `_dayEnded` 从 Day1 日终态里释放出来；否则地图/NPC/提示卡仍会长期以 `DayEnd` 语义运行，只是存读档不再被拦。
  3. UI 侧建议作为第二刀补：
     - `RefreshView()` 顶部状态分开显示 `save` / `load` 状态；
     - `HandleDefaultLoad()` / `HandleLoad()` 失败时透传真实 blocker reason，而不是只报泛化失败。
- 本轮验证：纯静态代码链排查，未改文件、未跑 `Begin-Slice`，结论属于“静态推断成立，live 复现仍待补”。

## 2026-04-19 追加｜NPC resident 位置与正式存档格式分界

- 这轮补查后确认：`NPCAutoRoamController` 的 resident 快照只存在于 `PersistentPlayerSceneBridge` 的运行态缓存里，`GameSaveData` / `SaveDataDTOs` 里没有正式的 NPC resident 位置字段。
- 也就是说，当前默认存档和普通存档都不能算已经闭环保存“所有场景 NPC 的位置”。
- 现在桥上的恢复口会把 `residentPosition` 和 `homeAnchorPosition` 摆回去，但调用链是 `resumeResidentLogic: true`，并不等于“只恢复坐标、不激活”。
- 如果后续要正式补这条语义，正确方向应是先把 resident 快照纳入正式存档 DTO，再单独拆出一个只回坐标的恢复口，不要直接把 bridge 里的 runtime 恢复当成存档闭环。

## 2026-04-19 箱子放置错位与推墙穿模最小修复

### 用户目标
- 直接修掉两个马上影响打包体验的箱子问题：
  1. 放置预览位置与实际落点不一致
  2. 推动箱子时箱子不会像玩家一样被墙挡住

### 根因结论
1. `PlacementGridCalculator.GetPlacementPosition()` 之前把视觉层“假想的底部对齐偏移”叠进了实际落点，导致箱子真正落地时 collider 中心和预览不在一套真值上。
2. `ChestController.TryPush()` 之前只在目标中心点做 `OverlapCircleAll`，对宽箱子、贴边和墙角不成立，所以会出现“玩家撞墙、箱子不撞墙”的坏相。

### 本轮已做
1. `Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs`
   - 新增 `GetPlacementColliderLocalCenter()`
   - `GetPlacementPosition()` 改回按真实 collider 几何中心计算
   - `TryGetPlacementReachEnvelopeBounds()` 改回按真实 collider footprint 计算
   - `GetPreviewSpriteLocalPosition()` 同步改成和实际落点共用同一套 collider 中心合同
2. `Assets/YYY_Scripts/World/Placeable/ChestController.cs`
   - `TryPush()` 改为 `Collider2D.Cast`
   - 加 `0.02f` 薄 skin，减少贴边漏判
3. `Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs`
   - 新增箱子 placement/reach/preview 三条几何合同测试
   - 新增推墙逻辑文本护栏，防止退回 `OverlapCircleAll`

### 验证
1. `git diff --check -- Assets/YYY_Scripts/World/Placeable/ChestController.cs Assets/YYY_Scripts/Service/Placement/PlacementGridCalculator.cs Assets/YYY_Tests/Editor/ChestPlacementGridTests.cs`
   - 通过
2. `validate_script / compile --skip-mcp`
   - 当前都被 `CodexCodeGuard timeout` 与 `CLI/Unity bridge` 阻塞
   - 没拿到 live compile 票
3. 当前可诚实宣称的状态：
   - `文本结构 clean`
   - `语法肉眼复核通过`
   - `Unity live 终验待补`

### 当前恢复点
- 如果下一轮继续收箱子，直接从：
  - 实机验证 `Box_1 / Box_2` 的预览与实际落点是否完全重合
  - 贴墙推动是否仍会把 collider 推进墙里
  这两个 case 开始，不需要再重做根因分析。

### 补充尾账｜测试程序集边界修复
1. 新增的 `ChestPlacementGridTests.cs` 最初直接强类型调用 `PlacementGridCalculator`，在 `Tests.Editor.asmdef` 下会报 `CS0103`。
2. 根因不是运行时代码缺失，而是当前测试 asmdef 没直接引用运行时代码程序集。
3. 最小修法没有去改整个 asmdef 结构，而是把测试改成：
   - 通过 `AppDomain + Reflection` 定位 `PlacementGridCalculator`
   - 反射调用 `GetRequiredGridSizeFromPrefab / GetPlacementPosition / GetPreviewSpriteLocalPosition / TryGetPlacementReachEnvelopeBounds`
4. 这样既保留了测试护栏，也不会再因为 asmdef 边界直接编不过。

## 2026-04-20 只读排查：默认存档读取失败链

- 当前主线目标不变，仍是存档系统收口；本轮子任务是只读钉死“默认存档读取失败”最可能的单一主根因，并限定在 `SaveManager / PackageSaveSettingsPanel / 默认槽 UI 入口 / F9 / blocker / 错误提示链`。
- 本轮完成：
  1. 静态核对 `SaveManager.Update()/LoadGame()/QuickLoadDefaultSlot()/ExecuteQuickLoadHotkey()/TryGetDefaultSlotSummary()/TryReadSaveData()/GetSaveFilePath()`。
  2. 静态核对 `PackageSaveSettingsPanel.RefreshView()/HandleDefaultLoad()/HandleLoad()` 与 `PackagePanelTabsUI.OpenSettings()` 的默认槽 UI 入口链。
  3. 回看 `StoryProgressPersistenceService.CanLoadNow()`、`SpringDay1Director.TryGetStorySaveLoadBlockReason()` 与两份合同测试，确认读档 blocker 的权威来源。
- 最关键判断：
  1. 当前最可能被用户感知成“默认存档读取失败”的单一主根因，不是默认存档文件路径，也不是 `QuickLoadDefaultSlot()` 还在偷走重开链，而是设置页默认槽入口把“读档 blocker”吞成了统一失败。
  2. 具体表现是：
     - `PackageSaveSettingsPanel.RefreshView()` 用的是 `CanExecutePlayerSaveAction()` 来写头部状态，没用 `CanExecutePlayerLoadAction()` 判断默认槽当前能不能读。
     - 默认槽按钮是否可点只看 `TryGetDefaultSlotSummary()` 的文件存在/摘要状态，不看读档 blocker。
     - `HandleDefaultLoad()` 不在弹确认前先报 `CanExecutePlayerLoadAction()` 的 blocker，确认后直接调用 `QuickLoadDefaultSlot(success => ... "默认存档读取失败")`，把剧情接管/场景恢复窗口也压成了假“失败”。
  3. `F9` 本身不是这条主根因：
     - `ExecuteQuickLoadHotkey()` 已先判 `SaveExists(DefaultProgressSlotName)`，再判 `CanExecutePlayerLoadAction(out blockerReason)`，会直接 toast 真实 blocker。
     - 只有通过前置检查后，`QuickLoadDefaultSlot()`/`LoadGameInternal()` 真读文件失败时，`F9` 才会落到统一失败 toast。
- 文件路径与摘要链结论：
  1. 默认存档路径仍是 `Application.persistentDataPath/Save/__default_progress__.json`；旧 `项目根/Save` 与 `Assets/Save` 只做一次迁移兼容，不是当前最像的首嫌。
  2. 摘要链和实际读档链共用同一个 `GetSaveFilePath()` / `TryReadSaveData()`；如果默认槽摘要已经正常显示，说明“路径错了/根本没找到文件”不是这次最可能主因。
- 最小安全修法建议：
  1. 只改 `PackageSaveSettingsPanel`，不要动 `SaveManager` 的正式读档主链。
  2. 让默认槽 UI 和普通槽 UI 在点击前统一先走 `manager.CanExecutePlayerLoadAction(out blockerReason)`：
     - 有 blocker：直接 toast 真实原因，不弹确认，不再落“默认存档读取失败”。
     - 无 blocker：再继续弹确认并调用现有读档方法。
  3. `RefreshView()` 里默认槽状态文案改为基于“摘要状态 + load blocker”双条件：
     - 文件/摘要异常继续显示现有摘要错误。
     - 文件正常但当前不可读时，显式显示“默认存档存在，但当前不可读取：{blockerReason}”。
  4. 最稳的最小护栏是补一条 `PackageSaveSettingsPanel` 合同测试，要求默认槽按钮和普通槽按钮在 load blocker 命中时必须前置报 blocker，而不是统一 toast “读取失败”。
- 当前恢复点：
  1. 如果下一轮转施工，第一刀只收 `PackageSaveSettingsPanel` 的读档提示链，不回扩 `SaveManager` 存储格式或剧情存档合同。
  2. 本轮始终只读，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`；当前 live 状态保持 `PARKED`。

## 2026-04-20 真实施工｜Day2 存档 blocker 退场 + 设置页真实 load blocker 透传

- 当前主线目标不变，仍是按 `0417` 收存档系统；这轮直接收用户当前最痛的两件事：
  1. `Day2 / D14` 后仍不能 `新建存档 / 读档 / 读默认存档`
  2. 设置页把真实 blocker 误报成“默认存档读取失败”
- 本轮子任务：
  1. 修 `SpringDay1Director` 的 `DayEnd` 退出口
  2. 修 `PackageSaveSettingsPanel` 的 `save/load` 状态与读档提示链
  3. 补文本合同测试与最小代码闸门
- 子任务服务于什么：
  - 把当前最阻塞打包体验的存档坏相先收成“第二天常态不再被 Day1 卡住，设置页不再误导玩家”
- 修复后恢复点：
  - 下一轮只需要补 live 票：
    1. `Day2` 离开 `Home` 后能否 `新建普通存档 / 读取普通存档 / 读取默认存档`
    2. 左侧任务栏是否已退出 Day1 未完成态

### 本轮代码结论
1. 主根因已确认：
   - `SpringDay1Director` 之前把 `!IsHomeSceneActive()` 长期当成 `DayEnd` 未收束
   - 所以第二天一离开 `Home`，`TryGetStorySaveLoadBlockReason()` 仍会继续拦存档
2. 设置页只是第二层误导：
   - 顶部状态只看 save blocker
   - 默认槽/普通槽的 load 失败又统一 toast 成“读取失败”
   - 用户会被误导成“默认存档文件坏了”

### 本轮已做
1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - 新增 `_dayEndSceneSettlePending`
   - `HandleSleep()` 进入 `DayEnd` 时显式置位
   - `IsDayEndSceneSettlePending()` 改成只看真正的瞬时收束态
   - 新增 `IsManagedDay1RuntimeCurrentlyActive()` 与 `ShouldPersistCurrentRuntimeState()`
   - `IsStoryRuntimeSceneActive()` 现在会在 Day2 常态自动退场
   - `TryResolvePlayerFacingPhase()` 跟随 runtime 窗口退场，避免第二天继续挂 Day1 任务/提示
2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - `CaptureSpringDay1Progress()` 只在 Day1 runtime 仍有效时才保存 Day1 临时快照
3. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - `RefreshView()` 现在分开读 `canSave / canLoad / canRestart`
   - 顶部状态会直接报“当前可保存但不可读取 / 当前可读取但不可保存 / 当前都不可用”
   - 默认槽摘要在“文件存在但当前被 blocker 拦住”时会显示真实 blocker
   - 默认槽/普通槽加载按钮同时服从 `canLoad`
   - `HandleDefaultLoad()` / `HandleLoad()` 在确认前先报真实 blocker，不再统一 toast “读取失败”
4. 合同护栏
   - `Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs`
   - `Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`

### 本轮验证
1. `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs Assets/YYY_Tests/Editor/SaveManagerDefaultSlotContractTests.cs Assets/YYY_Tests/Editor/SaveManagerDay1RestoreContractTests.cs`
   - 通过
2. `manage_script validate`
   - `SpringDay1Director`：`warning(3)`，无 error
   - `StoryProgressPersistenceService`：`clean`
   - `PackageSaveSettingsPanel`：`clean`
   - 两份测试：`clean`
3. `errors --count 20 --output-limit 10`
   - `0 errors / 0 warnings`
4. `compile` 与部分 `validate_script`
   - 仍被 `CodexCodeGuard timeout / stale_status` 卡住
   - 当前能诚实宣称的是：
     - `代码层 clean`
     - `fresh console clean`
     - `Unity compile-first 绿票待补`
5. `thread-state`
   - 本轮沿既有 `ACTIVE` slice 施工，收尾已执行 `Park-Slice`
   - 当前 live 状态：`PARKED`
   - 当前 blocker：
     - `Day2/默认存档 live 终验待补`
     - `Unity compile-first 仍受 CodexCodeGuard timeout / stale_status 阻塞`

### 当前判断
1. 这轮已经不是只读分析，而是代码层最小闭环已落地。
2. 当前最直接的玩家向收益是：
   - 第二天常态不再被 Day1 `DayEnd` blocker 卡住存档
   - 设置页不再把真实 blocker 伪装成“默认存档读取失败”
3. 当前最大剩余不是方向不清，而是还差 live 终验票。

## 2026-04-20 回滚记录｜撤回 Day2 blocker / 设置页修法

- 用户实测后明确反馈：
  1. 出屋位置异常
  2. 可存档时机异常
  3. 剧情提示异常
  4. 整体状态错乱
- 复盘后确认问题不是设置页文案本身，而是我这轮把存档修法越线带进了 `SpringDay1Director` 的 Day1 运行窗口判断：
  - 改了 Day1 何时仍算 active
  - 改了 `DayEnd` 收束态
  - 改了任务卡 / 提示 / Day1 临时快照何时继续接管
- 这属于“想修 save/load blocker，却实质动了剧情导演合同”，因此必须整体撤回，不能继续留在现场。

### 已回滚
1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
   - 撤回 `_dayEndSceneSettlePending`
   - 撤回 `ShouldPersistCurrentRuntimeState()`
   - 撤回 `IsManagedDay1RuntimeCurrentlyActive()`
   - 撤回对 `TryResolvePlayerFacingPhase()`、`HandleSleep()`、`TryFinalizePendingForcedSleepRestPlacement()`、`IsStoryRuntimeSceneActive()` 的这轮修改
   - 撤回我补进去的 `StoryPhase.DayEnd` save/load blocker 分支
2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
   - 撤回 `CaptureSpringDay1Progress()` 的 Day1 runtime 门槛
3. `Assets/YYY_Scripts/UI/Save/PackageSaveSettingsPanel.cs`
   - 撤回 `save/load` 双态 blocker 文案
   - 撤回加载按钮对 `canLoad` 的前置约束
   - 撤回 `HandleDefaultLoad()` / `HandleLoad()` 里这轮新增的 blocker 透传
4. 两份合同测试
   - 撤回与上面几条回滚内容对应的新增文本护栏

### 回滚后现场
1. 这轮我引入的存档/剧情联动改动已经撤掉。
2. 当前 relevant 文件里仍有一些别的 dirty，但不是我这轮新增：
   - `SpringDay1Director.cs` 还存在他线已有的 `Home` 进入门槛与 story actor informal surface 相关脏改
   - 我已避开未动
3. 当前状态只能诚实落为：
   - `我的错误改动已撤回`
   - `现场是否完全恢复，待用户复测`

### 验证
1. `git diff --check --` 本轮回滚涉及文件：通过
2. `manage_script validate`
   - `SpringDay1Director`：`warning(3)`，无 error
   - `StoryProgressPersistenceService`：`clean`
   - `PackageSaveSettingsPanel`：`clean`

### thread-state
- 本轮已执行 `Park-Slice`
- 当前 live 状态：`PARKED`
- 当前 blocker：
  1. `需用户复测：出屋位置 / 可存档时机 / 剧情提示是否恢复`
  2. `SpringDay1Director 仍有他线既存脏改，已避开未动`

## 2026-04-20 追加索引｜Day1 剧情 blocker 节点与正式保存清单已做只读梳理
- 本轮用户明确要求：
  - 不改 `Day1`
  - 只把“当前剧情是否正在进行”的判断节点查清
  - 并把正式存档应该保存的内容列成清单
- 当前新钉实的结论是：
  1. 权威入口仍是 `StoryProgressPersistenceService.CanSaveNow()/CanLoadNow()`
  2. 其内部顺序固定为：
     - 正式对白
     - NPC 闲聊
     - `SpringDay1Director.TryGetStorySaveLoadBlockReason(...)`
     - `save` 专属工作台 blocker
  3. `Day1` 分阶段 blocker 当前真实口径已经可直接列成清单：
     - `CrashAndMeet` 整段拦
     - `EnterVillage / HealingAndHP / WorkbenchFlashback / DinnerConflict / ReturnAndReminder` 都要求本段关键对白真正完成才放
     - `FarmingTutorial` 只有“和村长收口”这拍还拦
     - `FreeTime` 只有夜间引导未收束时拦
     - `DayEnd` 当前不在正式剧情 blocker 分支里
  4. 正式存档应保存的主合同也已重新对齐：
     - 时间
     - 玩家位置与选槽来源
     - 剧情长期态
     - Day1 运行进度
     - 血量/精力
     - 当前 scene worldObjects
     - off-scene world snapshots
- 详细技术恢复点已回写：
  - [4.0.0_三场景持久化与门链收口/memory.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/存档系统/4.0.0_三场景持久化与门链收口/memory.md)
