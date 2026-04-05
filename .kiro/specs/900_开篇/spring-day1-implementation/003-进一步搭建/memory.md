# Spring Day1 - 003-进一步搭建 - 工作记录

## 模块概述
- 模块目标：在 `main` 真实现场上继续推进 spring-day1 的对话/UI/剧情闭环，并把对话系统逐步接回当前 NPC 与实际场景。

## 当前状态
- **完成度**：已进入继续搭建阶段，完成了 `NPC001` 的最小对话测试接入。
- **最后更新**：2026-03-13
- **状态**：进行中

## 会话记录

### 会话 1 - 2026-03-13
**用户需求**：在确认 NPC 生成工具与运行时代码已在 `main` 的前提下，审查当前 NPC 基础挂载是否合格，思考是否应该给所有 NPC 默认搭载某些内容，并授权给 `NPC001` 重新搭载对话测试相关脚本和内容，用于验证当前对话系统是否正常。
**完成任务**：
- 依据场景修改规则先做现状审查；由于 Unity MCP 当前 `Transport closed`，本轮改为直接读取 `Assets/222_Prefabs/NPC/001.prefab`、`002.prefab`、`003.prefab` 取证。
- 确认三个 NPC 预制体当前共有一致的基础链：`SpriteRenderer`、`Animator`、排序脚本、`CircleCollider2D`、`NPCAnimController`、`NPCMotionController`；目前都没有 `NPCDialogueInteractable`。
- 收敛默认挂载策略：所有 NPC 适合默认携带“显示/排序/移动/基础交互载体”这类通用组件；不建议默认给所有 NPC 挂具体对话脚本与剧情资源，语义层配置应按 NPC 单独绑定。
- 仅对 `Assets/222_Prefabs/NPC/001.prefab` 做最小配置补充：新增 `NPCDialogueInteractable`，并绑定 `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`，用于当前对话链测试。
- 追加核查 `NPCMotionController.cs`，确认其 `rb == null` 时会在运行时自动尝试获取 `Rigidbody2D`，否则回退到 `Transform` 位移检测；因此不会成为本次对话测试的直接阻断。
**修改文件**：
- `Assets/222_Prefabs/NPC/001.prefab` - [修改]：新增 `NPCDialogueInteractable`，绑定 `SpringDay1_FirstDialogue.asset`。
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [新增]：建立本子工作区记忆并记录当前起点。
**解决方案**：在 MCP 失联、当前场景 live 状态无法直接读取的情况下，优先采用“只改 `NPC001` 预制体、不动布局与层级”的最小方案，把对话测试能力尽快接回当前 NPC 链路。
**遗留问题**：
- [ ] Unity MCP 仍为 `Transport closed`，当前无法直接核查编辑器里 `NPCs/001` 的 live 挂载状态。
- [ ] `Assets/222_Prefabs/NPC/` 当前整体未进入 Git 跟踪，本轮修改已落到本地资源，但尚未进入版本管理。
- [ ] 下一步应以 `NPC001` 为入口做一次真实对话链手工验收，重点确认：可触发、可推进、时间暂停、生效字体、对话结束关闭。

### 会话 2 - 2026-03-14
**用户需求**：重新全面排查对话 UI 的显示/隐藏问题，确保显隐逻辑没有明显缺口；同时完整回顾当前对话内容的设计方式、运行流程与存储结构。
**完成任务**：
- 先按场景修改规则审查 `DialogueUI.cs` 与 `Primary.unity` / `DialogueValidation.unity` 的现状，确认当前问题属于“场景结构与显隐控制作用域错位”，而不是剧情数据本身错误。
- 核查证据链后确认根因：`DialogueCanvas` 下的 `DialoguePanel`、`头像`、`SpeakerNameText`、`ContinueButton`、`DialogueText` 都是同级子物体；但旧版 `DialogueUI` 在 `root == null` 时会把 `root` 自动指向 `DialoguePanel`，随后 `CanvasGroup` 也挂到 `DialoguePanel`，导致淡入淡出只控制到面板背景而不是整套对话 UI。
- 修改 `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`：`root` 的默认回退从 `DialoguePanel` 改为脚本所在对象（`DialogueCanvas`），并新增 `ResolveCanvasStateTarget()` / `IsInScope()`，即使未来有人把 `root` 显式配成某个子节点，只要它不能覆盖按钮、文本、头像，就自动回退到 `DialogueCanvas` 作为显隐目标。
- 复核影响面：本次没有改 `Primary.unity` 坐标、层级、Inspector 布局值，只修了控制逻辑；`DialogueValidation.unity` 由于也是同样的同级结构，因此同步受益，不会再出现“只隐藏面板、不隐藏按钮/文本”的错位。
- 额外回读并整理当前对话系统架构与数据路径，为后续 UI/剧情验收提供完整回顾基线。
**修改文件**：
- `Assets/YYY_Scripts/Story/UI/DialogueUI.cs` - [修改]：修正显隐控制作用域，统一由 `DialogueCanvas` 接管整套对话 UI 的可见性。
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [追加]：记录显隐问题根因与修正。
**解决方案**：不去硬改你当前场景布局，而是把 `DialogueUI` 的显隐作用域修成与现有层级结构一致，让 `CanvasGroup` 永远优先控制整套 `DialogueCanvas`，从根上消掉“按钮和文本留在场上、面板却单独淡出”的问题。
**遗留问题**：
- [ ] Unity MCP 仍不可用，当前还不能直接从编辑器现场抓取 PlayMode console / live hierarchy 证据。
- [ ] 本轮已完成代码级根因修正，但仍需要你在 Unity PlayMode 里再验一次 `Primary` 场景实际显隐表现。

### 会话 3 - 2026-03-15
**用户需求**：再次尝试 Unity MCP，并基于 MCP 真实现场结果，干净整洁地汇报当前全部进度、剩余内容、下一步动作，以及每一步完成后可以验收到什么。
**完成任务**：
- 重新测试 Unity MCP，确认其已恢复可用：成功读取 request context、当前活跃场景为 `Primary`，路径 `Assets/000_Scenes/Primary.unity`。
- 用 MCP 读取 `Primary` live 现场，确认 `DialogueCanvas` 当前真实路径为 `UI/DialogueCanvas`，实例 ID 为 `106978`；`NPC001` 当前真实路径为 `NPCs/001`，实例 ID 为 `-50286`。
- 进一步确认 `NPC001` live 现场已经挂上 `NPCDialogueInteractable`，并且 `dialogueSequence` 已真实指向 `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`，说明 NPC→对话入口链当前在编辑器现场已经接上。
- 进一步确认 `DialogueCanvas` 当前编辑态仍处于“序列化字段空、依赖运行时自动补线”的状态：`DialogueUI` 组件上 `root/speakerNameText/dialogueText/continueButton/portraitImage/backgroundImage/canvasGroup/fontLibrary` 当前 live 读取均为空，这与之前从 YAML 读到的结论一致。
- 使用 MCP 触发 Unity 刷新/请求编译；第二次按正确参数调用成功，当前未读到新的 console error。控制台中仅见 1 条既有 obsolete warning（`NPCPrefabGeneratorTool.cs`）和若干与本轮对话链无直接关联的 Animator / Playback 日志。
**修改文件**：
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [追加]：记录 MCP 恢复连通、live 现场证据与当前主线状态。
**解决方案**：当前已从“只能靠磁盘推断”回到“可通过 MCP 直接读编辑器现场”的状态；后续对 spring-day1 的 UI/NPC/剧情收尾，应优先基于 live 证据迭代，而不是继续仅靠 YAML 猜测。
**遗留问题**：
- [ ] `DialogueUI` 仍处于运行时自动补线模式，尚未在场景里落成稳定的 Inspector 显式引用。
- [ ] `fontLibrary` 当前 live 读取为空，说明字体库仍未真正在场景 `DialogueUI` 上挂好。
- [ ] 对话显隐修复虽然已落代码，但还需要用 PlayMode 真实跑一次，确认整套 `DialogueCanvas` 的淡入淡出表现与关闭收尾一致。

### 会话 3 - 2026-03-15
**用户需求**：重试 Unity MCP，并在此基础上干净、有条理地汇报当前全部进度、剩余内容、下一步动作，以及每一步完成后分别能验收到什么。
**完成任务**：
- 成功恢复并验证 Unity MCP 连通性：`debug_request_context`、`manage_scene/get_active`、`manage_scene/get_hierarchy`、`read_mcp_resource` 均已正常返回；当前活动实例为 `Sunset@21935cd3ad733705`，活动场景为 `Primary`。
- 通过 MCP 现场确认 `UI/DialogueCanvas` 存在且激活，当前组件链为 `RectTransform + Canvas + CanvasScaler + GraphicRaycaster + DialogueUI`；也确认 `NPCs/001` 当前真实已挂 `NPCDialogueInteractable`，而 `002/003` 仍无对话脚本。
- 通过 MCP 读到 `DialogueCanvas` 上 `DialogueUI` 的当前序列化/运行态属性：`root / speakerNameText / dialogueText / continueButton / portraitImage / backgroundImage / canvasGroup / fontLibrary` 目前都显示为空；结合代码可知，前 7 项依赖运行时自动补线，而 `fontLibrary` 当前没有自动补线逻辑，是一个真实剩余配置缺口。
- 通过 MCP 控制台确认当前项目存在新的编译阻断：`Assets/YYY_Scripts/Controller/Input/GameInputManager.cs` 报 4 条语法/预处理相关错误，属于项目级真实错误，而非 MCP 传输问题。
- 进一步只读核对 `GameInputManager.cs` 后确认：文件内 `#region/#endregion/#if/#endif` 结构确实异常，当前无法把 PlayMode 对话验收判断为已闭环。
**修改文件**：
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [追加]：记录 MCP 已恢复、现场证据、新编译阻断与剩余配置缺口。
**解决方案**：本轮不抢修业务细节，先把“工具链状态”和“项目真实阻断”分开：MCP 已恢复可用；当前首要问题已经切换为 `GameInputManager.cs` 编译阻断，其次是 `DialogueUI.fontLibrary` 仍未接到 `Primary` 场景。
**遗留问题**：
- [ ] `GameInputManager.cs` 当前编译错误会阻断后续任何 PlayMode 级对话验收。
- [ ] `DialogueUI.fontLibrary` 当前仍为空，后续即便显隐修好，也无法在 `Primary` 场景完整验收字体切换链。
- [ ] `Primary` 场景的对话显隐修复还缺一次“项目可编译前提下”的现场复验。
### 会话 4 - 2026-03-16
**用户需求**：在已发放 `DialogueUI.cs` 与 `Primary.unity` 两把 A 类锁的前提下，只完成一个 checkpoint：收口 `DialogueCanvas` 显隐、收口必要引用 / `fontLibrary` 闭环，并做 `NPC001 -> 对话 -> 结束` 的最小验收。
**完成任务**：
- 通过 Unity MCP 复核 `Primary` live 现场，确认 `DialogueCanvas` 的 `DialogueUI` 仍缺 `root/speakerNameText/dialogueText/continueButton/portraitImage/backgroundImage/canvasGroup/fontLibrary` 显式引用，且 `头像/Icon` 缺少 `Image`，`DialogueCanvas` 缺少 `CanvasGroup`。
- 只在 `Assets/000_Scenes/Primary.unity` 内补齐最小闭环：为 `DialogueCanvas` 新增 `CanvasGroup`，为 `头像/Icon` 新增 `Image`，并把 `DialogueUI` 所需 Inspector 引用与 `Assets/111_Data/UI/Fonts/Dialogue/DialogueFontLibrary_Default.asset` 全部显式落盘；未改现有布局、层级和尺寸。
- 使用 PlayMode + `DialogueDebugMenu` 走通 `NPC001` 真实入口；确认继续按钮能推进到下一句，字体切换生效（garbled -> `DialogueChinese SoftPixel SDF`，default -> `DialogueChinese V2 SDF`，retro -> `DialogueChinese SoftPixel SDF`），结束后 `IsDialogueActive=False`、`CanvasAlpha=0.00`、`TimePaused=False`、`InputEnabled=True`。
- 额外确认本轮最早抓到的 `CanvasAlpha=0.00` 假阴性，根因是 Unity 处于 `Play + Pause`；恢复运行后，显隐链路按预期工作。
**修改文件**：
- `Assets/000_Scenes/Primary.unity` - [修改]：补 `DialogueCanvas` 的 `CanvasGroup`、`头像/Icon` 的 `Image`，并显式写入 `DialogueUI` 所需场景引用与字体库引用。
- `Assets/YYY_Scripts/Story/UI/DialogueUI.cs` - [沿用]：继续使用此前已落地的显隐作用域修复版，本轮未新增代码改动。
- `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/memory.md` - [追加]：记录本轮 checkpoint 的场景收口与最小验收结果。
**解决方案**：保持现有布局完全不动，只通过显式 Inspector 引用和缺失组件补齐，让当前 `DialogueUI` 修复版稳定接管整套 `DialogueCanvas`；PlayMode 侧通过现有调试菜单做最小闭环验收，不扩写第二目标。
**遗留问题**：
- [ ] 当前只完成了逻辑链最小验收；后续仍建议在 Unity 可视界面内补一次纯手工视觉验收，专看版式、美术和节奏观感。
- [ ] 任意键推进、防双触发、更多剧情/NPC 扩展不在本轮 checkpoint 范围内，留待下一轮任务处理。

### �Ự 5 - 2026-03-16
**�û�����**���ھ��տ���ɺ󣬲�����������/���壬�������º˶� spring-day1 ��ǰ��ʵ Git ��㣬���ж� Day1 ��������һ����ֵ���ƽ����¹��ܡ����ļ�Ӱ�췶Χ����һ����С checkpoint��
**�������**��
- ���˵�ǰ��ʵ�ֳ�������Ŀ¼Ϊ `D:\Unity\Unity_learning\Sunset`����ǰ��֧Ϊ `codex/npc-asset-solidify-001`��`HEAD=8805542555b557f65c5d3ed21aacc2c7f8285d8d`��˵���˿̲��������ʺ�ֱ�ӿ��� spring-day1 ��ʵ�ֵ�ר�÷�֧��
- ���� `spring-day1` requirements������ `DialogueManager / DialogueSequenceSO / DialogueNode / StoryEvents / NPCDialogueInteractable`��ȷ�ϵ�ǰ�Ի����Ѿ߱�������/���ֻ�/UI/������/NPC������������ȱ�١��Ի���ɺ����� Day1 �����ƽ�������ʽ������
- ������һ����ֵ���ƽ����¹���Ϊ��**�׶ζԻ���ɺ�ľ����ƽ�����**����С����ǡ��Ի�����¼� + languageDecoded/�׶α���л� + ͬһ NPC ��ǰ�����з��������ѵ�ǰ���Լ��Ի��ջ�����Ϊ�����ɳн� Day1 ���ߵľ�����ڡ�
- Ӱ���жϣ��ù���Ӧ�������Ϊ**���� `Primary.unity`������ `GameInputManager.cs`���������� A �๲�����ļ�**���������� `Assets/YYY_Scripts/Story/` �µķ� hot-file �ű��� `Assets/111_Data/Story/Dialogue/` �����ʲ���
**�޸��ļ�**��
- `.kiro/specs/900_��ƪ/spring-day1-implementation/003-��һ���/memory.md` - [׷��]����¼��ǰ��ʵ��֧�������һ�����жϡ�
**�������**����������ʽ������Ӧ�ȴӸɾ������¿� `codex/spring-day1-...` �����֧������ task ������ͬ������Ӧֱ���ڵ�ǰ `codex/npc-asset-solidify-001` ��֧�ϼ����ƽ� Day1 ��ʵ�֡�
**��������**��
- [ ] ��һ����ʵʵ��ǰ���������е� spring-day1 �Լ��ĸɾ� `codex/` �����֧��
- [ ] ��һ�� checkpoint ����۽����Ի�����¼�/������/�׶�ǰ�����з�������һ�����ɢ�� UI ���ļ������������֡�

### �Ự 6 - 2026-03-16
**�û�����**��ֱ�ӿ�ʼ spring-day1 ����һ����ʵ����������ʵ�֡��Ի���ɺ��ƶ� Day1 ����ǰ��������С checkpoint���������� `codex/` �����֧ + task ������ͬ��ִ�С�
**�������**��
- �� `main@9b9a6bd0dd7c5ee7d18cc82e3ea9da74a146bf9d` �¿������ֳ� `codex/spring-day1-story-progression-001`���������ͣ������ spring-day1 �޹ص� `codex/npc-asset-solidify-001` �Ͽ�����
- �� `DialogueSequenceSO` ������С�����ƽ��ֶΣ�`markLanguageDecodedOnComplete` �� `followupSequence`���� `DialogueManager` ���롰��Ȼ���������ɡ����տ��߼�������ʱ��������м��� `HasCompletedSequence`���Լ� `DialogueSequenceCompletedEvent` / ���� `DialogueEndEvent` �¼����ݡ�
- �� `NPCDialogueInteractable` ��Ϊ������ʱ����״̬�Զ�ѡ���׶λ� follow-up ���У��׶β����д����ɱ�ǣ�������и�����룬��ͬʱ�� `IsLanguageDecoded` �е� true�������ٴν���ͬһ NPC ʱ�Զ��Ĳ��������С�
- ���� `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset`������ `SpringDay1_FirstDialogue.asset` ��ɡ���ɺ���� + follow-up ���á���ȫ��δ�� `Primary.unity`��`GameInputManager.cs` ������ A �๲�����ļ���
- ���������ִ֧�� `git-safe-sync.ps1 -Action sync -Mode task -IncludePaths ...` ��ɰ������ύ�����ͣ�`a9c952b717395c561c0f50a55bf3382dd7c4c925`���� hash `a9c952b7`����
**�޸��ļ�**��
- `Assets/YYY_Scripts/Story/Data/DialogueSequenceSO.cs` - [�޸�]�����������ƽ��ֶΡ�
- `Assets/YYY_Scripts/Story/Events/StoryEvents.cs` - [�޸�]������ `DialogueSequenceCompletedEvent`����չ `DialogueEndEvent` ���ݡ�
- `Assets/YYY_Scripts/Story/Managers/DialogueManager.cs` - [�޸�]��������������տڡ�����ʱ��ɱ��������л���
- `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs` - [�޸�]��������״̬�Զ������׶�/�����Ի���
- `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset` - [�޸�]��������ɺ����������������á�
- `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset` - [����]���׶κ�������Ի����ݡ�
- `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset.meta` - [����]���¶Ի��ʲ� meta��
**�������**���ѡ������ƽ�����С������ `Story` �ű���Ի��ʲ��㣬�����Ǽ��������������������ļ���������ǰ Day1 �Ѵӡ��ܲ��Ի�������Ϊ���Ի��ܸı�����ʱ����״̬��������ͬһ NPC ����һ�ζԻ�����
**��������**��
- [ ] �����ڸ��� worktree ���ʵ���� Git �̻�����δ�ڸ� worktree ��Ӧ�� Unity Editor �����ֳ� Play ��֤����һ���������Ӧ�����ڿɼ��������ֹ����գ���һ�ν������׶����룬�ڶ��ν����Զ�������������
- [ ] ������������ͨ������һ�����Ȼ���ƽ����ǰ� `DialogueSequenceCompletedEvent` �����ӵ� Day1 �Ľ׶ι���/��ѧ����ϵͳ��������ֻͣ���ڶԻ������㡣

