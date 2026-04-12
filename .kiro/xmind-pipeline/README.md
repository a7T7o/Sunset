# Sunset XMind 工程化管线

这套管线负责把 `Sunset` 的正式主文和 `项目文档总览` 母材料，稳定生成成可回指来源的 `.xmind` 导图。

## 当前边界

第一阶段只做三件事：

1. 读取白名单 Markdown 来源
2. 生成 `sunset-master-graph.json`
3. 从同一份 graph 一键生成 `7` 张 `.xmind`

第一阶段明确不做：

1. MCP 直接改 XMind
2. 大模型自由总结改写主文
3. 把线程回执原话直接塞进主图

## 目录

- `src/config/source-registry.json`
  - 白名单来源清单
- `src/config/topic-blueprints.json`
  - 当前导图蓝图
- `src/schema/xmind-schema.json`
  - graph 契约
- `src/build/`
  - build / validate / generate / smoke
- `src/lib/`
  - Markdown 抽取、归一化、稳定 ID、XMind 适配
- `fixtures/`
  - 增量更新实验夹具
- `tests/`
  - 最小工程化测试
- `output/`
  - `.xmind`、校验报告、实验报告

## 命令

```powershell
npm install
npm run smoke
npm run test
```

单独执行：

```powershell
npm run build:graph
npm run validate:graph
npm run generate:xmind
npm run experiment:incremental
```

## 当前产物

执行 `npm run smoke` 后会生成：

1. `sunset-master-graph.json`
2. `output/validation-report.json`
3. `output/incremental-update-report.json`
4. `output/Sunset_项目总图.xmind`
5. `output/Sunset_系统版图.xmind`
6. `output/Sunset_剧情与NPC.xmind`
7. `output/Sunset_交互与状态边界.xmind`
8. `output/Sunset_表现层与场景链.xmind`
9. `output/Sunset_工具链与AI治理.xmind`
10. `output/Sunset_进度与优先级.xmind`

## 规则口径

1. 第一层长期主文和第二层 `项目文档总览` 母材料允许生成主节点。
2. 第三层线程 memory / prompt / 一次性回执默认只允许做 `sourceRefs`，不直接长成主节点。
3. 节点 ID 按 `semanticKey` 稳定生成，不跟着自然语言标题频繁漂移。
4. 每次生成都必须过 `schema + 自定义校验 + XMind 文件 smoke`。
