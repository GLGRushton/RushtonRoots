using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class ActivityFeedItemConfiguration : IEntityTypeConfiguration<ActivityFeedItem>
{
    public void Configure(EntityTypeBuilder<ActivityFeedItem> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.ActivityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityType)
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.ActionUrl)
            .HasMaxLength(500);

        builder.Property(a => a.Points)
            .HasDefaultValue(0);

        builder.Property(a => a.IsPublic)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.ActivityType);
        builder.HasIndex(a => a.CreatedDateTime);
        builder.HasIndex(a => a.IsPublic);
    }
}
