using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class CitationConfiguration : IEntityTypeConfiguration<Citation>
{
    public void Configure(EntityTypeBuilder<Citation> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.PageNumber)
            .HasMaxLength(50);
            
        builder.Property(e => e.Quote)
            .HasMaxLength(2000);
            
        builder.Property(e => e.TranscriptionOrSummary)
            .HasMaxLength(2000);
            
        builder.Property(e => e.AccessedDate)
            .HasMaxLength(50);
            
        builder.Property(e => e.Url)
            .HasMaxLength(500);
            
        builder.HasOne(e => e.Source)
            .WithMany(s => s.Citations)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(e => e.SourceId);
    }
}
