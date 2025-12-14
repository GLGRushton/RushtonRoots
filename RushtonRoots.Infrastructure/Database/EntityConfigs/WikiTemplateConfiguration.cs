using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class WikiTemplateConfiguration : IEntityTypeConfiguration<WikiTemplate>
{
    public void Configure(EntityTypeBuilder<WikiTemplate> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.TemplateContent)
            .IsRequired();

        builder.Property(t => t.TemplateType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasMany(t => t.WikiPages)
            .WithOne(w => w.Template)
            .HasForeignKey(w => w.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
