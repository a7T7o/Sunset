# 2026-04-03 UI线程｜接管 spring-day1 全部玩家面问题并行 prompt

请先完整读取：

- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\0.0.1 SpringUI\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\UI系统\memory.md`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\900_开篇\spring-day1-implementation\003-进一步搭建\memory.md`
- `D:\Unity\Unity_learning\Sunset\.codex\threads\Sunset\spring-day1\memory_0.md`

这轮不是让你点评，也不是让你只接一两个残点。

这轮当前统一裁定已经改写为：

1. `spring-day1` 这边后续不再继续主刀玩家面 `UI/UE`。
2. 当前 spring-day1 里所有“玩家真正看到、真正觉得不对”的 UI 问题，从现在起整体转交给你这条 `UI / SpringUI` 线。
3. 你不是来接一个局部补丁，而是来正式接走这条线当前剩余的全部玩家面问题与体验收尾。
4. 但有一个例外仍然留在 `spring-day1`：
   - `NPCBubblePresenter.cs` 里“NPC 旧正式气泡 / 打断短气泡”这条旧气泡回正与 live 终验，仍由 spring-day1 自己处理。

## 当前已接受基线

先不要把一切都当成从零开始。

当前已经站住、但只代表“可继承基线”，不代表“玩家体验已最终过线”的内容如下：

1. 正规 `DialogueUI` 不再是整块透明：
   - 当前 live 已出现连续两次
   - `CanvasAlpha=1.00`
   - `CanvasInteractable=True`
   - `CanvasBlocksRaycasts=True`
2. 工作台“制作中离开后的小浮层”已经重新跑通：
   - 最新 workbench probe 已到
   - `switchOk=True`
   - `floatingVisible=True`
   - `floatingLabel='1'`
   - `floatingFill=0.02`
3. 工作台当前 5 个配方已经统一成 5 秒：
   - `Axe_0`
   - `Hoe_0`
   - `Pickaxe_0`
   - `Sword_0`
   - `Storage_1400`
4. 这些都只是你接手时应继承的“当前现场”，不是让你把它们直接包装成“已经没问题了”。

## 这轮唯一主刀

只做一件事：

把 spring-day1 当前剩余的全部玩家面 `UI/UE` 问题完整接走、重新审视、继续做到用户能真正验的状态。

翻成人话就是：

- 只要问题属于“玩家看到的 UI 不对、难看、不顺、还像半成品、还没接回用户原本认可的旧正式面”，就归你。
- 不要再让 spring-day1 继续做这些玩家面问题。

## 你这轮 exact-own 范围

你这轮默认接管的不是单文件，而是 spring-day1 当前 temp scene 下全部玩家面结果层，至少包括：

1. 正规对话 UI
2. Prompt / 提示 / WorldHint / 交互提示
3. Workbench 大面板、离台浮层、按钮/进度/文本排版与玩家面表现
4. 所有 Day1 当前 temp scene 下玩家真正会看到的 overlay / hint / page UI
5. 与上述玩家面强绑定的 prefab、UI utility、prefab-first 壳体、字体引用、UI 测试与 GameView 证据

你可以接的典型文件类别包括但不限于：

- `Assets/YYY_Scripts/Story/UI/*`
- `Assets/222_Prefabs/UI/Spring-day1/*`
- `Assets/111_Data/UI/Fonts/Dialogue/*`
- 与这些 UI 真正强绑定的 Editor tests / capture workflow

## 明确禁止漂移

这轮你不要做下面这些：

1. 不要接 `NPCBubblePresenter.cs`
   - `NPC` 旧正式气泡与打断短气泡回正留给 spring-day1
2. 不要吞 `Primary.unity`
3. 不要吞 `GameInputManager.cs`
4. 不要吞 `PlayerNpcChatSessionService.cs`
5. 不要吞 `StoryManager / SpringDay1Director / NPC` 状态机本体
6. 不要擅自把 `town` 未来内容、NPC 永久站位或剧情扩展塞回当前 temp scene
7. 不要自创一套新的视觉语言
   - 用户已经明确嫌当前这批新做的 UI “还是大量有问题”
   - 你必须以“用户之前认可的旧 formal-face / 旧版本”作为真值继续做
   - 不是自由创新

## 这轮你必须先做的事

不要一上来就继续改。

先完成 3 步：

1. 重新做一次玩家面总审视
   - 站在“现在游戏里玩家实际看到什么”这个角度
   - 不要只看代码结构
2. 明确拆开：
   - 哪些是 spring-day1 已经做出的可继承结果
   - 哪些是你接手后仍然没过线的问题
   - 哪些只是结构成立，还不是体验成立
3. 再开始施工
   - 不要把“当前能跑”直接说成“已经好看/已经完成”

## 当前你应该重点接走的问题

至少按下面 4 组去审：

1. 正规对话 UI
   - 现在虽然已经不再整块透明，但用户仍然觉得 UI 还有大量问题
   - 你要继续负责对话框的正式面、字面观感、版式、层次、稳定性和玩家观感
2. Workbench
   - 当前逻辑链已基本恢复
   - 但玩家面是否顺眼、是否像正式界面、视觉是否还粗糙、布局是否仍有半成品感，这些都归你
3. Prompt / Hint / WorldHint / 交互提示
   - 当前所有玩家会看到的提示层，都应由你统一收口
4. 当前 temp scene 下所有还让用户觉得“UI 还是大量有问题”的玩家面残项
   - 不要等 spring-day1 再替你兜这些

## 完成定义

你这轮交付时必须强制拆开 3 层：

1. 结构 / checkpoint
2. targeted probe / 局部验证
3. 真实入口体验

如果你只站住前两层，就必须明确写：

- `结构成立，但体验未成立`

不要再把结构 checkpoint 包装成玩家体验过线。

## 这轮回执必须按这个格式

先给用户可读层：

1. 当前主线：
2. 这轮实际做成了什么：
3. 现在还没做成什么：
4. 当前阶段：
5. 下一步只做什么：
6. 需要我现在做什么（没有就写无）：

然后再补技术审计层：

- changed_paths：
- 验证状态：
- 是否触碰高危目标：
- blocker_or_checkpoint：
- 当前 own 路径是否 clean：

## 这轮 thread-state 接线要求

如果你这轮会继续真实施工，先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`

第一次准备 sync 前，必须先跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`

如果这轮先停、卡住、让位或不准备收口，改跑：

- `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：

1. 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
2. 如果还没跑，原因是什么
3. 当前是 `ACTIVE / READY / PARKED / BLOCKED` 哪一个

## 最后一句话

这轮不要再把 spring-day1 当前所有玩家面问题甩回来。

从现在起，spring-day1 的玩家面 UI/UE 收口由你接。
