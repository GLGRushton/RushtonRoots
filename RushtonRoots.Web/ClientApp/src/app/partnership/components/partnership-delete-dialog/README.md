# PartnershipDeleteDialogComponent

## Overview

The `PartnershipDeleteDialogComponent` provides a comprehensive confirmation dialog for partnership deletion with three action options: soft delete, end partnership (recommended), and hard delete (admin only).

## Features

### Core Functionality
- **Partnership Summary Display**: Shows both partners with photos, names, dates, and partnership details
- **Three Action Options**:
  1. **End Partnership (Recommended)**: Mark partnership with an end date, preserve historical record
  2. **Soft Delete**: Mark as deleted, can be restored by administrator
  3. **Hard Delete (Admin Only)**: Permanently delete all data
- **Impact Warnings**: Clear warnings about what will be affected by each action type
- **Related Data Display**: Shows counts of children, shared events, photos, stories, and documents
- **Confirmation Checkbox**: Requires explicit user confirmation
- **Optional Child Transfer**: Ability to transfer children to another partnership
- **End Date Picker**: Required field when selecting "End Partnership" option

### User Interface
- Material Design card-based layout
- Responsive design (mobile, tablet, desktop)
- Clear visual indicators for different delete types
- Color-coded warnings (orange for caution, red for danger)
- Partner cards with photos or avatar placeholders
- Heart icon separator between partners (gray if already ended)
- Detailed partnership information (type, dates, location)

### Accessibility
- WCAG 2.1 AA compliant
- Keyboard navigation support
- Screen reader friendly
- High contrast mode support
- Reduced motion support for animations
- ARIA labels on all interactive elements
- Clear error messages

## Usage

### As Angular Component (in TypeScript)

```typescript
import { PartnershipDeleteDialogComponent } from './partnership/components/partnership-delete-dialog/partnership-delete-dialog.component';

// In your component
this.showDeleteDialog(partnershipData);

handleDeleteConfirmed(options: PartnershipDeleteOptions) {
  if (options.deleteType === 'end') {
    // Handle end partnership with options.endDate
    this.partnershipService.endPartnership(this.partnershipId, options.endDate);
  } else if (options.deleteType === 'soft') {
    // Handle soft delete
    this.partnershipService.softDelete(this.partnershipId);
  } else if (options.deleteType === 'hard') {
    // Handle hard delete (admin only)
    this.partnershipService.hardDelete(this.partnershipId);
  }
  
  // Optionally transfer children
  if (options.transferChildrenTo) {
    this.partnershipService.transferChildren(this.partnershipId, options.transferChildrenTo);
  }
}
```

### As Angular Element (in Razor View)

```html
<app-partnership-delete-dialog
  partnership-id="@Model.Id"
  person-a-id="@Model.PersonAId"
  person-a-name="@Model.PersonAName"
  person-a-photo-url="@Model.PersonAPhotoUrl"
  person-a-birth-date="@Model.PersonABirthDate?.ToString("o")"
  person-a-death-date="@Model.PersonADeathDate?.ToString("o")"
  person-a-is-deceased="@Model.PersonAIsDeceased.ToString().ToLower()"
  person-b-id="@Model.PersonBId"
  person-b-name="@Model.PersonBName"
  person-b-photo-url="@Model.PersonBPhotoUrl"
  person-b-birth-date="@Model.PersonBBirthDate?.ToString("o")"
  person-b-death-date="@Model.PersonBDeathDate?.ToString("o")"
  person-b-is-deceased="@Model.PersonBIsDeceased.ToString().ToLower()"
  partnership-type="@Model.PartnershipType"
  start-date="@Model.StartDate?.ToString("o")"
  end-date="@Model.EndDate?.ToString("o")"
  location="@Model.Location"
  related-data='@Html.Raw(Json.Serialize(Model.RelatedData))'
  is-admin="@User.IsInRole("Admin").ToString().ToLower()">
</app-partnership-delete-dialog>

<script>
  document.querySelector('app-partnership-delete-dialog')
    .addEventListener('deleteConfirmed', (event) => {
      const options = event.detail;
      // Submit form with delete options
      const form = document.createElement('form');
      form.method = 'POST';
      form.action = '/Partnership/Delete/@Model.Id';
      
      // Add delete type
      const deleteTypeInput = document.createElement('input');
      deleteTypeInput.type = 'hidden';
      deleteTypeInput.name = 'deleteType';
      deleteTypeInput.value = options.deleteType;
      form.appendChild(deleteTypeInput);
      
      // Add end date if present
      if (options.endDate) {
        const endDateInput = document.createElement('input');
        endDateInput.type = 'hidden';
        endDateInput.name = 'endDate';
        endDateInput.value = options.endDate;
        form.appendChild(endDateInput);
      }
      
      // Add anti-forgery token
      const tokenInput = document.createElement('input');
      tokenInput.type = 'hidden';
      tokenInput.name = '__RequestVerificationToken';
      tokenInput.value = '@Html.AntiForgeryToken()';
      form.appendChild(tokenInput);
      
      document.body.appendChild(form);
      form.submit();
    });
    
  document.querySelector('app-partnership-delete-dialog')
    .addEventListener('deleteCancelled', () => {
      window.location.href = '/Partnership/Index';
    });
</script>
```

