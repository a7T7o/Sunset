# 需求文档：春1日剧情实现

## 简介

实现 Unity 2D 像素风独立游戏的春季第1天完整剧情流程。春1日是玩家进入游戏世界的第一天，包含开场CG、NPC对话、移动教学、UI首次展示（血条/精力条）、耕种教学、砍树教学等核心内容。本需求聚焦于需要新搭建的5个管线系统（对话系统、事件触发系统、CG/过场系统、UI教学提示、NPC引导），以及将这些系统与已有系统（农田、精力、血量、制作、时间、物品）串联起来驱动春1日剧情。

## 术语表

- **对话系统（Dialogue_System）**：负责显示对话框、NPC头像、对话文本推进的系统
- **事件触发系统（Event_Trigger_System）**：根据时间、位置、交互、条件等触发剧情事件的系统
- **CG系统（CG_System）**：负责播放开场动画、黑屏淡入淡出、文字叠加等过场演出的系统
- **教学提示系统（Tutorial_Prompt_System）**：在屏幕上显示按键提示和操作引导的系统
- **NPC引导系统（NPC_Guide_System）**：控制NPC沿路径移动、等待玩家跟随的系统
- **剧情管理器（Story_Manager）**：管理春1日整体剧情流程推进、场景状态切换的顶层控制器
- **乱码文本（Garbled_Text）**：语言转换前显示的不可读符号文字
- **精力条（Stamina_Bar）**：显示玩家精力值的UI组件（蓝色）
- **血条（Health_Bar）**：显示玩家血量值的UI组件（红色）
- **落日村（Sunset_Village）**：游戏中玩家所在的村庄
- **玩家位置**：玩家 Collider 的中心点（`playerCollider.bounds.center`）

## 需求

### 需求1：对话系统

**用户故事**：作为玩家，我希望能看到NPC的对话内容和头像，以便理解剧情和角色关系。

#### 验收标准

1. WHEN 剧情触发一段对话时，THE Dialogue_System SHALL 在屏幕下方显示对话框，包含说话者名称、头像和对话文本
2. WHEN 对话文本正在逐字显示时，THE Dialogue_System SHALL 以可配置的速度逐字打出文本（打字机效果）
3. WHEN 玩家在文本逐字显示过程中按下确认键时，THE Dialogue_System SHALL 立即显示当前句的完整文本
4. WHEN 当前句文本已完整显示且玩家按下确认键时，THE Dialogue_System SHALL 推进到下一句对话
5. WHEN 对话序列的最后一句显示完毕且玩家按下确认键时，THE Dialogue_System SHALL 关闭对话框并恢复玩家控制
6. WHILE 对话框处于显示状态时，THE Dialogue_System SHALL 禁用玩家移动和交互输入
7. WHEN 对话节点标记为内心独白时，THE Dialogue_System SHALL 使用区别于普通对话的视觉样式（如斜体或不同颜色）显示文本

### 需求2：语言乱码转换

**用户故事**：作为玩家，我希望在首次遇到NPC时经历语言从乱码到正常文字的转换过程，以便感受主角融入异世界的体验。

#### 验收标准

1. WHILE 剧情变量 `languageDecoded` 为 false 时，THE Dialogue_System SHALL 将马库斯的首段对话文本显示为乱码符号（如"◆■▲●？！"）
2. WHEN 乱码对话播放完毕后触发记忆闪回事件时，THE Story_Manager SHALL 将 `languageDecoded` 设置为 true，此操作不可逆
3. WHEN `languageDecoded` 变为 true 后，THE Dialogue_System SHALL 将所有后续对话文本以正常中文显示
4. THE Dialogue_System SHALL 通过对话数据中的标记字段区分乱码文本和正常文本，而非在运行时动态转换

### 需求3：CG/过场系统

**用户故事**：作为玩家，我希望在游戏开场看到星空、流星坠落的过场演出，以便建立游戏世界的第一印象。

#### 验收标准

