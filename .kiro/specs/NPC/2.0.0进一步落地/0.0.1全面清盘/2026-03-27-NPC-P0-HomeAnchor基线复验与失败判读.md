# 2026-03-27 NPC P0 HomeAnchor 基线复验与失败判读

## 1. 验收范围

- 本文只服务 `P0`：
  - `T-P0-01`
  - `T-P0-02`
  - `T-P0-03`
  - `T-P0-05`
- 这轮不验：
  - 路线
  - 停留点
  - 相遇节奏
  - 气泡
  - 关系成长
  - 导航核心
  - `Primary.unity` mixed cleanup

## 2. 当前线程已完成的自验

- 已确认 `Primary.unity` 里三只 NPC 的 scene 层基础仍在：
  - `001_HomeAnchor`
  - `002_HomeAnchor`
  - `003_HomeAnchor`
- 已确认三只 NPC 的 `homeAnchor` scene override 仍在：
  - `001 -> 001_HomeAnchor`
  - `002 -> 002_HomeAnchor`
  - `003 -> 003_HomeAnchor`
- 已确认这三个 anchor 与 `001 / 002 / 003` 都挂在 `NPCs` 根下，父级结构一致。
- 已继续只修 [NPCAutoRoamControllerEditor.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/NPCAutoRoamControllerEditor.cs)：
  - Play Mode 下 `Home Anchor` 优先显示 runtime live 引用
  - 新增运行中诊断信息：
    - `Runtime Home Anchor`
    - `Serialized Home Anchor`
    - `Detected Anchor Candidate`
    - `Parent`
    - `Auto-repair`
  - 自动查找现在会回显查找来源：
    - `parent-sibling:*`
    - `self-child:*`
    - `scene-search:*`
  - `Create Home Anchor` 现在与自动补口统一为“优先挂在 parent 下”，不再和 scene 既有 sibling 口径打架
- 静态自验已过：
  - `git diff --check -- 'Assets/Editor/NPCAutoRoamControllerEditor.cs'`

## 3. 当前线程还没法自己闭环的部分

- 当前 `unityMCP` 处于“基线层可达、会话层缺失”的混合状态：
  - `check-unity-mcp-baseline.ps1 = pass`
  - 当前资源可列出 `unityMCP`
  - 但真正读 scene / editor 时返回：
    - `no_unity_session`
- 所以这轮还不能由我自己给出：
  - `001 / 002 / 003` Inspector 实时截图
  - Play 中三只 NPC 的 live `Home Anchor` 最终值
  - Unity Console 里这条脚本的 live 运行态证据

## 4. 这次到底要你验什么

- 只验两个点：
  1. `001 / 002 / 003` 的 `Home Anchor` 字段现在是不是能从“空”变成“非空或带明确诊断”
  2. 如果仍不对，Inspector 里的新诊断是不是已经把断点压成一个清楚的小点

## 5. 验收前提

- Unity 保持运行，不需要关闭 Editor
- 打开 scene：
  - [Primary.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Primary.unity)
- 等待脚本编译完成
- 优先在当前你已经在看的现场里复验，不需要额外重做 scene 配置

## 6. 验收步骤

1. 在 Hierarchy 里展开 `NPCs`，依次选中 `001`、`002`、`003`。
2. 每次都看 `NPCAutoRoamController` Inspector 里的 `Home Anchor` 区域。
3. 记录三件事：
   - 顶部 `Home Anchor` 字段当前显示什么
   - 有没有出现新的诊断框
   - 诊断框里的 `Auto-repair` 最后一行写了什么
4. 如果你愿意补一轮最短运行态确认，就进入一次 Play，再重复上面 3 步。
5. 看完后回到 Edit Mode。

## 7. 预期结果

- 最理想的通过态：
  - `001 / 002 / 003` 的 `Home Anchor` 都直接显示为非空
  - 名字分别对应：
    - `001_HomeAnchor`
    - `002_HomeAnchor`
    - `003_HomeAnchor`
- 如果还没完全通过，也应该比之前更明确：
  - 不再只是“空”
  - 而是至少能看到一组完整诊断：
    - `Runtime Home Anchor`
    - `Serialized Home Anchor`
    - `Detected Anchor Candidate`
    - `Auto-repair`

## 8. 失败判读

- 情况 A：
  - `Detected Anchor Candidate: None (not-found)`
  - 判读：
    - 当前更像“查找链没有打到目标 anchor”
    - 不是导航 runtime 问题

- 情况 B：
  - `Detected Anchor Candidate` 有值
  - `Auto-repair: Called SetHomeAnchor(...), but HomeAnchor is still empty.`
  - 判读：
    - 当前更像“确实找到了 anchor，也调用了赋值，但 runtime 没吃进去”
    - 断点已经压到赋值链，而不是 scene 丢 anchor

- 情况 C：
  - `Auto-repair: Bound runtime Home Anchor to ...`
  - 但你肉眼看到顶部字段还是像旧版那样空白
  - 判读：
    - 当前更像“Inspector 刷新链 / 编译缓存没回正”
    - 不是 `Primary.unity` 自身缺 anchor

- 情况 D：
  - 你完全看不到上述新诊断字段
  - 仍然是旧版空白 Inspector
  - 判读：
    - 当前更像“这份 Editor 脚本还没完成重编译或 Inspector 还停在旧版本”

- 情况 E：
  - `Runtime Home Anchor` 有值
  - 三只 NPC 都能稳定显示对应 `*_HomeAnchor`
  - 判读：
    - `T-P0-01` 基本通过
    - 下一步就可以继续补 `T-P0-05` 的最小正式验证证据

## 9. 最小回执模板

- `001：通过 / 不通过 / 部分通过`
- `002：通过 / 不通过 / 部分通过`
- `003：通过 / 不通过 / 部分通过`
- `顶部 Home Anchor 字段实际显示：`
- `Auto-repair 实际显示：`
- `是否进入 Play 后复验：是 / 否`
- `如果失败，最接近上面的哪种情况：A / B / C / D / E`
- `补充说明 / 截图：`
