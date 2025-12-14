# Phase 6.2 Implementation: Stories & Memories

## Overview
This document describes the implementation of Phase 6.2 from the ROADMAP.md - a comprehensive stories and memories system that enables family members to submit, categorize, and organize family stories with person associations, timeline views, and story collections.

## Implementation Date
December 2025

## Features Implemented

### 1. Story Submission System
- **Story Entity**: Core entity storing family stories and memories
- **Title & Slug**: Each story has a title and URL-friendly slug for easy navigation
- **Content Storage**: Full story content stored directly in database
- **Summary/Excerpt**: Optional summary field for previews
- **Submission Tracking**: Links to user who submitted the story
- **Published Status**: Stories can be draft or published
- **View Counter**: Tracks popularity with automatic view count increment
- **Collaborative Editing**: Optional flag to allow collaborative storytelling

### 2. Story Categorization
- **Category Field**: String-based categorization for flexible organization
- **Predefined Categories**: Support for common categories:
  - Childhood memories
  - War stories
  - Recipes
  - Traditions
  - Life lessons
  - Family events
  - Travel stories
  - Career milestones
  - And more...
- **Category Navigation**: API endpoints to browse stories by category
- **Category Discovery**: Automatic category list generation from existing stories
- **Category Filtering**: Search and filter stories by category

### 3. Story-to-Person Associations
- **StoryPerson Join Table**: Many-to-many relationship between stories and people
- **Multiple People**: Each story can be associated with multiple family members
- **Role in Story**: Optional field to describe each person's role in the story
- **Person-Based Queries**: Find all stories about a specific person
- **Associated People Display**: Stories show all related family members

### 4. Story Timeline View
- **Story Date**: Optional date field for when the events occurred
- **Timeline Sorting**: Stories can be sorted by story date
- **Date Range Filtering**: Search stories within specific date ranges
- **Location Field**: Optional location where story took place
- **Chronological Display**: Support for timeline-based story browsing

### 5. Story Collections/Books
- **StoryCollection Entity**: Group related stories into collections
- **Collection Properties**:
  - Name and slug
  - Description
  - Cover image URL
  - Display order
  - Published status
- **Story-Collection Association**: Stories can belong to a collection
- **Collection Navigation**: Browse stories within a collection
- **Collection Management**: Create, update, and delete collections
- **Collection Counts**: Automatic story count per collection

### 6. Collaborative Storytelling
- **AllowCollaboration Flag**: Stories can enable collaborative editing
- **Submitter Tracking**: Original submitter is tracked
- **Comment Support**: Existing comment system can be used for story discussions
- **Version History Ready**: Infrastructure supports future version tracking
- **Edit Permissions**: Admin/HouseholdAdmin can edit all stories

## Database Schema

### Story
```csharp
- Id: int (PK)
- Title: string (required, max 200)
- Slug: string (required, max 250, unique)
- Content: string (required)
- Summary: string? (max 1000)
- Category: string (required, max 100)
- StoryDate: DateTime? - When events occurred
- Location: string? (max 500)
- SubmittedByUserId: string (FK to ApplicationUser)
- IsPublished: bool (default false)
- ViewCount: int (default 0)
- AllowCollaboration: bool (default true)
- CollectionId: int? (FK to StoryCollection)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: SubmittedByUser, Collection, StoryPeople
```

### StoryPerson
```csharp
- StoryId: int (FK to Story)
- PersonId: int (FK to Person)
- RoleInStory: string? (max 200)
- Composite PK: (StoryId, PersonId)
```

### StoryCollection
```csharp
- Id: int (PK)
- Name: string (required, max 200)
- Slug: string (required, max 250, unique)
- Description: string? (max 2000)
- CoverImageUrl: string? (max 500)
- CreatedByUserId: string (FK to ApplicationUser)
- IsPublished: bool (default false)
- DisplayOrder: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: CreatedByUser, Stories
```

## Service Layer

