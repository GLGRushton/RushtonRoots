using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Database;

public class RushtonRootsDbContext : IdentityDbContext<ApplicationUser>
{
    public RushtonRootsDbContext(DbContextOptions<RushtonRootsDbContext> options)
        : base(options)
    {
    }

    // DbSets for domain entities
    public DbSet<Household> Households { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<ParentChild> ParentChildren { get; set; }
    public DbSet<Partnership> Partnerships { get; set; }
    public DbSet<HouseholdPermission> HouseholdPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the EntityConfigs directory
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RushtonRootsDbContext).Assembly);
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDateTime = now;
                entry.Entity.UpdatedDateTime = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedDateTime = now;
            }
        }
    }
}
