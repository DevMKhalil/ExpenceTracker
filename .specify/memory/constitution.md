<!--
  Sync Impact Report
  ==================
  Version change: 1.1.0 → 1.2.0 (MINOR — new localization
  requirement added to Principle II and Tech Stack)
  Modified principles:
    - Principle II: added Localization & Bi-Directional Layout
      section (Arabic + English, RTL/LTR, language switcher)
  Added sections: None (expanded within existing sections)
  Removed sections: None
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ aligned
    - .specify/templates/spec-template.md ✅ aligned
    - .specify/templates/tasks-template.md ✅ aligned
  Follow-up TODOs: None
-->

# ExpenseTracker Constitution

## Core Principles

### I. Clean Architecture & Code Quality

The application MUST be structured as a **Modular Monolith**.
Each business capability (e.g., Expenses, Categories, Budgets,
Reports) MUST be isolated into its own module with clear
boundaries:

- **Module Isolation**: Every module MUST be a separate folder
  (or class-library project) containing its own Domain,
  Application, and Infrastructure layers. Modules MUST NOT
  reference each other's internal types directly.
- **Inter-Module Communication**: Modules MUST communicate only
  through well-defined public contracts — shared interfaces,
  domain events, or an in-process mediator. No module may
  directly call another module's repositories or services.
- **Independent Deployability (future)**: Module boundaries MUST
  be clean enough that extracting a module into a separate
  microservice requires no changes to other modules — only
  swapping the in-process mediator call for an HTTP/message
  bus call.
- **Shared Kernel**: Cross-cutting types used by multiple modules
  (e.g., `UserId`, `Money` value objects, base `Entity` class)
  MUST live in a small Shared Kernel project. The Shared Kernel
  MUST remain minimal — if a type is used by only one module,
  it belongs in that module.

Within each module, code MUST follow Clean Architecture with
strict layer separation:

- **Domain Layer**: Pure C# classes, value objects, domain events,
  and aggregates. Zero framework dependencies. All business rules
  live here. Domain-Driven Design (DDD) tactical patterns
  (Entities, Value Objects, Aggregates, Domain Events) MUST be
  applied where complexity warrants them.
- **Application Layer**: Use cases orchestrated via CQRS — Commands
  for writes, Queries for reads. Each use case MUST be a single
  class with a single public method. MediatR or a lightweight
  in-process mediator MUST dispatch commands and queries.
- **Infrastructure Layer**: All external concerns (persistence,
  file I/O, external services) MUST be implemented here behind
  interfaces defined in the Domain or Application layer.
- **Presentation Layer**: ASP.NET Core Razor Pages (.NET 10).
  Pages MUST be thin — delegating to Application layer handlers.
  No business logic in PageModel classes.

SOLID principles are NON-NEGOTIABLE:

- **S**: Each class MUST have one reason to change.
- **O**: Extend behavior via new classes, not by modifying existing
  ones.
- **L**: Subtypes MUST be substitutable for their base types.
- **I**: Clients MUST NOT depend on methods they do not use.
  Prefer small, focused interfaces.
- **D**: High-level modules MUST NOT depend on low-level modules.
  Both MUST depend on abstractions.

OOP best practices MUST be followed: favor composition over
inheritance, encapsulate state, use meaningful names, keep methods
short and focused. Clean Code principles apply — no dead code, no
magic numbers, no deep nesting, descriptive naming throughout.

The application MUST be a Progressive Web App (PWA) with a valid
`manifest.json` and a service worker for offline-capable caching.

### II. UI/UX Excellence

The user interface MUST be simple, attractive, and easy to use.
Every UI decision MUST prioritize user experience:

- **Mobile-First**: All layouts MUST be responsive and optimized
  for mobile devices first, then scale up to desktop. Touch
  targets MUST meet minimum 44×44px sizing.
- **Mobile via PWA (minimum tech)**: The PWA approach is the ONLY
  mobile strategy — no native apps, no Xamarin/MAUI, no Blazor
  Hybrid, no React Native. The PWA MUST deliver a native-like
  experience on mobile: home-screen installable, full-screen
  mode (`"display": "standalone"` in manifest), splash screen,
  app icon, and offline support via service worker caching.
  This keeps the technology footprint to a single ASP.NET Core
  Razor Pages codebase serving all platforms.
- **Theme Support**: The application MUST support both dark and
  light themes. Theme preference MUST respect the user's system
  setting (`prefers-color-scheme`) and allow manual override.
  Theme switching MUST be instant with no page reload.
- **Motion & Animation**: Meaningful CSS transitions and
  animations MUST be used for state changes, page transitions,
  and micro-interactions (button feedback, loading states, card
  entries). Animations MUST respect `prefers-reduced-motion`.
- **Simplicity**: UI MUST avoid clutter. One primary action per
  screen. Progressive disclosure for advanced options. Clear
  visual hierarchy using typography, spacing, and color.
- **Accessibility**: WCAG 2.1 AA compliance MUST be the minimum
  target. Semantic HTML, proper ARIA labels, keyboard navigation,
  and sufficient color contrast are required.
- **Best Practices**: Follow established UX patterns — consistent
  navigation, clear feedback for user actions, optimistic UI
  updates where safe, skeleton loaders over spinners, and
  meaningful empty states.
