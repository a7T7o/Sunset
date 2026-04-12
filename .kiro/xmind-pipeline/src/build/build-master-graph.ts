import fs from "node:fs/promises";

import { graphOutputPath } from "../lib/paths.js";
import { buildMasterGraph } from "../lib/normalize.js";
import { isDirectRun } from "../lib/runtime.js";

export async function buildMasterGraphFile() {
  const graph = await buildMasterGraph();
  await fs.writeFile(graphOutputPath, `${JSON.stringify(graph, null, 2)}\n`, "utf8");
  return graph;
}

if (isDirectRun(import.meta.url)) {
  await buildMasterGraphFile();
}
