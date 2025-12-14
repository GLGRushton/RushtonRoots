using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class WikiPageVersionConfiguration : IEntityTypeConfiguration<WikiPageVersion>
{
    public void Configure(EntityTypeBuilder<WikiPageVersion> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.WikiPageId)
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Content)
            .IsRequired();

        builder.Property(v => v.Summary)
            .HasMaxLength(500);

        builder.Property(v => v.UpdatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(v => v.ChangeDescription)
            .HasMaxLength(1000);

        // Composite index for finding versions
        builder.HasIndex(v => new { v.WikiPageId, v.VersionNumber })
            .IsUnique();

        // Relationships
        builder.HasOne(v => v.WikiPage)
            .WithMany(w => w.Versions)
            .HasForeignKey(v => v.WikiPageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.UpdatedByUser)
            .WithMany()
            .HasForeignKey(v => v.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
