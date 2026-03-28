# Data Model: Expense Tracker Core

**Feature**: 001-expense-tracker-core  
**Date**: 2026-03-28

---

## Entity Overview

```
┌─────────────┐       M:N       ┌──────────────┐
│   Badge      │◄───────────────►│   Expense     │
│              │  ExpenseBadge   │              │
└─────────────┘                 └──────────────┘
```

Two core entities (`Badge`, `Expense`) linked through a join entity (`ExpenseBadge`) for the many-to-many relationship.

---

## Entities

### Badge

**Module**: Badges  
**Description**: A reusable category label for expenses. Supports soft-delete so historical expenses retain their badge information.

| Field | Type | Required | Default | Constraints |
|-------|------|----------|---------|-------------|
| `Id` | `Guid` | Yes | `Guid.NewGuid()` | Primary key |
| `Name` | `string` | Yes | — | Max 100 chars, unique (case-insensitive), not empty |
| `Color` | `string` | Yes | — | Hex color code (e.g., `#FF5733`), 7 chars exactly |
| `IsDeleted` | `bool` | Yes | `false` | Soft-delete flag |
| `CreatedAt` | `DateTimeOffset` | Yes | `DateTimeOffset.UtcNow` | Immutable after creation |
| `UpdatedAt` | `DateTimeOffset` | Yes | `DateTimeOffset.UtcNow` | Updated on every modification |

**Validation Rules**:
- `Name` must be non-empty and ≤ 100 characters.
- `Name` must be unique across all non-deleted badges (case-insensitive comparison).
- `Color` must match pattern `^#[0-9A-Fa-f]{6}$`.
- Cannot edit a soft-deleted badge (only un-delete or view historical data).

**State Transitions**:
```
Active ──[Delete]──► SoftDeleted
  ▲                      │
  └──── [Restore] ───────┘  (future, not in current scope)
```

---

### Expense

**Module**: Expenses  
**Description**: A spending record with name, amount, date, importance, optional notes, pending flag, and one or more badge associations.

| Field | Type | Required | Default | Constraints |
|-------|------|----------|---------|-------------|
| `Id` | `Guid` | Yes | `Guid.NewGuid()` | Primary key |
| `Name` | `string` | Yes | — | Max 200 chars, not empty |
| `Amount` | `decimal` | Yes | — | > 0, ≤ 99,999,999.99, 2 decimal places |
| `Date` | `DateTimeOffset` | Yes | Current date/time | Any valid date (past and future allowed) |
| `Importance` | `ImportanceLevel` | Yes | `Normal` | Enum: Normal = 0, Important = 1, VeryImportant = 2 |
| `Notes` | `string?` | No | `null` | Max 1000 chars |
| `IsPending` | `bool` | Yes | `false` | Pending/finalized flag |
| `CreatedAt` | `DateTimeOffset` | Yes | `DateTimeOffset.UtcNow` | Immutable after creation |
| `UpdatedAt` | `DateTimeOffset` | Yes | `DateTimeOffset.UtcNow` | Updated on every modification |

**Validation Rules**:
- `Name` must be non-empty and ≤ 200 characters.
- `Amount` must be > 0 and ≤ 99,999,999.99. Stored/displayed with 2 decimal places.
- `Date` is required. Defaults to current date/time on creation. Future dates are allowed (visually indicated in UI).
- `Importance` must be a valid `ImportanceLevel` enum value.
- `Notes` is optional; if provided, ≤ 1000 characters.
- At least one badge should be associated (validated at application layer as a warning, not a hard block).

---

### ExpenseBadge (Join Entity)

**Module**: Expenses  
**Description**: Represents the many-to-many relationship between Expense and Badge.

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `ExpenseId` | `Guid` | Yes | FK → Expense.Id, part of composite PK |
| `BadgeId` | `Guid` | Yes | FK → Badge.Id, part of composite PK |

**Validation Rules**:
- Composite primary key: (`ExpenseId`, `BadgeId`).
- Both foreign keys must reference existing records.
- No duplicate pairs allowed.

---

## Value Objects / Enums

### ImportanceLevel (Enum)

**Module**: Expenses

| Value | Name | Display (Arabic) | Display (English) |
|-------|------|-------------------|-------------------|
| 0 | `Normal` | عادي | Normal |
| 1 | `Important` | مهم | Important |
| 2 | `VeryImportant` | مهم جدًا | Very Important |

---

## Relationships

| From | To | Type | Notes |
|------|----|------|-------|
| Expense | Badge | Many-to-Many | Through `ExpenseBadge` join entity |
| Badge | Expense | Many-to-Many | A badge can be on many expenses |
| ExpenseBadge | Expense | Many-to-One | FK `ExpenseId` |
| ExpenseBadge | Badge | Many-to-One | FK `BadgeId` |

---

## EF Core Fluent API Mapping (Maintained for Future Swap)

### BadgeEntityConfiguration

```csharp
public class BadgeEntityConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.ToTable("Badges");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Color).IsRequired().HasMaxLength(7);
        builder.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired();
        builder.HasIndex(b => b.Name).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
```

### ExpenseEntityConfiguration

```csharp
public class ExpenseEntityConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Amount).IsRequired().HasPrecision(12, 2);
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.Importance).IsRequired().HasConversion<int>();
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.IsPending).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsPending);
    }
}
```

### ExpenseBadgeEntityConfiguration

```csharp
public class ExpenseBadgeEntityConfiguration : IEntityTypeConfiguration<ExpenseBadge>
{
    public void Configure(EntityTypeBuilder<ExpenseBadge> builder)
    {
        builder.ToTable("ExpenseBadges");
        builder.HasKey(eb => new { eb.ExpenseId, eb.BadgeId });

        builder.HasOne<Expense>()
            .WithMany()
            .HasForeignKey(eb => eb.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Badge>()
            .WithMany()
            .HasForeignKey(eb => eb.BadgeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## JSON Storage File Structure

When using the JSON file provider, data is stored in the configured `DataDirectory`:

```text
data/
├── badges.json         # Array of Badge objects
└── expenses.json       # Array of Expense objects (each contains BadgeIds array)
```

**Note**: In JSON storage, `ExpenseBadge` is denormalized — each expense JSON object contains a `BadgeIds: Guid[]` array. The `ExpenseBadge` join entity is materialized only when hydrating domain objects and for EF Core mapping compatibility.
