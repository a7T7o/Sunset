## 2026-04-09 14:25｜只读审计：Hoe 正常但 Seed / Sapling / Chest 无预览也无法放置
- 用户目标：
  - 只读审计 `GameInputManager.cs`、`HotbarSelectionService.cs`、`PlacementManager.cs`，直接指出为什么当前 live 会出现“Hoe 预览正常，但 Seed / Sapling / Chest 看不到预览也放不了”，并给出最小修法建议；不改业务源码
- 当前主线目标：
  - 锁定放置链回归的真实入口，不做实现，只把后续最小修点压到具体方法
- 本轮已完成：
  1. 用 `skills-governor` 做 Sunset 启动前置核查；由于 `sunset-startup-guard` 本轮未显式暴露，按项目 `AGENTS.md` 做了手工等价 startup preflight
  2. 只读精查：
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Controller\Input\GameInputManager.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Inventory\HotbarSelectionService.cs`
     - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Service\Placement\PlacementManager.cs`
  3. 把问题收窄到两处最可能根因：
     - `HotbarSelectionService.SelectInventoryIndex(...)` 只改 `selectedInventoryIndex`，不触发 `EquipCurrentTool()` / `OnSelectedChanged`
     - `HotbarSelectionService.EquipCurrentTool()` 与 `GameInputManager.HandleUseCurrentTool()` 仍大量依赖 `selectedIndex`，导致 inventory-held 的 placeable 没有完整进到 `PlacementManager`
  4. 额外确认：
     - `PlacementManager` 本体已有 `SeedData`、`SaplingData`、普通 `PlaceableItemData` 的预览验证和左键执行路径，因此不像共同主根因
- 关键决策：
  - 当前最值得修的不是 `PlacementManager` 内部 validator，也不是单个 item asset，而是“resolved held item 的唯一真源”
  - 最小改法应只收 `HotbarSelectionService` 的选择态桥接和 `GameInputManager` 的左键 fallback，不要扩回 `day1 / Primary / UI`
- 验证结果：
  - 本轮为纯静态代码审计，未运行 Unity / MCP / PlayMode
  - 结论状态：静态推断成立，live 仍待后续修复后复验
- 遗留问题 / 下一步：
  1. 若继续真实施工，先把 `HotbarSelectionService` 的 inventory-held 选择变成会驱动 `EquipCurrentTool` 与放置状态同步的真入口
  2. 再把 `GameInputManager.HandleUseCurrentTool()` / `TryEnqueueFromCurrentInput()` 从 hotbar-only/tool-only fallback 改为使用同一套 resolved held item
  3. 修后最小复验只看：
     - `Hoe -> Seed -> Hoe`
     - `Hoe -> Sapling`
     - `Hoe -> Chest`
     是否都能出现 preview 且左键可执行

## 2026-04-09 19:40｜只读盘点：Unity Editor 工具与菜单体系
- 用户目标：
  - 只读彻查 Sunset 项目中的 Unity Editor 工具与菜单体系，统计 Editor 工具脚本、`[MenuItem]` 脚本、真实可点击命令数，并按菜单根与功能分类整理代表脚本、问题价值和简历素材；不改业务文件
- 当前主线目标：
  - 输出一份可直接决策、可直接对外表述的 Editor tooling inventory 摘要
- 本轮已完成：
  1. 用 `skills-governor` 做前置核查；由于 `sunset-startup-guard` 当前会话未显式暴露，按项目 `AGENTS.md` 做了手工等价 startup preflight；本轮只读，不跑 `Begin-Slice`
  2. 完整扫描 `Assets/Editor/**` 与 `Assets/**/Editor/**` 下全部 `.cs`：共 `138` 个 Editor-tree 脚本，其中 `Assets/Editor/**/*` 为 `95` 个，其他 `*/Editor/**/*` 为 `43` 个；`Assets/YYY_Tests/Editor` 里另有 `38` 个 Editor tests，因此非测试 editor/tool/helper 脚本为 `100` 个
  3. 解析全部 `[MenuItem]`：`237` 条声明、`66` 个带菜单脚本、`69` 条 `validate=true` 校验项、`168` 条真实可点击命令
  4. 按菜单根收敛：`Tools` 共 `112` 条（其中 `Tools/Sunset` 为 `62` 条、其余 `Tools/*` 为 `50` 条）、`Sunset` 为 `53` 条、`FarmGame` 为 `3` 条
  5. 按“脚本主用途单归类”把 `66` 个菜单脚本 / `168` 条命令收成 10 个功能组，并提炼每组代表脚本、解决的问题和可用于简历的事实表述
- 关键决策：
  - 命令统计以“唯一可点击菜单路径”为准，不把 `validate=true` 的 enable-check / checked-state 重复项算成独立功能
  - 功能分类采用“按脚本主用途单归类”；混合脚本（如 `DayNightConfigCreator`、部分 `Story Validation` 菜单）不跨组重复计数
- 涉及文件或路径：
  - `D:\Unity\Unity_learning\Sunset\Assets\Editor\**`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\*\Editor\**`
  - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\**`
- 验证结果：
  - 本轮为纯静态盘点，使用脚本化解析 `MenuItem` 常量路径与 `validate` 标志完成去重
  - 结论状态：静态盘点成立，未运行 Unity / MCP / PlayMode
- 遗留问题 / 下一步：
  1. 若用户继续推进治理，可把这 `168` 条命令进一步收成“保留 / 合并 / 废弃 / 缺文档入口”清单
  2. 若用户只要对外材料，本轮摘要已经足够支撑简历或团队工具体系说明
