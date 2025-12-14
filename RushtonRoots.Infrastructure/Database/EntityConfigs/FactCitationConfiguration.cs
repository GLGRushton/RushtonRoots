using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class FactCitationConfiguration : IEntityTypeConfiguration<FactCitation>
{
    public void Configure(EntityTypeBuilder<FactCitation> builder)
    {
        builder.HasKey(fc => fc.Id);

        builder.Property(fc => fc.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fc => fc.EntityId)
            .IsRequired();

        builder.Property(fc => fc.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fc => fc.CitationId)
            .IsRequired();

        builder.Property(fc => fc.ConfidenceLevel)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Medium");

        builder.Property(fc => fc.Notes)
            .HasMaxLength(1000);

        builder.Property(fc => fc.AddedByUserId)
            .IsRequired();

        // Relationships
        builder.HasOne(fc => fc.Citation)
            .WithMany()
            .HasForeignKey(fc => fc.CitationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fc => fc.AddedBy)
            .WithMany()
            .HasForeignKey(fc => fc.AddedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(fc => new { fc.EntityType, fc.EntityId, fc.FieldName });
        builder.HasIndex(fc => fc.CitationId);
    }
}
