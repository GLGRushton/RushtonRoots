import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Story, StoryMedia, StoryPerson, MediaType } from '../../models/content.model';

/**
 * Comment for display in story
 */
export interface StoryComment {
  id: number;
  storyId: number;
  userId: string;
  userName: string;
  userAvatar?: string;
  comment: string;
  createdDate: Date;
  updatedDate?: Date;
  parentCommentId?: number;
  replies?: StoryComment[];
}

/**
 * Related story for display
 */
export interface RelatedStory {
  id: number;
  title: string;
  summary: string;
  imageUrl?: string;
  dateOfEvent?: Date;
  relationType: 'same-time-period' | 'same-people' | 'same-location';
}

/**
 * StoryDetailsComponent
 * Displays full story details with rich content, media, comments, and related stories
 */
@Component({
  selector: 'app-story-details',
  standalone: false,
  templateUrl: './story-details.component.html',
  styleUrls: ['./story-details.component.scss']
})
export class StoryDetailsComponent implements OnInit {
  /**
   * Story to display
   */
  @Input() story!: Story;

  /**
   * Comments on this story
   */
  @Input() comments: StoryComment[] = [];

  /**
   * Related stories
   */
  @Input() relatedStories: RelatedStory[] = [];

  /**
   * Whether the user can edit this story
   */
  @Input() canEdit: boolean = false;

  /**
   * Whether the user has liked this story
   */
  @Input() hasLiked: boolean = false;

  /**
   * Whether the user has favorited this story
   */
  @Input() hasFavorited: boolean = false;

  /**
   * Event emitted when user wants to edit the story
   */
  @Output() editStory = new EventEmitter<number>();

  /**
   * Event emitted when user wants to delete the story
   */
  @Output() deleteStory = new EventEmitter<number>();

  /**
   * Event emitted when user clicks share button
   */
  @Output() shareStory = new EventEmitter<number>();

  /**
   * Event emitted when user clicks print button
   */
  @Output() printStory = new EventEmitter<number>();

  /**
   * Event emitted when user likes the story
   */
  @Output() likeStory = new EventEmitter<number>();

  /**
   * Event emitted when user favorites the story
   */
  @Output() favoriteStory = new EventEmitter<number>();

  /**
   * Event emitted when user submits a comment
   */
  @Output() submitComment = new EventEmitter<{ storyId: number, comment: string, parentCommentId?: number }>();

  /**
   * Event emitted when user clicks a related person
   */
  @Output() viewPerson = new EventEmitter<number>();

  /**
   * Event emitted when user clicks a related story
   */
  @Output() viewRelatedStory = new EventEmitter<number>();

  /**
   * Current selected tab
   */
  selectedTab: number = 0;

  /**
   * Whether print view is active
   */
  isPrintView: boolean = false;

  /**
   * New comment text
   */
  newComment: string = '';

  /**
   * Reply comment ID (if replying to a comment)
   */
  replyToCommentId: number | null = null;

  /**
   * Reply comment text
   */
  replyText: string = '';

  /**
   * Selected media for lightbox
   */
  selectedMediaIndex: number = -1;

  /**
   * Media types enum for template
   */
  MediaType = MediaType;

  ngOnInit(): void {
    // Initialize component
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
    if (confirm('Are you sure you want to delete this story?')) {
      this.deleteStory.emit(this.story.id);
    }
  }

  /**
   * Handle share action
   */
  onShare(): void {
    this.shareStory.emit(this.story.id);
    // Copy URL to clipboard
    const url = window.location.href;
    navigator.clipboard.writeText(url).then(() => {
      alert('Story link copied to clipboard!');
    });
  }

  /**
   * Handle print action
   */
  onPrint(): void {
    this.isPrintView = true;
    this.printStory.emit(this.story.id);
    setTimeout(() => {
      window.print();
      this.isPrintView = false;
    }, 100);
  }

  /**
   * Handle like action
   */
  onLike(): void {
    this.likeStory.emit(this.story.id);
  }

  /**
   * Handle favorite action
   */
  onFavorite(): void {
    this.favoriteStory.emit(this.story.id);
  }

  /**
   * Handle comment submission
   */
  onSubmitComment(): void {
    if (this.newComment.trim()) {
      this.submitComment.emit({
        storyId: this.story.id,
        comment: this.newComment.trim()
      });
      this.newComment = '';
    }
  }

  /**
   * Handle reply to comment
   */
  onReplyToComment(commentId: number): void {
    this.replyToCommentId = commentId;
    this.replyText = '';
  }

  /**
   * Handle reply submission
   */
  onSubmitReply(): void {
    if (this.replyText.trim() && this.replyToCommentId) {
      this.submitComment.emit({
        storyId: this.story.id,
        comment: this.replyText.trim(),
        parentCommentId: this.replyToCommentId
      });
      this.replyText = '';
      this.replyToCommentId = null;
    }
  }

  /**
   * Cancel reply
   */
  onCancelReply(): void {
    this.replyToCommentId = null;
    this.replyText = '';
  }

  /**
   * Handle person click
   */
  onPersonClick(personId: number): void {
    this.viewPerson.emit(personId);
  }

  /**
   * Handle related story click
   */
  onRelatedStoryClick(storyId: number): void {
    this.viewRelatedStory.emit(storyId);
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
    if (this.selectedMediaIndex < this.story.media.length - 1) {
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
   * Get formatted event date
   */
  getEventDate(): string {
    if (!this.story.dateOfEvent) {
      return '';
    }
    const date = new Date(this.story.dateOfEvent);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  }

  /**
   * Get photo media items
   */
  getPhotos(): StoryMedia[] {
    return this.story.media.filter(m => m.type === MediaType.Photo);
  }

  /**
   * Get video media items
   */
  getVideos(): StoryMedia[] {
    return this.story.media.filter(m => m.type === MediaType.Video);
  }

  /**
   * Get audio media items
   */
  getAudioClips(): StoryMedia[] {
    return this.story.media.filter(m => m.type === MediaType.Audio);
  }

  /**
   * Get related stories by time period
   */
  getRelatedByTimePeriod(): RelatedStory[] {
    return this.relatedStories.filter(s => s.relationType === 'same-time-period');
  }

  /**
   * Get related stories by people
   */
  getRelatedByPeople(): RelatedStory[] {
    return this.relatedStories.filter(s => s.relationType === 'same-people');
  }

  /**
   * Get related stories by location
   */
  getRelatedByLocation(): RelatedStory[] {
    return this.relatedStories.filter(s => s.relationType === 'same-location');
  }

  /**
   * Get top-level comments (not replies)
   */
  getTopLevelComments(): StoryComment[] {
    return this.comments.filter(c => !c.parentCommentId);
  }

  /**
   * Get replies for a comment
   */
  getReplies(commentId: number): StoryComment[] {
    return this.comments.filter(c => c.parentCommentId === commentId);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date): string {
    const d = new Date(date);
    return d.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }
}
