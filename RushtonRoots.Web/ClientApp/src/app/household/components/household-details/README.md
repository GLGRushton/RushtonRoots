# Household Details & Members Components - Phase 4.2

## Overview

This directory contains the Phase 4.2 implementation of Household Details & Members components for the RushtonRoots application. These components provide comprehensive household management functionality including member management, permissions, settings, and activity tracking.

## Components

### 1. HouseholdDetailsComponent

**Purpose**: Main component for displaying detailed household information with tabbed interface.

**Features**:
- Tabbed interface (Overview, Members, Settings, Activity)
- Household information display
- Edit-in-place description field
- Privacy indicator with icons and chips
- Action menu (Edit, Delete)
- Responsive design

**Inputs**:
- `household: HouseholdDetails` - The household data
- `members: HouseholdMemberDetails[]` - List of household members
- `activityEvents: HouseholdActivityEvent[]` - Activity timeline events
- `canEdit: boolean` - Edit permission flag
- `canDelete: boolean` - Delete permission flag
- `canManageMembers: boolean` - Member management permission flag
- `canEditSettings: boolean` - Settings edit permission flag
- `currentUserId?: number` - Current user ID for member highlighting
- `initialTab: number` - Initial tab index (default: 0)

**Outputs**:
- `editClicked: EventEmitter<number>` - Emits household ID when edit is clicked
- `deleteClicked: EventEmitter<number>` - Emits household ID when delete is clicked
- `memberActionClicked: EventEmitter<MemberActionEvent>` - Emits member action events
- `inviteMemberClicked: EventEmitter<void>` - Emits when invite member is clicked
- `settingsUpdated: EventEmitter<any>` - Emits when settings are updated
- `anchorPersonClicked: EventEmitter<number>` - Emits person ID when anchor person is clicked

**Usage**:
```html
<app-household-details
  [household]="householdData"
  [members]="membersList"
  [activityEvents]="activityList"
  [canEdit]="true"
  [canManageMembers]="true"
  (editClicked)="onEditHousehold($event)"
  (memberActionClicked)="onMemberAction($event)">
</app-household-details>
```

### 2. HouseholdMembersComponent

**Purpose**: Display and manage household members with role badges and actions.

**Features**:
- Member cards with avatars and role badges
- Separate sections for active, invited, and inactive members
- Member action menu (view, change role, remove)
- Invite member button
- Current user indicator
- Permission-based visibility
- Empty state

**Inputs**:
- `members: HouseholdMemberDetails[]` - List of members
- `canManage: boolean` - Member management permission
- `currentUserId?: number` - Current user ID

**Outputs**:
- `memberAction: EventEmitter<MemberActionEvent>` - Emits member action events
- `inviteMember: EventEmitter<void>` - Emits when invite is clicked

**Usage**:
```html
<app-household-members
  [members]="members"
  [canManage]="true"
  [currentUserId]="123"
  (memberAction)="handleMemberAction($event)"
  (inviteMember)="openInviteDialog()">
</app-household-members>
```

### 3. MemberInviteDialogComponent

**Purpose**: Dialog for inviting new members to the household.

**Features**:
- Email input with validation
- Optional name fields
- Role selection dropdown
- Personal message field
- Role permissions information panel
- Form validation

**Constructor Data**:
- `householdName: string` - Name of the household for dialog title

**Returns**:
- `MemberInvitation` object when invitation is sent
- `undefined` when dialog is cancelled

**Usage**:
```typescript
const dialogRef = this.dialog.open(MemberInviteDialogComponent, {
  data: { householdName: 'Smith Family' }
});

dialogRef.afterClosed().subscribe((invitation: MemberInvitation) => {
  if (invitation) {
    // Send invitation
  }
});
```

### 4. HouseholdSettingsComponent

**Purpose**: Manage household settings including privacy and permissions.

**Features**:
- Privacy settings (Public, Family Only, Private)
- Member permission toggles
- Notification preferences
- Unsaved changes tracking
- Save and reset functionality
- Read-only mode

**Inputs**:
- `household: HouseholdDetails` - Household data
- `canEdit: boolean` - Edit permission flag

**Outputs**:
- `settingsUpdated: EventEmitter<any>` - Emits updated settings

**Usage**:
```html
<app-household-settings
  [household]="householdData"
  [canEdit]="true"
  (settingsUpdated)="onSettingsUpdate($event)">
</app-household-settings>
```

### 5. HouseholdActivityTimelineComponent

**Purpose**: Display chronological timeline of household activity events.

**Features**:
- Vertical timeline with event markers
- Event type icons and color coding
- User attribution with avatars
- Expandable event details
- Relative timestamp formatting
- Empty state

**Inputs**:
- `events: HouseholdActivityEvent[]` - List of activity events
- `householdId: number` - Household ID

