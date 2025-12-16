/**
 * Phase 8.3: Messaging & Notifications Models
 * 
 * This file contains all TypeScript interfaces and types for messaging and notifications.
 */

/**
 * User participant in a message thread
 */
export interface Participant {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  avatarUrl?: string;
  isOnline: boolean;
  lastSeen?: Date;
}

/**
 * Individual message in a thread
 */
export interface Message {
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

/**
 * Message attachment
 */
export interface MessageAttachment {
  id: number;
  fileName: string;
  fileSize: number;
  fileType: string;
  url: string;
  thumbnailUrl?: string;
}

/**
 * Message thread containing multiple messages
 */
export interface MessageThread {
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

/**
 * Message composition data
 */
export interface MessageCompositionData {
  recipientIds: number[];
  subject?: string;
  content: string;
  attachments?: File[];
  replyToMessageId?: number;
}

/**
 * Typing indicator data
 */
export interface TypingIndicator {
  threadId: number;
  userId: number;
  userName: string;
  isTyping: boolean;
}

/**
 * Notification types
 */
export enum NotificationType {
  MESSAGE = 'message',
  MENTION = 'mention',
  FAMILY_UPDATE = 'family_update',
  NEW_MEMBER = 'new_member',
  COMMENT = 'comment',
  LIKE = 'like',
  SHARE = 'share',
  EVENT_REMINDER = 'event_reminder',
  EVENT_RSVP = 'event_rsvp',
  BIRTHDAY = 'birthday',
  ANNIVERSARY = 'anniversary',
  PHOTO_TAG = 'photo_tag',
  STORY_PUBLISHED = 'story_published',
  RECIPE_COMMENT = 'recipe_comment',
  WIKI_EDIT = 'wiki_edit',
  SYSTEM = 'system'
}

/**
 * Notification priority
 */
export enum NotificationPriority {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  URGENT = 'urgent'
}

/**
 * Individual notification
 */
export interface Notification {
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
  groupKey?: string; // For grouping related notifications
}

/**
 * Grouped notifications
 */
export interface NotificationGroup {
  groupKey: string;
  type: NotificationType;
  count: number;
  notifications: Notification[];
  latestNotification: Notification;
  allRead: boolean;
}

/**
 * Notification filter options
 */
export interface NotificationFilter {
  types?: NotificationType[];
  showRead?: boolean;
  showUnread?: boolean;
  priority?: NotificationPriority;
  dateFrom?: Date;
  dateTo?: Date;
}

/**
 * Notification preferences (extends existing from auth module)
 */
export interface NotificationSettings {
  emailNotifications: boolean;
  pushNotifications: boolean;
  inAppNotifications: boolean;
  notificationSound: boolean;
  groupSimilar: boolean;
  muteUntil?: Date;
  mutedThreads: number[];
  preferences: {
    [key in NotificationType]?: {
      email: boolean;
      push: boolean;
      inApp: boolean;
    };
  };
}

/**
 * Notification type configuration
 */
export interface NotificationTypeConfig {
  type: NotificationType;
  icon: string;
  color: string;
  label: string;
  description: string;
}

/**
 * Notification type configurations
 */
export const NOTIFICATION_TYPE_CONFIGS: NotificationTypeConfig[] = [
  {
    type: NotificationType.MESSAGE,
    icon: 'message',
    color: '#2196F3',
    label: 'Messages',
    description: 'Direct messages from family members'
  },
  {
    type: NotificationType.MENTION,
    icon: 'alternate_email',
    color: '#9C27B0',
    label: 'Mentions',
    description: 'When someone mentions you'
  },
  {
    type: NotificationType.FAMILY_UPDATE,
    icon: 'family_restroom',
    color: '#4CAF50',
    label: 'Family Updates',
    description: 'Updates to family tree and relationships'
  },
  {
    type: NotificationType.NEW_MEMBER,
    icon: 'person_add',
    color: '#00BCD4',
    label: 'New Members',
    description: 'New family members join'
  },
  {
    type: NotificationType.COMMENT,
    icon: 'comment',
    color: '#FF9800',
    label: 'Comments',
    description: 'Comments on your content'
  },
  {
    type: NotificationType.LIKE,
    icon: 'favorite',
    color: '#E91E63',
    label: 'Likes',
    description: 'Someone liked your content'
  },
  {
    type: NotificationType.SHARE,
    icon: 'share',
    color: '#607D8B',
    label: 'Shares',
    description: 'Your content was shared'
  },
  {
    type: NotificationType.EVENT_REMINDER,
    icon: 'event',
    color: '#3F51B5',
    label: 'Event Reminders',
    description: 'Upcoming family events'
  },
  {
    type: NotificationType.EVENT_RSVP,
    icon: 'event_available',
    color: '#009688',
    label: 'Event RSVPs',
    description: 'RSVP updates for your events'
  },
  {
    type: NotificationType.BIRTHDAY,
    icon: 'cake',
    color: '#FF5722',
    label: 'Birthdays',
    description: 'Family member birthdays'
  },
  {
    type: NotificationType.ANNIVERSARY,
    icon: 'celebration',
    color: '#795548',
    label: 'Anniversaries',
    description: 'Family anniversaries'
  },
  {
    type: NotificationType.PHOTO_TAG,
    icon: 'photo_camera',
    color: '#FFC107',
    label: 'Photo Tags',
    description: 'Tagged in photos'
  },
  {
    type: NotificationType.STORY_PUBLISHED,
    icon: 'auto_stories',
    color: '#8BC34A',
    label: 'Story Published',
    description: 'New family stories published'
  },
  {
    type: NotificationType.RECIPE_COMMENT,
    icon: 'restaurant',
    color: '#CDDC39',
    label: 'Recipe Comments',
    description: 'Comments on recipes'
  },
  {
    type: NotificationType.WIKI_EDIT,
    icon: 'edit_note',
    color: '#673AB7',
    label: 'Wiki Edits',
    description: 'Wiki article updates'
  },
  {
    type: NotificationType.SYSTEM,
    icon: 'info',
    color: '#9E9E9E',
    label: 'System',
    description: 'System notifications'
  }
];

/**
 * Message search filters
 */
export interface MessageSearchFilter {
  query?: string;
  threadId?: number;
  senderId?: number;
  dateFrom?: Date;
  dateTo?: Date;
  hasAttachments?: boolean;
  isUnread?: boolean;
}

/**
 * Message thread summary for list view
 */
export interface MessageThreadSummary {
  thread: MessageThread;
  lastMessagePreview: string;
  unreadCount: number;
  participantNames: string;
  isOnline: boolean;
}

/**
 * Real-time message status
 */
export enum MessageStatus {
  SENDING = 'sending',
  SENT = 'sent',
  DELIVERED = 'delivered',
  READ = 'read',
  FAILED = 'failed'
}

/**
 * Message with status
 */
export interface MessageWithStatus extends Message {
  status: MessageStatus;
  tempId?: string; // Temporary ID for optimistic updates
}
