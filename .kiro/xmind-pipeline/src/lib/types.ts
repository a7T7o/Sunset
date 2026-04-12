export type Kind =
  | "project"
  | "module"
  | "system"
  | "scene"
  | "flow"
  | "rule"
  | "toolchain"
  | "governance"
  | "milestone"
  | "risk"
  | "source";

export type Status = "established" | "in_progress" | "partial" | "planned" | "parked" | "deprecated";
export type Stability = "canonical" | "stable" | "active" | "volatile" | "historical";

export type SourceRegistryEntry = {
  id: string;
  title: string;
  relativePath: string;
  tier: "canonical" | "supporting";
  allowPrimaryNodes: boolean;
  kindHints: string[];
  stability: Stability;
};

export type SourceRegistryFile = {
  version: number;
  sources: SourceRegistryEntry[];
};

export type SourceDescriptor = SourceRegistryEntry & {
  absolutePath: string;
};

export type ExtractedSection = {
  headingLine: string;
  headingText: string;
  normalizedHeading: string;
  level: number;
  path: string[];
  body: string;
  excerpt: string;
  listItems: string[];
};

export type ExtractedDocument = {
  sourceId: string;
  title: string;
  absolutePath: string;
  content: string;
  sections: ExtractedSection[];
};

export type GraphSourceRef = {
  sourceId: string;
  path: string;
  section: string;
  quoteHint: string;
  confidence: number;
};

export type GraphNode = {
  id: string;
  semanticKey: string;
  title: string;
  kind: Kind;
  summary: string;
  children: GraphNode[];
  sourceRefs: GraphSourceRef[];
  stability: Stability;
  status: Status;
  labels?: string[];
};

export type TopicIndexEntry = {
  id: string;
  semanticKey: string;
  title: string;
  kind: Kind;
  status: Status;
  stability: Stability;
  mapIds: string[];
  sourceRefCount: number;
};

export type GraphMap = {
  id: string;
  title: string;
  fileName: string;
  root: GraphNode;
};

export type GraphFile = {
  version: number;
  generatedAt: string;
  repositoryRoot: string;
  sources: SourceDescriptor[];
  topicIndex: TopicIndexEntry[];
  maps: GraphMap[];
};

export type ValidationIssue = {
  code: string;
  message: string;
  path?: string;
};

export type ValidationReport = {
  ok: boolean;
  generatedAt: string;
  stats: {
    sourceCount: number;
    mapCount: number;
    topicCount: number;
  };
  errors: ValidationIssue[];
  warnings: ValidationIssue[];
};
