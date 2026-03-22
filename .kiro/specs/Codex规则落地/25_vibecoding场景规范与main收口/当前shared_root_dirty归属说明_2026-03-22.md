# 当前 shared root dirty 归属说明

## 目标
- 把 `D:\Unity\Unity_learning\Sunset` 当前 mixed dirty 现场拆成可直接引用的归属口径。
- 以后任何线程来问“现在 dirty 怎么办”，先看这份，不再现场临时脑补。

## 使用原则
- 看见别人的 dirty，不等于你不能继续开发。
- 线程只需要认领自己的 `changed_paths`，不要替别人解释。
- 是否可进 `main`，不靠“现场看起来很脏”拍脑袋判断，而靠：
  - 统一回执
  - 统一批次表
  - 顺序收口窗口

## 当前归属分层

### A. 当前治理线自己认领
- 用途：当前主线正文与执行壳。
- 当前认领范围：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\25_vibecoding场景规范与main收口\`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\README.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\共享根执行模型与吞吐重构\01_执行批次\2026.03.21_main-only极简并发开发_01\可分发Prompt\`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\Codex规则落地\memory_0.md`
- 说明：
  - 这是本轮治理线程真实认领面。
  - `25` 是当前正文主线。
  - `2026.03.21_main-only极简并发开发_01` 只作为执行批次壳。

### B. 治理线旧账，但不再继续扩写
- 用途：历史残留、坏入口、待后续归档或停用说明。
- 当前范围：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\tasks.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\memory.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\24_main-only极简并发开发与scene-build迁移\*.md`
- 说明：
  - 它们仍在现场里，但已经不是健康活入口。
  - 后续只允许做“停用、归档、替换说明”类动作，不再往里继续叠正文。

### C. NPC 线程认领
- 当前范围：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\NPC\`
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md`
- 说明：
  - 这批 dirty 不归治理线代解释，也不归 `spring-day1` 或 `scene-build` 代收。

### D. `spring-day1` / 剧情整理线认领
- 当前范围：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0.1剧情初稿\` 下的删除/迁移
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\0.0阶段\` 下的新目录
- 说明：
  - `spring-day1` 的基础脊柱代码已经以 `83d809a9` 进入 `main`，不再属于“待 branch carrier 迁入”的当前 dirty。
  - 这条线当前 shared root 里真正剩下的，主要是：
    - `scene-build_handoff.md`
    - `requirements.md`
    - `memory.md`
    - 线程 `memory_0.md`
    - 旧剧情目录删除 / 新阶段目录整理
  - 如果后面再次浮出 `DialogueChinese*` TMP 资产 dirty，继续按这一线的历史保护资产处理，不退回 `scene-build` 重判。

### E. 农田 / 库存交互线状态
- 当前范围：
  - 第一批已入 `main`：
    - `Assets/YYY_Scripts/UI/Inventory/InventorySlotUI.cs`
    - `.kiro/specs/农田系统/2026.03.16/1.0.3基础UI与交互统一改进/*`
    - `.kiro/specs/农田系统/2026.03.16/1.0.2纠正001/memory.md`
    - `.kiro/specs/农田系统/2026.03.16/memory.md`
    - `.kiro/specs/农田系统/memory.md`
    - `.codex/threads/Sunset/农田交互修复V2/memory_0.md`
- 说明：
  - 农田第一批 `main` checkpoint 已完成：`f40d228d`
  - 当前 shared root 不应再把这批路径误判成“仍未收口 dirty”
  - 后续如果再出现农田相关 dirty，应按新的 Input / Placement / Inventory WIP 重新回执，不应由治理线或 `spring-day1` 代收。

## 当前一刀的判断口径
- 如果你是治理线程：
  - 只认 `A + B`。
  - 其中 `A` 是当前继续推进面，`B` 是后续停用/归档债。
- 如果你是业务线程：
  - 只认自己的业务路径和自己的 memory。
  - 不要因为 shared root 同时脏着别人的文件，就误判成自己“不能回执”。

## 统一回执时怎么用
- 每个线程只回：
  - 自己当前在改什么
  - `changed_paths`
  - 是否撞高危目标
  - 是否需要 Unity / MCP live 写
  - 当前能否入 `main`
  - `blocker_or_checkpoint`
- 治理线程拿这份归属说明做两件事：
  - 过滤掉“不属于该线程”的现场噪音
  - 再按批次表决定顺序进 `main`

## 当前执行更新（2026-03-22）
- 农田第一批 `main` 收口已完成：`f40d228d`
- `spring-day1` 基础脊柱迁入 `main` 已完成：`83d809a9`
- 因此当前 shared root mixed dirty 的重点，不再是“农田首批代码”或“spring-day1 基础脊柱代码”，而是：
  - 治理线当前一批入口 / 机制文件
  - NPC 业务修正
  - `spring-day1` 文档整理与 handoff
  - 旧剧情目录整理

## 明确作废口径
- `scene-build` 新路径迁移：作废
- 旧 queue / grant / ensure-branch 正文化扩建：作废
- 把 mixed dirty 直接揉成一个超大 commit：不采用

## 一句话结论
- 当前 shared root 的 dirty 不是“谁都别动”，而是“每条线只认自己的改动，治理侧用统一回执和批次表做顺序收口”。
