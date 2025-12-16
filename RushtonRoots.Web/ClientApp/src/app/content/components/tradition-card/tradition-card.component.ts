import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Tradition, TRADITION_FREQUENCY_CONFIG, MediaType } from '../../models/content.model';

/**
 * TraditionCardComponent
 * Displays a family tradition in a card format with image, title, metadata, and actions
 */
@Component({
  selector: 'app-tradition-card',
  standalone: false,
  templateUrl: './tradition-card.component.html',
  styleUrls: ['./tradition-card.component.scss']
})
export class TraditionCardComponent {
  /**
   * Tradition to display
   */
  @Input() tradition!: Tradition;

  /**
   * Card elevation level (0, 2, 4, 8)
   */
  @Input() elevation: number = 2;

  /**
   * Whether to truncate the description
   */
  @Input() truncateDescription: boolean = true;

  /**
   * Whether to show action buttons
   */
  @Input() showActions: boolean = true;

  /**
   * Whether the user can edit this tradition
   */
  @Input() canEdit: boolean = false;

  /**
   * Event emitted when the view button is clicked
   */
  @Output() viewTradition = new EventEmitter<number>();

  /**
   * Event emitted when the edit button is clicked
   */
  @Output() editTradition = new EventEmitter<number>();

  /**
   * Event emitted when the delete button is clicked
   */
  @Output() deleteTradition = new EventEmitter<number>();

  /**
   * Event emitted when a related person is clicked
   */
  @Output() viewPerson = new EventEmitter<number>();

  /**
   * Event emitted when a related recipe is clicked
   */
  @Output() viewRecipe = new EventEmitter<number>();

  /**
   * Frequency configuration
   */
  frequencyConfig = TRADITION_FREQUENCY_CONFIG;

  /**
   * Handle view action
   */
  onView(): void {
    this.viewTradition.emit(this.tradition.id);
  }

  /**
   * Handle edit action
   */
  onEdit(): void {
    this.editTradition.emit(this.tradition.id);
  }

  /**
   * Handle delete action
   */
  onDelete(): void {
    this.deleteTradition.emit(this.tradition.id);
  }

  /**
   * Handle person click
   */
  onPersonClick(personId: number): void {
    this.viewPerson.emit(personId);
  }

  /**
   * Handle recipe click
   */
  onRecipeClick(recipeId: number): void {
    this.viewRecipe.emit(recipeId);
  }

  /**
   * Get truncated description
   */
  getDescription(): string {
    if (!this.truncateDescription) {
      return this.tradition.description;
    }
    return this.tradition.description.length > 200
      ? this.tradition.description.substring(0, 200) + '...'
      : this.tradition.description;
  }

  /**
   * Get frequency label
   */
  getFrequencyLabel(): string {
    return this.frequencyConfig[this.tradition.frequency]?.label || this.tradition.frequency;
  }

  /**
   * Get frequency icon
   */
  getFrequencyIcon(): string {
    return this.frequencyConfig[this.tradition.frequency]?.icon || 'event';
  }

  /**
   * Get months display
   */
  getMonthsDisplay(): string {
    if (!this.tradition.monthsActive || this.tradition.monthsActive.length === 0) {
      return '';
    }
    
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const months = this.tradition.monthsActive.map(m => monthNames[m - 1]);
    
    if (months.length > 3) {
      return `${months.slice(0, 3).join(', ')}, +${months.length - 3} more`;
    }
    
    return months.join(', ');
  }

  /**
   * Get media count
   */
  getMediaCount(): number {
    return this.tradition.media.length;
  }

  /**
   * Get photo count
   */
  getPhotoCount(): number {
    return this.tradition.media.filter(m => m.type === MediaType.Photo).length;
  }
}