### 会话 7 - 2026-03-24
**用户需求**：不要再散着收 Day1 工作台；直接为 `spring-day1` 线程写一份新的执行 prompt，要求把当前工作台 UI、任务限制、血量/精力条、制作耗时与动画、交互距离与上下翻转、首次 `E` 提示气泡，以及“用户必须能手动拖动精调 UI”这 7 点原文完整摘录进去，并按老规范给出强约束回执格式。
**完成任务**：
- 新建 `26.03.24-Day1工作台UI与任务体验重收口委托-01.md`，作为当前 `003-进一步搭建` 阶段的正式执行 prompt。
- 在 prompt 中完整保留了用户 7 点原文，不做删意缩写。
- 将本轮真实目标收口为：工作台 UI 正式重做、用户可手调、Day1 任务严格约束、血量/精力条重做、工作台制作过程表现、距离/关闭/上下翻转/一次性 `E` 气泡统一收口。
- 单独把“为什么当前 UI 不能拖、改了什么之后可以拖”写成硬回执项，避免线程再次绕过去。
**修改文件**：
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.24-Day1工作台UI与任务体验重收口委托-01.md` - [新增]：spring-day1 新一轮正式执行 prompt。
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md` - [追加]：记录本轮 prompt 落盘。
**解决方案**：不再给 spring-day1 零碎口头要求，而是把这轮 Day1 工作台/UI/任务体验重收口直接写成一个带硬边界、硬回执、硬验收项的阶段 prompt，让后续执行不再漂移。
**遗留问题**：
- [ ] 线程后续仍需自己把这 7 点真实落地，不允许停在“理解了”或“分析了”。
- [ ] 如果用户后续在 prompt 的“用户补充区”继续加话，执行线程必须先吸收再推进。
## 2026-03-24 补记：Day1 工作台/UI/任务体验重收口已先清编译阻塞并完成代码侧收口
- 当前主线仍是 `26.03.24-Day1工作台UI与任务体验重收口委托-01.md` 这轮 Day1 工作台/UI/任务体验收口，没有漂移去别的系统。
- 本轮先按 `skills-governor` 做 Sunset 等价启动核查，并回读：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\26.03.24-Day1工作台UI与任务体验重收口委托-01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\steering\ui.md`
  - 当前子工作区 / 父工作区 / 线程记忆
- 关键代码收口：
  - `SpringDay1WorkbenchCraftingOverlay.cs`：补回正式 `EnsureRecipesLoaded()`，左侧配方列固定从 `Resources/Story/SpringDay1Workbench` 载入 `Axe_0 / Hoe_0 / Pickaxe_0`，并按手写 `RectTransform` 布局重做工作台浮层；当前不再依赖 `LayoutGroup / ContentSizeFitter` 锁死子节点，因此用户在 Play 时可直接选中运行时生成的子对象做微调，根节点仍会继续跟随工作台。
  - `CraftingStationInteractable.cs`：继续使用多 Collider 最近边界点做交互距离与上下翻转判断，保留 `E` 一次性提示气泡与 `1.5m` 超距关闭口径。
  - `SpringDay1Director.cs`：木材教学维持“浇水完成后才 arm，之后只累计新增木材”的严格判定；并把自动绑定工作台的 `interactionDistance` 收回到 `0.5f`，不再是旧的宽松距离。
  - `SpringDay1PromptOverlay.cs`、`SpringDay1StatusOverlay.cs`、`HealthSystem.cs`、`EnergySystem.cs`、`SpringDay1WorldHintBubble.cs`：继续接成 Day1 任务卡、正式 HP/EP 卡片和一次性工作台气泡链，旧 Slider 保持隐藏。
- 本轮验证：
  - `git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread spring-day1 -IncludePaths ...`
  - `CodexCodeGuard` 已对 8 个 C# 文件完成 UTF-8 / diff / `Assembly-CSharp` 编译检查，先前 `WorkbenchCraftingOverlay.cs` 的语法错误与缺方法阻塞已清零。
- 当前恢复点：
  - 代码侧已经回到“可编译、可继续做用户复测”的状态；
  - 下一步只需按白名单提交本轮 Day1 工作台/UI 代码面，不卷入 shared root 其他线程 dirty。
## 2026-03-24 补记：Day1 工作台/UI 与任务体验重收口委托-02 已完成代码侧收口
- 当前子工作区主线仍是 26.03.24-Day1工作台UI与任务体验重收口委托-02.md，没有漂移到别的系统。
- 本轮先按 skills-governor + sunset-workspace-router 的 Sunset 等价启动流程复核 live 现场，确认：
  - cwd = D:\Unity\Unity_learning\Sunset
  - ranch = main
  - HEAD = 84fc3818f8049d3cd6a5697f87f288429b2b361c
- 代码侧已落地并保留的收口面：
  - CraftingStationInteractable.cs：工作台距离判定改为“优先 Sprite 视觉边界 / physics shape，失败再回退 Collider”的统一边界语义；同时保留首次 E 气泡的持久化键。
  - SpringDay1WorkbenchCraftingOverlay.cs：工作台浮层改为真正跟随世界位置，左侧配方列显示 Axe_0 / Hoe_0 / Pickaxe_0 的图标、名称、制作耗时，右侧详情区维持正式游玩样式；并尝试驱动工作台 Animator 的制作布尔参数。
  - SpringDay1UiLayerUtility.cs：新增页面级 UI 阻挡判断，让 Tab / 背包 / 页面 UI 压住工作台浮层、任务卡、E 气泡。
  - NPCDialogueInteractable.cs + SpringDay1WorldHintBubble.cs：NPC 线补齐近距 E 键交互和统一风格提示气泡。
  - SpringDay1PromptOverlay.cs + SpringDay1Director.cs：任务提醒重做为左中任务页卡，支持逐条完成动画、阶段翻页，并在对话 / 工作台 / 页面 UI 打开时主动隐藏。
  - Recipe_9100_Axe_0.asset、Recipe_9101_Hoe_0.asset、Recipe_9102_Pickaxe_0.asset：补齐正式 craftingTime，继续复用现有 RecipeData / CraftingService 体系。
- 本轮静态验证已通过：
  - git diff --check（本轮白名单文件）
  - sunset-git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread spring-day1 -IncludePaths ...
  - 代码闸门通过，已对 8 个 C# 文件完成 UTF-8 / diff / 程序集级编译检查，程序集为 Assembly-CSharp, Tests.Editor
- 本轮没有再调用 MCP，也没有做 Unity live 写或 PlayMode 复测；当前最准确定性是：**代码侧收口完成，运行态最终观感验收仍待用户复测**。
- 当前恢复点：
  - 如果用户继续验收，应直接按 委托-02 关注工作台视觉距离、任务卡动画、页面级 UI 优先级、NPC E 提示与工作台动画触发；
  - 若用户认可本轮代码口径，再做白名单同步，不卷入 shared root 其他线程 dirty。
## 2026-03-25 ���ǣ��ѻ�������ͼƬ�������߼���ƫ����Ϊ Day1 �����ָ���ί��
- ��ǰ�ӹ���������û�и��⣬��Ȼ�� Day1 ����̨/UI/�����������տڣ�ֻ���û��ֲ��˸�Ӳ��ͼƬ���պ��߼���ƫ����˲��ܼ������á�������տڣ����û����⡱��ͣ����
- ������������
  - [26.03.25-Day1����̨UI�������������տ�ί��-03.md](/D:/Unity/Unity_learning/Sunset/.kiro/specs/900_��ƪ/spring-day1-implementation/003-��һ���/26.03.25-Day1����̨UI�������������տ�ί��-03.md)
- ��ί������ս��� 4 ���ؼ��£�
  1. ���û����� 4 ��ͼֱ�ӵ�����������֤�ݣ��������߳���˵���������ˡ���
  2. �ѹ���̨ `0.5m` �����߼���ʽ�տ�Ϊ�����������ߡ����壺Χ�ƿɼ�/�ɽ����������ȶ��߽磬�����������ĵ��ֲ� union ��Ū��
  3. Ҫ���̲߳𿪡��̳���һ�����������ݡ��͡�������ཻ����ʾ�������������̨��ʾֻ����һ�κ��������ʧ��
  4. Ҫ���߳��Լ���������̬���飬������ͣ�ڡ����û�����һ���������ա���
- ��ǰ�ָ��㣺
  - spring-day1 ��һ�ֲ�Ӧ�ٰ� ί��-02 �ġ������տڴ��û��⡱�����ƽ���
  - Ӧֱ�Ӷ�ȡ ί��-03������ͼƬ+�߼�˫���ջ������ĸ�Ӳ�ھ���������## 2026-03-25 补记：委托-03 本轮已完成代码侧收口并尝试运行态自验
- 当前子工作区主线仍是 `26.03.25-Day1工作台UI与任务体验重收口委托-03.md`，没有漂移到别的系统。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-unity-validation-loop`
  - `unity-mcp-orchestrator`
- `sunset-startup-guard` 本会话未显式暴露；已按 Sunset `AGENTS.md`、live `cwd/branch/HEAD`、MCP 占用文档与工作区委托做手工等价闸门。
- 本轮代码侧收口面：
  - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
  - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
  - `Assets/Editor/Story/SpringDay1WorkbenchSceneBinder.cs`
  - `Assets/Editor/Story/SpringDay1BedSceneBinder.cs`
- 这轮已落地的关键决策：
  - 工作台交互改成“交互包络线”语义：优先走 `PolygonCollider2D/EdgeCollider2D/CompositeCollider2D` 轮廓，再回退到 Sprite physics shape / 可视边界；玩家采样点改为脚底交互点，不再直接拿 transform 中心。
  - 工作台提示拆成双语义：第一次靠近显示教程型 `按 E 打开`；之后常规近距提示继续存在，不再和一次性提示混成一层。
  - 工作台浮层继续跟随工作台世界位置，上下方向仍按玩家相对工作台的垂直关系切换；同时修正 `Place()` 错误，避免左列空壳和右侧被挤爆。
  - 任务卡收成“一任务一页”，并在任务切页前补一拍完成动画；木料进度改成从 `FarmingTutorial` 阶段开始累计新增，不再在木料步骤激活时清零，因此同时堵住 bypass 和死锁。
- 本轮静态验证：
  - `git diff --check`（当前白名单文件）通过
  - `SpringDay1DialogueProgressionTests` EditMode 切片通过（10/10）
  - 上述 Day1 相关脚本逐个 `validate_script` 均为 0 error，仅剩通用 warning
- 本轮运行态自验尝试结果：
  - 已进入 Unity live 并尝试按 Day1 验收菜单推进；
  - 但被 shared root 中他线未收口文件 `Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs` 的编译错误阻断，无法继续给出可信的 Play 观感结论；
  - 相关阻断不是 spring-day1 本轮代码造成，而是农田线未提交/未清掉的 live 红字。
- 当前恢复点：
  - spring-day1 这轮代码面已到“可编译切片 + 逻辑口径已收”的状态；
  - 若要完成委托-03 的最终运行态回执，需先排除农田线 `FarmRuntimeLiveValidationRunner.cs` 的 shared root 编译阻断，再重跑 Play 自验和截图验收。

## 2026-03-25 补记：委托-04 已从 farm 编译阻断切到 Day1 live 自验，但当前 MCP/Play 取证只完成首段，剩余被 live 稳定性卡住
- 当前子工作区主线仍是 `26.03.25-Day1工作台UI与任务体验重收口委托-04.md`，这轮没有再扩写 Day1 新代码，只做 Play 自验与验收交接取证。
- 本轮显式使用：`skills-governor`（手工等价 Sunset startup guard）、`sunset-unity-validation-loop`、`sunset-acceptance-handoff`。
- 已确认的 live 事实：
  - shared root 当前 `farm` 编译红字已不再出现；`委托-03` 的旧 blocker 已解除。
  - `unityMCP` 会话、实例与项目根均对齐到 `D:/Unity/Unity_learning/Sunset`。
  - 通过 `DialogueDebugMenu -> Bootstrap / Log Snapshot / Step Validation`，已成功拿到 Day1 首段运行态证据：
    - `Phase=CrashAndMeet`
    - 可触发 `NPC 001` 首段对话
    - 对话进行时 `Prompt alpha=0.00`
    - `Input=False`
    - `Time paused depth=2`
    - `NPC roam=False`（after-step 快照）
  - 说明 Day1 的首段入口、对话期间任务提示压低、暂停与输入锁链在 live 中至少成功命中过一次。
- 当前新的 live 阻塞不再是 farm 编译，而是 MCP/Play 稳定性：
  - Play 窗口在后续继续 Step 时出现会话抖动与意外退回 Edit Mode；
  - 当前 Console 精确噪音为：
    - `There are no audio listeners in the scene`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs:18` 的 `go.IsActive()` assert
    - `Library/PackageCache/com.coplaydev.unity-mcp.../GameObjectSerializer.cs:501` 的 Animator/Playback 读取报错
  - 因此这轮未能在同一可信 Play 窗口内把工作台阶段、木料阶段和任务翻页阶段完整跑穿。
- 当前恢复点：
  - Day1 本线已从“编译被 farm 卡住”恢复到“首段运行态已取证，剩余是 live 稳定性 / 人工终验问题”；
  - 下一步应在一个稳定的 Play 窗口里继续跑 `WorkbenchFlashback -> FarmingTutorial`，并用正式验收单交给用户终验工作台、木料任务与一任务一页翻页承接。

## 2026-03-25 补记：Day1 工作台 UI / 任务卡按最新 6 条要求重做为正式版骨架
- 当前子工作区主线仍然是 Day1 工作台 / 任务体验重收口；本轮不是新开题，而是继续落实用户那 6 条最硬要求：任务卡自适应 + 日历撕页、工作台左列可见滚动、右侧材料区可滚动、制作按钮/进度状态机、离台小进度、去掉相对漂移。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff`
- `sunset-startup-guard` 本会话未显式暴露；已按 Sunset `AGENTS.md`、live `cwd/branch/HEAD`、当前工作区与 `.kiro/steering/ui.md` 做手工等价前置核查。
- 本轮代码面只动了两份 UI 脚本：
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
- 已落地的关键实现：
  - `SpringDay1PromptOverlay.cs`：改成双页结构，`PlayPageFlip` 不再是横向压缩，而是前页抬起 + 后页承接的日历式翻页；页内文本、任务列表、焦点条、footer 改成内容驱动布局，避免文字换行后下方内容重叠。
  - `SpringDay1WorkbenchCraftingOverlay.cs`：左侧 `RecipeColumn/Viewport/Content` 改成稳定滚动链；右侧 `DetailColumn` 改成描述/材料/数量/提示/制作区的自适应结构；制作时按钮进入 hover 可见、移出隐身但保留进度条的状态机；离开工作台后保留小型悬浮进度块；`Reposition()` 取消平滑漂移，改成固定锚定。
- 本轮只读验证结果：
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 通过
  - `validate_script(standard)`：两份脚本均 `0 error / 1 warning`，warning 为字符串拼接的通用 GC 提示，不是 blocking error
- 当前 owned errors：none
- 当前 external blockers：none（至少这两份 UI 脚本当前没有新的编译阻断）
- 当前 warnings：`validate_script` 返回的字符串拼接 GC warning，仅警告
- 当前恢复点：
  - 下一步优先交给用户做运行态观感验收：任务卡翻页、工作台左列图标与滚动、右侧材料区、制作中的按钮/进度叠层、离台小进度、工作台上下翻面与固定跟随。


## 2026-03-25 补记：Day1 UI 这刀已从“两份脚本单独过不去闸门”收成“7 文件依赖闭包可编译 checkpoint”
- 当前子工作区主线没有换题，仍然是 Day1 工作台 / UI / 任务体验重收口；这轮也没有切去碰 `Primary.unity`、Prefab 或字体资源。
- 我先按 live `main@55e2bccd` 跑只含两份 UI 的白名单 preflight，结果明确暴露：`SpringDay1PromptOverlay.cs` 与 `SpringDay1WorkbenchCraftingOverlay.cs` 当前并不是孤立改动，它们依赖 working tree 里的 Day1 支撑接口，而 `HEAD` 版 `SpringDay1Director / CraftingStationInteractable / SpringDay1WorldHintBubble / NPCDialogueInteractable` 还没有这些接口。
- 这轮我只继续补了两处 UI 自己的最后收口：
  - `SpringDay1WorkbenchCraftingOverlay.cs`：删除未使用的 `followSmoothTime`，并把工作台浮层的上/下显示方向锁定为“打开那一刻”的判定，不再在打开后随玩家短时绕位反复翻面。
  - `SpringDay1PromptOverlay.cs`：把翻页动画进一步改成右下角撕页语义，翻页时前页围绕右下角抬起，后页持续承接，不再只是左侧中心点的小幅旋转。
- 本轮关键验证结论：
  - 仅白名单两份 UI：`preflight` 失败，根因是白名单漏了 spring-day1 自己的 live 依赖闭包。
  - 白名单扩成 7 文件后再次 `preflight`：`Assembly-CSharp` 代码闸门通过。
  - 当前最小可收口代码面应为：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
- 当前恢复点：
  - 这轮已经不再是“UI 两份脚本还没写到位”，而是“依赖闭包已经核清，可以按 7 文件 checkpoint 白名单收口”；
  - 后续如继续做用户观感终验，应以这 7 文件为 spring-day1 的 Day1 UI 当前可编译骨架，而不是再把两份 UI 当成孤立热修。

## 2026-03-26 ���ǣ�Day1 hygiene �����յ�����ļ�β�ͣ���ǰֻʣ `Primary.unity` mixed blocker
- ��ǰ�ӹ���������û�л��⣬��Ȼ������ spring-day1 �� Day1 UI / ����̨�տڣ�ֻ�����ֲ��ټ���д���ܣ����ǰ� hot-file hygiene ���� own dirty��
- ��������ʽʹ�ã�
  - `skills-governor`
  - `sunset-lock-steward`
  - `sunset-no-red-handoff`
- `sunset-startup-guard` ���Ựδ��ʽ��¶���Ѱ� Sunset `AGENTS.md`��live `cwd / branch / HEAD`�����ļ��� `git status` ���ֹ��ȼ�բ�š�
- ����ֻ�����ж���
  - `Assets/000_Scenes/Primary.unity` ��ǰ���� `unlocked + mixed-in-place`������ȷ�� Day1 own �� `StoryManager / startLanguageDecoded / Anvil_0` ����̨��������ͬʱ�������λ�ơ�Player λ���� NPC debug override��������ּ�������ֻ�������� scene ��д��
- ������ʵ��������ʱβ�ͣ�
  - ɾ�� `003-��һ���` �� 5 �ݾ� prompt / hygiene ί�У�
    - `26.03.24-Day1����̨UI�������������տ�ί��-02.md`
    - `26.03.25-Day1����̨UI�������������տ�ί��-03.md`
    - `26.03.25-Day1����̨UI�������������տ�ί��-04.md`
    - `26.03.25-Day1����̨UI����������������ɨί��-05.md`
    - `26.03.25-Day1����̨UI����������������ɨί��-06.md`
  - ɾ����ʽ����Ŀ¼��`V1.0_UI��ʽ����_2026-03-25/`
  - ���� `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset`���������� Unity �Զ�������������� spring-day1 ���¡�
- ���־����������ʽ�����棺
  - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9100_Axe_0.asset`
  - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9101_Hoe_0.asset`
  - `Assets/Resources/Story/SpringDay1Workbench/Recipe_9102_Pickaxe_0.asset`
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese BitmapSong SDF.asset`
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese Pixel SDF.asset`
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese SoftPixel SDF.asset`
  - `Assets/TextMesh Pro/Resources/Fonts & Materials/DialogueChinese V2 SDF.asset`
  - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
  - `Assets/222_Prefabs/UI/Spring-day1/`
- ��ǰ��С��֤�����
  - `git diff --check -- Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs` ͨ��
  - ��ǰ formal dirty ������������ʽ�ʲ� / ���� / prefab + Primary.unity blocker��
- ��ǰ�ָ��㣺
  - ��һ����������տڣ�Ӧ���ȶ԰����� formal ���� `preflight / sync`
  - `Primary.unity` �Ա��뵥���� hot-file mixed blocker ���������˳�ֻ����һ��
## 2026-03-26 补记：本子工作区已满足进入 `spring-day1V2` 交接的条件
- 当前子工作区主线不再继续 Day1 新功能，也不再继续 hygiene；这轮唯一目标是判断能否进入下一代交接，并在确认后直接写出重型交接包。
- 本轮 live 判断依据：
  - `cwd = D:\Unity\Unity_learning\Sunset`
  - `branch = main`
  - `HEAD = ee3187573b62891a5b0a8d974f43c192c4125a34`
  - `shared-root-branch-occupancy.md` 当前仍为 `neutral-main-ready`
  - 已接受 checkpoint 与纠偏边界已由 `委托-07` 明确钉死
- 当前裁定：
  - `yes`，可以进入 `spring-day1V2` 交接
  - `Primary.unity` 虽仍是 `mixed hot-file blocker`，但它属于必须写进交接的边界，而不是继续阻止交接的未完成施工项
- 本轮已正式生成：
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\00_交接总纲.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\01_线程身份与职责.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\02_主线与支线迁移编年.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\03_关键节点_分叉路_判废记录.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\04_用户习惯_长期偏好_协作禁忌.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\05_当前现场_高权重事项_风险与未竟问题.md`
  - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\V2交接文档\06_证据索引_必读顺序_接手建议.md`
- 当前恢复点：
  - 这个子工作区的 V1 阶段应视为“已进入代际交接完成态”
  - 后续若继续施工，应由 `spring-day1V2` 先读交接包，再重新做 live preflight 与热区判断
## 2026-03-26 补记：本子工作区当前进入 shared root 尾账清扫与白名单收口，不恢复 Day1 施工
- 当前子工作区主线没有改题，但本轮子任务明确不是继续 Day1 业务，而是按 `26.03.26-Day1V2共享根大扫除与白名单收口-11.md` 只清 spring-day1 这条线 own dirty / untracked。
- 本轮在 stable launcher 白名单 `preflight` 下重新钉实：
  - 当前执行现场为 `main@eb6284fa`
  - `Primary.unity`、`GameInputManager.cs`、`StaticObjectOrderAutoCalibrator.cs`、导航 / 放置 / 农田 / `TagManager.asset` 都继续归类为 foreign dirty，本轮不碰
  - 当前 own 尾账主要就是 5 个 `DialogueChinese*.asset`、`spring-day1 / spring-day1V2` 线程文档、父工作区 `memory.md`、以及本子工作区 `memory.md + 委托/样式快照/V2 续工文档`
- 本轮关键判断：
  - `V2交接文档`、`委托-07/08/09/10/11`、`委托-02/03/04/05/06` 与 `V1.0_UI样式快照_2026-03-25` 这批文件，当前应按 spring-day1 自有证据与交接产物保留并纳入白名单，不再沿用旧 hygiene 口径把它们当成可直接删除的临时垃圾。
  - 本轮真正的“扫地”不是替 shared root 做总清扫，而是把 spring-day1 自己留在 shared root 的资产 / memory / prompt 产物明确认领并收口。
- 当前恢复点：
  - 如果白名单 `sync` 成功，本子工作区将回到“文档/资产尾账已入 main、只剩 foreign dirty 留在 shared root”的状态；
  - Day1 主线施工本轮仍保持暂停，不从这里恢复。

## 2026-03-28 补记：Day1 UI prefab 与运行时逻辑拆层关系已做只读复核
- 当前子工作区主线：响应用户要求，只读分析 `Assets/222_Prefabs/UI/Spring-day1/` 里用户手动调出来的正式面，判断如何把这套 UI 模式推广到“所有工作台共用，只换内容”；本轮不进 Unity、不用 MCP、不改代码或 prefab。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset `AGENTS.md` 手工完成等价前置核查，确认：
  - `cwd = D:\Unity\Unity_learning\Sunset`
  - `branch = main`
- 本轮已复核：
  - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
  - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
  - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/UI/SpringDay1UiLayerUtility.cs`
  - `26.03.24-Day1工作台UI与任务体验重收口委托-01.md`
  - `26.03.26-Day1V2首轮启动委托-09.md`
  - `26.03.26-Day1V2第二轮续工委托-10.md`
  - `V1.0_UI样式快照_2026-03-25/00_样式索引.md`
  - `05_当前现场_高权重事项_风险与未竟问题.md`
  - `06_证据索引_必读顺序_接手建议.md`
- 本轮稳定结论：
  1. `SpringDay1PromptOverlay.prefab` 与 `SpringDay1WorkbenchCraftingOverlay.prefab` 现在更像“视觉 formal-face / 手调样式基线”，不是当前运行时直接实例化的模板；它们自己的 prefab GUID 当前未被 scene 或其他业务资源直接引用。
  2. 当前真实行为基线仍在代码：`SpringDay1PromptOverlay.EnsureRuntime()`、`SpringDay1WorkbenchCraftingOverlay.EnsureRuntime()` 都会 `new GameObject + AddComponent + BuildUi()` 自建 UI。
  3. 当前语境里没有独立第三个“日志 prefab”；用户口中的“日志”更接近 `SpringDay1PromptOverlay` 这张任务/日志卡，以及文档与 DebugMenu 里的验证日志证据层。
  4. `SpringDay1PromptOverlay` 是内容驱动的 Day1 任务卡：数据源来自 `SpringDay1Director.BuildPromptCardModel()`，支持阶段标题、subtitle、focus、footer、逐条完成动画、双页日历式翻页和自适应高度。
  5. `SpringDay1WorkbenchCraftingOverlay` 是 spring-day1 专用工作台浮层：左列配方来自 `Resources/Story/SpringDay1Workbench`，交互入口受 `CraftingStationInteractable` 驱动，制作权限与进度又被 `SpringDay1Director.CanPerformWorkbenchCraft()` / `NotifyWorkbenchCraftProgress()` 约束。
  6. 现有实现把“样式”“内容结构”“Day1 业务规则”揉在同一层脚本里；这对 Day1 单线闭环可用，但不适合原样横向复制给所有工作台。
  7. `SpringDay1WorkbenchCraftingOverlay.prefab` 当前和脚本字段并未完全同步：`materialsViewportRect`、`materialsContentRect`、`progressRoot`、`progressBackgroundImage`、`floatingProgressRoot`、`floatingProgressIcon`、`floatingProgressFillImage`、`floatingProgressLabel` 仍为空，说明手调 prefab 正式面与后长出来的运行时功能面已经分叉。
  8. 如果要推广成“所有工作台共用这套 UI 模式”，正确方向应是拆成三层：
     - prefab 样式模板层
     - 内容 / 行为 schema 层
     - 站点特化规则层
    而不是继续走“纯代码 BuildUi”或“只复制 spring-day1 prefab”两条路。
