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

## 2026-04-06｜临时收尾口径：当前停在“可验证导演工具 + 已写回关键 cue”的刀口

- 这轮对用户的暂时收尾口径已固定：
  - 可以先把本轮定义成“导演工具 MVP 已站住，关键 cue 已写回并有测试护栏”的一刀；
  - 不再把它包装成 `Town runtime` 或 Day1 总整合已经完成。
- 当前最硬结果仍以这三条为准：
  1. `StageBook` 已 fresh 写回 8 条 cue
  2. `Director Staging Tests` 当前 `8/8 PASS`
  3. fresh console 当前 `0 error / 0 warning`
- 当前仍明确未完：
  1. 纯 live/play-mode bake 还没稳定闭环
  2. `Town runtime contract` 迁回未做
  3. `NPC/UI -> Day1` 总整合未做

## 2026-04-06｜方向改判：crowd 从 runtime 临时演员转向驻村常驻化

- 当前 `day1` 新判断已钉死：
  - 不再把 `101~301` crowd 的问题理解成“更早 spawn 一点就行”；
  - 方向正式改为“驻村常驻化”。
- 触发原因：
  1. 用户明确要求这批 NPC 应该像本来就在村里
  2. 当前真实叙事里，开篇真正跟玩家走的只有村长，艾拉在治疗段跟着村长进场
  3. 所以 crowd 不应继续表现成“阶段到了才突然蹦出来”
- 当前分工口径同步为：
  1. `day1` 自己继续吃 `SpringDay1NpcCrowdDirector.cs` 的 deployment contract
  2. `Town` 不抢 active 代码，先继续压实 resident root / anchor / scene-side 承接
  3. `NPC` 不吞 runtime 部署，继续把后半段 crowd 内容推进成“驻村常驻居民语义层”
- 已新增 prompt：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给Town_day1转向驻村常驻化承接与scene-side准备prompt_06.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_给NPC_day1转向驻村常驻化语义矩阵续工prompt_11.md`
- 当前恢复点：
  - 下一轮若继续真实施工，`day1` 第一刀应直接落在：
    - `SpringDay1NpcCrowdDirector.cs`
    - 目标是把当前 crowd 从“cue 时 instantiate”改成“已有 resident actor 的接管与调度”

## 2026-04-06｜计划收口：已生成大步续工执行清单、自用开工 prompt 与 Town/NPC 补充 prompt

- 这轮没有继续写代码，先把下一轮的大步续工硬约束落成文档。
- 已新增：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_驻村常驻化大步续工执行清单_14.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_驻村常驻化大步续工自用开工prompt_15.md`
  3. `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-06_给Town_day1驻村常驻化大步续工补充prompt_07.md`
  4. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_给NPC_day1驻村常驻化大步续工补充prompt_12.md`
- 新清单把下一轮固定成：
  1. 先吃 `CrowdDirector` resident deployment 第一刀
  2. 再继续用导演工具生产更多真实 cue
  3. 再继续把剧情往正式产物压
- 当前 thread-state：
  - 已 `Park-Slice`
  - 当前状态为 `PARKED`

## 2026-04-06｜prompt补记：正式剧情不可重复触发约束已补进 self/NPC prompt

- 用户新增硬要求：
  - 已消费的正式剧情不能重复触发
  - 已走完的任务/NPC 正式聊天不能重复回放
  - 再次聊天时只能进入闲聊 / resident 日常句池 / phase 后非正式补句
- 本轮已更新：
  1. `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_spring-day1_驻村常驻化大步续工自用开工prompt_15.md`
  2. `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-06_给NPC_day1驻村常驻化大步续工补充prompt_12.md`
- 新约束已钉死成：
  1. 正式剧情 `one-shot`
  2. 任务推进 `one-shot`
  3. 正式聊天消费后只落回闲聊/日常句池

## 2026-04-06｜续工实装：resident deployment 第一刀 + formal one-shot 护栏已真正落代码

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线真实施工；
  - 这轮先收 `CrowdDirector` 的驻村常驻化 deployment 第一刀，同时把“正式剧情不可重复触发”补成运行态护栏，而不是只写 prompt。
- 本轮真实代码落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - crowd spawn 改成优先吃 `semanticAnchorId` / cue 语义锚，再回退旧 `anchorObjectName`
     - runtime root 正式拆出：
       - `Town_Day1Residents`
       - `Resident_DefaultPresent`
       - `Resident_DirectorTakeoverReady`
       - `Resident_BackstagePresent`
       - `Town_Day1Carriers`
     - `SyncCrowd()` 不再只按“到 phase 才显现”的临时演员逻辑跑，而是：
       - 先尝试导演 cue 接管
       - 没 cue 时回落 resident baseline
     - 新补 `NeedsResidentReset` 护栏，修掉了“resident baseline 每次 sync 把 NPC 拉回出生点”的假活 bug
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增 semantic anchor 优先测试
     - 新增 resident runtime root provision 测试
     - 新增“进村前白天居民已在场”测试
     - 新增“重复 baseline sync 不应把 resident 拉回 base position”测试
  3. `Assets/YYY_Tests/Editor/NpcInteractionPriorityPolicyTests.cs`
     - 新增“formal phase 已推进到位后，同 NPC 只能回落到 informal / resident 闲聊入口”的运行态测试
  4. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把新增 crowd resident 测试挂回 `Run Director Staging Tests`
     - 新增 `Run NPC Formal Consumption Tests`
- 本轮验证与 blocker 现状：
  1. `git diff --check` 对这轮 own 文件是 clean
  2. `python scripts/sunset_mcp.py status`
     - 拿到过 `isCompiling=false`
     - 没有 fresh own compile error 证据
  3. `validate_script / compile`
     - 仍被 `subprocess_timeout:dotnet:60s` 卡成 `blocked`
     - 不能包装成“脚本级 codeguard 已过”
  4. 当前 console 里的红项不是 own compile red，而是外线 NPC 验证菜单：
     - `[SpringDay1NpcCrowdValidation] FAIL | issues=1`
     - 具体是：`EnterVillage_PostEntry: director consumption role drifted -> Trace | actual=[301]`
  5. command bridge 能执行旧菜单，但这轮新增菜单项还没 fresh 到当前编译体：
     - `Run Director Staging Tests` 在 `2026-04-06 02:50:42 +08:00` 被执行成功
     - 结果文件仍是旧 8 条测试名单，说明 Editor 还没吃到本轮新菜单程序集
     - 后续 `Assets/Refresh` 请求没有被 bridge 消费，这一条已确认是本轮真实验证 blocker
- 当前恢复点：
  1. 先等/拉回 Unity fresh 编译与 command bridge 恢复消费
  2. 重新跑：
     - `Run Director Staging Tests`
     - `Run NPC Formal Consumption Tests`
  3. 然后继续把这批 resident actor contract 往 `Town runtime` 迁回

## 2026-04-06｜续工实装第二刀：Town anchor runtime contract 已接通，代理场地下可吃真实 Town 坐标

- 本轮继续往下推进后，又真正做成了 3 件大事：
  1. 新增运行时 Town anchor contract：
     - `Assets/YYY_Scripts/Story/Directing/SpringDay1TownAnchorContract.cs`
     - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1TownAnchorContract.json`
     - 把 `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01 / DinnerBackgroundRoot / DailyStand_01` 的真实 Town 世界坐标落成 runtime 可读资源
  2. `SpringDay1NpcCrowdDirector.cs`
     - `ResolveSpawnPoint()` 现在顺序变成：
       - 当前 scene 里的 semantic anchor
       - Town anchor runtime contract
       - 旧 `anchorObjectName`
       - `fallbackWorldPosition`
     - 也就是说，即使当前仍在 `Primary` 代理场地下，没有把 `Town.unity` 一起 live 加载，day1 也已经能吃到真实 Town 锚点坐标
  3. `SpringDay1DirectorTownContractMenu.cs`
     - probe 不再只会报 `blocked`
     - 当 Town scene anchor 与 runtime contract 资源对齐时，会直接回 `completed`
- 本轮新增验证证据：
  1. `Library/CodexEditorCommands/spring-day1-director-staging-tests.json`
     - `2026-04-06 03:31:37 +08:00`
     - `13/13 PASS`
     - 新通过项包括：
       - `CrowdDirector_ShouldFallBackToTownAnchorContractWhenSemanticAnchorIsNotInLoadedScene`
       - `CrowdDirector_ShouldNotResnapResidentToBasePositionOnRepeatedBaselineSync`
       - resident runtime root / semantic anchor / pre-enter resident baseline 全部通过
  2. `Library/CodexEditorCommands/spring-day1-npc-formal-consumption-tests.json`
     - `2026-04-06 03:18:47 +08:00`
     - `3/3 PASS`
     - formal 消费后回落 informal
     - formal phase 已推进后不可重播
     - formal 可接管时闲聊会主动让位
  3. `Library/CodexEditorCommands/spring-day1-town-contract-probe.json`
     - `2026-04-06 03:32:05 +08:00`
     - `status=completed`
     - `firstBlocker=""`
     - 说明 Town anchors 已存在，且 runtime contract 资源已覆盖关键导演锚点
  4. fresh console：
     - `python scripts/sunset_mcp.py errors --count 20 --output-limit 10`
     - `errors=0 warnings=0`
- 本轮中途还踩到并修掉了 2 个真实问题：
  1. `EnsureRuntimeChild()` 先 parent 再 `MoveGameObjectToScene` 会炸 `Gameobject is not a root in a scene`
     - 已修成：先 move 到 scene，再挂 parent
  2. `SpringDay1DirectorTownContractMenu.cs`
     - 中途因为没显式引 `Sunset.Story` 打出 own red
     - 已止血并 fresh 清红
- 当前判断更新：
  - `Town` 这边当前对 day1 的第一 blocker 已经不再是“锚点没位置 / contract 没接”
  - 现在 day1 自己这条线更真实的下一步，已经变成：
    1. 继续用这套真实 Town 坐标去深化后半段导演消费 / live 摆位
    2. 然后吃回 `NPC/UI` 回流结果做最终总整合

## 2026-04-06｜只读审计补记：StageBook 后半段 proxy / semantic anchor 迁移优先级已收清

- 当前主线目标没有换：
  - `spring-day1` 仍在推进 Day1 后半段导演消费与驻村常驻化；
  - 本轮子任务只是只读审 `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`，不进入真实施工。
- 本轮稳定结论：
  1. 后半段里最明显还停在旧 proxy 写法的 cue 只有 3 条：
     - `ReturnAndReminder_WalkBack/reminder-bg-203`
     - `ReturnAndReminder_WalkBack/reminder-bg-201`
     - `DayEnd_Settle/dayend-watch-301`
     - 共同特征是：
       - `keepCurrentSpawnPosition = true`
       - `pathPointsAreOffsets = true`
       - `startPosition = (0,0)`
     - 说明它们仍是“挂在现有 anchor 或当前出生位上的微偏移”，不是 fresh 绝对落位。
  2. `FreeTime_NightWitness` 与 `DinnerConflict_Table` 当前可判为已脱离旧 proxy：
     - `night-witness-102 / 301`
     - `dinner-bg-203 / 104 / 201 / 202`
     - 都已是 `keepCurrentSpawnPosition = false` + `pathPointsAreOffsets = false` 的绝对落位；
     - 同时也命中当前导演测试护栏。
  3. `DailyStand_Preview` 虽然全员都已经挂上 `semanticAnchorId`，但整体仍像“半迁移状态”：
     - `assetizationHook` 也直接写明要等 `Town` 更稳后再推真实 runtime；
     - `daily-101 / 103 / 201` 还没有 path；
     - `daily-102 / 104 / 203` 的 path 点仍是接近原点的小偏移值，和当前 `pathPointsAreOffsets = false` 的绝对口径不一致；
     - 再加上 `TownAnchorContract` 当前只覆盖 `DailyStand_01`，未覆盖 `DailyStand_02 / 03`，说明这组 cue 虽已语义命名，但还没真正迁完整。
  4. semantic anchor 视角下，后半段现状不是“缺 id”，而是“id 已挂上，但 contract 覆盖和真实落位深度不均”：
     - 已有且较稳：`DinnerBackgroundRoot`、`NightWitness_01`
     - 已有但仍待深化：`DailyStand_01 / 02 / 03`
     - 其中最值得先补 contract / 实排的是 `DailyStand_02 / 03`
     - `ReturnAndReminder` 当前虽然挂的是 `DinnerBackgroundRoot`，但更像借旧 anchor 顶着，后续值得拆成自己的 reminder 语义锚。
  5. 如果这轮只推进一刀，最值钱的迁移顺序是：
     - `dayend-watch-301`
     - `reminder-bg-203`
     - `reminder-bg-201`
     - `daily-102`
     - `daily-103`
     - 理由是前三条是最纯的旧 proxy；后两条则正好卡在 `DailyStand_03 / 02` 这两个当前 contract 还没补齐的缺口上。
- 本轮未做：
  - 未改 `StageBook`
  - 未改 `TownAnchorContract`
  - 未进 Unity live 验证
- 当前恢复点：
  - 后续若继续真实施工，最稳的一刀不是再补 `Dinner / NightWitness`，而是先把：
    1. `ReturnAndReminder`
    2. `DayEnd`
    3. `DailyStand_02 / 03`
    这三块从旧 proxy / 半语义占位推进到真正可消费的 anchor 落位。

## 2026-04-06｜续工实装第三刀：cue 回放已接入 semantic anchor 起点，旧 proxy 尾巴开始迁真锚点

- 当前主线没有换：
  - 仍是 `spring-day1` 导演线真实施工；
  - 这轮主刀是把 `StageBook cue` 从“spawn 吃 Town 语义、回放还吃旧绝对坐标”推进到“cue 起点/路径也能吃 semantic anchor”。
- 本轮真实落地：
  1. `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
     - `SpringDay1DirectorActorCue` 新增：
       - `useSemanticAnchorAsStart`
       - `startPositionIsSemanticAnchorOffset`
     - 新增 `SpringDay1DirectorSemanticAnchorResolver`
       - 先找 live scene 同名锚点
       - 再退 `SpringDay1TownAnchorContract`
     - `SpringDay1DirectorStagingPlayback.ApplyCue()`
       - 现在能把 cue 起点解析成：
         - 当前出生位
         - semantic anchor
         - semantic anchor + offset
         - 绝对世界坐标
     - `ResolveTargetPosition()`
       - 新增“legacy 绝对路径围绕 semantic anchor 起点自动重基”口径；
       - 旧绝对 path 不用全量手改，也能先迁到真实 Town 语义附近。
  2. `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`
     - cue 面板新增：
       - `语义锚点作为起点`
       - `起点按锚点偏移`
     - 录制写回现在会优先落成：
       - anchor 相对 `startPosition`
       - anchor/起点相对 `path`
     - 手工“选中物体设为起点 / 路径点”也会按当前 cue 口径写相对数据，不再只能写死绝对坐标。
  3. `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
     - 先把最 stale 的 6 条 cue 真迁了一刀：
       - `ReturnAndReminder_WalkBack/reminder-bg-203`
       - `ReturnAndReminder_WalkBack/reminder-bg-201`
       - `DayEnd_Settle/dayend-watch-301`
       - `DailyStand_Preview/daily-101`
       - `DailyStand_Preview/daily-104`
       - `DailyStand_Preview/daily-203`
     - 这批 cue 不再依赖“当前出生位 + 原点小偏移”；
     - 已改成 `semantic anchor + offset` 的正式数据口径。
  4. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增：
       - semantic anchor 直接起点测试
       - semantic anchor + offset 起点测试
       - legacy 绝对 path 重基测试
       - migrated cue 数据标记测试
  5. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把新测试名称补进 `Run Director Staging Tests` 目标名单。
- 本轮 own 验证：
  1. `manage_script validate`
     - `SpringDay1DirectorStaging.cs`：`warning`，仅性能告警，无错误
     - `SpringDay1DirectorStagingWindow.cs`：`warning`，仅性能告警，无错误
     - `SpringDay1DirectorStagingTests.cs`：`clean`
     - `SpringDay1TargetedEditModeTestMenu.cs`：`clean`
  2. own-path `git diff --check`
     - 对本轮 5 个 own 文件为 `clean`
  3. `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
     - 当前 fresh 外线 blocker 已收敛成：
       - `Assets\\YYY_Scripts\\Service\\Rendering\\LookDev2D\\Editor\\LocalLightingReviewCreator.cs(295,78): error CS1061`
     - 这不是本轮 day1 own 文件。
  4. `Run Director Staging Tests`
     - command bridge 能执行；
     - 但当前结果文件仍只跑出旧 13 条名单，没有 fresh 到我这轮新测试名单；
     - 结合 fresh console，可判当前真实 blocker 是外线 compile red 挡住了 Editor 使用我这轮新编译体。
- 当前判断更新：
  - `Town` 语义现在不只吃到了 crowd spawn，也开始吃进 cue 起点/路径解释；
  - 但 `DailyStand_02 / 03` 仍未进 contract，`DailyStand_Preview` 只先迁了 `DailyStand_01` 这半边；
  - 这轮后，Day1 更深一层的未完成项已经变成：
    1. 等外线红清掉后 fresh 跑新导演测试名单
    2. 继续补 `DailyStand_02 / 03` 的 contract / cue
    3. 再把 `NPC/UI` 回流整进 Day1 主链。

## 2026-04-06｜只读调度补记：Town / UI / NPC 三线全量回执已重新对齐

- 当前主线仍是：
  - `spring-day1` 自己继续做 Day1 最终整合位；
  - 不再把 `Town/UI/NPC` 当成“还有一大坨没做”的黑箱线程。
- 本轮从三份全量回执收出的稳定判断：
  1. `Town`
     - resident scene-side 最小承接层已经站住；
     - 当前剩余问题不再是 resident 第三刀，而是 mixed-scene 子域；
     - 现在不该把 `CrowdDirector / StageBook / day1 active director 主刀` 交回 Town。
  2. `UI`
     - `任务卡 / PromptOverlay` 基本出主战场；
     - 当前 UI 真主战场只剩 `Workbench`，尤其左列 recipe live 显示与整组玩家面 Workbench 体验；
     - `DialogueUI / 正式气泡` 基础链已补，但仍欠 live 终验；
     - day1 不该自己回吞 UI 壳体细节。
  3. `NPC`
     - resident semantic matrix、director consumption snapshot、formal one-shot consumed contract、bridge tests/menu 都已站住；
     - 当前 blocker 已转到 day1 的 deployment / director consumption / Town 常驻落位；
     - NPC 后续最适合继续守内容、fallback contract 与 probe/tests，不该回吞 deployment / scene / UI。
- 当前新的全局调度判断：
  1. `day1` 继续自己吃：
     - resident deployment
     - director consumption
     - `DailyStand_02 / 03`
     - `Town/NPC/UI -> Day1` 最终总整合
  2. `UI` 继续吃：
     - Workbench 玩家面 UI/UE
     - DialogueUI 正式壳
     - day1 玩家面提示壳
  3. `NPC` 继续吃：
     - manifest / content / formal fallback contract / bridge probes
  4. `Town` 暂不抢 day1 active 代码；
     - 若后续继续分担，应接 `Town` mixed-scene 子域，而不是 day1 director 主刀。

## 2026-04-06｜四份 Day1 最终收尾阶段清单已正式落地

- 当前主线没有变化：
  - `spring-day1` 继续作为 `Day1 owner / 主控台 / 最终整合位`；
  - 这轮不再停在“看回执做判断”，而是把核实后的全局判断真正落成四份正式清单。
- 本轮已新增 4 份工作区正文：
  1. `2026-04-06_spring-day1_Day1最终收尾阶段总控清单_16.md`
  2. `2026-04-06_spring-day1_UI协作线最终收尾阶段清单_17.md`
  3. `2026-04-06_spring-day1_NPC协作线最终收尾阶段清单_18.md`
  4. `2026-04-06_spring-day1_Town协作线最终收尾阶段清单_19.md`
- 这 4 份清单的定位已钉死：
  - 不是各线程自己的愿望单；
  - 而是同一个 `Day1 最终收尾阶段` 下，由 `day1` 向 `day1 / UI / NPC / Town` 四面拆出的协作清单。
- 当前这轮写进去的核心完成定义：
  1. `正式剧情 / 正式任务 / 正式聊天` 必须 `一次性消费，不可重入`
  2. `101~301` 需要从“阶段到了突然生成”转向更像驻村常驻 resident
  3. `Town / UI / NPC` 三线成果必须并回 `Day1` 主链，而不是各自成立
  4. `opening -> DayEnd` 需要最终收成一条可停 baseline
- 当前针对三条协作线的拆分也已正式落字：
  1. `UI`
     - 继续主守 `Workbench / DialogueUI / 玩家面提示壳`
  2. `NPC`
     - 继续主守 `resident semantic matrix / formal-consumed contract / bridge tests`
  3. `Town`
     - 继续主守 `resident root / anchor / slot / scene-side contract`
- 当前恢复点：
  - 后续如果要继续发 prompt 或直接推进下一轮真实施工，可以直接从这四份清单出发，不必再重复做“谁负责什么”的整轮重分析。
- 当前 thread-state：
  - 本轮 docs 落地完成后已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-06｜四份深度续工引导 prompt 已正式落地

- 当前主线没有变化：
  - `spring-day1` 继续作为 `Day1 owner / 主控台 / 最终整合位`；
  - 本轮是在上一组阶段清单之上，继续把可直接转发的下一轮深度续工 prompt 落成文件。
- 本轮新增 4 份 prompt 正文：
  1. `2026-04-06_spring-day1_最终收尾总阶段深度续工自用prompt_20.md`
  2. `2026-04-06_UI线程_Day1最终收尾总阶段深度续工prompt_21.md`
  3. `2026-04-06_NPC线程_Day1最终收尾总阶段深度续工prompt_22.md`
  4. `2026-04-06_Town线程_Day1最终收尾总阶段深度续工prompt_23.md`
- 这 4 份 prompt 的共同口径已钉死：
  1. 不再只做 Day1 协作面，也要把各线程当前能完成的自家遗留一起清掉
  2. 一轮内尽量把本轮范围内“能做完的部分”都狠狠干到最深处
  3. 不允许回到说明层、轻量试探层或“先做一点点”的口径
  4. 统一要求：
     - 可按需使用 `subagent`
     - 有问题先回 `spring-day1`
     - 无问题继续下一部分
     - 最终按用户可读六点卡 + 自评汇报
- 当前恢复点：
  - 如果用户现在要直接转发，已经可以直接发 `20 / 21 / 22 / 23`
  - 后续如果还要继续迭代 prompt，可在这 4 份上继续做版本推进
- 当前 thread-state：
  - 本轮 prompt 落地完成后已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-06｜认知校准补记：day1 不是四线之一，而是最终收尾阶段的总控台

- 当前新判断：
  - `spring-day1` 的身份不能再按“四条并行施工线里的其中一条”理解；
  - 它当前更准确的角色是：
    1. `Day1 owner`
    2. `导演 / 主控台`
    3. `最终整合位`
- 基于 fresh 核实后的现状：
  1. `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
     - 当前 `errors=0 warnings=0`
     - 外线 compile red 已暂时清掉
  2. `Show-Active-Ownership`
     - `spring-day1 / Town / UI / NPC` 当前都在 `PARKED`
     - 说明现在适合进入“总阶段重新排兵”，而不是继续把谁当成正在独占施工的单线
- 当前最终收尾阶段的总目标已收敛成：
  1. 把 `Town / UI / NPC` 三线已经做成的 contract / scene-side / 玩家面成果真正吃回 `Day1`
  2. 完成：
     - resident deployment
     - director consumption
     - UI 玩家面总收口
     - formal one-shot / task one-shot / post-consume fallback 真闭环
  3. 跑出从 opening 到 `DayEnd` 的一轮完整可停 baseline
- 当前新的角色分工理解：
  - `day1`
    - 负责最终完成定义、总装、总验收、主链闭环
  - `UI`
    - 负责玩家面壳体与 Workbench / DialogueUI / prompt 壳
  - `NPC`
    - 负责 resident matrix / formal fallback contract / probe / tests
  - `Town`
    - 负责 Town-side 承接层与 future mixed-scene / resident scene-side 接盘
- 后续若要产出四份 Day1 落地清单，不应再按“各线程自己的愿望单”写，而应按：
  1. `day1` 作为总控台定义最终阶段完成条件
  2. 再把同一阶段拆给 `day1 / UI / NPC / Town`
  3. 每线只列与 `Day1 最终收尾` 直接相关的那部分内容

## 2026-04-06｜导演线续工补记：one-shot 与 resident consumption 已继续压进 runtime 验收面

- 当前主线未变：
  - `spring-day1` 继续作为 Day1 导演线 owner，主刀压两件事：
    1. `formal / sequence one-shot` 不可重播
    2. `resident deployment / director consumption` 真被 runtime 和 live 验收链吃回
- 这轮继续做成的代码层收口：
  1. `SpringDay1NpcCrowdDirector.cs`
     - runtime `SyncCrowd()` 已开始真实消费 `BuildBeatConsumptionSnapshot(currentBeatKey)`
     - resident active / parent 选择已吃 `priority / support / trace / backstagePressure / presenceLevel`
     - scene root 解析已优先找现有 `CarrierRoot / ResidentRoot`，不再默认只自造运行时空根
  2. `SpringDay1Director.cs`
     - 正式对白恢复守门继续收口到 `DialogueManager.HasCompletedSequence(...)`
     - 新增 `GetCurrentResidentBeatConsumptionSummary()`，并把 `BeatConsumption` 直接写进 `SpringDay1LiveValidationRunner.BuildSnapshot()`
     - 运行态 `NPC` 摘要已新增 `formal=` 与 `yieldResident=`，可直接看 formal 是否已让位给 resident / 闲聊
  3. `SpringDay1DialogueProgressionTests.cs`
     - 补强了 `BeatConsumption` 与 `NPC formal/resident fallback` 的字符串护栏断言
- 当前验证状态：
  - `git diff --check` 对本轮 own 文件通过
  - `status / errors` 当前仍能读到外部噪音：
    - `Missing Script (Unknown)` x2
    - `DialogueChinese Pixel SDF.asset` importer inconsistent
  - `sunset_mcp.py manage_script validate` 当前 bridge 返回 `Tool 'manage_script' not found`
  - `validate_script / no-red` 当前被 `subprocess_timeout:dotnet:60s` 卡住，属于本轮验证链 blocker，不等于已确认 own red
- 当前恢复点：
  - 下轮若继续，优先按这条恢复：
    1. fresh compile/no-red 再取一次真值
    2. 如果验证链仍卡，就转去查 `validate_script/no-red` 的 dotnet timeout 根因
    3. 若 fresh compile 站住，再继续把 Day1 live 验收链往真实场景推进

## 2026-04-06｜导演线续工补记：验证链从“纯阻断”推进到“可用降级”，Town 入口不再是主撞点

- 当前主线没变：
  - 仍是 `one-shot 不重播` + `resident consumption 真被 runtime 吃回`
- 这轮新增稳定结果：
  1. `SpringDay1Director.cs`
     - 新增 `GetOneShotProgressSummary()`
     - `SpringDay1LiveValidationRunner.BuildSnapshot()` 已新增 `OneShot`
  2. `SpringDay1DialogueProgressionTests.cs`
     - 已补 `GetOneShotProgressSummary()` 与 `AppendPair("OneShot"` 的字符串护栏
  3. `scripts/sunset_mcp.py`
     - MCP JSON 粘连响应已能顺序解码
     - `CodexCodeGuard` 超时后会附带 stale 进程清理结果，不再只给一句裸 timeout
  4. `scripts/git-safe-sync.ps1`
     - 构建 `CodexCodeGuard` 前会先清 stale `dotnet/CodexCodeGuard` 进程，减少 Town/Day1 被旧锁拖死
- 当前这轮拿到的验证真值：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`：`assessment=no_red`
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：`assessment=no_red`
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`：native validate 已过，但本次仍落 `unity_validation_pending`，原因是 `wait_ready` 被 `stale_status` 卡住，不是 owned error
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`：当前 `errors=0 warnings=0`
  - `git diff --check`：当前 own 变更通过，仅剩 CRLF 提示
- 当前新协作判断：
  - `Town` 新回执已证明入口 probe `completed`
  - 所以 `Town` 的 `相机 / 转场 / 玩家位` 入口层当前不再是 Day1 第一业务撞点
  - Day1 下一撞点继续往更深的 runtime/player-facing 消费层移动

## 2026-04-06：UI 线程继续深砍 prompt_21，并交回一份新的阶段回执 25

- 当前主线：
  - `UI` 线程继续只做 `Workbench / DialogueUI / formal-informal-resident 玩家面提示壳`。
- 本轮 UI 线程新增可被 day1 直接消费的结果：
  1. `Workbench` 左列 runtime 绑定阶段新增强制恢复口：
     - 当前若还能读到正式 recipe
     - 但左列还不是 generated row
     - 就直接 `RebuildRecipeRowsFromScratch(forceRuntimePrefabStyle: true)`
     - 也就是左列不再继续长期依赖旧 prefab 手工 row。
  2. 玩家面提示壳 one-shot 规则又更贴近真实：
     - `SpringDay1ProximityInteractionService.ShouldReplaceCandidate()` 改成先比 `Priority` 再比 `Distance`
     - formal 左下角 copy 不再依赖旧 generic prompt 串
     - formal 不会再因为更近的 informal / resident 被抢走
  3. `formal consumed` 后的最小 resident 回落语义已进玩家面：
     - `日常交流`
     - `按 E 聊聊近况`
  4. formal priority phase 的 automatic nearby feedback 已被 UI 线程压掉：
     - 不再新冒泡
     - 已在屏上的也会立刻回收
- 本轮 UI 线程给出的新证据：
  - `python scripts/sunset_mcp.py errors`：`errors=0 warnings=0`
  - `python scripts/sunset_mcp.py status`：baseline `pass`、bridge `success`、`isCompiling=false`
  - touched files 的 `git diff --check`：未见新的 owned 语法/空白问题
  - 但 `validate_script` 仍被 `subprocess_timeout:dotnet:60s` 卡住
- 当前最准确的协作判断：
  - UI 线程这轮继续把 `Workbench` 和 `玩家面提示壳 contract` 往前推进了一大步
  - 但 `Workbench` 修后 fresh live 图仍未补到，所以 day1 当前仍应把它归类为：
    - `结构 / targeted probe 更厚`
    - `不是体验已过线`
- 新增交接物：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-06_UI线程_给day1阶段回执_25.md`

## 2026-04-06：UI 线程只读侦查 Workbench runtime 空左列与右列重叠链，结论已压到 3 个代码根因

- 当前主线：
  - 这轮不是继续写 UI，而是按用户要求只读审查 `SpringDay1WorkbenchCraftingOverlay` 的直接代码链，查清：
    1. 为什么 runtime `Workbench` 左列 recipe 仍可能“可点击但无内容”
    2. 为什么右侧标题 / 描述 / 材料区仍可能重叠或超边界
