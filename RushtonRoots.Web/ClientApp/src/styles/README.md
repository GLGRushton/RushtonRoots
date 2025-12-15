# RushtonRoots SCSS Architecture

This directory contains the structured SCSS files for the RushtonRoots design system.

## Directory Structure

```
styles/
├── _variables.scss      # Design tokens (colors, spacing, typography, etc.)
├── _mixins.scss         # Reusable SCSS mixins
├── _typography.scss     # Typography styles and utilities
├── _layout.scss         # Layout utilities and grid system
├── _animations.scss     # Animation keyframes and utilities
├── _utilities.scss      # Utility classes for spacing, display, etc.
├── themes/
│   ├── _light-theme.scss   # Light theme configuration (for future use)
│   └── _dark-theme.scss    # Dark theme configuration (for future use)
└── material/
    └── _material-theme.scss # Angular Material theme configuration (for future use)
```

## Usage

All SCSS partials are imported in the main `styles.scss` file at the root of the `src` directory.

### Design Tokens (_variables.scss)

The variables file contains all design tokens including:
- **Colors**: Primary, accent, semantic colors
- **Typography**: Font families, sizes, weights, line heights
- **Spacing**: 8px grid system (xs, sm, md, lg, xl, xxl)
- **Border Radius**: Consistent border radii
- **Shadows**: Elevation levels
- **Breakpoints**: Responsive breakpoints
- **Z-index**: Layering system

### Mixins (_mixins.scss)

Reusable SCSS mixins for common patterns:
- `respond-to($breakpoint)` - Responsive media queries
- `elevation($level)` - Box shadow levels
- `flex-center` - Flexbox centering
- `truncate-text` - Text ellipsis
- `line-clamp($lines)` - Multi-line truncation
- `transition($properties, $duration, $easing)` - Smooth transitions
- `button-reset` - Reset button styles
- `card` - Card component styles
- `focus-visible` - Accessible focus indicators

### Typography (_typography.scss)

Typography styles including:
- Heading styles (h1-h6)
- Paragraph styles
- Link styles
- List styles
- Text utility classes

### Layout (_layout.scss)

Layout utilities including:
- Container and container-fluid
- Flexbox utilities
- Grid system
- Display utilities
- Position utilities
- Width and height utilities

### Animations (_animations.scss)

Animation keyframes and utilities:
- `fadeIn`, `fadeInUp`, `fadeInDown`
- `slideInRight`, `slideInLeft`
- `spin`, `pulse`
- Transition utility classes

### Utilities (_utilities.scss)

Utility classes for:
- Spacing (margin and padding)
- Border radius
- Shadows
- Overflow
- Cursor
- User select
- Pointer events

## Adding New Styles

When adding new styles:

1. **For global design tokens**: Add to `_variables.scss`
2. **For reusable patterns**: Create a mixin in `_mixins.scss`
3. **For component-specific styles**: Use component SCSS files
4. **For utility classes**: Add to `_utilities.scss`

## Best Practices

- Use design tokens (variables) instead of hardcoded values
- Use mixins for repeated patterns
- Follow BEM naming convention for classes
- Keep component styles scoped to components
- Use utility classes for common spacing and display needs
- Mobile-first responsive design
- Ensure accessibility (focus states, contrast, etc.)

## Theme System

The theme system supports:
- Light theme (default)
- Dark theme (planned for future)
- Custom Material Design theming

Themes are configured in the `themes/` directory and integrated with Angular Material.
