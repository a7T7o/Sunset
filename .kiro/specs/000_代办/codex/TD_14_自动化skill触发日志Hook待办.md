# TD_14 自动化 skill 触发日志 Hook 待办

## 来源
- 产生时间：2026-03-18
- 来源阶段：`Codex规则落地` 阶段 21 收口后的锐评审核
- 来源结论：当前 `skill-trigger-log.md` 已经完成规则化和人工强制，但尚未变成真正的自动化 hook / 脚本闸机

## 当前定位
- 这是 `000_代办` 记录层条目，不是新的正文工作区。
- 当前不得以此条目为理由开启新的正文阶段或在 clean shared root 上继续做二次基建。
- 现阶段优先级低于“验证阶段 21 产物是否可支撑正常开发”。

## 待办内容
1. 设计一个真正自动追加 `skill-trigger-log.md` 的物理触发器：
   - 可选方向：本地 hook、受控脚本闸机、统一收尾脚本、或 Codex 入口包装层
2. 确保它不破坏现有写回顺序：
   - 子工作区 / 项目 memory
   - 父工作区 / 项目 memory
   - 必要的全局层
   - `skill-trigger-log.md`
   - 线程 memory
3. 确保它不会把轻任务、闲聊、纯翻译误记成实质性任务
4. 确保它与 Sunset 的 `git-safe-sync.ps1`、`sunset-startup-guard`、`skills-governor` 不打架

## 当前禁止事项
- 不在本轮 clean 现场上直接开始开发该 hook
- 不把这个条目误当成“阶段 22 已经正式启动”
- 不为了这条待办而打断 farm 的实盘重测和线程恢复

## 恢复点
- 等 farm 的 `grant -> ensure-branch` 重测通过、shared root 恢复可持续开发节奏后，再决定是否把本条目升格为新的正文阶段
