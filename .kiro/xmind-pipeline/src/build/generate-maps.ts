import fs from "node:fs/promises";
import path from "node:path";

import { buildMasterGraphFile } from "./build-master-graph.js";
import { graphOutputPath, outputDir } from "../lib/paths.js";
import { isDirectRun } from "../lib/runtime.js";
import { getSdkReadiness, smokeCheckXmindArchive, writeAllMaps } from "../lib/xmind-writer.js";
import type { GraphFile } from "../lib/types.js";

async function readGraph(): Promise<GraphFile> {
  try {
    const raw = await fs.readFile(graphOutputPath, "utf8");
    return JSON.parse(raw) as GraphFile;
  } catch {
    return buildMasterGraphFile();
  }
}

export async function generateAllXmindFiles() {
  const graph = await readGraph();
  const written = await writeAllMaps(graph.maps);
  const report = {
    generatedAt: new Date().toISOString(),
    sdkReadiness: getSdkReadiness(),
    outputs: written.map((filePath) => ({
      filePath,
      smoke: smokeCheckXmindArchive(filePath)
    }))
  };

  await fs.writeFile(path.join(outputDir, "generation-report.json"), `${JSON.stringify(report, null, 2)}\n`, "utf8");
  return report;
}

if (isDirectRun(import.meta.url)) {
  await generateAllXmindFiles();
}
