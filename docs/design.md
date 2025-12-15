# RushtonRoots - System Design & Implementation Plan

## Document Overview

**Purpose**: This document provides a comprehensive phased implementation plan for RushtonRoots, a family genealogy and collaboration platform. It outlines the architecture, design decisions, implementation phases, and technical patterns to guide development.

**Last Updated**: December 2025  
**Document Owner**: Development Team  
**Review Frequency**: Quarterly

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [System Architecture](#system-architecture)
3. [Design Principles](#design-principles)
4. [Technical Stack](#technical-stack)
5. [Data Model Design](#data-model-design)
6. [Implementation Phases](#implementation-phases)
7. [Security & Privacy Design](#security--privacy-design)
8. [Scalability & Performance](#scalability--performance)
9. [Integration Points](#integration-points)
10. [Testing Strategy](#testing-strategy)
11. [Deployment Architecture](#deployment-architecture)
12. [Risk Mitigation](#risk-mitigation)

---

## Executive Summary

RushtonRoots is a comprehensive family platform that combines genealogy, family history, media management, collaboration tools, and knowledge preservation into a unified system. The platform is designed to serve multiple generations, providing tools for family members to connect, collaborate, and preserve their heritage.

### Key Objectives

- **Preserve**: Family history, stories, photos, and documents for future generations
- **Connect**: Family members across time, distance, and generations
- **Collaborate**: On shared goals, events, and family projects
- **Celebrate**: Family heritage, achievements, and traditions
- **Scale**: Support hundreds of thousands of family members over decades

### Success Criteria

- 99.9% uptime availability
- Page load times under 2 seconds
- Support for 100,000+ family members
- Zero data breaches
- High user engagement (50%+ monthly active users)

---

## System Architecture

### Architectural Pattern: Clean Architecture

RushtonRoots follows Clean Architecture principles with clear separation of concerns and unidirectional dependencies:

```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                  │
│          (RushtonRoots.Web - ASP.NET Core)          │
│   - Controllers (MVC & API)                         │
│   - Razor Views                                      │
│   - Angular 19 Frontend                              │
│   - Angular Elements Integration                     │
└────────────────────┬────────────────────────────────┘
                     │ depends on
                     ↓
┌─────────────────────────────────────────────────────┐
│               Application Layer                      │
│         (RushtonRoots.Application)                  │
│   - Services (Business Logic)                       │
│   - Validators (Input Validation)                   │
│   - Mappers (Entity ↔ ViewModel)                    │
└────────────────────┬────────────────────────────────┘
                     │ depends on
                     ↓
┌─────────────────────────────────────────────────────┐
│              Infrastructure Layer                    │
│        (RushtonRoots.Infrastructure)                │
│   - DbContext (EF Core)                             │
│   - Repositories (Data Access)                      │
│   - External Services (Azure Blob, etc.)            │
│   - Entity Configurations                           │
└────────────────────┬────────────────────────────────┘
                     │ depends on
                     ↓
┌─────────────────────────────────────────────────────┐
│                  Domain Layer                        │
│            (RushtonRoots.Domain)                    │
│   - Entities (Person, Household, etc.)              │
│   - View Models (UI/Models)                         │
│   - Request Models (UI/Requests)                    │
│   - Business Rules                                  │
└─────────────────────────────────────────────────────┘
```

### Dependency Flow

- **Domain**: Zero dependencies - contains pure business logic and entities
- **Infrastructure**: Depends on Domain - implements data persistence and external services
- **Application**: Depends on Domain & Infrastructure - orchestrates business operations
- **Web**: Depends on Application - handles HTTP requests and UI rendering
- **UnitTests**: Depends on all - validates functionality at all layers

---

## Design Principles

### SOLID Principles

1. **Single Responsibility Principle (SRP)**
   - Each class has one reason to change
   - Services focus on a single domain entity
   - Controllers delegate to services, keeping thin

2. **Open/Closed Principle (OCP)**
   - Extend behavior through new implementations
   - Use interfaces for abstraction
   - Avoid modifying existing stable code

3. **Liskov Substitution Principle (LSP)**
   - Implementations must be substitutable for their interfaces
   - Derived classes must honor base class contracts

4. **Interface Segregation Principle (ISP)**
   - Keep interfaces focused and small
   - Clients shouldn't depend on methods they don't use

5. **Dependency Inversion Principle (DIP)**
   - Depend on abstractions (interfaces), not concretions
   - Use constructor injection for dependencies

### Additional Design Principles

- **Convention over Configuration**: Autofac auto-registers classes based on naming conventions
- **Separation of Concerns**: Clear boundaries between layers
- **Don't Repeat Yourself (DRY)**: Reuse code through services and utilities
- **Keep It Simple (KISS)**: Prefer simple, maintainable solutions
- **You Aren't Gonna Need It (YAGNI)**: Implement features only when needed

---

## Technical Stack

### Backend Technologies

- **.NET 10**: Latest ASP.NET Core framework
- **ASP.NET Core MVC**: Server-side rendering with Razor views
- **ASP.NET Core Web API**: RESTful API endpoints
- **Entity Framework Core 10**: ORM for data access
- **SQL Server**: Primary relational database
- **ASP.NET Core Identity**: Authentication and authorization
- **Autofac 9.0**: Dependency injection container

### Frontend Technologies

- **Angular 19**: Modern TypeScript-based framework
- **Angular Elements**: Web components for Razor integration
- **RxJS**: Reactive programming for async operations
- **TypeScript**: Type-safe JavaScript development

### Infrastructure & DevOps

- **Azure Blob Storage**: Media and document storage
- **Kestrel**: High-performance web server
- **MSBuild**: Build system with npm integration
- **PowerShell**: Build automation scripts

### Testing & Quality

- **XUnit**: Unit testing framework
- **FakeItEasy**: Mocking framework
- **Coverlet**: Code coverage analysis

---

## Data Model Design

### Core Entities

#### Person (Central Entity)

```
Person
├── Id (int, PK)
├── FirstName (string, required)
├── MiddleName (string, nullable)
├── LastName (string, required)
├── MaidenName (string, nullable)
├── Gender (string, nullable)
├── DateOfBirth (DateTime, nullable)
├── DateOfDeath (DateTime, nullable)
├── BirthLocationId (int, FK → Location)
├── DeathLocationId (int, FK → Location)
├── CreatedDateTime (DateTime)
├── UpdatedDateTime (DateTime)
└── Relationships:
    ├── Households (Many-to-Many via HouseholdPermission)
    ├── ParentRelationships (One-to-Many via ParentChild)
    ├── ChildRelationships (One-to-Many via ParentChild)
    ├── Partnerships (One-to-Many)
    ├── Photos (One-to-Many via PersonPhoto)
    ├── LifeEvents (One-to-Many)
    ├── Stories (Many-to-Many via StoryPerson)
    ├── Documents (Many-to-Many via DocumentPerson)
    └── Media (Many-to-Many via MediaPerson)
```

#### Household (Family Unit)

```
Household
├── Id (int, PK)
├── Name (string, required)
├── Description (string, nullable)
├── CreatedDateTime (DateTime)
├── UpdatedDateTime (DateTime)
└── Relationships:
    ├── Members (Many-to-Many via HouseholdPermission)
    └── Events (One-to-Many via FamilyEvent)
```

#### Partnership (Marriages/Relationships)

```
Partnership
├── Id (int, PK)
├── Person1Id (int, FK → Person)
├── Person2Id (int, FK → Person)
├── StartDate (DateTime, nullable)
├── EndDate (DateTime, nullable)
├── LocationId (int, FK → Location)
├── PartnershipType (string) // Marriage, Domestic Partnership, etc.
├── CreatedDateTime (DateTime)
└── UpdatedDateTime (DateTime)
```

#### ParentChild (Lineage)

```
ParentChild
├── Id (int, PK)
├── ParentId (int, FK → Person)
├── ChildId (int, FK → Person)
├── RelationshipType (string) // Biological, Adopted, Step, Foster
├── CreatedDateTime (DateTime)
└── UpdatedDateTime (DateTime)
```

### Media & Document Entities

#### Media (Photos/Videos/Audio)

```
Media
├── Id (int, PK)
├── Title (string, required)
├── Description (string, nullable)
├── MediaType (string) // Photo, Video, Audio
├── BlobUrl (string, required)
├── ThumbnailUrl (string, nullable)
├── UploadedBy (string, FK → ApplicationUser)
├── UploadedDate (DateTime)
├── LocationId (int, FK → Location)
├── TakenDate (DateTime, nullable)
└── Relationships:
    ├── TaggedPeople (Many-to-Many via MediaPerson)
    ├── Albums (Many-to-Many via PhotoAlbum)
    └── Permissions (One-to-Many via MediaPermission)
```

#### Document

```
Document
├── Id (int, PK)
├── Title (string, required)
├── Description (string, nullable)
├── DocumentType (string) // Birth Certificate, Will, Deed, etc.
├── BlobUrl (string, required)
├── UploadedBy (string, FK → ApplicationUser)
├── UploadedDate (DateTime)
└── Relationships:
    ├── RelatedPeople (Many-to-Many via DocumentPerson)
    ├── Versions (One-to-Many via DocumentVersion)
    └── Permissions (One-to-Many via DocumentPermission)
```

### Collaboration Entities

#### WikiPage (Family Wiki)

```
WikiPage
├── Id (int, PK)
├── Title (string, required)
├── Content (string, required, Markdown)
├── CategoryId (int, FK → WikiCategory)
├── CreatedBy (string, FK → ApplicationUser)
├── CreatedDate (DateTime)
├── LastModifiedBy (string, FK → ApplicationUser)
├── LastModifiedDate (DateTime)
└── Relationships:
    ├── Versions (One-to-Many via WikiPageVersion)
    ├── Tags (Many-to-Many via WikiTag)
    └── Template (Many-to-One via WikiTemplate)
```

#### FamilyEvent

```
FamilyEvent
├── Id (int, PK)
├── Title (string, required)
├── Description (string, nullable)
├── EventType (string) // Reunion, Birthday, Anniversary
├── StartDate (DateTime, required)
├── EndDate (DateTime, nullable)
├── LocationId (int, FK → Location)
├── HouseholdId (int, FK → Household)
└── Relationships:
    └── RSVPs (One-to-Many via EventRsvp)
```

#### Message (Direct & Group Messaging)

```
Message
├── Id (int, PK)
├── ChatRoomId (int, FK → ChatRoom)
├── SenderId (string, FK → ApplicationUser)
├── Content (string, required)
├── SentDateTime (DateTime)
├── IsRead (bool)
└── ReadDateTime (DateTime, nullable)
```

### Authentication & Authorization

#### ApplicationUser (ASP.NET Identity)

```
ApplicationUser : IdentityUser
├── Id (string, PK)
├── Email (string, required, unique)
├── UserName (string, required, unique)
├── FirstName (string)
├── LastName (string)
├── PersonId (int, FK → Person, nullable)
└── Relationships:
    ├── Households (Many-to-Many via HouseholdPermission)
    └── Roles (Many-to-Many via IdentityUserRole)
```

### Database Relationships Summary

- **One-to-Many**: Person → LifeEvent, Person → BiographicalNote
- **Many-to-Many**: Person ↔ Household (via HouseholdPermission), Person ↔ Media (via MediaPerson)
- **Self-Referencing**: Person → Person (via ParentChild)
- **Hierarchical**: WikiCategory (tree structure)

---

## Implementation Phases

### Phase 1: Foundation & Core Genealogy (COMPLETE ✅)

**Duration**: Months 1-3  
**Status**: Complete

#### Deliverables
- [x] User authentication and authorization (ASP.NET Identity)
- [x] Person CRUD operations
- [x] Household management
- [x] Basic relationship management (parent-child, partnerships)
- [x] Person search and filtering

#### Technical Implementation
- Database migrations for core entities
- Repository pattern for data access
- Service layer for business logic
- MVC controllers for UI
- Angular components for interactive elements

### Phase 2: Enhanced Genealogy & Visualization (COMPLETE ✅)

**Duration**: Months 4-6  
**Status**: Complete

#### Deliverables
- [x] Family tree visualization (pedigree, descendant, fan charts)
- [x] Life events management
- [x] Location/place management with geocoding
- [x] Timeline view for person's life
- [x] Advanced search and discovery features
- [x] Relationship calculator

#### Technical Implementation
- D3.js or similar for tree visualization
- Google Maps API for geocoding
- Timeline component in Angular
- Advanced search with filtering
- Graph algorithms for relationship calculation

### Phase 3: Media & Document Management (COMPLETE ✅)

**Duration**: Months 7-9  
**Status**: Complete

#### Deliverables
- [x] Photo gallery with Azure Blob Storage
- [x] Photo tagging and albums
- [x] Document management system
- [x] Video and audio storage
- [x] Media permissions and sharing
- [x] Transcription support

#### Technical Implementation
- Azure Blob Storage integration
- Image processing (thumbnails, optimization)
- Video streaming capability
- Document preview (PDF.js)
- Permission system for media access

### Phase 4: Collaboration & Communication (COMPLETE ✅)

**Duration**: Months 10-12  
**Status**: Complete

#### Deliverables
- [x] Direct messaging and group chat
- [x] Email and in-app notifications
- [x] Family calendar and event planning
- [x] Task management
- [x] Collaborative editing
- [x] Comment system
- [x] Contribution workflow with approval

#### Technical Implementation
- SignalR for real-time messaging
- Background jobs for email notifications
- Calendar component with RSVP
- Notification hub with preferences
- Version control for collaborative content

### Phase 5: Research & DNA Integration (PLANNED)

**Duration**: Months 13-15  
**Status**: Not Started

#### Deliverables
- [ ] Research log and task management
- [ ] Source repository with citations
- [ ] DNA test result upload and management
- [ ] DNA match management
- [ ] Chromosome browser
- [ ] GEDCOM import/export
- [ ] FamilySearch API integration

#### Technical Implementation
- File parsers for DNA data (AncestryDNA, 23andMe formats)
- Relationship prediction algorithms
- GEDCOM parser and generator
- External API integration layer
- Chromosome visualization component

#### Design Decisions
- **DNA Data Storage**: Store raw DNA data in separate secure blob storage with encryption
- **API Integration**: Use adapter pattern for external genealogy services
- **GEDCOM Format**: Support GEDCOM 7.0 standard
- **Privacy**: DNA data visible only to data owner unless explicitly shared

### Phase 6: Family Knowledge Base (COMPLETE ✅)

**Duration**: Months 16-18  
**Status**: Complete

#### Deliverables
- [x] Wiki-style pages for family topics
- [x] Markdown editor for articles
- [x] Wiki version history
- [x] Story submission and categorization
- [x] Recipe repository
- [x] Tradition documentation

#### Technical Implementation
- Markdown parser and renderer
- WYSIWYG editor (TinyMCE or similar)
- Version control for wiki pages
- Recipe card templates
- Story collections and books

### Phase 7: Financial & Legal Tools (PLANNED)

**Duration**: Months 19-21  
**Status**: Not Started

#### Deliverables
- [ ] Secure storage for wills and trusts
- [ ] Asset inventory system
- [ ] Beneficiary tracking
- [ ] Family budget tracking
- [ ] Gift and expense tracking
- [ ] Property ownership timeline
- [ ] Heirloom registry

#### Technical Implementation
- Enhanced encryption for sensitive documents
- Financial calculation modules
- Property timeline visualization
- Expense splitting algorithms
- Inheritance planning tools

#### Design Decisions
- **Security**: Multi-factor authentication required for financial document access
- **Encryption**: AES-256 encryption for all financial and legal documents
- **Audit Trail**: Complete audit log for all access to sensitive documents
- **Compliance**: Follow estate planning best practices, but not legal advice

### Phase 8: Health & Medical History (PLANNED)

**Duration**: Months 22-24  
**Status**: Not Started

#### Deliverables
- [ ] HIPAA-compliant medical information storage
- [ ] Condition and diagnosis tracking
- [ ] Hereditary condition tracking
- [ ] Health analytics and risk analysis
- [ ] Health report generation
- [ ] Wellness challenge system

#### Technical Implementation
- HIPAA-compliant data storage and transmission
- Medical terminology database
- Health analytics algorithms
- Risk calculation based on family history
- Integration with health record standards (FHIR)

#### Design Decisions
- **HIPAA Compliance**: Implement Business Associate Agreement (BAA) requirements
- **Data Encryption**: End-to-end encryption for all medical data
- **Access Control**: Strict role-based access with audit logging
- **Anonymization**: Option to anonymize data for research purposes

### Phase 9: Education & Skills (PLANNED)

**Duration**: Months 25-27  
**Status**: Not Started

#### Deliverables
- [ ] Family skills directory
- [ ] Mentorship matching system
- [ ] Educational content library
- [ ] Quiz system about family history
- [ ] Career history tracking
- [ ] Professional network directory

#### Technical Implementation
- Skills taxonomy and categorization
- Matching algorithm for mentorship
- Learning management system (LMS) features
- Quiz engine with progress tracking
- Career timeline visualization

### Phase 10: Advanced Features & AI (PLANNED)

**Duration**: Months 28-30  
**Status**: Not Started

#### Deliverables
- [ ] AI-powered photo recognition
- [ ] AI transcription for audio/video
- [ ] AI-generated family summaries
- [ ] Duplicate detection
- [ ] AI relationship suggestions
- [ ] Predictive search

#### Technical Implementation
- Azure Cognitive Services for face recognition
- Speech-to-text API integration
- Natural language processing for summaries
- Machine learning for duplicate detection
- Graph neural networks for relationship prediction

#### Design Decisions
- **AI Ethics**: Transparent AI usage with user consent
- **Data Privacy**: AI processing on encrypted data where possible
- **Model Training**: Use anonymized data for model improvement
- **Human Review**: AI suggestions require human confirmation

### Phase 11: Mobile & Offline (PLANNED)

**Duration**: Months 31-33  
**Status**: Not Started

#### Deliverables
- [ ] Progressive Web App (PWA)
- [ ] Offline mode with sync
- [ ] Mobile-optimized UI
- [ ] Camera integration
- [ ] GPS tagging
- [ ] QR code generation

#### Technical Implementation
- Service Workers for offline caching
- IndexedDB for local storage
- Sync API for background synchronization
- Conflict resolution for offline edits
- Responsive design for mobile

### Phase 12: Globalization & Accessibility (PLANNED)

**Duration**: Months 34-36  
**Status**: Not Started

#### Deliverables
- [ ] Multi-language support (i18n)
- [ ] WCAG 2.1 AA compliance
- [ ] Screen reader optimization
- [ ] Keyboard navigation
- [ ] High contrast themes
- [ ] Cultural customization

#### Technical Implementation
- Angular i18n framework
- Accessibility testing automation
- ARIA labels and semantic HTML
- Cultural calendar systems
- Region-specific formats

### Phases 13-18: Advanced Platform Features (PLANNED)

**Remaining Phases** (Months 37-54):
- Phase 13: Security & Privacy enhancements
- Phase 14: Platform & API development
- Phase 15: Analytics & Reporting
- Phase 16: Gamification & Engagement
- Phase 17: Business & Monetization
- Phase 18: Performance & Scalability

---

## Security & Privacy Design

### Authentication & Authorization

#### Multi-Layered Security

1. **Authentication Layer**
   - ASP.NET Core Identity for user management
   - Password requirements: minimum 8 characters, complexity rules
   - Email verification for new accounts
   - Password reset with secure tokens
   - Future: Two-factor authentication (2FA)

2. **Authorization Layer**
   - Role-based access control (RBAC)
     - Roles: Admin, Family Member, Guest
   - Household-based permissions
   - Resource-level permissions (media, documents)
   - Claims-based authorization for fine-grained control

3. **Session Management**
   - Secure session cookies (HttpOnly, Secure, SameSite)
   - Session timeout configuration
   - Automatic session refresh
   - Single sign-out across devices

### Data Protection

#### Encryption Strategy

1. **Data at Rest**
   - SQL Server Transparent Data Encryption (TDE)
   - Azure Blob Storage encryption
   - Encrypted backups

2. **Data in Transit**
   - HTTPS/TLS 1.3 for all communications
   - Certificate pinning for mobile apps
   - Secure WebSocket connections (WSS) for real-time features

3. **Sensitive Data**
   - AES-256 encryption for financial/medical documents
   - Encryption keys stored in Azure Key Vault
   - Separate encryption for different sensitivity levels

#### Privacy Controls

1. **Living Person Privacy**
   - Automatic privacy mode for living individuals
   - Configurable privacy settings per person
   - Family member approval for profile updates

2. **Data Minimization**
   - Collect only necessary information
   - Optional fields for sensitive data
   - Data retention policies

3. **User Control**
   - Privacy dashboard for each user
   - Data export capability (GDPR compliance)
   - Right to be forgotten workflow
   - Granular sharing permissions

### Security Best Practices

1. **Input Validation**
   - Server-side validation for all inputs
   - Parameterized queries (EF Core prevents SQL injection)
   - XSS protection via encoding
   - CSRF tokens for state-changing operations

2. **API Security**
   - Rate limiting to prevent abuse
   - API key/token authentication
   - CORS configuration
   - Request signing for sensitive operations

3. **Audit Logging**
   - Log all authentication attempts
   - Log access to sensitive data
   - Log administrative actions
   - Secure log storage with retention policy

---

## Scalability & Performance

### Performance Optimization

#### Database Optimization

1. **Indexing Strategy**
   - Primary key indexes (automatic)
   - Foreign key indexes for joins
   - Composite indexes for common queries
   - Full-text search indexes for name/content search

2. **Query Optimization**
   - Use of Include() for eager loading
   - Pagination for large result sets
   - Asynchronous database operations
   - Database query performance monitoring

3. **Caching Strategy**
   - In-memory caching for frequently accessed data
   - Distributed cache (Redis) for multi-server deployments
   - Output caching for static content
   - Cache invalidation on data updates

#### Frontend Performance

1. **Angular Optimization**
   - Lazy loading of modules
   - OnPush change detection strategy
   - Virtual scrolling for large lists
   - Tree shaking and code splitting

2. **Asset Optimization**
   - Image compression and resizing
   - Thumbnail generation for photos
   - CDN for static assets
   - Brotli/Gzip compression

3. **Loading Strategies**
   - Progressive image loading
   - Skeleton screens for perceived performance
   - Service Worker caching (PWA)
   - Prefetching for predicted navigation

### Scalability Architecture

#### Horizontal Scaling

1. **Stateless Application Design**
   - No session state in application servers
   - Session stored in distributed cache
   - Enables load balancing across multiple servers

2. **Database Scaling**
   - Read replicas for query scaling
   - Database sharding for write scaling
   - Connection pooling
   - Query optimization

3. **Blob Storage Scaling**
   - Azure Blob Storage auto-scales
   - CDN for global distribution
   - Multiple storage accounts for isolation

#### Monitoring & Observability

1. **Application Monitoring**
   - Application Insights for telemetry
   - Performance metrics (response time, throughput)
   - Error tracking and alerting
   - Custom metrics for business logic

2. **Database Monitoring**
   - Query performance tracking
   - Index usage analysis
   - Deadlock detection
   - Connection pool monitoring

3. **Infrastructure Monitoring**
   - Server health checks
   - Resource utilization (CPU, memory, disk)
   - Network latency
   - Availability monitoring

---

## Integration Points

### External Service Integrations

#### Azure Blob Storage

**Purpose**: Store photos, videos, audio, and documents

**Implementation**:
- Azure.Storage.Blobs SDK
- Blob naming convention: `{year}/{month}/{guid}.{extension}`
- Public access level: Blob (for shared media)
- SAS tokens for time-limited access

**Configuration**:
```json
{
  "AzureBlobStorage": {
    "ConnectionString": "...",
    "ContainerName": "rushtonroots-files"
  }
}
```

#### Email Service

**Purpose**: Send notifications, password resets, and announcements

**Options**:
- Azure Communication Services
- SendGrid
- Mailgun

**Email Types**:
- Welcome emails
- Password reset
- Event invitations
- Notification digests
- System announcements

#### Geocoding Service

**Purpose**: Convert addresses to coordinates for location features

**Options**:
- Google Maps Geocoding API
- Azure Maps
- OpenStreetMap Nominatim

**Use Cases**:
- Birth/death location mapping
- Event location mapping
- Migration pattern visualization

#### Future Integrations

1. **FamilySearch API**
   - Search historical records
   - Import family tree data
   - Access record hints

2. **Ancestry.com** (if available)
   - Import hints and suggestions
   - Sync family tree

3. **DNA Testing Services**
   - AncestryDNA data import
   - 23andMe data import
   - MyHeritage DNA import

---

## Testing Strategy

### Unit Testing

**Framework**: XUnit  
**Mocking**: FakeItEasy

#### Test Coverage Goals
- Service layer: 90%+
- Validator layer: 100%
- Mapper layer: 100%
- Repository layer: 80%+

#### Testing Patterns

1. **Service Tests**
   ```csharp
   [Fact]
   public async Task CreatePerson_ValidRequest_ReturnsViewModel()
   {
       // Arrange
       var mockRepo = A.Fake<IPersonRepository>();
       var mockMapper = A.Fake<IPersonMapper>();
       var mockValidator = A.Fake<IPersonValidator>();
       
       var service = new PersonService(mockRepo, mockMapper, mockValidator);
       
       // Act
       var result = await service.CreateAsync(request);
       
       // Assert
       Assert.NotNull(result);
       A.CallTo(() => mockRepo.AddAsync(A<Person>._)).MustHaveHappened();
   }
   ```

2. **Validator Tests**
   ```csharp
   [Theory]
   [InlineData("", "Doe", false)] // Empty first name
   [InlineData("John", "", false)] // Empty last name
   [InlineData("John", "Doe", true)] // Valid
   public void Validate_FirstAndLastName_ReturnsExpected(
       string firstName, string lastName, bool isValid)
   {
       // Test implementation
   }
   ```

### Integration Testing

**Scope**: Test API endpoints with real database (in-memory or test database)

#### Test Database Strategy
- Use SQL Server LocalDB for integration tests
- Database created and seeded for each test run
- Transactions rolled back after each test

#### API Testing
- Test controller endpoints
- Validate HTTP status codes
- Verify response models
- Test authentication/authorization

### End-to-End Testing

**Framework**: Playwright or Selenium

#### Test Scenarios
- User registration and login flow
- Create person and add relationships
- Upload and tag photos
- Create wiki page
- Send message

### Performance Testing

**Tools**: JMeter, k6, or Azure Load Testing

#### Test Scenarios
- Load test: 1000 concurrent users
- Stress test: Find breaking point
- Endurance test: 24-hour sustained load
- Spike test: Sudden traffic increase

---

## Deployment Architecture

### Development Environment

- **Local Development**: IIS Express or Kestrel
- **Database**: SQL Server LocalDB
- **Blob Storage**: Azure Storage Emulator or Azurite
- **Build**: MSBuild with npm integration
- **Debug**: npm watch for Angular hot reload

### Staging Environment

- **Hosting**: Azure App Service (Windows)
- **Database**: Azure SQL Database (Basic or Standard tier)
- **Blob Storage**: Azure Blob Storage (Standard)
- **Configuration**: appsettings.Staging.json
- **Purpose**: Testing before production deployment

### Production Environment

#### Recommended Architecture

```
Internet
    │
    ↓
Azure Front Door (CDN + WAF)
    │
    ↓
Azure App Service (Premium tier)
    ├── Auto-scaling: 2-10 instances
    ├── Always On: Enabled
    └── HTTPS Only: Enabled
    │
    ↓
Azure SQL Database (Standard or Premium tier)
    ├── Geo-replication: Enabled
    ├── Automated backups: Enabled
    └── TDE: Enabled
    │
    ↓
Azure Blob Storage (Hot tier)
    ├── CDN integration
    ├── Geo-redundant: Enabled
    └── Versioning: Enabled
```

#### Configuration Management

- **App Settings**: Azure App Service configuration
- **Secrets**: Azure Key Vault
- **Connection Strings**: Managed identities where possible
- **Feature Flags**: Azure App Configuration

#### CI/CD Pipeline

1. **Build Pipeline**
   - Restore NuGet packages
   - Build .NET solution
   - npm install and build Angular
   - Run unit tests
   - Generate code coverage report

2. **Release Pipeline**
   - Deploy to staging
   - Run smoke tests
   - Manual approval gate
   - Deploy to production
   - Run health checks

### Disaster Recovery

- **Backup Strategy**: Daily automated backups with 30-day retention
- **Recovery Time Objective (RTO)**: 4 hours
- **Recovery Point Objective (RPO)**: 1 hour
- **Geo-Replication**: Database and blob storage replicated to secondary region

---

## Risk Mitigation

### Technical Risks

| Risk | Impact | Probability | Mitigation Strategy |
|------|--------|-------------|---------------------|
| Data loss | Critical | Low | Automated backups, geo-replication, version control |
| Security breach | Critical | Medium | Security audits, penetration testing, encryption |
| Performance degradation | High | Medium | Performance monitoring, caching, load testing |
| Third-party API failure | Medium | Medium | Retry logic, circuit breakers, graceful degradation |
| Database deadlocks | Medium | Low | Optimistic concurrency, query optimization |
| Azure service outage | High | Low | Multi-region deployment, disaster recovery plan |

### Business Risks

| Risk | Impact | Probability | Mitigation Strategy |
|------|--------|-------------|---------------------|
| Low user adoption | High | Medium | User research, intuitive UI, onboarding flow |
| Competition | Medium | High | Unique family-focused features, continuous innovation |
| Funding constraints | High | Low | Sustainable business model, phased development |
| Regulatory changes | Medium | Low | Compliance monitoring, legal consultation |

### Operational Risks

| Risk | Impact | Probability | Mitigation Strategy |
|------|--------|-------------|---------------------|
| Key person dependency | High | Medium | Documentation, knowledge sharing, code reviews |
| Support burden | Medium | High | Self-service tools, comprehensive help documentation |
| Technical debt | Medium | Medium | Regular refactoring, code quality standards |

---

## Next Steps

### Immediate Priorities (Next 3 Months)

1. **Phase 5: Research & DNA Integration**
   - Design DNA data model
   - Implement GEDCOM import
   - Integrate with FamilySearch API

2. **Performance Optimization**
   - Implement database indexing
   - Add Redis caching layer
   - Optimize Angular bundle size

3. **Security Enhancements**
   - Implement two-factor authentication
   - Add security audit logging
   - Penetration testing

### Medium-Term Goals (3-12 Months)

1. Complete Phase 7: Financial & Legal Tools
2. Complete Phase 8: Health & Medical History
3. Implement Progressive Web App (PWA)
4. Develop public API for integrations

### Long-Term Vision (1-3 Years)

1. AI-powered features (photo recognition, transcription)
2. Mobile native apps (iOS, Android)
3. Multi-language support
4. Platform for third-party developers

---

## Conclusion

RushtonRoots is designed as a comprehensive, scalable, and secure platform for family collaboration and history preservation. This design document provides a roadmap for implementing features in a phased approach, ensuring each phase builds upon previous work while delivering value to users.

The architecture follows industry best practices (Clean Architecture, SOLID principles) and leverages modern technologies (.NET 10, Angular 19) to create a maintainable and extensible codebase.

Regular reviews of this document will ensure the design remains aligned with user needs, technological advances, and business objectives.

---

## Appendix

### Glossary

- **GEDCOM**: Genealogical Data Communication - standard file format for genealogy data
- **HIPAA**: Health Insurance Portability and Accountability Act - U.S. healthcare privacy law
- **GDPR**: General Data Protection Regulation - EU data protection law
- **SAS Token**: Shared Access Signature - Azure time-limited access token
- **TDE**: Transparent Data Encryption - SQL Server encryption feature
- **WCAG**: Web Content Accessibility Guidelines - accessibility standards

### References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Angular Documentation](https://angular.io/docs)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Azure Architecture Center](https://docs.microsoft.com/azure/architecture/)

### Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | December 2025 | Development Team | Initial design document |

---

**End of Document**
