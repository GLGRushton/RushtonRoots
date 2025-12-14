using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class WikiCategoryConfiguration : IEntityTypeConfiguration<WikiCategory>
{
    public void Configure(EntityTypeBuilder<WikiCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        builder.Property(c => c.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Self-referencing relationship for parent-child categories
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.WikiPages)
            .WithOne(w => w.Category)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
