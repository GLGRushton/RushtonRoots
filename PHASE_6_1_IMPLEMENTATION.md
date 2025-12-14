# Phase 6.1 Implementation: Family Wiki System

## Overview
This document describes the implementation of Phase 6.1 from the ROADMAP.md - a comprehensive family wiki system that enables family members to create, organize, and search wiki-style pages for family topics, with full version history, categories, tags, and template support.

## Implementation Date
December 2025

## Features Implemented

### 1. Wiki-Style Pages for Family Topics
- **WikiPage Entity**: Core entity storing wiki page content in markdown format
- **Title & Slug**: Each page has a title and URL-friendly slug for easy navigation
- **Content Storage**: Markdown content stored directly in database
- **Summary/Excerpt**: Optional summary field for previews
- **Published Status**: Pages can be draft or published
- **View Counter**: Tracks popularity with automatic view count increment
- **Creator Tracking**: Links to user who created and last updated the page

### 2. Markdown Editor for Articles
- **Markdown Support**: Full markdown formatting support in content field
- **Rich Text Capability**: Support for headings, lists, links, images, code blocks, etc.
- **Content Validation**: Server-side validation ensures content is present and within limits
- **Frontend Integration Ready**: API returns markdown content for client-side rendering

### 3. Wiki Categories and Tags

#### Categories (Hierarchical Organization)
- **WikiCategory Entity**: Organize wiki pages into categories
- **Hierarchical Structure**: Support for parent-child category relationships
- **Category Properties**:
  - Name and slug
  - Description
  - Icon/emoji for visual identification
  - Display order for custom sorting
- **Category Navigation**: API endpoints to browse pages by category
- **Root Categories**: Special endpoint to get top-level categories

#### Tags (Flexible Classification)
- **WikiTag Entity**: Many-to-many relationship with wiki pages
- **Tag Properties**:
  - Name and slug
  - Description
  - Usage count (automatically updated)
- **Tag Cloud**: Popular tags endpoint for discovery
- **Tag Navigation**: Browse all pages with a specific tag
- **Automatic Maintenance**: Usage counts updated on page create/update/delete

### 4. Wiki Search and Navigation

#### Search Functionality
- **Full-Text Search**: Search across title, content, and summary
- **Category Filter**: Filter results by category
- **Tag Filter**: Filter by one or multiple tags
- **Published Filter**: Filter published vs. draft pages
- **Sorting Options**:
  - By title (alphabetical)
  - By creation date
  - By update date
  - By view count (popularity)
- **Pagination**: Support for page-by-page navigation
- **Total Count**: Returns both results and total count

#### Navigation
- **By Slug**: Direct access to pages via URL-friendly slugs
- **By Category**: Browse all pages in a category
- **By Tag**: Browse all pages with a tag
- **Recent Pages**: Get most recently updated pages
- **All Pages**: List all pages (with published filter)

### 5. Wiki Version History
- **WikiPageVersion Entity**: Complete version history for every page
- **Automatic Versioning**: New version created on every update
- **Version Properties**:
  - Version number (sequential)
  - Title, content, and summary at that version
  - User who made the update
  - Change description (optional commit message)
  - Timestamp
- **Version Navigation**: 
  - Get all versions for a page
  - Get specific version by number
- **Audit Trail**: Full history of changes for accountability

### 6. Wiki Templates

#### Template System
- **WikiTemplate Entity**: Reusable templates for common page types
- **Template Properties**:
  - Name and description
  - Template content with placeholders
  - Template type (Person, Place, Event, General, etc.)
  - Active status for enabling/disabling
  - Display order
- **Template Types**: Categorized templates for different use cases:
  - Person template (for family member biographies)
  - Place template (for family locations)
  - Event template (for family events)
  - General template (for any topic)
- **Template Application**: Pages can reference which template they use
- **Browse by Type**: Get all templates of a specific type

## Database Schema

### WikiPage
```csharp
- Id: int (PK)
- Title: string (required, max 200)
- Slug: string (required, max 250, unique)
- Content: string (required, markdown)
- Summary: string? (max 500)
- CategoryId: int? (FK to WikiCategory)
- TemplateId: int? (FK to WikiTemplate)
- CreatedByUserId: string (FK to ApplicationUser)
- LastUpdatedByUserId: string? (FK to ApplicationUser)
- IsPublished: bool (default false)
- ViewCount: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: Category, Template, CreatedByUser, LastUpdatedByUser, Versions, Tags
```

### WikiPageVersion
```csharp
- Id: int (PK)
- WikiPageId: int (FK to WikiPage)
- VersionNumber: int (sequential)
- Title: string (required, max 200)
- Content: string (required)
- Summary: string? (max 500)
- UpdatedByUserId: string (FK to ApplicationUser)
- ChangeDescription: string? (max 1000)
- CreatedDateTime: DateTime
- Navigation: WikiPage, UpdatedByUser
- Unique Index: (WikiPageId, VersionNumber)
```

