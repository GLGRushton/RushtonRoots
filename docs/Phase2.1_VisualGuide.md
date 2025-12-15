# Phase 2.1 Visual Design Guide

## Header Component Visual Layout

### Desktop View (≥960px)
```
┌────────────────────────────────────────────────────────────────────────────┐
│  🌳 Rushton Roots  │  Home People Households...  │  🔍 Search  🔔  👤 User ▼ │
└────────────────────────────────────────────────────────────────────────────┘
```

**Header Bar**:
- Background: Linear gradient from #2e7d32 → #388e3c → #43a047 (forest green)
- Height: Auto (padding: 16px 0)
- Position: Sticky (stays at top when scrolling)
- Shadow: 0 4px 12px rgba(46, 125, 50, 0.3)

**Left Section - Logo**:
- 🌳 Tree emoji icon (32px)
- "Rushton Roots" text (28px, bold, white)
- Clickable to navigate to home

**Center Section - Navigation** (Desktop):
- Horizontal pill-shaped container
- Background: rgba(255, 255, 255, 0.1) (translucent white)
- Border-radius: 20px
- Padding: 8px 16px
- Nav items with icons and labels
- Hover effect: rgba(255, 255, 255, 0.2)
- Active item: rgba(255, 255, 255, 0.25) with bold text

**Right Section - Actions**:
1. **Search Field**:
   - Width: 300px
   - Background: rgba(255, 255, 255, 0.15)
   - Border-radius: 20px
   - White text input
   - 🔍 icon prefix
   - Placeholder: "Search..."

2. **Notification Bell**:
   - 🔔 icon button
   - Badge for count (currently hidden)
   - White color

3. **User Menu**:
   - Pill-shaped button
   - Background: rgba(255, 255, 255, 0.15)
   - Border-radius: 25px
   - Padding: 8px 16px
   - Contains:
     - Avatar circle (36px, white bg, green initials)
     - User name (14px, bold)
     - Role text (11px, light)
     - ▼ dropdown icon

### Mobile View (<600px)
```
┌────────────────────────────────┐
│  ☰  🌳 Rushton  👤            │
├────────────────────────────────┤  (when menu open)
│  🏠 Home                       │
│  👥 People                     │
│  🏠 Households                 │
│  ❤️ Partnerships               │
│  👨‍👩‍👧‍👦 Parent-Child              │
│  🍽️ Recipes                    │
│  📖 Stories                    │
│  🎉 Traditions                 │
│  📄 Wiki                       │
└────────────────────────────────┘
```

**Mobile Header Changes**:
- ☰ Hamburger menu button (left)
- Logo text shortened to "Rushton"
- Search field hidden
- User info text hidden (avatar only)
- Dropdown icon hidden
- Navigation expands below header when toggled

**Mobile Navigation**:
- Full-width vertical list
- Icon + label for each item
- White text on green background
- 48px min height for touch targets
- Active item highlighted
- Smooth slide-in animation

## User Menu Dropdown

### Desktop Dropdown Menu
```
┌─────────────────────────────┐
│  👤   John Doe              │
│       System Admin          │
├─────────────────────────────┤
│  👤 My Profile              │
│  📊 Dashboard               │
│  👥 Add User                │ (admin only)
│  ⚙️ Admin Panel             │ (admin only)
├─────────────────────────────┤
│  🚪 Logout                  │
└─────────────────────────────┘
```

**Dropdown Styling**:
- Width: Auto (min 250px)
- Background: White
- Border-radius: 12px
- Shadow: 0 8px 16px rgba(0,0,0,0.15)
- Margin-top: 8px from trigger

**Header Section**:
- Large avatar (48px)
- Gradient background on avatar: #2e7d32 → #66bb6a
- User name (16px, bold)
- Role badge with color:
  - Admin: Red background, red text
  - Household Admin: Light green background, green text
  - Member: Light green background, primary text

**Menu Items**:
- 48px min height
- Icon (left, 20px)
- Label text (16px)
- Hover: Light gray background
- Icons turn green on hover

## Color Coding

