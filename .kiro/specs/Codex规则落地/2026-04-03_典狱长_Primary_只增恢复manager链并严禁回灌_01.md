# 2026-04-03_典狱长_Primary_只增恢复manager链并严禁回灌_01

你这轮的唯一目标，不是“顺手修一堆运行时报错”，也不是“把旧备份搬回来”，而是把 `Assets/000_Scenes/Primary.unity` 里缺失的时间/季节 manager 链，在当前磁盘版 `Primary` 之上做最小加法恢复。

## 当前已接受的基线

- 当前 `Primary.unity` 仍是用户独占锁，锁文件为：
  - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`
  - owner = `用户Primary独占`
- 当前磁盘版 `Primary.unity` 仍有未提交改动，且不能被你覆盖式处理：
  - `git diff --shortstat -- Assets/000_Scenes/Primary.unity`
  - 当前结果：`1 file changed, 1789 insertions(+), 590 deletions(-)`
- 当前 `Primary.unity` 文本中搜不到：
  - `SeasonManager`
  - `TimeManagerDebugger`
  - `m_Name: 'TimeManager '`
- 但以下只读参考里仍能搜到这条链：
  - `Assets/000_Scenes/primary_backup_2026-04-02_20-46-54.unity`
  - 老提交 `65e1ee35`
- 备份里的关键只读证据：
  - `SeasonManager` 在 `primary_backup_2026-04-02_20-46-54.unity:1191`
  - `TimeManager ` 在 `primary_backup_2026-04-02_20-46-54.unity:67111`
  - `TimeManagerDebugger` 脚本 GUID 在 `primary_backup_2026-04-02_20-46-54.unity:67141`
  - 关键字段：
    - `useTimeManager: 1`
    - `enableDebugControl: 0`
    - `enableDebugKeys: 1`
    - `nextDayKey: 275`
    - `nextSeasonKey: 274`
    - `prevSeasonKey: 273`
    - `enableScreenClock: 1`
- `GameInputManager` 里虽然有 `timeDebugger` / `enableTimeDebugKeys` 字段，但当前 `Primary` 和参考备份里它们都还是：
  - `timeDebugger: {fileID: 0}`
  - `enableTimeDebugKeys: 0`
  所以你这轮不准先假设必须改 `GameInputManager.cs` 或它的场景序列化值。

## 本轮唯一主刀

- 唯一主刀：`Primary.unity` 里的 `SeasonManager + TimeManager + TimeManagerDebugger` manager/debugger 链恢复。
- 修复对象是场景基线，不是：
  - `TreeController.cs`
  - 字体资产
  - `Town.unity`
  - `Home.unity`
  - `GameInputManager.cs`
  - 任何 scratch scene / validation scene / backup scene

## 两阶段硬闸门

### 阶段 A：现在默认只读

在用户没有明确把 `Primary` 写窗交给你之前，你现在只允许：

1. 只读审计当前磁盘版 `Primary.unity`
2. 对照只读参考，列出最小恢复方案
3. 明确如果进入写阶段，具体只会新增/恢复哪些对象与组件
4. 明确哪些现有字段你认为可能需要补引用；如果有，必须逐项列 exact field 和理由

### 阶段 B：只有在用户明确开放写窗后才允许真实写

只有同时满足下面 3 条，才允许你真的写 `Primary.unity`：

1. 用户明确说“现在允许你接手写 `Primary`”
2. `Primary.unity` 的用户独占锁已经释放或正式转交到你
3. 你确认没有别的 Unity live writer / MCP live writer 同时在碰这个 scene

少一条都不准写。

## 只允许的改动范围

如果阶段 B 被明确打开，你只允许在“当前磁盘版 `Assets/000_Scenes/Primary.unity`”上做最小增量恢复：

1. 恢复或新增 `SeasonManager` GameObject，并挂回 `SeasonManager` 组件
2. 恢复或新增名为 `TimeManager ` 的 GameObject，并挂回：
   - `TimeManager`
   - `TimeManagerDebugger`
3. `SeasonManager` 关键字段至少对齐到只读参考：
   - `useTimeManager: 1`
   - `enableDebugControl: 0`
4. `TimeManagerDebugger` 关键字段至少对齐到只读参考：
   - `enableDebugKeys: 1`
   - `nextDayKey: 275`
   - `nextSeasonKey: 274`
   - `prevSeasonKey: 273`
   - `enableScreenClock: 1`
5. 如果确实必须碰已有对象字段，只允许为这条 manager/debugger 链补最小必要引用

## 绝对禁止

你这轮绝对禁止以下动作：

1. 用任何旧备份、旧提交、旧 scratch scene、旧 validation 副本覆盖当前 `Primary.unity`
2. 运行任何 scene partial sync / scratch 回灌 / offline sync / validation copy，把旧 scene 内容灌回当前 `Primary`
3. 使用这些工具或同类动作：
   - `ScenePartialSyncTool`
   - `ScenePrimaryBackupScratchDryRunMenu`
   - `ScenePartialSyncValidationMenu`
   - 任何依赖 `Assets/__CodexSceneSyncScratch` 的 scene 回灌流程
4. 执行任何会把 `Primary.unity` 拉回旧版本的 Git 动作：
   - `git restore`
   - `git checkout --`
   - `git reset --hard`
   - 任何手工拷贝旧版文件覆盖当前磁盘版
5. 顺手修别的 manager、别的 scene、树脚本、字体、输入、Town、Home、TMP 资产
6. 以“清理现场”“顺手整理”为理由扩大 scope

## 稳定性防炸协议

如果阶段 B 被打开，在你第一次写 `Primary` 之前，必须先做下面 4 件事：

1. 从“当前磁盘版 `Primary.unity`”导出一个新鲜快照到 gitignored 路径，例如：
   - `.codex/artifacts/primary-repair-snapshots/<timestamp>/Primary.unity`
2. 记录这份当前磁盘版 `Primary.unity` 的：
   - 时间戳
   - 文件大小
   - `Get-FileHash` 结果
3. 明确这份快照的用途只是“回退证据”和“diff 对照基线”
4. 明确这份快照绝对不能再反向覆盖 `Assets/000_Scenes/Primary.unity`

## 出现以下任一迹象时，立刻停手

如果 Unity 或你的操作过程中出现以下任一情况：

1. `reload / external changes / reserialize whole scene` 提示异常
2. 看起来像要把 scene 拉回古早版本
3. 你一保存，diff 里出现大量与本轮无关的旧对象回潮、异常删除或成片替换
4. Unity 弹出会吞掉用户现场的 reload/revert 类选择框

你必须：

1. 立刻停手
2. 不保存
3. 不替用户点会吞现场的选项
4. 直接报 blocker，并把异常提示、当前 hash、快照路径、diff 特征一起报上来

## 你必须先交的五段分析

在任何真实写入之前，你必须先按下面 5 段报一次：

1. 原有配置
2. 问题原因
3. 建议修改
4. 修改后效果
5. 对原有功能的影响

这里不能只写“缺了 manager，所以补一下”；你必须明确说明：

- 当前磁盘版 `Primary` 还保留了哪些用户现场
- 你准备新增的对象/组件到底是哪几个
- 你为什么判断这轮不该碰 `GameInputManager`
- 你为什么判断这轮不该用 backup/scratch 覆盖

## 完成定义

只有在下面条件成立时，这轮才算完成：

1. `Primary.unity` 仍然保留当前磁盘版用户现场，没有被回灌成旧版本
2. 场景文本里能重新找到：
   - `SeasonManager`
   - `m_Name: 'TimeManager '`
   - `TimeManagerDebugger` 对应脚本 GUID `45df3a1e671e38048a3353a77f40d1d1`
3. 如果用户批准你做 live 验证，则必须进一步确认：
   - `SeasonManager` 缺失链路 warning 不再出现
   - 方向键跳天 / 跳季恢复
   - 树木季节逻辑不再因为 `SeasonManager.Instance == null` 退回等待失败
4. 你要显式报出：
   - 我新增了什么
   - 我保留了什么
   - 我没碰什么
   - 我清掉了什么
   - 当前 own 路径是否 clean

## 固定回执格式

### A. 用户可读汇报层

必须先按这个顺序写：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么

### B. 技术审计层

然后再补：

1. 原有配置
2. 问题原因
3. 建议修改
4. 修改后效果
5. 对原有功能的影响
6. `changed_paths`
7. 当前快照路径与 hash
8. 验证结果
9. blocker 或 checkpoint

## 一句话底线

你这轮不准做任何整场景 restore / partial sync / scratch 回灌；只允许在当前磁盘版 `Primary.unity` 上做 additive-only 的 manager/debugger 链恢复。如果你发现自己必须改 `SeasonManager / TimeManager / TimeManagerDebugger` 之外的既有对象字段，先停，逐项报 exact field 和理由，等用户确认。

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
```
