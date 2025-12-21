using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Person.
/// </summary>
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(p => p.HouseholdId)
            .IsRequired();
            
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.DateOfBirth)
            .HasColumnType("date");
            
        builder.Property(p => p.DateOfDeath)
            .HasColumnType("date");
            
        builder.Property(p => p.IsDeceased)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);
            
        builder.Property(p => p.CreatedDateTime)
            .IsRequired();
            
        builder.Property(p => p.UpdatedDateTime)
            .IsRequired();
        
        // Relationship: Person belongs to one Household
        builder.HasOne(p => p.Household)
            .WithMany(h => h.Members)
            .HasForeignKey(p => p.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Person has many ParentRelationships (as child)
        builder.HasMany(p => p.ChildRelationships)
            .WithOne(pc => pc.ChildPerson)
            .HasForeignKey(pc => pc.ChildPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Person has many ParentRelationships (as parent)
        builder.HasMany(p => p.ParentRelationships)
            .WithOne(pc => pc.ParentPerson)
            .HasForeignKey(pc => pc.ParentPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Person has many PartnershipsAsPersonA
        builder.HasMany(p => p.PartnershipsAsPersonA)
            .WithOne(pa => pa.PersonA)
            .HasForeignKey(pa => pa.PersonAId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Person has many PartnershipsAsPersonB
        builder.HasMany(p => p.PartnershipsAsPersonB)
            .WithOne(pa => pa.PersonB)
            .HasForeignKey(pa => pa.PersonBId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: Person has many HouseholdPermissions
        builder.HasMany(p => p.HouseholdPermissions)
            .WithOne(hp => hp.Person)
            .HasForeignKey(hp => hp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Performance indexes
        builder.HasIndex(p => p.HouseholdId); // For household member queries
        builder.HasIndex(p => p.LastName); // For name search queries
        builder.HasIndex(p => new { p.LastName, p.FirstName }); // For ordered name queries
        builder.HasIndex(p => p.DateOfBirth); // For birth date filtering
        builder.HasIndex(p => p.IsDeceased); // For deceased filtering
    }
}