### StoryService
**Methods**:
- `GetByIdAsync(int id, bool incrementViewCount)`: Get story by ID with optional view increment
- `GetBySlugAsync(string slug, bool incrementViewCount)`: Get story by slug
- `GetAllAsync(bool publishedOnly)`: Get all stories
- `GetByCategoryAsync(string category, bool publishedOnly)`: Get stories by category
- `GetByPersonIdAsync(int personId, bool publishedOnly)`: Get stories about a person
- `GetByCollectionIdAsync(int collectionId, bool publishedOnly)`: Get stories in collection
- `GetRecentAsync(int count, bool publishedOnly)`: Get recent stories
- `SearchAsync(SearchStoryRequest)`: Advanced search with filters and pagination
- `CreateAsync(CreateStoryRequest, string userId)`: Create new story
- `UpdateAsync(int id, UpdateStoryRequest, string userId)`: Update story
- `DeleteAsync(int id)`: Delete story
- `GetCategoriesAsync()`: Get all story categories

**Features**:
- Automatic slug generation from title
- Slug uniqueness enforcement (with counter suffix if needed)
- Person association management
- Input validation
- Search with multiple filters (text, category, person, collection, date range)
- Flexible sorting (title, date, views, updated)
- Pagination support

### StoryCollectionService
**Methods**:
- `GetByIdAsync(int id, bool includeStories)`: Get collection by ID
- `GetBySlugAsync(string slug, bool includeStories)`: Get collection by slug
- `GetAllAsync(bool publishedOnly)`: Get all collections
- `CreateAsync(CreateStoryCollectionRequest, string userId)`: Create collection
- `UpdateAsync(int id, UpdateStoryCollectionRequest)`: Update collection
- `DeleteAsync(int id)`: Delete collection

**Features**:
- Automatic slug generation
- Story count tracking
- Display order support

## API Endpoints

### StoryController (`/api/Story`)
- `GET /api/Story?publishedOnly={bool}`: Get all stories
- `GET /api/Story/{id}?incrementViewCount={bool}`: Get story by ID
- `GET /api/Story/slug/{slug}?incrementViewCount={bool}`: Get story by slug
- `POST /api/Story/search`: Search stories with filters
- `GET /api/Story/category/{category}?publishedOnly={bool}`: Get stories by category
- `GET /api/Story/person/{personId}?publishedOnly={bool}`: Get stories about person
- `GET /api/Story/collection/{collectionId}?publishedOnly={bool}`: Get stories in collection
- `GET /api/Story/recent?count={count}&publishedOnly={bool}`: Get recent stories
- `GET /api/Story/categories`: Get all story categories
- `POST /api/Story`: Create story (Admin/HouseholdAdmin only)
- `PUT /api/Story/{id}`: Update story (Admin/HouseholdAdmin only)
- `DELETE /api/Story/{id}`: Delete story (Admin only)

### StoryCollectionController (`/api/StoryCollection`)
- `GET /api/StoryCollection?publishedOnly={bool}`: Get all collections
- `GET /api/StoryCollection/{id}?includeStories={bool}`: Get collection by ID
- `GET /api/StoryCollection/slug/{slug}?includeStories={bool}`: Get collection by slug
- `POST /api/StoryCollection`: Create collection (Admin/HouseholdAdmin only)
- `PUT /api/StoryCollection/{id}`: Update collection (Admin/HouseholdAdmin only)
- `DELETE /api/StoryCollection/{id}`: Delete collection (Admin only)

## UI Components

### Views
- **StoryView/Index.cshtml**: Main stories landing page
  - Category browsing cards
  - Story collections display
  - Recently shared stories list
  - Interactive navigation

### Features
- Responsive design with green theme matching family wiki
- Category cards for easy browsing
- Collection cards with story counts
- Story list with rich metadata (date, author, views, category)
- Click-to-navigate functionality
- Error handling for API failures
- Loading states

## Security & Permissions

### Role-Based Access Control
- **All Users**: View published stories
- **Admin/HouseholdAdmin**: Create and update stories and collections
- **Admin Only**: Delete stories and collections

