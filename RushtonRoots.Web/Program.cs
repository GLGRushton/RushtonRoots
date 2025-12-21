using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Web;

var builder = WebApplication.CreateBuilder(args);

// Configure Autofac as the service provider
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacModule(builder.Configuration));
});

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure CORS if needed for external API access
// By default, the Angular app is served from the same origin, so CORS is not required
// Uncomment and configure if you need to allow external origins to access the API
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
*/

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RushtonRoots API",
        Version = "v1",
        Description = "API documentation for RushtonRoots - A comprehensive family platform",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "RushtonRoots Development Team"
        }
    });
    
    // Enable XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure Identity options as needed
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<RushtonRootsDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    
    // Security: Require HTTPS for cookies in production
    options.Cookie.SecurePolicy = builder.Environment.IsProduction() 
        ? CookieSecurePolicy.Always 
        : CookieSecurePolicy.SameAsRequest;
    
    // Security: Prevent client-side JavaScript access to auth cookies
    options.Cookie.HttpOnly = true;
    
    // Security: SameSite policy to prevent CSRF attacks
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure HSTS options for production
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365); // 1 year
        options.IncludeSubDomains = true;
        options.Preload = true;
    });
}

// Add Health Checks
var healthChecksBuilder = builder.Services.AddHealthChecks();

// Add database health check
healthChecksBuilder.AddDbContextCheck<RushtonRootsDbContext>(
    name: "database",
    tags: new[] { "db", "sql", "ready" });

// Add Azure Blob Storage health check
var azureStorageConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
if (!string.IsNullOrEmpty(azureStorageConnectionString) && 
    azureStorageConnectionString != "UseDevelopmentStorage=true")
{
    healthChecksBuilder.AddAzureBlobStorage(
        azureStorageConnectionString,
        name: "azurestorage",
        tags: new[] { "storage", "azure", "ready" });
}

var app = builder.Build();

// Run migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<RushtonRootsDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
    
    // Apply migrations
    dbContext.Database.Migrate();
    
    // Seed database
    var seeder = new DatabaseSeeder(dbContext, userManager, roleManager, logger);
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    // Configure HSTS (HTTP Strict Transport Security)
    // HSTS enforces HTTPS for 1 year, includes subdomains, and allows preload list inclusion
    app.UseHsts();
    
    // Force HTTPS redirect in production
    app.UseHttpsRedirection();
}
else
{
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RushtonRoots API v1");
        options.RoutePrefix = "api-docs"; // Access at /api-docs
        options.DocumentTitle = "RushtonRoots API Documentation";
    });
    
    // Optional in development - comment out if testing without SSL certificate
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

// Enable CORS if configured
// app.UseCors("AllowSpecificOrigins"); // Uncomment if CORS policy is configured above

app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers
app.MapControllers();

// Map Health Check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                exception = e.Value.Exception?.Message,
                data = e.Value.Data
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        
        await context.Response.WriteAsync(result);
    }
});

// Readiness probe - only checks critical services
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Liveness probe - simple check to verify app is running
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // No checks, just responds 200 if app is running
});

app.Run();
