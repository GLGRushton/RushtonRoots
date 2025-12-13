using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(e => e.AddressLine1)
            .HasMaxLength(200);
            
        builder.Property(e => e.AddressLine2)
            .HasMaxLength(200);
            
        builder.Property(e => e.City)
            .HasMaxLength(100);
            
        builder.Property(e => e.State)
            .HasMaxLength(100);
            
        builder.Property(e => e.Country)
            .HasMaxLength(100);
            
        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);
            
        builder.Property(e => e.Latitude)
            .HasPrecision(10, 7);
            
        builder.Property(e => e.Longitude)
            .HasPrecision(10, 7);
            
        builder.HasIndex(e => e.City);
        builder.HasIndex(e => e.Country);
    }
}
