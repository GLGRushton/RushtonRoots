# Copilot Instructions for RushtonRoots

## Project Overview

RushtonRoots is a family tree/genealogy web application built with:
- **Backend**: .NET 10 ASP.NET Core with MVC and API controllers
- **Frontend**: Angular 19 with Angular Elements for component embedding
- **Architecture**: Clean Architecture with 5 projects following SOLID principles
- **Database**: SQL Server with Entity Framework Core 10
- **Authentication**: ASP.NET Core Identity
- **DI Container**: Autofac with convention-based registration
- **Testing**: XUnit + FakeItEasy for mocking
- **Hosting**: Single Kestrel web server on one port

## Architecture & Project Structure

The solution follows Clean Architecture with unidirectional dependencies:

```
Domain (no dependencies)
   ↑
Infrastructure (depends on Domain)
   ↑
Application (depends on Domain & Infrastructure)
   ↑
Web (depends on Application)
   ↑
UnitTests (depends on all)
```

### 1. RushtonRoots.Domain
**Location**: `/RushtonRoots.Domain/`  
**Purpose**: Core domain layer with NO external dependencies

**Directory Structure**:
- `/Database/` - Domain entities (Person, Household, Partnership, ParentChild, etc.)
  - All entities inherit from `BaseEntity` (provides Id, CreatedDateTime, UpdatedDateTime)
  - Entities use navigation properties for relationships
- `/UI/Models/` - View models and DTOs for UI presentation
- `/UI/Requests/` - Request models (SearchRequest, CreatePersonRequest, etc.)

**Key Principles**:
- Pure domain logic only
- No infrastructure concerns (no EF, no external services)
- No project references

### 2. RushtonRoots.Infrastructure  
**Location**: `/RushtonRoots.Infrastructure/`  
**Purpose**: Data persistence and external service integration

**Directory Structure**:
- `/Database/RushtonRootsDbContext.cs` - Main DbContext (inherits from IdentityDbContext)
  - Automatically updates CreatedDateTime/UpdatedDateTime on SaveChanges
  - Uses `ApplyConfigurationsFromAssembly` for entity configurations
- `/Database/EntityConfigs/` - EF Core entity configurations (IEntityTypeConfiguration<T>)
  - One configuration per entity (PersonConfiguration, HouseholdConfiguration, etc.)
- `/Migrations/` - EF Core migrations
- `/Repositories/` - Repository implementations (currently empty, uses placeholder .gitkeep)

**NuGet Packages**:
- Microsoft.EntityFrameworkCore.SqlServer 10.0.1
- Microsoft.EntityFrameworkCore.Design 10.0.1
- Azure.Storage.Blobs 12.26.0

### 3. RushtonRoots.Application
**Location**: `/RushtonRoots.Application/`  
**Purpose**: Business logic orchestration, validation, and mapping

**Directory Structure**:
- `/Services/` - Business logic services (currently empty, uses placeholder .gitkeep)
- `/Mappers/` - Entity to ViewModel mapping (currently empty, uses placeholder .gitkeep)
- `/Validators/` - Input validation (currently empty, uses placeholder .gitkeep)
- `AssemblyMarker.cs` - Empty class used for assembly scanning in Autofac

**Pattern**: Services orchestrate operations by:
1. Validating input via validators
2. Mapping request to entity via mappers
3. Calling repository for data access
4. Mapping entity to view model via mappers

### 4. RushtonRoots.Web
**Location**: `/RushtonRoots.Web/`  
**Purpose**: Presentation layer - controllers, views, Angular frontend

**Directory Structure**:
- `Program.cs` - Application entry point with Autofac integration
  - Runs database migrations on startup
  - Configures Identity with password requirements
- `AutofacModule.cs` - Centralized DI configuration using convention-based registration
- `/Controllers/` - MVC and API controllers
  - `HomeController.cs` - UI controller
  - `SampleApiController.cs` - REST API controller
- `/Views/` - Razor views with Angular component embedding
- `/ClientApp/` - Angular 19 application
  - `angular.json` - Angular CLI configuration
  - `package.json` - npm dependencies (Angular 19, Angular Elements)
  - `/src/app/` - Angular components and modules
  - `/build-scripts/` - Build automation scripts
    - `start-watch.ps1` - Unified script to start/stop npm watch for development (Windows only)
