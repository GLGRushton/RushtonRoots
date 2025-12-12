# RushtonRoots
A family creation

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
- ✅ Clean separation: Angular builds to `ClientApp/dist/` during development
- ✅ Production builds copy to `wwwroot/` only during publish

## Prerequisites
- .NET 10 SDK
- Node.js 20+ and npm

## Getting Started

### First Time Setup
1. Clone the repository
2. Navigate to the solution directory
3. Install npm dependencies:
   ```bash
   cd RushtonRoots.Web/ClientApp
   npm install
   cd ../..
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```

### Running the Application

#### Development Mode
For local development, you need to run both the .NET backend and Angular build separately:

1. **Terminal 1 - Start the .NET backend:**
   ```bash
   cd RushtonRoots.Web
   dotnet run
   ```

2. **Terminal 2 - Build Angular (one-time or watch mode):**
   ```bash
   cd RushtonRoots.Web/ClientApp
   # One-time build
   npm run build -- --configuration development
   # OR watch mode (rebuilds on file changes)
   npm run watch
   ```

3. **Manually copy Angular files to wwwroot for testing:**
   ```bash
   # From RushtonRoots.Web directory
   # Windows (PowerShell):
   xcopy ClientApp\dist\* wwwroot\ /E /Y
   # Linux/Mac:
   cp -r ClientApp/dist/* wwwroot/
   ```

The application will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

**Note:** During development, `wwwroot/` remains empty until you manually copy files or run a publish. The Angular build outputs to `ClientApp/dist/` to maintain clean separation.

### Development Workflow

#### Local Development
The new workflow maintains clean separation between development and production:

- **Angular files** are built to `ClientApp/dist/` directory
- **wwwroot/** remains empty during development (only populated during publish)
- **Development builds** use non-hashed filenames (main.js, polyfills.js, runtime.js)
- **Production builds** use hashed filenames for cache-busting

To work with Angular components:
1. Make changes in `ClientApp/src/`
2. Run `npm run watch` in the ClientApp directory to auto-rebuild on changes
3. Manually copy files from `ClientApp/dist/` to `wwwroot/` when you want to test in the browser
4. Refresh your browser to see changes

#### Production/Publish Builds
For production deployment:
```bash
dotnet publish -c Release
```

This will:
1. Run `npm install` in the ClientApp directory
2. Build Angular with production configuration (optimization, minification, hashing)
3. Copy the built files from `ClientApp/dist/` to `wwwroot/`
4. Include `wwwroot/` files in the publish output

**Important:** No Angular development files, configs, or node_modules are ever copied to `wwwroot/`. Only the production build output from `dist/` is deployed.

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
│   │   ├── angular.json
│   │   ├── package.json
│   │   └── dist/                # Angular build output (development)
│   ├── AutofacModule.cs          # Dependency injection configuration
│   ├── Program.cs                # Application entry point
│   ├── appsettings.json          # Configuration (ConnectionStrings, Azure Blob Storage)
│   └── wwwroot/                  # Static files (Angular files copied here only on publish)
└── RushtonRoots.UnitTests/       # Unit tests (XUnit + FakeItEasy)

```

For detailed information about the architecture, patterns, and development guidelines, see [PATTERNS.md](PATTERNS.md).


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
- **Development**: Angular builds to `ClientApp/dist/` with `outputHashing: none` for consistent filenames (main.js, polyfills.js, runtime.js)
- **Production**: Angular builds to `ClientApp/dist/` with optimization and hashing enabled, then copied to `wwwroot/` during publish
- **Clean Separation**: No development files or node_modules are ever in `wwwroot/`

### Directory Structure
```
ClientApp/
  src/              # Angular source code
  dist/             # Development build output (gitignored)
  node_modules/     # npm dependencies (gitignored)
  angular.json      # Angular CLI configuration
  package.json      # npm package definition

wwwroot/
  # Empty during development
  # Populated with production build files only during 'dotnet publish'
  # Files: main.[hash].js, polyfills.[hash].js, runtime.[hash].js, styles.[hash].css
```

## API Endpoints
- `GET /api/sampleapi` - Returns a sample JSON response
- `GET /api/sampleapi/{id}` - Returns a sample JSON response with the specified ID

## Configuration

### MSBuild Targets
The project includes custom MSBuild targets:
- **NpmInstall**: Runs `npm install` if `node_modules` doesn't exist (before build)
- **PublishRunWebpack**: Builds Angular for production and copies output to `wwwroot/` during publish only

### Launch Settings
Edit `Properties/launchSettings.json` to customize:
- Port numbers
- HTTPS settings
- Environment variables

## Troubleshooting

### Angular components not rendering
1. **Ensure npm dependencies are installed:**
   ```bash
   cd RushtonRoots.Web/ClientApp
   npm install
   ```

2. **Build Angular to dist/:**
   ```bash
   cd RushtonRoots.Web/ClientApp
   npm run build -- --configuration development
   ```

3. **Copy build output to wwwroot for local testing:**
   ```bash
   # From RushtonRoots.Web directory
   # Windows (PowerShell):
   xcopy ClientApp\dist\* wwwroot\ /E /Y
   # Linux/Mac:
   cp -r ClientApp/dist/* wwwroot/
   ```

4. **Check browser console for errors**
5. **Verify script references** in Razor views use non-hashed names (main.js, polyfills.js, runtime.js)

### wwwroot is empty
This is normal during development! The `wwwroot/` directory only gets populated:
- When you manually copy files from `ClientApp/dist/` for local testing
- Automatically during `dotnet publish` for production deployment

### Production deployment fails
Ensure the publish process completes successfully:
```bash
dotnet publish -c Release -o ./publish
```
This will build Angular and copy files to `wwwroot/` automatically.

### Port conflicts
- Change the port in `Properties/launchSettings.json`
- Or use: `dotnet run --urls "http://localhost:XXXX"`

## License
This is a family project.