- 当前恢复点：
  - 只读分析与方案判断已完成；
  - 下一步应等待用户决定：是先抽“通用工作台模板 + schema”，还是先把 spring-day1 这套正式面整理成可迁移的第一份模板规范。
## 2026-03-28 补记：共享字体止血里 Day1 owner 的 6 文件已收成局部 checkpoint
- 当前子工作区主线：不是继续扩 Day1 功能，也不是下潜共享字体底座，而是按 `2026-03-28_典狱长_spring-day1_共享字体止血owner接盘_01.md`，只把 6 个 Day1-facing 文件里的 owner 止血收清楚。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
- 当前 6 文件裁决结果：
  - `A 必留`：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorldHintBubble.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 三者都把默认字体链从 `DialogueChinese V2 / BitmapSong / Pixel / SoftPixel` 收束为单一 `DialogueChinese SDF`
    - `Assets/111_Data/UI/Fonts/Dialogue/DialogueFontLibrary_Default.asset` 全部 key 暂时统一指向 `DialogueChinese SDF`
    - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1PromptOverlay.prefab`
    - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
    - 两份 prefab 中 Day1 文本节点全部改为 `DialogueChinese SDF`，并补清了 workbench prefab 内一处漏掉的 `V2` 序列化引用
  - `B 本轮一起留的 Day1 行为改动`：无；为避免 same-file contamination，我没有借这轮吞额外 Day1 行为改动
  - `C 本轮明确拆出的同文件污染`：
    - `SpringDay1PromptOverlay.cs` 里的 prompt 刷新 / 对话结束忽略 / DisplaySignature 相关行为
    - `SpringDay1WorldHintBubble.cs` 里的 `HideIfExists` 与对话结束忽略
    - `SpringDay1WorkbenchCraftingOverlay.cs` 里的拖拽微调、制作进度提示、玩家动画、director 通知、离台小进度标签等行为
    - 这些都已回退到 `HEAD`，不纳入这次共享字体止血 checkpoint
- 本轮最小验证：
  - `git diff --check`：无 whitespace/blocking error
  - `mcp validate_script`：
    - `SpringDay1PromptOverlay.cs` 0 error / 1 warning
    - `SpringDay1WorldHintBubble.cs` 0 error / 1 warning
    - `SpringDay1WorkbenchCraftingOverlay.cs` 0 error / 1 warning
  - 说明：warning 为通用字符串拼接 GC 提示，不是本轮新引入 compile red
  - 额外只读确认：这 6 文件内已无 `DialogueChinese V2 / BitmapSong / Pixel / SoftPixel` 默认引用残留
- 当前恢复点：
  - 这 6 文件已经形成可说明、可验证、可给用户判断的 Day1 owner 局部 checkpoint
  - 但 `spring-day1` 同根 own roots 仍不 clean；下一步若要 sync，这一刀之后只该处理同根 remaining dirty，不该回头扩成共享字体底座治理
## 2026-03-28 共享字体止血 owner 接盘回执补记（只读核查）
- 按 `2026-03-28_典狱长_spring-day1_共享字体止血owner接盘_01.md` 回执格式完成 live 复核。
- 当前 live Git：`main @ a0b3f0eb16e340fd5c2a3f20d4ac6644832690d1`。
- Day1 owner 6 文件仍保持同一结论：只保留 Day1-facing 默认字体链止血，统一收束到 `DialogueChinese SDF`，不下潜共享字体底座，不碰 `Primary.unity`。
- 通过 `git-safe-sync.ps1 -Action preflight -Mode task -OwnerThread spring-day1 -IncludePaths <6文件>` 再次确认：6 文件局部 checkpoint 成立，但 `Assets/YYY_Scripts/Story/UI` 同根仍有 remaining dirty/untracked（`SpringDay1StatusOverlay.cs`、`SpringDay1UiLayerUtility.cs`、`NpcWorldHintBubble.cs` 等），因此当前仍不能 sync。
- 本轮面向用户的判断保持：这是一个“可说明、可验证的 Day1 字体止血 checkpoint”，不是“共享字体系统已彻底稳定”。
- 下一步最小动作仍然只有两种：要么用户先按验收清单判断这刀视觉止血是否接受；要么后续单独清 `Assets/YYY_Scripts/Story/UI` 同根尾账，再谈 sync。

## 2026-03-28 Day1 owner 字体止血 checkpoint 停刀
- 用户已明确收下本轮 `Day1 owner 字体止血 checkpoint`，并裁定：这条线先停，不再继续由 `spring-day1` 往下施工。
- 后续接棒线程改为 `spring-day1V2`；本线程当前职责到此暂停。
- 当前冻结点保持不变：6 个 Day1-facing 文件的局部字体止血 checkpoint 已成形，但 `Assets/YYY_Scripts/Story/UI` 同根 remaining dirty/untracked 仍未 clean，因此本线程没有继续 sync，也不再自行开下一刀。
- 后续如需续工，应由 `spring-day1V2` 继承当前 checkpoint、blocker 与验收口径继续处理；本线程只保留现状记忆，不再扩写。

## 2026-04-03 补记：工作台 overlay 三问题只读侦察结论
- 当前子工作区主线：不是继续 UI 大改，也不是继续 `Primary` / 字体 / scene；这轮只读排查 `SpringDay1WorkbenchCraftingOverlay` 的 3 个现象：
  1. 制作时是否真的允许离开工作台
  2. 离台悬浮进度为什么不显示
  3. 玩家绕工作台移动时上下切换为什么可能不触发
- 本轮显式使用：
  - `skills-governor`
- 本轮只读查实：
  1. 代码层面“制作时允许离开”是成立的：
     - `LateUpdate()` 在超出 `_autoHideDistance` 后只会 `Hide()` 面板；
     - `Hide()` 不会停掉 `_craftRoutine`，而是仅把 `_isVisible=false`、解除 `blockNavOverUI`、再走 `HideImmediate()`；
     - `CraftRoutine()` 继续跑，`MaintainWorkbenchPose()` 也只会在仍贴近工作台时帮玩家转向，不会锁住位置。
  2. 离台悬浮进度当前真正的硬伤不在显示条件，而在可见性层级：
     - `floatingProgressRoot` 是同一 overlay 根对象的子节点；
     - `HideImmediate()` 直接把根 `CanvasGroup.alpha` 置 `0`；
     - `UpdateFloatingProgressVisibility()` 虽然会在 `_isVisible=false && _craftRoutine!=null` 时激活 `floatingProgressRoot`，但它仍继承父级 `CanvasGroup` 的透明度，所以逻辑上“该显示”时也会肉眼不可见。
  3. 上下切换的脆点主要在方向判定采样，而不是 `Reposition()` 本身：
     - overlay 每帧用 `ShouldDisplayBelow(SpringDay1UiLayerUtility.GetInteractionSamplePoint(_playerTransform))` 决定上下；
     - `CraftingStationInteractable.ShouldDisplayOverlayBelow()` 又先拿“玩家采样点 y”去和“工作台 visual bounds.center.y”做比较，只给 `0.04f` 的窄死区；
     - `GetInteractionSamplePoint()` 默认取玩家 collider / presentation bounds 的 `min.y + 0.02f`，也就是更接近脚底而不是身体中心；
     - 对宽 collider、偏移 sprite、绕侧边走位或脚底采样变化很小的情况，这套比较会长期卡在同一侧；再叠加 `_autoHideDistance=1.5f` 的提前收面板，就会表现成“怎么绕都不切”。
- 当前恢复点：
  - 如果后续真开修复切片，最小优先级应是：
    1. 先拆开 `HideImmediate()` 与 `UpdateFloatingProgressVisibility()` 的可见性控制，不要再让 floating 继承整张面板的 `CanvasGroup.alpha=0`
    2. 再收紧 `ShouldDisplayOverlayBelow()` 的判定基准，优先改成基于最近交互点 / 更稳定的人物采样，而不是单纯脚底 y 对 visual center
    3. 若仍有“绕一圈就消失”的体验问题，再看 `_autoHideDistance` 与 `LateUpdate()` 的 hide 时机
## 2026-04-03 补记：正规对话已恢复可见，工作台离台浮层已重新跑通

- 当前子工作区主线：
  - 用户这轮把 `spring-day1` 收窄成 4 个玩家可见问题：
    1. `NPC` 打断短气泡回旧正式样式
    2. 正规 `DialogueUI` 修复“有框没字 / 透明”
    3. 工作台上下切换、制作中可离开、离台悬浮进度重新跑通
    4. 工作台配方统一到 5 秒，并确认木剑 / 木箱已纳入
- 本轮显式使用：
  - `skills-governor`
  - `preference-preflight-gate`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
- 本轮实际落地：
  1. `DialogueUI.cs`
     - 把正规对话显现顺序改成“先让正规对话自己可见，再 fade 掉其他 UI”，不再把对话整块压在 `alpha=0` 等别的 UI 退场；
     - `EnsureDialogueVisualComponentsReady()` 会把正文 `TextMeshProUGUI` 强制拉回 enabled，补掉 `Primary.unity` 里旧 disabled 值导致的“有框没字”。
  2. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 把“大面板显示”和“离台 floating 显示”拆开，`UpdateFloatingProgressVisibility()` 现在会单独决定 root `CanvasGroup` 和 `PanelRoot` 的可见性，不再让 floating 跟着大面板一起透明；
     - 离台判定改成两层兜底：既看采样点离交互/可视包络的距离，也看玩家根位置离工作台中心的距离，避免人物已经明显离开但面板不收；
     - 维持制作时不锁玩家移动，只在仍贴近工作台时轻维持朝向；
     - 已确认 `Recipe_9100~9104` 当前全部 `craftingTime = 5`，包含 `Sword_0` 与 `Storage_1400`。
  3. `NPCBubblePresenter.cs`
     - 继续保留 conversation / reaction channel 行为接口；
     - 只把 `ReactionCue` 的紫色、无尾巴、紧缩版 special-case 全部撤掉，回到和普通 NPC 正式气泡同一张脸。
- 本轮 live / 编译证据：
  - `Assets/Refresh` 重新编译通过，最新 `Editor.log` 尾部为 `*** Tundra build success` + `Mono: successfully reloaded assembly`；
  - 当前唯一新增 warning 仍是 `DialogueUI.fadeInDuration` 未使用；无 owned compile error；
  - 正规对话 live：
    - `Sunset/Story/Debug/Play Spring Day1 Dialogue`
    - 连续两次 `Sunset/Story/Debug/Log Dialogue State`
    - 最新两次都已稳定到：
      - `DialogueFont='DialogueChinese Pixel SDF'`
      - `CanvasAlpha=1.00`
      - `CanvasInteractable=True`
      - `CanvasBlocksRaycasts=True`
    - 说明“正规对话整块透明”这一条已从玩家面恢复。
  - 工作台 live：
    - fresh Play 重新跑 `Sunset/Story/Debug/Run Spring Day1 Workbench Craft Exit Probe`
    - 最新结果已变为：
      - `belowSouth=False`
      - `belowNorth=True`
      - `switchOk=True`
      - `floatingVisible=True`
      - `floatingLabel='1'`
      - `floatingFill=0.02`
    - 新抓图：`.codex/artifacts/ui-captures/spring-ui/pending/20260403-144826-380_workbench-craft-exit-probe.png`
    - 这说明工作台制作中离开后，大面板已收，离台浮层已真正接棒。
- 当前仍未闭环：
  - `NPC` 气泡这条我只拿到了静态结构回正证据，还没补一张新的 live/GameView 终验图；
  - 所以这轮能诚实 claim：
    - 正规对话：`线程自测已过`
    - 工作台离台浮层：`线程自测已过`
    - NPC 打断短气泡：`结构成立，live 待终验`
- 当前恢复点：
  - 若下一轮继续，只剩一个最小动作：补 `NPC` 普通气泡 + 打断短气泡的 live 观感确认；
  - 不需要再回头重修正规对话或工作台离台浮层。
## 2026-04-03 补记：按用户最新裁定拆成 UI 线程与 spring-day1 线程并行

- 当前子工作区主线：
  - 用户最新改判为“两线程并行”：
    1. `UI / SpringUI` 线程正式接走 spring-day1 当前全部玩家面 `UI/UE` 问题；
    2. `spring-day1` 自己只保留逻辑完善、剧情/行为顺序把控，以及 `NPCBubblePresenter.cs` 的旧正式气泡回正与 live 终验。
- 本轮子任务：
  - 不再继续修业务；
  - 只把上面这次分工裁定收成两份可直接转发的续工 prompt，并把当前 slice 合法停到 `PARKED`。
- 本轮已落地：
  1. 新建 `2026-04-03_UI线程_接管spring-day1全部玩家面问题并行prompt_01.md`
     - 明确把正规对话 UI、Prompt/Hint/WorldHint、Workbench 玩家面、overlay/prefab-first 壳体、字体引用与 GameView 体验收口整体转交给 `UI / SpringUI`；
     - 同时明确排除 `NPCBubblePresenter.cs`、`Primary.unity`、`GameInputManager.cs`、NPC/Story 状态机本体。
  2. 新建 `2026-04-03_spring-day1_逻辑剧情控制与NPC旧气泡续工prompt_02.md`
     - 明确 spring-day1 后续只做非 UI 逻辑/剧情控制收口与 `NPC` 旧正式气泡 live 终验；
     - 同时明确冻结 `town` 未就位前的剧情扩写，不回漂到玩家面 UI。
  3. 当前 slice 已跑 `Park-Slice`
     - 状态已从 `ACTIVE` 合法切到 `PARKED`
     - 当前切片名：`并行分工prompt分发`
- 当前稳定判断：
  - 从这条子工作区往后看，玩家面问题不再由 spring-day1 继续主刀；
  - 这里保留的唯一表现层例外只剩 `NPCBubblePresenter.cs` 这条旧正式气泡。
- 涉及路径：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-03_UI线程_接管spring-day1全部玩家面问题并行prompt_01.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-03_spring-day1_逻辑剧情控制与NPC旧气泡续工prompt_02.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\state\active-threads\spring-day1.json`
- 当前恢复点：
  - 等用户转发这两份 prompt；
  - 若 spring-day1 后续继续，只按 `prompt_02` 收窄后的范围回来，不再把玩家面 UI 混回本线程。
## 2026-04-03 补记：Day1 非 UI 逻辑/剧情控制 5 文件只读裁定

- 当前子工作区主线：
  - 用户把本轮固定为 `spring-day1` 的非 UI 逻辑 / 剧情控制收口侦察；
  - 明确不碰 `Primary.unity`、`GameInputManager.cs`、玩家面 UI；
  - 只读分析以下 5 个文件：
    - `Assets/YYY_Scripts/Service/Player/PlayerNpcChatSessionService.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
    - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
  - `sunset-doc-encoding-auditor`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset `AGENTS.md` 手工完成等价启动核查，并在首条 `commentary` 明说本轮是只读分析、不跑 `Begin-Slice`。
- 本轮稳定裁定：
  1. 仍明确属于 `spring-day1` own 的真实残项，不在玩家面 `UI` 线程：
     - `CraftingStationInteractable.OnInteract()` 仍保留 live 路径里的验证兜底：
       `OnInteract -> SpringDay1Director.TryHandleWorkbenchTestInteraction()`；
       一旦 overlay / panel 没有真正接住，导演层会直接给出“测试用工作台交互：已记作一次基础制作”，这是逻辑推进口，不是 UI 抛光。
     - `PlayerNpcChatSessionService` 仍完整承载闲聊会话状态机：
       `StartConversation()`、`HandleSessionBreakDistance()`、`StartWalkAwayInterrupt()`、`CapturePendingResumeSnapshot()`、`ResolveResumeIntroPlan()` 这条“走开打断 / 重新接话 / 系统接管后恢复”链，仍属于 Day1 逻辑 own。
     - `SpringDay1Director` 仍是主剧情顺序与阶段闸门本体：
       `HandleDialogueSequenceCompleted()`、`TickPrimarySceneFlow()`、`TickFarmingTutorial()`、`BeginReturnReminder()`、`EnterFreeTime()`、`TryTriggerSleepFromBed()`，以及 `TryAutoBindWorkbenchInteractable()` / `TryAutoBindBedInteractable()` 的 temp-scene 运行时接线，都仍是 Day1 自己要守的底座。
     - `NPCDialogueInteractable.ResolveDialogueSequence()` 里“首段 / followup / 解码后续 / phase 已推进后续”的正式对话切换，也仍属于剧情控制 own，不该再丢给 UI。
  2. 已经属于 `UI own` 或明显跨到 foreign 的点，这轮不该再碰：
     - `PlayerNpcChatSessionService` 里的 `GetPromptCaption()`、`GetPromptDetail()`、`UpdateConversationBubbleLayout()`、`SyncConversationPromptOverlay()`，已经是提示文案、气泡摆位和 `InteractionHintOverlay` 的玩家面结果层。
     - `NPCDialogueInteractable.ReportProximityInteraction()`、`NPCInformalChatInteractable.ReportProximityInteraction()`、`CraftingStationInteractable.ReportWorkbenchProximityInteraction()`、`CraftingStationInteractable.ShouldDisplayOverlayBelow()`，都直接服务 `WorldHint / Prompt / Workbench overlay` 的显示与体感，已归 UI 线。
     - `SpringDay1Director.BuildPromptCardModel()`、`GetCurrentTaskLabel()`、`GetCurrentProgressLabel()`、`GetCurrentWorldHintSummary()`、`BuildPlayerFacingStatusSummary()`，以及本文件内所有 `SpringDay1PromptOverlay.Instance.Show(...)` 的玩家面提示文本，当前都应视为 UI 线程 own 的输出面，不再由 `spring-day1` 继续主刀润色。
  3. 如果只允许最小修一刀，最值得切的是：
     - 收掉 `CraftingStationInteractable.OnInteract()` 到 `SpringDay1Director.TryHandleWorkbenchTestInteraction()` 这条 live 验证 fallback，让“工作台没真正打开 UI / 没真正完成制作”时不再悄悄把 Day1 逻辑推进掉。
- 为什么这刀优先：
  - 它是明确的非 UI 逻辑问题；
  - 只触碰当前允许分析的 2 个文件；
  - 不会碰 `Primary.unity`、`GameInputManager.cs`、玩家面 UI；
  - 并且能防止“UI 没接住时，逻辑被假推进”这种 owner 污染，避免后续误判成 Day1 已过线。
- 当前恢复点：
  - 本轮没有进入真实施工；
  - 若下一轮继续，最小切片应只围绕“工作台 live fallback 不再伪造 craft 完成”收刀；
  - `闲聊气泡/提示/overlay/prompt wording` 继续留给 UI 线或已有例外线，不再混回本子工作区。

## 2026-04-03 补记：`NPCBubblePresenter` 旧正式气泡 live 终验只读侦察结论

- 当前子工作区主线：
  - 用户已把 `spring-day1` 收窄到 `NPCBubblePresenter` 的旧正式气泡 live 终验；
  - 明确不碰玩家面 UI、`Primary.unity`、`GameInputManager.cs`；
  - 本轮只读分析 `NPCBubblePresenter` 本体，以及工程内任何能触发或验证 NPC 气泡 / 打断短气泡的 Editor 菜单、测试、debug 入口。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
  - `sunset-doc-encoding-auditor`
- `sunset-startup-guard` 当前会话未显式暴露；已按 Sunset `AGENTS.md` 做手工等价启动核查，并在首条 `commentary` 明说本轮只读分析、不跑 `Begin-Slice`。
- 本轮查实：
  1. `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 的旧正式样式结构已基本回正：
     - `CurrentStyleVersion = 13`
     - `ApplyCurrentStylePreset()` 仍指向旧正式样式：金边暗底、大 padding、`fontSize = 32`、保留尾巴与 bob
     - `UpgradeLegacyStyleIfNeeded()` 会把旧序列化资源在运行时补升到当前 preset
     - `ReactionCue` 已不再保留独立紫色 / 无尾巴 / 紧缩 special-case，只剩 `displayMode / channelPriority` 语义分流
  2. 真正会打到“打断短气泡”的链路仍是：
     - `PlayerNpcChatSessionService.RunWalkAwayInterrupt()`
     - `ShowInterruptReactionCue()`
     - `BubblePresenter.ShowReactionCueImmediate(...)`
     - 然后才可能进入 `ShowInterruptNpcLine(...)`
     这说明打断短气泡并不是另一套 presenter，而是同一 presenter 的 reaction 通道
  3. 现成最小 live 抓手已经存在：
     - 普通旧正式气泡：
       - `NPCBubblePresenter` 组件 ContextMenu：`调试/显示随机自言自语`
       - `NPCAutoRoamController` 组件 ContextMenu：`调试/立即进入长停`、`调试/尝试附近聊天`
       - `NPCBubbleStressTalker` 可做持续循环压测
     - 打断短气泡：
       - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
       - 菜单项：
         - `Tools/Sunset/NPC/Validation/Trigger 002 Informal Chat`
         - `Tools/Sunset/NPC/Validation/Trigger 003 Informal Chat`
         - `Tools/Sunset/NPC/Validation/Trace 002 Informal Chat Interrupt`
         - `Tools/Sunset/NPC/Validation/Trace 003 Informal Chat Interrupt`
         - `Tools/Sunset/NPC/Validation/Trace 002 PlayerTyping Interrupt`
         - `Tools/Sunset/NPC/Validation/Trace 003 PlayerTyping Interrupt`
       - 其中 `Trace 002/003 ... Interrupt` 已经是最贴近当前 scene 真实 interrupt 链的现成入口
     - 更完整但仍隔离的 runtime probe：
       - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
       - `Tools/NPC/Spring Day1/Run Runtime Targeted Probe`
       - 会汇总 `instance / informal / pair / walkAway`，pair timeout 时还会额外打印 `BubblePresenter` 状态
  4. 当前自动化测试更多是在守结构，不足以替代 live 终验：
     - `NPCInformalChatInterruptMatrixTests` 主要验 interrupt / reaction 配置解析
     - `PlayerNpcConversationBubbleLayoutTests` 主要验对话气泡布局与排序
     - `PlayerThoughtBubblePresenterStyleTests` 主要验玩家 / NPC 样式差异与换行节奏
     - 都不能直接证明“旧正式气泡的 live 观感已经过线”
  5. 如果还要补最小 probe，最合适的 own 范围文件是：
     - `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs`
     - 原因：它已经直接命中 `002/003` 当前 scene 的 interrupt 链，最适合只补“cue 命中瞬间的 `BubblePresenter` 状态日志 / GameView 取证钩子”，不用扩写业务逻辑、不用碰 `Primary.unity`
