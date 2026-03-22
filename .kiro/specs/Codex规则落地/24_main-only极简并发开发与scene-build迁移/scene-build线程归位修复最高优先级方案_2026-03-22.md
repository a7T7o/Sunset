# scene-build线程归位修复最高优先级方案

## 文档目的
- 这份文档用于处理一个非常具体的问题：
  - `scene-build` 线程在 Codex 左侧栏里短暂显示在独立 worktree 下，但点击后又跳回 `Sunset`
  - 或者线程显示为 `main`
  - 或者切换分支时报：
    - `fatal: 'codex/scene-build-5.0.0-001' is already used by worktree at 'D:/Unity/Unity_learning/Sunset_worktrees/scene-build-5.0.0-001'`
- 这份文档的目标不是讨论 Git 理论，也不是重新设计治理体系，而是提供一套后续可直接照搬的修复 SOP。

## 问题定性
- 这类问题首先定性为：
  - `Codex 应用层线程映射错误`
- 不先定性为：
  - Git worktree 损坏
  - branch 真丢了
  - Unity 项目根损坏
  - scene-build 需要物理迁移到新目录

## 先决事实
- scene-build 的唯一正式 Git 现场应始终是：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- scene-build 的正式分支应始终是：
  - `codex/scene-build-5.0.0-001`
- shared root 仍然是：
  - `D:\Unity\Unity_learning\Sunset`
- 用户要修的是：
  - Codex 里这条线程怎么归位
- 用户不要我做的是：
  - 再造一个新项目根
  - 再做一次“物理迁移方案想象”
  - 把问题重新解释成 Git worktree 本体故障

## 最高优先级标准流程
### 0. 先确认 Git 正式现场没坏
- 必查：
  - `git -C D:\Unity\Unity_learning\Sunset worktree list --porcelain`
- 必须看到：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 如果这里是对的，就不要先碰 Git 结构。

### 1. 定位 scene-build 线程 id
- 先查：
  - `C:\Users\aTo\.codex\session_index.jsonl`
- 目标：
  - 找到当前“场景搭建 / scene-build”对应的线程 id
- 本次实战 id 是：
  - `019cc7ba-fb87-7012-a7ef-0ccee21121c0`

### 2. 同时修 4 层，不要只修 1 层
- 后续必须把下面 4 层一起看作同一组修复对象：
  - `C:\Users\aTo\.codex\state_5.sqlite`
  - `C:\Users\aTo\.codex\session_index.jsonl`
  - `C:\Users\aTo\.codex\.codex-global-state.json`
  - 该线程对应的 `rollout-*.jsonl`
- 只修其中一层，通常会出现半残状态：
  - 左侧分组对了，但点进去跳回去
  - 线程标题对了，但 branch 还是 `main`
  - sqlite 对了，但 rollout 旧 cwd 会把它重新拉回 `Sunset`

### 3. sqlite 是主绑定层
- 必修字段：
  - `threads.id = 019cc7ba-fb87-7012-a7ef-0ccee21121c0`
