# 01 - 场景 Traversal 配置与桥水

## 模块概述
- 承接 `Town / Primary` 场景侧 traversal 配置问题：
  - 农田边界越权阻挡
  - 桥 / 水 override
  - scene-side 显式配置与运行时自动补收冲突

## 当前稳定结论
- `Town` 农田挡路更像运行时 `Farmland_Border` 被 traversal 自动收集越权吃成阻挡
- `Primary` 桥/水并非静态没配，问题不应再被泛判成“桥配置缺失”
- 显式 override 配置优先级必须压过自动补收与 soft-pass

## 当前恢复点
- 新的 scene-side traversal 问题先归这条线
- 跨场景桥失效和 NPC roam spike 不再混写到这里
