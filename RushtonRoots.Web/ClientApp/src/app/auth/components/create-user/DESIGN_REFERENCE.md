# CreateUserComponent - Visual Design Reference

## Component Preview

The CreateUserComponent features a modern Material Design card-based interface with a gradient purple header and comprehensive form fields.

## Layout Structure

```
┌─────────────────────────────────────────────────────────┐
│  ╔═══════════════════════════════════════════════════╗  │
│  ║  👤  Create New User                              ║  │
│  ║      Create a new user account for a family member║  │
│  ╚═══════════════════════════════════════════════════╝  │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │  ✅ User created successfully!                     │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌─ Email Address ────────────────────────────────────┐ │
│  │  📧  user@example.com                              │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌─ Person ID ────────────────────────────────────────┐ │
│  │  🎫  123                                           │ │
│  └────────────────────────────────────────────────────┘ │
│  Link this user account to a person in the family tree  │
│                                                          │
│  ┌─ Password ─────────────────────────────────────────┐ │
│  │  🔒  ••••••••••••  👁️                             │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  Password Strength: Strong                               │
│  ████████████████████░░░░  80%                           │
│                                                          │
│  ┌─ Confirm Password ────────────────────────────────┐ │
│  │  🔒  ••••••••••••  👁️                             │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌─ Role ────────────────────────────────────────────┐ │
│  │  🛡️  Family Member ▼                              │ │
│  └────────────────────────────────────────────────────┘ │
│  Assign a role to define user permissions (optional)    │
│                                                          │
│  ┌─ Household ID (Optional) ─────────────────────────┐ │
│  │  🏠  10                                            │ │
│  └────────────────────────────────────────────────────┘ │
│  Optionally assign the user to a household              │
│                                                          │
│  ☑️ Send invitation email to the new user               │
│                                                          │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │  👤+ Create User│  │  ❌ Cancel      │              │
│  └─────────────────┘  └─────────────────┘              │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## Color Scheme

### Header Gradient
- Start: #667eea (Purple)
- End: #764ba2 (Deep Purple)

### Password Strength Colors
- **Weak**: #d32f2f (Red) - Score < 40
- **Medium**: #ff9800 (Orange) - Score 40-69
- **Strong**: #4caf50 (Green) - Score 70+

### Status Messages
- **Success**: Green background (#e8f5e9) with green text (#2e7d32)
- **Error**: Red background (#ffebee) with red text (#c62828)

## Responsive Behavior

### Desktop (> 768px)
- Card width: 600px max
- Two-column button layout
- Centered on screen with gradient background

### Mobile (≤ 768px)
- Full-width card (no border radius)
- Single-column button layout
- Reduced padding
- Smaller header icon and text

## Interactive States

### Form Fields
1. **Default**: Outlined with subtle border
2. **Focus**: Blue outline with elevated appearance
3. **Error**: Red border with error message below
4. **Success**: Green icon (for async validation)
5. **Disabled**: Gray background, reduced opacity

### Password Visibility Toggle
- **Hidden**: Eye with slash icon (visibility_off)
- **Visible**: Open eye icon (visibility)
- Toggles between password and text input types

### Submit Button States
1. **Enabled**: Purple background, white text
2. **Disabled**: Gray background, disabled cursor
3. **Loading**: Spinner animation, "Creating User..." text
4. **Hover**: Darker purple, slight elevation

## Validation Error Display

### Real-time Feedback
```
┌─ Email Address ────────────────────────────────────────┐
│  📧  invalid-email                                     │
└────────────────────────────────────────────────────────┘
⚠️ Please enter a valid email address
```

### Password Strength Requirements
```
Password Strength: Weak
████░░░░░░░░░░░░░░░░  25%

