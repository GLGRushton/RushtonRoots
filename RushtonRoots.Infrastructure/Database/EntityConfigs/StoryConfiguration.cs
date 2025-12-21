using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Story
/// </summary>
public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(s => s.Slug)
            .IsUnique();

        builder.Property(s => s.Content)
            .IsRequired();

        builder.Property(s => s.Summary)
            .HasMaxLength(1000);

        builder.Property(s => s.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Location)
            .HasMaxLength(500);

        builder.Property(s => s.SubmittedByUserId)
            .IsRequired();

        builder.Property(s => s.IsPublished)
            .HasDefaultValue(false);

        builder.Property(s => s.ViewCount)
            .HasDefaultValue(0);

        builder.Property(s => s.AllowCollaboration)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(s => s.SubmittedByUser)
            .WithMany()
            .HasForeignKey(s => s.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Collection)
            .WithMany(c => c.Stories)
            .HasForeignKey(s => s.CollectionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.StoryPeople)
            .WithOne(sp => sp.Story)
            .HasForeignKey(sp => sp.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Performance indexes
        builder.HasIndex(s => s.Category); // For category filtering
        builder.HasIndex(s => s.IsPublished); // For published filtering
        builder.HasIndex(s => s.SubmittedByUserId); // For user story queries
        builder.HasIndex(s => s.CollectionId); // For collection queries
        builder.HasIndex(s => new { s.IsPublished, s.CreatedDateTime }); // For published listings
    }
}
