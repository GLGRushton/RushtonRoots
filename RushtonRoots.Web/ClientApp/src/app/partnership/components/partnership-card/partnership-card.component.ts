import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PartnershipCard, PartnershipActionEvent, PARTNERSHIP_STATUSES } from '../../models/partnership.model';

/**
 * PartnershipCardComponent - Displays partnership information in a card format
 * Phase 5.1: Partnership Management
 */
@Component({
  selector: 'app-partnership-card',
  standalone: false,
  templateUrl: './partnership-card.component.html',
  styleUrls: ['./partnership-card.component.scss']
})
export class PartnershipCardComponent {
  /**
   * Partnership data to display
   */
  @Input() partnership!: PartnershipCard;

  /**
   * Card elevation (0, 2, 4, 8)
   */
  @Input() elevation: number = 2;

  /**
   * Whether to show action buttons
   */
  @Input() showActions: boolean = true;

  /**
   * Whether the current user can edit
   */
  @Input() canEdit: boolean = true;

  /**
   * Whether the current user can delete
   */
  @Input() canDelete: boolean = true;

  /**
   * Event emitted when an action is clicked
   */
  @Output() action = new EventEmitter<PartnershipActionEvent>();

  /**
   * Get status configuration
   */
  getStatusConfig() {
    return PARTNERSHIP_STATUSES.find(s => s.value === this.partnership.status) || PARTNERSHIP_STATUSES[PARTNERSHIP_STATUSES.length - 1];
  }

  /**
   * Handle view action
   */
  onView(): void {
    this.action.emit({
      action: 'view',
      partnership: this.partnership
    });
  }

  /**
   * Handle edit action
   */
  onEdit(): void {
    this.action.emit({
      action: 'edit',
      partnership: this.partnership
    });
  }

  /**
   * Handle delete action
   */
  onDelete(): void {
    this.action.emit({
      action: 'delete',
      partnership: this.partnership
    });
  }

  /**
   * Handle timeline action
   */
  onTimeline(): void {
    this.action.emit({
      action: 'timeline',
      partnership: this.partnership
    });
  }

  /**
   * Format date for display
   */
  formatDate(date?: Date): string {
    if (!date) return 'Unknown';
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  /**
   * Get avatar fallback initials
   */
  getInitials(name: string): string {
    const parts = name.split(' ');
    if (parts.length >= 2) {
      return parts[0][0] + parts[parts.length - 1][0];
    }
    return name.substring(0, 2).toUpperCase();
  }
}