### WikiCategory
```csharp
- Id: int (PK)
- Name: string (required, max 100)
- Slug: string (required, max 120, unique)
- Description: string? (max 500)
- ParentCategoryId: int? (FK to WikiCategory, self-referencing)
- Icon: string? (max 50, emoji or icon code)
- DisplayOrder: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: ParentCategory, ChildCategories, WikiPages
```

### WikiTag
```csharp
- Id: int (PK)
- Name: string (required, max 50)
- Slug: string (required, max 60, unique)
- Description: string? (max 200)
- UsageCount: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: WikiPages (many-to-many)
```

### WikiTemplate
```csharp
- Id: int (PK)
- Name: string (required, max 100)
- Description: string? (max 500)
- TemplateContent: string (required)
- TemplateType: string (required, max 50)
- IsActive: bool (default true)
- DisplayOrder: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: WikiPages
```

### WikiPageTags (Join Table)
```csharp
- WikiPageId: int (FK to WikiPage)
- WikiTagId: int (FK to WikiTag)
- Composite PK: (WikiPageId, WikiTagId)
```

## Service Layer

### WikiPageService
**Methods**:
- `GetByIdAsync(int id, bool incrementViewCount)`: Get page by ID with optional view increment
- `GetBySlugAsync(string slug, bool incrementViewCount)`: Get page by slug
- `GetAllAsync(bool publishedOnly)`: Get all pages
- `SearchAsync(WikiSearchRequest)`: Advanced search with filters and pagination
- `CreateAsync(CreateWikiPageRequest, string userId)`: Create new page with versioning
- `UpdateAsync(int id, UpdateWikiPageRequest, string userId)`: Update page with versioning
- `DeleteAsync(int id)`: Delete page
- `GetByCategoryAsync(int categoryId)`: Get pages in category
- `GetByTagAsync(int tagId)`: Get pages with tag
- `GetRecentAsync(int count, bool publishedOnly)`: Get recent pages
- `GetVersionHistoryAsync(int wikiPageId)`: Get all versions
- `GetVersionAsync(int wikiPageId, int versionNumber)`: Get specific version

**Features**:
- Automatic slug generation from title
- Slug uniqueness enforcement (with counter suffix if needed)
- Automatic version creation on create/update
- Tag usage count maintenance
- Input validation

### WikiCategoryService
**Methods**:
- `GetByIdAsync(int id)`: Get category by ID
- `GetBySlugAsync(string slug)`: Get category by slug
- `GetAllAsync()`: Get all categories
- `GetRootCategoriesAsync()`: Get top-level categories
- `CreateAsync(CreateWikiCategoryRequest)`: Create category
- `UpdateAsync(int id, UpdateWikiCategoryRequest)`: Update category
- `DeleteAsync(int id)`: Delete category

**Features**:
- Automatic slug generation
- Hierarchical category support

### WikiTagService
**Methods**:
- `GetByIdAsync(int id)`: Get tag by ID
- `GetBySlugAsync(string slug)`: Get tag by slug
- `GetAllAsync()`: Get all tags
- `GetPopularTagsAsync(int count)`: Get most used tags
- `CreateAsync(CreateWikiTagRequest)`: Create tag
- `DeleteAsync(int id)`: Delete tag

**Features**:
- Automatic slug generation
- Usage count tracking

### WikiTemplateService
**Methods**:
- `GetByIdAsync(int id)`: Get template by ID
- `GetAllAsync(bool activeOnly)`: Get all templates
- `GetByTypeAsync(string templateType)`: Get templates by type
- `CreateAsync(CreateWikiTemplateRequest)`: Create template
- `UpdateAsync(int id, UpdateWikiTemplateRequest)`: Update template
- `DeleteAsync(int id)`: Delete template

**Features**:
- Template type filtering
- Active/inactive status

## API Endpoints

### WikiPageController (`/api/WikiPage`)
- `GET /api/WikiPage`: Get all wiki pages
- `GET /api/WikiPage/{id}`: Get page by ID (increments view count)
- `GET /api/WikiPage/slug/{slug}`: Get page by slug (increments view count)
- `POST /api/WikiPage/search`: Search pages with filters
- `GET /api/WikiPage/category/{categoryId}`: Get pages by category
- `GET /api/WikiPage/tag/{tagId}`: Get pages by tag
- `GET /api/WikiPage/recent?count={count}&publishedOnly={bool}`: Get recent pages
- `POST /api/WikiPage`: Create page (Admin/HouseholdAdmin only)
- `PUT /api/WikiPage/{id}`: Update page (Admin/HouseholdAdmin only)
- `DELETE /api/WikiPage/{id}`: Delete page (Admin only)
- `GET /api/WikiPage/{id}/versions`: Get version history
- `GET /api/WikiPage/{id}/versions/{versionNumber}`: Get specific version

### WikiCategoryController (`/api/WikiCategory`)
- `GET /api/WikiCategory`: Get all categories
- `GET /api/WikiCategory/root`: Get root categories
- `GET /api/WikiCategory/{id}`: Get category by ID
- `GET /api/WikiCategory/slug/{slug}`: Get category by slug
- `POST /api/WikiCategory`: Create category (Admin/HouseholdAdmin only)
- `PUT /api/WikiCategory/{id}`: Update category (Admin/HouseholdAdmin only)
- `DELETE /api/WikiCategory/{id}`: Delete category (Admin only)

