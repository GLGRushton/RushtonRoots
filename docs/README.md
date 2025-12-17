# API Endpoints Analysis - Quick Reference

**Generated:** 2025-12-17  
**Related Documents:**
- [InternalLinks.md](./InternalLinks.md) - Complete link analysis
- [ApiEndpointsImplementationPlan.md](./ApiEndpointsImplementationPlan.md) - Phased implementation plan

---

## Executive Summary

This document provides a quick reference for missing API endpoints and controllers in the RushtonRoots application.

### Critical Findings

**Total Internal Links Analyzed:** 100+  
**Missing Controllers:** 8  
**Missing API Endpoints:** 60+  
**Deprecated Patterns:** 9

---

## Missing Endpoints by Priority

### 🔴 CRITICAL (Blocking Features)

| Endpoint | Type | Status | Impact |
|----------|------|--------|--------|
| `POST /api/person` | API | ❌ Missing | Angular form returns 404 |
| `PUT /api/person/{id}` | API | ❌ Missing | Cannot edit via Angular |
| `GET /api/person` | API | ❌ Missing | Cannot list persons via API |
| `/MediaGallery` | MVC | ❌ Missing | Linked from home page |

### 🟡 HIGH (Major Features)

| Endpoint | Type | Status | Impact |
|----------|------|--------|--------|
| `POST /api/partnership` | API | ❌ Missing | No API for partnerships |
| `POST /api/parentchild` | API | ❌ Missing | No API for parent-child |
| `/FamilyTree` | MVC | ❌ Missing | Linked from home page |
| `/Calendar` | MVC | ❌ Missing | In navigation menu |

### 🟢 MEDIUM (Nice to Have)

| Endpoint | Type | Status | Impact |
|----------|------|--------|--------|
| `/Account/Notifications` | MVC | ❌ Missing | In navigation menu |
| `/Account/Settings` | MVC | ❌ Missing | In navigation menu |
| `/Admin/Settings` | MVC | ❌ Missing | Admin feature |

### 🔵 LOW (Documentation/Static)

| Endpoint | Type | Status | Impact |
|----------|------|--------|--------|
| `/Help/*` | MVC | ❌ Missing | 10 help pages |
| `/About`, `/Contact`, etc. | MVC | ❌ Missing | Static info pages |

---

## Controllers Status Overview

### ✅ Existing Controllers

**MVC Controllers (11):**
- AccountController
- HomeController
- HouseholdController
- ParentChildController
- PartnershipController
- PersonController
- RecipeViewController
- StoryViewController
- TraditionViewController
- WikiController
- (RecipeViewController, StoryViewController, TraditionViewController)

**API Controllers (27):**
- ActivityFeedController
- ChatRoomController
- CommentController
- ConflictResolutionController
- ContributionController
- DocumentController
- EventRsvpController
- FamilyEventController
- FamilyTaskController
- FamilyTreeController
- LeaderboardController
- LifeEventController
- LocationController
- MediaController
- MessageController
- NotificationController
- PhotoAlbumController
- PhotoController
- PhotoTagController
- RecipeController
- SampleApiController
- SearchApiController
- StoryCollectionController
- StoryController
- TraditionController
- WikiControllersController
- WikiPageController

### ❌ Missing Controllers

**Need Creation:**
- MediaGalleryController (MVC)
- CalendarController (MVC)
- HelpController (MVC)
- AdminController (MVC)
- InfoController (MVC) - for static pages
- PersonController (API)
- PartnershipController (API)
- ParentChildController (API)
- HouseholdController (API)

---

## Deprecated Patterns to Remove

### MVC POST Actions → API Endpoints

**Should be migrated:**

```
POST /Person/Create       → POST /api/person
POST /Person/Edit         → PUT /api/person/{id}
POST /Person/Delete       → DELETE /api/person/{id}

POST /Partnership/Create  → POST /api/partnership
POST /Partnership/Edit    → PUT /api/partnership/{id}
POST /Partnership/Delete  → DELETE /api/partnership/{id}

POST /ParentChild/Create  → POST /api/parentchild
POST /ParentChild/Edit    → PUT /api/parentchild/{id}
POST /ParentChild/Delete  → DELETE /api/parentchild/{id}
```

**Reasoning:** API-first architecture, better separation of concerns, supports SPA development

---

## Implementation Phases Summary

### Phase 1: Core API Controllers (2 weeks)
- **1.1:** Person API Controller (2-3 days) ← START HERE
- **1.2:** Partnership & ParentChild APIs (3-4 days)
- **1.3:** Household API Controller (2-3 days)

### Phase 2: Media & Visualization (2 weeks)
- **2.1:** MediaGallery MVC Controller (3-4 days)
- **2.2:** FamilyTree MVC Controller (4-5 days)
- **2.3:** Calendar MVC Controller (4-5 days)

### Phase 3: User Experience (2 weeks)
- **3.1:** Account Additional Actions (2-3 days)
- **3.2:** Admin Controller (3-4 days)
- **3.3:** Help Controller (4-5 days)

