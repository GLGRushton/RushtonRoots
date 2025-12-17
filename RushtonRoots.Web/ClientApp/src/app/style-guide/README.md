# RushtonRoots Style Guide Component

## Overview

The StyleGuideComponent is a comprehensive design system documentation tool that serves as the single source of truth for all UI components, patterns, and design standards in the RushtonRoots application.

## Features

### 🎨 Design Foundation
- **Color Palette**: Primary, neutral, and semantic colors with SCSS variables
- **Typography**: Hierarchical heading system with size and weight specifications
- **Spacing System**: 8px grid system with visual examples
- **Material Icons**: Showcase of commonly used icons from Material Design

### 📦 Component Documentation
Documents **70+ components** across **10 completed phases**:
- **Phase 1-2**: Foundation & Layout (13 components)
- **Phase 3**: Person Management (11 components)
- **Phase 4**: Household Management (7 components)
- **Phase 5**: Relationship Management (11 components)
- **Phase 6**: Authentication (8 components)
- **Phase 7**: Content Pages (8 components)
- **Phase 8**: Advanced Features (13 components)
- **Phase 9**: Mobile & PWA (8 components)
- **Phase 10**: Accessibility (3 components)

### 🛠️ Developer Resources
- **Angular Material Theme Guide**: Custom theme configuration examples
- **Code Examples**: Angular Elements integration, Reactive Forms patterns
- **Best Practices**: Accessibility guidelines (WCAG 2.1 AA)
- **Component Patterns**: Standard patterns for forms, dialogs, tables, etc.

## Navigation

The style guide features a **sticky sidebar navigation** with 12 main sections:

1. **Foundation** - Design tokens and visual elements
2. **Core Components** - Reusable UI components (Phase 1-2)
3. **Person Management** - Person-related components
4. **Household Management** - Household components
5. **Relationships** - Partnership and ParentChild components
6. **Authentication** - Login, profile, and auth components
7. **Content Pages** - Wiki, Recipe, Story, Tradition components
8. **Advanced Features** - Media, Calendar, Messaging components
9. **Mobile & PWA** - Mobile-optimized and PWA components
10. **Accessibility** - Accessibility helper components
11. **Theme Customization** - Angular Material theming guide
12. **Code Examples** - Usage patterns and best practices

## Usage

### Accessing the Style Guide

The style guide is available at `/Home/StyleGuide` and is embedded using Angular Elements:

```html
<!-- In StyleGuide.cshtml -->
<app-style-guide></app-style-guide>
```

### Registering in app.module.ts

The component is registered as an Angular Element:

```typescript
import { StyleGuideComponent } from './style-guide/style-guide.component';

// In declarations array
StyleGuideComponent

// In ngDoBootstrap
safeDefine('app-style-guide', StyleGuideComponent);
```

## File Structure

```
/ClientApp/src/app/style-guide/
├── style-guide.component.ts        # TypeScript component logic
├── style-guide.component.html      # Template with all sections
├── style-guide.component.scss      # Component styling
└── README.md                       # This file
```

## Component Structure

### TypeScript (`style-guide.component.ts`)

**Key Properties**:
- `activeSection: string` - Tracks current active section for navigation highlighting
- `sections[]` - Navigation sections with IDs, labels, and icons
- `sampleBreadcrumbs[]` - Example breadcrumb data
- `samplePeople[]` - Example person data for component demonstrations
- `colors{}` - Color palette with SCSS variable names
- `spacingSizes[]` - Spacing scale with variable names
- `typographyExamples[]` - Typography scale with metadata
- `iconExamples[]` - Common Material Icons
- `phases[]` - Component organization by implementation phase

**Key Methods**:
- `scrollToSection(sectionId: string)` - Smooth scroll to section and update active state
- `onSearchChanged(searchTerm: string)` - Handle search component events
- `onEmptyStateAction()` - Handle empty state action clicks
- `showConfirmDialog()` - Demonstrate confirm dialog
- `showDeleteDialog()` - Demonstrate delete dialog

### Template (`style-guide.component.html`)

**Layout Structure**:
```
.style-guide-wrapper (flex container)
├── .style-guide-nav (sticky sidebar, 280px)
│   ├── .nav-header
│   └── .nav-sections (navigation buttons)
└── .style-guide-content (main content, flex:1)
    ├── header (title, subtitle, version badges)
    ├── section#foundation
    ├── section#core-components
    ├── section#person
    ├── section#household
    ├── section#relationship
    ├── section#auth
    ├── section#content
    ├── section#advanced
    ├── section#mobile
    ├── section#accessibility
    ├── section#theme
    ├── section#code-examples
    └── footer (version info, back-to-top)
```

### Styling (`style-guide.component.scss`)

**Key Classes**:
- `.style-guide-wrapper` - Flex container for sidebar + content
- `.style-guide-nav` - Sticky sidebar with navigation
- `.style-guide-content` - Main scrollable content area
- `.section-title` - Section headings with icons
- `.color-grid` - Responsive color swatch grid
- `.icon-grid` - Responsive icon showcase grid
- `.code-block` - Syntax-highlighted code examples
- `.phase-summary` - Phase overview cards

