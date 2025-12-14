using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class DocumentPersonConfiguration : IEntityTypeConfiguration<DocumentPerson>
{
    public void Configure(EntityTypeBuilder<DocumentPerson> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
        
        builder.HasOne(e => e.Document)
            .WithMany(d => d.DocumentPeople)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Person)
            .WithMany()
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Prevent duplicate associations
        builder.HasIndex(e => new { e.DocumentId, e.PersonId })
            .IsUnique();
    }
}
