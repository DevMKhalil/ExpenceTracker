# ExpenceTracker Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-03-28

## Active Technologies

- C# / .NET 10 + ASP.NET Core Razor Pages, Bootstrap 5 (LTR + RTL bundles already in project), MediatR (001-expense-tracker-core)

## Project Structure

```text
backend/
frontend/
tests/
```

## Commands

# Add commands for C# / .NET 10

## Code Style

C# / .NET 10: Follow standard conventions

## Recent Changes

- 001-expense-tracker-core: Added C# / .NET 10 + ASP.NET Core Razor Pages, Bootstrap 5 (LTR + RTL bundles already in project), MediatR

<!-- MANUAL ADDITIONS START -->

## SpecKit Pipeline

The project uses a spec-driven development pipeline with these commands:

1. `/speckit.specify` → Creates `spec.md`
2. `/speckit.plan` → Creates `plan.md`
3. `/speckit.tasks` → Creates `tasks.md`
4. `/speckit.implement` → Executes tasks from `tasks.md`
5. `/speckit.analyze` → Reviews spec artifacts, writes `analysis.md`
6. `/speckit.remediate` → Reads `analysis.md`, fixes spec artifacts
7. `/speckit.review` → Reviews implementation code against specs, writes `review.md`
8. `/speckit.fix` → Reads `review.md`, applies code fixes

**Review/Fix cycle**: Run `/speckit.review` (senior model) → `/speckit.fix` (junior model) → repeat until `review.md` shows `status: approved`.

<!-- MANUAL ADDITIONS END -->
