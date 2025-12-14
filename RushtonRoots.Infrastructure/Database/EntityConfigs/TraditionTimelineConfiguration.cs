using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for TraditionTimeline
/// </summary>
public class TraditionTimelineConfiguration : IEntityTypeConfiguration<TraditionTimeline>
{
    public void Configure(EntityTypeBuilder<TraditionTimeline> builder)
    {
        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.TraditionId)
            .IsRequired();

        builder.Property(tt => tt.EventDate)
            .IsRequired();

        builder.Property(tt => tt.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(tt => tt.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tt => tt.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tt => tt.RecordedByUserId)
            .IsRequired();

        builder.Property(tt => tt.PhotoUrl)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(tt => tt.Tradition)
            .WithMany(t => t.Timeline)
            .HasForeignKey(tt => tt.TraditionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tt => tt.RecordedByUser)
            .WithMany()
            .HasForeignKey(tt => tt.RecordedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
