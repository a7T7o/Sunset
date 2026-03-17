本轮未新增修改，只做只读核查。

**1. TMP 资源最终处理**
- **结论**：这 4 个 TMP 字体资源应**保留为 `spring-day1` 的项目字体资产**，但**放弃 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 里当前那份 dirty 增量**；不要把这份 dirty 再固化成提交。
- **证据**：
  - 它们已是 Git tracked 资产，且在 clean checkpoint `codex/spring-day1-story-progression-001 @ a9c952b78f1b0184edc6622820158b70ee1d7ea6` 中存在。
  - `spring-day1` 线程记忆和归属表都把这批字体归到 `spring-day1`：`D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md:1`、`D:\Unity\Unity_learning\Sunset\.kiro\specs\Steering规则区优化\当前运行基线与开发规则\Sunset当前工作树dirty与WIP归属表_2026-03-16.md:1`
  - 当前 dirty diff 明显像“动态字库/图集清空后的脏改”：4 文件合计约 `5199` 行删除、仅 `37` 行新增；其中 `DialogueChinese V2 SDF.asset` 的 atlas 从 `512x512` 缩到 `1x1`，不是适合直接固化的稳定内容。
  - 字体库当前实际绑了其中一部分：`V2` 和 `BitmapSong` 已被 `DialogueFontLibrary_Default.asset` 使用，见 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset:1`；`Pixel` 也在主场景留下材质实例名，见 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity:74882`
- **下一步**：对这 4 个文件的**当前 rescue dirty 只做证据导出后丢弃**；项目里保留它们的**已提交版本**，以后若要调字体，再在 branch-only 流程下重生成。

**2. 回到 `main + branch-only` 后的唯一入口**
- **结论**：
  - 根目录：`D:\Unity\Unity_learning\Sunset`
  - `spring-day1` 工作分支：`codex/spring-day1-story-progression-001`
  - 起点 commit：`a9c952b78f1b0184edc6622820158b70ee1d7ea6`
- **证据**：
  - 这条分支已经是 clean checkpoint，当前 worktree 无 dirty。
  - 它承载了最近一次已完成的 Day1 剧情推进最小闭环，而共享根目录现在仍停在事故现场 `codex/farm-1.0.2-correct001 @ 11e0b7b4`，不能作为直接续做入口。
- **下一步**：共享根目录先恢复到 `main`；随后 `spring-day1` 只在这个根目录里检出上述分支继续，不再依赖长期独立 worktree。

**3. 是否还需要长期独立 worktree**
- **结论**：**确认不再需要**。
- **证据**：
  - `spring-day1` 已有 clean checkpoint 分支。
  - 当前我名下剩余未收口内容只剩这 4 个字体 dirty，且它们不应再固化。
  - `Primary.unity` 与 `DialogueUI.cs` 本轮未读到未固化尾巴，说明并不存在“必须靠独立 worktree 才能保住现场”的剩余理由。
- **下一步**：后续 `spring-day1` 默认按“共享根目录 + 单分支 checkout + 白名单同步”继续；worktree 仅保留历史救援价值，不再作为常态入口。

**4. 从当前状态迁回 branch-only 的最小步骤**
- **结论**：最小过渡只需要 4 步。
- **证据**：当前真正阻塞 branch-only 的，不是代码主线，而是 `NPC_roam_phase2_rescue` 里这 4 个字体 dirty 归属还没收口。
- **下一步**：
  - 第 1 步：把 `D:\Unity\Unity_learning\Sunset @ codex/farm-1.0.2-correct001` 继续视为只读事故现场，不在这里写入。
  - 第 2 步：对 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 中这 4 个字体 dirty 做证据导出；随后**放弃这份 dirty**，回到该分支 `HEAD`。
  - 第 3 步：把共享根目录 checkout 恢复到 `main`。
  - 第 4 步：`spring-day1` 恢复时，只在共享根目录检出 `codex/spring-day1-story-progression-001` 继续 branch-only 开发/验收。

**5. `Primary.unity` / `DialogueUI.cs` 是否还有你必须认领的未固化尾巴**
- **结论**：**没有**。
- **证据**：
  - 在 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 上，对 `Assets/000_Scenes/Primary.unity` 和 `Assets/YYY_Scripts/Story/UI/DialogueUI.cs` 做 `git diff --name-only` 返回为空。
  - 共享根目录当前的 `git status` 里，这两个文件也不在我这条线的剩余 dirty 里。
  - `DialogueUI.cs` 当前已是已提交脚本，且场内仍保留 `fontLibrary` 接口，见 `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\DialogueUI.cs:26`
- **下一步**：这两个文件后续只有在新 checkpoint 明确重开时，才需要我重新认领；当前不属于未固化尾巴。

**6. 最终裁定**
- **结论**：`spring-day1` 在**共享根目录恢复到 `main`**、并且 **`NPC_roam_phase2_rescue` 中这 4 个 TMP 字体 dirty 被放弃/清空**之后，就可以**完全回到共享根目录 + 分支模式**。
- **证据**：届时我这条线不再持有任何未固化 hot-file 尾巴；剩余 clean 基线已经存在于 `codex/spring-day1-story-progression-001 @ a9c952b78f1b0184edc6622820158b70ee1d7ea6`。
- **下一步**：先收掉这 4 个字体 dirty，再把共享根目录切回 `main`，`spring-day1` 即可正式恢复 branch-only 常态。

---

本轮只读核查，无新增修改。

