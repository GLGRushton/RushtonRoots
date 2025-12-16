import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Notification, NotificationGroup, NotificationType, NotificationFilter, NOTIFICATION_TYPE_CONFIGS } from '../../models/messaging.model';

/**
 * Phase 8.3: Notification Panel Component
 * 
 * Displays a panel of notifications with grouping, filtering, and actions.
 * Shows notification icons, timestamps, and allows marking as read/unread.
 * Supports grouping related notifications and filtering by type.
 * 
 * Features:
 * - Notification list with icons and timestamps
 * - Group similar notifications
 * - Mark as read/unread
 * - Mark all as read
 * - Clear notifications
 * - Filter by notification type
 * - Navigate to notification source
 * - Responsive design for mobile
 */
@Component({
  selector: 'app-notification-panel',
  standalone: false,
  templateUrl: './notification-panel.component.html',
  styleUrls: ['./notification-panel.component.scss']
})
export class NotificationPanelComponent implements OnInit {
  @Input() notifications: Notification[] = [];
  @Input() groupNotifications: boolean = true;
  @Input() maxVisible: number = 50;
  
  @Output() notificationClicked = new EventEmitter<Notification>();
  @Output() notificationRead = new EventEmitter<number>();
  @Output() notificationUnread = new EventEmitter<number>();
  @Output() notificationDeleted = new EventEmitter<number>();
  @Output() allRead = new EventEmitter<void>();
  @Output() allCleared = new EventEmitter<void>();

  filteredNotifications: Notification[] = [];
  groupedNotifications: NotificationGroup[] = [];
  selectedFilter?: NotificationType;
  showReadNotifications: boolean = true;
  NotificationType = NotificationType; // Expose enum to template
  notificationTypeConfigs = NOTIFICATION_TYPE_CONFIGS;

  ngOnInit(): void {
    this.applyFilters();
  }

  ngOnChanges(): void {
    this.applyFilters();
  }

  /**
   * Apply filters to notifications
   */
  applyFilters(): void {
    // Filter notifications
    this.filteredNotifications = this.notifications.filter(notification => {
      // Filter by read status
      if (!this.showReadNotifications && notification.isRead) {
        return false;
      }

      // Filter by type
      if (this.selectedFilter && notification.type !== this.selectedFilter) {
        return false;
      }

      return true;
    });

    // Sort by timestamp (newest first)
    this.filteredNotifications.sort((a, b) => 
      new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
    );

    // Limit to maxVisible
    this.filteredNotifications = this.filteredNotifications.slice(0, this.maxVisible);

    // Group notifications if enabled
    if (this.groupNotifications) {
      this.groupedNotifications = this.groupNotificationsByKey();
    }
  }

  /**
   * Group notifications by group key
   */
  groupNotificationsByKey(): NotificationGroup[] {
    const groups = new Map<string, Notification[]>();

    this.filteredNotifications.forEach(notification => {
      const key = notification.groupKey || `notification-${notification.id}`;
      if (!groups.has(key)) {
        groups.set(key, []);
      }
      groups.get(key)!.push(notification);
    });

    return Array.from(groups.entries())
      .map(([groupKey, notifications]) => {
        const latestNotification = notifications[0];
        return {
          groupKey,
          type: latestNotification.type,
          count: notifications.length,
          notifications,
          latestNotification,
          allRead: notifications.every(n => n.isRead)
        };
      })
      .sort((a, b) => 
        new Date(b.latestNotification.timestamp).getTime() - 
        new Date(a.latestNotification.timestamp).getTime()
      );
  }

  /**
   * Handle notification click
   */
  onNotificationClick(notification: Notification): void {
    if (!notification.isRead) {
      this.notificationRead.emit(notification.id);
    }
    this.notificationClicked.emit(notification);
  }

  /**
   * Toggle read status
   */
  toggleReadStatus(notification: Notification, event: Event): void {
    event.stopPropagation();
    if (notification.isRead) {
      this.notificationUnread.emit(notification.id);
    } else {
      this.notificationRead.emit(notification.id);
    }
  }

  /**
   * Delete notification
   */
  deleteNotification(notificationId: number, event: Event): void {
    event.stopPropagation();
    this.notificationDeleted.emit(notificationId);
  }

  /**
   * Mark all as read
   */
  markAllAsRead(): void {
    this.allRead.emit();
  }

  /**
   * Clear all notifications
   */
  clearAllNotifications(): void {
    if (confirm('Are you sure you want to clear all notifications?')) {
      this.allCleared.emit();
    }
  }

  /**
   * Set filter
   */
  setFilter(type?: NotificationType): void {
    this.selectedFilter = type;
    this.applyFilters();
  }

  /**
   * Toggle show read notifications
   */
  toggleShowRead(): void {
    this.showReadNotifications = !this.showReadNotifications;
    this.applyFilters();
  }

  /**
   * Get unread count
   */
  getUnreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  /**
   * Get notification icon
   */
  getNotificationIcon(type: NotificationType): string {
    const config = this.notificationTypeConfigs.find(c => c.type === type);
    return config?.icon || 'notifications';
  }

  /**
   * Get notification color
   */
  getNotificationColor(type: NotificationType): string {
    const config = this.notificationTypeConfigs.find(c => c.type === type);
    return config?.color || '#9E9E9E';
  }

  /**
   * Get notification label
   */
  getNotificationLabel(type: NotificationType): string {
    const config = this.notificationTypeConfigs.find(c => c.type === type);
    return config?.label || type;
  }

  /**
   * Format timestamp
   */
  formatTimestamp(date: Date): string {
    const now = new Date();
    const notificationDate = new Date(date);
    const diffMs = now.getTime() - notificationDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) {
      return 'Just now';
    } else if (diffMins < 60) {
      return `${diffMins}m ago`;
    } else if (diffHours < 24) {
      return `${diffHours}h ago`;
    } else if (diffDays < 7) {
      return `${diffDays}d ago`;
    } else {
      return notificationDate.toLocaleDateString('en-US', { 
        month: 'short', 
        day: 'numeric',
        year: notificationDate.getFullYear() !== now.getFullYear() ? 'numeric' : undefined
      });
    }
  }

  /**
   * Get grouped notification summary
   */
  getGroupSummary(group: NotificationGroup): string {
    if (group.count === 1) {
      return group.latestNotification.title;
    }
    return `${group.latestNotification.title} (+${group.count - 1} more)`;
  }

  /**
   * Get unread count in a group
   */
  getUnreadCountInGroup(group: NotificationGroup): number {
    return group.notifications.filter(n => !n.isRead).length;
  }

  /**
   * Track by function for notifications
   */
  trackByNotificationId(index: number, notification: Notification): number {
    return notification.id;
  }

  /**
   * Track by function for groups
   */
  trackByGroupKey(index: number, group: NotificationGroup): string {
    return group.groupKey;
  }
}
