import fs from "node:fs/promises";

import { graphOutputPath, outputDir } from "../lib/paths.js";
import { isDirectRun } from "../lib/runtime.js";

export async function cleanOutput(): Promise<void> {
  await fs.rm(outputDir, { recursive: true, force: true });
  await fs.mkdir(outputDir, { recursive: true });
  await fs.rm(graphOutputPath, { force: true });
}

if (isDirectRun(import.meta.url)) {
  await cleanOutput();
}
