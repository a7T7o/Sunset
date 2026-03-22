# 0.0.2初步落地 - 可执行任务清单

## 本 checkpoint 目标
- 建立 Day1 的“阶段推进主链”。
- 让当前对话系统从“能播一段”升级到“能在播完后推动 Day1 前进”。

## 执行原则
- 先补主控脊柱，再补对话侧挂接。
- 先打通最小一条真实链路，再考虑下个 checkpoint。
- 不返工已验收 UI，不顺手扩写疗伤、闪回、教学。

## Checkpoint Checklist

### A. 建立阶段骨架
- [ ] 1. 新建 `StoryPhase`
  - 位置建议：`Assets/YYY_Scripts/Story/...`
  - 最小先覆盖：
    - `CrashAndMeet`
    - `EnterVillage`
    - `HealingAndHP`
    - `WorkbenchFlashback`
    - `FarmingTutorial`
    - `DinnerConflict`
    - `ReturnAndReminder`
    - `FreeTime`
    - `DayEnd`
  - 验收：
    - 阶段名可被代码直接引用
    - 不再依赖魔法字符串表达 Day1 主线进度

- [ ] 2. 新建 `StoryManager`
  - 最小职责：
    - 持有 `CurrentPhase`
    - 持有 `IsLanguageDecoded`
    - 提供阶段切换入口
    - 提供最小读接口给其他系统查询
  - 本轮边界：
    - 不做完整存档/全局变量系统
    - 不做全部剧情段的具体逻辑
  - 验收：
    - 项目里存在正式主控入口
    - `languageDecoded` 不再只挂在 `DialogueManager`

### B. 建立推进事件
- [ ] 3. 扩展 `StoryEvents`
  - 新增：
    - `DialogueSequenceCompletedEvent`
    - `StoryPhaseChangedEvent`
  - 事件最少应带：
    - 当前序列标识
    - 推进后的阶段信息（如适用）
  - 验收：
    - 对话完成和阶段变化不再只能靠组件直接互相调用

### C. 给对话资源加推进配置
- [ ] 4. 扩展 `DialogueSequenceSO`
  - 新增最小字段：
    - 完成后是否解码语言
    - 完成后推进到哪个 `StoryPhase`
    - 完成后切到哪个 follow-up 对话
  - 本轮要求：
    - 字段命名必须一眼能看懂
    - 不做过度通用化设计
  - 验收：
    - 单个对话资源可直接声明“播完以后发生什么”

### D. 让运行时真正发出推进信号
- [ ] 5. 扩展 `DialogueManager`
  - 在序列自然完成时发出 `DialogueSequenceCompletedEvent`
  - 仍保持现有：
    - 打字机逻辑
    - UI 播放链
    - 暂停/恢复时间链
  - 本轮不要做：
    - UI 返工
    - 新输入系统改造
  - 验收：
    - 对话“结束”与“完成序列”在语义上可以被正式识别

- [ ] 6. 把完成结果并入 `StoryManager`
  - 当序列完成后，能按资源配置：
    - 切换 `IsLanguageDecoded`
    - 推进 `CurrentPhase`
  - 验收：
    - 首段剧情的完成结果会真正改变 Day1 状态，而不是只播完消失

### E. 让 NPC 根据剧情阶段分流
- [ ] 7. 扩展 `NPCDialogueInteractable`
  - 从“单一序列”改成“首段 / 后续分流”
  - 优先读取：
    - `StoryManager` 状态
    - 其次兼容本地最小兜底状态
  - 本轮目标：
    - 先只服务 Day1 首个 NPC 的前后对话切换
  - 验收：
    - 同一 NPC 第二次交互时，不再重复首段乱码对话

### F. 打通最小真实数据闭环
- [ ] 8. 配置首个最小剧情闭环
  - 目标资源：
    - 首段对话 `SpringDay1_FirstDialogue`
    - follow-up 对话资源
  - 要达到的效果：
    - 首段播完 -> 解码语言
    - 首段播完 -> 记录阶段推进
    - 再次交互 -> 进入 follow-up
  - 验收：
    - 这是一个真实可跑通的最小 Day1 主链，不是纸面设计

### G. 给下个 checkpoint 留承接点
- [ ] 9. 明确 `HealingAndHP` 的挂接方式
  - 至少明确：
    - 它挂在哪个 `StoryPhase` 之后
    - 是由谁触发进入
    - 进入前必须满足什么条件
  - 验收：
    - `0.0.3` 开始时不需要重新争论“疗伤段从哪里接”

## 建议实现顺序
- 第一步：`StoryPhase`
- 第二步：`StoryManager`
- 第三步：`StoryEvents`
- 第四步：`DialogueSequenceSO`
- 第五步：`DialogueManager`
- 第六步：`NPCDialogueInteractable`
- 第七步：首个真实数据闭环
- 第八步：给 `0.0.3` 留承接说明

## 本轮验收点
- [ ] `StoryManager` 已成为 Day1 状态的正式入口
- [ ] 首段对话完成后 `IsLanguageDecoded` 成功切换
- [ ] 同一 NPC 再次交互时进入 follow-up，而不是重播首段
- [ ] 当前 `StoryPhase` 可被正式读取
- [ ] `HealingAndHP` 已有明确挂接入口

## 明确不在本轮内
- 不扩写 `0.0.3`
- 不返工 UI
- 不碰 `Primary.unity`
- 不顺手推进工作台、耕种、砍树、晚餐、睡觉
