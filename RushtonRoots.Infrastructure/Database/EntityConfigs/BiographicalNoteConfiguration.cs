using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class BiographicalNoteConfiguration : IEntityTypeConfiguration<BiographicalNote>
{
    public void Configure(EntityTypeBuilder<BiographicalNote> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.Content)
            .IsRequired();
            
        builder.Property(e => e.AuthorName)
            .HasMaxLength(200);
            
        builder.HasOne(e => e.Person)
            .WithMany(p => p.BiographicalNotes)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Source)
            .WithMany(s => s.BiographicalNotes)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasIndex(e => e.PersonId);
    }
}
