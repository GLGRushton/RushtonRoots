# PersonDeleteDialogComponent

Material Dialog component for confirming person deletion with comprehensive safety checks and cascade delete warnings.

## Features

- **Person Summary Display**: Shows name, dates, photo, and status
- **Cascade Delete Warnings**: Lists all related data that will be affected
- **Multiple Delete Options**:
  - Soft Delete (default): Marks as deleted, can be restored
  - Archive: Preserves data for historical purposes
  - Hard Delete (admin only): Permanently removes all data
- **Related Data Display**: Shows counts of affected relationships, photos, stories, documents, and life events
- **Confirmation Checkbox**: Required acknowledgment before deletion
- **Optional Relationship Transfer**: Transfer relationships to another person before deletion
- **Responsive Design**: Mobile-friendly layout
- **Accessibility**: WCAG 2.1 AA compliant

## Usage

### Opening the Dialog

```typescript
import { MatDialog } from '@angular/material/dialog';
import { PersonDeleteDialogComponent } from './person-delete-dialog/person-delete-dialog.component';
import { PersonDeleteDialogData } from '../models/person-delete.model';

constructor(private dialog: MatDialog) {}

openDeleteDialog(person: PersonViewModel): void {
  const dialogData: PersonDeleteDialogData = {
    personId: person.id,
    fullName: person.fullName,
    photoUrl: person.photoUrl,
    dateOfBirth: person.dateOfBirth,
    dateOfDeath: person.dateOfDeath,
    isDeceased: person.isDeceased,
    householdName: person.householdName,
    relatedData: {
      relationships: {
        parents: 2,
        children: 3,
        spouses: 1,
        siblings: 4,
        total: 10
      },
      householdMemberships: 1,
      photos: 25,
      stories: 5,
      documents: 8,
      lifeEvents: 12
    }
  };

  const dialogRef = this.dialog.open(PersonDeleteDialogComponent, {
    width: '600px',
    maxWidth: '90vw',
    data: dialogData,
    disableClose: true // Prevent closing by clicking outside
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.handlePersonDeletion(person.id, result);
    }
  });
}

handlePersonDeletion(personId: number, options: PersonDeleteOptions): void {
  switch (options.deleteType) {
    case 'soft':
      // Call API to soft delete person
      this.personService.softDelete(personId).subscribe(/* ... */);
      break;
    case 'archive':
      // Call API to archive person
      this.personService.archive(personId).subscribe(/* ... */);
      break;
    case 'hard':
      // Call API to permanently delete person
      this.personService.hardDelete(personId).subscribe(/* ... */);
      break;
  }
  
  if (options.transferRelationshipsTo) {
    // Handle relationship transfer
    this.personService.transferRelationships(
      personId, 
      options.transferRelationshipsTo
    ).subscribe(/* ... */);
  }
}
```

### Embedding in Razor View (Delete.cshtml)

```html
<app-person-delete-dialog 
  [person-id]="@Model.Id"
  [full-name]="@Model.FullName"
  [photo-url]="@Model.PhotoUrl"
  [date-of-birth]="@Model.DateOfBirth?.ToString("o")"
  [date-of-death]="@Model.DateOfDeath?.ToString("o")"
  [is-deceased]="@Model.IsDeceased.ToString().ToLower()"
  [household-name]="@Model.HouseholdName"
  [related-data]="@Html.Raw(Json.Serialize(Model.RelatedData))">
</app-person-delete-dialog>
```

## Data Models

### PersonDeleteDialogData

```typescript
interface PersonDeleteDialogData {
  personId: number;
  fullName: string;
  photoUrl?: string;
  dateOfBirth?: Date | string;
  dateOfDeath?: Date | string;
  isDeceased: boolean;
  householdName?: string;
  relatedData: PersonRelatedData;
}
```

### PersonRelatedData

```typescript
interface PersonRelatedData {
  relationships: RelationshipSummary;
  householdMemberships: number;
  photos: number;
  stories: number;
  documents: number;
  lifeEvents: number;
}
```

### PersonDeleteOptions (Return Type)

```typescript
interface PersonDeleteOptions {
  deleteType: 'soft' | 'hard' | 'archive';
  transferRelationshipsTo?: number;
  confirmed: boolean;
}
```

## Delete Types

### Soft Delete (Recommended)
- Person is marked as deleted in database
- Hidden from most views
- Data is preserved
- Can be restored by administrator
- Relationships remain intact but hidden

### Archive
- Person moved to archive section
- Visible only in archive views
- All data preserved for historical purposes
- Cannot be easily restored
- Useful for maintaining family history records

### Hard Delete (Admin Only)
- Person and ALL related data permanently deleted
- This includes:
  - All relationships (parent-child, partnerships)
  - All photos and media
  - All stories and documents
  - All life events
  - Household memberships
- **CANNOT BE UNDONE**
- Only available to administrators

## Related Data Warnings

The dialog automatically displays warnings for:

1. **Relationships**
   - Parents
   - Children
   - Spouses/Partners
   - Siblings

2. **Household Data**
   - Household memberships
   - Household permissions

3. **Media**
   - Photos
   - Videos
   - Documents

4. **Content**
   - Stories
   - Biographical notes
   - Life events

5. **Connections**
   - Calendar events
   - Shared content

## Accessibility Features

- ARIA labels on all interactive elements
- Keyboard navigation support
- Screen reader friendly
- Color contrast meets WCAG AA standards
- Focus management
- High contrast mode support
- Reduced motion support

## Security Considerations

- Hard delete option only shown to administrators
- Confirmation checkbox required
- Clear warning messages based on delete type
- Related data counts to inform user decision
- Dialog cannot be closed accidentally (disableClose option)

## Testing

See `/RushtonRoots.UnitTests/Person/PersonDeleteDialogComponentTests.cs` for unit tests.

### Test Scenarios

1. **Display Tests**
   - Person summary renders correctly
   - Related data counts display accurately
   - Delete options show based on user role

2. **Interaction Tests**
   - Confirmation checkbox enables delete button
   - Delete type selection updates warning message
   - Cancel button closes dialog without action

3. **Validation Tests**
   - Delete button disabled without confirmation
   - Transfer person ID accepts valid input
   - Form validation works correctly

4. **Cascade Delete Tests**
   - Soft delete preserves data
   - Archive moves to archive section
   - Hard delete removes all related data

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Dependencies

- @angular/material
- @angular/forms (ReactiveFormsModule)
- @angular/core
- Material Icons

## Related Components

- PersonIndexComponent (launches dialog from table actions)
- PersonDetailsComponent (launches dialog from detail view)
- ConfirmDialogComponent (simpler confirmation dialog)

## Future Enhancements

- [ ] Person autocomplete for relationship transfer
- [ ] Preview of what will be deleted before confirmation
- [ ] Undo functionality for soft deletes
- [ ] Batch delete support
- [ ] Export person data before deletion
- [ ] Email notification to affected family members
