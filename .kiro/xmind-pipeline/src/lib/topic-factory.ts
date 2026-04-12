import crypto from "node:crypto";

import type { GraphNode, GraphSourceRef, Kind, Stability, Status } from "./types.js";

export function buildNodeId(semanticKey: string): string {
  const digest = crypto.createHash("sha1").update(semanticKey).digest("hex").slice(0, 12);
  return `topic-${digest}`;
}

export function createSourceRef(sourceId: string, path: string, section: string, quoteHint: string, confidence: number): GraphSourceRef {
  return {
    sourceId,
    path,
    section,
    quoteHint: quoteHint.trim() || "章节正文为空。",
    confidence
  };
}

export function createNode(input: {
  semanticKey: string;
  title: string;
  kind: Kind;
  summary: string;
  status: Status;
  stability: Stability;
  sourceRefs: GraphSourceRef[];
  children?: GraphNode[];
  labels?: string[];
}): GraphNode {
  return {
    id: buildNodeId(input.semanticKey),
    semanticKey: input.semanticKey,
    title: input.title,
    kind: input.kind,
    summary: input.summary.trim(),
    status: input.status,
    stability: input.stability,
    sourceRefs: input.sourceRefs,
    children: input.children ?? [],
    labels: input.labels
  };
}

export function normalizeSectionTitle(headingText: string): string {
  return headingText
    .replace(/^[0-9]+(?:\.[0-9]+)*[.)]?\s*/, "")
    .replace(/^`|`$/g, "")
    .trim();
}
