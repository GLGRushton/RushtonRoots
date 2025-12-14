# Phase 6.3 Implementation: Recipes & Traditions

## Overview
This document describes the implementation of Phase 6.3 from the ROADMAP.md - a comprehensive family recipes and traditions system that enables family members to preserve culinary heritage and document cultural customs that have been passed down through generations.

## Implementation Date
December 2025

## Features Implemented

### 1. Family Recipe Repository
- **Recipe Entity**: Core entity storing family recipes with all necessary details
- **Recipe Properties**:
  - Name and URL-friendly slug
  - Description and category (Appetizer, Main Course, Dessert, Beverage, etc.)
  - Ingredients list (stored as structured text)
  - Step-by-step cooking instructions
  - Preparation and cooking time in minutes
  - Number of servings
  - Cuisine type (Italian, Mexican, American, etc.)
  - Photo URL for recipe image
  - Special notes and tips
  - Originator person (who created the recipe)
  - Published status and favorite flag
  - View count and rating metrics
- **Recipe Management**: Full CRUD operations with search and filtering capabilities

### 2. Recipe Cards with Photos
- **Recipe Card Display**: Visual recipe cards showing key information
- **Photo Support**: Each recipe can have a featured photo
- **Recipe Details Include**:
  - Recipe name and category
  - Preparation and cooking time
  - Number of servings
  - Average rating and rating count
  - Favorite indicator (star)
  - Originator attribution
- **Responsive Grid Layout**: Recipes displayed in responsive card grid

### 3. Recipe Ratings and Comments
- **RecipeRating Entity**: User ratings and reviews for recipes
- **Rating System**: 1-5 star rating scale
- **Rating Features**:
  - One rating per user per recipe (unique constraint)
  - Optional comment/review
  - "Has Made" flag to indicate user has tried the recipe
  - Automatic average rating calculation
  - Rating count tracking
- **Rating Service**: Manages rating CRUD operations with automatic aggregation
- **User Attribution**: Ratings linked to user accounts with timestamps

### 4. Tradition Documentation
- **Tradition Entity**: Documents family traditions and customs
- **Tradition Properties**:
  - Name and URL-friendly slug
  - Detailed description
  - Category (Holiday, Birthday, Anniversary, Seasonal, Religious, Cultural)
  - Frequency (Yearly, Monthly, Weekly, Special Occasions)
  - Started date (when tradition began)
  - Person who started the tradition
  - Current status (Active, Discontinued, Evolving)
  - Photo URL
  - How to celebrate instructions
  - Associated items (foods, decorations, etc.)
  - Published status
  - View count
- **Tradition Management**: Full CRUD operations with search and filtering

### 5. Tradition Timeline
- **TraditionTimeline Entity**: Tracks the evolution and history of traditions
- **Timeline Properties**:
  - Event date and title
  - Detailed description
  - Event type (Started, Modified, Paused, Resumed, Special Observance)
  - User who recorded the entry
  - Optional photo for the timeline event
  - Timestamps
- **Timeline Display**: Chronological display of tradition's history
- **Timeline Management**: Add, update, and delete timeline entries

### 6. Recipe Sharing and Printing
- **API Endpoints**: RESTful API for recipe access and sharing
- **Search Functionality**: Advanced recipe search with multiple filters
- **Category Browsing**: Browse recipes by category or cuisine
- **Favorites System**: Mark and filter favorite recipes
- **Export Ready**: Recipe data structured for easy export/printing (future enhancement)
- **View Tracking**: Track recipe popularity via view counts

## Database Schema

### Recipe
```csharp
- Id: int (PK)
- Name: string (required, max 200)
- Slug: string (required, max 250, unique)
- Description: string? (max 1000)
- Ingredients: string (required)
- Instructions: string (required)
- PrepTimeMinutes: int?
- CookTimeMinutes: int?
- Servings: int?
- Category: string (required, max 100)
- Cuisine: string? (max 100)
- PhotoUrl: string? (max 500)
- Notes: string? (max 2000)
- OriginatorPersonId: int? (FK to Person)
- SubmittedByUserId: string (FK to ApplicationUser)
- IsPublished: bool (default false)
- IsFavorite: bool (default false)
- AverageRating: decimal (precision 3,2, default 0)
- RatingCount: int (default 0)
- ViewCount: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: SubmittedByUser, OriginatorPerson, Ratings
```

### RecipeRating
```csharp
- Id: int (PK)
- RecipeId: int (FK to Recipe)
- UserId: string (FK to ApplicationUser)
- Rating: int (required, 1-5)
- Comment: string? (max 2000)
- HasMade: bool (default false)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Unique Index: (RecipeId, UserId)
- Navigation: Recipe, User
```