⚠️ Password requirements:
• At least 8 characters
• At least one uppercase letter
• At least one number
• At least one special character
```

### Form-Level Errors
```
┌────────────────────────────────────────────────────────┐
│  ⚠️ Passwords do not match                            │
└────────────────────────────────────────────────────────┘
```

## Accessibility Features

### ARIA Labels
- All form fields have `aria-label` attributes
- Error messages linked via `aria-describedby`
- Password toggle buttons have descriptive labels
- Form status announced to screen readers

### Keyboard Navigation
- Tab order: Email → Person ID → Password → Confirm → Role → Household → Checkbox → Create → Cancel
- Enter key submits form when focused on any field
- Escape key clears focus

### Focus Indicators
- Visible 2px blue outline on all interactive elements
- High contrast mode compatible
- Consistent focus style across all browsers

## Material Design Components Used

- **mat-card**: Card container with header
- **mat-form-field**: All input fields with outline appearance
- **mat-input**: Text and number inputs
- **mat-select**: Role dropdown
- **mat-checkbox**: Email invitation option
- **mat-button**: Submit and cancel buttons (raised and stroked)
- **mat-icon**: Icons throughout (email, lock, badge, home, etc.)
- **mat-progress-bar**: Password strength indicator
- **mat-spinner**: Loading state in button

## Animation Effects

### Entrance
- Card fades in from top (300ms ease-out)
- Fields appear with stagger effect (50ms delay each)

### Interactions
- Button ripple effect on click
- Field elevation on focus
- Smooth color transitions (200ms)

### Password Strength
- Progress bar fills smoothly (300ms)
- Color transitions between weak/medium/strong

### Loading
- Spinner rotates continuously
- Button text fades between states

## Icon Reference

| Icon | Purpose |
|------|---------|
| person_add | Header and submit button |
| email | Email field prefix |
| badge | Person ID field prefix |
| lock | Password field prefixes |
| visibility / visibility_off | Password toggles |
| admin_panel_settings | Role field prefix |
| home | Household ID field prefix |
| check_circle | Success message |
| error_outline | Error message |
| cancel | Cancel button |

## Form Field Hints

Subtle gray text appears below fields to provide context:

- **Email**: "The email address for the new user account"
- **Person ID**: "Link this user account to a person in the family tree"
- **Role**: "Assign a role to define user permissions (optional)"
- **Household ID**: "Optionally assign the user to a household"

## Example Scenarios

### Scenario 1: Creating Admin User
```
Email: admin@rushtonroots.com
Person ID: 42
Password: SecureAdmin123!
Confirm Password: SecureAdmin123!
Role: Admin
Household ID: 1
✅ Send invitation email
```

### Scenario 2: Creating Family Member
```
Email: john.doe@example.com
Person ID: 157
Password: MyPassword1!
Confirm Password: MyPassword1!
Role: Family Member
Household ID: (empty)
✅ Send invitation email
```

### Scenario 3: Validation Errors
```
Email: not-an-email          ← ⚠️ Invalid email format
Person ID: 0                 ← ⚠️ Must be at least 1
Password: weak               ← ⚠️ Password too weak
Confirm Password: different  ← ⚠️ Passwords don't match
```

## Browser Rendering

The component uses modern CSS features but includes fallbacks:

- **Flexbox**: For layout structure
- **CSS Grid**: For button groups
- **CSS Variables**: For theming (with fallback colors)
- **Backdrop Filter**: For card shadows (graceful degradation)

### Browser Support Matrix

| Browser | Version | Support Level |
|---------|---------|---------------|
| Chrome | 90+ | ✅ Full |
| Firefox | 88+ | ✅ Full |
| Safari | 14+ | ✅ Full |
| Edge | 90+ | ✅ Full |
| iOS Safari | 14+ | ✅ Full |
| Chrome Mobile | Current | ✅ Full |

## Performance Characteristics

- **Initial Load**: < 100ms (lazy loaded)
- **Form Interaction**: < 16ms (60fps)
- **Async Validation**: 500ms debounce
- **Submit Action**: Immediate feedback

## Future Enhancements

Visual mockups for planned features:

### Person Autocomplete (Future)
```
┌─ Person ──────────────────────────────────────────────┐
│  🔍  John D                                           │
│  ┌──────────────────────────────────────────────────┐ │
│  │  👤 John Doe (#157)                              │ │
│  │  👤 John Davis (#189)                            │ │
│  │  👤 Johnny Daniels (#203)                        │ │
│  └──────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────┘
```

### Avatar Upload (Future)
```
┌─ Profile Photo (Optional) ────────────────────────────┐
│      ┌──────────┐                                     │
│      │   📷     │   Choose file or drag here          │
│      └──────────┘                                     │
└────────────────────────────────────────────────────────┘
```

---

**Design System**: Material Design 3  
**Component Type**: Form / Card  
**Complexity**: Medium  
**Accessibility Level**: WCAG 2.1 AA  
**Mobile Optimized**: Yes
