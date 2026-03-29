# Implementation Plan: Expense Tracker Core

**Branch**: `001-expense-tracker-core` | **Date**: 2026-03-28 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-expense-tracker-core/spec.md`

**User Constraint**: No authentication implementation.

## Summary

Build a full-featured Expense Tracker web application using ASP.NET Core Razor Pages (.NET 10) with a Modular Monolith architecture. Core capabilities include badge (category) CRUD with color coding, expense entry with multi-badge tagging / importance levels / pending status, a dashboard with daily / monthly / badge breakdowns, and a quick-entry optimized UI. The application uses JSON file-based persistence (EF Core ready for future swap to SQL Server), supports Arabic and English localization with RTL layout, dark/light themes, and is delivered as a Progressive Web App (PWA).

## Technical Context

**Language/Version**: C# / .NET 10  
**Primary Dependencies**: ASP.NET Core Razor Pages, Bootstrap 5 (LTR + RTL bundles already in project), MediatR  
**Storage**: Structured JSON files (default provider); EF Core + SQL Server ready (future swap via config)  
**Testing**: xUnit, AngleSharp (Razor Pages integration tests)  
**Target Platform**: Cross-platform web (desktop + mobile browsers), installable PWA  
**Project Type**: Web application — ASP.NET Core Razor Pages, Modular Monolith  
**Performance Goals**: CRUD operations < 2 s, dashboard render < 2 s  
**Constraints**: Offline-capable (PWA service worker), RTL Arabic layout, no authentication, single user  
**Scale/Scope**: Single user, 2 modules (Badges, Expenses), ~6 page groups, 2 core entities

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| # | Principle | Requirement | Status | Notes |
|---|-----------|-------------|--------|-------|
| 1 | I — Modular Monolith | Modules in separate folders with Domain / Application / Infrastructure layers | ✅ PASS | Badges module + Expenses module as folders under `Modules/` |
| 2 | I — Clean Architecture | Strict layer separation: Domain → Application → Infrastructure → Presentation | ✅ PASS | Each module contains its own three layers; Pages are thin |
| 3 | I — CQRS | Commands for writes, Queries for reads via in-process mediator | ✅ PASS | MediatR dispatches all use cases |
| 4 | I — SOLID | SRP, OCP, LSP, ISP, DIP | ✅ PASS | Small focused interfaces in Domain, implementations in Infrastructure |
| 5 | I — PWA | manifest.json + service worker for offline caching | ✅ PASS | Will implement manifest, service worker, icons |
| 6 | II — Mobile-First | Responsive, touch-friendly, ≥ 44×44 px targets | ✅ PASS | Bootstrap 5 responsive grid + custom touch sizing |
| 7 | II — Theme Support | Dark / light with system preference + manual override | ✅ PASS | CSS custom properties + `data-bs-theme` attribute |
| 8 | II — Localization | Arabic + English, RTL / LTR, IStringLocalizer, .resx | ✅ PASS | Request localization middleware, Bootstrap RTL bundle |
| 9 | II — Accessibility | WCAG 2.1 AA, semantic HTML, ARIA, keyboard nav | ✅ PASS | Semantic markup, ARIA labels, contrast ratios |
| 10 | III — Repository Pattern | IRepository in Domain, implementations in Infrastructure | ✅ PASS | JSON file provider as default implementation |
| 11 | III — JSON File Storage | Atomic writes (temp + rename), configurable paths | ✅ PASS | `FileAtomicWriter` helper, path from `appsettings.json` |
| 12 | III — EF Core Ready | Fluent API `IEntityTypeConfiguration<T>` maintained | ✅ PASS | Configuration classes co-located in each module's Infrastructure |
| 13 | III — Provider Switching | Selectable via `Persistence:Provider` in `appsettings.json`; startup reads config and conditionally registers JSON or EF provider | ✅ PASS | Startup reads config key to register JSON or EF provider |
| 14 | — Auth excluded | User explicitly excluded authentication | ✅ PASS | Spec confirms single-user; no auth middleware needed |

All gates pass. Proceeding to Phase 0.

## Project Structure

### Documentation (this feature)

```text
specs/001-expense-tracker-core/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (page routes & AJAX endpoints)
│   └── routes.md
└── tasks.md             # Phase 2 output (NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
ExpenceTracker/
├── Program.cs                                  # Entry point, DI, middleware pipeline
├── ExpenceTracker.csproj                       # NuGet refs: MediatR, localization
├── appsettings.json                            # Persistence provider config, data dir
│
├── Shared/                                     # ── Shared Kernel ──
│   ├── Domain/
│   │   └── Entity.cs                           # Base entity (Guid Id, CreatedAt, UpdatedAt)
│   └── Infrastructure/
│       ├── JsonFileRepository.cs               # Generic JSON file repository base
│       └── FileAtomicWriter.cs                 # Atomic temp-file-then-rename writer
│
├── Modules/
│   ├── Badges/                                 # ── Badges Module ──
│   │   ├── Domain/
│   │   │   ├── Badge.cs                        # Entity: Name, Color, IsDeleted
│   │   │   └── IBadgeRepository.cs             # Repository interface
│   │   ├── Application/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateBadgeCommand.cs
│   │   │   │   ├── UpdateBadgeCommand.cs
│   │   │   │   └── DeleteBadgeCommand.cs       # Soft-delete logic
│   │   │   └── Queries/
│   │   │       ├── GetAllBadgesQuery.cs
│   │   │       ├── GetAllBadgesIncludingDeletedQuery.cs  # Returns all badges (active + soft-deleted) for historical display
│   │   │       └── GetBadgeByIdQuery.cs
│   │   └── Infrastructure/
│   │       ├── JsonBadgeRepository.cs           # JSON file implementation
│   │       └── BadgeEntityConfiguration.cs      # EF Core Fluent API (maintained)
│   │
│   └── Expenses/                               # ── Expenses Module ──
│       ├── Domain/
│       │   ├── Expense.cs                      # Entity: Name, Amount, Date, Importance, Notes, IsPending
│       │   ├── ExpenseBadge.cs                 # Join entity: ExpenseId + BadgeId
│       │   ├── ImportanceLevel.cs              # Enum: Normal, Important, VeryImportant
│       │   └── IExpenseRepository.cs           # Repository interface
│       ├── Application/
│       │   ├── Commands/
│       │   │   ├── CreateExpenseCommand.cs
│       │   │   ├── UpdateExpenseCommand.cs
│       │   │   └── DeleteExpenseCommand.cs
│       │   ├── Queries/
│       │       ├── GetAllExpensesQuery.cs       # Uses MediatR to dispatch GetAllBadgesIncludingDeletedQuery (no direct IBadgeRepository)
│       │       ├── GetExpenseByIdQuery.cs       # Uses MediatR to dispatch GetBadgeByIdQuery (no direct IBadgeRepository)
│       │       ├── GetDailySummaryQuery.cs
│       │       ├── GetMonthlySummaryQuery.cs
│       │       ├── GetBadgeSummaryQuery.cs      # Uses MediatR to dispatch GetAllBadgesIncludingDeletedQuery (no direct IBadgeRepository)
│       │       └── GetPendingTotalQuery.cs      # Sums all pending expenses for the dashboard pending total
│       │   └── DTOs/
│       │       ├── ExpenseDto.cs
│       │       └── DashboardSummaryDto.cs
│       └── Infrastructure/
│           ├── JsonExpenseRepository.cs         # JSON file implementation
│           └── ExpenseEntityConfiguration.cs    # EF Core Fluent API (maintained)
│
├── Pages/                                      # ── Presentation Layer ──
│   ├── _ViewImports.cshtml                     # Tag helpers, localizer injection
│   ├── _ViewStart.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml                      # Main layout: RTL/LTR, theme, nav, PWA
│   │   ├── _Layout.cshtml.css                  # Scoped layout styles
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Index.cshtml / .cshtml.cs               # Dashboard (daily, monthly, badge breakdown)
│   ├── Badges/
│   │   ├── Index.cshtml / .cshtml.cs           # Badge list
│   │   ├── Create.cshtml / .cshtml.cs          # Create badge
│   │   └── Edit.cshtml / .cshtml.cs            # Edit badge
│   └── Expenses/
│       ├── Index.cshtml / .cshtml.cs           # Expense list with filters
│       ├── Create.cshtml / .cshtml.cs          # Quick-entry form
│       └── Edit.cshtml / .cshtml.cs            # Edit expense
│
├── Resources/                                  # ── Localization ──
│   ├── Pages/
│   │   ├── IndexModel.ar.resx / .en.resx
│   │   ├── Badges/
│   │   │   ├── IndexModel.ar.resx / .en.resx
│   │   │   ├── CreateModel.ar.resx / .en.resx
│   │   │   └── EditModel.ar.resx / .en.resx
│   │   └── Expenses/
│   │       ├── IndexModel.ar.resx / .en.resx
│   │       ├── CreateModel.ar.resx / .en.resx
│   │       └── EditModel.ar.resx / .en.resx
│   └── SharedResource.ar.resx / .en.resx      # Shared labels (nav, buttons, validation)
│
└── wwwroot/
    ├── manifest.json                           # PWA manifest
    ├── service-worker.js                       # Offline caching strategy
    ├── css/
    │   └── site.css                            # Theme variables, RTL logical props, custom styles
    ├── js/
    │   └── site.js                             # Theme toggle, badge toggle, form helpers
    ├── icons/                                  # PWA icons (192×192, 512×512)
    └── lib/
        └── bootstrap/dist/                     # Bootstrap 5 LTR + RTL (already present)
