# 2026-03-31_典狱长_NPC_Primary过期锁善后与旧canonical恢复_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_Primary.unity删除面_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`

你这轮不是继续 NPC 功能，不是继续做 scene 主线，也不是让你顺手吞 `Primary` 整案。

你这轮唯一主刀只有一个：

- **把你名下这把 `Primary.unity` 过期 active lock 做合法善后，并把旧 canonical path [Assets/000_Scenes/Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity) 恢复回 `HEAD` 基线。**

## 一、当前已接受基线

1. `UI-V1` 已完成只读裁定：
   - `Assets/222_Prefabs/UI/Spring-day1/Primary.unity` 不是最终 canonical path
   - 它只是迁移 sibling
2. `spring-day1` 也已完成第一轮 single-writer 调查：
   - 旧 canonical path 可以从 `HEAD` 恢复
   - 但当前被一把 stale NPC active lock 阻断
3. 当前这把 active lock 的现实是：
   - 文件：`D:\Unity\Unity_learning\Sunset\.kiro\locks\active\A__Assets__000_Scenes__Primary.unity.lock.json`
   - `owner_thread = NPC`
   - `task = primary-scene-takeover`
   - `expected_release_at = 2026-03-27T19:17:21+08:00`
4. 当前旧路径 scene 本体又正好被删掉了，所以直接跑：
   - `Check-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity'`
   只会报：
   - `Target path does not exist`
5. 所以现在最先要做的，不是 duplicate 清理，也不是迁移讨论，而是：
   - **让这把锁和旧 canonical path 一起回到合法状态**

## 二、本轮唯一主刀

只做这一刀：

1. 把 `Assets/000_Scenes/Primary.unity`
2. 把 `Assets/000_Scenes/Primary.unity.meta`
3. 从 `HEAD` 原样恢复回来
4. 以 `NPC` owner 身份，合法释放这把 stale active lock

注意：

- 这轮恢复旧 canonical path，只允许是“恢复到 `HEAD` 基线”
- 不允许继续改 scene 内容
- 不允许碰新路径 scene

## 三、本轮允许纳入的范围

### 1. 场景与锁

- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Primary.unity.meta`
- `.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json`
- `.kiro/locks/history/*Primary*`
- `.kiro/scripts/locks/Check-Lock.ps1`
- `.kiro/scripts/locks/Release-Lock.ps1`

### 2. 线程 / 工作区记忆

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- 如果你这轮有更新：
  - NPC 工作区相关 `memory.md`

## 四、本轮明确禁止

- 不准碰 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity(.meta)`
- 不准改 `ProjectSettings/EditorBuildSettings.asset`
- 不准改 `Assets/Editor/NPCAutoRoamControllerEditor.cs`
- 不准顺手改 `Primary.unity` 场景内容
- 不准碰 TMP 字体
- 不准继续扩成 NPC scene 新需求
- 不准把这轮写成“我顺手接了 Primary 整案”

## 五、这轮实际动作顺序必须这样走

1. 先确认旧 canonical path 当前确实是删除面
2. 从 `HEAD` 恢复：
   - `Assets/000_Scenes/Primary.unity`
   - `Assets/000_Scenes/Primary.unity.meta`
3. 再以 `NPC` owner 身份运行锁释放：
   - `Release-Lock.ps1 -TargetPath 'Assets/000_Scenes/Primary.unity' -OwnerThread 'NPC' ...`
4. 然后对这组最小白名单跑 `preflight`
5. 如果过线，再 `sync`

不准先去改 scene 里面的任何内容。

## 六、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. 旧 canonical path 已从 `HEAD` 原样恢复
2. stale NPC active lock 已合法释放
3. 这轮没有顺手改任何 scene 内容
4. 真实跑过同组 `preflight`
5. 如果过线，真实跑过 `sync`
6. 你能给出提交 SHA

### B｜第一真实 blocker

如果你在恢复或释放锁时仍被挡住：

1. 只回第一真实 blocker
2. 必须说清：
   - 你已经做到哪一步
   - exact path 是什么
   - 为什么现在还不能把锁合法收掉

## 七、你这轮最容易犯的错

- 把“恢复旧 canonical path”做成“顺手继续改 Primary 场景”
- 直接去碰新路径 `Assets/222_Prefabs/UI/Spring-day1/Primary.unity`
- 顺手改 `Build Settings` 或编辑器硬编码
- 只释放锁，不恢复旧路径
- 只恢复旧路径，不释放锁
- 把这轮吹成“Primary 整案完成”

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
2. `stale NPC active lock 是否已释放`
   - 只能写 `是 / 否`
3. `这轮是否触碰任何新路径 scene 或 scene 内容`
   - 只能写 `是 / 否`
4. `当前 own 路径是否 clean`
   - 只能写 `是 / 否`
5. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
6. `如果没过线，第一真实 blocker 是什么`

一句话收口：

- **这轮只做两件事：恢复旧 canonical path，释放你名下这把 stale `Primary` 锁。**
