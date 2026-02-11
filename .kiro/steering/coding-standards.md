---
inclusion: manual
priority: P2
keywords: [编码, 命名, Region, 代码组织, EventBus]
lastUpdated: 2026-02-09
---

# Sunset 项目编码规范

## 🔴 文档写入规范（最高优先级）

### 问题背景
一次性写入大量内容（超过50行）容易被系统中断（aborted），导致文档写入失败。

### 强制规则

**写长文档时必须使用"少量多次写入"策略：**

1. **首次写入**：使用 `fsWrite` 创建文件，只写入标题和前30-40行
2. **后续追加**：使用 `fsAppend` 分段添加内容，每次30-40行
3. **内容分段**：按章节/表格/代码块自然分段

### 示例

```
# 正确做法
1. fsWrite 创建文件 + 写入第一章（30行）
2. fsAppend 追加第二章（35行）
3. fsAppend 追加第三章（30行）
4. fsAppend 追加第四章（25行）

# 错误做法
1. fsWrite 一次性写入整个文档（200行）→ 容易被中断
```

### 适用场景
- 设计文档（design.md）
- 需求文档（requirements.md）
- 分析报告
- 交互矩阵
- 任何超过50行的文档

### 注意事项
- 每次写入前确认上一次写入成功
- 如果被中断，从断点继续追加
- 保持内容的逻辑完整性（不要在表格中间断开）

## 命名规范

### 类和接口
- 类名使用 PascalCase：`TimeManager`, `PlayerController`
- 接口名以 I 开头：`ITimeService`, `IPoolable`
- 抽象类以 Base 或 Abstract 开头：`BaseItem`, `AbstractEnemy`

### 方法和属性
- 公共方法使用 PascalCase：`GetCurrentSeason()`, `AddItem()`
- 私有方法使用 PascalCase：`CalculateTimeStep()`, `UpdateAnimation()`
- 属性使用 PascalCase：`CurrentSeason`, `IsMoving`

### 字段
- 私有字段使用 camelCase 加下划线前缀：`_currentTime`, `_isMoving`
- 序列化字段使用 camelCase：`[SerializeField] private float moveSpeed;`
- 常量使用 UPPER_CASE：`MAX_HEALTH`, `DEFAULT_SPEED`
- 静态只读字段使用 PascalCase：`public static readonly int MaxSlots = 20;`

### 事件
- 事件使用 On 前缀：`OnDayChanged`, `OnItemAdded`
- 事件参数类使用 Event 后缀：`DayChangedEvent`, `ItemAddedEvent`

### 文件和文件夹
- 脚本文件名与类名一致：`TimeManager.cs`
- 文件夹使用 PascalCase：`Service`, `Controller`, `Data`

## 代码组织

### 类成员顺序
```csharp
public class ExampleClass : MonoBehaviour
{
    #region 常量
    private const int MAX_COUNT = 100;
    #endregion
    
    #region 静态成员
    public static ExampleClass Instance { get; private set; }
    public static event Action OnSomething;
    #endregion
    
    #region 序列化字段
    [Header("设置")]
    [SerializeField] private float speed = 5f;
    #endregion
    
    #region 私有字段
    private bool _isActive;
    private List<Item> _items = new List<Item>();
    #endregion
    
    #region 公共属性
    public bool IsActive => _isActive;
    #endregion
    
    #region Unity生命周期
    private void Awake() { }
    private void Start() { }
    private void Update() { }
    private void OnDestroy() { }
    #endregion
    
    #region 公共方法
    public void DoSomething() { }
    #endregion
    
    #region 私有方法
    private void HelperMethod() { }
    #endregion
    
    #region 编辑器方法
    #if UNITY_EDITOR
    [ContextMenu("测试")]
    private void DEBUG_Test() { }
    #endif
    #endregion
}
```

### 使用 Region 分组
- 必须使用 `#region` 组织代码
- 常用分组：常量、静态成员、序列化字段、私有字段、公共属性、Unity生命周期、公共方法、私有方法、编辑器方法

## 事件系统规范

### 使用 EventBus（推荐）
```csharp
using Sunset.Events;

// 订阅事件
void OnEnable()
{
    EventBus.Subscribe<DayChangedEvent>(OnDayChanged, priority: 50, owner: this);
}

void OnDisable()
{
    EventBus.Unsubscribe<DayChangedEvent>(OnDayChanged);
}

// 发布事件
EventBus.Publish(new DayChangedEvent { Year = 1, SeasonDay = 5, TotalDays = 35 });
```

