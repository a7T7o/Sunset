请先严格继承当前 `spring-day1` 现场，不要回方案模式，不要回轻量分发，不要再沿旧的 `runtime resident` 过渡路线继续加码。

当前总判断已经被用户重新钉死，而且你必须接受：

1. 最终正确形态不是 `Primary` 里继续保留 `001/002/003` 再用运行时 `Town_Day1Residents` 去补一个假常驻层
2. 最终正确形态是：
   - `Town` 里原生存在常驻居民
   - `Primary` 只保留开场真正必须在 `Primary` 的角色
   - 你后面的导演 / 部署 / 剧情消费只绑定和消费这些现成 NPC
3. 用户已经明确锚定：
   - `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor` 这类位置由用户自己在场景里调整和配置
   - 你不允许再用代码偷偷改这些位置
   - 你也不允许再用 runtime 创建同类替身锚点来绕开

从这一条开始，你的身份不是单纯 day1 施工线，而是：

- `Day1 owner`
- `导演线 owner`
- `最终整合位`
- `原生 resident 迁移收口责任人`

允许按需使用 `subagent`，但必须遵守：

1. 只有真能并行推进时才开
2. 只能用于：
   - 只读核实
   - 独立 tests / validation
   - 不和你主刀撞写面的窄实现
3. 最终架构、主线判断、整合和提交由你自己掌控
4. 用完就关

---

## 第一部分：先把 Day1 主控判断彻底改过来

从现在起，下面这些判断全部视为新硬规则：

1. `runtime resident` 不再是目标形态，只是历史过渡实现
2. `SpringDay1NpcCrowdDirector` 后续要往：
   - 只消费现成 scene resident
   - 只读现成 anchor / slot / home anchor
   - 不再自己现生 resident
   这个方向收
3. 你要明确把 Day1 剩余未完项重新聚焦到：
   - `Primary` 旧 resident 剥离
   - `Town` 原生 resident 接回
   - director / deployment / cue 对现成 resident 的消费
   - one-shot 正式剧情不可重播

---

## 第二部分：你自己这轮最该继续吃的主刀

顺序固定：

1. 收 `SpringDay1NpcCrowdDirector` 的过渡 runtime 生人逻辑
2. 改成“优先绑定现成 resident / anchor / root，只在历史兼容期最小保底”
3. 把 `Primary + Town` 的消费链改成：
   - `Primary` 不再被当成 resident 常驻容器
   - `Town` 才是 resident 主承接场
4. 继续守住：
   - 已消费正式剧情不能重播
   - formal consumed 后只能回落 resident / informal / 非正式补句

你这轮不是去碰用户手摆的位置，而是去收：

1. 代码不该再生成什么
2. 代码该如何消费现成场景对象
3. 主链怎样从“假 resident”迁回“真 resident”

---

## 第三部分：你必须主动吃回三条协作线的新边界

### A. Town

Town 后面应该主接：

1. `Town.unity` 里的原生 resident 根、组、槽位、原生 NPC 存在性
2. `Primary` 非开场 resident 的 scene-side 剥离
3. 但不替你写 day1 主逻辑

### B. NPC

NPC 后面应该主接：

1. resident 常驻内容层
2. formal consumed 后回落到 resident/informal 的内容与 contract
3. bridge tests / validation
4. 不再继续围绕 runtime spawn 扩写

### C. UI

UI 后面应该主接：

1. formal one-shot 玩家面
2. resident fallback 玩家面
3. Workbench / DialogueUI / 提示壳
4. 不需要围绕 runtime 假 resident 继续出额外适配

你自己必须把这些结果重新接回 day1 主链，而不是继续默认沿旧方向兼容。

---

## 第四部分：这轮能直接推进的 Day1 需求

这轮你继续推进时，优先盯这几类：

1. `EnterVillageCrowdRoot / KidLook_01 / NightWitness_01`
   - 继续从 runtime 代理消费往原生 resident 消费收
2. `DinnerBackgroundRoot / DailyStand_01~03`
   - 继续往真实 Town resident anchor 消费收
3. `SpringDay1NpcCrowdDirector`
   - 只读用户场景配置的 anchor，不再改位置
4. `SpringDay1Director`
   - one-shot 规则继续钉死
5. live / summary / snapshot
   - 改成能看出当前到底是“现成 resident 被消费”，而不是“runtime 又生了一批人”

---

## 第五部分：明确禁止事项

1. 不要再把 `runtime resident` 包装成最终方案
2. 不要再用代码改 `001_HomeAnchor / 002_HomeAnchor / 003_HomeAnchor`
3. 不要再用代码补假的 home anchor 替身
4. 不要碰 `Town.unity`
5. 不要碰 `Primary.unity`
6. 不要碰 `GameInputManager.cs`
7. 不要回吞 UI own 壳体细节
8. 不要回吞 NPC own 会话/气泡底座
9. 不要停在“我知道方向变了”

---

## 第六部分：你这轮结束时必须明确交代

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户现在做什么
7. 你这轮最核心的判断是什么
8. 你为什么认为这个判断成立
9. 你这轮最薄弱、最可能看错的点是什么
10. 你给自己这轮结果的自评

---

## thread-state

如果这轮从只读进入真实施工，或继续一个已开的施工切片，必须自己执行：

1. 开工前：`Begin-Slice`
2. 准备 sync 前：`Ready-To-Sync`
3. 中途停下或本轮结束：`Park-Slice`
