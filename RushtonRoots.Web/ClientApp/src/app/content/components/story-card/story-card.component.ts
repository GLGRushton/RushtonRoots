import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Story } from '../../models/content.model';

/**
 * StoryCardComponent
 * Displays a family story in a card format with image, title, metadata, and actions
 */
@Component({
  selector: 'app-story-card',
  standalone: false,
  templateUrl: './story-card.component.html',
  styleUrls: ['./story-card.component.scss']
})
export class StoryCardComponent {
  /**
   * Story to display
   */
  @Input() story!: Story;

  /**
   * Card elevation level (0, 2, 4, 8)
   */
  @Input() elevation: number = 2;

  /**
   * Whether to truncate the summary
   */
  @Input() truncateSummary: boolean = true;

  /**
   * Whether to show action buttons
   */
  @Input() showActions: boolean = true;

  /**
   * Whether the user can edit this story
   */
  @Input() canEdit: boolean = false;

  /**
   * Event emitted when the view button is clicked
   */
  @Output() viewStory = new EventEmitter<number>();

  /**
   * Event emitted when the edit button is clicked
   */
  @Output() editStory = new EventEmitter<number>();

  /**
   * Event emitted when the delete button is clicked
   */
  @Output() deleteStory = new EventEmitter<number>();

  /**
   * Event emitted when a related person is clicked
   */
  @Output() viewPerson = new EventEmitter<number>();

  /**
   * Handle view action
   */
  onView(): void {
    this.viewStory.emit(this.story.id);
  }

  /**
   * Handle edit action
   */
  onEdit(): void {
    this.editStory.emit(this.story.id);
  }

  /**
   * Handle delete action
   */
  onDelete(): void {
    this.deleteStory.emit(this.story.id);
  }

  /**
   * Handle person click
   */
  onPersonClick(personId: number): void {
    this.viewPerson.emit(personId);
  }

  /**
   * Get truncated summary
   */
  getSummary(): string {
    if (!this.truncateSummary) {
      return this.story.summary;
    }
    return this.story.summary.length > 200
      ? this.story.summary.substring(0, 200) + '...'
      : this.story.summary;
  }

  /**
   * Get formatted date of event
   */
  getEventDate(): string {
    if (!this.story.dateOfEvent) {
      return '';
    }
    const date = new Date(this.story.dateOfEvent);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }

  /**
   * Get media count
   */
  getMediaCount(): number {
    return this.story.media.length;
  }

  /**
   * Get photo count
   */
  getPhotoCount(): number {
    return this.story.media.filter(m => m.type === 'photo').length;
  }

  /**
   * Get video count
   */
  getVideoCount(): number {
    return this.story.media.filter(m => m.type === 'video').length;
  }
}
