# Skills和MCP 专属 Prompt

## 当前裁决
- 当前目标线程：`Skills和MCP`
- 当前裁决状态：`solidify-wip`
- 原因：
  - shared root live Git 当前不是 clean
  - 当前可见 dirty 归属到你这条线：
    - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
    - `.kiro/specs/Steering规则区优化/memory.md`
    - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`
  - 这批 dirty 不收口，业务线程的 live 准入会被闸机拦下
- 现在第一动作：
  - 先核归属，再用稳定 launcher 完成 governance sync 清场

## 可直接发送正文
```text
【清场优先｜Skills和MCP｜稳定 launcher 复工前清场】

当前 Sunset 的 shared root 还不能直接进入业务线程 live 准入。
原因不是 occupancy 出错，而是 Git working tree 仍有你这条线留下的文档 dirty，正在阻断后续业务线程的 shared root 调度。

你本轮状态裁决为：
- solidify-wip

你本轮唯一第一动作是：
- 先核对这批 dirty 是否确实属于你
- 若属实，立即用稳定 launcher 完成 governance sync 清场

请先只读核对以下 live 事实：
1. `D:\Unity\Unity_learning\Sunset`
2. `git branch --show-current`
3. `git rev-parse --short HEAD`
4. `git status --short --branch`
5. 当前 dirty 是否正好只有这 3 个文件：
   - `.codex/threads/Sunset/Skills和MCP/memory_0.md`
   - `.kiro/specs/Steering规则区优化/memory.md`
   - `.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md`

如果 live dirty 与上面不一致：
- 立刻停止
- 不要代提交流水线
- 先把差异如实写进回收卡

如果 live dirty 与上面一致，按以下命令执行清场：

powershell -ExecutionPolicy Bypass -File C:\Users\aTo\.codex\tools\sunset-git-safe-sync.ps1 -Action sync -Mode governance -OwnerThread "Skills和MCP" -IncludePaths ".codex/threads/Sunset/Skills和MCP/memory_0.md;.kiro/specs/Steering规则区优化/memory.md;.kiro/specs/Steering规则区优化/当前运行基线与开发规则/memory.md"

完成后请再次只读核对：
1. `git branch --show-current`
2. `git rev-parse --short HEAD`
3. `git status --short --branch`

把完整结果写入：
D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\23_前序阶段补漏审计与并发交通调度重建\2026.03.19_稳定launcher复工前清场\线程回收\Skills和MCP.md

只填写：
- `## 本轮回收区（由 Skills和MCP 填写）`

本轮禁止：
- 禁止顺手扩写新内容
- 禁止碰业务线程的 `request-branch / ensure-branch`
- 禁止在聊天里长篇复述回收卡正文

写完后聊天里只回复：
- 已回写文件路径
- 本轮是否完成清场
- 一句话摘要
```
