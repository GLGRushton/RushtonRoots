using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class PhotoTagConfiguration : IEntityTypeConfiguration<PhotoTag>
{
    public void Configure(EntityTypeBuilder<PhotoTag> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
        
        builder.HasOne(e => e.PersonPhoto)
            .WithMany(p => p.PhotoTags)
            .HasForeignKey(e => e.PersonPhotoId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Create index for efficient lookups
        builder.HasIndex(e => e.PersonPhotoId);
        builder.HasIndex(e => e.PersonId);
    }
}
