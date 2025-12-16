# ParentChildDetailsComponent

## Overview

The `ParentChildDetailsComponent` is a comprehensive Angular component for displaying detailed information about parent-child relationships in the RushtonRoots genealogy application. This component was developed as part of Phase 5.2 of the UI Design Plan.

## Features

### Tabbed Interface
- **Overview Tab**: Relationship type description and notes
- **Family Context Tab**: Mini family tree showing grandparents and siblings
- **Evidence Tab**: Supporting documents, sources, and DNA evidence
- **Timeline Tab**: Key events in the relationship

### Relationship Summary
- Parent information with photo, name, and dates
- Child information with photo, name, and dates
- Relationship type with icon and description
- Verification status badge
- Confidence score (for AI-suggested relationships)
- Creation and update timestamps

### Family Context Integration
- Integration with `FamilyTreeMiniComponent`
- Displays parent's parents (grandparents)
- Shows parent's other children (siblings)
- Clickable person cards for navigation

### Evidence Tracking
- Source citations
- Supporting documents
- DNA evidence
- Photos and other evidence types
- Links to external resources

### Timeline View
- Child's birth event
- Key life events showing parent-child interactions
- Visual timeline with icons and dates

### Actions
- Edit button (opens form in edit mode)
- Delete button (with confirmation)
- Verify relationship button (for unverified relationships)
- Inline note editing

## Component API

### Inputs

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `relationship` | `ParentChildDetails` | Required | The relationship details to display |
| `evidence` | `ParentChildEvidence[]` | `[]` | Array of evidence items |
| `events` | `ParentChildEvent[]` | `[]` | Array of timeline events |
| `grandparents` | `FamilyTreeNode[]` | `[]` | Array of grandparents |
| `siblings` | `FamilyTreeNode[]` | `[]` | Array of siblings |
| `canEdit` | `boolean` | `false` | Whether user can edit the relationship |
| `canDelete` | `boolean` | `false` | Whether user can delete the relationship |
| `canVerify` | `boolean` | `false` | Whether user can verify the relationship |
| `initialTab` | `number` | `0` | Initial tab index to display |

### Outputs

| Output | Type | Description |
|--------|------|-------------|
| `editClicked` | `EventEmitter<number>` | Emitted when edit button is clicked (relationship ID) |
| `deleteClicked` | `EventEmitter<number>` | Emitted when delete is confirmed (relationship ID) |
| `verifyClicked` | `EventEmitter<number>` | Emitted when verify button is clicked (relationship ID) |
| `personClicked` | `EventEmitter<number>` | Emitted when a person is clicked (person ID) |
| `evidenceAdded` | `EventEmitter<ParentChildEvidence>` | Emitted when evidence is added |
| `evidenceDeleted` | `EventEmitter<number>` | Emitted when evidence is deleted (evidence ID) |
| `noteUpdated` | `EventEmitter<string>` | Emitted when notes are updated |

## Usage

### In Razor View

```cshtml
@model RushtonRoots.Domain.UI.Models.ParentChildViewModel

<app-parent-child-details
    [relationship]="relationshipData"
    [evidence]="evidenceData"
    [events]="eventsData"
    [grandparents]="grandparentsData"
    [siblings]="siblingsData"
    [can-edit]="@(User.IsInRole("Admin") || User.IsInRole("HouseholdAdmin") ? "true" : "false")"
    [can-delete]="@(User.IsInRole("Admin") || User.IsInRole("HouseholdAdmin") ? "true" : "false")"
    [can-verify]="@(User.IsInRole("Admin") || User.IsInRole("HouseholdAdmin") ? "true" : "false")"
    (editClicked)="handleEdit($event)"
    (deleteClicked)="handleDelete($event)"
    (verifyClicked)="handleVerify($event)"
    (personClicked)="handlePersonClick($event)"
    (noteUpdated)="handleNoteUpdate($event)">
</app-parent-child-details>
```

### In TypeScript

