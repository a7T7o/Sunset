# 农田交互修复V2 - GameInputManager 热文件专项批次

```text
【真实开发准入批次 02｜农田交互修复V2｜GameInputManager 热文件专项】
使用前提：
- NPC 已完成本轮并成功 return-main
- shared root 仍是 main + neutral + clean
- 当前 `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 锁状态为 unlocked；执行时仍必须 live 再查

你的 continuation branch：
- codex/farm-1.0.2-cleanroom001

本轮真实目标：
- 不再重复第一检查点
- 直接进入依赖 `GameInputManager.cs` 的第二检查点
- 在拿到 A 类热文件锁的前提下，完成最小热文件 checkpoint

你的第一动作：
1. 先执行：
   powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action request-branch -OwnerThread "农田交互修复V2" -BranchName "codex/farm-1.0.2-cleanroom001" -CheckpointHint "farm-hotfile-gameinput-02" -QueueNote "dev-batch-02"

分流规则：

- 如果返回 GRANTED 或 ALREADY_GRANTED：
  1. 执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action ensure-branch -OwnerThread "农田交互修复V2" -BranchName "codex/farm-1.0.2-cleanroom001"
  2. 先查锁：
     powershell -ExecutionPolicy Bypass -File .kiro/scripts/locks/Check-Lock.ps1 -TargetPath "Assets/YYY_Scripts/Controller/Input/GameInputManager.cs"
  3. 如果返回 unlocked，立刻申请锁：
     powershell -ExecutionPolicy Bypass -File .kiro/scripts/locks/Acquire-Lock.ps1 -TargetPath "Assets/YYY_Scripts/Controller/Input/GameInputManager.cs" -OwnerThread "农田交互修复V2" -Task "farm-hotfile-batch-02" -Checkpoint "minimal-GameInputManager checkpoint for farm continuation"
  4. 成功拿锁后，本轮只允许推进最小热文件检查点：
     - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
     - 直接耦合的 `Assets/YYY_Scripts/UI/Inventory/`
     - 如确有必要，允许读写：
       - `Assets/YYY_Scripts/Farm/`
       - `Assets/YYY_Scripts/Service/Placement/`
     - 但不要扩张到 `Primary.unity` / `DialogueUI.cs`
  5. 本轮禁止保留到最终交付中的 branch 噪音：
     - AGENTS.md
     - scripts/git-safe-sync.ps1
     - 与本热文件检查点无关的治理文档漂移
  6. 如果形成最小 checkpoint，执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode task -OwnerThread "农田交互修复V2" -ScopeRoots "Assets/YYY_Scripts/Controller/Input","Assets/YYY_Scripts/UI/Inventory","Assets/YYY_Scripts/Farm","Assets/YYY_Scripts/Service/Placement" -IncludePaths "<按实际改动逐条填写>"
  7. sync 成功后先释放锁：
     powershell -ExecutionPolicy Bypass -File .kiro/scripts/locks/Release-Lock.ps1 -TargetPath "Assets/YYY_Scripts/Controller/Input/GameInputManager.cs" -OwnerThread "农田交互修复V2" -ReleaseNote "hotfile checkpoint complete"
  8. 然后执行：
     powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action return-main -OwnerThread "农田交互修复V2"

- 如果第 2 步发现锁已被占用：
  - 不准硬抢
  - 不准改热文件
  - 直接 return-main
  - 聊天中把 blocker_or_checkpoint 写成 hotfile-locked-by-other

- 如果你已经拿到锁，但修改后无法完成安全 checkpoint：
  - 不准自行释放锁
  - 不准 return-main
  - 立即停写
  - 聊天中把 blocker_or_checkpoint 写成 hotfile-held-blocked
  - 等治理线程接管

- 如果 request-branch 返回 LOCKED_PLEASE_YIELD：
  - 不准 ensure-branch
  - 不准申请热文件锁
  - 不准写 tracked 文件
  - 只在 Draft 保留最小恢复点后等待

本轮聊天只回复：
- 已读 prompt 路径
- request-branch: <结果>
- ensure-branch: <成功 / 未执行 / 失败>
- hotfile_lock: <acquired / locked / not-needed / held-blocked / 未执行>
- sync: <成功 / 未执行 / 失败>
- return-main: <成功 / 未执行 / 失败>
- changed_paths: <路径列表 / none>
- blocker_or_checkpoint: <一句话>
- 一句话摘要

本轮禁止：
- Primary.unity
- DialogueUI.cs
- Unity / MCP / Play Mode
- 在 main 写 tracked 文档
- 长篇叙述过程
```
