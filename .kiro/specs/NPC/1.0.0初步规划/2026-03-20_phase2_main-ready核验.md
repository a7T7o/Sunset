# NPC Phase2 Main-Ready 核验

## 本轮结论
- `carrier_ready = no`
- `main_ready = yes`

## 依据
- 当前 `main` 与 `codex/npc-roam-phase2-003` 在 NPC 业务允许域内的真实树差异，已不再包含 `Assets/100_Anim/NPC/`、`Assets/111_Data/NPC/`、`Assets/222_Prefabs/NPC/`、`Assets/Sprites/NPC/`、`Assets/YYY_Scripts/Anim/NPC/`、`Assets/YYY_Scripts/Controller/NPC/`、`Assets/YYY_Scripts/Data/NPCRoamProfile.cs`、`Assets/Editor/NPCPrefabGeneratorTool.cs`、`Assets/Editor/NPCAutoRoamControllerEditor.cs` 这些运行时或编辑器交付物差异。
- 当前 `main` 已具备 `Primary.unity` 所依赖的 NPC prefab、anim、profile 与 runtime 基础链路，不再依赖 `codex/npc-roam-phase2-003` 上的 branch-only NPC 资产。
- 但 `codex/npc-roam-phase2-003` 相对当前 `main` 仍带有大量 NPC 允许域之外的历史治理/文档树差异，因此它还不能被视为“干净的最终 carrier”。

## 本轮 checkpoint 语义
- 本轮不再新增 NPC 业务代码或资源。
- 本轮仅固化 live 核验结论，作为后续继续处理 `codex/npc-roam-phase2-003` 时的 main-ready 基线。
