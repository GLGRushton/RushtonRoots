# RushtonRoots
A comprehensive family platform designed to serve multiple generations.

## Overview
This is a .NET 10 ASP.NET Core + Angular application hosted on a single port. The application demonstrates:
- Single web host serving API controllers, UI controllers, and Razor views
- Angular components integrated within Razor views
- Angular Elements for embedding Angular components in server-side rendered pages
- PowerShell automation for development workflow

## Architecture
- **Backend**: ASP.NET Core 10 with MVC and API controllers
- **Frontend**: Angular 19 with Angular Elements support
- **Hosting**: Single port (Kestrel web server)
- **Build System**: MSBuild with npm integration

## Features
- ✅ ASP.NET Core 10 backend
- ✅ Angular 19 frontend
- ✅ Single port hosting
- ✅ API controllers for REST services
- ✅ Razor views with Angular component integration
- ✅ Automated npm watch for local development (Debug builds only)

## Prerequisites
- .NET 10 SDK
- Node.js 20+ and npm
- PowerShell (for Windows development)
- SQL Server (LocalDB, Express, or full SQL Server instance)

## Getting Started

### First Time Setup
1. Clone the repository
2. Navigate to the solution directory
3. Install EF Core tools (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```
4. Configure database connection string in `RushtonRoots.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=.;Initial Catalog=RushtonRoots;persist security info=True;Integrated Security=SSPI;MultipleActiveResultSets=True;Encrypt=False"
   }
   ```
   - For LocalDB, use `Data Source=(localdb)\\mssqllocaldb;...`
   - For SQL Server Express, use `Data Source=.\\SQLEXPRESS;...`
   - For full SQL Server, use `Data Source=.;...` or `Data Source=localhost;...`
5. Build the solution:
   ```bash
   dotnet build
   ```
   This will automatically:
   - Restore NuGet packages
   - Install npm dependencies (if not already installed)
   - Start npm watch in a separate window (Debug builds only, Windows only)
6. Database migrations will run automatically on first application start (see Program.cs)
   - Alternatively, run migrations manually:
     ```bash
     dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
     ```

### Running the Application
```bash
cd RushtonRoots.Web
dotnet run
```

The application will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

### Development Workflow

#### Local Debug Builds (Windows)
When building in Debug mode on Windows, the PowerShell script `ClientApp/build-scripts/start-watch.ps1` will:
1. Check if npm dependencies are installed (runs `npm install` if needed)
2. Start `npm run watch` in a separate terminal window (Windows Terminal if available, otherwise PowerShell or cmd.exe)
3. Track the terminal process ID in `.watch-pid`
4. Prevent duplicate watch processes

The watch process will automatically rebuild Angular assets when you make changes to TypeScript/HTML/CSS files.

#### Managing the Watch Process
To manually start the watch process:
```powershell
cd RushtonRoots.Web\ClientApp\build-scripts
.\start-watch.ps1
```

To stop the npm watch process:
```powershell
cd RushtonRoots.Web\ClientApp\build-scripts
.\start-watch.ps1 -Stop
```

#### CI/QA/Production Builds
The npm watch script will NOT run if:
- Configuration is not Debug
- Environment variable `IsCI` is set to `true`
- Any CI environment is detected (CI, TF_BUILD, GITHUB_ACTIONS, etc.)

### Project Structure
```
RushtonRoots/
├── RushtonRoots.sln
├── PATTERNS.md                     # Detailed architecture and patterns documentation
├── RushtonRoots.Domain/           # Domain layer (no dependencies)
│   ├── UI/
│   │   ├── Models/                # View models and DTOs
│   │   └── Requests/              # Request models (SearchRequest, etc.)
│   └── Database/                  # Domain entities
├── RushtonRoots.Infrastructure/   # Infrastructure layer (depends on Domain)
│   ├── Database/
│   │   ├── RushtonRootsDbContext.cs
│   │   └── EntityConfigs/         # EF Core entity configurations
│   ├── Migrations/                # EF Core migrations
│   └── Repositories/              # Repository implementations
├── RushtonRoots.Application/      # Application layer (depends on Domain & Infrastructure)
│   ├── Services/                  # Business logic services
│   ├── Mappers/                   # Entity to ViewModel mappers
│   └── Validators/                # Input validation logic
├── RushtonRoots.Web/             # Presentation layer (depends on Application)
│   ├── Controllers/              # MVC and API controllers
│   │   ├── HomeController.cs
│   │   └── SampleApiController.cs
│   ├── Views/                    # Razor views
│   │   ├── Home/
│   │   │   └── Index.cshtml
│   │   └── Shared/
│   │       └── _Layout.cshtml
│   ├── ClientApp/                # Angular application
│   │   ├── src/
│   │   │   ├── app/
│   │   │   │   ├── app.module.ts
│   │   │   │   ├── app.component.*
│   │   │   │   └── welcome/     # Example Angular Element component
│   │   │   ├── main.ts
│   │   │   └── index.html
│   │   ├── build-scripts/        # Build automation scripts
│   │   │   └── start-watch.ps1   # Unified watch management script (start/stop)
│   │   ├── angular.json
│   │   └── package.json
│   ├── Scripts/                  # Legacy scripts directory (deprecated)
│   ├── AutofacModule.cs          # Dependency injection configuration
│   ├── Program.cs                # Application entry point
│   ├── appsettings.json          # Configuration (ConnectionStrings, Azure Blob Storage)
│   └── wwwroot/                  # Static files and Angular build output
└── RushtonRoots.UnitTests/       # Unit tests (XUnit + FakeItEasy)

