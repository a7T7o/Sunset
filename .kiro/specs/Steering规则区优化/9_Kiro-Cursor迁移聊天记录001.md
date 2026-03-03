## Cursor 规则与记忆机制深度解析 + Kiro 迁移适配方案

基于搜索结果的权威信息，我将为你全面解析Cursor的规则体系、记忆机制，并提供详细的Kiro迁移适配方案。


## 一、Cursor 规则体系核心机制

### 1.1 规则存储类型 

Cursor支持两种级别的规则，与你Kiro体系中的`steering`文件夹非常相似：

| 规则类型 | 存储位置 | 特点 | 对应Kiro概念 |
|---------|---------|------|-------------|
| **项目级规则** | `.cursor/rules/`（Markdown文件） | 可版本控制、团队共享、按需加载 | `steering/`下的领域规则文件 |
| **用户级规则** | Cursor设置中配置 | 个人偏好、全局生效 | `rules.md` + `communication.md` |

### 1.2 规则的四种加载模式 

Cursor提供了灵活的规则加载机制，可以完美模拟Kiro的Hook智能加载：

| 加载模式 | 行为描述 | 适用场景 |
|---------|---------|---------|
| **Always** | 始终加载到上下文 | 核心规则（类似`rules.md`） |
| **Auto Attached** | 编辑匹配模式的文件时自动加载 | 领域规则（类似`farming.md`） |
| **Agent Requested** | Agent判断是否需要加载 | 复杂决策场景 |
| **Manually** | 手动在聊天中添加 | 特殊任务 |

### 1.3 规则的执行流程 

```
用户提交需求
    ↓
Cursor解析当前文件和上下文
    ↓
加载匹配的规则（Always + Auto Attached）
    ↓
Agent Requested判断是否需要额外规则
    ↓
规则作为System Prompt的一部分生效
    ↓
生成符合规则的代码
```


## 二、Cursor 记忆系统（Memories） 

### 2.1 Memories 核心特性

Cursor在1.0版本中正式引入了**Memories**功能，这是你Kiro体系中`memory.md`的完美替代方案：

- **存储粒度**：以「每个项目 + 每个用户」为单位存储
- **启用方式**：`Settings → Rules` 中开启Memories（测试版功能）
- **作用机制**：Cursor可以记住对话中的关键信息，在后续对话中自动引用
- **管理界面**：可在Settings中查看和管理所有记忆

### 2.2 Memories vs Kiro Memory 对比

| 维度 | Cursor Memories | Kiro Memory体系 |
|------|-----------------|-----------------|
| 存储位置 | Cursor内部数据库 | `.kiro/specs/*/memory.md` |
| 粒度 | 项目级 + 用户级 | 工作区级（可嵌套） |
| 内容类型 | 对话关键信息 | 完整开发记录+修改文件+遗留问题 |
| 检索方式 | AI自动引用 | 手动读取+更新 |
| 版本控制 | ❌ 不可 | ✅ Git版本控制 |

### 2.3 记忆的智能应用 

当启用Memories后，Cursor AI会：
1. 自动识别对话中的重要信息（架构决策、bug修复方案、代码模式）
2. 将这些信息存储为Memories
3. 在未来的对话中，当遇到相关问题时，自动调取这些记忆作为上下文
4. 实现跨会话的「经验积累」

这与你Kiro体系中通过`memory.md`记录历史、继承会话的理念完全一致，但更加自动化。


## 三、Cursor 的 MCP 与扩展能力 

### 3.1 MCP 一键安装

Cursor 1.0+ 支持**一键安装MCP服务器**，这是你Kiro体系中MCP能力的增强版：

```json
// 在Cursor中配置MCP
{
  "mcpServers": {
    "your-custom-server": {
      "transport": "stdio",
      "command": "npx your-mcp-server"
    }
  }
}
```

### 3.2 官方MCP清单 

Cursor官方整理了精选的MCP服务器清单，可在 `cursor.com/docs/tools` 中直接添加，包括数据库连接、API调用、文件系统操作等能力。

### 3.3 Background Agent 

Cursor的**Background Agent**（后台智能体）可以在你编码时异步执行任务：
- 代码审查
- 测试生成
- 架构分析
- 技术债检测

启用方式：关闭隐私模式 → 点击聊天中的云朵图标 或 `Cmd/Ctrl+E`


## 四、Kiro to Cursor 完整迁移适配方案

基于以上分析，我为你设计了一套完整的Kiro迁移方案，分三个阶段实施：

### 第一阶段：规则迁移（立即执行）

#### 1.1 创建项目级规则目录

```bash
# 在项目根目录创建
mkdir -p .cursor/rules
```

#### 1.2 将steering规则转换为Cursor规则