```

**Structure Decision**: Single ASP.NET Core Razor Pages project with folder-based Modular Monolith. Modules (`Badges`, `Expenses`) are isolated as folders under `Modules/`, each with their own Domain / Application / Infrastructure layers. Presentation (`Pages/`) stays at the project root following Razor Pages conventions. Shared Kernel lives in `Shared/`. **Cross-module communication**: The Expenses module MUST NOT directly inject `IBadgeRepository`. Instead, it dispatches `GetAllBadgesQuery` / `GetAllBadgesIncludingDeletedQuery` / `GetBadgeByIdQuery` via MediatR to the Badges module, preserving module boundaries. Use `GetAllBadgesIncludingDeletedQuery` when historical (soft-deleted) badge data is needed (e.g., expense lists, dashboard badge breakdown). This is appropriate because the scope (2 modules, single-user) does not warrant separate class-library projects while still maintaining clean module boundaries per the constitution.

## Skeleton Loader Strategy

**Approach**: CSS-only placeholder shimmer during the initial paint. Since the application uses server-rendered Razor Pages (not a client-side SPA), traditional JavaScript skeleton loaders are not applicable. Instead, the layout renders lightweight CSS shimmer placeholders in the initial HTML response. These placeholders are visible during the brief window before the server-rendered content is fully painted. The shimmer effect is achieved purely with CSS `@keyframes` and `linear-gradient` animations on placeholder `<div>` elements, requiring no JavaScript. This applies to the badge list (T031), expense list (T051), and dashboard (T058) pages.

## Complexity Tracking

No constitution violations requiring justification. All principles are met with the planned approach.
