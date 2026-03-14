# 导航检查线程记忆

## 线程概述

- 线程目标：对 Sunset 导航系统做只读审计、核实现状、沉淀可接续的判断基线。
- 默认工作目录：`D:\Unity\Unity_learning\Sunset`
- 默认分支：`main`

## 当前状态

- 最近更新：2026-03-15
- 当前主线目标：把线程旧分析、Sunset 规则、现状代码、场景挂载、Prefab 分布和 Editor 配置统一核对，形成我方审计结论。
- 当前恢复点：若继续推进，直接从 `1.0.0初步检查/03_现状核实与差异分析.md` 的问题清单进入下一轮整改或方案拆解。

## 会话记录

### 会话 1 - 2026-03-15

- 用户显式指定线程目录 `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\导航检查` 与工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\999_全面重构_26.03.15\导航检查`，要求彻底阅读线程目录全部内容，并核实现状代码与工作区文档后输出我的分析。
- 已完成事项：读取线程旧稿 `01_导航代码索引.md`、`02_导航全盘分析报告.md`；确认子工作区和父工作区当前无现成文档；读取 `.kiro/steering/README.md`、`rules.md`、`workspace-memory.md`、`systems.md` 与 `.codex/threads/线程分支对照表.md`；核实导航主链代码、Primary 场景挂载、相关 Prefab 与 Editor 代码。
- 关键决策：本轮不覆盖线程旧稿，而是在 `1.0.0初步检查` 目录新增 `03_现状核实与差异分析.md`；按“子工作区 memory → 父工作区 memory → 线程 memory”顺序首次建立记忆链。
- 关键结论：`GameInputManager -> PlayerAutoNavigator -> NavGrid2D` 仍是核心主链；`PlacementNavigator` 为运行时创建；`CloudShadowManager` 反射读取 NavGrid 私有字段但主场景当前未启用该路径；`NavGrid2D` 当前并非真正 Zero GC，因为 `Physics2D.OverlapCircleAll` / `Physics2D.OverlapPointAll` 官方文档明确说明会分配返回数组。
- 主线恢复点：后续如继续本线程，应先围绕 GC 审计、刷新节流、文档漂移三件事展开，而不是回到旧报告的排序热点单点判断。
