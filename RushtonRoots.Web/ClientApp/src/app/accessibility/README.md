# Accessibility Module

## Overview

The Accessibility Module provides comprehensive WCAG 2.1 AA compliant accessibility features for the RushtonRoots application. This module includes components, services, and styles to ensure the application is usable by everyone, including people with disabilities.

## Features

### 🎯 WCAG 2.1 AA Compliance
- Meets Web Content Accessibility Guidelines (WCAG) 2.1 Level AA
- Automated testing with Axe accessibility engine
- Manual testing with screen readers (NVDA, JAWS, VoiceOver)
- Color contrast ratios meet or exceed 4.5:1 for normal text

### ⌨️ Keyboard Navigation
- Full keyboard support for all interactive elements
- Custom keyboard shortcuts for common actions
- Focus management and trap for modals
- Skip navigation links
- Tab order optimization

### 🔊 Screen Reader Support
- Proper semantic HTML structure
- Comprehensive ARIA labels and descriptions
- Landmarks for easy navigation
- Live regions for dynamic content
- Alternative text for all images

### 👁️ Visual Accessibility
- High-contrast focus indicators
- Supports prefers-contrast: high media query
- Supports prefers-color-scheme: dark media query
- Supports prefers-reduced-motion media query
- Minimum 44x44px touch targets

## Components

### SkipNavigationComponent
Provides skip links that appear on keyboard focus, allowing users to skip to:
- Main content (Alt+S)
- Navigation (Alt+N)
- Footer

**Usage:**
```html
<app-skip-navigation></app-skip-navigation>
```

### KeyboardShortcutsDialogComponent
Displays a dialog showing all available keyboard shortcuts in the application.

**Available Shortcuts:**
- `Alt + S` - Skip to main content
- `Alt + N` - Skip to navigation
- `Alt + Shift + A` - Open accessibility menu
- `/` - Focus search box
- `Shift + ?` - Show keyboard shortcuts help

**Usage:**
```typescript
import { MatDialog } from '@angular/material/dialog';
import { KeyboardShortcutsDialogComponent } from './accessibility/components/keyboard-shortcuts-dialog/keyboard-shortcuts-dialog.component';

constructor(private dialog: MatDialog) {}

showKeyboardShortcuts() {
  this.dialog.open(KeyboardShortcutsDialogComponent);
}
```

### AccessibilityStatementComponent
Comprehensive accessibility statement page documenting:
- Compliance standards (WCAG 2.1 AA, Section 508, ADA)
- Accessibility features
- Known issues and workarounds
- Testing methodology
- Contact information for accessibility feedback

**Usage:**
```html
<app-accessibility-statement></app-accessibility-statement>
```

## Services

### AccessibilityTestingService
Automated accessibility testing using the Axe engine.

**Methods:**
```typescript
// Run audit on entire document
const results = await accessibilityTestingService.runAudit();

// Run audit on specific element
const results = await accessibilityTestingService.runAuditOnElement(element);

// Get violations
const violations = accessibilityTestingService.getViolations(results);

// Get accessibility score (0-100)
const score = accessibilityTestingService.getScore(results);

// Log results to console
accessibilityTestingService.logResults(results);

// Get violation summary by impact
const summary = accessibilityTestingService.getViolationSummary(results);
// Returns: { critical: 0, serious: 2, moderate: 5, minor: 3 }
```

### FocusManagementService
Manages focus for keyboard navigation and accessibility.

**Methods:**
```typescript
// Set focus to an element
focusManagementService.setFocus(element);

// Restore focus to previously focused element
focusManagementService.restoreFocus();

// Set focus to first focusable element in container
focusManagementService.setFocusToFirstElement(container);

// Get all focusable elements
const focusable = focusManagementService.getFocusableElements(container);

// Trap focus within a container (returns cleanup function)
const cleanup = focusManagementService.trapFocus(modalElement);
// Call cleanup() when done to remove event listeners

// Get next/previous focusable element
const next = focusManagementService.getNextFocusable(currentElement);
const prev = focusManagementService.getPreviousFocusable(currentElement);
```

