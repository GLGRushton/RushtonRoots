using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for ParentChild.
/// </summary>
public class ParentChildConfiguration : IEntityTypeConfiguration<ParentChild>
{
    public void Configure(EntityTypeBuilder<ParentChild> builder)
    {
        builder.HasKey(pc => pc.Id);
        
        builder.Property(pc => pc.Id)
            .ValueGeneratedOnAdd();
            
        builder.Property(pc => pc.ParentPersonId)
            .IsRequired();
            
        builder.Property(pc => pc.ChildPersonId)
            .IsRequired();
            
        builder.Property(pc => pc.RelationshipType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(pc => pc.CreatedDateTime)
            .IsRequired();
            
        builder.Property(pc => pc.UpdatedDateTime)
            .IsRequired();
        
        // Unique constraint on (ParentPersonId, ChildPersonId)
        builder.HasIndex(pc => new { pc.ParentPersonId, pc.ChildPersonId })
            .IsUnique();
        
        // Relationship: ParentChild has one ParentPerson
        builder.HasOne(pc => pc.ParentPerson)
            .WithMany(p => p.ParentRelationships)
            .HasForeignKey(pc => pc.ParentPersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Relationship: ParentChild has one ChildPerson
        builder.HasOne(pc => pc.ChildPerson)
            .WithMany(p => p.ChildRelationships)
            .HasForeignKey(pc => pc.ChildPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
