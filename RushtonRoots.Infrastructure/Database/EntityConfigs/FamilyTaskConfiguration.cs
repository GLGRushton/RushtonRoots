using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class FamilyTaskConfiguration : IEntityTypeConfiguration<FamilyTask>
{
    public void Configure(EntityTypeBuilder<FamilyTask> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Priority)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AssignedToUser)
            .WithMany()
            .HasForeignKey(e => e.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Household)
            .WithMany()
            .HasForeignKey(e => e.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RelatedEvent)
            .WithMany()
            .HasForeignKey(e => e.RelatedEventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CreatedByUserId);
        builder.HasIndex(e => e.AssignedToUserId);
        builder.HasIndex(e => e.HouseholdId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.DueDate);
    }
}
