# NPC - 验收失败返工与 live 验收闭环
```text
【恢复开发总控 01｜NPC｜验收失败返工与 live 验收闭环】
你的 continuation branch：
- codex/npc-roam-phase2-003

这轮不是普通跟进，而是正式返工。
真实验收反馈已经明确：
- 气泡仍压在 NPC 脸上，不自然
- NPC 和 NPC 仍会叠在一起
- 玩家仍能直接穿过 NPC

所以这轮不接受“分支里已经改过”“静态资源已提交”式回执。
这轮只接受两种结果：
- 真正把 live 可见问题修到达标
- 或把真正阻塞 live 达标的层面精准揪出来

你的第一动作 MUST 是：
1. 执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003" -CheckpointHint "npc-live-acceptance-rework" -QueueNote "recovery-control-01"

分流规则：
- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
  2. 进入 branch 后先做 live 审计，不准先宣称成功。至少先回答：
     - 当前 branch 的 `001/002/003.prefab` 是否已经是 `BoxCollider2D + Rigidbody2D + 非 Trigger`
     - 当前 branch 的 `NPCBubblePresenter` 默认头顶高度是否已经抬高
     - 为什么用户当前在 Unity 里仍看到“压脸气泡 + 可穿透 + 可重叠”
  3. 你必须先判断 root cause class：
     - `A`：用户当前看到的不是你这条 branch 成果，而是 main / 旧 carrier / 旧实例
     - `B`：branch 虽已改 prefab/script，但 live 场景并没有真正吃到这些改动
     - `C`：你这条 branch 的实现本身仍不够，进了 branch 也还是会压脸或穿透
  4. 本轮允许域只限：
     - Assets/100_Anim/NPC/
     - Assets/111_Data/NPC/
     - Assets/222_Prefabs/NPC/
     - Assets/Sprites/NPC/
     - Assets/YYY_Scripts/Anim/NPC/
     - Assets/YYY_Scripts/Controller/NPC/
     - Assets/YYY_Scripts/Data/NPCRoamProfile.cs
     - Assets/Editor/NPCPrefabGeneratorTool.cs
     - Assets/Editor/NPCAutoRoamControllerEditor.cs
     - .kiro/specs/NPC/
     - .codex/threads/Sunset/NPC/memory_0.md
  5. 这轮硬验收标准固定为：
     - 气泡底边不能再压进 NPC 脸部五官区
     - 气泡要像“头顶说话”，不是“脸前招牌”
     - NPC 与 NPC 不能再中心重叠
     - 玩家不能再直接穿过 NPC 下半身实体区
  6. 碰撞体口径固定：
     - 不准把整个 NPC 做成整块硬碰撞
     - 只保留下半身实体感，参考玩家碰撞体思路
     - 如果还需要交互范围，交互 trigger 和实体 blocker 必须分离
  7. 如果你能在允许域内完成真实返工 checkpoint，就直接做，不准只交分析
  8. 如果你确认这轮绕不开以下任一项：
     - Primary.unity
     - 非 NPC 允许域的 scene instance override
     - 玩家碰撞主链
     - Unity / MCP 专属写窗口
     那就立刻停下，不准擅自升级范围，并把：
     - blocker_or_checkpoint 写成 `needs-primary-lock`
       或 `needs-unity-window`
     - 同时写清真正卡住 live 验收的那一层
  9. 如果形成 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "NPC" -ScopeRoots "Assets/100_Anim/NPC","Assets/111_Data/NPC","Assets/222_Prefabs/NPC","Assets/Sprites/NPC","Assets/YYY_Scripts/Anim/NPC","Assets/YYY_Scripts/Controller/NPC","Assets/YYY_Scripts/Data/NPCRoamProfile.cs","Assets/Editor/NPCPrefabGeneratorTool.cs","Assets/Editor/NPCAutoRoamControllerEditor.cs",".kiro/specs/NPC",".codex/threads/Sunset/NPC" -IncludePaths "<按实际改动逐条填写>"
  10. sync 成功后立刻执行：
      powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "NPC"

- 如果返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准在 main 写 tracked 文件
  - 草稿只准写进 Draft

聊天只回复：
- 已读 prompt 路径
- request-branch
- ensure-branch
- hotfile_lock: not-needed
- 当前 branch / HEAD
- live_audit_result
- root_cause_class
- changed_paths
- 是否触碰 Unity / MCP
- 是否做了真实手测
- sync
- return-main
- carrier_ready
- main_ready
- main_ready_basis
- blocker_or_checkpoint
- 一句话摘要

本轮禁止：
- 把“分支里已经改过”当成完成
- 只修 `NPCPrefabGeneratorTool` 就结束
- 只改 prefab/script 但不核 live 场景是否真吃到
- 回避玩家 / NPC / NPC 之间的真实碰撞结果
```
