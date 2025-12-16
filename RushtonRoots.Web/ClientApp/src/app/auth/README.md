# Authentication Components - Phases 6.1 & 6.2

This directory contains the authentication and account management components for RushtonRoots, built with Angular 19 and Material Design.

## Phase 6.1: Login & Registration ✅

### Components

#### 1. LoginComponent (`app-login`)

Modern login form with professional UI and comprehensive features.

**Features:**
- Email and password input with validation
- "Remember Me" checkbox
- Password visibility toggle
- Social login buttons (Google, Facebook, Microsoft) - prepared for future implementation
- Loading states during authentication
- Error message display
- Links to forgot password and registration pages
- Fully responsive design

**Usage in Razor Views:**

```html
<app-login 
  returnurl="/Dashboard"
  (loginSubmit)="handleLogin($event)"
  (socialLogin)="handleSocialLogin($event)">
</app-login>

<script>
  const loginElement = document.querySelector('app-login');
  
  loginElement.addEventListener('loginSubmit', (event) => {
    const formData = event.detail;
    // formData contains: { email, password, rememberMe }
    console.log('Login submitted:', formData);
    
    // Submit to server
    fetch('/Account/Login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
      if (data.success) {
        window.location.href = formData.returnUrl || '/';
      } else {
        loginElement.setAttribute('errormessage', data.error);
      }
    });
  });
  
  loginElement.addEventListener('socialLogin', (event) => {
    const providerId = event.detail;
    console.log('Social login with:', providerId);
    // Redirect to social login endpoint when implemented
  });
</script>
```

**TypeScript Interface:**

```typescript
interface LoginFormData {
  email: string;
  password: string;
  rememberMe: boolean;
}
```

---

### 2. ForgotPasswordComponent (`app-forgot-password`)

Password reset request form with clean UI and success states.

**Features:**
- Email input with validation
- Loading state during request
- Success message with instructions
- Error handling
- Link back to login page
- Fully responsive design

**Usage in Razor Views:**

```html
<app-forgot-password 
  (forgotPasswordSubmit)="handleForgotPassword($event)">
</app-forgot-password>

<script>
  const forgotPasswordElement = document.querySelector('app-forgot-password');
  
  forgotPasswordElement.addEventListener('forgotPasswordSubmit', (event) => {
    const formData = event.detail;
    // formData contains: { email }
    console.log('Forgot password submitted:', formData);
    
    // Submit to server
    fetch('/Account/ForgotPassword', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
      if (data.success) {
        forgotPasswordElement.setAttribute('successmessage', 
          'Password reset instructions have been sent to your email.');
      } else {
        forgotPasswordElement.setAttribute('errormessage', data.error);
      }
    });
  });
</script>
```

**TypeScript Interface:**

```typescript
interface ForgotPasswordFormData {
  email: string;
}
```

---

### 3. ResetPasswordComponent (`app-reset-password`)

Comprehensive password reset form with strength indicator.

**Features:**
- Email and new password inputs
- Password confirmation field
- Real-time password strength indicator
  - Visual progress bar with color coding
  - Strength levels: Weak, Fair, Good, Strong
  - Feedback suggestions for improvement
- Password visibility toggles for both fields
- Password requirements checklist
- Form validation including password matching
- Loading state during reset
- Fully responsive design

**Usage in Razor Views:**

```html
<app-reset-password 
  code="@Model.Code"
  (resetPasswordSubmit)="handleResetPassword($event)">
</app-reset-password>

<script>
  const resetPasswordElement = document.querySelector('app-reset-password');
  
  resetPasswordElement.addEventListener('resetPasswordSubmit', (event) => {
    const formData = event.detail;
    // formData contains: { email, password, confirmPassword, code }
    console.log('Reset password submitted:', formData);
    
    // Submit to server
    fetch('/Account/ResetPassword', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })
    .then(response => response.json())
    .then(data => {
      if (data.success) {
        window.location.href = '/Account/Login?message=PasswordReset';
      } else {
        resetPasswordElement.setAttribute('errormessage', data.error);
      }
    });
  });
</script>
```

**TypeScript Interface:**

```typescript
interface ResetPasswordFormData {
  email: string;
  password: string;
  confirmPassword: string;
  code: string;
}
```

---

## Phase 6.2: User Profile & Settings ✅

### Components

#### 4. UserProfileComponent (`app-user-profile`)

