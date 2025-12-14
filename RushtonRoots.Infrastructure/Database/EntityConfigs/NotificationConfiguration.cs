using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.UserId)
            .IsRequired();
        
        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Message)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(e => e.ActionUrl)
            .HasMaxLength(500);
        
        builder.Property(e => e.RelatedEntityType)
            .HasMaxLength(100);
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IsRead);
        builder.HasIndex(e => e.CreatedDateTime);
        builder.HasIndex(e => new { e.UserId, e.IsRead });
    }
}
