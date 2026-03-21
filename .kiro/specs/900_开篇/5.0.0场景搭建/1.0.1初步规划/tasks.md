# 任务清单：1.0.1 初步规划

## 本阶段任务

- [x] 1. 建立父工作区与子工作区的基础文档框架
- [x] 2. 明确当前阶段只做规划与只读普查，不直接改真实 scene
- [x] 3. 完成首轮资产入口普查，确认 scene / prefab / tile / sprite 基础是否存在
- [x] 4. 固化后续场景骨架与 Tilemap 分层方案
- [x] 5. 固化“资产普查 → 骨架 → 底稿 → 结构层 → 装饰层 → 逻辑层 → 回读 → 交付”的执行顺序
- [x] 6. 确认下一阶段新 scene 命名、路径与最小创建方案
- [x] 7. 确认首版 prefab 候选池的整理粒度
- [ ] 8. 满足准入条件后进入真实场景搭建

## 本阶段完成判定

- [x] 工作区三件套与 `memory` 已建立
- [x] 首轮资产普查已落文档
- [x] 场景骨架与 Tilemap 分层方案已稳定
- [x] 后续执行顺序已明确
- [x] 下一阶段准入时的 scene 命名与创建方案已最终确定
- [x] 首版 prefab 候选池已收口为 `A 全收录 / B 按桶 / C 按需补入`

## 下一阶段最小动作

- [x] 1. 选定独立新 scene 名称与路径（当前推荐：`Assets/000_Scenes/SceneBuild_01.unity`）
- [x] 2. 创建 scene 骨架与 `Grid + Tilemaps`
- [ ] 3. 先画底稿，不急着放满 prefab
- [ ] 4. 用一轮 MCP 回读确认层级、Console 和基础视觉秩序

## 当前按执行顺序排好的未完成清单

- [x] 1. 收口 imported 快照：`shared-root-import_2026-03-20` 明确只作为“迁入证据快照层”保留，不再作为 live 正文继续追加
- [x] 2. 收口 `memory_1.md` 角色：明确它仅作为“迁入续卷 / 历史快照卷”保留，后续线程活跃记忆继续写 `memory_0.md`
- [x] 3. 以当前专属 worktree 现场重新复核 create-only 准入口径，并将旧的 shared-root 结论换成当前有效口径：当前未获 scene 写态准入，主阻塞是用户写线程裁定未下发；未见 Unity / MCP 占用冲突证据
- [x] 4. 等待你收完当前第二波回执，并裁定我是否成为下一位唯一的 Unity / MCP 写线程
- [x] 5. 修正 Unity / MCP 当前连接根目录：已确认当前 `projectRoot = D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`
- [ ] 6. 处理本次误写到 shared root 的 `SceneBuild_01` 错位资产，再恢复到合法施工现场
- [x] 7. 在 worktree 对应的合法 Unity 现场中，创建 `SceneBuild_01` 的最小 scene 骨架与 `Grid + Tilemaps`
- [ ] 8. 完成地图底稿：地表、道路、水体或高差构图
- [ ] 9. 完成结构层：建筑、边界、围栏、入口和主视觉模块
- [ ] 10. 完成装饰层：植被、小物件、中景节奏和空间填充
- [ ] 11. 完成逻辑层：碰撞、遮挡、锚点与必要辅助层
- [ ] 12. 用一轮 MCP / Console / 层级回读完成施工自检
- [ ] 13. 交付可继续精修的高质量场景初稿

## 开工前三个尾项

