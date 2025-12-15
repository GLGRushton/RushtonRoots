# Person Module - Phase 3.1 Implementation

This module contains the Angular components for the Person Index & Search feature, implementing Phase 3.1 of the UI Design Plan.

## Components

### 1. PersonIndexComponent
**Selector**: `app-person-index`

Main container component that integrates search and table functionality.

**Inputs**:
- `initialPeople: PersonTableRow[]` - Array of people to display
- `households: HouseholdOption[]` - Array of households for filtering
- `canEdit: boolean` - Whether user can edit people
- `canDelete: boolean` - Whether user can delete people
- `initialFilters?: PersonSearchFilters` - Initial filter values

**Features**:
- Client-side filtering (extensible to server-side)
- CSV export functionality
- Loading and error states
- Permission-based action visibility

**Usage in Razor Views**:
```html
<app-person-index 
    initial-people='@Html.Raw(peopleJson)'
    households='@Html.Raw(householdsJson)'
    initial-filters='@Html.Raw(filtersJson)'
    can-edit="true"
    can-delete="true">
</app-person-index>
```

### 2. PersonSearchComponent
**Selector**: `app-person-search`

Advanced search interface with filters and chips.

**Inputs**:
- `households: HouseholdOption[]` - Available households for filtering
- `initialFilters?: PersonSearchFilters` - Initial filter values

**Outputs**:
- `search: EventEmitter<PersonSearchFilters>` - Emits when search is triggered
- `filtersChanged: EventEmitter<PersonSearchFilters>` - Emits when filters change

**Features**:
- Text search with debouncing (400ms)
- Household dropdown filter
- Deceased status filter
- Date range filters (birth/death) with Material Datepicker
- Surname filter
- Active filter chips with remove capability
- Collapsible advanced filters section
- Clear all filters button

**Filter Types**:
```typescript
interface PersonSearchFilters {
  searchTerm?: string;
  householdId?: number;
  isDeceased?: boolean;
  birthDateFrom?: Date;
  birthDateTo?: Date;
  deathDateFrom?: Date;
  deathDateTo?: Date;
  surname?: string;
}
```

### 3. PersonTableComponent
**Selector**: `app-person-table`

Data table with sorting, pagination, and responsive design.

**Inputs**:
- `people: PersonTableRow[]` - Array of people to display
- `showPagination: boolean` - Enable/disable pagination (default: true)
- `pageSize: number` - Items per page (default: 10)
- `pageSizeOptions: number[]` - Available page sizes (default: [5, 10, 25, 50, 100])
- `showActions: boolean` - Show action buttons (default: true)
- `showSelection: boolean` - Enable row selection (default: false)
- `canEdit: boolean` - Show edit button (default: false)
- `canDelete: boolean` - Show delete button (default: false)

**Outputs**:
- `actionTriggered: EventEmitter<PersonAction>` - Emits when action button clicked
- `pageChanged: EventEmitter<PageEvent>` - Emits on page change
- `sortChanged: EventEmitter<Sort>` - Emits on sort change
- `selectionChanged: EventEmitter<PersonTableRow[]>` - Emits when selection changes

**Features**:
- Material table with sorting and pagination
- Desktop table view with avatars/placeholders
- Responsive mobile card view (automatic switch at 768px)
- Quick action buttons (view, edit, delete)
- CSV export functionality
- Row selection support (optional)
- No data state handling
- Avatar images with fallback to initials

**Row Format**:
```typescript
interface PersonTableRow {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  householdName: string;
  dateOfBirth?: Date | string;
  dateOfDeath?: Date | string;
  isDeceased: boolean;
  photoUrl?: string;
}
```

## Responsive Design

All components are mobile-responsive:

- **Desktop (>768px)**: Full table view with all columns
- **Mobile (≤768px)**: Card view with essential information

## Material Design Components Used

- MatCard
- MatFormField, MatInput, MatSelect
- MatButton, MatIconButton
- MatTable, MatPaginator, MatSort
- MatChip (with removal)
- MatCheckbox
- MatTooltip
- MatDatepicker
- MatDivider
- MatProgressSpinner
- MatIcon

## Integration Example

See `Views/Person/Index-Angular.cshtml` for a complete integration example with Razor views.

### Data Transformation

Transform C# ViewModels to JavaScript objects:

```csharp
var peopleData = Model.Select(p => new
{
    id = p.Id,
    firstName = p.FirstName,
    lastName = p.LastName,
    fullName = p.FullName,
    householdName = p.HouseholdName,
    dateOfBirth = p.DateOfBirth,
    dateOfDeath = p.DateOfDeath,
    isDeceased = p.IsDeceased,
    photoUrl = p.PhotoUrl
}).ToArray();

var peopleJson = JsonSerializer.Serialize(peopleData);
```

## Future Enhancements

- Server-side filtering and pagination
- PDF export functionality
- Bulk operations (delete multiple)
- Advanced sorting options
- Column visibility toggle
- Save filter presets
- Quick search shortcuts

## Testing

Components can be tested individually:

```typescript
import { PersonIndexComponent } from './person-index.component';
import { PersonTableComponent } from './person-table.component';
import { PersonSearchComponent } from './person-search.component';
```

## Performance Notes

- Search debouncing reduces API calls
- Client-side filtering is fast for <1000 records
- For larger datasets, implement server-side filtering
- Virtual scrolling can be added for very large lists
- Lazy loading images for better performance
