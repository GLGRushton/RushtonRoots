# Messaging Module - Phase 8.3

This module provides comprehensive messaging and notification functionality for the RushtonRoots family genealogy application.

## Components

### 1. MessageThreadComponent
Displays a list of message threads with unread indicators and last message preview.

**Features:**
- Thread list with avatars and online status
- Unread message count badges
- Last message preview
- Thread actions (archive, mute, delete)
- Search and filter threads
- Typing indicators
- Responsive design for mobile

**Inputs:**
- `threads: MessageThread[]` - Array of message threads
- `currentUserId: number` - Current user ID for filtering
- `selectedThreadId?: number` - Currently selected thread ID
- `showArchived: boolean` - Whether to show archived threads

**Outputs:**
- `threadSelected: EventEmitter<MessageThread>` - Emitted when a thread is selected
- `threadArchived: EventEmitter<number>` - Emitted when a thread is archived
- `threadDeleted: EventEmitter<number>` - Emitted when a thread is deleted
- `threadMuted: EventEmitter<number>` - Emitted when a thread is muted/unmuted
- `newMessageClicked: EventEmitter<void>` - Emitted when new message button is clicked

### 2. ChatInterfaceComponent
Real-time chat interface for messaging within a thread.

**Features:**
- Message list with sender avatars and timestamps
- Message composition input with file attachments
- Send button with keyboard shortcuts (Enter to send)
- Real-time message delivery indicators
- Typing indicators when other users are typing
- Message grouping by date
- Auto-scroll to latest message
- Edit and delete messages
- Message status indicators (sending, sent, delivered, read)

**Inputs:**
- `thread?: MessageThread` - Current message thread
- `currentUserId: number` - Current user ID
- `canSendMessages: boolean` - Whether user can send messages

**Outputs:**
- `messageSent: EventEmitter<MessageCompositionData>` - Emitted when message is sent
- `messageDeleted: EventEmitter<number>` - Emitted when message is deleted
- `messageEdited: EventEmitter<{messageId: number, content: string}>` - Emitted when message is edited
- `typingStarted: EventEmitter<void>` - Emitted when user starts typing
- `typingStopped: EventEmitter<void>` - Emitted when user stops typing
- `attachmentSelected: EventEmitter<File[]>` - Emitted when files are attached

### 3. NotificationPanelComponent
Notification panel with grouping, filtering, and actions.

**Features:**
- Notification list with icons and timestamps
- Group similar notifications
- Mark as read/unread
- Mark all as read
- Clear notifications
- Filter by notification type
- Navigate to notification source
- 16 notification types with icons and colors
- Responsive design for mobile

**Inputs:**
- `notifications: Notification[]` - Array of notifications
- `groupNotifications: boolean` - Whether to group notifications (default: true)
- `maxVisible: number` - Maximum number of notifications to show (default: 50)

**Outputs:**
- `notificationClicked: EventEmitter<Notification>` - Emitted when notification is clicked
- `notificationRead: EventEmitter<number>` - Emitted when notification is marked as read
- `notificationUnread: EventEmitter<number>` - Emitted when notification is marked as unread
- `notificationDeleted: EventEmitter<number>` - Emitted when notification is deleted
- `allRead: EventEmitter<void>` - Emitted when all notifications are marked as read
- `allCleared: EventEmitter<void>` - Emitted when all notifications are cleared

### 4. MessageCompositionDialogComponent
Dialog for composing new messages or starting new conversations.

**Features:**
- Recipient selection with autocomplete
- Optional subject line
- Message composition textarea
- File attachments
- Character count
- Send and cancel actions
- Support for replies

**Dialog Data:**
- `recipients?: Participant[]` - Initial recipients
- `availableRecipients?: Participant[]` - Available recipients for autocomplete
- `subject?: string` - Initial subject
- `replyToMessageId?: number` - ID of message being replied to

**Returns:**
- `MessageCompositionData | undefined` - Message data if sent, undefined if canceled

## Models

### MessageThread
Represents a message thread containing multiple messages.

```typescript
interface MessageThread {
  id: number;
  subject?: string;
  participants: Participant[];
  messages: Message[];
  unreadCount: number;
  lastMessage?: Message;
  createdAt: Date;
  updatedAt: Date;
  isArchived: boolean;
  isMuted: boolean;
  isTyping?: boolean;
  typingUsers?: Participant[];
}
```

### Message
Individual message in a thread.