- **Localization & Bi-Directional Layout**: The application MUST
  support two languages: **Arabic (ar)** and **English (en)**.
  The user MUST be able to switch between them at any time via
  a visible language toggle in the UI. Requirements:
  - ASP.NET Core request localization middleware MUST be used
    with `IStringLocalizer<T>` / `IViewLocalizer` for all
    user-facing strings. No hardcoded display text in Razor
    pages or PageModel classes.
  - Resource files (`.resx`) MUST be organized per module
    (e.g., `Resources/Pages/Expenses/Index.ar.resx`).
  - When Arabic is active, the entire layout MUST switch to
    **RTL** (`dir="rtl"` on `<html>`). CSS MUST use logical
    properties (`margin-inline-start` instead of `margin-left`)
    so layouts flip correctly without duplicate stylesheets.
  - Bootstrap 5 RTL bundle MUST be loaded when `dir="rtl"` is
    active.
  - The selected language MUST be persisted in a cookie
    (`.AspNetCore.Culture`) so it survives page reloads and
    sessions.
  - Date, number, and currency formatting MUST respect the
    active culture (`CultureInfo`).
  - The language toggle MUST switch instantly without a full
    page reload where feasible (a form POST to set the cookie
    followed by redirect is acceptable for Razor Pages).

### III. Flexible Data Persistence

Data MUST be stored in local JSON files for development simplicity,
but the persistence layer MUST be structured so that switching to a
relational database (specifically SQL Server via EF Core) requires
only configuration changes and a new `IRepository` implementation:

- **Repository Pattern**: All data access MUST go through
  repository interfaces defined in the Domain layer
  (e.g., `IExpenseRepository`, `ICategoryRepository`).
- **File-Based Provider**: The default implementation MUST
  serialize/deserialize entities to/from structured JSON files.
  File paths MUST be configurable. File operations MUST be
  atomic (write to temp file, then rename).
- **EF Core Ready**: Entity classes MUST be designed as valid
  EF Core entities (proper key properties, navigation properties
  where needed, no file-specific concerns leaking into the
  domain). When switching to SQL Server, only a new DbContext
  and EF Core repository implementation are needed.
- **Fluent API Mapping (mandatory)**: All entity-to-table mapping
  MUST use EF Core Fluent API via `IEntityTypeConfiguration<T>`
  classes — one configuration class per entity, co-located in
  the module's Infrastructure layer. Data annotations on domain
  entities are PROHIBITED (domain classes MUST remain persistence
  ignorant). Fluent configurations MUST define: primary keys,
  column types/lengths, required/optional fields, relationships,
  indexes, and value conversions. Even while the file-based
  provider is active, Fluent configurations MUST be maintained
  so that switching to EF Core requires zero mapping work.
- **Provider Switching**: The active persistence provider MUST be
  selectable via `appsettings.json` configuration. Dependency
  injection MUST wire the correct implementation at startup based
  on this setting.
- **Data Integrity**: Regardless of provider, all writes MUST be
  validated at the domain level before reaching the persistence
  layer. Unique constraints and referential integrity MUST be
  enforced in code (domain validation) so they hold for both
  file and database providers.

## Technology Stack & Constraints

- **Runtime**: .NET 10 (latest stable)
- **Framework**: ASP.NET Core Razor Pages
- **ORM (future)**: Entity Framework Core (SQL Server provider)
- **Storage (default)**: Structured JSON files in a configurable
  data directory
- **CSS**: Bootstrap 5 with custom theme variables for dark/light
  mode, supplemented by site-specific CSS
- **JavaScript**: Vanilla JS or minimal library for PWA service
  worker, theme toggling, and motion. No heavy SPA frameworks.
- **Localization**: ASP.NET Core request localization with
  `IStringLocalizer` / `IViewLocalizer`. Resource files (`.resx`)
  for Arabic (ar) and English (en). Bootstrap 5 RTL CSS bundle
  for Arabic layout.
- **Target Platform**: Cross-platform web (desktop + mobile
  browsers), installable as PWA
- **Performance**: Pages MUST load in under 2 seconds on 3G.
  Lighthouse PWA score MUST be 90+. No blocking JS in critical
  render path.
- **Security**: HTTPS enforced. Anti-forgery tokens on all forms.
  Input validation at both client and server. No raw SQL (when
  DB provider is active). Output encoding to prevent XSS.

## Development Workflow & Quality Gates

- **Feature Branches**: All work MUST happen on feature branches.
  Direct commits to `main` are prohibited.
- **Spec-First**: Every feature MUST have a specification
  (`spec.md`) and implementation plan (`plan.md`) before coding
  begins.
- **Constitution Compliance**: Every PR MUST be verified against
  this constitution. Violations MUST be resolved before merge.
- **Code Review**: All changes MUST be reviewed. Reviewers MUST
  check for SOLID violations, layer boundary crossings, and UI/UX
  regression.
- **Testing**: Unit tests for domain logic and application
  handlers. Integration tests for persistence providers. Manual
  UI testing against both themes on mobile and desktop viewports.
- **No Over-Engineering**: YAGNI applies. Do not add abstractions,
  patterns, or infrastructure for hypothetical future needs.
  Complexity MUST be justified by a current requirement.

## Governance

This constitution is the highest-authority document for the
ExpenseTracker project. All design decisions, code reviews, and
architectural choices MUST comply with these principles.

- **Amendments**: Any change to this constitution MUST be
  documented with a rationale, versioned, and reviewed. Breaking
  changes to principles require MAJOR version bump.
- **Versioning**: Constitution follows semantic versioning
  (MAJOR.MINOR.PATCH). MAJOR = principle removal/redefinition,
  MINOR = new principle or material expansion, PATCH = wording
  or clarification.
- **Compliance Review**: At the start of each feature planning
  phase, the Constitution Check in `plan.md` MUST verify
  alignment with all active principles.
- **Conflict Resolution**: When a principle conflicts with a
  practical constraint, document the conflict, the chosen
  resolution, and the justification in the feature's plan.

**Version**: 1.2.0 | **Ratified**: 2026-03-27 | **Last Amended**: 2026-03-27
