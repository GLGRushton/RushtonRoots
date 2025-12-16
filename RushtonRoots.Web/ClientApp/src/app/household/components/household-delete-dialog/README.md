# HouseholdDeleteDialogComponent

## Overview

A comprehensive Material Design dialog component for confirming household deletion with impact analysis and multiple deletion options. This component ensures users understand the consequences of deleting a household and provides flexible options for handling the deletion.

## Features

### Core Features
- **Household Summary Display**: Shows household name, anchor person, member count, and creation date
- **Impact Analysis**: Displays comprehensive list of all related data that will be affected
- **Multiple Deletion Types**:
  - **Soft Delete** (Recommended): Marks household as deleted, can be restored by admin
  - **Archive**: Preserves household for historical purposes, members lose active access
  - **Hard Delete** (Admin Only): Permanently deletes all data - cannot be undone
- **Member Notifications**: Optional email notifications to all household members
- **Safety Confirmations**: Required confirmation checkbox before deletion
- **Admin Controls**: Hard delete option only visible to administrators
- **Responsive Design**: Fully functional on mobile, tablet, and desktop devices
- **Accessibility**: WCAG 2.1 AA compliant with keyboard navigation support

### Impact Warnings

The component displays warnings about:
- **Members**: Number of members who will lose household access
- **Events**: Events associated with the household
- **Shared Media**: Photos, videos, and documents
- **Permissions**: Member permission settings
- **Total Impact**: Aggregate count of all affected items

### Deletion Options

#### 1. Soft Delete (Default)
- Marks household as deleted in database
- Hidden from most views
- Can be restored by administrator
- Members lose access but can be re-added
- **Use Case**: Temporary removal or cleanup

#### 2. Archive
- Moves household to archive view
- Data preserved for historical purposes
- Members lose active access
- Visible only in archive views
- **Use Case**: Historical preservation, inactive households

#### 3. Hard Delete (Admin Only)
- Permanently deletes household and all related data
- Cannot be undone
- All members lose access
- All associated data removed
- **Use Case**: Complete removal, GDPR compliance

## Usage

### Opening the Dialog

```typescript
import { MatDialog } from '@angular/material/dialog';
import { HouseholdDeleteDialogComponent } from './path/to/household-delete-dialog.component';
import { HouseholdDeleteDialogData } from './path/to/household-delete.model';

constructor(private dialog: MatDialog) {}

deleteHousehold(householdId: number) {
  const dialogData: HouseholdDeleteDialogData = {
    householdId: 123,
    householdName: 'Smith Family',
    anchorPersonName: 'John Smith',
    anchorPersonId: 456,
    memberCount: 5,
    createdDate: new Date('2020-01-15'),
    isAdmin: this.authService.isAdmin(),
    relatedData: {
      members: 5,
      events: 12,
      sharedMedia: 45,
      documents: 8,
      permissions: 15
    }
  };

  const dialogRef = this.dialog.open(HouseholdDeleteDialogComponent, {
    width: '600px',
    maxWidth: '90vw',
    data: dialogData,
    disableClose: true // Prevent accidental closure
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      // User confirmed deletion
      this.handleHouseholdDeletion(householdId, result);
    }
  });
}
```

### Handling the Result

```typescript
handleHouseholdDeletion(householdId: number, options: HouseholdDeleteOptions) {
  console.log(`Delete Type: ${options.deleteType}`);
  console.log(`Notify Members: ${options.notifyMembers}`);
  console.log(`Confirmed: ${options.confirmed}`);

  // Call backend API based on delete type
  switch (options.deleteType) {
    case 'soft':
      this.householdService.softDelete(householdId, options.notifyMembers).subscribe(
        result => this.handleSuccess(result),
        error => this.handleError(error)
      );
      break;
    
    case 'archive':
      this.householdService.archive(householdId, options.notifyMembers).subscribe(
        result => this.handleSuccess(result),
        error => this.handleError(error)
      );
      break;
    
    case 'hard':
      this.householdService.hardDelete(householdId, options.notifyMembers).subscribe(
        result => this.handleSuccess(result),
        error => this.handleError(error)
      );
      break;
  }
}
```

### Using as Angular Element

```typescript
// In app.module.ts
import { HouseholdDeleteDialogComponent } from './household/components/household-delete-dialog/household-delete-dialog.component';

@NgModule({
  declarations: [HouseholdDeleteDialogComponent],
  // ... other config
})
export class AppModule {
  constructor(private injector: Injector) {
    const householdDeleteElement = createCustomElement(HouseholdDeleteDialogComponent, { injector });
    customElements.define('app-household-delete-dialog', householdDeleteElement);
  }
}
```

```html
<!-- In Razor view (Delete.cshtml) -->
<app-household-delete-dialog
  household-id="@Model.Id"
  household-name="@Model.HouseholdName"
  anchor-person-name="@Model.AnchorPersonName"
  anchor-person-id="@Model.AnchorPersonId"
  member-count="@Model.MemberCount"
  created-date="@Model.CreatedDateTime.ToString("o")"
  is-admin="@User.IsInRole("Admin")"
  related-data='@Json.Serialize(relatedData)'>
</app-household-delete-dialog>
```

## TypeScript Interfaces

### HouseholdDeleteDialogData

```typescript
interface HouseholdDeleteDialogData {
  householdId: number;
  householdName: string;
  anchorPersonName?: string;
  anchorPersonId?: number;
  memberCount: number;
  createdDate?: Date | string;
  relatedData: HouseholdRelatedData;
  isAdmin?: boolean;
}
```

### HouseholdRelatedData

