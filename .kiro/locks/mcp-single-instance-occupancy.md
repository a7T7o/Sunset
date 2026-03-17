# mcp-single-instance-occupancy

## 文件身份
- 本文件用于记录 `D:\Unity\Unity_learning\Sunset` 的 Unity / MCP 单实例共享占用口径。
- 它不替代：
  - `shared-root-branch-occupancy.md`
  - A 类热文件锁
  - Git 白名单同步
- 它只回答一件事：
  - 当前 Unity / MCP 单实例是否适合继续读写。

## 当前状态
- project_root: `D:\Unity\Unity_learning\Sunset`
- editor_policy: `single-instance-shared-editor`
- default_write_policy: `single-writer-only`
- default_read_policy: `read-mostly-not-guaranteed`
- current_claim: `none`
- play_mode_status: `must-verify-live`
- play_mode_exit_policy: `must-return-to-edit-mode-before-handoff`
- compile_status: `must-verify-live`
- domain_reload_status: `must-verify-live`
- last_updated: `2026-03-17`

## 进入 Unity / MCP 前必须先核
1. 当前任务是不是确实需要 Unity / MCP。
2. 当前 shared root 是否允许进入。
3. 当前 Editor 是否在：
   - Play Mode
   - Compile
   - Domain Reload
   - Package / Asset Refresh
4. 当前是否命中共享热区：
   - `Primary.unity`
   - Prefab / Scene / Inspector 写入
   - 共享 UI / 字体 / Sorting / Layer / 材质
5. 当前 Console 与 MCP 是否出现对象失效、端口占用、返回中间态等信号。

## 默认裁定
- Git 现场中性，不代表 Unity / MCP 现场安全。
- 单实例 Editor 下：
  - 写操作默认不假设天然排队安全。
  - 读操作也不假设一定能读到最终稳定态。
- 凡是为了调试、取证或验收进入 Play Mode，完成当前步骤后都必须主动退回 Edit Mode；把运行中的 Editor 留给别人处理，视为现场未清干净。
- 只要不能证明当前 still safe，就先停写、先复核、先降级只读。

## 冲突信号
- Play / Stop 切换
- Compile / Domain Reload
- Prefab / Scene / Inspector 状态突然跳变
- MCP 端口不可用、对象不存在、读到旧对象
- Console 中出现与当前目标不一致的工具噪音或中间态报错

## 命中冲突后的动作
1. 停止继续写。
2. 重新核：
   - 当前工作目录 / 分支 / `HEAD`
   - Editor 状态
   - Console
   - 当前 shared root 与热文件占用
3. 只能有限次重试，且每次重试前都必须重新取现场。
4. 无法证明现场安全时，退回只读取证并汇报。
5. 如果本线程进入过 Play Mode，则在汇报或交棒前先确认已经回到 Edit Mode。

## 一句话口径
- shared root 在 `main` 只代表 Git 入口中性；Unity / MCP 读写还要再过单实例占用层。
