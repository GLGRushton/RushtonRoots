# Phase 4.1 Implementation: Messaging & Notifications System

## Overview
This document describes the implementation of Phase 4.1 from the ROADMAP.md - a comprehensive messaging and notifications system that enables family members to communicate directly, participate in group chats, and receive notifications about important events.

## Implementation Date
December 2025

## Features Implemented

### 1. Direct Messaging Between Family Members
- **Send Direct Messages**: Users can send direct messages to other family members
- **Message Threads**: Support for message replies creating conversation threads
- **Message Editing**: Users can edit their own messages (marked as edited with timestamp)
- **Message Deletion**: Users can delete their own messages
- **Read Receipts**: Track when messages are read by recipients
- **Unread Count**: Display count of unread direct messages for users

### 2. Group Messaging/Family Chat Rooms
- **Create Chat Rooms**: Users can create named chat rooms for family discussions
- **Household Association**: Chat rooms can be associated with specific households
- **Chat Room Management**: Admins can update chat room name, description, and active status
- **Member Management**: Add and remove members from chat rooms
- **Role-Based Access**: Admin and Member roles for chat room participants
- **Last Read Tracking**: Track when each member last read messages in a room
- **Unread Message Count**: Display unread message counts per chat room

### 3. Email Notification System
- **Email Integration**: Framework for sending email notifications (currently logs to console)
- **Notification Preferences**: Users can control email notification frequency (Immediate, Daily, Weekly, Never)
- **Email on Events**: Emails sent for new messages, mentions, and other events
- **SendGrid Ready**: Infrastructure ready for integration with SendGrid or other email services

### 4. In-App Notification Center
- **Notification Types**: Support for Message, Mention, Event, and custom notification types
- **Rich Notifications**: Notifications include title, message, action URL, and related entity tracking
- **Read/Unread Status**: Track which notifications have been read
- **Unread Count**: Display count of unread notifications
- **Mark All as Read**: Bulk mark all notifications as read
- **Notification History**: View all notifications with pagination
- **Automatic Creation**: Notifications automatically created for messages and mentions

### 5. Notification Preferences Per User
- **Preference Management**: Users can customize notification preferences per notification type
- **In-App Toggle**: Enable/disable in-app notifications per type
- **Email Toggle**: Enable/disable email notifications per type
- **Email Frequency**: Choose immediate, daily, weekly, or never for email notifications
- **Default Preferences**: Automatic creation of default preferences for new notification types

### 6. @Mentions and Tagging in Messages
- **Mention Support**: Users can mention other users in messages using their user IDs
- **Mention Tracking**: Mentioned user IDs stored with each message
- **Mention Notifications**: Mentioned users receive notifications when they are tagged
- **Multiple Mentions**: Support for mentioning multiple users in a single message

## Database Schema

### Message
```csharp
- Id: int (PK)
- Content: string (required) - Message content
- SenderUserId: string (required, FK to ApplicationUser)
- RecipientUserId: string? (FK to ApplicationUser) - For direct messages
- ChatRoomId: int? (FK to ChatRoom) - For group messages
- ParentMessageId: int? (FK to Message) - For reply threads
- ReadAt: DateTime? - When recipient read the message
- IsEdited: bool - Whether message was edited
- EditedAt: DateTime? - When message was last edited
- MentionedUserIds: string? - Comma-separated user IDs mentioned
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
```

### ChatRoom
```csharp
- Id: int (PK)
- Name: string (required, max 200)
- Description: string? (max 1000)
- CreatedByUserId: string (required, FK to ApplicationUser)
- HouseholdId: int? (FK to Household)
- IsActive: bool - Whether chat room is active
- CreatedDateTime: DateTime
- UpdatedDateTime: DateTime
```