**1. 归属裁定**
- **结论**：shared root 这组 4 个 dirty 和 `D:\Unity\Unity_learning\Sunset_worktrees\NPC_roam_phase2_rescue` 这组 4 个 dirty，连同交叉出来的第 5 个相关资产，一共这 5 个字体资产都应继续归 `spring-day1`；**不改判给 `farm`、`NPC` 或其他线程**。
- **证据**：
  - `SoftPixel / V2 / BitmapSong` 已进入对话字体库：`D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset#L18`、`D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset#L26`、`D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset#L30`
  - `DialogueChinese SDF` 仍被主场景直接引用：`D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity#L687`
  - `SoftPixel` 也已进入 NPC 相关展示配置：`D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\001.prefab#L244`
- **最终动作**：这 5 个资产统一按 `spring-day1` 资产处理，只做尾巴清理，不做 owner 转移。
- **对 shared root 回 main 的影响**：归属一旦明确，就不需要跨线程协商，剩下只是清尾巴。

**2. `SoftPixel` 与 `DialogueChinese SDF` 的身份**
- **结论**：
  - `DialogueChinese SoftPixel SDF.asset`：**当前项目应保留的活资产**
  - `DialogueChinese SDF.asset`：**当前项目也应保留的基线资产**
  - **不是**“一个留、一个删”的关系；真正的历史残留是它们**当前分裂在两个现场的 dirty 状态**，不是这两个资产名本身。
- **证据**：
  - `SoftPixel` 被字体库当作 `garbled/retro` 使用：`D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset#L30`、`D:\Unity\Unity_learning\Sunset\Assets\111_Data\UI\Fonts\Dialogue\DialogueFontLibrary_Default.asset#L34`
  - `DialogueChinese SDF` 仍被 `Primary.unity` 直接挂到 TMP 文本和材质：`D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity#L687`
- **最终动作**：两者都保留**已提交版本**；不把其中任何一个当成“应整体删除的历史垃圾”。
- **对 shared root 回 main 的影响**：恢复 main 时不需要做字体资产删减决策，只需要丢弃 dirty。

**3. 5 个字体资产逐个最终动作**
- **结论**：
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
    - **保留已提交版本**
    - **导出证据后丢弃当前 dirty**
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
    - **保留已提交版本**
    - **导出证据后丢弃当前 dirty**
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SoftPixel SDF.asset`
    - **保留已提交版本**
    - **导出证据后丢弃 shared root 当前 dirty**
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
    - **保留已提交版本**
    - **导出证据后丢弃当前 dirty**
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
    - **保留已提交版本**
    - **导出证据后丢弃 rescue 当前 dirty**
- **证据**：
  - `SoftPixel` 当前 dirty 是大幅新增图集内容，像一次临时再生成：`git diff --stat` 显示约 `2455` 行新增
  - `DialogueChinese SDF` 当前 dirty 则是大幅删空 glyph/atlas：`git diff --stat` 显示约 `1009` 行删除
  - 这两种都不像“应该直接固化”的稳定修改
- **最终动作**：**这 5 个都不需要现在重生成，不需要迁到别处归档**；先以“保留已提交版本 + 丢弃当前 dirty”为统一口径。
- **对 shared root 回 main 的影响**：一旦按这个口径处理，字体问题会从“分裂现场”退化成“已清空的脏状态”。

**4. shared root 回 main 前先清哪一边**
- **结论**：**必须先各自导出证据**；导出后，**先清 shared root 的字体 dirty**，再清 `NPC_roam_phase2_rescue` 的字体 dirty。
- **证据**：
  - 两边 dirty 集合不一致：shared root 独有 `SoftPixel`，rescue 独有 `DialogueChinese SDF`
  - 如果不先各自导出证据，就会丢掉其中一边的唯一状态记录
  - shared root 本身又是要恢复 `main` 的现场，优先清它能最快解除主入口阻塞
- **最终动作**：
  - 先导出 shared root 这 4 个 dirty 的证据
  - 再导出 rescue 这 4 个 dirty 的证据
  - 然后先清 shared root
  - 最后清 rescue
- **对 shared root 回 main 的影响**：这是最小且不失真的回 main 顺序。

**5. `spring-day1-story-progression-001` 是否可退役为 branch-only 入口**
- **结论**：**确认可以**；字体 split dirty 清完后，后续只保留 `D:\Unity\Unity_learning\Sunset @ codex/spring-day1-story-progression-001` 作为 `spring-day1` 的 branch-only 唯一入口即可。
- **证据**：
  - clean checkpoint 已存在：`codex/spring-day1-story-progression-001 @ a9c952b78f1b0184edc6622820158b70ee1d7ea6`
  - 当前未发现还需要我继续认领的 `Primary.unity` / `DialogueUI.cs` 尾巴
  - 真正剩余阻塞只剩这组字体 dirty 的双现场分裂
- **最终动作**：保留分支，退掉旧的独立现场语义；以后从共享根目录直接检出该分支继续。
- **对 shared root 回 main 的影响**：字体问题一旦处理完，就不存在继续长期保留旧 worktree 的理由。

**6. 最终裁定**
- **结论**：`spring-day1` 距离彻底回到 `main + branch-only` **只差一个最小动作**：**把 shared root 与 rescue 两边这组分裂的字体 dirty 先各自导出证据，再统一丢弃当前 dirty，只保留已提交版本。**
- **证据**：这是当前唯一仍然同时占着两个现场、且文件集合不一致的 `spring-day1` 尾巴。
- **最终动作**：证据导出 → 清 shared root 字体 dirty → 清 rescue 字体 dirty。
- **对 shared root 回 main 的影响**：这一步完成后，`spring-day1` 就可以完全回到共享根目录 `main` + 分支检出模式。
