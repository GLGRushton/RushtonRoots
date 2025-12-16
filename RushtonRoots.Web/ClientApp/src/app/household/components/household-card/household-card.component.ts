import { Component, Input, Output, EventEmitter } from '@angular/core';
import { HouseholdCard, HouseholdAction } from '../../models/household.model';

/**
 * HouseholdCardComponent - Displays a single household as a Material Card
 * 
 * Features:
 * - Shows household name and details
 * - Displays member count with badge
 * - Shows anchor person (if available)
 * - Quick action buttons (view, edit, delete, manage members)
 * - Responsive card design
 * - Avatar display for anchor person
 */
@Component({
  selector: 'app-household-card',
  standalone: false,
  templateUrl: './household-card.component.html',
  styleUrls: ['./household-card.component.scss']
})
export class HouseholdCardComponent {
  @Input() household!: HouseholdCard;
  @Input() canEdit = false;
  @Input() canDelete = false;
  @Input() showMembers = true;
  @Input() elevation = 2; // Material elevation level (0-24)
  
  @Output() action = new EventEmitter<HouseholdAction>();

  /**
   * Handle view household details action
   */
  onView(): void {
    this.action.emit({
      type: 'view',
      householdId: this.household.id
    });
  }

  /**
   * Handle edit household action
   */
  onEdit(): void {
    this.action.emit({
      type: 'edit',
      householdId: this.household.id
    });
  }

  /**
   * Handle delete household action
   */
  onDelete(): void {
    this.action.emit({
      type: 'delete',
      householdId: this.household.id
    });
  }

  /**
   * Handle manage members action
   */
  onManageMembers(): void {
    this.action.emit({
      type: 'manage-members',
      householdId: this.household.id
    });
  }

  /**
   * Handle settings action
   */
  onSettings(): void {
    this.action.emit({
      type: 'settings',
      householdId: this.household.id
    });
  }

  /**
   * Get the avatar URL for the anchor person, or default
   */
  getAnchorAvatar(): string {
    return this.household.members?.find(m => m.isAnchor)?.photoUrl || 
           '/assets/images/default-household.png';
  }

  /**
   * Format date for display
   */
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  /**
   * Get member preview (first few members)
   */
  getMemberPreview(): string {
    if (!this.household.members || this.household.members.length === 0) {
      return 'No members';
    }
    
    const maxDisplay = 3;
    const memberNames = this.household.members
      .slice(0, maxDisplay)
      .map(m => m.firstName);
    
    if (this.household.members.length > maxDisplay) {
      return `${memberNames.join(', ')} and ${this.household.members.length - maxDisplay} more`;
    }
    
    return memberNames.join(', ');
  }
}