- 当前稳定判断：
  - 这条线现在不是“结构还乱”，而是“结构已基本回正，但还缺一手 live/GameView 终验图或瞬时状态证据”；
  - 所以下一步若继续，不该再回去大修 `NPCBubblePresenter` 本体，而应直接用现有菜单入口补终验证据。
- 当前恢复点：
  - 本轮没有进入真实施工，仍停留在只读分析；
  - 若下一轮继续，最小动作应是：
    1. 先用 `NPCInformalChatValidationMenu` 跑一次 `Trace 002/003 ... Interrupt`
    2. 若证据仍不够，再只在 `Assets/Editor/NPC/NPCInformalChatValidationMenu.cs` 补最小 probe
    3. 不回碰玩家面 UI、`Primary.unity`、`GameInputManager.cs`

## 2026-04-03 补记：`spring-day1` 非 UI 逻辑收口 + NPC 旧正式气泡 live 终验完成

- 当前子工作区主线：
  - 按 `2026-04-03_spring-day1_逻辑剧情控制与NPC旧气泡续工prompt_02.md` 收窄后，只做：
    - 非 UI 的逻辑 / 剧情控制收口
    - `NPCBubblePresenter` 旧正式气泡 live 终验
  - 明确不回碰玩家面 UI、`Primary.unity`、`GameInputManager.cs`
- 本轮施工与取证：
  1. 已按 thread-state 重新进入真实施工切片，保持 own 聚焦在：
     - `NPCBubblePresenter.cs`
     - `PlayerNpcChatSessionService.cs`
     - `NPCDialogueInteractable.cs`
     - `NPCInformalChatInteractable.cs`
     - `CraftingStationInteractable.cs`
     - `SpringDay1Director.cs`
  2. 用命令桥进入 Play，执行 `Sunset/Story/Debug/Bootstrap Spring Day1 Validation`，拿到 live 快照：
     - 当前 scene 仍是 `Primary`
     - Day1 仍从 `CrashAndMeet` 起跑
     - `Workbench=Anvil_0`
     - `sleepReady=False`
  3. 用 `Tools/Sunset/NPC/Validation/Trigger 002 Informal Chat` + `Capture Spring UI Evidence` 拿到普通会话 live 图：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260403-184219-745_manual.png`
     - 侧车 `json` 显示 WorldHint 已进入 `002|E|对方在回你|会自动继续，按 E 只跳过这句动效`
     - GameView 图像里 NPC 主气泡与同屏短句都已经回到暗底金边、带尾巴的旧正式脸
  4. 用 `Tools/Sunset/NPC/Validation/Trace 002 PlayerTyping Interrupt` + 单次 capture 拿到打断短气泡 live 图：
     - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending\20260403-184935-423_manual.png`
     - 侧车 `json` 显示 WorldHint 已进入 `002|E|聊到一半|正在收束，按 E 跳过收尾`
     - Editor.log 最终出现：
       `002 跑开中断完成 | playerExitSeen=True | npcReactionSeen=True | endReason=WalkAwayInterrupt`
     - 这说明打断短气泡不只结构回正，连 `WalkAwayInterrupt` 的运行态收尾也已真实闭环
  5. 首次为了硬抓短窗口，连续触发了多次 `Capture Spring UI Evidence`，引发一次
     `002 闭环 trace 超时：跑开中断未在 5.5 秒内收尾`
     的伪故障；复盘后确认这是取证脚本把 Editor update 挤爆导致的采样噪声，不是业务状态机真实红。改成单次抓图后，interrupt trace 正常完成。
  6. 逻辑侧最小补丁已落：
     - `SpringDay1Director.TryHandleWorkbenchTestInteraction()` 不再把 `_craftedCount` 直接记满
     - 当前改为只返回阻断提示：
       `工作台界面当前未接通，本次不会记作基础制作。等工作台真正打开后再完成这一步。`
     - 目的：关掉 `CraftingStationInteractable -> Director fallback` 这条“UI 没接住时伪推进 Day1 教学链”的逻辑后门
- 当前稳定判断：
  1. `NPC` 旧正式气泡 live 终验已经成立：
     - 普通气泡成立
     - 打断短气泡成立
     - 不需要再回去重打 `NPCBubblePresenter` 本体
  2. 当前 temp scene 里 `spring-day1` 仍需要守住的，是：
     - Day1 phase 顺序
     - workbench / free-time / sleep gating
     - informal chat interrupt / resume 行为顺序
     - temp-scene 的运行时补挂前提
  3. `town` 还没就位前，剧情扩写继续冻结；这轮不再往前置剧情漂
- 验证与风险：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 已过
  - Unity 在本轮补丁后再次成功打到 `Bootstrap` 快照，说明本轮 own 改动没有把 Day1 runtime 打红
  - Editor.log 仍可见 foreign/shared 历史红账：
    - `Assets/Editor/Tool_005_BatchStoneState.cs`
    - `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`
    - `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs`
    这些不是本轮新增，也不属于这轮主刀
- 当前恢复点：
  - 这轮 own 目标已经完成；
  - 若下一轮继续 `spring-day1`，只该继续非 UI 的逻辑 / 剧情边界收口，不回吞玩家面 UI。

## 2026-04-03 补记：workbench fallback 回归测试已在当前打开的 Unity 内真实跑通

- 当前子工作区主线：
  - 继续沿 `2026-04-03_spring-day1_逻辑剧情控制与NPC旧气泡续工prompt_02.md` 收口；
  - 不回碰玩家面 UI、`Primary.unity`、`GameInputManager.cs`；
  - 这一步只补 `SpringDay1Director.TryHandleWorkbenchTestInteraction()` 那个逻辑补丁的真实回归验证。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-no-red-handoff`
  - `sunset-unity-validation-loop`
  - `preference-preflight-gate` 手工等价流程
- 新增最小验证入口：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringDay1TargetedEditModeTestMenu.cs`
  - 菜单：`Sunset/Story/Validation/Run Workbench Fallback Guard Test`
  - 作用：在当前已打开的 Unity 里直接跑
    `SpringDay1LateDayRuntimeTests.Director_WorkbenchFallback_ShouldNotMarkCraftObjectiveComplete`
    并把结果写到
    `Library/CodexEditorCommands/spring-day1-workbench-fallback-test.json`
- 为什么这样做：
  - 当前工程已有打开中的 Unity 实例；
  - 直接走 `Unity.exe -runTests` 会撞项目锁，历史 `ui-tests-20260403.log` 已明确记录这条路只会得到“另一个 Unity 正在打开此项目”；
  - 因此本轮改为在当前打开的 Unity 内，用现有命令桥可调用的菜单方式拿最小可信证据。
- 本轮实际验证闭环：
  1. 先通过 `MENU=Assets/Refresh` 让当前打开的 Unity 编译新菜单脚本；
  2. `Editor.log` 最新尾部出现 `*** Tundra build success` 与 domain reload，说明这份 Editor 菜单本身没有把现场编红；
  3. 再通过命令桥执行：
     `MENU=Sunset/Story/Validation/Run Workbench Fallback Guard Test`
  4. 最终结果文件：
     `Library/CodexEditorCommands/spring-day1-workbench-fallback-test.json`
     已落为：
     - `status=completed`
     - `success=true`
     - `passCount=1`
     - `failCount=0`
     - `skipCount=0`
- 当前稳定结论：
  - `Director_WorkbenchFallback_ShouldNotMarkCraftObjectiveComplete` 已在当前打开的 Unity 里真实通过；
  - 所以这条逻辑补丁现在从“代码上看是对的”升级为“targeted probe 已成立”；
  - 但这仍然只是逻辑层 / targeted probe，不代表玩家面对工作台 UI 体验已经由本线程负责并已终验。
- 当前恢复点：
  - 这轮 `spring-day1` 自己能补的最关键逻辑证据已补齐；
  - 若后续继续，只应再做非 UI 的 phase / interrupt / gating 边界，不回吞玩家面 UI。

## 2026-04-04 补记：原剧本回正、Town 承接与非 UI 剧情框架已正式落成设计稿

- 当前子工作区主线已切到新的设计任务：
  - 不继续修 UI，不回头重打已跑通的玩家面链；
  - 只做 `spring-day1` 自己的原剧本回读、剧情扩充设计、Town 承接边界与后续非 UI 框架任务收束。
- 本轮已完成：
  1. 回读原案与当前实现，确认当前 Day1 的真问题不是“完全不能跑”，而是：
     - 缺 `矿洞口危险 -> 跟随撤离 -> 进村围观 -> 闲置小屋安置` 这条前半段；
     - 当前 crowd 里 `101~301` 不能直接当成原案正式角色真名；
     - 真正该守住的正式 Day1 主承载仍是 `马库斯 / 艾拉 / 卡尔`。
  2. 新增两份文档：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_原剧本回正与Town承接剧情扩充设计_01.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_非UI剧情扩充框架落地任务单_01.md`
  3. 把后续稳定口径钉死为：
     - 保留当前 9 个 `StoryPhase` 大骨架；
     - 优先在 phase 内补更细的剧情步；
     - 先把原案前半段与 Day1 夜间收束补回；
     - 其他 NPC 的主出现面最终收回 `Town`，temp scene 只留必要过场。
- 本轮关键决策：
  1. 不继续把 `101~301` 当作原案正式具名角色写剧情；
  2. 不在这轮硬塞“还没人消费的空代码骨架”，避免制造死架子；
  3. 先用文档把原案硬约束、现状缺口、NPC 走位和后续施工顺序彻底固定，再决定下一刀真实代码切片。
- 验证状态：
  - `静态设计结论成立`
  - 本轮未改业务代码、未跑 Unity live、未碰 `Primary.unity / GameInputManager.cs`
- thread-state：
  - 本轮沿用既有 `ACTIVE` slice 完成本刀；
  - 已执行 `Park-Slice`
  - 当前状态：`PARKED`
- 当前恢复点：
  - 后续若继续真实施工，优先顺序应是：
    1. `CrashAndMeet / EnterVillage` 内部剧情步扩充
    2. `HealingAndHP / WorkbenchFlashback` 正式剧情资产化
    3. `DinnerConflict / ReturnAndReminder / FreeTime` 的村庄包装层

## 2026-04-04 补记：Prompt/任务列表链只读核查已把 8 条里任务相关未完项收束到 4 个代码口

- 当前子工作区主线：
  - 本轮不是继续做 UI 施工，而是只读核查 `spring-day1` 当前 Prompt/任务列表链里，和用户那 8 条剩余需求最相关的未完项；
  - 只回答 4 件事：`首屏空白/缺字原因`、`是否仍一次只显示一个主任务`、`木头已有 >=3 时任务判定是否闭环`、`最小该补的代码点`。
- 本轮只读检查了：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1PromptOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DialogueProgressionTests.cs`
- 本轮稳定结论：
  1. `Director` 数据侧不是“没给字”：
     - `BuildFarmingTutorialPromptItems()` 仍明确构建 `5` 条非空教学任务；
     - 对应测试 `Director_FarmingTutorialPromptCard_ShouldExposeFiveFilledObjectives()` 也把“5 条任务、每条标题/说明非空”写死了。
  2. 但玩家真正看到的 `PromptOverlay` 仍被故意裁成“只显示当前主任务 1 条”：
     - `BuildCurrentViewState()` 传的是 `maxVisibleItems: 1`；
     - `PromptCardViewState.FromModel()` 还会先找首个未完成项，再把显示条数硬裁到 `1`；
     - 对应测试 `PromptOverlay_FarmingTutorial_ShouldOnlyRenderCurrentPrimaryTask()` 也明确断言前台只亮 `1` 条 row。
  3. 所以“首屏看起来空/缺字”的当前主嫌，不是 `Director` 没给任务，而是 `view` 层现在只打算显示一条，再叠加 runtime 壳复用/绑定链只重点保证前台第一条 row：
     - `CanReuseRuntimeInstance()` / `CanBindPageRoot()` 现在只要求页面上至少存在一条可绑定 row 链；
     - `PromptOverlay_RuntimeCanvas_ShouldBeScreenOverlayAndRenderFilledTaskTexts()` 也只强校验前台第一条 row 的 `label/detail` 非空，没有覆盖“首屏应显示多少条任务”或“真实首屏 live 是否仍会丢条目”。
  4. `木头已有 >=3` 时，导演层会直接把木材目标判完成并把焦点推进到“回工作台做一次真正制作”，这一拍有测试守住；
     - 但整条“真实制作 -> 教学完成”链还不算彻底闭环：
       - `SpringDay1WorkbenchCraftingOverlay.CanCraftSelected()` 只看 `recipe 解锁 + 材料够不够`，没有接 `SpringDay1Director.CanPerformWorkbenchCraft()`；
       - `SpringDay1Director.HandleCraftSuccess()` 目前对任意制作成功都直接 `_craftedCount++`，没有再核 `Day1 phase / 教学时机 / recipe 范围`。
  5. 因此当前最小该补的代码口，优先级应是：
     - `SpringDay1PromptOverlay.BuildCurrentViewState()` / `PromptCardViewState.FromModel()`：如果产品要求不再是一屏只给 1 条，就这里改；
     - `SpringDay1LateDayRuntimeTests`：把测试从“只验第一条 row 非空”补到“验首屏应显示的条数和文本”；
     - `SpringDay1WorkbenchCraftingOverlay.CanCraftSelected()`：接入 `SpringDay1Director.CanPerformWorkbenchCraft(out blockerMessage)`；
     - `SpringDay1Director.HandleCraftSuccess()`：只在 Day1 教学允许的真实制作成功里记 `_craftedCount`。
- 验证状态：
  - `静态推断成立`
  - 本轮未改代码、未跑 Unity live、未执行测试。
- thread-state：
  - 本轮保持只读分析，未跑 `Begin-Slice / Ready-To-Sync / Park-Slice`。
- 当前恢复点：
  - 如果下一轮改这条链，最值钱的顺序应是：
    1. 先决定 `Prompt` 首屏到底要 `1` 条还是 `多条`
    2. 再把对应测试一起改实
    3. 然后补 workbench 真制作链对 `Day1` 教学 gating 的正式接线

## 2026-04-04 补记：Workbench/工作台 8 条残项只读核查已收束到“壳复用、detail 上报、队列语义、翻面采样、导演层未接 live 进度”

- 当前子工作区主线：
  - 用户继续停留在 `spring-day1` 运行时链路排查；
  - 本轮子任务是只读回答 Workbench/工作台 相关未完成项，不改文件，只给源码和测试层的当前状态。
- 本轮只读检查了：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\CraftingStationInteractable.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorldHintBubble.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\InteractionHintOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiLayerUtility.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
- 本轮稳定结论：
  1. 左列 recipe 为空的最可疑根因不在 `CraftingService`，而在 `SpringDay1WorkbenchCraftingOverlay` 自己：
     - `EnsureRecipesLoaded()` 直接 `Resources.LoadAll<RecipeData>("Story/SpringDay1Workbench")`，并过滤 `requiredStation == Workbench && resultItemID >= 0`；这里没命中时，左列就没有数据源。
     - `CanReuseRuntimeInstance()` / `HasReusableRecipeRowChain()` 对“没有可用 row 但壳仍算可复用”的判定偏宽。
     - `BindRecipeRow()` 只要 `Accent/Icon/Name/Summary/Button` 缺一个就整行返回 `null`。
     - `RefreshAll()` 虽会尝试 `ShouldRebuildRecipeRowsFromScratch()`，但当前只抓“可见行无字/无摘要”这类坏壳，抓不到更隐性的布局/裁剪问题。
  2. workbench 提示 detail 链代码上基本已补齐：
     - `CraftingStationInteractable.BuildWorkbenchReadyDetail()`
     - `CraftingStationInteractable.ReportWorkbenchProximityInteraction()`
     - `SpringDay1ProximityInteractionService.ReportCandidate()`
     - `BuildWorldHintDetail()` / `ResolveOverlayPromptContent()`
     - `SpringDay1WorldHintBubble.Show()` / `InteractionHintOverlay.ShowPrompt()`
     - `SpringDay1InteractionPromptRuntimeTests.CraftingStationInteractable_BuildWorkbenchReadyDetail_UsesQueueCopyWhenOverlayHasActiveCraftQueue()` 也已守住“查看当前制作进度 / 继续追加”
     - 但 `ReportWorkbenchProximityInteraction()` 在 `overlay.IsVisible` 时会提前 `return`，所以 `BuildWorkbenchReadyDetail()` 里“按 E 关闭工作台”这条 proximity detail 分支，正常上报链下基本打不到。
  3. 队列数量 / 当前单件进度 / 追加制作的 overlay 本地语义已成立：
     - `OnCraftButtonClicked()`、`CraftRoutine()`、`UpdateProgressLabel()`、`BuildCraftButtonLabel()`、`GetRemainingCraftCount()`、`GetSelectableQuantityCap()` 共同定义了：
       - `remaining = total - completed`
       - “剩余”包含当前正在制作的这一件
       - “追加制作”只允许对当前同一配方加 `queueTotal`
     - 但 `SpringDay1Director.NotifyWorkbenchCraftProgress()` 只有定义，没有任何调用点，所以导演层 `BuildWorkbenchCraftProgressText()` / `GetCurrentProgressLabel()` 当前接不到 live 队列状态。
  4. `E toggle / 打开大 UI / 关闭后悬浮小框 / 静止锚定 / 翻面弹性` 这些动作，代码并非空白：
     - `CraftingStationInteractable.OnInteract()` -> `SpringDay1WorkbenchCraftingOverlay.Toggle()` 已支持同锚点再次按 `E` 关闭大 UI
     - `UpdateFloatingProgressVisibility()` 已支持“离台后仅保留小进度框”
     - `Reposition()` / `RepositionFloatingProgress()` 已有屏幕边缘 clamp、pixel snap 和锚点跟随
     - `ApplyDisplayDirection()` + `ShouldDisplayOverlayBelow()` 已有 hysteresis 与 `SmoothDamp`，不是硬切翻面
     - 但 `GetDisplayDecisionSamplePoint()` 现在仍直接走 `SpringDay1UiLayerUtility.GetInteractionSamplePoint()` 的脚底采样，`TryGetCenterSamplePoint()` 尚未接入实际翻面判定。
  5. 最近边界点与提示范围已经不是旧的小数值：
     - `ApplyDay1WorkbenchTuningIfNeeded()` 会把 `interactionDistance >= 1.42`、`overlayAutoCloseDistance >= 3.2`、`bubbleRevealDistance >= 2.4`
     - `GetClosestInteractionPoint()` 也会综合 `collider envelope / visual point / collider.ClosestPoint`
     - `SpringDay1InteractionPromptRuntimeTests.CraftingStationInteractable_PrefersNearestVisualEdgeOverFarColliderEnvelope()` 已守住“不被偏上 collider 包络短路”
     - 但玩家若仍感到“范围偏小”，更像是脚底 sample 与 `boundaryDistance` / 翻面判定联动残留，而不是 tunning 常量完全没加。
- 当前关键判断：
  - 当前 Workbench 不是“整条链都没做”，而是：
    1. 配方左列仍有 runtime 壳复用口子
    2. detail 文案链基本已补，但 `overlay visible` 时的 close 文案打不到
    3. 队列/追加只在 overlay 本地成立，导演层 live 进度没接上
    4. 翻面和平移已做，但脚底采样旧口子还在
- 验证状态：
  - `静态推断成立`
  - 本轮未改代码、未跑 Unity、未执行测试。
- 当前恢复点：
  - 若下一轮继续真实施工，最值钱的顺序应是：
    1. `SpringDay1Director.NotifyWorkbenchCraftProgress()` 正式接入 overlay live 进度
    2. WorkbenchOverlay 左列补“前台行文本真实非空”的测试
    3. 把翻面判定 sample 从脚底采样收紧到 center/bounds 混合

## 2026-04-04 补记：UI 协同提醒、UI 续工 prompt 与 spring-day1 自身执行任务单已补齐，当前线程已合法停回 `PARKED`

- 当前子工作区主线：
  - 用户要求先把 `spring-day1` 和 `UI` 的并行开发边界讲透，不要再靠聊天记忆协作；
  - 本轮子任务不是改代码，而是补齐：
    1. 给 UI 的剧情源协同开发提醒
    2. 给 UI 的正式续工 prompt
    3. 给我自己后续必须遵守的唯一执行任务单
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `sunset-prompt-slice-guard`
  - `preference-preflight-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
  - `user-readable-progress-report`
  - `delivery-self-review-gate`
- 本轮新增文档：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_剧情源协同开发提醒_03.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_玩家面续工与剧情源协同prompt_03.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_非UI剧情扩充执行约束与任务单_03.md`
- 本轮新增稳定结论：
  1. `spring-day1` 虽然不再主刀 UI 文件，但接下来会改到早期剧情源，因此仍会影响 UI 线程正在处理的任务栏、对话框、Prompt/Focus/Progress 文案和早期节奏。
  2. 正确口径不是“我不碰 UI 就完全不影响 UI”，而是：
     - 不碰 UI 文件 owner
     - 但会影响 UI 读取的剧情源
     - 因此必须单独给 UI 写协同提醒
  3. 后续 `spring-day1` 自己继续真实施工时，也不能再凭聊天记忆扩写，必须回到 `非UI剧情扩充执行约束与任务单_03` 这份唯一任务单。
  4. UI 与剧情线程的稳定分工再次钉死：
     - `spring-day1` = 剧情源、对白资产、阶段语义、角色出场
     - `UI` = 玩家真正看到和按到的正式面结果
- 本轮验证：
  - `git diff --check` 对 3 份新文档通过
  - 本轮没有改代码、没有进 Unity、没有新增 live 证据
  - 当前证据层级：`结构 / checkpoint`
- 本轮 thread-state：
  - 沿用既有 `ACTIVE` slice 完成文档补齐
  - `Ready-To-Sync`：未跑，原因：本轮不是 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 如果下一轮继续真实施工，我只能从：
    - `2026-04-04_spring-day1_非UI剧情扩充执行约束与任务单_03.md`
    规定的顺序开刀，先做 `CrashAndMeet / EnterVillage`
  - 如果下一轮要转发给 UI，直接使用：
    - `2026-04-04_UI线程_玩家面续工与剧情源协同prompt_03.md`

## 2026-04-04 补记：两份“继续施工引导 prompt”已补齐，后续给 UI 和给我自己都有可直接转发的入口