### Phase 4: Cleanup & Organization (2 weeks)
- **4.1:** Reorganize APIs to Controllers/Api/ (1 day)
- **4.2:** Static/Info Pages (2-3 days)
- **4.3:** Deprecate Old MVC POST (2-3 days)

**Total Duration:** 6-8 weeks

---

## Quick Start Guide

### To Fix Immediate 404 Error (Person API)

1. **Create API Controller:**
   ```bash
   mkdir -p RushtonRoots.Web/Controllers/Api
   touch RushtonRoots.Web/Controllers/Api/PersonController.cs
   ```

2. **Implement Endpoints:**
   ```csharp
   [ApiController]
   [Route("api/person")]
   public class PersonController : ControllerBase
   {
       [HttpGet]
       public async Task<IActionResult> GetAll() { }
       
       [HttpGet("{id}")]
       public async Task<IActionResult> GetById(int id) { }
       
       [HttpPost]
       public async Task<IActionResult> Create([FromBody] CreatePersonRequest request) { }
       
       [HttpPut("{id}")]
       public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonRequest request) { }
       
       [HttpDelete("{id}")]
       public async Task<IActionResult> Delete(int id) { }
   }
   ```

3. **Test with Angular:**
   - Run application
   - Navigate to person form
   - Verify POST /api/person works
   - Check response codes

### To Fix MediaGallery 404

1. **Create MVC Controller:**
   ```bash
   touch RushtonRoots.Web/Controllers/MediaGalleryController.cs
   ```

2. **Implement Actions:**
   ```csharp
   public class MediaGalleryController : Controller
   {
       [HttpGet]
       public async Task<IActionResult> Index() 
       {
           // Fetch media from MediaController API
           return View();
       }
       
       [HttpGet]
       public IActionResult Upload() 
       {
           return View();
       }
   }
   ```

3. **Create Views:**
   ```bash
   mkdir -p RushtonRoots.Web/Views/MediaGallery
   touch RushtonRoots.Web/Views/MediaGallery/Index.cshtml
   touch RushtonRoots.Web/Views/MediaGallery/Upload.cshtml
   ```

---

## Code Organization Recommendation

### Current Structure
```
Controllers/
├── PersonController.cs (MVC)
├── PartnershipController.cs (MVC)
├── MediaController.cs (API)
├── RecipeController.cs (API)
└── ... (mixed MVC and API)
```

### Recommended Structure
```
Controllers/
├── Api/
│   ├── PersonController.cs
│   ├── PartnershipController.cs
│   ├── MediaController.cs
│   ├── RecipeController.cs
│   └── ... (all API controllers)
├── PersonController.cs (MVC - views only)
├── MediaGalleryController.cs (MVC)
├── CalendarController.cs (MVC)
└── ... (MVC controllers)
```

**Benefits:**
- Clear separation of concerns
- Easier to navigate codebase
- Follows ASP.NET Core conventions
- Better for API versioning

---

## Testing Checklist

### For Each New API Endpoint

- [ ] Unit tests created (>80% coverage)
- [ ] Integration tests pass
- [ ] Postman/Swagger testing completed
- [ ] Authorization tested (different roles)
- [ ] Error handling tested
- [ ] Validation tested
- [ ] Performance acceptable (<200ms)

### For Each New MVC Controller

- [ ] Views render correctly
- [ ] Navigation links work
- [ ] Forms submit correctly
- [ ] Responsive on mobile
- [ ] Browser compatibility tested
- [ ] Accessibility checked

---

## Breaking Changes Warning

⚠️ **Phase 4.3** will deprecate MVC POST patterns

**Timeline:**
1. Sprint 1: Add [Obsolete] warnings
2. Sprint 2-3: Monitor usage, update documentation
3. Sprint 4: Remove deprecated endpoints

**Preparation:**
- Ensure all Angular forms use APIs before Phase 4.3
- Update all client code references
- Communicate changes to team

---

## Useful Commands

### Find all internal links
```bash
grep -r "href=\|window.location\|asp-action" \
  RushtonRoots.Web/Views \
  RushtonRoots.Web/ClientApp/src \
  --include="*.cshtml" --include="*.ts"
```

### Find API routes
```bash
grep -r "\[Route\]\|\[Http" \
  RushtonRoots.Web/Controllers \
  --include="*.cs"
```

### List all controllers
```bash
ls -1 RushtonRoots.Web/Controllers/*.cs | \
  xargs -I {} basename {} .cs
```

---

## Resources

### Documentation
- [InternalLinks.md](./InternalLinks.md) - Complete analysis (576 lines)
- [ApiEndpointsImplementationPlan.md](./ApiEndpointsImplementationPlan.md) - Full plan (985 lines)

### Code Patterns
- [PATTERNS.md](../PATTERNS.md) - Architecture patterns
- [README.md](../README.md) - Project overview

### ASP.NET Core
- [Web API documentation](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [RESTful API guidelines](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## Contact & Questions

For questions about this analysis or implementation plan:
1. Review the detailed documents in this directory
2. Check existing controller patterns in the codebase
3. Refer to PATTERNS.md for architecture guidelines
4. Open an issue for clarification

---

**Document Version:** 1.0  
**Last Updated:** 2025-12-17  
**Status:** Ready for Implementation