### Data Validation
- Title required, max 200 characters
- Content required
- Category required, max 100 characters
- Summary optional, max 1000 characters
- Location optional, max 500 characters
- All slugs unique per entity type

## Auto-Registration with Autofac

All services and repositories automatically registered by convention:
- `StoryService` → `IStoryService`
- `StoryCollectionService` → `IStoryCollectionService`
- `StoryRepository` → `IStoryRepository`
- `StoryCollectionRepository` → `IStoryCollectionRepository`

## Key Design Decisions

### 1. Flexible Categorization
- String-based categories for maximum flexibility
- Categories discovered automatically from existing stories
- Easy to add new categories without database changes
- No rigid category structure to limit creativity

### 2. Story-Person Associations
- Many-to-many relationship allows multiple people per story
- Optional "role in story" field adds context
- Enables powerful person-centric story discovery
- Bi-directional navigation (person → stories, story → people)

### 3. Collections as Books
- Collections group related stories thematically
- Optional cover images for visual appeal
- Display order allows custom sequencing
- Stories can exist independently or in collections
- Collections support family "books" or "albums"

### 4. Timeline Support
- Optional story date separate from creation date
- Enables chronological storytelling
- Supports historical family stories
- Location field adds geographical context

### 5. Collaborative Features
- AllowCollaboration flag enables shared storytelling
- Existing comment system provides discussion
- Infrastructure ready for version history
- Submitter tracking maintains attribution

### 6. Slug Generation
- Automatic generation from title
- Lowercase, hyphen-separated
- Unique slugs enforced with counter suffix if needed
- Example: "Grandma's Recipe" → "grandmas-recipe", "grandmas-recipe-2", etc.

### 7. View Counting
- Optional increment on GET endpoints
- Tracks story popularity
- Used for "most viewed" features
- Helps identify valuable content

## Testing Recommendations

### Manual Testing
1. Create a story with category "Childhood"
2. Associate story with multiple family members
3. Create a story collection "Family Recipes"
4. Add stories to the collection
5. Browse stories by category
6. Search for stories about a specific person
7. Filter stories by date range
8. Check view count increments
9. Test collaborative flag functionality

### Unit Testing
- Service layer validation
- Repository queries
- Slug generation logic
- Person association management
- Search filtering logic

## Future Enhancements

### Potential Additions
- **Rich Text Editor**: WYSIWYG editor for story content
- **Story Images**: Inline images within story content
- **Audio Recordings**: Attach audio narration to stories
- **Story Sharing**: Share stories outside the family (with privacy controls)
- **Story Printing**: Print individual stories or collections as PDF books
- **Story Ratings**: Let family members rate stories
- **Story Tags**: Additional tagging beyond categories
- **Story Prompts**: Suggested story ideas to encourage submissions
- **Version History**: Track changes to collaborative stories
- **Story Analytics**: Track which stories are most read/shared

## Migration Information

**Migration Name**: `AddStoryEntities`  
**Created**: December 14, 2025

**Tables Created**:
- `Stories`
- `StoryPeople` (join table)
- `StoryCollections`

**Indexes Created**:
- Unique index on Story.Slug
- Unique index on StoryCollection.Slug
- Composite primary key on StoryPeople (StoryId, PersonId)

## Success Metrics

✅ **All Phase 6.2 Requirements Met**:
1. ✅ Create story submission system
2. ✅ Implement story categorization (childhood, war stories, recipes, etc.)
3. ✅ Add story-to-person associations
4. ✅ Create story timeline view (via StoryDate and sorting)
5. ✅ Build story collections/books
6. ✅ Implement collaborative storytelling (via AllowCollaboration flag)

## Conclusion

Phase 6.2 establishes a comprehensive stories and memories system that enables family members to preserve and share their experiences. With flexible categorization, person associations, timeline support, and story collections, the platform provides multiple ways to organize and discover family stories. The collaborative features encourage family participation in building a rich repository of memories that will be treasured for generations to come.
