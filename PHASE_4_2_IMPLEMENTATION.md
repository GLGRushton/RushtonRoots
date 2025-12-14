# Phase 4.2 Implementation: Collaboration Tools

## Overview
This document describes the implementation of Phase 4.2 from the ROADMAP.md - a comprehensive collaboration tools system that enables family members to coordinate activities, plan events, manage tasks, and collaborate on family content through comments.

## Implementation Date
December 2025

## Features Implemented

### 1. Shared Family Calendar
- **Family Events**: Create and manage family events (reunions, birthdays, anniversaries, meetings, etc.)
- **Event Details**: Rich event information including title, description, location, start/end times
- **All-Day Events**: Support for all-day events
- **Recurring Events**: Framework for recurring events (daily, weekly, monthly, yearly)
- **Event Types**: Categorize events by type (Reunion, Birthday, Anniversary, Meeting, etc.)
- **Household Association**: Link events to specific households
- **Event Cancellation**: Mark events as cancelled while preserving history
- **Upcoming Events**: Query upcoming events with customizable count
- **Date Range Queries**: Find events within specific date ranges

### 2. Event Planning & RSVP Functionality
- **RSVP System**: Users can respond to event invitations
- **RSVP Status**: Attending, NotAttending, Maybe, Pending
- **Guest Count**: Track number of additional guests
- **RSVP Notes**: Users can add notes with their RSVP
- **Response Tracking**: Track when users responded
- **RSVP Counts**: Automatic calculation of attending, not attending, and maybe counts
- **Unique RSVPs**: One RSVP per user per event (enforced at database level)
- **RSVP Updates**: Users can change their RSVP status
- **RSVP Notifications**: Automatic notifications when RSVP is recorded

### 3. Task Management for Family Projects
- **Task Creation**: Create tasks for family projects and events
- **Task Assignment**: Assign tasks to family members
- **Task Status**: Pending, InProgress, Completed, Cancelled
- **Task Priority**: Low, Medium, High, Urgent
- **Due Dates**: Set and track task due dates
- **Completion Tracking**: Track when tasks are completed
- **Event Association**: Link tasks to specific events
- **Household Tasks**: Associate tasks with households
- **Task Queries**: Search by status, assigned user, household, or related event
- **Assignment Notifications**: Automatic notifications when tasks are assigned

### 4. Collaborative Editing & Comment System
- **Universal Comments**: Comment on any entity (Person, Media, BiographicalNote, etc.)
- **Comment Threading**: Reply to comments creating conversation threads
- **Comment Editing**: Users can edit their own comments (marked as edited)
- **Comment Deletion**: Users can delete their own comments
- **Entity Association**: Comments linked to specific entities by type and ID
- **User Attribution**: Track comment author with username display
- **Comment Replies**: Nested comment structure for discussions
- **Timestamp Tracking**: Track creation and edit timestamps

## Database Schema

### FamilyEvent
```csharp
- Id: int (PK)
- Title: string (required, max 200) - Event title
- Description: string? (max 2000) - Event description
- StartDateTime: DateTime (required) - Event start time
- EndDateTime: DateTime? - Event end time (optional)
- Location: string? (max 500) - Event location
- IsAllDay: bool - Whether event is all-day
- EventType: string (required, max 50) - Type of event
- IsRecurring: bool - Whether event recurs
- RecurrencePattern: string? (max 100) - Recurrence pattern
- CreatedByUserId: string (required, FK to ApplicationUser)
- HouseholdId: int? (FK to Household)
- IsCancelled: bool - Whether event is cancelled
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
```

### EventRsvp
```csharp
- Id: int (PK)
- FamilyEventId: int (required, FK to FamilyEvent)
- UserId: string (required, FK to ApplicationUser)
- Status: string (required, max 50) - Attending, NotAttending, Maybe, Pending
- GuestCount: int? - Number of additional guests
- Notes: string? (max 1000) - RSVP notes
- ResponseDateTime: DateTime? - When user responded
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
- Unique Index: (FamilyEventId, UserId)
```

### FamilyTask
```csharp
- Id: int (PK)
- Title: string (required, max 200) - Task title
- Description: string? (max 2000) - Task description
- Status: string (required, max 50) - Pending, InProgress, Completed, Cancelled
- Priority: string (required, max 50) - Low, Medium, High, Urgent
- DueDate: DateTime? - Task due date
- CompletedDate: DateTime? - When task was completed
- CreatedByUserId: string (required, FK to ApplicationUser)
- AssignedToUserId: string? (FK to ApplicationUser)
- HouseholdId: int? (FK to Household)
- RelatedEventId: int? (FK to FamilyEvent)
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
```

