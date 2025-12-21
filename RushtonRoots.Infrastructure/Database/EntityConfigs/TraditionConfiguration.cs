using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Tradition
/// </summary>
public class TraditionConfiguration : IEntityTypeConfiguration<Tradition>
{
    public void Configure(EntityTypeBuilder<Tradition> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Description)
            .IsRequired();

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Frequency)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Active");

        builder.Property(t => t.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(t => t.HowToCelebrate)
            .HasMaxLength(2000);

        builder.Property(t => t.AssociatedItems)
            .HasMaxLength(1000);

        builder.Property(t => t.SubmittedByUserId)
            .IsRequired();

        builder.Property(t => t.IsPublished)
            .HasDefaultValue(false);

        builder.Property(t => t.ViewCount)
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(t => t.SubmittedByUser)
            .WithMany()
            .HasForeignKey(t => t.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.StartedByPerson)
            .WithMany()
            .HasForeignKey(t => t.StartedByPersonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Timeline)
            .WithOne(tt => tt.Tradition)
            .HasForeignKey(tt => tt.TraditionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Performance indexes
        builder.HasIndex(t => t.Category); // For category filtering
        builder.HasIndex(t => t.Status); // For status filtering
        builder.HasIndex(t => t.IsPublished); // For published filtering
        builder.HasIndex(t => t.SubmittedByUserId); // For user tradition queries
        builder.HasIndex(t => new { t.IsPublished, t.Status }); // For active tradition listings
    }
}
