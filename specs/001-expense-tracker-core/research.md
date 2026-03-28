# Research: Expense Tracker Core

**Feature**: 001-expense-tracker-core  
**Date**: 2026-03-28  
**Status**: Complete — all unknowns resolved

---

## R-001: JSON File-Based Persistence with Atomic Writes in .NET

**Context**: Constitution mandates structured JSON files as the default storage provider with atomic writes (write-to-temp then rename).

**Decision**: Implement a generic `JsonFileRepository<T>` base class that serializes entities to individual JSON collection files (one file per entity type, e.g., `badges.json`, `expenses.json`). Use `System.Text.Json` for serialization.

**Rationale**:
- `System.Text.Json` is built into .NET 10 — zero additional dependency.
- Atomic write via `File.WriteAllText` to a `.tmp` file followed by `File.Move` with overwrite ensures no partial writes.
- A file-level `SemaphoreSlim` per collection prevents concurrent write corruption in the single-user scenario.
- Each entity type gets its own JSON file in a configurable `DataDirectory` path from `appsettings.json`.

**Alternatives Considered**:
- **LiteDB**: Lightweight embedded DB, but adds a dependency and doesn't match the constitution's explicit "JSON files" requirement.
- **SQLite**: Good embedded option, but the constitution specifically calls for JSON as the default provider (SQLite/SQL Server is the future swap).
- **One file per entity instance**: Too many files, harder to query/filter; a single file per collection is more practical at this scale.

---

## R-002: MediatR Setup in .NET 10 Razor Pages

**Context**: Constitution requires CQRS pattern with an in-process mediator.

**Decision**: Use `MediatR` (latest stable for .NET 10) registered via `builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))`. Commands return `Result<T>` wrapper for success/failure. Queries return DTOs. PageModel classes inject `IMediator` and dispatch in `OnGet` / `OnPost` handlers.

**Rationale**:
- MediatR is the de-facto .NET CQRS mediator with excellent Razor Pages compatibility.
- Single assembly scan is sufficient for a single-project modular monolith.
- Pipeline behaviors can be added later (validation, logging) without changing handlers.

**Alternatives Considered**:
- **Custom mediator**: More control but reinvents the wheel for no benefit at this scale.
- **Direct service injection**: Simpler but violates the constitution's CQRS mediator requirement.

---

## R-003: ASP.NET Core Localization — Arabic RTL + English LTR

**Context**: Constitution requires Arabic + English support and the spec requires Arabic RTL as primary UI language.

**Decision**: Use ASP.NET Core request localization with `IStringLocalizer<T>` per PageModel and `IViewLocalizer` in Razor views. Resource files (`.resx`) organized per page. Language persisted in `.AspNetCore.Culture` cookie. Layout switches `dir="rtl"` and `lang="ar"` on `<html>` based on active culture. Bootstrap RTL bundle (`bootstrap.rtl.min.css`) loaded conditionally when Arabic is active.

**Rationale**:
- Built-in ASP.NET Core localization is the standard approach — well-documented, no extra dependencies.
- Bootstrap 5 RTL bundle is already present in the project's `wwwroot/lib/bootstrap/dist/css/`.
- CSS logical properties (`margin-inline-start`, `padding-inline-end`) in custom CSS ensure styles flip without duplication.
- Cookie-based persistence is straightforward for Razor Pages (form POST to set culture, redirect).

**Alternatives Considered**:
- **URL-based culture (e.g., `/ar/badges`)**: More complex routing; cookie approach is simpler and sufficient for single-user.
- **JavaScript-only i18n**: Doesn't work with server-rendered Razor Pages; server localization is required.

---

## R-004: PWA — manifest.json and Service Worker

**Context**: Constitution requires the app be a Progressive Web App with manifest.json and service worker.

**Decision**: Create `wwwroot/manifest.json` with app name (Arabic + English), icons (192×192, 512×512), `"display": "standalone"`, theme color. Service worker uses a cache-first strategy for static assets (CSS, JS, icons) and network-first for page requests. Register the service worker from `_Layout.cshtml`.

**Rationale**:
- Minimal approach per constitution: installable, standalone display, offline-capable for static assets.
- Cache-first for static assets provides instant loads; network-first for pages ensures fresh data.
- No heavy PWA library needed — vanilla JS service worker is sufficient.

