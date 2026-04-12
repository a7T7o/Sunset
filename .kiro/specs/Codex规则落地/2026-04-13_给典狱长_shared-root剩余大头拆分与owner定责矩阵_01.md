# 2026-04-13｜shared-root 剩余大头拆分与 owner 定责矩阵

## 一句话结论
- 资产大头已经由治理线程先代收两刀：
  - `8a3ad181` `2026.04.13_Codex规则落地_10`
  - `a54e2342` `2026.04.13_Codex规则落地_11`
- 当前 shared-root 剩余的真大头已经不再是非代码资产，而是代码、测试、Editor 工具和少量治理文档。
- 这轮之后，其他线程不该再靠聊天猜“哪些是自己的”；应直接按下表和对应 prompt 回收自己的尾账。

## 当前盘面
- 当前 `git status --porcelain=v1 -uall`：`251`
- 当前头部根：
  - `Assets/YYY_Scripts = 79`
  - `Assets/Editor = 66`
  - `Assets/YYY_Tests = 53`
  - `.kiro/xmind-pipeline = 36`
  - `Docx/大总结 = 7`
  - `.codex/threads = 4`
  - `.kiro/specs = 4`
  - `ProjectSettings = 2`
- 当前 `Assets` 非代码剩余只剩 3 项，而且都是代码目录 folder meta：
  - `Assets/Editor/Town.meta`
  - `Assets/YYY_Scripts/Story/Dialogue.meta`
  - `Assets/YYY_Scripts/UI/Save.meta`

## 责任矩阵

| 责任线程 | 当前主要根 | 代表样本 | 当前裁定 | 说明 |
|---|---|---|---|---|
| `spring-day1` | `Assets/Editor/Story`、`Assets/YYY_Scripts/Story`、`Assets/YYY_Tests/Editor` 中 `SpringDay1*` | `SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs`、`SpringDay1DirectorStagingWindow.cs`、`SpringDay1DirectorStagingTests.cs` | `继续发 prompt` | 当前剩余最大代码面，且仍是 live 主线，必须自己收 own 尾账 |
| `NPC` | `Assets/Editor/NPC`、`Assets/YYY_Scripts/Controller/NPC`、`Assets/YYY_Tests/Editor` 中 `Npc*` | `NPCAutoRoamController.cs`、`NPCBubblePresenter.cs`、`NpcCrowdManifestSceneDutyTests.cs` | `继续发 prompt` | 代码与测试尾账仍明显成组，不适合治理线程代吞 |
| `UI` | `Assets/YYY_Scripts/Story/UI`、`Assets/YYY_Scripts/UI`、`Assets/YYY_Tests/Editor` 中 UI 相关 | `InteractionHintOverlay.cs`、`PackagePanelTabsUI.cs`、`PackagePanelLayoutGuardsTests.cs` | `继续发 prompt` | 线程虽 parked，但 own 改动还留在 shared-root，应自收 |
| `树石修复` | `Assets/Editor` 中树石工具、`Assets/YYY_Scripts/Controller` 中树石控制器 | `Tool_004_BatchTreeState.cs`、`Tool_005_BatchStoneState.cs`、`TreeController.cs`、`StoneController.cs` | `继续发 prompt` | 上一轮已经证实 stone-only 会被 same-root remaining dirty 卡住，必须整组收 |
| `Codex规则落地` | `.kiro/xmind-pipeline`、`ProjectSettings`、零散 editor/helper/test roots | `ProjectSettings/QualitySettings.asset`、`ProjectSettings/EditorBuildSettings.asset`、`Chest*`、`ScenePartialSync*`、`xmind-pipeline/*` | `继续施工` | 这是治理线程 own 的剩余大头，不应甩给业务线程 |
| `项目文档总览` | `Docx/大总结` | `01_总览.md`、`04_剧情NPC.md`、`07_AI治理.md` | `治理线程可代收` | 线程已 parked，且这批是 docs-only，可由治理线程安全代收 |
| `UI系统/树石修复` parked docs | `.kiro/specs/*`、`.codex/threads/*` | `UI系统/memory.md`、`树石修复/memory_0.md` | `治理线程可代收` | 线程已 parked，docs-only，可由治理线程安全代收 |

## 不要再误判的点
- `Assets/Editor/Town.meta`、`Assets/YYY_Scripts/Story/Dialogue.meta`、`Assets/YYY_Scripts/UI/Save.meta` 不是代码文件，只是 3 个代码目录的 folder meta。
- `ProjectSettings/EditorBuildSettings.asset` 虽然是非代码，但它会改 build scene 列表；不能和普通截图/素材同口径盲提。
- `ProjectSettings/QualitySettings.asset` 虽然是配置，不是代码，但它直接影响打包表现层；应继续作为有设计后果的配置面处理。
- `.kiro/xmind-pipeline` 是治理线程 own 工具链，不属于业务线程 cleanup 范围。

## 本轮建议动作
1. 先由治理线程把 docs-only 和 3 个 folder meta 再收一刀。
2. 再按上表对 `spring-day1 / NPC / UI / 树石修复` 发“只收 own 尾账、不做新功能”的 prompt。
3. 治理线程自己保留 `ProjectSettings + xmind-pipeline + 零散 Codex own helper/test` 这组，不往外甩。
