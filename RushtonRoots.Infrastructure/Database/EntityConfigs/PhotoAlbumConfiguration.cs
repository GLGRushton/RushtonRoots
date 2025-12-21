using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class PhotoAlbumConfiguration : IEntityTypeConfiguration<PhotoAlbum>
{
    public void Configure(EntityTypeBuilder<PhotoAlbum> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.CoverPhotoUrl)
            .HasMaxLength(500);
        
        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(e => e.Photos)
            .WithOne(p => p.PhotoAlbum)
            .HasForeignKey(p => p.PhotoAlbumId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Performance indexes
        builder.HasIndex(e => e.CreatedByUserId); // For user album queries
        builder.HasIndex(e => e.IsPublic); // For public album filtering
        builder.HasIndex(e => new { e.DisplayOrder, e.CreatedDateTime }); // For ordered listing
    }
}
