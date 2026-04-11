using System;
using System.Collections.Generic;

namespace ExpenceTracker.Modules.Expenses.Application.DTOs
{
    public record BadgeBreakdownItem(Guid BadgeId, string BadgeName, string BadgeColor, decimal Total);

    public record DashboardSummaryDto(decimal DailyTotal, decimal MonthlyTotal, decimal PendingTotal, List<BadgeBreakdownItem> BadgeBreakdown);
}