### Comment
```csharp
- Id: int (PK)
- Content: string (required, max 2000) - Comment content
- UserId: string (required, FK to ApplicationUser)
- EntityType: string (required, max 100) - Type of entity commented on
- EntityId: int (required) - ID of entity commented on
- ParentCommentId: int? (FK to Comment) - Parent comment for replies
- IsEdited: bool - Whether comment was edited
- EditedAt: DateTime? - When comment was last edited
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
```

## API Endpoints

### FamilyEventController
- `GET /api/familyevent/{id}` - Get event by ID
- `GET /api/familyevent` - Get all events
- `GET /api/familyevent/household/{householdId}` - Get household events
- `GET /api/familyevent/daterange?startDate=X&endDate=Y` - Get events in date range
- `GET /api/familyevent/upcoming?count=10` - Get upcoming events
- `POST /api/familyevent` - Create a new event
- `PUT /api/familyevent/{id}` - Update event
- `DELETE /api/familyevent/{id}` - Delete event

### EventRsvpController
- `GET /api/eventrsvp/{id}` - Get RSVP by ID
- `GET /api/eventrsvp/event/{eventId}` - Get RSVPs for an event
- `GET /api/eventrsvp/user/{userId}` - Get user's RSVPs
- `GET /api/eventrsvp/my-rsvps` - Get current user's RSVPs
- `POST /api/eventrsvp` - Create RSVP
- `PUT /api/eventrsvp/{id}` - Update RSVP
- `DELETE /api/eventrsvp/{id}` - Delete RSVP

### FamilyTaskController
- `GET /api/familytask/{id}` - Get task by ID
- `GET /api/familytask` - Get all tasks
- `GET /api/familytask/household/{householdId}` - Get household tasks
- `GET /api/familytask/assigned-to-me` - Get current user's assigned tasks
- `GET /api/familytask/status/{status}` - Get tasks by status
- `GET /api/familytask/event/{eventId}` - Get tasks for an event
- `POST /api/familytask` - Create a new task
- `PUT /api/familytask/{id}` - Update task
- `DELETE /api/familytask/{id}` - Delete task

### CommentController
- `GET /api/comment/{id}` - Get comment by ID
- `GET /api/comment/entity/{entityType}/{entityId}` - Get comments for entity
- `GET /api/comment/user/{userId}` - Get user's comments
- `GET /api/comment/my-comments` - Get current user's comments
- `POST /api/comment` - Create a new comment
- `PUT /api/comment/{id}` - Update comment
- `DELETE /api/comment/{id}` - Delete comment

## Architecture

### Services
- **FamilyEventService**: Manages family events with authorization and notifications
- **EventRsvpService**: Handles RSVP creation, updates, and prevents duplicates
- **FamilyTaskService**: Manages tasks with assignment and notification logic
- **CommentService**: Handles comments on any entity type with edit tracking

### Repositories
- **FamilyEventRepository**: Data access for events with date range and upcoming queries
- **EventRsvpRepository**: Data access for RSVPs with event/user lookups
- **FamilyTaskRepository**: Data access for tasks with status, assignment, and event queries
- **CommentRepository**: Data access for comments with entity and user queries

### Mappers
- **FamilyEventMapper**: Maps between FamilyEvent entities and view models with RSVP counts
- **EventRsvpMapper**: Maps between EventRsvp entities and view models
- **FamilyTaskMapper**: Maps between FamilyTask entities and view models
- **CommentMapper**: Maps between Comment entities and view models with threading

## Security & Authorization

### Event Security
- Users can only update/delete events they created
- All event endpoints require authentication
- Events can be viewed by any authenticated user (household filtering available)

### RSVP Security
- Users can only create/update/delete their own RSVPs
- RSVP uniqueness enforced per event per user
- All RSVP endpoints require authentication

### Task Security
- Only task creator can delete tasks
- Task creator or assigned user can update tasks
- Assignment changes trigger notifications
- All task endpoints require authentication

### Comment Security
- Users can only edit/delete their own comments
- Comments marked as edited with timestamp
- All comment endpoints require authentication

## Auto-Registration with Autofac

All services, repositories, and mappers follow naming conventions and are automatically registered by Autofac:
- Services ending with "Service" → Registered as IXService
- Repositories ending with "Repository" → Registered as IXRepository
- Mappers ending with "Mapper" → Registered as IXMapper

No manual DI registration required in AutofacModule.cs.

## Database Migrations

Migration: `20251214111117_AddCollaborationTools`

This migration adds four new tables:
- `FamilyEvents` - Stores family calendar events
- `EventRsvps` - Stores event RSVPs with unique constraint
- `FamilyTasks` - Stores family project tasks
- `Comments` - Stores comments on any entity

