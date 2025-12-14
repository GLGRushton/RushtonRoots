using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class MediaPersonConfiguration : IEntityTypeConfiguration<MediaPerson>
{
    public void Configure(EntityTypeBuilder<MediaPerson> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
        
        builder.HasOne(e => e.Media)
            .WithMany(m => m.MediaPeople)
            .HasForeignKey(e => e.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Unique constraint to prevent duplicate media-person associations
        builder.HasIndex(e => new { e.MediaId, e.PersonId })
            .IsUnique();
    }
}
