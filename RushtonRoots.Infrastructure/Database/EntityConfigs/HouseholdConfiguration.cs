using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Household.
/// </summary>
public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(h => h.HouseholdName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(h => h.CreatedDateTime)
            .IsRequired();
            
        builder.Property(h => h.UpdatedDateTime)
            .IsRequired();
        
        // AnchorPersonId is unique - one person can only anchor one household
        builder.HasIndex(h => h.AnchorPersonId)
            .IsUnique();
        
        // Relationship: Household has one AnchorPerson
        builder.HasOne(h => h.AnchorPerson)
            .WithMany()
            .HasForeignKey(h => h.AnchorPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Household has many Members
        builder.HasMany(h => h.Members)
            .WithOne(p => p.Household)
            .HasForeignKey(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Household has many Permissions
        builder.HasMany(h => h.Permissions)
            .WithOne(hp => hp.Household)
            .HasForeignKey(hp => hp.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
