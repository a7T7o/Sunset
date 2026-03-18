# NPC 可分发 Prompt

## 阶段一｜只读唤醒
```text
【阶段一｜只读唤醒｜NPC】

当前 Sunset 已恢复到统一健康基线：
- shared root = D:\Unity\Unity_learning\Sunset
- branch = main
- HEAD = 9b14814b
- git status clean
- shared root = neutral

你这一轮先不要写文件、不要切分支、不要进入 Play Mode。
你的第一条 commentary 必须显式说明你正在使用 skills-governor、sunset-workspace-router，并先完成只读 preflight。

这次不要只在聊天里回复。请把完整结果写入：
D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\NPC.md
只填写：
- `## 阶段一｜线程回收区（由 NPC 填写）`

请只读完成以下动作：
1. 在 shared root 复核 cwd / branch / HEAD / git status --short --branch。
2. 读取：
   - D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\NPC\memory_0.md
   - D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\memory.md
   - 当前相关 NPC 子工作区 memory
3. 只读确认当前 continuation branch 是否仍是：
   - codex/npc-roam-phase2-003 @ 7385d123
4. 只读判断你下一轮真实恢复开发是否会碰：
   - Unity / MCP
   - Play Mode
   - A 类热文件

本轮禁止：
- 禁止写业务文件
- 禁止 ensure-branch
- 禁止进入 Play Mode

写完回收文件后，在聊天里只回复：
- 已回写文件路径
- 是否通过阶段一
- 一句话摘要
```

## 阶段二｜准入
```text
【阶段二｜准入｜NPC】

仅当阶段一已经完成，且治理线程明确允许你进入阶段二时，才执行本轮。

当前前提仍必须满足：
- D:\Unity\Unity_learning\Sunset
- main
- clean
- occupancy = neutral

现在授权你按 NPC 的唯一 continuation branch 进入真实开发：
- codex/npc-roam-phase2-003 @ 7385d123

请把完整结果写入：
D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\22_恢复开发分发与回收\线程回收\NPC.md
只填写：
- `## 阶段二｜线程回收区（由 NPC 填写）`

请严格执行：
1. 先执行 grant：
   git-safe-sync.ps1 -Action grant-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
2. 再执行准入：
   git-safe-sync.ps1 -Action ensure-branch -OwnerThread "NPC" -BranchName "codex/npc-roam-phase2-003"
3. 进入分支后先做最小 live 基线复核，再开始真实 checkpoint。
4. 如果你进入 Unity / MCP / Play Mode，做完后必须先退回 Edit Mode。
5. 一个 checkpoint 完成后：
   - commit + push
   - 如涉及锁则释放锁
   - 执行 return-main 归还 shared root

本轮禁止：
- 禁止在 main 上写 NPC 业务内容
- 禁止绕过 grant 直接 ensure-branch
- 禁止做完 Play Mode 不退回 Edit Mode

写完回收文件后，在聊天里只回复：
- 已回写文件路径
- grant / ensure-branch 是否成功
- 一句话摘要
```
