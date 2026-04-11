# Tasks: Expense Tracker Core

**Input**: Design documents from `/specs/001-expense-tracker-core/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/routes.md ✅, quickstart.md ✅

**Tests**: Not requested — no test tasks included.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

**Implementation Note**: This task list is designed for step-by-step execution by a less experienced model. Each task includes exact file paths and specific instructions. Follow tasks in order within each phase. Tasks marked `[P]` can be done in parallel within the same phase.

**Android Offline Note**: This application is a Progressive Web App (PWA). The PWA setup (manifest, service worker, icons) is included in the Foundational phase so the app is installable on Android and works offline from the start.

## Format: `[ID] [P?] [Story?] Description`

> **Note**: Task T005 was intentionally removed (YAGNI).

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Create the folder structure and install NuGet packages so all later tasks have a place to go.

- [x] T001 Create the full folder structure under `ExpenceTracker/` by creating these empty directories: `Shared/Domain/`, `Shared/Infrastructure/`, `Modules/Badges/Domain/`, `Modules/Badges/Application/Commands/`, `Modules/Badges/Application/Queries/`, `Modules/Badges/Infrastructure/`, `Modules/Expenses/Domain/`, `Modules/Expenses/Application/Commands/`, `Modules/Expenses/Application/Queries/`, `Modules/Expenses/Application/DTOs/`, `Modules/Expenses/Infrastructure/`, `Pages/Badges/`, `Pages/Expenses/`, `Resources/Pages/Badges/`, `Resources/Pages/Expenses/`, `Resources/`, `wwwroot/icons/`
- [x] T002 Add NuGet package `MediatR` (latest stable) to `ExpenceTracker/ExpenceTracker.csproj` by running `dotnet add package MediatR` from the `ExpenceTracker/` folder
- [x] T003 Update `ExpenceTracker/appsettings.json` — add a `"Persistence"` section with `"Provider": "JsonFile"` and `"DataDirectory": "data"` so the JSON file repository knows where to store data files. Keep existing content and just merge the new section.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core shared infrastructure that MUST be complete before ANY user story can be implemented. This includes the base entity, JSON file persistence, MediatR wiring, localization, layout, theme, and PWA setup for Android offline.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

### Shared Domain Layer

- [x] T004 [P] Create `ExpenceTracker/Shared/Domain/Entity.cs` — abstract base class with properties: `Guid Id` (initialized to `Guid.NewGuid()`), `DateTimeOffset CreatedAt` (initialized to `DateTimeOffset.UtcNow`), `DateTimeOffset UpdatedAt` (initialized to `DateTimeOffset.UtcNow`). The class should be `public abstract class Entity` in namespace `ExpenceTracker.Shared.Domain`.

### Shared Infrastructure Layer (JSON File Persistence)

- [x] T006 Create `ExpenceTracker/Shared/Infrastructure/FileAtomicWriter.cs` — a static helper class in namespace `ExpenceTracker.Shared.Infrastructure`. It has one public static async method: `WriteAllTextAsync(string filePath, string content)`. This method writes `content` to a temporary file (`filePath + ".tmp"`), then moves (renames) it to `filePath` with overwrite enabled using `File.Move(tempPath, filePath, overwrite: true)`. This prevents data corruption from partial writes. Wrap in try/finally to delete the temp file if the move fails.
- [x] T007 Create `ExpenceTracker/Shared/Infrastructure/JsonFileRepository.cs` — a generic abstract base class `JsonFileRepository<T>` in namespace `ExpenceTracker.Shared.Infrastructure` where `T : Entity`. Constructor takes `string filePath` (the full path to the JSON file, e.g., `data/badges.json`). Uses a private `SemaphoreSlim(1,1)` for thread safety. Implements these protected async methods: (1) `ReadAllAsync()` — if file does not exist, return empty `List<T>`. Otherwise read the file with `File.ReadAllTextAsync`, deserialize with `System.Text.Json.JsonSerializer.Deserialize<List<T>>` using `PropertyNameCaseInsensitive = true`. (2) `WriteAllAsync(List<T> items)` — serialize with `System.Text.Json.JsonSerializer.Serialize` using `WriteIndented = true`, then call `FileAtomicWriter.WriteAllTextAsync`. (3) Ensure the directory of `filePath` exists in the constructor using `Directory.CreateDirectory(Path.GetDirectoryName(filePath))`.

### Program.cs Configuration (MediatR + Localization + Persistence)

- [x] T008 Update `ExpenceTracker/Program.cs` — Add MediatR registration: `builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));`. Add localization services: `builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");`. Configure request localization with supported cultures `"ar"` (Arabic) and `"en"` (English), default culture `"ar"`. Add `builder.Services.Configure<RequestLocalizationOptions>(...)` setting `DefaultRequestCulture = new RequestCulture("ar")`, `SupportedCultures` and `SupportedUICultures` to both `ar` and `en`. Add `app.UseRequestLocalization()` before `app.UseRouting()`. Read the `Persistence:DataDirectory` config value with a fallback of `"data"` and store it for DI registration (used in Phase 3+). Also add `builder.Services.AddRazorPages().AddViewLocalization().AddDataAnnotationsLocalization();` to replace the existing `AddRazorPages()` call.

### Localization Resources (Shared)

- [x] T009 [P] Create `ExpenceTracker/Resources/SharedResource.cs` — an empty marker class: `public class SharedResource { }` in namespace `ExpenceTracker.Resources`. This is needed by `IStringLocalizer<SharedResource>`.
- [x] T010 [P] Create `ExpenceTracker/Resources/SharedResource.ar.resx` — Arabic shared labels. Include keys: `AppTitle` = `متتبع المصروفات`, `Dashboard` = `لوحة القيادة`, `Badges` = `الشارات`, `Expenses` = `المصروفات`, `Save` = `حفظ`, `Cancel` = `إلغاء`, `Edit` = `تعديل`, `Delete` = `حذف`, `Create` = `إضافة`, `Back` = `رجوع`, `Name` = `الاسم`, `Color` = `اللون`, `Amount` = `المبلغ`, `Date` = `التاريخ`, `Notes` = `ملاحظات`, `Pending` = `معلق`, `Importance` = `الأهمية`, `Normal` = `عادي`, `Important` = `مهم`, `VeryImportant` = `مهم جدًا`, `Actions` = `إجراءات`, `Confirm` = `تأكيد`, `DeleteConfirm` = `هل أنت متأكد من الحذف؟`, `NoData` = `لا توجد بيانات`, `DailyTotal` = `إجمالي اليوم`, `MonthlyTotal` = `إجمالي الشهر`, `PendingTotal` = `إجمالي المعلق`, `ByBadge` = `حسب الشارة`, `RequiredField` = `هذا الحقل مطلوب`, `PositiveNumber` = `يجب أن يكون رقم موجب`, `ThemeLight` = `فاتح`, `ThemeDark` = `داكن`, `Language` = `اللغة`.
- [x] T011 [P] Create `ExpenceTracker/Resources/SharedResource.en.resx` — English shared labels. Include the same keys as T010 but with English values: `AppTitle` = `Expense Tracker`, `Dashboard` = `Dashboard`, `Badges` = `Badges`, `Expenses` = `Expenses`, `Save` = `Save`, `Cancel` = `Cancel`, `Edit` = `Edit`, `Delete` = `Delete`, `Create` = `Create`, `Back` = `Back`, `Name` = `Name`, `Color` = `Color`, `Amount` = `Amount`, `Date` = `Date`, `Notes` = `Notes`, `Pending` = `Pending`, `Importance` = `Importance`, `Normal` = `Normal`, `Important` = `Important`, `VeryImportant` = `Very Important`, `Actions` = `Actions`, `Confirm` = `Confirm`, `DeleteConfirm` = `Are you sure you want to delete?`, `NoData` = `No data available`, `DailyTotal` = `Today's Total`, `MonthlyTotal` = `Monthly Total`, `PendingTotal` = `Pending Total`, `ByBadge` = `By Badge`, `RequiredField` = `This field is required`, `PositiveNumber` = `Must be a positive number`, `ThemeLight` = `Light`, `ThemeDark` = `Dark`, `Language` = `Language`.

