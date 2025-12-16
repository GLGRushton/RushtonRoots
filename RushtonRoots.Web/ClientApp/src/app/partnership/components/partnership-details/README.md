# PartnershipDetailsComponent

## Overview

The `PartnershipDetailsComponent` is a comprehensive Angular component for displaying detailed information about partnerships/relationships in the RushtonRoots genealogy application. It provides a rich, tabbed interface with multiple views of partnership data.

## Phase

**Phase 4.2: Partnership Details** (Week 2-3)

## Features

### 5-Tab Interface

1. **Overview Tab** - Partnership summary
   - Both partners with clickable avatars
   - Partnership type and status with icons
   - Date information (start, end, duration)
   - Location display
   - Editable notes and description
   - Created/updated timestamps

2. **Timeline Tab** - Chronological partnership events
   - Integrates existing `PartnershipTimelineComponent`
   - Auto-populated start and end events
   - Visual timeline with icons and colors

3. **Children Tab** - Children from this partnership
   - Grid layout of child cards
   - Child photos (or default avatars)
   - Clickable cards to navigate to child details
   - Birth dates and ages
   - Deceased indicators

4. **Media Tab** - Photo gallery
   - Grid layout of photos
   - Photo title and description
   - Upload dates
   - Set as primary photo (edit mode)
   - Delete photo (edit mode)

5. **Events Tab** - Partnership events and celebrations
   - Event cards with icons
   - Event titles, dates, descriptions
   - Location information
   - Event type categorization

### Action Buttons

- **Edit Button** (Admin/HouseholdAdmin only) - Navigate to edit form
- **Delete Button** (Admin only) - Navigate to delete confirmation

### Edit-in-Place

- Description field
- Notes field
- Save/Cancel buttons

### Partner Navigation

- Partner avatars are clickable
- Navigate to person details for each partner

## Component Structure

### TypeScript Models

Located in `/partnership/models/partnership.model.ts`:

- `PartnershipDetails` - Main partnership data interface
- `PartnershipChild` - Child information
- `PartnershipPhoto` - Photo/media data
- `PartnershipEvent` - Event data
- `PartnershipDetailsTab` - Tab configuration

### Input Properties

```typescript
@Input() partnership!: PartnershipDetails;
@Input() children: PartnershipChild[] = [];
@Input() photos: PartnershipPhoto[] = [];
@Input() events: PartnershipEvent[] = [];
@Input() canEdit = false;
@Input() canDelete = false;
@Input() initialTab: number = 0;
```

### Output Events

```typescript
@Output() editClicked = new EventEmitter<number>();
@Output() deleteClicked = new EventEmitter<number>();
@Output() personClicked = new EventEmitter<number>();
@Output() childClicked = new EventEmitter<number>();
@Output() photoUploaded = new EventEmitter<File>();
@Output() photoDeleted = new EventEmitter<number>();
@Output() photoPrimaryChanged = new EventEmitter<number>();
@Output() eventAdded = new EventEmitter<PartnershipEvent>();
```

## Razor View Integration

### Details.cshtml

The component is integrated as an Angular Element in `/Views/Partnership/Details.cshtml`:

```html
<app-partnership-details
    partnership='@JsonSerializer.Serialize(partnershipDetails, jsonOptions)'
    children='@JsonSerializer.Serialize(children, jsonOptions)'
    photos='@JsonSerializer.Serialize(photos, jsonOptions)'
    events='@JsonSerializer.Serialize(events, jsonOptions)'
    can-edit="@canEdit.ToString().ToLower()"
    can-delete="@canDelete.ToString().ToLower()"
    initial-tab="0">
</app-partnership-details>
```

### Data Transformation

Server-side data transformation from `PartnershipViewModel` to `PartnershipDetails`:

- Partnership type display mapping
- Status calculation (current vs ended)
- Duration calculation
- Date formatting (ISO 8601 for Angular)
- Role-based permission checks

### Event Handlers

JavaScript event handlers in Details.cshtml:

- `editClicked` → Navigate to `/Partnership/Edit/{id}`
- `deleteClicked` → Navigate to `/Partnership/Delete/{id}`
- `personClicked` → Navigate to `/Person/Details/{personId}`
- `childClicked` → Navigate to `/Person/Details/{childId}`
- `photoUploaded` → TODO: Backend implementation
- `photoDeleted` → TODO: Backend implementation
- `photoPrimaryChanged` → TODO: Backend implementation
- `eventAdded` → TODO: Backend implementation

## Angular Module Registration

### PartnershipModule

Component declared and exported in `/partnership/partnership.module.ts`:

```typescript
import { PartnershipDetailsComponent } from './components/partnership-details/partnership-details.component';

@NgModule({
  declarations: [
    PartnershipDetailsComponent
  ],
  exports: [
    PartnershipDetailsComponent
  ]
})
```

### Angular Element Registration

Registered in `app.module.ts`:

```typescript
import { PartnershipDetailsComponent } from './partnership/components/partnership-details/partnership-details.component';

// In bootstrapping
safeDefine('app-partnership-details', PartnershipDetailsComponent);
```

## Styling

SCSS file: `/partnership/components/partnership-details/partnership-details.component.scss`

Features:
- Material Design theming
- Responsive layout (mobile, tablet, desktop)
- Partner photo display with circular avatars
- Tab badges for counts
- Card grid layouts
- Hover effects
- Accessibility focus indicators

Uses variables from `/styles/_variables.scss`:
- `$spacing-*` for consistent spacing
- `$breakpoint-*` for responsive breakpoints
- `$shadow-*` for elevation
- `$border-radius-*` for rounded corners

## Accessibility

- ARIA labels on all interactive elements
- Semantic HTML structure
- Keyboard navigation support
- Focus indicators
- Alt text on images
- WCAG 2.1 AA compliance

## Performance

- Lazy loading via tabs (only active tab rendered)
- Efficient change detection
- Badge updates on data changes
- Minimal re-renders

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Testing Status

- ⏳ Unit tests pending (test infrastructure setup required)
- ⏳ E2E tests pending (Playwright/Cypress configuration required)
- ⏳ Manual testing recommended
- ✅ TypeScript compilation verified
- ✅ Build successful

## TODO

### Backend Implementation

1. **Children Relationship Fetching**
   - Query children from ParentChild table
   - Include birth dates and ages
   - Include deceased status

2. **Photo Management**
   - Photo upload endpoint
   - Photo delete endpoint
   - Set primary photo endpoint
   - Fetch photos from database

3. **Event Management**
   - Event creation endpoint
   - Fetch partnership events
   - Event type categorization

4. **Partnership Entity Enhancement**
   - Add `Location` field (string, nullable)
   - Add `Notes` field (string, nullable)
   - Add `Description` field (string, nullable)

### Testing

1. Create unit tests for component
2. Create integration tests for Razor view
3. Manual end-to-end testing
4. Cross-browser testing
5. Mobile responsiveness testing

## Related Components

- `PartnershipIndexComponent` - Partnership list view
- `PartnershipCardComponent` - Partnership card display
- `PartnershipFormComponent` - Partnership create/edit form
- `PartnershipTimelineComponent` - Timeline visualization (used in Details)

## Documentation

- Main documentation: `/docs/UpdateDesigns.md` - Phase 4.2
- Architecture: `/PATTERNS.md`
- Project structure: `/PROJECT_STRUCTURE_IMPLEMENTATION.md`

## Version History

- **2025-12-16**: Initial implementation (Phase 4.2)
  - Created component with 5-tab interface
  - Integrated with existing timeline component
  - Razor view integration complete
  - TypeScript models defined
  - Build verified successful
