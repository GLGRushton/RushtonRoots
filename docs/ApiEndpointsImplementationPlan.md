# API Endpoints Implementation Plan - RushtonRoots

**Created:** 2025-12-17  
**Purpose:** Phased implementation plan for all missing API endpoints and controllers  
**Related:** [InternalLinks.md](./InternalLinks.md)

---

## Table of Contents
1. [Overview](#overview)
2. [Implementation Phases](#implementation-phases)
3. [Phase Breakdown](#phase-breakdown)
4. [Success Criteria](#success-criteria)
5. [Risk Mitigation](#risk-mitigation)

---

## Overview

This plan outlines the implementation of all missing API endpoints and MVC controllers identified in the Internal Links Analysis. The work is divided into phases based on:
- **Priority** (Critical > High > Medium > Low)
- **Dependencies** (foundational work first)
- **Complexity** (smaller PRs for easier review)
- **User Impact** (customer-facing features prioritized)

### Summary Statistics
- **Total Phases:** 4 major phases
- **Total Sub-phases:** 12 sub-phases
- **Estimated Duration:** 6-8 weeks
- **API Controllers to Create:** 4
- **MVC Controllers to Create:** 5
- **Endpoints to Implement:** ~60+

---

## Implementation Phases

### Phase 1: Core API Controllers (Critical - Weeks 1-2)
**Goal:** Implement missing API endpoints for core entities used by Angular components

**Sub-phases:**
- Phase 1.1: Person API Controller
- Phase 1.2: Partnership & ParentChild API Controllers
- Phase 1.3: Household API Controller

### Phase 2: Media & Visualization Controllers (High - Weeks 3-4)
**Goal:** Implement MVC controllers for user-facing features

**Sub-phases:**
- Phase 2.1: MediaGallery MVC Controller
- Phase 2.2: FamilyTree MVC Controller
- Phase 2.3: Calendar MVC Controller

### Phase 3: User Experience Enhancements (Medium - Weeks 5-6)
**Goal:** Add administrative and user-facing features

**Sub-phases:**
- Phase 3.1: Account Additional Actions
- Phase 3.2: Admin Controller
- Phase 3.3: Help Controller

### Phase 4: Code Organization & Cleanup (Low - Weeks 7-8)
**Goal:** Reorganize controllers and remove deprecated patterns

**Sub-phases:**
- Phase 4.1: Organize API Controllers into Controllers/Api/
- Phase 4.2: Static/Info Page Controllers
- Phase 4.3: Deprecate Old MVC POST Patterns

---

## Phase Breakdown

---

## Phase 1: Core API Controllers (Critical)

### Phase 1.1: Person API Controller

**Goal:** Create RESTful API controller for Person entity to support Angular person-form component

**Why First:**
- Angular `person-form.component.ts` explicitly references `/api/person` endpoints
- Currently returns 404 errors
- Blocking Angular form functionality

**Scope:**
```
Controllers/Api/PersonController.cs
```

**Endpoints to Implement:**
- `GET /api/person` - List all persons (with pagination)
- `GET /api/person/{id}` - Get person by ID
- `POST /api/person` - Create new person (with photo upload)
- `PUT /api/person/{id}` - Update person (with photo upload)
- `DELETE /api/person/{id}` - Delete person (soft delete)
- `GET /api/person/search` - Search persons (query parameters)

**Technical Requirements:**
1. Create `PersonApiController` in `Controllers/Api/` directory
2. Add `[ApiController]` and `[Route("api/person")]` attributes
3. Support multipart/form-data for photo uploads
4. Reuse existing `IPersonService` from Application layer
5. Add proper error handling and status codes
6. Add anti-forgery token validation for POST/PUT/DELETE
7. Add authorization attributes (Admin/HouseholdAdmin for write operations)

**Dependencies:**
- IPersonService (✅ Already exists)
- IHouseholdService (✅ Already exists)
- Domain models (✅ Already exists)

**Testing:**
- Unit tests for all endpoints
- Integration tests for photo upload
- Test Angular person-form integration

**Estimated Effort:** 2-3 days

**Acceptance Criteria:**
- [x] All 6 endpoints implemented and tested
- [x] Angular person-form.component.ts successfully creates/updates persons
- [x] Photo upload works correctly
- [x] Proper validation and error messages returned
- [x] Authorization working correctly

**Status:** ✅ **COMPLETE** (2025-12-17)

**Deliverables:**
- PersonApiController with 6 RESTful endpoints
- 19 comprehensive unit tests (all passing)
- Support for multipart/form-data photo uploads
- Enhanced Person entity with 8 additional fields
- Database migration applied
- Full integration with existing PersonService

---

### Phase 1.2: Partnership & ParentChild API Controllers

**Goal:** Create RESTful API controllers for relationship entities

**Why Second:**
- Core relationship management features
- Currently only have MVC controllers
- Needed for API-first architecture

**Scope:**
```
Controllers/Api/PartnershipController.cs
Controllers/Api/ParentChildController.cs
```

**Endpoints to Implement:**

#### Partnership API
- `GET /api/partnership` - List all partnerships
- `GET /api/partnership/{id}` - Get partnership by ID
- `POST /api/partnership` - Create partnership
- `PUT /api/partnership/{id}` - Update partnership
- `DELETE /api/partnership/{id}` - Delete partnership
- `GET /api/partnership/person/{personId}` - Get partnerships for person

#### ParentChild API
- `GET /api/parentchild` - List all parent-child relationships
- `GET /api/parentchild/{id}` - Get relationship by ID
- `POST /api/parentchild` - Create relationship
- `PUT /api/parentchild/{id}` - Update relationship
- `DELETE /api/parentchild/{id}` - Delete relationship
- `GET /api/parentchild/person/{personId}` - Get relationships for person
- `GET /api/parentchild/parents/{childId}` - Get parents of child
- `GET /api/parentchild/children/{parentId}` - Get children of parent

**Technical Requirements:**
1. Create controllers in `Controllers/Api/` directory
2. Reuse existing services from Application layer
3. Add proper validation for relationship integrity
4. Add authorization (Admin/HouseholdAdmin for modifications)
5. Return proper HTTP status codes
6. Include related entity information in responses

**Dependencies:**
- IPartnershipService (✅ Already exists)
- IParentChildService (✅ Already exists)

**Testing:**
- Unit tests for all endpoints
- Integration tests for relationship creation/deletion
- Test relationship integrity validation

**Estimated Effort:** 3-4 days

**Acceptance Criteria:**
- [x] All Partnership endpoints implemented and tested
- [x] All ParentChild endpoints implemented and tested
- [x] Relationship integrity validated
- [x] Proper error handling for invalid relationships
- [x] Authorization working correctly

**Status:** ✅ **COMPLETE** (2025-12-17)

**Deliverables:**
- PartnershipController with 6 RESTful endpoints
- ParentChildController with 9 RESTful endpoints (including 3 specialized query endpoints)
- 21 comprehensive unit tests for PartnershipController (all passing)
- 25 comprehensive unit tests for ParentChildController (all passing)
- Relationship integrity validation (prevents self-partnerships and self-parenting)
- Full integration with existing PartnershipService and ParentChildService
- Authorization implemented (Admin/HouseholdAdmin for write operations)

---

### Phase 1.3: Household API Controller

**Goal:** Create RESTful API controller for Household entity

**Why Third:**
- Core family organization feature
- Currently only has MVC controller
- Needed for household management features

**Scope:**
```
Controllers/Api/HouseholdController.cs
```

**Endpoints to Implement:**
- `GET /api/household` - List all households
- `GET /api/household/{id}` - Get household by ID
- `POST /api/household` - Create household
- `PUT /api/household/{id}` - Update household
- `DELETE /api/household/{id}` - Delete household
- `GET /api/household/{id}/members` - Get household members
- `POST /api/household/{id}/members` - Add member to household
- `DELETE /api/household/{id}/members/{personId}` - Remove member
- `PUT /api/household/{id}/settings` - Update household settings

**Technical Requirements:**
1. Create controller in `Controllers/Api/` directory
2. Reuse existing `IHouseholdService`
3. Add member management endpoints
4. Add household settings management
5. Add authorization (HouseholdAdmin or owner only)
6. Include member count in list responses

**Dependencies:**
- IHouseholdService (✅ Already exists)
- IPersonService (✅ Already exists)

**Testing:**
- Unit tests for all endpoints
- Integration tests for member management
- Test household settings updates

**Estimated Effort:** 2-3 days

**Status:** ✅ **COMPLETE** (2025-12-17)

**Deliverables:**
- HouseholdController API with 9 RESTful endpoints
- 31 comprehensive unit tests (all passing)
- Request models for member management (AddHouseholdMemberRequest, UpdateHouseholdSettingsRequest)
- Extended IHouseholdService with member management methods
- Full integration with existing HouseholdService and HouseholdRepository
- Authorization implemented (Admin for create/delete, Admin/HouseholdAdmin for updates and member management)

**Implementation Notes:**
- Member removal endpoint (`DELETE /api/household/{id}/members/{personId}`) throws an error by design, as persons must belong to exactly one household per domain model. To move a person between households, use the Add Member endpoint to assign them to a new household.
- Settings management includes archive/unarchive functionality
- All endpoints include proper validation and error handling

---

## Phase 2: Media & Visualization Controllers (High Priority)

### Phase 2.1: MediaGallery MVC Controller

**Goal:** Create MVC controller and views for photo/video gallery

**Why First in Phase 2:**
- Direct link from home page (high visibility)
- User-facing feature with immediate impact
- MediaController API already exists

**Scope:**
```
Controllers/MediaGalleryController.cs
Views/MediaGallery/Index.cshtml
Views/MediaGallery/Upload.cshtml
```

**Actions to Implement:**
- `GET /MediaGallery` - Display photo/video gallery
- `GET /MediaGallery/Upload` - Upload page
- `GET /MediaGallery?type=video` - Filter by video type

**Technical Requirements:**
1. Create `MediaGalleryController` (MVC controller)
2. Create `Index` action - integrates with MediaController API
3. Create `Upload` action - file upload page
4. Create responsive gallery view with thumbnails
5. Support filtering by type (photo/video)
6. Support pagination
7. Add authorization (authenticated users only)
8. Integrate with existing Angular media components if available

**Dependencies:**
- MediaController API (✅ Already exists)
- Media domain models (✅ Already exists)

**Testing:**
- Manual testing of gallery display
- Test upload functionality
- Test filtering and pagination
- Test on mobile devices

**Estimated Effort:** 3-4 days

**Acceptance Criteria:**
- [x] Gallery displays photos and videos
- [x] Upload page works correctly
- [x] Filtering by type functions properly
- [x] Pagination implemented
- [x] Responsive design on mobile
- [x] Link from home page works

**Status:** ✅ **COMPLETE** (2025-12-17)

**Deliverables:**
- MediaGalleryController with Index and Upload actions
- Index.cshtml view integrating Angular media-gallery component
- Upload.cshtml view integrating Angular photo-upload component
- 13 comprehensive unit tests (all passing)
- Support for pagination and filtering by media type (photo/video)
- Authorization via [Authorize] attribute (authenticated users only)
- Responsive fallback UI for non-JavaScript browsers
- Full integration with existing MediaController API and Angular components

**Implementation Notes:**
- Leveraged existing Angular media-gallery component registered as custom element
- Used data-* attributes to avoid Razor directive conflicts
- Following established MVC controller patterns from HomeController
- Minimal changes approach - reusing existing Angular components and API endpoints
- Gallery accessible at /MediaGallery, Upload at /MediaGallery/Upload
- Filtering via query parameters: ?type=video or ?type=photo
- Pagination via query parameters: ?page=2&pageSize=48

---

### Phase 2.2: FamilyTree MVC Controller

**Goal:** Create MVC controller and view for family tree visualization

**Why Second:**
- Linked from home page
- Key genealogy feature
- FamilyTreeController API already exists

**Scope:**
```
Controllers/FamilyTreeMvcController.cs
Views/FamilyTree/Index.cshtml
```

**Actions to Implement:**
- `GET /FamilyTree` - Display interactive family tree

**Technical Requirements:**
1. Add `Index` action to existing FamilyTreeController (API controller)
   - OR create separate MVC controller
2. Create tree visualization view
3. Integrate with FamilyTreeController API endpoints
4. Support interactive tree navigation
5. Add person selection and detail view
6. Add zoom/pan controls
7. Add authorization (authenticated users only)

**Dependencies:**
- FamilyTreeController API (✅ Already exists)
- Family tree visualization library (✅ Using existing Angular family-tree component)

**Technology Decisions:**
- ✅ Tree visualization library: Angular family-tree component (already registered as custom element)
- ✅ Rendering approach: Client-side rendering with Angular

**Testing:**
- Manual testing of tree display
- Test with small and large family trees
- Test interactive features
- Cross-browser testing

**Estimated Effort:** 4-5 days

**Acceptance Criteria:**
- [x] Family tree displays correctly
- [x] Interactive navigation works
- [x] Person details accessible
- [x] Performance acceptable with large datasets
- [x] Link from home page works

**Status:** ✅ **COMPLETE** (2025-12-17)

**Deliverables:**
- FamilyTreeMvcController with Index action
- Views/FamilyTree/Index.cshtml integrating Angular family-tree component
- 22 comprehensive unit tests (all passing)
- Support for multiple view modes (descendant, pedigree, fan)
- Person selection via personId parameter
- Zoom/pan controls via Angular component
- Authorization via [Authorize] attribute (authenticated users only)
- Responsive design with noscript fallback
- Full integration with existing FamilyTreeController API endpoints

**Implementation Notes:**
- Created separate MVC controller (FamilyTreeMvcController) instead of mixing with API controller
- Leveraged existing Angular family-tree component registered as custom element
- Used data attributes and ViewData to pass parameters to Angular component
- Following established MVC controller patterns from MediaGalleryController
- Minimal changes approach - reusing existing Angular component and API endpoints
- Family tree accessible at /FamilyTree
- View mode selection via query parameter: ?view=pedigree or ?view=fan
- Person focus via query parameter: ?personId=42
- Generation depth via query parameter: ?generations=5

---

### Phase 2.3: Calendar MVC Controller

**Goal:** Create MVC controller and views for family calendar

**Why Third:**
- Referenced in navigation menu
- Important for event management
- FamilyEventController API already exists

**Scope:**
```
Controllers/CalendarController.cs
Views/Calendar/Index.cshtml
Views/Calendar/Create.cshtml
```

**Actions to Implement:**
- `GET /Calendar` - Display calendar view
- `GET /Calendar/Create` - Create event page

**Technical Requirements:**
1. Create `CalendarController` (MVC controller)
2. Create `Index` action - displays calendar
3. Create `Create` action - event creation page
4. Integrate with FamilyEventController API
5. Support multiple calendar views (month, week, day)
6. Show birthdays, anniversaries, events
7. Add event RSVP integration
8. Add authorization (authenticated users)

**Dependencies:**
- FamilyEventController API (✅ Already exists)
- EventRsvpController API (✅ Already exists)
- Calendar UI component (needs selection)

**Technology Decisions:**
- Choose calendar library (FullCalendar, custom Angular calendar component)
- Decide if Angular calendar component already exists to reuse

**Testing:**
- Manual testing of calendar display
- Test event creation
- Test RSVP functionality
- Test recurring events

**Estimated Effort:** 4-5 days

**Acceptance Criteria:**
- [ ] Calendar displays events correctly
- [ ] Event creation works
- [ ] Different view modes functional
- [ ] RSVP integration working
- [ ] Navigation menu link works

---

## Phase 3: User Experience Enhancements (Medium Priority)

### Phase 3.1: Account Additional Actions

**Goal:** Add missing Account controller actions

**Why First in Phase 3:**
- User-facing features
- Referenced in navigation
- Extends existing AccountController

**Scope:**
```
Controllers/AccountController.cs (extend existing)
Views/Account/Notifications.cshtml
Views/Account/Settings.cshtml
```

**Actions to Implement:**
- `GET /Account/Notifications` - User notifications page
- `GET /Account/Settings` - User settings page

**Technical Requirements:**
1. Add `Notifications` action to AccountController
2. Create notifications view
3. Integrate with NotificationController API
4. Add `Settings` action to AccountController
5. Create user settings view
6. Add settings management (preferences, privacy, etc.)
7. Add authorization (authenticated users only)

**Dependencies:**
- NotificationController API (✅ Already exists)
- User settings domain model (may need creation)

**Testing:**
- Manual testing of notifications page
- Test settings updates
- Test notification preferences

**Estimated Effort:** 2-3 days

**Acceptance Criteria:**
- [ ] Notifications page displays user notifications
- [ ] Settings page allows preference updates
- [ ] Changes save correctly
- [ ] Navigation menu links work

---

### Phase 3.2: Admin Controller

**Goal:** Create admin controller and dashboard

**Why Second:**
- Administrative features
- Referenced in navigation menu
- Needed for system management

**Scope:**
```
Controllers/AdminController.cs
Views/Admin/Settings.cshtml
Views/Admin/Dashboard.cshtml (optional)
```

**Actions to Implement:**
- `GET /Admin/Settings` - System settings page
- `GET /Admin/Dashboard` - Admin dashboard (optional)

**Technical Requirements:**
1. Create `AdminController`
2. Add `Settings` action - system configuration
3. Add authorization (Admin role only)
4. Create settings view
5. Add system settings management
6. Add user management integration
7. Add activity logs (optional)

**Dependencies:**
- Admin-specific services (may need creation)
- System settings storage

**Testing:**
- Manual testing with admin account
- Test settings updates
- Test authorization (non-admins blocked)

**Estimated Effort:** 3-4 days

**Acceptance Criteria:**
- [ ] Admin dashboard accessible to admins only
- [ ] Settings page functional
- [ ] Authorization properly enforced
- [ ] Navigation menu link works for admins

---

### Phase 3.3: Help Controller

**Goal:** Create help/documentation controller

**Why Third:**
- Documentation feature
- Multiple help pages referenced
- Lower user impact

**Scope:**
```
Controllers/HelpController.cs
Views/Help/Index.cshtml
Views/Help/GettingStarted.cshtml
Views/Help/Account.cshtml
Views/Help/Calendar.cshtml
Views/Help/PersonManagement.cshtml
Views/Help/HouseholdManagement.cshtml
Views/Help/RelationshipManagement.cshtml
Views/Help/Recipes.cshtml
Views/Help/Stories.cshtml
Views/Help/Traditions.cshtml
Views/Help/Wiki.cshtml
```

**Actions to Implement:**
- `GET /Help` - Help index/landing page
- `GET /Help/GettingStarted` - Getting started guide
- `GET /Help/Account` - Account help
- `GET /Help/Calendar` - Calendar help
- `GET /Help/PersonManagement` - Person management help
- `GET /Help/HouseholdManagement` - Household help
- `GET /Help/RelationshipManagement` - Relationship help
- `GET /Help/Recipes` - Recipe help
- `GET /Help/Stories` - Stories help
- `GET /Help/Traditions` - Traditions help
- `GET /Help/Wiki` - Wiki help

**Technical Requirements:**
1. Create `HelpController`
2. Create action for each help topic
3. Create help views with documentation
4. Add search functionality (optional)
5. Add table of contents
6. Add screenshots and examples
7. No authorization needed (public help)

**Dependencies:**
- None (static content)

**Testing:**
- Manual review of all help pages
- Test navigation between pages
- Test search functionality if implemented

**Estimated Effort:** 4-5 days (content creation takes time)

**Acceptance Criteria:**
- [ ] All help pages accessible
- [ ] Content clear and helpful
- [ ] Screenshots and examples included
- [ ] Navigation between topics works
- [ ] Navigation menu links work

---

## Phase 4: Code Organization & Cleanup (Low Priority)

### Phase 4.1: Organize API Controllers into Controllers/Api/

**Goal:** Move all API controllers to organized directory structure

**Why First in Phase 4:**
- Code organization
- Easier maintenance
- Follows best practices

**Scope:**
- Move 27 API controllers to `Controllers/Api/` directory
- Update namespaces
- Update any references

**Files to Move:**
```
Controllers/ActivityFeedController.cs → Controllers/Api/ActivityFeedController.cs
Controllers/ChatRoomController.cs → Controllers/Api/ChatRoomController.cs
Controllers/CommentController.cs → Controllers/Api/CommentController.cs
... (all 27 API controllers)
```

**Technical Requirements:**
1. Create `Controllers/Api/` directory
2. Move all API controllers (those with [ApiController] attribute)
3. Update namespaces to `RushtonRoots.Web.Controllers.Api`
4. Ensure routing still works (should not change)
5. Update any controller references
6. Update AutofacModule if needed

**Dependencies:**
- None (pure refactoring)

**Testing:**
- Run all existing API tests
- Manual smoke testing of all APIs
- Verify routing unchanged

**Estimated Effort:** 1 day

**Acceptance Criteria:**
- [ ] All API controllers in Controllers/Api/ directory
- [ ] All tests passing
- [ ] No broken references
- [ ] Routing unchanged
- [ ] Application builds and runs correctly

---

### Phase 4.2: Static/Info Page Controllers

**Goal:** Create controllers for static informational pages

**Why Second:**
- Low priority features
- Simple static content
- Referenced but not critical

**Scope:**
```
Controllers/InfoController.cs
Views/Info/About.cshtml
Views/Info/Contact.cshtml
Views/Info/Help.cshtml
Views/Info/Mission.cshtml
Views/Info/Privacy.cshtml
Views/Info/Terms.cshtml
Views/Info/Story.cshtml
```

**Actions to Implement:**
- `GET /About` - About page
- `GET /Contact` - Contact page
- `GET /Mission` - Mission statement
- `GET /Privacy` - Privacy policy
- `GET /Terms` - Terms of service
- `GET /Story` - Family story

**Technical Requirements:**
1. Create `InfoController` (single controller for all static pages)
2. Create action for each page
3. Create static content views
4. Add proper meta tags for SEO
5. Add social sharing tags
6. No authorization needed (public pages)

**Dependencies:**
- None (static content)

**Testing:**
- Manual review of all pages
- Check SEO meta tags
- Verify social sharing works

**Estimated Effort:** 2-3 days

**Acceptance Criteria:**
- [ ] All static pages accessible
- [ ] Content complete and accurate
- [ ] SEO meta tags present
- [ ] Social sharing functional
- [ ] Links work from navigation

---

### Phase 4.3: Deprecate Old MVC POST Patterns

**Goal:** Remove deprecated MVC POST actions in favor of API endpoints

**Why Last:**
- Breaking changes
- Requires careful migration
- Ensures API stability first

**Scope:**
- Deprecate MVC POST actions for Person, Partnership, ParentChild
- Update Angular forms to use APIs
- Remove or mark obsolete old endpoints

**Endpoints to Deprecate:**
```
POST /Person/Create → Use POST /api/person
POST /Person/Edit → Use PUT /api/person/{id}
POST /Person/Delete → Use DELETE /api/person/{id}
POST /Partnership/Create → Use POST /api/partnership
POST /Partnership/Edit → Use PUT /api/partnership/{id}
POST /Partnership/Delete → Use DELETE /api/partnership/{id}
POST /ParentChild/Create → Use POST /api/parentchild
POST /ParentChild/Edit → Use PUT /api/parentchild/{id}
POST /ParentChild/Delete → Use DELETE /api/parentchild/{id}
```

**Technical Requirements:**
1. Ensure all API endpoints stable and tested
2. Update Angular components to use API endpoints
3. Add [Obsolete] attributes to old MVC POST actions
4. Add deprecation warnings in logs
5. Update documentation
6. Plan removal timeline

**Migration Steps:**
1. Add [Obsolete] warnings (1 sprint)
2. Monitor usage in logs (1 sprint)
3. Remove unused endpoints (1 sprint)

**Dependencies:**
- Phases 1.1, 1.2, 1.3 complete (API controllers exist)
- All Angular forms updated

**Testing:**
- Ensure all forms work with new APIs
- Monitor for any lingering MVC POST usage
- Integration testing

**Estimated Effort:** 2-3 days (+ 3 sprints for full deprecation)

**Acceptance Criteria:**
- [ ] All Angular forms use API endpoints
- [ ] Old MVC POST actions marked obsolete
- [ ] Deprecation warnings in logs
- [ ] Documentation updated
- [ ] Migration timeline established

---

## Success Criteria

### Overall Project Success Metrics

1. **Functionality**
   - [ ] All referenced URLs return 200 OK (not 404)
   - [ ] All Angular forms function correctly with APIs
   - [ ] All navigation links work
   - [ ] No console errors in browser

2. **Code Quality**
   - [ ] All new controllers follow SOLID principles
   - [ ] All controllers have unit tests (>80% coverage)
   - [ ] All API endpoints documented (Swagger/OpenAPI)
   - [ ] Code review approval for each PR

3. **Performance**
   - [ ] API response times < 200ms (95th percentile)
   - [ ] Page load times < 2 seconds
   - [ ] No N+1 query issues

4. **Security**
   - [ ] All endpoints have proper authorization
   - [ ] Anti-forgery tokens validated
   - [ ] Input validation on all endpoints
   - [ ] No sensitive data in error messages

5. **User Experience**
   - [ ] All features accessible from navigation
   - [ ] Forms provide clear validation feedback
   - [ ] Error messages user-friendly
   - [ ] Mobile responsive

### Phase Success Metrics

Each phase is considered complete when:
- [ ] All endpoints/actions implemented
- [ ] Unit tests passing (>80% coverage)
- [ ] Integration tests passing
- [ ] Manual testing completed
- [ ] Code review approved
- [ ] Documentation updated
- [ ] PR merged to main branch

---

## Risk Mitigation

### Identified Risks and Mitigation Strategies

#### Risk 1: Breaking Changes to Existing Functionality
**Likelihood:** Medium  
**Impact:** High  
**Mitigation:**
- Implement new API controllers alongside existing MVC controllers
- Don't remove MVC controllers until API is stable
- Use feature flags for new endpoints
- Comprehensive testing before deprecation

#### Risk 2: Performance Issues with New APIs
**Likelihood:** Medium  
**Impact:** Medium  
**Mitigation:**
- Load testing before production deployment
- Implement caching where appropriate
- Use pagination for large datasets
- Monitor API response times in production

#### Risk 3: Timeline Delays
**Likelihood:** Medium  
**Impact:** Medium  
**Mitigation:**
- Break phases into smaller PRs
- Prioritize critical features
- Have buffer time in estimates
- Regular progress reviews

#### Risk 4: Angular Form Integration Issues
**Likelihood:** Low  
**Impact:** High  
**Mitigation:**
- Test Angular forms early in Phase 1.1
- Verify person-form.component.ts works with new API
- Add integration tests
- Manual testing of all forms

#### Risk 5: Authorization/Security Issues
**Likelihood:** Low  
**Impact:** High  
**Mitigation:**
- Security review for all new endpoints
- Use existing authorization patterns
- Test with different user roles
- Penetration testing before production

#### Risk 6: Database Performance
**Likelihood:** Low  
**Impact:** Medium  
**Mitigation:**
- Review all queries for N+1 issues
- Add database indexes where needed
- Use Entity Framework profiling
- Load testing with production-like data

---

## Implementation Guidelines

### Development Standards

1. **Naming Conventions**
   - API Controllers: `{Entity}Controller` in `Controllers/Api/`
   - MVC Controllers: `{Entity}Controller` in `Controllers/`
   - Actions: PascalCase (e.g., `GetById`, `Create`, `Update`)
   - Routes: lowercase with dashes (e.g., `/api/person`, `/api/family-event`)

2. **Error Handling**
   - Return appropriate HTTP status codes
   - 200 OK for successful GET/PUT
   - 201 Created for successful POST
   - 204 No Content for successful DELETE
   - 400 Bad Request for validation errors
   - 401 Unauthorized for auth failures
   - 403 Forbidden for authorization failures
   - 404 Not Found for missing resources
   - 500 Internal Server Error for exceptions

3. **Response Format**
   - Use consistent JSON structure
   - Include error details in validation failures
   - Use DTOs for API responses (not domain entities)
   - Include pagination metadata for lists

4. **Testing Requirements**
   - Unit tests for all controller actions
   - Integration tests for API endpoints
   - Manual testing for MVC views
   - Load testing for new APIs

5. **Documentation**
   - XML comments on all public methods
   - Swagger/OpenAPI documentation for APIs
   - Update README with new endpoints
   - Create usage examples

### Code Review Checklist

For each PR:
- [ ] Follows SOLID principles
- [ ] Has unit tests (>80% coverage)
- [ ] Passes all existing tests
- [ ] No code smells or duplications
- [ ] Proper error handling
- [ ] Authorization implemented correctly
- [ ] Input validation present
- [ ] Documentation updated
- [ ] No breaking changes (or properly handled)
- [ ] Performance acceptable

---

## Timeline Summary

| Phase | Sub-Phase | Duration | Dependencies | Critical Path | Status |
|-------|-----------|----------|--------------|---------------|--------|
| 1.1 | Person API | 2-3 days | None | ✅ Yes | ✅ Complete |
| 1.2 | Partnership & ParentChild APIs | 3-4 days | None | ✅ Yes | ✅ Complete |
| 1.3 | Household API | 2-3 days | None | No | ✅ Complete |
| 2.1 | MediaGallery MVC | 3-4 days | None | ✅ Yes | ✅ Complete |
| 2.2 | FamilyTree MVC | 4-5 days | None | No | ✅ Complete |
| 2.3 | Calendar MVC | 4-5 days | None | No | 🔲 Pending |
| 3.1 | Account Actions | 2-3 days | None | No | 🔲 Pending |
| 3.2 | Admin Controller | 3-4 days | None | No | 🔲 Pending |
| 3.3 | Help Controller | 4-5 days | None | No | 🔲 Pending |
| 4.1 | Reorganize APIs | 1 day | Phases 1-3 | No | 🔲 Pending |
| 4.2 | Static Pages | 2-3 days | None | No | 🔲 Pending |
| 4.3 | Deprecate Old Patterns | 2-3 days | Phase 1 | No | 🔲 Pending |

**Total Estimated Duration:** 6-8 weeks  
**Critical Path Duration:** 4-5 weeks  
**Phases Complete:** 5 of 12 (42%)  
**Time to Date:** ~3 weeks

---

## Next Actions

### Immediate Next Steps (Start Phase 2.3)

**Phase 2.3: Calendar MVC Controller**

1. **Create Feature Branch**
   ```bash
   git checkout -b feature/phase-2.3-calendar-mvc
   ```

2. **Analyze Existing Components**
   - Check for existing Angular calendar component
   - Review FamilyEventController API endpoints
   - Review EventRsvpController API endpoints

3. **Create CalendarController MVC**
   - File: `Controllers/CalendarController.cs`
   - Namespace: `RushtonRoots.Web.Controllers`
   - Attributes: `[Authorize]`
   - Actions: `Index` (GET /Calendar), `Create` (GET /Calendar/Create)

4. **Create Views**
   - File: `Views/Calendar/Index.cshtml`
   - File: `Views/Calendar/Create.cshtml`
   - Integrate with Angular calendar component (if exists)
   - Support multiple calendar views (month, week, day)

5. **Implement Features**
   - Display birthdays, anniversaries, events
   - Event RSVP integration
   - Event creation page

6. **Create Unit Tests**
   - File: `RushtonRoots.UnitTests/Controllers/CalendarControllerTests.cs`
   - Test all actions
   - Test authorization
   - Test ViewData setup

7. **Manual Testing**
   - Test calendar display
   - Test event creation
   - Test RSVP functionality

8. **Create PR**
   - Title: "Phase 2.3: Calendar MVC Controller"
   - Link to this plan

### Previously Completed Phases

**Phase 1: Core API Controllers** ✅ Complete
- Phase 1.1: Person API Controller ✅
- Phase 1.2: Partnership & ParentChild API Controllers ✅  
- Phase 1.3: Household API Controller ✅

**Phase 2: Media & Visualization Controllers** (3/3 complete)
- Phase 2.1: MediaGallery MVC Controller ✅
- Phase 2.2: FamilyTree MVC Controller ✅
   - Add test results

---

## Appendix

### Related Documents
- [InternalLinks.md](./InternalLinks.md) - Complete analysis of all links
- [README.md](../README.md) - Project overview
- [PATTERNS.md](../PATTERNS.md) - Architecture patterns

### Reference Materials
- ASP.NET Core Web API documentation
- Entity Framework Core best practices
- Clean Architecture guidelines
- RESTful API design principles

---

**Plan Status:** Ready for Implementation  
**Approved By:** [Pending]  
**Start Date:** [To be determined]  
**Target Completion:** [To be determined]