## Inputs

| Input | Type | Required | Description |
|-------|------|----------|-------------|
| `partnership-id` | number | Yes | Partnership ID |
| `person-a-id` | number | Yes | First partner's person ID |
| `person-a-name` | string | Yes | First partner's full name |
| `person-a-photo-url` | string | No | First partner's photo URL |
| `person-a-birth-date` | Date/string | No | First partner's birth date |
| `person-a-death-date` | Date/string | No | First partner's death date |
| `person-a-is-deceased` | boolean | Yes | Whether first partner is deceased |
| `person-b-id` | number | Yes | Second partner's person ID |
| `person-b-name` | string | Yes | Second partner's full name |
| `person-b-photo-url` | string | No | Second partner's photo URL |
| `person-b-birth-date` | Date/string | No | Second partner's birth date |
| `person-b-death-date` | Date/string | No | Second partner's death date |
| `person-b-is-deceased` | boolean | Yes | Whether second partner is deceased |
| `partnership-type` | string | Yes | Type of partnership (Married, Partnered, etc.) |
| `start-date` | Date/string | No | Partnership start date |
| `end-date` | Date/string | No | Partnership end date (if already ended) |
| `location` | string | No | Partnership location |
| `related-data` | string (JSON) | No | Related data counts (children, events, photos, etc.) |
| `is-admin` | boolean | Yes | Whether current user is an admin (shows hard delete option) |

## Outputs

| Output | Type | Description |
|--------|------|-------------|
| `deleteConfirmed` | PartnershipDeleteOptions | Emitted when user confirms deletion |
| `deleteCancelled` | void | Emitted when user cancels |

## Data Models

### PartnershipDeleteOptions

```typescript
interface PartnershipDeleteOptions {
  deleteType: 'soft' | 'hard' | 'end';
  endDate?: Date; // Required if deleteType is 'end'
  transferChildrenTo?: number; // Partnership ID to transfer children to
  confirmed: boolean;
}
```

### Related Data Structure

```typescript
{
  children: number;
  sharedEvents: number;
  photos: number;
  stories: number;
  documents: number;
}
```

## Action Types

### 1. End Partnership (Recommended)
- **Use Case**: Partnership has ended (divorce, separation, death)
- **Effect**: Sets `EndDate` field, preserves all historical data
- **Reversible**: Yes, admin can remove end date
- **Impact**: Children retain parent partnership reference, all data preserved
- **UI**: Primary button (blue), requires end date selection

### 2. Soft Delete
- **Use Case**: Accidental partnership creation, temporary removal
- **Effect**: Sets `IsDeleted` flag to true, hides from normal views
- **Reversible**: Yes, admin can restore
- **Impact**: Hidden from views but data preserved in database
- **UI**: Accent button (purple)

