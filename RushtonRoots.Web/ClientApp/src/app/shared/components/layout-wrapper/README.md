# LayoutWrapperComponent

**Location**: `RushtonRoots.Web/ClientApp/src/app/shared/components/layout-wrapper/`  
**Created**: December 17, 2025  
**Phase**: 11.1 - Layout Migration

## Overview

The LayoutWrapperComponent is the main layout orchestrator for the RushtonRoots application. It integrates all layout components (header, navigation, breadcrumbs, footer) into a unified, reusable component that can be embedded in `_Layout.cshtml` as an Angular Element.

## Purpose

- **Unify Layout Structure**: Combine all layout components into a single component
- **Simplify _Layout.cshtml**: Reduce complexity in Razor views
- **Support Breadcrumbs**: Enable contextual navigation across the application
- **Content Projection**: Use Angular's `<ng-content>` to embed `@RenderBody()` content
- **Maintain Responsiveness**: Handle mobile, tablet, and desktop layouts

## Component Structure

```
LayoutWrapperComponent
├── SkipNavigationComponent (accessibility)
├── HeaderComponent
│   ├── Logo & Title
│   ├── NavigationComponent (desktop/mobile)
│   ├── Search Bar
│   ├── Notification Bell
│   └── UserMenuComponent
├── BreadcrumbComponent (optional)
├── Main Content (<ng-content>)
└── FooterComponent
```

## Usage

### In _Layout.cshtml

```html
<app-layout-wrapper
    userinfo='@Html.Raw(Json.Serialize(new {
        name = User.Identity?.Name ?? "",
        role = User.IsInRole("Admin") ? "System Admin" : "Family Member",
        isAuthenticated = User.Identity?.IsAuthenticated ?? false,
        isAdmin = User.IsInRole("Admin"),
        isHouseholdAdmin = User.IsInRole("HouseholdAdmin")
    }))'
    breadcrumbitems='@Html.Raw(Json.Serialize(ViewData["Breadcrumbs"] ?? new object[] { }))'
    showsearch="true"
    shownotifications="true"
    showbreadcrumbs="@((ViewData["ShowBreadcrumbs"] as bool?) ?? false)">
    
    @RenderBody()
    
</app-layout-wrapper>
```

### Setting Breadcrumbs in Controllers

```csharp
// In any controller action
ViewData["Breadcrumbs"] = new[]
{
    new { label = "Home", url = "/", icon = "home" },
    new { label = "People", url = "/Person" },
    new { label = "John Doe" }  // Current page (no URL)
};
ViewData["ShowBreadcrumbs"] = true;
```

## Properties

### Inputs

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `userinfo` | `string \| UserInfo` | - | User authentication and role information |
| `breadcrumbitems` | `string \| BreadcrumbItem[]` | `[]` | Breadcrumb navigation items |
| `showsearch` | `boolean` | `true` | Whether to display search bar in header |
| `shownotifications` | `boolean` | `true` | Whether to display notification bell |
| `showbreadcrumbs` | `boolean` | `false` | Whether to display breadcrumb navigation |

### Outputs

| Event | Type | Description |
|-------|------|-------------|
| `searchQuery` | `EventEmitter<string>` | Emitted when user performs a search |
| `logout` | `EventEmitter<void>` | Emitted when user logs out |

## UserInfo Interface

```typescript
interface UserInfo {
  name: string;
  role: string;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isHouseholdAdmin: boolean;
}
```

## BreadcrumbItem Interface

```typescript
interface BreadcrumbItem {
  label: string;
  url?: string;
  icon?: string;
}
```

## Features

### 1. Skip to Main Content
Uses `SkipNavigationComponent` for accessibility, allowing keyboard users to bypass navigation and go directly to content.

### 2. Integrated Header & Navigation
The HeaderComponent includes:
- Site logo and title
- Desktop horizontal navigation
- Mobile hamburger menu toggle
- Global search bar
- Notification bell
- User menu dropdown