### Tradition
```csharp
- Id: int (PK)
- Name: string (required, max 200)
- Slug: string (required, max 250, unique)
- Description: string (required)
- Category: string (required, max 100)
- Frequency: string (required, max 100)
- StartedDate: DateTime?
- StartedByPersonId: int? (FK to Person)
- Status: string (required, max 50, default "Active")
- PhotoUrl: string? (max 500)
- HowToCelebrate: string? (max 2000)
- AssociatedItems: string? (max 1000)
- SubmittedByUserId: string (FK to ApplicationUser)
- IsPublished: bool (default false)
- ViewCount: int (default 0)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: SubmittedByUser, StartedByPerson, Timeline
```

### TraditionTimeline
```csharp
- Id: int (PK)
- TraditionId: int (FK to Tradition)
- EventDate: DateTime (required)
- Title: string (required, max 200)
- Description: string (required, max 2000)
- EventType: string (required, max 100)
- RecordedByUserId: string (FK to ApplicationUser)
- PhotoUrl: string? (max 500)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Navigation: Tradition, RecordedByUser
```

## Service Layer

### RecipeService
**Methods**:
- `GetByIdAsync(int id, bool incrementViewCount)`: Get recipe by ID
- `GetBySlugAsync(string slug, bool incrementViewCount)`: Get recipe by slug
- `GetAllAsync(bool publishedOnly)`: Get all recipes
- `GetByCategoryAsync(string category, bool publishedOnly)`: Get recipes by category
- `GetByCuisineAsync(string cuisine, bool publishedOnly)`: Get recipes by cuisine
- `GetFavoritesAsync(bool publishedOnly)`: Get favorite recipes
- `GetRecentAsync(int count, bool publishedOnly)`: Get recent recipes
- `GetByPersonAsync(int personId, bool publishedOnly)`: Get recipes by originator
- `SearchAsync(SearchRecipeRequest)`: Advanced search with filters
- `CreateAsync(CreateRecipeRequest, string userId)`: Create new recipe
- `UpdateAsync(int id, UpdateRecipeRequest, string userId)`: Update recipe
- `DeleteAsync(int id)`: Delete recipe
- `GetCategoriesAsync()`: Get all categories
- `GetCuisinesAsync()`: Get all cuisines

**Features**:
- Automatic slug generation and uniqueness enforcement
- View count tracking
- Search with multiple filters (text, category, cuisine, time, rating)
- Flexible sorting options

### RecipeRatingService
**Methods**:
- `GetByIdAsync(int id)`: Get rating by ID
- `GetByRecipeAndUserAsync(int recipeId, string userId)`: Get user's rating for recipe
- `GetByRecipeAsync(int recipeId)`: Get all ratings for recipe
- `CreateAsync(CreateRecipeRatingRequest, string userId)`: Create new rating
- `UpdateAsync(int id, UpdateRecipeRatingRequest, string userId)`: Update rating
- `DeleteAsync(int id)`: Delete rating

**Features**:
- One rating per user per recipe enforcement
- Automatic average rating calculation
- Rating count updates
- User authorization checks

### TraditionService
**Methods**:
- `GetByIdAsync(int id, bool incrementViewCount)`: Get tradition by ID
- `GetBySlugAsync(string slug, bool incrementViewCount)`: Get tradition by slug
- `GetAllAsync(bool publishedOnly)`: Get all traditions
- `GetByCategoryAsync(string category, bool publishedOnly)`: Get traditions by category
- `GetByStatusAsync(string status, bool publishedOnly)`: Get traditions by status
- `GetByPersonAsync(int personId, bool publishedOnly)`: Get traditions by starter
- `GetRecentAsync(int count, bool publishedOnly)`: Get recent traditions
- `SearchAsync(SearchTraditionRequest)`: Advanced search with filters
- `CreateAsync(CreateTraditionRequest, string userId)`: Create new tradition
- `UpdateAsync(int id, UpdateTraditionRequest, string userId)`: Update tradition
- `DeleteAsync(int id)`: Delete tradition
- `GetCategoriesAsync()`: Get all categories

**Features**:
- Automatic slug generation
- Timeline inclusion in results
- Status-based filtering

### TraditionTimelineService
**Methods**:
- `GetByIdAsync(int id)`: Get timeline entry by ID
- `GetByTraditionAsync(int traditionId)`: Get all timeline entries for tradition
- `CreateAsync(CreateTraditionTimelineRequest, string userId)`: Create timeline entry
- `UpdateAsync(int id, UpdateTraditionTimelineRequest, string userId)`: Update timeline entry
- `DeleteAsync(int id)`: Delete timeline entry

**Features**:
- Chronological ordering
- User attribution

## API Endpoints

