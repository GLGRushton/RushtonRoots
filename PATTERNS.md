# RushtonRoots - Project Patterns and Architecture

## Overview
This document describes the architectural patterns, project structure, and development guidelines for the RushtonRoots application.

## Project Structure

### Solution Organization
The solution follows Clean Architecture principles with clear separation of concerns:

```
RushtonRoots/
├── RushtonRoots.Domain/          # Domain layer - no dependencies
├── RushtonRoots.Infrastructure/   # Infrastructure layer - depends on Domain
├── RushtonRoots.Application/      # Application layer - depends on Domain and Infrastructure
├── RushtonRoots.Web/             # Presentation layer - depends on Application
└── RushtonRoots.UnitTests/       # Tests - depends on all projects
```

### RushtonRoots.Domain
**Purpose**: Contains all domain models and entities. This is the core of the application with no external dependencies.

**Directory Structure**:
- `/UI/Models` - View models and DTOs for UI presentation
- `/UI/Requests` - Request models (e.g., SearchRequest, CreatePersonRequest)
- `/Database` - Database entities representing domain objects

**Key Principles**:
- No project references to other projects
- Pure domain logic only
- No infrastructure concerns (no database, no external services)

**Example Entity**:
```csharp
namespace RushtonRoots.Domain.Database;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    // Navigation properties
}
```

**Example UI Model**:
```csharp
namespace RushtonRoots.Domain.UI.Models;

public class PersonViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string BirthDate { get; set; }
}
```

**Example Request Model**:
```csharp
namespace RushtonRoots.Domain.UI.Requests;

public class SearchRequest
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
```

### RushtonRoots.Infrastructure
**Purpose**: Handles data persistence, external services, and infrastructure concerns.

**Dependencies**: References Domain project only

**Directory Structure**:
- `/Database` - DbContext and database configuration
- `/Database/EntityConfigs` - Entity Framework configurations
- `/Migrations` - EF Core migrations
- `/Repositories` - Repository implementations

**Key Responsibilities**:
- Database access via Entity Framework Core
- Azure Blob Storage integration
- Repository pattern implementation

**Example DbContext**:
```csharp
namespace RushtonRoots.Infrastructure.Database;

public class RushtonRootsDbContext : DbContext
{
    public DbSet<Person> People { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RushtonRootsDbContext).Assembly);
    }
}
```

**Example Entity Configuration**:
```csharp
namespace RushtonRoots.Infrastructure.Database.EntityConfigs;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
    }
}
```

**Example Repository Interface**:
```csharp
namespace RushtonRoots.Infrastructure.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(int id);
    Task<IEnumerable<Person>> GetAllAsync();
    Task<Person> AddAsync(Person person);
    Task UpdateAsync(Person person);
    Task DeleteAsync(int id);
}
```

**Example Repository Implementation**:
```csharp
namespace RushtonRoots.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly RushtonRootsDbContext _context;
    
    public PersonRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }
    
    public async Task<Person?> GetByIdAsync(int id)
    {
        return await _context.People.FindAsync(id);
    }
    
    // Other methods...
}
```

### RushtonRoots.Application
**Purpose**: Contains business logic, validation, and mapping between domain and UI models.

**Dependencies**: References Domain and Infrastructure projects

**Directory Structure**:
- `/Services` - Business logic and application services
- `/Mappers` - Mapping between entities and view models
- `/Validators` - Validation logic for create/update operations

**Key Responsibilities**:
- Orchestrate business operations
- Map between entities and DTOs
- Validate input data
- Coordinate repository calls

**Example Service Interface**:
```csharp
namespace RushtonRoots.Application.Services;

public interface IPersonService
{
    Task<PersonViewModel?> GetPersonByIdAsync(int id);
    Task<IEnumerable<PersonViewModel>> SearchPeopleAsync(SearchRequest request);
    Task<PersonViewModel> CreatePersonAsync(CreatePersonRequest request);
    Task UpdatePersonAsync(int id, UpdatePersonRequest request);
    Task DeletePersonAsync(int id);
}
```

**Example Service Implementation**:
```csharp
namespace RushtonRoots.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    private readonly IPersonMapper _mapper;
    private readonly IPersonValidator _validator;
    
    public PersonService(
        IPersonRepository repository,
        IPersonMapper mapper,
        IPersonValidator validator)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
    }
    
    public async Task<PersonViewModel> CreatePersonAsync(CreatePersonRequest request)
    {
        // Validate
        var validationResult = await _validator.ValidateCreateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Map to entity
        var person = _mapper.MapToEntity(request);
        
        // Save via repository
        var savedPerson = await _repository.AddAsync(person);
        
        // Map to view model
        return _mapper.MapToViewModel(savedPerson);
    }
    
    // Other methods...
}
```