### KeyboardNavigationService
Manages keyboard shortcuts and global keyboard navigation.

**Methods:**
```typescript
// Register a custom shortcut
keyboardNavigationService.registerShortcut('custom-action', {
  key: 'k',
  ctrlKey: true,
  description: 'Custom action',
  action: () => console.log('Custom action triggered')
});

// Unregister a shortcut
keyboardNavigationService.unregisterShortcut('custom-action');

// Get all shortcuts
const shortcuts = keyboardNavigationService.getAllShortcuts();

// Listen to shortcut triggers
keyboardNavigationService.getShortcutTriggered$().subscribe(shortcut => {
  console.log('Shortcut triggered:', shortcut.description);
});
```

## Styles

### Global Accessibility Styles
Imported automatically via `styles/_accessibility.scss`.

**Features:**
- `:focus-visible` support with high-visibility outlines
- Screen reader only content (`.sr-only`, `.visually-hidden`)
- High contrast mode support
- Reduced motion support
- Color contrast improvements
- Touch target size enforcement (44x44px minimum)
- ARIA live region styling
- Print accessibility

**CSS Classes:**
```scss
// Hide content visually but keep for screen readers
.sr-only, .screen-reader-only, .visually-hidden

// Skip links (automatically styled)
.skip-link

// Dark background text
.dark-background

// Error and success messages
.error-message, .success-message

// Required field indicator (automatically added)
.required, [required], [aria-required="true"]
```

## Installation

The module is already installed and configured. To use it:

1. **Import the module** in your app.module.ts:
```typescript
import { AccessibilityModule } from './accessibility/accessibility.module';

@NgModule({
  imports: [
    // ... other imports
    AccessibilityModule
  ]
})
export class AppModule { }
```

2. **Add skip navigation** to your app.component.html:
```html
<app-skip-navigation></app-skip-navigation>
<!-- rest of your app -->
```

3. **Add main content id/role**:
```html
<main id="main-content" role="main" tabindex="-1">
  <!-- your main content -->
</main>
```

4. **Add navigation role**:
```html
<nav role="navigation">
  <!-- your navigation -->
</nav>
```

5. **Add footer role**:
```html
<footer role="contentinfo">
  <!-- your footer -->
</footer>
```

## Testing

### Automated Testing
Run automated accessibility tests in development:
```typescript
import { AccessibilityTestingService } from './accessibility/services/accessibility-testing.service';

constructor(private a11yService: AccessibilityTestingService) {}

async ngOnInit() {
  if (!environment.production) {
    const results = await this.a11yService.runAudit();
    this.a11yService.logResults(results);
    
    const score = this.a11yService.getScore(results);
    console.log(`Accessibility Score: ${score}/100`);
  }
}
```

### Manual Testing Checklist

#### Keyboard Navigation
- [ ] Tab through all interactive elements
- [ ] Shift+Tab navigates backwards
- [ ] Enter/Space activates buttons and links
- [ ] Arrow keys work in menus and dropdowns
- [ ] Escape closes modals and menus
- [ ] Focus is visible at all times
- [ ] Focus order is logical
- [ ] No keyboard traps

#### Screen Readers
- [ ] Test with NVDA (Windows)
- [ ] Test with JAWS (Windows)
- [ ] Test with VoiceOver (macOS)
- [ ] All images have alt text
- [ ] Form labels are properly associated
- [ ] Error messages are announced
- [ ] Dynamic content updates are announced
- [ ] Landmarks are properly defined

#### Visual
- [ ] Text contrast meets 4.5:1 (normal) or 3:1 (large)
- [ ] Focus indicators are clearly visible
- [ ] Text can be resized to 200% without loss of content
- [ ] Content works without color alone
- [ ] UI components have 3:1 contrast ratio

#### Mobile
- [ ] Touch targets are at least 44x44px
- [ ] Works in portrait and landscape
- [ ] Zoom works up to 400%
- [ ] Content reflows properly

## Browser Support

- ✅ Chrome/Edge (latest 2 versions)
- ✅ Firefox (latest 2 versions)
- ✅ Safari (latest 2 versions)
- ✅ iOS Safari (latest 2 versions)
- ✅ Android Chrome (latest 2 versions)

