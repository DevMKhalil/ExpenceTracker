using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenceTracker.Modules.Expenses.Domain;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Expenses.Infrastructure
{
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
}
