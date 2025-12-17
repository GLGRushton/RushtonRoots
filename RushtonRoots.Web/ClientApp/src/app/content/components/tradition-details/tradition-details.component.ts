import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Tradition, StoryPerson, StoryMedia, MediaType, TRADITION_FREQUENCY_CONFIG } from '../../models/content.model';

/**
 * Tradition participant with role information
 */
export interface TraditionParticipant extends StoryPerson {
  participantRole?: 'host' | 'organizer' | 'participant';
}

/**
 * Related tradition recipe
 */
export interface RelatedTraditionRecipe {
  id: number;
  title: string;
  description: string;
  imageUrl?: string;
  prepTime?: number;
  difficulty?: string;
}

/**
 * Related tradition story
 */
export interface RelatedTraditionStory {
  id: number;
  title: string;
  summary: string;
  imageUrl?: string;
  dateOfEvent?: Date;
}

/**
 * Tradition occurrence (calendar event)
 */
export interface TraditionOccurrence {
  id: number;
  traditionId: number;
  occurrenceDate: Date;
  location?: string;
  attendees?: number[];
  attendeeNames?: string[];
  notes?: string;
}

/**
 * TraditionDetailsComponent
 * Displays full tradition details with comprehensive information, participants, related content, and calendar
 */
@Component({
  selector: 'app-tradition-details',
  standalone: false,
  templateUrl: './tradition-details.component.html',
  styleUrls: ['./tradition-details.component.scss']
})
export class TraditionDetailsComponent implements OnInit {
  /**
   * Tradition to display
   */
  @Input() tradition!: Tradition;

  /**
   * Participants in this tradition with roles
   */
  @Input() participants: TraditionParticipant[] = [];

  /**
   * Related recipes (holiday dishes, etc.)
   */
  @Input() relatedRecipes: RelatedTraditionRecipe[] = [];

  /**
   * Related stories (memories of celebrations)
   */
  @Input() relatedStories: RelatedTraditionStory[] = [];

  /**
   * Past occurrences with attendance
   */
  @Input() pastOccurrences: TraditionOccurrence[] = [];

  /**
   * Next occurrence date
   */
  @Input() nextOccurrence?: TraditionOccurrence;

  /**
   * Whether the user can edit this tradition
   */
  @Input() canEdit: boolean = false;

  /**
   * Whether the user has favorited this tradition
   */
  @Input() hasFavorited: boolean = false;

  /**
   * Event emitted when user wants to edit the tradition
   */
  @Output() editTradition = new EventEmitter<number>();

  /**
   * Event emitted when user wants to delete the tradition
   */
  @Output() deleteTradition = new EventEmitter<number>();

  /**
   * Event emitted when user clicks share button
   */
  @Output() shareTradition = new EventEmitter<number>();

  /**
   * Event emitted when user clicks print button
   */
  @Output() printTradition = new EventEmitter<number>();

  /**
   * Event emitted when user favorites the tradition
   */
  @Output() favoriteTradition = new EventEmitter<number>();

  /**
   * Event emitted when user clicks a participant
   */
  @Output() viewPerson = new EventEmitter<number>();

  /**
   * Event emitted when user clicks a related recipe
   */
  @Output() viewRecipe = new EventEmitter<number>();

  /**
   * Event emitted when user clicks a related story
   */
  @Output() viewStory = new EventEmitter<number>();

  /**
   * Event emitted when user wants to create calendar event
   */
  @Output() createCalendarEvent = new EventEmitter<number>();

  /**
   * Current selected tab
   */
  selectedTab: number = 0;

  /**
   * Whether print view is active
   */
  isPrintView: boolean = false;

  /**
   * Selected media for lightbox
   */
  selectedMediaIndex: number = -1;

  /**
   * Media types enum for template
   */
  MediaType = MediaType;

  /**
   * Frequency configuration
   */
  frequencyConfig = TRADITION_FREQUENCY_CONFIG;

  ngOnInit(): void {
    // Initialize component
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
    if (confirm('Are you sure you want to delete this tradition?')) {
      this.deleteTradition.emit(this.tradition.id);
    }
  }