### Layout, Theme & Navigation

- [x] T012 Rewrite `ExpenceTracker/Pages/Shared/_Layout.cshtml` — Complete layout with: (1) Inject `IStringLocalizer<SharedResource>` and `IViewLocalizer`. (2) Set `<html lang="@CultureInfo.CurrentCulture.TwoLetterISOLanguageName" dir="@(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar" ? "rtl" : "ltr")" data-bs-theme="light">`. (3) In `<head>`: load Bootstrap 5 CSS conditionally — if Arabic load `bootstrap.rtl.min.css`, else `bootstrap.min.css` from `~/lib/bootstrap/dist/css/`. Then load `~/css/site.css`. Add `<link rel="manifest" href="/manifest.json">` for PWA. Add `<meta name="theme-color" content="#0d6efd">`. Add `<meta name="viewport" content="width=device-width, initial-scale=1.0">`. (4) In `<body>`: a Bootstrap 5 navbar with brand text from `@Localizer["AppTitle"]`, nav links to Dashboard (`/`), Badges (`/Badges`), Expenses (`/Expenses`). Include a theme toggle button (sun/moon icon) and a language toggle link. (5) A `<main>` container with `@RenderBody()`. (6) Before closing `</body>`: load jQuery, Bootstrap JS bundle, `~/js/site.js`, and register the service worker with `<script>if('serviceWorker' in navigator){navigator.serviceWorker.register('/service-worker.js');}</script>`. Use `@RenderSection("Scripts", required: false)` for page-specific scripts.
- [x] T013 Update `ExpenceTracker/Pages/Shared/_Layout.cshtml.css` — Add CSS custom properties for theme colors using `:root` and `[data-bs-theme="dark"]` selectors. Add styles for: (1) badge color pills (`.badge-pill` — inline-block, border-radius 1rem, padding 0.25em 0.75em, font-weight bold, cursor pointer). (2) Importance level indicators (`.importance-normal`, `.importance-important`, `.importance-very-important` with distinct colors). (3) Pending expense styling (`.expense-pending` — opacity 0.7, dashed left border). (4) Touch-friendly controls: minimum 44x44px touch targets. (5) Smooth card shadows and spacing for mobile. Use CSS logical properties (`margin-inline-start` instead of `margin-left`) for RTL compatibility.
- [x] T014 Update `ExpenceTracker/Pages/_ViewImports.cshtml` — Add: `@using ExpenceTracker`, `@using ExpenceTracker.Resources`, `@using Microsoft.AspNetCore.Mvc.Localization`, `@using Microsoft.Extensions.Localization`, `@using System.Globalization`, `@inject IViewLocalizer Localizer`, `@inject IStringLocalizer<SharedResource> SharedLocalizer`, `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`.
- [x] T015 Update `ExpenceTracker/wwwroot/css/site.css` — Add: (1) CSS custom properties for theme under `:root` — primary, secondary, success, danger, surface, text colors. (2) `[data-bs-theme="dark"]` overrides for all properties. (3) Global styles: `body { font-family: 'Segoe UI', Tahoma, sans-serif; }`. (4) RTL logical properties where needed. (5) Mobile-first responsive styles. (6) Badge pill toggle styles: `.badge-toggle { display: inline-block; padding: 6px 16px; border-radius: 20px; border: 2px solid; cursor: pointer; user-select: none; transition: all 0.2s; }` and `.badge-toggle.selected { color: white; }`. (7) Quick-entry form styles. Keep existing content and append new styles.
- [x] T016 Create `ExpenceTracker/wwwroot/js/site.js` — Replace contents with: (1) **Theme toggle**: On DOMContentLoaded, read `localStorage.getItem('theme')`. If null, detect system preference with `matchMedia('(prefers-color-scheme: dark)')`. Set `document.documentElement.setAttribute('data-bs-theme', theme)`. Bind click on `#theme-toggle` button to toggle between 'light' and 'dark', save to `localStorage`, update the button icon (☀️ for light, 🌙 for dark). (2) **Badge pill toggle**: Bind click on `.badge-toggle` elements — toggle `.selected` class, update a hidden input `#BadgeIds` with comma-separated IDs of all selected badges. Badge pills MUST be minimum 44x44px for touch targets (ensure via CSS `min-width: 44px; min-height: 44px;`). Add a brief scale-up animation (CSS `transform: scale(1.1)` for 150ms) when toggled. (3) **Delete confirmation**: Bind click on `.btn-delete` elements — show `confirm()` dialog with localized message, only submit the form if confirmed.

### PWA Setup (Android Offline Installability)

