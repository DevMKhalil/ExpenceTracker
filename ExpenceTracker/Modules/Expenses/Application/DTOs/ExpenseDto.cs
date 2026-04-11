using System;
using System.Collections.Generic;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Application.DTOs
{
    public record ExpenseDto(
        Guid Id,
        string Name,
        decimal Amount,
        DateTimeOffset Date,
        ImportanceLevel Importance,
        string? Notes,
        bool IsPending,
        List<Guid> BadgeIds,
        DateTimeOffset CreatedAt,
        List<ExpenseDto.BadgeInfo> Badges
    )
    {
        public record BadgeInfo(Guid Id, string Name, string Color, bool IsDeleted);
    }
}