1. WHEN 游戏启动春1日时，THE CG_System SHALL 按顺序播放开场CG序列：黑屏淡入星空 → 流星坠落 → 主角昏迷渐黑
2. THE CG_System SHALL 支持黑屏淡入和淡出过渡效果，过渡时长可配置
3. THE CG_System SHALL 支持在黑屏或画面上叠加显示文字信息
4. WHILE CG序列正在播放时，THE CG_System SHALL 禁用所有玩家输入
5. WHEN CG序列播放完毕时，THE CG_System SHALL 通知 Story_Manager 以推进到下一个剧情阶段
6. THE CG_System SHALL 支持屏幕震动效果（用于流星撞击等场景）
7. THE CG_System SHALL 支持画面模糊效果的短时开启和关闭（用于主角醒来的主观视角）

### 需求4：事件触发系统

**用户故事**：作为开发者，我希望有一个灵活的事件触发系统，以便通过配置数据驱动春1日各场景的剧情推进。

#### 验收标准

1. THE Event_Trigger_System SHALL 支持以下四种触发方式：位置触发（玩家进入指定区域）、交互触发（玩家按键交互）、条件触发（剧情变量满足条件）、事件链触发（前置事件完成后自动触发）
2. WHEN 玩家位置（`playerCollider.bounds.center`）进入位置触发器的区域时，THE Event_Trigger_System SHALL 触发关联的事件
3. WHEN 一个事件被触发时，THE Event_Trigger_System SHALL 执行该事件关联的动作序列（如播放对话、切换场景、显示UI等）
4. THE Event_Trigger_System SHALL 支持为每个触发器配置"仅触发一次"或"可重复触发"
5. THE Event_Trigger_System SHALL 支持为触发器配置前置条件（如要求某个剧情变量为特定值）
6. WHEN 事件动作序列执行完毕时，THE Event_Trigger_System SHALL 更新相关剧情变量并通知 Story_Manager

### 需求5：教学提示系统

**用户故事**：作为玩家，我希望在需要学习新操作时看到按键提示，以便知道如何操作。

#### 验收标准

1. WHEN 剧情进入教学阶段时，THE Tutorial_Prompt_System SHALL 在屏幕上显示对应的按键提示文本（如"使用 WASD 移动"）
2. WHEN 玩家成功完成提示要求的操作时，THE Tutorial_Prompt_System SHALL 隐藏当前提示并通知事件系统教学步骤已完成
3. THE Tutorial_Prompt_System SHALL 支持分步教学，前一步完成后自动显示下一步提示
4. THE Tutorial_Prompt_System SHALL 支持以下教学类型：移动教学（检测WASD输入）、奔跑教学（检测Shift+移动）、交互教学（检测E键交互）、工具使用教学（检测工具操作完成）
5. WHILE 教学提示正在显示时，THE Tutorial_Prompt_System SHALL 保持提示可见直到玩家完成对应操作或剧情强制推进

### 需求6：NPC引导系统

**用户故事**：作为玩家，我希望NPC能带路引导我前往目的地，以便在不熟悉地图时知道该往哪走。

#### 验收标准

1. WHEN 剧情需要NPC引导玩家时，THE NPC_Guide_System SHALL 控制指定NPC沿预设路径点序列移动
2. WHILE NPC正在引导时，IF 玩家位置（`playerCollider.bounds.center`）与NPC的距离超过设定阈值，THEN THE NPC_Guide_System SHALL 暂停NPC移动并等待玩家靠近
3. WHEN 玩家靠近到阈值范围内时，THE NPC_Guide_System SHALL 恢复NPC沿路径移动
4. WHEN NPC到达路径终点时，THE NPC_Guide_System SHALL 通知事件系统引导完成
5. THE NPC_Guide_System SHALL 支持在路径点上配置停留事件（如到达某点后触发对话）

### 需求7：血条UI首次展示

**用户故事**：作为玩家，我希望在艾拉治疗时第一次看到血条UI，以便了解自己的生命值状态。

#### 验收标准

1. WHILE 春1日剧情开始到14:30治疗场景之前，THE Health_Bar SHALL 保持隐藏状态
2. WHEN 14:30治疗剧情触发时，THE Story_Manager SHALL 激活 Health_Bar 的显示
3. WHEN Health_Bar 首次显示时，THE Health_Bar SHALL 在屏幕左上角显示，初始值为 60/100
4. WHEN 艾拉治疗动作完成时，THE Health_Bar SHALL 以缓动动画将数值从 60/100 变化到 85/100
5. WHEN Health_Bar 首次出现后，THE Health_Bar SHALL 在后续游戏中持续可见

### 需求8：精力条UI首次展示

**用户故事**：作为玩家，我希望在第一次耕地后看到精力条UI，以便了解自己的体力状态。

