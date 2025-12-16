# HouseholdFormComponent

Angular component for creating and editing households with Material Design.

## Overview

The `HouseholdFormComponent` provides a comprehensive form interface for managing household information, including basic details, anchor person selection, member management, privacy settings, and permission configuration.

## Features

- ✅ Basic information section (name, description)
- ✅ Anchor person selection with autocomplete
- ✅ Initial members selection (multiple person autocomplete)
- ✅ Privacy settings (public, family only, private)
- ✅ Household permission defaults
- ✅ Comprehensive form validation
- ✅ Create/Update mode support
- ✅ Material Design UI
- ✅ Responsive layout
- ✅ Accessibility features

## Usage

### In Angular Template

```html
<app-household-form
  [householdId]="householdId"
  [initialData]="householdData"
  [peopleList]="people"
  (formSubmit)="onFormSubmit($event)"
  (formCancel)="onFormCancel()">
</app-household-form>
```

### As Angular Element (in Razor view)

```html
<app-household-form
  household-id="123"
  people-list='[{"id":1,"fullName":"John Doe",...}]'
  initial-data='{"householdName":"Smith Family",...}'>
</app-household-form>

<script>
  const householdForm = document.querySelector('app-household-form');
  
  householdForm.addEventListener('formSubmit', (event) => {
    const formData = event.detail;
    // Submit to backend
    fetch('/Household/Create', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': document.querySelector('[name="__RequestVerificationToken"]').value
      },
      body: JSON.stringify(formData)
    })
    .then(response => {
      if (response.ok) {
        window.location.href = '/Household/Index';
      }
    });
  });
  
  householdForm.addEventListener('formCancel', () => {
    window.location.href = '/Household/Index';
  });
</script>
```

## Inputs

| Property | Type | Description | Required |
|----------|------|-------------|----------|
| `householdId` | `number` | ID of household being edited (for edit mode) | No |
| `initialData` | `Partial<HouseholdFormData>` | Initial form data for edit mode | No |
| `peopleList` | `PersonOption[]` | List of all people for selection | Yes |

## Outputs

| Event | Type | Description |
|-------|------|-------------|
| `formSubmit` | `EventEmitter<HouseholdFormData>` | Emitted when form is submitted with valid data |
| `formCancel` | `EventEmitter<void>` | Emitted when user cancels the form |

## Data Models

### HouseholdFormData

```typescript
interface HouseholdFormData {
  id?: number;
  householdName: string;
  description?: string;
  anchorPersonId?: number;
  anchorPersonName?: string;
  initialMemberIds?: number[];
  initialMembers?: HouseholdFormMember[];
  privacyLevel: 'public' | 'family' | 'private';
  allowMemberInvites: boolean;
  allowMemberEdits: boolean;
  allowMemberUploads: boolean;
}
```

### PersonOption

```typescript
interface PersonOption {
  id: number;
  fullName: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: Date | string;
  photoUrl?: string;
  householdName?: string;
}
```

### HouseholdFormMember

```typescript
interface HouseholdFormMember {
  personId: number;
  fullName: string;
  photoUrl?: string;
  role: 'admin' | 'editor' | 'contributor' | 'viewer';
  canInvite: boolean;
  canEdit: boolean;
  canUpload: boolean;
}
```

## Form Sections

### 1. Basic Information
- **Household Name**: Required field, max 200 characters
- **Description**: Optional field, max 2000 characters with character counter

### 2. Anchor Person
- **Search**: Autocomplete search with debouncing (300ms)
- **Display**: Shows selected person with avatar and details
- **Clear**: Option to remove selected anchor person

### 3. Initial Members (Create mode only)
- **Search**: Autocomplete to find and add members
- **List**: Shows all selected members with role selection
- **Roles**: 
  - Admin: Full control over household
  - Editor: Can edit and manage members
  - Contributor: Can add content
  - Viewer: Read-only access
- **Remove**: Option to remove members from selection

### 4. Privacy Settings
- **Public**: Visible to everyone, including non-family members
- **Family Only**: Visible only to registered family members
- **Private**: Visible only to household members

### 5. Permission Defaults
- **Allow Member Invites**: Members can invite others
- **Allow Member Edits**: Members can edit household information
- **Allow Media Uploads**: Members can upload photos and documents

