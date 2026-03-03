---
name: snapshot
description: Create conversation snapshot for context preservation
---

# Snapshot Command

Execute when user triggers /snapshot:

## Step 1: Extract Context
- User prompt (complete)
- AI progress summary (completed/incomplete)
- Modified files list
- Key decisions and user preferences

## Step 2: Determine Workspace
1. Has specific workspace -> save to that workspace
2. No specific workspace -> skip

## Step 3: Determine Filename
Format: YYYY-MM-DD_SessionX_ContinuationY.md
Get session number from memory.md latest record

## Step 4: Save to Path
Path: .kiro/specs/[workspace]/缁ф壙浼氳瘽memory/

## Step 5: Update Memory
Append snapshot record to memory.md

---

Report the created snapshot file path.