All tables include appropriate indexes for performance:
- FamilyEvent: Indexed on CreatedByUserId, HouseholdId, StartDateTime, EventType
- EventRsvp: Unique index on (FamilyEventId, UserId), indexed on FamilyEventId, UserId
- FamilyTask: Indexed on CreatedByUserId, AssignedToUserId, HouseholdId, Status, DueDate
- Comment: Indexed on UserId, (EntityType, EntityId), ParentCommentId, CreatedDateTime

## Notification Integration

The collaboration tools integrate with the Phase 4.1 notification system:

### Event Notifications
- Event creator receives notification when event is created
- (Future) Household members notified of new events

### RSVP Notifications
- User receives confirmation when RSVP is recorded
- (Future) Event creator notified of RSVPs

### Task Notifications
- Task creator receives notification when task is created
- Assigned user receives notification when task is assigned to them
- (Future) Notifications when tasks are completed or overdue

### Comment Notifications
- (Future) Entity owner notified of new comments
- (Future) Comment thread participants notified of replies
- (Future) @mentions in comments trigger notifications

## Future Enhancements

### Advanced Calendar Features (Phase 5+)
- Calendar view UI (month, week, day views)
- Event invitations sent to specific users
- Event reminders (email, in-app)
- Import/export calendar events (iCal format)
- Calendar sharing with external calendars
- Event attachments

### Enhanced RSVP Features (Phase 5+)
- RSVP deadlines
- Meal preference tracking
- Transportation needs
- Dietary restrictions
- Plus-one management
- RSVP analytics

### Advanced Task Management (Phase 6+)
- Task dependencies
- Task templates for common events
- Gantt chart visualization
- Task checklists/subtasks
- Time tracking
- Task attachments
- Recurring tasks

### Enhanced Comments (Phase 6+)
- Rich text formatting
- Comment reactions (emoji)
- Comment attachments (images, files)
- @mentions with notifications
- Comment moderation
- Comment search
- Comment voting/likes

## Testing

### Unit Tests
Unit tests created for:
- FamilyEventService CRUD operations and authorization
- FamilyTaskService task management and notifications
- CommentService comment operations and ownership
- All test suites pass successfully

### Integration Tests (Future)
Integration tests should verify:
- Event and RSVP workflow
- Task assignment and completion flow
- Comment threading and nesting
- Notification triggers

## Usage Examples

### Create Family Event
```http
POST /api/familyevent
{
  "title": "Annual Family Reunion",
  "description": "Join us for our 2025 family reunion!",
  "startDateTime": "2025-07-15T10:00:00Z",
  "endDateTime": "2025-07-15T18:00:00Z",
  "location": "Central Park Pavilion",
  "isAllDay": false,
  "eventType": "Reunion",
  "isRecurring": false,
  "householdId": 1
}
```

### RSVP to Event
```http
POST /api/eventrsvp
{
  "familyEventId": 1,
  "status": "Attending",
  "guestCount": 2,
  "notes": "Looking forward to it! Bringing my spouse and kids."
}
```

### Create Task
```http
POST /api/familytask
{
  "title": "Book reunion venue",
  "description": "Reserve the pavilion at Central Park",
  "status": "Pending",
  "priority": "High",
  "dueDate": "2025-06-01T00:00:00Z",
  "assignedToUserId": "user-123",
  "householdId": 1,
  "relatedEventId": 1
}
```

### Add Comment
```http
POST /api/comment
{
  "content": "This is a wonderful photo of Grandma!",
  "entityType": "Media",
  "entityId": 42
}
```

### Reply to Comment
```http
POST /api/comment
{
  "content": "I agree! Such a great memory.",
  "entityType": "Media",
  "entityId": 42,
  "parentCommentId": 15
}
```

## Success Criteria

✅ **Shared Family Calendar**: Family events can be created and viewed by all family members  
✅ **Event Planning**: Rich event details with location, description, and timing  
✅ **RSVP Functionality**: Family members can RSVP to events with status and guest count  
✅ **Task Management**: Tasks can be created, assigned, and tracked for family projects  
✅ **Collaborative Editing**: Comments enable collaboration on family content  
✅ **Comment System**: Comments work on profiles, photos, and other entities  

Phase 4.2 implementation is **COMPLETE**. All features from the roadmap have been implemented.

## Related Documentation
- See ROADMAP.md for overall project phases
- See PHASE_4_1_IMPLEMENTATION.md for messaging and notifications foundation
- See PATTERNS.md for architecture patterns used
- See README.md for setup and usage instructions
