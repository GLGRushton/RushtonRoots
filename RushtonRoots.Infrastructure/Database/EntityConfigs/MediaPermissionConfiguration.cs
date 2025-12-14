using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class MediaPermissionConfiguration : IEntityTypeConfiguration<MediaPermission>
{
    public void Configure(EntityTypeBuilder<MediaPermission> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.PermissionLevel)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasOne(e => e.Media)
            .WithMany(m => m.MediaPermissions)
            .HasForeignKey(e => e.MediaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Household)
            .WithMany()
            .HasForeignKey(e => e.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
