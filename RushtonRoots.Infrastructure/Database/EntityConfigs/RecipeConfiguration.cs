using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

/// <summary>
/// Entity configuration for Recipe
/// </summary>
public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        builder.Property(r => r.Ingredients)
            .IsRequired();

        builder.Property(r => r.Instructions)
            .IsRequired();

        builder.Property(r => r.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Cuisine)
            .HasMaxLength(100);

        builder.Property(r => r.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        builder.Property(r => r.SubmittedByUserId)
            .IsRequired();

        builder.Property(r => r.IsPublished)
            .HasDefaultValue(false);

        builder.Property(r => r.IsFavorite)
            .HasDefaultValue(false);

        builder.Property(r => r.AverageRating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.RatingCount)
            .HasDefaultValue(0);

        builder.Property(r => r.ViewCount)
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(r => r.SubmittedByUser)
            .WithMany()
            .HasForeignKey(r => r.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.OriginatorPerson)
            .WithMany()
            .HasForeignKey(r => r.OriginatorPersonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(r => r.Ratings)
            .WithOne(rr => rr.Recipe)
            .HasForeignKey(rr => rr.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Performance indexes
        builder.HasIndex(r => r.Category); // For category filtering
        builder.HasIndex(r => r.IsPublished); // For published filtering
        builder.HasIndex(r => r.IsFavorite); // For favorite filtering
        builder.HasIndex(r => r.SubmittedByUserId); // For user recipe queries
        builder.HasIndex(r => new { r.IsPublished, r.AverageRating }); // For top-rated listings
    }
}
