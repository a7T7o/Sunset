import fs from "node:fs/promises";
import path from "node:path";

import AdmZip from "adm-zip";
import { Workbook as SdkWorkbook } from "xmind";
import { RootTopic, Topic, Workbook } from "xmind-generator";

import { outputDir } from "./paths.js";
import type { GraphMap, GraphNode } from "./types.js";

function buildNote(node: GraphNode): string {
  const lines = [
    node.summary,
    "",
    `kind: ${node.kind}`,
    `status: ${node.status}`,
    `stability: ${node.stability}`,
    "",
    "sourceRefs:"
  ];

  for (const ref of node.sourceRefs) {
    lines.push(`- [${ref.sourceId}] ${ref.section}`);
  }

  return lines.join("\n");
}

function buildTopic(node: GraphNode, isRoot = false): any {
  let builder = isRoot ? RootTopic(node.title) : Topic(node.title);
  builder = builder.ref(node.id).note(buildNote(node)).labels(node.labels ?? []);

  if (node.children.length > 0) {
    builder = builder.children(node.children.map((child) => buildTopic(child)));
  }

  return builder;
}

export async function writeMapFile(map: GraphMap): Promise<string> {
  await fs.mkdir(outputDir, { recursive: true });
  const workbook = Workbook(buildTopic(map.root, true));
  const filePath = path.join(outputDir, map.fileName);
  const archive = await workbook.archive();
  await fs.writeFile(filePath, Buffer.from(archive));
  return filePath;
}

export async function writeAllMaps(maps: GraphMap[]): Promise<string[]> {
  const outputs: string[] = [];
  for (const map of maps) {
    outputs.push(await writeMapFile(map));
  }
  return outputs;
}

export function smokeCheckXmindArchive(filePath: string) {
  const zip = new AdmZip(filePath);
  const entries = zip.getEntries().map((entry) => entry.entryName);
  return {
    exists: entries.length > 0,
    hasManifest: entries.some((entry) => entry.endsWith("manifest.json") || entry.endsWith("manifest.xml")),
    hasContentJson: entries.some((entry) => entry.endsWith("content.json")),
    entryCount: entries.length
  };
}

export function getSdkReadiness() {
  const workbook = new SdkWorkbook();
  workbook.createSheet("SDK Smoke", "Central Topic");
  const status = workbook.validate();
  return {
    package: "xmind",
    ok: Boolean(status.status),
    errors: status.errors ?? []
  };
}