### 3. Hard Delete (Admin Only)
- **Use Case**: Data cleanup, GDPR compliance, incorrect data
- **Effect**: Permanently deletes partnership and optionally related data
- **Reversible**: No, permanent deletion
- **Impact**: Children lose parent partnership reference, all data deleted
- **UI**: Warn button (red), requires admin role

## Styling

The component uses Material Design with custom SCSS:
- Card-based layout with partner information
- Color-coded action buttons
- Warning cards with orange/red borders
- Responsive grid for partner display
- Mobile-optimized layout (stacked cards)
- Accessibility features (high contrast, reduced motion support)

## Dependencies

- Angular Material (MatCard, MatButton, MatRadioButton, MatCheckbox, MatIcon, MatList, MatFormField, MatDatepicker)
- Angular Reactive Forms
- Partnership models (`partnership-delete.model.ts`)

## Testing

### Unit Tests (To Be Implemented)

```typescript
describe('PartnershipDeleteDialogComponent', () => {
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should default to "end partnership" option', () => {
    expect(component.selectedDeleteType).toBe('end');
  });

  it('should require end date when end partnership selected', () => {
    component.deleteForm.get('deleteType')?.setValue('end');
    expect(component.deleteForm.get('endDate')?.hasError('required')).toBeTruthy();
  });

  it('should show hard delete only for admins', () => {
    component.isAdmin = false;
    fixture.detectChanges();
    const hardDeleteOption = fixture.nativeElement.querySelector('[value="hard"]');
    expect(hardDeleteOption).toBeNull();
  });

  it('should emit deleteConfirmed with correct options', () => {
    spyOn(component.deleteConfirmed, 'emit');
    component.deleteForm.patchValue({
      deleteType: 'end',
      endDate: new Date(),
      confirmationCheckbox: true
    });
    component.onDelete();
    expect(component.deleteConfirmed.emit).toHaveBeenCalled();
  });
});
```

## Related Components

- `PersonDeleteDialogComponent` - Similar pattern for person deletion
- `HouseholdDeleteDialogComponent` - Similar pattern for household deletion
- `PartnershipFormComponent` - For creating/editing partnerships
- `PartnershipDetailsComponent` - For viewing partnership details

## Backend Integration Requirements

### Controller Actions Required

1. **Soft Delete**
   ```csharp
   [HttpPost]
   public async Task<IActionResult> SoftDelete(int id)
   {
       var partnership = await _partnershipService.GetByIdAsync(id);
       partnership.IsDeleted = true;
       partnership.DeletedDateTime = DateTime.UtcNow;
       await _partnershipService.UpdateAsync(partnership);
       return RedirectToAction("Index");
   }
   ```

2. **End Partnership**
   ```csharp
   [HttpPost]
   public async Task<IActionResult> EndPartnership(int id, DateTime endDate)
   {
       var partnership = await _partnershipService.GetByIdAsync(id);
       partnership.EndDate = endDate;
       await _partnershipService.UpdateAsync(partnership);
       return RedirectToAction("Index");
   }
   ```

3. **Hard Delete**
   ```csharp
   [HttpPost]
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> HardDelete(int id)
   {
       await _partnershipService.DeleteAsync(id); // Cascade delete
       return RedirectToAction("Index");
   }
   ```

### Database Schema Requirements

```sql
ALTER TABLE Partnerships
ADD IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedDateTime DATETIME2 NULL;
```

## Migration Notes

- Partnership entity needs `IsDeleted` and `DeletedDateTime` fields for soft delete
- Existing partnerships may not have `EndDate` set
- Query filters should exclude `IsDeleted = true` partnerships by default
- Admin views should show soft-deleted partnerships with restore option

## Known Issues / Future Enhancements

- [ ] Add partnership restoration feature for admins
- [ ] Add audit log for deletion actions
- [ ] Add email notifications to both partners when partnership is ended/deleted
- [ ] Add "disputed" status as alternative to deletion (similar to ParentChild)
- [ ] Add relationship transfer wizard for complex scenarios
- [ ] Add preview of cascade delete impacts before confirmation

## Version History

- **v1.0.0** (December 2024) - Initial implementation with soft delete, end partnership, and hard delete options
