# mcp-hot-zones

## 文件身份
- 本文件列出 Sunset 在 Unity / MCP 单实例下最容易产生并发漂移的热区。
- 它服务于：
  - 启动闸门
  - 线程前置核查
  - 冲突时的只读降级判断

## 热区 A：编辑器全局状态
- Play Mode / Stop
- 把 Editor 停留在 Play Mode 的遗留运行态
- Compile
- Domain Reload
- Package / Asset Refresh
- 任何会让对象句柄、组件实例、Inspector 状态瞬间重载的动作

## 热区 B：共享场景 / Prefab / Inspector
- `Assets/000_Scenes/Primary.unity`
- 共享 Prefab
- 共享 ScriptableObject
- Inspector 实时改值
- 任何需要从 Scene / Prefab 上即时回读序列化结果的 MCP 操作

## 热区 C：A 类热文件
- `Primary.unity`
- `GameInputManager.cs`
- 以及 `.kiro/locks/active` 中当前被定义为 A 类热文件的对象
- 当前补充口径：
  - `Primary.unity`：继续按单 scene writer 处理；`dirty + unlocked + ownerless` 代表 stale mixed scene，当前指定接盘顺序为 `NPC -> spring-day1`
  - `GameInputManager.cs`：继续视为共享热点，但当前允许按触点并发；只有撞到同一入口 / 方法 / 行为链时，才升级成真正单写者问题

## 热区 D：共享表现层资源
- 字体资源
- Sorting Layer
- Layer / Tag
- UI 样式、材质、公共工具脚本
- 对话框、气泡样式、字体配置与其他直接影响观感的视觉资源
- 虽然不一定属于某业务线核心，但会间接影响 NPC / UI / 生成器 / 验收表现

## 热区 E：文档与记忆写入
- `.kiro/specs/**/memory.md`
- `.codex/threads/**/memory_0.md`
- 多线程同时往 shared root 下的治理与线程记忆写入时，也属于轻度热区

## 命中热区时的建议动作
1. 先判断自己是只读核查还是写入线程。
2. 先查 shared root 占用与 A 类锁。
3. 若涉及 Unity / MCP 写入：
   - 默认按单写者处理
   - 不假设别人会自动排队
4. 若热区状态不清：
   - 先只读
   - 先回收证据
   - 再决定是否继续
5. 若自己为了取证进入过 Play Mode：
   - 完成当前步骤后立即 Stop
   - 确认已回到 Edit Mode
   - 再把 Unity 现场让给其他线程

## 一句话口径
- Git 分支没冲突，不代表 Unity / MCP 没冲突；凡是命中这些热区，都要先把 Editor 当共享单实例资源处理。
