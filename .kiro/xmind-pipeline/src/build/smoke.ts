import { cleanOutput } from "./clean-output.js";
import { buildMasterGraphFile } from "./build-master-graph.js";
import { validateGraphFile } from "./validate-graph.js";
import { generateAllXmindFiles } from "./generate-maps.js";
import { runIncrementalExperiment } from "./run-incremental-experiment.js";
import { isDirectRun } from "../lib/runtime.js";

export async function runSmoke() {
  await cleanOutput();
  const graph = await buildMasterGraphFile();
  const validation = await validateGraphFile();
  if (!validation.ok) {
    throw new Error(`Graph 校验失败，错误数：${validation.errors.length}`);
  }
  const generation = await generateAllXmindFiles();
  const incremental = await runIncrementalExperiment();

  return {
    generatedAt: new Date().toISOString(),
    graph: {
      sourceCount: graph.sources.length,
      topicCount: graph.topicIndex.length,
      mapCount: graph.maps.length
    },
    validation,
    generation,
    incremental
  };
}

if (isDirectRun(import.meta.url)) {
  await runSmoke();
}
