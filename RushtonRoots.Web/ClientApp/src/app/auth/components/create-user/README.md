# CreateUserComponent

## Overview

The CreateUserComponent is an admin-only Angular component for creating new user accounts in the RushtonRoots family tree application. It features a comprehensive form with Material Design styling, real-time validation, and password strength checking.

## Features

- **Admin-Only Access**: Designed for administrators to create user accounts
- **Material Design**: Full integration with Angular Material components
- **Form Validation**: Comprehensive reactive form validation including:
  - Email format validation
  - Async email uniqueness check
  - Password strength validation
  - Password confirmation matching
  - Person ID validation
- **Password Strength Indicator**: Real-time visual feedback with color-coded progress bar
- **Role Selection**: Dropdown to assign user roles (Admin, HouseholdAdmin, FamilyMember)
- **Person Linkage**: Link user account to a person in the family tree
- **Household Assignment**: Optional household ID field
- **Email Invitation**: Option to send invitation email to new user
- **Responsive Design**: Mobile-friendly layout
- **Accessibility**: ARIA labels, keyboard navigation, screen reader support

## Component Structure

### Files

- `create-user.component.ts` - Component logic and form handling
- `create-user.component.html` - Template with Material Design form
- `create-user.component.scss` - Styles with responsive design
- `create-user.component.spec.ts` - Unit tests

### Dependencies

- Angular 19+
- Angular Material
- Reactive Forms Module
- RxJS for async validation

## Usage

### In Angular Application

```typescript
import { CreateUserComponent } from './auth/components/create-user/create-user.component';

// In template
<app-create-user 
  [successMessage]="successMsg"
  [errorMessage]="errorMsg"
  (userCreateSubmit)="handleUserCreate($event)">
</app-create-user>
```

### As Angular Element (in Razor Views)

```html
<app-create-user
    success-message="@TempData["SuccessMessage"]"
    error-message="@ViewData["ErrorMessage"]">
</app-create-user>
```

## Form Fields

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Email | text | Yes | Email format, Async uniqueness check |
| Person ID | number | Yes | Min value: 1 |
| Password | password | Yes | Min 8 chars, Strength check |
| Confirm Password | password | Yes | Must match password |
| Role | select | No | Admin, HouseholdAdmin, FamilyMember |
| Household ID | number | No | Min value: 1 |
| Send Invitation | checkbox | No | Default: true |

## Password Strength Calculation

The password strength is calculated based on:

- **Length**: 8+ characters (25 points), 12+ characters (+15 points)
- **Uppercase Letters**: +20 points
- **Lowercase Letters**: +20 points
- **Numbers**: +20 points
- **Special Characters**: +20 points

**Strength Levels**:
- Weak: < 40 points (Red)
- Medium: 40-69 points (Orange)
- Strong: 70+ points (Green)

## Events

### Output Events

- `userCreateSubmit`: Emitted when form is submitted with valid data
  ```typescript
  interface CreateUserFormData {
    email: string;
    personId: number;
    password: string;
    confirmPassword: string;
    role?: string;
    householdId?: number | null;
    sendInvitationEmail: boolean;
  }
  ```

## Testing

### Unit Tests

The component includes comprehensive unit tests covering:

- Form initialization and default values
- All validation rules
- Password strength calculation
- Password visibility toggles
- Form submission (valid and invalid cases)
- Error message generation
- Cancel/reset functionality
- Password strength display

### Running Tests

```bash
cd RushtonRoots.Web/ClientApp
npm test
```

**Note**: Test infrastructure requires karma-jasmine and related packages to be installed.

### Test Coverage Goals

- Component initialization: ✅
- Form validation: ✅
- Password strength: ✅
- User interactions: ✅
- Error handling: ✅
- Accessibility: ⏳ (requires manual testing)

## Admin Authorization Directives

### AdminOnlyDirective

Shows/hides content based on admin roles.

```html
<!-- Default: Shows for Admin or HouseholdAdmin -->
<div *appAdminOnly>Admin content</div>

<!-- Specific role -->
<div *appAdminOnly="'HouseholdAdmin'">Household admin content</div>

<!-- Multiple roles -->
<div *appAdminOnly="['Admin', 'SuperAdmin']">Multiple admin types</div>
```

### RoleGuardDirective

Flexible role-based content visibility.

```html
<!-- Single role -->
<div *appRoleGuard="'Admin'">Admin only</div>

<!-- Any of multiple roles (default strategy) -->
<div *appRoleGuard="['Admin', 'Editor']; strategy: 'any'">
  Admin OR Editor
</div>

<!-- All roles required -->
<div *appRoleGuard="['Admin', 'Editor']; strategy: 'all'">
  Admin AND Editor
</div>
```

**Note**: Current implementation uses placeholder role checking. In production, integrate with actual authentication service.

## Integration with Backend

The component emits form data via the `userCreateSubmit` event. The Razor view (`CreateUser.cshtml`) includes JavaScript to:

1. Listen for the event
2. Create a form element
3. Add anti-forgery token
4. Map form fields to C# model properties
5. Submit to `/Account/CreateUser` endpoint

### Backend Model

```csharp
public class CreateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public int PersonId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    public string? Role { get; set; }
}
```

## Accessibility Features

- Proper ARIA labels on all form fields
- Keyboard navigation support
- Focus management
- Screen reader announcements for errors
- High contrast mode support
- Reduced motion support
- Color contrast meets WCAG AA standards

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+
- Mobile browsers (iOS Safari, Chrome Mobile)

## Future Enhancements

- [ ] Person autocomplete search instead of manual ID entry
- [ ] Household autocomplete search
- [ ] Real-time email validation via API
- [ ] Avatar upload for new user
- [ ] Bulk user import feature
- [ ] User invitation email preview
- [ ] Integration with actual authentication service for role checking

## Related Components

- LoginComponent
- UserProfileComponent
- AdminOnlyDirective
- RoleGuardDirective

## Version History

- **v1.0.0** (December 2024)
  - Initial implementation
  - Material Design integration
  - Form validation
  - Password strength indicator
  - Admin directives
  - Angular Elements registration
  - Unit tests
