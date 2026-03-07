# 报告：Primary.unity & slot1.json（回滚+成因剖析+处置建议）

- 生成日期：2026-03-06
- 范围：`Assets/000_Scenes/Primary.unity`、`Assets/Saves/slot1.json`
- 结论：两文件已从 working tree 回滚（当前 `git status --short` 不再包含它们）。

---

## 1) Primary.unity（你不懂它也正常）

### 1.1 这类 diff 通常意味着什么
- `.unity` 是 Unity 场景的序列化文本（YAML）。任何“保存场景”的行为都会改写它。
- 大 diff 往往不是“你真的改了逻辑”，而是：
  - Inspector/Hierarchy 的某些属性被改动（哪怕无意）
  - Prefab Instance 产生/清理 override
  - Editor 脚本在保存时自动修补（如持久化 GUID 修复器）

### 1.2 本次 diff 的核心信号（取证摘要）
- 变化集中在 `m_Layer`：大量对象从 `0` 改为 `20/21/22`。
- 还出现 `PrefabInstance` 的 `propertyPath: m_Layer` override。
- 这非常像：
  - 你（或脚本）批量调整了某些对象层级/Tilemap 的 Layer
  - 或者误触导致 Layer 被重置/同步

### 1.3 风险评估
- 风险不是“文件很大”，而是“Layer 改动会改变碰撞/射线/交互筛选/渲染规则”。
- 只要不是明确要改 Layer，这种变更属于**高概率污染**，不应夹带提交。

### 1.4 处置建议
- 默认策略：**不提交 Primary.unity 的大 diff**，除非你明确要做“场景结构/Layer/Prefab override”改动。
- 需要真正改场景时：
  - 在 Unity 里明确记录你改了哪些对象/Layer
  - 改完立刻自查：交互、碰撞层过滤、Tilemap、相机遮挡
  - 再提交（否则就回滚）

---

## 2) slot1.json（存档系统产物）

### 2.1 为什么它会频繁变化
- 这是运行时存档产物：时间、玩家坐标、世界对象状态每次保存都会不同。
- 本次 diff 的取证摘要：
  - `createdTime/lastSaveTime` 变化
  - `gameTime.day/hour/minute` 变化（例如 day 20 -> 3）
  - `player.positionX/Y` 变化
  - `worldObjects` 列表大量 GUID/坐标/sortingOrder/`genericData` 变化

### 2.2 上游生成链路（核心脚本）
- `Assets/YYY_Scripts/Data/Core/SaveManager.cs`
  - `SaveGame(slotName)`：组装 `GameSaveData` → `JsonUtility.ToJson` → `File.WriteAllText`
  - `LoadGame(slotName)`：反向修剪 `PruneStaleRecords()` → 反序列化 → Restore → `RestoreAllFromSaveData`
- `Assets/YYY_Scripts/Data/Core/SaveDataDTOs.cs`
  - 定义 JSON 字段结构：`GameSaveData`、`WorldObjectSaveData` 等
  - `WorldObjectSaveData` 里包含 layer、position、sortingLayerName、sortingOrder、genericData
- `Assets/YYY_Scripts/Data/Core/PersistentObjectRegistry.cs`
  - `CollectAllSaveData()` 收集所有可存档对象
  - `RestoreAllFromSaveData()`：匹配 GUID、反向修剪、找不到就 `DynamicObjectFactory.TryReconstruct()`
- `Assets/YYY_Scripts/Data/Core/DynamicObjectFactory.cs`
  - 按 `objectType` 重建：`Drop/Tree/Stone/Chest/Crop`
- `Assets/YYY_Scripts/UI/Debug/SaveLoadDebugUI.cs`
  - 默认槽位 `slot1`，常见是快捷键/按钮触发保存，导致 `slot1.json` 经常被刷新

### 2.3 风险评估
- 把 `slot1.json` 当“源代码”提交，会引入：
  - 无意义 diff 噪声（每次运行都变）
  - 团队协作冲突（每个人本地状态不同）
  - 误导审计（看起来像你改了存档系统，其实只是跑了一次游戏）

### 2.4 处置建议（务实）
- 默认策略：**不提交 slot1.json**。
- 推荐落地动作（按优先级）：
  1) `.gitignore` 忽略 `Assets/Saves/*.json`（或整个 Saves 目录）
  2) 存档输出目录避免放在 `Assets/`（否则 Unity 可能当作资源/触发导入）
  3) 调试 UI 继续用，但把“生成测试存档”与“要提交的基准数据”严格隔离

---

## 3) 与“回滚后重新生成报告”的对应
- 回滚：已完成（这两文件已从工作区修改列表消失）。
- 报告：本文件即为“Primary.unity 解释 + slot1.json 机制剖析 + 处置建议”。