- 当前子工作区主线：
  - 用户要求在现有正文文档之外，再单独补两份真正可直接转发的“引导壳 prompt”：
    1. 给 UI 的继续施工引导
    2. 给 `spring-day1` 自己的继续施工引导
- 本轮新增文档：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_继续施工引导prompt_04.md`
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_继续施工引导prompt_04.md`
- 本轮新增稳定结论：
  1. 给 `UI` 的 prompt 不能只是让它“看提醒并回一条已读”，而必须明确告诉它：
     - 吸收提醒
     - 不中断当前 slice
     - 继续把玩家面剩余问题往前做
  2. 给我自己的 prompt 也不能只重复一句“继续做剧情”，而要把：
     - 原案文档
     - 设计文档
     - 框架任务单
     - 当前执行任务单
     一起收成下一刀唯一入口。
  3. 这样后续不管是给 UI 转发，还是我自己下一轮继续，都不需要再从长聊天里拼语义。
- 本轮验证：
  - `git diff --check` 对两份新 prompt 文件通过
  - 本轮没有改代码、没有进 Unity、没有新增 live 证据
  - 当前证据层级：`结构 / checkpoint`
- 本轮 thread-state：
  - `Begin-Slice`：已跑
  - `Ready-To-Sync`：未跑，原因：本轮不是 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 给 UI 转发时，优先使用：
    - `2026-04-04_UI线程_继续施工引导prompt_04.md`
  - 我自己下一轮继续时，优先使用：
    - `2026-04-04_spring-day1_继续施工引导prompt_04.md`
## 2026-04-04 补记：按新增设计文档重判 Town 在 spring-day1 扩充中的职责

- 当前子工作区主线没有换题：
  - `spring-day1` 当前唯一主刀仍是 `CrashAndMeet / EnterVillage` 的前半段剧情扩充；
  - 这条刀明确禁止提前写 `Town` scene、禁止抢跑 `Town` 正式迁移。
- 这轮只读重判后，Town 在本次 spring 剧情扩展里的正确职责被收束为两层：
  1. **当前不该接的**：
     - 不接 `CrashAndMeet / EnterVillage` 的剧情源主刀
     - 不接 `StoryPhase` 外壳改写
     - 不接 UI 玩家面
     - 不提前把 Day1 正式前半段直接写进 `Town`
  2. **后续应该接的**：
     - 村民日常站位与漫游
     - 围观村民 / 饭馆村民 / 小孩的常驻背景层
     - `FreeTime` 的 `Town` 夜间见闻包
     - 老杰克 / 老乔治 / 老汤姆 / 小米这类“村庄本来就在运转”的主要出现面
     - Day1 之后的村庄生活面
- 这轮新增的稳定判断：
  1. `temp scene` 仍只负责“受伤、被带走、被安置、被教会第一天怎么活下来”的硬剧情链；
  2. `Town` 负责“真正的村庄存在感”和“后续 NPC 的主要生活面”；
  3. 正确迁移顺序不是先往 `Town` 里塞剧情，而是：
     - 先把 Day1 正式主承载在 temp 主线里站稳
     - 再把群像主要出现面迁到 `Town`
     - 最后再收 temp 中的临时 crowd 代理
  4. 对当前 `Town` 线程/接盘位来说，最合理的现实动作应是：
     - 先把 `Town` 自己修到可稳定承接
     - 再准备“村口围观位 / 路边小孩视线位 / 夜间见闻点 / 饭馆背景层 / 日常站位”这些承载结构
     - 等剧情源和切场合同稳定后再正式接主出现面
- 本轮验证：
  - 只读完整回看了：
    - `2026-04-04_spring-day1_原剧本回正与Town承接剧情扩充设计_01.md`
    - `2026-04-04_spring-day1_非UI剧情扩充框架落地任务单_01.md`
    - `2026-04-04_spring-day1_CrashAndMeet-EnterVillage剧情扩充任务单_02.md`
    - `2026-04-04_spring-day1_非UI剧情扩充执行约束与任务单_03.md`
    - `2026-04-04_UI线程_剧情源协同开发提醒_03.md`
    - 两份 `继续施工引导prompt_04.md`
  - 本轮没有改代码、没有改 scene、没有进 Unity
  - 当前证据层级：`结构 / 责任边界判断`
- 当前恢复点：
  - 若下一轮继续 `spring-day1` 主线，仍只能先做 `CrashAndMeet / EnterVillage`
  - 若下一轮轮到 `Town` 接盘，正确入口已不再是“补剧情源”，而是“修好 Town 现场并准备村庄承载层”

## 2026-04-04 补记：CrashAndMeet / EnterVillage 当前补丁只读复核后，真实剩余缺口已收窄到“1 个提交阻断 + 1 个文案残留 + 1 个验证空洞”

- 当前子工作区主线没有换题：
  - 仍是 `CrashAndMeet / EnterVillage` 的内部剧情扩充；
  - 本轮子任务是按用户点名文件做只读复核，不改代码、不改资产，只判断当前补丁相对任务单还差什么、有没有明显编译/逻辑/测试风险，以及提交后下一刀该切什么。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-workspace-router`
  - `delivery-self-review-gate`
- 本轮手工等价执行：
  - `sunset-startup-guard`
- 本轮只读核对文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset`
  - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset`
  - `Assets/111_Data/Story/Dialogue/SpringDay1_VillageGate.asset`
  - `Assets/111_Data/Story/Dialogue/SpringDay1_HouseArrival.asset`
  - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
- 本轮新增稳定结论：
  1. 这批改动在结构上已经基本达到当前任务单对前半段的完成定义：
     - `CrashAndMeet` 已补成“醒来 / 语言错位 / 危险逼近 / 撤离起点”
     - `EnterVillage` 已补成“进村围观 / 闲置小屋安置 / 艾拉接手前置”
     - `HealingAndHP` 的后续接点仍保留在导演层里。
  2. 当前唯一明确的提交级红点不是编译错误，而是 `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset` 仍有 `git diff --check` 报的 trailing whitespace；不清掉，这一刀还不能算白名单 clean。
  3. 当前唯一明确的用户面残留是 `SpringDay1Director.BuildPromptItems(StoryPhase.HealingAndHP)` 里仍写着“等待首段对话完成后触发”，和新链路“进村安置结束后进入疗伤”已经不一致。
  4. 当前最大的真实风险不在脚本静态红错，而在验证空洞：
     - `validate_script` 对 `SpringDay1Director.cs` 为 `0 error / 2 warning`
     - `validate_script` 对 `SpringDay1DialogueProgressionTests.cs` 为 `0 error / 0 warning`
     - 但这份测试类本质上仍是大量 `File.ReadAllText + StringAssert.Contains` 的静态文本断言，没有真正跑通 `FirstDialogue -> Followup -> VillageGate -> HouseArrival -> HealingAndHP` 的运行链。
  5. `SpringDay1DialogueProgressionTests.cs` 不是这条刀的隔离测试类：
     - 它从开场对白一路串到 UI、Workbench、Bed、PromptOverlay、自由时段等多根路径；
     - 因此即便只验证这一刀，也可能被别的旧根噪音拖红，不适合作为“当前补丁是否真正闭环”的唯一绿灯。
- 本轮验证：
  - `git diff --check` 目标文件集报 1 个问题：
    - `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset:47` trailing whitespace
  - `validate_script`：
    - `SpringDay1Director.cs`：`0 error / 2 warning`
    - `SpringDay1DialogueProgressionTests.cs`：`0 error / 0 warning`
  - 本轮没有改代码、没有进 Unity、没有跑 live、没有执行测试。
- 当前证据层级：
  - `结构成立，live 待验证`
- 当前恢复点：
  - 如果先收当前补丁，必须先清：
    1. `SpringDay1_FirstDialogue.asset` 的 trailing whitespace
    2. `SpringDay1Director.cs` 里 `HealingAndHP` 的旧说明文案
  - 如果提交后继续下一刀，最值钱的不是马上扩后续剧情，而是补一个最小真实消费 probe：
    - 专门验证 `FirstDialogue -> Followup -> VillageGate -> HouseArrival -> HealingAndHP`
    - 最好从当前巨型 `SpringDay1DialogueProgressionTests.cs` 里拆出一条更隔离的早期链验证入口

## 2026-04-04 补记：opening 扩充第二个小 checkpoint 已把尾账、旧文案和最小消费 probe 一起补上

- 当前子工作区主线没有换题：
  - 仍是 `CrashAndMeet / EnterVillage` 的内部剧情扩充；
  - 本轮子任务是把上一轮只读复核点名的 3 个收口空洞直接补掉，而不是继续新增剧情段。
- 本轮显式使用：
  - `skills-governor`
  - `sunset-no-red-handoff`
- 本轮并行只读复核：
  - 使用一个 `gpt-5.4` 子智能体只读复核当前 opening 补丁剩余缺口；
  - 结论已吸收进本轮收口动作，后续应关闭该子线程，不再悬挂。
- 本轮实际改动：
  1. 清掉 `Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue.asset` 的尾部空白字符，解除当前提交阻断。
  2. 把 `SpringDay1Director.GetCurrentProgressLabel()` 与 `GetPromptFocusText()` 的 opening 文案再压近当前链路状态：
     - `CrashAndMeet` 可区分“已听懂村长的话，等待撤离矿洞口”
     - `EnterVillage` 可区分“村口围观已过，等待进屋安置”
     - 活动态下也能更准确给出“别回头撤离 / 先在闲置小屋落脚”等焦点提示。
  3. 把 `BuildPromptItems(StoryPhase.HealingAndHP)` 的旧说明从“等待首段对话完成后触发”改成“等待进村安置链收束后触发”，消除 opening 扩充后的语义残留。
  4. 把 `SpringDay1_HouseArrival.asset` 再钉实成“这不是谁家的房间”的闲置旧屋语义，避免回漂成家庭成员房间。
  5. 新增隔离测试：
     - `Assets/YYY_Tests/Editor/SpringDay1OpeningDialogueAssetGraphTests.cs`
     - 用 `AssetDatabase.LoadAssetAtPath<DialogueSequenceSO>()` 真正加载 4 份对白资产，验证：
       - followup 图谱是否是 `First -> Followup -> VillageGate -> HouseArrival`
       - `HouseArrival` 是否真的是最后一拍
       - 解码标志、相位推进和“矿洞危险 / 村民围观 / 小孩视线 / 不是谁家的房间 / 艾拉接手”这些 opening 语义是否都还在
- 本轮验证：
  - `git diff --check` 对本轮 owned 文件通过
  - `CodexCodeGuard`：
    - `SpringDay1Director.cs`
    - `SpringDay1OpeningDialogueAssetGraphTests.cs`
    - 结果 `CanContinue = true`
  - 本轮没有进 Unity、没有跑 live
  - 当前证据层级：`结构更稳 + 资产消费 probe 已补，live 仍待验证`
- 当前恢复点：
  - 如果继续这条线，下一步不该再往 `CrashAndMeet / EnterVillage` 里硬塞新段，而应优先看：
    1. 是否需要一次 Unity / EditMode 真实跑证据
    2. 或者在当前 slice 内停表，等待后续 live 验证再决定是否还要补导演层细节

## 2026-04-04 补记：当前 slice 已在两次本地提交后合法停车

- 本轮已形成 2 个本地 checkpoint：
  1. `741abea6 Expand spring day1 opening dialogue chain`
  2. `e8c56f98 Tighten spring day1 opening checkpoint`
- 当前 thread-state：
  - `Begin-Slice`：已跑并沿用
  - `Ready-To-Sync`：未跑，原因：本轮没有进入 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前 blocker：
  - `opening live validation pending`
- 当前恢复点：
  - 如果下一轮继续，不再先扩写内容；
  - 直接从“拿 opening 链更真实的 Unity / EditMode 证据”开始。

## 2026-04-04 补记：Town 已补成 spring-day1 可接的“村庄承载层壳”，但仍不是前半段剧情源

- 当前子工作区主线没有换题：
  - 仍是 `spring-day1` 的 Day1 扩充准备；
  - 本轮子任务是把 `Town` 从“只读边界判断”推进到“可承接后续村庄生活面”的最小结构，而不是把 `CrashAndMeet / EnterVillage` 正式写进 `Town`。
- 本轮实际落地：
  1. `Town.unity` 已正式纳入仓库，并提交进 `8df1b4e0 2026.04.04_Codex规则落地_03`。
  2. 在 `Town` 的 `SCENE` 根下新增 `Town_Day1Carriers`，只放空承载锚点：
     - `EnterVillageCrowdRoot`
     - `KidLook_01`
     - `DinnerBackgroundRoot`
     - `NightWitness_01`
     - `DailyStand_01`
     - `DailyStand_02`
     - `DailyStand_03`
  3. 没有把 `CrashAndMeet / EnterVillage` 的正式剧情源、对白资产或相位推进脚本直接写进 `Town`。
  4. 同一刀里把 `Town` 相机链从“运行时猜测”收成“场景显式接线”：
     - `CameraDeadZoneSync.mainCamera -> Main Camera`
     - `CameraDeadZoneSync.boundingCollider -> _CameraBounds`
     - `CinemachineConfiner2D.BoundingShape2D -> _CameraBounds`
  5. 作为配套运行时兜底，本轮还一起提交了：
     - `PersistentManagers.cs`
     - `PersistentObjectRegistry.cs`
     - `CameraDeadZoneSync.cs`
     - `DialogueUI.cs`
     用来收口 `Town` 承接时的常驻管理器、边界链和中文字体回退问题。
- 当前子工作区判断：
  - `Town` 现在已经有“村庄承载层壳”，可以继续承接围观村民、路边视线位、饭馆背景层、夜间见闻点这类后续扩充；
  - 但它仍然不是 `CrashAndMeet / EnterVillage` 的剧情源 owner。
- 当前验证状态：
  - `git diff --check`：代码文件仅有 LF/CRLF 警告；
  - `Town.unity` 静态结构与引用补线已核对；
  - 本轮没有进 Unity、没有做 live 转场复验；
  - 当前只能宣称：`结构与承载壳成立，live 待验证`。
- 当前恢复点：
  - `spring-day1` 后续若继续接 `Town`，应优先消费 `Town_Day1Carriers` 这批空锚点；
  - 不要回头把 `CrashAndMeet / EnterVillage` 的正式剧情源塞进 `Town`。

## 2026-04-04 只读剧情审计：收出 Day1 后半天最小 beat 清单

- 本轮性质：
  - 只读审计；未进入真实施工，未跑 `Begin-Slice`
  - 当前 live 状态沿用上一轮 `PARKED`
- 用户目标：
  - 回读原案与现有设计文档，为 `HealingAndHP -> DayEnd` 这条后半天链收出“最小但完整”的剧情 beat 清单
- 本轮强制回读材料：
  1. `春1日_坠落_融合版.md`
  2. `Deepseek聊天记录001.md`
  3. `Deepseek-2-P1.md`
  4. `2026-04-04_spring-day1_原剧本回正与Town承接剧情扩充设计_01.md`
  5. `2026-04-04_spring-day1_非UI剧情扩充框架落地任务单_01.md`
- 本轮收出的后半天最小剧情口径：
  1. `HealingAndHP`
     - 艾拉检查伤势并正式接手治疗
     - 治疗魔法生效，`HP` 首次出现并常驻
     - 艾拉对主角服饰 / 来历的好奇与谨慎并存
     - 村长在旁稳定局面，明确“先活下来，别的后面再说”
  2. `WorkbenchFlashback`
     - 村长明确这就是村里当前最好但仍然很基础的工作台
     - 玩家触碰工作台触发回闪
     - 玩家回忆起基础工具 / 材料 / 配方碎片
     - 艾拉被主角的怪异自言自语再吓退半步
     - 村长把玩家从闪回拉回现实，并转去“先学会自己活下去”
  3. `DinnerConflict`
     - 晚饭先承担“劳作后真正吃上饭”的喘息
     - 卡尔带着明确敌意出现，质疑外来者为什么能被带回村并分到资源
     - 村长压住场面，但压不掉卡尔的不信任
     - 冲突后要留一拍沉默 / 余味，不能吵完就硬切功能
  4. `ReturnAndReminder`
     - 黄昏回屋路上补一段“村子收摊、夜色压下来”的归途
     - 村长正式说出“两点前必须睡”的夜间规则
     - 规则语义必须是村里人真的怕，不是系统播报
     - 村长离开后，玩家第一次被单独留在这个地方
  5. `FreeTime`
     - 自由时段不是新大剧情，而是让玩家看见村子夜里的 3-4 个见闻点
     - 可见点最少应包括农田尾声 / 铁匠铺暗灯 / 村边或码头剪影 / 小孩或群众视线中的至少几项
     - 这段要同时保留“暂时被接纳”和“仍是外来者”的双重感
     - 夜间压力提示逐步抬升到睡觉前
  6. `DayEnd`
     - 玩家主动上床或被两点规则逼到必须结束当天
     - Day1 收束时要留下三层余味：被村庄暂时接住、卡尔敌意未解、夜里不对劲
- 本轮同步钉实的必须出声 / 被提及角色：
  - 必须出声：`马库斯 / 艾拉 / 卡尔`
  - 最少要被提及或作为夜间见闻出现：`老杰克 / 老乔治 / 老汤姆 / 小米 / 围观村民 / 饭馆村民 / 小孩`
- 本轮同步钉实的禁偏项：
  1. 不把疗伤段写成精力条先出现；顺序必须仍是 `HP -> EP`
  2. 不把工作台写成“外面世界更先进”的串台口径
  3. 不把 Day1 种子回漂成大蒜；正式口径仍是花菜
  4. 不把安置屋写成大儿子的房子；必须是闲置旧屋
  5. 不把晚餐写成单纯功能恢复点；它必须承担卡尔敌意
  6. 不把 `101~301` 后补 crowd 槽位写成原案正式主角色真名
- 当前恢复点：
  - 如果后续继续落地 `HealingAndHP -> DayEnd`，应以本轮 beats 为最小剧情圣经；
  - 先补阶段内部剧情步与对白资产，不先漂回 UI 或别的实现层。

## 2026-04-04 opening 验证续补：targeted menu 已接回 opening，旧测试口径已同步回正

- 当前子工作区主线没有换题：
  - 仍是 `spring-day1` 的非 UI opening 收口；
  - 这轮子任务只做“让 opening 这条链更容易点跑验证，并把自家旧测试口径和当前导演层重新对齐”。
- 本轮实际完成：
  1. 在 `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs` 新增 opening 专项入口：
     - `Sunset/Story/Validation/Run Opening Bridge Tests`
     - 对应结果文件：`spring-day1-opening-bridge-tests.json`
     - 目前挂入 4 个 opening 相关测试：
       - `SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_FormExpectedFollowupGraph`
       - `SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_PreserveOpeningSemantics`
       - `SpringDay1OpeningRuntimeBridgeTests.HouseArrivalCompletion_ShouldBridgeIntoHealingAndHp`
       - `SpringDay1OpeningRuntimeBridgeTests.LiveValidationRunner_ShouldRecommendOpeningActionsForCrashAndEnterVillage`
  2. 回正 `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs` 里残留的旧 opening 推荐动作断言：
     - 旧口径仍写“等待进村围观与安置收束……”
     - 现已同步为导演层当前真实字符串：
       `等待进村围观、闲置小屋安置与艾拉接盘前置收束；若链路未续播可再次触发 NPC001。`
- 本轮边界与判断：
  - 没有回漂到 UI owner，也没有碰 `Primary.unity / GameInputManager.cs`；
  - 这轮最核心的判断是：opening 当前更真实的缺口已经不是再加剧情字，而是验证入口和测试口径要跟上当前导演层。
- 本轮验证状态：
  - `git diff --check -- Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：通过
  - 两个 touched 文件的尾随空白扫描：未发现尾账
  - `sunset_mcp.py validate_script` 与直接 `CodexCodeGuard` 这轮都被本机 `dotnet` 构建/编译链卡在超时：
    - `subprocess_timeout:dotnet:600s`
  - 因此这轮只能诚实写：
    - `文本层与测试口径已回正`
    - `CLI 程序集级验证被工具超时阻断`
    - `Unity/live 仍待验证`
- thread-state：
  - 本轮沿用已开的 slice 继续最小施工
  - `Ready-To-Sync`：未跑，原因：本轮不是 sync 收口
  - `Park-Slice`：已跑
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 如果下一轮继续 opening，这轮新增的 menu 可以直接作为最小点跑入口；
  - 更值钱的下一步不再是继续扩 opening 文案，而是拿到真实 CLI/Unity 证据，确认 opening bridge tests 和实际消费链一致。

## 2026-04-04 中场协同补记：已给 UI 收一份“只盯缺字链 fresh live”的窄切续工 prompt

- 当前子工作区主线没有换题：
  - `spring-day1` 自己仍守 non-UI opening；
  - 但用户中场要求我基于 UI 最新进度，判断是否需要给 UI 一份新的续工 prompt。
- 本轮对子线程协同的判断：
  1. 需要；
  2. 而且不能再给 UI 一个“继续收 UI/UE 全链”的大散 prompt；
  3. 当前最值钱的下一刀必须压回：
     - `缺字链 fresh live`
     - 或 `第一真实 blocker 报实`
- 本轮新增 prompt 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_UI线程_缺字链fresh-live与第一真实blocker续工prompt_05.md`
- 这份 prompt 的核心切刀：
  1. 只盯 4 个玩家面 case：
     - 开局左侧任务栏
     - 中间任务卡
     - 村长 `继续`
     - Workbench 左列 recipe
  2. 明确禁止 UI 线程这轮回漂到：
     - 多悬浮框 `3x2`
     - Workbench 全量 polish
     - 气泡总整合
     - 或回吞剧情 owner
  3. 强制 UI 线程把：
     - `结构层证据`
     - `体验/live 证据`
     分开回
  4. 如果仍未压住缺字链，只允许报第一真实 blocker，不准再泛修 backlog
- 当前恢复点：
  - 用户若要现在转发给 UI，优先转这份 `prompt_05`；
  - `spring-day1` 自己仍不接 UI owner，继续保持 non-UI 主线边界。

## 2026-04-04 中场协同补记：已给 `spring-day1` 收一份“opening 验证闭环 / 第一真实 blocker”窄切续工 prompt

- 当前子工作区主线没有换题：
  - `spring-day1` 仍守 `CrashAndMeet / EnterVillage` 的 non-UI opening；
  - 用户中场让我基于 `day1` 最新回卡，判断是否要给它一份新的续工 prompt。
- 本轮对子线程协同的判断：
  1. 需要；
  2. 但不该再让 `day1` 继续补 opening 结构；
  3. 当前最该压回的一刀，只能是：
     - `opening 验证闭环`
     - 或 `第一真实 blocker 报实`
- 本轮新增 prompt 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_spring-day1_opening验证闭环与第一真实blocker续工prompt_06.md`
- 这份 prompt 的核心切刀：
  1. 把当前已接受基线冻结为：
     - non-UI opening 归 `spring-day1`
     - UI 仍归 UI 线程
     - opening menu 已存在
     - 旧断言已回正
  2. 强制 `spring-day1` 这轮只交二选一结果：
     - fresh opening 验证证据
     - 第一真实 blocker
  3. 明确禁止继续：
     - 扩剧情结构
     - 扩 opening 文案
     - 碰 UI owner
     - 顺手推进后半段或别的 bridge
  4. 强制把：
     - `结构线证据`
     - `验证 / live 证据`
     分开回
