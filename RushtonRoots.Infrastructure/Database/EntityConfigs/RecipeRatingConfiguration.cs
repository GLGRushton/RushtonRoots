using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for RecipeRating
/// </summary>
public class RecipeRatingConfiguration : IEntityTypeConfiguration<RecipeRating>
{
    public void Configure(EntityTypeBuilder<RecipeRating> builder)
    {
        builder.HasKey(rr => rr.Id);

        builder.Property(rr => rr.RecipeId)
            .IsRequired();

        builder.Property(rr => rr.UserId)
            .IsRequired();

        builder.Property(rr => rr.Rating)
            .IsRequired();

        builder.Property(rr => rr.Comment)
            .HasMaxLength(2000);

        builder.Property(rr => rr.HasMade)
            .HasDefaultValue(false);

        // Unique constraint: one rating per user per recipe
        builder.HasIndex(rr => new { rr.RecipeId, rr.UserId })
            .IsUnique();

        // Relationships
        builder.HasOne(rr => rr.Recipe)
            .WithMany(r => r.Ratings)
            .HasForeignKey(rr => rr.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rr => rr.User)
            .WithMany()
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
