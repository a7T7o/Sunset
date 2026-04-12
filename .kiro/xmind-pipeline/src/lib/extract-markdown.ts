import fs from "node:fs/promises";

import type { ExtractedDocument, ExtractedSection, SourceDescriptor } from "./types.js";

const headingPattern = /^(#{1,6})\s+(.*)$/;
const listPattern = /^\s*(?:[-*]|\d+\.)\s+(.*)$/;

function stripInlineMarkdown(value: string): string {
  return value
    .replace(/`([^`]+)`/g, "$1")
    .replace(/\*\*([^*]+)\*\*/g, "$1")
    .replace(/\*([^*]+)\*/g, "$1")
    .replace(/\[([^\]]+)\]\([^)]+\)/g, "$1")
    .replace(/\s+/g, " ")
    .trim();
}

function buildExcerpt(body: string): string {
  const lines = body
    .split(/\r?\n/)
    .map((line) => stripInlineMarkdown(line))
    .filter(Boolean);

  return (lines.join(" ").slice(0, 180) || "该章节当前没有补充正文。").trim();
}

export async function extractMarkdownDocument(source: SourceDescriptor): Promise<ExtractedDocument> {
  const content = await fs.readFile(source.absolutePath, "utf8");
  const lines = content.split(/\r?\n/);
  const sections: ExtractedSection[] = [];
  const stack: Array<{ level: number; headingText: string }> = [];

  let currentHeadingLine = "";
  let currentHeadingText = source.title;
  let currentLevel = 1;
  let currentBody: string[] = [];

  const flushSection = () => {
    if (!currentHeadingLine) {
      return;
    }

    const body = currentBody.join("\n").trim();
    const listItems = body
      .split(/\r?\n/)
      .map((line) => listPattern.exec(line)?.[1]?.trim())
      .filter((value): value is string => Boolean(value));

    sections.push({
      headingLine: currentHeadingLine,
      headingText: currentHeadingText,
      normalizedHeading: stripInlineMarkdown(currentHeadingText),
      level: currentLevel,
      path: stack.map((entry) => entry.headingText),
      body,
      excerpt: buildExcerpt(body),
      listItems
    });
  };

  for (const line of lines) {
    const headingMatch = headingPattern.exec(line);
    if (headingMatch) {
      flushSection();

      const level = headingMatch[1].length;
      const headingText = headingMatch[2].trim();
      while (stack.length > 0 && stack[stack.length - 1].level >= level) {
        stack.pop();
      }
      stack.push({ level, headingText });

      currentHeadingLine = line.trim();
      currentHeadingText = headingText;
      currentLevel = level;
      currentBody = [];
      continue;
    }

    if (currentHeadingLine) {
      currentBody.push(line);
    }
  }

  flushSection();

  return {
    sourceId: source.id,
    title: source.title,
    absolutePath: source.absolutePath,
    content,
    sections
  };
}

export async function extractAllDocuments(sources: SourceDescriptor[]): Promise<Record<string, ExtractedDocument>> {
  const entries = await Promise.all(sources.map(async (source) => [source.id, await extractMarkdownDocument(source)] as const));
  return Object.fromEntries(entries);
}

export function findSection(document: ExtractedDocument, sectionHint: string): ExtractedSection {
  const exact = document.sections.find((section) => section.headingLine === sectionHint || section.headingText === sectionHint);
  if (exact) {
    return exact;
  }

  const normalizedHint = stripInlineMarkdown(sectionHint.replace(/^#{1,6}\s+/, ""));
  const fuzzy = document.sections.find((section) => section.normalizedHeading === normalizedHint);
  if (fuzzy) {
    return fuzzy;
  }

  throw new Error(`无法在来源 ${document.sourceId} 中找到章节：${sectionHint}`);
}

export function findDescendantSections(document: ExtractedDocument, target: ExtractedSection): ExtractedSection[] {
  const targetPath = target.path.join(" > ");
  return document.sections.filter((section) => {
    if (section.headingLine === target.headingLine) {
      return false;
    }

    return section.path.length > target.path.length && section.path.slice(0, target.path.length).join(" > ") === targetPath;
  });
}
