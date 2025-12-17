# API Migration Plan - Executive Summary

## Quick Overview

This document provides a quick reference for the API migration plan. For full details, see [api-migration.md](./api-migration.md).

---

## What We're Doing

**Goal**: Migrate 4 legacy MVC controllers to modern REST API endpoints.

**Controllers to Migrate**:
1. PersonController → `/api/person`
2. HouseholdController → `/api/household`
3. PartnershipController → `/api/partnership`
4. ParentChildController → `/api/parentchild`

**Already Done**: 28 controllers already use the `/api/[controller]` pattern ✅

---

## Why We're Doing This

- **Consistency**: All APIs will follow the same pattern
- **Modern Architecture**: RESTful design with proper HTTP verbs
- **Better Documentation**: Auto-generate Swagger/OpenAPI docs
- **Improved Developer Experience**: Predictable API patterns
- **Reduced Technical Debt**: Remove old MVC endpoints
- **Future-Ready**: Enable API versioning

---

## Migration Strategy

Each controller migration follows the same 5-step pattern:

### Step 1: Create New API Controller
- Create `*ApiController.cs` with REST endpoints
- Keep old MVC controller intact (no breaking changes)
- Estimated: 4-6 hours per controller

### Step 2: Update Angular Components
- Update components to use new API endpoints
- Create Angular services (if needed)
- Update HTTP calls
- Estimated: 5-8 hours per controller

### Step 3: Set Up Angular Routing
- Add routes in `app-routing.module.ts`
- Enable SPA navigation (no page reloads)
- Estimated: 3-4 hours per controller

### Step 4: Deprecate Old Endpoints
- Mark old endpoints as `[Obsolete]`
- Add logging to track usage
- Monitor for 2+ weeks
- Estimated: 2 hours per controller

### Step 5: Remove Old Endpoints
- Delete old MVC controller
- Delete Razor views
- Full regression testing
- Estimated: 2 hours per controller

---

## Timeline

**Per Controller**: 16-22 hours (2-3 weeks at part-time pace)

**Total Effort**:
- 4 controllers × 17-22 hours = **68-88 hours**
- Final cleanup phase = **21-30 hours**
- **Grand Total: 89-118 hours** (~11-15 weeks at 8 hrs/week)

**Recommended Approach**:
- Start with Person (establish pattern)
- Parallelize Household, Partnership, ParentChild
- **Timeline: 6-10 weeks** with parallel work

---

## Migration Phases Overview

| Phase | Controller | Effort | Can Run in Parallel? |
|-------|-----------|--------|---------------------|
| Phase 1 | Person | 17-22 hrs | Start first |
| Phase 2 | Household | 17-22 hrs | ✅ Yes (after Phase 1.1 complete) |
| Phase 3 | Partnership | 16-21 hrs | ✅ Yes (after Phase 1.1 complete) |
| Phase 4 | ParentChild | 16-21 hrs | ✅ Yes (after Phase 1.1 complete) |
| Phase 5 | Cleanup & Docs | 21-30 hrs | ❌ No (requires all above complete) |

---

## Key Benefits

### For Developers
- Consistent API patterns (easy to learn and use)
- Auto-generated Swagger documentation
- Better TypeScript type safety
- Easier testing

### For Users
- Faster page navigation (SPA, no reloads)
- Better performance
- More responsive UI

### For the Application
- Reduced technical debt
- Easier to maintain
- Future-ready for API versioning
- Better monitoring and logging

---

## Risk Mitigation

### How We're Staying Safe

1. **No Breaking Changes**: Old endpoints work during migration
2. **Phased Approach**: One controller at a time
3. **Comprehensive Testing**: Unit, integration, E2E tests
4. **Rollback Plan**: Can revert at any point
5. **Monitoring**: Track usage of deprecated endpoints
6. **Staging First**: Deploy to staging before production

### What Could Go Wrong?

| Risk | Mitigation |
|------|------------|
| Data loss | Database backups before each phase |
| Performance issues | Load testing before deployment |
| User confusion | Clear communication and documentation |
| Timeline overrun | Buffer time built into estimates |

---

## Success Criteria

### We'll Know We're Done When:

- ✅ All 4 controllers migrated to `/api/[controller]`
- ✅ All Angular components use new API endpoints
- ✅ Swagger documentation auto-generates
- ✅ Zero 404 errors on old endpoints
- ✅ All automated tests pass
- ✅ Performance metrics are stable or improved
- ✅ Old MVC controllers and views removed

---

## Next Steps

1. **Review this plan** with the team
2. **Approve and schedule** Phase 1 (Person)
3. **Set up testing environment** and CI/CD for API testing
4. **Begin Phase 1.1**: Create PersonApiController
5. **Monitor and iterate** based on lessons learned

---

## Quick Reference Links

- [Full Migration Plan](./api-migration.md)
- [Endpoint Mapping Reference](./api-migration.md#appendix-a-endpoint-mapping-reference)
- [Angular Routing Reference](./api-migration.md#appendix-b-angular-routing-reference)
- [Testing Strategy](./api-migration.md#testing-strategy)
- [Rollback Plan](./api-migration.md#rollback-plan)

---

**Questions?** See the full [api-migration.md](./api-migration.md) document for detailed information.

**Document Version**: 1.0  
**Last Updated**: December 2025
