# 浏览器与 WebSearch 说明（2026-03-09）

## 一、这次实际修复了什么

### 1. 浏览器 relay 认证漂移
- 根因不是模型，也不是 2API 本身
- 根因是仓库根 `.env` 里残留了旧的 `OPENCLAW_GATEWAY_TOKEN`
- 这会导致 OpenClaw 在不同位置读到不同 token，从而出现 relay 端口一会儿能用、一会儿 `401`

### 2. 启动脚本继承 token 不稳定
- 已补强 `start-openclaw-hidden.ps1`
- 现在它会在启动 Gateway 前主动读取 `C:\Users\aTo\.openclaw\openclaw.json` 中的 `gateway.auth.token`
- 这样用户以后运行 `start-openclaw.cmd` 时，token 继承会更稳定

### 3. Edge 遗留扩展污染
- `~\.openclaw\browser\edge-extension` 里存在旧版硬编码 token
- 已移除这些硬编码默认值，避免 Edge 再次回退到旧 token

## 二、现在已经可以做到什么

### 可用能力
- `main` 可用 `browser`
- `main` 可用 `web_fetch`
- OpenClaw 托管浏览器 `openclaw` profile 已验证可打开网页
- `chrome` relay `18792` 已验证认证正常
- `edge` relay `18793` 已验证认证正常
- 浏览器控制服务 `18791` 已验证使用当前 token 可访问

### 当前边界
- `chrome` / `edge` relay 还没有附着实际标签页
- 所以现在浏览器层面是“底座已好，最后一跳还要手动点”

## 三、为什么 `web_search` 还是不行

### 真实原因
- `web_search` 不复用你当前模型中转的 `OPENAI_API_KEY`
- 它走的是独立搜索 provider
- 你当前没有配置真实可用的搜索 provider key

### 当前已做处理
- 已显式把 `tools.web.search.provider` 设为 `brave`
- 已确认 `web_fetch` 正常可用
- 已确认 `web_search` 的失败原因就是缺搜索 key，而不是别的配置错误

### 结论
- 我不能凭空把 `web_search` 自动补齐
- 你必须提供至少一个真实可用的搜索 provider key，才能让它真正联网搜索

## 四、当前推荐联网策略

### 立即可用
1. 用 `browser` 处理网页交互、动态页面、登录态页面
2. 用 `web_fetch` 抓已知 URL 内容

### 待补齐
3. 用 `web_search` 做“自然语言直接搜网”，前提是补搜索 key

## 五、Chrome / Edge 最后一跳怎么做

### Chrome
1. 运行 `一键配置Chrome扩展.cmd`
2. 在 Chrome 扩展页开启开发者模式
3. `Load unpacked`
4. 选择 `~\.openclaw\browser\chrome-extension`
5. 打开扩展 `Options`
6. 确认端口是 `18792`
7. 粘贴当前 gateway token 并保存
8. 在目标标签页点击扩展图标，直到徽标显示 `ON`

### Edge
1. 运行 `一键配置Edge扩展.cmd`
2. 在 Edge 扩展页开启开发者模式
3. `Load unpacked`
4. 选择 `~\.openclaw\browser\chrome-extension`
5. 打开扩展 `Options`
6. 把端口改成 `18793`
7. 粘贴当前 gateway token 并保存
8. 在目标标签页点击扩展图标，直到徽标显示 `ON`

## 六、对下一阶段的判断

- 现在可以开始做“多 agent 入口与派单设计”
- 但更适合先做“半自动派单”，不要直接跳到全自动自治
- 推荐顺序：
  1. 浏览器最后一跳
  2. `main/Feishu -> pm` 入口统一
  3. `pm -> reader/reviewer/builder` 派单协议
  4. 自动派单试点