- `/Scripts/` - Legacy scripts directory (deprecated)
- `/wwwroot/` - Static files and Angular build output

**NuGet Packages**:
- Autofac 9.0.0
- Autofac.Extensions.DependencyInjection 10.0.0
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.1
- Microsoft.EntityFrameworkCore.Design 10.0.1

### 5. RushtonRoots.UnitTests
**Location**: `/RushtonRoots.UnitTests/`  
**Purpose**: Unit and integration testing

**Testing Stack**:
- XUnit for test framework
- FakeItEasy for mocking
- coverlet.collector for code coverage

**NuGet Packages**:
- xunit 2.9.3
- xunit.runner.visualstudio 3.1.4
- FakeItEasy 8.3.0
- Microsoft.NET.Test.Sdk 17.14.1

## Dependency Injection with Autofac

All DI registration is in `RushtonRoots.Web/AutofacModule.cs` using **convention-based registration**:

```csharp
// Repositories: Classes ending with "Repository" in Infrastructure assembly
builder.RegisterAssemblyTypes(typeof(RushtonRootsDbContext).Assembly)
    .Where(t => t.Name.EndsWith("Repository"))
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();

// Services: Classes ending with "Service" in Application assembly  
builder.RegisterAssemblyTypes(typeof(AssemblyMarker).Assembly)
    .Where(t => t.Name.EndsWith("Service"))
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();

// Validators: Classes ending with "Validator" in Application assembly
builder.RegisterAssemblyTypes(typeof(AssemblyMarker).Assembly)
    .Where(t => t.Name.EndsWith("Validator"))
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();

// Mappers: Classes ending with "Mapper" in Application assembly
builder.RegisterAssemblyTypes(typeof(AssemblyMarker).Assembly)
    .Where(t => t.Name.EndsWith("Mapper"))
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();
```

**Important**: When creating new services, repositories, validators, or mappers:
- Name them with the appropriate suffix (e.g., `PersonService`, `PersonRepository`)
- Create an interface (e.g., `IPersonService`, `IPersonRepository`)
- They will be **automatically registered** by convention

## Database Configuration

**Connection String**: Configured in `appsettings.json` / `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RushtonRootsDb;..."
  }
}
```

**Entity Framework Commands** (run from repository root):

```bash
# Create migration
dotnet ef migrations add MigrationName --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Apply migrations
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Remove last migration
dotnet ef migrations remove --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

**Important**: 
- Migrations run automatically on application startup (see `Program.cs`)
- All entities inherit from `BaseEntity` which provides CreatedDateTime/UpdatedDateTime
- DbContext automatically updates timestamps in SaveChanges/SaveChangesAsync

## Build System

**Build Configuration**:
- The project uses MSBuild with custom targets in `RushtonRoots.Web.csproj`
- **NpmInstall** target: Runs `npm install` if `node_modules` doesn't exist
- **StartNpmWatch** target: Starts npm watch in Debug mode (Windows only, not in CI)
- **PublishRunWebpack** target: Builds Angular for production during publish

**Build Commands**:

```bash
# Build solution
dotnet build

# Build for Release
dotnet build -c Release

# Run the application
cd RushtonRoots.Web
dotnet run

# Run tests
dotnet test

