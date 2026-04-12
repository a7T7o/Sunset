import fs from "node:fs/promises";
import path from "node:path";

import registryJson from "../config/source-registry.json" with { type: "json" };

import { repoRoot } from "./paths.js";
import type { SourceDescriptor, SourceRegistryFile } from "./types.js";

export async function loadSourceRegistry(): Promise<SourceDescriptor[]> {
  const registry = registryJson as SourceRegistryFile;

  return Promise.all(
    registry.sources.map(async (source) => {
      const absolutePath = path.join(repoRoot, source.relativePath);
      await fs.access(absolutePath);
      return {
        ...source,
        absolutePath
      };
    })
  );
}
