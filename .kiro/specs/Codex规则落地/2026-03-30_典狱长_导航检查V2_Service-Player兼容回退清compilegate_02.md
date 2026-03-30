# 2026-03-30_典狱长_导航检查V2_Service-Player兼容回退清compilegate_02

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查V2\2026-03-30_典狱长_导航检查V2_Service-Player根接盘回执_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_典狱长_导航检查V2_Service-Player根接盘开工_01.md`

你上一轮有一半判断是对的：

- `preflight` 的确真实失败了
- `own roots remaining dirty` 的确已经是 `0`

但你有一半判断不够准，必须立刻纠正：

- 这不是单纯“Service/Player 根里自己写坏了两行”
- 真实情况是：`Assets/YYY_Scripts/Service/Player` 当前新增调用，吃到了 **`Assets/YYY_Scripts/Data` 根里还没归仓的新 API**
- 所以这轮的正确表述不是“根内 compile gate，直接在原逻辑上往前修就行”
- 而是：**`Service/Player` 正在对 `Data` 根的未归仓 API 发生依赖漂移；你这轮要先证明这根能不能在不扩根的前提下自愈。**

## 一、当前已接受基线

下面这些事实已经被治理位复核过，不要再重讲旧叙事：

1. 你上一轮 `preflight` 结果属实：
   - `CanContinue=False`
   - `own roots remaining dirty 数量: 0`
   - blocker 是代码闸门，不是 same-root remaining dirty

2. 两条 compile gate 也属实：
   - `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs:100`
   - `Assets/YYY_Scripts/Service/Player/PlayerNpcRelationshipService.cs:68`

3. 但这两条红不是孤立根内事实，而是跨根 API 漂移：
   - 当前 working tree 的 `Assets/YYY_Scripts/Data/NPCRoamProfile.cs` 里有 `HasInformalConversationContent`
   - 当前 working tree 的 `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs` 里有 `Shift`
   - 但 `HEAD` 基线里这两个 API 都不存在

一句话翻成人话：

- 你现在卡住，不是因为 `Service/Player` 自己天然就必须扩到 `Data`
- 而是因为它当前引用了别人根里还没归仓的新接口

## 二、本轮唯一主刀

只做这一刀：

- **把 `Assets/YYY_Scripts/Service/Player` 这整根重新压回 `HEAD` 可编译兼容面**
- 目标不是“顺手把 `Data` 也带走”
- 目标是先证明：**这根能否不扩根就自己过掉 compile gate**

本轮允许施工范围仍然只有：

- `Assets/YYY_Scripts/Service/Player/**`
- `.codex/threads/Sunset/导航检查V2/**`

## 三、这轮绝对禁止

### 1. 不准扩到 `Assets/YYY_Scripts/Data/**`

尤其不准把下面两个文件一起吞并进白名单：

- `Assets/YYY_Scripts/Data/NPCRoamProfile.cs`
- `Assets/YYY_Scripts/Data/NPCRelationshipStage.cs`

原因很简单：

- 这轮的治理问题正是要判断 `Service/Player` 能不能先不依赖这两个未归仓 API
- 如果你直接把 `Data` 一起带上，这轮证据就废了

### 2. 不准回漂到其他根

继续禁止：

- `Assets/YYY_Scripts/Story/**`
- `Assets/Editor/**`
- `Assets/222_Prefabs/UI/Spring-day1/**`
- `Assets/000_Scenes/**`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/**`
- `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
- `Assets/YYY_Scripts/Service/Navigation/**`

### 3. 不准顺手扩大 NPC / informal chat 语义面

尤其不要把这轮包装成：

- “我要继续做 NPC informal chat”
- “我要一起把 `PlayerNpcChatSessionService.cs` 也补完整”
- “我顺手把 `Data` 根也整理了”

这轮不是 feature 推进。
这轮是 **compatibility rollback / compile gate cleanup**。

## 四、你这轮必须做的具体动作

### 1. 只修这两处依赖漂移

#### `PlayerNpcNearbyFeedbackService.cs`

当前多出来的是：

- 对 `roamProfile.HasInformalConversationContent` 的直接依赖

你要做的是：

- 让这个文件重新对 `HEAD` 兼容
- 优先选择 **根内兼容回退**

允许的方向只有两种：

1. 直接去掉这条新增依赖，先回到 `HEAD` 的可编译逻辑
2. 如果你能在 **不改 `Data/**`** 的前提下，用当前根内已有信息实现同等 guard，也可以

默认推荐第一种。不要为了保一条新 guard，把 `Data` 根拉进来。

#### `PlayerNpcRelationshipService.cs`

当前多出来的是：

- `AdjustStage(...)`
- 它内部直接调用了 `NPCRelationshipStageUtility.Shift(...)`

你要做的是：

- 保留 `AdjustStage(...)` 这个根内入口
- 但把内部实现改写成 **不依赖 `Shift`**

也就是说：

- 在 `PlayerNpcRelationshipService.cs` 里本地完成 stage 的数值平移、边界钳制和回写
- 只使用 `HEAD` 里已经有的基础能力，例如：
  - `GetStage(...)`
  - `SetStage(...)`
  - `NPCRelationshipStageUtility.Sanitize(...)`
  - `NPCRelationshipStageUtility.FromStoredValue(...)`

不要去改 `NPCRelationshipStage.cs`。

### 2. 修完后只允许重跑同一条 preflight

命令不变：

```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread 导航检查V2 -IncludePaths "Assets/YYY_Scripts/Service/Player,.codex/threads/Sunset/导航检查V2"
```

### 3. 只有 preflight 通过，才允许继续 sync

```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread 导航检查V2 -IncludePaths "Assets/YYY_Scripts/Service/Player,.codex/threads/Sunset/导航检查V2"
```

## 五、这轮完成定义

你这轮只接受两种结果：

### A｜根内兼容回退成功

- 你没有扩到 `Data/**`
- 你只在 `Service/Player/**` 内消掉了这两条 compile gate
- 你真实重跑了 `preflight`
- 如果通过，再真实 `sync`
- 如果已 `sync`，必须给提交 SHA

### B｜已证明无法根内自愈

只有满足以下全部条件，才允许报 `B`：

- 你已经先按上面要求尝试根内兼容回退
- 你没有改 `Data/**`
- 你真实重跑了 `preflight`
- 仍然存在新的第一真实 blocker
- 并且这个 blocker 能明确证明：**不扩到 `Data/**` 就过不去**

不接受这种假 `B`：

- 只是把旧红报一遍
- 还没改就说“必须扩根”
- 顺手把 `Data` 一起改了再说“现在过了”

## 六、你这轮回执必须额外说清

继续按：

- `A1 保底六点卡`
- `A2 用户补充层`
- `停步自省`
- `B 技术审计层`

但这轮必须额外补 4 件事：

1. 你是否承认上一轮“纯根内 compile gate”表述不够准  
   你要明确写：真实问题是不是 `Service/Player -> Data` 的 API 漂移

2. `PlayerNpcNearbyFeedbackService.cs` 这轮用了哪种兼容回退  
   是去掉 guard，还是根内等价改写

3. `PlayerNpcRelationshipService.cs` 这轮如何避免依赖 `Shift`  
   要明确写是“本地 clamp 改写”，不是继续讲抽象理由

4. 本轮是否碰了 `Assets/YYY_Scripts/Data/**`  
   正确答案默认应为：`否`

## 七、你这轮最容易犯的错

- 一看见 working tree 里 `Data` 已经有 API，就默认把它当基线
- 直接把 `NPCRoamProfile.cs` 和 `NPCRelationshipStage.cs` 一起吞并
- 把“兼容回退”做成“继续推进 NPC informal chat feature”
- 还没尝试根内回退，就先宣称“必须 cross-root”
- 回执里继续把问题说成“根里两条普通 compile error”，不承认依赖漂移

一句话收口：

- **你这轮不是继续扩功能，而是先把 `Assets/YYY_Scripts/Service/Player` 压回一个不依赖 `Data` 新 API 的可归仓面。**
