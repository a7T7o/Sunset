# 编辑器工具 - 开发记忆

## 模块概述

本工作区记录项目中所有编辑器工具的设计、功能和维护信息。这些工具用于提高开发效率，包括批量生成 SO、批量修改资产、动画生成等。

## 当前状态
- **完成度**: 95%
- **最后更新**: 2026-01-04
- **状态**: 已完成分类优化

## 工具清单

### 1. 批量物品 SO 生成工具 V2

| 属性 | 值 |
|------|------|
| 文件 | `Assets/Editor/Tool_BatchItemSOGenerator.cs` |
| 菜单 | Tools → 📦 批量生成物品 SO |
| 功能 | 从选中的 Sprite 批量生成物品 SO |
| 版本 | V2 - 大类+小类分类结构 |

**大类分类**：
| 大类 | 小类 | ID 范围 |
|------|------|---------|
| 工具装备 | 工具、武器 | 0XXX |
| 种植类 | 种子、作物 | 10XX-11XX |
| 可放置 | 树苗、工作台、存储、交互展示、简单事件 | 12XX-16XX |
| 消耗品 | 食物、药水 | 40XX, 50XX |
| 材料 | 矿石、锭、自然材料、怪物掉落 | 30XX-33XX |
| 其他 | 基础物品、家具、特殊物品 | 6XXX, 7XXX |

**支持的 SO 类型**：
- ItemData（基础物品）
- ToolData（工具）
- WeaponData（武器）
- SeedData（种子）
- SaplingData（树苗）
- CropData（作物）
- FoodData（食物）
- MaterialData（材料）
- PotionData（药水）
- WorkstationData（工作台）✨ 新增
- StorageData（存储容器）✨ 新增
- InteractiveDisplayData（交互展示）✨ 新增
- SimpleEventData（简单事件）✨ 新增

**工作流程**：
1. 在 Project 窗口选择 Sprite、Texture 或文件夹
2. 点击"获取选中项"按钮
3. 选择大类 → 选择小类
4. 配置 ID 和属性
5. 点击生成

**UI 设计（V2 更新）**：
- 数据库设置区：拖入 MasterItemDatabase
- Sprite 选择区：显示选中的 Sprite 列表和预计 ID
- 类型选择区：**两行布局 - 大类按钮 + 小类按钮**
- ID 设置区：连续 ID 模式和起始 ID（自动根据类型设置）
- 通用属性区：价格、堆叠、显示尺寸
- 类型专属区：根据选择的类型显示不同字段
- 输出设置区：输出文件夹选择（自动根据类型设置）

### 2. 批量物品 SO 修改工具

| 属性 | 值 |
|------|------|
| 文件 | `Assets/Editor/Tool_BatchItemSOModifier.cs` |
| 菜单 | Tools → 批量修改物品 SO |
| 功能 | 批量修改已有物品 SO 的属性 |

### 3. 动画生成工具

| 属性 | 值 |
|------|------|
| 文件 | `Assets/Editor/ToolAnimationGeneratorTool.cs` |
| 菜单 | Tools → 手持三向生成流程 → 工具动画一键生成 |
| 功能 | 从 Aseprite 源文件生成工具动画 |

### 4. 世界预制体生成工具

| 属性 | 值 |
|------|------|
| 文件 | `Assets/Editor/WorldPrefabGeneratorTool.cs` |
| 菜单 | Tools → 世界预制体生成器 |
| 功能 | 从物品 SO 生成世界掉落物预制体 |

### 5. 数据库同步助手

| 属性 | 值 |
|------|------|
| 文件 | `Assets/Editor/DatabaseSyncHelper.cs` |
| 功能 | 自动收集所有物品 SO 到数据库 |

## 同步规则

### 🔴 重要：SO 字段变更同步

当修改 ItemData 或其子类的字段时，必须同步更新以下工具：

1. **Tool_BatchItemSOGenerator.cs**
   - 添加/删除 UI 字段
   - 更新生成逻辑

2. **Tool_BatchItemSOModifier.cs**
   - 添加/删除修改选项

3. **相关 Editor 脚本**
   - 自定义 Inspector

### 同步检查清单

- [ ] 新增字段 → 批量生成工具添加 UI
- [ ] 删除字段 → 批量工具移除相关代码
- [ ] 修改字段类型 → 更新类型处理
- [ ] 新增 SO 子类 → 添加新类型支持

## 会话记录

### 会话 2 - 2026-01-04

**用户需求**:
> 批量生成工具的物品类型按钮直接根据 SO 的设计来分类，大类里面再放入小类的选项，方便检索

**完成任务**:
1. 重构 Tool_BatchItemSOGenerator.cs 为 V2 版本
2. 实现大类+小类的层级分类结构
3. 添加 4 种新的可放置物品类型支持（工作台、存储、交互展示、简单事件）
4. 自动设置 ID 范围和输出路径
5. 设置持久化支持

**修改文件**:
- `Assets/Editor/Tool_BatchItemSOGenerator.cs` - 完全重构为 V2 版本
- `.kiro/specs/编辑器工具/0_批量SO生成器分类优化/` - 创建 spec 文档

