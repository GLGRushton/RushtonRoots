import { Component, Input, OnInit } from '@angular/core';
import { Story, ContentCategory, ContentSearchFilters, ContentType } from '../../models/content.model';
import { StoryComment, RelatedStory } from '../story-details/story-details.component';

/**
 * StoryIndexComponent
 * Container component for story listing and detail views
 * Integrates ContentGridComponent for listing and StoryDetailsComponent for detail view
 */
@Component({
  selector: 'app-story-index',
  standalone: false,
  templateUrl: './story-index.component.html',
  styleUrls: ['./story-index.component.scss']
})
export class StoryIndexComponent implements OnInit {
  /**
   * List of all stories
   */
  @Input() stories: Story[] = [];

  /**
   * Story categories
   */
  @Input() categories: ContentCategory[] = [];

  /**
   * Featured stories
   */
  @Input() featuredStories: Story[] = [];

  /**
   * Recent stories
   */
  @Input() recentStories: Story[] = [];

  /**
   * Whether the user can edit stories
   */
  @Input() canEdit: boolean = false;

  /**
   * Story ID from query parameter
   */
  @Input() storyId: number = 0;

  /**
   * Story slug from query parameter
   */
  @Input() slug: string = '';

  /**
   * Category filter from query parameter
   */
  @Input() categoryFilter: string = '';

  /**
   * Current view mode
   */
  viewMode: 'list' | 'detail' = 'list';

  /**
   * Currently selected story
   */
  selectedStory: Story | null = null;

  /**
   * Comments for the selected story
   */
  storyComments: StoryComment[] = [];

  /**
   * Related stories for the selected story
   */
  relatedStories: RelatedStory[] = [];

  /**
   * User interaction flags
   */
  hasLiked: boolean = false;
  hasFavorited: boolean = false;

  /**
   * Search filters
   */
  searchFilters: ContentSearchFilters = {
    searchText: '',
    contentType: ContentType.Story
  };

  /**
   * Breadcrumb items
   */
  breadcrumbItems: Array<{ label: string; url: string }> = [];

  /**
   * ContentType enum reference for use in template
   */
  readonly ContentType = ContentType;

  ngOnInit(): void {
    // Determine view mode based on query parameters
    if (this.storyId || this.slug) {
      this.viewMode = 'detail';
      this.loadStoryDetail();
    } else {
      this.viewMode = 'list';
      this.buildListBreadcrumbs();
    }

    // Apply category filter if provided
    if (this.categoryFilter) {
      const category = this.categories.find(c => c.slug === this.categoryFilter);
      if (category) {
        this.searchFilters.categoryId = category.id;
      }
    }
  }

  /**
   * Load story details
   */
  loadStoryDetail(): void {
    // Find story by ID or slug
    if (this.storyId) {
      this.selectedStory = this.stories.find(s => s.id === this.storyId) || null;
    } else if (this.slug) {
      this.selectedStory = this.stories.find(s => s.slug === this.slug) || null;
    }

    if (this.selectedStory) {
      this.buildDetailBreadcrumbs();
      this.loadComments();
      this.loadRelatedStories();
      this.checkUserInteractions();
    }
  }

  /**
   * Load comments for the selected story
   */
  loadComments(): void {
    if (!this.selectedStory) return;

    // TODO: Fetch from API
    // For now, using mock data
    this.storyComments = [];
  }

  /**
   * Load related stories
   */
  loadRelatedStories(): void {
    if (!this.selectedStory) return;

    this.relatedStories = [];

    // Find stories from same time period (within 5 years)
    if (this.selectedStory.dateOfEvent) {
      const eventYear = new Date(this.selectedStory.dateOfEvent).getFullYear();
      this.stories
        .filter(s => 
          s.id !== this.selectedStory!.id &&
          s.dateOfEvent &&
          Math.abs(new Date(s.dateOfEvent).getFullYear() - eventYear) <= 5
        )
        .slice(0, 3)
        .forEach(s => {
          this.relatedStories.push({
            id: s.id,
            title: s.title,
            summary: s.summary,
            imageUrl: s.imageUrl,
            dateOfEvent: s.dateOfEvent,
            relationType: 'same-time-period'
          });
        });
    }

    // Find stories about same people
    if (this.selectedStory.relatedPeople.length > 0) {
      const personIds = this.selectedStory.relatedPeople.map(p => p.personId);
      this.stories
        .filter(s => 
          s.id !== this.selectedStory!.id &&
          s.relatedPeople.some(p => personIds.includes(p.personId))
        )
        .slice(0, 3)
        .forEach(s => {
          this.relatedStories.push({
            id: s.id,
            title: s.title,
            summary: s.summary,
            imageUrl: s.imageUrl,
            dateOfEvent: s.dateOfEvent,
            relationType: 'same-people'
          });
        });
    }

    // Find stories from same location
    if (this.selectedStory.location) {
      this.stories
        .filter(s => 
          s.id !== this.selectedStory!.id &&
          s.location === this.selectedStory!.location
        )
        .slice(0, 3)
        .forEach(s => {
          this.relatedStories.push({
            id: s.id,
            title: s.title,
            summary: s.summary,
            imageUrl: s.imageUrl,
            dateOfEvent: s.dateOfEvent,
            relationType: 'same-location'
          });
        });
    }
  }

