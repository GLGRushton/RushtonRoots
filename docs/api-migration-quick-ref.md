# API Migration - Quick Reference Card

**Status**: Planning Phase  
**Target**: Q1 2026

---

## Controllers to Migrate

| # | Controller | Old Pattern | New Pattern | Status |
|---|------------|-------------|-------------|--------|
| 1 | Person | `/Person/*` | `/api/person` | ⏳ Planned |
| 2 | Household | `/Household/*` | `/api/household` | ⏳ Planned |
| 3 | Partnership | `/Partnership/*` | `/api/partnership` | ⏳ Planned |
| 4 | ParentChild | `/ParentChild/*` | `/api/parentchild` | ⏳ Planned |

---

## Endpoint Mapping

### Person

```
GET  /Person              → GET    /api/person
GET  /Person/Details/5    → GET    /api/person/5
POST /Person/Create       → POST   /api/person
POST /Person/Edit/5       → PUT    /api/person/5
POST /Person/Delete/5     → DELETE /api/person/5
```

### Household

```
GET  /Household             → GET    /api/household
GET  /Household/Details/5   → GET    /api/household/5
GET  /Household/Members/5   → GET    /api/household/5/members
POST /Household/Create      → POST   /api/household
POST /Household/Edit/5      → PUT    /api/household/5
POST /Household/Delete/5    → DELETE /api/household/5
```

### Partnership

```
GET  /Partnership           → GET    /api/partnership
GET  /Partnership/Details/5 → GET    /api/partnership/5
POST /Partnership/Create    → POST   /api/partnership
POST /Partnership/Edit/5    → PUT    /api/partnership/5
POST /Partnership/Delete/5  → DELETE /api/partnership/5
```

### ParentChild

```
GET  /ParentChild           → GET    /api/parentchild
GET  /ParentChild/Details/5 → GET    /api/parentchild/5
POST /ParentChild/Create    → POST   /api/parentchild
POST /ParentChild/Edit/5    → PUT    /api/parentchild/5
POST /ParentChild/Delete/5  → DELETE /api/parentchild/5
```

---

## Migration Steps (Per Controller)

1. **Create API Controller** (4-6 hrs)
   - Add `*ApiController.cs`
   - Implement REST endpoints
   - Keep old controller

2. **Update Angular** (5-8 hrs)
   - Create/update service
   - Update components
   - Switch to API calls

3. **Add Routing** (3-4 hrs)
   - Add routes in `app-routing.module.ts`
   - Enable SPA navigation

4. **Deprecate Old** (2 hrs)
   - Mark `[Obsolete]`
   - Add logging
   - Monitor 2+ weeks

5. **Remove Old** (2 hrs)
   - Delete controller
   - Delete views
   - Run tests

---

## Timeline

| Approach | Duration |
|----------|----------|
| Sequential (safer) | 11-15 weeks |
| Parallel (faster) | 6-10 weeks |

**Recommended**: Start with Person sequentially, then parallelize the rest.

---

## Key Files

### Backend
- `Controllers/PersonApiController.cs` (create)
- `Controllers/PersonController.cs` (deprecate → delete)
- `Views/Person/*.cshtml` (delete)

### Frontend
- `app/person/services/person.service.ts` (create/update)
- `app/person/components/*.ts` (update)
- `app/app-routing.module.ts` (update)

---

## Testing Checklist

- [ ] Unit tests for API controller
- [ ] Integration tests with database
- [ ] Angular component tests
- [ ] E2E tests for workflows
- [ ] Performance/load tests
- [ ] Manual UI testing
- [ ] Accessibility testing

---

## Success Metrics

| Metric | Target |
|--------|--------|
| Code Coverage | ≥ 80% |
| API Response Time (p95) | < 500ms |
| Page Load Time | < 2s |
| Zero 404 Errors | ✅ |
| User-Reported Bugs | < 5 critical |

---

## Common Commands

### Backend

```bash
# Create migration
dotnet ef migrations add AddPersonApi --project RushtonRoots.Infrastructure

# Run tests
dotnet test

# Build
dotnet build
```

### Frontend

```bash
# Install dependencies
npm install

# Run Angular dev server
npm start

# Run tests
npm test

# Build for production
npm run build -- --configuration production
```

---

## Rollback

If something goes wrong:

1. **Quick Rollback** (during Steps 1-3):
   ```bash
   git revert <commit-hash>
   git push
   ```
   Old endpoints still work ✅

2. **Full Rollback** (after Step 5):
   ```bash
   git checkout <previous-tag>
   # Restore controller from git history
   # Redeploy
   ```

---

## Links

- [Full Plan](./api-migration.md) - Complete migration plan
- [Summary](./api-migration-summary.md) - Executive summary
- [Endpoint Map](./api-migration.md#appendix-a-endpoint-mapping-reference) - Detailed mapping
- [Angular Routes](./api-migration.md#appendix-b-angular-routing-reference) - Route examples

---

**Last Updated**: December 2025  
**Version**: 1.0
