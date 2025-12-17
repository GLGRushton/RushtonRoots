# API Migration Plan - RushtonRoots

## Document Overview

**Purpose**: This document outlines a phased plan to migrate old MVC-style CRUD endpoints to modern REST API endpoints (`/api/[controller]` pattern) and remove legacy endpoints.

**Last Updated**: December 2025  
**Status**: Planning Phase  
**Target Completion**: Q1 2026

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Current State Analysis](#current-state-analysis)
3. [Migration Goals](#migration-goals)
4. [Migration Phases](#migration-phases)
5. [Testing Strategy](#testing-strategy)
6. [Rollback Plan](#rollback-plan)
7. [Success Metrics](#success-metrics)

---

## Executive Summary

RushtonRoots has recently migrated to Angular components and modern API patterns using the `/api/[controller]` endpoint structure. However, several legacy MVC controllers still exist that use the old pattern (e.g., `POST /Person/Delete/5` instead of `DELETE /api/person/5`).

### Current Status

- **✅ Already Migrated**: 28 API controllers following REST conventions
- **⚠️ Needs Migration**: 4 MVC controllers (Person, Household, Partnership, ParentChild)
- **✅ View Controllers**: 5 controllers remain as view-only (Account, Recipe, Story, Tradition, Wiki view controllers)

### Migration Impact

This migration will:
- Improve API consistency and discoverability
- Enable better API documentation (Swagger/OpenAPI)
- Simplify frontend development with predictable REST patterns
- Reduce technical debt and confusion between old/new patterns
- Enable API versioning in the future

---

## Current State Analysis

### Controllers Already Using API Pattern ✅

The following controllers already follow the `/api/[controller]` REST pattern and require **no changes**:

| Controller | Route | CRUD Operations |
|------------|-------|-----------------|
| ActivityFeedController | `/api/activityfeed` | GET only (read feeds) |
| ChatRoomController | `/api/chatroom` | Full CRUD + members |
| CommentController | `/api/comment` | Full CRUD |
| ConflictResolutionController | `/api/conflictresolution` | GET, POST (resolve) |
| ContributionController | `/api/contribution` | GET, POST (review, apply) |
| DocumentController | `/api/document` | Full CRUD + upload |
| EventRsvpController | `/api/eventrsvp` | Full CRUD |
| FamilyEventController | `/api/familyevent` | Full CRUD |
| FamilyTaskController | `/api/familytask` | Full CRUD |
| FamilyTreeController | `/api/familytree` | GET only (pedigree, descendants) |
| LeaderboardController | `/api/leaderboard` | GET only |
| LifeEventController | `/api/lifeevent` | GET, POST, PUT, DELETE |
| LocationController | `/api/location` | GET, POST, DELETE + search |
| MediaController | `/api/media` | Full CRUD + upload + streaming |
| MessageController | `/api/message` | Full CRUD + read status |
| NotificationController | `/api/notification` | GET, POST, DELETE + preferences |
| PhotoAlbumController | `/api/photoalbum` | Full CRUD |
| PhotoController | `/api/photo` | Full CRUD + upload + timeline |
| PhotoTagController | `/api/phototag` | GET, POST, DELETE |
| RecipeController | `/api/recipe` | Full CRUD + search + categories |
| SearchApiController | `/api/searchapi` | GET only (advanced search) |
| StoryCollectionController | `/api/storycollection` | Full CRUD |
| StoryController | `/api/story` | Full CRUD + search |
| TraditionController | `/api/tradition` | Full CRUD + search |
| WikiCategoryController | `/api/wikicategory` | Full CRUD |
| WikiTagController | `/api/wikitag` | GET, POST, DELETE |
| WikiPageController | `/api/wikipage` | Full CRUD + versioning |
| TraditionTimelineController | `/api/traditiontimeline` | GET only |

### Controllers Requiring Migration ⚠️

The following controllers use the old MVC pattern and need to be migrated:

| Controller | Current Pattern | Target Pattern | Razor Views | Angular Components |
|------------|----------------|----------------|-------------|-------------------|
| PersonController | `/Person/*` | `/api/person` | 5 views (Index, Details, Create, Edit, Delete) | person-form, person-index, person-details, person-table |
| HouseholdController | `/Household/*` | `/api/household` | 6 views (Index, Details, Members, Create, Edit, Delete) | household-index, household-details, household-form |
| PartnershipController | `/Partnership/*` | `/api/partnership` | 5 views (Index, Details, Create, Edit, Delete) | partnership-index, partnership-details, partnership-form |
| ParentChildController | `/ParentChild/*` | `/api/parentchild` | 5 views (Index, Details, Create, Edit, Delete) | parent-child-index, parent-child-details, parent-child-form |

### Controllers Remaining as View Controllers ✅

These controllers serve Razor views and should **NOT** be migrated (they don't expose CRUD APIs):

| Controller | Purpose |
|------------|---------|
| AccountController | Authentication UI (Login, Register, Profile) |
| HomeController | Landing page |
| RecipeViewController | Recipe view pages |
| StoryViewController | Story view pages |
| TraditionViewController | Tradition view pages |
| WikiController | Wiki view pages |

### Angular Component Endpoint Usage

#### Person Module
- **person-form.component.ts**:
  - ✅ Already uses `/api/person` (POST, PUT) - Lines 291, 317
  - ⚠️ Redirects to `/Person/Details/{id}` (MVC) - Lines 309, 342, 366
  - ⚠️ Redirects to `/Person/Index` (MVC) - Line 368

- **person-index.component.ts**:
  - ⚠️ Links to `/Person/Delete/{id}` - Line 108
  - ⚠️ Links to `/Person/Create` - Line 121

- **person-table.component.ts**:
  - ⚠️ Links to `/Person/Details/{id}` - Line 96
  - ⚠️ Links to `/Person/Edit/{id}` - Line 103

- **person-details.component.ts**:
  - ⚠️ Uses `window.location.href` (URL reading) - Line 161

#### Household Module
- **household-index.component.ts**:
  - ⚠️ Links to `/Household/Details/{id}` - Line 189
  - ⚠️ Links to `/Household/Edit/{id}` - Line 197
  - ⚠️ Links to `/Household/Delete/{id}` - Line 210
  - ⚠️ Links to `/Household/Members/{id}` - Line 219
  - ⚠️ Links to `/Household/Create` - Line 234

#### Partnership Module
- **partnership-index.component.ts**:
  - ⚠️ Links to `/Partnership/Details/{id}` - Line 262
  - ⚠️ Links to `/Partnership/Edit/{id}` - Line 271
  - ⚠️ Links to `/Partnership/Delete/{id}` - Line 281
  - ⚠️ Links to `/Partnership/Create` - Line 298

#### Navigation & Shared Components
- **navigation.component.ts**:
  - ⚠️ Menu items: `/Person`, `/Person/Create`, `/Household`, `/Household/Create`, `/Partnership`, `/ParentChild`, `/Partnership/Create`

- **breadcrumb.service.ts**:
  - ⚠️ Breadcrumb links: `/Person`, `/Household`

- **Other components** (home-page, keyboard-navigation.service, etc.):
  - Multiple references to `/Person/*`, `/Household/*`, etc.

---

## Migration Goals

### Primary Objectives

1. **Consistency**: All CRUD APIs follow the same `/api/[controller]` REST pattern
2. **RESTful Design**: Use proper HTTP verbs (GET, POST, PUT, DELETE)
3. **Backward Compatibility**: Maintain functionality during migration with minimal disruption
4. **Angular Routing**: Migrate from server-side redirects to Angular routing where appropriate
5. **API Documentation**: Enable Swagger/OpenAPI auto-documentation
6. **Clean Removal**: Remove old MVC endpoints after migration is complete

### Success Criteria

- ✅ All CRUD operations use `/api/[controller]` endpoints
- ✅ All Angular components updated to use new API endpoints
- ✅ Zero broken links or 404 errors
- ✅ All automated tests pass
- ✅ Swagger documentation auto-generates for all API endpoints
- ✅ Old MVC endpoints deprecated and removed
- ✅ Server-side views removed (except for Account, Home, and *View controllers)

---

## Migration Phases

### Phase 1: Person API Migration

Migrate the Person controller from MVC pattern to API pattern.

#### Phase 1.1: Create PersonApiController

**Objective**: Create new API controller alongside existing MVC controller.

**Tasks**:
1. Create `PersonApiController.cs` in `/Controllers`
   - Route: `[Route("api/[controller]")]`
   - Inherit from `ControllerBase` (not `Controller`)
   - Implement RESTful endpoints:
     - `GET /api/person` - Get all people (with optional search)
     - `GET /api/person/{id}` - Get person by ID
     - `POST /api/person` - Create person (with optional photo upload)
     - `PUT /api/person/{id}` - Update person (with optional photo upload)
     - `DELETE /api/person/{id}` - Delete person
     - `GET /api/person/search` - Search people (query params)
   
2. Inject `IPersonService` and `IHouseholdService`

3. Return JSON responses (not views):
   - Success: `Ok(viewModel)`
   - Not Found: `NotFound()`
   - Created: `CreatedAtAction()`
   - Validation errors: `BadRequest(errors)`

4. Support multipart form data for photo uploads

5. Add XML comments for Swagger documentation

6. Handle authorization with `[Authorize]` attributes

**Files to Create**:
- `RushtonRoots.Web/Controllers/PersonApiController.cs`

**Testing**:
- Unit tests for each endpoint
- Integration tests with test database
- Manual testing with Postman/Swagger

**Success Criteria**:
- API endpoints return correct JSON responses
- Swagger documentation auto-generates
- Authorization works correctly
- Photo upload works for POST and PUT

**Estimated Effort**: 4-6 hours

---

#### Phase 1.2: Update Angular Components for Person

**Objective**: Update all Angular components to use new Person API endpoints.

**Tasks**:

1. **Create PersonService** (if not exists):
   - `src/app/person/services/person.service.ts`
   - Implement methods:
     - `getAll()` → `GET /api/person`
     - `getById(id)` → `GET /api/person/{id}`
     - `search(criteria)` → `GET /api/person/search`
     - `create(person)` → `POST /api/person`
     - `update(id, person)` → `PUT /api/person/{id}`
     - `delete(id)` → `DELETE /api/person/{id}`
   - Use `HttpClient` for all requests
   - Return Observables

2. **Update person-form.component.ts**:
   - ✅ Already uses `/api/person` for POST/PUT (no changes needed)
   - ❌ Change redirects from `/Person/Details/{id}` to Angular routing
   - ❌ Change redirects from `/Person/Index` to Angular routing
   - Option 1: Navigate within Angular SPA: `this.router.navigate(['/person', id])`
   - Option 2: Keep server redirects but use API routes (if server-side rendering is required)

3. **Update person-index.component.ts**:
   - Change `/Person/Delete/{id}` → Use delete dialog or `DELETE /api/person/{id}`
   - Change `/Person/Create` → Angular route `/person/create`

4. **Update person-table.component.ts**:
   - Change `/Person/Details/{id}` → Angular route `/person/{id}`
   - Change `/Person/Edit/{id}` → Angular route `/person/edit/{id}`

5. **Update person-details.component.ts**:
   - Ensure it fetches from `GET /api/person/{id}`
   - Load data via PersonService

6. **Update navigation.component.ts**:
   - Update menu items to Angular routes:
     - `/Person` → `/person` (or keep as is if server-side rendering is used)
     - `/Person/Create` → `/person/create`

7. **Update breadcrumb.service.ts**:
   - Update breadcrumb URLs to match Angular routes

8. **Update other components**:
   - keyboard-navigation.service.ts
   - home-page.component.ts
   - style-guide.component.ts
   - not-found.component.ts

**Decision Point**: Angular SPA Routing vs. Server-Side Rendering

**Option A**: Full Angular SPA (Recommended)
- Pros: Modern SPA experience, faster navigation, better UX
- Cons: Requires Angular routing setup, more client-side code
- Impact: Medium (need to set up routes in app-routing.module.ts)

**Option B**: Hybrid (Server-side views + API)
- Pros: Minimal changes, uses existing Razor views
- Cons: Page reloads, less modern UX, harder to maintain two systems
- Impact: Low (keep Razor views, just update API calls)

**Recommendation**: Go with Option A (Full Angular SPA) for Person module as Angular components already exist.

**Files to Modify**:
- `src/app/person/person.module.ts` (if service needs to be added)
- `src/app/person/components/person-form/person-form.component.ts`
- `src/app/person/components/person-index/person-index.component.ts`
- `src/app/person/components/person-table/person-table.component.ts`
- `src/app/person/components/person-details/person-details.component.ts`
- `src/app/shared/components/navigation/navigation.component.ts`
- `src/app/shared/services/breadcrumb.service.ts`
- `src/app/accessibility/services/keyboard-navigation.service.ts`
- `src/app/home/components/home-page/home-page.component.ts`
- `src/app/style-guide/style-guide.component.ts`
- `src/app/shared/components/not-found/not-found.component.ts`

**Files to Create**:
- `src/app/person/services/person.service.ts` (if not exists)

**Testing**:
- Test all person operations (create, read, update, delete)
- Test navigation between person pages
- Test search functionality
- Test photo upload
- Verify breadcrumbs update correctly
- Test keyboard navigation shortcuts

**Success Criteria**:
- All person components use PersonService
- All API calls use `/api/person` endpoints
- Navigation works within Angular SPA
- No console errors
- All features work as before

**Estimated Effort**: 6-8 hours

---

#### Phase 1.3: Set Up Angular Routing for Person

**Objective**: Configure Angular routes for Person module to enable SPA navigation.

**Tasks**:

1. **Update app-routing.module.ts**:
   - Add routes for Person module:
     ```typescript
     {
       path: 'person',
       children: [
         { path: '', component: PersonIndexComponent },
         { path: 'create', component: PersonFormComponent },
         { path: ':id', component: PersonDetailsComponent },
         { path: 'edit/:id', component: PersonFormComponent }
       ]
     }
     ```

2. **Update PersonFormComponent** to handle both create and edit:
   - Use `ActivatedRoute` to detect if `:id` param exists
   - If id exists, fetch person and populate form (edit mode)
   - If no id, show empty form (create mode)

3. **Update navigation to use RouterLink**:
   - Replace `window.location.href` with `router.navigate()`
   - Update navigation component links

4. **Test routing**:
   - Direct URL navigation: `/person`, `/person/create`, `/person/123`
   - Deep linking works correctly
   - Browser back/forward buttons work
   - Breadcrumbs update correctly

**Files to Modify**:
- `src/app/app-routing.module.ts`
- `src/app/person/components/person-form/person-form.component.ts`
- `src/app/person/components/person-index/person-index.component.ts`
- `src/app/person/components/person-table/person-table.component.ts`
- `src/app/person/components/person-details/person-details.component.ts`

**Testing**:
- Test all route navigation
- Test deep linking
- Test browser history
- Test route guards (if needed for authorization)

**Success Criteria**:
- All person routes work correctly
- No page reloads on navigation
- URL updates correctly
- Deep links work

**Estimated Effort**: 3-4 hours

---

#### Phase 1.4: Deprecate Old Person MVC Endpoints

**Objective**: Mark old Person MVC endpoints as deprecated and add logging.

**Tasks**:

1. **Update PersonController.cs**:
   - Add `[Obsolete("Use /api/person endpoints instead")]` attribute to all action methods
   - Add logging to track usage of deprecated endpoints
   - Add response header: `X-Deprecated: true; use /api/person`

2. **Add deprecation warnings** to Razor views:
   - Add banner at top: "This page is deprecated. Redirecting to new version..."
   - Optionally auto-redirect to Angular routes after delay

3. **Monitor usage**:
   - Set up logging/analytics to track deprecated endpoint usage
   - Identify any external integrations using old endpoints

4. **Document deprecation**:
   - Update API documentation
   - Notify any external API consumers (if applicable)

**Files to Modify**:
- `RushtonRoots.Web/Controllers/PersonController.cs`
- `RushtonRoots.Web/Views/Person/*.cshtml` (optional deprecation banners)

**Testing**:
- Verify deprecation warnings appear in logs
- Verify response headers include deprecation notice
- Test that old endpoints still work (backward compatibility)

**Success Criteria**:
- Old endpoints marked as deprecated
- Logging captures deprecated usage
- No breaking changes to existing functionality

**Estimated Effort**: 2 hours

---

#### Phase 1.5: Remove Old Person MVC Endpoints and Views

**Objective**: Complete removal of deprecated Person MVC controller and Razor views.

**Prerequisites**:
- Phase 1.1, 1.2, 1.3 completed
- No usage of deprecated endpoints in logs for 2+ weeks
- All Angular components migrated and tested

**Tasks**:

1. **Remove PersonController.cs** (old MVC controller):
   - Delete `RushtonRoots.Web/Controllers/PersonController.cs`

2. **Remove Person Razor Views**:
   - Delete `RushtonRoots.Web/Views/Person/Index.cshtml`
   - Delete `RushtonRoots.Web/Views/Person/Details.cshtml`
   - Delete `RushtonRoots.Web/Views/Person/Create.cshtml`
   - Delete `RushtonRoots.Web/Views/Person/Edit.cshtml`
   - Delete `RushtonRoots.Web/Views/Person/Delete.cshtml`
   - Delete `RushtonRoots.Web/Views/Person/` directory (if empty)

3. **Update Program.cs / Startup** (if needed):
   - Remove any Person-specific MVC route configurations

4. **Run full regression tests**:
   - Verify all person functionality works via API
   - Verify no 404 errors on person pages
   - Test all CRUD operations
   - Test photo uploads
   - Test search

**Files to Delete**:
- `RushtonRoots.Web/Controllers/PersonController.cs`
- `RushtonRoots.Web/Views/Person/Index.cshtml`
- `RushtonRoots.Web/Views/Person/Details.cshtml`
- `RushtonRoots.Web/Views/Person/Create.cshtml`
- `RushtonRoots.Web/Views/Person/Edit.cshtml`
- `RushtonRoots.Web/Views/Person/Delete.cshtml`

**Testing**:
- Full regression test suite
- Manual testing of all person features
- Verify no broken links

**Success Criteria**:
- Old PersonController removed
- All Razor views removed
- All tests pass
- No 404 errors
- Application works correctly

**Estimated Effort**: 2 hours

**Total Phase 1 Effort**: 17-22 hours

---

### Phase 2: Household API Migration

Migrate the Household controller from MVC pattern to API pattern.

#### Phase 2.1: Create HouseholdApiController

**Objective**: Create new API controller alongside existing MVC controller.

**Tasks**:
1. Create `HouseholdApiController.cs` in `/Controllers`
   - Route: `[Route("api/[controller]")]`
   - Inherit from `ControllerBase`
   - Implement RESTful endpoints:
     - `GET /api/household` - Get all households
     - `GET /api/household/{id}` - Get household by ID
     - `GET /api/household/{id}/members` - Get household members
     - `POST /api/household` - Create household
     - `PUT /api/household/{id}` - Update household
     - `DELETE /api/household/{id}` - Delete household
     - `POST /api/household/{id}/members` - Add member to household
     - `DELETE /api/household/{id}/members/{personId}` - Remove member from household
   
2. Inject `IHouseholdService`

3. Return JSON responses

4. Add XML comments for Swagger

5. Handle authorization

**Files to Create**:
- `RushtonRoots.Web/Controllers/HouseholdApiController.cs`

**Testing**:
- Unit tests for each endpoint
- Integration tests
- Manual testing with Postman/Swagger

**Success Criteria**:
- API endpoints return correct JSON
- Swagger documentation auto-generates
- Authorization works
- Member management works

**Estimated Effort**: 4-6 hours

---

#### Phase 2.2: Update Angular Components for Household

**Objective**: Update all Angular components to use new Household API endpoints.

**Tasks**:

1. **Create HouseholdService** (if not exists):
   - `src/app/household/services/household.service.ts`
   - Implement methods:
     - `getAll()` → `GET /api/household`
     - `getById(id)` → `GET /api/household/{id}`
     - `getMembers(id)` → `GET /api/household/{id}/members`
     - `create(household)` → `POST /api/household`
     - `update(id, household)` → `PUT /api/household/{id}`
     - `delete(id)` → `DELETE /api/household/{id}`
     - `addMember(id, personId)` → `POST /api/household/{id}/members`
     - `removeMember(id, personId)` → `DELETE /api/household/{id}/members/{personId}`

2. **Update household-index.component.ts**:
   - Change `/Household/Details/{id}` → `/household/{id}`
   - Change `/Household/Edit/{id}` → `/household/edit/{id}`
   - Change `/Household/Delete/{id}` → Use delete dialog or API
   - Change `/Household/Members/{id}` → `/household/{id}/members`
   - Change `/Household/Create` → `/household/create`

3. **Update household-details.component.ts**:
   - Use HouseholdService to fetch data

4. **Update household-form.component.ts**:
   - Use HouseholdService for create/update

5. **Update household-members.component.ts**:
   - Use HouseholdService for member management

6. **Update navigation and breadcrumbs**:
   - Update menu items to Angular routes

**Files to Modify**:
- Multiple household component files
- Navigation components
- Breadcrumb service

**Files to Create**:
- `src/app/household/services/household.service.ts` (if not exists)

**Testing**:
- Test all household operations
- Test member management
- Test navigation

**Success Criteria**:
- All household components use HouseholdService
- All API calls use `/api/household` endpoints
- Navigation works within Angular SPA

**Estimated Effort**: 6-8 hours

---

#### Phase 2.3: Set Up Angular Routing for Household

**Objective**: Configure Angular routes for Household module.

**Tasks**:

1. **Update app-routing.module.ts**:
   - Add routes for Household module:
     ```typescript
     {
       path: 'household',
       children: [
         { path: '', component: HouseholdIndexComponent },
         { path: 'create', component: HouseholdFormComponent },
         { path: ':id', component: HouseholdDetailsComponent },
         { path: ':id/members', component: HouseholdMembersComponent },
         { path: 'edit/:id', component: HouseholdFormComponent }
       ]
     }
     ```

2. **Update HouseholdFormComponent** to handle create/edit

3. **Update navigation to use RouterLink**

4. **Test routing**

**Files to Modify**:
- `src/app/app-routing.module.ts`
- Household component files

**Testing**:
- Test all route navigation
- Test deep linking

**Success Criteria**:
- All household routes work correctly
- No page reloads

**Estimated Effort**: 3-4 hours

---

#### Phase 2.4: Deprecate Old Household MVC Endpoints

**Objective**: Mark old Household MVC endpoints as deprecated.

**Tasks**:
1. Update HouseholdController.cs with `[Obsolete]` attributes
2. Add logging
3. Add response headers
4. Monitor usage

**Files to Modify**:
- `RushtonRoots.Web/Controllers/HouseholdController.cs`

**Testing**:
- Verify deprecation warnings

**Success Criteria**:
- Old endpoints marked as deprecated
- Logging works

**Estimated Effort**: 2 hours

---

#### Phase 2.5: Remove Old Household MVC Endpoints and Views

**Objective**: Complete removal of deprecated Household MVC controller and views.

**Prerequisites**:
- Phase 2.1, 2.2, 2.3 completed
- No usage of deprecated endpoints for 2+ weeks

**Tasks**:
1. Delete HouseholdController.cs
2. Delete all Household Razor views
3. Run regression tests

**Files to Delete**:
- `RushtonRoots.Web/Controllers/HouseholdController.cs`
- `RushtonRoots.Web/Views/Household/*.cshtml`

**Testing**:
- Full regression tests

**Success Criteria**:
- Old controller and views removed
- All tests pass

**Estimated Effort**: 2 hours

**Total Phase 2 Effort**: 17-22 hours

---

### Phase 3: Partnership API Migration

Migrate the Partnership controller from MVC pattern to API pattern.

#### Phase 3.1: Create PartnershipApiController

**Objective**: Create new API controller alongside existing MVC controller.

**Tasks**:
1. Create `PartnershipApiController.cs` in `/Controllers`
   - Route: `[Route("api/[controller]")]`
   - Inherit from `ControllerBase`
   - Implement RESTful endpoints:
     - `GET /api/partnership` - Get all partnerships
     - `GET /api/partnership/{id}` - Get partnership by ID
     - `GET /api/partnership/person/{personId}` - Get partnerships for a person
     - `POST /api/partnership` - Create partnership
     - `PUT /api/partnership/{id}` - Update partnership
     - `DELETE /api/partnership/{id}` - Delete partnership
   
2. Inject `IPartnershipService`

3. Return JSON responses

4. Add XML comments for Swagger

5. Handle authorization

**Files to Create**:
- `RushtonRoots.Web/Controllers/PartnershipApiController.cs`

**Testing**:
- Unit tests
- Integration tests
- Manual testing

**Success Criteria**:
- API endpoints work correctly
- Swagger documentation generates

**Estimated Effort**: 4-6 hours

---

#### Phase 3.2: Update Angular Components for Partnership

**Objective**: Update all Angular components to use new Partnership API endpoints.

**Tasks**:

1. **Create PartnershipService** (if not exists):
   - `src/app/partnership/services/partnership.service.ts`
   - Implement CRUD methods

2. **Update partnership-index.component.ts**:
   - Change all MVC endpoints to Angular routes

3. **Update partnership-details.component.ts**:
   - Use PartnershipService

4. **Update partnership-form.component.ts**:
   - Use PartnershipService

5. **Update navigation and breadcrumbs**

**Files to Modify**:
- Multiple partnership component files
- Navigation components

**Files to Create**:
- `src/app/partnership/services/partnership.service.ts` (if not exists)

**Testing**:
- Test all partnership operations
- Test navigation

**Success Criteria**:
- All partnership components use PartnershipService
- Navigation works

**Estimated Effort**: 5-7 hours

---

#### Phase 3.3: Set Up Angular Routing for Partnership

**Objective**: Configure Angular routes for Partnership module.

**Tasks**:

1. **Update app-routing.module.ts**:
   - Add partnership routes

2. **Update navigation**

3. **Test routing**

**Files to Modify**:
- `src/app/app-routing.module.ts`
- Partnership component files

**Testing**:
- Test routing

**Success Criteria**:
- Routes work correctly

**Estimated Effort**: 3-4 hours

---

#### Phase 3.4: Deprecate Old Partnership MVC Endpoints

**Objective**: Mark old Partnership MVC endpoints as deprecated.

**Tasks**:
1. Update PartnershipController.cs
2. Add logging
3. Monitor usage

**Files to Modify**:
- `RushtonRoots.Web/Controllers/PartnershipController.cs`

**Testing**:
- Verify deprecation

**Success Criteria**:
- Endpoints deprecated

**Estimated Effort**: 2 hours

---

#### Phase 3.5: Remove Old Partnership MVC Endpoints and Views

**Objective**: Complete removal of deprecated Partnership MVC controller and views.

**Prerequisites**:
- All previous Partnership phases completed
- No deprecated usage for 2+ weeks

**Tasks**:
1. Delete PartnershipController.cs
2. Delete Razor views
3. Run tests

**Files to Delete**:
- `RushtonRoots.Web/Controllers/PartnershipController.cs`
- `RushtonRoots.Web/Views/Partnership/*.cshtml`

**Testing**:
- Full regression

**Success Criteria**:
- Removed successfully

**Estimated Effort**: 2 hours

**Total Phase 3 Effort**: 16-21 hours

---

### Phase 4: ParentChild API Migration

Migrate the ParentChild controller from MVC pattern to API pattern.

#### Phase 4.1: Create ParentChildApiController

**Objective**: Create new API controller alongside existing MVC controller.

**Tasks**:
1. Create `ParentChildApiController.cs` in `/Controllers`
   - Route: `[Route("api/[controller]")]`
   - Inherit from `ControllerBase`
   - Implement RESTful endpoints:
     - `GET /api/parentchild` - Get all parent-child relationships
     - `GET /api/parentchild/{id}` - Get relationship by ID
     - `GET /api/parentchild/parent/{parentId}` - Get children of parent
     - `GET /api/parentchild/child/{childId}` - Get parents of child
     - `POST /api/parentchild` - Create relationship
     - `PUT /api/parentchild/{id}` - Update relationship
     - `DELETE /api/parentchild/{id}` - Delete relationship
   
2. Inject `IParentChildService`

3. Return JSON responses

4. Add XML comments for Swagger

5. Handle authorization

**Files to Create**:
- `RushtonRoots.Web/Controllers/ParentChildApiController.cs`

**Testing**:
- Unit tests
- Integration tests
- Manual testing

**Success Criteria**:
- API endpoints work correctly
- Swagger documentation generates

**Estimated Effort**: 4-6 hours

---

#### Phase 4.2: Update Angular Components for ParentChild

**Objective**: Update all Angular components to use new ParentChild API endpoints.

**Tasks**:

1. **Create ParentChildService** (if not exists):
   - `src/app/parent-child/services/parent-child.service.ts`
   - Implement CRUD methods

2. **Update parent-child-index.component.ts**:
   - Update to use API endpoints

3. **Update parent-child-details.component.ts**:
   - Use ParentChildService

4. **Update parent-child-form.component.ts**:
   - Use ParentChildService

5. **Update navigation**

**Files to Modify**:
- Parent-child component files
- Navigation components

**Files to Create**:
- `src/app/parent-child/services/parent-child.service.ts` (if not exists)

**Testing**:
- Test all operations
- Test navigation

**Success Criteria**:
- Components use ParentChildService
- Navigation works

**Estimated Effort**: 5-7 hours

---

#### Phase 4.3: Set Up Angular Routing for ParentChild

**Objective**: Configure Angular routes for ParentChild module.

**Tasks**:

1. **Update app-routing.module.ts**:
   - Add parent-child routes

2. **Update navigation**

3. **Test routing**

**Files to Modify**:
- `src/app/app-routing.module.ts`
- ParentChild component files

**Testing**:
- Test routing

**Success Criteria**:
- Routes work

**Estimated Effort**: 3-4 hours

---

#### Phase 4.4: Deprecate Old ParentChild MVC Endpoints

**Objective**: Mark old ParentChild MVC endpoints as deprecated.

**Tasks**:
1. Update ParentChildController.cs
2. Add logging
3. Monitor usage

**Files to Modify**:
- `RushtonRoots.Web/Controllers/ParentChildController.cs`

**Testing**:
- Verify deprecation

**Success Criteria**:
- Endpoints deprecated

**Estimated Effort**: 2 hours

---

#### Phase 4.5: Remove Old ParentChild MVC Endpoints and Views

**Objective**: Complete removal of deprecated ParentChild MVC controller and views.

**Prerequisites**:
- All previous ParentChild phases completed
- No deprecated usage for 2+ weeks

**Tasks**:
1. Delete ParentChildController.cs
2. Delete Razor views
3. Run tests

**Files to Delete**:
- `RushtonRoots.Web/Controllers/ParentChildController.cs`
- `RushtonRoots.Web/Views/ParentChild/*.cshtml`

**Testing**:
- Full regression

**Success Criteria**:
- Removed successfully

**Estimated Effort**: 2 hours

**Total Phase 4 Effort**: 16-21 hours

---

### Phase 5: Final Cleanup and Documentation

#### Phase 5.1: Enable Swagger/OpenAPI Documentation

**Objective**: Configure Swagger for all API endpoints.

**Tasks**:

1. **Install Swashbuckle.AspNetCore** (if not already installed):
   ```bash
   dotnet add package Swashbuckle.AspNetCore
   ```

2. **Update Program.cs**:
   - Add Swagger services
   - Configure Swagger UI
   - Add XML documentation
   - Configure API versioning (optional for future)

3. **Generate XML documentation**:
   - Update .csproj to generate XML docs
   - Add XML comments to all API controllers

4. **Test Swagger UI**:
   - Navigate to `/swagger`
   - Verify all endpoints documented
   - Test "Try it out" functionality

5. **Configure authentication in Swagger** (if needed):
   - Add JWT/Cookie auth config

**Files to Modify**:
- `RushtonRoots.Web/Program.cs`
- `RushtonRoots.Web/RushtonRoots.Web.csproj`
- All API controller files (add XML comments)

**Testing**:
- Access Swagger UI
- Test all endpoints via Swagger
- Verify documentation accuracy

**Success Criteria**:
- Swagger UI accessible
- All API endpoints documented
- "Try it out" works for all endpoints

**Estimated Effort**: 4-6 hours

---

#### Phase 5.2: Update API Documentation

**Objective**: Create comprehensive API documentation.

**Tasks**:

1. **Create API documentation**:
   - `docs/api-documentation.md`
   - Document all endpoints
   - Include request/response examples
   - Document authentication
   - Document error codes

2. **Update README.md**:
   - Add link to Swagger UI
   - Add link to API documentation

3. **Create API client examples**:
   - Document how to call APIs from Angular
   - Provide cURL examples
   - Provide Postman collection (optional)

**Files to Create**:
- `docs/api-documentation.md`

**Files to Modify**:
- `README.md`

**Success Criteria**:
- Comprehensive API docs exist
- README updated

**Estimated Effort**: 3-4 hours

---

#### Phase 5.3: Performance Testing and Optimization

**Objective**: Ensure API endpoints are performant.

**Tasks**:

1. **Load testing**:
   - Test API endpoints under load
   - Identify bottlenecks
   - Optimize database queries

2. **Caching**:
   - Add response caching where appropriate
   - Add ETag support for conditional requests

3. **Rate limiting** (if needed):
   - Add rate limiting middleware
   - Configure limits per endpoint

4. **Monitoring**:
   - Add application insights
   - Set up performance monitoring

**Files to Modify**:
- API controller files (add caching)
- `Program.cs` (add middleware)

**Testing**:
- Load testing with tools like k6 or JMeter
- Performance profiling

**Success Criteria**:
- APIs respond within acceptable time
- No performance regressions
- Monitoring in place

**Estimated Effort**: 4-6 hours

---

#### Phase 5.4: Final Regression Testing

**Objective**: Comprehensive testing of all migrated APIs.

**Tasks**:

1. **Automated testing**:
   - Run all unit tests
   - Run all integration tests
   - Achieve 80%+ code coverage for API controllers

2. **Manual testing**:
   - Test all CRUD operations
   - Test all Angular components
   - Test error handling
   - Test authorization
   - Test file uploads

3. **Browser testing**:
   - Test in Chrome, Firefox, Safari, Edge
   - Test mobile browsers

4. **Accessibility testing**:
   - Verify keyboard navigation
   - Verify screen reader compatibility

**Success Criteria**:
- All tests pass
- No critical bugs
- All browsers work correctly

**Estimated Effort**: 6-8 hours

---

#### Phase 5.5: Deployment and Monitoring

**Objective**: Deploy migration to production and monitor.

**Tasks**:

1. **Deploy to staging**:
   - Deploy all changes to staging environment
   - Run smoke tests
   - Monitor for issues

2. **Deploy to production**:
   - Use blue-green deployment or canary release
   - Monitor error rates
   - Monitor performance metrics

3. **Post-deployment validation**:
   - Verify all endpoints work
   - Check error logs
   - Monitor user feedback

4. **Rollback plan ready**:
   - Have rollback procedure documented
   - Test rollback in staging

**Success Criteria**:
- Successful deployment
- No critical production issues
- Monitoring shows healthy metrics

**Estimated Effort**: 4-6 hours

**Total Phase 5 Effort**: 21-30 hours

---

## Testing Strategy

### Unit Testing

**Scope**: Individual API controllers and services

**Tools**: 
- XUnit
- FakeItEasy (for mocking)

**Coverage Goal**: 80%+ for API controllers

**Test Cases**:
- Happy path for all CRUD operations
- Error handling (not found, validation errors, etc.)
- Authorization (authenticated vs. anonymous)
- Edge cases (null inputs, empty collections, etc.)

**Example**:
```csharp
[Fact]
public async Task GetById_ExistingPerson_ReturnsOk()
{
    // Arrange
    var mockService = A.Fake<IPersonService>();
    var person = new PersonViewModel { Id = 1, FirstName = "John" };
    A.CallTo(() => mockService.GetByIdAsync(1)).Returns(person);
    var controller = new PersonApiController(mockService);

    // Act
    var result = await controller.GetById(1);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var returnedPerson = Assert.IsType<PersonViewModel>(okResult.Value);
    Assert.Equal(1, returnedPerson.Id);
}
```

### Integration Testing

**Scope**: API endpoints with database

**Tools**:
- XUnit
- TestServer (ASP.NET Core)
- In-memory database or test database

**Test Cases**:
- Full request/response cycle
- Database operations
- Authentication/authorization
- File uploads
- Search functionality

**Example**:
```csharp
[Fact]
public async Task CreatePerson_ValidData_Returns201()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new CreatePersonRequest
    {
        FirstName = "Jane",
        LastName = "Doe"
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/person", request);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var person = await response.Content.ReadFromJsonAsync<PersonViewModel>();
    Assert.NotNull(person);
    Assert.Equal("Jane", person.FirstName);
}
```

### End-to-End Testing

**Scope**: Full user workflows through Angular UI

**Tools**:
- Protractor or Cypress or Playwright
- Selenium (optional)

**Test Cases**:
- User registration and login
- Create person through UI
- Edit person through UI
- Delete person through UI
- Search for people
- Navigate between pages

### Manual Testing

**Scope**: UI/UX verification, exploratory testing

**Test Cases**:
- Visual design verification
- Responsive design
- Accessibility (WCAG compliance)
- Error message clarity
- Performance (page load times)

### Performance Testing

**Scope**: API performance under load

**Tools**:
- k6
- Apache JMeter
- Artillery

**Test Cases**:
- 100 concurrent users creating people
- 1000 concurrent users searching
- Large data sets (10,000+ people)
- File upload performance (multiple photos)

**Success Criteria**:
- 95th percentile response time < 500ms
- No errors under normal load
- Graceful degradation under high load

---

## Rollback Plan

### Pre-Migration Checklist

- [ ] Complete database backup
- [ ] Tag current version in git
- [ ] Document all configuration changes
- [ ] Have rollback script ready
- [ ] Test rollback in staging environment

### Rollback Triggers

Rollback if any of the following occur:
- Critical bugs affecting core functionality
- Data corruption or loss
- Performance degradation > 50%
- Security vulnerabilities introduced
- User-reported issues > acceptable threshold

### Rollback Procedure

#### Quick Rollback (Phase 1-4 sub-phases 1-3)

If issues occur during Phases 1.1-1.3, 2.1-2.3, 3.1-3.3, or 4.1-4.3:

1. **Revert code changes**:
   ```bash
   git revert <commit-hash>
   git push
   ```

2. **Redeploy previous version**:
   ```bash
   dotnet publish -c Release
   # Deploy to server
   ```

3. **Verify old endpoints still work**:
   - Test old MVC endpoints
   - Verify Razor views render
   - Check Angular components

**Impact**: Low - Old endpoints still exist, minimal disruption

#### Full Rollback (After removing old controllers)

If issues occur after Phases 1.5, 2.5, 3.5, or 4.5:

1. **Restore database backup** (if needed):
   ```bash
   # Restore from backup
   ```

2. **Revert to tagged version**:
   ```bash
   git checkout <previous-tag>
   dotnet publish -c Release
   # Deploy to server
   ```

3. **Restore old controllers and views** (from git history):
   ```bash
   git checkout <previous-tag> -- RushtonRoots.Web/Controllers/PersonController.cs
   git checkout <previous-tag> -- RushtonRoots.Web/Views/Person/
   # Commit and deploy
   ```

4. **Notify users** of rollback and ETA for fix

**Impact**: Medium - Requires redeployment, potential downtime

### Post-Rollback Actions

1. **Root cause analysis**: Identify what went wrong
2. **Fix issues**: Address bugs in development environment
3. **Additional testing**: Add tests to prevent regression
4. **Retry migration**: After fixes validated

---

## Success Metrics

### Technical Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| API Endpoint Consistency | 100% | All CRUD APIs use `/api/[controller]` pattern |
| Code Coverage | ≥ 80% | XUnit code coverage report |
| API Response Time (p95) | < 500ms | Application Insights / monitoring |
| Swagger Documentation | 100% | All endpoints documented |
| Zero Deprecation Usage | 0 calls/day | Log analytics after 2 week grace period |
| No 404 Errors | 0 | Error monitoring |
| Angular Bundle Size | No increase > 10% | Build output analysis |

### User Experience Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Page Load Time | < 2s | Google Lighthouse |
| SPA Navigation Time | < 300ms | Performance profiling |
| User-Reported Bugs | < 5 critical bugs | Issue tracker |
| User Satisfaction | ≥ 4.5/5 | User surveys |
| Accessibility Score | ≥ 95 | WAVE, axe DevTools |

### Business Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Migration Completion | 100% | All 4 controllers migrated |
| Zero Downtime | 0 hours | Uptime monitoring |
| Development Velocity | No slowdown | Sprint velocity tracking |
| Technical Debt Reduction | -4 MVC controllers | Code metrics |

---

## Risk Mitigation

### Identified Risks

| Risk | Likelihood | Impact | Mitigation Strategy |
|------|------------|--------|---------------------|
| Data loss during migration | Low | High | Database backups before each phase; test in staging |
| Breaking existing integrations | Medium | Medium | Deprecation period; logging; monitoring |
| Performance degradation | Low | Medium | Load testing; performance monitoring; optimization |
| User confusion | Medium | Low | Clear communication; documentation; gradual rollout |
| Security vulnerabilities | Low | High | Security review; automated scanning; authorization testing |
| Incomplete testing | Medium | Medium | Comprehensive test plan; automated testing; QA review |
| Timeline overrun | Medium | Low | Buffer time; phase independence; prioritization |

### Mitigation Actions

1. **Comprehensive Testing**: Unit, integration, E2E, performance, security
2. **Staged Rollout**: Deploy to staging first, monitor, then production
3. **Monitoring**: Application Insights, error tracking, user analytics
4. **Communication**: Notify users of changes, provide documentation
5. **Rollback Plan**: Have tested rollback procedure ready
6. **Pair Programming**: Complex migrations done by 2 developers
7. **Code Review**: All changes reviewed by senior developer
8. **Security Scan**: Run CodeQL and dependency scanning before deployment

---

## Timeline Summary

| Phase | Estimated Effort | Dependencies |
|-------|------------------|--------------|
| Phase 1: Person API | 17-22 hours | None |
| Phase 2: Household API | 17-22 hours | None (can run parallel to Phase 1) |
| Phase 3: Partnership API | 16-21 hours | None (can run parallel to Phase 1, 2) |
| Phase 4: ParentChild API | 16-21 hours | None (can run parallel to Phase 1, 2, 3) |
| Phase 5: Cleanup & Docs | 21-30 hours | Phases 1-4 complete |
| **Total** | **87-116 hours** | **~11-15 weeks at 8 hrs/week** |

### Recommended Approach

**Option 1: Sequential (Lower Risk)**
- Complete one phase fully before starting next
- Easier to manage and test
- Timeline: 11-15 weeks

**Option 2: Parallel (Faster)**
- Run Phases 1-4 in parallel (different developers or modules)
- Requires more coordination
- Timeline: 6-8 weeks

**Recommendation**: Start with Phase 1 (Person) sequentially to establish pattern, then parallelize Phases 2-4.

---

## Appendix A: Endpoint Mapping Reference

### Person Endpoints

| Old MVC Pattern | New API Pattern | HTTP Method | Description |
|----------------|-----------------|-------------|-------------|
| `GET /Person` | `GET /api/person` | GET | List all people |
| `GET /Person/Details/5` | `GET /api/person/5` | GET | Get person by ID |
| `GET /Person/Create` | (Angular route) | - | Show create form |
| `POST /Person/Create` | `POST /api/person` | POST | Create person |
| `GET /Person/Edit/5` | (Angular route) | - | Show edit form |
| `POST /Person/Edit/5` | `PUT /api/person/5` | PUT | Update person |
| `GET /Person/Delete/5` | (Angular route/dialog) | - | Show delete confirmation |
| `POST /Person/Delete/5` | `DELETE /api/person/5` | DELETE | Delete person |

### Household Endpoints

| Old MVC Pattern | New API Pattern | HTTP Method | Description |
|----------------|-----------------|-------------|-------------|
| `GET /Household` | `GET /api/household` | GET | List all households |
| `GET /Household/Details/5` | `GET /api/household/5` | GET | Get household by ID |
| `GET /Household/Members/5` | `GET /api/household/5/members` | GET | Get household members |
| `POST /Household/Create` | `POST /api/household` | POST | Create household |
| `POST /Household/Edit/5` | `PUT /api/household/5` | PUT | Update household |
| `POST /Household/Delete/5` | `DELETE /api/household/5` | DELETE | Delete household |

### Partnership Endpoints

| Old MVC Pattern | New API Pattern | HTTP Method | Description |
|----------------|-----------------|-------------|-------------|
| `GET /Partnership` | `GET /api/partnership` | GET | List all partnerships |
| `GET /Partnership/Details/5` | `GET /api/partnership/5` | GET | Get partnership by ID |
| `POST /Partnership/Create` | `POST /api/partnership` | POST | Create partnership |
| `POST /Partnership/Edit/5` | `PUT /api/partnership/5` | PUT | Update partnership |
| `POST /Partnership/Delete/5` | `DELETE /api/partnership/5` | DELETE | Delete partnership |

### ParentChild Endpoints

| Old MVC Pattern | New API Pattern | HTTP Method | Description |
|----------------|-----------------|-------------|-------------|
| `GET /ParentChild` | `GET /api/parentchild` | GET | List all relationships |
| `GET /ParentChild/Details/5` | `GET /api/parentchild/5` | GET | Get relationship by ID |
| `POST /ParentChild/Create` | `POST /api/parentchild` | POST | Create relationship |
| `POST /ParentChild/Edit/5` | `PUT /api/parentchild/5` | PUT | Update relationship |
| `POST /ParentChild/Delete/5` | `DELETE /api/parentchild/5` | DELETE | Delete relationship |

---

## Appendix B: Angular Routing Reference

### Person Routes

```typescript
{
  path: 'person',
  children: [
    { path: '', component: PersonIndexComponent },  // List all people
    { path: 'create', component: PersonFormComponent },  // Create new person
    { path: ':id', component: PersonDetailsComponent },  // View person details
    { path: 'edit/:id', component: PersonFormComponent }  // Edit person
  ]
}
```

### Household Routes

```typescript
{
  path: 'household',
  children: [
    { path: '', component: HouseholdIndexComponent },  // List all households
    { path: 'create', component: HouseholdFormComponent },  // Create new household
    { path: ':id', component: HouseholdDetailsComponent },  // View household details
    { path: ':id/members', component: HouseholdMembersComponent },  // View household members
    { path: 'edit/:id', component: HouseholdFormComponent }  // Edit household
  ]
}
```

### Partnership Routes

```typescript
{
  path: 'partnership',
  children: [
    { path: '', component: PartnershipIndexComponent },  // List all partnerships
    { path: 'create', component: PartnershipFormComponent },  // Create new partnership
    { path: ':id', component: PartnershipDetailsComponent },  // View partnership details
    { path: 'edit/:id', component: PartnershipFormComponent }  // Edit partnership
  ]
}
```

### ParentChild Routes

```typescript
{
  path: 'parentchild',
  children: [
    { path: '', component: ParentChildIndexComponent },  // List all relationships
    { path: 'create', component: ParentChildFormComponent },  // Create new relationship
    { path: ':id', component: ParentChildDetailsComponent },  // View relationship details
    { path: 'edit/:id', component: ParentChildFormComponent }  // Edit relationship
  ]
}
```

---

## Appendix C: Service Interface Examples

### PersonService Interface

```typescript
export interface IPersonService {
  getAll(): Observable<PersonViewModel[]>;
  getById(id: number): Observable<PersonViewModel>;
  search(criteria: SearchPersonRequest): Observable<PersonViewModel[]>;
  create(person: CreatePersonRequest): Observable<PersonViewModel>;
  update(id: number, person: UpdatePersonRequest): Observable<PersonViewModel>;
  delete(id: number): Observable<void>;
}
```

### HouseholdService Interface

```typescript
export interface IHouseholdService {
  getAll(): Observable<HouseholdViewModel[]>;
  getById(id: number): Observable<HouseholdViewModel>;
  getMembers(id: number): Observable<PersonViewModel[]>;
  create(household: CreateHouseholdRequest): Observable<HouseholdViewModel>;
  update(id: number, household: UpdateHouseholdRequest): Observable<HouseholdViewModel>;
  delete(id: number): Observable<void>;
  addMember(householdId: number, personId: number): Observable<void>;
  removeMember(householdId: number, personId: number): Observable<void>;
}
```

---

## Document Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-12-17 | Copilot | Initial document creation |

---

**End of Document**
