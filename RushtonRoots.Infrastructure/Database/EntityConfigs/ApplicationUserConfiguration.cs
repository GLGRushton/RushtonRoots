using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for ApplicationUser (extends IdentityUser).
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(au => au.PersonId);
        
        // PersonId is unique - one person can have only one user account
        builder.HasIndex(au => au.PersonId)
            .IsUnique()
            .HasFilter("[PersonId] IS NOT NULL");
        
        // Relationship: ApplicationUser has one Person
        builder.HasOne(au => au.Person)
            .WithMany()
            .HasForeignKey(au => au.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