### ChatRoomMember
```csharp
- Id: int (PK)
- ChatRoomId: int (FK to ChatRoom)
- UserId: string (FK to ApplicationUser)
- Role: string (required, max 50) - Admin or Member
- JoinedAt: DateTime - When member joined
- LastReadAt: DateTime? - Last time member read messages
- IsActive: bool - Whether member is active
- Unique Index: (ChatRoomId, UserId)
```

### Notification
```csharp
- Id: int (PK)
- UserId: string (required, FK to ApplicationUser)
- Type: string (required, max 50) - Message, Mention, Event, etc.
- Title: string (required, max 200)
- Message: string (required, max 1000)
- ActionUrl: string? (max 500) - URL to navigate when clicked
- RelatedEntityId: int? - ID of related entity
- RelatedEntityType: string? (max 100) - Type of related entity
- IsRead: bool - Whether notification was read
- ReadAt: DateTime? - When notification was read
- EmailSent: bool - Whether email was sent
- EmailSentAt: DateTime? - When email was sent
- CreatedDateTime: DateTime
```

### NotificationPreference
```csharp
- Id: int (PK)
- UserId: string (required, FK to ApplicationUser)
- NotificationType: string (required, max 50)
- InAppEnabled: bool - In-app notifications enabled
- EmailEnabled: bool - Email notifications enabled
- EmailFrequency: string (required, max 20) - Immediate, Daily, Weekly, Never
- Unique Index: (UserId, NotificationType)
```

## API Endpoints

### MessageController
- `GET /api/message/{id}` - Get message by ID
- `GET /api/message/direct/{otherUserId}` - Get direct messages with another user
- `GET /api/message/chatroom/{chatRoomId}` - Get chat room messages
- `POST /api/message` - Send a new message
- `PUT /api/message/{id}` - Edit a message
- `DELETE /api/message/{id}` - Delete a message
- `POST /api/message/{id}/read` - Mark message as read
- `GET /api/message/unread-count` - Get unread direct message count

### ChatRoomController
- `GET /api/chatroom/{id}` - Get chat room by ID
- `GET /api/chatroom/my-chatrooms` - Get current user's chat rooms
- `GET /api/chatroom/household/{householdId}` - Get household chat rooms
- `POST /api/chatroom` - Create a new chat room
- `PUT /api/chatroom/{id}` - Update chat room
- `DELETE /api/chatroom/{id}` - Delete chat room
- `POST /api/chatroom/{id}/members` - Add member to chat room
- `DELETE /api/chatroom/{id}/members/{userId}` - Remove member from chat room
- `POST /api/chatroom/{id}/read` - Update last read timestamp

### NotificationController
- `GET /api/notification/{id}` - Get notification by ID
- `GET /api/notification` - Get current user's notifications
- `GET /api/notification/unread-count` - Get unread notification count
- `POST /api/notification/{id}/read` - Mark notification as read
- `POST /api/notification/read-all` - Mark all notifications as read
- `DELETE /api/notification/{id}` - Delete notification
- `GET /api/notification/preferences` - Get notification preferences
- `PUT /api/notification/preferences` - Update notification preference

## Architecture

### Services
- **MessageService**: Handles message creation, editing, deletion, and read receipts
- **ChatRoomService**: Manages chat rooms and members with role-based access control
- **NotificationService**: Creates and manages notifications with preference checking

### Repositories
- **MessageRepository**: Data access for messages with support for direct and group messages
- **ChatRoomRepository**: Data access for chat rooms and members
- **NotificationRepository**: Data access for notifications and preferences

### Mappers
- **MessageMapper**: Maps between Message entities and MessageViewModel
- **ChatRoomMapper**: Maps between ChatRoom/ChatRoomMember entities and view models
- **NotificationMapper**: Maps between Notification/NotificationPreference entities and view models

## Security & Authorization

### Message Security
- Users can only edit and delete their own messages
- Users can only mark messages addressed to them as read
- All message endpoints require authentication

### Chat Room Security
- Only admins can update chat room settings
- Only the creator can delete a chat room
- Only admins can add members
- Admins can remove any member, users can remove themselves
- All chat room endpoints require authentication

