using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ContributionScoreConfiguration : IEntityTypeConfiguration<ContributionScore>
{
    public void Configure(EntityTypeBuilder<ContributionScore> builder)
    {
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.UserId)
            .IsRequired();

        builder.Property(cs => cs.TotalPoints)
            .HasDefaultValue(0);

        builder.Property(cs => cs.ContributionsSubmitted)
            .HasDefaultValue(0);

        builder.Property(cs => cs.ContributionsApproved)
            .HasDefaultValue(0);

        builder.Property(cs => cs.ContributionsRejected)
            .HasDefaultValue(0);

        builder.Property(cs => cs.CitationsAdded)
            .HasDefaultValue(0);

        builder.Property(cs => cs.ConflictsResolved)
            .HasDefaultValue(0);

        builder.Property(cs => cs.PeopleAdded)
            .HasDefaultValue(0);

        builder.Property(cs => cs.PhotosUploaded)
            .HasDefaultValue(0);

        builder.Property(cs => cs.StoriesWritten)
            .HasDefaultValue(0);

        builder.Property(cs => cs.CurrentRank)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Novice");

        // Relationships
        builder.HasOne(cs => cs.User)
            .WithMany()
            .HasForeignKey(cs => cs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cs => cs.UserId)
            .IsUnique(); // One score record per user
        builder.HasIndex(cs => cs.TotalPoints);
        builder.HasIndex(cs => cs.LastActivityDate);
    }
}
