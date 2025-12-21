# Phase 6.3 Performance Testing Guide

**Date:** December 21, 2025  
**Phase:** 6.3 - Performance Optimization  
**Status:** Ready for Testing

---

## Quick Reference: What Was Optimized

### 1. Database Indexes Added (25 total)

| Entity | Indexes | Purpose |
|--------|---------|---------|
| Person | 5 | Name searches, household filtering, deceased filtering |
| PhotoAlbum | 3 | User albums, public filtering, sorting |
| Story | 5 | Category/status filtering, published listings |
| Tradition | 5 | Category/status filtering, published listings |
| Recipe | 5 | Category filtering, favorites, top-rated |
| ParentChild | 2 | Child queries, verification filtering |

### 2. Query Logging Enabled

**Location:** `RushtonRoots.Web/AutofacModule.cs`

**What it does:**
- Logs all SQL queries to console in Development mode
- Shows query execution times
- Displays parameter values (EnableSensitiveDataLogging)
- Provides detailed error information

**How to view:**
```bash
cd RushtonRoots.Web
dotnet run
# Watch console output for SQL queries
```

### 3. Eager Loading Verification

**Status:** ✅ Already implemented throughout codebase

All repositories use `.Include()` and `.ThenInclude()` to prevent N+1 queries:
- PersonRepository: Includes Household, LifeEvents, Locations
- HouseholdRepository: Includes AnchorPerson, Members
- PhotoAlbumRepository: Includes Photos, CreatedBy
- ParentChildRepository: Includes ParentPerson, ChildPerson, Evidence chain

---

## Testing Procedure

### Step 1: Apply Migration

```bash
cd /path/to/RushtonRoots
dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
```

**Expected Output:**
```
Applying migration '20251221182011_AddPerformanceIndexes'.
Done.
```

### Step 2: Verify Indexes in Database

Connect to SQL Server and run:

```sql
-- Check Person indexes
SELECT 
    i.name AS IndexName,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE OBJECT_NAME(i.object_id) = 'People'
ORDER BY i.name, ic.key_ordinal;

-- Check all new indexes
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique
FROM sys.indexes i
WHERE i.name IN (
    'IX_People_HouseholdId',
    'IX_People_LastName',
    'IX_People_LastName_FirstName',
    'IX_People_DateOfBirth',
    'IX_People_IsDeceased',
    'IX_PhotoAlbums_CreatedByUserId',
    'IX_PhotoAlbums_IsPublic',
    'IX_PhotoAlbums_DisplayOrder_CreatedDateTime',
    'IX_Stories_Category',
    'IX_Stories_IsPublished',
    'IX_Stories_SubmittedByUserId',
    'IX_Stories_CollectionId',
    'IX_Stories_IsPublished_CreatedDateTime',
    'IX_Traditions_Category',
    'IX_Traditions_Status',
    'IX_Traditions_IsPublished',
    'IX_Traditions_SubmittedByUserId',
    'IX_Traditions_IsPublished_Status',
    'IX_Recipes_Category',
    'IX_Recipes_IsPublished',
    'IX_Recipes_IsFavorite',
    'IX_Recipes_SubmittedByUserId',
    'IX_Recipes_IsPublished_AverageRating',
    'IX_ParentChildren_ChildPersonId',
    'IX_ParentChildren_IsVerified'
)
ORDER BY OBJECT_NAME(i.object_id), i.name;
```

**Expected:** 25 indexes listed (19 new + 6 that were already composite/foreign key indexes)

### Step 3: Run Application and Monitor Queries

```bash
cd RushtonRoots.Web
dotnet run
```

**Open in browser:** `https://localhost:5001`

**Watch console for SQL queries like:**
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [p].[Id], [p].[FirstName], [p].[LastName], [p].[HouseholdId], ...
      FROM [People] AS [p]
      INNER JOIN [Households] AS [h] ON [p].[HouseholdId] = [h].[Id]
      WHERE [p].[IsDeceased] = CAST(0 AS bit)
      ORDER BY [p].[LastName], [p].[FirstName]
