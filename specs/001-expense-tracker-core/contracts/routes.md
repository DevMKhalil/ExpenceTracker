# Routes & Page Contracts: Expense Tracker Core

**Feature**: 001-expense-tracker-core  
**Date**: 2026-03-28

---

## Page Routes

All routes follow ASP.NET Core Razor Pages conventions (`/Pages/{Folder}/{Page}` → URL `/{Folder}/{Page}`).

### Dashboard

| Route | Page | Method | Description |
|-------|------|--------|-------------|
| `/` | `Pages/Index.cshtml` | GET | Dashboard — daily total, monthly total, badge breakdown, pending total |

**Page Model** (`IndexModel`):
- **OnGet**: Dispatches `GetDailySummaryQuery`, `GetMonthlySummaryQuery`, `GetBadgeSummaryQuery` via MediatR.
- **View Data**: `DailyTotal (decimal)`, `MonthlyTotal (decimal)`, `PendingTotal (decimal)`, `BadgeBreakdown (List<BadgeSummaryDto>)` where each entry has `BadgeName`, `BadgeColor`, `Total`.

---

### Badge Management

| Route | Page | Method | Description |
|-------|------|--------|-------------|
| `/Badges` | `Pages/Badges/Index.cshtml` | GET | List all active (non-deleted) badges |
| `/Badges/Create` | `Pages/Badges/Create.cshtml` | GET | Show create badge form |
| `/Badges/Create` | `Pages/Badges/Create.cshtml` | POST | Submit new badge (`Name`, `Color`) |
| `/Badges/Edit/{id}` | `Pages/Badges/Edit.cshtml` | GET | Show edit form for badge by ID |
| `/Badges/Edit/{id}` | `Pages/Badges/Edit.cshtml` | POST | Submit badge updates |
| `/Badges/Delete` | `Pages/Badges/Index.cshtml` | POST | Soft-delete badge by ID (handler on Index page) |

**Create Form Fields**:
- `Name` (string, required, max 100)
- `Color` (string, required, hex color picker)

**Edit Form Fields**: Same as Create, pre-populated with existing values.

**Delete Behavior**: POST handler on Badges/Index. If badge has expenses: confirm dialog → soft-delete. If no expenses: soft-delete (uniform behavior).

---

### Expense Management

| Route | Page | Method | Description |
|-------|------|--------|-------------|
| `/Expenses` | `Pages/Expenses/Index.cshtml` | GET | List all expenses (newest first) |
| `/Expenses/Create` | `Pages/Expenses/Create.cshtml` | GET | Show quick-entry expense form |
| `/Expenses/Create` | `Pages/Expenses/Create.cshtml` | POST | Submit new expense |
| `/Expenses/Edit/{id}` | `Pages/Expenses/Edit.cshtml` | GET | Show edit form for expense by ID |
| `/Expenses/Edit/{id}` | `Pages/Expenses/Edit.cshtml` | POST | Submit expense updates |
| `/Expenses/Delete` | `Pages/Expenses/Index.cshtml` | POST | Delete expense by ID (handler on Index page) |

**Create Form Fields** (Quick Entry):
- `Name` (string, required, max 200)
- `Amount` (decimal, required, > 0)
- `Date` (DateTimeOffset, defaults to now, editable)
- `Importance` (select: Normal / Important / Very Important, defaults to Normal)
- `BadgeIds` (multi-select via inline pill toggles, at least one recommended)
- `Notes` (textarea, optional, max 1000)
- `IsPending` (checkbox, defaults to unchecked)

**Edit Form Fields**: Same as Create, pre-populated with existing values.

**Delete Behavior**: POST handler on Expenses/Index. Permanent delete with confirmation.

---

## Navigation Structure

```
┌─────────────────────────────────────────────┐
│  Navbar                                      │
│  ┌──────┐  ┌──────┐  ┌──────────┐  ┌─────┐ │
│  │ لوحة │  │الشارات│  │المصروفات │  │ 🌙  │ │
│  │القيادة│  │      │  │          │  │/☀️  │ │
│  └──────┘  └──────┘  └──────────┘  └─────┘ │
│                                    ┌─────┐  │
│                                    │AR/EN│  │
│                                    └─────┘  │
└─────────────────────────────────────────────┘
```

- **لوحة القيادة / Dashboard** → `/` (Index)
- **الشارات / Badges** → `/Badges`
- **المصروفات / Expenses** → `/Expenses`
- **Theme toggle** → Client-side `data-bs-theme` switch
- **Language toggle** → Form POST to set `.AspNetCore.Culture` cookie, redirect

---

## Anti-Forgery

All POST forms include `@Html.AntiForgeryToken()` via Razor Pages conventions (auto-validated by the framework). No additional CSRF configuration needed.
