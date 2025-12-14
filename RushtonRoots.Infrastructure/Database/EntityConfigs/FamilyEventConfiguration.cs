using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class FamilyEventConfiguration : IEntityTypeConfiguration<FamilyEvent>
{
    public void Configure(EntityTypeBuilder<FamilyEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.StartDateTime)
            .IsRequired();

        builder.Property(e => e.Location)
            .HasMaxLength(500);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.RecurrencePattern)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedByUserId)
            .IsRequired();

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Household)
            .WithMany()
            .HasForeignKey(e => e.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Rsvps)
            .WithOne(r => r.FamilyEvent)
            .HasForeignKey(r => r.FamilyEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.CreatedByUserId);
        builder.HasIndex(e => e.HouseholdId);
        builder.HasIndex(e => e.StartDateTime);
        builder.HasIndex(e => e.EventType);
    }
}