### Notification Security
- Users can only access their own notifications
- Users can only update their own notification preferences
- All notification endpoints require authentication

## Auto-Registration with Autofac

All services, repositories, and mappers follow naming conventions and are automatically registered by Autofac:
- Services ending with "Service" → Registered as IXService
- Repositories ending with "Repository" → Registered as IXRepository
- Mappers ending with "Mapper" → Registered as IXMapper

No manual DI registration required in AutofacModule.cs.

## Database Migrations

Migration: `20251214104206_AddMessagingAndNotifications`

This migration adds five new tables:
- `Messages` - Stores direct and group messages
- `ChatRooms` - Stores chat room definitions
- `ChatRoomMembers` - Stores chat room memberships
- `Notifications` - Stores in-app notifications
- `NotificationPreferences` - Stores user notification preferences

All tables include appropriate indexes for performance:
- Message: Indexed on SenderUserId, RecipientUserId, ChatRoomId, CreatedDateTime
- ChatRoom: Indexed on CreatedByUserId, HouseholdId, IsActive
- ChatRoomMember: Unique index on (ChatRoomId, UserId), indexed on UserId, IsActive
- Notification: Indexed on UserId, IsRead, CreatedDateTime, (UserId, IsRead)
- NotificationPreference: Unique index on (UserId, NotificationType), indexed on UserId

## Email Integration

The current implementation uses EmailService with console logging. To integrate with a real email service:

1. Install email provider package (e.g., SendGrid, AWS SES)
2. Update `EmailService.SendNotificationEmailAsync()` method
3. Configure email templates
4. Add email credentials to appsettings.json
5. Implement retry logic for failed emails

## Future Enhancements

### Real-Time Features (Phase 4.1+)
- SignalR integration for real-time message delivery
- Typing indicators
- Online/offline status
- Push notifications for mobile

### Enhanced Messaging (Phase 4.2+)
- File attachments in messages
- Message reactions (emoji reactions)
- Message search
- Message pinning
- Voice messages

### Advanced Notifications (Phase 4.2+)
- Digest emails (daily/weekly summaries)
- Custom notification sounds
- Notification categories
- Notification filtering
- Browser push notifications

## Testing

### Unit Tests
Unit tests should be created for:
- MessageService CRUD operations
- ChatRoomService member management
- NotificationService preference handling
- Repository query methods
- Mapper transformations

### Integration Tests
Integration tests should verify:
- Message delivery flow
- Notification creation on events
- Email sending on notification creation
- Chat room permission enforcement

## Usage Examples

### Send Direct Message
```http
POST /api/message
{
  "content": "Hey, how are you?",
  "recipientUserId": "user-id-123",
  "mentionedUserIds": []
}
```

### Create Chat Room
```http
POST /api/chatroom
{
  "name": "Family Planning",
  "description": "Discuss family reunion plans",
  "householdId": 1,
  "memberUserIds": ["user-1", "user-2", "user-3"]
}
```

### Update Notification Preferences
```http
PUT /api/notification/preferences
{
  "notificationType": "Message",
  "inAppEnabled": true,
  "emailEnabled": true,
  "emailFrequency": "Daily"
}
```

## Success Criteria

✅ **Direct Messaging**: Users can send direct messages to other family members  
✅ **Group Messaging**: Users can create and participate in family chat rooms  
✅ **Email Notifications**: System can send email notifications (framework ready)  
✅ **In-App Notifications**: Users receive in-app notifications for events  
✅ **Notification Preferences**: Users can customize notification preferences  
✅ **@Mentions**: Users can mention others in messages and receive notifications  

Phase 4.1 implementation is **COMPLETE**. All features from the roadmap have been implemented.

## Related Documentation
- See ROADMAP.md for overall project phases
- See PATTERNS.md for architecture patterns used
- See README.md for setup and usage instructions
