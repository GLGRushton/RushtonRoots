# Household Module

This module contains Angular components for managing and displaying households in the RushtonRoots application.

## Phase 4.1: Household Index & Cards

This phase implements the household index page with card-based layout, search, filtering, and sorting capabilities.

## Components

### HouseholdIndexComponent

Main component for the household index page that displays all households in a responsive grid layout.

**Features:**
- Responsive grid layout (1-4 columns based on screen size)
- Search by household name or anchor person
- Filtering options (member count, creation date, has anchor)
- Multiple sorting options
- Empty state when no households found
- Loading state support
- Create household button (if user has permission)

**Inputs:**
- `initialHouseholds: HouseholdCard[]` - Array of households to display
- `canEdit: boolean` - Whether user can edit households
- `canDelete: boolean` - Whether user can delete households
- `canCreate: boolean` - Whether user can create new households

**Usage:**
```html
<app-household-index
  [initialHouseholds]="households"
  [canEdit]="true"
  [canDelete]="true"
  [canCreate]="true">
</app-household-index>
```

### HouseholdCardComponent

Displays a single household as a Material Card with details and quick actions.

**Features:**
- Material Card design with elevation
- Member count badge
- Anchor person display with avatar
- Member preview (shows first 3 members)
- Creation and update dates
- Quick action buttons (View, Edit)
- More actions menu (Manage Members, Settings, Delete)
- Responsive design
- Hover effects

**Inputs:**
- `household: HouseholdCard` - The household data to display
- `canEdit: boolean` - Whether to show edit button
- `canDelete: boolean` - Whether to show delete option
- `showMembers: boolean` - Whether to show member preview (default: true)
- `elevation: number` - Material elevation level 0-24 (default: 2)

**Outputs:**
- `action: EventEmitter<HouseholdAction>` - Emits when user performs an action

**Usage:**
```html
<app-household-card
  [household]="householdData"
  [canEdit]="true"
  [canDelete]="true"
  [showMembers]="true"
  [elevation]="2"
  (action)="onHouseholdAction($event)">
</app-household-card>
```

## Models

### HouseholdCard
```typescript
interface HouseholdCard {
  id: number;
  householdName: string;
  anchorPersonId?: number;
  anchorPersonName?: string;
  memberCount: number;
  createdDateTime: Date;
  updatedDateTime: Date;
  members?: HouseholdMember[];
}
```

### HouseholdMember
```typescript
interface HouseholdMember {
  personId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  photoUrl?: string;
  relationship?: string;
  isAnchor: boolean;
}
```

### HouseholdSearchFilters
```typescript
interface HouseholdSearchFilters {
  searchTerm?: string;
  minMemberCount?: number;
  maxMemberCount?: number;
  createdAfter?: Date;
  createdBefore?: Date;
  hasAnchor?: boolean;
}
```

### HouseholdSortOption
```typescript
interface HouseholdSortOption {
  field: 'name' | 'memberCount' | 'createdDate' | 'updatedDate';
  direction: 'asc' | 'desc';
  label: string;
}
```

### HouseholdAction
```typescript
interface HouseholdAction {
  type: 'view' | 'edit' | 'delete' | 'manage-members' | 'settings';
  householdId: number;
  data?: any;
}
```

## Available Sort Options

The module provides predefined sorting options:
- Name (A-Z)
- Name (Z-A)
- Most Members
- Least Members
- Newest First
- Oldest First
- Recently Updated

## Material Design Components Used

- **MatCard** - Card layout for households
- **MatBadge** - Member count badges
- **MatButton** - Action buttons
- **MatIcon** - Icons throughout
- **MatMenu** - More actions menu
- **MatFormField** - Search and filter inputs
- **MatSelect** - Sort dropdown
- **MatTooltip** - Helpful tooltips
- **MatDivider** - Visual separators
- **MatProgressSpinner** - Loading indicator

## Responsive Breakpoints

- **Mobile (< 600px)**: 1 column grid
- **Tablet (600px - 960px)**: 2 column grid
- **Small Desktop (960px - 1280px)**: 3 column grid
- **Large Desktop (≥ 1280px)**: 4 column grid

## Angular Elements

Both components are registered as Angular Elements and can be used in Razor views:

```html
<!-- In a .cshtml file -->
<app-household-index
  initial-households='@Json.Serialize(Model.Households)'
  can-edit="true"
  can-delete="false"
  can-create="true">
</app-household-index>
```

## Future Enhancements

Potential improvements for future phases:
- Advanced filter panel with more options
- Household comparison view
- Bulk operations (delete, merge)
- Export to PDF/CSV
- Household statistics dashboard
- Drag-and-drop sorting
- Custom grid layouts
- Household templates
