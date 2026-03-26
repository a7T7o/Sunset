# 009-hover 遮挡链闭环与 live 收口

这轮我先把你的最新回执定性清楚：

1. 我接受你已经把 `008` 跑到了真正的第一轮 Unity / MCP live。
2. 我接受你现在不是“脚本级闭环”，而是“4 组 live 已经真实跑过”。
3. 我也接受其中 3 组已经过线：
   - `hoe-runtime-chain`
   - `watering-runtime-chain`
   - `high-tree-feedback-chain`
4. 但我不接受你把这轮包装成“已经差不多收口”。

因为现在唯一剩余点已经缩得非常精确：

> [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs)  
> 到 `OcclusionManager.SetPreviewBounds(...)` 的 hover 链在 live 中仍然产出 `previewBounds=null`，  
> 导致中心格没有驱动透明。

所以你下一轮不准再回头重讲前 3 组通过了什么，也不准漂去别的系统。

---

## 一、当前已接受的基线

当前我接受的事实只有这些：

- `008` 第一轮 live 已跑实
- 4 组里已有 3 组通过
- 当前唯一未闭环项是：
  - `hover-occlusion-chain`
- 这条链当前的稳定失败形态是：
  - `sideStayedOpaque=True`
  - `centerBecameTransparent=False`
  - `centerRecovered=True`
  - `centerBoundsIntersected=False`
  - `centerTrackedByManager=False`
  - `previewBounds="null"`

这说明当前问题已经不是“hover 遮挡大方向不对”，而是：

> preview hover bounds 没有被稳定提交到遮挡管理链，  
> 所以中心格根本没进入应有的透明判定。

---

## 二、这轮唯一主刀

### 这轮唯一主刀固定为：

> 只锁 `hover-occlusion-chain`，把 `previewBounds=null` 这一处精确剩余点闭环。

这轮不准回头补：

- 锄头链
- 水壶链
- 高等级树链

除非你在修 hover 的过程中明确打坏了它们。

---

## 三、这轮允许做什么

### 允许：

1. 只围绕以下文件做最小补口：
   - [FarmToolPreview.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmToolPreview.cs)
   - 与 `OcclusionManager.SetPreviewBounds(...)` 直连的最小调用链
   - 如确有必要，再碰：
     - [FarmRuntimeLiveValidationRunner.cs](/D:/Unity/Unity_learning/Sunset/Assets/YYY_Scripts/Farm/FarmRuntimeLiveValidationRunner.cs)
2. 补最小 diagnostics，确认：
   - preview bounds 在什么时候是 `null`
   - 是谁没算出来
   - 是谁没传出去
   - 是谁没登记到 manager
3. 只重跑：
   - `hover-occlusion-chain`
4. 如果 hover 修完后你怀疑影响到了别的链，最多补跑一条最相关回归样本，不准重新 full-run 4 组

---

## 四、这轮明确禁止

### 不允许：

1. 漂去别的农田功能
2. 再顺手改工具损坏、水壶、高级树逻辑
3. 不准把前 3 组已通过的链再当本轮主叙事
4. 不准为了取证长时间跑 Play 刷日志
5. 不准把“还有 warning”混成 blocker

---

## 五、这轮真正要交出的东西

这轮最少要交出的不是“hover 好像又更合理了一点”，而是下面两类之一：

### 结局 A：hover 遮挡链闭环通过

你要明确给出：

- `previewBounds` 不再是 `null`
- `centerTrackedByManager=True`
- `centerBoundsIntersected=True`
- `centerBecameTransparent=True`
- `centerRecovered=True`

### 结局 B：仍未通过，但责任点继续收窄

你要明确给出：

- `previewBounds` 现在在哪一层断掉
- 当前最小剩余点精确到文件 / 方法 / 条件
- 下一个最小补口动作是什么

如果只是“hover 还没完全好，但大概更接近了”，这轮不算完成。

---

## 六、验证纪律

1. 只跑 `hover-occlusion-chain`
2. 一旦拿到足够证据，立刻 `Stop`
3. 完成后必须退回 `Edit Mode`
4. 不准继续把日志刷成洪水

---

## 七、下一次回执固定格式

- 当前在改什么：
- 当前是第几轮 / 第几块：
- 是否仍只锁 `hover-occlusion-chain`：
- changed_paths：
- 这轮是否已重跑 hover live：
- `previewBounds` 最新结果：
- `centerTrackedByManager` 最新结果：
- `centerBoundsIntersected` 最新结果：
- `centerBecameTransparent` 最新结果：
- `centerRecovered` 最新结果：
- 如果仍未通过，当前最小剩余点是什么：
- code_self_check：
- pre_sync_validation：
- blocker_or_checkpoint：
- 一句话摘要：
