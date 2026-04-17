---
name: speckit-review
description: Review implementation code against spec artifacts and constitution. Writes
  surgical findings to review.md for the fix agent to consume. Use after speckit.implement
  to validate code quality.
compatibility: Requires spec-kit project structure with .specify/ directory
metadata:
  author: github-spec-kit
  source: custom
---

# Speckit Review Skill

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

Process each implementation file through these review categories. Be thorough and unforgiving — assume the fixing model needs maximum detail.

#### A. SPEC_VIOLATION — Code Doesn't Match a Functional Requirement

For each FR-### in spec.md, verify the implementation satisfies it:
- Is the feature actually implemented?
- Does the behavior match the acceptance criteria?
- Are all specified inputs, outputs, and states handled?

#### B. CONSTITUTION_VIOLATION — Breaks Architecture Principles

Check every implementation file against constitution MUST principles:
- Correct layer placement (e.g., no business logic in UI layer)
- Pattern compliance (e.g., CQRS, Repository, Mediator if mandated)
- Dependency direction (inner layers don't reference outer layers)
- Naming conventions and project structure rules

#### C. LOGIC_ERROR — Bug or Incorrect Behavior

- Off-by-one errors, null reference risks, race conditions
- Incorrect conditional logic, missing null checks
- Wrong data type conversions or comparisons
- Unreachable code, infinite loops, missing break statements

#### D. SECURITY — Vulnerability

- SQL injection, XSS, CSRF vulnerabilities
- Missing input validation or sanitization
- Hardcoded secrets or credentials
- Missing authorization checks
- Insecure deserialization or file handling

#### E. MISSING_IMPL — Specified but Not Implemented

- Tasks marked in tasks.md but corresponding code not found
- Partial implementations (function stub exists but body is empty/TODO)
- Missing error handling specified in requirements
- Missing validation rules from spec

#### F. CODE_QUALITY — SOLID Violation, Bad Naming, Wrong Patterns

- Single Responsibility violations (class doing too much)
- Open/Closed principle violations
- Incorrect use of patterns specified in plan.md
- Naming that doesn't match domain terminology from spec
- Dead code, unused imports, commented-out blocks

#### G. TEST_GAP — Missing or Weak Test Coverage

- Missing unit tests for business logic
- Missing integration tests for specified workflows
- Tests that don't actually assert meaningful behavior
- Missing edge case test coverage specified in spec

### 5. Severity Assignment

Use this heuristic:

- **CRITICAL**: Constitution MUST violation, security vulnerability, spec deviation that breaks core functionality, missing entire feature
- **HIGH**: Logic error causing incorrect behavior, missing validation, SOLID violation in core domain, missing tests for critical paths
- **MEDIUM**: Code quality issue, naming inconsistency, partial implementation of non-core feature, missing edge case tests
- **LOW**: Style issue grounded in constitution/plan conventions, minor optimization opportunity, documentation gap

### 6. Write Review Report

Write the full review report to `REVIEW` (i.e., `FEATURE_DIR/review.md`). The file MUST begin with a YAML frontmatter block:

```yaml
---
iteration: 1          # Increment if review.md already exists (read previous iteration number + 1)
status: changes_requested   # changes_requested | approved
generated_at: "2026-04-11T14:30:00Z"  # ISO 8601 UTC timestamp
reviewed_files: 12    # Total number of files reviewed
source_specs:
  spec.md: "2026-04-10T12:00:00Z"
  plan.md: "2026-04-10T12:15:00Z"
  tasks.md: "2026-04-10T13:00:00Z"
---
```

**Iteration tracking**: If `review.md` already exists, read it and extract the `iteration` number from frontmatter. Set the new iteration to `previous + 1`. Also read the previous findings to check which issues were resolved (DO NOT carry forward resolved findings).

**Status logic**:
- `changes_requested`: If ANY CRITICAL or HIGH severity findings exist
- `approved`: If 0 CRITICAL and 0 HIGH findings remain (MEDIUM/LOW are acceptable)

After the frontmatter, write the report body:

```markdown
# Implementation Review — Iteration {N}

## Summary

- **Files reviewed**: X
- **Total findings**: Y (Z CRITICAL, W HIGH, V MEDIUM, U LOW)
- **Status**: {changes_requested | approved}
- **Verdict**: {One-sentence assessment}

## Findings

### CRITICAL

#### R{N}.C1 — {Short title}

- **Category**: {SPEC_VIOLATION | CONSTITUTION_VIOLATION | SECURITY | ...}
- **File**: `path/to/file.cs` (Lines 45-52)
- **Reference**: FR-007 / Constitution Principle III / OWASP A03
- **Code**:
  ```csharp
  // The actual problematic code, quoted verbatim
  ```
- **Problem**: {Detailed explanation of why this is wrong}
- **Fix**:
  1. {Step 1}
  2. {Step 2}
  3. Expected result:
     ```csharp
     // The corrected code
     ```

### HIGH
...

### MEDIUM
...

### LOW
...

## Resolved from Previous Iteration
(Only present if iteration > 1)

| Previous ID | Title | Status |
|-------------|-------|--------|
| R1.C1 | ... | ✅ Resolved |

## Next Steps
{If changes_requested: "Run `/speckit.fix` to apply the fixes from this review, then run `/speckit.review` again to verify."}
{If approved: "Implementation meets all spec requirements. No further fixes needed."}
```

### 7. Output Chat Summary

After writing the file, output a concise summary in chat:

```
## Review Complete — Iteration {N}

- **Status**: {CHANGES REQUESTED ⚠️ | APPROVED ✅}
- **Files reviewed**: X
- **Findings**: Y total (Z CRITICAL, W HIGH, V MEDIUM, U LOW)
- **Report saved to**: `specs/{branch}/review.md`

{If changes_requested: "Run `/speckit.fix` to apply fixes, then `/speckit.review` to verify."}
{If approved: "Implementation is clean. No action needed."}
```

### 8. Offer Fix Step

Point the user to the saved report and suggest the next step:

> "The full review report has been saved to `specs/{branch}/review.md`. To automatically apply the fixes, run `/speckit.fix`. The fix agent will read the report directly from the file."

## Operating Principles

### Context Efficiency

- Use progressive disclosure — don't load every file at once
- Read files in batches, prioritizing files referenced in CRITICAL/HIGH tasks first
- Skip binary files, generated files, and third-party libraries

### Review Quality

- **Be ruthless**: You are the senior reviewer. Miss nothing. The fixing model depends on your precision.
- **Be specific**: "Line 47 is wrong" is useless. "Line 47 uses `string.Format` instead of parameterized query, violating FR-007" is actionable.
- **Quote code**: Always include the actual problematic snippet. The fix agent may not have your context window.
- **Provide the fix**: Don't just describe what's wrong — show exactly what the code should look like.
- **Reference specs**: Every finding must trace to a FR-###, SC-###, constitution principle, or established best practice. No personal opinions.

### Boundaries

- **NEVER** modify source code — only write to review.md
- **NEVER** modify spec artifacts (spec.md, plan.md, tasks.md)
- **NEVER** flag style preferences not grounded in constitution or plan
- **NEVER** suggest architecture changes — flag violations against the existing architecture