### RecipeController (`/api/Recipe`)
- `GET /api/Recipe`: Get all recipes
- `GET /api/Recipe/{id}`: Get recipe by ID (increments view count)
- `GET /api/Recipe/slug/{slug}`: Get recipe by slug
- `POST /api/Recipe/search`: Search recipes with filters
- `GET /api/Recipe/category/{category}`: Get recipes by category
- `GET /api/Recipe/cuisine/{cuisine}`: Get recipes by cuisine
- `GET /api/Recipe/favorites`: Get favorite recipes
- `GET /api/Recipe/recent?count={count}`: Get recent recipes
- `GET /api/Recipe/person/{personId}`: Get recipes by person
- `GET /api/Recipe/categories`: Get all categories
- `GET /api/Recipe/cuisines`: Get all cuisines
- `POST /api/Recipe`: Create recipe (Admin/HouseholdAdmin only)
- `PUT /api/Recipe/{id}`: Update recipe (Admin/HouseholdAdmin only)
- `DELETE /api/Recipe/{id}`: Delete recipe (Admin only)

### RecipeRatingController (`/api/RecipeRating`)
- `GET /api/RecipeRating/{id}`: Get rating by ID
- `GET /api/RecipeRating/recipe/{recipeId}/user`: Get current user's rating
- `GET /api/RecipeRating/recipe/{recipeId}`: Get all ratings for recipe
- `POST /api/RecipeRating`: Create rating
- `PUT /api/RecipeRating/{id}`: Update rating
- `DELETE /api/RecipeRating/{id}`: Delete rating

### TraditionController (`/api/Tradition`)
- `GET /api/Tradition`: Get all traditions
- `GET /api/Tradition/{id}`: Get tradition by ID (increments view count)
- `GET /api/Tradition/slug/{slug}`: Get tradition by slug
- `POST /api/Tradition/search`: Search traditions with filters
- `GET /api/Tradition/category/{category}`: Get traditions by category
- `GET /api/Tradition/status/{status}`: Get traditions by status
- `GET /api/Tradition/person/{personId}`: Get traditions by person
- `GET /api/Tradition/recent?count={count}`: Get recent traditions
- `GET /api/Tradition/categories`: Get all categories
- `POST /api/Tradition`: Create tradition (Admin/HouseholdAdmin only)
- `PUT /api/Tradition/{id}`: Update tradition (Admin/HouseholdAdmin only)
- `DELETE /api/Tradition/{id}`: Delete tradition (Admin only)

### TraditionTimelineController (`/api/TraditionTimeline`)
- `GET /api/TraditionTimeline/{id}`: Get timeline entry by ID
- `GET /api/TraditionTimeline/tradition/{traditionId}`: Get timeline for tradition
- `POST /api/TraditionTimeline`: Create timeline entry (Admin/HouseholdAdmin only)
- `PUT /api/TraditionTimeline/{id}`: Update timeline entry (Admin/HouseholdAdmin only)
- `DELETE /api/TraditionTimeline/{id}`: Delete timeline entry (Admin only)

## UI Components

### Views
- **Recipe/Index.cshtml**: Main recipe browsing page
  - Category grid for easy navigation
  - Favorite recipes section with star indicators
  - Recently added recipes with ratings
  - Recipe cards with photos, times, servings, ratings
  - Responsive grid layout

- **Tradition/Index.cshtml**: Main tradition browsing page
  - Category grid for tradition types
  - Active traditions section
  - Recently added traditions
  - Tradition cards with photos, status badges, timeline counts
  - Responsive grid layout

### Features
- Responsive design with green theme matching other family features
- Category browsing cards with hover effects
- Recipe/Tradition cards showing rich metadata
- Rating display with star visualization
- Click-to-navigate functionality (placeholders for future detail views)
- Error handling for API failures
- Loading states

## Security & Permissions

### Role-Based Access Control
- **All Users**: View published recipes and traditions, rate recipes
- **Admin/HouseholdAdmin**: Create and update recipes and traditions
- **Admin Only**: Delete recipes and traditions
- **Rating Authorization**: Users can only update/delete their own ratings

### Data Validation
- Recipe name required, max 200 characters
- Ingredients and instructions required
- Rating must be 1-5
- One rating per user per recipe (database constraint)
- Tradition name and description required
- All slugs unique per entity type

## Auto-Registration with Autofac

All services and repositories automatically registered by convention:
- `RecipeService` → `IRecipeService`
- `RecipeRatingService` → `IRecipeRatingService`
- `TraditionService` → `ITraditionService`
- `TraditionTimelineService` → `ITraditionTimelineService`
- `RecipeRepository` → `IRecipeRepository`
- `RecipeRatingRepository` → `IRecipeRatingRepository`
- `TraditionRepository` → `ITraditionRepository`
- `TraditionTimelineRepository` → `ITraditionTimelineRepository`

## Key Design Decisions