- 当前恢复点：
  - 如果用户现在要继续压 `day1`，优先转发这份 `prompt_06`；
  - `spring-day1` 后续最值钱的推进，不再是“继续加结构”，而是“把验证闭环或 blocker 说死”。

## 2026-04-04 补记：`SpringDay1PromptOverlay`“任务列表背景还在但文字大面积消失”只读根因重排

- 当前子工作区主线没有换题：
  - 仍是 `spring-day1` / `UI` 并行链里的 Day1 PromptOverlay 玩家面稳定性；
  - 本轮子任务只做单文件只读排查：为什么 [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs) 会出现“任务列表背景还在但文字大面积消失”的假活状态。
- 本轮只读结论重排后，最可能的前 3 个代码根因是：
  1. 字体可读性链过于激进，先把中文字体判 unusable，再把整页文本回退到 `TMP_Settings.defaultFontAsset`，导致背景仍在、中文文本大面积掉空。
     - 关键位置：
       - `ResolveFontAsset()` / `CanFontRenderText()` / `IsFontAssetUsable()`
       - `EnsurePromptTextReady()` / `EnsurePromptTextContent()`
  2. runtime shell 复用闸门仍然偏宽，只要前后台页里“有一条能 bind 的 row”就可能继续复用半残页壳；后面的 `BindExistingRows()` / `ApplyRowsToPage()` 再去补救，容易留下“有壳没字”的假活。
     - 关键位置：
       - `EnsureRuntime()` / `TryUseExistingRuntimeInstance()` / `TryBindRuntimeShell()`
       - `CanReuseRuntimeInstance()` / `CanBindPageRoot()` / `HasBindableRowChain()`
       - `BindExistingRows()` / `BindRow()`
  3. hide/show + queued reveal + front/back flip 的恢复路径没有强制重新证明“前台页已真正可读”；同签名路径会直接 `FadeCanvasGroup(1)`，而不是必然重跑一次前台页内容重建。
     - 关键位置：
       - `LateUpdate()`
       - `NeedsReadableContentRecovery()`
       - `TransitionToPendingState()` / `PlayPageFlip()`
       - `WaitAndRevealQueuedPrompt()` / `ApplyPageVisibility()`
- 当前子工作区最值钱的下一刀判断：
  - 先别碰 `SpringDay1Director`；
  - 优先收紧 `SpringDay1PromptOverlay` 自己的 runtime shell 健康判定，并在 `ApplyStateToPage()` 之后补一层“前台页 title/subtitle/focus/footer + 第一条 row 的 label/detail 已真可读”的结果断言；
  - 这样能先把“壳复用假过线”和“真正字体坏链”分开。
- 本轮验证状态：
  - `静态推断成立`
  - 未改代码、未进 Unity、未跑 live。
- 当前恢复点：
  - 如果下一轮继续这条线，优先做 `PromptOverlay` 本文件内的前台页健康判定与结果断言；
  - 如果这刀做完后仍空字，再把第一嫌疑切到共享中文字体链。

## 2026-04-04 中场协同补记：已给 `NPC` 收一份“玩家面 NPC 方向分工与第一刀认领” prompt

- 当前子工作区主线没有换题：
  - `spring-day1` 仍守 Day1 剧情源 / non-UI 主线；
  - 本轮子任务是用户明确要求我先停自己当前施工，给 `NPC` 发一份专门做 NPC 方向分工的 prompt。
- 本轮新增稳定结论：
  1. `NPC` 现在不该再被当成泛 UI owner；
  2. 但玩家面剩余问题里，确实有一大簇应该由 `NPC` 接：
     - NPC / 玩家 / NPC-NPC 气泡的显示层级与 speaking-owner
     - 气泡不被场景遮挡
     - 气泡背景不透明
     - NPC 头顶提示退场与 NPC 自己的提示语义输出链
     - 正式/非正式聊天优先级与关闭/阻断闭环
  3. 这些东西必须先收成 `exact-own / 协作切片 / 明确不归我`，再由 `NPC` 自己认领第一刀。
- 本轮新增 prompt 文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-04_NPC线程_玩家面NPC方向分工与第一刀认领prompt_01.md`
- 这份 prompt 的核心切刀：
  1. 冻结当前三方基线：
     - `UI` 管壳体与 formal-face
     - `spring-day1` 管剧情顺序与状态
     - `NPC` 管 NPC 玩家可见聊天 / 气泡 / 提示行为链
  2. 先让 `NPC` 把剩余需求总账拆成 own matrix；
  3. 再只认领第一刀最该从 NPC 开始做的那一刀，而不是直接大铺实现。
- 当前恢复点：
  - 如果用户现在要转发给 `NPC`，优先转这份 `prompt_01`；
  - 我方 `UI` 当前保持 `PARKED`，不在这轮继续吞 NPC 方向业务实现。

## 2026-04-04｜补记：为避免剧情 / 群像 / 气泡继续串线，已暂停 opening blocker 并先给 `NPC-v` 下发新 prompt

- 当前子工作区主线没有换题：
  - 仍是 `spring-day1` 的 opening 验证闭环与真实 blocker 收口；
  - 本轮子任务只是协同分工，不是改写主线。
- 本轮实际完成：
  1. 已将 `spring-day1 / UI / NPC-v` 三方边界重新钉死：
     - `spring-day1` 只守 opening / Day1 逻辑 / 当前 blocker
     - `UI` 只守玩家面 `UI/UE`
     - `NPC-v` 只守原剧本角色回正、NPC 本体与气泡运行态
  2. 已新增可直接转发的 `NPC-v` 续工文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-04_NPC-v_春一日原剧本群像回正与NPC本体收口prompt_05.md`
  3. 这份 prompt 明确把 `pair bubble`、旧气泡样式、`101~301` 回正和匿名降级策略全部压回 `NPC-v` own。
- 本轮验证状态：
  - `文档分工已落盘`
  - 未继续代码施工。
- 当前恢复点：
  - `NPC-v` 续工 prompt 已就位；
  - 我这条线下一次恢复时，回到 `opening validation / PromptOverlay blocker`，不再自己吞 NPC 本体线。

## 2026-04-05｜补记：opening 验证闭环已压到 4/4 通过，`PromptOverlay` destroyed-row blocker 已补守卫

- 当前子工作区主线没有换：
  - 仍是 `spring-day1` opening 验证闭环与当前真实 blocker 收口；
  - 这轮完成后停车，等待用户做 runtime 复测。
