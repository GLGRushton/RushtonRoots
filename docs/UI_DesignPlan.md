# RushtonRoots - UI Design & Enhancement Plan

## Document Overview

**Purpose**: This document provides a comprehensive phased plan to improve the UI elements of the RushtonRoots application. It focuses on migrating inline components from .cshtml views to reusable Angular components and revamping the design and CSS for a modern, appealing user experience.

**Last Updated**: December 2025  
**Document Owner**: Development Team  
**Status**: Planning Phase

---

## Table of Contents

1. [Current State Analysis](#current-state-analysis)
2. [Design Principles & Goals](#design-principles--goals)
3. [Design System & Component Library](#design-system--component-library)
4. [Implementation Phases](#implementation-phases)
5. [Component Migration Strategy](#component-migration-strategy)
6. [CSS/Styling Strategy](#cssstyling-strategy)
7. [Accessibility & Responsiveness](#accessibility--responsiveness)
8. [Testing Strategy](#testing-strategy)
9. [Success Metrics](#success-metrics)

---

## Current State Analysis

### Existing Structure

#### .cshtml Views (39 total)
- **Account** (9 views): Login, Register, Profile, CreateUser, ForgotPassword, ResetPassword, etc.
- **Person** (5 views): Index, Create, Edit, Details, Delete
- **Household** (6 views): Index, Create, Edit, Details, Delete, Members
- **Partnership** (5 views): Index, Create, Edit, Details, Delete
- **ParentChild** (5 views): Index, Create, Edit, Details, Delete
- **Home** (1 view): Index
- **Recipe, StoryView, Tradition, Wiki** (4 views): Index pages
- **Shared** (2 views): _Layout, _ValidationScriptsPartial

#### Angular Components (3 total)
- **app-root**: Main application wrapper
- **app-welcome**: Welcome message component (used on Home page)
- **app-family-tree**: Family tree visualization component (used on Home page)

### Current UI Issues

1. **Inline Styles**: Most views have `<style>` blocks directly in .cshtml files
2. **Inconsistent Design**: Different pages use different color schemes and layouts
3. **Basic Bootstrap**: Limited use of Bootstrap, inconsistent implementation
4. **Poor Component Reusability**: Forms, tables, and UI patterns are duplicated across views
5. **Accessibility Gaps**: Missing ARIA labels, keyboard navigation, screen reader support
6. **Mobile Experience**: Minimal responsive design, some pages not mobile-friendly
7. **Visual Appeal**: Basic, outdated design aesthetic
8. **No Design System**: No unified color palette, typography, or spacing system

### What Works Well

1. **Layout Structure**: _Layout.cshtml provides good header/footer foundation
2. **Green Theme**: Consistent green color palette (#2e7d32, #4caf50, etc.)
3. **Angular Elements**: Successfully integrated with Razor views
4. **Family Tree Component**: Well-implemented interactive visualization
5. **Responsive Header**: Header adapts well to mobile devices

---

## Design Principles & Goals

### Core Design Principles

1. **Consistency**: Unified design language across all pages and components
2. **Clarity**: Clear visual hierarchy and intuitive navigation
3. **Accessibility**: WCAG 2.1 AA compliant, keyboard navigable, screen reader friendly
4. **Performance**: Fast load times, optimized assets, lazy loading
5. **Responsiveness**: Mobile-first design, works on all screen sizes
6. **Delight**: Smooth animations, thoughtful interactions, pleasant aesthetics

### Design Goals

1. **Modern Aesthetic**: Contemporary design that feels fresh and professional
2. **Family-Friendly**: Warm, welcoming, and approachable visual style
3. **Heritage Focus**: Design elements that evoke family, tradition, and connection
4. **Professional Quality**: Polished, high-quality UI that users trust
5. **Scalability**: Design system that supports future features

### Visual Direction

- **Color Palette**: Refined green theme with complementary colors
- **Typography**: Clear, readable fonts with proper hierarchy
- **Spacing**: Consistent padding and margins using 8px grid system
- **Shadows**: Subtle elevation for depth and focus
- **Borders**: Minimal borders, prefer shadows and spacing
- **Icons**: Consistent icon set (Material Icons or Font Awesome)
- **Imagery**: High-quality photos, thoughtful placeholders

---

## Design System & Component Library

### Angular Material Integration

**Recommendation**: Adopt Angular Material as the primary UI component library

**Benefits**:
- Battle-tested components with built-in accessibility
- Consistent theming system
- Responsive and mobile-friendly
- Excellent documentation
- Active community and maintenance
- Seamless Angular integration

**Core Components to Use**:
- MatButton, MatIconButton
- MatFormField, MatInput, MatSelect
- MatCard
- MatTable, MatPaginator, MatSort
- MatDialog, MatBottomSheet
- MatSnackBar for notifications
- MatToolbar, MatSidenav
- MatTabs, MatExpansionPanel
- MatDatepicker
- MatChipList, MatBadge
- MatProgressSpinner, MatProgressBar

### Custom Design Tokens

```scss
// Color Palette
$primary: #2e7d32;        // Forest Green
$primary-light: #4caf50;  // Light Green
$primary-dark: #1b5e20;   // Dark Green
$accent: #66bb6a;         // Accent Green
$warn: #d32f2f;           // Red for warnings
$background: #f5f5f5;     // Light gray background
$surface: #ffffff;        // White surface
$text-primary: #212121;   // Dark text
$text-secondary: #757575; // Gray text

// Typography
$font-family: 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
$font-size-base: 16px;
$font-weight-normal: 400;
$font-weight-medium: 500;
$font-weight-bold: 700;

// Spacing (8px grid)
$spacing-xs: 4px;
$spacing-sm: 8px;
$spacing-md: 16px;
$spacing-lg: 24px;
$spacing-xl: 32px;
$spacing-xxl: 48px;

// Border Radius
$border-radius-sm: 4px;
$border-radius-md: 8px;
$border-radius-lg: 12px;
$border-radius-xl: 16px;

// Elevation (shadows)
$shadow-sm: 0 2px 4px rgba(0,0,0,0.1);
$shadow-md: 0 4px 8px rgba(0,0,0,0.12);
$shadow-lg: 0 8px 16px rgba(0,0,0,0.15);
```

---

## Implementation Phases

### Phase 1: Foundation & Design System (Months 1-2)

**Goal**: Establish design system, install Angular Material, create core reusable components

#### Phase 1.1: Setup & Infrastructure (Weeks 1-2) ✅ COMPLETE

**Tasks**:
- [x] Install Angular Material and CDK
- [x] Configure Angular Material theming with RushtonRoots colors
- [x] Set up global styles.scss with design tokens
- [x] Create shared Angular module for common components
- [x] Install Material Icons or Font Awesome
- [x] Set up CSS architecture (SCSS structure)
- [x] Create style guide documentation page

**Deliverables**:
- Angular Material integrated and themed ✅
- Global SCSS with design tokens ✅
- SharedModule with common imports ✅
- Style guide component for reference ✅

**Success Criteria**: Design system is in place and can be used for new components ✅

**Completed**: December 2025

#### Phase 1.2: Core Reusable Components (Weeks 3-4) ✅ COMPLETE

**Tasks**:
- [x] Create PersonCardComponent (for displaying person summary)
- [x] Create PersonListComponent (for person tables/lists)
- [x] Create SearchBarComponent (reusable search interface)
- [x] Create PageHeaderComponent (consistent page headers)
- [x] Create EmptyStateComponent (for "no data" states)
- [x] Create ConfirmDialogComponent (for delete confirmations)
- [x] Create LoadingSpinnerComponent (consistent loading states)
- [x] Create BreadcrumbComponent (navigation breadcrumbs)

**Deliverables**:
- 8 reusable Angular components ✅
- Component documentation and examples ✅
- Storybook or style guide integration (components registered as Angular Elements) ✅

**Success Criteria**: Core components are built and can be reused across features ✅

**Completed**: December 2025

---

### Phase 2: Layout & Navigation Enhancement (Month 3)

**Goal**: Modernize layout, header, footer, and navigation system

#### Phase 2.1: Header & Navigation Redesign (Weeks 5-6) ✅ COMPLETE

**Tasks**:
- [x] Migrate _Layout.cshtml header to Angular HeaderComponent
- [x] Implement responsive navigation menu (mobile hamburger menu)
- [x] Add navigation sidebar option for authenticated users
- [x] Improve user profile dropdown
- [x] Add breadcrumb navigation (component already available from Phase 1.2)
- [x] Implement search in header (global search)
- [x] Add notification bell icon (for future notifications)
- [x] Enhance mobile header experience

**Deliverables**:
- HeaderComponent (Angular) ✅
- NavigationComponent with responsive menu ✅
- UserMenuComponent ✅
- BreadcrumbComponent ✅ (from Phase 1.2)
- Global search component ✅ (integrated in HeaderComponent)

**Success Criteria**: Navigation is intuitive, responsive, and visually appealing ✅

**Completed**: December 2025

#### Phase 2.2: Footer & Page Layout (Week 7) ✅ COMPLETE

**Tasks**:
- [x] Migrate footer to Angular FooterComponent
- [x] Improve footer design and content organization
- [x] Add social media links and contact info
- [x] Create PageLayoutComponent wrapper
- [x] Implement consistent page container widths
- [x] Add page transition animations
- [x] Improve overall page spacing and rhythm

**Deliverables**:
- FooterComponent (Angular) ✅
- PageLayoutComponent ✅
- Consistent layout system ✅

**Success Criteria**: All pages have consistent, professional layout ✅

**Completed**: December 2025

---

### Phase 3: Person Management UI (Months 4-5)

**Goal**: Transform Person views into modern Angular-based interface

#### Phase 3.1: Person Index & Search (Weeks 8-9)

**Tasks**:
- [ ] Create PersonIndexComponent (Angular)
- [ ] Implement advanced search with filters (MatFormField)
- [ ] Build PersonTableComponent with MatTable
- [ ] Add sorting and pagination (MatSort, MatPaginator)
- [ ] Create filter chips for active filters (MatChip)
- [ ] Add quick actions menu (edit, delete, view)
- [ ] Implement responsive card view for mobile
- [ ] Add export functionality (CSV, PDF)

**Deliverables**:
- PersonIndexComponent
- PersonTableComponent
- PersonSearchComponent
- Responsive mobile view

**Success Criteria**: Person index page is fast, searchable, and easy to use

#### Phase 3.2: Person Details & Timeline (Week 10)

**Tasks**:
- [ ] Create PersonDetailsComponent (Angular)
- [ ] Build PersonTimelineComponent (life events)
- [ ] Create RelationshipVisualizerComponent
- [ ] Add PhotoGalleryComponent
- [ ] Implement tabbed interface (MatTabs)
- [ ] Add edit-in-place functionality
- [ ] Create person action buttons (edit, delete, share)
- [ ] Improve photo upload experience

**Deliverables**:
- PersonDetailsComponent
- PersonTimelineComponent
- RelationshipVisualizerComponent
- PhotoGalleryComponent

**Success Criteria**: Person details page is informative and easy to navigate

#### Phase 3.3: Person Create & Edit Forms (Weeks 11-12)

**Tasks**:
- [ ] Create PersonFormComponent (Angular reactive forms)
- [ ] Implement step-by-step wizard for person creation (MatStepper)
- [ ] Add form validation with clear error messages
- [ ] Create DatePickerComponent with MatDatepicker
- [ ] Build LocationAutocompleteComponent
- [ ] Add photo upload with preview
- [ ] Implement autosave draft functionality
- [ ] Create form success/error notifications (MatSnackBar)

**Deliverables**:
- PersonFormComponent
- Wizard-based create flow
- Enhanced form validation
- Autosave functionality

**Success Criteria**: Creating and editing people is intuitive and error-free

---

### Phase 4: Household Management UI (Month 6)

**Goal**: Modernize Household views with Angular components

#### Phase 4.1: Household Index & Cards (Weeks 13-14)

**Tasks**:
- [ ] Create HouseholdIndexComponent (Angular)
- [ ] Build HouseholdCardComponent (MatCard)
- [ ] Implement grid layout for household cards
- [ ] Add household search and filters
- [ ] Create household quick actions
- [ ] Add member count badges (MatBadge)
- [ ] Implement household sorting options

**Deliverables**:
- HouseholdIndexComponent
- HouseholdCardComponent
- Card grid layout

**Success Criteria**: Households are visually displayed in card format with clear actions

#### Phase 4.2: Household Details & Members (Weeks 15-16)

**Tasks**:
- [ ] Create HouseholdDetailsComponent
- [ ] Build HouseholdMembersComponent with member management
- [ ] Add member invitation flow (dialog)
- [ ] Create permission management UI
- [ ] Implement member role badges
- [ ] Add household settings panel
- [ ] Create household activity timeline

**Deliverables**:
- HouseholdDetailsComponent
- HouseholdMembersComponent
- Member management interface

**Success Criteria**: Household management is clear and easy to use

---

### Phase 5: Relationship Management UI (Month 7)

**Goal**: Improve Partnership and ParentChild relationship interfaces

#### Phase 5.1: Partnership Management (Weeks 17-18)

**Tasks**:
- [ ] Create PartnershipIndexComponent
- [ ] Build PartnershipCardComponent
- [ ] Implement relationship timeline visualization
- [ ] Add partnership search and filters
- [ ] Create partnership form with partner selection
- [ ] Build partnership type selector
- [ ] Add partnership status indicators

**Deliverables**:
- PartnershipIndexComponent
- PartnershipCardComponent
- Enhanced partnership forms

**Success Criteria**: Partnerships are easy to create and visualize

#### Phase 5.2: Parent-Child Relationships (Weeks 19-20)

**Tasks**:
- [ ] Create ParentChildIndexComponent
- [ ] Build FamilyTreeMiniComponent (compact tree view)
- [ ] Implement parent/child selection with autocomplete
- [ ] Add relationship type selector (biological, adopted, etc.)
- [ ] Create relationship validation UI
- [ ] Build relationship suggestions (AI-powered)
- [ ] Add bulk relationship import

**Deliverables**:
- ParentChildIndexComponent
- FamilyTreeMiniComponent
- Relationship validation

**Success Criteria**: Parent-child relationships are intuitive to manage

---

### Phase 6: Account & Authentication UI (Month 8)

**Goal**: Modernize all authentication and account management pages

#### Phase 6.1: Login & Registration (Weeks 21-22)

**Tasks**:
- [ ] Create LoginComponent (Angular)
- [ ] Build modern login form design
- [ ] Add "Remember Me" toggle
- [ ] Create password visibility toggle
- [ ] Build ForgotPasswordComponent
- [ ] Improve reset password flow
- [ ] Add social login buttons (for future use)
- [ ] Create loading states for auth actions

**Deliverables**:
- LoginComponent
- ForgotPasswordComponent
- ResetPasswordComponent
- Modern auth UI

**Success Criteria**: Login experience is smooth and professional

#### Phase 6.2: User Profile & Settings (Weeks 23-24)

**Tasks**:
- [ ] Create UserProfileComponent
- [ ] Build profile edit form
- [ ] Add avatar upload with crop
- [ ] Create notification preferences UI
- [ ] Build privacy settings panel
- [ ] Add connected accounts section
- [ ] Create account deletion flow
- [ ] Implement tabbed settings interface

**Deliverables**:
- UserProfileComponent
- Settings panels
- Avatar upload

**Success Criteria**: Profile management is comprehensive and user-friendly

---

### Phase 7: Content Pages UI (Month 9)

**Goal**: Enhance Wiki, Recipe, Story, and Tradition pages

#### Phase 7.1: Wiki & Knowledge Base (Weeks 25-26)

**Tasks**:
- [ ] Create WikiIndexComponent
- [ ] Build WikiArticleComponent
- [ ] Implement Markdown editor (ngx-markdown-editor)
- [ ] Add wiki search with highlighting
- [ ] Create wiki category navigation
- [ ] Build table of contents component
- [ ] Add version history UI
- [ ] Implement collaborative editing indicators

**Deliverables**:
- WikiIndexComponent
- WikiArticleComponent
- Markdown editor integration

**Success Criteria**: Wiki is easy to navigate and edit

#### Phase 7.2: Recipes, Stories, & Traditions (Week 27)

**Tasks**:
- [ ] Create RecipeCardComponent
- [ ] Build RecipeDetailsComponent
- [ ] Create StoryCardComponent
- [ ] Build TraditionCardComponent
- [ ] Implement masonry grid layout
- [ ] Add category filters and tags
- [ ] Create print-friendly recipe view
- [ ] Add recipe rating and comments

**Deliverables**:
- Recipe, Story, and Tradition components
- Masonry grid layouts
- Enhanced detail views

**Success Criteria**: Content is beautifully presented and easy to browse

---

### Phase 8: Advanced Components (Months 10-11)

**Goal**: Create advanced UI components for complex features

#### Phase 8.1: Media Gallery Enhancements (Weeks 28-30)

**Tasks**:
- [ ] Create MediaGalleryComponent
- [ ] Build photo lightbox with swipe gestures
- [ ] Implement photo tagging interface
- [ ] Add album creation and management
- [ ] Create photo upload drag-and-drop
- [ ] Build photo editing tools (crop, rotate, filters)
- [ ] Add video player component
- [ ] Implement infinite scroll for photos

**Deliverables**:
- MediaGalleryComponent
- Photo lightbox
- Photo editing interface

**Success Criteria**: Media gallery is feature-rich and performant

#### Phase 8.2: Calendar & Events (Weeks 31-32)

**Tasks**:
- [ ] Create CalendarComponent (FullCalendar integration)
- [ ] Build EventCardComponent
- [ ] Implement event creation dialog
- [ ] Add RSVP interface
- [ ] Create event reminder settings
- [ ] Build recurring event UI
- [ ] Add event export (iCal)

**Deliverables**:
- CalendarComponent
- EventCardComponent
- RSVP interface

**Success Criteria**: Calendar is interactive and easy to use

#### Phase 8.3: Messaging & Notifications (Weeks 33-34)

**tasks**:
- [ ] Create MessageThreadComponent
- [ ] Build ChatInterfaceComponent
- [ ] Implement notification panel
- [ ] Add real-time message indicators
- [ ] Create message composition dialog
- [ ] Build notification preferences
- [ ] Add notification grouping

**Deliverables**:
- MessageThreadComponent
- ChatInterfaceComponent
- Notification panel

**Success Criteria**: Messaging is intuitive and responsive

---

### Phase 9: Mobile Optimization (Month 12)

**Goal**: Ensure excellent mobile experience across all features

#### Phase 9.1: Mobile-First Components (Weeks 35-37)

**Tasks**:
- [ ] Review all components for mobile usability
- [ ] Implement mobile-specific navigation patterns
- [ ] Create bottom sheet components (MatBottomSheet)
- [ ] Add touch-friendly button sizes
- [ ] Implement swipe gestures where appropriate
- [ ] Create mobile-optimized forms
- [ ] Add pull-to-refresh functionality
- [ ] Optimize performance for mobile devices

**Deliverables**:
- Mobile-optimized components
- Touch-friendly interfaces
- Performance improvements

**Success Criteria**: All features work excellently on mobile devices

#### Phase 9.2: Progressive Web App Features (Week 38)

**Tasks**:
- [ ] Implement service worker for offline support
- [ ] Add "Add to Home Screen" prompt
- [ ] Create app shell architecture
- [ ] Implement offline indicators
- [ ] Add background sync for forms
- [ ] Create push notification support
- [ ] Optimize for app-like experience

**Deliverables**:
- PWA functionality
- Offline support
- Push notifications

**Success Criteria**: App works offline and feels native on mobile

---

### Phase 10: Accessibility & Polish (Months 13-14)

**Goal**: Ensure WCAG 2.1 AA compliance and polish all UI elements

#### Phase 10.1: Accessibility Audit & Fixes (Weeks 39-42)

**Tasks**:
- [ ] Run automated accessibility testing (Axe, Lighthouse)
- [ ] Add ARIA labels to all interactive elements
- [ ] Ensure keyboard navigation throughout
- [ ] Add skip navigation links
- [ ] Improve focus indicators
- [ ] Test with screen readers (NVDA, JAWS)
- [ ] Add alt text to all images
- [ ] Ensure color contrast meets WCAG standards
- [ ] Create accessibility statement page

**Deliverables**:
- WCAG 2.1 AA compliance
- Accessibility documentation
- Screen reader optimization

**Success Criteria**: All pages pass automated accessibility tests

#### Phase 10.2: Animations & Micro-interactions (Weeks 43-44)

**Tasks**:
- [ ] Add page transition animations
- [ ] Create loading state animations
- [ ] Implement hover effects
- [ ] Add success/error animations
- [ ] Create skeleton screens for loading states
- [ ] Implement smooth scroll behavior
- [ ] Add component enter/exit animations
- [ ] Create progress indicators for multi-step processes

**Deliverables**:
- Consistent animations
- Loading states
- Smooth transitions

**Success Criteria**: UI feels polished and responsive

---

### Phase 11: Performance Optimization (Month 15)

**Goal**: Optimize load times and runtime performance

#### Phase 11.1: Bundle Optimization (Weeks 45-46)

**Tasks**:
- [ ] Analyze bundle sizes with webpack-bundle-analyzer
- [ ] Implement lazy loading for all routes
- [ ] Split vendor bundles
- [ ] Enable Angular Ivy compilation optimizations
- [ ] Minimize and compress assets
- [ ] Implement tree shaking
- [ ] Optimize images (WebP format, lazy loading)
- [ ] Add CDN for static assets

**Deliverables**:
- Smaller bundle sizes
- Faster load times
- Optimized assets

**Success Criteria**: Initial page load under 2 seconds

#### Phase 11.2: Runtime Performance (Week 47)

**Tasks**:
- [ ] Implement OnPush change detection strategy
- [ ] Add virtual scrolling for large lists
- [ ] Optimize API calls (caching, debouncing)
- [ ] Implement pagination for all large datasets
- [ ] Profile and optimize slow components
- [ ] Add performance monitoring
- [ ] Optimize rendering with trackBy
- [ ] Minimize re-renders

**Deliverables**:
- Improved runtime performance
- Smooth scrolling and interactions
- Performance monitoring

**Success Criteria**: 60 FPS interactions, fast component rendering

---

### Phase 12: Testing & Documentation (Month 16)

**Goal**: Ensure quality through testing and comprehensive documentation

#### Phase 12.1: Component Testing (Weeks 48-50)

**Tasks**:
- [ ] Write unit tests for all components
- [ ] Create integration tests for key workflows
- [ ] Add visual regression tests (Percy or similar)
- [ ] Implement E2E tests with Playwright
- [ ] Test responsive behavior
- [ ] Test keyboard navigation
- [ ] Test screen reader compatibility
- [ ] Achieve 80%+ code coverage

**Deliverables**:
- Comprehensive test suite
- Visual regression tests
- E2E tests

**Success Criteria**: All components have tests, no regressions

#### Phase 12.2: Documentation & Training (Weeks 51-52)

**Tasks**:
- [ ] Create component documentation (Storybook)
- [ ] Write design system documentation
- [ ] Create user guides for new UI
- [ ] Record video tutorials
- [ ] Create migration guide from old UI
- [ ] Document accessibility features
- [ ] Create developer onboarding guide
- [ ] Publish changelog and release notes

**Deliverables**:
- Storybook component library
- User documentation
- Developer guides

**Success Criteria**: All components are documented and easy to use

---

## Component Migration Strategy

### Migration Principles

1. **Incremental Migration**: Migrate one feature area at a time, not all at once
2. **Backward Compatibility**: Old and new UI can coexist during migration
3. **Feature Parity**: New components must match or exceed functionality of old views
4. **User Testing**: Test each migration with users before full rollout
5. **Rollback Plan**: Ability to revert to old UI if issues arise

### Migration Process

For each .cshtml view to migrate:

1. **Analyze**: Document current functionality and UI elements
2. **Design**: Create mockups for new Angular component
3. **Build**: Implement Angular component with Material Design
4. **Test**: Unit tests, integration tests, visual tests
5. **Review**: Code review and accessibility review
6. **Deploy**: Deploy behind feature flag
7. **Monitor**: Track usage and errors
8. **Deprecate**: Remove old .cshtml view

### Component Mapping

| .cshtml View | Angular Component | Priority | Estimated Effort |
|--------------|-------------------|----------|------------------|
| Home/Index.cshtml | HomePageComponent | High | Medium |
| Person/Index.cshtml | PersonIndexComponent | High | High |
| Person/Details.cshtml | PersonDetailsComponent | High | High |
| Person/Create.cshtml | PersonFormComponent | High | Medium |
| Person/Edit.cshtml | PersonFormComponent | High | Medium |
| Household/Index.cshtml | HouseholdIndexComponent | Medium | Medium |
| Household/Details.cshtml | HouseholdDetailsComponent | Medium | Medium |
| Partnership/Index.cshtml | PartnershipIndexComponent | Medium | Medium |
| ParentChild/Index.cshtml | ParentChildIndexComponent | Medium | Medium |
| Account/Login.cshtml | LoginComponent | High | Low |
| Account/Profile.cshtml | UserProfileComponent | High | Medium |
| Wiki/Index.cshtml | WikiIndexComponent | Low | High |
| Recipe/Index.cshtml | RecipeIndexComponent | Low | Medium |

### Static vs. Dynamic Components

**Keep in .cshtml** (static layout elements):
- _Layout.cshtml header structure (initially)
- _Layout.cshtml footer structure (initially)
- _ViewImports.cshtml
- _ViewStart.cshtml

**Migrate to Angular** (dynamic/interactive components):
- All Index, Create, Edit, Details pages
- Search components
- Forms
- Tables and lists
- Interactive visualizations
- User-specific UI elements

---

## CSS/Styling Strategy

### SCSS Architecture

```
RushtonRoots.Web/ClientApp/src/
├── styles/
│   ├── _variables.scss          # Design tokens
│   ├── _mixins.scss              # Reusable mixins
│   ├── _typography.scss          # Font styles
│   ├── _layout.scss              # Layout utilities
│   ├── _animations.scss          # Animation keyframes
│   ├── _utilities.scss           # Utility classes
│   ├── themes/
│   │   ├── _light-theme.scss     # Light theme
│   │   └── _dark-theme.scss      # Dark theme (future)
│   └── material/
│       └── _material-theme.scss  # Material theming
└── styles.scss                   # Main import file
```

### Naming Conventions

- **BEM Methodology**: Block__Element--Modifier
- **Component-scoped styles**: Use Angular component styles
- **Utility classes**: Use for spacing, typography, etc.
- **Avoid inline styles**: Use classes instead

### CSS Best Practices

1. **Mobile-First**: Write styles for mobile, then add breakpoints for larger screens
2. **Design Tokens**: Use SCSS variables for colors, spacing, etc.
3. **Component Styles**: Scope styles to components when possible
4. **Global Styles**: Minimal global styles, prefer component styles
5. **Performance**: Minimize CSS size, avoid expensive selectors
6. **Maintainability**: Clear organization, good documentation

### Responsive Breakpoints

```scss
$breakpoint-xs: 0;
$breakpoint-sm: 600px;    // Small tablet
$breakpoint-md: 960px;    // Tablet
$breakpoint-lg: 1280px;   // Desktop
$breakpoint-xl: 1920px;   // Large desktop
```

---

## Accessibility & Responsiveness

### Accessibility Requirements

1. **WCAG 2.1 AA Compliance**: All pages must meet Level AA standards
2. **Keyboard Navigation**: All interactive elements must be keyboard accessible
3. **Screen Reader Support**: Proper ARIA labels and landmarks
4. **Color Contrast**: Minimum 4.5:1 for normal text, 3:1 for large text
5. **Focus Indicators**: Visible focus states on all interactive elements
6. **Alternative Text**: All images must have descriptive alt text
7. **Form Labels**: All form fields must have associated labels
8. **Error Messages**: Clear, descriptive error messages

### Responsive Design Requirements

1. **Mobile-First**: Design for mobile, enhance for desktop
2. **Touch Targets**: Minimum 44x44px touch targets on mobile
3. **Viewport Meta Tag**: Properly configured for mobile devices
4. **Flexible Layouts**: Use flexbox and grid for responsive layouts
5. **Responsive Images**: Use srcset and responsive image techniques
6. **Breakpoints**: Support all common device sizes
7. **Orientation**: Support both portrait and landscape
8. **Testing**: Test on real devices, not just emulators

---

## Testing Strategy

### Component Testing

1. **Unit Tests**: Test component logic in isolation
2. **Component Rendering**: Test that components render correctly
3. **User Interactions**: Test button clicks, form submissions, etc.
4. **Props/Inputs**: Test different input combinations
5. **Outputs/Events**: Test that events are emitted correctly
6. **Edge Cases**: Test error states, empty states, loading states

### Visual Testing

1. **Snapshot Tests**: Detect unintended visual changes
2. **Visual Regression**: Compare screenshots across builds
3. **Cross-Browser**: Test in Chrome, Firefox, Safari, Edge
4. **Responsive**: Test at different screen sizes
5. **Accessibility**: Automated accessibility tests

### E2E Testing

1. **User Workflows**: Test complete user journeys
2. **Critical Paths**: Login, create person, search, etc.
3. **Error Handling**: Test error scenarios
4. **Performance**: Monitor load times and performance
5. **Mobile**: Test mobile-specific interactions

---

## Success Metrics

### User Experience Metrics

1. **Task Completion Rate**: % of users who complete key tasks
2. **Time on Task**: Average time to complete key workflows
3. **Error Rate**: % of tasks completed with errors
4. **User Satisfaction**: NPS score, user surveys
5. **Accessibility**: % of WCAG criteria met

### Technical Metrics

1. **Page Load Time**: < 2 seconds for initial load
2. **Time to Interactive**: < 3 seconds
3. **Lighthouse Score**: > 90 for Performance, Accessibility, Best Practices
4. **Bundle Size**: < 500KB initial bundle
5. **Code Coverage**: > 80% test coverage
6. **Zero Critical Bugs**: No P0 bugs in production

### Business Metrics

1. **User Engagement**: Monthly active users
2. **Feature Adoption**: % of users using new features
3. **Retention**: User retention rate month-over-month
4. **Support Tickets**: Decrease in UI-related support tickets
5. **User Growth**: New user registrations

---

## Next Steps

### Immediate Actions (Next 4 Weeks)

1. **Week 1**: Install Angular Material, configure theming
2. **Week 2**: Create design tokens and global SCSS
3. **Week 3**: Build first 3 core components (PersonCard, SearchBar, PageHeader)
4. **Week 4**: Migrate Home page to use new components

### Medium-Term Goals (Months 2-6)

1. Complete Phase 1 and Phase 2 (Foundation and Layout)
2. Migrate Person management to Angular (Phase 3)
3. Migrate Household management to Angular (Phase 4)
4. Begin relationship management migration (Phase 5)

### Long-Term Vision (Months 7-16)

1. Complete all component migrations (Phases 6-8)
2. Achieve mobile optimization (Phase 9)
3. Ensure full accessibility compliance (Phase 10)
4. Optimize performance (Phase 11)
5. Comprehensive testing and documentation (Phase 12)

---

## Appendix

### Design Resources

- **Mockups**: [Figma/Sketch designs - TBD]
- **Style Guide**: [Style guide URL - TBD]
- **Component Library**: [Storybook URL - TBD]
- **Accessibility Guidelines**: [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)

### Reference Materials

- [Angular Material Documentation](https://material.angular.io/)
- [Material Design Guidelines](https://material.io/design)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/Understanding/)
- [Angular Best Practices](https://angular.io/guide/styleguide)
- [CSS Architecture (BEM)](http://getbem.com/)

### Glossary

- **BEM**: Block Element Modifier - CSS naming methodology
- **WCAG**: Web Content Accessibility Guidelines
- **ARIA**: Accessible Rich Internet Applications
- **PWA**: Progressive Web App
- **Material Design**: Design system by Google
- **Angular Material**: Official Material Design components for Angular

---

**Document Version**: 1.0  
**Last Updated**: December 2025  
**Next Review**: January 2026