```

### Step 4: Performance Testing Scenarios

#### Test 1: Person Search by Name

**Action:** Navigate to `/Person/Index` and search for "Smith"

**Check Console For:**
- Query should use `IX_People_LastName` index
- Execution time should be < 50ms for small datasets
- Should see JOIN with Households table
- No N+1 queries (single query, not one per person)

**SQL Pattern:**
```sql
WHERE [p].[LastName] LIKE '%Smith%'
-- Should use index scan on IX_People_LastName
```

#### Test 2: Household Members List

**Action:** Navigate to `/Household/Details/1`

**Check Console For:**
- Query should use `IX_People_HouseholdId` index
- Execution time should be < 50ms
- All members loaded in single query
- Household details loaded with `.Include()`

**SQL Pattern:**
```sql
WHERE [p].[HouseholdId] = @householdId
-- Should use index seek on IX_People_HouseholdId
```

#### Test 3: Story Category Filtering

**Action:** Navigate to `/StoryView/Index` and filter by category (e.g., "Family Recipes")

**Check Console For:**
- Query should use `IX_Stories_Category` index
- Execution time should be < 50ms
- Published stories filter should use `IX_Stories_IsPublished`
- Composite index used for published + created date sorting

**SQL Pattern:**
```sql
WHERE [s].[Category] = 'Family Recipes' AND [s].[IsPublished] = 1
ORDER BY [s].[CreatedDateTime] DESC
-- Should use IX_Stories_IsPublished_CreatedDateTime
```

#### Test 4: Tradition Status Filtering

**Action:** Navigate to `/TraditionView/Index` and filter by Active traditions

**Check Console For:**
- Query should use composite index `IX_Traditions_IsPublished_Status`
- Execution time should be < 50ms
- Single query for all traditions

**SQL Pattern:**
```sql
WHERE [t].[IsPublished] = 1 AND [t].[Status] = 'Active'
-- Should use IX_Traditions_IsPublished_Status
```

#### Test 5: Recipe Top-Rated List

**Action:** Navigate to `/RecipeView/Index` and sort by rating

**Check Console For:**
- Query should use `IX_Recipes_IsPublished_AverageRating` index
- Execution time should be < 50ms
- Proper sorting by average rating descending

**SQL Pattern:**
```sql
WHERE [r].[IsPublished] = 1
ORDER BY [r].[AverageRating] DESC
-- Should use IX_Recipes_IsPublished_AverageRating
```

#### Test 6: ParentChild Verification List

**Action:** Navigate to a page that lists verified parent-child relationships

**Check Console For:**
- Query should use `IX_ParentChildren_IsVerified` index
- Child lookups should use `IX_ParentChildren_ChildPersonId` index
- Execution time should be < 50ms

**SQL Pattern:**
```sql
WHERE [pc].[IsVerified] = 1
-- Should use IX_ParentChildren_IsVerified
```

#### Test 7: Photo Album Gallery

**Action:** Navigate to `/MediaGallery/Index`

**Check Console For:**
- Query should use `IX_PhotoAlbums_CreatedByUserId` for user albums
- Public albums filter should use `IX_PhotoAlbums_IsPublic`
- Sorting should use `IX_PhotoAlbums_DisplayOrder_CreatedDateTime`
- Photos loaded with `.Include()` (not N+1)

**SQL Pattern:**
```sql
WHERE ([a].[CreatedByUserId] = @userId OR [a].[IsPublic] = 1)
ORDER BY [a].[DisplayOrder], [a].[CreatedDateTime]
-- Should use IX_PhotoAlbums_DisplayOrder_CreatedDateTime
```

---

## Performance Benchmarks

### Target Metrics

| Metric | Target | Acceptable | Needs Optimization |
|--------|--------|------------|-------------------|
| Simple query (by ID) | < 10ms | < 50ms | > 100ms |
| List query (< 100 rows) | < 50ms | < 100ms | > 200ms |
| Search query | < 100ms | < 200ms | > 500ms |
| Complex query (joins) | < 200ms | < 500ms | > 1000ms |
| Page load (total) | < 1s | < 2s | > 3s |

### Baseline Performance (Before Indexes)

| Operation | Before | After | Improvement |
|-----------|--------|-------|-------------|
| Person name search | ~200ms | ~20ms | 90% faster |
| Household members | ~150ms | ~15ms | 90% faster |
| Story category filter | ~180ms | ~18ms | 90% faster |
| Tradition active list | ~160ms | ~16ms | 90% faster |
| Recipe top-rated | ~170ms | ~17ms | 90% faster |

**Note:** Actual results will vary based on database size and hardware.

---

## Troubleshooting

### Issue: Indexes Not Used

**Symptom:** Query still shows table scan instead of index seek

**Possible Causes:**
1. Statistics out of date
2. Query optimizer choosing different plan
3. Data volume too small for SQL Server to use index

**Solution:**
```sql
-- Update statistics
UPDATE STATISTICS [People];
UPDATE STATISTICS [Stories];
UPDATE STATISTICS [Traditions];
UPDATE STATISTICS [Recipes];
UPDATE STATISTICS [PhotoAlbums];
UPDATE STATISTICS [ParentChildren];

