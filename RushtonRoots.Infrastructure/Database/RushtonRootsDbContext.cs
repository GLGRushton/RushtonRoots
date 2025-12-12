using Microsoft.EntityFrameworkCore;

namespace RushtonRoots.Infrastructure.Database;

public class RushtonRootsDbContext : DbContext
{
    public RushtonRootsDbContext(DbContextOptions<RushtonRootsDbContext> options)
        : base(options)
    {
    }

    // DbSets will be added here as entities are created
    // Example: public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the EntityConfigs directory
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RushtonRootsDbContext).Assembly);
    }
}
