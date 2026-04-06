# Home 屋内 scene-side 适配回执

## 1. 当前主线

把 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 从“只有中文瓦片/家具的室内壳”推进到“day1 能开始消费的玩家住处 scene-side 语义层”，但不抢位置权、不回吞 day1 主逻辑。

## 2. 这轮实际做成了什么

1. 重新审了 `Home.unity` 当前现场：
   - 当前磁盘版已经是一份简化屋内场景；
   - 有床、枕头、地板、家具、相机、`PersistentManagers`；
   - 但没有 `HomeBed / HomeDoor / HomeEntryAnchor` 这类给剧情链吃的语义点。
2. 已直接修改 [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity)：
   - 在 `Home` 根下新增 `Home_Contracts`；
   - 在其下新增：
     - `HomeDoor`
     - `HomeEntryAnchor`
   - 在现有床根下新增：
     - `HomeBed`
3. 已给 `HomeBed` 补上最小 `BoxCollider2D (isTrigger=true)`：
   - 这样 `SpringDay1Director` 后续在屋内场景里能把它识别为可绑定的休息承载点；
   - 不需要我现在去硬塞 day1 组件或逻辑。
4. 这轮没有动这些东西：
   - 现有床、枕头、家具、地板 Tilemap
   - 相机
   - `PersistentManagers`
   - 任何 `day1/NPC/UI` 代码
5. 已拿到一层部分 live 证据：
   - Unity 里曾成功把 `Home` 切成 active scene 一次；
   - 当时返回：
     - `active_scene = Home`
     - `isDirty = false`
     - `rootCount = 3`
   - 说明当前 `Home.unity` 至少能被 Editor 重新打开，而不是 YAML 已坏。

## 3. 五段审计

### 原有配置

- [Home.unity](/D:/Unity/Unity_learning/Sunset/Assets/000_Scenes/Home.unity) 当前是一个小型屋内场景，根层以 `Home` Grid + 中文命名 Tilemap 为主。
- 现有场景里能读出的核心物体是：
  - `床`
  - `枕头`
  - `椅子`
  - `家具`
  - `地板`
- 但它缺少 day1 逻辑明确会查找的英文语义名：
  - `HomeBed`
  - `HomeDoor`
  - `HomeEntryAnchor`

### 问题原因

- `SpringDay1Director` 当前对住处/休息链的第一层识别，仍然明显依赖名字约定：
  - `Bed / PlayerBed / HomeBed`
  - `House 1_2 / HomeDoor / HouseDoor / Door`
- 当前 `Home.unity` 虽然有室内美术壳，但没有对应语义点；
- 结果就是：
  - 场景有床，不等于逻辑能稳定认出“这是床”；
  - 场景是玩家住处，不等于逻辑能稳定认出“这是屋内入口/返回点”。

### 建议修改

- 不改现有美术摆位，不替用户决定门口坐标；
- 只补最小 scene-side 语义层：
  - `HomeBed`
  - `HomeDoor`
  - `HomeEntryAnchor`
- 其中：
  - `HomeBed` 直接挂在现有床根下，并补触发 collider；
  - `HomeDoor / HomeEntryAnchor` 作为用户后续可手摆的入口合同点先落 hierarchy，不偷改位置。

### 修改后效果

- day1 以后开始真的吃屋内场景时，至少已经有一层明确语义锚点可接：
  - `HomeBed` = 住处/睡觉承接点
  - `HomeDoor` = 屋内入口/门位语义点
  - `HomeEntryAnchor` = 后续玩家或导演链可消费的入口落位点
- 这让 `Home` 从“纯美术壳”变成“开始可被剧情逻辑消费的 scene-side 住处场景”。

### 对原有功能的影响

- 不改 Tilemap 内容；
- 不改现有床和家具位置；
- 不改相机；
- 不改 `PersistentManagers`；
- 不新增 day1 业务逻辑；
- 当前真实新增影响只在：
  - hierarchy 多了一层 `Home_Contracts`
  - 床根下多了一个 `HomeBed` trigger 锚点

