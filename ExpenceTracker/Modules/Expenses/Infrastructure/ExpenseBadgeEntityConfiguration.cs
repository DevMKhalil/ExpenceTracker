using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Infrastructure
{
    public class ExpenseBadgeEntityConfiguration : IEntityTypeConfiguration<ExpenseBadge>
    {
        public void Configure(EntityTypeBuilder<ExpenseBadge> builder)
        {
            builder.ToTable("ExpenseBadges");
            builder.HasKey(eb => new { eb.ExpenseId, eb.BadgeId });
            
            // Note: navigation properties would be mapped here if Expense object had an ICollection<ExpenseBadge>.
            // Since this is just the mapping info for the Fluent API:
        }
    }
}
