using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Author)
            .HasMaxLength(200);
            
        builder.Property(e => e.Publisher)
            .HasMaxLength(200);
            
        builder.Property(e => e.RepositoryName)
            .HasMaxLength(200);
            
        builder.Property(e => e.RepositoryUrl)
            .HasMaxLength(500);
            
        builder.Property(e => e.CallNumber)
            .HasMaxLength(100);
            
        builder.Property(e => e.SourceType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Notes)
            .HasMaxLength(2000);
            
        builder.HasIndex(e => e.SourceType);
        builder.HasIndex(e => e.Title);
    }
}
