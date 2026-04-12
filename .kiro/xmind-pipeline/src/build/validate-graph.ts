import fs from "node:fs/promises";
import path from "node:path";

import { buildMasterGraphFile } from "./build-master-graph.js";
import { graphOutputPath, validationReportPath } from "../lib/paths.js";
import { isDirectRun } from "../lib/runtime.js";
import { validateGraph } from "../lib/validate.js";
import type { GraphFile } from "../lib/types.js";

async function readGraph(): Promise<GraphFile> {
  try {
    const raw = await fs.readFile(graphOutputPath, "utf8");
    return JSON.parse(raw) as GraphFile;
  } catch {
    return buildMasterGraphFile();
  }
}

export async function validateGraphFile() {
  const graph = await readGraph();
  const report = validateGraph(graph);
  await fs.mkdir(path.dirname(validationReportPath), { recursive: true });
  await fs.writeFile(validationReportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
  return report;
}

if (isDirectRun(import.meta.url)) {
  const report = await validateGraphFile();
  if (!report.ok) {
    process.exitCode = 1;
  }
}
