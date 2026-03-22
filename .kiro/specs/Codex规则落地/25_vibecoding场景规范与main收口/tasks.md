# 阶段任务：vibecoding 场景规范与 main 收口

## 用户当前裁定
1. `scene-build` 的 Codex 归位已经完成，后续不再把它当成本阶段主阻塞。
2. `vibecoding` 场景规范适配必须立即进入当前主线。
3. `2026.03.21_main-only极简并发开发_01` 与本阶段不是两条冲突主线：
   - 它是执行批次壳
   - 本阶段是治理正文主线
4. `TD_14` 先不冲自动 hook，本轮更优先看：
   - `sunset-startup-guard` 一级告警
   - `trigger-log` 格式统一
   - `Promote-To-Learning` 真实产出
5. 需要一套能直接用于并发现场的：
   - 统一回执
   - dirty 归属说明
   - main 收口机制
6. `scene-build` 物理迁移到新路径这条错误路线彻底作废。

## 当前主线定义
- 当前治理线程的主线已经不再是“修 scene-build 归位”。
- 当前主线是两件事并成一条：
  - 给 `vibecoding` 场景规范适配建立正式入口
  - 给 `main-only` 并发开发建立可执行的统一收口机制

## 阶段边界
- 本阶段不再继续：
  - `scene-build` 新路径迁移
  - queue / grant / ensure-branch 正文化扩建
  - `TD_14` 自动 hook 实现
- 本阶段要先收住：
  - 并发线程怎么报进度
  - dirty 怎么归属
  - 什么时候一起收口进 `main`
  - 全局 skills 线程该帮 Sunset 做哪一刀，不该发散到哪里

## P0：立刻完成
- [x] 建立本阶段正式治理工作区
- [x] 明确“批次壳”与“治理正文主线”的层级关系
- [x] 新增统一回执 / 统一收口 / 顺序进 `main` 的机制文档
- [x] 新增给全局 skills 线程的窄范围委托 prompt
- [x] 把 `scene-build` 归位问题从当前主线中移出，改记为已完成前置

## P1：当前真正在跑的治理内容
- [x] 产出第一版 `vibecoding` 场景规范适配 brief
- [x] 形成第一版当前 shared root dirty 归属说明
- [x] 发起一轮“统一回执窗口”，收当前活线程是否可入 `main`
- [x] 基于统一回执结果形成第一版 main 收口批次表
- [x] 产出第二波“直接恢复开发”唤醒顺序与线程专属 prompt
- [x] 将 `.cs` 代码闸门正式接入 `git-safe-sync.ps1`
- [x] 完成一次真实代码闸门直测与一次 `task preflight` 集成测试

## P2：外部支援
- [x] 让全局 skills 线程只做 `sunset-startup-guard` 一级告警处理方案
- [x] 根据外部方案裁定：
  - `sunset-startup-guard` 继续保留为 Sunset 硬前置
  - 但正式退出当前 `manual-equivalent` 一级告警统计
  - 后续治理侧只再盯：startup preflight 是否真的做了、首条 commentary 是否显式说明、trigger log 是否按 v2 落盘
- [x] 明确本阶段 trigger log 口径：
  - Sunset 治理线程从本阶段起只追加 `STL-[id]` v2 记录
  - 不再往 `skill-trigger-log.md` 追加 `## 2026-...` 自由标题式 Sunset 记录

## 当前判断
- `scene-build` 归位：已完成
- `scene-build` 新路径迁移：已作废
- `TD_14` 自动 hook：已裁定转入冻结后备项，不再算当前剩余项
- `2026.03.21_main-only极简并发开发_01`：继续保留，但只作为执行层壳，不再冒充正文主线
- `sunset-startup-guard`：继续保留为项目硬前置，但退出当前 `manual-equivalent` 一级告警口径
- `skill-trigger-log`：本阶段开始只用 `STL-v2` 记 Sunset 治理记录
- 第一轮执行进度：
  - 农田已入 `main @ f40d228d`
  - `spring-day1` 基础脊柱已入 `main @ 83d809a9`
  - 当前 B组 下一候选只剩遮挡检查

## 完成标准
- [x] 项目经理现在能一句话说清当前主线：
  - “正文看 `25_vibecoding场景规范与main收口`，执行看 `2026.03.21_main-only极简并发开发_01`”
- [x] 至少有一版正式的 `vibecoding` 场景规范适配 brief
- [x] 至少有一版正式的当前 shared root dirty 归属说明
- [x] 至少有一轮真实“统一回执 -> main 收口批次”演练
- [x] `sunset-startup-guard` 一级告警处理方案已被吸收为本地裁定
- [x] `TD_14` 是否继续做，有明确裁定
- [x] 已形成下一波直接开发唤醒顺序，项目经理可不再靠聊天临时拼 prompt
- [x] 当前线程 prompt / 收口模板 / live 快照已同步“代码闸门”口径
- [x] 当前批次原始材料已形成归档与关账摘要