#### 验收标准

1. WHILE 春1日剧情开始到15:10耕种教学之前，THE Stamina_Bar SHALL 保持隐藏状态
2. WHEN 玩家在耕种教学中成功开垦第一格农田时，THE Story_Manager SHALL 激活 Stamina_Bar 的显示
3. WHEN Stamina_Bar 首次显示时，THE Stamina_Bar SHALL 在血条下方显示，初始值为 80/200（反映坠落受伤导致的精力削减）
4. WHEN 玩家执行消耗精力的操作时，THE Stamina_Bar SHALL 实时更新显示当前精力值
5. WHEN Stamina_Bar 首次出现后，THE Stamina_Bar SHALL 在后续游戏中持续可见

### 需求9：低精力警告

**用户故事**：作为玩家，我希望在精力过低时收到警告，以便及时休息避免晕倒。

#### 验收标准

1. WHEN 玩家精力值降至20以下时，THE Stamina_Bar SHALL 切换为红色闪烁状态
2. WHEN 低精力状态触发时，THE Tutorial_Prompt_System SHALL 显示警告提示"精力过低！请尽快进食或休息，否则会晕倒。"
3. WHILE 玩家处于低精力状态时，THE Story_Manager SHALL 将玩家移动速度降低20%
4. WHEN 玩家精力值恢复到20以上时，THE Stamina_Bar SHALL 恢复正常蓝色显示，移动速度恢复正常

### 需求10：剧情流程管理

**用户故事**：作为开发者，我希望有一个顶层管理器控制春1日的整体剧情推进，以便各系统协调工作。

#### 验收标准

1. THE Story_Manager SHALL 按以下固定顺序管理春1日的剧情阶段：开场CG → 坠落与相遇（14:00）→ 初入村庄（14:20）→ 疗伤与血条（14:30）→ 工作台与闪回（14:45）→ 耕种教学（15:10）→ 晚餐与冲突（16:00）→ 归途与夜间提醒（17:00）→ 自由时段 → 睡觉结束
2. WHEN 一个剧情阶段的所有事件完成时，THE Story_Manager SHALL 自动推进到下一个阶段
3. WHILE 剧情处于脚本化阶段（开场CG到17:00归途）时，THE Story_Manager SHALL 控制游戏内时间按剧情节奏推进，而非实时流逝
4. WHEN 剧情进入自由时段（17:00之后）时，THE Story_Manager SHALL 恢复正常的游戏内时间流逝
5. WHEN 玩家与床交互触发睡觉时，THE Story_Manager SHALL 执行春1日结束流程：淡出画面、显示"春1日结束"文字、推进日历到春2日、重置精力到满值
6. IF 玩家在自由时段未在凌晨2点前睡觉，THEN THE Story_Manager SHALL 触发强制睡眠事件

### 需求11：场景过渡

**用户故事**：作为玩家，我希望在不同地点之间切换时有平滑的过渡效果，以便保持沉浸感。

#### 验收标准

1. WHEN 剧情需要切换场景（如从森林到村庄、从室外到小屋内）时，THE CG_System SHALL 播放黑屏淡出→淡入的过渡效果
2. WHILE 场景过渡正在进行时，THE CG_System SHALL 禁用玩家输入
3. WHEN 场景过渡完成时，THE CG_System SHALL 恢复玩家输入并通知 Story_Manager

### 需求12：NPC环境气泡对话

**用户故事**：作为玩家，我希望看到村民围观时头顶冒出的议论文字，以便感受村庄的生活氛围。

#### 验收标准

1. WHEN 剧情触发环境气泡对话时，THE Dialogue_System SHALL 在指定NPC头顶显示气泡文本
2. THE Dialogue_System SHALL 支持多个NPC同时或依次显示气泡文本
3. WHEN 气泡文本显示达到配置的持续时间后，THE Dialogue_System SHALL 自动隐藏该气泡
4. WHILE 环境气泡对话显示时，THE Dialogue_System SHALL 保持玩家可移动状态（不锁定玩家输入）

### 需求13：记忆闪回效果

**用户故事**：作为玩家，我希望在触摸工作台时体验记忆闪回，以便理解主角拥有跨时代知识的设定。

#### 验收标准

