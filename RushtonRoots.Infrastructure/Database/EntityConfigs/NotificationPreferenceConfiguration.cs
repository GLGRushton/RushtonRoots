using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.UserId)
            .IsRequired();
        
        builder.Property(e => e.NotificationType)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.EmailFrequency)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(e => new { e.UserId, e.NotificationType })
            .IsUnique();
        
        builder.HasIndex(e => e.UserId);
    }
}
