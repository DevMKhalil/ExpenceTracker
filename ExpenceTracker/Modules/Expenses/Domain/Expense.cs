using System;
using System.Collections.Generic;
using ExpenceTracker.Shared.Domain;

namespace ExpenceTracker.Modules.Expenses.Domain
{
    public class Expense : Entity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
        public ImportanceLevel Importance { get; set; } = ImportanceLevel.Normal;
        public string? Notes { get; set; }
        public bool IsPending { get; set; } = false;
        
        // This is the denormalized form for JSON storage
        public List<Guid> BadgeIds { get; set; } = new List<Guid>();
    }
}
