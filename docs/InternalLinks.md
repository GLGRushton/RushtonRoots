# Internal Links Analysis - RushtonRoots

**Generated:** 2025-12-17  
**Purpose:** Document all internal links currently used in the application and their implementation status

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [MVC Routes (Razor Views)](#mvc-routes-razor-views)
3. [Angular Navigation URLs](#angular-navigation-urls)
4. [API Endpoints](#api-endpoints)
5. [Existing Controllers](#existing-controllers)
6. [Missing Implementations](#missing-implementations)
7. [Deprecated/Obsolete Links](#deprecatedobsolete-links)
8. [Recommendations](#recommendations)

---

## Executive Summary

This document analyzes all internal links and navigation used throughout the RushtonRoots application, including:
- **Razor View Links** (asp-action, asp-controller, href, @Url.Action)
- **Angular Navigation** (window.location.href, url properties)
- **API Endpoints** (existing and referenced)
- **Controller Actions** (MVC and API)

### Key Findings:
- **27 API Controllers** properly configured with [ApiController] attribute
- **11 MVC Controllers** for server-side rendering
- **Multiple missing endpoints** identified for Calendar, MediaGallery, Help, Admin
- **API endpoint gaps** for Person, Partnership, ParentChild CRUD operations
- **Inconsistent routing patterns** mixing MVC and API approaches

---

## MVC Routes (Razor Views)

### ASP.NET Tag Helper Routes

#### Account Controller
- `asp-action="Login"` - ✅ Implemented
- `asp-action="ForgotPassword"` - ✅ Implemented  
- `asp-action="ResetPassword"` - ✅ Implemented
- `asp-action="Profile"` - ✅ Implemented
- `asp-controller="Account"` - ✅ Implemented

#### Person Controller
- `asp-action="Index"` - ✅ Implemented
- `asp-action="Create"` - ✅ Implemented
- `asp-action="Edit"` - ✅ Implemented
- `asp-action="Details"` - ✅ Implemented
- `asp-action="Delete"` - ✅ Implemented
- `asp-controller="Person"` - ✅ Implemented

#### Household Controller
- `asp-action="Index"` - ✅ Implemented
- `asp-action="Create"` - ✅ Implemented
- `asp-action="Edit"` - ✅ Implemented
- `asp-action="Details"` - ✅ Implemented
- `asp-action="Delete"` - ✅ Implemented
- `asp-action="Members"` - ✅ Implemented

#### Partnership Controller
- `asp-action="Index"` - ✅ Implemented
- `asp-action="Create"` - ✅ Implemented
- `asp-action="Edit"` - ✅ Implemented
- `asp-action="Details"` - ✅ Implemented
- `asp-action="Delete"` - ✅ Implemented

#### ParentChild Controller
- `asp-action="Index"` - ✅ Implemented
- `asp-action="Create"` - ✅ Implemented
- `asp-action="Edit"` - ✅ Implemented
- `asp-action="Details"` - ✅ Implemented
- `asp-action="Delete"` - ✅ Implemented

#### Home Controller
- `asp-controller="Home"` - ✅ Implemented
- `asp-action="Index"` - ✅ Implemented

### Direct href Links in Razor Views
- `/Person` - ✅ Implemented
- `/Household` - ✅ Implemented
- `/StoryView` - ✅ Implemented
- `/MediaGallery` - ❌ **MISSING CONTROLLER**

### @Url.Action Calls
- `@Url.Action("Create", "Household")` - ✅ Implemented
- `@Url.Action("Edit", "Person")` - ✅ Implemented
- `@Url.Action("Delete", "Person")` - ✅ Implemented
- `@Url.Action("Details", "Person")` - ✅ Implemented
- `@Url.Action("UpdateField", "Person")` - ❌ **MISSING ACTION**
- `@Url.Action("UpdateNotes", "ParentChild")` - ❌ **MISSING ACTION**
- `@Url.Action("UpdateSettings", "Household")` - ❌ **MISSING ACTION**
- All Partnership and ParentChild CRUD operations - ✅ Implemented

---

## Angular Navigation URLs

### Implemented Routes
- `/` - ✅ Home page
- `/Account/Profile` - ✅ User profile
- `/Account/CreateUser` - ✅ Create user (Admin)
- `/Account/Logout` - ✅ Logout
- `/Account/AccessDenied` - ✅ Access denied
- `/Home/StyleGuide` - ✅ Style guide
- `/Person` - ✅ Person list
- `/Person/Create` - ✅ Create person
- `/Person/Index` - ✅ Person index (same as /Person)
- `/Person?search=true` - ✅ Person search
- `/Household` - ✅ Household list
- `/Household/Create` - ✅ Create household
- `/Partnership` - ✅ Partnership list
- `/Partnership/Create` - ✅ Create partnership
- `/ParentChild` - ✅ Parent-child relationships
- `/Recipe` - ✅ Recipe list (API controller exists)
- `/Recipe/Index` - ⚠️ **INCONSISTENT** (Should use API)
- `/StoryView` - ✅ Story view
- `/StoryView/Index` - ✅ Story index
- `/Tradition` - ✅ Tradition list (API controller exists)
- `/Tradition/Index` - ⚠️ **INCONSISTENT** (Should use API)
- `/Wiki` - ✅ Wiki pages
- `/Wiki/Index` - ✅ Wiki index

### Missing Routes (Referenced but Not Implemented)
- `/Account/Notifications` - ❌ **MISSING ACTION** (NotificationController is API only)
- `/Account/Settings` - ❌ **MISSING ACTION**
- `/Admin/Settings` - ❌ **MISSING CONTROLLER**
- `/Calendar` - ❌ **MISSING CONTROLLER** (FamilyEventController is API only)
- `/Calendar/Create` - ❌ **MISSING CONTROLLER**
- `/MediaGallery` - ❌ **MISSING CONTROLLER** (MediaController is API only)
- `/MediaGallery/Upload` - ❌ **MISSING CONTROLLER**
- `/MediaGallery?type=video` - ❌ **MISSING CONTROLLER**
- `/FamilyTree` - ❌ **MISSING CONTROLLER** (FamilyTreeController is API only)

### Help Pages (All Missing)
- `/Help/Account` - ❌ **MISSING CONTROLLER**
- `/Help/Calendar` - ❌ **MISSING CONTROLLER**
- `/Help/GettingStarted` - ❌ **MISSING CONTROLLER**
- `/Help/HouseholdManagement` - ❌ **MISSING CONTROLLER**
- `/Help/PersonManagement` - ❌ **MISSING CONTROLLER**
- `/Help/Recipes` - ❌ **MISSING CONTROLLER**
- `/Help/RelationshipManagement` - ❌ **MISSING CONTROLLER**
- `/Help/Stories` - ❌ **MISSING CONTROLLER**
- `/Help/Traditions` - ❌ **MISSING CONTROLLER**
- `/Help/Wiki` - ❌ **MISSING CONTROLLER**

### Static/Info Pages (All Missing)
- `/about` - ❌ **MISSING CONTROLLER**
- `/contact` - ❌ **MISSING CONTROLLER**
- `/help` - ❌ **MISSING CONTROLLER**
- `/mission` - ❌ **MISSING CONTROLLER**
- `/privacy` - ❌ **MISSING CONTROLLER**
- `/terms` - ❌ **MISSING CONTROLLER**
- `/story` - ❌ **MISSING CONTROLLER**

---

## API Endpoints

### Existing API Controllers (✅ Fully Implemented)

#### ActivityFeedController (`/api/ActivityFeed`)
- `GET /api/ActivityFeed/recent` - Get recent activities
- `GET /api/ActivityFeed/public` - Get public activities
- `GET /api/ActivityFeed/user/{userId}` - Get user activities
- `GET /api/ActivityFeed/my-activities` - Get current user activities

#### ChatRoomController (`/api/ChatRoom`)
- `GET /api/ChatRoom/{id}` - Get chat room details
- `GET /api/ChatRoom/my-chatrooms` - Get user's chat rooms
- `GET /api/ChatRoom/household/{householdId}` - Get household chat rooms
- `POST /api/ChatRoom` - Create chat room
- `PUT /api/ChatRoom/{id}` - Update chat room
- `DELETE /api/ChatRoom/{id}` - Delete chat room
- `POST /api/ChatRoom/{id}/members` - Add member
- `DELETE /api/ChatRoom/{id}/members/{userId}` - Remove member
- `POST /api/ChatRoom/{id}/read` - Mark as read

#### CommentController (`/api/Comment`)
- `GET /api/Comment/{id}` - Get comment
- `GET /api/Comment/entity/{entityType}/{entityId}` - Get entity comments
- `GET /api/Comment/user/{userId}` - Get user comments
- `GET /api/Comment/my-comments` - Get current user comments
- `POST /api/Comment` - Create comment
- `PUT /api/Comment/{id}` - Update comment
- `DELETE /api/Comment/{id}` - Delete comment

#### ConflictResolutionController (`/api/ConflictResolution`)
- `GET /api/ConflictResolution/{id}` - Get conflict
- `GET /api/ConflictResolution/open` - Get open conflicts
- `POST /api/ConflictResolution/resolve` - Resolve conflict

#### ContributionController (`/api/Contribution`)
- `GET /api/Contribution/{id}` - Get contribution
- `GET /api/Contribution/status/{status}` - Get by status
- `GET /api/Contribution/pending` - Get pending
- `GET /api/Contribution/my-contributions` - Get user contributions
- `POST /api/Contribution` - Create contribution
- `POST /api/Contribution/review` - Review contribution
- `POST /api/Contribution/apply/{id}` - Apply contribution

#### DocumentController (`/api/Document`)
- `GET /api/Document/{id}` - Get document
- `GET /api/Document` - List documents
- `GET /api/Document/user/{userId}` - Get user documents
- `GET /api/Document/my-documents` - Get current user documents
- `GET /api/Document/category/{category}` - Get by category
- `GET /api/Document/person/{personId}` - Get person documents
- `POST /api/Document/search` - Search documents
- `POST /api/Document/upload` - Upload document
- `PUT /api/Document/{id}` - Update document
- `DELETE /api/Document/{id}` - Delete document
- `POST /api/Document/{id}/version` - Add version
- `GET /api/Document/{id}/versions` - Get versions
- `GET /api/Document/{id}/preview` - Get preview

#### EventRsvpController (`/api/EventRsvp`)
- `GET /api/EventRsvp/{id}` - Get RSVP
- `GET /api/EventRsvp/event/{eventId}` - Get event RSVPs
- `GET /api/EventRsvp/user/{userId}` - Get user RSVPs
- `GET /api/EventRsvp/my-rsvps` - Get current user RSVPs
- `POST /api/EventRsvp` - Create RSVP
- `PUT /api/EventRsvp/{id}` - Update RSVP
- `DELETE /api/EventRsvp/{id}` - Delete RSVP

#### FamilyEventController (`/api/FamilyEvent`)
- `GET /api/FamilyEvent/{id}` - Get event
- `GET /api/FamilyEvent` - List events
- `GET /api/FamilyEvent/household/{householdId}` - Get household events
- `GET /api/FamilyEvent/daterange` - Get events in date range
- `GET /api/FamilyEvent/upcoming` - Get upcoming events
- `POST /api/FamilyEvent` - Create event
- `PUT /api/FamilyEvent/{id}` - Update event
- `DELETE /api/FamilyEvent/{id}` - Delete event

#### FamilyTaskController (`/api/FamilyTask`)
- `GET /api/FamilyTask/{id}` - Get task
- `GET /api/FamilyTask` - List tasks
- `GET /api/FamilyTask/household/{householdId}` - Get household tasks
- `GET /api/FamilyTask/assigned-to-me` - Get assigned tasks
- `GET /api/FamilyTask/status/{status}` - Get by status
- `GET /api/FamilyTask/event/{eventId}` - Get event tasks
- `POST /api/FamilyTask` - Create task
- `PUT /api/FamilyTask/{id}` - Update task
- `DELETE /api/FamilyTask/{id}` - Delete task

#### FamilyTreeController (`/api/FamilyTree`)
- `GET /api/FamilyTree/pedigree/{personId}` - Get pedigree
- `GET /api/FamilyTree/descendants/{personId}` - Get descendants
- `GET /api/FamilyTree/all` - Get full tree

#### LeaderboardController (`/api/Leaderboard`)
- `GET /api/Leaderboard` - Get leaderboard
- `GET /api/Leaderboard/user/{userId}` - Get user score
- `GET /api/Leaderboard/my-score` - Get current user score

#### LifeEventController (`/api/LifeEvent`)
- `GET /api/LifeEvent/{id}` - Get life event
- `GET /api/LifeEvent/person/{personId}` - Get person events
- `GET /api/LifeEvent/person/{personId}/timeline` - Get timeline
- `POST /api/LifeEvent` - Create event
- `PUT /api/LifeEvent/{id}` - Update event
- `DELETE /api/LifeEvent/{id}` - Delete event

#### LocationController (`/api/Location`)
- `GET /api/Location/{id}` - Get location
- `GET /api/Location` - List locations
- `GET /api/Location/search` - Search locations
- `POST /api/Location` - Create location
- `DELETE /api/Location/{id}` - Delete location

#### MediaController (`/api/Media`)
- `GET /api/Media/{id}` - Get media
- `GET /api/Media` - List media
- `GET /api/Media/user/{userId}` - Get user media
- `GET /api/Media/my-media` - Get current user media
- `GET /api/Media/person/{personId}` - Get person media
- `POST /api/Media/search` - Search media
- `POST /api/Media/upload` - Upload media
- `PUT /api/Media/{id}` - Update media
- `DELETE /api/Media/{id}` - Delete media
- `GET /api/Media/{id}/stream` - Stream media
- `POST /api/Media/{id}/markers` - Add marker
- `GET /api/Media/{id}/markers` - Get markers
- `DELETE /api/Media/markers/{markerId}` - Delete marker

#### MessageController (`/api/Message`)
- `GET /api/Message/{id}` - Get message
- `GET /api/Message/direct/{otherUserId}` - Get direct messages
- `GET /api/Message/chatroom/{chatRoomId}` - Get chat room messages
- `POST /api/Message` - Send message
- `PUT /api/Message/{id}` - Update message
- `DELETE /api/Message/{id}` - Delete message
- `POST /api/Message/{id}/read` - Mark as read

#### NotificationController (`/api/Notification`)
- Full CRUD operations for notifications

#### PhotoAlbumController (`/api/PhotoAlbum`)
- Full CRUD operations for photo albums

#### PhotoController (`/api/Photo`)
- Full CRUD operations for photos

#### PhotoTagController (`/api/PhotoTag`)
- Full CRUD operations for photo tags

#### RecipeController (`/api/Recipe`)
- `GET /api/Recipe` - List recipes
- `GET /api/Recipe/{id}` - Get recipe
- `GET /api/Recipe/slug/{slug}` - Get by slug
- `POST /api/Recipe/search` - Search recipes
- `GET /api/Recipe/category/{category}` - Get by category
- `GET /api/Recipe/cuisine/{cuisine}` - Get by cuisine
- `GET /api/Recipe/favorites` - Get favorites
- `GET /api/Recipe/recent` - Get recent
- `GET /api/Recipe/person/{personId}` - Get person recipes
- `GET /api/Recipe/categories` - List categories
- `GET /api/Recipe/cuisines` - List cuisines
- `POST /api/Recipe` - Create recipe
- `PUT /api/Recipe/{id}` - Update recipe
- `DELETE /api/Recipe/{id}` - Delete recipe

#### SearchApiController (`/api/SearchApi`)
- `GET /api/SearchApi/person` - Search persons
- `GET /api/SearchApi/relationship` - Find relationships
- `GET /api/SearchApi/surname-distribution` - Get surname stats
- `GET /api/SearchApi/by-location/{locationId}` - Get by location
- `GET /api/SearchApi/by-event-type` - Get by event type

#### StoryController (`/api/Story`)
- `GET /api/Story` - List stories
- `GET /api/Story/{id}` - Get story
- `GET /api/Story/slug/{slug}` - Get by slug
- `POST /api/Story/search` - Search stories
- Full CRUD operations

#### StoryCollectionController (`/api/StoryCollection`)
- Full CRUD operations for story collections

#### TraditionController (`/api/Tradition`)
- Full CRUD operations for traditions
- Similar structure to RecipeController

#### WikiPageController (`/api/WikiPage`)
- Full CRUD operations for wiki pages

### Missing API Endpoints (Referenced but Not Implemented)

#### Person API (`/api/person` or `/api/Person`)
- ❌ `POST /api/person` - **MISSING** (referenced in person-form.component.ts)
- ❌ `PUT /api/person/{id}` - **MISSING** (referenced in person-form.component.ts)
- ❌ `GET /api/person` - **MISSING** (should list all persons)
- ❌ `GET /api/person/{id}` - **MISSING** (should get person details)
- ❌ `DELETE /api/person/{id}` - **MISSING** (should delete person)

**Note:** PersonController exists but is MVC-only, not API controller

#### Partnership API (`/api/partnership` or `/api/Partnership`)
- ❌ All CRUD operations missing
- PartnershipController exists but is MVC-only

#### ParentChild API (`/api/parentchild` or `/api/ParentChild`)
- ❌ All CRUD operations missing
- ParentChildController exists but is MVC-only

#### Household API (`/api/household` or `/api/Household`)
- ❌ API endpoints missing
- HouseholdController exists but is MVC-only

---

## Existing Controllers

### MVC Controllers (Server-Side Rendering)
1. **AccountController** - Authentication and user management
2. **HomeController** - Home page and style guide
3. **HouseholdController** - Household CRUD operations
4. **ParentChildController** - Parent-child relationship CRUD
5. **PartnershipController** - Partnership CRUD
6. **PersonController** - Person CRUD operations
7. **RecipeViewController** - Recipe view pages
8. **StoryViewController** - Story view pages
9. **TraditionViewController** - Tradition view pages
10. **WikiController** - Wiki view pages

### API Controllers (RESTful APIs)
All controllers listed in [API Endpoints](#api-endpoints) section above (27 total)

---

## Missing Implementations

### Critical Missing Endpoints (High Priority)

#### 1. Person API Endpoints
**Impact:** HIGH - Angular form expects these endpoints  
**Current State:** MVC controller only  
**Required:**
- `POST /api/person` - Create person
- `PUT /api/person/{id}` - Update person
- `GET /api/person` - List persons
- `GET /api/person/{id}` - Get person details
- `DELETE /api/person/{id}` - Delete person

#### 2. MediaGallery Controller/View
**Impact:** HIGH - Direct link from home page  
**Current State:** API controller exists (MediaController), no MVC view  
**Required:**
- MVC Controller: `MediaGalleryController`
- Actions: `Index`, `Upload`
- Views: Index view for photo gallery
- Integration with MediaController API

#### 3. Calendar/FamilyEvent Views
**Impact:** MEDIUM - Referenced in navigation  
**Current State:** API controller exists (FamilyEventController), no MVC views  
**Required:**
- MVC Controller: `CalendarController`
- Actions: `Index`, `Create`
- Views: Calendar view, event creation
- Integration with FamilyEventController API

#### 4. FamilyTree View
**Impact:** MEDIUM - Referenced in navigation and home page  
**Current State:** API controller exists (FamilyTreeController), no MVC view  
**Required:**
- MVC Controller: `FamilyTreeController` (or add to existing API controller)
- Action: `Index`
- View: Family tree visualization

### Medium Priority Missing Endpoints

#### 5. Account Additional Actions
**Required:**
- `GET /Account/Notifications` - Notifications page
- `GET /Account/Settings` - User settings page

#### 6. Admin Controller
**Required:**
- `AdminController` with `Settings` action
- Admin dashboard views

#### 7. Help Controller
**Required:**
- `HelpController` with actions for each help topic
- Help documentation views

### Low Priority Missing Endpoints

#### 8. Static/Info Pages
**Required:**
- Controllers/Views for: About, Contact, Help, Mission, Privacy, Terms, Story

---

## Deprecated/Obsolete Links

### Links That Should Be Removed/Replaced

#### 1. MVC POST Actions for API-First Entities
**Issue:** These entities have API controllers but still use MVC POST patterns  
**Deprecated:**
- `POST: Person/Create` → Should use `POST /api/person`
- `POST: Person/Edit` → Should use `PUT /api/person/{id}`
- `POST: Person/Delete` → Should use `DELETE /api/person/{id}`
- `POST: Partnership/Create` → Should use `POST /api/partnership`
- `POST: Partnership/Edit` → Should use `PUT /api/partnership/{id}`
- `POST: Partnership/Delete` → Should use `DELETE /api/partnership/{id}`
- `POST: ParentChild/Create` → Should use `POST /api/parentchild`
- `POST: ParentChild/Edit` → Should use `PUT /api/parentchild/{id}`
- `POST: ParentChild/Delete` → Should use `DELETE /api/parentchild/{id}`

**Recommendation:** Migrate these to use API endpoints with Angular forms

#### 2. Redundant Index Routes
**Issue:** Both `/Entity` and `/Entity/Index` point to same action  
**Deprecated:**
- `/Person/Index` (use `/Person`)
- `/Recipe/Index` (use `/Recipe`)
- `/Tradition/Index` (use `/Tradition`)
- `/StoryView/Index` (use `/StoryView`)
- `/Wiki/Index` (use `/Wiki`)

**Recommendation:** Standardize on root entity path

#### 3. Mixed MVC/API Pattern for Same Entity
**Issue:** Recipe, Tradition, Story, Wiki have both MVC and API controllers  
**Current:**
- `RecipeController` (API) + `RecipeViewController` (MVC)
- `TraditionController` (API) + `TraditionViewController` (MVC)
- `StoryController` (API) + `StoryViewController` (MVC)
- `WikiPageController` (API) + `WikiController` (MVC)

**Recommendation:** Keep this pattern as it separates concerns (API for data, View for rendering)

---

## Recommendations

### 1. Organize API Controllers
**Action:** Move all API controllers to `Controllers/Api/` directory  
**Benefits:**
- Clear separation of MVC and API controllers
- Easier to maintain and navigate
- Follows ASP.NET Core best practices

**Structure:**
```
Controllers/
├── Api/
│   ├── ActivityFeedController.cs
│   ├── ChatRoomController.cs
│   ├── PersonController.cs
│   ├── PartnershipController.cs
│   ├── ParentChildController.cs
│   ├── HouseholdController.cs
│   └── ... (all other API controllers)
├── AccountController.cs
├── HomeController.cs
├── MediaGalleryController.cs (new)
├── CalendarController.cs (new)
├── FamilyTreeController.cs (new)
└── ... (other MVC controllers)
```

### 2. Create Missing API Controllers
**Priority Order:**
1. **Person API** - Highest priority (Angular form expects it)
2. **Partnership API** - High priority (relationship management)
3. **ParentChild API** - High priority (relationship management)
4. **Household API** - Medium priority

### 3. Create Missing MVC Controllers/Views
**Priority Order:**
1. **MediaGalleryController** - High (linked from home)
2. **CalendarController** - High (navigation menu)
3. **FamilyTreeController** - Medium (linked from home)
4. **HelpController** - Low (documentation)
5. **Static page controllers** - Low (info pages)

### 4. Standardize Routing Patterns
**Guidelines:**
- Use `/api/{entity}` for all API endpoints
- Use `/{Entity}` for MVC views (e.g., `/Person`, `/Household`)
- Avoid `/Entity/Index` in favor of `/Entity`
- Use RESTful verbs for API (GET, POST, PUT, DELETE)
- Use MVC actions for server-rendered forms (Create, Edit, Delete views)

### 5. Migration Strategy
**Approach:**
1. Create API controllers for Person, Partnership, ParentChild, Household
2. Update Angular components to use new API endpoints
3. Keep MVC controllers for initial page loads and non-SPA features
4. Gradually migrate forms to use API + Angular instead of MVC POST
5. Deprecate MVC POST actions once API is stable

### 6. Testing Strategy
**Required:**
- Unit tests for all new API controllers
- Integration tests for API endpoints
- Manual testing of Angular forms with new APIs
- Ensure MVC views still work during transition

---

## Next Steps

See [ApiEndpointsImplementationPlan.md](./ApiEndpointsImplementationPlan.md) for the phased implementation plan.

---

**Document Status:** Complete  
**Last Updated:** 2025-12-17  
**Review Required:** Before starting Phase 1 implementation
