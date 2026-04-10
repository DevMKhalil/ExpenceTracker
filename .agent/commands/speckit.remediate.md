---
description: "Apply fixes from a SpecKit Analysis Report to spec.md, plan.md, and tasks.md. Use after /speckit.analyze identifies issues that need remediation."
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Goal

Read a provided SpecKit Analysis Report (the output of `/speckit.analyze`) and directly apply the recommended fixes to the corresponding project files (`spec.md`, `plan.md`, `tasks.md`) in the current workspace. This command bridges the gap between analysis and implementation — turning findings into concrete file edits.

## Operating Constraints

**WRITE MODE**: This agent modifies `spec.md`, `plan.md`, and `tasks.md` directly. All changes are scoped strictly to the recommendations in the provided analysis report.

**Strict Adherence**: Base your changes STRICTLY on the "Recommendation" column of the provided report. Do not hallucinate fixes, rewrite unrelated sections, or apply changes outside the scope of the report. If a recommendation is ambiguous or incomplete, flag it and skip rather than guessing.

**Constitution Authority**: The project constitution (`.specify/memory/constitution.md`) is **non-negotiable**. If a recommendation in the analysis report would violate a constitution MUST principle, **do not apply it**. Instead, flag the conflict in the output summary and recommend the user update the recommendation or amend the constitution separately.

**No New Branches**: Apply all modifications directly to the current files in the active workspace. Do NOT create or suggest creating a new Git branch.

## Execution Steps

### 1. Initialize Remediation Context

Run `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` once from repo root and parse JSON for FEATURE_DIR and AVAILABLE_DOCS. Derive absolute paths:

- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md
- ANALYSIS = FEATURE_DIR/analysis.md

Abort with an error message if any required file is missing (instruct the user to run the missing prerequisite command).
For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot").

### 2. Load Constitution

Read `.specify/memory/constitution.md` and extract all MUST/SHOULD normative statements. These serve as guardrails — no edit may violate a MUST principle.

### 3. Obtain Analysis Report

**Primary — Auto-detect from file**: Check if `ANALYSIS` (i.e., `FEATURE_DIR/analysis.md`) exists.

- If found:
  1. Read the file contents
  2. Parse the YAML frontmatter to extract `generated_at` and `source_files` timestamps
  3. **Stale detection**: For each file in `source_files`, compare its recorded timestamp against the file's current last-modified time. Run:
     ```powershell
     Get-Item "$SPEC","$PLAN","$TASKS" | Select-Object Name, @{N='LastWrite';E={$_.LastWriteTimeUtc.ToString('o')}}
     ```
     If ANY source file has a last-modified time **newer** than the timestamp recorded in the report, emit a warning:
     > "⚠ Analysis report may be stale — `{file}` was modified after the report was generated ({report_time} vs {current_time}). Consider re-running `/speckit.analyze` for an up-to-date report."
     
     **Continue processing** (do not block) — the warning is informational only.
  4. Proceed to Step 4 with the report content (skip the frontmatter when parsing findings)

**Fallback — User-provided report**: If `analysis.md` does not exist, accept the report from:

- **Direct paste**: User pastes the report into the chat message or as `$ARGUMENTS`
- **File reference**: User points to a file containing the report

If neither the file exists nor a report is provided, prompt the user:

> "No `analysis.md` found in the feature directory. Please either:
> 1. Run `/speckit.analyze` first to generate the report, or
> 2. Paste the analysis report directly into the chat."

### 4. Parse the Findings Table

Extract each row from the report's Findings table:

| Field | Description |
|-------|-------------|
| **ID** | Finding identifier (e.g., A1, C3, E7) |
| **Category** | Duplication, Ambiguity, Underspecification, Constitution, Coverage, Inconsistency |
| **Severity** | CRITICAL, HIGH, MEDIUM, LOW |
| **Location(s)** | Target file and section (e.g., `spec.md` FR-024, `tasks.md` T058) |
| **Summary** | Description of the issue |
| **Recommendation** | The exact fix to apply |

Also extract:
- **Coverage Summary Table** — requirements missing task coverage
- **Constitution Alignment Issues** — principle violations
- **Unmapped Tasks** — tasks with no requirement mapping

### 5. Prioritize Findings

Sort all findings by severity for processing order:

