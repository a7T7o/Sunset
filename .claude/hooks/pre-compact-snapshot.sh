#!/usr/bin/env bash
set -euo pipefail

# Hook: pre-compact reminder marker
# Purpose: Warn that compaction is approaching.
# No snapshot file is created here.

echo "[hook][reminder][pre-compact] compaction marker only; no snapshot automation configured"