### Role Badges
1. **System Admin**:
   - Background: rgba(211, 47, 47, 0.1) (light red)
   - Text: #d32f2f (red)
   - Border: None

2. **Household Admin**:
   - Background: rgba(102, 187, 106, 0.1) (light green)
   - Text: #388e3c (forest green)
   - Border: None

3. **Family Member**:
   - Background: rgba(46, 125, 50, 0.1) (light green)
   - Text: #2e7d32 (primary green)
   - Border: None

## Navigation Icons

| Route | Icon | Label |
|-------|------|-------|
| / | 🏠 home | Home |
| /Person | 👥 people | People |
| /Household | 🏠 house | Households |
| /Partnership | ❤️ favorite | Partnerships |
| /ParentChild | 👨‍👩‍👧‍👦 family_restroom | Parent-Child |
| /Recipe | 🍽️ restaurant | Recipes |
| /StoryView | 📖 book | Stories |
| /Tradition | 🎉 celebration | Traditions |
| /Wiki | 📄 description | Wiki |

## Responsive Breakpoints

| Breakpoint | Width | Changes |
|------------|-------|---------|
| Mobile | < 600px | Hamburger menu, vertical nav, compact user menu |
| Tablet | 600px - 959px | Horizontal nav appears, search visible |
| Desktop | ≥ 960px | Full layout with all features |
| Large Desktop | ≥ 1920px | Same as desktop |

## Animation & Transitions

1. **Hover Effects**: 0.3s ease transition
2. **Menu Toggle**: Smooth slide-in/out
3. **Dropdown**: Fade-in 200ms
4. **Nav Items**: translateY(-2px) on hover
5. **Active States**: Background color transition

## Accessibility Features

1. **Keyboard Navigation**:
   - Tab through all interactive elements
   - Enter/Space to activate buttons
   - Escape to close dropdowns

2. **ARIA Labels**:
   - aria-label="Toggle navigation menu" (hamburger)
   - aria-label="User menu" (user button)
   - aria-label="Notifications" (bell icon)
   - aria-label="Global search" (search input)
   - aria-current="page" (active nav item)

3. **Focus Indicators**:
   - 2px solid outline in primary color
   - 2px offset from element
   - Visible on all interactive elements

4. **Screen Reader Support**:
   - Semantic HTML structure
   - Descriptive labels
   - Role announcements
   - State changes announced

## Design Tokens Used

```scss
// Colors
$primary: #2e7d32
$primary-light: #4caf50
$primary-dark: #1b5e20
$accent: #66bb6a
$warn: #d32f2f

// Spacing
$spacing-xs: 4px
$spacing-sm: 8px
$spacing-md: 16px
$spacing-lg: 24px

// Borders
$border-radius-md: 8px
$border-radius-lg: 12px

// Shadows
$shadow-sm: 0 2px 4px rgba(0,0,0,0.1)
$shadow-md: 0 4px 8px rgba(0,0,0,0.12)
$shadow-lg: 0 8px 16px rgba(0,0,0,0.15)
```

## Material Components Used

- `<mat-toolbar>` - Header bar
- `<mat-icon>` - All icons
- `<mat-button>` - All buttons
- `<mat-icon-button>` - Icon-only buttons
- `<mat-menu>` - User dropdown
- `<mat-form-field>` - Search input
- `<mat-badge>` - Notification count
- `<mat-nav-list>` - Mobile navigation
- `<mat-divider>` - Menu separators

## Visual Hierarchy

1. **Primary**: Logo and branding
2. **Secondary**: Navigation items
3. **Tertiary**: User actions (search, notifications, profile)
4. **Quaternary**: Dropdown menu items

## Consistency with Existing Design

The new header maintains:
- ✅ Green color palette
- ✅ Tree emoji branding
- ✅ Gradient backgrounds
- ✅ Rounded corners
- ✅ Clean, modern aesthetic
- ✅ Family-friendly visual style
- ✅ Professional quality

While adding:
- ✨ Material Design principles
- ✨ Better mobile experience
- ✨ Improved accessibility
- ✨ Consistent interactions
- ✨ Role-based UI
- ✨ Future-ready architecture
