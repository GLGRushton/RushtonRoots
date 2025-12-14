using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class EventRsvpConfiguration : IEntityTypeConfiguration<EventRsvp>
{
    public void Configure(EntityTypeBuilder<EventRsvp> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FamilyEventId)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.HasOne(e => e.FamilyEvent)
            .WithMany(ev => ev.Rsvps)
            .HasForeignKey(e => e.FamilyEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.FamilyEventId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.FamilyEventId, e.UserId })
            .IsUnique();
    }
}
