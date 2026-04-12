# 02 - 跨场景 Persistent 重绑

## 模块概述
- 承接 `Town -> Primary`、`Primary -> Home -> Primary` 这类跨场景后 traversal 失效问题。

## 当前稳定结论
- 这条线的真因已改判为：
  - persistent player 与 scene player 的重绑时序
  - `TraversalBlockManager2D` 运行时绑定对象命中错误
- 最小修补已落在：
  - `PersistentPlayerSceneBridge`
  - `TraversalBlockManager2D.BindRuntimeSceneReferences(...)`

## 当前恢复点
- 后续只要是“直进没事，跨场景后坏”，优先查这条线
- scene-side 桥水和 NPC roam spike 不再回写这里
