# 2026-04-11｜给典狱长｜`portfolio-review` 外发版体系最小闭环统筹

## 当前已接受基线
- 当前用户已经明确拍板：
  - 存档线程 own 只先收 `FreshStartBaseline / 默认开局旧壳清扫`
  - 不再让存档线程继续包办更大的“外发版/构建链/发布治理”问题
- 当前用户真正要的不是“再临时手搓一个包”，而是：
  - 为 `Sunset` 建一条可重复复用的 `portfolio-review` 外发版路径
- 当前目标档位固定为：
  - `portfolio-review`
- 当前不做的档位：
  - `public-demo`

## 本轮唯一主刀
- 只收一刀：
  - 把 `Sunset` 当前“开发包 vs 外发包没有分离”的问题，收成一版可落地的 `portfolio-review` 外发版最小闭环

## 这轮你要做什么
- 你这轮不是来泛审整个项目，也不是来重做所有业务线程。
- 你只负责把下面这条外发版链路收清楚：
  1. 当前 build chain 真实现状审清
  2. `portfolio-review` 候选外发版的最小完成定义钉死
  3. 需要谁改什么、你自己改什么、哪些必须转给 owner，全部压成可执行清单
  4. 如果有你自己可以直接落的治理/配置/审计文件，就直接落，不要只停在建议稿

## 允许 scope
- 只允许覆盖这些范围：
  - `portfolio-review` 的构建语义
  - Player Settings / 构建后端 / Development Build / Symbols / Burst Debug / 运行时开发依赖 审计
  - 打包产物级别的“是否混入开发残留”审计
  - 外发版最小 README / 命名 / 输出目录规范
  - 需要分发给其他 owner 线程的最小 prompt / 清单 / 问卷

## 明确禁止漂移
- 不要扩成：
  - 整个项目所有系统的大而全发布治理
  - `public-demo`
  - 泛清理所有调试代码
  - 重新接手存档线程 own 的默认开局语义
  - Workbench / Town / NPC / UI / Day1 的业务功能返修
- 不要把“外发版体系”偷换成“只打一包看看”
- 不要把“还没验证的建议”说成“已经形成正式外发流程”

## 你必须先钉死的事实
- 当前用户贴出的 build 现场里，已经明确出现：
  - `Assembly-CSharp.dll`
  - `MCPForUnity.Runtime.dll`
  - `Sunset_BurstDebugInformation_DoNotShip`
- 这意味着当前产物仍更接近“开发现场直接打出来的运行包”，不是正式外发版

## 完成定义
- 这轮做完，至少要能明确交出下面 6 件东西：
  1. `portfolio-review` 的正式目标定义
  2. 当前 build chain 的 fresh 审计结论
  3. `必须立刻做` 与 `可以后做` 的分层清单
  4. 哪些项你自己能直接推进，哪些项必须转给 owner
  5. 一版可执行的 `portfolio-review` 候选外发流程
  6. 用户可读的人话结论：现在离“可放心外发的简历 demo 包”还差什么

## 当前我方已知的高价值关注点
- 你应优先核清：
  1. `Mono -> IL2CPP`
  2. `MCPForUnity.Runtime.dll` 是否可从外发版剔除
  3. `DoNotShip` 产物是否可通过构建配置或后处理去掉
  4. Development Build / debug symbols / Burst debug information 的真实开关与来源
  5. 运行时开发入口、测试入口、调试入口是否会混入外发版
  6. 是否需要一个 `README.txt` / 命名规范 / 输出目录规范

## 与存档线程的边界
- 存档线程本轮 own：
  - `FreshStartBaseline / 默认开局旧壳清扫`
- 你这轮不要再把它拉回你的主刀里。
- 但你最终如果做到打包后 smoke / 外发版复检，仍必须保留一组存档验收项：
  - `F9` 默认开局
  - 默认槽不自动写回
  - 普通槽新建 / 覆盖 / 读取 / 删除
  - 箱子作者预设内容在包里是否仍成立
  - save debug 旧面板是否确实不再出现

## 外部 blocker 的正确口径
- 如果你发现 `IL2CPP` / 剔除运行时开发 DLL / Build 后处理 / 某条发布配置，不是你自己这条线能直接拍板落地的：
  - 可以报 external blocker
  - 但必须精确到：
    - 谁是 owner
    - 哪个文件/设置面
    - 你需要它只做哪一刀
- 不允许只写“还有风险”“需要再看看”

## 固定回执格式
- `当前主线`
- `这轮实际做成了什么`
- `现在还没做成什么`
- `当前阶段`
- `下一步只做什么`
- `需要用户现在做什么`
- `为什么这轮这样判断`
- `外发版最小闭环清单`
- `必须分发给其他 owner 的项`
- `技术审计层`
  - `changed_paths`
  - `当前是否直接进入真实施工`
  - `是否触碰构建配置/发布配置`
  - `本轮 fresh build / settings / 产物证据`
  - `blocker_or_checkpoint`

## 这轮结束后，用户应能直接做的决策
- 用户读完你的结果后，必须能直接判断：
  1. 当前能不能开始做 `portfolio-review` 候选包
  2. 还差哪些必须先清
  3. 哪些工作要继续留在典狱长位
  4. 哪些工作该再发给 owner 线程

---

## thread-state 接线
- 这条 prompt 会让你从只读进入真实施工或治理落盘，请按当前 Sunset live 规则自跑：
  - 开工前先 `Begin-Slice`
  - 首次准备 sync 前先 `Ready-To-Sync`
  - 如果本轮停下不继续，先 `Park-Slice`
