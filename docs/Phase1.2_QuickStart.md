# Phase 1.2 Components - Quick Start Guide

This guide shows how to use the 8 core reusable components created in Phase 1.2.

## Components Overview

All components are available in two ways:
1. **In Angular templates** - Use the standard Angular component syntax
2. **In Razor views** - Use as Angular Elements (custom HTML elements)

---

## 1. PersonCardComponent

### In Razor Views (.cshtml)
```html
<app-person-card 
  first-name="John"
  last-name="Doe"
  birth-date="1980-01-15"
  death-date="2050-12-31"
  photo-url="/images/john.jpg"
  show-actions="true">
</app-person-card>
```

### In Angular Templates
```html
<app-person-card
  [firstName]="person.firstName"
  [lastName]="person.lastName"
  [birthDate]="person.birthDate"
  [deathDate]="person.deathDate"
  [photoUrl]="person.photoUrl"
  [showActions]="true">
</app-person-card>
```

---

## 2. PersonListComponent

### In Angular Templates Only
```typescript
// Component
samplePeople: PersonListItem[] = [
  { id: 1, firstName: 'John', lastName: 'Doe', birthDate: '1980-01-15' },
  { id: 2, firstName: 'Jane', lastName: 'Smith', birthDate: '1985-05-20' }
];

onPersonSelected(personId: number) {
  console.log('Selected person:', personId);
}
```

```html
<!-- Template -->
<app-person-list 
  [people]="samplePeople"
  [showPagination]="true"
  (personSelected)="onPersonSelected($event)">
</app-person-list>
```

---

## 3. SearchBarComponent

### In Razor Views (.cshtml)
```html
<app-search-bar 
  placeholder="Search people..."
  debounce-time="300">
</app-search-bar>

<script>
  document.querySelector('app-search-bar')
    .addEventListener('searchChanged', (e) => {
      console.log('Search:', e.detail);
      // Perform search with: e.detail
    });
</script>
```

### In Angular Templates
```typescript
// Component
onSearchChanged(searchTerm: string) {
  console.log('Searching for:', searchTerm);
  // Perform search...
}
```

```html
<!-- Template -->
<app-search-bar 
  [placeholder]="'Search people...'"
  [debounceTime]="300"
  (searchChanged)="onSearchChanged($event)">
</app-search-bar>
```

---

## 4. PageHeaderComponent

### In Razor Views (.cshtml)
```html
<app-page-header 
  title="People Management"
  subtitle="View and manage family members"
  show-back-button="false">
  <!-- Actions go in named slot -->
  <div slot="actions">
    <button class="btn btn-primary">Add Person</button>
  </div>
</app-page-header>
```

### In Angular Templates
```typescript
// Component
breadcrumbs = [
  { label: 'Home', url: '/', icon: 'home' },
  { label: 'People', url: '/Person' },
  { label: 'John Doe' }
];
```

```html
<!-- Template -->
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

## 5. BreadcrumbComponent

### In Razor Views (.cshtml)
```html
<!-- Note: Breadcrumbs must be passed as JSON string -->
<app-breadcrumb 
  items='[
    {"label":"Home","url":"/","icon":"home"},
    {"label":"People","url":"/Person"},
    {"label":"John Doe"}
  ]'>
</app-breadcrumb>
```

### In Angular Templates
```typescript
// Component
breadcrumbs = [
  { label: 'Home', url: '/', icon: 'home' },
  { label: 'People', url: '/Person' },
  { label: 'John Doe' }
];
```

```html
<!-- Template -->
<app-breadcrumb [items]="breadcrumbs"></app-breadcrumb>
```

---

## 6. EmptyStateComponent

### In Razor Views (.cshtml)
```html
<app-empty-state 
  icon="person_off"
  message="No people found"
  submessage="Try adjusting your search criteria"
  action-label="Add Person">
</app-empty-state>

<script>
  document.querySelector('app-empty-state')
    .addEventListener('actionClick', () => {
      window.location.href = '/Person/Create';
    });