### WikiTagController (`/api/WikiTag`)
- `GET /api/WikiTag`: Get all tags
- `GET /api/WikiTag/popular?count={count}`: Get popular tags
- `GET /api/WikiTag/{id}`: Get tag by ID
- `GET /api/WikiTag/slug/{slug}`: Get tag by slug
- `POST /api/WikiTag`: Create tag (Admin/HouseholdAdmin only)
- `DELETE /api/WikiTag/{id}`: Delete tag (Admin only)

### WikiTemplateController (`/api/WikiTemplate`)
- `GET /api/WikiTemplate?activeOnly={bool}`: Get all templates
- `GET /api/WikiTemplate/type/{templateType}`: Get templates by type
- `GET /api/WikiTemplate/{id}`: Get template by ID
- `POST /api/WikiTemplate`: Create template (Admin only)
- `PUT /api/WikiTemplate/{id}`: Update template (Admin only)
- `DELETE /api/WikiTemplate/{id}`: Delete template (Admin only)

## UI Components

### Views
- **Wiki/Index.cshtml**: Main wiki landing page
  - Categories grid with icons and page counts
  - Recently updated pages list
  - Popular tags cloud
  - Interactive navigation to browse content

### Features
- Responsive design with green theme
- Category cards with hover effects
- Page list with metadata (updated date, author, views)
- Tag cloud with usage counts
- Click-to-navigate functionality
- Error handling for API failures

## Security & Permissions

### Role-Based Access Control
- **All Users**: View published wiki pages
- **Admin/HouseholdAdmin**: Create and update pages, categories, tags
- **Admin Only**: Delete pages, categories, tags; manage templates

### Data Validation
- Title required, max 200 characters
- Content required
- Summary optional, max 500 characters
- Change description optional, max 1000 characters
- All slugs unique per entity type

## Auto-Registration with Autofac

All services, repositories, mappers, and validators automatically registered by convention:
- Classes ending in `Service` → registered as services
- Classes ending in `Repository` → registered as repositories
- Classes ending in `Mapper` → registered as mappers
- Classes ending in `Validator` → registered as validators

## Key Design Decisions

### 1. Markdown Storage
- Store markdown directly in database for simplicity
- Client-side rendering for flexibility
- Server validates presence and length only

### 2. Slug Generation
- Automatic generation from title
- Lowercase, hyphen-separated
- Unique slugs enforced with counter suffix if needed
- Example: "Family History" → "family-history", "family-history-2", etc.

### 3. Version History
- Every update creates a new version
- First version (v1) created on page creation
- Version numbers sequential per page
- Optional change description for documentation

### 4. Tag Usage Counts
- Automatically maintained on page operations
- Enables "popular tags" feature
- Helps users discover frequently used topics

### 5. View Counting
- Optional increment on GET endpoints
- Allows tracking page popularity
- Used for "most viewed" features

## Testing Recommendations

### Manual Testing
1. Create a category (e.g., "Family Stories" with 📖 icon)
2. Create a tag (e.g., "traditions")
3. Create a wiki page with category and tags
4. Update the page and verify version history
5. Browse by category and tag
6. Search for pages
7. View recent pages
8. Check view count increments

### Unit Testing
- Service layer validation
- Mapper transformations
- Repository queries
- Slug generation logic
- Version number sequencing

## Future Enhancements

### Potential Additions
- **Rich Markdown Editor UI**: Integrated WYSIWYG editor for easier content creation
- **Image Upload**: Direct image upload for wiki pages
- **Cross-References**: Link between wiki pages
- **Page Comments**: Discussion on wiki pages
- **Contribution Workflow**: Suggest edits for review
- **Export**: PDF or HTML export of wiki pages
- **Search Highlighting**: Highlight search terms in results
- **Related Pages**: Suggest related pages based on tags/category

## Migration Information

**Migration Name**: `AddWikiEntities`  
**Created**: December 14, 2025

**Tables Created**:
- `WikiPages`
- `WikiPageVersions`
- `WikiCategories`
- `WikiTags`
- `WikiTemplates`
- `WikiPageTags` (join table)

**Indexes Created**:
- Unique index on WikiPage.Slug
- Unique index on WikiCategory.Slug
- Unique index on WikiTag.Slug
- Unique composite index on WikiPageVersion (WikiPageId, VersionNumber)

## Success Metrics

✅ **All Phase 6.1 Requirements Met**:
1. ✅ Wiki-style pages for family topics
2. ✅ Markdown editor support (via API)
3. ✅ Wiki categories and tags
4. ✅ Wiki search and navigation
5. ✅ Wiki version history
6. ✅ Wiki templates

## Conclusion

Phase 6.1 establishes a solid foundation for the family knowledge base. The wiki system provides comprehensive tools for organizing, creating, and discovering family information in a structured, searchable format. With full version history, flexible categorization, and template support, family members can collaboratively build and maintain a rich repository of family knowledge that will grow with the family for generations.
