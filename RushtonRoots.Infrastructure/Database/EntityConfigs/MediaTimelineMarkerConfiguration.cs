using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class MediaTimelineMarkerConfiguration : IEntityTypeConfiguration<MediaTimelineMarker>
{
    public void Configure(EntityTypeBuilder<MediaTimelineMarker> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        
        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(500);
        
        builder.HasOne(e => e.Media)
            .WithMany(m => m.TimelineMarkers)
            .HasForeignKey(e => e.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