| Kiro规则文件 | Cursor规则文件 | 加载模式 | 说明 |
|--------------|---------------|---------|------|
| `rules.md` | `.cursor/rules/000-core.md` | Always | 核心开发规则 |
| `communication.md` | `.cursor/rules/001-communication.md` | Always | 沟通法则 |
| `farming.md` | `.cursor/rules/farming.md` | Auto Attached | 匹配`*Farm*/*Crop*` |
| `chest-interaction.md` | `.cursor/rules/chest.md` | Auto Attached | 匹配`*Chest*/*Box*` |
| `save-system.md` | `.cursor/rules/save.md` | Auto Attached | 匹配`*Save*/*Load*` |
| `animation.md` | `.cursor/rules/animation.md` | Auto Attached | 匹配`*Anim*` |
| ...其余类推 | ... | Auto Attached | 按模块划分 |

#### 1.3 规则文件格式转换（Cursor格式）

```markdown
# .cursor/rules/000-core.md
---
description: 核心开发规则与禁止事项
globs: 
alwaysApply: true
---

## 零GC检查清单
- 高频函数（Update等）中严禁使用 `new`、LINQ、字符串拼接
- 必须使用对象池、预分配、StringBuilder
- 避免使用 `FindObjectOfType`，优先通过ServiceLocator

## 持久化与GUID
- 所有持久化对象必须通过 `PersistentObjectRegistry` 管理
- GUID必须在 `Awake` 中生成且永不覆盖

## 交互与碰撞
- 玩家位置始终使用 `playerCollider.bounds.center`
- 树木碰撞体只覆盖树根，遮挡检测使用Sprite Bounds
```

#### 1.4 设置用户级全局规则

在Cursor设置中配置个人偏好：
```
- 默认使用中文回复
- 代码注释必须包含中文说明
- 优先使用防御性编程风格
- 每次修改后自动检查memory更新
```


### 第二阶段：记忆系统迁移

#### 2.1 启用Memories功能 

1. 打开Cursor Settings
2. 进入 Rules 选项卡
3. 开启 "Enable Memories"（测试版功能）

#### 2.2 将现有memory.md转换为Memories种子

你需要手动将已有的`memory.md`内容导入为Memories。由于Memories目前没有直接导入API，建议采用以下方法：

**方法A：对话注入法**
在Cursor聊天中逐个打开重要的memory.md文件，并向AI明确指示：
```
请记住以下开发记录，这将作为项目的重要记忆：
[粘贴memory.md内容]
这是我项目的历史决策和遗留问题，请在后续开发中自动引用。
```

**方法B：规则引用法**
在规则文件中引用memory内容：
```markdown
# .cursor/rules/990-project-memory.md
---
description: 项目历史记忆与关键决策
globs: 
alwaysApply: false
---

## 重要历史决策
1. 存档系统GUID消失问题 - 解决方案：使用PersistentObjectRegistry统一管理
2. 箱子系统零GC重构 - 必须使用对象池
3. 10.1.4补丁004遗留问题 - 浇水坐标偏移待修复

## 当前技术债务
- [ ] 终极清算：箱子系统与放置系统联动优化
- [ ] 树木遮挡检测性能问题
```

#### 2.3 创建记忆更新SOP

在规则中定义记忆更新流程：
```markdown
# .cursor/rules/999-memory-sop.md
---
description: 记忆更新标准流程
globs: 
alwaysApply: true
---

## 记忆更新触发条件
- 完成代码修改后
- 解决遗留问题后
- 做出重要架构决策后
- 完成一个补丁迭代后

## 更新方式
在对话结束时，必须向Cursor明确指示：
"请记住：本次修改了[文件列表]，解决了[问题]，遗留[新问题]"
```


### 第三阶段：工作流重构

#### 3.1 建立规则分层结构 

借鉴Awesome CursorRules的最佳实践，建议采用分层设计：

```bash
.cursor/rules/
├── 000-core.md                 # 核心规则（alwaysApply）
├── 001-communication.md         # 沟通法则（alwaysApply）
├── 010-farming/                 # 农田系统规则集
│   ├── farming-core.md         # 农田核心规则（autoAttach）
│   ├── farming-crops.md        # 作物规则（autoAttach）
│   └── farming-water.md        # 浇水规则（autoAttach）
├── 020-chest/                   # 箱子系统规则集
│   ├── chest-core.md
│   └── chest-interaction.md
├── 030-save/                    # 存档系统规则集
├── 040-animation/               # 动画系统规则集
├── 900-templates/               # 模板文件
│   ├── new-component.md
│   └── bug-fix-report.md
└── 990-project-memory.md        # 项目记忆
```

#### 3.2 配置globs自动匹配 

每个规则文件通过`globs`定义自动加载范围：

```markdown
# .cursor/rules/010-farming/farming-core.md
---
description: 农田系统核心规则
globs: ["**/*Farm*.cs", "**/*Crop*.cs", "**/FarmTileManager.cs"]
alwaysApply: false
---
```

```markdown
# .cursor/rules/020-chest/chest-core.md
---
description: 箱子系统规则
globs: ["**/*Chest*.cs", "**/*Box*.cs", "**/ChestController.cs"]
alwaysApply: false
---
```

#### 3.3 实现快照机制