  /**
   * Handle share action
   */
  onShare(): void {
    this.shareTradition.emit(this.tradition.id);
    // Copy URL to clipboard
    const url = window.location.href;
    navigator.clipboard.writeText(url).then(() => {
      alert('Tradition link copied to clipboard!');
    });
  }

  /**
   * Handle print action
   */
  onPrint(): void {
    this.isPrintView = true;
    this.printTradition.emit(this.tradition.id);
    setTimeout(() => {
      window.print();
      this.isPrintView = false;
    }, 100);
  }

  /**
   * Handle favorite action
   */
  onFavorite(): void {
    this.favoriteTradition.emit(this.tradition.id);
  }

  /**
   * Handle participant click
   */
  onParticipantClick(personId: number): void {
    this.viewPerson.emit(personId);
  }

  /**
   * Handle related recipe click
   */
  onRecipeClick(recipeId: number): void {
    this.viewRecipe.emit(recipeId);
  }

  /**
   * Handle related story click
   */
  onStoryClick(storyId: number): void {
    this.viewStory.emit(storyId);
  }

  /**
   * Handle create calendar event action
   */
  onCreateCalendarEvent(): void {
    this.createCalendarEvent.emit(this.tradition.id);
  }

  /**
   * Open media lightbox
   */
  openMediaLightbox(index: number): void {
    this.selectedMediaIndex = index;
  }

  /**
   * Close media lightbox
   */
  closeMediaLightbox(): void {
    this.selectedMediaIndex = -1;
  }

  /**
   * Navigate to next media in lightbox
   */
  nextMedia(): void {
    if (this.selectedMediaIndex < this.tradition.media.length - 1) {
      this.selectedMediaIndex++;
    }
  }

  /**
   * Navigate to previous media in lightbox
   */
  previousMedia(): void {
    if (this.selectedMediaIndex > 0) {
      this.selectedMediaIndex--;
    }
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
      return 'Year-round';
    }
    
    const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 
                        'July', 'August', 'September', 'October', 'November', 'December'];
    const months = this.tradition.monthsActive.map(m => monthNames[m - 1]);
    
    if (months.length === 12) {
      return 'Year-round';
    }
    
    if (months.length > 3) {
      return `${months.slice(0, 3).join(', ')}, and ${months.length - 3} more`;
    }
    
    return months.join(', ');
  }

  /**
   * Get years active display
   */
  getYearsActiveDisplay(): string {
    if (!this.tradition.startedYear) {
      return 'Unknown';
    }
    const currentYear = new Date().getFullYear();
    const years = currentYear - this.tradition.startedYear;
    return `${years} years (since ${this.tradition.startedYear})`;
  }

  /**
   * Get photo media items
   */
  getPhotos(): StoryMedia[] {
    return this.tradition.media.filter(m => m.type === MediaType.Photo);
  }

  /**
   * Get video media items
   */
  getVideos(): StoryMedia[] {
    return this.tradition.media.filter(m => m.type === MediaType.Video);
  }

  /**
   * Get participants by role
   */
  getParticipantsByRole(role: 'host' | 'organizer' | 'participant'): TraditionParticipant[] {
    return this.participants.filter(p => p.participantRole === role);
  }

  /**
   * Get all participants without specific role
   */
  getGeneralParticipants(): TraditionParticipant[] {
    return this.participants.filter(p => !p.participantRole || p.participantRole === 'participant');
  }

  /**
   * Format date for display
   */
  formatDate(date: Date): string {
    const d = new Date(date);
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }

  /**
   * Get next occurrence display
   */
  getNextOccurrenceDisplay(): string {
    if (!this.nextOccurrence) {
      return 'No upcoming occurrence scheduled';
    }
    return this.formatDate(this.nextOccurrence.occurrenceDate);
  }

  /**
   * Check if tradition has media
   */
  hasMedia(): boolean {
    return this.tradition.media && this.tradition.media.length > 0;
  }

  /**
   * Check if tradition has related content
   */
  hasRelatedContent(): boolean {
    return (this.relatedRecipes && this.relatedRecipes.length > 0) ||
           (this.relatedStories && this.relatedStories.length > 0);
  }

  /**
   * Check if tradition has calendar information
   */
  hasCalendarInfo(): boolean {
    return !!(this.nextOccurrence || (this.pastOccurrences && this.pastOccurrences.length > 0));
  }
}