### 1. Recipe Structure
- **Ingredients as Text**: Stored as structured text (JSON or delimited) for flexibility
- **Time Tracking**: Separate prep and cook times for accurate planning
- **Multi-Category**: Each recipe has category and optional cuisine for dual organization
- **Originator Attribution**: Link to Person entity to credit recipe creators

### 2. Rating System
- **One Per User**: Database constraint ensures one rating per user per recipe
- **Automatic Aggregation**: Average rating and count updated automatically
- **Comment Optional**: Ratings can have optional review text
- **"Has Made" Flag**: Track which users have actually tried the recipe

### 3. Tradition Timeline
- **Chronological History**: Timeline entries track tradition evolution
- **Event Types**: Categorize timeline events (Started, Modified, etc.)
- **User Attribution**: Track who recorded each timeline entry
- **Photo Support**: Timeline entries can include photos

### 4. Slug Generation
- **Automatic from Name**: Generate URL-friendly slugs
- **Uniqueness Enforcement**: Counter suffix added if needed (e.g., "apple-pie-2")
- **SEO Friendly**: Lowercase, hyphen-separated format

### 5. View Counting
- **Optional Increment**: View count can be incremented on GET requests
- **Popularity Tracking**: Enables "most viewed" features
- **Recipe/Tradition Analytics**: Track engagement with content

### 6. Favorite System
- **Boolean Flag**: Simple favorite indicator per recipe
- **Quick Filtering**: Easy to filter and display favorites
- **User-Specific**: Future enhancement could make favorites per-user

## Testing Recommendations

### Manual Testing
1. Create a recipe with all fields populated
2. Add multiple ratings to the recipe and verify average calculation
3. Mark a recipe as favorite and verify it appears in favorites section
4. Create a tradition with initial date and started by person
5. Add timeline entries to track tradition evolution
6. Browse recipes by category and cuisine
7. Search for recipes with various filters
8. View recipe/tradition detail pages
9. Test rating creation, update, and deletion
10. Verify one-rating-per-user constraint

### Unit Testing
- Service layer validation
- Repository queries
- Slug generation logic
- Rating aggregation calculation
- Timeline chronological sorting
- Search filtering logic

## Future Enhancements

### Recipe Features
- **Rich Text Editor**: WYSIWYG editor for instructions
- **Ingredient Parser**: Parse and structure ingredient lists
- **Unit Conversion**: Convert measurements (cups to grams, etc.)
- **Nutrition Info**: Calculate or allow manual entry of nutrition facts
- **Recipe Scaling**: Automatically scale ingredients for different servings
- **Print Formatting**: Beautiful printable recipe cards
- **Recipe Collections**: Group related recipes into cookbooks
- **Shopping Lists**: Generate shopping lists from recipes
- **Cooking Mode**: Step-by-step cooking interface
- **Recipe Variations**: Track and document recipe variations

### Tradition Features
- **Calendar Integration**: Link traditions to specific dates
- **Notification Reminders**: Remind family of upcoming traditions
- **Tradition Photos Gallery**: Multiple photos per tradition
- **Participation Tracking**: Track which family members participate
- **Tradition Voting**: Vote on continuing or modifying traditions
- **Cross-Linking**: Link traditions to related recipes, stories, photos
- **Video Documentation**: Record video of traditions being practiced

### Sharing Features
- **PDF Export**: Export recipes as formatted PDFs
- **Email Sharing**: Email recipes to family members
- **Recipe Cards**: Generate printable recipe cards
- **QR Codes**: Generate QR codes for easy recipe sharing
- **Social Sharing**: Share to social media
- **Public Links**: Generate shareable public links

## Migration Information

**Migration Name**: `AddRecipeAndTraditionEntities`  
**Created**: December 14, 2025

**Tables Created**:
- `Recipes`
- `RecipeRatings`
- `Traditions`
- `TraditionTimelines`

**Indexes Created**:
- Unique index on Recipe.Slug
- Unique index on Tradition.Slug
- Unique composite index on RecipeRating (RecipeId, UserId)

## Success Metrics

✅ **All Phase 6.3 Requirements Met**:
1. ✅ Create family recipe repository
2. ✅ Implement recipe cards with photos
3. ✅ Add recipe ratings and comments
4. ✅ Create tradition documentation (holidays, customs)
5. ✅ Build tradition timeline (when traditions started)
6. ✅ Implement recipe sharing and printing (API ready, export features future enhancement)

## Conclusion

Phase 6.3 successfully implements a comprehensive family recipes and traditions system. The recipe repository preserves culinary heritage with detailed recipe information, photos, ratings, and comments. The tradition documentation system captures the cultural customs that define the family's identity, with timeline tracking to show how traditions evolve over time.

Together with the wiki system (Phase 6.1) and stories system (Phase 6.2), this completes the Family Knowledge Base initiative, providing the Rushton family with three complementary ways to preserve and share their collective knowledge, memories, and traditions for future generations.
