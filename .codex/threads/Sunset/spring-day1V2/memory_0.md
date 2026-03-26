# spring-day1V2 - 线程记忆

## 线程概述
- 线程名称：spring-day1V2
- 线程分组：Sunset
- 线程定位：承接 `spring-day1` 的 V2 代际续工，但第一轮先做接班后的 live preflight 与非热正式面首刀裁定。
- 当前核心目标：重新钉实 `ee318757` 基线、own 正式面与 `Primary.unity` 的 mixed hot-file blocker，然后只给出一个安全的 V2 第一刀。

## 当前状态
- **完成度**：已完成首轮接班 preflight 与第一刀裁定，尚未进入实现
- **最后更新**：2026-03-26
- **状态**：等待按非热正式面进入第一刀

## 会话记录

### 会话 1 - 2026-03-26

**用户需求**:
> 继续执行，请先完整读取 `26.03.26-Day1V2首轮启动委托-09.md`；这轮唯一主刀固定为：只做接班后的 live preflight 与非热正式面首刀裁定，先重新钉实 `ee318757` 基线、own 正式面和 `Primary.unity` 的 mixed hot-file blocker，再给出 V2 第一刀。不要先碰 `Primary.unity`，不要删 `.kiro` 证据，也不要扩认 `GameInputManager.cs / PlacementManager.cs / StaticObjectOrderAutoCalibrator.cs / TagManager.asset`；只按文档里的最小回执格式回复。

**完成任务**:
1. 完整读取 `委托-09`、V2 交接包 `00/05/06`、纠偏委托 `07`、状态确认委托 `08`、旧线程 `memory_0.md`、父工作区 `memory.md`、`shared-root-branch-occupancy.md` 与锁脚本。
2. 按 Sunset 等价启动闸门完成只读 preflight：
   - `cwd = D:\Unity\Unity_learning\Sunset`
   - `branch = main`
   - `HEAD = 45e4e89baf6c75d8803c1458e08f28bf1b217a66`
   - `shared-root-branch-occupancy.md` 当前仍为 `neutral-main-ready`
3. 重新钉实 `ee318757`：`git show --no-patch --oneline ee318757` 仍可解析，且 `git merge-base --is-ancestor ee318757 HEAD` 返回 `yes`，因此它继续作为 Day1 已接受 checkpoint。
4. 复核 own 正式面仍完整存在，且当前 `git status` 未显示这些正式面 dirty：
   - `Assets/222_Prefabs/UI/Spring-day1/`
   - `Assets/Resources/Story/SpringDay1Workbench/*.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese*.asset`
   - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
5. 复核 `Primary.unity` 当前 lock 为 `unlocked`，但 diff 同时包含：
   - Day1 自己的 `StoryManager / startLanguageDecoded / preferStoryWorkbenchOverlay`
   - `Assets/222_Prefabs/NPC/001.prefab`、`Assets/222_Prefabs/NPC/002.prefab` 对应的 `showDebugLog / drawDebugPath` 现场改动
   因此它仍是 `mixed hot-file blocker`。
6. 给出 V2 第一刀裁定：当前不申请 scene 进入，第一刀继续固定在 non-hot 正式面，只从 Day1 prefab / recipe / DialogueChinese 字体资产 / Day1 test 这组正式面里选切口继续。

**修改文件**:
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md` - [追加]：记录 V2 首轮 live preflight 与第一刀裁定。
- `.codex/threads/Sunset/spring-day1V2/memory_0.md` - [新增]：建立 V2 线程记忆并记录接班第一轮结论。

**解决方案**:
- 把 `ee318757` 与 live `HEAD` 区分开：前者是 Day1 已接受 checkpoint，后者只是 shared root 当前现场。
- 把 own 正式面与 `Primary.unity` 热区严格拆开：正式面继续作为 V2 的安全入口，scene 面继续只读阻塞。

**遗留问题**:
- [ ] 当前 `HEAD` 已前进到 `45e4e89...`，后续每一刀仍需继续按 live 现场重做 preflight，不能把交接包里的旧 `HEAD` 当当前真相。
- [ ] `Primary.unity` 仍是 `unlocked + mixed hot-file blocker`，在归属拆清并拿到安全写窗口前，V2 不能 claim scene owner。
- [ ] 下一轮应先从两个 UI prefab、workbench recipe、DialogueChinese 字体资产与 Day1 test 这组 non-hot 正式面中，选定唯一的实现切口。

### 会话 2 - 2026-03-26

**用户需求**:
> 请先完整读取 `26.03.26-Day1V2共享根大扫除与白名单收口-11.md`。这轮唯一主刀固定为：只做 spring-day1 这条线 own dirty / untracked 的认领、清扫和白名单收口；不恢复 Day1 主线施工。不要顺手继续 Day1 业务，不要碰 Primary.unity / GameInputManager.cs，也不要替别的线程扫地。只按文档里的固定回执格式回复。

**完成任务**:
1. 完整读取 `委托-11`，并结合父工作区 / 子工作区 / 当前线程记忆，只把本轮范围锁定为 spring-day1 自有尾账清扫与白名单收口。
2. 通过 stable launcher 对 Day1 白名单跑 `preflight`，确认执行现场为 `main@eb6284fa`，且当前白名单不触发 C# 代码闸门。
3. 明确认领为 spring-day1 自有 tail 的 dirty / untracked：
   - 5 个 `DialogueChinese*.asset`
   - `.codex/threads/Sunset/spring-day1/`
   - `.codex/threads/Sunset/spring-day1V2/`
   - `.kiro/specs/900_开篇/spring-day1-implementation/memory.md`
   - `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/`
   - `.kiro/specs/900_开篇/spring-day1-implementation/26.03.26-Day1V2共享根大扫除与白名单收口-11.md`
4. 明确排除本轮不碰的 foreign dirty：
   - `Assets/000_Scenes/Primary.unity`
   - `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
   - `Assets/Editor/StaticObjectOrderAutoCalibrator.cs`
   - `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
   - 导航 / 农田 / `ProjectSettings/TagManager.asset` 等 shared root 其他脏改
5. 这轮把旧 Day1 prompt / 交接文档 / 样式快照重新定性为“应保留并纳入白名单的自有证据”，不再沿用旧 hygiene 口径把它们当成要删掉的临时垃圾。

**修改文件**:
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [追加]：记录本轮 shared root 尾账清扫与白名单收口口径。
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md` - [追加]：同步父层的 own / foreign 拆分与收口目标。
- `.codex/threads/Sunset/spring-day1V2/memory_0.md` - [追加]：记录当前线程这轮只做尾账清扫，不恢复 Day1 施工。

