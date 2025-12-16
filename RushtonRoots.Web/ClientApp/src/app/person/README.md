# Person Module - Phase 3.1 & 3.2 Implementation

This module contains the Angular components for Person management, implementing Phase 3.1 and 3.2 of the UI Design Plan.

## Phase 3.1: Person Index & Search (✅ Complete)

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
import { PersonDetailsComponent } from './person-details.component';
import { PersonTimelineComponent } from './person-timeline.component';
import { RelationshipVisualizerComponent } from './relationship-visualizer.component';
import { PhotoGalleryComponent } from './photo-gallery.component';
```

---

## Phase 3.2: Person Details & Timeline (✅ Complete)

### 4. PersonDetailsComponent
**Selector**: `app-person-details`

Main component for displaying comprehensive person information with a tabbed interface.

**Inputs**:
- `person: PersonDetails` - Complete person data object
- `timelineEvents: TimelineEvent[]` - Array of timeline events
- `relationships: PersonRelationship[]` - Array of family relationships
- `photos: PersonPhoto[]` - Array of person photos
- `canEdit: boolean` - Permission to edit person data
- `canDelete: boolean` - Permission to delete person
- `initialTab: number` - Initial tab index (default: 0)

**Outputs**:
- `editClicked: EventEmitter<number>` - Emits person ID when edit clicked
- `deleteClicked: EventEmitter<number>` - Emits person ID when delete clicked
- `shareClicked: EventEmitter<number>` - Emits person ID when share clicked
- `relationshipPersonClicked: EventEmitter<number>` - Emits when relationship card clicked
- `photoUploaded: EventEmitter<File>` - Emits file when photo uploaded
- `photoDeleted: EventEmitter<number>` - Emits photo ID when deleted
- `photoPrimaryChanged: EventEmitter<number>` - Emits photo ID when set as primary
- `fieldUpdated: EventEmitter<{field: string, value: any}>` - Emits field updates

**Features**:
- Tabbed interface (Overview, Timeline, Relationships, Photos)
- Person header with photo and vital information
- Edit-in-place functionality for biography
- Action menu with edit, delete, and share
- Responsive design for mobile and desktop
- Integration with all Phase 3.2 sub-components

**Tab Structure**:
1. **Overview**: Biography, education, occupation, notes with edit-in-place
2. **Timeline**: Life events in chronological order
3. **Relationships**: Parents, spouses, children, siblings
4. **Photos**: Photo gallery with upload and management

### 5. PersonTimelineComponent
**Selector**: `app-person-timeline`

Vertical timeline displaying significant life events.

**Inputs**:
- `personId: number` - Person identifier
- `events: TimelineEvent[]` - Array of timeline events
- `dateOfBirth?: Date | string` - Person's birth date
- `dateOfDeath?: Date | string` - Person's death date
- `isDeceased: boolean` - Whether person is deceased

**Features**:
- Vertical timeline with event markers
- Auto-populated birth/death events
- Event type icons and color coding
- Age calculation at each event
- Chronological sorting
- Responsive design
- Event types: birth, death, marriage, education, career, milestone, other

**Event Format**:
```typescript
interface TimelineEvent {
  id: number;
  personId: number;
  title: string;
  date: Date | string;
  description?: string;
  eventType: 'birth' | 'death' | 'marriage' | 'education' | 'career' | 'milestone' | 'other';
  icon?: string;
  location?: string;
}
```

### 6. RelationshipVisualizerComponent
**Selector**: `app-relationship-visualizer`

Displays family relationships in organized sections.

**Inputs**:
- `personId: number` - Person identifier
- `personName: string` - Person's full name
- `relationships: PersonRelationship[]` - Array of relationships

**Outputs**:
- `personClicked: EventEmitter<number>` - Emits person ID when card clicked

**Features**:
- Organized sections for parents, spouses, children, siblings
- Clickable person cards for navigation
- Photo avatars with fallback images
- Life span display (birth year - death year)
- Relationship duration for marriages
- Responsive grid layout
- Handles missing relationships gracefully

**Relationship Format**:
```typescript
interface PersonRelationship {
  relationshipType: 'parent' | 'child' | 'spouse' | 'partner' | 'sibling';
  relatedPerson: RelatedPersonInfo;
  relationshipDetails?: string;
  startDate?: Date | string;
  endDate?: Date | string;
}
```

### 7. PhotoGalleryComponent
**Selector**: `app-photo-gallery`

Photo management with grid layout and lightbox viewer.

**Inputs**:
- `personId: number` - Person identifier
- `photos: PersonPhoto[]` - Array of photos
- `canEdit: boolean` - Permission to edit photos

**Outputs**:
- `photoUploaded: EventEmitter<File>` - Emits file when uploaded
- `photoDeleted: EventEmitter<number>` - Emits photo ID when deleted
- `photoPrimaryChanged: EventEmitter<number>` - Emits photo ID when set as primary
- `photoClicked: EventEmitter<PersonPhoto>` - Emits photo when clicked

**Features**:
- Responsive grid layout (250px columns)
- Photo upload with file input
- Primary photo selection and badge
- Full-screen lightbox with navigation
- Previous/Next photo controls
- Photo deletion with confirmation
- Thumbnail support
- Upload date display
- Mobile-optimized design

**Photo Format**:
```typescript
interface PersonPhoto {
  id: number;
  personId: number;
  photoUrl: string;
  thumbnailUrl?: string;
  title?: string;
  description?: string;
  uploadDate: Date | string;
  isPrimary: boolean;
  tags?: string[];
}
```

## Material Design Components Used (Complete List)

Phase 3.1 & 3.2 use:
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
- **MatTabs** (Phase 3.2)
- **MatMenu** (Phase 3.2)

## Usage Examples

### PersonDetailsComponent in Razor View

```html
@{
    var personJson = JsonSerializer.Serialize(Model.Person);
    var eventsJson = JsonSerializer.Serialize(Model.Events);
    var relationshipsJson = JsonSerializer.Serialize(Model.Relationships);
    var photosJson = JsonSerializer.Serialize(Model.Photos);
}

<script>
    window.personData = @Html.Raw(personJson);
    window.events = @Html.Raw(eventsJson);
    window.relationships = @Html.Raw(relationshipsJson);
    window.photos = @Html.Raw(photosJson);
</script>

<app-person-details
    person="personData"
    timeline-events="events"
    relationships="relationships"
    photos="photos"
    can-edit="@(User.IsInRole("Editor") ? "true" : "false")"
    can-delete="@(User.IsInRole("Admin") ? "true" : "false")">
</app-person-details>
```

See `USAGE_EXAMPLES.html` for more comprehensive examples.

## Performance Notes

- Search debouncing reduces API calls
- Client-side filtering is fast for <1000 records
- For larger datasets, implement server-side filtering
- Virtual scrolling can be added for very large lists
- Lazy loading images for better performance
