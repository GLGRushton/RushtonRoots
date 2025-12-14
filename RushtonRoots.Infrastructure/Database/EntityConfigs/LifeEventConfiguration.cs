using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class LifeEventConfiguration : IEntityTypeConfiguration<LifeEvent>
{
    public void Configure(EntityTypeBuilder<LifeEvent> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.Description)
            .HasMaxLength(2000);
            
        builder.HasOne(e => e.Person)
            .WithMany(p => p.LifeEvents)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Location)
            .WithMany(l => l.LifeEvents)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(e => e.Source)
            .WithMany(s => s.LifeEvents)
            .HasForeignKey(e => e.SourceId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasIndex(e => e.PersonId);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.EventDate);
    }
}
