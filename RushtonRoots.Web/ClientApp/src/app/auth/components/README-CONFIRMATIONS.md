# Password Confirmation and Email Verification Components

This directory contains the Phase 1.2 confirmation components for the authentication flow.

## Components

### ForgotPasswordConfirmationComponent
**Path**: `/auth/components/forgot-password-confirmation/`

Displays a confirmation screen after a user requests a password reset.

**Features**:
- Success message with email sent indicator
- Email address display
- Helpful information section (check spam, wait, etc.)
- Resend email button with loading states
- Link back to login page
- Fully responsive Material Design layout

**Usage**:
```html
<app-forgot-password-confirmation 
    email="user@example.com"
    (resendEmail)="handleResendEmail($event)">
</app-forgot-password-confirmation>
```

**Inputs**:
- `email` (string): The email address where the reset link was sent

**Outputs**:
- `resendEmail` (EventEmitter<string>): Emitted when user requests to resend the email

---

### ResetPasswordConfirmationComponent
**Path**: `/auth/components/reset-password-confirmation/`

Displays a confirmation screen after successful password reset with auto-redirect.

**Features**:
- Password reset success message
- Auto-redirect countdown timer (5 seconds)
- Manual "Go to Login Now" button
- Security tips section
- Countdown animation with pulse effect
- Fully responsive Material Design layout

**Usage**:
```html
<app-reset-password-confirmation></app-reset-password-confirmation>
```

**Inputs**: None

**Outputs**: None

**Behavior**:
- Automatically redirects to `/Account/Login` after 5 seconds
- User can click "Go to Login Now" to redirect immediately
- Countdown is cleared on component destruction

---

### ConfirmEmailComponent
**Path**: `/auth/components/confirm-email/`

Displays email verification status with success/error states.

**Features**:
- Email verification status display (success/error)
- Success state with "What's Next?" guidance
- Error states for invalid/expired tokens
- Detailed error messages
- Resend confirmation email functionality
- Links to login and home page
- Fully responsive Material Design layout

**Usage**:
```html
<app-confirm-email 
    [status]="{ success: true, message: 'Email confirmed successfully' }"
    email="user@example.com"
    (resendEmail)="handleResendConfirmation($event)">
</app-confirm-email>
```

**Inputs**:
- `status` (EmailConfirmationStatus | null): Confirmation result from server
  - `success` (boolean): Whether confirmation was successful
  - `message` (string): Message to display
  - `tokenValid?` (boolean): Whether the token was valid
- `email` (string): User's email address

**Outputs**:
- `resendEmail` (EventEmitter<string>): Emitted when user requests to resend confirmation

**EmailConfirmationStatus Interface**:
```typescript
export interface EmailConfirmationStatus {
  success: boolean;
  message: string;
  tokenValid?: boolean;
}
```

---

## Angular Elements Registration

All three components are registered as Angular Elements in `app.module.ts`:

```typescript
safeDefine('app-forgot-password-confirmation', ForgotPasswordConfirmationComponent);
safeDefine('app-reset-password-confirmation', ResetPasswordConfirmationComponent);
safeDefine('app-confirm-email', ConfirmEmailComponent);
```

## Razor View Integration

### ForgotPasswordConfirmation.cshtml
```html
@{
    ViewData["Title"] = "Forgot Password Confirmation";
}

<app-forgot-password-confirmation 
    email="@ViewData["Email"]">
</app-forgot-password-confirmation>
```

### ResetPasswordConfirmation.cshtml
```html
@{
    ViewData["Title"] = "Reset Password Confirmation";
}

<app-reset-password-confirmation></app-reset-password-confirmation>
```

### ConfirmEmail.cshtml
```html
@{
    ViewData["Title"] = "Email Confirmed";
}

<app-confirm-email 
    [status]="{ success: true, message: 'Your email has been confirmed successfully.' }"
    email="@ViewData["Email"]">
</app-confirm-email>
```

## Styling

All components use:
- Angular Material components (mat-card, mat-icon, mat-button, mat-spinner)
- Responsive design (mobile-first approach)
- Gradient backgrounds for visual appeal
- Consistent color scheme with Material Design
- Smooth animations and transitions
- WCAG 2.1 AA accessibility compliance

## Testing

Unit tests should cover:
- Component initialization
- Input/Output bindings
- User interactions (button clicks, resend email)
- Auto-redirect timer (ResetPasswordConfirmation)
- Status state changes (ConfirmEmail)
- Responsive layout rendering

## Dependencies

- Angular 19
- Angular Material 19
- Angular Router (for navigation)
- RxJS (for observables and timers)
