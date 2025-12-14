using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ContributionApprovalConfiguration : IEntityTypeConfiguration<ContributionApproval>
{
    public void Configure(EntityTypeBuilder<ContributionApproval> builder)
    {
        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.ContributionId)
            .IsRequired();

        builder.Property(ca => ca.ApproverUserId)
            .IsRequired();

        builder.Property(ca => ca.Decision)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ca => ca.Notes)
            .HasMaxLength(1000);

        builder.Property(ca => ca.DecisionDate)
            .IsRequired();

        // Relationships
        builder.HasOne(ca => ca.Contribution)
            .WithMany(c => c.Approvals)
            .HasForeignKey(ca => ca.ContributionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.Approver)
            .WithMany()
            .HasForeignKey(ca => ca.ApproverUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ca => ca.ContributionId);
        builder.HasIndex(ca => ca.ApproverUserId);
        builder.HasIndex(ca => ca.DecisionDate);
    }
}
