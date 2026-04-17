---
description: "Apply fixes from a SpecKit Review Report (review.md) to implementation source code. Use after speckit.review identifies issues."
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Goal

Read the review report (`FEATURE_DIR/review.md`) produced by `/speckit.review` and apply the recommended fixes to the implementation source code. You are the **execution agent** — follow fix instructions precisely without creative interpretation.

## Operating Constraints

**WRITE MODE on source code — READ-ONLY on spec artifacts and review.md**: Modify only the implementation files referenced in review.md findings. Do NOT modify `spec.md`, `plan.md`, `tasks.md`, or `review.md` itself.

**Strict Adherence**: Apply ONLY the fixes described in `review.md`. Do not refactor, add features, or "improve" code that wasn't flagged.

**Constitution Authority**: The project constitution (`.specify/memory/constitution.md`) is **non-negotiable**. If a fix would violate a MUST principle, skip and flag it.

## Execution Steps

### 1. Initialize Fix Context

Run `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` once from repo root and parse JSON for FEATURE_DIR.

### 2. Load Review Report

Read `FEATURE_DIR/review.md`. If `status: approved`, exit with success message. Otherwise parse all findings.

### 3. Load Constitution

Read `.specify/memory/constitution.md` for MUST/SHOULD guardrails.

### 4. Apply Fixes by Priority

Process CRITICAL → HIGH → MEDIUM → LOW. For each finding:
1. Locate the exact code in the specified file
2. Follow the step-by-step fix instructions
3. Use the "Expected result" code if provided
4. Skip if instructions are ambiguous or would violate constitution

### 5. Build Verification

Run `dotnet build --no-restore` (or appropriate build command) after all fixes.

### 6. Output Fix Summary

Report applied/skipped fixes, build status, and modified files. Recommend `/speckit.review` to verify.
