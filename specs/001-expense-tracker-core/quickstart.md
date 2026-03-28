# Quickstart: Expense Tracker Core

**Feature**: 001-expense-tracker-core  
**Date**: 2026-03-28

---

## Prerequisites

- .NET 10 SDK
- A modern web browser (Chrome, Edge, Firefox, Safari)
- A text editor or IDE (VS Code, Visual Studio, Rider)

---

## Setup

### 1. Restore Dependencies

```bash
cd ExpenceTracker
dotnet restore
```

### 2. Configuration

Edit `appsettings.json` to set the persistence provider and data directory:

```json
{
  "Persistence": {
    "Provider": "JsonFile",
    "DataDirectory": "data"
  }
}
```

- `Provider`: `"JsonFile"` (default) or `"EfCore"` (future — requires connection string).
- `DataDirectory`: Relative or absolute path where JSON data files are stored. Created automatically on first run.

### 3. Run the Application

```bash
dotnet run
```

The app starts at `https://localhost:5001` (HTTPS) or `http://localhost:5000`.

---

## Key Workflows

### Add a Badge

1. Navigate to **الشارات / Badges** via the navbar.
2. Click **إضافة شارة / Add Badge**.
3. Enter a name and pick a color.
4. Click **حفظ / Save**.

### Record an Expense (Quick Entry)

1. Navigate to **المصروفات / Expenses** → **إضافة مصروف / Add Expense**.
2. Enter the expense name and amount (required).
3. Tap badge pills to toggle categories on/off.
4. Adjust date, importance, notes, or pending flag as needed.
5. Click **حفظ / Save** — the form resets for the next entry.

### View Dashboard

1. Navigate to **لوحة القيادة / Dashboard** (home page).
2. See today's total, this month's total, and spending by badge.
3. Pending expenses are shown separately.

### Switch Language

Click the **AR / EN** toggle in the navbar. The page reloads in the selected language with the appropriate text direction (RTL for Arabic, LTR for English).

### Switch Theme

Click the **🌙 / ☀️** icon in the navbar. The theme toggles instantly between light and dark mode.

---

## Project Structure Summary

```
ExpenceTracker/
├── Modules/Badges/       # Badge domain, commands, queries, persistence
├── Modules/Expenses/     # Expense domain, commands, queries, persistence
├── Shared/               # Base entity, shared infrastructure
├── Pages/                # Razor Pages (presentation layer)
├── Resources/            # Localization .resx files (ar + en)
└── wwwroot/              # Static assets, PWA manifest, service worker
```

---

## NuGet Packages Required

| Package | Purpose |
|---------|---------|
| `MediatR` | CQRS command/query dispatching |
| `Microsoft.Extensions.Localization` | `IStringLocalizer` / `IViewLocalizer` |

All other dependencies (Bootstrap 5, jQuery Validation) are already present as client-side libraries in `wwwroot/lib/`.

---

## Data Files

On first run with `JsonFile` provider, the following files are created automatically in the `DataDirectory`:

```
data/
├── badges.json     # Badge records
└── expenses.json   # Expense records (with embedded BadgeIds)
```

These are plain JSON arrays — human-readable and easy to back up or inspect.