**解决方案**:
- 把“扫地”理解为只收 spring-day1 自己留在 shared root 的资产 / memory / 委托文档尾账，而不是替 shared root 做总清扫。
- 把 `V2交接文档`、`委托-07/08/09/10/11`、Day1 UI prompt 产物和 `V1.0_UI样式快照_2026-03-25` 明确当成需要保留并收口的自有证据。

**遗留问题**:
- [ ] 当前 shared root 里的 foreign dirty 仍然很多，但它们不属于本轮 spring-day1 白名单。
- [ ] `Primary.unity` 继续是 mixed hot-file blocker；本轮收口完成后，Day1 主线仍不从 scene 面恢复。

### 会话 3 - 2026-03-26

**用户需求**:
> 检查一下和你 day1 家族相关的 dirty，去认领一下哪些是你的，开始。

**完成任务**:
1. 按 `skills-governor + sunset-workspace-router` 重新做了 Sunset 手工等价启动闸门，并把本轮范围锁死为“只审 Day1 家族当前 live dirty 的 owner”，不恢复 Day1 施工。
2. 重新核实 live 现场为 `D:\Unity\Unity_learning\Sunset @ main @ 8c4e6ff7`，当前 `git status` 里仍然脏着的 Day1 家族可疑项只剩 3 个字体资产：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
3. 对这 3 个字体资产做了 file-level owner 复核，而不是继续沿用上一轮“全都算 Day1 own”的宽认领：
   - `DialogueChinese V2 SDF.asset`：`definitely ours`
     - GUID 只被 `DialogueFontLibrary_Default.asset`、`SpringDay1PromptOverlay.prefab`、`SpringDay1WorkbenchCraftingOverlay.prefab` 引用；
     - diff 新增 glyph 已带出 `Day1 任务页`、`和 NPC001 完成首段对话`、`从 E 键接触开始` 这一组 Day1 专有文案。
   - `DialogueChinese SDF.asset`：`Day1 强相关，但当前不安全吞并`
     - 仓库 live 引用面只剩 `Primary.unity`；
     - diff glyph 带出 `任意键继续对话`，说明它与 Day1 对话 UI 强相关；
     - 但因为唯一 live 入口是 `Primary.unity`，当前仍应按 `mixed scene surface` 报实。
   - `DialogueChinese Pixel SDF.asset`：`Day1 强相关，但当前不安全吞并`
     - 当前被 `Primary.unity` 与 `NPC 001/002/003 prefab` 共用；
     - diff 更像 live 运行时对白 / 提示文案缓存，不能在“不碰 NPC / 不碰 Primary”约束下直接吞并。
4. 用 stable launcher 试探了“只对白名单 `DialogueChinese V2 SDF.asset + Day1 记忆` 做 `preflight`”：
   - `V2` 自身被允许纳入；
   - 但脚本仍以 `Assets/TextMesh Pro/Resources/Fonts & Materials` 这个 own root 下还有 `DialogueChinese Pixel SDF.asset / DialogueChinese SDF.asset` same-root remaining dirty 为由阻断继续 sync。

**修改文件**:
- `.kiro/specs/900_开篇/spring-day1-implementation/memory.md` - [追加]：记录当前 3 个 Day1 相关字体 dirty 的 owner 三分法与 same-root blocker。
- `.codex/threads/Sunset/spring-day1V2/memory_0.md` - [追加]：记录本轮 Day1 家族 dirty 认领复核与 preflight 阻断。

**解决方案**:
- 把这轮 Day1 家族 dirty 从“整个字体包一起认领”收紧为“只有 `DialogueChinese V2 SDF.asset` 可稳认领，`SDF/Pixel` 继续按 mixed surface 报实”，避免为了收口而把 `Primary.unity` / NPC 共用面的副产物一起吞掉。
- 通过单文件 `preflight` 先把真正的阻断点钉死：当前不是 `V2` 自己不能收，而是 same-root 下还挂着两个不安全吞并的相关字体 dirty。

**遗留问题**:
- [ ] 如果后续仍要做 Day1 字体面白名单 sync，必须先对 `DialogueChinese SDF.asset / DialogueChinese Pixel SDF.asset` 的 mixed 归属再做一次明确裁定；否则 stable launcher 仍会因 same-root remaining dirty 拒绝继续。
- [ ] `Primary.unity` 继续是 mixed hot-file blocker；只要 `DialogueChinese SDF.asset` 仍只挂在 scene live 面上，Day1 这边就不应把它静默吞并成“纯 own formal-face”。
