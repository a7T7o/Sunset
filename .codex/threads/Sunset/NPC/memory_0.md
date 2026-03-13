# NPC 线程记忆

## 线程概述

- **用户主线**: 在 `D:\Unity\Unity_learning\Sunset` 的 `main` 主项目里实现 NPC 通用模板生成器，让用户只导入固定格式 PNG，就能一键得到动画、控制器和 NPC 预制体。
- **当前现场**: `D:\Unity\Unity_learning\Sunset @ main`
- **线程说明**: 当前仓库里不存在现成的 `Sunset/NPC` 线程记忆文件，本文件自 2026-03-13 起补建，用于承接这条 NPC 工具主线。

## 当前状态

- **完成度**: 85%
- **最后更新**: 2026-03-13
- **状态**: 工具与运行时语义已重写完成，待 Git 白名单同步与用户手测

## 固定模板规则

- 一张 PNG = 一个 NPC
- 固定 4 行 3 列
- 第 0 行：Down
- 第 1 行：Left
- 第 2 行：Right
- 第 3 行：Up
- Idle 只使用每行第 1 列
- Move 使用每行 3 帧
- 左右继续桥接为 `Side + flipX`
- 不再生成或配置 Death

## 会话记录

### 会话 1 - 2026-03-13

**用户需求**:
> 检查当前 `main` 的 NPC 工具是否真的符合最新进度，再检查 Unity MCP 是否能连上，然后回到主线程继续完成最初拍板过的 NPC 固定模板工具修订。

**完成任务**:
1. 核对 `main` 现场：确认 `Assets/Editor/NPCPrefabGeneratorTool.cs` 仍是旧版“PNG 文件夹 / NPC 名称 / 动作映射 / Death”工具，不符合用户最新口径。
2. 复测 Unity MCP：`get_console_logs` 与 `recompile_scripts` 都返回 `Connection failed: Unknown error`，无法把 Unity 编译写成已验证事实。
3. 将 `Assets/Editor/NPCPrefabGeneratorTool.cs` 重写为固定模板版：只保留“获取选中项”“一键生成 NPC 资源”、输出路径、Sorting Layer 和运行时默认值；一张 PNG 直接生成 Idle / Move、Animator Controller 和 Prefab。
4. 将 `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` 改为仅保留 Idle / Move 两种状态，并继续用 `Direction + flipX` 处理左右朝向。
5. 将 `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` 去掉 Death 桥接入口，只保留运动检测、朝向驱动和停驻逻辑。
6. 清零仓库内相关旧引用：全仓搜索已无 `Death`、`PlayDeath`、`SetDeathState` 或三参 `SetAnimationSpeed` 的残留调用。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 固定模板 NPC 生成器
- `Assets/YYY_Scripts/Anim/NPC/NPCAnimController.cs` - Idle / Move 运行时状态控制
- `Assets/YYY_Scripts/Controller/NPC/NPCMotionController.cs` - NPC 运动桥接
- `.kiro/specs/001_BeFore_26.1.21/编辑器工具/0_批量SO生成器分类优化/memory.md` - 子工作区落盘
- `.kiro/specs/001_BeFore_26.1.21/编辑器工具/memory.md` - 父工作区落盘

**验证结果**:
- 代码回读通过：工具 UI 中已无 `Death`、NPC 名称输入、动作映射或扫描输入等旧配置。
- 引用链回读通过：运行时脚本内已无 Death 状态残留。
- Unity MCP 未通过：连接失败，未完成编辑器内编译验证。

**遗留问题 / 下一步**:
- [ ] 执行 Git 白名单同步，仅带上 NPC 工具与三层 memory。
- [ ] 等用户在 Unity 主项目中手测“选中 PNG/文件夹 -> 一键生成 -> 拖入场景”闭环。

### 会话 2 - 2026-03-13

**用户需求**:
> 先检查当前 `main` 的代码是否符合最新进度，再检查 MCP 是否能正常连接，然后把最初对 NPC 工具设计的修订彻底补齐。

**完成任务**:
1. 在 `main` 上重新回读 `NPCPrefabGeneratorTool.cs`、`NPCAnimController.cs`、`NPCMotionController.cs`，确认核心固定模板逻辑已经落盘，不再是旧版可配置方案。
2. 找到一个真实未对齐点：`获取选中项` 按钮仍在底部，不符合用户要求的顶部布局；已将其上移到“选中项”区最上方。
3. 保持底部只剩 `一键生成 NPC 资源`，使交互符合“顶部获取、底部生成”的最终口径。
4. 再次实测 Unity MCP：`get_console_logs` 与 `recompile_scripts` 继续返回 `Connection failed: Unknown error`。
5. 静态搜索确认仓库内未再出现 `PlayDeath`、`SetDeathState`、`deathAnimationSpeed`、`动作映射`、`扫描输入`、`PNG 文件夹` 等旧残留。

**修改文件**:
- `Assets/Editor/NPCPrefabGeneratorTool.cs` - 顶部获取选中项按钮布局修正

**验证结果**:
- `main` 代码口径回读通过：固定模板、Idle 中间帧、Move 三帧、左右 `Side + flipX` 均仍成立。
- MCP 连接失败，Unity 编译结果仍未验证。

**恢复点 / 下一步**:
- 当前主线已恢复到“可以在 `main` 上同步并交给用户手测”的阶段。
- 下一步执行 Git 白名单同步；若脚本阻断，则如实回报阻断原因。

**Git 实际结果**:
1. `main` 上的安全同步被脚本拦截，阻断原因为“真实任务前必须先创建 `codex/` 任务分支”。
2. 本轮已在同一路径 `D:\Unity\Unity_learning\Sunset` 内切换到 `codex/npc-fixed-template-main`，未切出新的 worktree。
3. 通过白名单同步成功提交并推送，首次提交号为 `f62022ef`。
