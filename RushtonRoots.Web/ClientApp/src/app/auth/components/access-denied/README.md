# Access Denied Component

## Overview

The **AccessDeniedComponent** displays a clear, user-friendly access denied page when users attempt to access resources they don't have permission for. It provides helpful guidance and options for requesting access.

## Features

- **Clear Messaging**: Displays a prominent access denied message with Material Design icons
- **Optional Reason**: Shows specific reason for denial if provided
- **Resource Information**: Displays the name of the denied resource
- **Request Access**: Button to send an access request to administrators
- **Contact Administrator**: Mailto link for direct contact (if email provided)
- **Navigation Options**: 
  - "Return to Home" button
  - "Go Back" button using browser history
- **Success Feedback**: Shows confirmation message when access request is sent
- **Helpful Information**: "What can you do?" section with actionable steps
- **Help Section**: Guidance on getting assistance
- **Responsive Design**: Works seamlessly on all screen sizes
- **Material Design**: Consistent with the application's design system

## Usage

### In Razor View (AccessDenied.cshtml)

```html
<app-access-denied 
    reason="@ViewData["Reason"]?.ToString()"
    resource-name="@ViewData["ResourceName"]?.ToString()"
    contact-email="@ViewData["ContactEmail"]?.ToString()">
</app-access-denied>
```

### In Angular Template

```html
<app-access-denied 
    [reason]="denialReason"
    [resourceName]="resourceName"
    [contactEmail]="adminEmail"
    (requestAccess)="handleRequestAccess()">
</app-access-denied>
```

## Component API

### Inputs

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `reason` | string | '' | Optional reason for access denial. If not provided, displays default message |
| `resourceName` | string | '' | Optional name of the resource that was denied |
| `contactEmail` | string | '' | Optional administrator email address for contact |

### Outputs

| Event | Type | Description |
|-------|------|-------------|
| `requestAccess` | EventEmitter<void> | Emitted when user clicks "Request Access" button |

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `requestingAccess` | boolean | Loading state for access request |
| `requestSuccess` | boolean | Success state after access request sent |
| `displayReason` | string (getter) | Returns custom reason or default message |
| `hasContactInfo` | boolean (getter) | True if contact email is provided |
| `mailtoLink` | string (getter) | Mailto link with pre-filled subject and body |

### Methods

| Method | Parameters | Returns | Description |
|--------|------------|---------|-------------|
| `onRequestAccess()` | none | void | Handles request access button click |
| `goBack()` | none | void | Navigates to previous page using browser history |

## Controller Setup

To pass data to the component, set ViewData in your controller:

```csharp
public IActionResult AccessDenied(string reason = null, string resourceName = null)
{
    ViewData["Reason"] = reason;
    ViewData["ResourceName"] = resourceName;
    ViewData["ContactEmail"] = "admin@rushtonroots.com"; // Or get from config
    return View();
}
```

## Scenarios

### Scenario 1: Basic Access Denied
User tries to access a resource without permission:

```csharp
return RedirectToAction("AccessDenied", "Account");
```

### Scenario 2: Access Denied with Reason
User tries to access admin-only feature:

```csharp
return RedirectToAction("AccessDenied", "Account", new { 
    reason = "This feature requires administrator privileges",
    resourceName = "User Management"
});
```

### Scenario 3: Access Denied with Contact Info
User tries to access household they're not a member of:

```csharp
var household = await _householdService.GetByIdAsync(id);
return RedirectToAction("AccessDenied", "Account", new { 
    reason = "You are not a member of this household",
    resourceName = household.Name
});
```

## Styling

The component uses a red gradient background (`#dc143c` to `#8b0000`) to visually distinguish it from other pages. The styling is fully responsive and follows Material Design principles.

### Color Scheme

- **Warning Icon**: `#f44336` (Material Red)
- **Lock Icon**: `#f44336` (Material Red)
- **Actions Section**: `#e3f2fd` background with `#2196f3` border (Material Blue)
- **Success Message**: `#e8f5e9` background with `#4caf50` border (Material Green)
- **Help Section**: `#fff3e0` background with `#ff9800` border (Material Orange)

## Accessibility

- **ARIA Roles**: All interactive elements have appropriate ARIA labels
- **Keyboard Navigation**: All buttons and links are keyboard accessible
- **Screen Reader Support**: Proper heading hierarchy and semantic HTML
- **Color Contrast**: Meets WCAG 2.1 AA standards
- **Focus Indicators**: Clear focus states on all interactive elements

## Testing

### Unit Tests (TODO)

```typescript
describe('AccessDeniedComponent', () => {
  it('should display default reason when no reason provided', () => {
    // Test implementation
  });

  it('should display custom reason when provided', () => {
    // Test implementation
  });

  it('should show contact button when email provided', () => {
    // Test implementation
  });

  it('should emit requestAccess event on button click', () => {
    // Test implementation
  });

  it('should navigate back on goBack()', () => {
    // Test implementation
  });
});
```

### Manual Testing Scenarios

1. **No Inputs**: Component displays with default messages
2. **With Reason**: Custom reason appears in message
3. **With Resource Name**: Resource name displays prominently
4. **With Contact Email**: Contact administrator button appears
5. **Request Access**: Button shows loading state, then success message
6. **Navigation**: Home and Back buttons work correctly
7. **Responsive**: Test on mobile, tablet, and desktop sizes
8. **Accessibility**: Test with keyboard navigation and screen reader

## Integration with Authorization

This component works with ASP.NET Core authorization:

```csharp
[Authorize(Roles = "Admin")]
public IActionResult AdminDashboard()
{
    // Only admins can access
    return View();
}
```

If unauthorized, the user is redirected to:
```
/Account/AccessDenied
```

## Future Enhancements

- [ ] Implement actual API call for requesting access (currently simulated)
- [ ] Add unit tests using Jasmine/Karma
- [ ] Track access request history
- [ ] Email notification to administrators
- [ ] Customizable redirect after access granted
- [ ] Integration with permission management system
- [ ] Audit logging for access denial events

## Related Components

- **LoginComponent**: Authentication entry point
- **UserProfileComponent**: User settings and preferences
- **CreateUserComponent**: Admin user creation (for granting access)

## Files

- `access-denied.component.ts`: Component logic
- `access-denied.component.html`: Template
- `access-denied.component.scss`: Styles
- `AccessDenied.cshtml`: Razor view wrapper

## Version History

- **1.0.0** (Phase 1.4): Initial implementation with all core features

## References

- [Material Design Icons](https://fonts.google.com/icons)
- [Angular Material](https://material.angular.io/)
- [ASP.NET Core Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/)