**Responsive Breakpoints**:
- **1280px**: Smaller sidebar (240px)
- **960px**: Horizontal mobile navigation, single-column grids
- **600px**: Compact mobile layout, smaller typography

## Adding New Components

When adding new components to the application, update the style guide:

### 1. Update TypeScript Data

Add the component to the appropriate phase in the `phases` array:

```typescript
{
  name: 'Phase X: Feature Name',
  status: 'Complete',
  components: [
    // ... existing components
    'NewComponent',
  ]
}
```

### 2. Add Section (if needed)

For major new features, add a new section:

```typescript
// In sections array
{ id: 'new-section', label: 'New Section', icon: 'icon_name' }
```

### 3. Add HTML Section

Add the component demonstration in the HTML template:

```html
<section id="new-section" class="mb-xxl">
  <h2 class="section-title mb-lg">
    <mat-icon>icon_name</mat-icon>
    Section Title
  </h2>
  <p class="section-description mb-lg">
    Section description
  </p>

  <mat-card>
    <mat-card-header>
      <mat-card-title>Component Name</mat-card-title>
      <mat-card-subtitle>Component description</mat-card-subtitle>
    </mat-card-header>
    <mat-card-content>
      <!-- Component demonstration -->
      <app-new-component [input]="value"></app-new-component>
    </mat-card-content>
  </mat-card>
</section>
```

### 4. Update Navigation

The navigation automatically updates based on the `sections` array.

## Color Palette

### Primary Colors
- **Primary Dark**: `#1b5e20` (`$primary-dark`)
- **Primary**: `#2e7d32` (`$primary`)
- **Primary Light**: `#4caf50` (`$primary-light`)
- **Accent**: `#66bb6a` (`$accent`)

### Neutral Colors
- **Text Primary**: `#212121` (`$text-primary`)
- **Text Secondary**: `#757575` (`$text-secondary`)
- **Background**: `#f5f5f5` (`$background`)
- **Surface**: `#ffffff` (`$surface`)

### Semantic Colors
- **Success**: `#4caf50` (`$success`)
- **Warning**: `#ff9800` (`$warning`)
- **Error**: `#d32f2f` (`$error`)
- **Info**: `#2196f3` (`$info`)

## Typography Scale

- **H1**: 32px / 700 - Page Title
- **H2**: 24px / 600 - Section Title
- **H3**: 20px / 600 - Subsection Title
- **H4**: 18px / 500 - Card Title
- **H5**: 16px / 500 - Small Heading
- **H6**: 14px / 500 - Label Heading
- **P**: 16px / 400 - Body Text

## Spacing System (8px Grid)

- **XS**: 4px (`$spacing-xs`)
- **SM**: 8px (`$spacing-sm`)
- **MD**: 16px (`$spacing-md`)
- **LG**: 24px (`$spacing-lg`)
- **XL**: 32px (`$spacing-xl`)
- **XXL**: 48px (`$spacing-xxl`)

## Maintenance

### Regular Updates

- **After Each Phase**: Update the `phases` array with new components
- **New Patterns**: Add code examples for new patterns
- **Theme Changes**: Update theme documentation if Material theme is modified
- **Design Tokens**: Update color/spacing/typography if design tokens change

### Testing

- **Manual Testing**: Verify all component examples work correctly
- **Responsive Testing**: Test on mobile, tablet, and desktop sizes
- **Accessibility**: Ensure all content is accessible (ARIA labels, keyboard navigation)
- **Cross-Browser**: Test in Chrome, Firefox, Safari, Edge

## Future Enhancements

Planned improvements (not yet implemented):

1. **Main Menu Link**: Add navigation link from main menu (admin/developer only)
2. **Interactive Playgrounds**: Add live code editors for components
3. **Usage Statistics**: Track which components are most viewed
4. **Downloadable Assets**: Provide design asset downloads (icons, colors, etc.)
5. **Search Functionality**: Add search across all components and sections
6. **Version History**: Track changes to design system over time
7. **Component Metadata**: Add version numbers, dependencies, browser support info

## Browser Support

The style guide supports:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Accessibility

The style guide is WCAG 2.1 AA compliant:
- ✅ Keyboard navigation (Tab, Enter, Arrow keys)
- ✅ Screen reader support (ARIA labels, semantic HTML)
- ✅ Color contrast meets 4.5:1 minimum
- ✅ Focus indicators visible
- ✅ Skip navigation links
- ✅ Responsive text sizing

## Contributing

To contribute to the style guide:

1. Review the component structure
2. Add your component to the appropriate phase
3. Create a demonstration in the HTML template
4. Test thoroughly on all device sizes
5. Update this README if needed
6. Submit a PR with clear description

## Questions or Issues?

For questions about the style guide or to report issues, contact the development team or create a GitHub issue.

---

**Version**: 1.0  
**Last Updated**: December 17, 2025  
**Maintained By**: RushtonRoots Development Team
