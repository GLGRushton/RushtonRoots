# Developer Onboarding Guide - RushtonRoots

**Last Updated:** December 2025  
**Version:** 1.0

Welcome to the RushtonRoots development team! This guide will help you get up and running with the codebase.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [First Day Setup](#first-day-setup)
3. [Development Workflow](#development-workflow)
4. [Architecture Overview](#architecture-overview)
5. [Code Standards](#code-standards)
6. [Testing Guidelines](#testing-guidelines)
7. [Common Tasks](#common-tasks)
8. [Troubleshooting](#troubleshooting)
9. [Resources](#resources)

---

## Prerequisites

### Required Software

1. **Development Environment**
   - Visual Studio 2022 (17.8+) or Visual Studio Code
   - .NET 10 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
   - Node.js 20+ ([Download](https://nodejs.org/))
   - Git ([Download](https://git-scm.com/))

2. **Database**
   - SQL Server 2019+ OR
   - SQL Server Express ([Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)) OR
   - SQL Server LocalDB (included with Visual Studio)

3. **Storage Emulator (Development)**
   - Docker Desktop ([Download](https://www.docker.com/products/docker-desktop/)) for Azurite OR
   - Azurite via npm: `npm install -g azurite`

4. **Optional Tools**
   - SQL Server Management Studio (SSMS) for database management
   - Azure Storage Explorer for blob storage management
   - Postman for API testing

### Recommended Extensions

**Visual Studio:**
- ReSharper (optional, for enhanced code analysis)
- Web Essentials

**VS Code:**
- C# Dev Kit
- Angular Language Service
- ESLint
- REST Client

---

## First Day Setup

### 1. Clone the Repository

```bash
git clone https://github.com/GLGRushton/RushtonRoots.git
cd RushtonRoots
```

### 2. Install Global Tools

```bash
# Entity Framework Core CLI
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

### 3. Configure Database

**Option A: SQL Server LocalDB (Recommended for Windows)**

No additional setup needed. Connection string is pre-configured in `appsettings.Development.json`.

**Option B: SQL Server Express**

Edit `RushtonRoots.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=RushtonRoots_Dev;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False"
  }
}
```

**Option C: Full SQL Server**

Edit `RushtonRoots.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=RushtonRoots_Dev;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=False"
  }
}
```

### 4. Start Azure Storage Emulator

**Using Docker (Recommended):**
```bash
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
  --name azurite \
  mcr.microsoft.com/azure-storage/azurite
```

**Using npm:**
```bash
azurite --silent --location ~/azurite
```

### 5. Build the Solution

```bash
# Restore packages and build
dotnet build

# This will automatically:
# - Restore NuGet packages
# - Run npm install in ClientApp
# - Start npm watch (Windows only, Debug mode)
```

### 6. Apply Database Migrations

Migrations run automatically on first application start. Alternatively:

```bash
cd RushtonRoots.Web
dotnet ef database update --project ../RushtonRoots.Infrastructure
```

### 7. Run the Application

```bash
cd RushtonRoots.Web
dotnet run
```

Navigate to:
- **Application**: https://localhost:5001
- **API Documentation**: https://localhost:5001/api-docs (Development only)

### 8. Default Login Credentials

After database seeding, you can log in with:

- **Admin User**: Check `DatabaseSeeder.cs` for seeded credentials (not committed to source control for security)

---

## Development Workflow

### Daily Workflow

1. **Start Development Services**
   ```bash
   # Start Azurite (if not running)
   docker start azurite
   
   # Start application
   cd RushtonRoots.Web
   dotnet run
   ```

2. **Make Code Changes**
   - Angular changes: Auto-rebuild with npm watch (Windows Debug builds)
   - C# changes: Stop and restart `dotnet run`

3. **Run Tests**
   ```bash
   # Run all tests
   dotnet test
   
   # Run specific test project
   dotnet test RushtonRoots.UnitTests
   
   # Run with coverage
   dotnet test /p:CollectCoverage=true
   ```

4. **Create Pull Request**
   ```bash
   git checkout -b feature/your-feature-name
   git add .
   git commit -m "Description of changes"
   git push origin feature/your-feature-name
   ```

### Branch Strategy

- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Urgent production fixes

### Commit Message Format

```
<type>: <description>

[optional body]

[optional footer]
```

**Types:**
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code formatting (no logic changes)
- `refactor:` Code refactoring
- `test:` Adding or updating tests
- `chore:` Maintenance tasks

**Example:**
```
feat: Add thumbnail generation for uploaded photos

- Implemented GenerateThumbnailsAsync using ImageSharp
- Added support for multiple thumbnail sizes
- Updated PhotoService to auto-generate thumbnails on upload

Closes #123
```

---

## Architecture Overview

### Clean Architecture Layers

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│      (RushtonRoots.Web)             │
│   - Controllers (MVC + API)         │
│   - Views (Razor + Angular)         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Application Layer               │
│   (RushtonRoots.Application)        │
│   - Services (Business Logic)       │
│   - Mappers                         │
│   - Validators                      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    Infrastructure Layer             │
│  (RushtonRoots.Infrastructure)      │
│   - Repositories                    │
│   - DbContext                       │
│   - External Services               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│         Domain Layer                │
│     (RushtonRoots.Domain)           │
│   - Entities                        │
│   - ViewModels                      │
│   - Request Models                  │
└─────────────────────────────────────┘
```

### Dependency Injection

RushtonRoots uses **Autofac** with **convention-based registration**:

- Classes ending in `Service` → Auto-registered as `IService`
- Classes ending in `Repository` → Auto-registered as `IRepository`
- Classes ending in `Mapper` → Auto-registered as `IMapper`
- Classes ending in `Validator` → Auto-registered as `IValidator`

**Configuration:** `RushtonRoots.Web/AutofacModule.cs`

### Database

- **ORM:** Entity Framework Core 10
- **Database:** SQL Server
- **Migrations:** Automatic on startup (configured in `Program.cs`)
- **Entities:** 50+ domain entities with relationships
- **Timestamp Management:** Automatic via `BaseEntity` class

---

## Code Standards

### Naming Conventions

**C# Code:**
- **Classes/Interfaces:** PascalCase (`PersonService`, `IPersonRepository`)
- **Methods:** PascalCase (`GetByIdAsync`, `CreateAsync`)
- **Properties:** PascalCase (`FirstName`, `BirthDate`)
- **Private fields:** _camelCase (`_personRepository`, `_mapper`)
- **Parameters:** camelCase (`personId`, `createRequest`)

**TypeScript/Angular:**
- **Classes/Interfaces:** PascalCase (`PersonFormComponent`, `IPersonViewModel`)
- **Methods/Functions:** camelCase (`getPersonById`, `validateForm`)
- **Properties:** camelCase (`firstName`, `birthDate`)
- **Constants:** UPPER_SNAKE_CASE (`API_BASE_URL`)

### SOLID Principles

1. **Single Responsibility:** Each class has one reason to change
2. **Open/Closed:** Extend via new implementations, not modifications
3. **Liskov Substitution:** Implementations must be substitutable for interfaces
4. **Interface Segregation:** Keep interfaces focused and small
5. **Dependency Inversion:** Depend on abstractions, inject via constructor

### Code Style

**Controllers:**
- Keep thin - delegate to services
- Return appropriate HTTP status codes
- Use DTOs/ViewModels, not domain entities
- Add `[Authorize]` attributes where needed

**Services:**
- One service per domain entity
- Always validate input
- Use async/await
- Throw appropriate exceptions (`ValidationException`, `NotFoundException`)

**Repositories:**
- Keep simple (basic CRUD)
- Return domain entities
- Don't expose `IQueryable`
- Use Include() for eager loading

**Tests:**
- Use AAA pattern (Arrange, Act, Assert)
- One assertion per test (when practical)
- Use FakeItEasy for mocking: `A.Fake<IService>()`
- Descriptive test names: `GetById_WithValidId_ReturnsViewModel`

---

## Testing Guidelines

### Test Structure

```
RushtonRoots.UnitTests/
├── Controllers/
│   ├── Api/
│   │   └── PersonControllerTests.cs
│   └── PersonControllerTests.cs
├── Services/
│   └── PersonServiceTests.cs
├── Repositories/
│   └── PersonRepositoryTests.cs
└── Mappers/
    └── PersonMapperTests.cs
```

### Writing Tests

**Unit Test Example:**
```csharp
[Fact]
public async Task GetByIdAsync_WithValidId_ReturnsViewModel()
{
    // Arrange
    var mockRepo = A.Fake<IPersonRepository>();
    var mockMapper = A.Fake<IPersonMapper>();
    var person = new Person { Id = 1, FirstName = "John" };
    var viewModel = new PersonViewModel { Id = 1, FirstName = "John" };
    
    A.CallTo(() => mockRepo.GetByIdAsync(1)).Returns(person);
    A.CallTo(() => mockMapper.MapToViewModel(person)).Returns(viewModel);
    
    var service = new PersonService(mockRepo, mockMapper);
    
    // Act
    var result = await service.GetByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
    Assert.Equal("John", result.FirstName);
}
```

**Repository Test with In-Memory Database:**
```csharp
[Fact]
public async Task GetByIdAsync_WithValidId_ReturnsPerson()
{
    // Arrange
    var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    
    using var context = new RushtonRootsDbContext(options);
    var person = new Person { FirstName = "John", LastName = "Doe" };
    context.People.Add(person);
    await context.SaveChangesAsync();
    
    var repository = new PersonRepository(context);
    
    // Act
    var result = await repository.GetByIdAsync(person.Id);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("John", result.FirstName);
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test -v n

# Run specific test
dotnet test --filter "FullyQualifiedName~PersonServiceTests"

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Coverage Goals

- **Minimum:** 80% overall coverage
- **Services:** 90%+ coverage
- **Controllers:** 80%+ coverage
- **Repositories:** 85%+ coverage

---

## Common Tasks

### Adding a New Entity

1. **Create Entity** (`Domain/Database/MyEntity.cs`)
   ```csharp
   public class MyEntity : BaseEntity
   {
       public string Name { get; set; } = string.Empty;
       public int HouseholdId { get; set; }
       public Household? Household { get; set; }
   }
   ```

2. **Create Entity Configuration** (`Infrastructure/Database/EntityConfigs/MyEntityConfiguration.cs`)
   ```csharp
   public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
   {
       public void Configure(EntityTypeBuilder<MyEntity> builder)
       {
           builder.HasKey(e => e.Id);
           builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
           builder.HasOne(e => e.Household)
                  .WithMany()
                  .HasForeignKey(e => e.HouseholdId)
                  .OnDelete(DeleteBehavior.Restrict);
       }
   }
   ```

3. **Add DbSet** to `RushtonRootsDbContext.cs`
   ```csharp
   public DbSet<MyEntity> MyEntities { get; set; }
   ```

4. **Create Migration**
   ```bash
   cd RushtonRoots.Web
   dotnet ef migrations add AddMyEntity --project ../RushtonRoots.Infrastructure
   ```

5. **Create ViewModel** (`Domain/UI/Models/MyEntityViewModel.cs`)

6. **Create Repository** (`Infrastructure/Repositories/MyEntityRepository.cs`)
   - Naming: `MyEntityRepository` → Auto-registered as `IMyEntityRepository`

7. **Create Mapper** (`Application/Mappers/MyEntityMapper.cs`)
   - Naming: `MyEntityMapper` → Auto-registered as `IMyEntityMapper`

8. **Create Service** (`Application/Services/MyEntityService.cs`)
   - Naming: `MyEntityService` → Auto-registered as `IMyEntityService`

9. **Create Controller** (`Web/Controllers/Api/MyEntityController.cs`)

10. **Write Tests** (`UnitTests/`)

### Adding a New API Endpoint

1. **Create in API Controller:**
   ```csharp
   [HttpGet("{id}")]
   [ProducesResponseType(typeof(MyEntityViewModel), StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> GetById(int id)
   {
       try
       {
           var result = await _myEntityService.GetByIdAsync(id);
           return Ok(result);
       }
       catch (NotFoundException)
       {
           return NotFound();
       }
   }
   ```

2. **Add XML Documentation:**
   ```csharp
   /// <summary>
   /// Gets a MyEntity by its ID
   /// </summary>
   /// <param name="id">The ID of the entity to retrieve</param>
   /// <returns>The MyEntity with the specified ID</returns>
   /// <response code="200">Returns the entity</response>
   /// <response code="404">If the entity is not found</response>
   [HttpGet("{id}")]
   ```

3. **Write Controller Tests**

4. **Test in Swagger:** Navigate to `/api-docs`

### Creating a Database Migration

```bash
# Navigate to Web project
cd RushtonRoots.Web

# Create migration
dotnet ef migrations add MigrationName --project ../RushtonRoots.Infrastructure

# Review the migration file
# Located in: Infrastructure/Migrations/

# Apply migration (or let auto-migration do it)
dotnet ef database update --project ../RushtonRoots.Infrastructure
```

### Running the Application

**Development:**
```bash
cd RushtonRoots.Web
dotnet run
```

**Production Build:**
```bash
dotnet build -c Release
dotnet run -c Release
```

**With Specific Port:**
```bash
dotnet run --urls "https://localhost:5001"
```

---

## Troubleshooting

### Build Issues

**npm watch not starting:**
- Ensure you're on Windows (PowerShell scripts)
- Check PowerShell execution policy: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Verify Debug configuration

**Angular not rendering:**
```bash
cd RushtonRoots.Web/ClientApp
npm install
npm run build
```

**Migration fails:**
```bash
# Drop and recreate database
dotnet ef database drop --force --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
dotnet run  # Auto-migration will recreate
```

### Database Issues

**Cannot connect to SQL Server:**
- Verify SQL Server is running: `services.msc` → SQL Server
- Check connection string in `appsettings.Development.json`
- For LocalDB: Install with Visual Studio or SQL Server Express

**Database already exists error:**
```bash
dotnet ef database drop --force --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

### Azure Storage Issues

**Azurite not running:**
```bash
# Start Azurite with Docker
docker start azurite

# Or create new container
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
  --name azurite \
  mcr.microsoft.com/azure-storage/azurite
```

**Blob upload fails:**
- Verify Azurite is running: `docker ps`
- Check connection string in `appsettings.Development.json`
- Ensure container is created (auto-created on first upload)

### Test Failures

**In-memory database issues:**
- Each test should use a unique database name: `Guid.NewGuid().ToString()`
- Dispose context properly: `using var context = ...`

**Mocking issues with FakeItEasy:**
```csharp
// Correct syntax
A.CallTo(() => mock.MethodAsync()).Returns(result);

// For void methods
A.CallTo(() => mock.VoidMethod()).DoesNothing();

// For exceptions
A.CallTo(() => mock.Method()).Throws<Exception>();
```

---

## Resources

### Documentation

- **[README.md](../README.md)** - Project overview and quick start
- **[ROADMAP.md](../ROADMAP.md)** - Feature roadmap (770+ lines)
- **[CodebaseReviewAndPhasedPlan.md](./CodebaseReviewAndPhasedPlan.md)** - Comprehensive review and implementation plan
- **[AzureStorageSetup.md](./AzureStorageSetup.md)** - Azure Blob Storage setup guide
- **[Phase6.1-TestCoverageReport.md](./Phase6.1-TestCoverageReport.md)** - Test coverage analysis

### API Documentation

- **Swagger UI:** https://localhost:5001/api-docs (Development only)
- **OpenAPI Spec:** https://localhost:5001/swagger/v1/swagger.json

### External Resources

- **ASP.NET Core:** https://docs.microsoft.com/en-us/aspnet/core/
- **Entity Framework Core:** https://docs.microsoft.com/en-us/ef/core/
- **Angular:** https://angular.io/docs
- **Autofac:** https://autofac.readthedocs.io/
- **XUnit:** https://xunit.net/
- **FakeItEasy:** https://fakeiteasy.github.io/

### Community

- **GitHub Issues:** [Report bugs or request features](https://github.com/GLGRushton/RushtonRoots/issues)
- **Pull Requests:** [Contribute code](https://github.com/GLGRushton/RushtonRoots/pulls)

---

## Next Steps

After completing this onboarding:

1. **Explore the Codebase**
   - Review existing controllers, services, repositories
   - Understand the domain model (50+ entities)
   - Study test examples

2. **Pick a Starter Task**
   - Check GitHub Issues for "good first issue" labels
   - Fix a small bug or add a test
   - Review existing PRs to understand code review standards

3. **Schedule Pairing Sessions**
   - Pair with senior developers
   - Ask questions about architecture decisions
   - Learn about deployment process

4. **Read the Test Coverage Report**
   - See [Phase6.1-TestCoverageReport.md](./Phase6.1-TestCoverageReport.md)
   - Understand testing patterns used in the codebase

---

**Welcome to the team!** 🎉

If you have questions or need help, don't hesitate to ask in team channels or create a GitHub issue.

---

**Document Version:** 1.0  
**Last Updated:** December 2025  
**Maintained By:** Development Team