**解决方案**:
- 添加 ItemMainCategory 大类枚举（6 个大类）
- 扩展 ItemSOType 枚举（15 种小类）
- 使用静态 Dictionary 实现映射关系
- UI 采用两行按钮布局：大类 + 小类
- 切换大类时自动选中第一个小类并更新 ID/路径

---

### 会话 1 - 2026-01-04

**用户需求**:
> 创建编辑器工具工作区，记录批量 SO 生成工具等关键工具的设计和维护信息

**完成任务**:
1. 创建编辑器工具工作区
2. 记录现有工具清单
3. 建立 SO 同步规则

**修改文件**:
- `.kiro/specs/编辑器工具/memory.md` - 创建工作区记忆文档
- `.kiro/steering/so-design.md` - 添加 SO 工具同步规则

---

## 关键决策

| 决策 | 原因 | 日期 |
|------|------|------|
| SO 字段变更必须同步工具 | 保持工具与数据结构一致 | 2026-01-04 |

## 相关文件

| 文件 | 说明 |
|------|------|
| `Assets/Editor/Tool_BatchItemSOGenerator.cs` | 批量生成物品 SO |
| `Assets/Editor/Tool_BatchItemSOModifier.cs` | 批量修改物品 SO |
| `Assets/Editor/DatabaseSyncHelper.cs` | 数据库同步助手 |
| `Assets/Editor/WorldPrefabGeneratorTool.cs` | 世界预制体生成器 |
| `Assets/Editor/ToolAnimationGeneratorTool.cs` | 工具动画生成器 |

---

### 会话 3 - 2026-03-13

**主线目标**:
> 在 `main` 主项目里把 NPC 通用模板工具改成固定模板傻瓜版，让用户只导入 PNG 就能直接得到 Idle / Move、Controller 和 Prefab。

**本轮阻塞 / 子任务**:
1. 核对 `main` 上的真实代码是否已经同步到用户最新口径。
2. 复测 Unity MCP 是否恢复可用。
3. 修正工具与 NPC 运行时脚本之间的状态口径漂移。

**完成任务**:
1. 确认当前 `main` 的 `Assets/Editor/NPCPrefabGeneratorTool.cs` 仍是旧版长表单工具，并未对齐用户要求。
2. 在当前仓库里未发现独立的 NPC 生成器 spec，故本轮按“编辑器工具”工作区承接落盘，同时补建 `Sunset/NPC` 线程记忆。
3. 将 NPC 工具重写为固定模板版：选中 PNG / 文件夹后，一键生成 `Idle + Move + Controller + Prefab`，命名以图片名为基准，不再暴露 NPC 名称、动作映射、Death 等旧配置。
4. 清理 `NPCAnimController.cs` 与 `NPCMotionController.cs` 的 Death 逻辑，使运行时只保留 Idle / Move，保证生成器与挂载组件语义一致。
5. 复测 Unity MCP，`get_console_logs` 与 `recompile_scripts` 仍返回 `Connection failed: Unknown error`，因此本轮验证停留在代码回读、旧引用清零和 Git 白名单收口前检查。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 重写为固定模板 NPC 生成器
- `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` - 运行时仅保留 Idle / Move 状态
- `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` - 去除 Death 入口，保留运动检测与朝向桥接
- `.codex/threads/Sunset/NPC/memory_0.md` - 补建当前线程记忆入口

**当前恢复点**:
- 主线已回到“用户可在 `D:\Unity\Unity_learning\Sunset@main` 上直接测试 NPC 固定模板工具”的状态。
- 当前唯一未闭环点是 Unity MCP 断开导致的未编译验证；下一步应执行白名单 Git 同步并等待用户在 Unity 主项目里手测。

---

### 会话 4 - 2026-03-13

**主线目标**:
> 在 `main` 主项目里把 NPC 通用模板工具收口到用户最终确认的最简交互，并准备进入可同步、可手测状态。

**本轮阻塞 / 子任务**:
1. 复核 `main` 上的真实 UI 是否完全对齐用户最新口径。
2. 复测 Unity MCP 连接状态。
3. 仅补齐还没对齐的最小缺口。

**完成任务**:
1. 代码回读确认：固定模板生成链已成立，工具只生成 Idle / Move 两类动画及配套 Controller / Prefab。
2. 发现此前仍有一个交互细节未对齐：`获取选中项` 按钮在底部，不符合用户指定的顶部操作习惯。
3. 已将 `获取选中项` 移入“选中项”区顶部，底部操作区现仅保留 `一键生成 NPC 资源`。
4. 全仓静态搜索确认，旧版 `Death / 动作映射 / 扫描输入 / NPC 名称` 口径未再残留到当前主线代码。
5. Unity MCP 仍返回 `Connection failed: Unknown error`，本轮依旧无法补做编辑器内编译验证。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 调整为顶部获取选中项、底部单独生成的最终交互

**当前恢复点**:
- 主线仍是 NPC 固定模板生成器闭环。
- 当前代码口径已对齐，下一步应执行白名单 Git 同步，并等待用户在 `main` 主项目中手测。
