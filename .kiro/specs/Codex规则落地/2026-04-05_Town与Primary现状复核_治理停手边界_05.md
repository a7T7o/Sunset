# 2026-04-05_Town与Primary现状复核_治理停手边界_05

## A. 用户可读层

1. 当前主线  
当前治理主线不是继续自己下场改 `Town / Primary`，而是把这两处现场重新压实到“谁还能动、谁现在不能动”的边界上。

2. 这轮实际做成了什么  
已重新只读复核 `Town.unity` 与 `Primary.unity` 的当前磁盘现场。  
已确认 `Town` 这条线现在没有新的治理线程 own 必做点，剩下的仍是外线 blocker。  
已确认 `Primary` 的现场相较 2026-04-03 已经变化，但治理线程现在仍不能安全推进，因为 A 类锁已经在 `农田交互修复V3` 手里。

3. 现在还没做成什么  
没有推进任何新的 scene 写入。  
没有继续修 `Town`。  
没有继续补 `Primary` manager/debugger 链。  
这轮只做到了“把能不能继续碰”重新判清。

4. 当前阶段  
`Town`：继续维持“只剩外线 blocker”。  
`Primary`：继续维持“只读 / 外线 owner”。  
治理线程这轮不该自行接盘任一 scene。

5. 下一步只做什么  
`Town` 下一步只继续盯：
- `UI` 的 `DialogueUI / 字体链`
- `农田交互修复V3` 的 `PlacementManager.cs` 编译红

`Primary` 下一步只继续盯：
- 当前 owner 是否自己把 manager/debugger 链补全
- 或未来锁正式转交后，再重新判是否允许治理线程接手

6. 需要用户现在做什么  
当前不需要再让我继续碰 `Town.unity`。  
当前也不应让我继续碰 `Primary.unity`。  
如果后续要继续推进 `Primary`，应先明确 owner 变更或锁释放。

## B. 关键裁定

### 1. `Town` 为什么现在不该再自刀

1. `Town` 当前编辑态仍能确认：
   - `Main Camera + AudioListener` 在场
   - 场景自体并未重新跌回“坏 scene”定性
2. 当前真 blocker 仍是外线：
   - `UI` 的 `DialogueUI / 字体链`
   - `农田交互修复V3` 的 `PlacementManager.cs` 编译红
3. `Town.unity` 当前仍是 mixed dirty，且主相机静态上虽然还没有 `CinemachineBrain`，但 runtime 跟随链已经按用户 fresh 实测通过；在没有新 runtime 回归的前提下，治理线程不该为了“结构更漂亮”重新去碰 scene。

### 2. `Primary` 为什么现在仍不能推进

1. `Primary` 的 2026-04-03 老判断已部分过时：
   - 当前磁盘版里已经能直接看到 `SeasonManager`
   - 也能看到 `PersistentManagers`
2. 但当前磁盘版里仍没有补齐整条链：
   - 还看不到 `TimeManagerDebugger`
3. 更关键的是，A 类热文件锁当前已经不在“用户独占”口径，而是：
   - owner = `农田交互修复V3`
4. 所以治理线程现在的正确动作不是“看到现场变了就自己接着改”，而是：
   - 继续停在只读
   - 不吞 current owner 的 scene 现场
   - 只把边界和现状重新说清

## C. 不可碰清单

当前治理线程不应静默吞并以下路径：

1. `Assets/000_Scenes/Primary.unity`
2. `Assets/000_Scenes/Town.unity`
3. `Assets/YYY_Scripts/Service/Placement/PlacementManager.cs`
4. `Assets/YYY_Scripts/Story/UI/DialogueUI.cs`
5. `Assets/YYY_Scripts/Service/Camera/CameraDeadZoneSync.cs`
6. `Assets/YYY_Scripts/Controller/Input/GameInputManager.cs`
7. `Assets/YYY_Scripts/Service/Rendering/CloudShadowManager.cs`
8. `Assets/YYY_Scripts/Service/PersistentManagers.cs`
9. `Assets/YYY_Scripts/Service/TimeManager.cs`
10. `Assets/YYY_Scripts/TimeManagerDebugger.cs`

## D. 技术审计层

1. `Primary` 当前锁文件  
   - `D:/Unity/Unity_learning/Sunset/.kiro/locks/active/A__Assets__000_Scenes__Primary.unity.lock.json`
   - `owner_thread = 农田交互修复V3`
2. `Primary` 当前磁盘现场的直接证据  
   - 可搜到 `SeasonManager`
   - 可搜到 `PersistentManagers`
   - 仍搜不到 `TimeManagerDebugger`
3. `Town` 当前磁盘现场的直接证据  
   - 可搜到 `AudioListener`
   - 可搜到 `CloudShadowManager`
   - 当前静态文本里仍看不到 `CinemachineBrain`
4. 本轮是否改 scene  
   - 否
5. 本轮结论  
   - `Town` 不再治理自刀
   - `Primary` 继续只读停手
