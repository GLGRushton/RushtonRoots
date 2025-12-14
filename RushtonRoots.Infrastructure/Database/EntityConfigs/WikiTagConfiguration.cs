using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class WikiTagConfiguration : IEntityTypeConfiguration<WikiTag>
{
    public void Configure(EntityTypeBuilder<WikiTag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(60);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Description)
            .HasMaxLength(200);

        builder.Property(t => t.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Many-to-many relationship with WikiPage (configured in WikiPageConfiguration)
    }
}
