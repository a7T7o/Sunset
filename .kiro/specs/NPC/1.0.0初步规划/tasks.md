# 实现计划 - NPC 自动移动初步规划

## 当前任务状态

- [x] 1. 固化 V1 / V2 范围与验收口径
  - V1 已落地：单 NPC 随机移动 + 短停 / 长停 + 自言自语气泡
  - 轻量环境聊天已进入当前实现，但仍限定为“长停期间、附近空闲 NPC、1~2 句气泡”
  - 当前仍不包含完整日程系统、关系系统、多人围聊和跨楼层复杂导航

- [x] 2. 落地 NPC 自动移动状态机
  - 已实现 `ShortPause / Moving / LongPause / Inactive`
  - 已接入短停 `0.5s ~ 3s`、短停累计 `3 ~ 5` 次后长停
  - 已补卡住检测、路径重建和失败回退链路

- [x] 3. 落地随机活动范围与目标点选择策略
  - 已使用 home 点 / 出生点作为漫游锚点
  - 已实现活动半径、最小移动距离、随机采样与不可达重试
  - 已强制走 `NavGrid2D` 可达性校验，不绕开障碍

- [x] 4. 落地 NPC 导航适配层
  - 已复用 `NavGrid2D.TryFindPath(...)`
  - 已通过 `NPCMotionController.SetExternalVelocity(...)` 驱动现有运行时
  - 当前未额外拆出 `NPCNavigationAgent`，而是先在 `NPCAutoRoamController` 内闭环实现

- [x] 5. 落地自动移动与动画联动
  - 已由 `NPCMotionController` 驱动 `NPCAnimController`
  - 运动中统一 `Move`，停驻 / 聊天统一 `Idle`
  - 继续沿用 `Side + flipX`，没有新增额外动作状态

- [x] 6. 落地长停留气泡表现层
  - 已新增 `NPCBubblePresenter`
  - 已支持自言自语文本池、指定文本显示、自动隐藏
  - 已与 `NPCRoamProfile` 联动，气泡不接 `DialogueUI`

- [x] 7. 落地第二阶段轻量聊天规则
  - 已限制为长停阶段发起
  - 已限制伙伴必须处于停留态且未聊天
  - 已实现“找不到可聊天对象时回退为自言自语”

- [x] 8. 落地配置化数据入口
  - 已新增 `NPCRoamProfile`
  - 已在 `NPCAutoRoamController` Inspector 和自定义 Editor 暴露调试入口
  - 已让生成器默认挂载 `NPCBubblePresenter`、`NPCAutoRoamController` 与默认 Profile

- [ ] 9. 收口第一版手工验收清单
  - [x] 编译阻断已解除：`FarmToolPreview` 已补兼容重载，Unity 可正常编译
  - [x] Play 验证通过：`Primary` 场景内 `001 / 002 / 003` 已观察到移动、停留与长停气泡
  - [ ] 仍待补强：真实场景下成功抓到一组 NPC 配对聊天的正样本
  - [ ] 仍待用户侧或下一轮继续补充：人工视觉确认“不穿墙 / 不离开活动区域”