**Example Mapper**:
```csharp
namespace RushtonRoots.Application.Mappers;

public interface IPersonMapper
{
    PersonViewModel MapToViewModel(Person person);
    Person MapToEntity(CreatePersonRequest request);
}

public class PersonMapper : IPersonMapper
{
    public PersonViewModel MapToViewModel(Person person)
    {
        return new PersonViewModel
        {
            Id = person.Id,
            FullName = $"{person.FirstName} {person.LastName}",
            BirthDate = person.DateOfBirth?.ToString("yyyy-MM-dd") ?? "Unknown"
        };
    }
    
    public Person MapToEntity(CreatePersonRequest request)
    {
        return new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth
        };
    }
}
```

**Example Validator**:
```csharp
namespace RushtonRoots.Application.Validators;

public interface IPersonValidator
{
    Task<ValidationResult> ValidateCreateAsync(CreatePersonRequest request);
    Task<ValidationResult> ValidateUpdateAsync(UpdatePersonRequest request);
}

public class PersonValidator : IPersonValidator
{
    public async Task<ValidationResult> ValidateCreateAsync(CreatePersonRequest request)
    {
        var result = new ValidationResult();
        
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            result.AddError("FirstName", "First name is required");
        }
        
        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            result.AddError("LastName", "Last name is required");
        }
        
        return result;
    }
}
```

### RushtonRoots.Web
**Purpose**: Presentation layer - MVC controllers, API endpoints, and views.

**Dependencies**: References Application project (which transitively includes Domain and Infrastructure)

**Key Responsibilities**:
- Handle HTTP requests
- Return views or JSON responses
- Minimal logic - delegate to services

**Example Controller**:
```csharp
namespace RushtonRoots.Web.Controllers;

public class PeopleController : Controller
{
    private readonly IPersonService _personService;
    
    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] SearchRequest request)
    {
        var people = await _personService.SearchPeopleAsync(request);
        return View(people);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var person = await _personService.GetPersonByIdAsync(id);
        if (person == null)
        {
            return NotFound();
        }
        return View(person);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {
        var person = await _personService.CreatePersonAsync(request);
        return CreatedAtAction(nameof(Details), new { id = person.Id }, person);
    }
}
```

### RushtonRoots.UnitTests
**Purpose**: Comprehensive unit testing with XUnit and FakeItEasy.

**Dependencies**: References all projects

**Testing Strategy**:
- Test services with mocked repositories
- Test repositories with in-memory database
- Test validators independently
- Test mappers for correct transformations

**Example Test**:
```csharp
namespace RushtonRoots.UnitTests.Services;

public class PersonServiceTests
{
    [Fact]
    public async Task CreatePersonAsync_ValidRequest_ReturnsViewModel()
    {
        // Arrange
        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();
        
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe"
        };
        
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var viewModel = new PersonViewModel { Id = 1, FullName = "John Doe" };
        
        A.CallTo(() => mockValidator.ValidateCreateAsync(request))
            .Returns(ValidationResult.Success());
        A.CallTo(() => mockMapper.MapToEntity(request)).Returns(person);
        A.CallTo(() => mockRepository.AddAsync(person)).Returns(person);
        A.CallTo(() => mockMapper.MapToViewModel(person)).Returns(viewModel);
        
        var service = new PersonService(mockRepository, mockMapper, mockValidator);
        
        // Act
        var result = await service.CreatePersonAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.FullName);
    }
}
```

## Architectural Patterns

### 1. SOLID Principles

#### Single Responsibility Principle (SRP)
Each class has one reason to change:
- Controllers handle HTTP concerns only
- Services handle business logic only
- Repositories handle data access only
- Validators handle validation only
- Mappers handle mapping only

#### Open/Closed Principle (OCP)
Classes are open for extension but closed for modification:
- Use interfaces for dependencies
- Extend functionality through new implementations
- Use strategy pattern when appropriate

#### Liskov Substitution Principle (LSP)
Derived classes must be substitutable for their base classes:
- Implement interfaces consistently
- Maintain expected behavior contracts

#### Interface Segregation Principle (ISP)
Clients shouldn't depend on interfaces they don't use:
- Keep interfaces focused and small
- Split large interfaces into specific ones

#### Dependency Inversion Principle (DIP)
Depend on abstractions, not concretions:
- All dependencies are injected via constructor
- Use interfaces for all service dependencies
- Configured via Autofac in `AutofacModule`

### 2. Repository Pattern
All data access goes through repositories:
- Abstracts database implementation details
- Provides a collection-like interface
- Located in Infrastructure layer
- Interfaces define contracts

Benefits:
- Easy to test with mocks
- Database agnostic
- Centralized data access logic

### 3. Service Layer Pattern
Business logic is encapsulated in services:
- Controllers are thin and delegate to services
- Services orchestrate operations
- Services use validators before modifying data
- Services use mappers for transformations

### 4. Request/Response Flow

