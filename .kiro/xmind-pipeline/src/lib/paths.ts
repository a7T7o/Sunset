import path from "node:path";
import { fileURLToPath } from "node:url";

const thisFile = fileURLToPath(import.meta.url);
const libDir = path.dirname(thisFile);

export const pipelineRoot = path.resolve(libDir, "..", "..");
export const repoRoot = path.resolve(pipelineRoot, "..", "..");
export const outputDir = path.join(pipelineRoot, "output");
export const graphOutputPath = path.join(pipelineRoot, "sunset-master-graph.json");
export const validationReportPath = path.join(outputDir, "validation-report.json");
export const incrementalReportPath = path.join(outputDir, "incremental-update-report.json");

export const registryPath = path.join(pipelineRoot, "src", "config", "source-registry.json");
export const schemaPath = path.join(pipelineRoot, "src", "schema", "xmind-schema.json");