## 4. 当前还没做成什么

1. `HomeDoor` 现在只是 scene-side 合同点，还不是最终可交互门体。
2. `HomeEntryAnchor` 现在只是合同点，还没和任何切场/玩家落位逻辑正式绑定。
3. 这轮没有去硬做 `SceneTransitionTrigger2D`，因为当前最值钱的是先把语义层补齐，而不是在没有确认门位前强绑错误转场。

## 5. 当前阶段

`Home` 当前进入的是：

- `scene-side 语义适配已落地`
- 但还不是 `屋内外切场链 fully ready`

更准确地说，这轮把 `Home` 从“住处壳”推进到了“住处可消费合同层”。

## 6. 下一步只做什么

如果下一轮继续压 `Home`，最值钱的不是再改床，而是二选一：

1. 用户先手摆 `HomeDoor / HomeEntryAnchor` 到真实门位，然后我再补门口 contract/probe
2. 或者等 day1 真要吃屋内切场时，再按最新入口位补最小 `SceneTransitionTrigger2D` / player-facing contract

## 7. 需要用户现在做什么

如果你愿意进屋内 scene 手摆，当前最值钱的是：

1. 看一下 `Home_Contracts/HomeDoor`
2. 看一下 `Home_Contracts/HomeDoor/HomeEntryAnchor`
3. 决定它们是不是要放到你认定的真实门口

如果你现在不想手摆，也可以先不动；当前 `HomeBed` 这一层已经先站住了。

## 8. 这轮最核心的判断

`Home` 现在最该补的不是“更多家具”或“直接绑转场”，而是先把玩家住处的最小语义层补清。

## 9. 为什么我认为这个判断成立

因为当前屋内 scene 的真实缺口，不是“没有床”，而是“逻辑看不懂这张床和这个入口”。

只要 `HomeBed/HomeDoor/HomeEntryAnchor` 这层没有落进 scene：

- day1 继续往下吃 `Home` 时就只能靠旧 fallback 或 runtime 临时修补；
- 这会和你现在明确要求的“位置权归用户、scene-side 先落地”正面冲突。

## 10. 这轮最薄弱、最可能看错的点

当前最薄弱点是：

- `HomeDoor` 还没有根据真实门洞位置做最终摆放；
- 所以我这轮没有把它包装成“已经是最终可用门口”。

## 11. 自评

这轮我给自己 `8.5/10`。

好的地方是：

- 方向没有歪；
- 没有重做屋内；
- 真把 `Home` 往 day1 可消费层推进了。

最保守的地方是：

- 我刻意没有越过用户位置权，去假装把门口也“自动做完”。

## 12. 验证状态

- 文本层：
  - `git diff --check -- Assets/000_Scenes/Home.unity` = clean
- CLI fresh console：
  - `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 10 --output-limit 5`
  - 当次返回 `0 error / 0 warning`
- direct MCP partial live：
  - 曾成功把 `Home` 切为 active scene，一次性证明当前场景文件可被 Unity 打开；
  - 但后续 active scene 被别的 live 线程切回 `Primary/Town`，并且再次窄口补验时命中 `This cannot be used during play mode`
  - 所以这轮不能把 `Home` 包装成“完整 live 验收已过”

## 13. No-Red 证据卡 v2

- `cli_red_check_command`: `py -3 D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 10 --output-limit 5`
- `cli_red_check_scope`: `Home scene-side adaptation`
- `cli_red_check_assessment`: `unity_validation_pending`
- `unity_red_check`: `live-pending`
- `mcp_fallback`: `required`
- `mcp_fallback_reason`: `scene_live_flow_required`
- `current_owned_errors`: `none`
- `current_external_blockers`: `其他 active 线程反复切回 Primary/Town，并在再次补验时进入 PlayMode，导致 Home live 二次补验未完全闭环`
- `current_warnings`: `none`
