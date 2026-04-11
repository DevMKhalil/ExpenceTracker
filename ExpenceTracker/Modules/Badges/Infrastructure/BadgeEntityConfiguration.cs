using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Infrastructure
{
    public class BadgeEntityConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Color).IsRequired().HasMaxLength(7);
            builder.Property(b => b.IsDeleted).HasDefaultValue(false);
            builder.Property(b => b.CreatedAt).IsRequired();
            builder.Property(b => b.UpdatedAt).IsRequired();

            builder.HasIndex(b => b.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }
    }
}
