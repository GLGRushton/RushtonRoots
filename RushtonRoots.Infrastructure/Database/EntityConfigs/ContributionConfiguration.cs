using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ContributionConfiguration : IEntityTypeConfiguration<Contribution>
{
    public void Configure(EntityTypeBuilder<Contribution> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.EntityId)
            .IsRequired();

        builder.Property(c => c.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.OldValue)
            .HasMaxLength(2000);

        builder.Property(c => c.NewValue)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.ContributorUserId)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(c => c.ReviewNotes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(c => c.Contributor)
            .WithMany()
            .HasForeignKey(c => c.ContributorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Reviewer)
            .WithMany()
            .HasForeignKey(c => c.ReviewerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Citation)
            .WithMany()
            .HasForeignKey(c => c.CitationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Approvals)
            .WithOne(a => a.Contribution)
            .HasForeignKey(a => a.ContributionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Conflicts)
            .WithOne(cr => cr.Contribution)
            .HasForeignKey(cr => cr.ContributionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => new { c.EntityType, c.EntityId });
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ContributorUserId);
        builder.HasIndex(c => c.CreatedDateTime);
    }
}
