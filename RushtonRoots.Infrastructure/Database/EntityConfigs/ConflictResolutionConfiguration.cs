using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ConflictResolutionConfiguration : IEntityTypeConfiguration<ConflictResolution>
{
    public void Configure(EntityTypeBuilder<ConflictResolution> builder)
    {
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cr => cr.EntityId)
            .IsRequired();

        builder.Property(cr => cr.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cr => cr.ConflictType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cr => cr.CurrentValue)
            .HasMaxLength(2000);

        builder.Property(cr => cr.ConflictingValue)
            .HasMaxLength(2000);

        builder.Property(cr => cr.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.Property(cr => cr.Resolution)
            .HasMaxLength(50);

        builder.Property(cr => cr.ResolutionNotes)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(cr => cr.Contribution)
            .WithMany(c => c.Conflicts)
            .HasForeignKey(cr => cr.ContributionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cr => cr.ResolvedBy)
            .WithMany()
            .HasForeignKey(cr => cr.ResolvedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.AcceptedCitation)
            .WithMany()
            .HasForeignKey(cr => cr.AcceptedCitationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(cr => new { cr.EntityType, cr.EntityId });
        builder.HasIndex(cr => cr.Status);
        builder.HasIndex(cr => cr.ContributionId);
    }
}
