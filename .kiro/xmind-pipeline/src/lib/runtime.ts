import { pathToFileURL } from "node:url";

export function isDirectRun(importMetaUrl: string): boolean {
  if (!process.argv[1]) {
    return false;
  }

  return importMetaUrl === pathToFileURL(process.argv[1]).href;
}
