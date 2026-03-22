# 农田｜第一批 main 收口执行

你这一轮不要继续扩写新内容，直接进入第一批 `main` 收口。

## 你现在只做这几件事
1. 停止继续开发，不新增任何业务改动。
2. 只核对这批白名单路径是否就是你当前最终交付面：
   - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
   - `.kiro/specs/农田系统/2026.03.16/1.0.3基础UI与交互统一改进/`
   - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/memory.md`
   - `.kiro/specs/农田系统/2026.03.16/memory.md`
   - `.kiro/specs/农田系统/memory.md`
   - `.codex/threads/Sunset/农田交互修复V2/memory_0.md`
3. 不要解释 shared root 里的 unrelated dirty，也不要替别的线程收口。
4. 如果以上白名单无误，就只对这批路径做最小 `main` 收口。
5. 收口后立刻停下，不再顺手继续改。

## 本轮禁止事项
- 不要继续扩写 `1.0.3`
- 不要碰白名单之外的路径
- 不要把其他线程的 dirty 混进来
- 不要因为旧 `git-safe-sync` 的 `main + task` 口径继续把这轮卡死在“不能提交”
- 不要做大而全提交

## 你回我只要这些
- 实际提交到 `main` 的路径
- 提交 SHA
- 当前 `git status` 是否 clean
- blocker_or_checkpoint
- 一句话摘要

## 一句话口径
- 这轮不是继续开发，而是把你已经完成的农田 `1.0.3 + InventorySlotUI shake` 收成第一批 `main` checkpoint。
