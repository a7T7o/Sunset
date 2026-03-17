1. 唯一 carrier  
结论：我确认 `codex/npc-roam-phase2-002 @ 6e2af71b204750f554935eae3728108d23c86bb2` 已经是 NPC 后续唯一 continuation branch carrier。  
证据：`git branch -vv` 显示 `codex/npc-roam-phase2-001` 停在 `28aef95d`，`codex/npc-generator-pipeline` 还在更早的 `7b3bdd6c`；`git branch --contains 6e2af71b...` 只返回 `codex/npc-roam-phase2-002`。  
退役条件：无，它本身就是后续唯一应继续推进的分支载体。  
branch-only 入口：后续回根目录时，只应回到这个分支，不应再回 `001` 或旧的 pipeline 分支。

2. continuation worktree 的性质  
结论：我确认 `NPC_roam_phase2_continue` 只是临时物理容器，不是 NPC 的长期合法现场。  
证据：项目规则已经写死，长期功能线程默认应回 `D:\Unity\Unity_learning\Sunset` 根目录，再按真实任务切到 `codex/` 分支；`worktree` 只保留给高风险隔离、故障修复、特殊实验。当前 continuation worktree 之所以存在，只是因为共享根目录还没恢复到 `main`。  
退役条件：共享根目录恢复为 clean `main`，并完成一次根目录 branch-only 切入验证。  
branch-only 入口：不在这个容器里长期开发，而是在根目录检出 `codex/npc-roam-phase2-002`。

3. 共享根目录恢复为 `main` 后的最小 branch-only 入口  
结论：最小 branch-only 入口已经明确。  
证据：当前唯一 continuation carrier 是 `codex/npc-roam-phase2-002 @ 6e2af71b...`，而 continuation worktree 只是它的临时承载容器。  
退役条件：根目录 `D:\Unity\Unity_learning\Sunset` 先恢复到 clean `main`。  
branch-only 入口：工作目录 `D:\Unity\Unity_learning\Sunset`；检出分支 `codex/npc-roam-phase2-002`；起点 commit 以当前 carrier tip `6e2af71b204750f554935eae3728108d23c86bb2` 为准。

4. rescue worktree 的降级与删除  
结论：`NPC_roam_phase2_rescue` 现在就可以彻底降级为只读取证点，但现在还不适合删除。  
证据：NPC continuation 已经转移到 `codex/npc-roam-phase2-002`；rescue 只剩 4 个已确认归属 `spring-day1` 的 TMP 字体资源 dirty，不再承载 NPC 后续主线。  
退役条件：它可以删除的前提是，那 4 个 `spring-day1` dirty 先被对应 owner/治理处理掉，避免删除 worktree 时把未提交现场一并丢失；并且 branch-only 根目录入口已经验证完成。  
branch-only 入口：与 rescue 无关，后续 NPC 不再从 rescue 进入。

5. continuation worktree 的退役时机  
结论：`NPC_roam_phase2_continue` 也应是可退役的临时容器，不应变成长期现场。  
证据：它当前只是在根目录未恢复 `main` 时，临时承载 `codex/npc-roam-phase2-002`；分支本身已经推上远端，业务载体在 branch，不在这个物理目录。  
退役条件：还缺 1 个动作，不是业务迁移，而是入口迁移验证：在 `D:\Unity\Unity_learning\Sunset` 恢复为 clean `main` 后，从根目录成功检出 `codex/npc-roam-phase2-002`，并确认 `git status` clean。做到这一步，continuation worktree 就可以退役。  
branch-only 入口：验证成功后，唯一入口就是 `D:\Unity\Unity_learning\Sunset + codex/npc-roam-phase2-002`。

6. 最终裁定  
结论：NPC 可以在“共享根目录 `D:\Unity\Unity_learning\Sunset` 恢复为 clean `main`，并完成一次从根目录成功检出 `codex/npc-roam-phase2-002` 且 `git status` clean 的验证”之后，完全回到共享根目录 + 分支模式，不再需要任何 NPC 专用 worktree。  
证据：当前唯一 continuation carrier 已明确为 `codex/npc-roam-phase2-002 @ 6e2af71b...`，而两个 NPC worktree 都只是事故期/过渡期容器。  
退役条件：先退役 rescue，再在完成根目录 branch-only 验证后退役 continuation。  
branch-only 入口：`D:\Unity\Unity_learning\Sunset` -> `git checkout codex/npc-roam-phase2-002` -> 从 `6e2af71b...` 这条线继续。