</script>
```

### In Angular Templates
```typescript
// Component
onAddPerson() {
  // Navigate to create person page
}
```

```html
<!-- Template -->
<app-empty-state 
  [icon]="'person_off'"
  [message]="'No people found'"
  [submessage]="'Try adjusting your search criteria'"
  [actionLabel]="'Add Person'"
  (actionClick)="onAddPerson()">
</app-empty-state>
```

---

## 7. LoadingSpinnerComponent

### In Razor Views (.cshtml)
```html
<!-- Small spinner -->
<app-loading-spinner size="small"></app-loading-spinner>

<!-- Medium spinner with message -->
<app-loading-spinner 
  size="medium" 
  message="Loading people...">
</app-loading-spinner>

<!-- Full-page overlay -->
<app-loading-spinner 
  size="large"
  message="Processing..." 
  overlay="true">
</app-loading-spinner>
```

### In Angular Templates
```html
<!-- Show loading state conditionally -->
<app-loading-spinner 
  *ngIf="isLoading"
  [size]="'medium'"
  [message]="'Loading people...'"
  [overlay]="true">
</app-loading-spinner>

<!-- Content -->
<div *ngIf="!isLoading">
  <!-- Your content here -->
</div>
```

---

## 8. ConfirmDialogComponent

### Usage via Service (Angular Only)

```typescript
import { ConfirmDialogService } from './shared/components/confirm-dialog/confirm-dialog.service';

export class MyComponent {
  constructor(private confirmDialog: ConfirmDialogService) {}

  deletePerson(person: Person) {
    this.confirmDialog.confirmDelete(person.name, 'Person')
      .subscribe(confirmed => {
        if (confirmed) {
          // Perform delete
          this.personService.delete(person.id);
        }
      });
  }

  performAction() {
    this.confirmDialog.confirm({
      title: 'Confirm Action',
      message: 'Are you sure you want to proceed?',
      confirmText: 'Yes, Continue',
      cancelText: 'Cancel',
      confirmColor: 'primary'
    }).subscribe(confirmed => {
      if (confirmed) {
        // Perform action
      }
    });
  }
}
```

---

## Common Patterns

### Loading State Pattern
```typescript
// Component
isLoading = false;
people: Person[] = [];

async loadPeople() {
  this.isLoading = true;
  try {
    this.people = await this.personService.getAll();
  } finally {
    this.isLoading = false;
  }
}
```

```html
<!-- Template -->
<app-loading-spinner *ngIf="isLoading" [size]="'medium'" [message]="'Loading...'">
</app-loading-spinner>

<app-person-list *ngIf="!isLoading && people.length > 0" [people]="people">
</app-person-list>

<app-empty-state *ngIf="!isLoading && people.length === 0"
  [icon]="'person_off'"
  [message]="'No people found'">
</app-empty-state>
```

### Search with Results Pattern
```typescript
// Component
searchTerm = '';
allPeople: Person[] = [];
filteredPeople: Person[] = [];

onSearch(term: string) {
  this.searchTerm = term;
  this.filteredPeople = this.allPeople.filter(p => 
    p.firstName.toLowerCase().includes(term.toLowerCase()) ||
    p.lastName.toLowerCase().includes(term.toLowerCase())
  );
}
```

```html
<!-- Template -->
<app-search-bar 
  [placeholder]="'Search people...'"
  (searchChanged)="onSearch($event)">
</app-search-bar>

<app-person-list 
  *ngIf="filteredPeople.length > 0"
  [people]="filteredPeople">
</app-person-list>

<app-empty-state 
  *ngIf="searchTerm && filteredPeople.length === 0"
  [icon]="'search_off'"
  [message]="'No results found'"
  [submessage]="'Try a different search term'">
</app-empty-state>
```

---

## Viewing the Components

To see all components in action:

1. Start the application:
   ```bash
   cd RushtonRoots.Web
   dotnet run
   ```

2. Navigate to the Style Guide page (typically `/StyleGuide` or check your routing configuration)

3. Scroll to the "Phase 1.2: Core Reusable Components" section

---

## Next Steps

These components are ready to be used in:
- Person management pages (Index, Details, Create, Edit)
- Household management pages
- Partnership and ParentChild relationship pages
- Any other feature requiring consistent UI patterns

For detailed documentation, see: `docs/Phase1.2_Components_Documentation.md`
