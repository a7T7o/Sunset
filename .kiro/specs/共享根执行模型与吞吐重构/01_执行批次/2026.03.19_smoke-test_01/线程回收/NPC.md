# NPC｜执行层 smoke test 01｜治理镜像回收卡

- 状态：已回收，waiting 保留
- 首轮执行结果：
  - `request-branch`：`LOCKED_PLEASE_YIELD`
  - `ticket`：`3`
  - `ensure-branch`：未执行
  - `return-main`：未执行
- Draft 证据：
  - `D:\Unity\Unity_learning\Sunset\.codex\drafts\NPC\20260319-182621-codex-npc-roam-phase2-003.md`
- 当前队列位置：
  - `导航检查` 已完成退场后，`NPC` 已成为当前队首 waiting 条目
  - continuation branch 仍为 `codex/npc-roam-phase2-003`
- 当前治理裁定：
  - 本轮 smoke test 结果有效，不需要重写 tracked 回收或业务记忆
  - 下一步应等待治理线程执行 `wake-next` 或发放专属唤醒，再重做 live preflight，并按既有 prompt 继续 `request-branch -> ensure-branch`
- 说明：本文件由治理线程根据聊天最小回执统一回填；业务线程不得直接修改。

## 续跑结果（治理补记）
- `wake-next`：已由治理线程执行，grant 已正式发放给 `NPC`
- `request-branch`：`ALREADY_GRANTED`
- `ensure-branch`：成功
- `return-main`：成功
- `post_return_evidence_mode`：`defer-tracked-while-queue-waiting`
- 只读核对范围：
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
  - `Assets/111_Data/NPC/NPC_DefaultRoamProfile.asset`
- 续跑完成后的治理裁定：
  - `NPC` 本轮 smoke test 只读闭环已成立
  - queue 中 `ticket 3 / NPC` 已转为 `completed`
  - 后续如继续 NPC 业务，应回到 `codex/npc-roam-phase2-003`，但不属于本轮回收