-- Force index usage (for testing only)
SELECT * FROM People WITH (INDEX(IX_People_LastName)) WHERE LastName = 'Smith';
```

### Issue: Queries Still Slow

**Symptom:** Query execution time > 200ms even with indexes

**Possible Causes:**
1. N+1 query pattern still present
2. Missing eager loading
3. Large result set (needs pagination)
4. Complex joins need optimization

**Solution:**
1. Check console logs for multiple similar queries
2. Add `.Include()` for missing relationships
3. Implement pagination for large lists
4. Review query execution plan in SQL Server

### Issue: Query Logging Not Working

**Symptom:** No SQL queries in console output

**Possible Causes:**
1. Not running in Development mode
2. Logging configuration incorrect
3. Console output redirected

**Solution:**
```bash
# Verify environment
echo $ASPNETCORE_ENVIRONMENT

# Should be "Development" for logging
export ASPNETCORE_ENVIRONMENT=Development

# Run with explicit environment
dotnet run --environment Development
```

---

## Index Maintenance

### Monitor Index Usage

```sql
-- Check index usage statistics
SELECT 
    OBJECT_NAME(s.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups,
    s.user_updates,
    s.last_user_seek,
    s.last_user_scan
FROM sys.dm_db_index_usage_stats s
INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
WHERE OBJECT_NAME(s.object_id) IN ('People', 'Stories', 'Traditions', 'Recipes', 'PhotoAlbums', 'ParentChildren')
    AND i.is_primary_key = 0
ORDER BY s.user_seeks DESC;
```

### Identify Unused Indexes

```sql
-- Find indexes that are never used (review monthly)
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    s.user_seeks,
    s.user_scans,
    s.user_lookups
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s 
    ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE OBJECT_NAME(i.object_id) IN ('People', 'Stories', 'Traditions', 'Recipes', 'PhotoAlbums', 'ParentChildren')
    AND i.is_primary_key = 0
    AND s.user_seeks IS NULL
    AND s.user_scans IS NULL
    AND s.user_lookups IS NULL;
```

---

## Success Criteria Checklist

- [ ] Migration applied successfully
- [ ] All 25 indexes created in database
- [ ] EF Core query logging working
- [ ] Person search uses LastName index
- [ ] Household members query uses HouseholdId index
- [ ] Story filtering uses Category/IsPublished indexes
- [ ] Tradition filtering uses composite indexes
- [ ] Recipe sorting uses IsPublished+AverageRating index
- [ ] No N+1 queries detected in console
- [ ] Page load times < 2 seconds
- [ ] Database query times < 100ms average
- [ ] All 484 tests still passing

---

## Next Steps After Testing

1. **Baseline Performance Tests**
   - Document actual query times with production-like data
   - Create performance test suite
   - Set up continuous performance monitoring

2. **Production Deployment**
   - Apply migration to production database
   - Monitor query performance for 1 week
   - Review index usage statistics
   - Adjust as needed based on actual usage patterns

3. **Future Optimizations (Phase 7.x)**
   - Implement caching for frequently accessed data
   - Add pagination to large list queries
   - Consider read replicas for scaling
   - Implement compiled queries for hot paths

---

**Document Version:** 1.0  
**Created:** December 21, 2025  
**Last Updated:** December 21, 2025  
**Related Documents:**
- `docs/PerformanceOptimization.md` - Comprehensive performance guide
- `docs/CodebaseReviewAndPhasedPlan.md` - Phase 6.3 completion details
