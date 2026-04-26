# README深挖的只读探索线程

## 2026-04-23｜Day1 可玩主线节拍只读深挖

- 当前主线目标：
  - 只读回答 `Sunset` 当前 Day1 可玩主线由哪些具体节拍组成、每拍接入哪些系统、完成什么教学/叙事/状态承接，以及哪些点最适合写进仓库 README 对外说明。
- 本轮子任务：
  - 重点精读以下 6 个文件并交叉验证：
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Data\StoryPhase.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1Director.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\Managers\SpringDay1NpcCrowdDirector.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1DirectorStagingTests.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\SpringDay1LateDayRuntimeTests.cs`
    - `D:\Unity\Unity_learning\Sunset\Assets\YYY_Tests\Editor\NpcCrowdManifestSceneDutyTests.cs`
- 本轮完成：
  1. 已把 Day1 主线压成对外可懂的 9 拍：
     - 矿洞醒来与首遇
     - 跟随进村与围观安置
     - 疗伤与 HP 显形
     - 工作台交代与闪回
     - 开垦 / 播种 / 浇水 / 收木 / 制作五步教学
     - 白天收口后的傍晚自由窗
     - 晚饭冲突
     - 归途提醒
     - 夜间自由活动到第一夜收束
  2. 已确认这条主线串起的核心系统：
     - `Town -> Primary -> Home` 场景切换
     - 正式对话与 Prompt 卡片
     - HP / Energy / 低精力减速
     - 工作台交互与制作 UI
     - 农田放置、播种、浇水、资源采集
     - 床交互、夜间提示与强制睡觉
     - 7 名村民 crowd 的剧情分层调度
  3. 已确认最关键的教学与叙事承接：
     - 疗伤段负责把“先救人、后追问”和 HP 系统一起落地
     - 工作台段负责把“活下去的手艺”重新接回玩家手里
     - 农田段不是抽象教学，而是用一次完整劳动证明自己有留下来的价值
     - 晚饭段第一次把村里的敌意正面摊开
     - 夜晚段把“你暂时被留下，但还不算自己人”的气氛钉住
  4. 已确认 README 最值得写的时间门槛：
     - `18:00` 晚饭接管
     - `19:00` 归途提醒
     - `19:30` 自由时段开放且床可直接使用
     - `22:00 / 24:00 / 25:00` 夜间压力逐级加重
     - `26:00` 强制睡觉进入 `DayEnd`
- 关键决策：
  1. 对外不要写成“做了很多系统”，而要写成“一天的完整生存与融入流程”。
  2. crowd 最适合翻译成：
     - `村里的人会按剧情重新围观、退场、旁听或留下夜间目击感`
     - 不要直接暴露 `Priority / Support / Trace / BackstagePressure`。
  3. README 首屏最强一句话应围绕：
     - `被救起、被观察、被治疗、被要求干活证明自己、再在夜里决定是否立刻结束这一天`
- 验证结果：
  - 本轮为只读分析，未跑 `Begin-Slice`，未改任何业务文件。
- 当前恢复点：
  1. 若下一轮继续本线，可直接产出：
     - 一版 `README 一段话卖点`
     - 一版 `README 节拍表`
  2. 若用户要继续深挖，可扩成：
     - `Day1 节拍 -> 系统 -> 玩家感知` 三列素材表
