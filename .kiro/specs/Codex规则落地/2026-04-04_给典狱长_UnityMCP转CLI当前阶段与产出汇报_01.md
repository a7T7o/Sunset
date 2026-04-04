# 给典狱长：UnityMCP转CLI 当前阶段与产出汇报

## 1. 我当前在完成的事情

我这条线当前不是继续扩搜，也不是做大而全 CLI 产品，而是在把下面这件事先落成 Sunset 内部可用工具：

- 用一套轻量 CLI，把“每次改完代码都要快速确认当前有没有 fresh compile red”这条高频链先做实。

当前主线已经明确压成：

- `compile-first`
- `no-red first`
- `Python first`
- `尽量少碰 PowerShell`

也就是说，这条线当前服务的不是低频 live 验收，而是日常开发里最常发生的：

- 改完代码
- 请求 Unity fresh compile
- 读 Console
- 判断当前红错是不是我 own 的
- 判断是不是 external blocker

## 2. 我已经解决的需求

这条线当前已经解决了 4 个之前反复拖慢开发的需求。

### 2.1 把“查红”从口头动作变成一条可重复命令

现在已经能直接跑：

```text
python D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py errors --count 200
python D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py compile Assets/你的脚本.cs --count 20
python D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py no-red Assets/你的脚本.cs --count 20
```

这不再依赖人肉点 Unity、手动翻 Console、再口头描述“好像还有红”。

### 2.2 把 own red 和 external red 拆开

当前 CLI 已经能把两层事实拆开：

- 目标脚本自己的代码闸门是否通过
- Unity 当前 fresh compile 后的 Console 红错是否仍存在
- 这些红错是目标 own，还是 external blocker

这一点已经实测成立。

例如这轮对：

- `Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs`

做显式检查时，工具返回的是：

- own 代码闸门通过
- own red = 0
- external red > 0
- 结论 = `external_red`

也就是说，它不会把外部爆红错误算成目标脚本自己的锅。

### 2.3 在 shared root 脏仓库里主动拦噪音

当前 Sunset 仓库 changed `.cs` 很多。

如果继续沿用“不给路径就全仓 compile 判断”的口径，这条 CLI 很容易把别的线程红错、旧尾账、共享根脏现场一起吞进来，最后工具本身就会变成噪音制造机。

所以这版已经补了一个非常关键的保护：

- 如果用户不给明确脚本路径，而当前 changed `.cs` 过多，CLI 会主动阻断，并要求显式传路径，或者明确说明要扫全部 changed 文件。

这条保护当前已经实测生效。

### 2.4 明确减少 PowerShell 参与

当前已不再继续深挖旧的 PowerShell 主实现。

新的主实现已经迁到：

- `D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py`

而：

- `D:/Unity/Unity_learning/Sunset/scripts/sunset-mcp.ps1`

现在只是极薄 wrapper，运行 Python 后立即退出。

这条决策的原因不是审美，而是工程现场已经明确暴露出：

- `Codex -> powershell.exe` 子进程滞留
- PowerShell 版原型曾在输出收口上卡死
- 用户已明确要求减少机器负载、减少 PowerShell

所以“Python first、PowerShell 只留薄壳”不是附加优化，而是当前机器现场下的硬价值判断。

## 3. 我最终要做成什么

这条线最终不是做一个“什么都能调”的炫技 CLI，而是做一条 Sunset 内部开发高频链：

### 3.1 目标完成形态

每次改完代码后，开发者能用一条轻量命令快速得到：

1. 本轮目标脚本自己的代码闸门是否过了
2. Unity fresh compile 后还有多少红错
3. 这些红错里哪些是 own，哪些是 external
4. 当前是不是可以诚实 claim：
   - `代码层已过`
   - `external_red`
   - `no_red`
   - `Unity 红错验证未闭环`

### 3.2 下一阶段不该只追功能面

我当前对下一刀的判断很明确：

- 不能只补 `play / stop / menu / route`
- 必须把资源护栏一起补上

也就是下一刀的正确顺序应是：

1. `validate_script`
2. 控制命令
3. 资源/超时/输出上限/失败回收

否则很容易把现在机器上已经暴露出的 shell 膨胀问题复制进新工具链。

## 4. 现在已经交出的产出

### 4.1 已落盘文件

- `D:/Unity/Unity_learning/Sunset/scripts/sunset_mcp.py`
- `D:/Unity/Unity_learning/Sunset/scripts/sunset-mcp.ps1`

### 4.2 当前已可用命令

- `baseline`
- `status`
- `doctor`
- `errors`
- `compile`
- `no-red`
- `recover-bridge`

### 4.3 当前已做过的关键验证

- `baseline` 已通过
- `errors` 已能快速返回 fresh Console 红错
- `compile` 已能串起：
  - `CodexCodeGuard`
  - `refresh_unity`
  - `wait_for_ready`
  - `read_console`
- `no-red` 已能输出：
  - own red / external red 分离结果
- `python -m py_compile scripts/sunset_mcp.py` 通过
- `git diff --check -- scripts/sunset_mcp.py scripts/sunset-mcp.ps1` 通过

### 4.4 当前 fresh 红错现场

这轮最新 quick check 的 fresh 结果是：

- 当前 Console 红错 = `14`
- 当前 warning = `0`
- 当前主要集中在：
  - `D:/Unity/Unity_learning/Sunset/Assets/YYY_Tests/Editor/SpringDay1OpeningRuntimeBridgeTests.cs`

这组结果是通过新 CLI 直接读 fresh Console 得到的，不是旧日志复述。

## 5. 当前阶段判断

我的判断是：

- 这条线已经不是空研究
- 也不是 PPT 原型
- 已经到了“值得继续做下一刀”的阶段

理由不是因为它已经做了很多命令，而是因为它已经把最值钱的高频链路做成了可用原型：

- `compile-first`
- own red / external red 分离
- changed `.cs` 过多时主动拦截
- Python first，少碰 PowerShell

如果典狱长当前要判这条线是否继续，我的建议是：

- 可以继续
- 优先级可以给高
- 但下一刀必须带资源护栏一起做，不能只追控制命令表面功能

## 6. 对典狱长最重要的一句话

这条线当前真正服务的，不是“做一个更帅的 CLI”，而是：

- 先把 Sunset 的爆红规范真正落地成一条轻量、高频、低负载的开发动作：
- 每次修改代码后，都能快速确认当前有没有 fresh compile red，以及这些红错是不是自己 own 的。