Comprehensive user profile and settings management with tabbed interface.

**Features:**
- Profile header with avatar, name, and completeness indicator
- Tabbed interface for different settings sections:
  - Profile: View and edit personal information
  - Notifications: Manage notification preferences
  - Privacy: Control privacy settings
  - Connected Accounts: Manage social account connections
  - Security: Change password and delete account
- Avatar upload with preview and validation
- Profile completeness calculation with suggestions
- Edit mode with comprehensive form validation
- Responsive design for all screen sizes

**Usage in Razor Views:**

```html
<app-user-profile 
  [userProfile]="profileData"
  [notificationPreferences]="notifications"
  [privacySettings]="privacy"
  [connectedAccounts]="accounts"
  (profileUpdate)="handleProfileUpdate($event)"
  (avatarUpload)="handleAvatarUpload($event)"
  (notificationsUpdate)="handleNotificationsUpdate($event)"
  (privacyUpdate)="handlePrivacyUpdate($event)"
  (accountConnect)="handleAccountConnect($event)"
  (accountDisconnect)="handleAccountDisconnect($event)"
  (accountDelete)="handleAccountDelete($event)">
</app-user-profile>

<script>
  const userProfileElement = document.querySelector('app-user-profile');
  
  // Example profile data
  const profileData = {
    id: '123',
    email: 'john@example.com',
    firstName: 'John',
    lastName: 'Doe',
    displayName: 'John Doe',
    bio: 'Family genealogy enthusiast',
    avatarUrl: '/images/avatars/john.jpg',
    phoneNumber: '+1 234 567 8900',
    dateOfBirth: new Date('1980-01-15'),
    location: 'Seattle, WA',
    website: 'https://example.com',
    createdAt: new Date('2023-01-01'),
    updatedAt: new Date('2025-12-01')
  };
  
  userProfileElement.setAttribute('userProfile', JSON.stringify(profileData));
  
  // Handle profile updates
  userProfileElement.addEventListener('profileUpdate', (event) => {
    const formData = event.detail;
    // Submit to server
    fetch('/Account/UpdateProfile', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    });
  });
  
  // Handle avatar upload
  userProfileElement.addEventListener('avatarUpload', (event) => {
    const file = event.detail;
    const formData = new FormData();
    formData.append('avatar', file);
    
    fetch('/Account/UploadAvatar', {
      method: 'POST',
      body: formData
    });
  });
</script>
```

**TypeScript Interfaces:**

```typescript
interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  displayName: string;
  bio?: string;
  avatarUrl?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  location?: string;
  website?: string;
  createdAt: Date;
  updatedAt: Date;
}

interface ProfileEditFormData {
  firstName: string;
  lastName: string;
  displayName: string;
  bio?: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  location?: string;
  website?: string;
}
```

---

#### 5. NotificationPreferencesComponent (`app-notification-preferences`)

Manage notification settings across email, push, and in-app channels.

**Features:**
- Email notification preferences (family updates, comments, mentions, digests)
- Push notification preferences with master toggle
- In-app notification preferences
- Organized by notification type
- Save and reset to defaults functionality
- Real-time change tracking

**Standalone Usage:**

```html
<app-notification-preferences 
  [preferences]="notificationPreferences"
  (preferencesUpdate)="handleUpdate($event)">
</app-notification-preferences>
```

**TypeScript Interface:**

```typescript
interface NotificationPreferences {
  emailNotifications: {
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
    weeklyDigest: boolean;
    monthlyNewsletter: boolean;
  };
  pushNotifications: {
    enabled: boolean;
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
  };
  inAppNotifications: {
    familyUpdates: boolean;
    newMembers: boolean;
    comments: boolean;
    mentions: boolean;
  };
}
```

---

#### 6. PrivacySettingsComponent (`app-privacy-settings`)

Control privacy and visibility settings.

**Features:**
- Profile visibility settings (Public, Family Only, Private)
- Information visibility toggles (email, phone, DOB, location)
- Discoverability settings (family search, search engine indexing)
- Radio group for visibility selection with descriptions
- Save and reset functionality

**Standalone Usage:**

```html
<app-privacy-settings 
  [settings]="privacySettings"
  (settingsUpdate)="handleUpdate($event)">
</app-privacy-settings>
```

**TypeScript Interface:**

