请先完整读取：
[D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-23_shared-root第二波blocker分流批次_03.md]

不要再把这轮当成“继续业务上传 `Data/Core` 三文件”。

你当前唯一主线固定为：
把 `InventoryItem.cs + SaveDataDTOs.cs + SaveManager.cs` 这一组的 `Ready-To-Sync / CodexCodeGuard / git-safe-sync` 挂死收成一次可判断的工具 incident；如果已有足够证据就不盲目重跑，如果证据不够，最多只做一次同白名单最小复现。

你必须先继承并且不要推翻的当前真状态：
1. 上一刀 `prompt_02` 已经完成，不准继续把同一挂死包装成“再试业务上传”。
2. 当前 `thread-state = PARKED`
3. 当前业务事实已经够明确：
   - [InventoryItem.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/InventoryItem.cs)
   - [SaveDataDTOs.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs)
   - [SaveManager.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs)
   可以诚实视作同一历史小批
4. 当前治理位新增判断也必须继承：
   - 普通 `git diff --name-status HEAD --` 对这 3 文件是瞬间返回的
   - [CodexCodeGuard Program.cs](/D:/Unity/Unity_learning/Sunset/scripts/CodexCodeGuard/Program.cs) 的实现看上去即使异常也会吐 JSON 再退出
   - 所以当前更像 `launcher / preflight / process` incident，而不像这 3 文件 same-root 本身的问题

这轮唯一允许的范围固定为：
1. 这 3 个 `Data/Core` 文件
2. 与这次 preflight incident 直接相关的输出、日志、memory
3. 不准扩到：
   - [StoryProgressPersistenceService.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs)
   - 任意 `SaveManager* / StoryProgressPersistenceService* / Workbench* / SpringDay1DirectorStagingTests*`

这轮必须按顺序执行：
1. 先判断：你手里现有证据是否已经足以把这次挂死收成 incident。
2. 如果证据已足够：
   - 不重跑
   - 直接把 incident 证据卡补齐并停车
3. 如果证据不够：
   - 只用同一白名单做最多 `1` 次最小复现
   - 只为抓 incident 证据，不是为继续业务上传
4. 最终必须明确回答：
   - 挂死到底发生在 `Ready-To-Sync -> sunset-git-safe-sync preflight -> CodexCodeGuard -> git diff --name-status HEAD --` 的哪一层
   - 这次 incident 是“可继续交给业务线程自己排”，还是“应升级给治理/工具位”
5. 不准继续第二个切片。

这轮完成定义只有两种：
1. incident 证据已经足够，且你已把它收成明确结论并停车
2. 或者你做了最多 1 次同白名单最小复现后，拿到更硬的 incident 结论并停车

最终回执必须额外明确：
1. 这轮有没有重跑复现；如果没有，为什么现有证据已足够
2. incident 最终落在哪一层
3. 这轮有没有继续把它当业务上传推进
4. 你建议这件事下一步归业务线程继续，还是升级给治理/工具位

[thread-state 接线要求｜本轮强制]
如果你这轮会继续做真实 incident 复现，先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 `Ready-To-Sync` 复现前，必须先跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮只读补证或最终停车，跑：
`D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`