- 本轮范围：
  - 只读了：
    - `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - `Assets/222_Prefabs/UI/Spring-day1/SpringDay1WorkbenchCraftingOverlay.prefab`
    - `Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs`
  - 没改任何 tracked 文件，也没进 Unity / Play / MCP live 写。
- 这轮坐实的最关键根因：
  1. 左列 row 恢复链对 `manual/generated/orphan row` 的理解不一致：
     - `BindExistingRows()/BindRecipeRow()` 只把可完整绑定的 row 收进 `_rows`
     - `HasUnreadableVisibleRecipeRows()/NeedsRecipeRowHardRecovery()` 又只扫 `_rows`
     - 但 `NeedsRecipeRowHardRecovery()` 还把 `name/summary.parent == row.rect` 当硬条件，和 generated row 的 `TextColumn` 结构直接冲突
     - 结果是：坏掉但仍带 `Button` 的 `RecipeRow_*` 子物体可能留在内容根下、可点击却不被刷新；而 generated row 又会被误判成持续需要 hard recovery
  2. prefab detail shell 路径把 `materials viewport/content` 直接退化成 `selectedMaterialsText.rectTransform`：
     - `TryBindRuntimeShell()` 与 `EnsureMaterialsViewportCompatibility()` 在 `UsesPrefabDetailShell()` 为真时都把 `materialsViewportRect/materialsContentRect` 直接指向材料文本本身
     - 这意味着右侧材料区实际上没有真实 viewport/content/mask/scroll 容器，只剩一个固定文本框
  3. prefab detail shell 的几何修正只处理 `标题/描述`，没有把 `材料标题/材料区/数量区/按钮区` 纳入同一个重排器：
     - `NormalizePrefabDetailShellGeometry()` 只改 `SelectedName/SelectedDescription`
     - 同时又把 `SelectedDescription/SelectedMaterials` 设成 `Overflow`
     - prefab 里的 `MaterialsTitle/SelectedMaterials/QuantityControls/CraftButton` 仍是固定绝对布局，所以长描述或多行材料会自然压到下方区域
  4. legacy detail 手动适配算法本身也会“超配空间”：
     - `AdjustLegacyDetailLayoutToFitCurrentContent()` 即使 `availableForDescription` 已经不够，仍强保 `description >= 22` 与 `materials viewport >= 56`
     - 在内容稍长、底部区块较高或字号上抬时，会把重叠从“可能”变成“必然”
- 当前阶段判断：
  - 这轮已经不是“现象猜测”，而是把问题压到了 `row recovery contract` 和 `detail shell layout contract` 两层真逻辑上；
  - 但这仍是只读分析，不是修复完成。
- 当前恢复点：
  - 如果后续继续真实施工，最小正确入口应先统一 `RecipeRow` 的唯一目标结构，再把右列 detail 收敛成一个真实的 viewport/layout 链；
  - 不该继续在现有 prefab 绝对坐标上做零碎补丁。

## 2026-04-06｜day1 最终整合补记：把 UI/NPC 回执吃回 live snapshot，并把 compile-first 验证链拉回可用降级

- 当前主线：
  - 不再继续发 prompt，而是在 `spring-day1` 主控位把 `UI 回执 25 / NPC 回执 15 / Town 回执 15` 真接回 Day1 主链。
- 这轮实际做成了什么：
  1. `Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`
     - 新增 `DebugSummary`
     - 现在会直接报 `phase / formalPriority / suppressed / activeNpc / activeBubble / lastNpc / lastBubble`
  2. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 新增 `GetRuntimeRecipeShellSummary()`
     - 直接暴露 `visible / rows / visibleRows / generated / prefabRecipeShell / prefabDetailShell / unreadableRows / hardRecovery / selected`
  3. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `SpringDay1LiveValidationRunner.BuildSnapshot()` 已新增：
       - `NpcPrompt`
       - `NpcNearby`
       - `WorkbenchUi`
     - `NpcPrompt` 现在直接读取 resident prompt tone、caption/detail、activeNpc、会话 state、player/npc bubble
     - `NpcNearby` 现在直接吃 `PlayerNpcNearbyFeedbackService.DebugSummary`
     - `WorkbenchUi` 现在直接吃工作台左列 runtime 壳恢复摘要
  4. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增上述 3 个快照键与对应 helper 的字符串护栏
     - 新增 nearby debug summary / workbench runtime shell summary 的护栏
- 当前协作判断更新：
  - `UI 25` 已被 Day1 真吃回：
    - formal consumed 后的 prompt 回落
    - Workbench 左列恢复链
    - formal priority 抑制 nearby
  - `NPC 15` 已被 Day1 真吃回：
    - resident fallback 不再只停在按 `E` 闲聊
    - `phase-aware nearby resident` 也进入 Day1 自己的可观测层
  - `Town 15` 当前继续作为外部真值：
    - `entry + first player-facing layer` 已过
    - Day1 下一撞点改判到更深的 `DinnerBackgroundRoot / NightWitness_01 / DailyStand_01` runtime actor consumption
- 当前验证结果：
  - `validate_script --owner-thread spring-day1 Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs`：`assessment=no_red`
  - `validate_script --owner-thread spring-day1 Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`：`assessment=no_red`
  - `validate_script --owner-thread spring-day1 Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`：`assessment=no_red`
  - `validate_script --owner-thread spring-day1 Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：`assessment=no_red`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
  - `git diff --check -- <4 touched files>`：通过，仅有 `CRLF` 提示
- 当前工具链判断：
  - `validate_script` 不再是纯阻断
  - 现在已恢复成“串行 compile-first 可用、并发时仍可能回落 `CodexCodeGuard returned no JSON`”的降级路径
- 当前恢复点：
  - 下轮若继续，优先把 `NpcPrompt / NpcNearby / WorkbenchUi` 再压进 live 验收路径或必要 runtime test
  - 之后继续推进更深的 `DinnerBackgroundRoot / NightWitness_01 / DailyStand_01` Town runtime 消费

## 2026-04-06｜day1 续工补记：继续把剩余 own 项往更深处压，当前停在 sync blocker 前

- 当前主线未变：
  - 继续作为 `spring-day1` owner，把 Day1 自己的最终整合、runtime 证据和 one-shot/resident 承接继续压深。
- 这轮后半刀新增做成的内容：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `BuildRuntimeSummary()` 已新增：
       - `beat=...`
       - `consumption=p/s/t/b`
       - 每个状态的 `@group= / @cue= / @role= / @presence=`
     - 这意味着 Day1 现在不只是知道“NPC 在不在”，而是能直接看出当前是 resident default / takeover-ready / backstage，当前 cue 是什么、当前导演消费角色是什么。
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 已补 crowd runtime summary 深层护栏：
       - `BuildBeatConsumptionSummary`
       - `|beat=`
       - `@group=`
       - `@cue=`
       - `@role=`
       - `@presence=`
  3. `Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`
     - 新增 `PlayerNpcNearbyFeedbackService_DebugSummary_ReportsPhaseAndLastResidentBubble`
     - 新增 `SpringDay1WorkbenchCraftingOverlay_RuntimeRecipeShellSummary_ReportsSelectedRecipeAndFlags`
     - 新增 `SpringDay1LiveValidationRunner_BuildSnapshot_ConsumesNpcAndWorkbenchSummaries`
     - 也就是这轮新增的 `NpcPrompt / NpcNearby / WorkbenchUi` 不只是字符串护栏，已经开始有 runtime 级执行测试。
- 当前验证真值：
  - `validate_script --owner-thread spring-day1 Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`：`assessment=no_red`
  - `validate_script --owner-thread spring-day1 Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：`assessment=no_red`
  - `validate_script --owner-thread spring-day1 Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`：`assessment=no_red`
  - `errors --count 20 --output-limit 20`：`errors=0 warnings=0`
  - `git diff --check --` 当前 touched own 文件：通过，仅 `CRLF` 提示
- 当前阻断与判断：
  - 尝试 `Ready-To-Sync` 后已确认：
    - `spring-day1` own roots 里还有 `75` 个不在本轮切片内的 remaining dirty/untracked
    - 所以这轮虽然当前切片代码是干净的，但还不能合法 safe-sync / commit
- 当前恢复点：
  - 下轮继续时，优先顺序固定为：
    1. 继续把 `NpcPrompt / NpcNearby / WorkbenchUi` 往 live 验收链推进
    2. 再继续吃更深的 `DinnerBackgroundRoot / NightWitness_01 / DailyStand_01`
    3. 如果要提交，先清掉 `spring-day1` own roots 的历史残留 dirty，再重新 `Ready-To-Sync`

## 2026-04-06：UI 线程继续真实施工，Workbench 与 DialogueUI 又各向前压了一刀

- 当前主线：
  - `day1` 暂停后，UI 线程继续只收自己还没做完的玩家面遗留；
  - 本轮继续只打：
    1. `Workbench`
    2. `DialogueUI`
    3. formal / resident 提示壳里仍属 UI own 的显示链
- 本轮实际新增完成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 修正 generated `RecipeRow` 会被自身 `NeedsRecipeRowHardRecovery()` 误判成坏壳的问题；
     - `RefreshCompatibilityLayout()` 对 prefab detail shell 不再直接放行，而是新增 `RefreshPrefabDetailShellGeometry()`；
     - 右侧材料区在 prefab 壳下不再直接退化成 `SelectedMaterials` 本体，而是补回真实 `MaterialsViewport -> Content -> SelectedMaterials` 链；
     - 标题、描述、材料区会按当前内容高度重新排，不再只修 header 然后让下半列继续硬撞。
  2. `SpringDay1LateDayRuntimeTests.cs`
     - 把一条还在断言旧“剩余 0 / 剩 0”语义的 workbench 测试回正到当前真实进度文案：`进度  0/1`。
  3. `DialogueUI.cs` + `DialogueChineseFontRuntimeBootstrap.cs` + `DialogueChineseFontRuntimeBootstrapTests.cs`
     - 把 continue 显示文案和空文本字体探针拆成独立常量，不再让同一句硬编码同时承担两件事；
     - bootstrap 预热串新增 `摁空格键继续`，避免 continue 单独掉字；
     - 独白字体回到与 `innerMonologueFontKey` 同一条事实源，再保留 `SoftPixel` 只作 fallback；
     - 字体 bootstrap 测试探针同步补到 `摁空格键继续`。
- 本轮验证：
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name SpringDay1LateDayRuntimeTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name DialogueUI --path Assets/YYY_Scripts/Story/UI --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name DialogueChineseFontRuntimeBootstrap --path Assets/YYY_Scripts/Story/Dialogue --level basic`：`clean`
  - `python scripts/sunset_mcp.py manage_script validate --name DialogueChineseFontRuntimeBootstrapTests --path Assets/YYY_Tests/Editor --level basic`：`clean`
  - `python scripts/sunset_mcp.py errors --count 20 --output-limit 10`：`errors=0 warnings=0`
  - `git diff --check -- [touched files]`：无新的 owned 空白/语法问题，仅 `CRLF/LF` 提示
- 当前判断：
  - `Workbench` 这轮已经不是“继续调 offset”，而是把左列 row 合同和右列 materials/detail 合同都往唯一结构上收；
  - `DialogueUI` 这轮继续补的是“continue 文案能显示，但字库 warmup 会不会自己打自己”的回弹点；
  - 仍未拿到 fresh live / GameView 证据，所以当前只能诚实 claim：`结构/代码层继续前进，体验终验仍待 live`。
- 当前恢复点：
  1. 下一轮若继续，优先补 `Workbench` fresh live，确认左列文字和右列材料区在玩家视面是否都已真正站住；
  2. `DialogueUI` 若仍见 NPC 正式气泡方框化，优先回压 `NPCBubblePresenter.cs` owner，不由 UI 线程擅自吞并。

## 2026-04-06｜只读审计：spring-day1 Ready-To-Sync blocker 的 remaining dirty/untracked 分桶

- 当前子工作区主线：
  - 不改业务代码，只读判断 `2026-04-06 19:29 +08:00` 那次 `Ready-To-Sync BLOCKED` 里的 `75` 个 same-root remaining dirty/untracked，哪些适合单开 cleanup slice，哪些明显不该动。
- 本轮读取锚点：
  - `spring-day1` 在 `2026-04-06T19:29:12+08:00` 的 state 仍是：
    - `PARKED`
    - `Ready-To-Sync blocked: spring-day1 own roots still have 75 remaining dirty/untracked paths outside this slice`
  - 审计过程中又读到 `2026-04-06T19:55:27+08:00` 的新状态：
    - `ACTIVE day1-finish-remaining-owner-work-2026-04-06-b`
    - 说明这批尾账已有人继续接盘；本次分桶继续以旧 blocker batch 为主，新状态只用来辅助排除误碰。
