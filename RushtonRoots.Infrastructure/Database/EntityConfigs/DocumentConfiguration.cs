using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.DocumentUrl)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(e => e.ThumbnailUrl)
            .HasMaxLength(500);
        
        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.BlobName)
            .HasMaxLength(500);
        
        builder.Property(e => e.ContentType)
            .HasMaxLength(100);
        
        builder.HasOne(e => e.UploadedBy)
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(e => e.Versions)
            .WithOne(v => v.Document)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.DocumentPeople)
            .WithOne(dp => dp.Document)
            .HasForeignKey(dp => dp.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.DocumentPermissions)
            .WithOne(p => p.Document)
            .HasForeignKey(p => p.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