**Alternatives Considered**:
- **Workbox (Google)**: Powerful but adds complexity and a build step; vanilla service worker is adequate for this scope.
- **Blazor PWA template**: Different tech stack; not applicable to Razor Pages.

---

## R-005: Dark / Light Theme with Bootstrap 5

**Context**: Constitution requires dark and light themes with system preference + manual override.

**Decision**: Use Bootstrap 5.3+ `data-bs-theme` attribute on `<html>`. Default to system preference via `matchMedia('(prefers-color-scheme: dark)')`. Store user override in `localStorage`. Theme toggle button in the navbar switches `data-bs-theme` between `"light"` and `"dark"` with no page reload. Custom CSS variables for app-specific colors respect the active theme.

**Rationale**:
- Bootstrap 5.3+ has built-in dark mode via `data-bs-theme` — no custom dark stylesheet needed.
- `localStorage` persists the choice across sessions without server involvement.
- Instant switch (JS attribute toggle) meets the constitution's "no page reload" requirement.

**Alternatives Considered**:
- **Separate dark.css**: Maintenance burden, duplication; `data-bs-theme` is cleaner.
- **Server-side theme cookie**: Unnecessary round-trip; client-only is faster.

---

## R-006: Inline Badge Toggle for Quick Entry

**Context**: Spec FR-021 requires badge selection via inline toggle (no separate dialog). Spec requires quick entry in < 20 seconds.

**Decision**: Render all active (non-soft-deleted) badges as colored pill buttons on the expense form. Clicking a pill toggles it on/off (CSS class toggle + hidden checkbox/field update). Selected badges are submitted as a comma-separated list of IDs or as multiple hidden inputs. No AJAX required — pure client-side toggle.

**Rationale**:
- Pill-style buttons with badge colors provide instant visual feedback.
- No dialog or dropdown = fastest interaction for tagging.
- Works without JavaScript for accessibility (fallback: multi-select), enhanced with JS.
- Aligns with quick-entry timing goal (< 20 seconds for a basic expense).

**Alternatives Considered**:
- **Multi-select dropdown**: Slower interaction, less visual.
- **Modal dialog**: Explicitly prohibited by FR-021.
- **Autocomplete/search**: Overkill for a small badge list.

---

## R-007: Form Reset After Save for Quick Entry

**Context**: Spec FR-019 requires form reset after successful save, ready for next entry.

**Decision**: After a successful POST to create an expense, redirect back to the Create page with a `?saved=true` query parameter. The Create PageModel detects this and sets a `TempData` success message. The form loads fresh with defaults (current date, Normal importance, Pending unchecked). This follows the PRG (Post-Redirect-Get) pattern standard in Razor Pages.

**Rationale**:
- PRG prevents duplicate form submissions on browser refresh.
- Redirect to the same Create page (not to the list) keeps the user in entry mode.
- `TempData` flash message confirms the save without disrupting the flow.

**Alternatives Considered**:
- **AJAX submission + JS form reset**: Faster but more complex; PRG is the Razor Pages standard and works without JS.
- **Redirect to expense list**: Breaks quick-entry flow; user has to navigate back.

---

## R-008: Soft-Delete for Badges

**Context**: Spec FR-004 requires soft-delete for badges that have expenses attached.

**Decision**: `Badge` entity has an `IsDeleted` boolean (default `false`). Delete command checks if any expenses reference the badge. If yes: set `IsDeleted = true` after user confirmation. If no: physical delete is acceptable, but soft-delete is used uniformly for simplicity. All badge queries filter `IsDeleted == false` by default. Expense display still shows soft-deleted badge names/colors from the stored reference.

**Rationale**:
- Uniform soft-delete simplifies the repository logic (always update, never remove).
- Expenses store `BadgeId` references; the badge record must persist so historical data retains its name/color.
- Query-level filtering is simple and reliable.

**Alternatives Considered**:
- **Physical delete + snapshot badge data on expense**: Requires copying badge name/color to each expense; more complex, denormalized.
- **Physical delete only (no history)**: Doesn't meet the spec requirement to preserve badge info on historical expenses.
