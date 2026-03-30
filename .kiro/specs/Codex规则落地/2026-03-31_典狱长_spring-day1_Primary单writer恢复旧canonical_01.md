# 2026-03-31_典狱长_spring-day1_Primary单writer恢复旧canonical_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_Primary.unity删除面_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`

你这轮不是继续 UI，不是继续 Day1 剧情，不是继续讨论迁移语义。

你这轮唯一主刀只有一个：

- **把 `Assets/000_Scenes/Primary.unity` 旧 canonical path 恢复回来，作为 `Primary.unity` single-writer 第一刀。**

## 一、当前已接受基线

1. `UI-V1` 已经完成只读裁定：
   - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 不是最终 canonical path
   - 它只是迁移 sibling / 临时复制面
2. 当前仓库现场已经钉死：
   - `ProjectSettings/EditorBuildSettings.asset` 仍指向 `Assets/000_Scenes/Primary.unity`
   - `Assets/Editor/NPCAutoRoamControllerEditor.cs` 仍硬编码旧路径
   - working tree 当前删掉的是旧路径 `Assets/000_Scenes/Primary.unity(.meta)`
3. 这意味着这轮目标不是“确认新路径迁移成立”，而是：
   - **先把旧 canonical path 立回去**

## 二、这轮关于热文件锁的额外现实

这轮必须先正视一个真实 blocker，不准装作没看见：

1. 当前锁池里还有这把 active lock：
   - `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`
2. 其内容显示：
   - `owner_thread = NPC`
   - `task = primary-scene-takeover`
   - `expected_release_at = 2026-03-27T19:17:21+08:00`
3. 当前旧路径 scene 本体又正好被删掉了，所以直接跑：
   - `Check-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity'`
   当前会因为目标文件不存在而失败
4. 所以你这轮不能把它口头误判成“无锁可写”
5. 更准确的口径是：
   - **这是一个 `deleted canonical path + stale NPC lock` 的 hot-file 接盘面**

## 三、本轮唯一主刀

只做这一刀：

1. 先把旧路径 `Assets/000_Scenes/Primary.unity(.meta)` 恢复到 `HEAD` 基线
2. 不碰新路径 scene
3. 不碰 `Build Settings`
4. 不碰 `NPCAutoRoamControllerEditor.cs`
5. 不处理 same-GUID duplicate
6. 不顺手改 scene 内容

也就是说，这轮只做：

- **“恢复旧 canonical path”**

不是做：

- duplicate 清理
- GUID 重建
- 正式迁移
- Day1 / UI 业务续写

## 四、这轮允许纳入的范围

### 1. 场景面

- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Primary.unity.meta`

### 2. 线程 / 工作区记忆

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
- 如果你这轮有更新：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`

### 3. 锁证据

允许只读读取：

- `.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json`
- `.kiro/scripts/locks/Check-Lock.ps1`
- `.kiro/scripts/locks/Acquire-Lock.ps1`
- `.kiro/scripts/locks/Release-Lock.ps1`

## 五、这轮明确禁止

- 不准碰 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)`
- 不准改 `ProjectSettings/EditorBuildSettings.asset`
- 不准改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
- 不准改别的 scene / prefab / script
- 不准顺手处理 TMP 字体
- 不准继续把这题讲成 UI 迁移讨论
- 不准把 stale lock 误报成“当前无锁”

## 六、完成定义

你这轮只有两种合格结果：

### A｜真开工过线

1. 你已把旧路径 `Assets/000_Scenes/Primary.unity(.meta)` 恢复回 `HEAD` 基线
2. 本轮没有顺手改 scene 内容
3. 你已真实跑过同组 `preflight`
4. 如果 lock 现实允许，你已完成同组 `sync`
5. 你能给出提交 SHA

### B｜第一真实 blocker

如果 stale NPC lock 或别的 hot-file 现实仍挡住你：

1. 你只允许回第一真实 blocker
2. 必须说清：
   - 它是锁 blocker，还是别的 blocker
   - exact path
   - 现在为什么还不能写
   - 你已经做到哪一步

## 七、你这轮最容易犯的错

- 一看到旧路径被删，就直接漂去清 duplicate / 改 GUID
- 把 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 一起动掉
- 顺手改 `Build Settings` 或编辑器硬编码
- 因为锁脚本对删除目标报错，就口头写成“当前无锁”
- 把“恢复旧 canonical path”做成“直接重做整个 Primary 案”

## 八、固定回执格式

回执按这个顺序：

- `A1 保底六点卡`
- `A2 用户补充层`
- `B 技术审计层`

其中 `A1 保底六点卡` 仍必须逐项显式写：

1. `当前主线`
2. `这轮实际做成了什么`
3. `现在还没做成什么`
4. `当前阶段`
5. `下一步只做什么`
6. `需要用户现在做什么`

## 九、A2 用户补充层必须额外显式回答

你必须额外补 6 条：

1. `旧 canonical path 是否已恢复`
   - 只能写 `是 / 否`
2. `这轮是否触碰新路径 scene`
   - 只能写 `是 / 否`
3. `当前 stale NPC lock 是否仍阻断`
   - 只能写 `是 / 否`
4. `如果阻断，第一真实 blocker 是什么`
5. `这轮是否已经拿到提交 SHA`
   - 只能写 `是 / 否`
6. `这轮是否触碰任何超出白名单的文件`
   - 只能写 `是 / 否`

一句话收口：

- **这轮只把 `Assets/000_Scenes/Primary.unity` 旧 canonical path 恢复回来，不做别的。**