### 保留静态事件（兼容）
- 现有静态事件暂时保留
- 新功能优先使用 EventBus
- 逐步迁移到 EventBus

## 服务定位器规范

### 注册服务
```csharp
using Sunset.Services;

// 在 Awake 或 Start 中注册
ServiceLocator.Register<ITimeService>(this, ServiceScope.Global);
```

### 获取服务
```csharp
// 获取服务
var timeService = ServiceLocator.Get<ITimeService>();

// 安全获取
if (ServiceLocator.TryGet<ITimeService>(out var service))
{
    // 使用服务
}
```

## 对象池规范

### 实现 IPoolable 接口
```csharp
using Sunset.Pool;

public class DroppedItem : MonoBehaviour, IPoolable
{
    public void OnSpawn()
    {
        // 初始化/重置状态
    }
    
    public void OnDespawn()
    {
        // 清理状态
    }
}
```

### 使用对象池
```csharp
// 获取或创建池
var pool = PoolManager.Instance.GetOrCreatePool(prefab, initialSize: 10);

// 从池获取
var item = pool.Spawn(position, rotation);

// 返回池
pool.Despawn(item);
```

## 注释规范

### XML 文档注释
```csharp
/// <summary>
/// 时间管理器 - 管理游戏内时间流逝
/// </summary>
public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// 获取当前季节
    /// </summary>
    /// <returns>当前季节枚举值</returns>
    public Season GetSeason() { }
}
```

### 行内注释
```csharp
// ✅ 正确：解释为什么这样做
// 使用 Collider 底部而非 Transform 位置，因为这更准确地反映物体的实际位置
float sortingY = collider.bounds.min.y;

// ❌ 错误：解释代码做了什么（代码本身已经说明）
// 获取 Y 坐标
float y = transform.position.y;
```

## 调试规范

### 日志格式
```csharp
// 使用颜色标记不同类型的日志
Debug.Log($"<color=cyan>[TimeManager] 初始化完成</color>");
Debug.Log($"<color=yellow>[TimeManager] 警告信息</color>");
Debug.Log($"<color=red>[TimeManager] 错误信息</color>");
Debug.Log($"<color=green>[TimeManager] 成功信息</color>");
```

### 调试开关
```csharp
[Header("调试")]
[SerializeField] private bool showDebugInfo = false;

if (showDebugInfo)
{
    Debug.Log("调试信息");
}
```

## 性能规范

### 避免每帧分配
```csharp
// ❌ 错误：每帧创建新字符串
void Update()
{
    Debug.Log($"Position: {transform.position}");
}

// ✅ 正确：使用条件和间隔
private float _lastLogTime;
void Update()
{
    if (showDebugInfo && Time.time - _lastLogTime > 1f)
    {
        _lastLogTime = Time.time;
        Debug.Log($"Position: {transform.position}");
    }
}
```

### 使用对象池
- 频繁创建/销毁的对象必须使用对象池
- 包括：掉落物、粒子效果、子弹、敌人等

### 事件驱动替代 Update
```csharp
// ❌ 错误：每帧检查
void Update()
{
    if (TimeManager.Instance.GetHour() == 6)
    {
        // 早上6点的逻辑
    }
}

// ✅ 正确：订阅事件
void OnEnable()
{
    TimeManager.OnHourChanged += OnHourChanged;
}

void OnHourChanged(int hour)
{
    if (hour == 6)
    {
        // 早上6点的逻辑
    }
}
```

## 文件夹结构

```
Assets/Scripts/
├── Core/               # 核心框架
│   ├── Events/         # 事件系统
│   ├── Services/       # 服务定位器
│   └── Pool/           # 对象池
├── Service/            # 游戏服务
│   ├── Time/           # 时间系统
│   ├── Weather/        # 天气系统
│   ├── Inventory/      # 背包系统
│   └── Navigation/     # 导航系统
├── Controller/         # 控制器
│   ├── Player/         # 玩家控制
│   ├── NPC/            # NPC控制
│   └── Input/          # 输入管理
├── Data/               # 数据定义
│   ├── Items/          # 物品数据
│   ├── Enums/          # 枚举定义
│   └── Database/       # 数据库
├── UI/                 # UI系统
├── Farm/               # 农田系统
├── World/              # 世界系统
└── Utils/              # 工具类
```
