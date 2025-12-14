using Autofac;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Web;

/// <summary>
/// Autofac module for dependency injection configuration.
/// This keeps Program.cs clean by centralizing all DI registration.
/// </summary>
public class AutofacModule : Module
{
    private readonly IConfiguration _configuration;

    public AutofacModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // Register DbContext
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<RushtonRootsDbContext>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            // Use SQLite if running in Linux/CI environment, otherwise use SQL Server
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                optionsBuilder.UseSqlite("Data Source=rushtonroots.db");
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            
            return new RushtonRootsDbContext(optionsBuilder.Options);
        })
        .AsSelf()
        .InstancePerLifetimeScope();

        // Register repositories from Infrastructure
        // Example: builder.RegisterType<PersonRepository>().As<IPersonRepository>().InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(typeof(RushtonRootsDbContext).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Register infrastructure services (BlobStorageService, etc.)
        builder.RegisterAssemblyTypes(typeof(RushtonRootsDbContext).Assembly)
            .Where(t => t.Name.EndsWith("Service") && t.Namespace != null && t.Namespace.Contains("Infrastructure"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Register services from Application
        // Example: builder.RegisterType<PersonService>().As<IPersonService>().InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(typeof(RushtonRoots.Application.AssemblyMarker).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Register validators from Application
        builder.RegisterAssemblyTypes(typeof(RushtonRoots.Application.AssemblyMarker).Assembly)
            .Where(t => t.Name.EndsWith("Validator"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Register mappers from Application
        builder.RegisterAssemblyTypes(typeof(RushtonRoots.Application.AssemblyMarker).Assembly)
            .Where(t => t.Name.EndsWith("Mapper"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
