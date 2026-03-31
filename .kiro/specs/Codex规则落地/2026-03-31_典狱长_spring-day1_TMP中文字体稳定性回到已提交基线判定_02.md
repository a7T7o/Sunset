# 2026-03-31_典狱长_spring-day1_TMP中文字体稳定性回到已提交基线判定_02

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_TMP中文字体稳定性_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`

你这轮不是继续 `Day1` 剧情，不是继续 `Primary`，也不是继续 UI 终验。

你这轮唯一主刀只有一个：

- **把 `Assets/TextMesh Pro/Resources/Fonts & Materials/` 下当前这 6 份 `DialogueChinese* / LiberationSans Fallback` dirty，当成一整根共享 TMP 中文字体稳定性案，判清“它们现在能不能整根安全回到已提交基线”。**

## 一、当前已接受基线

1. `Primary` 案已经结束：
   - 旧 canonical path 已恢复
   - stale lock 已释放
   - 新路径 duplicate 已删除
2. 当前仓库里仍在 churn 的字体资产只有这 6 份：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SoftPixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`
3. 当前治理已接受：
   - 这不是 `Day1 / UI / NPC / 导航` 任一业务线自己的尾账
   - 这是独立的 `共享 TMP 中文字体 importer / 动态资产稳定性案`
4. 当前 diff 量级已经被钉死：
   - `15133 insertions`
   - `410 deletions`
5. 当前生成器硬证也已经成立：
   - `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
   - `AtlasPopulationMode.Dynamic`
   - `isMultiAtlasTexturesEnabled = true`

所以这轮不再讨论“它属于谁的业务需求”，而是只回答：

- **这 6 份 dirty 现在能不能作为一整根不可信 churn，直接安全回到当前已提交基线。**

## 二、这轮唯一主刀

只做这一刀：

1. 只处理这 6 份字体资产
2. 只判断一件事：
   - 当前 dirty 是否可以整根回到 `HEAD` 基线
3. 如果答案是 `可以`：
   - 你就真的把这 6 份资产恢复到 `HEAD`
4. 如果答案是 `不可以`：
   - 你就只回第一真实 blocker

## 三、本轮允许的合格结果

### A｜安全回基线并完成清理

适用条件：

- 你已核实当前 live 业务入口并不依赖这些“尚未提交的字体 churn”才能成立
- 你能诚实证明这 6 份 dirty 现在更像 importer / atlas / glyph 副产物，而不是必须保留的未提交业务成果

动作：

1. 只把这 6 份资产恢复到 `HEAD` 基线
2. 恢复后重新核：
   - 这 6 份文件 `git diff` 为空
   - 当前字体根不再残留这批 dirty
3. 本轮如果你补了 own memory，可以做 docs-only sync

### B｜第一真实 blocker

如果你不能诚实地把这 6 份资产整根恢复到 `HEAD`：

1. 不要半恢复、不要挑 1 到 2 份顺手处理
2. 只回第一真实 blocker
3. 必须说清：
   - exact path
   - 为什么当前不能整根回到 `HEAD`
   - 还差什么证据或现实条件

## 四、本轮明确禁止

- 不准再碰 `Primary.unity`
- 不准继续写 `Day1` 剧情 / UI / NPC / 导航业务代码
- 不准动 `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
- 不准动 `Assets/111_Data/UI/Fonts/Dialogue/DialogueFontLibrary_Default.asset`
- 不准顺手改任何 prefab / scene / Build Settings / Editor 入口
- 不准把这轮扩成“重建新字体方案”
- 不准只清 `V2` 或只清 `Pixel`，必须把这 6 份当一整根处理

## 五、完成定义

你这轮只有两种合格结果：

### A｜真过线

1. 你已证明这 6 份 dirty 可以整根安全回到 `HEAD`
2. 你已实际完成恢复
3. 当前这 6 份资产的 dirty 已清空
4. 本轮没有顺手改任何业务文件或生成器
5. 如果你补了 own memory 并归仓，给 docs-only SHA
6. 如果本轮只是恢复到已提交基线、没有新的代码提交，也要明确写：
   - `代码 SHA = 无（原因：本轮是清理未提交 churn，不是新增代码提交）`

### B｜第一真实 blocker

1. 你无法诚实恢复这 6 份资产
2. 你只回第一真实 blocker
3. 不准顺手转去做“共享字体重建”

## 六、本轮最容易犯的错

- 又把这题写回 `Day1` 业务尾账
- 只盯 `DialogueChinese V2 SDF.asset`，不处理同根其他 5 份
- 顺手去碰生成器、字体库、业务 prefab 或 scene
- 把“恢复到已提交基线”偷换成“继续提交当前 churn”
- 把“这轮清掉 dirty”误写成“整条 `spring-day1` 已 clean”

## 七、固定回执格式

回执按这个顺序：

- `A1 保底六点卡`
- `A2 用户补充层`
- `B 技术审计层`

其中 `A1 保底六点卡` 仍必须逐项显式写：

1. `当前主线`
2. `这轮实际做成了什么`
3. `现在还没做成什么`
4. `当前阶段`
5. `下一步只做什么`
6. `需要用户现在做什么`

## 八、A2 用户补充层必须额外显式回答

你必须额外补 6 条：

1. `这轮最终是 A 还是 B`
2. `这 6 份字体资产是否已整根恢复到 HEAD`
   - 只能写 `是 / 否`
3. `本轮是否触碰生成器或任何业务文件`
   - 只能写 `是 / 否`
4. `当前这 6 份字体资产是否 clean`
   - 只能写 `是 / 否`
5. `这轮是否拿到提交 SHA`
   - 只能写 `是 / 否`
6. `如果没过线，第一真实 blocker 是什么`

一句话收口：

- **这轮只判并处理 6 份共享 TMP 中文字体 dirty 能不能整根回到已提交基线，不再把它们挂回任何业务线。**
