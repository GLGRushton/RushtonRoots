using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class PhotoPermissionConfiguration : IEntityTypeConfiguration<PhotoPermission>
{
    public void Configure(EntityTypeBuilder<PhotoPermission> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.PermissionLevel)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasOne(e => e.PersonPhoto)
            .WithMany(p => p.PhotoPermissions)
            .HasForeignKey(e => e.PersonPhotoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.PhotoAlbum)
            .WithMany()
            .HasForeignKey(e => e.PhotoAlbumId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Household)
            .WithMany()
            .HasForeignKey(e => e.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Create indexes for efficient permission checks
        builder.HasIndex(e => new { e.PersonPhotoId, e.UserId });
        builder.HasIndex(e => new { e.PhotoAlbumId, e.UserId });
        builder.HasIndex(e => new { e.PersonPhotoId, e.HouseholdId });
        builder.HasIndex(e => new { e.PhotoAlbumId, e.HouseholdId });
    }
}
