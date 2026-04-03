# 2026-04-03_典狱长_spring-day1_共享TMP中文字体缺Atlas稳定性修复_01

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-30_单独立案_TMP中文字体稳定性_01.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-03-31_典狱长_spring-day1_TMP中文字体稳定性回到已提交基线判定_02.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\memory.md`

你这轮不是继续 `Day1` 剧情，不是继续 UI，不是继续 NPC，也不是去背 `Library` 本地缓存坏账。

你这轮唯一主刀只有一个：

- **把当前已经坏到运行时缺 `Atlas` / 缺 `m_AtlasTextures` 的共享 TMP 中文字体资产修回“可稳定加载、可被现有消费者使用”的状态。**

## 一、当前已接受基线

1. 治理层已经钉死：
   - 这不是 `Day1 / UI / NPC` 任一业务线自己的尾账；
   - 这是独立的 `共享 TMP 中文字体稳定性案`。
2. 当前 shared root 里真正还在 churn 的相关资产是：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`
3. 当前 `git diff --stat` 已明确：
   - `DialogueChinese SDF.asset` 与 `DialogueChinese Pixel SDF.asset` 都发生了大块删除；
   - `LiberationSans SDF - Fallback.asset` 也在 churn。
4. 当前更强的新 incident 证据已经成立，不再只是 importer 风险：
   - 运行时报：
     - `The Font Atlas Texture of the Font Asset DialogueChinese SDF ... is missing`
     - `The Font Atlas Texture of the Font Asset DialogueChinese Pixel SDF ... is missing`
     - `MissingReferenceException: The variable m_AtlasTextures of TMP_FontAsset doesn't exist anymore`
   - 受影响消费者横跨：
     - `SpringDay1PromptOverlay`
     - `SpringDay1WorldHintBubble`
     - `NPCBubblePresenter`
     - `PlayerThoughtBubblePresenter`
   - 这说明当前不是“某个业务 prefab 引错字”，而是底层字体资产本体已坏。
5. 当前磁盘级硬证据也已钉死：
   - `DialogueChinese SDF.asset`：
     - `m_Material: {fileID: 0}`
     - `m_AtlasTextures:`
     - `- {fileID: 0}`
     - `atlas: {fileID: 0}`
   - `DialogueChinese Pixel SDF.asset` 同样出现：
     - `m_Material: {fileID: 0}`
     - `m_AtlasTextures:`
     - `- {fileID: 0}`
     - `atlas: {fileID: 0}`
   - `git diff` 还能看到：
     - 旧 `Material` 子对象整段被删掉
     - creation settings 被改空
     - point size / metrics 被重写
6. 当前生成器仍然是强相关支撑面：
   - `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
   - 仍明确创建：
     - `AtlasPopulationMode.Dynamic`
     - `isMultiAtlasTexturesEnabled = true`
7. 当前继续明确排除：
   - `Library/EditorSnapSettings.asset`
   - `Library/UIElements/EditorWindows/UnityEditor.InspectorWindow.pref`
   - `Library/BuildProfileContext.asset`
   - `Library/BuildProfiles/*`
   - `Library/StateCache/*`
   这些依旧是本地缓存/布局层坏文件，不是你这轮 owner。

## 二、本轮唯一主刀

只做这一刀：

1. 修复或重建以下共享字体资产，使其重新具备有效的 material / atlas / `m_AtlasTextures` 链：
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
   - `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`
2. 如确有必要，最小修改：
   - `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
3. 只允许把消费者脚本/Prefab/Scene 当验证面看，不允许把它们改成这轮主修对象。

## 三、这轮允许的 scope

本轮允许进入的路径只有：

- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
- `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`
- `Assets/Editor/Story/DialogueChineseFontAssetCreator.cs`
- 你自己的 memory / 工作区 memory

如果你为了验证，需要只读查看以下消费者，允许：

- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
- `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
- `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
- `Assets/YYY_Scripts/Service/Player/PlayerThoughtBubblePresenter.cs`

但默认不改它们。

## 四、本轮明确禁止

- 不准把这轮扩成 `Day1` 业务续工
- 不准碰 `Primary.unity`
- 不准碰 `Town.unity`
- 不准顺手改 `DialogueFontLibrary_Default.asset`
- 不准顺手改业务 prefab / scene / Overlay 默认引用，除非你已经证明“底层字体修好后仍有独立消费者坏账”，否则先停下来报 blocker
- 不准去背 `Library` / `UserSettings` 本地缓存保存失败
- 不准重新打开 `DialogueChinese V2 / SoftPixel / BitmapSong` 这些当前不在 incident 主刀内的字体支线

## 五、完成定义

你这轮只有一个过线标准：

1. `DialogueChinese SDF.asset` 与 `DialogueChinese Pixel SDF.asset` 重新拥有有效 atlas / material / `m_AtlasTextures`
2. 当前最小运行验证里，不再出现：
   - `The Font Atlas Texture ... is missing`
   - `MissingReferenceException: The variable m_AtlasTextures of TMP_FontAsset doesn't exist anymore`
3. 你必须明确说清这轮采取的是哪条路：
   - `安全回到 HEAD/已提交基线`
   - 或 `以可重复方式最小重建`
4. 如果你动了 `DialogueChineseFontAssetCreator.cs`，必须明确说明：
   - 改它是为了避免再次生成坏资产
   - 不是顺手继续扩功能

## 六、最容易犯的错

- 又把这题挂回 `Day1` 业务尾账
- 只修 `SDF`，不修 `Pixel`
- 只把消费者引用改走，掩盖底层字体资产仍然是坏的
- 把 `Library` 本地缓存报错错判成主因
- 顺手把 `DialogueFontLibrary_Default.asset`、Prefab、Scene 一起改脏

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

1. `DialogueChinese SDF 现在是否已有有效 atlas / m_AtlasTextures`
   - 只能写 `是 / 否`
2. `DialogueChinese Pixel SDF 现在是否已有有效 atlas / m_AtlasTextures`
   - 只能写 `是 / 否`
3. `本轮是否动了 DialogueChineseFontAssetCreator.cs`
   - 只能写 `是 / 否`
4. `本轮是否动了任何业务 prefab / scene / font library`
   - 只能写 `是 / 否`
5. `最小运行验证里是否已消掉 Atlas missing / m_AtlasTextures missing`
   - 只能写 `是 / 否`
6. `如果没过线，第一真实 blocker 是什么`

一句话收口：

- **这轮只修共享 TMP 中文字体底座，让它重新可加载，不再把运行时炸锅挂回业务线。**

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
```