```typescript
interface Message {
  id: number;
  threadId: number;
  senderId: number;
  senderName: string;
  senderAvatarUrl?: string;
  content: string;
  timestamp: Date;
  isRead: boolean;
  isEdited: boolean;
  editedAt?: Date;
  attachments?: MessageAttachment[];
  replyToMessageId?: number;
}
```

### Notification
Individual notification.

```typescript
interface Notification {
  id: number;
  type: NotificationType;
  priority: NotificationPriority;
  title: string;
  message: string;
  timestamp: Date;
  isRead: boolean;
  actionUrl?: string;
  actionLabel?: string;
  actorId?: number;
  actorName?: string;
  actorAvatarUrl?: string;
  metadata?: Record<string, any>;
  groupKey?: string;
}
```

### NotificationType
Enumeration of notification types:
- MESSAGE - Direct messages
- MENTION - When someone mentions you
- FAMILY_UPDATE - Updates to family tree
- NEW_MEMBER - New family members join
- COMMENT - Comments on content
- LIKE - Content liked
- SHARE - Content shared
- EVENT_REMINDER - Event reminders
- EVENT_RSVP - RSVP updates
- BIRTHDAY - Birthdays
- ANNIVERSARY - Anniversaries
- PHOTO_TAG - Photo tags
- STORY_PUBLISHED - New stories
- RECIPE_COMMENT - Recipe comments
- WIKI_EDIT - Wiki edits
- SYSTEM - System notifications

## Usage Examples

### In TypeScript

```typescript
import { MessageThread, Notification } from './messaging/models/messaging.model';

// Sample message threads
const threads: MessageThread[] = [
  {
    id: 1,
    participants: [user1, user2],
    messages: [...],
    unreadCount: 3,
    lastMessage: {...},
    createdAt: new Date(),
    updatedAt: new Date(),
    isArchived: false,
    isMuted: false
  }
];

// Sample notifications
const notifications: Notification[] = [
  {
    id: 1,
    type: NotificationType.MESSAGE,
    priority: NotificationPriority.MEDIUM,
    title: 'New message from John',
    message: 'John sent you a message',
    timestamp: new Date(),
    isRead: false,
    actorName: 'John Doe',
    groupKey: 'messages-1'
  }
];
```

### In Razor Views

```html
<!-- Message Thread List -->
<app-message-thread 
  [threads]="threads"
  [currentUserId]="currentUserId"
  [selectedThreadId]="selectedThreadId"
  (threadSelected)="onThreadSelected($event)"
  (newMessageClicked)="openNewMessageDialog()">
</app-message-thread>

<!-- Chat Interface -->
<app-chat-interface
  [thread]="selectedThread"
  [currentUserId]="currentUserId"
  (messageSent)="onMessageSent($event)"
  (typingStarted)="handleTypingStarted()">
</app-chat-interface>

<!-- Notification Panel -->
<app-notification-panel
  [notifications]="notifications"
  [groupNotifications]="true"
  (notificationClicked)="handleNotificationClick($event)"
  (allRead)="markAllNotificationsAsRead()">
</app-notification-panel>
```

## Integration with Backend

The components are designed to work with a backend that provides:

1. **Message API**:
   - GET /api/messages/threads - Get message threads
   - GET /api/messages/thread/{id} - Get thread details
   - POST /api/messages - Send new message
   - PUT /api/messages/{id} - Edit message
   - DELETE /api/messages/{id} - Delete message

2. **Notification API**:
   - GET /api/notifications - Get notifications
   - PUT /api/notifications/{id}/read - Mark as read
   - PUT /api/notifications/read-all - Mark all as read
   - DELETE /api/notifications/{id} - Delete notification

3. **Real-time Updates** (SignalR):
   - OnMessageReceived - New message received
   - OnTypingStarted - User started typing
   - OnTypingStopped - User stopped typing
   - OnNotificationReceived - New notification received

## Styling

All components use Material Design with the RushtonRoots color scheme:
- Primary: #2e7d32 (Forest Green)
- Accent: #4caf50 (Light Green)
- Text: #212121 (Dark Gray)

Components are fully responsive and work well on mobile devices.

## Dependencies

- Angular Material Components
- Angular CDK (TextFieldModule for autosize textarea)
- RxJS for reactive programming
- TypeScript for type safety

## Future Enhancements

- Voice messages
- Video calls
- Message reactions (emoji)
- GIF support
- Rich text formatting
- Thread pinning
- Message search
- Notification sounds
- Desktop notifications via Web Push API
