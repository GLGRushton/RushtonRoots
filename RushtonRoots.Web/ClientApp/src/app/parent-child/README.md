# Parent-Child Relationships Module

## Overview

The Parent-Child module provides comprehensive UI components for managing parent-child relationships in the RushtonRoots family tree application. This module was implemented in Phase 5.2 of the UI Design Plan.

## Components

### 1. ParentChildIndexComponent
**Selector**: `app-parent-child-index`

Main index page for viewing and managing parent-child relationships.

**Features**:
- Responsive grid layout (1-4 columns)
- Search by parent or child name
- Multiple sorting options
- Relationship type filtering
- Verified-only filter
- Empty and loading states

**Usage in Razor**:
```html
<app-parent-child-index></app-parent-child-index>
```

### 2. ParentChildCardComponent
**Selector**: `app-parent-child-card`

Displays individual parent-child relationship as a card.

**Inputs**:
- `relationship: ParentChildCard` - Relationship data
- `elevation: number` - Card elevation (0, 2, 4, 8)
- `showActions: boolean` - Show/hide action buttons

**Outputs**:
- `action: ParentChildActionEvent` - Emitted when user performs action

**Features**:
- Parent and child avatars with fallback initials
- Relationship type chips with icons
- Verification badge for verified relationships
- Confidence badge for AI suggestions
- Child age and birth date display
- Quick actions (View, Edit, Delete, Verify)

**Usage**:
```typescript
<app-parent-child-card
  [relationship]="relationshipData"
  [elevation]="2"
  [showActions]="true"
  (action)="onAction($event)">
</app-parent-child-card>
```

### 3. ParentChildFormComponent
**Selector**: `app-parent-child-form`

Form for creating or editing parent-child relationships.

**Inputs**:
- `relationship?: ParentChildFormData` - Existing data for edit mode
- `availablePeople: PersonOption[]` - People available for selection

**Outputs**:
- `submitted: ParentChildFormData` - Emitted on form submission
- `cancelled: void` - Emitted when form is cancelled
- `validateRequested: ParentChildFormData` - Emitted when validation is requested

**Features**:
- Parent autocomplete with person search
- Child autocomplete with person search
- Relationship type selector with descriptions
- Notes field (500 char limit)
- Verified checkbox
- Validation panel with errors/warnings
- Edit mode support

### 4. FamilyTreeMiniComponent
**Selector**: `app-family-tree-mini`

Compact family tree visualization showing multiple generations.

**Inputs**:
- `focusPersonId: number` - Person to center tree on
- `generations: number` - Number of generations to show (default: 2)
- `showSpouses: boolean` - Show/hide spouse relationships (default: true)
- `compact: boolean` - Use compact mode (default: false)

**Outputs**:
- `personClicked: number` - Emitted when a person is clicked

**Features**:
- Displays grandparents, parents, focus person, spouses, and children
- Visual generation indicators
- Connecting lines between generations
- Person cards with avatars and life spans
- Age calculation for living persons
- Focus person highlighted with star badge

### 5. RelationshipValidationComponent
**Selector**: `app-relationship-validation`

Displays relationship validation results.

**Inputs**:
- `validationResult: ValidationResult` - Validation data
- `expanded: boolean` - Initial expansion state (default: true)

**Features**:
- Color-coded status indicators
- Error messages with types
- Warning messages with types
- Success message for valid relationships
- Expandable panel

### 6. RelationshipSuggestionsComponent
**Selector**: `app-relationship-suggestions`

AI-powered relationship suggestions.

**Outputs**:
- `suggestionAccepted: RelationshipSuggestion` - Emitted when suggestion is accepted
- `suggestionRejected: string` - Emitted when suggestion is rejected (suggestion ID)

**Features**:
- Confidence score display (0-100%)
- AI reasoning explanation
- Evidence sources
- Accept/reject actions
- Refresh functionality
- Help section
- Loading and empty states

### 7. BulkRelationshipImportComponent
**Selector**: `app-bulk-relationship-import`

Bulk import multiple relationships.

**Outputs**:
- `importCompleted: BulkImportResult` - Emitted when import completes

**Features**:
- Two import methods: Manual entry and CSV upload
- Dynamic table for manual entry
- CSV file upload with parsing
- Template download
- Import results with error details
- Reset functionality

## TypeScript Models

Located in `models/parent-child.model.ts`:

### Core Interfaces

```typescript
interface ParentChildCard {
  id: number;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  parentPhotoUrl?: string;
  childPhotoUrl?: string;
  relationshipType: string;
  relationshipTypeDisplay: string;
  relationshipTypeIcon: string;
  relationshipTypeColor: string;
  childBirthDate?: Date;
  childAge?: number;
  isVerified: boolean;
  confidence?: number;
  createdDateTime: Date;
  updatedDateTime: Date;
}

interface RelationshipTypeConfig {
  value: string;
  display: string;
  icon: string;
  color: string;
  description: string;
}

interface ParentChildFormData {
  id?: number;
  parentPersonId?: number;
  childPersonId?: number;
  relationshipType: string;
  notes?: string;
  isVerified?: boolean;
}

interface FamilyTreeNode {
  id: number;
  name: string;
  photoUrl?: string;
  birthDate?: Date;
  deathDate?: Date;
  generation: number;
  parents?: FamilyTreeNode[];
  children?: FamilyTreeNode[];
  spouses?: FamilyTreeNode[];
}

interface RelationshipSuggestion {
  id: string;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  confidence: number;
  reasoning: string;
  suggestedType: string;
  sources?: string[];
}

interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
  warnings: ValidationWarning[];
}
```

