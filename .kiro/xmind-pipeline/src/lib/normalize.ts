import { mapBlueprints, type BlueprintGroup, type BlueprintRef } from "../config/topic-blueprints.js";

import { extractAllDocuments, findDescendantSections, findSection } from "./extract-markdown.js";
import { loadSourceRegistry } from "./source-registry.js";
import { createNode, createSourceRef, normalizeSectionTitle } from "./topic-factory.js";
import { repoRoot } from "./paths.js";
import type { ExtractedDocument, GraphFile, GraphMap, GraphNode, Kind, SourceDescriptor, TopicIndexEntry } from "./types.js";

function dedupeSourceRefs(refs: ReturnType<typeof createSourceRef>[]) {
  const seen = new Set<string>();
  return refs.filter((ref) => {
    const key = `${ref.sourceId}::${ref.section}`;
    if (seen.has(key)) {
      return false;
    }
    seen.add(key);
    return true;
  });
}

function inferChildKind(groupKind: Kind): Kind {
  switch (groupKind) {
    case "project":
      return "module";
    case "module":
      return "system";
    default:
      return groupKind;
  }
}

function inferChildStatus(status: BlueprintGroup["status"]): BlueprintGroup["status"] {
  if (status === "in_progress") {
    return "partial";
  }
  return status;
}

function inferChildStability(stability: BlueprintGroup["stability"]): BlueprintGroup["stability"] {
  if (stability === "canonical") {
    return "stable";
  }
  return stability;
}

function resolveSource(sources: SourceDescriptor[], sourceId: string): SourceDescriptor {
  const source = sources.find((candidate) => candidate.id === sourceId);
  if (!source) {
    throw new Error(`未在 registry 中找到来源：${sourceId}`);
  }
  return source;
}

function buildSourceRef(ref: BlueprintRef, source: SourceDescriptor, documents: Record<string, ExtractedDocument>) {
  const section = findSection(documents[ref.sourceId], ref.section);
  return createSourceRef(ref.sourceId, source.absolutePath, section.headingLine, section.excerpt, 0.95);
}

function buildSectionSummary(document: ExtractedDocument, sectionHint: string): string {
  const section = findSection(document, sectionHint);
  if (section.excerpt !== "该章节当前没有补充正文。") {
    return section.excerpt;
  }

  const descendants = findDescendantSections(document, section)
    .slice(0, 4)
    .map((entry) => normalizeSectionTitle(entry.headingText))
    .filter(Boolean);

  if (descendants.length > 0) {
    return `当前章节的核心子结构包括：${descendants.join("、")}。`;
  }

  return section.excerpt;
}

function buildSectionNode(group: BlueprintGroup, ref: BlueprintRef, source: SourceDescriptor, documents: Record<string, ExtractedDocument>): GraphNode {
  const section = findSection(documents[ref.sourceId], ref.section);
  const summary = buildSectionSummary(documents[ref.sourceId], ref.section);
  return createNode({
    semanticKey: `${group.semanticKey}::${ref.sourceId}::${section.normalizedHeading}`,
    title: normalizeSectionTitle(section.headingText),
    kind: inferChildKind(group.kind),
    summary,
    status: inferChildStatus(group.status),
    stability: inferChildStability(group.stability),
    sourceRefs: [createSourceRef(ref.sourceId, source.absolutePath, section.headingLine, summary, 0.97)],
    labels: [inferChildKind(group.kind), inferChildStatus(group.status), inferChildStability(group.stability)]
  });
}

function buildGroupNode(group: BlueprintGroup, sources: SourceDescriptor[], documents: Record<string, ExtractedDocument>): GraphNode {
  const children = group.refs.map((ref) => buildSectionNode(group, ref, resolveSource(sources, ref.sourceId), documents));
  const sourceRefs = dedupeSourceRefs(group.refs.map((ref) => buildSourceRef(ref, resolveSource(sources, ref.sourceId), documents)));

  return createNode({
    semanticKey: group.semanticKey,
    title: group.title,
    kind: group.kind,
    summary: group.summary,
    status: group.status,
    stability: group.stability,
    sourceRefs,
    children,
    labels: [group.kind, group.status, group.stability]
  });
}

function buildMapNode(map: (typeof mapBlueprints)[number], sources: SourceDescriptor[], documents: Record<string, ExtractedDocument>): GraphMap {
  const children = map.groups.map((group) => buildGroupNode(group, sources, documents));
  const sourceRefs = dedupeSourceRefs(map.root.refs.map((ref) => buildSourceRef(ref, resolveSource(sources, ref.sourceId), documents)));
  const root = createNode({
    semanticKey: map.root.semanticKey,
    title: map.root.title,
    kind: map.root.kind,
    summary: map.root.summary,
    status: map.root.status,
    stability: map.root.stability,
    sourceRefs,
    children,
    labels: [map.root.kind, map.root.status, map.root.stability]
  });

  return {
    id: map.id,
    title: map.title,
    fileName: map.fileName,
    root
  };
}

function collectTopicIndex(maps: GraphMap[]): TopicIndexEntry[] {
  const index = new Map<string, TopicIndexEntry>();

  const visit = (node: GraphNode, mapId: string) => {
    const existing = index.get(node.id);
    if (!existing) {
      index.set(node.id, {
        id: node.id,
        semanticKey: node.semanticKey,
        title: node.title,
        kind: node.kind,
        status: node.status,
        stability: node.stability,
        mapIds: [mapId],
        sourceRefCount: node.sourceRefs.length
      });
    } else if (!existing.mapIds.includes(mapId)) {
      existing.mapIds.push(mapId);
    }

    node.children.forEach((child) => visit(child, mapId));
  };

  maps.forEach((map) => visit(map.root, map.id));

  return [...index.values()].sort((left, right) => left.semanticKey.localeCompare(right.semanticKey, "zh-CN"));
}

export async function buildMasterGraph(): Promise<GraphFile> {
  const sources = await loadSourceRegistry();
  const documents = await extractAllDocuments(sources);
  const maps = mapBlueprints.map((map) => buildMapNode(map, sources, documents));

  return {
    version: 1,
    generatedAt: new Date().toISOString(),
    repositoryRoot: repoRoot,
    sources,
    topicIndex: collectTopicIndex(maps),
    maps
  };
}
