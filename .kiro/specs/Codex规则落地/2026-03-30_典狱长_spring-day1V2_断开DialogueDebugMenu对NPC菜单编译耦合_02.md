# 2026-03-30_典狱长_spring-day1V2_断开DialogueDebugMenu对NPC菜单编译耦合_02

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1V2\2026-03-30_剩余SpringStory根接盘开工回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_spring-day1V2_剩余SpringStory根接盘开工_01.md`

你这轮报出来的 blocker 是真的，但你“下一步不该由我继续修，应交治理位改判别根”这句，我现在改判。

原因很直接：

- 当前 blocker 不是完整 NPC 功能逻辑跨根
- 也不是你必须去碰 `Assets/Editor/NPCInformalChatValidationMenu.cs`
- 真正卡住你的，只是 `Assets/Editor/Story/DialogueDebugMenu.cs` 里对一个 **EditorPrefs 锁 key 常量** 的编译时直接引用

也就是说，这不是“必须交给别根 owner 才能继续”的那种 blocker。

这是一个：

- **可以继续留在 `Assets/Editor/Story/**` 根内，用最小 decouple 修掉的编译耦合**

---

## 一、当前已接受基线

下面这些事实已经成立，不要重讲：

1. 你这轮身份已经是 `Spring integrator`，不是旧的字体止血停表线。
2. `UI-V1` peeled roots 继续对你禁触：
   - `Assets/YYY_Scripts/Story/UI/**`
   - `Assets/222_Prefabs/UI/Spring-day1/**`
   - `Assets/Resources/Story/**`
   - `.kiro/specs/UI系统/0.0.1 SpringUI/**`
3. 你上一轮的 `preflight` 结果属实：
   - `own roots remaining dirty = 0`
   - 第一真实 blocker = `Assets/Editor/Story/DialogueDebugMenu.cs:23`
   - `CS0103`：`NPCInformalChatValidationMenu` 不存在

治理位新增的关键判断只有一个：

- 这不是该停给 `NPC` 的 blocker
- 而是该由你在自己白名单根内先做最小 decouple 的 blocker

---

## 二、本轮唯一主刀

只做这一刀：

- **在 `Assets/Editor/Story/DialogueDebugMenu.cs` 内，断开它对 `NPCInformalChatValidationMenu.ExclusiveValidationLockKey` 的编译时直接引用**

然后立刻重跑你上一轮同一组 include paths 的 `preflight`。

如果通过，再同白名单 `sync`。

---

## 三、这轮允许你改的范围

本轮仍然只允许：

- `Assets/YYY_Scripts/Story/Interaction/**`
- `Assets/YYY_Scripts/Story/Managers/**`
- `Assets/Editor/Story/**`
- `Assets/YYY_Tests/Editor/**`
- `.codex/threads/Sunset/spring-day1V2/**`
- `.kiro/specs/900_开篇/spring-day1-implementation/**`

但实际推荐你改动的核心代码文件应只有：

- `Assets/Editor/Story/DialogueDebugMenu.cs`

如果为了记录本轮 blocker->修法->重跑结果，需要更新：

- `.codex/threads/Sunset/spring-day1V2/memory_0.md`
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`

也可以带上。

---

## 四、这轮绝对不要碰

### 1. 不准碰 `Assets/Editor/NPCInformalChatValidationMenu.cs`

这轮不是让你去吞 `Assets/Editor` 直系根。

### 2. 不准碰 `Assets/Editor/NavigationStaticPointValidationMenu.cs`

这不是你这轮的事。

### 3. 不准回漂去 UI peeled roots / Service / Data / 字体 / scene

继续禁止：

- `Assets/YYY_Scripts/Story/UI/**`
- `Assets/222_Prefabs/UI/Spring-day1/**`
- `Assets/Resources/Story/**`
- `Assets/YYY_Scripts/Service/Player/**`
- `Assets/YYY_Scripts/Data/**`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/**`
- `Assets/000_Scenes/Primary.unity`

### 4. 不准把这轮扩大成“重构 NPC validation 菜单体系”

你这轮只做 decouple，不做菜单迁移，不做 owner 重写，不做 Editor 根整理。

---

## 五、你这轮必须怎么修

### 目标

让 `DialogueDebugMenu.cs` 不再在编译期依赖 `NPCInformalChatValidationMenu` 这个类型。

### 推荐修法

直接在 `DialogueDebugMenu.cs` 内本地声明等价锁 key 常量，例如：

- `private const string NpcInformalChatValidationLockKey = "Sunset.NpcInformalChatValidation.Active";`

然后把：

- `NPCInformalChatValidationMenu.ExclusiveValidationLockKey`

替换成这个本地常量。

### 为什么我要求你这样修

因为从你给的直接证据已经足够看出：

- `NPCInformalChatValidationMenu` 被 `DialogueDebugMenu` 用到的，只有一条 `EditorPrefs` key 常量
- 这不是必须保留的强类型耦合
- 用本地常量断开编译依赖，比现在就去搬 `Assets/Editor` 直系根更窄、更快、更符合这轮 slice

---

## 六、修完后你必须做的验证

### 1. 重跑同一条 preflight

命令不变：

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

### 2. 只有 preflight 通过，才允许继续 sync

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
  -Action sync `
  -Mode task `
  -OwnerThread spring-day1V2 `
  -IncludePaths $include
```

---

## 七、这轮完成定义

你这轮只接受两种结果：

### A｜编译耦合已在根内断开，并成功 `preflight -> sync`

- 只在允许范围内修
- 没碰 `Assets/Editor` 直系根
- `preflight` 通过
- `sync` 成功
- 给出提交 SHA

### B｜仍有新的第一真实 blocker

但要满足：

- 你已经先按本轮要求做了 `DialogueDebugMenu.cs` 的 decouple
- 然后真实重跑了 `preflight`
- 新 blocker 不是把旧的 `CS0103` 原样报回来

不接受：

- 还没改就把锅交回治理位
- 把这轮扩大成 Editor 根搬家
- 顺手去碰 NPC 菜单文件

---

## 八、你这轮回执必须额外写清

继续按：

- `A1 保底六点卡`
- `A2 用户补充层`
- `停步自省`
- `B 技术审计层`

但这轮必须额外回答：

1. 你是否接受这次改判  
   也就是：这不是“必须交给别根处理”的 blocker，而是你当前根内可做的 decouple

2. `DialogueDebugMenu.cs` 具体怎样断开了对 `NPCInformalChatValidationMenu` 的编译耦合  
   必须写清是“本地常量替换”，不是空泛说“做了解耦”

3. 本轮是否碰了 `Assets/Editor` 直系根  
   正确答案默认应为：`否`

4. 修完后新的 `preflight` 结果是什么  
   必须写 `CanContinue=True/False`

---

## 九、你这轮最容易犯的错

- 继续坚持“这轮不该由我修”
- 顺手去碰 `NPCInformalChatValidationMenu.cs`
- 把常量 decouple 扩成菜单系统重构
- 改完后不重跑同一条 `preflight`
- 继续把旧 `CS0103` 当最终结论回卡

一句话收口：

- **这轮不是让你跨根修 NPC，而是让你在 `DialogueDebugMenu.cs` 里先断开这条细编译线，把 Spring Story 包继续往前推进。**
