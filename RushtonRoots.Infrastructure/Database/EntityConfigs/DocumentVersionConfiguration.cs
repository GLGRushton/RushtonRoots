using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.DocumentUrl)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(e => e.BlobName)
            .HasMaxLength(500);
        
        builder.Property(e => e.ContentType)
            .HasMaxLength(100);
        
        builder.Property(e => e.ChangeNotes)
            .HasMaxLength(1000);
        
        builder.HasOne(e => e.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.UploadedBy)
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Ensure unique version numbers per document
        builder.HasIndex(e => new { e.DocumentId, e.VersionNumber })
            .IsUnique();
    }
}
