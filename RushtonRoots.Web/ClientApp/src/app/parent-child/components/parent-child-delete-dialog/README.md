# ParentChildDeleteDialogComponent

## Overview

The `ParentChildDeleteDialogComponent` is a comprehensive Angular Material dialog component designed for confirming the deletion of parent-child relationships in the RushtonRoots genealogy application. This component provides detailed impact analysis, multiple deletion options, and safety checks to ensure users make informed decisions when removing critical family tree connections.

## Features

### Core Functionality

- **Relationship Summary Display**: Shows parent and child information with photos, names, dates, and lifespan
- **Impact Analysis**: Calculates and displays the impact of deletion on:
  - Lineage connections (ancestors/descendants lost)
  - Sibling relationships
  - Family tree visualization
  - Associated evidence, photos, and stories
- **Multiple Deletion Options**:
  - **Soft Delete**: Mark as deleted (restorable by admin)
  - **Mark as Disputed**: Flag relationship as uncertain/questionable
  - **Hard Delete**: Permanent removal (admin only)
- **Family Tree Context**: Placeholder for mini family tree visualization
- **Safety Confirmations**: Required checkbox confirmation before deletion
- **Dispute Reason**: Required text field when marking as disputed

### User Interface

- **Material Design**: Professional UI using Angular Material components
- **Responsive Layout**: Mobile-friendly design with adaptive layouts
- **Accessibility**: WCAG 2.1 AA compliant with ARIA labels and keyboard navigation
- **Dynamic Warnings**: Context-sensitive messages based on selected deletion type
- **Impact Severity Indicators**: Color-coded warnings (critical/high/medium/low)

## Component Architecture

### File Structure

```
parent-child-delete-dialog/
├── parent-child-delete-dialog.component.ts    # Component logic
├── parent-child-delete-dialog.component.html  # Template
├── parent-child-delete-dialog.component.scss  # Styles
└── README.md                                   # This file
```

### Dependencies

- **Angular Core**: Component, Input, Output, EventEmitter, OnInit
- **Angular Forms**: FormBuilder, FormGroup, Validators, ReactiveFormsModule
- **Angular Material**: 
  - MatCardModule
  - MatButtonModule
  - MatIconModule
  - MatChipsModule
  - MatFormFieldModule
  - MatInputModule
  - MatRadioModule
  - MatCheckboxModule

## Usage

### As Angular Element

The component is registered as an Angular Element for embedding in Razor views:

```html
<app-parent-child-delete-dialog
  relationship-id="1"
  parent-id="100"
  parent-name="John Doe"
  parent-photo-url="/photos/john-doe.jpg"
  parent-birth-date="1950-01-15"
  parent-death-date="2020-05-20"
  parent-is-deceased="true"
  child-id="200"
  child-name="Jane Doe"
  child-photo-url="/photos/jane-doe.jpg"
  child-birth-date="1975-03-10"
  child-is-deceased="false"
  relationship-type="Biological"
  is-verified="true"
  related-data='{"lineageImpact":{"ancestorsLost":5,"descendantsLost":3,"generationsAffected":2},"siblings":2,"treeNodes":8,"evidence":0,"photos":1,"stories":0}'
  is-admin="true">
</app-parent-child-delete-dialog>
```

### Event Handlers

```javascript
const deleteDialog = document.querySelector('app-parent-child-delete-dialog');

// Handle delete confirmed
deleteDialog.addEventListener('deleteConfirmed', function(event) {
  const options = event.detail;
  console.log('Delete type:', options.deleteType);
  console.log('Dispute reason:', options.disputeReason);
  console.log('Confirmed:', options.confirmed);
  // Submit to backend...
});

// Handle delete cancelled
deleteDialog.addEventListener('deleteCancelled', function() {
  console.log('User cancelled deletion');
  // Redirect or close dialog...
});
```

## Input Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `relationship-id` | number | Yes | ID of the parent-child relationship |
| `parent-id` | number | Yes | ID of the parent person |
| `parent-name` | string | Yes | Full name of the parent |
| `parent-photo-url` | string | No | URL to parent's photo |
| `parent-birth-date` | Date/string | No | Parent's birth date |
| `parent-death-date` | Date/string | No | Parent's death date |
| `parent-is-deceased` | boolean | Yes | Whether parent is deceased |
| `child-id` | number | Yes | ID of the child person |
| `child-name` | string | Yes | Full name of the child |
| `child-photo-url` | string | No | URL to child's photo |
| `child-birth-date` | Date/string | No | Child's birth date |
| `child-death-date` | Date/string | No | Child's death date |
| `child-is-deceased` | boolean | Yes | Whether child is deceased |
| `relationship-type` | string | Yes | Type of relationship (Biological, Adopted, etc.) |
| `is-verified` | boolean | Yes | Whether relationship is verified |
| `related-data` | string (JSON) | Yes | JSON object with impact data |
| `is-admin` | boolean | Yes | Whether current user is admin (enables hard delete) |