## Screen Reader Support

- ✅ NVDA (latest version)
- ✅ JAWS (latest version)
- ✅ VoiceOver (macOS/iOS latest)
- ✅ TalkBack (Android latest)
- ✅ Narrator (Windows latest)

## Accessibility Standards Compliance

### WCAG 2.1 Level AA
The application conforms to WCAG 2.1 Level AA standards, including:
- **Perceivable**: Content is presentable to users in ways they can perceive
- **Operable**: User interface components and navigation are operable
- **Understandable**: Information and operation of the UI is understandable
- **Robust**: Content can be interpreted by assistive technologies

### Section 508
Compliant with Section 508 of the Rehabilitation Act.

### ADA
Compliant with the Americans with Disabilities Act (ADA).

## Best Practices for Developers

### 1. Use Semantic HTML
```html
<!-- Good -->
<button>Click me</button>
<nav><a href="/home">Home</a></nav>
<main><h1>Page Title</h1></main>

<!-- Bad -->
<div onclick="...">Click me</div>
<div class="nav"><span>Home</span></div>
<div><span class="title">Page Title</span></div>
```

### 2. Provide Alt Text
```html
<!-- Good -->
<img src="photo.jpg" alt="Family gathering at Christmas 2020">

<!-- Bad -->
<img src="photo.jpg">
<img src="photo.jpg" alt="image">
```

### 3. Label Form Fields
```html
<!-- Good -->
<label for="name">Name:</label>
<input id="name" type="text" required>

<!-- Bad -->
<input type="text" placeholder="Name">
```

### 4. Use ARIA When Needed
```html
<!-- Good -->
<button aria-label="Close dialog" aria-controls="dialog-1">
  <mat-icon>close</mat-icon>
</button>

<!-- Bad -->
<button><mat-icon>close</mat-icon></button>
```

### 5. Manage Focus
```typescript
// In modals
ngAfterViewInit() {
  this.focusService.setFocusToFirstElement(this.modalElement);
  this.cleanupFocusTrap = this.focusService.trapFocus(this.modalElement);
}

ngOnDestroy() {
  if (this.cleanupFocusTrap) {
    this.cleanupFocusTrap();
  }
  this.focusService.restoreFocus();
}
```

### 6. Announce Dynamic Changes
```html
<!-- Use aria-live for dynamic content -->
<div aria-live="polite" aria-atomic="true">
  {{ statusMessage }}
</div>

<!-- For urgent alerts -->
<div role="alert" aria-live="assertive">
  {{ errorMessage }}
</div>
```

### 7. Test with Keyboard
Before committing code:
1. Disconnect your mouse
2. Navigate through your feature using only the keyboard
3. Ensure everything is accessible and the focus order makes sense

### 8. Run Automated Tests
```bash
# In development
npm start
# Open browser console
# Check for accessibility violations
```

## Reporting Accessibility Issues

If you discover an accessibility barrier:

1. **Email**: accessibility@rushtonroots.com
2. **Create an issue** in the repository with the "accessibility" label
3. **Provide details**:
   - What you were trying to do
   - What went wrong
   - Browser and assistive technology used
   - Steps to reproduce

## Resources

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Axe Accessibility Testing](https://www.deque.com/axe/)
- [WebAIM Resources](https://webaim.org/)
- [MDN Accessibility](https://developer.mozilla.org/en-US/docs/Web/Accessibility)
- [Material Design Accessibility](https://material.io/design/usability/accessibility.html)

## Changelog

### Version 1.0.0 (December 2025)
- ✅ Initial release
- ✅ WCAG 2.1 AA compliance
- ✅ Skip navigation component
- ✅ Keyboard shortcuts system
- ✅ Accessibility statement page
- ✅ Focus management service
- ✅ Automated testing with Axe
- ✅ Comprehensive accessibility styles
- ✅ Screen reader optimization
- ✅ High contrast and reduced motion support

## License

Copyright (c) 2025 RushtonRoots. All rights reserved.
