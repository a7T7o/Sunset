# progress

## 当前总状态
- 阶段名称：联网与浏览器基础设施收敛
- 状态：核心链路已恢复，可进入下一阶段
- 最后更新：2026-03-09

## 已完成
- `main` 默认模型已稳定为 `gpt-5.4-xhigh-fast`
- `reader`、`reviewer` 已能读取 `D:\Unity\Unity_learning\Sunset`
- Feishu 私聊链路可用，Gateway 已重新启动并稳定监听
- `main` 已具备 `browser`、`web_fetch`、`web_search` 工具入口
- OpenClaw 托管浏览器 `openclaw` profile 已验证可打开网页
- `chrome` relay `18792` 已验证认证正常
- `edge` relay `18793` 已验证认证正常
- 仓库根 `.env` 中旧 `OPENCLAW_GATEWAY_TOKEN` 已对齐到当前配置
- `start-openclaw-hidden.ps1` 已补强为启动时显式继承当前配置 token
- Edge 遗留扩展中的硬编码旧 token 已移除

## 已验证
- `http://127.0.0.1:18791/` 浏览器控制服务使用当前 token 可访问
- `http://127.0.0.1:18792/json/version` 使用当前 token 可访问
- `http://127.0.0.1:18793/json/version` 使用当前 token 可访问
- `pnpm openclaw browser open https://example.com` 成功
- `main` 调用 `web_fetch` 成功
- `main` 调用 `web_search` 仍失败，但失败原因已收敛为“缺搜索 provider key”

## 当前未完成
- Chrome / Edge 仍需要用户在浏览器 UI 中手动加载 unpacked extension 并点击附着标签页
- `web_search` 仍缺真实可用的搜索 provider key
- `main/Feishu -> pm -> worker` 的正式自动派单流程尚未落地

## 当前判断
- 现在已经不是“系统能不能启动”的问题
- 现在也不是“browser relay 认证漂移”的问题
- 当前真正剩余的是两类工作：
  - 浏览器侧最后一跳的人机操作
  - 多 agent 流程设计与自动派单落地
