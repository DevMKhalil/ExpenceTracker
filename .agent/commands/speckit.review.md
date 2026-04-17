---
description: "Review implementation code against spec.md, plan.md, tasks.md, and constitution.md. Writes detailed findings to review.md in the feature directory."
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Goal

Act as a **senior code reviewer**. Systematically review the implementation produced by `/speckit.implement` against the specification artifacts (`spec.md`, `plan.md`, `tasks.md`) and the project constitution. Write a detailed, surgical review report to `FEATURE_DIR/review.md` that a **less capable model** can mechanically follow to fix every issue.

This is NOT a spec-vs-spec analysis (that's `/speckit.analyze`). This reviews **actual source code** against the specs.

## Operating Constraints

**READ-ONLY on source code and specs — WRITE-ONLY to review.md**: Do **not** modify any source code files, `spec.md`, `plan.md`, or `tasks.md`. The full structured review report is written to `FEATURE_DIR/review.md`. A concise summary is also displayed in chat.

**Surgical Precision**: The fixing model is weaker than you. Every finding MUST include:
- Exact file path and line number(s)
- The problematic code snippet (quoted verbatim)
- WHY it's wrong (referencing a specific FR-###, constitution principle, or best practice)
- Step-by-step fix instructions — treat the fix agent like a junior developer who needs precise orders

**Constitution Authority**: The project constitution (`.specify/memory/constitution.md`) is **non-negotiable**. Constitution violations are automatically CRITICAL severity.

**No Speculation**: Only report issues you can verify against the spec artifacts or constitution. Do not flag hypothetical issues or personal style preferences not grounded in project standards.

## Execution Steps

### 1. Initialize Review Context

Run `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` once from repo root and parse JSON for FEATURE_DIR and AVAILABLE_DOCS. Derive absolute paths:

- SPEC = FEATURE_DIR/spec.md
- PLAN = FEATURE_DIR/plan.md
- TASKS = FEATURE_DIR/tasks.md
- REVIEW = FEATURE_DIR/review.md

Abort with an error message if any required file is missing (instruct the user to run the missing prerequisite command).
For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot").

### 2. Load Specification Context

Load the following from spec artifacts:

**From spec.md:**
- Functional Requirements (FR-###) with acceptance criteria
- Success Criteria (SC-###)
- User Stories and edge cases
- Non-functional requirements (performance, security, accessibility)

**From plan.md:**
- Architecture decisions (patterns, layers, folder structure)
- Tech stack choices
- Data model and entity relationships
- Phase structure and component boundaries

**From tasks.md:**
- Task list with file paths and descriptions
- Implementation order and dependencies
- Phase grouping

**From constitution (`.specify/memory/constitution.md`):**
- All MUST/SHOULD normative statements
- Architecture principles and constraints

### 3. Discover Implementation Files

Use the plan's file structure and tasks.md file paths to identify all implementation files. Run:

```powershell
# Get all source files referenced in tasks.md or matching the plan's structure
Get-ChildItem -Recurse -File -Include *.cs,*.cshtml,*.css,*.js,*.ts,*.json,*.razor,*.yml,*.yaml | Where-Object { $_.FullName -notmatch '(\\bin\\|\\obj\\|\\node_modules\\|\\wwwroot\\lib\\|\\.specify\\)' } | Select-Object FullName
```

Cross-reference discovered files against tasks.md to identify:
- Files that SHOULD exist (referenced in tasks) but DON'T → `MISSING_IMPL`
- Files that exist but aren't referenced → note for manual check (do not auto-flag)

### 4. Review Passes

Process each implementation file through these review categories:

#### A. SPEC_VIOLATION — Code Doesn't Match a Functional Requirement
#### B. CONSTITUTION_VIOLATION — Breaks Architecture Principles
#### C. LOGIC_ERROR — Bug or Incorrect Behavior
#### D. SECURITY — Vulnerability
#### E. MISSING_IMPL — Specified but Not Implemented
#### F. CODE_QUALITY — SOLID Violation, Bad Naming, Wrong Patterns
#### G. TEST_GAP — Missing or Weak Test Coverage

### 5. Severity Assignment

- **CRITICAL**: Constitution MUST violation, security vulnerability, spec deviation that breaks core functionality
- **HIGH**: Logic error, missing validation, SOLID violation in core domain, missing critical tests
- **MEDIUM**: Code quality, naming, partial non-core implementation, missing edge case tests
- **LOW**: Style grounded in convention, minor optimization, documentation gap

### 6. Write Review Report

Write to `FEATURE_DIR/review.md` with YAML frontmatter including `iteration`, `status` (changes_requested | approved), `generated_at`, and `reviewed_files` count.

**Convergence**: `status: approved` when 0 CRITICAL + 0 HIGH findings remain.

### 7. Output Chat Summary

Concise summary with status, finding counts, and next step suggestion.