### 3. Conditional Breadcrumbs
Breadcrumbs are only displayed when:
- `showbreadcrumbs` is set to `true`
- `breadcrumbitems` array has at least one item

### 4. Content Projection
Uses Angular's `<ng-content>` to embed the main content area, allowing `@RenderBody()` from Razor to render inside the layout.

### 5. Consistent Footer
FooterComponent provides:
- Brand section
- Navigation links (About, Resources, Support)
- Contact information
- Social media links
- Copyright notice

## Responsive Behavior

### Desktop (≥ 960px)
- Horizontal navigation in header
- Full-width content container (max 1400px)
- Search bar visible in header

### Tablet (600px - 959px)
- Adaptive navigation
- Medium-width content
- Reduced header padding

### Mobile (< 600px)
- Hamburger menu toggle
- Vertical mobile navigation menu
- Single-column layout
- Reduced content padding

## Accessibility

- **Keyboard Navigation**: Full keyboard support for all interactive elements
- **ARIA Labels**: Proper labeling for screen readers
- **Skip Links**: Skip to main content link for keyboard users
- **Semantic HTML**: Uses `<header>`, `<nav>`, `<main>`, `<footer>` elements
- **Focus Management**: Proper focus indicators on all interactive elements

## Styling

Styles are defined in `layout-wrapper.component.scss`:
- Flexbox layout for vertical stacking
- Min-height 100vh to ensure footer stays at bottom
- Responsive padding and margins
- Consistent max-width for content area

## Dependencies

### Angular Modules
- `CommonModule`
- `SharedModule` (includes Material modules)
- `AccessibilityModule`

### Sub-Components
- `SkipNavigationComponent`
- `HeaderComponent`
- `NavigationComponent` (used by HeaderComponent)
- `UserMenuComponent` (used by HeaderComponent)
- `BreadcrumbComponent`
- `FooterComponent`

## Module Registrations

**SharedModule**:
```typescript
declarations: [ LayoutWrapperComponent ],
exports: [ LayoutWrapperComponent ]
```

**AppModule**:
```typescript
import { LayoutWrapperComponent } from './shared/components/layout-wrapper/layout-wrapper.component';
safeDefine('app-layout-wrapper', LayoutWrapperComponent);
```

## Events

### Logout Event
When the user clicks logout:
1. `onLogout()` method is called
2. Finds the hidden logout form by ID: `logoutForm`
3. Submits the form for server-side logout
4. Emits `logout` event (currently unused but available for future use)

### Search Event
When the user performs a search:
1. Header emits `searchQuery` event
2. LayoutWrapper re-emits the event
3. Currently unused, but available for global search implementation

## Testing

### Manual Testing Checklist
- [ ] Layout renders on all pages
- [ ] Header displays user info correctly
- [ ] Navigation links work (desktop & mobile)
- [ ] Mobile menu toggles properly
- [ ] Breadcrumbs appear when enabled
- [ ] Breadcrumb navigation works
- [ ] Footer links work
- [ ] Logout functionality works
- [ ] Search bar appears for authenticated users
- [ ] Notification bell appears for authenticated users

### Unit Tests (Pending)
- [ ] User info parsing from JSON string
- [ ] Breadcrumb items parsing from JSON string
- [ ] Conditional breadcrumb display
- [ ] Logout event handling
- [ ] Search event propagation

## Future Enhancements

1. **Global Search**: Implement actual search functionality
2. **Notifications**: Connect notification bell to real notification system
3. **Theming**: Support light/dark mode toggle
4. **Customization**: Allow per-page layout customization (hide search, etc.)
5. **Loading States**: Add layout-level loading indicators

## Related Components

- [HeaderComponent](../header/README.md)
- [NavigationComponent](../navigation/README.md)
- [BreadcrumbComponent](../breadcrumb/README.md)
- [FooterComponent](../footer/README.md)
- [SkipNavigationComponent](../../../accessibility/components/skip-navigation/README.md)

## Changelog

- **2025-12-17**: Initial creation for Phase 11.1 Layout Migration