# Publish
dotnet publish -c Release
```

**Environment Detection**:
- npm watch script only runs in Debug configuration on Windows
- Automatically skips in CI environments (CI, IsCI, TF_BUILD, GITHUB_ACTIONS)

## Angular Integration

**Build Process**:
- Development: `npm run watch` auto-rebuilds on file changes (outputHashing: none)
- Production: `npm run build -- --configuration production` (with optimization and hashing)
- Output directory: `wwwroot/` (main.js, polyfills.js, runtime.js, styles.css)

**Angular Elements**:
- Angular components are registered as custom elements in `app.module.ts`
- Use in Razor views: `<app-welcome name="User" message="Welcome!"></app-welcome>`

**Important Files**:
- `ClientApp/angular.json` - Angular CLI configuration
- `ClientApp/package.json` - npm dependencies (Angular 19)
- `ClientApp/src/main.ts` - Bootstrap entry point
- `ClientApp/src/app/app.module.ts` - Module with Angular Elements registration

## Development Workflow

### Adding a New Feature

Follow this step-by-step process:

1. **Define Entity** in `Domain/Database/`
   ```csharp
   public class MyEntity : BaseEntity
   {
       public string Name { get; set; } = string.Empty;
       // Navigation properties
   }
   ```

2. **Create Entity Configuration** in `Infrastructure/Database/EntityConfigs/`
   ```csharp
   public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
   {
       public void Configure(EntityTypeBuilder<MyEntity> builder)
       {
           builder.HasKey(e => e.Id);
           builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
       }
   }
   ```

3. **Add DbSet** in `RushtonRootsDbContext.cs`
   ```csharp
   public DbSet<MyEntity> MyEntities { get; set; }
   ```

4. **Create Migration**
   ```bash
   dotnet ef migrations add AddMyEntity --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
   ```

5. **Define UI Models** in `Domain/UI/Models/` and `Domain/UI/Requests/`

6. **Create Repository** in `Infrastructure/Repositories/`
   - Name: `MyEntityRepository.cs` and `IMyEntityRepository.cs`
   - Will be auto-registered by Autofac

7. **Create Mapper** in `Application/Mappers/`
   - Name: `MyEntityMapper.cs` and `IMyEntityMapper.cs`
   - Will be auto-registered by Autofac

8. **Create Validator** in `Application/Validators/`
   - Name: `MyEntityValidator.cs` and `IMyEntityValidator.cs`
   - Will be auto-registered by Autofac

9. **Create Service** in `Application/Services/`
   - Name: `MyEntityService.cs` and `IMyEntityService.cs`
   - Will be auto-registered by Autofac

10. **Create Controller** in `Web/Controllers/`
    - Inject service via constructor
    - Keep thin - delegate to service

11. **Write Unit Tests** in `UnitTests/`
    - Use FakeItEasy for mocking: `var mock = A.Fake<IMyService>();`
    - Setup calls: `A.CallTo(() => mock.Method()).Returns(result);`

### Testing Strategy

**Unit Tests**:
- Test services with mocked repositories
- Test validators independently
- Test mappers for correct transformations
- Use `A.Fake<T>()` for mocking with FakeItEasy
- Use `A.CallTo(() => mock.Method()).Returns(value)` for setup

**Example Test**:
```csharp
[Fact]
public async Task GetById_ReturnsViewModel()
{
    // Arrange
    var mockRepo = A.Fake<IPersonRepository>();
    var mockMapper = A.Fake<IPersonMapper>();
    var person = new Person { Id = 1, FirstName = "John" };
    
    A.CallTo(() => mockRepo.GetByIdAsync(1)).Returns(person);
    A.CallTo(() => mockMapper.MapToViewModel(person)).Returns(new PersonViewModel { Id = 1 });
    
    var service = new PersonService(mockRepo, mockMapper);
    
    // Act
    var result = await service.GetByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
}
```

## Common Patterns

### Controller Pattern (Thin Controllers)
```csharp
[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    private readonly IMyService _service;
    
    public MyController(IMyService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
```

### Service Pattern
```csharp
public class MyService : IMyService
{
    private readonly IMyRepository _repository;
    private readonly IMyMapper _mapper;
    private readonly IMyValidator _validator;
    
    public async Task<MyViewModel> CreateAsync(CreateMyRequest request)
    {
        // 1. Validate
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);
        
        // 2. Map to entity
        var entity = _mapper.MapToEntity(request);
        
        // 3. Save via repository
        var saved = await _repository.AddAsync(entity);
        
        // 4. Map to view model
        return _mapper.MapToViewModel(saved);
    }
}
```

### Repository Pattern
```csharp
public class MyRepository : IMyRepository
{
    private readonly RushtonRootsDbContext _context;
    
    public async Task<MyEntity?> GetByIdAsync(int id)
    {
        return await _context.MyEntities.FindAsync(id);
    }
    
    public async Task<MyEntity> AddAsync(MyEntity entity)
    {
        _context.MyEntities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
```

## Configuration Files

**appsettings.json** - Base configuration:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RushtonRootsDb;..."
  },
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files"
  }
}
```

**appsettings.Development.json** - Development overrides:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RushtonRootsDb_Dev;..."
  },
  "AzureBlobStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "rushtonroots-files-dev"
  }
}
```

## Key Files to Reference

**Architecture Documentation**:
- `/README.md` - Project overview, setup, and usage
- `/PATTERNS.md` - Detailed architecture patterns and code examples
- `/IMPLEMENTATION.md` - Solution implementation summary
- `/PROJECT_STRUCTURE_IMPLEMENTATION.md` - Project structure details

**Core Configuration**:
- `RushtonRoots.Web/Program.cs` - App entry point, Autofac setup, Identity config
- `RushtonRoots.Web/AutofacModule.cs` - DI registration (ALWAYS update for manual registration)
- `RushtonRoots.Infrastructure/Database/RushtonRootsDbContext.cs` - DbContext and timestamp logic

**Build**:
- `RushtonRoots.Web/RushtonRoots.Web.csproj` - MSBuild targets for npm integration
- `RushtonRoots.Web/ClientApp/package.json` - npm scripts and Angular dependencies

## Important Guidelines

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Extend via new implementations, not modifications
- **Liskov Substitution**: Implementations must be substitutable for interfaces
- **Interface Segregation**: Keep interfaces focused and small
- **Dependency Inversion**: Depend on abstractions (interfaces), inject via constructor

### Best Practices
1. **Controllers**: Keep thin, delegate to services, return appropriate HTTP status codes
2. **Services**: One service per domain entity, always validate input, use async/await
3. **Repositories**: Keep simple (basic CRUD), return domain entities, don't expose IQueryable
4. **Validation**: Validate all input at service layer, return detailed errors
5. **Error Handling**: Use exception filters for global handling, never expose sensitive info
6. **Security**: No secrets in code, validate/sanitize all input, use parameterized queries (EF Core does this)

### Naming Conventions
- Entities: PascalCase, singular (Person, Household)
- DbSets: PascalCase, plural (People, Households)
- Interfaces: IPascalCase (IPersonService, IPersonRepository)
- Services: PascalCase + "Service" suffix (PersonService) - auto-registered
- Repositories: PascalCase + "Repository" suffix (PersonRepository) - auto-registered
- Validators: PascalCase + "Validator" suffix (PersonValidator) - auto-registered
- Mappers: PascalCase + "Mapper" suffix (PersonMapper) - auto-registered

## Troubleshooting

### Build Issues
- **npm watch not starting**: Windows only, check PowerShell execution policy, verify Debug config
- **Angular not rendering**: Run `cd ClientApp && npm install`, check browser console
- **EF migrations fail**: Ensure --startup-project is RushtonRoots.Web, check connection string

### Common Errors
- **Missing DbSet**: Add to RushtonRootsDbContext.cs
- **DI resolution fails**: Check class name suffix (Service, Repository, Validator, Mapper)
- **Migration not applied**: Runs on startup, but can manually run `dotnet ef database update`
- **Timestamps not updating**: Ensure entity inherits from BaseEntity

## CI/CD Considerations

- npm watch is automatically skipped in CI environments
- Set `IsCI=true` environment variable to disable watch scripts
- Build in Release configuration: `dotnet build -c Release`
- Tests run via: `dotnet test`
- Publish via: `dotnet publish -c Release`

## Quick Reference Commands

```bash
# Build
dotnet build

# Test  
dotnet test

# Run application
cd RushtonRoots.Web && dotnet run

# Create migration
dotnet ef migrations add <Name> --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Update database
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Install npm packages (if needed)
cd RushtonRoots.Web/ClientApp && npm install

# Build Angular
cd RushtonRoots.Web/ClientApp && npm run build

# Watch Angular (auto-rebuild)
cd RushtonRoots.Web/ClientApp && npm run watch
```

## Summary

This is a well-architected Clean Architecture application with:
- ✅ Clear separation of concerns (5 projects)
- ✅ SOLID principles throughout
- ✅ Convention-based DI with Autofac
- ✅ Entity Framework Core with automatic migrations
- ✅ ASP.NET Core Identity for authentication
- ✅ Angular 19 with Angular Elements
- ✅ Single port hosting (API + UI + static files)
- ✅ Comprehensive unit testing with XUnit + FakeItEasy
- ✅ PowerShell automation for development workflow

When working on this project, always follow the established patterns, leverage the convention-based DI registration, and reference the detailed documentation in README.md and PATTERNS.md for comprehensive examples.
