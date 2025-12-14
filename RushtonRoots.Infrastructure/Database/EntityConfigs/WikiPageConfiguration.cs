using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class WikiPageConfiguration : IEntityTypeConfiguration<WikiPage>
{
    public void Configure(EntityTypeBuilder<WikiPage> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(w => w.Slug)
            .IsUnique();

        builder.Property(w => w.Content)
            .IsRequired();

        builder.Property(w => w.Summary)
            .HasMaxLength(500);

        builder.Property(w => w.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(w => w.LastUpdatedByUserId)
            .HasMaxLength(450);

        builder.Property(w => w.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(w => w.Category)
            .WithMany(c => c.WikiPages)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(w => w.Template)
            .WithMany(t => t.WikiPages)
            .HasForeignKey(w => w.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(w => w.CreatedByUser)
            .WithMany()
            .HasForeignKey(w => w.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.LastUpdatedByUser)
            .WithMany()
            .HasForeignKey(w => w.LastUpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Versions)
            .WithOne(v => v.WikiPage)
            .HasForeignKey(v => v.WikiPageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many relationship with WikiTag
        builder.HasMany(w => w.Tags)
            .WithMany(t => t.WikiPages)
            .UsingEntity(j => j.ToTable("WikiPageTags"));
    }
}
