import { Component, Input } from '@angular/core';
import { HouseholdActivityEvent } from '../../models/household-details.model';

/**
 * HouseholdActivityTimelineComponent - Display household activity feed
 * 
 * Features:
 * - Chronological timeline of events
 * - Event type icons and colors
 * - User attribution
 * - Expandable event details
 * - Infinite scroll (future enhancement)
 */
@Component({
  selector: 'app-household-activity-timeline',
  standalone: false,
  templateUrl: './household-activity-timeline.component.html',
  styleUrls: ['./household-activity-timeline.component.scss']
})
export class HouseholdActivityTimelineComponent {
  @Input() events: HouseholdActivityEvent[] = [];
  @Input() householdId!: number;

  expandedEventIds: Set<number> = new Set();

  getEventIcon(eventType: string): string {
    const iconMap: { [key: string]: string } = {
      'member_joined': 'person_add',
      'member_left': 'person_remove',
      'member_invited': 'mail',
      'household_created': 'home',
      'household_updated': 'edit',
      'photo_uploaded': 'photo',
      'member_role_changed': 'admin_panel_settings',
      'settings_changed': 'settings'
    };
    return iconMap[eventType] || 'event';
  }

  getEventColor(eventType: string): string {
    const colorMap: { [key: string]: string } = {
      'member_joined': '#4caf50',
      'member_left': '#f44336',
      'member_invited': '#2196f3',
      'household_created': '#2e7d32',
      'household_updated': '#ff9800',
      'photo_uploaded': '#9c27b0',
      'member_role_changed': '#607d8b',
      'settings_changed': '#795548'
    };
    return colorMap[eventType] || '#757575';
  }

  toggleEventDetails(eventId: number): void {
    if (this.expandedEventIds.has(eventId)) {
      this.expandedEventIds.delete(eventId);
    } else {
      this.expandedEventIds.add(eventId);
    }
  }

  isEventExpanded(eventId: number): boolean {
    return this.expandedEventIds.has(eventId);
  }

  formatTimestamp(timestamp: Date | string): string {
    const date = typeof timestamp === 'string' ? new Date(timestamp) : timestamp;
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getSortedEvents(): HouseholdActivityEvent[] {
    return [...this.events].sort((a, b) => {
      const dateA = typeof a.timestamp === 'string' ? new Date(a.timestamp) : a.timestamp;
      const dateB = typeof b.timestamp === 'string' ? new Date(b.timestamp) : b.timestamp;
      return dateB.getTime() - dateA.getTime(); // Most recent first
    });
  }
}
