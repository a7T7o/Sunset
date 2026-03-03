---
name: memory-update
description: Manual memory update flow
---

# Memory Update Command

Execute when user triggers /memory-update:

## Step 1: Determine Workspace
1. Has specific workspace -> use that memory.md
2. No specific workspace -> use main memory .kiro/specs/memory.md

## Step 2: Check if Update Needed
All conversations must be recorded to memory.

## Step 3: Volume Check
- Main memory threshold: 200 lines
- Sub memory threshold: 300 lines

## Step 4: Append Record

Format:
### Session X - YYYY-MM-DD
**User Request**: [summary]
**Completed**:
- [task1]
**Modified Files**:
- path/file1.cs - [operation]: [description]
**Remaining Issues**:
- [ ] [issue1]

---

Report the updated memory location and record number when done.
