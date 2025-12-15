# Database Migration Fix Plan

## Problem Statement

The current database migration setup is breaking local development due to:
1. Mixed SQLite and SQL Server migrations causing compatibility issues
2. SQLite-specific annotations (e.g., `Sqlite:Autoincrement`) in migration files
3. Conditional database provider selection in `AutofacModule.cs` based on OS platform
4. Multiple incremental migrations that should be consolidated

## Current State Analysis

### Existing Migrations (13 total)
1. `20251212145727_InitialFamilyCoreSchema.cs`
2. `20251213223753_AddIdentityAndRoles.cs` - Contains SQLite annotations
3. `20251213225709_AddPersonDateOfDeathAndPhotoUrl.cs`
4. `20251213235749_AddPhase2_2Entities.cs` - Contains SQLite annotations
5. `20251214004045_AddPhotoGalleryFeatures.cs` - Contains SQLite annotations
6. `20251214100343_AddDocumentManagement.cs`
7. `20251214102425_AddMediaManagement.cs` - Contains SQLite annotations
8. `20251214104206_AddMessagingAndNotifications.cs` - Contains SQLite annotations
9. `20251214111117_AddCollaborationTools.cs` - Contains SQLite annotations
10. `20251214112554_AddPhase4_3ContributionSystem.cs` - Contains SQLite annotations
11. `20251214114626_AddWikiEntities.cs` - Contains SQLite annotations
12. `20251214121536_AddStoryEntities.cs` - Contains SQLite annotations
13. `20251214122911_AddRecipeAndTraditionEntities.cs` - Contains SQLite annotations

### SQLite References Found

**In Code:**
- `RushtonRoots.Infrastructure/RushtonRoots.Infrastructure.csproj`: 
  - Line 14: `<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.1" />`
- `RushtonRoots.Web/AutofacModule.cs`:
  - Lines 29-31: Conditional SQLite usage for Unix platforms
  - `optionsBuilder.UseSqlite("Data Source=rushtonroots.db");`

**In Migrations:**
- Multiple `.Annotation("Sqlite:Autoincrement", true)` references
- Multiple `.OldAnnotation("Sqlite:Autoincrement", true)` references
- SQLite-specific column types (`TEXT`, `INTEGER`)
- Incompatible with SQL Server's native types

## Solution Steps

### Phase 1: Backup and Remove
1. **Delete all existing migrations**
   - Remove all files in `/RushtonRoots.Infrastructure/Migrations/` except `.gitkeep`
   - This includes all 13 migration files and the model snapshot

2. **Remove SQLite package reference**
   - Edit `RushtonRoots.Infrastructure/RushtonRoots.Infrastructure.csproj`
   - Remove line 14: `<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.1" />`

3. **Remove SQLite configuration from AutofacModule**
   - Edit `RushtonRoots.Web/AutofacModule.cs`
   - Remove the conditional logic (lines 28-36)
   - Use only SQL Server configuration

### Phase 2: Create Fresh Migration
4. **Create initial migration using EF tools**
   - Run from repository root:
   ```bash
   dotnet ef migrations add InitialCreate --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
   ```
   - This will generate a clean SQL Server-only migration

5. **Review generated migration**
   - Verify no SQLite annotations are present
   - Ensure all entities are properly included
   - Check for proper indexes and constraints

### Phase 3: Manual Index Additions (If Needed)
6. **Add custom indexes if required**
   - After reviewing the generated migration, manually edit if needed
   - Add performance-critical indexes
   - Add unique constraints as appropriate
   - Follow SQL Server best practices

### Phase 4: Testing
7. **Test the build**
   ```bash
   dotnet build
   ```

8. **Test migration application** (on a clean database)
   ```bash
   dotnet ef database update --project RushtonRoots.Infrastructure --startup-project RushtonRoots.Web
   ```

9. **Verify database schema**
   - Connect to SQL Server
   - Verify all tables created
   - Verify indexes and constraints

## Expected Outcomes

### Files to be Modified
1. `RushtonRoots.Infrastructure/RushtonRoots.Infrastructure.csproj` - Remove SQLite package
2. `RushtonRoots.Web/AutofacModule.cs` - Remove SQLite configuration
3. `RushtonRoots.Infrastructure/Migrations/` - Delete all existing migrations, add new InitialCreate

### Files to be Deleted
- All 27 migration files (13 migration classes + 13 Designer classes + 1 snapshot)

### Files to be Created
- New `InitialCreate` migration (3 files: migration, designer, snapshot)

## Benefits

1. **Simplified Configuration**: Single database provider (SQL Server only)
2. **No Platform-Specific Logic**: Remove conditional Unix/Windows database selection
3. **Clean Migration History**: Single initial migration instead of 13 incremental ones
4. **SQL Server Optimized**: Migration generated specifically for SQL Server
5. **Easier to Maintain**: No mixed database provider concerns
6. **Fixes Build Issues**: Removes incompatible SQLite annotations

## Risks and Mitigations

### Risk: Data Loss
- **Mitigation**: This is a development setup with no production data. Fresh start is appropriate.

### Risk: Missing Indexes
- **Mitigation**: Review generated migration and manually add critical indexes if needed.

### Risk: Build Failures
- **Mitigation**: Test build after each step, can rollback Git changes if needed.

## Implementation Notes

- All migrations should be created using `dotnet ef migrations add` command
- Manual editing of migrations is acceptable for indexes, constraints, and optimizations
- Never manually edit the Designer files or ModelSnapshot
- Always test migrations on a clean database before committing
- Keep the `.gitkeep` file in the Migrations directory

## Success Criteria

- [ ] All SQLite references removed from codebase
- [ ] Single InitialCreate migration exists
- [ ] Build succeeds without warnings related to migrations
- [ ] Migration can be applied to a clean SQL Server database
- [ ] All entities are properly represented in the migration
- [ ] No SQLite-specific annotations in migration files
- [ ] AutofacModule uses only SQL Server configuration
