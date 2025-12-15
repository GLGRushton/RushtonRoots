# Phase 1.2 Core Reusable Components Documentation

This document provides comprehensive documentation for the 8 core reusable Angular components created in Phase 1.2 of the UI Design Plan.

## Table of Contents
1. [PersonCardComponent](#personcard-component)
2. [PersonListComponent](#personlist-component)
3. [SearchBarComponent](#searchbar-component)
4. [PageHeaderComponent](#pageheader-component)
5. [EmptyStateComponent](#emptystate-component)
6. [ConfirmDialogComponent](#confirmdialog-component)
7. [LoadingSpinnerComponent](#loadingspinner-component)
8. [BreadcrumbComponent](#breadcrumb-component)

---

## PersonCard Component

**Location**: `src/app/shared/components/person-card/`  
**Selector**: `app-person-card`  
**Angular Element**: `<app-person-card>`

### Purpose
Displays a person summary in a Material Design card format with optional actions.

### Inputs
- `personId?: number` - ID of the person
- `firstName: string` - Person's first name
- `lastName: string` - Person's last name
- `birthDate?: string` - Birth date (ISO format)
- `deathDate?: string` - Death date (ISO format)
- `photoUrl?: string` - URL to person's photo
- `showActions: boolean` - Show View/Edit buttons (default: false)

### Features
- Displays person avatar (photo or initials)
- Shows full name and life span (birth year - death year)
- Hover effect with elevation
- Optional action buttons for View and Edit
- Responsive design

### Usage in Razor Views
```html
<app-person-card 
  first-name="John"
  last-name="Doe"
  birth-date="1980-01-15"
  photo-url="/images/john-doe.jpg"
  show-actions="true">
</app-person-card>
```

### Usage in Angular Templates
```html
<app-person-card
  [firstName]="'John'"
  [lastName]="'Doe'"
  [birthDate]="'1980-01-15'"
  [photoUrl]="'/images/john-doe.jpg'"
  [showActions]="true">
</app-person-card>
```

---

## PersonList Component

**Location**: `src/app/shared/components/person-list/`  
**Selector**: `app-person-list`  
**Angular Element**: `<app-person-list>`

### Purpose
Displays a list of people in a Material Table with sorting, pagination, and actions.

### Inputs
- `people: PersonListItem[]` - Array of people to display
- `showPagination: boolean` - Show pagination controls (default: true)

### Outputs
- `personSelected: EventEmitter<number>` - Emits when a person is selected

### PersonListItem Interface
```typescript
interface PersonListItem {
  id: number;
  firstName: string;
  lastName: string;
  birthDate?: string;
  deathDate?: string;
}
```

### Features
- Sortable columns (firstName, lastName, birthDate, deathDate)
- Pagination with configurable page sizes
- View and Edit action buttons
- Empty state integration
- Responsive design

### Usage in Angular Templates
```html
<app-person-list 
  [people]="peopleArray"
  [showPagination]="true"
  (personSelected)="onPersonSelected($event)">
</app-person-list>
```

---

## SearchBar Component

**Location**: `src/app/shared/components/search-bar/`  
**Selector**: `app-search-bar`  
**Angular Element**: `<app-search-bar>`

### Purpose
Reusable search interface with debounced input to reduce API calls.

### Inputs
- `placeholder: string` - Placeholder text (default: 'Search...')
- `debounceTime: number` - Debounce time in milliseconds (default: 300)

### Outputs
- `searchChanged: EventEmitter<string>` - Emits debounced search term

### Features
- Debounced search input (prevents excessive API calls)
- Clear button when text is entered
- Material Design outlined input
- Search icon prefix
- Accessible with proper ARIA labels

### Usage in Angular Templates
```html
<app-search-bar 
  [placeholder]="'Search people...'"
  [debounceTime]="300"
  (searchChanged)="onSearch($event)">
</app-search-bar>
```

### Usage in Razor Views
```html
<app-search-bar 
  placeholder="Search people..."
  debounce-time="300">
</app-search-bar>

<script>
  document.querySelector('app-search-bar').addEventListener('searchChanged', (e) => {
    console.log('Search term:', e.detail);
  });
</script>
```

---

## PageHeader Component

**Location**: `src/app/shared/components/page-header/`  
**Selector**: `app-page-header`  
**Angular Element**: `<app-page-header>`

### Purpose
Consistent page headers with title, subtitle, breadcrumbs, and action buttons.

### Inputs
- `title: string` - Page title
- `subtitle?: string` - Optional subtitle/description
- `breadcrumbs: BreadcrumbItem[]` - Array of breadcrumb items
- `showBackButton: boolean` - Show back navigation button (default: false)

### Content Projection
- `[actions]` - Content projected for action buttons

### BreadcrumbItem Interface
```typescript
interface BreadcrumbItem {
  label: string;
  url?: string;
  icon?: string;
}
```

### Features
- Material Design card elevation
- Optional back button
- Breadcrumb integration
- Content projection for custom actions
- Responsive layout (stacks on mobile)

### Usage in Angular Templates
```html
<app-page-header 
  [title]="'People Management'"
  [subtitle]="'View and manage family members'"
  [breadcrumbs]="breadcrumbs"
  [showBackButton]="false">
  <div actions>
    <button mat-raised-button color="primary">Add Person</button>
  </div>
</app-page-header>
```

---

## EmptyState Component

**Location**: `src/app/shared/components/empty-state/`  
**Selector**: `app-empty-state`  
**Angular Element**: `<app-empty-state>`

### Purpose
Display "no data" states with icon, message, and optional action button.

### Inputs
- `icon: string` - Material icon name (default: 'inbox')
- `message: string` - Primary message (default: 'No data available')
- `submessage?: string` - Optional secondary message
- `actionLabel?: string` - Label for action button

### Outputs
- `actionClick: EventEmitter<void>` - Emits when action button is clicked

### Features
- Large icon with opacity
- Primary and secondary messages
- Optional action button
- Content projection for custom content
- Responsive design

### Usage in Angular Templates
```html
<app-empty-state 
  [icon]="'person_off'"
  [message]="'No people found'"
  [submessage]="'Try adjusting your search criteria'"
  [actionLabel]="'Add Person'"
  (actionClick)="onAddPerson()">
</app-empty-state>
```

---

## ConfirmDialog Component

**Location**: `src/app/shared/components/confirm-dialog/`  
**Selector**: `app-confirm-dialog`  
**Service**: `ConfirmDialogService`

### Purpose
Confirmation dialogs for delete operations and other actions requiring user confirmation.

### Service Usage
The component is designed to be used via the `ConfirmDialogService` for easy invocation.

### ConfirmDialogData Interface
```typescript
interface ConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;  // default: 'Confirm'
  cancelText?: string;   // default: 'Cancel'
  confirmColor?: 'primary' | 'accent' | 'warn';  // default: 'primary'
}
```

### Service Methods
- `confirm(data: ConfirmDialogData): Observable<boolean>`
- `confirmDelete(itemName: string, itemType: string): Observable<boolean>`

### Features
- Material Dialog integration
- Configurable title, message, and button text
- Returns Observable<boolean> for result
- Auto-focus on cancel button
- Keyboard accessible

### Usage in Components
```typescript
constructor(private confirmDialog: ConfirmDialogService) {}

// Generic confirm
this.confirmDialog.confirm({
  title: 'Confirm Action',
  message: 'Are you sure you want to proceed?',
  confirmText: 'Yes, Proceed',
  cancelText: 'Cancel'
}).subscribe(confirmed => {
  if (confirmed) {
    // User confirmed
  }
});

// Delete confirm
this.confirmDialog.confirmDelete('John Doe', 'Person')
  .subscribe(confirmed => {
    if (confirmed) {
      // Perform delete
    }
  });
```

---

## LoadingSpinner Component

**Location**: `src/app/shared/components/loading-spinner/`  
**Selector**: `app-loading-spinner`  
**Angular Element**: `<app-loading-spinner>`

### Purpose
Consistent loading states with Material Spinner and optional message.

### Inputs
- `size: 'small' | 'medium' | 'large'` - Spinner size (default: 'medium')
- `message?: string` - Optional loading message
- `overlay: boolean` - Show as full-page overlay (default: false)

### Size Mapping
- `small`: 30px diameter
- `medium`: 50px diameter
- `large`: 80px diameter

### Features
- Three size variants
- Optional loading message
- Full-page overlay mode with backdrop blur
- Centered layout
- Accessible with ARIA labels

### Usage in Angular Templates
```html
<!-- Inline spinner -->
<app-loading-spinner 
  [size]="'medium'"
  [message]="'Loading people...'">
</app-loading-spinner>

<!-- Full-page overlay -->
<app-loading-spinner 
  [size]="'large'"
  [message]="'Processing...'"
  [overlay]="true">
</app-loading-spinner>
```

---

## Breadcrumb Component

**Location**: `src/app/shared/components/breadcrumb/`  
**Selector**: `app-breadcrumb`  
**Angular Element**: `<app-breadcrumb>`

### Purpose
Navigation breadcrumbs for hierarchical page navigation.

### Inputs
- `items: BreadcrumbItem[]` - Array of breadcrumb items

### BreadcrumbItem Interface
```typescript
interface BreadcrumbItem {
  label: string;
  url?: string;
  icon?: string;
}
```

### Features
- Material icons support
- Chevron separators
- Active state for current page
- Clickable links (except last item)
- Accessible navigation with ARIA labels
- Responsive design

### Usage in Angular Templates
```html
<app-breadcrumb 
  [items]="[
    {label: 'Home', url: '/', icon: 'home'},
    {label: 'People', url: '/Person'},
    {label: 'John Doe'}
  ]">
</app-breadcrumb>
```

---

## Integration Notes

### Angular Elements
All components are registered as Angular Elements in `app.module.ts`, making them available for use in Razor views (.cshtml files).

### Shared Module
All components are declared and exported in the `SharedModule`, making them available to any feature module that imports `SharedModule`.

### Material Dependencies
All components use Angular Material components and follow Material Design guidelines for consistency.

### Accessibility
All components include:
- Proper ARIA labels
- Keyboard navigation support
- Focus management
- Screen reader friendly markup

### Responsive Design
All components are responsive and work well on:
- Desktop (1280px+)
- Tablet (768px - 1280px)
- Mobile (< 768px)

---

## Testing the Components

Components can be viewed and tested in the Style Guide page:

1. Run the application: `dotnet run` from `RushtonRoots.Web`
2. Navigate to the style guide page (check your routes)
3. Scroll to "Phase 1.2: Core Reusable Components" section

---

## Next Steps

These components provide the foundation for:
- Person management UI (Phase 3)
- Household management UI (Phase 4)
- Relationship management UI (Phase 5)
- And all other features in the UI Design Plan

Components can be extended or customized as needed while maintaining consistency across the application.