```
┌──────────────┐
│   Browser    │
└──────┬───────┘
       │ HTTP Request
       ▼
┌──────────────┐
│  Controller  │ (Minimal logic, delegates to service)
└──────┬───────┘
       │ Call service method
       ▼
┌──────────────┐
│   Service    │ (Business logic orchestration)
└──────┬───────┘
       │
       ├─► Validator (Validate input)
       │
       ├─► Mapper (Map request to entity)
       │
       ├─► Repository (Data access)
       │
       └─► Mapper (Map entity to view model)
       │
       ▼
┌──────────────┐
│  Controller  │ (Return view or JSON)
└──────┬───────┘
       │ HTTP Response
       ▼
┌──────────────┐
│   Browser    │
└──────────────┘
```

## Dependency Injection with Autofac

All dependency injection is configured in `AutofacModule` to keep `Program.cs` clean.

### Registration Patterns

**Convention-based registration**:
```csharp
// Registers all classes ending with "Repository" in Infrastructure assembly
builder.RegisterAssemblyTypes(typeof(RushtonRootsDbContext).Assembly)
    .Where(t => t.Name.EndsWith("Repository"))
    .AsImplementedInterfaces()
    .InstancePerLifetimeScope();
```

**Explicit registration**:
```csharp
builder.RegisterType<PersonService>()
    .As<IPersonService>()
    .InstancePerLifetimeScope();
```

### Lifetime Scopes
- `InstancePerLifetimeScope` - New instance per HTTP request (recommended for most services)
- `SingleInstance` - Singleton (use sparingly, for truly stateless services)
- `InstancePerDependency` - New instance every time (rarely needed)

## Database Configuration

### Entity Framework Core Setup
Connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RushtonRootsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Registered in `AutofacModule`:
```csharp
builder.Register(c =>
{
    var optionsBuilder = new DbContextOptionsBuilder<RushtonRootsDbContext>();
    optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    return new RushtonRootsDbContext(optionsBuilder.Options);
})
.AsSelf()
.InstancePerLifetimeScope();
```

### Migrations
Create migrations from the Infrastructure project:
```bash
# From repository root
dotnet ef migrations add InitialCreate --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web

# Apply migrations
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

## Azure Blob Storage Configuration

Configuration in `appsettings.json`:
```json
{
  "AzureBlobStorage": {
    "ConnectionString": "",
    "ContainerName": "rushtonroots-files"
  }
}
```

Development configuration uses local emulator:
```json
{
  "AzureBlobStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "rushtonroots-files-dev"
  }
}
```

## Development Workflow

### Adding a New Feature
1. Define entities in `Domain/Database`
2. Define view models in `Domain/UI/Models`
3. Define request models in `Domain/UI/Requests`
4. Create entity configuration in `Infrastructure/Database/EntityConfigs`
5. Create repository interface and implementation in `Infrastructure/Repositories`
6. Create mapper in `Application/Mappers`
7. Create validator in `Application/Validators`
8. Create service in `Application/Services`
9. Create controller in `Web/Controllers`
10. Write unit tests in `UnitTests`

### Testing Strategy
- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test database operations with real DbContext
- **Mocking**: Use FakeItEasy to mock dependencies
- **Coverage**: Aim for high coverage of business logic

## Best Practices

### Controllers
- Keep thin - delegate to services
- Use attribute routing for APIs
- Return appropriate HTTP status codes
- Handle exceptions with filters

### Services
- One service per domain entity
- Use dependency injection
- Always validate input
- Use mappers for transformations
- Make async for I/O operations

### Repositories
- Keep simple - basic CRUD operations
- Return domain entities
- Use async/await
- Don't expose IQueryable outside repository

### Validation
- Validate all input at service layer
- Return detailed validation errors
- Don't trust client-side validation

### Error Handling
- Use exception filters for global error handling
- Log errors appropriately
- Return user-friendly error messages
- Never expose sensitive information

## Security Considerations

1. **No Secrets in Code**: Store connection strings in configuration
2. **Input Validation**: Always validate and sanitize input
3. **SQL Injection**: Use parameterized queries (EF Core does this)
4. **Authentication/Authorization**: Add when needed with ASP.NET Core Identity
5. **HTTPS**: Always use HTTPS in production

## Future Enhancements

- Add FluentValidation for more sophisticated validation
- Add AutoMapper for more complex mapping scenarios
- Add MediatR for CQRS pattern
- Add Serilog for structured logging
- Add Health Checks for monitoring
- Add API versioning
- Add Swagger/OpenAPI documentation

## Summary

This architecture provides:
- ✅ Clear separation of concerns
- ✅ Testable components
- ✅ SOLID principles
- ✅ Repository pattern for data access
- ✅ Service layer for business logic
- ✅ Clean dependency injection
- ✅ Scalable and maintainable codebase
