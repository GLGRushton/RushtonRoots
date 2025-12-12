using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for HouseholdPermission.
/// </summary>
public class HouseholdPermissionConfiguration : IEntityTypeConfiguration<HouseholdPermission>
{
    public void Configure(EntityTypeBuilder<HouseholdPermission> builder)
    {
        builder.HasKey(hp => hp.Id);
        
        builder.Property(hp => hp.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(hp => hp.HouseholdId)
            .IsRequired();
            
        builder.Property(hp => hp.PersonId)
            .IsRequired();
            
        builder.Property(hp => hp.Role)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(hp => hp.CreatedDateTime)
            .IsRequired();
            
        builder.Property(hp => hp.UpdatedDateTime)
            .IsRequired();
        
        // Unique constraint on (HouseholdId, PersonId) - one person can have only one role per household
        builder.HasIndex(hp => new { hp.HouseholdId, hp.PersonId })
            .IsUnique();
        
        // Relationship: HouseholdPermission has one Household
        builder.HasOne(hp => hp.Household)
            .WithMany(h => h.Permissions)
            .HasForeignKey(hp => hp.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Relationship: HouseholdPermission has one Person
        builder.HasOne(hp => hp.Person)
            .WithMany(p => p.HouseholdPermissions)
            .HasForeignKey(hp => hp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
