# 2026-04-09｜Sunset XMind 工程化管线落地与闭环说明

## 1. 这次真正落成了什么

本轮已经把 `Sunset` 的 XMind 工程化从“路线讨论”推进到“可直接运行的一套正式管线”。

当前已真实落盘在：

`D:\Unity\Unity_learning\Sunset\.kiro\xmind-pipeline\`

已经落成的核心产物包括：

1. `package.json`
2. `src/config/source-registry.json`
3. `src/config/topic-blueprints.ts`
4. `src/schema/xmind-schema.json`
5. `src/lib/` 下的 Markdown 抽取、稳定 ID、归一化、校验、XMind 写出模块
6. `src/build/` 下的 `build / validate / generate / smoke / incremental` 命令脚本
7. `tests/xmind-pipeline.test.ts`
8. `sunset-master-graph.json`
9. `output/` 下的 `7` 张 `.xmind`

这意味着：

`正式文档源 -> registry -> extract -> normalize -> graph -> xmind -> validate -> smoke`

已经不是纸面路线，而是线程自测已过的真实闭环。

## 2. 当前正式来源口径

这次没有把线程 memory、一次性 prompt 和一次性回执直接做成主图。

当前正式来源分三层：

1. 第一层：长期主文
   - `Docx/大总结/Sunset_持续策划案/00_总索引.md`
   - `01_总览.md ~ 08_进度总表.md`
2. 第二层：`项目文档总览` 母材料
   - `Sunset项目素材总包`
   - `Sunset三维度母卷`
   - `压缩原则与禁写边界`
   - `用户本周实做与统筹回执`
3. 第三层：一次性治理 prompt
   - 当前只允许做风险提醒和来源补口，不直接长成主节点

当前 `source-registry.json` 已把这套白名单固定下来。

## 3. 当前已生成的导图

当前一键生成的 `.xmind` 为：

1. `Sunset_项目总图.xmind`
2. `Sunset_系统版图.xmind`
3. `Sunset_剧情与NPC.xmind`
4. `Sunset_交互与状态边界.xmind`
5. `Sunset_表现层与场景链.xmind`
6. `Sunset_工具链与AI治理.xmind`
7. `Sunset_进度与优先级.xmind`

输出目录：

`D:\Unity\Unity_learning\Sunset\.kiro\xmind-pipeline\output\`

## 4. 当前命令

```powershell
cd D:\Unity\Unity_learning\Sunset\.kiro\xmind-pipeline
npm install
npm run smoke
npm run test
```

单跑命令：

```powershell
npm run build:graph
npm run validate:graph
npm run generate:xmind
npm run experiment:incremental
```

## 5. 当前验证结果

这轮已通过：

1. `npm run smoke`
2. `npm test`
3. `validation-report.json` = `ok: true`
4. `generation-report.json` 显示 `7/7` `.xmind` 都存在 `content.json + manifest`
5. `incremental-update-report.json` 显示：
   - `idStable = true`
   - `contentChanged = true`

也就是说，这轮已经满足：

1. 白名单来源建立
2. graph 稳定生成
3. `7` 张 `.xmind` 一键生成
4. 每张图可回指 `sourceRefs`
5. `build -> validate -> generate` 闭环可跑
6. 至少一次增量稳定性实验已做

## 6. 当前仍然保留的边界

这套第一版仍然是“结构化主文导图器”，不是“AI 自由脑补导图器”。

当前刻意保留的边界：

1. 不做大模型自由改写主文
2. 不把线程回执原话直接塞进主图
3. 不让 MCP 进入第一阶段主链
4. `xmind` SDK 当前作为已接入的 patch/适配预留层与 readiness 证明，不抢主生成链

## 7. 现在最值钱的下一步

如果继续，这条线最值钱的下一步不再是“继续讨论路线”，而是：

1. 手工打开这 `7` 张图做第一轮结构审图
2. 根据审图结果继续精修 `topic-blueprints.ts`
3. 再决定是否扩到更多主题图或更细的 patch 能力

一句话收束：

`Sunset` 的 XMind 工程化第一阶段已经正式落地，而且已经具备持续迭代的主链。