1. **CRITICAL** — Must be resolved (constitution violations, missing core coverage)
2. **HIGH** — Should be resolved (duplicates, conflicts, untestable criteria)
3. **MEDIUM** — Recommended (terminology drift, edge case gaps)
4. **LOW** — Optional (style, minor wording)

### 6. Apply Fixes File-by-File

Process files in this strict order to maintain consistency:

#### A. spec.md (First)

- Read the current `spec.md`
- For each finding targeting `spec.md`, apply the Recommendation:
  - **Duplication**: Merge or remove duplicate requirements, keeping the clearer phrasing
  - **Ambiguity**: Replace vague terms with measurable criteria as specified
  - **Underspecification**: Add missing details, acceptance criteria, or edge cases
  - **Inconsistency**: Align terminology, fix conflicting statements
- Preserve existing section structure and numbering
- Do NOT renumber requirements if one is removed — leave a note: `<!-- Removed: FR-XXX consolidated into FR-YYY per analysis report -->`

#### B. plan.md (Second)

- Read the current `plan.md`
- For each finding targeting `plan.md`, apply the Recommendation:
  - **Architecture fixes**: Update stack choices, data model references, or phase descriptions
  - **Terminology alignment**: Match terminology to updated `spec.md`
  - **Constitution compliance**: Ensure architecture decisions respect constitution principles
- Preserve the Constitution Check table integrity

#### C. tasks.md (Third)

- Read the current `tasks.md`
- For each finding targeting `tasks.md`, apply the Recommendation:
  - **Coverage gaps**: Add new tasks for uncovered requirements (place in appropriate phase)
  - **Unmapped tasks**: Add requirement references to orphan tasks, or flag for removal
  - **Dependency fixes**: Correct task ordering or add missing dependency notes
  - **Terminology alignment**: Match task descriptions to updated `spec.md` and `plan.md`
- **CRITICAL — Task Numbering Integrity**: NEVER renumber the entire task list. If a task is removed, replace its content with: `<!-- Task T0XX intentionally removed per analysis report: [reason] -->` to maintain sequence integrity.
- New tasks MUST be appended at the end of their target phase with the next available ID in sequence.

### 7. Validate Constitution Compliance

After applying all edits, do a final pass:

- Re-check that no applied edit violates a constitution MUST principle
- If a violation is detected, **revert that specific edit** and add it to the Skipped Fixes list

### 8. Output Remediation Summary

Output a structured summary confirming all applied changes:

```markdown
## Remediation Summary

### Applied Fixes

| Finding ID | Severity | File | Action Taken |
|------------|----------|------|-------------|
| A1 | HIGH | spec.md | Merged FR-012 into FR-008 |
| C3 | CRITICAL | tasks.md | Added task T073 for FR-024 coverage |

### Skipped Fixes (if any)

| Finding ID | Severity | Reason |
|------------|----------|--------|
| D2 | MEDIUM | Recommendation conflicts with Constitution Principle I (CQRS) |

### Files Modified
- spec.md: X changes
- plan.md: Y changes
- tasks.md: Z changes

### Next Steps
Run `/speckit.analyze` again to verify all issues are resolved.
```

If the report was read from `analysis.md`, include the source attribution:
```markdown
### Source
- **Report file**: `analysis.md`
- **Generated at**: {generated_at from frontmatter}
- **Stale warnings**: {count, or "None"}
```

## Operating Principles

### Safety

- **Minimal edits**: Apply only what the report recommends — no opportunistic refactoring
- **Constitution is sacrosanct**: Never apply a fix that violates a MUST principle
- **Preserve structure**: Maintain existing numbering, IDs, and document organization
- **Task numbering integrity**: Never renumber tasks — use placeholder comments for removals
- **Reversible changes**: Each edit should be traceable back to a specific finding ID

### Quality

- **Exact phrasing**: Use the wording from the Recommendation column when possible
- **Cross-file consistency**: After editing spec.md, ensure plan.md and tasks.md terminology matches
- **Traceability**: Every edit links to a finding ID from the analysis report

### Boundaries

- **NEVER** apply fixes not present in the analysis report
- **NEVER** rewrite sections that aren't mentioned in findings
- **NEVER** add new features, requirements, or tasks beyond what the report recommends
- **NEVER** modify the constitution (that requires a separate `/speckit.constitution` update)
- **NEVER** create new Git branches
- **ALWAYS** recommend running `/speckit.analyze` after remediation to verify

## Context

$ARGUMENTS
