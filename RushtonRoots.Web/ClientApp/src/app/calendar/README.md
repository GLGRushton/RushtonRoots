# Calendar Module - Phase 8.2

## Overview

The Calendar Module provides comprehensive event management functionality for the RushtonRoots application. It includes an interactive calendar component powered by FullCalendar, event cards, and dialogs for creating, editing, and responding to events.

## Components

### 1. CalendarComponent (`app-calendar`)

Main calendar component with FullCalendar integration supporting multiple views, event filtering, and interactive event management.

**Features:**
- Month, week, day, and list views
- Event drag-and-drop and resizing
- Event filtering by category and RSVP status
- Search functionality
- Responsive design
- Event click handlers

**Usage:**

```html
<app-calendar
  [events]="events"
  [initialView]="'dayGridMonth'"
  [canEdit]="true"
  (eventClick)="handleEventClick($event)"
  (eventCreate)="handleEventCreate($event)"
  (eventEdit)="handleEventEdit($event)"
  (eventDelete)="handleEventDelete($event)"
  (dateSelect)="handleDateSelect($event)">
</app-calendar>
```

**Inputs:**
- `events: CalendarEvent[]` - Array of calendar events
- `initialView: CalendarView` - Initial calendar view (default: month)
- `canEdit: boolean` - Whether user can edit events (default: true)

**Outputs:**
- `eventClick: EventEmitter<CalendarEvent>` - Fired when event is clicked
- `eventCreate: EventEmitter<Date>` - Fired when create event button is clicked
- `eventEdit: EventEmitter<CalendarEvent>` - Fired when event is edited
- `eventDelete: EventEmitter<CalendarEvent>` - Fired when event is deleted
- `dateSelect: EventEmitter<{start: Date, end: Date}>` - Fired when date range is selected

### 2. EventCardComponent (`app-event-card`)

Displays event information in a card format with RSVP status, attendee information, and quick actions.

**Features:**
- Event metadata display
- RSVP status indicator
- Attendee count and status
- Category badges
- Quick action buttons
- Responsive design

**Usage:**

```html
<app-event-card
  [event]="event"
  [canEdit]="true"
  [compact]="false"
  [elevation]="2"
  (viewClick)="handleViewClick($event)"
  (editClick)="handleEditClick($event)"
  (deleteClick)="handleDeleteClick($event)"
  (rsvpClick)="handleRsvpClick($event)">
</app-event-card>
```

**Inputs:**
- `event: CalendarEvent` - Event to display (required)
- `canEdit: boolean` - Whether user can edit event (default: false)
- `compact: boolean` - Compact display mode (default: false)
- `elevation: number` - Card elevation (default: 2)

**Outputs:**
- `viewClick: EventEmitter<CalendarEvent>` - View event details
- `editClick: EventEmitter<CalendarEvent>` - Edit event
- `deleteClick: EventEmitter<CalendarEvent>` - Delete event
- `rsvpClick: EventEmitter<CalendarEvent>` - RSVP to event

### 3. EventFormDialogComponent (`app-event-form-dialog`)

Dialog for creating and editing calendar events with comprehensive form validation.

**Features:**
- Title, description, date/time fields
- Location input
- Attendee selection
- Recurring event configuration
- Multiple reminders
- Privacy settings
- Form validation

**Usage:**

```typescript
// In TypeScript component
import { MatDialog } from '@angular/material/dialog';
import { EventFormDialogComponent } from './calendar/components/event-form-dialog/event-form-dialog.component';

constructor(private dialog: MatDialog) {}

openEventDialog(event?: CalendarEvent, startDate?: Date) {
  const dialogRef = this.dialog.open(EventFormDialogComponent, {
    width: '900px',
    maxHeight: '90vh',
    data: { event, startDate }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      // Handle event creation/update
      console.log('Event data:', result);
    }
  });
}
```

**Dialog Data:**
- `event?: CalendarEvent` - Event to edit (optional, for edit mode)
- `startDate?: Date` - Initial start date (optional, for create mode)

**Returns:**
- `EventFormData` - Form data with event details, or null if cancelled

### 4. EventRsvpDialogComponent (`app-event-rsvp-dialog`)

Dialog for responding to event invitations with RSVP status, comment, and guest count.

**Features:**
- RSVP status selection (attending, maybe, declined)
- Optional comment field
- Guest count selector
- Event information display

**Usage:**

```typescript
import { EventRsvpDialogComponent } from './calendar/components/event-rsvp-dialog/event-rsvp-dialog.component';

openRsvpDialog(event: CalendarEvent) {
  const dialogRef = this.dialog.open(EventRsvpDialogComponent, {
    width: '600px',
    data: { event }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      // Handle RSVP submission
      console.log('RSVP data:', result);
    }
  });
}
```

**Dialog Data:**
- `event: CalendarEvent` - Event to RSVP to (required)

**Returns:**
- `RsvpFormData` - RSVP data with status, comment, and guest count, or null if cancelled