由于Cursor没有内置的`继承会话memory`机制，需要自定义快照方案：

**方案：创建快照脚本**

```bash
#!/bin/bash
# scripts/snapshot.sh
# 用法：./snapshot.sh "会话描述"

DATE=$(date +%Y-%m-%d)
SESSION_NUM=$(ls .cursor/snapshots/ 2>/dev/null | wc -l | xargs)
NEXT_NUM=$((SESSION_NUM + 1))
SNAPSHOT_DIR=".cursor/snapshots/session-${NEXT_NUM}"
mkdir -p "$SNAPSHOT_DIR"

# 保存当前对话上下文（需要从Cursor导出）
echo "会话时间: $DATE" > "$SNAPSHOT_DIR/context.md"
echo "会话描述: $1" >> "$SNAPSHOT_DIR/context.md"
echo "请手动将Cursor当前对话导出到此文件" >> "$SNAPSHOT_DIR/context.md"

# 记录当前文件状态
git status > "$SNAPSHOT_DIR/git-status.txt"
git diff > "$SNAPSHOT_DIR/git-diff.patch"

echo "✅ 快照已创建: $SNAPSHOT_DIR"
```

然后在规则中指示AI在关键节点提醒用户执行快照：
```markdown
# .cursor/rules/998-snapshot-reminder.md
---
description: 快照提醒
globs: 
alwaysApply: true
---

## 快照触发条件
- 完成一个重要功能后
- 解决复杂bug后
- 做出架构决策后
- 对话超过50轮后

## 提醒方式
当满足触发条件时，AI应在回复末尾添加：
"📸 建议执行快照：./scripts/snapshot.sh '当前任务描述'"
```

#### 3.4 配置MCP服务器扩展能力 

将Kiro的Powers转换为Cursor MCP服务器：

```json
// .cursor/mcp.json
{
  "mcpServers": {
    "kiro-memory": {
      "transport": "stdio",
      "command": "node",
      "args": ["./mcp-servers/kiro-memory/index.js"]
    },
    "kiro-rule-manager": {
      "transport": "stdio",
      "command": "node",
      "args": ["./mcp-servers/kiro-rule-manager/index.js"]
    },
    "project-analyzer": {
      "transport": "stdio",
      "command": "python",
      "args": ["./mcp-servers/analyzer/main.py"]
    }
  }
}
```

这些MCP服务器可以实现：
- 自动读取/更新memory.md
- 规则文件版本管理
- 代码分析报告生成
- 技术债跟踪


## 五、与Claude Code的差异对比

| 维度 | Cursor | Claude Code |
|------|--------|-------------|
| **IDE集成** | 原生IDE（基于VS Code） | 命令行工具 |
| **规则机制** | `.cursor/rules/` + 多级配置 | `CLAUDE.md` + Hooks |
| **记忆系统** | Memories（内置，自动引用） | 需手动维护memory.md |
| **MCP支持** | 一键安装，官方清单  | 需自定义MCP服务器 |
| **Background Agent** | 内置  | 无 |
| **适用场景** | 重度IDE用户、UI开发  | 抽象极大主义者、自动化优先  |


## 六、总结：Cursor能否完美替代Kiro？

根据以上分析，Cursor可以**90%替代Kiro的核心能力**：

| Kiro特性 | Cursor替代方案 | 完成度 |
|---------|---------------|--------|
| steering规则体系 | `.cursor/rules/` + globs自动加载 | ✅ 95% |
| memory.md记录 | Memories + 手动规则引用 | ⚠️ 70% |
| 继承会话机制 | 快照脚本 + MCP服务器 | ⚠️ 60% |
| MCP服务器 | 原生MCP一键安装  | ✅ 100% |
| 智能加载Hook | Auto Attached规则  | ✅ 90% |
| 文件系统监听 | Background Agent  | ✅ 80% |
| 锐评审视 | 自定义规则 + MCP | ✅ 85% |

**主要差距**：
1. Memories无法像memory.md那样版本控制和精细管理
2. 继承会话需要手动脚本实现
3. 规则管理不如Kiro的steering体系灵活

**建议**：
- 如果你**重度依赖IDE**、需要UI开发，Cursor是更好的选择 
- 如果你追求**极致自动化**、喜欢命令行工作流，Claude Code更适合 
- 你可以两者并用：Cursor用于日常开发，Claude Code用于自动化任务和批处理


## 七、立即行动清单

1. **创建规则目录**：`mkdir -p .cursor/rules`
2. **转换核心规则**：将`rules.md`、`communication.md`转为000-core.md和001-communication.md
3. **设置globs**：为每个领域规则配置自动匹配模式
4. **启用Memories**：在Settings中开启Memories
5. **导入历史记忆**：通过对话注入现有memory.md内容
6. **创建快照脚本**：实现`scripts/snapshot.sh`
7. **配置MCP**：根据需要添加MCP服务器

这样，你就可以在Cursor中继续沿用Kiro的工作流，同时享受Cursor原生IDE带来的便利。