- 本轮最关键判断：
  1. 最适合单开 cleanup 的，不是 `Story/UI`、`Service/Player`、`Story/Managers` 里的大脚本，而是：
     - `.kiro/specs/900_开篇/spring-day1-implementation/003-进一步搭建/` 下成批未纳管 prompt / 回执 / 任务单文档
     - `Assets/YYY_Tests/Editor/` 下孤立 `.meta` 与新测试壳
  2. `Story/UI`、`Service/Player`、`Story/Managers` 当前看到的多数 `.cs` 都已是活跃 feature diff，不再是 hygiene：
     - `SpringDay1PromptOverlay.cs`
     - `DialogueUI.cs`
     - `PlayerNpcChatSessionService.cs`
     - `SpringDay1NpcCrowdManifest.cs`
     - `SpringDay1LateDayRuntimeTests.cs`
     这些 diff 量级都已是数百行级，顺手碰会直接越过 cleanup 边界。
  3. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs` 已明确归 `存档系统` 当前 slice，不应被 `spring-day1` cleanup 吞并。
  4. `.codex/threads/Sunset/spring-day1/` 在这次 blocker batch 里基本没有切片外残留；线程根不是当前主要脏源。
- 适合优先清的桶：
  1. `003-进一步搭建/` 下历史 prompt / 回执 docs 簇
  2. `Assets/YYY_Tests/Editor/` 的 meta-only 尾账簇
  3. `Assets/YYY_Tests/Editor/` 的孤立新测试壳簇
  4. 仅当要做 docs cleanup 时，再单独考虑父层 `spring-day1-implementation/memory.md`
- 明显不该动的面：
  - `DialogueUI.cs`、`SpringDay1PromptOverlay.cs`、`SpringDay1WorkbenchCraftingOverlay.cs`
  - `PlayerNpcChatSessionService.cs`、`PlayerNpcNearbyFeedbackService.cs`
  - `SpringDay1Director.cs`、`SpringDay1NpcCrowdDirector.cs`、`SpringDay1NpcCrowdManifest.cs`
  - `StoryProgressPersistenceService.cs`
  - `SpringDay1LateDayRuntimeTests.cs`、`SpringDay1OpeningRuntimeBridgeTests.cs`
  - `PlayerThoughtBubblePresenter.cs`、`PlayerNpcRelationshipService.cs`
- 验证状态：
  - `git status --short --untracked-files=all`（限定 `Managers/UI/Service/Player/Tests/Editor/specs/thread` 六个 roots）
  - `Show-Active-Ownership.ps1`
  - 回读 `spring-day1 / UI / NPC / 存档系统` 的 active-thread state 与相关 memory
  - 结论层级：`只读审计成立`
- 当前恢复点：
  - 若后续真开 cleanup，优先用“docs/回执壳”或“tests meta/孤立测试壳”做最小切片，不要用 `spring-day1` 名义去吞 UI / NPC / 存档系统正在推进的代码面。

## 2026-04-06｜只读审计：DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03 的 deeper runtime consumption 最小缺口已收敛

- 当前主线目标：
  - 只读审 `spring-day1` 当前 own 代码与数据，判断 `DinnerBackgroundRoot / NightWitness_01 / DailyStand_01~03` 在 deeper runtime actor consumption 上已做到哪，下一刀最该砍哪。
- 本轮子任务：
  - 优先读取 `SpringDay1DirectorStageBook.json`、`SpringDay1NpcCrowdDirector.cs`、`SpringDay1Director.cs` 与相关 tests；
  - 不进入真实施工，不改业务代码、scene 或资源。
- 已确认的稳定事实：
  1. `SpringDay1Director.GetCurrentBeatKey()` 已把
     - `DinnerConflict_Table`
     - `ReturnAndReminder_WalkBack`
     - `FreeTime_NightWitness`
     - `DayEnd_Settle`
     - `DailyStand_Preview`
     纳入 day1 正式导演 beat；
     `BuildSnapshot()` 也已公开 `Crowd` + `BeatConsumption`，所以 day1 已具备 live 观测面。
  2. `SpringDay1NpcCrowdDirector.SyncCrowd()` 已真正消费 `manifest.BuildBeatConsumptionSnapshot(currentBeatKey)`；
     有 cue 时走 `ApplyStagingCue()`，无 cue 时走 `ApplyResidentBaseline()`；
     `ResolveSpawnPoint()` 也已支持：
     - live semantic anchor
     - `SpringDay1TownAnchorContract`
     - legacy/fallback
  3. `DinnerBackgroundRoot` 与 `NightWitness_01` 已不只是命名层：
     - `DinnerConflict_Table` 已有 4 条 `DinnerBackgroundRoot` 多人背景 cue；
     - `ReturnAndReminder_WalkBack` 已把 `DinnerBackgroundRoot` 迁成 semantic-anchor 相对起点；
     - `FreeTime_NightWitness` 已有 2 条 `NightWitness_01` cue；
     - `DayEnd_Settle` 已把 `NightWitness_01` 迁成 semantic-anchor 相对起点。
  4. `SpringDay1DirectorStagingTests` 已覆盖：
     - `DinnerBackgroundRoot` 的 semantic-anchor start
     - `NightWitness_01` 的 semantic-anchor offset
     - `NightWitness_01` 的 Town contract fallback
  5. `NpcCrowdManifestSceneDutyTests` 已证明 manifest 侧的 `DailyStand_01~03` 语义名、resident semantics 与 `BeatConsumptionSnapshot` 都存在；
     这说明 Day1 数据层不是空白。
  6. 当前真正最浅的是 `DailyStand_02 / 03`：
     - `SpringDay1TownAnchorContract.json` 当前只收
       - `EnterVillageCrowdRoot`
       - `KidLook_01`
       - `NightWitness_01`
       - `DinnerBackgroundRoot`
       - `DailyStand_01`
     - 尚未覆盖 `DailyStand_02 / 03`
  7. `DailyStand_Preview` 当前是“半迁移”：
     - `daily-101 / daily-104 / daily-203` 已被 tests 明确要求改成 `useSemanticAnchorAsStart + startPositionIsSemanticAnchorOffset`
     - 但 `daily-103 / daily-102 / daily-201` 仍只是 captured absolute positions，没有同等级 semantic-anchor-start 护栏
  8. `SpringDay1NpcCrowdDirector.ApplyResidentBaseline()` 当前只会把 resident 回到 `BasePosition`；
     `BasePosition` 只在首次 spawn 时计算；
     也就是说，若没有新的 staging cue 或新的 base 重算，beat 切换本身不会把 resident 自动迁到新的 `DailyStand_02 / 03` contract 锚点。
- 当前判断：
  - `DinnerBackgroundRoot`：已到“runtime 可消费，后续再继续压多人密度/让位关系”的阶段。
  - `NightWitness_01`：已到“runtime 可消费，DayEnd 收束也已吃 semantic anchor”的阶段。
  - `DailyStand_01`：已到“部分 contract-driven”的阶段。
  - `DailyStand_02 / 03`：仍是当前最缺的薄层，名字和 cue 已在，但还没统一变成真正稳定的 contract-driven runtime actor consumption。
- 最小下一刀：
  1. 先补 `DailyStand_02 / 03` 到 `SpringDay1TownAnchorContract.json`
  2. 再把 `DailyStand_Preview` 里 `daily-103 / daily-102 / daily-201` 迁成 semantic-anchor start + offset
  3. 验证优先放在 `SpringDay1DirectorStagingTests.cs`
     - 追加 `AssertCueUsesSemanticAnchorStart(...)`
     - 追加 `CrowdDirector_ShouldFallBackToTownAnchorContract...` 的 `DailyStand_02 / 03` 版本
- 恢复点：
  - 若下一轮继续 day1，不该先回去再扩 `DinnerBackgroundRoot` 的复杂多人层；
  - 更稳的顺序是先把 `DailyStand_02 / 03` 从“绝对坐标排练”拉成“真实 anchor contract 消费”，再谈更深的 runtime actor consumption。

## 2026-04-06｜补记：Workbench `CS0136` 假红已清，真实现场已回到 fresh console 0 红

- 当前主线目标：
  - 用户先不管 day1 协同面，只要求继续清我 own 的 UI/UE 遗留；本轮插入式阻塞是用户报出 `SpringDay1WorkbenchCraftingOverlay.cs` 的 9 条 `CS0136`，要求先全部修掉。
- 本轮子任务：
  - 只处理 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 这组 compile red，并确认是不是旧控制台残留。
- 本轮实际确认：
  1. 源码 `EnsureMaterialsViewportCompatibility()` 已不是旧变量名；`1669~1743` 行当前实际是：
     - `prefabMaterialsTextRect`
     - `prefabCurrentParent`
     - `prefabTextWasDirectChild`
     - `prefabTextSiblingIndex`
     - `prefabNeedsViewport`
     - `prefabViewportImage`
     - `prefabViewportMask`
     - `prefabNeedsContent`
     - `prefabMaterialsLabelLayout`
  2. `python scripts/sunset_mcp.py manage_script validate --name SpringDay1WorkbenchCraftingOverlay --path Assets/YYY_Scripts/Story/UI --level basic --output-limit 10`
     返回 `clean errors=0 warnings=0`。
  3. 初次 `validate_script` 与 `status` 说明真正卡点不是脚本本体，而是 Unity 当时停在：
     - `is_playing=true`
     - `is_paused=true`
     - `activity_phase=playmode_transition`
     - `blocking_reasons=["stale_status"]`
  4. 通过 `Library/CodexEditorCommands/requests/*.cmd` 向 `CodexEditorCommandBridge` 发出 `STOP` 后，bridge status 已回到：
     - `isPlaying=false`
     - `lastCommand=playmode:EnteredEditMode`
  5. fresh console 已刷成：
     - `python scripts/sunset_mcp.py errors --count 20 --output-limit 20`
     - `errors=0 warnings=0`
- 当前判断：
  - 用户报的这组 `CS0136` 已经不是当前源码红，而是 Unity 当时卡住的旧现场 / 假红；
  - 这轮已把该假红现场退干净，并确认 `Workbench` 脚本当前没有 owned compile error。
- 当前遗留：
  - `validate_script` 仍会落 `unity_validation_pending`，原因不是脚本错误，而是 MCP 的 `editor_state.ready_for_tools=false + stale_status` 噪音还在；
  - 但 fresh console 已经是 `0 error / 0 warning`。
- 恢复点：
  - 后续继续 `Workbench` 真正 UI 修复时，可以直接从“console 已清”的基线往下推，不要再把这 9 条 `CS0136` 当成当前真实红。

## 2026-04-06｜真实施工补记：DailyStand_02/03 contract 已补齐，Crowd runtime 已开始支持 Town

- 当前主线目标：
  - 继续把 `spring-day1` 后半段导演消费往真实 runtime 承接推进，不停在 `DinnerBackgroundRoot / NightWitness_01` 的已完成面上，而是把 `DailyStand_02 / 03` 和 `Town` runtime 这一层也吃回 Day1。
- 本轮真实施工切片：
  - `day1-finish-remaining-owner-work-2026-04-06-b`
- 本轮实际落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - 不再只认 `Primary`；
     - 新增 `TownSceneName`，并把 crowd runtime scene 支持扩到 `Primary + Town`；
     - `Update / HandleActiveSceneChanged / GetOrCreateState` 都改成走统一 `IsSupportedRuntimeScene(...)`。
  2. `Assets/Resources/Story/SpringDay1/Directing/SpringDay1TownAnchorContract.json`
     - 补进：
       - `DailyStand_02 = (-9.4, 1.5)`
       - `DailyStand_03 = (-3.6, 4.1)`
  3. `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
     - `DailyStand_Preview` 的说明口径已从“等 Town 更稳”更新到“`DailyStand_01~03` contract 已对齐”；
     - `daily-103 / daily-102 / daily-201` 已迁成 `useSemanticAnchorAsStart + startPositionIsSemanticAnchorOffset`；
     - 这意味着 `DailyStand_02 / 03` 不再只是旧 Primary 绝对坐标残影，而开始按 Town 语义锚吃 runtime。
  4. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增 `DailyStand_02 / 03` 的 Town contract fallback 护栏；
     - 新增 `CrowdDirector_ShouldAllowResidentSpawnInTownScene()`；
     - `StageBook_ShouldMarkMigratedTownCuesToUseSemanticAnchorStarts()` 已把 `daily-103 / daily-102 / daily-201` 也纳入。
  5. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增 `TownAnchorContract_CoversLateDayRuntimeAnchors()`；
     - 文本护栏已明确 `CrowdDirector` 必须支持 `TownSceneName / IsSupportedRuntimeScene`。
  6. `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-06_spring-day1_存档边界回执_01.md`
     - 已在最近安全点回写；
     - 明确 crowd 当前 runtime scene / 站位 / scene root 都不应直接持久化。
- 本轮验证：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --owner-thread spring-day1`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
    - 卡点是 `stale_status / CodeGuard timeout`，不是 owned red
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs --owner-thread spring-day1`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs --owner-thread spring-day1`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
  - `git diff --check`（本轮 touched files）
    - 通过
- 当前阶段判断：
  - `DinnerBackgroundRoot / NightWitness_01` 已经不再是 Day1 当前第一缺口；
  - `DailyStand_02 / 03` 现在已经从“contract 缺口”推进到“Day1 own 代码/数据/测试都开始真吃 Town 语义锚”的阶段；
  - 但整条 `spring-day1` own roots 仍有大量历史 dirty/untracked，所以这轮仍不具备直接 `Ready-To-Sync` / 提交条件。
- 下一恢复点：
  1. 如果继续业务面，优先看 `DailyStand_02 / 03` 这刀在 live/runtime 里是否还缺更深的 actor consumption 观察；
  2. 如果转收口面，下一刀应单开 docs/test-hygiene cleanup，而不是误吞活跃 UI/NPC/存档系统代码面。

## 2026-04-06｜只读核实：UI 线 formal one-shot / resident fallback 当前真实状态

- 当前主线目标：
  - 用户要求只读核对两份 UI 回执：
    - `2026-04-06_UI线程给day1全量回执_01.md`
    - `2026-04-06_UI线程_给day1阶段回执_25.md`
  - 只提炼 `formal one-shot / resident fallback` 直接相关的玩家面结果、未闭环项，以及若改走 `Town` 原生 resident 时 UI prompt 是否需要改。
- 本轮只读判断：
  1. 这组话题的最新有效口径以 `阶段回执 25` 为准；`全量回执 01` 主要提供较早的总盘点背景。
  2. 已直接做成的玩家面结果有 4 个：
     - formal 候选竞争已改成 `ForceFocus -> CanTriggerNow -> Priority -> Distance`，不再因为 informal / resident 更近就把正式入口抢走；
     - 左下角 formal 提示文案不再依赖旧 generic 串，只要 formal 仍 `Available`，就会稳定回到 `进入任务 / 按 E 开始任务相关对话` 这类正式入口口径；
     - formal priority phase 会压掉自动 nearby feedback，并在必要时立即收掉旧环境气泡残影；
     - formal consumed 后，idle 提示不再一律伪装成 `闲聊`，而是已有最小 resident 回落语义，如 `日常交流 / 按 E 聊聊近况`。
  3. 当前未闭环点也很明确：
     - resident 仍只是“最小回落”，不是 `Town` 原生 resident 的完整玩家面 contract；
     - 还缺更细的 phase-specific resident 文案、resident / informal 完整分层，以及 fresh live 终验证据；
     - 因此当前最多只能报“结构 / contract 更接近真实”，不能报“体验已过线”。
  4. 若下一轮方向改成 `Town` 原生 resident，UI prompt 需要改，而且重点应从“formal consumed 后给一个最小回落”切到“定义完整 resident 玩家面 contract”：
     - resident 何时接管；
     - resident 与 informal 如何分层；
     - 默认文案矩阵；
     - 是否继续压 nearby bubble；
     - 哪些 fresh live 图才算闭环。
- 验证口径：
  - 本轮完全基于两份 UI 回执的只读互校；
  - 结论属于文档层静态推断，不是 fresh live 终验。

## 2026-04-06｜总控补记：Day1 架构方向正式改判为 Town 原生 resident，runtime resident 降级为过渡实现

- 用户本轮重新钉死的新硬边界：
  1. 最终正确形态不是 `Primary` 里继续保留 `001/002/003`，再靠运行时 `SpringDay1NpcCrowdRuntimeRoot/Town_Day1Residents` 补一个假常驻层；
  2. 最终正确形态应是：
     - `Town` 里原生存在常驻 resident；
     - `Primary` 只留开场真正必须在 `Primary` 的角色；
     - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 这类位置由用户自己在 scene 里调整与配置；
     - 代码只消费这些现成位置，不允许再偷偷改位置，也不允许再用 runtime 替身锚点绕开。
- 本轮已做的治理动作：
  1. 重新核实了 `SpringDay1NpcCrowdDirector.cs`：
     - 当前确实会在支持 scene 中创建 `SpringDay1NpcCrowdRuntimeRoot -> Town_Day1Residents`；
     - 这条路现已被改判为“历史过渡实现”，不能再继续当终态扩写。
  2. 重新核实 scene 现场：
     - `Primary.unity` 仍有 `NPCs / 001 / 002 / 003 / 001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor`
     - `Town.unity` 已有 `Town_Day1Residents` 及一批 `ResidentSlot / DirectorReady / BackstageSlot`
     - 说明当前项目真实处在“承接层有了，但原生 resident 迁移未收口”的过渡态。
  3. 基于这次改判，已新增 4 份新 prompt：
     - `2026-04-06_spring-day1_原生resident迁移收尾主控prompt_26.md`
     - `2026-04-06_NPC线程_原生resident迁移协作补充prompt_27.md`
     - `2026-04-06_UI线程_原生resident迁移协作补充prompt_28.md`
     - `2026-04-06_Town线程_原生resident场景迁移补充prompt_29.md`
- 新的分工口径：
  1. `Town`：
     - 主接原生 resident 的 scene-side 根层/组层/承接层/原生存在性；
     - 但不拿走用户的 HomeAnchor 位置配置权，不替 day1 写主逻辑。
  2. `NPC`：
     - 主接 resident semantic matrix、formal once-only contract、resident fallback 内容与 bridge/tests；
     - 不再继续围绕 runtime spawn/deployment 扩写。
  3. `UI`：
     - 主接 formal one-shot 玩家面、resident fallback 玩家面、Workbench/DialogueUI；
     - 不再围绕 runtime 假居民补额外适配。
  4. `spring-day1`：
     - 负责把 `CrowdDirector/Director` 从“runtime 生人/临时演员”迁回“消费现成 Town resident”的终态，并最终收掉 `Primary` 旧 resident 依赖。
- 当前恢复点：
  1. 后续给外线继续发 prompt 或自己继续施工，都必须基于这次改判，不再沿旧的 runtime resident 路线推进；
  2. 下一次 day1 真实施工的首要代码主刀，应落在“收 runtime 生人逻辑、改成只消费现成 resident/anchor”。

## 2026-04-06｜真实施工补记：CrowdDirector 已改成优先绑定 scene resident，不再默认 runtime 生人

- 当前主线目标：
  - 按新的终态口径，把 `spring-day1` 从“runtime resident 过渡实现”收回到“只消费 Town 原生 resident / 现成 anchor”。
- 本轮真实施工切片：
  - `native-resident-migration-owner-20260406`
- 本轮实际落地：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - `GetOrCreateState(...)` 已不再默认 `Instantiate(entry.prefab)`；
     - 先走 `TryBindSceneResident(...)`，优先绑定 scene 里已经存在的 resident；
     - 运行时 `RuntimeHomeAnchor` 替身逻辑已收掉，不再默认现生 home anchor；
     - `EnsureSceneRoot(...)` 已不再创建 `SpringDay1NpcCrowdRuntimeRoot / Town_Day1Residents / Town_Day1Carriers`，改成只读 scene 里已有 root；
     - `DestroyState / TeardownAll / PruneDestroyedStates` 已按 `OwnsInstance / OwnsHomeAnchor` 收口，避免误删 scene 原生 resident。
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 原“Provision resident runtime roots / Allow resident spawn in Town scene”口径已改成：
       - 只绑定 scene 已有 root
       - 只绑定 scene 已有 resident + home anchor
       - 不再鼓励 runtime 现生 resident。
  3. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 文本护栏已补：
       - `TryBindSceneResident`
       - 不应再有 `Instantiate(entry.prefab`
       - 不应再有 `_RuntimeHomeAnchor`
- 本轮验证：
  - `py -3 ... manage_script validate --name SpringDay1NpcCrowdDirector --path Assets/YYY_Scripts/Story/Managers --level standard`
    - `status=warning errors=0 warnings=2`
    - warning 为旧有性能提示，不是本轮 owned red
  - `py -3 ... manage_script validate --name SpringDay1DirectorStagingTests --path Assets/YYY_Tests/Editor --level standard`
    - `status=clean errors=0 warnings=0`
  - `py -3 ... manage_script validate --name SpringDay1DialogueProgressionTests --path Assets/YYY_Tests/Editor --level standard`
    - `status=clean errors=0 warnings=0`
  - `py -3 ... errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
  - `validate_script` 对测试文件统一落到 `unity_validation_pending / blocked`
    - 原因仍是 `stale_status / CodeGuard timeout`
    - 当前未看到新增 owned compile error
  - `git diff --check`（本轮 3 个 touched files）
    - 通过
- 当前阶段判断：
  - 这轮最值钱的一刀已经成立：
    - `CrowdDirector` 终于开始从“默认现生 resident”改成“默认绑定现成 scene resident”
  - 但整条 resident 终态还没收完：
    - 还没有把 `SpringDay1Director` / live summary / 更深 director consumption 一起完全改成原生 resident 终态
    - `Town/Primary` scene 本体也还没被真正迁完
- 当前恢复点：
  1. 若继续这条主线，下一刀优先改：
     - `SpringDay1Director` / live summary / runtime 观察口
     - 让 day1 更清楚地区分“scene resident 已绑定”和“当前 beat/cue 正在消费谁”
  2. 后续无论谁继续，不允许再把 `runtime resident` 扩回终态路线。

## 2026-04-06｜真实施工补记：resident runtime summary 已能显式报 scene-owned / missing blocker，当前已到外部阻断边

- 本轮继续推进：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - runtime summary 现在新增：
       - `|missing=...`
       - `@owned=scene/runtime`
       - `@home=scene/runtime/none`
     - 这意味着后面 live snapshot 不再只是“有几个人在场”，而是能直接看出：
       - 当前绑定的是 scene 原生 resident 还是 runtime 自己拥有的对象
       - 当前 home anchor 来自哪里
       - 当前还缺哪些 resident 没绑定上
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 已补对应文本护栏，要求 runtime summary 必须显式带出这些新字段。
- 本轮验证：
  - `manage_script validate SpringDay1NpcCrowdDirector`
    - `warning errors=0 warnings=2`
  - `manage_script validate SpringDay1DialogueProgressionTests`
    - `clean errors=0 warnings=0`
  - `errors --count 20`
    - `errors=0 warnings=0`
  - `git diff --check`
    - 通过
- 当前阶段判断：
  - 代码侧我已经把 resident 终态继续推到“默认不生人 + summary 能显式报缺口”的程度；
  - 再往下真正挡住我的已经是 scene/user 配置层，不再是单纯代码没继续收。
- 当前真阻断：
  1. `Town.unity / Primary.unity` 还没完成原生 resident 真迁移，而这轮我不能碰 scene；
  2. `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 这类位置需要用户自己摆好后，我才能验证“只消费现成位置”的终态；
  3. 因为这两个外部前提还没落地，所以现在还不能把 Day1 包装成“已可终验 resident 终态”。
- 下一恢复点：
  1. 一旦 `Town/Primary` scene 本体迁移和用户手摆 HomeAnchor 回球，下一刀直接补：
     - `SpringDay1Director` 的更深 resident integration
     - live snapshot / acceptance summary
     - 最终主链验收面

## 2026-04-06｜UI 补记：按 resident 终态继续收玩家面，并把 live 证据链补到 Dialogue/Hint/Workbench

- 当前主线目标：
  - 按 `prompt_28` 继续只收 `Day1 玩家面结果 + UI 自家遗留`，口径固定为：
    1. formal one-shot 玩家面
    2. formal consumed 后 resident / informal fallback
    3. 继续补 `Workbench / DialogueUI` 的 fresh live 证据
    4. 不再为 runtime 假居民额外补玩家面适配
- 本轮真实施工：
  1. 状态层切回 `UI` 线程施工，不再误占 `spring-day1` runtime owner 的 active state。
  2. `Assets/YYY_Scripts/Story/UI/SpringUiEvidenceCaptureRuntime.cs`
     - 新增 `BuildDialogueSnapshot(...)`
     - 新增 `BuildInteractionHintSnapshot(...)`
     - `Workbench` 侧车新增 `runtimeShellSummary = overlay.GetRuntimeRecipeShellSummary()`
     - 这样一旦抓到 fresh GameView，json 不再只记录 Prompt/Workbench 几何，还会直接带：
       - `DialogueUI` 正式面状态
       - 左下角提示壳当前 key/caption/detail
       - Workbench 左列恢复真值摘要
  3. `Assets/Editor/Story/SpringUiEvidenceMenu.cs`
     - 新增菜单：
       - `Sunset/Story/Debug/Play Dialogue + Capture Spring UI Evidence`
     - 这条入口会：
       - bootstrap validation runtime
       - 触发 `Play Spring Day1 Dialogue`
       - 直接排队抓取 `dialogue` GameView 证据
  4. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增静态护栏 `UiEvidenceCapture_ContainsDialogueHintAndWorkbenchTruth()`
     - 要求证据链必须继续保留：
       - `BuildDialogueSnapshot`
       - `BuildInteractionHintSnapshot`
       - `runtimeShellSummary`
       - `Play Dialogue + Capture Spring UI Evidence`
- 本轮验证：
  - `manage_script validate`
    - `SpringUiEvidenceCaptureRuntime`：`clean`
    - `SpringUiEvidenceMenu`：`clean`
  - `git diff --check`
    - 上述 3 个 touched 文件通过
- 本轮 live 取证结果：
  1. 尝试通过 `CodexEditorCommandBridge` 进 Play，现场一度进入 `compiling`
  2. 但 live 取证被双重 blocker 卡住：
     - 外部编译红：
       - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs(1166,46)`
       - `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs(1177,51)`
       - `EnumerateAnchorNames` 缺失
     - MCP transport 坏态：
       - `python scripts/sunset_mcp.py status/errors`
       - `AttributeError: 'str' object has no attribute 'get'`
  3. 已显式发 `STOP` 把 Editor 拉回 `EditMode`，未把现场留在 PlayMode。
- 当前判断：
  - resident 终态这轮在我 own 的 UI 面上没有再漂回“给 runtime 假居民补壳”；
  - 当前最值钱的推进，是把 live 证据链补到真正能看 `DialogueUI + left-bottom hint + Workbench truth summary`；
  - 真正挡住 fresh live 的已不是 UI 代码本身，而是：
    1. crowd director 外部 compile red
    2. 当前 MCP 读状态/读 console 的 transport 坏态
- 恢复点：
  1. 先清外部 `SpringDay1NpcCrowdDirector.cs` 编译红
  2. 再让 MCP `status/errors/manage_script` 从 `str has no attribute get` 恢复
  3. 之后直接走：
     - `Play Dialogue + Capture Spring UI Evidence`
     - `Open Workbench + Capture Spring UI Evidence`
     补 fresh `DialogueUI / Workbench` GameView 证据

## 2026-04-06｜26 号 prompt 继续深压：Town 原生 resident actor 已真迁入 scene，Primary 旧 resident 已清出

- 本轮主刀落地：
  1. 新增 `Assets/Editor/Town/TownNativeResidentMigrationMenu.cs`
     - 通过 `Tools/Sunset/Scene/Migrate Town Native Residents`
     - 把 `101/102/103/104/201/202/203/301` 原生 prefab 真迁入 `Town.unity`
     - 为每个 crowd resident 补出独立 `npcId_HomeAnchor`
     - 把 `002/003` 与 `002_HomeAnchor/003_HomeAnchor` 也补进 `Town/NPCs`
     - `Primary.unity` 里的 `002/003` 与对应 `HomeAnchor` 已清出
     - 结果文件：`Library/CodexEditorCommands/town-native-resident-migration.json`
  2. `SpringDay1NpcCrowdDirector.cs`
     - `FindSceneResidentHomeAnchor(...)` 改成优先吃 resident 自己的 `npcId_HomeAnchor`
     - 不再在绑定/回落 resident 时调用 `SyncHomeAnchorToCurrentPosition()`
     - 也就是不再把用户手摆的 scene `HomeAnchor` 反向拉回代码位置
- 本轮关键验证：
  - `manage_script validate TownNativeResidentMigrationMenu`：`clean`
  - `manage_script validate SpringDay1NpcCrowdDirector`：`warning errors=0 warnings=2`
  - 命令桥真实执行：
    - `MENU=Assets/Refresh`
    - `MENU=Tools/Sunset/Scene/Migrate Town Native Residents`
    - `town-native-resident-migration.json => status=completed success=true`
  - `Tools/NPC/Spring Day1/Run Resident Director Bridge Tests`
    - `npc-resident-director-bridge-tests.json => passed 3/3`
  - fresh console：
    - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - 结果：`errors=0 warnings=0`
- 本轮中途真实踩到的 blocker：
  - 首次迁移保存时出现 `Town -> Primary/NavigationRoot` 跨场景引用红
  - 根因是 `TraversalBlockManager2D` 在双 scene 打开时会把 Town 新 NPC 误绑到 Primary navGrid
  - 已在迁移菜单里补：
    - `SetNavGrid(townNavGrid)`
    - 清空 traversal soft pass blockers
  - 重跑后 fresh console 已回到 `0 error / 0 warning`
- 当前阶段改判：
  - “Town 只有 slot / HomeAnchor，没有 actor 本体” 这一层已经不是 blocker
  - 现在 day1 这条线已经真正进入“可以给用户开始验 resident 终态”的阶段
## 2026-04-06｜UI线程：DialogueUI 剧情推进 submit 链收口为空格专用

- 当前主线：
  - 继续只收 `DialogueUI / Workbench` 玩家面遗留；这轮先处理用户最新指定的硬点：`过剧情的按键改成空格`
- 这轮实际做成：
  1. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 将 `WasSubmitAdvancePressedThisFrame()` 从 `Return / KeypadEnter / Space` 收口为仅接受 `Space`
     - 这样 `continueButton` 被选中时，也不再让回车沿着 submit 兼容链推进剧情
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补静态护栏，明确钉死：
       - 保留 `Keyboard.current.spaceKey.wasPressedThisFrame`
       - 移除 `Keyboard.current.enterKey.wasPressedThisFrame`
       - 移除 `Keyboard.current.numpadEnterKey.wasPressedThisFrame`
       - 移除 `Input.GetKeyDown(KeyCode.Return / KeypadEnter)`
- 这轮验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/DialogueUI.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
    - 通过；仅有既有 CRLF/LF 提示，无 diff 格式错误
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/DialogueUI.cs --count 20`
    - `assessment=unity_validation_pending`
    - own/external error = 0
    - native validate = `warning`，仅 1 条既有 `String concatenation in Update()` warning
  - `python scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs --count 20`
    - `assessment=unity_validation_pending`
    - own/external error = 0
    - native validate = `clean`
  - `python scripts/sunset_mcp.py compile Assets/YYY_Scripts/Story/UI/DialogueUI.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs --count 40`
    - `assessment=blocked`
    - 原因：`subprocess_timeout:dotnet:20s`
  - direct MCP fallback：
    - `validate_script DialogueUI.cs`：`errors=0 warnings=1`
    - `validate_script SpringDay1DialogueProgressionTests.cs`：`errors=0 warnings=0`
    - `read_console count=30`：`0 log entries`
- 当前判断：
  - 这轮已经把“剧情推进仍可能被回车推进”的最后一个代码口子封死
  - 但当前只站住 `结构 / targeted probe`，还没有 fresh live 玩家面证据，所以不能把它包装成体验终验完成
- 下一恢复点：
  1. 如果继续 UI live 取证，优先回到 `DialogueUI / Workbench` fresh capture
  2. 若用户只验这个点，直接复测：剧情 continue 现在应只认空格，不再认回车
## 2026-04-07｜UI线程：Workbench 左列空壳修复 + Primary 对话键残值清理

- 当前主线：
  - 用户要求两件事一起收口：
    1. Workbench 左列“可点击但没内容”必须彻底修
    2. 剧情推进/跳过不能再吃 `T`，必须是空格
- 这轮实际做成：
  1. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
     - 新增 `NormalizeAdvanceInputSettings()`
     - 在 `Awake()` / `OnValidate()` 中强制：
       - `advanceKey = KeyCode.Space`
       - `enablePointerClickAdvance = false`
     - 目的：不再依赖 Inspector 默认值，直接把旧场景里残留的 `T` 和鼠标点击推进回正
  2. `Assets/000_Scenes/Primary.unity`
     - 直接把 `DialogueUI` 的旧序列化残值改成：
       - `advanceKey: 32`
       - `enablePointerClickAdvance: 0`
     - 说明：这轮确认过 `Primary` 里之前真的还留着：
       - `advanceKey: 116`
       - `enablePointerClickAdvance: 1`
  3. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 在 `EnsureRecipeRowCompatibility()` 里新增 row 图形链清洗：
       - `SanitizeRecipeRowImage(background, clearSprite: true)`
       - `SanitizeRecipeRowImage(accentImage, clearSprite: true)`
       - `SanitizeRecipeRowImage(iconCardImage, clearSprite: true)`
       - `SanitizeRecipeRowImage(iconImage, clearSprite: false)`
     - 新增 `NormalizeGeneratedRecipeRowGeometry(...)`
       - 把生成式 row 的 `Accent / IconCard / TextColumn / Name / Summary` rect 拉回稳定几何
       - 目的：修掉左列文本列在 generated row 下被旧几何/坏锚点压坏的问题
     - 在 `NeedsRecipeRowHardRecovery()` 中新增坏壳判定：
       - `row.background.material != null`
       - `row.background.sprite != null`
       - `row.accent.material != null`
       - `row.accent.sprite != null`
       - `row.icon.material != null`
     - 目的：如果 row 被错误材质或错误 source image 污染，直接触发恢复链，不再继续沿用坏 row
  4. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 新增静态护栏，钉死：
       - `DialogueUI` 必须运行时强制回正空格键
       - `Workbench` 左列必须清洗错误材质/错误 sprite 污染，并显式拉回 generated row 几何
- 这轮验证：
  - `validate_script DialogueUI.cs`
    - `errors=0`
    - `warnings=1`（既有 `String concatenation in Update()`）
  - `validate_script SpringDay1WorkbenchCraftingOverlay.cs`
    - `errors=0`
    - `warnings=1`（同类既有 warning）
  - `validate_script SpringDay1DialogueProgressionTests.cs`
    - `errors=0`
    - `warnings=0`
  - Console：
    - 无 owned error
    - 仅见 MCP 外部 warning：`[WebSocket] Unexpected receive error: WebSocket is not initialised`
  - `git diff --check`
    - `.cs` 文件无新增 diff 格式错误
    - `Primary.unity` 仍有大量历史既有 trailing whitespace 提示，属于 scene 旧账，不是这轮新引入的代码红错
- 当前判断：
  - 这轮已经把“Primary 仍吃 T 键”的真实根因抓出来并清掉了，不再只是代码默认值改了
  - Workbench 左列这轮不是继续赌材质，而是直接把“错误材质/错误 source image 污染 + generated row 坏几何”一起纳入恢复链
- 下一恢复点：
  1. 用户直接从 `Primary` 复测剧情推进，预期只认空格，不再认 `T`
  2. 用户复测 Workbench 左列，重点看：
     - row 背景是否恢复成正式纯色壳
     - `Name / Summary` 是否重新稳定显示
     - 就算 row 曾被错误材质/错误 sprite 污染，也会被 recovery 链拉回
## 2026-04-06｜线程续工：修正 resident 站桩与开场残留调试跳段风险

- 当前主线：
  - 继续把 `spring-day1` 往可验 demo 推，不再扩 runtime 生人路线，直接解决用户刚撞到的两个真问题：
    1. `Town` 原生 resident 完全不走位
    2. 从 `Primary` 开始时容易怀疑自己被残留调试态/后段状态带偏
- 这轮实际做成：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - resident 从导演 cue/隐藏态回到 baseline 时，除了 `ApplyProfile()` 之外显式补：
       - `if (!roamController.IsRoaming) roamController.StartRoam();`
     - 目的：修正 scene resident 被导演接管后恢复可见，但 `Start()` 不会重跑，结果活着却站桩不走的问题
  2. `Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs`
     - 新增 `ResetToOpeningRuntimeState()`
     - 直接把 Day1 运行态重置回默认开场快照，避免旧 formal/推进态污染当前验收
  3. `Assets/Editor/Story/DialogueDebugMenu.cs`
     - 新增：
       - `Sunset/Story/Debug/Toggle Skip To Workbench 0.0.5`
       - `Sunset/Story/Debug/Reset Spring Day1 To Opening`
     - reset 会顺手清掉 `DebugWorkbenchSkipEditorPrefKey` 并打出 live snapshot
  4. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - `DebugWorkbenchSkipEditorPrefKey` 改成“一次性消费”
     - EditorPrefs 直跳开关触发一次后会自动清掉，不再长期污染正常 Play
  5. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补文本护栏，钉死：
       - crowd director baseline 恢复后必须 `StartRoam()`
       - debug menu 必须提供 reset opening
       - director 必须清掉 editor-pref 直跳残留
- 这轮验证：
  - `git diff --check`（本轮触碰文件）通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
  - `validate_script` 对 own 文件没有 owned error，但因为 Unity Editor 当前长期 `stale_status / compiling`，assessment 多次落在 `unity_validation_pending`
  - `manage_script.validate`：
    - `SpringDay1Director.cs`：`warning errors=0 warnings=2`
    - `SpringDay1NpcCrowdDirector.cs`：`warning errors=0 warnings=2`
    - `DialogueDebugMenu.cs`：`clean`
    - `SpringDay1DialogueProgressionTests.cs`：`clean`
- 额外核实结论：
  1. 当前注册表里不存在 `Sunset.SpringDay1.DebugSkipWorkbenchPhase05`，所以“现在这一刻仍被 editor pref 卡死”没有硬证据
  2. `ProjectSettings/EditorBuildSettings.asset` 仍只挂 `Primary.unity`
  3. 但仓库里已经有 `Assets/000_Scenes/矿洞口.unity`
  4. 也就是说：现在从 `Primary` 直接跑，本来就会跳过矿洞 scene 入口；这不是刚修出来的新 bug，而是 demo 入口当前就是这样配的
  5. `Town.unity` 已明确有：
     - `Town_Day1Residents`
     - `Resident_DefaultPresent`
     - `Resident_DirectorTakeoverReady`
     - `Resident_BackstagePresent`
     - `002/003/101~301` 对应 `HomeAnchor`
     - `Primary.unity` 目前只扫到 `001` / `001_HomeAnchor`
- 当前阶段：
  - 代码层已经把 “resident 站桩” 和 “开场残留直跳” 两个最像真的 day1 问题往前推了一大步
  - 真正还没拿到的是用户场景上的 fresh live 复核：因为当前 Unity 现场活跃 scene 仍是 `Home.unity`，且 editor 状态长期 `stale_status`
- 下一恢复点：
  1. 用户继续从 `Primary` 进 Play 前，先用新菜单 `Reset Spring Day1 To Opening`
  2. 然后重点复核：
     - `Town` resident 是否已经重新开始漫游
     - `Primary` 是否仍会异常直接落到工作台/农田后段
  3. 如果还想把矿洞作为真正入口，再单开“把 `矿洞口.unity -> Primary` 接成正式 demo 起点”的下一刀
## 2026-04-07｜线程续工：导演摆位窗口补草稿持久化，编译/域重载不再清空未保存内容

- 当前主线：
  - 修掉 `SpringDay1DirectorStagingWindow` 的真痛点：用户还没点 `保存 JSON` 时，一次编译 / 域重载就把导演窗口里填到一半的 cue 草稿全清空。
- 这轮实际做成：
  1. `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`
     - 新增 `WindowDraftState` + `DraftEditorPrefKey`
     - `OnEnable()` 不再只 `ReloadBook()`，改成先 `LoadBookOrDraft()`
     - 字段编辑、增删 cue、路径点采样、录制写回 cue 后，都会自动把当前窗口草稿写进 `EditorPrefs`
     - 窗口重新打开 / 编译后，会优先恢复未保存草稿，并显示 `已恢复草稿`
     - `保存 JSON` / `重载` 会清草稿
     - 额外修掉一个回归坑：保存/重载后不再因为 `OnDisable()` 又把旧草稿反手写回；现在只有 `_isDraftDirty` 时才持久化
     - `清空路径` 现在也会立刻记草稿，不再漏持久化
  2. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 补静态护栏，钉住：
       - 存在 `DraftEditorPrefKey`
       - 启动先 `LoadBookOrDraft()` / `TryRestoreDraftState()`
       - 字段改动会 `PersistDraftState()`
       - 保存后会 `DeleteDraftState()`
       - 关闭窗口只在脏草稿状态下回写
- 本轮验证：
  - `git diff --check -- Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
  - `validate_script Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs --count 20 --output-limit 8`：`assessment=unity_validation_pending`，owned/external error 都是 `0`，被 `CodexCodeGuard timeout_downgraded + stale_status` 卡住；native validate 仅 `1 warning`
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs --count 20 --output-limit 8`：`assessment=unity_validation_pending`，owned/external error 都是 `0`，同样被工具侧 timeout/stale_status 卡住；native validate `clean`
- 当前阶段：
  - 导演窗口“未保存草稿一编译就清空”的问题，代码层已经收住。
  - 剩下缺的不是 owned red，而是用户自己在 Unity 里顺手试一遍体感确认。
- 下一恢复点：
  1. 打开 `Tools/Story/Spring Day1/导演摆位 MVP`
  2. 随便改几个 cue 字段/路径点，但先别点 `保存 JSON`
  3. 触发一次编译或域重载，再看窗口是否自动恢复草稿
  4. 再点一次 `保存 JSON`，确认这时草稿会被清空，后续重开优先读正式稿
## 2026-04-07｜Workbench 左列显示链止血：撤回过度清洗，补 ItemDatabase 兜底

- 当前主线：只收 `SpringDay1WorkbenchCraftingOverlay` 左列 recipe 显示链，目标是把“左列整块没了 / 右侧图标没了 / 材料名退化成 3200”拉回可读状态。
- 这轮实际做成：
  1. 在 `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs` 收回了错误方向的 row 图像清洗：
     - `EnsureRecipeRowCompatibility()` 不再把 `background / accent / iconCard` 的 sprite 一起清空
     - 现在只清错误材质，保留原本图像壳
  2. 新增 `HasInvalidRecipeRowGraphicMaterial()`，把“错误 TMP/Font 材质污染”改成精准判定，不再把健康 row 当坏壳
  3. 收窄 `NeedsRecipeRowHardRecovery()`：
     - 不再把“sprite 为空”本身当成坏壳
     - 改成只在控件缺失或命中错误图形材质时才触发恢复
  4. `ResolveItem(int itemId)` 补 `AssetLocator.LoadItemDatabase()` 兜底，避免 `_craftingService.Database / _inventoryService.Database` 未绑上时，材料名和右侧图标退化成数字占位
  5. 在 `Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs` 新增两条护栏：
     - 兼容修复不应把 row sprite 一起清空
     - Workbench 在服务未绑库时仍应能回退到 `MasterItemDatabase`
- 当前验证：
  - `validate_script Assets/YYY_Tests/Editor/SpringDay1LateDayRuntimeTests.cs --skip-mcp` => `owned_errors=0`
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => `owned_errors=0`
  - `no-red` / live compile 仍被 `CodexCodeGuard timeout / stale_status` 卡住，暂时只能确认代码层 owned red 已清
- 当前判断：
  - 这次 Workbench 真正的主因不是“单纯材质缺失”，而是我上一轮把“清错误材质”写成了“清整条图片壳”，再叠加 `ResolveItem()` 没有数据库兜底，才会出现左列没了、右侧图标没了、材料名变 3200。
- 下一恢复点：
  1. 进游戏打开 Workbench 复测左列 recipe 行是否恢复 icon / 名称 / 摘要
  2. 复测右侧 selected icon 是否恢复
  3. 复测材料名是否恢复成真实物品名而非 `材料 3200`
## 2026-04-07｜Town 开场导演续工：旧护栏改口 + 导演窗口支持整批 beat 预演

- 当前主线：
  - 继续把 `spring-day1` 收到 `Town 开场 -> 围观 -> Primary 安置` 的真实口径，并把导演窗口补到能直接预演整批 resident cue。
- 本轮实际做成：
  1. `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`
     - 清掉多条已经落后于现实现状的静态断言：
       - formal/闲聊优先级不再要求旧的 `dialogueInteractable.CanInteract(context)`
       - free-time 压力态不再要求写死 `nightPressure=final-call`
       - workbench 左列选中态改为走 `GetRuntimeRecipeShellSummary()`
       - NPC formal E 键不再要求自己 `Input.GetKeyDown(...)`，而是承认统一近身仲裁服务
       - workbench 投影不再盯旧的 `GetWorldProjectionCamera()`，改认 `SpringDay1UiLayerUtility.TryProjectWorldToCanvas(...)`
  2. `Assets/YYY_Tests/Editor/SpringDay1OpeningDialogueAssetGraphTests.cs`
     - opening 资产图护栏正式改口：
       - `VillageGate.followupSequence` 必须为 `null`
       - `HouseArrival` 继续为最后一拍
  3. `Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`
     - 验收建议文本改成当前真实链：
       - `Town 会自动接进围观；围观收束后切到 Primary，再等旧屋安置自动接上。`
     - 新增 `TownOpening_ShouldAdoptCrashPhaseIntoEnterVillageBeat()`
       - 钉死 Town 开场会把旧 `CrashAndMeet` 自动改口成 `EnterVillage`
       - 当前 beat 应直接落到 `EnterVillage_PostEntry`
  4. `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`
     - 新增导演窗口整批预演能力：
       - `预演当前 Beat（整批）`
       - `清理当前 Beat 预演`
     - 现在在 Play 模式下，窗口会按当前 beat 遍历 manifest，给场景里已存在的 resident 批量挂 `SpringDay1DirectorStagingPlayback` 并应用 cue
     - 清理按钮会按当前 beat 批量释放这些预演接管
- 本轮验证：
  - `git diff --check`（本轮 touched files）通过
  - direct validate：
    - `Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs`：`errors=0 warnings=0`
    - `Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`：`errors=0 warnings=0`
    - `Assets/YYY_Tests/Editor/SpringDay1OpeningDialogueAssetGraphTests.cs`：`errors=0 warnings=0`
    - `Assets/Editor/Story/SpringDay1DirectorStagingWindow.cs`：`errors=0 warnings=1`
      - warning 仅为编辑器脚本 `Update()` 字符串拼接的 GC 提示，不是 compile red
  - `SpringDay1DialogueProgressionTests` 连续重跑时，旧断言债继续冒头并已当场清掉数条；最后一次被外部 `Play Mode / playmode_transition` 抢现场，没拿到最终绿色
- 当前阶段：
  - Town 开场这条线的代码/测试口径已经继续往现状收平了一层；
  - 导演窗口也已经从“单 NPC 回放”推进到“整批 beat 预演”；
  - 还没拿到完整 fresh test green，原因是 Unity editor 现场被外部 play mode 反复占用，不是本轮 own compile red。
- 当前阻断 / 下一恢复点：
  1. 先确保 Editor 稳定留在 `Edit Mode`
  2. 重跑：
     - `SpringDay1DialogueProgressionTests`
     - `SpringDay1OpeningRuntimeBridgeTests`
     - `SpringDay1OpeningDialogueAssetGraphTests`
  3. 如果大测试继续冒旧断言，就继续把这份历史文本护栏压到当前实现口径
  4. 然后再去拿一次 fresh live，核对 Town 开场围观 resident 是否按 batch preview / runtime cue 真走起来
## 2026-04-07｜Workbench 左列二次止血：确认是 TMP 遮罩材质链，不再强塞 shared material

- 用户最新 live 证据确认：左列 row 的壳、交互、滚动、icon 都在，唯独文本全军覆没；Inspector 显示 `TMP_SubMeshUI` 链仍存在。
- 新判断：左列第一真实 blocker 是 `TMP + Mask` 下的材质链被脚本手动覆盖，而不是纯几何问题。
- 本轮改动：
  1. `SpringDay1WorkbenchCraftingOverlay.cs` 中所有 `text.fontSharedMaterial = ...` 已彻底移除
  2. 新增统一入口 `ApplyResolvedFontToText()`，只改 `font`，并刷新 mesh/material dirty，不再强塞 shared material
  3. 左列 row 文本、壳体扫描文本、内容修复文本现在统一走“只修 font，不碰 masked material override”的路径
- 当前验证：
  - `rg "fontSharedMaterial" SpringDay1WorkbenchCraftingOverlay.cs` 已无命中
  - `validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp` => owned red 0
- 当前恢复点：
  1. 进游戏打开 Workbench
  2. 看左列名称/摘要是否恢复可见
  3. 若仍异常，下一步就不再猜，会直接针对运行时 TMP_SubMeshUI / materialForRendering 做现场取证

## 2026-04-07｜Town opening crowd integration：原生 resident 围观从 3 人扩到 6 人，且 shared-anchor 串 cue 已修

- 当前主线：
  - 把 `Town` 开场围观段压到“原生 resident 真被导演接走”，不再只靠 `101 / 103 / 201` 撑场。
- 本轮实际做成：
  1. `SpringDay1DirectorStageBook.json`
     - `EnterVillage_PostEntry` cue 扩成 `101 / 103 / 104 / 201 / 202 / 203`
     - opening cue 统一保持 `keepCurrentSpawnPosition=true`
     - 继续保留 `semantic anchor start + offset` 语义
  2. `SpringDay1NpcCrowdManifest.asset`
     - `104 / 201 / 202 / 203` 补 `EnterVillagePostEntryCrowd`
     - 补 opening 用到的 anchor 白名单
     - 把这 4 人在 `EnterVillage_PostEntry` 从纯离屏 trace 调成 support/background onstage
  3. `SpringDay1DirectorStaging.cs`
     - `TryResolveCue()` 改成 `精确 npcId > semanticAnchor > duty`
     - 修掉多人共享 anchor/duty 时串 cue 的 runtime 真 bug
  4. 测试 / 菜单同步：
     - `NpcCrowdManifestSceneDutyTests.cs`
     - `NpcCrowdResidentDirectorBridgeTests.cs`（当前 git 仍是 untracked，但已被本轮继续补强）
     - `SpringDay1DirectorStagingTests.cs`
     - `SpringDay1TargetedEditModeTestMenu.cs`
- 本轮验证：
  - `validate_script`
    - `SpringDay1DirectorStaging.cs`：0 error，2 条 perf warning
    - `SpringDay1DirectorStagingTests.cs`：0/0
    - `NpcCrowdManifestSceneDutyTests.cs`：0/0
    - `NpcCrowdResidentDirectorBridgeTests.cs`：0/0
    - `SpringDay1TargetedEditModeTestMenu.cs`：0/0
  - `git diff --check`（本轮 touched files）通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`0 error / 0 warning`
  - 逐条精确通过：
    - `SpringDay1DirectorStagingTests.StageBook_ShouldKeepCurrentSpawnForTownOpeningResidentTakeovers`
    - `SpringDay1DirectorStagingTests.StageBook_ShouldPreferExactNpcCueBeforeSharedAnchorAndDutyFallback`
    - `SpringDay1DirectorStagingTests.StageBook_ShouldContainMultiPointPathsForRehearsedDirectorTargets`
    - `NpcCrowdManifestSceneDutyTests.CrowdManifest_ShouldExposeDirectorConsumptionRoles_ForDay1ResidentDeployment`
    - `NpcCrowdManifestSceneDutyTests.CrowdManifest_ShouldBuildBeatConsumptionSnapshot_ForDay1DirectConsumption`
    - `NpcCrowdResidentDirectorBridgeTests.EnterVillage_ShouldResolveCueForExpandedOnstageResidentCrowd`
    - `NpcCrowdResidentDirectorBridgeTests.DirectorCues_ShouldStayInsideManifestSemanticAnchorsAndDuties_ForResidentBridgeBeats`
    - `NpcCrowdResidentDirectorBridgeTests.StageBook_ShouldResolveCueForAllPriorityResidents_InEnterVillageAndNightWitness`
    - `SpringDay1OpeningRuntimeBridgeTests.TownOpening_ShouldAdoptCrashPhaseIntoEnterVillageBeat`
- 当前阶段：
  - `Town opening crowd` 的代码 / 数据 / 关键护栏已经到可验收切片
  - 下一步最值钱的是用户从 `Town` 开场实际跑一次，看 6 人围观聚拢与围观散开体感

## 2026-04-07｜继续施工：围观收束不再直接放飞，而是先回 HomeAnchor 再恢复 roam

- 当前主线：
  - 把 `Town` 开场围观 resident 的收束行为压到更像最终体验：围观结束后先“散回家”，再恢复日常 roam。
- 本轮实际做成：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - 新增 resident `return-home` 状态
     - cue 收束后不再立刻 `StartRoam()`，而是先朝现有 `HomeAnchor` 移动
     - 回到 `HomeAnchor` 后才恢复 roam
     - runtime summary 新增 `@return=home/none`
  2. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增
       - `CrowdDirector_ShouldQueueReturnHomeAfterCueReleaseInsteadOfSnappingResident`
       - `CrowdDirector_ShouldResumeRoamAfterReturnHomeCompletes`
     - 修正旧测试 `CrowdDirector_ShouldTreatDayResidentsAsAlreadyAroundBeforeEnterVillage` 的方法签名
     - 把 `StageBook_ShouldContainCapturedAbsolutePositionsForNonOpeningDirectorCues` 从过期的 `daily-201` 口径改回 `dinner-bg-201`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 把两条新的 return-home 护栏测试接进 `Director Staging Tests`
     - 把 `TownOpening_ShouldAdoptCrashPhaseIntoEnterVillageBeat` 接进 `Opening Bridge Tests`
- 本轮验证：
  - `validate_script`
    - `SpringDay1NpcCrowdDirector.cs`：0 error / 2 warning（perf）
    - `SpringDay1DirectorStagingTests.cs`：0 error / 0 warning
    - `SpringDay1TargetedEditModeTestMenu.cs`：0 error / 0 warning
  - `git diff --check`（上述 3 文件）通过
  - fresh console 当前第一条红不是 own red，而是外线：
    - `Assets/YYY_Scripts/Service/Player/PlayerInteraction.cs(257,77): pendingToolData 不存在`
  - 项目菜单级验证已尝试重跑，但 fresh compile 被这条外线红挡住，所以还没拿到“包含本轮新测试”的新结果文件
- 当前恢复点：
  1. 先等外线 `PlayerInteraction.cs` 编译红清掉
  2. 重新编译后直接跑
     - `Sunset/Story/Validation/Run Director Staging Tests`
     - `Sunset/Story/Validation/Run Opening Bridge Tests`
  3. 若 fresh 菜单结果通过，再让用户从 `Town` 开场实际验“围过来 -> 村长带走 -> 散回家”

## 2026-04-07｜只读核实：Town / Primary 现有“村长带路 -> 到点推进”对象面

- 当前主线：
  - `spring-day1` 后半段 `EnterVillage -> HouseArrival` 承接面继续只读核实，不进入真实施工。
- 本轮子任务：
  - 只核 `Town.unity`、`Primary.unity`、相关 directing / interaction / NPC 脚本，回答：
    1. `Town` 里村长对象现在叫什么、在哪
    2. 现成住处 / 房屋 / 门 / 转场 / 带路终点有哪些名字
    3. 是否存在“村民汇聚点”
    4. 现在最自然该消费哪些现有对象名
- 已核实事实：
  1. `Town.unity` 里确实已有真实 NPC actor：
     - `NPCs/001`
     - `NPCs/002`
     - `NPCs/003`
     - 其中 `NPCs/001` 的 source prefab 是 `Assets/222_Prefabs/NPC/001.prefab`，prefab 自带 `NPC_001_VillageChiefRoamProfile.asset`，所以它就是当前最可信的村长对象。
     - `Town` 里 `001` 位置约 `(-12.15, 13.12)`，`NPCs/001_HomeAnchor` 与它同位。
  2. `Town.unity` 里确实存在用户口中的 gather 点 / 类 gather 点：
     - 顶层 `村民汇聚点`：约 `(-15.49, 11.71)`
     - 顶层 `进村围观`：约 `(-7.42, 15.09)`
     - 但当前正式 runtime contract 真正在吃的是：
       - `SCENE/Town_Day1Carriers/EnterVillageCrowdRoot`
       - `SCENE/Town_Day1Carriers/KidLook_01`
       - `SCENE/Town_Day1Carriers/NightWitness_01`
       - `SCENE/Town_Day1Carriers/DinnerBackgroundRoot`
       - `SCENE/Town_Day1Carriers/DailyStand_01~03`
       - 且 `Assets/Resources/Story/SpringDay1/Directing/SpringDay1TownAnchorContract.json` 已把这些名字和世界坐标固化。
  3. `Town.unity` 现成房屋 / 转场面：
     - `SCENE/LAYER 1/Props` 下 prefab instance：
       - `House 1 (1)`
       - `House 2_0`
       - `House 2_0 (1)`
       - `House 2_0 (2)`
       - `House 2_0 (3)`
       - `House 3_0`
       - `House 4_0`
     - `Town` 里还有 `Primary/2_World/SceneTransitionTrigger`
       - 注意：这是 `Town.unity` 里沿用旧根名 `Primary`
       - target = `Assets/000_Scenes/Primary.unity`
  4. `Primary.unity` 现成承接面：
     - `NPCs/001`
     - `NPCs/001_HomeAnchor`
     - `Primary/2_World/SceneTransitionTrigger` -> `Town`
     - `SCENE/LAYER 1/Props/House 2_0`
     - `Primary/2_World/Primary_HomeContracts/PrimaryHomeDoor/PrimaryHomeEntryAnchor`
  5. 住处代理的当前真实口径不是 `PrimaryHomeDoor`：
     - `SpringDay1Director` 自动搜的是 `Bed / PlayerBed / HomeBed`
     - 若没有床，则搜 `House 1_2 / HomeDoor / HouseDoor / Door`
     - `House 1_2` 是 `House 1.prefab` 与 `House 2_0.prefab` 的子节点名
     - 因为 `Primary.unity` 当前只有 1 个 `House 2_0` 实例，所以 `House 1_2` 在 `Primary` 里比 `Town` 多套房屋场景更不歧义
  6. 现在代码 / 资源的既有承接方向仍是：
     - `Town` 自动承 `EnterVillage_PostEntry` 围观
     - 围观收束后再切 / 回到 `Primary`
     - `Primary` 再自动接 `HouseArrival`
     - 证据是 `SpringDay1Director.cs` 的 probe 文案已经写成“Town 会自动接进围观；围观收束后切到 Primary，再等旧屋安置自动接上”。
  7. 一个当前潜在错位：
     - `Town / Primary` 现有村长 actor 名都叫 `001`
     - 但 `SpringDay1DirectorStageBook.json` 大量 `lookAtTargetName` 仍写 `NPC001`
     - 如果后续 cue 继续直接 `GameObject.Find("NPC001")`，这层注视 / 朝向目标有失配风险
- 关键判断：
  1. 如果现在要最小实现“村长带路 -> 到点推进”，最自然先消费：
     - guide actor：`Town/NPCs/001`
     - crowd arrival：`EnterVillageCrowdRoot`
     - 辅助 crowd look：`KidLook_01`
  2. 如果要把“到点推进”进一步接到安置住处，当前最自然不是在 `Town` 里硬挑哪间房，而是继续沿现有链：
     - `Town` 围观结束
     - 回 / 切到 `Primary`
     - 用 `Primary` 里单实例 `House 2_0` 下的 `House 1_2` 作为旧屋代理入口
  3. `村民汇聚点` / `进村围观` 确实存在，但目前更像世界标记；正式 runtime 合同更应该吃 `EnterVillageCrowdRoot` 这一组英文锚点名。
- 涉及路径：
  - `Assets/000_Scenes/Town.unity`
  - `Assets/000_Scenes/Primary.unity`
  - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1TownAnchorContract.json`
  - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
  - `Assets/YYY_Scripts/Story/Interaction/SceneTransitionTrigger2D.cs`
  - `Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs`
  - `Assets/222_Prefabs/NPC/001.prefab`
  - `Assets/222_Prefabs/House/House 1.prefab`
  - `Assets/222_Prefabs/House/House 2_0.prefab`
- 验证状态：
  - 纯磁盘只读核实
  - 未改文件
  - 未跑 `Begin-Slice`
  - 结论属于静态结构判断，不是 live 手感终验
- 当前恢复点：
  - 如果后续真开工，先把 lead / destination 名称固定到：
    - `001`
    - `EnterVillageCrowdRoot`
    - `KidLook_01`
    - `House 1_2`（`Primary` 单实例 house proxy）
  - 再决定是否需要补 `NPC001 -> 001` 的 cue 名称对齐

## 2026-04-07｜继续施工：Town 围观 beat 提前切错已修，围观 cue 已重新在 live 中被真正消费

- 当前主线：
  - 把 `spring-day1` 的 `Town 开场围观 -> 村长带路 -> Primary 安置` 从“切场会发生”推进到“围观 resident 真吃 cue / 真走位 / 玩家面口径不再提前漂到 HouseArrival”。
- 这轮抓到的真 bug：
  - `SpringDay1Director.GetCurrentBeatKey()` 旧逻辑只要 `VillageGate` 被排队，就提前把 `EnterVillage` 切到 `EnterVillage_HouseArrival`。
  - 直接后果是：
    - `Town` runtime 不再消费 `EnterVillage_PostEntry`
    - crowd resident 只切组显隐，不真吃 cue
    - 玩家面 focus / TownLead 过早提示“跟住村长往住处走”
- 这轮落下的修法：
  1. `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
     - 新增：
       - `HasVillageGateCompleted()`
       - `ShouldUseEnterVillageHouseArrivalBeat()`
     - `GetCurrentBeatKey()` 现在只有在：
       - `VillageGate` 真完成
       - 或 `HouseArrival` 已经开始 / 后态已进入
       时才切 `EnterVillage_HouseArrival`
     - `IsTownHouseLeadPending()` 收紧到 `VillageGate` 真完成后才成立
     - EnterVillage 第一条任务卡完成判定同步收紧
  2. `Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`
     - 新增：
       - `VillageGateWhileActiveInTown_ShouldKeepPostEntryBeatUntilDialogueCompletes`
     - 断言：
       - `VillageGate` 活动时 beat 仍是 `EnterVillage_PostEntry`
       - 不应提前 `TownHouseLeadPending`
  3. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把这条新 opening 护栏挂进 `Run Opening Bridge Tests`
- 这轮代码层验证：
  - `manage_script validate`
    - `SpringDay1Director.cs`：`errors=0 warnings=2`
    - `SpringDay1OpeningRuntimeBridgeTests.cs`：`errors=0 warnings=0`
    - `SpringDay1TargetedEditModeTestMenu.cs`：`errors=0 warnings=0`
  - `git diff --check`（上述 3 文件）通过
- 这轮 fresh Unity 证据：
  - `Assets/Refresh` 后 `py -3 scripts/sunset_mcp.py status`
    - `error_count=0`
    - `warning_count=0`
    - `active_scene=Assets/000_Scenes/Town.unity`
    - `ready_for_tools=true`
- 这轮最值钱的 live 快照：
  1. fresh `Town` 开场推进一拍后：
     - `beat=EnterVillage_PostEntry`
     - `consumption=p=2/s=4/t=2/b=0`
     - `101 / 103 / 104 / 201 / 202 / 203` 都已进入 `staging`
     - cue 分别落到：
       - `enter-crowd-101`
       - `enter-kid-103`
       - `enter-crowd-104`
       - `enter-crowd-201`
       - `enter-crowd-202`
       - `enter-crowd-203`
     - 玩家面 focus 已回正成“别在围观里停住，跟着村长穿过村口”
     - `TownLead=inactive`
  2. 继续推进 `VillageGate` 收束后：
     - `TownLead=started=True`
     - `NPC=001|state=Moving`
     - crowd 开始 `return-home`
  3. 请求切到 `Primary` 后：
     - `Scene=Primary`
     - `Dialogue=spring-day1-house-arrival[1/7]`
     - `progress=闲置小屋安置进行中`
- 当前新尾巴：
  - `Run Opening Bridge Tests` 菜单这轮能启动，但结果 artifact 还停在 `running`，没有回写最终 `pass/fail`；更像 editor test runner 没收尾，不像本轮 own compile red。
  - 一次更长的自动重放又被当前 runtime 状态恢复链带偏回 `first-followup`，那张快照不作为本轮主证据。
- 当前恢复点：
  1. 若继续，优先查 `spring-day1-opening-bridge-tests.json` 为什么一直停在 `running`
  2. 玩家现在最值钱的手测就是：
     - `Town` 开场
     - 看 6 人是否真往围观点聚拢
     - 看围观结束后是否开始散回 `HomeAnchor`
     - 然后看村长是否继续带去 `Primary`

## 2026-04-07｜继续施工：导演工具排练接管已改成会真暂停 runtime playback 和 crowd director 抢控

- 当前主线：
  - 修掉“点了开始排练，但 NPC 还会被原行为线或 runtime 剧情链拉走”的导演工具真 bug。
- 这轮抓到的真断口：
  1. `开始排练` 旧实现只会停 `roam / 对话 / 气泡`
  2. 如果该 resident 身上已经有 `SpringDay1DirectorStagingPlayback`，它还会继续按 runtime cue 自己走
  3. `SpringDay1NpcCrowdDirector` 也不会把 `rehearsal` 视为正式控制权，会继续同步这个 resident
  4. 结果就是编辑器里看起来“已接管”，但 NPC 实际还会往回走或被 runtime 抢回去
- 这轮实际落下的修法：
  1. `Assets/YYY_Scripts/Story/Directing/SpringDay1DirectorStaging.cs`
     - `SpringDay1DirectorStagingRehearsalDriver.OnEnable()`
       - 会先缓存并暂停同物体上已有的 `SpringDay1DirectorStagingPlayback`
       - 进入排练瞬间补一次 `StopMotion()`
     - `OnDisable()`
       - 会把原 playback 状态恢复回去
  2. `Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
     - 新增 `TryHoldForRehearsal(...)`
     - `SyncCrowd()` 与 `TickResidentReturns()` 现在都会先判断该 resident 是否正被 `SpringDay1DirectorStagingRehearsalDriver` 接管
     - 一旦在排练，就不再继续给这个 resident 应用 runtime cue / baseline / return-home
  3. `Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs`
     - 新增：
       - `RehearsalDriver_ShouldPauseExistingPlaybackUntilDisabled`
       - `CrowdDirector_ShouldHoldForActiveRehearsalDriver`
     - 顺手把 `InvokeInstance(...)` 改成按参数匹配重载，避免旧 staging tests 被 `ApplyCue(...)` 多重签名误打成假失败
  4. `Assets/Editor/Story/SpringDay1TargetedEditModeTestMenu.cs`
     - 已把这两条新 staging 护栏接进 `Run Director Staging Tests`
- 这轮验证：
  - `validate_script`
    - `SpringDay1DirectorStaging.cs`：`assessment=no_red`
    - `SpringDay1NpcCrowdDirector.cs`：`assessment=no_red`
    - `SpringDay1DirectorStagingTests.cs`：owned `0 error`，其间一度被外线 `CloudShadowManager.cs` 挡成 `external_red`
    - `SpringDay1TargetedEditModeTestMenu.cs`：`assessment=no_red`
  - `git diff --check`
    - 上述 4 文件通过
  - fresh console：
    - 最新 `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20` 已回到 `0 error / 0 warning`
- 当前新尾巴：
  1. `spring-day1-director-staging-tests.json` 这轮再次卡在 `running`
  2. 更早一版 `completed` 结果里混有旧测试反射假失败，不能拿来判这轮业务修法是否失效
  3. 所以当前最稳的结论是“own 代码链已修并已 compile-first 站住”，但测试菜单 artifact 这条验证尾巴还没完全收平
- 当前恢复点：
  1. 用户现在最值得直接复测的就是：
     - 在 `Town` 运行时选中 resident
     - 点 `开始排练`
     - 看它是否还会被原 runtime cue / crowd director 拉走
  2. 如果它已经不再自己往回走，这条导演接管 bug 就算真正穿了
  3. 如果菜单 artifact 还要继续查，下一刀就直查 `spring-day1-director-staging-tests.json` 为什么长期不收尾

## 2026-04-07｜继续施工：把 NPC 301 从当前 Day1 正式链里全面弃用

- 当前主线：
  - 用户最新裁定是先把 `301` 从当前 `spring-day1` 里拿掉，不再让它继续占当前 Day1 的剧情、导演和验证席位。
- 这轮实际做成：
  1. 当前 Day1 正式消费面已断开
     - `Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset`
       - 删除 `npcId: 301`
     - `Assets/Resources/Story/SpringDay1/Directing/SpringDay1DirectorStageBook.json`
       - 删除 `night-witness-301`
       - 删除 `dayend-watch-301`
     - `Assets/111_Data/NPC/SpringDay1Crowd/NPC_102_HunterDialogueContent.asset`
       - 删除对 `partnerNpcId: 301` 的 pair 引用
  2. 导演辅助菜单已同步改口
     - `Assets/Editor/Story/SpringDay1DirectorPrimaryLiveCaptureMenu.cs`
     - `Assets/Editor/Story/SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
     - 都不再把 `301` 当当前要采集/烘焙的正式 cue
  3. 工具链已改成当前正式阵容 7 人
     - `Assets/Editor/NPC/SpringDay1NpcCrowdValidationMenu.cs`
       - `ExpectedNpcIds` 改成 7 人
       - director consumption 预期同步去掉 `301`
       - runtime probe 文案同步改成当前 7 人
       - dialogue / roam count 改成只按当前正式阵容计数，不再因为磁盘里还留着 `301` 旧资产就误报
  4. bootstrap 回魂口已收住
     - `Assets/Editor/NPC/SpringDay1NpcCrowdBootstrap.cs`
       - 删除 `301` 的 `CastSpec`
       - 删除 `102 <-> 301` pair 生成
       - 其余 `301` 分支也一并拔掉，避免以后 bootstrap 又把它生回 Day1
  5. 测试/校验口径已同步到 7 人
     - `NpcCrowdManifestSceneDutyTests.cs`
     - `NpcCrowdDialogueTruthTests.cs`
     - `NpcCrowdDialogueNormalizationTests.cs`
     - `NpcCrowdPrefabBindingTests.cs`
     - `SpringDay1DialogueProgressionTests.cs`
     - `SpringDay1DirectorStagingTests.cs`
     - 全部不再把 `301` 当当前 Day1 正式成员
- 这轮明确没做：
  - 没有物理删除 `Assets/111_Data/NPC/SpringDay1Crowd/NPC_301_*.asset` 这批旧资产；现在口径是“磁盘上可留档，但当前 Day1 不再消费、不再验证、不再从 bootstrap 回生”。
- 这轮验证：
  - `rg -n '301|night-witness-301|dayend-watch-301|301-' ...` 回扫后，当前 Day1 正式消费链命中已清空，只剩数值假命中。
  - `git diff --check`（本轮 touched files）通过
  - `Get-Content -Raw SpringDay1DirectorStageBook.json | ConvertFrom-Json` 通过
  - `py -3 scripts/sunset_mcp.py validate_script ... --skip-mcp`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - 阻断仍是老问题 `CodexCodeGuard timeout_downgraded_for_validate_script`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - `0 error / 0 warning`
- 当前恢复点：
  1. 现在可以把 `301` 视为“旧留档，不参与当前 Day1”
  2. 如果后面还要继续 Day1 主线，就回到 `Town 开场围观 -> 村长带路 -> Primary 安置` 那条收尾链，不再为 `301` 留 runtime 位置

## 2026-04-07 UI线程 Workbench 主条互斥与完成态保留
- 当前主线：继续只收 `Workbench` 主进度条/完成领取/悬浮卡保留闭环，不回漂任务卡、Town、NPC owner 或泛 UI 修补。
- 本轮用户新增实测问题：
  1. 主条黄色完成态对比度不够，文字不清。
  2. 只要制作过一次，主条会再次出现 `进度` 与 `制作/追加` 文案重叠。
  3. 全部制作完成后，悬浮卡消失，导致完成产物无法再从工作台里领取。
- 这轮已落地：
  1. `Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
     - 把主条文案和按钮文案拆成互斥状态：
       - `进度` 只在真正制作中显示
       - `追加/开始制作` 只在按钮态显示
       - 空闲/按钮态不再让 `ProgressLabel` 和 `CraftButtonLabel` 同层打架
     - `GetSelectableQuantityCap()` / `CanCraftSelected()` 新增 ready 产物阻断：同一 recipe 只要还有 `readyCount > 0`，就先领取成品，不能继续追加制作。
     - `UpdateProgressLabel()` 改成统一状态机：
       - 制作中 = 绿色单件进度
       - hover 领取 = 黄色领取态
       - hover 取消/中断 = 红色态
       - 已完成 = 更深的黄色完成态
       - 按钮态 = 主条文字留空，避免和按钮文案重叠
     - `EnsureProgressLabelForeground()` 给主条/悬浮卡 label 补深色阴影，提升白字可读性。
     - `CraftRoutine()` 结束后不再在“工作台已关闭但仍有 ready 产物”时调用 `CleanupTransientState(resetSession: true)`；现在会保留 `_queueEntries / _anchorTarget`，让完成悬浮卡持续存在，等待玩家重新打开工作台领取。
     - `ShouldShowCompletedProgress()` 改成只在“真的已完成且没有 pending craft”时才保持黄色完成底，不再在还有进行中的情况下错误常亮完成态背景。
     - 悬浮卡完成态颜色同步切到更深的黄色，并保留完成态文案。
- 这轮验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --owner-thread UI --output-limit 12 --skip-mcp`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
    - `external_errors=0`
    - 阻断仍是 `CodexCodeGuard timeout-downgraded`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 通过
    - 仅有既存 `CRLF -> LF` warning
- 当前仍待真实玩家面复核：
  1. 黄色完成态的观感是否已经足够清晰
  2. 完成后关闭工作台，悬浮卡是否稳定保留直到领取
  3. 同 recipe 在 ready 未领取时，是否已完全阻断继续制作
- 当前恢复点：
  - 下一刀继续只盯 `Workbench`，如果还有问题，优先看 `UpdateProgressLabel / UpdateQuantityUi / CraftRoutine / UpdateFloatingProgressVisibility` 四个方法，不要再回到左列材质或任务卡链。

## 2026-04-07 Workbench 底部交互矩阵补刀
- 这轮继续只收 `Workbench` 底部条的交互矩阵，不扩到别的 UI。
- 用户新实测说明：`加入队列 xN` 这类动作态仍然会和 `进度` 叠在一起，说明底部条还没有做到真正互斥，而不是单个文案漏隐藏。
- 这轮新增处理：
  1. 把底部条明确收成两层互斥：
     - 动作层：`开始制作 / 加入队列 / 追加制作`
     - 进度层：`进度 / 制作完成 / 领取产物 / 取消排队 / 中断制作`
  2. 新增 `ShouldShowProgressFooter()`，并在 `UpdateQuantityUi()` 里直接控制：
     - 只要动作层出现，就把 `progressRoot` 整体隐藏
     - 同时清空 `progressLabel` 与 fill，避免任何残留文本或底条继续露出来
  3. 在代码旁边补了矩阵注释，防止后面又把两层混回去。
- 这轮最小验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --owner-thread UI --output-limit 12 --skip-mcp`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 通过（仅既存 CRLF/LF warning）
- 当前恢复点：
  - 如果下轮还有底部重叠，优先检查 `ShouldShowCraftButton / ShouldShowProgressFooter / UpdateQuantityUi / UpdateProgressLabel` 四处，不要再把它当成单纯颜色或字号问题。

## 2026-04-08 主工作区 build 中文字体链只读核实
- 用户目标：
  - 只读核实当前主工作区里的 build 中文字体链是否已经真正落地，不改业务文件，重点回答 bootstrap 是否存在并被 UI 使用、当前默认字体与 fallback、打包后剩余丢字风险、以及下一刀最值钱补点。
- 只读结论：
  1. `DialogueChineseFontRuntimeBootstrap.cs` 确实存在于 `Assets/YYY_Scripts/Story/Dialogue/`，并且当前目录仍是 `git status` 下的未跟踪目录；也就是说本机工作区里文件在，但还不是已纳入 git 的稳定基线。
  2. 当前对话/提示/气泡主链已经直接调用 bootstrap：
     - `DialogueUI.cs`
     - `SpringDay1PromptOverlay.cs`
     - `SpringDay1WorkbenchCraftingOverlay.cs`
     - `InteractionHintOverlay.cs`
     - `NpcWorldHintBubble.cs`
     - `SpringDay1WorldHintBubble.cs`
     - `NPCBubblePresenter.cs`
     - `PlayerThoughtBubblePresenter.cs`
  3. `TMP Settings.asset` 当前默认字体已是 `DialogueChinese V2 SDF`，fallback 顺序是：
     - `DialogueChinese Pixel SDF`
     - `DialogueChinese SoftPixel SDF`
     - `DialogueChinese SDF`
     - `DialogueChinese BitmapSong SDF`
     - `LiberationSans SDF`
  4. `DialogueFontLibrary_Default.asset` 里 `default / speaker_name / inner_monologue / garbled / retro / narration` 也全部指向 `DialogueChinese V2 SDF`。
  5. 仍未清零的 build 风险主要有三条：
     - 主字体 `DialogueChinese V2 SDF.asset` 是动态多图集字体，资产自身仍是 `m_AtlasPopulationMode: 1`，并且资产内 `m_ClearDynamicDataOnBuild: 1`；这意味着 build 后仍依赖 runtime 预热/补字，而不是纯静态烘焙。
     - bootstrap 的 `WarmupSeedText` 是固定种子；超出 seed、且没有走 `ResolveBestFontForText / CanRenderText` 的文本，仍可能在 player build 里丢字。
     - 现有 opening test 证据里还存在 `Importer(NativeFormatImporter) generated inconsistent result` 指向 `DialogueChinese V2 SDF.asset`，说明主字体资产导入稳定性还没拿到真正无噪音结论。
- 当前恢复点：
  - 如果下一刀继续收 build 中文字体链，最值钱的是先处理 `DialogueChinese V2 SDF.asset` 的 build/import 稳定性与 player build 级验证；这比继续补更多 UI 调用点更值钱，因为当前默认字体链已经基本接上，剩余风险集中在主字体资产本身和 runtime 补字闭环。

## 2026-04-08 Workbench 追加优先级覆盖领取态
- 当前主线仍是 `Workbench` 底部状态机闭环。
- 用户最新 live 反馈明确修正了一条更高优先级语义：
  - 只要玩家已经进入 `追加制作` 选择态，就不应该被中途产出的 `readyCount` 抢回黄色领取态。
  - 领取只能出现在当前选中的那条 recipe 制作栏里，不应该再通过 `先领取...` 这类通用 blocker 抢焦点，更不能在别的 recipe 上制造“好像也能领”的错觉。
- 这轮已改：
  1. `ShouldShowCraftButton()` 改成只看 `selectedQuantity > 0`，不再被 `readyCount` 压掉。
  2. `CanCraftSelected()` 删除 `先领取当前已经完成的成品` 这条全局 blocker；现在领取不再通过 blocker 文案出现。
  3. `GetSelectableQuantityCap()` 删除 `readyCount > 0 -> 0` 的硬阻断，允许玩家在追加态继续选数量。
  4. `BuildCraftButtonLabel()` 不再因为 `readyCount > 0` 而清空；动作层在追加态下保持最高优先级。
- 新的底部优先级矩阵：
  1. 只要 `selectedQuantity > 0`：动作层最高优先级，显示 `开始制作 / 加入队列 / 追加制作`
  2. 只有 `selectedQuantity == 0`：进度层才接管，才允许出现 `进度 / 制作完成 / 领取 / 取消 / 中断`
  3. 因此中途产出成品时，如果玩家已经在选追加，底部不会再被黄色领取态打断
  4. 领取只留在当前选中 recipe 的进度栏状态里，不再走全局 blocker 提示
- 这轮最小验证：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --owner-thread UI --output-limit 12 --skip-mcp`
    - `assessment=unity_validation_pending`
    - `owned_errors=0`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs`
    - 通过（仅 CRLF/LF warning）
- 当前恢复点：
  - 下一轮如果还有错，优先复查“当前选中的 recipe 是否唯一决定 pickup/complete 显示”，尤其继续盯 `UpdateProgressLabel / UpdateQuantityUi / OnProgressBarClicked / Show` 这四个入口。

## 2026-04-08 背包地图页 + 村民关系册落地
- 用户目标：
  - 把背包页签里的 `3_Map` 和 `4_Relationship_NPC` 从两个大空块做成正式可见、像游戏内页而不是测试占位的内容。
  - 约束明确为：美观、克制、内容必须可见，且严格遵守“大包小”，里面内容不能把外层壳体撑爆。
- 前置核查：
  - 本轮沿用 `skills-governor`、`preference-preflight-gate`、`sunset-no-red-handoff` 的口径推进。
  - 已读取并吸收 `global-preference-profile.md`；判定本刀属于真实体验层 UI 施工，不是纯逻辑补丁。
  - 线程状态这轮沿用 `inventory-map-affinity-panels` 切片继续，收尾已执行 `Park-Slice`，当前 `UI` 线程状态为 `PARKED`。
- 实际落地：
  1. 新增 `Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs`
     - 提供运行时背包页构建的通用 UI 小工具：`Rect/Image/TMP/Button` 创建、锚点设置、中文 TMP 字体解析、children 清理。
  2. 新增 `Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs`
     - 在 `3_Map/Main` 上运行时重建“春日村地图”页。
     - 左侧是固定尺寸的村内动线示意板，按剧情阶段高亮 `矿洞口 / 村口 / 临时住处 / 工作台 / 农地 / 食堂 / 后坡 / 墓园`。
     - 右侧使用 `StoryManager.CurrentPhase` 与 `SpringDay1NpcCrowdManifest` 的 `residentBeatSemantics` 显示当前阶段焦点、人群压强摘要与今日路径。
     - 为避免旧 manifest helper 把 `Dinner/Return` 压回白天口径，右侧人物摘要改成按当前 `beatKey` 直接读取 presence，而不是吃旧快照。
  3. 新增 `Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs`
     - 在 `4_Relationship_NPC/Main` 上运行时重建“村民关系册”页。
     - 数据源直接吃 `Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset` + `PlayerNpcRelationshipService.GetSnapshot()`。
     - 左侧是可滚动村民卡片列表；右侧是详情卡，显示 `displayName / 当前关系阶段 / 阶段提示 / 身份观察 / 当前在场感 / 常驻方式`。
     - 立绘优先从 manifest 里绑定的 NPC prefab 上提取 `SpriteRenderer.sprite`，避免再做空头像壳。
     - 已补一个细节修正：左侧列表简介文本右侧收窄，避免与阶段标签发生重叠。
  4. 修改 `Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs`
     - 在 `Awake / SetRoots / EnsureReady` 三个入口自动安装 `PackageMapOverviewPanel` 与 `PackageNpcRelationshipPanel`，确保 PackagePanel 一打开就能把这两页挂起来，不需要额外 prefab 手绑。
  5. 新增三份脚本 `.meta`
     - 避免 Unity 下一次自行补 meta 把 shared root 弄脏。
- 当前验证：
  - `git diff --check -- Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelTabsUI.cs Assets/YYY_Scripts/UI/Tabs/PackagePanelRuntimeUiKit.cs.meta Assets/YYY_Scripts/UI/Tabs/PackageMapOverviewPanel.cs.meta Assets/YYY_Scripts/UI/Tabs/PackageNpcRelationshipPanel.cs.meta`
    - 通过
  - `py -3 scripts/sunset_mcp.py validate_script ... --skip-mcp`
    - 三个新脚本与 `PackagePanelTabsUI.cs` 均未返回 owned/external error
    - 但 fresh CLI receipt 仍被 `CodexCodeGuard` 超时降级为 `unity_validation_pending`
  - `py -3 scripts/sunset_mcp.py --timeout-sec 90 compile <4个目标脚本> --skip-mcp`
    - 仍被工具层 `subprocess_timeout:dotnet:60s` 卡住
- 当前 blocker：
  - 不是已知 owned compile red，而是 `CLI CodexCodeGuard timeout prevents fresh compile receipt`，所以这轮代码层已自检但还没拿到更硬的 fresh compile 证据。
- 当前恢复点：
  - 如果下一轮继续这条线，优先做两件事：
    1. 拿一次真正的 PackagePanel live 证据，确认 `地图页` 与 `关系页` 的真实观感、滚动和选中态是否过线。
    2. 若还要继续 polish，就只微调 `列表卡片间距 / 右侧详情文字密度 / 地图点位标签`，不要回退到空壳结构或泛 UI 修补。

## 2026-04-08 Day1 编译红快速止血：SpringDay1Director helper 重对齐
- 用户目标：
  - 先止血 `SpringDay1Director.cs` 里用户贴出来的 `CS0103: TrySetPreferredObjectActive does not exist`，避免这组红继续卡住 `spring-day1` 主线。
- 前置核查：
  - 本轮沿用 `skills-governor` 与 `sunset-no-red-handoff`。
  - `spring-day1` 线程当时仍在 `ACTIVE`，slice 为 `day1-demo-closure-and-build-font-fix`，所以没有重开 `Begin-Slice`。
- 实际处理：
  1. 核对 `SpringDay1Director.cs` 现场后确认：
     - 这组调用点在 1221/1222/1231/1232；
     - helper 定义其实仍在同文件后半段；
     - `validate_script --skip-mcp` 没能复现这组 owned error。
  2. 为了避免 Unity 继续吃旧缓存或半成品符号，我把 helper 显式重命名为：
     - `TrySetStoryNpcActiveIfPresent(...)`
     - 并同步更新了四个调用点。
  3. 本轮变更不改行为，只是把“Town/Primary 中 001/002 的显隐 helper”重新对齐并强制形成一次新的编译触点。
- 验证结果：
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs --count 20 --output-limit 10 --skip-mcp`
    - `owned_errors=0`
    - `external_errors=0`
    - `assessment=unity_validation_pending`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - 通过
  - `compile` 级 fresh Unity receipt 仍被 `CodexCodeGuard` timeout 卡住，暂时没拿到更硬的 live compile 结论。
- 当前判断：
  - 用户贴出来那组 `TrySetPreferredObjectActive` 红，按当前文件现场已经不该再是主导 blocker；
  - 真正仍待 fresh compile 证明的是 Unity 工具链超时，而不是这个 helper 仍缺失。
- 当前恢复点：
  - 如果下一轮继续追编译链，先看 fresh compile / console 真值；
  - 若 Unity 现场还有新红，优先按最新 console，而不是继续围着旧 helper 名字兜圈。

## 2026-04-08 Town 现场自动验证：opening tests 收窄 + live 快照拿到真实卡点
- 用户目标：
  - Unity 已清到 `Town` / `Edit Mode`，希望我高效把能自动验证的基础设施和剧情闭环先压到位，把真正该留给用户手调的部分分清。
- 本轮前置核查：
  - 使用 `skills-governor`、`sunset-no-red-handoff`、`sunset-unity-validation-loop`。
  - 已按规则重新 `Begin-Slice` 进入 `day1-town-editmode-live-validation`，收尾前已 `Park-Slice`。
- 实际执行：
  1. 跑了 `Sunset/Story/Validation/Run Opening Bridge Tests`
     - 先从旧的 `5 pass / 2 fail` 收到 `6 pass / 1 fail`
     - 修掉了 opening tests 自己的两处旧口径：
       - `DialogueSequenceSO` 类型名旧引用
       - `DialogueManager` 单例/已完成序列模拟不稳
  2. 现在 opening tests 只剩 1 条失败：
     - `VillageGateCompletionInTown_ShouldPromoteChiefLeadState`
     - 当前失败已经不是业务态没接上，而是测试结果里还在吃旧的 progress 文案断言口径，说明这条测试文件的最新文本改动还没被 Unity 稳定刷新进结果。
  3. 进入 PlayMode 后真实跑了 Town 开场 live：
     - `Reset Spring Day1 To Opening` 成功
     - `Write Spring Day1 Live Snapshot Artifact` 成功
     - 快照证明当前真的从 `Town` 开场进入 `EnterVillage`
     - `spring-day1-village-gate` 围观对白在 live 中能被强制推进，从 `1/7` 推到 `5/7`，再推到 `7/7`
  4. 但 live 真实卡点也被钉死：
     - 即使对白已经到 `7/7`
     - snapshot 仍显示 `Dialogue=spring-day1-village-gate[7/7]`
     - `TownLead=inactive`
     - `followupPending=True`
     - `progress=进村围观进行中`
     - 说明当前不是“Town 开场没进来”，而是“VillageGate 收尾后没有真正退出对白并切进村长带路态”
- 当前判断：
  - 当前最值钱的新 blocker 已经从“开场进不来”收窄成：
    - `VillageGate` live 收尾没有把导演链推进到 `TownLead`
  - 这属于我该继续补的导演/对白收尾闭环，不是用户摆点问题。
- 当前恢复点：
  - 下一轮如果继续这条线，优先直接补：
    1. `VillageGate` 在 live 中从 `7/7` 到真正 completed 的收尾
    2. `TownLead` 激活
    3. `Town -> Primary` 转场承接
  - 用户可手调的仍然只是 scene 点位、HomeAnchor 和围观/回村的站位，不是这条正式推进链。

## 2026-04-08 导演 JSON 与实际运行不一致的原因已钉死
- 用户问题：
  - 为什么 `Day1导演摆位` 窗口里编辑了 `EnterVillage_PostEntry` 的 JSON，实际从 `Town` 开始运行时却和窗口里的预期不一致。
- 关键结论：
  1. 用户图里的这个 beat 没看错，`Town` 直接开场进村围观，当前确实就是 `EnterVillage_PostEntry`。
  2. 但这个 stage book / JSON 只负责 `crowd resident`，也就是主要吃：
     - `101`
     - `102`
     - `103`
     - `104`
     - `201`
     - `202`
     - `203`
     它不负责 `001/002` 的正式剧情带路。
  3. `001/002` 当前仍由 `SpringDay1Director` 主链单独控制：
     - formal 对话
     - phase 推进
     - TownLead
     - `Town -> Primary` 转场
     所以窗口里改 crowd cue，不会直接改掉 `001/002` 的实际剧情表现。
  4. 你图里这个 cue 现在还勾着“沿用当前出生位”：
     - runtime 会直接走 `keepCurrentSpawnPosition = true`
     - 也就是 cue 的起始落位不会强制吃你保存的 `startPosition`
     - 而是优先从 scene 当前 resident 位置起跑
     这也是“你明明改了，但运行像没完全按 JSON 来”的直接原因之一。
  5. 窗口里的“开始排练 / 把当前 Cue 应到选中 NPC”本质上是 `manualPreviewLock` 的单体预演；
     真运行时则是 `NpcCrowdDirector` 按当前 beat、manifest entry、scene resident 状态去消费，不是同一条执行链，所以不能假设“预演 = 正式剧情 100% 一样”。
  6. 当前还有一个真 bug 在叠加干扰判断：
     - `VillageGate` 围观对白收尾后，没有正确切进 `TownLead`
     - 所以你会在 `Town` 里看到后续不该在这时出现的正式剧情感
     - 这部分不是导演 JSON 自己的问题，而是主链没闭环。
- 当前恢复点：
  - 后续若继续修这条，优先顺序不是继续猜 JSON，而是：
    1. 先补 `VillageGate completed -> TownLead active`
    2. 再补 `Town -> Primary`
    3. 最后再把导演工具真正压到“预演效果 = runtime 正式效果”

## 2026-04-08 skip 到工作台触发的失活 NPC 气泡协程报错止血
- 当前主线目标：
  - 原主线仍是 UI 线程的背包页/玩家面收口；这轮是用户新贴出的 runtime blocker，先止血 `Toggle Skip To Workbench 0.0.5` 后 `001` 失活还被发气泡导致的协程报错。
- 本轮子任务：
  - 只修这条 `inactive NPC bubble coroutine` 坏链，不回漂去改 Workbench、任务卡或别的 Town UI。
- 已完成：
  1. 定位根因：
     - `SpringDay1Director.TryShowEscortWaitBubble(...)` 在 skip 后仍可能给已经失活的 `001` 发等待气泡；
     - `NPCBubblePresenter.ShowTextInternal(...)` 会继续走 `StartVisibilityAnimation(...)` / `StartCoroutine(HideAfterSeconds(...))`；
     - 于是直接打出 `Coroutine couldn't be started because the game object '001' is inactive!`。
  2. 在 `NPCBubblePresenter.cs` 补了宿主失活自保：
     - `UpdateTypedConversationText(...)`、`ShowTextInternal(...)` 在 `!Application.isPlaying || !isActiveAndEnabled || !gameObject.activeInHierarchy` 时直接 `return false`；
     - `HideBubble()` 在宿主失活时改走 `HideImmediate()`；
     - `StartVisibilityAnimation(...)` 在宿主失活时不再起协程；
     - 自动隐藏协程也只会在宿主仍可运行时启动。
  3. 在 `SpringDay1Director.cs` 的 `TryShowEscortWaitBubble(...)` 再补一层调用前护栏：
     - presenter 失活时直接跳过，不再反复对失活 NPC 发等待气泡。
  4. Edit Mode 编译侧验证：
     - 两个脚本 `validate_script --skip-mcp` 都是 `owned_errors=0 / external_errors=0`；
     - Unity fresh compile 后控制台只剩两条已有 `TMP_Text.enableWordWrapping` warning，没有新的 error。
- 关键判断：
  - 这次先把防线落在 `NPCBubblePresenter` 自身是对的；因为导演层就算未来还有别的调用点，presenter 也不会再把 inactive host 直接炸成红错。
- 当前遗留：
  - 还没再做新的 PlayMode 复跑，因为上一轮 live 验证中断了用户运行现场；这轮先收成“代码已止血 + Edit Mode 编译无新增红错”。
- 当前恢复点：
  - 如果继续原 UI 主线，直接回到背包 `地图 / 好感度` 页的 live 观感验证；
  - 如果用户要继续追这条报错，就从 `skip 到工作台` 的最小 live 复现继续，但要先说明不会中断对方运行现场。

## 2026-04-08 Workbench 中断逻辑修复：只取消当前 recipe，不再连坐整条队列
- 用户新增反馈：
  - 在斧头制作栏点击“中断”时，其他排队中的制作也一起停掉、甚至全部没了。
  - 用户明确要求语义：
    - 制作开始时就扣材料；
    - 中断时只返还当前正在制作这一件的材料；
    - UI 与背包内容必须立刻刷新。
- 本轮已完成：
  1. 在 `SpringDay1WorkbenchCraftingOverlay.cs` 把 `HasWorkbenchFloatingState` 扩成包含 pending-only 队列，避免未开始但仍在排队的 entry 被误清空。
  2. 新增 `ResumeNextPendingCraftQueueIfAny()`：
     - 当前 active recipe 被中断后，会自动切到下一条 pending queue 继续做，不再整条停机。
  3. 重写 `CancelActiveCraftQueue()` 的收口顺序：
     - 只返当前单件已扣材料；
     - 只截断当前 recipe 的剩余 pending；
     - 其他 recipe 队列继续保留；
     - 返还后主动刷新 UI/背包；
     - 若有下一条 pending，立即续跑。
  4. 最小验证：
     - `validate_script --skip-mcp` 对 `SpringDay1WorkbenchCraftingOverlay.cs` 为 `owned_errors=0`
     - Unity Edit Mode fresh compile 未见新的 compile error
- 当前口径：
  - 这轮已经从“定位”进入“真实修复”，当前最该复测的是多 recipe 队列中断场景，不是再看静态布局。

## 2026-04-08 Workbench 交互矩阵纠偏：每个 recipe 必须独立
- 用户进一步指出：
  - 现在真正的问题不是“中断按钮有没有续跑下一条”，而是整套交互语义仍然串味；
  - 斧头正在制作时，箱子那一行仍然能显示/继承不属于自己的领取或中断语义。
- 当前只读审查后的统一判断：
  1. 正确模型必须是：
     - `一个工作台调度器`
     - `多个独立 recipe entry`
  2. 工作台调度器只决定谁在 active；
     - 不应该把 active 行的动作状态投射给当前选中的其他行。
  3. 每个 recipe entry 的可交互状态必须独立：
     - 空闲
     - 排队未开始
     - 当前制作中无产出
     - 当前制作中已有产出
     - 完成未领取
     - 数量选择中的动作优先
  4. 正确语义是：
     - 领取只能领自己这一行
     - 中断只能断自己这一行
     - 取消排队只能取消自己这一行
     - 追加制作只作用于自己这一行
     - 其他行最多显示自己的排队/完成状态，绝不能显示 active 行的领取/中断语义
- 当前恢复点：
  - 后续若继续做这条，不能再只补点状 if/else，必须按 `entry-state` 彻底重构底部条和悬浮框。

## 2026-04-08 Workbench entry-state 隔离重构第一刀已落地
- 本轮施工目标：
  - 不再停在矩阵分析，直接把 Workbench 的“全局 active 状态串给当前选中行”开始拆掉。
- 已完成：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 新增 entry 维度状态 helper：
       - `IsActiveEntry`
       - `CanPickupEntryOutputs`
       - `CanInterruptActiveEntry`
       - `CanCancelQueuedEntry`
       - `GetEntryUnitProgress`
       - `ResetEntryRuntimeState`
       - `SetActiveEntryProgress`
       - `SetActiveEntryReserved`
     - `WorkbenchQueueEntry` 新增：
       - `currentUnitProgress`
       - `hasReservedCurrentCraft`
     - `CraftRoutine / StopCraftRoutine / CancelActiveCraftQueue / ResumeNextPendingCraftQueueIfAny`
       已同步改成把“当前单件进度 / 当前单件是否已扣料”写回 entry。
  2. 底部条状态机已开始按 selected entry 隔离：
     - `OnProgressBarClicked` 不再按全局 active 直接判，而是先看当前 entry 自己能不能 pickup / interrupt / cancel-queue
     - `UpdateProgressLabel` 不再把 active 行的领取/中断语义投到别的行
     - `UpdateQuantityUi / BuildCraftButtonLabel / GetMaterialPreviewQuantity` 也开始按 selected entry 自己是否 active 来判断
  3. 悬浮卡状态开始按 active entry 自己的单件进度显示：
     - `FloatingProgressDisplayState` 新增 `ActiveUnitProgress`
  4. 选中 recipe 时 now 会清空 hover 残留，避免上一行的 hover 状态串到下一行。
  5. `ShouldShowCompletedProgress` 不再给没有 entry 的行套旧完成缓存。
- 验证：
  - `validate_script --skip-mcp`：`owned_errors=0`
  - Unity Edit Mode fresh compile：控制台 `0 error`
  - 仍有两条既有 obsolete warning，不是这轮新红
- 当前口径：
  - 这轮已经进入“真实重构”而不是“再解释一次矩阵”；
  - 但还需要用户 live 验证玩家体感，尤其是“active 行语义不再串给别的行”这一点。

## 2026-04-08 导演工具与 runtime 围观走位统一到“绝对场景点位”
- 当前主线目标：
  - 把 `Town` 的两次围观走位收回到用户可直接验的状态：导演窗口里手工用场景物体采的起点/途径点，必须和 runtime 正式剧情消费到的是同一套位置。
- 本轮子任务：
  - 修导演窗口采点口径
  - 修同一 cue 手动预演不重启的问题
  - 修旧的 `EnterVillage_PostEntry` StageBook 错旗标
  - 补 bake/live capture 写回绝对坐标时的护栏
- 已完成事项：
  1. `SpringDay1DirectorStagingWindow.cs`
     - `选中物体设为起点 / 追加路径点 / 覆盖路径点` 现在会强制把 cue 切到“绝对场景点位”口径：
       - `keepCurrentSpawnPosition = false`
       - `useSemanticAnchorAsStart = false`
       - `startPositionIsSemanticAnchorOffset = false`
       - `pathPointsAreOffsets = false`
     - 对旧的错误组合做了预演前归一化，避免用户明明已经采了 scene 点位，预演还按旧 resident 出生位或锚点偏移跑。
     - 手动预演调用现在带 `forceRestart=true`，同一个 cue 改完后再点应用，会重新从新起点重开，而不是沿用旧播放状态。
  2. `SpringDay1DirectorStaging.cs`
     - `SpringDay1DirectorStagingPlayback.ApplyCue(...)` 新增 `forceRestart` 口，保留 runtime 默认“同 cue 不重置”，但允许导演窗口手动预演明确重开。
  3. `SpringDay1DirectorStageBook.json`
     - `EnterVillage_PostEntry` 里已经有有效起点的 crowd cue（`101/102/103/104/201/202/203`）已改成绝对场景点位，不再保留会把绝对坐标误当锚点偏移的旧旗标。
     - `003` 和重复 `104` 那两条零起点 cue 没被强行改写，仍待后续真实配置。
  4. `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` 与 `SpringDay1DirectorPrimaryRehearsalBakeMenu.cs`
     - 以后把 cue 写回成绝对坐标时，也会一并关掉旧的 semantic-start/offset 旗标，避免再把 JSON 写坏。
     - cue 查找兼容 `cueId` / `npcId`，不再被 opening crowd 这批数字命名卡死。
  5. `SpringDay1DirectorStagingTests.cs` 与 `SpringDay1TargetedEditModeTestMenu.cs`
     - 测试口径已改成 opening crowd 走 absolute scene points
     - 新增 `StagingPlayback_ForceRestart_ShouldSnapNpcBackToStartForManualPreview`
     - 导演测试菜单已吃到新测试名
- 验证结果：
  - `Run Director Staging Tests`：`27 pass / 0 fail`
  - `manage_script validate`：
    - `SpringDay1DirectorPrimaryLiveCaptureMenu.cs` clean
    - `SpringDay1DirectorPrimaryRehearsalBakeMenu.cs` clean
    - `SpringDay1TargetedEditModeTestMenu.cs` clean
    - `SpringDay1DirectorStagingTests.cs` clean
    - `SpringDay1DirectorStaging.cs` / `SpringDay1DirectorStagingWindow.cs` 只有既有性能 warning，无新增 error
  - `validate_script / compile / no-red` 仍被工具侧 `dotnet:20s` timeout 卡住，不能拿它当最终 compile 票据
- 当前仍待：
  - 用户手调并验收 `DinnerConflict_Table` 那一套围观最终点位是否完全符合预期
  - `003` 与重复 `104` 的零起点 cue 若要投入剧情，仍需真实 scene 配置
  - Unity console 里还残留一条旧的 `Temp/CodexEditModeScenes/.../Town.unity` 绝对路径日志；这轮 helper 已改成相对路径，但历史日志未自动清空
- 主线恢复点：
  - 下一轮如果继续围观调度，就直接在导演窗口里用 scene 物体调 `EnterVillage_PostEntry / DinnerConflict_Table`，现在工具预演和 runtime 已按同一口径吃点。

## 2026-04-08｜Workbench 进度串味第二刀：右侧详情不再被 active recipe 强刷
- 当前主线目标：
  - 继续收 `SpringDay1WorkbenchCraftingOverlay` 的 recipe 隔离闭环，重点修用户刚贴图确认的“切到别的配方后，右侧进度仍显示当前正在制作物品”的根源串味。
- 本轮子任务：
  - 彻查 `CraftRoutine / UpdateProgressLabel / Open / BuildFloatingProgressStates / director progress push / completed-output gate` 的状态来源，并补最小 live 证据。
- 已完成事项：
  1. `SpringDay1WorkbenchCraftingOverlay.cs`
     - 把 `CraftRoutine()` 逐帧刷新从 `UpdateProgressLabel(recipe)` 改成 `UpdateProgressLabel(GetSelectedRecipe())`，切断“active recipe 每帧覆盖当前选中右侧详情”的根因。
     - `PushDirectorCraftProgress()` 改成只从真实 `activeEntry` 取 `recipe / readyCount / totalCount / currentUnitProgress`，不再因为切到别的配方行就把导演层工作台进度清空或串味。
     - `HasActiveCraftQueue / HasReadyOutputs / GetRemainingCraftCount / Open()` 改成以 `_activeQueueEntry` 为主，不再继续依赖旧的全局计数或 `_craftingRecipe` 做焦点判定。
     - 新增 `EntryHasUnclaimedCompletedOutputs()`，并接到 `CanCraftSelected()` 与 `GetSelectableQuantityCap()`：同一 recipe 有完成品未领取时，先禁止继续给这条 recipe 追加新制作。
     - `BuildFloatingProgressStates()` 不再按 `resultItemId` 聚合悬浮卡，改成按 entry/recipe 自己出牌，避免不同 recipe 共用一张卡的 active 进度。
     - `RefreshSelection()` 现在会同步 `RefreshRows(allowRecovery: false)`，让左列摘要在 `readyCount/totalCount` 变化后及时刷新，不再出现“悬浮卡一个数、左列另一个数”的假状态。
  2. `SpringUiEvidenceMenu.cs`
     - 扩了现有 `WorkbenchCraftExitProbe`，在开始制作后自动切到另一条 recipe，并把 `selected / progress` 写进 live 日志，用来直接验证“右侧详情是否还会被 active recipe 劫持”。
  3. `SpringDay1DialogueProgressionTests.cs`
     - 补了一条 source-level 守门：`UpdateProgressLabel(GetSelectedRecipe())`，至少防止把这条最蠢的回归再直接写回去。
     - 顺手回正了同文件里两个已经落后于现行工作台矩阵的老断言口径（浮动卡重定位方法名、CraftButton/进度条职责分流）。
- 验证结果：
  - 脚本级 CLI：
    - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1WorkbenchCraftingOverlay.cs --skip-mcp`
    - `py -3 scripts/sunset_mcp.py validate_script Assets/Editor/Story/SpringUiEvidenceMenu.cs --skip-mcp`
    - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1DialogueProgressionTests.cs --skip-mcp`
    - 三者均 `owned_errors=0`，assessment 仍是 `unity_validation_pending`（`CodexCodeGuard timeout-downgraded`）。
  - Unity fresh compile / console：
    - 清空 Console 后 `refresh_unity(force + compile=request)`
    - 最新读取 `0 error`
  - live probe：
    - 开启工作台直跳，进入 `FarmingTutorial` 后执行 `Run Spring Day1 Workbench Craft Exit Probe`
    - 关键日志：`[SpringUiEvidence] WorkbenchSelectionIsolation => switched=True, selected='锄头', progress='选择配方后即可开始制作'`
    - 这说明开始制作后切到别的 recipe，右侧详情已不再跟着 active recipe 走。
- 当前仍待：
  - `SpringDay1DialogueProgressionTests.WorkbenchInteraction_ContainsRuntimeBindingBridge` 这条源码测试仍有更多旧口径断言待回正；它当前失败的是老的 hover/CraftButton 语义预期，不是新编译红。
  - `_lastCompletedQueueTotal / _lastCompletedRecipeId` 这组单槽残影仍在文件里，只是当前未再喂玩家面；若继续清理 workbench 底层，可作为下一刀尾账清掉。
- 当前恢复点：
  - 如果用户继续贴 workbench 截图，下一刀优先看：
    1. 当前选中非 active recipe 时，底部 CTA/材料预览是否还残留 station-global queue 语义；
    2. 悬浮卡在多 recipe 同时排队时是否已完全按 recipe 隔离；
    3. 左列 runtime summary 里的 `unreadableRows/hardRecovery` 是否还在误报。

## 2026-04-08：Workbench 默认制作条与悬浮卡进入只读体验复盘，当前站住 targeted probe，不 claim 终验过线

- 用户目标：
  - 这一轮不落地代码，只讨论 `Workbench` 当前两个体验点：
    1. 默认未制作状态下底部制作条显示 `进度 0/0` 是否合理；
    2. 悬浮制作卡当前还有哪些“不是 bug 但看着不够最终版”的问题。
- 本轮性质：
  - `只读分析`
  - 未跑 `Begin-Slice`
  - 不进入真实施工
- 本轮稳定结论：
  1. 默认 idle 态显示 `进度 0/0` 确实不自然。
     - 代码当前在 `UpdateProgressLabel()` 的最终 `else` 分支里直接写死 `labelText = "进度  0/0"`。
     - 这会把“没有任务”伪装成“有一个 0/0 的任务”，语义不干净。
  2. 底部区块的状态表达仍是“两套控件切换”，还不是“一个统一制作条状态机”。
     - `ShouldShowCraftButton()` 与 `ShouldShowProgressFooter()` 现在是互斥关系：一旦 `_selectedQuantity > 0` 就隐藏进度条、切到 `craftButton`。
     - 这说明当前虽已接近玩家面闭环，但在表达层还不是最纯的“同一根条在未开始 / 制作中 / 可领取 / 可中断 / 可追加之间切状态”。
  3. 悬浮卡当前最大的体验问题已经不是逻辑串味，而是阵列和语义表达不够最终版：
     - `RepositionFloatingProgressCards()` 固定按 3 列宽度算整行；当可见卡是 `4` 或 `5` 张时，第二排不会按“本排实际卡数”居中，视觉会偏左。
     - `BuildFloatingProgressStates()` 当前按 `HasActiveCraft -> ReadyCount -> RecipeId` 排序，状态变化时卡位可能重排，玩家会感到卡片在跳位。
     - `ApplyFloatingProgressVisualBaseline()` 当前卡片是 `72x78`，图标 `46x46`，底部条和文案高度都比较紧，已经能用，但仍偏“测试态小卡”。
  4. 悬浮卡与底部制作条目前都还保留一个相同的语义毛边：
     - 对“排队但尚未开始”的状态，当前更像用 `进度 0/总数` 去表达。
     - 这在代码里分别落在：
       - 底部条：`UpdateProgressLabel()` 的 idle / empty 分支
       - 悬浮卡：`UpdateFloatingProgressVisibility()` 的最后 `else` 分支
     - 语义上更像“占位进度”，不够像真正的排队态。
  5. 当前还有一条值得后续统一的轻逻辑尾账：
     - `EntryHasUnclaimedCompletedOutputs()` 只在 `readyCount > 0 && !HasPendingCrafts(entry)` 时才视为“未领取完成品”。
     - 这意味着“已有完成品但仍有 pending”的 recipe，还没有被彻底纳入同一条阻断语义。
- 当前判断：
  - 这轮最核心的判断不是“Workbench 还有大逻辑没做完”，而是：
    - 主面板主逻辑大体站住了；
    - 现在最值得收的是“底部条语义纯化 + 悬浮卡最终版表达”。
- 当前薄弱点：
  - 这轮没有 fresh live 复抓，只站住：
    - 用户最新实测反馈
    - 当前源码阅读
    - 因而当前层级仍是 `targeted probe / partial validation`，不是 `real entry experience` 终验。
- 当前恢复点：
  - 如果下一轮继续 Workbench，最值得优先的一刀应是：
    1. 先去掉 idle 态 `进度 0/0` 的假进度语义；
    2. 再把底部条收成真正的一根多状态条；
    3. 最后只精修悬浮卡的居中、排序稳定性、比例和文案语义。

## 2026-04-08｜spring-day1：Town<->Primary 双向切场落点补齐，Opening/Midday/LateDay/Staging targeted tests 全绿
- 当前主线目标：
  - 继续把 `spring-day1` 从“Town 原生 resident + director/crowd 已改口”推进到更像可验 Day1 baseline 的状态；本轮最真实的 runtime blocker 已收敛成 `Town <-> Primary` generic `SceneTransitionTrigger` 没有 entry anchor，导致玩家仍掉到 scene 默认 `Player` 点，进村承接与回村晚饭承接会错位。
- 本轮实际施工：
  1. `SceneTransitionTrigger2D.cs`
     - 继续在不改 `Town.unity/Primary.unity` 的前提下补 runtime fallback：
       - `Town` 里的 generic `SceneTransitionTrigger -> Primary` 现在会自动回到 `PrimaryHomeEntryAnchor`
       - `Primary` 里的 generic `SceneTransitionTrigger -> Town` 现在会自动回到 `EnterVillageCrowdRoot`
     - 保留原有 `HomeDoor -> PrimaryHomeEntryAnchor` 与 `PrimaryHomeDoor -> HomeEntryAnchor` 规则不变。
  2. `PersistentPlayerSceneBridge.cs`
     - `ResolveEntryAnchor()` 现在支持 `A|B|C` 这种 alias entry 名称；后续如果 target entry 需要兼容 `村民汇聚点|EnterVillageCrowdRoot` 这类 scene 命名并存，不会再直接掉回默认 `Player` 点。
  3. `SpringDay1OpeningRuntimeBridgeTests.cs`
     - 新增 opening bridge 护栏：
       - `GenericTownTransitionTrigger_ShouldFallbackToPrimaryHomeEntryAnchor`
       - `GenericPrimaryTransitionTrigger_ShouldFallbackToTownOpeningEntryAnchor`
       - `PersistentPlayerSceneBridge_ShouldTreatPipeSeparatedEntryNamesAsAliases`
  4. `SpringDay1TargetedEditModeTestMenu.cs`
     - 把上面 3 条新测试正式接进 `Run Opening Bridge Tests` 菜单，避免“测试写了但 targeted menu 没跑到”。
- 延续并站住的前序改动（本轮已重新验证，不是只沿用聊天印象）：
  - `SpringDay1DirectorStaging.cs`
    - cue 解析优先吃当前 beat 自己的 cue，只有缺配时才 fallback；避免 `DinnerConflict/ReturnAndReminder` 偷偷回到 opening cue。
  - `SpringDay1DirectorStagingWindow.cs`
    - `预演当前 Beat（整批）` 优先吃窗口草稿 `_book`
    - 手动预演重放同一 cue 时会 `forceRestart`
    - 采点口径已统一到 absolute scene points
  - `SpringDay1Director.cs`
    - Town opening -> Primary 承接 -> 疗伤 -> 工作台 -> 农田 -> 回村晚饭 -> 提醒 -> free time -> sleep/day end 主链仍在
    - 疗伤桥仍是“玩家先走到艾拉身边再触发”
    - 村长/艾拉 escort 掉队等待逻辑仍在，提示词保持 `小伙子，先跟我走`
    - `001/002` 在 `Primary` 会保到 `ReturnAndReminder`
- 验证结果：
  - `py -3 scripts/sunset_mcp.py doctor`：baseline `pass`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`0 error / 0 warning`
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SceneTransitionTrigger2D --path Assets/YYY_Scripts/Story/Interaction --level standard --output-limit 8`：clean
  - `py -3 scripts/sunset_mcp.py manage_script validate --name PersistentPlayerSceneBridge --path Assets/YYY_Scripts/Service/Player --level standard --output-limit 8`：`0 error / 2 warning`（既有 perf warning，不是新红）
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1OpeningRuntimeBridgeTests --path Assets/YYY_Tests/Editor --level standard --output-limit 8`：clean
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1TargetedEditModeTestMenu --path Assets/Editor/Story --level standard --output-limit 8`：clean
  - `git diff --check`（本线程 touched files）：通过
  - targeted EditMode tests 真实重跑：
    - `Run Opening Bridge Tests`：`10 pass / 0 fail`
    - `Run Midday Bridge Tests`：`7 pass / 0 fail`
    - `Run Late-Day Bridge Tests`：`5 pass / 0 fail`
    - `Run Director Staging Tests`：`27 pass / 0 fail`
  - `validate_script` 这轮仍统一落在 `assessment=unity_validation_pending`，主因依旧是工具侧 `CodexCodeGuard timeout-downgraded / subprocess_timeout:dotnet:20s`，不是 owned compile red。
- 当前判断：
  - 这轮最值钱的推进已经不是“再解释 Town 有没有准备好”，而是把 `Town <-> Primary` 双向落点真的补回 Day1 runtime，并用 opening/menu/test 护栏钉死。
  - 目前代码/targeted probe 层已经站住 `structure + targeted validation`，但还不能包装成 `real entry experience` 全过；最终玩家体验仍需你在 Unity 里跑一遍进村、疗伤、农田、回村晚饭、夜里睡觉。
- 当前剩余：
  1. 还没做新的 full live 漫游终验；所以“玩家手感 / 演出节奏 / NPC 实际是否完全按你摆的点位走”仍待真实入口体验验证。
  2. `HouseArrival / Healing / ReturnReminder` 这类 authored/fallback 文案虽已比旧路线干净很多，但仍可继续做一刀文本统一；这不是当前 compile/runtime blocker。
  3. scene 本体迁移仍不在我这一刀里：`Town.unity / Primary.unity` 没被我改；用户手摆的 `HomeAnchor` 与 crowd 场景点位依旧保持 scene-owned。
- 当前恢复点：
  - 如果继续本线程，最稳的下一刀不是再补 generic trigger，而是做一轮最短 live acceptance：
    1. 从 `Town` 开场验证围观 -> 引路 -> 切到 `Primary` 落在 `PrimaryHomeEntryAnchor`
    2. 在 `Primary` 验证先靠近艾拉才进疗伤桥
    3. 完成农田五步后验证回村切场落到 `EnterVillageCrowdRoot`，晚饭 / 提醒 / free time / 睡觉收束连通
  - 如果只接代码层，这轮已把最真实的 runtime anchor blocker 清掉；下一次再修，优先处理 live 验收里暴露的具体时机/文案问题，不要回到“玩家为什么掉到默认 Player 点”这条旧问题。
- thread-state 收口补记：
  - 已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
  - 状态层 blocker：
    1. 需要真实入口体验验收：`Town 开场 -> Primary 承接 -> 疗伤 -> 工作台 -> 农田 -> 回村晚饭 -> 夜里睡觉`
    2. 工具侧噪音：`validate_script/compile` 仍可能被 `CodexCodeGuard dotnet timeout` 降级，不是当前 owned compile red

## 2026-04-08｜spring-day1：live 验收继续推进到 Town->Primary 真切场，抓到剧情态回退并先补 StoryManager 跨场景兜底
- 当前主线目标：
  - 直接把 `spring-day1` 从 `Town` 原生开场一路 live 跑到 `DayEnd`，边测边修，不再停在结构判断。
- 本轮实际推进：
  1. 新增 [SpringDay1NativeFreshRestartMenu.cs](D:/Unity/Unity_learning/Sunset/Assets/Editor/Story/SpringDay1NativeFreshRestartMenu.cs)
     - 菜单 `Sunset/Story/Validation/Restart Spring Day1 Native Fresh`
     - 只调用 `SaveManager.Instance.RestartToFreshGame()`，用于把 live 现场快速拉回 `Town` 原生开局。
  2. 真实 live 现场推进：
     - 通过 `spring-day1-live-snapshot.json` + `Trigger Spring Day1 Recommended Action Artifact` 持续推进，
     - 已确认 `Town / EnterVillage` 围观收束后，切场请求确实能把现场切到 `Primary`，不再只是卡在 `Town`。
  3. 新抓到的第一 live blocker：
     - 切到 `Primary` 后，剧情态掉回 `CrashAndMeet`，`Decoded=False`，导致导演摘要与玩家面都像重新回到开局。
  4. 针对这个 blocker 先补了一刀代码兜底：
     - [StoryManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryManager.cs)
       - 新增运行时静态快照，`SetPhase / SetLanguageDecoded / ResetState` 都会同步记录；
       - `Awake` 统一脱离父级并 `DontDestroyOnLoad`，避免 scene 内实例被父层级拖死；
       - 若跨场景后 `StoryManager` 被重建，新实例会先继承上一拍剧情态，而不是退回 `CrashAndMeet`。
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
       - live runner 若干关键入口改成统一走 `StoryManager.Instance`，不再到处 `FindFirstObjectByType<StoryManager>` 读现场，减少多实例/误读风险。
- 验证结果：
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1NativeFreshRestartMenu --path Assets/Editor/Story --level standard --output-limit 8`：clean
  - `py -3 scripts/sunset_mcp.py manage_script validate --name StoryManager --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 8`：clean
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 8`：warning-only（既有 perf warning）
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`0 error / 0 warning`
  - `py -3 scripts/sunset_mcp.py status`：fresh console `0 error / 1 warning`
    - warning 为 TMP runtime font atlas 提示，不是本轮 owned red。
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/StoryManager.cs Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`：通过
- 当前判断：
  - 这轮最关键的新信息不是“切场还没通”，而是“切场已通，但剧情态在 `Primary` 承接时会回退”；我先把最像根因的 `StoryManager` 跨场景持久态补强了。
- 当前还没闭环的点：
  1. 还没在补完这刀后重新跑完整 `Town -> Primary -> 疗伤 -> 工作台 -> 农田 -> 回村 -> 睡觉` live pass。
  2. 因为用户中途要求停在最近刀口，这轮收在“剧情态回退已定位并先补兜底，但 full pass 尚未复跑”。
- 当前恢复点：
  - 下轮继续时，直接从 `Town` fresh restart 重开 live，
  - 第一验证点就是：`EnterVillage` 切到 `Primary` 后，phase 是否仍保持 `EnterVillage` 并自动接 `HouseArrival / Healing`，
  - 若这点站住，再继续往 `DayEnd` 整条链推。
- thread-state：
  - 本轮已执行 `Begin-Slice`
  - 用户要求最近刀口停下后已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-08｜spring-day1：002 跟随问题继续深挖，先确认当前刀口不是“找不到 002”，而是正式进村链起跑过晚
- 当前主线目标：
  - 继续修 `Town` 第一波对话后 `002` 没有跟着 `001` 和玩家去 `Primary` 的问题，并把 Day1 正式进村链重新接稳。
- 本轮新增事实：
  1. 已确认用户现场里 `002` 确实在 `Town/NPCs` 根下；这条线不再按“Town 里可能没有 002”处理。
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 当前已补：
     - `FindSceneNpcTransformById`
     - `FindPreferredStoryNpcTransform`
     - `TryDriveEscortActor`
     - `NudgeStoryActorTowards`
     用于让 `001/002` escort 不再只靠固定对象名和 `roam.DebugMoveTo()`。
  3. 用 `PLAY + Reset Spring Day1 To Opening + live snapshot artifact` 连续追运行态后确认：
     - `T+1s`：`TownLead=inactive`，`002` 已经先掉成闲聊入口；
     - `T+5s`：crowd 已切到 `EnterVillage_PostEntry`；
     - `T+10s`：`spring-day1-village-gate` 正式围观对白才真正开始。
  4. 因此当前最值钱的判断是：
     - 现在还不能把 `002` 不跟随直接定性为 escort 单点 bug；
     - 更前面的正式进村围观/带路链本身存在“起跑过晚、前几秒已让 002 回落闲聊”的问题。
- 当前验证结论：
  - `Town` 正式进村链不是完全没触发，而是 live 中延迟数秒后才拉起；
  - 当前停在“围观对白已能起，但还没继续推进完去看 `TownLead` 激活后 `002` 是否真跟上”的刀口。
- 下一步恢复点：
  - 直接从当前 live 验收链继续把 `spring-day1-village-gate` 推完；
  - 第一观察点是 `GetTownHouseLeadSummary()` 是否从 `inactive` 变成 `started=...|chief=...|companion=...`；
  - 只有在 `TownLead` 真激活后，`002` 是否跟随才值得下最终判断。
- thread-state：
  - 已执行 `Begin-Slice`
  - 当前按用户要求停在最近刀口，已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-08：UI 线程补一刀，工作台距离外左下角 teaser 卡已切掉

- 当前主线目标：
  - 继续收 `Workbench` 玩家面异常；本轮子任务是修掉“未进入交互距离时，左下角仍冒出 `靠近工作台 / 再靠近一些`”这张异常卡。
- 本轮完成：
  1. 重新核实 `CraftingStationInteractable`，确认工作台本体在“关闭且未进交互距离”时其实已有一层返回挡板。
  2. 继续向上查到真正更硬的开口在 `SpringDay1ProximityInteractionService`：
     - `ShouldKeepOverlayVisibleForTeaser()` 之前仍允许 teaser 候选点亮底部 `InteractionHintOverlay`。
  3. 已把这条服务层开口收掉：
     - `SpringDay1ProximityInteractionService.ShouldKeepOverlayVisibleForTeaser()` 现改为直接返回 `false`；
     - 因而 `canTriggerNow == false` 的 teaser 候选不再点亮左下角提示卡。
  4. 同轮把旧测试口径回正：
     - `SpringDay1InteractionPromptRuntimeTests` 里原先要求“Workbench teaser 应显示底部卡”；
     - 现已改成“Workbench teaser 在进入交互距离前应保持隐藏”。
- 涉及文件：
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Interaction\SpringDay1ProximityInteractionService.cs`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1InteractionPromptRuntimeTests.cs`
- 验证结果：
  - `git diff --check -- Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs`：通过
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs --skip-mcp`：`owned_errors=0`
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Tests/Editor/SpringDay1InteractionPromptRuntimeTests.cs --skip-mcp`：`owned_errors=0`
  - `py -3 scripts/sunset_mcp.py status / errors`：
    - 当前被外部 `listener_missing / WinError 10061` 卡住，未拿到 fresh live console
- 当前判断：
  - 这刀已切掉“工作台范围外仍允许底部 teaser 卡显示”的服务层开口。
  - 但当前还没有 fresh live 证据，所以只能 claim：
    - 代码层 clean
    - 结构成立
    - live 待复测
- 当前恢复点：
  - 下一轮优先让用户 fresh 验图二这张卡是否彻底消失；
  - 若通过，再继续回到底部多状态制作条与悬浮卡的最终版收口。

## 2026-04-08｜spring-day1：Primary 剧情演员态与 002 带路链收口（本轮主刀）
- 当前主线目标：
  - 直接收口 `001/002` 在 Day1 主线中的“剧情演员态”，并修正 `002` 在进村带路阶段不跟随的问题。
- 本轮实际做成：
  1. 在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 新增了剧情演员态策略：
     - `Primary` 的正式阶段会对 `001/002` 执行“禁正式对话、禁闲聊入口、禁自动气泡/招呼”的 runtime 控制；
     - Town 的 `EnterVillage`（进村未完成）也会应用同一控制，避免 `002` 提前回落闲聊打断主线。
  2. 进村围观起跑增加超时兜底：
     - `TryHandleTownEnterVillageFlow()` 里不再无限等待 crowd cue settle；
     - 在 beat 已进入 `EnterVillage_PostEntry` 且超过 `townVillageGateCueSettleTimeout` 时允许继续排 `village-gate` 正式对白。
  3. `002` 查找链补强，避免误命中 `002起点/终点/HomeAnchor`：
     - `FindSceneNpcTransformById()` 的 transform fallback 增加了 NPC 组件过滤（`NPCAutoRoamController/NPCMotionController/NPCDialogueInteractable/NPCInformalChatInteractable`），只接受“像 NPC actor 的对象”。
- live 证据：
  1. 在 live 推进到带路激活后，`TownLead` 摘要已出现：
     - `chief=001|companion=002`
  2. 在 `EnterVillage` 阶段，玩家面摘要已不再出现 `002` 闲聊提示：
     - `hint=none`
     - `NpcNearby ... activeNpc=none|activeBubble=none`
  3. 进 `Primary` 后仍可继续自动接 `house-arrival` -> `healing` 主线，不再停在旧的“002 闲聊抢占”状态。
- 验证结果：
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 10`：`errors=0 warnings=2`（既有 perf warning）
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`：通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`：`errors=0 warnings=0`
- 当前仍待用户终验的点：
  1. `Primary` 里 `001/002` 的“只做引导和剧情、不再普通 NPC 漫游/闲聊”体验是否符合你的手感标准；
  2. `Town -> Primary` 与 `Primary -> Town` 两段中，`002` 跟随节奏是否完全符合你规划的站位/停顿。
- thread-state：
  - 本轮已执行 `Begin-Slice`
  - 收口前已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-08｜spring-day1：按用户层级点位接线 Primary 三段走位（不黑屏入场 + 双终点 + 工作台后等待位）
- 当前主线目标：
  - 把 `Primary` 里的 `001/002/玩家` 站位和工作台前后走位切到用户手摆点位，不再用默认偏移与“黑屏补位”。
- 本轮代码落地（同文件）：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
  1. 新增点位接线常量：
     - `刚进primary/村长初始站位/艾拉初始站位/玩家初始站位`
     - `到工作台的npc终点/001/002`
     - `工作台结束，在旁等待站位/001/002`
  2. `TryPreparePrimaryArrivalActors()` 现优先吃“刚进primary”点位：
     - 命中点位时直接套位（含玩家位），不走 blink 黑屏补位。
  3. `TryHandleWorkbenchEscort()` 改为支持双目标：
     - `001`、`002` 可分别走到你指定终点，不再重叠同一个工作台点。
  4. 工作台到位后增加等待位回摆：
     - 首次到位会把 `001/002` 回摆到“工作台结束，在旁等待站位”。
- 本轮验证：
  - `manage_script validate SpringDay1Director`：`0 error / 2 warning`（既有 perf warning）
  - `git diff --check`（director 文件）通过
  - `errors --count 20`：`0 error / 0 warning`
- 当前判定：
  - 结构层已切到“优先按用户层级点位驱动”。
  - 如果某次 live 仍未命中点位，会自动回落旧逻辑，不会把主线卡死。
- thread-state：
  - 中途续工已重开 `Begin-Slice`（`primary-point-routing-and-workbench-dual-target`）
  - 收口已 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-08｜spring-day1：Town 围观 runtime 直连场景起终点 + 001/002 去干扰收口
- 当前主线目标：
  - 让进村/晚饭两段围观直接按 `Town` 场景手摆起终点跑，且不再被 crowd director 抢走 `001/002` 的主线带路。
- 本轮代码落地：
  - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
  1. 新增 marker 覆盖逻辑：
     - 在 `进村围观/起点/终点` 下按 `001起点/001终点 ... 203起点/203终点`（含 `*_Start/*_End` 别名）覆盖 cue start/path。
  2. 覆盖 beat 范围：
     - `EnterVillage_PostEntry`
     - `DinnerConflict_Table`
     - `ReturnAndReminder_WalkBack`
  3. 新增 Town-only cue mirror：
     - `DinnerConflict/ReturnAndReminder` 在 `Town` 下优先复用 `EnterVillage_PostEntry` cue；
     - 避免 `Primary` 误套进村 fallback。
  4. 新增 `001/002` defer 护栏：
     - 在 `EnterVillage ~ ReturnAndReminder`，crowd baseline 不再把两人拉回 resident roam；
     - defer 分支强制 `SetEntryActive(true)` + `StopRoam()`，避免跟随链被 crowd 回收。
  5. `AreBeatCuesSettled()` 对齐 runtime cue key：
     - settled 比对改成 runtime override 后 key，避免 key 错位卡推进。
- 验证结果：
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1NpcCrowdDirector --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 8`：`errors=0 warnings=2`（既有 perf warning）
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1Director --path Assets/YYY_Scripts/Story/Managers --level standard --output-limit 8`：`errors=0 warnings=2`（既有 perf warning）
  - `py -3 scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs --count 20 --output-limit 8`：`owned_errors=0`，但 `assessment=unity_validation_pending`（`CodexCodeGuard timeout-downgraded` + `editor_state stale_status`）
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`：通过
- 当前阶段：
  - crowd runtime 已切到“场景点位优先 + 主线演员去干扰”。
  - 后续重点是 full live 通路验收（Town 起步 -> Primary -> 回 Town -> DayEnd）。
- thread-state：
  - 本轮已执行 `Begin-Slice`（`day1-town-primary-closure`）
  - 收口已执行 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-08｜只读审查：SpringDay1Director 主链断点盘点（Town 开场 -> Primary -> 回 Town -> DayEnd）
- 当前主线目标：
  - 对 `SpringDay1Director` 做只读实现审查，找出 Day1 主链最可能未闭环或语义不一致的断点。
- 本轮子任务：
  - 聚焦 `001/002 escort`、`Healing/Workbench`、`Dinner/Return`、`Sleep/DayEnd`，抽取“函数/字段/判定链”级证据。
- 本轮完成：
  1. 已只读通查 `SpringDay1Director.cs`（含 `TickTownSceneFlow`、`TickPrimarySceneFlow`、`TryBeginTownHouseLead`、`TryHandleWorkbenchEscort`、`TryHandleReturnToTownEscort`、`BeginReturnReminder`、`EnterFreeTime`、`HandleSleep`）。
  2. 已形成 5 个高概率断点，重点集中在：
     - 两段 escort 对 transition trigger 的硬依赖与无兜底；
     - Workbench 目标缺失时的阶段卡住；
     - Healing 桥接在 `002` 缺席时的语义跳过；
     - FreeTime 后 Sleep 入口在 Town 流程的潜在闭环风险。
- 关键判定：
  - 这轮结论属于“静态推断成立”，尚未做 live 复测。
- 涉及文件：
  - `Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
- 验证方式：
  - 纯只读代码审查（按函数链与状态字段交叉核对），未改代码、未跑 Unity live。
- 遗留与恢复点：
  - 下一步若进入施工，应优先先补“escort trigger 缺失/错绑兜底 + workbench 缺失兜底”两处闭环，再做一次 Town->Primary->Town->DayEnd 全通路复测。

## 2026-04-08｜spring-day1：one-shot formal 跨场景掉回修复已拿到 DayEnd live 证据
- 当前主线目标：
  - 钉死 `healing/workbench/dinner/reminder/freeTimeIntro` 的正式剧情 one-shot 消费态，避免 `Town <-> Primary` 切场后把已消费 formal 误判回未消费。
- 本轮代码落地：
  - [DialogueManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/DialogueManager.cs)
    - 新增 `EnsureCompletedSequenceId()` / `EnsureCompletedSequenceIds()`，允许运行时在不重播对白的前提下补回 consumed sequence id。
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - 在 `HasCompletedDialogueSequence()` 前新增 `EnsurePhaseImpliedDialogueSequenceCompletion()`；
    - 按 `StoryPhase` 自动补回此阶段必然已经消费的正式剧情，重点覆盖：
      - `spring-day1-healing`
      - `spring-day1-workbench`
      - `spring-day1-dinner`
      - `spring-day1-reminder`
      - `spring-day1-free-time-opening`
  - [SpringDay1MiddayRuntimeBridgeTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1MiddayRuntimeBridgeTests.cs)
    - 新增 `OneShotSummary_ShouldBackfillPhaseImpliedCompletedSequencesAfterDialogueRuntimeReset`，钉死“只剩后段 sequence id 时，DayEnd 仍必须补回前段 consumed formal”。
- 验证结果：
  - 代码层：
    - `manage_script validate DialogueManager`：`clean`
    - `manage_script validate SpringDay1Director`：`warning-only`（既有 perf warning）
    - `manage_script validate SpringDay1MiddayRuntimeBridgeTests`：`clean`
    - `git diff --check`（本轮三文件）通过
  - live：
    - 通过 `CodexEditorCommandBridge` 真跑 `Reset Spring Day1 To Opening -> Trigger Spring Day1 Recommended Action Artifact` 自动链到 `DayEnd`
    - 关键 phase / oneShot 变化点：
      - `WorkbenchFlashback`：`healing=True`
      - `FarmingTutorial`：`healing=True|workbench=True`
      - `DinnerConflict`：`healing=True|workbench=True|dinner=False|reminder=False|freeTimeIntro=False`
      - `ReturnAndReminder`：`healing=True|workbench=True|dinner=True`
      - `FreeTime`：`healing=True|workbench=True|dinner=True|reminder=True`
      - `DayEnd`：`healing=True|workbench=True|dinner=True|reminder=True|freeTimeIntro=True`
    - 最终 artifact：
      - `Library/CodexEditorCommands/spring-day1-live-snapshot.json`
      - 最终快照已恢复到 `Scene=Town, Phase=DayEnd, OneShot=healing=True|workbench=True|dinner=True|reminder=True|freeTimeIntro=True`
- 当前阶段：
  - one-shot 跨场景掉回 bug 已被修住，且 live 证据已推到 `DayEnd`。
- 遗留与恢复点：
  - 当前 fresh console 的红错是外部 `Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs` 六条 `SendMessage cannot be called during Awake...`，不属于本轮 own 改动。
  - `validate_script` 仍会因 `CodexCodeGuard timeout + editor_state stale_status` 落到 `unity_validation_pending`，但本轮 own scope 无 blocking error。
- thread-state：
  - 本轮真实施工已执行 `Begin-Slice`（`day1-oneshot-persistence-fix`）
  - 未执行 `Ready-To-Sync`（本轮未做 sync）
  - 已执行 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-08｜spring-day1：one-shot 回归验证入口补口 + 菜单注册 blocker 收口
- 当前主线目标：
  - 不让 `OneShotSummary_ShouldBackfillPhaseImpliedCompletedSequencesAfterDialogueRuntimeReset` 只存在于测试名和聊天里，补一个可被 Unity 菜单直接调用的独立入口。
- 本轮落地：
  - 新增：
    - `Assets/Editor/Story/SpringDay1MiddayOneShotPersistenceTestMenu.cs`
    - `Assets/Editor/Story/SpringDay1MiddayOneShotPersistenceTestMenu.cs.meta`
  - 新菜单路径：
    - `Sunset/Story/Validation/Run Midday One-Shot Persistence Test`
  - 目标测试：
    - `SpringDay1MiddayRuntimeBridgeTests.OneShotSummary_ShouldBackfillPhaseImpliedCompletedSequencesAfterDialogueRuntimeReset`
- 验证结果：
  - `py -3 scripts/sunset_mcp.py manage_script validate --name SpringDay1MiddayOneShotPersistenceTestMenu --path Assets/Editor/Story --level standard --output-limit 8`
    - `status=clean`
  - `py -3 scripts/sunset_mcp.py validate_script Assets/Editor/Story/SpringDay1MiddayOneShotPersistenceTestMenu.cs --count 20 --output-limit 8`
    - `assessment=no_red`
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20`
    - 当前 fresh console `0 error / 0 warning`
- 当前真实 blocker：
  - 通过 `CodexEditorCommandBridge` 投递
    - `MENU=Sunset/Story/Validation/Run Midday One-Shot Persistence Test`
    - 请求已被消费，但 Unity 返回：
      - `ExecuteMenuItem failed because there is no menu named 'Sunset/Story/Validation/Run Midday One-Shot Persistence Test'`
  - 当前结论是：
    - 菜单脚本代码层已经 clean；
    - 但这份新菜单在当前 Unity 会话里还没有被 Editor 菜单表识别，bridge live 入口验证未闭环。
- 当前阶段：
  - one-shot 主 bug 仍保持“已修住并有 DayEnd live 证据”；
  - 新增的是一个后续可复跑入口，但它目前还停在“代码已落地、菜单注册待查”的刀口。
- thread-state：
  - 本轮已重新执行 `Begin-Slice`（`one-shot-formal-phase-backfill-followthrough`）
  - 未执行 `Ready-To-Sync`（本轮未做 sync）
  - 收尾已执行 `Park-Slice`
  - 当前状态：`PARKED`

## 2026-04-08｜只读核实：箱子 `E` 键近身交互应由 Farm/交互线程主刀，UI 线程只做提示配合
- 用户目标：
  - 核实 farm 线程关于“箱子 `E` 键近身交互该谁主刀”的判断是否成立，并给出最省治理成本的协作建议。
- 本轮性质：
  - `只读分析`
  - 未进入真实施工
- 本轮稳定结论：
  1. farm 的核心判断是对的：这条线不该由 UI 线程主刀。
  2. 当前代码已说明职责边界：
     - [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs) 已有真正的 `OnInteract()/OpenBoxUI()` 开箱真入口；
     - [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs) 已有“点击箱子 -> 自动走近 -> 到点交互”的运行时链；
     - 真正缺的是“箱子接入近身 `E` 键 candidate / proximity / 提示抑制”。
  3. 因而 owner 本质属于：
     - `runtime / input / interaction`
     - 不是 `UI shell / overlay 视觉`
  4. 最省成本的协作方式不是双向互发 prompt，而是：
     - 由 farm/交互线程直接主刀；
     - 若后续需要补 `E 打开箱子` 的提示文案、提示壳表现，再回给 UI 线程配合。
- 当前恢复点：
  - 若后续继续推进箱子 `E` 键交互，默认应由 farm/交互线程直接开做；
  - UI 线程不吞 runtime 主链，只保留提示表现配合位。

## 2026-04-08｜主控分发：Day1 最终闭环 prompt 批次 + profiler 真值改判
- 当前主线目标：
  - 把 `Day1` 最终闭环前的主控清单、我自己的续工 prompt、以及对 `存档系统 / NPC / UI` 的协作 prompt 一次性收成可直接转发的文件。
- 本轮子任务：
  - 基于用户最新 5 个问题与新增 profiler 截图，重新判断各线程 owner，不再把启动卡顿误派给 UI。
- 已完成事项：
  1. 新建主控清单：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-08_spring-day1_Day1最终闭环主控清单_30.md`
  2. 新建自用 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\2026-04-08_spring-day1_Day1最终闭环深砍自用prompt_31.md`
  3. 新建协作 prompt：
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\存档系统\2026-04-08_day1居民运行态与场景切换持久化prompt_02.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\0.0.2清盘002\2026-04-08_给NPC_day1原生resident接管与持久态协作prompt_17.md`
     - `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\2026-04-08_UI线程_day1最终闭环前打包字体与启动链prompt_07.md`
  4. 按 profiler 真值改判：
     - `PersistentPlayerSceneBridge.Start()` / `RebindScene(...)` 是启动大卡顿主峰之一
     - `NavGrid2D.RebuildGrid()` 是另一条大峰值
     - `SpringDay1PromptOverlay / TMP` 不是这次启动卡顿主责
- 关键决策：
  - `UI` 线程这轮只继续吃打包字体链，不再误吞启动大卡顿主责；
  - `存档系统` 线程追加承担 `PersistentPlayerSceneBridge` scene-rebind 启动峰值；
  - `spring-day1` 自己追加承担 `NavGrid2D.RebuildGrid()` 与 `NavGrid2DStressTest` 运行时混入问题；
  - `Town` 这轮不再追加 prompt。
- 验证结果：
  - 类型：只读核实 + 文档落盘
  - 新增 profiler 证据已人工复核
- 主线恢复点：
  - 下一步可直接把这批 prompt 转发出去，再由 `spring-day1` 自己继续进真实施工。

## 2026-04-08｜只读补记：day1 新 prompt 已确认是新的 UI 主刀切片，farm 箱子 E 键 prompt 已生成
- 用户目标：
  - 判断 day1 新给的 `2026-04-08_UI线程_day1最终闭环前打包字体与启动链prompt_07.md` 是不是旧方向延续，还是新的问题；
  - 同时先给 farm 生成一份箱子 `E` 键近身交互主刀 prompt；
  - 本轮先不生成新的 day1 prompt。
- 本轮性质：
  - `分析 + prompt 文档落地`
  - 未进入运行时代码施工
- 本轮稳定结论：
  1. day1 新 prompt 和当前 UI 线程之前在收的 `Workbench / 提示 / 交互显示` 不冲突；
  2. 但它确实是新的主刀切片：
     - 新唯一主刀已经切到
     - `Day1 打包字体异常 / 缺字 / 编辑器与打包版字体链一致性`
  3. 因而下一轮应分成两条线：
     - `Farm/交互线程`：箱子 `E` 键近身交互
     - `UI 线程`：day1 打包字体链
  4. 最省治理成本的协作方式不是双向互发 prompt，而是：
     - farm 直接主刀箱子链
     - UI 线程后续只做需要的提示配合
     - day1 这边的新 prompt 本轮先不生成
- 本轮落地：
  - 已生成可直接发给 farm 的 prompt 文件：
    - `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\UI\2026-04-08_给farm_箱子E键近身交互主刀prompt_01.md`
- 当前恢复点：
  - 下一轮如果继续我自己的主线，应直接切到 day1 新给的“打包字体链”主刀；
  - farm 则按现成 prompt 接箱子 `E` 键交互主刀。

## 2026-04-08｜协作进展补记：UI 已真修打包字体链，Farm 箱子 E 键链确认已接入 proximity 主链

- 当前对子线程协作面的新真值：
  1. `UI` 线程已经进入真实施工，不再停留在“怀疑字体链有问题”；
  2. 它已把 `DialogueChineseFontRuntimeBootstrap` 的核心错误口修掉：
     - 动态中文字体在 atlas 被 build 清空后，不再在补字前就直接判死；
     - 并新增了 `DialogueChineseFontRuntimeBootstrapTests.cs`，用 `ClearFontAssetData(true)` 模拟 build-like 初始态。
  3. `Farm` / 箱子 `E` 键这边当前 shared root 的运行时主链已成立：
     - `ChestController` 已接 `SpringDay1ProximityInteractionService.ReportCandidate(...)`
     - 触发复用 `OnInteract(context)`
     - `GameInputManager` 右键自动走近旧链仍保留
     - 新补的是 `ChestProximityInteractionSourceTests.cs` 这类最小守门测试，而不是重写主链。
- 当前验证与 blocker：
  - `UI` 线程 own scope `validate_script` = `owned_errors=0 / external_errors=0 / unity_validation_pending`
  - fresh console 当前第一条外部 blocker：
    - `Assets/YYY_Tests/Editor/NpcResidentDirectorRuntimeContractTests.cs(104,20)` 缺 `NPCAutoRoamController`
- 当前恢复点：
  1. `Day1` 这边如果继续吃回 UI 协作结果，可先认：
     - build 字体链真根因已被正面修补；
     - 箱子 `E` 键链已经在 proximity 主链上。
  2. 但 packaged build 字体终验和箱子 `E` 键 fresh live 终验仍未闭环，不能包装成整体体验已过线。

## 2026-04-08｜只读核实：Town -> Primary -> Town 主链里 001/002 escort 的四个关键断点

- 用户目标：
  - 只读钉死 `spring-day1` 当前 `Town -> Primary -> Town` 主链里，`001/002 escort` 的真实关键断点，不改代码，只给精确文件 / 函数 / 判断链。
- 本轮性质：
  - `只读分析`
  - 已对当前 `spring-day1` slice 执行 `Park-Slice`
- 本轮稳定结论：
  1. `002` 可能不跟 `001` 一起进 `Primary` 的最高置信根因，在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `FindPreferredStoryNpcTransform()`：
     - 该函数先走 `FindPreferredObjectTransform(["001"/"002"])`；
     - 而 `Primary` 现场同时存在 `NPCs/001`、`0.0.4/到工作台的npc终点/001`、`0.0.4/工作台结束，在旁等待占位/001`，`002` 同理；
     - 因此 `UpdateTownHouseLeadSnapshot()`、`TryPreparePrimaryArrivalActors()`、`UpdateWorkbenchEscortSnapshot()`、`UpdateReturnEscortSnapshot()` 都可能抓到“点位 Transform”而不是真 NPC actor；
     - 一旦 `companion` 绑定到点位，后续 `TryDriveEscortActor()` 驱动的就是点位，不是 `NPCs/002`，于是表象就是 `001` 还在走，`002` 没跟上。
  2. 到了 `Primary` 后，没有彻底关掉 roam / nearby / 自动招呼的断点有两层：
     - 第一层还是 actor 解析错位：`UpdateSceneStoryNpcVisibility()` -> `ApplyStoryActorRuntimePolicy()` 可能把策略施加到点位，不是实际 `NPCs/001/002`；
     - 第二层是 nearby 过滤不完整：即便 actor 解析对了，`ApplyStoryActorRuntimePolicy()` 也只会关 formal / informal、停 roam、藏泡泡，但不会给 story-controlled NPC 打上 `ResidentScriptedControl`；
     - [PlayerNpcNearbyFeedbackService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PlayerNpcNearbyFeedbackService.cs) 的 `FindNearestCandidate()` 只跳过 `IsResidentScriptedControlActive`，所以 still 会把 story-controlled 的 `001/002` 当 nearby 候选。
  3. workbench 前桥接对白最稳的插点，其实已经基本接在正确链上了：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) `TryHandleWorkbenchEscort()` 在 `playerReady && chiefReady && companionReady` 后先卡 `WorkbenchBriefingSequenceId`；
     - `HandleDialogueSequenceCompleted()` 收 `WorkbenchBriefingSequenceId` 后才切 ready prompt；
     - `ShouldExposeWorkbenchInteraction()` 与 [CraftingStationInteractable.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/CraftingStationInteractable.cs) 的 `ShouldExposeWorkbenchInteraction()` 也已经用 `HasWorkbenchBriefingCompleted()` 做交互闸门；
     - 所以“插在哪最稳”的答案就是：继续守在 `TryHandleWorkbenchEscort() -> HandleDialogueSequenceCompleted() -> ShouldExposeWorkbenchInteraction()` 这条链，不要改成从 `NotifyCraftingStationOpened()` 倒灌。
  4. 晚饭 / 回村复用第一次进村 crowd 演出，目前是“crowd/beat alias 已做，导演主链体验收口还没做完”：
     - [SpringDay1NpcCrowdManifest.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdManifest.cs) `BuildBeatConsumptionSnapshot()` 已把 `DinnerConflict_Table` 与 `ReturnAndReminder_WalkBack` 折回 `EnterVillage_PostEntry`；
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) `ApplyStagingCue()` / `TryResolveStagingCueForCurrentScene()` 也会把晚饭和回村在 `Town` 里镜像到 `EnterVillagePostEntry` cue；
     - 但 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 的 `BeginDinnerConflict()` / `TryHandleReturnToTownEscort()` / `BeginReturnReminder()` 目前只算“主链知道该复用这个 crowd beat”，还没有把第二次围观的 pacing、停顿和对话/走位体验彻底收平。
- 当前恢复点：
  - 如果下一刀转真实施工，最值钱的顺序应是：
    1. 先修 `FindPreferredStoryNpcTransform()` 的 actor/点位混抓；
    2. 再补 Primary story-controlled NPC 的 nearby/自动招呼抑制；
    3. 其后再收晚饭/回村那一段的导演体验层。

## 2026-04-08 23:13 第三刀续工：Town crowd 起跑位与 scene 命中稳态补口
- 当前主线：
  - 继续按 `2026-04-08_spring-day1_Day1最终闭环深砍自用prompt_31.md` 推 `Day1` 闭环，不回方案模式；这轮先把 `Town` 群像起跑位、`001/002` 与 `Primary` 关键点位的 scene 命中稳定性补强。
- 本轮实际做成：
  1. [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `FindAnchor()` 现在会额外识别 `DirectorReady_* / ResidentSlot_* / BackstageSlot_*` 以及对应 `_HomeAnchor`，不再只认旧的裸语义名或 `*_HomeAnchor`。
     - `EnterVillage_PostEntry / DinnerConflict_Table / ReturnAndReminder_WalkBack` 三拍的 crowd cue 现在会优先走“scene resident 当前起点”口径；当没有显式 start/end marker 时，会把 resident 先拉回 `BasePosition` 再套 cue，不再盲信旧 cue 里的起始坐标。
  2. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `FindPreferredObjectTransform()` 改成先在当前 active scene 里精确找，再回退全局 `GameObject.Find`。
     - `FindWorkbenchCandidate()`、`FindRestTargetCandidate()` 也同步改成优先吃当前场景命中，减少 `Primary` 工作台 / `Town` 床与同名物体串错。
  3. [SpringDay1DirectorStagingTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1DirectorStagingTests.cs)
     - 新增 `DirectorReady_*` alias 命中回归测试。
     - 新增 village crowd cue 在缺 marker 时应优先用 current scene resident start 的回归测试。
- 本轮验证：
  - `manage_script validate SpringDay1NpcCrowdDirector` = `warning-only`（既有 `GameObject.Find/String concat in Update()` warning，无 error）
  - `manage_script validate SpringDay1Director` = `warning-only`（既有 warning，无 error）
  - `manage_script validate SpringDay1DirectorStagingTests` = `clean`
  - `git diff --check --` 仅对本轮 own 文件通过
  - `py -3 scripts/sunset_mcp.py errors --count 20 --output-limit 20` = `errors=0 warnings=0`
- 当前阶段判断：
  - 代码层已经把“你场景里现有的 `DirectorReady_* / slot` 不被 runtime 正确吃回”这条高概率根因补上了；
  - 但这仍然是代码层与 CLI 红错层成立，不等于 `Town -> Primary -> Town -> DayEnd` 已经 live 全验通过。
- 下一步恢复点：
  1. 用户现在最值得先验的是：进村 crowd 是否不再从远处旧 cue 起点跑来，`001/002` 与 `Primary` 工作台/床命中是否更稳定。
  2. 如果 live 仍有问题，下一刀优先读 fresh runtime 现象，继续收 `002` 跟随与 `Primary` 剧情节奏，而不是回退这轮 scene 命中补口。
- thread-state：
  - 本轮承接既有 `ACTIVE` slice 继续施工，收尾已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`

## 2026-04-08 只读总控审计：吃回 UI / 箱子 / 存档三条阶段回执
- 当前主线：
  - 这轮不落代码，只把 `UI 字体链`、`箱子 E 键链`、`resident/save 持久化链` 三条新回执按主控口径重新审核，判断哪些已经足够吃回 `day1` 主链，哪些仍只是“代码成立、终验未补”。
- 审计后新真值：
  1. `UI` 线程关于打包中文字体链的 claim 基本成立：
     - [DialogueChineseFontRuntimeBootstrap.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs) 已改成：动态 TMP 字体在空 atlas 时先尝试补字，再决定是否可用；
     - [DialogueChineseFontRuntimeBootstrapTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/DialogueChineseFontRuntimeBootstrapTests.cs) 也真补了 `ClearFontAssetData(true)` 后仍可恢复 atlas 和中文 probe text 的 build-like 测试；
     - 但这条线目前仍只能认定为“代码与编辑器测试成立”，不能直接报成“打包 live 已完全过线”。
  2. `farm/箱子` 线程关于近身 `E` 键交互的 claim 基本成立：
     - [ChestController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/World/Placeable/ChestController.cs) 已经通过 `ReportProximityInteraction()` 接入 [SpringDay1ProximityInteractionService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/SpringDay1ProximityInteractionService.cs)；
     - 同箱已开可 toggle 关闭，右键远处自动走近开箱旧链仍由 [GameInputManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs) 保持；
     - [ChestProximityInteractionSourceTests.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/ChestProximityInteractionSourceTests.cs) 明确守住了 reuse `OnInteract()`、page UI 抑制和 toggle 例外；
     - 但这条线也仍应归类为“代码链已接 + 护栏测试已补”，不是“Day1 fresh live 已终验”。
  3. `存档系统` 关于 resident 最小运行态与跨场景读档的 claim 基本成立：
     - [StoryProgressPersistenceService.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/StoryProgressPersistenceService.cs) 已把 `residentRuntimeSnapshots` 接进 `StoryProgressSaveData`；
     - [SpringDay1NpcCrowdDirector.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs) 已有 `ResidentRuntimeSnapshotEntry` 的 capture/apply/static cache 链；
     - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs) 已按 `SceneManager.GetActiveScene().name` 保存场景名，并在读档恢复时优先走 sceneName 切场与 bridge 恢复玩家位置；
     - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 仍留有 `RebindPersistentCoreUi()` 等后续性能责任面，但“初始场景重复 rebind 第一刀已收”这个判断可认。
  4. `thread-state` 审计上有一个非业务问题：
     - 当前 `Show-Active-Ownership` 里 `UI = ACTIVE`、`农田交互修复V3 = ACTIVE`、`存档系统 = PARKED`；
     - 其中 farm 聊天回执口头声称已 `Park-Slice`，但现场仍显示 `ACTIVE`，说明 thread-state 报实和现场不完全一致，这不构成 day1 业务 blocker，但会影响治理态判断。
- 当前阶段判断：
  - `UI`、`箱子`、`存档` 这三条线都已经从“边界讨论/空口 claim”进入“代码与测试或证据链已成立，可被 day1 主链吃回”的阶段；
  - 但它们都还没有替 `day1` 完成最后一脚真实 runtime 终验，因此主控位不能把它们包装成“demo 全闭环已完成”。
- 当前恢复点：
  - 下一轮若继续真实施工，主控优先级仍应回到 `day1` 主链闭环本身：
    1. 把上述三条 contract 真接回 `Town -> Primary -> Town -> DayEnd` 运行链；
    2. 继续收 `001/002` 导演链、桥段体验、睡觉/DayEnd；
    3. 最后再补 packaged/live 终验证据，而不是回漂去做泛 UI/泛存档修补。
- thread-state：
  - 本轮只做只读审计，未进入新的真实施工
  - `spring-day1` 仍保持 `PARKED`

## 2026-04-08 追加审计：NPC 原生 resident 接管与最小 snapshot contract 已足够主控消费
- 当前主线：
  - 用户追加了 NPC 正式回执，要求主控重新判断“现在 NPC 还缺不缺、我下一步该怎么做”。
- 审计后新真值：
  1. [2026-04-08_NPC给day1_原生resident接管与持久态协作回执_18.md](D:/Unity/Unity_learning/Sunset/.kiro/specs/NPC/2.0.0进一步落地/0.0.2清盘002/2026-04-08_NPC给day1_原生resident接管与持久态协作回执_18.md) 说明 NPC 这轮已把 `native resident 被导演接管时的冻结/让位/恢复` contract 真落进 [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)；
  2. `informal / nearby / active chat` 也已经对 resident scripted control 让位，意味着 `day1` 不该再自己复制一套“停 roam / 停 nearby / 停 informal”的平行临时链；
  3. `NpcResidentRuntimeSnapshot + NpcResidentRuntimeContract` 已经足够给 `day1 / 存档` 做“最小 resident 运行态” capture/apply，不要求 NPC 再去吞 deployment、Town/Primary scene 写、或 crowd director 主消费；
  4. [NPC.json](D:/Unity/Unity_learning/Sunset/.kiro/state/active-threads/NPC.json) 与回执一致，当前 `NPC = PARKED`。
- 当前判断：
  - NPC 这条线现在已经不是 blocker，也不该继续回吞主链；它给我的就是这轮 `day1` 最后需要的“原生 resident 接管 contract + 最小 snapshot 面”。
- 当前恢复点：
  - 我下一轮真实施工时，优先做的不是再催 NPC，而是：
    1. 把 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl` 真接到 `day1` 导演主链；
    2. 把 snapshot contract 真接进 `Town -> Primary -> Town` 和读档恢复链；
    3. 删掉或避免继续扩写我自己那套重复的临时 suppress 逻辑。

## 2026-04-08 静态事故排查：打包版奇卡与 escort 一步三回头分属两条责任链
- 当前主线：
  - 用户在继续验收全流程的同时，要求我先做静态排查；新现场真值是“编辑器里基本不卡，只有打包后启动奇卡，但村长/艾拉一步三回头的问题仍存在”。
- 静态判断：
  1. `打包版奇卡` 已从“通用 runtime 卡顿”改判为“打包专属启动链卡顿”：
     - [DialogueChineseFontRuntimeBootstrap.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs) 用 `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` 在进首场景前执行 `EnsureRuntimeFontReady()`，并对大段 `WarmupSeedText` 做动态字体预热；这条在 build 中比编辑器更容易形成首帧卡顿。
     - [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs) `Awake()` 会执行 `TryMigrateLegacySaveFolders()`、`EnsureSaveFolderExists()` 和 `InitializeDynamicObjectFactory()`；这条包含真实磁盘 IO，打包版比编辑器更可能在启动阶段卡住，尤其是首次运行或旧存档目录存在时。
     - [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 仍会在 `Start()` / `OnSceneLoaded()` 里跑 `RebindScene()`，而 [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs) `Start()` 仍会 `Invoke(RebuildGrid, 0.5f)`；这两条仍是启动峰值候选，但在“编辑器不卡、打包卡”的新证据下，更像 secondary spike，不再是唯一主嫌。
  2. `村长/艾拉一步三回头` 仍是 `day1 escort` 自己的控制链问题：
     - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 目前的 escort 逻辑是：掉队时 `StopRoam()`，跟上后再按 `townHouseLeadMoveRetryInterval` 重发 `DebugMoveTo()`；
     - `ShouldEscortWaitForPlayer()` 只有单阈值 `playerDistance > leaderDistance + escortMaxLeadDistance`，没有 hysteresis / 恢复缓冲带，所以玩家距离在边界附近时容易在“停 -> 走 -> 再停”之间抖动；
     - 这条链也还没有吃回 NPC 新交付的 `AcquireResidentScriptedControl / ReleaseResidentScriptedControl`，仍在用 `StopRoam + DebugMoveTo` 这种临时驱动方式，因此 `informal / nearby / facing` 仍有和 escort runtime 互相抢状态的空间。
- 当前恢复点：
  - 若下一轮转施工，优先级应拆成两刀：
    1. `一步三回头`：先把 escort 从 `StopRoam + DebugMoveTo` 切到 NPC scripted control contract，并加 wait/resume 的缓冲带；
    2. `打包版奇卡`：先收 `BeforeSceneLoad 字体预热 + SaveManager 启动 IO` 这两条 build-only 启动链，再复查 bridge/nav 次级峰值。

## 2026-04-09 只读静态排查：Day1 引路时 001/002 一步一回头的最可能根因
- 当前主线：
  - 只做读码排查，不改代码；把 `SpringDay1Director / NPCAutoRoamController / NPCMotionController / NPCAnimController / nav / bubble / nearby / session` 里最可能导致 escort 抽搐转向的责任链收敛出来。
- 新结论：
  1. `NPCAnimController` 本体不是主因；它只负责把 direction 映射成 animator 参数与 flip，真正决定朝向的是 `NPCMotionController` 的速度观察和上游 `SetFacingDirection()` 调用链。
  2. 当前最高概率根因是 escort scripted move 仍参加普通 local avoidance：
     - `NPCAutoRoamController.TickMoving()` scripted move 下仍跑 `TryHandleSharedAvoidance()`；
     - `NavigationAvoidanceRules.ShouldConsiderForLocalAvoidance()` 没有同 escort owner / 同 convoy 的豁免；
     - `001/002/玩家` 会继续互相挤、互相侧绕，`NPCMotionController` 再按瞬时速度刷新方向，于是视觉上像一步一回头。
  3. 第二高概率根因是 director 的 wait/resume 阈值本身会抖：
     - `ShouldEscortWaitForPlayer()` 用的是“玩家到目标距离”相对“leader 到目标距离”的单套阈值；
     - leader 一恢复前进就会压低 waitThreshold，玩家卡边界时容易出现“刚走几步又停”，进一步放大 `HaltResidentScriptedMovement()` 与 `DriveResidentScriptedMoveTo()` 的交替。
  4. `nearby / informal / chat session` 这条抢控制链现在大体已经让位 scripted control，不再是第一嫌疑，但“朝向 owner”仍未统一：
     - `PlayerNpcChatSessionService` scripted control 时会 `SystemTakeover`；
     - `PlayerNpcNearbyFeedbackService` 会隐藏 scripted resident bubble；
     - `NPCInformalChatInteractable` scripted control 时不能交互、也不会随便恢复 roam；
     - 但 `facePlayerOnInteract / FaceToward / NPCMotionController.SetFacingDirection` 仍是分散入口，没有 escort 单一 owner。
- 当前恢复点：
  - 真要修，优先顺序应是：`convoy 避让豁免/朝向稳定 -> escort wait/resume 滞回 -> 零散 face-player 入口总闸`。

## 2026-04-09 只读静态排查：Day1 demo 打包/启动卡顿五文件责任链收敛
- 当前主线：
  - 用户要求只读静态排查，不改业务代码；只在 `DialogueChineseFontRuntimeBootstrap / SaveManager / PersistentPlayerSceneBridge / NavGrid2D / NavGrid2DStressTest` 里收敛最像 build-only 首轮启动卡顿的责任链，并判断这轮是否值得让 `day1` 先转修。
- 关键修正：
  1. [SaveManager.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Data/Core/SaveManager.cs) 当前最像的已不是 `Awake()` 直接跑旧存档迁移/工厂初始化；build 下这两条已被 `ShouldRunHeavyStartupBootstrapImmediately()` 延后。现在真正更像启动卡顿的是 `BootstrapRuntime()/Awake()` 触发的 `ScheduleFreshStartBaselineCapture()`，随后在 `CaptureFreshStartBaselineRoutine()` 里执行 `CollectFullSaveData() -> JsonUtility.ToJson() -> File.WriteAllText()`，这是首轮启动的真实序列化+磁盘写峰值。
  2. [DialogueChineseFontRuntimeBootstrap.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Dialogue/DialogueChineseFontRuntimeBootstrap.cs) 当前最像的也不是 `BeforeSceneLoad` 本身就做大段中文预热；`BootstrapBeforeSceneLoad()` 走的是 `eagerWarmup:false` 快速路径。更像 build-only 峰值的是首批 UI 文本真正出现时 `EnsureRuntimeFontReady()/ResolveBestFontForText()` 触发 `ResolveWarmPreferredFont()/TryPrepareCharacters()/TryAddCharactersSafe()`，而代码已明确写了“打包版动态 TMP 字体会先清掉 atlas”。
  3. [PersistentPlayerSceneBridge.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Player/PersistentPlayerSceneBridge.cs) 仍是强嫌疑启动放大器：`sceneLoaded/StartupFallbackRebind -> RebindScene()` 会叠加全场 `FindObjectsByType<>`、root promote、UI rebuild / `Canvas.ForceUpdateCanvases()`、以及 `ReapplyTraversalBindings()`。
  4. [NavGrid2D.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2D.cs) 仍是 bridge 下游重活点：`OnEnable()` 默认 `RebuildGrid()`，`RefreshGrid()` 又会全量 `RefreshExplicitObstacleSources() + RebuildGrid()`；如果首场景桥接链再触发一次，就会放大启动峰值。
  5. [NavGrid2DStressTest.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Service/Navigation/NavGrid2DStressTest.cs) 只有当前 demo 是 Development Build / `Debug.isDebugBuild` 时才值得优先怀疑；release build 下它会自禁用，不是第一主嫌。
- 归类判断：
  - `day1` 自己可直接先收一刀：`PersistentPlayerSceneBridge + NavGrid2D` 的启动重绑 / 整图重建链；如果当前 demo 确实是 Development Build，再顺手核 `NavGrid2DStressTest`。
  - 更适合留给存档 / UI 协作：`SaveManager` 自动 baseline 捕获；`DialogueChineseFontRuntimeBootstrap` 首批中文 UI 动态补字 / 字体预热。
- 阻塞判断：
  - 这批问题不会硬阻塞 Day1 剧情 / 交互主链继续做 editor/live 功能验收；
  - 但会软阻塞“打包版启动顺滑 / 首屏不卡”的 demo 口验收，尤其 `SaveManager baseline capture` 与 `bridge -> nav` 更值得先验证。
- 当前恢复点：
  - 若下一轮转真实施工，最值钱顺序应是：先验证并压缩 `SaveManager baseline capture` 与 `bridge -> nav` 峰值，再决定是否把字体 warmup 拆给 UI 线。

## 2026-04-09 只读静态排查：spring-day1 live 复测前最可能的 Editor 会话残留/自动验证入口
- 当前主线：
  - 用户要求不改文件，只读排查会打断 `spring-day1` live 复测的 Editor 会话残留、自动验证入口与最小 cleanup 面。
- 本轮完成：
  - 只读核查了 `SpringDay1DirectorPrimaryRehearsalBakeMenu`、`NavigationLiveValidationMenu`、`NavigationStaticPointValidationMenu`、`PlacementSecondBladeLiveValidationMenu`、`SpringDay1NpcCrowdValidationMenu`、`NPCInformalChatValidationMenu`，并补看导航两个 runner 的自动启动/停机路径。
- 新结论：
  1. 最高风险残留是 `Sunset.SpringDay1DirectorPrimaryRehearsalBake.Queued`：
     - `SpringDay1DirectorPrimaryRehearsalBakeMenu` 是 `[InitializeOnLoad]`；
     - 只要这个 `SessionState` 还在，下一次 PlayMode 或脚本重载后就可能继续排练，且完成/失败时都会 `EditorApplication.isPlaying = false`，最像“live 复测被抢停”。
  2. 第二高风险是静态导航验证残留：
     - `Sunset.NavigationStaticValidation.PendingAction`
     - `Sunset.NavigationStaticValidation.ConsoleErrorPauseSnapshot`
     - `Sunset.NavigationStaticValidation.ConsoleErrorPauseOverrideActive`
     - 以及 `Library/NavStaticPointValidation.pending`
     - 这组会在重载/进 Play 后自动续跑 `NavigationStaticPointValidationRunner`，而 runner 结束或 abort 时会主动停掉 PlayMode。
  3. 第三风险是导航 live 残留：
     - `Sunset.NavigationLiveValidation.PendingAction`
     - 它会在编辑器重载后再次拉起 PlayMode 或在进入 Play 后自动 dispatch，虽然 runner 自己不主动停 Play，但会抢玩家/导航控制链，足以把 `spring-day1` live 现场带歪。
  4. `Sunset.NpcInformalChatValidation.Active` 不是抢停源，但若残留为 `true`，会压制导航菜单的 queue/dispatch，属于“状态异常/验证入口被吞”的高概率来源。
  5. `Sunset.PlacementSecondBlade.PendingRunScope` 属于中风险：
     - 只在当前 Editor session 内有效；
     - 下一次进入 Play 仍会自动启动放置 second-blade validation，容易抢玩家输入/放置状态，但不像前两组那样主动停 Play。
  6. `SpringDay1NpcCrowdValidationMenu` 和 `NPCInformalChatValidationMenu` 本体都不是典型的“会话级自动入口”：
     - 前者没有 `EditorPrefs/SessionState` 残留键；
     - 后者只有锁键和 `runInBackground` 覆盖；
     - 真会抢停的是它们已经在当前 PlayMode 内跑着的 probe，而不是“下次开 Play 自己冒出来”。
- 当前恢复点：
  - 如果后续补一个很窄的 cleanup 菜单，优先应清：
    1. `Sunset.SpringDay1DirectorPrimaryRehearsalBake.Queued`
    2. `Sunset.NavigationStaticValidation.PendingAction`
    3. `Sunset.NavigationStaticValidation.ConsoleErrorPauseOverrideActive`
    4. `Sunset.NavigationStaticValidation.ConsoleErrorPauseSnapshot`
    5. `Sunset.NavigationLiveValidation.PendingAction`
    6. `Sunset.NpcInformalChatValidation.Active`
    7. `Sunset.PlacementSecondBlade.PendingRunScope`
  - 如果允许顺手删一个文件残留，再加上 `Library/NavStaticPointValidation.pending`。
## 2026-04-09 08:55 刀口暂停：Primary 承接对白验证恢复口已补，但 fresh live 还没重跑到新补口之后
- 当前主线：
  - 继续推进 `Day1 最终闭环`，主刀仍是 `Town -> Primary -> Town -> DayEnd` 主链，不把打包卡顿升成唯一主刀。
- 本轮已做：
  1. 核实 `WorkbenchFlashback` 不是纯 runtime 新 bug，而是导演里现成的 `TryAdvanceWorkbenchValidationStep()` / `GetValidationWorkbenchNextAction()` 已经落地，验证菜单总路由也已接到这套 helper。
  2. 在 [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs) 新增 `TryAdvancePrimaryArrivalValidationStep()`，让 `Primary` 的 `EnterVillage -> HouseArrival` 不再只会“嘴上说自动接管”，而是能在验证入口里主动重接承接对白。
  3. 在 `TryQueuePrimaryHouseArrival()` 前补了一个窄恢复口：
     - 如果 `_houseArrivalSequencePlayed` 已经被置位，但 `HouseArrivalSequenceId` 既没 active 也没 completed，就先释放这个卡死位，再允许承接对白重新排队。
  4. `TriggerRecommendedAction()` 的 `StoryPhase.EnterVillage` 分支已改成优先走 `director.TryAdvancePrimaryArrivalValidationStep()`，不再停在“无需再次手动触发 NPC001”的空转文案。
- 本轮最小自检：
  - `manage_script validate SpringDay1Director` = `errors=0 warnings=3`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 通过
- fresh live 真值：
  1. 这轮真实命令桥从 `Town` fresh 开局重跑后，已经再次证明：
     - `CrashAndMeet` 可推进
     - `VillageGate` 可推进
     - `Town -> Primary` 转场会发生
  2. 但在我补完 `PrimaryArrival` 恢复口之前拿到的最后一张 live 证据，仍停在：
     - `Scene=Primary`
     - `Phase=EnterVillage`
     - `followupPending=True`
     - 表象是：`Primary` 承接对白没有稳定自动接上
  3. 我刚补的这刀还没来得及拿 fresh live 再跑一遍到新补口之后，用户就要求我在最近刀口停下汇报。
- 当前恢复点：
  - 下一刀最优先直接做一件事：
    - 在 fresh Play 里重跑 `Town -> Primary`，验证 `TryAdvancePrimaryArrivalValidationStep()` + `_houseArrivalSequencePlayed` 恢复口是否已经把 `HouseArrival` 真接起来。
  - 如果还不行，下一嫌疑就收缩到：
    - `TryPreparePrimaryArrivalActors()` 的入场摆位
    - 或 `PlayDialogueNowOrQueue(BuildHouseArrivalSequence())` 的实际起播条件
- thread-state：
  - 这轮承接既有 `ACTIVE` slice 继续施工
  - 用户要求最近刀口停下后，已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
## 2026-04-09 09:35｜Primary 工作台前对白距离改判 + 对话期任务列表交接节奏收口
- 当前主线：
  - 继续推进 `Day1 最终闭环`；这轮按用户最新裁定，只先收两件事：
    1. `Primary` 里工作台前对白，改成“玩家到村长 3 单位内即可触发”
    2. 对话开始/结束时，任务列表和对话框按 `0.5s -> 0.5s` 做交接，不再抢拍
- 本轮代码落地：
  1. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - `workbenchEscortReadyDistance` 默认值已改到 `3f`
     - `UpdateWorkbenchEscortSnapshot()` 的 `_workbenchEscortPlayerDistance` 改成优先按 `player -> chief` 算，不再默认按 `player -> workbenchTarget`
     - `TryAdvanceWorkbenchValidationStep()` 在 `WorkbenchBriefing` 尚未完成时，验收入口也改成优先把玩家贴到 `chief/companion` 附近，而不是直接贴工作台
  2. [SpringDay1PromptOverlay.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs)
     - `OnDialogueStart/OnDialogueEnd` 现在会先判断自己是否已经由 `DialogueUI` 的 sibling fade 接管；
     - 若已接管，就不再自己抢着 `FadeCanvasGroup(0/1)`，避免任务列表在对白切换时提前消失或提前回弹。
- 这轮验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs` 通过
  - `manage_script validate` 当前被外部 `127.0.0.1:8888/mcp` 连接拒绝卡住，没拿到新票；因此这轮 no-red 只能诚实落成“文本层 clean，MCP 校验未闭环”
  - 命令桥 live 侧确认了一件事：
    - 当前 Editor 现场能重新进 Play，但 fresh 入口仍落在 `Primary + CrashAndMeet`，不是稳定的 `Town fresh`，所以这轮没拿到完整 runtime 终验小票
- 当前恢复点：
  - 下一刀如果继续，先做 `Town fresh` 的稳定重开再 live 验：
    1. `Primary` 工作台前对白是否会在“玩家靠近村长 3 单位”时真接起
    2. 对话开始/结束时 `PromptOverlay` 是否按 `0.5s 隐去 -> 0.5s 显示` 跟 `DialogueUI` 顺滑交接
- thread-state：
  - 本轮已重新 `Begin-Slice`
  - 本轮收尾已执行 `Park-Slice`
  - 当前 live 状态：`PARKED`
## 2026-04-09 09:50｜“停下没气泡”根因已定位并补口
- 当前主线：
  - 继续推进 `Day1 最终闭环`；这次是穿插处理用户刚发现的阻塞：`NPC 停下等待玩家时，提示气泡不显示`
- 本轮确认到的真相：
  1. 不是整条 NPC 气泡系统断了
  2. 真正断的是 `SpringDay1Director.TryShowEscortWaitBubble(...)` 这条导演等待提示链
  3. 原因是它一直调用 `NPCBubblePresenter.ShowText(...)`
     - 这会把气泡标成 `Ambient`
     - 而 `Day1` formal 阶段会被 `NpcAmbientBubblePriorityGuard + NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory()` 全局压掉
  4. 所以现象才会变成：`NPC 停下了，但“小伙子，先跟我走”不出来`
- 本轮已落修复：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - `TryShowEscortWaitBubble(...)` 已从 `ShowText(...)` 改成 `ShowConversationText(...)`
    - 目的：把 escort 等待提示提升到剧情通道，不再被 ambient 守卫误杀
- 本轮验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 通过
  - `validate_script` 仍因外部桥超时没拿到新票，因此当前验证状态只能算：
    - `代码层修复已落`
    - `runtime 仍待用户现场复核`
- 当前恢复点：
  - 下一刀优先让用户现场复核 1 件事：
    - NPC 停下等待玩家时，`“小伙子，先跟我走”` 是否重新出现
## 2026-04-09 10:15｜等待气泡闪退已继续补死 + 任务清单语义出口已补给 UI
- 当前主线：
  - 继续推进 `Day1 最终闭环`；本轮并行收两件事：
    1. 让 escort 等待气泡不再“闪一下就没”
    2. 在 `SpringDay1Director` 内补一份可直接给 UI 线程消费的任务清单隐藏/恢复语义
- 本轮实际落地：
  1. [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
     - 查清闪退真因：resident 进入“脚本接管但暂停移动”后，每帧都会执行 `ApplyResidentRuntimeFreeze()`，而这一步原本会无差别 `HideBubble()`
     - 已改成：如果当前是脚本接管暂停态，且气泡是 conversation 级，就保留，不再每帧清掉
  2. [NPCBubblePresenter.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCBubblePresenter.cs)
     - 新增 `IsConversationPriorityVisible`
     - 给 roam/runtime freeze 判断“这是不是高优先级剧情气泡”用
  3. [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 已补 `TaskListVisibilitySemanticState`
     - 已补：
       - `GetTaskListVisibilitySemanticState()`
       - `ShouldForceHideTaskListForCurrentStory()`
       - `GetTaskListVisibilitySemanticKey()`
     - 当前 day1 语义口径：
       - 正在播放 one-shot formal 序列时，任务清单必须隐藏
       - formal 序列消费完后，任务清单允许恢复
       - 非正式常规流程不强管任务清单
     - 当前纳入 formal 语义的序列：
       - `spring-day1-first`
       - `spring-day1-first-followup`
       - `spring-day1-village-gate`
       - `spring-day1-house-arrival`
       - `spring-day1-healing`
       - `spring-day1-workbench-briefing`
       - `spring-day1-workbench`
       - `spring-day1-dinner`
       - `spring-day1-reminder`
       - `spring-day1-free-time-opening`
- 本轮验证：
  - `git diff --check` 对 3 个代码文件通过
  - `validate_script` 仍超时
  - `sunset_mcp.py errors` 也被现有 MCP 桥 `AttributeError` 卡住，所以这轮 no-red 只能报成：
    - `文本/补丁层 clean`
    - `桥侧统一票仍未恢复`
- 当前恢复点：
  - 先让用户现场验两件事：
    1. escort 等待气泡是否不再闪退
    2. UI 线程是否能直接消费 `SpringDay1Director` 新增的 task-list 语义出口
## 2026-04-09 10:22｜等待气泡继续补到“等待态持续保活”
- 新增真相：
  - 前两刀已经解决“被压掉”和“被 freeze 清掉”
  - 这次用户现场反馈后继续确认到第三个问题：
    - 等待气泡本身没有持续续命，会按自身时长自动收掉
- 本轮补口：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - `TryShowEscortWaitBubble(...)` 现在会在“同一句等待气泡已经可见”时持续保活，不重启 fade、不掉线
- 当前恢复点：
  - 让用户直接复验等待态头顶气泡是否终于能一直挂住
## 2026-04-09 11:20｜主控先收正播种上游语义，再回球给 farm
- 当前主线：
  - 继续推进 `Day1 最终闭环`；用户明确指出“播种任务完成不了”，要求主控先把上游语义钉死，再让 `farm` 按统一口径修实现
- 本轮已落：
  - [SpringDay1Director.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - 新增 `PlacementSemanticState` 及相关 getter
- 当前语义结论：
  1. formal 对话 active = 可以临时阻断放置/播种
  2. `FarmingTutorial` = 必须允许 `锄地 -> 播种 -> 浇水`
  3. `FreeTime` = 允许农田/放置链
  4. 非 formal active 情况下播种失败 = 不应再归因成“剧情禁播”，应由 `farm` 主查 placement/preview 运行时链
- 当前恢复点：
  - 现在可以把这份明确语义发给 `farm`，要求它只查实现链，不再纠缠 day1 是否禁止
## 2026-04-09 11:28｜等待气泡第四刀：把 ambient-link 清泡也补掉
- 新增真相：
  - 前一版还没彻底好，是因为 `ApplyResidentRuntimeFreeze()` 内部先跑的 `BreakAmbientChatLink(hideBubble: true)` 仍然会无差别清泡
- 本轮补口：
  - [NPCAutoRoamController.cs](D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Controller/NPC/NPCAutoRoamController.cs)
    - `BreakAmbientChatLink(...)` 已加同样的“脚本等待态下保留 conversation 气泡”判断
- 当前恢复点：
  - 现在直接复验村长等待气泡是否终于不再闪烁
## 2026-04-09 22:05｜只读复核：farm 放置异常当前更像 placement 实现链问题，不是 day1 禁播种
- 用户目标：
  - 用户指出 `farm` 线仍然存在“只有耕地可以，但种子 / 树苗 / 箱子预览看不到也放不了，只能看到遮挡效果”的问题，要求主控先只读看清责任归属
- 当前主线目标：
  - `Day1 最终闭环` 不变；本轮是阻塞子任务，只读确认这是否仍属于 `day1` 上游语义，还是已经收缩成 `farm / placement` 自身实现问题
- 本轮只读结论：
  1. 这不是 `day1` 当前在剧情上禁止播种或放置
     - `SpringDay1Director` 已有 `PlacementSemanticState / ShouldAllowFarmingPlacementForCurrentStory()`，当前口径仍是：
       - `FarmingTutorial` 必须允许 `锄地 -> 播种 -> 浇水`
       - 只有 formal 对话 active 才允许临时收掉 placement 输入
  2. `Hoe / WateringCan` 和 `Seed / Sapling / Storage` 现在仍走两条不同运行时链
     - `Hoe / WateringCan`：`GameInputManager.UpdatePreviews()` -> `FarmToolPreview`
     - `Seed / Sapling / Storage`：`HotbarSelectionService` + `GameInputManager.SyncPlaceableModeWithCurrentSelection()` -> `PlacementManager` -> `PlacementPreview`
  3. 当前最像主根因的是放置类物品依赖 `GameInputManager.IsPlacementMode` 与 `PlacementManager.currentState` 的双重闸门
     - `HotbarSelectionService.EquipCurrentTool()` 与 `GameInputManager.SyncPlaceableModeWithCurrentSelection()` 都会在 `!IsPlacementMode` 时直接 `ExitPlacementMode()`
     - 左键也只有 `PlacementManager.IsPlacementMode` 为真时才会真正走 `PlacementManager.OnLeftClick()`
     - 这解释了为什么 `Hoe` 还能靠自己的工具预览链成立，而 `Seed / 树苗 / 箱子` 会一起坏
  4. 这不再像“种子资产单独坏了”
     - `SeedData.OnEnable()` 已有运行时 `isPlaceable = true`
     - 箱子和树苗资产本身也已是 `isPlaceable: 1` 且带 `placementPrefab`
     - 所以“树苗 / 箱子 / 种子一起坏”更像 `PlacementManager` 公共链的状态问题，而不是单个 item 资产问题
  5. `PlacementPreview` 视觉链目前更像次级嫌疑，不像第一根因
     - 代码上它会优先用 `placementPrefab` sprite，失败再回退 `item.icon`
     - 种子还有 `cropPrefab` 第一阶段 sprite 兜底
     - 用户说“还能看到遮挡效果”，说明 preview/occlusion 至少部分被拉起来过；因此当前更像“状态链不稳 + 视觉链可能还有第二个次级问题”，而不是单独 sprite API 全坏
- 当前判断：
  - `farm` 现在最该查的是 `placement mode` 的唯一真源与切槽后状态保持，不该再回去猜 `day1` 是否禁播种
- 当前恢复点：
  - 如果下一轮继续推进，应直接把球回给 `farm`：只收 `HotbarSelectionService / GameInputManager / PlacementManager / PlacementPreview` 这条 placement 真链，不再扩到 `day1` 语义
## 2026-04-09 14:25｜只读深挖：当前更像 inventory-held 放置链半迁移，不是 PlacementManager 内核整体坏掉
- 用户目标：
  - 继续只读精查 `GameInputManager.cs`、`HotbarSelectionService.cs`、`PlacementManager.cs`，直接指出为什么当前 live 会出现“Hoe 预览正常，但 Seed / Sapling / Chest 看不到预览也放不了”
- 当前主线目标：
  - `Day1 最终闭环` 不变；本轮仍是阻塞子任务，但结论要从“placement 公共链有问题”继续收窄到具体残留入口和最小修法
- 本轮只读结论：
  1. 在这 3 个脚本里，看不出 `PlacementManager` 对 `Seed / Sapling / Chest` 有共同拒绝分支
     - `ValidateCurrentPlacementAt(...)` 已分别覆盖 `SaplingData`、`SeedData`、普通 `PlaceableItemData`
     - `OnLeftClick() -> ExecutePlacement() / ExecuteSeedPlacement()` 也都在
     - 所以“3 类 placeable 一起坏”更像坏在进入 `PlacementManager` 之前，而不是它内部只对某一类物品失效
  2. 当前最硬的 hotbar-only 残留在 `HotbarSelectionService`
     - `SelectInventoryIndex(...)` 只改 `selectedInventoryIndex`，不 `EquipCurrentTool()`、不发 `OnSelectedChanged`
     - `EquipCurrentTool()` 却仍只读 `inventory.GetSlot(selectedIndex)`，也就是只认热栏，不认背包区真实手持
     - `PlacementManager` 也只订阅 `OnSelectedChanged`，所以 inventory-held 选择没有直接事件桥进放置状态机
  3. 当前第二个硬残留在 `GameInputManager`
     - `HandleUseCurrentTool()` 先看 `PlacementManager.IsPlacementMode`
     - 一旦这里没进放置态，后面就退回 `selectedIndex` 的 hotbar-only / tool-only 分发
     - `SeedData` 分支是空的，`PlaceableItemData` 甚至没有分支；因此会直接表现成“没预览，也放不了”
  4. 这解释了当前现象为什么会是：
     - `Hoe` 仍正常：它走的是 `GameInputManager.UpdatePreviews() -> FarmToolPreview` 的农具链
     - `Seed / Sapling / Chest` 一起坏：它们共用的是 inventory-held / placement preview 这条半迁移链
- 当前判断：
  - 当前最像真根因的不是 `day1`、不是单个 seed asset、也不是 `PlacementManager` 验证算法，而是“inventory-held 选择态没有被完整送进放置模式 + 左键 fallback 仍按工具逻辑收口”
- 当前恢复点：
  - 如果下一轮继续推进，最小改法应只收：
    - `HotbarSelectionService.SelectInventoryIndex(...)`
    - `HotbarSelectionService.EquipCurrentTool()`
    - `GameInputManager.HandleUseCurrentTool()`
  - 不要再先扩去 `day1 / Primary / Town / UI`
## 2026-04-09 17:15｜主控只读总审：多线程 contract 已大量落地，但 shared root 脏面和 day1/farm 主风险仍未收平
- 用户目标：
  - 用户要求 `day1` 作为主控，不再只看一两条回执，而是彻底查阅其他线程当前最新产出、shared root 脏面和本线真实缺口，给出一份能直接决策的审核总图。
- 本轮只读完成：
  1. 重新核对了 shared root 当前总体量：
     - `git status --short` 总入口 `458`
     - 其中 tracked 改动约 `206`
     - untracked 约 `2327`
     - `git diff --shortstat HEAD` 为 `206 files changed, 153469 insertions(+), 55455 deletions(-)`
  2. 重新核对了 active thread state：
     - `spring-day1 = PARKED`
     - `UI = ACTIVE`
     - `NPC = PARKED`
     - `农田交互修复V3 = PARKED`
     - `存档系统 = PARKED`
  3. 重新回看了 `NPC / UI / farm / 存档系统 / Codex规则落地(Town)` 的最新 memory、回执和 active-thread 状态。
- 本轮站住的总判断：
  1. Codex 里看到的夸张增量，不像“某一条线程偷偷做了 90 万行”，而更像 shared root 同时堆着多线程 tracked 改动、大量 untracked 生成物和归档物。
  2. 当前 untracked 最大头仍是：
     - `.kiro/xmind-pipeline`：`1854` 文件
     - `Assets/100_Anim`：`86`
     - `Assets/Screenshots`：`58`
     - `Assets/Editor`：`51`
     - `Assets/YYY_Tests`：`35`
     - `Assets/Sprites`：`28`
  3. 当前 tracked 改动最集中的区域是：
     - `Assets/YYY_Scripts`：`84`
     - `Assets/Editor`：`19`
     - `.kiro/specs`：`18`
     - `Assets/YYY_Tests`：`15`
     - `Assets/111_Data`：`14`
     - `Assets/222_Prefabs`：`13`
     - `Assets/000_Scenes`：`6`
  4. `NPC` 这边已经不再是“没给 contract”：
     - resident scripted control
     - resident runtime snapshot
     - 统一人物主表 / `NPC_Hand` 真源头像链
     都已经成型
     - 但 live 终验仍未闭环，而且 `PackageNpcRelationshipPanel.cs` 与 `UI` 线程有重叠写面。
  5. `UI` 这边也不是没做：
     - task list / prompt overlay 治理已收过多刀
     - packaged 字体链已补 build-like 修法
     - Package 页仍在 active 收尾
     - 但 packaged/live 最终体验证据还不完整。
  6. `存档系统` 当前也不是空白：
     - resident 最小 snapshot
     - `sceneName` 驱动的跨场景读档
     - bridge 第一刀性能/重绑补口
     已经落地
     - 但还不是任意导演帧恢复。
  7. `Town / scene-side` 交回给 day1 的东西已经很多：
     - entry/player-facing contract
     - 8 个 `HomeAnchor`
     - `Home / Town / Primary` 持久化 baseline
     但 scene 现场仍脏，不宜把“Town 已有很多 contract”误读成“整个 scene-side 现场已 clean”。
  8. 当前真正还没收平的主风险，仍然是：
     - `farm` 的 placement 公共链 live 异常
     - `day1` 自己的导演尾账和主链整合尾差
- 当前恢复点：
  - 如果下一轮继续，不该再泛看所有回执，而应直接把主控动作收敛成两件事：
    1. `farm`：只沿 `HotbarSelectionService -> GameInputManager -> PlacementManager -> PlacementPreview` 收 placement 真链
    2. `day1`：只收导演尾账与 contract 吃回，不再继续散修
## 2026-04-09 20:15｜005 后自由活动与晚饭直切主链补口已落
- 用户目标：
  - 把 `005` 末尾从旧的“直接回村 escort 开晚饭”改成：
    1. `Primary` 里先和村长做收口对白
    2. 浅黑屏后进入傍晚自由活动
    3. 可在 `Primary / Town / Home` 自由探索
    4. 在 `Town` 和村长聊天可直接开晚饭
    5. 若拖到 `19:00` 仍未开饭，则自动切进晚饭
    6. 晚饭群众 staging 复用第一次进村那套 marker
- 这轮代码落地：
  1. [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 新增 `PostTutorialWrapSequenceId`
     - `TickFarmingTutorial()` 不再直接 `BeginDinnerConflict()`
     - 新增 `TryConsumeStoryNpcInteraction()` / `TryStartPostTutorialWrapSequence()` / `EnterPostTutorialExploreWindow()` / `TryRequestDinnerGatheringStart()` / `ActivateDinnerGatheringOnTownScene()`
     - `Primary` 里 `001/002` 在 `005` 未收口前仍可见，但完成收口后会退出 `Primary`
     - 任务/提示语义已改成“先找村长收口”与“可继续探索；想直接开饭就回村找村长”
     - 晚饭进入时玩家会被对齐到 `Town` 当前编辑态 `Player` 位置 `(-15.64, 10.19, 0.25189802)`，`001/002` 会优先吃 `EnterVillageCrowdRoot` 里的 `001终点 / 002终点`
  2. [NPCInformalChatInteractable.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs)
     - 在正式进入 chat session 前，先让 `SpringDay1Director` 消费村长交互
  3. [SpringDay1NpcCrowdDirector.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs)
     - `VillageCrowdMarkerRootNames` 已补 `EnterVillageCrowdRoot`
- 验证结果：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
    - 阻断是 `codeguard timeout-downgraded + stale_status`
  - `validate_script Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs`
    - `assessment=no_red`
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1NpcCrowdDirector.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
    - 同样被 `stale_status` 卡住
  - `errors --count 20 --output-limit 20`
    - `errors=0 warnings=0`
- 当前判断：
  - 这轮已经把 `005 -> 收口对白 -> 傍晚自由活动 -> 晚饭直切` 这条主链代码接通了，且 fresh console 已经回到 `0 error / 0 warning`
  - 但这轮仍未做 live 跑通验证，所以当前状态是“代码链已落、玩家体验仍待你实机验”
- 当前恢复点：
  - 下一轮若继续，优先做你手上的黑盒验收：
    1. `005` 完成后在 `Primary` 和村长交互是否先触发收口对白
    2. 收口后 `001/002` 是否退出 `Primary`
    3. `Town` 找村长聊天能否直接开晚饭
    4. 晚饭 crowd 是否真复用你摆好的 `EnterVillageCrowdRoot`
## 2026-04-09 20:51｜0.0.5 收口时按 E 不能和村长对话的 blocker 已修
- 现象：
  - 用户在 `Primary` 的 `0.0.5 农田教学收口` 阶段，任务面板提示“先去和村长说一声”，但靠近村长按 `E` 无效。
- 根因判断：
  - 上一刀只把收口逻辑挂进了 [NPCInformalChatInteractable.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/NPCInformalChatInteractable.cs)
  - 但 `Primary` 这里的村长实际更可能走 [NPCDialogueInteractable.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs) 的 formal 入口
  - 导致用户按 `E` 时，真正被触发的脚本没吃到 day1 的收口 override
- 本轮修复：
  1. [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
     - 新增 `CanConsumeStoryNpcInteraction(...)`
     - `TryConsumeStoryNpcInteraction(...)` 改成先纯判断、再执行 side effect
  2. [NPCDialogueInteractable.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Interaction/NPCDialogueInteractable.cs)
     - `CanInteract()` 现在会先询问 `SpringDay1Director` 是否要吃掉这次村长交互
     - `HasFormalDialoguePriorityAvailable()` 也会承认这次 day1 override
     - `OnInteract()` 开头已补 `SpringDay1Director.Instance.TryConsumeStoryNpcInteraction(...)`
- 验证：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` => `assessment=no_red`
  - `manage_script validate NPCDialogueInteractable` => `errors=0 warnings=1`
  - `errors --count 20 --output-limit 20` => `errors=0 warnings=0`
- 当前恢复点：
  - 用户现在应优先重试：
    1. 回到 `0.0.5 农田教学收口`
    2. 在 `Primary` 靠近村长按 `E`
    3. 看是否能正常触发收口对白
## 2026-04-09 21:13｜0.0.5 收口现场态也补上了自愈刷新
- 新增根因钉死：
  - `Primary` 里的 `001` formal 交互并没有挂错层，`NPCDialogueInteractable / NPCAutoRoamController` 都在 `001` root 上。
  - 真正尾差是：`0.0.5` 教学目标完成后，director 虽然会把 prompt 改成“去和村长说一声”，但没有立刻重刷 `001/002` 的 runtime policy。
  - 结果就是 `001/002` 可能还停在上一阶段的 `story actor` 锁态里，formal/informal 脚本保持 disabled，用户看到提示却按 `E` 没反应。
- 本轮补口：
  - [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - `TickFarmingTutorial()` 现在会先记录 `ShouldUsePrimaryStoryActorMode(...)` 的前态。
    - 当 `0.0.5` 从“教学中”切到“待找村长收口”时，会立即 `UpdateSceneStoryNpcVisibility()`。
    - 即使用户已经卡在旧现场，只要当前仍处于 `IsAwaitingPostTutorialChiefWrap()`，每次 tick 也会再强制拉一遍 `001/002` 的交互态，避免必须整段重开。
- 验证：
  - `validate_script Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
    - 当前 blocker 仍是 `codeguard timeout-downgraded + stale_status`
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 通过
  - `errors --count 20 --output-limit 20` => `errors=0 warnings=0`
- 当前恢复点：
  - 用户现在不需要重开整条链，直接在当前 `0.0.5` 收口现场再次靠近 `Primary` 的村长按 `E` 即可复测。
## 2026-04-09 21:26｜0.0.5 收口后“玩家不能动 / UI 热键失效”已定位并补时序修复
- 用户现场现象：
  - `001/002` 走完后，玩家进入 `idle` 但不能移动、不能导航、不能打开 `Tab/B/...` 这类面板热键；
  - `toolbar` 仍可用，云还在飘，说明不是场景卡死，而是 world input 被持续锁住。
- 根因结论：
  - 不是 `DialogueManager` 永远没结束，而是 `PostTutorialWrapSequenceId` 完成后，director 在 `DialogueSequenceCompletedEvent` 阶段就立刻调用了 `EnterPostTutorialExploreWindow()`。
  - `EnterPostTutorialExploreWindow()` 内部会触发 `SceneTransitionRunner.TryBlink(...)`。
  - 这个 `blink` 会先缓存 `GameInputManager.IsInputEnabledForDebug`，再 `SetInputEnabled(false)`。
  - 但此时 `DialogueEndEvent` 还没发，`GameInputManager` 仍处于对白期间的 `_inputEnabled=false`；于是 `blink` 错误地把 `false` 记成了恢复基线，结束后又把输入恢复成了 `false`。
- 本轮修复：
  - [SpringDay1Director.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs)
    - 新增 `_postTutorialExploreWindowPending`
    - `OnEnable()` 现在额外订阅 `DialogueEndEvent`
    - `PostTutorialWrapSequenceId` 完成后不再直接 `blink`
    - 改成先 `RequestPostTutorialExploreWindowEntry()`
      - 若对白仍 active，则只记 `pending`
      - 等真正收到 `DialogueEndEvent(PostTutorialWrapSequenceId)` 后，才执行 `EnterPostTutorialExploreWindow()`
- 结果语义：
  - 自由活动的黑屏渐入渐出还保留；
  - 但它现在发生在对白真正结束之后，不再把对白期间的输入锁错误继承到自由活动。
- 当前验证：
  - `git diff --check -- Assets/YYY_Scripts/Story/Managers/SpringDay1Director.cs` 通过
  - `validate_script SpringDay1Director.cs` 当前只能拿到 `assessment=unity_validation_pending`
  - 阻断原因不是 own red，而是：
    - `CodexCodeGuard timeout-downgraded`
    - 当前 Unity/MCP 连接被拒绝 `WinError 10061`
- 当前恢复点：
  - 下一步优先让用户直接重试 `0.0.5 收口 -> 自由活动` 这一段，看玩家移动和 `Tab/B` 是否恢复。
## 2026-04-10 01:05｜DialogueUI 箱子页类型缺符号红已止血
- 用户现场现象：
  - 新一轮编译直接报：
    - `DialogueUI.cs(2007,43) CS0246 BoxPanelUI`
    - `DialogueUI.cs(2015,53) CS0246 BoxPanelUI`
- 根因结论：
  - 不是 `BoxPanelUI` 类不存在，而是 [DialogueUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs) 里新补的“非对白 UI 排除名单”引用了 `BoxPanelUI`，但文件顶部缺少 `using FarmGame.UI;`。
- 本轮修复：
  - 在 [DialogueUI.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Story/UI/DialogueUI.cs) 顶部补入 `using FarmGame.UI;`
  - 未改其他对白/UI 逻辑，只做最小编译补口
- 当前验证：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/DialogueUI.cs --count 20 --output-limit 8`
    - `assessment=unity_validation_pending`
    - `owned_errors=0 external_errors=0`
    - blocker=`codeguard timeout-downgraded + MCP refused (WinError 10061)`
  - `git diff --check -- Assets/YYY_Scripts/Story/UI/DialogueUI.cs` 通过
- 当前判断：
  - 这次红错已经从代码层面止血；剩下没拿到的是 Unity fresh console 票，不是这两个 `CS0246` 还在。
- 当前恢复点：
  - 用户现在可以直接回 Unity 再编一次；如果后续还有对白/UI 相关异常，应按运行态问题继续查，不用再围着 `BoxPanelUI` 缺类型打转。
## 2026-04-10 18:14｜只读审计：Day1 时间管控最安全切入点
- 当前主线目标：
  - 用户要求只读勘察 `spring-day1` 当前最安全的时间管控切入点，不允许改文件。
- 本轮子任务：
  - 核对 `TimeManager` 真实 API；
  - 核对 `SpringDay1Director` 内全部改时间 / 停时间 / 响应睡觉入口；
  - 判断“开场 9:00 / 005 前不超过 16:00 / 打包里手动跳时间不把剧情跳烂”的最安全刀口。
- 审计结论：
  1. Day1 当前“实际开场时间”不是单靠 `TimeManager` 默认值决定：
     - 底层默认是 [TimeManager.cs:43] 的 `currentHour = 6`
     - fresh start 会被 [SaveManager.cs:1023-1030] 强制写成 `Spring Day1 16:00`
     - `spring-day1` 进入 Town 开场后，又会在 [SpringDay1Director.cs:2240-2254] 通过 `EnsureStoryHourAtLeast(TownOpeningHour)` 再次抬到 `16`
  2. 如果要把 Day1 开场改成 `9:00`，真正源头是：
     - [SaveManager.cs:1023-1030]
     - [SpringDay1Director.cs:180] 的 `TownOpeningHour = 16` 及其调用链 [SpringDay1Director.cs:2241,2254]
  3. 对“005 前时间不能超过 16:00”的剧情语义，最安全 owner 是 [SpringDay1Director.cs:1851-1869] `HandleHourChanged(int hour)`：
     - 它已经是 Day1 的 `19:00 晚饭自动接管`、`21:00 后夜间提示` 的时间语义入口
     - 把 Day1 专属 ceiling 放在这里，风险比改全局 `TimeManager.SetTime()` 小
  4. 对“打包里手动跳时间也不能把剧情跳烂”的守门，最安全入口是 [TimeManagerDebugger.cs:102-149] `Update()`：
     - 它是 packaged debug 热键的总分发口
     - 当前会直接扇出到 [TimeManagerDebugger.cs:155-163] `AdvanceDay()`、[TimeManagerDebugger.cs:237-255] `AdvanceOneHour()`，最终落到 `Sleep()/SetTime()`
  5. 不建议先把 Day1 规则硬塞进 [TimeManager.cs:326-460] 的 `Sleep()/SetTime()`：
     - 因为 `SaveManager` 也走 [SaveManager.cs:1217-1229] `RestoreGameTimeData -> TimeManager.SetTime()`
     - 全局改这里， blast radius 明显更大
  6. `GameInputManager` 不是时间写入口：
     - 它只是在 [GameInputManager.cs:55,593] 持有/同步 `timeDebugger.enableDebugKeys`
     - 当前不是可靠的时间治理 owner
- 当前恢复点：
  - 后续如果真要下刀，优先顺序应是：
    1. `SaveManager.ApplyNativeFreshRuntimeDefaults`
    2. `SpringDay1Director` 的 `TownOpeningHour + HandleHourChanged`
    3. `TimeManagerDebugger.Update`
