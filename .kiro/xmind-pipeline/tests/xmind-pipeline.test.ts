import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import test from "node:test";

import { extractMarkdownDocument, findSection } from "../src/lib/extract-markdown.js";
import { buildMasterGraph } from "../src/lib/normalize.js";
import { outputDir } from "../src/lib/paths.js";
import { loadSourceRegistry } from "../src/lib/source-registry.js";
import { buildNodeId } from "../src/lib/topic-factory.js";
import { validateGraph } from "../src/lib/validate.js";
import { smokeCheckXmindArchive, writeMapFile } from "../src/lib/xmind-writer.js";

test("source registry 中的路径全部存在", async () => {
  const sources = await loadSourceRegistry();
  assert.ok(sources.length >= 10);
  sources.forEach((source) => assert.ok(fs.existsSync(source.absolutePath), source.absolutePath));
});

test("Markdown 标题解析可定位真实章节", async () => {
  const [source] = (await loadSourceRegistry()).filter((item) => item.id === "plan-overview");
  const document = await extractMarkdownDocument(source);
  const section = findSection(document, "## 5. 项目核心玩法闭环");
  assert.equal(section.headingText, "5. 项目核心玩法闭环");
  assert.ok(section.excerpt.includes("资源积累") || section.excerpt.length > 10);
});

test("同一 semanticKey 会生成稳定节点 ID", () => {
  const first = buildNodeId("demo::stable");
  const second = buildNodeId("demo::stable");
  const third = buildNodeId("demo::other");
  assert.equal(first, second);
  assert.notEqual(first, third);
});

test("master graph 的 sourceRefs 都能回指真实文件", async () => {
  const graph = await buildMasterGraph();
  graph.maps.forEach((map) => {
    const stack = [map.root];
    while (stack.length > 0) {
      const node = stack.pop()!;
      node.sourceRefs.forEach((ref) => {
        assert.ok(fs.existsSync(ref.path), ref.path);
        assert.ok(ref.section.length > 0);
      });
      stack.push(...node.children);
    }
  });
});

test("graph 可通过 schema 与自定义校验", async () => {
  const graph = await buildMasterGraph();
  const report = validateGraph(graph);
  assert.equal(report.ok, true, JSON.stringify(report.errors, null, 2));
});

test("可生成第一张 xmind 并通过 archive smoke", async () => {
  const graph = await buildMasterGraph();
  fs.mkdirSync(outputDir, { recursive: true });
  const filePath = await writeMapFile(graph.maps[0]);
  const smoke = smokeCheckXmindArchive(filePath);
  assert.equal(path.extname(filePath), ".xmind");
  assert.equal(smoke.exists, true);
  assert.equal(smoke.hasContentJson, true);
});
