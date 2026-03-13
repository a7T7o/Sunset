# 批量 SO 生成器分类优化 - 开发记忆

## 模块概述

将 `Tool_BatchItemSOGenerator` 的物品类型选择从扁平按钮布局改为大类+小类的层级结构（V2 版本），支持更多物品类型并提升用户体验。

## 当前状态

- **完成度**: 100% ✅
- **最后更新**: 2026-01-04
- **状态**: ✅ 已完成

## 包含文件

| 文件 | 说明 |
|------|------|
| `requirements.md` | 需求文档（大类分类系统、小类动态显示） |
| `design.md` | 设计文档 |
| `tasks.md` | 任务列表 |

## 完成内容

1. ✅ 添加 ItemMainCategory 大类枚举（6 个大类：工具装备/种植类/可放置/消耗品/材料/其他）
2. ✅ 扩展 ItemSOType 枚举（15 种小类）
3. ✅ 使用静态 Dictionary 实现大类→小类映射
4. ✅ UI 采用两行按钮布局：大类 + 小类
5. ✅ 切换大类时自动选中第一个小类并更新 ID/路径
6. ✅ 新增 4 种可放置物品类型（WorkstationData/StorageData/InteractiveDisplayData/SimpleEventData）

## 涉及的代码文件

| 文件 | 操作 | 说明 |
|------|------|------|
| `Tool_BatchItemSOGenerator.cs` | 重构 | V2 版本，大类+小类层级结构 |

---

### 会话 3 - 2026-03-13

**用户需求**:
> 把 NPC 通用模板工具收敛为固定模板傻瓜版：默认全部 NPC 都是同一种 4 向 3 帧 PNG，只保留“获取选中项”和“一键生成”，直接从单张 PNG 生成 Idle / Move、Animator Controller 和可拖入场景的 NPC Prefab。

**完成任务**:
1. 在 `main` 现场核定 `Assets/Editor/NPCPrefabGeneratorTool.cs` 仍是旧版“PNG 文件夹 / NPC 名称 / 动作映射 / Death”工具，与用户最新口径不一致。
2. 将工具整文件重写为固定模板版：通过 Project 选中 PNG 或文件夹收集素材，一张 PNG 直接生成一套 NPC 资源；只保留输出路径、Sorting Layer 和运行时默认值。
3. 将模板规则硬编码为：4 行 3 列、PPU=16、Bottom Center Pivot、第 0 行 Down / 第 1 行 Left / 第 2 行 Right / 第 3 行 Up，Idle 只取每行第 1 列，Move 使用三帧，左右桥接为 `Side + flipX`。
4. 同步清理 `NPCAnimController.cs` 与 `NPCMotionController.cs` 的 Death 逻辑，使运行时只保留 Idle / Move 两种状态，避免工具输出与运行时语义错位。
5. 复测 Unity MCP：`get_console_logs` 与 `recompile_scripts` 继续返回 `Connection failed: Unknown error`，本轮无法完成 Unity 编译闭环，只能做代码回读与引用链检查。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 重写为固定模板 NPC 资源生成器
- `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` - 删除 Death 状态，只保留 Idle / Move
- `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` - 删除 Death 桥接，只保留运动/朝向驱动

**解决方案**:
- 将 NPC 生成入口改为完全基于 Project 选中项的“傻瓜流”，不再要求额外命名、映射或动作指派。
- 用 `State + Direction` 两个 Animator 参数维持 4 向行为，其中左右方向继续共用 `Side` 动画并依靠 `SpriteRenderer.flipX` 翻转。
- 把旧版工具里的可配置模板能力整体删掉，确保 UI 和生成结果都锁死到当前项目唯一有效的 NPC 素材规范。

**遗留问题**:
- [ ] Unity MCP 当前不可用，尚未完成编辑器内真实编译验证。
- [ ] 本轮白名单 Git 同步待执行，不能混入仓库内其他无关 dirty。

---

### 会话 4 - 2026-03-13

**主线目标**:
> 继续核对 `main` 上的 NPC 固定模板工具是否完全贴合用户最终拍板的交互，并把未对齐的 UI 细节补到位。

**本轮阻塞 / 子任务**:
1. 逐项回读 `main` 的真实代码，确认是否还有旧交互残留。
2. 复测 Unity MCP 当前是否恢复连接。
3. 只修正真正没对齐的交互，不重做已经完成的逻辑。

**完成任务**:
1. 回读确认 `Assets/Editor/NPCPrefabGeneratorTool.cs` 里的核心生成逻辑已对齐固定模板：一张 PNG 生成 Idle / Move / Controller / Prefab，且 Idle 取中间帧、Move 取三帧。
2. 发现并修正一个真实未对齐点：用户要求“获取选中项”按钮位于顶部，主线程代码此前仍把它放在底部；现已改为在“选中项”区最上方触发选取。
3. 保持底部只保留“一键生成 NPC 资源”，使工具交互更接近用户指定的 `Tool_001_BatchProject` 习惯。
4. 再次实测 Unity MCP，`get_console_logs` 与 `recompile_scripts` 仍均返回 `Connection failed: Unknown error`。
5. 全仓静态回扫未发现 `PlayDeath`、`SetDeathState`、`deathAnimationSpeed`、`动作映射`、`扫描输入`、`NPC 名称（可选覆盖）`、`PNG 文件夹` 等旧口径残留。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 将“获取选中项”按钮上移到选中区顶部，底部仅保留一键生成

**当前恢复点**:
- `main` 上的 NPC 工具交互已与用户最新硬规则对齐。
- 下一步进入 Git 白名单同步；Unity 内真实编译验证仍受 MCP 断连阻塞。

**Git 收尾结果**:
1. 直接在 `main` 执行 `git-safe-sync.ps1 -Action sync -Mode task` 被脚本阻断，原因是“真实任务不能直接在 `main` 同步，必须先创建 `codex/` 任务分支”。
2. 随后使用临时 `stash` 无损隔离当前白名单改动，执行 `ensure-branch` 创建并切换到 `codex/npc-fixed-template-main`，再恢复改动。
3. 已在 `codex/npc-fixed-template-main` 上完成白名单同步并推送成功，首次提交为 `f62022ef`。
