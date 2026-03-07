#!/usr/bin/env bash
set -euo pipefail

# Hook: pre-bash danger gate
# Purpose: Best-effort guard for a few obviously dangerous shell patterns.
# This remains defense-in-depth rather than the main safety boundary.

cmd=""
if [ ! -t 0 ]; then
  cmd="$(cat || true)"
fi

cmd_one_line="$(printf "%s" "$cmd" | tr '\n' ' ' | sed 's/[[:space:]]\+/ /g')"

if printf "%s" "$cmd_one_line" | grep -Eiq '(^|[[:space:]])rm[[:space:]]+-rf[[:space:]]+/|git[[:space:]]+reset[[:space:]]+--hard|git[[:space:]]+push[[:space:]]+(-f|--force)'; then
  echo "[hook][block][pre-bash] matched dangerous shell pattern" >&2
  exit 2
fi

exit 0
