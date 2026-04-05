# 2026-04-05｜给 scene-build-5.0.0-001｜Town 当前 first blocker 之 Tilemap 工具编译红收口

请先完整读取 [D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-05_给典狱长_Town各anchor可承接等级表_升级条件与剩余blocker推进图_08.md]

这轮不要继续扩 `scene-build` 功能面，也不要回去做 Tile 产出测试。

你当前唯一主刀固定为：

- 把 [TilemapToColliderObjects.cs](/D:/Unity/Unity_learning/Sunset/Assets/Editor/TilemapToColliderObjects.cs) 当前 fresh console 里的 `CS0051` 编译红彻底收掉

当前治理位已把 `Town` 的 first blocker 改判为你这条线，原因是：

1. 当前 fresh `status` 里唯一站着的 compile error 就是：
   - `Assets\\Editor\\TilemapToColliderObjects.cs(177,24): error CS0051`
2. 旧的 `PlacementManager.cs` 已不再有 fresh 编译红证据
3. 这条 editor compile red 先挡住了 `Town` 后面所有稳定 live / validate

## 这轮只做什么

1. 只修这条 `CS0051`
2. 修完后至少补到：
   - 轻量 no-red 证据
   - fresh console 不再出现这条红
3. 不要顺手扩：
   - `Town`
   - `UI`
   - `Primary`
   - 新的 Tilemap 工具能力
   - 新的 scene-build 资产

## 你要交什么

按典狱长口径回复，只交这些：

1. 当前主线
2. 这轮实际做成了什么
3. 现在还没做成什么
4. 当前阶段
5. 下一步只做什么
6. 需要用户 / 典狱长现在做什么

再补技术审计层：

1. `changed_paths`
2. 这条 `CS0051` 的根因定性
3. 你实际怎么收的
4. fresh console / validate / diff-check 结果
5. `thread-state`

## 不要做的事

1. 不要再说 shared-root dirty 很多，所以先不修红
2. 不要把“工具已落地”当成“这条 blocker 已过”
3. 不要顺手改 `TilemapSelectionToColliderWorkflow.cs`，除非它是这条红的直接联动修复
