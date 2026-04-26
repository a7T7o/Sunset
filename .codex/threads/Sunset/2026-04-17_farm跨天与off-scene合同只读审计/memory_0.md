# 2026-04-17_farm跨天与off-scene合同只读审计

## 当前主线

- 只读复核 farm 线程关于“`Primary` 离场后农田/作物没有补跨天结算”的判断是否成立。
- 本轮子任务：限定在农田/作物跨天与 off-scene world-state contract，不改代码，不扩到箱子/背包/UI，也不碰 `ChestController` 或 `PersistentPlayerSceneBridge` 的非农田语义。
- 子任务服务于：给主线程一个可直接决策的审计结论，判断这一轮该不该顺手动 farm 代码。
- 修复后恢复点：若继续推进，实现应回到单独的 `farm off-scene catch-up contract` 刀次，而不是在本轮审计里直接施工。

## 会话记录

### 会话 1 - 2026-04-17

**用户需求**:
> 只读审计 Sunset 里 farm 线程刚给出的结论是否成立，范围只限农田/作物跨天与 off-scene world-state contract。不要改代码，不要扩到箱子/背包/UI，也不要碰 ChestController 或 PersistentPlayerSceneBridge 的非农田语义。

**已完成事项**:
1. 只读检查 `FarmTileManager.cs`、`FarmTileData.cs`、`CropController.cs`、`CropInstanceData.cs`、`TimeManager.cs`、`PersistentPlayerSceneBridge.cs`、`SaveDataDTOs.cs`、`SaveManager.cs`。
2. 复核 farm 线程“`Primary` 离场后农田/作物没有补跨天结算”这一判断，确认其根因方向成立。
3. 额外钉死最小正确合同应为“双层”：bridge/save contract 负责提供离场 elapsed-days，farm 脚本负责消费 delta 做补算。
4. 补充一个 farm 线程未明说但会影响安全落地的关键边界：`FarmTileManager.Save()` 不保存作物占位，而 bridge 恢复顺序是 `FarmTileManager -> Crop`，所以不能把空地到期补算抢跑到 crop 重新挂回之前。

**关键决策**:
- 结论层级是“静态代码证据成立”，不是 live 结果。
- 这一轮不建议顺手进 farm 代码；更安全的是先把结论收成下一刀的施工边界。

**涉及文件 / 路径**:
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\FarmTileData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\CropController.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Farm\Data\CropInstanceData.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\TimeManager.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Player\PersistentPlayerSceneBridge.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveDataDTOs.cs`
- `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Data\Core\SaveManager.cs`
- `D:\Unity\Unity_learning\Sunset\.kiro\specs\农田系统\memory.md`

**验证结果**:
- 只读静态审计完成。
- 未修改业务代码。
- 未运行 Unity live / MCP live。
- 未虚构任何运行态结果。

**遗留问题 / 下一步**:
- 如果后续继续，下一刀应单独定义为 `farm off-scene catch-up contract`。
- 该刀至少要同时回答三件事：
  1. off-scene snapshot 在离场时如何记录时间锚点；
  2. `FarmTileManager` / `CropController` 在回场时如何消费 delta；
  3. 恢复顺序如何避免 manager 先把“其实有 crop 的 tile”误判为空地。
