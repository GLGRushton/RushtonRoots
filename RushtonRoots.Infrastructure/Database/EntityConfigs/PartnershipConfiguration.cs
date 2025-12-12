using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Partnership.
/// </summary>
public class PartnershipConfiguration : IEntityTypeConfiguration<Partnership>
{
    public void Configure(EntityTypeBuilder<Partnership> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(p => p.PersonAId)
            .IsRequired();
            
        builder.Property(p => p.PersonBId)
            .IsRequired();
            
        builder.Property(p => p.PartnershipType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.StartDate)
            .HasColumnType("date");
            
        builder.Property(p => p.EndDate)
            .HasColumnType("date");
            
        builder.Property(p => p.CreatedDateTime)
            .IsRequired();
            
        builder.Property(p => p.UpdatedDateTime)
            .IsRequired();
        
        // Unique constraint using computed columns for ordering
        // This ensures (A, B) and (B, A) are treated as duplicates
        builder.HasIndex(p => new { p.PersonAId, p.PersonBId })
            .IsUnique()
            .HasFilter("[PersonAId] < [PersonBId]");
            
        builder.HasIndex(p => new { p.PersonBId, p.PersonAId })
            .IsUnique()
            .HasFilter("[PersonBId] < [PersonAId]");
        
        // Relationship: Partnership has one PersonA
        builder.HasOne(p => p.PersonA)
            .WithMany(per => per.PartnershipsAsPersonA)
            .HasForeignKey(p => p.PersonAId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Partnership has one PersonB
        builder.HasOne(p => p.PersonB)
            .WithMany(per => per.PartnershipsAsPersonB)
            .HasForeignKey(p => p.PersonBId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