```typescript
interface PrivacySettings {
  profileVisibility: 'public' | 'family' | 'private';
  showEmail: boolean;
  showPhoneNumber: boolean;
  showDateOfBirth: boolean;
  showLocation: boolean;
  allowSearchEngineIndexing: boolean;
  allowFamilyMemberSearch: boolean;
}
```

---

#### 7. ConnectedAccountsComponent (`app-connected-accounts`)

Manage connected social accounts for easy sign-in.

**Features:**
- Connected account cards for Google, Facebook, Microsoft
- Connection status indicators
- Account information display (email, connected date, last used)
- Connect/disconnect functionality
- Provider-specific icons and colors
- Help section explaining benefits

**Standalone Usage:**

```html
<app-connected-accounts 
  [connectedAccounts]="accounts"
  (accountConnect)="handleConnect($event)"
  (accountDisconnect)="handleDisconnect($event)">
</app-connected-accounts>
```

**TypeScript Interface:**

```typescript
interface ConnectedAccount {
  id: string;
  provider: string; // 'google', 'facebook', 'microsoft'
  providerAccountId: string;
  email: string;
  connectedAt: Date;
  lastUsed?: Date;
  status: 'active' | 'revoked' | 'expired';
}
```

---

#### 8. AccountDeletionComponent (`app-account-deletion`)

Handle account security and deletion flow.

**Features:**
- Security actions section (change password, 2FA, active sessions)
- Danger zone with account deletion
- Deletion form with reason selection
- Confirmation fields (email and password)
- Optional feedback textarea
- Delete immediately checkbox (vs 30-day grace period)
- Warning messages and confirmation flow

**Standalone Usage:**

```html
<app-account-deletion 
  [canDelete]="true"
  (accountDelete)="handleDelete($event)">
</app-account-deletion>
```

**TypeScript Interface:**

```typescript
interface AccountDeletionRequest {
  reason: string; // 'privacy', 'not_useful', 'alternative', 'temporary', 'other'
  feedback?: string;
  confirmEmail: string;
  confirmPassword: string;
  transferDataTo?: string;
  deleteImmediately: boolean;
}
```

---

## Profile Completeness Calculation

The UserProfileComponent includes a profile completeness calculator that evaluates:

**Weighted Fields:**
- First Name: 15%
- Last Name: 15%
- Display Name: 10%
- Bio: 15%
- Avatar: 15%
- Phone Number: 10%
- Date of Birth: 10%
- Location: 10%

**Features:**
- Visual progress bar
- Missing fields list
- Contextual suggestions for improvement
- Color-coded indicator (red < 50%, yellow 50-75%, green > 75%)

---

## Password Strength Calculation

The ResetPasswordComponent includes a sophisticated password strength calculator that checks:

