# SceneBuild_01 场景理解与精修施工方案快照（2026-03-22）

> 说明：本文件用于完整保留 2026-03-22 这轮对 `SceneBuild_01` 的场景理解、施工边界与精修方案判断，避免后续 MCP 调试或线程切换冲掉这份已形成的稳定认知。
>
> 下方内容按当时输出口径完整保留，只做文件化整理，不改结论方向。

## 先说结论

- 我现在的统一认知已经很明确：`SceneBuild_01` 不是“整村总图”，而是春 1 日 `14:20` 进村后到 `15:10` 农田/砍树教学前后的主承载场景，核心是“废弃小屋 + 院落 + 工作台 + 小块农田 + 教学落点”。这条口径来自 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\scene-build_handoff.md:1` 和 `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\requirements.md:1`。
- 这张 scene 现在已经不是空白骨架了；它已经有结构、装饰、锚点和触发框架，真正缺的是“叙事语义精修”，不是继续泛装饰。
- 如果下一轮正式开工，我的第一刀不会先建新文件夹、新脚本、新 prefab，而是优先只改 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:132` 这一份 scene。
- 按你这轮要求，本轮我没有改任何文件，也没有更新 memory。

## 现场与我自己的“身份证”

- 当前工作树：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 当前分支：`codex/scene-build-5.0.0-001`
- 当前 `HEAD`：`8e641e67`
- 当前 worktree `git status`：只有 4 个 TMP 字体资源 dirty；它们现在客观存在，但不属于我这条 `scene-build` 的下一轮施工面。
- 当前 shared root：`D:\Unity\Unity_learning\Sunset @ main`
- 当前 shared root `HEAD`：`c6af2657`
- 我已验证到 shared root 现在并不干净，所以“最终回 main”是方向，不是此刻立刻迁移的安全前提。

## 我现在对场景的完整理解

- 它是什么：住处安置场景、教学起步场景、生活启动场景。
- 它不是什么：不是整村、不是矿洞口、不是晚餐冲突区、不是纯摆景图。
- 它必须承载的动作：东侧进入、NPC 带入住处、院心对话、屋侧工作台闪回、农田教学、砍树教学、回屋/室内衔接。
- 它现在的骨架已经在 scene 里：`GameplayAnchors`、`DebugPreview`、`Systems`、`SceneRoot`、`LightingFX`、`PrefabSetDress`、`Tilemaps`，可直接在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:132`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:167`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:1539`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:1747`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:1784`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:2264`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:4157` 看到。
- 它现在已有主要承载体：`Structure_House_Main`、`Decor_Farmstead`、3 个装饰组簇、4 个锚点、2 个触发区，分别在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:4475`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5144`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5178`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5211`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5244`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5887`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5918`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5949`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:5980`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:6361`、`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity:6439`。
- 我对它的空间想象现在是：东侧有明确进入口，进入后不是直接散开，而是被导向一个“院心留白区”；左中是废弃小屋本体；屋侧/屋前有明确的工作台和生活区；农田教学区与砍树点贴着院落但不压院心；整张图服务“住下—恢复—学习—开始劳动”。
- 当前最大缺口不是“东西太少”，而是“语义不够硬”：入口抵达感、院心主焦点、工作台身份、农田/砍树教学落点、屋内外衔接，还没完全像剧情场景那样一眼就懂。

## 我下一轮会怎么做

- 第一刀只做 `主入口动线`：收紧东侧进入后的抵达顺序，让“被带入住处”更明确。
- 第二刀做 `院心留白`：把院落中心从“有东西”变成“能站、能看、能对话、能停镜头”。
- 第三刀做 `工作台语义`：优先把现在偏 generic 的院内道具，提升成明确的工作台/闪回焦点。
- 第四刀做 `教学落点`：把农田教学区和砍树教学区从“附近有地方”变成“我一看就知道该去哪里学”。
- 第五刀做 `屋内外衔接`：至少让屋外门口、回屋点、治疗/床铺链路在室外语义上成立。
- 第六刀才可能补少量装饰，但只补服务剧情的装饰，不再做泛装饰扩张。

## 我会具体动哪些文件

- 第一优先、也是下一轮真正的主战场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity`
- 我预计会优先改这几个现有对象：
  - `DecorCluster_YardLife`
  - `Decor_YardSupplies_01`
  - `Decor_YardSupplies_02`
  - `Anchor_Entry_EastGate`
  - `Anchor_Stand_YardCenter`
  - `Anchor_Interact_HouseYardSide`
  - `Trigger_EastGateApproach`
  - `Trigger_YardCore`
- 我预计会在 scene 内新增少量“空节点级语义点”，但不是新文件，可能包括：
  - `Anchor_Interact_Workbench`
  - `Anchor_Observe_FarmLesson`
  - `Anchor_Observe_TreeLesson`
  - `Anchor_Door_Exterior`
  - 必要时再加一个入口引导停点
- 我暂时不打算先创建新的文件夹。
- 我暂时不打算先创建新的脚本。
- 我暂时不打算先创建新的 prefab 资产文件。
- 真正施工完成一个稳定 checkpoint 后，我才会补写这些已有文档，而不是先堆文档：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\tasks.md:1`
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\1.0.1初步规划\memory.md:1`
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\memory.md:1`
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_0.md`

## 我现在不认为有必要立刻新建的东西

- 不需要先建“大全 prefab 索引”文档。
- 不需要先建新的子工作区。
- 不需要先补新的美术规范文档。
- 不需要先碰 `Primary` 做对照施工。
- 如果后面确实进入“按参考图高精度复原”的稳定阶段，我才会考虑补一个很轻量的语义索引文件，而且只放在当前子工作区，不会再开文档帝国。

## 我明确不会碰的东西

- `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\Primary.unity`
- 整村扩图
- 矿洞口/骷髅兵段
- 晚餐/饭馆/冲突区
- 这 4 个 TMP 字体资源
- shared root 上的 live 场景施工

## 一句话把我扒光

- 我现在不是在“想搭什么场景”，而是在等你放我进真实精修；一旦你说开始，我会先只在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` 里，把“东侧进入 → 院心留白 → 工作台闪回 → 农田/砍树教学 → 回屋衔接”这 5 个语义点做硬，不先加新文件夹、不先造脚本、不先堆 prefab。

## 原始收束句

如果你认可，我下一条就不再讨论，直接进入这份 scene 的第一轮精修施工。