  /**
   * Check user interactions (likes, favorites)
   */
  checkUserInteractions(): void {
    // TODO: Check from API or local storage
    this.hasLiked = false;
    this.hasFavorited = false;
  }

  /**
   * Build breadcrumbs for list view
   */
  buildListBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Home', url: '/' },
      { label: 'Stories', url: '/StoryView' }
    ];

    if (this.categoryFilter) {
      const category = this.categories.find(c => c.slug === this.categoryFilter);
      if (category) {
        this.breadcrumbItems.push({ label: category.name, url: `/StoryView?category=${category.slug}` });
      }
    }
  }

  /**
   * Build breadcrumbs for detail view
   */
  buildDetailBreadcrumbs(): void {
    this.breadcrumbItems = [
      { label: 'Home', url: '/' },
      { label: 'Stories', url: '/StoryView' }
    ];

    if (this.selectedStory?.categoryName) {
      this.breadcrumbItems.push({ 
        label: this.selectedStory.categoryName, 
        url: `/StoryView?category=${this.selectedStory.slug}` 
      });
    }

    if (this.selectedStory) {
      this.breadcrumbItems.push({ 
        label: this.selectedStory.title, 
        url: `/StoryView?storyId=${this.selectedStory.id}` 
      });
    }
  }

  /**
   * Handle viewing a story
   */
  onViewStory(storyId: number): void {
    window.location.href = `/StoryView?storyId=${storyId}`;
  }

  /**
   * Handle editing a story
   */
  onEditStory(storyId: number): void {
    window.location.href = `/StoryView/Edit/${storyId}`;
  }

  /**
   * Handle deleting a story
   */
  onDeleteStory(storyId: number): void {
    if (confirm('Are you sure you want to delete this story?')) {
      window.location.href = `/StoryView/Delete/${storyId}`;
    }
  }

  /**
   * Handle viewing a person
   */
  onViewPerson(personId: number): void {
    window.location.href = `/Person/Details/${personId}`;
  }

  /**
   * Handle share story
   */
  onShareStory(storyId: number): void {
    const url = `${window.location.origin}/StoryView?storyId=${storyId}`;
    navigator.clipboard.writeText(url).then(() => {
      alert('Story link copied to clipboard!');
    });
  }

  /**
   * Handle print story
   */
  onPrintStory(storyId: number): void {
    // Print is handled by the StoryDetailsComponent
  }

  /**
   * Handle like story
   */
  onLikeStory(storyId: number): void {
    // TODO: Call API to like story
    this.hasLiked = !this.hasLiked;
    console.log('Like story:', storyId);
  }

  /**
   * Handle favorite story
   */
  onFavoriteStory(storyId: number): void {
    // TODO: Call API to favorite story
    this.hasFavorited = !this.hasFavorited;
    console.log('Favorite story:', storyId);
  }

  /**
   * Handle comment submission
   */
  onSubmitComment(data: { storyId: number, comment: string, parentCommentId?: number }): void {
    // TODO: Call API to submit comment
    console.log('Submit comment:', data);
    
    // Mock: Add comment to list
    const newComment: StoryComment = {
      id: this.storyComments.length + 1,
      storyId: data.storyId,
      userId: 'current-user',
      userName: 'Current User',
      comment: data.comment,
      createdDate: new Date(),
      parentCommentId: data.parentCommentId
    };
    this.storyComments.push(newComment);
  }

  /**
   * Handle category change
   */
  onCategoryChange(categoryId: number): void {
    const category = this.categories.find(c => c.id === categoryId);
    if (category) {
      window.location.href = `/StoryView?category=${category.slug}`;
    }
  }

  /**
   * Handle search
   */
  onSearch(filters: ContentSearchFilters): void {
    this.searchFilters = { ...filters, contentType: ContentType.Story };
  }

  /**
   * Get filtered stories
   */
  getFilteredStories(): Story[] {
    let filtered = [...this.stories];

    // Apply search text
    if (this.searchFilters.searchText) {
      const searchLower = this.searchFilters.searchText.toLowerCase();
      filtered = filtered.filter(s => 
        s.title.toLowerCase().includes(searchLower) ||
        s.summary.toLowerCase().includes(searchLower) ||
        s.content.toLowerCase().includes(searchLower)
      );
    }

    // Apply category filter
    if (this.searchFilters.categoryId) {
      filtered = filtered.filter(s => s.categoryId === this.searchFilters.categoryId);
    }

    // Apply tags filter
    if (this.searchFilters.tags && this.searchFilters.tags.length > 0) {
      filtered = filtered.filter(s => 
        s.tags.some(tag => this.searchFilters.tags!.includes(tag))
      );
    }

    // Apply status filter
    if (this.searchFilters.status) {
      filtered = filtered.filter(s => s.status === this.searchFilters.status);
    }

    // Apply featured filter
    if (this.searchFilters.featured !== undefined) {
      filtered = filtered.filter(s => s.featured === this.searchFilters.featured);
    }

    return filtered;
  }
}
