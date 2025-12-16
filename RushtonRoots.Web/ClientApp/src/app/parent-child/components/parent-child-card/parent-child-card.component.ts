import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ParentChildCard, ParentChildActionEvent, RELATIONSHIP_TYPES } from '../../models/parent-child.model';

/**
 * ParentChildCardComponent - Displays parent-child relationship card
 * Phase 5.2: Parent-Child Relationships
 */
@Component({
  selector: 'app-parent-child-card',
  standalone: false,
  templateUrl: './parent-child-card.component.html',
  styleUrls: ['./parent-child-card.component.scss']
})
export class ParentChildCardComponent {
  /**
   * Parent-child relationship data
   */
  @Input() relationship!: ParentChildCard;

  /**
   * Card elevation (0, 2, 4, 8)
   */
  @Input() elevation: number = 2;

  /**
   * Show action buttons
   */
  @Input() showActions: boolean = true;

  /**
   * Whether the current user can edit relationships
   */
  @Input() canEdit: boolean = true;

  /**
   * Whether the current user can delete relationships
   */
  @Input() canDelete: boolean = true;

  /**
   * Event emitted when an action is clicked
   */
  @Output() action = new EventEmitter<ParentChildActionEvent>();

  /**
   * Get relationship type configuration
   */
  get relationshipTypeConfig() {
    return RELATIONSHIP_TYPES.find(t => t.value === this.relationship.relationshipType) || RELATIONSHIP_TYPES[0];
  }

  /**
   * Get parent initials for fallback avatar
   */
  get parentInitials(): string {
    const names = this.relationship.parentName.split(' ');
    return names.length > 1 
      ? `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
      : this.relationship.parentName.substring(0, 2).toUpperCase();
  }

  /**
   * Get child initials for fallback avatar
   */
  get childInitials(): string {
    const names = this.relationship.childName.split(' ');
    return names.length > 1 
      ? `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
      : this.relationship.childName.substring(0, 2).toUpperCase();
  }

  /**
   * Get age display text
   */
  get ageDisplay(): string {
    if (this.relationship.childAge !== undefined && this.relationship.childAge !== null) {
      return `${this.relationship.childAge} years old`;
    }
    return '';
  }

  /**
   * Handle view action
   */
  onView(): void {
    this.action.emit({
      action: 'view',
      parentChildId: this.relationship.id
    });
  }

  /**
   * Handle edit action
   */
  onEdit(): void {
    this.action.emit({
      action: 'edit',
      parentChildId: this.relationship.id
    });
  }

  /**
   * Handle delete action
   */
  onDelete(): void {
    this.action.emit({
      action: 'delete',
      parentChildId: this.relationship.id
    });
  }

  /**
   * Handle verify action
   */
  onVerify(): void {
    this.action.emit({
      action: 'verify',
      parentChildId: this.relationship.id
    });
  }

  /**
   * Get confidence badge color
   */
  getConfidenceBadgeColor(): string {
    if (!this.relationship.confidence) return '';
    if (this.relationship.confidence >= 80) return 'primary';
    if (this.relationship.confidence >= 60) return 'accent';
    return 'warn';
  }
}
