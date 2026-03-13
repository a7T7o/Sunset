# NPC / farm / spring-day1 总恢复分类表（2026-03-13）

## A 类：本轮必须进入最终恢复
- 最终承载链：codex/main-reflow-carrier@c4c69cb8bec7390a17ee7fe497fd0c0caf4b9a8b，并在本轮继续承接 spring-day1 白名单恢复。
- NPC：已回流主项目的业务文件、线程锚点、回根仓库 rollout 语义。
- arm：已回流主项目的 PlacementManager.cs、PlacementValidator.cs、线程锚点、回根仓库 rollout 语义。
- spring-day1 白名单恢复对象：
  - Assets/111_Data/Story/Dialogue.meta
  - Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset
  - Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset.meta
  - Assets/Editor/Story/DialogueDebugMenu.cs
  - Assets/Editor/Story/DialogueDebugMenu.cs.meta
  - Assets/YYY_Scripts/Story/Managers/DialogueManager.cs（当前主项目承载面已具备增强版）
  - Assets/YYY_Scripts/Story/UI/DialogueUI.cs（当前主项目承载面已具备增强版）
- 三条线程：NPC、农田交互修复V2、spring-day1 的主项目语义锚点与 rollout 收口。
- 默认规则收口：主项目优先、worktree 降级为例外工具链。

## B 类：本轮必须保留，但不执行迁回
- codex/npc-generator-pipeline
- codex/farm-10.2.2-patch002
- codex/restored-mixed-snapshot-20260311
- 以上分支保留为历史对照、异常回函、恢复追溯与高风险隔离工具链，不再作为默认开发现场。

## C 类：本轮明确排除，交回原线程后续补回
- .codex/threads/OpenClaw/部署与配置龙穴V2/memory_3.md
- .codex/threads/Sunset/backup-script/
- 与 spring-day1 无关的其他线程 memory / 文档 / 场景 / 系统线 dirty。

## D 类：当前不确定归属，先保护，不迁回
- Assets/000_Scenes/Primary.unity
- 五套 TMP 中文字体资产当前工作树 dirty：
  - Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset
  - Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset
  - Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SDF.asset
  - Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SoftPixel SDF.asset
  - Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset
- 处理策略：本轮仅做快照保护，不纳入最终恢复提交；后续交回 spring-day1 自身线程判断是否需要单独补回。

## 当前结论
- 本轮不存在“关键文件归属不明”的恢复对象：NPC/farm/spring-day1 的最终恢复白名单已明确。
- 本轮也不存在“为了恢复必须混入无关 dirty”的前提；所有无关对象均已落入 C/D 类并隔离。