- [x] T017 [P] Create `ExpenceTracker/wwwroot/manifest.json` — PWA manifest for Android installability. Include: `"name": "متتبع المصروفات - Expense Tracker"`, `"short_name": "المصروفات"`, `"description": "Track your daily expenses"`, `"start_url": "/"`, `"display": "standalone"`, `"orientation": "portrait"`, `"background_color": "#ffffff"`, `"theme_color": "#0d6efd"`, `"lang": "ar"`, `"dir": "rtl"`, `"categories": ["finance", "utilities"]`, `"icons": [{"src": "/icons/icon-192x192.png", "sizes": "192x192", "type": "image/png", "purpose": "any maskable"}, {"src": "/icons/icon-512x512.png", "sizes": "512x512", "type": "image/png", "purpose": "any maskable"}]`.
- [x] T018 [P] Create `ExpenceTracker/wwwroot/service-worker.js` — Service worker for offline caching. Use a versioned cache name like `expense-tracker-v1`. In the `install` event: open the cache and add all critical assets to the cache: `/`, `/Badges`, `/Expenses`, `/css/site.css`, `/js/site.js`, `/manifest.json`, the Bootstrap CSS file, the Bootstrap JS bundle, jQuery. In the `activate` event: delete any old caches (cache names that don't match the current version). In the `fetch` event: for navigation requests (mode === 'navigate') use network-first strategy — try fetch, if it fails serve from cache, if cache misses serve an offline fallback page `/offline.html`. For static assets (css, js, images, fonts) use cache-first strategy — try cache, if miss fetch from network and store in cache. For all other requests use network-first with cache fallback.
- [x] T019 [P] Create `ExpenceTracker/Pages/Offline.cshtml` — a simple offline fallback Razor page with `@page "/offline"`. Display a centered message in Arabic: `أنت غير متصل بالإنترنت` (You are offline) with an icon and a retry button that calls `location.reload()`. Use minimal inline styles so it works without cached CSS. Also create `ExpenceTracker/Pages/Offline.cshtml.cs` as a minimal PageModel class.
- [x] T020 [P] Create placeholder PWA icons: `ExpenceTracker/wwwroot/icons/icon-192x192.png` and `ExpenceTracker/wwwroot/icons/icon-512x512.png` — Generate simple solid-color square PNG images (blue `#0d6efd` background with white "₪" or "$" symbol centered) at 192x192 and 512x512 pixels. These are required for Android "Add to Home Screen". If you cannot generate images, create a simple SVG icon at `ExpenceTracker/wwwroot/icons/icon.svg` and reference it in the manifest instead.

**Checkpoint**: Foundation ready — shared kernel, JSON persistence, MediatR, localization, layout, theme toggle, PWA (Android offline) are all in place. User story implementation can now begin.

---

## Phase 3: User Story 1 — Badge Management (Priority: P1) 🎯 MVP

**Goal**: Full CRUD for badges (categories) — each badge has a name and a unique user-chosen color. Users can create, view, edit, and soft-delete badges.

**Independent Test**: Navigate to `/Badges`, create a badge "Food" with green color, see it in the list, edit its name to "Groceries", then delete it. Verify soft-deleted badges don't appear in the list.

### Domain Layer

- [x] T021 [P] [US1] Create `ExpenceTracker/Modules/Badges/Domain/Badge.cs` — a class `Badge` extending `Entity` (from `Shared.Domain`). Namespace: `ExpenceTracker.Modules.Badges.Domain`. Properties: `string Name { get; set; }` (required, max 100 chars), `string Color { get; set; }` (required, hex color like `#FF5733`, exactly 7 chars), `bool IsDeleted { get; set; }` (default `false`). Add a parameterless constructor and a constructor that takes `string name, string color` and sets them.
- [x] T022 [P] [US1] Create `ExpenceTracker/Modules/Badges/Domain/IBadgeRepository.cs` — interface `IBadgeRepository` in namespace `ExpenceTracker.Modules.Badges.Domain`. Methods: `Task<List<Badge>> GetAllActiveAsync()` (returns non-deleted badges), `Task<List<Badge>> GetAllIncludingDeletedAsync()` (returns all badges), `Task<Badge?> GetByIdAsync(Guid id)`, `Task AddAsync(Badge badge)`, `Task UpdateAsync(Badge badge)`, `Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)` (case-insensitive check among non-deleted badges).

### Infrastructure Layer

- [x] T023 [US1] Create `ExpenceTracker/Modules/Badges/Infrastructure/JsonBadgeRepository.cs` — class `JsonBadgeRepository` extending `JsonFileRepository<Badge>` and implementing `IBadgeRepository`. Namespace: `ExpenceTracker.Modules.Badges.Infrastructure`. Constructor takes `IConfiguration configuration` and passes the JSON file path to base: `Path.Combine(configuration["Persistence:DataDirectory"] ?? "data", "badges.json")`. Implement all interface methods: `GetAllActiveAsync` filters `IsDeleted == false`; `GetByIdAsync` finds by ID; `AddAsync` reads all, adds new badge, writes all; `UpdateAsync` reads all, replaces the matching badge (set `UpdatedAt = DateTimeOffset.UtcNow`), writes all; `ExistsByNameAsync` checks if any non-deleted badge has the same name (case-insensitive), optionally excluding a specific ID.
- [x] T024 [P] [US1] Create `ExpenceTracker/Modules/Badges/Infrastructure/BadgeEntityConfiguration.cs` — EF Core `IEntityTypeConfiguration<Badge>` in namespace `ExpenceTracker.Modules.Badges.Infrastructure`. Copy the exact Fluent API from data-model.md: table name "Badges", key on Id, Name required max 100, Color required max 7, IsDeleted default false, CreatedAt/UpdatedAt required, unique index on Name filtered where IsDeleted = 0.

### Application Layer (Commands & Queries)

- [x] T025 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Commands/CreateBadgeCommand.cs` — contains: (1) A `record CreateBadgeCommand(string Name, string Color) : IRequest<Guid>`. (2) A handler class `CreateBadgeCommandHandler : IRequestHandler<CreateBadgeCommand, Guid>`. Handler injects `IBadgeRepository`. In `Handle`: validate that Name is not empty and Color matches `^#[0-9A-Fa-f]{6}$`, check `ExistsByNameAsync` to ensure unique name (throw if duplicate), create a new `Badge` with the name and color, call `AddAsync`, return the new badge's `Id`.
- [x] T026 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Commands/UpdateBadgeCommand.cs` — contains: (1) A `record UpdateBadgeCommand(Guid Id, string Name, string Color) : IRequest<bool>`. (2) Handler class. In `Handle`: get badge by ID (return false if not found), validate Name and Color same as create, check unique name excluding current ID, update badge properties, call `UpdateAsync`, return true.
- [x] T027 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Commands/DeleteBadgeCommand.cs` — contains: (1) A `record DeleteBadgeCommand(Guid Id) : IRequest<bool>`. (2) Handler class. In `Handle`: get badge by ID (return false if not found), set `IsDeleted = true`, call `UpdateAsync`, return true. This is always a soft-delete.
- [x] T028 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Queries/GetAllBadgesQuery.cs` — contains: (1) A `record GetAllBadgesQuery() : IRequest<List<Badge>>`. (2) Handler returns `repository.GetAllActiveAsync()`.
- [x] T073 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Queries/GetAllBadgesIncludingDeletedQuery.cs` — contains: (1) A `record GetAllBadgesIncludingDeletedQuery() : IRequest<List<Badge>>`. (2) Handler returns `repository.GetAllIncludingDeletedAsync()`. This query is used by the Expenses module (T049, T057) to fetch active + soft-deleted badges for historical display.
- [x] T029 [P] [US1] Create `ExpenceTracker/Modules/Badges/Application/Queries/GetBadgeByIdQuery.cs` — contains: (1) A `record GetBadgeByIdQuery(Guid Id) : IRequest<Badge?>`. (2) Handler returns `repository.GetByIdAsync(id)`.

### DI Registration

- [x] T030 [US1] Update `ExpenceTracker/Program.cs` — Read `Persistence:Provider` from `appsettings.json`. If the value is `"JsonFile"` (or absent/default), register `builder.Services.AddScoped<IBadgeRepository, JsonBadgeRepository>();`. If the value is `"EfCore"`, register the EF Core provider instead (placeholder for future). Add the necessary `using` statements for `ExpenceTracker.Modules.Badges.Domain` and `ExpenceTracker.Modules.Badges.Infrastructure`.

### Presentation Layer (Razor Pages)

- [x] T031 [US1] Create `ExpenceTracker/Pages/Badges/Index.cshtml` and `ExpenceTracker/Pages/Badges/Index.cshtml.cs` — The PageModel injects `IMediator` and `IStringLocalizer<SharedResource>`. `OnGetAsync`: dispatches `GetAllBadgesQuery`, binds result to `public List<Badge> Badges { get; set; }`. `OnPostDeleteAsync(Guid id)`: dispatches `DeleteBadgeCommand(id)`, redirects to self. The Razor view: `@page` directive, page title from localizer, a "Create" button linking to `/Badges/Create`, a table (or card list for mobile) showing each badge with its color as a colored circle/pill, name, and Edit/Delete action buttons. Delete button is inside a `<form method="post" asp-page-handler="Delete">` with a hidden `id` field. Use `@SharedLocalizer["key"]` for all text. Show **CSS-only skeleton shimmer placeholders** (per Skeleton Loader Strategy in plan.md) for the initial server-side paint. Show "No data" message if list is empty.
- [x] T032 [US1] Create `ExpenceTracker/Pages/Badges/Create.cshtml` and `ExpenceTracker/Pages/Badges/Create.cshtml.cs` — The PageModel injects `IMediator` and `IStringLocalizer<SharedResource>`. Bind properties: `[BindProperty] public string Name { get; set; }`, `[BindProperty] public string Color { get; set; } = "#4CAF50"`. `OnGet`: returns Page(). `OnPostAsync`: validate input (return Page with errors if invalid), dispatch `CreateBadgeCommand(Name, Color)`, redirect to `/Badges`. The Razor view: a form with a text input for Name (required), a color input (`<input type="color">`) for Color, and Save/Cancel buttons. Show validation errors using `asp-validation-for` tag helpers. Use shared localization for all labels.
- [x] T033 [US1] Create `ExpenceTracker/Pages/Badges/Edit.cshtml` and `ExpenceTracker/Pages/Badges/Edit.cshtml.cs` — The PageModel injects `IMediator`. Route: `@page "{id:guid}"`. `OnGetAsync(Guid id)`: dispatch `GetBadgeByIdQuery(id)`, if null return NotFound(), bind to properties. `[BindProperty] public Guid Id`, `[BindProperty] public string Name`, `[BindProperty] public string Color`. `OnPostAsync`: dispatch `UpdateBadgeCommand(Id, Name, Color)`, redirect to `/Badges`. Razor view: same form as Create but pre-populated.

### Localization for Badge Pages

- [x] T034 [P] [US1] Create localization resource files for badge pages: (1) `ExpenceTracker/Resources/Pages/Badges/IndexModel.ar.resx` with keys: `Title` = `الشارات`, `CreateNew` = `إضافة شارة جديدة`, `ConfirmDelete` = `هل أنت متأكد من حذف هذه الشارة؟`. (2) `ExpenceTracker/Resources/Pages/Badges/IndexModel.en.resx` with English equivalents. (3) `ExpenceTracker/Resources/Pages/Badges/CreateModel.ar.resx` with keys: `Title` = `إضافة شارة`, `NamePlaceholder` = `اسم الشارة`. (4) `ExpenceTracker/Resources/Pages/Badges/CreateModel.en.resx` with English equivalents. (5) `ExpenceTracker/Resources/Pages/Badges/EditModel.ar.resx` with keys: `Title` = `تعديل الشارة`. (6) `ExpenceTracker/Resources/Pages/Badges/EditModel.en.resx` with English equivalents.

**Checkpoint**: Badge Management (US1) is fully functional. You can create, view, edit, and soft-delete badges at `/Badges`. This is the first MVP increment.

---

## Phase 4: User Story 2 — Expense Entry (Priority: P1) 🎯 MVP

**Goal**: Users can add a new expense with name, amount, date, importance level, one or more badges, optional notes, and a pending status. The date defaults to now, importance defaults to Normal, and pending defaults to unchecked.

**Independent Test**: Navigate to `/Expenses/Create`, enter "Lunch" with amount 150, select the "Food" badge, leave defaults, click Save. Verify the expense is persisted in `data/expenses.json`.

### Domain Layer

- [x] T035 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Domain/ImportanceLevel.cs` — an enum `ImportanceLevel` in namespace `ExpenceTracker.Modules.Expenses.Domain` with values: `Normal = 0`, `Important = 1`, `VeryImportant = 2`.
- [x] T036 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Domain/Expense.cs` — a class `Expense` extending `Entity`. Namespace: `ExpenceTracker.Modules.Expenses.Domain`. Properties: `string Name { get; set; }` (required, max 200), `decimal Amount { get; set; }` (positive, max 99999999.99), `DateTimeOffset Date { get; set; }` (defaults to `DateTimeOffset.Now`), `ImportanceLevel Importance { get; set; }` (defaults to `Normal`), `string? Notes { get; set; }` (max 1000), `bool IsPending { get; set; }` (default `false`), `List<Guid> BadgeIds { get; set; }` (list of associated badge IDs — this is the denormalized form for JSON storage).
- [x] T037 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Domain/ExpenseBadge.cs` — a class `ExpenseBadge` in namespace `ExpenceTracker.Modules.Expenses.Domain`. Properties: `Guid ExpenseId { get; set; }`, `Guid BadgeId { get; set; }`. This is the join entity for EF Core mapping. In JSON storage mode, this entity is not directly used — `Expense.BadgeIds` is used instead.
- [x] T038 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Domain/IExpenseRepository.cs` — interface `IExpenseRepository` in namespace `ExpenceTracker.Modules.Expenses.Domain`. Methods: `Task<List<Expense>> GetAllAsync()`, `Task<Expense?> GetByIdAsync(Guid id)`, `Task AddAsync(Expense expense)`, `Task UpdateAsync(Expense expense)`, `Task DeleteAsync(Guid id)`, `Task<List<Expense>> GetByDateRangeAsync(DateTimeOffset from, DateTimeOffset to)`, `Task<bool> AnyWithBadgeAsync(Guid badgeId)` (returns true if any expense uses this badge).

### Infrastructure Layer

- [x] T039 [US2] Create `ExpenceTracker/Modules/Expenses/Infrastructure/JsonExpenseRepository.cs` — class extending `JsonFileRepository<Expense>` and implementing `IExpenseRepository`. Namespace: `ExpenceTracker.Modules.Expenses.Infrastructure`. Constructor takes `IConfiguration` and builds path: `Path.Combine(config["Persistence:DataDirectory"] ?? "data", "expenses.json")`. Implement: `GetAllAsync` returns all items sorted by Date descending; `GetByIdAsync` finds by ID; `AddAsync` reads all, adds, writes; `UpdateAsync` reads all, replaces matching item (set UpdatedAt), writes; `DeleteAsync` reads all, removes matching item, writes; `GetByDateRangeAsync` filters by date range; `AnyWithBadgeAsync` checks if any expense's `BadgeIds` list contains the given badge ID.
- [x] T040 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Infrastructure/ExpenseEntityConfiguration.cs` — EF Core Fluent API from data-model.md for Expense entity. Table "Expenses", key on Id, Name required max 200, Amount precision(12,2), Date required with index, Importance as int, Notes max 1000, IsPending default false with index, CreatedAt/UpdatedAt required.
- [x] T041 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Infrastructure/ExpenseBadgeEntityConfiguration.cs` — EF Core Fluent API from data-model.md for ExpenseBadge join entity. Table "ExpenseBadges", composite key (ExpenseId, BadgeId), FK to Expense with cascade delete, FK to Badge with restrict delete.

### Application Layer (DTOs & Commands)

- [x] T042 [P] [US2] Create `ExpenceTracker/Modules/Expenses/Application/DTOs/ExpenseDto.cs` — a record `ExpenseDto` in namespace `ExpenceTracker.Modules.Expenses.Application.DTOs`. Properties: `Guid Id`, `string Name`, `decimal Amount`, `DateTimeOffset Date`, `ImportanceLevel Importance`, `string? Notes`, `bool IsPending`, `List<Guid> BadgeIds`, `DateTimeOffset CreatedAt`. Also include a list of badge details: `List<BadgeInfo> Badges` where `BadgeInfo` is an inner record with `Guid Id`, `string Name`, `string Color`, `bool IsDeleted`.
- [x] T043 [US2] Create `ExpenceTracker/Modules/Expenses/Application/Commands/CreateExpenseCommand.cs` — contains: (1) A record `CreateExpenseCommand(string Name, decimal Amount, DateTimeOffset Date, ImportanceLevel Importance, string? Notes, bool IsPending, List<Guid> BadgeIds) : IRequest<Guid>`. (2) Handler injects `IExpenseRepository`. In `Handle`: validate Name not empty, Amount > 0 and ≤ 99999999.99, create `Expense` with all fields, set `Date` (use provided or `DateTimeOffset.Now`), call `AddAsync`, return `Id`.

### DI Registration

- [x] T044 [US2] Update `ExpenceTracker/Program.cs` — Read `Persistence:Provider` from `appsettings.json` (reuse the same config value from T030). If `"JsonFile"` (or absent/default), register `builder.Services.AddScoped<IExpenseRepository, JsonExpenseRepository>();`. If `"EfCore"`, register the EF Core provider instead (placeholder for future). Add using for `ExpenceTracker.Modules.Expenses.Domain` and `ExpenceTracker.Modules.Expenses.Infrastructure`.

### Presentation Layer

- [x] T045 [US2] Create `ExpenceTracker/Pages/Expenses/Create.cshtml` and `ExpenceTracker/Pages/Expenses/Create.cshtml.cs` — PageModel injects `IMediator` and `IStringLocalizer<SharedResource>` (**NOT** `IBadgeRepository` — use MediatR to dispatch `GetAllBadgesQuery` to fetch badges from the Badges module). Properties: `[BindProperty] public string Name`, `[BindProperty] public decimal Amount`, `[BindProperty] public DateTimeOffset Date` (default `DateTimeOffset.Now`), `[BindProperty] public ImportanceLevel Importance` (default Normal), `[BindProperty] public string? Notes`, `[BindProperty] public bool IsPending`, `[BindProperty] public string BadgeIds` (comma-separated string of Guid IDs), `public List<Badge> AvailableBadges` (for display). `OnGetAsync`: dispatch `GetAllBadgesQuery` via MediatR to load all active badges, set Date to now. `OnPostAsync`: parse BadgeIds string into `List<Guid>`, dispatch `CreateExpenseCommand`, redirect to `/Expenses/Create?saved=true` (PRG pattern for quick-entry). If `saved` query param present, show success TempData message. Razor view: form with — text input for Name, number input for Amount (step="0.01", min="0.01"), datetime-local input for Date, select dropdown for Importance (Normal/Important/VeryImportant with Arabic labels), a badge toggle section showing all badges as colored pills that toggle on/off with JavaScript, textarea for Notes, checkbox for IsPending, hidden input for BadgeIds (updated by JS). Save and Cancel buttons.

### Localization for Expense Create

- [x] T046 [P] [US2] Create localization resource files: (1) `ExpenceTracker/Resources/Pages/Expenses/CreateModel.ar.resx` with keys: `Title` = `إضافة مصروف`, `NamePlaceholder` = `اسم المصروف`, `AmountPlaceholder` = `المبلغ`, `SelectBadges` = `اختر الشارات`, `NotesPlaceholder` = `ملاحظات (اختياري)`, `SavedSuccess` = `تم حفظ المصروف بنجاح`. (2) `ExpenceTracker/Resources/Pages/Expenses/CreateModel.en.resx` with English equivalents: `Title` = `Add Expense`, `NamePlaceholder` = `Expense name`, `AmountPlaceholder` = `Amount`, `SelectBadges` = `Select badges`, `NotesPlaceholder` = `Notes (optional)`, `SavedSuccess` = `Expense saved successfully`.

**Checkpoint**: Expense Entry (US2) is fully functional. Users can add expenses with all fields at `/Expenses/Create`. Combined with US1, this is the core MVP.

---

## Phase 5: User Story 3 — Expense List & Management (Priority: P2)

**Goal**: Users can view all expenses in a list, edit any expense, and delete expenses. The list shows name, amount, date, badges with colors, importance level, and pending status.

**Independent Test**: After adding some expenses via `/Expenses/Create`, navigate to `/Expenses`, see them in the list, edit one (change amount), delete another.

### Application Layer (Commands & Queries)

- [x] T047 [P] [US3] Create `ExpenceTracker/Modules/Expenses/Application/Commands/UpdateExpenseCommand.cs` — record `UpdateExpenseCommand(Guid Id, string Name, decimal Amount, DateTimeOffset Date, ImportanceLevel Importance, string? Notes, bool IsPending, List<Guid> BadgeIds) : IRequest<bool>`. Handler: get by ID, return false if not found, validate same as create, update all fields, set `UpdatedAt`, call `UpdateAsync`, return true.
- [x] T048 [P] [US3] Create `ExpenceTracker/Modules/Expenses/Application/Commands/DeleteExpenseCommand.cs` — record `DeleteExpenseCommand(Guid Id) : IRequest<bool>`. Handler: call `DeleteAsync(id)`, return true. This is a permanent delete (not soft-delete).
- [x] T049 [P] [US3] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetAllExpensesQuery.cs` — record `GetAllExpensesQuery() : IRequest<List<ExpenseDto>>`. Handler injects `IExpenseRepository` and `IMediator` (**NOT** `IBadgeRepository`). Gets all expenses, then dispatches `GetAllBadgesIncludingDeletedQuery` via MediatR to fetch all badges (active + soft-deleted for historical display), maps each expense to `ExpenseDto` with badge details (name and color from badge lookup). Soft-deleted badges MUST be mapped with a flag or visual indicator (reduced opacity + “deleted” label) so the presentation layer can distinguish them. Returns list sorted by Date descending.
- [x] T050 [P] [US3] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetExpenseByIdQuery.cs` — record `GetExpenseByIdQuery(Guid Id) : IRequest<ExpenseDto?>`. Handler injects `IExpenseRepository` and `IMediator` (**NOT** `IBadgeRepository`). Gets expense by ID, dispatches `GetBadgeByIdQuery` via MediatR for each badge ID to load badge details, maps to DTO, returns.

### Presentation Layer

- [x] T051 [US3] Create `ExpenceTracker/Pages/Expenses/Index.cshtml` and `ExpenceTracker/Pages/Expenses/Index.cshtml.cs` — PageModel injects `IMediator`, `IStringLocalizer<SharedResource>`. `OnGetAsync`: dispatch `GetAllExpensesQuery`, bind to `public List<ExpenseDto> Expenses`. `OnPostDeleteAsync(Guid id)`: dispatch `DeleteExpenseCommand(id)`, redirect to self. Razor view: `@page` directive, page title, "Add Expense" button linking to `/Expenses/Create`, a responsive card list or table showing each expense with: Name (bold), Amount (formatted with 2 decimal places), Date (formatted), colored badge pills for each associated badge, importance level indicator (colored text or icon: green for Normal, orange for Important, red for Very Important), pending status (if pending show a dashed border or label). **Soft-deleted badges** MUST be rendered with reduced opacity and a “deleted” indicator so users can distinguish them from active badges. **Future-dated expenses** MUST be visually distinguished (e.g., a distinct icon or color highlight per FR-025). The list MUST implement a **"Load More" button** pagination approach (loading 20 items per batch, per FR-028) for long lists. Show **CSS-only skeleton shimmer placeholders** (per Skeleton Loader Strategy in plan.md) for the initial server-side paint. Each expense has Edit and Delete action buttons. Pending expenses should have `.expense-pending` CSS class. Use `@SharedLocalizer` for all labels.
- [x] T052 [US3] Create `ExpenceTracker/Pages/Expenses/Edit.cshtml` and `ExpenceTracker/Pages/Expenses/Edit.cshtml.cs` — Route: `@page "{id:guid}"`. PageModel injects `IMediator` (**NOT** `IBadgeRepository` — use MediatR to dispatch `GetAllBadgesQuery` for the badge picker). `OnGetAsync(Guid id)`: dispatch `GetExpenseByIdQuery(id)`, if null return NotFound(), bind properties from DTO. Dispatch `GetAllBadgesQuery` to populate the available badges list. `OnPostAsync`: dispatch `UpdateExpenseCommand(...)`, redirect to `/Expenses`. Razor view: same form layout as Create page but pre-populated with existing values. Badge toggles should show already-selected badges as `.selected`.

### Localization for Expense List & Edit

- [x] T053 [P] [US3] Create localization resource files: (1) `ExpenceTracker/Resources/Pages/Expenses/IndexModel.ar.resx` with keys: `Title` = `المصروفات`, `AddNew` = `إضافة مصروف جديد`, `ConfirmDelete` = `هل أنت متأكد من حذف هذا المصروف؟`. (2) `ExpenceTracker/Resources/Pages/Expenses/IndexModel.en.resx` with English values. (3) `ExpenceTracker/Resources/Pages/Expenses/EditModel.ar.resx` with keys: `Title` = `تعديل المصروف`. (4) `ExpenceTracker/Resources/Pages/Expenses/EditModel.en.resx` with English values.

**Checkpoint**: Expense List & Management (US3) is fully functional. Users can view, edit, and delete expenses at `/Expenses`. All P1 and P2-list functionality complete.

---

## Phase 6: User Story 4 — Dashboard Overview (Priority: P2)

**Goal**: The home page (`/`) shows a dashboard with: today's total spending, this month's total spending, a breakdown of spending by badge (each showing the badge color), and a separate pending total. All totals exclude pending expenses except the pending total itself.

**Independent Test**: Add several expenses for today with different badges, some pending. Open `/`. Verify daily total matches sum of non-pending expenses. Verify monthly total. Verify badge breakdown adds up. Verify pending total shows separately.

### Application Layer (DTOs & Queries)

- [x] T054 [P] [US4] Create `ExpenceTracker/Modules/Expenses/Application/DTOs/DashboardSummaryDto.cs` — defines two records in namespace `ExpenceTracker.Modules.Expenses.Application.DTOs`: (1) `record BadgeBreakdownItem(Guid BadgeId, string BadgeName, string BadgeColor, decimal Total)` — actively consumed by T057 and T058. (2) *(Optional/future)* `record DashboardSummaryDto(decimal DailyTotal, decimal MonthlyTotal, decimal PendingTotal, List<BadgeBreakdownItem> BadgeBreakdown)` — currently unused; T058 dispatches individual queries instead. Retain for potential future aggregation endpoint.
- [x] T055 [P] [US4] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetDailySummaryQuery.cs` — record `GetDailySummaryQuery(DateTimeOffset Date) : IRequest<decimal>`. Handler: get all expenses, filter by `Date.Date == query.Date.Date` and `IsPending == false`, sum amounts, return total.
- [x] T056 [P] [US4] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetMonthlySummaryQuery.cs` — record `GetMonthlySummaryQuery(int Year, int Month) : IRequest<decimal>`. Handler: get all expenses, filter by year/month and not pending, sum amounts, return total.
- [x] T057 [P] [US4] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetBadgeSummaryQuery.cs` — record `GetBadgeSummaryQuery() : IRequest<List<BadgeBreakdownItem>>`. Handler injects `IExpenseRepository` and `IMediator` (**NOT** `IBadgeRepository`). Gets all non-pending expenses, then dispatches `GetAllBadgesIncludingDeletedQuery` via MediatR to fetch all badges (active + soft-deleted). For each badge that has expenses: sum the amounts of non-pending expenses that contain that badge ID. Return a list of `BadgeBreakdownItem` sorted by total descending.
- [x] T074 [P] [US4] Create `ExpenceTracker/Modules/Expenses/Application/Queries/GetPendingTotalQuery.cs` — record `GetPendingTotalQuery() : IRequest<decimal>`. Handler injects `IExpenseRepository`. Gets all expenses, filters `IsPending == true`, sums amounts, returns total. This query is dispatched by the dashboard (T058) to retrieve the pending total via MediatR, avoiding inline calculation in the PageModel.

### Presentation Layer

- [x] T058 [US4] Update `ExpenceTracker/Pages/Index.cshtml` and `ExpenceTracker/Pages/Index.cshtml.cs` — Replace the existing default page with the dashboard. PageModel injects `IMediator` and `IStringLocalizer<SharedResource>` (**NOT** `IBadgeRepository`). `OnGetAsync`: dispatch `GetDailySummaryQuery(DateTimeOffset.Now)`, `GetMonthlySummaryQuery(DateTime.Now.Year, DateTime.Now.Month)`, `GetBadgeSummaryQuery()`, and `GetPendingTotalQuery()` to get the pending total via MediatR (no inline calculation). Expose properties: `decimal DailyTotal`, `decimal MonthlyTotal`, `decimal PendingTotal`, `List<BadgeBreakdownItem> BadgeBreakdown`. Show **CSS-only skeleton shimmer placeholders** (per Skeleton Loader Strategy in plan.md) for the initial server-side paint. The dashboard displays summary cards only — it does NOT render individual expense rows, so no "Load More" pagination or FR-025 future-date indicators apply here (those are handled in T051). Razor view: a responsive dashboard layout with Bootstrap cards: (1) Top row: 3 cards side-by-side on desktop, stacked on mobile — "Daily Total" card with large number, "Monthly Total" card with large number, "Pending Total" card with large number (in a muted/dashed style). (2) Below: "By Badge" section showing the top 5 badge breakdown items (per FR-018), each as a card/row with the badge color circle, badge name, and total amount. Numbers formatted to 2 decimal places. Show "No data" message if no expenses exist. Use localized labels from SharedResource.
- [x] T059 [P] [US4] Create localization resource files: (1) `ExpenceTracker/Resources/Pages/IndexModel.ar.resx` with keys: `Title` = `لوحة القيادة`, `WelcomeMessage` = `مرحبا بك في متتبع المصروفات`. (2) `ExpenceTracker/Resources/Pages/IndexModel.en.resx` with English equivalents.

**Checkpoint**: Dashboard (US4) is fully functional. The home page shows daily, monthly, pending totals and badge breakdown. All P1 + P2 features complete.

---

## Phase 7: User Story 5 — Quick Entry UI (Priority: P3)

**Goal**: Optimize the expense entry form for speed. After saving, the form resets instantly for the next entry. Badge toggles are one-tap inline pills. Tab order is logical. Defaults pre-fill most fields so only name/amount are required.

**Independent Test**: Open `/Expenses/Create`, type a name, type an amount, tap a badge pill, hit Save. Form should reset with success message. Immediately type next expense. Should take < 20 seconds per expense.

- [x] T060 [US5] Enhance `ExpenceTracker/Pages/Expenses/Create.cshtml` — Add the following quick-entry UX improvements: (1) After a successful save redirect, show a toast/alert success message that auto-dismisses after 3 seconds (use TempData + JS). (2) Set `autofocus` on the Name input so the cursor is ready immediately. (3) Ensure tab order is: Name → Amount → Date → Importance → Badge pills → Notes → Pending → Save button. (4) Add keyboard shortcut: Ctrl+Enter submits the form (add a small JS snippet). (5) Make the Amount input type="number" with `inputmode="decimal"` for mobile numeric keyboard on Android.
- [x] T061 [US5] Enhance `ExpenceTracker/wwwroot/js/site.js` — Add Quick Entry specific enhancements: (1) Auto-dismiss logic for success toast: after page load, if `.toast-success` element exists, auto-hide after 3 seconds. (2) Ctrl+Enter keyboard shortcut to submit the expense form. (3) Auto-focus the Name input after form reset for rapid consecutive entry.
- [x] T062 [US5] Enhance `ExpenceTracker/wwwroot/css/site.css` — Add: (1) `.badge-toggle` pills with larger size on mobile (`@media (max-width: 768px)` — increase padding to 10px 20px and font-size to 1rem). (2) `.toast-success` styles — fixed top-center position, green background, white text, slide-in animation. (3) Form field focus highlighting for better visual feedback. (4) Input fields with larger font on mobile for readability (`font-size: 16px` to prevent iOS zoom).

**Checkpoint**: Quick Entry UI (US5) is fully functional. Expense entry is optimized for speed on both desktop and mobile.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final improvements that affect multiple user stories — PWA validation, theme polish, language toggle, and overall quality.

- [x] T063 Verify and fix the language toggle in `ExpenceTracker/Pages/Shared/_Layout.cshtml` — Ensure the language toggle is a form POST that sets the `.AspNetCore.Culture` cookie. When Arabic is selected, set `c=ar|uic=ar`. When English, `c=en|uic=en`. After setting the cookie, redirect to the same page. The toggle should show "EN" when Arabic is active (offering to switch to English) and "AR" when English is active.
- [x] T064 Test the theme toggle in `ExpenceTracker/wwwroot/js/site.js` — Verify that: (1) On first visit, system preference is detected. (2) Clicking the toggle switches themes instantly. (3) The preference is saved in `localStorage` and persists across page loads. (4) The toggle icon updates (☀️ ↔ 🌙). Fix any issues found.
- [x] T065 Verify PWA installability for Android — Open the app in Chrome on Android (or Chrome DevTools mobile emulation). (1) Check that the "Install" prompt appears (or "Add to Home Screen" in the menu). (2) Verify `manifest.json` is loaded (DevTools → Application → Manifest). (3) Verify service worker is registered (DevTools → Application → Service Workers). (4) Go offline (DevTools → Network → Offline), navigate to each page — cached pages should load. (5) Fix any issues found. Common fixes: ensure `start_url` is cached in service worker install, ensure all critical assets are listed.
- [x] T066 Update `ExpenceTracker/Pages/Error.cshtml` — Style the error page to match the app theme. Show a user-friendly error message in Arabic (with English fallback). Include a "Go to Dashboard" link.
- [x] T067 Update `ExpenceTracker/Pages/Privacy.cshtml` — Replace default content with a simple privacy notice in Arabic explaining that data is stored locally on the device and no data is sent to external servers.
- [x] T068 Run `ExpenceTracker/specs/001-expense-tracker-core/quickstart.md` validation — Follow each step in quickstart.md manually: restore dependencies, run the app, test adding a badge, adding an expense, viewing the dashboard, switching language, switching theme. Fix any issues discovered.
- [x] T069 [P] Add CSS transitions and reduced-motion support — Add smooth CSS transitions to layout components: card entries (fade/slide on appear), page navigation transitions, and button feedback (scale/color on press). Include `@media (prefers-reduced-motion: reduce)` rules that disable or minimize all animations and transitions for accessibility. Apply in `ExpenceTracker/wwwroot/css/site.css`.
- [x] T070 [P] WCAG 2.1 AA accessibility audit — Audit all pages for WCAG 2.1 AA compliance: (1) Verify all interactive elements have appropriate ARIA labels. (2) Check color contrast ratios meet AA minimums (4.5:1 for normal text, 3:1 for large text). (3) Verify full keyboard navigation (Tab, Enter, Escape) across all pages. (4) Ensure focus indicators are clearly visible. (5) Test with screen reader (e.g., NVDA or Narrator). Fix any violations found.
- [x] T071 [P] Lighthouse performance validation — Run a Lighthouse audit (Chrome DevTools) on the deployed/running app with throttled 3G network simulation. Verify: (1) PWA score >= 90. (2) Page load times < 2 seconds on throttled 3G. (3) Performance score >= 80. Fix any issues found (e.g., optimize images, defer non-critical JS, reduce CSS blocking).
- [x] T072 [P] Manual user story validation across themes and languages — Manually execute every acceptance scenario from all 5 user stories (US1–US5 in spec.md) across both themes (light and dark) and both languages (Arabic and English). Verify: (1) All text is properly localized (no hardcoded strings). (2) RTL layout is correct in Arabic, LTR in English. (3) Theme colors and contrast are correct in both themes. (4) All CRUD operations function correctly in all combinations. Document and fix any issues found.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion — BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Phase 2 completion — No dependencies on other stories
- **User Story 2 (Phase 4)**: Depends on Phase 2 completion + requires badges from US1 to exist for badge selection
- **User Story 3 (Phase 5)**: Depends on Phase 4 completion (needs expenses to list/edit/delete)
- **User Story 4 (Phase 6)**: Depends on Phase 4 completion (needs expenses for dashboard calculations)
- **User Story 5 (Phase 7)**: Depends on Phase 4 completion (enhances the expense create form)
- **Polish (Phase 8)**: Depends on all user stories being complete

### User Story Dependencies

- **US1 (Badges)**: Can start immediately after Phase 2
- **US2 (Expense Entry)**: Can start after Phase 2, but needs US1 badges for the badge picker. Implement after US1 or ensure a few test badges exist.
- **US3 (Expense List)**: Needs US2 done first (needs expenses in the system)
- **US4 (Dashboard)**: Needs US2 done first (needs expense data for calculations). Can run in parallel with US3.
- **US5 (Quick Entry)**: Needs US2 done first (enhances the create form). Can run in parallel with US3 and US4.

### Within Each User Story

1. Domain layer first (entities, interfaces)
2. Infrastructure layer second (repository implementations)
3. Application layer third (commands, queries)
4. DI registration
5. Presentation layer last (Razor Pages)
6. Localization files (can be parallel with presentation)

### Parallel Opportunities

- T004 can run in parallel with other Shared Domain tasks
- T009 + T010 + T011 can run in parallel (localization resources)
- T017 + T018 + T019 + T020 can run in parallel (PWA assets)
- T021 + T022 can run in parallel (badge domain)
- T025 + T026 + T027 + T028 + T073 + T029 can run in parallel (badge commands/queries)
- T035 + T036 + T037 + T038 can run in parallel (expense domain)
- T040 + T041 can run in parallel (expense EF Core configs)
- T047 + T048 + T049 + T050 can run in parallel (expense list commands/queries)
- T054 + T055 + T056 + T057 + T074 can run in parallel (dashboard queries)
- US3 + US4 + US5 can proceed in parallel after US2 is complete

---

## Parallel Example: Phase 3 (User Story 1 — Badges)

```bash
# Step 1: Domain layer (parallel)
Task T021: "Create Badge entity in Modules/Badges/Domain/Badge.cs"
Task T022: "Create IBadgeRepository in Modules/Badges/Domain/IBadgeRepository.cs"

# Step 2: Infrastructure (sequential after domain)
Task T023: "Create JsonBadgeRepository in Modules/Badges/Infrastructure/JsonBadgeRepository.cs"
Task T024: "Create BadgeEntityConfiguration" (parallel with T023 — different file)

# Step 3: Application (parallel — all in different files, depend only on domain)
Task T025: "CreateBadgeCommand"
Task T026: "UpdateBadgeCommand"
Task T027: "DeleteBadgeCommand"
Task T028: "GetAllBadgesQuery"
Task T073: "GetAllBadgesIncludingDeletedQuery"
Task T029: "GetBadgeByIdQuery"

# Step 4: DI (sequential — must register before pages)
Task T030: "Register badge services in Program.cs"

# Step 5: Pages (sequential — depend on commands/queries)
Task T031: "Badges/Index page"
Task T032: "Badges/Create page"
Task T033: "Badges/Edit page"
Task T034: "Localization files" (parallel with pages)
```

---

## Implementation Strategy

### MVP First (User Story 1 + User Story 2)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks everything)
3. Complete Phase 3: User Story 1 — Badge Management
4. Complete Phase 4: User Story 2 — Expense Entry
5. **STOP and VALIDATE**: Test creating badges and recording expenses. App should be installable on Android via PWA.
6. This is a usable MVP — users can track expenses.

### Incremental Delivery

1. Setup + Foundational → Foundation ready, PWA installable
2. Add US1 (Badges) → Test independently → Badges work
3. Add US2 (Expense Entry) → Test independently → Core MVP!
4. Add US3 (Expense List) → Test → Full CRUD done
5. Add US4 (Dashboard) → Test → Analytics done
6. Add US5 (Quick Entry) → Test → UX optimized
7. Polish → Final validation → Ship!

### Critical Path

```
Phase 1 → Phase 2 → Phase 3 (US1) → Phase 4 (US2) → Phase 5/6/7 (parallel) → Phase 8
```

---

## Summary

| Metric | Value |
|--------|-------|
| **Total tasks** | 73 |
| **Phase 1 (Setup)** | 3 tasks |
| **Phase 2 (Foundational)** | 16 tasks |
| **Phase 3 — US1 (Badges)** | 15 tasks |
| **Phase 4 — US2 (Expense Entry)** | 12 tasks |
| **Phase 5 — US3 (Expense List)** | 7 tasks |
| **Phase 6 — US4 (Dashboard)** | 7 tasks |
| **Phase 7 — US5 (Quick Entry)** | 3 tasks |
| **Phase 8 (Polish)** | 10 tasks |
| **Parallel opportunities** | 10+ groups of parallel tasks |
| **MVP scope** | Phase 1-4 (US1 + US2 = 45 tasks) |
| **Android offline** | PWA setup in Phase 2 (T017-T020) |

---

## Notes

- All text labels MUST use `IStringLocalizer` / `SharedLocalizer` — never hardcode Arabic or English strings in Razor views or PageModels.
- All amounts are displayed with 2 decimal places (e.g., `amount.ToString("N2")`).
- All forms use anti-forgery tokens automatically (Razor Pages convention).
- JSON data files are stored in `/data` folder (configurable via `appsettings.json`).
- The service worker caches all static assets and pages for Android offline use.
- Badge colors are stored as hex strings (e.g., `#FF5733`).
- Soft-deleted badges are hidden from badge lists and badge pickers but preserved for historical expense display.
- [P] tasks = different files, no dependencies within the same step.
- [USn] label maps each task to its user story for traceability.
- Commit after each completed task or logical group of parallel tasks.
- Stop at any checkpoint to validate that story independently before proceeding.
