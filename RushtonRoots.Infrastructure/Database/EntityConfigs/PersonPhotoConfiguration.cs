using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class PersonPhotoConfiguration : IEntityTypeConfiguration<PersonPhoto>
{
    public void Configure(EntityTypeBuilder<PersonPhoto> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.PhotoUrl)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Caption)
            .HasMaxLength(500);
            
        builder.HasOne(e => e.Person)
            .WithMany(p => p.Photos)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(e => e.PersonId);
        builder.HasIndex(e => new { e.PersonId, e.IsPrimary });
    }
}