## Form Validation

- Household name is required
- Household name cannot exceed 200 characters
- Description cannot exceed 2000 characters
- Form must be valid before submission
- Warns user about unsaved changes when canceling

## Features in Detail

### Autocomplete

Both anchor person and member selection use autocomplete with:
- Debounced search (300ms) for performance
- Avatar/placeholder display
- Person details (name, birth date, household)
- Limit of 10 results per search
- Filters out already selected members and anchor person

### Member Management

- Add members with default "contributor" role
- Change member role with immediate permission updates
- Remove members from selection
- Visual feedback (snackbar) for all actions
- Avatar or initials placeholder for each member

### Privacy Options

Visual radio button group with:
- Icon for each privacy level
- Clear description of each option
- Highlighted when selected

### Responsive Design

- Desktop: Full-width form with optimal spacing
- Tablet: Adjusted layouts and spacing
- Mobile: 
  - Stacked buttons
  - Smaller avatars
  - Full-width fields
  - Touch-friendly controls

## Accessibility

- ✅ ARIA labels on all interactive elements
- ✅ Keyboard navigation support
- ✅ Screen reader friendly
- ✅ High contrast mode support
- ✅ Reduced motion support
- ✅ Color contrast meets WCAG AA standards
- ✅ Focus indicators visible
- ✅ Error messages associated with fields

## Material Components Used

- `mat-card` - Container and member/person cards
- `mat-form-field` - All form inputs
- `mat-input` - Text inputs and textareas
- `mat-autocomplete` - Person/member selection
- `mat-radio-group`, `mat-radio-button` - Privacy settings
- `mat-checkbox` - Permission settings
- `mat-select` - Role selection
- `mat-button`, `mat-raised-button` - Action buttons
- `mat-icon` - Icons throughout
- `mat-spinner` - Loading state
- `mat-snack-bar` - Notifications

## Styling

Professional Material Design styling with:
- Card-based layout
- Color-coded sections
- Avatar displays
- Responsive breakpoints (768px, 480px)
- Smooth transitions
- Gradient avatars for persons without photos

## Event Flow

1. User fills out form
2. Validates in real-time
3. Clicks submit button
4. Component emits `formSubmit` event with data
5. Parent handles submission (e.g., POST to backend)
6. On success, navigate to appropriate page

OR

1. User clicks cancel
2. Confirms if form is dirty
3. Component emits `formCancel` event
4. Parent handles navigation

## Integration with Backend

The form data can be mapped to:
- `CreateHouseholdRequest` for new households
- `UpdateHouseholdRequest` for existing households

Example mapping:
```typescript
const createRequest = {
  HouseholdName: formData.householdName,
  AnchorPersonId: formData.anchorPersonId,
  // Additional backend-specific properties
};
```

## Future Enhancements

- [ ] Inline validation for duplicate household names
- [ ] Drag-and-drop member reordering
- [ ] Bulk member import from CSV
- [ ] Photo upload for household
- [ ] Advanced permission customization per member
- [ ] Email invitation preview
- [ ] Household templates

## Related Components

- `HouseholdDetailsComponent` - Displays household details
- `HouseholdMembersComponent` - Manages household members
- `HouseholdIndexComponent` - Lists all households
- `MemberInviteDialogComponent` - Sends member invitations

## File Structure

```
household-form/
├── household-form.component.ts        # Component logic
├── household-form.component.html      # Template
├── household-form.component.scss      # Styles
└── README.md                          # This file
```

## Dependencies

- Angular 19
- Angular Material 19
- RxJS 7
- Household models (`household-form.model.ts`)

## Testing

Unit tests should cover:
- Component initialization
- Form validation
- Anchor person selection
- Member addition/removal
- Role changes
- Privacy settings
- Permission toggles
- Form submission
- Form cancellation

## Notes

- Component uses reactive forms for validation
- Autocomplete has 300ms debounce for performance
- Member search excludes already selected members and anchor person
- Privacy level defaults to "family"
- All permissions default to true/enabled
- Form dirty state is tracked for cancel confirmation

## Version History

- **1.0.0** (December 2025): Initial implementation
  - Complete form with all required features
  - Material Design UI
  - Responsive layout
  - Accessibility features