```typescript
import { Component } from '@angular/core';
import { ParentChildDetails, ParentChildEvidence, ParentChildEvent, FamilyTreeNode } from './models/parent-child.model';

@Component({
  selector: 'app-example',
  template: `
    <app-parent-child-details
      [relationship]="relationship"
      [evidence]="evidence"
      [events]="events"
      [grandparents]="grandparents"
      [siblings]="siblings"
      [canEdit]="true"
      [canDelete]="true"
      [canVerify]="true"
      (editClicked)="onEdit($event)"
      (deleteClicked)="onDelete($event)"
      (verifyClicked)="onVerify($event)"
      (personClicked)="onPersonClick($event)"
      (noteUpdated)="onNoteUpdate($event)">
    </app-parent-child-details>
  `
})
export class ExampleComponent {
  relationship: ParentChildDetails;
  evidence: ParentChildEvidence[] = [];
  events: ParentChildEvent[] = [];
  grandparents: FamilyTreeNode[] = [];
  siblings: FamilyTreeNode[] = [];

  onEdit(relationshipId: number): void {
    console.log('Edit relationship:', relationshipId);
  }

  onDelete(relationshipId: number): void {
    console.log('Delete relationship:', relationshipId);
  }

  onVerify(relationshipId: number): void {
    console.log('Verify relationship:', relationshipId);
  }

  onPersonClick(personId: number): void {
    console.log('Navigate to person:', personId);
  }

  onNoteUpdate(notes: string): void {
    console.log('Update notes:', notes);
  }
}
```

## TypeScript Interfaces

### ParentChildDetails

```typescript
interface ParentChildDetails {
  id: number;
  parentPersonId: number;
  childPersonId: number;
  parentName: string;
  childName: string;
  parentPhotoUrl?: string;
  childPhotoUrl?: string;
  parentBirthDate?: Date;
  parentDeathDate?: Date;
  childBirthDate?: Date;
  childDeathDate?: Date;
  relationshipType: string;
  relationshipTypeDisplay: string;
  relationshipTypeIcon: string;
  relationshipTypeColor: string;
  relationshipTypeDescription: string;
  isVerified: boolean;
  confidence?: number;
  notes?: string;
  createdDateTime: Date;
  updatedDateTime: Date;
}
```

### ParentChildEvidence

```typescript
interface ParentChildEvidence {
  id: number;
  type: 'source' | 'document' | 'dna' | 'photo' | 'other';
  title: string;
  description?: string;
  url?: string;
  documentUrl?: string;
  addedDate: Date;
}
```

### ParentChildEvent

```typescript
interface ParentChildEvent {
  id: number;
  title: string;
  date: Date;
  description?: string;
  type: 'birth' | 'adoption' | 'guardianship' | 'other';
  icon: string;
  color: string;
}
```

## Styling

The component uses Material Design with professional styling:
- Responsive grid layout
- Mobile-first design
- Touch-friendly interface
- High contrast mode support
- Reduced motion support
- WCAG 2.1 AA compliant

## Dependencies

- Angular Material (Tabs, Cards, Lists, Icons, Chips, Buttons)
- `FamilyTreeMiniComponent` (for family context visualization)
- FormsModule (for ngModel in notes editing)
- CommonModule

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers (iOS Safari, Chrome Mobile)

## Accessibility

- ARIA labels on all interactive elements
- Keyboard navigation support
- Screen reader friendly
- Color contrast meets WCAG AA standards
- Focus indicators visible
- Semantic HTML structure

## Testing

Unit tests should cover:
- Component initialization
- Tab navigation
- Event emissions
- Data binding
- Conditional rendering
- Note editing workflow

## Integration with Backend

The component expects data in the following format from the backend:

1. **Relationship Details**: Transformed from `ParentChildViewModel`
2. **Evidence**: Fetched from evidence repository/service
3. **Events**: Generated from relationship timeline
4. **Family Context**: Fetched from Person and ParentChild relationships

See `/Views/ParentChild/Details.cshtml` for the Razor integration example.

## Future Enhancements

Potential improvements:
1. Evidence upload functionality
2. Event creation/editing
3. Relationship verification workflow
4. Photo gallery integration
5. Document viewer
6. DNA evidence visualization
7. Printable relationship report
8. Export to PDF

## Related Components

- `ParentChildIndexComponent` - List view
- `ParentChildFormComponent` - Create/Edit form
- `FamilyTreeMiniComponent` - Family tree visualization
- `RelationshipValidationComponent` - Validation display

## Support

For questions or issues, refer to:
- Main documentation: `/docs/UpdateDesigns.md` (Phase 5.2)
- Architecture patterns: `/PATTERNS.md`
- Project README: `/README.md`