1. WHEN 玩家在工作台旁按下交互键时，THE CG_System SHALL 播放记忆闪回序列：白闪效果 → 闪回画面（图片序列或简单蒙太奇）→ 回到正常画面
2. WHEN 记忆闪回序列播放完毕时，THE Story_Manager SHALL 解锁基础制作配方
3. WHEN 配方解锁后，THE Tutorial_Prompt_System SHALL 显示提示"按 C 打开手工制作菜单"

### 需求14：耕种教学流程

**用户故事**：作为玩家，我希望在马库斯的指导下学会耕种的完整流程，以便掌握游戏的核心玩法。

#### 验收标准

1. WHEN 耕种教学阶段开始时，THE Story_Manager SHALL 将锄头和花菜种子添加到玩家背包
2. THE Tutorial_Prompt_System SHALL 按以下顺序显示教学步骤：`使用锄头开垦土地` → `选择种子播种` → `使用水壶浇水`
3. WHEN 玩家完成每个教学步骤时，THE Tutorial_Prompt_System SHALL 自动推进到下一步提示
4. WHEN 玩家完成8格完整种植流程（开垦+播种+浇水）时，THE Story_Manager SHALL 标记耕种教学为完成

### 需求15：砍树教学流程

**用户故事**：作为玩家，我希望学会砍树获取木材，以便为后续制作工具做准备。

#### 验收标准

1. WHEN 砍树教学阶段开始时，THE Tutorial_Prompt_System SHALL 显示提示"使用斧头砍伐树木"
2. WHEN 玩家成功砍倒树木并掉落木材时，THE Tutorial_Prompt_System SHALL 显示提示"拾取木材"
3. WHEN 玩家拾取木材后，THE Story_Manager SHALL 标记砍树教学为完成

### 需求16：晚餐精力恢复

**用户故事**：作为玩家，我希望在饭馆吃饭后恢复精力，以便体验精力管理的核心循环。

#### 验收标准

1. WHEN 晚餐剧情触发时，THE Story_Manager SHALL 恢复玩家精力值30点
2. WHEN 精力恢复时，THE Stamina_Bar SHALL 以动画效果显示精力值增加，精力条颜色从暗蓝变为亮蓝

### 需求17：剧情数据配置

**用户故事**：作为开发者，我希望春1日的对话内容、事件序列、触发条件等数据以配置文件形式存储，以便后续修改剧情时无需改动代码。

#### 验收标准

1. THE Story_Manager SHALL 从数据配置（ScriptableObject 或 JSON）中读取春1日的剧情阶段定义和事件序列
2. THE Dialogue_System SHALL 从数据配置中读取所有对话内容（说话者、头像引用、文本、类型标记）
3. THE Event_Trigger_System SHALL 从数据配置中读取触发器的类型、条件、关联动作
4. THE Tutorial_Prompt_System SHALL 从数据配置中读取教学步骤的提示文本和完成条件

### 需求18：骷髅兵威胁展示

**用户故事**：作为玩家，我希望在远处看到骷髅兵的身影，以便感受到这个世界的危险。

#### 验收标准

1. WHEN 坠落与相遇场景加载时，THE Story_Manager SHALL 在矿洞口远处放置骷髅兵，处于巡逻状态
2. THE Story_Manager SHALL 确保骷髅兵的感知半径小于骷髅兵与玩家初始位置的距离，使骷髅兵不会发现玩家
3. WHILE 春1日剧情进行时，THE Story_Manager SHALL 确保骷髅兵不触发战斗，仅作为视觉威胁展示

---

## 系统集成与依赖

### 与现有系统的集成点

#### 时间系统（TimeManager）
- **依赖接口**：`TimeManager.Instance.GetHour()`, `TimeManager.Instance.SetPaused(bool)`
- **集成点**：
  - Story_Manager 在脚本化阶段暂停时间流逝（`SetPaused(true)`）
  - 在自由时段恢复时间流逝（`SetPaused(false)`）
  - 监听 `TimeManager.OnHourChanged` 事件触发时间相关剧情（如14:00坠落、14:30治疗）
  - 检测凌晨2点触发强制睡眠

