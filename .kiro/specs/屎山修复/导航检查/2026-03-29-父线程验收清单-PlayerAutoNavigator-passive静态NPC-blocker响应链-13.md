# 2026-03-29-父线程验收清单-PlayerAutoNavigator-passive静态NPC-blocker响应链-13

本清单只服务父线程对 `导航检查V2` 下一轮的单刀验收。  
这一轮不再审“高保真矩阵有没有完成”，  
而是审：矩阵已经把第一责任点钉死之后，  
`PlayerAutoNavigator` 的 passive/static NPC blocker 响应链有没有真的开始工作。

---

## 一、当前固定基线

父线程下一轮先固定这 4 个基线：

1. 普通地面点导航 = 玩家实际占位中心语义
2. 跟随交互目标 = `ClosestPoint + stopRadius`
3. `SingleNpcNear raw` 当前基线坏相是稳定推土机：
   - `playerReached=False`
   - `npcPushDisplacement≈2.29`
   - `detourMoveFrames=0`
   - `hardStopFrames=0`
   - `actionChanges=1`
4. `NpcAvoidsPlayer / NpcNpcCrossing` 当前仍是绿护栏

如果下一轮回执试图把这 4 条重新讲成“其实不确定”，  
直接判漂移。

---

## 二、Scope Gate

### Gate P0：只准这一刀

允许写入：

1. `PlayerAutoNavigator.cs`
2. 导航检查相关文档 / memory

如果下一轮又碰了下面任一项，默认先判 scope 违规：

1. `NavigationStaticPointValidationRunner.cs`
2. `NavigationStaticPointValidationMenu.cs`
3. `NavigationLiveValidationRunner.cs`
4. `NavigationLiveValidationMenu.cs`
5. `NavigationLocalAvoidanceSolver.cs`
6. `NPCAutoRoamController.cs`
7. ground 契约触点

除非它能明确证明这些改动是不可回避且仍服务同一刀，  
否则这轮不接受。

---

## 三、代码链 Gate

### Gate P1：必须明确交代旧断点

父线程必须在回执里直接看到：

1. 这轮到底改了 `HandleSharedDynamicBlocker(...)`、`ShouldDeferPassiveNpcBlockerRepath(...)`、`ShouldBreakSinglePassiveNpcStopJitter(...)` 里的哪一段
2. 原先是哪一个 `if` / 哪段返回让 passive/static NPC blocker 继续留在 `PathMove`
3. 这轮具体靠什么条件，把它从旧链上掰开

如果只看到“我调了几个阈值 / 指标更好了”，  
但看不到断点解释，  
直接判不合格。

### Gate P2：必须出现真实新响应

父线程必须能从回执和 fresh 结果里看到：

1. detour 开始真实进入
2. 或 `BlockedInput / HardStop` 后出现了明确升级
3. 或 blocker 被明确升级到 rebuild / detour / 恢复入口

如果最终仍是纯 `PathMove` 主路径把 NPC 顶走，  
哪怕数字略好看，也不算这一刀有效。

---

## 四、结果 Gate

### Gate P3：`SingleNpcNear raw` 的推土机签名必须被打破

父线程最先看这 4 项：

1. `npcPushDisplacement`
2. `detourMoveFrames`
3. `actionChanges`
4. `playerReached`

下一轮要么：

1. `SingleNpcNear raw` 直接过线

要么至少满足：

1. 不再稳定复现 `npcPushDisplacement≈2.29`
2. 不再稳定复现 `detourMoveFrames=0`
3. 不再稳定复现 `actionChanges=1`
4. 能明确指向新的单一责任点

如果这 4 条还原样存在，  
就说明 runtime 第一刀还没落到位。

### Gate P4：moving / crowd 只看最小护栏，不看这轮总修复

父线程只确认：

1. `MovingNpc raw ×1` 没被这刀明显带坏
2. `Crowd raw ×1` 没被这刀明显带坏
3. 不接受把这两条拿回来当主叙事

### Gate P5：NPC 两条必须仍绿

下一轮至少要看到：

1. `NpcAvoidsPlayer ×1 pass`
2. `NpcNpcCrossing ×1 pass`

如果这两条掉红，  
说明这刀把共享避让护栏带坏，直接判回归。

---

## 五、回执质量 Gate

### Gate P6：不能再把诊断 checkpoint 写成阶段乐观结论

下一轮如果出现下面任何一种说法，父线程直接降级处理：

1. “已经接近收口”
2. “已进入最后残余抖动阶段”
3. “整体功能主链已经基本闭环”

除非它拿到的是：

1. `SingleNpcNear raw` 真实过线
2. moving / crowd / NPC 护栏都稳住

否则这些阶段判断都不成立。

### Gate P7：own dirty 不 clean 不能冒充收口

如果回执里的 `当前 own 路径是否 clean` 不是 `yes`，  
那它最多只能 claim：

1. 技术 checkpoint
2. 失败收缩 checkpoint

不能 claim：

1. 当前这一刀已彻底收口
2. 可以直接安全 sync

---

## 六、父线程下一轮审查顺序

固定按这个顺序看：

1. 先看 scope 有没有漂
2. 再看它到底改了哪一个 `if` / 哪一段链
3. 再看 `SingleNpcNear raw` 推土机签名有没有被打破
4. 再看 moving / crowd / NPC 护栏
5. 最后才看 `changed_paths / dirty / checkpoint`

---

## 七、当前阶段判断

这轮最准确的父线程判断固定为：

1. 高保真矩阵已经完成，旧 pass 已降级
2. ground 契约已切出去，不再是下一刀 runtime 主刀
3. 下一刀唯一主刀已经收缩到：
   - `PlayerAutoNavigator.cs`
   - passive/static NPC blocker 响应链
4. 只要这条链还没打穿，导航线就不能说“开始收尾”
