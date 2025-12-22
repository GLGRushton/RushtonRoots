# Hardcoded Data Replacement - Phased Implementation Plan

**Date:** December 2025  
**Version:** 1.1  
**Status:** 🚧 In Progress - Phase 1.1 Complete

**Progress:**
- **Phase 1.1:** ✅ Complete (Home Page Statistics Service)
- **Phase 1.2:** ⏸️ Pending (Admin Dashboard Statistics Service)
- **Phase 1.3:** ⏸️ Pending (Story & Tradition Related Content Services)

---

## Executive Summary

This document outlines a comprehensive, phased plan to replace all hardcoded and sample data throughout the RushtonRoots application with dynamic data from the database. The analysis has identified multiple areas where placeholder data, sample data, or disconnected wireframes exist instead of being properly wired to backend services.

### Key Findings

**Areas Requiring Updates:**
- ✅ Home page statistics now connected to backend services (Phase 1.1 Complete)
- ✋ Admin dashboard showing placeholder values ("-")
- ✋ Multiple Angular components with sample data fallbacks
- ✋ Components with TODO comments for API integration
- ✋ Mock data in frontend components for related content

**Overall Health:**
- **Build Status:** ✅ Successful
- **Test Coverage:** ✅ 498 tests passing (+14 in Phase 1.1)
- **Architecture:** ✅ Clean Architecture properly implemented
- **Priority:** 🔶 Medium - Functional but using placeholder data

---

## Table of Contents