#### 精力系统（EnergySystem）
- **依赖接口**：`EnergySystem.Instance.CurrentEnergy`, `EnergySystem.Instance.RestoreEnergy(int)`
- **集成点**：
  - 监听 `EnergySystem.OnEnergyChanged` 事件更新精力条UI
  - 监听 `EnergySystem.OnEnergyDepleted` 事件触发晕倒
  - 检测精力值 < 20 触发低精力警告
  - 晚餐剧情调用 `RestoreEnergy(30)` 恢复精力
  - 睡觉时调用 `FullRestore()` 完全恢复

#### 农田系统（FarmTileManager）
- **依赖接口**：`FarmTileManager.Instance.CreateTile()`, `FarmTileManager.Instance.SetWatered()`
- **集成点**：
  - 监听农田创建事件，检测玩家完成第一次耕地
  - 监听浇水事件，检测玩家完成浇水教学
  - 耕种教学完成条件：8格农田已开垦+播种+浇水

#### 背包系统（InventoryService）
- **依赖接口**：`InventoryService.AddItem()`, `InventoryService.RemoveItem()`, `InventoryService.HasItem()`
- **集成点**：
  - 耕种教学开始时添加锄头（ID 0001）和花菜种子（ID 1001）
  - 砍树教学检测背包中是否有木材（ID 3001）
  - 制作教学检测配方解锁状态

#### 交互系统（GameInputManager）
- **依赖接口**：`GameInputManager.SetInputEnabled(bool)`
- **集成点**：
  - 对话/CG播放时禁用玩家输入（`SetInputEnabled(false)`）
  - 对话/CG结束时恢复玩家输入（`SetInputEnabled(true)`）
  - 监听交互事件（E键）触发工作台闪回、床铺睡觉

### 新系统间的依赖关系

```
Story_Manager (顶层控制器)
    ├── CG_System (过场演出)
    ├── Dialogue_System (对话显示)
    ├── Event_Trigger_System (事件触发)
    ├── Tutorial_Prompt_System (教学提示)
    └── NPC_Guide_System (NPC引导)
```

- **Story_Manager** 是唯一的顶层控制器，负责推进剧情阶段
- **Event_Trigger_System** 监听游戏事件，触发后通知 Story_Manager
- **CG_System / Dialogue_System** 播放完成后通知 Story_Manager 继续推进
- **Tutorial_Prompt_System** 检测玩家操作完成后通知 Story_Manager
- **NPC_Guide_System** 引导完成后通知 Story_Manager

---

## 核心数据结构

### 对话数据（DialogueData）

```csharp
[System.Serializable]
public class DialogueData
{
    public string dialogueId;           // 对话唯一ID（如"spring_day1_marcus_01"）
    public string speakerName;          // 说话者名称
    public Sprite speakerAvatar;        // 说话者头像
    public DialogueType type;           // 对话类型（Normal/Monologue/Bubble）
    public List<DialogueLine> lines;    // 对话句子列表
}

[System.Serializable]
public class DialogueLine
{
    public string text;                 // 对话文本
    public bool isGarbled;              // 是否为乱码文本
    public float typingSpeed;           // 打字速度（字符/秒，默认30）
    public AudioClip voiceClip;         // 语音片段（可选）
}

public enum DialogueType
{
    Normal,      // 普通对话（全屏对话框）
    Monologue,   // 内心独白（斜体样式）
    Bubble       // 环境气泡（NPC头顶）
}
```

### 事件数据（EventData）

```csharp
[System.Serializable]
public class EventData
{
    public string eventId;              // 事件唯一ID
    public EventTriggerType triggerType; // 触发类型
    public EventCondition condition;     // 触发条件
    public bool triggerOnce;            // 是否仅触发一次
    public List<EventAction> actions;   // 事件动作序列
}

public enum EventTriggerType
{
    Position,    // 位置触发（玩家进入区域）
    Interaction, // 交互触发（按E键）
    Condition,   // 条件触发（变量满足）
    EventChain   // 事件链触发（前置事件完成）
}

[System.Serializable]
public class EventCondition
{
    public string variableName;         // 剧情变量名
    public CompareOperator op;          // 比较运算符
    public int value;                   // 比较值
}

[System.Serializable]
public class EventAction
{
    public EventActionType type;        // 动作类型
    public string targetId;             // 目标ID（对话ID/场景名/物品ID等）
    public int intParam;                // 整数参数
    public string stringParam;          // 字符串参数
}

public enum EventActionType
{
    PlayDialogue,    // 播放对话
    PlayCG,          // 播放CG
    ShowTutorial,    // 显示教学提示
    AddItem,         // 添加物品
    SetVariable,     // 设置变量
    ChangeScene,     // 切换场景
    StartGuide       // 开始NPC引导
}
```

