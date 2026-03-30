# 2026-03-30｜spring-day1V2｜断开 DialogueDebugMenu 对 NPC 菜单编译耦合回执｜02

## A1 保底六点卡

1. 当前主线

- 我接受这次改判。
- 这轮不是继续把 blocker 交回治理位，而是作为 `Spring integrator`，在允许根内用最小 decouple 修掉 `Assets/Editor/Story/DialogueDebugMenu.cs` 对 NPC 菜单锁 key 的编译期直接引用。

2. 这轮实际做成了什么

- 只在 `Assets/Editor/Story/DialogueDebugMenu.cs` 内做了最小 decouple：
  - 新增本地常量：
    - `NpcInformalChatValidationLockKey = "Sunset.NpcInformalChatValidation.Active"`
  - 将：
    - `NPCInformalChatValidationMenu.ExclusiveValidationLockKey`
    替换为本地常量引用
- 没有碰：
  - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - 任意 UI peeled roots
- 修完后已真实重跑同一组 `preflight`，结果恢复为：
  - `CanContinue = True`
  - `代码闸门通过 = True`

3. 现在还没做成什么

- 本文件落盘时，最终 `sync` 还未执行。
- 这轮 commit SHA 还没生成。

4. 当前阶段

- 当前阶段是：
  - 根内最小 decouple 已完成
  - 同组 `preflight` 已恢复通过
  - 下一步只剩同白名单 `sync`

5. 下一步只做什么

- 下一步只做同白名单 `sync`，不再改实现，不再扩根。

6. 需要用户现在做什么

- 无。

## A2 用户补充层

### 1. 是否接受这次改判

- `是`
- 当前 blocker 已经证明不是“必须交别根 owner”的问题，而是 `Assets/Editor/Story` 根内的一条细编译耦合，可以在当前切片内最小修掉。

### 2. 具体怎样断开了编译耦合

- 不是空泛“做了解耦”。
- 具体做法是：
  - 在 [DialogueDebugMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/DialogueDebugMenu.cs) 内本地声明：
    - `private const string NpcInformalChatValidationLockKey = "Sunset.NpcInformalChatValidation.Active";`
  - 然后把 `EditorPrefs.GetBool(...)` 里的：
    - `NPCInformalChatValidationMenu.ExclusiveValidationLockKey`
    换成：
    - `NpcInformalChatValidationLockKey`
- 这样保留了原本的锁语义，但切断了对 `NPCInformalChatValidationMenu` 类型的编译期直接依赖。

### 3. 本轮是否碰了 Assets/Editor 直系根

- `否`
- 我没有改：
  - `Assets/Editor/NPCInformalChatValidationMenu.cs`
  - `Assets/Editor/NavigationStaticPointValidationMenu.cs`
  - `Assets/Editor/NPC.meta`

### 4. 修完后的 preflight 结果

- `CanContinue = True`
- `own roots remaining dirty = 0`
- `代码闸门通过 = True`

## 停步自省

- 自评：
  - `8.5/10`
- 做得最对的地方：
  - 这次没有再把一个可在根内切开的细编译耦合升级成“必须跨根改判”的治理问题
- 最薄弱点：
  - 这轮我只验证了这条耦合足以解除当前首个 blocker，还没看到 `sync` 真正跑完前的最终提交状态
- 最可能看错处：
  - 后续若还有新的 blocker，可能会暴露这包里第二层跨根耦合；但至少旧的 `CS0103` 已经不再是当前首 blocker

## B 技术审计层

### changed_paths

- `Assets/Editor/Story/DialogueDebugMenu.cs`
- `.codex/threads/Sunset/spring-day1V2/2026-03-30_断开DialogueDebugMenu对NPC菜单编译耦合回执_02.md`
- `.codex/threads/Sunset/spring-day1V2/memory_0.md`
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`

### 验证状态

- 已真实运行 `preflight`：`yes`
- 已真实运行 `sync`：`待下一步执行`
- 当前修后状态：`CanContinue=True`

### 修后 preflight 命令

```powershell
$include = @(
  'Assets/YYY_Scripts/Story/Interaction',
  'Assets/YYY_Scripts/Story/Managers',
  'Assets/Editor/Story',
  'Assets/YYY_Tests/Editor',
  '.codex/threads/Sunset/spring-day1V2',
  '.kiro/specs/900_开篇/spring-day1-implementation'
)
& 'C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1' `
  -Action preflight `
  -Mode task `
  -OwnerThread spring-day1V2 `
  -IncludePaths $include
```

### 修后 preflight 核心结果

- `当前 HEAD = 41c25505`
- `是否允许按当前模式继续 = True`
- `own roots remaining dirty 数量 = 0`
- `代码闸门通过 = True`

### 是否触碰高危目标

- `否`

### blocker_or_checkpoint

- 当前旧 blocker：
  - `DialogueDebugMenu.cs -> NPCInformalChatValidationMenu` 的 `CS0103`
  - 已在根内最小 decouple 后解除
- 当前状态：
  - 可进入同白名单 `sync`
