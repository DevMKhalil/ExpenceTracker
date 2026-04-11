using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenceTracker.Modules.Expenses.Domain;

namespace ExpenceTracker.Modules.Expenses.Infrastructure
{
    public class ExpenseEntityConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Amount).HasPrecision(12, 2).IsRequired();
            builder.Property(e => e.Date).IsRequired();
            builder.Property(e => e.Importance).HasConversion<int>().IsRequired();
            builder.Property(e => e.Notes).HasMaxLength(1000);
            builder.Property(e => e.IsPending).HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            builder.HasIndex(e => e.Date);
            builder.HasIndex(e => e.IsPending);
            builder.Ignore(e => e.BadgeIds); // EF Core uses join entity
        }
    }
}