```

## Documentation

- **[PATTERNS.md](PATTERNS.md)** - Detailed architecture, patterns, and development guidelines
- **[ROADMAP.md](ROADMAP.md)** - Comprehensive development roadmap with phased feature plans
- **[IMPLEMENTATION.md](IMPLEMENTATION.md)** - Solution implementation summary
- **[PROJECT_STRUCTURE_IMPLEMENTATION.md](PROJECT_STRUCTURE_IMPLEMENTATION.md)** - Project structure details


## Angular Integration

### Using Angular Components in Razor Views
Angular components can be embedded in Razor views using Angular Elements:

1. Create an Angular component (e.g., `WelcomeComponent`)
2. Register it as a custom element in `app.module.ts`:
   ```typescript
   const welcomeElement = createCustomElement(WelcomeComponent, { injector: this.injector });
   customElements.define('app-welcome', welcomeElement);
   ```
3. Use it in a Razor view:
   ```html
   <app-welcome name="User Name" message="Welcome!"></app-welcome>
   ```

### Build Process
- **Development**: Angular builds to `wwwroot/` with `outputHashing: none` for consistent filenames
- **Production**: Angular builds with optimization and hashing enabled

## API Endpoints
- `GET /api/sampleapi` - Returns a sample JSON response
- `GET /api/sampleapi/{id}` - Returns a sample JSON response with the specified ID

## Configuration

### Database Configuration

The application uses Entity Framework Core with SQL Server. The database is created and updated automatically on application startup.

#### Connection String
Edit `appsettings.json` to configure your database connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=RushtonRoots;persist security info=True;Integrated Security=SSPI;MultipleActiveResultSets=True;Encrypt=False"
  }
}
```

**Connection String Options:**
- **LocalDB**: `Data Source=(localdb)\\mssqllocaldb;Initial Catalog=RushtonRoots;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False`
- **SQL Server Express**: `Data Source=.\\SQLEXPRESS;Initial Catalog=RushtonRoots;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False`
- **Full SQL Server**: `Data Source=.;Initial Catalog=RushtonRoots;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False`
- **Remote Server**: `Data Source=server_name;Initial Catalog=RushtonRoots;User Id=username;Password=password;MultipleActiveResultSets=True;Encrypt=True`

#### Migration Commands

**Automatic Migrations** (Recommended):  
Migrations run automatically when the application starts (configured in `Program.cs`).

**Manual Migration Commands**:
```bash
# Apply all pending migrations to the database
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Create a new migration
dotnet ef migrations add MigrationName --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Remove the last migration (only if not applied to database)
dotnet ef migrations remove --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Drop the database (WARNING: deletes all data)
dotnet ef database drop --force --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Create database from scratch (applies all migrations)
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

#### Entity Configurations
All entity configurations are located in `RushtonRoots.Infrastructure/Database/EntityConfigs/` and are automatically applied using:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(RushtonRootsDbContext).Assembly);
```

The database includes 50+ entities with proper relationships, indexes, and constraints:
- **Core**: Person, Household, Partnership, ParentChild
- **Media**: Photo, PhotoAlbum, Document, Media
- **Collaboration**: Message, ChatRoom, Comment, Notification
- **Genealogy**: LifeEvent, Location, Source, Citation
- **Content**: Story, Recipe, Tradition, WikiPage
- And many more...

#### Automatic Timestamp Management
All entities inherit from `BaseEntity` which provides:
- `CreatedDateTime`: Set automatically on entity creation
- `UpdatedDateTime`: Updated automatically on entity modification

Timestamps are managed in `DbContext.SaveChanges()` and `DbContext.SaveChangesAsync()`.

### MSBuild Targets
The project includes custom MSBuild targets:
- **NpmInstall**: Runs `npm install` if `node_modules` doesn't exist
- **StartNpmWatch**: Starts the npm watch process (Debug builds only)
- **PublishRunWebpack**: Builds Angular for production during publish

### Launch Settings
Edit `Properties/launchSettings.json` to customize:
- Port numbers
- HTTPS settings
- Environment variables

## Troubleshooting

### Database Issues

#### Cannot connect to SQL Server
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- For LocalDB: Ensure it's installed with Visual Studio or SQL Server Express
- For remote connections: Verify SQL Server is configured to allow remote connections
- Check firewall settings

#### Migration fails
- Ensure you have proper permissions on the database
- Drop and recreate the database: `dotnet ef database drop --force` then run the application
- Check for syntax errors in migration files
- Verify entity configurations are correct

#### Database already exists error
- If migrations fail, you may need to drop the database and recreate:
  ```bash
  dotnet ef database drop --force --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
  dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
  ```

### npm watch not starting
- Ensure you're on Windows (PowerShell scripts don't run on Linux/Mac)
- Check that PowerShell execution policy allows script execution
- Verify you're building in Debug configuration

### Angular components not rendering
- Ensure npm dependencies are installed: `cd ClientApp && npm install`
- Build Angular: `cd ClientApp && npm run build -- --configuration development`
- Check browser console for errors
- Verify script references in Razor views match built file names

### Port conflicts
- Change the port in `Properties/launchSettings.json`
- Or use: `dotnet run --urls "http://localhost:XXXX"`

## License
This is a family project.
