using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for StoryCollection
/// </summary>
public class StoryCollectionConfiguration : IEntityTypeConfiguration<StoryCollection>
{
    public void Configure(EntityTypeBuilder<StoryCollection> builder)
    {
        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sc => sc.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(sc => sc.Slug)
            .IsUnique();

        builder.Property(sc => sc.Description)
            .HasMaxLength(2000);

        builder.Property(sc => sc.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(sc => sc.CreatedByUserId)
            .IsRequired();

        builder.Property(sc => sc.IsPublished)
            .HasDefaultValue(false);

        builder.Property(sc => sc.DisplayOrder)
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(sc => sc.CreatedByUser)
            .WithMany()
            .HasForeignKey(sc => sc.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sc => sc.Stories)
            .WithOne(s => s.Collection)
            .HasForeignKey(s => s.CollectionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
