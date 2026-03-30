# 2026-03-31｜NPC｜Primary 过期锁善后与旧 canonical 恢复回执｜01

## A1 保底六点卡

1. 当前主线

- 这轮不是继续 NPC 功能，不是继续 scene 主线，也不是接 `Primary` 整案。
- 这轮只做两件事：
  - 把旧 canonical path `Assets/000_Scenes/Primary.unity(.meta)` 恢复回 `HEAD` 基线
  - 释放我名下这把 stale `Primary` active lock

2. 这轮实际做成了什么

- 已先确认旧 canonical path 当前确实是删除面
- 已把：
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/000_Scenes/Primary.unity.meta`
  从 `HEAD` 原样恢复回来
- 已以 `NPC` owner 身份合法释放：
  - `.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json`
- 已生成新的 released history：
  - `.kiro/locks/history/A__Assets__000_Scenes__Primary.unity.lock.20260331-011357.released.json`
- 已确认这轮没有触碰任何新路径 scene，也没有改任何 scene 内容

3. 现在还没做成什么

- 本文件落盘时，`preflight` 和 `sync` 还没回填最终结果
- 提交 SHA 还没生成

4. 当前阶段

- 当前阶段是：
  - 物理善后已完成
  - 旧 canonical path 已回到 `HEAD` 基线
  - stale NPC active lock 已释放
  - 下一步只剩最小白名单 `preflight -> sync`

5. 下一步只做什么

- 下一步只做这组最小白名单的 `preflight`
- 如果过线，就直接 `sync`

6. 需要用户现在做什么

- 无

## A2 用户补充层

1. 旧 canonical path 是否已恢复

- `是`

2. stale NPC active lock 是否已释放

- `是`

3. 这轮是否触碰任何新路径 scene 或 scene 内容

- `否`

4. 当前 own 路径是否 clean

- `否`

5. 这轮是否拿到提交 SHA

- `否`

6. 如果没过线，第一真实 blocker 是什么

- `尚未出现；本文件落盘时还在进入 preflight 之前`

## B 技术审计层

### 关键命令与结果

```powershell
git restore --source=HEAD --worktree -- 'Assets/000_Scenes/Primary.unity' 'Assets/000_Scenes/Primary.unity.meta'
```

- 恢复后：
  - `git diff --name-status -- Assets/000_Scenes/Primary.unity Assets/000_Scenes/Primary.unity.meta`
  - 返回空结果，说明旧 canonical path 已回到 `HEAD` 基线

```powershell
powershell -ExecutionPolicy Bypass -File '.kiro/scripts/locks/Check-Lock.ps1' -TargetPath 'Assets/000_Scenes/Primary.unity'
```

- 释放前返回：
  - `state = locked`

```powershell
powershell -ExecutionPolicy Bypass -File '.kiro/scripts/locks/Release-Lock.ps1' `
  -TargetPath 'Assets/000_Scenes/Primary.unity' `
  -OwnerThread 'NPC' `
  -ReleasedBy 'NPC' `
  -ReleaseNote 'stale active lock cleanup after restoring canonical Primary scene to HEAD baseline'
```

- 释放返回：
  - `state = released`
  - `history_path = D:\Unity\Unity_learning\Sunset\.kiro\locks\history\A__Assets__000_Scenes__Primary.unity.lock.20260331-011357.released.json`

```powershell
powershell -ExecutionPolicy Bypass -File '.kiro/scripts/locks/Check-Lock.ps1' -TargetPath 'Assets/000_Scenes/Primary.unity'
```

- 释放后返回：
  - `state = unlocked`

### 本轮最小白名单（待 preflight / sync）

- `Assets/000_Scenes/Primary.unity`
- `Assets/000_Scenes/Primary.unity.meta`
- `.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json`
- `.kiro/locks/history/A__Assets__000_Scenes__Primary.unity.lock.20260331-011357.released.json`
- `.codex/threads/Sunset/NPC/memory_0.md`
- `.codex/threads/Sunset/NPC/2026-03-31_典狱长_NPC_Primary过期锁善后与旧canonical恢复回执_01.md`
