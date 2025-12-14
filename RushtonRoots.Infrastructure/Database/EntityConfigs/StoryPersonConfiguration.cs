using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for StoryPerson join table
/// </summary>
public class StoryPersonConfiguration : IEntityTypeConfiguration<StoryPerson>
{
    public void Configure(EntityTypeBuilder<StoryPerson> builder)
    {
        builder.HasKey(sp => new { sp.StoryId, sp.PersonId });

        builder.Property(sp => sp.RoleInStory)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(sp => sp.Story)
            .WithMany(s => s.StoryPeople)
            .HasForeignKey(sp => sp.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.Person)
            .WithMany(p => p.StoryPeople)
            .HasForeignKey(sp => sp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