### 剧情阶段数据（StoryStageData）

```csharp
[System.Serializable]
public class StoryStageData
{
    public string stageId;              // 阶段ID（如"stage_opening_cg"）
    public string stageName;            // 阶段名称（如"开场CG"）
    public int gameHour;                // 游戏内时间（14表示14:00）
    public bool pauseTime;              // 是否暂停时间流逝
    public List<string> eventIds;       // 该阶段包含的事件ID列表
    public string nextStageId;          // 下一阶段ID
}
```

---

## 非功能性需求

### 性能要求

1. **对话系统响应时间**：玩家按下确认键后，对话推进延迟 < 100ms
2. **CG加载时间**：CG序列开始播放前的加载时间 < 2秒
3. **事件触发检测频率**：位置触发器每帧检测一次，不影响帧率（保持60fps）
4. **内存占用**：春1日所有对话数据、CG资源加载后内存增量 < 50MB

### 可维护性要求

1. **数据驱动**：所有对话文本、事件序列、触发条件必须以配置数据形式存储，修改剧情时无需改动代码
2. **模块解耦**：各系统（对话、CG、事件、教学）之间通过事件通信，避免直接引用
3. **调试支持**：提供剧情跳转工具，可在编辑器中直接跳转到任意剧情阶段进行测试
4. **日志记录**：所有剧情事件触发、阶段切换、变量修改必须记录日志，便于排查问题

### 可扩展性要求

1. **多日剧情支持**：系统设计必须支持后续添加春2日、春3日等剧情，无需重构核心框架
2. **多语言支持**：对话文本必须支持本地化，为后续多语言版本预留接口
3. **剧情分支**：事件系统必须支持条件分支，为后续多结局剧情预留扩展点
4. **自定义动作**：EventAction 必须支持通过反射或委托注册自定义动作类型

---

## 异常处理与边界情况

### 玩家跳过教学

**场景**：玩家在教学阶段意外完成了后续步骤（如先浇水再开垦）

**处理**：
- Tutorial_Prompt_System 检测到步骤乱序时，自动标记前置步骤为已完成
- 不阻止玩家操作，但仍按顺序显示教学提示

### 玩家卡在剧情中

**场景**：玩家在NPC引导阶段走错路，无法跟上NPC

**处理**：
- NPC_Guide_System 等待玩家超过30秒后，显示箭头指示NPC位置
- 等待超过60秒后，NPC传送到玩家附近，重新开始引导

### 对话数据缺失

**场景**：配置文件中引用的对话ID不存在

**处理**：
- Dialogue_System 检测到缺失时，显示占位文本"[对话数据缺失: {dialogueId}]"
- 记录错误日志，但不中断剧情流程

### 精力耗尽晕倒

**场景**：玩家在教学阶段精力耗尽晕倒

**处理**：
- Story_Manager 检测到晕倒事件时，暂停当前教学
- 播放晕倒动画后，传送玩家到床边，恢复50%精力
- 重新开始当前教学阶段

### 时间冲突

**场景**：玩家在自由时段操作导致错过剧情触发时间点

**处理**：
- Event_Trigger_System 检测到时间已过但事件未触发时，在下一个整点强制触发
- 例如：16:00晚餐剧情未触发，则在16:10强制触发

---

## 测试验收标准

### 功能测试

1. **对话系统测试**
   - 验证所有对话文本正确显示，无乱码（除设计的乱码文本）
   - 验证打字机效果流畅，按键跳过功能正常
   - 验证对话结束后玩家控制正确恢复

2. **CG系统测试**
   - 验证开场CG序列完整播放，无卡顿
   - 验证黑屏过渡效果平滑，时长符合预期
   - 验证记忆闪回效果正确触发

3. **事件触发测试**
   - 验证所有位置触发器在正确位置触发
   - 验证事件链按顺序正确执行
   - 验证"仅触发一次"的触发器不会重复触发

4. **教学系统测试**
   - 验证移动/奔跑/交互/工具使用教学正确检测玩家操作
   - 验证教学步骤按顺序推进
   - 验证教学完成后提示正确隐藏