```typescript
interface HouseholdRelatedData {
  members: number;
  events: number;
  sharedMedia: number;
  documents: number;
  permissions: number;
}
```

### HouseholdDeleteOptions

```typescript
interface HouseholdDeleteOptions {
  deleteType: 'soft' | 'hard' | 'archive';
  notifyMembers: boolean;
  confirmed: boolean;
}
```

### HouseholdDeleteResult

```typescript
interface HouseholdDeleteResult {
  success: boolean;
  deleteType: 'soft' | 'hard' | 'archive';
  message?: string;
  error?: string;
}
```

## Component Methods

### Public Methods

- `formatDate(date?: Date | string): string` - Formats dates for display
- `hasRelatedData(): boolean` - Checks if there are any related data items
- `getTotalRelatedItems(): number` - Returns total count of affected items
- `getWarningMessage(): string` - Returns appropriate warning message based on delete type
- `getDeleteButtonText(): string` - Returns button text based on delete type
- `getDeleteButtonColor(): 'primary' | 'accent' | 'warn'` - Returns button color based on delete type
- `onCancel(): void` - Handles cancel action
- `onDelete(): void` - Handles delete confirmation

## Styling

The component uses Material Design with custom SCSS styling:

- **Color Scheme**: Material colors (red for warnings, blue for info)
- **Responsive Layout**: Mobile-first design with breakpoints
- **Accessibility**: High contrast mode support, reduced motion support
- **Dark Mode**: Automatic theme adaptation
- **Visual Hierarchy**: Clear separation of sections with icons

## Accessibility Features

- **ARIA Labels**: All interactive elements properly labeled
- **Keyboard Navigation**: Full keyboard support (Tab, Enter, Escape)
- **Screen Reader Support**: Proper announcements for all content
- **Focus Management**: Initial focus on cancel button (safe default)
- **Color Contrast**: WCAG AA compliant (4.5:1 minimum)
- **Error Messages**: Associated with form fields for screen readers
- **High Contrast Mode**: Enhanced borders and colors
- **Reduced Motion**: Transitions disabled when requested

## Testing

### Unit Tests

```typescript
describe('HouseholdDeleteDialogComponent', () => {
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display household summary correctly', () => {
    expect(fixture.nativeElement.querySelector('.household-name').textContent)
      .toContain('Smith Family');
  });

  it('should show hard delete option only for admins', () => {
    component.isAdmin = true;
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[value="hard"]')).toBeTruthy();
  });

  it('should disable delete button until confirmed', () => {
    const deleteButton = fixture.nativeElement.querySelector('.delete-button');
    expect(deleteButton.disabled).toBeTruthy();
    
    component.deleteForm.patchValue({ confirmationCheckbox: true });
    fixture.detectChanges();
    expect(deleteButton.disabled).toBeFalsy();
  });

  it('should calculate total related items correctly', () => {
    expect(component.getTotalRelatedItems()).toBe(85); // 5+12+45+8+15
  });
});
```

### Integration Tests

Test the complete delete workflow:
1. Open dialog with household data
2. Verify summary display
3. Select delete type
4. Toggle member notification
5. Check confirmation checkbox
6. Click delete button
7. Verify result returned to parent

## Backend Integration

### Required Backend Endpoints

```csharp
// HouseholdController.cs

[HttpPost]
public async Task<IActionResult> SoftDelete(int id, bool notifyMembers = false)
{
    var household = await _householdService.GetByIdAsync(id);
    if (household == null) return NotFound();
    
    household.IsDeleted = true;
    household.DeletedDateTime = DateTime.UtcNow;
    await _householdService.UpdateAsync(household);
    
    if (notifyMembers)
        await _notificationService.NotifyMembersOfDeletion(household);
    
    return Ok(new { success = true, message = "Household soft deleted" });
}

[HttpPost]
public async Task<IActionResult> Archive(int id, bool notifyMembers = false)
{
    var household = await _householdService.GetByIdAsync(id);
    if (household == null) return NotFound();
    
    household.IsArchived = true;
    household.ArchivedDateTime = DateTime.UtcNow;
    await _householdService.UpdateAsync(household);
    
    if (notifyMembers)
        await _notificationService.NotifyMembersOfArchival(household);
    
    return Ok(new { success = true, message = "Household archived" });
}

[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> HardDelete(int id, bool notifyMembers = false)
{
    var household = await _householdService.GetByIdAsync(id);
    if (household == null) return NotFound();
    
    if (notifyMembers)
        await _notificationService.NotifyMembersOfDeletion(household);
    
    await _householdService.HardDeleteAsync(id);
    
    return Ok(new { success = true, message = "Household permanently deleted" });
}
```

## Future Enhancements

- [ ] Add undo functionality for soft deletes
- [ ] Implement cascading delete preview
- [ ] Add export household data before deletion
- [ ] Support for bulk household deletion
- [ ] Integration with audit log for compliance
- [ ] Customizable notification email templates
- [ ] Scheduled deletion (delete after X days)
- [ ] Delete confirmation via email link

## Related Components

- **PersonDeleteDialogComponent**: Similar pattern for person deletion
- **HouseholdDetailsComponent**: Parent component that opens this dialog
- **HouseholdIndexComponent**: List view with delete actions
- **HouseholdMembersComponent**: Shows members who will be affected

## Version History

- **v1.0.0** (2025-12-16): Initial implementation
  - Core deletion functionality
  - Three deletion types (soft, archive, hard)
  - Member notification option
  - Admin-only hard delete
  - Comprehensive impact analysis
  - Full accessibility support

## License

Part of the RushtonRoots application. Internal use only.