- 必须改成：
  - `title = 场景搭建`
  - `cwd = D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
  - `git_branch = codex/scene-build-5.0.0-001`
  - `git_sha = 8e641e67149f413c74181f4c6753895c2cdfcf53`
- 关键经验：
  - `cwd` 优先写普通路径，不优先写 `\\?\` 前缀
  - 因为 Codex 左侧 workspace roots 往往用的是普通路径，前缀不一致可能导致 UI 分组归一失败

### 4. rollout 是回跳根因层
- 必查文件：
  - `C:\Users\aTo\.codex\sessions\2026\03\07\rollout-2026-03-07T17-57-28-019cc7ba-fb87-7012-a7ef-0ccee21121c0.jsonl`
- 处理原则：
  - 不要只修 1 条残留旧 cwd
  - 必须全量扫描整份 rollout 里的所有 `payload.cwd`
- 正确目标：
  - 该线程整份 rollout 中所有 `cwd` 都统一为：
    - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
- 为什么：
  - 这次实战证明，只改 sqlite 不够
  - rollout 里旧的 `Sunset` cwd 会让线程打开后重新跳回 shared root

### 5. session_index 是左侧名字和最新索引层
- 必做：
  - 给该线程追加一条最新索引
- 推荐内容：
  - `id = 019cc7ba-fb87-7012-a7ef-0ccee21121c0`
  - `thread_name = 场景搭建`
  - `updated_at = 当前 UTC 时间`
- 如果中间误写过脏索引，例如：
  - `????`
- 必须删掉，避免 UI 读到坏标题。

### 6. global-state 是左侧工作区分组层
- 必查字段：
  - `electron-saved-workspace-roots`
  - `active-workspace-roots`
  - `electron-workspace-root-labels`
- 必须保证：
  - `D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001` 在 roots 中可见
  - 最好还给它一个 label，例如：
    - `scene-build`
- 这层不是唯一根因，但缺它容易出现：
  - 左侧出现空文件夹
  - 线程不挂到预期分组下

### 7. 每次操作前先备份
- 至少备份：
  - `state_5.sqlite`
  - `session_index.jsonl`
  - `.codex-global-state.json`
  - 目标 `rollout-*.jsonl`
- 原因：
  - 这类文件一旦写坏，Codex 会直接闪烁、线程打不开、甚至报 `failed to parse thread ID`

### 8. 修改 rollout 时的唯一推荐方式
- 最高优先级方法：
  - 按行读取 JSONL
  - 对每一行做 `json.loads`
  - 只修改 `payload.cwd`
  - 再用 `json.dumps(..., ensure_ascii=False, separators=(',', ':'))` 写回
- 不推荐：
  - 直接字符串替换超长行
  - 人工改一行巨长 JSON
  - 混用 BOM / `utf-8-sig`
  - 先用不稳定替换，再人工补半截

### 9. 成功判定标准
- 必须同时满足：
  - `state_5.sqlite` 里的线程 `cwd` 是旧 worktree
  - 该线程 rollout 中所有 `cwd` 都是旧 worktree
  - `session_index` 最新一条是正确标题
  - `active-workspace-roots` 含 scene-build worktree
  - 重启 Codex 后点击线程，不再跳回 `Sunset`
  - 不再出现：
    - `fatal: 'codex/scene-build-5.0.0-001' is already used by worktree ...`

## 后续一律先用的修复顺序
1. 备份 4 个状态文件。
2. 查 `session_index.jsonl` 定位线程 id。
3. 核对 `git worktree list --porcelain`，确认 Git 正式现场没坏。
4. 修 `state_5.sqlite` 的 `title/cwd/git_branch/git_sha`。
5. 逐行 JSON 解析重写该线程 rollout，把全部 `payload.cwd` 统一到 worktree。
6. 给 `session_index.jsonl` 追加一条最新正确索引。
7. 清掉错误索引行，例如误写的 `????`。
8. 修 `.codex-global-state.json`，保证 scene-build root 可见。
9. 重启 Codex 实测。

## 反例黑名单
### 反例 1：先怀疑 Git worktree 坏了
- 错误原因：
  - 会把应用层映射问题误打成 Git 问题
- 实际后果：
  - 白白浪费时间
  - 甚至引出不该有的新路径、新迁移

### 反例 2：擅自创造新目标路径
- 错误例子：
  - `D:\Unity\Unity_learning\SceneBuild_Standalone\...`
  - `D:\Unity\Unity_learning\scene-build-5.0.0-001`
- 错误原因：
  - 用户要的是 Codex 归位，不是 Git 正式现场改址
- 实际后果：
  - 制造第二现场
  - 干扰判断
  - 让问题更乱

### 反例 3：只修 sqlite，不修 rollout
- 错误原因：
  - rollout 里的旧 `cwd` 会在打开线程时把现场拉回 shared root
- 实际后果：
  - 左侧看着像对了
  - 点击后还是跳回 `Sunset`

### 反例 4：只修 1 条 rollout 残留，不做全量统一
- 错误原因：
  - 这条线程整份历史 rollout 可能大部分都是旧 root
- 实际后果：
  - 改完一处，还是会回跳

### 反例 5：直接手改超长 JSONL 单行
- 错误原因：
  - 容易破坏转义、引号、编码
- 实际后果：
  - `Failed to resume task`
  - `failed to parse thread ID from rollout file`
  - 界面闪烁

### 反例 6：把 JSONL 写成带 BOM 的 UTF-8
- 错误原因：
  - Codex 读这类 JSONL 时可能直接炸在第 1 行
- 实际后果：
  - 会话无法恢复
  - 线程 id 解析失败

### 反例 7：在 PowerShell 管道里直接写中文标题
- 错误原因：
  - 这次实战中出现了 `场景搭建 -> ????`
- 实际后果：
  - session_index 和 sqlite 都可能被污染
- 经验：
  - 需要时优先用 Python + `ensure_ascii=False`
  - 或用 Unicode 转义字面量

## 我这次的犯错历史
### 第一类错误：认知错误
- 我一开始把“Codex 应用层归位”误做成了“Git worktree 物理迁移”。
- 我错误创造了新路径概念，偏离了用户真实需求。

### 第二类错误：执行顺序错误
- 我先改了部分层，没有把 sqlite / rollout / session_index / global-state 当成一组。
- 结果造成了：
  - 一半是对的
  - 一半还在把线程往 `Sunset` 拉

### 第三类错误：文件写入方式错误
- 我尝试过手工替换超长 JSONL 单行。
- 我还一度把 rollout 写成了 BOM 版本。
- 结果直接引发：
  - `failed to parse thread ID`
  - 对话闪烁

### 第四类错误：编码处理错误
- 我在 PowerShell / Python 混用过程中，把中文标题写成过 `????`。
- 这说明本地状态修复时必须把“编码与转义安全”当成一等公民，而不是附带问题。

## 本次最终实战结论
- 对 scene-build 这种“Git 正式 worktree 没坏，但 Codex UI 映射错了”的问题：
  - 第一优先级永远是修 Codex 本地状态层
  - 不是重做 Git 结构
- 本次被证明有效的主解法是：
  - `sqlite + rollout + session_index + global-state` 四层同时统一
- 后续任何人接手此类问题，都应先照搬本文流程。
- 只有在本文流程全部完成后仍失败，才允许进入第二优先级探索：
  - 是否还存在别的缓存层
  - 是否桌面端还有未公开的索引源
  - 是否需要专门针对某版本 Codex 再补兼容方案

## 第二优先级探索口径
- 允许探索，但只能在本文流程全部走完且失败后进行。
- 不允许跳过本文，直接进入自由发挥。

## 直接执行口令
- 后续同类问题，默认执行：
  1. 先确认 Git worktree 没坏
  2. 锁定线程 id
  3. 备份 4 个状态文件
  4. 统一修 `sqlite / rollout / session_index / global-state`
  5. 重启 Codex 验证
  6. 若仍失败，再查更深缓存层