### Constants

```typescript
const RELATIONSHIP_TYPES: RelationshipTypeConfig[] = [
  { value: 'biological', display: 'Biological', icon: 'bloodtype', color: 'primary', description: '...' },
  { value: 'adopted', display: 'Adopted', icon: 'volunteer_activism', color: 'accent', description: '...' },
  { value: 'step', display: 'Step', icon: 'family_restroom', color: 'accent', description: '...' },
  { value: 'guardian', display: 'Guardian', icon: 'shield', color: 'accent', description: '...' },
  { value: 'foster', display: 'Foster', icon: 'home', color: 'accent', description: '...' },
  { value: 'unknown', display: 'Unknown', icon: 'help_outline', color: 'warn', description: '...' }
];

const PARENT_CHILD_SORT_OPTIONS: ParentChildSortOption[] = [
  { value: 'childName-asc', display: 'Child Name (A-Z)' },
  { value: 'childName-desc', display: 'Child Name (Z-A)' },
  { value: 'parentName-asc', display: 'Parent Name (A-Z)' },
  { value: 'parentName-desc', display: 'Parent Name (Z-A)' },
  { value: 'childBirthDate-asc', display: 'Birth Date (Oldest First)' },
  { value: 'childBirthDate-desc', display: 'Birth Date (Newest First)' },
  { value: 'created-desc', display: 'Recently Created' },
  { value: 'updated-desc', display: 'Recently Updated' }
];
```

## Material Design Components Used

- MatCard
- MatChip
- MatButton, MatIconButton
- MatIcon
- MatMenu
- MatFormField, MatInput, MatSelect
- MatAutocomplete
- MatCheckbox
- MatExpansionPanel
- MatProgressSpinner
- MatDivider
- MatTooltip
- MatTable
- MatButtonToggleGroup
- MatSlideToggle

## Responsive Design

All components follow mobile-first design principles:

- **Mobile (< 768px)**: Single column layout, touch-friendly buttons
- **Tablet (768px - 1200px)**: 2-column grid
- **Desktop (1200px - 1600px)**: 3-column grid
- **Large Desktop (> 1600px)**: 4-column grid

## Integration with Backend

The module is designed to work with the existing backend:

### Domain Entity
`RushtonRoots.Domain.Database.ParentChild`:
```csharp
public class ParentChild : BaseEntity
{
    public int ParentPersonId { get; set; }
    public int ChildPersonId { get; set; }
    public string RelationshipType { get; set; }
    
    public Person? ParentPerson { get; set; }
    public Person? ChildPerson { get; set; }
}
```

### Backend Integration Points

1. **GET /api/parentchild** - List all relationships
2. **GET /api/parentchild/{id}** - Get single relationship
3. **POST /api/parentchild** - Create new relationship
4. **PUT /api/parentchild/{id}** - Update relationship
5. **DELETE /api/parentchild/{id}** - Delete relationship
6. **POST /api/parentchild/validate** - Validate relationship
7. **GET /api/parentchild/suggestions** - Get AI suggestions
8. **POST /api/parentchild/bulk-import** - Bulk import

## Development Notes

### Adding the Module to a View

1. Import the module in `app.module.ts` (already done)
2. Use the Angular Elements in Razor views:

```cshtml
@{
    ViewData["Title"] = "Parent-Child Relationships";
}

<div class="container">
    <app-parent-child-index></app-parent-child-index>
</div>
```

### Styling

Components use:
- Material Design theming
- BEM methodology for CSS classes
- SCSS with component-scoped styles
- Responsive breakpoints defined in styles/_variables.scss

### Testing

Create unit tests following the pattern:
```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ParentChildCardComponent } from './parent-child-card.component';

describe('ParentChildCardComponent', () => {
  let component: ParentChildCardComponent;
  let fixture: ComponentFixture<ParentChildCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParentChildCardComponent ]
    })
    .compileComponents();
  });

  // Add tests here
});
```

## Future Enhancements

Potential improvements for future phases:

1. **Advanced AI Features**:
   - Machine learning model for higher accuracy suggestions
   - Pattern recognition for bulk relationship discovery
   - Automatic verification based on multiple evidence sources

2. **Enhanced Visualization**:
   - Interactive family tree with zoom/pan
   - 3D family tree visualization
   - Timeline view of relationships

3. **Import/Export**:
   - GEDCOM import/export support
   - Excel import/export
   - Integration with ancestry.com and other services

4. **Collaboration**:
   - Real-time collaborative editing
   - Relationship approval workflow
   - Change history and audit trail

5. **Mobile App**:
   - Native mobile app using these components
   - Offline support
   - Photo capture for documentation

## Support

For questions or issues, refer to:
- Main documentation: `/docs/UI_DesignPlan.md`
- Architecture patterns: `/PATTERNS.md`
- Project README: `/README.md`
