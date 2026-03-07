#!/usr/bin/env bash
set -euo pipefail

# Hook: prompt-submit marker
# Purpose: Lightweight lifecycle marker only.
# It does not inject steering or other large prompt content into the model context.

echo "[hook][marker][prompt-submit] lifecycle marker only; steering must still be read explicitly"