- 本轮实际完成：
  1. `PromptOverlay` 本体补口：
     - [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
     - `HasReadablePromptRow()` 改成把 destroyed row 明确视为 unreadable
     - `AnimateRowCompletion()` 开头、循环内、收尾前都补了 `IsRowBindingAlive(row)` 守卫
     - 目的：任务行在完成动画中途被销毁/重绑时，不再继续触碰 stale `CanvasGroup`
  2. `PromptOverlay` 回归探针补强：
     - [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
     - 新增 `PromptOverlay_CompletionAnimation_ShouldStopTouchingDestroyedRowCanvasGroup`
     - 现有 `PromptOverlay_ShouldRecoverFromDestroyedRowCanvasGroup`
     - fresh 结果：`2/2 PASS`
  3. opening 资产图谱验证链补强：
     - [SpringDay1OpeningDialogueAssetGraphTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningDialogueAssetGraphTests.cs)
     - `LoadAsset()` 先 `ImportAsset(ForceUpdate)` 再读取
     - 真正根因已坐实在 `.meta` YAML 空值格式
  4. followup 资产 root cause 已压实：
     - [SpringDay1_FirstDialogue_Followup.asset.meta](D:/Unity/Unity_learning/Sunset/Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset.meta)
     - Unity fresh 报错不是“文件物理不存在”，而是 `.meta` 的空字段写法被导入链判成坏 YAML
     - 当前现场经 fresh import 后，opening graph `2/2 PASS`
  5. opening runtime bridge 测试口径回正：
     - [SpringDay1OpeningRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs)
     - 去掉 `SendMessage` 的 `ShouldRunBehaviour()` 噪音
     - `InvokeInstance()` 扩到 `Public + NonPublic`
     - 为 `HouseArrivalCompletion_ShouldBridgeIntoHealingAndHp` 增补临时 `Primary` 测试场景上下文，不碰真实 `Primary.unity`
  6. 定向测试入口已补：
     - [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
     - 新增：
       - `Sunset/Story/Validation/Run PromptOverlay Guard Tests`
       - `Sunset/Story/Validation/Run Opening Graph Tests`
- fresh 验证结果：
  - `Run PromptOverlay Guard Tests`：`2 PASS / 0 FAIL`
  - `Run Opening Graph Tests`：`2 PASS / 0 FAIL`
  - `Run Opening Bridge Tests`：`4 PASS / 0 FAIL`
  - `python scripts/sunset_mcp.py errors --count 30`：
    - 当前未见 `PromptOverlay MissingReference`
    - 当前未见 `ShouldRunBehaviour`
    - 剩余一条 `UnityEditor.Graphs.Edge.WakeUp()` 的 `NullReferenceException`，判定为 domain reload / Editor 噪音，不是这轮 own 问题
- 本轮未完成：
  - 还没拿到用户自己的 live 路径复测；
  - [SpringDay1_FirstDialogue_Followup.asset.meta](D:/Unity/Unity_learning/Sunset/Assets/111_Data/Story/Dialogue/SpringDay1_FirstDialogue_Followup.asset.meta) 目前仍是 Unity 锁定态，导致我没法把空值尾随空格再收成更干净的文本格式；但 fresh opening graph 已经通过，功能上不再阻断。
- 当前恢复点：
  - 如果下一轮继续本线，优先让用户按真实剧情路径再打一次：
    - 疗伤后 Prompt 是否还会炸 destroyed-row
    - opening 从 first -> followup -> gate -> house-arrival 是否还会断桥
  - 若用户 live 仍能复现，再直接回到 `PromptOverlay` 运行态复盘；否则这刀可视为验证闭环已成立。

## 2026-04-05｜补记：已审 NPC 回执并重列 Day1 own 下一刀任务单

- 当前子工作区主线没有换：
  - `spring-day1` 继续只守 Day1 own，不回吞 UI 与 NPC own。
- 本轮实际完成：
  1. 审核了 `NPC-v` 最新回执，并正式接受它退出：
     - `SpringDay1Director.cs`
     - `SpringDay1NpcCrowdDirector.cs`
     - `Primary.unity`
     - `PromptOverlay`
     - opening / Day1 正式剧情控制
  2. 以 Day1 owner 身份补给 `NPC-v` 真值：
     - `formal > casual > ambient`
     - `NPC001/002/003` 为原 Day1 主角色承载
     - `101~301` 无 exact mapping 时统一降为群众层 / 线索层 / 氛围层
  3. 已新增可直接转发的 `NPC-v` 续工文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_NPC-v_Day1真值补线与NPC正式非正式优先级续工prompt_06.md`
  4. 已新增我方下一刀任务单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_Day1后半段正式剧情收口任务单_07.md`
  5. 当前我方最深下一刀已经重新说死：
     - 直接砍穿 `HealingAndHP -> WorkbenchFlashback -> FarmingTutorial -> DinnerConflict -> ReturnAndReminder -> FreeTime -> DayEnd`
- 当前恢复点：
  - 等用户转发 `prompt_06` 给 `NPC-v`；
  - 我方之后直接按 `任务单_07` 开砍 Day1 后半段正式剧情版。

## 2026-04-05｜补记：101~301 crowd 资产仍残留 Day1 正式角色暗示，只读回正建议已收口

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守 Day1 own / 正式剧情边界；
  - 本轮子任务是只读审 `Assets/111_Data/NPC/SpringDay1Crowd` 与 `Assets/222_Prefabs/NPC` 中 `101/102/103/104/201/202/203/301` 的 crowd 资产，确认哪些字段还把它们写成 Day1 固定具名角色。
- 本轮实际完成：
  1. 逐个复核了 8 组 `DialogueContent + RoamProfile + Prefab`，确认问题不只在文件名。
  2. 已坐实的“正式角色暗示”共有 4 层：
     - `m_Name / 文件名 / RoamProfile 名称` 仍是 `LedgerScribe / Hunter / ErrandBoy / Carpenter / Seamstress / Florist / CanteenKeeper / GraveWardenBone`
     - `bundleId` 仍是单角色专属事件标签，如 `101-ledger-trace`、`301-bone-night`
     - `pairDialogueSets` 里仍互相直呼 `莱札 / 阿澈 / 沈斧 / 麦禾 / 桃羽 / 白槿 / 炎栎 / 朽钟`
     - prefab 内还各自内嵌了一份职业化 `selfTalkLines / chatInitiatorLines / chatResponderLines`
  3. `RoamProfile` 的基础漫游句本身已经偏 generic，不是主矛盾；真正该先回正的是命名层、配对称呼层、以及 `DialogueContent / Prefab` 的强职业独白层。
  4. 8 个编号的最小回正口径已收敛为：
     - `101` = 群众层 `记事/对账位`
     - `102` = 群众层 `后坡盯梢/看脚印位`
     - `103` = 群众层 `快腿见闻位`
     - `104` = 群众层 `修缮帮手位`
     - `201` = 群众层 `缝补/安抚位`
     - `202` = 群众层 `安神草/摆花位`
     - `203` = 群众层 `端汤/灶台位`
     - `301` = 群众层 `后坡守夜/怪谈位`
- 当前验证状态：
  - `静态只读审计成立`
  - 未改代码、未改资产、未进 Unity、未跑 `Begin-Slice`。
- 当前恢复点：
  - 若后续要真做“回正 101~301”，最小顺序应先动：
    - `m_Name / 文件名 / bundleId`
    - `pairDialogueSets` 的互称
    - prefab 与 `DialogueContent` 里的强职业独白
  - `npcId / partnerNpcId / 漫游参数` 暂不需要先动。

## 2026-04-05｜补记：后半段正式桥接已补到 `FarmingTutorial -> DinnerConflict` 即时接管，midday bridge fresh `8/8 PASS`

- 当前子工作区主线没有换：
  - 仍是 `spring-day1` 的 `Day1 后半段正式剧情收口`；
  - 本轮只继续 own 范围里的导演桥接、最小测试和验证入口，不回吞 UI / NPC / Town / 字体。
- 本轮实际完成：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 新增 `BeginDinnerConflict()`，把 `FarmingTutorial` 收束改成“立刻切相位 + 立刻排队正式晚餐对白”，不再空掉一拍；
     - `TickPrimarySceneFlow()` 里的晚餐 fallback 也统一走同一入口；
     - `DinnerConflict / ReturnAndReminder / DayEnd` 期间，工作台现在会明确让位给正式剧情：
       - `CanPerformWorkbenchCraft(...)` 给出正式 blocker
       - `ShouldExposeWorkbenchInteraction()` 在这几段返回 `false`
     - `DinnerConflict / ReturnAndReminder` 的 focus 文案也改成显式强调“formal 优先接管”。
  2. [SpringDay1MiddayRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1MiddayRuntimeBridgeTests.cs)
     - 新增 3 条 runtime bridge probe：
       - `WorkbenchCompletion_ShouldAdvanceIntoFarmingTutorial`
       - `FarmingTutorialCompletion_ShouldImmediatelyBridgeIntoDinnerConflict`
       - `DinnerAndReminderPhases_ShouldYieldWorkbenchToFormalStory`
  3. [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
     - `Run Midday Bridge Tests` 扩到当前 fresh 的 `8` 条测试。
- fresh 验证结果：
  - `git diff --check`（本轮 touched files）通过；
  - 命令桥先执行 `MENU=Assets/Refresh`，后执行：
    - `MENU=Sunset/Story/Validation/Run Midday Bridge Tests`
  - 结果文件：
    - `D:\Unity\Unity_learning\Sunset\Library\CodexEditorCommands\spring-day1-midday-bridge-tests.json`
  - fresh 结果：
    - `8 PASS / 0 FAIL`
  - 仍存在噪声：
    - `LiberationSans SDF` 缺中文 glyph warning 仍在 midday tests 输出中出现；
    - 本轮按用户口径不回扩到 TMP / 字体。
  - CLI 侧附加核查未闭环：
    - `python scripts/sunset_mcp.py errors --count 20` 返回 `127.0.0.1:8888` refused；
    - 因此这轮运行态证据以 Unity 命令桥 + targeted editmode 为准，不 claim CLI fresh console 已通过。
- 当前恢复点：
  - 这刀已经把“后半段 formal 桥接只靠口头说”推进成了导演硬约束 + runtime probe；
  - 下一刀最值钱的方向不再是重复修晚餐入口，而是继续压：
    - `ReturnAndReminder / FreeTime / DayEnd` 的尾声矩阵
    - 或把后半段已正式化范围收成更完整的用户向阶段结论。

## 2026-04-05｜补记：UI 只读定位 `004/005` 任务卡“只剩底板没字”主因在 PromptOverlay 字体覆盖判定

- 当前子工作区主线没有换：
  - 仍是 `spring-day1` 的 Day1 UI / 任务卡体验收口；
  - 本轮子任务是只读排查 `HealingAndHP -> WorkbenchFlashback -> FarmingTutorial` 交界处左侧任务卡“有底板没字”，不进入真实施工。
- 本轮实际完成：
  1. 只读复核了 `SpringDay1PromptOverlay.cs`、`DialogueUI.cs`、`SpringDay1Director.cs`，以及 `SpringDay1LateDayRuntimeTests.cs`、`SpringDay1MiddayRuntimeBridgeTests.cs`、`SpringDay1DialogueProgressionTests.cs`。
  2. 当前最像主根因的是 `SpringDay1PromptOverlay` 自己的字体选择链，而不是导演层没给数据：
     - `_fontAsset` 只在 `ResolveFontAsset()` 里按固定 `FontCoverageProbeText = "当前任务工作台继续制作"` 选一次；
     - `EnsurePromptTextReady()` / `EnsurePromptTextContent()` 后续即使遇到 `004/005` 新文本，也只会重复套用同一个 `_fontAsset`，不会按当前文本重新选字体；
     - `004/005` 的真实文案里大量字符不在这条固定 probe 内，例如 `靠近 / 看完 / 回忆 / 开垦 / 花椰菜 / 浇水壶 / 收集 / 木材 / 真正 / 基础`，因此更容易出现底板还在、TMP 字形没出来。
  3. `DialogueUI` 这条线更像触发边界而不是主故障：
     - `FadeNonDialogueUi()` / `ApplyNonDialogueUiSnapshot()` 会在对白阶段把 `PromptOverlay` 作为 sibling UI 关掉再恢复；
     - 所以问题最容易出现在 `003 -> 004` 和 `004 -> 005` 这种“对白收束后重新显字”的边界。
  4. `SpringDay1Director` 当前 `004/005` 的 prompt model 本身不是空的：
     - `BuildWorkbenchFlashbackPromptItems()` 明确给了 2 条 filled item；
     - `BuildFarmingTutorialPromptItems()` 明确给了 5 条 filled item；
     - 因此不像 `BuildPromptCardModel()` 返回空数据。
  5. 为什么玩家看到“只剩黑色透明底条”：
     - `PromptCardViewState.FromModel(..., maxVisibleItems = 1)` 当前只保留 1 条主任务行；
     - `ApplyRow()` 会保持 `row.root` active、`row.group.alpha = 1f`，并持续给 `row.plate` / `row.bulletFill` 上色；
     - 一旦文字因为字体覆盖失败没渲染出来，玩家就只剩一条半透明底板。
- 当前验证状态：
  - `静态推断成立`
  - 未改代码、未进 Unity、未跑 live。
- 当前恢复点：
  - 若后续真修，最小修复面优先只收在 `SpringDay1PromptOverlay.cs`：
    - 让字体选择改成按“当前 expected text”重选，而不是只认固定 probe；
    - 并在 `004/005` 边界补一条经过 `DialogueUI` hide/show 后仍能读出 row label/detail 的 runtime test。
  - 暂不建议先改 `SpringDay1Director` 文案或先动 `DialogueUI` 淡入淡出策略。

## 2026-04-05｜补记：Workbench 左列 recipe“可点但像空白”只读归因已收束到壳复用与 manual 行布局漏判

- 当前子工作区主线没有换：
  - 仍在 `spring-day1` / Day1 玩家面链的遗留问题排查范围内；
  - 本轮子任务被用户收窄为：只读检查 `SpringDay1WorkbenchCraftingOverlay` 左列 recipe 为什么会出现“还能点，但文字经常不显示/像空白”。
- 本轮只读检查了：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1WorkbenchCraftingOverlay.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringDay1UiPrefabRegistry.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\UI\Spring-day1\SpringDay1WorkbenchCraftingOverlay.prefab`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1Workbench\Recipe_9100_Axe_0.asset`
  - `D:\Unity\Unity_learning\Sunset\Assets\Resources\Story\SpringDay1Workbench\Recipe_9101_Hoe_0.asset`
- 当前稳定结论：
  1. 数据源不像主根因：
     - `EnsureRecipesLoaded()` 直接从 `Resources/Story/SpringDay1Workbench` 读取配方；
     - 当前 recipe 资产里有稳定的 `recipeName / description / requiredStation`；
     - `RefreshRows()` 也明确每轮都会把 `GetRecipeDisplayName()` 和 `BuildRowSummary()` 写进 `Name / Summary`。
  2. 最可疑根因 1 是 `runtime 壳复用门槛仍偏宽`：
     - `EnsureRuntime() -> TryUseExistingRuntimeInstance() -> CanReuseRuntimeInstance() -> HasReusableRecipeRowChain()` 只要求现有 `RecipeRow_*` 上的 `Button/Image/Name/Summary` 在检查当下“看起来可用”；
     - 它没有把“这套 row 壳后续 refresh 后是否仍能稳定可见”纳入复用判定；
     - 因此旧壳一旦带着还能点的 `Button/Image` 过关，后面就可能留下“按钮在，字没了”的半残状态。
  3. 最可疑根因 2 是 `manual recipe row` 的布局/裁切漏判：
     - prefab 里的 `RecipeRow_*` 不是代码新建的 `HorizontalLayoutGroup + TextColumn` 结构，而是 `Accent / IconCard / Name / Summary` 直挂在 row 下的 manual 壳；
     - `UsesManualRecipeShell()` 命中后会关掉 `recipeContentRect` 的 `VerticalLayoutGroup / ContentSizeFitter`，改由 `EnsureRecipeRowCompatibility()` + `EnsureManualRecipeRowGeometry()` 手工算位置；
     - 这套判据主要看 `Text` 自己的 `rect/alpha/font/text`，抓不到“文本仍有宽高，但被父级几何或 viewport 裁掉”的情况。
  4. 次级根因更像 `刷新时序 + manual 几何`，不像字体：
     - `RefreshRows()` 里先补文案，再走 `RefreshRecipeContentGeometry()`；
     - `EnsureRecipeRowCompatibility()` 在 manual 壳分支会依据当下 `rowRect.rect.width` 和子节点宽度推导文本高度与位置；
     - 如果这时拿到的是 stale 宽度，最终可能出现 row 仍可点、`HasReadableRecipeText()` 也未必立即判死，但玩家视觉上已经像空白。
  5. `字体可读性` 目前不像第一主嫌：
     - 脚本里已经有 `ResolveFont()`、`ApplyResolvedFontToShellTexts()`、`EnsureWorkbenchTextContent()`、`CanFontRenderText()` 多层兜底；
     - recipe 名称本身还是 `Axe_0 / Hoe_0` 这类英文，不符合“只有中文字库缺 glyph 才整行空掉”的表现。
- 当前最小修复面建议：
  1. 第一优先只收 `SpringDay1WorkbenchCraftingOverlay.cs`：
     - 收紧 `CanReuseRuntimeInstance()` / `HasReusableRecipeRowChain()` 的复用条件，别再只看“此刻有字且有宽高”，而要把 row 结构兼容性也纳进去；
     - 或更直接：一旦命中 manual/prefab row 壳，首次 `RefreshAll()` 前先 `RebuildRecipeRowsFromScratch()`，不要继续沿用旧 row。
  2. 第二优先补测试，不扩文件面：
     - 在 `SpringDay1LateDayRuntimeTests.cs` 增一条针对“可复用但文本不可见的 manual shell”或“refresh 后左列首行文本必须仍可读”的 probe；
     - 当前测试只守住“world-space stale 壳”和“缺失文本链的 incomplete 壳”，还没守“refresh 后视觉可读”。
- 当前验证状态：
  - `静态只读审计成立`
  - 未改代码、未改 prefab、未进 Unity、未执行测试。
- 当前恢复点：
  - 如果下一轮真修这条链，最值钱顺序应是：
    1. 先收紧左列 row 壳复用 / 首次强制重建
    2. 再补“refresh 后左列文本仍可读”的 runtime test
    3. 最后才考虑继续深挖字体或 copy 层

## 2026-04-05｜补记：后半段尾声矩阵 + PromptOverlay inactive 崩点 + TMP atlas 尾巴已一刀收平

- 当前子工作区主线没有换：
  - 仍是 `spring-day1` 的 `Day1 后半段正式剧情收口`；
  - 本轮子任务是在继续深砍 `ReturnAndReminder / FreeTime / DayEnd` 的尾声矩阵时，顺手把用户 live 报出的 `PromptOverlay inactive` runtime 崩点和 fresh console 里的 own/importer 噪音一起收口。
- 本轮实际完成：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 把 `FreeTime` 收成了真正的 `intro pending -> intro complete -> 夜间压力递进 -> DayEnd` 闭环：
       - `EnterFreeTime()` 不再一进相位就开放睡觉，而是先 `Show(FreeTimeIntroBridgePromptText)`；
       - `_freeTimeIntroCompleted` 现在成为正式 gate；
       - `HandleHourChanged()` 不会再在 intro 未完成时提前推进 `night / midnight / final-call`；
       - `IsSleepInteractionAvailable()` 必须等 intro 完成后才会放开；
       - `GetValidationFreeTimeNextAction()` / `TryAdvanceFreeTimeValidationStep()` 也会先报“先听完夜里的见闻”，不再偷跑到睡觉收束；
       - `FreeTime` intro 未完成时，工作台制作与交互提示也会继续让位给 formal 夜间见闻。
  2. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
     - 新增 runtime 自救：
       - `Show()` / `QueuePromptReveal()` 现在会先 `EnsureRuntimeObjectActive()`；
       - 解决了用户 live 路径里“艾拉回血后，导演层一调用 `Show()` 就因为 `SpringDay1PromptOverlay` 已 inactive 而无法起协程”的崩点。
  3. [SpringDay1LateDayRuntimeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs)
     - 新增 / 回正：
       - `PromptOverlay_Show_ShouldReactivateInactiveRuntimeInstanceBeforeStartingTransition`
       - `ReminderCompletion_ShouldEnterFreeTimeWithIntroPendingAndYieldWorkbenchToFormalNightIntro`
       - `FreeTimePlayerFacingCopy_ShouldTightenAcrossNightPressure` 先测 intro pending，再测 intro complete 后的 relaxed/final-call copy
       - `BedBridge_EndsDayAndRestoresSystems` 改成不再依赖会唤醒 `PersistentManagers` 的 `SetTime(...)`
       - `DayEndPlayerFacingCopy...` 断言回正到当前导演真实文案
  4. [SpringDay1MiddayRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1MiddayRuntimeBridgeTests.cs)
     - `DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime` 已回正到新真值：
       - 刚进 `FreeTime` 时先是 `等待自由时段见闻接管`
       - `sleepReady=false`
     - `EnsureTestPrimarySceneActive()` 改成保存到项目相对路径 `Temp/CodexEditModeScenes/Primary.unity`，清掉了这轮 own 的 `Invalid AssetDatabase path` console 错。
  5. [SpringDay1DialogueProgressionTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs)
     - 静态字符串护栏已同步到新语义：
       - `FreeTimeIntroBridgePromptText`
       - `自由时段见闻会先接管`
       - `听完村里夜间见闻`
       - `先把夜里的见闻听完，再决定今晚还做不做别的`
  6. [SpringDay1TargetedEditModeTestMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs)
     - `PromptOverlay Guard` 与 `Late-Day` 菜单都已扩到新 probe。
  7. [DialogueChinese Pixel SDF.asset](D:/Unity/Unity_learning/Sunset/Assets/TextMesh%20Pro/Resources/Fonts%20%26%20Materials/DialogueChinese%20Pixel%20SDF.asset)
     - `m_AtlasTextures` 末尾多余的空 `fileID: 0` atlas 槽位已剔掉；
     - 这轮 fresh refresh + tests 后，之前那条 `Importer(NativeFormatImporter) generated inconsistent result` 已不再出现在 fresh console。
- 本轮 fresh 验证：
  - `Run PromptOverlay Guard Tests` = `3 PASS / 0 FAIL`
  - `Run Late-Day Bridge Tests` = `5 PASS / 0 FAIL`
  - `Run Midday Bridge Tests` = `8 PASS / 0 FAIL`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 10`
    - `errors=0 warnings=0`
    - 当前剩余只看到测试结果落盘的 `Saving results to ... TestResults.xml` exception 噪音，已不再有 own blocker
- 当前阶段：
  - `ReturnAndReminder -> FreeTime -> DayEnd` 的 formal 尾声矩阵已经从骨架推进到“导演硬 gate + runtime probe + fresh console clean”三层一起站住；
  - `PromptOverlay inactive` 这条 live 崩点也已补回运行态护栏。
- 当前恢复点：
  - 下次若继续 `spring-day1`，不再回到“尾声链会不会断 / PromptOverlay 会不会在疗伤后炸”的阶段；
  - 直接从“后半段正式剧情还能继续往哪段扩厚，或是否转成用户终验包”往下接。

## 2026-04-05｜补记：已向典狱长发起 Town 承接边界与导演分场问询

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守 Day1 own 的剧情顺序、phase 真值与正式桥接；
  - 本轮子任务是把“导演线什么时候能按场地分场、按 Town 锚点排 NPC 剧本走位”收成一份给典狱长的精确问询，不回漂去写 Town scene / UI / NPC own。
- 本轮实际完成：
  1. 只读复核了当前 `Town` 的 accepted baseline：
     - `Town` 当前身份是“村庄承载层”，不是 `CrashAndMeet / EnterVillage` 前半段剧情源 owner；
     - 当前治理记录里可直接消费的 carrier 语义至少包括：
       - `Town_Day1Carriers`
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `DinnerBackgroundRoot`
       - `NightWitness_01`
       - `DailyStand_01~03`
  2. 结合用户最新补充事实，收紧了导演线当前真正要问的问题：
     - `矿洞` 还没做好；
     - `Town` 用户已经做了一部分；
     - `Primary` 只是当前临时承载，不应被误认成正式剧情长期场地；
     - 当前真正要判的是“哪些 beat 现在能按 Town 写戏，哪些还只能维持临时/抽象承接”。
  3. 已新增可直接转发给典狱长的文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_给典狱长_spring-day1与Town承接边界及导演分场问询_08.md`
- 当前结论：
  - 我现在不是要典狱长替我接盘 Day1；
  - 我要他回答的是：
    1. `Town` 当前正式身份与承载边界
    2. `spring-day1` 现在可消费的 `Town` 范围
    3. 当前可开始的导演分场范围
    4. 当前可开始的 NPC 剧本走位范围
    5. 哪些段落仍必须留在临时承载 / 抽象承接
- 当前验证状态：
  - `静态只读协同问询成立`
  - 本轮未改 runtime 代码、未进 Unity、未跑新的 targeted tests。
- 当前恢复点：
  - 等用户把 `问询_08` 转给典狱长并拿回边界回信；
  - 我下一步再据此决定：
    - 继续把 `CrashAndMeet / EnterVillage` 留在当前临时承载推进
    - 还是从某个明确 beat 开始，正式按 `Town` 做导演分场与群像调度脚本。

## 2026-04-05｜典狱长回裁定：Town 现可供导演线做“后续生活面分场”，但仍不支持整条 Day1 全量写死到 Town

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守 Day1 own 的剧情顺序、phase 真值与后半段正式桥接；
  - 本轮只是拿典狱长口径，把“Town 到底能消费到什么程度”裁定清楚。
- 本轮新增稳定裁定：
  1. `Town` 当前正式身份已经够清楚：
     - 村庄承载层
     - 面向 Day1 后续生活面、背景层、群像层、夜间见闻层、日常站位层
     - 不是 `CrashAndMeet / EnterVillage` 前半段剧情源 owner
  2. `spring-day1` 现在已经可以消费 `Town` 的范围，但只到“导演层 / 分场层 / 背景调度层”：
     - 可开始按 `Town_Day1Carriers` 的空锚点写戏
     - 可开始写 post-entry crowd / dinner background / night witness / daily stands 的导演脚本
     - 还不能把 runtime 切场、精确路径、相机联动、秒级走位写死在 `Town`
  3. phase 级裁定：
     - `CrashAndMeet`：继续临时承载，不按 `Town`
     - `EnterVillage`：必须拆成两段看
       - 进村前：继续临时承载
       - 进村后围观/安置/小屋外部氛围：可开始按 `Town` 做分场与背景层
     - `HealingAndHP`：继续临时/抽象承载
     - `WorkbenchFlashback`：继续临时/抽象承载
     - `FarmingTutorial`：继续临时/抽象承载
     - `DinnerConflict`：可开始按 `Town` 做晚餐背景层和人群关系，但核心 runtime 仍不要写死
     - `ReturnAndReminder`：可开始按“回到村中/村口安静下来”的氛围承接写分场，但核心推进仍保留抽象承载
     - `FreeTime`：可开始按 `Town` 做夜间见闻层与村中闲置站位层
     - `DayEnd`：可开始按 `Town` 的夜间见闻与次日站位语义做导演承接，但不先写 runtime 路径
  4. NPC 剧本走位当前可开始的粒度：
     - 可写：谁在什么时候出现、占哪个锚点、朝向谁、围观怎样让位、背景层怎么布人、夜间谁在什么方位投来目光或短句
     - 不可写死：tile 级路径、最终 nav 路线、切场 timing、相机联动、最终 spawn/runtime 触发
  5. 当前 first blocker 不是 `Town` 身份不清，而是：
     - `Town` 还没 `sync-ready`
     - 外线 blocker 仍在 `camera/frustum`、`DialogueUI/字体链`、`PlacementManager.cs compile red`
     - 因此导演线当前应消费“稳定锚点语义”，不要消费“live 已全闭环”的假前提
- 当前导演可消费语义表：
  - `Town_Day1Carriers`：Day1 后续村庄戏的总承载根
  - `EnterVillageCrowdRoot`：进村后围观层 / 让位层 / 初见集体视线
  - `KidLook_01`：单点儿童视线位 / 好奇观察位
  - `DinnerBackgroundRoot`：晚餐阶段背景层 / 远景生活层 / 非主戏人群层
  - `NightWitness_01`：夜间见闻位 / 夜里有人看见主角或投来一句话
  - `DailyStand_01~03`：第二天开始的村中日常站位层
- 当前恢复点：
  - 若继续 `spring-day1`，下一刀最合理的是：
    1. 继续把 `CrashAndMeet / EnterVillage` 前半段留在当前临时承载
    2. 从 `EnterVillage` 的 post-entry crowd 开始，把 `DinnerConflict / FreeTime / DayEnd` 的导演分场与背景调度正式按 `Town` 写
    3. 先写分场、锚点占位、出场关系、视线关系，不先写 runtime 切场与精确移动路线
- thread-state：
  - 本轮在交付问询后已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-05｜补记：已产出三份“情况说明型 prompt”，正式把导演线 / NPC / 典狱长的工作面拆开

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守 Day1 own；
  - 本轮子任务不是继续写剧情代码，而是把当前整体状态、Town 边界、已分出的工作面，收成 3 份可直接转发 / 继续使用的说明型 prompt。
- 本轮实际完成：
  1. 已新增给典狱长的全景说明：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-05_给典狱长_spring-day1与Town导演协同全景说明_06.md`
     - 内容重点是：
       - 当前导演线已经做到哪
       - `UI / NPC / Town 外线` 分别已分出去什么
       - 典狱长继续往下最值得盯的，仍是 `Town` 的总治理与剩余 blocker，而不是替我接盘导演线
  2. 已新增给 `NPC-v2` 的协同说明：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_给NPC_v2_Day1导演协同与Town群像承接说明_07.md`
     - 内容重点是：
       - `EnterVillage` post-entry crowd
       - `DinnerConflict` 背景层
       - `NightWitness_01`
       - `DailyStand_01~03`
       - 这些群像层 / 背景层 / 观察层现在已经正式分给 NPC 线去承接
       - 但不让它回吞导演 phase、Town scene 本体、runtime 切场和精确 nav 路径
  3. 已新增给我自己的自续工说明：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_导演线自续工全景说明与分工边界_09.md`
     - 内容重点是：
       - 哪些段落继续留在临时承载
       - 哪些段落现在开始按 `Town` 分场
       - 当前该先写“导演脚本级分场与调度”，而不是继续漂去 UI / Town scene / runtime 精确路径
- 当前稳定结论：
  - 从这轮开始，`spring-day1` / `NPC` / `典狱长` 三方不再只靠聊天记忆协作；
  - 三条线的工作面已经被明确写成：
    1. `spring-day1`：分场、phase 真值、导演层承接
    2. `NPC`：群像层、背景层、观察层、夜间见闻层、站位层
    3. `典狱长`：Town 总治理与剩余 blocker 推进
- 当前验证状态：
  - `文档级协同拆分成立`
  - 本轮未改 runtime 代码、未进 Unity、未跑新的 targeted tests。
- thread-state：
  - 本轮产出 3 份说明型 prompt 后已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
- 当前恢复点：
  - 等用户转发给典狱长 / NPC；
  - 我这边下一步就不再回到“要不要分工”的阶段，而是直接按 `自续工说明_09` 继续导演分场与 Town 承接写作。

## 2026-04-05｜补记：已把导演线真正推进到 `Town` 可消费的最深文档层，新增 10/11/12 三份正文

- 当前子工作区主线没有换：
  - `spring-day1` 这轮没有回漂去 UI、代码修红或 Town scene 本体；
  - 唯一主刀就是把 `EnterVillage post-entry -> DayEnd` 的导演分场、群像矩阵和后续 runtime 落位边界，真正写成可承接的正文。
- 本轮实际完成：
  1. 新增导演分场正文：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_导演分场与Town承接脚本_10.md`
     - 已把以下段落写成导演层分场：
       - `EnterVillage` post-entry
       - `DinnerConflict` 背景层
       - `ReturnAndReminder` 回村安静层
       - `FreeTime` 夜间见闻层
       - `DayEnd` 夜间收束与次日预示
     - 并明确了每场的：
       - 主锚点
       - 主戏人物
       - 背景层职责
       - 玩家感知目标
       - 当前故意不写死的 runtime 项
  2. 新增 NPC 群像矩阵正文：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_NPC剧本走位与群像层矩阵_11.md`
     - 已把以下内容写清：
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `DinnerBackgroundRoot`
       - `NightWitness_01`
       - `DailyStand_01~03`
     - 对每个锚点都拆出了：
       - 适合的角色类型
       - 不适合的角色类型
       - 朝向/动作
       - 说话强度
       - 必须让位给谁
     - 同时明确：
       - Day1 正式主戏角色仍是 `马库斯 / 艾拉 / 卡尔`
       - `101~301` 继续只按 crowd 外壳使用，不 claim 正式原案人设
  3. 新增阶段边界与后续落位清单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_阶段承载边界与后续runtime落位清单_12.md`
     - 已把 10 个 phase 的承载方式正式拆成：
       - 当前承载方式
       - 当前导演可写深度
       - 当前不落 runtime 的原因
       - `Town sync-ready` 后的 runtime 落位
     - 并给出了后续最合理的 runtime 落位优先顺序：
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `DinnerBackgroundRoot`
       - `NightWitness_01`
       - `DailyStand_01~03`
- 当前稳定结论：
  1. 导演线现在已经不再停留在“Town 能不能用”的问边界阶段，而是已经把 `Town` 可消费的导演层内容写到了最深文档层。
  2. 当前还能继续抽象推进的空白已经很少；再往下就不再是“继续想清楚”，而会变成：
     - `NPC` 去接群像 runtime 承接
     - `Town` 去接场地 runtime 承接
     - 或导演线回到具体 phase 文本 / 正式剧情资产化
  3. 当前最重要的硬边界也已经彻底写实：
     - `CrashAndMeet / EnterVillage` 前半段 / `HealingAndHP / WorkbenchFlashback / FarmingTutorial` 不迁进 `Town`
     - `EnterVillage` 后半段、`DinnerConflict` 背景层、`FreeTime / DayEnd` 夜间层开始按 `Town` 写
     - 但不假装 `Town` 已具备完整 runtime 闭环
- 当前验证状态：
  - `文档级自检成立`
  - 本轮未改 runtime 代码、未进 Unity、未跑新的编译/PlayMode 验证
  - 当前属于导演层与协同层真值落盘，不属于运行态验证轮。
- 当前恢复点：
  - 后续若继续 `spring-day1`，不再需要重新解释：
    - `Town` 能不能开始消费
    - 哪些 phase 可按 `Town` 写
    - 群像层该由谁承接
  - 可直接从两条路里选一条继续：
    1. 回到具体 phase 文本与正式剧情资产化
    2. 等 `NPC / Town` 各自把 runtime 承接能力推进后，再接下一层导演落地

## 2026-04-05｜补记：已完成对 NPC 与典狱长的二次同步，明确这轮 10/11/12 正文该通知谁

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守导演线；
  - 本轮子任务不是再做导演正文，而是把刚落下来的 `10/11/12` 三份正文同步给真正需要的人。
- 本轮实际完成：
  1. 已新增给 `NPC-v2` 的二次同步文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-05_给NPC_v2_导演正文同步与后续承接提示_08.md`
     - 重点说明：
       - 导演线已经新增 `10/11/12`
       - `NPC` 现在拿到的不只是边界，而是更细的群像矩阵与 phase 承载边界
       - `NPC` 后续最该继续承接的是：
         - 进村围观层
         - 晚餐背景层
         - 夜间见闻层
         - 次日站位层
  2. 已新增给典狱长的二次同步文件：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-05_给典狱长_导演正文同步与Town后续承接提示_07.md`
     - 重点说明：
       - `spring-day1` 现在不是只拿到边界，而是已经把 Town 消费面写成正式正文
       - 典狱长后续最值钱的继续推进点，不再是重裁边界，而是：
         - Town 各 anchor 的可承接等级
         - Town 何时升级到更接近 runtime 消费
         - 外线 blocker 如何继续压
  3. 当前已明确“谁不用通知”：
     - `UI` 这轮不需要同步
     - 原因：`10/11/12` 属于导演层 / 群像层 / Town 承接边界，不是 UI 当前主线依赖
- 当前稳定结论：
  - 这轮新增的 3 份导演正文现在已经同步给：
    1. `NPC`
    2. 典狱长
  - 不需要再扩散给别的线程。
- 当前恢复点：
  - 你后续若继续转发，只需要转这两份新同步文件；
  - `spring-day1` 自己下一步可以回到导演线本体继续做正式剧情资产化，不再停在“该通知谁”阶段。

## 2026-04-05｜补记：已新增导演线自己的后续执行底板，正式把“未完成剧情 + 轻量导演工具”并成一张任务清单

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守导演线；
  - 本轮子任务不是继续写剧情正文，也不是直接开做工具，而是先把后续迭代要严格遵守的执行底板写成正式文档。
- 本轮实际完成：
  1. 已新增：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_spring-day1_导演线后续任务清单与轻量导演工具路线图_13.md`
  2. 该文档已把两条并行线正式收成统一清单：
     - `A 组`：Day1 导演线未完成的正式内容
       - `CrashAndMeet`
       - `EnterVillage`
       - `HealingAndHP / WorkbenchFlashback / FarmingTutorial`
       - `DinnerConflict / ReturnAndReminder / FreeTime / DayEnd`
     - `B 组`：轻量导演工具 MVP
       - beat 数据化
       - 单 NPC 手动排练记录
       - 路径点/站位点回放
       - 剧情段挂接
       - 最小编辑器入口
  3. 文档中已明确写死当前止损线：
     - 不做完整版导演编辑器
     - 不做多 NPC 同时录制
     - 不做复杂时间线/事件轨道
     - 不为了工具反过来拖死 Day1 本体
  4. 文档中已明确后续最推荐的执行顺序：
     - 先继续压剧情本体
     - 再并行补最小导演工具数据层
     - 第一批优先吃工具的场景只限：
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `NightWitness_01`
       - `DailyStand_01`
- 当前稳定结论：
  - 从这份 `13` 号文档开始，后续 `spring-day1` 继续时不再按“想到哪做哪”推进；
  - 而是正式按：
    1. 未完成剧情清单
    2. 轻量导演工具 MVP
    3. 止损线与验收分层
    这 3 层一起推进。
- 当前验证状态：
  - `文档级执行底板成立`
  - 本轮未改 runtime 代码、未进 Unity、未做工具实现
- 当前恢复点：
  - 你审核完 `13` 号文档后；
  - 下一轮我就直接按这份清单往下做，不再重复起方案。

## 2026-04-05｜补记：单 NPC 导演录制 / 回放 / 挂接 Day1 beat 只读架构判点

- 当前子工作区主线没有换：
  - `spring-day1` 仍只守导演线；
  - 本轮子任务是基于现有 `SpringDay1NpcCrowdManifest / SpringDay1NpcCrowdDirector / SpringDay1Director / DialogueSequenceSO`，判断“单 NPC 导演录制 / 回放 / 挂接 Day1 beat”最稳该落在哪一层。
- 本轮实际完成：
  1. 已确认最佳接点不在 `DialogueSequenceSO`，也不应把 beat 编排直接塞进 `SpringDay1NpcCrowdManifest`。
  2. 当前最稳方案是：
     - `SpringDay1Director` 负责 `何时触发哪个 Day1 beat`
     - `SpringDay1NpcCrowdDirector` 负责 `对哪个 npcId 执行哪段回放`
     - `SpringDay1NpcCrowdManifest` 继续只守 `谁 / 默认在哪 / 默认朝向 / 哪些 phase 可出现`
  3. 建议新增的最小数据层为：
     - `SpringDay1NpcPerformanceClipSO`
     - `SpringDay1NpcPerformanceFrame`
     - `SpringDay1NpcBeatBindingSO`
     - 可选 editor-only `SpringDay1NpcPerformanceRecorder`
  4. 推荐的最小运行链路为：
     - `SpringDay1Director` 在 `HandleDialogueSequenceCompleted / BeginDinnerConflict / EnterFreeTime` 这类已有 beat 边界处发起 beat 请求
     - `SpringDay1NpcCrowdDirector` 解析 binding，锁定目标 NPC，短暂接管 `NPCAutoRoamController / NPCMotionController` 做单 NPC 回放
     - 回放结束后恢复 roam / facing / home anchor 基线
  5. 备选方案是：
     - 在 `SpringDay1NpcCrowdManifest.Entry` 内追加 beat cue 列表
     - 由 `SpringDay1NpcCrowdDirector` 直接监听 `StoryPhaseChangedEvent / DialogueSequenceCompletedEvent` 自动触发
     - 但这会把“静态出生清单”和“Day1 编排”缠在同一个 asset 里，后续更容易失控
- 当前稳定结论：
  - 单 NPC 导演最稳的挂接层是 `SpringDay1Director -> SpringDay1NpcCrowdDirector` 这一层间接口；
  - 不是 `DialogueSequenceSO`，也不是把全部 beat 逻辑压进 `Manifest`。
- 当前风险与边界：
  - 这轮只是静态代码分析，未进 Unity，未做 live 验证；
  - “录制”如果要做真录制，建议放 editor-only，不要把录制器本体塞进 runtime 导演链。
- 当前恢复点：
  - 若下一轮进入真实施工，第一刀应先做：
    - `SpringDay1NpcPerformanceClipSO + BeatBindingSO`
    - `SpringDay1NpcCrowdDirector` 单 NPC 回放接口
    - `SpringDay1Director` 先只挂 1 个 beat 做贯通验证；
  - 不要第一刀就做多 NPC、多轨道或 UI 联动。

## 2026-04-05｜续工落地：A组正式剧情 + B组轻量导演工具 MVP 已真实下刀

- 当前子工作区主线没有换：
  - 仍然只做 `spring-day1` 导演线；
  - 本轮按 `13` 号任务单并行推进两条：
    1. `A组` Day1 正式剧情内容继续压实
    2. `B组` 轻量导演工具 MVP 真实落地
- 本轮实际完成：
  1. 已新增 runtime 导演数据层：
     - `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
     - 内容包括：
       - beat key 常量
       - stage book / beat / cue / path point 数据结构
       - `Resources` JSON 装载
       - 单 NPC cue 回放组件
       - 单 NPC `WASD / 方向键` 排练组件
  2. 已把导演 beat 真正接进 Day1 主线：
     - `SpringDay1Director.cs` 新增 `GetCurrentBeatKey()`
     - 当前已把：
       - `CrashAndMeet_Awake / Escape`
       - `EnterVillage_PostEntry / HouseArrival`
       - `HealingAndHP_Treatment`
       - `WorkbenchFlashback_Recall`
       - `FarmingTutorial_Fieldwork`
       - `DinnerConflict_Table`
       - `ReturnAndReminder_WalkBack`
       - `FreeTime_NightWitness`
       - `DayEnd_Settle / DailyStand_Preview`
       映射成可直接消费的 beat 真值
  3. 已把导演 cue 真正接进 crowd runtime：
     - `SpringDay1NpcCrowdDirector.cs`
     - `SpringDay1NpcCrowdManifest.cs`
     - 当前逻辑已支持：
       - 根据当前 beat 解析 cue
       - 对匹配 `npcId / semanticAnchorId / duty` 的 crowd 进行单 NPC 接管
       - 暂停 roam
       - 应用起点、朝向、路径点、停顿
       - beat 退出后恢复默认 home anchor / facing / roam
  4. 已新增最小 editor 入口：
     - `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`
     - 当前窗口已支持：
       - 选择 beat
       - 编辑正式摘要 / 资产化钩子
       - 选择 / 新增 / 删除 cue
       - 记录起点
       - 追加路径点
       - 保存 JSON
       - 在 Play Mode 下给选中 NPC 挂排练器
       - 把当前 cue 手动回放到选中 NPC
  5. 已新增初始 stage book JSON：
     - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
     - 当前已把 opening -> day end 的导演 beat 摘要和资产化钩子落成结构化文本；
     - 并已先接入：
       - `EnterVillage_PostEntry`
       - `FreeTime_NightWitness`
       - `DailyStand_Preview`
     - 其中优先吃工具的真实锚点包括：
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `NightWitness_01`
  6. 已把 `A组` 正式文本继续压深到现有对白资产：
     - `SpringDay1_FirstDialogue.asset`
     - `SpringDay1_FirstDialogue_Followup.asset`
     - `SpringDay1_VillageGate.asset`
     - `SpringDay1_HouseArrival.asset`
     - `SpringDay1_ReturnReminder.asset`
     - `SpringDay1_FreeTimeOpening.asset`
     - 这轮补强的重点是：
       - 矿洞口醒来时的敌我判断
       - 怪物逼近与撤离的紧迫感
       - 进村围观的压力感与让位感
       - 闲置小屋“先安置、未接纳”的语义
       - 夜里两点规则不只是嘴上吓唬
       - 自由时段里“今晚先容下你，明天再看你值不值得留下”的村庄气质
  7. 已新增最小 targeted 测试：
     - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 当前已覆盖：
       - stage book cue 解析
       - 单 NPC 起点接管
       - `Director` 的 `FreeTime / DayEnd` beat 暴露
- 当前验证状态：
  - 代码层脚本校验：
    - `SpringDay1DirectorStaging.cs`：`warning-only`
    - `SpringDay1Director.cs`：`warning-only`
    - `SpringDay1NpcCrowdDirector.cs`：`warning-only`
    - `SpringDay1NpcCrowdManifest.cs`：`pass`
    - `SpringDay1DirectorStagingWindow.cs`：`pass`
    - `SpringDay1DirectorStagingTests.cs`：`pass`
  - Unity fresh console：
    - 本轮 compile 后未读到新的 `Error`
    - 当前能看到的主要是既有运行日志与 MCP/菜单日志
  - targeted tests：
    - `SpringDay1DirectorStagingTests`：`3/3 pass`
    - `SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_PreserveOpeningSemantics`：`pass`
    - `SpringDay1OpeningRuntimeBridgeTests.HouseArrivalCompletion_ShouldBridgeIntoHealingAndHp`：`pass`
    - `SpringDay1MiddayDialogueAssetGraphTests.MiddayDialogueAssets_ShouldPreserveLaterDaySemantics`：`pass`
    - `SpringDay1MiddayRuntimeBridgeTests.Director_ShouldPreferAuthoredDialogueAssetsForMiddayPhases`：`pass`
  - 额外说明：
    - 通过菜单或连续跑指定用例时，Unity 会偶发出现“Editor 正在或即将进入 Play Mode，测试无法启动”的工具链噪音；
    - 这轮已通过重新停回 Edit Mode 的方式绕开，当前不把它判成 Day1 业务 blocker
- 当前稳定结论：
  - `B组` 已经不再是“只做骨架”：
    - 数据化
    - 最小编辑入口
    - 单 NPC 排练
    - 保存 JSON
    - 回放
    - Day1 真实 beat 挂接
    都已经落地
  - `A组` 这轮也不是只写思路：
    - 已把 opening / enter-village / reminder / free-time 的正式文本层继续压进真实对白资产
    - 并把全段 beat 摘要结构化进 stage book
- 当前仍未完成：
  1. 还没有做多人同录、时间线、多轨道，这一轮故意不做
  2. `DinnerBackgroundRoot` 尚未吃进复杂多人层，只保留了摘要与后续钩子
  3. 还没给 `FreeTime / DayEnd` 做更深的 live 排练证据，目前只做到 stage cue + 单 NPC 链路成立
- 当前恢复点：
  - 下一轮若继续本线，最值钱的下一刀应直接做：
    1. 用导演窗口在 live 里真实排练 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01`
    2. 把 `DinnerBackgroundRoot` 的多人背景层按当前 MVP 再吃一层
    3. 再补 1~2 条 `DayEnd / DailyStand` 的 live 回放证据

## 2026-04-05｜续工补记：导演排练全接管、最小录制写回、后半段 Town cue 再压深

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线；
  - 这轮继续同时推进：
    1. `A组` Day1 正式剧情/导演消费层
    2. `B组` 轻量导演工具 MVP
- 本轮子任务：
  - 先把“排练时完全接管 NPC、不要把玩家一起带走”落到真实代码；
  - 再把 `ReturnAndReminder / DayEnd` 的后半段 Town 可消费 cue 继续压进 stage book；
  - 同时补最小验证，不回 UI、不碰 `Primary.unity / Town.unity / GameInputManager.cs / NPC own`。
- 这轮真实做成了什么：
  1. 导演工具接管链继续加深：
     - `SpringDay1DirectorPlayerRehearsalLock` 已新增；
     - 排练开始时会一起锁住 `PlayerMovement / PlayerController / PlayerAutoNavigator / PlayerInteraction`；
     - 停止排练时恢复原状态，避免玩家和排练中的 NPC 一起走。
  2. `SpringDay1DirectorStagingPlayback` 已补“同 cue 不重复 Apply”的护栏：
     - 同一 `beat + cue` 不再每次 crowd sync 都把 NPC 拉回起点；
     - 换 cue 时也会先释放旧 takeover，再接入新 cue，避免 lockDepth 累加漂移。
  3. 导演窗口继续从“摆位器”推进成最小录制器：
     - 新增 `开始录制 Cue / 停止录制写回 Cue`；
     - 录制时会按采样间隔与最小位移自动记录 runtime 位置/朝向；
     - 停止录制后直接写回当前 cue 的 `startPosition / facing / path`；
     - 同时补了“一次只接管一只 NPC”的收口，切换排练对象会先停掉上一只。
  4. 后半段导演消费再压深：
     - `ReturnAndReminder_WalkBack` 已补最小 `DinnerBackgroundRoot` cue；
     - `DayEnd_Settle` 已补最小 `NightWitness_01` cue；
     - 当前 stage book 已不只覆盖 `EnterVillage / Dinner / FreeTime / DailyStand`，也把回屋提醒和第一夜收束继续吃进去了。
  5. 验证层新增与补强：
     - `SpringDay1DirectorStagingTests` 新增：
       - `StagingPlayback_ReapplyingSameCue_ShouldNotSnapNpcBackToStart`
       - `PlayerRehearsalLock_ShouldDisablePlayerMotionUntilRelease`
     - `SpringDay1TargetedEditModeTestMenu.cs` 已补导演 staging tests 菜单入口代码；
     - 命令桥当轮未识别新菜单项，`Editor.log` 已读到明确原因：`there is no menu named 'Sunset/Story/Validation/Run Director Staging Tests'`，因此这轮不把它包装成“测试失败”，而是判成菜单注册层未 fresh 接上。
- 这轮验证结果：
  - `git diff --check`：
    - `SpringDay1DirectorStaging.cs`
    - `SpringDay1DirectorStagingWindow.cs`
    - `SpringDay1NpcCrowdDirector.cs`
    - `SpringDay1DirectorStagingTests.cs`
    - `SpringDay1TargetedEditModeTestMenu.cs`
    - `SpringDay1DirectorStageBook.json`
    全部 clean
  - `python scripts/sunset_mcp.py status`：
    - compile baseline = `pass`
    - fresh console 一度回到 `0 error / 0 warning`
  - 目标化菜单验证：
    - `Run Midday Graph Tests`：`3/3 pass`
    - `Run Midday Bridge Tests`：`8/8 pass`
  - 当前 console 尾部仍有测试框架/字体导入副产物：
    - `PersistentManagers` 的 `DontDestroyOnLoad` editor-test 噪音
    - `DialogueChinese Pixel SDF.asset` 的 `Importer generated inconsistent result`
    - 它们出现在 edit-mode test 过程里，本轮不把它们判成导演线 own blocker
- 当前稳定结论：
  - 轻量导演工具已经从“可摆位”继续推进到：
    - 单 NPC 完全接管
    - 玩家侧临时冻结
    - 最小自动录制
    - 写回 cue
    - runtime 再消费不重复重置
  - 后半段 Day1 导演消费也已经从摘要层继续压到：
    - `ReturnAndReminder`
    - `DayEnd`
    的真实 anchor cue 层
- 当前还没做成什么：
  1. `Run Director Staging Tests` 菜单入口还没拿到命令桥 fresh 识别证据；
  2. 还没拿到 live 里对 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01` 的人工排练保存证据；
  3. `DinnerBackgroundRoot` 还没推进到复杂多人层与更细的围观关系。
- 当前恢复点：
  - 下一轮若继续导演线，优先顺序应直接是：
    1. 先在 live 里用导演窗口排练并保存 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01`
    2. 再确认新 `Director Staging Tests` 菜单入口何时被 Unity fresh 注册
    3. 最后把 `DinnerBackgroundRoot` 再吃深一层

## 2026-04-05｜续工补记：Primary live capture 已打通，导演测试 fresh 7/7 通过

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线真实施工；
  - 这轮继续同时推进：
    1. `A组` Day1 后半段导演消费继续下沉
    2. `B组` 轻量导演工具 MVP 继续打穿到真实可用
- 本轮子任务：
  - 不再围着 `Town` 空锚点空转，直接转吃 `Primary` 代理现场；
  - 把 `Primary live capture -> 写回 cue -> 导演测试护栏` 收成可重复验证的真实链路；
  - 同时确认 `Director Staging Tests` 是否已真正 fresh 覆盖新增护栏。
- 这轮真实做成了什么：
  1. `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` 已作为导演线 own 工具接入：
     - 菜单：`Sunset/Story/Validation/Run Director Primary Live Capture`
     - 真实从 `Primary` 代理锚点读取 `101/103/203/104/201/202/102/301` 等关键 NPC 的当前落位；
     - 成功把 `EnterVillage_PostEntry / DinnerConflict_Table / FreeTime_NightWitness / DailyStand_Preview` 共 `14` 条关键 cue 写成绝对落位；
     - 命令桥结果文件：`Library/CodexEditorCommands/spring-day1-director-primary-live-capture.json`
     - 本轮 live 结果：`completed`
  2. 导演测试已补“关键 cue 不再是 offset 空壳”的护栏：
     - `SpringDay1DirectorStagingTests.StageBook_ShouldContainCapturedAbsolutePositionsForKeyDirectorCues`
     - 通过 `SpringDay1DirectorStagingDatabase.Load(forceReload: true)` 读真实 stage book
     - 断言 `enter-crowd-101 / dinner-bg-203 / night-witness-102 / daily-201`
       - `keepCurrentSpawnPosition = false`
       - `pathPointsAreOffsets = false`
       - `startPosition` 已离开原点，说明 live capture 真写进了导演数据
  3. `SpringDay1TargetedEditModeTestMenu.cs` 已把新护栏接进 `Run Director Staging Tests`
     - 这次不再停在旧的 `6/6`
     - fresh 结果已到 `7/7 PASS`
     - 结果文件：`Library/CodexEditorCommands/spring-day1-director-staging-tests.json`
     - Unity XML 结果：`C:\\Users\\aTo\\AppData\\LocalLow\\DefaultCompany\\Sunset\\TestResults.xml`
  4. 当前第一真实 blocker 没有漂：
     - `Town` 的 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01 / DinnerBackgroundRoot / DailyStand_01`
       仍然都是空 `Transform`，世界位置压在原点；
     - 所以现在可以继续把 `Town` 当导演承接层写，但还不能包装成“最终 runtime 可直接排练的真实 anchor”。
- 这轮验证结果：
  - `python scripts/sunset_mcp.py status`
    - baseline = `pass`
    - Unity 当前在 `Primary.unity`
    - `Edit Mode`
    - `0 error`
    - warning 仅剩测试框架副产物
  - `python scripts/sunset_mcp.py manage_script validate`
    - `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` = `clean`
    - `SpringDay1DirectorStagingTests.cs` = `clean`
    - `SpringDay1TargetedEditModeTestMenu.cs` = `clean`
  - `Run Director Primary Live Capture`
    - `completed`
    - 成功写回 `14` 条关键 cue
  - `Run Director Staging Tests`
    - `7/7 PASS`
  - `git diff --check`
    - 当前这轮 own 文件 clean
- 当前还没做成什么：
  1. 没去碰 `Town.unity` 真锚点，因为用户明确禁止；
  2. 没继续扩到复杂多人 `DinnerBackgroundRoot` 排练；
  3. 还没拿到导演窗口里“人工 WASD 排练并保存三处锚点”的真人手工轨迹证据。
- 当前恢复点：
  - 下一轮若继续本线，最值钱的下一刀应直接是：
    1. 用导演窗口在 live 里手工排练并保存 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01`
    2. 在不碰 `Town.unity` 的前提下，再把 `DinnerBackgroundRoot` 推深一层
    3. 等 `Town` 真实锚点落位后，再把代理 `Primary` cue 迁回 `Town` runtime contract

## 2026-04-05｜续工补记：已给 NPC 线补发最新并行续工入口

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线；
  - 本轮只是把并行协作的 `NPC` 入口重新钉死。
- 本轮新增 prompt：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-05_NPC线程_day1后半段群像内容并行续工prompt_03.md`
- 这份 prompt 锁死了 `NPC` 现在真正该做的事：
  1. 不再补底座
  2. 不回吞导演工具 / Town runtime / UI
  3. 只收后半段会被导演线消费的群像内容层：
     - `EnterVillage_PostEntry`
     - `DinnerConflict / ReturnAndReminder`
     - `FreeTime / DayEnd / DailyStand`
- 当前恢复点：
  - 后续如果继续并行推进，`NPC` 线应按 `03` 号 prompt 继续，不再沿旧聊天自行解释边界。

## 2026-04-05｜续工补记：三线分发后，spring-day1 自己仍是导演总整合位

- 当前主线没有换：
  - `spring-day1` 自己不是纯分发者；
  - 分发完 `UI / NPC / Town` 后，仍然是 Day1 导演线 owner 与最终整合位。
- 当前 own 下一步被锁成：
  1. 用导演窗口手工排练并保存
     - `EnterVillageCrowdRoot`
     - `KidLook_01`
     - `NightWitness_01`
  2. 把 `DinnerBackgroundRoot` 往复杂多人层再推进一刀
  3. 等 `Town` 最小 runtime contract 落地后，优先把当前 `Primary` 代理承接迁回真实 `Town` runtime
  4. 再把 `NPC` 回来的群像内容继续吃进导演消费层
- 当前恢复点：
  - 后续若用户追问“你自己做到哪里”，统一按上面这条回答，不再把自己误说成只负责分发 prompt。

## 2026-04-06｜导演线续工：排练 bake 工具落地，三处关键锚点与 DinnerBackgroundRoot 已写回

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线 owner；
  - 本轮只做两件事：
    1. 把导演排练工具真正做成可写回的 MVP
    2. 把 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01` 与 `DinnerBackgroundRoot` 的 cue 真正压深
- 本轮真实落地：
  - 新增工具：
    - `Assets/Editor/Story/SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
    - 支持通过稳定旧入口 `Run Director Primary Live Capture` + signal 切到 bake 分支
    - 优先尝试 play 触发；如果 shared Editor 的 play 切换卡住，就自动走 `edit-mode fallback bake`
  - 已更新：
    - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
    - `EnterVillage_PostEntry`
      - `enter-crowd-101` => `pathPointCount=3`
      - `enter-kid-103` => `pathPointCount=3`
    - `FreeTime_NightWitness`
      - `night-witness-102` => `pathPointCount=3`
      - `night-witness-301` => `pathPointCount=3`
    - `DinnerConflict_Table`
      - `dinner-bg-203/104/201/202` => 全部 `pathPointCount=4`
  - 测试护栏已补：
    - `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
      - 新增 `StageBook_ShouldContainMultiPointPathsForRehearsedDirectorTargets`
    - `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
      - `Run Director Staging Tests` 扩到 `8/8`
- 本轮验证：
  - `Library/CodexEditorCommands/spring-day1-director-primary-rehearsal-bake.json`
    - `status=completed`
    - `message=导演 rehearsal bake 已通过 edit-mode fallback 完成，写回 8 条 cue`
  - `Library/CodexEditorCommands/spring-day1-director-staging-tests.json`
    - `status=completed`
    - `passCount=8`
    - `failCount=0`
  - `python scripts/sunset_mcp.py errors --count 60 --output-limit 20`
    - `errors=0 warnings=0`
- 本轮最关键判断：
  - 当前 shared Editor 的 play 切换并不稳定，继续死磕“必须拿到纯 play 录制”只会拖住主线；
  - 所以导演工具必须自带 fallback，先保证 cue 数据真正可写回、可验证、可被当前导演链消费。
- 当前还没做成什么：
  1. 还没拿到“纯 play-mode 手工 WASD 录制”的干净证据
  2. 还没把这些代理结果迁回 `Town runtime contract`
  3. 还没把 `NPC` 群像内容层与 `UI` 玩家面最新结果重新并回 Day1 主链
- 当前恢复点：
  - 下一轮继续时，顺序固定为：
    1. 继续补 `Town runtime contract`
    2. 再把当前代理排练结果迁回 `Town`
    3. 最后做 `NPC/UI -> Day1` 总整合
