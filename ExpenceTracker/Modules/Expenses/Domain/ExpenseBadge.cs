using System;

namespace ExpenceTracker.Modules.Expenses.Domain
{
    public class ExpenseBadge
    {
        public Guid ExpenseId { get; set; }
        public Guid BadgeId { get; set; }
    }
}
