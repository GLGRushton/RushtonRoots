using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.MediaUrl)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(500);
        
        builder.Property(e => e.MediaType)
            .IsRequired();
        
        builder.Property(e => e.BlobName)
            .HasMaxLength(500);
        
        builder.Property(e => e.ContentType)
            .HasMaxLength(100);
        
        builder.Property(e => e.Transcription);
        
        builder.HasOne(e => e.UploadedBy)
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(e => e.TimelineMarkers)
            .WithOne(m => m.Media)
            .HasForeignKey(m => m.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.MediaPeople)
            .WithOne(mp => mp.Media)
            .HasForeignKey(mp => mp.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.MediaPermissions)
            .WithOne(p => p.Media)
            .HasForeignKey(p => p.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