## Output Events

### deleteConfirmed

Emitted when user confirms deletion. Event detail includes:

```typescript
{
  deleteType: 'soft' | 'disputed' | 'hard',
  disputeReason?: string,  // Only when deleteType is 'disputed'
  confirmed: boolean       // Always true when event fires
}
```

### deleteCancelled

Emitted when user cancels the deletion. No event detail.

## Related Data Structure

The `related-data` input expects a JSON string with the following structure:

```typescript
{
  lineageImpact: {
    ancestorsLost: number,      // Ancestors child will lose access to
    descendantsLost: number,    // Descendants parent will lose access to
    generationsAffected: number // Number of generations impacted
  },
  siblings: number,             // Sibling relationships affected
  treeNodes: number,            // Family tree nodes disconnected
  evidence: number,             // Evidence items attached
  photos: number,               // Photos tagged with both
  stories: number               // Stories about relationship
}
```

## TypeScript Models

See `/parent-child/models/parent-child-delete.model.ts` for full type definitions:

- `ParentChildDeleteDialogData`
- `ParentChildRelatedData`
- `LineageImpact`
- `RelationshipImpactSummary`
- `ParentChildDeleteOptions`
- `ParentChildDeleteResult`

## Styling

The component uses a comprehensive SCSS file with:

- **Responsive Design**: Breakpoints for mobile, tablet, and desktop
- **Color Coding**: Severity-based colors for impact warnings
- **Accessibility**: High contrast mode and reduced motion support
- **Material Design**: Consistent with Angular Material theme
- **Custom Classes**: 
  - `.delete-dialog-card` - Main container
  - `.relationship-summary` - Parent/child info section
  - `.impact-warnings` - Impact analysis section
  - `.family-tree-context` - Tree visualization section
  - `.delete-form` - Delete options form
  - `.action-buttons` - Button container

## Backend Integration

### Controller Action (Example)

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id, string deleteType, string? disputeReason)
{
    var relationship = await _context.ParentChild.FindAsync(id);
    if (relationship == null) return NotFound();
    
    switch (deleteType)
    {
        case "soft":
            relationship.IsDeleted = true;
            relationship.DeletedDateTime = DateTime.UtcNow;
            break;
        case "disputed":
            relationship.IsDisputed = true;
            relationship.DisputedDateTime = DateTime.UtcNow;
            relationship.DisputeReason = disputeReason;
            break;
        case "hard":
            if (!User.IsInRole("Admin")) return Forbid();
            _context.ParentChild.Remove(relationship);
            break;
    }
    
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}
```

## Testing

### Unit Tests (To Do)

- Component initialization
- Form validation
- Delete type selection
- Impact summary calculation
- Event emission
- Dispute reason validation

### Integration Tests (To Do)

- Full delete workflow (soft)
- Full disputed workflow
- Admin hard delete workflow
- Cancel workflow
- Backend integration

## Known Issues & Future Enhancements

### Current Limitations

1. **EF Migration Pending**: Domain model updated but migration not created due to build errors in unrelated Partnership views
2. **Static Related Data**: Currently uses placeholder data; needs backend calculation
3. **No Mini Tree Integration**: Family tree context shows placeholder; awaits FamilyTreeMiniComponent integration
4. **No Unit Tests**: Test infrastructure not yet set up

### Planned Enhancements

1. **Integrate FamilyTreeMiniComponent**: Show actual family tree visualization
2. **Real-time Impact Calculation**: Backend service to calculate actual impact
3. **Restore Functionality**: Admin UI to restore soft-deleted relationships
4. **Dispute Review**: Admin UI to review and resolve disputed relationships
5. **Undo Feature**: Short-term undo for accidental deletions
6. **Audit Trail**: Log all deletion actions with timestamps and reasons

## Version History

- **v1.0.0** (2025-12-16): Initial implementation
  - Complete component with all deletion options
  - Material Design UI with responsive layout
  - Impact analysis with dynamic severity indicators
  - Integration with Delete.cshtml Razor view
  - Domain model updates for soft delete and disputed status

## Contributors

- GitHub Copilot Agent (Initial implementation)
- GLGRushton (Project owner)

## License

Part of the RushtonRoots genealogy application. See main project LICENSE for details.
