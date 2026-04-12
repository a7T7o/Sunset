import fs from "node:fs";

import Ajv2020 from "ajv/dist/2020.js";

import schema from "../schema/xmind-schema.json" with { type: "json" };

import type { GraphFile, GraphNode, ValidationIssue, ValidationReport } from "./types.js";

export function validateGraph(graph: GraphFile): ValidationReport {
  const ajv = new Ajv2020({ allErrors: true, strict: false });
  const validate = ajv.compile(schema);
  const errors: ValidationIssue[] = [];
  const warnings: ValidationIssue[] = [];

  if (!validate(graph)) {
    for (const issue of validate.errors ?? []) {
      errors.push({
        code: `schema:${issue.keyword}`,
        message: issue.message ?? "未知 schema 错误",
        path: issue.instancePath
      });
    }
  }

  const sourceIds = new Set(graph.sources.map((source) => source.id));
  const visitedIds = new Set<string>();
  const duplicateNodeIds = new Set<string>();

  graph.sources.forEach((source) => {
    if (!fs.existsSync(source.absolutePath)) {
      errors.push({
        code: "source:missing",
        message: `来源文件不存在：${source.absolutePath}`,
        path: source.relativePath
      });
    }
  });

  const visit = (node: GraphNode, mapId: string) => {
    if (!node.title.trim()) {
      errors.push({
        code: "topic:empty-title",
        message: `节点标题不能为空：${node.semanticKey}`,
        path: `${mapId}/${node.id}`
      });
    }

    if (visitedIds.has(node.id)) {
      duplicateNodeIds.add(node.id);
    } else {
      visitedIds.add(node.id);
    }

    node.sourceRefs.forEach((ref) => {
      if (!sourceIds.has(ref.sourceId)) {
        errors.push({
          code: "ref:unknown-source",
          message: `节点引用了不存在的 sourceId：${ref.sourceId}`,
          path: `${mapId}/${node.id}`
        });
      }
      if (!ref.quoteHint.trim()) {
        errors.push({
          code: "ref:empty-quote",
          message: `节点存在空 quoteHint：${node.semanticKey}`,
          path: `${mapId}/${node.id}`
        });
      }
      if (ref.path && !fs.existsSync(ref.path)) {
        errors.push({
          code: "ref:missing-path",
          message: `sourceRef 指向的文件不存在：${ref.path}`,
          path: `${mapId}/${node.id}`
        });
      }
    });

    node.children.forEach((child) => visit(child, mapId));
  };

  graph.maps.forEach((map) => visit(map.root, map.id));

  if (duplicateNodeIds.size > 0) {
    warnings.push({
      code: "topic:duplicate-id-reused",
      message: `存在 ${duplicateNodeIds.size} 个跨图复用节点 ID；这在多图复用语义下允许，但需要保持语义一致。`
    });
  }

  const topicIndexIds = new Set(graph.topicIndex.map((entry) => entry.id));
  graph.topicIndex.forEach((entry) => {
    if (!visitedIds.has(entry.id)) {
      errors.push({
        code: "topic-index:orphan",
        message: `topicIndex 存在未被任何 map 引用的节点：${entry.semanticKey}`,
        path: entry.id
      });
    }
  });

  visitedIds.forEach((id) => {
    if (!topicIndexIds.has(id)) {
      errors.push({
        code: "topic-index:missing",
        message: `某个 map 节点未被收录到 topicIndex：${id}`,
        path: id
      });
    }
  });

  return {
    ok: errors.length === 0,
    generatedAt: new Date().toISOString(),
    stats: {
      sourceCount: graph.sources.length,
      mapCount: graph.maps.length,
      topicCount: graph.topicIndex.length
    },
    errors,
    warnings
  };
}