1. **Length**: Minimum 8 characters (12+ for extra points)
2. **Uppercase Letters**: At least one (A-Z)
3. **Lowercase Letters**: At least one (a-z)
4. **Numbers**: At least one (0-9)
5. **Special Characters**: At least one (!@#$%^&* etc.)

**Strength Levels:**
- **Weak** (0-39%): Red - Password needs significant improvement
- **Fair** (40-59%): Orange - Password meets minimum requirements
- **Good** (60-79%): Light Green - Strong password
- **Strong** (80-100%): Green - Excellent password

---

## Material Design Components Used

All components leverage Angular Material for a consistent, professional UI:

- **MatCard**: Form containers with elevation
- **MatFormField**: Input field wrappers
- **MatInput**: Text and password inputs
- **MatCheckbox**: "Remember Me" toggle
- **MatIcon**: Visual icons (email, lock, visibility, etc.)
- **MatButton**: Primary and stroked buttons
- **MatProgressBar**: Password strength indicator
- **MatProgressSpinner**: Loading states
- **MatTooltip**: Helpful hints

---

## Styling and Theming

All components follow the RushtonRoots design system:

- **Primary Color**: #2e7d32 (Forest Green)
- **Accent Color**: #66bb6a (Light Green)
- **Gradient Background**: Linear gradient from light gray to light green
- **Card Elevation**: Box shadow for depth
- **Responsive Breakpoints**: Mobile-first approach
- **Animations**: Smooth transitions on hover and focus

---

## Form Validation

All forms include comprehensive validation:

- **Required Fields**: All inputs are required
- **Email Validation**: Proper email format check
- **Password Length**: Minimum 8 characters
- **Password Strength**: Weak passwords are rejected
- **Password Matching**: Confirmation must match
- **Real-time Feedback**: Errors shown as user types
- **Touched State**: Errors only shown after field interaction

---

## Accessibility

Components are built with accessibility in mind:

- **ARIA Labels**: Proper labels for screen readers
- **Keyboard Navigation**: Full keyboard support
- **Focus States**: Clear visual focus indicators
- **Error Messages**: Descriptive error messages
- **Button States**: Disabled states during loading
- **Semantic HTML**: Proper heading hierarchy

---

## Responsive Design

All components are fully responsive:

- **Desktop** (>600px): Full card width with centered layout
- **Mobile** (<600px): Compact layout with adjusted spacing
- **Flexible Grid**: Components adapt to container width
- **Touch Targets**: Minimum 44x44px for mobile

---

## Integration with ASP.NET Core Identity

These components are designed to work seamlessly with ASP.NET Core Identity:

1. **LoginComponent**: Posts to `/Account/Login` endpoint
2. **ForgotPasswordComponent**: Posts to `/Account/ForgotPassword` endpoint
3. **ResetPasswordComponent**: Posts to `/Account/ResetPassword` endpoint

Ensure your Account controller methods accept JSON and return appropriate responses.

---

## File Structure

```
auth/
├── auth.module.ts                           # AuthModule definition
├── models/
│   ├── auth.model.ts                        # TypeScript interfaces for auth
│   └── user-profile.model.ts                # TypeScript interfaces for profile
└── components/
    ├── login/
    │   ├── login.component.ts               # Component logic
    │   ├── login.component.html             # Template
    │   └── login.component.scss             # Styles
    ├── forgot-password/
    │   ├── forgot-password.component.ts     # Component logic
    │   ├── forgot-password.component.html   # Template
    │   └── forgot-password.component.scss   # Styles
    ├── reset-password/
    │   ├── reset-password.component.ts      # Component logic
    │   ├── reset-password.component.html    # Template
    │   └── reset-password.component.scss    # Styles
    ├── user-profile/
    │   ├── user-profile.component.ts        # Component logic
    │   ├── user-profile.component.html      # Template
    │   └── user-profile.component.scss      # Styles
    ├── notification-preferences/
    │   ├── notification-preferences.component.ts
    │   ├── notification-preferences.component.html
    │   └── notification-preferences.component.scss
    ├── privacy-settings/
    │   ├── privacy-settings.component.ts
    │   ├── privacy-settings.component.html
    │   └── privacy-settings.component.scss
    ├── connected-accounts/
    │   ├── connected-accounts.component.ts
    │   ├── connected-accounts.component.html
    │   └── connected-accounts.component.scss
    └── account-deletion/
        ├── account-deletion.component.ts
        ├── account-deletion.component.html
        └── account-deletion.component.scss
```

---

## Future Enhancements

### Social Login Integration

Social login buttons are already in place for:
- Google OAuth
- Facebook Login
- Microsoft Account

To enable, update the `enabled` flag in `SOCIAL_LOGIN_PROVIDERS` and implement the backend OAuth flow.

### Two-Factor Authentication

Consider adding:
- 2FA setup during registration
- 2FA verification during login
- Backup codes management

### Additional Features

**Phase 6.1 Components:**
- Password strength meter on registration
- Account lockout after failed attempts
- Remember device option
- Password history to prevent reuse
- Email verification before password reset

**Phase 6.2 Components:**
- Avatar cropping and rotation tool
- Export profile data (GDPR compliance)
- Download account data archive
- Advanced notification scheduling
- Notification frequency controls
- Privacy audit log
- Data portability between platforms
- Two-factor authentication settings

---

## Testing

Test the components by:

1. Building the Angular application: `npm run build`
2. Running the .NET application
3. Navigating to the Account pages
4. Testing form validation
5. Testing error states
6. Testing success flows
7. Testing responsive behavior on mobile

---

## Support

For questions or issues with these components, refer to:
- Angular Material documentation: https://material.angular.io/
- Angular Forms documentation: https://angular.io/guide/forms
- ASP.NET Core Identity documentation: https://docs.microsoft.com/aspnet/core/security/authentication/identity

---

**Last Updated**: December 2025  
**Phases**: 6.1 (Login & Registration) & 6.2 (User Profile & Settings)  
**Status**: ✅ Both Phases Complete