### 5. EventDetailsDialogComponent (`app-event-details-dialog`)

Dialog for viewing full event details with attendee list, RSVP status, and export functionality.

**Features:**
- Full event information display
- Attendee list with RSVP status
- Edit/delete actions
- Export to iCal functionality
- RSVP button
- Recurring event information

**Usage:**

```typescript
import { EventDetailsDialogComponent } from './calendar/components/event-details-dialog/event-details-dialog.component';

openEventDetails(event: CalendarEvent, canEdit: boolean) {
  const dialogRef = this.dialog.open(EventDetailsDialogComponent, {
    width: '700px',
    maxHeight: '90vh',
    data: { event, canEdit }
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      // Handle actions (edit, delete, rsvp, export)
      console.log('Action:', result.action, 'Data:', result.data);
    }
  });
}
```

**Dialog Data:**
- `event: CalendarEvent` - Event to display (required)
- `canEdit: boolean` - Whether user can edit event (required)

**Returns:**
- `{ action: string, data?: any, event?: CalendarEvent }` - Action result, or null if cancelled

## Models

### CalendarEvent

```typescript
interface CalendarEvent {
  id: string;
  title: string;
  description?: string;
  start: Date;
  end: Date;
  allDay?: boolean;
  location?: string;
  organizer: EventAttendee;
  attendees: EventAttendee[];
  recurrence?: RecurrenceRule;
  reminders: EventReminder[];
  color?: string;
  category?: EventCategory;
  isPrivate?: boolean;
  rsvpRequired?: boolean;
  createdAt: Date;
  updatedAt: Date;
}
```

### EventAttendee

```typescript
interface EventAttendee {
  id: string;
  name: string;
  email: string;
  avatarUrl?: string;
  rsvpStatus: RsvpStatus;
  rsvpComment?: string;
  guestCount?: number;
  respondedAt?: Date;
}
```

### RecurrenceRule

```typescript
interface RecurrenceRule {
  frequency: RecurrenceFrequency; // daily, weekly, monthly, yearly
  interval?: number; // e.g., every 2 weeks
  endDate?: Date;
  occurrences?: number;
  daysOfWeek?: number[]; // 0 = Sunday, 1 = Monday, etc.
  dayOfMonth?: number;
  monthOfYear?: number;
}
```

### EventReminder

```typescript
interface EventReminder {
  id: string;
  type: ReminderType; // email, push, both
  minutesBefore: number;
  enabled: boolean;
}
```

## Enums

### EventCategory

- Birthday
- Anniversary
- Reunion
- Holiday
- Memorial
- Wedding
- Baptism
- Graduation
- Meeting
- Other

### RsvpStatus

- Pending
- Attending
- Maybe
- Declined
- NotResponded

### RecurrenceFrequency

- Daily
- Weekly
- Monthly
- Yearly

## Usage in Razor Views

```html
<!-- In .cshtml file -->
<app-calendar></app-calendar>

<!-- Or with specific configuration -->
<app-calendar
  initial-view="timeGridWeek"
  can-edit="true">
</app-calendar>

<!-- Event card -->
<app-event-card
  event='@JsonConvert.SerializeObject(event)'
  can-edit="true">
</app-event-card>
```

## C# Integration Example

```csharp
// In Controller
public IActionResult Calendar()
{
    var events = _eventService.GetUpcomingEvents();
    ViewData["Events"] = JsonConvert.SerializeObject(events);
    return View();
}

// In View
<script>
  const eventsData = @Html.Raw(ViewData["Events"]);
  const calendarElement = document.querySelector('app-calendar');
  if (calendarElement) {
    calendarElement.setAttribute('events', JSON.stringify(eventsData));
  }
</script>
```

## Dependencies

- **@fullcalendar/core**: Core FullCalendar functionality
- **@fullcalendar/angular**: Angular integration
- **@fullcalendar/daygrid**: Day grid views
- **@fullcalendar/timegrid**: Time grid views
- **@fullcalendar/list**: List view
- **@fullcalendar/interaction**: Drag and drop, selection
- **ical-generator**: iCal export (optional)

## Styling

All components follow the RushtonRoots design system with Material Design styling. Custom SCSS variables can be found in `/styles/variables.scss`.

## Accessibility

- All components are keyboard navigable
- ARIA labels and roles are properly set
- Screen reader support
- Focus indicators on interactive elements
- Semantic HTML structure

## Responsive Design

- Mobile-first approach
- Breakpoints at 600px and 960px
- Touch-friendly button sizes
- Adaptive layouts for small screens
- Swipe gestures on mobile (via FullCalendar)

## Browser Compatibility

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Future Enhancements

- Real-time event updates via SignalR
- Calendar sharing and permissions
- Event templates
- Email reminders
- Push notifications
- Calendar sync with external calendars (Google Calendar, Outlook)
- Event conflicts detection
- Suggested times for events

## Support

For issues or questions, please refer to the main RushtonRoots documentation or contact the development team.
