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
    public DbSet<LifeEvent> LifeEvents { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PersonPhoto> PersonPhotos { get; set; }
    public DbSet<PhotoAlbum> PhotoAlbums { get; set; }
    public DbSet<PhotoTag> PhotoTags { get; set; }
    public DbSet<PhotoPermission> PhotoPermissions { get; set; }
    public DbSet<BiographicalNote> BiographicalNotes { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Citation> Citations { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentVersion> DocumentVersions { get; set; }
    public DbSet<DocumentPerson> DocumentPeople { get; set; }
    public DbSet<DocumentPermission> DocumentPermissions { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<MediaTimelineMarker> MediaTimelineMarkers { get; set; }
    public DbSet<MediaPerson> MediaPeople { get; set; }
    public DbSet<MediaPermission> MediaPermissions { get; set; }

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