5. **UI显示测试**
   - 验证血条在14:30首次显示，初始值60/100
   - 验证精力条在首次耕地后显示，初始值80/200
   - 验证低精力警告在精力<20时触发

### 集成测试

1. **完整剧情流程测试**
   - 从游戏启动到春1日结束，完整走一遍剧情
   - 验证所有剧情阶段按顺序触发
   - 验证时间控制正确（脚本化阶段暂停，自由时段恢复）

2. **系统联动测试**
   - 验证耕种教学与农田系统正确联动
   - 验证精力消耗与精力系统正确联动
   - 验证物品添加与背包系统正确联动

### 异常测试

1. **跳过测试**：尝试跳过教学步骤，验证系统不崩溃
2. **卡住测试**：故意走错路，验证NPC引导的容错机制
3. **晕倒测试**：在教学阶段耗尽精力，验证恢复机制
4. **数据缺失测试**：删除部分对话数据，验证占位文本显示

### 性能测试

1. **帧率测试**：剧情全程保持60fps，无明显掉帧
2. **内存测试**：剧情结束后内存增量 < 50MB
3. **加载测试**：CG加载时间 < 2秒

---

## 实现优先级

### P0（核心功能，必须实现）

1. Story_Manager - 剧情流程控制
2. Dialogue_System - 基础对话显示
3. Event_Trigger_System - 位置触发和交互触发
4. 血条/精力条首次展示
5. 时间系统集成（暂停/恢复）

### P1（重要功能，影响体验）

1. CG_System - 开场CG和场景过渡
2. Tutorial_Prompt_System - 教学提示
3. NPC_Guide_System - NPC引导
4. 记忆闪回效果
5. 低精力警告

### P2（增强功能，可后续迭代）

1. 环境气泡对话
2. 骷髅兵威胁展示
3. 剧情跳转调试工具
4. 语音片段播放
5. 打字机效果的高级配置（速度曲线、停顿）

---

## 技术风险与缓解措施

### 风险1：剧情流程复杂度高，容易出现状态混乱

**缓解措施**：
- 使用状态机模式管理剧情阶段
- 每个阶段有明确的进入/退出条件
- 提供可视化调试工具查看当前状态

### 风险2：对话数据量大，手动配置容易出错

**缓解措施**：
- 提供编辑器工具辅助配置对话数据
- 实现数据校验功能，检测缺失引用和格式错误
- 支持从Excel导入对话文本

### 风险3：时间控制与现有系统冲突

**缓解措施**：
- TimeManager 提供 `SetPaused()` 接口，明确暂停/恢复时机
- Story_Manager 在阶段切换时显式控制时间状态
- 添加日志记录时间控制的每次调用

### 风险4：多系统联动导致耦合度高

**缓解措施**：
- 使用事件总线（EventBus）进行系统间通信
- 避免直接引用其他系统的实例
- 定义清晰的接口契约

---

## Day1 场景承载约束（供 `scene-build` 使用）

### 场景模块拆分
- Day1 不应被当成单一大场景，而应拆成：开场/坠落/初遇区、入村过渡区、农舍院落主场景、小屋室内/治疗/床铺区、工作台/闪回点、农田/砍树教学区、晚餐/冲突区、归途/夜间提醒/自由时段承载区。
- 当前真正进入 `scene-build` 的核心对象，应聚焦在“农舍院落主场景 + 室内最小衔接 + 工作台 + 教学区”。

### `SceneBuild_01` 的正式身份
- `SceneBuild_01` 的正式身份是：春1日 `14:20 进入村庄` 后，到 `15:10 耕种/砍树教学` 前后的**住处安置与教学主场景**。
- 它不是整座村庄总图，不是矿洞口威胁区，也不是晚餐/冲突区。

### `SceneBuild_01` 的强制承载动作
- 东侧进入
- NPC 带入住处
- 院落中心站位对话
- 工作台交互与闪回
- 农田教学落点
- 砍树教学落点
- 回屋 / 室内衔接

### 禁止误扩边界
- 不要把 `SceneBuild_01` 继续扩成整村总览
- 不要把开场坠落 / 骷髅兵威胁段塞进院落主场景
- 不要把晚餐 / 饭馆 / 冲突段强塞进该场景
- 不要用泛装饰扩张替代剧情承载设计

### 正式交付口径
- 供 `scene-build` 直接施工的完整空间 brief，见：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\scene-build_handoff.md`