- [x] 1. `imported 快照`
  - 结论：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.kiro\specs\900_开篇\5.0.0场景搭建\shared-root-import_2026-03-20\` 定性为“迁入证据快照层”
  - 口径：保留只读证据价值，不再作为主正文或长期并行工作区追加内容
- [x] 2. `memory_1.md`
  - 结论：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_1.md` 定性为“迁入续卷 / 历史快照卷”
  - 口径：后续线程活跃记忆继续写 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\.codex\threads\Sunset\Skills和MCP\memory_0.md`
- [x] 3. `create-only 准入复核`
  - 结论：已按当前 worktree 口径完成重审
  - 口径：当前阻塞不是 Unity / MCP 占用，而是你尚未下发 scene 写态裁定；因此本轮继续停留在 docs / 规划 / 普查层

## 首个施工窗口最新阻塞 / 历史阻塞复盘

- [x] 1. `Unity 连接根目录错连`
  - 当前事实：本轮已复核当前 `projectRoot = D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001`
  - 影响：首个 checkpoint 已经在合法 worktree 现场完成，不再受 shared root 错连阻断
  - 结论：该阻塞已解除
- [ ] 2. `shared root 误写资产待处理`
  - 当前事实：本轮只读复核时仍看到 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity` 存在；但嵌套错位路径 `D:\Unity\Unity_learning\Sunset\Assets\000_Scenes\SceneBuild_01.unity\SceneBuild_01.unity` 已不存在
  - 结论：shared root 同名 scene 是否保留、替换或删除，需要单独裁定；它不再阻断本 worktree 内已完成的首个 checkpoint

## 首个 checkpoint 收口（2026-03-21）

- [x] 1. 在 `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` 完成 create-only 骨架创建
- [x] 2. 场景根层级已落地：`SceneRoot / Systems / Tilemaps / PrefabSetDress / GameplayAnchors / LightingFX / DebugPreview`
- [x] 3. `SceneRoot/Tilemaps/Grid` 与 8 个 Tilemap 已落地：`TM_Ground_Base / TM_Ground_Detail / TM_Path_Water / TM_Structure_Back / TM_Structure_Front / TM_Decor_Front / TM_Occlusion / TM_Logic`
- [x] 4. 文件级回读已完成：结果 JSON、场景 YAML、相机与 Tilemap `sortingOrder` 已复核
- [x] 5. 临时施工脚本 `Assets/Editor/CodexSceneBuild01Checkpoint.cs` 已删除，不保留到长期交付面
- [ ] 6. 下一步仍待裁定：是否从首个 checkpoint 扩到“地图底稿”阶段

## 后续主线路线

- [x] 1. 完成独立新 scene 的命名、路径与最小创建方案
- [x] 2. 在独立 scene 中搭建 `Grid + Tilemap` 基础骨架
- [ ] 3. 完成地图底稿：地表、道路、水体或高差构图
- [ ] 4. 完成结构层：建筑、边界、围栏、入口和主视觉模块
- [ ] 5. 完成装饰层：植被、小物件、中景节奏和空间填充
- [ ] 6. 完成逻辑层：碰撞、遮挡、锚点与必要辅助层
- [ ] 7. 通过 MCP / Console / 层级回读完成一轮施工自检
- [ ] 8. 交付可继续精修的高质量场景初稿

## ???? v1 checkpoint?2026-03-21?
- [x] 1. ? `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001\Assets\000_Scenes\SceneBuild_01.unity` ???? `TM_Ground_Base / TM_Ground_Detail / TM_Path_Water` ?? Tilemap ????
- [x] 2. `TM_Ground_Base` ??? 159 ??????? `m_Origin = {-9,-5,0}`?`m_Size = {19,11,1}`?
- [x] 3. `TM_Ground_Detail` ??? 107 ??? / ?????? `m_Origin = {-12,-6,0}`?`m_Size = {24,12,1}`?
- [x] 4. `TM_Path_Water` ??? 21 ??????? `m_Origin = {4,0,0}`?`m_Size = {7,5,1}`?
- [x] 5. ?????????? `Assets/Editor/CodexSceneBuild01MapDraft.cs` ? `.meta` ???????? live ?????
- [ ] 6. ????????????????????????????
- [ ] 7. ? Unity live / Console ???????????????? scene ????? Console ???
