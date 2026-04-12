import fs from "node:fs/promises";
import path from "node:path";

import { extractMarkdownDocument, findSection } from "../lib/extract-markdown.js";
import { incrementalReportPath, pipelineRoot } from "../lib/paths.js";
import { isDirectRun } from "../lib/runtime.js";
import { buildNodeId } from "../lib/topic-factory.js";
import type { SourceDescriptor } from "../lib/types.js";

async function readFixtureDocument(fileName: string) {
  const source: SourceDescriptor = {
    id: fileName.replace(/\.md$/, ""),
    title: fileName,
    relativePath: `fixtures/incremental/${fileName}`,
    absolutePath: path.join(pipelineRoot, "fixtures", "incremental", fileName),
    tier: "supporting",
    allowPrimaryNodes: true,
    kindHints: ["project"],
    stability: "active"
  };

  return extractMarkdownDocument(source);
}

export async function runIncrementalExperiment() {
  const base = await readFixtureDocument("base.md");
  const updated = await readFixtureDocument("updated.md");
  const baseSection = findSection(base, "## 2. 项目是什么");
  const updatedSection = findSection(updated, "## 2. 项目是什么");
  const stableId = buildNodeId("incremental::project-positioning");

  const report = {
    generatedAt: new Date().toISOString(),
    stableSemanticKey: "incremental::project-positioning",
    stableId,
    before: {
      excerpt: baseSection.excerpt,
      id: stableId
    },
    after: {
      excerpt: updatedSection.excerpt,
      id: stableId
    },
    idStable: stableId === buildNodeId("incremental::project-positioning"),
    contentChanged: baseSection.excerpt !== updatedSection.excerpt
  };

  await fs.writeFile(incrementalReportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
  return report;
}

if (isDirectRun(import.meta.url)) {
  await runIncrementalExperiment();
}
