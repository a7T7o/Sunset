#!/usr/bin/env bash
set -euo pipefail

# Hook: stop append-only checker
# Purpose: Best-effort stop-time audit for memory append-only discipline.
# It cannot replace manual memory closure, but it can warn when an existing memory.md
# appears to have deletions in its staged or unstaged diff.

if ! git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  echo "[hook][reminder][stop] not inside git work tree; memory update remains manual"
  exit 0
fi

declare -A seen_files=()
while IFS= read -r file; do
  [ -n "$file" ] || continue
  seen_files["$file"]=1
done < <(
  {
    git diff --diff-filter=M --name-only -- 2>/dev/null || true
    git diff --cached --diff-filter=M --name-only -- 2>/dev/null || true
  } | tr -d '\r' | grep -E '(^|/|\\)memory\.md$' || true
)

violations=()
for file in "${!seen_files[@]}"; do
  unstaged_diff="$(git diff --unified=0 -- "$file" || true)"
  staged_diff="$(git diff --cached --unified=0 -- "$file" || true)"

  if printf '%s
%s
' "$unstaged_diff" "$staged_diff" | grep -Eq '^-[^-]'; then
    violations+=("$file")
  fi
done

if [ "${#violations[@]}" -gt 0 ]; then
  echo "[hook][warn][stop] append-only suspicion: deletions detected in modified memory.md" >&2
  for file in "${violations[@]}"; do
    echo "[hook][warn][stop] $file" >&2
  done
  exit 2
fi

echo "[hook][reminder][stop] no deletions detected in modified memory.md; memory closure still requires manual verification"
exit 0
