# blockers

## B1. `web_search` 缺搜索 provider key

### 现象
- `main` 调用 `web_search` 返回失败
- 当前失败已收敛为缺 key，而不是模型、代理或 Gateway 配错
- 当前显式 provider 为 `brave`

### 影响
- 不能把“浏览器可用”误判为“搜索也可用”
- 现在可稳定依赖的是：
  - `browser`
  - `web_fetch`
  - 已知 URL 抓取

### 解除方式
- 提供一个真实可用的搜索 provider key
- 推荐优先级：
  1. `BRAVE_API_KEY`
  2. `PERPLEXITY_API_KEY`
  3. `GEMINI_API_KEY`
  4. `XAI_API_KEY`
  5. `KIMI_API_KEY` / `MOONSHOT_API_KEY`

---

## B2. Chrome / Edge 还差浏览器 UI 最后一跳

### 现象
- relay 认证已经恢复正常
- 但浏览器里还没有附着实际标签页，所以 `chrome` / `edge` 仍显示 `0 tabs`

### 影响
- OpenClaw 目前能稳定控制的是托管浏览器 `openclaw`
- 还不能直接操控你正在使用的 Chrome / Edge 真实标签页

### 解除方式
在 Chrome 或 Edge 中手动完成：
1. 打开扩展管理页
2. 开启开发者模式
3. `Load unpacked`
4. 选择 `~\.openclaw\browser\chrome-extension`
5. 打开扩展 `Options`
6. Chrome 填 `18792`，Edge 填 `18793`
7. 粘贴当前 gateway token 并保存
8. 固定扩展后，在目标标签页点击扩展图标，直到徽标显示 `ON`

---

## B3. 自动派单仍是设计阶段

### 现象
- `main`、Feishu、`reader`、`reviewer`、`pm`、`builder` 都已具备基础条件
- 但尚未形成稳定的“总控入口 -> 自动分发 -> 汇总回收”闭环

### 影响
- 现在不能把一句高层指令自动变成完整多 agent 协作
- 当前更适合“半自动派单”，不是“全自动自治”

### 解除方式
- 先统一入口策略
- 再定义 `pm` 的派单协议
- 最后再落地自动派单试点