1. [Background & Context](#1-background--context)
2. [Identified Hardcoded Data Locations](#2-identified-hardcoded-data-locations)
3. [Phased Implementation Plan](#3-phased-implementation-plan)
4. [Success Metrics](#4-success-metrics)
5. [Risk Assessment](#5-risk-assessment)

---

## 1. Background & Context

### 1.1 Problem Statement

Several features in RushtonRoots were initially implemented as wireframes with hardcoded data to demonstrate UI/UX concepts. While the UI exists and looks complete to users, the backend services and data connections are not fully implemented. This creates a disconnect between what appears to be functional and what actually works with real data.

### 1.2 User Impact

**Current State:**
- Users see statistics that don't reflect their actual family data
- Sample/placeholder data appears instead of real family information
- Features appear functional but don't update with real data changes
- Potential confusion when counts/statistics don't match reality

**Desired State:**
- All statistics and counts reflect real database data
- No sample or placeholder data visible to users
- All UI components properly connected to backend services
- Real-time data updates reflected in the interface

### 1.3 Technical Context

The application follows Clean Architecture with:
- **Domain Layer**: Entities and models (complete)
- **Infrastructure Layer**: Database and repositories (mostly complete)
- **Application Layer**: Services and business logic (partially complete)
- **Web Layer**: Controllers and views (mixed - some wired, some not)

---

## 2. Identified Hardcoded Data Locations

### 2.1 Home Page & Dashboard

**Location:** `RushtonRoots.Web/Views/Home/Index.cshtml`  
**Component:** `app-home-page` Angular component  
**Issue:** ViewBag data not populated by HomeController

**Hardcoded/Missing Data:**
- `totalMembers` - Family member count
- `oldestAncestor` - Oldest family member
- `newestMember` - Most recently added member
- `totalPhotos` - Photo count
- `totalStories` - Story count
- `activeHouseholds` - Household count
- `recentAdditions` - Recently added people
- `upcomingBirthdays` - Upcoming birthday events
- `upcomingAnniversaries` - Upcoming anniversaries
- `recentEvents` - Recent family events
- `activityFeed` - Recent activity

**Controller:** `RushtonRoots.Web/Controllers/HomeController.cs`  
**Current State:** Returns empty ViewBag (all defaults to 0 or empty arrays)

---

### 2.2 Admin Dashboard

**Location:** `RushtonRoots.Web/Views/Admin/Dashboard.cshtml`  
**Issue:** Statistics display placeholder "-" values

**Hardcoded Values:**
```html
<div class="stat-value">-</div>  <!-- Total Users -->
<div class="stat-value">-</div>  <!-- Total Households -->
<div class="stat-value">-</div>  <!-- Total Persons -->
<div class="stat-value">-</div>  <!-- Media Items -->
```

**Controller:** `RushtonRoots.Web/Controllers/AdminController.cs`  
**Current State:** Controller has commented-out code for statistics

```csharp
// Optional: Add system statistics to ViewData
// ViewData["TotalUsers"] = await _userService.GetUserCountAsync();
// ViewData["TotalHouseholds"] = await _householdService.GetHouseholdCountAsync();
// ViewData["TotalPersons"] = await _personService.GetPersonCountAsync();
```

---

### 2.3 Parent-Child Index Component

**Location:** `RushtonRoots.Web/ClientApp/src/app/parent-child/components/parent-child-index/parent-child-index.component.ts`  
**Issue:** Uses sample data when no input data provided

**Sample Data Method:** `getSampleData(): ParentChildCard[]`  
**Lines:** 179-230+

**Current Behavior:**
```typescript
if (this.relationships && this.relationships.length > 0) {
  // Use input data if available
  this.allRelationships = this.relationships.map(/* ... */);
} else {
  // Use sample data for demonstration/testing
  this.allRelationships = this.getSampleData();
}
```

**Sample Data Includes:**
- Hardcoded parent-child relationships
- Fake person IDs (101, 201, etc.)
- Placeholder names ("John Smith", "Emily Smith", etc.)
- Fixed dates

---

### 2.4 Family Tree Component

**Location:** `RushtonRoots.Web/ClientApp/src/app/family-tree/family-tree.component.ts`  
**Issue:** Falls back to sample data on API failure

**Sample Data Method:** `loadSampleData()`  
**Lines:** 127-135+

**Current Behavior:**
```typescript
try {
  const data = await firstValueFrom(this.http.get<FamilyData>('/api/familytree/all'));
  this.allPeople = data.people || [];
} catch (err: unknown) {
  console.error('Error loading family data:', err);
  this.error = 'Failed to load family data';
  this.loadSampleData(); // Fallback to sample data
}
```

**Sample Data:**
- Hardcoded person objects
- Fake IDs and names

---

### 2.5 Story Index Component

**Location:** `RushtonRoots.Web/ClientApp/src/app/content/components/story-index/story-index.component.ts`  
**Issue:** Comments and related stories use mock data

**Methods with Mock Data:**
- `loadComments()` - Line 146: "For now, using mock data"
- `loadRelatedStories()` - Returns empty array instead of fetching from API

**Current Code:**
```typescript
loadComments(): void {
  if (!this.selectedStory) return;
  // TODO: Fetch from API
  // For now, using mock data
  this.storyComments = [];
}
```

---

### 2.6 Tradition Index Component

**Location:** `RushtonRoots.Web/ClientApp/src/app/content/components/tradition-index/tradition-index.component.ts`  
**Issue:** Multiple features using mock data

**Methods with Mock Data:**
- `loadRelatedRecipes()` - Line 192: "For now, using mock data"
- `loadRelatedStories()` - Line 203: "For now, using mock data"
- `loadOccurrences()` - Line 214: "For now, using mock data"

**Current Code:**
```typescript
loadRelatedRecipes(): void {
  // TODO: Fetch from API
  // For now, using mock data
  this.relatedRecipes = [];
}
```

---

## 3. Phased Implementation Plan

### Overview

The implementation is divided into **3 main phases**, with each phase broken into **sub-phases** for manageable PRs. Each sub-phase should be completable in a single PR to facilitate code review and minimize merge conflicts.

---

## Phase 1: Backend Services & API Endpoints

**Goal:** Create all necessary backend services to provide real data to the frontend.

**Estimated Duration:** 3-4 weeks  
**Priority:** 🔴 High - Foundation for all other phases

---

### Phase 1.1: Home Page Statistics Service

**Status:** ✅ Complete  
**Scope:** Create service to provide all statistics for the home page dashboard

**Backend Changes:**

1. **Create Service Interface & Implementation** ✅
   - File: `RushtonRoots.Application/Services/IHomePageService.cs` ✅
   - File: `RushtonRoots.Application/Services/HomePageService.cs` ✅
   - Methods:
     - `Task<HomePageStatistics> GetStatisticsAsync()` ✅
     - `Task<List<RecentAddition>> GetRecentAdditionsAsync(int count = 5)` ✅
     - `Task<List<UpcomingBirthday>> GetUpcomingBirthdaysAsync(int days = 30)` ✅
     - `Task<List<UpcomingAnniversary>> GetUpcomingAnniversariesAsync(int days = 30)` ✅
     - `Task<List<ActivityFeedItem>> GetActivityFeedAsync(int count = 20)` ✅

2. **Update HomeController** ✅
   - File: `RushtonRoots.Web/Controllers/HomeController.cs` ✅
   - Inject `IHomePageService` ✅
   - Populate ViewBag with real data ✅

3. **Create Required Domain Models** ✅
   - File: `RushtonRoots.Domain/UI/Models/HomePageStatistics.cs` ✅
   - File: `RushtonRoots.Domain/UI/Models/RecentAddition.cs` ✅
   - File: `RushtonRoots.Domain/UI/Models/UpcomingBirthday.cs` ✅
   - File: `RushtonRoots.Domain/UI/Models/UpcomingAnniversary.cs` ✅
   - File: `RushtonRoots.Domain/UI/Models/ActivityFeedItemViewModel.cs` (already existed) ✅

4. **Add Unit Tests** ✅
   - File: `RushtonRoots.UnitTests/Services/HomePageServiceTests.cs` ✅
   - Test all methods with mocked repository data ✅
   - Verify calculations (birthdays, anniversaries, etc.) ✅
   - Test edge cases (no data, etc.) ✅
   - **Coverage:** 14 comprehensive tests covering all methods and edge cases

**Success Criteria:**
- ✅ HomePageService created and tested (14 tests, 100% method coverage)
- ✅ HomeController populated with real data
- ✅ Unit tests passing (498 total tests passing)
- ✅ No hardcoded values in controller
- ✅ Service automatically registered via Autofac convention

**Implementation Notes:**
- Service directly accesses DbContext for optimal performance
- Birthday and anniversary calculations handle year transitions correctly
- Activity feed loads user information separately to avoid navigation property issues in testing
- All queries use .AsNoTracking() equivalent via projections for read-only data
- Service handles null/empty data gracefully

**Dependencies:** None

---

### Phase 1.2: Admin Dashboard Statistics Service

**Scope:** Create service to provide admin dashboard statistics

**Backend Changes:**

1. **Create Service Interface & Implementation**
   - File: `RushtonRoots.Application/Services/IAdminDashboardService.cs`
   - File: `RushtonRoots.Application/Services/AdminDashboardService.cs`
   - Methods:
     - `Task<AdminStatistics> GetSystemStatisticsAsync()`
     - `Task<List<RecentActivity>> GetRecentActivityAsync(int count = 20)`

2. **Update AdminController**
   - File: `RushtonRoots.Web/Controllers/AdminController.cs`
   - Inject `IAdminDashboardService`
   - Populate ViewData with statistics:
     ```csharp
     public async Task<IActionResult> Dashboard()
     {
         var stats = await _adminDashboardService.GetSystemStatisticsAsync();
         ViewData["TotalUsers"] = stats.TotalUsers;
         ViewData["TotalHouseholds"] = stats.TotalHouseholds;
         ViewData["TotalPersons"] = stats.TotalPersons;
         ViewData["MediaItems"] = stats.MediaItems;
         ViewData["RecentActivity"] = await _adminDashboardService.GetRecentActivityAsync();
         
         return View();
     }
     ```

3. **Create Domain Models**
   - File: `RushtonRoots.Domain/UI/Models/AdminStatistics.cs`
   - File: `RushtonRoots.Domain/UI/Models/RecentActivity.cs`

4. **Add Unit Tests**
   - File: `RushtonRoots.UnitTests/Services/AdminDashboardServiceTests.cs`
   - Test statistics calculation
   - Test activity feed generation

**Success Criteria:**
- ✅ AdminDashboardService created and tested
- ✅ AdminController populated with real data
- ✅ Unit tests passing
- ✅ No "-" placeholder values

**Dependencies:** None

---

### Phase 1.3: Story & Tradition Related Content Services

**Scope:** Create services for comments, related stories, and related content

**Backend Changes:**

1. **Enhance Story Service**
   - File: `RushtonRoots.Application/Services/IStoryService.cs` (update existing)
   - Add methods:
     - `Task<List<StoryComment>> GetStoryCommentsAsync(int storyId)`
     - `Task<List<StoryViewModel>> GetRelatedStoriesAsync(int storyId, int count = 5)`

2. **Enhance Tradition Service**
   - File: `RushtonRoots.Application/Services/ITraditionService.cs` (update existing)
   - Add methods:
     - `Task<List<RecipeViewModel>> GetRelatedRecipesAsync(int traditionId)`
     - `Task<List<StoryViewModel>> GetRelatedStoriesAsync(int traditionId)`
     - `Task<List<TraditionOccurrence>> GetPastOccurrencesAsync(int traditionId, int count = 5)`
     - `Task<TraditionOccurrence?> GetNextOccurrenceAsync(int traditionId)`

3. **Create Domain Models**
   - File: `RushtonRoots.Domain/UI/Models/StoryComment.cs`
   - File: `RushtonRoots.Domain/UI/Models/TraditionOccurrence.cs`

4. **Add Unit Tests**
   - File: `RushtonRoots.UnitTests/Services/StoryServiceTests.cs` (update)
   - File: `RushtonRoots.UnitTests/Services/TraditionServiceTests.cs` (update)
   - Test relationship queries
   - Test occurrence calculations

**Success Criteria:**
- ✅ Services enhanced with new methods
- ✅ Unit tests passing for new functionality
- ✅ Domain models created

**Dependencies:** None

---

## Phase 2: API Endpoint Updates

**Goal:** Ensure all API endpoints exist and return real data.

**Estimated Duration:** 2-3 weeks  
**Priority:** 🔴 High - Required for frontend integration

---

### Phase 2.1: Story & Tradition API Endpoints

**Scope:** Add missing API endpoints for related content

**Backend Changes:**

1. **Update StoryController API**
   - File: `RushtonRoots.Web/Controllers/Api/StoryController.cs`
   - Add endpoints:
     ```csharp
     [HttpGet("{id}/comments")]
     public async Task<IActionResult> GetComments(int id)
     
     [HttpGet("{id}/related")]
     public async Task<IActionResult> GetRelatedStories(int id, [FromQuery] int count = 5)
     ```

2. **Update TraditionController API**
   - File: `RushtonRoots.Web/Controllers/Api/TraditionController.cs`
   - Add endpoints:
     ```csharp
     [HttpGet("{id}/recipes")]
     public async Task<IActionResult> GetRelatedRecipes(int id)
     
     [HttpGet("{id}/stories")]
     public async Task<IActionResult> GetRelatedStories(int id)
     
     [HttpGet("{id}/occurrences/past")]
     public async Task<IActionResult> GetPastOccurrences(int id, [FromQuery] int count = 5)
     
     [HttpGet("{id}/occurrences/next")]
     public async Task<IActionResult> GetNextOccurrence(int id)
     ```

3. **Add API Documentation**
   - Update Swagger/OpenAPI documentation
   - Add XML comments to all new endpoints

4. **Add Integration Tests**
   - Test API endpoints return correct data structure
   - Test error handling (404, 500, etc.)
   - Test authorization if required

**Success Criteria:**
- ✅ All API endpoints implemented
- ✅ Swagger documentation updated
- ✅ Integration tests passing
- ✅ Proper error handling

**Dependencies:** Phase 1.3

---

### Phase 2.2: Parent-Child & Family Tree Data Endpoints

**Scope:** Ensure parent-child and family tree endpoints return complete data

**Backend Changes:**

1. **Review ParentChildController API**
   - File: `RushtonRoots.Web/Controllers/Api/ParentChildController.cs`
   - Verify endpoint returns complete data matching `ParentChildCard` interface
   - Add any missing fields to response

2. **Review FamilyTreeController API**
   - File: `RushtonRoots.Web/Controllers/Api/FamilyTreeController.cs`
   - Verify `/api/familytree/all` endpoint exists and returns complete data
   - Ensure response includes all fields needed by frontend

3. **Add Error Handling**
   - Ensure proper HTTP status codes
   - Return meaningful error messages
   - Handle edge cases (empty tree, circular references, etc.)

4. **Add Integration Tests**
   - Test endpoints with real database
   - Test with various data scenarios
   - Test error conditions

**Success Criteria:**
- ✅ Endpoints return complete data structures
- ✅ Error handling implemented
- ✅ Integration tests passing
- ✅ Response matches frontend interface

**Dependencies:** None (using existing services)

---

## Phase 3: Frontend Integration

**Goal:** Update Angular components to use real API data instead of sample data.

**Estimated Duration:** 2-3 weeks  
**Priority:** 🟡 Medium - User-facing changes

---

### Phase 3.1: Remove Sample Data Fallbacks

**Scope:** Remove hardcoded sample data from components

**Frontend Changes:**

1. **Update Parent-Child Index Component**
   - File: `RushtonRoots.Web/ClientApp/src/app/parent-child/components/parent-child-index/parent-child-index.component.ts`
   - Remove `getSampleData()` method
   - Update `loadRelationships()` to handle empty data gracefully:
     ```typescript
     if (this.relationships && this.relationships.length > 0) {
       this.allRelationships = this.relationships.map(/* ... */);
     } else {
       // Show empty state message instead of sample data
       this.allRelationships = [];
       this.showEmptyState = true;
     }
     ```
   - Add empty state UI to template

2. **Update Family Tree Component**
   - File: `RushtonRoots.Web/ClientApp/src/app/family-tree/family-tree.component.ts`
   - Remove `loadSampleData()` method
   - Update error handling:
     ```typescript
     catch (err: unknown) {
       console.error('Error loading family data:', err);
       this.error = 'Failed to load family data. Please try again later.';
       this.allPeople = []; // Empty instead of sample data
     }
     ```
   - Add retry button in error state

3. **Add Component Tests**
   - Test empty state rendering
   - Test error state rendering
   - Test successful data loading

**Success Criteria:**
- ✅ No sample data in production code
- ✅ Empty states render correctly
- ✅ Error states render correctly
- ✅ Component tests passing

**Dependencies:** Phase 2.2

---

### Phase 3.2: Connect Story & Tradition Components to APIs

**Scope:** Replace mock data with API calls

**Frontend Changes:**

1. **Update Story Index Component**
   - File: `RushtonRoots.Web/ClientApp/src/app/content/components/story-index/story-index.component.ts`
   - Replace `loadComments()`:
     ```typescript
     loadComments(): void {
       if (!this.selectedStory) return;
       
       this.http.get<StoryComment[]>(`/api/story/${this.selectedStory.id}/comments`)
         .subscribe({
           next: (comments) => { this.storyComments = comments; },
           error: (err) => { console.error('Failed to load comments:', err); }
         });
     }
     ```
   - Replace `loadRelatedStories()` with API call

2. **Update Tradition Index Component**
   - File: `RushtonRoots.Web/ClientApp/src/app/content/components/tradition-index/tradition-index.component.ts`
   - Replace `loadRelatedRecipes()` with API call
   - Replace `loadRelatedStories()` with API call
   - Replace `loadOccurrences()` with API call

3. **Remove TODO Comments**
   - Remove all "TODO: Fetch from API" comments
   - Remove "For now, using mock data" comments

4. **Add Component Tests**
   - Test API calls with mocked HttpClient
   - Test data binding after API response
   - Test error handling

**Success Criteria:**
- ✅ All API calls implemented
- ✅ No TODO comments for data fetching
- ✅ No mock data in components
- ✅ Component tests passing

**Dependencies:** Phase 2.1

---

### Phase 3.3: Home Page Data Binding

**Scope:** Verify home page Angular component correctly receives and displays data

**Frontend Changes:**

1. **Review Home Page Component**
   - File: `RushtonRoots.Web/ClientApp/src/app/home/components/home-page/home-page.component.ts`
   - Verify data binding from `@Input() data` works correctly
   - Ensure all statistics display properly
   - Test empty states for each section

2. **Update Templates**
   - File: `RushtonRoots.Web/ClientApp/src/app/home/components/home-page/home-page.component.html`
   - Verify all ViewBag data is properly bound
   - Add loading states if needed
   - Ensure empty states render correctly

3. **Add Visual Testing**
   - Screenshot test for full home page
   - Screenshot test for each statistic card
   - Screenshot test for empty states

**Success Criteria:**
- ✅ All statistics display real data
- ✅ Empty states render when no data
- ✅ Visual tests passing
- ✅ No hardcoded values in templates

**Dependencies:** Phase 1.1

---

## Phase 4: View Updates & Cleanup

**Goal:** Update Razor views to display real data and remove placeholders.

**Estimated Duration:** 1-2 weeks  
**Priority:** 🟡 Medium - Polish and cleanup

---

### Phase 4.1: Admin Dashboard View Updates

**Scope:** Replace placeholder values with dynamic data

**View Changes:**

1. **Update Dashboard View**
   - File: `RushtonRoots.Web/Views/Admin/Dashboard.cshtml`
   - Replace hardcoded placeholders:
     ```html
     <!-- Before -->
     <div class="stat-value">-</div>
     
     <!-- After -->
     <div class="stat-value">@ViewData["TotalUsers"]</div>
     ```
   - Apply to all statistics
   - Add fallback for null values

2. **Add Recent Activity Display**
   - Implement activity feed display
   - Show real activity data
   - Add pagination if needed

**Success Criteria:**
- ✅ No "-" placeholder values
- ✅ All statistics show real data
- ✅ Recent activity displays correctly
- ✅ Null values handled gracefully

**Dependencies:** Phase 1.2

---

### Phase 4.2: Verify All View Data Bindings

**Scope:** Review all views to ensure proper data binding

**View Changes:**

1. **Review All Razor Views**
   - Search for hardcoded values
   - Verify ViewBag/ViewData bindings
   - Check for placeholder text

2. **Update Documentation**
   - Document ViewBag contracts for each view
   - Update comments in controllers

**Success Criteria:**
- ✅ No hardcoded placeholder values
- ✅ All views properly bound to data
- ✅ Documentation updated

**Dependencies:** All previous phases

---

## 4. Success Metrics

### 4.1 Technical Metrics

**Code Quality:**
- ✅ No `TODO` comments related to data fetching
- ✅ No `getSampleData()` or `loadSampleData()` methods
- ✅ No hardcoded placeholder values in views
- ✅ 80%+ test coverage for new services

**Performance:**
- ✅ Home page loads statistics in <500ms
- ✅ Admin dashboard loads statistics in <500ms
- ✅ API endpoints respond in <200ms average

**Reliability:**
- ✅ All unit tests passing
- ✅ All integration tests passing
- ✅ Zero errors in production logs related to data fetching

### 4.2 User Experience Metrics

**Functionality:**
- ✅ Statistics accurately reflect database counts
- ✅ Recent additions show real recent data
- ✅ Birthdays/anniversaries calculated correctly
- ✅ Related content displays relevant items

**Usability:**
- ✅ Empty states clearly communicate lack of data
- ✅ Error states provide helpful messages
- ✅ Loading states prevent user confusion

### 4.3 Acceptance Criteria

For each phase to be considered complete:

1. **All code changes implemented** as specified
2. **All tests passing** (unit + integration)
3. **Code review completed** and approved
4. **Manual testing completed** on dev environment
5. **No regressions** in existing functionality
6. **Documentation updated** if applicable
7. **Performance requirements met**

---

## 5. Risk Assessment

### 5.1 Technical Risks

**Risk:** Database performance with complex statistics queries  
**Impact:** 🟡 Medium  
**Mitigation:**
- Add database indexes for common queries
- Implement caching for statistics (5-minute cache)
- Use efficient EF Core queries with proper includes
- Monitor query execution times

**Risk:** Breaking changes to existing components  
**Impact:** 🟡 Medium  
**Mitigation:**
- Comprehensive testing before changes
- Feature flags for gradual rollout
- Maintain backwards compatibility where possible
- Have rollback plan for each phase

**Risk:** API endpoint performance under load  
**Impact:** 🟢 Low  
**Mitigation:**
- Load testing before deployment
- Implement response caching
- Use pagination for large result sets
- Monitor API performance metrics

### 5.2 Schedule Risks

**Risk:** Phase dependencies cause delays  
**Impact:** 🟡 Medium  
**Mitigation:**
- Clear phase boundaries
- Work can start on frontend while backend in progress
- Parallel work on independent phases
- Regular progress reviews

**Risk:** Testing takes longer than estimated  
**Impact:** 🟢 Low  
**Mitigation:**
- Allocate 30% buffer for testing
- Automated testing to speed up validation
- Clear acceptance criteria upfront

### 5.3 Quality Risks

**Risk:** Incomplete data causes empty states  
**Impact:** 🟡 Medium  
**Mitigation:**
- Design proper empty states for all components
- Provide helpful messages guiding users to add data
- Seed database with example data for development
- Documentation on expected data

**Risk:** Edge cases not handled  
**Impact:** 🟡 Medium  
**Mitigation:**
- Comprehensive test coverage including edge cases
- Manual testing with various data scenarios
- Handle null/empty values gracefully
- Error logging for unexpected scenarios

---

## 6. Testing Strategy

### 6.1 Unit Testing

**For Each Service:**
- Test all public methods
- Test with empty data
- Test with null inputs
- Test calculation logic
- Mock all dependencies

**Target Coverage:** 80%+ for new code

### 6.2 Integration Testing

**For Each API Endpoint:**
- Test successful responses
- Test 404 responses (not found)
- Test 400 responses (bad request)
- Test authorization (if applicable)
- Test with real database

**Target Coverage:** All happy paths + major error paths

### 6.3 Manual Testing

**For Each Phase:**
- Test in browser with real data
- Test with empty database
- Test with large dataset
- Test error scenarios
- Cross-browser testing (Chrome, Firefox, Safari, Edge)
- Mobile responsive testing

### 6.4 Performance Testing

**Key Metrics:**
- Page load time <2 seconds
- API response time <200ms average
- Database query time <100ms
- No N+1 query problems

**Tools:**
- Browser DevTools for frontend
- MiniProfiler for backend
- SQL Server Profiler for database

---

## 7. Implementation Guidelines

### 7.1 Coding Standards

**Follow Existing Patterns:**
- Use convention-based DI registration
- Follow existing service patterns
- Match existing API response structures
- Maintain consistent error handling

**Code Quality:**
- No magic numbers or strings
- Meaningful variable names
- XML comments on public methods
- Keep methods focused and small

### 7.2 Pull Request Guidelines

**Each PR Should:**
- Focus on single sub-phase
- Include all tests
- Update relevant documentation
- Be reviewable in <1 hour
- Have passing CI/CD checks

**PR Description Template:**
```markdown
## Phase [X.Y]: [Sub-phase Name]

### Changes Made
- [ ] Created service/controller/component
- [ ] Added unit tests
- [ ] Added integration tests
- [ ] Updated documentation

### Testing Performed
- [ ] Unit tests passing (X tests)
- [ ] Integration tests passing (X tests)
- [ ] Manual testing completed
- [ ] No regressions found

### Screenshots
[If UI changes]

### Notes
[Any important information for reviewers]
```

### 7.3 Database Considerations

**Indexing Strategy:**
- Add indexes for statistics queries
- Index on CreatedDateTime for recent additions
- Index on BirthDate for birthday calculations
- Monitor index usage and effectiveness

**Query Optimization:**
- Use .AsNoTracking() for read-only queries
- Use proper .Include() for navigation properties
- Avoid N+1 queries
- Use projection (.Select()) when full entity not needed

---

## 8. Rollout Strategy

### 8.1 Development Environment

**Phase Completion:**
1. Implement changes on feature branch
2. Run all tests
3. Manual testing by developer
4. Create PR for review

### 8.2 Staging/Testing Environment

**After PR Merge:**
1. Deploy to staging environment
2. Run automated test suite
3. Manual QA testing
4. Performance testing
5. Stakeholder review

### 8.3 Production Deployment

**After Staging Approval:**
1. Deploy during low-traffic window
2. Monitor error logs
3. Monitor performance metrics
4. Have rollback plan ready
5. Gradual rollout with monitoring

### 8.4 Rollback Plan

**If Issues Detected:**
1. Immediate rollback to previous version
2. Analyze logs and errors
3. Fix issues in development
4. Re-test thoroughly
5. Schedule new deployment

---

## 9. Documentation Updates

### 9.1 Developer Documentation

**Update These Files:**
- `README.md` - If new setup steps required
- `PATTERNS.md` - If new patterns introduced
- `docs/ApiDocumentation.md` - For new API endpoints
- `docs/DeveloperOnboarding.md` - If workflow changes

### 9.2 API Documentation

**For Each New Endpoint:**
- XML comments in code
- Swagger/OpenAPI schema
- Example requests/responses
- Error code documentation

### 9.3 User Documentation

**Update If Needed:**
- Help documentation for new features
- Screenshots if UI changed
- FAQs for common questions

---

## 10. Monitoring & Maintenance

### 10.1 Post-Deployment Monitoring

**Monitor These Metrics:**
- Error rates in application logs
- API response times
- Database query performance
- User engagement with new data

**Alert Thresholds:**
- Error rate >1% - immediate alert
- API response >500ms - warning
- Database query >200ms - warning

### 10.2 Ongoing Maintenance

**Regular Reviews:**
- Weekly monitoring of statistics accuracy
- Monthly performance review
- Quarterly code review for improvements

**Data Quality:**
- Monitor for edge cases in production
- Track user feedback on data accuracy
- Refine calculations as needed

---

## 11. Appendix

### 11.1 File Locations Quick Reference

**Backend Services:**
- Services: `RushtonRoots.Application/Services/`
- Domain Models: `RushtonRoots.Domain/UI/Models/`
- Controllers: `RushtonRoots.Web/Controllers/`
- API Controllers: `RushtonRoots.Web/Controllers/Api/`

**Frontend Components:**
- Angular Components: `RushtonRoots.Web/ClientApp/src/app/`
- Home Page: `RushtonRoots.Web/ClientApp/src/app/home/`
- Parent-Child: `RushtonRoots.Web/ClientApp/src/app/parent-child/`
- Content: `RushtonRoots.Web/ClientApp/src/app/content/`

**Views:**
- Razor Views: `RushtonRoots.Web/Views/`
- Home Views: `RushtonRoots.Web/Views/Home/`
- Admin Views: `RushtonRoots.Web/Views/Admin/`

**Tests:**
- Unit Tests: `RushtonRoots.UnitTests/`

### 11.2 Related Documentation

- **Architecture**: `/README.md`, `/PATTERNS.md`
- **API Reference**: `/docs/ApiDocumentation.md`
- **Development Setup**: `/docs/DeveloperOnboarding.md`
- **Deployment**: `/docs/DeploymentGuide.md`

### 11.3 Communication Plan

**Status Updates:**
- Daily: Team standup with progress update
- Weekly: Email update to stakeholders
- Per Phase: Demo to stakeholders

**Issue Escalation:**
- Blockers raised immediately
- Technical decisions discussed in PR comments
- Major changes require design review

---

## 12. Summary

This phased plan provides a comprehensive roadmap to replace all hardcoded and sample data throughout the RushtonRoots application with dynamic, database-driven data. By following this structured approach:

1. **Backend services** are built first to provide data
2. **API endpoints** ensure data is accessible
3. **Frontend components** integrate with APIs
4. **Views** are updated to display real data

Each phase builds on the previous, with clear success criteria and testing requirements. The plan prioritizes user-facing features while maintaining code quality and system performance.

**Total Estimated Duration:** 8-12 weeks  
**Phases:** 3 main phases, 10 sub-phases  
**Success Metric:** Zero hardcoded data in production code

---

**Document Status:** ✅ Complete  
**Next Steps:** Begin Phase 1.1 implementation  
**Last Updated:** December 2025  
**Document Owner:** Development Team