**Usage**:
```html
<app-household-activity-timeline
  [events]="activityEvents"
  [householdId]="123">
</app-household-activity-timeline>
```

## TypeScript Models

### HouseholdDetails
```typescript
interface HouseholdDetails {
  id: number;
  householdName: string;
  description?: string;
  anchorPersonId?: number;
  anchorPersonName?: string;
  anchorPersonPhotoUrl?: string;
  memberCount: number;
  createdDateTime: Date | string;
  updatedDateTime: Date | string;
  createdByUserId?: number;
  createdByUserName?: string;
  privacy: 'Public' | 'FamilyOnly' | 'Private';
  allowMemberInvites: boolean;
  members?: HouseholdMemberDetails[];
  settings?: HouseholdSettings;
}
```

### HouseholdMemberDetails
```typescript
interface HouseholdMemberDetails extends HouseholdMember {
  email?: string;
  phone?: string;
  role: HouseholdRole;
  permissions: HouseholdPermissions;
  joinedDate: Date | string;
  invitedBy?: string;
  status: 'Active' | 'Invited' | 'Inactive';
  lastActive?: Date | string;
}
```

### HouseholdRole
```typescript
type HouseholdRole = 'Owner' | 'Admin' | 'Member' | 'Viewer';
```

### Role Configurations
```typescript
const HOUSEHOLD_ROLES: RoleConfig[] = [
  { role: 'Owner', label: 'Owner', description: 'Full control over household and all settings', color: 'primary', icon: 'star' },
  { role: 'Admin', label: 'Admin', description: 'Can manage members and edit household information', color: 'accent', icon: 'admin_panel_settings' },
  { role: 'Member', label: 'Member', description: 'Can view and contribute to household content', color: 'default', icon: 'person' },
  { role: 'Viewer', label: 'Viewer', description: 'Can only view household information', color: 'default', icon: 'visibility' }
];
```

## Material Components Used

- **MatCard** - Card containers
- **MatButton, MatIconButton** - Action buttons
- **MatIcon** - Material icons
- **MatMenu** - Context menus
- **MatBadge** - Member count badges
- **MatTooltip** - Tooltips
- **MatFormField, MatInput, MatSelect** - Form inputs
- **MatTabs** - Tabbed interface
- **MatChipsModule** - Status and role chips
- **MatDialog** - Invitation dialog
- **MatSlideToggle** - Settings toggles
- **MatRadioButton** - Privacy selection
- **MatDivider** - Visual separators
- **MatProgressSpinner** - Loading states

## Angular Elements Registration

All components are registered as custom elements in `app.module.ts`:

```typescript
customElements.define('app-household-details', householdDetailsElement);
customElements.define('app-household-members', householdMembersElement);
customElements.define('app-member-invite-dialog', memberInviteDialogElement);
customElements.define('app-household-settings', householdSettingsElement);
customElements.define('app-household-activity-timeline', householdActivityTimelineElement);
```

## Backend Integration

These components expect the following API endpoints:

### GET /api/households/{id}
Returns HouseholdDetails object

### GET /api/households/{id}/members
Returns array of HouseholdMemberDetails

### GET /api/households/{id}/activity
Returns array of HouseholdActivityEvent

### POST /api/households/{id}/members/invite
Accepts MemberInvitation object

### PUT /api/households/{id}/settings
Accepts HouseholdSettings object

### PATCH /api/households/{id}/members/{memberId}/role
Accepts role update data

### DELETE /api/households/{id}/members/{memberId}
Removes member from household

## Responsive Design

All components are fully responsive with mobile-first design:

- **Desktop (>768px)**: Full card grid, horizontal layouts
- **Mobile (<768px)**: Single column, vertical stacking, full-width buttons

## Accessibility

- Proper ARIA labels on all interactive elements
- Keyboard navigation support
- Screen reader friendly
- Sufficient color contrast
- Focus indicators on all inputs

## Testing Recommendations

1. **Unit Tests**: Test component logic, event emissions, data transformations
2. **Integration Tests**: Test component interactions, API calls
3. **E2E Tests**: Test complete user workflows (invite member, change role, update settings)
4. **Visual Tests**: Test responsive layouts at different breakpoints
5. **Accessibility Tests**: Run automated accessibility tests with Axe

## Future Enhancements

1. Bulk member operations
2. Member search and filtering
3. Advanced permission matrix
4. Activity filtering and search
5. Export member list
6. Email templates customization
7. Member status notifications
8. Audit log for security-sensitive actions

## Related Documentation

- [Phase 4.1 - Household Index & Cards](../household-card/README.md)
- [UI Design Plan](../../../../docs/UI_DesignPlan.md)
- [Angular Material Documentation](https://material.angular.io/)
